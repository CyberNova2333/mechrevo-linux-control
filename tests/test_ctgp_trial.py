"""Offline trial rollback review: no /dev/mem, fork, GPU load or service calls."""
import importlib.util
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

spec = importlib.util.spec_from_file_location('ctgp_trial', Path(__file__).resolve().parents[1] /
                                            'research/custom-gpu-20261115/ctgp-trial.py')
trial = importlib.util.module_from_spec(spec)
spec.loader.exec_module(trial)


class EC:
    def __init__(self):
        self.values = {r: 0 for r in (*trial.MASKS, *trial.OBSERVE)}
        self.values.update({0x751: 0x30, 0x727: 0x80, 0x741: 0x80, 0x7cc: 0x81})
        self.writes = []

    def read(self, r):
        return self.values[r]

    def write_byte(self, r, b):
        self.writes.append((r, b))
        self.values[r] = b


class RollbackTest(unittest.TestCase):
    def setUp(self):
        self.ec = EC()
        self.before = dict(self.ec.values)
        self.ownership = trial.Ownership.initial(self.before)
        targets = {0x751: 0, 0x726: 128, 0x727: 64, 0x743: 5, 0x744: 35}
        for r, v in targets.items():
            self.ec.values[r] = (self.ec.values[r] & ~trial.MASKS[r]) | v
            self.ownership.confirmed[r] = v

    def restore(self):
        return trial.restore(self.ec, self.before, self.ownership, lambda _: None)

    def test_full_restore_disables_gpu_before_old_target_and_preserves_unowned_bits(self):
        self.ec.values[0x743] |= 0x80
        self.assertTrue(self.restore()['complete'])
        self.assertEqual(self.ec.writes[0], (0x743, 0x80))
        self.assertEqual(self.ec.values[0x727], 0x80)
        self.assertEqual(self.ec.values[0x751], 0x30)
        self.assertEqual(self.ec.values[0x744], 0)
        self.assertFalse(any(r in (0x741, 0x7ab, 0x7c5) for r, _ in self.ec.writes))

    def test_mode_key_wins_while_our_gpu_request_is_removed(self):
        self.ec.values.update({0x751: 0xa0, 0x726: 0, 0x727: 0x80})
        result = self.restore()
        self.assertTrue(result['mode_key_preserved'])
        self.assertEqual(self.ec.values[0x751], 0xa0)
        self.assertEqual(self.ec.values[0x743], 0)
        self.assertEqual(self.ec.values[0x744], 0)
        self.assertFalse(any(r in trial.MODE_REGS for r, _ in self.ec.writes))

    def test_external_appe_owner_prevents_all_writes(self):
        self.ec.values[0x741] |= 1
        self.assertTrue(self.restore()['external_appe_preserved'])
        self.assertEqual(self.ec.writes, [])

    def test_foreign_gpu_target_relinquishes_entire_gpu_group(self):
        self.ec.values[0x744] = 12
        result = self.restore()
        self.assertFalse(result['complete'])
        self.assertFalse(any(r in (0x743, 0x744, 0x745, 0x746) for r, _ in self.ec.writes))
        self.assertFalse(any(r in trial.MODE_REGS for r, _ in self.ec.writes))
        self.assertEqual(self.ec.values[0x744], 12)

    def test_parent_dies_between_intent_and_write_old_target_remains_restorable(self):
        self.ownership.confirmed[0x744] = 50
        self.ownership.pending = (0x744, 35)
        self.ec.values[0x744] = 50
        self.assertTrue(self.restore()['complete'])
        self.assertEqual(self.ec.values[0x744], 0)

    def test_parent_dies_after_pending_write_before_readback(self):
        self.ownership.confirmed[0x744] = 50
        self.ownership.pending = (0x744, 35)
        self.assertTrue(self.restore()['complete'])
        self.assertEqual(self.ec.values, self.before)

    def test_old_confirmed_target_is_not_reclaimed_after_new_confirmation(self):
        self.ec.values[0x744] = 50  # Earlier phase, now external after 35 was confirmed.
        self.assertFalse(self.restore()['complete'])
        self.assertEqual(self.ec.writes, [])
        self.assertEqual(self.ec.values[0x744], 50)

    def test_appe_takeover_mid_restore_stops_all_remaining_writes(self):
        original = self.ec.write_byte
        def write(r, b):
            original(r, b)
            if r == 0x743:
                self.ec.values[0x741] |= 1
        self.ec.write_byte = write
        result = self.restore()
        self.assertTrue(result['external_appe_preserved'])
        self.assertEqual(self.ec.writes, [(0x743, 0)])
        self.assertEqual(self.ec.values[0x744], 35)
        self.assertEqual(self.ec.values[0x751], 0)

    def test_mode_key_mid_restore_is_not_overwritten_by_cached_group_state(self):
        original = self.ec.write_byte
        def write(r, b):
            original(r, b)
            if r == 0x726:
                self.ec.values.update({0x751: 0xa0, 0x727: 0x80})
        self.ec.write_byte = write
        result = self.restore()
        self.assertTrue(result['mode_key_preserved'])
        self.assertEqual(self.ec.values[0x751], 0xa0)
        self.assertFalse(any(r in (0x727, 0x751) for r, _ in self.ec.writes))

    def test_gpu_handoff_mid_restore_preserves_remaining_gpu_and_mode_fields(self):
        original = self.ec.write_byte
        def write(r, b):
            original(r, b)
            if r == 0x743:
                self.ec.values[0x744] = 50
        self.ec.write_byte = write
        self.assertFalse(self.restore()['complete'])
        self.assertEqual(self.ec.writes, [(0x743, 0)])
        self.assertEqual(self.ec.values[0x744], 50)
        self.assertEqual(self.ec.values[0x751], 0)

    def test_partial_restoration_retries_without_losing_ownership(self):
        original = self.ec.write_byte
        fail_once = [True]
        def write(r, b):
            if r == 0x744 and fail_once[0]:
                fail_once[0] = False
                raise OSError('transient')
            original(r, b)
        self.ec.write_byte = write
        self.assertFalse(self.restore()['complete'])
        self.assertTrue(self.restore()['complete'])
        self.assertEqual(self.ec.values, self.before)

    def test_mask_validation_and_exact_round_adapter_rule(self):
        with self.assertRaises(ValueError):
            trial.write_mask(self.ec, 0x743, 8, lambda _: None)
        self.assertFalse(self.ec.writes)
        for flags in range(256):
            for fallback in (0, 1):
                self.ec.values.update({0x7cc: flags, 0x490: fallback})
                self.assertEqual(trial.round_adapter(self.ec.read), bool(flags & 1 or not flags & 15 and fallback))

    def guard(self, eof):
        payload = json.dumps(self.ownership.payload()).encode() + b'\n'
        blocks = [payload, b'' if eof else b'not-json\n']
        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory) / 'guard.json'
            with (patch.object(trial.signal, 'signal'), patch.object(trial.os, 'read', side_effect=blocks),
                  patch.object(trial.select, 'select', return_value=([9], [], [])),
                  patch.object(trial.os, 'kill') as kill_parent,
                  patch.object(trial.os, 'killpg') as kill_load):
                code = trial.guard_child(self.ec, self.before, 9, 54321, 12345, output)
                calls = (kill_parent.call_args_list, kill_load.call_args_list)
            return code, json.loads(output.read_text()), calls

    def test_guard_eof_restores_without_signalling_normal_parent(self):
        code, result, calls = self.guard(True)
        self.assertEqual(code, 0)
        self.assertTrue(result['parent_eof'])
        self.assertEqual(calls[0], [])
        self.assertEqual(self.ec.values, self.before)

    def test_guard_monitor_failure_still_stops_parent_before_rollback(self):
        code, result, calls = self.guard(False)
        self.assertEqual(code, 0)
        self.assertFalse(result['parent_eof'])
        self.assertIsNotNone(result['monitor_error'])
        self.assertEqual(calls[0][0].args, (12345, trial.signal.SIGKILL))
        self.assertEqual(self.ec.values, self.before)

    def test_all_restore_io_positions_before_and_after_acceptance_retry_cleanly(self):
        def inject(position, after):
            self.setUp()
            read, write = self.ec.read, self.ec.write_byte
            counter = [0]
            def operation(function, *args):
                counter[0] += 1
                fail = counter[0] == position
                if fail and not after:
                    raise OSError('injected before')
                result = function(*args)
                if fail:
                    raise OSError('injected after')
                return result
            self.ec.read = lambda reg: operation(read, reg)
            self.ec.write_byte = lambda reg, value: operation(write, reg, value)
            result = self.restore()
            count = counter[0]
            if not result['complete']:
                result = self.restore()
            self.assertTrue(result['complete'], (position, after, result))
            self.assertEqual(self.ec.values, self.before, (position, after))
            return count
        total = inject(0, False)
        self.assertGreater(total, 50)
        for position in range(1, total + 1):
            for after in (False, True):
                with self.subTest(position=position, after=after):
                    inject(position, after)


class ProtocolTest(unittest.TestCase):
    def make_transaction(self):
        ec = EC()
        before = dict(ec.values)
        transaction = trial.Transaction(ec, before, 777)
        return ec, before, transaction

    def test_each_actual_write_sends_pending_before_and_discards_previous_after_readback(self):
        ec, _, tx = self.make_transaction()
        messages = []
        def send(_fd, payload):
            messages.append(json.loads(payload))
            return len(payload)
        with patch.object(trial.os, 'write', side_effect=send):
            tx.put(0x744, 50)
            tx.put(0x744, 35)
        states = [trial.Ownership.parse(m) for m in messages]
        self.assertEqual([s.accepts(0x744) for s in states], [{0, 50}, {50}, {50, 35}, {35}])
        self.assertEqual(states[-1].pending, None)
        self.assertEqual(ec.values[0x744], 35)

    def test_error_after_write_keeps_only_pending_and_forbids_another_mutation(self):
        ec, before, tx = self.make_transaction()
        messages = []
        def send(_fd, payload):
            messages.append(json.loads(payload))
            return len(payload)
        original = ec.write_byte
        def write(r, b):
            original(r, b)
            raise OSError('failed after acceptance')
        ec.write_byte = write
        with patch.object(trial.os, 'write', side_effect=send):
            with self.assertRaises(OSError):
                tx.put(0x744, 50)
            with self.assertRaisesRegex(RuntimeError, 'unresolved'):
                tx.put(0x744, 35)
        self.assertEqual(len(messages), 1)
        state = trial.Ownership.parse(messages[-1])
        self.assertEqual(state.accepts(0x744), {0, 50})
        ec.write_byte = original
        self.assertTrue(trial.restore(ec, before, state, lambda _: None)['complete'])
        self.assertEqual(ec.values, before)

    def test_protocol_rejects_invalid_registers_masks_and_multiple_pending_writes(self):
        state = trial.Ownership.initial(EC().values)
        for pending in ({'reg': 0x741, 'value': 1}, {'reg': 0x743, 'value': 8},
                        [{'reg': 0x743, 'value': 1}], {'reg': True, 'value': 1}):
            payload = state.payload()
            payload['pending'] = pending
            with self.assertRaises(ValueError):
                trial.Ownership.parse(payload)

if __name__ == '__main__':
    unittest.main()

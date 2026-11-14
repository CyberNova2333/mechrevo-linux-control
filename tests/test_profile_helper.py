import importlib.machinery
import importlib.util
from pathlib import Path
import unittest

path = Path(__file__).resolve().parents[1] / 'system/mechrevo-profile-helper'
loader = importlib.machinery.SourceFileLoader('helper', str(path))
spec = importlib.util.spec_from_loader(loader.name, loader)
helper = importlib.util.module_from_spec(spec)
loader.exec_module(helper)


class FakeEC:
    def __init__(self):
        self.values = dict.fromkeys(helper.REGISTERS, 0)
        self.values.update({0x751: 0x30, 0x706: 0x40, 0x727: 0x80, 0x7E8: 0x10})
        self.writes = []

    def read(self, address):
        return self.values[address]

    def write_flag(self, address, enabled):
        self.writes.append((address, enabled))
        mask = helper.FLAG_MASKS[address]
        self.values[address] = (self.values[address] & ~mask) | (mask if enabled else 0)

    def write_mode(self, value):
        self.writes.append(value)
        self.values[helper.MODE] = value


class HelperTest(unittest.TestCase):
    def test_only_mode_is_changed(self):
        ec = FakeEC()
        original = ec.values.copy()
        for name, target in helper.MODES.items():
            if name == 'custom':
                continue
            self.assertEqual(helper.set_mode(ec, name, pause=lambda _: None)['mode'], name)
            self.assertEqual(ec.values, {**original, helper.MODE: target})

    def test_custom_state_and_overrides_are_rejected(self):
        for address, value in [(0x706, 0x41), (0x726, 0x80), (0x727, 0xC0),
                               (0x7C5, 0x80), (0x783, 1), (0x784, 1),
                               (0x785, 1), (0x786, 1), (0x787, 1), (0x741, 1), (0x751, 0x70), (0x751, 0xFF)]:
            with self.subTest(address=address):
                ec = FakeEC()
                ec.values[address] = value
                with self.assertRaises(RuntimeError):
                    helper.set_mode(ec, 'office', pause=lambda _: None)
                self.assertEqual(ec.writes, [])

    def test_intervening_key_is_not_overwritten(self):
        ec = FakeEC()
        def keypress(_):
            ec.values[helper.MODE] = 0x20
        with self.assertRaises(RuntimeError):
            helper.set_mode(ec, 'office', pause=keypress)
        self.assertEqual(ec.values[helper.MODE], 0x20)
        self.assertEqual(ec.writes, [0xA0])

    def test_error_rolls_back_our_own_write(self):
        ec = FakeEC()
        def fail(_):
            raise OSError('injected read failure')
        with self.assertRaises(OSError):
            helper.set_mode(ec, 'office', pause=fail)
        self.assertEqual(ec.writes, [0xA0, 0x30])

    def test_custom_entry_exit_preserves_unrelated_bits(self):
        for destination in ('office', 'balanced', 'turbo'):
            ec = FakeEC(); ec.values[0x726] = 5; ec.values[0x727] = 0x83
            ec.values[0x7AB] = 0xE1  # Logo bits are never written.
            original = ec.values.copy()
            result = helper.set_mode(ec, 'custom', pause=lambda _: None)
            self.assertEqual(result['mode'], 'custom')
            self.assertEqual(ec.values, {**original, 0x751: 0, 0x726: 0x85, 0x727: 0xC3})
            result = helper.set_mode(ec, destination, pause=lambda _: None)
            self.assertEqual(ec.values, {**original, 0x751: helper.MODES[destination]})

    def test_custom_partial_failure_restores_every_owned_flag(self):
        for fail_step in (1, 2, 3):
            ec = FakeEC(); original = ec.values.copy(); count = [0]
            mode_writer, flag_writer = ec.write_mode, ec.write_flag
            def inject(fn):
                def wrapped(*args):
                    count[0] += 1
                    fn(*args)  # Simulate error after the device accepted the write.
                    if count[0] == fail_step:
                        raise OSError('injected partial write')
                return wrapped
            ec.write_mode, ec.write_flag = inject(mode_writer), inject(flag_writer)
            with self.assertRaises(OSError):
                helper.set_mode(ec, 'custom', pause=lambda _: None)
            self.assertEqual(ec.values, original)

    def test_custom_rollback_does_not_undo_external_mode_key(self):
        ec = FakeEC()
        def keypress(_):
            ec.values[0x751] = 0xA0
            ec.values[0x726] &= ~0x80; ec.values[0x727] &= ~0x40
        with self.assertRaises(RuntimeError):
            helper.set_mode(ec, 'custom', pause=keypress)
        self.assertEqual(ec.values[0x751], 0xA0)
        self.assertEqual(ec.values[0x726] & 0x80, 0)
        self.assertEqual(ec.values[0x727] & 0x40, 0)

    def test_noop_custom_does_not_write_again(self):
        ec = FakeEC(); helper.set_mode(ec, 'custom', pause=lambda _: None)
        ec.writes.clear(); helper.set_mode(ec, 'custom', pause=lambda _: None)
        self.assertEqual(ec.writes, [])

    def test_oem_aliases_do_not_confuse_balanced_and_custom(self):
        ec = FakeEC(); ec.values[0x751] = 0x10
        self.assertTrue(helper.standard_state(ec.values))
        self.assertEqual(helper.status(ec)['mode'], 'turbo')
        ec.values[0x751] = 0
        self.assertEqual(helper.status(ec)['mode'], 'balanced')
        helper.set_mode(ec, 'custom', pause=lambda _: None)
        self.assertEqual(helper.status(ec)['mode'], 'custom')
        helper.set_mode(ec, 'balanced', pause=lambda _: None)
        self.assertEqual(helper.status(ec)['mode'], 'balanced')
        self.assertEqual(ec.values[0x727], 0x80)

    def test_invalid_actions_fail_before_hardware_access(self):
        for args in [[], ['set', 'quiet'], ['set', 'turbo', 'extra'], ['read', '0x751']]:
            with self.assertRaises(ValueError):
                helper.main(args)


if __name__ == '__main__':
    unittest.main()

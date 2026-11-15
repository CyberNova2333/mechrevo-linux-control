"""Behavioral tests from restored OEM code; no device access or GUI required."""
import sys
from pathlib import Path
import unittest
from unittest.mock import Mock

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "iteglow/src"))
from iteglow.oem_scheduler import (
    ACK_OFF, ACK_ON, LoadSample, Operation as Op, SchedulerState,
    WorkloadScheduler, advance, round_adapter_present,
)

REPLIES = {Op.COMBINED_ON: ACK_ON, Op.COMBINED_OFF: ACK_OFF}


def tick(state, cpu, gpu, *, game=True, replies=REPLIES):
    return advance(state, LoadSample(cpu, gpu), replies,
                   round_adapter=True, game_supported=game)


class OemSchedulerTest(unittest.TestCase):
    def test_startup_idle_cleans_unknown_state_once(self):
        result = tick(SchedulerState(), 0, 0)
        self.assertEqual(result.operations, (Op.COMBINED_OFF, Op.GAME_OFF))
        self.assertEqual(result.state.cpu_count, -1)
        self.assertEqual(result.state.game_launch_condition, 0)
        self.assertEqual(tick(result.state, 0, 0).operations, ())

    def test_strict_thresholds_and_cpu_middle_band(self):
        for cpu, expected in ((9, 1), (10, 0), (95, 0), (96, 3)):
            with self.subTest(cpu=cpu):
                result = tick(SchedulerState(cpu_count=2), cpu, 90, game=False)
                self.assertEqual(result.state.cpu_count, expected)
        for gpu, combined, game in ((50, -1, -1), (51, -1, 1), (90, -1, 1), (91, 1, 1)):
            with self.subTest(gpu=gpu):
                result = tick(SchedulerState(), 50, gpu)
                self.assertEqual((result.state.combined_gpu_count, result.state.game_gpu_count),
                                 (combined, game))

    def test_three_startup_high_samples_and_four_from_idle(self):
        for start, expected_ticks in ((SchedulerState(), 3),
                                      (tick(SchedulerState(), 0, 0).state, 4)):
            state = start
            for number in range(1, expected_ticks + 1):
                result = tick(state, 96, 91)
                self.assertEqual(Op.COMBINED_ON in result.operations, number == expected_ticks)
                state = result.state
            self.assertEqual(tick(state, 100, 100).operations, ())

    def test_gpu_counter_is_not_consecutive_sample_counter(self):
        state = SchedulerState()
        for cpu, gpu in ((96, 91), (96, 91), (96, 90), (96, 91)):
            result = tick(state, cpu, gpu)
            self.assertNotIn(Op.COMBINED_ON, result.operations)
            state = result.state
        self.assertIn(Op.COMBINED_ON, tick(state, 96, 91).operations)

    def test_combined_exit_requires_minus_one_not_zero(self):
        state = SchedulerState(cpu_count=3, combined_gpu_count=3,
                               game_gpu_count=3, combined_reply=ACK_ON)
        for number in range(1, 5):
            result = tick(state, 9, 100)
            self.assertEqual(Op.COMBINED_OFF in result.operations, number == 4)
            state = result.state
        self.assertEqual(result.operations, (Op.COMBINED_OFF, Op.GAME_ON))

    def test_middle_cpu_resets_count_but_does_not_clear_combined_ack(self):
        state = SchedulerState(cpu_count=3, combined_gpu_count=3,
                               game_gpu_count=3, combined_reply=ACK_ON)
        for _ in range(10):
            result = tick(state, 50, 100)
            self.assertEqual(result.operations, ())
            self.assertEqual(result.state.combined_reply, ACK_ON)
            self.assertFalse(result.state.combined_condition)
            state = result.state
        self.assertEqual(tick(state, 9, 100).operations, (Op.COMBINED_OFF, Op.GAME_ON))

    def test_game_is_stopped_before_combined_on(self):
        state = SchedulerState(cpu_count=2, combined_gpu_count=2,
                               game_gpu_count=3, game_on=True)
        result = tick(state, 100, 100)
        self.assertEqual(result.operations, (Op.GAME_OFF, Op.COMBINED_ON))
        self.assertFalse(result.state.game_on)

    def test_failed_combined_on_can_reenter_game_in_same_callback(self):
        state = SchedulerState(cpu_count=2, combined_gpu_count=2,
                               game_gpu_count=3, game_on=True)
        result = tick(state, 100, 100, replies={Op.COMBINED_ON: 0})
        self.assertEqual(result.operations, (Op.GAME_OFF, Op.COMBINED_ON, Op.GAME_ON))
        self.assertTrue(result.state.combined_condition)
        self.assertTrue(result.state.game_on)

    def test_single_gpu_ack_blocks_combined_but_not_game(self):
        state = SchedulerState(cpu_count=2, combined_gpu_count=2,
                               game_gpu_count=2, single_gpu_reply=ACK_ON)
        result = tick(state, 100, 100, replies={})
        self.assertEqual(result.operations, (Op.GAME_ON,))
        self.assertTrue(result.state.combined_condition)

    def test_game_hysteresis_and_unsupported_feature(self):
        state = SchedulerState(game_gpu_count=3, game_on=True, combined_reply=ACK_OFF)
        for number in range(1, 5):
            result = tick(state, 50, 50)
            self.assertEqual(Op.GAME_OFF in result.operations, number == 4)
            state = result.state
        result = tick(SchedulerState(), 0, 0, game=False)
        self.assertEqual(result.operations, (Op.COMBINED_OFF,))
        self.assertEqual(result.state.game_gpu_count, 0)
        self.assertEqual(result.state.game_launch_condition, -1)

    def test_missing_or_failed_ack_is_not_assumed_success(self):
        for reply in (0, 1, 0xFE):
            result = tick(SchedulerState(), 0, 0, replies={Op.COMBINED_OFF: reply})
            self.assertEqual(result.state.combined_reply, reply)
            self.assertEqual(tick(result.state, 0, 0, replies=REPLIES).operations,
                             (Op.COMBINED_OFF,))
        with self.assertRaisesRegex(ValueError, "explicit firmware reply"):
            tick(SchedulerState(), 0, 0, replies={})

    def test_round_adapter_gate_skips_sampling_and_preserves_state(self):
        state = SchedulerState(cpu_count=3, combined_gpu_count=3,
                               game_gpu_count=3, combined_reply=ACK_ON,
                               combined_condition=True)
        model = WorkloadScheduler(game_supported=True, state=state)
        sampler = Mock(side_effect=AssertionError("must not sample on battery"))
        result = model.poll(lambda: False, sampler, {})
        self.assertIs(result.state, state)
        self.assertFalse(result.sampled)
        self.assertEqual(result.operations, ())
        sampler.assert_not_called()

    def test_poll_failure_does_not_commit_partial_counters(self):
        model = WorkloadScheduler(game_supported=True)
        before = model.state
        with self.assertRaises(ValueError):
            model.poll(lambda: True, lambda: LoadSample(0, 0), {})
        self.assertIs(model.state, before)
        result = model.poll(lambda: True, lambda: LoadSample(0, 0), REPLIES)
        self.assertEqual(result.operations, (Op.COMBINED_OFF, Op.GAME_OFF))

    def test_restore_keeps_launch_marker_and_never_issues_operations(self):
        model = WorkloadScheduler(game_supported=True,
                                  state=SchedulerState(cpu_count=3, combined_reply=ACK_ON,
                                                       game_on=True, game_launch_condition=0))
        model.restore_parameters()
        self.assertEqual(model.state, SchedulerState(game_launch_condition=0))

    def test_ec_round_socket_precedence_and_low_nibble_fallback(self):
        for port, legacy, expected in ((1, 0, True), (3, 0, True), (0, 1, True),
                                       (0x10, 1, True), (2, 1, False), (8, 1, False),
                                       (0, 0, False), (0xF0, 2, False)):
            with self.subTest(port=port, legacy=legacy):
                self.assertEqual(round_adapter_present(port, legacy), expected)

    def test_invalid_samples_and_replies_are_rejected(self):
        for invalid in (-1, 101, 95.5, True, None):
            with self.subTest(invalid=invalid):
                with self.assertRaises(ValueError):
                    LoadSample(invalid, 0)
        for invalid in (-1, 256, True):
            with self.subTest(reply=invalid):
                with self.assertRaises(ValueError):
                    tick(SchedulerState(), 0, 0, replies={Op.COMBINED_OFF: invalid})


if __name__ == "__main__":
    unittest.main()

"""Offline safety checks for the recovered EC Logo protocol; never opens /dev/mem."""
import importlib.machinery
import importlib.util
from pathlib import Path
import unittest
from unittest.mock import patch


path = Path(__file__).resolve().parents[1] / 'system/mechrevo-profile-helper'
loader = importlib.machinery.SourceFileLoader('logo_helper', str(path))
spec = importlib.util.spec_from_loader(loader.name, loader)
helper = importlib.util.module_from_spec(spec)
loader.exec_module(helper)


class LogoEC:
    """Byte device with the real mask writer and a self-clearing apply trigger."""
    def __init__(self, fail_write=None, fail_after=True):
        self.fd = -100
        self.values = {0x767: 0x35, 0x769: 12, 0x76a: 23, 0x76b: 34,
                       0x78c: 0xA5, 0x7ab: 0x5F, 0x7c5: 0xD8}
        self.writes = []
        self.reads = []
        self.fail_write = fail_write
        self.fail_after = fail_after

    def read(self, address):
        self.reads.append(address)
        return self.values[address]

    def write_logo(self, address, value):
        with patch.object(helper.os, 'pwrite', side_effect=self._pwrite):
            helper.EC.write_logo(self, address, value)

    def _pwrite(self, fd, data, offset):
        assert fd == self.fd
        address = offset - helper.BASE
        self.writes.append((address, data[0]))
        fail = len(self.writes) == self.fail_write
        if fail and not self.fail_after:
            raise OSError('write rejected')
        self.values[address] = data[0] & ~0x10 if address == 0x78c else data[0]
        if fail:
            raise OSError('write accepted then failed')
        return len(data)


def config(mode='solid', brightness=75, color=None):
    return {'mode': mode, 'brightness': brightness,
            'color': [255, 136, 51] if color is None else color}


class LogoTest(unittest.TestCase):
    def apply(self, ec, settings=None, pause=lambda _: None):
        return helper.set_logo(ec, settings or config(), pause=pause)

    def test_actual_writer_preserves_every_unowned_bit(self):
        for address, mask in {**helper.LOGO_MASKS, 0x78c: 0x10}.items():
            for original in range(256):
                for owned in (0, mask):
                    with self.subTest(address=hex(address), original=original, owned=owned):
                        ec = LogoEC()
                        ec.values[address] = original
                        ec.write_logo(address, owned)
                        written = ec.writes[-1][1]
                        self.assertEqual(written & ~mask, original & ~mask)
                        self.assertEqual(written & mask, owned)

    def test_effects_preserve_performance_battery_and_support_flags(self):
        for mode, color in [('solid', [255, 136, 51]),
                            ('breathing', [0, 255, 255]), ('rainbow', [255, 255, 255])]:
            for high in range(0, 256, 8):
                ec = LogoEC()
                ec.values[0x7c5] = high | 2
                before = ec.values.copy()
                self.apply(ec, config(mode=mode, color=color))
                self.assertEqual(ec.values[0x7c5] & 0xF8, high)
                self.assertEqual(ec.values[0x7ab] & 0x1F, before[0x7ab] & 0x1F)
                self.assertEqual(ec.values[0x767] & 0x7F, before[0x767] & 0x7F)
                self.assertEqual(ec.values[0x78c], before[0x78c])

    def test_off_only_changes_brightness_and_commits(self):
        ec = LogoEC()
        ec.values[0x767] |= 0x80
        ec.values[0x7c5] |= 6
        before = ec.values.copy()
        result = self.apply(ec, config(brightness=0))
        self.assertEqual(ec.values, {**before, 0x7ab: before[0x7ab] & 0x1F})
        self.assertEqual([a for a, _ in ec.writes], [0x7ab, 0x78c])
        self.assertFalse(result['logo_enabled'])

    def test_black_and_sub_quantum_colors_turn_logo_off(self):
        for color in ([0, 0, 0], [5, 5, 5]):
            ec = LogoEC()
            before = ec.values.copy()
            result = self.apply(ec, config(color=color))
            self.assertFalse(result['logo_enabled'])
            self.assertEqual(ec.values[0x7ab] & 0xE0, 0)
            self.assertEqual([ec.values[a] for a in (0x769, 0x76a, 0x76b)],
                             [before[a] for a in (0x769, 0x76a, 0x76b)])

    def test_breathing_presets_match_official_color_numbers(self):
        colors = [(255, 255, 255), (255, 0, 0), (0, 255, 0),
                  (0, 0, 255), (255, 255, 0), (136, 0, 255), (0, 255, 255)]
        for index, color in enumerate(colors, 1):
            ec = LogoEC()
            ec.values[0x767] |= 0x80
            result = self.apply(ec, config('breathing', color=list(color)))
            self.assertEqual(ec.values[0x7c5] & 7, index)
            self.assertEqual(ec.values[0x767] & 0x80, 0)
            self.assertEqual(result['color'], list(color))

    def test_rainbow_clears_breathing_without_rewriting_rgb(self):
        ec = LogoEC()
        ec.values[0x7c5] |= 7
        before = ec.values.copy()
        result = self.apply(ec, config('rainbow'))
        self.assertEqual(ec.values[0x767] & 0x80, 0x80)
        self.assertEqual(ec.values[0x7c5] & 7, 0)
        self.assertEqual(result['mode'], 'rainbow')
        for address in (0x769, 0x76a, 0x76b):
            self.assertEqual(ec.values[address], before[address])

    def test_brightness_and_color_quantization(self):
        for percent in (0, 25, 50, 75, 100):
            ec = LogoEC()
            result = self.apply(ec, config(brightness=percent))
            self.assertEqual(ec.values[0x7ab] >> 5, percent // 25)
            self.assertEqual(result['brightness'], percent)
        for channel in range(256):
            ec = LogoEC()
            self.apply(ec, config(color=[channel, 255, 255]))
            self.assertEqual(ec.values[0x769], min(int(channel / 5.1), 50))

    def test_readback_roundtrip_does_not_dim_static_color(self):
        # The GUI and `logo on` both reuse readback values. Every bucket must survive.
        for level in range(51):
            with self.subTest(level=level):
                ec = LogoEC()
                ec.values[0x769] = level
                ec.values[0x76a] = 50
                ec.values[0x76b] = 50
                state = helper.logo_status(ec)
                self.apply(ec, {key: state[key] for key in ('mode', 'brightness', 'color')})
                self.assertEqual(ec.values[0x769], level)

    def test_failure_at_every_write_restores_owned_state(self):
        for settings, steps in [(config(), 7), (config('breathing', color=[255, 0, 0]), 4),
                                (config('rainbow'), 4), (config(brightness=0), 2)]:
            for step in range(1, steps + 1):
                for accepted in (False, True):
                    with self.subTest(settings=settings, step=step, accepted=accepted):
                        ec = LogoEC(fail_write=step, fail_after=accepted)
                        before = ec.values.copy()
                        with self.assertRaises(OSError):
                            self.apply(ec, settings)
                        self.assertEqual(ec.values, before)

    def test_readback_or_pause_failure_rolls_back(self):
        ec = LogoEC()
        before = ec.values.copy()
        def fail_pause(_):
            raise OSError('settling interrupted')
        with self.assertRaises(OSError):
            self.apply(ec, pause=fail_pause)
        self.assertEqual(ec.values, before)
        ec = LogoEC()
        before = ec.values.copy()
        def fail_one_read(_):
            old_read = ec.read
            def injected(address):
                ec.read = old_read
                raise OSError('readback failed')
            ec.read = injected
        with self.assertRaises(OSError):
            self.apply(ec, pause=fail_one_read)
        self.assertEqual(ec.values, before)

    def test_rollback_keeps_external_owned_changes_and_new_shared_flags(self):
        ec = LogoEC()
        before = ec.values.copy()
        def external_change(_):
            ec.values[0x769] = 7
            ec.values[0x7ab] = 0x21
            ec.values[0x7c5] ^= 0xC0
            ec.values[0x767] ^= 0x01
        with self.assertRaises(RuntimeError):
            self.apply(ec, pause=external_change)
        self.assertEqual(ec.values[0x769], 7)
        self.assertEqual(ec.values[0x7ab], 0x21)
        self.assertEqual(ec.values[0x7c5], before[0x7c5] ^ 0xC0)
        self.assertEqual(ec.values[0x767], before[0x767] ^ 0x01)
        self.assertEqual(ec.values[0x76a], before[0x76a])
        self.assertEqual(ec.values[0x76b], before[0x76b])

    def test_invalid_inputs_fail_before_any_device_access(self):
        malformed = [None, [], {}, {**config(), 'extra': 1},
                     {**config(), 'mode': 'wave'}, {**config(), 'mode': []},
                     {**config(), 'brightness': True}, {**config(), 'brightness': 20},
                     {**config(), 'brightness': 25.0}, {**config(), 'brightness': -25},
                     {**config(), 'brightness': 125}, {**config(), 'color': (1, 2, 3)},
                     {**config(), 'color': [0, 0]}, {**config(), 'color': [0, 0, 256]},
                     {**config(), 'color': [0, -1, 1]}, {**config(), 'color': [0, 1, True]},
                     {**config(), 'color': [0, 1, 1.0]}, config('breathing', color=[136, 0, 254])]
        for settings in malformed:
            with self.subTest(settings=settings):
                ec = LogoEC()
                with self.assertRaises(ValueError):
                    helper.set_logo(ec, settings, pause=lambda _: None)
                self.assertEqual(ec.reads, [])
                self.assertEqual(ec.writes, [])

    def test_unsupported_and_out_of_range_hardware_do_not_write(self):
        for address, value in [(0x7ab, 0), (0x7ab, 0xA1), (0x769, 51)]:
            ec = LogoEC()
            ec.values[address] = value
            with self.assertRaises(RuntimeError):
                self.apply(ec)
            self.assertEqual(ec.writes, [])

    def test_low_level_writer_rejects_other_registers_and_bits(self):
        for address, value in [(0x751, 0), (0x7ab, 1), (0x7c5, 0x80),
                               (0x78c, 1), (0x769, 256), (0x769, -1), (0x769, True)]:
            ec = LogoEC()
            with self.assertRaises(ValueError):
                ec.write_logo(address, value)
            self.assertEqual(ec.writes, [])


if __name__ == '__main__':
    unittest.main()

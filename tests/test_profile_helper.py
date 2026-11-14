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

    def write_mode(self, value):
        self.writes.append(value)
        self.values[helper.MODE] = value


class HelperTest(unittest.TestCase):
    def test_only_mode_is_changed(self):
        ec = FakeEC()
        original = ec.values.copy()
        for name, target in helper.MODES.items():
            self.assertEqual(helper.set_mode(ec, name, pause=lambda _: None)['mode'], name)
            self.assertEqual(ec.values, {**original, helper.MODE: target})

    def test_custom_state_and_overrides_are_rejected(self):
        for address, value in [(0x706, 0x41), (0x726, 0x80), (0x727, 0xC0),
                               (0x7C5, 0x80), (0x783, 1), (0x784, 1),
                               (0x785, 1), (0x751, 0x70), (0x751, 0xFF)]:
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

    def test_invalid_actions_fail_before_hardware_access(self):
        for args in [[], ['set', 'custom'], ['set', 'turbo', 'extra'], ['read', '0x751']]:
            with self.assertRaises(ValueError):
                helper.main(args)


if __name__ == '__main__':
    unittest.main()

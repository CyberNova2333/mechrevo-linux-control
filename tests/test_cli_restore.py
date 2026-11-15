"""The manual restore command must preserve OFF without a power-on pulse."""
import contextlib
import io
from pathlib import Path
import sys
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "iteglow/src"))
from iteglow import cli, protocol


class CliRestoreTest(unittest.TestCase):
    def restore(self, saved):
        device = Mock()
        device.__enter__ = Mock(return_value=device)
        device.__exit__ = Mock(return_value=False)
        with patch.object(cli.state, "load", return_value={"lightbar": saved}), \
             patch.object(cli.devices, "available_devices", return_value=[("led", "usb", "lightbar")]), \
             patch.object(cli, "_open", return_value=device) as opener, \
             contextlib.redirect_stdout(io.StringIO()), contextlib.redirect_stderr(io.StringIO()):
            result = cli.cmd_restore(SimpleNamespace(system=False))
        return result, device, opener

    def test_off_and_zero_brightness_restore_without_enabling_controller(self):
        for saved in ({"mode": "off", "brightness": 50}, {"mode": "wave", "brightness": 0}):
            with self.subTest(saved=saved):
                result, device, _ = self.restore(saved)
                self.assertEqual(result, 0)
                reports = device.send.call_args.args[0]
                self.assertEqual(reports, tuple(protocol.lightbar_off()))
                self.assertNotIn(protocol.lightbar_power_on()[0], reports)

    def test_on_restore_enables_controller_and_keeps_saved_brightness(self):
        result, device, _ = self.restore({"mode": "wave", "brightness": 31})
        self.assertEqual(result, 0)
        reports = device.send.call_args.args[0]
        self.assertEqual(reports[0], protocol.lightbar_power_on()[0])
        self.assertEqual(reports[-1][1][4], 31)

    def test_invalid_state_does_not_open_device(self):
        for saved in ({"mode": "wave", "brightness": 101}, {"mode": "off", "path": "/dev/mem"}, []):
            with self.subTest(saved=saved):
                result, device, opener = self.restore(saved)
                self.assertEqual(result, 1)
                opener.assert_not_called()
                device.send.assert_not_called()

    def test_pin_is_an_export_and_does_not_promise_automatic_system_restore(self):
        output = io.StringIO()
        with patch.object(cli.os, "geteuid", return_value=0), \
             patch.object(cli.state, "load", return_value={"keyboard": {"mode": "off"}}), \
             patch.object(cli.state, "write") as write, contextlib.redirect_stdout(output):
            self.assertEqual(cli.cmd_pin(SimpleNamespace()), 0)
        write.assert_called_once()
        self.assertIn("Export only", output.getvalue())
        self.assertNotIn("iteglow-restore.service", output.getvalue())


if __name__ == "__main__":
    unittest.main()

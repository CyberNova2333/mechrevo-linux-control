"""Report persistence failure without retrying or denying successful lights."""
import contextlib
import io
from pathlib import Path
import sys
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "iteglow/src"))
from iteglow import cli, state


class PersistenceTest(unittest.TestCase):
    @patch.object(state.os, "geteuid", return_value=1000)
    @patch.object(state, "load", return_value={"lightbar": {"mode": "off"}})
    @patch.object(state, "write")
    def test_success_preserves_other_device(self, write, _load, _uid):
        self.assertTrue(state.save("keyboard", {"mode": "off"}))
        self.assertEqual(write.call_args.args[1], {"lightbar": {"mode": "off"}, "keyboard": {"mode": "off"}})

    @patch.object(state.os, "geteuid", return_value=1000)
    @patch.object(state, "load", return_value={})
    @patch.object(state, "write", side_effect=OSError("disk full"))
    def test_write_failure_returns_false(self, _write, _load, _uid):
        self.assertFalse(state.save("keyboard", {"mode": "off"}))

    @patch.object(state.os, "geteuid", return_value=1000)
    @patch.object(state, "load", return_value=[])
    @patch.object(state, "write")
    def test_invalid_existing_state_is_not_silently_overwritten(self, write, _load, _uid):
        self.assertFalse(state.save("keyboard", {"mode": "off"}))
        write.assert_not_called()

    @patch.object(cli.state, "save", return_value=False)
    def test_cli_off_reports_persistence_failure_but_keeps_success(self, save):
        device = Mock()
        device.__enter__ = Mock(return_value=device)
        device.__exit__ = Mock(return_value=False)
        error = io.StringIO()
        with patch.object(cli, "_open", return_value=device), contextlib.redirect_stderr(error):
            self.assertEqual(cli.cmd_off(SimpleNamespace(device="keyboard")), 0)
        self.assertEqual(device.send.call_count, 1)
        save.assert_called_once_with("keyboard", {"mode": "off"})
        self.assertIn("turned off", error.getvalue())
        self.assertIn("could not be saved", error.getvalue())

    @patch.object(cli.state, "save", return_value=False)
    def test_cli_set_reports_persistence_failure_without_reapplying(self, save):
        device = Mock()
        device.__enter__ = Mock(return_value=device)
        device.__exit__ = Mock(return_value=False)
        args = SimpleNamespace(device="keyboard", mode="rainbow", color=(0, 255, 255),
                               palette=None, brightness=25, speed=5, direction=None, reactive=False)
        error = io.StringIO()
        with patch.object(cli, "_open", return_value=device), contextlib.redirect_stderr(error):
            self.assertEqual(cli.cmd_set(args), 0)
        self.assertEqual(device.send.call_count, 1)
        self.assertEqual(save.call_count, 1)
        self.assertIn("lighting applied", error.getvalue())
        self.assertIn("could not be saved", error.getvalue())


if __name__ == "__main__":
    unittest.main()

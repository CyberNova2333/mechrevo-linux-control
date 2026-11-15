"""Run under Xvfb; preferences use temporary files and no device backend."""
from copy import deepcopy
import json
import os
from pathlib import Path
import sys
import tempfile
import unittest
from unittest.mock import Mock, patch

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "iteglow/src"))
import gi
gi.require_version("Gtk", "4.0")
from gi.repository import Gtk
from iteglow import lighting_policy_gui as gui
from iteglow.lighting_policy import default_config


@unittest.skipUnless(os.environ.get("DISPLAY"), "requires an isolated Xvfb display")
class PolicyGuiTest(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        Gtk.init()

    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.path = Path(self.temporary.name) / "policy.json"

    def tearDown(self):
        self.temporary.cleanup()

    def test_construct_edit_then_explicit_save_only(self):
        notify = Mock()
        panel = gui.LightingPolicyPanel(self.path, on_saved=notify)
        self.assertEqual(panel.config(), default_config())
        self.assertFalse(panel.grid.get_sensitive())
        panel.enabled.set_active(True); panel.power_enabled.set_active(True)
        panel.idle_minutes.set_value(3)
        panel.brightness["keyboard"]["ac"].set_value(63)
        panel.brightness["logo"]["dc"].set_selected(1)
        self.assertFalse(self.path.exists())
        notify.assert_not_called()
        self.assertTrue(panel.grid.get_sensitive())
        panel.save()
        written = json.loads(self.path.read_text())
        self.assertEqual(written["brightness"]["keyboard"]["ac"], 63)
        self.assertEqual(written["brightness"]["logo"]["dc"], 25)
        self.assertEqual(written["idle_minutes"], 3)
        self.assertEqual(self.path.stat().st_mode & 0o777, 0o600)
        notify.assert_called_once_with(written)
        self.assertEqual(gui.LightingPolicyPanel(self.path).config(), written)

    def test_corrupt_existing_file_is_not_silently_overwritten(self):
        self.path.write_text("broken")
        panel = gui.LightingPolicyPanel(self.path)
        self.assertIn("尚未改动文件", panel.message.get_text())
        self.assertEqual(self.path.read_text(), "broken")
        self.assertFalse(panel.enabled.get_active())

    def test_invalid_config_leaves_existing_valid_file_and_no_temp(self):
        gui.save_config(self.path, default_config())
        before = self.path.read_bytes()
        invalid = deepcopy(default_config()); invalid["brightness"]["logo"]["ac"] = 30
        with self.assertRaises(ValueError): gui.save_config(self.path, invalid)
        self.assertEqual(self.path.read_bytes(), before)
        self.assertEqual(list(self.path.parent.iterdir()), [self.path])

    def test_atomic_replace_failure_leaves_previous_file(self):
        gui.save_config(self.path, default_config())
        before = self.path.read_bytes()
        changed = default_config(); changed["enabled"] = True
        with patch.object(gui.os, "replace", side_effect=OSError("disk full")):
            with self.assertRaises(OSError): gui.save_config(self.path, changed)
        self.assertEqual(self.path.read_bytes(), before)
        self.assertEqual(list(self.path.parent.iterdir()), [self.path])

    def test_save_failure_does_not_notify_or_claim_success(self):
        notify = Mock(); panel = gui.LightingPolicyPanel(self.path, on_saved=notify)
        with patch.object(gui, "save_config", side_effect=OSError("disk full")):
            panel.save()
        self.assertIn("保存失败", panel.message.get_text())
        notify.assert_not_called()

    def test_notification_failure_keeps_saved_config_and_distinct_message(self):
        panel = gui.LightingPolicyPanel(self.path, on_saved=Mock(side_effect=RuntimeError("offline")))
        panel.save()
        self.assertTrue(self.path.exists())
        self.assertIn("配置已保存", panel.message.get_text())
        self.assertIn("通知会话服务失败", panel.message.get_text())


if __name__ == "__main__":
    unittest.main()

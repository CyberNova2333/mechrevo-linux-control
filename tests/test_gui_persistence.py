import sys
from pathlib import Path
import unittest
from unittest.mock import patch

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "iteglow/src"))
from iteglow import gui
from gi.repository import Gtk
Gtk.init()


class FakeDevice:
    def __init__(self, *_): self.sent = []
    def query(self, _): return bytes([0x88, 2, 5, 5, 30, 8, 0, 0])
    def send(self, reports): self.sent.append(reports)
    def close(self): pass


class GuiPersistenceTest(unittest.TestCase):
    @patch.object(gui.devices, "HidLed", FakeDevice)
    @patch.object(gui.state, "load", return_value={})
    @patch.object(gui.state, "save", return_value=False)
    def test_applied_off_with_failed_save_is_shown_accurately(self, save, _load):
        panel = gui.DevicePanel("keyboard")
        try:
            panel.mode = "off"
            panel.apply()
            self.assertEqual(len(panel.dev.sent), 1)
            self.assertEqual(save.call_args.args[1]["mode"], "off")
            self.assertEqual(panel.status.get_text(), "灯光已应用，但恢复设置保存失败")
        finally:
            panel.close()


if __name__ == "__main__":
    unittest.main()

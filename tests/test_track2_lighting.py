"""Track 2 tests: extended firmware effects, 12H per-key matrix, hotkey sync, and GUI."""
import contextlib
import io
import json
import os
from pathlib import Path
import sys
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "iteglow/src"))
from iteglow import cli, effects, gui, keymap, lighting_session, protocol  # noqa: E402
from gi.repository import Gtk  # noqa: E402

Gtk.init()


class FakeLed:
    def __init__(self, reply=None):
        self.sent = []
        self.reply = reply or bytes([0x88, 2, 5, 1, 50, 8, 0, 0])
        self.closed = False

    def __enter__(self):
        return self

    def __exit__(self, *_):
        self.closed = True
        return False

    def query(self, _cmd):
        return self.reply

    def send(self, reports):
        self.sent.append(list(reports))

    def close(self):
        self.closed = True


class _NoopTx:
    def __init__(self, _zone):
        pass

    def __enter__(self):
        return self

    def __exit__(self, *_):
        return False


class ProtocolAndKeymapTest(unittest.TestCase):
    def test_101_layout_matches_oem_3p1nd_101_indices(self):
        caps = keymap.ALL_KEYCAPS
        self.assertEqual(len(caps), 101)
        by_name = {cap.name: cap for cap in caps}
        # Anchor coordinates proved on live hardware + CCUWinUI.cs SetColorKeyNumbers3p_101_Mapping
        self.assertEqual(by_name["Esc"].primary, 0)
        self.assertEqual(by_name["W"].primary, 45)
        self.assertEqual(by_name["A"].primary, 65)
        self.assertEqual(by_name["S"].primary, 66)
        self.assertEqual(by_name["D"].primary, 67)
        self.assertEqual(by_name["LCtrl"].primary, 105)
        self.assertEqual(by_name["Fn"].primary, 107)
        self.assertEqual(by_name["Win"].primary, 108)
        self.assertEqual(by_name["LAlt"].primary, 109)
        self.assertEqual(by_name["Space"].primary, 112)
        self.assertEqual(by_name["←"].primary, 118)
        self.assertEqual(by_name["↓"].primary, 119)
        self.assertEqual(by_name["→"].primary, 120)
        for cap in caps:
            for idx in cap.indices:
                self.assertGreaterEqual(idx, 0)
                self.assertLess(idx, protocol.KBD_MATRIX_CELLS)

    def test_keyboard_solid_unchanged_after_shared_packer(self):
        reps = protocol.keyboard_solid(12, 34, 56, 35)
        self.assertEqual(len(reps), 10)
        self.assertEqual(reps[0], ("feature", bytes([0x08, 0x02, 0x33, 0x00, 35, 0x00, 0x00, 0x00])))
        self.assertEqual(reps[1], ("feature", bytes([0x12, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00])))
        for kind, pkt in reps[2:]:
            self.assertEqual(kind, "output")
            self.assertEqual(len(pkt), 65)
            self.assertEqual(pkt[0], 0)
            self.assertEqual(list(pkt[1:5]), [0, 12, 34, 56])

    def test_keyboard_matrix_12h_transposes_top_to_bottom_into_wire_slots(self):
        cells = [(0, 0, 0)] * protocol.KBD_MATRIX_CELLS
        # Top-left Esc (row=0, col=0 => idx=0) should map to slot 5 (col*6 + (5 - 0))
        cells[0] = (10, 20, 30)
        # Bottom-left LCtrl (row=5, col=0 => idx=105) should map to slot 0
        cells[105] = (40, 50, 60)
        # W key (row=2, col=3 => idx=45) maps to slot 3*6 + (5 - 2) = 21 (pkt 1 slot 5)
        cells[45] = (255, 255, 255)
        reps = protocol.keyboard_matrix(cells, 50, calibrate=True)
        pkt0 = reps[2][1]
        pkt1 = reps[3][1]
        self.assertEqual(list(pkt0[1:5]), [0, 40, 50, 60])         # slot 0 (LCtrl)
        self.assertEqual(list(pkt0[1 + 5 * 4:1 + 6 * 4]), [0, 10, 20, 30])  # slot 5 (Esc)
        # Slot 21 is packet 1 slot 5 -> calibrated white (125, 255, 185)
        self.assertEqual(list(pkt1[1 + 5 * 4:1 + 6 * 4]), [0, 125, 255, 185])

    def test_extended_effects_and_decode(self):
        s_ltr = effects.build("keyboard", "sine", brightness=35, speed=3, direction=1)[-1][1]
        s_rtl = effects.build("keyboard", "sine", brightness=35, speed=3, direction=2)[-1][1]
        inter = effects.build("keyboard", "interlace", brightness=35, speed=3)[-1][1]
        d_pos = effects.build("keyboard", "diagonal", brightness=35, speed=3, direction=1)[-1][1]
        d_rev = effects.build("keyboard", "diagonal", brightness=35, speed=3, direction=2)[-1][1]
        self.assertEqual((s_ltr[2], s_ltr[6]), (0x03, 16))
        self.assertEqual((s_rtl[2], s_rtl[6]), (0x03, 17))
        self.assertEqual((inter[2], inter[6]), (0x03, 19))
        self.assertEqual((d_pos[2], d_pos[6]), (0x03, 33))
        self.assertEqual((d_rev[2], d_rev[6]), (0x03, 32))
        self.assertEqual(effects.decode_keyboard_state(protocol.parse_keyboard_state(bytes([0x88, 2, 3, 3, 35, 8, 17, 0]))), ("sine", 2))
        self.assertEqual(effects.decode_keyboard_state(protocol.parse_keyboard_state(bytes([0x88, 2, 3, 3, 35, 8, 19, 0]))), ("interlace", 1))
        self.assertEqual(effects.decode_keyboard_state(protocol.parse_keyboard_state(bytes([0x88, 2, 3, 3, 35, 8, 32, 0]))), ("diagonal", 2))
        self.assertEqual(effects.decode_keyboard_state(protocol.parse_keyboard_state(bytes([0x88, 2, 0x33, 0, 35, 8, 0, 0])), saved_mode="matrix"), ("matrix", 1))


class MatrixCliAndSessionTest(unittest.TestCase):
    def test_cli_matrix_preset_and_inline_keymap_parse(self):
        fake = FakeLed()
        with patch.object(cli.os, "geteuid", return_value=1000), \
             patch.object(cli, "_open", return_value=fake), \
             patch.object(cli, "ZoneTransaction", _NoopTx), \
             patch.object(cli.state, "save", return_value=True) as save:
            rc = cli.main(["set", "keyboard", "matrix", "--preset", "wasd_fps", "--brightness", "40"])
        self.assertEqual(rc, 0)
        self.assertEqual(len(fake.sent[0]), 10)
        self.assertEqual(save.call_args.args[1]["preset"], "wasd_fps")
        self.assertEqual(save.call_args.args[1]["brightness"], 40)

        parsed = cli._parse_matrix(json.dumps({"W": "#ff0000", "Space": [0, 128, 255]}))
        self.assertEqual(len(parsed), 126)
        self.assertEqual(parsed[45], (255, 0, 0))
        self.assertEqual(parsed[112], (0, 128, 255))

    def test_validate_hid_accepts_matrix_and_extended_modes(self):
        cells = [list(c) for c in keymap.preset_cells("zones")]
        action = lighting_session.validate_hid("keyboard", {"mode": "matrix", "matrix": cells, "brightness": 36, "calibrate": True})
        self.assertEqual(len(action.reports), 10)
        self.assertEqual(action.reports[0][1][4], 36)
        sine_action = lighting_session.validate_hid("keyboard", {"mode": "sine", "direction": 2, "brightness": 22})
        self.assertEqual(sine_action.reports[-1][1][6], 17)
        with self.assertRaises(ValueError):
            lighting_session.validate_hid("lightbar", {"mode": "static", "matrix": cells})

    def test_hotkey_sync_updates_state_without_fighting_external_or_policy_writes(self):
        with tempfile.TemporaryDirectory() as td:
            d = Path(td)
            uid = os.getuid()
            wmi = d / "wmi-state.json"
            wmi.write_text(json.dumps({"wmi_seq": 1, "wmi_last_code": 193, "wmi_events": [{"seq": 1, "code": 193}]}))
            initial = {"keyboard": {"mode": "rainbow", "brightness": 50, "speed": 1, "direction": 1, "reactive": False}}
            (d / "state.json").write_text(json.dumps(initial))
            hw = {"power": 2, "mode": 5, "speed": 1, "brightness": 50, "direction": 0}
            sync = lighting_session.KeyboardHotkeySync(d, uid, wmi_state_path=wmi, query_kbd=lambda: dict(hw), transaction=_NoopTx)
            sync.record_applied()
            self.assertIsNone(sync.sync_if_needed(force_query=True))

            # Hardware hotkey Fn+F6 dims brightness -> WMI 0xC1 fires
            hw["brightness"] = 36
            wmi.write_text(json.dumps({"wmi_seq": 2, "wmi_last_code": 193, "wmi_events": [{"seq": 2, "code": 193}]}))
            ev = sync.sync_if_needed(force_query=False)
            self.assertEqual(ev, {"hotkey_sync": "keyboard", "mode": "rainbow", "brightness": 36})
            self.assertEqual(json.loads((d / "state.json").read_text())["keyboard"]["brightness"], 36)

            # Policy dims to 12 WITHOUT modifying state.json; record_applied keeps baseline 36 intact
            hw["brightness"] = 12
            sync.record_applied()
            self.assertIsNone(sync.sync_if_needed(force_query=True))
            self.assertEqual(json.loads((d / "state.json").read_text())["keyboard"]["brightness"], 36)

            # Suspend resets baseline; wake returns hw reset before restore without clobbering state.json
            sync.reset()
            hw["brightness"] = 50
            self.assertIsNone(sync.sync_if_needed(force_query=True))
            self.assertEqual(json.loads((d / "state.json").read_text())["keyboard"]["brightness"], 36)


class Track2GuiTest(unittest.TestCase):
    @patch.object(gui.devices, "HidLed", lambda *_: FakeLed(bytes([0x88, 2, 3, 4, 35, 8, 17, 0])))
    @patch.object(gui.state, "load", return_value={})
    @patch.object(gui.state, "save", return_value=True)
    def test_gui_decodes_sine_and_edits_matrix_without_writes_on_construct(self, save, _load):
        panel = gui.DevicePanel("keyboard")
        try:
            self.assertEqual(panel.dev.sent, [])
            self.assertEqual(panel.mode, "sine")
            self.assertEqual(panel.direction.get_selected(), 1)
            panel.mode = "matrix"
            panel.apply_matrix_preset("wasd_fps")
            panel.apply()
            sent = panel.dev.sent[-1]
            self.assertEqual(len(sent), 10)
            self.assertEqual(sent[0][1][2], 0x33)
            self.assertEqual(len(save.call_args.args[1]["matrix"]), 126)
        finally:
            panel.close()

    def test_gui_poll_hotkey_updates_brightness_without_rewriting_hardware(self):
        fake = FakeLed(bytes([0x88, 2, 5, 4, 35, 8, 0, 0]))
        with patch.object(gui.devices, "HidLed", lambda *_: fake), \
             patch.object(gui.state, "load", return_value={}):
            panel = gui.DevicePanel("keyboard")
            try:
                self.assertEqual(panel.bright.get_value(), 70.0)
                fake.reply = bytes([0x88, 2, 5, 4, 50, 8, 0, 0])
                self.assertTrue(panel._poll_hotkey())
                self.assertEqual(panel.bright.get_value(), 100.0)
                self.assertEqual(fake.sent, [])
                self.assertIsNone(panel.pending)

                fake.reply = bytes([0x88, 1, 5, 4, 0, 8, 0, 0])
                self.assertTrue(panel._poll_hotkey())
                self.assertEqual(panel.bright.get_value(), 0.0)
                self.assertIn("快捷键已关闭键盘背光", panel.status.get_text())
                self.assertEqual(fake.sent, [])
            finally:
                panel.close()


if __name__ == "__main__":
    unittest.main()

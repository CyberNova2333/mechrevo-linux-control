"""GTK4 UI tests for WaterCoolingPanel and main window integration."""
from __future__ import annotations

import os
from pathlib import Path
import sys
import tempfile
import unittest
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "iteglow/src"))

import gi

gi.require_version("GLib", "2.0")
gi.require_version("Gtk", "4.0")
from gi.repository import Gio, GLib, Gtk

from iteglow import gui, watercooling as wc  # noqa: E402
from iteglow.watercooling_gui import WaterCoolingPanel  # noqa: E402


def _pump_glib(iterations: int = 20) -> None:
    ctx = GLib.MainContext.default()
    for _ in range(iterations):
        while ctx.pending():
            ctx.iteration(False)


class _DummyPanel(Gtk.Box):
    def __init__(self, *args, **kwargs):
        super().__init__(orientation=Gtk.Orientation.VERTICAL, spacing=6)

    def close(self):
        pass


class WaterCoolingGuiTests(unittest.TestCase):
    def test_panel_constructs_with_zero_io_and_builds_validated_config(self):
        calls = []

        def fake_req(req):
            calls.append(req)
            return {"service_running": True, "config": wc.default_config()}

        with tempfile.TemporaryDirectory() as tmp:
            cfg_path = Path(tmp) / "wc.json"
            panel = WaterCoolingPanel(config_path=cfg_path, request_fn=fake_req, poll_interval_ms=60000)
            self.assertEqual(calls, [])
            self.assertFalse(cfg_path.exists())
            self.assertFalse(panel.auto_connect_switch.get_active())
            self.assertTrue(panel.flow_protect_switch.get_active())

            # Default strategy is preset -> preset row visible, manual/curve hidden
            self.assertTrue(panel.preset_row.get_visible())
            self.assertFalse(panel.manual_box.get_visible())
            self.assertFalse(panel.curve_box.get_visible())
            self.assertFalse(panel.fan_color_row.get_visible())

            # Switch to manual & split LED policy
            panel.strategy_dd.set_selected(1)  # manual
            panel.fan_policy_dd.set_selected(3)  # split
            panel.led_mode_dd.set_selected(1)  # static
            panel.manual_pump_duty.set_value(72)
            panel.manual_fan_duty.set_value(58)
            _pump_glib()

            self.assertTrue(panel.manual_box.get_visible())
            self.assertTrue(panel.fan_color_row.get_visible())
            built = panel.build_config_from_ui()
            self.assertEqual(built["strategy"], "manual")
            self.assertEqual(built["manual"]["pump_duty"], 72)
            self.assertEqual(built["led"]["fan_policy"], "split")
            panel.close()

    def test_snapshot_updates_telemetry_and_flow_fault_banner(self):
        with tempfile.TemporaryDirectory() as tmp:
            panel = WaterCoolingPanel(config_path=Path(tmp) / "wc.json", request_fn=lambda r: {})
            panel.apply_snapshot({
                "connected": True,
                "sleeping": False,
                "flow_tripped": True,
                "flow_trip_reason": "水泵开启超过 5s 未检测到水流 (meter_state=1)",
                "device_name": "CoolingSystem LCT22002",
                "device_mac": "E2:90:B5:F8:43:2E",
                "fw_version": "2.0.0.4",
                "meter_state": 1,
                "reason": "flow_fault_locked",
                "temp_c": 52.5,
                "applied": {"pump_volt": 0, "pump_duty": 0, "fan_duty": 0},
                "ec": {
                    "ec_available": True,
                    "ec_stale": False,
                    "lc_supported": True,
                    "ec_pump_volt": 7,
                    "ec_pump_enabled": True,
                    "ec_fan_duty": 40,
                    "ec_fan_enabled": True,
                    "wmi_c1_seq": 4,
                },
                "config": wc.default_config(),
            })
            self.assertEqual(panel.status_pill.get_text(), "断流锁定")
            self.assertIn("2.0.0.4", panel.device_label.get_text())
            self.assertIn("52.5°C", panel.flow_label.get_text())
            self.assertIn("WMI C1 seq=4", panel.ec_label.get_text())
            self.assertTrue(panel.fault_label.get_visible())
            panel.close()

    def test_apply_saves_locally_when_daemon_socket_missing(self):
        def missing_sock(_req):
            raise FileNotFoundError("no socket")

        with tempfile.TemporaryDirectory() as tmp:
            cfg_path = Path(tmp) / "wc.json"
            panel = WaterCoolingPanel(config_path=cfg_path, request_fn=missing_sock)
            panel.led_mode_dd.set_selected(2)  # breathing
            panel._on_apply_clicked()
            # Wait briefly for worker thread -> GLib idle callback
            for _ in range(30):
                _pump_glib(5)
                if cfg_path.exists():
                    break
                GLib.usleep(10000)
            self.assertTrue(cfg_path.exists())
            saved = wc.load_config(cfg_path)
            self.assertTrue(saved["led"]["mode"] == "breathing")
            self.assertIn("本地配置文件", panel.msg_label.get_text())
            panel.close()

    def test_close_stops_poll_timer_and_ignores_callbacks(self):
        with tempfile.TemporaryDirectory() as tmp:
            panel = WaterCoolingPanel(
                config_path=Path(tmp) / "wc.json",
                request_fn=lambda _r: {"service_running": True, "config": wc.default_config()},
                poll_interval_ms=250,
            )
            panel._poll_id = GLib.timeout_add(250, panel._on_poll_tick)
            self.assertIsNotNone(panel._poll_id)
            panel.close()
            self.assertIsNone(panel._poll_id)
            self.assertFalse(panel._on_poll_tick())
            self.assertEqual(panel.msg_label.get_text(), "就绪（仅读取本地配置，未主动操作水冷硬件）")

    def test_main_app_navigation_includes_watercooling_tab(self):
        with tempfile.TemporaryDirectory() as tmp, \
             patch.dict(os.environ, {"XDG_CONFIG_HOME": tmp}), \
             patch.object(gui.devices, "available_devices", return_value=[]), \
             patch.object(gui.state, "load", return_value={}), \
             patch.object(gui, "PerformancePanel", _DummyPanel), \
             patch.object(gui, "LogoPanel", _DummyPanel), \
             patch.object(gui, "ProfilesPanel", _DummyPanel), \
             patch.object(gui, "LightingPolicyPanel", _DummyPanel):
            app = gui.App()
            app.set_flags(app.get_flags() | Gio.ApplicationFlags.NON_UNIQUE)
            app.register(None)
            app.activate()
            _pump_glib()
            win = app.get_active_window()
            self.assertIsNotNone(win)
            self.assertIsInstance(app.watercooling_panel, WaterCoolingPanel)
            content = win.get_content() if hasattr(win, "get_content") else win.get_child()
            stacks = []
            queue = [content]
            while queue:
                cur = queue.pop(0)
                if cur is None:
                    continue
                if isinstance(cur, Gtk.Stack):
                    stacks.append(cur)
                child = cur.get_first_child()
                while child is not None:
                    queue.append(child)
                    child = child.get_next_sibling()
            matched_stack = None
            for s in stacks:
                child = s.get_child_by_name("watercooling")
                if child is not None and (child is app.watercooling_panel or app.watercooling_panel.is_ancestor(child)):
                    matched_stack = s
                    break
            self.assertIsNotNone(matched_stack)
            matched_stack.set_visible_child_name("watercooling")
            _pump_glib()
            app.do_shutdown()
            self.assertIsNone(app.watercooling_panel)
            if win is not None:
                win.destroy()
            app.quit()

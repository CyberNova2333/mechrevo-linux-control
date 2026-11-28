"""Offline comprehensive tests for MECHREVO BLE water-cooling support and EC telemetry."""
from __future__ import annotations

import contextlib
import importlib.machinery
import importlib.util
import io
import json
import os
from pathlib import Path
import socket
import tempfile
import threading
import unittest
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[1]
import sys

sys.path.insert(0, str(ROOT / "iteglow/src"))

from iteglow import cli  # noqa: E402
from iteglow import watercooling as wc  # noqa: E402
from iteglow.watercooling_ble import CoolerBleClient  # noqa: E402
from iteglow.watercooling_service import (  # noqa: E402
    CoolerCoordinator,
    _RequestQueue,
    _read_request,
    _remove_stale_socket,
    cooler_lock,
    dispatch_command,
    service_request,
)


def _load_profile_helper():
    path = ROOT / "system/mechrevo-profile-helper"
    loader = importlib.machinery.SourceFileLoader("mechrevo_profile_helper_wc_test", str(path))
    spec = importlib.util.spec_from_loader(loader.name, loader)
    mod = importlib.util.module_from_spec(spec)
    loader.exec_module(mod)
    return mod


class FakeClock:
    def __init__(self, start: float = 100.0):
        self.now = start

    def __call__(self) -> float:
        return self.now

    def advance(self, seconds: float) -> None:
        self.now += seconds


class FakeBleTransport:
    def __init__(self, clock: FakeClock, *, transient_connect_failures: int = 0, fail_request_once: bool = False):
        self.clock = clock
        self.transient_connect_failures = transient_connect_failures
        self.fail_request_once = fail_request_once
        self.connect_calls = 0
        self.pair_calls = 0
        self.scans = 0
        self.writes = []  # (timestamp, write_type, bytes)
        self.disconnected = False
        self.auto_reply_handshake = True
        self.next_meter_state = 2
        self._on_notify = None
        self._on_disconnect = None

    def scan(self, timeout_s: float = 5.0, target_mac: str = ""):
        self.scans += 1
        return [{
            "path": "/org/bluez/hci0/dev_E2_90_B5_F8_43_2E",
            "name": "CoolingSystem LCT22002",
            "address": target_mac or "E2:90:B5:F8:43:2E",
            "rssi": -55,
            "connected": False,
            "paired": False,
        }]

    def connect(self, *, target_mac: str = "", scan_timeout_s: float = 6.0, on_notify=None, on_disconnect=None):
        self.connect_calls += 1
        if self.connect_calls <= self.transient_connect_failures:
            raise RuntimeError("GDBus.Error:org.bluez.Error.Failed: le-connection-abort-by-local")
        self._on_notify = on_notify
        self._on_disconnect = on_disconnect
        self.disconnected = False
        return {"name": "CoolingSystem LCT22002", "address": target_mac or "E2:90:B5:F8:43:2E"}

    def write_frame(self, data: bytes, write_type: str = "request") -> None:
        if self.fail_request_once and write_type == "request":
            self.fail_request_once = False
            raise RuntimeError("Write Request unsupported")
        self.writes.append((self.clock.now, write_type, bytes(data)))
        if data == b"sw" and self.auto_reply_handshake and self._on_notify is not None:
            self._on_notify(b"MCU F/W Version: 2.0.0.4\r\n")
        elif len(data) == 8 and data[0] == 0xFE and data[1] == 0x32 and self._on_notify is not None:
            self._on_notify(bytes((0xFE, 0x32, 0x05, self.next_meter_state, 0xEF)))

    def disconnect(self) -> None:
        self.disconnected = True

    def pump(self, seconds: float) -> None:
        self.clock.advance(seconds)


class ProtocolAndConfigTests(unittest.TestCase):
    def test_pump_voltage_and_fan_encoding(self):
        self.assertEqual(wc.encode_handshake(), b"sw")
        self.assertEqual(wc.encode_sleep(), bytes.fromhex("fe190001000000ef"))
        self.assertEqual(wc.encode_meter_query(), bytes.fromhex("fe320000000000ef"))
        self.assertEqual(wc.encode_fan(0), bytes.fromhex("fe1b0000000000ef"))
        self.assertEqual(wc.encode_fan(40), bytes.fromhex("fe1b0128000000ef"))

        expected = {
            0: "fe1c0000000000ef",
            7: "fe1c013c020000ef",
            8: "fe1c013c030000ef",
            11: "fe1c013c000000ef",
            12: "fe1c013c010000ef",
        }
        for volt, hex_frame in expected.items():
            with self.subTest(volt=volt):
                self.assertEqual(wc.encode_pump(volt, 60).hex(), hex_frame)

        with self.assertRaises(ValueError):
            wc.encode_pump(True, 60)  # bool must never pass as int
        with self.assertRaises(ValueError):
            wc.encode_pump(9, 60)

    def test_dual_zone_led_plans(self):
        off_plan = [f.hex() for f, _ in wc.encode_led_plan({"mode": "off", "color": [1, 2, 3], "fan_policy": "sync", "fan_color": [0, 0, 0]})]
        self.assertEqual(off_plan, ["fe330000000000ef", "fe1e0000000000ef"])

        sync_plan = [f.hex() for f, _ in wc.encode_led_plan({"mode": "colorful", "color": [255, 0, 0], "fan_policy": "sync", "fan_color": [0, 0, 0]})]
        self.assertEqual(sync_plan, ["fe330000000000ef", "fe1e01ff000002ef"])

        body_only_plan = [f.hex() for f, _ in wc.encode_led_plan({"mode": "static", "color": [0, 255, 0], "fan_policy": "body_only", "fan_color": [0, 0, 0]})]
        self.assertEqual(body_only_plan, ["fe1e0000000000ef", "fe330100000000ef", "fe1e0100ff0000ef"])

        freeze_plan = [f.hex() for f, _ in wc.encode_led_plan({"mode": "breathing", "color": [0, 0, 255], "fan_policy": "freeze", "fan_color": [0, 0, 0]})]
        self.assertEqual(freeze_plan, ["fe330100000000ef", "fe1e010000ff01ef"])

        split_plan = [f.hex() for f, _ in wc.encode_led_plan({"mode": "breathing_colorful", "color": [0, 0, 255], "fan_policy": "split", "fan_color": [255, 0, 128]})]
        self.assertEqual(split_plan, ["fe330000000000ef", "fe1e01ff008000ef", "fe330100000000ef", "fe1e010000ff03ef"])

    def test_rx_parser(self):
        p1 = wc.parse_rx_packet(bytes.fromhex("fe310502ef"))
        self.assertEqual((p1["kind"], p1["meter_state"]), ("meter", 2))
        p2 = wc.parse_rx_packet(b"MCU F/W Version: 2.0.0.4\r\n")
        self.assertEqual((p2["kind"], p2["fw_version"]), ("firmware", "2.0.0.4"))
        p3 = wc.parse_rx_packet(b"\xff\x80\x01")
        self.assertEqual(p3["kind"], "unknown")

    def test_config_validation_safety_defaults(self):
        cfg = wc.default_config()
        self.assertFalse(cfg["auto_connect"])
        self.assertTrue(cfg["flow_protection"])
        self.assertTrue(cfg["sleep_on_disconnect"])

        for bad in (
            {"auto_connect": 1},
            {"unknown": 1},
            {"mac": "not-a-mac"},
            {"manual": {"pump_volt": 7, "pump_duty": 0, "fan_duty": 40}},
            {"manual": {"pump_volt": True, "pump_duty": 60, "fan_duty": 40}},
            {"curve": {"hysteresis_c": 3, "points": [
                {"temp_c": 60, "pump_volt": 7, "pump_duty": 60, "fan_duty": 40},
                {"temp_c": 60, "pump_volt": 8, "pump_duty": 70, "fan_duty": 55},
            ]}},
        ):
            with self.subTest(bad=bad), self.assertRaises(ValueError):
                wc.validate_config(bad)

        with tempfile.TemporaryDirectory() as tmp:
            p = Path(tmp) / "wc.json"
            custom = wc.validate_config({"mac": "e2:90:b5:f8:43:2e", "strategy": "manual"})
            wc.save_config(custom, p)
            loaded = wc.load_config(p)
            self.assertEqual(loaded["mac"], "E2:90:B5:F8:43:2E")
            self.assertEqual(loaded["strategy"], "manual")


class TelemetryCurveAndFlowTests(unittest.TestCase):
    def test_host_temp_and_ec_telemetry_readers(self):
        with tempfile.TemporaryDirectory() as tmp:
            root = Path(tmp)
            hw0 = root / "hwmon0"
            hw0.mkdir()
            (hw0 / "name").write_text("acpitz", encoding="utf-8")
            (hw0 / "temp1_input").write_text("79000", encoding="utf-8")

            hw1 = root / "hwmon1"
            hw1.mkdir()
            (hw1 / "name").write_text("coretemp", encoding="utf-8")
            (hw1 / "temp1_label").write_text("Package id 0", encoding="utf-8")
            (hw1 / "temp1_input").write_text("54300", encoding="utf-8")
            self.assertEqual(wc.read_host_temp_c(root), 54.3)

            wmi_path = root / "wmi.json"
            ec_path = root / "ec.json"
            wmi_path.write_text(json.dumps({"wmi_seq": 5, "wmi_events": [{"seq": 3, "code": 193}]}), encoding="utf-8")
            import time
            ec_path.write_text(json.dumps({
                "abi": 1,
                "lc_supported": True,
                "lc_ec_enabled": False,
                "ec_fan_enabled": True,
                "ec_fan_duty": 40,
                "ec_pump_enabled": True,
                "ec_pump_volt": 7,
                "updated_monotonic_ms": int(time.monotonic() * 1000),
            }), encoding="utf-8")
            state = wc.read_ec_watercooler_state(wmi_path, ec_path)
            self.assertTrue(state["wmi_available"])
            self.assertEqual(state["wmi_c1_seq"], 3)
            self.assertTrue(state["ec_available"])
            self.assertFalse(state["ec_stale"])
            self.assertEqual((state["ec_pump_volt"], state["ec_fan_duty"]), (7, 40))

    def test_smart_curve_hysteresis_and_ec_follow_fallback(self):
        cfg = wc.validate_config({"strategy": "smart_curve"})
        pv, pd, fd, tier, _ = wc.evaluate_cooling_target(cfg, temp_c=66.0, ec_state=None, current_tier=0)
        self.assertEqual((tier, pv, pd, fd), (1, 8, 70, 55))
        # Drops only 2C (66 -> 63): hysteresis is 3C (threshold 65 - 3 = 62), so stay in tier 1
        pv, pd, fd, tier, _ = wc.evaluate_cooling_target(cfg, temp_c=63.0, ec_state=None, current_tier=tier)
        self.assertEqual(tier, 1)
        # Drops to 62.0C -> steps back down to tier 0
        pv, pd, fd, tier, _ = wc.evaluate_cooling_target(cfg, temp_c=62.0, ec_state=None, current_tier=tier)
        self.assertEqual((tier, pv, fd), (0, 7, 40))

        ec_cfg = wc.validate_config({"strategy": "ec_follow"})
        pv, pd, fd, _, reason = wc.evaluate_cooling_target(
            ec_cfg, temp_c=50.0, ec_state={"ec_available": True, "ec_stale": True, "ec_pump_enabled": True, "ec_pump_volt": 12, "ec_fan_enabled": True, "ec_fan_duty": 100}
        )
        self.assertEqual((reason, pv, fd), ("ec_fallback_silent", 7, 40))

        pv, pd, fd, _, reason = wc.evaluate_cooling_target(
            ec_cfg, temp_c=50.0, ec_state={"ec_available": True, "ec_stale": False, "ec_pump_enabled": True, "ec_pump_volt": 8, "ec_fan_enabled": True, "ec_fan_duty": 55}
        )
        self.assertEqual((reason, pv, pd, fd), ("ec_follow", 8, 60, 55))

    def test_flow_safety_monitor_latch_and_reset(self):
        mon = wc.FlowSafetyMonitor(grace_s=5.0, fault_hold_s=3.0)
        self.assertFalse(mon.update(now=0.0, pump_volt=7, meter_state=1, flow_protection=True))
        self.assertFalse(mon.update(now=4.9, pump_volt=7, meter_state=1, flow_protection=True))
        self.assertTrue(mon.update(now=5.0, pump_volt=7, meter_state=1, flow_protection=True))
        self.assertTrue(mon.tripped)
        mon.reset()
        self.assertFalse(mon.tripped)
        # Confirmed flow followed by stall > 3s trips
        self.assertFalse(mon.update(now=10.0, pump_volt=7, meter_state=1, flow_protection=True))
        self.assertFalse(mon.update(now=11.0, pump_volt=7, meter_state=2, flow_protection=True))
        self.assertFalse(mon.update(now=12.0, pump_volt=7, meter_state=1, flow_protection=True))
        self.assertTrue(mon.update(now=15.0, pump_volt=7, meter_state=1, flow_protection=True))


class BleClientCoordinatorAndCliTests(unittest.TestCase):
    def test_ble_client_retries_0x3e_paces_tx_and_falls_back_from_request(self):
        clock = FakeClock(100.0)
        tr = FakeBleTransport(clock, transient_connect_failures=2, fail_request_once=True)
        client = CoolerBleClient(tr, min_gap_s=0.1, clock=clock)
        info = client.connect()
        self.assertEqual(tr.connect_calls, 3)
        self.assertEqual(tr.pair_calls, 0)
        self.assertEqual(info["fw_version"], "2.0.0.4")
        self.assertEqual(info["meter_state"], 2)
        self.assertEqual(client.preferred_write_type, "command")

        client.apply_actuators(7, 60, 40)
        deltas = [tr.writes[i][0] - tr.writes[i - 1][0] for i in range(1, len(tr.writes))]
        self.assertTrue(all(d >= 0.0999 for d in deltas), f"Expected >=100ms TX gap, got {deltas}")

        client.disconnect(send_sleep=True, stop_actuators=True)
        self.assertTrue(tr.disconnected)
        sent_hex = [w[2].hex() for w in tr.writes]
        self.assertIn("fe1c0000000000ef", sent_hex)
        self.assertIn("fe190001000000ef", sent_hex)

    def test_coordinator_lifecycle_sleep_stall_and_ipc_socket(self):
        clock = FakeClock(200.0)
        tr = FakeBleTransport(clock)
        client = CoolerBleClient(tr, min_gap_s=0.1, clock=clock)
        ec_state = {
            "ec_available": True,
            "ec_stale": False,
            "ec_pump_enabled": True,
            "ec_pump_volt": 7,
            "ec_fan_enabled": True,
            "ec_fan_duty": 40,
            "wmi_c1_seq": 1,
        }
        with tempfile.TemporaryDirectory() as tmp:
            tmp_path = Path(tmp)
            cfg_path = tmp_path / "wc.json"
            coord = CoolerCoordinator(
                client=client,
                config=wc.default_config(),
                config_saver=lambda c: wc.save_config(c, cfg_path),
                temp_reader=lambda: 45.0,
                ec_reader=lambda: dict(ec_state),
                clock=clock,
            )
            # Default auto_connect=False -> tick must not connect
            coord.tick()
            self.assertFalse(client.connected)
            self.assertEqual(tr.connect_calls, 0)

            res = coord.connect()
            self.assertTrue(res["connected"])
            self.assertEqual((res["applied"]["pump_volt"], res["applied"]["fan_duty"]), (7, 40))

            # Simulate flow loss -> stall latches and emergency shuts off pump
            tr.next_meter_state = 1
            for _ in range(8):
                clock.advance(1.1)
                coord.tick()
            st = coord.status()
            self.assertTrue(st["flow_tripped"])
            self.assertEqual(st["applied"], {"pump_volt": 0, "pump_duty": 0, "fan_duty": 0})

            coord.reset_fault()
            tr.next_meter_state = 2
            client.query_meter()
            coord.apply_targets(force=True)
            self.assertEqual(coord.status()["applied"]["pump_volt"], 7)

            # Sleep tears down BLE & blocks reconnect until resumed
            coord.prepare_for_sleep(True)
            self.assertFalse(client.connected)
            with self.assertRaises(RuntimeError):
                coord.connect()
            coord.prepare_for_sleep(False)

            # Real AF_UNIX IPC roundtrip with dispatch_command
            sock_p = tmp_path / "wc.sock"
            srv = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
            srv.bind(str(sock_p))
            os.chmod(sock_p, 0o600)
            srv.listen(2)

            def serve_one():
                conn, _ = srv.accept()
                with conn:
                    req = json.loads(conn.recv(4096).decode("utf-8"))
                    resp = {"ok": True, "result": dispatch_command(coord, req)}
                    conn.sendall((json.dumps(resp) + "\n").encode("utf-8"))

            t = threading.Thread(target=serve_one, daemon=True)
            t.start()
            try:
                got = service_request({"op": "status"}, sock_path=sock_p, timeout_s=3.0)
                self.assertTrue(got["service_running"])
                self.assertEqual(got["config"]["preset"], "silent")
            finally:
                t.join(timeout=2.0)
                srv.close()

    def test_partial_pump_write_and_manual_disconnect(self):
        clock = FakeClock()
        tr = FakeBleTransport(clock)
        client = CoolerBleClient(tr, clock=clock)
        coord = CoolerCoordinator(client=client, config={"preset": "stop", "auto_connect": True},
                                  config_saver=lambda cfg: cfg, temp_reader=lambda: 40, ec_reader=dict, clock=clock)
        for req in ({"op": "connect", "mac": 42}, {"op": "connect", "mac": "bad"},
                    {"op": "prepare_sleep", "entering": "false"}, {"op": "scan", "timeout_s": float("inf")}):
            with self.assertRaises(ValueError):
                dispatch_command(coord, req)
        self.assertFalse(coord.sleeping)
        self.assertEqual(tr.connect_calls, 0)
        coord.connect()
        write = tr.write_frame

        def fail_fan(data, write_type):
            if data[:2] == bytes([0xfe, 0x1b]):
                raise RuntimeError("ATT error: 0x0e")
            write(data, write_type)

        with patch.object(tr, "write_frame", side_effect=fail_fan):
            with self.assertRaises(RuntimeError):
                coord.set_config({"preset": "silent"})
            self.assertEqual((coord.applied_pump_volt, coord.applied_fan_duty), (7, 0))
            before = len(tr.writes)
            coord.connect()
            self.assertEqual(len(tr.writes), before)
            tr.next_meter_state = 1
            clock.advance(6)
            coord.tick()
            self.assertTrue(coord.flow_monitor.tripped)
            self.assertTrue(coord.actuators_dirty)
        coord.tick()
        self.assertFalse(coord.actuators_dirty)
        self.assertEqual(coord.applied_pump_volt, 0)
        coord.reset_fault()
        with patch.object(coord, "_config_saver", side_effect=OSError("read-only")):
            with self.assertRaises(OSError):
                coord.emergency_stop()
        self.assertIn(wc.encode_sleep(), [frame for _, _, frame in tr.writes])
        clock.advance(60)
        coord.tick()
        self.assertFalse(client.connected)

    def test_missing_meter_reply_invalidates_previous_good_reading(self):
        clock = FakeClock()
        tr = FakeBleTransport(clock)
        client = CoolerBleClient(tr, clock=clock)
        client.connect()
        self.assertEqual(client.meter_state, 2)
        tr._on_notify = None
        self.assertIsNone(client.query_meter())

    def test_delayed_glib_requests_keep_their_own_deadlines_and_replies(self):
        import queue
        from iteglow import watercooling_service as service

        requests = _RequestQueue(None)
        requests.busy = True
        expired, first, second = (queue.Queue(1) for _ in range(3))
        with patch.object(service.time, "monotonic", return_value=10):
            requests.submit({"op": "connect"}, expired, 11)
            requests.submit({"op": "disconnect"}, first, 30)
            requests.submit({"op": "prepare_sleep"}, second)
        requests.busy = False
        with patch.object(service.time, "monotonic", return_value=12), \
             patch.object(service, "dispatch_command", side_effect=lambda _, req: req["op"]) as dispatch:
            requests.drain()
        self.assertEqual(dispatch.call_count, 2)
        self.assertTrue(expired.empty())
        self.assertEqual(first.get_nowait()["result"], "disconnect")
        self.assertEqual(second.get_nowait()["result"], "prepare_sleep")

    def test_socket_framing_stale_paths_and_offline_lock(self):
        with tempfile.TemporaryDirectory() as tmp:
            p = Path(tmp) / "ipc"
            p.write_text("preserve")
            with self.assertRaises(RuntimeError):
                _remove_stale_socket(p)
            self.assertEqual(p.read_text(), "preserve")
            p.unlink()
            with socket.socket(socket.AF_UNIX) as srv:
                srv.bind(str(p))
                srv.listen(1)
                with self.assertRaises(RuntimeError):
                    _remove_stale_socket(p)
            _remove_stale_socket(p)
            self.assertFalse(p.exists())
            with cooler_lock(Path(tmp)), self.assertRaises(RuntimeError), cooler_lock(Path(tmp)):
                self.fail("duplicate lock acquired")
        with contextlib.ExitStack() as stack:
            reader, writer = map(stack.enter_context, socket.socketpair())
            writer.sendall(b'{"op":')
            writer.sendall(b'"status"}\n')
            self.assertEqual(_read_request(reader), {"op": "status"})
            writer.sendall(b"x" * 16384 + b"\n")
            with self.assertRaises(ValueError):
                _read_request(reader)

    def test_cli_cooler_offline_commands_and_root_guard(self):
        parser = cli.build_parser()
        with tempfile.TemporaryDirectory() as tmp, \
             patch.dict(os.environ, {"XDG_CONFIG_HOME": tmp, "XDG_RUNTIME_DIR": tmp}), \
             patch.object(os, "geteuid", return_value=1000):
            buf = io.StringIO()
            args = parser.parse_args(["cooler", "set", "--strategy", "manual", "--pump-volt", "8", "--pump-duty", "75", "--fan-duty", "55"])
            with contextlib.redirect_stdout(buf):
                rc = args.func(args)
            self.assertEqual(rc, 0)
            payload = json.loads(buf.getvalue())
            self.assertEqual(payload["config"]["manual"], {"pump_volt": 8, "pump_duty": 75, "fan_duty": 55})

            buf2 = io.StringIO()
            args2 = parser.parse_args(["cooler", "led", "static", "--fan-policy", "split", "--color", "#00ff88", "--fan-color", "255,0,0"])
            with contextlib.redirect_stdout(buf2):
                rc2 = args2.func(args2)
            self.assertEqual(rc2, 0)
            led_cfg = json.loads(buf2.getvalue())["config"]["led"]
            self.assertEqual((led_cfg["mode"], led_cfg["fan_policy"], led_cfg["color"], led_cfg["fan_color"]), ("static", "split", [0, 255, 136], [255, 0, 0]))

            buf3 = io.StringIO()
            args3 = parser.parse_args(["cooler", "status"])
            with contextlib.redirect_stdout(buf3):
                rc3 = args3.func(args3)
            self.assertEqual(rc3, 0)
            self.assertFalse(json.loads(buf3.getvalue())["service_running"])

        with patch.object(os, "geteuid", return_value=0):
            args_root = parser.parse_args(["cooler", "set", "--preset", "silent"])
            with contextlib.redirect_stderr(io.StringIO()):
                self.assertEqual(args_root.func(args_root), 1)

    def test_profile_helper_lc_telemetry_is_strictly_read_only(self):
        helper = _load_profile_helper()
        regs = dict.fromkeys(helper.REGISTERS, 0)
        regs.update({
            helper.MODE: helper.MODES["office"],
            0x706: 0x40,
            0x7A4: 0x80,
            0x7C7: 0x0C,
            0x7C8: 0x60,
            0x7E6: 0xA8,
            0x7E7: 0x87,
            0xD48: 0xFF,
        })
        lc = helper.watercooling_status(regs)
        self.assertTrue(lc["lc_supported"])
        self.assertTrue(lc["lc_fan_table_supported"])
        self.assertFalse(lc["lc_hwoc_supported"])
        self.assertFalse(lc["lc_ec_enabled"])
        self.assertEqual(lc["ec_c2_index"], 3)
        self.assertTrue(lc["ec_fan_enabled"])
        self.assertEqual(lc["ec_fan_duty"], 40)
        self.assertTrue(lc["ec_pump_enabled"])
        self.assertEqual(lc["ec_pump_volt"], 7)

        writes = []

        class RecordingEC:
            def __enter__(self):
                return self

            def __exit__(self, *exc):
                return False

            def read(self, reg):
                return regs[reg]

            def write(self, reg, val):
                writes.append((reg, val))
                regs[reg] = val

            def write_mode(self, val):
                writes.append((helper.MODE, val))
                regs[helper.MODE] = val

            def write_flag(self, reg, bit):
                writes.append((reg, bit))
                regs[reg] = bit

        ec = RecordingEC()
        st = helper.status(ec)
        self.assertEqual(st["watercooling"]["ec_pump_volt"], 7)
        self.assertEqual(writes, [])

        with tempfile.TemporaryDirectory() as tmp:
            lc_file = Path(tmp) / "lc.json"
            missing_state = Path(tmp) / "missing_state.json"
            with patch.object(helper, "check_machine", return_value=None), \
                 patch.object(helper, "STATE", missing_state), \
                 patch.object(helper, "turbo_request", return_value={"active": False, "recovery_needed": False}), \
                 patch.object(helper, "run_with_lock", side_effect=lambda fn, **_kw: fn(ec)):
                lc_out = helper.main(["lc-status"])
                helper.main(["set", "balanced"])
            self.assertEqual(lc_out["ec_fan_duty"], 40)
            self.assertEqual(regs[helper.MODE], helper.MODES["balanced"])
            written_regs = {r for r, _ in writes}
            self.assertFalse({0x7A4, 0x7C7, 0x7C8, 0x7E6, 0x7E7, 0xD48} & written_regs)

            old_umask = os.umask(0o077)
            try:
                with patch.object(helper, "LC_EC_STATE", lc_file):
                    helper.write_lc_ec_state(lc)
            finally:
                os.umask(old_umask)
            self.assertEqual(lc_file.stat().st_mode & 0o777, 0o644)

    def test_write_disconnect_error_preserves_request_mode_and_schedules_fast_reconnect(self):
        from iteglow.watercooling_service import LINK_DROP_RECONNECT_DELAY_S

        clock = FakeClock(300.0)
        tr = FakeBleTransport(clock)
        client = CoolerBleClient(tr, min_gap_s=0.1, clock=clock)
        events = []
        with tempfile.TemporaryDirectory() as tmp:
            cfg_path = Path(tmp) / "wc.json"
            cfg = wc.validate_config({"auto_connect": True, "mac": "E2:90:B5:F8:43:2E", "preset": "balanced"})
            coord = CoolerCoordinator(
                client=client,
                config=cfg,
                config_saver=lambda c: wc.save_config(c, cfg_path),
                temp_reader=lambda: 65.0,
                ec_reader=lambda: {},
                clock=clock,
                event_logger=events.append,
            )
            coord.tick()
            self.assertTrue(client.connected)
            self.assertEqual(client.preferred_write_type, "request")

            # Simulate sudden BLE link drop during meter poll write
            orig_write = tr.write_frame

            def failing_write(data: bytes, write_type: str = "request") -> None:
                raise RuntimeError("GDBus.Error:org.bluez.Error.NotConnected: Not Connected")

            tr.write_frame = failing_write
            clock.advance(2.1)
            coord.tick()
            self.assertFalse(client.connected)
            self.assertEqual(client.preferred_write_type, "request")
            self.assertTrue(any(e["event"] == "cooler_disconnected" and e["reason"] == "write_link_lost" for e in events))

            # Restore transport: must NOT reconnect immediately before LINK_DROP_RECONNECT_DELAY_S, then succeeds
            tr.write_frame = orig_write
            prev_calls = tr.connect_calls
            clock.advance(LINK_DROP_RECONNECT_DELAY_S - 0.5)
            coord.tick()
            self.assertFalse(client.connected)
            self.assertEqual(tr.connect_calls, prev_calls)

            clock.advance(0.6)
            coord.tick()
            self.assertTrue(client.connected)
            self.assertEqual(tr.connect_calls, prev_calls + 1)

    def test_auto_reconnect_backoff_wake_delay_and_consecutive_meter_errors(self):
        from iteglow.watercooling_service import RECONNECT_BACKOFF_S, WAKE_RECONNECT_DELAY_S

        clock = FakeClock(500.0)
        tr = FakeBleTransport(clock)
        client = CoolerBleClient(tr, min_gap_s=0.1, clock=clock)
        events = []

        # Force connect failures to verify backoff progression (3 -> 5 -> 8 -> 15)
        real_connect = tr.connect
        tr.connect = lambda **_kw: (_ for _ in ()).throw(RuntimeError("未扫描到机械革命蓝牙水冷设备"))
        with tempfile.TemporaryDirectory() as tmp:
            cfg_path = Path(tmp) / "wc.json"
            cfg = wc.validate_config({"auto_connect": True, "preset": "balanced"})
            coord = CoolerCoordinator(
                client=client,
                config=cfg,
                config_saver=lambda c: wc.save_config(c, cfg_path),
                temp_reader=lambda: 60.0,
                ec_reader=lambda: {},
                clock=clock,
                event_logger=events.append,
            )
            for expected_delay in RECONNECT_BACKOFF_S:
                coord.tick()
                self.assertEqual(events[-1]["event"], "cooler_connect_failed")
                self.assertEqual(events[-1]["next_retry_s"], expected_delay)
                clock.advance(expected_delay - 0.5)
                before = len(events)
                coord.tick()
                self.assertEqual(len(events), before)
                clock.advance(0.6)

            # Wake from sleep waits WAKE_RECONNECT_DELAY_S before reconnecting
            tr.connect = real_connect
            coord.prepare_for_sleep(True)
            coord.prepare_for_sleep(False)
            clock.advance(WAKE_RECONNECT_DELAY_S - 0.5)
            coord.tick()
            self.assertFalse(client.connected)
            clock.advance(0.6)
            coord.tick()
            self.assertTrue(client.connected)

            # Two consecutive non-disconnect meter errors force a clean disconnect so auto-reconnect can recover
            def transient_err(_data: bytes, write_type: str = "request") -> None:
                raise RuntimeError("transient gatt timeout")

            tr.write_frame = transient_err
            clock.advance(2.1)
            coord.tick()
            self.assertTrue(client.connected)
            clock.advance(2.1)
            coord.tick()
            self.assertFalse(client.connected)
            self.assertEqual(coord.last_reason, "disconnected:meter_poll_failed")

    def test_bluez_scan_tolerates_in_progress_and_matches_alias(self):
        from iteglow.watercooling_ble import BluezBleTransport

        tr = BluezBleTransport()
        calls = []

        class FakeVariant:
            def __init__(self, *_a):
                pass

        class FakeGLib:
            Variant = FakeVariant

        tr._bus = object()
        tr._GLib = FakeGLib
        tr.pump = lambda _s: None

        def fake_call(path, iface, method, params=None, timeout_ms=12000):
            calls.append(method)
            if method in ("SetDiscoveryFilter", "StartDiscovery"):
                raise RuntimeError("GDBus.Error:org.bluez.Error.InProgress: Operation already in progress")
            return None

        tr._call = fake_call
        tr._managed_objects = lambda: {
            "/org/bluez/hci0/dev_E2_90_B5_F8_43_2E": {
                "org.bluez.Device1": {
                    "Address": "E2:90:B5:F8:43:2E",
                    "Name": "",
                    "Alias": "CoolingSystem LCT22002",
                    "RSSI": -61,
                    "Connected": False,
                    "Paired": False,
                }
            }
        }
        found = tr.scan(timeout_s=0.1, target_mac="")
        self.assertEqual(len(found), 1)
        self.assertEqual(found[0]["address"], "E2:90:B5:F8:43:2E")
        self.assertNotIn("StopDiscovery", calls)

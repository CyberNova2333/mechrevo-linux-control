# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
"""Pure protocol, validation, thermal-curve, and flow-safety engine for MECHREVO BLE water coolers.

Verified against CoolingSystem LCT22002 (MCU F/W 2.0.0.4) live hardware and OEM GCUService.
Importing this module performs no D-Bus, BLE, sysfs, or filesystem writes.
"""
from __future__ import annotations

from dataclasses import dataclass
import json
import os
from pathlib import Path
import re
import time
from typing import Dict, List, Optional, Tuple

from . import state as state_io

NUS_SERVICE_UUID = "6e400001-b5a3-f393-e0a9-e50e24dcca9e"
NUS_TX_UUID = "6e400002-b5a3-f393-e0a9-e50e24dcca9e"
NUS_RX_UUID = "6e400003-b5a3-f393-e0a9-e50e24dcca9e"
DEVICE_NAME_PREFIXES = ("CoolingSystem LCT22002", "CoolingSystem LCT21001")

PUMP_VOLT_TO_CODE: Dict[int, int] = {11: 0, 12: 1, 7: 2, 8: 3}
VALID_PUMP_VOLTS = (0, 7, 8, 11, 12)
LED_MODES: Dict[str, int] = {
    "off": 0,
    "static": 0,
    "breathing": 1,
    "colorful": 2,
    "breathing_colorful": 3,
    "mode4": 4,
}
FAN_LED_POLICIES = ("sync", "body_only", "freeze", "split")
STRATEGIES = ("manual", "preset", "smart_curve", "ec_follow")

PRESETS: Dict[str, Dict[str, int]] = {
    "stop": {"pump_volt": 0, "pump_duty": 0, "fan_duty": 0},
    "silent": {"pump_volt": 7, "pump_duty": 60, "fan_duty": 40},
    "balanced": {"pump_volt": 8, "pump_duty": 60, "fan_duty": 50},
    "performance": {"pump_volt": 11, "pump_duty": 60, "fan_duty": 60},
    "turbo": {"pump_volt": 11, "pump_duty": 90, "fan_duty": 90},
    "max": {"pump_volt": 12, "pump_duty": 100, "fan_duty": 100},
}

MAC_RE = re.compile(r"^[0-9A-Fa-f]{2}(:[0-9A-Fa-f]{2}){5}$")
DEFAULT_WMI_STATE_PATH = Path("/run/mechrevo-wmi-state.json")
DEFAULT_LC_EC_STATE_PATH = Path("/run/mechrevo-watercooler-ec.json")


def config_path() -> Path:
    base = os.environ.get("XDG_CONFIG_HOME") or os.path.expanduser("~/.config")
    return Path(base) / "iteglow" / "watercooling.json"


def _int(value: object, low: int, high: int, field: str) -> int:
    if type(value) is not int or not low <= value <= high:
        raise ValueError(f"{field}: 需要 {low}–{high} 的整数")
    return value


def _rgb(value: object, field: str) -> List[int]:
    if not isinstance(value, (list, tuple)) or len(value) != 3:
        raise ValueError(f"{field}: 需要 3 个 RGB 通道")
    return [_int(c, 0, 255, field) for c in value]


def default_config() -> dict:
    return {
        "version": 1,
        "auto_connect": False,
        "mac": "",
        "strategy": "preset",
        "preset": "silent",
        "manual": {"pump_volt": 7, "pump_duty": 60, "fan_duty": 40},
        "curve": {
            "hysteresis_c": 3,
            "points": [
                {"temp_c": 45, "pump_volt": 7, "pump_duty": 60, "fan_duty": 40},
                {"temp_c": 65, "pump_volt": 8, "pump_duty": 70, "fan_duty": 55},
                {"temp_c": 80, "pump_volt": 11, "pump_duty": 90, "fan_duty": 85},
            ],
        },
        "led": {
            "mode": "off",
            "color": [0, 128, 255],
            "fan_policy": "sync",
            "fan_color": [0, 128, 255],
        },
        "flow_protection": True,
        "sleep_on_disconnect": True,
    }


def _validate_stage(stage: object, label: str) -> dict:
    if not isinstance(stage, dict):
        raise ValueError(f"{label}: 配置必须是对象")
    req = {"pump_volt", "pump_duty", "fan_duty"}
    if not req.issubset(stage):
        raise ValueError(f"{label}: 缺少 pump_volt / pump_duty / fan_duty")
    pv = stage["pump_volt"]
    if type(pv) is not int or pv not in VALID_PUMP_VOLTS:
        raise ValueError(f"{label}.pump_volt: 仅支持 0, 7, 8, 11, 12")
    pd = _int(stage["pump_duty"], 0, 100, f"{label}.pump_duty")
    fd = _int(stage["fan_duty"], 0, 100, f"{label}.fan_duty")
    if pv == 0:
        pd = 0
    elif pd == 0:
        raise ValueError(f"{label}: 水泵非 0V 时 pump_duty 必须为 1–100")
    return {"pump_volt": pv, "pump_duty": pd, "fan_duty": fd}


def validate_config(raw: object) -> dict:
    """Validate and return a detached canonical watercooling configuration."""
    if not isinstance(raw, dict):
        raise ValueError("水冷配置必须是 JSON 对象")
    allowed = set(default_config())
    unknown = set(raw) - allowed
    if unknown:
        raise ValueError(f"水冷配置包含未知字段: {sorted(unknown)}")
    cfg = default_config()
    if raw.get("version", 1) != 1 or type(raw.get("version", 1)) is not int:
        raise ValueError("不支持的水冷配置版本")
    for key in ("auto_connect", "flow_protection", "sleep_on_disconnect"):
        if key in raw:
            if type(raw[key]) is not bool:
                raise ValueError(f"{key}: 需要布尔值")
            cfg[key] = raw[key]
    if "mac" in raw:
        mac = raw["mac"]
        if not isinstance(mac, str) or (mac != "" and not MAC_RE.fullmatch(mac)):
            raise ValueError("mac: 需要空字符串或 XX:XX:XX:XX:XX:XX")
        cfg["mac"] = mac.upper()
    if "strategy" in raw:
        if raw["strategy"] not in STRATEGIES:
            raise ValueError(f"strategy: 仅支持 {STRATEGIES}")
        cfg["strategy"] = raw["strategy"]
    if "preset" in raw:
        if raw["preset"] not in PRESETS:
            raise ValueError(f"preset: 仅支持 {tuple(PRESETS)}")
        cfg["preset"] = raw["preset"]
    if "manual" in raw:
        m = raw["manual"]
        if not isinstance(m, dict) or set(m) != {"pump_volt", "pump_duty", "fan_duty"}:
            raise ValueError("manual: 字段必须为 pump_volt, pump_duty, fan_duty")
        cfg["manual"] = _validate_stage(m, "manual")
    if "curve" in raw:
        c = raw["curve"]
        if not isinstance(c, dict) or set(c) != {"hysteresis_c", "points"}:
            raise ValueError("curve: 字段必须为 hysteresis_c, points")
        hyst = _int(c["hysteresis_c"], 1, 15, "curve.hysteresis_c")
        pts = c["points"]
        if not isinstance(pts, list) or not 2 <= len(pts) <= 6:
            raise ValueError("curve.points: 需要 2–6 个温控节点")
        parsed_pts = []
        prev_t = -1
        for idx, pt in enumerate(pts):
            if not isinstance(pt, dict) or set(pt) != {"temp_c", "pump_volt", "pump_duty", "fan_duty"}:
                raise ValueError(f"curve.points[{idx}]: 字段不完整或有多余字段")
            t = _int(pt["temp_c"], 25, 100, f"curve.points[{idx}].temp_c")
            if t <= prev_t:
                raise ValueError("curve.points: temp_c 必须严格递增")
            prev_t = t
            st = _validate_stage(pt, f"curve.points[{idx}]")
            parsed_pts.append({"temp_c": t, **st})
        cfg["curve"] = {"hysteresis_c": hyst, "points": parsed_pts}
    if "led" in raw:
        led = raw["led"]
        if not isinstance(led, dict) or not {"mode", "color", "fan_policy", "fan_color"}.issuperset(led):
            raise ValueError("led: 包含未知字段")
        merged_led = dict(cfg["led"], **led)
        if merged_led["mode"] not in LED_MODES:
            raise ValueError(f"led.mode: 仅支持 {tuple(LED_MODES)}")
        if merged_led["fan_policy"] not in FAN_LED_POLICIES:
            raise ValueError(f"led.fan_policy: 仅支持 {FAN_LED_POLICIES}")
        merged_led["color"] = _rgb(merged_led["color"], "led.color")
        merged_led["fan_color"] = _rgb(merged_led["fan_color"], "led.fan_color")
        cfg["led"] = merged_led
    return cfg


def load_config(path: Optional[Path] = None) -> dict:
    p = Path(path) if path is not None else config_path()
    from .lighting_session import _read_saved
    missing = object()
    raw = _read_saved(p, os.getuid(), missing=missing, writable_mask=0o022)
    return default_config() if raw is missing else validate_config(raw)


def save_config(cfg: dict, path: Optional[Path] = None) -> dict:
    canonical = validate_config(cfg)
    p = Path(path) if path is not None else config_path()
    state_io.write(str(p), canonical)
    return canonical


# ---------------------------------------------------------------- BLE framing

def encode_handshake() -> bytes:
    return b"sw"


def encode_meter_query() -> bytes:
    return bytes((0xFE, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEF))


def encode_sleep() -> bytes:
    return bytes((0xFE, 0x19, 0x00, 0x01, 0x00, 0x00, 0x00, 0xEF))


def encode_priming(enable: bool) -> bytes:
    if enable:
        return bytes((0xFE, 0x19, 0x01, 0x04, 0x50, 0x00, 0x00, 0xEF))
    return bytes((0xFE, 0x19, 0x00, 0x04, 0x00, 0x00, 0x00, 0xEF))


def encode_fan(duty: int) -> bytes:
    d = _int(duty, 0, 100, "fan_duty")
    if d == 0:
        return bytes((0xFE, 0x1B, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEF))
    return bytes((0xFE, 0x1B, 0x01, d, 0x00, 0x00, 0x00, 0xEF))


def encode_pump(volt: int, duty: int = 60) -> bytes:
    if type(volt) is not int or volt not in VALID_PUMP_VOLTS:
        raise ValueError("pump_volt: 仅支持 0, 7, 8, 11, 12")
    d = _int(duty, 0, 100, "pump_duty")
    if volt == 0 or d == 0:
        return bytes((0xFE, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEF))
    return bytes((0xFE, 0x1C, 0x01, d, PUMP_VOLT_TO_CODE[volt], 0x00, 0x00, 0xEF))


def encode_main_led(on: bool, color: List[int], mode: str) -> bytes:
    if not on or mode == "off":
        return bytes((0xFE, 0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEF))
    if mode not in LED_MODES:
        raise ValueError(f"led.mode: 未知模式 {mode!r}")
    r, g, b = _rgb(color, "led.color")
    return bytes((0xFE, 0x1E, 0x01, r, g, b, LED_MODES[mode], 0xEF))


def encode_fan_led_ctrl(freeze: bool, color: Tuple[int, int, int] = (0, 0, 0), mode_code: int = 0) -> bytes:
    """Build 0x33 frame.

    Verified on LCT22002 (2.0.0.4):
      - byte[2] == 0x00: fan LED syncs with / follows 0x1E main LED
      - byte[2] == 0x01: fan LED freezes at its current latched color/off state
    """
    if not freeze:
        return bytes((0xFE, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEF))
    r, g, b = (_int(c, 0, 255, "fan_color") for c in color)
    m = _int(mode_code, 0, 255, "fan_mode")
    return bytes((0xFE, 0x33, 0x01, r, g, b, m, 0xEF))


def encode_led_plan(led_cfg: dict) -> List[Tuple[bytes, str]]:
    """Build an ordered list of (frame, label) commands for LCT22002 dual-zone LEDs."""
    mode = led_cfg["mode"]
    color = led_cfg["color"]
    policy = led_cfg["fan_policy"]
    fan_color = led_cfg["fan_color"]

    if mode == "off":
        return [
            (encode_fan_led_ctrl(False), "led_sync_reset"),
            (encode_main_led(False, color, "off"), "led_all_off"),
        ]
    if policy == "sync":
        return [
            (encode_fan_led_ctrl(False), "led_sync_enable"),
            (encode_main_led(True, color, mode), "led_main"),
        ]
    if policy == "body_only":
        return [
            (encode_main_led(False, color, "off"), "led_clear_for_body_only"),
            (encode_fan_led_ctrl(True), "led_fan_freeze_off"),
            (encode_main_led(True, color, mode), "led_body_main"),
        ]
    if policy == "freeze":
        return [
            (encode_fan_led_ctrl(True), "led_fan_freeze"),
            (encode_main_led(True, color, mode), "led_body_main"),
        ]
    if policy == "split":
        return [
            (encode_fan_led_ctrl(False), "led_split_sync_fan"),
            (encode_main_led(True, fan_color, "static"), "led_split_latch_fan"),
            (encode_fan_led_ctrl(True), "led_split_freeze_fan"),
            (encode_main_led(True, color, mode), "led_split_body"),
        ]
    raise ValueError(f"Unknown fan_policy: {policy!r}")


def parse_rx_packet(data: bytes) -> dict:
    raw = bytes(data)
    if len(raw) >= 4 and raw[0] == 0xFE and raw[1] in (0x31, 0x32):
        return {
            "kind": "meter",
            "opcode": raw[1],
            "byte2": raw[2],
            "meter_state": raw[3],
            "hex": raw.hex(" "),
        }
    try:
        text = raw.decode("utf-8").strip("\x00\r\n ")
        fw_ver = text[17:].strip() if text.startswith("MCU F/W Version:") and len(text) > 17 else None
        return {"kind": "firmware", "text": text, "fw_version": fw_ver, "hex": raw.hex(" ")}
    except UnicodeDecodeError:
        return {"kind": "unknown", "hex": raw.hex(" ")}


# ----------------------------------------------------------- Telemetry readers

def read_host_temp_c(hwmon_root: Path = Path("/sys/class/hwmon")) -> Optional[float]:
    """Read highest safe host thermal sensor in Celsius (preferring CPU package)."""
    if not hwmon_root.exists():
        return None
    package_temps: List[float] = []
    fallback_temps: List[float] = []
    for dev in sorted(hwmon_root.iterdir()):
        try:
            name = (dev / "name").read_text(encoding="utf-8").strip()
        except OSError:
            continue
        if name not in ("coretemp", "k10temp", "zenpower", "acpitz", "nouveau", "amdgpu"):
            continue
        for inp in sorted(dev.glob("temp*_input")):
            try:
                milli = int(inp.read_text(encoding="utf-8").strip())
            except (OSError, ValueError):
                continue
            celsius = milli / 1000.0
            if not -20.0 <= celsius <= 140.0:
                continue
            label_path = inp.with_name(inp.name.replace("_input", "_label"))
            label = ""
            if label_path.exists():
                try:
                    label = label_path.read_text(encoding="utf-8").strip().lower()
                except OSError:
                    pass
            if name in ("coretemp", "k10temp", "zenpower") and ("package" in label or "tctl" in label or "tdie" in label):
                package_temps.append(celsius)
            else:
                fallback_temps.append(celsius)
    if package_temps:
        return round(max(package_temps), 1)
    if fallback_temps:
        return round(max(fallback_temps), 1)
    return None


def read_ec_watercooler_state(
    wmi_path: Path = DEFAULT_WMI_STATE_PATH,
    ec_path: Path = DEFAULT_LC_EC_STATE_PATH,
    *,
    max_age_ms: int = 15000,
) -> dict:
    """Safely read root-published read-only EC/WMI cooler telemetry."""
    result = {
        "wmi_available": False,
        "wmi_seq": None,
        "wmi_c1_seq": None,
        "ec_available": False,
        "ec_stale": True,
        "lc_supported": None,
        "lc_fan_table_supported": None,
        "lc_hwoc_supported": None,
        "lc_ec_enabled": None,
        "ec_fan_enabled": False,
        "ec_fan_duty": 0,
        "ec_pump_enabled": False,
        "ec_pump_volt": 0,
    }
    try:
        wmi = json.loads(wmi_path.read_text(encoding="utf-8"))
        if isinstance(wmi, dict):
            result["wmi_available"] = True
            if isinstance(wmi.get("wmi_seq"), int):
                result["wmi_seq"] = wmi["wmi_seq"]
            c1_seqs = [
                e["seq"]
                for e in wmi.get("wmi_events", [])
                if isinstance(e, dict) and e.get("code") == 193 and isinstance(e.get("seq"), int)
            ]
            if c1_seqs:
                result["wmi_c1_seq"] = max(c1_seqs)
    except (OSError, ValueError, TypeError):
        pass

    try:
        ec = json.loads(ec_path.read_text(encoding="utf-8"))
        if isinstance(ec, dict) and ec.get("abi") == 1:
            result["ec_available"] = True
            now_ms = int(time.monotonic() * 1000)
            updated = ec.get("updated_monotonic_ms")
            if isinstance(updated, int) and 0 <= now_ms - updated <= max_age_ms:
                result["ec_stale"] = False
            for k in (
                "lc_supported",
                "lc_fan_table_supported",
                "lc_hwoc_supported",
                "lc_ec_enabled",
                "ec_fan_enabled",
                "ec_fan_duty",
                "ec_pump_enabled",
                "ec_pump_volt",
            ):
                if k in ec:
                    result[k] = ec[k]
    except (OSError, ValueError, TypeError):
        pass
    return result


# --------------------------------------------------- Target & safety evaluators

@dataclass
class FlowSafetyMonitor:
    """Monitors BLE meter_state while pump is active and latches dry/stall faults."""

    grace_s: float = 5.0
    fault_hold_s: float = 3.0
    tripped: bool = False
    trip_reason: str = ""
    _pump_on_since: Optional[float] = None
    _flow_confirmed: bool = False
    _bad_since: Optional[float] = None

    def reset(self) -> None:
        self.tripped = False
        self.trip_reason = ""
        self._pump_on_since = None
        self._flow_confirmed = False
        self._bad_since = None

    def on_disconnect(self) -> None:
        self._pump_on_since = None
        self._flow_confirmed = False
        self._bad_since = None

    def update(
        self,
        *,
        now: float,
        pump_volt: int,
        meter_state: Optional[int],
        flow_protection: bool,
    ) -> bool:
        """Update safety state and return True if flow fault just tripped or is active."""
        if self.tripped:
            return True
        if not flow_protection or pump_volt <= 0:
            self._pump_on_since = None
            self._flow_confirmed = False
            self._bad_since = None
            return False

        if self._pump_on_since is None:
            self._pump_on_since = now
            self._flow_confirmed = meter_state == 2
            self._bad_since = None
            return False

        if meter_state == 2:
            self._flow_confirmed = True
            self._bad_since = None
            return False

        if not self._flow_confirmed:
            if now - self._pump_on_since >= self.grace_s:
                self.tripped = True
                self.trip_reason = f"水泵开启超过 {self.grace_s:.0f}s 未检测到水流 (meter_state={meter_state})"
                return True
            return False

        if self._bad_since is None:
            self._bad_since = now
            return False
        if now - self._bad_since >= self.fault_hold_s:
            self.tripped = True
            self.trip_reason = f"水泵运行中水流信号丢失超过 {self.fault_hold_s:.0f}s (meter_state={meter_state})"
            return True
        return False


def evaluate_cooling_target(
    cfg: dict,
    *,
    temp_c: Optional[float],
    ec_state: Optional[dict],
    current_tier: int = 0,
    flow_tripped: bool = False,
    sleeping: bool = False,
) -> Tuple[int, int, int, int, str]:
    """Compute (pump_volt, pump_duty, fan_duty, new_tier_idx, reason)."""
    if sleeping:
        return 0, 0, 0, current_tier, "sleeping"
    if flow_tripped:
        return 0, 0, 0, current_tier, "flow_fault_locked"

    strategy = cfg["strategy"]
    if strategy == "manual":
        m = cfg["manual"]
        return m["pump_volt"], m["pump_duty"], m["fan_duty"], current_tier, "manual"

    if strategy == "preset":
        p = PRESETS[cfg["preset"]]
        return p["pump_volt"], p["pump_duty"], p["fan_duty"], current_tier, f"preset:{cfg['preset']}"

    if strategy == "ec_follow":
        if ec_state and ec_state.get("ec_available") and not ec_state.get("ec_stale"):
            pv = ec_state.get("ec_pump_volt", 0) if ec_state.get("ec_pump_enabled") else 0
            fd = ec_state.get("ec_fan_duty", 0) if ec_state.get("ec_fan_enabled") else 0
            if pv in VALID_PUMP_VOLTS and 0 <= fd <= 100:
                pd = 60 if pv > 0 else 0
                return pv, pd, fd, current_tier, "ec_follow"
        p = PRESETS["silent"]
        return p["pump_volt"], p["pump_duty"], p["fan_duty"], current_tier, "ec_fallback_silent"

    # smart_curve with hysteresis
    points = cfg["curve"]["points"]
    hyst = cfg["curve"]["hysteresis_c"]
    idx = min(max(current_tier, 0), len(points) - 1)
    if temp_c is None:
        pt = points[0]
        return pt["pump_volt"], pt["pump_duty"], pt["fan_duty"], 0, "curve_no_sensor_safe_floor"

    # Step up immediately whenever temperature reaches a higher point threshold
    for i in range(len(points) - 1, -1, -1):
        if temp_c >= points[i]["temp_c"]:
            if i >= idx:
                idx = i
            break
    # Step down only when temp drops below the current active point threshold by hysteresis
    while idx > 0 and temp_c <= points[idx]["temp_c"] - hyst:
        idx -= 1

    pt = points[idx]
    return pt["pump_volt"], pt["pump_duty"], pt["fan_duty"], idx, f"smart_curve:{temp_c:.1f}C->tier{idx}"

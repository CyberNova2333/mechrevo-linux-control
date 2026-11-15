"""Pure AC/DC brightness and shared idle-light policy; never performs I/O.

``saved`` contains the user's last *manually applied* records, never temporary
policy outputs. Only acknowledge actions after the existing device backend has
successfully applied them. Keep this memory in the session, not in saved state.
"""
from __future__ import annotations

from dataclasses import dataclass, replace
import json
import math
from typing import Optional, Tuple

from . import effects, protocol

DEVICES = ("keyboard", "lightbar", "logo")
LOGO_COLORS = ((255, 255, 255), (255, 0, 0), (0, 255, 0), (0, 0, 255),
               (255, 255, 0), (136, 0, 255), (0, 255, 255))


def default_config():
    return {"version": 1, "enabled": False, "power_enabled": False,
            "idle_minutes": 0,
            "brightness": {device: {"ac": 100, "dc": 100} for device in DEVICES}}


def _integer(value, low, high, field):
    if type(value) is not int or not low <= value <= high:
        raise ValueError(f"{field}: 需要 {low}–{high} 的整数")
    return value


def validate_config(config):
    """Return a detached, canonical config; reject unknown fields, not clamp."""
    if not isinstance(config, dict) or set(config) != set(default_config()):
        raise ValueError("灯光策略配置字段不完整或包含未知字段")
    if type(config["version"]) is not int or config["version"] != 1:
        raise ValueError("不支持的灯光策略版本")
    for key in ("enabled", "power_enabled"):
        if type(config[key]) is not bool:
            raise ValueError(f"{key}: 需要布尔值")
    _integer(config["idle_minutes"], 0, 1440, "空闲分钟数")
    brightness = config["brightness"]
    if not isinstance(brightness, dict) or set(brightness) != set(DEVICES):
        raise ValueError("需要键盘、灯带和 Logo 三个灯区的亮度")
    for device, values in brightness.items():
        if not isinstance(values, dict) or set(values) != {"ac", "dc"}:
            raise ValueError(f"{device}: 需要独立 AC / DC 亮度")
        for source, value in values.items():
            _integer(value, 0, 100, f"{device} {source} 亮度")
            if device == "logo" and value % 25:
                raise ValueError("Logo 亮度仅支持 0、25、50、75、100%")
    return json.loads(_encode(config))


def _encode(value):
    return json.dumps(value, sort_keys=True, separators=(",", ":"))


def _color(value):
    if not isinstance(value, (list, tuple)) or len(value) != 3:
        raise ValueError("RGB 颜色需要三个通道")
    return [_integer(channel, 0, 255, "颜色") for channel in value]


def _baseline(device, saved):
    """Validate source records without importing the execution backend."""
    if not isinstance(saved, dict):
        raise ValueError("已应用灯光记录必须是对象")
    saved = dict(saved)
    if device == "logo":
        if set(saved) != {"mode", "brightness", "color"}:
            raise ValueError("Logo 记录字段错误")
        if saved["mode"] not in ("solid", "breathing", "rainbow"):
            raise ValueError("未知 Logo 灯效")
        if _integer(saved["brightness"], 0, 100, "Logo 亮度") % 25:
            raise ValueError("Logo 亮度必须是 25% 的倍数")
        saved["color"] = _color(saved["color"])
        if saved["mode"] == "breathing" and tuple(saved["color"]) not in LOGO_COLORS:
            raise ValueError("不支持的 Logo 呼吸颜色")
    else:
        if set(saved) - {"mode", "brightness", "color", "colors", "speed", "direction", "reactive"}:
            raise ValueError("灯光记录包含未知字段")
        if not isinstance(saved.get("mode"), str) or saved["mode"] not in effects.MODES_BY_LABEL[device]:
            raise ValueError("未知灯效")
        maximum = protocol.KBD_BRIGHTNESS_MAX if device == "keyboard" else protocol.LB_BRIGHTNESS_MAX
        if saved.get("brightness") is not None:
            _integer(saved["brightness"], 0, maximum, "亮度")
        if "color" in saved:
            saved["color"] = _color(saved["color"])
        if saved.get("colors") is not None:
            if not isinstance(saved["colors"], (tuple, list)) or not 1 <= len(saved["colors"]) <= 7:
                raise ValueError("颜色列表需要 1–7 项")
            saved["colors"] = [_color(color) for color in saved["colors"]]
        if "speed" in saved:
            _integer(saved["speed"], 1, 10, "速度")
        if "direction" in saved:
            _integer(saved["direction"], 1, 4 if device == "keyboard" else 2, "方向")
        if "reactive" in saved and type(saved["reactive"]) is not bool:
            raise ValueError("按键响应需要布尔值")
    return saved


def brightness_value(device, percent):
    """Nearest hardware step; integer half-up rounding (keyboard has 50 steps)."""
    _integer(percent, 0, 100, "亮度百分比")
    if device not in DEVICES:
        raise ValueError("未知灯区")
    if device == "logo" and percent % 25:
        raise ValueError("Logo 亮度必须是 25% 的倍数")
    maximum = protocol.KBD_BRIGHTNESS_MAX if device == "keyboard" else 100
    return (percent * maximum + 50) // 100


@dataclass(frozen=True)
class AppliedZone:
    device: str
    baseline_json: str
    params_json: str
    active_json: str
    idle: bool


@dataclass(frozen=True)
class PolicyMemory:
    zones: Tuple[AppliedZone, ...] = ()


@dataclass(frozen=True)
class PolicyAction(AppliedZone):
    reason: str
    release: bool = False

    @property
    def params(self):
        return json.loads(self.params_json)


@dataclass(frozen=True)
class PolicyPlan:
    actions: Tuple[PolicyAction, ...]
    memory: PolicyMemory
    errors: Tuple[str, ...] = ()
    status: str = "disabled"


def action_matches(action, saved):
    """Recheck manual state immediately before executing a queued action."""
    try:
        return _encode(_baseline(action.device, saved[action.device])) == action.baseline_json
    except (KeyError, ValueError, TypeError):
        return False


def acknowledge(memory, action):
    """Only call on successful writes; failed zones remain eligible for retry."""
    zones = [zone for zone in memory.zones if zone.device != action.device]
    if not action.release:
        zones.append(AppliedZone(action.device, action.baseline_json, action.params_json,
                                 action.active_json, action.idle))
    return PolicyMemory(tuple(sorted(zones, key=lambda zone: DEVICES.index(zone.device))))


def plan(config, *, power, idle_seconds, saved, memory=None):
    """Calculate changes without reading files, clocks, D-Bus or hardware.

    ``power`` is 'ac', 'dc', or None; ``idle_seconds`` is nonnegative or None.
    Unknown inputs retain the last known policy output rather than guessing.
    Assign ``plan.memory`` first (it forgets stale ownership), execute each
    action only while ``action_matches`` and the session gate still hold, and
    acknowledge each successful action independently.
    """
    config = validate_config(config)
    if power not in ("ac", "dc", None):
        raise ValueError("power 必须是 ac、dc 或 None")
    if idle_seconds is not None and (type(idle_seconds) not in (int, float)
                                    or not math.isfinite(idle_seconds) or idle_seconds < 0):
        raise ValueError("idle_seconds 必须是非负秒数或 None")
    if not isinstance(saved, dict) or set(saved) - set(DEVICES):
        raise ValueError("已应用记录包含未知灯区")
    memory = memory or PolicyMemory()
    previous = {zone.device: zone for zone in memory.zones}
    owned, actions, errors = [], [], []
    for device in DEVICES:
        if device not in saved:
            continue
        try:
            baseline = _baseline(device, saved[device])
        except (ValueError, TypeError) as error:
            errors.append(f"{device}: {error}")
            continue
        baseline_json = _encode(baseline)
        old = previous.get(device)
        if old and old.baseline_json != baseline_json:
            old = None  # A manual edit takes ownership; never resurrect old effects.
        if old:
            owned.append(old)
        if not config["enabled"]:
            if old and old.params_json != baseline_json:
                actions.append(PolicyAction(device, baseline_json, baseline_json,
                                            baseline_json, False, "disabled", True))
            continue
        manually_off = (baseline.get("mode") == "off" or baseline.get("brightness") == 0
                        or (device == "logo" and baseline["mode"] == "solid"
                            and all(value * 10 // 51 == 0 for value in baseline["color"])))
        active = dict(baseline)
        if config["power_enabled"] and not manually_off:
            if power is not None:
                active["brightness"] = brightness_value(device, config["brightness"][device][power])
            elif old:
                active = json.loads(old.active_json)
        active_json = _encode(active)
        idle = (idle_seconds >= config["idle_minutes"] * 60 if idle_seconds is not None
                else bool(old and old.idle)) if config["idle_minutes"] else False
        effective = dict(active)
        if idle and not manually_off:
            effective["brightness"] = 0
        effective_json = _encode(effective)
        current_json = old.params_json if old else baseline_json
        if effective_json != current_json:
            actions.append(PolicyAction(device, baseline_json, effective_json, active_json,
                                        idle, "idle" if idle else "active" if old and old.idle else "power",
                                        effective_json == baseline_json))
        elif old:
            # An AC/DC change while idle updates only the future restore target.
            owned[owned.index(old)] = replace(old, active_json=active_json, idle=idle)
    return PolicyPlan(tuple(actions), PolicyMemory(tuple(owned)), tuple(errors),
                      "enabled" if config["enabled"] else "disabled")

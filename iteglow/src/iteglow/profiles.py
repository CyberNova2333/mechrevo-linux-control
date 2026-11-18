# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and ADDITIONAL_TERMS.zh-CN.md; ref: ADDITIONAL_TERMS.en.md).
"""Named performance + lighting profile presets and safe app-triggered switching.

Profiles and auto-switch rules are stored in ``~/.config/iteglow/profiles.json``,
strictly separate from ``~/.local/state/iteglow/state.json``.
Importing this module performs no D-Bus, /proc, EC, or HID I/O.
"""
from __future__ import annotations

import copy
from dataclasses import dataclass
import json
import os
from pathlib import Path
import stat
import subprocess
import tempfile
import time
from typing import Callable, Dict, Iterable, List, Optional, Sequence, Tuple

from . import effects, lighting_session, protocol, state
from .zone_lock import ZoneTransaction

PERF_MODES = ("office", "balanced", "turbo", "custom")
TURBO_VARIANTS = ("quiet", "extreme")
MIN_FAN_DUTY = (40, 40, 40, 40, 50, 55, 60, 70, 80, 90, 100)
MAX_PROFILE_BYTES = 262144
MAX_PROFILES = 32
MAX_RULES = 32
WMI_MODE_KEY_CODE = 176
IGNORED_SELF_TOKENS = frozenset({"iteglow", "mechrevo-control", "mechrevo-profile-helper"})


def profiles_path() -> Path:
    """Return the user's Track 3 profile configuration path."""
    base = Path(os.environ.get("XDG_CONFIG_HOME") or (Path.home() / ".config"))
    return base / "iteglow" / "profiles.json"


def default_config() -> dict:
    """Return the default opt-out configuration with auto-switching disabled."""
    return {
        "version": 1,
        "auto_switch_enabled": False,
        "restore_previous": True,
        "cooldown_seconds": 5,
        "profiles": {},
        "rules": [],
    }


def _int_range(value, low: int, high: int, field: str) -> int:
    if type(value) is not int or not low <= value <= high:
        raise ValueError(f"{field}: 需要 {low}..{high} 的整数")
    return value


def _clean_text(value, max_len: int, field: str, *, forbid_sep: bool = False) -> str:
    if not isinstance(value, str):
        raise ValueError(f"{field}: 需要字符串")
    text = value.strip()
    if not text or len(text) > max_len:
        raise ValueError(f"{field}: 长度需在 1..{max_len} 之间")
    if any(ord(ch) < 32 or ord(ch) == 127 for ch in text):
        raise ValueError(f"{field}: 不能包含控制字符")
    if forbid_sep and ("/" in text or "\\" in text):
        raise ValueError(f"{field}: 不能包含路径分隔符")
    return text


def validate_tuning(config: dict) -> dict:
    """Validate custom tuning config identically to mechrevo-profile-helper."""
    required = {"pl1", "pl2", "cpu_fan", "gpu_fan"}
    if not isinstance(config, dict) or set(config) not in (
        required,
        required | {"gpu_ctgp"},
        required | {"gpu_ctgp", "gpu_db"},
    ):
        raise ValueError("tuning: 自定义调校字段不完整或包含未知字段")
    pl1 = _int_range(config["pl1"], 25, 75, "tuning.pl1")
    pl2 = _int_range(config["pl2"], 25, 75, "tuning.pl2")
    if pl1 != pl2:
        raise ValueError("tuning: pl1 与 pl2 需保持一致 (25..75 W)")
    out: dict = {"pl1": pl1, "pl2": pl2}
    for fan_key in ("cpu_fan", "gpu_fan"):
        curve = config[fan_key]
        if not isinstance(curve, list) or len(curve) != 11:
            raise ValueError(f"tuning.{fan_key}: 风扇曲线需要 11 个节点")
        clean_curve = [
            _int_range(v, MIN_FAN_DUTY[idx], 100, f"tuning.{fan_key}[{idx}]")
            for idx, v in enumerate(curve)
        ]
        if clean_curve != sorted(clean_curve):
            raise ValueError(f"tuning.{fan_key}: 风扇占空比需随温度单调不减")
        out[fan_key] = clean_curve
    if "gpu_ctgp" in config:
        out["gpu_ctgp"] = _int_range(config["gpu_ctgp"], 65, 115, "tuning.gpu_ctgp")
    if "gpu_db" in config:
        db = config["gpu_db"]
        if type(db) is not int or (db != 0 and not 5 <= db <= 25):
            raise ValueError("tuning.gpu_db: 仅支持 0 或 5..25 W")
        out["gpu_db"] = db
    return out


def validate_performance(perf: Optional[dict]) -> Optional[dict]:
    """Validate optional performance block of a profile."""
    if perf is None:
        return None
    if not isinstance(perf, dict) or set(perf) - {"mode", "turbo", "tuning"}:
        raise ValueError("performance: 包含未知字段")
    mode = perf.get("mode")
    if not isinstance(mode, str) or mode not in PERF_MODES:
        raise ValueError("performance.mode: 必须是 office/balanced/turbo/custom")
    out: dict = {"mode": mode}
    turbo = perf.get("turbo")
    if turbo is not None:
        if mode != "turbo" or turbo not in TURBO_VARIANTS:
            raise ValueError("performance.turbo: 仅 turbo 模式支持 quiet/extreme")
        out["turbo"] = turbo
    tuning = perf.get("tuning")
    if tuning is not None:
        if mode != "custom":
            raise ValueError("performance.tuning: 仅 custom 模式支持调校参数")
        out["tuning"] = validate_tuning(tuning)
    return out


def validate_lighting(lighting: Optional[dict]) -> Optional[dict]:
    """Validate optional lighting block reusing lighting_session zone validators."""
    if lighting is None:
        return None
    if not isinstance(lighting, dict) or set(lighting) - {"keyboard", "lightbar", "logo"}:
        raise ValueError("lighting: 仅支持 keyboard / lightbar / logo")
    out: dict = {}
    for zone in ("keyboard", "lightbar"):
        if zone in lighting and lighting[zone] is not None:
            raw = copy.deepcopy(lighting[zone])
            lighting_session.validate_hid(zone, raw)
            out[zone] = json.loads(json.dumps(raw))
    if "logo" in lighting and lighting["logo"] is not None:
        raw_logo = copy.deepcopy(lighting["logo"])
        lighting_session.validate_logo(raw_logo)
        out["logo"] = json.loads(json.dumps(raw_logo))
    return out or None


def validate_profile(profile: dict) -> dict:
    """Validate one named performance+lighting preset bundle."""
    if not isinstance(profile, dict) or set(profile) - {"label", "performance", "lighting"}:
        raise ValueError("profile: 包含未知字段")
    out: dict = {}
    if "label" in profile and profile["label"] is not None:
        out["label"] = _clean_text(profile["label"], 80, "profile.label")
    perf = validate_performance(profile.get("performance"))
    lighting = validate_lighting(profile.get("lighting"))
    if perf is None and lighting is None:
        raise ValueError("profile: 至少需要包含性能或灯光配置")
    if perf is not None:
        out["performance"] = perf
    if lighting is not None:
        out["lighting"] = lighting
    return out


def validate_rule(rule: dict, profile_names: Iterable[str]) -> dict:
    """Validate one app auto-switch rule."""
    if not isinstance(rule, dict) or set(rule) != {"name", "enabled", "match", "profile"}:
        raise ValueError("rule: 需要 name / enabled / match / profile 字段")
    name = _clean_text(rule["name"], 64, "rule.name")
    enabled = rule["enabled"]
    if type(enabled) is not bool:
        raise ValueError("rule.enabled: 需要布尔值")
    raw_match = rule["match"]
    if not isinstance(raw_match, (list, tuple)) or not 1 <= len(raw_match) <= 8:
        raise ValueError("rule.match: 需要 1..8 个匹配关键词")
    clean_match = [_clean_text(item, 128, "rule.match[]").lower() for item in raw_match]
    prof = _clean_text(rule["profile"], 48, "rule.profile", forbid_sep=True)
    if prof not in profile_names:
        raise ValueError(f"rule.profile: 引用了不存在的预设 {prof!r}")
    return {"name": name, "enabled": enabled, "match": clean_match, "profile": prof}


def validate_config(config: dict) -> dict:
    """Validate and canonicalize full profiles.json payload."""
    expected_keys = set(default_config())
    if not isinstance(config, dict) or set(config) != expected_keys:
        raise ValueError("profiles 配置字段不完整或包含未知字段")
    if type(config["version"]) is not int or config["version"] != 1:
        raise ValueError("不支持的 profiles 配置版本")
    for key in ("auto_switch_enabled", "restore_previous"):
        if type(config[key]) is not bool:
            raise ValueError(f"{key}: 需要布尔值")
    cooldown = _int_range(config["cooldown_seconds"], 0, 120, "cooldown_seconds")
    raw_profiles = config["profiles"]
    if not isinstance(raw_profiles, dict) or len(raw_profiles) > MAX_PROFILES:
        raise ValueError(f"profiles: 最多支持 {MAX_PROFILES} 个预设")
    profiles: Dict[str, dict] = {}
    for raw_name, raw_profile in raw_profiles.items():
        clean_name = _clean_text(raw_name, 48, "profile name", forbid_sep=True)
        if clean_name in profiles:
            raise ValueError(f"profile name 重复: {clean_name!r}")
        profiles[clean_name] = validate_profile(raw_profile)
    raw_rules = config["rules"]
    if not isinstance(raw_rules, list) or len(raw_rules) > MAX_RULES:
        raise ValueError(f"rules: 最多支持 {MAX_RULES} 条规则")
    rules = [validate_rule(item, profiles.keys()) for item in raw_rules]
    return {
        "version": 1,
        "auto_switch_enabled": config["auto_switch_enabled"],
        "restore_previous": config["restore_previous"],
        "cooldown_seconds": cooldown,
        "profiles": profiles,
        "rules": rules,
    }


def builtin_templates() -> Dict[str, dict]:
    """Return validated built-in starter preset bundles (never auto-applied)."""
    templates = {
        "quiet-office": {
            "label": "静音办公 · 低亮蓝色背光",
            "performance": {"mode": "office"},
            "lighting": {
                "keyboard": {"mode": "solid", "color": [0, 180, 255], "brightness": 20},
                "lightbar": {"mode": "off"},
                "logo": {"mode": "solid", "brightness": 0, "color": [255, 255, 255]},
            },
        },
        "balanced-daily": {
            "label": "均衡日常 · 正弦波背光",
            "performance": {"mode": "balanced"},
            "lighting": {
                "keyboard": {"mode": "sine", "brightness": 35, "speed": 5, "direction": 1},
                "lightbar": {"mode": "off"},
            },
        },
        "gaming-fps": {
            "label": "FPS 狂暴 · WASD+方向键高亮",
            "performance": {"mode": "turbo", "turbo": "extreme"},
            "lighting": {
                "keyboard": {
                    "mode": "matrix",
                    "preset": "wasd_fps",
                    "color": [0, 255, 255],
                    "brightness": 50,
                    "calibrate": True,
                },
            },
        },
        "dark-focus": {
            "label": "暗夜专注 · 全部熄灯",
            "performance": {"mode": "balanced"},
            "lighting": {
                "keyboard": {"mode": "off"},
                "lightbar": {"mode": "off"},
                "logo": {"mode": "solid", "brightness": 0, "color": [255, 255, 255]},
            },
        },
    }
    return {name: validate_profile(body) for name, body in templates.items()}


def load_config(path: Optional[Path] = None, uid: Optional[int] = None) -> dict:
    """Safely read and validate profiles.json (rejects symlinks and oversized files)."""
    target = Path(path) if path is not None else profiles_path()
    owner = os.getuid() if uid is None else uid
    saved = lighting_session._read_saved(target, owner, max_bytes=MAX_PROFILE_BYTES)
    if saved is None:
        return default_config()
    return validate_config(saved)


def read_config(path: Optional[Path] = None, uid: Optional[int] = None) -> dict:
    """Alias matching lighting_policy_session.read_config naming."""
    return load_config(path, uid)


def save_config(path: Optional[Path], config: dict) -> dict:
    """Validate and atomically write profiles.json."""
    clean = validate_config(config)
    target = Path(path) if path is not None else profiles_path()
    target.parent.mkdir(parents=True, exist_ok=True)
    temporary = None
    try:
        with tempfile.NamedTemporaryFile(
            mode="w", encoding="utf-8", dir=target.parent, prefix=".profiles-", delete=False
        ) as stream:
            temporary = Path(stream.name)
            json.dump(clean, stream, ensure_ascii=False, indent=2)
            stream.write("\n")
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(temporary, target)
    finally:
        if temporary is not None:
            temporary.unlink(missing_ok=True)
    return clean


def _read_small_proc(path: Path, limit: int = 4096) -> bytes:
    with path.open("rb") as stream:
        return stream.read(limit)


def _extract_tokens(comm_text: str, cmdline_raw: bytes) -> Tuple[str, ...]:
    tokens = set()
    clean_comm = comm_text.strip().lower()
    if clean_comm:
        tokens.add(clean_comm)
    parts = [part.decode("utf-8", errors="ignore").strip() for part in cmdline_raw.split(b"\x00") if part]
    for idx, part in enumerate(parts[:6]):
        low = part.lower()
        if not low:
            continue
        if idx == 0:
            tokens.add(low)
        # Support Unix '/' and Proton/Wine '\' executable paths cleanly.
        base = low.replace("\\", "/").rsplit("/", 1)[-1]
        if base:
            tokens.add(base)
        if base.endswith(".exe") and len(base) > 4:
            tokens.add(base[:-4])
    return tuple(sorted(tokens))


def scan_processes(proc_root: Path = Path("/proc"), uid: Optional[int] = None) -> List[Tuple[str, ...]]:
    """Return normalized process token tuples owned by ``uid`` (never reads files outside /proc)."""
    owner = os.getuid() if uid is None else uid
    results: List[Tuple[str, ...]] = []
    try:
        entries = list(proc_root.iterdir())
    except OSError:
        return results
    for entry in entries:
        if not entry.name.isdigit():
            continue
        try:
            info = entry.stat()
            if not stat.S_ISDIR(info.st_mode) or info.st_uid != owner:
                continue
            comm_raw = _read_small_proc(entry / "comm", 256).decode("utf-8", errors="ignore")
            cmd_raw = _read_small_proc(entry / "cmdline", 4096)
        except OSError:
            continue
        if not cmd_raw:
            continue  # Skip kernel threads / zombies.
        tokens = _extract_tokens(comm_raw, cmd_raw)
        if not tokens or any(t in IGNORED_SELF_TOKENS or t.startswith("iteglow") or t.startswith("mechrevo-") for t in tokens):
            continue
        results.append(tokens)
    return results


def match_rule(rules: Sequence[dict], processes: Sequence[Sequence[str]]) -> Optional[dict]:
    """Return the first enabled rule whose match token hits any running process token."""
    for rule in rules:
        if not rule.get("enabled"):
            continue
        needles = rule.get("match", ())
        for tokens in processes:
            for needle in needles:
                if any(needle in token for token in tokens):
                    return rule
    return None


def read_wmi_snapshot(path: Path = lighting_session.WMI_STATE_PATH) -> Optional[dict]:
    """Read /run/mechrevo-wmi-state.json safely (returns None if unavailable)."""
    try:
        fd = os.open(path, os.O_RDONLY | os.O_CLOEXEC | os.O_NOFOLLOW | os.O_NONBLOCK)
    except OSError:
        return None
    try:
        with os.fdopen(fd, "rb") as stream:
            info = os.fstat(stream.fileno())
            if not stat.S_ISREG(info.st_mode) or info.st_size > 65536:
                return None
            data = stream.read(65537)
            if len(data) > 65536:
                return None
            payload = json.loads(data)
            return payload if isinstance(payload, dict) else None
    except (OSError, ValueError, TypeError):
        return None


def latest_wmi_code_seq(snapshot: Optional[dict], code: int = WMI_MODE_KEY_CODE) -> int:
    """Return highest event sequence number for ``code`` in WMI state snapshot (0 if absent)."""
    if not isinstance(snapshot, dict):
        return 0
    best = 0
    events = snapshot.get("wmi_events")
    if isinstance(events, list):
        for item in events:
            if (
                isinstance(item, dict)
                and item.get("code") == code
                and type(item.get("seq")) is int
                and item["seq"] > best
            ):
                best = item["seq"]
    if snapshot.get("wmi_last_code") == code and type(snapshot.get("wmi_seq")) is int:
        best = max(best, snapshot["wmi_seq"])
    return best


@dataclass(frozen=True)
class RuleDecision:
    action: str  # noop | apply | release | yield_mode_key | yielded | fan_safety_paused | hold | disabled
    rule_name: Optional[str] = None
    profile_name: Optional[str] = None
    restore_profile: Optional[dict] = None
    reason: str = ""


class RuleEvaluator:
    """Pure state machine for app-triggered profile switching with 0xB0 yield & cooldown."""

    def __init__(self):
        self.active_rule: Optional[str] = None
        self.active_profile: Optional[str] = None
        self.baseline_profile: Optional[dict] = None
        self.last_switch_time: Optional[float] = None
        self.missing_since: Optional[float] = None
        self.last_b0_seq: Optional[int] = None
        self.yielded_until_exit: Optional[str] = None

    def reset(self) -> None:
        self.active_rule = None
        self.active_profile = None
        self.baseline_profile = None
        self.missing_since = None
        self.yielded_until_exit = None

    def clear_active_keeping_yield(self) -> None:
        self.active_rule = None
        self.active_profile = None
        self.baseline_profile = None
        self.missing_since = None

    def step(
        self,
        config: dict,
        *,
        processes: Sequence[Sequence[str]],
        b0_seq: int = 0,
        fan_safety_protect: bool = False,
        now: Optional[float] = None,
        capture_baseline: Optional[Callable[[], Optional[dict]]] = None,
    ) -> RuleDecision:
        current_time = time.monotonic() if now is None else float(now)
        if self.last_b0_seq is None:
            self.last_b0_seq = b0_seq
        elif b0_seq > self.last_b0_seq:
            self.last_b0_seq = b0_seq
            if self.active_rule is not None:
                yielded = self.active_rule
                prev_prof = self.active_profile
                self.yielded_until_exit = yielded
                self.clear_active_keeping_yield()
                self.last_switch_time = current_time
                return RuleDecision(
                    "yield_mode_key",
                    rule_name=yielded,
                    profile_name=prev_prof,
                    reason="物理模式键 0xB0 已接管，自动切换让步",
                )

        if not config.get("auto_switch_enabled"):
            self.yielded_until_exit = None
            if self.active_rule is not None:
                prev_rule = self.active_rule
                prev_prof = self.active_profile
                restore = self.baseline_profile if config.get("restore_previous") else None
                self.reset()
                self.last_switch_time = current_time
                return RuleDecision(
                    "release",
                    rule_name=prev_rule,
                    profile_name=prev_prof,
                    restore_profile=restore,
                    reason="自动切换已关闭，恢复先前状态",
                )
            return RuleDecision("disabled")

        matched = match_rule(config.get("rules", ()), processes)
        cooldown = int(config.get("cooldown_seconds", 5))

        if self.yielded_until_exit is not None:
            if matched is not None and matched["name"] == self.yielded_until_exit:
                self.missing_since = None
                return RuleDecision("yielded", rule_name=matched["name"], profile_name=matched["profile"])
            if matched is None:
                if self.missing_since is None:
                    self.missing_since = current_time
                if (current_time - self.missing_since) < cooldown:
                    return RuleDecision("yielded", rule_name=self.yielded_until_exit)
            self.yielded_until_exit = None
            self.missing_since = None

        if matched is not None:
            self.missing_since = None
            if fan_safety_protect and config["profiles"][matched["profile"]].get("performance") is not None:
                return RuleDecision(
                    "fan_safety_paused",
                    rule_name=matched["name"],
                    profile_name=matched["profile"],
                    reason="EC 高温风扇安全保护激活，暂停自动性能切换",
                )
            if self.active_rule == matched["name"] and self.active_profile == matched["profile"]:
                return RuleDecision("noop", rule_name=matched["name"], profile_name=matched["profile"])
            if (
                self.active_rule is not None
                and self.last_switch_time is not None
                and (current_time - self.last_switch_time) < cooldown
            ):
                return RuleDecision("hold", rule_name=self.active_rule, profile_name=self.active_profile)
            if self.active_rule is None and capture_baseline is not None:
                self.baseline_profile = capture_baseline()
            self.active_rule = matched["name"]
            self.active_profile = matched["profile"]
            self.last_switch_time = current_time
            return RuleDecision("apply", rule_name=matched["name"], profile_name=matched["profile"])

        if self.active_rule is not None:
            if self.missing_since is None:
                self.missing_since = current_time
            if (current_time - self.missing_since) < cooldown:
                return RuleDecision("hold", rule_name=self.active_rule, profile_name=self.active_profile)
            prev_rule = self.active_rule
            prev_prof = self.active_profile
            restore = self.baseline_profile if config.get("restore_previous") else None
            if fan_safety_protect and restore and restore.get("performance") is not None:
                # Never touch performance EC while thermal safety is active; drop perf restore only.
                restore = {k: v for k, v in restore.items() if k != "performance"}
            self.clear_active_keeping_yield()
            self.last_switch_time = current_time
            return RuleDecision(
                "release",
                rule_name=prev_rule,
                profile_name=prev_prof,
                restore_profile=restore if restore else None,
                reason="匹配进程已退出，释放规则",
            )

        return RuleDecision("noop")


def default_helper_runner(args: Sequence[str]) -> dict:
    """Invoke mechrevo-profile-helper via pkexec (non-interactive) and parse JSON."""
    proc = subprocess.run(
        ["/usr/bin/pkexec", "--disable-internal-agent", lighting_session.HELPER, *args],
        capture_output=True, text=True, timeout=6, check=False,
    )
    raw = (proc.stdout or "").strip()
    try:
        data = json.loads(raw)
    except (ValueError, TypeError) as exc:
        raise RuntimeError((proc.stderr or "").strip() or f"性能助手返回无效响应: {exc}") from exc
    if proc.returncode or (isinstance(data, dict) and "error" in data):
        err = data.get("error") if isinstance(data, dict) else None
        raise RuntimeError(err or (proc.stderr or "").strip() or "性能助手执行失败")
    if not isinstance(data, dict):
        raise RuntimeError("性能助手响应格式无效")
    return data


def _read_manual_lighting(directory: Path, uid: int) -> dict:
    lighting: dict = {}
    saved_hid = lighting_session._read_saved(directory / "state.json", uid)
    if isinstance(saved_hid, dict):
        for zone in ("keyboard", "lightbar"):
            if zone in saved_hid:
                lighting[zone] = copy.deepcopy(saved_hid[zone])
    saved_logo = lighting_session._read_saved(directory / "logo.json", uid)
    if isinstance(saved_logo, dict):
        lighting["logo"] = copy.deepcopy(saved_logo)
    return lighting


def capture_current_profile(
    label: Optional[str] = None,
    *,
    directory: Optional[Path] = None,
    uid: Optional[int] = None,
    helper_runner: Optional[Callable[[Sequence[str]], dict]] = None,
    include_lighting: bool = True,
    include_performance: bool = True,
) -> dict:
    """Snapshot current lighting state and firmware performance mode into a validated profile."""
    runner = default_helper_runner if helper_runner is None else helper_runner
    profile: dict = {}
    if label:
        profile["label"] = _clean_text(label, 80, "label")
    if include_performance:
        st = runner(["status"])
        mode = st.get("mode")
        if mode in PERF_MODES:
            perf: dict = {"mode": mode}
            turbo = st.get("turbo") if isinstance(st.get("turbo"), dict) else {}
            if mode == "turbo" and turbo.get("active") and turbo.get("variant") in TURBO_VARIANTS:
                perf["turbo"] = turbo["variant"]
            elif mode == "custom" and st.get("tuning_active") and isinstance(st.get("tuning"), dict):
                perf["tuning"] = st["tuning"]
            profile["performance"] = perf
    if include_lighting:
        state_dir = (
            Path(directory)
            if directory is not None
            else Path(os.environ.get("XDG_STATE_HOME") or (Path.home() / ".local/state")) / "iteglow"
        )
        owner = os.getuid() if uid is None else uid
        light = _read_manual_lighting(state_dir, owner)
        if light:
            profile["lighting"] = light
    return validate_profile(profile)


def _apply_performance(
    perf: dict,
    *,
    helper_runner: Callable[[Sequence[str]], dict],
    ignore_errors: bool = False,
) -> Optional[str]:
    """Apply validated performance target while obeying fan_safety_protect and adapter limits."""
    st = helper_runner(["status"])
    if st.get("fan_safety_protect") is True:
        msg = "EC 高温风扇安全保护激活，跳过性能切换"
        if ignore_errors:
            return msg
        raise RuntimeError(msg)
    if not st.get("switchable"):
        msg = st.get("reason") or "当前固件状态不可通过软件切换"
        if ignore_errors:
            return msg
        raise RuntimeError(msg)

    target_mode = perf["mode"]
    current_mode = st.get("mode")
    turbo_info = st.get("turbo") if isinstance(st.get("turbo"), dict) else {}

    if target_mode == "custom" and perf.get("tuning") is not None:
        if st.get("tuning") != perf["tuning"]:
            helper_runner(["apply", json.dumps(perf["tuning"])])
        return None

    if current_mode != target_mode or (target_mode == "custom" and st.get("tuning_active")):
        helper_runner(["set", target_mode])
        st = helper_runner(["status"])
        turbo_info = st.get("turbo") if isinstance(st.get("turbo"), dict) else {}

    desired_turbo = perf.get("turbo") if target_mode == "turbo" else None
    if desired_turbo is not None:
        eligible = (
            turbo_info.get("available") is True
            and turbo_info.get("round_adapter") is True
            and turbo_info.get("combined_supported") is True
            and not st.get("tuning_active")
            and not st.get("tuning_recovery_pending")
        )
        if eligible and turbo_info.get("variant") != desired_turbo:
            helper_runner(["turbo", desired_turbo])
    return None


def apply_profile(
    profile: dict,
    *,
    helper_runner: Optional[Callable[[Sequence[str]], dict]] = None,
    hid_writer: Optional[Callable[[lighting_session.RestoreAction], object]] = None,
    transaction=ZoneTransaction,
    save_manual: bool = True,
    apply_lighting: bool = True,
    apply_performance: bool = True,
) -> dict:
    """Validate and apply a profile bundle's lighting and/or performance settings."""
    runner = default_helper_runner if helper_runner is None else helper_runner
    clean = validate_profile(profile)
    applied: dict = {"lighting": [], "performance": None, "warnings": []}
    writer = hid_writer or lighting_session.apply_action

    if apply_lighting and clean.get("lighting"):
        if hid_writer is None:
            from . import devices
            present = {lbl for _led, _usb, lbl in devices.available_devices()}
        else:
            present = None
        light = clean["lighting"]
        for zone in ("keyboard", "lightbar"):
            if zone not in light:
                continue
            if present is not None and zone not in present:
                applied["warnings"].append(f"{zone}: device not present")
                continue
            action = lighting_session.validate_hid(zone, light[zone])
            with transaction(zone):
                if writer(action) is False:
                    raise RuntimeError(f"{zone} writer reported failure")
                if save_manual:
                    state.save(zone, copy.deepcopy(light[zone]))
            applied["lighting"].append(zone)
        if "logo" in light:
            action = lighting_session.validate_logo(light["logo"])
            with transaction("logo"):
                if writer(action) is False:
                    raise RuntimeError("logo writer reported failure")
                if save_manual:
                    with transaction("manual-state"):
                        logo_path = Path(state.user_state_path()).with_name("logo.json")
                        state.write(str(logo_path), copy.deepcopy(light["logo"]))
            applied["lighting"].append("logo")

    if apply_performance and clean.get("performance"):
        warning = _apply_performance(clean["performance"], helper_runner=runner, ignore_errors=False)
        if warning:
            applied["warnings"].append(warning)
        else:
            applied["performance"] = clean["performance"]["mode"]
    return applied


class ProfileRuleRunner:
    """Session watcher adapter connecting RuleEvaluator with /proc, WMI, and hardware."""

    def __init__(
        self,
        *,
        active: Callable[[], bool],
        config: Callable[[], dict],
        directory: Path,
        uid: int,
        apply: bool = False,
        process_scanner: Optional[Callable[[], Sequence[Sequence[str]]]] = None,
        wmi_reader: Callable[[], Optional[dict]] = read_wmi_snapshot,
        helper_runner: Callable[[Sequence[str]], dict] = default_helper_runner,
        hid_writer: Optional[Callable[[lighting_session.RestoreAction], object]] = None,
        transaction=ZoneTransaction,
        clock: Callable[[], float] = time.monotonic,
        emit: Optional[Callable[[dict], None]] = None,
    ):
        self.active = active
        self.config = config
        self.directory = Path(directory)
        self.uid = uid
        self.apply = apply
        self.scanner = process_scanner or (lambda: scan_processes(uid=self.uid))
        self.wmi_reader = wmi_reader
        self.helper_runner = helper_runner
        self.hid_writer = hid_writer
        self.transaction = transaction
        self.clock = clock
        self.emit = emit
        self.evaluator = RuleEvaluator()
        self.owns_lighting = False

    def deactivate(self) -> None:
        self.evaluator.reset()
        self.owns_lighting = False

    def _capture_baseline(self) -> Optional[dict]:
        try:
            return capture_current_profile(
                directory=self.directory,
                uid=self.uid,
                helper_runner=self.helper_runner,
                include_lighting=True,
                include_performance=True,
            )
        except Exception:
            # Fall back to manual lighting baseline only if helper is unavailable.
            light = _read_manual_lighting(self.directory, self.uid)
            return {"lighting": light} if light else None

    def tick(self) -> dict:
        result: dict = {
            "action": "noop",
            "active_lighting": self.owns_lighting,
            "released_lighting": False,
            "errors": [],
        }
        try:
            if self.active() is not True:
                self.deactivate()
                result["action"] = "inactive"
                result["active_lighting"] = False
                return result
            cfg = validate_config(self.config())
            # Zero overhead when opt-in disabled and nothing owned.
            if not cfg["auto_switch_enabled"] and self.evaluator.active_rule is None:
                result["action"] = "disabled"
                return result
            wmi = self.wmi_reader()
            b0_seq = latest_wmi_code_seq(wmi, WMI_MODE_KEY_CODE)
            fan_safety = bool(wmi.get("fan_safety_protect")) if isinstance(wmi, dict) else False
            procs = self.scanner()
            decision = self.evaluator.step(
                cfg,
                processes=procs,
                b0_seq=b0_seq,
                fan_safety_protect=fan_safety,
                now=self.clock(),
                capture_baseline=self._capture_baseline,
            )
            result["action"] = decision.action
            result["rule"] = decision.rule_name
            result["profile"] = decision.profile_name
            if decision.reason:
                result["reason"] = decision.reason

            if decision.action == "apply" and decision.profile_name:
                target = cfg["profiles"][decision.profile_name]
                if self.apply:
                    apply_profile(
                        target,
                        helper_runner=self.helper_runner,
                        hid_writer=self.hid_writer,
                        transaction=self.transaction,
                        save_manual=False,
                    )
                if target.get("lighting"):
                    self.owns_lighting = True
                result["active_lighting"] = self.owns_lighting
            elif decision.action == "release":
                was_owning = self.owns_lighting
                self.owns_lighting = False
                result["active_lighting"] = False
                result["released_lighting"] = was_owning
                if self.apply and decision.restore_profile:
                    apply_profile(
                        decision.restore_profile,
                        helper_runner=self.helper_runner,
                        hid_writer=self.hid_writer,
                        transaction=self.transaction,
                        save_manual=False,
                    )
            elif decision.action == "yield_mode_key":
                was_owning = self.owns_lighting
                self.owns_lighting = False
                result["active_lighting"] = False
                result["released_lighting"] = was_owning
                if self.apply:
                    # On 0xB0 Mode Key yield, NEVER touch firmware performance mode;
                    # only restore saved manual lighting if a rule owned lighting.
                    if was_owning:
                        manual_light = _read_manual_lighting(self.directory, self.uid)
                        if manual_light:
                            apply_profile(
                                {"lighting": manual_light},
                                helper_runner=self.helper_runner,
                                hid_writer=self.hid_writer,
                                transaction=self.transaction,
                                save_manual=False,
                                apply_performance=False,
                            )
            if self.emit and decision.action not in ("noop", "disabled", "hold", "yielded"):
                self.emit({"event": "profile_rule", **result})
        except Exception as exc:
            result["errors"].append(str(exc))
            if self.emit:
                self.emit({"event": "profile_rule", **result})
        return result

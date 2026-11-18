# Portions added/modified by CyberNova are part of MECHREVO Linux Control and licensed under AGPL-3.0-only with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; upstream notices preserved).
"""iteglow command-line interface -- pure stdlib, no third-party deps."""
from __future__ import annotations

import argparse
import json
import os
import subprocess
import sys
from typing import List, Optional

from . import __version__, devices, effects, keymap, protocol, state
from . import watercooling as wc
from .protocol import RGB
from .watercooling_ble import CoolerBleClient
from .watercooling_service import is_service_running, run_daemon, service_request
from .zone_lock import ZoneTransaction


def _parse_color(s: str) -> RGB:
    s = s.strip().lstrip("#")
    if len(s) == 6 and all(c in "0123456789abcdefABCDEF" for c in s):
        return (int(s[0:2], 16), int(s[2:4], 16), int(s[4:6], 16))
    parts = s.split(",")
    if len(parts) == 3:
        try:
            return tuple(int(p) for p in parts)  # type: ignore[return-value]
        except ValueError:
            pass
    raise argparse.ArgumentTypeError(f"bad colour {s!r}; use #RRGGBB or r,g,b")


def _parse_color_value(value) -> RGB:
    if isinstance(value, str):
        return _parse_color(value)
    if isinstance(value, (list, tuple)) and len(value) == 3:
        return tuple(int(c) for c in value)  # type: ignore[return-value]
    raise argparse.ArgumentTypeError(f"bad colour entry {value!r}")


def _parse_matrix(spec: str) -> List[RGB]:
    text = spec.strip()
    if text.startswith("@"):
        text = text[1:]
    if os.path.isfile(text):
        with open(text) as f:
            data = json.load(f)
    else:
        try:
            data = json.loads(text)
        except ValueError as exc:
            raise argparse.ArgumentTypeError(f"invalid matrix JSON or file {spec!r}: {exc}") from exc
    if isinstance(data, dict):
        cells = keymap.blank_cells()
        by_name = {cap.name.lower(): cap for cap in keymap.ALL_KEYCAPS}
        for key, raw_col in data.items():
            cap = by_name.get(str(key).strip().lower())
            if cap is None:
                raise argparse.ArgumentTypeError(f"unknown key name {key!r} in matrix map")
            col = _parse_color_value(raw_col)
            for idx in cap.indices:
                cells[idx] = col
        return cells
    if isinstance(data, list) and len(data) == protocol.KBD_MATRIX_ROWS and all(
        isinstance(row, list) and len(row) == protocol.KBD_MATRIX_COLS for row in data
    ):
        return [_parse_color_value(cell) for row in data for cell in row]
    if isinstance(data, list) and len(data) == protocol.KBD_MATRIX_CELLS:
        return [_parse_color_value(cell) for cell in data]
    raise argparse.ArgumentTypeError(
        f"matrix must be 126 cells, 6x21 rows, or {{key: color}} JSON")


def _resolve_direction(label: str, value: str) -> int:
    table = effects.DIRECTIONS_BY_LABEL[label]
    if value in table:
        return table[value]
    try:
        return int(value)
    except ValueError:
        raise argparse.ArgumentTypeError(
            f"bad direction {value!r}; choose from {sorted(table)} or an integer")


# --------------------------------------------------------------------- commands

def cmd_doctor(_args) -> int:
    print(f"iteglow {__version__} (MECHREVO 本机适配)\n")
    print('本机使用已有 hid-generic/hidraw 接口；不要求加载 tuxedo-drivers。')
    mods = ""
    try:
        with open("/proc/modules") as f:
            mods = f.read()
    except OSError:
        pass
    for m in ("ite_8291", "ite_8291_lb"):
        print(f"  module {m:12s}: {'loaded' if m in mods else 'NOT loaded'}")
    print()
    ok = True
    for led, usb, label in devices.KNOWN:
        path = devices.hidraw_for_led(led)
        if not path:
            print(f"  {label:9s} ({usb}): not found  (LED node {led} missing)")
            ok = False
            continue
        access = os.access(path, os.R_OK | os.W_OK)
        extra = ""
        if access:
            try:
                with devices.HidLed(led) as dev:
                    fw = protocol.parse_fw_version(dev.query(protocol.QUERY_FW_VERSION))
                    extra = f"  fw {fw[0]}.{fw[1]}"
                    if label == "keyboard":
                        st = protocol.parse_keyboard_state(dev.query(protocol.QUERY_EFFECT))
                        onoff = "off" if st["power"] == 0x01 or st["brightness"] == 0 else "on"
                        mode_name, _ = effects.decode_keyboard_state(st)
                        extra += (f"  [{onoff}, {mode_name} 0x{st['mode']:02x}, "
                                  f"brightness {st['brightness']}]")
            except OSError:
                pass
        print(f"  {label:9s} ({usb}): {path}  "
              f"[{'read/write OK' if access else 'NO ACCESS -- install the udev rule for your desktop user'}]"
              f"{extra}")
        ok = ok and access
    if not ok:
        print("\nFixes:")
        print("  * 本机设备缺失 -> 检查 USB 设备和维护记录，不要为此强装平台驱动")
        print("  * NO ACCESS            -> sudo install -m644 data/60-iteglow.rules "
              "/etc/udev/rules.d/ && sudo udevadm control --reload && reboot")
    return 0 if ok else 1


def cmd_list(_args) -> int:
    present = {lbl for _led, _usb, lbl in devices.available_devices()}
    for label, modes in (("keyboard", effects.KEYBOARD_MODES),
                         ("lightbar", effects.LIGHTBAR_MODES)):
        tag = "" if label in present else "  (not detected)"
        print(f"{label}{tag}: {', '.join(modes)}")
    print(f"matrix presets: {', '.join(keymap.PRESETS)}")
    return 0


def _open(label: str) -> devices.HidLed:
    return devices.HidLed(devices.led_for_label(label))


def _user_lighting_only() -> bool:
    if os.geteuid() == 0:
        print('Run lighting commands as your normal desktop user, without sudo. '
              'The installed udev rule grants HID access; user-session locks must share that identity. '
              'Only "iteglow pin" needs sudo for exporting state.', file=sys.stderr)
        return False
    return True


def cmd_set(args) -> int:
    if not _user_lighting_only():
        return 1
    # default: first direction of the device's own table (forward / ltr)
    direction = _resolve_direction(args.device, args.direction) if args.direction else 1
    params = dict(color=args.color, colors=args.palette or None,
                  brightness=args.brightness, speed=args.speed, direction=direction,
                  reactive=args.reactive)
    matrix = getattr(args, "matrix", None)
    preset = getattr(args, "preset", None)
    calibrate = bool(getattr(args, "calibrate", False))
    build_kwargs = dict(params)
    if args.device == "keyboard" and args.mode == "matrix":
        if matrix is not None:
            params["matrix"] = [list(c) for c in matrix]
            build_kwargs["matrix"] = matrix
        elif preset:
            params["preset"] = preset
            build_kwargs["preset"] = preset
        if calibrate:
            params["calibrate"] = True
            build_kwargs["calibrate"] = True
    reports = effects.build(args.device, args.mode, **build_kwargs)
    with ZoneTransaction(args.device), _open(args.device) as dev:
        dev.send(reports)
        saved = state.save(args.device, dict(mode=args.mode, **params))
    if not saved:
        print(f"warning: {args.device} lighting applied, but restore settings could not be saved", file=sys.stderr)
    return 0


def cmd_off(args) -> int:
    if not _user_lighting_only():
        return 1
    targets = [args.device] if args.device else \
        [lbl for _led, _usb, lbl in devices.available_devices()]
    if not targets:
        print("no devices detected", file=sys.stderr)
        return 1
    for label in targets:
        with ZoneTransaction(label), _open(label) as dev:
            dev.send(effects.build(label, "off"))
            saved = state.save(label, dict(mode="off"))
        if not saved:
            print(f"warning: {label} lighting turned off, but restore settings could not be saved", file=sys.stderr)
    return 0


def _build_from_params(label: str, params: dict):
    colors = params.get("colors")
    matrix = params.get("matrix")
    return effects.build(
        label, params.get("mode", "off"),
        color=tuple(params.get("color") or (0, 255, 255)),
        colors=[tuple(c) for c in colors] if colors else None,
        brightness=params.get("brightness"),
        speed=params.get("speed", 5),
        direction=params.get("direction", 1),
        reactive=params.get("reactive", False),
        matrix=[tuple(c) for c in matrix] if matrix else None,
        preset=params.get("preset"),
        calibrate=bool(params.get("calibrate", False)),
    )


def cmd_restore(args) -> int:
    if not _user_lighting_only():
        return 1
    from .lighting_session import validate_hid
    path = state.SYSTEM_STATE_PATH if args.system else state.user_state_path()
    data = state.load(path)
    if not data:
        print(f"no saved state at {path}", file=sys.stderr)
        # An absent optional system export is a no-op for scripted restores.
        return 0 if args.system else 1
    if not isinstance(data, dict):
        print("invalid saved lighting state: expected an object", file=sys.stderr)
        return 1
    present = {lbl for _led, _usb, lbl in devices.available_devices()}
    restored = []
    invalid = False
    for label, params in data.items():
        if label not in present:
            continue
        try:
            reports = validate_hid(label, params).reports
        except (ValueError, TypeError) as error:
            print(f"invalid saved {label} state: {error}", file=sys.stderr)
            invalid = True
            continue
        with ZoneTransaction(label):
            # Another cooperating client may have saved a newer manual value
            # while this restore was waiting for the transaction.
            fresh = state.load(path)
            params = fresh.get(label) if isinstance(fresh, dict) else None
            if params is None:
                continue
            try:
                reports = validate_hid(label, params).reports
            except (ValueError, TypeError) as error:
                print(f"invalid saved {label} state: {error}", file=sys.stderr)
                invalid = True
                continue
            with _open(label) as dev:
                dev.send(reports)
        restored.append(f"{label}={params.get('mode', 'off')}")
    print("restored: " + (", ".join(restored) if restored else "nothing (no saved devices present)"))
    return 1 if invalid else 0


def cmd_pin(args) -> int:
    if os.geteuid() != 0:
        print("pin writes the system state; run it with sudo.", file=sys.stderr)
        return 1
    # find the invoking user's state file: pkexec (GUI) sets PKEXEC_UID,
    # sudo sets SUDO_USER
    src_user = None
    if os.environ.get("PKEXEC_UID"):
        import pwd
        src_user = pwd.getpwuid(int(os.environ["PKEXEC_UID"])).pw_name
    elif os.environ.get("SUDO_USER") not in (None, "root"):
        src_user = os.environ["SUDO_USER"]
    src = state.user_state_path(src_user)
    data = state.load(src)
    if not data:
        print(f"no saved state at {src}; run `iteglow set ...` (or the GUI) first.",
              file=sys.stderr)
        return 1
    state.write(state.SYSTEM_STATE_PATH, data)
    print(f"pinned {', '.join(sorted(data))} -> {state.SYSTEM_STATE_PATH}")
    print("Export only; restore it manually with `iteglow restore --system`.")
    print("Automatic restore uses your user state via iteglow-session-restore.service when configured.")
    return 0


def cmd_profile(args) -> int:
    if not _user_lighting_only():
        return 1
    from . import profiles
    path = profiles.profiles_path()
    try:
        cfg = profiles.load_config(path)
        action = args.profile_cmd
        if action == "list":
            print(f"config: {path}")
            print(f"auto_switch: {'on' if cfg['auto_switch_enabled'] else 'off'} "
                  f"(restore_previous={cfg['restore_previous']}, cooldown={cfg['cooldown_seconds']}s)")
            if cfg["profiles"]:
                print("profiles:")
                for name, body in cfg["profiles"].items():
                    perf = body.get("performance", {}).get("mode", "-")
                    turbo = body.get("performance", {}).get("turbo")
                    if turbo:
                        perf = f"{perf}/{turbo}"
                    zones = ",".join(sorted((body.get("lighting") or {}).keys())) or "-"
                    label = f" — {body['label']}" if body.get("label") else ""
                    print(f"  {name}: perf={perf} lights={zones}{label}")
            else:
                print("profiles: (none — run `iteglow profile init-templates` to add starters)")
            if cfg["rules"]:
                print("rules:")
                for rule in cfg["rules"]:
                    st = "on" if rule["enabled"] else "off"
                    print(f"  [{st}] {rule['name']}: match={','.join(rule['match'])} -> {rule['profile']}")
            return 0
        if action == "init-templates":
            added = []
            for name, body in profiles.builtin_templates().items():
                if name not in cfg["profiles"]:
                    cfg["profiles"][name] = body
                    added.append(name)
            profiles.save_config(path, cfg)
            print("added templates: " + (", ".join(added) if added else "none (already present)"))
            return 0
        if action == "save":
            prof = profiles.capture_current_profile(
                args.label,
                include_lighting=not args.no_lighting,
                include_performance=not args.no_performance,
            )
            cfg["profiles"][args.name] = prof
            profiles.save_config(path, cfg)
            print(f"saved profile {args.name!r} -> {path}")
            return 0
        if action == "apply":
            if args.name not in cfg["profiles"]:
                raise ValueError(f"unknown profile {args.name!r}")
            res = profiles.apply_profile(
                cfg["profiles"][args.name],
                save_manual=True,
                apply_lighting=not args.performance_only,
                apply_performance=not args.lighting_only,
            )
            print(json.dumps({"applied": args.name, **res}, ensure_ascii=False))
            return 0
        if action == "delete":
            if args.name not in cfg["profiles"]:
                raise ValueError(f"unknown profile {args.name!r}")
            refs = [r["name"] for r in cfg["rules"] if r["profile"] == args.name]
            if refs and not args.force:
                raise ValueError(f"profile {args.name!r} is referenced by rules {refs}; pass --force to delete both")
            del cfg["profiles"][args.name]
            if refs:
                cfg["rules"] = [r for r in cfg["rules"] if r["profile"] != args.name]
            profiles.save_config(path, cfg)
            print(f"deleted profile {args.name!r}")
            return 0
        if action == "rule-add":
            cfg["rules"] = [r for r in cfg["rules"] if r["name"] != args.name]
            cfg["rules"].append({
                "name": args.name,
                "enabled": not args.disabled,
                "match": list(args.match),
                "profile": args.profile,
            })
            profiles.save_config(path, cfg)
            print(f"saved rule {args.name!r} -> {args.profile!r}")
            return 0
        if action == "rule-del":
            before = len(cfg["rules"])
            cfg["rules"] = [r for r in cfg["rules"] if r["name"] != args.name]
            if len(cfg["rules"]) == before:
                raise ValueError(f"unknown rule {args.name!r}")
            profiles.save_config(path, cfg)
            print(f"deleted rule {args.name!r}")
            return 0
        if action == "auto":
            if args.state in ("on", "off"):
                cfg["auto_switch_enabled"] = (args.state == "on")
                if args.cooldown is not None:
                    cfg["cooldown_seconds"] = args.cooldown
                if args.restore is not None:
                    cfg["restore_previous"] = args.restore
                profiles.save_config(path, cfg)
            print(json.dumps({
                "auto_switch_enabled": cfg["auto_switch_enabled"],
                "restore_previous": cfg["restore_previous"],
                "cooldown_seconds": cfg["cooldown_seconds"],
                "rules": len(cfg["rules"]),
            }, ensure_ascii=False))
            return 0
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 1
    return 1


def _save_local_wc_patch(patch: dict) -> dict:
    cur = wc.load_config()
    merged = dict(cur)
    for k, v in patch.items():
        if k in ("manual", "curve", "led") and isinstance(v, dict) and isinstance(merged.get(k), dict):
            sub = dict(merged[k])
            sub.update(v)
            merged[k] = sub
        else:
            merged[k] = v
    return wc.save_config(merged)


def cmd_cooler(args) -> int:
    action = args.cooler_action
    try:
        if action == "daemon":
            return run_daemon()
        if action == "ec-status":
            print(json.dumps({
                "temp_c": wc.read_host_temp_c(),
                "ec": wc.read_ec_watercooler_state(),
            }, ensure_ascii=False, indent=2))
            return 0
        if action == "status":
            if is_service_running():
                res = service_request({"op": "status"})
            else:
                res = {
                    "service_running": False,
                    "connected": False,
                    "temp_c": wc.read_host_temp_c(),
                    "ec": wc.read_ec_watercooler_state(),
                    "config": wc.load_config(),
                }
            print(json.dumps(res, ensure_ascii=False, indent=2))
            return 0

        if not _user_lighting_only():
            return 1

        if action == "scan":
            if is_service_running():
                devs = service_request({"op": "scan", "timeout_s": args.timeout}).get("devices", [])
            else:
                devs = CoolerBleClient().scan(timeout_s=args.timeout, target_mac=args.mac or "")
            print(json.dumps({"devices": devs}, ensure_ascii=False, indent=2))
            return 0
        if action == "connect":
            if not is_service_running():
                raise RuntimeError("水冷守护进程未运行，请先执行 `systemctl --user start iteglow-watercooling.service` 或 `iteglow cooler daemon`")
            res = service_request({"op": "connect", "mac": args.mac or ""}, timeout_s=24.0)
            print(json.dumps(res, ensure_ascii=False, indent=2))
            return 0
        if action == "disconnect":
            if not is_service_running():
                raise RuntimeError("水冷守护进程未运行")
            res = service_request({"op": "disconnect", "send_sleep": not args.no_sleep})
            print(json.dumps(res, ensure_ascii=False, indent=2))
            return 0
        if action == "sleep":
            if is_service_running():
                res = service_request({"op": "disconnect", "send_sleep": True})
            else:
                cli = CoolerBleClient()
                cli.connect(target_mac=args.mac or wc.load_config().get("mac", ""))
                cli.disconnect(send_sleep=True, stop_actuators=True)
                res = {"connected": False, "sleep_sent": True}
            print(json.dumps(res, ensure_ascii=False, indent=2))
            return 0
        if action == "reset-fault":
            res = service_request({"op": "reset_fault"})
            print(json.dumps(res, ensure_ascii=False, indent=2))
            return 0
        if action == "emergency-stop":
            if is_service_running():
                res = service_request({"op": "emergency_stop"})
            else:
                saved = _save_local_wc_patch({"strategy": "preset", "preset": "stop", "led": {"mode": "off"}})
                res = {"service_running": False, "config": saved}
            print(json.dumps(res, ensure_ascii=False, indent=2))
            return 0
        if action == "set":
            patch = {}
            if args.strategy is not None:
                patch["strategy"] = args.strategy
            if args.preset is not None:
                patch["preset"] = args.preset
            if args.mac is not None:
                patch["mac"] = args.mac
            if args.auto_connect is not None:
                patch["auto_connect"] = args.auto_connect == "on"
            if args.flow_protection is not None:
                patch["flow_protection"] = args.flow_protection == "on"
            if args.sleep_on_disconnect is not None:
                patch["sleep_on_disconnect"] = args.sleep_on_disconnect == "on"
            manual = {}
            if args.pump_volt is not None:
                manual["pump_volt"] = args.pump_volt
            if args.pump_duty is not None:
                manual["pump_duty"] = args.pump_duty
            if args.fan_duty is not None:
                manual["fan_duty"] = args.fan_duty
            if manual:
                base_manual = dict(wc.load_config()["manual"])
                base_manual.update(manual)
                patch["manual"] = base_manual
            if is_service_running():
                res = service_request({"op": "set_config", "config": patch, "apply_now": not args.no_apply})
            else:
                res = {"service_running": False, "config": _save_local_wc_patch(patch)}
            print(json.dumps(res, ensure_ascii=False, indent=2))
            return 0
        if action == "led":
            led_patch = {"mode": args.mode}
            if args.color is not None:
                led_patch["color"] = list(args.color)
            if args.fan_policy is not None:
                led_patch["fan_policy"] = args.fan_policy
            if args.fan_color is not None:
                led_patch["fan_color"] = list(args.fan_color)
            if is_service_running():
                res = service_request({"op": "set_config", "config": {"led": led_patch}, "apply_now": True})
            else:
                res = {"service_running": False, "config": _save_local_wc_patch({"led": led_patch})}
            print(json.dumps(res, ensure_ascii=False, indent=2))
            return 0
    except (OSError, ValueError, RuntimeError) as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 1
    return 1


# --------------------------------------------------------------------- parser

def build_parser() -> argparse.ArgumentParser:
    p = argparse.ArgumentParser(prog="iteglow", description=__doc__)
    p.add_argument("--version", action="version", version=f"iteglow {__version__}")
    sub = p.add_subparsers(dest="cmd", required=True)

    sub.add_parser("doctor", help="check drivers, devices and permissions").set_defaults(func=cmd_doctor)
    sub.add_parser("list", help="list devices and their modes").set_defaults(func=cmd_list)

    s = sub.add_parser("set", help="apply a mode to a device")
    s.add_argument("device", choices=["keyboard", "lightbar"])
    s.add_argument("mode", help="mode name (see `iteglow list`)")
    s.add_argument("--color", type=_parse_color, default=(0, 255, 255),
                   help="single colour for static/solid (#RRGGBB or r,g,b)")
    s.add_argument("--palette", type=_parse_color, nargs="+", metavar="COLOR",
                   help="up to 7 colours for multi-colour effects")
    s.add_argument("--brightness", type=int, default=None, help="0..100 (lightbar) / 0..50 (keyboard)")
    s.add_argument("--speed", type=int, default=5, help="1 (fast) .. 10 (slow)")
    s.add_argument("--reactive", action="store_true", help="按键触发（随机闪烁/涟漪/极光/烟花）；默认自动播放")
    s.add_argument("--direction", default=None,
                   help="lightbar: forward/reverse; keyboard: ltr/rtl/btt/ttb "
                        "(default: forward / ltr)")
    s.add_argument("--preset", choices=sorted(keymap.PRESETS), default=None,
                   help="per-key preset when mode=matrix")
    s.add_argument("--matrix", type=_parse_matrix, default=None,
                   help="per-key JSON file/@file or inline JSON (126 RGB cells, 6x21, or {key: color})")
    s.add_argument("--calibrate", action="store_true",
                   help="apply OEM white/primary color calibration for keyboard matrix mode")
    s.set_defaults(func=cmd_set)

    o = sub.add_parser("off", help="turn a device (or all) off")
    o.add_argument("device", nargs="?", choices=["keyboard", "lightbar"])
    o.set_defaults(func=cmd_off)

    r = sub.add_parser("restore", help="re-apply the last saved state")
    r.add_argument("--system", action="store_true",
                   help="manually restore the exported system state")
    r.set_defaults(func=cmd_restore)

    pin = sub.add_parser(
        "pin", help="export your saved state system-wide; does not enable automatic restore (sudo)")
    pin.set_defaults(func=cmd_pin)

    pr = sub.add_parser("profile", help="manage performance+lighting profiles and app rules")
    pr_sub = pr.add_subparsers(dest="profile_cmd", required=True)
    pr_sub.add_parser("list", help="list saved profiles and auto-switch rules")
    pr_sub.add_parser("init-templates", help="populate starter profile templates")
    ps = pr_sub.add_parser("save", help="capture current state into a named profile")
    ps.add_argument("name")
    ps.add_argument("--label", default=None)
    ps.add_argument("--no-performance", action="store_true")
    ps.add_argument("--no-lighting", action="store_true")
    pa = pr_sub.add_parser("apply", help="apply a saved profile")
    pa.add_argument("name")
    pa_g = pa.add_mutually_exclusive_group()
    pa_g.add_argument("--lighting-only", action="store_true")
    pa_g.add_argument("--performance-only", action="store_true")
    pd = pr_sub.add_parser("delete", help="delete a saved profile")
    pd.add_argument("name")
    pd.add_argument("--force", action="store_true")
    pra = pr_sub.add_parser("rule-add", help="add or update an app auto-switch rule")
    pra.add_argument("name")
    pra.add_argument("profile")
    pra.add_argument("--match", nargs="+", required=True)
    pra.add_argument("--disabled", action="store_true")
    prd = pr_sub.add_parser("rule-del", help="delete an app auto-switch rule")
    prd.add_argument("name")
    pau = pr_sub.add_parser("auto", help="configure or inspect app auto-switching")
    pau.add_argument("state", choices=["on", "off", "status"])
    pau.add_argument("--cooldown", type=int, default=None)
    pau.add_argument("--restore", dest="restore", action="store_true", default=None)
    pau.add_argument("--no-restore", dest="restore", action="store_false")
    pr.set_defaults(func=cmd_profile)

    wc_p = sub.add_parser("cooler", help="control external BLE water cooler (LCT22002 / LCT21001)")
    wc_sub = wc_p.add_subparsers(dest="cooler_action", required=True)
    wc_sub.add_parser("status", help="show cooler status, telemetry, and saved config")
    wc_sub.add_parser("ec-status", help="show read-only host EC/WMI cooler telemetry")
    wc_sc = wc_sub.add_parser("scan", help="scan nearby BLE water coolers")
    wc_sc.add_argument("--timeout", type=float, default=5.0)
    wc_sc.add_argument("--mac", default="")
    wc_cn = wc_sub.add_parser("connect", help="connect to water cooler via user daemon")
    wc_cn.add_argument("--mac", default="")
    wc_dc = wc_sub.add_parser("disconnect", help="safely stop and disconnect cooler")
    wc_dc.add_argument("--no-sleep", action="store_true")
    wc_sl = wc_sub.add_parser("sleep", help="send GoToSleepLC (0x19 00 01) and disconnect")
    wc_sl.add_argument("--mac", default="")
    wc_sub.add_parser("reset-fault", help="reset latched flow-safety fault")
    wc_sub.add_parser("emergency-stop", help="stop pump/fan/LEDs and put cooler to sleep")
    wc_st = wc_sub.add_parser("set", help="update cooler strategy, preset, manual stage, or safety flags")
    wc_st.add_argument("--strategy", choices=wc.STRATEGIES)
    wc_st.add_argument("--preset", choices=tuple(wc.PRESETS))
    wc_st.add_argument("--pump-volt", type=int, choices=wc.VALID_PUMP_VOLTS)
    wc_st.add_argument("--pump-duty", type=int)
    wc_st.add_argument("--fan-duty", type=int)
    wc_st.add_argument("--mac", default=None)
    wc_st.add_argument("--auto-connect", choices=("on", "off"))
    wc_st.add_argument("--flow-protection", choices=("on", "off"))
    wc_st.add_argument("--sleep-on-disconnect", choices=("on", "off"))
    wc_st.add_argument("--no-apply", action="store_true")
    wc_ld = wc_sub.add_parser("led", help="configure cooler dual-zone LED (0x1E + 0x33)")
    wc_ld.add_argument("mode", choices=tuple(wc.LED_MODES))
    wc_ld.add_argument("--color", type=_parse_color)
    wc_ld.add_argument("--fan-policy", choices=wc.FAN_LED_POLICIES)
    wc_ld.add_argument("--fan-color", type=_parse_color)
    wc_sub.add_parser("daemon", help="run unprivileged desktop-user water-cooling BLE daemon")
    wc_p.set_defaults(func=cmd_cooler)

    g = sub.add_parser("gui", help="launch the graphical control panel")
    g.set_defaults(func=lambda a: _launch_gui())
    return p


def _launch_gui() -> int:
    try:
        from . import gui
    except Exception as e:  # noqa: BLE001
        print(f"GUI unavailable: {e}\nInstall PyGObject/GTK4 (see README).", file=sys.stderr)
        return 1
    return gui.main()


def main(argv: Optional[List[str]] = None) -> int:
    args = build_parser().parse_args(argv)
    try:
        return args.func(args)
    except argparse.ArgumentTypeError as e:
        print(f"error: {e}", file=sys.stderr)
        return 2
    except (FileNotFoundError, PermissionError) as e:
        print(f"error: {e}", file=sys.stderr)
        if isinstance(e, PermissionError):
            print("hint: install the udev rule or run with sudo (`iteglow doctor`).",
                  file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())

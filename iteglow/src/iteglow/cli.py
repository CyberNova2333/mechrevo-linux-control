"""iteglow command-line interface -- pure stdlib, no third-party deps."""
from __future__ import annotations

import argparse
import os
import sys
from typing import List, Optional

from . import __version__, devices, effects, protocol, state
from .protocol import RGB


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
                        extra += (f"  [{onoff}, mode 0x{st['mode']:02x}, "
                                  f"brightness {st['brightness']}]")
            except OSError:
                pass
        print(f"  {label:9s} ({usb}): {path}  "
              f"[{'read/write OK' if access else 'NO ACCESS -- install udev rule or use sudo'}]"
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
    return 0


def _open(label: str) -> devices.HidLed:
    return devices.HidLed(devices.led_for_label(label))


def cmd_set(args) -> int:
    # default: first direction of the device's own table (forward / ltr)
    direction = _resolve_direction(args.device, args.direction) if args.direction else 1
    params = dict(color=args.color, colors=args.palette or None,
                  brightness=args.brightness, speed=args.speed, direction=direction,
                  reactive=args.reactive)
    reports = effects.build(args.device, args.mode, **params)
    with _open(args.device) as dev:
        dev.send(reports)
    state.save(args.device, dict(mode=args.mode, **params))
    return 0


def cmd_off(args) -> int:
    targets = [args.device] if args.device else \
        [lbl for _led, _usb, lbl in devices.available_devices()]
    if not targets:
        print("no devices detected", file=sys.stderr)
        return 1
    for label in targets:
        with _open(label) as dev:
            dev.send(effects.build(label, "off"))
        state.save(label, dict(mode="off"))
    return 0


def _build_from_params(label: str, params: dict):
    colors = params.get("colors")
    return effects.build(
        label, params.get("mode", "off"),
        color=tuple(params.get("color") or (0, 255, 255)),
        colors=[tuple(c) for c in colors] if colors else None,
        brightness=params.get("brightness"),
        speed=params.get("speed", 5),
        direction=params.get("direction", 1),
        reactive=params.get("reactive", False),
    )


def cmd_restore(args) -> int:
    path = state.SYSTEM_STATE_PATH if args.system else state.user_state_path()
    data = state.load(path)
    if not data:
        print(f"no saved state at {path}", file=sys.stderr)
        # absence is not an error for the boot/resume service
        return 0 if args.system else 1
    present = {lbl for _led, _usb, lbl in devices.available_devices()}
    restored = []
    for label, params in data.items():
        if label not in present:
            continue
        reports = _build_from_params(label, params)
        if label == "lightbar":
            # re-enable the controller: lost on suspend and never restored by
            # the kernel driver (tuxedo-drivers issue #371)
            reports = protocol.lightbar_power_on() + reports
        with _open(label) as dev:
            dev.send(reports)
        restored.append(f"{label}={params.get('mode', 'off')}")
    print("restored: " + (", ".join(restored) if restored else "nothing (no saved devices present)"))
    return 0


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
    print("iteglow-restore.service will re-apply this on every boot and resume.")
    return 0


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
    s.set_defaults(func=cmd_set)

    o = sub.add_parser("off", help="turn a device (or all) off")
    o.add_argument("device", nargs="?", choices=["keyboard", "lightbar"])
    o.set_defaults(func=cmd_off)

    r = sub.add_parser("restore", help="re-apply the last saved state")
    r.add_argument("--system", action="store_true",
                   help="use the pinned system state (what boot/resume applies)")
    r.set_defaults(func=cmd_restore)

    pin = sub.add_parser(
        "pin", help="pin your last-set state for every boot and resume (sudo)")
    pin.set_defaults(func=cmd_pin)

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

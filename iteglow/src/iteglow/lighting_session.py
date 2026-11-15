"""Restore last-applied lights in one active local graphical session.

Default operation is a dry run. ``--apply`` explicitly enables the existing
HID protocols and the fixed, validated Logo helper. The watcher also evaluates
explicitly enabled session lighting policy; --once only previews/restores manual
records. No performance settings, startup defaults or system-wide pins are used.
Importing this module does not access devices, D-Bus or saved files.
"""
from __future__ import annotations

import argparse
from dataclasses import dataclass
import json
import os
from pathlib import Path
import signal
import stat
import subprocess
from typing import Callable, Optional, Tuple

from . import effects, protocol

HELPER = "/usr/local/libexec/mechrevo-profile-helper"
MAX_STATE_BYTES = 65536
LOGO_COLORS = ((255, 255, 255), (255, 0, 0), (0, 255, 0), (0, 0, 255),
               (255, 255, 0), (136, 0, 255), (0, 255, 255))


def _int(value, low, high, field):
    if type(value) is not int or not low <= value <= high:
        raise ValueError(f"{field}: expected integer {low}..{high}")
    return value


def _color(value):
    if not isinstance(value, (list, tuple)) or len(value) != 3:
        raise ValueError("color: expected three RGB channels")
    return tuple(_int(channel, 0, 255, "color") for channel in value)


@dataclass(frozen=True)
class RestoreAction:
    device: str
    reports: Tuple[protocol.Report, ...] = ()
    logo_json: Optional[str] = None


def validate_hid(device: str, saved: dict) -> RestoreAction:
    if device not in effects.MODES_BY_LABEL or not isinstance(saved, dict):
        raise ValueError("unknown device or invalid saved parameters")
    allowed = {"mode", "color", "colors", "brightness", "speed", "direction", "reactive"}
    if set(saved) - allowed:
        raise ValueError("unknown saved lighting fields")
    mode = saved.get("mode")
    if not isinstance(mode, str) or mode not in effects.MODES_BY_LABEL[device]:
        raise ValueError("unknown saved lighting mode")
    maximum = protocol.KBD_BRIGHTNESS_MAX if device == "keyboard" else protocol.LB_BRIGHTNESS_MAX
    brightness = saved.get("brightness")
    brightness = maximum if brightness is None else _int(brightness, 0, maximum, "brightness")
    color = _color(saved.get("color", [0, 255, 255]))
    palette = saved.get("colors")
    if palette is not None:
        if not isinstance(palette, list) or not 1 <= len(palette) <= 7:
            raise ValueError("colors: expected one to seven RGB colors")
        palette = [_color(value) for value in palette]
    speed = _int(saved.get("speed", 5), 1, 10, "speed")
    direction = _int(saved.get("direction", 1), 1, 4 if device == "keyboard" else 2, "direction")
    reactive = saved.get("reactive", False)
    if type(reactive) is not bool:
        raise ValueError("reactive: expected a boolean")
    # A zero brightness is also OFF. In particular, never send lightbar
    # power-on before an OFF restore (the old cmd_restore did that).
    if mode == "off" or brightness == 0:
        reports = effects.build(device, "off")
    else:
        reports = effects.build(device, mode, color=color, colors=palette,
                                brightness=brightness, speed=speed,
                                direction=direction, reactive=reactive)
        if device == "lightbar":
            reports = protocol.lightbar_power_on() + reports
    return RestoreAction(device, tuple(reports))


def validate_logo(saved: dict) -> RestoreAction:
    if not isinstance(saved, dict) or set(saved) != {"mode", "brightness", "color"}:
        raise ValueError("invalid last-applied Logo record")
    mode = saved["mode"]
    if mode not in ("solid", "breathing", "rainbow"):
        raise ValueError("unknown Logo mode")
    brightness = _int(saved["brightness"], 0, 100, "Logo brightness")
    if brightness not in (0, 25, 50, 75, 100):
        raise ValueError("unsupported Logo brightness")
    color = _color(saved["color"])
    if mode == "breathing" and color not in LOGO_COLORS:
        raise ValueError("unsupported Logo breathing color")
    data = {"mode": mode, "brightness": brightness, "color": list(color)}
    return RestoreAction("logo", logo_json=json.dumps(data, separators=(",", ":")))


def _read_saved(path: Path, uid: int):
    try:
        fd = os.open(path, os.O_RDONLY | os.O_CLOEXEC | os.O_NOFOLLOW | os.O_NONBLOCK)
    except FileNotFoundError:
        return None
    with os.fdopen(fd, "rb") as stream:
        info = os.fstat(stream.fileno())
        if not stat.S_ISREG(info.st_mode) or info.st_uid != uid or info.st_mode & 0o002:
            raise ValueError("saved state must be a user-owned regular file, not world-writable")
        if info.st_size > MAX_STATE_BYTES:
            raise ValueError("saved state exceeds size limit")
        data = stream.read(MAX_STATE_BYTES + 1)
        if len(data) > MAX_STATE_BYTES:
            raise ValueError("saved state exceeds size limit")
        return json.loads(data)


def load_actions(directory: Path, uid: int):
    """Only applied state.json/logo.json; never logo-prefs or system pin data."""
    actions, errors = [], []
    try:
        saved = _read_saved(directory / "state.json", uid)
        if saved is not None:
            if not isinstance(saved, dict) or set(saved) - {"keyboard", "lightbar"}:
                raise ValueError("state.json has unknown devices")
            for device in ("keyboard", "lightbar"):
                if device in saved:
                    try:
                        actions.append(validate_hid(device, saved[device]))
                    except (ValueError, TypeError) as error:
                        errors.append(f"{device}: {error}")
    except (OSError, ValueError, TypeError) as error:
        errors.append(f"state.json: {error}")
    try:
        saved = _read_saved(directory / "logo.json", uid)
        if saved is not None:
            actions.append(validate_logo(saved))
    except (OSError, ValueError, TypeError) as error:
        errors.append(f"logo.json: {error}")
    return tuple(actions), tuple(errors)


def eligible_session(properties: dict, uid: int) -> bool:
    user = properties.get("User", ())
    seat = properties.get("Seat", ())
    return (isinstance(user, (tuple, list)) and len(user) == 2 and user[0] == uid
            and isinstance(seat, (tuple, list)) and len(seat) == 2 and bool(seat[0])
            and properties.get("Active") is True and properties.get("Remote") is False
            and properties.get("State") == "active" and properties.get("Class") == "user"
            and properties.get("Type") in ("x11", "wayland"))


class Lifecycle:
    """Testable event gate; AC/DC and idle do not trigger restoration."""

    def __init__(self, active: Callable[[], bool], schedule: Callable[[], None],
                 cancel: Callable[[], None]):
        self.active, self.schedule, self.cancel = active, schedule, cancel
        self.started = self.sleeping = self.stopped = False

    def startup(self):
        if not self.started and not self.stopped:
            self.started = True
            if self.active():
                self.schedule()

    def prepare_for_sleep(self, sleeping: bool):
        if type(sleeping) is not bool:
            raise ValueError("sleep signal must be boolean")
        if self.stopped:
            return
        if sleeping:
            self.sleeping = True
            self.cancel()
        elif self.sleeping:
            self.sleeping = False
            if self.active():
                self.schedule()

    def stop(self):
        self.stopped = True
        self.cancel()

    def can_restore(self):
        return not self.stopped and not self.sleeping and self.active()


def apply_action(action: RestoreAction):
    """Explicit execution backend, called only with --apply and an active gate."""
    from .zone_lock import ZoneTransaction
    with ZoneTransaction(action.device):
        return _apply_action_locked(action)


def _apply_action_locked(action: RestoreAction):
    if action.device == "logo":
        # Revalidate even if called independently of load_actions.
        action = validate_logo(json.loads(action.logo_json))
        result = subprocess.run(
            ["/usr/bin/pkexec", "--disable-internal-agent", HELPER, "logo", action.logo_json],
            capture_output=True, text=True, timeout=5, check=False)
        if result.returncode:
            raise RuntimeError(result.stderr.strip() or "Logo helper failed")
        response = json.loads(result.stdout)
        if not isinstance(response, dict) or "error" in response:
            raise RuntimeError("Logo helper rejected restoration")
        expected = json.loads(action.logo_json)
        if any(response.get(key) != value for key, value in expected.items()):
            raise RuntimeError("Logo helper readback differs from saved state")
    elif action.device in effects.MODES_BY_LABEL:
        from . import devices
        with devices.HidLed(devices.led_for_label(action.device)) as device:
            device.send(action.reports)
    else:
        raise ValueError("unknown restore target")


def restore_once(directory, uid, active, *, apply=False, writer=None, transaction=None):
    """Recheck the owning session before each device; dry-run never calls writer."""
    if not active():
        return {"restored": [], "planned": [], "errors": [], "skipped": "inactive session"}
    actions, errors = load_actions(directory, uid)
    result = {"restored": [], "planned": [], "errors": list(errors)}
    for action in actions:
        if not active():
            result["skipped"] = "session ended or became inactive"
            break
        result["planned"].append(action.device)
        if apply:
            try:
                from .zone_lock import ZoneTransaction
                with (transaction or ZoneTransaction)(action.device):
                    latest, errors = load_actions(directory, uid)
                    result['errors'].extend(errors)
                    latest = next((value for value in latest if value.device == action.device), None)
                    if latest is None:
                        continue
                    if not active():
                        result['skipped'] = 'session ended or became inactive'
                        break
                    if (writer or apply_action)(latest) is False:
                        raise RuntimeError('lighting writer reported failure')
                    result["restored"].append(action.device)
            except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
                result["errors"].append(f"{action.device}: {error}")
    return result


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--apply", action="store_true", help="execute validated lighting restoration")
    parser.add_argument("--once", action="store_true", help="evaluate startup once without watching D-Bus")
    args = parser.parse_args(argv)
    # One watcher per user even when started manually alongside the user unit.
    # No lock is needed for --once, which defaults to a read-only preview.
    lock_fd = None
    if not args.once:
        import fcntl
        runtime = os.environ.get("XDG_RUNTIME_DIR")
        if not runtime:
            parser.error("XDG_RUNTIME_DIR is required for a session watcher")
        lock_fd = os.open(Path(runtime) / "iteglow-session-restore.lock",
                          os.O_CREAT | os.O_RDWR | os.O_CLOEXEC | os.O_NOFOLLOW, 0o600)
        info = os.fstat(lock_fd)
        if not stat.S_ISREG(info.st_mode) or info.st_uid != os.getuid() or info.st_mode & 0o022:
            os.close(lock_fd)
            raise RuntimeError("unsafe session watcher lock")
        try:
            fcntl.flock(lock_fd, fcntl.LOCK_EX | fcntl.LOCK_NB)
        except BlockingIOError:
            os.close(lock_fd)
            print(json.dumps({"skipped": "lighting session watcher already running"}))
            return 0
    from gi.repository import Gio, GLib
    uid = os.getuid()
    system_bus = Gio.bus_get_sync(Gio.BusType.SYSTEM, None)
    user_bus = Gio.bus_get_sync(Gio.BusType.SESSION, None)

    def call(bus, destination, path, interface, method, parameters=None):
        return bus.call_sync(destination, path, interface, method, parameters, None,
                             Gio.DBusCallFlags.NONE, 2500, None).unpack()

    def properties(path):
        return call(system_bus, "org.freedesktop.login1", path,
                    "org.freedesktop.DBus.Properties", "GetAll",
                    GLib.Variant("(s)", ("org.freedesktop.login1.Session",)))[0]

    sessions = call(system_bus, "org.freedesktop.login1", "/org/freedesktop/login1",
                    "org.freedesktop.login1.Manager", "ListSessions")[0]
    candidates = [(sid, path) for sid, owner, _name, _seat, path in sessions
                  if owner == uid and eligible_session(properties(path), uid)]
    if len(candidates) != 1:
        print(json.dumps({"skipped": "exactly one active local graphical session is required"}))
        if lock_fd is not None:
            os.close(lock_fd)
        return 0
    session_id, session_path = candidates[0]

    def active():
        try:
            # Bind once to this session: never adopt a different login later.
            if not eligible_session(properties(session_path), uid):
                return False
            unit = call(user_bus, "org.freedesktop.systemd1", "/org/freedesktop/systemd1",
                        "org.freedesktop.systemd1.Manager", "GetUnit",
                        GLib.Variant("(s)", ("graphical-session.target",)))[0]
            state = call(user_bus, "org.freedesktop.systemd1", unit,
                         "org.freedesktop.DBus.Properties", "Get",
                         GLib.Variant("(ss)", ("org.freedesktop.systemd1.Unit", "ActiveState")))[0]
            return state == "active"
        except GLib.Error:
            return False

    directory = Path(os.environ.get("XDG_STATE_HOME", str(Path.home() / ".local/state"))) / "iteglow"
    if args.once:
        print(json.dumps(restore_once(directory, uid, active, apply=args.apply)), flush=True)
        return 0

    from .lighting_policy_session import CONFIG_PATH, PolicyRunner, SessionInputs, read_config, read_manual
    inputs = SessionInputs(
        lambda *parameters: call(system_bus, *parameters),
        lambda *parameters: call(user_bus, *parameters), GLib.Variant)
    loop = GLib.MainLoop()
    pending = None
    last_active = False
    lifecycle = None
    policy = PolicyRunner(
        active=lambda: lifecycle is not None and lifecycle.can_restore(),
        config=lambda: read_config(CONFIG_PATH, uid),
        manual=lambda: read_manual(directory, uid), power=inputs.power, idle=inputs.idle,
        apply=args.apply, writer=apply_action,
        emit=lambda event: print(json.dumps(event, ensure_ascii=False), flush=True))

    def cancel():
        nonlocal pending, last_active
        policy.begin_restore()
        last_active = False
        if pending is not None:
            GLib.source_remove(pending)
            pending = None

    def restore():
        nonlocal pending
        pending = None
        policy.begin_restore()
        try:
            print(json.dumps(restore_once(directory, uid, lifecycle.can_restore, apply=args.apply)), flush=True)
        finally:
            policy.finish_restore()
        policy.tick()
        return False

    def schedule():
        nonlocal pending, last_active
        cancel()
        last_active = True
        # Let USB and the graphical session settle; read saved state only then.
        pending = GLib.timeout_add_seconds(2, restore)

    lifecycle = Lifecycle(active, schedule, cancel)

    def policy_tick():
        nonlocal last_active
        current = lifecycle.can_restore()
        if not current:
            policy.deactivate()
            last_active = False
        elif not last_active:
            # Inactivity drops ownership. Restore the saved manual baseline
            # before rejoining policy, so old idle-OFF cannot become stranded.
            schedule()
        policy.tick()
        return True

    def manager_signal(_connection, _sender, _path, _interface, member, params, _data):
        values = params.unpack()
        if member == "PrepareForSleep":
            lifecycle.prepare_for_sleep(values[0])
        elif member == "PrepareForShutdown" and values[0]:
            lifecycle.stop()
        elif member == "SessionRemoved" and values[0] == session_id:
            lifecycle.stop()
            loop.quit()

    def stop():
        lifecycle.stop()
        policy.deactivate()
        loop.quit()
        return False

    subscription = system_bus.signal_subscribe(
        "org.freedesktop.login1", "org.freedesktop.login1.Manager", None,
        "/org/freedesktop/login1", None, Gio.DBusSignalFlags.NONE, manager_signal, None)
    GLib.unix_signal_add(GLib.PRIORITY_DEFAULT, signal.SIGTERM, stop)
    GLib.unix_signal_add(GLib.PRIORITY_DEFAULT, signal.SIGINT, stop)
    lifecycle.startup()
    policy_timer = GLib.timeout_add_seconds(5, policy_tick)
    try:
        loop.run()
    finally:
        lifecycle.stop()
        policy.deactivate()
        GLib.source_remove(policy_timer)
        system_bus.signal_unsubscribe(subscription)
        if lock_fd is not None:
            os.close(lock_fd)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

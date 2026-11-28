# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
"""Desktop user-session daemon, coordinator state machine, and UNIX IPC for water coolers.

Runs unprivileged in the active desktop user session (no sudo / no arbitrary EC writes).
Listens to logind PrepareForSleep / PrepareForShutdown to safely sleep and disconnect BLE.
"""
from __future__ import annotations

from collections import deque
from contextlib import contextmanager
import fcntl
import json
import os
from pathlib import Path
import queue
import signal
import socket
import stat
import struct
import threading
import time
from typing import Callable, Optional

from . import watercooling as wc
from .watercooling_ble import CoolerBleClient

RECONNECT_BACKOFF_S = (3.0, 5.0, 8.0, 15.0)
LINK_DROP_RECONNECT_DELAY_S = 2.0
WAKE_RECONNECT_DELAY_S = 2.5
MAX_CONSECUTIVE_METER_ERRORS = 2


def runtime_dir() -> Path:
    return Path(os.environ.get("XDG_RUNTIME_DIR") or f"/run/user/{os.getuid()}")


def socket_path(base: Optional[Path] = None) -> Path:
    return (base or runtime_dir()) / "iteglow-watercooling.sock"


def lock_path(base: Optional[Path] = None) -> Path:
    return (base or runtime_dir()) / "iteglow-watercooling.lock"


class CoolerCoordinator:
    """Testable coordinator bridging CoolerBleClient, thermal curves, EC telemetry, and flow safety."""

    def __init__(
        self,
        *,
        client: Optional[CoolerBleClient] = None,
        config: Optional[dict] = None,
        config_saver: Optional[Callable[[dict], dict]] = None,
        temp_reader: Callable[[], Optional[float]] = wc.read_host_temp_c,
        ec_reader: Callable[[], dict] = wc.read_ec_watercooler_state,
        clock: Callable[[], float] = time.monotonic,
        flow_monitor: Optional[wc.FlowSafetyMonitor] = None,
        event_logger: Optional[Callable[[dict], None]] = None,
    ):
        self.client = client or CoolerBleClient(clock=clock)
        self.config = wc.validate_config(config) if config is not None else wc.load_config()
        self._config_saver = config_saver or wc.save_config
        self._temp_reader = temp_reader
        self._ec_reader = ec_reader
        self._clock = clock
        self.flow_monitor = flow_monitor or wc.FlowSafetyMonitor()
        self._event_logger = event_logger
        self.client.on_disconnect_event = self._on_ble_disconnected

        self.sleeping = False
        self.current_tier = 0
        self._pump_may_be_on = False
        self.applied_pump_volt = 0
        self.applied_pump_duty = 0
        self.applied_fan_duty = 0
        self.applied_led: Optional[str] = None
        self.actuators_dirty = False
        self.led_dirty = False
        self.last_reason = "idle"
        self.last_error = ""
        self.last_temp_c: Optional[float] = None
        self.last_ec_state: dict = {}
        self._last_meter_poll = 0.0
        self._last_c1_seq: Optional[int] = None
        self._last_connect_attempt = 0.0
        self._manual_disconnect = False
        self._next_connect_at = 0.0
        self._reconnect_failures = 0
        self._consecutive_meter_errors = 0

    def _emit(self, event: str, **fields) -> None:
        if self._event_logger is None:
            return
        payload = {"event": event, **fields}
        try:
            self._event_logger(payload)
        except Exception:
            pass

    def _on_ble_disconnected(self, reason: str) -> None:
        self.flow_monitor.on_disconnect()
        self._pump_may_be_on = False
        self.applied_pump_volt = 0
        self.applied_pump_duty = 0
        self.applied_fan_duty = 0
        self.applied_led = None
        self._consecutive_meter_errors = 0
        self.last_reason = f"disconnected:{reason}"
        if reason not in ("manual_disconnect", "prepare_for_sleep"):
            self._reconnect_failures = 0
            self._next_connect_at = self._clock() + LINK_DROP_RECONNECT_DELAY_S
        self._emit(
            "cooler_disconnected",
            reason=reason,
            device_mac=self.client.device_mac,
            device_name=self.client.device_name,
        )

    def _safe_sample_inputs(self) -> None:
        try:
            self.last_temp_c = self._temp_reader()
        except Exception:
            self.last_temp_c = None
        try:
            ec = self._ec_reader()
            if isinstance(ec, dict):
                c1 = ec.get("wmi_c1_seq")
                if c1 is not None and c1 != self._last_c1_seq:
                    self._last_c1_seq = c1
                    if self.config["strategy"] == "ec_follow" and self.client.connected:
                        self.actuators_dirty = True
                self.last_ec_state = ec
        except Exception:
            pass

    def status_snapshot(self) -> dict:
        return {
            "service_running": True,
            "connected": self.client.connected,
            "sleeping": self.sleeping,
            "device_name": self.client.device_name,
            "device_mac": self.client.device_mac,
            "fw_version": self.client.fw_version,
            "meter_state": self.client.meter_state,
            "flow_tripped": self.flow_monitor.tripped,
            "flow_trip_reason": self.flow_monitor.trip_reason,
            "applied": {
                "pump_volt": self.applied_pump_volt,
                "pump_duty": self.applied_pump_duty,
                "fan_duty": self.applied_fan_duty,
            },
            "current_tier": self.current_tier,
            "reason": self.last_reason,
            "last_error": self.last_error,
            "temp_c": self.last_temp_c,
            "ec": self.last_ec_state,
            "config": self.config,
        }

    def status(self) -> dict:
        self._safe_sample_inputs()
        return self.status_snapshot()

    def scan(self, timeout_s: float = 5.0) -> list:
        return self.client.scan(timeout_s=timeout_s, target_mac=self.config.get("mac", ""))

    def connect(self, mac: str = "") -> dict:
        if self.sleeping:
            raise RuntimeError("系统正在进入睡眠/休眠，禁止连接水冷")
        if not isinstance(mac, str) or (mac and not wc.MAC_RE.fullmatch(mac)):
            raise ValueError("无效的水冷 MAC 地址")
        self._manual_disconnect = False
        target = mac.upper() if mac else self.config.get("mac", "")
        if self.client.connected:
            if target and target.upper() != self.client.device_mac.upper():
                raise ValueError("请先断开当前水冷设备再连接其他设备")
            return {**self.status(), "connect_info": self.client.connect(target_mac=target)}
        self.last_error = ""
        info = self.client.connect(target_mac=target)
        self.flow_monitor.on_disconnect()
        self._reconnect_failures = 0
        self._next_connect_at = 0.0
        self._consecutive_meter_errors = 0
        self.actuators_dirty = True
        self.led_dirty = True
        self._last_meter_poll = self._clock()
        self.apply_targets(force=True)
        self._emit(
            "cooler_connected",
            device_name=self.client.device_name,
            device_mac=self.client.device_mac,
            fw_version=self.client.fw_version,
            meter_state=self.client.meter_state,
        )
        return {**self.status(), "connect_info": info}

    def _stop_link(self, sleep: bool, reason: str) -> Optional[str]:
        try:
            self.client.disconnect(send_sleep=sleep, stop_actuators=True)
        except Exception as exc:
            self.last_error = str(exc)
            self._emit("cooler_stop_failed", reason=reason, error=self.last_error)
            return self.last_error
        finally:
            self._on_ble_disconnected(reason)
        return None

    def disconnect(self, *, send_sleep: Optional[bool] = None) -> dict:
        self._manual_disconnect = True
        use_sleep = self.config["sleep_on_disconnect"] if send_sleep is None else bool(send_sleep)
        error = self._stop_link(use_sleep, "manual_disconnect")
        self.last_reason = "disconnected"
        if error:
            raise RuntimeError(error)
        return self.status()

    def emergency_stop(self) -> dict:
        self._manual_disconnect = True
        error = self._stop_link(True, "manual_disconnect")
        self.config["strategy"] = "preset"
        self.config["preset"] = "stop"
        self.config["led"]["mode"] = "off"
        self.last_reason = "emergency_stop"
        try:
            self._config_saver(self.config)
        except Exception as exc:
            if error:
                raise RuntimeError(f"{error}；{exc}") from exc
            raise
        if error:
            raise RuntimeError(error)
        return self.status()

    def reset_fault(self) -> dict:
        self.flow_monitor.reset()
        self.actuators_dirty = True
        self.last_error = ""
        self.last_reason = "fault_reset"
        self._emit("flow_fault_reset")
        if self.client.connected and not self.sleeping:
            self.apply_targets(force=True)
        return self.status()

    def set_config(self, patch: dict, *, apply_now: bool = True) -> dict:
        if not isinstance(patch, dict):
            raise ValueError("配置更新必须是 JSON 对象")
        merged = dict(self.config)
        for k, v in patch.items():
            if k in ("manual", "curve", "led") and isinstance(v, dict) and isinstance(merged.get(k), dict):
                sub = dict(merged[k])
                sub.update(v)
                merged[k] = sub
            else:
                merged[k] = v
        canonical = wc.validate_config(merged)
        saved = self._config_saver(canonical)
        if saved["auto_connect"] and not self.config["auto_connect"]:
            self._manual_disconnect = False
        self.config = saved
        self.actuators_dirty = True
        self.led_dirty = True
        if apply_now and self.client.connected and not self.sleeping:
            self.apply_targets(force=True)
        return self.status()

    def prepare_for_sleep(self, entering: bool) -> dict:
        if entering:
            self.sleeping = True
            if self.client.connected:
                self._stop_link(True, "prepare_for_sleep")
            self.last_reason = "sleeping"
        else:
            self.sleeping = False
            self._reconnect_failures = 0
            self._next_connect_at = self._clock() + WAKE_RECONNECT_DELAY_S
            self.last_reason = "resumed"
        return self.status()

    def _write_actuators(self, pv: int, pd: int, fd: int) -> None:
        pump, fan = wc.encode_pump(pv, pd), wc.encode_fan(fd)
        self.actuators_dirty = True
        if pv > 0 and not self._pump_may_be_on:
            self.client.meter_state = None
            self._pump_may_be_on = True
            self.flow_monitor.update(now=self._clock(), pump_volt=pv, meter_state=None,
                                     flow_protection=self.config["flow_protection"])
        self.client.send_paced(pump)
        self.applied_pump_volt, self.applied_pump_duty = pv, pd
        self._pump_may_be_on = pv > 0
        self.client.send_paced(fan)
        self.applied_fan_duty = fd
        self.actuators_dirty = False

    def apply_targets(self, *, force: bool = False) -> None:
        if not self.client.connected:
            return
        now = self._clock()
        self._safe_sample_inputs()

        if self.flow_monitor.update(
            now=now,
            pump_volt=int(self._pump_may_be_on),
            meter_state=self.client.meter_state,
            flow_protection=self.config["flow_protection"],
        ):
            was_tripped = self.last_reason == "flow_fault_locked"
            self.last_error = self.flow_monitor.trip_reason
            if force or self.actuators_dirty or self._pump_may_be_on or self.applied_fan_duty:
                try:
                    self._write_actuators(0, 0, 0)
                except Exception as exc:
                    self.last_error = f"{self.flow_monitor.trip_reason}; 紧急停泵失败(将重试): {exc}"
            self.last_reason = "flow_fault_locked"
            if not was_tripped:
                self._emit(
                    "flow_fault_tripped",
                    reason=self.flow_monitor.trip_reason,
                    meter_state=self.client.meter_state,
                )
            return

        pv, pd, fd, new_tier, reason = wc.evaluate_cooling_target(
            self.config,
            temp_c=self.last_temp_c,
            ec_state=self.last_ec_state,
            current_tier=self.current_tier,
            flow_tripped=self.flow_monitor.tripped,
            sleeping=self.sleeping,
        )
        self.current_tier = new_tier
        self.last_reason = reason

        if force or self.actuators_dirty or (pv, pd, fd) != (self.applied_pump_volt, self.applied_pump_duty, self.applied_fan_duty):
            self._write_actuators(pv, pd, fd)
            self.flow_monitor.update(
                now=now,
                pump_volt=pv,
                meter_state=self.client.meter_state,
                flow_protection=self.config["flow_protection"],
            )

        led_sig = json.dumps(self.config["led"], sort_keys=True)
        if force or self.led_dirty or led_sig != self.applied_led:
            self.client.apply_led(self.config["led"])
            self.applied_led = led_sig
            self.led_dirty = False

    def tick(self) -> None:
        now = self._clock()
        if self.sleeping:
            return
        if not self.client.connected:
            if self.config["auto_connect"] and not self._manual_disconnect and not self.flow_monitor.tripped and now >= self._next_connect_at:
                self._last_connect_attempt = now
                try:
                    self.connect()
                except Exception as exc:
                    delay = RECONNECT_BACKOFF_S[min(self._reconnect_failures, len(RECONNECT_BACKOFF_S) - 1)]
                    self._reconnect_failures += 1
                    self._next_connect_at = self._clock() + delay
                    self.last_error = str(exc)
                    self._emit(
                        "cooler_connect_failed",
                        attempt=self._reconnect_failures,
                        next_retry_s=delay,
                        error=str(exc),
                    )
            return

        poll_interval = 2.0 if self._pump_may_be_on else 6.0
        if now - self._last_meter_poll >= poll_interval:
            self._last_meter_poll = now
            try:
                self.client.query_meter()
                self._consecutive_meter_errors = 0
            except Exception as exc:
                self.last_error = f"流量轮询异常: {exc}"
                self._consecutive_meter_errors += 1
                self._emit(
                    "meter_poll_error",
                    consecutive=self._consecutive_meter_errors,
                    connected=self.client.connected,
                    error=str(exc),
                )
                if not self.client.connected:
                    return
                if self._consecutive_meter_errors >= MAX_CONSECUTIVE_METER_ERRORS:
                    if self.config["flow_protection"] and self._pump_may_be_on:
                        self.flow_monitor.tripped = True
                        self.flow_monitor.trip_reason = self.last_error
                    self._stop_link(True, "meter_poll_failed")
                    return
        try:
            self.apply_targets(force=False)
        except Exception as exc:
            self.last_error = str(exc)
            self._emit("apply_targets_error", connected=self.client.connected, error=str(exc))


def dispatch_command(coordinator: CoolerCoordinator, req: dict) -> dict:
    if not isinstance(req, dict) or "op" not in req:
        raise ValueError("请求缺少 op 字段")
    def boolean(key, default):
        value = req.get(key, default)
        if type(value) is not bool:
            raise ValueError(f"{key} 需要布尔值")
        return value

    op = req["op"]
    if op == "ping":
        return {"pong": True, "connected": coordinator.client.connected}
    if op == "status":
        return coordinator.status()
    if op == "scan":
        return {"devices": coordinator.scan(req.get("timeout_s", 5.0))}
    if op == "connect":
        return coordinator.connect(req.get("mac", ""))
    if op == "disconnect":
        return coordinator.disconnect(send_sleep=None if req.get("send_sleep") is None else boolean("send_sleep", True))
    if op == "emergency_stop":
        return coordinator.emergency_stop()
    if op == "reset_fault":
        return coordinator.reset_fault()
    if op == "set_config":
        return coordinator.set_config(req.get("config"), apply_now=boolean("apply_now", True))
    if op == "prepare_sleep":
        return coordinator.prepare_for_sleep(boolean("entering", True))
    raise ValueError(f"不支持的水冷命令: {op}")


def _verify_runtime_dir(path: Path) -> None:
    if not path.exists():
        path.mkdir(parents=True, mode=0o700, exist_ok=True)
    info = path.lstat()
    if not stat.S_ISDIR(info.st_mode) or info.st_uid != os.getuid() or info.st_mode & 0o022:
        raise RuntimeError(f"不安全的运行时目录: {path}")


def service_request(req: dict, *, sock_path: Optional[Path] = None, timeout_s: float = 16.0) -> dict:
    target = sock_path or socket_path()
    info = target.lstat()
    if not stat.S_ISSOCK(info.st_mode) or info.st_uid != os.getuid() or info.st_mode & 0o077:
        raise RuntimeError(f"水冷服务 socket 权限异常: {target}")
    with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as cli:
        cli.settimeout(timeout_s)
        cli.connect(str(target))
        cli.sendall((json.dumps(req, ensure_ascii=False) + "\n").encode("utf-8"))
        buf = bytearray()
        while b"\n" not in buf:
            chunk = cli.recv(8192)
            if not chunk or len(buf) + len(chunk) > 131072:
                raise RuntimeError("水冷服务响应不完整或过长")
            buf.extend(chunk)
    resp = json.loads(buf.decode("utf-8"))
    if not isinstance(resp, dict) or resp.get("ok") is not True:
        raise RuntimeError(resp.get("error", "水冷服务执行失败") if isinstance(resp, dict) else "水冷服务响应无效")
    return resp["result"]


def is_service_running(sock_path: Optional[Path] = None) -> bool:
    try:
        res = service_request({"op": "ping"}, sock_path=sock_path, timeout_s=1.0)
        return res.get("pong") is True
    except Exception:
        return False


@contextmanager
def cooler_lock(rdir: Path):
    _verify_runtime_dir(rdir)
    fd = os.open(lock_path(rdir), os.O_CREAT | os.O_RDWR | os.O_CLOEXEC | os.O_NOFOLLOW | os.O_NONBLOCK, 0o600)
    try:
        info = os.fstat(fd)
        if not stat.S_ISREG(info.st_mode) or info.st_uid != os.getuid() or info.st_mode & 0o077:
            raise RuntimeError("水冷锁文件权限异常")
        try:
            fcntl.flock(fd, fcntl.LOCK_EX | fcntl.LOCK_NB)
        except BlockingIOError as exc:
            raise RuntimeError("水冷服务或配置正忙") from exc
        yield
    finally:
        os.close(fd)


def _remove_stale_socket(path: Path) -> None:
    try:
        info = path.lstat()
    except FileNotFoundError:
        return
    if not stat.S_ISSOCK(info.st_mode) or info.st_uid != os.getuid():
        raise RuntimeError(f"水冷 socket 路径已被占用: {path}")
    with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as probe:
        probe.settimeout(0.5)
        try:
            probe.connect(str(path))
        except ConnectionRefusedError:
            path.unlink()
            return
    raise RuntimeError("水冷控制 socket 仍被占用")


def _read_request(conn: socket.socket) -> dict:
    deadline = time.monotonic() + 2.0
    buf = bytearray()
    while len(buf) < 16384:
        remaining = deadline - time.monotonic()
        if remaining <= 0:
            raise TimeoutError("读取水冷请求超时")
        conn.settimeout(remaining)
        chunk = conn.recv(min(4096, 16384 - len(buf)))
        if not chunk:
            break
        line, end, _ = chunk.partition(b"\n")
        buf.extend(line)
        if end:
            return json.loads(buf)
    raise ValueError("水冷服务请求格式无效")


class _RequestQueue:
    """Serialize BLE operations across callbacks reentered by GLib during GATT waits."""

    def __init__(self, coordinator):
        self.coordinator = coordinator
        self.pending = deque()
        self.busy = False
        self.stopping = False

    def submit(self, req, reply, deadline=None):
        if self.stopping or (deadline is not None and time.monotonic() >= deadline):
            return False
        op = req.get("op") if isinstance(req, dict) else None
        if self.busy and op in ("ping", "status"):
            result = ({"pong": True, "connected": self.coordinator.client.connected}
                      if op == "ping" else self.coordinator.status_snapshot())
            reply.put({"ok": True, "result": result})
        else:
            self.pending.append((req, reply, deadline))
            self.drain()
        return False  # GLib idle callbacks run once.

    def drain(self):
        if self.busy:
            return
        self.busy = True
        try:
            while self.pending and not self.stopping:
                req, reply, deadline = self.pending.popleft()
                if deadline is not None and time.monotonic() >= deadline:
                    continue
                try:
                    result = dispatch_command(self.coordinator, req)
                except Exception as exc:
                    reply.put({"ok": False, "error": str(exc)})
                else:
                    reply.put({"ok": True, "result": result})
        finally:
            self.busy = False


def run_daemon() -> int:
    if os.geteuid() == 0:
        raise RuntimeError("水冷用户服务必须以当前桌面用户运行，请勿使用 root/sudo")
    import gi
    gi.require_version("Gio", "2.0")
    gi.require_version("GLib", "2.0")
    from gi.repository import Gio, GLib

    rdir = runtime_dir()
    spath = socket_path(rdir)
    with cooler_lock(rdir):
        _remove_stale_socket(spath)

        srv = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
        srv.bind(str(spath))
        os.chmod(spath, 0o600)
        srv.listen(8)
        srv.settimeout(0.5)

        def log_event(payload: dict) -> None:
            print(json.dumps(payload, ensure_ascii=False), flush=True)

        coordinator = CoolerCoordinator(event_logger=log_event)
        loop = GLib.MainLoop()
        system_bus = Gio.bus_get_sync(Gio.BusType.SYSTEM, None)
        requests = _RequestQueue(coordinator)

        def on_manager_signal(_conn, _sender, _path, _iface, member, params, *_user_data):
            values = params.unpack()
            if member == "PrepareForSleep":
                entering = bool(values[0])
                log_event({"event": "prepare_for_sleep", "entering": entering})
                requests.submit({"op": "prepare_sleep", "entering": entering}, queue.Queue(1))
            elif member == "PrepareForShutdown" and values[0]:
                log_event({"event": "prepare_for_shutdown"})
                stop_daemon()

        def on_tick():
            if requests.busy or requests.stopping:
                return True
            requests.busy = True
            try:
                coordinator.tick()
            finally:
                requests.busy = False
            requests.drain()
            return True

        def stop_daemon(*_):
            requests.stopping = True
            loop.quit()
            return False

        sub_id = system_bus.signal_subscribe(
            "org.freedesktop.login1",
            "org.freedesktop.login1.Manager",
            None,
            "/org/freedesktop/login1",
            None,
            Gio.DBusSignalFlags.NONE,
            on_manager_signal,
            None,
        )
        GLib.unix_signal_add(GLib.PRIORITY_DEFAULT, signal.SIGTERM, stop_daemon)
        GLib.unix_signal_add(GLib.PRIORITY_DEFAULT, signal.SIGINT, stop_daemon)
        tick_id = GLib.timeout_add_seconds(1, on_tick)

        def sock_worker():
            while not requests.stopping:
                try:
                    conn, _ = srv.accept()
                except socket.timeout:
                    continue
                except OSError:
                    break
                with conn:
                    conn.settimeout(2.0)
                    try:
                        _, uid, _ = struct.unpack("3i", conn.getsockopt(socket.SOL_SOCKET, socket.SO_PEERCRED, 12))
                        if uid != os.getuid():
                            raise PermissionError("仅允许同用户进程访问水冷服务")
                        req = _read_request(conn)
                        res_q: queue.Queue = queue.Queue(1)
                        GLib.idle_add(requests.submit, req, res_q, time.monotonic() + 20.0)
                        resp = res_q.get(timeout=20.0)
                    except queue.Empty:
                        resp = {"ok": False, "error": "水冷服务执行超时"}
                    except Exception as exc:
                        resp = {"ok": False, "error": str(exc)}
                    try:
                        conn.settimeout(2.0)
                        conn.sendall((json.dumps(resp, ensure_ascii=False) + "\n").encode("utf-8"))
                    except OSError:
                        pass

        worker = threading.Thread(target=sock_worker, daemon=True)
        worker.start()
        print(json.dumps({"event": "watercooling_daemon_started", "socket": str(spath)}, ensure_ascii=False), flush=True)
        try:
            loop.run()
        finally:
            requests.stopping = True
            GLib.source_remove(tick_id)
            system_bus.signal_unsubscribe(sub_id)
            try:
                coordinator.prepare_for_sleep(True)
            finally:
                srv.close()
            if spath.exists():
                try:
                    spath.unlink()
                except OSError:
                    pass
    return 0

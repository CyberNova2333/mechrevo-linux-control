"""Desktop user-session daemon, coordinator state machine, and UNIX IPC for water coolers.

Runs unprivileged in the active desktop user session (no sudo / no arbitrary EC writes).
Listens to logind PrepareForSleep / PrepareForShutdown to safely sleep and disconnect BLE.
"""
from __future__ import annotations

import errno
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
    ):
        self.client = client or CoolerBleClient(clock=clock)
        self.config = wc.validate_config(config) if config is not None else wc.load_config()
        self._config_saver = config_saver or wc.save_config
        self._temp_reader = temp_reader
        self._ec_reader = ec_reader
        self._clock = clock
        self.flow_monitor = flow_monitor or wc.FlowSafetyMonitor()
        self.client.on_disconnect_event = self._on_ble_disconnected

        self.sleeping = False
        self.current_tier = 0
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

    def _on_ble_disconnected(self, reason: str) -> None:
        self.flow_monitor.on_disconnect()
        self.applied_pump_volt = 0
        self.applied_pump_duty = 0
        self.applied_fan_duty = 0
        self.applied_led = None
        self.last_reason = f"disconnected:{reason}"

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

    def status(self) -> dict:
        self._safe_sample_inputs()
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

    def scan(self, timeout_s: float = 5.0) -> list:
        return self.client.scan(timeout_s=timeout_s, target_mac=self.config.get("mac", ""))

    def connect(self, mac: str = "") -> dict:
        if self.sleeping:
            raise RuntimeError("系统正在进入睡眠/休眠，禁止连接水冷")
        target = mac.upper() if mac else self.config.get("mac", "")
        self.last_error = ""
        info = self.client.connect(target_mac=target)
        self.flow_monitor.on_disconnect()
        self.actuators_dirty = True
        self.led_dirty = True
        self._last_meter_poll = self._clock()
        self.apply_targets(force=True)
        return {**self.status(), "connect_info": info}

    def disconnect(self, *, send_sleep: Optional[bool] = None) -> dict:
        use_sleep = self.config["sleep_on_disconnect"] if send_sleep is None else bool(send_sleep)
        self.client.disconnect(send_sleep=use_sleep, stop_actuators=True)
        self._on_ble_disconnected("manual_disconnect")
        self.last_reason = "disconnected"
        return self.status()

    def emergency_stop(self) -> dict:
        self.config["strategy"] = "preset"
        self.config["preset"] = "stop"
        self.config["led"]["mode"] = "off"
        self._config_saver(self.config)
        self.disconnect(send_sleep=True)
        self.last_reason = "emergency_stop"
        return self.status()

    def reset_fault(self) -> dict:
        self.flow_monitor.reset()
        self.actuators_dirty = True
        self.last_error = ""
        self.last_reason = "fault_reset"
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
                self.client.disconnect(send_sleep=True, stop_actuators=True)
                self._on_ble_disconnected("prepare_for_sleep")
            self.last_reason = "sleeping"
        else:
            self.sleeping = False
            self.last_reason = "resumed"
        return self.status()

    def apply_targets(self, *, force: bool = False) -> None:
        if not self.client.connected:
            return
        now = self._clock()
        self._safe_sample_inputs()

        if self.flow_monitor.update(
            now=now,
            pump_volt=self.applied_pump_volt,
            meter_state=self.client.meter_state,
            flow_protection=self.config["flow_protection"],
        ):
            self.last_error = self.flow_monitor.trip_reason
            if (self.applied_pump_volt, self.applied_pump_duty, self.applied_fan_duty) != (0, 0, 0):
                try:
                    self.client.apply_actuators(0, 0, 0)
                except Exception as exc:
                    self.last_error = f"{self.flow_monitor.trip_reason}; 紧急停泵失败: {exc}"
                self.applied_pump_volt = 0
                self.applied_pump_duty = 0
                self.applied_fan_duty = 0
            self.last_reason = "flow_fault_locked"
            self.actuators_dirty = False
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
            self.client.apply_actuators(pv, pd, fd)
            self.applied_pump_volt = pv
            self.applied_pump_duty = pd
            self.applied_fan_duty = fd
            self.actuators_dirty = False
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
            if self.config["auto_connect"] and not self.flow_monitor.tripped and now - self._last_connect_attempt >= 15.0:
                self._last_connect_attempt = now
                try:
                    self.connect()
                except Exception as exc:
                    self.last_error = str(exc)
            return

        poll_interval = 2.0 if self.applied_pump_volt > 0 else 6.0
        if now - self._last_meter_poll >= poll_interval:
            self._last_meter_poll = now
            try:
                self.client.query_meter()
            except Exception as exc:
                self.last_error = f"流量轮询异常: {exc}"
        try:
            self.apply_targets(force=False)
        except Exception as exc:
            self.last_error = str(exc)


def dispatch_command(coordinator: CoolerCoordinator, req: dict) -> dict:
    if not isinstance(req, dict) or "op" not in req:
        raise ValueError("请求缺少 op 字段")
    op = req["op"]
    if op == "ping":
        return {"pong": True, "connected": coordinator.client.connected}
    if op == "status":
        return coordinator.status()
    if op == "scan":
        return {"devices": coordinator.scan(float(req.get("timeout_s", 5.0)))}
    if op == "connect":
        return coordinator.connect(str(req.get("mac", "")))
    if op == "disconnect":
        return coordinator.disconnect(send_sleep=req.get("send_sleep"))
    if op == "emergency_stop":
        return coordinator.emergency_stop()
    if op == "reset_fault":
        return coordinator.reset_fault()
    if op == "set_config":
        return coordinator.set_config(req.get("config", {}), apply_now=bool(req.get("apply_now", True)))
    if op == "prepare_sleep":
        return coordinator.prepare_for_sleep(bool(req.get("entering", True)))
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


def run_daemon() -> int:
    if os.geteuid() == 0:
        raise RuntimeError("水冷用户服务必须以当前桌面用户运行，请勿使用 root/sudo")
    import gi
    gi.require_version("Gio", "2.0")
    gi.require_version("GLib", "2.0")
    from gi.repository import Gio, GLib

    rdir = runtime_dir()
    _verify_runtime_dir(rdir)
    lpath = lock_path(rdir)
    spath = socket_path(rdir)

    lock_fd = os.open(lpath, os.O_CREAT | os.O_RDWR | os.O_CLOEXEC | os.O_NOFOLLOW, 0o600)
    try:
        fcntl.flock(lock_fd, fcntl.LOCK_EX | fcntl.LOCK_NB)
    except BlockingIOError as exc:
        os.close(lock_fd)
        raise RuntimeError("水冷守护进程已在运行") from exc

    try:
        if spath.exists():
            try:
                is_alive = is_service_running(spath)
            except Exception:
                is_alive = False
            if is_alive:
                raise RuntimeError("水冷控制 socket 仍被占用")
            spath.unlink()

        srv = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
        srv.bind(str(spath))
        os.chmod(spath, 0o600)
        srv.listen(8)
        srv.settimeout(0.5)

        coordinator = CoolerCoordinator()
        loop = GLib.MainLoop()
        system_bus = Gio.bus_get_sync(Gio.BusType.SYSTEM, None)
        stop_flag = [False]

        def on_manager_signal(_conn, _sender, _path, _iface, member, params, *_user_data):
            values = params.unpack()
            if member == "PrepareForSleep":
                coordinator.prepare_for_sleep(bool(values[0]))
                print(json.dumps({"event": "prepare_for_sleep", "entering": bool(values[0])}, ensure_ascii=False), flush=True)
            elif member == "PrepareForShutdown" and values[0]:
                coordinator.prepare_for_sleep(True)
                stop_daemon()

        def on_tick():
            coordinator.tick()
            return True

        def stop_daemon(*_):
            stop_flag[0] = True
            try:
                coordinator.prepare_for_sleep(True)
            except Exception:
                pass
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
            while not stop_flag[0]:
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
                        raw = conn.recv(16384)
                        if not raw or b"\n" not in raw:
                            raise ValueError("水冷服务请求格式无效")
                        req = json.loads(raw.decode("utf-8"))
                        res_q: queue.Queue = queue.Queue(1)

                        def run_main():
                            try:
                                res_q.put({"ok": True, "result": dispatch_command(coordinator, req)})
                            except Exception as exc:
                                res_q.put({"ok": False, "error": str(exc)})
                            return False

                        GLib.idle_add(run_main)
                        resp = res_q.get(timeout=20.0)
                    except Exception as exc:
                        resp = {"ok": False, "error": str(exc)}
                    try:
                        conn.sendall((json.dumps(resp, ensure_ascii=False) + "\n").encode("utf-8"))
                    except OSError:
                        pass

        worker = threading.Thread(target=sock_worker, daemon=True)
        worker.start()
        print(json.dumps({"event": "watercooling_daemon_started", "socket": str(spath)}, ensure_ascii=False), flush=True)
        try:
            loop.run()
        finally:
            stop_flag[0] = True
            GLib.source_remove(tick_id)
            system_bus.signal_unsubscribe(sub_id)
            srv.close()
            if spath.exists():
                try:
                    spath.unlink()
                except OSError:
                    pass
    finally:
        try:
            fcntl.flock(lock_fd, fcntl.LOCK_UN)
        except OSError:
            pass
        os.close(lock_fd)
    return 0

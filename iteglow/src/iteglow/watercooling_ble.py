# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
"""Unpaired BlueZ BLE GATT transport and paced session controller for MECHREVO water coolers.

Verified live rules:
  - Never calls BlueZ Device1.Pair() (LCT22002 rejects SMP with 0x05).
  - Retries Device1.Connect() up to 3 attempts on transient HCI 0x3e / le-connection-abort-by-local.
  - Uses ATT Write Request ('request') with >=100ms TX pacing to avoid MCU UART collision drops.
  - Sends 'sw' handshake + baseline 0xFE 0x32 meter poll on connect.
  - Safely shuts down actuators and sends GoToSleepLC (0xFE 0x19 00 01) on disconnect.
"""
from __future__ import annotations

import math
import time
from typing import Callable, Dict, List, Optional, Protocol, Tuple

from . import watercooling as wc

BLUEZ_SERVICE = "org.bluez"
ADAPTER_PATH = "/org/bluez/hci0"


def _err_text(exc: BaseException) -> str:
    return str(exc).lower()


def _is_in_progress(exc: BaseException) -> bool:
    msg = _err_text(exc)
    return "inprogress" in msg or "already in progress" in msg or "alreadystarted" in msg


def _is_already_connected(exc: BaseException) -> bool:
    msg = _err_text(exc)
    return "alreadyconnected" in msg or "already connected" in msg


def _is_already_notifying(exc: BaseException) -> bool:
    msg = _err_text(exc)
    return "alreadynotifying" in msg or "already notifying" in msg


def _is_disconnect_error(exc: BaseException) -> bool:
    msg = _err_text(exc)
    return any(
        token in msg
        for token in (
            "org.bluez.error.notconnected",
            "notconnected",
            "not connected",
            "disconnected",
            "software caused connection abort",
            "connection reset",
            "transport endpoint is not connected",
            "does not exist",
            "unknownobject",
            "no such object",
            "le-connection-abort",
            "br-connection-",
        )
    )


class BleTransport(Protocol):
    def scan(self, timeout_s: float = 5.0, target_mac: str = "") -> List[dict]: ...
    def connect(
        self,
        *,
        target_mac: str = "",
        scan_timeout_s: float = 6.0,
        on_notify: Optional[Callable[[bytes], None]] = None,
        on_disconnect: Optional[Callable[[str], None]] = None,
    ) -> dict: ...
    def write_frame(self, data: bytes, write_type: str = "request") -> None: ...
    def disconnect(self) -> None: ...
    def pump(self, seconds: float) -> None: ...


class BluezBleTransport:
    """Real BlueZ D-Bus GATT client backed by system Gio/GLib."""

    def __init__(self, adapter_path: str = ADAPTER_PATH):
        self._Gio = None
        self._GLib = None
        self._bus = None
        self._ctx = None
        self._adapter_path = adapter_path
        self._dev_path: Optional[str] = None
        self._tx_path: Optional[str] = None
        self._rx_path: Optional[str] = None
        self._sub_id: Optional[int] = None
        self._notifying = False
        self._connected = False
        self._on_notify: Optional[Callable[[bytes], None]] = None
        self._on_disconnect: Optional[Callable[[str], None]] = None

    def _ensure_bus(self) -> None:
        if self._bus is not None:
            return
        import gi
        gi.require_version("Gio", "2.0")
        gi.require_version("GLib", "2.0")
        from gi.repository import Gio, GLib

        self._Gio = Gio
        self._GLib = GLib
        self._bus = Gio.bus_get_sync(Gio.BusType.SYSTEM, None)
        self._ctx = GLib.MainContext.default()

    def pump(self, seconds: float) -> None:
        self._ensure_bus()
        end = time.monotonic() + max(0.0, seconds)
        while time.monotonic() < end:
            while self._ctx.pending():
                self._ctx.iteration(False)
            time.sleep(min(0.02, max(0.001, end - time.monotonic())))
        while self._ctx.pending():
            self._ctx.iteration(False)

    def _managed_objects(self) -> dict:
        self._ensure_bus()
        res = self._bus.call_sync(
            BLUEZ_SERVICE,
            "/",
            "org.freedesktop.DBus.ObjectManager",
            "GetManagedObjects",
            None,
            self._GLib.VariantType("(a{oa{sa{sv}}})"),
            self._Gio.DBusCallFlags.NONE,
            5000,
            None,
        )
        return res.unpack()[0]

    def _call(self, path: str, iface: str, method: str, params=None, timeout_ms: int = 12000):
        self._ensure_bus()
        return self._bus.call_sync(
            BLUEZ_SERVICE,
            path,
            iface,
            method,
            params,
            None,
            self._Gio.DBusCallFlags.NONE,
            timeout_ms,
            None,
        )

    @staticmethod
    def _matches(dev_props: dict, target_mac: str) -> bool:
        addr = str(dev_props.get("Address", "")).upper()
        if target_mac:
            return addr == target_mac.upper()
        names = (str(dev_props.get("Name", "")), str(dev_props.get("Alias", "")))
        return any(
            nm and (nm.startswith(prefix) or prefix.upper() in nm.upper())
            for nm in names
            for prefix in wc.DEVICE_NAME_PREFIXES
        )

    def _find_matching_devices(self, objs: dict, target_mac: str = "") -> List[dict]:
        found = []
        for path, ifaces in sorted(objs.items()):
            dev = ifaces.get("org.bluez.Device1")
            if not dev or not self._matches(dev, target_mac):
                continue
            found.append({
                "path": path,
                "name": str(dev.get("Name") or dev.get("Alias") or ""),
                "address": str(dev.get("Address", "")).upper(),
                "rssi": dev.get("RSSI"),
                "connected": bool(dev.get("Connected", False)),
                "paired": bool(dev.get("Paired", False)),
            })
        return found

    def _find_nus_chars(self, objs: dict, dev_path: str) -> Tuple[Optional[str], Optional[str]]:
        tx_path = rx_path = None
        prefix = dev_path + "/"
        for path, ifaces in objs.items():
            if not path.startswith(prefix):
                continue
            ch = ifaces.get("org.bluez.GattCharacteristic1")
            if not ch:
                continue
            uuid = str(ch.get("UUID", "")).lower()
            if uuid == wc.NUS_TX_UUID:
                tx_path = path
            elif uuid == wc.NUS_RX_UUID:
                rx_path = path
        return tx_path, rx_path

    def scan(self, timeout_s: float = 5.0, target_mac: str = "") -> List[dict]:
        self._ensure_bus()
        filt = self._GLib.Variant("(a{sv})", ({"Transport": self._GLib.Variant("s", "le")},))
        started = False
        try:
            try:
                self._call(self._adapter_path, "org.bluez.Adapter1", "SetDiscoveryFilter", filt)
            except Exception as exc:
                if not _is_in_progress(exc):
                    raise
            try:
                self._call(self._adapter_path, "org.bluez.Adapter1", "StartDiscovery")
                started = True
            except Exception as exc:
                if not _is_in_progress(exc):
                    raise
            deadline = time.monotonic() + timeout_s
            while time.monotonic() < deadline:
                self.pump(0.25)
                matches = self._find_matching_devices(self._managed_objects(), target_mac)
                if matches and any(m.get("rssi") is not None or m.get("connected") for m in matches):
                    break
            return self._find_matching_devices(self._managed_objects(), target_mac)
        finally:
            if started:
                try:
                    self._call(self._adapter_path, "org.bluez.Adapter1", "StopDiscovery")
                except Exception:
                    pass
            self.pump(0.15)

    def _unhook_signals(self) -> None:
        if self._sub_id is not None:
            try:
                self._bus.signal_unsubscribe(self._sub_id)
            except Exception:
                pass
            self._sub_id = None

    def _hook_signals(self) -> None:
        self._ensure_bus()
        self._unhook_signals()

        def on_signal(_conn, _sender, path, iface, signal, params, *_user_data):
            if iface != "org.freedesktop.DBus.Properties" or signal != "PropertiesChanged":
                return
            changed_iface, changed, _inv = params.unpack()
            if path == self._rx_path and "Value" in changed and self._on_notify is not None:
                self._on_notify(bytes(changed["Value"]))
                return
            if path == self._dev_path and changed_iface == "org.bluez.Device1":
                if "Connected" in changed and changed["Connected"] is False and self._connected:
                    self._connected = False
                    self._notifying = False
                    if self._on_disconnect is not None:
                        self._on_disconnect("peer_or_link_disconnected")

        self._sub_id = self._bus.signal_subscribe(
            BLUEZ_SERVICE,
            None,
            None,
            None,
            None,
            self._Gio.DBusSignalFlags.NONE,
            on_signal,
        )

    def connect(
        self,
        *,
        target_mac: str = "",
        scan_timeout_s: float = 6.0,
        on_notify: Optional[Callable[[bytes], None]] = None,
        on_disconnect: Optional[Callable[[str], None]] = None,
    ) -> dict:
        self._on_notify = on_notify
        self._on_disconnect = on_disconnect

        objs = self._managed_objects()
        cached_matches = self._find_matching_devices(objs, target_mac)
        matches = cached_matches
        if not matches or all(m.get("rssi") is None and not m.get("connected") for m in matches):
            scanned = self.scan(timeout_s=scan_timeout_s, target_mac=target_mac)
            if scanned:
                matches = scanned
            elif cached_matches and target_mac:
                matches = cached_matches
        if not matches:
            raise RuntimeError("未扫描到机械革命蓝牙水冷设备（请确认水冷开机且未处于离线模式）")

        target = matches[0]
        self._dev_path = target["path"]
        self._tx_path = None
        self._rx_path = None
        self._hook_signals()

        if target.get("connected"):
            self._connected = True
        else:
            try:
                self._call(self._dev_path, "org.bluez.Device1", "Connect", timeout_ms=12000)
                self._connected = True
            except Exception as exc:
                if _is_already_connected(exc):
                    self._connected = True
                else:
                    self._unhook_signals()
                    raise RuntimeError(f"BLE 连接水冷失败：{exc}") from exc

        deadline = time.monotonic() + 6.0
        resolved = False
        while time.monotonic() < deadline:
            self.pump(0.15)
            objs = self._managed_objects()
            dev_ifaces = objs.get(self._dev_path, {}).get("org.bluez.Device1", {})
            if dev_ifaces.get("ServicesResolved"):
                resolved = True
                target["name"] = str(dev_ifaces.get("Name") or dev_ifaces.get("Alias") or target["name"])
                target["address"] = str(dev_ifaces.get("Address", target["address"])).upper()
                break
        if not resolved:
            self.disconnect()
            raise RuntimeError("BLE GATT 服务发现超时")

        tx_path, rx_path = self._find_nus_chars(objs, self._dev_path)
        if not tx_path or not rx_path:
            self.disconnect()
            raise RuntimeError("未找到 Nordic UART TX/RX 特征值")
        self._tx_path = tx_path
        self._rx_path = rx_path

        try:
            self._call(self._rx_path, "org.bluez.GattCharacteristic1", "StartNotify", timeout_ms=5000)
        except Exception as exc:
            if not _is_already_notifying(exc):
                self.disconnect()
                raise RuntimeError(f"启用水冷 BLE 通知失败：{exc}") from exc
        self._notifying = True
        self.pump(0.25)
        return {"name": target["name"], "address": target["address"], "path": self._dev_path}

    def write_frame(self, data: bytes, write_type: str = "request") -> None:
        if not self._tx_path or not self._connected:
            raise RuntimeError("水冷蓝牙未连接")
        val_var = self._GLib.Variant("(aya{sv})", (list(data), {"type": self._GLib.Variant("s", write_type)}))
        self._call(self._tx_path, "org.bluez.GattCharacteristic1", "WriteValue", val_var, timeout_ms=5000)

    def disconnect(self) -> None:
        was_connected = self._connected
        self._connected = False
        if self._notifying and self._rx_path and was_connected:
            try:
                self._call(self._rx_path, "org.bluez.GattCharacteristic1", "StopNotify", timeout_ms=3000)
            except Exception:
                pass
            self._notifying = False
            self.pump(0.15)
        if self._dev_path:
            try:
                self._call(self._dev_path, "org.bluez.Device1", "Disconnect", timeout_ms=5000)
            except Exception:
                pass
            self.pump(0.3)
        self._unhook_signals()
        self._tx_path = None
        self._rx_path = None


class CoolerBleClient:
    """High-level paced water cooler session managing handshake, writes, and teardown."""

    def __init__(
        self,
        transport: Optional[BleTransport] = None,
        *,
        min_gap_s: float = 0.1,
        clock: Callable[[], float] = time.monotonic,
    ):
        self.transport = transport or BluezBleTransport()
        self.min_gap_s = float(min_gap_s)
        self._clock = clock
        self._last_tx_at: float = 0.0
        self.connected = False
        self.device_name = ""
        self.device_mac = ""
        self.fw_version: Optional[str] = None
        self.meter_state: Optional[int] = None
        self.last_rx_at: Optional[float] = None
        self.preferred_write_type = "request"
        self.on_disconnect_event: Optional[Callable[[str], None]] = None

    def _on_notify(self, raw: bytes) -> None:
        pkt = wc.parse_rx_packet(raw)
        self.last_rx_at = self._clock()
        if pkt["kind"] == "meter":
            self.meter_state = pkt["meter_state"]
        elif pkt["kind"] == "firmware" and pkt.get("fw_version"):
            self.fw_version = pkt["fw_version"]

    def _on_disconnect(self, reason: str) -> None:
        self.connected = False
        if self.on_disconnect_event is not None:
            self.on_disconnect_event(reason)

    def scan(self, timeout_s: float = 5.0, target_mac: str = "") -> List[dict]:
        if type(timeout_s) not in (int, float) or (isinstance(timeout_s, float) and not math.isfinite(timeout_s)):
            raise ValueError("timeout_s 需要有限数值")
        return self.transport.scan(timeout_s=max(0.5, min(timeout_s, 20.0)), target_mac=target_mac)

    def _handle_link_drop(self, reason: str) -> None:
        was_connected = self.connected
        self.connected = False
        try:
            self.transport.disconnect()
        except Exception:
            pass
        if was_connected and self.on_disconnect_event is not None:
            self.on_disconnect_event(reason)

    def send_paced(self, data: bytes, *, force_type: Optional[str] = None) -> None:
        if not self.connected:
            raise RuntimeError("水冷蓝牙未连接")
        elapsed = self._clock() - self._last_tx_at
        if self._last_tx_at > 0 and elapsed < self.min_gap_s:
            self.transport.pump(self.min_gap_s - elapsed)
        if not self.connected:
            raise RuntimeError("水冷蓝牙连接已断开")
        wtype = force_type or self.preferred_write_type
        try:
            self.transport.write_frame(data, write_type=wtype)
        except Exception as exc:
            if _is_disconnect_error(exc):
                self._handle_link_drop("write_link_lost")
                raise
            if "att error: 0x0e" in _err_text(exc) or "unlikely" in _err_text(exc):
                raise
            if force_type is None and wtype == "request":
                self.preferred_write_type = "command"
                try:
                    self.transport.write_frame(data, write_type="command")
                except Exception as exc2:
                    if _is_disconnect_error(exc2):
                        self._handle_link_drop("write_link_lost")
                    raise
            else:
                raise
        self._last_tx_at = self._clock()

    def connect(self, target_mac: str = "", scan_timeout_s: float = 6.0) -> dict:
        if self.connected:
            return {"name": self.device_name, "address": self.device_mac, "fw_version": self.fw_version,
                    "meter_state": self.meter_state}
        self.fw_version = None
        self.meter_state = None
        last_err: Optional[Exception] = None
        info: Optional[dict] = None
        for attempt in range(1, 4):
            try:
                info = self.transport.connect(
                    target_mac=target_mac,
                    scan_timeout_s=scan_timeout_s,
                    on_notify=self._on_notify,
                    on_disconnect=self._on_disconnect,
                )
                last_err = None
                break
            except Exception as exc:
                last_err = exc
                try:
                    self.transport.disconnect()
                except Exception:
                    pass
                msg = str(exc).lower()
                if attempt < 3 and any(
                    tok in msg
                    for tok in (
                        "le-connection-abort-by-local",
                        "inprogress",
                        "timeout",
                        "page-timeout",
                        "br-connection-canceled",
                    )
                ):
                    self.transport.pump(0.9)
                    continue
                raise
        if last_err is not None or info is None:
            raise RuntimeError(str(last_err))
        self.connected = True
        self.device_name = info.get("name", "")
        self.device_mac = info.get("address", "")
        self.preferred_write_type = "request"
        try:
            self.send_paced(wc.encode_handshake())
            self.transport.pump(0.35)
            self.send_paced(wc.encode_meter_query())
            self.transport.pump(0.25)
            if not self.connected:
                raise RuntimeError("握手期间水冷蓝牙连接已断开")
        except Exception:
            self.disconnect(send_sleep=False, stop_actuators=False)
            raise
        return {
            "name": self.device_name,
            "address": self.device_mac,
            "fw_version": self.fw_version,
            "meter_state": self.meter_state,
        }

    def query_meter(self) -> Optional[int]:
        self.meter_state = None
        try:
            self.send_paced(wc.encode_meter_query())
            self.transport.pump(0.18)
        except Exception:
            self.meter_state = None
            raise
        return self.meter_state

    def apply_actuators(self, pump_volt: int, pump_duty: int, fan_duty: int) -> None:
        self.send_paced(wc.encode_pump(pump_volt, pump_duty))
        self.send_paced(wc.encode_fan(fan_duty))

    def apply_led(self, led_cfg: dict) -> None:
        for frame, _label in wc.encode_led_plan(led_cfg):
            self.send_paced(frame)

    def disconnect(self, *, send_sleep: bool = True, stop_actuators: bool = True) -> None:
        errors = []
        if self.connected:
            if stop_actuators:
                frames = (("水泵", wc.encode_pump(0, 0)), ("风扇", wc.encode_fan(0)),
                          ("风扇灯", wc.encode_fan_led_ctrl(False)), ("主灯", wc.encode_main_led(False, [0, 0, 0], "off")))
                for name, frame in frames:
                    try:
                        self.send_paced(frame)
                    except Exception as exc:
                        errors.append(f"{name}: {exc}")
            if send_sleep:
                try:
                    self.send_paced(wc.encode_sleep(), force_type="command")
                    self.transport.pump(0.25)
                except Exception as exc:
                    errors.append(f"休眠: {exc}")
        self.connected = False
        self.transport.disconnect()
        if errors:
            raise RuntimeError("未确认设备停止指令：" + "；".join(errors))

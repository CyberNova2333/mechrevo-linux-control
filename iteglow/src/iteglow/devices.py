# Portions added/modified by CyberNova are part of MECHREVO Linux Control and licensed under AGPL-3.0-only with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; upstream notices preserved).
"""Locate and talk to the ITE RGB hidraw devices.

Devices are found via their kernel LED node (created by tuxedo-drivers), which
points at the owning HID device and thus its ``hidraw`` node. This is more
robust than guessing ``/dev/hidrawN`` numbers, which are not stable.
"""
from __future__ import annotations

import fcntl
import os
from pathlib import Path
import time
from typing import Iterable, List, Optional, Tuple

from . import protocol
from .protocol import Report
from .zone_lock import ZoneTransaction

# (led node, USB vendor:product, friendly label)
KEYBOARD = ("rgb:kbd_backlight", "048d:600b", "keyboard")
LIGHTBAR = ("rgb:lightbar", "048d:7001", "lightbar")
KNOWN = (KEYBOARD, LIGHTBAR)


def _IOC(d: int, t: str, nr: int, size: int) -> int:
    return (d << 30) | (size << 16) | (ord(t) << 8) | nr


def _HIDIOCSFEATURE(length: int) -> int:
    return _IOC(3, "H", 0x06, length)  # _IOC_WRITE | _IOC_READ


def _HIDIOCGFEATURE(length: int) -> int:
    return _IOC(3, "H", 0x07, length)


# GET_REPORT buffer: 1 report-number byte + 8-byte reply block. Offering
# exactly 8 makes the firmware's reply overflow the transfer (EOVERFLOW).
_REPLY_LEN = 9


def hidraw_for_led(led_name: str) -> Optional[str]:
    """Return ``/dev/hidrawN`` backing the given LED node, or ``None``."""
    base = f"/sys/class/leds/{led_name}/device/hidraw"
    try:
        nodes = os.listdir(base)
    except OSError:
        nodes = []
    if nodes:
        return f"/dev/{nodes[0]}"
    # Local fallback: only the physically verified machine and exact USB IDs.
    board = Path("/sys/class/dmi/id/board_name")
    if not board.exists() or board.read_text().strip() != "YAOSHI Series-XxAF55xx":
        return None
    expected = {"rgb:kbd_backlight": "0003:0000048D:0000600B",
                "rgb:lightbar": "0003:0000048D:00007001"}.get(led_name)
    for node in sorted(Path("/sys/class/hidraw").glob("*")):
        props = dict(line.split("=", 1) for line in (node / "device/uevent").read_text().splitlines() if "=" in line)
        if expected and props.get("HID_ID") == expected:
            return "/dev/" + node.name
    return None


class HidLed:
    """An open hidraw handle for one ITE RGB device."""

    def __init__(self, led_name: str):
        path = hidraw_for_led(led_name)
        if path is None:
            raise FileNotFoundError(
                f"No hidraw device for /sys/class/leds/{led_name}. "
                "Is tuxedo-drivers loaded and your hardware supported? "
                "Run `iteglow doctor`."
            )
        self.led_name = led_name
        self.path = path
        self.fd = os.open(path, os.O_RDWR)

    def send(self, reports: Iterable[Report]) -> None:
        with self.transaction():
            self._send(reports)

    def transaction(self):
        label = next(label for led, _, label in KNOWN if led == self.led_name)
        return ZoneTransaction(label)

    def _send(self, reports: Iterable[Report]) -> None:
        for kind, data in reports:
            if kind == "feature":
                fcntl.ioctl(self.fd, _HIDIOCSFEATURE(len(data)), data)
            elif kind == "output":
                os.write(self.fd, data)
            elif kind == "sleep":       # settle delay, milliseconds in data[0]
                time.sleep(data[0] / 1000)
            else:
                raise ValueError(f"unknown report kind {kind!r}")

    def query(self, command: int) -> bytes:
        """Send a query command (``protocol.QUERY_*``) and return the 8-byte
        reply, starting with an echo of the command byte."""
        with self.transaction():
            self._send(protocol.query_report(command))
            buf = bytearray(_REPLY_LEN)     # buf[0] = report number 0 (unnumbered)
            fcntl.ioctl(self.fd, _HIDIOCGFEATURE(_REPLY_LEN), buf)
            return bytes(buf[1:])

    def close(self) -> None:
        try:
            os.close(self.fd)
        except OSError:
            pass

    def __enter__(self) -> "HidLed":
        return self

    def __exit__(self, *exc) -> None:
        self.close()


def available_devices() -> List[Tuple[str, str, str]]:
    """Return the subset of KNOWN devices present on this system."""
    return [(led, usb, label) for led, usb, label in KNOWN if hidraw_for_led(led)]


def led_for_label(label: str) -> str:
    for led, _usb, lbl in KNOWN:
        if lbl == label:
            return led
    raise KeyError(label)

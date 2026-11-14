"""Low-level command construction for ITE 8291/8233 RGB controllers.

Every public function is pure: it returns a list of ``(kind, payload)`` reports
where ``kind`` is ``"feature"`` (a HID SET_REPORT / feature report) or
``"output"`` (an interrupt-OUT report). Nothing here touches hardware, so the
byte construction is trivially unit-testable. Execution is done by
:class:`iteglow.devices.HidLed`.

Protocol reference: ``docs/protocol.md``. Reverse-engineered from the GPL
tuxedo-drivers sources (``ite_8291.c``, ``ite_8291_lb.c``) plus direct probing
of the 0x7001 lightbar. Command *byte sequences* are functional facts, not the
driver's code; no GPL source is copied here.
"""
from __future__ import annotations

from typing import List, Sequence, Tuple

Report = Tuple[str, bytes]
RGB = Tuple[int, int, int]

KBD_BRIGHTNESS_MAX = 0x32          # keyboard (ITE 8291) max brightness
LB_BRIGHTNESS_MAX = 0x64           # lightbar max brightness
SPEED_MIN, SPEED_MAX = 0x01, 0x0A  # 1 = fastest, 10 = slowest

LB_SELECTOR = 0x22                 # product selector byte for the 0x7001 lightbar
KBD_POWER_ON = 0x02                # power state: 0x01 off, 0x02 on

# Query commands: send as a feature report, then read the reply back with
# GET_REPORT (see HidLed.query). The reply echoes the command byte first.
QUERY_FW_VERSION = 0x80            # reply: 80 <major> <minor> 00 ...
QUERY_EFFECT = 0x88                # reply: 88 <power> <mode> <speed> <brightness> ...
                                   # (keyboard only; the 0x7001 lightbar echoes the
                                   # command but returns zeros for the state bytes)


def _clamp(v: int, lo: int, hi: int) -> int:
    return max(lo, min(hi, int(v)))


def _byte(v: int) -> int:
    return _clamp(v, 0, 0xFF)


def _feature(data: Sequence[int]) -> Report:
    return ("feature", bytes(data))


def _output(data: Sequence[int]) -> Report:
    return ("output", bytes(data))


def query_report(command: int) -> List[Report]:
    """Announce a query (``QUERY_FW_VERSION`` / ``QUERY_EFFECT``); the reply is
    then read with GET_REPORT."""
    return [_feature([_byte(command), 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])]


def parse_fw_version(reply: bytes) -> Tuple[int, int]:
    """Decode a QUERY_FW_VERSION reply into (major, minor)."""
    return (reply[1], reply[2])


def parse_keyboard_state(reply: bytes) -> dict:
    """Decode a QUERY_EFFECT reply (same layout as the 0x08 set command,
    minus the leading command byte)."""
    return {"power": reply[1], "mode": reply[2],
            "speed": reply[3], "brightness": reply[4],
            "direction": reply[6]}


def palette(colors: Sequence[RGB]) -> List[Report]:
    """Define the 7-colour palette used by the multi-colour effects.

    Report: ``14 00 <index 1..7> RR GG BB 00 00``
    """
    reps: List[Report] = []
    for i, (r, g, b) in enumerate(list(colors)[:7]):
        reps.append(_feature([0x14, 0x00, i + 1, _byte(r), _byte(g), _byte(b), 0x00, 0x00]))
    return reps


# --------------------------------------------------------------- lightbar (0x7001)

def lightbar_static(r: int, g: int, b: int, brightness: int) -> List[Report]:
    bright = _clamp(brightness, 0, LB_BRIGHTNESS_MAX)
    return [
        _feature([0x14, 0x00, 0x01, _byte(r), _byte(g), _byte(b), 0x00, 0x00]),
        _feature([0x08, LB_SELECTOR, 0x01, 0x01, bright, 0x01, 0x00, 0x00]),
    ]


def lightbar_effect(effect_id: int, colors: Sequence[RGB], brightness: int,
                    speed: int, direction: int) -> List[Report]:
    """Animated effect. ``08 22 <effect> <speed> <bright> 08 <direction> 00``."""
    bright = _clamp(brightness, 0, LB_BRIGHTNESS_MAX)
    spd = _clamp(speed, SPEED_MIN, SPEED_MAX)
    reps = palette(colors)
    reps.append(_feature([0x08, LB_SELECTOR, _byte(effect_id), spd, bright,
                          0x08, _byte(direction), 0x00]))
    return reps


def lightbar_off() -> List[Report]:
    return lightbar_static(0, 0, 0, 0)


def lightbar_power_on() -> List[Report]:
    """Enable the lightbar controller.

    Required after suspend: the kernel driver's ``write_on()`` has no 0x7001
    case (tuxedo-drivers issue #371), so the controller is never re-enabled on
    resume and subsequent state writes are ignored. Byte sequence verified on
    0x7001 in that issue (matches the 0x6010 variant).
    """
    return [_feature([0x1A, 0x00, 0x01, 0x04, 0x00, 0x00, 0x00, 0x01])]


def lightbar_power_off() -> List[Report]:
    """True power-off sequence (per tuxedo-drivers; the mono-black write and
    delay are needed for the controller to turn off reliably before sleep)."""
    return lightbar_static(0, 0, 0, 0) + [
        ("sleep", bytes([50])),
        _feature([0x12, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00]),
        _feature([0x08, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]),
        _feature([0x08, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]),
        _feature([0x1A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01]),
    ]


# --------------------------------------------------------------- keyboard (0x600b)

def keyboard_brightness(brightness: int) -> List[Report]:
    """``09 02 <brightness> 00 00 00 00 00``"""
    bright = _clamp(brightness, 0, KBD_BRIGHTNESS_MAX)
    return [_feature([0x09, 0x02, bright, 0x00, 0x00, 0x00, 0x00, 0x00])]


def keyboard_effect(mode: int, colors: Sequence[RGB], brightness: int,
                    speed: int, direction: int, use_palette: bool = True) -> List[Report]:
    """``08 02 <mode> <speed> <bright> 08 <direction> 00`` (+ palette first)."""
    bright = _clamp(brightness, 0, KBD_BRIGHTNESS_MAX)
    spd = _clamp(speed, SPEED_MIN, SPEED_MAX)
    reps = palette(colors) if use_palette else []
    reps.append(_feature([0x08, KBD_POWER_ON, _byte(mode), spd, bright,
                          0x08, _byte(direction), 0x00]))
    return reps


def keyboard_solid(r: int, g: int, b: int, brightness: int) -> List[Report]:
    """Solid colour across the whole keyboard via "user mode" 0x33.

    Sends the control params, announces 8 data packets, then writes 8 x 65-byte
    output reports whose payload repeats the ``[0, R, G, B]`` pattern.
    """
    bright = _clamp(brightness, 0, KBD_BRIGHTNESS_MAX)
    reps: List[Report] = [
        _feature([0x08, KBD_POWER_ON, 0x33, 0x00, bright, 0x00, 0x00, 0x00]),
        _feature([0x12, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00]),
    ]
    r, g, b = _byte(r), _byte(g), _byte(b)
    pkt = bytearray(65)            # pkt[0] = 0 (report id)
    for i in range(1, 65):
        if (i + 2) % 4 == 0:
            pkt[i] = r
        if (i + 1) % 4 == 0:
            pkt[i] = g
        if i % 4 == 0:
            pkt[i] = b
    reps += [_output(bytes(pkt)) for _ in range(8)]
    return reps


def keyboard_off() -> List[Report]:
    return keyboard_brightness(0)

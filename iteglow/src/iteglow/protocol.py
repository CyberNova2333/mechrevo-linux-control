# Portions added/modified by CyberNova are part of MECHREVO Linux Control and licensed under AGPL-3.0-only with Additional Terms (see LICENSE and ADDITIONAL_TERMS.zh-CN.md; upstream notices preserved).
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
KBD_MATRIX_ROWS = 6
KBD_MATRIX_COLS = 21
KBD_MATRIX_CELLS = KBD_MATRIX_ROWS * KBD_MATRIX_COLS
KBD_WIRE_SLOTS = 128
LB_BRIGHTNESS_MAX = 0x64           # lightbar max brightness
SPEED_MIN, SPEED_MAX = 0x01, 0x0A  # 1 = fastest, 10 = slowest

# Exact per-key color compensation table from OEM WKDColor.cheatRGB_HIDKeyboard3.
_HID_KEYBOARD3_CALIBRATION = {
    (255, 255, 255): (125, 255, 185),
    (255, 165, 0): (255, 125, 0),
    (255, 255, 0): (210, 255, 0),
    (0, 0, 255): (0, 20, 255),
    (139, 0, 255): (110, 0, 255),
}

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


def calibrate_keyboard3(rgb: RGB) -> RGB:
    """Apply OEM WKDColor white/primary compensation when requested."""
    key = (_byte(rgb[0]), _byte(rgb[1]), _byte(rgb[2]))
    return _HID_KEYBOARD3_CALIBRATION.get(key, key)


def _pack_usermode(slots: Sequence[RGB], brightness: int) -> List[Report]:
    """Build 0x33 / 12H feature + 8x65-byte output reports from 128 wire slots."""
    bright = _clamp(brightness, 0, KBD_BRIGHTNESS_MAX)
    reps: List[Report] = [
        _feature([0x08, KBD_POWER_ON, 0x33, 0x00, bright, 0x00, 0x00, 0x00]),
        _feature([0x12, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00]),
    ]
    for pkt_idx in range(8):
        pkt = bytearray(65)  # pkt[0] = 0 (report id)
        base_slot = pkt_idx * 16
        for j in range(16):
            slot_idx = base_slot + j
            r, g, b = slots[slot_idx] if slot_idx < len(slots) else (0, 0, 0)
            off = 1 + j * 4
            pkt[off + 1] = _byte(r)
            pkt[off + 2] = _byte(g)
            pkt[off + 3] = _byte(b)
        reps.append(_output(bytes(pkt)))
    return reps


def keyboard_solid(r: int, g: int, b: int, brightness: int) -> List[Report]:
    """Solid colour across the whole keyboard via "user mode" 0x33.

    Sends the control params, announces 8 data packets, then writes 8 x 65-byte
    output reports whose payload repeats the ``[0, R, G, B]`` pattern.
    """
    rgb = (_byte(r), _byte(g), _byte(b))
    return _pack_usermode([rgb] * KBD_WIRE_SLOTS, brightness)


def keyboard_matrix(cells: Sequence[RGB], brightness: int,
                    calibrate: bool = False) -> List[Report]:
    """Per-key 6x21 matrix via 12H (``0x33``).

    ``cells`` is row-major top-to-bottom (row 0 top Esc row .. row 5 bottom row).
    Firmware 12H expects column-major slots ``col * 6 + (5 - row)`` padded to 128.
    """
    if len(cells) != KBD_MATRIX_CELLS:
        raise ValueError(f"expected {KBD_MATRIX_CELLS} matrix cells, got {len(cells)}")
    slots: List[RGB] = [(0, 0, 0)] * KBD_WIRE_SLOTS
    for col in range(KBD_MATRIX_COLS):
        for hw_row in range(KBD_MATRIX_ROWS):
            top_row = KBD_MATRIX_ROWS - 1 - hw_row
            rgb = tuple(cells[top_row * KBD_MATRIX_COLS + col])
            if calibrate:
                rgb = calibrate_keyboard3(rgb)  # type: ignore[arg-type]
            slots[col * KBD_MATRIX_ROWS + hw_row] = rgb  # type: ignore[assignment]
    return _pack_usermode(slots, brightness)


def keyboard_off() -> List[Report]:
    return keyboard_brightness(0)

"""Mode metadata + a single dispatch point shared by the CLI and GUI.

Each mode maps to a spec dict with the effect ``code`` and which controls it
uses (``single`` colour, ``palette``, ``speed``, ``direction``). ``off`` is a
pseudo-mode handled specially.
"""
from __future__ import annotations

from typing import Dict, List, Optional, Sequence

from . import protocol
from .protocol import RGB, Report

RAINBOW: List[RGB] = [
    (255, 0, 0), (255, 165, 0), (255, 255, 0), (0, 255, 0),
    (0, 0, 255), (0, 255, 255), (139, 0, 255),
]

LIGHTBAR_MODES: Dict[str, dict] = {
    "off":       dict(code=0x00, off=True),
    "static":    dict(code=0x01, single=True),
    "breathing": dict(code=0x02, palette=True, speed=True),
    "wave":      dict(code=0x03, speed=True, direction=True),  # fixed rainbow; palette ignored
    "marquee":   dict(code=0x04, palette=True, speed=True, direction=True),
    "neon":      dict(code=0x05, palette=True, speed=True, direction=True),
    "runner":    dict(code=0x06, palette=True, speed=True, direction=True),
}

KEYBOARD_MODES: Dict[str, dict] = {
    "off":       dict(code=0x00, off=True),
    "solid":     dict(code=0x33, single=True),
    "breathing": dict(code=0x02, palette=True, speed=True),
    "wave":      dict(code=0x03, palette=True, speed=True, direction=True),
    "reactive":  dict(code=0x04, palette=True, speed=True, trigger=True),
    "rainbow":   dict(code=0x05, speed=True),
    "ripple":    dict(code=0x06, palette=True, speed=True, trigger=True),
    "marquee":   dict(code=0x09, palette=True, speed=True),
    "raindrop":  dict(code=0x0A, palette=True, speed=True),
    "aurora":    dict(code=0x0E, palette=True, speed=True, trigger=True),
    "spark":     dict(code=0x11, palette=True, speed=True, trigger=True),
}

# direction name -> byte
LIGHTBAR_DIRECTIONS = {"forward": 1, "reverse": 2}
KEYBOARD_DIRECTIONS = {"ltr": 1, "rtl": 2, "btt": 3, "ttb": 4}

MODES_BY_LABEL = {"lightbar": LIGHTBAR_MODES, "keyboard": KEYBOARD_MODES}
DIRECTIONS_BY_LABEL = {"lightbar": LIGHTBAR_DIRECTIONS, "keyboard": KEYBOARD_DIRECTIONS}


def build(label: str, mode: str, *,
          color: RGB = (0, 255, 255),
          colors: Optional[Sequence[RGB]] = None,
          brightness: Optional[int] = None,
          speed: int = 5,
          direction: int = 1,
          reactive: bool = False) -> List[Report]:
    """Translate a (device, mode, params) selection into protocol reports."""
    modes = MODES_BY_LABEL[label]
    if mode not in modes:
        raise ValueError(f"unknown {label} mode {mode!r}; choose from {sorted(modes)}")
    spec = modes[mode]
    pal = list(colors) if colors else RAINBOW

    if label == "lightbar":
        bright = protocol.LB_BRIGHTNESS_MAX if brightness is None else brightness
        if spec.get("off"):
            return protocol.lightbar_off()
        if spec.get("single"):
            return protocol.lightbar_static(*color, bright)
        return protocol.lightbar_effect(spec["code"], pal, bright, speed, direction)

    # keyboard
    bright = protocol.KBD_BRIGHTNESS_MAX if brightness is None else brightness
    if spec.get("off"):
        return protocol.keyboard_off()
    if spec.get("single"):
        return protocol.keyboard_solid(*color, bright)
    # Byte 6 is overloaded: wave direction vs. key trigger (0 auto, 1 reactive).
    # Never reuse a saved wave direction for a key-trigger-capable effect.
    behavior = int(bool(reactive)) if spec.get("trigger") else direction if spec.get("direction") else 0
    return protocol.keyboard_effect(spec["code"], pal, bright, speed, behavior,
                                    use_palette=bool(spec.get("palette")))

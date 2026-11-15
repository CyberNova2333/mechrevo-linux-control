"""Physical 101-key layout (MEZone_3p1nd_101) and per-key matrix presets.

All matrix cell coordinates use the logical top-to-bottom 6x21 grid
(``idx = row * 21 + col``, ``row=0`` top Esc/Fn row, ``row=5`` bottom Ctrl/Space
row). :func:`iteglow.protocol.keyboard_matrix` handles translating logical grid
order into firmware ``12H`` wire slots.
"""
from __future__ import annotations

from dataclasses import dataclass
from typing import Dict, List, Sequence, Tuple

from .protocol import KBD_MATRIX_CELLS, KBD_MATRIX_COLS, KBD_MATRIX_ROWS, RGB


@dataclass(frozen=True)
class KeyCap:
    name: str
    row: int
    col: int
    width: float
    indices: Tuple[int, ...]
    group: str = "alpha"

    @property
    def primary(self) -> int:
        return self.indices[0]


def _cap(name: str, row: int, col: int, width: float = 1.0,
         extra: Sequence[int] = (), group: str = "alpha") -> KeyCap:
    primary = row * KBD_MATRIX_COLS + col
    return KeyCap(name=name, row=row, col=col, width=width,
                  indices=(primary, *extra), group=group)


# Verified 101-key physical layout (20 + 18 + 18 + 16 + 16 + 13 = 101)
# Matches OEM SetColorKeyNumbers3p_101_Mapping() + hardware 12H verification.
KEYBOARD_101_ROWS: Tuple[Tuple[KeyCap, ...], ...] = (
    # Row 0 (top): Esc..F12 + nav + top numpad (indices 0..19)
    (
        _cap("Esc", 0, 0, group="mod"),
        *(_cap(f"F{i}", 0, i, group="fn") for i in range(1, 13)),
        _cap("PrtSc", 0, 13, group="nav"),
        _cap("Pause", 0, 14, group="nav"),
        _cap("Del", 0, 15, group="nav"),
        _cap("Home", 0, 16, group="nav"),
        _cap("PgUp", 0, 17, group="nav"),
        _cap("PgDn", 0, 18, group="nav"),
        _cap("End", 0, 19, group="nav"),
    ),
    # Row 1: `..= (21..33), Bksp (35), NumLk / * - (36..39)
    (
        *(_cap(lbl, 1, c, group="alpha") for c, lbl in enumerate(
            ("`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "="))),
        _cap("Bksp", 1, 14, width=2.0, group="mod"),
        _cap("NumLk", 1, 15, group="num"),
        _cap("/", 1, 16, group="num"),
        _cap("*", 1, 17, group="num"),
        _cap("-", 1, 18, group="num"),
    ),
    # Row 2: Tab (42), Q..\ (44..56), Num7..+ (57..60)
    (
        _cap("Tab", 2, 0, width=1.5, group="mod"),
        *(_cap(lbl, 2, 2 + i, group=("wasd" if lbl == "W" else "alpha"))
          for i, lbl in enumerate(
              ("Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]", "\\"))),
        _cap("Num7", 2, 15, group="num"),
        _cap("Num8", 2, 16, group="num"),
        _cap("Num9", 2, 17, group="num"),
        _cap("+", 2, 18, group="num"),
    ),
    # Row 3: Caps (63), A..' (65..75), Enter (77), Num4..6 (78..80)
    (
        _cap("Caps", 3, 0, width=1.8, group="mod"),
        *(_cap(lbl, 3, 2 + i, group=("wasd" if lbl in ("A", "S", "D") else "alpha"))
          for i, lbl in enumerate(
              ("A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'"))),
        _cap("Enter", 3, 14, width=2.0, group="mod"),
        _cap("Num4", 3, 15, group="num"),
        _cap("Num5", 3, 16, group="num"),
        _cap("Num6", 3, 17, group="num"),
    ),
    # Row 4: LShift (84,85), Z../ (87..96), RShift (98), Up (99), Num1..3 (100..102)
    (
        _cap("LShift", 4, 0, width=2.3, extra=(85,), group="mod"),
        *(_cap(lbl, 4, 3 + i, group="alpha") for i, lbl in enumerate(
            ("Z", "X", "C", "V", "B", "N", "M", ",", ".", "/"))),
        _cap("RShift", 4, 14, width=1.7, group="mod"),
        _cap("↑", 4, 15, group="arrows"),
        _cap("Num1", 4, 16, group="num"),
        _cap("Num2", 4, 17, group="num"),
        _cap("Num3", 4, 18, group="num"),
    ),
    # Row 5: LCtrl (105), Fn (107), Win (108), LAlt (109), Space (112),
    # RAlt (115), RCtrl (117), Left/Down/Right (118..120), Num0/./Ent (121..123)
    (
        _cap("LCtrl", 5, 0, width=1.5, group="mod"),
        _cap("Fn", 5, 2, group="mod"),
        _cap("Win", 5, 3, group="mod"),
        _cap("LAlt", 5, 4, group="mod"),
        _cap("Space", 5, 7, width=4.0, extra=(110, 111, 113, 114), group="mod"),
        _cap("RAlt", 5, 10, group="mod"),
        _cap("RCtrl", 5, 12, group="mod"),
        _cap("←", 5, 13, group="arrows"),
        _cap("↓", 5, 14, group="arrows"),
        _cap("→", 5, 15, group="arrows"),
        _cap("Num0", 5, 16, width=1.5, group="num"),
        _cap("Num.", 5, 17, group="num"),
        _cap("NumEnt", 5, 18, group="num"),
    ),
)

ALL_KEYCAPS: Tuple[KeyCap, ...] = tuple(cap for row in KEYBOARD_101_ROWS for cap in row)

PRESETS: Dict[str, str] = {
    "gaming": "游戏分区 (WASD/方向红光 + 功能区蓝紫 + 主区青色)",
    "wasd_fps": "FPS 战术高亮 (仅亮 WASD 与方向键)",
    "zones": "功能分区 (字母/功能/修饰/小键盘分色)",
    "rainbow_grid": "彩虹渐变矩阵",
    "gradient": "双色横向渐变",
}


def blank_cells(fill: RGB = (0, 0, 0)) -> List[RGB]:
    rgb = (int(fill[0]), int(fill[1]), int(fill[2]))
    return [rgb] * KBD_MATRIX_CELLS


def _lerp(c1: RGB, c2: RGB, t: float) -> RGB:
    t = max(0.0, min(1.0, t))
    return (
        round(c1[0] + (c2[0] - c1[0]) * t),
        round(c1[1] + (c2[1] - c1[1]) * t),
        round(c1[2] + (c2[2] - c1[2]) * t),
    )


def _hue_rgb(pos: float) -> RGB:
    h = (pos % 1.0) * 6.0
    i = int(h) % 6
    f = h - int(h)
    up, down = round(255 * f), round(255 * (1.0 - f))
    return (
        (255, up, 0), (down, 255, 0), (0, 255, up),
        (0, down, 255), (up, 0, 255), (255, 0, down),
    )[i]


def preset_cells(name: str, accent: RGB = (0, 255, 255),
                 secondary: RGB = (255, 0, 128)) -> List[RGB]:
    """Generate a 126-cell RGB matrix for one of ``PRESETS``."""
    if name not in PRESETS:
        raise ValueError(f"unknown matrix preset {name!r}; choose from {sorted(PRESETS)}")
    cells = blank_cells()
    if name == "rainbow_grid":
        for r in range(KBD_MATRIX_ROWS):
            for c in range(KBD_MATRIX_COLS):
                cells[r * KBD_MATRIX_COLS + c] = _hue_rgb(c / 19.0 + r * 0.04)
        return cells
    if name == "gradient":
        for r in range(KBD_MATRIX_ROWS):
            for c in range(KBD_MATRIX_COLS):
                cells[r * KBD_MATRIX_COLS + c] = _lerp(accent, secondary, c / 18.0)
        return cells

    group_colors: Dict[str, RGB]
    if name == "wasd_fps":
        group_colors = {
            "wasd": (255, 40, 0),
            "arrows": accent,
            "fn": (0, 0, 0), "nav": (0, 0, 0), "mod": (0, 0, 0),
            "num": (0, 0, 0), "alpha": (0, 0, 0),
        }
        for idx in (0, 63, 77, 84, 85, 112):  # Esc, Caps, Enter, Shift, Space dim cue
            cells[idx] = (20, 45, 70)
    elif name == "gaming":
        group_colors = {
            "wasd": (255, 32, 0),
            "arrows": (255, 140, 0),
            "fn": (110, 0, 255),
            "nav": (0, 120, 255),
            "mod": (0, 90, 255),
            "num": (0, 220, 160),
            "alpha": accent,
        }
    else:  # zones
        group_colors = {
            "wasd": accent,
            "alpha": accent,
            "fn": (255, 165, 0),
            "nav": (255, 220, 0),
            "mod": (110, 80, 255),
            "arrows": (255, 60, 120),
            "num": (0, 220, 140),
        }
    for cap in ALL_KEYCAPS:
        col = group_colors[cap.group]
        for idx in cap.indices:
            if 0 <= idx < KBD_MATRIX_CELLS:
                cells[idx] = col
    return cells

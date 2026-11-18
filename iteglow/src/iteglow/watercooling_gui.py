# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
"""GTK4 water-cooling control tab integrated into iteglow main window."""
from __future__ import annotations

import math
from pathlib import Path
import threading
from typing import Callable, List, Optional

import gi

gi.require_version("Gdk", "4.0")
gi.require_version("GLib", "2.0")
gi.require_version("Gtk", "4.0")
from gi.repository import Gdk, GLib, Gtk

from . import watercooling as wc
from .watercooling_service import service_request


STRATEGY_ITEMS = (
    ("preset", "预设模式"),
    ("manual", "手动固定"),
    ("smart_curve", "智能温控曲线"),
    ("ec_follow", "跟随主机 EC"),
)

PRESET_ITEMS = (
    ("stop", "停机 (0V / 0%)"),
    ("silent", "静音 (7V · 泵60% · 扇40%)"),
    ("balanced", "均衡 (8V · 泵60% · 扇50%)"),
    ("performance", "性能 (11V · 泵60% · 扇60%)"),
    ("turbo", "狂暴 (11V · 泵90% · 扇90%)"),
    ("max", "全速 (12V · 泵100% · 扇100%)"),
)

VOLT_ITEMS = (
    (0, "0V (停泵)"),
    (7, "7V"),
    (8, "8V"),
    (11, "11V"),
    (12, "12V"),
)

LED_MODE_ITEMS = (
    ("off", "关闭灯光"),
    ("static", "静态单色"),
    ("breathing", "机身呼吸"),
    ("colorful", "机身幻彩"),
    ("breathing_colorful", "机身呼吸幻彩"),
    ("mode4", "全区静态"),
)

FAN_POLICY_ITEMS = (
    ("sync", "双区同步 (风扇=机身)"),
    ("body_only", "仅机身灯亮 (风扇关)"),
    ("freeze", "锁定风扇当前灯态"),
    ("split", "双区独立双色"),
)


def _label(text: str = "", css: Optional[str] = None, wrap: bool = False) -> Gtk.Label:
    lbl = Gtk.Label(label=text)
    lbl.set_xalign(0)
    lbl.set_wrap(wrap)
    if css:
        for cls in css.split():
            lbl.add_css_class(cls)
    return lbl


def _box(vertical: bool = True, spacing: int = 8, css: Optional[str] = None) -> Gtk.Box:
    orient = Gtk.Orientation.VERTICAL if vertical else Gtk.Orientation.HORIZONTAL
    box = Gtk.Box(orientation=orient, spacing=spacing)
    if css:
        for cls in css.split():
            box.add_css_class(cls)
    return box


def _card(title: str, subtitle: Optional[str] = None) -> Gtk.Box:
    card = _box(vertical=True, spacing=12, css="control-card")
    head = _box(vertical=True, spacing=3)
    head.append(_label(title, "section-title"))
    if subtitle:
        head.append(_label(subtitle, "small-note", wrap=True))
    card.append(head)
    return card


def _dropdown(items) -> Gtk.DropDown:
    labels = [lbl for _, lbl in items]
    dd = Gtk.DropDown.new_from_strings(labels)
    dd.set_hexpand(False)
    return dd


def _selected_key(dd: Gtk.DropDown, items):
    idx = dd.get_selected()
    if 0 <= idx < len(items):
        return items[idx][0]
    return items[0][0]


def _select_key(dd: Gtk.DropDown, items, key) -> None:
    for idx, (k, _) in enumerate(items):
        if k == key:
            dd.set_selected(idx)
            return


def _spin(low: int, high: int, value: int, step: int = 1) -> Gtk.SpinButton:
    sb = Gtk.SpinButton.new_with_range(low, high, step)
    sb.set_digits(0)
    sb.set_value(value)
    return sb


def _rgba(rgb: List[int]) -> Gdk.RGBA:
    c = Gdk.RGBA()
    r, g, b = rgb
    c.red, c.green, c.blue, c.alpha = r / 255.0, g / 255.0, b / 255.0, 1.0
    return c


def _rgb_from_btn(btn: Gtk.ColorDialogButton) -> List[int]:
    c = btn.get_rgba()
    return [
        max(0, min(255, int(round(c.red * 255)))),
        max(0, min(255, int(round(c.green * 255)))),
        max(0, min(255, int(round(c.blue * 255)))),
    ]


def _color_btn(rgb: List[int]) -> Gtk.ColorDialogButton:
    dlg = Gtk.ColorDialog(with_alpha=False)
    btn = Gtk.ColorDialogButton(dialog=dlg)
    btn.add_css_class("color-swatch")
    btn.set_rgba(_rgba(rgb))
    return btn


def _row(title: str, widget: Gtk.Widget, note: Optional[str] = None, well: bool = False) -> Gtk.Box:
    outer = _box(vertical=False, spacing=12, css="well" if well else None)
    outer.set_valign(Gtk.Align.CENTER)
    left = _box(vertical=True, spacing=2)
    left.set_hexpand(True)
    left.append(_label(title))
    if note:
        left.append(_label(note, "small-note", wrap=True))
    outer.append(left)
    widget.set_valign(Gtk.Align.CENTER)
    outer.append(widget)
    return outer


def _rounded(cr, x: float, y: float, w: float, h: float, radius: float) -> None:
    cr.new_sub_path()
    for cx, cy, start in (
        (x + w - radius, y + radius, -math.pi / 2),
        (x + w - radius, y + h - radius, 0),
        (x + radius, y + h - radius, math.pi / 2),
        (x + radius, y + radius, math.pi),
    ):
        cr.arc(cx, cy, radius, start, start + math.pi / 2)
    cr.close_path()


class CoolerHardwarePreview(Gtk.DrawingArea):
    """Live dual-zone silhouette preview for LCT22002 fan ring, chassis lightbar, and flow loop."""

    def __init__(self, panel: "WaterCoolingPanel"):
        super().__init__()
        self._panel = panel
        self.set_content_width(248)
        self.set_content_height(196)
        self.set_hexpand(False)
        self.set_draw_func(self._draw)

    def _draw(self, _area, cr, width: int, height: int) -> None:
        _rounded(cr, 0, 0, width, height, 12)
        cr.set_source_rgb(0.06, 0.08, 0.12)
        cr.fill_preserve()
        cr.set_source_rgb(0.16, 0.21, 0.30)
        cr.set_line_width(1.0)
        cr.stroke()

        mode = _selected_key(self._panel.led_mode_dd, LED_MODE_ITEMS)
        policy = _selected_key(self._panel.fan_policy_dd, FAN_POLICY_ITEMS)
        body_rgb = [c / 255.0 for c in _rgb_from_btn(self._panel.body_color_btn)]
        fan_rgb = [c / 255.0 for c in _rgb_from_btn(self._panel.fan_color_btn)]

        if mode in ("colorful", "breathing_colorful"):
            body_eff = (0.32, 0.78, 0.96)
        elif mode == "off":
            body_eff = None
        else:
            body_eff = tuple(body_rgb)

        if mode == "off" or policy == "body_only":
            fan_eff = None
        elif policy == "sync":
            fan_eff = body_eff
        elif policy == "split":
            fan_eff = tuple(fan_rgb)
        else:
            fan_eff = (0.45, 0.56, 0.72)  # latched unknown prior hue

        # Chassis body outline
        bx, by, bw, bh = 18.0, 24.0, width - 36.0, height - 52.0
        _rounded(cr, bx, by, bw, bh, 12)
        cr.set_source_rgb(0.10, 0.13, 0.19)
        cr.fill_preserve()
        cr.set_source_rgb(0.22, 0.28, 0.38)
        cr.set_line_width(1.2)
        cr.stroke()

        # Side radiator louvers
        cr.set_source_rgb(0.15, 0.20, 0.28)
        for i in range(6):
            ly = by + 22 + i * 16
            _rounded(cr, bx + 12, ly, 24, 6, 3)
            cr.fill()

        # Fan hub + illuminated ring (center-right)
        cx = bx + bw * 0.61
        cy = by + bh * 0.50
        radius = min(bw, bh) * 0.34
        if fan_eff is not None:
            cr.arc(cx, cy, radius + 4.0, 0, 2 * math.pi)
            cr.set_source_rgba(fan_eff[0], fan_eff[1], fan_eff[2], 0.20)
            cr.set_line_width(8.0)
            cr.stroke()
        cr.arc(cx, cy, radius, 0, 2 * math.pi)
        cr.set_source_rgb(0.07, 0.09, 0.14)
        cr.fill_preserve()
        if fan_eff is not None:
            cr.set_source_rgb(*fan_eff)
            cr.set_line_width(3.4)
        else:
            cr.set_source_rgb(0.20, 0.25, 0.34)
            cr.set_line_width(2.0)
        cr.stroke()

        # Fan blades + center cap
        cr.set_source_rgb(0.18, 0.23, 0.32)
        cr.set_line_width(1.6)
        for k in range(7):
            ang = k * (2 * math.pi / 7)
            cr.move_to(cx + math.cos(ang) * 7, cy + math.sin(ang) * 7)
            cr.line_to(cx + math.cos(ang + 0.35) * (radius - 4), cy + math.sin(ang + 0.35) * (radius - 4))
        cr.stroke()
        cr.arc(cx, cy, 7.5, 0, 2 * math.pi)
        cr.set_source_rgb(0.23, 0.30, 0.42)
        cr.fill()

        # Chassis edge lightbar strip (adjacent to fan)
        bar_x, bar_y, bar_w, bar_h = bx + bw - 20, by + 16, 7.0, bh - 32
        if body_eff is not None:
            _rounded(cr, bar_x - 3, bar_y - 2, bar_w + 6, bar_h + 4, 6)
            cr.set_source_rgba(body_eff[0], body_eff[1], body_eff[2], 0.24)
            cr.fill()
            _rounded(cr, bar_x, bar_y, bar_w, bar_h, 3.5)
            cr.set_source_rgb(*body_eff)
            cr.fill()
        else:
            _rounded(cr, bar_x, bar_y, bar_w, bar_h, 3.5)
            cr.set_source_rgb(0.20, 0.25, 0.34)
            cr.fill()

        # Firmware BLE status indicator dot
        dot_x, dot_y = bx + 16, by + 12
        cr.arc(dot_x, dot_y, 3.2, 0, 2 * math.pi)
        if self._panel._last_snapshot.get("flow_tripped"):
            cr.set_source_rgb(0.96, 0.26, 0.36)
        elif self._panel._last_snapshot.get("connected"):
            cr.set_source_rgb(0.22, 0.82, 0.56)
        else:
            cr.set_source_rgb(0.26, 0.56, 0.96)
        cr.fill()

        # Footer caption
        cr.select_font_face("sans-serif", 0, 0)
        cr.set_font_size(10)
        cr.set_source_rgb(0.54, 0.63, 0.76)
        cr.move_to(18, height - 11)
        cr.show_text("LCT22002 双区灯光与状态指示预览")


class CoolerCurveChart(Gtk.DrawingArea):
    """Dark precision chart plotting smart-curve thermal tiers."""

    def __init__(self, panel: "WaterCoolingPanel"):
        super().__init__()
        self._panel = panel
        self.set_content_height(136)
        self.set_hexpand(True)
        self.set_draw_func(self._draw)

    def _draw(self, _area, cr, width: int, height: int) -> None:
        left, right, top, bottom = 36, width - 16, 14, height - 24
        _rounded(cr, 0, 0, width, height, 10)
        cr.set_source_rgb(0.06, 0.08, 0.12)
        cr.fill_preserve()
        cr.set_source_rgb(0.15, 0.20, 0.28)
        cr.set_line_width(1.0)
        cr.stroke()

        cr.set_font_size(10)
        for pct in (0, 50, 100):
            y = bottom - (pct / 100.0) * (bottom - top)
            cr.set_source_rgb(0.13, 0.17, 0.24)
            cr.move_to(left, y)
            cr.line_to(right, y)
            cr.stroke()
            cr.set_source_rgb(0.50, 0.58, 0.70)
            cr.move_to(6, y + 3)
            cr.show_text(f"{pct}%")

        rows = self._panel._curve_rows
        if not rows:
            return
        pts = sorted(
            (
                r["temp"].get_value_as_int(),
                r["pump_duty"].get_value_as_int(),
                r["fan_duty"].get_value_as_int(),
            )
            for r in rows
        )
        t_min, t_max = 25.0, 100.0

        for idx, color in ((2, (0.22, 0.74, 0.97)), (1, (0.39, 0.40, 0.95))):
            coords = [
                (
                    left + (t - t_min) / (t_max - t_min) * (right - left),
                    bottom - (val / 100.0) * (bottom - top),
                )
                for t, *vals in [(p[0], p[1], p[2]) for p in pts]
                for val in [vals[idx - 1]]
            ]
            cr.set_source_rgb(*color)
            cr.set_line_width(2.2)
            for i, (x, y) in enumerate(coords):
                if i == 0:
                    cr.move_to(x, y)
                else:
                    cr.line_to(x, y)
            cr.stroke()
            for x, y in coords:
                cr.arc(x, y, 3.2, 0, 2 * math.pi)
                cr.fill()

        cr.set_source_rgb(0.50, 0.58, 0.70)
        for temp in (30, 50, 70, 90):
            tx = left + (temp - t_min) / (t_max - t_min) * (right - left) - 10
            cr.move_to(tx, height - 7)
            cr.show_text(f"{temp}°C")


class WaterCoolingPanel(Gtk.Box):
    """UI panel for BLE water-cooler telemetry, pump/fan strategies, and dual-zone LEDs.

    Construction performs no D-Bus, BLE, socket, or disk write I/O.
    """

    def __init__(
        self,
        *,
        config_path: Optional[Path] = None,
        request_fn: Optional[Callable[[dict], dict]] = None,
        poll_interval_ms: int = 3000,
    ):
        super().__init__(orientation=Gtk.Orientation.VERTICAL, spacing=0)
        self._config_path = config_path
        self._request_fn: Callable[[dict], dict] = request_fn or service_request
        self._poll_interval_ms = int(poll_interval_ms)
        self._poll_id: Optional[int] = None
        self._closed = False
        self._syncing_ui = False
        self._op_lock = threading.Lock()
        self._busy = False
        self._curve_rows: List[dict] = []
        self._last_snapshot: dict = {}

        try:
            self._config = wc.load_config(self._config_path)
            init_err = ""
        except Exception as exc:
            self._config = wc.default_config()
            init_err = f"配置加载失败，已使用默认安全配置: {exc}"

        root = _box(vertical=True, spacing=16, css="device-page")

        # --- Page Title Row ---
        title_row = _box(vertical=False, spacing=12)
        title_col = _box(vertical=True, spacing=3)
        title_col.set_hexpand(True)
        title_col.append(_label("水冷控制", "page-title"))
        title_col.append(_label("MECHREVO 外置蓝牙水冷 (LCT22002 / LCT21001) · 非配对直连 · 默认不主动改写硬件", "subtitle", wrap=True))
        title_row.append(title_col)
        self.status_pill = _label("未连接", "connection-pill pill-muted")
        self.status_pill.set_valign(Gtk.Align.CENTER)
        title_row.append(self.status_pill)
        root.append(title_row)

        # --- Hero Telemetry Deck (Metrics Left + Dual-Zone Visual Preview Right) ---
        hero = _box(vertical=False, spacing=18, css="hero-deck")
        hero_left = _box(vertical=True, spacing=12)
        hero_left.set_hexpand(True)

        self.device_label = _label("等待连接水冷设备", "section-title", wrap=True)
        hero_left.append(self.device_label)

        kpi_grid = Gtk.Grid(column_spacing=10, row_spacing=10, column_homogeneous=True)
        self._flow_kpi, flow_tile = self._make_kpi("水流回路", "待机")
        self._pump_kpi, pump_tile = self._make_kpi("水泵输出", "0V · 0%")
        self._fan_kpi, fan_tile = self._make_kpi("冷排风扇", "0%")
        self._temp_kpi, temp_tile = self._make_kpi("CPU 封装", "--°C")
        kpi_grid.attach(flow_tile, 0, 0, 1, 1)
        kpi_grid.attach(pump_tile, 1, 0, 1, 1)
        kpi_grid.attach(fan_tile, 2, 0, 1, 1)
        kpi_grid.attach(temp_tile, 3, 0, 1, 1)
        hero_left.append(kpi_grid)

        telemetry_well = _box(vertical=True, spacing=4, css="well")
        self.flow_label = _label("水流: 未知 · 输出: 泵 0V/0% · 风扇 0% · 主机温度: --°C", "small-note", wrap=True)
        self.ec_label = _label("EC 遥测: 等待守护数据", "small-note", wrap=True)
        telemetry_well.append(self.flow_label)
        telemetry_well.append(self.ec_label)
        hero_left.append(telemetry_well)

        self.fault_label = _label("", "small-note", wrap=True)
        self.fault_label.set_visible(False)
        hero_left.append(self.fault_label)

        btn_row = _box(vertical=False, spacing=8)
        self.connect_btn = Gtk.Button(label="连接水冷")
        self.connect_btn.add_css_class("suggested-action")
        self.scan_btn = Gtk.Button(label="扫描设备")
        self.disconnect_btn = Gtk.Button(label="安全断开")
        self.refresh_btn = Gtk.Button(label="刷新")
        self.reset_fault_btn = Gtk.Button(label="重置故障")
        self.estop_btn = Gtk.Button(label="紧急停泵")
        self.estop_btn.add_css_class("destructive-action")
        for b in (self.connect_btn, self.scan_btn, self.disconnect_btn, self.refresh_btn, self.reset_fault_btn, self.estop_btn):
            btn_row.append(b)
        hero_left.append(btn_row)

        self.scan_btn.connect("clicked", lambda *_: self._dispatch({"op": "scan", "timeout_s": 4.0}, self._on_scan_done))
        self.connect_btn.connect("clicked", lambda *_: self._dispatch({"op": "connect", "mac": self.mac_entry.get_text().strip()}, self._on_status_done))
        self.disconnect_btn.connect("clicked", lambda *_: self._dispatch({"op": "disconnect"}, self._on_status_done))
        self.reset_fault_btn.connect("clicked", lambda *_: self._dispatch({"op": "reset_fault"}, self._on_status_done))
        self.estop_btn.connect("clicked", lambda *_: self._dispatch({"op": "emergency_stop"}, self._on_status_done))
        self.refresh_btn.connect("clicked", lambda *_: self._dispatch({"op": "status"}, self._on_status_done))

        hero.append(hero_left)
        self.led_preview = CoolerHardwarePreview(self)
        hero.append(self.led_preview)
        root.append(hero)

        # --- Lower Asymmetric 2-Column Workspace ---
        columns = _box(vertical=False, spacing=16)

        # Left Column: Thermal Strategy & Curve
        cool_card = _card("水泵与风扇散热策略", "按预设档位、固定电压、温控曲线或 EC 遥测协同运行")
        cool_card.set_hexpand(True)
        self.strategy_dd = _dropdown(STRATEGY_ITEMS)
        self.strategy_dd.connect("notify::selected", lambda *_: self._update_strategy_visibility())
        cool_card.append(_row("控制策略", self.strategy_dd))

        self.preset_dd = _dropdown(PRESET_ITEMS)
        self.preset_row = _row("固定预设档位", self.preset_dd, well=True)
        cool_card.append(self.preset_row)

        self.manual_box = _box(vertical=True, spacing=8, css="well")
        self.manual_volt_dd = _dropdown(VOLT_ITEMS)
        self.manual_pump_duty = _spin(0, 100, 60)
        self.manual_fan_duty = _spin(0, 100, 40)
        self.manual_box.append(_row("水泵供电电压", self.manual_volt_dd))
        self.manual_box.append(_row("水泵占空比 (%)", self.manual_pump_duty))
        self.manual_box.append(_row("冷排风扇占空比 (%)", self.manual_fan_duty))
        cool_card.append(self.manual_box)

        self.curve_box = _box(vertical=True, spacing=8, css="well")
        self.curve_chart = CoolerCurveChart(self)
        self.curve_box.append(self.curve_chart)
        self.hyst_spin = _spin(1, 15, 3)
        self.curve_box.append(_row("降档滞回温度 (°C)", self.hyst_spin, "升温立即升档，降温需低于当前档位温度至少此滞回值才降档"))
        self.curve_points_box = _box(vertical=True, spacing=6)
        self.curve_box.append(self.curve_points_box)
        cool_card.append(self.curve_box)

        self.ec_note = _label(
            "跟随主机 EC 只读遥测 (0x7E6/0x7E7 & WMI 0xC1)；若遥测过期或不可用，自动回退至静音档 (7V/60%/40%)。",
            "small-note",
            wrap=True,
        )
        cool_card.append(self.ec_note)
        columns.append(cool_card)

        # Right Column: Dual-Zone LED + Safety & Connection Settings
        right_col = _box(vertical=True, spacing=16)
        right_col.set_hexpand(True)

        led_card = _card("双区灯光 (风扇 + 机身)", "蓝牙指示灯由固件独占；风扇区通过锁存与同步实现分控")
        self.led_mode_dd = _dropdown(LED_MODE_ITEMS)
        self.fan_policy_dd = _dropdown(FAN_POLICY_ITEMS)
        self.body_color_btn = _color_btn([0, 128, 255])
        self.fan_color_btn = _color_btn([0, 128, 255])
        self.led_mode_dd.connect("notify::selected", lambda *_: self._on_led_ui_changed())
        self.fan_policy_dd.connect("notify::selected", lambda *_: self._on_led_ui_changed())
        self.body_color_btn.connect("notify::rgba", lambda *_: self._on_led_ui_changed())
        self.fan_color_btn.connect("notify::rgba", lambda *_: self._on_led_ui_changed())
        self.fan_color_row = _row("风扇灯独立颜色 (split 模式)", self.fan_color_btn)

        led_card.append(_row("灯光效果模式", self.led_mode_dd))
        led_card.append(_row("风扇区联动策略", self.fan_policy_dd))
        led_card.append(_row("机身 / 主灯颜色", self.body_color_btn))
        led_card.append(self.fan_color_row)
        right_col.append(led_card)

        guard_card = _card("保护机制与设备绑定", "安全参数立即随配置校验保存，断开或休眠自动停泵待机")
        mac_row = _box(vertical=False, spacing=8)
        mac_row.set_valign(Gtk.Align.CENTER)
        mac_row.append(_label("目标 MAC:"))
        self.mac_entry = Gtk.Entry()
        self.mac_entry.set_placeholder_text("留空自动匹配 CoolingSystem LCT22002/LCT21001")
        self.mac_entry.set_hexpand(True)
        mac_row.append(self.mac_entry)
        guard_card.append(mac_row)

        self.auto_connect_switch = Gtk.Switch()
        self.flow_protect_switch = Gtk.Switch()
        self.sleep_disconnect_switch = Gtk.Switch()
        guard_card.append(_row("后台自动连接", self.auto_connect_switch, "默认关闭；启用后后台服务在非睡眠状态自动重连", well=True))
        guard_card.append(_row("断流 / 干转保护", self.flow_protect_switch, "水泵运行中未见水流循环超过阈值将自动停泵锁定", well=True))
        guard_card.append(_row("断开/睡眠发送待机包", self.sleep_disconnect_switch, "退出连接或休眠前停泵并发送 GoToSleepLC (0x19 00 01)", well=True))

        action_row = _box(vertical=False, spacing=10)
        action_row.set_valign(Gtk.Align.CENTER)
        self.apply_btn = Gtk.Button(label="保存并应用配置")
        self.apply_btn.add_css_class("suggested-action")
        self.apply_btn.connect("clicked", lambda *_: self._on_apply_clicked())
        self.msg_label = _label(init_err or "就绪（仅读取本地配置，未主动操作水冷硬件）", "status-text", wrap=True)
        self.msg_label.set_hexpand(True)
        action_row.append(self.apply_btn)
        action_row.append(self.msg_label)
        guard_card.append(action_row)
        right_col.append(guard_card)

        columns.append(right_col)
        root.append(columns)
        self.append(root)

        self._load_config_into_widgets(self._config)
        self.connect("map", self._on_map)
        self.connect("unmap", self._on_unmap)

    @staticmethod
    def _make_kpi(title: str, initial: str):
        tile = _box(vertical=True, spacing=3, css="metric-tile")
        tile.append(_label(title, "kpi-label"))
        val = _label(initial, "kpi-value")
        tile.append(val)
        return val, tile

    def _on_led_ui_changed(self) -> None:
        self._update_strategy_visibility()
        self.led_preview.queue_draw()

    def _rebuild_curve_rows(self, points: List[dict]) -> None:
        while child := self.curve_points_box.get_first_child():
            self.curve_points_box.remove(child)
        self._curve_rows = []
        for idx, pt in enumerate(points):
            row = _box(vertical=False, spacing=6)
            row.append(_label(f"T{idx + 1} ≥"))
            t_spin = _spin(25, 100, pt["temp_c"])
            t_spin.connect("value-changed", lambda *_: self.curve_chart.queue_draw())
            row.append(t_spin)
            row.append(_label("°C · 泵"))
            v_dd = _dropdown(VOLT_ITEMS)
            _select_key(v_dd, VOLT_ITEMS, pt["pump_volt"])
            row.append(v_dd)
            pd_spin = _spin(0, 100, pt["pump_duty"])
            pd_spin.connect("value-changed", lambda *_: self.curve_chart.queue_draw())
            row.append(pd_spin)
            row.append(_label("% · 扇"))
            fd_spin = _spin(0, 100, pt["fan_duty"])
            fd_spin.connect("value-changed", lambda *_: self.curve_chart.queue_draw())
            row.append(fd_spin)
            row.append(_label("%"))
            self.curve_points_box.append(row)
            self._curve_rows.append({
                "temp": t_spin,
                "volt": v_dd,
                "pump_duty": pd_spin,
                "fan_duty": fd_spin,
            })
        self.curve_chart.queue_draw()

    def _load_config_into_widgets(self, cfg: dict) -> None:
        self._syncing_ui = True
        try:
            self._config = wc.validate_config(cfg)
            self.mac_entry.set_text(self._config["mac"])
            self.auto_connect_switch.set_active(self._config["auto_connect"])
            self.flow_protect_switch.set_active(self._config["flow_protection"])
            self.sleep_disconnect_switch.set_active(self._config["sleep_on_disconnect"])
            _select_key(self.strategy_dd, STRATEGY_ITEMS, self._config["strategy"])
            _select_key(self.preset_dd, PRESET_ITEMS, self._config["preset"])

            man = self._config["manual"]
            _select_key(self.manual_volt_dd, VOLT_ITEMS, man["pump_volt"])
            self.manual_pump_duty.set_value(man["pump_duty"])
            self.manual_fan_duty.set_value(man["fan_duty"])

            curve = self._config["curve"]
            self.hyst_spin.set_value(curve["hysteresis_c"])
            self._rebuild_curve_rows(curve["points"])

            led = self._config["led"]
            _select_key(self.led_mode_dd, LED_MODE_ITEMS, led["mode"])
            _select_key(self.fan_policy_dd, FAN_POLICY_ITEMS, led["fan_policy"])
            self.body_color_btn.set_rgba(_rgba(led["color"]))
            self.fan_color_btn.set_rgba(_rgba(led["fan_color"]))
        finally:
            self._syncing_ui = False
        self._update_strategy_visibility()
        self.led_preview.queue_draw()

    def _update_strategy_visibility(self) -> None:
        strategy = _selected_key(self.strategy_dd, STRATEGY_ITEMS)
        self.preset_row.set_visible(strategy == "preset")
        self.manual_box.set_visible(strategy == "manual")
        self.curve_box.set_visible(strategy == "smart_curve")
        self.ec_note.set_visible(strategy == "ec_follow")
        fan_policy = _selected_key(self.fan_policy_dd, FAN_POLICY_ITEMS)
        self.fan_color_row.set_visible(fan_policy == "split")

    def build_config_from_ui(self) -> dict:
        """Read current UI widgets and validate into a canonical config dict."""
        pts = [
            {
                "temp_c": r["temp"].get_value_as_int(),
                "pump_volt": _selected_key(r["volt"], VOLT_ITEMS),
                "pump_duty": r["pump_duty"].get_value_as_int(),
                "fan_duty": r["fan_duty"].get_value_as_int(),
            }
            for r in self._curve_rows
        ]
        raw = {
            "version": 1,
            "auto_connect": bool(self.auto_connect_switch.get_active()),
            "mac": self.mac_entry.get_text().strip(),
            "strategy": _selected_key(self.strategy_dd, STRATEGY_ITEMS),
            "preset": _selected_key(self.preset_dd, PRESET_ITEMS),
            "manual": {
                "pump_volt": _selected_key(self.manual_volt_dd, VOLT_ITEMS),
                "pump_duty": self.manual_pump_duty.get_value_as_int(),
                "fan_duty": self.manual_fan_duty.get_value_as_int(),
            },
            "curve": {
                "hysteresis_c": self.hyst_spin.get_value_as_int(),
                "points": pts,
            },
            "led": {
                "mode": _selected_key(self.led_mode_dd, LED_MODE_ITEMS),
                "color": _rgb_from_btn(self.body_color_btn),
                "fan_policy": _selected_key(self.fan_policy_dd, FAN_POLICY_ITEMS),
                "fan_color": _rgb_from_btn(self.fan_color_btn),
            },
            "flow_protection": bool(self.flow_protect_switch.get_active()),
            "sleep_on_disconnect": bool(self.sleep_disconnect_switch.get_active()),
        }
        return wc.validate_config(raw)

    def _set_pill_style(self, text: str, variant: str) -> None:
        self.status_pill.set_text(text)
        for cls in ("pill-warn", "pill-danger", "pill-muted"):
            self.status_pill.remove_css_class(cls)
        if variant:
            self.status_pill.add_css_class(variant)

    def apply_snapshot(self, payload: dict, *, sync_config: bool = True) -> None:
        """Update status, flow, EC/WMI telemetry and optionally config widgets from service state."""
        if not isinstance(payload, dict):
            return
        self._last_snapshot = dict(payload)
        connected = bool(payload.get("connected", False))
        sleeping = bool(payload.get("sleeping", False))
        tripped = bool(payload.get("flow_tripped", False))
        if sleeping:
            self._set_pill_style("系统睡眠暂停", "pill-warn")
        elif tripped:
            self._set_pill_style("断流锁定", "pill-danger")
        elif connected:
            self._set_pill_style("已连接", "")
        else:
            self._set_pill_style("未连接", "pill-muted")

        dev_name = payload.get("device_name") or "待连接外置水冷"
        dev_mac = payload.get("device_mac") or "--"
        fw = payload.get("fw_version") or "--"
        reason = payload.get("reason") or "idle"
        self.device_label.set_text(f"{dev_name} ({dev_mac}) · 固件 {fw} · {reason}")

        meter = payload.get("meter_state")
        meter_short = "正常循环" if meter == 2 else (f"无水流 ({meter})" if meter is not None else "未知")
        meter_text = "正常循环(2)" if meter == 2 else (f"无水流({meter})" if meter is not None else "未知")
        app = payload.get("applied") or {}
        pv, pd, fd = app.get("pump_volt", 0), app.get("pump_duty", 0), app.get("fan_duty", 0)
        temp_c = payload.get("temp_c")
        temp_text = f"{temp_c:.1f}°C" if isinstance(temp_c, (int, float)) else "--°C"

        self._flow_kpi.set_text(meter_short)
        self._pump_kpi.set_text(f"{pv}V · {pd}%")
        self._fan_kpi.set_text(f"{fd}%")
        self._temp_kpi.set_text(temp_text)

        self.flow_label.set_text(
            f"水流: {meter_text} · 输出: 泵 {pv}V/{pd}% · 风扇 {fd}% · 主机温度: {temp_text}"
        )

        ec = payload.get("ec") or {}
        if ec.get("ec_available"):
            stale = "过期" if ec.get("ec_stale") else "实时"
            sup_txt = "已支持" if ec.get("lc_supported") else "未确认"
            pump_txt = f"{ec.get('ec_pump_volt')}V" if ec.get("ec_pump_enabled") else "关闭"
            fan_txt = f"{ec.get('ec_fan_duty')}%" if ec.get("ec_fan_enabled") else "关闭"
            self.ec_label.set_text(
                f"EC 遥测({stale}): {sup_txt} · 泵 {pump_txt} · 扇 {fan_txt} · WMI C1 seq={ec.get('wmi_c1_seq')}"
            )
        else:
            wmi_txt = "就绪" if ec.get("wmi_available") else "无信号"
            self.ec_label.set_text(f"EC 遥测: 等待守护数据 · WMI {wmi_txt} (C1 seq={ec.get('wmi_c1_seq')})")

        err = payload.get("flow_trip_reason") or payload.get("last_error") or ""
        if tripped or err:
            self.fault_label.set_text(f"安全告警: {err}")
            self.fault_label.set_visible(True)
        else:
            self.fault_label.set_text("")
            self.fault_label.set_visible(False)

        if sync_config and "config" in payload and not self._busy:
            self._load_config_into_widgets(payload["config"])
        self.led_preview.queue_draw()

    def _on_apply_clicked(self) -> None:
        try:
            cfg = self.build_config_from_ui()
        except Exception as exc:
            self.msg_label.set_text(f"参数无效: {exc}")
            return
        self._config = cfg
        self._dispatch(
            {"op": "set_config", "config": cfg, "apply_now": True},
            self._on_apply_done,
            fallback_cfg=cfg,
        )

    def _on_apply_done(self, ok: bool, res, cfg: Optional[dict] = None) -> None:
        if ok:
            self.apply_snapshot(res)
            self.msg_label.set_text("配置已保存并提交至水冷后台服务")
            return
        msg = str(res)
        if cfg is not None and ("No such file" in msg or "Connection refused" in msg or "Socket" in msg or isinstance(res, FileNotFoundError)):
            try:
                saved = wc.save_config(cfg, self._config_path)
                self._config = saved
                self.msg_label.set_text("后台服务未运行，已保存到本地配置文件")
                return
            except Exception as save_exc:
                msg = str(save_exc)
        self.msg_label.set_text(f"保存失败: {msg}")

    def _on_status_done(self, ok: bool, res) -> None:
        if ok:
            self.apply_snapshot(res)
            self.msg_label.set_text("状态已同步")
        else:
            self._set_pill_style("服务未启动", "pill-muted")
            self.msg_label.set_text(f"读取后台服务失败: {res}")

    def _on_scan_done(self, ok: bool, res) -> None:
        if not ok:
            self.msg_label.set_text(f"扫描失败: {res}")
            return
        devs = res.get("devices", []) if isinstance(res, dict) else []
        if not devs:
            self.msg_label.set_text("未发现周边 MECHREVO 蓝牙水冷设备")
            return
        first = devs[0]
        if not self.mac_entry.get_text().strip() and first.get("address"):
            self.mac_entry.set_text(first["address"])
        summary = ", ".join(f"{d.get('name')} ({d.get('address')}, RSSI={d.get('rssi')})" for d in devs)
        self.msg_label.set_text(f"发现 {len(devs)} 台水冷: {summary}")

    def _dispatch(self, req: dict, done_cb, *, fallback_cfg: Optional[dict] = None) -> None:
        if not self._op_lock.acquire(blocking=False):
            return
        self._busy = True
        self.msg_label.set_text(f"正在执行 {req.get('op')}...")

        def worker():
            try:
                out = self._request_fn(req)
                ok = True
            except Exception as exc:
                out = exc
                ok = False

            def finish():
                self._busy = False
                self._op_lock.release()
                if self._closed:
                    return False
                if fallback_cfg is not None:
                    done_cb(ok, out, fallback_cfg)
                else:
                    done_cb(ok, out)
                return False

            GLib.idle_add(finish)

        threading.Thread(target=worker, daemon=True).start()

    def _on_poll_tick(self) -> bool:
        if self._closed:
            self._poll_id = None
            return False
        if not self._busy:
            self._dispatch({"op": "status"}, lambda ok, res: self.apply_snapshot(res, sync_config=False) if ok else None)
        return True

    def _on_map(self, *_args) -> None:
        if self._closed:
            return
        if self._poll_id is None and self._poll_interval_ms > 0:
            self._poll_id = GLib.timeout_add(self._poll_interval_ms, self._on_poll_tick)
        self._dispatch({"op": "status"}, self._on_status_done)

    def _on_unmap(self, *_args) -> None:
        if self._poll_id is not None:
            GLib.source_remove(self._poll_id)
            self._poll_id = None

    def close(self) -> None:
        """Stop polling timers and ignore any late worker callbacks."""
        self._closed = True
        self._on_unmap()

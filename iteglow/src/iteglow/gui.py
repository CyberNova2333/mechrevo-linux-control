"""GTK4 control panel. Optional front-end; the CLI/library has no GUI deps.

Uses the system PyGObject (python3-gi + gir1.2-gtk-4.0). One tab per detected
device; missing devices simply don't appear, which makes the keyboard optional.

Panels never touch the hardware until the user changes a control. On startup
the keyboard panel is initialised from the device's actual state (the ITE
keyboard firmware supports effect readback via QUERY_EFFECT; the lightbar does
not, so its controls just show defaults). All construction happens before any
signal is connected, so no callback can run against a half-built panel.
"""
from __future__ import annotations

import gi

gi.require_version("Gtk", "4.0")
gi.require_version("Gdk", "4.0")
from gi.repository import Gdk, Gio, GLib, Gtk  # noqa: E402

from . import devices, effects, protocol, state  # noqa: E402


def _rgba_to_rgb(rgba):
    return (round(rgba.red * 255), round(rgba.green * 255), round(rgba.blue * 255))


def _make_rgba(rgb):
    r, g, b = rgb
    c = Gdk.RGBA()
    c.red, c.green, c.blue, c.alpha = r / 255, g / 255, b / 255, 1.0
    return c


def _make_color_button(rgb):
    try:
        btn = Gtk.ColorDialogButton.new(Gtk.ColorDialog())
        btn.set_rgba(_make_rgba(rgb))
    except AttributeError:  # GTK < 4.10
        btn = Gtk.ColorButton.new_with_rgba(_make_rgba(rgb))
    return btn


def _connect_color(btn, cb):
    try:
        btn.connect("notify::rgba", lambda *_: cb())
    except Exception:  # noqa: BLE001
        btn.connect("color-set", lambda *_: cb())


# direction options shown per device (label -> byte)
_DIR_LABELS = {
    "lightbar": [("Forward", 1), ("Reverse", 2)],
    "keyboard": [("Left → Right", 1), ("Right → Left", 2),
                 ("Bottom → Top", 3), ("Top → Bottom", 4)],
}


class DevicePanel(Gtk.Box):
    def __init__(self, label: str):
        super().__init__(orientation=Gtk.Orientation.VERTICAL, spacing=12)
        for fn in ("set_margin_top", "set_margin_bottom", "set_margin_start", "set_margin_end"):
            getattr(self, fn)(16)
        self.label = label
        self.dev = devices.HidLed(devices.led_for_label(label))
        self.modes = effects.MODES_BY_LABEL[label]
        self.mode_names = list(self.modes)
        self.bmax = protocol.LB_BRIGHTNESS_MAX if label == "lightbar" else protocol.KBD_BRIGHTNESS_MAX
        self.dirs = _DIR_LABELS[label]
        self._pending = False
        self._ready = False   # no hardware writes until construction is done

        # ---- build every widget first; signals are connected at the end ----
        grid = Gtk.Grid(row_spacing=10, column_spacing=12)
        self.append(grid)
        r = 0

        grid.attach(Gtk.Label(label="灯效", xalign=0), 0, r, 1, 1)
        self.mode_dd = Gtk.DropDown.new_from_strings(self.mode_names)
        self.mode_dd.set_hexpand(True)
        grid.attach(self.mode_dd, 1, r, 1, 1); r += 1

        grid.attach(Gtk.Label(label="亮度", xalign=0), 0, r, 1, 1)
        self.bright = Gtk.Scale.new_with_range(Gtk.Orientation.HORIZONTAL, 0, self.bmax, 1)
        self.bright.set_hexpand(True)
        self.bright.set_draw_value(True)
        grid.attach(self.bright, 1, r, 1, 1); r += 1

        self.speed_lbl = Gtk.Label(label="速度（1 最快）", xalign=0)
        grid.attach(self.speed_lbl, 0, r, 1, 1)
        self.speed = Gtk.Scale.new_with_range(Gtk.Orientation.HORIZONTAL, 1, 10, 1)
        self.speed.set_hexpand(True); self.speed.set_draw_value(True)
        grid.attach(self.speed, 1, r, 1, 1); r += 1

        self.dir_lbl = Gtk.Label(label="方向", xalign=0)
        grid.attach(self.dir_lbl, 0, r, 1, 1)
        self.dir_dd = Gtk.DropDown.new_from_strings([d[0] for d in self.dirs])
        grid.attach(self.dir_dd, 1, r, 1, 1); r += 1

        self.color_lbl = Gtk.Label(label="颜色", xalign=0)
        grid.attach(self.color_lbl, 0, r, 1, 1)
        self.color_btn = _make_color_button((0, 255, 255))
        grid.attach(self.color_btn, 1, r, 1, 1); r += 1

        self.palette_lbl = Gtk.Label(label="调色板", xalign=0)
        grid.attach(self.palette_lbl, 0, r, 1, 1)
        self.palette_box = Gtk.Box(orientation=Gtk.Orientation.HORIZONTAL, spacing=4)
        self.palette_btns = []
        for c in effects.RAINBOW:
            b = _make_color_button(c)
            self.palette_box.append(b); self.palette_btns.append(b)
        reset = Gtk.Button(label="彩虹")
        self.palette_box.append(reset)
        grid.attach(self.palette_box, 1, r, 1, 1); r += 1

        self.status = Gtk.Label(xalign=0); self.status.add_css_class("dim-label")
        self.append(self.status)

        # ---- initial values: defaults, then the device's actual state ----
        if "off" in self.mode_names and len(self.mode_names) > 1:
            self.mode_dd.set_selected(1)          # first non-off mode
        self.bright.set_value(self.bmax * 0.6)
        self.speed.set_value(5)
        self._init_from_saved()
        self._init_from_device()
        self._update_visibility()

        # ---- connect signals last, so nothing above could fire them ----
        self.mode_dd.connect("notify::selected", lambda *_: self.schedule())
        self.bright.connect("value-changed", lambda *_: self.schedule())
        self.speed.connect("value-changed", lambda *_: self.schedule())
        self.dir_dd.connect("notify::selected", lambda *_: self.schedule())
        _connect_color(self.color_btn, self.schedule)
        for b in self.palette_btns:
            _connect_color(b, self.schedule)
        reset.connect("clicked", self._reset_rainbow)
        self._ready = True

    def _init_from_saved(self):
        """Prefill controls from the last state this user applied. This is
        the only source for colours and the palette (the firmware cannot
        report them back) and for everything on the lightbar."""
        saved = state.load(state.user_state_path()).get(self.label) or {}
        mode = saved.get("mode")
        if mode in self.mode_names:
            self.mode_dd.set_selected(self.mode_names.index(mode))
        if saved.get("brightness") is not None:
            self.bright.set_value(min(saved["brightness"], self.bmax))
        if saved.get("speed"):
            self.speed.set_value(saved["speed"])
        for i, (_name, byte) in enumerate(self.dirs):
            if byte == saved.get("direction"):
                self.dir_dd.set_selected(i)
        if saved.get("color"):
            self.color_btn.set_rgba(_make_rgba(saved["color"]))
        for btn, c in zip(self.palette_btns, saved.get("colors") or []):
            btn.set_rgba(_make_rgba(c))

    def _init_from_device(self):
        """Reflect the device's current state (keyboard only; the lightbar
        firmware does not report its state back)."""
        if self.label != "keyboard":
            return
        try:
            st = protocol.parse_keyboard_state(self.dev.query(protocol.QUERY_EFFECT))
        except OSError:
            return
        if st["power"] == 0x01 or st["brightness"] == 0:
            self.mode_dd.set_selected(self.mode_names.index("off"))
        else:
            for i, name in enumerate(self.mode_names):
                if self.modes[name].get("code") == st["mode"] and not self.modes[name].get("off"):
                    self.mode_dd.set_selected(i)
                    break
        self.bright.set_value(min(st["brightness"], self.bmax))
        if protocol.SPEED_MIN <= st["speed"] <= protocol.SPEED_MAX:
            self.speed.set_value(st["speed"])
        for i, (_name, byte) in enumerate(self.dirs):
            if byte == st.get("direction"):
                self.dir_dd.set_selected(i)

    def _reset_rainbow(self, *_):
        for b, c in zip(self.palette_btns, effects.RAINBOW):
            b.set_rgba(_make_rgba(c))
        self.schedule()

    def _spec(self):
        return self.modes[self.mode_names[self.mode_dd.get_selected()]]

    def _update_visibility(self):
        spec = self._spec()
        for w in (self.speed_lbl, self.speed):
            w.set_visible(bool(spec.get("speed")))
        for w in (self.dir_lbl, self.dir_dd):
            w.set_visible(bool(spec.get("direction")))
        for w in (self.color_lbl, self.color_btn):
            w.set_visible(bool(spec.get("single")))
        for w in (self.palette_lbl, self.palette_box):
            w.set_visible(bool(spec.get("palette")))

    def schedule(self, *_):
        if not self._ready:
            return
        self._update_visibility()
        if self._pending:
            return
        self._pending = True
        GLib.timeout_add(50, self._do_apply)

    def _do_apply(self):
        self._pending = False
        try:
            self.apply()
            self.status.set_text("")
        except Exception as e:  # noqa: BLE001
            self.status.set_text(f"error: {e}")
        return False

    def apply(self):
        params = dict(
            mode=self.mode_names[self.mode_dd.get_selected()],
            color=_rgba_to_rgb(self.color_btn.get_rgba()),
            colors=[_rgba_to_rgb(b.get_rgba()) for b in self.palette_btns],
            brightness=int(self.bright.get_value()),
            speed=int(self.speed.get_value()),
            direction=self.dirs[self.dir_dd.get_selected()][1],
        )
        mode = params.pop("mode")
        self.dev.send(effects.build(self.label, mode, **params))
        state.save(self.label, dict(mode=mode, **params))


class App(Gtk.Application):
    def __init__(self):
        super().__init__(application_id="org.iteglow.Control")

    def do_activate(self):
        win = self.get_active_window()
        if win is not None:       # re-launched: raise the existing window
            win.present()
            return
        win = Gtk.ApplicationWindow(application=self, title="机械革命灯光 · iteglow")
        win.set_default_size(580, 440)
        present = devices.available_devices()
        if not present:
            win.set_child(Gtk.Label(
                label="No supported devices found.\nRun `iteglow doctor` in a terminal."))
            win.present()
            return
        nb = Gtk.Notebook()
        nb.set_vexpand(True)
        self.panels = []
        for _led, _usb, label in present:
            try:
                page = DevicePanel(label)
                self.panels.append(page)
            except Exception as e:  # noqa: BLE001
                page = Gtk.Label(label=f"{label} unavailable:\n{e}")
            nb.append_page(page, Gtk.Label(label={"keyboard": "键盘灯", "lightbar": "机身灯带"}[label]))
        outer = Gtk.Box(orientation=Gtk.Orientation.VERTICAL)
        outer.append(nb)
        outer.append(self._make_pin_bar())
        win.set_child(outer)
        win.present()

    def _make_pin_bar(self):
        bar = Gtk.Box(orientation=Gtk.Orientation.HORIZONTAL, spacing=8)
        for fn in ("set_margin_top", "set_margin_bottom", "set_margin_start", "set_margin_end"):
            getattr(bar, fn)(10)
        hint = Gtk.Label(label="调整后立即生效；灯带显示上次保存值", xalign=0, hexpand=True)
        hint.add_css_class("dim-label")
        bar.append(hint)
        button = Gtk.Button(label="关闭全部灯光")
        button.connect("clicked", self._all_off)
        bar.append(button)
        return bar

    def _all_off(self, _button):
        for panel in self.panels:
            panel.mode_dd.set_selected(panel.mode_names.index("off"))
            panel.bright.set_value(0)
            panel._do_apply()


def main() -> int:
    return App().run(None)


if __name__ == "__main__":
    raise SystemExit(main())

# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
"""Small policy settings card. Saves preferences; never applies device writes."""
import json
import os
from pathlib import Path
import tempfile

from gi.repository import Gtk

from .lighting_policy import DEVICES, default_config, validate_config

CONFIG_PATH = Path.home() / ".config/iteglow/lighting-policy.json"


def load_config(path):
    try:
        with Path(path).open("rb") as stream:
            raw = stream.read(65537)
    except FileNotFoundError:
        return default_config()
    if len(raw) > 65536:
        raise ValueError("灯光策略配置超过大小限制")
    return validate_config(json.loads(raw))


def save_config(path, config):
    """Atomic user preference write; validation failure leaves the old file."""
    config = validate_config(config)
    path = Path(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    temporary = None
    try:
        with tempfile.NamedTemporaryFile(mode="w", encoding="utf-8", dir=path.parent,
                                         prefix=".lighting-policy-", delete=False) as stream:
            temporary = Path(stream.name)
            json.dump(config, stream, ensure_ascii=False, indent=2)
            stream.write("\n")
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(temporary, path)
    finally:
        if temporary is not None:
            temporary.unlink(missing_ok=True)
    return config


class LightingPolicyPanel(Gtk.Box):
    """Embed beside existing lighting controls; on_saved receives valid config.

    Constructing/editing never saves. Saving only writes this preference file;
    the session watcher owns eligibility checks, policy planning and execution.
    """

    def __init__(self, path=None, on_saved=None):
        super().__init__(orientation=Gtk.Orientation.VERTICAL, spacing=12)
        self.add_css_class("control-card")
        self.path = Path(path) if path is not None else CONFIG_PATH
        self.on_saved = on_saved
        row = Gtk.Box(spacing=12)
        row.add_css_class("well")
        title = Gtk.Label(label="自动灯光", xalign=0)
        title.add_css_class("section-title"); title.set_hexpand(True); row.append(title)
        self.enabled = Gtk.Switch(); self.enabled.set_valign(Gtk.Align.CENTER)
        self.enabled.set_tooltip_text("启用自动灯光策略")
        row.append(self.enabled); self.append(row)
        note = Gtk.Label(label="按供电方式设置亮度，空闲时自动熄灯；键鼠恢复活动后还原灯效。", xalign=0, wrap=True)
        note.add_css_class("small-note"); self.append(note)
        self.power_enabled = Gtk.CheckButton(label="分别设置插电与电池亮度")
        self.append(self.power_enabled)
        self.grid = Gtk.Grid(column_spacing=18, row_spacing=10)
        self.grid.add_css_class("well")
        for column, caption in enumerate(("灯区", "插电", "电池")):
            label = Gtk.Label(label=caption, xalign=0)
            label.add_css_class("small-note"); self.grid.attach(label, column, 0, 1, 1)
        self.brightness = {}
        for row_index, (device, caption) in enumerate(zip(DEVICES, ("键盘背光", "机身灯带", "后盖 Logo")), 1):
            label = Gtk.Label(label=caption, xalign=0); label.set_hexpand(True)
            self.grid.attach(label, 0, row_index, 1, 1)
            self.brightness[device] = {}
            for column, source in enumerate(("ac", "dc"), 1):
                box = Gtk.Box(spacing=6)
                if device == "logo":
                    widget = Gtk.DropDown.new_from_strings(["0", "25", "50", "75", "100"])
                else:
                    widget = Gtk.SpinButton.new_with_range(0, 100, 1); widget.set_numeric(True)
                widget.set_tooltip_text(caption + ("插电亮度" if source == "ac" else "电池亮度"))
                self.brightness[device][source] = widget
                box.append(widget); box.append(Gtk.Label(label="%"))
                self.grid.attach(box, column, row_index, 1, 1)
        self.append(self.grid)
        row = Gtk.Box(spacing=12)
        label = Gtk.Label(label="空闲后熄灯", xalign=0); label.set_hexpand(True); row.append(label)
        self.idle_minutes = Gtk.SpinButton.new_with_range(0, 1440, 1)
        self.idle_minutes.set_numeric(True); row.append(self.idle_minutes)
        row.append(Gtk.Label(label="分钟（0 = 不熄灯）")); self.append(row)
        note = Gtk.Label(label="保留现有灯效，手动关闭的灯区不会自动点亮。自动策略的临时亮度不会覆盖手动设置。", xalign=0, wrap=True)
        note.add_css_class("small-note"); self.append(note)
        actions = Gtk.Box(spacing=12)
        self.save_button = Gtk.Button(label="保存自动灯光设置")
        self.save_button.add_css_class("suggested-action"); self.save_button.connect("clicked", self.save)
        actions.append(self.save_button); self.append(actions)
        self.message = Gtk.Label(xalign=0, wrap=True); self.message.add_css_class("status-text")
        self.append(self.message)
        self.enabled.connect("notify::active", self.refresh)
        self.power_enabled.connect("toggled", self.refresh)
        try:
            config = load_config(self.path)
        except (OSError, ValueError, TypeError) as error:
            config = default_config()
            self.message.set_text("现有配置无法读取；尚未改动文件：" + str(error))
        else:
            self.message.set_text("自动策略默认关闭；编辑后保存生效。" if not config["enabled"] else "已载入自动灯光设置。")
        self.load_values(config)

    def load_values(self, config):
        config = validate_config(config)
        self.enabled.set_active(config["enabled"])
        self.power_enabled.set_active(config["power_enabled"])
        self.idle_minutes.set_value(config["idle_minutes"])
        for device in DEVICES:
            for source, widget in self.brightness[device].items():
                value = config["brightness"][device][source]
                if device == "logo": widget.set_selected(value // 25)
                else: widget.set_value(value)
        self.refresh()

    def refresh(self, *_):
        enabled = self.enabled.get_active()
        self.power_enabled.set_sensitive(enabled)
        self.grid.set_sensitive(enabled and self.power_enabled.get_active())
        self.idle_minutes.set_sensitive(enabled)

    def config(self):
        brightness = {device: {source: widget.get_selected() * 25 if device == "logo"
                              else widget.get_value_as_int()
                              for source, widget in self.brightness[device].items()}
                      for device in DEVICES}
        return validate_config({"version": 1, "enabled": self.enabled.get_active(),
                                "power_enabled": self.power_enabled.get_active(),
                                "idle_minutes": self.idle_minutes.get_value_as_int(),
                                "brightness": brightness})

    def save(self, *_):
        try:
            config = save_config(self.path, self.config())
        except (OSError, ValueError, TypeError) as error:
            self.message.set_text("保存失败：" + str(error)); return
        self.message.set_text("已保存；自动策略已启用。" if config["enabled"] else "已保存；自动策略已关闭。")
        if self.on_saved:
            try: self.on_saved(config)
            except Exception as error:
                self.message.set_text("配置已保存，但通知会话服务失败：" + str(error))

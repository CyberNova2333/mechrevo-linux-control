"""GTK4 control panel for performance+lighting profile bundles and app rules.

Constructing or editing this panel never alters hardware or writes files;
only explicit Save/Apply clicks trigger actions.
"""
from __future__ import annotations

import copy
from pathlib import Path
import threading
from typing import Callable, Dict, List, Optional

from gi.repository import GLib, Gtk

from . import profiles


def _summarize_profile(body: dict) -> str:
    parts: List[str] = []
    perf = body.get("performance")
    if perf:
        mode = perf.get("mode", "")
        if perf.get("turbo"):
            mode = f"{mode}/{perf['turbo']}"
        elif perf.get("tuning"):
            mode = f"{mode} ({perf['tuning']['pl1']}W)"
        parts.append(f"性能: {mode}")
    light = body.get("lighting")
    if light:
        zones = []
        for z in ("keyboard", "lightbar", "logo"):
            if z in light:
                zmode = light[z].get("mode", "off")
                if z == "keyboard" and zmode == "matrix" and light[z].get("preset"):
                    zmode = f"matrix:{light[z]['preset']}"
                zones.append(f"{z}={zmode}")
        if zones:
            parts.append("灯光: " + ", ".join(zones))
    if body.get("label"):
        parts.append(body["label"])
    return " · ".join(parts) if parts else "空预设"


class ProfilesPanel(Gtk.Box):
    """Profile bundles + safe opt-in process auto-switching controls."""

    def __init__(
        self,
        path: Optional[Path] = None,
        *,
        on_saved: Optional[Callable[[dict], None]] = None,
        capture_fn: Optional[Callable[..., dict]] = None,
        apply_fn: Optional[Callable[..., dict]] = None,
    ):
        super().__init__(orientation=Gtk.Orientation.VERTICAL, spacing=14)
        self.add_css_class("device-page")
        self.path = Path(path) if path is not None else profiles.profiles_path()
        self.on_saved = on_saved
        self._capture_fn = capture_fn or profiles.capture_current_profile
        self._apply_fn = apply_fn or profiles.apply_profile
        self._closed = False
        self._busy = False
        self._profiles: Dict[str, dict] = {}
        self._rules: List[dict] = []
        self._profile_names: List[str] = []
        self._suppress_refresh = False

        header = Gtk.Label(label="情景预设与联动", xalign=0)
        header.add_css_class("page-title")
        self.append(header)
        sub = Gtk.Label(
            label="组合保存性能档位与三区灯光，并可按当前用户应用进程自动切换（默认关闭）。",
            xalign=0,
            wrap=True,
        )
        sub.add_css_class("subtitle")
        self.append(sub)

        self._build_profiles_card()
        self._build_rules_card()

        self.message = Gtk.Label(xalign=0, wrap=True)
        self.message.add_css_class("status-text")
        self.append(self.message)

        try:
            cfg = profiles.load_config(self.path)
        except (OSError, ValueError, TypeError) as exc:
            cfg = profiles.default_config()
            self.load_values(cfg)
            self.message.set_text("现有配置无法读取；尚未改动文件：" + str(exc))
        else:
            self.load_values(cfg)
            self.message.set_text(
                "应用自动联动已启用。" if cfg["auto_switch_enabled"] else "自动联动默认关闭；预设仅在点击应用或匹配启用规则时生效。"
            )

    def _build_profiles_card(self) -> None:
        card = Gtk.Box(orientation=Gtk.Orientation.VERTICAL, spacing=12)
        card.add_css_class("control-card")
        title = Gtk.Label(label="预设组合（性能 + 灯光）", xalign=0)
        title.add_css_class("section-title")
        card.append(title)

        select_row = Gtk.Box(spacing=10)
        select_row.append(Gtk.Label(label="选择预设", xalign=0))
        self.profile_dropdown = Gtk.DropDown.new_from_strings(["（暂无预设）"])
        self.profile_dropdown.set_hexpand(True)
        self.profile_dropdown.connect("notify::selected", self._on_profile_selected)
        select_row.append(self.profile_dropdown)
        card.append(select_row)

        self.profile_summary = Gtk.Label(label="尚未创建预设，可导入内置模板或从当前状态保存。", xalign=0, wrap=True)
        self.profile_summary.add_css_class("small-note")
        card.append(self.profile_summary)

        btn_row = Gtk.Box(spacing=10)
        self.apply_button = Gtk.Button(label="立即应用预设")
        self.apply_button.add_css_class("suggested-action")
        self.apply_button.connect("clicked", self.apply_selected)
        btn_row.append(self.apply_button)
        self.template_button = Gtk.Button(label="导入内置模板")
        self.template_button.connect("clicked", self.import_templates)
        btn_row.append(self.template_button)
        self.delete_button = Gtk.Button(label="删除预设")
        self.delete_button.add_css_class("destructive-action")
        self.delete_button.connect("clicked", self.delete_selected)
        btn_row.append(self.delete_button)
        card.append(btn_row)

        card.append(Gtk.Separator(orientation=Gtk.Orientation.HORIZONTAL))

        cap_label = Gtk.Label(label="从当前硬件状态捕获并保存为预设", xalign=0)
        cap_label.add_css_class("small-note")
        card.append(cap_label)

        entry_row = Gtk.Box(spacing=10)
        self.capture_name = Gtk.Entry()
        self.capture_name.set_placeholder_text("预设名称（如 coding / fps）")
        self.capture_name.set_hexpand(True)
        self.capture_label = Gtk.Entry()
        self.capture_label.set_placeholder_text("备注说明（可选）")
        self.capture_label.set_hexpand(True)
        entry_row.append(self.capture_name)
        entry_row.append(self.capture_label)
        card.append(entry_row)

        opts_row = Gtk.Box(spacing=14)
        self.capture_perf = Gtk.CheckButton(label="包含当前性能档位")
        self.capture_perf.set_active(True)
        self.capture_light = Gtk.CheckButton(label="包含当前三区灯光")
        self.capture_light.set_active(True)
        self.capture_button = Gtk.Button(label="保存当前状态为预设")
        self.capture_button.connect("clicked", self.capture_current)
        opts_row.append(self.capture_perf)
        opts_row.append(self.capture_light)
        opts_row.append(self.capture_button)
        card.append(opts_row)
        self.append(card)

    def _build_rules_card(self) -> None:
        card = Gtk.Box(orientation=Gtk.Orientation.VERTICAL, spacing=12)
        card.add_css_class("control-card")

        top = Gtk.Box(spacing=12)
        title = Gtk.Label(label="应用联动自动切换", xalign=0)
        title.add_css_class("section-title")
        title.set_hexpand(True)
        top.append(title)
        self.auto_enabled = Gtk.Switch()
        self.auto_enabled.set_valign(Gtk.Align.CENTER)
        self.auto_enabled.connect("notify::active", self._refresh_sensitivity)
        top.append(self.auto_enabled)
        card.append(top)

        safety_note = Gtk.Label(
            label="按下机身物理模式键 (0xB0) 时立即让步，不再覆盖当前档位；EC 高温风扇安全保护与圆口电源策略始终优先。",
            xalign=0,
            wrap=True,
        )
        safety_note.add_css_class("small-note")
        card.append(safety_note)

        cfg_row = Gtk.Box(spacing=14)
        self.restore_previous = Gtk.CheckButton(label="应用退出后恢复先前状态")
        self.restore_previous.set_active(True)
        cfg_row.append(self.restore_previous)
        cfg_row.append(Gtk.Label(label="退出防抖"))
        self.cooldown_spin = Gtk.SpinButton.new_with_range(0, 120, 1)
        self.cooldown_spin.set_numeric(True)
        self.cooldown_spin.set_value(5)
        cfg_row.append(self.cooldown_spin)
        cfg_row.append(Gtk.Label(label="秒"))
        card.append(cfg_row)

        self.rules_box = Gtk.Box(orientation=Gtk.Orientation.VERTICAL, spacing=6)
        card.append(self.rules_box)

        add_row = Gtk.Box(spacing=8)
        self.rule_name_entry = Gtk.Entry()
        self.rule_name_entry.set_placeholder_text("规则名称")
        self.rule_match_entry = Gtk.Entry()
        self.rule_match_entry.set_placeholder_text("匹配进程关键词（逗号分隔）")
        self.rule_match_entry.set_hexpand(True)
        self.rule_profile_dropdown = Gtk.DropDown.new_from_strings(["（无可用预设）"])
        self.add_rule_button = Gtk.Button(label="添加规则")
        self.add_rule_button.connect("clicked", self.add_rule_clicked)
        add_row.append(self.rule_name_entry)
        add_row.append(self.rule_match_entry)
        add_row.append(self.rule_profile_dropdown)
        add_row.append(self.add_rule_button)
        card.append(add_row)

        actions = Gtk.Box(spacing=12)
        self.save_button = Gtk.Button(label="保存预设与联动规则")
        self.save_button.add_css_class("suggested-action")
        self.save_button.connect("clicked", self.save)
        actions.append(self.save_button)
        card.append(actions)
        self.append(card)

    def _rebuild_profile_dropdowns(self, select_name: Optional[str] = None) -> None:
        self._suppress_refresh = True
        try:
            self._profile_names = list(self._profiles.keys())
            labels = self._profile_names if self._profile_names else ["（暂无预设）"]
            self.profile_dropdown.set_model(Gtk.StringList.new(labels))
            rule_labels = self._profile_names if self._profile_names else ["（无可用预设）"]
            self.rule_profile_dropdown.set_model(Gtk.StringList.new(rule_labels))
            if self._profile_names:
                idx = self._profile_names.index(select_name) if select_name in self._profile_names else 0
                self.profile_dropdown.set_selected(idx)
                self.rule_profile_dropdown.set_selected(0)
            else:
                self.profile_dropdown.set_selected(0)
                self.rule_profile_dropdown.set_selected(0)
        finally:
            self._suppress_refresh = False
        self._on_profile_selected()

    def _rebuild_rules_list(self) -> None:
        child = self.rules_box.get_first_child()
        while child is not None:
            nxt = child.get_next_sibling()
            self.rules_box.remove(child)
            child = nxt
        if not self._rules:
            empty = Gtk.Label(label="尚无应用联动规则。", xalign=0)
            empty.add_css_class("small-note")
            self.rules_box.append(empty)
            return
        for idx, rule in enumerate(self._rules):
            row = Gtk.Box(spacing=8)
            row.add_css_class("well")
            chk = Gtk.CheckButton(label=rule["name"])
            chk.set_active(bool(rule["enabled"]))
            chk.connect("toggled", lambda btn, i=idx: self._toggle_rule(i, btn.get_active()))
            row.append(chk)
            desc = Gtk.Label(
                label=f"匹配: {', '.join(rule['match'])}  →  预设: {rule['profile']}",
                xalign=0,
                wrap=True,
            )
            desc.set_hexpand(True)
            desc.add_css_class("small-note")
            row.append(desc)
            del_btn = Gtk.Button(label="移除")
            del_btn.add_css_class("destructive-action")
            del_btn.connect("clicked", lambda _b, i=idx: self._remove_rule(i))
            row.append(del_btn)
            self.rules_box.append(row)

    def _toggle_rule(self, index: int, enabled: bool) -> None:
        if 0 <= index < len(self._rules):
            self._rules[index]["enabled"] = bool(enabled)

    def _remove_rule(self, index: int) -> None:
        if 0 <= index < len(self._rules):
            removed = self._rules.pop(index)
            self._rebuild_rules_list()
            self.message.set_text(f"已移除规则 {removed['name']!r}（点击“保存预设与联动规则”后写入文件）。")

    def _selected_profile_name(self) -> Optional[str]:
        if not self._profile_names:
            return None
        idx = self.profile_dropdown.get_selected()
        if 0 <= idx < len(self._profile_names):
            return self._profile_names[idx]
        return None

    def _on_profile_selected(self, *_args) -> None:
        if self._suppress_refresh:
            return
        name = self._selected_profile_name()
        if name is None:
            self.profile_summary.set_text("尚未创建预设，可导入内置模板或从当前状态保存。")
        else:
            self.profile_summary.set_text(_summarize_profile(self._profiles[name]))
        self._refresh_sensitivity()

    def _refresh_sensitivity(self, *_args) -> None:
        has_profiles = bool(self._profile_names)
        enabled = self.auto_enabled.get_active()
        self.apply_button.set_sensitive(has_profiles and not self._busy)
        self.delete_button.set_sensitive(has_profiles and not self._busy)
        self.capture_button.set_sensitive(not self._busy)
        self.template_button.set_sensitive(not self._busy)
        self.restore_previous.set_sensitive(enabled)
        self.cooldown_spin.set_sensitive(enabled)
        self.add_rule_button.set_sensitive(has_profiles)

    def load_values(self, config: dict) -> None:
        cfg = profiles.validate_config(config)
        self.auto_enabled.set_active(cfg["auto_switch_enabled"])
        self.restore_previous.set_active(cfg["restore_previous"])
        self.cooldown_spin.set_value(cfg["cooldown_seconds"])
        self._profiles = copy.deepcopy(cfg["profiles"])
        self._rules = copy.deepcopy(cfg["rules"])
        self._rebuild_profile_dropdowns()
        self._rebuild_rules_list()
        self._refresh_sensitivity()

    def config(self) -> dict:
        return profiles.validate_config(
            {
                "version": 1,
                "auto_switch_enabled": self.auto_enabled.get_active(),
                "restore_previous": self.restore_previous.get_active(),
                "cooldown_seconds": self.cooldown_spin.get_value_as_int(),
                "profiles": copy.deepcopy(self._profiles),
                "rules": copy.deepcopy(self._rules),
            }
        )

    def save(self, *_args) -> None:
        try:
            saved = profiles.save_config(self.path, self.config())
        except (OSError, ValueError, TypeError) as exc:
            self.message.set_text("保存失败：" + str(exc))
            return
        self.load_values(saved)
        state_msg = "已保存；应用自动联动已启用。" if saved["auto_switch_enabled"] else "已保存；应用自动联动保持关闭。"
        self.message.set_text(state_msg)
        if self.on_saved:
            try:
                self.on_saved(saved)
            except Exception as exc:
                self.message.set_text("配置已保存，但回调通知失败：" + str(exc))

    def import_templates(self, *_args) -> None:
        added = []
        for name, body in profiles.builtin_templates().items():
            if name not in self._profiles:
                self._profiles[name] = body
                added.append(name)
        first = added[0] if added else self._selected_profile_name()
        self._rebuild_profile_dropdowns(select_name=first)
        if added:
            self.save()
            self.message.set_text(f"已导入内置模板：{', '.join(added)}")
        else:
            self.message.set_text("内置模板已全部存在。")

    def delete_selected(self, *_args) -> None:
        name = self._selected_profile_name()
        if not name:
            return
        self._profiles.pop(name, None)
        self._rules = [r for r in self._rules if r["profile"] != name]
        self._rebuild_profile_dropdowns()
        self._rebuild_rules_list()
        self.save()
        self.message.set_text(f"已删除预设 {name!r}。")

    def add_rule_clicked(self, *_args) -> None:
        name = self.rule_name_entry.get_text().strip()
        raw_match = [tok.strip().lower() for tok in self.rule_match_entry.get_text().split(",") if tok.strip()]
        idx = self.rule_profile_dropdown.get_selected()
        target = self._profile_names[idx] if 0 <= idx < len(self._profile_names) else ""
        try:
            rule = profiles.validate_rule(
                {"name": name, "enabled": True, "match": raw_match, "profile": target},
                self._profiles.keys(),
            )
        except ValueError as exc:
            self.message.set_text("规则无效：" + str(exc))
            return
        self._rules = [r for r in self._rules if r["name"] != rule["name"]]
        self._rules.append(rule)
        self.rule_name_entry.set_text("")
        self.rule_match_entry.set_text("")
        self._rebuild_rules_list()
        self.message.set_text(f"已添加规则 {rule['name']!r}（点击“保存预设与联动规则”后写入文件）。")

    def _run_async(self, label: str, fn: Callable[[], object], done: Callable[[object, Optional[Exception]], None]) -> None:
        if self._busy:
            return
        self._busy = True
        self._refresh_sensitivity()
        self.message.set_text(label)

        def worker():
            res = None
            err: Optional[Exception] = None
            try:
                res = fn()
            except Exception as exc:  # noqa: BLE001
                err = exc
            GLib.idle_add(lambda: self._finish_async(done, res, err))

        threading.Thread(target=worker, daemon=True).start()

    def _finish_async(self, done: Callable[[object, Optional[Exception]], None], res: object, err: Optional[Exception]) -> bool:
        self._busy = False
        if not self._closed:
            done(res, err)
            self._refresh_sensitivity()
        return False

    def capture_current(self, *_args) -> None:
        name = self.capture_name.get_text().strip()
        label = self.capture_label.get_text().strip() or None
        inc_perf = self.capture_perf.get_active()
        inc_light = self.capture_light.get_active()
        try:
            name = profiles._clean_text(name, 48, "预设名称", forbid_sep=True)
        except ValueError as exc:
            self.message.set_text("无法保存：" + str(exc))
            return
        if not inc_perf and not inc_light:
            self.message.set_text("请至少勾选“包含当前性能档位”或“包含当前三区灯光”。")
            return

        def task():
            return self._capture_fn(label, include_lighting=inc_light, include_performance=inc_perf)

        def finish(res, err):
            if err is not None:
                self.message.set_text("捕获当前状态失败：" + str(err))
                return
            self._profiles[name] = res  # type: ignore[assignment]
            self.capture_name.set_text("")
            self.capture_label.set_text("")
            self._rebuild_profile_dropdowns(select_name=name)
            self.save()
            self.message.set_text(f"已从当前状态保存预设 {name!r}。")

        self._run_async(f"正在读取当前状态并保存预设 {name!r}…", task, finish)

    def apply_selected(self, *_args) -> None:
        name = self._selected_profile_name()
        if not name:
            return
        body = copy.deepcopy(self._profiles[name])

        def task():
            return self._apply_fn(body, save_manual=True)

        def finish(res, err):
            if err is not None:
                self.message.set_text(f"应用预设 {name!r} 失败：{err}")
                return
            info = res if isinstance(res, dict) else {}
            warn = info.get("warnings") or []
            suffix = f"（注意：{'; '.join(warn)}）" if warn else ""
            self.message.set_text(f"已应用预设 {name!r}{suffix}。")

        self._run_async(f"正在应用预设 {name!r}…", task, finish)

    def close(self) -> None:
        self._closed = True

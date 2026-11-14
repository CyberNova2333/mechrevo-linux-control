"""Asynchronous UI for this machine's verified firmware mode helper."""
import json

from gi.repository import Gio, GLib, Gtk

HELPER = '/usr/local/libexec/mechrevo-profile-helper'
LABELS = {'office': '办公 · 绿色', 'balanced': '均衡 · 蓝色', 'turbo': '狂暴 · 紫色'}


class PerformancePanel(Gtk.Box):
    def __init__(self):
        super().__init__(orientation=Gtk.Orientation.VERTICAL, spacing=14)
        self.add_css_class('performance-page')
        self.busy = False
        self.switchable = False
        title = Gtk.Label(label='性能模式', xalign=0)
        title.add_css_class('page-title'); self.append(title)
        subtitle = Gtk.Label(label='直接切换机身固件档位，与硬件模式键保持同步', xalign=0, wrap=True)
        subtitle.add_css_class('subtitle'); self.append(subtitle)
        self.current = Gtk.Label(label='正在读取当前档位…', xalign=0)
        self.current.add_css_class('performance-current'); self.append(self.current)
        self.buttons = []
        row = Gtk.Box(spacing=12, homogeneous=True)
        for mode, title, color, css in [('office','办公模式','绿色指示灯','green-led'),
                                       ('balanced','均衡模式','蓝色指示灯','blue-led'),
                                       ('turbo','狂暴模式','紫色指示灯','purple-led')]:
            button = Gtk.Button(); button.add_css_class('mode-card')
            content = Gtk.Box(orientation=Gtk.Orientation.VERTICAL, spacing=12)
            name = Gtk.Label(label=title, xalign=0); name.add_css_class('mode-name')
            led = Gtk.Label(label='● '+color, xalign=0); led.add_css_class('mode-led'); led.add_css_class(css)
            content.append(name); content.append(led); button.set_child(content)
            button.set_sensitive(False)
            button.connect('clicked', lambda _button, selected=mode: self.request(['set', selected]))
            row.append(button); self.buttons.append((mode, button))
        self.append(row)
        self.message = Gtk.Label(xalign=0, wrap=True)
        self.message.add_css_class('status-text'); self.append(self.message)
        note = Gtk.Label(label='点击模式卡片立即切换。机身模式键仍可使用，当前状态每 3 秒刷新。', xalign=0, wrap=True)
        note.add_css_class('subtitle'); self.append(note)
        note2 = Gtk.Label(label='启动、开机与唤醒时保持当前档位，不自动切换。', xalign=0, wrap=True)
        note2.add_css_class('subtitle'); self.append(note2)
        pending = Gtk.Box(orientation=Gtk.Orientation.VERTICAL, spacing=10)
        pending.add_css_class('control-card')
        title = Gtk.Label(label='更多模式 · 待验证', xalign=0); title.add_css_class('section-title'); pending.append(title)
        detail = Gtk.Label(label='自定义（白色）、静音狂暴与高能狂暴尚未开放。\n当前“狂暴”对应机身按键的紫色档，不指定狂暴子模式。', xalign=0, wrap=True)
        detail.add_css_class('subtitle'); pending.append(detail)
        self.append(pending)
        self.connect('map', self._mapped)
        self.connect('unmap', self._unmapped)
        self.timer = None
        self.request(['status'])

    def _mapped(self, *_):
        if self.timer is None:
            self.timer = GLib.timeout_add_seconds(3, self._poll)
        self._poll()

    def _unmapped(self, *_):
        if self.timer is not None:
            GLib.source_remove(self.timer)
            self.timer = None

    def _poll(self):
        if not self.busy:
            self.request(['status'])
        return True

    def request(self, args):
        if self.busy:
            return
        self.busy = True
        for _, button in self.buttons:
            button.set_sensitive(False)
        if args[0] == 'set':
            self.message.set_text('正在切换并核对硬件回读…')
        try:
            process = Gio.Subprocess.new(['/usr/bin/pkexec', '--disable-internal-agent', HELPER, *args],
                                         Gio.SubprocessFlags.STDOUT_PIPE | Gio.SubprocessFlags.STDERR_PIPE)
        except GLib.Error as exc:
            self.busy = False
            self.message.set_text('性能控制未就绪：' + str(exc))
            return
        timeout = GLib.timeout_add_seconds(5, lambda: (process.force_exit(), False)[1])
        process.communicate_utf8_async(None, None, self._finished, (args[0], timeout))

    def _finished(self, process, result, context):
        operation, timeout = context
        # The timeout may already have fired; find_source avoids a GLib warning.
        if GLib.MainContext.default().find_source_by_id(timeout):
            GLib.source_remove(timeout)
        self.busy = False
        try:
            _, output, error = process.communicate_utf8_finish(result)
            try:
                data = json.loads(output)
            except (ValueError, TypeError):
                raise RuntimeError(error.strip() or '性能助手未返回数据（可能未授权或已超时）')
            if not process.get_successful() or 'error' in data:
                raise RuntimeError(data.get('error', error.strip() or '性能控制失败'))
            self.switchable = data['switchable']
            mode = data['mode']
            self.current.set_text('当前：' + LABELS.get(mode, '未知档位'))
            self.message.set_text(data['reason'] or ('切换成功，硬件回读一致' if operation == 'set' else '已连接固件档位控制'))
            for value, button in self.buttons:
                button.set_sensitive(self.switchable and value != mode)
                if value == mode:
                    button.add_css_class('selected')
                else:
                    button.remove_css_class('selected')
        except Exception as exc:
            self.switchable = False
            self.current.set_text('当前：无法读取')
            self.message.set_text(str(exc))

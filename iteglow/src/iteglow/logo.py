# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
"""Asynchronous controls for the verified, independent EC logo light."""
import json
import math
import time
from pathlib import Path
from gi.repository import Gio, GLib, Gtk
from . import state
from .performance import HELPER
from .zone_lock import ZoneTransaction

MODES = [('solid', '常亮'), ('breathing', '呼吸'), ('rainbow', '七彩渐变')]
COLORS = [('白色', [255,255,255]), ('红色', [255,0,0]), ('绿色', [0,255,0]),
          ('蓝色', [0,0,255]), ('黄色', [255,255,0]), ('紫色', [136,0,255]), ('青色', [0,255,255])]
APPLIED = Path.home()/'.local/state/iteglow/logo.json'
PREFS = Path.home()/'.local/state/iteglow/logo-prefs.json'


class LogoEmblemPreview(Gtk.DrawingArea):
    """Rear-lid backlit logo emblem preview; reads UI controls only."""
    def __init__(self, panel):
        super().__init__()
        self._panel = panel
        self.set_content_height(132)
        self.set_hexpand(True)
        self.set_draw_func(self._draw)

    def _draw(self, _area, cr, width, height):
        cr.set_source_rgb(0.055, 0.072, 0.105)
        cr.rectangle(0, 0, width, height)
        cr.fill()
        cr.set_source_rgb(0.14, 0.18, 0.25)
        cr.set_line_width(1.0)
        cr.rectangle(0.5, 0.5, width - 1, height - 1)
        cr.stroke()

        cfg = self._panel.config()
        br = cfg['brightness'] / 100.0
        mode = cfg['mode']
        r, g, b = [v / 255.0 for v in cfg['color']]
        if mode == 'rainbow':
            r, g, b = (0.28, 0.78, 0.98)

        # Brushed A-cover metal bezel silhouette
        lw, lh = min(width * 0.56, 360.0), height - 28.0
        lx, ly = (width - lw) / 2.0, 14.0
        cr.set_source_rgb(0.10, 0.13, 0.19)
        cr.rectangle(lx, ly, lw, lh); cr.fill_preserve()
        cr.set_source_rgb(0.20, 0.26, 0.36); cr.stroke()

        cx, cy = width / 2.0, height / 2.0
        if br > 0:
            for rad, alpha in ((34, 0.12 * br), (22, 0.22 * br)):
                cr.arc(cx, cy, rad, 0, 2 * math.pi)
                cr.set_source_rgba(r, g, b, alpha)
                cr.fill()
        cr.set_line_width(3.4)
        cr.set_line_cap(1)
        if br > 0:
            cr.set_source_rgba(r, g, b, 0.35 + 0.65 * br)
        else:
            cr.set_source_rgb(0.24, 0.29, 0.38)
        cr.move_to(cx - 22, cy + 12)
        cr.line_to(cx - 10, cy - 12)
        cr.line_to(cx, cy + 4)
        cr.line_to(cx + 10, cy - 12)
        cr.line_to(cx + 22, cy + 12)
        cr.stroke()

        cr.select_font_face('sans-serif', 0, 0)
        cr.set_font_size(10)
        cr.set_source_rgb(0.54, 0.63, 0.76)
        cr.move_to(16, 22); cr.show_text('REAR LID EMBLEM  /  ' + dict(MODES)[mode])


class LogoPanel(Gtk.Box):
    def __init__(self):
        super().__init__(orientation=Gtk.Orientation.VERTICAL, spacing=18)
        self.add_css_class('device-page')
        self.busy = False; self.pending = None; self.initialized = False; self.closed = False; self.held_app = None
        self.zone = None; self.zone_retry = None; self.zone_args = None; self.zone_started = None
        title = Gtk.Label(label='后盖 Logo', xalign=0)
        title.add_css_class('page-title'); self.append(title)
        description = Gtk.Label(label='独立设置屏幕后盖灯光', xalign=0, wrap=True)
        description.add_css_class('subtitle'); self.append(description)
        card = Gtk.Box(orientation=Gtk.Orientation.VERTICAL, spacing=16)
        card.add_css_class('control-card'); self.append(card)
        self.current = Gtk.Label(label='正在读取 Logo 状态…', xalign=0)
        self.current.add_css_class('section-title'); card.append(self.current)
        self.mode = Gtk.DropDown.new_from_strings([caption for _,caption in MODES])
        self.brightness = Gtk.DropDown.new_from_strings(['关闭', '25%', '50%', '75%', '100%'])
        self.brightness.set_selected(1)
        self.palette = Gtk.DropDown.new_from_strings([caption for caption,_ in COLORS]+['自选颜色'])
        self.preview = LogoEmblemPreview(self)
        card.append(self.preview)
        for caption, widget in [('灯光效果',self.mode), ('亮度',self.brightness), ('颜色',self.palette)]:
            row=Gtk.Box(spacing=12); row.add_css_class('well'); label=Gtk.Label(label=caption,xalign=0);label.set_hexpand(True)
            row.append(label);row.append(widget);card.append(row)
        self.rgb_row=Gtk.Box(spacing=10); self.rgb=[]
        for name in ('红','绿','蓝'):
            self.rgb_row.append(Gtk.Label(label=name)); spin=Gtk.SpinButton.new_with_range(0,255,1)
            spin.set_value(255);spin.set_numeric(True)
            spin.connect('value-changed', lambda *_: self.preview.queue_draw())
            self.rgb.append(spin);self.rgb_row.append(spin)
        card.append(self.rgb_row)
        self.palette.connect('notify::selected',self.selection_changed)
        self.mode.connect('notify::selected',self.selection_changed)
        self.brightness.connect('notify::selected', lambda *_: self.preview.queue_draw())
        self.selection_changed()
        note=Gtk.Label(label='呼吸支持七种固定颜色；常亮可自选颜色。编辑后点击“应用灯光”。',xalign=0,wrap=True)
        note.add_css_class('small-note');card.append(note)
        row=Gtk.Box(spacing=12)
        self.apply_button=Gtk.Button(label='应用灯光');self.apply_button.add_css_class('suggested-action')
        self.apply_button.connect('clicked',self.apply); row.append(self.apply_button)
        self.off_button=Gtk.Button(label='关闭 Logo');self.off_button.add_css_class('destructive-action');self.off_button.connect('clicked',lambda *_:self.set_enabled(False));row.append(self.off_button)
        self.buttons=[self.apply_button,self.off_button]
        for button in self.buttons:button.set_sensitive(False)
        card.append(row)
        self.message=Gtk.Label(xalign=0,wrap=True);self.message.add_css_class('status-text');self.append(self.message)
        self.connect('map',lambda *_:self.request(['logo-status']))
        self.timer=GLib.timeout_add_seconds(3,self.poll)

    def selection_changed(self,*_):
        mode=MODES[self.mode.get_selected()][0]
        if mode=='breathing' and self.palette.get_selected()==7:self.palette.set_selected(0)
        self.palette.set_sensitive(mode!='rainbow')
        self.rgb_row.set_visible(mode=='solid' and self.palette.get_selected()==7)
        if hasattr(self, 'preview'):
            self.preview.queue_draw()

    def load_values(self,config):
        try:
            index=[mode for mode,_ in MODES].index(config['mode'])
            brightness=config['brightness'];color=config['color']
            if type(brightness) is not int or brightness not in (0,25,50,75,100):return
            if not isinstance(color,list) or len(color)!=3 or any(type(c) is not int or not 0<=c<=255 for c in color):return
            self.mode.set_selected(index);self.brightness.set_selected(brightness//25)
            colors=[rgb for _,rgb in COLORS]
            self.palette.set_selected(colors.index(color) if color in colors else 7)
            for spin,value in zip(self.rgb,color):spin.set_value(value)
            self.selection_changed()
        except (KeyError,TypeError,ValueError):return

    def config(self):
        mode=MODES[self.mode.get_selected()][0];index=self.palette.get_selected()
        if mode=='breathing' and index==7:index=0
        color=COLORS[index][1] if index<7 else [spin.get_value_as_int() for spin in self.rgb]
        return {'mode':mode,'brightness':self.brightness.get_selected()*25,'color':color}

    def apply(self,*_):
        config=self.config()
        self.request(['logo',json.dumps(config)])

    def poll(self):
        if self.closed:return False
        if self.get_mapped() and not self.busy:self.request(['logo-status'])
        return True

    def set_enabled(self,enabled):
        self.request(['logo','on' if enabled else 'off'])

    def request(self,args):
        if self.closed:return
        if args[0]=='logo' and self.held_app is None:
            app=Gio.Application.get_default()
            if app is not None:app.hold();self.held_app=app
        if self.busy:
            if args[0]!='logo-status':self.pending=args
            return
        self.busy=True
        for button in self.buttons:button.set_sensitive(False)
        if args[0]=='logo':
            self.zone=ZoneTransaction('logo',reentrant=False)
            try:
                if not self.zone.acquire(blocking=False):
                    self.zone_args=args;self.zone_started=time.monotonic()
                    self.zone_retry=GLib.timeout_add(100,self.retry_zone)
                    self.message.set_text('正在等待当前 Logo 灯光操作完成…')
                    return
            except Exception as exc:
                self.busy=False;self.message.set_text(str(exc));self.release_zone();self.release_hold();return
        self.start_helper(args)

    def retry_zone(self):
        if self.closed:
            self.zone_retry=None;self.zone_args=None;self.busy=False
            self.release_zone();self.release_hold();return False
        try:
            if not self.zone.acquire(blocking=False):
                if time.monotonic()-self.zone_started<5:return True
                raise TimeoutError('Logo 灯光操作仍忙，请稍后重试')
        except Exception as exc:
            self.zone_retry=None;self.zone_args=None;self.pending=None;self.busy=False
            self.message.set_text(str(exc));self.release_zone();self.release_hold()
            for button in self.buttons:button.set_sensitive(self.initialized)
            return False
        args=self.pending or self.zone_args
        self.pending=None;self.zone_args=None;self.zone_retry=None
        self.start_helper(args)
        return False

    def start_helper(self,args):
        try:
            process=Gio.Subprocess.new(['/usr/bin/pkexec','--disable-internal-agent',HELPER,*args],
                Gio.SubprocessFlags.STDOUT_PIPE|Gio.SubprocessFlags.STDERR_PIPE)
        except GLib.Error as exc:
            self.busy=False;self.message.set_text(str(exc));self.release_zone();self.release_hold();return
        timeout=GLib.timeout_add_seconds(5,lambda:(process.force_exit(),False)[1])
        process.communicate_utf8_async(None,None,self.finished,(args,timeout))

    def finished(self,process,result,context):
        args,timeout=context
        if GLib.MainContext.default().find_source_by_id(timeout):GLib.source_remove(timeout)
        self.busy=False
        try:
            _,output,error=process.communicate_utf8_finish(result)
            data=json.loads(output)
            if not process.get_successful() or 'error' in data:
                raise RuntimeError(data.get('error') or error.strip() or 'Logo 控制失败')
            if args[0]=='logo':
                try:
                    from .lighting_session import validate_logo
                    actual={k:data[k] for k in ('mode','brightness','color')}
                    validate_logo(actual)
                    state.write(str(APPLIED),actual)
                    if args[1] not in ('on','off'):state.write(str(PREFS),json.loads(args[1]))
                except OSError as exc:save_error='；保存恢复设置失败：'+str(exc)
                else:save_error=''
            else:save_error=''
            if self.closed:return
            caption=dict(MODES)[data['mode']]
            self.current.set_text(('当前：'+caption+' · '+str(data['brightness'])+'%') if data['logo_enabled'] else '当前：Logo 灯已关闭')
            if not self.initialized:
                self.load_values(data if data['logo_enabled'] else state.load(str(PREFS)) or data)
                self.initialized=True
            for button in self.buttons:button.set_sensitive(True)
            self.message.set_text(('已读取当前硬件状态' if args[0]=='logo-status' else '设置已发送，寄存器回读一致')+save_error)
        except Exception as exc:
            if not self.closed:self.message.set_text(str(exc))
        finally:
            self.release_zone()
            if self.closed:self.pending=None;self.release_hold()
        if self.pending and not self.closed:
            args,self.pending=self.pending,None;self.request(args)
        else:self.release_hold()

    def release_zone(self):
        if self.zone is not None:
            zone,self.zone=self.zone,None;zone.release()

    def release_hold(self):
        if self.held_app is not None:
            app,self.held_app=self.held_app,None;app.release()

    def close(self):
        self.closed=True
        if self.timer:GLib.source_remove(self.timer);self.timer=None
        if self.zone_retry is not None:
            GLib.source_remove(self.zone_retry);self.zone_retry=None
            self.zone_args=None;self.pending=None;self.busy=False
            self.release_zone();self.release_hold()
        # A helper already writing retains the zone and application hold until
        # its callback saves the readback, even if the window has been closed.

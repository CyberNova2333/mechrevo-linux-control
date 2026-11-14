"""Chinese GTK4 control centre. Construction and previews never write hardware."""
from __future__ import annotations
import math
from pathlib import Path
import gi
gi.require_version('Gtk', '4.0')
gi.require_version('Gdk', '4.0')
from gi.repository import Gdk, GLib, Gtk
from . import devices, effects, protocol, state
from .performance import PerformancePanel

MODE_TEXT = {
    'off': ('关闭', '关闭当前设备的灯光'),
    'solid': ('常亮', '整个键盘保持同一种颜色'),
    'static': ('常亮', '灯带保持同一种颜色'),
    'breathing': ('呼吸', '灯光逐渐亮起与淡出'),
    'wave': ('彩色波浪', '色彩沿指定方向流动'),
    'reactive': ('随机闪烁', '星点随机闪烁，也可随按键触发'),
    'rainbow': ('彩虹渐变', '自动循环彩虹色彩'),
    'ripple': ('涟漪', '灯光向周围扩散'),
    'marquee': ('跑马灯', '色彩依次流转'),
    'raindrop': ('雨滴', '如雨滴般散落的灯光'),
    'aurora': ('极光', '光带柔和扩散，也可随按键触发'),
    'spark': ('烟花', '光点绽放，也可随按键触发'),
    'neon': ('霓虹', '多种颜色交替变化'),
    'runner': ('追光', '光点沿灯带移动'),
}
DIRECTIONS = {
    'keyboard': [('从左向右', 1), ('从右向左', 2), ('从下向上', 3), ('从上向下', 4)],
    'lightbar': [('正向', 1), ('反向', 2)],
}


def label(text='', css=None, **kwargs):
    widget = Gtk.Label(label=text, xalign=0, **kwargs)
    if css:
        widget.add_css_class(css)
    return widget


def box(vertical=True, spacing=0, css=None):
    widget = Gtk.Box(orientation=Gtk.Orientation.VERTICAL if vertical else Gtk.Orientation.HORIZONTAL, spacing=spacing)
    if css:
        widget.add_css_class(css)
    return widget


def rgba(rgb):
    color = Gdk.RGBA()
    color.red, color.green, color.blue, color.alpha = *[v / 255 for v in rgb], 1.0
    return color


def rgb(color):
    return tuple(round(v * 255) for v in (color.red, color.green, color.blue))


def color_button(color):
    try:
        button = Gtk.ColorDialogButton.new(Gtk.ColorDialog(title='选择灯光颜色'))
    except AttributeError:
        button = Gtk.ColorButton()
    button.set_rgba(rgba(color))
    button.add_css_class('color-swatch')
    return button


def on_color(button, callback):
    signal = 'notify::rgba' if isinstance(button, Gtk.ColorDialogButton) else 'color-set'
    button.connect(signal, lambda *_: callback())


def rounded(cr, x, y, w, h, radius):
    cr.new_sub_path()
    for cx, cy, start in [(x+w-radius,y+radius,-math.pi/2), (x+w-radius,y+h-radius,0),
                          (x+radius,y+h-radius,math.pi/2), (x+radius,y+radius,math.pi)]:
        cr.arc(cx, cy, radius, start, start+math.pi/2)
    cr.close_path()


class DevicePreview(Gtk.DrawingArea):
    """Illustration only; never emits USB writes or captures keyboard input."""
    def __init__(self, panel):
        super().__init__()
        self.panel = panel
        self.set_content_height(158)
        self.set_hexpand(True)
        self.set_draw_func(self.draw)

    def draw(self, _area, cr, width, height):
        p = self.panel
        cr.set_source_rgb(.065, .085, .135)
        rounded(cr, 0, 0, width, height, 16); cr.fill()
        cr.select_font_face('sans-serif', 0, 0)
        cr.set_font_size(10)
        cr.set_source_rgb(.60, .67, .78)
        cr.move_to(20, 22); cr.show_text('RGB  /  '+('KEYBOARD' if p.device == 'keyboard' else 'LIGHTBAR'))
        mode = p.mode
        on = mode != 'off' and p.bright.get_value() > 0
        palette = [rgb(b.get_rgba()) for b in p.palette_buttons]
        solid = rgb(p.color.get_rgba())
        def tint(n):
            if not on: return (.20, .24, .31)
            c = solid if p.spec().get('single') else palette[n % len(palette)]
            factor = .35 + .65 * p.bright.get_value()/100
            return tuple(v/255*factor for v in c)
        if p.device == 'lightbar':
            cr.set_source_rgb(.13, .17, .24)
            rounded(cr, width*.12, 46, width*.76, 70, 12); cr.fill()
            cr.set_source_rgb(.24, .29, .38)
            rounded(cr, width*.16, 50, width*.68, 48, 5); cr.fill()
            for n in range(48):
                cr.set_source_rgb(*tint(n//7))
                rounded(cr, width*.13+n*width*.74/48, 106, width*.74/48-1, 5, 2); cr.fill()
        else:
            # Simplified full keyboard with numpad, deliberately labelled as an illustration.
            cr.save(); cr.translate(max(20, (width-610)/2), 38)
            scale = min(1.0, (width-40)/610); cr.scale(scale, scale)
            rows = [list('1234567890−=')+['Bksp'], list('QWERTYUIOP')+['[',']','\\'],
                    list('ASDFGHJKL')+[';','\'', 'Enter'], list('ZXCVBNM')+[',','.','/','↑'],
                    ['Ctrl','Fn','Alt','SPACE','Alt','←','↓','→']]
            for row, keys in enumerate(rows):
                x = row*4 if row < 4 else 0
                for n, key in enumerate(keys):
                    w = 148 if key=='SPACE' else 34 if len(key)>1 else 29
                    cr.set_source_rgb(.115,.15,.21)
                    rounded(cr,x,row*20,w,16,3);cr.fill_preserve()
                    cr.set_source_rgb(*tint(n//2+row));cr.set_line_width(.8);cr.stroke()
                    cr.set_font_size(6 if key=='SPACE' else 7)
                    cr.set_source_rgb(.75,.81,.9)
                    if key != 'SPACE': cr.move_to(x+5,row*20+11);cr.show_text(key)
                    x += w+4
            for row in range(5):
                for col in range(4):
                    cr.set_source_rgb(.115,.15,.21)
                    rounded(cr,460+col*32,row*20,28,16,3);cr.fill_preserve()
                    cr.set_source_rgb(*tint(col+row+3));cr.stroke()
            cr.restore()
        cr.set_font_size(9); cr.set_source_rgb(.53,.60,.71)
        cr.move_to(20,height-12); cr.show_text('颜色与布局示意 · 灯效请以实物为准')


class DevicePanel(Gtk.Box):
    def __init__(self, device):
        super().__init__(orientation=Gtk.Orientation.VERTICAL, spacing=16)
        self.device = device
        self.dev = devices.HidLed(devices.led_for_label(device))
        self.modes = effects.MODES_BY_LABEL[device]
        self.mode = 'solid' if device == 'keyboard' else 'static'
        self.bmax = protocol.KBD_BRIGHTNESS_MAX if device=='keyboard' else protocol.LB_BRIGHTNESS_MAX
        self.ready = False
        self.pending = None
        self.add_css_class('device-page')
        self.status = label('', 'status-text', wrap=True)
        self.bright = Gtk.Scale.new_with_range(Gtk.Orientation.HORIZONTAL, 0, 100, 1)
        self.bright.set_draw_value(False); self.bright.set_hexpand(True);self.bright.set_value(60)
        self.speed = Gtk.Scale.new_with_range(Gtk.Orientation.HORIZONTAL, 1, 10, 1)
        self.speed.set_draw_value(False);self.speed.set_value(6)
        self.direction = Gtk.DropDown.new_from_strings([t for t,_ in DIRECTIONS[device]])
        self.trigger = Gtk.DropDown.new_from_strings(['自动播放', '随按键触发'])
        self.color = color_button((0,255,255))
        self.palette_buttons = [color_button(c) for c in effects.RAINBOW]
        self._load()
        heading = box(False, 10)
        names = box(spacing=5)
        names.append(label('键盘背光' if device=='keyboard' else '机身灯带', 'page-title'))
        names.append(label('设置整块键盘的色彩与律动' if device=='keyboard' else '机身轮廓灯光 · 当前显示最近保存的设置', 'subtitle'))
        names.set_hexpand(True);heading.append(names)
        badge = label('● 已连接', 'connection-pill');badge.set_valign(Gtk.Align.CENTER);heading.append(badge)
        self.append(heading)
        self.preview = DevicePreview(self);self.append(self.preview)
        lower = box(False, 16)
        effect_card = box(spacing=12, css='control-card');effect_card.set_hexpand(True)
        effect_card.append(label('选择灯效', 'section-title'))
        self.effect_buttons = {}
        grid = Gtk.Grid(column_spacing=8, row_spacing=8, column_homogeneous=True)
        for index, name in enumerate(self.modes):
            title, description = MODE_TEXT[name]
            button = Gtk.Button(label=title)
            button.set_tooltip_text(description)
            button.add_css_class('effect-tile')
            button.set_hexpand(True)
            button.connect('clicked', lambda _, value=name: self.choose(value))
            grid.attach(button,index%3,index//3,1,1)
            self.effect_buttons[name] = button
        effect_card.append(grid)
        self.description = label('', 'subtitle', wrap=True);effect_card.append(self.description)
        lower.append(effect_card)
        controls = box(spacing=14, css='control-card')
        controls.set_size_request(248, -1)
        controls.append(label('灯光参数', 'section-title'))
        self.bright_value = label('', 'value-label')
        controls.append(self._slider_row('亮度',self.bright,self.bright_value))
        self.speed_value = label('', 'value-label')
        self.speed_row = self._slider_row('速度',self.speed,self.speed_value)
        controls.append(self.speed_row)
        self.direction_row = self._select_row('流动方向',self.direction);controls.append(self.direction_row)
        self.trigger_row = self._select_row('触发方式',self.trigger);controls.append(self.trigger_row)
        self.trigger_hint = label('按键触发时，空闲熄灭属于正常现象', 'small-note', wrap=True)
        controls.append(self.trigger_hint)
        self.single_row = self._select_row('灯光颜色',self.color);controls.append(self.single_row)
        self.palette_row = box(spacing=8)
        pal_title = box(False,6);t=label('七色调色板');t.set_hexpand(True);pal_title.append(t)
        reset = Gtk.Button(label='重置');reset.add_css_class('small-button');reset.connect('clicked',self.reset_palette);pal_title.append(reset)
        self.palette_row.append(pal_title)
        swatches = Gtk.Grid(column_spacing=5,row_spacing=5,column_homogeneous=True)
        for index, button in enumerate(self.palette_buttons):
            button.set_tooltip_text(f'第 {index+1} 种颜色')
            swatches.attach(button,index%4,index//4,1,1)
        self.palette_row.append(swatches);controls.append(self.palette_row)
        lower.append(controls);self.append(lower)
        self.append(self.status)
        self.refresh()
        self.bright.connect('value-changed',lambda *_: self.schedule())
        self.speed.connect('value-changed',lambda *_: self.schedule())
        self.direction.connect('notify::selected',lambda *_: self.schedule())
        self.trigger.connect('notify::selected',lambda *_: self.schedule())
        on_color(self.color,self.schedule)
        for button in self.palette_buttons: on_color(button,self.schedule)
        self.ready = True

    def _slider_row(self,title,slider,value):
        result = box(spacing=4); top=box(False,8)
        name=label(title);name.set_hexpand(True);top.append(name);top.append(value)
        result.append(top);result.append(slider);return result

    def _select_row(self,title,control):
        result=box(False,8);text=label(title);text.set_hexpand(True)
        result.append(text);result.append(control);return result

    def _load(self):
        saved=state.load(state.user_state_path()).get(self.device) or {}
        if saved.get('mode') in self.modes:self.mode=saved['mode']
        if saved.get('brightness') is not None and saved['brightness']>0:
            self.bright.set_value(min(100,saved['brightness']/self.bmax*100))
        self.speed.set_value(11-min(10,max(1,saved.get('speed') or 5)))
        self.trigger.set_selected(int(bool(saved.get('reactive',False))))
        for i,(_,value) in enumerate(DIRECTIONS[self.device]):
            if value==saved.get('direction'):self.direction.set_selected(i)
        if saved.get('color'):self.color.set_rgba(rgba(saved['color']))
        for button,color in zip(self.palette_buttons,saved.get('colors') or []):button.set_rgba(rgba(color))
        if self.device=='keyboard':
            try:
                actual=protocol.parse_keyboard_state(self.dev.query(protocol.QUERY_EFFECT))
                if actual['power']==1 or actual['brightness']==0:self.mode='off'
                else:
                    self.mode=next((name for name,spec in self.modes.items() if spec['code']==actual['mode']),self.mode)
                    self.bright.set_value(actual['brightness']/self.bmax*100)
                    if 1<=actual['speed']<=10:self.speed.set_value(11-actual['speed'])
                    if self.spec().get('trigger'):self.trigger.set_selected(int(actual['direction']==1))
                    elif self.spec().get('direction') and 1<=actual['direction']<=4:self.direction.set_selected(actual['direction']-1)
            except OSError:self.status.set_text('无法读取当前灯效，已显示上次保存值')

    def spec(self):return self.modes[self.mode]

    def choose(self,mode):
        self.mode=mode
        if mode!='off' and self.bright.get_value()==0:self.bright.set_value(60)
        self.schedule()

    def refresh(self):
        spec=self.spec();on=not spec.get('off')
        for name,button in self.effect_buttons.items():
            if name==self.mode:button.add_css_class('selected')
            else:button.remove_css_class('selected')
        self.description.set_text(MODE_TEXT[self.mode][1])
        self.bright_value.set_text(f'{round(self.bright.get_value())}%')
        self.speed_value.set_text(f'{round(self.speed.get_value())} / 10 · 越高越快')
        self.bright.set_sensitive(on)
        for widget,visible in [(self.speed_row,spec.get('speed')), (self.direction_row,spec.get('direction')),
                               (self.trigger_row,spec.get('trigger')), (self.single_row,spec.get('single')),
                               (self.palette_row,spec.get('palette'))]:widget.set_visible(bool(visible))
        self.trigger_hint.set_visible(bool(spec.get('trigger')) and self.trigger.get_selected()==1)
        self.preview.queue_draw()

    def reset_palette(self,*_):
        was=self.ready;self.ready=False
        for button,color in zip(self.palette_buttons,effects.RAINBOW):button.set_rgba(rgba(color))
        self.ready=was;self.schedule()

    def schedule(self):
        if not self.ready:return
        self.refresh()
        if self.pending is not None:GLib.source_remove(self.pending)
        self.pending=GLib.timeout_add(90,self.apply)

    def params(self):
        return dict(mode=self.mode,color=rgb(self.color.get_rgba()),colors=[rgb(b.get_rgba()) for b in self.palette_buttons],
                    brightness=round(self.bright.get_value()*self.bmax/100),speed=11-round(self.speed.get_value()),
                    direction=DIRECTIONS[self.device][self.direction.get_selected()][1],reactive=self.trigger.get_selected()==1)

    def apply(self):
        self.pending=None
        params=self.params()
        try:
            self.dev.send(effects.build(self.device,**params))
            state.save(self.device,params)
            suffix=' · 按键时亮起' if self.spec().get('trigger') and params['reactive'] else ''
            self.status.set_text('已应用：'+MODE_TEXT[self.mode][0]+suffix)
        except Exception as exc:self.status.set_text('设置未成功：'+str(exc))
        return False

    def close(self):
        if self.pending is not None:
            GLib.source_remove(self.pending);self.pending=None
        self.dev.close()


class App(Gtk.Application):
    def __init__(self):
        super().__init__(application_id='org.iteglow.Control')
        self.panels=[]

    def do_activate(self):
        window=self.get_active_window()
        if window:window.present();return
        provider=Gtk.CssProvider()
        provider.load_from_path(str(Path(__file__).with_name('control.css')))
        Gtk.StyleContext.add_provider_for_display(Gdk.Display.get_default(),provider,Gtk.STYLE_PROVIDER_PRIORITY_USER + 1)
        Gtk.Window.set_default_icon_name('mechrevo-control')
        window=Gtk.ApplicationWindow(application=self,title='机械革命控制中心')
        window.add_css_class('mechrevo-shell');window.set_default_size(1040,760)
        root=box(False);window.set_child(root)
        sidebar=box(spacing=10,css='sidebar');sidebar.set_size_request(170,-1)
        brand=label('M / R','brand-mark');sidebar.append(brand)
        sidebar.append(label('机械革命','brand-title'))
        sidebar.append(label('本机控制中心','subtitle'))
        section=label('设备控制','nav-section');sidebar.append(section)
        self.stack=Gtk.Stack();self.stack.set_hexpand(True);self.stack.set_vexpand(True)
        self.stack.set_transition_type(Gtk.StackTransitionType.CROSSFADE)
        self.stack.set_transition_duration(120)
        self.nav={}
        detected={label for _,_,label in devices.available_devices()}
        for name,title,icon in [('keyboard','键盘背光','input-keyboard-symbolic'),('lightbar','机身灯带','display-brightness-symbolic'),('performance','性能模式','speedometer-symbolic')]:
            if name=='performance':page=PerformancePanel()
            elif name in detected:
                try:page=DevicePanel(name);self.panels.append(page)
                except Exception as exc:page=label('设备暂不可用：'+str(exc),wrap=True)
            else:page=label('未检测到该灯光设备，请检查设备连接',wrap=True)
            scroll=Gtk.ScrolledWindow();scroll.set_policy(Gtk.PolicyType.NEVER,Gtk.PolicyType.AUTOMATIC)
            scroll.set_child(page);scroll.add_css_class('page-scroll')
            self.stack.add_named(scroll,name)
            button=Gtk.Button();button.add_css_class('nav-button')
            content=box(False,10);content.append(Gtk.Image.new_from_icon_name(icon));content.append(label(title));button.set_child(content)
            button.connect('clicked',lambda _,value=name:self.navigate(value));sidebar.append(button);self.nav[name]=button
        spacer=box();spacer.set_vexpand(True);sidebar.append(spacer)
        sidebar.append(label('YAOSHI Series','device-name'))
        sidebar.append(label('Linux 本机适配','small-note'))
        root.append(sidebar)
        right=box();right.set_hexpand(True);root.append(right)
        bar=box(False,12,css='topbar')
        welcome=label('设备个性化','topbar-label');welcome.set_hexpand(True);bar.append(welcome)
        off=Gtk.Button(label='关闭全部灯光');off.set_tooltip_text('关闭键盘与机身灯带，保留配色设置')
        off.add_css_class('all-off');off.connect('clicked',self.all_off);bar.append(off);right.append(bar)
        right.append(self.stack)
        self.navigate('keyboard');window.present()

    def navigate(self,name):
        self.stack.set_visible_child_name(name)
        for key,button in self.nav.items():
            if key==name:button.add_css_class('selected')
            else:button.remove_css_class('selected')

    def all_off(self,*_):
        for panel in self.panels:panel.choose('off')

    def do_shutdown(self):
        for panel in self.panels:
            # Apply a pending user change before closing; never schedule on construction.
            if panel.pending is not None:
                GLib.source_remove(panel.pending);panel.pending=None;panel.apply()
            panel.close()
        Gtk.Application.do_shutdown(self)


def main():
    GLib.set_prgname('mechrevo-control')
    GLib.set_application_name('机械革命控制中心')
    return App().run(None)
if __name__=='__main__':raise SystemExit(main())

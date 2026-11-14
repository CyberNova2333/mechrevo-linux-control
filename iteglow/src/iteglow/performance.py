"""Asynchronous UI for this machine's verified firmware mode helper."""
import json
from pathlib import Path

from . import state

from gi.repository import Gio, GLib, Gtk

HELPER = '/usr/local/libexec/mechrevo-profile-helper'
LABELS = {'office': '办公 · 绿色', 'balanced': '均衡 · 蓝色', 'turbo': '狂暴 · 紫色', 'custom': '自定义模式 · 白色'}


MIN_DUTY = [40,40,40,40,50,55,60,70,80,90,100]
TEMPS = {'cpu_fan':[44,48,52,56,60,64,68,72,76,80,85],
         'gpu_fan':[42,46,50,54,58,62,66,70,74,78,81]}
PREFS = Path.home()/'.local/state/iteglow/performance.json'


class TuningEditor(Gtk.Box):
    def __init__(self, request):
        super().__init__(orientation=Gtk.Orientation.VERTICAL, spacing=12)
        self.request = request; self.spins = {}; self.initialized = False
        self.add_css_class('control-card')
        title=Gtk.Label(label='自定义调校', xalign=0); title.add_css_class('section-title'); self.append(title)
        note=Gtk.Label(label='应用后进入白色自定义档。切换档位、按机身模式键或休眠前会撤销调校；关闭窗口会保留设置。',xalign=0,wrap=True)
        note.add_css_class('subtitle');self.append(note)
        row=Gtk.Box(spacing=12)
        label=Gtk.Label(label='CPU 功耗上限',xalign=0);label.set_hexpand(True);row.append(label)
        self.power=Gtk.SpinButton.new_with_range(25,75,1);self.power.set_value(35);row.append(self.power)
        row.append(Gtk.Label(label='W'));self.append(row)
        note=Gtk.Label(label='已实测 25–75 W；持续与短时上限同步设置。GPU 功耗保持固件管理。',xalign=0,wrap=True)
        note.add_css_class('small-note');self.append(note)
        expander=Gtk.Expander(label='CPU / GPU 风扇曲线 · 点击展开编辑')
        content=Gtk.Box(orientation=Gtk.Orientation.VERTICAL,spacing=10)
        stack=Gtk.Stack();switcher=Gtk.StackSwitcher(stack=stack);content.append(switcher)
        for name,caption in [('cpu_fan','CPU 风扇'),('gpu_fan','GPU 风扇')]:
            page=Gtk.Box(orientation=Gtk.Orientation.VERTICAL,spacing=8)
            chart=Gtk.DrawingArea();chart.set_content_height(150);chart.set_hexpand(True)
            chart.set_draw_func(self.draw_curve,name);page.append(chart)
            grid=Gtk.Grid(column_spacing=8,row_spacing=6,column_homogeneous=True)
            self.spins[name]=[]
            for i,temp in enumerate(TEMPS[name]):
                box=Gtk.Box(orientation=Gtk.Orientation.VERTICAL,spacing=4)
                box.append(Gtk.Label(label=('<'+str(TEMPS[name][1]) if i==0 else str(temp))+'°C'))
                spin=Gtk.SpinButton.new_with_range(MIN_DUTY[i],100,5);spin.set_value(MIN_DUTY[i]);spin.set_numeric(True)
                spin.connect('value-changed',lambda _,c=chart:c.queue_draw())
                spin.set_sensitive(i<10);box.append(spin);self.spins[name].append(spin)
                grid.attach(box,i%4,i//4,1,1)
            page.append(grid);stack.add_titled(page,name,caption)
        content.append(stack)
        note=Gtk.Label(label='节点数值为风扇占空比（%）。固定使用出厂温度节点与回落迟滞；曲线不能低于冷却基线，高温节点锁定 100%。',xalign=0,wrap=True)
        note.add_css_class('small-note');content.append(note);expander.set_child(content);self.append(expander)
        self.expander=expander
        self.validation=Gtk.Label(xalign=0,wrap=True);self.validation.add_css_class('status-text');self.append(self.validation)
        actions=Gtk.Box(spacing=10)
        self.apply_button=Gtk.Button(label='应用功耗与曲线');self.apply_button.add_css_class('suggested-action')
        self.apply_button.connect('clicked',self.apply);actions.append(self.apply_button)
        self.reset_button=Gtk.Button(label='撤销调校 / 返回原档位');self.reset_button.connect('clicked',lambda _:request(['reset']));actions.append(self.reset_button)
        self.append(actions);self.set_available(False,False)
        saved=state.load(str(PREFS))
        if saved:self.load_values(saved)

    def load_values(self,config):
        try:
            power=int(config['pl1']);curves=[config[k] for k in self.spins]
            if not 25<=power<=75 or any(len(c)!=11 or any(type(v) is not int or not MIN_DUTY[i]<=v<=100 for i,v in enumerate(c)) for c in curves):return
            self.power.set_value(power)
            for name,spins in self.spins.items():
                for spin,v in zip(spins,config[name]):spin.set_value(v)
        except (KeyError,TypeError,ValueError):pass

    def set_available(self,available,active):
        self.apply_button.set_sensitive(available);self.reset_button.set_sensitive(active)

    def update_status(self,data):
        self.set_available(data['switchable'],data.get('tuning_active',False) or data.get('tuning_recovery_pending',False))
        if not self.initialized:
            if data.get('tuning'):self.load_values(data['tuning'])
            self.initialized=True
        config=data.get('tuning')
        self.validation.set_text(('硬件当前已应用：CPU '+str(config['pl1'])+' W · 独立双风扇曲线') if config else '编辑不会立即改变硬件，点击“应用”后生效。')

    def apply(self,_):
        config={'pl1':self.power.get_value_as_int(),'pl2':self.power.get_value_as_int()}
        config.update({name:[spin.get_value_as_int() for spin in spins] for name,spins in self.spins.items()})
        if any(config[name]!=sorted(config[name]) for name in self.spins):
            self.validation.set_text('请让风扇百分比随温度单调不减，再应用。');return
        try:state.write(str(PREFS),config)
        except OSError as exc:self.validation.set_text('无法保存设置：'+str(exc));return
        self.request(['apply',json.dumps(config)])

    def draw_curve(self,area,cr,width,height,name):
        left,right,top,bottom=36,width-14,12,height-25
        cr.set_source_rgb(.96,.97,.99);cr.paint();cr.set_line_width(1);cr.set_font_size(11)
        for value in (40,70,100):
            y=bottom-(value-30)/70*(bottom-top)
            cr.set_source_rgb(.86,.89,.94);cr.move_to(left,y);cr.line_to(right,y);cr.stroke()
            cr.set_source_rgb(.4,.46,.56);cr.move_to(3,y+4);cr.show_text(str(value)+'%')
        points=[]
        for i,spin in enumerate(self.spins.get(name,[])):
            temp=TEMPS[name][i];x=left+(temp-40)/50*(right-left);y=bottom-(spin.get_value()-30)/70*(bottom-top);points.append((x,y))
        cr.set_source_rgb(*((.23,.45,.85) if name=='cpu_fan' else (.58,.34,.79)));cr.set_line_width(2.5)
        for i,(x,y) in enumerate(points):
            if i==0:cr.move_to(x,y)
            else:cr.line_to(x,y)
        cr.stroke()
        for x,y in points:cr.arc(x,y,3,0,6.284);cr.fill()
        cr.set_source_rgb(.4,.46,.56)
        for temp in (40,60,80,90):cr.move_to(left+(temp-40)/50*(right-left)-9,height-6);cr.show_text(str(temp)+' C')


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
        row = Gtk.Grid(column_spacing=12, row_spacing=12, column_homogeneous=True)
        for index, (mode, title, color, css) in enumerate([('office','办公模式','绿色指示灯','green-led'),
                                       ('balanced','均衡模式','蓝色指示灯','blue-led'),
                                       ('turbo','狂暴模式','紫色指示灯','purple-led'),
                                       ('custom','自定义模式','白色指示灯','white-led')]):
            button = Gtk.Button(); button.add_css_class('mode-card')
            content = Gtk.Box(orientation=Gtk.Orientation.VERTICAL, spacing=12)
            name = Gtk.Label(label=title, xalign=0); name.add_css_class('mode-name')
            led = Gtk.Label(label='● '+color, xalign=0); led.add_css_class('mode-led'); led.add_css_class(css)
            content.append(name); content.append(led); button.set_child(content)
            button.set_sensitive(False)
            button.connect('clicked', lambda _button, selected=mode: self.request(['set', selected]))
            row.attach(button, index % 2, index // 2, 1, 1); self.buttons.append((mode, button))
        self.append(row)
        self.message = Gtk.Label(xalign=0, wrap=True)
        self.message.add_css_class('status-text'); self.append(self.message)
        note = Gtk.Label(label='点击模式卡片立即切换。机身模式键仍可使用，当前状态每 3 秒刷新。', xalign=0, wrap=True)
        note.add_css_class('subtitle'); self.append(note)
        note2 = Gtk.Label(label='启动、开机与唤醒时保持当前档位，不自动切换。', xalign=0, wrap=True)
        note2.add_css_class('subtitle'); self.append(note2)
        self.tuning = TuningEditor(self.request)
        self.append(self.tuning)
        pending = Gtk.Label(label='狂暴子模式研究：CPU 温度阈值已验证为静音 87°C / 高能 95°C；GPU 功耗与整机噪声效果尚未验收，暂不提供完整子模式切换。', xalign=0, wrap=True)
        pending.add_css_class('subtitle'); self.append(pending)
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
        self.tuning.set_available(False, False)
        if args[0] != 'status':
            self.message.set_text('正在应用设置并核对硬件回读…')
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
            self.current.set_text('当前：' + LABELS.get(mode, '未知档位') + (' · 已应用调校' if data.get('tuning_active') else ''))
            self.tuning.update_status(data)
            self.message.set_text(data['reason'] or ('设置已生效，硬件回读一致' if operation != 'status' else '已连接固件档位控制'))
            for value, button in self.buttons:
                button.set_sensitive(self.switchable and value != mode)
                if value == mode:
                    button.add_css_class('selected')
                else:
                    button.remove_css_class('selected')
        except Exception as exc:
            self.switchable = False
            self.tuning.set_available(False, False)
            self.current.set_text('当前：无法读取')
            self.message.set_text(str(exc))

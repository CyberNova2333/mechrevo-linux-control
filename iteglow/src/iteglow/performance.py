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
        self.gpu_supported = False; self.available = False
        self.add_css_class('control-card')
        title=Gtk.Label(label='自定义调校', xalign=0); title.add_css_class('section-title'); self.append(title)
        note=Gtk.Label(label='应用后进入白色自定义档。切换档位、按机身模式键或休眠前会撤销调校；关闭窗口会保留设置。',xalign=0,wrap=True)
        note.add_css_class('subtitle');self.append(note)
        row=Gtk.Box(spacing=12)
        label=Gtk.Label(label='CPU 功耗上限',xalign=0);label.set_hexpand(True);row.append(label)
        self.power=Gtk.SpinButton.new_with_range(25,75,1);self.power.set_value(35);row.append(self.power)
        row.append(Gtk.Label(label='W'));self.append(row)
        note=Gtk.Label(label='已实测 25–75 W；持续与短时上限同步设置。',xalign=0,wrap=True)
        note.add_css_class('small-note');self.append(note)
        row=Gtk.Box(spacing=12)
        self.gpu_enabled=Gtk.CheckButton(label='同时设置 GPU 功耗');self.gpu_enabled.set_hexpand(True)
        self.gpu_enabled.set_sensitive(False);row.append(self.gpu_enabled)
        self.gpu_power=Gtk.SpinButton.new_with_range(65,115,1);self.gpu_power.set_value(115)
        self.gpu_power.set_numeric(True);self.gpu_power.set_sensitive(False);row.append(self.gpu_power)
        row.append(Gtk.Label(label='W'));self.append(row)
        self.gpu_note=Gtk.Label(label='正在确认 GPU 调节支持。未勾选时由固件管理 GPU 功耗。',xalign=0,wrap=True)
        self.gpu_note.add_css_class('small-note');self.append(self.gpu_note)
        row=Gtk.Box(spacing=12)
        self.gpu_db_enabled=Gtk.CheckButton(label='Dynamic Boost · 额外 GPU 功耗')
        self.gpu_db_enabled.set_hexpand(True);row.append(self.gpu_db_enabled)
        self.gpu_db=Gtk.SpinButton.new_with_range(5,25,1);self.gpu_db.set_value(5);self.gpu_db.set_numeric(True)
        row.append(self.gpu_db);row.append(Gtk.Label(label='W'));self.append(row)
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
        self.gpu_enabled.connect('toggled',self.refresh_gpu)
        self.gpu_db_enabled.connect('toggled',self.refresh_gpu)
        saved=state.load(str(PREFS))
        if saved:self.load_values(saved)

    def load_values(self,config):
        try:
            power=int(config['pl1']);curves=[config[k] for k in self.spins]
            if not 25<=power<=75 or any(len(c)!=11 or any(type(v) is not int or not MIN_DUTY[i]<=v<=100 for i,v in enumerate(c)) for c in curves):return
            self.power.set_value(power)
            gpu=config.get('gpu_ctgp')
            if gpu is None:
                self.gpu_enabled.set_active(False);self.gpu_db_enabled.set_active(False)
            elif type(gpu) is int and 65<=gpu<=115:
                self.gpu_power.set_value(gpu);self.gpu_enabled.set_active(True)
                boost=config.get('gpu_db',0)
                if type(boost) is int and 5<=boost<=25:
                    self.gpu_db.set_value(boost);self.gpu_db_enabled.set_active(True)
                else:self.gpu_db_enabled.set_active(False)
            for name,spins in self.spins.items():
                for spin,v in zip(spins,config[name]):spin.set_value(v)
        except (KeyError,TypeError,ValueError):pass

    def set_available(self,available,active):
        self.available=bool(available)
        self.refresh_gpu();self.reset_button.set_sensitive(active)

    def refresh_gpu(self,*_):
        selected=self.gpu_enabled.get_active()
        self.gpu_enabled.set_sensitive(self.gpu_supported or selected)
        self.gpu_power.set_sensitive(self.gpu_supported and selected)
        self.gpu_db_enabled.set_sensitive(self.gpu_supported and selected)
        self.gpu_db.set_sensitive(self.gpu_supported and selected and self.gpu_db_enabled.get_active())
        self.apply_button.set_sensitive(self.available and (not selected or self.gpu_supported))

    def update_status(self,data):
        self.gpu_supported=(data.get('gpu_tuning_supported') is True
                            and data.get('gpu_tuning_range')==[65,115] and data.get('gpu_db_range')==[5,25])
        self.gpu_note.set_text('基础功耗 65–115 W；Dynamic Boost 可额外分配 5–25 W，实际用量随负载变化。未勾选 GPU 调节时由固件管理。'
                               if self.gpu_supported else data.get('gpu_tuning_reason') or 'GPU 调节尚未就绪；可继续设置 CPU 和风扇。')
        self.set_available(data['switchable'],data.get('tuning_active',False) or data.get('tuning_recovery_pending',False))
        if not self.initialized:
            if data.get('tuning'):self.load_values(data['tuning'])
            self.initialized=True
        config=data.get('tuning')
        self.validation.set_text(('硬件当前已应用：CPU '+str(config['pl1'])+' W'
                                 +(' · GPU '+str(config['gpu_ctgp'])+' W' if config.get('gpu_ctgp') is not None else '')
                                 +(' + '+str(config['gpu_db'])+' W Boost' if config.get('gpu_db',0) else '')
                                 +' · 独立双风扇曲线') if config else '编辑不会立即改变硬件，点击“应用”后生效。')

    def apply(self,_):
        config={'pl1':self.power.get_value_as_int(),'pl2':self.power.get_value_as_int()}
        config.update({name:[spin.get_value_as_int() for spin in spins] for name,spins in self.spins.items()})
        if self.gpu_enabled.get_active():
            if not self.gpu_supported:
                self.validation.set_text('GPU 调节状态尚未确认，请取消该项或稍后重试。');return
            config['gpu_ctgp']=self.gpu_power.get_value_as_int()
            config['gpu_db']=self.gpu_db.get_value_as_int() if self.gpu_db_enabled.get_active() else 0
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


class TurboEditor(Gtk.Box):
    def __init__(self, request):
        super().__init__(orientation=Gtk.Orientation.VERTICAL, spacing=12)
        self.add_css_class('control-card')
        title=Gtk.Label(label='狂暴子模式',xalign=0);title.add_css_class('section-title');self.append(title)
        self.current=Gtk.Label(label='正在读取调度状态…',xalign=0,wrap=True);self.append(self.current)
        self.buttons={};row=Gtk.Box(spacing=10)
        for mode,caption in [('quiet','静音狂暴'),('extreme','高能狂暴'),('off','交还固件控制')]:
            button=Gtk.Button(label=caption);button.set_sensitive(False)
            button.connect('clicked',lambda _,value=mode:request(['turbo',value]))
            row.append(button);self.buttons[mode]=button
        self.append(row)
        note=Gtk.Label(label='在狂暴档、原装圆口电源下可用。切换其他档位、拔掉电源或准备睡眠时，自动交还固件控制；唤醒后不自动重新启用。',xalign=0,wrap=True)
        note.add_css_class('small-note');self.append(note)
        budget=Gtk.Label(label='CPU 与 GPU 共享功耗预算。仅使用 GPU 时，两种子模式可能达到相同功耗。',xalign=0,wrap=True)
        budget.add_css_class('small-note');self.append(budget)

    def disable(self):
        for button in self.buttons.values():button.set_sensitive(False)

    def update_status(self,data):
        turbo=data.get('turbo')
        turbo=turbo if isinstance(turbo,dict) else {}
        active=turbo.get('active') is True
        available=turbo.get('available') is True
        recovering=turbo.get('recovery_needed') is True and not active
        uncertain=turbo.get('policy_uncertain') is True
        known=(all(type(turbo.get(key)) is bool for key in
                   ('active','recovery_needed','policy_uncertain','standard_turbo',
                    'round_adapter','combined_supported'))
               and all(type(data.get(key)) is bool for key in ('tuning_active','tuning_recovery_pending'))
               and type(turbo.get('read_rc')) is int and turbo['read_rc']==0
               and (not active or turbo.get('variant') in ('quiet','extreme')))
        supported=turbo.get('combined_supported') is True
        eligible=(available and known and supported and data.get('mode')=='turbo'
                  and turbo.get('standard_turbo') is True and turbo.get('round_adapter') is True
                  and not data['tuning_active'] and not data['tuning_recovery_pending']
                  and not recovering and not uncertain)
        variant=(turbo.get('variant') if active and turbo.get('variant') in ('quiet','extreme')
                 else 'off' if known and not active and not recovering and not uncertain else None)
        for name,button in self.buttons.items():
            # A known active lease can always be handed back even if its variant
            # or capability report is incomplete. Never infer active from truthy data.
            button.set_sensitive(bool((available and (active or recovering) and not uncertain) if name=='off' else eligible and name!=variant))
            if name==variant and available:button.add_css_class('selected')
            else:button.remove_css_class('selected')
        if uncertain:caption='固件状态无法确认，已停止软件切档；请正常重启电脑'
        elif not available and (active or recovering):caption='控制服务离线，正在等待归还控制；暂不能更改模式'
        elif not available:caption='调度服务未就绪，当前保持固件控制'
        elif recovering:caption='恢复尚未完成，请点击“交还固件控制”重试'
        elif not known:caption='调度状态尚未确认，请先交还固件控制' if active else '调度状态尚未确认，暂不能启用子模式'
        elif not supported:caption='固件未报告支持自动功耗调度，请先交还控制' if active else '固件未报告支持自动功耗调度'
        elif active:caption='当前：'+('静音狂暴' if variant=='quiet' else '高能狂暴')+' · 自动分配 CPU / GPU 功耗'
        elif data.get('mode')!='turbo':caption='先切换到上方的狂暴模式'
        elif not turbo.get('round_adapter'):caption='请使用原装圆口电源'
        else:caption='当前：由固件控制，尚未启用子模式调度'
        self.current.set_text(caption)


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
        self.turbo = TurboEditor(self.request)
        self.append(self.turbo);self.append(self.tuning)
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
        self.turbo.disable()
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
            self.turbo.update_status(data)
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
            self.turbo.disable()
            self.current.set_text('当前：无法读取')
            self.message.set_text(str(exc))

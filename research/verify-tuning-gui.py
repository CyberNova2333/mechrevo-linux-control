import sys,subprocess,json,time
from pathlib import Path
sys.path.insert(0,'/home/cybernova/.local/share/desktop-maintenance/sources/iteglow-0.3.2/src')
import gi
gi.require_version('Gtk','4.0');from gi.repository import Gtk,Gdk,GLib
from iteglow.performance import PerformancePanel
helper=['/usr/bin/pkexec','--disable-internal-agent','/usr/local/libexec/mechrevo-profile-helper']
original=json.loads(subprocess.check_output(helper+['status'],text=True))['mode'];assert original in ('office','balanced','turbo','custom')
output=Path('/home/cybernova/.local/share/desktop-maintenance/changes/2026-11-14-power-fan-validation')
rows=[];queue=['custom','apply','reset',original];pending=None;deadline=time.monotonic()+45;failure=[]
app=Gtk.Application(application_id='org.cybernova.MechrevoVerify')
def activate(app):
 global panel,window
 provider=Gtk.CssProvider();provider.load_from_path('/home/cybernova/.local/share/desktop-maintenance/sources/iteglow-0.3.2/src/iteglow/control.css')
 Gtk.StyleContext.add_provider_for_display(Gdk.Display.get_default(),provider,Gtk.STYLE_PROVIDER_PRIORITY_USER+1)
 window=Gtk.ApplicationWindow(application=app,title='机械革命 · 性能模式验证');window.set_default_size(960,1000)
 shell=Gtk.Box(orientation=Gtk.Orientation.VERTICAL);shell.add_css_class('mechrevo-shell');shell.set_margin_top(24);shell.set_margin_start(24);shell.set_margin_end(24);shell.set_margin_bottom(24)
 panel=PerformancePanel();panel.tuning.expander.set_expanded(True);scroll=Gtk.ScrolledWindow();scroll.set_child(panel);shell.append(scroll);scroll.set_vexpand(True);window.set_child(shell);window.present();GLib.timeout_add(600,tick)
def tick():
 global pending
 try:
  if time.monotonic()>deadline:raise RuntimeError('GUI timeout')
  if panel.busy:return True
  if pending is not None:
   data=json.loads(subprocess.check_output(helper+['status'],text=True))
   assert data['switchable'],data
   if pending=='apply':assert data['tuning_active'] and data['tuning']['pl1']==75,data
   elif pending=='reset':assert not data['tuning_active'],data
   else:assert data['mode']==pending,data
   assert '无法读取' not in panel.current.get_text()
   rows.append({'selected':pending,'current_label':panel.current.get_text(),'message':panel.message.get_text(),'hardware':data})
   if pending=='apply':
    subprocess.run(['scrot','-o',str(output/'gui-tuning-verified.png')],check=True)
   pending=None
  if not queue:app.quit();return False
  target=queue.pop(0)
  current=json.loads(subprocess.check_output(helper+['status'],text=True))['mode']
  if target=='apply':
   panel.tuning.power.set_value(75);panel.tuning.apply_button.emit('clicked')
  elif target=='reset':
   assert panel.tuning.reset_button.get_sensitive();panel.tuning.reset_button.emit('clicked')
  elif target!=current:
   button=dict(panel.buttons)[target];assert button.get_sensitive(),target;button.emit('clicked')
  pending=target;return True
 except Exception as exc:failure.append(str(exc));app.quit();return False
app.connect('activate',activate)
try:app.run([])
finally:
 subprocess.run(helper+['reset'],check=True,stdout=subprocess.DEVNULL)
 subprocess.run(helper+['set',original],check=True,stdout=subprocess.DEVNULL)
 (output/'gui-verification.json').write_text(json.dumps({'original':original,'steps':rows,'errors':failure},ensure_ascii=False,indent=2))
print(json.dumps({'steps':len(rows),'errors':failure,'restored':original},ensure_ascii=False))
if failure:raise SystemExit(1)

import sys,subprocess,json
from pathlib import Path
sys.path.insert(0,'/home/cybernova/.local/share/desktop-maintenance/sources/iteglow-0.3.2/src')
from iteglow.gui import App
from gi.repository import GLib
app=App();app.set_application_id('org.cybernova.TuningLayoutVerify');out=Path('/home/cybernova/.local/share/desktop-maintenance/changes/2026-11-14-power-fan-validation');errors=[]
def setup():
 app.navigate('performance');scroll=app.stack.get_child_by_name('performance');page=scroll.get_child().get_child();page.tuning.expander.set_expanded(True)
 GLib.timeout_add(1200,capture);return False
def capture():
 try:
  scroll=app.stack.get_child_by_name('performance');page=scroll.get_child().get_child();window=app.get_active_window()
  assert window.get_width()<=1100,window.get_width()
  assert page.get_width()<=scroll.get_width(),(page.get_width(),scroll.get_width())
  adj=scroll.get_vadjustment();adj.set_value(adj.get_upper()-adj.get_page_size())
  GLib.timeout_add(600,finish)
 except Exception as e:errors.append(str(e));app.quit()
 return False
def finish():
 subprocess.run(['scrot','-o',str(out/'gui-tuning-layout.png')],check=True);app.quit();return False
app.connect('activate',lambda _:GLib.timeout_add(800,setup));app.run([])
print(json.dumps({'layout_errors':errors}));assert not errors

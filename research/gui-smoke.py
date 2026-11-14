import sys,json,time
from pathlib import Path
sys.path.insert(0, '/home/cybernova/.local/share/desktop-maintenance/sources/iteglow-0.3.2/src')
import gi
gi.require_version('Gtk','4.0')
from gi.repository import Gtk,GLib
from iteglow.performance import PerformancePanel
Gtk.init()
panel=PerformancePanel()
window=Gtk.Window(title='机械革命性能页验证')
window.set_default_size(620,380)
window.set_child(panel)
window.present()
loop=GLib.MainLoop()
steps=iter(['office','balanced','turbo'])
pending=None
results=[]
deadline=time.monotonic()+20
def tick():
 global pending
 if time.monotonic()>deadline:
  results.append({'error':'timeout'})
  loop.quit();return False
 if panel.busy:return True
 if not panel.switchable:
  results.append({'error':panel.message.get_text()})
  loop.quit();return False
 if pending is not None:
  results.append({'requested':pending,'label':panel.current.get_text(),'message':panel.message.get_text()})
  if pending=='turbo':
   loop.quit();return False
 pending=next(steps)
 panel.request(['set',pending])
 return True
GLib.timeout_add(500,tick)
try:loop.run()
finally:
 import subprocess
 subprocess.run(['/usr/bin/pkexec','--disable-internal-agent','/usr/local/libexec/mechrevo-profile-helper','set','turbo'],check=True)
 window.destroy()
Path(__file__).with_suffix('.json').write_text(json.dumps(results,ensure_ascii=False,indent=2))
print(json.dumps(results,ensure_ascii=False))
assert len(results)==3 and all('error' not in r and '切换成功' in r['message'] for r in results),results

import sys,subprocess,time,json
from pathlib import Path
sys.path.insert(0,'/home/cybernova/.local/share/desktop-maintenance/sources/iteglow-0.3.2/src')
from iteglow import devices,protocol
results=[]
with devices.HidLed(devices.led_for_label('keyboard')) as dev:
 original=protocol.parse_keyboard_state(dev.query(protocol.QUERY_EFFECT))
 assert original['mode']==2,'Expected current breathing mode; stop rather than overwrite another selection'
 try:
  for name,x,y,code in [('reactive',750,945,4),('aurora',525,1180,14),('spark',750,1180,17)]:
   subprocess.run(['xdotool','mousemove','--window','146800644',str(x),str(y),'click','1'],check=True)
   time.sleep(.35)
   actual=protocol.parse_keyboard_state(dev.query(protocol.QUERY_EFFECT));results.append({'name':name,'actual':actual})
   assert actual['mode']==code and actual['direction']==0 and actual['brightness']==original['brightness'],actual
  subprocess.run(['scrot','-w','146800644','-o',str(Path(__file__).with_name('spark-ui.png'))],check=True)
 finally:
  subprocess.run(['xdotool','mousemove','--window','146800644','975','825','click','1'],check=True)
  time.sleep(.35)
  actual=protocol.parse_keyboard_state(dev.query(protocol.QUERY_EFFECT));results.append({'restored':actual})
  assert actual['mode']==2,actual
Path(__file__).with_suffix('.json').write_text(json.dumps(results,ensure_ascii=False,indent=2))
print(json.dumps(results,ensure_ascii=False))

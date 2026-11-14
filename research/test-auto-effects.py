import sys,time,json,signal
sys.path.insert(0,'/home/cybernova/.local/share/desktop-maintenance/sources/iteglow-0.3.2/src')
from iteglow import effects,protocol,devices,state
from iteglow.cli import _build_from_params
saved=state.load(state.user_state_path()).get('keyboard',{'mode':'off'})
with devices.HidLed(devices.led_for_label('keyboard')) as dev:
 original=protocol.parse_keyboard_state(dev.query(protocol.QUERY_EFFECT))
 restore=_build_from_params('keyboard',saved)
 if original['brightness']==0 or original['power']==1:restore=protocol.keyboard_off()
 signal.signal(signal.SIGTERM,lambda *_:(_ for _ in ()).throw(KeyboardInterrupt()))
 try:
  for mode in ['reactive','aurora','spark']:
   reports=effects.build('keyboard',mode,brightness=30,speed=4,direction=1)
   kind,data=reports[-1];data=bytearray(data);data[6]=0;reports[-1]=(kind,bytes(data))
   dev.send(reports)
   print(json.dumps({'time':time.strftime('%H:%M:%S'),'mode':mode,'readback':protocol.parse_keyboard_state(dev.query(protocol.QUERY_EFFECT))}),flush=True)
   time.sleep(8)
 finally:
  dev.send(restore)
  print(json.dumps({'restored':protocol.parse_keyboard_state(dev.query(protocol.QUERY_EFFECT))}),flush=True)

import os,json,time,signal
from pathlib import Path
assert Path('/sys/class/dmi/id/board_name').read_text().strip()=='YAOSHI Series-XxAF55xx'
assert Path('/sys/class/dmi/id/bios_version').read_text().strip()=='N.1.33MRO91'
assert 'fe410000-fe410fff : INOU0000:00' in Path('/proc/iomem').read_text()
f=os.open('/dev/mem',os.O_RDWR|os.O_CLOEXEC);address=0xfe410751
read=lambda:os.pread(f,1,address)[0]
old=read();assert old in [0xa0,0x20,0x30],hex(old)
last=old
signal.signal(signal.SIGTERM,lambda *_:(_ for _ in ()).throw(KeyboardInterrupt()))
try:
 for name,value in [('office',0xa0),('balanced',0x20),('turbo',0x30)]:
  assert read()==last,'Mode changed externally; stopping'
  os.pwrite(f,bytes([value]),address);last=value;time.sleep(.2)
  actual=read();print(json.dumps({'time':time.strftime('%H:%M:%S'),'mode':name,'requested':hex(value),'readback':hex(actual)}),flush=True)
  assert actual==value,'Write did not persist'
  time.sleep(8)
finally:
 if read()==last:
  os.pwrite(f,bytes([old]),address);print(json.dumps({'restored':hex(read())}),flush=True)
 os.close(f)

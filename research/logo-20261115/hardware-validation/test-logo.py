import os,fcntl,time,json,signal,runpy
from pathlib import Path
helper=runpy.run_path('/usr/local/libexec/mechrevo-profile-helper');helper['check_machine']()
lock=os.open('/run/mechrevo-profile.lock',os.O_RDWR|os.O_NOFOLLOW);fcntl.flock(lock,fcntl.LOCK_EX|fcntl.LOCK_NB)
fd=os.open('/dev/mem',os.O_RDWR);base=0xfe410000
masks={0x767:128,0x769:255,0x76a:255,0x76b:255,0x7ab:224,0x7c5:7}
read=lambda a:os.pread(fd,1,base+a)[0]
def write(a,v):assert os.pwrite(fd,bytes([v]),base+a)==1
def masked(a,v):write(a,(read(a)&~masks[a])|(v&masks[a]))
def commit():write(0x78c,read(0x78c)|16)
before={a:read(a) for a in masks};assert before[0x7ab]&1
rows=[]
signal.signal(signal.SIGTERM,lambda *_:(_ for _ in ()).throw(KeyboardInterrupt()))
try:
 time.sleep(8)
 for name,rgb in [('white',[50,50,50]),('red',[50,0,0]),('blue',[0,0,50]),('off',None)]:
  if rgb:
   masked(0x767,0);masked(0x7c5,0)
   for a,v in zip([0x769,0x76a,0x76b],rgb):masked(a,v)
  masked(0x7ab,32 if rgb else 0);commit()
  row={'phase':name,'time':time.time(),'registers':{hex(a):read(a) for a in masks}};rows.append(row);print(json.dumps(row),flush=True)
  time.sleep(10)
finally:
 for a,v in before.items():masked(a,v)
 commit();time.sleep(.4)
 after={a:read(a) for a in masks}
 Path(__file__).with_name('logo-trial.json').write_text(json.dumps({'before':before,'after':after,'samples':rows,'all_owned_bits_restored':all((after[a]&masks[a])==(v&masks[a]) for a,v in before.items())},indent=2))
 os.close(fd);os.close(lock);print('Restored original logo settings',flush=True)

"""Observe existing, previously tested OEM submode notifications without GPU load."""
from pathlib import Path
import subprocess,os,signal,time,json,fcntl
root=Path(__file__).resolve().parent;bridge=Path('/sys/kernel/mechrevo_turbo')
module=root/'bridge-build/mechrevo_turbo.ko'
assert os.uname().release=='6.8.0-149-generic'
assert not bridge.exists()
status=json.loads(subprocess.check_output(['/usr/local/libexec/mechrevo-profile-helper','status']))
assert status['mode']=='turbo' and status['switchable'] and not status['tuning_active']
assert json.loads(subprocess.check_output(['/usr/local/libexec/mechrevo-acpi-check']))['healthy']
pid=int(subprocess.check_output(['systemctl','show','nvidia-powerd','--property=MainPID','--value']));assert pid>1
lock=os.open('/run/mechrevo-profile.lock',os.O_RDWR|os.O_NOFOLLOW);fcntl.flock(lock,fcntl.LOCK_EX|fcntl.LOCK_NB)
proc=None;loaded=False;before=None;rows=[];changed=False
fd=os.open('/dev/mem',os.O_RDWR)
original={r:os.pread(fd,1,0xfe410000+r) for r in [0x741,0x786]}
def snapshot(label):
 row={'phase':label,'time':time.time(),'firmware':(bridge/'status').read_text()};rows.append(row);print(label,flush=True);return dict(line.split('=') for line in row['firmware'].splitlines())
try:
 subprocess.run(['insmod',str(module)],check=True);loaded=True
 before=snapshot('before');assert not int(before['728'],16)&1 and not int(before['741'],16)&1
 with (root/'nvpcf-switch.txt').open('w') as log:
  proc=subprocess.Popen(['gdb','-q','-nx','-batch','-p',str(pid),'-x',str(root/'observe-nvpcf.gdb')],stdout=log,stderr=log,start_new_session=True)
  time.sleep(2)
  changed=True;os.pwrite(fd,bytes([original[0x741][0]|1]),0xfe410741);(bridge/'workload').write_text('game');(bridge/'variant').write_text('quiet');snapshot('quiet');time.sleep(5)
  (bridge/'variant').write_text('extreme');snapshot('extreme');time.sleep(5)
finally:
 if proc and proc.poll() is None:
  os.killpg(proc.pid,signal.SIGINT)
  try:proc.wait(timeout=5)
  except subprocess.TimeoutExpired:os.killpg(proc.pid,signal.SIGKILL);proc.wait()
 if loaded:
  try:
   if before and changed:(bridge/'variant').write_text('quiet' if int(before['728'],16)&1 else 'extreme')
   (bridge/'workload').write_text('restore')
   for r,b in original.items():os.pwrite(fd,b,0xfe410000+r)
   time.sleep(.5);snapshot('restored')
  finally:subprocess.run(['rmmod','mechrevo_turbo'],check=True)
 os.close(fd);os.close(lock)
 (root/'switch-firmware.json').write_text(json.dumps(rows,indent=2))
 assert subprocess.run(['systemctl','is-active','--quiet','nvidia-powerd']).returncode==0

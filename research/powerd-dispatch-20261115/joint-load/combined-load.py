"""Bounded, guarded A/B of verified OEM game-budget requests; always restore."""
from pathlib import Path
import subprocess,os,signal,time,json,fcntl,hashlib
root=Path(__file__).resolve().parent;bridge=Path('/sys/kernel/mechrevo_turbo')
module=Path('/home/cybernova/.cache/mechrevo-linux-research/firmware-link-20261115/bridge-build/mechrevo_turbo.ko')
assert hashlib.sha256(Path('/usr/bin/nvidia-powerd').read_bytes()).hexdigest()=='db2643c4c37360e183230ea7f9c5da3e28c39ff79d08eba6ddb0f4f057d7bc74'
assert os.uname().release=='6.8.0-149-generic' and not bridge.exists()
status=json.loads(subprocess.check_output(['/usr/local/libexec/mechrevo-profile-helper','status']))
assert status['mode']=='turbo' and status['switchable'] and not status['tuning_active']
assert json.loads(subprocess.check_output(['/usr/local/libexec/mechrevo-acpi-check']))['healthy']
pid=int(subprocess.check_output(['systemctl','show','nvidia-powerd','--property=MainPID','--value']));assert pid>1
lock=os.open('/run/mechrevo-profile.lock',os.O_RDWR|os.O_NOFOLLOW);fcntl.flock(lock,fcntl.LOCK_EX|fcntl.LOCK_NB)
fd=os.open('/dev/mem',os.O_RDWR);original={r:os.pread(fd,1,0xfe410000+r) for r in [0x741,0x786]}
cpuhw=next(p for p in Path('/sys/class/hwmon').glob('hwmon*') if (p/'name').read_text().strip()=='coretemp')
rapl=Path('/sys/class/powercap/intel-rapl:0/energy_uj');maximum=int(rapl.with_name('max_energy_range_uj').read_text())
proc=None;loaded=False;before=None;rows=[];loads=[];logs=[]
def stop(*_):raise KeyboardInterrupt
signal.signal(signal.SIGTERM,stop);signal.signal(signal.SIGINT,stop)
def snap(label):
 row={'phase':label,'time':time.time(),'firmware':(bridge/'status').read_text()};rows.append(row);return dict(line.split('=') for line in row['firmware'].splitlines())
try:
 subprocess.run(['insmod',str(module)],check=True);loaded=True;before=snap('before')
 assert not int(before['728'],16)&1 and not int(before['741'],16)&1
 log=(root/'qboost-load.txt').open('w');logs.append(log)
 proc=subprocess.Popen(['gdb','-q','-nx','-batch','-p',str(pid),'-x',str(root/'observe-qboost-load.gdb')],stdout=log,stderr=log,start_new_session=True)
 time.sleep(2)
 os.pwrite(fd,bytes([original[0x741][0]|1]),0xfe410741);(bridge/'workload').write_text('game');(bridge/'variant').write_text('quiet')
 scripts=[Path('/home/cybernova/.cache/mechrevo-linux-research/powerd-coldboot-20261115/gpu-load.py'),Path('/home/cybernova/.cache/mechrevo-linux-research/vendor/cpu-load.py')]
 for script in scripts:
  path=root/('joint-'+script.stem+'.log');log=path.open('w');logs.append(log)
  p=subprocess.Popen(['/usr/bin/sudo','-u','cybernova','/home/cybernova/AI/workbench/.venv/bin/python',str(script)],stdout=log,stderr=log,start_new_session=True);loads.append(p)
  for _ in range(30):
   if 'READY' in path.read_text():break
   if p.poll() is not None:raise RuntimeError('load exited: '+str(script))
   time.sleep(.5)
  else:raise RuntimeError('load readiness timeout')
 prev_e=int(rapl.read_text());prev_t=time.monotonic()
 for index,mode in enumerate(['quiet','extreme','quiet','extreme']):
  (bridge/'variant').write_text(mode);snap(mode+'-start');print(mode+' round '+str(index+1),flush=True)
  for n in range(10):
   time.sleep(2)
   gpu=subprocess.check_output(['nvidia-smi','--query-gpu=temperature.gpu,power.draw,utilization.gpu,clocks.gr','--format=csv,noheader,nounits'],text=True).strip()
   temp=max(int(p.read_text())/1000 for p in cpuhw.glob('temp*_input'))
   now_e=int(rapl.read_text());now_t=time.monotonic();watts=((now_e-prev_e)%maximum)/1e6/(now_t-prev_t);prev_e,prev_t=now_e,now_t
   row={'phase':mode,'round':index,'sample':n,'time':time.time(),'cpu_temp':temp,'cpu_W':watts,'gpu':gpu};rows.append(row);print(json.dumps(row),flush=True)
   if temp>=97 or float(gpu.split(',')[0])>=78:raise RuntimeError('temperature guard')
   if any(p.poll() is not None for p in loads):raise RuntimeError('load ended early')
finally:
 for p in loads:
  if p.poll() is None:
   os.killpg(p.pid,signal.SIGTERM)
   try:p.wait(timeout=8)
   except subprocess.TimeoutExpired:os.killpg(p.pid,signal.SIGKILL);p.wait()
 if proc and proc.poll() is None:
  os.killpg(proc.pid,signal.SIGINT)
  try:proc.wait(timeout=5)
  except subprocess.TimeoutExpired:os.killpg(proc.pid,signal.SIGKILL);proc.wait()
 if loaded:
  try:
   if before:(bridge/'variant').write_text('quiet' if int(before['728'],16)&1 else 'extreme')
   (bridge/'workload').write_text('restore')
   for r,b in original.items():os.pwrite(fd,b,0xfe410000+r)
   time.sleep(.5);after=snap('restored');print('restored_equal='+str(before==after),flush=True)
  finally:subprocess.run(['rmmod','mechrevo_turbo'],check=True)
 os.close(fd);os.close(lock)
 for log in logs:log.close()
 (root/'qboost-load-samples.json').write_text(json.dumps(rows,indent=2))
 assert subprocess.run(['systemctl','is-active','--quiet','nvidia-powerd']).returncode==0

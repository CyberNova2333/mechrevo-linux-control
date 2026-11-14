import os,json,subprocess,time
from pathlib import Path
h='/usr/local/libexec/mechrevo-profile-helper'
run=lambda *args:json.loads(subprocess.check_output([h,*args],text=True))
config={'pl1':25,'pl2':25,'cpu_fan':[40,40,40,40,50,55,60,70,80,90,100],'gpu_fan':[40,40,40,40,50,55,60,70,80,90,100]}
original=run('status')['mode'];rows=[]
try:
 for action in ('mode_key','sleep_cleanup','guard_restart'):
  assert run('apply',json.dumps(config))['tuning_active']
  if action=='mode_key':
   f=os.open('/dev/mem',os.O_RDWR)
   for a,mask in ((0x726,128),(0x727,64)):
    v=os.pread(f,1,0xfe410000+a)[0];os.pwrite(f,bytes([v&~mask]),0xfe410000+a)
   os.pwrite(f,b'\x20',0xfe410751);os.close(f);time.sleep(2)
  elif action=='sleep_cleanup':subprocess.run(['systemctl','start','mechrevo-profile-sleep.service'],check=True)
  else:subprocess.run(['systemctl','restart','mechrevo-profile-guard.service'],check=True)
  result=run('status');assert result['switchable'] and not result['tuning_active'] and not result['tuning_recovery_pending'],result
  if action=='mode_key':assert result['mode']=='balanced',result
  rows.append({'action':action,'status':result})
  run('set',original)
finally:run('reset');run('set',original)
Path('/home/cybernova/.local/share/desktop-maintenance/changes/2026-11-14-power-fan-validation/recovery-verification.json').write_text(json.dumps(rows,ensure_ascii=False,indent=2))
print(json.dumps(rows,ensure_ascii=False))

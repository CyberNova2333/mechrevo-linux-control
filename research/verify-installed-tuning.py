import json,time,subprocess,os,signal
from pathlib import Path
helper='/usr/local/libexec/mechrevo-profile-helper';rows=[]
run=lambda *args:json.loads(subprocess.check_output([helper,*args],text=True))
original=run('status');assert original['mode']=='turbo' and not original['tuning_active']
f=os.open('/dev/mem',os.O_RDONLY);base=0xfe410000
read=lambda a:os.pread(f,1,base+a)[0]
registers=[0x726,0x727,0x728,0x741,0x751,0x783,0x784,0x785,0x786,0x787,0x7c5,0x7c6,0x7ab]
snap=lambda:{hex(a):read(a) for a in registers}
before=snap();table_before=list(os.pread(f,96,base+0xf00))
sensors=[p for h in Path('/sys/class/hwmon').glob('hwmon*') if (h/'name').read_text().strip()=='coretemp' for p in h.glob('temp*_input')]
mins=[40,40,40,40,50,55,60,70,80,90,100]
def stop(*_):raise KeyboardInterrupt
signal.signal(signal.SIGTERM,stop)
try:
 for cpu,gpu in [(40,70),(70,40),(40,70)]:
  config={'pl1':25,'pl2':25,'cpu_fan':[max(v,cpu) for v in mins],'gpu_fan':[max(v,gpu) for v in mins]}
  result=run('apply',json.dumps(config));assert result['tuning_active'],result
  for _ in range(5):
   time.sleep(4)
   row={'phase':f'cpu{cpu}_gpu{gpu}','cpu_temp':max(int(p.read_text())/1000 for p in sensors),'cpu_rpm':read(0x464)*256+read(0x465),'gpu_candidate':read(0x46c)*256+read(0x46d),'raw_46b_46d':[read(a) for a in (0x46b,0x46c,0x46d)]};rows.append(row);print(row,flush=True)
   if row['cpu_temp']>=85:raise RuntimeError('temperature guard')
finally:
 run('reset');run('set',original['mode']);after=snap();table_after=list(os.pread(f,96,base+0xf00));os.close(f)
 Path('/home/cybernova/.cache/mechrevo-linux-research/vendor/installed-tuning-trial.json').write_text(json.dumps({'before':before,'after':after,'table_before':table_before,'table_after':table_after,'samples':rows},indent=2))
 print('RESTORED',before==after,table_before==table_after,flush=True)

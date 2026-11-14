import os,time,json
from pathlib import Path
f=os.open('/dev/mem',os.O_RDWR);m=os.open('/dev/cpu/0/msr',os.O_RDONLY);base=0xfe410000
r=lambda a:os.pread(f,1,base+a)[0]
w=lambda a,v:os.pwrite(f,bytes([v]),base+a)
before={a:r(a) for a in (0x741,0x786,0x728)};rows=[]
p=Path('/sys/kernel/mechrevo_turbo');assert before[0x786]==0
try:
 (p/'workload').write_text('game')
 for appe in (0,1):
  w(0x741,(r(0x741)&~1)|appe)
  for variant in ('extreme','quiet','extreme'):
   (p/'variant').write_text(variant);time.sleep(1)
   tcc=int.from_bytes(os.pread(m,8,0x1a2),'little')
   row={'appe':appe,'variant':variant,'ec_tcc':r(0x786),'msr_1a2':hex(tcc),'tcc_offset':(tcc>>24)&63,'tcc_target':(tcc>>16)&255};rows.append(row);print(row,flush=True)
finally:
 (p/'variant').write_text('quiet' if before[0x728]&1 else 'extreme');(p/'workload').write_text('restore')
 for a in (0x741,0x786):w(a,before[a])
 os.close(f);os.close(m)
 Path('/home/cybernova/.cache/mechrevo-linux-research/vendor/tcc-trial.json').write_text(json.dumps(rows,indent=2))

"""Resolve OEM delegate metadata without executing any OEM methods."""
from pathlib import Path
import dnfile,struct,json,hashlib,re
root=Path(__file__).resolve().parent
source=(root.parent/'vendor/extracted/app/AiStoneService/MyControlCenter/GCUService.exe').read_bytes()
assert hashlib.sha256(source).hexdigest()=='9ee018d42684d02032da642d90376843383223d76d82a6fba6ce42e475a82093'
p=dnfile.dnPE(data=source)
names=json.loads((root/'metadata-names.json').read_text())
def typename(row):
 return '.'.join(str(getattr(row,a,'')) for a in ['TypeNamespace','TypeName']).strip('.')
for i,row in enumerate(p.net.mdtables.MemberRef,1):
 owner=typename(row.Class.row)
 names[hex(0x0a000000+i)]=owner+'.'+str(row.Name)
config_rva=next(r.Rva for r in p.net.mdtables.FieldRva if r.Field.row_index==0x2a26)
table_rva=struct.unpack_from('<I',p.get_data(config_rva,48),4)[0]
count=struct.unpack('<I',p.get_data(table_rva,4))[0];assert count==782
rows=[]
for i in range(count):
 t,dt,flags,extra=struct.unpack('<4I',p.get_data(table_rva+8+16*i,16))
 assert hex(t) in names and hex(dt) in names
 rows.append({'index':i,'target':hex(t),'delegate':hex(dt),'flags':flags,'extra':extra,'name':names[hex(t)]})
(root/'delegate-table.json').write_text(json.dumps({'source_sha256':hashlib.sha256(source).hexdigest(),'config_field':'0x04002a26','config_rva':hex(config_rva),'table_rva':hex(table_rva),'count':count,'entries':rows},ensure_ascii=True,indent=2))
pattern=re.compile(re.escape(r'\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002')+r'\[(\d+)\]')
for f in (root/'decoded-source').glob('*.cs'):
 s=f.read_text()
 def repl(m):
  row=rows[int(m[1])];name=re.sub('[^A-Za-z0-9_]+','_',row['name'])
  return 'ResolvedDelegates.'+name+'__'+m[1]
 s=pattern.sub(repl,s)
 (root/'decoded-source'/f.name).write_text(s)
(root/'decoded-source/delegate-map.json').write_text(json.dumps(rows,ensure_ascii=True,indent=2))
print('Statically resolved',count,'delegate targets; rewritten calls remain an analysis representation, not compilable original source.')

import os,struct,json,hashlib
from pathlib import Path
out=Path(__file__).parent
s=Path('/sys/firmware/efi/systab').read_text();address=int(next(x.split('=')[1] for x in s.splitlines() if x.startswith('ACPI20=')),16)
f=os.open('/dev/mem',os.O_RDONLY|os.O_CLOEXEC)
def read(a,n):
 assert 0x61285000<=a and a+n<=0x615dd000,(hex(a),n)
 b=os.pread(f,n,a);assert len(b)==n;return b
rsdp=read(address,36);assert rsdp[:8]==b'RSD PTR ' and sum(rsdp)%256==0
xsdt=struct.unpack_from('<Q',rsdp,24)[0];head=read(xsdt,36);assert head[:4]==b'XSDT';table=read(xsdt,struct.unpack_from('<I',head,4)[0]);assert sum(table)%256==0
rows=[]
for offset in range(36,len(table),8):
 a=struct.unpack_from('<Q',table,offset)[0];h=read(a,36);n=struct.unpack_from('<I',h,4)[0]
 b=read(a,n);row={'address':hex(a),'signature':h[:4].decode('ascii','replace'),'oem_id':h[16:24].decode('ascii','replace'),'length':n,'checksum':sum(b)%256};rows.append(row)
 if h[:4]==b'SSDT' and b'NPCF' in b:
  path=out/('current-'+h[16:24].decode('ascii').strip()+'.aml');path.write_bytes(b);row['npcf_file']=path.name;row['sha256']=hashlib.sha256(b).hexdigest()
os.close(f);(out/'current-xsdt.json').write_text(json.dumps({'rsdp':hex(address),'xsdt':hex(xsdt),'tables':rows},indent=2));print(json.dumps(rows,indent=2))

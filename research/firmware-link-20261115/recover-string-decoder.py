from pathlib import Path
import hashlib, json, runpy, struct, dnfile
from dncil.cil.body import CilMethodBody
from dncil.cil.body.reader import CilMethodBodyReaderBytes
root=Path(__file__).resolve().parent
vendor=root.parent/'vendor'
source=(vendor/'extracted/app/AiStoneService/MyControlCenter/GCUService.exe').read_bytes()
assert hashlib.sha256(source).hexdigest()=='9ee018d42684d02032da642d90376843383223d76d82a6fba6ce42e475a82093'
old=root.parent/'mechrevo-drivers/scripts/windows_oem_static.py'
ns={'__name__':'qualified_static_helpers'}
exec(compile(old.read_text().replace('value == 0x0A0001DC','value == 0x0A000219').replace('value == 0x0600182F','value == 0x06001AED'),str(old),'exec'),ns)
pe=dnfile.dnPE(data=source)
field_rva=next(r.Rva for r in pe.net.mdtables.FieldRva if r.Field.row_index==0x2a28)
read=lambda r,n:ns['read_rva'](source,pe,r,n)
fixed=bytes(((b^0x3c)-0xc3)&255 for b in read(field_rva,32))
wrapper=(vendor/'recovered/0x6001c6f.bin').read_bytes()
args=ns['wrapper_builder_arguments'](wrapper,0x6001c6f)
rva,n,local_rva,local_size,fallback,key_rva,discriminator=args['args'][2:]
key=ns['rc4'](fixed,read(key_rva,16));body=ns['rc4'](key,read(rva,n))
(root/'string-decoder-inner.bin').write_bytes(body)
receipt={'source_sha256':hashlib.sha256(source).hexdigest(),'field_token':'0x04002a28','fixed_key_rva':hex(field_rva),'wrapper':args,'body_hex':body.hex(),'body_sha256':hashlib.sha256(body).hexdigest()}
try:receipt['bounds']=ns['dynamic_body_bounds'](body)
except Exception as e:receipt['error']=str(e)
(root/'string-decoder-recovery.json').write_text(json.dumps(receipt,indent=2))
print(json.dumps(receipt,indent=2))

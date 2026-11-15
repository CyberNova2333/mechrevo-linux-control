"""Static rendering of the recovered OEM string function; never loads CLR code."""
from pathlib import Path
import re,json
root=Path(__file__).resolve().parent
decoder=r'\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017'
pattern=re.compile(re.escape(decoder)+r'\("((?:\\.|[^"\\])*)"\)')
escapes={'a':'\a','b':'\b','f':'\f','n':'\n','r':'\r','t':'\t','v':'\v','0':'\0','\\':'\\','"':'"',"'":"'"}
def unescape(s):
 return re.sub(r'\\(?:u([0-9a-fA-F]{4})|x([0-9a-fA-F]{1,4})|(.))',lambda m:chr(int(m[1] or m[2],16)) if m[1] or m[2] else escapes[m[3]],s)
def decode(s):
 # CIL: (char)(s[i] + 10 + i ^ (byte)(s.Length ^ 88)).
 b=s.encode('utf-16-le','surrogatepass');v=[int.from_bytes(b[i:i+2],'little') for i in range(0,len(b),2)]
 k=(len(v)^88)&255
 return b''.join((((c+10+i)^k)&65535).to_bytes(2,'little') for i,c in enumerate(v)).decode('utf-16-le','surrogatepass')
out=root/'decoded-source';out.mkdir(exist_ok=True)
manifest=[]
for p in [root/'WMIEC.cs',root/'GpuFeatures.cs',*(root.parent/'vendor').glob('*.cs')]:
 if p.stat().st_size>20_000_000:continue
 text=p.read_text();n=0
 def render(m):
  global n
  n+=1;return json.dumps(decode(unescape(m[1])),ensure_ascii=True)
 result=pattern.sub(render,text)
 if n:
  (out/p.name).write_text('// Reconstructed analysis view; strings decoded statically; unresolved delegates remain.\n'+result)
  manifest.append({'file':p.name,'decoded_calls':n})
(out/'manifest.json').write_text(json.dumps(manifest,indent=2));print(json.dumps(manifest,indent=2))

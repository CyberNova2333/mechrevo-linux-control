set pagination off
set confirm off
set print thread-events off
python
import gdb,json,struct,time
inferior=gdb.selected_inferior()
def reg(name):return int(gdb.parse_and_eval('$'+name))
def read64(addr):return struct.unpack('<Q',inferior.read_memory(addr,8))[0]
def fields(ptr,offsets):return {hex(off):struct.unpack('<i',inferior.read_memory(ptr+off,4))[0] for off in offsets}
def output(kind,data):gdb.write('QBOOST '+json.dumps({'kind':kind,'time':time.time(),**data})+'\n')
class Dispatch(gdb.Breakpoint):
 def stop(self):
  output('dispatch',{'handler':hex(read64(reg('rax')+0x38)),'consumer':hex(reg('rdi'))})
  return False
class Worker(gdb.Breakpoint):
 def stop(self):
  output('worker',{'controller':hex(read64(reg('rbx')+0xb0))})
  return False
class Policy(gdb.Breakpoint):
 def stop(self):
  controller=reg('rbx');policy=controller+0x28
  output('policy',{'controller':hex(controller),'controller_enabled':fields(controller,[0x204]),'fields':fields(policy,[0x24,0x28,0x2c,0x30,0x34,0x38,0x3c,0x40,0x50,0x54,0x5c,0x60,0xa0,0xa4,0x108,0x10c,0x114,0x118,0x1c0,0x1c4,0x1cc,0x1d0,0x210,0x214,0x278,0x27c,0x284,0x288,0x2a8,0x2b0,0x2b4,0x2b8,0x330,0x334,0x33c,0x340])})
  return False
class Setter(gdb.Breakpoint):
 def stop(self):
  output('gpu_set',{'requested_mW':reg('edx'),'limits':fields(reg('rsi'),[0x210,0x214]),'control':fields(reg('rdi'),[0x66c,0x670,0x674,0x67c,0x11d0])})
  return False
Dispatch('*0x4c50e3',internal=True)
Worker('*0x4d46f3',internal=True)
Policy('*0x4d968d',internal=True)
Setter('*0x4dc1b0',internal=True)
end
continue
detach
quit

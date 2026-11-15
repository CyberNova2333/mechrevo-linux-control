set pagination off
set confirm off
set print thread-events off
python
import gdb,json,struct
class Done(gdb.FinishBreakpoint):
 def __init__(self,ptr,sub):
  super().__init__(gdb.newest_frame(),internal=True)
  self.ptr,self.sub=ptr,sub
 def stop(self):
  raw=bytes(gdb.selected_inferior().read_memory(self.ptr,884))
  vals=struct.unpack_from('<13i',raw)
  gdb.write('NVPCF '+json.dumps({'subfunction':self.sub,'status':int(gdb.parse_and_eval('$eax')),'supported':vals[0],'tpp':vals[2],'rated_tgp':vals[3],'ctgp_offset_mW':vals[5],'target_tpp_offset_mW':vals[6],'max_output_offset_mW':vals[7],'min_output_offset_mW':vals[8]})+'\n')
  return False
class Entry(gdb.Breakpoint):
 def stop(self):
  if int(gdb.parse_and_eval('$edx'))==0x13b and int(gdb.parse_and_eval('$r8d'))==884:
   ptr=int(gdb.parse_and_eval('$rcx'));raw=bytes(gdb.selected_inferior().read_memory(ptr,20));sub=struct.unpack_from('<I',raw,16)[0]
   Done(ptr,sub)
  return False
Entry('*0x429080',internal=True)
end
continue
detach
quit

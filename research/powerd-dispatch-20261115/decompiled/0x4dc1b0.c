
int FUN_004dc1b0(long param_1,long param_2,uint param_3)

{
  uint uVar1;
  int iVar2;
  long lVar3;
  undefined8 *puVar4;
  uint local_2bc;
  undefined1 *local_2b8 [2];
  undefined1 local_2a8 [16];
  undefined1 **local_298;
  undefined1 *local_290;
  undefined1 *local_288 [2];
  undefined8 local_278;
  undefined8 uStack_270;
  undefined8 local_268;
  undefined8 uStack_260;
  undefined8 local_258;
  undefined8 uStack_250;
  undefined8 local_248;
  undefined8 uStack_240;
  undefined8 local_238;
  undefined8 uStack_230;
  undefined8 local_228;
  undefined8 uStack_220;
  undefined8 local_218;
  undefined8 uStack_210;
  undefined8 local_208;
  undefined8 uStack_200;
  undefined8 local_1f8;
  undefined8 uStack_1f0;
  undefined8 local_1e8;
  undefined8 uStack_1e0;
  undefined8 local_1d8;
  undefined8 uStack_1d0;
  undefined8 local_1c8;
  undefined8 local_1b8;
  uint local_1ac;
  undefined1 local_1a8;
  
  local_278 = *(undefined8 *)(param_2 + 0x1b4);
  uStack_270 = *(undefined8 *)(param_2 + 0x1bc);
  local_268 = *(undefined8 *)(param_2 + 0x1c4);
  uStack_260 = *(undefined8 *)(param_2 + 0x1cc);
  local_258 = *(undefined8 *)(param_2 + 0x1d4);
  uStack_250 = *(undefined8 *)(param_2 + 0x1dc);
  local_1c8 = *(undefined8 *)(param_2 + 0x264);
  local_248 = *(undefined8 *)(param_2 + 0x1e4);
  uStack_240 = *(undefined8 *)(param_2 + 0x1ec);
  local_238 = *(undefined8 *)(param_2 + 500);
  uStack_230 = *(undefined8 *)(param_2 + 0x1fc);
  local_228 = *(undefined8 *)(param_2 + 0x204);
  uStack_220 = *(undefined8 *)(param_2 + 0x20c);
  local_218 = *(undefined8 *)(param_2 + 0x214);
  uStack_210 = *(undefined8 *)(param_2 + 0x21c);
  local_208 = *(undefined8 *)(param_2 + 0x224);
  uStack_200 = *(undefined8 *)(param_2 + 0x22c);
  local_1f8 = *(undefined8 *)(param_2 + 0x234);
  uStack_1f0 = *(undefined8 *)(param_2 + 0x23c);
  local_1e8 = *(undefined8 *)(param_2 + 0x244);
  uStack_1e0 = *(undefined8 *)(param_2 + 0x24c);
  local_1d8 = *(undefined8 *)(param_2 + 0x254);
  uStack_1d0 = *(undefined8 *)(param_2 + 0x25c);
  uVar1 = *(uint *)(param_2 + 0x214);
  puVar4 = &local_1b8;
  for (lVar3 = 0x31; lVar3 != 0; lVar3 = lVar3 + -1) {
    *puVar4 = 0;
    puVar4 = puVar4 + 1;
  }
  local_2bc = param_3;
  if ((param_3 < uVar1) ||
     (uStack_220._4_4_ = (uint)((ulong)uStack_220 >> 0x20), uStack_220._4_4_ < param_3)) {
    iVar2 = 0xffff;
    if (1 < DAT_0065d090) {
      local_2b8[0] = (undefined1 *)0x2c;
      local_298 = local_288;
                    /* try { // try from 004dc348 to 004dc34c has its CatchHandler @ 004dc50c */
      local_298 = (undefined1 **)FUN_00599080(&local_298,local_2b8,0);
      local_288[0] = local_2b8[0];
      *local_298 = (undefined1 *)0x252021524f525245;
      local_298[1] = (undefined1 *)0x6f20736920642573;
      local_298[4] = (undefined1 *)0x78616d202c642520;
      *(undefined4 *)(local_298 + 5) = 0x6425203a;
      local_298[2] = (undefined1 *)0x617220666f207475;
      local_298[3] = (undefined1 *)0x3a6e696d2065676e;
      local_290 = local_2b8[0];
      *(char *)((long)local_298 + (long)local_2b8[0]) = '\0';
                    /* try { // try from 004dc3bd to 004dc3c1 has its CatchHandler @ 004dc514 */
      FUN_004dc020(local_2b8,&local_298,&DAT_005c3398,&local_2bc,&local_218,(long)&uStack_220 + 4);
      if (local_298 != local_288) {
        free(local_298);
      }
                    /* try { // try from 004dc3e4 to 004dc3e8 has its CatchHandler @ 004dc51c */
      FUN_004e5fb0(2,local_2b8[0]);
      if (local_2b8[0] != local_2a8) {
        free(local_2b8[0]);
      }
      iVar2 = 0xffff;
    }
  }
  else {
    local_1a8 = 0xf7;
    local_1b8._4_4_ = 1 << ((byte)*(undefined4 *)(param_1 + 0x1280) & 0x1f);
    local_1ac = param_3;
    iVar2 = FUN_00429080(DAT_0065d008,DAT_0065cfb0,0x2080e634,&local_1b8,0x188);
    if ((iVar2 != 0) && (1 < DAT_0065d090)) {
      local_2b8[0] = (undefined1 *)0x22;
      local_298 = local_288;
                    /* try { // try from 004dc44e to 004dc452 has its CatchHandler @ 004dc504 */
      local_298 = (undefined1 **)FUN_00599080(&local_298,local_2b8,0);
      local_288[0] = local_2b8[0];
      *local_298 = (undefined1 *)0x252021524f525245;
      local_298[1] = (undefined1 *)0x7320726f72726573;
      *(undefined2 *)(local_298 + 4) = 0x7469;
      local_298[2] = (undefined1 *)0x7020676e69747465;
      local_298[3] = (undefined1 *)0x6d696c207265776f;
      local_290 = local_2b8[0];
      *(char *)((long)local_298 + (long)local_2b8[0]) = '\0';
                    /* try { // try from 004dc4a2 to 004dc4a6 has its CatchHandler @ 004dc4fc */
      FUN_00447690(local_2b8,&local_298,&DAT_005c3398);
      if (local_298 != local_288) {
        free(local_298);
      }
                    /* try { // try from 004dc4c9 to 004dc4cd has its CatchHandler @ 004dc4f4 */
      FUN_004e5fb0(2,local_2b8[0]);
      if (local_2b8[0] != local_2a8) {
        free(local_2b8[0]);
      }
    }
  }
  return iVar2;
}


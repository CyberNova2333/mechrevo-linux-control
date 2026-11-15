
void FUN_004d9500(pthread_mutex_t *param_1)

{
  byte bVar1;
  int iVar2;
  byte bVar3;
  long in_stack_00000010;
  int iStack0000000000000018;
  int iStack000000000000001c;
  int iStack0000000000000020;
  int iStack0000000000000024;
  int iStack0000000000000028;
  int iStack000000000000002c;
  int iStack0000000000000030;
  byte bStack0000000000000034;
  undefined4 uStack0000000000000038;
  undefined4 uStack000000000000003c;
  undefined4 in_stack_00000040;
  
  iVar2 = iStack000000000000002c + 1;
  if (_iStack0000000000000028 < 0) {
    iVar2 = iStack000000000000002c + -1;
  }
  param_1[0x12].__data.__lock = iVar2;
  iVar2 = in_stack_00000010._4_4_ + 1;
  if (in_stack_00000010 < 0) {
    iVar2 = in_stack_00000010._4_4_ + -1;
  }
  *(int *)((long)param_1 + 0x2d8) = iVar2;
  iVar2 = iStack000000000000001c + 1;
  if (_iStack0000000000000018 < 0) {
    iVar2 = iStack000000000000001c + -1;
  }
  *(int *)((long)param_1 + 0x2dc) = iVar2;
  iVar2 = iStack0000000000000024 + 1;
  if (_iStack0000000000000020 < 0) {
    iVar2 = iStack0000000000000024 + -1;
  }
  *(int *)((long)param_1 + 0x2e0) = iVar2;
  iVar2 = iStack0000000000000030 + 1;
  if (iStack0000000000000030 < 0) {
    iVar2 = iStack0000000000000030 + -1;
  }
  *(int *)((long)param_1 + 0x2d4) = iVar2;
  iVar2 = iStack0000000000000018 + 1;
  if (iStack0000000000000018 < 0) {
    iVar2 = iStack0000000000000018 + -1;
  }
  *(int *)((long)param_1 + 0x2e4) = iVar2;
  iVar2 = iStack0000000000000020 + 1;
  if (iStack0000000000000020 < 0) {
    iVar2 = iStack0000000000000020 + -1;
  }
  *(int *)((long)param_1 + 0x2e8) = iVar2;
  iVar2 = iStack0000000000000028 + 1;
  if (iStack0000000000000028 < 0) {
    iVar2 = iStack0000000000000028 + -1;
  }
  *(int *)((long)param_1 + 0x2ec) = iVar2;
  *(undefined4 *)((long)param_1 + 0x31c) = uStack0000000000000038;
  *(undefined4 *)((long)param_1 + 0x328) = uStack000000000000003c;
  *(undefined4 *)((long)param_1 + 0x330) = in_stack_00000040;
  bVar1 = bStack0000000000000034 & 3;
  bVar3 = bVar1;
  if ((_iStack0000000000000030 & 0x300000000) == 0) {
    bVar3 = bStack0000000000000034 >> 4 & 3;
    bVar1 = bStack0000000000000034 >> 2 & 3;
  }
  if (bVar1 == 1) {
    *(undefined1 *)((long)param_1 + 0x304) = 0;
  }
  else if (bVar1 == 2) {
    *(undefined1 *)((long)param_1 + 0x304) = 1;
  }
  if (bVar3 == 1) {
    *(undefined1 *)((long)param_1 + 0x305) = 0;
  }
  else if (bVar3 == 2) {
    *(undefined1 *)((long)param_1 + 0x305) = 1;
  }
  iVar2 = pthread_mutex_lock(param_1);
  if (iVar2 != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_004204d3(iVar2);
  }
  if (*(char *)((long)param_1 + 0x204) != '\0') {
    FUN_004dddf0(param_1 + 1,0,0);
  }
  pthread_mutex_unlock(param_1);
  FUN_004c2dc0(*(undefined8 *)((long)param_1 + 0x11e8));
  return;
}


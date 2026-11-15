
void FUN_00512bc0(long param_1)

{
  long lVar1;
  void *__ptr;
  undefined8 uVar2;
  
  lVar1 = *(long *)(param_1 + 0xd8);
  if ((*(char *)(param_1 + 0xe6) == '\0') || (*(char *)(param_1 + 0xe5) == '\0')) {
    if (lVar1 != 0) {
      FUN_00511090(lVar1,*(undefined4 *)(param_1 + 0x10));
      __ptr = *(void **)(param_1 + 0xd8);
      if (__ptr != (void *)0x0) {
        FUN_00507b60(__ptr);
        free(__ptr);
      }
      *(undefined8 *)(param_1 + 0xd8) = 0;
      return;
    }
  }
  else if (lVar1 == 0) {
    uVar2 = FUN_0055e9b0(0x33910);
                    /* try { // try from 00512c5b to 00512c5f has its CatchHandler @ 00512c77 */
    FUN_00509b10(uVar2,param_1);
    *(undefined8 *)(param_1 + 0xd8) = uVar2;
                    /* try { // try from 00512c6d to 00512c71 has its CatchHandler @ 00512c7f */
    FUN_00512a70(uVar2,*(undefined4 *)(param_1 + 0x10));
  }
  return;
}


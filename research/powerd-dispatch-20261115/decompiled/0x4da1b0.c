
void FUN_004da1b0(long param_1)

{
  int iVar1;
  undefined8 uVar2;
  int local_6c;
  undefined1 *local_68 [2];
  undefined1 local_58 [16];
  undefined1 **local_48;
  undefined1 *local_40;
  undefined1 *local_38 [3];
  
  local_6c = *(int *)(param_1 + 0x67c);
  iVar1 = (*(int *)(param_1 + 0x11d4) + local_6c) - *(int *)(param_1 + 0x11d0);
  *(int *)(param_1 + 0x11d4) = iVar1;
  if ((iVar1 + 999U < 1999) && ((*(int *)(param_1 + 0x11d0) != 0 || (local_6c == 0)))) {
    return;
  }
  iVar1 = FUN_004dc1b0(param_1,param_1 + 0x28);
  if (iVar1 == 0) {
    *(int *)(param_1 + 0x11d0) = local_6c;
  }
  else {
    if (iVar1 == 0x1b) {
      uVar2 = FUN_0055d850(0x10);
                    /* try { // try from 00413df5 to 00413df9 has its CatchHandler @ 00413e2a */
      FUN_0056d730(uVar2,"insufficient permissions");
                    /* WARNING: Subroutine does not return */
      FUN_0055e630(uVar2,&PTR_PTR_006545e0,FUN_005672a0);
    }
    if (1 < DAT_0065d090) {
      local_68[0] = (undefined1 *)0x25;
      local_48 = local_38;
                    /* try { // try from 004da26b to 004da26f has its CatchHandler @ 004da318 */
      local_48 = (undefined1 **)FUN_00599080(&local_48,local_68,0);
      *(char *)((long)local_48 + 0x24) = '.';
      local_38[0] = local_68[0];
      *local_48 = (undefined1 *)0x252021524f525245;
      local_48[1] = (undefined1 *)0x7320726f72724573;
      *(undefined4 *)(local_48 + 4) = 0x6425203a;
      local_48[2] = (undefined1 *)0x4720676e69747465;
      local_48[3] = (undefined1 *)0x74696d696c205550;
      local_40 = local_68[0];
      *(char *)((long)local_48 + (long)local_68[0]) = '\0';
                    /* try { // try from 004da2c6 to 004da2ca has its CatchHandler @ 004da328 */
      FUN_00449620(local_68,&local_48,&DAT_005c3398,&local_6c);
      if (local_48 != local_38) {
        free(local_48);
      }
                    /* try { // try from 004da2ed to 004da2f1 has its CatchHandler @ 004da320 */
      FUN_004e5fb0(2,local_68[0]);
      if (local_68[0] != local_58) {
        free(local_68[0]);
      }
    }
  }
  *(undefined4 *)(param_1 + 0x11d4) = 0;
  return;
}


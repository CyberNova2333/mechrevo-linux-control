
void FUN_004da4f0(long param_1)

{
  int iVar1;
  undefined8 uVar2;
  int local_5c;
  undefined1 *local_58 [2];
  undefined1 local_48 [16];
  undefined1 **local_38;
  undefined1 *local_30;
  undefined1 *local_28 [2];
  
  local_5c = *(int *)(param_1 + 0xe8);
  if (local_5c != *(int *)(param_1 + 0x11d8)) {
    iVar1 = FUN_004dc6e0();
    if (iVar1 == 0) {
      *(int *)(param_1 + 0x11d8) = local_5c;
      return;
    }
    if (iVar1 == 0x1b) {
      uVar2 = FUN_0055d850(0x10);
                    /* try { // try from 00413eeb to 00413eef has its CatchHandler @ 00413f20 */
      FUN_0056d730(uVar2,"insufficient permissions");
                    /* WARNING: Subroutine does not return */
      FUN_0055e630(uVar2,&PTR_PTR_006545e0,FUN_005672a0);
    }
    if (1 < DAT_0065d090) {
      local_58[0] = (undefined1 *)0x29;
      local_38 = local_28;
                    /* try { // try from 004da57b to 004da57f has its CatchHandler @ 004da62f */
      local_38 = (undefined1 **)FUN_00599080(&local_38,local_58,0);
      *(char *)(local_38 + 5) = '.';
      local_28[0] = local_58[0];
      *local_38 = (undefined1 *)0x252021524f525245;
      local_38[1] = (undefined1 *)0x7320726f72724573;
      local_38[4] = (undefined1 *)0x6425203a74696d69;
      local_38[2] = (undefined1 *)0x4320676e69747465;
      local_38[3] = (undefined1 *)0x6c20504454205550;
      local_30 = local_58[0];
      *(char *)((long)local_38 + (long)local_58[0]) = '\0';
                    /* try { // try from 004da5dd to 004da5e1 has its CatchHandler @ 004da63f */
      FUN_00449620(local_58,&local_38,&DAT_005c3398,&local_5c);
      if (local_38 != local_28) {
        free(local_38);
      }
                    /* try { // try from 004da604 to 004da608 has its CatchHandler @ 004da637 */
      FUN_004e5fb0(2,local_58[0]);
      if (local_58[0] != local_48) {
        free(local_58[0]);
      }
    }
  }
  return;
}


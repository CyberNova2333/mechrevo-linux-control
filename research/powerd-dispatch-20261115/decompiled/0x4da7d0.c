
void FUN_004da7d0(long param_1,long param_2,char param_3)

{
  undefined1 local_7d;
  int local_7c [3];
  undefined8 local_70 [9];
  
  *(undefined1 *)(param_2 + 0x119c) = 0;
  local_7c[0] = 0;
  FUN_0044b6b0(local_7c);
  if (local_7c[0] != 1) {
    if (param_3 == '\0') {
      if ((*(uint *)(param_2 + 0x11b4) & 2) != 0) {
        return;
      }
      if (*(uint *)(param_2 + 0x11b4) != 0) {
        *(undefined1 *)(param_1 + 0x1239) = 0;
        return;
      }
    }
    local_70[0] = 0;
                    /* try { // try from 004da82a to 004da82e has its CatchHandler @ 004da876 */
    FUN_005053c0(&local_7d,local_70);
    *(undefined1 *)(param_2 + 0x119c) = 1;
    *(char *)(param_2 + 0x119d) = (char)((ulong)local_70[0] >> 0x20);
    *(undefined4 *)(param_2 + 0x11a0) = (undefined4)local_70[0];
    *(undefined1 *)(param_1 + 0x1239) = 1;
  }
  return;
}


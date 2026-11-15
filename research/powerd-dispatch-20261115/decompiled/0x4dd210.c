
void FUN_004dd210(double *param_1)

{
  uint uVar1;
  double dVar2;
  ssize_t sVar3;
  int iVar4;
  undefined8 local_30 [2];
  
  dVar2 = (double)FUN_00567c40();
  local_30[0] = 0;
  sVar3 = pread(*(int *)(param_1 + 1),local_30,8,(ulong)*(uint *)((long)param_1 + 0x24));
  iVar4 = 0;
  if (sVar3 == 8) {
    iVar4 = (int)local_30[0];
  }
  local_30[0] = 0;
  sVar3 = pread(*(int *)(param_1 + 1),local_30,8,(ulong)*(uint *)(param_1 + 4));
  uVar1 = 1;
  if (sVar3 == 8) {
    uVar1 = 1 << ((byte)((ulong)local_30[0] >> 8) & 0x1f);
  }
  if (*(int *)(param_1 + 2) != 0) {
    *param_1 = (double)(float)(((double)(uint)(iVar4 - *(int *)(param_1 + 2)) /
                               ((double)((long)dVar2 - (long)param_1[3]) / 1000000000.0)) /
                              (double)uVar1);
  }
  *(int *)(param_1 + 2) = iVar4;
  param_1[3] = dVar2;
  return;
}


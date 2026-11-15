
void FUN_004dd7a0(undefined8 param_1,long param_2,char *param_3,int *param_4)

{
  uint uVar1;
  int iVar2;
  byte *pbVar3;
  ulong uVar4;
  uint uVar5;
  byte bVar6;
  
  if (*param_3 != '\0') {
    if (*param_4 == 0) {
      uVar4 = 0;
    }
    else {
      pbVar3 = (byte *)(param_4 + 1);
      bVar6 = 0xff;
      do {
        if (*pbVar3 < (byte)param_3[1]) break;
        pbVar3 = pbVar3 + 0x18;
        bVar6 = bVar6 + 1;
      } while (pbVar3 != (byte *)(param_4 + (ulong)(*param_4 - 1) * 6 + 7));
      uVar4 = (ulong)bVar6;
      if (0x1f < bVar6) {
        uVar4 = 0;
      }
    }
    iVar2 = param_4[uVar4 * 6 + 2];
    uVar5 = *(uint *)(param_2 + 0x88);
    if (*(uint *)(param_2 + 0x88) <= (uint)param_4[uVar4 * 6 + 3]) {
      uVar5 = param_4[uVar4 * 6 + 3];
    }
    param_3[0x1c] = '\x01';
    uVar1 = (uint)(iVar2 * *(int *)(param_3 + 4)) / 1000;
    iVar2 = 0;
    if (uVar1 != uVar5) {
      iVar2 = uVar1 - uVar5;
    }
    *(int *)(param_3 + 0x20) = iVar2;
    *(int *)(param_3 + 0x24) = param_4[uVar4 * 6 + 2];
    *(int *)(param_3 + 0x28) = param_4[uVar4 * 6 + 6];
    *(int *)(param_3 + 0x2c) = param_4[uVar4 * 6 + 4];
    *(int *)(param_3 + 0x30) = param_4[uVar4 * 6 + 5];
  }
  return;
}


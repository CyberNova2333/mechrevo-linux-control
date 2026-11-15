
void FUN_004ddc80(long param_1,long param_2,char *param_3)

{
  char cVar1;
  int iVar2;
  uint uVar3;
  uint uVar4;
  uint uVar5;
  int iVar6;
  
  if (*param_3 != '\0') {
    uVar3 = *(uint *)(param_3 + 8);
    if (*(char *)(param_1 + 0x604) == '\0') {
      *(uint *)(param_1 + 0x608) = uVar3;
      *(undefined1 *)(param_1 + 0x604) = 1;
    }
    else {
      uVar4 = *(uint *)(param_1 + 0x608);
      iVar6 = 0x100 - (uint)*(byte *)(param_2 + 0x74);
      if (uVar3 < uVar4) {
        uVar3 = -(iVar6 * (uVar4 - uVar3) >> 8);
      }
      else {
        uVar3 = (uVar3 - uVar4) * iVar6 >> 8;
      }
      uVar3 = uVar4 + uVar3;
      *(uint *)(param_1 + 0x608) = uVar3;
    }
    *(uint *)(param_3 + 0xc) = uVar3;
  }
  cVar1 = *(char *)(param_2 + 0x28);
  param_3[0x10] = cVar1;
  if ((*(char *)(param_1 + 0x15) == '\0') || (*(char *)(param_1 + 0x604) != '\0')) {
    param_3[0x11] = '\x01';
    if (cVar1 == '\0') {
      param_3[0x12] = '\0';
      goto LAB_004ddce4;
    }
    if (*(char *)(param_2 + 8) != '\0') {
      param_3[0x12] = '\x01';
      if (*(char *)(param_2 + 0x8d) == '\0') {
        iVar6 = *(int *)(param_2 + 0x18);
        iVar2 = *(int *)(param_2 + 0xc) - (*(int *)(param_3 + 0xc) + *(int *)(param_3 + 4));
        if (iVar2 < 1) {
          uVar3 = 0;
          if (iVar2 != 0) {
            uVar5 = (uint)*(ushort *)(param_2 + 0x22) * iVar2;
            uVar4 = uVar5 >> 0xc | 0xfff00000;
            if (-1 < (int)uVar5) {
              uVar4 = uVar5 >> 0xc;
            }
            uVar3 = uVar4;
            if ((uVar5 & 0x800) != 0) {
              uVar3 = uVar4 - 1;
              if (uVar5 == 0) {
                uVar3 = uVar4 + 1;
              }
            }
          }
        }
        else {
          uVar3 = iVar2 * (uint)*(ushort *)(param_2 + 0x20);
          uVar3 = (uVar3 >> 0xc) + ((int)uVar3 >> 0xb & 1U);
        }
        iVar2 = uVar3 + *(int *)(param_3 + 4);
        if ((iVar6 <= iVar2) && (iVar6 = iVar2, *(int *)(param_2 + 0x1c) <= iVar2)) {
          iVar6 = *(int *)(param_2 + 0x1c);
        }
        *(int *)(param_3 + 0x14) = iVar6;
        return;
      }
      goto LAB_004ddce4;
    }
    param_3[0x12] = '\0';
  }
  else {
    param_3[0x11] = '\0';
    param_3[0x12] = '\0';
    if (cVar1 == '\0') goto LAB_004ddce4;
  }
  if (*(char *)(param_2 + 0x8d) == '\0') {
    *(undefined4 *)(param_3 + 0x14) = *(undefined4 *)(param_2 + 0x10);
    return;
  }
LAB_004ddce4:
  param_3[0x14] = '\0';
  param_3[0x15] = '\0';
  param_3[0x16] = '\0';
  param_3[0x17] = '\0';
  return;
}


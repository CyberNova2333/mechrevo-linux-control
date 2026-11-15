
void FUN_004dab30(pthread_mutex_t *param_1)

{
  int iVar1;
  int iVar2;
  uint uVar3;
  ulong uVar4;
  double dVar5;
  undefined8 local_f8;
  undefined8 uStack_f0;
  undefined8 local_e8;
  undefined8 uStack_e0;
  undefined8 local_d8;
  undefined8 uStack_d0;
  undefined8 local_c8;
  undefined8 uStack_c0;
  undefined8 local_b8;
  undefined8 uStack_b0;
  undefined8 local_a8;
  undefined8 uStack_a0;
  undefined8 local_98;
  undefined8 uStack_90;
  undefined8 local_88;
  undefined8 uStack_80;
  undefined8 local_78;
  undefined8 uStack_70;
  undefined8 local_68;
  undefined8 uStack_60;
  undefined8 local_58;
  undefined8 uStack_50;
  undefined8 local_48;
  
  if (*(int *)((long)param_1 + 0x11e0) != 1) {
    return;
  }
  iVar2 = param_1[0x72].__data.__lock;
  iVar1 = *(int *)((long)param_1 + 0x1240);
  if (iVar2 == 0) {
    iVar2 = *(int *)((long)param_1 + 0x63c);
  }
  *(int *)((long)param_1 + 0x66c) = iVar2;
  dVar5 = (double)FUN_004dd210((long)param_1 + 0x11f0);
  iVar2 = (int)(long)(dVar5 * 1000.0);
  *(int *)((long)param_1 + 0x670) = iVar2;
  if (iVar2 == 0) {
    FUN_004dd8f0(param_1 + 1);
    iVar2 = *(int *)((long)param_1 + 0x670);
  }
  param_1[0x29].__size[0] = iVar2 != 0;
  iVar2 = pthread_mutex_lock(param_1);
  if (iVar2 != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_004204d3(iVar2);
  }
  local_f8 = *(undefined8 *)((long)param_1 + 0x6c);
  uStack_f0 = *(undefined8 *)((long)param_1 + 0x74);
  local_e8 = *(undefined8 *)((long)param_1 + 0x7c);
  uStack_e0 = *(undefined8 *)((long)param_1 + 0x84);
  local_d8 = *(undefined8 *)((long)param_1 + 0x8c);
  uStack_d0 = *(undefined8 *)((long)param_1 + 0x94);
  local_c8 = *(undefined8 *)((long)param_1 + 0x9c);
  uStack_c0 = *(undefined8 *)((long)param_1 + 0xa4);
  local_b8 = *(undefined8 *)((long)param_1 + 0xac);
  uStack_b0 = *(undefined8 *)((long)param_1 + 0xb4);
  local_a8 = *(undefined8 *)((long)param_1 + 0xbc);
  uStack_a0 = *(undefined8 *)((long)param_1 + 0xc4);
  local_98 = *(undefined8 *)((long)param_1 + 0xcc);
  uStack_90 = *(undefined8 *)((long)param_1 + 0xd4);
  local_88 = *(undefined8 *)((long)param_1 + 0xdc);
  uStack_80 = *(undefined8 *)((long)param_1 + 0xe4);
  local_48 = *(undefined8 *)((long)param_1 + 0x11c);
  local_78 = *(undefined8 *)((long)param_1 + 0xec);
  uStack_70 = *(undefined8 *)((long)param_1 + 0xf4);
  local_68 = *(undefined8 *)((long)param_1 + 0xfc);
  uStack_60 = *(undefined8 *)((long)param_1 + 0x104);
  local_58 = *(undefined8 *)((long)param_1 + 0x10c);
  uStack_50 = *(undefined8 *)((long)param_1 + 0x114);
  pthread_mutex_unlock(param_1);
  if (iVar1 == 0x100) {
LAB_004dac94:
    iVar2 = *(int *)((long)param_1 + 0x7c);
  }
  else {
    if ((char)uStack_d0 != '\0') {
      FUN_004ddc80(param_1 + 1,&local_f8,param_1 + 0x29);
      FUN_004dd770((long)param_1 + 0x119c,&local_f8);
      FUN_004da7d0(param_1,param_1,0);
      FUN_004dd7a0((long)param_1 + 0x1188,&local_f8,(long)param_1 + 0x119c,(long)param_1 + 0x1288);
      iVar2 = pthread_mutex_lock(param_1);
      if (iVar2 != 0) {
                    /* WARNING: Subroutine does not return */
        FUN_004204d3(iVar2);
      }
      *(undefined1 *)((long)param_1 + 0x374) = 1;
      *(undefined4 *)((long)param_1 + 0x34c) = *(undefined4 *)((long)param_1 + 0x67c);
      if (*(char *)((long)param_1 + 0x11b8) != '\0') {
        *(undefined4 *)((long)param_1 + 0x3f4) = *(undefined4 *)((long)param_1 + 0x11bc);
        *(undefined4 *)((long)param_1 + 0x3dc) = *(undefined4 *)((long)param_1 + 0x11c0);
        *(undefined4 *)((long)param_1 + 0x3e4) = *(undefined4 *)((long)param_1 + 0x11c4);
        *(undefined8 *)((long)param_1 + 0x3ec) = *(undefined8 *)((long)param_1 + 0x11c8);
      }
      FUN_004dddf0(param_1 + 1,0,0);
      local_f8 = *(undefined8 *)((long)param_1 + 0x6c);
      uStack_f0 = *(undefined8 *)((long)param_1 + 0x74);
      local_e8 = *(undefined8 *)((long)param_1 + 0x7c);
      uStack_e0 = *(undefined8 *)((long)param_1 + 0x84);
      local_d8 = *(undefined8 *)((long)param_1 + 0x8c);
      uStack_d0 = *(undefined8 *)((long)param_1 + 0x94);
      local_48 = *(undefined8 *)((long)param_1 + 0x11c);
      local_c8 = *(undefined8 *)((long)param_1 + 0x9c);
      uStack_c0 = *(undefined8 *)((long)param_1 + 0xa4);
      local_b8 = *(undefined8 *)((long)param_1 + 0xac);
      uStack_b0 = *(undefined8 *)((long)param_1 + 0xb4);
      local_a8 = *(undefined8 *)((long)param_1 + 0xbc);
      uStack_a0 = *(undefined8 *)((long)param_1 + 0xc4);
      local_98 = *(undefined8 *)((long)param_1 + 0xcc);
      uStack_90 = *(undefined8 *)((long)param_1 + 0xd4);
      local_88 = *(undefined8 *)((long)param_1 + 0xdc);
      uStack_80 = *(undefined8 *)((long)param_1 + 0xe4);
      local_78 = *(undefined8 *)((long)param_1 + 0xec);
      uStack_70 = *(undefined8 *)((long)param_1 + 0xf4);
      local_68 = *(undefined8 *)((long)param_1 + 0xfc);
      uStack_60 = *(undefined8 *)((long)param_1 + 0x104);
      local_58 = *(undefined8 *)((long)param_1 + 0x10c);
      uStack_50 = *(undefined8 *)((long)param_1 + 0x114);
      pthread_mutex_unlock(param_1);
    }
    if (*(char *)((long)param_1 + 0x1238) != '\0') {
      if (*(int *)((long)param_1 + 0x1244) == 0x7fffffff) goto LAB_004dac9d;
      goto LAB_004dac94;
    }
    iVar2 = *(int *)((long)param_1 + 0x7c);
    if (iVar2 == *(int *)((long)param_1 + 0x9c)) {
      iVar2 = param_1[4].__data.__lock;
    }
  }
  *(int *)((long)param_1 + 0x67c) = iVar2;
LAB_004dac9d:
  FUN_004da1b0(param_1);
  FUN_004da4f0(param_1);
  if (*(char *)((long)param_1 + 0x41) != '\0') {
    FUN_004da650(param_1);
  }
  iVar2 = pthread_mutex_lock(param_1);
  if (iVar2 != 0) {
                    /* WARNING: Subroutine does not return */
    FUN_004204d3(iVar2);
  }
  uVar3 = *(int *)((long)param_1 + 0x680) + 1;
  uVar4 = (ulong)uVar3;
  if (*(uint *)((long)param_1 + 0x684) <= uVar3) {
    uVar4 = 0;
  }
  *(int *)((long)param_1 + 0x680) = (int)uVar4;
  param_1->__size[uVar4 * 0x2c + 0x6b0] = *(char *)((long)param_1 + 0x67a);
  *(undefined4 *)(param_1->__size + uVar4 * 0x2c + 0x68c) = *(undefined4 *)((long)param_1 + 0x674);
  *(undefined4 *)(param_1->__size + uVar4 * 0x2c + 0x690) = *(undefined4 *)((long)param_1 + 0x66c);
  *(undefined4 *)(param_1->__size + uVar4 * 0x2c + 0x694) = *(undefined4 *)((long)param_1 + 0x11d8);
  *(int *)(param_1->__size + uVar4 * 0x2c + 0x698) = param_1[0x72].__data.__lock;
  *(undefined4 *)(param_1->__size + uVar4 * 0x2c + 0x6a0) = *(undefined4 *)((long)param_1 + 0x670);
  pthread_mutex_unlock(param_1);
  if ((((iVar1 == 0x100) || (*(char *)((long)param_1 + 0x1239) == '\0')) ||
      (*(char *)((long)param_1 + 0x678) == '\0')) || (*(char *)((long)param_1 + 0x679) == '\0')) {
    FUN_004dd8f0(param_1 + 1);
    FUN_004c2fd0(*(undefined8 *)((long)param_1 + 0x11e8),2000);
  }
  else {
    FUN_004c2fd0(*(undefined8 *)((long)param_1 + 0x11e8),100);
  }
  return;
}


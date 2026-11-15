
void FUN_004dddf0(long param_1,undefined8 *param_2,undefined8 *param_3)

{
  char cVar1;
  undefined8 uVar2;
  int iVar3;
  int iVar4;
  uint uVar5;
  uint uVar6;
  uint uVar7;
  long lVar8;
  char *pcVar9;
  long lVar10;
  short *psVar11;
  int *piVar12;
  uint *puVar13;
  undefined1 uVar14;
  uint uVar15;
  int iVar16;
  uint uVar17;
  ulong uVar18;
  uint uVar19;
  uint uVar20;
  char local_8d;
  uint local_8c;
  char local_88 [8];
  char local_80 [8];
  long alStack_78 [9];
  
  if (param_2 != (undefined8 *)0x0) {
    uVar2 = *(undefined8 *)(param_1 + 0x4c);
    *param_2 = *(undefined8 *)(param_1 + 0x44);
    param_2[1] = uVar2;
    uVar2 = *(undefined8 *)(param_1 + 0x5c);
    param_2[2] = *(undefined8 *)(param_1 + 0x54);
    param_2[3] = uVar2;
    uVar2 = *(undefined8 *)(param_1 + 0x6c);
    param_2[4] = *(undefined8 *)(param_1 + 100);
    param_2[5] = uVar2;
    uVar2 = *(undefined8 *)(param_1 + 0x7c);
    param_2[6] = *(undefined8 *)(param_1 + 0x74);
    param_2[7] = uVar2;
    uVar2 = *(undefined8 *)(param_1 + 0x8c);
    param_2[8] = *(undefined8 *)(param_1 + 0x84);
    param_2[9] = uVar2;
    uVar2 = *(undefined8 *)(param_1 + 0x9c);
    param_2[10] = *(undefined8 *)(param_1 + 0x94);
    param_2[0xb] = uVar2;
    uVar2 = *(undefined8 *)(param_1 + 0xac);
    param_2[0xc] = *(undefined8 *)(param_1 + 0xa4);
    param_2[0xd] = uVar2;
    uVar2 = *(undefined8 *)(param_1 + 0xbc);
    param_2[0xe] = *(undefined8 *)(param_1 + 0xb4);
    param_2[0xf] = uVar2;
    uVar2 = *(undefined8 *)(param_1 + 0xcc);
    param_2[0x10] = *(undefined8 *)(param_1 + 0xc4);
    param_2[0x11] = uVar2;
    uVar2 = *(undefined8 *)(param_1 + 0xdc);
    param_2[0x12] = *(undefined8 *)(param_1 + 0xd4);
    param_2[0x13] = uVar2;
    uVar2 = *(undefined8 *)(param_1 + 0xec);
    param_2[0x14] = *(undefined8 *)(param_1 + 0xe4);
    param_2[0x15] = uVar2;
    param_2[0x16] = *(undefined8 *)(param_1 + 0xf4);
  }
  *(undefined1 *)(param_1 + 0xd2) = *(undefined1 *)(param_1 + 0x274);
  lVar8 = 2;
  pcVar9 = (char *)(param_1 + 0x224);
  do {
    local_88[lVar8] = *pcVar9;
    local_80[lVar8] = pcVar9[1];
    lVar8 = lVar8 + 1;
    pcVar9 = pcVar9 + 0xb8;
  } while (lVar8 != 8);
  pcVar9 = local_88;
  do {
    if (pcVar9[2] != '\0') {
      uVar14 = 1;
      break;
    }
    pcVar9 = pcVar9 + 1;
    uVar14 = 0;
  } while (pcVar9 != local_88 + 6);
  *(undefined1 *)(param_1 + 0xb4) = uVar14;
  pcVar9 = local_80;
  do {
    if (pcVar9[2] != '\0') {
      uVar14 = 1;
      break;
    }
    pcVar9 = pcVar9 + 1;
    uVar14 = 0;
  } while (local_80 + 6 != pcVar9);
  *(undefined1 *)(param_1 + 0xb5) = uVar14;
  lVar8 = param_1 + 0x1bc;
  alStack_78[3] = param_1 + 0x274;
  alStack_78[4] = param_1 + 0x32c;
  alStack_78[5] = param_1 + 0x3e4;
  alStack_78[6] = param_1 + 0x49c;
  alStack_78[7] = param_1 + 0x554;
  lVar10 = 2;
  if (*(char *)(param_1 + 0xd1) == '\0') {
    while( true ) {
      *(bool *)lVar8 = local_88[lVar10] == '\0';
      lVar10 = lVar10 + 1;
      if (lVar10 == 8) break;
      lVar8 = alStack_78[lVar10];
    }
    local_8d = *(char *)(param_1 + 0xb4);
    *(bool *)(param_1 + 0x4c) = local_8d == '\0';
  }
  else {
    while( true ) {
      *(bool *)lVar8 = local_80[lVar10] == '\0';
      lVar10 = lVar10 + 1;
      if (lVar10 == 8) break;
      lVar8 = alStack_78[lVar10];
    }
    local_8d = *(char *)(param_1 + 0xb4);
    *(bool *)(param_1 + 0x4c) = *(char *)(param_1 + 0xb5) == '\0';
  }
  iVar4 = *(int *)(param_1 + 0x558);
  *(undefined4 *)(param_1 + 0x108) = 0;
  if (((iVar4 != 0) || (iVar4 = *(int *)(param_1 + 0x4a0), iVar4 != 0)) ||
     (iVar4 = *(int *)(param_1 + 1000), iVar4 != 0)) {
    *(int *)(param_1 + 0x108) = iVar4;
  }
  iVar3 = *(int *)(param_1 + 0x55c);
  *(undefined4 *)(param_1 + 0x10c) = 0;
  if (((iVar3 != 0) || (iVar3 = *(int *)(param_1 + 0x4a4), iVar3 != 0)) ||
     (iVar3 = *(int *)(param_1 + 0x3ec), iVar3 != 0)) {
    *(int *)(param_1 + 0x10c) = iVar3;
  }
  iVar16 = *(int *)(param_1 + 0x564);
  *(undefined4 *)(param_1 + 0x114) = 0;
  if (((iVar16 != 0) || (iVar16 = *(int *)(param_1 + 0x4ac), iVar16 != 0)) ||
     (iVar16 = *(int *)(param_1 + 0x3f4), iVar16 != 0)) {
    *(int *)(param_1 + 0x114) = iVar16;
  }
  iVar16 = *(int *)(param_1 + 0x568);
  *(undefined4 *)(param_1 + 0x118) = 0;
  if (((iVar16 != 0) || (iVar16 = *(int *)(param_1 + 0x4b0), iVar16 != 0)) ||
     (iVar16 = *(int *)(param_1 + 0x3f8), iVar16 != 0)) {
    *(int *)(param_1 + 0x118) = iVar16;
  }
  iVar16 = *(int *)(param_1 + 0x5f4);
  *(undefined4 *)(param_1 + 0x1a4) = 0;
  if (((iVar16 != 0) || (iVar16 = *(int *)(param_1 + 0x53c), iVar16 != 0)) ||
     (iVar16 = *(int *)(param_1 + 0x484), iVar16 != 0)) {
    *(int *)(param_1 + 0x1a4) = iVar16;
  }
  iVar16 = *(int *)(param_1 + 0x5f8);
  *(undefined4 *)(param_1 + 0x1a8) = 0;
  if (((iVar16 != 0) || (iVar16 = *(int *)(param_1 + 0x540), iVar16 != 0)) ||
     (iVar16 = *(int *)(param_1 + 0x488), iVar16 != 0)) {
    *(int *)(param_1 + 0x1a8) = iVar16;
  }
  uVar20 = *(uint *)(param_1 + 0xa0);
  iVar16 = *(int *)(param_1 + 0x4c4);
  if (*(int *)(param_1 + 0x4c4) == 0) {
    iVar16 = *(int *)(param_1 + 0x57c);
  }
  if (*(int *)(param_1 + 0x40c) != 0) {
    iVar16 = *(int *)(param_1 + 0x40c);
  }
  *(int *)(param_1 + 300) = iVar16;
  iVar16 = *(int *)(param_1 + 0x4cc);
  if (*(int *)(param_1 + 0x4cc) == 0) {
    iVar16 = *(int *)(param_1 + 0x584);
  }
  if (*(int *)(param_1 + 0x414) != 0) {
    iVar16 = *(int *)(param_1 + 0x414);
  }
  *(int *)(param_1 + 0x134) = iVar16;
  iVar16 = *(int *)(param_1 + 0x4c8);
  if (*(int *)(param_1 + 0x4c8) == 0) {
    iVar16 = *(int *)(param_1 + 0x580);
  }
  if (*(int *)(param_1 + 0x410) != 0) {
    iVar16 = *(int *)(param_1 + 0x410);
  }
  *(int *)(param_1 + 0x130) = iVar16;
  uVar6 = *(uint *)(param_1 + 0x1e4);
  uVar7 = *(uint *)(param_1 + 0x1c8);
  uVar19 = uVar20;
  if (uVar7 == 0) {
    uVar7 = uVar20;
  }
  else if (uVar7 <= uVar20) {
    uVar19 = uVar7;
  }
  *(uint *)(param_1 + 0x58) = uVar7;
  lVar8 = param_1;
  iVar3 = FUN_004dd860(*(undefined4 *)(param_1 + 0x1c4),*(undefined4 *)(param_1 + 0x2a8),
                       *(undefined4 *)(param_1 + 0x1c4),*(undefined4 *)(param_1 + 0x27c),
                       *(undefined4 *)(param_1 + 0x334),iVar3,*(undefined4 *)(param_1 + 0x24),
                       *(undefined4 *)(param_1 + 0x28));
  uVar7 = *(uint *)(lVar8 + 0xa4);
  uVar17 = ((iVar3 + 0xfaU) / 500) * 500;
  if (uVar17 <= uVar19) {
    uVar19 = uVar17;
  }
  if (uVar19 < uVar7) {
    uVar19 = uVar7;
  }
  *(uint *)(lVar8 + 0x54) = uVar19;
  if (uVar6 == 0) {
    local_8c = *(uint *)(lVar8 + 0x74);
  }
  else {
    uVar17 = *(uint *)(lVar8 + 300);
    if ((*(uint *)(lVar8 + 300) == 0) && (uVar17 = 10000000, uVar6 < 0x989681)) {
      uVar17 = uVar6;
    }
    local_8c = ((uVar17 + 0x3e) / 0x7d) * 0x7d;
    if (*(uint *)(lVar8 + 0xa8) < local_8c) {
      local_8c = *(uint *)(lVar8 + 0xa8);
    }
    if (local_8c < uVar7) {
      local_8c = uVar7;
    }
    *(uint *)(lVar8 + 0x74) = local_8c;
  }
  iVar4 = FUN_004dd860(uVar19,*(undefined4 *)(param_1 + 0x2b0),*(undefined4 *)(lVar8 + 0x1c0),
                       *(undefined4 *)(lVar8 + 0x278),*(undefined4 *)(lVar8 + 0x330),iVar4,
                       *(undefined4 *)(lVar8 + 0x3c),*(undefined4 *)(lVar8 + 0x40));
  uVar15 = ((iVar4 + 0xfaU) / 500) * 500;
  *(uint *)(lVar8 + 0x50) = uVar15;
  iVar4 = FUN_004dd860();
  uVar17 = ((iVar4 + 0xfaU) / 500) * 500;
  *(uint *)(lVar8 + 0x5c) = uVar17;
  iVar4 = FUN_004dd860();
  if (uVar17 < uVar7) {
    uVar17 = uVar7;
  }
  if (uVar20 < uVar17) {
    uVar17 = uVar20;
  }
  *(uint *)(lVar8 + 0x5c) = uVar17;
  uVar5 = ((iVar4 + 0xfaU) / 500) * 500;
  if (uVar5 <= uVar20) {
    uVar20 = uVar5;
  }
  if (uVar20 < uVar17) {
    uVar20 = uVar17;
  }
  if (local_8d == '\0') {
    if (uVar15 < uVar17) {
      uVar15 = uVar17;
    }
  }
  else if (uVar15 < uVar19) {
    uVar15 = uVar19;
  }
  *(uint *)(lVar8 + 0x50) = uVar15;
  if (uVar15 < uVar20) {
    uVar20 = uVar15;
  }
  *(uint *)(lVar8 + 0x60) = uVar20;
  if (uVar6 != 0) {
    uVar6 = FUN_004dd860(local_8c,*(undefined4 *)(param_1 + 700),*(undefined4 *)(lVar8 + 0x25c),
                         *(undefined4 *)(lVar8 + 0x314),*(undefined4 *)(lVar8 + 0x3cc),
                         *(undefined4 *)(lVar8 + 0x1a4),10000000,0xffffffffff676980);
    uVar20 = *(uint *)(lVar8 + 0x1a8);
    *(uint *)(lVar8 + 0xec) = uVar6;
    if ((uVar20 == 0) && (uVar20 = *(uint *)(lVar8 + 0x3d0), uVar20 == 0)) {
      uVar20 = *(uint *)(lVar8 + 0x318);
      if ((uVar20 != 0) || (uVar20 = *(uint *)(lVar8 + 0x260), uVar20 != 0)) {
        if (10000000 < uVar20) {
          uVar20 = 10000000;
        }
        goto LAB_004de3fb;
      }
      *(undefined4 *)(lVar8 + 0xf0) = 10000000;
      uVar20 = 10000000;
    }
    else {
LAB_004de3fb:
      *(uint *)(lVar8 + 0xf0) = uVar20;
    }
    cVar1 = *(char *)(lVar8 + 0xb5);
    if (uVar20 < uVar6) {
      uVar6 = uVar20;
    }
    uVar18 = 0;
    uVar6 = (uVar6 + 0x3e) / 0x7d;
    uVar20 = uVar6 * 0x7d;
    *(uint *)(lVar8 + 0x70) = uVar20;
    if (cVar1 == '\0') {
      uVar18 = (ulong)(uVar6 * 0x177) / 5;
    }
    uVar5 = local_8c;
    iVar4 = FUN_004dd860(local_8c,*(undefined4 *)(param_1 + 0x2ac),*(undefined4 *)(lVar8 + 0x1ec),
                         *(undefined4 *)(lVar8 + 0x2a4),uVar18,*(undefined4 *)(lVar8 + 0x134),
                         10000000,0xffffffffff676980);
    uVar19 = ((iVar4 + 0x3eU) / 0x7d) * 0x7d;
    *(uint *)(lVar8 + 0x7c) = uVar19;
    iVar4 = FUN_004dd860();
    uVar6 = *(uint *)(lVar8 + 0xa8);
    if (uVar19 < uVar7) {
      uVar19 = uVar7;
    }
    if (uVar6 < uVar19) {
      uVar19 = uVar6;
    }
    *(uint *)(lVar8 + 0x7c) = uVar19;
    uVar7 = ((iVar4 + 0x3eU) / 0x7d) * 0x7d;
    if (uVar6 < uVar7) {
      uVar7 = uVar6;
    }
    if (uVar7 < uVar19) {
      uVar7 = uVar19;
    }
    if (cVar1 == '\0') {
      if (uVar20 < uVar19) {
        uVar20 = uVar19;
      }
    }
    else if (uVar20 < uVar5) {
      uVar20 = uVar5;
    }
    *(uint *)(lVar8 + 0x70) = uVar20;
    if (uVar20 < uVar7) {
      uVar7 = uVar20;
    }
    *(uint *)(lVar8 + 0x78) = uVar7;
  }
  psVar11 = (short *)(lVar8 + 0x5c2);
  do {
    if (*psVar11 != 0) {
      *(short *)(lVar8 + 0xba) = *psVar11;
      break;
    }
    psVar11 = psVar11 + -0x5c;
  } while (psVar11 != (short *)(lVar8 + 0x172));
  pcVar9 = (char *)(lVar8 + 0x5be);
  uVar18 = 8;
  do {
    uVar20 = (int)uVar18 - 1;
    uVar18 = (ulong)uVar20;
    if (*pcVar9 != '\0') {
      lVar10 = uVar18 * 0xb8 + lVar8;
      *(undefined2 *)(lVar8 + 0xb6) = *(undefined2 *)(lVar10 + 0xb6);
      *(undefined1 *)(lVar8 + 0xb8) = *(undefined1 *)(lVar10 + 0xb8);
      break;
    }
    pcVar9 = pcVar9 + -0xb8;
  } while (uVar20 != 2);
  *(undefined1 *)(lVar8 + 0xf4) = 0;
  pcVar9 = (char *)(lVar8 + 0x264);
  do {
    if (*pcVar9 != '\0') {
      *(undefined1 *)(lVar8 + 0xf4) = 1;
      break;
    }
    pcVar9 = pcVar9 + 0xb8;
  } while ((char *)(lVar8 + 0x6b4) != pcVar9);
  cVar1 = *(char *)(lVar8 + 0xd1);
  *(undefined4 *)(lVar8 + 0xf8) = 0;
  if ((local_8d == '\0' && cVar1 == '\0') && (*(char *)(lVar8 + 0xf4) == '\0')) {
    iVar4 = *(int *)(lVar8 + 0x600);
    if ((iVar4 == 0) &&
       ((iVar4 = *(int *)(lVar8 + 0x548), iVar4 == 0 &&
        (iVar4 = *(int *)(lVar8 + 0x490), iVar4 == 0)))) {
      *(uint *)(lVar8 + 0xf8) = uVar15 - uVar17;
    }
    else {
      *(int *)(lVar8 + 0xf8) = iVar4;
    }
  }
  *(undefined1 *)(lVar8 + 0xbc) = 0;
  pcVar9 = (char *)(lVar8 + 0x22c);
  do {
    if (*pcVar9 != '\0') {
      *(undefined1 *)(lVar8 + 0xbc) = 1;
      break;
    }
    pcVar9 = pcVar9 + 0xb8;
  } while (pcVar9 != (char *)(lVar8 + 0x67c));
  piVar12 = (int *)(lVar8 + 0x5b8);
  *(undefined4 *)(lVar8 + 0xb0) = 0;
  do {
    if (*piVar12 != 0) {
      *(int *)(lVar8 + 0xb0) = *piVar12;
      break;
    }
    piVar12 = piVar12 + -0x2e;
  } while (piVar12 != (int *)(lVar8 + 0x168));
  piVar12 = (int *)(lVar8 + 0x5cc);
  *(undefined4 *)(lVar8 + 0xc4) = 0;
  do {
    if (*piVar12 != 0) {
      *(int *)(lVar8 + 0xc4) = *piVar12;
      break;
    }
    piVar12 = piVar12 + -0x2e;
  } while ((int *)(lVar8 + 0x17c) != piVar12);
  piVar12 = (int *)(lVar8 + 0x5ec);
  *(undefined4 *)(lVar8 + 0xe4) = 0;
  do {
    if (*piVar12 != 0) {
      *(int *)(lVar8 + 0xe4) = *piVar12;
      break;
    }
    piVar12 = piVar12 + -0x2e;
  } while (piVar12 != (int *)(lVar8 + 0x30c));
  piVar12 = (int *)(lVar8 + 0x5f0);
  *(undefined4 *)(lVar8 + 0xe8) = 10000000;
  do {
    if (*piVar12 != 0) {
      *(int *)(lVar8 + 0xe8) = *piVar12;
      break;
    }
    piVar12 = piVar12 + -0x2e;
  } while (piVar12 != (int *)(lVar8 + 0x310));
  *(undefined4 *)(lVar8 + 0xc0) = 0;
  if (((cVar1 != '\0') && (*(char *)(lVar8 + 0xb5) == '\0')) && (*(char *)(lVar8 + 0xbc) == '\0')) {
    puVar13 = (uint *)(lVar8 + 0x5c8);
    do {
      uVar20 = *puVar13;
      if (uVar20 != 0) goto LAB_004dea65;
      puVar13 = puVar13 + -0x2e;
    } while ((uint *)(lVar8 + 0x178) != puVar13);
    iVar4 = *(int *)(lVar8 + 0xb0);
    if (iVar4 == 2) {
      uVar20 = *(uint *)(lVar8 + 0xc4);
      *(uint *)(lVar8 + 0xc0) = uVar20;
    }
    else if (iVar4 == 3) {
      uVar20 = *(uint *)(lVar8 + 0xe8);
      *(uint *)(lVar8 + 0xc0) = uVar20;
    }
    else {
      if (iVar4 != 1) goto LAB_004de6ac;
      uVar20 = *(uint *)(lVar8 + 0xe4);
      *(uint *)(lVar8 + 0xc0) = uVar20;
    }
    if (uVar20 != 0) {
LAB_004dea65:
      uVar6 = *(uint *)(lVar8 + 0x70);
      if (uVar20 <= *(uint *)(lVar8 + 0xe4)) {
        uVar20 = *(uint *)(lVar8 + 0xe4);
      }
      uVar7 = uVar6;
      if (*(uint *)(lVar8 + 0xe8) <= uVar6) {
        uVar7 = *(uint *)(lVar8 + 0xe8);
      }
      if (uVar7 < uVar20) {
        uVar20 = uVar7;
      }
      uVar20 = (uVar20 + 0x3e) / 0x7d;
      uVar6 = uVar6 + uVar20 * -0x7d;
      *(uint *)(lVar8 + 0xc0) = uVar20 * 0x7d;
      if (uVar6 < *(uint *)(lVar8 + 0x7c)) {
        uVar6 = *(uint *)(lVar8 + 0x7c);
      }
      if (*(uint *)(lVar8 + 0x78) < uVar6) {
        uVar6 = *(uint *)(lVar8 + 0x78);
      }
      *(uint *)(lVar8 + 0x7c) = uVar6;
    }
  }
LAB_004de6ac:
  piVar12 = (int *)(lVar8 + 0x5d0);
  *(undefined4 *)(lVar8 + 200) = 0;
  do {
    if (*piVar12 != 0) {
      *(int *)(lVar8 + 200) = *piVar12;
      break;
    }
    piVar12 = piVar12 + -0x2e;
  } while ((int *)(lVar8 + 0x180) != piVar12);
  piVar12 = (int *)(lVar8 + 0x5d4);
  *(undefined4 *)(lVar8 + 0xcc) = 0;
  do {
    if (*piVar12 != 0) {
      *(int *)(lVar8 + 0xcc) = *piVar12;
      break;
    }
    piVar12 = piVar12 + -0x2e;
  } while (piVar12 != (int *)(lVar8 + 0x184));
  piVar12 = (int *)(lVar8 + 0x5dc);
  *(undefined4 *)(lVar8 + 0xd4) = 0;
  do {
    if (*piVar12 != 0) {
      *(int *)(lVar8 + 0xd4) = *piVar12;
      break;
    }
    piVar12 = piVar12 + -0x2e;
  } while ((int *)(lVar8 + 0x18c) != piVar12);
  piVar12 = (int *)(lVar8 + 0x5e0);
  *(undefined4 *)(lVar8 + 0xd8) = 0;
  do {
    if (*piVar12 != 0) {
      *(int *)(lVar8 + 0xd8) = *piVar12;
      break;
    }
    piVar12 = piVar12 + -0x2e;
  } while (piVar12 != (int *)(lVar8 + 400));
  piVar12 = (int *)(lVar8 + 0x5e8);
  *(undefined4 *)(lVar8 + 0xe0) = 0;
  do {
    iVar4 = *piVar12;
    if (iVar4 != 0) {
      *(int *)(lVar8 + 0xe0) = iVar4;
      *(int *)(lVar8 + 0xdc) = iVar4;
      goto LAB_004de7c0;
    }
    piVar12 = piVar12 + -0x2e;
  } while (piVar12 != (int *)(lVar8 + 0x198));
  piVar12 = (int *)(lVar8 + 0x5e4);
  *(undefined4 *)(lVar8 + 0xdc) = 0;
  do {
    if (*piVar12 != 0) {
      *(int *)(lVar8 + 0xdc) = *piVar12;
      break;
    }
    piVar12 = piVar12 + -0x2e;
  } while (piVar12 != (int *)(lVar8 + 0x194));
LAB_004de7c0:
  *(undefined1 *)(lVar8 + 0xd0) = 0;
  pcVar9 = (char *)(lVar8 + 0x5d8);
  do {
    if (*pcVar9 == '\x01') {
      *(undefined1 *)(lVar8 + 0xd0) = 1;
      break;
    }
    pcVar9 = pcVar9 + -0xb8;
  } while ((char *)(lVar8 + 0x188) != pcVar9);
  if (cVar1 == '\0') {
    piVar12 = (int *)(lVar8 + 0x54c);
    do {
      if (*piVar12 != 0) {
        *(int *)(lVar8 + 0x44) = *piVar12;
        break;
      }
      piVar12 = piVar12 + -0x2e;
    } while (piVar12 != (int *)(param_1 + 0x26c));
  }
  else {
    *(undefined4 *)(lVar8 + 0x44) = 0;
  }
  piVar12 = (int *)(lVar8 + 0x5b4);
  do {
    if (*piVar12 != 0) {
      *(int *)(lVar8 + 0xac) = *piVar12;
      break;
    }
    piVar12 = piVar12 + -0x2e;
  } while ((int *)(lVar8 + 0x164) != piVar12);
  if (cVar1 == '\0') {
    *(undefined4 *)(lVar8 + 0x48) = 0;
    goto LAB_004de82d;
  }
  if (*(char *)(lVar8 + 0xb5) == '\0') {
    uVar20 = *(uint *)(lVar8 + 0x550);
    if (((*(uint *)(lVar8 + 0x550) == 0) &&
        (uVar20 = *(uint *)(lVar8 + 0x498), *(uint *)(lVar8 + 0x498) == 0)) &&
       (uVar20 = *(uint *)(lVar8 + 0x3e0), *(uint *)(lVar8 + 0x3e0) == 0)) {
      if (*(int *)(lVar8 + 0xac) == 1) {
        *(undefined4 *)(lVar8 + 0x48) = *(undefined4 *)(lVar8 + 0x78);
        goto LAB_004de82d;
      }
      goto LAB_004de939;
    }
  }
  else {
LAB_004de939:
    uVar20 = local_8c;
  }
  *(uint *)(lVar8 + 0x48) = uVar20;
LAB_004de82d:
  *(undefined1 *)(lVar8 + 0x6c) = 1;
  if (param_3 != (undefined8 *)0x0) {
    uVar2 = *(undefined8 *)(lVar8 + 0x4c);
    *param_3 = *(undefined8 *)(lVar8 + 0x44);
    param_3[1] = uVar2;
    uVar2 = *(undefined8 *)(lVar8 + 0x5c);
    param_3[2] = *(undefined8 *)(lVar8 + 0x54);
    param_3[3] = uVar2;
    uVar2 = *(undefined8 *)(lVar8 + 0x6c);
    param_3[4] = *(undefined8 *)(lVar8 + 100);
    param_3[5] = uVar2;
    uVar2 = *(undefined8 *)(lVar8 + 0x7c);
    param_3[6] = *(undefined8 *)(lVar8 + 0x74);
    param_3[7] = uVar2;
    uVar2 = *(undefined8 *)(lVar8 + 0x8c);
    param_3[8] = *(undefined8 *)(lVar8 + 0x84);
    param_3[9] = uVar2;
    uVar2 = *(undefined8 *)(lVar8 + 0x9c);
    param_3[10] = *(undefined8 *)(lVar8 + 0x94);
    param_3[0xb] = uVar2;
    uVar2 = *(undefined8 *)(lVar8 + 0xac);
    param_3[0xc] = *(undefined8 *)(lVar8 + 0xa4);
    param_3[0xd] = uVar2;
    uVar2 = *(undefined8 *)(lVar8 + 0xbc);
    param_3[0xe] = *(undefined8 *)(lVar8 + 0xb4);
    param_3[0xf] = uVar2;
    uVar2 = *(undefined8 *)(lVar8 + 0xcc);
    param_3[0x10] = *(undefined8 *)(lVar8 + 0xc4);
    param_3[0x11] = uVar2;
    uVar2 = *(undefined8 *)(lVar8 + 0xdc);
    param_3[0x12] = *(undefined8 *)(lVar8 + 0xd4);
    param_3[0x13] = uVar2;
    uVar2 = *(undefined8 *)(lVar8 + 0xec);
    param_3[0x14] = *(undefined8 *)(lVar8 + 0xe4);
    param_3[0x15] = uVar2;
    param_3[0x16] = *(undefined8 *)(lVar8 + 0xf4);
  }
  return;
}


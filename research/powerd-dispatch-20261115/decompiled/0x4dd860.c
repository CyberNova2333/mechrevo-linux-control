
uint FUN_004dd860(int param_1,int param_2,uint param_3,uint param_4,uint param_5,uint param_6,
                 int param_7,int param_8)

{
  uint uVar1;
  uint uVar2;
  
  if ((param_6 != 0) || (param_6 = param_5, param_5 != 0)) {
    return param_6;
  }
  uVar2 = param_1 + param_7;
  if ((int)uVar2 < 0) {
    uVar2 = 0;
  }
  if (param_4 == 0) {
    if (param_2 + 1U < 2) {
      param_4 = uVar2;
      if (param_3 <= uVar2) {
        param_4 = param_3;
      }
    }
    else {
      uVar1 = param_2 - 1;
      if (param_2 < 0) {
        uVar1 = param_2 + 1U;
      }
      uVar1 = uVar1 + param_1;
      if ((0 < (int)uVar1) && (param_4 = uVar2, uVar1 <= uVar2)) {
        param_4 = uVar1;
      }
    }
  }
  else if (uVar2 < param_4) {
    param_4 = uVar2;
  }
  uVar2 = param_1 + param_8;
  if ((int)uVar2 < 0) {
    uVar2 = 0;
  }
  if (uVar2 < param_4) {
    uVar2 = param_4;
  }
  return uVar2;
}


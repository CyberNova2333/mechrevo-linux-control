
void FUN_004c5090(long param_1)

{
  undefined8 *puVar1;
  undefined8 *puVar2;
  
  puVar1 = *(undefined8 **)(param_1 + 0x30);
  for (puVar2 = *(undefined8 **)(param_1 + 0x28); puVar2 != puVar1; puVar2 = puVar2 + 1) {
    (**(code **)(*(long *)*puVar2 + 0x38))();
  }
  return;
}


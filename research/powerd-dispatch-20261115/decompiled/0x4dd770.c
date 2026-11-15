
void FUN_004dd770(long param_1,long param_2)

{
  *(undefined4 *)(param_1 + 0x18) = 0;
  if (*(char *)(param_2 + 0x8d) == '\0') {
    *(undefined4 *)(param_1 + 0x18) = 2;
  }
  if (*(char *)(param_2 + 8) == '\0') {
    *(uint *)(param_1 + 0x18) = *(uint *)(param_1 + 0x18) | 4;
  }
  if (*(char *)(param_2 + 0x8c) != '\0') {
    *(uint *)(param_1 + 0x18) = *(uint *)(param_1 + 0x18) | 8;
  }
  return;
}


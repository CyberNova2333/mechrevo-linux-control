# powerd：NVPCF 更新的消费者、预算仲裁和 GPU 控制环

本目录记录 2026-11-15 对本机 `/usr/bin/nvidia-powerd` 的静态分析，以及主任务随后取得的精简动态验证记录。SHA-256：`db2643c4c37360e183230ea7f9c5da3e28c39ff79d08eba6ddb0f4f057d7bc74`。REA/Ghidra 12.2，仅反编译和文件读取；本子任务没有附加进程、启停服务、写硬件、修改二进制或内核。动态验证由主任务执行。

此前已证明 quiet/high 的 `targetTppOffsetmW` 分别为 75000/145000，且两者都有 `ctgpOffsetmW=50000,maxOutputOffsetmW=25000,minOutputOffsetmW=-15000`。本轮追踪这些值进入消费者以后如何使用。

## 重要结论和适用边界

**QBoost 代码把 TPP 偏移量加在最终 cTGP 上，并用 CPU/GPU 合计测量值反馈，而不是直接把该偏移量作为独立 GPU 上限。** 同时，GPU 最大输出仍由 cTGP 加 maxOutput 偏移组成。因此，仅 GPU 负载中两档都达到 140 W，不能单独证明 TPP 联动失效。

**后续动态观察已证实本机正好符合这一计算**：QBoost 是实际消费者，controller+204=1，quiet/high 的最终 TPP 分别为 **190/260 W**，两者 cTGP 都是115 W、minOutput 都是100 W、maxOutput 都是140 W。default cTGP=65 W，固件 cTGP offset=50 W；TPP/min/max 的更高覆盖层均为0。

`runtime-policies.json` 从主任务 `qboost-switch.txt` 的 `QBOOST` JSON 行提取，`runtime-firmware.json` 是同轮 before/quiet/extreme/restored 记录；before/restored 字段完全一致，日志最后明确 GDB detached。原始文件来源 `/home/cybernova/.cache/mechrevo-linux-research/control-replica-20261115/`。本轮无压力负载，**仅证明预算已正确仲裁，尚不等于 CPU+GPU 联合负载的物理功耗已验收**。

**默认OEM路径边界：本页早期动态预算使用显式 GAMM=1 的兼容实验。** OEM game-support查询是 `0x50000000013`；该型号GFID=19的实际支持回复为0xAA。GAMM可执行以及显式GAMM下130/140 W的GPU差异，都不能直接当成默认OEM调度验收。后续[官方CTAF两轮对照](official-combined/README.md)已单独确认 `CSTM=1,GATM=0` 时静音/高能CPU约45/75 W、GPU均约140 W，合计约185/215 W，总预算仍为190/260 W。新lease桥真实ACK、常驻自动阈值及归还已[单独验收](../turbo-qualification-20261115/README.md)，真实PM与物理失电仍未测试。不能误把0x16（LCD_OD）当成game-support查询。

## 从通知到消费者

`0x4c5090` 遍历 dispatcher `+0x28..+0x30` 的对象指针数组，将 60 字节事件按值复制到栈，再调用每个对象的 `vtable+0x38`。实际调用指令在 `0x4c50e3`。

| 类 | vtable 地址 | 回调 | 本次事件消费行为 |
| --- | --- | --- | --- |
| DLSteerWorker | 0x64d460 | 0x4b9d50 | 只读取 event[0] 活跃标记，保存到 worker+0xe2，调用启停路径 |
| QBoostWorker | 0x64ea50 | 0x4d46c0 | 取 worker+0xb0 的 controller；非空时传整份事件给 0x4d9500 |
| AutoFLWorker | 0x64f408 | 0x4c2bc0 | 单条 `ret`，不处理此事件 |
| JpacWorker | 0x64fee0 | 0x512c90 | 只读取 event[0]，保存到 worker+0xe6，调用 0x512bc0 创建/销毁内部对象 |

RTTI 字符串、typeinfo 与 vtable 的原始地址保存在 `worker-vtables.json`。部分间接回调未被 Ghidra 建立为函数，本目录另存 objdump 原始反汇编；不能把反编译工具未列出当成函数不存在。

事件布局是 `bool/padding[4] + configuration[56]`。configuration 即此前 `0x44b850` 的输出。

## QBoost 接收和仲裁

令 `C = *(QBoostWorker+0xb0)`，`P = C+0x28`。`0x4d9500(C,event)` 将带符号偏移编码为正数加 1、负数减 1，以便零作为未指定状态。零偏移编码成 1。此处 AC 在前，battery/DC 在后。

| 字段 | event 偏移 | C 偏移 | P 偏移 |
| --- | --- | --- | --- |
| AC cTGP offset | +0x24 | +0x2d0 | +0x2a8 |
| DC cTGP offset | +0x28 | +0x2d4 | +0x2ac |
| AC TPP offset | +0x0c | +0x2d8 | +0x2b0 |
| AC maxOutput offset | +0x14 | +0x2dc | +0x2b4 |
| AC minOutput offset | +0x1c | +0x2e0 | +0x2b8 |
| DC TPP offset | +0x10 | +0x2e4 | +0x2bc |
| DC maxOutput offset | +0x18 | +0x2e8 | +0x2c0 |
| DC minOutput offset | +0x20 | +0x2ec | +0x2c4 |

如果 C+0x204 为真，接收函数调用 `0x4dddf0(P,0,0)` 重算仲裁；随后唤醒 `*(C+0x11e8)` 的周期工作线程。

`0x4dd860(base, encodedOffset, default, tierA, tierB, highest, upperOffset, lowerOffset)` 是核心仲裁辅助函数：

1. highest 非零则直接返回 highest；否则 tierB 非零则直接返回 tierB。
2. 其余分支把 upper/lowerOffset 与 base 相加，并把负数截为零。
3. tierA 非零时使用 tierA 并作上限限制；否则有 encodedOffset 时还原偏移后与 base 相加，无有效偏移时使用 default。
4. 最后按上下限裁剪。实现含无符号整数和负数特例，精确行为以反编译和汇编为准。

以下均为 **P 的字节偏移**：

| AC 项 | base | encodedOffset | default | tierA | tierB | highest | upper/lowerOffset | 最终结果 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| cTGP | +1c4 | +2a8 | +1c4 | +27c | +334 | +10c | +24 / +28 | +54 |
| TPP | 最终 cTGP(+54) | +2b0 | +1c0 | +278 | +330 | +108 | +3c / +40 | +50 |
| minOutput | 最终 cTGP(+54) | +2b8 | +1cc | +284 | +33c | +114 | +2c / +30 | +5c |
| maxOutput | 最终 cTGP(+54) | +2b4 | +1d0 | +288 | +340 | +118 | +34 / +38 | +60 |

highest 是更高层非零字段的选择结果：TPP 从 P+558、+4a0、+3e8 顺序取首个非零；cTGP 为 +55c、+4a4、+3ec；minOutput 为 +564、+4ac、+3f4；maxOutput 为 +568、+4b0、+3f8。本轮未给这些层臆造来源名称。

各结果按 500 mW 舍入，随后再次裁剪。GPU 绝对上下界 P+a0/P+a4；TPP 还会被抬到 cTGP 或 minOutput，maxOutput 不高于 TPP。原始全部条件见 0x4dddf0 证据。

## 控制环与最终写入

`0x4dab30(C)` 周期执行时调用 `0x4ddc80(P,snapshot,state)`：snapshot 是 C+0x6c 处当前仲裁结果的副本，state=C+0x668。

- state+8（C+670）是 **CPU package 功率 mW**，经滤波写到 state+c（C+674）。上游 `0x4dd210` 通过两个 `pread` 读取计数和单位，按能量差/时间差算功率，主周期乘1000。初始化 `0x4dd38a` 明确将 Intel 采样器+20/+24设为 MSR `0x606`（RAPL_POWER_UNIT）/`0x611`（PKG_ENERGY_STATUS）；AMD 备用分支为 `0xc0010299`/`0xc001029b`。采样器=C+11f0，fd=C+11f8，两个寄存器地址=C+1210/+1214。原始初始化汇编保存在 `assembly/cpu-energy-sampler.asm`。
- 在 AC 控制分支，`error = snapshot+c - (state+c + state+4)`，即当前 TPP 减去滤波功率和另一功率测量值（C+66c）之和。
- 按正负误差使用两个增益，增加到 state+4 后，裁剪到 snapshot+18/+1c 的 min/maxOutput，写入 state+14（C+67c）。有关闭、未准备好及 DC 分支，不能只看主算式。
- `0x4da1b0(C)` 累积请求和上次成功值的差（C+11d4）；达到约 1000 mW 阈值或首次有效值时调用 `0x4dc1b0(C,P,requested_mW)`。
- `0x4dc1b0` 检查请求位于 P+214（min）和 P+210（max）之间，调用 RM `0x2080e634`，参数长度 0x188。参数块 +4 = `1 << *(uint32*)(C+1280)`、+c = 请求 mW、+10 = 0xf7。成功后 C+11d0 保存请求值。

本轮没有在公开同版本头文件中确认 `0x2080e634` 的正式 API 名称，故只记录实际命令编号和布局，不伪造符号名。

## 主任务可用的只读观察点

| 指令/函数 | 读取内容 |
| --- | --- |
| 0x4c50e3，indirect call 前 | rdi=worker，rax=vtable，*(rax+38)=实际回调；rsp 起 60 字节事件 |
| 0x4d46f3 | rbx=QBoostWorker；*(rbx+b0)=C，可判是否存在 controller |
| 0x4d9688，重算调用前 | rdi=P |
| 0x4d968d，重算返回 | rbx=C；C+78/+7c/+84/+88 是 AC TPP/cTGP/min/max |
| 0x4de2b0，TPP helper 调用前 | rdi=base,esi=encodedOffset,edx=default,ecx=tierA,r8d=tierB,r9d=highest；rsp+0/+8 是 upper/lowerOffset（进入 helper 后为 rsp+8/+16） |
| 0x4dc1b0 函数入口 | rdi=C,rsi=P,edx=requested_mW |

这些是断点位置和字段说明，不包含附加进程或写硬件的自动脚本。所有地址只适用于本目录记录的二进制哈希。

## 保存内容

`rea-evidence.json` 是本次 REA 原始证据导出；`assembly/` 保留关键函数的 objdump 反汇编；`decompiled/` 是从证据中逐函数提取的 Ghidra 伪代码；`SHA256SUMS` 对资料逐项校验。它们均为分析表示，不是 NVIDIA 原始源代码。

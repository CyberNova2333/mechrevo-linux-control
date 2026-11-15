# 固件到 NVIDIA 守护进程的联动链路

证据状态：2026-11-15，在 YAOSHI Series-XxAF55xx / N.1.33MRO91、Ubuntu 6.8.0-149、NVIDIA 595.99.02 上完成静态还原和短时动态观察。没有升级内核、替换 NVIDIA 二进制、修改 ACPI 表或发布新的 GUI 控件。

## 本轮确认的结果

此前的“75 W／145 W 预算没有落实”需要进一步限定：**静音与高能的参数都已通过驱动成功送达 powerd**，不是停留在 EC 或 ACPI 变量里。

GDB 仅观察原有 `0x13b` RM 查询的返回值；不修改返回值、寄存器或功耗上限。复用已经验证过的 GAMM/PMSF 路径，保持普通狂暴档，临时启用 APPE。静音保持约 5 秒，再恢复高能约 5 秒；没有运行压力负载。

| 观察项 | 静音 | 高能 |
| --- | --- | --- |
| EC TB43（0x728 bit 0） | 1 | 0 |
| EC ETPP（0x7f7） | 75 | 145 |
| NPCF ATPP | 600 | 1160 |
| RM subfunction 3 返回状态 | 0（成功） | 0（成功） |
| `targetTppOffsetmW` | 75000 | 145000 |
| `ctgpOffsetmW` | 50000 | 50000 |
| `maxOutputOffsetmW` | 25000 | 25000 |
| `minOutputOffsetmW` | -15000 | -15000 |

记录到 7 次 75000 和 13 次 145000 的成功动态读取。输入参数采用同版本 NVIDIA 官方头文件布局。原始记录为 `nvpcf-switch.txt`、`switch-firmware.json`；`summary.json` 是从原始记录重新计算的摘要。

NVIDIA 接口明确将 `targetTppOffsetmW` 定义为 **TPP 偏移量**，不能把 75 W 直接称为独立 GPU 上限，也不能仅凭这个值断言整机实际预算应为 75 W。换算后的目标值和约束执行仍待追踪。本轮没有重复 GPU 压力测试，因此实际功耗差异仍以之前约 140 W 的对照为准，不能宣布已修好。

## 官方控制中心的还原范围

原始 GCUService.exe SHA-256：`9ee018d42684d02032da642d90376843383223d76d82a6fba6ce42e475a82093`。此前已提取 5765 个受保护 CIL 方法体；本轮进一步完成：

- 静态解开字符串函数 `0x06001c6f` 的动态包装。内层 75 字节，SHA-256 `85e69314158f4e83a98e660fdf50526d19b0c399189e80ef318c0262e72374ae`。算法为 `(char)((s[i] + 10 + i) ^ (byte)(s.Length ^ 88))`。
- 从当前制品的 `0x04002a26` 字段及 RVA `0x8dbee8` 解析 782 条 delegate 元数据，所有目标 token 均有对应元数据。没有运行官方 Windows 服务或驱动。
- 在七个与 EC、模式、风扇和 GPU 功能有关的类中解码 2069 处字符串调用，将间接调用替换为含真实目标名称的分析表示。

`decoded-source/` 保存完整选定类的还原视图；`delegate-table.json` 提供 token、调用标志与名称映射。**这是反编译分析代码，不是厂商原始工程，也不是可以直接编译安装的完整 Linux 驱动。** 原始变量名、注释、所有运行时语义并不能从二进制保证恢复；仍有 ILSpy 的类型提示、编译器生成对象和少量混淆符号。

核心路径现已准确识别：

1. `UserSet_Cpu_Core_Offset_Extreme/Silent` 保留其他位，修改 EC `0x728 bit 0`。
2. `WMIEC` 连接 `root\\WMI`，对象 `AcpiTest_MULong.InstanceName='ACPI\\PNP0C14\\1_1'`。
3. 调用 `GetSetULong`，输入属性 `Data`：静音 `0x50000000009`，高能 `0x5000000000A`；输出读取 `Return`，转 UInt64，取低字节。
4. 固件 PMSF 结合 GAMM/CTAF、APPE 和 TB43 更新温控及 ETPP，经 NPCF 动态表和通知送到 NVIDIA 驱动。
5. powerd 的 `0x44b850` 先查询静态配置（subfunction 4），再读取动态配置（3）；`0x4c58b0` 比较新旧字段，变化时发往 `0x4c5090`。

已还原的自定义 GPU 接口另有 EC `0x743` 控制位、`0x744` cTGP 差值、`0x745` TPP 目标字段、`0x746` DB 增量字段。它们不是已验收的用户入口，本轮未写这些接口。不能为让“静音”看起来生效而擅自套用自定义限额。

## 反编译证据与限制

REA/Ghidra 12.2 分析的是本机原始 `/usr/bin/nvidia-powerd`，SHA-256 `db2643c4c37360e183230ea7f9c5da3e28c39ff79d08eba6ddb0f4f057d7bc74`。23 条 Evidence 保存在 `rea-evidence.json`；工具的 `unknowns: 0` 仅表示没有登记未决对象，不代表整个程序已分析完毕。

另一个 AutoFL/NvRm 初始化路径 `0x4b4370` 含 1000 W 默认 TPP 和 `max(default, MSR TDP + offset)`。汇编确认有 `maxsd`，但当前守护进程中没有找到对应对象；不能将这段静态代码当成本机当前 GPU 140 W 行为的根因。原拓扑兼容对照也未使静音实际功耗降低。

下一处明确的分析边界是 **`0x4c5090` 通知的消费者与实际功率控制器**：需追踪偏移如何组成最终预算、是否经过限幅，以及送往 RM 的控制命令。现有证据已排除本次正常开机下的动态表读取失败和全链路通知缺失。

同版本官方来源：[参数结构与偏移量定义](https://github.com/NVIDIA/open-gpu-kernel-modules/blob/595.99.02/src/common/sdk/nvidia/inc/ctrl/ctrl0000/ctrl0000system.h)、[动态表解析](https://github.com/NVIDIA/open-gpu-kernel-modules/blob/595.99.02/src/nvidia/src/kernel/rmapi/client_resource.c)。解析将有符号 16 位字段按 125 mW 单位转换，和本机观察一致。下载文件及哈希保存在 `nvidia-source/`。

## 恢复与失败记录

最终 APPE=0x80（原有其他位保留）、TB43=0、0x786=0；GATM/CSTM 恢复为 0，临时模块卸载，GDB 已 detach，原 powerd 服务保持运行。没有强制水冷状态、修改泵速或关闭机内风扇。

最初误取缓存中旧版研究模块，前置条件拒绝，未进入切档；随后从当前仓库已审查源码为现有 6.8.0-149 构建临时模块。一次未启用 APPE 的对照只改变 TB43/APTC，ETPP 不变，记录保存在 `inactive-appe-*`，不能作为有效预算切换。该轮遗留的未启用 APTC 低位已恢复，后续有效对照具备完整前后恢复记录。

保留本轮研究脚本供审计，不作为一键运行或常驻服务安装。原始固件表、OEM 可执行文件和内存内容不打入本次维护资料；保存的是选定代码视图、工具证据和精简参数记录。

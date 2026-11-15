# 自定义 GPU 功耗：官方调用链、本机表与待验证边界

2026-11-15。仅离线反编译、源码/AML 阅读；本子任务没有加载 OEM 驱动、调用 ACPI、写 EC 或部署服务。下文区分官方程序行为、固件静态定义与实机读数；写入顺序是分析结果，不是已经执行的测试。

## 证据入口

* 官方包 `vendor/extracted/app/AiStoneService/MyControlCenter/GCUService.exe`，SHA-256 `9ee018d42684d02032da642d90376843383223d76d82a6fba6ce42e475a82093`。解混淆分析图像仅用于 ILSpy；生成的 C# 是静态呈现，并非官方可重新构建源码。
* [GpuFeatures.cs](../firmware-link-20261115/decoded-source/GpuFeatures.cs)：408–473 为 EC setter。
* [manager-restored.cs](../firmware-link-20261115/decoded-source/manager-restored.cs)：2732/2785 状态发给 GUI；4635–4928 单项变更；5808 载入表；6271 默认档案；6846 初始化；7132 应用档案；7345 DB 开关；8590 OC；8653 游戏档。
* [SmartApcTableCtrl.cs](decoded-source/SmartApcTableCtrl.cs)、[AcpiCtrl.cs](decoded-source/AcpiCtrl.cs)：本轮独立解码。
* 当前固件反编译缓存 `/home/cybernova/.cache/mechrevo-linux-research/powerd-20261115/DSDT.dsl`，SMRW 在 102814，50 系选表在 102978–103097，EC 字段在 105061 起，PMSF 在 105615，_Q83/_Q84/_Q9C 在 106071 起。
* NPCF 定义在同目录 `current-Opt2Tabl.dsl`，1742 起；动态 PBD2 封装在 2070 起。

## 1. 本机身份、默认值与范围

主代理经过当前 helper 的机型验证后，只读提供：`EC[0x7d2]=0x19`、`EC[0x7e8]=0x18`、`0x743..0x746=0`、`0x7cc=0x81`、`0x490=7`。读数来源为主代理本轮采样，本子任务没有再读硬件。官方 `GetnModuleId_GPUFromEC` 读取地址 2002（0x7d2）并掩码 `0x1f`；AML `GFID` 也在 0x7d2，**不是 0x7d3**。**此处原先把 0x7e8 的十进制值 24 当成 APVersionCheck，已撤回。** 0x7e8 的官方常量名是 `ADDR_EC_DEFAULT_MODE`，随模式变化；真实 APVersionCheck 来自 UEFI 变量结构体，见第 8 节。主代理随后只读真实 UEFI，确认 `OemMagicVariable` payload 长度 180、`APVersionCheck=25`，`UniWillVariable` 不存在；本子任务未自行读硬件。

`SMAPCTABLE_STRUCT` 前九个字节按序为：

| 索引 | 官方字段 | 本机 GFID=0x19 的静态值 | 当前用途 |
|---|---|---:|---|
| 0–2 | PL1 / PL2 / PL4 | 依 ADPT 选表 | CPU 路径，不能在这里当作已验收瓦数 |
| 3 | TccOffset | 10 | CPU 温度偏移 |
| 4 | ConfigurableTGP | 50 | **可增加范围**，不是绝对 GPU 上限 |
| 5 | DynamicBoost | 25 | DB 增量上限 |
| 6 | TargetTemperature | 87 | 本轮不把它替代 NVAPI 查询出的温度范围 |
| 7 | DefaultTGP | 65 | CTGP 基数 |
| 8 | DynamicBoostCpuTdp | 255 | 在本轮主调用链未发现消费，不解释为 255 W |

表是 12 字节，最后四字节均 FF；结构体定义只有上述九个 byte。对本机 GFID，SMRW 的实际条件顺序为：

| 条件 | 表名 | 原始 12 字节 |
|---|---|---|
| ADPT=0x0a | G2A7 | `91 91 91 0a 32 19 57 41 ff ff ff ff` |
| 否则 ADPT=0x08 | G287 | `af af af 0a 32 19 57 41 ff ff ff ff` |
| 否则 ADPT=0x08 且 MOID=6 且 ARLR | G2Y7 | 被上一分支覆盖，当前方法不可达 |
| 其它 | G2X7 | `d2 d2 d2 0a 32 19 57 41 ff ff ff ff` |

ADPT 是 `EC[0x49f]` 的位 3..6，不能拿 0x490 的圆口状态替代。三条可达分支的 GPU base/range/DB 完全一致，故 GPU 静态范围不依赖尚未提供的 ADPT。GFID=0x1a 的对应 GPU 字段为 80/70/25，0x1b 为 95/55/25；这两组不是本机范围。

官方 `LoadProfileAll` 计算：`CTGPmaximum = DefaultTGP + ConfigurableTGP`，`CTGPsupport = range > 0`，`DBsupport = DynamicBoost > 0`。GUI 最小/最大为 CTGP **65..115**、DB **5..25**。对 DB 关闭有单独开关，不用把 0 塞进开启时的滑块范围。默认档案 APVersion>=24 最后覆盖为 `DynamicBoostSwitch=0`、`DynamicBoost=5`、GPU 时钟 offset=0；所有版本 `DynamicBoostTotalProcessingPowerTarget=255`。主代理已只读确认真实 UEFI APVersionCheck=25，故本机新建自定义档为 CTGP 115、DB 关闭、DB 保存值 5、TPP 保存值 255。DB 关闭实际写入的 TPP/DB 是 0，而不是 255/5。

主代理随后以固定0xbb只读探针实读SMRW，GFID=25、ADPT=0，结果为 `d2 d2 d2 0a 32 19 57 41 ff ff ff ff`，与G2X7一致；探针已卸载。见[运行表证据](hardware-validation/smrw-readonly.json)。命名buffer仍可能被OEM 0xaa路径修改，因此不能把此快照当所有开机永不变的常量，不开放任意SMRW写入。

## 2. 表怎样进入官方程序

`SmartApcTableCtrl.GetSMAPCTable()` 调 `GetTableByDriver()`；`SupportSmartApcTable` 默认 1。类中旧 GN20 SKU/项目列表没有在此路径实际构成门禁，不能仅根据那些列表判本机不支持。

`GetTableByDriver()` 按偏移 0..11 逐次读取，取每次返回 DWORD 的第一个字节，最后做 `BytesToStructure<SMAPCTABLE_STRUCT>`。每次锁等待最多 1 秒，获取失败会跳过该项；异常会被吞掉，外层可返回零结构体。这不是可靠的新实现错误处理示范。

`AcpiCtrl.ReadSmartApcTableByKey` 打开 `\\.\ACPIDriver`，输入两个字节 `{0xbb, offset}`，调用 IOCTL **0x9c40a500**，期待四字节返回。失败缓冲区初始化为 FF。同包 UWACPIDriver.sys（46,352 字节，SHA-256 `6e360ee4a0c42b2a4eaf46208c1592c87f8cd71aca2a6c52831a2ad356e61235`）与既有离线研究样本逐字节一致；其该 IOCTL 分支构造 SMRW 请求。既有静态反汇编指出其 ACPI buffer 长度为 4。当前 AML 却创建覆盖 13 字节的字段：RWFG 1 字节、重叠的 REOF 与 12 字节 WRBF。未在 Windows 执行此路径，不据此宣称 OEM 运行失败。

Linux受限读取的确切接口是 `\_SB.INOU.SMRW`，单个ACPI_TYPE_BUFFER参数，13字节零填充，buf[0]=0xbb、buf[1]=0/4/8；返回必须为ACPI_TYPE_INTEGER且不大于0xffffffff，依小端把三DWORD拼成12字节。方法内RWFG覆盖bit0..7、REOF覆盖bit8..15、WRBF覆盖bit8..103，所以不能把官方驱动短输入长度照搬为2/4字节ACPI参数。读取GFID与ADPT的路径分别为 `\_SB.PC00.LPCB.EC0.GFID` 与 `.ADPT`。主代理的一次性[固定只读探针](table-probe/mechrevo_gpu_table_probe.c)已实读并卸载；没有提供用户命令或写接口。绝不能复用允许 0xaa 的通用 RPC。SMRW 的最后三种逐字节读法（偏移 9/10/11 建 DWORD）越过 12 字节表尾，也不应复制。

`LoadProfileAll` 一开始读表并写注册表，之后才依据 ApUseFlag 分支再读或从注册表取七个主要字段。失效检查接受的条件较宽：PL4=0 或七字段全 FF 才判表失效；随后七字段各自 FF 转 0。DebugMode=1 会强制写一组 115/20/15 等测试表，**不是本机默认表**，不可拿来做范围。

## 3. EC 寄存器契约

| Setter | 字段 | 写法/单位 |
|---|---|---|
| SetGpuConfigurableTgpFunCtrlEnable | 0x743 bit2 | 读改写，status==1 置位，保留其余位 |
| SetGpuConfigurableTGPTarget(target,base) | 0x744 CTVA | `target-base`，转换 byte；低于 base 则不写；没有自身上界钳制 |
| SetGpuDynamicBoostFunCtrlEnable | 0x743 bit0 GNEN | 功能控制开关，保留其余位 |
| SetGpuDynamicBoostEnable | 0x743 bit1 ECDC | DB 开关，保留其余位 |
| SetGpuDynamicBoostTotalProcessingPowerTarget | 0x745 DBCT | 直接 byte；本轮不能证明自定义档下它等同 NPCF.ATPP |
| SetGpuDynamicBoostMaxinumTGP | 0x746 MXDB | 直接 byte，官方传入 DB 增量，不是绝对 GPU TGP |
| SetSuperPerformance | 0x728 bit1 CMSP | 独立于静音/高能的 bit0；实验不应顺便开启 |

`Convert.ToByte` 对溢出抛异常，并非截断；旧服务部分入口缺少合法范围校验，新实现应在任何副作用前校验完整请求。0x743 bit2 在当前 AML 未命名，但官方 EC setter 明确存在；只以 AML 命名字段枚举功能会漏掉它。

## 4. 精确调用顺序

`Init()` 先刷新选中档案，再 `SetUserProfile(mode,isManual)`，最后决定功能开关。操作模式整数是 0=办公、1=均衡、2=狂暴、3=自定义，档案名却是 Mode2=办公、Mode1=均衡、Mode3=狂暴、Mode4=自定义，不可混用编号。

在 `SetUserProfile` 内，先按需 `SetFanMode`，恢复保护/用户风扇表，应用 CPU 字段；GPU 段在 `m_OcSupport && m_IsNvGpu` 下。时钟、温度、部分 OC 行为另受 `supportGPU` 控制。CTGP/DB 写入直接受 `PowerModeEvent.g_ACLineStatus == 1` 控制，而不是 `supportGPU`；这是 Windows AC 电源状态。GUI 的 IsAC 在 APVersion>=24 又检查圆口状态，二者不是同一个变量。

自定义、AC、非 OC、DB 关闭时的相关顺序：

1. CTGP 支持时写 `0x744 = min(profile.target, maximum)-base`。这里只有上界钳制；低于 base 则底层不写。
2. DB 支持且自定义时，`SetGpuDynamicBoostSwitch(0)`：0x745=0 → 0x746=0 → 0x743 bit1 清零。
3. 0x743 bit0 置一。
4. 返回 Init，0x743 bit0 再置一 → bit2 置一。

自定义、AC、非 OC、DB 开启时，仅第 2 项变为：写 profile.TPP → profile.DB → bit1 置一，之后同样启用 bit0、bit2。OC 已开启时 `SetGpuDynamicBoostSwitch` 忽略关闭请求，仍启用 DB；上层还可能把 DB 值覆盖成表最大值。因此验收必须明确 OC 关闭，不能把 OC 路径当成开关失灵。

普通三档 Init 的最后动作恒为 bit0 清零 → bit1 清零 → bit2 清零。SetUserProfile 在 DC 不执行上述 CTGP/DB 内容，只关闭 Whisper；不能因此声称服务在 DC 自动清空这四个字节。需连同 PowerModeEvent/模式切换的整体事件路径验证。

其它入口与差异：

| 官方入口 | 顺序/边界 |
|---|---|
| 用户单项 CTGP target | 保存档案字段 → 直接 setter；没有 SetUserProfile 的上界钳制 |
| 用户单项 DB switch | 保存 → 上述 TPP、DB、开关顺序 |
| 用户单项 DB value | 保存；OC=0 写所选值，OC=1 写表 DBmax |
| 用户单项 TPP | 保存 → 直接 0x745 byte，无确认过的上层范围 |
| UserSet_Mode_Detail | 深层 else-if，一次消息只处理首个匹配字段，不适合一次传五字段冒充事务 |
| SetGameMode(onoff,ctgp,db) | bit2 → target(base+ctgp，即 offset 参数) → TPP/0 → bit1 → MXDB → CMSP=false；不在这里置 bit0 |
| SetGameMode(GameProfile) | DB 用 Clamp(5,table.DB)；CTGP 日志写 Clamp 但未见真正钳制，不能复制这个漏洞 |
| LoadOverClockingValue(1) | bit2=1 → CTGP=max → bit1=1 → DBmax → CMSP=true；还有 CPU/显存副作用，不是纯 GPU 限制接口 |
| UserSet_HwOCMode 开启 | 切狂暴 → bit2=1 → CTGP=max+DB → bit1=0 → GPU OC；超出普通 CTGP 滑块范围的特殊路径，不纳入默认滑块 |
| SetDynamicBoostforTurboMode | bit1=1 → DBmax |
| TriggerCTGPforTurboMode | 仅 bit2=1 |

另有新字段 `CustomTGPinGCUforGN21_Value`：处理器在 manager 5536–5538 拼接并执行 `nvidia-smi -pl <int>`；`CustomTGPinGCUforGN21_Enable` 分支只存字段，未见硬件操作。此路径与 EC 的 configurable TGP 是两条通道；Linux 已有 nvidia-smi 不代表 EC 路径已复刻，也不应让它们竞争写入。

## 5. EC → AML → NVIDIA 的转换及空白

### 写入没有隐藏的“提交”调用

继续追到实际传输后确认：`MyEcCtrl.Write` → `AcpiCtrl.Write` → `WriteACPI(0x9c40a48c,addr,data)`。后者把两个 int 作为 8 字节输入，发给 UWACPIDriver。驱动 IOCTL 分派 RVA 0x29cc → 0x2b05 → 0x25b8；在 RVA 0x25e0 构造方法名 ECRW、两个 ACPI integer 参数，再发 IOCTL 0x32c004。当前 AML `ECRW` 只把地址加 0xfe410000 后执行 MMRW 的 byte 写入，**不调用 PMSF、_Q83 或其它 WMI 方法**。AcpiCtrl 默认每次读/写后休眠 10 ms；错误常被吞掉，因此 Linux 实现应加强逐次回读而非复制错误处理。独立反汇编证据见 [ecrw-driver.disasm.txt](ecrw-driver.disasm.txt)。

官方自定义标志由 SetFanMode(3) 设置：CustomerModeLightOn 置 0x727 bit6/CUME，0x751=0（无 FanBoost），随后 0x726 bit7=1。它没有把 `SetCustomModetoEC` 的 0x706=0x41 当成本入口步骤；应继续复用已验收的 Linux set_mode(custom)，不要额外猜写 0x706。

APPE(0x741 bit0)是OEM已运行上下文，完整App初始化见第8节。GPU setter不写APPE，AML只有字段声明；**主代理后续单变量对照现已证明本机APPE是这些自定义GPU请求生效的关键条件**，见第9节。同一100W请求、相同DB关闭字节，APPE0时派生值不变，APPE1后派生值和实耗均改变。不能在有残留CPU/风扇目标或外来控制器时贸然开启这个应用存在标志。

0x743/744/745/746 在 AML 只有 field 声明，没有找到直接 consumer；输入如何变为下面的运行字段，属于 EC 固件逻辑，本轮没有该 EC 的解包源码。不可推断 `0x744 == 0x788` 恒成立。

可确认的 AML 出口：

* `_Q83`：`NPCF.ACBT = EC.CTWA(0x788) * 8`，然后 `INOU.PWUP(OCPL)`。
* `_Q84`：`DBEN(0x7c3 bit3)==1` 时 `NPCF.DBAC=0; NPCF.AMAT=DBAP(0x7d5)*8`；否则 DBAC=1，不清旧 AMAT。
* `_Q9C`：仅 CUME(0x727 bit6)=0 时 `ATPP = ETPP(0x7f7)*8`。其中 `(ETPP != 0) || (ETPP != 255)` 在 byte 域恒真，是固件原文；不悄悄“修正”为 AND。
* `PMSF` 自定义 CUME=1 分支：GFID=0x19 默认 ATPP=0x4d8（1240 个八分之一单位，即 155 W），0x1a/1b 为 0x640。若 MOID=6 且 ARLR，0x19 在 ADPT=0x0a 时改 0x2f8（95 W），否则 0x500（160 W）。其它 GPU 有其它分支，不适用于本机 GFID。
* PMSF 最后调用 _Q83、_Q84、_Q9C。自定义时 _Q9C 不以 ETPP 覆盖上面的常量，因此 **DBCT=255 不等于 ATPP=255W**，也未证明手调 DBCT 能在本机改变自定义 ATPP。
* PWUP 通知 NPCF 0xc0 与 GPU 0xc5。NPCF PBD2 中 TGPA=ACBT（CTGP 开启时）、TPPA=ATPP、MAGA=AMAT，DBAC/DBDC 打包进控制位。具体消费者启用与限值生效由并行 backend 研究/实机验证负责。

OEM setter 没有显式触发 PMSF；EC 可通过 _Q83、_Q84 等事件更新派生参数。单独调用 PMSF 只会消费当前 CTWA/DBAP 等值，不会把 CTVA 输入转换成 CTWA；因此 0x744 已改而 0x788 不变时，不能用反复刷新 PMSF 掩盖 EC 层未响应。如果 0x788 已改、ACBT 仍旧，才属于 AML 事件传播/刷新这一层的隔离问题。只调用 PWUP 是通知，不重算 ACBT。

这个方向说明“写入成功”至少分为五层：请求寄存器回读 → EC 计算字段改变 → AML/NPCF 参数改变 → NVIDIA 接受并报告 enforced limit → 相同负载下实耗变化。前四层任一失败，都不能用 GUI 按钮成功取代验收。

## 6. 首次最小下调试验及后续验证边界

主代理已执行首次 115→100→115 下调试验，快照与 36 个样本已保存在 `cache/control-replica-20261115/custom-gpu-first`；请求字节按预期改变，但派生 CTWA 未变，完整恢复报告通过，详见第 8 节。首次下调实效未成立；后续APPE单变量对照已证明100/115W生效，运行SMRW也已只读验证，见第9节。GPU base/range/DB 为65/50/25。应检查圆口供电、GPU 身份、当前模式与 OC/保护状态。为避免两个控制器相互覆盖，实验期间只保留一个明确拥有这些字段的 backend。不要新开 OC，不动 CPU 或风扇曲线，不把 TPP 255 暴露成可选“瓦数”。

同一自定义模式、DB 关闭背景下，以默认 CTGP=115 作基线，仅改 target 为 100（0x744=35），比较前述五层。若没有真实下调，首先停止扩大写入范围并恢复，分析哪一层未响应；不叠加更高 DB 或把特定驱动接口失败当作继续加压的理由。100 是本机静态范围内的可辨识下调试点，不是已确认固件必然执行的安全限值。

实施方必须先把 0x743..0x746 纳入有超时/崩溃恢复能力的事务，快照原模式及其它实际受影响字段。若原来 bit0/bit2 为 0，恢复时先撤销实验启用位，再恢复目标/DB/TPP，最后保留其它位地恢复功能掩码与原模式；若原先已开启，恢复顺序需避免在旧开关有效期间装载不相配的目标。模式恢复可能重写 EC 字段，最终必须回读全套比较。恢复失败要留下明确失败状态，不标已恢复；不能仅依靠 Python finally。

记录固定负载、采样时长、温度、GPU utilization、requested/enforced limit、实耗分布及原始 EC/NPCF 快照。低利用率或温度限频时即使功耗下降也不能证明 CTGP 成功。真实待机/唤醒、热插拔、daemon 崩溃另做验收，本轮未执行。

候选脚本 [ctgp-trial.py](ctgp-trial.py) **已由主代理执行首次硬件试验**，本子任务仅只读其结果；不带 `--execute` 仍只输出计划。它复用已安装 helper 的自定义入口，限制写入七个地址/掩码，固定 115→100→115 各 12 样本；保留 APPE、不调用 PMSF、不修改 CPU/风扇或 Logo。每次写后等待至少 10 ms 并回读。独立 fork 子进程持有继承的 profile flock，在父进程退出或心跳超时后做掩码恢复；超时时先终止自身父试验进程，避免父进程恢复运行后再次写入。子进程也具有 65 秒绝对期限，不声称可处理内核崩溃或断电。guard 协议只保留各寄存器的最后确认值，以及至多一个待确认写入；写前发送 pending，回读成功后发送 confirmed 并立即丢弃旧值，不再接受全部历史值。恢复进度也逐项推进 confirmed/pending，使写入接受后报错仍可重试，不把自己的部分恢复误判为外部接管。每步恢复重读 APPE 和整个相关组；APPE 改变停止所有写，GPU 外来值同时保留其自定义模式上下文，模式键只保留模式组并撤销仍属于试验的 GPU 请求。结果包含未完全恢复原因，不能忽略。用户态逐步读取无法消除读写之间的全部硬件竞态，profile flock 只协调遵守它的软件。结果输出在 root 专用 `/run/mechrevo-custom-gpu-trial-PID`。

本轮通过 [test_ctgp_trial.py](../../tests/test_ctgp_trial.py) 的 18 项离线测试：完整掩码恢复、模式键保留、APPE/GPU 中途接管、已过期历史值拒绝、pending/confirmed 实际发送顺序、写入接受后报错及拒绝继续写入、全部恢复 I/O 位置逐一注入接受前/后失败并重试、参数与 512 组圆口条件，以及模拟 EOF、guard 输入异常时仍停止负载并恢复的分支。这些测试没有打开硬件、创建真实 fork、发送真实信号或运行 GPU 负载；主代理首次硬件试验已覆盖正常父进程 EOF 后 guard 恢复；故意终止父进程、超时、真实睡眠/热插拔仍不能由这次正常恢复推定已验收。

## 7. GUI 接入边界

| 项目 | 当前证据 | 接入条件 |
|---|---|---|
| CTGP 65..115 | SMRW实表、APPE单变量、65/100/115W实效与回退均通过 | 真实helper/GUI已安装，真实pkexec联合负载、reset完整恢复及旧CPU-only兼容最终复测通过 |
| DB 开关与 5..25 | DB5/15/25派生DBAP与实耗随动、关闭归回100W、完整恢复均通过 | TPP固定255；生产入口已安装，最终撤销/有界等锁复测通过 |
| TPP 自由滑块 | 仅 byte setter 与 255 默认，缺本机 custom 转换证据 | 继续保持不可用；显示未知/固件管理 |
| GPU OC / CMSP | 独立特殊分支且联动 CPU/显存 | 本轮不自动纳入，不能通过“复刻功耗”隐式启用 |
| nvidia-smi -pl | 官方另有独立分支 | 显示与 EC 控制的不同来源，避免双写 |

没有用 DebugMode 示例、其它机型报告、表尾 FF 或单次字节回读补齐未知范围；没有升级内核。


## 8. 首次实测后的 APVersion 来源纠正与门禁定位

### APVersionCheck 的确切来源

对原包同目录 `UEFI_Firmware.dll` 继续静态反汇编，SHA-256 为 `4cfe064827dad6da7bf6fb9fc07dc00b4032f1881331d59bfa8d5937a6d534be`。`ReadUefi` 导出 RVA `0x2190` 在地址 `0x1800021d5` 调用 `GetFirmwareEnvironmentVariableW`（IAT `0x18001d090`），变量名先为 `OemMagicVariable`，失败才在 `0x18000221e` 尝试 `UniWillVariable`；两者 GUID 均为 `{9f33f85c-13ca-4fd1-9c4a-96217722c593}`，读取缓冲区 512 字节。此方法不通过 EC 0x7e8 获取版本。

官方 `NvramVariable.GetFwVars`（缓存 `logo-20261115/NvramVariable.cs:354–417`）调用 `ReadUefi(0,NVRAM_STRUCT_SIZE)`，再把返回缓冲区 Marshal 成 `NVRAM_STRUCT`。`Get`/`GetAPVersionCheck` 读取其中的 `APVersionCheck` 字段，并把值写入 `Support\APVersionCheck` 注册表项。新导出的 [NVRAM_STRUCT.cs](decoded-source/NVRAM_STRUCT.cs) 保留该类型声明；metadata TypeDef `0x02000266` 是 SequentialLayout，没有 ClassLayout 覆盖，字段 `0x04001354` 为 byte（signature `0605`）。按声明和 ByValArray 长度计算，APVersionCheck 位于 payload **0x43**；Linux efivarfs 文件前有 4 字节 attributes，因此对应文件 **0x47**。只读核验应按同样的变量优先级读取，检查有效 payload 长度至少 68 字节，不用假定 Windows 注册表默认值或 EC 动态字段。

结构计算与二进制哈希保存在 [apversion-source.json](apversion-source.json)，导出函数反汇编为 [readuefi.disasm.txt](readuefi.disasm.txt)。本子任务未读取真实 efivarfs、未调用 DLL、未写 NVRAM。

完整服务元数据分析视图 `firmware-link-20261115/decoded-source/GCUService.cs:871` 明确列出 `ADDR_EC_DEFAULT_MODE = 2024`。恢复源码 `manager-restored.cs:2656/2716/7517` 会在某些禁用、恢复默认或首次启动分支把该地址写 0。这与实测普通狂暴 0x18、自定义 0x28 的变化一致地排除了“固件版本寄存器”解释；具体每个位的意义仍未完全证明，不另行命名。

`ctgp-trial.py` 中原有 `before[0x7e8] == 24` 及 `APVersion=24` 文案同样是错误关联，已向主代理报告。本文不会通过写入 0x7e8 来满足这个错误门禁；试验代码修改和后续硬件验证由主代理负责。

### 首次试验已经排除和未排除的条件

只读分析主代理提供的 `cache/control-replica-20261115/custom-gpu-first/{before,samples,restoration}.json`：36 个样本中，三段 CTVA 为 50→35→50，CTWA 始终 50；CUME=1、0x726 bit7=1，0x743=5，即 GNEN=1、ECDC=0、CTGP function=1；DBCT/MXDB 均为 0。APPE 字节始终 0x80，因此 APPE bit0 **始终为 0**。进入自定义后 DBAP 从 25 降为 15，而不是跟随 DB 关闭变为 0。恢复报告 complete=true 且无 skipped/errors；本子任务仅引用报告，不把它扩展为未记录项目的实测。

静态 SMRW base=65、CTWA=50、DBAP=15 与稳态约 130 W 相符；普通狂暴对应的 65+50+25 与约 140 W 相符。这是对读数的解释，不是 GPU 驱动暴露的严格上限证明。重点是 **CTGP 下调和 DB 关闭两类用户输入均未进入派生状态，但固件的档位变化已生效**，故问题位置早于 `_Q83`/`_Q84`，不能用重复 PMSF/PWUP 证明输入已生效。

官方 Init 的 `SetUserProfile`→GNEN=1→CTGP function=1 与当前 0x743=5 的终态一致。直接写入传输只有 ECRW 字节写，没有找到 CTVA 专属 commit。额外的官方应用上下文有：

* `MyFanTableCtrl.FanTable_Refresh`→`RefreshDefaultFanTableAll` 获取默认风扇表时先 APPE=0，完成后 APPE=1（`fanmanager-restored.cs:571–574,2454–2475`）。`Set_APExistToEC` 是对 0x741 bit0 的读改写（`ec-restored.cs:585–595`）。
* `LoadProfileAll` 在 `ApUseFlag!=1` 时保存 UEFI `ApUseFlag=1`（`manager-restored.cs:5831–5836`）。`syncBiosSettings` 在同步模式前保存 UEFI `ApExistFlag=1`（6796–6801）。这些是持久化 BIOS 变量，与 EC APPE 分属两条路径；没有证据允许把它们等同，也不应为试探 GPU 随意改整个变量。
* `SetUserProfile` 还加载已有风扇表并应用 CPU profile，然后才写 GPU 输入。它们构成完整官方上下文，不能为了模拟这一步顺便启用 CPU/GPU 超频或复制未知风扇目标。

对当前目录所有已解编译 AML 的检索结果，APPE、GNEN、ECDC、CTVA、DBCT、MXDB 仅有 field 声明；CTWA 唯一的消费是 `_Q83`。因此缺失的 **CTVA→CTWA / MXDB→DBAP 门禁位于 EC 固件逻辑**，现有 DSDT 和 Windows 控制中心代码不能给出该 MCU 内部条件的源码。在首次试验后的静态分析阶段，APPE只是明确的OEM上下文差异，不能单凭源码宣称因果。主代理随后沿用单一控制者、所有权保护和恢复逻辑完成隔离对照，实测因果证据已补在第9/10节；仍没有EC MCU内部源码。


### 后续补充：完整应用生命周期与两个 UEFI flag

主代理的真实只读 UEFI 结果为 `OemMagicVariable` payload=180、APVersionCheck=25，且未找到备用 `UniWillVariable`。这个结果替代先前错误的 EC=24 推断。官方本版托管结构完整大小计算为188字节，不应因此错误拒绝实际180字节payload中的版本读取：版本字段本身仅要求至少68字节，不能依据未使用的结构尾部盲读或改写。

对 5,765 个恢复 CIL 方法进行 call/callvirt/xref 检索后，新增 [application-context-xrefs.json](application-context-xrefs.json) 和 [App.cs](decoded-source/App.cs)。比此前“获取风扇表后 APPE=1”的结论更完整的顺序是：

1. `App.StartMainWork`（App.cs:1434–1438）先调用 `InitalizeStartPerparation`。
2. `InitalizeStartPerparation`（1229–1260）先 `FanTable_Init`，再**无条件** `Set_APExistToEC(true)`，接着机型/电源/WMI监听初始化。
3. 之后 `StartUpTask`（1500–1504）才启动 `MyFan.EnableByService`。该方法先 `LoadProfileAll`，再 `syncBiosSettings`，最后按 APVersion 进入 `SwitchModePowerChange(isInit:true)` / `Init`、加载 profile 与 FanBoost。
4. `ModernOn`（App.cs:110–121）刷新通用电源、APPE=1、`MyFan.Resume`，随后重启双负载调度。
5. `Application_Exit`（1201–1222）先禁用风扇/灯光等组件，再 APPE=0 并卸载 EC 驱动；卸载启动分支（713–720）也是 APPE=0。
6. `Set_APExistToEC` 仅对 0x741 bit0 读改写；其异步任务等100ms后只回读日志，没有第二次隐藏提交。

`FanTable_Init` 并非每次都会从 EC 拉取表：EC版本变化、本地表缺失/无效/全零时才 `FanTable_Refresh`，否则载入保存的20份表。刷新路径临时 APPE=0，按 Gaming/Office/Turbo 发送 EC 0xf5f=2/3/1、0xf5d=0xfd、0xf5e=0xc9，请求默认表；每500ms检查，最多10次，读回并保存后 APPE=1。无论是否走这条刷新路径，外层 App 都会重新置 APPE=1。因此 APPE 是**服务运行存在状态**，不是 CTGP setter 末尾的专用 commit。

两个同名相近字段必须区分：

| 字段 | 官方保存与读取路径 | 已确认 EC 对应关系 |
|---|---|---|
| `NVRAM_STRUCT.ApUseFlag`（payload0x61） | `LoadProfileAll` 首用时置1、决定表从BIOS/本地注册表选取；卸载置0 | 未找到用户态直接EC转发；不能把它当APPE |
| `NVRAM_STRUCT.ApExistFlag`（payload0x5d） | `syncBiosSettings`置1，卸载置0；通过NvramVariable.WriteUefi保存 | 未找到此setter调用Set_APExistToEC；固件是否内部同步仍未知 |
| `EC APPE`（0x741 bit0） | 上述App生命周期直接Set_APExistToEC，退出清0 | 明确的运行期EC控制位，可独立于两项持久变量存在 |

`WriteUefi` 导出 RVA0x2330 的静态反汇编保存为 [writeuefi.disasm.txt](writeuefi.disasm.txt)：读取同GUID的变量，合并输入区间，调用 `SetFirmwareEnvironmentVariableW`（IAT0x18001d108）。没有出现 ECRW/EC 地址写入，也没有由字段名分派到EC setter。UEFI运行时服务背后的固件/SMM处理不在现有样本内，故不能据此宣称固件绝不传播；只能说当前用户态实现里没有直接EC通路。本次没有执行UEFI写入。


## 9. APPE单变量突破与DB后续边界

主代理执行 `ctgp-appe-trial.py` 后，保存32个真实样本及正常EOF完整回退记录；本子任务仅复制、读取和汇总。原始root目录为 `/run/mechrevo-custom-gpu-appe-trial-133189`，持久证据见 [APPE对照摘要](hardware-validation/appe-summary.json)、[全部样本](hardware-validation/appe-contrast/samples.json)和[恢复记录](hardware-validation/appe-contrast/restoration.json)。下表功耗均取每阶段最后5个样本平均，排除切换边界仍保留上一策略的短暂读数。

| APPE bit0 | 请求CTGP | 请求DB | 派生CTWA / DBAP | 真实GPU功耗 |
| --- | ---: | --- | --- | ---: |
| 0 | 100W | 关闭 | 50 / 15 | 129.444W |
| 1 | 100W | 关闭 | 35 / 0 | 99.638W |
| 1 | 115W | 关闭 | 50 / 0 | 115.148W |

100W两组之间仅APPE bit0变化，CTVA=35、743低3位=5、745/746=0保持不变；证明原因位于EC应用存在门禁，不是必须反复PMSF。再保持APPE1把CTVA35→50，可见GPU实耗随100→115W。最高观察CPU70°C/GPU53°C由主代理汇总，未改CPU/风扇/Logo或UEFI。完整回退没有错误或跳过；这是正常EOF路径，不把它扩大为物理断电或内核故障验收。

### TPP255能证实到哪一步

`manager-restored.cs:6460`对默认profile无条件设 `DynamicBoostTotalProcessingPowerTarget=255`；7345的DB开启顺序为745=profile.TPP、746=profile.DB、743 bit1置一，关闭则745=0、746=0、bit1清零。`GpuFeatures.cs:462`仅直接byte写745，没有把255转换为瓦数或其它值。现有AML中的DBCT/MXDB没有直接消费者；custom CUME=1时_Q9C不把ETPP覆盖ATPP，因此**255是已证实的OEM默认协议值，不能据此称255W或“无限功率”**。其EC内部特殊值语义仍待固件/对照证据。

建议后续固定CTGP100、APPE1，DB关闭基线→5→15→25→关闭，TPP仅在DB开启时固定255，保持CPU/风扇无残留、无OC；每档观察请求745/746/743bit1、派生7c3bit3/7d5与788、驱动limit及实际功耗。按base+CTWA+DBAP推断应约100/105/115/125/100W，但只有实测才能确认。后续 `ctgp-db-trial.py`已完成全部42个真实样本，包括DB关闭下65/115W端点，见第10节。生产助手／GUI的事务集成和组合验收另由主任务完成。


## 10. DB与CTGP范围实测

主代理已执行独立 `ctgp-db-trial.py`，固定APPE1、TPP=255（仅DB开启时）、CTGP100下DB关闭→5→15→25→关闭，再测DB关闭下CTGP65和115。全部7阶段各6样本，共42样本，未触发60秒guard期限。见[全部样本](hardware-validation/db-range/samples.json)、[按阶段摘要](hardware-validation/db-range-summary.json)、[完整回退](hardware-validation/db-range/restoration.json)。本子任务仅归档和统计，没有运行硬件试验。

下表取每阶段最后4个样本平均；初始加载与刚切档样本不混入稳态均值。第一阶段仍在GPU负载预热/预算过渡，不能把那6个瞬态样本当额外上限验收；100W基线已有第9节APPE对照和本轮DB关闭后的重复结果。

| CTGP请求 | DB增量 | 745协议值 | 派生CTWA / DBAP | GPU实耗 |
| ---: | ---: | ---: | --- | ---: |
| 100W | 5W | 255 | 35 / 5 | 105.548W |
| 100W | 15W | 255 | 35 / 15 | 115.345W |
| 100W | 25W | 255 | 35 / 25 | 125.048W |
| 100W | 关闭 | 0 | 35 / 0 | 100.273W |
| 65W | 关闭 | 0 | 0 / 0 | 64.998W |
| 115W | 关闭 | 0 | 50 / 0 | 115.620W |

请求→EC派生→实际功耗的链路现已成立，运行表也已确认base65、range50、DBmax25。因此可据此实现 **CTGP65..115W，DB关闭或5..25W，TPP固定官方255** 的受限控制；不提供TPP自由瓦数输入，不开放超频或超出表范围。测试覆盖DB代表点5/15/25及CTGP端点/100W，不声称遍历了每个整数或同时最高115+25W组合。最高CPU70°C/GPU54°C。恢复complete=true、errors/skipped均空，主代理已恢复未选档daemon。真实helper／GUI现已安装，前台锁争用修复后，主代理再次通过真实pkexec完成生产入口最终验收：CPU35W+GPU100W/DB15W联合负载12样本，末10样本均值CPU34.981W/GPU115.026W，最高CPU/GPU均51°C。reset成功归还全部拥有掩码、风扇RAM和Logo共享位；旧CPU-only配置再apply/reset也成功，journal不含GPU地址、GPU字节未改变，最终全部归还且journal消失。见[生产验收原始记录](hardware-validation/custom-production.json)和[摘要](hardware-validation/custom-production-summary.json)。最终已部署helper SHA-256为 `c241582401674b3c03469504f6b73c7d467651b89be0f2e8b4b656f16b788177`；主任务报告前台1.5秒有界等锁专项19项测试通过，此前全套316项通过。当前接口、v2 journal/旧v1兼容与恢复保护见[后端契约](../../system/mechrevo-custom-gpu-backend.md)。这批生产入口验收仍不包含真实睡眠、物理AC切换或水冷BLE。

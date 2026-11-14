# 官方自定义、狂暴子模式与水冷联动研究

2026-11-14；本机 YAOSHI Series-XxAF55xx，BIOS N.1.33MRO91。
用户确认使用原装外置水冷，目前尚未连接。本次只分析文件与只读 EC，没有修改硬件设置。

## 本次新增证据与之前结论的修正

官方包 ControlCenterX 5.56.60.46 中的 GCUService.exe 核心方法仍受保护；但 CCU.WinUI.msix 内
CCUWinUI.dll 的前端业务逻辑可完整反编译。新的结论来自这份官方前端及本机 DSDT，而不依赖其他型号推断。

**重要修正**：不能把本版“静音狂暴 / 高能狂暴”等同于 M3T1 / M3T2 两个文件。
本版前端有单独的 TurboSubModeSwitchCommand，发送 EXTREME/SILENT 指令；主档位及 ProfileIndex 是另一组输入。
旧记录中“两份模板相同”这个观察仍成立，但它不足以解释新版子模式，也不能据此判定二者没有差别。

分析原件及哈希记录在 official-modes-water-evidence.json；完整 OEM DLL/反编译内容仅保留本机 cache，未纳入 Git 或维护快照。

## 1. 自定义模式是一组可保存的调参配置

主模式枚举由前端 Fan/Status 消费：Office=0、Gaming（中文均衡）=1、Turbo=2、Customize=3。
切自定义时，前端发送 OPERATING_CUSTOM_MODE + ProfileIndex，并另外通知水冷管理层 IsCustomRun=true。
自定义配置可分别保存 CPU 功耗/温度目标、GPU 功率与频率参数、风扇曲线、风扇独立输出等，具体控件受能力位限制。
当前包有 5 份 Mode4_Profile 文件；前端允许选配置并重命名，而非“点白灯就得到一个统一性能数值”。

官方 CustomSettingPageViewModel 的温度控件把温度目标换算为 TjMax - 目标温度后发送 CpuTccOffset。
因此 UI 的“温度墙”与 EC 温度偏移是不同单位表达，不能直接把 UI 数字写入寄存器。

固件 DSDT 的 PMSC 明确优先判断 CUME（0x727 bit6）：为 1 时 PMDD=3；之后才看 TBME/UFME 分辨前三档。
这证明固件确实认识独立自定义状态，并非白色指示灯纯装饰。但完整调参需要多项设置、通知与回退逻辑。
0x706 / 0x726 等额外控制流程目前主要来自第三方研究，未以本机原厂运行轨迹验证。

## 2. 狂暴子模式、显卡超频、普通静音是不同设置

官方 HomePageViewModel.TurboSubModeSwitchCommand：

| UI 选择 | Fan/Control 消息 |
| --- | --- |
| 高能狂暴（OverClocking） | Action=SET_CPU_CORE_OFFSET_EXTREME，EXTREME=1 |
| 静音狂暴（SilentPerformance） | Action=SET_CPU_CORE_OFFSET_SILENT，SILENT=1 |

此处没有切主档位到其他模式，也没有直接选择 M3T1/M3T2。
服务中同时存在 UserSet_Cpu_Core_Offset_Extreme/Silent、WMIWriteSilentSubModeForBiosRAM、
WMIWriteExtremeSubModeForBiosRAM 等专用接口，但它们的真实实现仍被保护。
消息名含 CORE_OFFSET 不能单独证明只改电压；不能据此猜一项 EC 写入就是完整切换。

另一个独立 GpuOcOpenCommand/GpuOcCloseCommand 才负责显卡频率偏移，通过 SET_OPERATING_MODE_DETAIL
发送 GpuCoreClockOffsetOC。支持机型会分别读取 TurboOCAdjust43 与 TurboOCAdjust46；其他分支使用不同默认值。
这些默认值不是本机验证值，本次不应用。IsAC 也是单独状态；“高能狂暴”不是“插 AC 电源”的同义词。
普通 Silent/WhisperMode 与本处静音狂暴也不能混为一谈。

前端通过 IsTurboSubModeSupport 控制子模式是否出现，通过 SilentPerformanceModeSwitch 初始化所选状态，
并用独立的 IsTurboOCAdjustSupport / IsTurboOCDefaultON 等能力处理显卡超频。

本机固件另有 TB43=0x728 bit0；PMSF 会依此选择不同 APTC（温度偏移）分支。
结合命名及前端 43/46 两套参数，它很可能关联静音子策略，但**设置极性和全部切换动作尚未通过原厂实测确认**。
本次只读 TB43=0，不能因此宣称当前 Linux 紫档已经完整进入“高能狂暴”。

## 3. 水冷是联动层，不能概括为新增第五档

官方前端每次切主模式，同时发送两类消息：

| 主档位 | 普通性能控制 | 通知 LCHWOC/Control |
| --- | --- | --- |
| 办公 | OPERATING_OFFICE_MODE，ProfileIndex=0 | IsNormalRun=0 |
| 均衡 | OPERATING_GAMING_MODE，ProfileIndex=0 | IsNormalRun=1 |
| 狂暴 | OPERATING_TURBO_MODE，ProfileIndex=0 | IsNormalRun=2 |
| 自定义 | OPERATING_CUSTOM_MODE，所选 ProfileIndex | IsCustomRun=true |

所以界面仍围绕原来的四档运行，同时把当前档位交给水冷超性能管理层。
这张表确认的是前端到服务的路由，**不是水冷接入后每一档的最终功耗数值表**。
主模式按钮路径不意味着服务内部永远不会临时强制模式；连接/错误/回退处理需进一步追踪。

水冷控制分成两层：

1. **外置设备层 BT_LC**：蓝牙连接、泵、冷排风扇、灯光、流量状态、注水状态与设备固件版本。
   只有蓝牙状态为 Connected 的路径才按正常连接显示；还保留独立的 LC_MeterNormal 流量状态。
   只插水管、设备已供电、蓝牙已连、液体正常流动，不能自动当成同一个状态。
2. **主机联动层 LCHWOC**：包含 Support、Connected、Enable、NormalRun、CustomRun。
   官方服务有 SetStatus、SetEnableFlow、RamFan1P5SetMode、RefreshDefaultFanTable、GetEcFanTable、SetOCLevel 等方法。
   可确认它具备连接状态和风扇表/超性能协调机制，但不能从方法名断言具体执行顺序或全部断连保护行为。

“超性能开关”也与“连接水冷”分开：前端收到 LCHWOC 的 Enable 后，会隐藏自定义页的手动 cTGP/DB 控件；
关闭后恢复显示。这说明软件在不同控制路径之间避免同时调节同一组 GPU 参数，并非单纯把所有滑块加到最大。
对应前端的开关消息为 SET_OPERATING_MODE_DETAIL + OverClockingSwitch=1/0。

外置水冷泵与冷排风扇有自己的选项，不能与主机办公/均衡/狂暴名字强行一一对应：

- 泵：安静 / 均衡 / 最大；前端发送 LC_PumpCtrl + 索引。
- 冷排风扇：更安静 / 安静 / 均衡 / 最大，支持时另有自动；发送 LC_FanCtrl + 索引。
- 前端给风扇显示赋值 40/50/60/90；泵有 45/60/90 的暂时赋值，但状态回报又将正值转换为 100。
  因而不能拿这些前端显示值当泵的实际占空比、供电电压或转速，也不能声称它们就是本机三档水冷曲线。

## 4. 本机固件证据：水冷状态与等级共用一个字节

DSDT 的 EC 区域定义：

| 字段 | 位置 | 本次只读值 |
| --- | --- | --- |
| CUME | 0x727 bit6 | 0（未进入自定义） |
| TB43 | 0x728 bit0 | 0（仅记录，子档极性未实测） |
| LCSE | 0x7C7 bit1 | 0 |
| OCPL | 0x7C7 bits2–4 | 3 |
| LCOC | 0x7A4 bit3 | 0（不据此判定物理水冷能力不存在） |

用户确认未连接水冷时，0x7C7 整字节已经是 **0x0C**，并非 0。
所以它不是可任意整字节写 0/1 的纯布尔值。OpenRevo 的连接/断开路径有这样的整字节写法，
在本机直接照搬会同时改动 OCPL；必须先确认各位的真实预期用途与保持规则，不能当成已验证官方协议。
本次没有写入此寄存器。

DSDT.PMSF 在特定 CSTM/TB43/LCSE 分支中选择不同 APTC 值（例如水冷与风冷路径出现 2 与 10），
说明水冷标志可以参与主机温度策略。该分支还依赖其他软件/型号标志，不能宣称所有档位都会固定改变同样温度。
固件 _Q83/_Q84/_Q9C/_Q8A 等事件调用 PWUP(OCPL)，PWUP 向 NVIDIA 平台对象及独显发通知。
因此 Linux 完整复现还需核对 NVIDIA 驱动实际是否响应，不能只检查 EC 写入成功或指示灯颜色。

## 5. 仍缺少的实机证据与下一步

尚不能给出本机“风冷/水冷 × 办公/均衡/静音狂暴/高能狂暴/自定义”的最终 PL1、PL2、GPU TGP、
内置风扇曲线和外置泵/风扇转速数值。官方后台方法被保护；包里的静态通用模板不是本机激活后数据。

原装水冷尚未连接，故本次没有进行真实连接、流量、断连与策略回退测试。
下一步应按正常方式连接已准备好的原装水冷，先记录蓝牙/设备状态及 EC 只读变化；
若需要精确复现官方全部策略，需补原生 Windows 官方服务的活动配置、状态消息与切档前后硬件记录。
测试矩阵应区分：仅物理连接、蓝牙正常且有流量、超性能开关关闭/开启、各主档位、两种狂暴子模式、断连回退。
现有 Linux GUI 仍只承诺前三个固件基础档，未加入水冷自动调度或未验证的子档写入。

## 本机证据索引

- vendor/ui/HomePageViewModel.cs：ModeSwitchCommand、TurboSubModeSwitchCommand、GpuOcOpenCommand、RefreshItemSupport。
- vendor/ui/CustomSettingPageViewModel.cs：CustomModeProfileSetCommand、HWOCSwitchCommand、Service_DataChange。
- vendor/ui/LiquidCoolingSettingViewModel.cs：LcPumpModeSettingCommand、LcFanModeSettingCommand、Service_DataChange。
- vendor/GCUService.cs：LiquidCooling、LiquidHWOC、MainOption、ModeProfile、MyFanManager_RamFan1p5。
- vendor/DSDT.dsl：EC 字段定义（约 105080–105170 行）、PMSF、PMSC、PWUP。

上述 vendor 路径均相对于 ~/.cache/mechrevo-linux-research；这是当前官方本地包和本机固件证据。
辅助对照：https://github.com/faintonce/open-revo/blob/main/src-tauri/crates/openrevo-core/src/driver/ble_cooler.rs 。

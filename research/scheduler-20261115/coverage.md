# 官方性能调度与灯效生命周期：静态覆盖盘点

日期：2026-11-15。OEM 5.56.60.46。分析对象是本地已恢复的方法体，不执行 OEM 服务。未读写硬件、未改系统服务、未升级内核。这里的“官方行为”指所列方法的代码行为，不代表所有型号分支都适用于本机，也不代表请求已产生硬件效果。

证据路径：`../firmware-link-20261115/decoded-source/` 简称 F；本目录 `decoded-source/` 简称 S。原程序集 SHA-256 为 `9ee018d42684d02032da642d90376843383223d76d82a6fba6ce42e475a82093`。S 是 ILSpy 静态反编译后还原字符串/间接委托得到的阅读稿，不能当作原始可编译源码。

## 负载调度的精确规则

`F/sysmanager-restored.cs:1333 InitCPUGPUConbineONOFFTimers` 建立 1500 ms 定时器；`cpugpuDTRunTimer_Elapsed`（1878–2010）只在圆口在线且不重入时继续。三个计数器初值 0，区间 -1..3。

| 输入 | 计数更新 |
| --- | --- |
| CPU >95% | CPU 计数 +1，最多3 |
| CPU <10% | CPU 计数 -1，最少-1 |
| 10% ≤ CPU ≤95% | CPU 计数直接归0 |
| GPU >90% | 双负载 GPU 计数 +1，最多3 |
| GPU ≤90% | 双负载 GPU 计数 -1，最少-1 |
| GPU >50%，游戏策略受支持 | 游戏 GPU 计数 +1，最多3 |
| GPU ≤50%，游戏策略受支持 | 游戏 GPU 计数 -1，最少-1 |

双负载 CPU/GPU 计数均到3：先关闭已开启的 GAME；此前 combined 返回不是0xBB、且 single-GPU 返回不是0xBB，才请求 `WMICPUGPUCombineRunON`。任一计数到-1且 combined 返回不是0xAA，则请求 OFF。GAME 计数到3且 combined 返回不是0xBB，才开启 GAME；计数到-1且 GAME 已开或初始 launch marker=-1，关闭 GAME 并把 marker 置0。GAME 方法返回值不参与判断。

这不是“三次连续高负载”：从初始0需要3次，从稳定空闲-1需要4次；计数会逐步升降。CPU中间区间归零也不直接关闭已获0xBB的策略。若 combined ON 返回未知/失败，官方同一个回调可能先 GAME OFF、combined ON，再 GAME ON。拔掉圆口则整个回调返回，不清计数、不主动 OFF；不能把外层固件/电源事件的作用猜进这一方法。

`RestoreCPUGPUCombineParams`（2014–2038）仅在注册表 DefaultCPUGPUCombine=1 时清理软件计数/状态/两个返回码，**没有**重置 game launch marker，没有发 WMI，finally 写注册表0。模型保留这个细节。

新增 `iteglow/src/iteglow/oem_scheduler.py` 为纯计算模型；采样回调及明确返回码可注入，仅返回 operation 名，不提供硬件执行器。16项测试覆盖阈值边界、迟滞、成功/失败返回码、优先顺序、圆口冻结和采样失败。重入需调用者串行化；没有复制 OEM 缺少 finally 可能卡住重入标志的缺陷。模型现已接入专用内核lease桥、root daemon和GUI；真实自动阈值及恢复证据见[实机资格记录](../turbo-qualification-20261115/README.md)。上述16项是最初纯模型测试，不代表当前全部测试数量。

## 采样口径

CPU：`F/sysmanager-restored.cs:2446/2631` → `S/CPUInfo.cs:223` → `S/CpuUsageMonitor.cs:40`。NtQuerySystemInformation(8) 按 Environment.ProcessorCount 遍历并求和全部逻辑处理器 IdleTime/KernelTime/UserTime，按累计差分计算 `(Δkernel + Δuser - Δidle)/(Δkernel + Δuser)*100`，限制0..100，Math.Round 后取 int。首个样本、查询失败、非正总差分为0。它不是最大忙碌核、不是进程 CPU%；UI 的 GetCPUUsage 与调度函数复用同一个 monitor，故采样窗口可能被 UI 读取缩短。Linux 的 /proc/stat 等价程度需明确标注；尤其 iowait 口径不应未经说明固化为“完全等同 Windows”。

GPU：`S/GPUTypeClass.cs:169/583`，NVIDIA 路径 GetNVGpuCount 后依次 GetNVGpuUsage(index, ref percentage)，只保留最后一张卡结果，忽略每次 bool 返回；初值0。不是将 GPU、多引擎或进程利用率相加。非 NVIDIA 才走 GpuMonitor。

原生 `GPUInfoDLL.dll` 的 GetNVGpuUsage RVA0x7b60 → RVA0x21c0，NVAPI QueryInterface ID `0x60ded2ed`，结构版本 `0x10048`，成功后读取结构+12字节。与 NVIDIA 官方 [接口表](https://github.com/NVIDIA/nvapi/blob/main/nvapi_interface.h) 和 [结构定义](https://github.com/NVIDIA/nvapi/blob/main/nvapi.h) 对应 `NvAPI_GPU_GetDynamicPstatesInfoEx` / `utilization[0].percentage`，即GPU域过去1秒忙碌比例；不是framebuffer、video或bus域。两段原生反汇编保存在本目录。Linux NVML/nvidia-smi 采样不是相同 API，采样时间窗需实测记录，不能仅凭百分数宣称逐样本一致。

## 电源与保护字段

| 字段/来源 | 已确认语义 | 限制 |
| --- | --- | --- |
| `F/ec-restored.cs:325 GetRoundSocketStatusFromEC` | 0x7cc bit0=1即圆口在线；否则仅低4位全0时才回退0x490 bit0；低4位非零且bit0=0视为非圆口 | 不可用通用 AC online 替代，USB-C 可能也供电 |
| `S/PowerModeEvent.cs:68 RefreshACLineStatus` | Windows PowerLineStatus Offline→g_ACLineStatus=0、Online→1，Unknown不改原值；另读取TypeC但未参与该赋值 | 与圆口是两个独立判断来源 |
| 0x7a5 bit3 | GetFanSafetyProtectStatus；SafetyProtectionUpdate 接保护事件后切回对应出厂风扇表；custom使用Gaming表 | 本方法不公布触发温度，不编造一个固定阈值 |
| 0x7a5 bit2 | SetFanQuietModeEnable 使用 mask0xfb 修改 | 不等同狂暴 TB43 或 NVIDIA WhisperMode |
| 0x7a5 bit4 | SetOverBoostMode 使用 mask0xef 修改 | 未验证的模式，不开放写入 |
| 0x786 bit7 +低位 | SetCpuTccOffset(value,true) 写value OR0x80；禁用写0 | 温度阈值须结合TjMax和平台定义 |
| 0x787 | SetFanSwitchSpeed(value,true) 写(value/100) OR0x80（商>0）；禁用写0 | 原函数名为转速变化设置，不把它当RPM目标 |
| 0x783/784/785 | SetPL1/2/4分别直接写对应字节 | PL4另有0x727 bit7 double flag；当前只验证PL1/2同步25–75W |
| 0x751 bit6 | FanBoostEnable，与 mode位共同编码 | 单独全速请求尚未本轮实测 |

GetBitFromByte 的索引已用 `S/UtilityExtensions.cs:55` 确认是从0开始。`F/manager-restored.cs:3002 SafetyProtectionUpdate` 按档位选择DefaultFanTable_Office/Gaming/Turbo，保护解除后恢复保存的表。Linux当前guard仅处理寄存器/曲线与管理记录不一致；未实现这个EC保护事件→恢复出厂表的专门逻辑。

## 官方到当前 Linux 的覆盖矩阵

| 功能 | 官方入口/行为 | 当前覆盖及缺口 |
| --- | --- | --- |
| 办公/均衡/狂暴/自定义入口 | F/manager:7428 SetFanMode，0x751模式+FanBoost，自定义另CUME/白灯 | 四档已实测，GUI可用；不等于完整profile |
| 模式按键轮转 | F/manager:2897 ModeSwitchChanged：NV机 office→balanced→turbo→custom→office；无圆口且AP≥24时balanced→office；custom ID39/43跳过custom | 当前GUI手动切换及guard检测硬件切档；没有冒充服务参与按键轮转 |
| 档位与电源重新同步 | F/manager:2415 SwitchModePowerChange，读取EC档位；无圆口清GPU core OC；恢复对应submode/SmartBalance/OC、TGP并Init | 当前狂暴lease失电/PM等归还、醒后不自动选档；尚未复刻所有profile/SmartBalance/OC重载，不应简单改成拔电强制office |
| CPU/GPU双负载/游戏策略 | F/sysmanager:1878，每1.5秒，计数与返回码门控 | 自动CTAF阈值和真实BB/AA已实测；本机game支持返回AA，正式路径不启用GAMM，不能以兼容实验冒充官方支持 |
| 静音/高能狂暴 | F/manager UserSet_Cpu_Core_Offset_Silent/Extreme，TB43+WMI；GPU core offset与策略链路关联 | GUI与常驻调度已接入；CTAF实测静音CPU约45W/高能约75W、GPU均约140W；190/260W是预算，未测得260W实际输出，未做声压测量 |
| 自定义CPU功耗 | F/manager:7200/7589，custom写PL1/2/4，非custom置0；TCC/电压另控制 | 现有同步25–75W已实测；独立PL2/PL4、TCC、电压未开放 |
| 风扇曲线 | FanTable +F/manager:7140保护/表选择 | 已验证双风扇独立11占空比节点、固定出厂温度/迟滞、高温100%；未开放温度节点/零转速/全速boost |
| 风扇安全保护 | F/manager:3002、7797，EC保护bit触发表回退 | 未专门监听/展示；首先适合只读纳入遥测，禁止猜温度触发值 |
| 睡眠后性能恢复 | F/manager:2872 Resume→LoadProfileAll→SwitchModePowerChange→FanBoost | Linux主动在睡眠前撤销自定义，醒后不自动重用，是已记录的安全边界 |
| Modern Standby /PowerStatusChange | RamFan1p5类两override为空 | 不凭函数名补造行为；实际电源同步需追踪外部调用 |
| 键盘/灯带灯效与亮度 | S/RGBKeyboard与HIDKeyboard，效果/调色板/速度等 | 已有GUI和经用户验收自动灯效；不含全部官方音乐/逐键/Welcome功能 |
| 灯效保持 | S/HIDKeyboard:1569，按项目ID保存LastEffect、PowerSwitch，AC/DC亮度分别存；Resume重新跑效果并保持Off | 已安装iteglow-session-restore.service，服务启动真实恢复三个灯区；真实登录/睡眠唤醒仍未验收 |
| 开盖/合盖 | S/SystemLidEvent:158–189只记录LaptopLidStatus及日志 | 还需查消费者，不能推断合盖自动关全部灯 |
| 空闲关灯 | S/KeyboardCloseTimer:143/162/175，键鼠事件重启计时，超时关动画，下一输入恢复；HIDKeyboard:2802 | 自动灯光页及活动会话runner已实现，默认关闭；注入空闲刺激下真实键盘熄灭/活动恢复通过，不覆盖手动设置 |
| AC/DC灯光 | S/RGBKeyboard:1312→UnPlugged/Plugged；HIDKeyboard:2753选择独立亮度，四区音乐效果在DC停止/AC重启 | 三灯区独立AC/DC亮度已实现，默认关闭；通用AC与圆口分开。真实AC键盘亮度变化通过，物理AC/DC转换未测 |
| Logo | EC独立RGB/呼吸/渐变、亮度/关闭 | 已发布且用户逐项目视确认；保留共享性能/电池位，详见[Logo证据](../logo-20261115/README.md) |

## 仍未覆盖或需要补全验收的官方行为

证据状态：除特别标注外，本节基于当前源码已确认。OEM包包含多个机型分支；“在代码中存在”不等于本机具备该硬件或需要开放同名按钮。

| 范围 | 准确缺口与证据 | 当前处理 |
| --- | --- | --- |
| 自定义GPU | CTGP、DB开关/增量、TPP与应用存在上下文；F/manager:7345、F/GpuFeatures:408–473 | APPE单变量与CTGP65/115W、DB5/15/25W均实测通过，真实helper/GUI已部署；真实pkexec CPU35+GPU100/DB15联合负载、完整reset和旧CPU-only兼容最终复测通过。范围65..115W、DB关闭或5..25W，TPP255仅当官方协议值，不当255W滑块 |
| CPU高级调节 | 独立PL1/PL2/PL4、TCC、CPU电压；F/manager:7200/7589；GPU时钟/显存offset另经NVAPI | 现有仅同步25–75W；本机支持和效果未单独验收，不开放超频 |
| 完整风扇表 | OEM保存20份profile，含温度与DownT；App.FanTable_Init及F/manager:7140 | 现有双风扇11个占空比节点、出厂温度/迟滞和冷却下限；未提供自由温度、零转速、FanBoost全速、quiet/overboost或787变化速率调节 |
| EC风扇保护事件 | 7a5 bit3触发对应出厂表回退，解除后恢复；F/manager:3002/7797 | 无专门保护事件监听/界面；当前guard不等同OEM此事件逻辑，不能猜固定触发温度 |
| SmartBalance | 支持7ac bit0，启用bit1；F/ec:429、F/manager:3505–3547 | 尚未确认本机支持位和实际效果，不随切档猜写 |
| 多档案与配对 | 办公/均衡/狂暴各2份、自定义5份，SetPairedProfileIndex；F/manager:3183及LoadDefaultProfile | 现有保存一套手动设置，没有全量应用/游戏绑定和档案库 |
| 模式键/OSD | F/manager:2897包含受AP版本、圆口及custom ID影响的第四档轮转 | 现有硬件前三档轮转、GUI第四档已实测；未接管OEM主机事件或完整OSD |
| 电源/睡眠恢复 | App.ModernOn先APPE1再Resume，F/manager:2872重新加载档案 | Linux刻意先撤销再恢复未选档服务；不跨boot/睡眠自动重施性能。新lease真实睡眠/物理失电尚待验收 |
| 原装水冷 | 连接、流量、泵/冷排风扇、灯光、断连恢复与主机HWOC策略 | 用户设备当前离线运行，蓝牙联动未实现；[官方水冷研究](../official-modes-and-watercooling.md)不能当Linux已支持 |
| 噪声档名 | 官方静音43/46等分支是请求与命名 | 温控/功耗已测，未用声级计证明噪声指标 |
| 逐键灯光 | HIDKeyboard映射effect51至用户矩阵，ITE控制层有UserModeBy12H | 当前统一颜色/调色板，无逐键布局编辑器；需核对本机矩阵和物理键位 |
| 主机生成灯效 | ITE控制层Set_Lighting_Effect的音乐34、字母/堆叠/霓虹/星空等AP模式与固件版本条件 | 现有已验收灯效不代表OEM枚举32种都能下发。音乐与主机动画未实现，不能只增加下拉框选项 |
| Welcome/无应用灯效 | S/HIDKeyboard:1514 AsyncWelcome；ITE Set_Welcome_TimeOut_Effect_Enable:3815及相邻Welcome设置，读取/写入UEFI RGBKeyboard数组与HID | 会话恢复只处理登录后最后成功设置，不负责固件欢迎/预登录持久灯效；未执行UEFI写入 |
| 电池Logo/共享位 | 7c5 bit6与RGB呼吸颜色位独立，官方Get/SetBatteryLogo | 已保留此位，不凭名称把它当后盖Logo总开关；物理含义尚未目视验收 |
| 亮度快捷键同步 | S/HIDKeyboard SetBrightnessByScanCode及WMI/热键通知 | 固件直接快捷键行为与GUI持久化同步未完整验证；不能承诺键帽调亮度后自动保存 |
| lid/S5及额外灯区 | S/SystemLidEvent只有记录状态；ITE存在hinge/base/sync多机型分支 | 未找到可用于本机的完整消费者规则，不臆造合盖/S5策略，不凭枚举新增不存在的灯区 |

ITE控制层静态文件原缓存为 `~/.cache/mechrevo-linux-research/logo-20261115/LM_ITE_RGB.cs`，Set_Lighting_Effect起于3441；effect34走音乐DLL、51走用户矩阵、部分AP动画依设备/版本分支，不能用一个通用effect枚举证明本机支持。待持久保存的范围以当前已反编译证据为准，缓存不是安装依赖。

### APUseFlag / ApExistFlag 与EC APPE的边界

真实UEFI `OemMagicVariable` payload180、APVersionCheck=25。`ApUseFlag`在payload0x61、`ApExistFlag`在0x5d，经NvramVariable→WriteUefi→SetFirmwareEnvironmentVariableW保存；扫描5,765个恢复方法后未找到这两个setter直接转发EC的通路。固件/SMM内部是否传播未知，不能把“未找到用户态直接路径”说成固件绝不关联。

EC APPE则是0x741 bit0，App启动准备在FanTable_Init之后无条件置1，然后才启动MyFan.EnableByService/LoadProfileAll/syncBiosSettings/Init。ModernOn重新置1；Application_Exit禁用组件后清0。读取默认风扇表只是其中临时清0再置1的分支，**不是唯一APPE初始化来源**。真实APPE单变量试验已证明本机自定义GPU请求依赖这个运行上下文。完整源文件、交叉引用及UEFI/EC区分见[自定义GPU报告第8节](../custom-gpu-20261115/official-custom-gpu.md)。未为此试探UEFI写入。

### 灯光策略验收边界

新policy默认off、三灯区手动恢复与独立配置保存已接入；事务锁已实际部署，watcher启动恢复三灯区通过。主代理记录 `lighting-policy-hardware.json` 显示真实键盘亮度50→20→0→20→50，灯带/Logo手动关闭区没有写入，state.json/logo.json哈希不变；实际电源保持AC，idle为合成刺激。真实登录、休眠唤醒和物理AC/DC切换不能用这次试验替代，见[自动灯光策略](../../docs/lighting-policy.md)。

历史说明：本报告最初盘点时尚无实际恢复服务、负载模型未接入执行器；这些判断已由本页当前矩阵及实机资格记录替代。现用服务名为iteglow-session-restore.service，早期建议的iteglow-restore.service没有安装。活动桌面身份必须动态查询，不固定某次登录会话编号。

# 2026-11-14：自定义调校与狂暴策略链路

仅适用于 MECHREVO YAOSHI Series-XxAF55xx，BIOS N.1.33MRO91，未连接水冷。前篇 custom-and-turbo-validation.md 是此前阶段记录，本篇取代其中“功耗和曲线尚未开放”的状态。

## 实际生效的自定义控制

官方 RAMFan1p5 的 APPE（0x741 bit0）是重要前置条件。进入白色自定义档后设置 APPE，APL1/APL2（0x783/784）均设25、45、25 W。12线程 Torch CPU矩阵负载下，RAPL energy_uj 差分实测稳态分别约25、45、25 W，CPU最高62°C（不计初始桌面负载）。Linux RAPL constraint 文件仍显示210 W；这不是实际限功耗失效。没有写MSR、PL4、电压、超频或GPU功耗。

追加25→75→25 W同负载验证：75 W阶段稳定74.94–75.01 W，最高78°C，未触发85°C停止阈值；结束后所有原寄存器与RAM完整恢复。记录见cpu75-power-trial.json。发布范围据此扩大到25–75 W。

发布范围为25–75 W，PL1/PL2同步；不是该CPU性能上限，也不是完整的原厂可调范围。扩大上限、独立短时功率和窗口时间需要另外实测，不根据出厂字节猜测开放。

### 出厂曲线与独立风扇

本机 EC 0x78e=FC，支持 RAMFan1p5。恢复官方当前版本 MyFanTableCtrl/FanTable_Manager1p5 的静态代码后确认查询握手：F5F写档位（办公3、均衡2、狂暴1），F5D写253，F5E写201，等待两个应答字节变化，读取F00–F5F 96字节。查询期间不启用手动曲线；查询结束恢复原RAM、APPE和TB43。办公/均衡/狂暴及静音标志下得到相同出厂曲线。

CPU升温阈值：48/52/56/60/64/68/72/76/80/85°C，回落阈值0/46/50/54/58/62/66/70/74/78/82°C。
GPU升温阈值：46/50/54/58/62/66/70/74/78/81°C，回落阈值0/44/48/52/56/60/64/68/72/76/80°C。
风扇原始值为百分比×2，出厂有效节点0/30/35/40/50/55/60/70/80/90/100%，剩余占空比固定100%，无效温度255。

自定义提交：RAM控制关闭（7C6 bit2=0）、独立双风扇（7C5 bit7=1）、写96字节并核对、开启7C6 bit2。40%→70%→40%下CPU转速实际升降。正式助手另进行CPU40/GPU70→CPU70/GPU40→CPU40/GPU70的独立响应验证，记录见 installed-tuning-trial.json。固定出厂温度和迟滞，仅编辑11个占空比节点；最低40%且不能低于对应出厂值，必须单调，高温100%。

**遥测更正**：旧OEM GetEcGpuFanRpm 拼接46C/46B，但本机46B实际跟随CPU PL2（210→85→25/45），故此前旧测试中的gpu_rpm不可靠，不可用于GPU风扇验收。46C/46D组合随GPU风扇独立指令反向升降而CPU计数跟随CPU指令，作为实测候选计数保存；GUI不显示未经硬件文档确认的GPU RPM。

## 静音／高能为何此前无效果

不仅是子模式位未传播。恢复官方 MySystemManager.cpugpuDTRunTimer_Elapsed 后发现：
- 每1500ms检查负载，且要求圆口适配器在线。
- CPU>95%与GPU>90%累计到3次后，调用CPU/GPU双负载策略（CTAF，CSTM=1）。
- 游戏路径GPU>50%累计到3次后启用GAMM（GATM=1）；退出阈值有计数迟滞，双负载优先。
- 本机Linux启动后CSTM/GATM都为0，原先只改TB43不会进入DSDT PMSF的有效子策略分支。

临时精确机型内核桥接调用本机已有CTAF/GAMM/PMSF，不安装模块。游戏策略下GAMS置位，APTC/APTN变为高能0x8A、静音0x92（10/18°C offset，enable=1）。但**APPE=0时CPU真实 MSR 0x1A2仍为0x07690000**，温控阈值105−7=98°C，没有变化。

**APPE=1后**，高能/静音/高能读取到 MSR 0x0A690000 / 0x12690000 / 0x0A690000，即实际CPU温控阈值95/87/95°C。这里只读MSR，所有修改通过已确认EC/ACPI协议完成。恢复APPE、策略和EC偏移后，MSR回到0x07690000。原始试验 tcc-trial.json。温度含义参见 Linux turbostat 对MSR_TEMPERATURE_TARGET的解码：
https://github.com/torvalds/linux/blob/master/tools/power/x86/turbostat/turbostat.c

这验证的是CPU温度政策传递，并非已经测得整机43dB噪声，也不是GPU的完整动态功率策略。GPU单负载约63W，启用游戏分支后仍无明显子模式功率差异。测试没有强行把CPU加热到87/95°C来触发节流。

NVIDIA595.99.02官方文档要求nvidia-powerd支持Dynamic Boost：
https://download.nvidia.com/XFree86/Linux-x86_64/595.99.02/README/dynamicboost.html
本机驱动报告Notebook Dynamic Boost supported且已安装powerd，但未配置服务。临时启动先发现DBus权限问题，补齐当前协议的DBus规则后仍失败于“NvPCF static configuration”。探测结束删除临时DBus规则，未启用失效服务。不可把这一失败直接当作所有GPU限制的唯一根因。

结论：找到了缺失的后台策略和APPE接管条件，CPU温控已有实质验证。完整狂暴子模式按钮仍不开放；固定温度上限不冒充完整的官方动态策略。GPU Dynamic Boost、噪声、双负载全过程及其他固件版本仍待验证。

## 发布与恢复约定

- 精确DMI/BIOS/iomem匹配；不接受任意地址、文件路径或调压参数。
- root独占锁，写前记录/run/mechrevo-tuning.json；包含原值、目标值和phase，原子保存0600。断电重启不自动重新应用。
- 曲线写入期间关闭RAM控制，随后验证全部目标；失败按所属掩码恢复，保留无关位及Logo。
- guard每秒检查有管理记录的设置；硬件模式键改档或写入中断时撤销所属覆盖，保留按键选择的新档位。第三方功耗/曲线改写与预期均不符时停止写入并保留恢复记录，不盲目覆盖。
- 点击其他档位先撤销自定义。显式撤销返回应用前档位（若用户已通过模式键改档则保留该档）。关闭GUI保留已应用设置。
- 系统睡眠前及守护服务停止时撤销；唤醒不自动重新应用。支持测试休眠前清理入口，不为了验证而真的休眠用户桌面。
- GUI保存编辑值供下次使用，但启动不写硬件。CPU/GPU曲线百分比不是准确RPM。

维护备份包含助手、两个systemd单元、GUI、补丁、测试与本研究；不包含原始OEM程序、恢复镜像、原始ACPI表。临时模块源码仅供研究，不加入启动或DKMS。

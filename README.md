# MECHREVO 本机控制实验

临时本地 Git 仓库，用于 YAOSHI Series-XxAF55xx / BIOS N.1.33MRO91 的灯光和性能模式研究。

基线为已验证的 iteglow 0.3.2 本机适配版。仅经过实机验证的性能控制进入安装版本。没有设置远程仓库。

## 已完成

2026-11-14：原生 GTK GUI 增加“性能档位”页，办公（绿）、均衡（蓝）、狂暴（紫）可立即切换。
软件只修改本机按键实测的 EC 0x751，值为 0xA0 / 0x20 / 0x30。
用户确认软件测试中三色依次正确变化；第二轮测试和 GUI 实际调用记录见 research/。
该控制对应固件模式键，不声称复现 Windows 控制中心的所有功耗、驱动或超频策略。
启动、退出、重启、唤醒均不自动套用档位；机身按键仍然有效，性能页可见时每 3 秒回读。

GUI 以普通用户运行。system/mechrevo-profile-helper 安装为 root 所有、0755，使用隔离模式系统 Python，
只允许 status 和 set office|balanced|turbo，没有地址/数值/文件参数。
polkit 仅允许本机活动登录用户 cybernova 调用该固定助手；不向 GUI 开放 /dev/mem。
机型、BIOS、内存资源不匹配，或检测到自定义、功耗覆盖、风扇加速，都会拒绝切档。
进程间锁、二次状态检查、写后回读及条件回退避免并发覆盖。出现外部按键改变时不强行恢复。

## 自定义与狂暴子模式：仍未验证，未启用

静态分析确定了更多协议线索，但本次没有对未知自定义/功率/风扇寄存器进行试写：

- OpenRevo 包含精确板型 YAOSHI Series-XxAF55xx；其自定义流程涉及 0x726 bit7、0x727 bit6 白灯标志，
  0x706 自定义使能、RAM 风扇表、CPU PL1/PL2/PL4、GPU cTGP/Dynamic Boost 等。
  仅设置白灯无法证明完整的自定义策略生效。其他参数也不能仅凭板型列表直接用于本机。
- 当前官方 ControlCenterX 5.56.60.46 语言文件称两个子模式为“静音狂暴”和“高能狂暴”。
  后者资源键 strEnableOverClocking，不能将用户说的“AC狂暴”直接解释为 AC/DC 供电开关。
- 官方 Mode3_Profile1/2 均 Activated=false；M3T1/M3T2 风扇表除了 Name 完全相同。
  两份配置还含 175W GPU 目标等通用值，无法作为本机 RTX 5070 Ti Laptop 的实测配置。
- Windows 服务的相关 SetUserProfile / SetCustomModetoEC 方法被保护，现有反编译结果没有可用方法体。
  MainOption/ModeProfile 的 TurboSilentGPUOCOffset、OverClockingEnabled 仅证明软件存在不同路径，不能推导写入序列。
- 同类 Linux 项目的 AMD 型号资料说明 RAMFan1p5 狂暴子档使用风扇表，但不能替代本机 Intel 平台验证。
  普通 Silent 快捷键的支持位 0x766 bit0 在本机为 0；它不等同“静音狂暴”，因此未借用该开关。

下一步需要本机原生 Windows 控制中心实际识别硬件后生成的活动配置与切换前后 EC/功耗/风扇记录，
或具有本板型实机验证的数据源。现有通用 KVM/Proton 环境不能代替这一步。
拿到数据后应先做可恢复单项验证、温度与风扇检查，再解除 GUI 限制。

协议参考（仅事实研究，未复制 GPL 项目源码进入 MIT GUI）：

- https://github.com/faintonce/open-revo ：ec_mode.rs、wmi_ec.rs、builtin_models.json。
- https://github.com/jerrita/mw15xa-ctrl/blob/main/docs/operating-modes-policy.md ：不同 AMD 机型，仅参考。
- 官方本地包 ~/Downloads/ControlCenterX_5.56.60.46_Mechrevo.zip ：UserPofiles、UserFanTables 和语言资源。

## 验证与维护

`/usr/bin/python3 -m unittest discover -s tests -v`：5 项测试覆盖白名单、拒绝异常状态、外部按键竞争、回退与非目标寄存器不变。
research/gui-smoke.json 记录已安装 GUI 模块通过 pkexec 实际依次切换三档并恢复紫色。
research/basic-mode-probe-repeat.jsonl 对应用户亲眼确认的自动切档测试。

永久源码位于 ~/.local/share/desktop-maintenance/sources/iteglow-0.3.2 和 sources/mechrevo-profile-1.0。
维护备份包含 GUI、补丁、助手、polkit 文件、桌面入口与本次验证资料。
临时仓库可删除，已安装功能不依赖此目录。

iteglow 更新：先执行 desktop-maintenance backup 更新前；在新版源码上检查
patches/iteglow-0.3.2-mechrevo.patch，合并 GUI/性能页与精确设备匹配；运行测试后再替换安装源。
此适配没有交给 pip/apt 自动覆盖。不得将旧文件整份覆盖到不兼容新版。
BIOS 更新后助手会拒绝切档；重新核实资源地址与硬件键值后才可更新 BIOS 白名单，不能直接删掉校验。

恢复系统组件时，从维护备份还原以下固定路径，助手 root:root 0755、polkit 文件 root:root 0644：

- /usr/local/libexec/mechrevo-profile-helper
- /usr/share/polkit-1/actions/org.cybernova.mechrevo.profile.policy
- /etc/polkit-1/rules.d/49-mechrevo-profile.rules

恢复后用 `pkexec --disable-internal-agent /usr/local/libexec/mechrevo-profile-helper status` 只读验证。
撤销适配时可删除上述三个文件，并恢复改动前 GUI 源码及桌面入口；无需卸载内核驱动或修改 BIOS。

## 中文界面与灯效修复（后续更新）

已修复 spark / aurora / reactive 的自动播放：此前误把波浪方向当作按键触发标志。
用户已确认三者正常，GUI 分别显示“烟花 / 极光 / 随机闪烁”，并提供独立触发方式。
界面已改为浅色侧栏、设备示意、中文灯效卡片、独立参数区与性能卡片。
详细证据、协议来源及维护要求见 [灯效与界面记录](research/lighting-fix.md)。
全部 9 项测试通过；GUI 实际按钮回读、较小窗口布局也已检查。

# MECHREVO 本机控制实验

临时本地 Git 仓库，无远程。适用于 YAOSHI Series-XxAF55xx / BIOS N.1.33MRO91。

## 已安装

- 中文 GTK 控制中心：键盘、灯带、性能模式。
- 修复随机闪烁、极光、烟花的自动播放；用户已确认正常。
- 办公（绿）、均衡（蓝）、狂暴（紫），以及自定义模式（白）的进入与退出。
- 自定义 CPU 功耗 25–75 W（PL1/PL2同步），CPU/GPU 独立风扇曲线与一键撤销；不提供超频。
- 启动、退出、开机或唤醒不自动套用模式；可见性能页每3秒回读。

GUI以普通用户运行；固定root助手通过polkit提供固定档位和经过范围检查的调校，不能指定地址或任意寄存器值。精确机型、BIOS、资源地址及状态白名单限制访问，部分写入失败会尝试恢复并如实报告。机身按键的外部变化优先。

## 当前研究结果

自定义功耗已用 CPU 持续负载实测，双风扇已验证独立响应。狂暴子模式已定位缺失的官方负载策略和 APPE 接管条件：补齐后 CPU 真实温控阈值可在高能95°C、静音87°C之间切换。GPU功率和整机噪声尚未完成验收，因此完整子模式按钮仍未开放。

[最新功耗、风扇与狂暴策略验证](research/power-fan-validation.md)为当前事实；早期研究记录保留为历史，不代表最新发布状态。

- [灯光修复与界面](research/lighting-fix.md)
- [官方模式和水冷逻辑](research/official-modes-and-watercooling.md)：历史静态研究；现已接入离线水冷，固件联动未启用。
- [目标方法哈希与验证摘要](research/custom-turbo-evidence.json)
- research/experimental-turbo：未发布的临时研究模块，不是运行依赖，不得自动安装。

用户补充了可发光Logo；尚未发布独立Logo控制。切档不写入共享Logo字节或灯光HID接口。

## 测试与维护

`/usr/bin/python3 -m unittest discover -s tests -v`：31项测试。实际GUI与pkexec链路另行验证。

永久GUI源码：`~/.local/share/desktop-maintenance/sources/iteglow-0.3.2`。
永久助手、测试及研究资料：`~/.local/share/desktop-maintenance/sources/mechrevo-profile-1.0`。
维护备份包含这些源码、补丁、助手、polkit、桌面入口和灯光设置；删除此临时仓库不会影响运行。

更新前执行 `desktop-maintenance backup 更新前`。更新iteglow时在新版上检查
`patches/iteglow-0.3.2-mechrevo.patch`，解决冲突并跑测试后再替换永久源，不能直接覆盖或让pip抹掉适配。
BIOS更新后需重新验证，不能移除机型/BIOS校验。新增轻量恢复守护服务，无常驻内核驱动。

恢复系统文件：助手 `/usr/local/libexec/mechrevo-profile-helper` 为root:root0755；
`/usr/share/polkit-1/actions/org.cybernova.mechrevo.profile.policy` 和
`/etc/polkit-1/rules.d/49-mechrevo-profile.rules` 为root:root0644。
恢复后用 `pkexec --disable-internal-agent /usr/local/libexec/mechrevo-profile-helper status` 只读核对。

新增系统单元 mechrevo-profile-guard.service 与 mechrevo-profile-sleep.service 均安装到 /etc/systemd/system，root:root0644。恢复时先 reset 撤销调校，再安装助手与单元，执行 systemctl daemon-reload、systemctl enable --now mechrevo-profile-guard.service、systemctl enable mechrevo-profile-sleep.service。睡眠前清理、机身按键竞争及守护重启均已实测；唤醒不重施调校。运行时 /run/mechrevo-tuning.json 是本次开机的恢复记录，不备份或跨开机恢复。

## 2026-11-15 GPU联动研究进度

正常重启已恢复 ACPI；powerd 两轮开关实测使 GPU 从约65W恢复到约140W，现已开机启用并加入睡眠前停用、唤醒健康检查。静音档虽改变固件平台预算，实际GPU仍约140W；临时修正CPU拓扑虽消除AutoFL报错，仍未解决预算落实，且该试验适配已撤下。完整子模式GUI继续不开放。

见 [最新实测、服务恢复与后续路线](research/powerd-coldboot-20261115/README.md)；[之前的ACPI失配诊断](research/powerd-acpi-resume-20261115.md)保留为历史。真实shutdown休眠仍未验收。

内核保持6.8系列；用户明确禁止未经同意跨大版本升级。已撤销本次未启动的7.0 HWE安装。安全更新按具体CVE与Ubuntu包版本核对，见[安全核对](research/powerd-coldboot-20261115/kernel-security.md)。

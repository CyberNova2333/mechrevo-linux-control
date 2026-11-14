# MECHREVO 本机控制实验

临时本地 Git 仓库，无远程。适用于 YAOSHI Series-XxAF55xx / BIOS N.1.33MRO91。

## 已安装

- 中文 GTK 控制中心：键盘、灯带、性能模式。
- 修复随机闪烁、极光、烟花的自动播放；用户已确认正常。
- 办公（绿）、均衡（蓝）、狂暴（紫），以及自定义基础档（白）的进入与退出。
- 白色基础档只提供模式入口。自定义功耗、超频、温度目标、风扇曲线尚未开放。
- 启动、退出、开机或唤醒不自动套用模式；可见性能页每3秒回读。

GUI以普通用户运行；固定root助手通过polkit提供status与set office|balanced|turbo|custom，不能指定地址或任意值。精确机型、BIOS、资源地址及状态白名单限制访问，部分写入失败会尝试恢复并如实报告。机身按键的外部变化优先。

## 当前研究结果

官方服务的受保护方法已恢复，静音/高能狂暴命令和自定义入口均有本版本源码依据。狂暴子模式的寄存器与ACPI调用已测试，但约63W GPU负载没有可辨识的策略差异，NVIDIA还会记录不支持的固件通知，因此未开放子模式切换。普通模式也能复现该通知日志；不应把它描述成新增GPU崩溃，亦不能当作GPU功耗联动已经实现。

[最新自定义与狂暴验证](research/custom-and-turbo-validation.md)为当前事实；早期研究记录保留为历史，不代表最新发布状态。

- [灯光修复与界面](research/lighting-fix.md)
- [官方模式和水冷逻辑](research/official-modes-and-watercooling.md)：水冷未连接，继续暂缓。
- [目标方法哈希与验证摘要](research/custom-turbo-evidence.json)
- research/experimental-turbo：未发布的临时研究模块，不是运行依赖，不得自动安装。

用户补充了可发光Logo；尚未发布独立Logo控制。切档不写入共享Logo字节或灯光HID接口。

## 测试与维护

`/usr/bin/python3 -m unittest discover -s tests -v`：14项测试。实际GUI与pkexec链路另行验证。

永久GUI源码：`~/.local/share/desktop-maintenance/sources/iteglow-0.3.2`。
永久助手、测试及研究资料：`~/.local/share/desktop-maintenance/sources/mechrevo-profile-1.0`。
维护备份包含这些源码、补丁、助手、polkit、桌面入口和灯光设置；删除此临时仓库不会影响运行。

更新前执行 `desktop-maintenance backup 更新前`。更新iteglow时在新版上检查
`patches/iteglow-0.3.2-mechrevo.patch`，解决冲突并跑测试后再替换永久源，不能直接覆盖或让pip抹掉适配。
BIOS更新后需重新验证，不能移除机型/BIOS校验。当前没有新常驻服务或内核驱动。

恢复系统文件：助手 `/usr/local/libexec/mechrevo-profile-helper` 为root:root0755；
`/usr/share/polkit-1/actions/org.cybernova.mechrevo.profile.policy` 和
`/etc/polkit-1/rules.d/49-mechrevo-profile.rules` 为root:root0644。
恢复后用 `pkexec --disable-internal-agent /usr/local/libexec/mechrevo-profile-helper status` 只读核对。

# 正常重启后的 GPU 实测与剩余问题

证据状态：除单独标明的推断和待验证项外，以下来自 2026-11-15 本机实测。原始采样见同目录 JSON。用户原装水冷已通电并离线运行；蓝牙连接后立即断开，固件 LCSE 仍为 0，没有伪造水冷联动状态。

## 已恢复的能力

正常重启后 DSDT 与 26 张 SSDT 校验全部通过，NPCF status=15。powerd 持续运行，原先的 NvPCF static configuration 错误消失。没有替换 NVIDIA 二进制，也没有热修改 ACPI 表。

使用现有 PyTorch CUDA 环境，6144×6144 FP16 矩阵乘法、阻塞 CUDA Event 同步，避免 CPU 忙等。两轮相同负载的结果：

| 阶段 | 实际 GPU 平均功耗 | enforced limit | 平均核心频率 |
| --- | --- | --- | --- |
| powerd 关闭 1 | 64.96 W | 65 W | 1229.67 MHz |
| powerd 开启 1 | 139.74 W | 140 W | 2217.17 MHz |
| powerd 关闭 2 | 65.01 W | 65 W | 1198.50 MHz |
| powerd 开启 2 | 139.72 W | 140 W | 2215.83 MHz |

每阶段 6 个样本，间隔约 2 秒，GPU 利用率约 99%，最高 55°C。结果证明本机 AC Dynamic Boost 实际有效，不只是显示上限变化。短时采样不是长时间稳定性或全部混合负载验收。

## 静音档的预算已经变化，但尚未落实到实际 GPU 功耗

临时研究模块调用已有官方 GAMM/PMSF，启用 APPE，依次高能→静音→高能。每阶段 8 个样本，间隔约 2 秒。

| 字段 | 高能 | 静音 |
| --- | --- | --- |
| TB43 | 0 | 1 |
| APTC/APTN | 0x8a | 0x92 |
| EC ETPP | 145 | 75 |
| NPCF ATPP | 1160（145×8） | 600（75×8） |
| GPU 实际功耗 | 约 140 W | 约 140 W |

所以可以确认固件预算传到了 ACPI NPCF 对象，不能据此声称驱动已执行预算。ATPP 是平台预算字段，不应直接当成独立 GPU TGP。CPU 95/87°C 温控阈值此前已有 MSR 实测；本次没有测定整机噪声目标。完整子模式按钮继续不开放。

测试后 APPE、TB43、0x786 与原值一致，GAMM/CTAF 已恢复，临时模块已卸载，未安装常驻研究驱动。

## CPU 拓扑异常与 AutoFL

当前 6.8.0-149 内核日志报告 `Max logical packages: 24`。所有 CPU 的 physical_package_id=0，但 package_cpus_list/core_siblings 每个只包含自身，lscpu 因而显示 24 sockets、每 socket 一个 core。`/sys/devices/cpu_core/cpus` 与 cpu_atom/cpus 不存在。

powerd 日志持续报告 `Failed to create AutoFL core: AutoFL: >1 CPU package`。限定文件访问的 strace 确认其子进程读取各 CPU 的 topology/core_siblings。**强关联推断**：异常拓扑阻止了 AutoFL 初始化，可能解释平台预算无法落实；仍需在修正拓扑后做同负载对照才能确认因果。此前“重复 RAPL 接口可能是原因”的猜测没有验证，不作为结论。

另有 `disable Dynamic Boost DC controller`，属于固件禁用电池控制器的日志；不能把已验证的交流供电 Dynamic Boost 一并判定为失败。停止 powerd 仍可能记录短时间上限错误，进程正常退出；未隐藏日志。

曾安装 Ubuntu HWE 7.0.0-43 并完成 NVIDIA DKMS 构建，**未启动过**。用户明确要求不得擅自升级内核大版本后，已卸载本次新增的全部 9 个 HWE 包，恢复 6.8 GRUB 默认入口和 DKMS 状态。不要沿用旧聊天里的“重启进入 7.0”计划。下一步只研究现有 6.8 系列的适配路径；切换内核系列必须先获用户同意，不得以安全修复为由自行跨版本。

## 已安装的服务保护

- powerd 经两轮真实负载验证后已开机启用，ExecCondition 检查 ACPI 完整性。
- mechrevo-powerd-sleep.service 在睡眠前记录 powerd 是否运行并停止；唤醒后只恢复此前运行的服务。恢复前再次检查并保存 ACPI，损坏时保持停止。恢复任务排在 nvidia-resume.service 之后。
- 已测试脚本、服务启动/停止恢复事务以及损坏 ACPI 的模拟拒绝路径。**未实际执行本次休眠或验证 shutdown 休眠绕行**，不能宣称解决了固件 S4 问题。
- 开机基线单元使用 RemainAfterExit，避免每次重启 powerd 重复抓取；日志输出摘要，私有原始诊断仅保留最近 8 份。原始 AML 不进入维护备份。
- 31 项测试通过；systemd 单元检查通过，既有 nginx 单元字段警告与本次无关。

源码与单元在仓库 system/；安装至 /usr/local/libexec 与 /etc/systemd/system。恢复维护备份后执行 daemon-reload，再启用 nvidia-powerd、mechrevo-acpi-baseline 和 mechrevo-powerd-sleep；启动 powerd 仍经过 ACPI 检查。不恢复 /run 中的临时标记。

## 安全核对与后续路线

保持运行 Ubuntu 6.8.0-149.149-generic，APT 刷新后同系列最新候选仍为 6.8.0-149.149。已核对 /proc/version_signature、包完整性、Canonical CVE 状态及发行包 changelog。见 [内核安全核对](kernel-security.md)。不锁死 6.8 安全更新，不安装其他系列，也没有禁用 user namespaces、容器或 VPN 功能。

后续先研究 6.8 拓扑异常的兼容修复，再验证 quiet/extreme 的实际功耗与撤销路径，通过后接入 GUI。水冷仍维持用户离线运行状态；蓝牙控制和真实休眠测试各自尚未完成。

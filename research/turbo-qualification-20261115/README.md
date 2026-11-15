# 狂暴调度实机验收与范围

证据状态：除特别标注外，本页依据2026-11-15主任务采集的真实设备JSON和当前源码。原始文件来自 `~/.cache/mechrevo-linux-research/control-replica-20261115/`，归档仅复制和提取记录，没有重新执行负载或写硬件。

机型为MECHREVO YAOSHI Series-XxAF55xx，BIOS N.1.33MRO91，内核保持6.8.0-149-generic。被验收模块SHA-256为 `803853479e5f28ce8cf2420f2dc05f7b07483c15809e3664bc961d1b4fa250d0`。本轮不升级内核。

## 已取得的实机证据

| 项目 | 观察结果 | 原始记录 |
| --- | --- | --- |
| OEM支持查询 | combined=0x55，game=0xAA；默认不调用GAMM | [lease-qualification.json](lease-qualification.json) |
| 真实工作负载ACK | CTAF ON=0xBB、OFF=0xAA，CSTM随之变化 | 同上 |
| 归还 | release、最后owner FD关闭、8秒超时后inactive、无pending，APPE/TB43拥有位和APTC恢复 | 同上，15条阶段记录 |
| 自动阈值 | CPU与GPU计数达到3后自动CTAF ON，CPU负载结束后OFF；GATM始终0 | [daemon-auto-load.json](daemon-auto-load.json)，26条采样加最终恢复 |
| daemon被终止 | 12条读样均为owner_closed、inactive、无pending | [daemon-kill-recovery.json](daemon-kill-recovery.json) |
| 转普通档/自定义 | quiet→balanced及quiet→自定义25W/双风扇调校均先释放turbo；reset后回普通狂暴 | [profile-turbo-integration.json](profile-turbo-integration.json) |
| 生命周期排练 | active quiet→pre后模块和socket消失→post经prepare启动；最终inactive、无pending | [lifecycle-rehearsal.json](lifecycle-rehearsal.json)，`actual_sleep=false` |
| powerd停止 | 无负载quiet活动中停止powerd，2条观察样本内释放；重启powerd后不重新选择，拥有字段与Logo共享位归还 | [powerd-loss-rehearsal.json](powerd-loss-rehearsal.json) |

自动负载记录中，第3个观察样本见到CSTM=1及模型真实回复0xBB；CPU结束后的6条样本均CSTM=0、回复0xAA。稳定GPU约140W，最高观察温度CPU86°C/GPU60°C。这份自动验收没有记录CPU package功率，CPU45/75W、GPU均140W的物理对照来自另外的[官方CTAF两轮试验](../powerd-dispatch-20261115/official-combined/README.md)。190/260W是联合预算，不能写成GPU直接输出或已测得260W总功率。

[summary.json](summary.json)是机器可读摘要，[daemon-events.json](daemon-events.json)提取阈值计数、回复和状态。归还比较按驱动拥有的掩码计算：0x741 bit0、0x728 bit0、0x786全字节及CSTM/GATM；不把整个EC快照相等当成条件。0x741的其他位和派生0x7f7可由固件自行变化。记录中可见的Logo共享位0x7ab高3位和0x7c5低3位保持原样，独立Logo颜色/灯效验收见[灯光记录](../logo-20261115/README.md)。

## 生命周期复核

当前主服务和sleep单元均已安装、enabled；只读查询确认 `sleep.target.Requires=mechrevo-turbo-sleep.service`。`RequiredBy`需要enable建立`.requires`关系才生效；本机suspend/hibernate服务本身`Requires=sleep.target`且`After=sleep.target`，配合turbo-sleep的`Before=sleep.target`，清理失败会阻止实际睡眠入口。[systemd依赖说明](https://www.freedesktop.org/software/systemd/man/latest/systemd.unit.html)

pre先请求suspend释放，再stop daemon关闭FD，最后卸载模块并确认消失；不能卸载时保留恢复记录并失败。turbo-sleep在profile-sleep和powerd-sleep之前启动，两者停止次序相反，所以powerd的post先排队恢复，turbo的post随后运行；主服务`After=nvidia-powerd.service`且`ExecCondition`要求powerd active。profile-sleep只有pre reset，不会在恢复时重新套用自定义参数。[systemd顺序说明](https://www.freedesktop.org/software/systemd/man/latest/systemd.unit.html)

启动时prepare在modprobe前检查精确身份、验收文件哈希和新ACPI健康，加载后核对srcversion。失败不会运行daemon；`WantedBy=multi-user.target`不会把桌面启动变成硬依赖。主服务60秒内最多尝试3次，防止永久门禁失败持续刷日志。powerd不运行时ExecCondition正常跳过，不主动启动它。睡眠记录异常、旧boot/身份变化或哈希变化均禁止自动恢复；恢复成功也不会重新选择quiet/extreme。[ExecCondition说明](https://www.freedesktop.org/software/systemd/man/latest/systemd.service.html)

复核没有发现当前范围内的阻断问题。固定命令、不使用shell、root私有socket/marker、NOFOLLOW及现有profile flock提供访问和互斥边界；EC不具备跨控制器版本号，仍不支持与另一性能写入器并行运行。`systemd-analyze verify`对新增单元未报错，输出仅有本机既有nginx未知键和bt_ddns权限问题。

## 尚未由这批记录证明

- 本机真实suspend/hibernate以及恢复后的固件行为。当前只有pre/post排练，内核PM notifier是离线模拟验证。
- 物理拔插圆口AC、电池供电下转换，以及新lease活动时按机身模式键。相关保护已有离线测试，但不能以普通helper切档替代物理试验。
- powerd停止时同时施加高负载的情形；本轮真实停止/恢复试验在无负载状态进行。
- 内核崩溃、断电后的恢复，EC不响应时的实际硬件故障注入。
- 本批turbo记录不含自定义GPU cTGP/DB验收。后续独立APPE/DB对照已成功，真实helper/GUI已部署，CPU35W+GPU100W/DB15W联合负载末10样本平均34.981/115.026W，完整恢复和旧CPU-only兼容最终复测通过。详见[当前自定义GPU报告](../custom-gpu-20261115/official-custom-gpu.md)，不要把后续证据归到本批记录。

源码与运行契约见[后端说明](../../system/mechrevo-turbo-backend.md)和[驱动说明](../../system/mechrevo-turbo-driver/README.md)。本目录各证据文件的哈希记录在`SHA256SUMS`。

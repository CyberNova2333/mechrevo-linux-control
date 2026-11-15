# 精确机型狂暴策略桥候选

`mechrevo_turbo_lease` 是本机的生产候选，不是通用笔记本驱动。构建本目录不会安装、加载或写硬件。它只接受 MECHREVO / YAOSHI Series-XxAF55xx / BIOS N.1.33MRO91、当前已验证的 `6.8.0-149-generic` 内核、`INOU0000` 的 `0xfe410000–0xfe410fff` ACPI 资源和对应 OEM WMI GUID。当前内核安全更新条件由已有主机维护流程判断；此目录不会更换或升级内核。

## 构建与当前边界

```sh
make CC=x86_64-linux-gnu-gcc-13 W=1
python3 test_core.py
```

驱动构建子任务只运行离线测试；主代理随后已部署当前哈希并完成真实WMI、归还、自动阈值调度与pre/post排练，详见[实机资格记录](../../research/turbo-qualification-20261115/README.md)。`build-manifest.json`保留构建时状态，不表示此后仍未部署。编译成功本身不代表验收，真实PM和物理拔电仍未测试。

`test_core.py` 从当前 C 文件提取实际租约、所有权、回滚、watchdog 和 PM notifier 函数，用模拟固件执行，并启用 AddressSanitizer/UndefinedBehaviorSanitizer。它不打开设备、不加载模块、不执行真实 ACPI。当前测试覆盖：

- 全部 256 种圆口状态的主/备用判定，排除 USB-C 供电误判。
- 支持码门禁、其他 FD 无法控制 lease、normal release、共享字节保留。
- 136 个 release I/O 点、79 个 acquire I/O 点、72 个活动 WMI I/O 点，分别在接受前/接受后注入故障，全部恢复模拟初始状态。
- 模式键改档、GPU 自定义 bit2、外部 APTC/工作负载接管、超时、AC 消失、PM prepare 和恢复后不自动重新接管。
- 恢复瞬时错误重试、持续失败的有界重试和 module pin。

这里的 PM 测试是实际 C 函数的离线 notifier 测试，**没有让本机睡眠/休眠**。AC 测试只修改模拟寄存器，**没有物理拔插电源**。后续真实WMI框架与release/FD close/8秒超时结果已单独归档，不能把这些结果延伸为PM或物理拔电已验证。

## ABI 1

字符设备 `/dev/mechrevo-turbo`，权限 `0600`。每次 open/read/write 都要求真实初始命名空间的 root 与 `CAP_SYS_RAWIO`；用户态 GUI 不能直接写设备。只有通过 `acquire` 的那一个 open-file-description 是 owner；复制这个 FD 的进程共同持有它，最后一个副本关闭时执行归还。`O_CLOEXEC` 应由 daemon 设置。

命令是完整匹配的 ASCII 字符串，允许一个末尾换行，不接受地址、路径、任意参数或嵌入 NUL：

| 命令 | 行为 |
| --- | --- |
| `acquire quiet` / `acquire extreme` | 先检查支持、普通狂暴/圆口供电/无其他策略所有者，再捕获基线、设置 APPE/TB43 和 PMSF |
| `variant quiet` / `variant extreme` | 活动 owner 切换 TB43 并调用 PMSF |
| `combined 0` / `combined 1` | 原始 OEM WMI combined 通知，验证真实 ACK 与 CSTM 回读 |
| `game 0` / `game 1` | 仅支持查询明确允许时发送 GAMM 通知，验证 ACK 与 GATM |
| `heartbeat` | 检查仍满足安全条件后，将租约延长 8 秒 |
| `release` | 归还仍属于本 lease 的状态；无活动 owner 的 root FD 可请求重试待恢复状态 |
| `status json` / `status text` | 仅修改本 FD 的读取格式；默认 JSON |

用户态每 1 秒发 heartbeat，独立的工作负载采样可按 OEM 1.5 秒周期运行。内核每 250 ms 检查租约/模式/电源/所有权。用户态应在每次命令事务中与现有 helper 共用 `/run/mechrevo-profile.lock`；内核 mutex 只序列化本模块，不能替代这个跨进程锁。

每条有效 write 尝试使 `sequence` 增加，失败通过负 errno 返回，同时保存 `last_rc`；成功返回原写入长度。write 后用 `pread(fd,4096,0)` 获取完整 JSON。每个 FD 的部分读取复用该次快照；重新从偏移 0 读取会生成新快照。不要依赖 read 刷新租约。

稳定状态字段包括：

```text
abi, active, variant, sequence, last_ack, return_present, return_type,
last_rc, acpi_status, stop_reason, lease_remaining_ms,
recovery_needed, restore_rc, restore_skipped, recovery_attempts,
recovery_pin, policy_uncertain, read_rc,
combined_supported, game_supported, combined_support_ack, game_support_ack,
cstm, gatm, standard_turbo, round_adapter, regs, calls
```

`regs` 使用无 `0x` 的小写十六进制键（如 `"728"`），值是整数。状态读取失败时 `read_rc` 非零，`cstm/gatm=null`，用户态必须停止动作；不能把默认 false 当成有效硬件读取。`calls` 保留 EC read/write、PMSF、CTAF/GAMM 最近的完整 ACPI status、返回类型与整数。`last_ack` 是最近命令的真实 WMI 整数，没有返回时为 `null`；心跳不会伪造 ACK。

## OEM 证据与支持查询

来源一：`research/firmware-link-20261115/decoded-source/WMIEC.cs` 的 `GetSetULong` 方法及 `ec-restored.cs:GetRoundSocketStatusFromEC`。

来源二：本机 `DSDT.dsl` 中 `\_SB.AMW0._WDG/WMBC/OEMG`、`\_SB.INOU.CTAF/GAMM` 与 `\_SB.PC00.LPCB.EC0.PMSF`。

`_WDG` 将 GUID `ABBC0F6F-8EA1-11D1-00A0-C90629100000` 映射到方法对象 `BC`。使用 instance 0、method ID 4，8 字节小端 `Data`；`WMBC` 将输入放入 AC00 后执行 OEMG。

| Data | 官方作用 | 返回 |
| --- | --- | --- |
| `0x50000000003` | combined 支持查询 | `0x55` 支持 |
| `0x50000000013` | game 支持查询 | `0x55` 支持，`0xaa` 不支持 |
| `0x50000000001` / `...02` | CTAF off/on | `0xaa` / `0xbb` |
| `0x50000000014` / `...15` | GAMM on/off | `0xbb` / `0xaa` |

注意 `0x50000000016` 是 LCD Overdrive 支持查询，不能当成 game 支持。DSDT 的 game 条件还依赖 GFID/MOID；手动 GAMM 能产生效果，不代表官方默认调度允许它。本候选不硬编码 `game_supported=true`。

直接调用 CTAF/GAMM/PMSF 是 void，ACPI 成功本身没有 `0xbb/0xaa` ACK。候选通过真正的 WMI 包装取得 ACK，不按期望值生成。未知 ACK、类型或回读不符会失败并归还 lease，不继续执行未验证的失败 ACK 兼容分支。

圆口供电复刻官方逻辑：`0x7cc bit0` 为 1 则有效；否则仅当 `0x7cc & 0x0f == 0` 才读取 `0x490 bit0` 作为备用。`0x7e8` 不是本功能的判定来源。

## 所有权与归还

只直接写 `0x741 bit0`（APPE）、`0x728 bit0`（TB43）及尚属于本策略的 `0x786`（APTC/APTN）。不写模式字节、功耗限制、自定义控制位、风扇曲线、Logo 或水冷控制。共享位始终重新读取后合并。CSTM/GATM 通过固定 OEM 方法维护。

归还的触发包括主动 release、最后 owner FD 关闭、8 秒超时、失去圆口供电、外部模式/策略变化、PM prepare，以及模块退出时的最后检查。模块不会在 PM resume 自动重新接管。

归还前冻结所有权判断，不能先运行 PMSF 再把外部 APTC 当成自己的新目标：

- 普通模式键改变档位：保留模式字节，不直接恢复 APTC。仍属于本 lease 的 APPE/TB43 与工作负载变量可归还；OEM PMSF 可按新模式重新计算派生参数。
- 自定义或同档外部策略接管：停止全部 PMSF/WMI 调用，放弃旧策略所有权，`restore_skipped` 标明；不能清除自定义也在使用的 APPE。
- I/O 失败：保留待恢复所有权，每秒重试，最多 8 次。恢复未完成时自行 pin 模块，阻止正常卸载，也拒绝新 acquire。显式 release 可开始新一轮重试。
- 某次自身 PMSF/WMI 生成 APTC 后连续两次无法读取：`policy_uncertain=true`，保留 pin 和错误，不把未知结果猜成已恢复或外部接管。该异常需要进一步诊断，不能称恢复成功。

`restore_skipped` bits 0/1/2/3/4 分别对应 APPE、TB43、APTC、CSTM、GATM。它也可能表示某次未被设备接受的写入已经保持原值。用户态只有 `active=false` 且 `recovery_needed=false` 时才可清除恢复日志；保留最终 status 作为审计结果。

所有权判断依赖当前值与本模块记录的值；EC 没有跨控制者的原子版本号。因此不支持与其他性能写入器并行运行。该限制不能由内核 mutex 或读回测试消除。

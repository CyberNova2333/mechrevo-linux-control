# 狂暴调度后端候选

本目录包含 Python 标准库守护进程、root 控制客户端、驱动生命周期入口和离线测试；源码本身不安装或启用服务，实机部署与验收状态见仓库主 README。驱动源在 `mechrevo-turbo-driver/`，现有 profile helper 集成由主任务维护。

## 职责与互斥

内核 misc-device lease 是唯一硬件写入者，负责严格位掩码、普通狂暴/圆口AC前置检查、真实WMI ACK、每条命令回读和失败恢复。FD close、8秒heartbeat超时、模式键/AC变化、PM prepare 都由内核归还；用户态不重复写EC恢复，不使用 `/dev/mem`。

守护进程每1秒续租，每1.5秒采样全CPU聚合利用率与本机唯一 NVIDIA GPU 0 的利用率，并复用 `oem_scheduler.py` 的计数器/阈值/顺序。每次实际ACK后重算尚未执行的操作，保留先 GAME_OFF 再 COMBINED_ON 的顺序，不把请求值伪装成固件回复。NVIDIA query 有0.8秒超时；采样失败、状态失败、发现自定义journal均归还并停止此策略。活动lease每次tick在heartbeat前用1秒超时的systemctl查询powerd存活；停止或探针失败立即归还，之后不会因powerd恢复而自动重获。无负载quiet活动中实际停止/重启powerd已验证这一行为，2条观察样本内释放且无pending。

联合负载/游戏负载支持以bridge实际WMI查询为准：combined support使用命令0x50000000003，game support使用0x50000000013。只有combined_supported=true才允许接管；game_supported=false时纯模型不采样游戏计数、不发GAME_ON/OFF。手动GAMM方法能执行不等于OEM报告支持。运行中支持标志改变时归还，不遗留先前游戏标志。

所有操作占用原 `/run/mechrevo-profile.lock`。锁忙时本次跳过，不强抢或暗中续租；超过8秒由内核归还。单独 `/run/mechrevo-turbo-daemon.lock` 贯穿守护进程生命周期。持有单实例锁后，只有 root持有/0600/确实连接被拒绝 的旧socket可被移除；活socket、普通文件、符号链接和检查期间改变的inode均拒绝。

现有 profile guard 没有调校journal时是空操作。GUI改变基础档位或自定义调校之前，应由现有root profile helper先 `release` 并确认 `active=false`，释放后继续原有事务。不要持有profile_lock再调用turbo release，避免嵌套锁；调用方可在release返回后重新取得profilelock再处理原操作。设备检查仍防止在二者间重新抢入不符合条件的档位。

开机不自动选择子模式。模式键、失电、睡眠、崩溃或停止导致释放后，不自动重新获取。`resume` 仅解除用户态睡眠门禁，用户必须再次显式选择。

守护进程启动及每次select（包括同一lease内改变子模式）都执行只读 `mechrevo-acpi-check --summary` 并要求JSON healthy=true，同时检查nvidia-powerd为active。休眠后重新选择不能绕过这个门禁；失败时已有lease也会归还。该检查不在每1.5秒采样中重复哈希ACPI。

## 模块加载与睡眠

本机曾在休眠恢复后出现ACPI表指针损坏，因此仅在select前检查并不足够：内核bridge持有的旧ACPI handle必须在睡眠前消失。`mechrevo-turbo-lifecycle`只开放`prepare|pre|post`，不提供档位或寄存器操作。

- 主服务 `ExecCondition` 要求powerd已active，不替用户启动powerd。`ExecStartPre ... prepare`先检查精确DMI/内核、root验收清单和磁盘驱动哈希，再检查当前ACPI健康，最后才modprobe并核验loaded srcversion；不使用modules-load盲加载。
- 睡眠`pre`记录原服务是否运行和当前验收哈希，请求suspend归还，停止daemon使FD关闭，卸载bridge并确认模块已消失。即使daemon原本不运行，也清理残留模块。任何无法停止/卸载的情况都失败，sleep unit使用`RequiredBy=sleep.target`使该错误阻止睡眠。必须同时启用此unit；主服务的`Also=`已声明这一关系。
- `pre`在profile-sleep和powerd-sleep之前执行，逆序停止时powerd-sleep的`post`先排队恢复powerd。turbo的`post`重新检查ACPI、启动身份和原驱动哈希，只为睡前原本运行的daemon排队启动；主服务的`After=nvidia-powerd.service`等待其已排队的恢复，若powerd仍停止则ExecCondition跳过。新daemon始终inactive，用户需重新选择子模式。
- `/run/mechrevo-turbo-sleep.json`是root:0600的原子恢复记录。重复pre保留最初运行意图；睡眠中拒绝启动，ACPI失败或身份/哈希变化保留失败记录且不加载模块。单独lifecycle锁串行化这些阶段，prepare最多等3秒允许post排队交接，不重试已执行的生命周期操作。

生命周期离线测试覆盖这些顺序与失败分支；另已完成真实active quiet→pre卸载→post加载的排练，最终服务恢复但策略inactive，尚未执行真实休眠。两unit已enabled并确认sleep.target的Requires关系。内核驱动的PM回调仍作为额外恢复保护；健康检查失败时保持驱动/daemon停止，需正常重启排除损坏表指针后再检查。主服务60秒最多启动3次，prepare永久失败不会持续重试或阻塞桌面启动。

## 固定接口

helper始终输出JSON：成功 `{"ok":true,"result":...}`；失败 `{"ok":false,"error":"..."}` 并退出1。

| 命令 | 行为 |
| --- | --- |
| `mechrevo-turbo-helper doctor` | 读取身份与文件存在性，不打开设备或检查固件 |
| `... status` | 返回 `result.bridge.active/variant/...` 和纯模型状态 |
| `... select quiet` | 仅普通狂暴且圆口AC时获取/改变静音lease |
| `... select extreme` | 同上，高能lease |
| `... release` | 显式释放，并确认内核状态已inactive |
| `... suspend` | 先设置睡眠门禁再释放 |
| `... resume` | 解除门禁，不主动获取lease |

UDS `/run/mechrevo-turbo.sock` 为root:0600，另核验 SO_PEERCRED uid0；每个连接一行JSON，最大2048字节。请求为 `{"command":"select","variant":"quiet"}`，其余请求仅 `{"command":"status"}` 等。没有任意EC/ACPI/命令执行接口。

内核命令：`acquire quiet|extreme`、`variant quiet|extreme`、`combined 0|1`、`game 0|1`、`heartbeat`、`release`。每次写入后 `pread(...,0)` 检查ABI1、递增sequence、last_rc=0和回读。真实WMI返回保留u64，纯模型按OEM取低byte；缺少返回不制造0xBB/0xAA。固件读取错误立即失败。

当前bridge对不匹配ACK直接返回errno并归还，live路径随即停止；离线测试保留纯模型“失败ACK后重算游戏回退”的顺序验证，不表示在未知固件返回后继续写硬件。active=false但recovery_needed=true同样视为未完成恢复，保留journal并拒绝新接管。

## 审计与资格门禁

事务journal `/run/mechrevo-turbo.json` 使用0600原子替换和fsync，第一次硬件写入前记录；含初始状态、最近操作、实际回复和纯调度状态。归还失败保留日志；正常退出和内核已释放后的重新确认才清除。它不是能重放任意EC写入的恢复脚本。

live启动必须同时满足：root；DMI三项完全匹配；内核恰为6.8.0-149-generic；root:0600验收清单；GPU联合预算、lease恢复、实际OEM ACK三个项目都为true；磁盘驱动SHA匹配；已加载模块srcversion与验收文件相符；代码与模型文件root持有且他人不可写。示例清单故意不可用，不代表实机验收成功。

候选部署布局（主任务review及实机验收后再安装）：`/usr/local/libexec/` 下放 helper、daemon、lifecycle三个可执行入口，以及 `mechrevo_turbo_backend.py`、`mechrevo_turbo_lifecycle.py` 和现有纯模型的同版本 `oem_scheduler.py`。所有文件root持有且他人不可写，固定加载路径不读取用户环境。驱动需先经审查构建/验收并安装到当前内核目录，主服务仅受控加载该验收哈希。配置位于 `/etc/mechrevo-turbo-validation.json`。

源码可直接用 `/usr/bin/python3 system/mechrevo-turbo-helper doctor` 检查；在源码布局下仅回退读取本仓库的纯模型，live门禁仍要求代码root持有。离线测试命令：`/usr/bin/python3 -m unittest discover -s tests -p 'test_turbo*.py' -v`。

显式GAMM兼容实验观察到190/260W预算和130/140W GPU差异；本机实际OEM game support=0xAA，默认路径禁止GAMM。正式CTAF对照中静音约45W CPU+140W GPU、高能约75W CPU+140W GPU，190/260W仍是联合预算而非GPU直接功耗值。新bridge实机combined support=0x55、ON/OFF ACK=0xBB/0xAA，release/FD close/8秒超时均已回读确认拥有位恢复。常驻自动阈值ON/OFF、daemon终止释放、普通档/自定义转场及PM pre/post排练也已验证，证据与未验证范围见[实机资格记录](../research/turbo-qualification-20261115/README.md)；真实睡眠、物理失电仍不能用离线测试代替。

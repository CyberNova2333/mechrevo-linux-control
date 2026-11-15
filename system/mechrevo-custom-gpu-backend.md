# 自定义GPU调校与恢复记录

本页描述已部署助手接口，生产入口的真实pkexec联合负载与完整恢复已通过验收。`apply JSON`原有CPU/风扇四个字段保持兼容；可选`gpu_ctgp`为65–115整数W，只有同时提供它才允许可选`gpu_db`，取值为0或5–25整数W。缺少`gpu_db`表示关闭，缺少`gpu_ctgp`完全不接管或写GPU寄存器。不存在自由TPP参数。

GPU入口需要精确机型/BIOS/资源、当前内核6.8.0-149-generic、GFID低5位=25，以及固定GUID的真实UEFI变量payload180字节、offset0x43版本25；0x7e8不用于版本判断。显式GPU apply还要求powerd active和ACPI healthy=true，检查有超时且在撤销现有调校之前完成，status不重复哈希ACPI。需要圆口供电，活动GPU调校失电会由现有profile guard撤销。status返回`gpu_tuning_supported`、`gpu_tuning_range=[65,115]`、`gpu_db_range=[5,25]`和失败原因；GPU能力检查失败不阻止旧CPU/风扇status读取。

## 已验证映射

| EC位置 | 拥有范围 | 取值 |
| --- | --- | --- |
| 0x743 | 低3位 | bit0功能使能，bit2 cTGP使能，bit1 DB使能 |
| 0x744 | 全字节 | cTGP−65 |
| 0x745 | 全字节 | DB开时TPP固定255，关时0 |
| 0x746 | 全字节 | DB瓦数，关时0 |

APPE bit0置1以后才写目标；GPU写序为0x744→0x745→0x746→0x743，使能位最后生效。退出先撤销GPU使能/目标，再还原CPU与风扇，最后归还APPE与模式。0x741、0x7c5等共享字节每次持久化后重新读取，只合并拥有位，保留Logo及其他非拥有位。

即使不选择GPU，CPU/风扇调校也要使用共享APPE。因此所有新apply都要求GPU请求字节处于干净状态；CPU-only事务仅把GPU作为只读上下文守卫，不把它纳入journal地址或写入。若另一控制器后来写入GPU请求，则拒绝擅自撤掉其依赖的APPE，保留日志并提示冲突。

## journal v2

`/run/mechrevo-tuning.json`每个拥有地址记录before/target/confirmed，另有最多一个pending写。每次硬件写入前原子保存pending并fsync，随后重新检查整组所有权、读取最新共享位；只允许当前拥有位仍为confirmed时写入。读回成功后才保存confirmed并清pending。pending是崩溃恢复依据，不是把另一控制器提前写入相同目标当作自己的授权。

恢复每一步均重新检查，包括较长风扇RAM扫描后的模式/APPE/GPU再检查。历史before不持续放进可接受集合。中途I/O或日志保存错误保留confirmed/pending，下次guard能判断写入未发生或已接受后失败。模式键变化只放弃模式组，仍归还可证明属于本助手的设置；GPU/CPU/风扇/APPE出现外来值时停止全部覆盖并保留恢复记录。EC没有跨控制器原子版本号，最后一次读与写之间仍不能实现硬件级原子互斥，禁止并行运行其他性能写入器。

前台操作遇到profile锁忙时每20毫秒重试，最多等待1.5秒；到期明确报告忙，不打开EC、不强抢锁。后台reconcile保持非阻塞并立即让路。turbo释放仍在profile锁之前执行，失败后的只读fallback不会隐藏active/pending，未释放时禁止转场。

v1只读状态不改写、不清理。恢复时才迁移：active仅接受已保存target；preparing因旧格式没有pending，允许一次before/target确认并标记`legacy_ambiguous=true`，之后只使用新confirmed。任意第三值均拒绝猜测。旧active记录若已经被旧版部分恢复，无法区分外来写回baseline，仍保留记录要求检查，不自动清空或接管GPU。

睡眠清理由`mechrevo-profile-sleep.service`执行reset。它改用`RequiredBy=sleep.target`及现有`Before=sleep.target`，部署时需reenable以去掉旧wants并建立requires；未完成归还时阻止睡眠，避免携带未知调校状态继续恢复。主turbo-sleep仍先于profile-sleep执行；本改动不更改powerd-sleep或内核。

## 验证

`tests/test_gpu_tuning.py`包含固定OEM字节/顺序断言、DB范围、CPU-only不写GPU、v1迁移、外来接管、pending保存前后错误、读回失败以及每个实际apply/restore写入在接受前后的故障注入。原CPU/风扇与基础档位测试继续执行。实机APPE/DB实验见[官方自定义GPU记录](../research/custom-gpu-20261115/official-custom-gpu.md)；本助手后续生产验收原始记录见[custom-production.json](../research/custom-gpu-20261115/hardware-validation/custom-production.json)：CPU35W+GPU100W/DB15W均值34.981/115.026W，reset归还全部拥有位/风扇RAM/Logo位，旧CPU-only配置不写GPU且最终journal消失。真实睡眠及物理拔电仍不在该记录范围内。

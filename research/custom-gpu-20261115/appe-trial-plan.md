# APPE 对照候选

`ctgp-appe-trial.py`是独立研究脚本，默认只输出计划；本文件编写时未执行硬件试验。它不改动原`ctgp-trial.py`，不安装进助手或GUI。

无APPE的第一轮请求未观察到功耗变化，因此下一轮保持自定义GPU请求、DB关闭和负载一致，只比较APPE：APPE=0时115→100W，随后目标保持100W只将APPE bit0设1，最后APPE=1时100→115W。每阶段8个样本，独立guardian从GPU启动前起计60秒绝对deadline，超过4秒心跳间隔或父进程退出均停止负载并恢复。

入口读取固定GUID的真实UEFI `OemMagicVariable`，检查efivarfs的4字节属性头后payload恰180字节、payload offset0x43的`APVersionCheck=25`；0x7e8不作版本判断。仅允许当前内核6.8.0-149-generic、精确DMI/BIOS、普通狂暴、圆口供电、健康ACPI、powerd active，且没有其他turbo设备/socket/调校journal。

唯一新增写掩码是0x741 bit0。CPU自定义0x783–0x787必须全0，风扇使能0x7c5 bit7、0x7c6 bit2必须关闭，始终不写CPU、风扇、Logo、PMSF、WMI或UEFI。每次写前和每个样本均检查当前所有权、圆口及CPU/风扇门禁。

恢复顺序为撤销GPU使能并还原目标→归还APPE→归还模式；每步重新核对拥有值。guardian保留每个字段的last-confirmed值及唯一pending写，覆盖父进程死在提交前后、部分恢复失败重试。其他控制器改变GPU/APPE或CPU/风扇控制时停止覆盖，保留其上下文并报告不完整恢复；模式键改变则保留模式，归还仍属于本脚本的GPU/APPE。EC没有原子所有权版本号，不能辨别另一控制器写入同一个当前值，故不支持并行性能控制。

24项离线测试已通过，包括全部恢复I/O位置在接受前后的故障注入、pending APPE恢复、共享位保留、外来接管和真实UEFI数据格式门禁。scheduler子代理已独立静态复核无新增阻断。离线命令：`python3 -m unittest discover -s tests -p test_ctgp_appe_trial.py -v`。这些结果不是功耗生效或实机恢复已经验证的证明。

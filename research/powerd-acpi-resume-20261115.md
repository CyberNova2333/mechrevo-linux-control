# 2026-11-15：powerd 的配置失败与休眠后 ACPI 地址失配

历史记录：正常重启及 GPU 对照已完成，服务状态和后续路线以[最新实测](powerd-coldboot-20261115/README.md)为准。下文“待重启/disabled”描述的是重启前状态。

## 已确认事实

当前内核6.8.0-149/NVIDIA595.99.02，boot起于2026-11-13 22:02，随后经历两次休眠恢复。未更换驱动、未写ACPI内存、未热替换AML。

1. nvidia-powerd补齐D-Bus权限后仍报`Error in getting the NvPCF static configuration`。
2. REA/Ghidra只读分析原始/usr/bin/nvidia-powerd（SHA256 db2643c4c37360e183230ea7f9c5da3e28c39ff79d08eba6ddb0f4f057d7bc74）：报错字符串0x5ce090由0x41afe2引用，所属函数FUN_00524af0的热区调用FUN_00429080，命令0x13b，操作4，缓冲0x374字节；返回非零即抛出此报错。这条路径是驱动配置查询失败，不是已证明缺少XML文件。反编译器的函数边界和不返回调用推断仅作为静态证据。
3. 内核同期报`\_SB.NPCF._DSM.SNSR AE_NOT_FOUND`。只读sysfs设备status又触发`\_SB.NPCF._STA.XSXG AE_NOT_FOUND`。
4. sysfs SSDT18–26九张表都失配；SSDT19实际是DBGP，SSDT20是DMAR，SSDT24是BGRT，SSDT26是TPM2（仅76字节，而原SSDT26是16553字节）。部分其他表指向AML中段，校验和错误。不是单纯缺少Linux设备绑定。
5. 从EFI ACPI20指针0x613ba014，只读/dev/mem解析当前XSDT0x613b9728，严格限定ACPI Tables/NVS区域并校验长度和校验和。当前XSDT列出的有效表全部校验正确。
6. 原始boot日志的Opt2Tabl位于0x612c5000；当前XSDT把同名表放在0x612c6000，即上移一页。PtidDevc起连续九张SSDT同样上移0x1000；早先表未变。
7. 正确Opt2Tabl的SHA256为8bfb33b85deb2c68b9d21d8d3a90ebf8156191008b5dc7f90635f0ebdfac19df。反编译其NPCF._STA只判断CDIS并返回0x0D/0x0F，根本不引用XSXG；NPCF._DSM只检查UUID并调用NPCF方法，也不引用SNSR。故运行中的旧方法指针在执行错误位置的AML。
8. 正确固件支持NVPCF rev0x0200，GET_SUPPORTED返回14字节，静态表版本0x25；当前驱动随包头文件也定义最高0x25。版本本身尚无“不支持”的证据。

结论：当前会话的ACPI表映射已经失效，足以造成NPCF查询失败。先前约63W的GPU负载试验是在这个已损坏的固件接口状态下进行的，不能据此断言Linux驱动永远不支持模式联动。自定义CPU功耗与风扇的直接EC实测仍有效；完整GPU联动必须在重新建立正确ACPI命名空间后重测。

已找到同类上游问题218853：平台S4恢复时固件移动表地址，但恢复的内核保留旧指针；该报告shutdown休眠路径不受影响。本机因果链有独立的物理地址与表校验支持，shutdown模式的修复效果仍须本机再做真实休眠恢复验证。
https://www.mail-archive.com/acpi-bugzilla@lists.sourceforge.net/msg51341.html

## 已落地的准备与待验证修复

- 补齐/etc/dbus-1/system.d/nvidia-dbus.conf：仅root可拥有nvidia.powerd.server，默认上下文可发送消息。未授权普通用户冒充服务。
- 安装/etc/systemd/system/nvidia-powerd.service，沿用发行包服务主体，加ACPI健康检查ExecCondition。**仍disabled**；没有开机自动启用未经验证的GPU策略。
- 当前试启动被ExecCondition挡住，MainPID=0、Result=exec-condition，证明不会在错误AML上反复启动powerd。
- 新增/usr/local/libexec/mechrevo-acpi-check，只读DSDT/SSDT签名、声明长度、校验和，并确认NVDA0820定义存在；不读MSDM授权表，不执行ACPI方法，不写固件。
- mechrevo-acpi-baseline.service已enable；下次正常启动保存私有基线至/var/lib/mechrevo-acpi-check，目录0700。每次capture有独立boot_id/时间戳，含原始AML与JSON。原始表不进入维护备份；源码和摘要进入。
- /etc/systemd/sleep.conf.d/70-mechrevo-hibernate-shutdown.conf设置HibernateMode=shutdown。仍保存/恢复内存快照；最后断电走普通shutdown路径，不走平台S4。可能需要按电源键唤醒，而非依赖S4专属的开盖唤醒。这是待验证的绕行措施，不声称已修复固件。
- 不改Bitwarden退出/唤醒重开流程，不改NVIDIA显存保存，不改现有CPU/风扇GUI。
- 25项测试通过（原21项+合法表/错签名/截断/校验错误）。systemd单元语法检查通过；另有既存nginx配置警告，与本次单元无关。
- REA evidence已导出私有cache，目标已关闭，interactive进程正常退出。REA/Revula默认不加载状态未改。

## 必须先做的一步

需要用户保存工作后进行**正常重启，不能以休眠替代**。本次尚未获得重启确认，因此没有触发重启。不能安全地在当前运行内核中把九张AML及其已解析方法指针原地搬迁；仅重启powerd无效。

## 重启后继续路线（未完成，不得报验收）

1. 确认boot_id变化；sudo /usr/local/libexec/mechrevo-acpi-check应healthy=true，NVDA0820 status应返回15；留存baseline与日志。
2. sudo systemctl start nvidia-powerd；核对服务持续运行，NPCF无AE_NOT_FOUND。不先enable。若仍失败，继续查静态调用返回码，不能沿用旧的地址失配结论解释新的错误。
3. 使用有硬截止时间和温度上限的CUDA负载，记录daemon OFF→ON→OFF→ON的GPU实际功耗、requested/enforced limit、利用率、频率与温度，确认不是只改显示值。恢复用户原档位及其他覆盖。
4. 再测试普通三档、APPE+官方GAMM/CTAF负载策略以及TB43极限/静音的GPU预算差异；区分CPU温控阈值95/87°C、GPU预算和整机噪声，不互相冒充。
5. 验证新shutdown休眠/唤醒后的ACPI签名、地址、NPCF和powerd状态，保留前后表摘要。此步骤涉及实际休眠，需在用户方便时进行。
6. 只有实际GPU策略生效和恢复路径验收后才启用正式服务、添加GUI控制、更新最终备份。

## 回退

若撤销本次准备，停用mechrevo-acpi-baseline.service，删除本次新增powerd服务/DBus规则/健康助手和HibernateMode片段，daemon-reload并reload dbus。不要删除已有mechrevo-profile-guard/sleep服务。正常重启仍是清理当前坏ACPI会话的必要步骤。

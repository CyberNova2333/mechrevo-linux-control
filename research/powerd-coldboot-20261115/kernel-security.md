# 2026-11-15 内核安全核对

运行版本、磁盘默认启动版本均为 Ubuntu 6.8.0-149.149-generic。检查期间没有启动 7.0；本次 HWE 安装已全部撤销。发行版回补补丁不要求上游主版本号变化，核对依据是 Ubuntu 完整包版本与具体公告，而非“版本号更大”。

| 漏洞 | Ubuntu 24.04 GA 修复版本 | 本机核对 |
| --- | --- | --- |
| [CVE-2026-98432：IPv4 TCP](https://ubuntu.com/security/CVE-2026-98432) | 6.8.0-149.149 | 已运行该修复包；changelog 含 free inet_opt and ireq_opt after an RCU grace period |
| [CVE-2026-99056：ESP-over-TCP/strparser](https://ubuntu.com/security/CVE-2026-99056) | 6.8.0-149.149 | 已运行该修复包；changelog 含等待 strp_recv 结束的修复 |
| [CVE-2026-81000：TUNderflow](https://ubuntu.com/security/CVE-2026-81000) | 6.8.0-149.149 | 已覆盖 |
| [CVE-2026-74469：SCTP 溢出](https://ubuntu.com/security/CVE-2026-74469) | 6.8.0-149.149 | 已覆盖 |
| [CVE-2026-31431：Copy Fail](https://ubuntu.com/security/CVE-2026-31431) | 6.8.0-117.117 | 当前版本更晚，changelog 保留修复记录 |
| [CVE-2026-99284：skb_zerocopy 共享片段标记](https://ubuntu.com/security/CVE-2026-99284) | 官方仍为 Needs evaluation | 未取得本版本已修复的证据，不能声称全部近期漏洞均已解决 |

前两项亦列于 [USN-9022-1](https://ubuntu.com/security/notices/USN-9022-1)，于 2026-11-10 发布。Canonical [6.8.0-149.149 changelog](https://changelogs.ubuntu.com/changelogs/pool/main/l/linux/linux_6.8.0-149.149/changelog) 与公告交叉核对。没有运行漏洞利用程序。

这是指定日期对列出漏洞的核对，不是所有已知或未来漏洞的无条件安全保证。CVE-2026-99284 是另一个本地提权问题，不能与已修复的 TCP CVE-2026-98432 混淆。当前保留 AppArmor 的 unprivileged user namespace 限制，但不把它当成完整补丁；未擅自禁用容器或网络能力。

以后保持 6.8 GA 安全更新渠道；跨系列内核切换先获得用户明确同意，检查 CVE 修复版本、NVIDIA/DKMS、虚拟化、休眠和回退路径，再安排重启。不要为固定系列而 hold 住全部安全更新。

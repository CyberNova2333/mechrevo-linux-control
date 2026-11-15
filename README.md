# MECHREVO 本机控制实验

临时本地 Git 仓库，无远程。适用于 YAOSHI Series-XxAF55xx / BIOS N.1.33MRO91。

## 已安装

- 中文 GTK 控制中心：键盘、灯带、后盖 Logo、性能模式。
- Logo 常亮自选色、七色呼吸、七彩渐变、25%级进亮度和关闭；用户已逐项目视确认。
- 用户会话启动/唤醒恢复三个灯区的最后成功应用设置；服务启动实测通过，真实休眠唤醒尚未验收。
- 修复随机闪烁、极光、烟花的自动播放；用户已确认正常。
- 办公（绿）、均衡（蓝）、狂暴（紫），以及自定义模式（白）的进入与退出。
- 自定义 CPU 功耗 25–75 W（PL1/PL2同步）、可选 GPU 基础功耗65–115 W与 Dynamic Boost关闭或5–25 W、CPU/GPU独立风扇曲线；GPU/DB默认不勾选，不提供超频。
- 静音／高能狂暴已接入性能页：按官方负载计数自动请求联合调度，使用真实固件 ACK；需要在普通狂暴档、原装圆口供电时主动选择。
- 自动灯光页支持三个灯区的 AC/DC 独立亮度与空闲熄灯、活动恢复，默认关闭；手动关闭的灯区不会被自动打开。
- 启动、退出、开机或唤醒不自动套用模式；可见性能页每3秒回读。

GUI以普通用户运行；固定root助手通过polkit提供固定档位和经过范围检查的调校，不能指定地址或任意寄存器值。精确机型、BIOS、资源地址及状态白名单限制访问，部分写入失败会尝试恢复并如实报告。机身按键的外部变化优先。

## 当前研究结果

自定义功耗已用 CPU 持续负载实测，双风扇已验证独立响应。狂暴子模式已定位缺失的官方负载策略和 APPE 接管条件：补齐后 CPU 真实温控阈值可在高能95°C、静音87°C之间切换。[官方CTAF联合负载路径](research/powerd-dispatch-20261115/official-combined/README.md)已两轮实测：静音CPU约45W、高能约75W，GPU两档均约140W，合计约185/215W。[常驻调度与恢复验收](research/turbo-qualification-20261115/README.md)已确认自动阈值、释放、退出、超时及转场；服务已启用，启动时策略inactive。真实休眠和物理失电尚未验收。未进行声压测量。

[最新功耗、风扇与狂暴策略验证](research/power-fan-validation.md)为当前事实；早期研究记录保留为历史，不代表最新发布状态。

自定义 GPU 调节已找到关键 APPE 门禁：相同100W请求在APPE0时约129.4W，APPE1时约99.6W；115W请求约115.1W，完整回退通过。运行SMRW表及CTGP65/115W、DB5/15/25W均已实测，DB关闭可归回100W；支持范围为CTGP65..115W、DB关闭或5..25W，TPP固定官方协议值255。真实helper与GUI已安装这些控件和事务接口；前台1.5秒有界等锁修复后，真实pkexec最终复测已通过：CPU35W+GPU100W/DB15W联合负载末10样本平均34.981/115.026W，最高CPU/GPU均51°C；reset完整归还拥有位、风扇RAM和Logo位，旧CPU-only配置也完整恢复且未写GPU，最终恢复记录消失。实现见[自定义GPU后端](system/mechrevo-custom-gpu-backend.md)。真实 UEFI `APVersionCheck=25`，不能再把随模式变化的 EC 0x7e8 当版本号。见[自定义 GPU 证据](research/custom-gpu-20261115/official-custom-gpu.md)。[覆盖清单](research/scheduler-20261115/coverage.md)区分当前实现、硬件验收与仍未覆盖的官方功能。

- [灯光修复与界面](research/lighting-fix.md)
- [官方模式和水冷逻辑](research/official-modes-and-watercooling.md)：历史静态研究；现已接入离线水冷，固件联动未启用。
- [目标方法哈希与验证摘要](research/custom-turbo-evidence.json)
- research/experimental-turbo：未发布的临时研究模块，不是运行依赖，不得自动安装。

Logo 独立控制已发布，掩码写入保留同字节的性能、电池与支持标志，切档保留灯光位。见[协议与验证](research/logo-20261115/README.md)。

## 测试与维护

`xvfb-run -a /usr/bin/python3 -m unittest discover -s tests -v` 运行离线行为测试；实际GUI和pkexec链路另外验证。

永久GUI源码：`~/.local/share/desktop-maintenance/sources/iteglow-0.3.2`。
永久助手、测试及研究资料：`~/.local/share/desktop-maintenance/sources/mechrevo-profile-1.0`。
维护备份包含这些源码、补丁、助手、polkit、桌面入口和灯光设置，以及新 turbo helper／daemon／lifecycle、模块、验收清单和系统单元；删除此临时仓库不会影响运行。详细恢复步骤见 `~/.local/share/desktop-maintenance/README.md`。

更新前执行 `desktop-maintenance backup 更新前`。更新iteglow时在新版上检查
`patches/iteglow-0.3.2-mechrevo.patch`，解决冲突并跑测试后再替换永久源，不能直接覆盖或让pip抹掉适配。
BIOS更新后需重新验证，不能移除机型/BIOS校验。狂暴调度使用精确机型lease内核桥；服务只加载当前验收哈希，睡前卸载、恢复后健康检查通过才重载，不使用modules-load盲加载。

主服务 `mechrevo-turbo.service` 与睡眠清理单元 `mechrevo-turbo-sleep.service` 均已安装、enabled；服务就绪不等于已经选择子模式。当前验收固定为 **6.8.0-149-generic / BIOS N.1.33MRO91**，模块 SHA-256 为 `803853479e5f28ce8cf2420f2dc05f7b07483c15809e3664bc961d1b4fa250d0`，见[构建清单](system/mechrevo-turbo-driver/build-manifest.json)。更新内核补丁或 BIOS 后需重新构建和实机验收；不得修改校验白名单、复制旧验收标志或自动绕过门禁。继续安装经过核对的发行版安全更新，驱动适配失败时保持功能停用，不以此阻止安全更新。

恢复系统文件：助手 `/usr/local/libexec/mechrevo-profile-helper` 为root:root0755；
`/usr/share/polkit-1/actions/org.cybernova.mechrevo.profile.policy` 和
`/etc/polkit-1/rules.d/49-mechrevo-profile.rules` 为root:root0644。
恢复后用 `pkexec --disable-internal-agent /usr/local/libexec/mechrevo-profile-helper status` 只读核对。

新增系统单元 mechrevo-profile-guard.service 与 mechrevo-profile-sleep.service 均安装到 /etc/systemd/system，root:root0644。恢复时先 reset 撤销调校，再安装助手与单元，执行 systemctl daemon-reload、systemctl enable --now mechrevo-profile-guard.service、systemctl reenable mechrevo-profile-sleep.service。新单元改为RequiredBy=sleep.target，reenable用于移除旧wants并建立requires，使reset失败阻止睡眠；恢复时须确认sleep.target的Requires关系。睡眠前清理、机身按键竞争及守护重启均已实测；唤醒不重施调校。运行时 /run/mechrevo-tuning.json 是本次开机的恢复记录，不备份或跨开机恢复。

turbo 的 `/run/mechrevo-turbo.json`、sleep marker、socket、锁及自动灯光的内存所有权同样不备份、不跨 boot 重放。更新或回退 helper／模块前先释放当前策略、停止相应服务，确认恢复完成；不要在活动 lease 下替换模块。完整运行边界见[后端与生命周期](system/mechrevo-turbo-backend.md)。

## 2026-11-15 GPU联动研究进度

正常重启已恢复 ACPI；powerd 两轮开关实测使 GPU 从约65W恢复到约140W，现已开机启用并加入睡眠前停用、唤醒健康检查。QBoost静音/高能总预算分别190/260W，两者GPU单项上限均140W。[最新官方CTAF对照](research/powerd-dispatch-20261115/official-combined/README.md)显示静音优先把CPU降至约45W，高能约75W，因此GPU两档仍可达到约140W；实际CPU+GPU合计约185/215W，未测得260W输出。此前130/140W GPU差异来自[显式GAMM兼容实验](research/powerd-dispatch-20261115/joint-load/README.md)，不代表本机默认游戏调度支持。临时CPU拓扑适配已撤下，新lease桥、常驻自动阈值及普通档/自定义转场已[实机验收](research/turbo-qualification-20261115/README.md)；睡眠生命周期仅完成pre/post排练，真实睡眠仍待验证。

见 [最新实测、服务恢复与后续路线](research/powerd-coldboot-20261115/README.md)；[之前的ACPI失配诊断](research/powerd-acpi-resume-20261115.md)保留为历史。真实shutdown休眠仍未验收。

内核保持6.8系列；用户明确禁止未经同意跨大版本升级。已撤销本次未启动的7.0 HWE安装。安全更新按具体CVE与Ubuntu包版本核对，见[安全核对](research/powerd-coldboot-20261115/kernel-security.md)。

本轮进一步确认：静音／高能的 75000／145000 mW **TPP偏移**均成功送达 powerd。已解开关键类的字符串和782个间接调用映射；最终预算与控制命令现已动态验证，见[QBoost执行链](research/powerd-dispatch-20261115/README.md)。见[固件联动还原与运行证据](research/firmware-link-20261115/README.md)。

## 灯光自动恢复

`iteglow-session-restore.service` 是当前图形用户的服务，恢复 `~/.local/state/iteglow/state.json` 和 `logo.json`；`logo-prefs.json` 只供编辑器回填。关闭状态不会先发送灯带开启报文。服务在图形会话启动和logind唤醒通知后延迟2秒读取最新记录，用户离开/退出会话时不写硬件，不自动切性能模式。

自动灯光配置独立保存在 `~/.config/iteglow/lighting-policy.json`，默认总开关关闭。启用后由同一会话服务每5秒读取电源和空闲状态，只临时调整已手动开启灯区的亮度；Logo按25%步进。每次执行前重读手动设置，成功后才记录所有权，手动修改优先。自动熄灯不会覆盖最后手动灯效，禁用策略归还仍属于策略的亮度。真实键盘亮度已验证50→20→0→20→50，灯带/Logo手动关闭区未写入，手动状态文件哈希不变；这次使用注入的空闲输入，没有物理拔电或真实休眠。详见[自动灯光策略](docs/lighting-policy.md)。

三灯区事务锁已实际部署，watcher启动恢复三灯区通过；GUI、CLI与watcher使用同一桌面用户。`iteglow set/off/restore`明确拒绝sudo/root，依现有udev权限运行；仅导出用的`pin`仍使用sudo。真实睡眠、物理AC切换和水冷BLE仍未验证。

维护恢复后：`systemctl --user daemon-reload`，再 `systemctl --user enable --now iteglow-session-restore.service`。只读预览可用永久源码模块 `python3 -m iteglow.lighting_session --once`（系统Python和正确PYTHONPATH）。旧CLI的`pin`只导出手工恢复文件，不会安装系统级服务。

# 用户会话灯光恢复与自动策略

更新：2026-11-15。恢复与策略已由主任务同步到永久源码，并以用户服务 `iteglow-session-restore.service` 运行，单元包含 `--apply`。自动策略默认关闭。以下区分部署、离线测试与真实硬件记录，不把合成事件当作物理睡眠或拔电验收。

## 当前入口与恢复源

恢复入口是 [lighting_session.py](../../iteglow/src/iteglow/lighting_session.py)，单元是 [iteglow-session-restore.service](../../system/iteglow-session-restore.service)。[lighting_policy_session.py](../../iteglow/src/iteglow/lighting_policy_session.py) 在同一 watcher 中运行 [纯策略](../../iteglow/src/iteglow/lighting_policy.py)。GUI 左栏“自动灯光”已接入，见[使用说明](../../docs/lighting-policy.md)。

仅恢复用户 `XDG_STATE_HOME/iteglow/state.json` 的 keyboard/lightbar，以及 `logo.json` 的实际已应用 `mode/brightness/color`。Logo brightness=0 表示关闭。编辑偏好 `logo-prefs.json`、旧 system pin、性能调校都不是恢复来源；没有保存值就没有默认灯效。GUI/CLI 成功手动应用后保存，失败明确区分“灯光已应用”和“恢复设置保存失败”。`pin` 仅导出，不启用服务。

直接运行模块仍默认 dry-run；`--once` 只预览一次手动恢复，不运行策略、不写设备。实际单元显式 `--apply`，PYTHONPATH 指向永久源码。旧 system restore 服务没有同时启用。

## 生命周期与输入

watcher 绑定启动时唯一活动、同 UID、本地 seat、x11/wayland、class=user 会话，不改绑其他登录。每区写入前重新核查绑定会话及 `graphical-session.target`。启动及一组完整 `PrepareForSleep(true→false)` 后等待 2 秒恢复最新手动值。true 取消待处理恢复，false 必须跟在 true 后；重复事件不叠加。恢复期间跳过策略，完成后清空策略记忆再计算。

非活动会话不写硬件并丢弃旧策略所有权；同一会话重新活动时先恢复手动值。`PrepareForShutdown(true)`、当前 SessionRemoved、SIGTERM 取消回调。runtime watcher flock 避免重复监听，unit 的 PartOf 让桌面退出停止 watcher。睡前不主动改灯，也没有新增合盖策略。

策略每 5 秒采样一次，仅在绑定会话活动时工作。通用电源使用 UPower OnBattery（bool），空闲使用 Mutter GetIdletime（uint64 毫秒）。这与性能功能的 EC 圆口判断独立。供电亮度按区设置 AC/DC 百分比；空闲超时熄灭，恢复活动后还原当前供电亮度或手动亮度。保留效果、配色、速度；手动关闭区不会点亮。临时策略绝不写 state.json/logo.json。关闭策略只归还本会话确实接管且手动记录未变的区域。

未知电源不猜 AC/DC；未知空闲不把已空闲关闭的灯点亮。无效配置不授权写入。每区实际成功才确认策略所有权，失败区下次重试；稳定状态和重复错误不每 5 秒刷日志。

## 验证与事务边界

保存文件限制 64 KiB，同 UID 普通文件、不能 world-writable，不跟随末级 symlink。字段白名单和范围在生成协议前验证，坏区不阻碍其他有效区。Off/brightness0 只发灭灯报文，灯带不先 power-on。Logo 用固定 pkexec/helper、5 秒超时及实际回读比对，HID 用既有机型/设备识别。恢复不反写手动状态。

本次新补跨进程事务锁，源码和离线竞争测试完成，**锁改动尚待主任务复核、同步部署**：ZoneTransaction 按用户 runtime 和 keyboard/lightbar/logo 分锁；完整 HID 多报文与 query 请求/回读不能交错。GUI/CLI 的发送与保存同属事务，策略取得锁后重读 manual 并 action_matches，启动恢复同样在锁内重读。Logo GUI 异步等待锁，不阻塞 GTK，持锁到 helper 完成和实际值保存后。固定顺序 zone→profile 或 zone→manual-state，无反向路径。独立 manual-state 锁避免两个 HID 区同时保存共享 JSON 而丢记录。该锁只约束合作的同用户客户端，不能约束外部直接访问 HID 的程序。

已验证：恢复 17 项、纯策略 16 项、会话 runner 15 项；事务锁 7 项（含真实多进程竞争、root CLI 拒绝和同线程异步非重入）、Logo 异步事务 7 项。覆盖等待手动修改、完整 HID 事务、共享保存、off 不闪亮、未知输入、会话失效、恢复顺序、异步关闭/超时/重开。GUI 自动灯光导航、窗口关闭清理与性能未知状态另有 Gtk/mock 测试。

实机记录由主任务执行：已安装 watcher 启动后 keyboard/lightbar/logo 恢复成功，策略默认 disabled；真实 D-Bus 电源/idle 读取成功。真实 AC 读数加合成 idle 刺激使键盘原始亮度 **50→20→0→20→50**，各步无错误、手动文件哈希不变。证据 `~/.cache/mechrevo-linux-research/control-replica-20261115/lighting-policy-hardware.json` 明确标注 synthetic_idle_input=true、physical_power_transition=false；主任务另确认手动关闭区不写。

**未验收**：真实注销/重新登录、真实睡眠/唤醒、物理 AC/DC 切换，不能用合成刺激结果替代。设备晚于 2 秒出现会报告恢复失败，未加入盲目重发；新事务锁尚未做实机竞争测试。

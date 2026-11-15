# 自动灯光设置

在机械革命控制中心左栏打开“自动灯光”，开启总开关，按需勾选“分别设置插电与电池亮度”，或填写“空闲后熄灯”的分钟数，再点击“保存自动灯光设置”。只有保存后生效，默认关闭。后台在活动桌面会话中每 5 秒检查一次，因此不一定立即反映到灯光。

| 设置 | 行为 |
| --- | --- |
| 插电 / 电池亮度 | 键盘、灯带为 0–100%；Logo 为 0/25/50/75/100%。保留手动灯效、颜色与速度 |
| 空闲后熄灯 | 1–1440 分钟；0 关闭此功能。恢复活动后还原当前供电策略亮度，未启用供电策略时还原手动亮度 |
| 关闭总开关并保存 | 只把本次策略实际接管、且手动设置未变化的灯区交还手动设置 |

先在各灯区应用希望保留的灯效。没有已保存手动灯效的区域不会自动设置默认效果；手动关闭的区域保持关闭。“关闭全部灯光”也不会被自动策略马上点亮。自动亮度是临时输出，不覆盖手动保存值。

“插电”来自 UPower 通用供电状态，与性能页面的“原装圆口电源”要求不同。电源状态未知时不猜 AC/DC；空闲状态未知时不会主动唤亮已因空闲关闭的灯。

会话启动和睡眠唤醒入口先恢复最后成功保存的手动值，再计算自动策略。恢复期间暂停自动策略，退出桌面或切换到其他用户会停止写入。当前没有合盖、音乐同步、逐键编辑或开机欢迎动画策略。

配置为 `~/.config/iteglow/lighting-policy.json`。手动 HID 状态为 `$XDG_STATE_HOME/iteglow/state.json`（默认 `~/.local/state/iteglow/state.json`），Logo 实际值为 `~/.local/state/iteglow/logo.json`。`logo-prefs.json` 只保存编辑偏好。维护备份应保留这些配置、永久源码和用户服务；不要用旧 system pin 文件替换当前手动状态。

只读检查：

```sh
systemctl --user status iteglow-session-restore.service
journalctl --user -u iteglow-session-restore.service -n 30 --no-pager
PYTHONPATH="$HOME/.local/share/desktop-maintenance/sources/iteglow-0.3.2/src" python3 -m iteglow.lighting_session --once
```

最后一条只预览手动恢复，不执行策略、不写设备。如果 GUI 显示“灯光已应用，但恢复设置保存失败”，当前灯效已发送，下一次恢复仍可能使用旧值；先解决保存错误再重新应用。

新的事务锁要求 GUI、CLI 和 watcher 使用同一个普通桌面用户。`iteglow set`、`off`、`restore` 明确拒绝 sudo/root；本机已有 udev 设备权限，不需要 sudo。仅导出用的 `iteglow pin` 仍需要 sudo。这是旧 CLI 用法的兼容变化，不通过放宽 runtime 目录权限来绕过。

2026-11-15 已确认用户服务启动时三灯区恢复、真实 D-Bus 读取，以及真实键盘在合成空闲刺激下完成 50→20→0→20→50（协议原始范围 0–50，页面百分比范围 0–100），手动文件不变。真实登录、物理睡眠/唤醒和物理 AC/DC 切换尚未测试。实现与验证边界见[会话记录](../research/scheduler-20261115/lighting-session.md)。

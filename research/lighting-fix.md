# 灯效修复与中文控制中心（2026-11-14）

## 已验证的根因

旧实现把 wave 的 direction（默认 1）传给每一种键盘效果，HID 命令第 6 字节实际上复用两种含义：

- 波浪 0x03：1/2/3/4 是方向。
- 随机闪烁 0x04、涟漪 0x06、极光 0x0E、烟花 0x11：0 自动播放，1 按键触发。
- 不使用该参数的其他键盘效果：写 0。机身灯带协议保持独立。

旧版 reactive、aurora、spark 的熄灭现象由误启用按键触发造成，不是硬件不支持。
修复后用户已亲眼确认三种效果都能自动亮起。安装后的新 GUI 逐项点击测试回读同样正确。
reactive 是保留兼容性的 CLI/状态文件标识，GUI 显示实际含义“随机闪烁”。
新 GUI 提供独立的“自动播放 / 随按键触发”选择；通过键盘固件处理按键响应，不捕获 Linux 键盘输入。

协议交叉参考：
- https://github.com/pobrn/ite8291r3-ctl/blob/master/ite8291r3_ctl/ite8291r3.py
- https://github.com/pobrn/ite8291r3-ctl/blob/master/ite8291r3_ctl/__main__.py （--reactive 显式写 1）
- https://github.com/CalcProgrammer1/OpenRGB/blob/master/Controllers/ClevoKeyboardController/RGBController_ClevoKeyboard.cpp （非波浪默认 behavior 0）

注意：tuxedo 驱动注释中 key mode 的 0/1 描述与这两个用户态实现及本机测试相反，不能据此改回。
这里只使用协议事实，未复制参考项目的 GPL 实现代码。

## 界面

原生 GTK4 浅色侧栏、设备示意、中文灯效卡片、独立参数区和性能模式卡片。
亮度显示百分比，键盘仍按固件 0–50、灯带按 0–100 换算；速度显示 1 慢→10 快，内部换算为 10→1。
所有灯效名称、方向和触发选项已汉化；打开窗口不应用灯效、不切性能档。
关闭灯光保留配色和亮度偏好，重新选择效果可恢复非零亮度。
键盘支持当前效果回读，灯带显示最近保存的设置；预览只是布局与配色示意。
界面样式仅作用于本应用，未添加 GNOME 扩展。自定义与狂暴子档仍未开放。

agy 经用户授权完成登录，并提供只读布局建议。最终原生实现、协议修复及验证由当前会话完成。
授权码未写入仓库、维护资料或应用代码。

## 验证与维护

- tests/test_lighting_gui.py：方向/触发隔离、旧保存格式兼容、构建无硬件写入、参数换算和保存、关闭后重选亮度恢复。
- tests/test_profile_helper.py：之前的性能助手 5 项测试仍通过。共 9 项。
- test-auto-effects-repeat.jsonl：用户确认三个灯效正常的实机测试。
- verify-installed-ui.json：新 GUI 三个按钮的实机回读，均为自动播放；测试结束恢复当时的呼吸效果。
- spark-ui.png、lightbar-small-ui.png、performance-new-ui.png：默认/较小窗口的视觉检查。

维护补丁现包含 effects.py、cli.py、gui.py、performance.py、control.css、devices.py。
应用图标及桌面入口纳入维护备份，源码永久保存在 desktop-maintenance/sources；不依赖临时仓库。
恢复或升级时必须一起带上 control.css；CLI 的 --reactive 参数及状态字段也要保留。

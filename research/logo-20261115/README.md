# EC Logo 灯协议恢复（2026-11-15）

本记录来自机械革命控制中心 5.56.60.46 的 GCUService.exe 静态分析。原始文件、离线恢复的 CIL 方法体、静态解码后的分析视图均在 [manifest.json](manifest.json) 中记录 SHA-256。没有执行 OEM 程序；恢复视图不是可编译的官方原始源码。关键 IL 在 [logo-methods.il](logo-methods.il)，相关解码片段在 [logo-excerpts.cs](logo-excerpts.cs)。

## 适用机型与入口

本机的 Linux 后端限定 MECHREVO / YAOSHI Series-XxAF55xx / BIOS N.1.33MRO91，EC 资源为 `INOU0000:00` 的 `0xfe410000–0xfe410fff`。本协议的寄存器号是相对此 EC 资源的偏移，不应拿到其他机型试写。

官方支持检测有两处独立证据：

- `LightingModel.LM_ITE_RGB.GetDeviceSupport`（`0x0600055f`）读取 `0x7ab & 1`，等于 1 时设置 `MBlogoSupport=true`。
- `NvramVariable.isSupportECALogo`（`0x06000ce0`）读取同一个 bit；`NvramVariable.Get` 将结果保存为 `HKLM\Software\OEM\GamingCenter2\Support\MBALogo`，但单区/单色键盘机型强制禁用。本机已确认有逐键 RGB，不属于这个排除分支。

官方有 HID Logo 和 EC Logo 两条路径。`HIDLightbar4_logo` 的 HID control byte 是 35，但每次运行灯效也会调用 `HIDKeyboard.RunEffctEC`。没有相应 HID 实例时，`MyRGBKeyboard.HIDLightbar_logo.RunEffctECALogo` 直接执行相同的 EC 协议；因此 EC Logo 不依赖给键盘或灯带发送 HID 报文。Linux 后端仅实现该精确机型的 EC 路径。

核心方法：

| 方法 | Token | 用途 |
| --- | --- | --- |
| `HIDKeyboard.RunEffctEC` | `0x060017c0` | 应用 EC Logo 灯效、亮度、开关 |
| `HIDKeyboard.CloseEffectEC` | `0x060017c1` | 仅关闭亮度并提交 |
| `HIDKeyboard.DivideValue` | `0x060017c2` | RGB 通道量化 |
| `HIDLightbar_logo.RunEffctECALogo` | `0x0600080b` | 独立 EC 路径，参数来自注册表/GUI |

## 寄存器与所有权

所有共享字节都必须在现有性能锁内**重新读取后只修改所属位**。恢复时也只能恢复所属位，不能把旧的整字节写回。

| 偏移 | Logo 所属位 | 意义 | 必须保留 |
| --- | --- | --- | --- |
| `0x7ab` | `0xe0` | 亮度档 0、1、2、3、4 分别是 0%、25%、50%、75%、100% | 低 5 位，含 bit0 支持标志 |
| `0x767` | `0x80` | 混色模式标志 | 低 7 位 |
| `0x7c5` | `0x07` | 呼吸预设编号 0–7，0 不是呼吸 | 高 5 位，特别是 bit6 电池 Logo 策略与 bit7 性能标志 |
| `0x769` | `0xff` | 常亮红色通道 0–50 | — |
| `0x76a` | `0xff` | 常亮绿色通道 0–50 | — |
| `0x76b` | `0xff` | 常亮蓝色通道 0–50 | — |
| `0x78c` | 只 OR `0x10` | 应用触发 | 其他全部位；不能要求触发位持续为 1 |

`MySettingManager.GetBatteryLogoStatus` / `SetBatteryLogo`（`0x060011f9` / `0x060011fa`）证明 `0x7c5 bit6` 是独立的 `BatteryLogoEnable` 状态：setter 用 `|0x40` 或 `&0xbf`，无额外触发。该功能的视觉语义尚未确认，不能当成主 Logo 开关。其 WMI 支持查询另用 `GetSetULong(Data=3298534883332)`；这不属于 RGB Logo 的必要调用链。

## 灯效与参数

### 常亮

清除 `0x767 bit7` 和 `0x7c5 low3`。每个 RGB 输入字节以 `min(floor(v/5.1),50)` 量化，写入 `0x769..0x76b`。若三个量化结果都为 0，`HIDKeyboard.RunEffctEC` 保留原 RGB 寄存器并将亮度设为 0，避免旧颜色重新发光。

独立的 `RunEffctECALogo` 在黑色时只跳过 RGB 写入，没有将亮度清零；这是两个 OEM 实现的行为差异。Linux 选择前者的关灯行为，避免把“黑色”显示成上一次颜色。

整数量化 `v*10//51` 在 0–255 输入范围与官方浮点路径一致。读回只能知道量化桶，无法恢复用户原先的精确 8-bit 色值；用 `(level*51+9)//10` 作为可往返的代表值，避免反复读回、重新应用时渐渐变暗。普通四舍五入 `round(level*5.1)` 不能保证该性质，例如 level 1 会成为 5，再写回会变成 0。

### 呼吸

清除 `0x767 bit7`，将以下编号写入 `0x7c5 low3`；不写 RGB 通道寄存器。官方 EC 路径只接受这些精确预设，任意颜色不会得到正确呼吸编号。

| 编号 | 颜色 | RGB |
| --- | --- | --- |
| 1 | 白 | 255,255,255 |
| 2 | 红 | 255,0,0 |
| 3 | 绿 | 0,255,0 |
| 4 | 蓝 | 0,0,255 |
| 5 | 黄 | 255,255,0 |
| 6 | 紫 | **136**,0,255 |
| 7 | 青 | 0,255,255 |

### 混色

清除 `0x7c5 low3`，设置 `0x767 bit7`，不写 RGB 通道。官方名为 `Mix`，EC 路径日志称 `Colorful`；本地 GUI 可显示“七彩渐变”。实际颜色轮换形式与周期以本机目视结果为准。

三个灯效最后都更新 `0x7ab` 亮度高三位，再读 `0x78c` 并 OR `0x10` 提交。EC 灯效方法没有写入速度、方向或逐灯分区参数，不能向 GUI 提供无效控制。不要混淆枚举值：`RGBKB_Effect.Single/Breathing/Mix` 是 0/1/17；HID 内部保存的 `_EffectData.save_effect` 使用 1/2/19。

### 关闭

官方 `CloseEffectEC` 只将 `0x7ab` 高三位清零，再 OR `0x78c bit4`。关闭不改变效果与颜色，重新打开可保留之前设置。

## 官方持久化与策略

Logo 的开关、效果和亮度是独立状态，未发现 EC-only 路径需要同步写键盘/灯带状态。官方在 `HKLM\Software\OEM\GamingCenter2\RGBKeyboard` 保存：

- `Lightbar_logo_PowerSwitch`：字符串 0/1。
- `Lightbar_logo_ACLight`、`Lightbar_logo_DCLight`：字符串亮度档 0–4。
- `Lightbar_logo_Effect`：DWORD 枚举。
- `Lightbar_logo_Color_R/G/B`：字符串 0–255。
- HID 实例还用 `SettingsStorageExtensions.SaveAsync` 保存按 project ID 区分的 `LastEffect`。

AC/DC 电源事件选择各自的亮度，然后重放 EC 灯效。`KeyboardCloseTimer` 是键盘与各灯区共享的空闲计时器：用户设定秒数后，键盘或鼠标活动重启计时；超时广播 `true`，Logo 调用 `CloseEffectEC` 或 `SetBrightnessALogo(0)`；恢复活动广播 `false`，恢复此前效果/当前 AC/DC 亮度。0 秒表示禁用空闲熄灯。GUI 的灯光设置不应与该临时亮度状态混为一谈。

`HIDLightbar_logo.Resume` 停止 AP mode 并重放灯效、恢复关闭状态。`Suspend` 调用 `CloseAnimation`，后者通过 HID 的 brightness strategy 操作，不是单独的 EC `CloseEffectEC`。因此源码证明恢复路径存在，但不能仅凭此声称 Linux 已复刻和验证 S3/S4 前后的 Logo 策略。重启、休眠持久化与空闲/ACDC 自动化应由主代理单独实现、验证和记录。

## 本机验证及实现审阅

主代理报告：用户已目视确认白色→红色→蓝色→关闭四步正常。随后用户也确认呼吸与混色两种效果正常；本子任务没有进行任何硬件写入。

`system/mechrevo-profile-helper` 的 Logo 实现已做离线审阅，量化往返问题已向主代理报告并修复。[tests/test_logo.py](../../tests/test_logo.py) 的 14 项测试通过，覆盖共享位所有 256 种原值、七种呼吸映射、常亮/混色/关闭、量化往返、每步写入失败回滚、读回失败、外部变更保护及输入拒绝。测试调用真实 `EC.write_logo` 掩码方法并模拟 `pwrite`，不打开或写入 `/dev/mem`。

读回能确认寄存器内容，不能单独证明灯已发光。应用触发位可能由固件清除；验证应读取 Logo 所属状态位，同时通过目视确认并检查键盘/灯带未受影响。回滚遇到已被其他控制者改写的所属位时，应保留外部值并报告失败，不能以旧快照覆盖用户或固件的新状态。

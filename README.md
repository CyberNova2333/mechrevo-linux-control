<p align="center">
  <img src="iteglow/data/mechrevo-control.svg" alt="MECHREVO Linux Control" width="84" height="84" />
</p>

<h1 align="center">MECHREVO Linux Control (<code>mechrevo-linux-control</code>)</h1>

<p align="center">
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-AGPL--3.0--only-blue.svg" alt="License: AGPL-3.0-only" /></a>
  <a href="#快速测试与-cli-使用"><img src="https://img.shields.io/badge/Python-3.12-3776AB.svg" alt="Python 3.12" /></a>
  <a href="#核心功能模块"><img src="https://img.shields.io/badge/UI-GTK4-4A86CF.svg" alt="GTK4" /></a>
</p>

<p align="center">
  <a href="https://github.com/CyberNova2333/mechrevo-linux-control">项目主页</a> · <a href="#核心功能模块">核心功能模块</a> · <a href="#快速测试与-cli-使用">快速测试与 CLI</a> · <a href="#开源协议-license">开源协议</a>
</p>

面向机械革命 **YAOSHI Series (`XxAF55xx` / BIOS `N.1.33MRO91`)** 的完整 Linux 本地硬件控制中心、外置蓝牙水冷控制栈、EC/WMI 固件性能调校与 NVIDIA/ACPI 休眠修复套件。

## 核心功能模块

### 1. GTK4 硬件控制中心 (`iteglow`)
- **暗色钛合金视觉控制台**：统一深色主题（`control.css` / `gui.py`），涵盖键盘背光、机身灯带、后盖 Logo、自动灯光策略、情景预设联动、性能模式与水冷控制七大模块。
- **键盘背光 & 机身灯带（ITE 8291 / 8233 USB HID）**：支持 101 键单键自定义矩阵（[keymap.py](iteglow/src/iteglow/keymap.py)）、全区灯效与自动播放/按键触发状态同步，使用桌面对等 udev 规则与 `ZoneTransaction` 分区锁（[zone_lock.py](iteglow/src/iteglow/zone_lock.py)）。
- **独立后盖 Logo 灯控制**：EC 掩码安全写入，支持常亮自选色、七色呼吸、七彩渐变、25% 步进亮度与关闭（[logo.py](iteglow/src/iteglow/logo.py)），内嵌实时发光预览。
- **自动灯光、热键同步与情景联动（`session-daemon/` 异步 Rust 守护进程）**：支持插电 / 电池分设亮度与空闲熄灯（[lighting_policy.py](iteglow/src/iteglow/lighting_policy.py) / [session-daemon/src/policy.rs](session-daemon/src/policy.rs)）、`Fn+F6`/`Fn+F7`（WMI `0xC1`）亮度状态同步（[session-daemon/src/hotkey.rs](session-daemon/src/hotkey.rs)），以及基于当前用户应用进程的 opt-in 情景预设切换（[profiles.py](iteglow/src/iteglow/profiles.py) / [session-daemon/src/profiles.rs](session-daemon/src/profiles.rs)），物理模式键 `0xB0` 切换时自动让步；用户级服务 [iteglow-session-restore.service](system/iteglow-session-restore.service) 由单线程异步 `tokio` + `zbus` 原生二进制 `iteglow-sessiond` 承载（常驻内存约 1.5–2.2 MB，与现有分区锁 `ZoneTransaction` 与状态文件完全兼容）。

### 2. 外置蓝牙水冷控制与守护 (`watercooling*` / `watercooling-daemon/`)
- **实机验证协议**：针对 `CoolingSystem LCT22002`（固件 `2.0.0.4`）/ `LCT21001` 的非配对 BlueZ GATT 传输（[watercooling_ble.py](iteglow/src/iteglow/watercooling_ble.py) / [watercooling-daemon/src/ble.rs](watercooling-daemon/src/ble.rs)），禁用易失败的 SMP 配对、支持瞬时 `0x3e` 断链 3 次自动重试、兼容扫描 `InProgress` 与 `AlreadyConnected` 接管、`ATT 0x0e` 瞬时容错与 `>=100ms` UART 发送节流。
- **异步 Rust 守护进程 (`watercooling-daemon/`)**：基于单线程异步 `tokio` + `zbus` 重构的 `iteglow-watercoolingd`（常驻内存约 1.5–2.6 MB，100% 兼容现有 AF_UNIX JSON IPC 与 `~/.config/iteglow/watercooling.json`），`ping`/`status` 走无阻塞快照快路径，具备结构化 `journald` 日志与唤醒/断连指数退避重连，并随附蓝牙防掉线配置（[81-bt-ax211-no-autosuspend.rules](system/81-bt-ax211-no-autosuspend.rules)、[bluez-le-timeout.conf.example](system/bluez-le-timeout.conf.example)）。
- **泵扇驱动与双区灯效**：支持 `0x1C` 水泵供电（`0/7/8/11/12V` + 占空比）、`0x1B` 风扇占空比、`0x19 00 01` 待机（`GoToSleepLC`），并基于 `0x1E` + `0x33`（风扇锁存/同步开关）实现双区协同或分色控制（`sync` / `body_only` / `freeze` / `split`）。
- **多重安全保护**：`FlowSafetyMonitor` 持续监控 `meter_state` 流量信号，干转或断流超时即锁定停泵；用户级守护进程（[iteglow-watercooling.service](system/iteglow-watercooling.service)）默认关闭自动连接（`auto_connect: False`），监听 `logind` `PrepareForSleep` / `PrepareForShutdown` 在睡眠前自动停泵并发送待机指令。
- **EC/WMI 只读协同 telemetry**：[mechrevo-profile-helper](system/mechrevo-profile-helper) 新增只读采集 `0x7A4 / 0x7C7 / 0x7C8 / 0x7E6 / 0x7E7 / 0xD48`，通过 `/run/mechrevo-watercooler-ec.json`（`0644`）发布给水冷服务（不修改 `EC 0x7C7` 或内核桥接模块）。

### 3. 固件性能档位、功耗调校与狂暴调度 (`system/`)
- **固定档位切换**：办公（绿）、均衡（蓝）、狂暴（紫）、自定义（白）模式切换，严格受限于主机 DMI/BIOS/状态校验与 `EC 0x7A5` 高温风扇安全保护。
- **有界功耗与风扇曲线调校**：支持 CPU 功耗上限 `25–75 W`（PL1/PL2 同步）、受限验证范围内的 GPU 基础功耗（`65–115 W`）/ Dynamic Boost（`5–25 W`）及 11 段 CPU/GPU 独立风扇曲线；切换模式、按下机身模式键或睡眠前自动完整回退还原。
- **受控狂暴子模式内核桥**：`system/mechrevo-turbo-driver/mechrevo_turbo_lease.c` 与 `mechrevo_turbo_backend.py` 协同官方 `nvidia-powerd` 触发静音狂暴 / 高能狂暴联合功率调度；以构建 SHA-256 白名单门禁绑定已验收内核版本（**6.8.0-149-generic / BIOS N.1.33MRO91**）。

### 4. NVIDIA 休眠挂起修复与 ACPI 睡眠安全守卫 (`system/`)
- `mechrevo-nvidia-hibernate-patch` + `nvidia-dkms-hibernate-shutdown.patch`：通过 DKMS 挂钩绕过休眠关机阶段 `nv_pci_remove_helper` 异常问题。
- `mechrevo-powerd-sleep` + `mechrevo-acpi-check`：睡眠前后停用与唤醒健康校验，防止过期 ACPI 映射影响唤醒后 GPU 动态加速预算。

---

## 快速测试与 CLI 使用

```bash
# 运行全部离线单元测试与 GTK4 界面测试
PYTHONPATH=iteglow/src GSK_RENDERER=cairo xvfb-run -a /usr/bin/python3 -m unittest discover -s tests -v

# 运行 Rust 异步水冷 & 会话灯光守护进程单元测试与发布构建
(cd watercooling-daemon && cargo test && cargo build --release)
(cd session-daemon && cargo test && cargo build --release)

# 构建 .deb / .rpm / Type-2 .AppImage 发行包（产物输出至 dist/）
FETCH_APPIMAGE_RUNTIME=1 ./packaging/build-packages.sh

# 启动桌面控制中心
PYTHONPATH=iteglow/src /usr/bin/python3 -m iteglow.cli gui

# 外置蓝牙水冷 CLI（普通桌面用户运行，无需 sudo）
PYTHONPATH=iteglow/src /usr/bin/python3 -m iteglow.cli cooler status
PYTHONPATH=iteglow/src /usr/bin/python3 -m iteglow.cli cooler ec-status
PYTHONPATH=iteglow/src /usr/bin/python3 -m iteglow.cli cooler scan
```

> 安全约束与架构细节详见仓库内 [docs/lighting-policy.md](docs/lighting-policy.md)、[system/mechrevo-turbo-backend.md](system/mechrevo-turbo-backend.md) 与 [system/mechrevo-custom-gpu-backend.md](system/mechrevo-custom-gpu-backend.md)；历史实验与协议抓包取证保留在 [research/](research/) 目录下供溯源查阅。

---

## 开源协议 (License)

- 本项目整体以 **GNU Affero General Public License v3.0 (`AGPL-3.0-only`)** 开源发布，完整协议文本见根目录 [LICENSE](LICENSE)。
- 兼容组件声明：
  - `iteglow/` 目录基于上游 **MIT License** 的 [iteglow](https://github.com/JeanElsner/iteglow)（保留 [iteglow/LICENSE](iteglow/LICENSE)）进行深度二次开发，并将补丁同步至 [system/iteglow-0.3.2-mechrevo.patch](system/iteglow-0.3.2-mechrevo.patch)。
  - `system/mechrevo-turbo-driver/mechrevo_turbo_lease.c` 为 Linux 内核模块源码，保持 `MODULE_LICENSE("GPL")`。

- **附加条款与资产规则**：本项目依据 GNU AGPLv3 第 7 条适用 [`LICENSE-ADDITIONAL_TERMS-zh-CN.md`](LICENSE-ADDITIONAL_TERMS-zh-CN.md)（英文参考译文：[`LICENSE-ADDITIONAL_TERMS-en.md`](LICENSE-ADDITIONAL_TERMS-en.md)，以简体中文完整版正文为准），对来源陈述、修改标识、禁止冒充、扩展模块有条件附加许可及项目标识边界作出补充说明；工程落地参考见 [`IMPLEMENTATION-GUIDE.zh-en.md`](IMPLEMENTATION-GUIDE.zh-en.md)，人员署名、AI 辅助工具、上游/第三方代码及独立美术字体与标识资产综合清单见 [`ASSETS_LICENSES.md`](ASSETS_LICENSES.md)。附加要求与第 9.5–9.6 款附加许可豁免均分别核实相应版权持有人有效授权，未取得该等授权的第三方或上游材料仍受其原有有效许可约束。

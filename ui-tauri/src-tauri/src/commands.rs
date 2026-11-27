// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Tauri 2 IPC command handlers bridging the Web UI with native HID devices,
//! the Rust `iteglow-watercoolingd` AF_UNIX IPC socket, user config/state files,
//! and the validated `pkexec /usr/local/libexec/mechrevo-profile-helper` boundary.

use crate::hid::{
    apply_and_save_hid, available_hid_devices, hidraw_for_label, load_saved_hid, preset_cells,
    query_keyboard_hw, KeyboardHwInfo, KBD_BRIGHTNESS_MAX, LB_BRIGHTNESS_MAX, LOGO_COLORS,
};
use crate::zone_lock::{
    config_dir, read_user_json, runtime_dir, state_dir, write_user_json, ZoneGuard,
    MAX_PROFILE_BYTES, MAX_STATE_BYTES,
};
use serde_json::{json, Map, Value};
use std::fs;
use std::path::Path;
use std::process::Stdio;
use std::time::Duration;
use tokio::io::{AsyncBufReadExt, AsyncWriteExt, BufReader};
use tokio::net::UnixStream;
use tokio::process::Command;

const PKEXEC: &str = "/usr/bin/pkexec";
const PROFILE_HELPER: &str = "/usr/local/libexec/mechrevo-profile-helper";
const MIN_FAN_DUTY: [i64; 11] = [40, 40, 40, 40, 50, 55, 60, 70, 80, 90, 100];

// ----------------------------------------------------------------- Helper runners

async fn run_profile_helper(args: &[&str], timeout_secs: u64) -> Result<Value, String> {
    let mut cmd = Command::new(PKEXEC);
    cmd.arg("--disable-internal-agent")
        .arg(PROFILE_HELPER)
        .args(args)
        .stdin(Stdio::null())
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .kill_on_drop(true);
    let output = tokio::time::timeout(Duration::from_secs(timeout_secs), cmd.output())
        .await
        .map_err(|_| format!("硬件助手超时 ({timeout_secs}s)"))?
        .map_err(|e| format!("无法启动硬件助手: {e}"))?;

    let stdout = String::from_utf8_lossy(&output.stdout).trim().to_string();
    let stderr = String::from_utf8_lossy(&output.stderr).trim().to_string();

    let parsed: Result<Value, _> = serde_json::from_str(&stdout);
    if !output.status.success() {
        let msg = parsed
            .ok()
            .and_then(|v| v.get("error").and_then(|e| e.as_str()).map(String::from))
            .filter(|s| !s.is_empty())
            .or_else(|| if !stderr.is_empty() { Some(stderr) } else { None })
            .or_else(|| if !stdout.is_empty() { Some(stdout) } else { None })
            .unwrap_or_else(|| "硬件助手未授权或执行失败".to_string());
        return Err(msg);
    }
    let data = parsed.map_err(|_| {
        if !stderr.is_empty() {
            stderr
        } else {
            "硬件助手未返回有效 JSON（可能未授权）".to_string()
        }
    })?;
    if let Some(err) = data.get("error").and_then(|e| e.as_str()) {
        return Err(err.to_string());
    }
    Ok(data)
}

fn read_dmi(field: &str) -> String {
    fs::read_to_string(Path::new("/sys/class/dmi/id").join(field))
        .map(|s| s.trim().to_string())
        .unwrap_or_else(|_| "--".to_string())
}

fn kernel_release() -> String {
    let mut uts: libc::utsname = unsafe { std::mem::zeroed() };
    if unsafe { libc::uname(&mut uts) } != 0 {
        return "--".to_string();
    }
    let bytes: Vec<u8> = uts
        .release
        .iter()
        .take_while(|&&c| c != 0)
        .map(|&c| c as u8)
        .collect();
    String::from_utf8(bytes).unwrap_or_else(|_| "--".to_string())
}

fn check_flock_held(path: &Path) -> bool {
    let Ok(f) = fs::OpenOptions::new().read(true).open(path) else {
        return false;
    };
    use std::os::unix::io::AsRawFd;
    let rc = unsafe { libc::flock(f.as_raw_fd(), libc::LOCK_EX | libc::LOCK_NB) };
    if rc == 0 {
        unsafe {
            let _ = libc::flock(f.as_raw_fd(), libc::LOCK_UN);
        }
        false
    } else {
        true
    }
}

// ----------------------------------------------------------------- System & All-Off

#[tauri::command]
pub async fn get_system_info() -> Result<Value, String> {
    let rdir = runtime_dir();
    let wc_sock = rdir.join("iteglow-watercooling.sock");
    let wc_up = UnixStream::connect(&wc_sock).await.is_ok();
    let session_up = check_flock_held(&rdir.join("iteglow-session-restore.lock"));
    let turbo_up = Path::new("/run/mechrevo-turbo.sock").exists();
    let guard_up = Path::new("/run/mechrevo-watercooler-ec.json").exists()
        || Path::new("/run/mechrevo-wmi-state.json").exists();

    Ok(json!({
        "sys_vendor": read_dmi("sys_vendor"),
        "board_name": read_dmi("board_name"),
        "bios_version": read_dmi("bios_version"),
        "kernel": kernel_release(),
        "hid_devices": available_hid_devices(),
        "daemons": {
            "watercooling": wc_up,
            "session_restore": session_up,
            "turbo": turbo_up,
            "profile_guard": guard_up,
        },
        "version": env!("CARGO_PKG_VERSION"),
    }))
}

#[tauri::command]
pub async fn all_lights_off() -> Result<Value, String> {
    let mut turned_off = Vec::new();
    let mut errors = Vec::new();

    for dev in ["keyboard", "lightbar"] {
        if hidraw_for_label(dev).is_none() {
            continue;
        }
        let mut saved = match load_saved_hid(dev) {
            Value::Object(m) => m,
            _ => Map::new(),
        };
        saved.insert("mode".into(), json!("off"));
        match apply_and_save_hid(dev, &Value::Object(saved), true) {
            Ok(_) => turned_off.push(dev.to_string()),
            Err(e) => errors.push(format!("{dev}: {e}")),
        }
    }

    match set_logo_enabled(false).await {
        Ok(_) => turned_off.push("logo".to_string()),
        Err(e) => errors.push(format!("logo: {e}")),
    }

    Ok(json!({
        "turned_off": turned_off,
        "errors": errors,
    }))
}

// ----------------------------------------------------------------- Keyboard & Lightbar HID

#[tauri::command]
pub async fn get_lighting_state(device: String) -> Result<Value, String> {
    if device != "keyboard" && device != "lightbar" {
        return Err("未知灯区".to_string());
    }
    let available = hidraw_for_label(&device).is_some();
    let saved = load_saved_hid(&device);
    let saved_mode = saved.get("mode").and_then(|v| v.as_str()).map(String::from);
    let hardware = if available && device == "keyboard" {
        query_keyboard_hw(saved_mode.as_deref()).ok()
    } else {
        None
    };
    let bmax = if device == "keyboard" {
        KBD_BRIGHTNESS_MAX
    } else {
        LB_BRIGHTNESS_MAX
    };
    Ok(json!({
        "device": device,
        "available": available,
        "bmax": bmax,
        "saved": saved,
        "hardware": hardware,
    }))
}

#[tauri::command]
pub async fn apply_lighting(device: String, params: Value) -> Result<Value, String> {
    if device != "keyboard" && device != "lightbar" {
        return Err("未知灯区".to_string());
    }
    let hw = apply_and_save_hid(&device, &params, true)?;
    Ok(json!({
        "ok": true,
        "hardware": hw,
    }))
}

#[tauri::command]
pub async fn poll_keyboard_hw() -> Result<KeyboardHwInfo, String> {
    let saved = load_saved_hid("keyboard");
    let saved_mode = saved.get("mode").and_then(|v| v.as_str()).map(String::from);
    query_keyboard_hw(saved_mode.as_deref())
}

#[tauri::command]
pub async fn get_matrix_preset(preset: String, accent: [u8; 3]) -> Result<Vec<[u8; 3]>, String> {
    preset_cells(&preset, accent)
}

// ----------------------------------------------------------------- Rear-lid Logo

fn validate_logo_params(mode: &str, brightness: u8, color: [u8; 3]) -> Result<Value, String> {
    if !matches!(mode, "solid" | "breathing" | "rainbow") {
        return Err("不支持的 Logo 模式".to_string());
    }
    if !matches!(brightness, 0 | 25 | 50 | 75 | 100) {
        return Err("Logo 亮度仅支持 0、25、50、75、100%".to_string());
    }
    if mode == "breathing" && !LOGO_COLORS.contains(&color) {
        return Err("Logo 呼吸模式仅支持七种固定颜色".to_string());
    }
    Ok(json!({
        "mode": mode,
        "brightness": brightness,
        "color": color,
    }))
}

#[tauri::command]
pub async fn get_logo_status() -> Result<Value, String> {
    let hw = run_profile_helper(&["logo-status"], 5).await?;
    let prefs = read_user_json(&state_dir().join("logo-prefs.json"), MAX_STATE_BYTES)
        .ok()
        .flatten();
    Ok(json!({
        "hardware": hw,
        "prefs": prefs,
    }))
}

#[tauri::command]
pub async fn apply_logo(mode: String, brightness: u8, color: [u8; 3]) -> Result<Value, String> {
    let cfg = validate_logo_params(&mode, brightness, color)?;
    let encoded = serde_json::to_string(&cfg).map_err(|e| e.to_string())?;
    let _guard = ZoneGuard::acquire("logo")?;
    let resp = run_profile_helper(&["logo", &encoded], 5).await?;

    let actual = json!({
        "mode": resp.get("mode").cloned().unwrap_or(json!(mode)),
        "brightness": resp.get("brightness").cloned().unwrap_or(json!(brightness)),
        "color": resp.get("color").cloned().unwrap_or(json!(color)),
    });
    let _sguard = ZoneGuard::acquire("manual-state")?;
    let _ = write_user_json(&state_dir().join("logo.json"), &actual);
    let _ = write_user_json(&state_dir().join("logo-prefs.json"), &cfg);
    Ok(resp)
}

#[tauri::command]
pub async fn set_logo_enabled(enabled: bool) -> Result<Value, String> {
    let _guard = ZoneGuard::acquire("logo")?;
    let arg = if enabled { "on" } else { "off" };
    let resp = run_profile_helper(&["logo", arg], 5).await?;
    if let (Some(mode), Some(brightness), Some(color)) = (
        resp.get("mode"),
        resp.get("brightness"),
        resp.get("color"),
    ) {
        let actual = json!({
            "mode": mode,
            "brightness": brightness,
            "color": color,
        });
        let _sguard = ZoneGuard::acquire("manual-state")?;
        let _ = write_user_json(&state_dir().join("logo.json"), &actual);
    }
    Ok(resp)
}

// ----------------------------------------------------------------- Auto Lighting Policy

fn default_policy_config() -> Value {
    json!({
        "version": 1,
        "enabled": false,
        "power_enabled": false,
        "idle_minutes": 0,
        "brightness": {
            "keyboard": {"ac": 100, "dc": 100},
            "lightbar": {"ac": 100, "dc": 100},
            "logo": {"ac": 100, "dc": 100},
        }
    })
}

fn validate_policy_config(cfg: &Value) -> Result<Value, String> {
    let obj = cfg.as_object().ok_or("灯光策略配置格式无效")?;
    if obj.get("version").and_then(|v| v.as_i64()) != Some(1) {
        return Err("不支持的灯光策略版本".to_string());
    }
    let enabled = obj.get("enabled").and_then(|v| v.as_bool()).ok_or("enabled 需为布尔值")?;
    let power_enabled = obj
        .get("power_enabled")
        .and_then(|v| v.as_bool())
        .ok_or("power_enabled 需为布尔值")?;
    let idle_minutes = obj
        .get("idle_minutes")
        .and_then(|v| v.as_i64())
        .ok_or("空闲分钟数需为整数")?;
    if !(0..=1440).contains(&idle_minutes) {
        return Err("空闲分钟数需在 0..1440 之间".to_string());
    }
    let bright = obj
        .get("brightness")
        .and_then(|v| v.as_object())
        .ok_or("缺少亮度配置")?;
    let mut clean_bright = Map::new();
    for dev in ["keyboard", "lightbar", "logo"] {
        let d = bright
            .get(dev)
            .and_then(|v| v.as_object())
            .ok_or_else(|| format!("缺少 {dev} 亮度配置"))?;
        let ac = d.get("ac").and_then(|v| v.as_i64()).ok_or("AC 亮度需为整数")?;
        let dc = d.get("dc").and_then(|v| v.as_i64()).ok_or("DC 亮度需为整数")?;
        if !(0..=100).contains(&ac) || !(0..=100).contains(&dc) {
            return Err("亮度百分比需在 0..100 之间".to_string());
        }
        if dev == "logo" && (ac % 25 != 0 || dc % 25 != 0) {
            return Err("Logo 亮度仅支持 0、25、50、75、100%".to_string());
        }
        clean_bright.insert(dev.to_string(), json!({"ac": ac, "dc": dc}));
    }
    Ok(json!({
        "version": 1,
        "enabled": enabled,
        "power_enabled": power_enabled,
        "idle_minutes": idle_minutes,
        "brightness": Value::Object(clean_bright),
    }))
}

#[tauri::command]
pub async fn get_lighting_policy() -> Result<Value, String> {
    let path = config_dir().join("lighting-policy.json");
    match read_user_json(&path, MAX_STATE_BYTES)? {
        None => Ok(default_policy_config()),
        Some(v) => validate_policy_config(&v),
    }
}

#[tauri::command]
pub async fn save_lighting_policy(config: Value) -> Result<Value, String> {
    let clean = validate_policy_config(&config)?;
    let path = config_dir().join("lighting-policy.json");
    write_user_json(&path, &clean)?;
    Ok(clean)
}

// ----------------------------------------------------------------- Profiles & App Auto-Switch

fn default_profiles_config() -> Value {
    json!({
        "version": 1,
        "auto_switch_enabled": false,
        "restore_previous": true,
        "cooldown_seconds": 5,
        "profiles": {},
        "rules": [],
    })
}

#[tauri::command]
pub async fn get_builtin_templates() -> Result<Value, String> {
    Ok(json!({
        "quiet-office": {
            "label": "静音办公 · 低亮蓝色背光",
            "performance": {"mode": "office"},
            "lighting": {
                "keyboard": {"mode": "solid", "color": [0, 180, 255], "brightness": 20},
                "lightbar": {"mode": "off"},
                "logo": {"mode": "solid", "brightness": 0, "color": [255, 255, 255]},
            },
        },
        "balanced-daily": {
            "label": "均衡日常 · 正弦波背光",
            "performance": {"mode": "balanced"},
            "lighting": {
                "keyboard": {"mode": "sine", "brightness": 35, "speed": 5, "direction": 1},
                "lightbar": {"mode": "off"},
            },
        },
        "gaming-fps": {
            "label": "FPS 狂暴 · WASD+方向键高亮",
            "performance": {"mode": "turbo", "turbo": "extreme"},
            "lighting": {
                "keyboard": {
                    "mode": "matrix",
                    "preset": "wasd_fps",
                    "color": [0, 255, 255],
                    "brightness": 50,
                    "calibrate": true,
                },
            },
        },
        "dark-focus": {
            "label": "暗夜专注 · 全部熄灯",
            "performance": {"mode": "balanced"},
            "lighting": {
                "keyboard": {"mode": "off"},
                "lightbar": {"mode": "off"},
                "logo": {"mode": "solid", "brightness": 0, "color": [255, 255, 255]},
            },
        },
    }))
}

#[tauri::command]
pub async fn get_profiles_config() -> Result<Value, String> {
    let path = config_dir().join("profiles.json");
    match read_user_json(&path, MAX_PROFILE_BYTES)? {
        None => Ok(default_profiles_config()),
        Some(v) => Ok(v),
    }
}

#[tauri::command]
pub async fn save_profiles_config(config: Value) -> Result<Value, String> {
    let obj = config.as_object().ok_or("情景配置格式无效")?;
    if obj.get("version").and_then(|v| v.as_i64()) != Some(1) {
        return Err("不支持的 profiles 版本".to_string());
    }
    let cooldown = obj
        .get("cooldown_seconds")
        .and_then(|v| v.as_i64())
        .ok_or("cooldown_seconds 需为整数")?;
    if !(0..=120).contains(&cooldown) {
        return Err("退出防抖需在 0..120 秒之间".to_string());
    }
    let path = config_dir().join("profiles.json");
    write_user_json(&path, &config)?;
    Ok(config)
}

#[tauri::command]
pub async fn capture_current_profile(
    label: Option<String>,
    include_lighting: bool,
    include_performance: bool,
) -> Result<Value, String> {
    if !include_lighting && !include_performance {
        return Err("请至少勾选性能档位或三区灯光".to_string());
    }
    let mut profile = Map::new();
    if let Some(lbl) = label.map(|s| s.trim().to_string()).filter(|s| !s.is_empty()) {
        profile.insert("label".into(), json!(lbl));
    }
    if include_performance {
        let st = run_profile_helper(&["status"], 6).await?;
        if let Some(mode) = st.get("mode").and_then(|v| v.as_str()) {
            if matches!(mode, "office" | "balanced" | "turbo" | "custom") {
                let mut perf = Map::new();
                perf.insert("mode".into(), json!(mode));
                if mode == "turbo" {
                    if let Some(t) = st.get("turbo").and_then(|v| v.as_object()) {
                        if t.get("active").and_then(|v| v.as_bool()) == Some(true) {
                            if let Some(var) = t.get("variant").and_then(|v| v.as_str()) {
                                if matches!(var, "quiet" | "extreme") {
                                    perf.insert("turbo".into(), json!(var));
                                }
                            }
                        }
                    }
                } else if mode == "custom"
                    && st.get("tuning_active").and_then(|v| v.as_bool()) == Some(true)
                {
                    if let Some(tuning) = st.get("tuning") {
                        if tuning.is_object() {
                            perf.insert("tuning".into(), tuning.clone());
                        }
                    }
                }
                profile.insert("performance".into(), Value::Object(perf));
            }
        }
    }
    if include_lighting {
        let mut light = Map::new();
        if let Ok(Some(Value::Object(hid))) =
            read_user_json(&state_dir().join("state.json"), MAX_STATE_BYTES)
        {
            for zone in ["keyboard", "lightbar"] {
                if let Some(v) = hid.get(zone) {
                    light.insert(zone.to_string(), v.clone());
                }
            }
        }
        if let Ok(Some(logo @ Value::Object(_))) =
            read_user_json(&state_dir().join("logo.json"), MAX_STATE_BYTES)
        {
            light.insert("logo".to_string(), logo);
        }
        if !light.is_empty() {
            profile.insert("lighting".into(), Value::Object(light));
        }
    }
    if !profile.contains_key("performance") && !profile.contains_key("lighting") {
        return Err("未捕获到任何有效的性能或灯光状态".to_string());
    }
    Ok(Value::Object(profile))
}

#[tauri::command]
pub async fn apply_profile_bundle(profile: Value) -> Result<Value, String> {
    let obj = profile.as_object().ok_or("预设格式无效")?;
    let mut applied_lighting = Vec::new();
    let mut warnings = Vec::new();
    let mut applied_perf: Option<String> = None;

    if let Some(light) = obj.get("lighting").and_then(|v| v.as_object()) {
        for zone in ["keyboard", "lightbar"] {
            if let Some(zcfg) = light.get(zone) {
                if hidraw_for_label(zone).is_none() {
                    warnings.push(format!("{zone}: 未检测到硬件"));
                    continue;
                }
                apply_and_save_hid(zone, zcfg, true)?;
                applied_lighting.push(zone.to_string());
            }
        }
        if let Some(lcfg) = light.get("logo").and_then(|v| v.as_object()) {
            let mode = lcfg
                .get("mode")
                .and_then(|v| v.as_str())
                .unwrap_or("solid")
                .to_string();
            let brightness = lcfg.get("brightness").and_then(|v| v.as_u64()).unwrap_or(0) as u8;
            let color = crate::hid::parse_rgb(lcfg.get("color").unwrap_or(&json!([255, 255, 255])))?;
            apply_logo(mode, brightness, color).await?;
            applied_lighting.push("logo".to_string());
        }
    }

    if let Some(perf) = obj.get("performance").and_then(|v| v.as_object()) {
        let mode = perf
            .get("mode")
            .and_then(|v| v.as_str())
            .ok_or("缺少性能档位")?;
        if mode == "custom" {
            if let Some(tuning) = perf.get("tuning") {
                apply_performance_tuning(tuning.clone()).await?;
                applied_perf = Some("custom".to_string());
            } else {
                set_performance_mode("custom".to_string()).await?;
                applied_perf = Some("custom".to_string());
            }
        } else {
            set_performance_mode(mode.to_string()).await?;
            if mode == "turbo" {
                if let Some(var) = perf.get("turbo").and_then(|v| v.as_str()) {
                    let _ = set_turbo_variant(var.to_string()).await;
                }
            }
            applied_perf = Some(mode.to_string());
        }
    }

    Ok(json!({
        "lighting": applied_lighting,
        "performance": applied_perf,
        "warnings": warnings,
    }))
}

// ----------------------------------------------------------------- Performance & Fan Curves

fn validate_tuning(cfg: &Value) -> Result<Value, String> {
    let obj = cfg.as_object().ok_or("调校参数必须是对象")?;
    let pl1 = obj.get("pl1").and_then(|v| v.as_i64()).ok_or("CPU 功耗需为整数")?;
    let pl2 = obj.get("pl2").and_then(|v| v.as_i64()).ok_or("CPU 功耗需为整数")?;
    if !(25..=75).contains(&pl1) || pl1 != pl2 {
        return Err("已验证 CPU 功耗范围为 25–75 W，且持续/短时上限需一致".to_string());
    }
    let mut out = Map::new();
    out.insert("pl1".into(), json!(pl1));
    out.insert("pl2".into(), json!(pl2));

    for key in ["cpu_fan", "gpu_fan"] {
        let arr = obj
            .get(key)
            .and_then(|v| v.as_array())
            .ok_or("风扇曲线需要 11 个节点")?;
        if arr.len() != 11 {
            return Err("风扇曲线需要 11 个节点".to_string());
        }
        let mut vals = Vec::with_capacity(11);
        for (i, item) in arr.iter().enumerate() {
            let v = item.as_i64().ok_or("风扇节点需为整数")?;
            if v < MIN_FAN_DUTY[i] || v > 100 {
                return Err("风扇不能低于出厂冷却基线，高温节点锁定 100%".to_string());
            }
            vals.push(v);
        }
        if vals.windows(2).any(|w| w[0] > w[1]) {
            return Err("请让风扇占空比随温度单调不减".to_string());
        }
        out.insert(key.into(), json!(vals));
    }

    if let Some(ctgp) = obj.get("gpu_ctgp").filter(|v| !v.is_null()) {
        let v = ctgp.as_i64().ok_or("GPU 基础功耗需为整数")?;
        if !(65..=115).contains(&v) {
            return Err("已验证 GPU cTGP 范围为 65–115 W".to_string());
        }
        out.insert("gpu_ctgp".into(), json!(v));
        if let Some(db) = obj.get("gpu_db").filter(|v| !v.is_null()) {
            let d = db.as_i64().ok_or("Dynamic Boost 需为整数")?;
            if d != 0 && !(5..=25).contains(&d) {
                return Err("Dynamic Boost 仅支持 0 或 5–25 W".to_string());
            }
            out.insert("gpu_db".into(), json!(d));
        }
    }
    Ok(Value::Object(out))
}

#[tauri::command]
pub async fn get_performance_status() -> Result<Value, String> {
    let hw = run_profile_helper(&["status"], 6).await?;
    let prefs = read_user_json(&state_dir().join("performance.json"), MAX_STATE_BYTES)
        .ok()
        .flatten();
    Ok(json!({
        "status": hw,
        "prefs": prefs,
    }))
}

#[tauri::command]
pub async fn set_performance_mode(mode: String) -> Result<Value, String> {
    if !matches!(mode.as_str(), "office" | "balanced" | "turbo" | "custom") {
        return Err("不支持的固件档位".to_string());
    }
    run_profile_helper(&["set", &mode], 6).await
}

#[tauri::command]
pub async fn set_turbo_variant(variant: String) -> Result<Value, String> {
    if !matches!(variant.as_str(), "quiet" | "extreme" | "off") {
        return Err("不支持的狂暴子模式".to_string());
    }
    run_profile_helper(&["turbo", &variant], 6).await
}

#[tauri::command]
pub async fn apply_performance_tuning(config: Value) -> Result<Value, String> {
    let clean = validate_tuning(&config)?;
    let _ = write_user_json(&state_dir().join("performance.json"), &clean);
    let encoded = serde_json::to_string(&clean).map_err(|e| e.to_string())?;
    run_profile_helper(&["apply", &encoded], 8).await
}

#[tauri::command]
pub async fn reset_performance_tuning() -> Result<Value, String> {
    run_profile_helper(&["reset"], 6).await
}

// ----------------------------------------------------------------- Water-Cooling AF_UNIX IPC

fn default_watercooling_config() -> Value {
    json!({
        "version": 1,
        "auto_connect": false,
        "mac": "",
        "strategy": "preset",
        "preset": "balanced",
        "manual": {"pump_volt": 7, "pump_duty": 60, "fan_duty": 40},
        "curve": {
            "hysteresis_c": 3,
            "points": [
                {"temp_c": 45, "pump_volt": 7, "pump_duty": 60, "fan_duty": 40},
                {"temp_c": 65, "pump_volt": 8, "pump_duty": 70, "fan_duty": 55},
                {"temp_c": 80, "pump_volt": 11, "pump_duty": 90, "fan_duty": 85},
            ],
        },
        "led": {
            "mode": "static",
            "color": [0, 128, 255],
            "fan_policy": "sync",
            "fan_color": [0, 128, 255],
        },
        "flow_protection": true,
        "sleep_on_disconnect": true,
    })
}

#[tauri::command]
pub async fn get_watercooling_local_config() -> Result<Value, String> {
    let path = config_dir().join("watercooling.json");
    match read_user_json(&path, MAX_STATE_BYTES)? {
        Some(v) => Ok(v),
        None => Ok(default_watercooling_config()),
    }
}

#[tauri::command]
pub async fn watercooling_request(req: Value) -> Result<Value, String> {
    let sock_path = runtime_dir().join("iteglow-watercooling.sock");
    let op = req.get("op").and_then(|v| v.as_str()).unwrap_or("");
    let timeout_s = match op {
        "scan" | "connect" => 15u64,
        _ => 5u64,
    };

    let io_fut = async {
        let mut stream = UnixStream::connect(&sock_path)
            .await
            .map_err(|e| format!("无法连接水冷后台服务 ({}): {e}", sock_path.display()))?;
        let mut line = serde_json::to_string(&req).map_err(|e| e.to_string())?;
        line.push('\n');
        stream
            .write_all(line.as_bytes())
            .await
            .map_err(|e| format!("发送 IPC 请求失败: {e}"))?;
        let mut reader = BufReader::new(stream);
        let mut resp_line = String::new();
        reader
            .read_line(&mut resp_line)
            .await
            .map_err(|e| format!("读取 IPC 响应失败: {e}"))?;
        let parsed: Value = serde_json::from_str(resp_line.trim())
            .map_err(|e| format!("解析水冷服务响应失败: {e}"))?;
        if parsed.get("ok").and_then(|v| v.as_bool()) != Some(true) {
            let err = parsed
                .get("error")
                .and_then(|v| v.as_str())
                .unwrap_or("水冷服务返回错误");
            return Err(err.to_string());
        }
        Ok(parsed.get("result").cloned().unwrap_or(Value::Null))
    };

    match tokio::time::timeout(Duration::from_secs(timeout_s), io_fut).await {
        Ok(Ok(res)) => Ok(res),
        Ok(Err(e)) => {
            // If saving config while daemon is offline, persist locally cleanly.
            if op == "set_config" {
                if let Some(cfg) = req.get("config") {
                    let path = config_dir().join("watercooling.json");
                    write_user_json(&path, cfg)?;
                    return Ok(json!({"saved_offline": true, "config": cfg}));
                }
            }
            Err(e)
        }
        Err(_) => Err(format!("水冷后台服务响应超时 ({timeout_s}s)")),
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn logo_validation_rules() {
        assert!(validate_logo_params("solid", 50, [12, 34, 56]).is_ok());
        assert!(validate_logo_params("breathing", 75, [0, 255, 255]).is_ok());
        assert!(validate_logo_params("breathing", 75, [12, 34, 56]).is_err());
        assert!(validate_logo_params("solid", 30, [255, 255, 255]).is_err());
        assert!(validate_logo_params("strobe", 50, [255, 255, 255]).is_err());
    }

    #[test]
    fn policy_validation_enforces_logo_steps_and_bounds() {
        let def = default_policy_config();
        assert!(validate_policy_config(&def).is_ok());

        let mut bad_logo = def.clone();
        bad_logo["brightness"]["logo"]["dc"] = json!(33);
        assert!(validate_policy_config(&bad_logo).is_err());

        let mut bad_idle = def.clone();
        bad_idle["idle_minutes"] = json!(1441);
        assert!(validate_policy_config(&bad_idle).is_err());
    }

    #[test]
    fn tuning_validation_enforces_power_and_fan_floor() {
        let ok = json!({
            "pl1": 45,
            "pl2": 45,
            "cpu_fan": [40, 40, 45, 50, 55, 60, 65, 75, 85, 95, 100],
            "gpu_fan": [40, 40, 40, 40, 50, 55, 60, 70, 80, 90, 100],
            "gpu_ctgp": 100,
            "gpu_db": 15
        });
        assert!(validate_tuning(&ok).is_ok());

        let ok_null_gpu = json!({
            "pl1": 35,
            "pl2": 35,
            "cpu_fan": MIN_FAN_DUTY,
            "gpu_fan": MIN_FAN_DUTY,
            "gpu_ctgp": null,
            "gpu_db": null
        });
        let cleaned = validate_tuning(&ok_null_gpu).unwrap();
        assert!(cleaned.get("gpu_ctgp").is_none());

        // Below factory cooling baseline
        let mut bad_floor = ok.clone();
        bad_floor["cpu_fan"][4] = json!(45);
        assert!(validate_tuning(&bad_floor).is_err());

        // Non-monotonic
        let mut bad_mono = ok.clone();
        bad_mono["cpu_fan"][2] = json!(60);
        bad_mono["cpu_fan"][3] = json!(50);
        assert!(validate_tuning(&bad_mono).is_err());

        // Out of range GPU DB (1..4 disallowed)
        let mut bad_db = ok.clone();
        bad_db["gpu_db"] = json!(3);
        assert!(validate_tuning(&bad_db).is_err());
    }
}

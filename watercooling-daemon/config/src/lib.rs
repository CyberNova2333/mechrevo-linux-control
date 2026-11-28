// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Shared persistent settings for the cooler daemon and its offline clients.

use serde::{Deserialize, Serialize};
use serde_json::{Map, Value};
use std::collections::HashSet;
use std::fs;
use std::io::{Read, Write};
use std::os::fd::AsRawFd;
use std::os::unix::fs::{DirBuilderExt, MetadataExt, OpenOptionsExt, PermissionsExt};
use std::path::{Path, PathBuf};
use std::sync::atomic::{AtomicU64, Ordering};

const MAX_CONFIG_BYTES: u64 = 65_536;
static TMP_SEQ: AtomicU64 = AtomicU64::new(0);

pub const VALID_PUMP_VOLTS: &[i64] = &[0, 7, 8, 11, 12];

#[derive(Debug, Clone, PartialEq, Eq, Serialize, Deserialize)]
pub struct StageConfig {
    pub pump_volt: i64,
    pub pump_duty: i64,
    pub fan_duty: i64,
}

#[derive(Debug, Clone, PartialEq, Eq, Serialize, Deserialize)]
pub struct CurvePoint {
    pub temp_c: i64,
    pub pump_volt: i64,
    pub pump_duty: i64,
    pub fan_duty: i64,
}

#[derive(Debug, Clone, PartialEq, Eq, Serialize, Deserialize)]
pub struct CurveConfig {
    pub hysteresis_c: i64,
    pub points: Vec<CurvePoint>,
}

#[derive(Debug, Clone, PartialEq, Eq, Serialize, Deserialize)]
pub struct LedConfig {
    pub mode: String,
    pub color: [i64; 3],
    pub fan_policy: String,
    pub fan_color: [i64; 3],
}

#[derive(Debug, Clone, PartialEq, Eq, Serialize, Deserialize)]
pub struct CoolerConfig {
    pub version: i64,
    pub auto_connect: bool,
    pub mac: String,
    pub strategy: String,
    pub preset: String,
    pub manual: StageConfig,
    pub curve: CurveConfig,
    pub led: LedConfig,
    pub flow_protection: bool,
    pub sleep_on_disconnect: bool,
}

impl Default for CoolerConfig {
    fn default() -> Self {
        Self {
            version: 1,
            auto_connect: false,
            mac: String::new(),
            strategy: "preset".to_string(),
            preset: "silent".to_string(),
            manual: StageConfig {
                pump_volt: 7,
                pump_duty: 60,
                fan_duty: 40,
            },
            curve: CurveConfig {
                hysteresis_c: 3,
                points: vec![
                    CurvePoint {
                        temp_c: 45,
                        pump_volt: 7,
                        pump_duty: 60,
                        fan_duty: 40,
                    },
                    CurvePoint {
                        temp_c: 65,
                        pump_volt: 8,
                        pump_duty: 70,
                        fan_duty: 55,
                    },
                    CurvePoint {
                        temp_c: 80,
                        pump_volt: 11,
                        pump_duty: 90,
                        fan_duty: 85,
                    },
                ],
            },
            led: LedConfig {
                mode: "off".to_string(),
                color: [0, 128, 255],
                fan_policy: "sync".to_string(),
                fan_color: [0, 128, 255],
            },
            flow_protection: true,
            sleep_on_disconnect: true,
        }
    }
}

pub fn config_path() -> PathBuf {
    if let Ok(xdg) = std::env::var("XDG_CONFIG_HOME") {
        if !xdg.trim().is_empty() {
            return PathBuf::from(xdg).join("iteglow").join("watercooling.json");
        }
    }
    let home = std::env::var("HOME").unwrap_or_else(|_| "/tmp".to_string());
    PathBuf::from(home)
        .join(".config")
        .join("iteglow")
        .join("watercooling.json")
}

pub fn pump_volt_to_code(volt: i64) -> Option<u8> {
    match volt {
        11 => Some(0),
        12 => Some(1),
        7 => Some(2),
        8 => Some(3),
        _ => None,
    }
}

pub fn led_mode_code(mode: &str) -> Option<u8> {
    match mode {
        "off" | "static" => Some(0),
        "breathing" => Some(1),
        "colorful" => Some(2),
        "breathing_colorful" => Some(3),
        "mode4" => Some(4),
        _ => None,
    }
}

pub fn preset_stage(preset: &str) -> Option<StageConfig> {
    let (pv, pd, fd) = match preset {
        "stop" => (0, 0, 0),
        "silent" => (7, 60, 40),
        "balanced" => (8, 60, 50),
        "performance" => (11, 60, 60),
        "turbo" => (11, 90, 90),
        "max" => (12, 100, 100),
        _ => return None,
    };
    Some(StageConfig {
        pump_volt: pv,
        pump_duty: pd,
        fan_duty: fd,
    })
}

fn valid_strategy(s: &str) -> bool {
    matches!(s, "manual" | "preset" | "smart_curve" | "ec_follow")
}

fn valid_fan_policy(s: &str) -> bool {
    matches!(s, "sync" | "body_only" | "freeze" | "split")
}

pub fn is_valid_mac(mac: &str) -> bool {
    let bytes = mac.as_bytes();
    if bytes.len() != 17 {
        return false;
    }
    for (i, &b) in bytes.iter().enumerate() {
        if i % 3 == 2 {
            if b != b':' {
                return false;
            }
        } else if !b.is_ascii_hexdigit() {
            return false;
        }
    }
    true
}

fn require_int(v: &Value, low: i64, high: i64, field: &str) -> Result<i64, String> {
    let n = v
        .as_i64()
        .ok_or_else(|| format!("{field}: 需要 {low}–{high} 的整数"))?;
    if !(low..=high).contains(&n) {
        return Err(format!("{field}: 需要 {low}–{high} 的整数"));
    }
    Ok(n)
}

fn require_rgb(v: &Value, field: &str) -> Result<[i64; 3], String> {
    let arr = v
        .as_array()
        .ok_or_else(|| format!("{field}: 需要 3 个 RGB 通道"))?;
    if arr.len() != 3 {
        return Err(format!("{field}: 需要 3 个 RGB 通道"));
    }
    Ok([
        require_int(&arr[0], 0, 255, field)?,
        require_int(&arr[1], 0, 255, field)?,
        require_int(&arr[2], 0, 255, field)?,
    ])
}

fn validate_stage(obj: &Map<String, Value>, label: &str) -> Result<StageConfig, String> {
    let pv_v = obj
        .get("pump_volt")
        .ok_or_else(|| format!("{label}: 缺少 pump_volt / pump_duty / fan_duty"))?;
    let pd_v = obj
        .get("pump_duty")
        .ok_or_else(|| format!("{label}: 缺少 pump_volt / pump_duty / fan_duty"))?;
    let fd_v = obj
        .get("fan_duty")
        .ok_or_else(|| format!("{label}: 缺少 pump_volt / pump_duty / fan_duty"))?;

    let pv = pv_v
        .as_i64()
        .filter(|n| VALID_PUMP_VOLTS.contains(n))
        .ok_or_else(|| format!("{label}.pump_volt: 仅支持 0, 7, 8, 11, 12"))?;
    let mut pd = require_int(pd_v, 0, 100, &format!("{label}.pump_duty"))?;
    let fd = require_int(fd_v, 0, 100, &format!("{label}.fan_duty"))?;
    if pv == 0 {
        pd = 0;
    } else if pd == 0 {
        return Err(format!("{label}: 水泵非 0V 时 pump_duty 必须为 1–100"));
    }
    Ok(StageConfig {
        pump_volt: pv,
        pump_duty: pd,
        fan_duty: fd,
    })
}

pub fn validate_config_value(raw: &Value) -> Result<CoolerConfig, String> {
    let obj = raw
        .as_object()
        .ok_or_else(|| "水冷配置必须是 JSON 对象".to_string())?;
    let allowed: HashSet<&str> = [
        "version",
        "auto_connect",
        "mac",
        "strategy",
        "preset",
        "manual",
        "curve",
        "led",
        "flow_protection",
        "sleep_on_disconnect",
    ]
    .into_iter()
    .collect();
    let mut unknown: Vec<&str> = obj
        .keys()
        .map(String::as_str)
        .filter(|k| !allowed.contains(k))
        .collect();
    if !unknown.is_empty() {
        unknown.sort_unstable();
        return Err(format!("水冷配置包含未知字段: {unknown:?}"));
    }

    let mut cfg = CoolerConfig::default();

    if let Some(v) = obj.get("version") {
        if v.as_i64() != Some(1) {
            return Err("不支持的水冷配置版本".to_string());
        }
    }
    if let Some(v) = obj.get("auto_connect") {
        cfg.auto_connect = v.as_bool().ok_or("auto_connect: 需要布尔值")?;
    }
    if let Some(v) = obj.get("flow_protection") {
        cfg.flow_protection = v.as_bool().ok_or("flow_protection: 需要布尔值")?;
    }
    if let Some(v) = obj.get("sleep_on_disconnect") {
        cfg.sleep_on_disconnect = v.as_bool().ok_or("sleep_on_disconnect: 需要布尔值")?;
    }
    if let Some(v) = obj.get("mac") {
        let s = v.as_str().ok_or("mac: 需要空字符串或 XX:XX:XX:XX:XX:XX")?;
        if !s.is_empty() && !is_valid_mac(s) {
            return Err("mac: 需要空字符串或 XX:XX:XX:XX:XX:XX".to_string());
        }
        cfg.mac = s.to_ascii_uppercase();
    }
    if let Some(v) = obj.get("strategy") {
        let s = v
            .as_str()
            .filter(|s| valid_strategy(s))
            .ok_or("strategy: 仅支持 ('manual', 'preset', 'smart_curve', 'ec_follow')")?;
        cfg.strategy = s.to_string();
    }
    if let Some(v) = obj.get("preset") {
        let s = v.as_str().filter(|s| preset_stage(s).is_some()).ok_or(
            "preset: 仅支持 ('stop', 'silent', 'balanced', 'performance', 'turbo', 'max')",
        )?;
        cfg.preset = s.to_string();
    }
    if let Some(v) = obj.get("manual") {
        let m = v
            .as_object()
            .ok_or("manual: 字段必须为 pump_volt, pump_duty, fan_duty")?;
        if m.len() != 3
            || !m.contains_key("pump_volt")
            || !m.contains_key("pump_duty")
            || !m.contains_key("fan_duty")
        {
            return Err("manual: 字段必须为 pump_volt, pump_duty, fan_duty".to_string());
        }
        cfg.manual = validate_stage(m, "manual")?;
    }
    if let Some(v) = obj.get("curve") {
        let c = v
            .as_object()
            .ok_or("curve: 字段必须为 hysteresis_c, points")?;
        if c.len() != 2 || !c.contains_key("hysteresis_c") || !c.contains_key("points") {
            return Err("curve: 字段必须为 hysteresis_c, points".to_string());
        }
        let hyst = require_int(&c["hysteresis_c"], 1, 15, "curve.hysteresis_c")?;
        let pts = c["points"]
            .as_array()
            .filter(|a| (2..=6).contains(&a.len()))
            .ok_or("curve.points: 需要 2–6 个温控节点")?;
        let mut parsed = Vec::with_capacity(pts.len());
        let mut prev_t = -1_i64;
        for (idx, pt) in pts.iter().enumerate() {
            let po = pt
                .as_object()
                .ok_or_else(|| format!("curve.points[{idx}]: 字段不完整或有多余字段"))?;
            if po.len() != 4
                || !po.contains_key("temp_c")
                || !po.contains_key("pump_volt")
                || !po.contains_key("pump_duty")
                || !po.contains_key("fan_duty")
            {
                return Err(format!("curve.points[{idx}]: 字段不完整或有多余字段"));
            }
            let t = require_int(
                &po["temp_c"],
                25,
                100,
                &format!("curve.points[{idx}].temp_c"),
            )?;
            if t <= prev_t {
                return Err("curve.points: temp_c 必须严格递增".to_string());
            }
            prev_t = t;
            let st = validate_stage(po, &format!("curve.points[{idx}]"))?;
            parsed.push(CurvePoint {
                temp_c: t,
                pump_volt: st.pump_volt,
                pump_duty: st.pump_duty,
                fan_duty: st.fan_duty,
            });
        }
        cfg.curve = CurveConfig {
            hysteresis_c: hyst,
            points: parsed,
        };
    }
    if let Some(v) = obj.get("led") {
        let l = v.as_object().ok_or("led: 包含未知字段")?;
        let led_allowed: HashSet<&str> = ["mode", "color", "fan_policy", "fan_color"]
            .into_iter()
            .collect();
        if l.keys().any(|k| !led_allowed.contains(k.as_str())) {
            return Err("led: 包含未知字段".to_string());
        }
        let mut led = cfg.led.clone();
        if let Some(m) = l.get("mode") {
            let ms = m
                .as_str()
                .filter(|s| led_mode_code(s).is_some())
                .ok_or("led.mode: 无效灯光模式")?;
            led.mode = ms.to_string();
        }
        if let Some(fp) = l.get("fan_policy") {
            let fps = fp
                .as_str()
                .filter(|s| valid_fan_policy(s))
                .ok_or("led.fan_policy: 仅支持 ('sync', 'body_only', 'freeze', 'split')")?;
            led.fan_policy = fps.to_string();
        }
        if let Some(col) = l.get("color") {
            led.color = require_rgb(col, "led.color")?;
        }
        if let Some(fcol) = l.get("fan_color") {
            led.fan_color = require_rgb(fcol, "led.fan_color")?;
        }
        cfg.led = led;
    }

    Ok(cfg)
}

pub fn merge_config_patch(current: &CoolerConfig, patch: &Value) -> Result<CoolerConfig, String> {
    let patch_obj = patch
        .as_object()
        .ok_or_else(|| "配置更新必须是 JSON 对象".to_string())?;
    let mut base = serde_json::to_value(current)
        .map_err(|e| e.to_string())?
        .as_object()
        .cloned()
        .unwrap_or_default();

    for (k, v) in patch_obj {
        if matches!(k.as_str(), "manual" | "curve" | "led") {
            if let (Some(sub_patch), Some(Value::Object(existing))) =
                (v.as_object(), base.get(k).cloned())
            {
                let mut merged_sub = existing;
                for (sk, sv) in sub_patch {
                    merged_sub.insert(sk.clone(), sv.clone());
                }
                base.insert(k.clone(), Value::Object(merged_sub));
                continue;
            }
        }
        base.insert(k.clone(), v.clone());
    }
    validate_config_value(&Value::Object(base))
}

pub fn load_config(path: &Path) -> Result<CoolerConfig, String> {
    let f = match fs::OpenOptions::new()
        .read(true)
        .custom_flags(libc::O_NOFOLLOW | libc::O_CLOEXEC | libc::O_NONBLOCK)
        .open(path)
    {
        Ok(f) => f,
        Err(e) if e.kind() == std::io::ErrorKind::NotFound => return Ok(CoolerConfig::default()),
        Err(e) => return Err(format!("读取水冷配置失败: {e}")),
    };
    let meta = f
        .metadata()
        .map_err(|e| format!("读取水冷配置属性失败: {e}"))?;
    if !meta.is_file() {
        return Err("水冷配置文件不是普通文件".to_string());
    }
    if meta.len() > MAX_CONFIG_BYTES {
        return Err("水冷配置文件体积超过限制".to_string());
    }
    let my_uid = unsafe { libc::geteuid() };
    if meta.uid() != my_uid || (meta.mode() & 0o022) != 0 {
        return Err("水冷配置文件归属或权限不安全".to_string());
    }
    let mut buf = Vec::new();
    f.take(MAX_CONFIG_BYTES + 1)
        .read_to_end(&mut buf)
        .map_err(|e| format!("读取水冷配置失败: {e}"))?;
    if (buf.len() as u64) > MAX_CONFIG_BYTES {
        return Err("水冷配置文件体积超过限制".to_string());
    }
    let raw: Value =
        serde_json::from_slice(&buf).map_err(|e| format!("解析水冷配置 JSON 失败: {e}"))?;
    validate_config_value(&raw)
}

pub fn save_config(cfg: &CoolerConfig, path: &Path) -> Result<CoolerConfig, String> {
    let val = serde_json::to_value(cfg).map_err(|e| e.to_string())?;
    let canonical = validate_config_value(&val)?;
    if let Some(parent) = path.parent() {
        fs::DirBuilder::new()
            .recursive(true)
            .mode(0o700)
            .create(parent)
            .map_err(|e| format!("创建配置目录失败: {e}"))?;
    }
    let seq = TMP_SEQ.fetch_add(1, Ordering::Relaxed);
    let mut tmp = path.as_os_str().to_os_string();
    tmp.push(format!(".tmp.{}.{seq}", std::process::id()));
    let tmp_path = PathBuf::from(tmp);
    let mut body = serde_json::to_string_pretty(&canonical).map_err(|e| e.to_string())?;
    body.push('\n');
    let mut f = fs::OpenOptions::new()
        .write(true)
        .create_new(true)
        .mode(0o600)
        .custom_flags(libc::O_NOFOLLOW | libc::O_CLOEXEC)
        .open(&tmp_path)
        .map_err(|e| format!("创建临时配置失败: {e}"))?;
    let write_res: Result<(), String> = (|| {
        f.set_permissions(fs::Permissions::from_mode(0o600))
            .map_err(|e| format!("设置临时配置权限失败: {e}"))?;
        f.write_all(body.as_bytes())
            .map_err(|e| format!("写入临时配置失败: {e}"))?;
        f.sync_all().map_err(|e| format!("刷盘临时配置失败: {e}"))?;
        Ok(())
    })();
    if let Err(e) = write_res {
        let _ = fs::remove_file(&tmp_path);
        return Err(e);
    }
    if let Err(e) = fs::rename(&tmp_path, path) {
        let _ = fs::remove_file(&tmp_path);
        return Err(format!("替换配置文件失败: {e}"));
    }
    Ok(canonical)
}

/// The daemon holds this lock for its lifetime. An offline writer holds it for one patch.
pub fn acquire_lock(runtime_dir: &Path) -> Result<fs::File, String> {
    let uid = unsafe { libc::geteuid() };
    let dir = fs::symlink_metadata(runtime_dir).map_err(|e| format!("检查运行时目录失败: {e}"))?;
    if !dir.is_dir() || dir.uid() != uid || dir.mode() & 0o022 != 0 {
        return Err("不安全的水冷运行时目录".into());
    }
    let file = fs::OpenOptions::new()
        .create(true)
        .truncate(false)
        .read(true)
        .write(true)
        .mode(0o600)
        .custom_flags(libc::O_CLOEXEC | libc::O_NOFOLLOW | libc::O_NONBLOCK)
        .open(runtime_dir.join("iteglow-watercooling.lock"))
        .map_err(|e| e.to_string())?;
    let meta = file.metadata().map_err(|e| e.to_string())?;
    if !meta.is_file() || meta.uid() != uid || meta.mode() & 0o077 != 0 {
        return Err("水冷锁文件权限异常".into());
    }
    if unsafe { libc::flock(file.as_raw_fd(), libc::LOCK_EX | libc::LOCK_NB) } != 0 {
        return Err(format!(
            "水冷服务或配置正忙: {}",
            std::io::Error::last_os_error()
        ));
    }
    Ok(file)
}

pub fn save_offline_patch(
    path: &Path,
    runtime_dir: &Path,
    patch: &Value,
) -> Result<CoolerConfig, String> {
    let _lock = acquire_lock(runtime_dir)?;
    let updated = merge_config_patch(&load_config(path)?, patch)?;
    save_config(&updated, path)
}

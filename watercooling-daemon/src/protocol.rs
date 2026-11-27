// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Pure LCT22002 / LCT21001 BLE protocol framing, config validation, telemetry readers,
//! thermal-curve evaluator, and flow-safety state machine.
//!
//! 100% compatible with `iteglow/watercooling.py` JSON schema and wire protocol.

use serde::{Deserialize, Serialize};
use serde_json::{Map, Value};
use std::collections::HashSet;
use std::fs;
use std::io::{Read, Write};
use std::os::unix::fs::{MetadataExt, OpenOptionsExt, PermissionsExt};
use std::path::{Path, PathBuf};
use std::sync::atomic::{AtomicU64, Ordering};

const MAX_CONFIG_BYTES: u64 = 65_536;
static TMP_SEQ: AtomicU64 = AtomicU64::new(0);

#[allow(dead_code)]
pub const NUS_SERVICE_UUID: &str = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
pub const NUS_TX_UUID: &str = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";
pub const NUS_RX_UUID: &str = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";
pub const DEVICE_NAME_PREFIXES: &[&str] = &["CoolingSystem LCT22002", "CoolingSystem LCT21001"];

pub const VALID_PUMP_VOLTS: &[i64] = &[0, 7, 8, 11, 12];
pub const DEFAULT_WMI_STATE_PATH: &str = "/run/mechrevo-wmi-state.json";
pub const DEFAULT_LC_EC_STATE_PATH: &str = "/run/mechrevo-watercooler-ec.json";

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
        let s = v
            .as_str()
            .ok_or("mac: 需要空字符串或 XX:XX:XX:XX:XX:XX")?;
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
        let s = v
            .as_str()
            .filter(|s| preset_stage(s).is_some())
            .ok_or("preset: 仅支持 ('stop', 'silent', 'balanced', 'performance', 'turbo', 'max')")?;
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
            let t = require_int(&po["temp_c"], 25, 100, &format!("curve.points[{idx}].temp_c"))?;
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
    let meta = f.metadata().map_err(|e| format!("读取水冷配置属性失败: {e}"))?;
    if !meta.is_file() {
        return Err("水冷配置文件不是普通文件".to_string());
    }
    if meta.len() > MAX_CONFIG_BYTES {
        return Err("水冷配置文件体积超过限制".to_string());
    }
    let my_uid = unsafe { libc::geteuid() };
    if meta.uid() != my_uid || (meta.mode() & 0o002) != 0 {
        return Err("水冷配置文件归属或权限不安全".to_string());
    }
    let mut buf = Vec::new();
    f.take(MAX_CONFIG_BYTES + 1)
        .read_to_end(&mut buf)
        .map_err(|e| format!("读取水冷配置失败: {e}"))?;
    if (buf.len() as u64) > MAX_CONFIG_BYTES {
        return Err("水冷配置文件体积超过限制".to_string());
    }
    let raw: Value = serde_json::from_slice(&buf).map_err(|e| format!("解析水冷配置 JSON 失败: {e}"))?;
    validate_config_value(&raw)
}

pub fn save_config(cfg: &CoolerConfig, path: &Path) -> Result<CoolerConfig, String> {
    let val = serde_json::to_value(cfg).map_err(|e| e.to_string())?;
    let canonical = validate_config_value(&val)?;
    if let Some(parent) = path.parent() {
        fs::create_dir_all(parent).map_err(|e| format!("创建配置目录失败: {e}"))?;
        let _ = fs::set_permissions(parent, fs::Permissions::from_mode(0o700));
    }
    let seq = TMP_SEQ.fetch_add(1, Ordering::Relaxed);
    let mut tmp = path.as_os_str().to_os_string();
    tmp.push(format!(".tmp.{}.{seq}", std::process::id()));
    let tmp_path = PathBuf::from(tmp);
    let mut body = serde_json::to_string_pretty(&canonical).map_err(|e| e.to_string())?;
    body.push('\n');
    let write_res: Result<(), String> = (|| {
        let mut f = fs::OpenOptions::new()
            .write(true)
            .create_new(true)
            .mode(0o600)
            .custom_flags(libc::O_NOFOLLOW | libc::O_CLOEXEC)
            .open(&tmp_path)
            .map_err(|e| format!("创建临时配置失败: {e}"))?;
        f.set_permissions(fs::Permissions::from_mode(0o600))
            .map_err(|e| format!("设置临时配置权限失败: {e}"))?;
        f.write_all(body.as_bytes())
            .map_err(|e| format!("写入临时配置失败: {e}"))?;
        f.sync_all()
            .map_err(|e| format!("刷盘临时配置失败: {e}"))?;
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

// ---------------------------------------------------------------- BLE framing

pub fn encode_handshake() -> Vec<u8> {
    b"sw".to_vec()
}

pub fn encode_meter_query() -> Vec<u8> {
    vec![0xFE, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEF]
}

pub fn encode_sleep() -> Vec<u8> {
    vec![0xFE, 0x19, 0x00, 0x01, 0x00, 0x00, 0x00, 0xEF]
}

pub fn encode_fan(duty: i64) -> Result<Vec<u8>, String> {
    if !(0..=100).contains(&duty) {
        return Err("fan_duty: 需要 0–100 的整数".to_string());
    }
    if duty == 0 {
        Ok(vec![0xFE, 0x1B, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEF])
    } else {
        Ok(vec![0xFE, 0x1B, 0x01, duty as u8, 0x00, 0x00, 0x00, 0xEF])
    }
}

pub fn encode_pump(volt: i64, duty: i64) -> Result<Vec<u8>, String> {
    if !VALID_PUMP_VOLTS.contains(&volt) {
        return Err("pump_volt: 仅支持 0, 7, 8, 11, 12".to_string());
    }
    if !(0..=100).contains(&duty) {
        return Err("pump_duty: 需要 0–100 的整数".to_string());
    }
    if volt == 0 || duty == 0 {
        Ok(vec![0xFE, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEF])
    } else {
        let code = pump_volt_to_code(volt).unwrap();
        Ok(vec![0xFE, 0x1C, 0x01, duty as u8, code, 0x00, 0x00, 0xEF])
    }
}

pub fn encode_main_led(on: bool, color: [i64; 3], mode: &str) -> Result<Vec<u8>, String> {
    if !on || mode == "off" {
        return Ok(vec![0xFE, 0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEF]);
    }
    let m = led_mode_code(mode).ok_or_else(|| format!("led.mode: 未知模式 {mode:?}"))?;
    for c in color {
        if !(0..=255).contains(&c) {
            return Err("led.color: 需要 0–255 的整数".to_string());
        }
    }
    Ok(vec![
        0xFE,
        0x1E,
        0x01,
        color[0] as u8,
        color[1] as u8,
        color[2] as u8,
        m,
        0xEF,
    ])
}

pub fn encode_fan_led_ctrl(freeze: bool, color: [u8; 3], mode_code: u8) -> Vec<u8> {
    if !freeze {
        vec![0xFE, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00, 0xEF]
    } else {
        vec![0xFE, 0x33, 0x01, color[0], color[1], color[2], mode_code, 0xEF]
    }
}

pub fn encode_led_plan(led: &LedConfig) -> Result<Vec<(Vec<u8>, &'static str)>, String> {
    let mode = led.mode.as_str();
    let policy = led.fan_policy.as_str();
    if mode == "off" {
        return Ok(vec![
            (encode_fan_led_ctrl(false, [0, 0, 0], 0), "led_sync_reset"),
            (encode_main_led(false, led.color, "off")?, "led_all_off"),
        ]);
    }
    match policy {
        "sync" => Ok(vec![
            (encode_fan_led_ctrl(false, [0, 0, 0], 0), "led_sync_enable"),
            (encode_main_led(true, led.color, mode)?, "led_main"),
        ]),
        "body_only" => Ok(vec![
            (
                encode_main_led(false, led.color, "off")?,
                "led_clear_for_body_only",
            ),
            (
                encode_fan_led_ctrl(true, [0, 0, 0], 0),
                "led_fan_freeze_off",
            ),
            (encode_main_led(true, led.color, mode)?, "led_body_main"),
        ]),
        "freeze" => Ok(vec![
            (encode_fan_led_ctrl(true, [0, 0, 0], 0), "led_fan_freeze"),
            (encode_main_led(true, led.color, mode)?, "led_body_main"),
        ]),
        "split" => Ok(vec![
            (
                encode_fan_led_ctrl(false, [0, 0, 0], 0),
                "led_split_sync_fan",
            ),
            (
                encode_main_led(true, led.fan_color, "static")?,
                "led_split_latch_fan",
            ),
            (
                encode_fan_led_ctrl(true, [0, 0, 0], 0),
                "led_split_freeze_fan",
            ),
            (encode_main_led(true, led.color, mode)?, "led_split_body"),
        ]),
        other => Err(format!("Unknown fan_policy: {other:?}")),
    }
}

#[derive(Debug, Clone, PartialEq, Eq)]
pub enum RxPacket {
    Meter {
        opcode: u8,
        byte2: u8,
        meter_state: u8,
    },
    Firmware {
        text: String,
        fw_version: Option<String>,
    },
    Unknown,
}

pub fn parse_rx_packet(raw: &[u8]) -> RxPacket {
    if raw.len() >= 4 && raw[0] == 0xFE && (raw[1] == 0x31 || raw[1] == 0x32) {
        return RxPacket::Meter {
            opcode: raw[1],
            byte2: raw[2],
            meter_state: raw[3],
        };
    }
    if let Ok(s) = std::str::from_utf8(raw) {
        let text = s.trim_matches(|c| matches!(c, '\0' | '\r' | '\n' | ' '));
        let fw_version = text
            .strip_prefix("MCU F/W Version:")
            .map(str::trim)
            .filter(|v| !v.is_empty())
            .map(ToOwned::to_owned);
        return RxPacket::Firmware {
            text: text.to_string(),
            fw_version,
        };
    }
    RxPacket::Unknown
}

// ----------------------------------------------------------- Telemetry readers

pub fn monotonic_ms() -> i64 {
    let mut ts = libc::timespec {
        tv_sec: 0,
        tv_nsec: 0,
    };
    unsafe {
        if libc::clock_gettime(libc::CLOCK_MONOTONIC, &mut ts) == 0 {
            return ts.tv_sec * 1000 + ts.tv_nsec / 1_000_000;
        }
    }
    0
}

pub fn read_host_temp_c(hwmon_root: &Path) -> Option<f64> {
    let entries = fs::read_dir(hwmon_root).ok()?;
    let mut dirs: Vec<PathBuf> = entries.filter_map(|e| e.ok().map(|x| x.path())).collect();
    dirs.sort();

    let mut package_temps = Vec::new();
    let mut fallback_temps = Vec::new();

    for dev in dirs {
        let Ok(name_raw) = fs::read_to_string(dev.join("name")) else {
            continue;
        };
        let name = name_raw.trim();
        if !matches!(
            name,
            "coretemp" | "k10temp" | "zenpower" | "acpitz" | "nouveau" | "amdgpu"
        ) {
            continue;
        }
        let Ok(sub) = fs::read_dir(&dev) else {
            continue;
        };
        let mut inputs: Vec<PathBuf> = sub
            .filter_map(|e| e.ok().map(|x| x.path()))
            .filter(|p| {
                p.file_name()
                    .and_then(|s| s.to_str())
                    .map(|n| n.starts_with("temp") && n.ends_with("_input"))
                    .unwrap_or(false)
            })
            .collect();
        inputs.sort();

        for inp in inputs {
            let Ok(val_s) = fs::read_to_string(&inp) else {
                continue;
            };
            let Ok(milli) = val_s.trim().parse::<i64>() else {
                continue;
            };
            let celsius = milli as f64 / 1000.0;
            if !(-20.0..=140.0).contains(&celsius) {
                continue;
            }
            let label = inp
                .file_name()
                .and_then(|n| n.to_str())
                .map(|n| n.replace("_input", "_label"))
                .and_then(|ln| fs::read_to_string(dev.join(ln)).ok())
                .map(|s| s.trim().to_ascii_lowercase())
                .unwrap_or_default();

            if matches!(name, "coretemp" | "k10temp" | "zenpower")
                && (label.contains("package") || label.contains("tctl") || label.contains("tdie"))
            {
                package_temps.push(celsius);
            } else {
                fallback_temps.push(celsius);
            }
        }
    }

    let chosen = if !package_temps.is_empty() {
        package_temps.into_iter().reduce(f64::max)
    } else {
        fallback_temps.into_iter().reduce(f64::max)
    };
    chosen.map(|t| (t * 10.0).round() / 10.0)
}

#[derive(Debug, Clone, PartialEq, Eq, Serialize, Deserialize)]
pub struct EcWatercoolerState {
    pub wmi_available: bool,
    pub wmi_seq: Option<i64>,
    pub wmi_c1_seq: Option<i64>,
    pub ec_available: bool,
    pub ec_stale: bool,
    pub lc_supported: Option<bool>,
    pub lc_fan_table_supported: Option<bool>,
    pub lc_hwoc_supported: Option<bool>,
    pub lc_ec_enabled: Option<bool>,
    pub ec_fan_enabled: bool,
    pub ec_fan_duty: i64,
    pub ec_pump_enabled: bool,
    pub ec_pump_volt: i64,
}

impl Default for EcWatercoolerState {
    fn default() -> Self {
        Self {
            wmi_available: false,
            wmi_seq: None,
            wmi_c1_seq: None,
            ec_available: false,
            ec_stale: true,
            lc_supported: None,
            lc_fan_table_supported: None,
            lc_hwoc_supported: None,
            lc_ec_enabled: None,
            ec_fan_enabled: false,
            ec_fan_duty: 0,
            ec_pump_enabled: false,
            ec_pump_volt: 0,
        }
    }
}

pub fn read_ec_watercooler_state(
    wmi_path: &Path,
    ec_path: &Path,
    max_age_ms: i64,
) -> EcWatercoolerState {
    let mut res = EcWatercoolerState::default();

    if let Ok(txt) = fs::read_to_string(wmi_path) {
        if let Ok(Value::Object(wmi)) = serde_json::from_str::<Value>(&txt) {
            res.wmi_available = true;
            res.wmi_seq = wmi.get("wmi_seq").and_then(Value::as_i64);
            if let Some(events) = wmi.get("wmi_events").and_then(Value::as_array) {
                res.wmi_c1_seq = events
                    .iter()
                    .filter_map(|e| e.as_object())
                    .filter(|e| e.get("code").and_then(Value::as_i64) == Some(193))
                    .filter_map(|e| e.get("seq").and_then(Value::as_i64))
                    .max();
            }
        }
    }

    if let Ok(txt) = fs::read_to_string(ec_path) {
        if let Ok(Value::Object(ec)) = serde_json::from_str::<Value>(&txt) {
            if ec.get("abi").and_then(Value::as_i64) == Some(1) {
                res.ec_available = true;
                let now_ms = monotonic_ms();
                if let Some(updated) = ec.get("updated_monotonic_ms").and_then(Value::as_i64) {
                    let age = now_ms - updated;
                    if (0..=max_age_ms).contains(&age) {
                        res.ec_stale = false;
                    }
                }
                if let Some(b) = ec.get("lc_supported").and_then(Value::as_bool) {
                    res.lc_supported = Some(b);
                }
                if let Some(b) = ec.get("lc_fan_table_supported").and_then(Value::as_bool) {
                    res.lc_fan_table_supported = Some(b);
                }
                if let Some(b) = ec.get("lc_hwoc_supported").and_then(Value::as_bool) {
                    res.lc_hwoc_supported = Some(b);
                }
                if let Some(b) = ec.get("lc_ec_enabled").and_then(Value::as_bool) {
                    res.lc_ec_enabled = Some(b);
                }
                if let Some(b) = ec.get("ec_fan_enabled").and_then(Value::as_bool) {
                    res.ec_fan_enabled = b;
                }
                if let Some(n) = ec.get("ec_fan_duty").and_then(Value::as_i64) {
                    res.ec_fan_duty = n;
                }
                if let Some(b) = ec.get("ec_pump_enabled").and_then(Value::as_bool) {
                    res.ec_pump_enabled = b;
                }
                if let Some(n) = ec.get("ec_pump_volt").and_then(Value::as_i64) {
                    res.ec_pump_volt = n;
                }
            }
        }
    }

    res
}

// --------------------------------------------------- Target & safety evaluators

#[derive(Debug, Clone)]
pub struct FlowSafetyMonitor {
    pub grace_s: f64,
    pub fault_hold_s: f64,
    pub tripped: bool,
    pub trip_reason: String,
    pump_on_since: Option<f64>,
    flow_confirmed: bool,
    bad_since: Option<f64>,
}

impl Default for FlowSafetyMonitor {
    fn default() -> Self {
        Self::new(5.0, 3.0)
    }
}

impl FlowSafetyMonitor {
    pub fn new(grace_s: f64, fault_hold_s: f64) -> Self {
        Self {
            grace_s,
            fault_hold_s,
            tripped: false,
            trip_reason: String::new(),
            pump_on_since: None,
            flow_confirmed: false,
            bad_since: None,
        }
    }

    pub fn reset(&mut self) {
        self.tripped = false;
        self.trip_reason.clear();
        self.on_disconnect();
    }

    pub fn on_disconnect(&mut self) {
        self.pump_on_since = None;
        self.flow_confirmed = false;
        self.bad_since = None;
    }

    pub fn update(
        &mut self,
        now: f64,
        pump_volt: i64,
        meter_state: Option<u8>,
        flow_protection: bool,
    ) -> bool {
        if self.tripped {
            return true;
        }
        if !flow_protection || pump_volt <= 0 {
            self.on_disconnect();
            return false;
        }
        let Some(since) = self.pump_on_since else {
            self.pump_on_since = Some(now);
            self.flow_confirmed = meter_state == Some(2);
            self.bad_since = None;
            return false;
        };

        if meter_state == Some(2) {
            self.flow_confirmed = true;
            self.bad_since = None;
            return false;
        }

        if !self.flow_confirmed {
            if now - since >= self.grace_s {
                self.tripped = true;
                self.trip_reason = format!(
                    "水泵开启超过 {:.0}s 未检测到水流 (meter_state={:?})",
                    self.grace_s, meter_state
                );
                return true;
            }
            return false;
        }

        let Some(bad) = self.bad_since else {
            self.bad_since = Some(now);
            return false;
        };
        if now - bad >= self.fault_hold_s {
            self.tripped = true;
            self.trip_reason = format!(
                "水泵运行中水流信号丢失超过 {:.0}s (meter_state={:?})",
                self.fault_hold_s, meter_state
            );
            return true;
        }
        false
    }
}

pub fn evaluate_cooling_target(
    cfg: &CoolerConfig,
    temp_c: Option<f64>,
    ec_state: Option<&EcWatercoolerState>,
    current_tier: usize,
    flow_tripped: bool,
    sleeping: bool,
) -> (i64, i64, i64, usize, String) {
    if sleeping {
        return (0, 0, 0, current_tier, "sleeping".to_string());
    }
    if flow_tripped {
        return (0, 0, 0, current_tier, "flow_fault_locked".to_string());
    }

    match cfg.strategy.as_str() {
        "manual" => (
            cfg.manual.pump_volt,
            cfg.manual.pump_duty,
            cfg.manual.fan_duty,
            current_tier,
            "manual".to_string(),
        ),
        "preset" => {
            let p = preset_stage(&cfg.preset).unwrap_or(StageConfig {
                pump_volt: 7,
                pump_duty: 60,
                fan_duty: 40,
            });
            (
                p.pump_volt,
                p.pump_duty,
                p.fan_duty,
                current_tier,
                format!("preset:{}", cfg.preset),
            )
        }
        "ec_follow" => {
            if let Some(ec) = ec_state {
                if ec.ec_available && !ec.ec_stale {
                    let pv = if ec.ec_pump_enabled { ec.ec_pump_volt } else { 0 };
                    let fd = if ec.ec_fan_enabled { ec.ec_fan_duty } else { 0 };
                    if VALID_PUMP_VOLTS.contains(&pv) && (0..=100).contains(&fd) {
                        let pd = if pv > 0 { 60 } else { 0 };
                        return (pv, pd, fd, current_tier, "ec_follow".to_string());
                    }
                }
            }
            let p = preset_stage("silent").unwrap();
            (
                p.pump_volt,
                p.pump_duty,
                p.fan_duty,
                current_tier,
                "ec_fallback_silent".to_string(),
            )
        }
        _ => {
            let points = &cfg.curve.points;
            let hyst = cfg.curve.hysteresis_c as f64;
            let mut idx = current_tier.min(points.len().saturating_sub(1));
            let Some(temp) = temp_c else {
                let pt = &points[0];
                return (
                    pt.pump_volt,
                    pt.pump_duty,
                    pt.fan_duty,
                    0,
                    "curve_no_sensor_safe_floor".to_string(),
                );
            };

            for i in (0..points.len()).rev() {
                if temp >= points[i].temp_c as f64 {
                    if i >= idx {
                        idx = i;
                    }
                    break;
                }
            }
            while idx > 0 && temp <= (points[idx].temp_c as f64 - hyst) {
                idx -= 1;
            }
            let pt = &points[idx];
            (
                pt.pump_volt,
                pt.pump_duty,
                pt.fan_duty,
                idx,
                format!("smart_curve:{temp:.1}C->tier{idx}"),
            )
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use serde_json::json;

    fn to_hex(bytes: &[u8]) -> String {
        bytes.iter().map(|b| format!("{b:02x}")).collect()
    }

    #[test]
    fn test_pump_fan_and_led_encoding_matches_python() {
        assert_eq!(encode_handshake(), b"sw");
        assert_eq!(to_hex(&encode_sleep()), "fe190001000000ef");
        assert_eq!(to_hex(&encode_meter_query()), "fe320000000000ef");
        assert_eq!(to_hex(&encode_fan(0).unwrap()), "fe1b0000000000ef");
        assert_eq!(to_hex(&encode_fan(40).unwrap()), "fe1b0128000000ef");

        for (volt, expected) in [
            (0, "fe1c0000000000ef"),
            (7, "fe1c013c020000ef"),
            (8, "fe1c013c030000ef"),
            (11, "fe1c013c000000ef"),
            (12, "fe1c013c010000ef"),
        ] {
            assert_eq!(to_hex(&encode_pump(volt, 60).unwrap()), expected);
        }
        assert!(encode_pump(9, 60).is_err());

        let split = encode_led_plan(&LedConfig {
            mode: "breathing_colorful".to_string(),
            color: [0, 0, 255],
            fan_policy: "split".to_string(),
            fan_color: [255, 0, 128],
        })
        .unwrap();
        let hexes: Vec<String> = split.iter().map(|(f, _)| to_hex(f)).collect();
        assert_eq!(
            hexes,
            vec![
                "fe330000000000ef",
                "fe1e01ff008000ef",
                "fe330100000000ef",
                "fe1e010000ff03ef"
            ]
        );
    }

    #[test]
    fn test_rx_parser() {
        assert_eq!(
            parse_rx_packet(&[0xFE, 0x31, 0x05, 0x02, 0xEF]),
            RxPacket::Meter {
                opcode: 0x31,
                byte2: 0x05,
                meter_state: 2
            }
        );
        assert_eq!(
            parse_rx_packet(b"MCU F/W Version: 2.0.0.4\r\n"),
            RxPacket::Firmware {
                text: "MCU F/W Version: 2.0.0.4".to_string(),
                fw_version: Some("2.0.0.4".to_string())
            }
        );
        assert_eq!(parse_rx_packet(&[0xFF, 0x80, 0x01]), RxPacket::Unknown);
    }

    #[test]
    fn test_config_validation_and_curve_hysteresis() {
        assert!(validate_config_value(&json!({"auto_connect": 1})).is_err());
        assert!(validate_config_value(&json!({"unknown": 1})).is_err());
        assert!(validate_config_value(&json!({"mac": "not-a-mac"})).is_err());
        let ok = validate_config_value(&json!({"mac": "e2:90:b5:f8:43:2e", "strategy": "smart_curve"})).unwrap();
        assert_eq!(ok.mac, "E2:90:B5:F8:43:2E");

        let (pv, pd, fd, tier, _) = evaluate_cooling_target(&ok, Some(66.0), None, 0, false, false);
        assert_eq!((tier, pv, pd, fd), (1, 8, 70, 55));
        let (_, _, _, tier2, _) = evaluate_cooling_target(&ok, Some(63.0), None, tier, false, false);
        assert_eq!(tier2, 1);
        let (pv3, _, fd3, tier3, _) = evaluate_cooling_target(&ok, Some(62.0), None, tier2, false, false);
        assert_eq!((tier3, pv3, fd3), (0, 7, 40));
    }

    #[test]
    fn test_flow_safety_monitor() {
        let mut mon = FlowSafetyMonitor::new(5.0, 3.0);
        assert!(!mon.update(0.0, 7, Some(1), true));
        assert!(!mon.update(4.9, 7, Some(1), true));
        assert!(mon.update(5.0, 7, Some(1), true));
        assert!(mon.tripped);
        mon.reset();
        assert!(!mon.tripped);
        assert!(!mon.update(10.0, 7, Some(1), true));
        assert!(!mon.update(11.0, 7, Some(2), true));
        assert!(!mon.update(12.0, 7, Some(1), true));
        assert!(mon.update(15.0, 7, Some(1), true));
    }
}

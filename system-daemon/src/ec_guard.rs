// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Root EC recovery guard, WMI event dispatcher (0xB0 mode key, 0xBB thermal safety,
//! 0xC1 hotkey), and read-only water-cooling EC telemetry publisher, wire-compatible
//! with `system/mechrevo-profile-helper`.

use crate::common::{
    atomic_write_json, check_machine, monotonic_ms, turbo_socket_request, LockError, RootFlock,
    PROFILE_LOCK, TURBO_DEVICE, TURBO_SOCKET,
};
use serde_json::{json, Map, Value};
use std::collections::{BTreeMap, BTreeSet};
use std::ffi::CString;
use std::fs::{self, File};
use std::io::Read;
use std::os::unix::ffi::OsStrExt;
use std::os::unix::fs::{FileTypeExt, MetadataExt};
use std::os::unix::io::{AsRawFd, FromRawFd, RawFd};
use std::path::{Path, PathBuf};
use std::sync::atomic::{AtomicBool, AtomicI32, Ordering};
use std::thread;
use std::time::{Duration, Instant};

pub const EC_BASE: u64 = 0xFE41_0000;
pub const REG_MODE: u16 = 0x751;
pub const WMI_STATE_PATH: &str = "/run/mechrevo-wmi-state.json";
pub const LC_EC_STATE_PATH: &str = "/run/mechrevo-watercooler-ec.json";
pub const TUNING_STATE_PATH: &str = "/run/mechrevo-tuning.json";
pub const TURBO_JOURNAL_PATH: &str = "/run/mechrevo-turbo.json";
pub const SLEEP_MARKER_PATH: &str = "/run/mechrevo-turbo-sleep.json";

pub const WMI_SCANCODE_MODE_SWITCH: i64 = 0xB0;
pub const WMI_SCANCODE_SAFETY_TEMP: i64 = 0xBB;
pub const WMI_SCANCODE_HOTKEY_C1: i64 = 0xC1;

pub const REGISTERS: &[u16] = &[
    REG_MODE, 0x490, 0x706, 0x726, 0x727, 0x728, 0x741, 0x743, 0x744, 0x745, 0x746, 0x766, 0x783,
    0x784, 0x785, 0x786, 0x787, 0x7A4, 0x7A5, 0x7AB, 0x7AC, 0x7C5, 0x7C6, 0x7C7, 0x7C8, 0x7CC,
    0x7D2, 0x7E6, 0x7E7, 0x7E8, 0xD48,
];

pub(crate) const BASE_OWN_MASKS: &[(u16, u8)] = &[
    (0x726, 128),
    (0x727, 64),
    (REG_MODE, 255),
    (0x741, 1),
    (0x783, 255),
    (0x784, 255),
    (0x7C5, 128),
    (0x7C6, 4),
];

pub(crate) const GPU_OWN_MASKS: &[(u16, u8)] = &[
    (0x743, 7),
    (0x744, 255),
    (0x745, 255),
    (0x746, 255),
];

pub(crate) const LOGO_MASKS: &[(u16, u8)] = &[
    (0x767, 0x80),
    (0x769, 0xFF),
    (0x76A, 0xFF),
    (0x76B, 0xFF),
    (0x7AB, 0xE0),
    (0x7C5, 0x07),
];

pub(crate) const MIN_DUTY: [i64; 11] = [40, 40, 40, 40, 50, 55, 60, 70, 80, 90, 100];

static GUARD_STOP: AtomicBool = AtomicBool::new(false);
static WAKE_WRITER_FD: AtomicI32 = AtomicI32::new(-1);

extern "C" fn handle_guard_signal(_sig: libc::c_int) {
    GUARD_STOP.store(true, Ordering::SeqCst);
    let fd = WAKE_WRITER_FD.load(Ordering::SeqCst);
    if fd >= 0 {
        let byte: u8 = 0;
        unsafe {
            let _ = libc::write(fd, &byte as *const u8 as *const libc::c_void, 1);
        }
    }
}

fn install_guard_signals() {
    unsafe {
        let mut sa: libc::sigaction = std::mem::zeroed();
        sa.sa_sigaction = handle_guard_signal as *const () as usize;
        libc::sigemptyset(&mut sa.sa_mask);
        sa.sa_flags = 0;
        libc::sigaction(libc::SIGTERM, &sa, std::ptr::null_mut());
        libc::sigaction(libc::SIGINT, &sa, std::ptr::null_mut());
    }
}

fn flag_mask(addr: u16) -> Option<u8> {
    match addr {
        0x726 => Some(0x80),
        0x727 => Some(0x40),
        _ => None,
    }
}

fn is_own_mask_addr(addr: u16) -> bool {
    BASE_OWN_MASKS.iter().any(|(a, _)| *a == addr)
        || GPU_OWN_MASKS.iter().any(|(a, _)| *a == addr)
}

fn mode_byte(name: &str) -> Option<u8> {
    match name {
        "office" => Some(0xA0),
        "balanced" => Some(0x20),
        "turbo" => Some(0x30),
        "custom" => Some(0x00),
        _ => None,
    }
}

fn owned_group(addr: u16) -> &'static str {
    match addr {
        REG_MODE | 0x726 | 0x727 => "mode",
        0x743 | 0x744 | 0x745 | 0x746 => "gpu",
        0x783 | 0x784 => "cpu",
        0x741 => "appe",
        _ => "fan",
    }
}

// ----------------------------------------------------------------- EC I/O trait

pub(crate) fn logo_mask(addr: u16) -> Option<u8> {
    if addr == 0x78C {
        return Some(0x10);
    }
    LOGO_MASKS.iter().find(|(a, _)| *a == addr).map(|(_, m)| *m)
}

pub trait EcIo {
    fn read(&self, addr: u16) -> Result<u8, String>;
    fn write_mode(&mut self, value: u8) -> Result<(), String>;
    fn write_flag(&mut self, addr: u16, enabled: bool) -> Result<(), String>;
    fn write_tuning(&mut self, addr: u16, value: u8) -> Result<(), String>;
    fn write_logo(&mut self, addr: u16, value: u8) -> Result<(), String>;
}

pub struct RealEc {
    file: File,
}

impl RealEc {
    pub fn open(writable: bool) -> Result<Self, String> {
        let c_path = CString::new("/dev/mem").unwrap();
        let flags = if writable {
            libc::O_RDWR | libc::O_CLOEXEC
        } else {
            libc::O_RDONLY | libc::O_CLOEXEC
        };
        let fd = unsafe { libc::open(c_path.as_ptr(), flags) };
        if fd < 0 {
            return Err(format!(
                "打开 /dev/mem 失败: {}",
                std::io::Error::last_os_error()
            ));
        }
        Ok(Self {
            file: unsafe { File::from_raw_fd(fd) },
        })
    }
}

impl EcIo for RealEc {
    fn read(&self, addr: u16) -> Result<u8, String> {
        let allowed = REGISTERS.contains(&addr)
            || matches!(addr, 0x767 | 0x769 | 0x76A | 0x76B | 0x78C | 0x7AB | 0x7C5)
            || (0xF00..=0xF5F).contains(&addr);
        if !allowed {
            return Err("Unsupported register".to_string());
        }
        let mut b: u8 = 0;
        let offset = (EC_BASE + addr as u64) as libc::off_t;
        let n = unsafe {
            libc::pread(
                self.file.as_raw_fd(),
                &mut b as *mut u8 as *mut libc::c_void,
                1,
                offset,
            )
        };
        if n != 1 {
            return Err("EC 读取不完整".to_string());
        }
        Ok(b)
    }

    fn write_mode(&mut self, value: u8) -> Result<(), String> {
        if !matches!(value, 0xA0 | 0x20 | 0x30 | 0x00 | 0x10) {
            return Err("Unsupported mode value".to_string());
        }
        let offset = (EC_BASE + REG_MODE as u64) as libc::off_t;
        let n = unsafe {
            libc::pwrite(
                self.file.as_raw_fd(),
                &value as *const u8 as *const libc::c_void,
                1,
                offset,
            )
        };
        if n != 1 {
            return Err("EC 写入不完整".to_string());
        }
        Ok(())
    }

    fn write_flag(&mut self, addr: u16, enabled: bool) -> Result<(), String> {
        let Some(mask) = flag_mask(addr) else {
            return Err("Unsupported flag".to_string());
        };
        let cur = self.read(addr)?;
        let value = (cur & !mask) | if enabled { mask } else { 0 };
        let offset = (EC_BASE + addr as u64) as libc::off_t;
        let n = unsafe {
            libc::pwrite(
                self.file.as_raw_fd(),
                &value as *const u8 as *const libc::c_void,
                1,
                offset,
            )
        };
        if n != 1 {
            return Err("EC 标志写入不完整".to_string());
        }
        Ok(())
    }

    fn write_tuning(&mut self, addr: u16, value: u8) -> Result<(), String> {
        if !is_own_mask_addr(addr) && !(0xF00..=0xF5F).contains(&addr) {
            return Err("Unsupported tuning register".to_string());
        }
        let offset = (EC_BASE + addr as u64) as libc::off_t;
        let n = unsafe {
            libc::pwrite(
                self.file.as_raw_fd(),
                &value as *const u8 as *const libc::c_void,
                1,
                offset,
            )
        };
        if n != 1 {
            return Err("EC 写入不完整".to_string());
        }
        Ok(())
    }

    fn write_logo(&mut self, addr: u16, value: u8) -> Result<(), String> {
        let Some(mask) = logo_mask(addr) else {
            return Err("Unsupported logo register".to_string());
        };
        if (value & !mask) != 0 {
            return Err("Invalid logo bits".to_string());
        }
        let cur = self.read(addr)?;
        let merged = (cur & !mask) | value;
        let offset = (EC_BASE + addr as u64) as libc::off_t;
        let n = unsafe {
            libc::pwrite(
                self.file.as_raw_fd(),
                &merged as *const u8 as *const libc::c_void,
                1,
                offset,
            )
        };
        if n != 1 {
            return Err("Logo 写入不完整".to_string());
        }
        Ok(())
    }
}

// ----------------------------------------------------------------- EC state & telemetry

pub fn snapshot<E: EcIo>(ec: &E) -> Result<BTreeMap<u16, u8>, String> {
    let mut vals = BTreeMap::new();
    for &addr in REGISTERS {
        vals.insert(addr, ec.read(addr)?);
    }
    Ok(vals)
}

pub fn standard_state(vals: &BTreeMap<u16, u8>) -> bool {
    let get = |a: u16| *vals.get(&a).unwrap_or(&0);
    let common = get(0x706) == 0x40
        && (get(0x741) & 1) == 0
        && (get(0x7C5) & 0x80) == 0
        && (get(0x7C6) & 4) == 0
        && [0x783u16, 0x784, 0x785, 0x786, 0x787]
            .iter()
            .all(|&a| get(a) == 0);
    let raw_mode = get(REG_MODE);
    let regular = matches!(raw_mode, 0xA0 | 0x00 | 0x10 | 0x20 | 0x30)
        && (get(0x726) & 0x80) == 0
        && (get(0x727) & 0x40) == 0;
    let custom = raw_mode == 0 && (get(0x726) & 0x80) != 0 && (get(0x727) & 0x40) != 0;
    common && (regular || custom)
}

pub fn mode_name(vals: &BTreeMap<u16, u8>) -> &'static str {
    let get = |a: u16| *vals.get(&a).unwrap_or(&0);
    let raw = get(REG_MODE);
    if raw == 0 && (get(0x726) & 0x80) != 0 && (get(0x727) & 0x40) != 0 {
        return "custom";
    }
    match raw {
        0x10 | 0x30 => "turbo",
        0x00 | 0x20 => "balanced",
        0xA0 => "office",
        _ => "unknown",
    }
}

pub fn fan_safety_protect_ec<E: EcIo>(ec: &E) -> Result<bool, String> {
    Ok((ec.read(0x7A5)? & 0x08) != 0)
}

pub fn gpu_round_adapter<E: EcIo>(ec: &E) -> Result<bool, String> {
    let flags = ec.read(0x7CC)?;
    Ok((flags & 1) != 0 || ((flags & 15) == 0 && (ec.read(0x490)? & 1) != 0))
}

pub fn unmanaged_gpu_clean<E: EcIo>(ec: &E) -> Result<bool, String> {
    for &(addr, mask) in GPU_OWN_MASKS {
        if (ec.read(addr)? & mask) != 0 {
            return Ok(false);
        }
    }
    Ok(true)
}

pub fn watercooling_status(vals: &BTreeMap<u16, u8>) -> Value {
    let get = |a: u16| *vals.get(&a).unwrap_or(&0);
    let v7a4 = get(0x7A4);
    let v7c7 = get(0x7C7);
    let v7c8 = get(0x7C8);
    let v7e6 = get(0x7E6);
    let v7e7 = get(0x7E7);
    let vd48 = get(0xD48);
    json!({
        "abi": 1,
        "lc_supported": (v7a4 & 0x80) != 0,
        "lc_fan_table_supported": (v7c8 & 0x20) != 0,
        "lc_hwoc_supported": vd48 != 0xFF && (vd48 & 0x04) != 0,
        "lc_ec_enabled": (v7c7 & 0x02) != 0,
        "ec_c2_index": (v7c7 >> 2) & 0x07,
        "ec_fan_enabled": (v7e6 & 0x80) != 0,
        "ec_fan_duty": (v7e6 & 0x7F).min(100),
        "ec_pump_enabled": (v7e7 & 0x80) != 0,
        "ec_pump_volt": v7e7 & 0x7F,
        "raw": {
            "0x7a4": v7a4,
            "0x7c7": v7c7,
            "0x7c8": v7c8,
            "0x7e6": v7e6,
            "0x7e7": v7e7,
            "0xd48": vd48,
        },
        "updated_monotonic_ms": monotonic_ms(),
    })
}

pub fn sanitize_wmi_state(bridge: &Value) -> Value {
    let mut events = Vec::new();
    if let Some(arr) = bridge.get("wmi_events").and_then(|v| v.as_array()) {
        for item in arr {
            if let Some(obj) = item.as_object() {
                if let (Some(seq), Some(code), Some(ms)) = (
                    obj.get("seq").and_then(|v| v.as_i64()),
                    obj.get("code").and_then(|v| v.as_i64()),
                    obj.get("ms").and_then(|v| v.as_i64()),
                ) {
                    events.push(json!({"seq": seq, "code": code, "ms": ms}));
                }
            }
        }
    }
    let tail = if events.len() > 16 {
        events[events.len() - 16..].to_vec()
    } else {
        events
    };
    json!({
        "abi": 1,
        "wmi_bound": bridge.get("wmi_bound").and_then(|v| v.as_bool()).unwrap_or(false),
        "wmi_seq": bridge.get("wmi_seq").and_then(|v| v.as_i64()).unwrap_or(0),
        "wmi_last_code": bridge.get("wmi_last_code").cloned().unwrap_or(Value::Null),
        "wmi_events": tail,
        "fan_safety_protect": bridge.get("fan_safety_protect").and_then(|v| v.as_bool()).unwrap_or(false),
        "smart_balance_supported": bridge.get("smart_balance_supported").and_then(|v| v.as_bool()).unwrap_or(false),
        "smart_balance_enabled": bridge.get("smart_balance_enabled").and_then(|v| v.as_bool()).unwrap_or(false),
        "updated_monotonic_ms": monotonic_ms(),
    })
}

// ----------------------------------------------------------------- Tuning journal v1/v2

#[derive(Debug, Clone)]
pub struct PendingWrite {
    pub address: u16,
    pub value: u8,
}

#[derive(Debug, Clone)]
pub struct TuningState {
    pub version: i64,
    pub phase: String,
    pub config: Value,
    pub has_gpu: bool,
    pub before: BTreeMap<u16, u8>,
    pub target: BTreeMap<u16, u8>,
    pub confirmed: BTreeMap<u16, u8>,
    pub pending: Option<PendingWrite>,
    pub relinquished: Vec<String>,
    pub legacy_ambiguous: bool,
}

impl TuningState {
    pub(crate) fn masks(&self) -> BTreeMap<u16, u8> {
        let mut m = BTreeMap::new();
        for &(a, mask) in BASE_OWN_MASKS {
            m.insert(a, mask);
        }
        if self.has_gpu {
            for &(a, mask) in GPU_OWN_MASKS {
                m.insert(a, mask);
            }
        }
        for a in 0xF00u16..=0xF5F {
            m.insert(a, 255);
        }
        m
    }

    fn to_json(&self) -> Value {
        let map_u16 = |src: &BTreeMap<u16, u8>| -> Value {
            let mut obj = Map::new();
            for (k, v) in src {
                obj.insert(k.to_string(), json!(*v));
            }
            Value::Object(obj)
        };
        let pending_val = match &self.pending {
            None => Value::Null,
            Some(p) => json!({"address": p.address, "value": p.value}),
        };
        json!({
            "version": self.version,
            "phase": self.phase,
            "config": self.config,
            "before": map_u16(&self.before),
            "target": map_u16(&self.target),
            "confirmed": map_u16(&self.confirmed),
            "pending": pending_val,
            "relinquished": self.relinquished,
            "legacy_ambiguous": self.legacy_ambiguous,
        })
    }
}

pub(crate) fn validate_tuning_config(config: &Value) -> Result<(), String> {
    let Some(obj) = config.as_object() else {
        return Err("自定义设置字段不完整".to_string());
    };
    let required = ["pl1", "pl2", "cpu_fan", "gpu_fan"];
    if required.iter().any(|k| !obj.contains_key(*k)) {
        return Err("自定义设置字段不完整".to_string());
    }
    let has_ctgp = obj.contains_key("gpu_ctgp");
    let has_db = obj.contains_key("gpu_db");
    if (has_db && !has_ctgp) || obj.len() != 4 + usize::from(has_ctgp) + usize::from(has_db) {
        return Err("自定义设置字段不完整".to_string());
    }
    if has_ctgp {
        let Some(v) = obj["gpu_ctgp"].as_i64() else {
            return Err("已验证 GPU cTGP 范围为65–115 W".to_string());
        };
        if !(65..=115).contains(&v) {
            return Err("已验证 GPU cTGP 范围为65–115 W".to_string());
        }
    }
    if has_db {
        let Some(v) = obj["gpu_db"].as_i64() else {
            return Err("Dynamic Boost仅支持关闭或5–25 W，不接受自定义TPP".to_string());
        };
        if v != 0 && !(5..=25).contains(&v) {
            return Err("Dynamic Boost仅支持关闭或5–25 W，不接受自定义TPP".to_string());
        }
    }
    let (Some(pl1), Some(pl2)) = (obj["pl1"].as_i64(), obj["pl2"].as_i64()) else {
        return Err("已验证 CPU 范围为 25–75 W，持续与短时上限需保持一致".to_string());
    };
    if !(25..=75).contains(&pl1) || pl1 != pl2 {
        return Err("已验证 CPU 范围为 25–75 W，持续与短时上限需保持一致".to_string());
    }
    for name in ["cpu_fan", "gpu_fan"] {
        let Some(arr) = obj[name].as_array() else {
            return Err("风扇曲线需要 11 个节点".to_string());
        };
        if arr.len() != 11 {
            return Err("风扇曲线需要 11 个节点".to_string());
        }
        let mut vals = Vec::with_capacity(11);
        for (i, item) in arr.iter().enumerate() {
            let Some(v) = item.as_i64() else {
                return Err("风扇不能低于已验证的出厂冷却基线，高温节点必须为 100%".to_string());
            };
            if v < MIN_DUTY[i] || v > 100 {
                return Err("风扇不能低于已验证的出厂冷却基线，高温节点必须为 100%".to_string());
            }
            vals.push(v);
        }
        if vals.windows(2).any(|w| w[0] > w[1]) {
            return Err("风扇转速必须随温度单调不减".to_string());
        }
    }
    Ok(())
}

fn parse_u16_byte_map(val: &Value, expected: &BTreeMap<u16, u8>) -> Result<BTreeMap<u16, u8>, String> {
    let Some(obj) = val.as_object() else {
        return Err("自定义恢复记录数据异常".to_string());
    };
    if obj.len() != expected.len() {
        return Err("自定义恢复记录数据异常".to_string());
    }
    let mut out = BTreeMap::new();
    for (k, v) in obj {
        let addr: u16 = k.parse().map_err(|_| "自定义恢复记录数据异常".to_string())?;
        if !expected.contains_key(&addr) {
            return Err("自定义恢复记录数据异常".to_string());
        }
        let Some(byte) = v.as_i64() else {
            return Err("自定义恢复记录数据异常".to_string());
        };
        if !(0..=255).contains(&byte) {
            return Err("自定义恢复记录数据异常".to_string());
        }
        out.insert(addr, byte as u8);
    }
    Ok(out)
}

pub fn read_tuning_state(path: &Path) -> Result<Option<TuningState>, String> {
    let c_path = CString::new(path.as_os_str().as_bytes())
        .map_err(|_| "自定义恢复记录路径异常".to_string())?;
    let fd = unsafe {
        libc::open(
            c_path.as_ptr(),
            libc::O_RDONLY | libc::O_NOFOLLOW | libc::O_CLOEXEC,
        )
    };
    if fd < 0 {
        let err = std::io::Error::last_os_error();
        if err.raw_os_error() == Some(libc::ENOENT) {
            return Ok(None);
        }
        return Err(format!("打开恢复记录失败: {err}"));
    }
    let mut file = unsafe { File::from_raw_fd(fd) };
    let meta = file
        .metadata()
        .map_err(|e| format!("stat 恢复记录失败: {e}"))?;
    let euid = unsafe { libc::geteuid() };
    if !meta.is_file() || meta.uid() != euid || (meta.mode() & 0o077) != 0 {
        return Err("自定义恢复记录权限异常".to_string());
    }
    let mut raw = Vec::new();
    (&mut file)
        .take(32_769)
        .read_to_end(&mut raw)
        .map_err(|e| format!("读取恢复记录失败: {e}"))?;
    if raw.len() > 32_768 {
        return Err("自定义恢复记录过长".to_string());
    }
    let data: Value =
        serde_json::from_slice(&raw).map_err(|_| "自定义恢复记录格式异常".to_string())?;
    let Some(obj) = data.as_object() else {
        return Err("自定义恢复记录格式异常".to_string());
    };
    let Some(version) = obj.get("version").and_then(|v| v.as_i64()) else {
        return Err("自定义恢复记录格式异常".to_string());
    };
    let Some(phase) = obj.get("phase").and_then(|v| v.as_str()) else {
        return Err("自定义恢复记录格式异常".to_string());
    };
    if !matches!(version, 1 | 2) || !matches!(phase, "preparing" | "active" | "restoring") {
        return Err("自定义恢复记录格式异常".to_string());
    }
    let config = obj
        .get("config")
        .ok_or_else(|| "自定义恢复记录格式异常".to_string())?
        .clone();
    validate_tuning_config(&config)?;
    let has_gpu = config
        .as_object()
        .map(|m| m.contains_key("gpu_ctgp"))
        .unwrap_or(false);
    if version == 1 && (has_gpu || phase == "restoring") {
        return Err("旧恢复记录不能声明GPU所有权".to_string());
    }
    let dummy = TuningState {
        version,
        phase: phase.to_string(),
        config: config.clone(),
        has_gpu,
        before: BTreeMap::new(),
        target: BTreeMap::new(),
        confirmed: BTreeMap::new(),
        pending: None,
        relinquished: Vec::new(),
        legacy_ambiguous: false,
    };
    let masks = dummy.masks();
    let before = parse_u16_byte_map(
        obj.get("before").ok_or("自定义恢复记录数据异常")?,
        &masks,
    )?;
    let target = parse_u16_byte_map(
        obj.get("target").ok_or("自定义恢复记录数据异常")?,
        &masks,
    )?;

    if version == 2 {
        const V2_KEYS: &[&str] = &[
            "version",
            "phase",
            "config",
            "before",
            "target",
            "confirmed",
            "pending",
            "relinquished",
            "legacy_ambiguous",
        ];
        if obj.len() != V2_KEYS.len() || V2_KEYS.iter().any(|k| !obj.contains_key(*k)) {
            return Err("新恢复记录字段异常".to_string());
        }
        let confirmed = parse_u16_byte_map(&obj["confirmed"], &masks)?;
        for (&a, &v) in &confirmed {
            if (v & !masks[&a]) != 0 {
                return Err("新恢复记录所有权异常".to_string());
            }
        }
        let Some(legacy_ambiguous) = obj["legacy_ambiguous"].as_bool() else {
            return Err("新恢复记录所有权异常".to_string());
        };
        let Some(rel_arr) = obj["relinquished"].as_array() else {
            return Err("新恢复记录所有权异常".to_string());
        };
        let mut relinquished = Vec::with_capacity(rel_arr.len());
        for g in rel_arr {
            let Some(gs) = g.as_str() else {
                return Err("新恢复记录所有权异常".to_string());
            };
            if !matches!(gs, "mode" | "gpu" | "cpu" | "appe" | "fan") {
                return Err("新恢复记录所有权异常".to_string());
            }
            relinquished.push(gs.to_string());
        }
        let pending = if obj["pending"].is_null() {
            None
        } else {
            let Some(pobj) = obj["pending"].as_object() else {
                return Err("新恢复记录pending异常".to_string());
            };
            if pobj.len() != 2 || !pobj.contains_key("address") || !pobj.contains_key("value") {
                return Err("新恢复记录pending异常".to_string());
            }
            let Some(addr_i) = pobj["address"].as_i64() else {
                return Err("新恢复记录pending异常".to_string());
            };
            let Ok(addr) = u16::try_from(addr_i) else {
                return Err("新恢复记录pending异常".to_string());
            };
            let Some(&mask) = masks.get(&addr) else {
                return Err("新恢复记录pending异常".to_string());
            };
            let Some(val_i) = pobj["value"].as_i64() else {
                return Err("新恢复记录pending异常".to_string());
            };
            if !(0..=255).contains(&val_i) || ((val_i as u8) & !mask) != 0 {
                return Err("新恢复记录pending异常".to_string());
            }
            Some(PendingWrite {
                address: addr,
                value: val_i as u8,
            })
        };
        return Ok(Some(TuningState {
            version: 2,
            phase: phase.to_string(),
            config,
            has_gpu,
            before,
            target,
            confirmed,
            pending,
            relinquished,
            legacy_ambiguous,
        }));
    }

    Ok(Some(TuningState {
        version: 1,
        phase: phase.to_string(),
        config,
        has_gpu,
        before,
        target,
        confirmed: BTreeMap::new(),
        pending: None,
        relinquished: Vec::new(),
        legacy_ambiguous: false,
    }))
}

pub(crate) fn save_tuning_state(path: &Path, state: &TuningState) -> Result<(), String> {
    atomic_write_json(path, &state.to_json(), 0o600, ".mechrevo-", true)
}

pub fn tuning_matches<E: EcIo>(ec: &E, state: &TuningState) -> Result<bool, String> {
    let masks = state.masks();
    let gpu_context = state.has_gpu || unmanaged_gpu_clean(ec)?;
    if !gpu_context || !state.relinquished.is_empty() || state.pending.is_some() {
        return Ok(false);
    }
    for (&a, &v) in &state.target {
        let mask = masks[&a];
        if (ec.read(a)? & mask) != (v & mask) {
            return Ok(false);
        }
    }
    Ok(true)
}

fn accepted_bits(state: &TuningState, addr: u16) -> [Option<u8>; 2] {
    let c = state.confirmed.get(&addr).copied();
    let p = state
        .pending
        .as_ref()
        .filter(|pw| pw.address == addr)
        .map(|pw| pw.value);
    [c, p]
}

fn bit_allowed(val: u8, state: &TuningState, addr: u16, mask: u8, skip_fan_ram: bool) -> bool {
    let [c, p] = accepted_bits(state, addr);
    if c == Some(val) || p == Some(val) {
        return true;
    }
    if skip_fan_ram && matches!(addr, 0x741 | 0x7C5 | 0x7C6) {
        if let Some(&b) = state.before.get(&addr) {
            if val == (b & mask) {
                return true;
            }
        }
    }
    false
}

fn add_relinquished(state: &mut TuningState, groups: &BTreeSet<&str>) {
    let mut set: BTreeSet<String> = state.relinquished.iter().cloned().collect();
    for g in groups {
        set.insert((*g).to_string());
    }
    state.relinquished = set.into_iter().collect();
}

fn check_owned<E: EcIo>(
    ec: &E,
    state_path: &Path,
    state: &mut TuningState,
    allow_mode_handoff: bool,
    skip_fan_ram: bool,
) -> Result<(), String> {
    let allowed_relinq: &[&str] = if skip_fan_ram {
        &["mode", "fan", "appe"]
    } else {
        &["mode"]
    };
    if state
        .relinquished
        .iter()
        .any(|g| !allowed_relinq.contains(&g.as_str()))
    {
        return Err("检测到外来调校接管，保留恢复记录并暂停全部写入".to_string());
    }
    let mut lost: BTreeSet<&str> = BTreeSet::new();
    if !state.has_gpu && !unmanaged_gpu_clean(ec)? {
        lost.insert("gpu");
    }
    let masks = state.masks();
    for (&a, &mask) in &masks {
        if skip_fan_ram && (0xF00..=0xF5F).contains(&a) {
            continue;
        }
        let group = owned_group(a);
        if state.relinquished.iter().any(|g| g == group) {
            continue;
        }
        let val = ec.read(a)? & mask;
        if !bit_allowed(val, state, a, mask, skip_fan_ram) {
            lost.insert(group);
        }
    }
    // Recheck shared policy/mode context after the longer fan-RAM scan.
    for a in [REG_MODE, 0x726, 0x727, 0x741, 0x743, 0x744, 0x745, 0x746] {
        if let Some(&mask) = masks.get(&a) {
            let group = owned_group(a);
            if !state.relinquished.iter().any(|g| g == group) {
                let val = ec.read(a)? & mask;
                if !bit_allowed(val, state, a, mask, skip_fan_ram) {
                    lost.insert(group);
                }
            }
        }
    }
    if !state.has_gpu && !unmanaged_gpu_clean(ec)? {
        lost.insert("gpu");
    }
    if !lost.is_empty() {
        add_relinquished(state, &lost);
        save_tuning_state(state_path, state)?;
    }
    let unallowed_lost = lost.iter().any(|g| !allowed_relinq.contains(g));
    let mode_lost = state.relinquished.iter().any(|g| g == "mode");
    if unallowed_lost || (mode_lost && !allow_mode_handoff) {
        return Err("检测到外来控制变化，保留其GPU/APPE/风扇或模式状态".to_string());
    }
    Ok(())
}

fn migrate_state<E: EcIo>(
    ec: &E,
    state_path: &Path,
    state: TuningState,
) -> Result<TuningState, String> {
    if state.version == 2 {
        return Ok(state);
    }
    if !unmanaged_gpu_clean(ec)? {
        return Err("旧恢复记录无法证明外来GPU的APPE上下文，保留记录".to_string());
    }
    let masks = state.masks();
    let preparing = state.phase == "preparing";
    let mut confirmed = BTreeMap::new();
    let mut relinquished = Vec::new();
    for (&a, &mask) in &masks {
        let value = ec.read(a)? & mask;
        let target = state.target[&a] & mask;
        let before = state.before[&a] & mask;
        let ok = value == target || (preparing && value == before);
        if !ok {
            if owned_group(a) == "mode" {
                if !relinquished.iter().any(|g| g == "mode") {
                    relinquished.push("mode".to_string());
                }
            } else {
                return Err("旧恢复记录发现外来值，拒绝猜测所有权".to_string());
            }
        }
        confirmed.insert(a, if preparing { value } else { target });
    }
    let converted = TuningState {
        version: 2,
        confirmed,
        pending: None,
        relinquished,
        legacy_ambiguous: preparing,
        ..state
    };
    save_tuning_state(state_path, &converted)?;
    Ok(converted)
}

fn resolve_pending<E: EcIo>(
    ec: &E,
    state_path: &Path,
    state: &mut TuningState,
    skip_fan_ram: bool,
) -> Result<(), String> {
    let Some(pw) = state.pending.clone() else {
        return Ok(());
    };
    let a = pw.address;
    if skip_fan_ram && (0xF00..=0xF5F).contains(&a) {
        state.pending = None;
        save_tuning_state(state_path, state)?;
        return Ok(());
    }
    check_owned(ec, state_path, state, true, skip_fan_ram)?;
    let mask = state.masks()[&a];
    let value = ec.read(a)? & mask;
    let group = owned_group(a);
    if !state.relinquished.iter().any(|g| g == group)
        && !bit_allowed(value, state, a, mask, skip_fan_ram)
    {
        let mut s = BTreeSet::new();
        s.insert(group);
        add_relinquished(state, &s);
        save_tuning_state(state_path, state)?;
        return Err("pending恢复时发现外来写入".to_string());
    }
    state.confirmed.insert(a, value);
    state.pending = None;
    save_tuning_state(state_path, state)
}

pub(crate) fn tuning_put<E: EcIo>(
    ec: &mut E,
    state_path: &Path,
    state: &mut TuningState,
    addr: u16,
    value: u8,
    restoring: bool,
    skip_fan_ram: bool,
) -> Result<(), String> {
    if state.pending.is_some() {
        return Err("存在未确认写入，停止后续调校".to_string());
    }
    let group = owned_group(addr);
    if restoring && state.relinquished.iter().any(|g| g == group) {
        return Ok(());
    }
    check_owned(ec, state_path, state, restoring, skip_fan_ram)?;
    if restoring && state.relinquished.iter().any(|g| g == group) {
        return Ok(());
    }
    let mask = state.masks()[&addr];
    let bits = value & mask;
    let current = ec.read(addr)?;
    if restoring && skip_fan_ram && matches!(addr, 0x741 | 0x7C5 | 0x7C6) && (current & mask) == bits {
        state.confirmed.insert(addr, bits);
        return Ok(());
    }
    if Some(current & mask) != state.confirmed.get(&addr).copied() {
        let mut s = BTreeSet::new();
        s.insert(group);
        add_relinquished(state, &s);
        save_tuning_state(state_path, state)?;
        return Err("写入前所有权发生变化".to_string());
    }
    if (current & mask) == bits {
        return Ok(());
    }
    state.pending = Some(PendingWrite {
        address: addr,
        value: bits,
    });
    save_tuning_state(state_path, state)?;

    check_owned(ec, state_path, state, restoring, skip_fan_ram)?;
    if restoring && state.relinquished.iter().any(|g| g == group) {
        state.pending = None;
        save_tuning_state(state_path, state)?;
        return Ok(());
    }
    let current = ec.read(addr)?;
    if restoring && skip_fan_ram && matches!(addr, 0x741 | 0x7C5 | 0x7C6) && (current & mask) == bits {
        state.confirmed.insert(addr, bits);
        state.pending = None;
        save_tuning_state(state_path, state)?;
        return Ok(());
    }
    if Some(current & mask) != state.confirmed.get(&addr).copied() {
        let mut s = BTreeSet::new();
        s.insert(group);
        add_relinquished(state, &s);
        save_tuning_state(state_path, state)?;
        return Err("恢复记录保存期间发生外来写入".to_string());
    }
    ec.write_tuning(addr, (current & !mask) | bits)?;
    if (ec.read(addr)? & mask) != bits {
        return Err("自定义写入回读不一致".to_string());
    }
    state.confirmed.insert(addr, bits);
    state.pending = None;
    save_tuning_state(state_path, state)
}

pub fn restore_tuning<E: EcIo>(
    ec: &mut E,
    state_path: &Path,
    skip_fan_ram_in: bool,
) -> Result<(), String> {
    let Some(state) = read_tuning_state(state_path)? else {
        return Ok(());
    };
    let skip_fan_ram = skip_fan_ram_in || fan_safety_protect_ec(ec)?;
    let mut state = migrate_state(ec, state_path, state)?;
    resolve_pending(ec, state_path, &mut state, skip_fan_ram)?;
    state.phase = "restoring".to_string();
    save_tuning_state(state_path, &state)?;
    check_owned(ec, state_path, &mut state, true, skip_fan_ram)?;
    let before = state.before.clone();

    if state.has_gpu {
        for a in [0x743u16, 0x745, 0x746, 0x744] {
            tuning_put(ec, state_path, &mut state, a, before[&a], true, skip_fan_ram)?;
        }
    }
    for a in [0x783u16, 0x784] {
        tuning_put(ec, state_path, &mut state, a, before[&a], true, skip_fan_ram)?;
    }
    tuning_put(ec, state_path, &mut state, 0x7C6, 0, true, skip_fan_ram)?;
    if !skip_fan_ram {
        for a in 0xF00u16..=0xF5F {
            tuning_put(ec, state_path, &mut state, a, before[&a], true, skip_fan_ram)?;
        }
    }
    tuning_put(ec, state_path, &mut state, 0x7C5, before[&0x7C5], true, skip_fan_ram)?;
    tuning_put(ec, state_path, &mut state, 0x7C6, before[&0x7C6], true, skip_fan_ram)?;
    tuning_put(ec, state_path, &mut state, 0x741, before[&0x741], true, skip_fan_ram)?;
    for a in [0x726u16, 0x727, REG_MODE] {
        tuning_put(ec, state_path, &mut state, a, before[&a], true, skip_fan_ram)?;
    }
    check_owned(ec, state_path, &mut state, true, skip_fan_ram)?;
    fs::remove_file(state_path).map_err(|e| format!("清理恢复记录失败: {e}"))?;
    Ok(())
}

pub fn set_mode<E: EcIo>(ec: &mut E, name: &str, pause: Duration) -> Result<(), String> {
    let Some(target) = mode_byte(name) else {
        return Err(format!("不支持的档位: {name}"));
    };
    let before = snapshot(ec)?;
    if !standard_state(&before) {
        return Err("当前 EC 状态不属于已验证的基础档位，拒绝覆盖".to_string());
    }
    if name == "custom" && (*before.get(&0x7A5).unwrap_or(&0) & 0x08) != 0 {
        return Err("EC 处于高温风扇安全保护状态，暂停进入自定义档".to_string());
    }
    if mode_name(&before) == name {
        return Ok(());
    }
    if snapshot(ec)? != before {
        return Err("硬件状态正在变化，请稍后重试".to_string());
    }

    let steps: Vec<(u16, u8)> = if name == "custom" {
        vec![(0x727, 1), (REG_MODE, target), (0x726, 1)]
    } else if mode_name(&before) == "custom" {
        vec![(0x726, 0), (0x727, 0), (REG_MODE, target)]
    } else {
        vec![(REG_MODE, target)]
    };

    let mut attempted: Vec<(u16, u8, u8, u8)> = Vec::new();
    let res = (|| -> Result<(), String> {
        for (addr, value) in steps {
            let old = ec.read(addr)?;
            let mask = flag_mask(addr).unwrap_or(0xFF);
            let expected = if addr == REG_MODE {
                value
            } else {
                (old & !mask) | if value != 0 { mask } else { 0 }
            };
            attempted.push((addr, old, expected, mask));
            if addr == REG_MODE {
                ec.write_mode(value)?;
            } else {
                ec.write_flag(addr, value != 0)?;
            }
        }
        if !pause.is_zero() {
            thread::sleep(pause);
        }
        let after = snapshot(ec)?;
        if mode_name(&after) != name || !standard_state(&after) {
            return Err("档位回读不一致，请检查机身模式键".to_string());
        }
        Ok(())
    })();

    if let Err(err) = res {
        let mut rb_errors = Vec::new();
        for (addr, old, expected, mask) in attempted.into_iter().rev() {
            match ec.read(addr) {
                Ok(cur) if (cur & mask) == (expected & mask) => {
                    let r = if addr == REG_MODE {
                        ec.write_mode(old)
                    } else {
                        ec.write_flag(addr, (old & mask) != 0)
                    };
                    if let Err(e) = r {
                        rb_errors.push(e);
                    }
                }
                Ok(_) => {} // Hardware key or another owner won.
                Err(e) => rb_errors.push(e),
            }
        }
        if !rb_errors.is_empty() {
            return Err(format!("{err}；恢复未完成：{}", rb_errors.join("；")));
        }
        return Err(err);
    }
    Ok(())
}

pub fn reconcile_ec<E: EcIo>(
    ec: &mut E,
    state_path: &Path,
    force_safety: bool,
) -> Result<(), String> {
    let Some(state) = read_tuning_state(state_path)? else {
        return Ok(());
    };
    let safety = fan_safety_protect_ec(ec)?;
    let need_restore = force_safety
        || safety
        || state.phase != "active"
        || !tuning_matches(ec, &state)?
        || (state.has_gpu && !gpu_round_adapter(ec)?);
    if need_restore {
        restore_tuning(ec, state_path, safety || force_safety)?;
    }
    Ok(())
}

// ----------------------------------------------------------------- WMI device & Guard loop

fn read_turbo_bridge(fd: RawFd) -> Result<Value, String> {
    let mut buf = vec![0u8; 16_384];
    let n = unsafe { libc::pread(fd, buf.as_mut_ptr() as *mut libc::c_void, buf.len(), 0) };
    if n <= 0 {
        return Err(format!(
            "读取内核 WMI 桥接状态失败: {}",
            std::io::Error::last_os_error()
        ));
    }
    buf.truncate(n as usize);
    let data: Value =
        serde_json::from_slice(&buf).map_err(|e| format!("解析 WMI 桥接 JSON 失败: {e}"))?;
    if data.get("abi").and_then(|v| v.as_i64()) != Some(1)
        || !data.get("active").map(|v| v.is_boolean()).unwrap_or(false)
    {
        return Err("内核 WMI 桥接状态格式异常".to_string());
    }
    Ok(data)
}

fn open_wmi_watch() -> Result<(File, Value), String> {
    let c_path = CString::new(TURBO_DEVICE).unwrap();
    let fd = unsafe {
        libc::open(
            c_path.as_ptr(),
            libc::O_RDONLY | libc::O_NOFOLLOW | libc::O_NONBLOCK | libc::O_CLOEXEC,
        )
    };
    if fd < 0 {
        return Err(format!(
            "打开 WMI 桥接设备失败: {}",
            std::io::Error::last_os_error()
        ));
    }
    let file = unsafe { File::from_raw_fd(fd) };
    let meta = file
        .metadata()
        .map_err(|e| format!("stat WMI 桥接设备失败: {e}"))?;
    if !meta.file_type().is_char_device() || meta.uid() != 0 || (meta.mode() & 0o077) != 0 {
        return Err("狂暴调度设备权限异常".to_string());
    }
    let initial = read_turbo_bridge(file.as_raw_fd())?;
    Ok((file, initial))
}

fn turbo_release_optional() -> Result<Value, String> {
    let sock = Path::new(TURBO_SOCKET);
    if sock.exists() {
        if let Ok(resp) = turbo_socket_request(sock, &json!({"command": "release"}), Duration::from_secs(4)) {
            if resp.get("ok").and_then(|v| v.as_bool()) == Some(true) {
                let result = resp.get("result").cloned().unwrap_or(Value::Null);
                let bridge = result.get("bridge").cloned().unwrap_or(result);
                if bridge.get("active").map(|v| v.is_boolean()).unwrap_or(false) {
                    return Ok(bridge);
                }
            }
        }
    }
    // Fallback: inspect journal & /dev/mechrevo-turbo directly.
    if Path::new(TURBO_JOURNAL_PATH).exists() {
        return Err("狂暴调度存在待恢复记录".to_string());
    }
    let dev_path = Path::new(TURBO_DEVICE);
    if dev_path.exists() {
        let (_f, bridge) = open_wmi_watch()?;
        return Ok(bridge);
    }
    Ok(json!({"available": false, "active": false, "recovery_needed": false}))
}

pub(crate) fn run_with_lock<T, F>(writable: bool, background: bool, f: F) -> Result<T, LockError>
where
    F: FnOnce(&mut RealEc) -> Result<T, String>,
{
    let wait = if background {
        None
    } else {
        Some(Duration::from_millis(1500))
    };
    let _guard = RootFlock::acquire(Path::new(PROFILE_LOCK), wait)?;
    let mut ec = RealEc::open(writable).map_err(LockError::Other)?;
    f(&mut ec).map_err(LockError::Other)
}

fn handle_mode_key_wmi(bridge: &Value, mut last_release_seq: Option<i64>) -> Result<Option<i64>, String> {
    let stop_reason = bridge.get("stop_reason").and_then(|v| v.as_str());
    let seq = bridge.get("sequence").and_then(|v| v.as_i64());
    let wmi_turbo = bridge.get("active").and_then(|v| v.as_bool()) == Some(true)
        || (stop_reason == Some("mode_key_wmi") && seq != last_release_seq);

    if wmi_turbo {
        let released = turbo_release_optional()?;
        if released.get("stop_reason").and_then(|v| v.as_str()) == Some("mode_key_wmi") {
            if let Some(rseq) = released.get("sequence").and_then(|v| v.as_i64()) {
                last_release_seq = Some(rseq);
            }
        } else if stop_reason == Some("mode_key_wmi") {
            last_release_seq = seq;
        }
        if released.get("active").and_then(|v| v.as_bool()) == Some(true)
            || released.get("recovery_needed").and_then(|v| v.as_bool()) == Some(true)
        {
            return Err("狂暴调度尚未释放，暂不执行模式键切换".to_string());
        }
    } else if stop_reason == Some("mode_key_wmi") {
        last_release_seq = seq;
    }

    let state_path = Path::new(TUNING_STATE_PATH);
    run_with_lock(true, false, |ec| {
        let has_state = read_tuning_state(state_path)?.is_some();
        let appe_set = (ec.read(0x741)? & 1) != 0;
        if has_state {
            let safety = fan_safety_protect_ec(ec)?;
            restore_tuning(ec, state_path, safety)?;
            let snap = snapshot(ec)?;
            if standard_state(&snap) && mode_name(&snap) != "office" {
                set_mode(ec, "office", Duration::from_millis(250))?;
            }
        } else if appe_set || wmi_turbo {
            let snap = snapshot(ec)?;
            let current = mode_name(&snap);
            if standard_state(&snap) {
                let next = match current {
                    "turbo" => Some("balanced"),
                    "balanced" => Some("office"),
                    "office" => Some("turbo"),
                    _ => None,
                };
                if let Some(target) = next {
                    set_mode(ec, target, Duration::from_millis(250))?;
                }
            }
        }
        Ok(())
    })
    .map_err(|e| e.to_string())?;

    Ok(last_release_seq)
}

fn write_wmi_state(payload: &Value) -> Result<(), String> {
    atomic_write_json(Path::new(WMI_STATE_PATH), payload, 0o644, ".mechrevo-wmi-", false)
}

fn write_lc_ec_state(payload: &Value) -> Result<(), String> {
    atomic_write_json(
        Path::new(LC_EC_STATE_PATH),
        payload,
        0o644,
        ".mechrevo-lc-ec-",
        false,
    )
}

pub fn sample_lc_status() -> Result<Value, String> {
    check_machine()?;
    run_with_lock(false, false, |ec| Ok(watercooling_status(&snapshot(ec)?)))
        .map_err(|e| e.to_string())
}

pub fn reset_tuning() -> Result<Value, String> {
    check_machine()?;
    let released = turbo_release_optional()?;
    if released.get("active").and_then(|v| v.as_bool()) == Some(true)
        || released.get("recovery_needed").and_then(|v| v.as_bool()) == Some(true)
    {
        return Err("狂暴调度尚未释放，拒绝重置".to_string());
    }
    let state_path = Path::new(TUNING_STATE_PATH);
    run_with_lock(true, false, |ec| {
        restore_tuning(ec, state_path, false)?;
        let snap = snapshot(ec)?;
        Ok(json!({
            "mode": mode_name(&snap),
            "standard": standard_state(&snap),
            "tuning_active": false,
        }))
    })
    .map_err(|e| e.to_string())
}

pub fn run_guard() -> Result<(), String> {
    if unsafe { libc::geteuid() } != 0 {
        return Err("性能恢复守护进程需要root权限".to_string());
    }
    let mut pipe_fds = [-1i32; 2];
    if unsafe { libc::pipe2(pipe_fds.as_mut_ptr(), libc::O_NONBLOCK | libc::O_CLOEXEC) } != 0 {
        return Err(format!("pipe2 失败: {}", std::io::Error::last_os_error()));
    }
    let wake_r = pipe_fds[0];
    let wake_w = pipe_fds[1];
    WAKE_WRITER_FD.store(wake_w, Ordering::SeqCst);
    GUARD_STOP.store(false, Ordering::SeqCst);
    install_guard_signals();

    let mut wmi_file: Option<File> = None;
    let mut last_wmi_seq: i64 = 0;
    let mut last_release_seq: Option<i64> = None;
    let mut pending_safety = false;
    let mut last_error_at: Option<Instant> = None;
    let mut last_lc_sample: Option<Instant> = None;

    let mut log_error = |msg: &str| {
        let now = Instant::now();
        if last_error_at
            .map(|t| now.duration_since(t) > Duration::from_secs(30))
            .unwrap_or(true)
        {
            eprintln!("{msg}");
            last_error_at = Some(now);
        }
    };

    let refresh_lc = |last_lc: &mut Option<Instant>| {
        if check_machine().is_ok() {
            if let Ok(payload) =
                run_with_lock(false, true, |ec| Ok(watercooling_status(&snapshot(ec)?)))
            {
                let _ = write_lc_ec_state(&payload);
            }
        }
        *last_lc = Some(Instant::now());
    };

    let state_path = Path::new(TUNING_STATE_PATH);
    let sleep_marker = Path::new(SLEEP_MARKER_PATH);
    let turbo_dev = Path::new(TURBO_DEVICE);

    while !GUARD_STOP.load(Ordering::SeqCst) {
        let sleeping = sleep_marker.exists();
        if sleeping {
            wmi_file = None;
        } else if wmi_file.is_none() && turbo_dev.exists() {
            match open_wmi_watch() {
                Ok((file, initial)) => {
                    let max_ev = initial
                        .get("wmi_events")
                        .and_then(|v| v.as_array())
                        .map(|arr| {
                            arr.iter()
                                .filter_map(|e| e.get("seq").and_then(|s| s.as_i64()))
                                .max()
                                .unwrap_or(0)
                        })
                        .unwrap_or(0);
                    let top_seq = initial.get("wmi_seq").and_then(|v| v.as_i64()).unwrap_or(0);
                    last_wmi_seq = max_ev.max(top_seq);
                    last_release_seq =
                        if initial.get("stop_reason").and_then(|v| v.as_str()) == Some("mode_key_wmi") {
                            initial.get("sequence").and_then(|v| v.as_i64())
                        } else {
                            None
                        };
                    let _ = write_wmi_state(&sanitize_wmi_state(&initial));
                    wmi_file = Some(file);
                }
                Err(e) => {
                    wmi_file = None;
                    log_error(&e);
                }
            }
        }

        if !sleeping && (pending_safety || state_path.exists()) {
            let res = check_machine()
                .map_err(LockError::Other)
                .and_then(|()| {
                    run_with_lock(true, !pending_safety, |ec| {
                        reconcile_ec(ec, state_path, pending_safety)
                    })
                });
            match res {
                Ok(()) => pending_safety = false,
                Err(LockError::Busy(_)) => {}
                Err(LockError::Other(e)) => {
                    pending_safety = false;
                    log_error(&e);
                }
            }
        }

        if !sleeping
            && last_lc_sample
                .map(|t| t.elapsed() >= Duration::from_secs(3))
                .unwrap_or(true)
        {
            refresh_lc(&mut last_lc_sample);
        }

        if GUARD_STOP.load(Ordering::SeqCst) {
            break;
        }

        let mut pfds = [
            libc::pollfd {
                fd: wake_r,
                events: libc::POLLIN,
                revents: 0,
            },
            libc::pollfd {
                fd: wmi_file.as_ref().map(|f| f.as_raw_fd()).unwrap_or(-1),
                events: libc::POLLIN,
                revents: 0,
            },
        ];
        let nfds: libc::nfds_t = if wmi_file.is_some() { 2 } else { 1 };
        let rc = unsafe { libc::poll(pfds.as_mut_ptr(), nfds, 1000) };
        if rc > 0 && (pfds[0].revents & libc::POLLIN) != 0 {
            let mut drain = [0u8; 64];
            unsafe {
                let _ = libc::read(wake_r, drain.as_mut_ptr() as *mut libc::c_void, drain.len());
            }
        }
        if GUARD_STOP.load(Ordering::SeqCst) {
            break;
        }
        let wmi_ready = rc > 0 && nfds == 2 && (pfds[1].revents & (libc::POLLIN | libc::POLLERR | libc::POLLHUP)) != 0;
        if !wmi_ready {
            continue;
        }
        let Some(ref file) = wmi_file else {
            continue;
        };
        let bridge = match read_turbo_bridge(file.as_raw_fd()) {
            Ok(b) => b,
            Err(e) => {
                wmi_file = None;
                log_error(&e);
                continue;
            }
        };

        let mut codes = Vec::new();
        if let Some(arr) = bridge.get("wmi_events").and_then(|v| v.as_array()) {
            for ev in arr {
                if let Some(seq) = ev.get("seq").and_then(|v| v.as_i64()) {
                    if seq > last_wmi_seq {
                        last_wmi_seq = seq;
                        if let Some(code) = ev.get("code").and_then(|v| v.as_i64()) {
                            codes.push(code);
                        }
                    }
                }
            }
        }
        if let Err(e) = write_wmi_state(&sanitize_wmi_state(&bridge)) {
            log_error(&e);
        }

        if codes.contains(&WMI_SCANCODE_SAFETY_TEMP) {
            pending_safety = true;
            match check_machine()
                .map_err(LockError::Other)
                .and_then(|()| run_with_lock(true, false, |ec| reconcile_ec(ec, state_path, true)))
            {
                Ok(()) => pending_safety = false,
                Err(e) => log_error(&e.to_string()),
            }
        }
        if codes.contains(&WMI_SCANCODE_MODE_SWITCH) {
            match check_machine().and_then(|()| handle_mode_key_wmi(&bridge, last_release_seq)) {
                Ok(new_seq) => last_release_seq = new_seq,
                Err(e) => log_error(&e),
            }
        }
        if codes.contains(&WMI_SCANCODE_HOTKEY_C1) {
            refresh_lc(&mut last_lc_sample);
        }
    }

    WAKE_WRITER_FD.store(-1, Ordering::SeqCst);
    drop(wmi_file);
    unsafe {
        libc::close(wake_r);
        libc::close(wake_w);
    }
    let _ = PathBuf::new();
    Ok(())
}

#[cfg(test)]
mod tests {
    use super::*;

    struct FakeEc {
        vals: BTreeMap<u16, u8>,
    }

    impl FakeEc {
        fn standard_turbo() -> Self {
            let mut vals = BTreeMap::new();
            for &r in REGISTERS {
                vals.insert(r, 0);
            }
            for a in 0xF00u16..=0xF5F {
                vals.insert(a, 0);
            }
            vals.insert(0x706, 0x40);
            vals.insert(REG_MODE, 0x30);
            vals.insert(0x7CC, 0x81); // round adapter
            vals.insert(0x7A4, 0x80);
            vals.insert(0x7C7, 0x0C);
            vals.insert(0x7C8, 0x60);
            vals.insert(0x7E6, 0x80);
            vals.insert(0x7E7, 0x80);
            vals.insert(0xD48, 0xFF);
            Self { vals }
        }
    }

    impl EcIo for FakeEc {
        fn read(&self, addr: u16) -> Result<u8, String> {
            Ok(*self.vals.get(&addr).unwrap_or(&0))
        }
        fn write_mode(&mut self, value: u8) -> Result<(), String> {
            self.vals.insert(REG_MODE, value);
            Ok(())
        }
        fn write_flag(&mut self, addr: u16, enabled: bool) -> Result<(), String> {
            let mask = flag_mask(addr).unwrap();
            let cur = *self.vals.get(&addr).unwrap_or(&0);
            self.vals.insert(addr, (cur & !mask) | if enabled { mask } else { 0 });
            Ok(())
        }
        fn write_tuning(&mut self, addr: u16, value: u8) -> Result<(), String> {
            self.vals.insert(addr, value);
            Ok(())
        }
        fn write_logo(&mut self, addr: u16, value: u8) -> Result<(), String> {
            let mask = logo_mask(addr).ok_or_else(|| "Unsupported logo register".to_string())?;
            if (value & !mask) != 0 {
                return Err("Invalid logo bits".to_string());
            }
            let cur = *self.vals.get(&addr).unwrap_or(&0);
            self.vals.insert(addr, (cur & !mask) | value);
            Ok(())
        }
    }

    #[test]
    fn test_watercooling_status_matches_live_ec_bytes() {
        let ec = FakeEc::standard_turbo();
        let snap = snapshot(&ec).unwrap();
        let wc = watercooling_status(&snap);
        assert_eq!(wc["lc_supported"], true);
        assert_eq!(wc["lc_fan_table_supported"], true);
        assert_eq!(wc["lc_hwoc_supported"], false);
        assert_eq!(wc["lc_ec_enabled"], false);
        assert_eq!(wc["ec_c2_index"], 3);
        assert_eq!(wc["ec_fan_enabled"], true);
        assert_eq!(wc["ec_fan_duty"], 0);
        assert_eq!(wc["ec_pump_enabled"], true);
        assert_eq!(wc["ec_pump_volt"], 0);
    }

    #[test]
    fn test_set_mode_transitions_and_restore_tuning_roundtrip() {
        let mut ec = FakeEc::standard_turbo();
        set_mode(&mut ec, "balanced", Duration::ZERO).unwrap();
        assert_eq!(mode_name(&snapshot(&ec).unwrap()), "balanced");
        set_mode(&mut ec, "custom", Duration::ZERO).unwrap();
        assert_eq!(mode_name(&snapshot(&ec).unwrap()), "custom");
        assert_eq!(ec.read(0x726).unwrap() & 0x80, 0x80);
        assert_eq!(ec.read(0x727).unwrap() & 0x40, 0x40);
        set_mode(&mut ec, "office", Duration::ZERO).unwrap();
        assert_eq!(mode_name(&snapshot(&ec).unwrap()), "office");
        assert_eq!(ec.read(0x726).unwrap() & 0x80, 0);
        assert_eq!(ec.read(0x727).unwrap() & 0x40, 0);
    }

    #[test]
    fn test_tuning_restore_with_foreign_mode_handoff_and_thermal_safety() {
        let dir = std::env::temp_dir().join(format!("guardd-test-{}", std::process::id()));
        let _ = fs::remove_dir_all(&dir);
        fs::create_dir_all(&dir).unwrap();
        let state_path = dir.join("tuning.json");

        let mut ec = FakeEc::standard_turbo();
        let before_snap = ec.vals.clone();

        let cfg = json!({
            "pl1": 45,
            "pl2": 45,
            "cpu_fan": [40,40,40,40,50,55,60,70,80,90,100],
            "gpu_fan": [40,40,40,40,50,55,60,70,80,90,100],
        });
        let mut before = BTreeMap::new();
        let mut target = BTreeMap::new();
        let mut confirmed = BTreeMap::new();
        for &(a, mask) in BASE_OWN_MASKS {
            before.insert(a, before_snap[&a]);
            let mut tv = before_snap[&a];
            match a {
                0x726 | 0x727 | 0x741 | 0x7C5 | 0x7C6 => tv |= mask,
                REG_MODE => tv = 0,
                0x783 | 0x784 => tv = 45,
                _ => {}
            }
            target.insert(a, tv);
            confirmed.insert(a, tv & mask);
            ec.vals.insert(a, tv);
        }
        for a in 0xF00u16..=0xF5F {
            before.insert(a, 0);
            target.insert(a, 100);
            confirmed.insert(a, 100);
            ec.vals.insert(a, 100);
        }
        let st = TuningState {
            version: 2,
            phase: "active".into(),
            config: cfg,
            has_gpu: false,
            before,
            target,
            confirmed,
            pending: None,
            relinquished: Vec::new(),
            legacy_ambiguous: false,
        };
        save_tuning_state(&state_path, &st).unwrap();
        assert!(tuning_matches(&ec, &st).unwrap());

        // Simulate physical Mode Key switching to office while custom tuning active:
        ec.vals.insert(REG_MODE, 0xA0);
        ec.vals.insert(0x726, 0);
        ec.vals.insert(0x727, 0);

        reconcile_ec(&mut ec, &state_path, false).unwrap();
        assert!(!state_path.exists());
        // Physical Mode Key (0xA0) won, while CPU/APPE/fan overrides were cleanly restored!
        assert_eq!(ec.read(REG_MODE).unwrap(), 0xA0);
        assert_eq!(ec.read(0x783).unwrap(), 0);
        assert_eq!(ec.read(0x741).unwrap() & 1, 0);
        assert_eq!(ec.read(0xF00).unwrap(), 0);

        let _ = fs::remove_dir_all(&dir);
    }
}

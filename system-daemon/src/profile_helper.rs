// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Native Rust root helper (`mechrevo-profile-helper`): firmware mode switching,
//! rear-lid Logo EC control, bounded CPU/GPU power & 11-node fan tuning with
//! v2 recovery journal, and turbo lease status/control coordination.

use crate::common::{
    check_machine, kernel_release, run_command_timeout, trusted_file, turbo_socket_request,
    KERNEL, TURBO_DEVICE, TURBO_SOCKET,
};
use crate::ec_guard::{
    fan_safety_protect_ec, gpu_round_adapter, logo_mask, mode_name, read_tuning_state,
    reconcile_ec, restore_tuning, run_with_lock, save_tuning_state, set_mode, snapshot,
    standard_state, tuning_matches, tuning_put, unmanaged_gpu_clean, validate_tuning_config,
    watercooling_status, EcIo, TuningState, BASE_OWN_MASKS, GPU_OWN_MASKS, LOGO_MASKS, REG_MODE,
    TUNING_STATE_PATH, TURBO_JOURNAL_PATH,
};
use serde_json::{json, Map, Value};
use std::collections::BTreeMap;
use std::ffi::CString;
use std::fs::File;
use std::os::unix::ffi::OsStrExt;
use std::os::unix::fs::{FileTypeExt, MetadataExt};
use std::os::unix::io::{AsRawFd, FromRawFd};
use std::path::Path;
use std::thread;
use std::time::Duration;

pub const GPU_UEFI_PATH: &str =
    "/sys/firmware/efi/efivars/OemMagicVariable-9f33f85c-13ca-4fd1-9c4a-96217722c593";

pub const LOGO_COLORS: [[u8; 3]; 7] = [
    [255, 255, 255],
    [255, 0, 0],
    [0, 255, 0],
    [0, 0, 255],
    [255, 255, 0],
    [136, 0, 255],
    [0, 255, 255],
];

const CPU_UP: [u8; 16] = [48, 52, 56, 60, 64, 68, 72, 76, 80, 85, 255, 255, 255, 255, 255, 255];
const GPU_UP: [u8; 16] = [46, 50, 54, 58, 62, 66, 70, 74, 78, 81, 255, 255, 255, 255, 255, 255];
const CPU_DOWN: [u8; 16] = [0, 46, 50, 54, 58, 62, 66, 70, 74, 78, 82, 255, 255, 255, 255, 255];
const GPU_DOWN: [u8; 16] = [0, 44, 48, 52, 56, 60, 64, 68, 72, 76, 80, 255, 255, 255, 255, 255];

// ----------------------------------------------------------------- Turbo socket + fallback

fn turbo_fallback(err: Option<&str>) -> Result<Value, String> {
    if Path::new(TURBO_JOURNAL_PATH).exists() {
        return Err("狂暴调度存在待恢复记录".to_string());
    }
    let mut out = Map::new();
    out.insert("available".into(), json!(false));
    out.insert("active".into(), json!(false));
    let dev_path = Path::new(TURBO_DEVICE);
    if dev_path.exists() {
        let c_path = CString::new(dev_path.as_os_str().as_bytes())
            .map_err(|_| "设备路径无效".to_string())?;
        let fd = unsafe {
            libc::open(
                c_path.as_ptr(),
                libc::O_RDONLY | libc::O_NOFOLLOW | libc::O_CLOEXEC,
            )
        };
        if fd < 0 {
            return Err(format!(
                "打开狂暴调度设备失败: {}",
                std::io::Error::last_os_error()
            ));
        }
        let file = unsafe { File::from_raw_fd(fd) };
        let meta = file
            .metadata()
            .map_err(|e| format!("stat 狂暴调度设备失败: {e}"))?;
        if !meta.file_type().is_char_device() || meta.uid() != 0 || (meta.mode() & 0o077) != 0 {
            return Err("狂暴调度设备权限异常".to_string());
        }
        let mut buf = vec![0u8; 8192];
        let n = unsafe {
            libc::pread(
                file.as_raw_fd(),
                buf.as_mut_ptr() as *mut libc::c_void,
                buf.len(),
                0,
            )
        };
        if n <= 0 {
            return Err("无法确认内核调度恢复状态".to_string());
        }
        buf.truncate(n as usize);
        let bridge: Value = serde_json::from_slice(&buf)
            .map_err(|_| "无法确认内核调度恢复状态".to_string())?;
        let valid = bridge.get("abi").and_then(|v| v.as_i64()) == Some(1)
            && bridge.get("read_rc").and_then(|v| v.as_i64()) == Some(0)
            && bridge.get("active").map(|v| v.is_boolean()).unwrap_or(false)
            && bridge
                .get("recovery_needed")
                .map(|v| v.is_boolean())
                .unwrap_or(false);
        if !valid {
            return Err("无法确认内核调度恢复状态".to_string());
        }
        if let Some(obj) = bridge.as_object() {
            for (k, v) in obj {
                out.insert(k.clone(), v.clone());
            }
        }
    }
    if let Some(e) = err {
        out.insert("error".into(), json!(e));
    }
    Ok(Value::Object(out))
}

fn turbo_request(req: &Value, optional: bool) -> Result<Value, String> {
    let sock = Path::new(TURBO_SOCKET);
    if !sock.exists() {
        if optional {
            return turbo_fallback(None);
        }
        return Err("狂暴调度服务未运行，或存在待恢复记录".to_string());
    }
    let call = (|| -> Result<Value, String> {
        let data = turbo_socket_request(sock, req, Duration::from_secs(4))?;
        if data.get("ok").and_then(|v| v.as_bool()) != Some(true) {
            let msg = data
                .get("error")
                .and_then(|v| v.as_str())
                .unwrap_or("狂暴调度服务不可用");
            return Err(msg.to_string());
        }
        let result = data
            .get("result")
            .ok_or_else(|| "狂暴调度未返回有效状态".to_string())?;
        let bridge = result.get("bridge").unwrap_or(result);
        if !bridge
            .get("active")
            .map(|v| v.is_boolean())
            .unwrap_or(false)
        {
            return Err("狂暴调度未返回有效状态".to_string());
        }
        let mut out = Map::new();
        out.insert("available".into(), json!(true));
        if let Some(obj) = bridge.as_object() {
            for (k, v) in obj {
                out.insert(k.clone(), v.clone());
            }
        }
        out.insert(
            "scheduler".into(),
            result.get("scheduler").cloned().unwrap_or(Value::Null),
        );
        Ok(Value::Object(out))
    })();

    match call {
        Ok(v) => Ok(v),
        Err(err) => {
            if optional {
                turbo_fallback(Some(&err))
            } else {
                Err(format!("狂暴调度操作失败：{err}"))
            }
        }
    }
}

// ----------------------------------------------------------------- Rear-lid Logo

pub fn logo_status<E: EcIo>(ec: &E) -> Result<Value, String> {
    let raw = ec.read(0x7AB)?;
    if (raw & 1) == 0 {
        return Err("本机固件未报告 Logo 灯支持".to_string());
    }
    let level = raw >> 5;
    if level > 4 {
        return Err("Logo 亮度状态不在官方范围内".to_string());
    }
    let rainbow = (ec.read(0x767)? & 128) != 0;
    let breathing = ec.read(0x7C5)? & 7;
    let channels = [ec.read(0x769)?, ec.read(0x76A)?, ec.read(0x76B)?];
    if channels.iter().any(|&c| c > 50) {
        return Err("Logo RGB 状态不在官方范围内".to_string());
    }
    let color: [u8; 3] = if breathing != 0 && (breathing as usize) <= LOGO_COLORS.len() {
        LOGO_COLORS[(breathing - 1) as usize]
    } else {
        [
            ((channels[0] as u16) * 51).div_ceil(10) as u8,
            ((channels[1] as u16) * 51).div_ceil(10) as u8,
            ((channels[2] as u16) * 51).div_ceil(10) as u8,
        ]
    };
    let mode = if rainbow {
        "rainbow"
    } else if breathing != 0 {
        "breathing"
    } else {
        "solid"
    };
    Ok(json!({
        "logo_enabled": level != 0,
        "brightness": (level as u16) * 25,
        "mode": mode,
        "color": color,
    }))
}

#[derive(Debug, Clone)]
pub struct LogoConfig {
    pub mode: String,
    pub brightness: u8,
    pub color: [u8; 3],
}

pub fn validate_logo(cfg: &Value) -> Result<LogoConfig, String> {
    let Some(obj) = cfg.as_object() else {
        return Err("Logo 设置字段不完整".to_string());
    };
    if obj.len() != 3
        || !obj.contains_key("mode")
        || !obj.contains_key("brightness")
        || !obj.contains_key("color")
    {
        return Err("Logo 设置字段不完整".to_string());
    }
    let Some(mode) = obj["mode"].as_str() else {
        return Err("Logo 灯效或亮度不在官方范围内".to_string());
    };
    if !matches!(mode, "solid" | "breathing" | "rainbow") {
        return Err("Logo 灯效或亮度不在官方范围内".to_string());
    }
    let Some(b) = obj["brightness"].as_i64() else {
        return Err("Logo 灯效或亮度不在官方范围内".to_string());
    };
    if !matches!(b, 0 | 25 | 50 | 75 | 100) {
        return Err("Logo 灯效或亮度不在官方范围内".to_string());
    }
    let Some(arr) = obj["color"].as_array() else {
        return Err("Logo RGB 必须为三个 0–255 整数".to_string());
    };
    if arr.len() != 3 {
        return Err("Logo RGB 必须为三个 0–255 整数".to_string());
    }
    let mut color = [0u8; 3];
    for (i, item) in arr.iter().enumerate() {
        let Some(v) = item.as_i64() else {
            return Err("Logo RGB 必须为三个 0–255 整数".to_string());
        };
        if !(0..=255).contains(&v) {
            return Err("Logo RGB 必须为三个 0–255 整数".to_string());
        }
        color[i] = v as u8;
    }
    if mode == "breathing" && !LOGO_COLORS.contains(&color) {
        return Err("Logo 呼吸仅支持白、红、绿、蓝、黄、紫、青".to_string());
    }
    Ok(LogoConfig {
        mode: mode.to_string(),
        brightness: b as u8,
        color,
    })
}

pub fn set_logo<E: EcIo>(ec: &mut E, cfg: &LogoConfig, pause: Duration) -> Result<Value, String> {
    let _ = logo_status(ec)?;
    let mut before: BTreeMap<u16, u8> = BTreeMap::new();
    for &(addr, _) in LOGO_MASKS {
        before.insert(addr, ec.read(addr)?);
    }

    let mut target: Vec<(u16, u8)> = Vec::new();
    let mut level: u8 = (cfg.brightness / 25) << 5;
    if level != 0 {
        target.push((0x767, if cfg.mode == "rainbow" { 128 } else { 0 }));
        let breathing_idx = if cfg.mode == "breathing" {
            LOGO_COLORS
                .iter()
                .position(|c| *c == cfg.color)
                .map(|i| (i + 1) as u8)
                .unwrap_or(0)
        } else {
            0
        };
        target.push((0x7C5, breathing_idx));
        if cfg.mode == "solid" {
            let channels = [
                (((cfg.color[0] as u16) * 10) / 51).min(50) as u8,
                (((cfg.color[1] as u16) * 10) / 51).min(50) as u8,
                (((cfg.color[2] as u16) * 10) / 51).min(50) as u8,
            ];
            if channels == [0, 0, 0] {
                level = 0;
            } else {
                target.push((0x769, channels[0]));
                target.push((0x76A, channels[1]));
                target.push((0x76B, channels[2]));
            }
        }
    }
    target.push((0x7AB, level));

    let mut attempted: Vec<(u16, u8)> = Vec::new();
    let apply_res = (|| -> Result<Value, String> {
        for &(addr, val) in &target {
            attempted.push((addr, val));
            ec.write_logo(addr, val)?;
        }
        ec.write_logo(0x78C, 16)?;
        if !pause.is_zero() {
            thread::sleep(pause);
        }
        for &(addr, val) in &target {
            let mask = logo_mask(addr).unwrap_or(0xFF);
            if (ec.read(addr)? & mask) != val {
                return Err("Logo 设置回读不一致".to_string());
            }
        }
        logo_status(ec)
    })();

    if let Err(err) = apply_res {
        let mut failures: Vec<String> = Vec::new();
        for &(addr, val) in attempted.iter().rev() {
            let mask = logo_mask(addr).unwrap_or(0xFF);
            match ec.read(addr) {
                Ok(cur) if (cur & mask) == val => {
                    let prev = before[&addr] & mask;
                    if let Err(e) = ec.write_logo(addr, prev) {
                        failures.push(e);
                    }
                }
                Ok(_) => {}
                Err(e) => failures.push(e),
            }
        }
        if let Err(e) = ec.write_logo(0x78C, 16) {
            failures.push(e);
        }
        if !failures.is_empty() {
            return Err(format!("{err}；Logo 恢复未完成：{}", failures.join("；")));
        }
        return Err(err);
    }
    apply_res
}

// ----------------------------------------------------------------- GPU capability & Status

pub fn parse_gpu_uefi(raw: &[u8]) -> Result<(), String> {
    if raw.len() != 184 || raw[4 + 0x43] != 25 {
        return Err("GPU调校需要真实UEFI payload180、APVersionCheck=25".to_string());
    }
    Ok(())
}

pub fn check_gpu_capability<E: EcIo>(ec: &E) -> Result<(), String> {
    check_machine()?;
    if kernel_release()? != KERNEL || (ec.read(0x7D2)? & 31) != 25 {
        return Err(format!("GPU调校仅支持已验证内核{KERNEL}与GFID=25"));
    }
    let raw = trusted_file(Path::new(GPU_UEFI_PATH), false, 185)
        .map_err(|_| "GPU固件变量权限异常".to_string())?;
    parse_gpu_uefi(&raw)
}

fn check_gpu_runtime() -> Result<(), String> {
    let p1 = run_command_timeout(
        "/usr/bin/systemctl",
        &["is-active", "--quiet", "nvidia-powerd.service"],
        Duration::from_secs(2),
    )?;
    if p1.code != 0 {
        return Err("nvidia-powerd未运行，拒绝启用GPU调校".to_string());
    }
    let p2 = run_command_timeout(
        "/usr/local/libexec/mechrevo-acpi-check",
        &["--summary"],
        Duration::from_secs(3),
    )?;
    let data: Value = serde_json::from_str(p2.stdout.trim())
        .map_err(|_| "ACPI完整性检查未通过，拒绝启用GPU调校".to_string())?;
    if p2.code != 0 || data.get("healthy").and_then(|v| v.as_bool()) != Some(true) {
        return Err("ACPI完整性检查未通过，拒绝启用GPU调校".to_string());
    }
    Ok(())
}

pub fn status_with_gpu_check<E: EcIo>(
    ec: &E,
    state_path: &Path,
    gpu_check: Result<(), String>,
) -> Result<Value, String> {
    let values = snapshot(ec)?;
    let state = read_tuning_state(state_path)?;
    let managed = match &state {
        Some(st) if st.phase == "active" => tuning_matches(ec, st)?,
        _ => false,
    };
    let standard = standard_state(&values) && state.is_none();
    let safety = (*values.get(&0x7A5).unwrap_or(&0) & 0x08) != 0;
    let v7ac = *values.get(&0x7AC).unwrap_or(&0);
    let sb_sup = (v7ac & 0x01) != 0;
    let sb_en = (v7ac & 0x02) != 0;
    let (gpu_supported, gpu_reason) = match gpu_check {
        Ok(()) => (true, String::new()),
        Err(e) => (false, e),
    };
    let tuning_val = if managed {
        state.as_ref().map(|s| s.config.clone()).unwrap_or(Value::Null)
    } else {
        Value::Null
    };
    let raw_mode = *values.get(&REG_MODE).unwrap_or(&0);
    let v728 = *values.get(&0x728).unwrap_or(&0);
    Ok(json!({
        "mode": mode_name(&values),
        "switchable": standard || managed,
        "reason": if standard || managed { "" } else { "检测到其他控制程序、未验证的风扇或功耗设置，暂停软件切档" },
        "ec_mode": format!("0x{raw_mode:x}"),
        "custom_supported": true,
        "custom_tuning_supported": true,
        "tuning_active": managed,
        "tuning_recovery_pending": state.is_some() && !managed,
        "tuning": tuning_val,
        "cpu_limits_w": [25, 75],
        "gpu_tuning_supported": gpu_supported,
        "gpu_tuning_range": [65, 115],
        "gpu_db_range": [5, 25],
        "gpu_tuning_reason": gpu_reason,
        "turbo_variants_supported": false,
        "turbo_variant": if (v728 & 1) != 0 { "quiet" } else { "extreme" },
        "fan_safety_protect": safety,
        "smart_balance_supported": sb_sup,
        "smart_balance_enabled": sb_en,
        "watercooling": watercooling_status(&values),
        "wmi_bound": false,
    }))
}

pub fn status<E: EcIo>(ec: &E, state_path: &Path) -> Result<Value, String> {
    let gpu_res = check_gpu_capability(ec);
    status_with_gpu_check(ec, state_path, gpu_res)
}

pub fn status_with_turbo<E: EcIo>(ec: &E, state_path: &Path, turbo: &Value) -> Result<Value, String> {
    let mut result = status(ec, state_path)?;
    let obj = result.as_object_mut().unwrap();
    obj.insert("turbo".into(), turbo.clone());
    let avail = turbo.get("available").and_then(|v| v.as_bool()).unwrap_or(false);
    obj.insert("turbo_variants_supported".into(), json!(avail));
    let wmi_bound = turbo.get("wmi_bound").and_then(|v| v.as_bool()).unwrap_or(false);
    obj.insert("wmi_bound".into(), json!(wmi_bound));
    if let Some(seq) = turbo.get("wmi_seq") {
        obj.insert("wmi_seq".into(), seq.clone());
        obj.insert(
            "wmi_last_code".into(),
            turbo.get("wmi_last_code").cloned().unwrap_or(Value::Null),
        );
    }
    let active = turbo.get("active").and_then(|v| v.as_bool()).unwrap_or(false);
    let rec = turbo
        .get("recovery_needed")
        .and_then(|v| v.as_bool())
        .unwrap_or(false);
    let unc = turbo
        .get("policy_uncertain")
        .and_then(|v| v.as_bool())
        .unwrap_or(false);
    if active && !avail {
        obj.insert("switchable".into(), json!(false));
        obj.insert(
            "reason".into(),
            json!("狂暴调度后台未就绪，等待内核归还控制后再切档"),
        );
        return Ok(result);
    }
    if rec && !active {
        obj.insert("switchable".into(), json!(false));
        obj.insert(
            "reason".into(),
            json!(if unc {
                "固件温控状态无法确认，已停止软件切档；请正常重启电脑"
            } else {
                "狂暴调度恢复尚未完成，请先交还固件控制"
            }),
        );
        return Ok(result);
    }
    let std_turbo = turbo
        .get("standard_turbo")
        .and_then(|v| v.as_bool())
        .unwrap_or(false);
    if active
        && std_turbo
        && obj.get("mode").and_then(|v| v.as_str()) == Some("turbo")
        && obj.get("tuning_active").and_then(|v| v.as_bool()) == Some(false)
        && obj.get("tuning_recovery_pending").and_then(|v| v.as_bool()) == Some(false)
    {
        obj.insert("switchable".into(), json!(true));
        obj.insert("reason".into(), json!(""));
    }
    Ok(result)
}

// ----------------------------------------------------------------- Apply custom tuning

fn make_fan_table(config: &Value) -> Result<Vec<u8>, String> {
    let mut table = Vec::with_capacity(96);
    table.extend_from_slice(&CPU_UP);
    table.extend_from_slice(&CPU_DOWN);
    for item in config["cpu_fan"]
        .as_array()
        .ok_or("风扇曲线需要 11 个节点")?
    {
        table.push((item.as_i64().unwrap_or(100) * 2) as u8);
    }
    table.extend_from_slice(&[200; 5]);
    table.extend_from_slice(&GPU_UP);
    table.extend_from_slice(&GPU_DOWN);
    for item in config["gpu_fan"]
        .as_array()
        .ok_or("风扇曲线需要 11 个节点")?
    {
        table.push((item.as_i64().unwrap_or(100) * 2) as u8);
    }
    table.extend_from_slice(&[200; 5]);
    Ok(table)
}

pub fn apply_tuning<E: EcIo>(
    ec: &mut E,
    state_path: &Path,
    config: &Value,
    pause: Duration,
    verify_gpu_env: bool,
) -> Result<(), String> {
    validate_tuning_config(config)?;
    if fan_safety_protect_ec(ec)? {
        return Err("EC 处于高温风扇安全保护状态，暂停自定义调校".to_string());
    }
    let has_gpu = config
        .as_object()
        .map(|m| m.contains_key("gpu_ctgp"))
        .unwrap_or(false);
    if has_gpu {
        if verify_gpu_env {
            check_gpu_capability(ec)?;
            check_gpu_runtime()?;
        }
        if !gpu_round_adapter(ec)? {
            return Err("GPU调校需要圆口电源".to_string());
        }
    }
    restore_tuning(ec, state_path, false)?;
    let initial = snapshot(ec)?;
    if !standard_state(&initial) {
        return Err("当前控制状态不允许接管自定义设置".to_string());
    }
    if (*initial.get(&0x7A5).unwrap_or(&0) & 0x08) != 0 {
        return Err("EC 处于高温风扇安全保护状态，暂停自定义调校".to_string());
    }
    if !unmanaged_gpu_clean(ec)? {
        return Err("已有外来自定义GPU请求，拒绝接管".to_string());
    }

    let mut masks: BTreeMap<u16, u8> = BTreeMap::new();
    for &(a, m) in BASE_OWN_MASKS {
        masks.insert(a, m);
    }
    if has_gpu {
        for &(a, m) in GPU_OWN_MASKS {
            masks.insert(a, m);
        }
    }
    for a in 0xF00u16..=0xF5F {
        masks.insert(a, 255);
    }

    let mut before: BTreeMap<u16, u8> = BTreeMap::new();
    for &a in masks.keys() {
        before.insert(a, ec.read(a)?);
    }
    let mut target = before.clone();
    for (a, mask) in [
        (0x726u16, 128u8),
        (0x727, 64),
        (0x741, 1),
        (0x7C5, 128),
        (0x7C6, 4),
    ] {
        *target.get_mut(&a).unwrap() |= mask;
    }
    target.insert(REG_MODE, 0);
    target.insert(0x783, config["pl1"].as_i64().unwrap() as u8);
    target.insert(0x784, config["pl2"].as_i64().unwrap() as u8);
    if has_gpu {
        let boost = config.get("gpu_db").and_then(|v| v.as_i64()).unwrap_or(0) as u8;
        let ctgp = config["gpu_ctgp"].as_i64().unwrap() as u8;
        target.insert(0x743, (before[&0x743] & !7) | if boost != 0 { 7 } else { 5 });
        target.insert(0x744, ctgp - 65);
        target.insert(0x745, if boost != 0 { 255 } else { 0 });
        target.insert(0x746, boost);
    }
    let fan_table = make_fan_table(config)?;
    for (i, &val) in fan_table.iter().enumerate() {
        target.insert(0xF00 + i as u16, val);
    }

    if snapshot(ec)? != initial {
        return Err("硬件状态变化，请重试".to_string());
    }
    for (&a, &v) in &before {
        if ec.read(a)? != v {
            return Err("硬件状态变化，请重试".to_string());
        }
    }

    let mut confirmed: BTreeMap<u16, u8> = BTreeMap::new();
    for (&a, &v) in &before {
        confirmed.insert(a, v & masks[&a]);
    }
    let mut state = TuningState {
        version: 2,
        phase: "preparing".to_string(),
        config: config.clone(),
        has_gpu,
        before,
        target: target.clone(),
        confirmed,
        pending: None,
        relinquished: Vec::new(),
        legacy_ambiguous: false,
    };
    save_tuning_state(state_path, &state)?;

    let mut safety_triggered = false;
    let res = (|| -> Result<(), String> {
        let mut put = |ec: &mut E, state: &mut TuningState, a: u16, v: u8| -> Result<(), String> {
            if fan_safety_protect_ec(ec)? {
                safety_triggered = true;
                return Err("EC 触发高温风扇安全保护，撤销自定义调校".to_string());
            }
            if has_gpu && !gpu_round_adapter(ec)? {
                return Err("圆口电源失去，撤销GPU调校".to_string());
            }
            tuning_put(ec, state_path, state, a, v, false, false)
        };
        for a in [0x727u16, REG_MODE, 0x726] {
            put(ec, &mut state, a, target[&a])?;
        }
        for a in [0x741u16, 0x783, 0x784, 0x7C5] {
            put(ec, &mut state, a, target[&a])?;
        }
        if has_gpu {
            for a in [0x744u16, 0x745, 0x746, 0x743] {
                put(ec, &mut state, a, target[&a])?;
            }
        }
        for a in 0xF00u16..=0xF5F {
            put(ec, &mut state, a, target[&a])?;
        }
        put(ec, &mut state, 0x7C6, target[&0x7C6])?;
        if !pause.is_zero() {
            thread::sleep(pause);
        }
        if has_gpu && !gpu_round_adapter(ec)? {
            return Err("圆口电源失去，撤销GPU调校".to_string());
        }
        if !tuning_matches(ec, &state)? {
            return Err("自定义设置回读不一致".to_string());
        }
        state.phase = "active".to_string();
        save_tuning_state(state_path, &state)?;
        Ok(())
    })();

    if let Err(err) = res {
        if let Err(rec_err) = restore_tuning(ec, state_path, safety_triggered) {
            return Err(format!("{err}；恢复尚未完成：{rec_err}"));
        }
        return Err(err);
    }
    Ok(())
}

// ----------------------------------------------------------------- CLI entrypoint

pub fn run_cli(args: &[String]) -> Result<Value, String> {
    let usage = "用法：status | set office|balanced|turbo|custom | apply JSON | reset | logo-status | logo on|off|JSON | turbo-status | turbo quiet|extreme|off | lc-status";
    let is_valid = match args.len() {
        1 => matches!(
            args[0].as_str(),
            "status" | "reset" | "reconcile" | "logo-status" | "turbo-status" | "lc-status"
        ),
        2 => match args[0].as_str() {
            "set" => matches!(args[1].as_str(), "office" | "balanced" | "turbo" | "custom"),
            "apply" | "logo" => true,
            "turbo" => matches!(args[1].as_str(), "quiet" | "extreme" | "off"),
            _ => false,
        },
        _ => false,
    };
    if !is_valid {
        return Err(usage.to_string());
    }

    let cmd = args[0].as_str();
    let apply_cfg: Option<Value> = if cmd == "apply" {
        if args[1].len() >= 4096 {
            return Err("设置过长".to_string());
        }
        let parsed: Value = serde_json::from_str(&args[1]).map_err(|e| format!("JSON 无效: {e}"))?;
        validate_tuning_config(&parsed)?;
        let guard_check = run_command_timeout(
            "/usr/bin/systemctl",
            &["is-active", "--quiet", "mechrevo-profile-guard.service"],
            Duration::from_secs(2),
        )?;
        if guard_check.code != 0 {
            return Err("恢复守护服务未运行，拒绝应用自定义设置".to_string());
        }
        Some(parsed)
    } else {
        None
    };

    let logo_cfg: Option<LogoConfig> = if cmd == "logo" {
        if args[1].len() > 1024 {
            return Err("Logo 设置过长".to_string());
        }
        if args[1] != "on" && args[1] != "off" {
            let parsed: Value =
                serde_json::from_str(&args[1]).map_err(|e| format!("Logo JSON 无效: {e}"))?;
            Some(validate_logo(&parsed)?)
        } else {
            None
        }
    } else {
        None
    };

    let state_path = Path::new(TUNING_STATE_PATH);
    if cmd == "reconcile" && !state_path.exists() {
        return Ok(json!({"idle": true}));
    }

    check_machine()?;

    if cmd == "turbo-status" {
        return turbo_request(&json!({"command": "status"}), true);
    }
    if cmd == "turbo" {
        let req = if args[1] == "off" {
            json!({"command": "release"})
        } else {
            json!({"command": "select", "variant": args[1]})
        };
        let _ = turbo_request(&req, false)?;
        return run_cli(&["status".to_string()]);
    }

    let turbo: Option<Value> = if matches!(cmd, "set" | "apply" | "reset") {
        let released = turbo_request(&json!({"command": "release"}), true)?;
        if released.get("active").and_then(|v| v.as_bool()) == Some(true)
            || released.get("recovery_needed").and_then(|v| v.as_bool()) == Some(true)
        {
            return Err("狂暴调度尚未释放，拒绝切换或调校".to_string());
        }
        Some(released)
    } else if cmd == "status" {
        Some(turbo_request(&json!({"command": "status"}), true)?)
    } else {
        None
    };

    let writable = !matches!(cmd, "status" | "logo-status" | "lc-status");
    let background = cmd == "reconcile";

    run_with_lock(writable, background, |ec| {
        match cmd {
            "lc-status" => Ok(watercooling_status(&snapshot(ec)?)),
            "logo-status" => logo_status(ec),
            "logo" => {
                let target = match logo_cfg {
                    Some(c) => c,
                    None => {
                        let cur = logo_status(ec)?;
                        let cur_mode = cur["mode"].as_str().unwrap_or("solid").to_string();
                        let cur_br = cur["brightness"].as_u64().unwrap_or(0) as u8;
                        let mut cur_col = [
                            cur["color"][0].as_u64().unwrap_or(0) as u8,
                            cur["color"][1].as_u64().unwrap_or(0) as u8,
                            cur["color"][2].as_u64().unwrap_or(0) as u8,
                        ];
                        let next_br = if args[1] == "on" {
                            if cur_br > 0 { cur_br } else { 25 }
                        } else {
                            0
                        };
                        if args[1] == "on" && cur_col == [0, 0, 0] {
                            cur_col = [255, 255, 255];
                        }
                        LogoConfig {
                            mode: cur_mode,
                            brightness: next_br,
                            color: cur_col,
                        }
                    }
                };
                set_logo(ec, &target, Duration::from_millis(150))
            }
            "apply" => {
                apply_tuning(
                    ec,
                    state_path,
                    apply_cfg.as_ref().unwrap(),
                    Duration::from_millis(250),
                    true,
                )?;
                status_with_turbo(ec, state_path, turbo.as_ref().unwrap())
            }
            "set" => {
                restore_tuning(ec, state_path, false)?;
                set_mode(ec, &args[1], Duration::from_millis(250))?;
                status_with_turbo(ec, state_path, turbo.as_ref().unwrap())
            }
            "reset" => {
                restore_tuning(ec, state_path, false)?;
                status_with_turbo(ec, state_path, turbo.as_ref().unwrap())
            }
            "reconcile" => {
                reconcile_ec(ec, state_path, false)?;
                status(ec, state_path)
            }
            "status" => status_with_turbo(ec, state_path, turbo.as_ref().unwrap()),
            _ => Err(usage.to_string()),
        }
    })
    .map_err(|e| e.to_string())
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::ec_guard::REGISTERS;
    use std::fs;

    struct MockEc {
        vals: BTreeMap<u16, u8>,
    }

    impl MockEc {
        fn new() -> Self {
            let mut vals = BTreeMap::new();
            for &r in REGISTERS {
                vals.insert(r, 0);
            }
            for &(a, _) in LOGO_MASKS {
                vals.insert(a, 0);
            }
            vals.insert(0x78C, 0);
            for a in 0xF00u16..=0xF5F {
                vals.insert(a, 0);
            }
            vals.insert(0x706, 0x40);
            vals.insert(REG_MODE, 0x30);
            vals.insert(0x7AB, 0x81); // logo supported, level=4 (100%)
            vals.insert(0x767, 0x80); // rainbow
            vals.insert(0x7CC, 0x81); // round adapter
            Self { vals }
        }
    }

    impl EcIo for MockEc {
        fn read(&self, addr: u16) -> Result<u8, String> {
            Ok(*self.vals.get(&addr).unwrap_or(&0))
        }
        fn write_mode(&mut self, value: u8) -> Result<(), String> {
            self.vals.insert(REG_MODE, value);
            Ok(())
        }
        fn write_flag(&mut self, addr: u16, enabled: bool) -> Result<(), String> {
            let mask = match addr {
                0x726 => 0x80,
                0x727 => 0x40,
                _ => return Err("bad flag".into()),
            };
            let cur = *self.vals.get(&addr).unwrap_or(&0);
            self.vals
                .insert(addr, (cur & !mask) | if enabled { mask } else { 0 });
            Ok(())
        }
        fn write_tuning(&mut self, addr: u16, value: u8) -> Result<(), String> {
            self.vals.insert(addr, value);
            Ok(())
        }
        fn write_logo(&mut self, addr: u16, value: u8) -> Result<(), String> {
            let mask = logo_mask(addr).ok_or("bad logo reg")?;
            let cur = *self.vals.get(&addr).unwrap_or(&0);
            self.vals.insert(addr, (cur & !mask) | value);
            Ok(())
        }
    }

    #[test]
    fn logo_read_and_write_roundtrip() {
        let mut ec = MockEc::new();
        let st = logo_status(&ec).unwrap();
        assert_eq!(st, json!({"logo_enabled": true, "brightness": 100, "mode": "rainbow", "color": [0, 0, 0]}));

        let cfg = validate_logo(&json!({"mode": "solid", "brightness": 50, "color": [0, 255, 255]})).unwrap();
        let after = set_logo(&mut ec, &cfg, Duration::ZERO).unwrap();
        assert_eq!(after["mode"], "solid");
        assert_eq!(after["brightness"], 50);
        assert_eq!(after["color"], json!([0, 255, 255]));

        let off_cfg = validate_logo(&json!({"mode": "solid", "brightness": 0, "color": [0, 255, 255]})).unwrap();
        let off = set_logo(&mut ec, &off_cfg, Duration::ZERO).unwrap();
        assert_eq!(off["logo_enabled"], false);
        assert_eq!(off["brightness"], 0);
        // Solid color registers preserved when turning off
        assert_eq!(off["color"], json!([0, 255, 255]));
    }

    #[test]
    fn apply_and_restore_tuning_roundtrip() {
        let dir = std::env::temp_dir().join(format!("ph-test-{}", std::process::id()));
        let _ = fs::remove_dir_all(&dir);
        fs::create_dir_all(&dir).unwrap();
        let state_path = dir.join("tuning.json");

        let mut ec = MockEc::new();
        let cfg = json!({
            "pl1": 55,
            "pl2": 55,
            "cpu_fan": [40, 40, 45, 50, 55, 60, 65, 75, 85, 95, 100],
            "gpu_fan": [40, 40, 40, 40, 50, 55, 60, 70, 80, 90, 100],
            "gpu_ctgp": 100,
            "gpu_db": 15
        });
        apply_tuning(&mut ec, &state_path, &cfg, Duration::ZERO, false).unwrap();
        assert!(state_path.exists());
        assert_eq!(ec.read(REG_MODE).unwrap(), 0);
        assert_eq!(ec.read(0x783).unwrap(), 55);
        assert_eq!(ec.read(0x744).unwrap(), 35); // 100 - 65
        assert_eq!(ec.read(0x746).unwrap(), 15);

        let st = status_with_gpu_check(&ec, &state_path, Ok(())).unwrap();
        assert_eq!(st["mode"], "custom");
        assert_eq!(st["tuning_active"], true);
        assert_eq!(st["tuning"], cfg);

        restore_tuning(&mut ec, &state_path, false).unwrap();
        assert!(!state_path.exists());
        assert_eq!(ec.read(REG_MODE).unwrap(), 0x30);
        assert_eq!(ec.read(0x783).unwrap(), 0);
        assert_eq!(ec.read(0x743).unwrap() & 7, 0);

        let _ = fs::remove_dir_all(&dir);
    }
}

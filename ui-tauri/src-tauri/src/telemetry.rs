// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Non-root live hardware telemetry reader for the Home Dashboard & System Monitor.
//! Strictly read-only: never writes to HID, EC, sysfs, or BLE devices.

use serde_json::{json, Value};
use std::ffi::CString;
use std::fs;
use std::path::Path;
use std::process::Stdio;
use std::sync::{Mutex, OnceLock};
use std::time::Duration;
use tokio::process::Command;

static PREV_CPU_TIMES: OnceLock<Mutex<Option<(u64, u64)>>> = OnceLock::new();
static CPU_MODEL_CACHE: OnceLock<String> = OnceLock::new();
static CPU_CORES_CACHE: OnceLock<usize> = OnceLock::new();
static CPU_MAX_MHZ_CACHE: OnceLock<u32> = OnceLock::new();

fn read_cpu_model() -> String {
    CPU_MODEL_CACHE
        .get_or_init(|| {
            if let Ok(content) = fs::read_to_string("/proc/cpuinfo") {
                for line in content.lines() {
                    if let Some(rest) = line.strip_prefix("model name") {
                        if let Some((_, val)) = rest.split_once(':') {
                            let clean = val.trim();
                            if !clean.is_empty() {
                                return clean.to_string();
                            }
                        }
                    }
                }
            }
            "Unknown Processor".to_string()
        })
        .clone()
}

fn read_cpu_cores() -> usize {
    *CPU_CORES_CACHE.get_or_init(|| {
        fs::read_to_string("/proc/cpuinfo")
            .map(|c| c.lines().filter(|l| l.starts_with("processor\t:") || l.starts_with("processor :")).count())
            .unwrap_or(1)
            .max(1)
    })
}

fn read_cpu_max_mhz() -> u32 {
    *CPU_MAX_MHZ_CACHE.get_or_init(|| {
        let mut best_khz: u64 = 0;
        if let Ok(entries) = fs::read_dir("/sys/devices/system/cpu") {
            for ent in entries.flatten() {
                let name = ent.file_name();
                let s = name.to_string_lossy();
                if s.starts_with("cpu") && s[3..].chars().all(|c| c.is_ascii_digit()) {
                    let p = ent.path().join("cpufreq/cpuinfo_max_freq");
                    if let Ok(raw) = fs::read_to_string(p) {
                        if let Ok(khz) = raw.trim().parse::<u64>() {
                            best_khz = best_khz.max(khz);
                        }
                    }
                }
            }
        }
        (best_khz / 1000) as u32
    })
}

fn read_cpu_cur_mhz() -> Option<u32> {
    let mut sum_khz: u64 = 0;
    let mut count: u64 = 0;
    let entries = fs::read_dir("/sys/devices/system/cpu").ok()?;
    for ent in entries.flatten() {
        let name = ent.file_name();
        let s = name.to_string_lossy();
        if s.starts_with("cpu") && s[3..].chars().all(|c| c.is_ascii_digit()) {
            let p = ent.path().join("cpufreq/scaling_cur_freq");
            if let Ok(raw) = fs::read_to_string(p) {
                if let Ok(khz) = raw.trim().parse::<u64>() {
                    sum_khz += khz;
                    count += 1;
                }
            }
        }
    }
    if count > 0 {
        Some(((sum_khz / count) / 1000) as u32)
    } else {
        None
    }
}

fn parse_proc_stat_totals(content: &str) -> Option<(u64, u64)> {
    let first = content.lines().next()?;
    if !first.starts_with("cpu ") {
        return None;
    }
    let nums: Vec<u64> = first
        .split_whitespace()
        .skip(1)
        .filter_map(|s| s.parse::<u64>().ok())
        .collect();
    if nums.len() < 4 {
        return None;
    }
    let idle = nums[3] + nums.get(4).copied().unwrap_or(0);
    let total: u64 = nums.iter().sum();
    Some((idle, total))
}

fn compute_cpu_usage(prev: Option<(u64, u64)>, curr: (u64, u64)) -> Option<f64> {
    let (p_idle, p_total) = prev?;
    let (c_idle, c_total) = curr;
    let dt = c_total.saturating_sub(p_total);
    let di = c_idle.saturating_sub(p_idle);
    if dt == 0 {
        return Some(0.0);
    }
    let busy = dt.saturating_sub(di) as f64;
    Some(((busy / dt as f64) * 100.0).clamp(0.0, 100.0))
}

fn read_cpu_usage_pct() -> f64 {
    let curr = fs::read_to_string("/proc/stat")
        .ok()
        .and_then(|s| parse_proc_stat_totals(&s));
    let lock = PREV_CPU_TIMES.get_or_init(|| Mutex::new(None));
    if let (Some(c), Ok(mut guard)) = (curr, lock.lock()) {
        let pct = compute_cpu_usage(*guard, c);
        *guard = Some(c);
        if let Some(v) = pct {
            return (v * 10.0).round() / 10.0;
        }
    }
    // First-sample quick estimate from 1-min loadavg
    if let Ok(la) = fs::read_to_string("/proc/loadavg") {
        if let Some(v) = la.split_whitespace().next().and_then(|s| s.parse::<f64>().ok()) {
            let cores = read_cpu_cores() as f64;
            return (((v / cores) * 100.0).clamp(1.0, 100.0) * 10.0).round() / 10.0;
        }
    }
    0.0
}

fn read_hwmon_temps() -> (Option<f64>, Option<f64>) {
    let mut cpu_temp: Option<f64> = None;
    let mut fallback_cpu: Option<f64> = None;
    let mut nvme_temp: Option<f64> = None;

    let Ok(entries) = fs::read_dir("/sys/class/hwmon") else {
        return (None, None);
    };
    for ent in entries.flatten() {
        let dir = ent.path();
        let name = fs::read_to_string(dir.join("name"))
            .map(|s| s.trim().to_string())
            .unwrap_or_default();

        if matches!(name.as_str(), "coretemp" | "k10temp" | "zenpower") {
            let mut pkg_val: Option<f64> = None;
            let mut first_val: Option<f64> = None;
            for idx in 1..=16 {
                let input = dir.join(format!("temp{idx}_input"));
                let Ok(raw) = fs::read_to_string(&input) else {
                    continue;
                };
                let Ok(mc) = raw.trim().parse::<f64>() else {
                    continue;
                };
                let c = (mc / 1000.0 * 10.0).round() / 10.0;
                if first_val.is_none() {
                    first_val = Some(c);
                }
                let label = fs::read_to_string(dir.join(format!("temp{idx}_label")))
                    .map(|s| s.trim().to_ascii_lowercase())
                    .unwrap_or_default();
                if label.contains("package") || label.contains("tctl") || label.contains("tdie") {
                    pkg_val = Some(c);
                    break;
                }
            }
            if cpu_temp.is_none() {
                cpu_temp = pkg_val.or(first_val);
            }
        } else if name == "acpitz" && fallback_cpu.is_none() {
            if let Ok(raw) = fs::read_to_string(dir.join("temp1_input")) {
                if let Ok(mc) = raw.trim().parse::<f64>() {
                    fallback_cpu = Some((mc / 1000.0 * 10.0).round() / 10.0);
                }
            }
        } else if name == "nvme" && nvme_temp.is_none() {
            if let Ok(raw) = fs::read_to_string(dir.join("temp1_input")) {
                if let Ok(mc) = raw.trim().parse::<f64>() {
                    nvme_temp = Some((mc / 1000.0 * 10.0).round() / 10.0);
                }
            }
        }
    }
    (cpu_temp.or(fallback_cpu), nvme_temp)
}

fn parse_meminfo(content: &str) -> (f64, f64, f64) {
    let mut total_kb: u64 = 0;
    let mut avail_kb: u64 = 0;
    for line in content.lines() {
        if let Some(rest) = line.strip_prefix("MemTotal:") {
            total_kb = rest.split_whitespace().next().and_then(|s| s.parse().ok()).unwrap_or(0);
        } else if let Some(rest) = line.strip_prefix("MemAvailable:") {
            avail_kb = rest.split_whitespace().next().and_then(|s| s.parse().ok()).unwrap_or(0);
        }
    }
    if total_kb == 0 {
        return (0.0, 0.0, 0.0);
    }
    let used_kb = total_kb.saturating_sub(avail_kb);
    let total_gb = ((total_kb as f64 / 1_048_576.0) * 10.0).round() / 10.0;
    let used_gb = ((used_kb as f64 / 1_048_576.0) * 10.0).round() / 10.0;
    let pct = (((used_kb as f64 / total_kb as f64) * 100.0) * 10.0).round() / 10.0;
    (used_gb, total_gb, pct)
}

fn read_root_storage() -> (f64, f64, f64) {
    let Ok(c_path) = CString::new("/") else {
        return (0.0, 0.0, 0.0);
    };
    let mut st: libc::statvfs = unsafe { std::mem::zeroed() };
    let rc = unsafe { libc::statvfs(c_path.as_ptr(), &mut st) };
    if rc != 0 || st.f_blocks == 0 {
        return (0.0, 0.0, 0.0);
    }
    let frsize = st.f_frsize as f64;
    let total_bytes = (st.f_blocks as f64) * frsize;
    let avail_bytes = (st.f_bavail as f64) * frsize;
    let used_bytes = (total_bytes - avail_bytes).max(0.0);
    let gib = 1024.0 * 1024.0 * 1024.0;
    let total_gb = ((total_bytes / gib) * 10.0).round() / 10.0;
    let used_gb = ((used_bytes / gib) * 10.0).round() / 10.0;
    let pct = (((used_bytes / total_bytes) * 100.0) * 10.0).round() / 10.0;
    (used_gb, total_gb, pct)
}

fn read_power_supply() -> Value {
    let mut ac_online = false;
    let mut battery_pct: Option<u32> = None;
    let mut battery_status = "Unknown".to_string();

    if let Ok(entries) = fs::read_dir("/sys/class/power_supply") {
        for ent in entries.flatten() {
            let p = ent.path();
            let ptype = fs::read_to_string(p.join("type"))
                .map(|s| s.trim().to_string())
                .unwrap_or_default();
            if ptype == "Mains" {
                if fs::read_to_string(p.join("online"))
                    .map(|s| s.trim() == "1")
                    .unwrap_or(false)
                {
                    ac_online = true;
                }
            } else if ptype == "Battery" && battery_pct.is_none() {
                battery_pct = fs::read_to_string(p.join("capacity"))
                    .ok()
                    .and_then(|s| s.trim().parse::<u32>().ok());
                if let Ok(st) = fs::read_to_string(p.join("status")) {
                    battery_status = st.trim().to_string();
                }
            }
        }
    }

    json!({
        "ac_online": ac_online,
        "battery_pct": battery_pct,
        "battery_status": battery_status,
    })
}

fn parse_opt_f64(field: &str) -> Option<f64> {
    let clean = field.trim();
    if clean.is_empty() || clean.contains("N/A") {
        return None;
    }
    clean.parse::<f64>().ok()
}

fn parse_nvidia_smi_line(line: &str) -> Option<Value> {
    let parts: Vec<&str> = line.split(',').map(|s| s.trim()).collect();
    if parts.len() < 7 {
        return None;
    }
    Some(json!({
        "available": true,
        "name": parts[0],
        "temp_c": parse_opt_f64(parts[1]),
        "usage_pct": parse_opt_f64(parts[2]),
        "power_w": parse_opt_f64(parts[3]),
        "clock_mhz": parse_opt_f64(parts[4]).map(|v| v.round() as u32),
        "vram_used_mb": parse_opt_f64(parts[5]).map(|v| v.round() as u32),
        "vram_total_mb": parse_opt_f64(parts[6]).map(|v| v.round() as u32),
    }))
}

async fn read_gpu_telemetry() -> Value {
    let mut cmd = Command::new("nvidia-smi");
    cmd.args([
        "--query-gpu=name,temperature.gpu,utilization.gpu,power.draw,clocks.current.graphics,memory.used,memory.total",
        "--format=csv,noheader,nounits",
    ])
    .stdin(Stdio::null())
    .stdout(Stdio::piped())
    .stderr(Stdio::null())
    .kill_on_drop(true);

    let out = tokio::time::timeout(Duration::from_millis(1200), cmd.output()).await;
    if let Ok(Ok(res)) = out {
        if res.status.success() {
            let stdout = String::from_utf8_lossy(&res.stdout);
            if let Some(first) = stdout.lines().find(|l| !l.trim().is_empty()) {
                if let Some(parsed) = parse_nvidia_smi_line(first) {
                    return parsed;
                }
            }
        }
    }
    json!({
        "available": false,
        "name": "独立显卡休眠 / 未就绪",
        "temp_c": null,
        "usage_pct": null,
        "power_w": null,
        "clock_mhz": null,
        "vram_used_mb": null,
        "vram_total_mb": null,
    })
}

fn read_ec_cooler_telemetry() -> Value {
    if let Ok(raw) = fs::read_to_string(Path::new("/run/mechrevo-watercooler-ec.json")) {
        if let Ok(v) = serde_json::from_str::<Value>(&raw) {
            return v;
        }
    }
    Value::Null
}

#[tauri::command]
pub async fn get_live_telemetry() -> Result<Value, String> {
    let gpu_fut = read_gpu_telemetry();

    let cpu_model = read_cpu_model();
    let cpu_cores = read_cpu_cores();
    let cpu_max_mhz = read_cpu_max_mhz();
    let cpu_cur_mhz = read_cpu_cur_mhz();
    let cpu_usage = read_cpu_usage_pct();
    let (cpu_temp, nvme_temp) = read_hwmon_temps();

    let (mem_used_gb, mem_total_gb, mem_pct) = fs::read_to_string("/proc/meminfo")
        .map(|s| parse_meminfo(&s))
        .unwrap_or((0.0, 0.0, 0.0));
    let (disk_used_gb, disk_total_gb, disk_pct) = read_root_storage();
    let power = read_power_supply();
    let ec_cooler = read_ec_cooler_telemetry();
    let gpu = gpu_fut.await;

    Ok(json!({
        "cpu": {
            "model": cpu_model,
            "usage_pct": cpu_usage,
            "temp_c": cpu_temp,
            "freq_mhz": cpu_cur_mhz,
            "max_freq_mhz": cpu_max_mhz,
            "cores": cpu_cores,
        },
        "gpu": gpu,
        "memory": {
            "used_gb": mem_used_gb,
            "total_gb": mem_total_gb,
            "usage_pct": mem_pct,
        },
        "storage": {
            "used_gb": disk_used_gb,
            "total_gb": disk_total_gb,
            "usage_pct": disk_pct,
            "nvme_temp_c": nvme_temp,
        },
        "power": power,
        "ec_cooler": ec_cooler,
    }))
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parses_proc_stat_and_computes_usage() {
        let s1 = "cpu  100 0 100 800 0 0 0 0 0 0\ncpu0 50 0 50 400";
        let s2 = "cpu  150 0 150 900 0 0 0 0 0 0\ncpu0 75 0 75 450";
        let t1 = parse_proc_stat_totals(s1).unwrap();
        let t2 = parse_proc_stat_totals(s2).unwrap();
        let pct = compute_cpu_usage(Some(t1), t2).unwrap();
        assert!((pct - 50.0).abs() < 0.01);
    }

    #[test]
    fn parses_meminfo_values() {
        let sample = "MemTotal:       33554432 kB\nMemFree:         2000000 kB\nMemAvailable:   16777216 kB\n";
        let (used, total, pct) = parse_meminfo(sample);
        assert!((total - 32.0).abs() < 0.1);
        assert!((used - 16.0).abs() < 0.1);
        assert!((pct - 50.0).abs() < 0.1);
    }

    #[test]
    fn parses_nvidia_smi_csv_with_na() {
        let line = "NVIDIA GeForce RTX 5070 Ti Laptop GPU, 30, 12, 18.50, 1450, 190, 12227";
        let v = parse_nvidia_smi_line(line).unwrap();
        assert_eq!(v["available"], json!(true));
        assert_eq!(v["temp_c"], json!(30.0));
        assert_eq!(v["power_w"], json!(18.5));
        assert_eq!(v["vram_total_mb"], json!(12227));

        let na = "RTX, [N/A], 0, [N/A], 210, 100, 12227";
        let v2 = parse_nvidia_smi_line(na).unwrap();
        assert!(v2["temp_c"].is_null());
        assert!(v2["power_w"].is_null());
    }
}

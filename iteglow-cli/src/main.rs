// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Zero-Python native Rust CLI for MECHREVO Linux Control (`iteglow`).

mod hid;
mod zone_lock;

use crate::hid::{
    apply_and_save_hid, available_devices, build_reports, decode_keyboard_mode, hidraw_for_label,
    open_hidraw, parse_color_str, parse_fw_version, parse_keyboard_state, parse_matrix_spec,
    query_raw, resolve_direction, send_reports, KEYBOARD_MODES, KNOWN_DEVICES, LIGHTBAR_MODES,
    PRESET_NAMES, QUERY_EFFECT, QUERY_FW_VERSION,
};
use crate::zone_lock::{
    config_dir, read_json_file, read_user_state_for_pin, runtime_dir, state_dir,
    user_home_for_uid, user_info_for_name, write_json_file, ZoneGuard, MAX_PROFILE_BYTES,
    MAX_STATE_BYTES, SYSTEM_STATE_PATH,
};
use serde_json::{json, Map, Value};
use std::fs;
use std::io::{BufRead, BufReader, Read, Write};
use std::os::unix::net::UnixStream;
use std::os::unix::process::CommandExt;
use std::path::{Path, PathBuf};
use std::process::{Command, ExitCode, Stdio};
use std::time::Duration;

const VERSION: &str = env!("CARGO_PKG_VERSION");
const PKEXEC: &str = "/usr/bin/pkexec";
const PROFILE_HELPER: &str = "/usr/local/libexec/mechrevo-profile-helper";

fn usage_main() -> &'static str {
    "Usage: iteglow [--version] <command> [args]\n\n\
Commands:\n  \
  doctor       check drivers, devices and permissions\n  \
  list         list devices, lighting modes and matrix presets\n  \
  set          apply a mode to keyboard or lightbar\n  \
  off          turn a device (or all devices) off\n  \
  restore      re-apply the last saved state [--system]\n  \
  pin          export saved user state to /etc/iteglow/state.json (sudo)\n  \
  profile      manage performance+lighting profiles and app rules\n  \
  cooler       control external BLE water cooler (LCT22002 / LCT21001)\n  \
  gui          launch desktop control center (Tauri 2; --gtk for legacy GTK4)"
}

fn user_lighting_only() -> Result<(), String> {
    if unsafe { libc::geteuid() } == 0 {
        return Err(
            "Run lighting commands as your normal desktop user, without sudo. \
The installed udev rule grants HID access; user-session locks must share that identity. \
Only \"iteglow pin\" needs sudo for exporting state."
                .to_string(),
        );
    }
    Ok(())
}

fn run_profile_helper(args: &[&str]) -> Result<Value, String> {
    let out = Command::new(PKEXEC)
        .arg("--disable-internal-agent")
        .arg(PROFILE_HELPER)
        .args(args)
        .stdin(Stdio::null())
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .output()
        .map_err(|e| format!("failed to run hardware helper: {e}"))?;
    let stdout = String::from_utf8_lossy(&out.stdout).trim().to_string();
    let stderr = String::from_utf8_lossy(&out.stderr).trim().to_string();
    let parsed: Result<Value, _> = serde_json::from_str(&stdout);
    if !out.status.success() {
        let msg = parsed
            .ok()
            .and_then(|v| v.get("error").and_then(|e| e.as_str()).map(String::from))
            .filter(|s| !s.is_empty())
            .or((!stderr.is_empty()).then_some(stderr))
            .or((!stdout.is_empty()).then_some(stdout))
            .unwrap_or_else(|| "hardware helper failed or unauthorized".to_string());
        return Err(msg);
    }
    let data = parsed.map_err(|_| {
        if !stderr.is_empty() {
            stderr
        } else {
            "hardware helper returned invalid JSON".to_string()
        }
    })?;
    if let Some(err) = data.get("error").and_then(|v| v.as_str()) {
        return Err(err.to_string());
    }
    Ok(data)
}

// ----------------------------------------------------------------- doctor & list

fn cmd_doctor() -> Result<bool, String> {
    println!("iteglow {VERSION} (MECHREVO 原生 Rust CLI)\n");
    println!("本机使用已有 hid-generic/hidraw 接口；不要求加载 tuxedo-drivers。");
    let mods = fs::read_to_string("/proc/modules").unwrap_or_default();
    for m in ["ite_8291", "ite_8291_lb"] {
        let st = if mods.contains(m) {
            "loaded"
        } else {
            "NOT loaded"
        };
        println!("  module {m:12}: {st}");
    }
    println!();
    let mut ok = true;
    for &(led, usb, label) in KNOWN_DEVICES {
        let Some(path) = hidraw_for_label(label) else {
            println!("  {label:9} ({usb}): not found  (LED node {led} missing)");
            ok = false;
            continue;
        };
        let c_path = std::ffi::CString::new(path.as_str()).unwrap();
        let access = unsafe { libc::access(c_path.as_ptr(), libc::R_OK | libc::W_OK) } == 0;
        let mut extra = String::new();
        if access {
            let _guard = if unsafe { libc::geteuid() } != 0 {
                ZoneGuard::acquire(label).ok()
            } else {
                None
            };
            if unsafe { libc::geteuid() } == 0 || _guard.is_some() {
                if let Ok(mut f) = open_hidraw(label) {
                    if let Ok(raw_fw) = query_raw(&mut f, QUERY_FW_VERSION) {
                        let fw = parse_fw_version(&raw_fw);
                        extra.push_str(&format!("  fw {}.{}", fw.0, fw.1));
                    }
                    if label == "keyboard" {
                        if let Ok(raw_eff) = query_raw(&mut f, QUERY_EFFECT) {
                            let hw = parse_keyboard_state(&raw_eff);
                            let onoff = if hw.power == 0x01 || hw.brightness == 0 {
                                "off"
                            } else {
                                "on"
                            };
                            let (mode_name, _) = decode_keyboard_mode(hw, None);
                            extra.push_str(&format!(
                                "  [{onoff}, {mode_name} 0x{:02x}, brightness {}]",
                                hw.mode, hw.brightness
                            ));
                        }
                    }
                }
            } else {
                extra.push_str("  [lock busy]");
            }
        }
        let access_str = if access {
            "read/write OK"
        } else {
            "NO ACCESS -- install the udev rule for your desktop user"
        };
        println!("  {label:9} ({usb}): {path}  [{access_str}]{extra}");
        ok = ok && access;
    }
    if !ok {
        println!("\nFixes:");
        println!("  * 本机设备缺失 -> 检查 USB 设备和维护记录，不要为此强装平台驱动");
        println!("  * NO ACCESS   -> sudo install -m644 /usr/lib/udev/rules.d/60-iteglow.rules /etc/udev/rules.d/ && sudo udevadm control --reload");
    }
    Ok(ok)
}

fn cmd_list() -> Result<(), String> {
    let present = available_devices();
    let ktag = if present.contains(&"keyboard") {
        ""
    } else {
        "  (not detected)"
    };
    let ltag = if present.contains(&"lightbar") {
        ""
    } else {
        "  (not detected)"
    };
    let kmodes: Vec<&str> = KEYBOARD_MODES.iter().map(|m| m.name).collect();
    let lmodes: Vec<&str> = LIGHTBAR_MODES.iter().map(|m| m.name).collect();
    println!("keyboard{ktag}: {}", kmodes.join(", "));
    println!("lightbar{ltag}: {}", lmodes.join(", "));
    println!("matrix presets: {}", PRESET_NAMES.join(", "));
    Ok(())
}

// ----------------------------------------------------------------- set / off / restore / pin

fn cmd_set(args: &[String]) -> Result<(), String> {
    user_lighting_only()?;
    if args.len() < 2 {
        return Err("Usage: iteglow set <keyboard|lightbar> <mode> [--color C] [--palette C...] [--brightness N] [--speed N] [--reactive] [--direction D] [--preset P] [--matrix JSON|@FILE] [--calibrate]".to_string());
    }
    let device = args[0].as_str();
    if device != "keyboard" && device != "lightbar" {
        return Err(format!("unknown device {device:?}; choose keyboard or lightbar"));
    }
    let mode = args[1].as_str();
    let mut color = [0u8, 255, 255];
    let mut palette: Option<Vec<[u8; 3]>> = None;
    let mut brightness: Option<i64> = None;
    let mut speed: i64 = 5;
    let mut reactive = false;
    let mut direction: u8 = 1;
    let mut preset: Option<String> = None;
    let mut matrix: Option<Vec<[u8; 3]>> = None;
    let mut calibrate = false;

    let mut i = 2usize;
    while i < args.len() {
        match args[i].as_str() {
            "--color" => {
                i += 1;
                let v = args.get(i).ok_or("--color requires a value")?;
                color = parse_color_str(v)?;
            }
            "--palette" => {
                let mut cols = Vec::new();
                while i + 1 < args.len() && !args[i + 1].starts_with("--") {
                    i += 1;
                    cols.push(parse_color_str(&args[i])?);
                }
                if cols.is_empty() {
                    return Err("--palette requires at least one color".to_string());
                }
                palette = Some(cols);
            }
            "--brightness" => {
                i += 1;
                let v = args.get(i).ok_or("--brightness requires an integer")?;
                brightness = Some(v.parse::<i64>().map_err(|_| "invalid --brightness")?);
            }
            "--speed" => {
                i += 1;
                let v = args.get(i).ok_or("--speed requires an integer")?;
                speed = v.parse::<i64>().map_err(|_| "invalid --speed")?;
            }
            "--reactive" => reactive = true,
            "--direction" => {
                i += 1;
                let v = args.get(i).ok_or("--direction requires a value")?;
                direction = resolve_direction(device, v)?;
            }
            "--preset" => {
                i += 1;
                let v = args.get(i).ok_or("--preset requires a value")?;
                if !PRESET_NAMES.contains(&v.as_str()) {
                    return Err(format!("unknown matrix preset {v:?}"));
                }
                preset = Some(v.clone());
            }
            "--matrix" => {
                i += 1;
                let v = args.get(i).ok_or("--matrix requires JSON or file")?;
                matrix = Some(parse_matrix_spec(v)?);
            }
            "--calibrate" => calibrate = true,
            other => return Err(format!("unknown option for set: {other}")),
        }
        i += 1;
    }

    let mut params = Map::new();
    params.insert("mode".into(), json!(mode));
    params.insert("color".into(), json!(color));
    params.insert(
        "colors".into(),
        match palette {
            Some(c) => json!(c),
            None => Value::Null,
        },
    );
    params.insert(
        "brightness".into(),
        match brightness {
            Some(b) => json!(b),
            None => Value::Null,
        },
    );
    params.insert("speed".into(), json!(speed));
    params.insert("direction".into(), json!(direction));
    params.insert("reactive".into(), json!(reactive));
    if device == "keyboard" && mode == "matrix" {
        if let Some(m) = matrix {
            params.insert("matrix".into(), json!(m));
        } else if let Some(p) = preset {
            params.insert("preset".into(), json!(p));
        }
        if calibrate {
            params.insert("calibrate".into(), json!(true));
        }
    }

    apply_and_save_hid(device, &Value::Object(params), true)
}

fn cmd_off(args: &[String]) -> Result<(), String> {
    user_lighting_only()?;
    let targets: Vec<&str> = if let Some(dev) = args.first() {
        if args.len() > 1 || (dev != "keyboard" && dev != "lightbar") {
            return Err("Usage: iteglow off [keyboard|lightbar]".to_string());
        }
        vec![dev.as_str()]
    } else {
        let av = available_devices();
        if av.is_empty() {
            return Err("no devices detected".to_string());
        }
        av
    };
    for label in targets {
        apply_and_save_hid(label, &json!({"mode": "off"}), true)?;
    }
    Ok(())
}

fn cmd_restore(args: &[String]) -> Result<(), String> {
    user_lighting_only()?;
    let system_mode = match args {
        [] => false,
        [flag] if flag == "--system" => true,
        _ => return Err("Usage: iteglow restore [--system]".to_string()),
    };
    let path = if system_mode {
        PathBuf::from(SYSTEM_STATE_PATH)
    } else {
        state_dir().join("state.json")
    };
    let Some(data) = read_json_file(&path, MAX_STATE_BYTES, !system_mode)? else {
        eprintln!("no saved state at {}", path.display());
        return if system_mode {
            Ok(())
        } else {
            Err(format!("no saved state at {}", path.display()))
        };
    };
    let Some(obj) = data.as_object() else {
        return Err("invalid saved lighting state: expected an object".to_string());
    };
    let present = available_devices();
    let mut restored: Vec<String> = Vec::new();
    let mut invalid = false;

    for (label, params) in obj {
        if !present.contains(&label.as_str()) {
            continue;
        }
        if let Err(err) = build_reports(label, params) {
            eprintln!("invalid saved {label} state: {err}");
            invalid = true;
            continue;
        }
        let _guard = ZoneGuard::acquire(label)?;
        let fresh = read_json_file(&path, MAX_STATE_BYTES, !system_mode)?;
        let Some(fresh_params) = fresh
            .as_ref()
            .and_then(|v| v.as_object())
            .and_then(|m| m.get(label))
        else {
            continue;
        };
        let reports = match build_reports(label, fresh_params) {
            Ok(r) => r,
            Err(err) => {
                eprintln!("invalid saved {label} state: {err}");
                invalid = true;
                continue;
            }
        };
        let mut f = open_hidraw(label)?;
        send_reports(&mut f, &reports)?;
        let m = fresh_params
            .get("mode")
            .and_then(|v| v.as_str())
            .unwrap_or("off");
        restored.push(format!("{label}={m}"));
    }
    if restored.is_empty() {
        println!("restored: nothing (no saved devices present)");
    } else {
        println!("restored: {}", restored.join(", "));
    }
    if invalid {
        return Err("one or more saved device entries were invalid".to_string());
    }
    Ok(())
}

fn cmd_pin() -> Result<(), String> {
    if unsafe { libc::geteuid() } != 0 {
        return Err("pin writes the system state; run it with sudo.".to_string());
    }
    let invoker: Option<(u32, PathBuf)> = if let Ok(uid_str) = std::env::var("PKEXEC_UID") {
        let uid: u32 = uid_str.parse().map_err(|_| "invalid PKEXEC_UID")?;
        let home = user_home_for_uid(uid).ok_or("cannot resolve PKEXEC_UID home directory")?;
        Some((uid, home))
    } else if let Ok(user) = std::env::var("SUDO_USER") {
        if !user.is_empty() && user != "root" {
            let info =
                user_info_for_name(&user).ok_or("cannot resolve SUDO_USER account info")?;
            Some(info)
        } else {
            None
        }
    } else {
        None
    };

    let (src_display, data_opt) = match invoker {
        Some((uid, home)) => {
            let disp = home.join(".local/state/iteglow/state.json");
            let data = read_user_state_for_pin(&home, uid, MAX_STATE_BYTES)?;
            (disp, data)
        }
        None => {
            let src = state_dir().join("state.json");
            let data = read_json_file(&src, MAX_STATE_BYTES, true)?;
            (src, data)
        }
    };
    let Some(data) = data_opt else {
        return Err(format!(
            "no saved state at {}; run `iteglow set ...` (or the GUI) first.",
            src_display.display()
        ));
    };
    let Some(obj) = data.as_object() else {
        return Err("invalid saved lighting state".to_string());
    };
    let mut clean = Map::new();
    for (label, params) in obj {
        if label != "keyboard" && label != "lightbar" {
            return Err(format!("unexpected device key {label:?} in saved lighting state"));
        }
        build_reports(label, params)
            .map_err(|e| format!("invalid saved {label} state: {e}"))?;
        clean.insert(label.clone(), params.clone());
    }
    if clean.is_empty() {
        return Err("saved lighting state contains no valid device entries".to_string());
    }
    let mut keys: Vec<String> = clean.keys().cloned().collect();
    keys.sort();
    write_json_file(Path::new(SYSTEM_STATE_PATH), &Value::Object(clean), 0o644)?;
    println!("pinned {} -> {SYSTEM_STATE_PATH}", keys.join(", "));
    println!("Export only; restore it manually with `iteglow restore --system`.");
    println!("Automatic restore uses your user state via iteglow-session-restore.service when configured.");
    Ok(())
}

// ----------------------------------------------------------------- profile commands

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

fn builtin_templates() -> Map<String, Value> {
    let val = json!({
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
    });
    val.as_object().cloned().unwrap_or_default()
}

const MAX_PROFILES: usize = 32;
const MAX_RULES: usize = 32;
const MAX_MATCH_TOKENS: usize = 8;

fn validate_identifier(name: &str, max_len: usize, kind: &str) -> Result<(), String> {
    let trimmed = name.trim();
    if trimmed.is_empty() || trimmed != name || trimmed.chars().count() > max_len {
        return Err(format!("{kind} name must be 1..={max_len} non-whitespace-padded characters"));
    }
    if trimmed.chars().any(|c| c.is_control() || c == '/' || c == '\\') {
        return Err(format!("{kind} name contains forbidden control or path characters"));
    }
    Ok(())
}

fn ensure_profiles_schema(cfg: &mut Value) -> Result<(), String> {
    let Some(root) = cfg.as_object_mut() else {
        return Err("invalid profiles config: expected JSON object".to_string());
    };
    if !root.get("profiles").map(|v| v.is_object()).unwrap_or(false) {
        return Err("invalid profiles config: 'profiles' must be a JSON object".to_string());
    }
    if !root.get("rules").map(|v| v.is_array()).unwrap_or(false) {
        return Err("invalid profiles config: 'rules' must be a JSON array".to_string());
    }
    Ok(())
}

fn cmd_profile(args: &[String]) -> Result<(), String> {
    user_lighting_only()?;
    let Some(action) = args.first().map(|s| s.as_str()) else {
        return Err("Usage: iteglow profile list|init-templates|save|apply|delete|rule-add|rule-del|auto".to_string());
    };
    let path = config_dir().join("profiles.json");
    let mut cfg = read_json_file(&path, MAX_PROFILE_BYTES, true)?
        .unwrap_or_else(default_profiles_config);
    ensure_profiles_schema(&mut cfg)?;

    match action {
        "list" => {
            println!("config: {}", path.display());
            let auto_on = cfg["auto_switch_enabled"].as_bool().unwrap_or(false);
            let restore_prev = cfg["restore_previous"].as_bool().unwrap_or(true);
            let cd = cfg["cooldown_seconds"].as_i64().unwrap_or(5);
            println!(
                "auto_switch: {} (restore_previous={restore_prev}, cooldown={cd}s)",
                if auto_on { "on" } else { "off" }
            );
            let profs = cfg["profiles"].as_object();
            if let Some(map) = profs.filter(|m| !m.is_empty()) {
                println!("profiles:");
                for (name, body) in map {
                    let mut perf = body
                        .get("performance")
                        .and_then(|p| p.get("mode"))
                        .and_then(|v| v.as_str())
                        .unwrap_or("-")
                        .to_string();
                    if let Some(t) = body
                        .get("performance")
                        .and_then(|p| p.get("turbo"))
                        .and_then(|v| v.as_str())
                    {
                        perf = format!("{perf}/{t}");
                    }
                    let mut zones: Vec<&str> = body
                        .get("lighting")
                        .and_then(|l| l.as_object())
                        .map(|m| m.keys().map(|k| k.as_str()).collect())
                        .unwrap_or_default();
                    zones.sort();
                    let zstr = if zones.is_empty() {
                        "-".to_string()
                    } else {
                        zones.join(",")
                    };
                    let lbl = body
                        .get("label")
                        .and_then(|v| v.as_str())
                        .filter(|s| !s.is_empty())
                        .map(|s| format!(" — {s}"))
                        .unwrap_or_default();
                    println!("  {name}: perf={perf} lights={zstr}{lbl}");
                }
            } else {
                println!("profiles: (none — run `iteglow profile init-templates` to add starters)");
            }
            if let Some(rules) = cfg["rules"].as_array().filter(|a| !a.is_empty()) {
                println!("rules:");
                for r in rules {
                    let st = if r["enabled"].as_bool().unwrap_or(false) {
                        "on"
                    } else {
                        "off"
                    };
                    let rname = r["name"].as_str().unwrap_or("?");
                    let prof = r["profile"].as_str().unwrap_or("?");
                    let m: Vec<&str> = r["match"]
                        .as_array()
                        .map(|a| a.iter().filter_map(|v| v.as_str()).collect())
                        .unwrap_or_default();
                    println!("  [{st}] {rname}: match={} -> {prof}", m.join(","));
                }
            }
            Ok(())
        }
        "init-templates" => {
            let mut added = Vec::new();
            let profs = cfg["profiles"]
                .as_object_mut()
                .ok_or("invalid profiles config")?;
            for (k, v) in builtin_templates() {
                if !profs.contains_key(&k) {
                    profs.insert(k.clone(), v);
                    added.push(k);
                }
            }
            write_json_file(&path, &cfg, 0o600)?;
            if added.is_empty() {
                println!("added templates: none (already present)");
            } else {
                println!("added templates: {}", added.join(", "));
            }
            Ok(())
        }
        "save" => {
            let name = args.get(1).ok_or("Usage: iteglow profile save <name> [--label L] [--no-performance] [--no-lighting]")?;
            validate_identifier(name, 48, "profile")?;
            let mut label: Option<String> = None;
            let mut include_perf = true;
            let mut include_light = true;
            let mut i = 2usize;
            while i < args.len() {
                match args[i].as_str() {
                    "--label" => {
                        i += 1;
                        label = Some(args.get(i).ok_or("--label requires value")?.clone());
                    }
                    "--no-performance" => include_perf = false,
                    "--no-lighting" => include_light = false,
                    other => return Err(format!("unknown flag {other}")),
                }
                i += 1;
            }
            let mut bundle = Map::new();
            if let Some(l) = label {
                bundle.insert("label".into(), json!(l));
            }
            if include_perf {
                let st = run_profile_helper(&["status"])?;
                if let Some(mode) = st.get("mode").and_then(|v| v.as_str()) {
                    if matches!(mode, "office" | "balanced" | "turbo" | "custom") {
                        let mut p = Map::new();
                        p.insert("mode".into(), json!(mode));
                        if mode == "turbo" {
                            if st["turbo"]["active"].as_bool() == Some(true) {
                                if let Some(var) = st["turbo"]["variant"].as_str() {
                                    if matches!(var, "quiet" | "extreme") {
                                        p.insert("turbo".into(), json!(var));
                                    }
                                }
                            }
                        } else if mode == "custom"
                            && st["tuning_active"].as_bool() == Some(true)
                            && st["tuning"].is_object()
                        {
                            p.insert("tuning".into(), st["tuning"].clone());
                        }
                        bundle.insert("performance".into(), Value::Object(p));
                    }
                }
            }
            if include_light {
                let mut light = Map::new();
                if let Ok(Some(Value::Object(hid))) =
                    read_json_file(&state_dir().join("state.json"), MAX_STATE_BYTES, true)
                {
                    for z in ["keyboard", "lightbar"] {
                        if let Some(v) = hid.get(z) {
                            light.insert(z.into(), v.clone());
                        }
                    }
                }
                if let Ok(Some(logo @ Value::Object(_))) =
                    read_json_file(&state_dir().join("logo.json"), MAX_STATE_BYTES, true)
                {
                    light.insert("logo".into(), logo);
                }
                if !light.is_empty() {
                    bundle.insert("lighting".into(), Value::Object(light));
                }
            }
            if !bundle.contains_key("performance") && !bundle.contains_key("lighting") {
                return Err("nothing captured".to_string());
            }
            let profs = cfg
                .get_mut("profiles")
                .and_then(|v| v.as_object_mut())
                .ok_or("invalid profiles config")?;
            if !profs.contains_key(name) && profs.len() >= MAX_PROFILES {
                return Err(format!("profile count exceeds maximum ({MAX_PROFILES})"));
            }
            profs.insert(name.clone(), Value::Object(bundle));
            write_json_file(&path, &cfg, 0o600)?;
            println!("saved profile {name:?} -> {}", path.display());
            Ok(())
        }
        "apply" => {
            let name = args.get(1).ok_or("Usage: iteglow profile apply <name> [--lighting-only|--performance-only]")?;
            let mut apply_light = true;
            let mut apply_perf = true;
            for flag in &args[2..] {
                match flag.as_str() {
                    "--lighting-only" => apply_perf = false,
                    "--performance-only" => apply_light = false,
                    other => return Err(format!("unknown flag {other}")),
                }
            }
            let bundle = cfg["profiles"]
                .as_object()
                .and_then(|m| m.get(name))
                .ok_or_else(|| format!("unknown profile {name:?}"))?
                .clone();
            let mut applied_lighting = Vec::new();
            let mut applied_perf: Option<String> = None;
            if apply_light {
                if let Some(light) = bundle.get("lighting").and_then(|v| v.as_object()) {
                    for z in ["keyboard", "lightbar"] {
                        if let Some(zcfg) = light.get(z) {
                            if hidraw_for_label(z).is_some() {
                                apply_and_save_hid(z, zcfg, true)?;
                                applied_lighting.push(z.to_string());
                            }
                        }
                    }
                    if let Some(lcfg) = light.get("logo") {
                        let _g = ZoneGuard::acquire("logo")?;
                        let enc = serde_json::to_string(lcfg).map_err(|e| e.to_string())?;
                        let resp = run_profile_helper(&["logo", &enc])?;
                        let _sg = ZoneGuard::acquire("manual-state")?;
                        let _ = write_json_file(
                            &state_dir().join("logo.json"),
                            &json!({
                                "mode": resp["mode"],
                                "brightness": resp["brightness"],
                                "color": resp["color"],
                            }),
                            0o600,
                        );
                        applied_lighting.push("logo".to_string());
                    }
                }
            }
            if apply_perf {
                if let Some(perf) = bundle.get("performance").and_then(|v| v.as_object()) {
                    let mode = perf
                        .get("mode")
                        .and_then(|v| v.as_str())
                        .ok_or("missing performance mode")?;
                    if mode == "custom" {
                        if let Some(tuning) = perf.get("tuning") {
                            let enc = serde_json::to_string(tuning).map_err(|e| e.to_string())?;
                            run_profile_helper(&["apply", &enc])?;
                        } else {
                            run_profile_helper(&["set", "custom"])?;
                        }
                    } else {
                        run_profile_helper(&["set", mode])?;
                        if mode == "turbo" {
                            if let Some(var) = perf.get("turbo").and_then(|v| v.as_str()) {
                                if let Err(e) = run_profile_helper(&["turbo", var]) {
                                    eprintln!("warning: turbo variant {var:?} failed: {e}");
                                }
                            }
                        }
                    }
                    applied_perf = Some(mode.to_string());
                }
            }
            println!(
                "{}",
                json!({
                    "applied": name,
                    "lighting": applied_lighting,
                    "performance": applied_perf,
                })
            );
            Ok(())
        }
        "delete" => {
            let name = args.get(1).ok_or("Usage: iteglow profile delete <name> [--force]")?;
            let force = args.get(2).map(|s| s == "--force").unwrap_or(false);
            let exists = cfg["profiles"]
                .as_object()
                .map(|m| m.contains_key(name))
                .unwrap_or(false);
            if !exists {
                return Err(format!("unknown profile {name:?}"));
            }
            let refs: Vec<String> = cfg["rules"]
                .as_array()
                .map(|arr| {
                    arr.iter()
                        .filter(|r| r["profile"].as_str() == Some(name.as_str()))
                        .filter_map(|r| r["name"].as_str().map(String::from))
                        .collect()
                })
                .unwrap_or_default();
            if !refs.is_empty() && !force {
                return Err(format!(
                    "profile {name:?} is referenced by rules {refs:?}; pass --force to delete both"
                ));
            }
            cfg["profiles"].as_object_mut().unwrap().remove(name);
            if !refs.is_empty() {
                if let Some(arr) = cfg["rules"].as_array_mut() {
                    arr.retain(|r| r["profile"].as_str() != Some(name.as_str()));
                }
            }
            write_json_file(&path, &cfg, 0o600)?;
            println!("deleted profile {name:?}");
            Ok(())
        }
        "rule-add" => {
            if args.len() < 5 {
                return Err("Usage: iteglow profile rule-add <name> <profile> --match <kw...> [--disabled]".to_string());
            }
            let rname = &args[1];
            validate_identifier(rname, 64, "rule")?;
            let target_prof = &args[2];
            if !cfg
                .get("profiles")
                .and_then(|v| v.as_object())
                .map(|m| m.contains_key(target_prof))
                .unwrap_or(false)
            {
                return Err(format!("unknown profile {target_prof:?}"));
            }
            let mut matches: Vec<String> = Vec::new();
            let mut disabled = false;
            let mut i = 3usize;
            while i < args.len() {
                match args[i].as_str() {
                    "--match" => {
                        while i + 1 < args.len() && !args[i + 1].starts_with("--") {
                            i += 1;
                            let tok = args[i].trim();
                            if tok.is_empty() || tok.len() > 64 {
                                return Err("match token must be 1..=64 characters".to_string());
                            }
                            matches.push(tok.to_string());
                        }
                    }
                    "--disabled" => disabled = true,
                    other => return Err(format!("unknown flag {other}")),
                }
                i += 1;
            }
            if matches.is_empty() || matches.len() > MAX_MATCH_TOKENS {
                return Err(format!("--match requires 1..={MAX_MATCH_TOKENS} process keywords"));
            }
            let rules = cfg
                .get_mut("rules")
                .and_then(|v| v.as_array_mut())
                .ok_or("invalid rules")?;
            rules.retain(|r| r["name"].as_str() != Some(rname.as_str()));
            if rules.len() >= MAX_RULES {
                return Err(format!("rule count exceeds maximum ({MAX_RULES})"));
            }
            rules.push(json!({
                "name": rname,
                "enabled": !disabled,
                "match": matches,
                "profile": target_prof,
            }));
            write_json_file(&path, &cfg, 0o600)?;
            println!("saved rule {rname:?} -> {target_prof:?}");
            Ok(())
        }
        "rule-del" => {
            let rname = args.get(1).ok_or("Usage: iteglow profile rule-del <name>")?;
            let rules = cfg["rules"].as_array_mut().ok_or("invalid rules")?;
            let before = rules.len();
            rules.retain(|r| r["name"].as_str() != Some(rname.as_str()));
            if rules.len() == before {
                return Err(format!("unknown rule {rname:?}"));
            }
            write_json_file(&path, &cfg, 0o600)?;
            println!("deleted rule {rname:?}");
            Ok(())
        }
        "auto" => {
            let state = args.get(1).map(|s| s.as_str()).ok_or("Usage: iteglow profile auto on|off|status [--cooldown N] [--restore|--no-restore]")?;
            if !matches!(state, "on" | "off" | "status") {
                return Err("state must be on, off, or status".to_string());
            }
            let mut cooldown: Option<i64> = None;
            let mut restore: Option<bool> = None;
            let mut i = 2usize;
            while i < args.len() {
                match args[i].as_str() {
                    "--cooldown" => {
                        i += 1;
                        let n = args
                            .get(i)
                            .ok_or("--cooldown requires integer")?
                            .parse::<i64>()
                            .map_err(|_| "invalid cooldown")?;
                        if !(0..=120).contains(&n) {
                            return Err("cooldown must be 0..120".to_string());
                        }
                        cooldown = Some(n);
                    }
                    "--restore" => restore = Some(true),
                    "--no-restore" => restore = Some(false),
                    other => return Err(format!("unknown flag {other}")),
                }
                i += 1;
            }
            if state == "on" || state == "off" {
                cfg["auto_switch_enabled"] = json!(state == "on");
                if let Some(cd) = cooldown {
                    cfg["cooldown_seconds"] = json!(cd);
                }
                if let Some(r) = restore {
                    cfg["restore_previous"] = json!(r);
                }
                write_json_file(&path, &cfg, 0o600)?;
            }
            println!(
                "{}",
                json!({
                    "auto_switch_enabled": cfg["auto_switch_enabled"],
                    "restore_previous": cfg["restore_previous"],
                    "cooldown_seconds": cfg["cooldown_seconds"],
                    "rules": cfg["rules"].as_array().map(|a| a.len()).unwrap_or(0),
                })
            );
            Ok(())
        }
        _ => Err(format!("unknown profile subcommand {action:?}")),
    }
}

// ----------------------------------------------------------------- cooler commands

fn read_host_temp_c() -> Option<f64> {
    if let Ok(entries) = fs::read_dir("/sys/class/hwmon") {
        let mut best: Option<f64> = None;
        for entry in entries.flatten() {
            let p = entry.path();
            let name = fs::read_to_string(p.join("name")).unwrap_or_default();
            let n = name.trim();
            if !matches!(n, "coretemp" | "k10temp" | "zenpower" | "x86_pkg_temp") {
                continue;
            }
            if let Ok(t_str) = fs::read_to_string(p.join("temp1_input")) {
                if let Ok(mc) = t_str.trim().parse::<f64>() {
                    let c = mc / 1000.0;
                    if (0.0..130.0).contains(&c) {
                        best = Some(best.map(|b| b.max(c)).unwrap_or(c));
                    }
                }
            }
        }
        if best.is_some() {
            return best;
        }
    }
    if let Ok(entries) = fs::read_dir("/sys/class/thermal") {
        let mut best: Option<f64> = None;
        for entry in entries.flatten() {
            let p = entry.path();
            let ty = fs::read_to_string(p.join("type")).unwrap_or_default();
            if !ty.trim().contains("pkg") && !ty.trim().contains("cpu") {
                continue;
            }
            if let Ok(t_str) = fs::read_to_string(p.join("temp")) {
                if let Ok(mc) = t_str.trim().parse::<f64>() {
                    let c = mc / 1000.0;
                    if (0.0..130.0).contains(&c) {
                        best = Some(best.map(|b| b.max(c)).unwrap_or(c));
                    }
                }
            }
        }
        return best;
    }
    None
}

fn read_ec_watercooler_state() -> Value {
    let ec = fs::read_to_string("/run/mechrevo-watercooler-ec.json")
        .ok()
        .and_then(|s| serde_json::from_str::<Value>(&s).ok());
    let wmi = fs::read_to_string("/run/mechrevo-wmi-state.json")
        .ok()
        .and_then(|s| serde_json::from_str::<Value>(&s).ok());
    let mut out = Map::new();
    out.insert("ec_available".into(), json!(ec.is_some()));
    if let Some(Value::Object(m)) = ec {
        for (k, v) in m {
            out.insert(k, v);
        }
    }
    out.insert("wmi_available".into(), json!(wmi.is_some()));
    if let Some(Value::Object(m)) = wmi {
        if let Some(v) = m.get("wmi_seq") {
            out.insert("wmi_seq".into(), v.clone());
        }
        if let Some(v) = m.get("wmi_last_code") {
            out.insert("wmi_last_code".into(), v.clone());
        }
    }
    Value::Object(out)
}

fn wc_sock_path() -> PathBuf {
    runtime_dir().join("iteglow-watercooling.sock")
}

fn wc_service_request(req: &Value, timeout_secs: u64) -> Result<Value, String> {
    let sock = wc_sock_path();
    let mut stream = UnixStream::connect(&sock)
        .map_err(|e| format!("无法连接水冷后台服务 ({}): {e}", sock.display()))?;
    let _ = stream.set_read_timeout(Some(Duration::from_secs(timeout_secs)));
    let _ = stream.set_write_timeout(Some(Duration::from_secs(timeout_secs)));
    let mut line = serde_json::to_string(req).map_err(|e| e.to_string())?;
    line.push('\n');
    stream
        .write_all(line.as_bytes())
        .map_err(|e| format!("发送 IPC 请求失败: {e}"))?;
    let reader = BufReader::new(stream);
    let mut resp = String::new();
    reader
        .take(65_537)
        .read_line(&mut resp)
        .map_err(|e| format!("读取 IPC 响应失败: {e}"))?;
    if resp.len() > 65_536 {
        return Err("IPC 响应超过大小限制".to_string());
    }
    let parsed: Value =
        serde_json::from_str(resp.trim()).map_err(|e| format!("解析响应失败: {e}"))?;
    if parsed.get("ok").and_then(|v| v.as_bool()) != Some(true) {
        let err = parsed
            .get("error")
            .and_then(|v| v.as_str())
            .unwrap_or("水冷服务返回错误");
        return Err(err.to_string());
    }
    Ok(parsed.get("result").cloned().unwrap_or(Value::Null))
}

fn wc_service_running() -> bool {
    wc_service_request(&json!({"op": "ping"}), 2).is_ok()
}

fn save_local_wc_patch(patch: &Map<String, Value>) -> Result<Value, String> {
    let path = config_dir().join("watercooling.json");
    let saved = iteglow_watercooling_config::save_offline_patch(&path, &runtime_dir(), &json!(patch))?;
    Ok(json!(saved))
}

fn find_companion_bin(name: &str, libexec: bool) -> Option<PathBuf> {
    if let Ok(exe) = fs::read_link("/proc/self/exe") {
        if let Some(dir) = exe.parent() {
            let direct = dir.join(name);
            if direct.is_file() {
                return Some(direct);
            }
            if libexec {
                let sib = dir.join("../libexec").join(name);
                if sib.is_file() {
                    return Some(sib);
                }
            }
        }
    }
    let home = std::env::var("HOME").unwrap_or_default();
    let candidates = if libexec {
        vec![
            PathBuf::from(format!("{home}/.local/libexec/{name}")),
            PathBuf::from(format!("/usr/libexec/{name}")),
            PathBuf::from(format!("/usr/local/libexec/{name}")),
        ]
    } else {
        vec![
            PathBuf::from(format!("{home}/.local/bin/{name}")),
            PathBuf::from(format!("/usr/bin/{name}")),
            PathBuf::from(format!("/usr/local/bin/{name}")),
        ]
    };
    candidates.into_iter().find(|p| p.is_file())
}

fn print_pretty(val: &Value) {
    println!(
        "{}",
        serde_json::to_string_pretty(val).unwrap_or_else(|_| val.to_string())
    );
}

fn cmd_cooler(args: &[String]) -> Result<(), String> {
    let Some(action) = args.first().map(|s| s.as_str()) else {
        return Err("Usage: iteglow cooler status|ec-status|scan|connect|disconnect|sleep|reset-fault|emergency-stop|set|led|daemon".to_string());
    };
    match action {
        "daemon" => {
            let bin = find_companion_bin("iteglow-watercoolingd", true)
                .ok_or("iteglow-watercoolingd binary not found")?;
            let err = Command::new(bin).args(&args[1..]).exec();
            Err(format!("exec iteglow-watercoolingd failed: {err}"))
        }
        "ec-status" => {
            print_pretty(&json!({
                "temp_c": read_host_temp_c(),
                "ec": read_ec_watercooler_state(),
            }));
            Ok(())
        }
        "status" => {
            let res = if wc_service_running() {
                wc_service_request(&json!({"op": "status"}), 5)?
            } else {
                let cfg = json!(iteglow_watercooling_config::load_config(&config_dir().join("watercooling.json"))?);
                json!({
                    "service_running": false,
                    "connected": false,
                    "temp_c": read_host_temp_c(),
                    "ec": read_ec_watercooler_state(),
                    "config": cfg,
                })
            };
            print_pretty(&res);
            Ok(())
        }
        "scan" => {
            user_lighting_only()?;
            let mut timeout = 5.0f64;
            let mut i = 1usize;
            while i < args.len() {
                match args[i].as_str() {
                    "--timeout" => {
                        i += 1;
                        let parsed = args
                            .get(i)
                            .ok_or("--timeout requires number")?
                            .parse::<f64>()
                            .map_err(|_| "invalid timeout")?;
                        if !parsed.is_finite() || !(0.5..=20.0).contains(&parsed) {
                            return Err("--timeout must be a finite number between 0.5 and 20.0".to_string());
                        }
                        timeout = parsed;
                    }
                    "--mac" => {
                        i += 1;
                    }
                    other => return Err(format!("unknown flag {other}")),
                }
                i += 1;
            }
            let res = wc_service_request(
                &json!({"op": "scan", "timeout_s": timeout}),
                (timeout.ceil() as u64).saturating_add(10),
            )?;
            print_pretty(&json!({"devices": res.get("devices").cloned().unwrap_or(json!([]))}));
            Ok(())
        }
        "connect" => {
            user_lighting_only()?;
            let mut mac = String::new();
            let mut i = 1usize;
            while i < args.len() {
                if args[i] == "--mac" {
                    i += 1;
                    mac = args.get(i).ok_or("--mac requires value")?.clone();
                } else {
                    return Err(format!("unknown flag {}", args[i]));
                }
                i += 1;
            }
            let res = wc_service_request(&json!({"op": "connect", "mac": mac}), 24)?;
            print_pretty(&res);
            Ok(())
        }
        "disconnect" => {
            user_lighting_only()?;
            let no_sleep = args.get(1).map(|s| s == "--no-sleep").unwrap_or(false);
            let res = wc_service_request(&json!({"op": "disconnect", "send_sleep": !no_sleep}), 8)?;
            print_pretty(&res);
            Ok(())
        }
        "sleep" => {
            user_lighting_only()?;
            let res = wc_service_request(&json!({"op": "disconnect", "send_sleep": true}), 8)?;
            print_pretty(&res);
            Ok(())
        }
        "reset-fault" => {
            user_lighting_only()?;
            let res = wc_service_request(&json!({"op": "reset_fault"}), 5)?;
            print_pretty(&res);
            Ok(())
        }
        "emergency-stop" => {
            user_lighting_only()?;
            let res = if wc_service_running() {
                wc_service_request(&json!({"op": "emergency_stop"}), 8)?
            } else {
                let mut patch = Map::new();
                patch.insert("strategy".into(), json!("preset"));
                patch.insert("preset".into(), json!("stop"));
                patch.insert("led".into(), json!({"mode": "off"}));
                json!({"service_running": false, "config": save_local_wc_patch(&patch)?})
            };
            print_pretty(&res);
            Ok(())
        }
        "set" => {
            user_lighting_only()?;
            let mut patch = Map::new();
            let mut manual = Map::new();
            let mut no_apply = false;
            let mut i = 1usize;
            while i < args.len() {
                match args[i].as_str() {
                    "--strategy" => {
                        i += 1;
                        patch.insert("strategy".into(), json!(args.get(i).ok_or("--strategy requires value")?));
                    }
                    "--preset" => {
                        i += 1;
                        patch.insert("preset".into(), json!(args.get(i).ok_or("--preset requires value")?));
                    }
                    "--mac" => {
                        i += 1;
                        patch.insert("mac".into(), json!(args.get(i).ok_or("--mac requires value")?));
                    }
                    "--auto-connect" => {
                        i += 1;
                        let v = args.get(i).ok_or("--auto-connect requires on|off")?;
                        let b = match v.as_str() {
                            "on" => true,
                            "off" => false,
                            _ => return Err("--auto-connect requires strictly 'on' or 'off'".to_string()),
                        };
                        patch.insert("auto_connect".into(), json!(b));
                    }
                    "--flow-protection" => {
                        i += 1;
                        let v = args.get(i).ok_or("--flow-protection requires on|off")?;
                        let b = match v.as_str() {
                            "on" => true,
                            "off" => false,
                            _ => return Err("--flow-protection requires strictly 'on' or 'off'".to_string()),
                        };
                        patch.insert("flow_protection".into(), json!(b));
                    }
                    "--sleep-on-disconnect" => {
                        i += 1;
                        let v = args.get(i).ok_or("--sleep-on-disconnect requires on|off")?;
                        let b = match v.as_str() {
                            "on" => true,
                            "off" => false,
                            _ => return Err("--sleep-on-disconnect requires strictly 'on' or 'off'".to_string()),
                        };
                        patch.insert("sleep_on_disconnect".into(), json!(b));
                    }
                    "--pump-volt" => {
                        i += 1;
                        let n = args.get(i).ok_or("--pump-volt requires integer")?.parse::<i64>().map_err(|_| "invalid pump-volt")?;
                        manual.insert("pump_volt".into(), json!(n));
                    }
                    "--pump-duty" => {
                        i += 1;
                        let n = args.get(i).ok_or("--pump-duty requires integer")?.parse::<i64>().map_err(|_| "invalid pump-duty")?;
                        manual.insert("pump_duty".into(), json!(n));
                    }
                    "--fan-duty" => {
                        i += 1;
                        let n = args.get(i).ok_or("--fan-duty requires integer")?.parse::<i64>().map_err(|_| "invalid fan-duty")?;
                        manual.insert("fan_duty".into(), json!(n));
                    }
                    "--no-apply" => no_apply = true,
                    other => return Err(format!("unknown flag {other}")),
                }
                i += 1;
            }
            if !manual.is_empty() {
                patch.insert("manual".into(), Value::Object(manual));
            }
            let res = if wc_service_running() {
                wc_service_request(
                    &json!({"op": "set_config", "config": Value::Object(patch), "apply_now": !no_apply}),
                    8,
                )?
            } else {
                json!({"service_running": false, "config": save_local_wc_patch(&patch)?})
            };
            print_pretty(&res);
            Ok(())
        }
        "led" => {
            user_lighting_only()?;
            let mode = args.get(1).ok_or("Usage: iteglow cooler led <mode> [--color C] [--fan-policy P] [--fan-color C]")?;
            let mut led = Map::new();
            led.insert("mode".into(), json!(mode));
            let mut i = 2usize;
            while i < args.len() {
                match args[i].as_str() {
                    "--color" => {
                        i += 1;
                        let c = parse_color_str(args.get(i).ok_or("--color requires value")?)?;
                        led.insert("color".into(), json!(c));
                    }
                    "--fan-policy" => {
                        i += 1;
                        led.insert("fan_policy".into(), json!(args.get(i).ok_or("--fan-policy requires value")?));
                    }
                    "--fan-color" => {
                        i += 1;
                        let c = parse_color_str(args.get(i).ok_or("--fan-color requires value")?)?;
                        led.insert("fan_color".into(), json!(c));
                    }
                    other => return Err(format!("unknown flag {other}")),
                }
                i += 1;
            }
            let mut patch = Map::new();
            patch.insert("led".into(), Value::Object(led));
            let res = if wc_service_running() {
                wc_service_request(
                    &json!({"op": "set_config", "config": Value::Object(patch), "apply_now": true}),
                    8,
                )?
            } else {
                json!({"service_running": false, "config": save_local_wc_patch(&patch)?})
            };
            print_pretty(&res);
            Ok(())
        }
        _ => Err(format!("unknown cooler action {action:?}")),
    }
}

fn cmd_gui(args: &[String]) -> Result<(), String> {
    if args.iter().any(|a| a == "--gtk") {
        let err = Command::new("/usr/bin/python3")
            .args(["-m", "iteglow.cli", "gui"])
            .exec();
        return Err(format!("exec legacy GTK4 GUI failed: {err}"));
    }
    let bin = find_companion_bin("mechrevo-control-gui", false)
        .ok_or("mechrevo-control-gui binary not found; install Tauri 2 GUI or pass --gtk")?;
    let err = Command::new(bin).exec();
    Err(format!("exec mechrevo-control-gui failed: {err}"))
}

fn main() -> ExitCode {
    let args: Vec<String> = std::env::args().skip(1).collect();
    if args.is_empty() || args[0] == "-h" || args[0] == "--help" {
        println!("{}", usage_main());
        return ExitCode::SUCCESS;
    }
    if args[0] == "-V" || args[0] == "--version" {
        println!("iteglow {VERSION}");
        return ExitCode::SUCCESS;
    }
    let res: Result<bool, String> = match args[0].as_str() {
        "doctor" => cmd_doctor(),
        "list" => cmd_list().map(|()| true),
        "set" => cmd_set(&args[1..]).map(|()| true),
        "off" => cmd_off(&args[1..]).map(|()| true),
        "restore" => cmd_restore(&args[1..]).map(|()| true),
        "pin" => cmd_pin().map(|()| true),
        "profile" => cmd_profile(&args[1..]).map(|()| true),
        "cooler" => cmd_cooler(&args[1..]).map(|()| true),
        "gui" => cmd_gui(&args[1..]).map(|()| true),
        other => Err(format!("unknown command {other:?}\n\n{}", usage_main())),
    };
    match res {
        Ok(true) => ExitCode::SUCCESS,
        Ok(false) => ExitCode::FAILURE,
        Err(err) => {
            eprintln!("error: {err}");
            ExitCode::FAILURE
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::hid::{preset_cells, KBD_MATRIX_CELLS};

    #[test]
    fn offline_cooler_patch_preserves_settings_and_rejects_invalid_or_busy_writes() {
        use std::os::unix::fs::PermissionsExt;
        use iteglow_watercooling_config::{acquire_lock, load_config, save_offline_patch};
        let dir = std::env::temp_dir().join(format!("iteglow-cli-wc-{}", std::process::id()));
        std::fs::create_dir(&dir).unwrap();
        std::fs::set_permissions(&dir, std::fs::Permissions::from_mode(0o700)).unwrap();
        let path = dir.join("watercooling.json");
        save_offline_patch(&path, &dir, &json!({"auto_connect": true, "led": {"mode": "static"}})).unwrap();
        let cfg = save_offline_patch(&path, &dir, &json!({"led": {"color": [4, 5, 6]}})).unwrap();
        assert!(cfg.auto_connect);
        assert_eq!(cfg.led.mode, "static");
        assert_eq!(cfg.led.color, [4, 5, 6]);
        assert!(save_offline_patch(&path, &dir, &json!({"manual": {"fan_duty": 101}})).is_err());
        let _daemon = acquire_lock(&dir).unwrap();
        assert!(save_offline_patch(&path, &dir, &json!({"auto_connect": false})).is_err());
        assert_eq!(load_config(&path).unwrap(), cfg);
        std::fs::remove_dir_all(dir).unwrap();
    }

    #[test]
    fn parse_colors_directions_and_inline_matrix() {
        assert_eq!(parse_color_str("#00ffff").unwrap(), [0, 255, 255]);
        assert_eq!(parse_color_str("255,40,0").unwrap(), [255, 40, 0]);
        assert!(parse_color_str("nope").is_err());

        assert_eq!(resolve_direction("keyboard", "rtl").unwrap(), 2);
        assert_eq!(resolve_direction("lightbar", "reverse").unwrap(), 2);

        let cells = parse_matrix_spec(r##"{"W": "#ff2800", "Esc": [0, 255, 255]}"##).unwrap();
        assert_eq!(cells.len(), KBD_MATRIX_CELLS);
        assert_eq!(cells[0], [0, 255, 255]);
        assert_eq!(cells[45], [255, 40, 0]);

        let p = preset_cells("wasd_fps", [0, 255, 255]).unwrap();
        assert_eq!(p[45], [255, 40, 0]);
        assert_eq!(p[65], [255, 40, 0]);
    }
}

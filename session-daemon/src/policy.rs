// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Pure AC/DC brightness and shared idle-light policy planner and session runner,
//! compatible with `iteglow.lighting_policy` and `iteglow.lighting_policy_session`.

use crate::protocol::{
    is_known_mode, keyboard_brightness, parse_rgb, parse_strict_int, validate_hid, validate_logo, RestoreAction,
    KBD_BRIGHTNESS_MAX, KBD_MATRIX_CELLS, LB_BRIGHTNESS_MAX, LOGO_COLORS, PRESET_NAMES,
};
use crate::zone_lock::{read_saved_json, ZoneGuard, MAX_STATE_BYTES};
use serde::Serialize;
use serde_json::{json, Map, Value};
use std::collections::{BTreeMap, HashMap};
use std::future::Future;
use std::path::Path;

pub const DEVICES: [&str; 3] = ["keyboard", "lightbar", "logo"];

pub fn default_config() -> Value {
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

pub fn canonical_encode(value: &Value) -> String {
    // serde_json::Map uses BTreeMap by default (preserve_order is disabled),
    // so to_string produces lexicographically sorted keys with compact separators,
    // matching Python's json.dumps(value, sort_keys=True, separators=(",", ":")).
    serde_json::to_string(value).unwrap_or_else(|_| "null".to_string())
}

pub fn validate_config(config: &Value) -> Result<Value, String> {
    let Some(obj) = config.as_object() else {
        return Err("灯光策略配置字段不完整或包含未知字段".to_string());
    };
    const EXPECTED: &[&str] = &["version", "enabled", "power_enabled", "idle_minutes", "brightness"];
    if obj.len() != EXPECTED.len() || EXPECTED.iter().any(|k| !obj.contains_key(*k)) {
        return Err("灯光策略配置字段不完整或包含未知字段".to_string());
    }
    if obj["version"].as_i64() != Some(1) {
        return Err("不支持的灯光策略版本".to_string());
    }
    for key in ["enabled", "power_enabled"] {
        if !obj[key].is_boolean() {
            return Err(format!("{key}: 需要布尔值"));
        }
    }
    parse_strict_int(&obj["idle_minutes"], 0, 1440, "空闲分钟数")?;

    let Some(brightness) = obj["brightness"].as_object() else {
        return Err("需要键盘、灯带和 Logo 三个灯区的亮度".to_string());
    };
    if brightness.len() != 3 || DEVICES.iter().any(|d| !brightness.contains_key(*d)) {
        return Err("需要键盘、灯带和 Logo 三个灯区的亮度".to_string());
    }
    for device in DEVICES {
        let Some(values) = brightness[device].as_object() else {
            return Err(format!("{device}: 需要独立 AC / DC 亮度"));
        };
        if values.len() != 2 || !values.contains_key("ac") || !values.contains_key("dc") {
            return Err(format!("{device}: 需要独立 AC / DC 亮度"));
        }
        for source in ["ac", "dc"] {
            let v = parse_strict_int(&values[source], 0, 100, &format!("{device} {source} 亮度"))?;
            if device == "logo" && v % 25 != 0 {
                return Err("Logo 亮度仅支持 0、25、50、75、100%".to_string());
            }
        }
    }
    Ok(config.clone())
}

pub fn brightness_value(device: &str, percent: i64) -> Result<i64, String> {
    if !(0..=100).contains(&percent) {
        return Err("亮度百分比: 需要 0–100 的整数".to_string());
    }
    if !DEVICES.contains(&device) {
        return Err("未知灯区".to_string());
    }
    if device == "logo" && percent % 25 != 0 {
        return Err("Logo 亮度必须是 25% 的倍数".to_string());
    }
    let maximum: i64 = if device == "keyboard" {
        KBD_BRIGHTNESS_MAX as i64
    } else {
        100
    };
    Ok((percent * maximum + 50) / 100)
}

fn validate_baseline(device: &str, saved: &Value) -> Result<Value, String> {
    let Some(obj) = saved.as_object() else {
        return Err("已应用灯光记录必须是对象".to_string());
    };
    let mut out = obj.clone();
    if device == "logo" {
        if out.len() != 3
            || !out.contains_key("mode")
            || !out.contains_key("brightness")
            || !out.contains_key("color")
        {
            return Err("Logo 记录字段错误".to_string());
        }
        let Some(mode) = out["mode"].as_str() else {
            return Err("未知 Logo 灯效".to_string());
        };
        if !matches!(mode, "solid" | "breathing" | "rainbow") {
            return Err("未知 Logo 灯效".to_string());
        }
        let bright = parse_strict_int(&out["brightness"], 0, 100, "Logo 亮度")?;
        if bright % 25 != 0 {
            return Err("Logo 亮度必须是 25% 的倍数".to_string());
        }
        let rgb = parse_rgb(&out["color"])?;
        if mode == "breathing" && !LOGO_COLORS.contains(&rgb) {
            return Err("不支持的 Logo 呼吸颜色".to_string());
        }
        out.insert("color".to_string(), json!([rgb.0, rgb.1, rgb.2]));
    } else {
        let mut allowed = vec![
            "mode",
            "brightness",
            "color",
            "colors",
            "speed",
            "direction",
            "reactive",
        ];
        if device == "keyboard" {
            allowed.extend_from_slice(&["matrix", "preset", "calibrate"]);
        }
        if out.keys().any(|k| !allowed.contains(&k.as_str())) {
            return Err("灯光记录包含未知字段".to_string());
        }
        let Some(mode) = out.get("mode").and_then(|v| v.as_str()) else {
            return Err("未知灯效".to_string());
        };
        if !is_known_mode(device, mode) {
            return Err("未知灯效".to_string());
        }
        let maximum = if device == "keyboard" {
            KBD_BRIGHTNESS_MAX as i64
        } else {
            LB_BRIGHTNESS_MAX as i64
        };
        if let Some(b) = out.get("brightness") {
            if !b.is_null() {
                parse_strict_int(b, 0, maximum, "亮度")?;
            }
        }
        if let Some(c) = out.get("color").cloned() {
            let rgb = parse_rgb(&c)?;
            out.insert("color".to_string(), json!([rgb.0, rgb.1, rgb.2]));
        }
        if let Some(cs) = out.get("colors").cloned() {
            if !cs.is_null() {
                let Some(arr) = cs.as_array() else {
                    return Err("颜色列表需要 1–7 项".to_string());
                };
                if arr.is_empty() || arr.len() > 7 {
                    return Err("颜色列表需要 1–7 项".to_string());
                }
                let mut clean = Vec::with_capacity(arr.len());
                for item in arr {
                    let rgb = parse_rgb(item)?;
                    clean.push(json!([rgb.0, rgb.1, rgb.2]));
                }
                out.insert("colors".to_string(), Value::Array(clean));
            }
        }
        if let Some(s) = out.get("speed") {
            parse_strict_int(s, 1, 10, "速度")?;
        }
        if let Some(d) = out.get("direction") {
            parse_strict_int(d, 1, if device == "keyboard" { 4 } else { 2 }, "方向")?;
        }
        if let Some(r) = out.get("reactive") {
            if !r.is_boolean() {
                return Err("按键响应需要布尔值".to_string());
            }
        }
        if let Some(m) = out.get("matrix").cloned() {
            if !m.is_null() {
                let Some(arr) = m.as_array() else {
                    return Err(format!("按键矩阵需要 {KBD_MATRIX_CELLS} 个 RGB 颜色"));
                };
                if arr.len() != KBD_MATRIX_CELLS {
                    return Err(format!("按键矩阵需要 {KBD_MATRIX_CELLS} 个 RGB 颜色"));
                }
                let mut clean = Vec::with_capacity(arr.len());
                for item in arr {
                    let rgb = parse_rgb(item)?;
                    clean.push(json!([rgb.0, rgb.1, rgb.2]));
                }
                out.insert("matrix".to_string(), Value::Array(clean));
            }
        }
        if let Some(p) = out.get("preset") {
            if !p.is_null() {
                let Some(name) = p.as_str() else {
                    return Err("未知按键矩阵预设".to_string());
                };
                if !PRESET_NAMES.contains(&name) {
                    return Err("未知按键矩阵预设".to_string());
                }
            }
        }
        if let Some(cal) = out.get("calibrate") {
            if !cal.is_boolean() {
                return Err("色彩校准需要布尔值".to_string());
            }
        }
    }
    Ok(Value::Object(out))
}

#[derive(Debug, Clone, PartialEq, Eq, Serialize)]
pub struct AppliedZone {
    pub device: String,
    pub baseline_json: String,
    pub params_json: String,
    pub active_json: String,
    pub idle: bool,
}

#[derive(Debug, Clone, Default, PartialEq, Eq, Serialize)]
pub struct PolicyMemory {
    pub zones: Vec<AppliedZone>,
}

#[derive(Debug, Clone, PartialEq, Eq)]
pub struct PolicyAction {
    pub device: String,
    pub baseline_json: String,
    pub params_json: String,
    pub active_json: String,
    pub idle: bool,
    pub reason: String,
    pub release: bool,
}

impl PolicyAction {
    pub fn params(&self) -> Value {
        serde_json::from_str(&self.params_json).unwrap_or(Value::Null)
    }
}

#[derive(Debug, Clone)]
pub struct PolicyPlan {
    pub actions: Vec<PolicyAction>,
    pub memory: PolicyMemory,
    pub errors: Vec<String>,
    pub status: String,
}

pub fn action_matches(action: &PolicyAction, saved: &BTreeMap<String, Value>) -> bool {
    let Some(val) = saved.get(&action.device) else {
        return false;
    };
    match validate_baseline(&action.device, val) {
        Ok(clean) => canonical_encode(&clean) == action.baseline_json,
        Err(_) => false,
    }
}

pub fn acknowledge(memory: &PolicyMemory, action: &PolicyAction) -> PolicyMemory {
    let mut zones: Vec<AppliedZone> = memory
        .zones
        .iter()
        .filter(|z| z.device != action.device)
        .cloned()
        .collect();
    if !action.release {
        zones.push(AppliedZone {
            device: action.device.clone(),
            baseline_json: action.baseline_json.clone(),
            params_json: action.params_json.clone(),
            active_json: action.active_json.clone(),
            idle: action.idle,
        });
    }
    zones.sort_by_key(|z| {
        DEVICES
            .iter()
            .position(|d| *d == z.device)
            .unwrap_or(DEVICES.len())
    });
    PolicyMemory { zones }
}

pub fn plan(
    config: &Value,
    power: Option<&str>,
    idle_seconds: Option<f64>,
    saved: &BTreeMap<String, Value>,
    memory: Option<&PolicyMemory>,
) -> Result<PolicyPlan, String> {
    let cfg = validate_config(config)?;
    if let Some(p) = power {
        if p != "ac" && p != "dc" {
            return Err("power 必须是 ac、dc 或 None".to_string());
        }
    }
    if let Some(idle) = idle_seconds {
        if !idle.is_finite() || idle < 0.0 {
            return Err("idle_seconds 必须是非负秒数或 None".to_string());
        }
    }
    for k in saved.keys() {
        if !DEVICES.contains(&k.as_str()) {
            return Err("已应用记录包含未知灯区".to_string());
        }
    }

    let prev_map: HashMap<&str, &AppliedZone> = memory
        .map(|m| m.zones.iter().map(|z| (z.device.as_str(), z)).collect())
        .unwrap_or_default();

    let enabled = cfg["enabled"].as_bool().unwrap_or(false);
    let power_enabled = cfg["power_enabled"].as_bool().unwrap_or(false);
    let idle_minutes = cfg["idle_minutes"].as_i64().unwrap_or(0);

    let mut owned: Vec<AppliedZone> = Vec::new();
    let mut actions: Vec<PolicyAction> = Vec::new();
    let mut errors: Vec<String> = Vec::new();

    for device in DEVICES {
        let Some(raw) = saved.get(device) else {
            continue;
        };
        let baseline = match validate_baseline(device, raw) {
            Ok(b) => b,
            Err(e) => {
                errors.push(format!("{device}: {e}"));
                continue;
            }
        };
        let baseline_json = canonical_encode(&baseline);
        let mut old = prev_map.get(device).copied();
        if let Some(o) = old {
            if o.baseline_json != baseline_json {
                old = None; // A manual edit takes ownership; never resurrect old effects.
            }
        }
        if let Some(o) = old {
            owned.push(o.clone());
        }
        if !enabled {
            if let Some(o) = old {
                if o.params_json != baseline_json {
                    actions.push(PolicyAction {
                        device: device.to_string(),
                        baseline_json: baseline_json.clone(),
                        params_json: baseline_json.clone(),
                        active_json: baseline_json,
                        idle: false,
                        reason: "disabled".to_string(),
                        release: true,
                    });
                }
            }
            continue;
        }

        let manually_off = baseline.get("mode").and_then(|v| v.as_str()) == Some("off")
            || baseline.get("brightness").and_then(|v| v.as_i64()) == Some(0)
            || (device == "logo"
                && baseline.get("mode").and_then(|v| v.as_str()) == Some("solid")
                && baseline
                    .get("color")
                    .and_then(|v| v.as_array())
                    .map(|arr| {
                        arr.iter()
                            .all(|c| (c.as_i64().unwrap_or(0) * 10 / 51) == 0)
                    })
                    .unwrap_or(false));

        let mut active = baseline.clone();
        if power_enabled && !manually_off {
            if let Some(src) = power {
                let pct = cfg["brightness"][device][src].as_i64().unwrap_or(100);
                let hw = brightness_value(device, pct)?;
                if let Some(map) = active.as_object_mut() {
                    map.insert("brightness".to_string(), json!(hw));
                }
            } else if let Some(o) = old {
                active = serde_json::from_str(&o.active_json).unwrap_or(baseline.clone());
            }
        }
        let active_json = canonical_encode(&active);

        let idle = if idle_minutes > 0 {
            match idle_seconds {
                Some(sec) => sec >= (idle_minutes as f64) * 60.0,
                None => old.map(|o| o.idle).unwrap_or(false),
            }
        } else {
            false
        };

        let mut effective = active.clone();
        if idle && !manually_off {
            if let Some(map) = effective.as_object_mut() {
                map.insert("brightness".to_string(), json!(0));
            }
        }
        let effective_json = canonical_encode(&effective);
        let current_json = old
            .map(|o| o.params_json.as_str())
            .unwrap_or(baseline_json.as_str());

        if effective_json != current_json {
            let reason = if idle {
                "idle"
            } else if old.map(|o| o.idle).unwrap_or(false) {
                "active"
            } else {
                "power"
            };
            let release = effective_json == baseline_json;
            actions.push(PolicyAction {
                device: device.to_string(),
                baseline_json,
                params_json: effective_json,
                active_json,
                idle,
                reason: reason.to_string(),
                release,
            });
        } else if let Some(o) = old {
            if let Some(slot) = owned.iter_mut().find(|z| z.device == o.device) {
                slot.active_json = active_json;
                slot.idle = idle;
            }
        }
    }

    Ok(PolicyPlan {
        actions,
        memory: PolicyMemory { zones: owned },
        errors,
        status: if enabled { "enabled" } else { "disabled" }.to_string(),
    })
}

// ----------------------------------------------------------------- Session files & Runner

#[derive(Debug, Clone, Default)]
pub struct ManualState {
    pub records: BTreeMap<String, Value>,
    pub errors: Vec<String>,
}

pub fn read_policy_config(path: &Path, uid: u32) -> Result<Value, String> {
    match read_saved_json(path, uid, MAX_STATE_BYTES)? {
        None => Ok(default_config()),
        Some(v) => validate_config(&v),
    }
}

pub fn read_manual(directory: &Path, uid: u32) -> ManualState {
    let mut records = BTreeMap::new();
    let mut errors = Vec::new();
    match read_saved_json(&directory.join("state.json"), uid, MAX_STATE_BYTES) {
        Ok(Some(saved)) => {
            if let Some(obj) = saved.as_object() {
                if obj.keys().any(|k| k != "keyboard" && k != "lightbar") {
                    errors.push("state.json: state.json contains unknown devices".to_string());
                } else {
                    for (k, v) in obj {
                        records.insert(k.clone(), v.clone());
                    }
                }
            } else {
                errors.push("state.json: state.json contains unknown devices".to_string());
            }
        }
        Ok(None) => {}
        Err(e) => errors.push(format!("state.json: {e}")),
    }

    match read_saved_json(&directory.join("logo.json"), uid, MAX_STATE_BYTES) {
        Ok(Some(saved)) => {
            records.insert("logo".to_string(), saved);
        }
        Ok(None) => {}
        Err(e) => errors.push(format!("logo.json: {e}")),
    }

    ManualState { records, errors }
}

pub struct PolicyRunner {
    pub apply: bool,
    pub memory: PolicyMemory,
    pub restoring: bool,
    last_report: Option<String>,
    config_snapshot: Option<Value>,
}

impl PolicyRunner {
    pub fn new(apply: bool) -> Self {
        Self {
            apply,
            memory: PolicyMemory::default(),
            restoring: false,
            last_report: None,
            config_snapshot: None,
        }
    }

    pub fn begin_restore(&mut self) {
        self.restoring = true;
    }

    pub fn finish_restore(&mut self) {
        self.memory = PolicyMemory::default();
        self.restoring = false;
    }

    pub fn deactivate(&mut self) {
        self.memory = PolicyMemory::default();
    }

    fn drop_zone(&mut self, device: &str) {
        self.memory.zones.retain(|z| z.device != device);
    }

    fn dedup_errors(errors: Vec<String>) -> Vec<String> {
        let mut out = Vec::new();
        for e in errors {
            if !out.contains(&e) {
                out.push(e);
            }
        }
        out
    }

    fn finish_event(
        &mut self,
        status: String,
        planned: Vec<Value>,
        applied: Vec<String>,
        skipped: Vec<String>,
        errors: Vec<String>,
        emit: &mut dyn FnMut(Value),
    ) -> Value {
        let errors = Self::dedup_errors(errors);
        let preview = if self.apply {
            Value::Null
        } else {
            Value::Array(planned.clone())
        };
        let sig_val = json!({
            "status": status,
            "errors": errors,
            "memory": self.memory,
            "config": self.config_snapshot,
            "preview": preview,
        });
        let signature = canonical_encode(&sig_val);
        let changed = self.last_report.as_deref() != Some(signature.as_str());
        self.last_report = Some(signature);

        let result = json!({
            "status": status,
            "planned": planned,
            "applied": applied,
            "skipped": skipped,
            "errors": errors,
        });
        if changed || !applied.is_empty() {
            let mut ev = Map::new();
            ev.insert("event".to_string(), json!("lighting_policy"));
            if let Some(obj) = result.as_object() {
                for (k, v) in obj {
                    ev.insert(k.clone(), v.clone());
                }
            }
            emit(Value::Object(ev));
        }
        result
    }

    #[allow(clippy::too_many_arguments)]
    pub async fn tick<FActive, FutActive, FPower, FutPower, FIdle, FutIdle, FWrite, FutWrite>(
        &mut self,
        mut is_active: FActive,
        mut read_cfg: impl FnMut() -> Result<Value, String>,
        mut read_manual_fn: impl FnMut() -> ManualState,
        mut sample_power: FPower,
        mut sample_idle: FIdle,
        mut write_action: FWrite,
        use_zone_lock: bool,
        emit: &mut dyn FnMut(Value),
    ) -> Value
    where
        FActive: FnMut() -> FutActive,
        FutActive: Future<Output = Result<bool, String>>,
        FPower: FnMut() -> FutPower,
        FutPower: Future<Output = Result<String, String>>,
        FIdle: FnMut() -> FutIdle,
        FutIdle: Future<Output = Result<f64, String>>,
        FWrite: FnMut(RestoreAction) -> FutWrite,
        FutWrite: Future<Output = Result<(), String>>,
    {
        let mut status = "disabled".to_string();
        let mut planned: Vec<Value> = Vec::new();
        let mut applied: Vec<String> = Vec::new();
        let mut skipped: Vec<String> = Vec::new();
        let mut errors: Vec<String> = Vec::new();

        match is_active().await {
            Ok(true) => {}
            Ok(false) => {
                self.deactivate();
                status = "inactive".to_string();
                return self.finish_event(status, planned, applied, skipped, errors, emit);
            }
            Err(e) => {
                self.deactivate();
                status = "error".to_string();
                errors.push(e);
                return self.finish_event(status, planned, applied, skipped, errors, emit);
            }
        }

        if self.restoring {
            status = "manual_restore_pending".to_string();
            return self.finish_event(status, planned, applied, skipped, errors, emit);
        }

        let cfg = match read_cfg().and_then(|c| validate_config(&c)) {
            Ok(c) => c,
            Err(e) => {
                status = "error".to_string();
                errors.push(e);
                return self.finish_event(status, planned, applied, skipped, errors, emit);
            }
        };
        self.config_snapshot = Some(cfg.clone());

        let enabled = cfg["enabled"].as_bool().unwrap_or(false);
        if !enabled && self.memory.zones.is_empty() {
            return self.finish_event(status, planned, applied, skipped, errors, emit);
        }

        let saved = read_manual_fn();
        errors.extend(saved.errors);

        let mut power: Option<String> = None;
        if enabled && cfg["power_enabled"].as_bool().unwrap_or(false) {
            match sample_power().await {
                Ok(p) if p == "ac" || p == "dc" => power = Some(p),
                Ok(_) => errors.push("power: invalid power sample".to_string()),
                Err(e) => errors.push(format!("power: {e}")),
            }
        }

        let mut idle: Option<f64> = None;
        if enabled && cfg["idle_minutes"].as_i64().unwrap_or(0) > 0 {
            match sample_idle().await {
                Ok(sec) if sec.is_finite() && sec >= 0.0 => idle = Some(sec),
                Ok(_) => errors.push("idle: invalid idle sample".to_string()),
                Err(e) => errors.push(format!("idle: {e}")),
            }
        }

        let proposal = match plan(
            &cfg,
            power.as_deref(),
            idle,
            &saved.records,
            Some(&self.memory),
        ) {
            Ok(p) => p,
            Err(e) => {
                status = "error".to_string();
                errors.push(e);
                return self.finish_event(status, planned, applied, skipped, errors, emit);
            }
        };

        self.memory = proposal.memory;
        status = if self.apply {
            proposal.status
        } else {
            format!("preview_{}", proposal.status)
        };
        errors.extend(proposal.errors);

        for action in proposal.actions {
            match is_active().await {
                Ok(true) => {}
                Ok(false) => {
                    self.deactivate();
                    status = "inactive".to_string();
                    break;
                }
                Err(e) => {
                    self.deactivate();
                    status = "error".to_string();
                    errors.push(e);
                    break;
                }
            }

            let _guard = if self.apply && use_zone_lock {
                match ZoneGuard::acquire(&action.device) {
                    Ok(g) => Some(g),
                    Err(e) => {
                        errors.push(format!("{}: {e}", action.device));
                        continue;
                    }
                }
            } else {
                None
            };

            let latest = read_manual_fn();
            errors.extend(latest.errors);
            if !action_matches(&action, &latest.records) {
                self.drop_zone(&action.device);
                skipped.push(format!("{}: manual state changed", action.device));
                continue;
            }

            let params = action.params();
            let target = if action.device == "logo" {
                validate_logo(&params)
            } else {
                validate_hid(&action.device, &params)
            };
            let mut target = match target {
                Ok(t) => t,
                Err(e) => {
                    errors.push(format!("{}: {e}", action.device));
                    continue;
                }
            };

            if action.device == "keyboard" {
                let brightness = params["brightness"].as_u64().unwrap_or(KBD_BRIGHTNESS_MAX.into());
                target.reports = keyboard_brightness(brightness as u8);
            }

            planned.push(json!({
                "device": action.device,
                "reason": action.reason,
                "brightness": params.get("brightness").cloned().unwrap_or(Value::Null),
            }));

            if !self.apply {
                continue;
            }

            match is_active().await {
                Ok(true) => {}
                Ok(false) => {
                    self.deactivate();
                    status = "inactive".to_string();
                    break;
                }
                Err(e) => {
                    self.deactivate();
                    status = "error".to_string();
                    errors.push(e);
                    break;
                }
            }

            if let Err(e) = write_action(target).await {
                errors.push(format!("{}: {e}", action.device));
                continue;
            }

            match is_active().await {
                Ok(true) => {}
                Ok(false) => {
                    self.deactivate();
                    status = "inactive".to_string();
                    break;
                }
                Err(e) => {
                    self.deactivate();
                    status = "error".to_string();
                    errors.push(e);
                    break;
                }
            }

            self.memory = acknowledge(&self.memory, &action);
            applied.push(action.device);
        }

        self.finish_event(status, planned, applied, skipped, errors, emit)
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::cell::RefCell;

    fn sample_cfg() -> Value {
        json!({
            "version": 1,
            "enabled": true,
            "power_enabled": true,
            "idle_minutes": 2,
            "brightness": {
                "keyboard": {"ac": 63, "dc": 21},
                "lightbar": {"ac": 70, "dc": 13},
                "logo": {"ac": 75, "dc": 25},
            }
        })
    }

    fn sample_saved() -> BTreeMap<String, Value> {
        let mut m = BTreeMap::new();
        m.insert(
            "keyboard".into(),
            json!({"mode": "spark", "brightness": 50, "direction": 4, "reactive": true}),
        );
        m.insert(
            "lightbar".into(),
            json!({"mode": "wave", "brightness": 100, "colors": [[3, 4, 5]]}),
        );
        m.insert(
            "logo".into(),
            json!({"mode": "breathing", "brightness": 100, "color": [255, 0, 0]}),
        );
        m
    }

    #[test]
    fn test_ac_dc_idle_and_manual_override_parity() {
        let cfg = sample_cfg();
        let saved = sample_saved();

        // AC scaling: 63% of 50 -> (3150+50)/100 = 32; 70% -> 70; 75% -> 75
        let ac = plan(&cfg, Some("ac"), Some(0.0), &saved, None).unwrap();
        let brights: Vec<i64> = ac
            .actions
            .iter()
            .map(|a| a.params()["brightness"].as_i64().unwrap())
            .collect();
        assert_eq!(brights, vec![32, 70, 75]);

        let mut mem = ac.memory;
        for a in &ac.actions {
            mem = acknowledge(&mem, a);
        }
        assert!(plan(&cfg, Some("ac"), Some(119.999), &saved, Some(&mem))
            .unwrap()
            .actions
            .is_empty());

        // Idle off at 120s
        let off = plan(&cfg, Some("ac"), Some(120.0), &saved, Some(&mem)).unwrap();
        assert_eq!(
            off.actions
                .iter()
                .map(|a| a.params()["brightness"].as_i64().unwrap())
                .collect::<Vec<_>>(),
            vec![0, 0, 0]
        );
        assert!(off.actions.iter().all(|a| a.reason == "idle"));

        let mut mem_off = off.memory;
        for a in &off.actions {
            mem_off = acknowledge(&mem_off, a);
        }
        // Power change while idle only updates future restore target
        let dc_idle = plan(&cfg, Some("dc"), Some(130.0), &saved, Some(&mem_off)).unwrap();
        assert!(dc_idle.actions.is_empty());
        let resumed = plan(&cfg, None, Some(0.0), &saved, Some(&dc_idle.memory)).unwrap();
        assert_eq!(
            resumed
                .actions
                .iter()
                .map(|a| a.params()["brightness"].as_i64().unwrap())
                .collect::<Vec<_>>(),
            vec![11, 13, 25]
        );
    }

    #[test]
    fn test_manual_off_and_black_logo_never_lit() {
        let cfg = sample_cfg();
        let mut saved = BTreeMap::new();
        saved.insert("keyboard".into(), json!({"mode": "off"}));
        saved.insert("lightbar".into(), json!({"mode": "wave", "brightness": 0}));
        saved.insert(
            "logo".into(),
            json!({"mode": "solid", "brightness": 100, "color": [4, 0, 1]}),
        );
        assert!(plan(&cfg, Some("ac"), Some(0.0), &saved, None)
            .unwrap()
            .actions
            .is_empty());
    }

    #[tokio::test]
    async fn test_policy_runner_dedup_and_partial_retry() {
        let cfg = sample_cfg();
        let saved = sample_saved();
        let mut runner = PolicyRunner::new(true);
        let fail_kbd = RefCell::new(true);
        let writes = RefCell::new(Vec::<String>::new());
        let mut emitted = Vec::<Value>::new();

        for _ in 0..2 {
            runner
                .tick(
                    || async { Ok(true) },
                    || Ok(cfg.clone()),
                    || ManualState {
                        records: saved.clone(),
                        errors: Vec::new(),
                    },
                    || async { Ok("ac".to_string()) },
                    || async { Ok(0.0) },
                    |act| {
                        if act.device == "keyboard" {
                            assert_eq!(act.reports, keyboard_brightness(32));
                        }
                        let fail = *fail_kbd.borrow();
                        writes.borrow_mut().push(act.device.clone());
                        async move {
                            if fail && act.device == "keyboard" {
                                Err("offline".to_string())
                            } else {
                                Ok(())
                            }
                        }
                    },
                    false,
                    &mut |ev| emitted.push(ev),
                )
                .await;
        }

        // First tick wrote keyboard (fail), lightbar (ok), logo (ok); second retried keyboard only.
        assert_eq!(writes.borrow().len(), 4);
        assert_eq!(emitted.len(), 1);

        *fail_kbd.borrow_mut() = false;
        let res = runner
            .tick(
                || async { Ok(true) },
                || Ok(cfg.clone()),
                || ManualState {
                    records: saved.clone(),
                    errors: Vec::new(),
                },
                || async { Ok("ac".to_string()) },
                || async { Ok(0.0) },
                |act| {
                    writes.borrow_mut().push(act.device.clone());
                    async { Ok(()) }
                },
                false,
                &mut |ev| emitted.push(ev),
            )
            .await;
        assert_eq!(res["applied"], json!(["keyboard"]));
        assert_eq!(emitted.len(), 2);
    }
}

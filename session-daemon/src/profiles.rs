// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! App-triggered performance & lighting profile auto-switch engine,
//! compatible with `iteglow.profiles` (`~/.config/iteglow/profiles.json`).

use crate::protocol::{
    apply_action, hidraw_available, parse_strict_int, run_profile_helper, validate_hid,
    validate_logo, RestoreAction,
};
use crate::zone_lock::{
    read_saved_json, read_system_json, ZoneGuard, MAX_PROFILE_BYTES, MAX_STATE_BYTES,
};
use serde_json::{json, Map, Value};
use std::collections::{BTreeMap, BTreeSet};
use std::fs;
use std::future::Future;
use std::os::unix::fs::MetadataExt;
use std::path::Path;

pub const PERF_MODES: &[&str] = &["office", "balanced", "turbo", "custom"];
pub const TURBO_VARIANTS: &[&str] = &["quiet", "extreme"];
pub const MIN_FAN_DUTY: [i64; 11] = [40, 40, 40, 40, 50, 55, 60, 70, 80, 90, 100];
pub const MAX_PROFILES: usize = 32;
pub const MAX_RULES: usize = 32;
pub const WMI_MODE_KEY_CODE: i64 = 176;
pub const WMI_STATE_PATH: &str = "/run/mechrevo-wmi-state.json";

const IGNORED_SELF_TOKENS: &[&str] = &["iteglow", "mechrevo-control", "mechrevo-profile-helper"];

pub fn default_config() -> Value {
    json!({
        "version": 1,
        "auto_switch_enabled": false,
        "restore_previous": true,
        "cooldown_seconds": 5,
        "profiles": {},
        "rules": [],
    })
}

fn clean_text(value: &Value, max_len: usize, field: &str, forbid_sep: bool) -> Result<String, String> {
    let Some(s) = value.as_str() else {
        return Err(format!("{field}: 需要字符串"));
    };
    let text = s.trim();
    if text.is_empty() || text.chars().count() > max_len {
        return Err(format!("{field}: 长度需在 1..{max_len} 之间"));
    }
    if text.chars().any(|c| (c as u32) < 32 || (c as u32) == 127) {
        return Err(format!("{field}: 不能包含控制字符"));
    }
    if forbid_sep && (text.contains('/') || text.contains('\\')) {
        return Err(format!("{field}: 不能包含路径分隔符"));
    }
    Ok(text.to_string())
}

pub fn validate_tuning(config: &Value) -> Result<Value, String> {
    let Some(obj) = config.as_object() else {
        return Err("tuning: 自定义调校字段不完整或包含未知字段".to_string());
    };
    let required = ["pl1", "pl2", "cpu_fan", "gpu_fan"];
    if required.iter().any(|k| !obj.contains_key(*k)) {
        return Err("tuning: 自定义调校字段不完整或包含未知字段".to_string());
    }
    let has_ctgp = obj.contains_key("gpu_ctgp");
    let has_db = obj.contains_key("gpu_db");
    if has_db && !has_ctgp {
        return Err("tuning: 自定义调校字段不完整或包含未知字段".to_string());
    }
    let allowed_len = 4 + usize::from(has_ctgp) + usize::from(has_db);
    if obj.len() != allowed_len {
        return Err("tuning: 自定义调校字段不完整或包含未知字段".to_string());
    }
    let pl1 = parse_strict_int(&obj["pl1"], 25, 75, "tuning.pl1")?;
    let pl2 = parse_strict_int(&obj["pl2"], 25, 75, "tuning.pl2")?;
    if pl1 != pl2 {
        return Err("tuning: pl1 与 pl2 需保持一致 (25..75 W)".to_string());
    }
    let mut out = Map::new();
    out.insert("pl1".into(), json!(pl1));
    out.insert("pl2".into(), json!(pl2));
    for fan_key in ["cpu_fan", "gpu_fan"] {
        let Some(arr) = obj[fan_key].as_array() else {
            return Err(format!("tuning.{fan_key}: 风扇曲线需要 11 个节点"));
        };
        if arr.len() != 11 {
            return Err(format!("tuning.{fan_key}: 风扇曲线需要 11 个节点"));
        }
        let mut clean = Vec::with_capacity(11);
        for (idx, item) in arr.iter().enumerate() {
            let v = parse_strict_int(item, MIN_FAN_DUTY[idx], 100, &format!("tuning.{fan_key}[{idx}]"))?;
            clean.push(v);
        }
        if clean.windows(2).any(|w| w[0] > w[1]) {
            return Err(format!("tuning.{fan_key}: 风扇占空比需随温度单调不减"));
        }
        out.insert(fan_key.into(), json!(clean));
    }
    if has_ctgp {
        let ctgp = parse_strict_int(&obj["gpu_ctgp"], 65, 115, "tuning.gpu_ctgp")?;
        out.insert("gpu_ctgp".into(), json!(ctgp));
    }
    if has_db {
        let Some(db) = obj["gpu_db"].as_i64() else {
            return Err("tuning.gpu_db: 仅支持 0 或 5..25 W".to_string());
        };
        if db != 0 && !(5..=25).contains(&db) {
            return Err("tuning.gpu_db: 仅支持 0 或 5..25 W".to_string());
        }
        out.insert("gpu_db".into(), json!(db));
    }
    Ok(Value::Object(out))
}

pub fn validate_performance(perf: Option<&Value>) -> Result<Option<Value>, String> {
    let Some(val) = perf else {
        return Ok(None);
    };
    if val.is_null() {
        return Ok(None);
    }
    let Some(obj) = val.as_object() else {
        return Err("performance: 包含未知字段".to_string());
    };
    if obj
        .keys()
        .any(|k| !matches!(k.as_str(), "mode" | "turbo" | "tuning"))
    {
        return Err("performance: 包含未知字段".to_string());
    }
    let Some(mode) = obj.get("mode").and_then(|v| v.as_str()) else {
        return Err("performance.mode: 必须是 office/balanced/turbo/custom".to_string());
    };
    if !PERF_MODES.contains(&mode) {
        return Err("performance.mode: 必须是 office/balanced/turbo/custom".to_string());
    }
    let mut out = Map::new();
    out.insert("mode".into(), json!(mode));
    if let Some(turbo) = obj.get("turbo") {
        if !turbo.is_null() {
            let Some(t) = turbo.as_str() else {
                return Err("performance.turbo: 仅 turbo 模式支持 quiet/extreme".to_string());
            };
            if mode != "turbo" || !TURBO_VARIANTS.contains(&t) {
                return Err("performance.turbo: 仅 turbo 模式支持 quiet/extreme".to_string());
            }
            out.insert("turbo".into(), json!(t));
        }
    }
    if let Some(tuning) = obj.get("tuning") {
        if !tuning.is_null() {
            if mode != "custom" {
                return Err("performance.tuning: 仅 custom 模式支持调校参数".to_string());
            }
            out.insert("tuning".into(), validate_tuning(tuning)?);
        }
    }
    Ok(Some(Value::Object(out)))
}

pub fn validate_lighting(lighting: Option<&Value>) -> Result<Option<Value>, String> {
    let Some(val) = lighting else {
        return Ok(None);
    };
    if val.is_null() {
        return Ok(None);
    }
    let Some(obj) = val.as_object() else {
        return Err("lighting: 仅支持 keyboard / lightbar / logo".to_string());
    };
    if obj
        .keys()
        .any(|k| !matches!(k.as_str(), "keyboard" | "lightbar" | "logo"))
    {
        return Err("lighting: 仅支持 keyboard / lightbar / logo".to_string());
    }
    let mut out = Map::new();
    for zone in ["keyboard", "lightbar"] {
        if let Some(zval) = obj.get(zone) {
            if !zval.is_null() {
                validate_hid(zone, zval)?;
                out.insert(zone.into(), zval.clone());
            }
        }
    }
    if let Some(lval) = obj.get("logo") {
        if !lval.is_null() {
            validate_logo(lval)?;
            out.insert("logo".into(), lval.clone());
        }
    }
    if out.is_empty() {
        Ok(None)
    } else {
        Ok(Some(Value::Object(out)))
    }
}

pub fn validate_profile(profile: &Value) -> Result<Value, String> {
    let Some(obj) = profile.as_object() else {
        return Err("profile: 包含未知字段".to_string());
    };
    if obj
        .keys()
        .any(|k| !matches!(k.as_str(), "label" | "performance" | "lighting"))
    {
        return Err("profile: 包含未知字段".to_string());
    }
    let mut out = Map::new();
    if let Some(lbl) = obj.get("label") {
        if !lbl.is_null() {
            out.insert("label".into(), json!(clean_text(lbl, 80, "profile.label", false)?));
        }
    }
    let perf = validate_performance(obj.get("performance"))?;
    let light = validate_lighting(obj.get("lighting"))?;
    if perf.is_none() && light.is_none() {
        return Err("profile: 至少需要包含性能或灯光配置".to_string());
    }
    if let Some(p) = perf {
        out.insert("performance".into(), p);
    }
    if let Some(l) = light {
        out.insert("lighting".into(), l);
    }
    Ok(Value::Object(out))
}

#[derive(Debug, Clone)]
pub struct ValidRule {
    pub name: String,
    pub enabled: bool,
    pub match_tokens: Vec<String>,
    pub profile: String,
}

#[derive(Debug, Clone)]
pub struct ProfilesConfig {
    pub auto_switch_enabled: bool,
    pub restore_previous: bool,
    pub cooldown_seconds: i64,
    pub profiles: BTreeMap<String, Value>,
    pub rules: Vec<ValidRule>,
}

pub fn validate_config(config: &Value) -> Result<ProfilesConfig, String> {
    let Some(obj) = config.as_object() else {
        return Err("profiles 配置字段不完整或包含未知字段".to_string());
    };
    const EXPECTED: &[&str] = &[
        "version",
        "auto_switch_enabled",
        "restore_previous",
        "cooldown_seconds",
        "profiles",
        "rules",
    ];
    if obj.len() != EXPECTED.len() || EXPECTED.iter().any(|k| !obj.contains_key(*k)) {
        return Err("profiles 配置字段不完整或包含未知字段".to_string());
    }
    if obj["version"].as_i64() != Some(1) {
        return Err("不支持的 profiles 配置版本".to_string());
    }
    let Some(auto_switch_enabled) = obj["auto_switch_enabled"].as_bool() else {
        return Err("auto_switch_enabled: 需要布尔值".to_string());
    };
    let Some(restore_previous) = obj["restore_previous"].as_bool() else {
        return Err("restore_previous: 需要布尔值".to_string());
    };
    let cooldown_seconds = parse_strict_int(&obj["cooldown_seconds"], 0, 120, "cooldown_seconds")?;

    let Some(raw_profiles) = obj["profiles"].as_object() else {
        return Err(format!("profiles: 最多支持 {MAX_PROFILES} 个预设"));
    };
    if raw_profiles.len() > MAX_PROFILES {
        return Err(format!("profiles: 最多支持 {MAX_PROFILES} 个预设"));
    }
    let mut profiles = BTreeMap::new();
    for (raw_name, raw_prof) in raw_profiles {
        let clean_name = clean_text(&Value::String(raw_name.clone()), 48, "profile name", true)?;
        if profiles.contains_key(&clean_name) {
            return Err(format!("profile name 重复: {clean_name:?}"));
        }
        profiles.insert(clean_name, validate_profile(raw_prof)?);
    }

    let Some(raw_rules) = obj["rules"].as_array() else {
        return Err(format!("rules: 最多支持 {MAX_RULES} 条规则"));
    };
    if raw_rules.len() > MAX_RULES {
        return Err(format!("rules: 最多支持 {MAX_RULES} 条规则"));
    }
    let mut rules = Vec::with_capacity(raw_rules.len());
    for item in raw_rules {
        let Some(robj) = item.as_object() else {
            return Err("rule: 需要 name / enabled / match / profile 字段".to_string());
        };
        if robj.len() != 4
            || !robj.contains_key("name")
            || !robj.contains_key("enabled")
            || !robj.contains_key("match")
            || !robj.contains_key("profile")
        {
            return Err("rule: 需要 name / enabled / match / profile 字段".to_string());
        }
        let name = clean_text(&robj["name"], 64, "rule.name", false)?;
        let Some(enabled) = robj["enabled"].as_bool() else {
            return Err("rule.enabled: 需要布尔值".to_string());
        };
        let Some(raw_match) = robj["match"].as_array() else {
            return Err("rule.match: 需要 1..8 个匹配关键词".to_string());
        };
        if raw_match.is_empty() || raw_match.len() > 8 {
            return Err("rule.match: 需要 1..8 个匹配关键词".to_string());
        }
        let mut match_tokens = Vec::with_capacity(raw_match.len());
        for m in raw_match {
            match_tokens.push(clean_text(m, 128, "rule.match[]", false)?.to_lowercase());
        }
        let profile = clean_text(&robj["profile"], 48, "rule.profile", true)?;
        if !profiles.contains_key(&profile) {
            return Err(format!("rule.profile: 引用了不存在的预设 {profile:?}"));
        }
        rules.push(ValidRule {
            name,
            enabled,
            match_tokens,
            profile,
        });
    }

    Ok(ProfilesConfig {
        auto_switch_enabled,
        restore_previous,
        cooldown_seconds,
        profiles,
        rules,
    })
}

pub fn read_profiles_config(path: &Path, uid: u32) -> Result<ProfilesConfig, String> {
    match read_saved_json(path, uid, MAX_PROFILE_BYTES)? {
        None => validate_config(&default_config()),
        Some(v) => validate_config(&v),
    }
}

// ----------------------------------------------------------------- /proc & WMI

fn extract_tokens(comm_text: &str, cmdline_raw: &[u8]) -> Vec<String> {
    let mut tokens = BTreeSet::new();
    let clean_comm = comm_text.trim().to_lowercase();
    if !clean_comm.is_empty() {
        tokens.insert(clean_comm);
    }
    let parts: Vec<String> = cmdline_raw
        .split(|&b| b == 0)
        .filter(|slice| !slice.is_empty())
        .map(|slice| String::from_utf8_lossy(slice).trim().to_string())
        .take(6)
        .collect();
    for (idx, part) in parts.iter().enumerate() {
        let low = part.to_lowercase();
        if low.is_empty() {
            continue;
        }
        if idx == 0 {
            tokens.insert(low.clone());
        }
        let normalized = low.replace('\\', "/");
        if let Some(base) = normalized.rsplit('/').next() {
            if !base.is_empty() {
                tokens.insert(base.to_string());
                if let Some(stem) = base.strip_suffix(".exe") {
                    if !stem.is_empty() {
                        tokens.insert(stem.to_string());
                    }
                }
            }
        }
    }
    tokens.into_iter().collect()
}

pub fn scan_processes(proc_root: &Path, uid: u32) -> Vec<Vec<String>> {
    let mut results = Vec::new();
    let Ok(entries) = fs::read_dir(proc_root) else {
        return results;
    };
    for entry in entries.flatten() {
        let fname = entry.file_name();
        let Some(name_str) = fname.to_str() else {
            continue;
        };
        if !name_str.bytes().all(|b| b.is_ascii_digit()) {
            continue;
        }
        let path = entry.path();
        let Ok(meta) = fs::metadata(&path) else {
            continue;
        };
        if !meta.is_dir() || meta.uid() != uid {
            continue;
        }
        let mut comm_raw = Vec::new();
        let Ok(comm_f) = fs::File::open(path.join("comm")) else {
            continue;
        };
        if std::io::Read::read_to_end(&mut std::io::Read::take(comm_f, 256), &mut comm_raw).is_err()
        {
            continue;
        }
        let mut cmd_raw = Vec::new();
        let Ok(cmd_f) = fs::File::open(path.join("cmdline")) else {
            continue;
        };
        if std::io::Read::read_to_end(&mut std::io::Read::take(cmd_f, 4096), &mut cmd_raw).is_err()
        {
            continue;
        }
        if cmd_raw.is_empty() {
            continue;
        }
        let comm_text = String::from_utf8_lossy(&comm_raw);
        let tokens = extract_tokens(&comm_text, &cmd_raw);
        if tokens.is_empty()
            || tokens.iter().any(|t| {
                IGNORED_SELF_TOKENS.contains(&t.as_str())
                    || t.starts_with("iteglow")
                    || t.starts_with("mechrevo-")
            })
        {
            continue;
        }
        results.push(tokens);
    }
    results
}

pub fn match_rule<'a>(rules: &'a [ValidRule], processes: &[Vec<String>]) -> Option<&'a ValidRule> {
    for rule in rules {
        if !rule.enabled {
            continue;
        }
        for tokens in processes {
            for needle in &rule.match_tokens {
                if tokens.iter().any(|t| t.contains(needle.as_str())) {
                    return Some(rule);
                }
            }
        }
    }
    None
}

pub fn read_wmi_snapshot(path: &Path) -> Option<Value> {
    read_system_json(path, 65_536).filter(|v| v.is_object())
}

pub fn latest_wmi_code_seq(snapshot: Option<&Value>, code: i64) -> i64 {
    let Some(obj) = snapshot.and_then(|v| v.as_object()) else {
        return 0;
    };
    let mut best: i64 = 0;
    if let Some(events) = obj.get("wmi_events").and_then(|v| v.as_array()) {
        for item in events {
            if let Some(ev) = item.as_object() {
                if ev.get("code").and_then(|v| v.as_i64()) == Some(code) {
                    if let Some(seq) = ev.get("seq").and_then(|v| v.as_i64()) {
                        if seq > best {
                            best = seq;
                        }
                    }
                }
            }
        }
    }
    if obj.get("wmi_last_code").and_then(|v| v.as_i64()) == Some(code) {
        if let Some(seq) = obj.get("wmi_seq").and_then(|v| v.as_i64()) {
            best = best.max(seq);
        }
    }
    best
}

// ----------------------------------------------------------------- RuleEvaluator

#[derive(Debug, Clone)]
pub struct RuleDecision {
    pub action: String,
    pub rule_name: Option<String>,
    pub profile_name: Option<String>,
    pub restore_profile: Option<Value>,
    pub reason: String,
}

impl RuleDecision {
    fn simple(action: &str) -> Self {
        Self {
            action: action.to_string(),
            rule_name: None,
            profile_name: None,
            restore_profile: None,
            reason: String::new(),
        }
    }
}

#[derive(Debug, Default)]
pub struct RuleEvaluator {
    pub active_rule: Option<String>,
    pub active_profile: Option<String>,
    pub baseline_profile: Option<Value>,
    pub last_switch_time: Option<f64>,
    pub missing_since: Option<f64>,
    pub last_b0_seq: Option<i64>,
    pub yielded_until_exit: Option<String>,
}

impl RuleEvaluator {
    pub fn reset(&mut self) {
        self.active_rule = None;
        self.active_profile = None;
        self.baseline_profile = None;
        self.missing_since = None;
        self.yielded_until_exit = None;
    }

    fn clear_active_keeping_yield(&mut self) {
        self.active_rule = None;
        self.active_profile = None;
        self.baseline_profile = None;
        self.missing_since = None;
    }

    pub async fn step<FCap, FutCap>(
        &mut self,
        config: &ProfilesConfig,
        processes: &[Vec<String>],
        b0_seq: i64,
        fan_safety_protect: bool,
        now: f64,
        mut capture_baseline: FCap,
    ) -> RuleDecision
    where
        FCap: FnMut() -> FutCap,
        FutCap: Future<Output = Option<Value>>,
    {
        match self.last_b0_seq {
            None => self.last_b0_seq = Some(b0_seq),
            Some(prev) if b0_seq == 0 && prev > 0 => {
                // Guard daemon restarted and sequence counter reset to 0.
                self.last_b0_seq = Some(0);
            }
            Some(prev) if b0_seq > 0 && b0_seq != prev => {
                self.last_b0_seq = Some(b0_seq);
                if let Some(yielded) = self.active_rule.clone() {
                    let prev_prof = self.active_profile.clone();
                    self.yielded_until_exit = Some(yielded.clone());
                    self.clear_active_keeping_yield();
                    self.last_switch_time = Some(now);
                    return RuleDecision {
                        action: "yield_mode_key".to_string(),
                        rule_name: Some(yielded),
                        profile_name: prev_prof,
                        restore_profile: None,
                        reason: "物理模式键 0xB0 已接管，自动切换让步".to_string(),
                    };
                }
            }
            _ => {}
        }

        if !config.auto_switch_enabled {
            self.yielded_until_exit = None;
            if let Some(prev_rule) = self.active_rule.clone() {
                let prev_prof = self.active_profile.clone();
                let restore = if config.restore_previous {
                    self.baseline_profile.clone()
                } else {
                    None
                };
                self.reset();
                self.last_switch_time = Some(now);
                return RuleDecision {
                    action: "release".to_string(),
                    rule_name: Some(prev_rule),
                    profile_name: prev_prof,
                    restore_profile: restore,
                    reason: "自动切换已关闭，恢复先前状态".to_string(),
                };
            }
            return RuleDecision::simple("disabled");
        }

        let matched = match_rule(&config.rules, processes);
        let cooldown = config.cooldown_seconds as f64;

        if let Some(yielded) = self.yielded_until_exit.clone() {
            if let Some(m) = matched {
                if m.name == yielded {
                    self.missing_since = None;
                    return RuleDecision {
                        action: "yielded".to_string(),
                        rule_name: Some(m.name.clone()),
                        profile_name: Some(m.profile.clone()),
                        restore_profile: None,
                        reason: String::new(),
                    };
                }
            } else {
                let since = *self.missing_since.get_or_insert(now);
                if (now - since) < cooldown {
                    return RuleDecision {
                        action: "yielded".to_string(),
                        rule_name: Some(yielded),
                        profile_name: None,
                        restore_profile: None,
                        reason: String::new(),
                    };
                }
            }
            self.yielded_until_exit = None;
            self.missing_since = None;
        }

        if let Some(m) = matched {
            self.missing_since = None;
            if fan_safety_protect
                && config
                    .profiles
                    .get(&m.profile)
                    .and_then(|p| p.get("performance"))
                    .is_some()
            {
                return RuleDecision {
                    action: "fan_safety_paused".to_string(),
                    rule_name: Some(m.name.clone()),
                    profile_name: Some(m.profile.clone()),
                    restore_profile: None,
                    reason: "EC 高温风扇安全保护激活，暂停自动性能切换".to_string(),
                };
            }
            if self.active_rule.as_deref() == Some(m.name.as_str())
                && self.active_profile.as_deref() == Some(m.profile.as_str())
            {
                return RuleDecision {
                    action: "noop".to_string(),
                    rule_name: Some(m.name.clone()),
                    profile_name: Some(m.profile.clone()),
                    restore_profile: None,
                    reason: String::new(),
                };
            }
            if self.active_rule.is_some() {
                if let Some(last) = self.last_switch_time {
                    if (now - last) < cooldown {
                        return RuleDecision {
                            action: "hold".to_string(),
                            rule_name: self.active_rule.clone(),
                            profile_name: self.active_profile.clone(),
                            restore_profile: None,
                            reason: String::new(),
                        };
                    }
                }
            }
            if self.active_rule.is_none() {
                self.baseline_profile = capture_baseline().await;
            }
            self.active_rule = Some(m.name.clone());
            self.active_profile = Some(m.profile.clone());
            self.last_switch_time = Some(now);
            return RuleDecision {
                action: "apply".to_string(),
                rule_name: Some(m.name.clone()),
                profile_name: Some(m.profile.clone()),
                restore_profile: None,
                reason: String::new(),
            };
        }

        if let Some(prev_rule) = self.active_rule.clone() {
            let since = *self.missing_since.get_or_insert(now);
            if (now - since) < cooldown {
                return RuleDecision {
                    action: "hold".to_string(),
                    rule_name: Some(prev_rule),
                    profile_name: self.active_profile.clone(),
                    restore_profile: None,
                    reason: String::new(),
                };
            }
            let prev_prof = self.active_profile.clone();
            let mut restore = if config.restore_previous {
                self.baseline_profile.clone()
            } else {
                None
            };
            if fan_safety_protect {
                if let Some(Value::Object(ref mut map)) = restore {
                    map.remove("performance");
                    if map.is_empty() {
                        restore = None;
                    }
                }
            }
            self.clear_active_keeping_yield();
            self.last_switch_time = Some(now);
            return RuleDecision {
                action: "release".to_string(),
                rule_name: Some(prev_rule),
                profile_name: prev_prof,
                restore_profile: restore,
                reason: "匹配进程已退出，释放规则".to_string(),
            };
        }

        RuleDecision::simple("noop")
    }
}

// ----------------------------------------------------------------- Runner & hardware apply

fn read_manual_lighting(directory: &Path, uid: u32) -> Map<String, Value> {
    let mut lighting = Map::new();
    if let Ok(Some(Value::Object(hid))) =
        read_saved_json(&directory.join("state.json"), uid, MAX_STATE_BYTES)
    {
        for zone in ["keyboard", "lightbar"] {
            if let Some(v) = hid.get(zone) {
                lighting.insert(zone.to_string(), v.clone());
            }
        }
    }
    if let Ok(Some(logo @ Value::Object(_))) =
        read_saved_json(&directory.join("logo.json"), uid, MAX_STATE_BYTES)
    {
        lighting.insert("logo".to_string(), logo);
    }
    lighting
}

async fn capture_current_profile(directory: &Path, uid: u32) -> Option<Value> {
    let mut profile = Map::new();
    if let Ok(st) = run_profile_helper(&["status"], 6).await {
        if let Some(mode) = st.get("mode").and_then(|v| v.as_str()) {
            if PERF_MODES.contains(&mode) {
                let mut perf = Map::new();
                perf.insert("mode".into(), json!(mode));
                let turbo = st.get("turbo").and_then(|v| v.as_object());
                if mode == "turbo" {
                    if let Some(t) = turbo {
                        if t.get("active").and_then(|v| v.as_bool()) == Some(true) {
                            if let Some(var) = t.get("variant").and_then(|v| v.as_str()) {
                                if TURBO_VARIANTS.contains(&var) {
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
    let light = read_manual_lighting(directory, uid);
    if !light.is_empty() {
        profile.insert("lighting".into(), Value::Object(light));
    }
    if profile.is_empty() {
        return None;
    }
    validate_profile(&Value::Object(profile)).ok()
}

async fn apply_performance(perf: &Value) -> Result<(), String> {
    let st = run_profile_helper(&["status"], 6).await?;
    if st.get("fan_safety_protect").and_then(|v| v.as_bool()) == Some(true) {
        return Err("EC 高温风扇安全保护激活，跳过性能切换".to_string());
    }
    if st.get("switchable").and_then(|v| v.as_bool()) != Some(true) {
        let msg = st
            .get("reason")
            .and_then(|v| v.as_str())
            .unwrap_or("当前固件状态不可通过软件切换");
        return Err(msg.to_string());
    }
    let target_mode = perf["mode"].as_str().unwrap_or("");
    let current_mode = st
        .get("mode")
        .and_then(|v| v.as_str())
        .unwrap_or("")
        .to_string();

    if target_mode == "custom" {
        if let Some(tuning) = perf.get("tuning") {
            if st.get("tuning") != Some(tuning) {
                let encoded = serde_json::to_string(tuning).map_err(|e| e.to_string())?;
                run_profile_helper(&["apply", &encoded], 6).await?;
            }
            return Ok(());
        }
    }

    let mut latest_st = st;
    if current_mode != target_mode
        || (target_mode == "custom"
            && latest_st.get("tuning_active").and_then(|v| v.as_bool()) == Some(true))
    {
        run_profile_helper(&["set", target_mode], 6).await?;
        latest_st = run_profile_helper(&["status"], 6).await?;
    }

    if target_mode == "turbo" {
        let turbo = latest_st.get("turbo").and_then(|v| v.as_object());
        if let Some(desired_turbo) = perf.get("turbo").and_then(|v| v.as_str()) {
            let eligible = turbo.map(|t| {
                t.get("available").and_then(|v| v.as_bool()) == Some(true)
                    && t.get("round_adapter").and_then(|v| v.as_bool()) == Some(true)
                    && t.get("combined_supported").and_then(|v| v.as_bool()) == Some(true)
            }) == Some(true)
                && latest_st.get("tuning_active").and_then(|v| v.as_bool()) != Some(true)
                && latest_st
                    .get("tuning_recovery_pending")
                    .and_then(|v| v.as_bool())
                    != Some(true);
            let cur_variant = turbo.and_then(|t| t.get("variant").and_then(|v| v.as_str()));
            if eligible && cur_variant != Some(desired_turbo) {
                run_profile_helper(&["turbo", desired_turbo], 6).await?;
            }
        } else if turbo.and_then(|t| t.get("active").and_then(|v| v.as_bool())) == Some(true) {
            // Baseline was standard firmware turbo; release active turbo submode lease.
            run_profile_helper(&["turbo", "off"], 6).await?;
        }
    }
    Ok(())
}

async fn apply_profile_bundle(
    profile: &Value,
    apply_lighting: bool,
    apply_perf: bool,
) -> Result<(), String> {
    let clean = validate_profile(profile)?;
    if apply_lighting {
        if let Some(light) = clean.get("lighting").and_then(|v| v.as_object()) {
            for zone in ["keyboard", "lightbar"] {
                if let Some(zcfg) = light.get(zone) {
                    if !hidraw_available(zone) {
                        continue;
                    }
                    let act: RestoreAction = validate_hid(zone, zcfg)?;
                    let _guard = ZoneGuard::acquire(zone)?;
                    apply_action(&act).await?;
                }
            }
            if let Some(lcfg) = light.get("logo") {
                let act = validate_logo(lcfg)?;
                let _guard = ZoneGuard::acquire("logo")?;
                apply_action(&act).await?;
            }
        }
    }
    if apply_perf {
        if let Some(perf) = clean.get("performance") {
            apply_performance(perf).await?;
        }
    }
    Ok(())
}

pub struct ProfileTickResult {
    pub action: String,
    pub active_lighting: bool,
    pub released_lighting: bool,
}

pub struct ProfileRuleRunner {
    pub apply: bool,
    pub evaluator: RuleEvaluator,
    pub owns_lighting: bool,
}

impl ProfileRuleRunner {
    pub fn new(apply: bool) -> Self {
        Self {
            apply,
            evaluator: RuleEvaluator::default(),
            owns_lighting: false,
        }
    }

    pub fn deactivate(&mut self) {
        self.evaluator.reset();
        self.owns_lighting = false;
    }

    pub async fn tick(
        &mut self,
        active: bool,
        config_path: &Path,
        state_dir: &Path,
        uid: u32,
        now_secs: f64,
        emit: &mut dyn FnMut(Value),
    ) -> ProfileTickResult {
        if !active {
            self.deactivate();
            return ProfileTickResult {
                action: "inactive".to_string(),
                active_lighting: false,
                released_lighting: false,
            };
        }

        let cfg = match read_profiles_config(config_path, uid) {
            Ok(c) => c,
            Err(e) => {
                emit(json!({
                    "event": "profile_rule",
                    "action": "noop",
                    "active_lighting": self.owns_lighting,
                    "released_lighting": false,
                    "errors": [e],
                }));
                return ProfileTickResult {
                    action: "noop".to_string(),
                    active_lighting: self.owns_lighting,
                    released_lighting: false,
                };
            }
        };

        // Zero overhead when opt-in disabled and nothing owned.
        if !cfg.auto_switch_enabled && self.evaluator.active_rule.is_none() {
            return ProfileTickResult {
                action: "disabled".to_string(),
                active_lighting: self.owns_lighting,
                released_lighting: false,
            };
        }

        let wmi = read_wmi_snapshot(Path::new(WMI_STATE_PATH));
        let b0_seq = latest_wmi_code_seq(wmi.as_ref(), WMI_MODE_KEY_CODE);
        let fan_safety = wmi
            .as_ref()
            .and_then(|v| v.get("fan_safety_protect"))
            .and_then(|v| v.as_bool())
            .unwrap_or(false);
        let procs = scan_processes(Path::new("/proc"), uid);

        let decision = self
            .evaluator
            .step(&cfg, &procs, b0_seq, fan_safety, now_secs, || async {
                capture_current_profile(state_dir, uid).await
            })
            .await;

        let mut errors: Vec<String> = Vec::new();
        let mut released_lighting = false;

        match decision.action.as_str() {
            "apply" => {
                if let Some(ref prof_name) = decision.profile_name {
                    if let Some(target) = cfg.profiles.get(prof_name) {
                        if self.apply {
                            if let Err(e) = apply_profile_bundle(target, true, true).await {
                                errors.push(e);
                            }
                        }
                        if target.get("lighting").is_some() {
                            self.owns_lighting = true;
                        }
                    }
                }
            }
            "release" => {
                let was_owning = self.owns_lighting;
                self.owns_lighting = false;
                released_lighting = was_owning;
                if self.apply {
                    if let Some(ref restore) = decision.restore_profile {
                        if let Err(e) = apply_profile_bundle(restore, true, true).await {
                            errors.push(e);
                        }
                    }
                }
            }
            "yield_mode_key" => {
                let was_owning = self.owns_lighting;
                self.owns_lighting = false;
                released_lighting = was_owning;
                if self.apply && was_owning {
                    let manual_light = read_manual_lighting(state_dir, uid);
                    if !manual_light.is_empty() {
                        let bundle = json!({"lighting": Value::Object(manual_light)});
                        if let Err(e) = apply_profile_bundle(&bundle, true, false).await {
                            errors.push(e);
                        }
                    }
                }
            }
            _ => {}
        }

        if !matches!(
            decision.action.as_str(),
            "noop" | "disabled" | "hold" | "yielded"
        ) || !errors.is_empty()
        {
            let mut ev = json!({
                "event": "profile_rule",
                "action": decision.action,
                "active_lighting": self.owns_lighting,
                "released_lighting": released_lighting,
                "rule": decision.rule_name,
                "profile": decision.profile_name,
                "errors": errors,
            });
            if !decision.reason.is_empty() {
                ev["reason"] = json!(decision.reason);
            }
            emit(ev);
        }

        ProfileTickResult {
            action: decision.action,
            active_lighting: self.owns_lighting,
            released_lighting,
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::path::PathBuf;
    use std::time::Instant;

    #[test]
    fn test_proc_scan_extracts_wine_and_ignores_self() {
        let root = std::env::temp_dir().join(format!(
            "iteglow-proc-test-{}-{}",
            std::process::id(),
            Instant::now().elapsed().as_nanos()
        ));
        let _ = fs::remove_dir_all(&root);
        fs::create_dir_all(root.join("1234")).unwrap();
        fs::write(root.join("1234/comm"), "MainThread\n").unwrap();
        fs::write(
            root.join("1234/cmdline"),
            b"Z:\\Games\\CP2077\\Cyberpunk2077.exe\0-skipStartScreen\0",
        )
        .unwrap();
        fs::create_dir_all(root.join("1235")).unwrap();
        fs::write(root.join("1235/comm"), "iteglow\n").unwrap();
        fs::write(
            root.join("1235/cmdline"),
            b"/usr/bin/python3\0-m\0iteglow.gui\0",
        )
        .unwrap();

        let uid = unsafe { libc::getuid() };
        let procs = scan_processes(&root, uid);
        assert_eq!(procs.len(), 1);
        assert!(procs[0].contains(&"cyberpunk2077.exe".to_string()));
        assert!(procs[0].contains(&"cyberpunk2077".to_string()));
        let _ = fs::remove_dir_all(&root);
    }

    #[tokio::test]
    async fn test_evaluator_cooldown_mode_key_yield_and_fan_safety() {
        let raw_cfg = json!({
            "version": 1,
            "auto_switch_enabled": true,
            "restore_previous": true,
            "cooldown_seconds": 5,
            "profiles": {
                "gaming-fps": {
                    "label": "FPS",
                    "performance": {"mode": "turbo", "turbo": "extreme"},
                    "lighting": {"keyboard": {"mode": "solid", "color": [0, 255, 255], "brightness": 50}}
                }
            },
            "rules": [
                {"name": "GameRule", "enabled": true, "match": ["cyberpunk2077", "blender"], "profile": "gaming-fps"}
            ]
        });
        let cfg = validate_config(&raw_cfg).unwrap();
        let baseline = json!({"performance": {"mode": "balanced"}, "lighting": {"keyboard": {"mode": "rainbow", "brightness": 50}}});
        let mut ev = RuleEvaluator::default();

        let d0 = ev.step(&cfg, &[], 3, false, 10.0, || async { Some(baseline.clone()) }).await;
        assert_eq!(d0.action, "noop");

        let d1 = ev
            .step(&cfg, &[vec!["cyberpunk2077".into()]], 3, false, 12.0, || async {
                Some(baseline.clone())
            })
            .await;
        assert_eq!(d1.action, "apply");
        assert_eq!(d1.rule_name.as_deref(), Some("GameRule"));

        let d2 = ev.step(&cfg, &[], 3, false, 15.0, || async { None }).await;
        assert_eq!(d2.action, "hold");

        // Physical Mode Key 0xB0 arrives
        let d3 = ev
            .step(&cfg, &[vec!["cyberpunk2077".into()]], 4, false, 16.0, || async { None })
            .await;
        assert_eq!(d3.action, "yield_mode_key");

        let d4 = ev
            .step(&cfg, &[vec!["cyberpunk2077".into()]], 4, false, 30.0, || async { None })
            .await;
        assert_eq!(d4.action, "yielded");

        assert_eq!(
            ev.step(&cfg, &[], 4, false, 31.0, || async { None }).await.action,
            "yielded"
        );
        assert_eq!(
            ev.step(&cfg, &[], 4, false, 37.0, || async { None }).await.action,
            "noop"
        );

        let d5 = ev
            .step(&cfg, &[vec!["blender".into()]], 4, true, 40.0, || async {
                Some(baseline.clone())
            })
            .await;
        assert_eq!(d5.action, "fan_safety_paused");

        assert_eq!(
            ev.step(&cfg, &[vec!["blender".into()]], 4, false, 41.0, || async {
                Some(baseline.clone())
            })
            .await
            .action,
            "apply"
        );
        assert_eq!(
            ev.step(&cfg, &[], 4, false, 42.0, || async { None }).await.action,
            "hold"
        );
        let rel = ev.step(&cfg, &[], 4, false, 48.0, || async { None }).await;
        assert_eq!(rel.action, "release");
        assert_eq!(rel.restore_profile, Some(baseline));
        let _ = PathBuf::new();
    }
}

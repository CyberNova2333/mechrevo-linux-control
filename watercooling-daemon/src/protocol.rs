// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Pure LCT22002 / LCT21001 BLE protocol framing, config validation, telemetry readers,
//! thermal-curve evaluator, and flow-safety state machine.
//!
//! 100% compatible with `iteglow/watercooling.py` JSON schema and wire protocol.

pub use iteglow_watercooling_config::*;
use serde::{Deserialize, Serialize};
use serde_json::Value;
use std::fs;
use std::path::{Path, PathBuf};

#[allow(dead_code)]
pub const NUS_SERVICE_UUID: &str = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
pub const NUS_TX_UUID: &str = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";
pub const NUS_RX_UUID: &str = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";
pub const DEVICE_NAME_PREFIXES: &[&str] = &["CoolingSystem LCT22002", "CoolingSystem LCT21001"];

pub const DEFAULT_WMI_STATE_PATH: &str = "/run/mechrevo-wmi-state.json";
pub const DEFAULT_LC_EC_STATE_PATH: &str = "/run/mechrevo-watercooler-ec.json";

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

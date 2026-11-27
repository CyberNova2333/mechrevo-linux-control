// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Session lifecycle gate (`Lifecycle`) and hardware Fn+F6/Fn+F7 (WMI 0xC1)
//! keyboard brightness synchronization (`KeyboardHotkeySync`).

use crate::profiles::{latest_wmi_code_seq, read_wmi_snapshot, WMI_STATE_PATH};
use crate::protocol::{
    decode_keyboard_state, query_keyboard_hw_locked, validate_hid, KeyboardHwState,
    KBD_BRIGHTNESS_MAX,
};
use crate::zone_lock::{read_saved_json, write_state_json, ZoneGuard, MAX_STATE_BYTES};
use serde_json::{json, Map, Value};
use std::path::PathBuf;

pub const WMI_KBD_BRIGHTNESS_CODE: i64 = 193;

/// Session restore gate matching `iteglow.lighting_session.Lifecycle`.
#[derive(Debug, Default)]
pub struct Lifecycle {
    pub started: bool,
    pub sleeping: bool,
    pub stopped: bool,
}

impl Lifecycle {
    /// Returns `true` if a restore should be scheduled.
    pub fn startup(&mut self, active: bool) -> bool {
        if !self.started && !self.stopped {
            self.started = true;
            return active;
        }
        false
    }

    /// Returns `(cancel, schedule)`.
    pub fn prepare_for_sleep(&mut self, sleeping: bool, active: bool) -> (bool, bool) {
        if self.stopped {
            return (false, false);
        }
        if sleeping {
            self.sleeping = true;
            (true, false)
        } else if self.sleeping {
            self.sleeping = false;
            (false, active)
        } else {
            (false, false)
        }
    }

    pub fn stop(&mut self) {
        self.stopped = true;
    }

    pub fn can_restore(&self, active: bool) -> bool {
        !self.stopped && !self.sleeping && active
    }
}

/// Sync hardware Fn+F6 / Fn+F7 (and WMI 0xC1) keyboard brightness into `state.json`.
pub struct KeyboardHotkeySync {
    directory: PathBuf,
    uid: u32,
    wmi_state_path: PathBuf,
    last_manual: Option<Value>,
    last_hw: Option<KeyboardHwState>,
    last_wmi_seq: Option<i64>,
}

impl KeyboardHotkeySync {
    pub fn new(directory: PathBuf, uid: u32) -> Self {
        Self {
            directory,
            uid,
            wmi_state_path: PathBuf::from(WMI_STATE_PATH),
            last_manual: None,
            last_hw: None,
            last_wmi_seq: None,
        }
    }

    #[cfg(test)]
    pub fn with_wmi_path(directory: PathBuf, uid: u32, wmi_state_path: PathBuf) -> Self {
        Self {
            directory,
            uid,
            wmi_state_path,
            last_manual: None,
            last_hw: None,
            last_wmi_seq: None,
        }
    }

    pub fn reset(&mut self) {
        self.last_hw = None;
    }

    fn current_wmi_seq(&self) -> Option<i64> {
        let raw = read_wmi_snapshot(&self.wmi_state_path)?;
        let seq = latest_wmi_code_seq(Some(&raw), WMI_KBD_BRIGHTNESS_CODE);
        if seq > 0 {
            Some(seq)
        } else {
            None
        }
    }

    fn read_manual_kbd(&self) -> (Option<Value>, Option<Value>) {
        match read_saved_json(&self.directory.join("state.json"), self.uid, MAX_STATE_BYTES) {
            Ok(Some(saved @ Value::Object(_))) => {
                let kbd = saved.get("keyboard").cloned();
                (Some(saved), kbd)
            }
            Ok(saved) => (saved, None),
            Err(_) => (None, None),
        }
    }

    /// Refresh manual & hardware snapshots after our own restore/policy write.
    /// Caller should hold `ZoneGuard("keyboard")`.
    pub fn record_applied_locked<FQuery>(&mut self, mut query_kbd: FQuery)
    where
        FQuery: FnMut() -> Option<KeyboardHwState>,
    {
        let (_, manual) = self.read_manual_kbd();
        self.last_manual = manual;
        self.last_hw = query_kbd();
        if let Some(seq) = self.current_wmi_seq() {
            self.last_wmi_seq = Some(seq);
        }
    }

    pub fn record_applied_hw(&mut self) {
        let Ok(_guard) = ZoneGuard::acquire("keyboard") else {
            return;
        };
        self.record_applied_locked(|| query_keyboard_hw_locked().ok());
    }

    pub fn sync_if_needed_hw(&mut self, force_query: bool) -> Option<Value> {
        self.sync_if_needed(force_query, true, || query_keyboard_hw_locked().ok())
    }

    pub fn sync_if_needed<FQuery>(
        &mut self,
        force_query: bool,
        use_zone_lock: bool,
        mut query_kbd: FQuery,
    ) -> Option<Value>
    where
        FQuery: FnMut() -> Option<KeyboardHwState>,
    {
        let (_, manual) = self.read_manual_kbd();
        if manual != self.last_manual {
            self.last_manual = manual;
            let _guard = if use_zone_lock {
                ZoneGuard::acquire("keyboard").ok()
            } else {
                None
            };
            self.last_hw = query_kbd();
            if let Some(seq) = self.current_wmi_seq() {
                self.last_wmi_seq = Some(seq);
            }
            return None;
        }

        let seq = self.current_wmi_seq();
        let wmi_changed = seq.is_some() && seq != self.last_wmi_seq;
        if let Some(s) = seq {
            self.last_wmi_seq = Some(s);
        }
        if !(wmi_changed || force_query) {
            return None;
        }

        let _kbd_guard = if use_zone_lock {
            match ZoneGuard::acquire("keyboard") {
                Ok(g) => Some(g),
                Err(_) => return None,
            }
        } else {
            None
        };

        let (full_state, current_manual) = self.read_manual_kbd();
        if current_manual != self.last_manual {
            self.last_manual = current_manual;
            self.last_hw = query_kbd();
            return None;
        }

        let actual = query_kbd()?;
        let Some(prev_hw) = self.last_hw else {
            self.last_hw = Some(actual);
            return None;
        };

        let mut changed = actual.power != prev_hw.power || actual.brightness != prev_hw.brightness;
        if !changed && actual.power != 1 && actual.brightness > 0 {
            changed = actual.mode != prev_hw.mode;
        }
        if !changed {
            return None;
        }
        self.last_hw = Some(actual);

        let new_bright: u8 = if actual.power == 1 || actual.brightness == 0 {
            0
        } else {
            actual.brightness.min(KBD_BRIGHTNESS_MAX)
        };

        let updated_val = if let Some(Value::Object(mut updated)) = current_manual {
            let saved_mode = updated
                .get("mode")
                .and_then(|v| v.as_str())
                .map(|s| s.to_string());
            if new_bright > 0 {
                let (mode_name, ui_dir) = decode_keyboard_state(actual, saved_mode.as_deref());
                if saved_mode.as_deref() == Some("off")
                    || saved_mode.as_deref() != Some(mode_name)
                {
                    updated.insert("mode".to_string(), json!(mode_name));
                    updated.insert("direction".to_string(), json!(ui_dir));
                    if (1..=10).contains(&actual.speed) {
                        updated.insert("speed".to_string(), json!(actual.speed));
                    }
                }
            }
            updated.insert("brightness".to_string(), json!(new_bright));
            Value::Object(updated)
        } else {
            let (mode_name, ui_dir) = decode_keyboard_state(actual, None);
            json!({
                "mode": mode_name,
                "brightness": new_bright,
                "direction": ui_dir,
            })
        };

        validate_hid("keyboard", &updated_val).ok()?;

        let mut next_data = match full_state {
            Some(Value::Object(m)) => m,
            _ => Map::new(),
        };
        let mode_str = updated_val["mode"].as_str().unwrap_or("off").to_string();
        next_data.insert("keyboard".to_string(), updated_val.clone());

        let _state_guard = if use_zone_lock {
            match ZoneGuard::acquire("manual-state") {
                Ok(g) => Some(g),
                Err(_) => return None,
            }
        } else {
            None
        };
        write_state_json(&self.directory.join("state.json"), &Value::Object(next_data)).ok()?;
        self.last_manual = Some(updated_val);
        Some(json!({
            "hotkey_sync": "keyboard",
            "mode": mode_str,
            "brightness": new_bright,
        }))
    }
}

pub fn check_eligible_session_props(
    user_uid: Option<u32>,
    seat_id: Option<&str>,
    active: Option<bool>,
    remote: Option<bool>,
    state: Option<&str>,
    class: Option<&str>,
    session_type: Option<&str>,
    expected_uid: u32,
) -> bool {
    user_uid == Some(expected_uid)
        && seat_id.map(|s| !s.is_empty()).unwrap_or(false)
        && active == Some(true)
        && remote == Some(false)
        && state == Some("active")
        && class == Some("user")
        && matches!(session_type, Some("x11" | "wayland"))
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::fs;
    use std::time::Instant;

    #[test]
    fn test_lifecycle_sleep_and_stop() {
        let mut lc = Lifecycle::default();
        assert!(lc.startup(true));
        assert!(!lc.startup(true));
        assert_eq!(lc.prepare_for_sleep(false, true), (false, false));
        assert_eq!(lc.prepare_for_sleep(true, true), (true, false));
        assert!(!lc.can_restore(true));
        assert_eq!(lc.prepare_for_sleep(false, true), (false, true));
        assert_eq!(lc.prepare_for_sleep(false, true), (false, false));
        assert!(lc.can_restore(true));
        lc.stop();
        assert_eq!(lc.prepare_for_sleep(true, true), (false, false));
        assert_eq!(lc.prepare_for_sleep(false, true), (false, false));
        assert!(!lc.can_restore(true));
    }

    #[test]
    fn test_hotkey_sync_updates_state_on_wmi_c1() {
        let dir = std::env::temp_dir().join(format!(
            "iteglow-hotkey-test-{}-{}",
            std::process::id(),
            Instant::now().elapsed().as_nanos()
        ));
        let _ = fs::remove_dir_all(&dir);
        fs::create_dir_all(&dir).unwrap();
        let uid = unsafe { libc::getuid() };

        write_state_json(
            &dir.join("state.json"),
            &json!({"keyboard": {"mode": "matrix", "brightness": 35}}),
        )
        .unwrap();
        let wmi_path = dir.join("wmi.json");
        fs::write(
            &wmi_path,
            r#"{"wmi_seq": 10, "wmi_last_code": 193, "wmi_events": [{"seq": 10, "code": 193}]}"#,
        )
        .unwrap();

        let mut hw = KeyboardHwState {
            power: 2,
            mode: 0x33,
            speed: 5,
            brightness: 35,
            direction: 1,
        };
        let mut sync = KeyboardHotkeySync::with_wmi_path(dir.clone(), uid, wmi_path.clone());
        sync.record_applied_locked(|| Some(hw));

        // No WMI change and force_query=false -> None
        assert!(sync.sync_if_needed(false, false, || Some(hw)).is_none());

        // Hardware Fn+F7 raises brightness to 50 and WMI 0xC1 seq increments
        hw.brightness = 50;
        fs::write(
            &wmi_path,
            r#"{"wmi_seq": 11, "wmi_last_code": 193, "wmi_events": [{"seq": 11, "code": 193}]}"#,
        )
        .unwrap();
        let ev = sync.sync_if_needed(false, false, || Some(hw)).unwrap();
        assert_eq!(ev["hotkey_sync"], "keyboard");
        assert_eq!(ev["brightness"], 50);

        let updated = read_saved_json(&dir.join("state.json"), uid, MAX_STATE_BYTES)
            .unwrap()
            .unwrap();
        assert_eq!(updated["keyboard"]["brightness"], 50);
        assert_eq!(updated["keyboard"]["mode"], "matrix");

        // Hardware mode hotkey switches from matrix (0x33) to wave (0x03)
        hw.mode = 0x03;
        hw.direction = 2;
        hw.speed = 4;
        hw.brightness = 36;
        fs::write(
            &wmi_path,
            r#"{"wmi_seq": 12, "wmi_last_code": 193, "wmi_events": [{"seq": 12, "code": 193}]}"#,
        )
        .unwrap();
        let ev2 = sync.sync_if_needed(false, false, || Some(hw)).unwrap();
        assert_eq!(ev2["mode"], "wave");
        assert_eq!(ev2["brightness"], 36);
        let updated2 = read_saved_json(&dir.join("state.json"), uid, MAX_STATE_BYTES)
            .unwrap()
            .unwrap();
        assert_eq!(updated2["keyboard"]["mode"], "wave");
        assert_eq!(updated2["keyboard"]["direction"], 2);
        assert_eq!(updated2["keyboard"]["speed"], 4);

        let _ = fs::remove_dir_all(&dir);
    }

    #[test]
    fn test_eligible_session_checks() {
        let uid = 1000;
        assert!(check_eligible_session_props(
            Some(uid),
            Some("seat0"),
            Some(true),
            Some(false),
            Some("active"),
            Some("user"),
            Some("x11"),
            uid
        ));
        assert!(!check_eligible_session_props(
            Some(uid + 1),
            Some("seat0"),
            Some(true),
            Some(false),
            Some("active"),
            Some("user"),
            Some("x11"),
            uid
        ));
        assert!(!check_eligible_session_props(
            Some(uid),
            Some(""),
            Some(true),
            Some(false),
            Some("active"),
            Some("user"),
            Some("wayland"),
            uid
        ));
        assert!(!check_eligible_session_props(
            Some(uid),
            Some("seat0"),
            Some(true),
            Some(false),
            Some("active"),
            Some("user"),
            Some("tty"),
            uid
        ));
    }
}

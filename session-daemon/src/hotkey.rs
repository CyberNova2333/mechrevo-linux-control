// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Session lifecycle gate (`Lifecycle`) and hardware Fn+F6/Fn+F7 (WMI 0xC1)
//! keyboard brightness synchronization (`KeyboardHotkeySync`).

use crate::profiles::{latest_wmi_code_seq, read_wmi_snapshot, WMI_STATE_PATH};
use crate::protocol::{
    apply_hid_locked, decode_keyboard_state, query_keyboard_hw_locked, validate_hid,
    KeyboardHwState, RestoreAction, KBD_BRIGHTNESS_MAX,
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
    drift_mode: Option<(u8, u8)>,
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
            drift_mode: None,
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
            drift_mode: None,
        }
    }

    pub fn reset(&mut self) {
        self.last_hw = None;
        self.drift_mode = None;
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
        match read_saved_json(
            &self.directory.join("state.json"),
            self.uid,
            MAX_STATE_BYTES,
        ) {
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
        self.drift_mode = None;
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
        self.sync_if_needed(
            force_query,
            true,
            || query_keyboard_hw_locked().ok(),
            apply_hid_locked,
        )
    }

    pub fn sync_if_needed(
        &mut self,
        force_query: bool,
        use_zone_lock: bool,
        mut query_kbd: impl FnMut() -> Option<KeyboardHwState>,
        mut write_kbd: impl FnMut(&RestoreAction) -> Result<(), String>,
    ) -> Option<Value> {
        let seq = self.current_wmi_seq();
        let wmi_changed = seq.is_some() && seq != self.last_wmi_seq;
        if !force_query && !wmi_changed && self.read_manual_kbd().1 == self.last_manual {
            return None;
        }

        let _kbd_guard = if use_zone_lock {
            Some(ZoneGuard::acquire("keyboard").ok()?)
        } else {
            None
        };
        let (_, current_manual) = self.read_manual_kbd();
        let mut actual = query_kbd()?;
        if current_manual != self.last_manual || self.last_hw.is_none() {
            self.drift_mode = None;
            self.last_manual = current_manual;
            self.last_hw = Some(actual);
            self.last_wmi_seq = seq;
            return None;
        }
        let prev_hw = self.last_hw?;
        let saved_mode = current_manual.as_ref().and_then(|v| v["mode"].as_str());
        let lit = actual.power != 1 && actual.brightness > 0;
        let (observed_mode, _) = decode_keyboard_state(actual, saved_mode);
        // The controller reports both a solid colour and an arbitrary matrix as
        // 0x33. A brightness notification cannot identify its per-key contents.
        let mismatch = lit && saved_mode.is_some_and(|mode| mode != "off" && mode != observed_mode);
        if mismatch && (!wmi_changed || actual.mode == 0x33) {
            let signature = (actual.mode, actual.direction);
            if self.drift_mode != Some(signature) {
                self.drift_mode = Some(signature);
                return None;
            }
            let mut params = current_manual.clone()?;
            // Preserve temporary policy brightness, including idle-off. Only a
            // real hotkey can contribute a new brightness to the manual file.
            let brightness = if wmi_changed {
                actual.brightness
            } else if prev_hw.power == 1 {
                0
            } else {
                prev_hw.brightness
            };
            params["brightness"] = json!(brightness.min(KBD_BRIGHTNESS_MAX));
            let action = validate_hid("keyboard", &params).ok()?;
            if let Err(err) = write_kbd(&action) {
                self.drift_mode = None;
                return Some(json!({"keyboard_restore_error": err}));
            }
            self.drift_mode = None;
            actual = query_kbd()?;
            let expected = if brightness == 0 { "off" } else { saved_mode? };
            if decode_keyboard_state(actual, saved_mode).0 != expected
                || actual.brightness != brightness
            {
                return Some(json!({"keyboard_restore_error": "灯效回读不一致"}));
            }
            if !wmi_changed || prev_hw.brightness == actual.brightness {
                self.last_hw = Some(actual);
                self.last_wmi_seq = seq;
                return Some(
                    json!({"keyboard_restored": saved_mode, "brightness": actual.brightness}),
                );
            }
            // Only persist the confirmed brightness below. Keep the saved effect.
        } else {
            self.drift_mode = None;
        }
        if !wmi_changed {
            return None;
        }
        if actual == prev_hw {
            self.last_wmi_seq = seq;
            return None;
        }
        let new_bright: u8 = if actual.power == 1 || actual.brightness == 0 {
            0
        } else {
            actual.brightness.min(KBD_BRIGHTNESS_MAX)
        };

        let updated_val = if let Some(Value::Object(mut updated)) = current_manual {
            let saved_mode = updated.get("mode").and_then(Value::as_str);
            if new_bright > 0 {
                let (mode_name, ui_dir) = decode_keyboard_state(actual, saved_mode);
                if saved_mode != Some(mode_name)
                    || actual.direction != prev_hw.direction
                    || actual.speed != prev_hw.speed
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
        let mode_str = updated_val["mode"].as_str().unwrap_or("off").to_string();

        let _state_guard = if use_zone_lock {
            match ZoneGuard::acquire("manual-state") {
                Ok(g) => Some(g),
                Err(_) => return None,
            }
        } else {
            None
        };
        // Re-read state.json while holding `manual-state` so concurrent lightbar writes are never clobbered.
        let state_path = self.directory.join("state.json");
        let mut next_data = match read_saved_json(&state_path, self.uid, MAX_STATE_BYTES).ok()? {
            Some(Value::Object(m)) if m.get("keyboard") == self.last_manual.as_ref() => m,
            None if self.last_manual.is_none() => Map::new(),
            _ => return None,
        };
        next_data.insert("keyboard".to_string(), updated_val.clone());

        write_state_json(
            &self.directory.join("state.json"),
            &Value::Object(next_data),
        )
        .ok()?;
        self.last_hw = Some(actual);
        if let Some(s) = seq {
            self.last_wmi_seq = Some(s);
        }
        self.last_manual = Some(updated_val);
        Some(json!({
            "hotkey_sync": "keyboard",
            "mode": mode_str,
            "brightness": new_bright,
        }))
    }
}

#[allow(clippy::too_many_arguments)]
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
        assert!(sync
            .sync_if_needed(false, false, || Some(hw), |_| panic!("unexpected restore"))
            .is_none());

        // Hardware Fn+F7 raises brightness to 50 and WMI 0xC1 seq increments
        hw.brightness = 50;
        fs::write(
            &wmi_path,
            r#"{"wmi_seq": 11, "wmi_last_code": 193, "wmi_events": [{"seq": 11, "code": 193}]}"#,
        )
        .unwrap();
        let ev = sync
            .sync_if_needed(false, false, || Some(hw), |_| panic!("unexpected restore"))
            .unwrap();
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
        let ev2 = sync
            .sync_if_needed(false, false, || Some(hw), |_| panic!("unexpected restore"))
            .unwrap();
        assert_eq!(ev2["mode"], "wave");
        assert_eq!(ev2["brightness"], 36);
        let updated2 = read_saved_json(&dir.join("state.json"), uid, MAX_STATE_BYTES)
            .unwrap()
            .unwrap();
        assert_eq!(updated2["keyboard"]["mode"], "wave");
        assert_eq!(updated2["keyboard"]["direction"], 2);
        assert_eq!(updated2["keyboard"]["speed"], 4);

        // Wake-up firmware can expose the old per-key bank, even with a C1.
        // Confirm it twice, replay the saved wave, and leave the file untouched.
        let saved_bytes = fs::read(dir.join("state.json")).unwrap();
        let drift = KeyboardHwState { mode: 0x33, ..hw };
        for seq in [12, 13] {
            fs::write(
                &wmi_path,
                json!({"wmi_seq": seq, "wmi_last_code": 193}).to_string(),
            )
            .unwrap();
            assert!(sync
                .sync_if_needed(true, false, || Some(drift), |_| panic!("transient read"))
                .is_none());
            let observed = std::cell::Cell::new(drift);
            let event = sync
                .sync_if_needed(
                    true,
                    false,
                    || Some(observed.get()),
                    |action| {
                        assert_eq!(
                            action,
                            &validate_hid("keyboard", &updated2["keyboard"]).unwrap()
                        );
                        observed.set(hw);
                        Ok(())
                    },
                )
                .unwrap();
            assert_eq!(event["keyboard_restored"], "wave");
            assert_eq!(fs::read(dir.join("state.json")).unwrap(), saved_bytes);
        }
        let dark = KeyboardHwState {
            brightness: 0,
            ..hw
        };
        assert!(sync
            .sync_if_needed(true, false, || Some(dark), |_| panic!("idle-off"))
            .is_none());
        assert_eq!(fs::read(dir.join("state.json")).unwrap(), saved_bytes);

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

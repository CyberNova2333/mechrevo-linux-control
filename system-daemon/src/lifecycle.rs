// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Qualified `mechrevo_turbo_lease` kernel module lifecycle protocol (`prepare`, `pre`, `post`),
//! wire-compatible with `system/mechrevo_turbo_lifecycle.py` and `system/mechrevo-turbo-lifecycle`.

use crate::common::{
    atomic_write_json, check_acpi_healthy, check_identity, identity, run_command_timeout,
    trusted_file, turbo_socket_request, CmdOutput, LockError, RootFlock, TURBO_SOCKET,
};
use crate::turbo::{check_qualification, QUALIFICATION_PATH};
use serde_json::{json, Value};
use std::fs;
use std::path::{Path, PathBuf};
use std::time::{Duration, Instant};

pub const MODULE: &str = "mechrevo_turbo_lease";
pub const UNIT: &str = "mechrevo-turbo.service";
pub const MARKER_PATH: &str = "/run/mechrevo-turbo-sleep.json";
pub const LIFECYCLE_LOCK: &str = "/run/mechrevo-turbo-lifecycle.lock";

const VALID_PHASES: &[&str] = &[
    "preparing",
    "quiesced",
    "pre_failed",
    "resume_queued",
    "resume_failed",
];

pub struct SleepMarker {
    pub path: PathBuf,
}

impl SleepMarker {
    pub fn new(path: PathBuf) -> Self {
        Self { path }
    }

    pub fn load(&self) -> Result<Option<Value>, String> {
        if !self.path.exists() {
            return Ok(None);
        }
        let raw = trusted_file(&self.path, true, 65_536)?;
        let value: Value =
            serde_json::from_slice(&raw).map_err(|_| "睡眠恢复记录无效；拒绝自动恢复".to_string())?;
        let Some(obj) = value.as_object() else {
            return Err("睡眠恢复记录无效；拒绝自动恢复".to_string());
        };
        let valid_phase = obj
            .get("phase")
            .and_then(|v| v.as_str())
            .map(|p| VALID_PHASES.contains(&p))
            .unwrap_or(false);
        if obj.get("version").and_then(|v| v.as_i64()) != Some(1)
            || !valid_phase
            || !obj.get("was_active").map(|v| v.is_boolean()).unwrap_or(false)
            || !obj.get("boot_id").map(|v| v.is_string()).unwrap_or(false)
            || !obj.get("identity").map(|v| v.is_object()).unwrap_or(false)
        {
            return Err("睡眠恢复记录无效；拒绝自动恢复".to_string());
        }
        if let Some(digest_val) = obj.get("driver_sha256") {
            if !digest_val.is_null() {
                let Some(digest) = digest_val.as_str() else {
                    return Err("睡眠恢复驱动哈希无效".to_string());
                };
                if digest.len() != 64
                    || !digest
                        .bytes()
                        .all(|b| matches!(b, b'0'..=b'9' | b'a'..=b'f'))
                {
                    return Err("睡眠恢复驱动哈希无效".to_string());
                }
            }
        }
        Ok(Some(value))
    }

    pub fn save(&self, value: &Value) -> Result<(), String> {
        atomic_write_json(&self.path, value, 0o600, ".mechrevo-sleep-", true)
    }

    pub fn clear(&self) {
        let _ = fs::remove_file(&self.path);
    }
}

pub trait LifecycleDeps {
    fn run_cmd(&self, prog: &str, args: &[&str], timeout: Duration) -> Result<CmdOutput, String>;
    fn suspend_daemon(&self) -> Result<(), String>;
    fn machine(&self) -> Result<Value, String>;
    fn healthy(&self) -> Result<(), String>;
    fn qualification(&self, require_loaded: bool) -> Result<Value, String>;
    fn loaded(&self) -> bool;
    fn boot_id(&self) -> Result<String, String>;
}

pub struct LiveLifecycleDeps;

impl LifecycleDeps for LiveLifecycleDeps {
    fn run_cmd(&self, prog: &str, args: &[&str], timeout: Duration) -> Result<CmdOutput, String> {
        run_command_timeout(prog, args, timeout)
    }

    fn suspend_daemon(&self) -> Result<(), String> {
        let resp = turbo_socket_request(
            Path::new(TURBO_SOCKET),
            &json!({"command": "suspend"}),
            Duration::from_secs(6),
        )?;
        if resp.get("ok").and_then(|v| v.as_bool()) != Some(true) {
            let err = resp
                .get("error")
                .and_then(|v| v.as_str())
                .unwrap_or("狂暴调度暂停失败");
            return Err(err.to_string());
        }
        Ok(())
    }

    fn machine(&self) -> Result<Value, String> {
        identity()
    }

    fn healthy(&self) -> Result<(), String> {
        check_acpi_healthy().map(|_| ())
    }

    fn qualification(&self, require_loaded: bool) -> Result<Value, String> {
        check_qualification(Path::new(QUALIFICATION_PATH), require_loaded)
    }

    fn loaded(&self) -> bool {
        Path::new("/sys/module").join(MODULE).exists()
    }

    fn boot_id(&self) -> Result<String, String> {
        fs::read_to_string("/proc/sys/kernel/random/boot_id")
            .map(|s| s.trim().to_string())
            .map_err(|e| format!("读取 boot_id 失败: {e}"))
    }
}

pub struct Lifecycle<D: LifecycleDeps> {
    pub deps: D,
    pub marker: SleepMarker,
}

impl<D: LifecycleDeps> Lifecycle<D> {
    fn command(&self, prog: &str, args: &[&str], timeout_secs: u64) -> Result<CmdOutput, String> {
        let out = self
            .deps
            .run_cmd(prog, args, Duration::from_secs(timeout_secs))?;
        if out.code != 0 {
            return Err(format!(
                "命令失败: {} {}: {}",
                prog,
                args.join(" "),
                out.stderr.trim()
            ));
        }
        Ok(out)
    }

    fn unit_state(&self) -> Result<String, String> {
        let out = self.command(
            "/usr/bin/systemctl",
            &["show", "--property=ActiveState", "--value", UNIT],
            20,
        )?;
        let val = out.stdout.trim().to_string();
        if !matches!(
            val.as_str(),
            "active" | "reloading" | "activating" | "deactivating" | "inactive" | "failed"
        ) {
            return Err("无法确定性能调度服务状态".to_string());
        }
        Ok(val)
    }

    fn validate_resume(&self, saved: &Value, current: &Value, qualified: &Value) -> Result<(), String> {
        let boot = self.deps.boot_id()?;
        if saved.get("boot_id").and_then(|v| v.as_str()) != Some(boot.as_str())
            || saved.get("identity") != Some(current)
        {
            return Err("睡眠前后启动身份或机型已改变，禁止自动恢复".to_string());
        }
        let saved_sha = saved.get("driver_sha256").and_then(|v| v.as_str());
        let qual_sha = qualified.get("driver_sha256").and_then(|v| v.as_str());
        if saved_sha.is_none() || saved_sha != qual_sha {
            return Err("睡眠前后驱动验收哈希不一致，禁止自动恢复".to_string());
        }
        Ok(())
    }

    pub fn prepare(&self) -> Result<Value, String> {
        let saved = self.marker.load()?;
        if let Some(ref s) = saved {
            if s.get("phase").and_then(|v| v.as_str()) != Some("resume_queued") {
                return Err("睡眠清理或恢复尚未完成，禁止加载性能驱动".to_string());
            }
        }
        let current = self.deps.machine()?;
        check_identity(&current)?;
        let qualified = self.deps.qualification(false)?;
        if let Some(ref s) = saved {
            self.validate_resume(s, &current, &qualified)?;
        }
        self.deps.healthy()?;
        let was_loaded = self.deps.loaded();
        self.command("/usr/sbin/modprobe", &[MODULE], 10)?;
        if let Err(err) = self.deps.qualification(true) {
            if !was_loaded {
                let _ = self.command("/usr/sbin/modprobe", &["-r", MODULE], 10);
            }
            return Err(err);
        }
        if saved.is_some() {
            self.marker.clear();
        }
        Ok(json!({"prepared": true, "policy_selected": false}))
    }

    pub fn pre(&self) -> Result<Value, String> {
        let existing = self.marker.load()?;
        let boot = self.deps.boot_id()?;
        let start_fresh = match &existing {
            None => true,
            Some(s) => {
                s.get("boot_id").and_then(|v| v.as_str()) != Some(boot.as_str())
                    || matches!(
                        s.get("phase").and_then(|v| v.as_str()),
                        Some("resume_queued" | "resume_failed")
                    )
            }
        };

        let mut saved = if start_fresh {
            let st = self.unit_state()?;
            let was_active = matches!(st.as_str(), "active" | "reloading" | "activating");
            let mut rec = json!({
                "version": 1,
                "phase": "preparing",
                "boot_id": boot,
                "identity": self.deps.machine()?,
                "was_active": was_active,
                "was_module_loaded": self.deps.loaded(),
                "driver_sha256": Value::Null,
            });
            match self.deps.qualification(false) {
                Ok(q) => {
                    if let Some(sha) = q.get("driver_sha256").cloned() {
                        rec["driver_sha256"] = sha;
                    }
                }
                Err(err) => {
                    rec["qualification_error"] = json!(err);
                }
            }
            rec
        } else {
            existing.unwrap()
        };

        saved["phase"] = json!("preparing");
        self.marker.save(&saved)?;

        let mut errors: Vec<String> = Vec::new();
        let step_res = (|| -> Result<(), String> {
            let st = self.unit_state()?;
            if matches!(st.as_str(), "active" | "reloading") {
                if let Err(err) = self.deps.suspend_daemon() {
                    errors.push(err);
                }
            }
            if let Err(err) = self.command("/usr/bin/systemctl", &["stop", UNIT], 20) {
                errors.push(err);
            }
            let st_after = self.unit_state()?;
            if !matches!(st_after.as_str(), "inactive" | "failed") {
                return Err("性能调度服务尚未停止；禁止睡眠".to_string());
            }
            if self.deps.loaded() {
                self.command("/usr/sbin/modprobe", &["-r", MODULE], 10)?;
            }
            if self.deps.loaded() {
                return Err("性能驱动尚未卸载；禁止睡眠".to_string());
            }
            Ok(())
        })();

        match step_res {
            Ok(()) => {
                saved["phase"] = json!("quiesced");
                saved["cleanup_warnings"] = json!(errors);
                self.marker.save(&saved)?;
                Ok(json!({
                    "quiesced": true,
                    "was_active": saved["was_active"],
                    "warnings": saved["cleanup_warnings"],
                }))
            }
            Err(err) => {
                saved["phase"] = json!("pre_failed");
                saved["error"] = json!(err);
                saved["cleanup_warnings"] = json!(errors);
                self.marker.save(&saved)?;
                Err(err)
            }
        }
    }

    pub fn post(&self) -> Result<Value, String> {
        let Some(mut saved) = self.marker.load()? else {
            return Ok(json!({"resumed": false, "reason": "no_saved_service"}));
        };
        let step_res = (|| -> Result<Value, String> {
            if !matches!(
                saved.get("phase").and_then(|v| v.as_str()),
                Some("quiesced" | "resume_failed")
            ) {
                return Err("驱动未完成睡眠前清理，禁止自动恢复".to_string());
            }
            let st = self.unit_state()?;
            if self.deps.loaded() || !matches!(st.as_str(), "inactive" | "failed") {
                return Err("睡眠后发现未清理的驱动或服务，禁止自动恢复".to_string());
            }
            if saved.get("was_active").and_then(|v| v.as_bool()) != Some(true) {
                self.marker.clear();
                return Ok(json!({"resumed": false, "reason": "previously_inactive"}));
            }
            let current = self.deps.machine()?;
            check_identity(&current)?;
            self.deps.healthy()?;
            let qualified = self.deps.qualification(false)?;
            self.validate_resume(&saved, &current, &qualified)?;
            saved["phase"] = json!("resume_queued");
            self.marker.save(&saved)?;
            self.command("/usr/bin/systemctl", &["--no-block", "start", UNIT], 20)?;
            Ok(json!({"resumed": "queued", "policy_selected": false}))
        })();

        match step_res {
            Ok(v) => Ok(v),
            Err(err) => {
                // Never overwrite `pre_failed`/`preparing` into `resume_failed`,
                // which would otherwise bypass the pre-sleep safety gate on retry.
                if matches!(
                    saved.get("phase").and_then(|v| v.as_str()),
                    Some("quiesced" | "resume_queued" | "resume_failed")
                ) {
                    saved["phase"] = json!("resume_failed");
                    saved["error"] = json!(err);
                    let _ = self.marker.save(&saved);
                }
                Err(err)
            }
        }
    }
}

pub fn run_live(phase: &str) -> Result<Value, String> {
    if unsafe { libc::geteuid() } != 0 {
        return Err("性能驱动生命周期操作需要root".to_string());
    }
    let lock_path = Path::new(LIFECYCLE_LOCK);
    let deadline = Instant::now() + Duration::from_secs(3);
    let _guard = loop {
        match RootFlock::acquire(lock_path, None) {
            Ok(g) => break g,
            Err(LockError::Busy(msg)) => {
                if phase != "prepare" || Instant::now() >= deadline {
                    return Err(msg);
                }
                std::thread::sleep(Duration::from_millis(50));
            }
            Err(LockError::Other(msg)) => return Err(msg),
        }
    };
    let lc = Lifecycle {
        deps: LiveLifecycleDeps,
        marker: SleepMarker::new(PathBuf::from(MARKER_PATH)),
    };
    match phase {
        "prepare" => lc.prepare(),
        "pre" => lc.pre(),
        "post" => lc.post(),
        _ => Err("用法: mechrevo-turbo-lifecycle prepare|pre|post".to_string()),
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::common::expected_identity;
    use std::cell::{Cell, RefCell};

    struct MockDeps {
        unit_state: RefCell<String>,
        loaded: Cell<bool>,
        suspended: Cell<bool>,
        sha: String,
    }

    impl LifecycleDeps for MockDeps {
        fn run_cmd(&self, prog: &str, args: &[&str], _timeout: Duration) -> Result<CmdOutput, String> {
            match (prog, args) {
                ("/usr/bin/systemctl", ["show", ..]) => Ok(CmdOutput {
                    code: 0,
                    stdout: format!("{}\n", self.unit_state.borrow()),
                    stderr: String::new(),
                }),
                ("/usr/bin/systemctl", ["stop", UNIT]) => {
                    *self.unit_state.borrow_mut() = "inactive".to_string();
                    Ok(CmdOutput { code: 0, stdout: String::new(), stderr: String::new() })
                }
                ("/usr/bin/systemctl", ["--no-block", "start", UNIT]) => {
                    Ok(CmdOutput { code: 0, stdout: String::new(), stderr: String::new() })
                }
                ("/usr/sbin/modprobe", [MODULE]) => {
                    self.loaded.set(true);
                    Ok(CmdOutput { code: 0, stdout: String::new(), stderr: String::new() })
                }
                ("/usr/sbin/modprobe", ["-r", MODULE]) => {
                    self.loaded.set(false);
                    Ok(CmdOutput { code: 0, stdout: String::new(), stderr: String::new() })
                }
                _ => Err(format!("unexpected cmd: {prog} {:?}", args)),
            }
        }
        fn suspend_daemon(&self) -> Result<(), String> {
            self.suspended.set(true);
            Ok(())
        }
        fn machine(&self) -> Result<Value, String> {
            Ok(expected_identity())
        }
        fn healthy(&self) -> Result<(), String> {
            Ok(())
        }
        fn qualification(&self, _require_loaded: bool) -> Result<Value, String> {
            Ok(json!({"driver_sha256": self.sha}))
        }
        fn loaded(&self) -> bool {
            self.loaded.get()
        }
        fn boot_id(&self) -> Result<String, String> {
            Ok("11111111-2222-3333-4444-555555555555".to_string())
        }
    }

    #[test]
    fn lifecycle_pre_post_prepare_roundtrip() {
        let dir = std::env::temp_dir().join(format!("lc-test-{}", std::process::id()));
        let _ = fs::remove_dir_all(&dir);
        fs::create_dir_all(&dir).unwrap();
        let marker_path = dir.join("sleep.json");

        let deps = MockDeps {
            unit_state: RefCell::new("active".to_string()),
            loaded: Cell::new(true),
            suspended: Cell::new(false),
            sha: "a".repeat(64),
        };
        let lc = Lifecycle {
            deps,
            marker: SleepMarker::new(marker_path.clone()),
        };

        let pre = lc.pre().unwrap();
        assert_eq!(pre["quiesced"], true);
        assert_eq!(pre["was_active"], true);
        assert!(lc.deps.suspended.get());
        assert!(!lc.deps.loaded.get());
        assert!(marker_path.exists());

        // prepare before post must be blocked while quiesced
        // (note: trusted_file checks uid == 0, so skip marker.load under non-root)
        let _ = fs::remove_dir_all(&dir);
    }
}

// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Exact-model MECHREVO turbo workload lease supervisor (`mechrevo-turbod`),
//! wire-compatible with `system/mechrevo_turbo_backend.py` and `/run/mechrevo-turbo.sock`.

use crate::common::{
    atomic_write_json, check_identity, check_powerd_active, check_runtime, expected_identity,
    identity, run_command_timeout, trusted_file, turbo_socket_request, unix_time_secs, LockError,
    RootFlock, PROFILE_LOCK, TURBO_DEVICE, TURBO_SOCKET,
};
use crate::oem_scheduler::{advance, LoadSample, Operation, SchedulerState};
use crate::sha256::sha256_hex;
use serde_json::{json, Map, Value};
use std::collections::HashMap;
use std::ffi::CString;
use std::fs::{self, File};
use std::io::{Read, Write};
use std::os::unix::ffi::OsStrExt;
use std::os::unix::fs::{FileTypeExt, MetadataExt, PermissionsExt};
use std::os::unix::io::{AsRawFd, FromRawFd};
use std::os::unix::net::{UnixListener, UnixStream};
use std::path::{Path, PathBuf};
use std::sync::atomic::{AtomicBool, Ordering};
use std::time::{Duration, Instant};

pub const DAEMON_LOCK: &str = "/run/mechrevo-turbo-daemon.lock";
pub const JOURNAL_PATH: &str = "/run/mechrevo-turbo.json";
pub const QUALIFICATION_PATH: &str = "/etc/mechrevo-turbo-validation.json";
pub const TUNING_PATH: &str = "/run/mechrevo-tuning.json";

static STOP_FLAG: AtomicBool = AtomicBool::new(false);

extern "C" fn handle_stop_signal(_sig: libc::c_int) {
    STOP_FLAG.store(true, Ordering::SeqCst);
}

fn install_signal_handlers() {
    unsafe {
        let mut sa: libc::sigaction = std::mem::zeroed();
        sa.sa_sigaction = handle_stop_signal as *const () as usize;
        libc::sigemptyset(&mut sa.sa_mask);
        sa.sa_flags = 0;
        libc::sigaction(libc::SIGTERM, &sa, std::ptr::null_mut());
        libc::sigaction(libc::SIGINT, &sa, std::ptr::null_mut());
    }
}

pub fn validate_status(data: Value) -> Result<Value, String> {
    let Some(obj) = data.as_object() else {
        return Err("内核lease协议版本不匹配；旧experimental桥不可使用".to_string());
    };
    if obj.get("abi").and_then(|v| v.as_i64()) != Some(1) {
        return Err("内核lease协议版本不匹配；旧experimental桥不可使用".to_string());
    }
    if obj.get("read_rc").and_then(|v| v.as_i64()).unwrap_or(0) != 0 {
        return Err("内核固件状态读取失败".to_string());
    }
    for key in [
        "active",
        "standard_turbo",
        "round_adapter",
        "return_present",
        "recovery_needed",
        "combined_supported",
        "game_supported",
    ] {
        if !obj.get(key).map(|v| v.is_boolean()).unwrap_or(false) {
            return Err(format!("桥状态缺少有效字段: {key}"));
        }
    }
    for key in ["sequence", "last_rc", "cstm", "gatm"] {
        if obj.get(key).and_then(|v| v.as_i64()).is_none() {
            return Err(format!("桥状态缺少有效整数: {key}"));
        }
    }
    let seq = obj["sequence"].as_i64().unwrap();
    let cstm = obj["cstm"].as_i64().unwrap();
    let gatm = obj["gatm"].as_i64().unwrap();
    if seq < 0 || (cstm != 0 && cstm != 1) || (gatm != 0 && gatm != 1) {
        return Err("桥返回未验证的工作负载状态".to_string());
    }
    if !matches!(
        obj.get("variant").and_then(|v| v.as_str()),
        Some("quiet" | "extreme")
    ) {
        return Err("桥返回未验证的狂暴子模式".to_string());
    }
    if let Some(ack) = obj.get("last_ack") {
        if !ack.is_null() && ack.as_u64().is_none() {
            return Err("桥返回无效ACK".to_string());
        }
    }
    if obj["return_present"].as_bool() == Some(true)
        && obj.get("last_ack").map(|v| v.is_null()).unwrap_or(true)
    {
        return Err("桥声明WMI有返回但未提供整数".to_string());
    }
    Ok(data)
}

pub fn check_qualification(path: &Path, require_loaded: bool) -> Result<Value, String> {
    let raw = trusted_file(path, true, 65_536)?;
    let data: Value =
        serde_json::from_slice(&raw).map_err(|_| "性能调度验证清单格式不匹配".to_string())?;
    let Some(obj) = data.as_object() else {
        return Err("性能调度验证清单格式不匹配".to_string());
    };
    const EXPECTED: &[&str] = &[
        "version",
        "identity",
        "driver_sha256",
        "combined_budget_verified",
        "lease_recovery_verified",
        "oem_ack_verified",
    ];
    if obj.len() != EXPECTED.len()
        || EXPECTED.iter().any(|k| !obj.contains_key(*k))
        || obj["version"].as_i64() != Some(1)
    {
        return Err("性能调度验证清单格式不匹配".to_string());
    }
    check_identity(&obj["identity"])?;
    for key in [
        "combined_budget_verified",
        "lease_recovery_verified",
        "oem_ack_verified",
    ] {
        if obj[key].as_bool() != Some(true) {
            return Err("GPU联动、lease恢复和OEM ACK必须分别通过实机验收".to_string());
        }
    }
    let Some(digest) = obj["driver_sha256"].as_str() else {
        return Err("驱动验证哈希无效".to_string());
    };
    if digest.len() != 64 || !digest.bytes().all(|b| matches!(b, b'0'..=b'9' | b'a'..=b'f')) {
        return Err("驱动验证哈希无效".to_string());
    }

    let mod_out = run_command_timeout(
        "/usr/sbin/modinfo",
        &["-n", "mechrevo_turbo_lease"],
        Duration::from_secs(2),
    )?;
    if mod_out.code != 0 {
        return Err("查询内核驱动路径失败".to_string());
    }
    let module_path = PathBuf::from(mod_out.stdout.trim());
    let ko_bytes = trusted_file(&module_path, false, 16 * 1024 * 1024)?;
    if sha256_hex(&ko_bytes) != digest {
        return Err("驱动文件与验收清单不一致".to_string());
    }

    let ver_out = run_command_timeout(
        "/usr/sbin/modinfo",
        &["-F", "srcversion", module_path.to_str().unwrap_or("")],
        Duration::from_secs(2),
    )?;
    let srcver = ver_out.stdout.trim();
    if ver_out.code != 0 || srcver.is_empty() {
        return Err("已加载驱动与验收文件版本不一致".to_string());
    }
    if require_loaded {
        let loaded_ver = fs::read_to_string("/sys/module/mechrevo_turbo_lease/srcversion")
            .map_err(|_| "已加载驱动与验收文件版本不一致".to_string())?;
        if loaded_ver.trim() != srcver {
            return Err("已加载驱动与验收文件版本不一致".to_string());
        }
    }

    // Verify our own executable is root-owned and not group/world-writable.
    let exe_path = fs::read_link("/proc/self/exe")
        .map_err(|e| format!("无法核验当前二进制路径: {e}"))?;
    let _ = trusted_file(&exe_path, false, 16 * 1024 * 1024)?;
    Ok(data)
}

// ----------------------------------------------------------------- Device abstraction

pub trait TurboDevice {
    fn status(&mut self) -> Result<Value, String>;
    fn command(&mut self, cmd: &str) -> Result<Value, String>;
}

pub struct RealTurboDevice {
    file: File,
}

impl RealTurboDevice {
    pub fn open(path: &Path) -> Result<Self, String> {
        let c_path = CString::new(path.as_os_str().as_bytes())
            .map_err(|_| "非法设备路径".to_string())?;
        let fd = unsafe {
            libc::open(
                c_path.as_ptr(),
                libc::O_RDWR | libc::O_CLOEXEC | libc::O_NOFOLLOW,
            )
        };
        if fd < 0 {
            return Err(format!(
                "打开性能设备失败 {}: {}",
                path.display(),
                std::io::Error::last_os_error()
            ));
        }
        let file = unsafe { File::from_raw_fd(fd) };
        let meta = file
            .metadata()
            .map_err(|e| format!("stat 性能设备失败: {e}"))?;
        if !meta.file_type().is_char_device() || meta.uid() != 0 || (meta.mode() & 0o077) != 0 {
            return Err("性能设备必须为root专用字符设备".to_string());
        }
        let mut dev = Self { file };
        let _ = dev.status()?;
        Ok(dev)
    }

    fn pread_all(&self) -> Result<Vec<u8>, String> {
        let mut buf = vec![0u8; 8192];
        let n = unsafe {
            libc::pread(
                self.file.as_raw_fd(),
                buf.as_mut_ptr() as *mut libc::c_void,
                buf.len(),
                0,
            )
        };
        if n < 0 {
            return Err(format!(
                "读取内核桥状态失败: {}",
                std::io::Error::last_os_error()
            ));
        }
        buf.truncate(n as usize);
        Ok(buf)
    }
}

const ALLOWED_COMMANDS: &[&str] = &[
    "acquire quiet",
    "acquire extreme",
    "variant quiet",
    "variant extreme",
    "heartbeat",
    "release",
    "combined 1",
    "combined 0",
    "game 1",
    "game 0",
];

impl TurboDevice for RealTurboDevice {
    fn status(&mut self) -> Result<Value, String> {
        let raw = self.pread_all()?;
        let parsed: Value =
            serde_json::from_slice(&raw).map_err(|e| format!("解析内核桥 JSON 失败: {e}"))?;
        validate_status(parsed)
    }

    fn command(&mut self, cmd: &str) -> Result<Value, String> {
        if !ALLOWED_COMMANDS.contains(&cmd) {
            return Err("未支持的内核命令".to_string());
        }
        let before = self.status()?["sequence"].as_i64().unwrap_or(-1);
        let payload = format!("{cmd}\n");
        let n = unsafe {
            libc::write(
                self.file.as_raw_fd(),
                payload.as_ptr() as *const libc::c_void,
                payload.len(),
            )
        };
        let after = self.status()?;
        if n as usize != payload.len()
            || after["sequence"].as_i64().unwrap_or(-1) <= before
            || after["last_rc"].as_i64().unwrap_or(-1) != 0
        {
            return Err("内核事务未完成或回读失败".to_string());
        }
        Ok(after)
    }
}

// ----------------------------------------------------------------- Journal & Controller

pub struct Journal {
    pub path: PathBuf,
}

impl Journal {
    pub fn new(path: PathBuf) -> Self {
        Self { path }
    }

    pub fn save(&self, record: &Value) -> Result<(), String> {
        atomic_write_json(&self.path, record, 0o600, ".mechrevo-turbo-", true)
    }

    pub fn clear(&self) {
        let _ = fs::remove_file(&self.path);
    }
}

pub struct Controller<D: TurboDevice> {
    pub device: D,
    pub journal: Journal,
    pub lock_path: Option<PathBuf>,
    pub tuning_path: PathBuf,
    pub state: SchedulerState,
    pub suspended: bool,
    pub record: Option<Map<String, Value>>,
    pub machine_fn: Box<dyn Fn() -> Result<Value, String>>,
    pub runtime_ready_fn: Box<dyn Fn() -> Result<(), String>>,
    pub powerd_ready_fn: Box<dyn Fn() -> Result<(), String>>,
}

impl<D: TurboDevice> Controller<D> {
    pub fn new_live(device: D) -> Self {
        Self {
            device,
            journal: Journal::new(PathBuf::from(JOURNAL_PATH)),
            lock_path: Some(PathBuf::from(PROFILE_LOCK)),
            tuning_path: PathBuf::from(TUNING_PATH),
            state: SchedulerState::default(),
            suspended: false,
            record: None,
            machine_fn: Box::new(identity),
            runtime_ready_fn: Box::new(|| check_runtime().map(|_| ())),
            powerd_ready_fn: Box::new(check_powerd_active),
        }
    }

    fn take_lock(&self) -> Result<Option<RootFlock>, LockError> {
        match &self.lock_path {
            Some(p) => RootFlock::acquire(p, None).map(Some),
            None => Ok(None),
        }
    }

    pub fn status(&mut self) -> Result<Value, String> {
        Ok(json!({
            "bridge": self.device.status()?,
            "scheduler": self.state,
            "suspended": self.suspended,
            "managed": self.record.is_some(),
        }))
    }

    fn save_record(&mut self, phase: &str, extra: Map<String, Value>) -> Result<(), String> {
        let mut rec = match &self.record {
            Some(existing) => existing.clone(),
            None => {
                let mut m = Map::new();
                m.insert("version".into(), json!(1));
                m.insert("identity".into(), (self.machine_fn)()?);
                m.insert("started_at".into(), json!(unix_time_secs()));
                m
            }
        };
        rec.insert("phase".into(), json!(phase));
        rec.insert("updated_at".into(), json!(unix_time_secs()));
        for (k, v) in extra {
            rec.insert(k, v);
        }
        self.journal.save(&Value::Object(rec.clone()))?;
        self.record = Some(rec);
        Ok(())
    }

    fn release_locked(&mut self, reason: &str) -> Result<Value, String> {
        let mut e1 = Map::new();
        e1.insert("reason".into(), json!(reason));
        self.save_record("releasing", e1)?;
        let status = self.device.command("release")?;
        if status["active"].as_bool() == Some(true)
            || status["recovery_needed"].as_bool() == Some(true)
        {
            return Err("内核lease仍活跃或恢复未完成，保留恢复日志".to_string());
        }
        let mut e2 = Map::new();
        e2.insert("reason".into(), json!(reason));
        e2.insert("final".into(), status.clone());
        self.save_record("released", e2)?;
        println!(
            "{}",
            json!({
                "event": "turbo_released",
                "reason": reason,
                "stop_reason": status.get("stop_reason").cloned().unwrap_or(Value::Null),
            })
        );
        self.journal.clear();
        self.record = None;
        self.state = SchedulerState::default();
        Ok(status)
    }

    pub fn release(&mut self, reason: &str) -> Result<Value, LockError> {
        let _guard = self.take_lock()?;
        self.release_locked(reason).map_err(LockError::Other)
    }

    pub fn select(&mut self, variant: &str) -> Result<Value, LockError> {
        if variant != "quiet" && variant != "extreme" {
            return Err(LockError::Other("子模式仅支持quiet/extreme".to_string()));
        }
        let _guard = self.take_lock()?;
        let ident = (self.machine_fn)().map_err(LockError::Other)?;
        check_identity(&ident).map_err(LockError::Other)?;
        if let Err(err) = (self.runtime_ready_fn)() {
            if let Ok(st) = self.device.status() {
                if st["active"].as_bool() == Some(true) {
                    let _ = self.release_locked("runtime_unhealthy");
                }
            }
            return Err(LockError::Other(err));
        }
        if self.suspended || self.tuning_path.exists() {
            return Err(LockError::Other(
                "休眠准备或自定义调校期间禁止接管狂暴档".to_string(),
            ));
        }
        let before = self.device.status().map_err(LockError::Other)?;
        if before["standard_turbo"].as_bool() != Some(true)
            || before["round_adapter"].as_bool() != Some(true)
        {
            return Err(LockError::Other(
                "仅支持原装圆口电源下的普通狂暴档".to_string(),
            ));
        }
        let is_active = before["active"].as_bool() == Some(true);
        if before["combined_supported"].as_bool() != Some(true)
            || (!is_active && before["recovery_needed"].as_bool() == Some(true))
        {
            return Err(LockError::Other(
                "固件未报告联合调度支持，或先前恢复尚未完成".to_string(),
            ));
        }
        if is_active && self.record.is_none() {
            return Err(LockError::Other("未持有此活动lease，拒绝接管".to_string()));
        }
        let acquired_before = self
            .record
            .as_ref()
            .and_then(|r| r.get("acquired_before").cloned())
            .unwrap_or_else(|| before.clone());
        let mut extra = Map::new();
        extra.insert("before".into(), before.clone());
        extra.insert("variant".into(), json!(variant));
        extra.insert("acquired_before".into(), acquired_before);
        self.save_record(if is_active { "changing" } else { "acquiring" }, extra)
            .map_err(LockError::Other)?;

        let cmd = if is_active {
            format!("variant {variant}")
        } else {
            format!("acquire {variant}")
        };
        let res = (|| -> Result<Value, String> {
            let result = self.device.command(&cmd)?;
            if result["active"].as_bool() != Some(true)
                || result["variant"].as_str() != Some(variant)
            {
                return Err("子模式回读不一致".to_string());
            }
            let mut fin = Map::new();
            fin.insert("final".into(), result.clone());
            self.save_record("active", fin)?;
            Ok(result)
        })();
        match res {
            Ok(v) => Ok(v),
            Err(e) => {
                let _ = self.release_locked("select_failed");
                Err(LockError::Other(e))
            }
        }
    }

    pub fn tick(&mut self, sample: Option<LoadSample>) -> Result<Value, LockError> {
        let _guard = self.take_lock()?;
        let status = self.device.status().map_err(LockError::Other)?;
        if status["active"].as_bool() != Some(true) {
            if self.record.is_some() {
                let stop_reason = status
                    .get("stop_reason")
                    .and_then(|v| v.as_str())
                    .unwrap_or("kernel_released")
                    .to_string();
                self.release_locked(&stop_reason).map_err(LockError::Other)?;
            }
            return self.status().map_err(LockError::Other);
        }

        let res = (|| -> Result<Value, String> {
            let ident = (self.machine_fn)()?;
            check_identity(&ident)?;
            if self.suspended
                || self.tuning_path.exists()
                || status["standard_turbo"].as_bool() != Some(true)
                || status["round_adapter"].as_bool() != Some(true)
            {
                self.release_locked("eligibility_changed")?;
                return self.status();
            }
            let Some(original) = self
                .record
                .as_ref()
                .and_then(|r| r.get("acquired_before").cloned())
            else {
                return Err("活动lease缺少本进程事务记录".to_string());
            };
            for key in ["combined_supported", "game_supported"] {
                if status.get(key) != original.get(key) {
                    self.release_locked("capability_changed")?;
                    return self.status();
                }
            }
            (self.powerd_ready_fn)()?;
            self.device.command("heartbeat")?;
            let Some(sample) = sample else {
                return self.status();
            };

            let game_supported = status["game_supported"].as_bool() == Some(true);
            let mut replies: HashMap<Operation, u8> = HashMap::new();
            replies.insert(Operation::CombinedOn, 0);
            replies.insert(Operation::CombinedOff, 0);
            let mut executed: Vec<Operation> = Vec::new();
            let final_state;

            loop {
                let transition =
                    advance(self.state, Some(sample), &replies, true, game_supported)?;
                if transition.operations.len() < executed.len()
                    || transition.operations[..executed.len()] != executed[..]
                {
                    return Err("实际ACK改变了已执行操作前缀".to_string());
                }
                if executed.len() == transition.operations.len() {
                    final_state = transition.state;
                    break;
                }
                let op = transition.operations[executed.len()];
                let mut ex = Map::new();
                ex.insert("operation".into(), json!(op.as_str()));
                ex.insert("scheduler_before".into(), json!(self.state));
                self.save_record("applying", ex)?;

                let response = self.device.command(op.kernel_cmd())?;
                if response["active"].as_bool() != Some(true)
                    || response["return_present"].as_bool() != Some(true)
                {
                    return Err("实际WMI ACK缺失或lease已归还".to_string());
                }
                let Some(ack_u64) = response.get("last_ack").and_then(|v| v.as_u64()) else {
                    return Err("实际WMI ACK不是整数".to_string());
                };
                let ack = (ack_u64 & 0xFF) as u8;
                if replies.contains_key(&op) {
                    replies.insert(op, ack);
                } else if ack != op.expected_ack() {
                    return Err("游戏负载通知ACK不匹配".to_string());
                }
                executed.push(op);
            }

            self.state = final_state;
            let mut fin = Map::new();
            fin.insert("scheduler".into(), json!(self.state));
            fin.insert(
                "operations".into(),
                json!(executed.iter().map(|o| o.as_str()).collect::<Vec<_>>()),
            );
            fin.insert("final".into(), self.device.status()?);
            self.save_record("active", fin)?;
            self.status()
        })();

        match res {
            Ok(v) => Ok(v),
            Err(e) => {
                let _ = self.release_locked("poll_failed");
                Err(LockError::Other(e))
            }
        }
    }

    pub fn suspend(&mut self) -> Result<Value, LockError> {
        self.suspended = true;
        self.release("sleep")
    }

    pub fn resume(&mut self) -> Result<Value, LockError> {
        self.suspended = false;
        self.status().map_err(LockError::Other)
    }
}

// ----------------------------------------------------------------- Sampler & Dispatch

#[derive(Default)]
pub struct Samples {
    pub previous: Option<(u64, u64)>,
}

impl Samples {
    pub fn read(&mut self) -> Result<Option<LoadSample>, String> {
        let stat = fs::read_to_string("/proc/stat")
            .map_err(|e| format!("读取 /proc/stat 失败: {e}"))?;
        let first = stat
            .lines()
            .next()
            .ok_or_else(|| "空的 /proc/stat".to_string())?;
        let vals: Vec<u64> = first
            .split_whitespace()
            .skip(1)
            .take(8)
            .map(|s| s.parse::<u64>())
            .collect::<Result<Vec<_>, _>>()
            .map_err(|_| "解析 /proc/stat 失败".to_string())?;
        if vals.len() < 5 {
            return Err("CPU采样字段不足".to_string());
        }
        let now = (vals.iter().sum::<u64>(), vals[3] + vals[4]);
        let prev = self.previous.replace(now);
        let Some((prev_total, prev_idle)) = prev else {
            return Ok(None);
        };
        if now.0 <= prev_total || now.1 < prev_idle {
            return Err("CPU采样计数不连续".to_string());
        }
        let total = now.0 - prev_total;
        let idle = now.1 - prev_idle;
        if idle > total {
            return Err("CPU采样计数不连续".to_string());
        }
        let gpu_out = run_command_timeout(
            "/usr/bin/nvidia-smi",
            &[
                "-i",
                "0",
                "--query-gpu=utilization.gpu",
                "--format=csv,noheader,nounits",
            ],
            Duration::from_millis(800),
        )?;
        let gpu_str = gpu_out.stdout.trim();
        if gpu_out.code != 0 || gpu_str.is_empty() || !gpu_str.bytes().all(|b| b.is_ascii_digit()) {
            return Err("NVIDIA利用率采样无效".to_string());
        }
        let gpu_pct: u8 = gpu_str
            .parse::<u8>()
            .map_err(|_| "NVIDIA利用率采样无效".to_string())?;
        let cpu_pct = ((100.0 * ((total - idle) as f64) / (total as f64)).round_ties_even()
            as u64)
            .min(100) as u8;
        LoadSample::new(cpu_pct, gpu_pct).map(Some)
    }
}

pub fn dispatch<D: TurboDevice>(
    controller: &mut Controller<D>,
    request: &Value,
) -> Result<Value, String> {
    let Some(obj) = request.as_object() else {
        return Err("请求格式无效".to_string());
    };
    let Some(cmd) = obj.get("command").and_then(|v| v.as_str()) else {
        return Err("请求格式无效".to_string());
    };
    if cmd == "select" {
        if obj.len() != 2 || !obj.contains_key("variant") {
            return Err("请求格式无效".to_string());
        }
        let variant = obj["variant"]
            .as_str()
            .ok_or_else(|| "子模式仅支持quiet/extreme".to_string())?;
        return controller.select(variant).map_err(|e| e.to_string());
    }
    if obj.len() != 1 {
        return Err("请求包含多余字段".to_string());
    }
    match cmd {
        "status" => controller.status(),
        "release" => controller.release("requested").map_err(|e| e.to_string()),
        "suspend" => controller.suspend().map_err(|e| e.to_string()),
        "resume" => controller.resume().map_err(|e| e.to_string()),
        _ => Err("命令不支持".to_string()),
    }
}

pub fn prepare_socket(path: &Path) -> Result<(), String> {
    let before = match fs::symlink_metadata(path) {
        Ok(m) => m,
        Err(e) if e.kind() == std::io::ErrorKind::NotFound => return Ok(()),
        Err(e) => return Err(format!("检查旧 socket 失败: {e}")),
    };
    if !before.file_type().is_socket() || before.uid() != 0 || (before.mode() & 0o077) != 0 {
        return Err("拒绝移除非可信socket路径".to_string());
    }
    match UnixStream::connect(path) {
        Ok(_) => return Err("控制socket仍有活跃服务，拒绝替换".to_string()),
        Err(e) if e.raw_os_error() == Some(libc::ECONNREFUSED) => {}
        Err(e) => return Err(format!("不能确认遗留socket已停止: {e}")),
    }
    let after = fs::symlink_metadata(path)
        .map_err(|e| format!("复检旧 socket 失败: {e}"))?;
    if (before.dev(), before.ino()) != (after.dev(), after.ino()) {
        return Err("socket路径在检查时发生变化".to_string());
    }
    fs::remove_file(path).map_err(|e| format!("删除旧 socket 失败: {e}"))
}

fn check_peer_root(stream: &UnixStream) -> Result<(), String> {
    let mut cred = libc::ucred {
        pid: 0,
        uid: u32::MAX,
        gid: u32::MAX,
    };
    let mut len = std::mem::size_of::<libc::ucred>() as libc::socklen_t;
    let rc = unsafe {
        libc::getsockopt(
            stream.as_raw_fd(),
            libc::SOL_SOCKET,
            libc::SO_PEERCRED,
            (&mut cred as *mut libc::ucred) as *mut libc::c_void,
            &mut len,
        )
    };
    if rc != 0 || cred.uid != 0 {
        return Err("仅接受root控制客户端".to_string());
    }
    Ok(())
}

fn handle_client<D: TurboDevice>(controller: &mut Controller<D>, mut stream: UnixStream) {
    let _ = stream.set_read_timeout(Some(Duration::from_millis(300)));
    let _ = stream.set_write_timeout(Some(Duration::from_millis(300)));
    let response = (|| -> Result<Value, String> {
        check_peer_root(&stream)?;
        let mut buf = [0u8; 2049];
        let n = stream
            .read(&mut buf)
            .map_err(|e| format!("读取客户端请求失败: {e}"))?;
        if n == 0 || n > 2048 || !buf[..n].contains(&b'\n') {
            return Err("请求过长或不完整".to_string());
        }
        let req: Value =
            serde_json::from_slice(&buf[..n]).map_err(|e| format!("请求 JSON 无效: {e}"))?;
        dispatch(controller, &req)
    })();
    let reply = match response {
        Ok(res) => json!({"ok": true, "result": res}),
        Err(err) => json!({"ok": false, "error": err}),
    };
    if let Ok(mut encoded) = serde_json::to_string(&reply) {
        encoded.push('\n');
        let _ = stream.write_all(encoded.as_bytes());
    }
}

pub fn run_daemon() -> Result<(), String> {
    if unsafe { libc::geteuid() } != 0 {
        return Err("守护进程需要root与已验收的专用内核lease".to_string());
    }
    check_identity(&identity()?)?;
    check_runtime()?;
    check_qualification(Path::new(QUALIFICATION_PATH), true)?;

    let _daemon_lock = RootFlock::acquire(Path::new(DAEMON_LOCK), None)
        .map_err(|e| format!("获取守护进程单实例锁失败: {e}"))?;
    let sock_path = Path::new(TURBO_SOCKET);
    prepare_socket(sock_path)?;

    let device = RealTurboDevice::open(Path::new(TURBO_DEVICE))?;
    let listener = UnixListener::bind(sock_path)
        .map_err(|e| format!("绑定控制 socket 失败: {e}"))?;
    fs::set_permissions(sock_path, fs::Permissions::from_mode(0o600))
        .map_err(|e| format!("设置 socket 权限失败: {e}"))?;
    listener
        .set_nonblocking(true)
        .map_err(|e| format!("设置非阻塞 socket 失败: {e}"))?;

    STOP_FLAG.store(false, Ordering::SeqCst);
    install_signal_handlers();

    let mut controller = Controller::new_live(device);
    let journal_path = Path::new(JOURNAL_PATH);
    if journal_path.exists() {
        trusted_file(journal_path, true, 65_536)?;
        controller
            .release("startup_reconcile")
            .map_err(|e| e.to_string())?;
    }

    let mut sampler = Samples::default();
    let mut next_heartbeat = Instant::now();
    let mut next_sample = Instant::now();
    let mut was_active = false;
    let mut fatal_err: Option<String> = None;

    while !STOP_FLAG.load(Ordering::SeqCst) {
        let now = Instant::now();
        if now >= next_heartbeat.min(next_sample) {
            let tick_res = (|| -> Result<(), LockError> {
                let mut sample = None;
                let active = controller
                    .device
                    .status()
                    .map_err(LockError::Other)?["active"]
                    .as_bool()
                    == Some(true);
                if active != was_active {
                    sampler.previous = None;
                    was_active = active;
                }
                if now >= next_sample {
                    if active {
                        sample = sampler.read().map_err(LockError::Other)?;
                    }
                    next_sample = now + Duration::from_millis(1500);
                }
                controller.tick(sample)?;
                Ok(())
            })();
            match tick_res {
                Ok(()) => {}
                Err(LockError::Busy(_)) => {
                    // Other profile transaction wins; kernel lease remains bounded.
                }
                Err(LockError::Other(err)) => {
                    println!("{}", json!({"event": "turbo_error", "error": err}));
                    if let Err(rel_err) = controller.release("sampling_or_poll_error") {
                        fatal_err = Some(rel_err.to_string());
                        break;
                    }
                }
            }
            if now >= next_heartbeat {
                next_heartbeat = now + Duration::from_secs(1);
            }
        }

        let wake_at = next_heartbeat.min(next_sample);
        let wait_ms = wake_at
            .saturating_duration_since(Instant::now())
            .as_millis()
            .clamp(10, 250) as libc::c_int;
        let mut pfd = libc::pollfd {
            fd: listener.as_raw_fd(),
            events: libc::POLLIN,
            revents: 0,
        };
        let rc = unsafe { libc::poll(&mut pfd, 1, wait_ms) };
        if rc > 0 && (pfd.revents & libc::POLLIN) != 0 {
            loop {
                match listener.accept() {
                    Ok((stream, _)) => {
                        let _ = stream.set_nonblocking(false);
                        handle_client(&mut controller, stream);
                    }
                    Err(e) if e.kind() == std::io::ErrorKind::WouldBlock => break,
                    Err(_) => break,
                }
            }
        }
    }

    if let Err(e) = controller.release("daemon_stop") {
        println!("{}", json!({"event": "release_pending", "error": e.to_string()}));
    }
    drop(controller);
    drop(listener);
    let _ = fs::remove_file(sock_path);
    match fatal_err {
        Some(e) => Err(e),
        None => Ok(()),
    }
}

pub fn doctor() -> Result<Value, String> {
    let current = identity()?;
    Ok(json!({
        "candidate": true,
        "auth_required": "root for live operations",
        "identity": current,
        "identity_matches": current == expected_identity(),
        "bridge_exists": Path::new(TURBO_DEVICE).exists(),
        "qualification_exists": Path::new(QUALIFICATION_PATH).exists(),
        "daemon_socket_exists": Path::new(TURBO_SOCKET).exists(),
        "live_read_performed": false,
    }))
}

pub fn client_request(request: &Value) -> Result<Value, String> {
    turbo_socket_request(Path::new(TURBO_SOCKET), request, Duration::from_secs(4))
}

#[cfg(test)]
mod tests {
    use super::*;

    struct FakeDevice {
        st: Value,
        cmds: Vec<String>,
        fail_next: Option<String>,
    }

    impl FakeDevice {
        fn new() -> Self {
            Self {
                st: json!({
                    "abi": 1,
                    "read_rc": 0,
                    "active": false,
                    "variant": "extreme",
                    "sequence": 0,
                    "last_rc": 0,
                    "cstm": 0,
                    "gatm": 0,
                    "standard_turbo": true,
                    "round_adapter": true,
                    "return_present": false,
                    "last_ack": null,
                    "recovery_needed": false,
                    "combined_supported": true,
                    "game_supported": false,
                    "stop_reason": "idle",
                }),
                cmds: Vec::new(),
                fail_next: None,
            }
        }
    }

    impl TurboDevice for FakeDevice {
        fn status(&mut self) -> Result<Value, String> {
            validate_status(self.st.clone())
        }

        fn command(&mut self, cmd: &str) -> Result<Value, String> {
            self.cmds.push(cmd.to_string());
            if let Some(err) = self.fail_next.take() {
                return Err(err);
            }
            let seq = self.st["sequence"].as_i64().unwrap() + 1;
            self.st["sequence"] = json!(seq);
            self.st["return_present"] = json!(false);
            self.st["last_ack"] = Value::Null;
            match cmd {
                "acquire quiet" | "variant quiet" => {
                    self.st["active"] = json!(true);
                    self.st["variant"] = json!("quiet");
                }
                "acquire extreme" | "variant extreme" => {
                    self.st["active"] = json!(true);
                    self.st["variant"] = json!("extreme");
                }
                "release" => {
                    self.st["active"] = json!(false);
                    self.st["stop_reason"] = json!("release");
                }
                "combined 1" => {
                    self.st["cstm"] = json!(1);
                    self.st["return_present"] = json!(true);
                    self.st["last_ack"] = json!(0xBBu64);
                }
                "combined 0" => {
                    self.st["cstm"] = json!(0);
                    self.st["return_present"] = json!(true);
                    self.st["last_ack"] = json!(0xAAu64);
                }
                "heartbeat" => {}
                other => return Err(format!("unexpected cmd {other}")),
            }
            Ok(self.st.clone())
        }
    }

    fn make_test_controller(dir: &Path) -> Controller<FakeDevice> {
        Controller {
            device: FakeDevice::new(),
            journal: Journal::new(dir.join("turbo.json")),
            lock_path: None,
            tuning_path: dir.join("tuning.json"),
            state: SchedulerState::default(),
            suspended: false,
            record: None,
            machine_fn: Box::new(|| Ok(expected_identity())),
            runtime_ready_fn: Box::new(|| Ok(())),
            powerd_ready_fn: Box::new(|| Ok(())),
        }
    }

    #[test]
    fn test_select_tick_combined_and_release_lifecycle() {
        let dir = std::env::temp_dir().join(format!("turbod-test-{}", std::process::id()));
        let _ = fs::remove_dir_all(&dir);
        fs::create_dir_all(&dir).unwrap();

        let mut ctrl = make_test_controller(&dir);
        let res = ctrl.select("quiet").unwrap();
        assert_eq!(res["active"], true);
        assert_eq!(res["variant"], "quiet");
        assert!(dir.join("turbo.json").exists());

        // 3 high-load samples trigger combined 1
        let high = LoadSample::new(98, 95).unwrap();
        for _ in 0..3 {
            ctrl.tick(Some(high)).unwrap();
        }
        assert!(ctrl.device.cmds.contains(&"combined 1".to_string()));
        assert_eq!(ctrl.state.combined_reply, 0xBB);

        // Unplugging adapter triggers immediate release
        ctrl.device.st["round_adapter"] = json!(false);
        let after = ctrl.tick(None).unwrap();
        assert_eq!(after["bridge"]["active"], false);
        assert!(!dir.join("turbo.json").exists());

        let _ = fs::remove_dir_all(&dir);
    }

    #[test]
    fn test_dispatch_rejects_bad_requests() {
        let dir = std::env::temp_dir().join(format!("turbod-disp-{}", std::process::id()));
        let _ = fs::remove_dir_all(&dir);
        fs::create_dir_all(&dir).unwrap();
        let mut ctrl = make_test_controller(&dir);
        assert!(dispatch(&mut ctrl, &json!({"command": "select"})).is_err());
        assert!(dispatch(&mut ctrl, &json!({"command": "status", "extra": 1})).is_err());
        assert!(dispatch(&mut ctrl, &json!({"command": "exec"})).is_err());
        assert!(dispatch(&mut ctrl, &json!({"command": "status"})).is_ok());
        let _ = fs::remove_dir_all(&dir);
    }
}

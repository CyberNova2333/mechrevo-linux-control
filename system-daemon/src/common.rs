// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Shared root-privilege safety primitives: DMI/kernel identity checks,
//! trusted file reads, flock-based profile locks, atomic JSON persistence,
//! bounded external command execution, and Unix socket client helpers.

use serde_json::{json, Value};
use std::ffi::CString;
use std::fs::{self, File, OpenOptions};
use std::io::{BufRead, BufReader, Read, Write};
use std::os::unix::ffi::OsStrExt;
use std::os::unix::fs::{FileTypeExt, MetadataExt, OpenOptionsExt, PermissionsExt};
use std::os::unix::io::{AsRawFd, FromRawFd};
use std::os::unix::net::UnixStream;
use std::path::{Path, PathBuf};
use std::process::{Command, Stdio};
use std::thread;
use std::time::{Duration, Instant, SystemTime, UNIX_EPOCH};

pub const KERNEL: &str = "6.8.0-149-generic";
pub const SYS_VENDOR: &str = "MECHREVO";
pub const BOARD_NAME: &str = "YAOSHI Series-XxAF55xx";
pub const BIOS_VERSION: &str = "N.1.33MRO91";

pub const PROFILE_LOCK: &str = "/run/mechrevo-profile.lock";
pub const TURBO_SOCKET: &str = "/run/mechrevo-turbo.sock";
pub const TURBO_DEVICE: &str = "/dev/mechrevo-turbo";

pub fn kernel_release() -> Result<String, String> {
    let mut uts: libc::utsname = unsafe { std::mem::zeroed() };
    if unsafe { libc::uname(&mut uts) } != 0 {
        return Err(format!("uname failed: {}", std::io::Error::last_os_error()));
    }
    let bytes: Vec<u8> = uts
        .release
        .iter()
        .take_while(|&&c| c != 0)
        .map(|&c| c as u8)
        .collect();
    String::from_utf8(bytes).map_err(|e| format!("invalid kernel release UTF-8: {e}"))
}

pub fn read_dmi_field(name: &str) -> Result<String, String> {
    let p = Path::new("/sys/class/dmi/id").join(name);
    fs::read_to_string(&p)
        .map(|s| s.trim().to_string())
        .map_err(|e| format!("读取 DMI {name} 失败: {e}"))
}

pub fn identity() -> Result<Value, String> {
    Ok(json!({
        "kernel_release": kernel_release()?,
        "dmi": {
            "sys_vendor": read_dmi_field("sys_vendor")?,
            "board_name": read_dmi_field("board_name")?,
            "bios_version": read_dmi_field("bios_version")?,
        }
    }))
}

pub fn expected_identity() -> Value {
    json!({
        "kernel_release": KERNEL,
        "dmi": {
            "sys_vendor": SYS_VENDOR,
            "board_name": BOARD_NAME,
            "bios_version": BIOS_VERSION,
        }
    })
}

pub fn check_identity(value: &Value) -> Result<(), String> {
    if value != &expected_identity() {
        return Err("机型、BIOS或内核已变化；此候选仅允许已验证的6.8.0-149分支".to_string());
    }
    Ok(())
}

pub fn check_machine() -> Result<(), String> {
    for (name, expected) in [
        ("sys_vendor", SYS_VENDOR),
        ("board_name", BOARD_NAME),
        ("bios_version", BIOS_VERSION),
    ] {
        let actual = read_dmi_field(name)?;
        if actual != expected {
            return Err("机型或 BIOS 已变化，需要重新验证性能适配".to_string());
        }
    }
    let iomem = fs::read_to_string("/proc/iomem")
        .map_err(|e| format!("读取 /proc/iomem 失败: {e}"))?;
    if !iomem.contains("fe410000-fe410fff : INOU0000:00") {
        return Err("固件 EC 内存资源与已验证地址不符".to_string());
    }
    Ok(())
}

pub fn monotonic_ms() -> u64 {
    let mut ts = libc::timespec {
        tv_sec: 0,
        tv_nsec: 0,
    };
    unsafe {
        libc::clock_gettime(libc::CLOCK_MONOTONIC, &mut ts);
    }
    (ts.tv_sec as u64)
        .saturating_mul(1000)
        .saturating_add((ts.tv_nsec as u64) / 1_000_000)
}

pub fn unix_time_secs() -> f64 {
    SystemTime::now()
        .duration_since(UNIX_EPOCH)
        .map(|d| d.as_secs_f64())
        .unwrap_or(0.0)
}

/// Read a root-owned regular file without following symlinks and enforcing strict permissions.
pub fn trusted_file(path: &Path, private: bool, limit: usize) -> Result<Vec<u8>, String> {
    let c_path = CString::new(path.as_os_str().as_bytes())
        .map_err(|_| format!("非法路径: {}", path.display()))?;
    let fd = unsafe {
        libc::open(
            c_path.as_ptr(),
            libc::O_RDONLY | libc::O_NOFOLLOW | libc::O_CLOEXEC,
        )
    };
    if fd < 0 {
        let err = std::io::Error::last_os_error();
        return Err(format!("打开文件失败 {}: {err}", path.display()));
    }
    let mut file = unsafe { File::from_raw_fd(fd) };
    let meta = file
        .metadata()
        .map_err(|e| format!("stat 失败 {}: {e}", path.display()))?;
    let forbidden: u32 = if private { 0o077 } else { 0o022 };
    if !meta.is_file() || meta.uid() != 0 || (meta.mode() & forbidden) != 0 {
        return Err(format!(
            "文件必须由root持有且不可由其他用户修改: {}",
            path.display()
        ));
    }
    let mut buf = Vec::new();
    let mut limited = (&mut file).take((limit + 1) as u64);
    limited
        .read_to_end(&mut buf)
        .map_err(|e| format!("读取失败 {}: {e}", path.display()))?;
    if buf.len() > limit {
        return Err("文件超过允许长度".to_string());
    }
    Ok(buf)
}

#[derive(Debug)]
pub enum LockError {
    Busy(String),
    Other(String),
}

impl std::fmt::Display for LockError {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            Self::Busy(s) | Self::Other(s) => f.write_str(s),
        }
    }
}

pub struct RootFlock {
    _file: File,
}

impl RootFlock {
    pub fn acquire(path: &Path, wait: Option<Duration>) -> Result<Self, LockError> {
        let file = OpenOptions::new()
            .create(true)
            .read(true)
            .write(true)
            .mode(0o600)
            .custom_flags(libc::O_CLOEXEC | libc::O_NOFOLLOW)
            .open(path)
            .map_err(|e| LockError::Other(format!("打开锁文件失败 {}: {e}", path.display())))?;
        let meta = file
            .metadata()
            .map_err(|e| LockError::Other(format!("stat 锁文件失败: {e}")))?;
        if !meta.is_file() || meta.uid() != 0 || (meta.mode() & 0o022) != 0 {
            return Err(LockError::Other("性能控制锁文件权限异常".to_string()));
        }
        let deadline = wait.map(|d| Instant::now() + d);
        loop {
            let rc = unsafe { libc::flock(file.as_raw_fd(), libc::LOCK_EX | libc::LOCK_NB) };
            if rc == 0 {
                return Ok(Self { _file: file });
            }
            let err = std::io::Error::last_os_error();
            if err.raw_os_error() != Some(libc::EWOULDBLOCK)
                && err.raw_os_error() != Some(libc::EAGAIN)
            {
                return Err(LockError::Other(format!("flock 失败: {err}")));
            }
            match deadline {
                None => {
                    return Err(LockError::Busy("性能控制锁忙".to_string()));
                }
                Some(dl) => {
                    let now = Instant::now();
                    if now >= dl {
                        return Err(LockError::Busy(
                            "性能控制忙：等待1.5秒后仍未取得锁，请稍后重试".to_string(),
                        ));
                    }
                    thread::sleep(Duration::from_millis(20).min(dl - now));
                }
            }
        }
    }
}

/// Atomically write JSON via temp file + fsync + chmod + rename (+ optional dir fsync).
pub fn atomic_write_json(
    target: &Path,
    payload: &Value,
    mode: u32,
    prefix: &str,
    fsync_dir: bool,
) -> Result<(), String> {
    let parent = target
        .parent()
        .ok_or_else(|| "invalid target path without parent".to_string())?;
    fs::create_dir_all(parent).map_err(|e| format!("create_dir_all failed: {e}"))?;
    let pid = std::process::id();
    let nanos = monotonic_ms();
    let mut temp_path = PathBuf::new();
    let mut file = None;
    for attempt in 0..32u32 {
        let candidate = parent.join(format!("{prefix}{pid}-{nanos}-{attempt}.tmp"));
        match OpenOptions::new()
            .create_new(true)
            .write(true)
            .mode(0o600)
            .custom_flags(libc::O_CLOEXEC | libc::O_NOFOLLOW)
            .open(&candidate)
        {
            Ok(f) => {
                temp_path = candidate;
                file = Some(f);
                break;
            }
            Err(e) if e.kind() == std::io::ErrorKind::AlreadyExists => continue,
            Err(e) => return Err(format!("创建临时文件失败: {e}")),
        }
    }
    let mut file = file.ok_or_else(|| "无法创建唯一临时文件".to_string())?;

    let write_res = (|| -> Result<(), String> {
        let raw = serde_json::to_vec(payload).map_err(|e| format!("JSON 序列化失败: {e}"))?;
        file.write_all(&raw)
            .map_err(|e| format!("写入临时文件失败: {e}"))?;
        file.flush()
            .map_err(|e| format!("flush 临时文件失败: {e}"))?;
        file.sync_all()
            .map_err(|e| format!("fsync 临时文件失败: {e}"))?;
        if mode != 0o600 {
            fs::set_permissions(&temp_path, fs::Permissions::from_mode(mode))
                .map_err(|e| format!("chmod 临时文件失败: {e}"))?;
        }
        fs::rename(&temp_path, target).map_err(|e| format!("原子替换失败: {e}"))?;
        if fsync_dir {
            let c_dir = CString::new(parent.as_os_str().as_bytes())
                .map_err(|_| "invalid parent dir".to_string())?;
            let dfd = unsafe {
                libc::open(
                    c_dir.as_ptr(),
                    libc::O_RDONLY | libc::O_DIRECTORY | libc::O_CLOEXEC,
                )
            };
            if dfd >= 0 {
                unsafe {
                    let _ = libc::fsync(dfd);
                    libc::close(dfd);
                }
            }
        }
        Ok(())
    })();

    if write_res.is_err() {
        let _ = fs::remove_file(&temp_path);
    }
    write_res
}

pub struct CmdOutput {
    pub code: i32,
    pub stdout: String,
    pub stderr: String,
}

/// Run an external binary with a hard wall-clock timeout and kill on expiry.
pub fn run_command_timeout(
    program: &str,
    args: &[&str],
    timeout: Duration,
) -> Result<CmdOutput, String> {
    let mut child = Command::new(program)
        .args(args)
        .stdin(Stdio::null())
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .spawn()
        .map_err(|e| format!("执行 {program} 失败: {e}"))?;

    let deadline = Instant::now() + timeout;
    let status = loop {
        match child.try_wait() {
            Ok(Some(st)) => break st,
            Ok(None) => {
                if Instant::now() >= deadline {
                    let _ = child.kill();
                    let _ = child.wait();
                    return Err(format!("命令超时 ({}ms): {program}", timeout.as_millis()));
                }
                thread::sleep(Duration::from_millis(10));
            }
            Err(e) => {
                let _ = child.kill();
                let _ = child.wait();
                return Err(format!("等待 {program} 失败: {e}"));
            }
        }
    };

    let mut stdout = String::new();
    let mut stderr = String::new();
    if let Some(mut out) = child.stdout.take() {
        let _ = out.read_to_string(&mut stdout);
    }
    if let Some(mut err) = child.stderr.take() {
        let _ = err.read_to_string(&mut stderr);
    }
    Ok(CmdOutput {
        code: status.code().unwrap_or(-1),
        stdout,
        stderr,
    })
}

pub fn check_acpi_healthy() -> Result<Value, String> {
    let out = run_command_timeout(
        "/usr/local/libexec/mechrevo-acpi-check",
        &["--summary"],
        Duration::from_secs(2),
    )?;
    let data: Value = serde_json::from_str(out.stdout.trim())
        .map_err(|_| "ACPI完整性检查未通过，禁止启用狂暴调度".to_string())?;
    if out.code != 0 || data.get("healthy").and_then(|v| v.as_bool()) != Some(true) {
        return Err("ACPI完整性检查未通过，禁止启用狂暴调度".to_string());
    }
    Ok(data)
}

pub fn check_powerd_active() -> Result<(), String> {
    let out = run_command_timeout(
        "/usr/bin/systemctl",
        &["is-active", "--quiet", "nvidia-powerd.service"],
        Duration::from_secs(1),
    )?;
    if out.code != 0 {
        return Err("nvidia-powerd未运行，禁止启用狂暴调度".to_string());
    }
    Ok(())
}

pub fn check_runtime() -> Result<Value, String> {
    let data = check_acpi_healthy()?;
    check_powerd_active()?;
    Ok(data)
}

/// Send one JSON-line request to `/run/mechrevo-turbo.sock` with strict root socket checks.
pub fn turbo_socket_request(sock_path: &Path, request: &Value, timeout: Duration) -> Result<Value, String> {
    let meta = fs::symlink_metadata(sock_path)
        .map_err(|e| format!("检查守护进程控制socket失败: {e}"))?;
    if !meta.file_type().is_socket() || meta.uid() != 0 || (meta.mode() & 0o077) != 0 {
        return Err("守护进程控制socket权限异常".to_string());
    }
    let mut stream = UnixStream::connect(sock_path)
        .map_err(|e| format!("连接狂暴调度守护进程失败: {e}"))?;
    let _ = stream.set_read_timeout(Some(timeout));
    let _ = stream.set_write_timeout(Some(timeout));
    let mut line = serde_json::to_string(request).map_err(|e| e.to_string())?;
    line.push('\n');
    stream
        .write_all(line.as_bytes())
        .map_err(|e| format!("发送请求失败: {e}"))?;
    let mut reader = BufReader::new(stream);
    let mut buf = Vec::new();
    let n = reader
        .by_ref()
        .take(32_769)
        .read_until(b'\n', &mut buf)
        .map_err(|e| format!("读取响应失败: {e}"))?;
    if n == 0 || buf.len() > 32_768 || !buf.ends_with(b"\n") {
        return Err("守护进程响应不完整或过长".to_string());
    }
    serde_json::from_slice(&buf).map_err(|e| format!("解析响应 JSON 失败: {e}"))
}

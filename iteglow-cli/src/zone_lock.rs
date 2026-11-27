// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Cooperating per-zone file locks (`$XDG_RUNTIME_DIR/iteglow-<zone>.lock`)
//! and atomic user/system JSON state I/O compatible with `iteglow-sessiond` and `mechrevo-control-gui`.

use serde_json::Value;
use std::collections::HashMap;
use std::ffi::CString;
use std::fs::{self, File, OpenOptions};
use std::io::{Read, Write};
use std::os::unix::ffi::OsStrExt;
use std::os::unix::fs::{MetadataExt, OpenOptionsExt, PermissionsExt};
use std::os::unix::io::{FromRawFd, RawFd};
use std::path::{Path, PathBuf};
use std::sync::{Arc, Mutex, OnceLock};
use std::thread;
use std::time::{Duration, Instant};

pub const VALID_ZONES: &[&str] = &["keyboard", "lightbar", "logo", "manual-state"];
pub const MAX_STATE_BYTES: usize = 65_536;
pub const MAX_PROFILE_BYTES: usize = 262_144;
pub const SYSTEM_STATE_PATH: &str = "/etc/iteglow/state.json";

struct LockEntry {
    depth: usize,
    fd: Option<RawFd>,
}

static ENTRIES: OnceLock<Mutex<HashMap<(PathBuf, String), Arc<Mutex<LockEntry>>>>> =
    OnceLock::new();

fn entry_for(rdir: &Path, zone: &str) -> Arc<Mutex<LockEntry>> {
    let map = ENTRIES.get_or_init(|| Mutex::new(HashMap::new()));
    let mut guard = map.lock().unwrap_or_else(|e| e.into_inner());
    guard
        .entry((rdir.to_path_buf(), zone.to_string()))
        .or_insert_with(|| Arc::new(Mutex::new(LockEntry { depth: 0, fd: None })))
        .clone()
}

pub fn runtime_dir() -> PathBuf {
    if let Ok(d) = std::env::var("XDG_RUNTIME_DIR") {
        if !d.trim().is_empty() {
            return PathBuf::from(d);
        }
    }
    let uid = unsafe { libc::geteuid() };
    PathBuf::from(format!("/run/user/{uid}"))
}

pub fn state_dir() -> PathBuf {
    if let Ok(d) = std::env::var("XDG_STATE_HOME") {
        if !d.trim().is_empty() {
            return PathBuf::from(d).join("iteglow");
        }
    }
    let home = std::env::var("HOME").unwrap_or_else(|_| "/".to_string());
    PathBuf::from(home).join(".local").join("state").join("iteglow")
}

pub fn config_dir() -> PathBuf {
    if let Ok(d) = std::env::var("XDG_CONFIG_HOME") {
        if !d.trim().is_empty() {
            return PathBuf::from(d).join("iteglow");
        }
    }
    let home = std::env::var("HOME").unwrap_or_else(|_| "/".to_string());
    PathBuf::from(home).join(".config").join("iteglow")
}

pub fn user_home_for_uid(uid: u32) -> Option<PathBuf> {
    let pw = unsafe { libc::getpwuid(uid) };
    if pw.is_null() {
        return None;
    }
    let dir_ptr = unsafe { (*pw).pw_dir };
    if dir_ptr.is_null() {
        return None;
    }
    let cstr = unsafe { std::ffi::CStr::from_ptr(dir_ptr) };
    cstr.to_str().ok().map(PathBuf::from)
}

pub fn user_home_for_name(name: &str) -> Option<PathBuf> {
    let cname = CString::new(name).ok()?;
    let pw = unsafe { libc::getpwnam(cname.as_ptr()) };
    if pw.is_null() {
        return None;
    }
    let dir_ptr = unsafe { (*pw).pw_dir };
    if dir_ptr.is_null() {
        return None;
    }
    let cstr = unsafe { std::ffi::CStr::from_ptr(dir_ptr) };
    cstr.to_str().ok().map(PathBuf::from)
}

fn open_lock_fd(rdir: &Path, zone: &str) -> Result<RawFd, String> {
    let my_uid = unsafe { libc::geteuid() };
    let c_dir = CString::new(rdir.as_os_str().as_bytes())
        .map_err(|_| "运行时目录路径无效".to_string())?;
    let dir_fd = unsafe {
        libc::open(
            c_dir.as_ptr(),
            libc::O_RDONLY | libc::O_DIRECTORY | libc::O_CLOEXEC | libc::O_NOFOLLOW,
        )
    };
    if dir_fd < 0 {
        return Err(format!(
            "无法打开运行时目录 {}: {}",
            rdir.display(),
            std::io::Error::last_os_error()
        ));
    }
    let res = (|| -> Result<RawFd, String> {
        let mut st: libc::stat = unsafe { std::mem::zeroed() };
        if unsafe { libc::fstat(dir_fd, &mut st) } != 0 {
            return Err("无法检查运行时目录权限".to_string());
        }
        if st.st_uid != my_uid || (st.st_mode & 0o022) != 0 {
            return Err("不安全的灯光运行时目录权限".to_string());
        }
        let name = CString::new(format!("iteglow-{zone}.lock"))
            .map_err(|_| "锁文件名无效".to_string())?;
        let fd = unsafe {
            libc::openat(
                dir_fd,
                name.as_ptr(),
                libc::O_CREAT | libc::O_RDWR | libc::O_CLOEXEC | libc::O_NOFOLLOW,
                0o600 as libc::c_uint,
            )
        };
        if fd < 0 {
            return Err(format!("打开分区锁失败: {}", std::io::Error::last_os_error()));
        }
        let mut lst: libc::stat = unsafe { std::mem::zeroed() };
        if unsafe { libc::fstat(fd, &mut lst) } != 0 {
            unsafe { libc::close(fd) };
            return Err("无法检查分区锁权限".to_string());
        }
        if (lst.st_mode & libc::S_IFMT) != libc::S_IFREG
            || lst.st_uid != my_uid
            || (lst.st_mode & 0o077) != 0
        {
            unsafe { libc::close(fd) };
            return Err("不安全的灯光分区锁权限".to_string());
        }
        Ok(fd)
    })();
    unsafe { libc::close(dir_fd) };
    res
}

pub struct ZoneGuard {
    entry: Arc<Mutex<LockEntry>>,
    acquired: bool,
}

impl ZoneGuard {
    pub fn acquire(zone: &str) -> Result<Self, String> {
        if !VALID_ZONES.contains(&zone) {
            return Err(format!("未知灯光分区锁: {zone}"));
        }
        let rdir = runtime_dir();
        let entry = entry_for(&rdir, zone);
        let deadline = Instant::now() + Duration::from_secs(5);
        let mut state = entry.lock().unwrap_or_else(|e| e.into_inner());
        if state.depth == 0 {
            let fd = open_lock_fd(&rdir, zone)?;
            loop {
                let rc = unsafe { libc::flock(fd, libc::LOCK_EX | libc::LOCK_NB) };
                if rc == 0 {
                    state.fd = Some(fd);
                    break;
                }
                let err = std::io::Error::last_os_error();
                if err.raw_os_error() != Some(libc::EWOULDBLOCK)
                    && err.raw_os_error() != Some(libc::EAGAIN)
                {
                    unsafe { libc::close(fd) };
                    return Err(format!("获取分区锁失败: {err}"));
                }
                if Instant::now() >= deadline {
                    unsafe { libc::close(fd) };
                    return Err("灯光分区事务正忙，请稍后重试".to_string());
                }
                drop(state);
                thread::sleep(Duration::from_millis(20));
                state = entry.lock().unwrap_or_else(|e| e.into_inner());
                if state.depth > 0 {
                    unsafe { libc::close(fd) };
                    break;
                }
            }
        }
        state.depth += 1;
        drop(state);
        Ok(Self {
            entry,
            acquired: true,
        })
    }
}

impl Drop for ZoneGuard {
    fn drop(&mut self) {
        if !self.acquired {
            return;
        }
        self.acquired = false;
        let mut state = self.entry.lock().unwrap_or_else(|e| e.into_inner());
        if state.depth > 0 {
            state.depth -= 1;
            if state.depth == 0 {
                if let Some(fd) = state.fd.take() {
                    unsafe {
                        let _ = libc::flock(fd, libc::LOCK_UN);
                        libc::close(fd);
                    }
                }
            }
        }
    }
}

pub fn read_json_file(path: &Path, max_bytes: usize, require_owner: bool) -> Result<Option<Value>, String> {
    let uid = unsafe { libc::geteuid() };
    let c_path = CString::new(path.as_os_str().as_bytes())
        .map_err(|_| "路径无效".to_string())?;
    let fd = unsafe {
        libc::open(
            c_path.as_ptr(),
            libc::O_RDONLY | libc::O_CLOEXEC | libc::O_NOFOLLOW | libc::O_NONBLOCK,
        )
    };
    if fd < 0 {
        let err = std::io::Error::last_os_error();
        if err.raw_os_error() == Some(libc::ENOENT) {
            return Ok(None);
        }
        return Err(format!("读取文件失败: {err}"));
    }
    let mut file = unsafe { File::from_raw_fd(fd) };
    let meta = file.metadata().map_err(|e| format!("检查文件失败: {e}"))?;
    if !meta.is_file() || (meta.mode() & 0o002) != 0 {
        return Err("文件必须为普通文件且不可被他人写入".to_string());
    }
    if require_owner && meta.uid() != uid {
        return Err("配置文件必须由当前用户持有".to_string());
    }
    if meta.len() > max_bytes as u64 {
        return Err("配置文件超过大小限制".to_string());
    }
    let mut buf = Vec::new();
    (&mut file)
        .take((max_bytes + 1) as u64)
        .read_to_end(&mut buf)
        .map_err(|e| format!("读取配置失败: {e}"))?;
    if buf.len() > max_bytes {
        return Err("配置文件超过大小限制".to_string());
    }
    let val: Value = serde_json::from_slice(&buf).map_err(|e| format!("JSON 解析失败: {e}"))?;
    Ok(Some(val))
}

pub fn write_json_file(path: &Path, data: &Value, mode: u32) -> Result<(), String> {
    if let Some(parent) = path.parent() {
        fs::create_dir_all(parent).map_err(|e| format!("创建目录失败: {e}"))?;
    }
    let tmp = PathBuf::from(format!("{}.tmp.{}", path.display(), std::process::id()));
    let mut file = OpenOptions::new()
        .create(true)
        .truncate(true)
        .write(true)
        .mode(mode)
        .custom_flags(libc::O_CLOEXEC | libc::O_NOFOLLOW)
        .open(&tmp)
        .map_err(|e| format!("创建临时文件失败: {e}"))?;
    let mut encoded = serde_json::to_string_pretty(data)
        .map_err(|e| format!("序列化 JSON 失败: {e}"))?;
    encoded.push('\n');
    let res = file
        .write_all(encoded.as_bytes())
        .and_then(|()| file.flush())
        .and_then(|()| file.sync_all())
        .map_err(|e| format!("写入文件失败: {e}"));
    if let Err(e) = res {
        let _ = fs::remove_file(&tmp);
        return Err(e);
    }
    let _ = fs::set_permissions(&tmp, fs::Permissions::from_mode(mode));
    fs::rename(&tmp, path).map_err(|e| {
        let _ = fs::remove_file(&tmp);
        format!("原子保存失败: {e}")
    })
}

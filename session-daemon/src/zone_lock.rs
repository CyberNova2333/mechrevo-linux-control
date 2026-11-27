// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Cooperating per-zone file locks (`$XDG_RUNTIME_DIR/iteglow-<zone>.lock`),
//! wire-compatible with `iteglow.zone_lock.ZoneTransaction`, plus safe state file I/O.
//!
//! Lock order is `zone -> manual-state` (or the Logo helper's profile lock).
//! Lock files are never unlinked so concurrent clients never race on distinct inodes.

use serde_json::Value;
use std::collections::HashMap;
use std::ffi::CString;
use std::fs::{self, File, OpenOptions};
use std::io::{Read, Write};
use std::os::unix::ffi::OsStrExt;
use std::os::unix::fs::{MetadataExt, OpenOptionsExt, PermissionsExt};
use std::os::unix::io::{AsRawFd, FromRawFd, RawFd};
use std::path::{Path, PathBuf};
use std::sync::{Arc, Mutex, OnceLock};
use std::thread;
use std::time::{Duration, Instant};

pub const VALID_ZONES: &[&str] = &["keyboard", "lightbar", "logo", "manual-state"];
pub const MAX_STATE_BYTES: usize = 65_536;
pub const MAX_PROFILE_BYTES: usize = 262_144;

struct LockEntry {
    depth: usize,
    fd: Option<RawFd>,
}

static ENTRIES: OnceLock<Mutex<HashMap<(PathBuf, String), Arc<Mutex<LockEntry>>>>> =
    OnceLock::new();

fn entry_for(runtime_dir: &Path, zone: &str) -> Arc<Mutex<LockEntry>> {
    let map = ENTRIES.get_or_init(|| Mutex::new(HashMap::new()));
    let mut guard = map.lock().unwrap_or_else(|e| e.into_inner());
    let abs = if runtime_dir.is_absolute() {
        runtime_dir.to_path_buf()
    } else {
        std::env::current_dir()
            .unwrap_or_else(|_| PathBuf::from("/"))
            .join(runtime_dir)
    };
    guard
        .entry((abs, zone.to_string()))
        .or_insert_with(|| {
            Arc::new(Mutex::new(LockEntry {
                depth: 0,
                fd: None,
            }))
        })
        .clone()
}

pub fn default_runtime_dir() -> PathBuf {
    if let Ok(d) = std::env::var("XDG_RUNTIME_DIR") {
        if !d.trim().is_empty() {
            return PathBuf::from(d);
        }
    }
    let uid = unsafe { libc::getuid() };
    PathBuf::from(format!("/run/user/{uid}"))
}

pub fn default_state_dir() -> PathBuf {
    if let Ok(d) = std::env::var("XDG_STATE_HOME") {
        if !d.trim().is_empty() {
            return PathBuf::from(d).join("iteglow");
        }
    }
    let home = std::env::var("HOME").unwrap_or_else(|_| "/".to_string());
    PathBuf::from(home)
        .join(".local")
        .join("state")
        .join("iteglow")
}

pub fn default_config_dir() -> PathBuf {
    if let Ok(d) = std::env::var("XDG_CONFIG_HOME") {
        if !d.trim().is_empty() {
            return PathBuf::from(d).join("iteglow");
        }
    }
    let home = std::env::var("HOME").unwrap_or_else(|_| "/".to_string());
    PathBuf::from(home).join(".config").join("iteglow")
}

fn open_lock_fd(runtime_dir: &Path, zone: &str) -> Result<RawFd, String> {
    let my_uid = unsafe { libc::getuid() };
    let c_dir = CString::new(runtime_dir.as_os_str().as_bytes())
        .map_err(|_| "invalid runtime directory path".to_string())?;
    let dir_fd = unsafe {
        libc::open(
            c_dir.as_ptr(),
            libc::O_RDONLY | libc::O_DIRECTORY | libc::O_CLOEXEC | libc::O_NOFOLLOW,
        )
    };
    if dir_fd < 0 {
        let err = std::io::Error::last_os_error();
        return Err(format!(
            "cannot open runtime directory {}: {err}",
            runtime_dir.display()
        ));
    }

    let result = (|| -> Result<RawFd, String> {
        let mut st: libc::stat = unsafe { std::mem::zeroed() };
        if unsafe { libc::fstat(dir_fd, &mut st) } != 0 {
            return Err(format!(
                "fstat runtime directory failed: {}",
                std::io::Error::last_os_error()
            ));
        }
        if st.st_uid != my_uid || (st.st_mode & 0o022) != 0 {
            return Err("unsafe lighting runtime directory".to_string());
        }

        let name = CString::new(format!("iteglow-{zone}.lock"))
            .map_err(|_| "invalid lock filename".to_string())?;
        let fd = unsafe {
            libc::openat(
                dir_fd,
                name.as_ptr(),
                libc::O_CREAT | libc::O_RDWR | libc::O_CLOEXEC | libc::O_NOFOLLOW,
                0o600 as libc::c_uint,
            )
        };
        if fd < 0 {
            return Err(format!(
                "open lighting transaction lock failed: {}",
                std::io::Error::last_os_error()
            ));
        }

        let mut lock_st: libc::stat = unsafe { std::mem::zeroed() };
        if unsafe { libc::fstat(fd, &mut lock_st) } != 0 {
            unsafe { libc::close(fd) };
            return Err(format!(
                "fstat lighting transaction lock failed: {}",
                std::io::Error::last_os_error()
            ));
        }
        let is_reg = (lock_st.st_mode & libc::S_IFMT) == libc::S_IFREG;
        if !is_reg || lock_st.st_uid != my_uid || (lock_st.st_mode & 0o077) != 0 {
            unsafe { libc::close(fd) };
            return Err("unsafe lighting transaction lock".to_string());
        }
        Ok(fd)
    })();

    unsafe { libc::close(dir_fd) };
    result
}

/// RAII guard matching `iteglow.zone_lock.ZoneTransaction`.
pub struct ZoneGuard {
    entry: Arc<Mutex<LockEntry>>,
    acquired: bool,
}

impl ZoneGuard {
    pub fn acquire(zone: &str) -> Result<Self, String> {
        Self::acquire_in(&default_runtime_dir(), zone, Duration::from_secs(5))
    }

    pub fn acquire_in(runtime_dir: &Path, zone: &str, timeout: Duration) -> Result<Self, String> {
        if !VALID_ZONES.contains(&zone) {
            return Err("unknown lighting lock".to_string());
        }
        let entry = entry_for(runtime_dir, zone);
        let deadline = Instant::now() + timeout;
        let mut state = entry.lock().unwrap_or_else(|e| e.into_inner());
        if state.depth == 0 {
            let fd = open_lock_fd(runtime_dir, zone)?;
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
                    return Err(format!("flock failed: {err}"));
                }
                if Instant::now() >= deadline {
                    unsafe { libc::close(fd) };
                    return Err("lighting transaction is busy".to_string());
                }
                drop(state);
                thread::sleep(Duration::from_millis(20));
                state = entry.lock().unwrap_or_else(|e| e.into_inner());
                if state.depth > 0 {
                    // Another thread in this process acquired while we slept; reuse it.
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

/// Acquire single-instance daemon lock (`$XDG_RUNTIME_DIR/iteglow-session-restore.lock`).
/// Returns `Ok(None)` if another instance already holds the lock.
pub fn acquire_session_watcher_lock(runtime_dir: &Path) -> Result<Option<File>, String> {
    let my_uid = unsafe { libc::getuid() };
    let lock_path = runtime_dir.join("iteglow-session-restore.lock");
    let file = OpenOptions::new()
        .create(true)
        .read(true)
        .write(true)
        .mode(0o600)
        .custom_flags(libc::O_CLOEXEC | libc::O_NOFOLLOW)
        .open(&lock_path)
        .map_err(|e| format!("open session watcher lock failed: {e}"))?;
    let meta = file
        .metadata()
        .map_err(|e| format!("stat session watcher lock failed: {e}"))?;
    if !meta.is_file() || meta.uid() != my_uid || (meta.mode() & 0o022) != 0 {
        return Err("unsafe session watcher lock".to_string());
    }
    let rc = unsafe { libc::flock(file.as_raw_fd(), libc::LOCK_EX | libc::LOCK_NB) };
    if rc != 0 {
        let err = std::io::Error::last_os_error();
        if err.raw_os_error() == Some(libc::EWOULDBLOCK)
            || err.raw_os_error() == Some(libc::EAGAIN)
        {
            return Ok(None);
        }
        return Err(format!("flock session watcher lock failed: {err}"));
    }
    Ok(Some(file))
}

/// Safe read matching `lighting_session._read_saved`: rejects symlinks, wrong owner,
/// world-writable files, non-regular files, and oversized payloads.
pub fn read_saved_json(path: &Path, uid: u32, max_bytes: usize) -> Result<Option<Value>, String> {
    let c_path =
        CString::new(path.as_os_str().as_bytes()).map_err(|_| "invalid file path".to_string())?;
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
        return Err(format!("open failed: {err}"));
    }
    let mut file = unsafe { File::from_raw_fd(fd) };
    let meta = file.metadata().map_err(|e| format!("fstat failed: {e}"))?;
    if !meta.is_file() || meta.uid() != uid || (meta.mode() & 0o002) != 0 {
        return Err(
            "saved state must be a user-owned regular file, not world-writable".to_string(),
        );
    }
    if meta.len() > max_bytes as u64 {
        return Err("saved state exceeds size limit".to_string());
    }
    let mut buf = Vec::with_capacity((meta.len() as usize).min(max_bytes) + 1);
    let mut limited = (&mut file).take((max_bytes + 1) as u64);
    limited
        .read_to_end(&mut buf)
        .map_err(|e| format!("read failed: {e}"))?;
    if buf.len() > max_bytes {
        return Err("saved state exceeds size limit".to_string());
    }
    let val: Value = serde_json::from_slice(&buf).map_err(|e| format!("invalid JSON: {e}"))?;
    Ok(Some(val))
}

/// Safe read for system-owned telemetry JSON (e.g., `/run/mechrevo-wmi-state.json`).
pub fn read_system_json(path: &Path, max_bytes: usize) -> Option<Value> {
    let c_path = CString::new(path.as_os_str().as_bytes()).ok()?;
    let fd = unsafe {
        libc::open(
            c_path.as_ptr(),
            libc::O_RDONLY | libc::O_CLOEXEC | libc::O_NOFOLLOW | libc::O_NONBLOCK,
        )
    };
    if fd < 0 {
        return None;
    }
    let mut file = unsafe { File::from_raw_fd(fd) };
    let meta = file.metadata().ok()?;
    if !meta.is_file() || meta.len() > max_bytes as u64 {
        return None;
    }
    let mut buf = Vec::with_capacity((meta.len() as usize).min(max_bytes) + 1);
    (&mut file).take((max_bytes + 1) as u64).read_to_end(&mut buf).ok()?;
    if buf.len() > max_bytes {
        return None;
    }
    serde_json::from_slice(&buf).ok()
}

/// Atomic JSON write matching `iteglow.state.write` (preserves 0600 permissions).
pub fn write_state_json(path: &Path, data: &Value) -> Result<(), String> {
    if let Some(parent) = path.parent() {
        fs::create_dir_all(parent).map_err(|e| format!("create_dir_all failed: {e}"))?;
    }
    let tmp = PathBuf::from(format!("{}.tmp", path.display()));
    let mut file = OpenOptions::new()
        .create(true)
        .truncate(true)
        .write(true)
        .mode(0o600)
        .custom_flags(libc::O_CLOEXEC | libc::O_NOFOLLOW)
        .open(&tmp)
        .map_err(|e| format!("open temp state file failed: {e}"))?;
    let mut encoded = serde_json::to_string_pretty(data)
        .map_err(|e| format!("serialize state JSON failed: {e}"))?;
    encoded.push('\n');
    file.write_all(encoded.as_bytes())
        .map_err(|e| format!("write temp state file failed: {e}"))?;
    file.flush()
        .map_err(|e| format!("flush temp state file failed: {e}"))?;
    let _ = fs::set_permissions(&tmp, fs::Permissions::from_mode(0o600));
    fs::rename(&tmp, path).map_err(|e| format!("rename state file failed: {e}"))?;
    Ok(())
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::os::unix::fs::symlink;

    fn temp_dir(tag: &str) -> PathBuf {
        let p = std::env::temp_dir().join(format!(
            "iteglow-sessiond-test-{tag}-{}-{}",
            std::process::id(),
            Instant::now().elapsed().as_nanos()
        ));
        let _ = fs::remove_dir_all(&p);
        fs::create_dir_all(&p).unwrap();
        fs::set_permissions(&p, fs::Permissions::from_mode(0o700)).unwrap();
        p
    }

    #[test]
    fn test_read_saved_rejects_symlink_oversize_world_writable_and_wrong_uid() {
        let dir = temp_dir("read-saved");
        let uid = unsafe { libc::getuid() };

        // Missing file -> Ok(None)
        assert!(read_saved_json(&dir.join("missing.json"), uid, MAX_STATE_BYTES)
            .unwrap()
            .is_none());

        // Malformed JSON -> Err
        let p = dir.join("state.json");
        fs::write(&p, "{broken").unwrap();
        assert!(read_saved_json(&p, uid, MAX_STATE_BYTES).is_err());

        // Oversized -> Err
        fs::write(&p, " ".repeat(MAX_STATE_BYTES + 1)).unwrap();
        assert!(read_saved_json(&p, uid, MAX_STATE_BYTES).is_err());

        // Symlink -> Err
        fs::remove_file(&p).unwrap();
        let target = dir.join("real.json");
        fs::write(&target, r#"{"keyboard":{"mode":"off"}}"#).unwrap();
        symlink(&target, &p).unwrap();
        assert!(read_saved_json(&p, uid, MAX_STATE_BYTES).is_err());

        // World writable -> Err
        fs::remove_file(&p).unwrap();
        fs::write(&p, r#"{"keyboard":{"mode":"off"}}"#).unwrap();
        fs::set_permissions(&p, fs::Permissions::from_mode(0o666)).unwrap();
        assert!(read_saved_json(&p, uid, MAX_STATE_BYTES).is_err());

        // Wrong owner -> Err
        fs::set_permissions(&p, fs::Permissions::from_mode(0o600)).unwrap();
        assert!(read_saved_json(&p, uid + 1, MAX_STATE_BYTES).is_err());

        // Valid -> Ok(Some)
        let val = read_saved_json(&p, uid, MAX_STATE_BYTES).unwrap().unwrap();
        assert_eq!(val["keyboard"]["mode"], "off");

        let _ = fs::remove_dir_all(&dir);
    }

    #[test]
    fn test_zone_guard_reentrant_and_never_unlinks() {
        let dir = temp_dir("zone-guard");
        {
            let _g1 = ZoneGuard::acquire_in(&dir, "keyboard", Duration::from_secs(1)).unwrap();
            let _g2 = ZoneGuard::acquire_in(&dir, "keyboard", Duration::from_secs(1)).unwrap();
            assert!(dir.join("iteglow-keyboard.lock").exists());
        }
        // Must remain on disk (never unlinked).
        assert!(dir.join("iteglow-keyboard.lock").exists());
        assert!(ZoneGuard::acquire_in(&dir, "invalid-zone", Duration::from_secs(1)).is_err());
        let _ = fs::remove_dir_all(&dir);
    }
}

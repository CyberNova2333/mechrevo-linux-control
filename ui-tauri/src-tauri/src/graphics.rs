// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

use serde::{Deserialize, Serialize};
use std::fs;
use std::os::unix::fs::OpenOptionsExt;
use std::path::Path;
use std::process::{Command, Stdio};

const PRIME: &str = "/usr/bin/prime-select";
const PKEXEC: &str = "/usr/bin/pkexec";

#[derive(Clone, Copy, PartialEq, Debug, Serialize, Deserialize)]
#[serde(rename_all = "kebab-case")]
pub enum GraphicsProfile {
    Intel,
    OnDemand,
    Nvidia,
}

impl GraphicsProfile {
    fn argument(self) -> &'static str {
        match self {
            Self::Intel => "intel",
            Self::OnDemand => "on-demand",
            Self::Nvidia => "nvidia",
        }
    }
}

fn check_hardware(pci: &Path) -> Result<(), String> {
    let devices = fs::read_dir(pci).map_err(|e| format!("无法读取 PCI 设备：{e}"))?;
    let mut intel = false;
    let mut nvidia = false;
    for device in devices.flatten() {
        let path = device.path();
        if !fs::read_to_string(path.join("class"))
            .unwrap_or_default()
            .starts_with("0x03")
        {
            continue;
        }
        match fs::read_to_string(path.join("vendor"))
            .unwrap_or_default()
            .trim()
        {
            "0x8086" => intel = true,
            "0x10de" => nvidia = true,
            _ => {}
        }
    }
    if intel && nvidia {
        Ok(())
    } else {
        Err("此功能需要 Intel 核显和 NVIDIA 独显同时可见".into())
    }
}

fn query(prime: &Path) -> Result<GraphicsProfile, String> {
    let out = Command::new(prime)
        .arg("query")
        .output()
        .map_err(|e| format!("无法查询 PRIME 配置，请检查 nvidia-prime：{e}"))?;
    if out.status.success() {
        match String::from_utf8_lossy(&out.stdout).trim() {
            "intel" => return Ok(GraphicsProfile::Intel),
            "on-demand" => return Ok(GraphicsProfile::OnDemand),
            "nvidia" => return Ok(GraphicsProfile::Nvidia),
            _ => {}
        }
    }
    Err("无法识别系统当前的 PRIME 配置".into())
}

fn select(
    profile: GraphicsProfile,
    prime: &Path,
    pkexec: &Path,
    lock: &Path,
) -> Result<GraphicsProfile, String> {
    let guard = fs::OpenOptions::new()
        .create(true)
        .truncate(false)
        .write(true)
        .mode(0o600)
        .custom_flags(libc::O_NOFOLLOW | libc::O_CLOEXEC | libc::O_NONBLOCK)
        .open(lock)
        .map_err(|e| format!("无法打开 PRIME 操作锁：{e}"))?;
    guard
        .try_lock()
        .map_err(|e| format!("无法锁定 PRIME 配置，请稍后重试：{e}"))?;
    if query(prime)? != profile {
        // prime-select can be rebuilding the initramfs. Let it finish even if the page closes.
        let out = Command::new(pkexec)
            .arg("--disable-internal-agent")
            .arg(prime)
            .arg(profile.argument())
            .stdin(Stdio::null())
            .stdout(Stdio::null())
            .output()
            .map_err(|e| format!("无法启动 PRIME 配置工具：{e}"))?;
        if !out.status.success() {
            return Err(format!(
                "PRIME 切换被取消或失败（{}） {}",
                out.status,
                String::from_utf8_lossy(&out.stderr).trim()
            ));
        }
        if query(prime)? != profile {
            return Err("PRIME 配置回读与所选模式不一致".into());
        }
    }
    Ok(profile)
}

#[tauri::command]
pub async fn get_graphics_profile() -> Result<GraphicsProfile, String> {
    tauri::async_runtime::spawn_blocking(|| {
        check_hardware(Path::new("/sys/bus/pci/devices"))?;
        query(Path::new(PRIME))
    })
    .await
    .map_err(|e| e.to_string())?
}

#[tauri::command]
pub async fn set_graphics_profile(mode: GraphicsProfile) -> Result<GraphicsProfile, String> {
    tauri::async_runtime::spawn_blocking(move || {
        check_hardware(Path::new("/sys/bus/pci/devices"))?;
        select(
            mode,
            Path::new(PRIME),
            Path::new(PKEXEC),
            &crate::zone_lock::runtime_dir().join("iteglow-prime.lock"),
        )
    })
    .await
    .map_err(|e| e.to_string())?
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::os::unix::fs::PermissionsExt;

    #[test]
    fn selection_uses_authorized_writer_and_verifies_readback() {
        let dir = std::env::temp_dir().join(format!("iteglow-prime-test-{}", std::process::id()));
        fs::create_dir(&dir).unwrap();
        let prime = dir.join("prime-select");
        let writer = dir.join("pkexec");
        let lock = dir.join("lock");
        fs::write(prime.with_extension("state"), "on-demand").unwrap();
        fs::write(&prime, "#!/bin/sh\n[ \"$1\" = query ] && cat \"$0.state\"").unwrap();
        fs::set_permissions(&prime, fs::Permissions::from_mode(0o700)).unwrap();
        assert_eq!(
            select(GraphicsProfile::OnDemand, &prime, &writer, &lock).unwrap(),
            GraphicsProfile::OnDemand
        );
        assert!(select(GraphicsProfile::Intel, &prime, &writer, &lock).is_err());
        fs::write(&writer, "#!/bin/sh\nexit 0").unwrap();
        fs::set_permissions(&writer, fs::Permissions::from_mode(0o700)).unwrap();
        assert!(select(GraphicsProfile::Intel, &prime, &writer, &lock)
            .unwrap_err()
            .contains("回读"));
        fs::write(&writer, "#!/bin/sh\n[ \"$1\" = --disable-internal-agent ] && [ \"$#\" = 3 ] && echo \"$3\" > \"$2.state\"").unwrap();
        for mode in [
            GraphicsProfile::Intel,
            GraphicsProfile::Nvidia,
            GraphicsProfile::OnDemand,
        ] {
            assert_eq!(select(mode, &prime, &writer, &lock).unwrap(), mode);
        }
        let holder = fs::File::open(&lock).unwrap();
        holder.try_lock().unwrap();
        assert!(select(GraphicsProfile::Nvidia, &prime, &writer, &lock).is_err());
        assert!(serde_json::from_str::<GraphicsProfile>("\"other\"").is_err());
        fs::remove_dir_all(dir).unwrap();
    }
}

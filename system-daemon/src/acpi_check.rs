// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Read-only firmware ACPI table integrity checker and private boot baseline capture,
//! wire-compatible with `system/mechrevo-acpi-check`.

use crate::common::{kernel_release, unix_time_secs};
use crate::sha256::sha256_hex;
use serde_json::{json, Value};
use std::fs::{self, File, OpenOptions};
use std::io::{Read, Write};
use std::os::unix::fs::{DirBuilderExt, OpenOptionsExt, PermissionsExt};
use std::path::{Path, PathBuf};
use std::time::{SystemTime, UNIX_EPOCH};

pub const ACPI_TABLES_DIR: &str = "/sys/firmware/acpi/tables";
pub const ACPI_CAPTURE_DIR: &str = "/var/lib/mechrevo-acpi-check";

pub fn is_dsdt_or_ssdt(name: &str) -> bool {
    if name == "DSDT" {
        return true;
    }
    if let Some(rest) = name.strip_prefix("SSDT") {
        return rest.bytes().all(|b| b.is_ascii_digit());
    }
    false
}

pub fn inspect_table(name: &str, data: &[u8]) -> Value {
    let expected = if name == "DSDT" { "DSDT" } else { "SSDT" };
    let sig_bytes = &data[..data.len().min(4)];
    let signature: String = sig_bytes
        .iter()
        .map(|&b| if b < 0x80 { b as char } else { '\u{FFFD}' })
        .collect();
    let length: u32 = if data.len() >= 8 {
        u32::from_le_bytes([data[4], data[5], data[6], data[7]])
    } else {
        0
    };
    let sum_mod256: u64 = data.iter().map(|&b| b as u64).sum::<u64>() % 256;
    let mut reasons: Vec<&'static str> = Vec::new();
    if signature != expected {
        reasons.push("signature-mismatch");
    }
    if length < 36 || (length as usize) != data.len() {
        reasons.push("length-mismatch");
    }
    if sum_mod256 != 0 {
        reasons.push("checksum-mismatch");
    }
    let contains_npcf = data.windows(8).any(|w| w == b"NVDA0820");
    json!({
        "table": name,
        "signature": signature,
        "declared_length": length,
        "read_length": data.len(),
        "checksum_mod256": sum_mod256,
        "sha256": sha256_hex(data),
        "valid": reasons.is_empty(),
        "errors": reasons,
        "contains_npcf": contains_npcf,
    })
}

fn is_capture_folder_name(name: &str) -> Option<u128> {
    let (prefix, suffix) = name.rsplit_once('-')?;
    if prefix.len() != 36
        || !prefix
            .bytes()
            .all(|b| matches!(b, b'0'..=b'9' | b'a'..=b'f' | b'-'))
    {
        return None;
    }
    if suffix.is_empty() || !suffix.bytes().all(|b| b.is_ascii_digit()) {
        return None;
    }
    suffix.parse::<u128>().ok()
}

fn write_private_file(path: &Path, bytes: &[u8]) -> Result<(), String> {
    let mut f = OpenOptions::new()
        .create(true)
        .truncate(true)
        .write(true)
        .mode(0o600)
        .custom_flags(libc::O_CLOEXEC | libc::O_NOFOLLOW)
        .open(path)
        .map_err(|e| format!("写入 {} 失败: {e}", path.display()))?;
    f.write_all(bytes)
        .map_err(|e| format!("写入 {} 失败: {e}", path.display()))?;
    Ok(())
}

pub fn collect(tables_dir: &Path, dest_dir: &Path, capture: bool) -> Result<Value, String> {
    let mut entries: Vec<(String, PathBuf)> = Vec::new();
    let rd = fs::read_dir(tables_dir)
        .map_err(|e| format!("读取 ACPI 目录 {} 失败: {e}", tables_dir.display()))?;
    for entry in rd {
        let entry = entry.map_err(|e| format!("遍历 ACPI 目录失败: {e}"))?;
        let fname = entry.file_name();
        let Some(name) = fname.to_str() else {
            continue;
        };
        if is_dsdt_or_ssdt(name) {
            entries.push((name.to_string(), entry.path()));
        }
    }
    entries.sort_by(|a, b| a.0.cmp(&b.0));

    let mut records: Vec<Value> = Vec::with_capacity(entries.len());
    let mut raw_tables: Vec<(String, Vec<u8>)> = Vec::new();
    for (name, path) in entries {
        let f = File::open(&path).map_err(|e| format!("打开 ACPI 表 {} 失败: {e}", path.display()))?;
        let mut buf = Vec::new();
        f.take(1024 * 1024)
            .read_to_end(&mut buf)
            .map_err(|e| format!("读取 ACPI 表 {} 失败: {e}", path.display()))?;
        records.push(inspect_table(&name, &buf));
        if capture {
            raw_tables.push((name, buf));
        }
    }

    let boot_id = fs::read_to_string("/proc/sys/kernel/random/boot_id")
        .map(|s| s.trim().to_string())
        .map_err(|e| format!("读取 boot_id 失败: {e}"))?;
    let kernel = kernel_release()?;
    let healthy = !records.is_empty()
        && records
            .iter()
            .all(|r| r.get("valid").and_then(|v| v.as_bool()) == Some(true))
        && records
            .iter()
            .any(|r| r.get("contains_npcf").and_then(|v| v.as_bool()) == Some(true));

    let mut report = json!({
        "boot_id": boot_id,
        "time_unix": unix_time_secs(),
        "kernel": kernel,
        "tables": records,
        "healthy": healthy,
    });

    if capture {
        unsafe {
            libc::umask(0o077);
        }
        fs::DirBuilder::new()
            .recursive(true)
            .mode(0o700)
            .create(dest_dir)
            .map_err(|e| format!("创建目录 {} 失败: {e}", dest_dir.display()))?;
        let _ = fs::set_permissions(dest_dir, fs::Permissions::from_mode(0o700));
        let nanos = SystemTime::now()
            .duration_since(UNIX_EPOCH)
            .map(|d| d.as_nanos())
            .unwrap_or(0);
        let folder = dest_dir.join(format!("{boot_id}-{nanos}"));
        fs::DirBuilder::new()
            .mode(0o700)
            .create(&folder)
            .map_err(|e| format!("创建采样目录 {} 失败: {e}", folder.display()))?;
        for (name, data) in &raw_tables {
            write_private_file(&folder.join(format!("{name}.aml")), data)?;
        }
        let pretty = serde_json::to_string_pretty(&report)
            .map_err(|e| format!("序列化报告失败: {e}"))?;
        write_private_file(&folder.join("report.json"), pretty.as_bytes())?;
        let latest_tmp = dest_dir.join("latest.new");
        write_private_file(&latest_tmp, pretty.as_bytes())?;
        fs::rename(&latest_tmp, dest_dir.join("latest.json"))
            .map_err(|e| format!("更新 latest.json 失败: {e}"))?;
        if let Some(obj) = report.as_object_mut() {
            obj.insert(
                "capture_directory".to_string(),
                json!(folder.to_string_lossy().to_string()),
            );
        }

        // Bound private diagnostics, keeping newest 8 captures.
        if let Ok(rd) = fs::read_dir(dest_dir) {
            let mut captures: Vec<(u128, PathBuf)> = Vec::new();
            for entry in rd.flatten() {
                let p = entry.path();
                let Ok(meta) = fs::symlink_metadata(&p) else {
                    continue;
                };
                if !meta.is_dir() || meta.file_type().is_symlink() {
                    continue;
                }
                if let Some(name) = p.file_name().and_then(|n| n.to_str()) {
                    if let Some(ts) = is_capture_folder_name(name) {
                        captures.push((ts, p));
                    }
                }
            }
            captures.sort_by_key(|(ts, _)| *ts);
            if captures.len() > 8 {
                for (_, old) in &captures[..captures.len() - 8] {
                    let _ = fs::remove_dir_all(old);
                }
            }
        }
    }

    Ok(report)
}

pub fn summarize(report: &Value) -> Value {
    let mut out = serde_json::Map::new();
    let mut table_count = 0usize;
    let mut invalid_tables: Vec<Value> = Vec::new();
    if let Some(obj) = report.as_object() {
        for (k, v) in obj {
            if k == "tables" {
                if let Some(arr) = v.as_array() {
                    table_count = arr.len();
                    for r in arr {
                        if r.get("valid").and_then(|b| b.as_bool()) != Some(true) {
                            if let Some(t) = r.get("table") {
                                invalid_tables.push(t.clone());
                            }
                        }
                    }
                }
            } else {
                out.insert(k.clone(), v.clone());
            }
        }
    }
    out.insert("table_count".to_string(), json!(table_count));
    out.insert("invalid_tables".to_string(), Value::Array(invalid_tables));
    Value::Object(out)
}

#[cfg(test)]
mod tests {
    use super::*;

    fn make_valid_aml(sig: &[u8; 4], payload: &[u8]) -> Vec<u8> {
        let len = (36 + payload.len()) as u32;
        let mut buf = vec![0u8; len as usize];
        buf[0..4].copy_from_slice(sig);
        buf[4..8].copy_from_slice(&len.to_le_bytes());
        buf[36..].copy_from_slice(payload);
        let sum: u8 = buf.iter().fold(0u8, |acc, &b| acc.wrapping_add(b));
        buf[9] = 0u8.wrapping_sub(sum);
        buf
    }

    #[test]
    fn inspect_valid_and_corrupted_tables() {
        let good = make_valid_aml(b"SSDT", b"hello NVDA0820 world");
        let rep = inspect_table("SSDT1", &good);
        assert_eq!(rep["valid"], true);
        assert_eq!(rep["contains_npcf"], true);
        assert_eq!(rep["checksum_mod256"], 0);

        let mut bad = good.clone();
        bad[10] ^= 0xFF;
        let rep_bad = inspect_table("SSDT1", &bad);
        assert_eq!(rep_bad["valid"], false);
        assert_eq!(rep_bad["errors"], json!(["checksum-mismatch"]));

        let rep_wrong_sig = inspect_table("DSDT", &good);
        assert_eq!(rep_wrong_sig["valid"], false);
        assert_eq!(rep_wrong_sig["errors"], json!(["signature-mismatch"]));
    }

    #[test]
    fn collect_and_prune_captures() {
        let base = std::env::temp_dir().join(format!("acpi-test-{}", std::process::id()));
        let _ = fs::remove_dir_all(&base);
        let tables = base.join("tables");
        let dest = base.join("dest");
        fs::create_dir_all(&tables).unwrap();
        fs::write(tables.join("DSDT"), make_valid_aml(b"DSDT", b"root")).unwrap();
        fs::write(tables.join("SSDT1"), make_valid_aml(b"SSDT", b"NVDA0820")).unwrap();
        fs::write(tables.join("FACP"), b"ignored").unwrap();

        let rep = collect(&tables, &dest, true).unwrap();
        assert_eq!(rep["healthy"], true);
        let sum = summarize(&rep);
        assert_eq!(sum["table_count"], 2);
        assert_eq!(sum["invalid_tables"], json!([]));
        assert!(dest.join("latest.json").is_file());

        let _ = fs::remove_dir_all(&base);
    }
}

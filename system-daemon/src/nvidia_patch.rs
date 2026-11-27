// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Idempotent NVIDIA open-gpu-kernel-modules DKMS hibernate-shutdown patcher,
//! byte-compatible with `system/mechrevo-nvidia-hibernate-patch`.

use std::fs;
use std::path::{Path, PathBuf};

pub const MARKER: &str = "mechrevo-nvidia-hibernate-patch";

const NV_PROTO_OLD: &str = "#if defined(CONFIG_PM)\nNV_STATUS     nv_set_system_power_state (nv_power_state_t, nv_pm_action_depth_t);\n#endif";
const NV_PROTO_NEW: &str = "#if defined(CONFIG_PM)\nNV_STATUS     nv_set_system_power_state (nv_power_state_t, nv_pm_action_depth_t);\nNvBool        nv_is_in_hibernate        (void);\n#endif";

const NV_C_OLD: &str = "    nv_system_power_state = power_state;\n\ndone:\n    up(&nv_system_power_state_lock);\n\n    return status;\n}";
const NV_C_NEW: &str = "    nv_system_power_state = power_state;\n\ndone:\n    up(&nv_system_power_state_lock);\n\n    return status;\n}\n\nNvBool\nnv_is_in_hibernate(void)\n{\n    return READ_ONCE(nv_system_power_state) == NV_POWER_STATE_IN_HIBERNATE;\n}";

const NV_PCI_OLD: &str = "    if (nvl->is_forced_shutdown)\n    {\n        nvl->is_forced_shutdown = NV_FALSE;\n        return;\n    }\n\n#if defined(CONFIG_PM_DEVFREQ)\n    nv_pci_tegra_unregister_devfreq(pci_dev);\n#endif\n\n    nv_pci_remove_helper(pci_dev, false);\n\n    if (pci_get_drvdata(pci_dev) != NULL)\n    {\n        nvl->nv_state.is_shutdown = NV_TRUE;\n    }";

const NV_PCI_NEW: &str = "    if (nvl->is_forced_shutdown)\n    {\n        nvl->is_forced_shutdown = NV_FALSE;\n        return;\n    }\n\n#if defined(CONFIG_PM)\n    /*\n     * mechrevo-nvidia-hibernate-patch: Skip device removal/unload during shutdown\n     * if the GPU is already suspended/frozen for hibernation (e.g. HibernateMode=shutdown).\n     * Calling nv_pci_remove_helper() while PM locks are held deadlocks before poweroff.\n     */\n    if (!nv_is_in_hibernate() && !(READ_ONCE(nvl->nv_state.flags) & NV_FLAG_SUSPENDED))\n#endif\n    {\n#if defined(CONFIG_PM_DEVFREQ)\n        nv_pci_tegra_unregister_devfreq(pci_dev);\n#endif\n\n        nv_pci_remove_helper(pci_dev, false);\n\n        if (pci_get_drvdata(pci_dev) != NULL)\n        {\n            nvl->nv_state.is_shutdown = NV_TRUE;\n        }\n    }";

fn extract_nv_pci_shutdown(nv_pci_text: &str) -> Result<&str, String> {
    // Match: static <ws> void <ws> nv_pci_shutdown <ws?> ( <ws?> struct <ws> pci_dev <ws?> * <ws?> pci_dev <ws?> ) <ws?> {
    let bytes = nv_pci_text.as_bytes();
    let mut search_from = 0usize;
    let found = loop {
        let Some(rel) = nv_pci_text[search_from..].find("nv_pci_shutdown") else {
            return Err("Could not locate nv_pci_shutdown() in nvidia/nv-pci.c".to_string());
        };
        let fn_pos = search_from + rel;
        // Walk backwards over whitespace + "void" + whitespace + "static"
        let prefix = nv_pci_text[..fn_pos].trim_end();
        let ok_prefix = if let Some(before_void) = prefix.strip_suffix("void") {
            let b = before_void.trim_end();
            b.ends_with("static")
        } else {
            false
        };
        let after_name = &nv_pci_text[fn_pos + "nv_pci_shutdown".len()..];
        if ok_prefix {
            let trimmed = after_name.trim_start();
            if let Some(inside) = trimmed.strip_prefix('(') {
                if let Some(close_idx) = inside.find(')') {
                    let sig = inside[..close_idx].trim();
                    let tokens: Vec<&str> = sig
                        .split(|c: char| c.is_whitespace() || c == '*')
                        .filter(|t| !t.is_empty())
                        .collect();
                    if tokens == ["struct", "pci_dev", "pci_dev"] && sig.contains('*') {
                        let after_paren = &inside[close_idx + 1..];
                        let ws_len = after_paren.len() - after_paren.trim_start().len();
                        if after_paren[ws_len..].starts_with('{') {
                            let brace_offset = (after_paren.as_ptr() as usize
                                - nv_pci_text.as_ptr() as usize)
                                + ws_len;
                            // Find "static" start
                            let static_pos = prefix
                                .strip_suffix("void")
                                .unwrap()
                                .trim_end()
                                .len()
                                - "static".len();
                            break (static_pos, brace_offset);
                        }
                    }
                }
            }
        }
        search_from = fn_pos + "nv_pci_shutdown".len();
    };

    let (start, open_brace) = found;
    let mut depth = 1usize;
    let mut idx = open_brace + 1;
    while idx < bytes.len() && depth > 0 {
        match bytes[idx] {
            b'{' => depth += 1,
            b'}' => depth -= 1,
            _ => {}
        }
        idx += 1;
    }
    if depth != 0 {
        return Err("Unbalanced braces while parsing nv_pci_shutdown() in nvidia/nv-pci.c".to_string());
    }
    Ok(&nv_pci_text[start..idx])
}

fn replace_once(text: &str, old: &str, new: &str, label: &str) -> Result<String, String> {
    let count = text.matches(old).count();
    if count != 1 {
        return Err(format!("Expected 1 match for {label}, found {count}"));
    }
    Ok(text.replacen(old, new, 1))
}

pub fn inspect_tree(build_dir: &Path) -> Result<&'static str, String> {
    let nv_proto = build_dir.join("common/inc/nv-proto.h");
    let nv_c = build_dir.join("nvidia/nv.c");
    let nv_pci = build_dir.join("nvidia/nv-pci.c");
    for path in [&nv_proto, &nv_c, &nv_pci] {
        if !path.is_file() {
            return Err(format!("Missing expected NVIDIA source file: {}", path.display()));
        }
    }

    let proto_text = fs::read_to_string(&nv_proto)
        .map_err(|e| format!("Failed to read {}: {e}", nv_proto.display()))?;
    let c_text = fs::read_to_string(&nv_c)
        .map_err(|e| format!("Failed to read {}: {e}", nv_c.display()))?;
    let pci_text = fs::read_to_string(&nv_pci)
        .map_err(|e| format!("Failed to read {}: {e}", nv_pci.display()))?;

    let f0 = proto_text.contains("nv_is_in_hibernate");
    let f1 = c_text.contains("nv_is_in_hibernate(");
    let f2 = pci_text.contains(MARKER) && pci_text.contains("nv_is_in_hibernate()");
    if f0 && f1 && f2 {
        return Ok("already_patched");
    }
    if f0 || f1 || f2 {
        return Err("Partially patched NVIDIA tree detected".to_string());
    }

    let shutdown_fn = extract_nv_pci_shutdown(&pci_text)?;
    if !shutdown_fn.contains("nv_pci_remove_helper") {
        return Ok("upstream_no_remove_helper");
    }
    if shutdown_fn.contains("NV_FLAG_SUSPENDED")
        || shutdown_fn.contains("NV_POWER_STATE_IN_HIBERNATE")
        || shutdown_fn.contains("nv_system_power_state")
    {
        return Ok("upstream_already_guarded");
    }
    Ok("needs_patch")
}

pub fn apply_patch(build_dir: &Path) -> Result<&'static str, String> {
    let status = inspect_tree(build_dir)?;
    if status != "needs_patch" {
        return Ok(status);
    }

    let targets: [(PathBuf, &str, &str, &str); 3] = [
        (build_dir.join("common/inc/nv-proto.h"), NV_PROTO_OLD, NV_PROTO_NEW, "nv-proto.h"),
        (build_dir.join("nvidia/nv.c"), NV_C_OLD, NV_C_NEW, "nv.c"),
        (build_dir.join("nvidia/nv-pci.c"), NV_PCI_OLD, NV_PCI_NEW, "nv-pci.c"),
    ];

    let mut originals: Vec<(PathBuf, String)> = Vec::with_capacity(3);
    let mut updated: Vec<(PathBuf, String)> = Vec::with_capacity(3);

    for (path, old, new, label) in targets {
        let orig = fs::read_to_string(&path)
            .map_err(|e| format!("Failed to read {}: {e}", path.display()))?;
        let next = replace_once(&orig, old, new, label)?;
        originals.push((path.clone(), orig));
        updated.push((path, next));
    }

    let pci_fn = extract_nv_pci_shutdown(&updated[2].1)?;
    if !originals[2].1.contains(NV_PCI_OLD) || !pci_fn.contains("nv_is_in_hibernate()") {
        return Err("Patched nv_pci_shutdown() sanity check failed".to_string());
    }

    let mut written: Vec<usize> = Vec::new();
    for (idx, (path, content)) in updated.iter().enumerate() {
        let tmp = path.with_extension(format!("mechrevo-tmp-{}", std::process::id()));
        let write_ok = (|| -> Result<(), std::io::Error> {
            use std::io::Write;
            use std::os::unix::fs::OpenOptionsExt;
            let mut f = fs::OpenOptions::new()
                .write(true)
                .create_new(true)
                .custom_flags(libc::O_NOFOLLOW | libc::O_CLOEXEC)
                .open(&tmp)?;
            f.write_all(content.as_bytes())?;
            f.sync_all()?;
            fs::rename(&tmp, path)?;
            Ok(())
        })();
        if let Err(err) = write_ok {
            let _ = fs::remove_file(&tmp);
            for w in written {
                let _ = fs::write(&originals[w].0, &originals[w].1);
            }
            return Err(format!("Failed to write {}: {err}", path.display()));
        }
        written.push(idx);
    }
    Ok("applied")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn apply_and_idempotent_recheck() {
        let dir = std::env::temp_dir().join(format!("nv-patch-test-{}", std::process::id()));
        let _ = fs::remove_dir_all(&dir);
        fs::create_dir_all(dir.join("common/inc")).unwrap();
        fs::create_dir_all(dir.join("nvidia")).unwrap();
        fs::write(dir.join("common/inc/nv-proto.h"), format!("/* hdr */\n{NV_PROTO_OLD}\n")).unwrap();
        fs::write(dir.join("nvidia/nv.c"), format!("NV_STATUS fn(void) {{\n{NV_C_OLD}\n")).unwrap();
        fs::write(
            dir.join("nvidia/nv-pci.c"),
            format!("static void nv_pci_shutdown(struct pci_dev *pci_dev)\n{{\n{NV_PCI_OLD}\n}}\n"),
        )
        .unwrap();

        assert_eq!(inspect_tree(&dir).unwrap(), "needs_patch");
        assert_eq!(apply_patch(&dir).unwrap(), "applied");
        assert_eq!(inspect_tree(&dir).unwrap(), "already_patched");
        assert_eq!(apply_patch(&dir).unwrap(), "already_patched");

        let _ = fs::remove_dir_all(&dir);
    }
}

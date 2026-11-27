// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

use mechrevo_system_daemon::nvidia_patch::{apply_patch, inspect_tree, MARKER};
use std::path::PathBuf;
use std::process::ExitCode;

fn main() -> ExitCode {
    let args: Vec<String> = std::env::args().skip(1).collect();
    let mut mode: Option<&str> = None;
    let mut dir_arg: Option<String> = None;
    for arg in &args {
        match arg.as_str() {
            "--apply" => {
                if mode.is_some() {
                    eprintln!("{MARKER}: ERROR: --apply and --check are mutually exclusive");
                    return ExitCode::FAILURE;
                }
                mode = Some("apply");
            }
            "--check" => {
                if mode.is_some() {
                    eprintln!("{MARKER}: ERROR: --apply and --check are mutually exclusive");
                    return ExitCode::FAILURE;
                }
                mode = Some("check");
            }
            a if a.starts_with('-') => {
                eprintln!("{MARKER}: ERROR: unknown option {a}");
                return ExitCode::FAILURE;
            }
            other => {
                if dir_arg.is_some() {
                    eprintln!("{MARKER}: ERROR: unexpected extra argument {other}");
                    return ExitCode::FAILURE;
                }
                dir_arg = Some(other.to_string());
            }
        }
    }
    let Some(mode) = mode else {
        eprintln!("Usage: {MARKER} (--apply | --check) [dir]");
        return ExitCode::FAILURE;
    };
    let raw_target = PathBuf::from(dir_arg.unwrap_or_else(|| ".".to_string()));
    let target = std::fs::canonicalize(&raw_target).unwrap_or(raw_target);
    let res = if mode == "apply" {
        apply_patch(&target)
    } else {
        inspect_tree(&target)
    };
    match res {
        Ok(status) => {
            println!("{MARKER}: {status} ({})", target.display());
            ExitCode::SUCCESS
        }
        Err(err) => {
            eprintln!("{MARKER}: ERROR: {err}");
            ExitCode::FAILURE
        }
    }
}

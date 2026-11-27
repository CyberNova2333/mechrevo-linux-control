// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Preserve nvidia-powerd activation across sleep, only with healthy resumed AML.

use mechrevo_system_daemon::common::run_command_timeout;
use std::fs::{self, OpenOptions};
use std::io::Write;
use std::os::unix::fs::OpenOptionsExt;
use std::path::Path;
use std::process::ExitCode;
use std::time::Duration;

const MARKER: &str = "/run/mechrevo-powerd-was-active";
const UNIT: &str = "nvidia-powerd.service";

fn run_rc(program: &str, args: &[&str]) -> i32 {
    run_command_timeout(program, args, Duration::from_secs(20))
        .map(|o| o.code)
        .unwrap_or(1)
}

fn run_phase(phase: &str) -> Result<(), String> {
    let marker = Path::new(MARKER);
    match phase {
        "pre" => {
            if run_rc("/usr/bin/systemctl", &["is-active", "--quiet", UNIT]) == 0 {
                let mut f = OpenOptions::new()
                    .write(true)
                    .create(true)
                    .truncate(true)
                    .mode(0o600)
                    .custom_flags(libc::O_NOFOLLOW | libc::O_CLOEXEC)
                    .open(marker)
                    .map_err(|e| format!("Failed to write marker: {e}"))?;
                f.write_all(b"active\n")
                    .map_err(|e| format!("Failed to write marker: {e}"))?;
                if run_rc("/usr/bin/systemctl", &["stop", UNIT]) != 0 {
                    return Err("Failed to stop powerd before sleep".to_string());
                }
            } else if !marker.exists() {
                println!("powerd was inactive; will remain inactive after sleep");
            }
            Ok(())
        }
        "post" => {
            if !marker.exists() {
                return Ok(());
            }
            let healthy = run_rc(
                "/usr/local/libexec/mechrevo-acpi-check",
                &["--capture", "--summary"],
            ) == 0;
            if !healthy {
                let _ = fs::remove_file(marker);
                eprintln!("ACPI check failed; powerd remains stopped. Normal reboot required.");
                return Ok(());
            }
            if run_rc("/usr/bin/systemctl", &["--no-block", "start", UNIT]) != 0 {
                return Err("Failed to queue powerd restart".to_string());
            }
            let _ = fs::remove_file(marker);
            Ok(())
        }
        _ => Err("Usage: mechrevo-powerd-sleep pre|post".to_string()),
    }
}

fn main() -> ExitCode {
    let args: Vec<String> = std::env::args().skip(1).collect();
    let res = if args.len() != 1 {
        Err("Usage: mechrevo-powerd-sleep pre|post".to_string())
    } else {
        run_phase(&args[0])
    };
    match res {
        Ok(()) => ExitCode::SUCCESS,
        Err(err) => {
            eprintln!("{err}");
            ExitCode::FAILURE
        }
    }
}

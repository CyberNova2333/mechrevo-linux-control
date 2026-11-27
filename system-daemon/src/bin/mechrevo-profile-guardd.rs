// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Native Rust root EC guard daemon for `mechrevo-profile-guard.service`.

use mechrevo_system_daemon::ec_guard::{reset_tuning, run_guard, sample_lc_status};
use serde_json::json;

const VERSION: &str = env!("CARGO_PKG_VERSION");

fn main() {
    let args: Vec<String> = std::env::args().skip(1).collect();
    if args.iter().any(|a| a == "--version" || a == "-V") {
        println!("mechrevo-profile-guardd {VERSION}");
        return;
    }
    if args.iter().any(|a| a == "--help" || a == "-h") {
        println!("mechrevo-profile-guardd {VERSION}");
        println!("MECHREVO EC tuning recovery guard, WMI event dispatcher & LC telemetry publisher (Rust).");
        println!("Usage:");
        println!("  mechrevo-profile-guardd [guard]      # run resident EC/WMI guard");
        println!("  mechrevo-profile-guardd lc-status    # read-only water-cooling EC snapshot");
        println!("  mechrevo-profile-guardd reset        # release managed tuning safely");
        return;
    }

    match args.first().map(String::as_str) {
        None | Some("guard") if args.len() <= 1 => {
            if let Err(e) = run_guard() {
                println!("{}", json!({"error": e}));
                std::process::exit(1);
            }
        }
        Some("lc-status") if args.len() == 1 => match sample_lc_status() {
            Ok(v) => println!("{v}"),
            Err(e) => {
                println!("{}", json!({"error": e}));
                std::process::exit(1);
            }
        },
        Some("reset") if args.len() == 1 => match reset_tuning() {
            Ok(v) => println!("{v}"),
            Err(e) => {
                println!("{}", json!({"error": e}));
                std::process::exit(1);
            }
        },
        _ => {
            println!("{}", json!({"error": "用法: mechrevo-profile-guardd [guard | lc-status | reset]"}));
            std::process::exit(1);
        }
    }
}

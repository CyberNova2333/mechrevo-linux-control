// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Native Rust root supervisor for `mechrevo-turbo.service` (`/dev/mechrevo-turbo` lease).

use mechrevo_system_daemon::turbo::{client_request, doctor, run_daemon};
use serde_json::json;

const VERSION: &str = env!("CARGO_PKG_VERSION");

fn main() {
    let args: Vec<String> = std::env::args().skip(1).collect();
    if args.iter().any(|a| a == "--version" || a == "-V") {
        println!("mechrevo-turbod {VERSION}");
        return;
    }
    if args.iter().any(|a| a == "--help" || a == "-h") {
        println!("mechrevo-turbod {VERSION}");
        println!("Exact-model MECHREVO turbo workload lease supervisor (Rust).");
        println!("Usage:");
        println!("  mechrevo-turbod                      # run resident daemon");
        println!("  mechrevo-turbod doctor               # read-only identity & path check");
        println!("  mechrevo-turbod status|release|suspend|resume");
        println!("  mechrevo-turbod select quiet|extreme");
        return;
    }

    let result = match args.first().map(String::as_str) {
        None | Some("daemon") => {
            if let Err(e) = run_daemon() {
                println!("{}", json!({"ok": false, "error": e}));
                std::process::exit(1);
            }
            return;
        }
        Some("doctor") if args.len() == 1 => doctor().map(|r| json!({"ok": true, "result": r})),
        Some(cmd @ ("status" | "release" | "suspend" | "resume")) if args.len() == 1 => {
            client_request(&json!({"command": cmd}))
        }
        Some("select") if args.len() == 2 => {
            client_request(&json!({"command": "select", "variant": args[1]}))
        }
        _ => Err("用法: mechrevo-turbod [daemon | doctor | status | select quiet|extreme | release | suspend | resume]".to_string()),
    };

    match result {
        Ok(resp) => {
            let ok = resp.get("ok").and_then(|v| v.as_bool()).unwrap_or(false);
            println!("{resp}");
            if !ok {
                std::process::exit(1);
            }
        }
        Err(e) => {
            println!("{}", json!({"ok": false, "error": e}));
            std::process::exit(1);
        }
    }
}

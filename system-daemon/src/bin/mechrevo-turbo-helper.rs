// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Root-only control socket client for `mechrevo-turbod`; always outputs JSON.

use mechrevo_system_daemon::common::{
    expected_identity, identity, turbo_socket_request, TURBO_DEVICE, TURBO_SOCKET,
};
use mechrevo_system_daemon::turbo::QUALIFICATION_PATH;
use serde_json::{json, Value};
use std::path::Path;
use std::process::ExitCode;
use std::time::Duration;

fn doctor() -> Result<Value, String> {
    let current = identity()?;
    let matches = current == expected_identity();
    Ok(json!({
        "candidate": true,
        "auth_required": "root for live operations",
        "identity": current,
        "identity_matches": matches,
        "bridge_exists": Path::new(TURBO_DEVICE).exists(),
        "qualification_exists": Path::new(QUALIFICATION_PATH).exists(),
        "daemon_socket_exists": Path::new(TURBO_SOCKET).exists(),
        "live_read_performed": false,
    }))
}

fn run() -> Result<Value, String> {
    let args: Vec<String> = std::env::args().skip(1).collect();
    if args.is_empty() {
        return Err("Usage: mechrevo-turbo-helper doctor|status|release|suspend|resume|select quiet|extreme".to_string());
    }
    match args[0].as_str() {
        "doctor" if args.len() == 1 => Ok(json!({"ok": true, "result": doctor()?})),
        cmd @ ("status" | "release" | "suspend" | "resume") if args.len() == 1 => {
            turbo_socket_request(
                Path::new(TURBO_SOCKET),
                &json!({"command": cmd}),
                Duration::from_secs(4),
            )
        }
        "select" if args.len() == 2 && matches!(args[1].as_str(), "quiet" | "extreme") => {
            turbo_socket_request(
                Path::new(TURBO_SOCKET),
                &json!({"command": "select", "variant": args[1]}),
                Duration::from_secs(4),
            )
        }
        _ => Err("Usage: mechrevo-turbo-helper doctor|status|release|suspend|resume|select quiet|extreme".to_string()),
    }
}

fn main() -> ExitCode {
    match run() {
        Ok(res) => {
            let ok = res.get("ok").and_then(|v| v.as_bool()) == Some(true);
            println!("{res}");
            if ok {
                ExitCode::SUCCESS
            } else {
                ExitCode::FAILURE
            }
        }
        Err(err) => {
            println!("{}", json!({"ok": false, "error": err}));
            ExitCode::FAILURE
        }
    }
}

// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

use mechrevo_system_daemon::lifecycle::run_live;
use serde_json::json;
use std::process::ExitCode;

fn main() -> ExitCode {
    let args: Vec<String> = std::env::args().skip(1).collect();
    if args.len() != 1 || !matches!(args[0].as_str(), "prepare" | "pre" | "post") {
        println!(
            "{}",
            json!({"ok": false, "error": "Usage: mechrevo-turbo-lifecycle prepare|pre|post"})
        );
        return ExitCode::FAILURE;
    }
    match run_live(&args[0]) {
        Ok(res) => {
            println!("{}", json!({"ok": true, "result": res}));
            ExitCode::SUCCESS
        }
        Err(err) => {
            println!("{}", json!({"ok": false, "error": err}));
            ExitCode::FAILURE
        }
    }
}

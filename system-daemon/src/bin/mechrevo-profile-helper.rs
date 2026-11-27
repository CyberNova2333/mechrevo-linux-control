// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

use mechrevo_system_daemon::profile_helper::run_cli;
use serde_json::{json, Value};
use std::process::ExitCode;

fn main() -> ExitCode {
    let args: Vec<String> = std::env::args().skip(1).collect();
    match run_cli(&args) {
        Ok(Value::Null) => ExitCode::SUCCESS,
        Ok(val) => {
            println!("{val}");
            ExitCode::SUCCESS
        }
        Err(err) => {
            println!("{}", json!({"error": err}));
            ExitCode::FAILURE
        }
    }
}

// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

use mechrevo_system_daemon::acpi_check::{collect, summarize, ACPI_CAPTURE_DIR, ACPI_TABLES_DIR};
use serde_json::json;
use std::collections::BTreeSet;
use std::path::Path;
use std::process::ExitCode;

fn run() -> Result<bool, String> {
    let args: Vec<String> = std::env::args().skip(1).collect();
    let mut seen = BTreeSet::new();
    for a in &args {
        if (a != "--capture" && a != "--summary") || !seen.insert(a.as_str()) {
            return Err("Usage: mechrevo-acpi-check [--capture] [--summary]".to_string());
        }
    }
    let capture = seen.contains("--capture");
    let summary = seen.contains("--summary");
    let result = collect(Path::new(ACPI_TABLES_DIR), Path::new(ACPI_CAPTURE_DIR), capture)?;
    let healthy = result.get("healthy").and_then(|v| v.as_bool()) == Some(true);
    let output = if summary { summarize(&result) } else { result };
    println!("{output}");
    Ok(healthy)
}

fn main() -> ExitCode {
    match run() {
        Ok(true) => ExitCode::SUCCESS,
        Ok(false) => ExitCode::FAILURE,
        Err(err) => {
            println!("{}", json!({"healthy": false, "error": err}));
            ExitCode::FAILURE
        }
    }
}

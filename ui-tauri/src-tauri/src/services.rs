// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Allowlisted systemd service inspection, lifecycle control, and journal log viewer
//! for MECHREVO Linux Control user and system background services.

use serde::Serialize;
use serde_json::{json, Value};
use std::collections::HashMap;
use std::process::Stdio;
use std::time::Duration;
use tokio::process::Command;

const SYSTEMCTL: &str = "/usr/bin/systemctl";
const JOURNALCTL: &str = "/usr/bin/journalctl";
const PKEXEC: &str = "/usr/bin/pkexec";

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum ServiceScope {
    User,
    System,
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum ServiceKind {
    Daemon,
    Hook,
}

#[derive(Clone, Copy, Debug)]
struct ServiceSpec {
    unit: &'static str,
    title: &'static str,
    summary: &'static str,
    binary: &'static str,
    scope: ServiceScope,
    kind: ServiceKind,
}

const MANAGED_SERVICES: &[ServiceSpec] = &[
    ServiceSpec {
        unit: "iteglow-watercooling.service",
        title: "冰河蓝牙水冷守护",
        summary: "管理外置蓝牙水冷 (LCT22002) 连接、温控曲线、断流保护与双区灯效",
        binary: "iteglow-watercoolingd",
        scope: ServiceScope::User,
        kind: ServiceKind::Daemon,
    },
    ServiceSpec {
        unit: "iteglow-session-restore.service",
        title: "会话灯光与情景联动守护",
        summary: "维持三区灯效恢复、Fn 快捷键同步、AC/DC 亮度策略与前台进程联动",
        binary: "iteglow-sessiond",
        scope: ServiceScope::User,
        kind: ServiceKind::Daemon,
    },
    ServiceSpec {
        unit: "mechrevo-turbo.service",
        title: "狂暴子模式联合调度守护",
        summary: "负责静音狂暴 / 高能狂暴功耗租约调度，拔电或休眠前自动交还固件控制",
        binary: "mechrevo-turbod",
        scope: ServiceScope::System,
        kind: ServiceKind::Daemon,
    },
    ServiceSpec {
        unit: "mechrevo-profile-guard.service",
        title: "EC 固件安全守护与遥测",
        summary: "监听物理模式键 (0xB0) 与 WMI/EC 遥测，守护高温风扇基线并异常回退调校",
        binary: "mechrevo-profile-guardd",
        scope: ServiceScope::System,
        kind: ServiceKind::Daemon,
    },
    ServiceSpec {
        unit: "mechrevo-acpi-baseline.service",
        title: "开机 ACPI 硬件基线校验",
        summary: "开机核对主板型号与固件基线，防止不兼容硬件误写寄存器",
        binary: "mechrevo-acpi-check",
        scope: ServiceScope::System,
        kind: ServiceKind::Hook,
    },
    ServiceSpec {
        unit: "mechrevo-profile-sleep.service",
        title: "睡眠前自定义调校回退钩子",
        summary: "系统休眠/挂起前自动安全撤销自定义功耗与风扇曲线",
        binary: "mechrevo-profile-helper",
        scope: ServiceScope::System,
        kind: ServiceKind::Hook,
    },
    ServiceSpec {
        unit: "mechrevo-turbo-sleep.service",
        title: "睡眠前狂暴租约清理钩子",
        summary: "系统休眠/挂起前强制释放狂暴联合调度租约，保障唤醒稳定性",
        binary: "mechrevo-turbo-lifecycle",
        scope: ServiceScope::System,
        kind: ServiceKind::Hook,
    },
    ServiceSpec {
        unit: "mechrevo-powerd-sleep.service",
        title: "NVIDIA 休眠功耗联动钩子",
        summary: "挂起与唤醒阶段协同管理 nvidia-powerd，避免独显功耗异常",
        binary: "mechrevo-powerd-sleep",
        scope: ServiceScope::System,
        kind: ServiceKind::Hook,
    },
];

const SHOW_PROPS: &str = "Id,LoadState,ActiveState,SubState,UnitFileState,Type,MainPID,MemoryCurrent,ActiveEnterTimestamp,NRestarts,Description,FragmentPath";

#[derive(Serialize)]
pub struct ServiceStatusItem {
    pub unit: String,
    pub title: String,
    pub summary: String,
    pub binary: String,
    pub scope: &'static str,
    pub kind: &'static str,
    pub installed: bool,
    pub load_state: String,
    pub active_state: String,
    pub sub_state: String,
    pub enabled_state: String,
    pub enabled: bool,
    pub running: bool,
    pub main_pid: u32,
    pub memory_bytes: Option<u64>,
    pub restarts: u32,
    pub active_since: String,
    pub unit_type: String,
    pub fragment_path: String,
}

fn find_spec(unit: &str) -> Option<&'static ServiceSpec> {
    MANAGED_SERVICES.iter().find(|s| s.unit == unit)
}

fn parse_systemctl_show_blocks(stdout: &str) -> HashMap<String, HashMap<String, String>> {
    let mut by_id: HashMap<String, HashMap<String, String>> = HashMap::new();
    let mut current: HashMap<String, String> = HashMap::new();

    for line in stdout.lines() {
        let trimmed = line.trim_end();
        if trimmed.is_empty() {
            if let Some(id) = current.get("Id").cloned() {
                by_id.insert(id, current);
            }
            current = HashMap::new();
            continue;
        }
        if let Some((k, v)) = trimmed.split_once('=') {
            current.insert(k.to_string(), v.to_string());
        }
    }
    if let Some(id) = current.get("Id").cloned() {
        by_id.insert(id, current);
    }
    by_id
}

fn parse_u64_opt(val: Option<&String>) -> Option<u64> {
    let raw = val?.trim();
    if raw.is_empty() || raw == "[not set]" || raw == "infinity" {
        return None;
    }
    let parsed = raw.parse::<u64>().ok()?;
    if parsed == u64::MAX {
        None
    } else {
        Some(parsed)
    }
}

fn build_item(spec: &ServiceSpec, props: Option<&HashMap<String, String>>) -> ServiceStatusItem {
    let get = |k: &str| {
        props
            .and_then(|m| m.get(k))
            .map(|s| s.trim().to_string())
            .unwrap_or_default()
    };
    let load_state = get("LoadState");
    let active_state = get("ActiveState");
    let sub_state = get("SubState");
    let enabled_state = get("UnitFileState");
    let installed = !load_state.is_empty() && load_state != "not-found";
    let running = active_state == "active" && matches!(sub_state.as_str(), "running" | "exited");
    let enabled = matches!(enabled_state.as_str(), "enabled" | "enabled-runtime" | "static");
    let main_pid = get("MainPID").parse::<u32>().unwrap_or(0);
    let restarts = get("NRestarts").parse::<u32>().unwrap_or(0);
    let memory_bytes = parse_u64_opt(props.and_then(|m| m.get("MemoryCurrent")));

    ServiceStatusItem {
        unit: spec.unit.to_string(),
        title: spec.title.to_string(),
        summary: spec.summary.to_string(),
        binary: spec.binary.to_string(),
        scope: match spec.scope {
            ServiceScope::User => "user",
            ServiceScope::System => "system",
        },
        kind: match spec.kind {
            ServiceKind::Daemon => "daemon",
            ServiceKind::Hook => "hook",
        },
        installed,
        load_state: if load_state.is_empty() {
            "unknown".to_string()
        } else {
            load_state
        },
        active_state: if active_state.is_empty() {
            "unknown".to_string()
        } else {
            active_state
        },
        sub_state: if sub_state.is_empty() {
            "unknown".to_string()
        } else {
            sub_state
        },
        enabled_state: if enabled_state.is_empty() {
            "disabled".to_string()
        } else {
            enabled_state
        },
        enabled,
        running,
        main_pid,
        memory_bytes,
        restarts,
        active_since: get("ActiveEnterTimestamp"),
        unit_type: get("Type"),
        fragment_path: get("FragmentPath"),
    }
}

async fn query_scope_props(
    scope: ServiceScope,
    units: &[&str],
) -> Result<HashMap<String, HashMap<String, String>>, String> {
    if units.is_empty() {
        return Ok(HashMap::new());
    }
    let mut cmd = Command::new(SYSTEMCTL);
    if scope == ServiceScope::User {
        cmd.arg("--user");
    }
    cmd.arg("show")
        .arg(format!("--property={SHOW_PROPS}"))
        .arg("--no-pager")
        .args(units)
        .stdin(Stdio::null())
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .kill_on_drop(true);

    let out = tokio::time::timeout(Duration::from_secs(4), cmd.output())
        .await
        .map_err(|_| "查询 systemd 服务状态超时 (4s)".to_string())?
        .map_err(|e| format!("无法调用 systemctl: {e}"))?;
    if !out.status.success() {
        let stderr = String::from_utf8_lossy(&out.stderr).trim().to_string();
        return Err(if stderr.is_empty() {
            format!("systemctl show 返回错误码 {}", out.status.code().unwrap_or(-1))
        } else {
            stderr
        });
    }
    let stdout = String::from_utf8_lossy(&out.stdout);
    Ok(parse_systemctl_show_blocks(&stdout))
}

/// Live active status of the 4 core background daemons, sourced directly from systemd.
pub async fn check_core_daemons_active() -> (bool, bool, bool, bool) {
    let user_map = query_scope_props(
        ServiceScope::User,
        &[
            "iteglow-watercooling.service",
            "iteglow-session-restore.service",
        ],
    )
    .await
    .unwrap_or_default();
    let sys_map = query_scope_props(
        ServiceScope::System,
        &[
            "mechrevo-turbo.service",
            "mechrevo-profile-guard.service",
        ],
    )
    .await
    .unwrap_or_default();

    let is_active = |map: &HashMap<String, HashMap<String, String>>, unit: &str| {
        map.get(unit)
            .and_then(|p| p.get("ActiveState"))
            .map(|s| s.trim() == "active")
            .unwrap_or(false)
    };
    (
        is_active(&user_map, "iteglow-watercooling.service"),
        is_active(&user_map, "iteglow-session-restore.service"),
        is_active(&sys_map, "mechrevo-turbo.service"),
        is_active(&sys_map, "mechrevo-profile-guard.service"),
    )
}

#[tauri::command]
pub async fn list_services() -> Result<Vec<ServiceStatusItem>, String> {
    let user_units: Vec<&str> = MANAGED_SERVICES
        .iter()
        .filter(|s| s.scope == ServiceScope::User)
        .map(|s| s.unit)
        .collect();
    let sys_units: Vec<&str> = MANAGED_SERVICES
        .iter()
        .filter(|s| s.scope == ServiceScope::System)
        .map(|s| s.unit)
        .collect();

    let user_map = query_scope_props(ServiceScope::User, &user_units).await?;
    let sys_map = query_scope_props(ServiceScope::System, &sys_units).await?;

    let items = MANAGED_SERVICES
        .iter()
        .map(|spec| {
            let map = match spec.scope {
                ServiceScope::User => &user_map,
                ServiceScope::System => &sys_map,
            };
            build_item(spec, map.get(spec.unit))
        })
        .collect();

    Ok(items)
}

fn validate_action(spec: &ServiceSpec, action: &str) -> Result<(), String> {
    match spec.kind {
        ServiceKind::Daemon => {
            if matches!(action, "start" | "stop" | "restart" | "enable" | "disable") {
                Ok(())
            } else {
                Err(format!("不支持的操作：{action}"))
            }
        }
        ServiceKind::Hook => {
            if matches!(action, "enable" | "disable") {
                Ok(())
            } else {
                Err("安全/休眠钩子仅支持启用或禁用自启，禁止在桌面会话中直接手动触发".to_string())
            }
        }
    }
}

#[tauri::command]
pub async fn control_service(unit: String, action: String) -> Result<Value, String> {
    let spec = find_spec(&unit).ok_or_else(|| format!("未受管的服务单元：{unit}"))?;
    validate_action(spec, &action)?;

    let (mut cmd, timeout_secs) = match spec.scope {
        ServiceScope::User => {
            let mut c = Command::new(SYSTEMCTL);
            c.args(["--user", &action, spec.unit]);
            (c, 12u64)
        }
        ServiceScope::System => {
            let mut c = Command::new(PKEXEC);
            c.args(["--disable-internal-agent", SYSTEMCTL, &action, spec.unit]);
            (c, 60u64)
        }
    };

    cmd.stdin(Stdio::null())
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .kill_on_drop(true);

    let output = tokio::time::timeout(Duration::from_secs(timeout_secs), cmd.output())
        .await
        .map_err(|_| format!("执行服务操作超时 ({timeout_secs}s)"))?
        .map_err(|e| format!("无法执行服务控制命令：{e}"))?;

    if !output.status.success() {
        let stderr = String::from_utf8_lossy(&output.stderr).trim().to_string();
        let stdout = String::from_utf8_lossy(&output.stdout).trim().to_string();
        let code = output.status.code().unwrap_or(-1);
        if spec.scope == ServiceScope::System
            && (code == 126
                || (code == 127 && stderr.to_ascii_lowercase().contains("not authorized"))
                || stderr.to_ascii_lowercase().contains("dismissed"))
        {
            return Err("管理员授权已取消，未修改系统服务".to_string());
        }
        let detail = if !stderr.is_empty() {
            stderr
        } else if !stdout.is_empty() {
            stdout
        } else {
            format!("systemctl 返回错误码 {code}")
        };
        return Err(detail);
    }

    // Re-read fresh status for the controlled unit immediately
    let fresh_map = query_scope_props(spec.scope, &[spec.unit])
        .await
        .unwrap_or_default();
    let item = build_item(spec, fresh_map.get(spec.unit));
    Ok(json!({
        "ok": true,
        "service": item,
    }))
}

const MAX_LOG_BYTES: usize = 512 * 1024;

#[tauri::command]
pub async fn get_service_logs(unit: String, lines: Option<u32>) -> Result<Value, String> {
    let spec = find_spec(&unit).ok_or_else(|| format!("未受管的服务单元：{unit}"))?;
    let count = lines.unwrap_or(60).clamp(10, 200);
    let count_str = count.to_string();

    let mut cmd = Command::new(JOURNALCTL);
    if spec.scope == ServiceScope::User {
        cmd.arg("--user");
    }
    cmd.args([
        "-u",
        spec.unit,
        "-n",
        &count_str,
        "--no-pager",
        "--output=short-iso",
    ])
    .stdin(Stdio::null())
    .stdout(Stdio::piped())
    .stderr(Stdio::piped())
    .kill_on_drop(true);

    let output = tokio::time::timeout(Duration::from_secs(5), cmd.output())
        .await
        .map_err(|_| "读取日志超时 (5s)".to_string())?
        .map_err(|e| format!("无法调用 journalctl：{e}"))?;

    let raw_bytes = if output.stdout.len() > MAX_LOG_BYTES {
        &output.stdout[output.stdout.len() - MAX_LOG_BYTES..]
    } else {
        &output.stdout[..]
    };
    let stdout = String::from_utf8_lossy(raw_bytes).to_string();
    let stderr = String::from_utf8_lossy(&output.stderr).trim().to_string();
    let log_lines: Vec<String> = stdout.lines().map(|l| l.to_string()).collect();

    Ok(json!({
        "unit": spec.unit,
        "title": spec.title,
        "lines": log_lines,
        "warning": if !stderr.is_empty() { Some(stderr) } else { None::<String> },
    }))
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parses_multi_unit_systemctl_show_output() {
        let sample = "\
Id=iteglow-watercooling.service
LoadState=loaded
ActiveState=active
SubState=running
UnitFileState=enabled
MainPID=7134
MemoryCurrent=2203648
NRestarts=0

Id=iteglow-session-restore.service
LoadState=loaded
ActiveState=inactive
SubState=dead
UnitFileState=disabled
MainPID=0
MemoryCurrent=[not set]
NRestarts=2
";
        let map = parse_systemctl_show_blocks(sample);
        assert_eq!(map.len(), 2);
        let wc = build_item(find_spec("iteglow-watercooling.service").unwrap(), map.get("iteglow-watercooling.service"));
        assert!(wc.installed);
        assert!(wc.running);
        assert!(wc.enabled);
        assert_eq!(wc.main_pid, 7134);
        assert_eq!(wc.memory_bytes, Some(2203648));

        let sess = build_item(find_spec("iteglow-session-restore.service").unwrap(), map.get("iteglow-session-restore.service"));
        assert!(sess.installed);
        assert!(!sess.running);
        assert!(!sess.enabled);
        assert_eq!(sess.memory_bytes, None);
        assert_eq!(sess.restarts, 2);
    }

    #[test]
    fn enforces_strict_allowlist_and_hook_safety() {
        assert!(find_spec("sshd.service").is_none());
        let daemon = find_spec("mechrevo-turbo.service").unwrap();
        assert!(validate_action(daemon, "restart").is_ok());
        assert!(validate_action(daemon, "mask").is_err());

        let hook = find_spec("mechrevo-profile-sleep.service").unwrap();
        assert!(validate_action(hook, "enable").is_ok());
        assert!(validate_action(hook, "disable").is_ok());
        assert!(validate_action(hook, "start").is_err());
        assert!(validate_action(hook, "restart").is_err());
    }
}

// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Unprivileged async Rust lighting session daemon for MECHREVO Linux Control.
//!
//! Drop-in replacement for `python3 -m iteglow.lighting_session [--apply] [--once]`:
//! - Default operation is dry-run; `--apply` enables HID + Logo + Profile writes
//! - Single-instance lock `$XDG_RUNTIME_DIR/iteglow-session-restore.lock`
//! - Cooperating per-zone locks `$XDG_RUNTIME_DIR/iteglow-{keyboard,lightbar,logo,manual-state}.lock`
//! - Binds once to the single active local graphical logind session (x11/wayland)
//! - Watches `PrepareForSleep`, `PrepareForShutdown`, and `SessionRemoved`
//! - Coordinates startup/resume restore, AC/DC & idle lighting policy, WMI 0xC1
//!   hotkey brightness sync, and opt-in app-triggered profile switching.

mod hotkey;
mod policy;
mod profiles;
mod protocol;
mod zone_lock;

use crate::hotkey::{check_eligible_session_props, KeyboardHotkeySync, Lifecycle};
use crate::policy::{read_manual, read_policy_config, PolicyRunner};
use crate::profiles::ProfileRuleRunner;
use crate::protocol::{apply_action_locked, load_actions, query_keyboard_hw_locked, RestoreAction};
use crate::zone_lock::{
    acquire_session_watcher_lock, default_config_dir, default_runtime_dir, default_state_dir,
    ZoneGuard,
};
use futures_util::StreamExt;
use serde_json::{json, Value};
use std::collections::HashMap;
use std::io::Write;
use std::path::Path;
use std::time::Duration;
use tokio::signal::unix::{signal, SignalKind};
use tokio::sync::mpsc;
use tokio::time::{interval, Instant};
use zbus::zvariant::{OwnedObjectPath, OwnedValue, Value as ZValue};
use zbus::{Connection, MatchRule, MessageStream};

const VERSION: &str = env!("CARGO_PKG_VERSION");
const DBUS_TIMEOUT: Duration = Duration::from_millis(2500);

fn emit_json(val: &Value) {
    let mut out = std::io::stdout().lock();
    let _ = writeln!(out, "{val}");
    let _ = out.flush();
}

// ----------------------------------------------------------------- D-Bus session helpers

async fn get_session_props(
    system_bus: &Connection,
    session_path: &str,
) -> Result<HashMap<String, OwnedValue>, String> {
    let reply = tokio::time::timeout(
        DBUS_TIMEOUT,
        system_bus.call_method(
            Some("org.freedesktop.login1"),
            session_path,
            Some("org.freedesktop.DBus.Properties"),
            "GetAll",
            &("org.freedesktop.login1.Session",),
        ),
    )
    .await
    .map_err(|_| "login1 GetAll timed out".to_string())?
    .map_err(|e| format!("login1 GetAll failed: {e}"))?;

    reply
        .body()
        .deserialize::<HashMap<String, OwnedValue>>()
        .map_err(|e| format!("login1 GetAll decode failed: {e}"))
}

fn is_eligible_session(props: &HashMap<String, OwnedValue>, uid: u32) -> bool {
    let user_uid = props.get("User").and_then(|v| {
        let zv: &ZValue = v;
        if let ZValue::Structure(s) = zv {
            s.fields().first().and_then(|f| u32::try_from(f).ok())
        } else {
            None
        }
    });
    let seat_id = props.get("Seat").and_then(|v| {
        let zv: &ZValue = v;
        if let ZValue::Structure(s) = zv {
            s.fields().first().and_then(|f| <&str>::try_from(f).ok())
        } else {
            None
        }
    });
    let active = props
        .get("Active")
        .and_then(|v| bool::try_from(&**v).ok());
    let remote = props
        .get("Remote")
        .and_then(|v| bool::try_from(&**v).ok());
    let state = props
        .get("State")
        .and_then(|v| <&str>::try_from(&**v).ok());
    let class = props
        .get("Class")
        .and_then(|v| <&str>::try_from(&**v).ok());
    let session_type = props
        .get("Type")
        .and_then(|v| <&str>::try_from(&**v).ok());

    check_eligible_session_props(user_uid, seat_id, active, remote, state, class, session_type, uid)
}

async fn find_unique_session(
    system_bus: &Connection,
    uid: u32,
) -> Result<Option<(String, String)>, String> {
    let reply = tokio::time::timeout(
        DBUS_TIMEOUT,
        system_bus.call_method(
            Some("org.freedesktop.login1"),
            "/org/freedesktop/login1",
            Some("org.freedesktop.login1.Manager"),
            "ListSessions",
            &(),
        ),
    )
    .await
    .map_err(|_| "login1 ListSessions timed out".to_string())?
    .map_err(|e| format!("login1 ListSessions failed: {e}"))?;

    let sessions: Vec<(String, u32, String, String, OwnedObjectPath)> = reply
        .body()
        .deserialize()
        .map_err(|e| format!("ListSessions decode failed: {e}"))?;

    let mut candidates = Vec::new();
    for (sid, owner, _name, _seat, path) in sessions {
        if owner != uid {
            continue;
        }
        let path_str = path.as_str().to_string();
        if let Ok(props) = get_session_props(system_bus, &path_str).await {
            if is_eligible_session(&props, uid) {
                candidates.push((sid, path_str));
            }
        }
    }
    if candidates.len() != 1 {
        return Ok(None);
    }
    Ok(candidates.into_iter().next())
}

async fn is_session_active(
    system_bus: &Connection,
    user_bus: &Connection,
    session_path: &str,
    uid: u32,
) -> bool {
    let Ok(props) = get_session_props(system_bus, session_path).await else {
        return false;
    };
    if !is_eligible_session(&props, uid) {
        return false;
    }
    let Ok(Ok(unit_reply)) = tokio::time::timeout(
        DBUS_TIMEOUT,
        user_bus.call_method(
            Some("org.freedesktop.systemd1"),
            "/org/freedesktop/systemd1",
            Some("org.freedesktop.systemd1.Manager"),
            "GetUnit",
            &("graphical-session.target",),
        ),
    )
    .await
    else {
        return false;
    };
    let Ok(unit_path) = unit_reply.body().deserialize::<OwnedObjectPath>() else {
        return false;
    };
    let Ok(Ok(state_reply)) = tokio::time::timeout(
        DBUS_TIMEOUT,
        user_bus.call_method(
            Some("org.freedesktop.systemd1"),
            unit_path.as_str(),
            Some("org.freedesktop.DBus.Properties"),
            "Get",
            &("org.freedesktop.systemd1.Unit", "ActiveState"),
        ),
    )
    .await
    else {
        return false;
    };
    let Ok(val) = state_reply.body().deserialize::<OwnedValue>() else {
        return false;
    };
    <&str>::try_from(&*val).ok() == Some("active")
}

async fn sample_power(system_bus: &Connection) -> Result<String, String> {
    let reply = tokio::time::timeout(
        DBUS_TIMEOUT,
        system_bus.call_method(
            Some("org.freedesktop.UPower"),
            "/org/freedesktop/UPower",
            Some("org.freedesktop.DBus.Properties"),
            "Get",
            &("org.freedesktop.UPower", "OnBattery"),
        ),
    )
    .await
    .map_err(|_| "UPower timed out".to_string())?
    .map_err(|e| format!("UPower failed: {e}"))?;

    let val: OwnedValue = reply
        .body()
        .deserialize()
        .map_err(|e| format!("UPower decode failed: {e}"))?;
    let on_battery = bool::try_from(&*val)
        .map_err(|_| "UPower OnBattery is not boolean".to_string())?;
    Ok(if on_battery { "dc" } else { "ac" }.to_string())
}

async fn sample_idle(user_bus: &Connection) -> Result<f64, String> {
    let reply = tokio::time::timeout(
        DBUS_TIMEOUT,
        user_bus.call_method(
            Some("org.gnome.Mutter.IdleMonitor"),
            "/org/gnome/Mutter/IdleMonitor/Core",
            Some("org.gnome.Mutter.IdleMonitor"),
            "GetIdletime",
            &(),
        ),
    )
    .await
    .map_err(|_| "Mutter GetIdletime timed out".to_string())?
    .map_err(|e| format!("Mutter GetIdletime failed: {e}"))?;

    let ms: u64 = reply
        .body()
        .deserialize()
        .map_err(|_| "Mutter GetIdletime is not uint64 milliseconds".to_string())?;
    Ok((ms as f64) / 1000.0)
}

#[derive(Debug)]
enum LogindEvent {
    PrepareForSleep(bool),
    PrepareForShutdown,
    SessionRemoved(String),
}

async fn spawn_logind_watcher(
    conn: Connection,
    tx: mpsc::UnboundedSender<LogindEvent>,
) -> Result<tokio::task::AbortHandle, String> {
    let rule = MatchRule::builder()
        .msg_type(zbus::message::Type::Signal)
        .interface("org.freedesktop.login1.Manager")
        .map_err(|e| e.to_string())?
        .path("/org/freedesktop/login1")
        .map_err(|e| e.to_string())?
        .build();
    let mut stream = MessageStream::for_match_rule(rule, &conn, Some(16))
        .await
        .map_err(|e| format!("subscribe logind signals failed: {e}"))?;

    let handle = tokio::spawn(async move {
        while let Some(item) = stream.next().await {
            let Ok(msg) = item else {
                continue;
            };
            let member = msg
                .header()
                .member()
                .map(|m| m.as_str().to_string())
                .unwrap_or_default();
            match member.as_str() {
                "PrepareForSleep" => {
                    if let Ok((entering,)) = msg.body().deserialize::<(bool,)>() {
                        let _ = tx.send(LogindEvent::PrepareForSleep(entering));
                    }
                }
                "PrepareForShutdown" => {
                    if let Ok((entering,)) = msg.body().deserialize::<(bool,)>() {
                        if entering {
                            let _ = tx.send(LogindEvent::PrepareForShutdown);
                        }
                    }
                }
                "SessionRemoved" => {
                    if let Ok((sid, _path)) = msg.body().deserialize::<(String, OwnedObjectPath)>()
                    {
                        let _ = tx.send(LogindEvent::SessionRemoved(sid));
                    }
                }
                _ => {}
            }
        }
    });
    Ok(handle.abort_handle())
}

// ----------------------------------------------------------------- Restore helper

async fn restore_once<FActive, FutActive>(
    directory: &Path,
    uid: u32,
    apply: bool,
    mut is_active: FActive,
) -> Value
where
    FActive: FnMut() -> FutActive,
    FutActive: std::future::Future<Output = bool>,
{
    if !is_active().await {
        return json!({
            "restored": [],
            "planned": [],
            "errors": [],
            "skipped": "inactive session",
        });
    }
    let (actions, mut errors) = load_actions(directory, uid);
    let mut restored: Vec<String> = Vec::new();
    let mut planned: Vec<String> = Vec::new();
    let mut skipped: Option<&str> = None;

    for action in actions {
        if !is_active().await {
            skipped = Some("session ended or became inactive");
            break;
        }
        planned.push(action.device.clone());
        if !apply {
            continue;
        }
        let _guard = match ZoneGuard::acquire(&action.device) {
            Ok(g) => g,
            Err(e) => {
                errors.push(format!("{}: {e}", action.device));
                continue;
            }
        };
        let (latest_actions, latest_errors) = load_actions(directory, uid);
        errors.extend(latest_errors);
        let Some(latest) = latest_actions
            .into_iter()
            .find(|a| a.device == action.device)
        else {
            continue;
        };
        if !is_active().await {
            skipped = Some("session ended or became inactive");
            break;
        }
        match apply_action_locked(&latest).await {
            Ok(()) => restored.push(action.device),
            Err(e) => errors.push(format!("{}: {e}", action.device)),
        }
    }

    let mut out = json!({
        "restored": restored,
        "planned": planned,
        "errors": errors,
    });
    if let Some(s) = skipped {
        out["skipped"] = json!(s);
    }
    out
}

// ----------------------------------------------------------------- Main daemon

struct CliArgs {
    apply: bool,
    once: bool,
    show_version: bool,
    show_help: bool,
}

fn parse_args() -> Result<CliArgs, String> {
    let mut cfg = CliArgs {
        apply: false,
        once: false,
        show_version: false,
        show_help: false,
    };
    for arg in std::env::args().skip(1) {
        match arg.as_str() {
            "--apply" => cfg.apply = true,
            "--once" => cfg.once = true,
            "--version" | "-V" => cfg.show_version = true,
            "--help" | "-h" => cfg.show_help = true,
            other => return Err(format!("unrecognized argument: {other}")),
        }
    }
    Ok(cfg)
}

#[tokio::main(flavor = "current_thread")]
async fn main() {
    let args = match parse_args() {
        Ok(a) => a,
        Err(e) => {
            eprintln!("error: {e}");
            std::process::exit(2);
        }
    };
    if args.show_version {
        println!("iteglow-sessiond {VERSION}");
        return;
    }
    if args.show_help {
        println!("iteglow-sessiond {VERSION}");
        println!("Restore saved lighting and run lighting policy / hotkey sync / profile rules.");
        println!("Usage: iteglow-sessiond [--apply] [--once] [--version]");
        return;
    }

    if let Err(e) = run(args).await {
        eprintln!("error: {e}");
        std::process::exit(1);
    }
}

async fn run(args: CliArgs) -> Result<(), String> {
    let _lock_file = if !args.once {
        let rdir = std::env::var("XDG_RUNTIME_DIR")
            .ok()
            .filter(|s| !s.trim().is_empty())
            .map(std::path::PathBuf::from)
            .ok_or_else(|| "XDG_RUNTIME_DIR is required for a session watcher".to_string())?;
        match acquire_session_watcher_lock(&rdir)? {
            Some(f) => Some(f),
            None => {
                emit_json(&json!({"skipped": "lighting session watcher already running"}));
                return Ok(());
            }
        }
    } else {
        None
    };

    let uid = unsafe { libc::getuid() };
    let system_bus = Connection::system()
        .await
        .map_err(|e| format!("connect system D-Bus failed: {e}"))?;
    let user_bus = Connection::session()
        .await
        .map_err(|e| format!("connect session D-Bus failed: {e}"))?;

    let Some((session_id, session_path)) = find_unique_session(&system_bus, uid).await? else {
        emit_json(&json!({"skipped": "exactly one active local graphical session is required"}));
        return Ok(());
    };

    let state_dir = default_state_dir();
    let cfg_dir = default_config_dir();
    let policy_cfg_path = cfg_dir.join("lighting-policy.json");
    let profiles_cfg_path = cfg_dir.join("profiles.json");

    if args.once {
        let res = restore_once(&state_dir, uid, args.apply, || {
            is_session_active(&system_bus, &user_bus, &session_path, uid)
        })
        .await;
        emit_json(&res);
        return Ok(());
    }

    let mut lifecycle = Lifecycle::default();
    let mut policy = PolicyRunner::new(args.apply);
    let mut rule_runner = ProfileRuleRunner::new(args.apply);
    let mut hotkey_sync = KeyboardHotkeySync::new(state_dir.clone(), uid);
    let mut restore_deadline: Option<Instant> = None;
    let mut last_active = false;
    let boot_epoch = Instant::now();

    let (logind_tx, mut logind_rx) = mpsc::unbounded_channel::<LogindEvent>();
    let logind_abort = spawn_logind_watcher(system_bus.clone(), logind_tx).await?;

    let mut sigterm = signal(SignalKind::terminate()).map_err(|e| e.to_string())?;
    let mut sigint = signal(SignalKind::interrupt()).map_err(|e| e.to_string())?;

    // Schedule startup restoration if session is currently active.
    let initial_active = is_session_active(&system_bus, &user_bus, &session_path, uid).await;
    if lifecycle.startup(initial_active) {
        policy.begin_restore();
        rule_runner.deactivate();
        hotkey_sync.reset();
        last_active = true;
        restore_deadline = Some(Instant::now() + Duration::from_secs(2));
    }

    let mut policy_ticker = interval(Duration::from_secs(5));
    policy_ticker.set_missed_tick_behavior(tokio::time::MissedTickBehavior::Skip);
    policy_ticker.tick().await; // consume immediate tick; first poll runs after 5s

    let mut hotkey_ticker = interval(Duration::from_secs(1));
    hotkey_ticker.set_missed_tick_behavior(tokio::time::MissedTickBehavior::Skip);
    hotkey_ticker.tick().await; // consume immediate tick

    emit_json(&json!({
        "event": "lighting_session_daemon_started",
        "backend": "rust",
        "version": VERSION,
        "apply": args.apply,
        "session_id": session_id,
        "runtime_dir": default_runtime_dir().display().to_string(),
    }));

    loop {
        let restore_fut = async {
            if let Some(deadline) = restore_deadline {
                tokio::time::sleep_until(deadline).await;
            } else {
                std::future::pending::<()>().await;
            }
        };

        tokio::select! {
            _ = sigterm.recv() => {
                break;
            }
            _ = sigint.recv() => {
                break;
            }
            Some(ev) = logind_rx.recv() => {
                match ev {
                    LogindEvent::PrepareForSleep(entering) => {
                        let active_now = is_session_active(&system_bus, &user_bus, &session_path, uid).await;
                        let (do_cancel, do_schedule) = lifecycle.prepare_for_sleep(entering, active_now);
                        if do_cancel {
                            policy.begin_restore();
                            rule_runner.deactivate();
                            hotkey_sync.reset();
                            last_active = false;
                            restore_deadline = None;
                        }
                        if do_schedule {
                            policy.begin_restore();
                            rule_runner.deactivate();
                            hotkey_sync.reset();
                            last_active = true;
                            restore_deadline = Some(Instant::now() + Duration::from_secs(2));
                        }
                    }
                    LogindEvent::PrepareForShutdown => {
                        lifecycle.stop();
                        policy.begin_restore();
                        rule_runner.deactivate();
                        hotkey_sync.reset();
                        restore_deadline = None;
                    }
                    LogindEvent::SessionRemoved(removed_sid) => {
                        if removed_sid == session_id {
                            lifecycle.stop();
                            break;
                        }
                    }
                }
            }
            _ = restore_fut => {
                restore_deadline = None;
                rule_runner.deactivate();
                policy.begin_restore();
                let restore_res = restore_once(&state_dir, uid, args.apply, || async {
                    let act = is_session_active(&system_bus, &user_bus, &session_path, uid).await;
                    lifecycle.can_restore(act)
                }).await;
                emit_json(&restore_res);
                policy.finish_restore();

                run_policy_tick(
                    &mut policy,
                    &lifecycle,
                    &mut hotkey_sync,
                    &system_bus,
                    &user_bus,
                    &session_path,
                    &policy_cfg_path,
                    &state_dir,
                    uid,
                ).await;

                if args.apply {
                    let act = is_session_active(&system_bus, &user_bus, &session_path, uid).await;
                    if lifecycle.can_restore(act) {
                        hotkey_sync.record_applied_hw();
                    }
                }
            }
            _ = hotkey_ticker.tick() => {
                if args.apply && restore_deadline.is_none() && !rule_runner.owns_lighting {
                    let act = is_session_active(&system_bus, &user_bus, &session_path, uid).await;
                    if lifecycle.can_restore(act) {
                        if let Some(ev) = hotkey_sync.sync_if_needed_hw(false) {
                            emit_json(&ev);
                        }
                    }
                }
            }
            _ = policy_ticker.tick() => {
                let act = is_session_active(&system_bus, &user_bus, &session_path, uid).await;
                let current = lifecycle.can_restore(act);
                if !current {
                    policy.deactivate();
                    rule_runner.deactivate();
                    hotkey_sync.reset();
                    last_active = false;
                } else if !last_active {
                    policy.begin_restore();
                    rule_runner.deactivate();
                    hotkey_sync.reset();
                    last_active = true;
                    restore_deadline = Some(Instant::now() + Duration::from_secs(2));
                }

                if restore_deadline.is_none() {
                    let now_secs = boot_epoch.elapsed().as_secs_f64();
                    let rule_res = rule_runner
                        .tick(
                            current,
                            &profiles_cfg_path,
                            &state_dir,
                            uid,
                            now_secs,
                            &mut |ev| emit_json(&ev),
                        )
                        .await;
                    if rule_res.active_lighting {
                        policy.begin_restore();
                    } else if rule_res.released_lighting {
                        policy.finish_restore();
                    }
                    if args.apply
                        && matches!(
                            rule_res.action.as_str(),
                            "apply" | "release" | "yield_mode_key"
                        )
                    {
                        hotkey_sync.record_applied_hw();
                    }
                }

                if args.apply && restore_deadline.is_none() && !rule_runner.owns_lighting {
                    let act_now = is_session_active(&system_bus, &user_bus, &session_path, uid).await;
                    if lifecycle.can_restore(act_now) {
                        if let Some(ev) = hotkey_sync.sync_if_needed_hw(true) {
                            emit_json(&ev);
                        }
                    }
                }

                run_policy_tick(
                    &mut policy,
                    &lifecycle,
                    &mut hotkey_sync,
                    &system_bus,
                    &user_bus,
                    &session_path,
                    &policy_cfg_path,
                    &state_dir,
                    uid,
                ).await;
            }
        }
    }

    lifecycle.stop();
    policy.deactivate();
    rule_runner.deactivate();
    logind_abort.abort();
    Ok(())
}

#[allow(clippy::too_many_arguments)]
async fn run_policy_tick(
    policy: &mut PolicyRunner,
    lifecycle: &Lifecycle,
    hotkey_sync: &mut KeyboardHotkeySync,
    system_bus: &Connection,
    user_bus: &Connection,
    session_path: &str,
    policy_cfg_path: &Path,
    state_dir: &Path,
    uid: u32,
) {
    let mut wrote_kbd = false;
    policy
        .tick(
            || async {
                let act = is_session_active(system_bus, user_bus, session_path, uid).await;
                Ok(lifecycle.can_restore(act))
            },
            || read_policy_config(policy_cfg_path, uid),
            || read_manual(state_dir, uid),
            || sample_power(system_bus),
            || sample_idle(user_bus),
            |action: RestoreAction| {
                let is_kbd = action.device == "keyboard";
                async move {
                    apply_action_locked(&action).await?;
                    if is_kbd {
                        // Record happens immediately after lock/write via outer flag.
                    }
                    Ok(())
                }
            },
            true,
            &mut |ev| {
                if let Some(applied) = ev.get("applied").and_then(|v| v.as_array()) {
                    if applied.iter().any(|x| x.as_str() == Some("keyboard")) {
                        wrote_kbd = true;
                    }
                }
                emit_json(&ev);
            },
        )
        .await;

    if wrote_kbd {
        if let Ok(_guard) = ZoneGuard::acquire("keyboard") {
            hotkey_sync.record_applied_locked(|| query_keyboard_hw_locked().ok());
        }
    }
}

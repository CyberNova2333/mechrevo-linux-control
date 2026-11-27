// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Unprivileged async Rust BLE water-cooling daemon for MECHREVO LCT22002 / LCT21001.
//!
//! Drop-in replacement for `python3 -m iteglow.cli cooler daemon`:
//! - Same AF_UNIX socket (`/run/user/$UID/iteglow-watercooling.sock`, 0600, SO_PEERCRED)
//! - Same lock file (`/run/user/$UID/iteglow-watercooling.lock`, flock)
//! - Same JSON config (`~/.config/iteglow/watercooling.json`)
//! - Listens to systemd-logind `PrepareForSleep` / `PrepareForShutdown`
//! - Non-blocking `ping`/`status` snapshot responses even while BLE is scanning/connecting

mod ble;
mod coordinator;
mod ipc;
mod protocol;

use crate::ble::{BluezBleTransport, CoolerBleClient};
use crate::coordinator::{load_config_or_default, CoolerCoordinator, MonotonicClock};
use crate::ipc::{bind_ipc_listener, serve_ipc, IpcCommand};
use futures_util::StreamExt;
use serde_json::{json, Value};
use std::fs::{self, OpenOptions};
use std::io::Write;
use std::os::unix::fs::{MetadataExt, OpenOptionsExt, PermissionsExt};
use std::os::unix::io::AsRawFd;
use std::path::{Path, PathBuf};
use std::sync::Arc;
use std::time::Duration;
use tokio::signal::unix::{signal, SignalKind};
use tokio::sync::{mpsc, RwLock};
use tokio::time::interval;
use zbus::{Connection, MatchRule, MessageStream};

const VERSION: &str = env!("CARGO_PKG_VERSION");

fn emit_json(val: &Value) {
    let mut out = std::io::stdout().lock();
    let _ = writeln!(out, "{val}");
    let _ = out.flush();
}

fn runtime_dir() -> PathBuf {
    if let Ok(d) = std::env::var("XDG_RUNTIME_DIR") {
        if !d.trim().is_empty() {
            return PathBuf::from(d);
        }
    }
    let uid = unsafe { libc::getuid() };
    PathBuf::from(format!("/run/user/{uid}"))
}

fn verify_runtime_dir(path: &Path, my_uid: u32) -> Result<(), String> {
    if !path.exists() {
        fs::create_dir_all(path).map_err(|e| format!("创建运行时目录失败: {e}"))?;
        let _ = fs::set_permissions(path, fs::Permissions::from_mode(0o700));
    }
    let meta = fs::symlink_metadata(path).map_err(|e| format!("检查运行时目录失败: {e}"))?;
    if !meta.is_dir() || meta.uid() != my_uid || (meta.mode() & 0o022) != 0 {
        return Err(format!("不安全的运行时目录: {}", path.display()));
    }
    Ok(())
}

fn acquire_lock(lock_path: &Path) -> Result<fs::File, String> {
    let file = OpenOptions::new()
        .create(true)
        .read(true)
        .write(true)
        .mode(0o600)
        .custom_flags(libc::O_CLOEXEC | libc::O_NOFOLLOW)
        .open(lock_path)
        .map_err(|e| format!("打开锁文件失败: {e}"))?;
    let rc = unsafe { libc::flock(file.as_raw_fd(), libc::LOCK_EX | libc::LOCK_NB) };
    if rc != 0 {
        return Err("水冷守护进程已在运行".to_string());
    }
    Ok(file)
}

#[derive(Debug)]
enum LogindEvent {
    PrepareForSleep(bool),
    PrepareForShutdown,
}

async fn spawn_logind_watcher(
    conn: Connection,
    tx: mpsc::UnboundedSender<LogindEvent>,
) -> Result<tokio::task::AbortHandle, String> {
    let rule = MatchRule::builder()
        .msg_type(zbus::message::Type::Signal)
        .sender("org.freedesktop.login1")
        .map_err(|e| e.to_string())?
        .interface("org.freedesktop.login1.Manager")
        .map_err(|e| e.to_string())?
        .path("/org/freedesktop/login1")
        .map_err(|e| e.to_string())?
        .build();
    let mut stream = MessageStream::for_match_rule(rule, &conn, Some(16))
        .await
        .map_err(|e| format!("订阅 logind 睡眠信号失败: {e}"))?;

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
            if member == "PrepareForSleep" {
                if let Ok((entering,)) = msg.body().deserialize::<(bool,)>() {
                    let _ = tx.send(LogindEvent::PrepareForSleep(entering));
                }
            } else if member == "PrepareForShutdown" {
                if let Ok((entering,)) = msg.body().deserialize::<(bool,)>() {
                    if entering {
                        let _ = tx.send(LogindEvent::PrepareForShutdown);
                    }
                }
            }
        }
    });
    Ok(handle.abort_handle())
}

#[tokio::main(flavor = "current_thread")]
async fn main() {
    let args: Vec<String> = std::env::args().collect();
    if args.iter().any(|a| a == "--version" || a == "-V") {
        println!("iteglow-watercoolingd {VERSION}");
        return;
    }
    if args.iter().any(|a| a == "--help" || a == "-h") {
        println!("iteglow-watercoolingd {VERSION}");
        println!("Unprivileged async Rust BLE water-cooling daemon for MECHREVO LCT22002/LCT21001.");
        println!("Usage: iteglow-watercoolingd [--version]");
        return;
    }

    let euid = unsafe { libc::geteuid() };
    if euid == 0 {
        eprintln!("error: 水冷用户服务必须以当前桌面用户运行，请勿使用 root/sudo");
        std::process::exit(1);
    }

    if let Err(e) = run_daemon(euid).await {
        eprintln!("error: {e}");
        std::process::exit(1);
    }
}

async fn run_daemon(my_uid: u32) -> Result<(), String> {
    let rdir = runtime_dir();
    verify_runtime_dir(&rdir, my_uid)?;

    let lock_path = rdir.join("iteglow-watercooling.lock");
    let sock_path = rdir.join("iteglow-watercooling.sock");
    let _lock_file = acquire_lock(&lock_path)?;

    let listener = bind_ipc_listener(&sock_path)?;
    let system_bus = Connection::system()
        .await
        .map_err(|e| format!("连接系统 D-Bus 失败: {e}"))?;

    let cfg_path = protocol::config_path();
    let cfg = load_config_or_default(&cfg_path);
    let shared_snapshot = Arc::new(RwLock::new(
        CoolerCoordinator::<BluezBleTransport>::default_snapshot(&cfg),
    ));

    let logger: Arc<dyn Fn(Value) + Send + Sync> = Arc::new(|v| emit_json(&v));
    let transport = BluezBleTransport::new(system_bus.clone());
    let client = CoolerBleClient::new(transport, Duration::from_millis(100));
    let clock = Arc::new(MonotonicClock::new());
    let mut coordinator = CoolerCoordinator::new(
        client,
        cfg,
        cfg_path,
        clock,
        shared_snapshot.clone(),
        Some(logger),
    );
    coordinator.publish_snapshot().await;

    let (cmd_tx, mut cmd_rx) = mpsc::channel::<IpcCommand>(32);
    let ipc_task = tokio::spawn(serve_ipc(listener, my_uid, cmd_tx, shared_snapshot.clone()));

    let (logind_tx, mut logind_rx) = mpsc::unbounded_channel::<LogindEvent>();
    let logind_abort = spawn_logind_watcher(system_bus, logind_tx).await?;

    let mut sigterm = signal(SignalKind::terminate()).map_err(|e| e.to_string())?;
    let mut sigint = signal(SignalKind::interrupt()).map_err(|e| e.to_string())?;
    let mut ticker = interval(Duration::from_secs(1));
    ticker.set_missed_tick_behavior(tokio::time::MissedTickBehavior::Skip);

    emit_json(&json!({
        "event": "watercooling_daemon_started",
        "backend": "rust",
        "version": VERSION,
        "socket": sock_path.display().to_string(),
    }));

    loop {
        tokio::select! {
            _ = sigterm.recv() => {
                emit_json(&json!({"event": "daemon_stopping", "signal": "SIGTERM"}));
                break;
            }
            _ = sigint.recv() => {
                emit_json(&json!({"event": "daemon_stopping", "signal": "SIGINT"}));
                break;
            }
            Some(ev) = logind_rx.recv() => {
                match ev {
                    LogindEvent::PrepareForSleep(entering) => {
                        emit_json(&json!({"event": "prepare_for_sleep", "entering": entering}));
                        coordinator.prepare_for_sleep(entering).await;
                    }
                    LogindEvent::PrepareForShutdown => {
                        emit_json(&json!({"event": "prepare_for_shutdown"}));
                        break;
                    }
                }
            }
            Some(cmd) = cmd_rx.recv() => {
                let resp = match coordinator.dispatch(&cmd.req).await {
                    Ok(result) => json!({"ok": true, "result": result}),
                    Err(error) => json!({"ok": false, "error": error}),
                };
                let _ = cmd.reply.send(resp);
            }
            _ = ticker.tick() => {
                coordinator.tick().await;
            }
        }
    }

    coordinator.prepare_for_sleep(true).await;
    logind_abort.abort();
    ipc_task.abort();
    let _ = fs::remove_file(&sock_path);
    Ok(())
}

// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! AF_UNIX JSON-line IPC server with SO_PEERCRED same-UID verification and non-blocking
//! snapshot fast-path for `ping` and `status` requests.

use crate::coordinator::StatusSnapshot;
use serde_json::{json, Value};
use std::os::unix::fs::{FileTypeExt, MetadataExt, PermissionsExt};
use std::path::Path;
use std::sync::Arc;
use std::time::Duration;
use tokio::io::{AsyncBufReadExt, AsyncReadExt, AsyncWriteExt, BufReader};
use tokio::net::{UnixListener, UnixStream};
use tokio::sync::{mpsc, oneshot, RwLock, Semaphore};
use tokio::time::timeout;

const MAX_IPC_LINE_BYTES: u64 = 16_384;
const MAX_CONCURRENT_IPC: usize = 24;

pub struct IpcCommand {
    pub req: Value,
    pub reply: oneshot::Sender<Value>,
}

pub async fn bind_ipc_listener(sock_path: &Path) -> Result<UnixListener, String> {
    match std::fs::symlink_metadata(sock_path) {
        Ok(meta) if meta.file_type().is_socket() && meta.uid() == unsafe { libc::geteuid() } => {
            // Both implementations hold the same daemon lock throughout bind and cleanup.
            match timeout(Duration::from_millis(500), UnixStream::connect(sock_path)).await {
                Ok(Err(e)) if e.kind() == std::io::ErrorKind::ConnectionRefused => {
                    std::fs::remove_file(sock_path).map_err(|e| e.to_string())?;
                }
                _ => return Err("水冷控制 socket 仍被占用".into()),
            }
        }
        Ok(_) => return Err(format!("水冷 socket 路径已被占用: {}", sock_path.display())),
        Err(e) if e.kind() == std::io::ErrorKind::NotFound => {}
        Err(e) => return Err(e.to_string()),
    }
    let listener = UnixListener::bind(sock_path)
        .map_err(|e| format!("绑定水冷 IPC socket 失败 ({}): {e}", sock_path.display()))?;
    std::fs::set_permissions(sock_path, std::fs::Permissions::from_mode(0o600))
        .map_err(|e| format!("设置 socket 权限 0600 失败: {e}"))?;
    Ok(listener)
}

pub async fn serve_ipc(
    listener: UnixListener,
    my_uid: u32,
    cmd_tx: mpsc::Sender<IpcCommand>,
    snapshot: Arc<RwLock<StatusSnapshot>>,
) {
    let sem = Arc::new(Semaphore::new(MAX_CONCURRENT_IPC));
    loop {
        let stream = match listener.accept().await {
            Ok((stream, _addr)) => stream,
            Err(_) => {
                tokio::time::sleep(Duration::from_millis(50)).await;
                continue;
            }
        };
        let Ok(permit) = sem.clone().try_acquire_owned() else {
            drop(stream);
            continue;
        };
        let tx = cmd_tx.clone();
        let snap = snapshot.clone();
        tokio::spawn(async move {
            let _permit = permit;
            handle_client(stream, my_uid, tx, snap).await;
        });
    }
}

async fn handle_client(
    mut stream: UnixStream,
    my_uid: u32,
    cmd_tx: mpsc::Sender<IpcCommand>,
    snapshot: Arc<RwLock<StatusSnapshot>>,
) {
    let peer_uid = match stream.peer_cred() {
        Ok(c) => c.uid(),
        Err(e) => {
            let _ = write_json(&mut stream, &json!({"ok": false, "error": format!("无法验证对端 UID: {e}")})).await;
            return;
        }
    };
    if peer_uid != my_uid {
        let _ = write_json(
            &mut stream,
            &json!({"ok": false, "error": "仅允许同用户进程访问水冷服务"}),
        )
        .await;
        return;
    }

    let (read_half, mut write_half) = stream.split();
    let mut reader = BufReader::new(read_half.take(MAX_IPC_LINE_BYTES + 1));
    let mut line = String::new();
    let read_res = timeout(Duration::from_secs(2), reader.read_line(&mut line)).await;
    let req: Value = match read_res {
        Ok(Ok(n)) if n > 0 && (n as u64) <= MAX_IPC_LINE_BYTES && line.ends_with('\n') => {
            match serde_json::from_str(line.trim()) {
                Ok(v) => v,
                Err(e) => {
                    let _ = write_json(&mut write_half, &json!({"ok": false, "error": format!("JSON 解析失败: {e}")})).await;
                    return;
                }
            }
        }
        _ => {
            let _ = write_json(&mut write_half, &json!({"ok": false, "error": "水冷服务请求格式无效"})).await;
            return;
        }
    };

    // Fast path: `ping` and `status` respond immediately from lock-free snapshot without waiting on BLE I/O
    if let Some(op) = req.get("op").and_then(Value::as_str) {
        if op == "ping" {
            let connected = snapshot.read().await.connected;
            let _ = write_json(&mut write_half, &json!({"ok": true, "result": {"pong": true, "connected": connected}})).await;
            return;
        }
        if op == "status" {
            let snap = snapshot.read().await.clone();
            let resp = match serde_json::to_value(&snap) {
                Ok(v) => json!({"ok": true, "result": v}),
                Err(e) => json!({"ok": false, "error": e.to_string()}),
            };
            let _ = write_json(&mut write_half, &resp).await;
            return;
        }
    }

    let (reply_tx, reply_rx) = oneshot::channel();
    if timeout(Duration::from_secs(3), cmd_tx.send(IpcCommand { req, reply: reply_tx }))
        .await
        .map_err(|_| ())
        .and_then(|r| r.map_err(|_| ()))
        .is_err()
    {
        let _ = write_json(&mut write_half, &json!({"ok": false, "error": "水冷服务正忙或正在退出"})).await;
        return;
    }

    let resp = match timeout(Duration::from_secs(20), reply_rx).await {
        Ok(Ok(val)) => val,
        Ok(Err(_)) => json!({"ok": false, "error": "水冷服务内部通道中断"}),
        Err(_) => json!({"ok": false, "error": "水冷服务执行超时"}),
    };
    let _ = write_json(&mut write_half, &resp).await;
}

async fn write_json<W: AsyncWriteExt + Unpin>(w: &mut W, val: &Value) -> std::io::Result<()> {
    let mut s = serde_json::to_string(val).unwrap_or_else(|_| "{\"ok\":false,\"error\":\"encode error\"}".to_string());
    s.push('\n');
    timeout(Duration::from_secs(2), async {
        w.write_all(s.as_bytes()).await?;
        w.flush().await
    })
    .await
    .map_err(|_| std::io::Error::new(std::io::ErrorKind::TimedOut, "ipc write timeout"))?
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::ble::BleTransport;
    use crate::coordinator::CoolerCoordinator;
    use crate::protocol::CoolerConfig;
    use tokio::io::{AsyncBufReadExt, AsyncWriteExt, BufReader};

    struct DummyTransport;
    impl BleTransport for DummyTransport {
        fn scan<'a>(&'a mut self, _: f64, _: &'a str) -> crate::ble::BoxFuture<'a, Result<Vec<crate::ble::ScannedDevice>, String>> {
            Box::pin(async { Ok(vec![]) })
        }
        fn connect<'a>(&'a mut self, _: &'a str, _: f64, _: mpsc::UnboundedSender<crate::ble::BleEvent>) -> crate::ble::BoxFuture<'a, Result<crate::ble::TransportConnectInfo, String>> {
            Box::pin(async { Err("unused".to_string()) })
        }
        fn write_frame<'a>(&'a mut self, _: &'a [u8], _: &'a str) -> crate::ble::BoxFuture<'a, Result<(), String>> {
            Box::pin(async { Ok(()) })
        }
        fn disconnect<'a>(&'a mut self) -> crate::ble::BoxFuture<'a, ()> {
            Box::pin(async {})
        }
    }

    #[tokio::test]
    async fn bind_preserves_existing_files_and_live_listeners() {
        let path = std::env::temp_dir().join(format!("iteglow-bind-{}.sock", std::process::id()));
        std::fs::write(&path, b"preserve").unwrap();
        assert!(bind_ipc_listener(&path).await.is_err());
        assert_eq!(std::fs::read(&path).unwrap(), b"preserve");
        std::fs::remove_file(&path).unwrap();
        let first = bind_ipc_listener(&path).await.unwrap();
        assert!(bind_ipc_listener(&path).await.is_err());
        drop(first);
        let _replacement = bind_ipc_listener(&path).await.unwrap();
        std::fs::remove_file(path).unwrap();
    }

    #[tokio::test]
    async fn test_ipc_ping_and_status_fast_path() {
        let sock = std::env::temp_dir().join(format!("iteglow-ipc-{}.sock", std::process::id()));
        let listener = bind_ipc_listener(&sock).await.unwrap();
        let cfg = CoolerConfig::default();
        let snap = Arc::new(RwLock::new(CoolerCoordinator::<DummyTransport>::default_snapshot(&cfg)));
        let (tx, _rx) = mpsc::channel(8);
        let uid = unsafe { libc::geteuid() };
        let srv = tokio::spawn(serve_ipc(listener, uid, tx, snap));

        let mut cli = UnixStream::connect(&sock).await.unwrap();
        cli.write_all(b"{\"op\":\"ping\"}\n").await.unwrap();
        let mut line = String::new();
        BufReader::new(&mut cli).read_line(&mut line).await.unwrap();
        let v: Value = serde_json::from_str(line.trim()).unwrap();
        assert_eq!(v["ok"], true);
        assert_eq!(v["result"]["pong"], true);

        let mut cli2 = UnixStream::connect(&sock).await.unwrap();
        cli2.write_all(b"{\"op\":\"status\"}\n").await.unwrap();
        let mut line2 = String::new();
        BufReader::new(&mut cli2).read_line(&mut line2).await.unwrap();
        let v2: Value = serde_json::from_str(line2.trim()).unwrap();
        assert_eq!(v2["ok"], true);
        assert_eq!(v2["result"]["service_running"], true);
        assert_eq!(v2["result"]["config"]["preset"], "silent");

        srv.abort();
        let _ = std::fs::remove_file(sock);
    }
}

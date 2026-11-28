// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Async BlueZ D-Bus BLE GATT transport (`zbus`) and paced session controller.
//!
//! Hardware rules (verified on CoolingSystem LCT22002 F/W 2.0.0.4):
//! - Never calls BlueZ Device1.Pair() (LCT22002 rejects SMP with 0x05).
//! - Retries Device1.Connect() up to 3 attempts on transient HCI 0x3e / abort / timeout.
//! - Tolerates InProgress during discovery and AlreadyConnected / AlreadyNotifying on takeover.
//! - Uses ATT Write Request ('request') with >=100ms TX pacing to avoid MCU UART collision drops.
//! - Never downgrades write_type on disconnect errors; immediately signals link loss.

use crate::protocol::{self, LedConfig, RxPacket, DEVICE_NAME_PREFIXES, NUS_RX_UUID, NUS_TX_UUID};
use futures_util::StreamExt;
use serde::{Deserialize, Serialize};
use std::collections::HashMap;
use std::future::Future;
use std::pin::Pin;
use std::sync::Arc;
use std::time::Duration;
use tokio::sync::{mpsc, Mutex};
use tokio::time::{sleep, timeout, Instant};
use zbus::zvariant::{OwnedObjectPath, OwnedValue, Value};
use zbus::{Connection, MatchRule, MessageStream};

const BLUEZ_SERVICE: &str = "org.bluez";
const ADAPTER_PATH: &str = "/org/bluez/hci0";

type ManagedObjects = HashMap<OwnedObjectPath, HashMap<String, HashMap<String, OwnedValue>>>;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ScannedDevice {
    pub path: String,
    pub name: String,
    pub address: String,
    pub rssi: Option<i16>,
    pub connected: bool,
    pub paired: bool,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ConnectInfo {
    pub name: String,
    pub address: String,
    pub fw_version: Option<String>,
    pub meter_state: Option<u8>,
}

#[derive(Debug, Clone)]
pub struct TransportConnectInfo {
    pub name: String,
    pub address: String,
}

#[derive(Debug, Clone)]
pub enum BleEvent {
    Notify(Vec<u8>),
    Disconnected(String),
}

pub fn is_in_progress(msg: &str) -> bool {
    let m = msg.to_ascii_lowercase();
    m.contains("inprogress") || m.contains("already in progress") || m.contains("alreadystarted")
}

pub fn is_already_connected(msg: &str) -> bool {
    let m = msg.to_ascii_lowercase();
    m.contains("alreadyconnected") || m.contains("already connected")
}

pub fn is_already_notifying(msg: &str) -> bool {
    let m = msg.to_ascii_lowercase();
    m.contains("alreadynotifying") || m.contains("already notifying")
}

pub fn is_disconnect_error(msg: &str) -> bool {
    let m = msg.to_ascii_lowercase();
    [
        "org.bluez.error.notconnected",
        "notconnected",
        "not connected",
        "disconnected",
        "software caused connection abort",
        "connection reset",
        "transport endpoint is not connected",
        "does not exist",
        "unknownobject",
        "no such object",
        "le-connection-abort",
        "br-connection-",
    ]
    .iter()
    .any(|tok| m.contains(tok))
}

fn is_transient_att_error(msg: &str) -> bool {
    let m = msg.to_ascii_lowercase();
    m.contains("att error: 0x0e") || m.contains("unlikely")
}

fn is_transient_connect_error(msg: &str) -> bool {
    let m = msg.to_ascii_lowercase();
    [
        "le-connection-abort-by-local",
        "inprogress",
        "timeout",
        "page-timeout",
        "br-connection-canceled",
    ]
    .iter()
    .any(|tok| m.contains(tok))
}

pub type BoxFuture<'a, T> = Pin<Box<dyn Future<Output = T> + Send + 'a>>;

pub trait BleTransport: Send + Sync {
    fn scan<'a>(
        &'a mut self,
        timeout_s: f64,
        target_mac: &'a str,
    ) -> BoxFuture<'a, Result<Vec<ScannedDevice>, String>>;

    fn connect<'a>(
        &'a mut self,
        target_mac: &'a str,
        scan_timeout_s: f64,
        events: mpsc::UnboundedSender<BleEvent>,
    ) -> BoxFuture<'a, Result<TransportConnectInfo, String>>;

    fn write_frame<'a>(
        &'a mut self,
        data: &'a [u8],
        write_type: &'a str,
    ) -> BoxFuture<'a, Result<(), String>>;

    fn disconnect<'a>(&'a mut self) -> BoxFuture<'a, ()>;
}

pub struct BluezBleTransport {
    conn: Connection,
    adapter_path: String,
    dev_path: Option<String>,
    tx_path: Option<String>,
    rx_path: Option<String>,
    notifying: bool,
    connected: bool,
    watcher_abort: Option<tokio::task::AbortHandle>,
}

impl BluezBleTransport {
    pub fn new(conn: Connection) -> Self {
        Self {
            conn,
            adapter_path: ADAPTER_PATH.to_string(),
            dev_path: None,
            tx_path: None,
            rx_path: None,
            notifying: false,
            connected: false,
            watcher_abort: None,
        }
    }

    async fn managed_objects(&self) -> Result<ManagedObjects, String> {
        let msg = timeout(
            Duration::from_secs(5),
            self.conn.call_method(
                Some(BLUEZ_SERVICE),
                "/",
                Some("org.freedesktop.DBus.ObjectManager"),
                "GetManagedObjects",
                &(),
            ),
        )
        .await
        .map_err(|_| "BlueZ GetManagedObjects 超时".to_string())?
        .map_err(|e| format!("BlueZ GetManagedObjects 失败: {e}"))?;

        msg.body()
            .deserialize::<ManagedObjects>()
            .map_err(|e| format!("解析 BlueZ ManagedObjects 失败: {e}"))
    }

    fn prop_str(props: &HashMap<String, OwnedValue>, key: &str) -> String {
        props
            .get(key)
            .and_then(|v| <&str>::try_from(v).ok())
            .map(ToOwned::to_owned)
            .unwrap_or_default()
    }

    fn prop_bool(props: &HashMap<String, OwnedValue>, key: &str) -> bool {
        props
            .get(key)
            .and_then(|v| bool::try_from(v).ok())
            .unwrap_or(false)
    }

    fn prop_i16(props: &HashMap<String, OwnedValue>, key: &str) -> Option<i16> {
        props.get(key).and_then(|v| i16::try_from(v).ok())
    }

    fn matches(props: &HashMap<String, OwnedValue>, target_mac: &str) -> bool {
        let addr = Self::prop_str(props, "Address").to_ascii_uppercase();
        if !target_mac.is_empty() {
            return addr == target_mac.to_ascii_uppercase();
        }
        let name = Self::prop_str(props, "Name");
        let alias = Self::prop_str(props, "Alias");
        [name.as_str(), alias.as_str()].iter().any(|nm| {
            !nm.is_empty()
                && DEVICE_NAME_PREFIXES.iter().any(|prefix| {
                    nm.starts_with(prefix)
                        || nm.to_ascii_uppercase().contains(&prefix.to_ascii_uppercase())
                })
        })
    }

    fn find_matching_devices(objs: &ManagedObjects, target_mac: &str) -> Vec<ScannedDevice> {
        let mut found = Vec::new();
        let mut entries: Vec<(&OwnedObjectPath, _)> = objs.iter().collect();
        entries.sort_by(|a, b| a.0.as_str().cmp(b.0.as_str()));

        for (path, ifaces) in entries {
            let Some(dev) = ifaces.get("org.bluez.Device1") else {
                continue;
            };
            if !Self::matches(dev, target_mac) {
                continue;
            }
            let name = {
                let n = Self::prop_str(dev, "Name");
                if !n.is_empty() {
                    n
                } else {
                    Self::prop_str(dev, "Alias")
                }
            };
            found.push(ScannedDevice {
                path: path.as_str().to_string(),
                name,
                address: Self::prop_str(dev, "Address").to_ascii_uppercase(),
                rssi: Self::prop_i16(dev, "RSSI"),
                connected: Self::prop_bool(dev, "Connected"),
                paired: Self::prop_bool(dev, "Paired"),
            });
        }
        found
    }

    fn find_nus_chars(objs: &ManagedObjects, dev_path: &str) -> (Option<String>, Option<String>) {
        let mut tx = None;
        let mut rx = None;
        let prefix = format!("{dev_path}/");
        for (path, ifaces) in objs {
            if !path.as_str().starts_with(&prefix) {
                continue;
            }
            let Some(ch) = ifaces.get("org.bluez.GattCharacteristic1") else {
                continue;
            };
            let uuid = Self::prop_str(ch, "UUID").to_ascii_lowercase();
            if uuid == NUS_TX_UUID {
                tx = Some(path.as_str().to_string());
            } else if uuid == NUS_RX_UUID {
                rx = Some(path.as_str().to_string());
            }
        }
        (tx, rx)
    }

    fn stop_watcher(&mut self) {
        if let Some(h) = self.watcher_abort.take() {
            h.abort();
        }
    }

    async fn spawn_watcher(
        &mut self,
        dev_path: String,
        rx_path: Arc<Mutex<Option<String>>>,
        events: mpsc::UnboundedSender<BleEvent>,
    ) -> Result<(), String> {
        self.stop_watcher();
        let rule = MatchRule::builder()
            .msg_type(zbus::message::Type::Signal)
            .sender(BLUEZ_SERVICE)
            .map_err(|e| e.to_string())?
            .interface("org.freedesktop.DBus.Properties")
            .map_err(|e| e.to_string())?
            .member("PropertiesChanged")
            .map_err(|e| e.to_string())?
            .path_namespace(dev_path.as_str())
            .map_err(|e| e.to_string())?
            .build();
        let mut stream = MessageStream::for_match_rule(rule, &self.conn, Some(64))
            .await
            .map_err(|e| format!("订阅 BlueZ 信号失败: {e}"))?;

        let handle = tokio::spawn(async move {
            while let Some(item) = stream.next().await {
                let Ok(msg) = item else {
                    break;
                };
                let path = msg
                    .header()
                    .path()
                    .map(|p| p.as_str().to_string())
                    .unwrap_or_default();
                let Ok((iface, changed, _inv)) = msg
                    .body()
                    .deserialize::<(String, HashMap<String, OwnedValue>, Vec<String>)>()
                else {
                    continue;
                };

                let expected_rx = rx_path.lock().await.clone();
                if iface == "org.bluez.GattCharacteristic1"
                    && expected_rx.as_deref() == Some(path.as_str())
                {
                    if let Some(val) = changed.get("Value") {
                        if let Ok(bytes) = Vec::<u8>::try_from(val.clone()) {
                            let _ = events.send(BleEvent::Notify(bytes));
                            continue;
                        }
                    }
                }

                if path == dev_path && iface == "org.bluez.Device1" {
                    if let Some(conn_val) = changed.get("Connected") {
                        if bool::try_from(conn_val) == Ok(false) {
                            let _ = events
                                .send(BleEvent::Disconnected("peer_or_link_disconnected".to_string()));
                        }
                    }
                }
            }
        });
        self.watcher_abort = Some(handle.abort_handle());
        Ok(())
    }

    async fn call_no_args(&self, path: &str, iface: &str, method: &str, secs: u64) -> Result<(), String> {
        timeout(
            Duration::from_secs(secs),
            self.conn
                .call_method(Some(BLUEZ_SERVICE), path, Some(iface), method, &()),
        )
        .await
        .map_err(|_| format!("BlueZ {iface}.{method} 超时"))?
        .map_err(|e| e.to_string())?;
        Ok(())
    }
}

impl BleTransport for BluezBleTransport {
    fn scan<'a>(
        &'a mut self,
        timeout_s: f64,
        target_mac: &'a str,
    ) -> BoxFuture<'a, Result<Vec<ScannedDevice>, String>> {
        Box::pin(async move {
            let mut filter: HashMap<&str, Value<'_>> = HashMap::new();
            filter.insert("Transport", Value::from("le"));

            if let Err(e) = timeout(
                Duration::from_secs(5),
                self.conn.call_method(
                    Some(BLUEZ_SERVICE),
                    self.adapter_path.as_str(),
                    Some("org.bluez.Adapter1"),
                    "SetDiscoveryFilter",
                    &(filter,),
                ),
            )
            .await
            .map_err(|_| "SetDiscoveryFilter 超时".to_string())
            .and_then(|r| r.map_err(|e| e.to_string()))
            {
                if !is_in_progress(&e) {
                    return Err(format!("设置 BLE 扫描过滤失败: {e}"));
                }
            }

            let started = match self
                .call_no_args(&self.adapter_path, "org.bluez.Adapter1", "StartDiscovery", 5)
                .await
            {
                Ok(()) => true,
                Err(e) if is_in_progress(&e) => false,
                Err(e) => return Err(format!("启动 BLE 扫描失败: {e}")),
            };

            let deadline = Instant::now() + Duration::from_secs_f64(timeout_s.max(0.2));
            let res = loop {
                sleep(Duration::from_millis(250)).await;
                if let Ok(objs) = self.managed_objects().await {
                    let matches = Self::find_matching_devices(&objs, target_mac);
                    if !matches.is_empty()
                        && matches.iter().any(|m| m.rssi.is_some() || m.connected)
                    {
                        break Ok(matches);
                    }
                }
                if Instant::now() >= deadline {
                    break self
                        .managed_objects()
                        .await
                        .map(|objs| Self::find_matching_devices(&objs, target_mac));
                }
            };

            if started {
                let _ = self
                    .call_no_args(&self.adapter_path, "org.bluez.Adapter1", "StopDiscovery", 3)
                    .await;
            }
            sleep(Duration::from_millis(120)).await;
            res
        })
    }

    fn connect<'a>(
        &'a mut self,
        target_mac: &'a str,
        scan_timeout_s: f64,
        events: mpsc::UnboundedSender<BleEvent>,
    ) -> BoxFuture<'a, Result<TransportConnectInfo, String>> {
        Box::pin(async move {
            let objs = self.managed_objects().await?;
            let cached = Self::find_matching_devices(&objs, target_mac);
            let mut matches = cached.clone();
            if matches.is_empty()
                || matches.iter().all(|m| m.rssi.is_none() && !m.connected)
            {
                let scanned = self.scan(scan_timeout_s, target_mac).await?;
                if !scanned.is_empty() {
                    matches = scanned;
                } else if !cached.is_empty() && !target_mac.is_empty() {
                    matches = cached;
                }
            }
            let Some(mut target) = matches.into_iter().next() else {
                return Err(
                    "未扫描到机械革命蓝牙水冷设备（请确认水冷开机且未处于离线模式）".to_string(),
                );
            };

            self.dev_path = Some(target.path.clone());
            self.tx_path = None;
            self.rx_path = None;
            let shared_rx = Arc::new(Mutex::new(None::<String>));
            self.spawn_watcher(target.path.clone(), shared_rx.clone(), events)
                .await?;

            if target.connected {
                self.connected = true;
            } else {
                match self
                    .call_no_args(&target.path, "org.bluez.Device1", "Connect", 12)
                    .await
                {
                    Ok(()) => self.connected = true,
                    Err(e) if is_already_connected(&e) => self.connected = true,
                    Err(e) => {
                        self.stop_watcher();
                        return Err(format!("BLE 连接水冷失败：{e}"));
                    }
                }
            }

            let deadline = Instant::now() + Duration::from_secs(6);
            let mut resolved_objs = None;
            while Instant::now() < deadline {
                sleep(Duration::from_millis(150)).await;
                if let Ok(objs) = self.managed_objects().await {
                    if let Some(dev) = objs
                        .get(&OwnedObjectPath::try_from(target.path.as_str()).unwrap())
                        .and_then(|ifaces| ifaces.get("org.bluez.Device1"))
                    {
                        if Self::prop_bool(dev, "ServicesResolved") {
                            let nm = Self::prop_str(dev, "Name");
                            let al = Self::prop_str(dev, "Alias");
                            if !nm.is_empty() {
                                target.name = nm;
                            } else if !al.is_empty() {
                                target.name = al;
                            }
                            let addr = Self::prop_str(dev, "Address");
                            if !addr.is_empty() {
                                target.address = addr.to_ascii_uppercase();
                            }
                            resolved_objs = Some(objs);
                            break;
                        }
                    }
                }
            }

            let Some(objs) = resolved_objs else {
                self.disconnect().await;
                return Err("BLE GATT 服务发现超时".to_string());
            };

            let (tx_opt, rx_opt) = Self::find_nus_chars(&objs, &target.path);
            let (Some(tx_path), Some(rx_path)) = (tx_opt, rx_opt) else {
                self.disconnect().await;
                return Err("未找到 Nordic UART TX/RX 特征值".to_string());
            };

            self.tx_path = Some(tx_path);
            self.rx_path = Some(rx_path.clone());
            *shared_rx.lock().await = Some(rx_path.clone());

            if let Err(e) = self
                .call_no_args(&rx_path, "org.bluez.GattCharacteristic1", "StartNotify", 5)
                .await
            {
                if !is_already_notifying(&e) {
                    self.disconnect().await;
                    return Err(format!("启用水冷 BLE 通知失败：{e}"));
                }
            }
            self.notifying = true;
            sleep(Duration::from_millis(200)).await;

            Ok(TransportConnectInfo {
                name: target.name,
                address: target.address,
            })
        })
    }

    fn write_frame<'a>(
        &'a mut self,
        data: &'a [u8],
        write_type: &'a str,
    ) -> BoxFuture<'a, Result<(), String>> {
        Box::pin(async move {
            let Some(tx_path) = self.tx_path.clone() else {
                return Err("水冷蓝牙未连接".to_string());
            };
            if !self.connected {
                return Err("水冷蓝牙未连接".to_string());
            }
            let mut opts: HashMap<&str, Value<'_>> = HashMap::new();
            opts.insert("type", Value::from(write_type));
            timeout(
                Duration::from_secs(5),
                self.conn.call_method(
                    Some(BLUEZ_SERVICE),
                    tx_path.as_str(),
                    Some("org.bluez.GattCharacteristic1"),
                    "WriteValue",
                    &(data.to_vec(), opts),
                ),
            )
            .await
            .map_err(|_| "BLE WriteValue 超时".to_string())?
            .map_err(|e| e.to_string())?;
            Ok(())
        })
    }

    fn disconnect<'a>(&'a mut self) -> BoxFuture<'a, ()> {
        Box::pin(async move {
            let was_connected = self.connected;
            self.connected = false;
            if self.notifying && was_connected {
                if let Some(rx) = self.rx_path.clone() {
                    let _ = self
                        .call_no_args(&rx, "org.bluez.GattCharacteristic1", "StopNotify", 3)
                        .await;
                }
                self.notifying = false;
                sleep(Duration::from_millis(120)).await;
            }
            if let Some(dev) = self.dev_path.clone() {
                let _ = self
                    .call_no_args(&dev, "org.bluez.Device1", "Disconnect", 5)
                    .await;
                sleep(Duration::from_millis(250)).await;
            }
            self.stop_watcher();
            self.tx_path = None;
            self.rx_path = None;
        })
    }
}

impl Drop for BluezBleTransport {
    fn drop(&mut self) {
        self.stop_watcher();
    }
}

pub struct CoolerBleClient<T: BleTransport> {
    pub transport: T,
    pub min_gap: Duration,
    last_tx_at: Option<Instant>,
    pub connected: bool,
    pub device_name: String,
    pub device_mac: String,
    pub fw_version: Option<String>,
    pub meter_state: Option<u8>,
    meter_rx_seq: u64,
    telemetry_lost: bool,
    pub preferred_write_type: String,
    event_rx: Option<mpsc::UnboundedReceiver<BleEvent>>,
    pending_disconnect: Option<String>,
}

impl<T: BleTransport> CoolerBleClient<T> {
    pub fn new(transport: T, min_gap: Duration) -> Self {
        Self {
            transport,
            min_gap,
            last_tx_at: None,
            connected: false,
            device_name: String::new(),
            device_mac: String::new(),
            fw_version: None,
            meter_state: None,
            meter_rx_seq: 0,
            telemetry_lost: false,
            preferred_write_type: "request".to_string(),
            event_rx: None,
            pending_disconnect: None,
        }
    }

    fn handle_event(&mut self, event: BleEvent) {
        match event {
            BleEvent::Notify(raw) => self.handle_rx(&raw),
            BleEvent::Disconnected(reason) if self.connected => {
                self.connected = false;
                self.pending_disconnect = Some(reason);
            }
            BleEvent::Disconnected(_) => {}
        }
    }

    fn notification_stream_closed(&mut self) {
        // Keep the GATT writer available so flow protection can still send stop frames.
        self.event_rx = None;
        self.telemetry_lost = true;
        self.meter_state = None;
    }

    fn poll_events(&mut self) {
        while let Some(rx) = self.event_rx.as_mut() {
            match rx.try_recv() {
                Ok(ev) => self.handle_event(ev),
                Err(mpsc::error::TryRecvError::Disconnected) => self.notification_stream_closed(),
                Err(mpsc::error::TryRecvError::Empty) => break,
            }
        }
    }

    pub fn drain_events(&mut self) -> Option<String> {
        self.poll_events();
        self.pending_disconnect.take()
    }

    async fn pump_events(&mut self, dur: Duration) {
        self.poll_events();
        let deadline = Instant::now() + dur;
        while let Some(rem) = deadline.checked_duration_since(Instant::now()) {
            let Some(rx) = self.event_rx.as_mut() else {
                sleep(rem).await;
                break;
            };
            match timeout(rem, rx.recv()).await {
                Ok(Some(ev)) => self.handle_event(ev),
                Ok(None) => self.notification_stream_closed(),
                Err(_) => break,
            }
        }
    }

    fn handle_rx(&mut self, raw: &[u8]) {
        match protocol::parse_rx_packet(raw) {
            RxPacket::Meter { meter_state, .. } => {
                self.meter_state = Some(meter_state);
                self.meter_rx_seq = self.meter_rx_seq.wrapping_add(1);
            }
            RxPacket::Firmware { fw_version, .. } => {
                if fw_version.is_some() {
                    self.fw_version = fw_version;
                }
            }
            RxPacket::Unknown => {}
        }
    }

    pub async fn scan(
        &mut self,
        timeout_s: f64,
        target_mac: &str,
    ) -> Result<Vec<ScannedDevice>, String> {
        self.transport.scan(timeout_s, target_mac).await
    }

    pub async fn send_paced(&mut self, data: &[u8], force_type: Option<&str>) -> Result<(), String> {
        self.poll_events();
        if !self.connected {
            return Err("水冷蓝牙未连接".to_string());
        }
        if let Some(last) = self.last_tx_at {
            let elapsed = last.elapsed();
            if elapsed < self.min_gap {
                self.pump_events(self.min_gap - elapsed).await;
            }
        }
        if !self.connected {
            return Err("水冷蓝牙连接已断开".into());
        }
        let wtype = force_type
            .map(ToOwned::to_owned)
            .unwrap_or_else(|| self.preferred_write_type.clone());
        match self.transport.write_frame(data, &wtype).await {
            Ok(()) => {}
            Err(e) => {
                if is_disconnect_error(&e) {
                    self.connected = false;
                    self.transport.disconnect().await;
                    self.pending_disconnect = Some("write_link_lost".to_string());
                    return Err(e);
                }
                if is_transient_att_error(&e) {
                    // ATT 0x0e (Unlikely) on LCT22002 can happen transiently on UART busy;
                    // do not tear down link or permanently downgrade write mode.
                    return Err(e);
                }
                if force_type.is_none() && wtype == "request" {
                    self.preferred_write_type = "command".to_string();
                    if let Err(e2) = self.transport.write_frame(data, "command").await {
                        if is_disconnect_error(&e2) {
                            self.connected = false;
                            self.transport.disconnect().await;
                            self.pending_disconnect = Some("write_link_lost".to_string());
                        }
                        return Err(e2);
                    }
                } else {
                    return Err(e);
                }
            }
        }
        self.last_tx_at = Some(Instant::now());
        Ok(())
    }

    pub async fn connect(
        &mut self,
        target_mac: &str,
        scan_timeout_s: f64,
    ) -> Result<ConnectInfo, String> {
        if self.connected {
            return Ok(ConnectInfo {
                name: self.device_name.clone(),
                address: self.device_mac.clone(),
                fw_version: self.fw_version.clone(),
                meter_state: self.meter_state,
            });
        }
        self.fw_version = None;
        self.telemetry_lost = false;
        self.meter_state = None;
        self.pending_disconnect = None;

        let mut last_err = None;
        let mut info = None;
        for attempt in 1..=3 {
            let (tx, rx) = mpsc::unbounded_channel();
            match self.transport.connect(target_mac, scan_timeout_s, tx).await {
                Ok(ci) => {
                    self.event_rx = Some(rx);
                    info = Some(ci);
                    last_err = None;
                    break;
                }
                Err(e) => {
                    if attempt < 3 && is_transient_connect_error(&e) {
                        self.transport.disconnect().await;
                        sleep(Duration::from_millis(900)).await;
                        last_err = Some(e);
                        continue;
                    }
                    self.transport.disconnect().await;
                    return Err(e);
                }
            }
        }
        let Some(ci) = info else {
            return Err(last_err.unwrap_or_else(|| "BLE 连接失败".to_string()));
        };

        self.connected = true;
        self.device_name = ci.name;
        self.device_mac = ci.address;
        self.preferred_write_type = "request".to_string();

        let hs = protocol::encode_handshake();
        if let Err(e) = self.send_paced(&hs, None).await {
            let _ = self.disconnect(false, false).await;
            return Err(e);
        }
        self.pump_events(Duration::from_millis(350)).await;
        let mq = protocol::encode_meter_query();
        if let Err(e) = self.send_paced(&mq, None).await {
            let _ = self.disconnect(false, false).await;
            return Err(e);
        }
        self.pump_events(Duration::from_millis(250)).await;
        if !self.connected || self.telemetry_lost {
            let _ = self.disconnect(false, false).await;
            return Err("握手期间水冷蓝牙连接已断开".into());
        }

        Ok(ConnectInfo {
            name: self.device_name.clone(),
            address: self.device_mac.clone(),
            fw_version: self.fw_version.clone(),
            meter_state: self.meter_state,
        })
    }

    pub async fn query_meter(&mut self) -> Result<Option<u8>, String> {
        self.meter_state = None;
        if self.telemetry_lost {
            return Err("BlueZ 水冷通知订阅已关闭".into());
        }
        let before_seq = self.meter_rx_seq;
        let mq = protocol::encode_meter_query();
        if let Err(e) = self.send_paced(&mq, None).await {
            self.meter_state = None;
            return Err(e);
        }
        self.pump_events(Duration::from_millis(180)).await;
        if self.meter_rx_seq == before_seq {
            // No fresh meter notification arrived; do not retain stale `Some(2)` across silent stalls.
            self.meter_state = None;
        }
        Ok(self.meter_state)
    }

    pub async fn apply_led(&mut self, led: &LedConfig) -> Result<(), String> {
        let plan = protocol::encode_led_plan(led)?;
        for (frame, _label) in plan {
            self.send_paced(&frame, None).await?;
        }
        Ok(())
    }

    pub async fn disconnect(&mut self, send_sleep: bool, stop_actuators: bool) -> Result<(), String> {
        let mut errors = Vec::new();
        if self.connected {
            if stop_actuators {
                let frames = [
                    ("水泵", protocol::encode_pump(0, 0)),
                    ("风扇", protocol::encode_fan(0)),
                    ("风扇灯", Ok(protocol::encode_fan_led_ctrl(false, [0, 0, 0], 0))),
                    ("主灯", protocol::encode_main_led(false, [0, 0, 0], "off")),
                ];
                for (name, frame) in frames {
                    let result = match frame {
                        Ok(bytes) => self.send_paced(&bytes, None).await,
                        Err(e) => Err(e),
                    };
                    if let Err(e) = result { errors.push(format!("{name}: {e}")); }
                }
            }
            if send_sleep {
                if let Err(e) = self.send_paced(&protocol::encode_sleep(), Some("command")).await {
                    errors.push(format!("休眠: {e}"));
                }
                self.pump_events(Duration::from_millis(250)).await;
            }
        }
        self.connected = false;
        self.event_rx = None;
        self.pending_disconnect = None;
        self.transport.disconnect().await;
        if errors.is_empty() { Ok(()) } else { Err(format!("未确认设备停止指令：{}", errors.join("；"))) }
    }
}

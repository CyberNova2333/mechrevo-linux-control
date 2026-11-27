// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Coordinator state machine bridging CoolerBleClient, thermal curves, EC telemetry,
//! flow safety protection, wake/disconnect backoff, and structured journald logging.

use crate::ble::{BleTransport, CoolerBleClient, ScannedDevice};
use crate::protocol::{
    self, CoolerConfig, EcWatercoolerState, FlowSafetyMonitor, StageConfig,
    DEFAULT_LC_EC_STATE_PATH, DEFAULT_WMI_STATE_PATH,
};
use serde::{Deserialize, Serialize};
use serde_json::{json, Value};
use std::path::{Path, PathBuf};
use std::sync::Arc;
use tokio::sync::RwLock;

pub const RECONNECT_BACKOFF_S: &[f64] = &[3.0, 5.0, 8.0, 15.0];
pub const LINK_DROP_RECONNECT_DELAY_S: f64 = 2.0;
pub const WAKE_RECONNECT_DELAY_S: f64 = 2.5;
pub const MAX_CONSECUTIVE_METER_ERRORS: u32 = 4;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct StatusSnapshot {
    pub service_running: bool,
    pub connected: bool,
    pub sleeping: bool,
    pub device_name: String,
    pub device_mac: String,
    pub fw_version: Option<String>,
    pub meter_state: Option<u8>,
    pub flow_tripped: bool,
    pub flow_trip_reason: String,
    pub applied: StageConfig,
    pub current_tier: usize,
    pub reason: String,
    pub last_error: String,
    pub temp_c: Option<f64>,
    pub ec: EcWatercoolerState,
    pub config: CoolerConfig,
}

pub trait Clock: Send + Sync {
    fn now_secs(&self) -> f64;
}

pub struct MonotonicClock {
    start: std::time::Instant,
}

impl MonotonicClock {
    pub fn new() -> Self {
        Self {
            start: std::time::Instant::now(),
        }
    }
}

impl Clock for MonotonicClock {
    fn now_secs(&self) -> f64 {
        self.start.elapsed().as_secs_f64()
    }
}

pub struct CoolerCoordinator<T: BleTransport> {
    pub client: CoolerBleClient<T>,
    pub config: CoolerConfig,
    config_path: PathBuf,
    hwmon_root: PathBuf,
    wmi_path: PathBuf,
    ec_path: PathBuf,
    clock: Arc<dyn Clock>,
    pub flow_monitor: FlowSafetyMonitor,
    event_logger: Option<Arc<dyn Fn(Value) + Send + Sync>>,
    shared_snapshot: Arc<RwLock<StatusSnapshot>>,

    pub sleeping: bool,
    pub current_tier: usize,
    pub applied: StageConfig,
    applied_led: Option<String>,
    actuators_dirty: bool,
    led_dirty: bool,
    pub last_reason: String,
    pub last_error: String,
    pub last_temp_c: Option<f64>,
    pub last_ec_state: EcWatercoolerState,
    last_meter_poll: f64,
    last_c1_seq: Option<i64>,
    next_connect_at: f64,
    reconnect_failures: usize,
    consecutive_meter_errors: u32,
}

impl<T: BleTransport> CoolerCoordinator<T> {
    pub fn new(
        client: CoolerBleClient<T>,
        config: CoolerConfig,
        config_path: PathBuf,
        clock: Arc<dyn Clock>,
        shared_snapshot: Arc<RwLock<StatusSnapshot>>,
        event_logger: Option<Arc<dyn Fn(Value) + Send + Sync>>,
    ) -> Self {
        let mut coord = Self {
            client,
            config,
            config_path,
            hwmon_root: PathBuf::from("/sys/class/hwmon"),
            wmi_path: PathBuf::from(DEFAULT_WMI_STATE_PATH),
            ec_path: PathBuf::from(DEFAULT_LC_EC_STATE_PATH),
            clock,
            flow_monitor: FlowSafetyMonitor::default(),
            event_logger,
            shared_snapshot,
            sleeping: false,
            current_tier: 0,
            applied: StageConfig {
                pump_volt: 0,
                pump_duty: 0,
                fan_duty: 0,
            },
            applied_led: None,
            actuators_dirty: false,
            led_dirty: false,
            last_reason: "idle".to_string(),
            last_error: String::new(),
            last_temp_c: None,
            last_ec_state: EcWatercoolerState::default(),
            last_meter_poll: 0.0,
            last_c1_seq: None,
            next_connect_at: 0.0,
            reconnect_failures: 0,
            consecutive_meter_errors: 0,
        };
        coord.sample_inputs();
        coord
    }

    #[cfg(test)]
    pub fn set_telemetry_paths(&mut self, hwmon: PathBuf, wmi: PathBuf, ec: PathBuf) {
        self.hwmon_root = hwmon;
        self.wmi_path = wmi;
        self.ec_path = ec;
    }

    fn emit(&self, payload: Value) {
        if let Some(logger) = &self.event_logger {
            logger(payload);
        }
    }

    pub fn build_snapshot(&self) -> StatusSnapshot {
        StatusSnapshot {
            service_running: true,
            connected: self.client.connected,
            sleeping: self.sleeping,
            device_name: self.client.device_name.clone(),
            device_mac: self.client.device_mac.clone(),
            fw_version: self.client.fw_version.clone(),
            meter_state: self.client.meter_state,
            flow_tripped: self.flow_monitor.tripped,
            flow_trip_reason: self.flow_monitor.trip_reason.clone(),
            applied: self.applied.clone(),
            current_tier: self.current_tier,
            reason: self.last_reason.clone(),
            last_error: self.last_error.clone(),
            temp_c: self.last_temp_c,
            ec: self.last_ec_state.clone(),
            config: self.config.clone(),
        }
    }

    pub async fn publish_snapshot(&self) {
        let snap = self.build_snapshot();
        *self.shared_snapshot.write().await = snap;
    }

    fn sample_inputs(&mut self) {
        self.last_temp_c = protocol::read_host_temp_c(&self.hwmon_root);
        let ec = protocol::read_ec_watercooler_state(&self.wmi_path, &self.ec_path, 15_000);
        if ec.wmi_c1_seq.is_some() && ec.wmi_c1_seq != self.last_c1_seq {
            self.last_c1_seq = ec.wmi_c1_seq;
            if self.config.strategy == "ec_follow" && self.client.connected {
                self.actuators_dirty = true;
            }
        }
        self.last_ec_state = ec;
    }

    fn on_ble_disconnected(&mut self, reason: &str) {
        self.flow_monitor.on_disconnect();
        self.applied = StageConfig {
            pump_volt: 0,
            pump_duty: 0,
            fan_duty: 0,
        };
        self.applied_led = None;
        self.consecutive_meter_errors = 0;
        self.last_reason = format!("disconnected:{reason}");
        if reason != "manual_disconnect" && reason != "prepare_for_sleep" {
            self.reconnect_failures = 0;
            self.next_connect_at = self.clock.now_secs() + LINK_DROP_RECONNECT_DELAY_S;
        }
        self.emit(json!({
            "event": "cooler_disconnected",
            "reason": reason,
            "device_mac": self.client.device_mac,
            "device_name": self.client.device_name,
        }));
    }

    fn drain_ble_events(&mut self) {
        if let Some(reason) = self.client.drain_events() {
            self.on_ble_disconnected(&reason);
        }
    }

    pub async fn status(&mut self) -> StatusSnapshot {
        self.drain_ble_events();
        self.sample_inputs();
        self.publish_snapshot().await;
        self.build_snapshot()
    }

    pub async fn scan(&mut self, timeout_s: f64) -> Result<Vec<ScannedDevice>, String> {
        let mac = self.config.mac.clone();
        self.client.scan(timeout_s, &mac).await
    }

    pub async fn connect(&mut self, mac: &str) -> Result<Value, String> {
        if self.sleeping {
            return Err("系统正在进入睡眠/休眠，禁止连接水冷".to_string());
        }
        let target = if !mac.is_empty() {
            mac.to_ascii_uppercase()
        } else {
            self.config.mac.clone()
        };
        self.last_error.clear();
        let info = self.client.connect(&target, 6.0).await?;
        self.flow_monitor.on_disconnect();
        self.reconnect_failures = 0;
        self.next_connect_at = 0.0;
        self.consecutive_meter_errors = 0;
        self.actuators_dirty = true;
        self.led_dirty = true;
        self.last_meter_poll = self.clock.now_secs();
        self.apply_targets(true).await?;
        self.emit(json!({
            "event": "cooler_connected",
            "device_name": self.client.device_name,
            "device_mac": self.client.device_mac,
            "fw_version": self.client.fw_version,
            "meter_state": self.client.meter_state,
        }));
        let snap = self.status().await;
        let mut out = serde_json::to_value(&snap).map_err(|e| e.to_string())?;
        if let Some(obj) = out.as_object_mut() {
            obj.insert(
                "connect_info".to_string(),
                serde_json::to_value(&info).map_err(|e| e.to_string())?,
            );
        }
        Ok(out)
    }

    pub async fn disconnect(&mut self, send_sleep: Option<bool>) -> StatusSnapshot {
        let use_sleep = send_sleep.unwrap_or(self.config.sleep_on_disconnect);
        self.client.disconnect(use_sleep, true).await;
        self.on_ble_disconnected("manual_disconnect");
        self.last_reason = "disconnected".to_string();
        self.status().await
    }

    pub async fn emergency_stop(&mut self) -> Result<StatusSnapshot, String> {
        self.config.strategy = "preset".to_string();
        self.config.preset = "stop".to_string();
        self.config.led.mode = "off".to_string();
        self.config = protocol::save_config(&self.config, &self.config_path)?;
        self.disconnect(Some(true)).await;
        self.last_reason = "emergency_stop".to_string();
        Ok(self.status().await)
    }

    pub async fn reset_fault(&mut self) -> Result<StatusSnapshot, String> {
        self.flow_monitor.reset();
        self.actuators_dirty = true;
        self.last_error.clear();
        self.last_reason = "fault_reset".to_string();
        self.emit(json!({"event": "flow_fault_reset"}));
        if self.client.connected && !self.sleeping {
            self.apply_targets(true).await?;
        }
        Ok(self.status().await)
    }

    pub async fn set_config(&mut self, patch: &Value, apply_now: bool) -> Result<StatusSnapshot, String> {
        let canonical = protocol::merge_config_patch(&self.config, patch)?;
        self.config = protocol::save_config(&canonical, &self.config_path)?;
        self.actuators_dirty = true;
        self.led_dirty = true;
        if apply_now && self.client.connected && !self.sleeping {
            self.apply_targets(true).await?;
        }
        Ok(self.status().await)
    }

    pub async fn prepare_for_sleep(&mut self, entering: bool) -> StatusSnapshot {
        if entering {
            self.sleeping = true;
            if self.client.connected {
                self.client.disconnect(true, true).await;
                self.on_ble_disconnected("prepare_for_sleep");
            }
            self.last_reason = "sleeping".to_string();
        } else {
            self.sleeping = false;
            self.reconnect_failures = 0;
            self.next_connect_at = self.clock.now_secs() + WAKE_RECONNECT_DELAY_S;
            self.last_reason = "resumed".to_string();
        }
        self.status().await
    }

    pub async fn apply_targets(&mut self, force: bool) -> Result<(), String> {
        self.drain_ble_events();
        if !self.client.connected {
            return Ok(());
        }
        let now = self.clock.now_secs();
        self.sample_inputs();

        if self.flow_monitor.update(
            now,
            self.applied.pump_volt,
            self.client.meter_state,
            self.config.flow_protection,
        ) {
            let was_tripped = self.last_reason == "flow_fault_locked";
            self.last_error = self.flow_monitor.trip_reason.clone();
            if self.applied.pump_volt != 0 || self.applied.pump_duty != 0 || self.applied.fan_duty != 0 {
                if let Err(e) = self.client.apply_actuators(0, 0, 0).await {
                    self.last_error = format!("{}; 紧急停泵失败: {e}", self.flow_monitor.trip_reason);
                }
                self.applied = StageConfig {
                    pump_volt: 0,
                    pump_duty: 0,
                    fan_duty: 0,
                };
            }
            self.last_reason = "flow_fault_locked".to_string();
            self.actuators_dirty = false;
            if !was_tripped {
                self.emit(json!({
                    "event": "flow_fault_tripped",
                    "reason": self.flow_monitor.trip_reason,
                    "meter_state": self.client.meter_state,
                }));
            }
            self.publish_snapshot().await;
            return Ok(());
        }

        let (pv, pd, fd, new_tier, reason) = protocol::evaluate_cooling_target(
            &self.config,
            self.last_temp_c,
            Some(&self.last_ec_state),
            self.current_tier,
            self.flow_monitor.tripped,
            self.sleeping,
        );
        self.current_tier = new_tier;
        self.last_reason = reason;

        let target = StageConfig {
            pump_volt: pv,
            pump_duty: pd,
            fan_duty: fd,
        };
        if force || self.actuators_dirty || target != self.applied {
            self.client.apply_actuators(pv, pd, fd).await?;
            self.applied = target;
            self.actuators_dirty = false;
            self.flow_monitor.update(
                now,
                pv,
                self.client.meter_state,
                self.config.flow_protection,
            );
        }

        let led_sig = serde_json::to_string(&self.config.led).unwrap_or_default();
        if force || self.led_dirty || self.applied_led.as_deref() != Some(led_sig.as_str()) {
            self.client.apply_led(&self.config.led).await?;
            self.applied_led = Some(led_sig);
            self.led_dirty = false;
        }
        self.publish_snapshot().await;
        Ok(())
    }

    pub async fn tick(&mut self) {
        self.drain_ble_events();
        let now = self.clock.now_secs();
        if self.sleeping {
            self.publish_snapshot().await;
            return;
        }
        if !self.client.connected {
            if self.config.auto_connect && !self.flow_monitor.tripped && now >= self.next_connect_at {
                if let Err(e) = self.connect("").await {
                    let delay = RECONNECT_BACKOFF_S
                        [self.reconnect_failures.min(RECONNECT_BACKOFF_S.len() - 1)];
                    self.reconnect_failures += 1;
                    self.next_connect_at = self.clock.now_secs() + delay;
                    self.last_error = e.clone();
                    self.emit(json!({
                        "event": "cooler_connect_failed",
                        "attempt": self.reconnect_failures,
                        "next_retry_s": delay,
                        "error": e,
                    }));
                }
            }
            self.publish_snapshot().await;
            return;
        }

        let poll_interval = if self.applied.pump_volt > 0 { 2.0 } else { 6.0 };
        if now - self.last_meter_poll >= poll_interval {
            self.last_meter_poll = now;
            match self.client.query_meter().await {
                Ok(_) => {
                    self.consecutive_meter_errors = 0;
                    self.drain_ble_events();
                }
                Err(e) => {
                    self.last_error = format!("流量轮询异常: {e}");
                    self.consecutive_meter_errors += 1;
                    self.drain_ble_events();
                    self.emit(json!({
                        "event": "meter_poll_error",
                        "consecutive": self.consecutive_meter_errors,
                        "connected": self.client.connected,
                        "error": e,
                    }));
                    if !self.client.connected {
                        self.publish_snapshot().await;
                        return;
                    }
                    if self.consecutive_meter_errors >= MAX_CONSECUTIVE_METER_ERRORS {
                        self.client.disconnect(false, false).await;
                        self.on_ble_disconnected("meter_poll_failed");
                        self.publish_snapshot().await;
                        return;
                    }
                }
            }
        }

        if let Err(e) = self.apply_targets(false).await {
            self.last_error = e.clone();
            self.drain_ble_events();
            self.emit(json!({
                "event": "apply_targets_error",
                "connected": self.client.connected,
                "error": e,
            }));
        }
        self.publish_snapshot().await;
    }

    pub async fn dispatch(&mut self, req: &Value) -> Result<Value, String> {
        let op = req
            .get("op")
            .and_then(Value::as_str)
            .ok_or_else(|| "请求缺少 op 字段".to_string())?;
        match op {
            "ping" => Ok(json!({"pong": true, "connected": self.client.connected})),
            "status" => serde_json::to_value(self.status().await).map_err(|e| e.to_string()),
            "scan" => {
                let timeout_s = req
                    .get("timeout_s")
                    .and_then(Value::as_f64)
                    .unwrap_or(5.0);
                let devices = self.scan(timeout_s).await?;
                Ok(json!({"devices": devices}))
            }
            "connect" => {
                let mac = req.get("mac").and_then(Value::as_str).unwrap_or("");
                self.connect(mac).await
            }
            "disconnect" => {
                let send_sleep = req.get("send_sleep").and_then(Value::as_bool);
                serde_json::to_value(self.disconnect(send_sleep).await).map_err(|e| e.to_string())
            }
            "emergency_stop" => {
                serde_json::to_value(self.emergency_stop().await?).map_err(|e| e.to_string())
            }
            "reset_fault" => {
                serde_json::to_value(self.reset_fault().await?).map_err(|e| e.to_string())
            }
            "set_config" => {
                let patch = req
                    .get("config")
                    .ok_or_else(|| "配置更新必须是 JSON 对象".to_string())?;
                let apply_now = req
                    .get("apply_now")
                    .and_then(Value::as_bool)
                    .unwrap_or(true);
                serde_json::to_value(self.set_config(patch, apply_now).await?)
                    .map_err(|e| e.to_string())
            }
            "prepare_sleep" => {
                let entering = req
                    .get("entering")
                    .and_then(Value::as_bool)
                    .unwrap_or(true);
                serde_json::to_value(self.prepare_for_sleep(entering).await)
                    .map_err(|e| e.to_string())
            }
            other => Err(format!("不支持的水冷命令: {other}")),
        }
    }

    pub fn default_snapshot(cfg: &CoolerConfig) -> StatusSnapshot {
        StatusSnapshot {
            service_running: true,
            connected: false,
            sleeping: false,
            device_name: String::new(),
            device_mac: String::new(),
            fw_version: None,
            meter_state: None,
            flow_tripped: false,
            flow_trip_reason: String::new(),
            applied: StageConfig {
                pump_volt: 0,
                pump_duty: 0,
                fan_duty: 0,
            },
            current_tier: 0,
            reason: "idle".to_string(),
            last_error: String::new(),
            temp_c: None,
            ec: EcWatercoolerState::default(),
            config: cfg.clone(),
        }
    }
}

pub fn load_config_or_default(path: &Path) -> CoolerConfig {
    protocol::load_config(path).unwrap_or_default()
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::ble::{BleEvent, BoxFuture, TransportConnectInfo};
    use std::sync::Mutex as StdMutex;
    use std::time::Duration;
    use tokio::sync::mpsc;

    struct FakeClock {
        now: StdMutex<f64>,
    }

    impl FakeClock {
        fn new(v: f64) -> Arc<Self> {
            Arc::new(Self {
                now: StdMutex::new(v),
            })
        }
        fn advance(&self, s: f64) {
            *self.now.lock().unwrap() += s;
        }
    }

    impl Clock for FakeClock {
        fn now_secs(&self) -> f64 {
            *self.now.lock().unwrap()
        }
    }

    struct FakeTransport {
        connect_calls: usize,
        fail_connects: usize,
        fail_write: Option<String>,
        next_meter: u8,
        writes: Vec<(String, Vec<u8>)>,
        events: Option<mpsc::UnboundedSender<BleEvent>>,
    }

    impl FakeTransport {
        fn new() -> Self {
            Self {
                connect_calls: 0,
                fail_connects: 0,
                fail_write: None,
                next_meter: 2,
                writes: Vec::new(),
                events: None,
            }
        }
    }

    impl BleTransport for FakeTransport {
        fn scan<'a>(
            &'a mut self,
            _timeout_s: f64,
            target_mac: &'a str,
        ) -> BoxFuture<'a, Result<Vec<ScannedDevice>, String>> {
            Box::pin(async move {
                Ok(vec![ScannedDevice {
                    path: "/org/bluez/hci0/dev_E2_90_B5_F8_43_2E".to_string(),
                    name: "CoolingSystem LCT22002".to_string(),
                    address: if target_mac.is_empty() {
                        "E2:90:B5:F8:43:2E".to_string()
                    } else {
                        target_mac.to_string()
                    },
                    rssi: Some(-55),
                    connected: false,
                    paired: false,
                }])
            })
        }

        fn connect<'a>(
            &'a mut self,
            target_mac: &'a str,
            _scan_timeout_s: f64,
            events: mpsc::UnboundedSender<BleEvent>,
        ) -> BoxFuture<'a, Result<TransportConnectInfo, String>> {
            Box::pin(async move {
                self.connect_calls += 1;
                if self.fail_connects > 0 {
                    self.fail_connects -= 1;
                    return Err("未扫描到机械革命蓝牙水冷设备".to_string());
                }
                self.events = Some(events);
                Ok(TransportConnectInfo {
                    name: "CoolingSystem LCT22002".to_string(),
                    address: if target_mac.is_empty() {
                        "E2:90:B5:F8:43:2E".to_string()
                    } else {
                        target_mac.to_string()
                    },
                })
            })
        }

        fn write_frame<'a>(
            &'a mut self,
            data: &'a [u8],
            write_type: &'a str,
        ) -> BoxFuture<'a, Result<(), String>> {
            Box::pin(async move {
                if let Some(err) = &self.fail_write {
                    return Err(err.clone());
                }
                self.writes.push((write_type.to_string(), data.to_vec()));
                if let Some(tx) = &self.events {
                    if data == b"sw" {
                        let _ = tx.send(BleEvent::Notify(b"MCU F/W Version: 2.0.0.4\r\n".to_vec()));
                    } else if data.len() == 8 && data[0] == 0xFE && data[1] == 0x32 {
                        let _ = tx.send(BleEvent::Notify(vec![0xFE, 0x32, 0x05, self.next_meter, 0xEF]));
                    }
                }
                Ok(())
            })
        }

        fn disconnect<'a>(&'a mut self) -> BoxFuture<'a, ()> {
            Box::pin(async move {
                self.events = None;
            })
        }
    }

    #[tokio::test]
    async fn test_coordinator_connect_disconnect_and_fast_reconnect() {
        let clock = FakeClock::new(100.0);
        let cfg = protocol::validate_config_value(&json!({
            "auto_connect": true,
            "mac": "E2:90:B5:F8:43:2E",
            "preset": "balanced"
        }))
        .unwrap();
        let snap = Arc::new(RwLock::new(CoolerCoordinator::<FakeTransport>::default_snapshot(&cfg)));
        let client = CoolerBleClient::new(FakeTransport::new(), Duration::from_millis(1));
        let tmp = std::env::temp_dir().join(format!("iteglow-rs-test-{}.json", std::process::id()));
        let mut coord = CoolerCoordinator::new(client, cfg, tmp.clone(), clock.clone(), snap, None);
        coord.set_telemetry_paths(
            PathBuf::from("/nonexistent-hwmon"),
            PathBuf::from("/nonexistent-wmi"),
            PathBuf::from("/nonexistent-ec"),
        );

        coord.tick().await;
        assert!(coord.client.connected);
        assert_eq!(coord.applied.pump_volt, 8);
        assert_eq!(coord.applied.fan_duty, 50);

        // Simulate sudden NotConnected write error -> must preserve "request" mode & schedule 2s reconnect
        coord.client.transport.fail_write =
            Some("GDBus.Error:org.bluez.Error.NotConnected: Not Connected".to_string());
        clock.advance(2.1);
        coord.tick().await;
        assert!(!coord.client.connected);
        assert_eq!(coord.client.preferred_write_type, "request");

        coord.client.transport.fail_write = None;
        clock.advance(1.0);
        coord.tick().await;
        assert!(!coord.client.connected);
        clock.advance(1.2);
        coord.tick().await;
        assert!(coord.client.connected);

        // Wake from sleep waits WAKE_RECONNECT_DELAY_S
        coord.prepare_for_sleep(true).await;
        assert!(!coord.client.connected);
        coord.prepare_for_sleep(false).await;
        clock.advance(2.0);
        coord.tick().await;
        assert!(!coord.client.connected);
        clock.advance(0.7);
        coord.tick().await;
        assert!(coord.client.connected);

        let _ = std::fs::remove_file(tmp);
    }

    #[tokio::test]
    async fn test_coordinator_backoff_and_flow_fault() {
        let clock = FakeClock::new(200.0);
        let cfg = protocol::validate_config_value(&json!({"auto_connect": true, "preset": "silent"})).unwrap();
        let snap = Arc::new(RwLock::new(CoolerCoordinator::<FakeTransport>::default_snapshot(&cfg)));
        let mut tr = FakeTransport::new();
        tr.fail_connects = 2;
        let client = CoolerBleClient::new(tr, Duration::from_millis(1));
        let tmp = std::env::temp_dir().join(format!("iteglow-rs-flow-{}.json", std::process::id()));
        let mut coord = CoolerCoordinator::new(client, cfg, tmp.clone(), clock.clone(), snap, None);
        coord.set_telemetry_paths(
            PathBuf::from("/nonexistent-hwmon"),
            PathBuf::from("/nonexistent-wmi"),
            PathBuf::from("/nonexistent-ec"),
        );

        // Failure #1 -> backoff 3s
        coord.tick().await;
        assert!(!coord.client.connected);
        clock.advance(2.5);
        coord.tick().await;
        assert_eq!(coord.client.transport.connect_calls, 1);
        // Failure #2 -> backoff 5s
        clock.advance(0.6);
        coord.tick().await;
        assert_eq!(coord.client.transport.connect_calls, 2);
        // Succeeds after 5s
        clock.advance(5.1);
        coord.tick().await;
        assert!(coord.client.connected);

        // Simulate flow loss -> trips flow safety monitor
        coord.client.transport.next_meter = 1;
        for _ in 0..8 {
            clock.advance(1.1);
            coord.tick().await;
        }
        assert!(coord.flow_monitor.tripped);
        assert_eq!(coord.applied.pump_volt, 0);

        let _ = std::fs::remove_file(tmp);
    }
}

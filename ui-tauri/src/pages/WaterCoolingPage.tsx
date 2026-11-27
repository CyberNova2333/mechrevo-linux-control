// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

import { useEffect, useRef, useState } from "react";
import { api } from "../api/tauri";
import {
  hexToRgb,
  rgbCss,
  rgbToHex,
  type Rgb,
  type WaterCoolingConfig,
  type WaterCoolingSnapshot,
} from "../types/hardware";

const STRATEGY_ITEMS = [
  { id: "preset", label: "预设模式" },
  { id: "manual", label: "手动固定" },
  { id: "smart_curve", label: "智能温控曲线" },
  { id: "ec_follow", label: "跟随主机 EC" },
] as const;

const PRESET_ITEMS = [
  { id: "stop", label: "停机 (0V / 0%)" },
  { id: "silent", label: "静音 (7V · 泵60% · 扇40%)" },
  { id: "balanced", label: "均衡 (8V · 泵60% · 扇50%)" },
  { id: "performance", label: "性能 (11V · 泵60% · 扇60%)" },
  { id: "turbo", label: "狂暴 (11V · 泵90% · 扇90%)" },
  { id: "max", label: "全速 (12V · 泵100% · 扇100%)" },
] as const;

const VOLT_ITEMS = [
  { v: 0, label: "0V (停泵)" },
  { v: 7, label: "7V" },
  { v: 8, label: "8V" },
  { v: 11, label: "11V" },
  { v: 12, label: "12V" },
];

const LED_MODES = [
  { id: "off", label: "关闭灯光" },
  { id: "static", label: "静态单色" },
  { id: "breathing", label: "机身呼吸" },
  { id: "colorful", label: "机身幻彩" },
  { id: "breathing_colorful", label: "机身呼吸幻彩" },
  { id: "mode4", label: "全区静态" },
] as const;

const FAN_POLICIES = [
  { id: "sync", label: "双区同步 (风扇=机身)" },
  { id: "body_only", label: "仅机身灯亮 (风扇关)" },
  { id: "freeze", label: "锁定风扇当前灯态" },
  { id: "split", label: "双区独立双色" },
] as const;

const DEFAULT_WC: WaterCoolingConfig = {
  version: 1,
  auto_connect: false,
  mac: "",
  strategy: "preset",
  preset: "balanced",
  manual: { pump_volt: 7, pump_duty: 60, fan_duty: 40 },
  curve: {
    hysteresis_c: 3,
    points: [
      { temp_c: 45, pump_volt: 7, pump_duty: 60, fan_duty: 40 },
      { temp_c: 65, pump_volt: 8, pump_duty: 70, fan_duty: 55 },
      { temp_c: 80, pump_volt: 11, pump_duty: 90, fan_duty: 85 },
    ],
  },
  led: {
    mode: "static",
    color: [0, 128, 255],
    fan_policy: "sync",
    fan_color: [0, 128, 255],
  },
  flow_protection: true,
  sleep_on_disconnect: true,
};

function CoolerRigSvg({
  led,
  snapshot,
}: {
  led: WaterCoolingConfig["led"];
  snapshot: WaterCoolingSnapshot | null;
}) {
  const bodyEff: Rgb | null =
    led.mode === "off"
      ? null
      : led.mode === "colorful" || led.mode === "breathing_colorful"
        ? [56, 189, 248]
        : led.color;

  const fanEff: Rgb | null =
    led.mode === "off" || led.fan_policy === "body_only"
      ? null
      : led.fan_policy === "sync"
        ? bodyEff
        : led.fan_policy === "split"
          ? led.fan_color
          : [115, 143, 184];

  const fanDuty = snapshot?.connected ? snapshot.applied?.fan_duty ?? 0 : 0;
  const fanDur = fanDuty > 0 ? `${Math.max(0.55, 3.6 - (fanDuty / 100) * 2.9).toFixed(2)}s` : "0s";
  const statusDot = snapshot?.flow_tripped
    ? "#fb7185"
    : snapshot?.connected
      ? "#34d399"
      : "#38bdf8";

  return (
    <div
      style={{
        width: 244,
        flexShrink: 0,
        background: "#090c12",
        border: "1px solid var(--border-seam)",
        borderRadius: 12,
        padding: "12px 12px 10px",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        gap: 6,
      }}
    >
      <svg width="218" height="162" viewBox="0 0 218 162" fill="none">
        <rect x="6" y="8" width="206" height="142" rx="14" fill="#111722" stroke="#253145" strokeWidth="1.2" />
        <circle cx="20" cy="22" r="3.4" fill={statusDot} />
        {[0, 1, 2, 3, 4, 5].map((i) => (
          <rect key={i} x="16" y={36 + i * 16} width="26" height="6" rx="3" fill="#1b2434" />
        ))}

        {/* Fan illuminated ring */}
        <g transform="translate(122, 79)">
          {fanEff && (
            <circle cx="0" cy="0" r="49" stroke={rgbCss(fanEff, 0.22)} strokeWidth="8" />
          )}
          <circle
            cx="0"
            cy="0"
            r="44"
            fill="#0a0e16"
            stroke={fanEff ? rgbCss(fanEff) : "#2a3548"}
            strokeWidth={fanEff ? "3.4" : "2"}
          />
          <g
            className={fanDuty > 0 ? "fan-rotor" : undefined}
            style={{ "--fan-dur": fanDur } as React.CSSProperties}
          >
            {[0, 1, 2, 3, 4, 5, 6].map((k) => {
              const a = (k * 2 * Math.PI) / 7;
              return (
                <path
                  key={k}
                  d={`M ${Math.cos(a) * 9} ${Math.sin(a) * 9} Q ${Math.cos(a + 0.22) * 26} ${
                    Math.sin(a + 0.22) * 26
                  } ${Math.cos(a + 0.42) * 38} ${Math.sin(a + 0.42) * 38}`}
                  stroke="#334155"
                  strokeWidth="3.2"
                  strokeLinecap="round"
                />
              );
            })}
          </g>
          <circle cx="0" cy="0" r="9.5" fill="#1f2a3c" stroke="#334155" strokeWidth="1.2" />
        </g>

        {/* Chassis side lightbar */}
        {bodyEff && (
          <rect x="187" y="23" width="14" height="112" rx="7" fill={rgbCss(bodyEff, 0.24)} />
        )}
        <rect
          x="190"
          y="26"
          width="8"
          height="106"
          rx="4"
          fill={bodyEff ? rgbCss(bodyEff) : "#263143"}
        />
      </svg>
      <span style={{ fontSize: 11, color: "var(--ink-muted)" }}>
        LCT22002 双区灯效与风轮实时预览
      </span>
    </div>
  );
}

function SmartCurveSvg({
  points,
  liveTemp,
}: {
  points: WaterCoolingConfig["curve"]["points"];
  liveTemp?: number | null;
}) {
  const w = 440;
  const h = 132;
  const left = 34;
  const right = w - 14;
  const top = 12;
  const bottom = h - 22;
  const sorted = [...points].sort((a, b) => a.temp_c - b.temp_c);
  const mapX = (t: number) => left + ((Math.max(25, Math.min(100, t)) - 25) / 75) * (right - left);
  const mapY = (p: number) => bottom - (Math.max(0, Math.min(100, p)) / 100) * (bottom - top);

  const pumpPts = sorted.map((p) => `${mapX(p.temp_c)},${mapY(p.pump_duty)}`).join(" ");
  const fanPts = sorted.map((p) => `${mapX(p.temp_c)},${mapY(p.fan_duty)}`).join(" ");

  return (
    <svg
      viewBox={`0 0 ${w} ${h}`}
      style={{
        width: "100%",
        height: 136,
        background: "#090c12",
        borderRadius: 9,
        border: "1px solid var(--border-seam)",
      }}
    >
      {[0, 50, 100].map((pct) => {
        const y = mapY(pct);
        return (
          <g key={pct}>
            <line x1={left} y1={y} x2={right} y2={y} stroke="#1a2230" strokeWidth="1" />
            <text x={4} y={y + 3} fill="#64748b" fontSize="9.5" className="mono">
              {pct}%
            </text>
          </g>
        );
      })}
      {[30, 50, 70, 90].map((t) => (
        <text key={t} x={mapX(t) - 10} y={h - 6} fill="#64748b" fontSize="9.5" className="mono">
          {t}°C
        </text>
      ))}
      {typeof liveTemp === "number" && (
        <line
          x1={mapX(liveTemp)}
          y1={top}
          x2={mapX(liveTemp)}
          y2={bottom}
          stroke="#fbbf24"
          strokeWidth="1.2"
          strokeDasharray="3 2"
        />
      )}
      <polyline points={pumpPts} fill="none" stroke="#818cf8" strokeWidth="2.2" />
      <polyline points={fanPts} fill="none" stroke="#38bdf8" strokeWidth="2.2" />
      {sorted.map((p, i) => (
        <g key={i}>
          <circle cx={mapX(p.temp_c)} cy={mapY(p.pump_duty)} r="3.2" fill="#818cf8" />
          <circle cx={mapX(p.temp_c)} cy={mapY(p.fan_duty)} r="3.2" fill="#38bdf8" />
        </g>
      ))}
    </svg>
  );
}

export function WaterCoolingPage() {
  const [cfg, setCfg] = useState<WaterCoolingConfig>(DEFAULT_WC);
  const [snap, setSnap] = useState<WaterCoolingSnapshot | null>(null);
  const [dirty, setDirty] = useState<boolean>(false);
  const [busy, setBusy] = useState<boolean>(false);
  const busyRef = useRef(false);
  busyRef.current = busy;
  const [msg, setMsg] = useState<string>("正在读取水冷守护状态…");
  const [isError, setIsError] = useState<boolean>(false);
  const dirtyRef = useRef(false);
  dirtyRef.current = dirty;

  function updateCfg(updater: (prev: WaterCoolingConfig) => WaterCoolingConfig) {
    setDirty(true);
    setCfg(updater);
  }

  async function pollStatus(forceConfigSync: boolean): Promise<boolean> {
    try {
      const res = await api.waterCoolingRequest<WaterCoolingSnapshot>({ op: "status" });
      setSnap(res);
      if (res.config && (forceConfigSync || !dirtyRef.current)) {
        setCfg(res.config);
      }
      setIsError(false);
      return true;
    } catch (err) {
      setMsg(`读取水冷后台服务失败：${String(err)}`);
      setIsError(true);
      return false;
    }
  }

  useEffect(() => {
    api
      .getWaterCoolingLocalConfig()
      .then((local) => {
        if (!dirtyRef.current) setCfg(local);
      })
      .catch(() => {});
    pollStatus(true).then((ok) => {
      if (ok) setMsg("已连接水冷 Rust 守护进程");
    });
    const timer = window.setInterval(() => {
      if (!busyRef.current) pollStatus(false);
    }, 3000);
    return () => window.clearInterval(timer);
  }, []);

  async function sendOp(label: string, req: Record<string, unknown>, clearDirty = false) {
    setBusy(true);
    setMsg(label);
    setIsError(false);
    try {
      const res = await api.waterCoolingRequest<Record<string, unknown>>(req);
      if (req.op === "scan") {
        const devs = (res.devices as Array<{ name?: string; address?: string; rssi?: number }>) || [];
        if (devs.length === 0) {
          setMsg("未发现周边 MECHREVO 蓝牙水冷设备");
        } else {
          if (!cfg.mac && devs[0].address) {
            updateCfg((p) => ({ ...p, mac: devs[0].address || "" }));
          }
          setMsg(
            `发现设备：${devs
              .map((d) => `${d.name ?? "LCT"} (${d.address ?? "--"}, RSSI=${d.rssi ?? "--"})`)
              .join("；")}`
          );
        }
      } else if ("saved_offline" in res) {
        if (clearDirty) setDirty(false);
        setMsg("水冷守护未运行，配置已保存至本地文件");
      } else {
        const nextSnap = res as unknown as WaterCoolingSnapshot;
        setSnap(nextSnap);
        if (nextSnap.config && clearDirty) {
          setCfg(nextSnap.config);
          setDirty(false);
        }
        setMsg(clearDirty ? "配置已保存并应用至水冷后台服务" : "操作已完成，状态已同步");
      }
    } catch (err) {
      setMsg(`操作失败：${String(err)}`);
      setIsError(true);
    } finally {
      setBusy(false);
    }
  }

  const pillVariant = snap?.sleeping
    ? "warn"
    : snap?.flow_tripped
      ? "danger"
      : snap?.connected
        ? "ok"
        : "";
  const pillText = snap?.sleeping
    ? "系统睡眠暂停"
    : snap?.flow_tripped
      ? "断流锁定"
      : snap?.connected
        ? "已连接"
        : "未连接";

  const meter = snap?.meter_state;
  const flowShort =
    meter === 2 ? "正常循环" : typeof meter === "number" ? `无水流 (${meter})` : "待机";
  const applied = snap?.applied ?? { pump_volt: 0, pump_duty: 0, fan_duty: 0 };
  const tempText = typeof snap?.temp_c === "number" ? `${snap.temp_c.toFixed(1)}°C` : "--°C";
  const ec = snap?.ec;
  const ecText = ec?.ec_available
    ? `EC 遥测 (${ec.ec_stale ? "过期" : "实时"})：${ec.lc_supported ? "已支持" : "未确认"} · 泵 ${
        ec.ec_pump_enabled ? `${ec.ec_pump_volt}V` : "关"
      } · 扇 ${ec.ec_fan_enabled ? `${ec.ec_fan_duty}%` : "关"} · WMI C1 seq=${ec.wmi_c1_seq ?? 0}`
    : `EC 遥测：等待守护数据 · WMI ${ec?.wmi_available ? "就绪" : "无信号"}`;
  const faultText = snap?.flow_trip_reason || snap?.last_error || "";

  return (
    <div className="page-stack">
      {/* Hero Telemetry Deck */}
      <section className="hero-stage">
        <div className="section-header">
          <div>
            <h2 className="section-title">
              {snap?.device_name || "待连接外置蓝牙水冷"}
            </h2>
            <p className="section-desc mono">
              {snap?.device_mac || "--"} · 固件 {snap?.fw_version || "--"} · {snap?.reason || "idle"}
            </p>
          </div>
          <span className={`pill ${pillVariant}`}>
            <span className={`dot ${pillVariant || "muted"}`} />
            {pillText}
          </span>
        </div>

        <div style={{ display: "flex", gap: 16, alignItems: "stretch", flexWrap: "wrap" }}>
          <div style={{ flex: 1, minWidth: 320, display: "flex", flexDirection: "column", gap: 12 }}>
            <div className="kpi-strip">
              <div className="kpi-box">
                <div className="kpi-label">水流回路</div>
                <div className="kpi-value">{flowShort}</div>
              </div>
              <div className="kpi-box">
                <div className="kpi-label">水泵输出</div>
                <div className="kpi-value mono">
                  {applied.pump_volt}V · {applied.pump_duty}%
                </div>
              </div>
              <div className="kpi-box">
                <div className="kpi-label">冷排风扇</div>
                <div className="kpi-value mono">{applied.fan_duty}%</div>
              </div>
              <div className="kpi-box">
                <div className="kpi-label">CPU 封装</div>
                <div className="kpi-value mono">{tempText}</div>
              </div>
            </div>

            <div className="well mono" style={{ fontSize: 11.5, color: "var(--ink-secondary)" }}>
              {ecText}
            </div>

            {(snap?.flow_tripped || faultText) && (
              <div className="status-bar error">安全告警：{faultText}</div>
            )}

            <div style={{ display: "flex", flexWrap: "wrap", gap: 8 }}>
              <button
                type="button"
                className="btn btn-primary btn-sm"
                disabled={busy}
                onClick={() => sendOp("正在连接水冷设备…", { op: "connect", mac: cfg.mac.trim() })}
              >
                连接水冷
              </button>
              <button
                type="button"
                className="btn btn-sm"
                disabled={busy}
                onClick={() => sendOp("正在扫描蓝牙水冷 (4s)…", { op: "scan", timeout_s: 4.0 })}
              >
                扫描设备
              </button>
              <button
                type="button"
                className="btn btn-sm"
                disabled={busy}
                onClick={() => sendOp("正在安全停泵并断开…", { op: "disconnect" })}
              >
                安全断开
              </button>
              <button
                type="button"
                className="btn btn-sm"
                disabled={busy}
                onClick={() => sendOp("正在刷新水冷状态…", { op: "status" })}
              >
                刷新
              </button>
              <button
                type="button"
                className="btn btn-sm"
                disabled={busy}
                onClick={() => sendOp("正在重置水流保护锁定…", { op: "reset_fault" })}
              >
                重置故障
              </button>
              <button
                type="button"
                className="btn btn-danger btn-sm"
                disabled={busy}
                onClick={() => sendOp("正在执行紧急停泵…", { op: "emergency_stop" })}
              >
                紧急停泵
              </button>
            </div>
          </div>

          <CoolerRigSvg led={cfg.led} snapshot={snap} />
        </div>
      </section>

      {/* Lower 2-Column Workspace */}
      <div className="split-equal">
        {/* Left: Thermal Strategy */}
        <section className="panel">
          <div>
            <h3 className="section-title">水泵与风扇散热策略</h3>
            <p className="section-desc">按预设档位、固定电压、温控曲线或 EC 遥测运行</p>
          </div>

          <div className="field-row">
            <span className="field-label">控制策略</span>
            <select
              className="select"
              value={cfg.strategy}
              onChange={(e) =>
                updateCfg((p) => ({
                  ...p,
                  strategy: e.target.value as WaterCoolingConfig["strategy"],
                }))
              }
            >
              {STRATEGY_ITEMS.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.label}
                </option>
              ))}
            </select>
          </div>

          {cfg.strategy === "preset" && (
            <div className="field-row well">
              <span className="field-label">固定预设档位</span>
              <select
                className="select"
                value={cfg.preset}
                onChange={(e) =>
                  updateCfg((p) => ({
                    ...p,
                    preset: e.target.value as WaterCoolingConfig["preset"],
                  }))
                }
              >
                {PRESET_ITEMS.map((pr) => (
                  <option key={pr.id} value={pr.id}>
                    {pr.label}
                  </option>
                ))}
              </select>
            </div>
          )}

          {cfg.strategy === "manual" && (
            <div className="well" style={{ display: "flex", flexDirection: "column", gap: 12 }}>
              <div className="field-row">
                <span className="field-label">水泵供电电压</span>
                <select
                  className="select mono"
                  value={cfg.manual.pump_volt}
                  onChange={(e) =>
                    updateCfg((p) => ({
                      ...p,
                      manual: { ...p.manual, pump_volt: Number(e.target.value) },
                    }))
                  }
                >
                  {VOLT_ITEMS.map((v) => (
                    <option key={v.v} value={v.v}>
                      {v.label}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <div className="field-row" style={{ marginBottom: 4 }}>
                  <span className="field-label">水泵占空比</span>
                  <span className="mono">{cfg.manual.pump_duty}%</span>
                </div>
                <input
                  type="range"
                  min={0}
                  max={100}
                  value={cfg.manual.pump_duty}
                  onChange={(e) =>
                    updateCfg((p) => ({
                      ...p,
                      manual: { ...p.manual, pump_duty: Number(e.target.value) },
                    }))
                  }
                />
              </div>
              <div>
                <div className="field-row" style={{ marginBottom: 4 }}>
                  <span className="field-label">冷排风扇占空比</span>
                  <span className="mono">{cfg.manual.fan_duty}%</span>
                </div>
                <input
                  type="range"
                  min={0}
                  max={100}
                  value={cfg.manual.fan_duty}
                  onChange={(e) =>
                    updateCfg((p) => ({
                      ...p,
                      manual: { ...p.manual, fan_duty: Number(e.target.value) },
                    }))
                  }
                />
              </div>
            </div>
          )}

          {cfg.strategy === "smart_curve" && (
            <div className="well" style={{ display: "flex", flexDirection: "column", gap: 10 }}>
              <SmartCurveSvg points={cfg.curve.points} liveTemp={snap?.temp_c} />
              <div style={{ display: "flex", gap: 14, fontSize: 11, color: "var(--ink-muted)" }}>
                <span>
                  <span className="dot" style={{ background: "#818cf8", marginRight: 5 }} />
                  水泵占空比
                </span>
                <span>
                  <span className="dot" style={{ background: "#38bdf8", marginRight: 5 }} />
                  冷排风扇
                </span>
                <span>
                  <span className="dot" style={{ background: "#fbbf24", marginRight: 5 }} />
                  实时 CPU 温度
                </span>
              </div>

              <div className="field-row">
                <span className="field-label">降档滞回温度 (°C)</span>
                <input
                  type="number"
                  className="input input-num"
                  min={1}
                  max={15}
                  value={cfg.curve.hysteresis_c}
                  onChange={(e) =>
                    updateCfg((p) => ({
                      ...p,
                      curve: {
                        ...p.curve,
                        hysteresis_c: Math.max(1, Math.min(15, Math.round(Number(e.target.value) || 3))),
                      },
                    }))
                  }
                />
              </div>

              {cfg.curve.points.map((pt, idx) => (
                <div
                  key={idx}
                  style={{
                    display: "grid",
                    gridTemplateColumns: "34px 66px 76px 66px 66px",
                    gap: 6,
                    alignItems: "center",
                    fontSize: 11.5,
                  }}
                >
                  <span className="mono" style={{ color: "var(--ink-muted)" }}>
                    T{idx + 1}
                  </span>
                  <input
                    type="number"
                    className="input input-num"
                    style={{ width: "100%", padding: "4px 6px" }}
                    min={25}
                    max={100}
                    title="触发温度 (°C)"
                    value={pt.temp_c}
                    onChange={(e) => {
                      const nextPts = cfg.curve.points.slice();
                      nextPts[idx] = { ...pt, temp_c: Number(e.target.value) || 25 };
                      updateCfg((p) => ({ ...p, curve: { ...p.curve, points: nextPts } }));
                    }}
                  />
                  <select
                    className="select mono"
                    style={{ padding: "4px 6px" }}
                    value={pt.pump_volt}
                    onChange={(e) => {
                      const nextPts = cfg.curve.points.slice();
                      nextPts[idx] = { ...pt, pump_volt: Number(e.target.value) };
                      updateCfg((p) => ({ ...p, curve: { ...p.curve, points: nextPts } }));
                    }}
                  >
                    {VOLT_ITEMS.map((v) => (
                      <option key={v.v} value={v.v}>
                        {v.v}V
                      </option>
                    ))}
                  </select>
                  <input
                    type="number"
                    className="input input-num"
                    style={{ width: "100%", padding: "4px 6px" }}
                    min={0}
                    max={100}
                    title="水泵占空比 (%)"
                    value={pt.pump_duty}
                    onChange={(e) => {
                      const nextPts = cfg.curve.points.slice();
                      nextPts[idx] = { ...pt, pump_duty: Number(e.target.value) || 0 };
                      updateCfg((p) => ({ ...p, curve: { ...p.curve, points: nextPts } }));
                    }}
                  />
                  <input
                    type="number"
                    className="input input-num"
                    style={{ width: "100%", padding: "4px 6px" }}
                    min={0}
                    max={100}
                    title="风扇占空比 (%)"
                    value={pt.fan_duty}
                    onChange={(e) => {
                      const nextPts = cfg.curve.points.slice();
                      nextPts[idx] = { ...pt, fan_duty: Number(e.target.value) || 0 };
                      updateCfg((p) => ({ ...p, curve: { ...p.curve, points: nextPts } }));
                    }}
                  />
                </div>
              ))}
            </div>
          )}

          {cfg.strategy === "ec_follow" && (
            <div className="well field-hint">
              跟随主机 EC 只读遥测 (0x7E6/0x7E7 & WMI 0xC1)；若遥测过期或不可用，自动回退至静音档 (7V / 60% / 40%)。
            </div>
          )}
        </section>

        {/* Right: Dual-Zone LED & Safety */}
        <div style={{ display: "flex", flexDirection: "column", gap: 16 }}>
          <section className="panel">
            <div>
              <h3 className="section-title">双区灯光（风扇 + 机身）</h3>
              <p className="section-desc">蓝牙指示灯由固件独占；风扇区通过锁存与同步实现分控</p>
            </div>

            <div className="field-row">
              <span className="field-label">灯光效果模式</span>
              <select
                className="select"
                value={cfg.led.mode}
                onChange={(e) =>
                  updateCfg((p) => ({
                    ...p,
                    led: { ...p.led, mode: e.target.value as WaterCoolingConfig["led"]["mode"] },
                  }))
                }
              >
                {LED_MODES.map((m) => (
                  <option key={m.id} value={m.id}>
                    {m.label}
                  </option>
                ))}
              </select>
            </div>

            <div className="field-row">
              <span className="field-label">风扇区联动策略</span>
              <select
                className="select"
                value={cfg.led.fan_policy}
                onChange={(e) =>
                  updateCfg((p) => ({
                    ...p,
                    led: {
                      ...p.led,
                      fan_policy: e.target.value as WaterCoolingConfig["led"]["fan_policy"],
                    },
                  }))
                }
              >
                {FAN_POLICIES.map((fp) => (
                  <option key={fp.id} value={fp.id}>
                    {fp.label}
                  </option>
                ))}
              </select>
            </div>

            <div className="field-row">
              <span className="field-label">机身 / 主灯颜色</span>
              <input
                type="color"
                className="color-input"
                value={rgbToHex(cfg.led.color)}
                onChange={(e) =>
                  updateCfg((p) => ({
                    ...p,
                    led: { ...p.led, color: hexToRgb(e.target.value) },
                  }))
                }
              />
            </div>

            {cfg.led.fan_policy === "split" && (
              <div className="field-row">
                <span className="field-label">风扇灯独立颜色 (split)</span>
                <input
                  type="color"
                  className="color-input"
                  value={rgbToHex(cfg.led.fan_color)}
                  onChange={(e) =>
                    updateCfg((p) => ({
                      ...p,
                      led: { ...p.led, fan_color: hexToRgb(e.target.value) },
                    }))
                  }
                />
              </div>
            )}
          </section>

          <section className="panel">
            <div>
              <h3 className="section-title">保护机制与设备绑定</h3>
              <p className="section-desc">推荐绑定 MAC 避免并发扫描漏扫</p>
            </div>

            <div className="field-row">
              <span className="field-label">目标 MAC</span>
              <input
                type="text"
                className="input mono"
                style={{ width: 210 }}
                placeholder="留空自动匹配 LCT22002"
                value={cfg.mac}
                onChange={(e) => updateCfg((p) => ({ ...p, mac: e.target.value }))}
              />
            </div>

            <div className="field-row well">
              <div>
                <div className="field-label">后台自动连接</div>
                <div className="field-hint">非睡眠状态下自动重连并恢复温控策略</div>
              </div>
              <label className="switch">
                <input
                  type="checkbox"
                  checked={cfg.auto_connect}
                  onChange={(e) => updateCfg((p) => ({ ...p, auto_connect: e.target.checked }))}
                />
                <span className="switch-track" />
              </label>
            </div>

            <div className="field-row well">
              <div>
                <div className="field-label">断流 / 干转保护</div>
                <div className="field-hint">运行中无水流循环超阈值立即锁定停泵</div>
              </div>
              <label className="switch">
                <input
                  type="checkbox"
                  checked={cfg.flow_protection}
                  onChange={(e) => updateCfg((p) => ({ ...p, flow_protection: e.target.checked }))}
                />
                <span className="switch-track" />
              </label>
            </div>

            <div className="field-row well">
              <div>
                <div className="field-label">断开 / 睡眠发送待机包</div>
                <div className="field-hint">退出或休眠前停泵并发送 GoToSleepLC (0x19 00 01)</div>
              </div>
              <label className="switch">
                <input
                  type="checkbox"
                  checked={cfg.sleep_on_disconnect}
                  onChange={(e) =>
                    updateCfg((p) => ({ ...p, sleep_on_disconnect: e.target.checked }))
                  }
                />
                <span className="switch-track" />
              </label>
            </div>

            <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
              <button
                type="button"
                className="btn btn-primary"
                disabled={busy}
                onClick={() =>
                  sendOp(
                    "正在保存并下发水冷配置…",
                    { op: "set_config", config: cfg, apply_now: true },
                    true
                  )
                }
              >
                保存并应用配置
              </button>
              {dirty && <span className="pill warn">有未保存修改</span>}
            </div>
          </section>
        </div>
      </div>

      <div className={`status-bar${isError ? " error" : ""}`}>
        <span>{msg}</span>
      </div>
    </div>
  );
}

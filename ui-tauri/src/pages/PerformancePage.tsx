// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

import { useEffect, useRef, useState } from "react";
import { api } from "../api/tauri";
import {
  CPU_FAN_TEMPS,
  GPU_FAN_TEMPS,
  MIN_FAN_DUTY,
  type PerformanceStatus,
  type TuningConfig,
} from "../types/hardware";

const MODE_CARDS = [
  { id: "office", title: "办公模式", led: "绿色指示灯", color: "var(--led-office)" },
  { id: "balanced", title: "均衡模式", led: "蓝色指示灯", color: "var(--led-balanced)" },
  { id: "turbo", title: "狂暴模式", led: "紫色指示灯", color: "var(--led-turbo)" },
  { id: "custom", title: "自定义模式", led: "白色指示灯", color: "var(--led-custom)" },
] as const;

function FanCurveSvg({
  temps,
  values,
  stroke,
}: {
  temps: number[];
  values: number[];
  stroke: string;
}) {
  const w = 520;
  const h = 152;
  const left = 36;
  const right = w - 14;
  const top = 12;
  const bottom = h - 24;

  const mapX = (t: number) => left + ((t - 40) / 50) * (right - left);
  const mapY = (v: number) => bottom - ((v - 30) / 70) * (bottom - top);

  const floorPts = temps.map((t, i) => `${mapX(t)},${mapY(MIN_FAN_DUTY[i])}`).join(" ");
  const floorArea = `${left},${bottom} ${floorPts} ${mapX(temps[temps.length - 1])},${bottom}`;
  const curvePts = temps.map((t, i) => `${mapX(t)},${mapY(values[i])}`).join(" ");

  return (
    <svg
      viewBox={`0 0 ${w} ${h}`}
      style={{
        width: "100%",
        height: 156,
        background: "#090c12",
        borderRadius: 10,
        border: "1px solid var(--border-seam)",
      }}
    >
      {[40, 70, 100].map((pct) => {
        const y = mapY(pct);
        return (
          <g key={pct}>
            <line x1={left} y1={y} x2={right} y2={y} stroke="#1c2433" strokeWidth="1" />
            <text x={5} y={y + 4} fill="#64748b" fontSize="10" className="mono">
              {pct}%
            </text>
          </g>
        );
      })}
      {[40, 60, 80, 90].map((t) => (
        <text key={t} x={mapX(t) - 10} y={h - 6} fill="#64748b" fontSize="10" className="mono">
          {t}°C
        </text>
      ))}
      <polygon points={floorArea} fill="rgba(100, 116, 139, 0.14)" />
      <polyline points={floorPts} fill="none" stroke="#475569" strokeWidth="1" strokeDasharray="3 3" />
      <polyline points={curvePts} fill="none" stroke={stroke} strokeOpacity="0.25" strokeWidth="6" />
      <polyline points={curvePts} fill="none" stroke={stroke} strokeWidth="2.3" />
      {temps.map((t, i) => (
        <circle key={i} cx={mapX(t)} cy={mapY(values[i])} r="3.4" fill={stroke} />
      ))}
    </svg>
  );
}

export function PerformancePage() {
  const [perf, setPerf] = useState<PerformanceStatus | null>(null);
  const [busy, setBusy] = useState<boolean>(false);
  const busyRef = useRef(false);
  busyRef.current = busy;
  const [statusMsg, setStatusMsg] = useState<string>("正在读取固件档位与调校状态…");
  const [isError, setIsError] = useState<boolean>(false);

  const [cpuPower, setCpuPower] = useState<number>(35);
  const [gpuEnabled, setGpuEnabled] = useState<boolean>(false);
  const [gpuPower, setGpuPower] = useState<number>(115);
  const [gpuDbEnabled, setGpuDbEnabled] = useState<boolean>(false);
  const [gpuDb, setGpuDb] = useState<number>(5);
  const [cpuFan, setCpuFan] = useState<number[]>([...MIN_FAN_DUTY]);
  const [gpuFan, setGpuFan] = useState<number[]>([...MIN_FAN_DUTY]);
  const [curveTab, setCurveTab] = useState<"cpu" | "gpu">("cpu");
  const [curveOpen, setCurveOpen] = useState<boolean>(false);
  const [initialized, setInitialized] = useState<boolean>(false);

  function hydrateTuning(t: TuningConfig) {
    setCpuPower(t.pl1);
    setCpuFan([...t.cpu_fan]);
    setGpuFan([...t.gpu_fan]);
    if (typeof t.gpu_ctgp === "number") {
      setGpuEnabled(true);
      setGpuPower(t.gpu_ctgp);
      if (typeof t.gpu_db === "number" && t.gpu_db >= 5) {
        setGpuDbEnabled(true);
        setGpuDb(t.gpu_db);
      } else {
        setGpuDbEnabled(false);
      }
    } else {
      setGpuEnabled(false);
      setGpuDbEnabled(false);
    }
  }

  async function refresh(syncPrefs: boolean) {
    try {
      const res = await api.getPerformanceStatus();
      setPerf(res.status);
      if (syncPrefs) {
        const src = res.status.tuning ?? res.prefs;
        if (src) hydrateTuning(src);
        setInitialized(true);
      }
      setStatusMsg(res.status.reason || "已连接固件档位控制");
      setIsError(false);
    } catch (err) {
      setStatusMsg(`读取固件状态失败：${String(err)}`);
      setIsError(true);
    }
  }

  useEffect(() => {
    refresh(true);
    const timer = window.setInterval(() => {
      if (!busyRef.current) refresh(false);
    }, 3500);
    return () => window.clearInterval(timer);
  }, []);

  async function runOp(label: string, fn: () => Promise<PerformanceStatus>) {
    setBusy(true);
    setStatusMsg(label);
    setIsError(false);
    try {
      const next = await fn();
      setPerf(next);
      setStatusMsg(next.reason || "设置已生效，硬件回读一致");
    } catch (err) {
      setStatusMsg(String(err));
      setIsError(true);
    } finally {
      setBusy(false);
    }
  }

  async function handleApplyTuning() {
    if (
      cpuFan.some((v, i) => i > 0 && v < cpuFan[i - 1]) ||
      gpuFan.some((v, i) => i > 0 && v < gpuFan[i - 1])
    ) {
      setStatusMsg("请确保风扇占空比随温度单调不减后再应用。");
      setIsError(true);
      return;
    }
    const cfg: TuningConfig = {
      pl1: cpuPower,
      pl2: cpuPower,
      cpu_fan: cpuFan,
      gpu_fan: gpuFan,
    };
    if (gpuEnabled) {
      cfg.gpu_ctgp = gpuPower;
      cfg.gpu_db = gpuDbEnabled ? gpuDb : 0;
    }
    await runOp("正在应用自定义功耗与双风扇曲线…", () => api.applyPerformanceTuning(cfg));
  }

  const safety = perf?.fan_safety_protect === true;
  const gpuSupported =
    perf?.gpu_tuning_supported === true &&
    perf.gpu_tuning_range?.[0] === 65 &&
    perf.gpu_tuning_range?.[1] === 115 &&
    perf.gpu_db_range?.[0] === 5 &&
    perf.gpu_db_range?.[1] === 25;

  // Exact Turbo submode eligibility checks matching performance.py
  const turbo = perf?.turbo ?? {};
  const turboActive = turbo.active === true;
  const turboAvail = turbo.available === true;
  const turboRecovering = turbo.recovery_needed === true && !turboActive;
  const turboUncertain = turbo.policy_uncertain === true;
  const turboKnown =
    typeof turbo.active === "boolean" &&
    typeof turbo.recovery_needed === "boolean" &&
    typeof turbo.policy_uncertain === "boolean" &&
    typeof turbo.standard_turbo === "boolean" &&
    typeof turbo.round_adapter === "boolean" &&
    typeof turbo.combined_supported === "boolean" &&
    turbo.read_rc === 0;
  const turboEligible =
    turboAvail &&
    turboKnown &&
    turbo.combined_supported === true &&
    perf?.mode === "turbo" &&
    turbo.standard_turbo === true &&
    turbo.round_adapter === true &&
    !perf.tuning_active &&
    !perf.tuning_recovery_pending &&
    !turboRecovering &&
    !turboUncertain;
  const activeTurboVariant =
    turboActive && (turbo.variant === "quiet" || turbo.variant === "extreme")
      ? turbo.variant
      : turboKnown && !turboActive && !turboRecovering && !turboUncertain
        ? "off"
        : null;

  const turboCaption = turboUncertain
    ? "固件状态无法确认，已停止软件切档；请正常重启电脑"
    : !turboAvail && (turboActive || turboRecovering)
      ? "控制服务离线，正在等待内核归还控制"
      : !turboAvail
        ? "调度服务未就绪，当前保持固件控制"
        : turboRecovering
          ? "恢复尚未完成，请点击“交还固件控制”重试"
          : !turboKnown
            ? "调度状态尚未确认"
            : !turbo.combined_supported
              ? "固件未报告支持联合功耗调度"
              : turboActive
                ? `当前：${activeTurboVariant === "quiet" ? "静音狂暴" : "高能狂暴"} · 自动分配 CPU / GPU 功耗`
                : perf?.mode !== "turbo"
                  ? "请先切换到上方的狂暴模式"
                  : !turbo.round_adapter
                    ? "请连接原装圆口电源"
                    : "当前：由固件控制，尚未启用子模式调度";

  return (
    <div className="page-stack">
      {/* Firmware Mode Deck */}
      <section className="hero-stage">
        <div className="section-header">
          <div>
            <h2 className="section-title">固件运行档位</h2>
            <p className="section-desc">
              与机身物理模式键保持双向同步，每 3.5 秒自动刷新 EC 状态
            </p>
          </div>
          <span className={`pill ${safety ? "danger" : perf?.switchable ? "ok" : "warn"}`}>
            <span className={`dot ${safety ? "danger" : perf?.switchable ? "ok" : "warn"}`} />
            {safety
              ? "EC 高温安全保护激活"
              : perf?.tuning_active
                ? "自定义调校运行中"
                : perf?.mode
                  ? `EC ${perf.ec_mode ?? ""}`
                  : "读取中"}
          </span>
        </div>

        <div className="perf-mode-grid">
          {MODE_CARDS.map((card) => {
            const isActive = perf?.mode === card.id;
            const disabled =
              busy || !perf?.switchable || isActive || (card.id === "custom" && safety);
            return (
              <button
                key={card.id}
                type="button"
                className={`perf-mode-card${isActive ? " active" : ""}`}
                disabled={disabled}
                onClick={() => runOp(`正在切换至${card.title}…`, () => api.setPerformanceMode(card.id))}
              >
                <span className="mode-title">{card.title}</span>
                <span className="mode-led-tag">
                  <span
                    className="dot"
                    style={{
                      background: card.color,
                      boxShadow: isActive ? `0 0 10px ${card.color}` : "none",
                    }}
                  />
                  {card.led}
                </span>
              </button>
            );
          })}
        </div>

        <div className="split-equal">
          <div className="well">
            <div className="kpi-label">EC 高温风扇安全保护 (0x7A5)</div>
            <div style={{ marginTop: 4, fontSize: 12.5, color: safety ? "var(--status-danger)" : "var(--ink-primary)" }}>
              {perf
                ? safety
                  ? "保护激活：已交由固件优先降温，暂停自定义调校"
                  : "正常"
                : "读取中…"}
            </div>
          </div>
          <div className="well">
            <div className="kpi-label">SmartBalance (0x7AC)</div>
            <div style={{ marginTop: 4, fontSize: 12.5 }}>
              {perf
                ? perf.smart_balance_supported
                  ? perf.smart_balance_enabled
                    ? "支持 · 已开启"
                    : "支持 · 未开启"
                  : "本机固件不支持 (bit0=0)"
                : "读取中…"}
            </div>
          </div>
        </div>
      </section>

      {/* Turbo Lease Submodes */}
      <section className="panel">
        <div className="section-header">
          <div>
            <h3 className="section-title">狂暴子模式调度（Rust mechrevo-turbod）</h3>
            <p className="section-desc">{turboCaption}</p>
          </div>
          <div style={{ display: "flex", gap: 8 }}>
            {(
              [
                { id: "quiet", label: "静音狂暴" },
                { id: "extreme", label: "高能狂暴" },
                { id: "off", label: "交还固件控制" },
              ] as const
            ).map((btn) => {
              const enabled =
                btn.id === "off"
                  ? turboAvail && (turboActive || turboRecovering) && !turboUncertain
                  : turboEligible && btn.id !== activeTurboVariant;
              return (
                <button
                  key={btn.id}
                  type="button"
                  className={`btn btn-sm${activeTurboVariant === btn.id && turboAvail ? " btn-selected" : ""}`}
                  disabled={busy || !enabled}
                  onClick={() => runOp(`正在切换狂暴子模式 (${btn.label})…`, () => api.setTurboVariant(btn.id))}
                >
                  {btn.label}
                </button>
              );
            })}
          </div>
        </div>
        <div className="field-hint">
          仅在原装圆口电源与普通狂暴档下可用；切档、拔电或休眠前自动交还固件控制，唤醒后不自动重获。
        </div>
      </section>

      {/* Custom Power & 11-Node Fan Curve */}
      <section className="panel">
        <div className="section-header">
          <div>
            <h3 className="section-title">自定义功耗与 11 段双风扇曲线</h3>
            <p className="section-desc">
              应用后切入白色自定义档；切换其他档位、按下模式键或睡眠前自动完整回退
            </p>
          </div>
          {perf?.tuning && (
            <span className="pill ok mono">
              CPU {perf.tuning.pl1}W
              {typeof perf.tuning.gpu_ctgp === "number" ? ` · GPU ${perf.tuning.gpu_ctgp}W` : ""}
              {perf.tuning.gpu_db ? ` + ${perf.tuning.gpu_db}W DB` : ""}
            </span>
          )}
        </div>

        <div className="well" style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <div className="field-row">
            <div>
              <div className="field-label">CPU 功耗上限（PL1 = PL2 同步）</div>
              <div className="field-hint">实机验证安全包络 25–75 W</div>
            </div>
            <div style={{ display: "inline-flex", alignItems: "center", gap: 10 }}>
              <input
                type="range"
                style={{ width: 160 }}
                min={25}
                max={75}
                value={cpuPower}
                onChange={(e) => setCpuPower(Number(e.target.value))}
              />
              <span className="mono" style={{ minWidth: 46, textAlign: "right", color: "var(--accent)", fontWeight: 700 }}>
                {cpuPower} W
              </span>
            </div>
          </div>

          <div className="field-row">
            <label style={{ display: "inline-flex", alignItems: "center", gap: 8, fontSize: 12.5 }}>
              <input
                type="checkbox"
                disabled={!gpuSupported && !gpuEnabled}
                checked={gpuEnabled}
                onChange={(e) => setGpuEnabled(e.target.checked)}
              />
              <span>同时调节 GPU 基础功耗 (cTGP 65–115 W)</span>
            </label>
            <div style={{ display: "inline-flex", alignItems: "center", gap: 10, opacity: gpuEnabled && gpuSupported ? 1 : 0.45 }}>
              <input
                type="range"
                style={{ width: 160 }}
                min={65}
                max={115}
                disabled={!gpuEnabled || !gpuSupported}
                value={gpuPower}
                onChange={(e) => setGpuPower(Number(e.target.value))}
              />
              <span className="mono" style={{ minWidth: 52, textAlign: "right", fontWeight: 700 }}>
                {gpuPower} W
              </span>
            </div>
          </div>

          <div className="field-row" style={{ opacity: gpuEnabled && gpuSupported ? 1 : 0.45 }}>
            <label style={{ display: "inline-flex", alignItems: "center", gap: 8, fontSize: 12.5 }}>
              <input
                type="checkbox"
                disabled={!gpuEnabled || !gpuSupported}
                checked={gpuDbEnabled}
                onChange={(e) => setGpuDbEnabled(e.target.checked)}
              />
              <span>Dynamic Boost 额外 GPU 功耗 (5–25 W)</span>
            </label>
            <div style={{ display: "inline-flex", alignItems: "center", gap: 10 }}>
              <input
                type="range"
                style={{ width: 160 }}
                min={5}
                max={25}
                disabled={!gpuEnabled || !gpuSupported || !gpuDbEnabled}
                value={gpuDb}
                onChange={(e) => setGpuDb(Number(e.target.value))}
              />
              <span className="mono" style={{ minWidth: 52, textAlign: "right", fontWeight: 700 }}>
                {gpuDbEnabled ? `${gpuDb} W` : "关闭"}
              </span>
            </div>
          </div>

          <div className="field-hint">
            {gpuSupported
              ? "未勾选 GPU 调节时由固件原生管理 GPU 功耗。"
              : perf?.gpu_tuning_reason || "GPU 调校能力确认中，可先调节 CPU 与双风扇曲线。"}
          </div>
        </div>

        {/* Collapsible 11-node fan curve editor */}
        <div className="well" style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <div className="field-row">
            <span className="field-label">CPU / GPU 11 段风扇曲线</span>
            <button type="button" className="btn btn-sm" onClick={() => setCurveOpen((o) => !o)}>
              {curveOpen ? "收起曲线编辑" : "展开曲线编辑"}
            </button>
          </div>

          {curveOpen && (
            <>
              <div style={{ display: "flex", gap: 8 }}>
                <button
                  type="button"
                  className={`btn btn-sm${curveTab === "cpu" ? " btn-selected" : ""}`}
                  onClick={() => setCurveTab("cpu")}
                >
                  CPU 风扇
                </button>
                <button
                  type="button"
                  className={`btn btn-sm${curveTab === "gpu" ? " btn-selected" : ""}`}
                  onClick={() => setCurveTab("gpu")}
                >
                  GPU 风扇
                </button>
              </div>

              <FanCurveSvg
                temps={curveTab === "cpu" ? CPU_FAN_TEMPS : GPU_FAN_TEMPS}
                values={curveTab === "cpu" ? cpuFan : gpuFan}
                stroke={curveTab === "cpu" ? "#38bdf8" : "#a855f7"}
              />

              <div
                style={{
                  display: "grid",
                  gridTemplateColumns: "repeat(auto-fill, minmax(92px, 1fr))",
                  gap: 8,
                }}
              >
                {(curveTab === "cpu" ? CPU_FAN_TEMPS : GPU_FAN_TEMPS).map((temp, i) => {
                  const arr = curveTab === "cpu" ? cpuFan : gpuFan;
                  const setArr = curveTab === "cpu" ? setCpuFan : setGpuFan;
                  return (
                    <div
                      key={i}
                      style={{
                        background: "var(--bg-panel)",
                        border: "1px solid var(--border-seam)",
                        borderRadius: 7,
                        padding: "6px 8px",
                        display: "flex",
                        flexDirection: "column",
                        gap: 4,
                      }}
                    >
                      <span className="mono" style={{ fontSize: 11, color: "var(--ink-muted)" }}>
                        {i === 0 ? `<${(curveTab === "cpu" ? CPU_FAN_TEMPS : GPU_FAN_TEMPS)[1]}` : temp}°C
                      </span>
                      <input
                        type="number"
                        className="input input-num"
                        style={{ width: "100%", padding: "4px 6px" }}
                        min={MIN_FAN_DUTY[i]}
                        max={100}
                        step={5}
                        disabled={i === 10}
                        value={arr[i]}
                        onChange={(e) => {
                          const v = Math.max(
                            MIN_FAN_DUTY[i],
                            Math.min(100, Math.round(Number(e.target.value) || MIN_FAN_DUTY[i]))
                          );
                          const next = arr.slice();
                          next[i] = v;
                          setArr(next);
                        }}
                      />
                    </div>
                  );
                })}
              </div>
              <div className="field-hint">
                阴影区为出厂最低散热安全基线，禁止下探；最高温节点锁定 100%，曲线须单调不减。
              </div>
            </>
          )}
        </div>

        <div style={{ display: "flex", gap: 10 }}>
          <button
            type="button"
            className="btn btn-primary"
            disabled={busy || !initialized || !perf?.switchable || safety || (gpuEnabled && !gpuSupported)}
            onClick={handleApplyTuning}
          >
            应用功耗与曲线
          </button>
          <button
            type="button"
            className="btn"
            disabled={busy || !(perf?.tuning_active || perf?.tuning_recovery_pending)}
            onClick={() => runOp("正在撤销自定义调校并还原固件…", () => api.resetPerformanceTuning())}
          >
            撤销调校 / 返回原档位
          </button>
        </div>
      </section>

      <div className={`status-bar${isError ? " error" : ""}`}>
        <span>{statusMsg}</span>
      </div>
    </div>
  );
}

// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

import { useEffect, useRef, useState } from "react";
import { api } from "../api/tauri";
import {
  hexToRgb,
  LOGO_FIXED_COLORS,
  LOGO_MODES,
  rgbCss,
  rgbToHex,
  type Rgb,
} from "../types/hardware";

const BRIGHTNESS_STEPS = [0, 25, 50, 75, 100] as const;

export function LogoPage() {
  const [mode, setMode] = useState<string>("solid");
  const [brightness, setBrightness] = useState<number>(25);
  const [color, setColor] = useState<Rgb>([0, 255, 255]);
  const [hwSummary, setHwSummary] = useState<string>("正在读取后盖 Logo 状态…");
  const [status, setStatus] = useState<string>("编辑后点击“应用灯光”写入 EC 寄存器");
  const [isError, setIsError] = useState<boolean>(false);
  const [busy, setBusy] = useState<boolean>(false);
  const busyRef = useRef(false);
  busyRef.current = busy;
  const [initialized, setInitialized] = useState<boolean>(false);

  async function loadStatus(syncControls: boolean) {
    try {
      const res = await api.getLogoStatus();
      const hw = res.hardware;
      const modeCaption = LOGO_MODES.find((m) => m.id === hw.mode)?.label ?? hw.mode;
      setHwSummary(
        hw.logo_enabled ? `当前硬件：${modeCaption} · ${hw.brightness}%` : "当前硬件：Logo 灯已关闭"
      );
      if (syncControls) {
        const src = hw.logo_enabled ? hw : res.prefs ?? hw;
        setMode(src.mode);
        setBrightness(src.brightness);
        setColor(src.color);
        setInitialized(true);
      }
      setIsError(false);
    } catch (err) {
      setStatus(`读取 Logo 状态失败：${String(err)}`);
      setIsError(true);
    }
  }

  useEffect(() => {
    loadStatus(true);
    const timer = window.setInterval(() => {
      if (!busyRef.current) loadStatus(false);
    }, 3500);
    return () => window.clearInterval(timer);
  }, []);

  async function handleApply() {
    setBusy(true);
    setStatus("正在发送 Logo 灯光指令并核对回读…");
    setIsError(false);
    try {
      const effectiveColor =
        mode === "breathing" &&
        !LOGO_FIXED_COLORS.some(
          (c) => c.rgb[0] === color[0] && c.rgb[1] === color[1] && c.rgb[2] === color[2]
        )
          ? LOGO_FIXED_COLORS[0].rgb
          : color;
      const hw = await api.applyLogo(mode, brightness, effectiveColor);
      const modeCaption = LOGO_MODES.find((m) => m.id === hw.mode)?.label ?? hw.mode;
      setHwSummary(
        hw.logo_enabled ? `当前硬件：${modeCaption} · ${hw.brightness}%` : "当前硬件：Logo 灯已关闭"
      );
      setStatus("设置已发送，EC 寄存器回读一致");
    } catch (err) {
      setStatus(`Logo 控制失败：${String(err)}`);
      setIsError(true);
    } finally {
      setBusy(false);
    }
  }

  async function handleOff() {
    setBusy(true);
    setStatus("正在关闭后盖 Logo…");
    setIsError(false);
    try {
      await api.setLogoEnabled(false);
      setBrightness(0);
      setHwSummary("当前硬件：Logo 灯已关闭");
      setStatus("已关闭后盖 Logo（保留先前灯效配色）");
    } catch (err) {
      setStatus(`关闭失败：${String(err)}`);
      setIsError(true);
    } finally {
      setBusy(false);
    }
  }

  const effectivePreview: Rgb = mode === "rainbow" ? [56, 189, 248] : color;
  const brFactor = brightness / 100;

  return (
    <div className="page-stack">
      <section className="hero-stage">
        <div className="section-header">
          <div>
            <h2 className="section-title">屏幕后盖发光徽标</h2>
            <p className="section-desc">{hwSummary}</p>
          </div>
          <span className={`pill ${brightness > 0 ? "ok" : ""}`}>
            <span className={`dot ${brightness > 0 ? "ok" : "muted"}`} />
            {brightness > 0 ? `${brightness}%` : "已关闭"}
          </span>
        </div>

        <div
          className="well"
          style={{
            display: "grid",
            placeItems: "center",
            padding: "22px 16px",
            background: "radial-gradient(circle at 50% 50%, #151d2a 0%, #0b0e14 70%)",
          }}
        >
          <div
            style={{
              width: 340,
              height: 146,
              borderRadius: 12,
              border: "1px solid #283448",
              background: "linear-gradient(155deg, #171e2b 0%, #0f141d 100%)",
              display: "grid",
              placeItems: "center",
              position: "relative",
              overflow: "hidden",
            }}
          >
            {brFactor > 0 && (
              <div
                style={{
                  position: "absolute",
                  width: 112,
                  height: 112,
                  borderRadius: "50%",
                  background: rgbCss(effectivePreview, 0.24 * brFactor),
                  filter: "blur(24px)",
                }}
              />
            )}
            <svg width="78" height="52" viewBox="0 0 78 52" fill="none" style={{ position: "relative" }}>
              <path
                d="M10 42L25 10L39 30L53 10L68 42"
                stroke={brFactor > 0 ? rgbCss(effectivePreview, 0.38 + 0.62 * brFactor) : "#334155"}
                strokeWidth="5.2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          </div>
        </div>
      </section>

      <section className="panel">
        <div className="field-row well">
          <div>
            <div className="field-label">灯光效果</div>
            <div className="field-hint">呼吸支持七种固定颜色，常亮支持任意自选 RGB</div>
          </div>
          <div style={{ display: "flex", gap: 6 }}>
            {LOGO_MODES.map((m) => (
              <button
                key={m.id}
                type="button"
                className={`btn btn-sm${mode === m.id ? " btn-selected" : ""}`}
                onClick={() => {
                  setMode(m.id);
                  if (m.id === "breathing") {
                    const match = LOGO_FIXED_COLORS.some(
                      (c) => c.rgb[0] === color[0] && c.rgb[1] === color[1] && c.rgb[2] === color[2]
                    );
                    if (!match) setColor(LOGO_FIXED_COLORS[0].rgb);
                  }
                }}
              >
                {m.label}
              </button>
            ))}
          </div>
        </div>

        <div className="field-row well">
          <div>
            <div className="field-label">硬件步进亮度</div>
            <div className="field-hint">EC 寄存器提供 5 档固定步进</div>
          </div>
          <div style={{ display: "flex", gap: 6 }}>
            {BRIGHTNESS_STEPS.map((b) => (
              <button
                key={b}
                type="button"
                className={`btn btn-sm mono${brightness === b ? " btn-selected" : ""}`}
                onClick={() => setBrightness(b)}
              >
                {b === 0 ? "关闭" : `${b}%`}
              </button>
            ))}
          </div>
        </div>

        {mode !== "rainbow" && (
          <div className="well" style={{ display: "flex", flexDirection: "column", gap: 10 }}>
            <div className="field-row">
              <span className="field-label">预设七色</span>
              {mode === "solid" && (
                <label style={{ display: "inline-flex", alignItems: "center", gap: 8, fontSize: 12, color: "var(--ink-muted)" }}>
                  自选颜色
                  <input
                    type="color"
                    className="color-input"
                    value={rgbToHex(color)}
                    onChange={(e) => setColor(hexToRgb(e.target.value))}
                  />
                </label>
              )}
            </div>
            <div style={{ display: "flex", flexWrap: "wrap", gap: 8 }}>
              {LOGO_FIXED_COLORS.map((item) => {
                const selected =
                  item.rgb[0] === color[0] && item.rgb[1] === color[1] && item.rgb[2] === color[2];
                return (
                  <button
                    key={item.name}
                    type="button"
                    className={`btn btn-sm${selected ? " btn-selected" : ""}`}
                    onClick={() => setColor([...item.rgb] as Rgb)}
                  >
                    <span
                      className="dot"
                      style={{ background: rgbCss(item.rgb), border: "1px solid rgba(255,255,255,0.25)" }}
                    />
                    {item.name}
                  </button>
                );
              })}
            </div>
          </div>
        )}

        <div style={{ display: "flex", gap: 10 }}>
          <button
            type="button"
            className="btn btn-primary"
            disabled={busy || !initialized}
            onClick={handleApply}
          >
            {busy ? "正在应用…" : "应用灯光"}
          </button>
          <button
            type="button"
            className="btn btn-danger"
            disabled={busy || !initialized}
            onClick={handleOff}
          >
            关闭 Logo
          </button>
        </div>
      </section>

      <div className={`status-bar${isError ? " error" : ""}`}>
        <span>{status}</span>
      </div>
    </div>
  );
}

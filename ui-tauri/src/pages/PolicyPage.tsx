// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

import { useEffect, useState } from "react";
import { api } from "../api/tauri";
import type { LightingPolicyConfig } from "../types/hardware";

const DEFAULT_POLICY: LightingPolicyConfig = {
  version: 1,
  enabled: false,
  power_enabled: false,
  idle_minutes: 0,
  brightness: {
    keyboard: { ac: 100, dc: 100 },
    lightbar: { ac: 100, dc: 100 },
    logo: { ac: 100, dc: 100 },
  },
};

const ZONES: { key: keyof LightingPolicyConfig["brightness"]; label: string; stepped?: boolean }[] = [
  { key: "keyboard", label: "键盘背光" },
  { key: "lightbar", label: "机身灯带" },
  { key: "logo", label: "后盖 Logo", stepped: true },
];

export function PolicyPage() {
  const [cfg, setCfg] = useState<LightingPolicyConfig>(DEFAULT_POLICY);
  const [status, setStatus] = useState<string>("正在读取自动灯光策略…");
  const [isError, setIsError] = useState<boolean>(false);
  const [saving, setSaving] = useState<boolean>(false);

  useEffect(() => {
    api
      .getLightingPolicy()
      .then((loaded) => {
        setCfg(loaded);
        setStatus(
          loaded.enabled
            ? "已载入自动灯光策略，由后台 iteglow-sessiond 守护实时执行。"
            : "自动策略当前关闭；修改后点击“保存自动灯光设置”生效。"
        );
        setIsError(false);
      })
      .catch((err) => {
        setStatus(`读取配置失败（未改动文件）：${String(err)}`);
        setIsError(true);
      });
  }, []);

  function setZoneBright(zone: keyof LightingPolicyConfig["brightness"], src: "ac" | "dc", value: number) {
    setCfg((prev) => ({
      ...prev,
      brightness: {
        ...prev.brightness,
        [zone]: {
          ...prev.brightness[zone],
          [src]: value,
        },
      },
    }));
  }

  async function handleSave() {
    setSaving(true);
    setIsError(false);
    try {
      const saved = await api.saveLightingPolicy(cfg);
      setCfg(saved);
      setStatus(
        saved.enabled
          ? "已保存；自动灯光策略已启用，后台守护将在 5 秒内接管生效。"
          : "已保存；自动灯光策略已关闭。"
      );
    } catch (err) {
      setStatus(`保存失败：${String(err)}`);
      setIsError(true);
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="page-stack">
      <section className="panel">
        <div className="field-row well">
          <div>
            <div className="section-title">启用自动灯光策略</div>
            <div className="field-hint">
              按插电 / 电池方式独立调节亮度，空闲超时后自动熄灯并在恢复键鼠操作后还原
            </div>
          </div>
          <label className="switch">
            <input
              type="checkbox"
              checked={cfg.enabled}
              onChange={(e) => setCfg({ ...cfg, enabled: e.target.checked })}
            />
            <span className="switch-track" />
          </label>
        </div>

        <div className="field-row well" style={{ opacity: cfg.enabled ? 1 : 0.48 }}>
          <div>
            <div className="field-label">按供电状态分别控制三区亮度</div>
            <div className="field-hint">仅作用于当前处于开启状态的灯区，手动关闭的灯区不会被自动点亮</div>
          </div>
          <label className="switch">
            <input
              type="checkbox"
              disabled={!cfg.enabled}
              checked={cfg.power_enabled}
              onChange={(e) => setCfg({ ...cfg, power_enabled: e.target.checked })}
            />
            <span className="switch-track" />
          </label>
        </div>

        <div
          className="well"
          style={{
            display: "flex",
            flexDirection: "column",
            gap: 10,
            opacity: cfg.enabled && cfg.power_enabled ? 1 : 0.45,
          }}
        >
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "1fr 140px 140px",
              gap: 12,
              fontSize: 11.5,
              color: "var(--ink-muted)",
              paddingBottom: 6,
              borderBottom: "1px solid var(--border-seam)",
            }}
          >
            <span>灯区</span>
            <span>插电 (AC)</span>
            <span>电池 (DC)</span>
          </div>

          {ZONES.map((z) => (
            <div
              key={z.key}
              style={{
                display: "grid",
                gridTemplateColumns: "1fr 140px 140px",
                gap: 12,
                alignItems: "center",
              }}
            >
              <span className="field-label">{z.label}</span>
              {(["ac", "dc"] as const).map((src) =>
                z.stepped ? (
                  <div key={src} style={{ display: "inline-flex", alignItems: "center", gap: 6 }}>
                    <select
                      className="select mono"
                      disabled={!cfg.enabled || !cfg.power_enabled}
                      value={cfg.brightness[z.key][src]}
                      onChange={(e) => setZoneBright(z.key, src, Number(e.target.value))}
                    >
                      {[0, 25, 50, 75, 100].map((v) => (
                        <option key={v} value={v}>
                          {v}%
                        </option>
                      ))}
                    </select>
                  </div>
                ) : (
                  <div key={src} style={{ display: "inline-flex", alignItems: "center", gap: 6 }}>
                    <input
                      type="number"
                      className="input input-num"
                      min={0}
                      max={100}
                      disabled={!cfg.enabled || !cfg.power_enabled}
                      value={cfg.brightness[z.key][src]}
                      onChange={(e) => {
                        const n = Math.max(0, Math.min(100, Math.round(Number(e.target.value) || 0)));
                        setZoneBright(z.key, src, n);
                      }}
                    />
                    <span className="mono" style={{ color: "var(--ink-muted)" }}>
                      %
                    </span>
                  </div>
                )
              )}
            </div>
          ))}
        </div>

        <div className="field-row well" style={{ opacity: cfg.enabled ? 1 : 0.48 }}>
          <div>
            <div className="field-label">空闲自动熄灯时间</div>
            <div className="field-hint">设为 0 表示不启用空闲熄灯，最大支持 1440 分钟</div>
          </div>
          <div style={{ display: "inline-flex", alignItems: "center", gap: 8 }}>
            <input
              type="number"
              className="input input-num"
              min={0}
              max={1440}
              disabled={!cfg.enabled}
              value={cfg.idle_minutes}
              onChange={(e) =>
                setCfg({
                  ...cfg,
                  idle_minutes: Math.max(0, Math.min(1440, Math.round(Number(e.target.value) || 0))),
                })
              }
            />
            <span style={{ color: "var(--ink-muted)", fontSize: 12 }}>分钟</span>
          </div>
        </div>

        <p className="field-hint" style={{ margin: 0 }}>
          自动策略产生的临时亮度不会覆盖手动灯效记录；关闭策略或会话退出时不会残留临时状态。
        </p>

        <div>
          <button type="button" className="btn btn-primary" disabled={saving} onClick={handleSave}>
            {saving ? "正在保存…" : "保存自动灯光设置"}
          </button>
        </div>
      </section>

      <div className={`status-bar${isError ? " error" : ""}`}>
        <span>{status}</span>
      </div>
    </div>
  );
}

// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

import { useEffect, useState } from "react";
import { Plus, Trash2 } from "lucide-react";
import { api } from "../api/tauri";
import type { ProfileBundle, ProfilesConfig } from "../types/hardware";

interface Props {
  onHardwareChanged?: () => void;
}

const DEFAULT_CFG: ProfilesConfig = {
  version: 1,
  auto_switch_enabled: false,
  restore_previous: true,
  cooldown_seconds: 5,
  profiles: {},
  rules: [],
};

function summarizeProfile(body: ProfileBundle | undefined): string {
  if (!body) return "尚未选择预设，可导入内置模板或从当前状态捕获保存。";
  const parts: string[] = [];
  if (body.performance) {
    let m: string = body.performance.mode;
    if (body.performance.turbo) m = `${m}/${body.performance.turbo}`;
    else if (body.performance.tuning) m = `${m} (${body.performance.tuning.pl1}W)`;
    parts.push(`性能: ${m}`);
  }
  if (body.lighting) {
    const zones: string[] = [];
    for (const z of ["keyboard", "lightbar", "logo"] as const) {
      const zv = body.lighting[z];
      if (zv) {
        let zm = zv.mode || "off";
        if (z === "keyboard" && zm === "matrix" && "preset" in zv && zv.preset) {
          zm = `matrix:${zv.preset}`;
        }
        zones.push(`${z}=${zm}`);
      }
    }
    if (zones.length) parts.push(`灯光: ${zones.join(", ")}`);
  }
  if (body.label) parts.push(body.label);
  return parts.join(" · ") || "空预设";
}

export function ProfilesPage({ onHardwareChanged }: Props) {
  const [cfg, setCfg] = useState<ProfilesConfig>(DEFAULT_CFG);
  const [selectedName, setSelectedName] = useState<string>("");
  const [capName, setCapName] = useState<string>("");
  const [capLabel, setCapLabel] = useState<string>("");
  const [capPerf, setCapPerf] = useState<boolean>(true);
  const [capLight, setCapLight] = useState<boolean>(true);

  const [ruleName, setRuleName] = useState<string>("");
  const [ruleMatch, setRuleMatch] = useState<string>("");
  const [ruleTarget, setRuleTarget] = useState<string>("");

  const [busy, setBusy] = useState<boolean>(false);
  const [status, setStatus] = useState<string>("正在读取情景预设配置…");
  const [isError, setIsError] = useState<boolean>(false);

  const profileNames = Object.keys(cfg.profiles);

  useEffect(() => {
    api
      .getProfilesConfig()
      .then((loaded) => {
        setCfg(loaded);
        const names = Object.keys(loaded.profiles);
        setSelectedName(names[0] ?? "");
        setRuleTarget(names[0] ?? "");
        setStatus(
          loaded.auto_switch_enabled
            ? "应用自动联动已启用。"
            : "自动联动默认关闭；预设仅在点击应用或匹配已启用规则时生效。"
        );
        setIsError(false);
      })
      .catch((err) => {
        setStatus(`读取预设失败：${String(err)}`);
        setIsError(true);
      });
  }, []);

  async function persist(next: ProfilesConfig, msg: string) {
    setBusy(true);
    setIsError(false);
    try {
      const saved = await api.saveProfilesConfig(next);
      setCfg(saved);
      setStatus(msg);
    } catch (err) {
      setStatus(`保存失败：${String(err)}`);
      setIsError(true);
    } finally {
      setBusy(false);
    }
  }

  async function handleImportTemplates() {
    setBusy(true);
    setIsError(false);
    try {
      const templates = await api.getBuiltinTemplates();
      const added: string[] = [];
      const nextProfiles = { ...cfg.profiles };
      for (const [k, v] of Object.entries(templates)) {
        if (!nextProfiles[k]) {
          nextProfiles[k] = v;
          added.push(k);
        }
      }
      if (added.length === 0) {
        setStatus("内置模板已全部存在。");
        setBusy(false);
        return;
      }
      const nextCfg = { ...cfg, profiles: nextProfiles };
      const saved = await api.saveProfilesConfig(nextCfg);
      setCfg(saved);
      setSelectedName(added[0]);
      if (!ruleTarget) setRuleTarget(added[0]);
      setStatus(`已导入内置模板：${added.join("、")}`);
    } catch (err) {
      setStatus(`导入失败：${String(err)}`);
      setIsError(true);
    } finally {
      setBusy(false);
    }
  }

  async function handleApplySelected() {
    if (!selectedName || !cfg.profiles[selectedName]) return;
    setBusy(true);
    setIsError(false);
    setStatus(`正在应用预设 ${selectedName}…`);
    try {
      const res = await api.applyProfileBundle(cfg.profiles[selectedName]);
      const warn = res.warnings.length ? `（注意：${res.warnings.join("；")}）` : "";
      setStatus(`已应用预设 ${selectedName}${warn}`);
      onHardwareChanged?.();
    } catch (err) {
      setStatus(`应用预设失败：${String(err)}`);
      setIsError(true);
    } finally {
      setBusy(false);
    }
  }

  async function handleDeleteSelected() {
    if (!selectedName) return;
    const nextProfiles = { ...cfg.profiles };
    delete nextProfiles[selectedName];
    const nextRules = cfg.rules.filter((r) => r.profile !== selectedName);
    const nextNames = Object.keys(nextProfiles);
    setSelectedName(nextNames[0] ?? "");
    if (ruleTarget === selectedName) setRuleTarget(nextNames[0] ?? "");
    await persist({ ...cfg, profiles: nextProfiles, rules: nextRules }, `已删除预设 ${selectedName}`);
  }

  async function handleCapture() {
    const cleanName = capName.trim();
    if (!cleanName || cleanName.includes("/") || cleanName.includes("\\")) {
      setStatus("请输入有效的预设名称（不能包含路径分隔符）");
      setIsError(true);
      return;
    }
    setBusy(true);
    setIsError(false);
    setStatus(`正在捕获当前硬件状态并保存为 ${cleanName}…`);
    try {
      const bundle = await api.captureCurrentProfile(capLabel.trim() || null, capLight, capPerf);
      const nextCfg = { ...cfg, profiles: { ...cfg.profiles, [cleanName]: bundle } };
      const saved = await api.saveProfilesConfig(nextCfg);
      setCfg(saved);
      setSelectedName(cleanName);
      if (!ruleTarget) setRuleTarget(cleanName);
      setCapName("");
      setCapLabel("");
      setStatus(`已从当前硬件状态保存预设 ${cleanName}`);
    } catch (err) {
      setStatus(`捕获失败：${String(err)}`);
      setIsError(true);
    } finally {
      setBusy(false);
    }
  }

  function handleAddRule() {
    const name = ruleName.trim();
    const tokens = ruleMatch
      .split(",")
      .map((s) => s.trim().toLowerCase())
      .filter(Boolean);
    const target = ruleTarget || profileNames[0] || "";
    if (!name || tokens.length === 0 || !target || !cfg.profiles[target]) {
      setStatus("请填写规则名称、至少一个进程关键词，并选择目标预设");
      setIsError(true);
      return;
    }
    const filtered = cfg.rules.filter((r) => r.name !== name);
    const next = {
      ...cfg,
      rules: [...filtered, { name, enabled: true, match: tokens, profile: target }],
    };
    setCfg(next);
    setRuleName("");
    setRuleMatch("");
    setStatus(`已添加规则 ${name}（点击“保存预设与联动规则”写入配置）`);
    setIsError(false);
  }

  return (
    <div className="page-stack">
      <div className="split-equal">
        {/* Profiles Bundle Card */}
        <section className="panel">
          <div>
            <h2 className="section-title">预设组合（性能 + 灯光）</h2>
            <p className="section-desc">将当前固件档位与三区灯效打包为一键情景</p>
          </div>

          <div className="well" style={{ display: "flex", flexDirection: "column", gap: 10 }}>
            <div className="field-row">
              <span className="field-label">当前选择</span>
              <select
                className="select"
                style={{ minWidth: 190 }}
                disabled={profileNames.length === 0}
                value={selectedName}
                onChange={(e) => setSelectedName(e.target.value)}
              >
                {profileNames.length === 0 ? (
                  <option value="">（暂无预设）</option>
                ) : (
                  profileNames.map((n) => (
                    <option key={n} value={n}>
                      {n}
                    </option>
                  ))
                )}
              </select>
            </div>
            <div className="field-hint">{summarizeProfile(cfg.profiles[selectedName])}</div>
            <div style={{ display: "flex", flexWrap: "wrap", gap: 8, marginTop: 4 }}>
              <button
                type="button"
                className="btn btn-primary btn-sm"
                disabled={busy || !selectedName}
                onClick={handleApplySelected}
              >
                立即应用预设
              </button>
              <button
                type="button"
                className="btn btn-sm"
                disabled={busy}
                onClick={handleImportTemplates}
              >
                导入内置模板
              </button>
              <button
                type="button"
                className="btn btn-danger btn-sm"
                disabled={busy || !selectedName}
                onClick={handleDeleteSelected}
              >
                删除预设
              </button>
            </div>
          </div>

          <div className="well" style={{ display: "flex", flexDirection: "column", gap: 10 }}>
            <div className="field-label">从当前硬件状态捕获并保存</div>
            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 8 }}>
              <input
                type="text"
                className="input"
                placeholder="预设名称（如 coding / fps）"
                value={capName}
                onChange={(e) => setCapName(e.target.value)}
              />
              <input
                type="text"
                className="input"
                placeholder="备注说明（可选）"
                value={capLabel}
                onChange={(e) => setCapLabel(e.target.value)}
              />
            </div>
            <div style={{ display: "flex", alignItems: "center", gap: 14, flexWrap: "wrap" }}>
              <label style={{ display: "inline-flex", alignItems: "center", gap: 6, fontSize: 12 }}>
                <input
                  type="checkbox"
                  checked={capPerf}
                  onChange={(e) => setCapPerf(e.target.checked)}
                />
                包含当前性能档位
              </label>
              <label style={{ display: "inline-flex", alignItems: "center", gap: 6, fontSize: 12 }}>
                <input
                  type="checkbox"
                  checked={capLight}
                  onChange={(e) => setCapLight(e.target.checked)}
                />
                包含当前三区灯光
              </label>
              <button
                type="button"
                className="btn btn-sm"
                style={{ marginLeft: "auto" }}
                disabled={busy}
                onClick={handleCapture}
              >
                保存当前状态
              </button>
            </div>
          </div>
        </section>

        {/* Auto-Switch Rules Card */}
        <section className="panel">
          <div className="field-row">
            <div>
              <h2 className="section-title">应用联动自动切换</h2>
              <p className="section-desc">
                按下机身物理模式键 (0xB0) 时立即让步，EC 高温保护始终优先
              </p>
            </div>
            <label className="switch">
              <input
                type="checkbox"
                checked={cfg.auto_switch_enabled}
                onChange={(e) => setCfg({ ...cfg, auto_switch_enabled: e.target.checked })}
              />
              <span className="switch-track" />
            </label>
          </div>

          <div
            className="well"
            style={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              gap: 12,
              flexWrap: "wrap",
              opacity: cfg.auto_switch_enabled ? 1 : 0.5,
            }}
          >
            <label style={{ display: "inline-flex", alignItems: "center", gap: 6, fontSize: 12 }}>
              <input
                type="checkbox"
                disabled={!cfg.auto_switch_enabled}
                checked={cfg.restore_previous}
                onChange={(e) => setCfg({ ...cfg, restore_previous: e.target.checked })}
              />
              应用退出后恢复先前状态
            </label>
            <div style={{ display: "inline-flex", alignItems: "center", gap: 6, fontSize: 12 }}>
              <span>退出防抖</span>
              <input
                type="number"
                className="input input-num"
                min={0}
                max={120}
                disabled={!cfg.auto_switch_enabled}
                value={cfg.cooldown_seconds}
                onChange={(e) =>
                  setCfg({
                    ...cfg,
                    cooldown_seconds: Math.max(0, Math.min(120, Math.round(Number(e.target.value) || 0))),
                  })
                }
              />
              <span style={{ color: "var(--ink-muted)" }}>秒</span>
            </div>
          </div>

          <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
            {cfg.rules.length === 0 ? (
              <div className="well field-hint">尚无应用联动规则，可在下方按进程名添加。</div>
            ) : (
              cfg.rules.map((rule, idx) => (
                <div key={rule.name} className="well field-row">
                  <label style={{ display: "inline-flex", alignItems: "center", gap: 8, minWidth: 0 }}>
                    <input
                      type="checkbox"
                      checked={rule.enabled}
                      onChange={(e) => {
                        const next = cfg.rules.slice();
                        next[idx] = { ...rule, enabled: e.target.checked };
                        setCfg({ ...cfg, rules: next });
                      }}
                    />
                    <div>
                      <div style={{ fontWeight: 600, fontSize: 12.5 }}>{rule.name}</div>
                      <div className="field-hint mono">
                        {rule.match.join(", ")} → {rule.profile}
                      </div>
                    </div>
                  </label>
                  <button
                    type="button"
                    className="btn btn-danger btn-sm"
                    onClick={() => {
                      const next = cfg.rules.filter((_, i) => i !== idx);
                      setCfg({ ...cfg, rules: next });
                      setStatus(`已移除规则 ${rule.name}（点击保存生效）`);
                    }}
                  >
                    <Trash2 size={12} />
                    移除
                  </button>
                </div>
              ))
            )}
          </div>

          <div className="well" style={{ display: "flex", flexDirection: "column", gap: 8 }}>
            <div style={{ display: "grid", gridTemplateColumns: "1fr 1.4fr", gap: 8 }}>
              <input
                type="text"
                className="input"
                placeholder="规则名称"
                value={ruleName}
                onChange={(e) => setRuleName(e.target.value)}
              />
              <input
                type="text"
                className="input"
                placeholder="匹配进程关键词（逗号分隔）"
                value={ruleMatch}
                onChange={(e) => setRuleMatch(e.target.value)}
              />
            </div>
            <div className="field-row">
              <select
                className="select"
                disabled={profileNames.length === 0}
                value={ruleTarget || profileNames[0] || ""}
                onChange={(e) => setRuleTarget(e.target.value)}
              >
                {profileNames.length === 0 ? (
                  <option value="">（无可用预设）</option>
                ) : (
                  profileNames.map((n) => (
                    <option key={n} value={n}>
                      目标预设：{n}
                    </option>
                  ))
                )}
              </select>
              <button
                type="button"
                className="btn btn-sm"
                disabled={profileNames.length === 0}
                onClick={handleAddRule}
              >
                <Plus size={13} />
                添加规则
              </button>
            </div>
          </div>

          <div>
            <button
              type="button"
              className="btn btn-primary"
              disabled={busy}
              onClick={() =>
                persist(
                  cfg,
                  cfg.auto_switch_enabled
                    ? "已保存；应用自动联动已启用。"
                    : "已保存；应用自动联动保持关闭。"
                )
              }
            >
              保存预设与联动规则
            </button>
          </div>
        </section>
      </div>

      <div className={`status-bar${isError ? " error" : ""}`}>
        <span>{status}</span>
      </div>
    </div>
  );
}

// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

import { useEffect, useRef, useState } from "react";
import { RotateCcw } from "lucide-react";
import { api } from "../api/tauri";
import {
  hexToRgb,
  KEYBOARD_101_ROWS,
  KEYBOARD_DIRECTIONS,
  KEYBOARD_MODES,
  LIGHTBAR_DIRECTIONS,
  LIGHTBAR_MODES,
  MATRIX_PRESETS,
  RAINBOW_DEFAULT,
  rgbCss,
  rgbToHex,
  type KeyCapDef,
  type KeyboardHwInfo,
  type LightingSavedParams,
  type Rgb,
} from "../types/hardware";

interface Props {
  device: "keyboard" | "lightbar";
}

function emptyMatrix(fill: Rgb = [0, 0, 0]): Rgb[] {
  return Array.from({ length: 126 }, () => [...fill] as Rgb);
}

export function LightingPage({ device }: Props) {
  const isKbd = device === "keyboard";
  const modes = isKbd ? KEYBOARD_MODES : LIGHTBAR_MODES;
  const directions = isKbd ? KEYBOARD_DIRECTIONS : LIGHTBAR_DIRECTIONS;
  const bmax = isKbd ? 50 : 100;

  const [available, setAvailable] = useState(true);
  const [loaded, setLoaded] = useState(false);
  const [mode, setMode] = useState<string>(isKbd ? "solid" : "static");
  const [brightnessPct, setBrightnessPct] = useState<number>(60);
  const [speedUi, setSpeedUi] = useState<number>(6); // 1..10 higher = faster; wire = 11 - speedUi
  const [direction, setDirection] = useState<number>(1);
  const [reactive, setReactive] = useState<boolean>(false);
  const [color, setColor] = useState<Rgb>([0, 255, 255]);
  const [palette, setPalette] = useState<Rgb[]>(RAINBOW_DEFAULT.map((c) => [...c] as Rgb));
  const [brush, setBrush] = useState<Rgb>([255, 40, 0]);
  const [matrix, setMatrix] = useState<Rgb[]>(() => emptyMatrix([0, 0, 0]));
  const [calibrate, setCalibrate] = useState<boolean>(false);
  const [painting, setPainting] = useState<false | "paint" | "erase">(false);
  const [status, setStatus] = useState<string>("正在读取设备状态…");
  const [isError, setIsError] = useState<boolean>(false);

  const debounceRef = useRef<number | null>(null);
  const lastHwRef = useRef<KeyboardHwInfo | null>(null);
  const loadedRef = useRef(false);

  const spec = modes.find((m) => m.id === mode) ?? modes[0];
  const isOn = !spec.off && brightnessPct > 0;

  // Initial read-only load (never writes hardware on mount!)
  useEffect(() => {
    let alive = true;
    api
      .getLightingState(device)
      .then(async (res) => {
        if (!alive) return;
        setAvailable(res.available);
        const saved = res.saved || {};
        if (saved.mode && modes.some((m) => m.id === saved.mode)) {
          setMode(saved.mode);
        }
        if (typeof saved.brightness === "number") {
          setBrightnessPct(Math.round(Math.min(100, (saved.brightness / res.bmax) * 100)));
        }
        if (typeof saved.speed === "number") {
          setSpeedUi(11 - Math.min(10, Math.max(1, saved.speed)));
        }
        if (typeof saved.direction === "number") {
          setDirection(saved.direction);
        }
        if (typeof saved.reactive === "boolean") {
          setReactive(saved.reactive);
        }
        if (Array.isArray(saved.color) && saved.color.length === 3) {
          setColor(saved.color as Rgb);
        }
        if (Array.isArray(saved.colors) && saved.colors.length > 0) {
          const nextPal = RAINBOW_DEFAULT.map((c, idx) =>
            saved.colors && saved.colors[idx] ? ([...saved.colors[idx]] as Rgb) : ([...c] as Rgb)
          );
          setPalette(nextPal);
        }
        if (isKbd) {
          if (Array.isArray(saved.matrix) && saved.matrix.length === 126) {
            setMatrix(saved.matrix.map((c) => [...c] as Rgb));
          } else {
            try {
              const preset = saved.preset || "gaming";
              const accent = (saved.color as Rgb) || [0, 255, 255];
              const cells = await api.getMatrixPreset(preset, accent);
              if (alive) setMatrix(cells);
            } catch {
              // keep empty fallback
            }
          }
          setCalibrate(Boolean(saved.calibrate));
          if (res.hardware) {
            lastHwRef.current = res.hardware;
            if (res.hardware.mode_name === "off") {
              setMode("off");
              setBrightnessPct(0);
            } else {
              setMode(res.hardware.mode_name);
              setBrightnessPct(Math.round(Math.min(100, (res.hardware.brightness / res.bmax) * 100)));
              if (res.hardware.speed >= 1 && res.hardware.speed <= 10) {
                setSpeedUi(11 - res.hardware.speed);
              }
            }
          }
        }
        setStatus(
          res.available
            ? isKbd
              ? "已读取当前硬件灯效状态"
              : "已载入最近保存的灯带配置"
            : "未检测到 USB HID 设备节点，请确认 udev 权限规则已加载"
        );
        setIsError(!res.available);
        loadedRef.current = true;
        setLoaded(true);
      })
      .catch((err) => {
        if (!alive) return;
        setStatus(`读取失败：${String(err)}`);
        setIsError(true);
        loadedRef.current = true;
        setLoaded(true);
      });
    return () => {
      alive = false;
    };
  }, [device]);

  // Live Fn+F6 / Fn+F7 hardware hotkey sync for keyboard
  useEffect(() => {
    if (!isKbd || !available || !loaded) return;
    const timer = window.setInterval(async () => {
      if (debounceRef.current !== null) return;
      try {
        const hw = await api.pollKeyboardHw();
        const prev = lastHwRef.current;
        if (!prev) {
          lastHwRef.current = hw;
          return;
        }
        if (hw.power !== prev.power || hw.brightness !== prev.brightness) {
          lastHwRef.current = hw;
          if (hw.power === 1 || hw.brightness === 0) {
            setBrightnessPct(0);
            setStatus("已同步硬件快捷键：键盘背光关闭");
          } else {
            const pct = Math.round(Math.min(100, (hw.brightness / bmax) * 100));
            setBrightnessPct(pct);
            if (mode === "off" && hw.mode_name !== "off") {
              setMode(hw.mode_name);
            }
            setStatus(`已同步硬件快捷键亮度：${pct}%`);
          }
          setIsError(false);
        }
      } catch {
        // ignore transient poll failures
      }
    }, 900);
    return () => window.clearInterval(timer);
  }, [isKbd, available, loaded, mode, bmax]);

  useEffect(() => {
    const onUp = () => setPainting(false);
    window.addEventListener("mouseup", onUp);
    return () => window.removeEventListener("mouseup", onUp);
  }, []);

  function scheduleApply(override: Partial<{
    mode: string;
    brightnessPct: number;
    speedUi: number;
    direction: number;
    reactive: boolean;
    color: Rgb;
    palette: Rgb[];
    matrix: Rgb[];
    calibrate: boolean;
  }> = {}) {
    if (!loadedRef.current || !available) return;
    const nextMode = override.mode ?? mode;
    const nextBright = override.brightnessPct ?? brightnessPct;
    const nextSpeed = override.speedUi ?? speedUi;
    const nextDir = override.direction ?? direction;
    const nextReactive = override.reactive ?? reactive;
    const nextColor = override.color ?? color;
    const nextPalette = override.palette ?? palette;
    const nextMatrix = override.matrix ?? matrix;
    const nextCalibrate = override.calibrate ?? calibrate;

    if (debounceRef.current !== null) {
      window.clearTimeout(debounceRef.current);
    }
    debounceRef.current = window.setTimeout(async () => {
      debounceRef.current = null;
      const payload: LightingSavedParams = {
        mode: nextMode,
        color: nextColor,
        colors: nextPalette,
        brightness: Math.round((nextBright * bmax) / 100),
        speed: 11 - nextSpeed,
        direction: nextDir,
        reactive: nextReactive,
      };
      if (isKbd && nextMode === "matrix") {
        payload.matrix = nextMatrix;
        if (nextCalibrate) payload.calibrate = true;
      }
      try {
        const res = await api.applyLighting(device, payload);
        if (res.hardware) lastHwRef.current = res.hardware;
        const mInfo = modes.find((m) => m.id === nextMode);
        const suffix = mInfo?.trigger && nextReactive ? " · 随按键亮起" : "";
        setStatus(`已应用：${mInfo?.title ?? nextMode}${suffix}`);
        setIsError(false);
      } catch (err) {
        setStatus(`设置未成功：${String(err)}`);
        setIsError(true);
      }
    }, 90);
  }

  function chooseMode(nextMode: string) {
    let nextBright = brightnessPct;
    if (nextMode !== "off" && brightnessPct === 0) {
      nextBright = 60;
      setBrightnessPct(60);
    }
    setMode(nextMode);
    scheduleApply({ mode: nextMode, brightnessPct: nextBright });
  }

  function paintKey(cap: KeyCapDef, erase: boolean, toggleOnSame: boolean) {
    if (!isKbd || mode !== "matrix") return;
    const primary = cap.indices[0];
    const current = matrix[primary] ?? [0, 0, 0];
    let target: Rgb = erase ? [0, 0, 0] : [...brush];
    if (
      !erase &&
      toggleOnSame &&
      current[0] === brush[0] &&
      current[1] === brush[1] &&
      current[2] === brush[2]
    ) {
      target = [0, 0, 0];
    }
    const next = matrix.slice();
    for (const idx of cap.indices) {
      if (idx >= 0 && idx < next.length) next[idx] = [...target] as Rgb;
    }
    setMatrix(next);
    scheduleApply({ matrix: next });
  }

  async function applyPreset(presetId: string) {
    try {
      const cells = await api.getMatrixPreset(presetId, brush);
      setMatrix(cells);
      scheduleApply({ mode: "matrix", matrix: cells });
    } catch (err) {
      setStatus(`应用预设失败：${String(err)}`);
      setIsError(true);
    }
  }

  function previewColorForKey(cap: KeyCapDef, rowIdx: number, colIdx: number): Rgb {
    if (!isOn) return [32, 40, 54];
    if (mode === "matrix") {
      return matrix[cap.indices[0]] ?? [0, 0, 0];
    }
    if (spec.single) return color;
    return palette[(rowIdx + Math.floor(colIdx / 2)) % palette.length];
  }

  if (!available && loaded) {
    return (
      <div className="page-stack">
        <div className="panel">
          <h2 className="section-title">未检测到{isKbd ? "键盘背光" : "机身灯带"}硬件</h2>
          <p className="section-desc">
            请确认 USB HID 控制器已连接，并已安装桌面对等权限规则（60-iteglow.rules）。
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="page-stack">
      {/* Hero Interactive Hardware Stage */}
      <section className="hero-stage">
        <div className="section-header">
          <div>
            <h2 className="section-title">
              {isKbd ? "101 键物理矩阵视图" : "机身氛围灯带预览"}
            </h2>
            <p className="section-desc">
              {isKbd && mode === "matrix"
                ? "左键点击或拖动上色，右键擦除熄灭；变更实时下发至 ITE 8291"
                : "颜色与布局示意，实际动态节奏以硬件控制器为准"}
            </p>
          </div>
          <span className="pill ok">
            <span className="dot ok" />
            USB HID 已连接
          </span>
        </div>

        {isKbd ? (
          <>
            {mode === "matrix" && (
              <div className="well" style={{ display: "flex", flexWrap: "wrap", gap: 10, alignItems: "center" }}>
                <label className="field-label" style={{ display: "inline-flex", alignItems: "center", gap: 8 }}>
                  画笔
                  <input
                    type="color"
                    className="color-input"
                    value={rgbToHex(brush)}
                    onChange={(e) => setBrush(hexToRgb(e.target.value))}
                  />
                </label>
                <button
                  type="button"
                  className="btn btn-sm"
                  onClick={() => {
                    const next = emptyMatrix(brush);
                    setMatrix(next);
                    scheduleApply({ matrix: next });
                  }}
                >
                  填满全键
                </button>
                <button
                  type="button"
                  className="btn btn-sm"
                  onClick={() => {
                    const next = emptyMatrix([0, 0, 0]);
                    setMatrix(next);
                    scheduleApply({ matrix: next });
                  }}
                >
                  全部熄灭
                </button>
                <span style={{ width: 1, height: 18, background: "var(--border-control)" }} />
                {MATRIX_PRESETS.map((p) => (
                  <button
                    key={p.id}
                    type="button"
                    className="btn btn-sm"
                    onClick={() => applyPreset(p.id)}
                  >
                    {p.label}
                  </button>
                ))}
                <label
                  style={{
                    marginLeft: "auto",
                    display: "inline-flex",
                    alignItems: "center",
                    gap: 6,
                    fontSize: 12,
                    color: "var(--ink-secondary)",
                    cursor: "pointer",
                  }}
                >
                  <input
                    type="checkbox"
                    checked={calibrate}
                    onChange={(e) => {
                      setCalibrate(e.target.checked);
                      scheduleApply({ calibrate: e.target.checked });
                    }}
                  />
                  OEM 白平衡校准
                </label>
              </div>
            )}

            <div
              className="keyboard-deck"
              onContextMenu={(e) => {
                if (mode === "matrix") e.preventDefault();
              }}
            >
              {KEYBOARD_101_ROWS.map((row, rIdx) => (
                <div key={rIdx} className="kbd-row">
                  {row.map((cap, cIdx) => {
                    const c = previewColorForKey(cap, rIdx, cIdx);
                    const lit = isOn && (c[0] > 0 || c[1] > 0 || c[2] > 0);
                    const dim = 0.35 + 0.65 * (brightnessPct / 100);
                    const glowColor = rgbCss(c, lit ? dim : 0);
                    return (
                      <button
                        key={`${rIdx}-${cIdx}-${cap.name}`}
                        type="button"
                        className="keycap"
                        style={{
                          flex: `${cap.width} 1 0%`,
                          borderColor: lit ? rgbCss(c, 0.72 * dim) : undefined,
                          boxShadow: lit ? `inset 0 -2px 8px ${rgbCss(c, 0.25 * dim)}` : undefined,
                        }}
                        onMouseDown={(e) => {
                          if (mode !== "matrix") return;
                          e.preventDefault();
                          const erase = e.button === 2;
                          setPainting(erase ? "erase" : "paint");
                          paintKey(cap, erase, true);
                        }}
                        onMouseEnter={() => {
                          if (mode === "matrix" && painting) {
                            paintKey(cap, painting === "erase", false);
                          }
                        }}
                        onContextMenu={(e) => {
                          e.preventDefault();
                        }}
                        title={
                          mode === "matrix"
                            ? `${cap.name}（左键上色 · 右键熄灭）`
                            : cap.name
                        }
                      >
                        <span>{cap.name}</span>
                        <span
                          className="keycap-led"
                          style={{
                            background: lit ? glowColor : "#1f2735",
                            boxShadow: lit ? `0 0 6px ${rgbCss(c, 0.65 * dim)}` : "none",
                          }}
                        />
                      </button>
                    );
                  })}
                </div>
              ))}
            </div>
          </>
        ) : (
          <div
            className="well"
            style={{
              padding: "24px 28px",
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
              gap: 16,
            }}
          >
            <div
              style={{
                width: "100%",
                maxWidth: 660,
                height: 76,
                borderRadius: 12,
                background: "linear-gradient(180deg, #171e2b 0%, #10151f 100%)",
                border: "1px solid #283448",
                position: "relative",
                display: "flex",
                alignItems: "flex-end",
                padding: "0 18px 10px",
              }}
            >
              <div
                style={{
                  width: "100%",
                  height: 7,
                  borderRadius: 999,
                  background: !isOn
                    ? "#1f2735"
                    : spec.single
                      ? rgbCss(color, 0.35 + 0.65 * (brightnessPct / 100))
                      : `linear-gradient(90deg, ${palette.map((p) => rgbCss(p)).join(", ")})`,
                  boxShadow: !isOn
                    ? "none"
                    : `0 0 18px ${rgbCss(spec.single ? color : palette[0], 0.55 * (brightnessPct / 100))}`,
                }}
              />
            </div>
          </div>
        )}
      </section>

      {/* Lower Control Bay */}
      <div className="split-2">
        <section className="panel">
          <div className="section-header">
            <div>
              <h3 className="section-title">选择灯效</h3>
              <p className="section-desc">{spec.desc}</p>
            </div>
          </div>
          <div className="tile-grid">
            {modes.map((m) => (
              <button
                key={m.id}
                type="button"
                className={`mode-tile${mode === m.id ? " selected" : ""}`}
                onClick={() => chooseMode(m.id)}
                title={m.desc}
              >
                {m.title}
              </button>
            ))}
          </div>
        </section>

        <section className="panel">
          <h3 className="section-title">灯光参数</h3>

          <div>
            <div className="field-row" style={{ marginBottom: 6 }}>
              <span className="field-label">亮度</span>
              <span className="mono" style={{ color: "var(--accent)", fontWeight: 600 }}>
                {brightnessPct}%
              </span>
            </div>
            <input
              type="range"
              min={0}
              max={100}
              step={1}
              disabled={spec.off}
              value={brightnessPct}
              onChange={(e) => {
                const v = Number(e.target.value);
                setBrightnessPct(v);
                scheduleApply({ brightnessPct: v });
              }}
            />
          </div>

          {spec.speed && (
            <div>
              <div className="field-row" style={{ marginBottom: 6 }}>
                <span className="field-label">速度</span>
                <span className="mono" style={{ color: "var(--accent)", fontWeight: 600 }}>
                  {speedUi} / 10
                </span>
              </div>
              <input
                type="range"
                min={1}
                max={10}
                step={1}
                value={speedUi}
                onChange={(e) => {
                  const v = Number(e.target.value);
                  setSpeedUi(v);
                  scheduleApply({ speedUi: v });
                }}
              />
            </div>
          )}

          {spec.direction && (
            <div className="field-row">
              <span className="field-label">流动方向</span>
              <select
                className="select"
                value={direction}
                onChange={(e) => {
                  const v = Number(e.target.value);
                  setDirection(v);
                  scheduleApply({ direction: v });
                }}
              >
                {directions.map((d) => (
                  <option key={d.value} value={d.value}>
                    {d.label}
                  </option>
                ))}
              </select>
            </div>
          )}

          {spec.trigger && (
            <div>
              <div className="field-row">
                <span className="field-label">触发方式</span>
                <select
                  className="select"
                  value={reactive ? "1" : "0"}
                  onChange={(e) => {
                    const v = e.target.value === "1";
                    setReactive(v);
                    scheduleApply({ reactive: v });
                  }}
                >
                  <option value="0">自动播放</option>
                  <option value="1">随按键触发</option>
                </select>
              </div>
              {reactive && (
                <div className="field-hint">按键触发时，空闲熄灭属于正常现象。</div>
              )}
            </div>
          )}

          {spec.single && (
            <div className="field-row">
              <span className="field-label">灯光颜色</span>
              <div style={{ display: "inline-flex", alignItems: "center", gap: 8 }}>
                <span className="mono" style={{ fontSize: 11.5, color: "var(--ink-muted)" }}>
                  {rgbToHex(color).toUpperCase()}
                </span>
                <input
                  type="color"
                  className="color-input"
                  value={rgbToHex(color)}
                  onChange={(e) => {
                    const rgb = hexToRgb(e.target.value);
                    setColor(rgb);
                    scheduleApply({ color: rgb });
                  }}
                />
              </div>
            </div>
          )}

          {spec.palette && (
            <div>
              <div className="field-row" style={{ marginBottom: 8 }}>
                <span className="field-label">七色调色板</span>
                <button
                  type="button"
                  className="btn btn-sm"
                  onClick={() => {
                    const next = RAINBOW_DEFAULT.map((c) => [...c] as Rgb);
                    setPalette(next);
                    scheduleApply({ palette: next });
                  }}
                >
                  <RotateCcw size={12} />
                  重置
                </button>
              </div>
              <div style={{ display: "grid", gridTemplateColumns: "repeat(7, 1fr)", gap: 6 }}>
                {palette.map((c, idx) => (
                  <input
                    key={idx}
                    type="color"
                    className="color-input"
                    style={{ width: "100%" }}
                    title={`第 ${idx + 1} 种颜色`}
                    value={rgbToHex(c)}
                    onChange={(e) => {
                      const next = palette.slice();
                      next[idx] = hexToRgb(e.target.value);
                      setPalette(next);
                      scheduleApply({ palette: next });
                    }}
                  />
                ))}
              </div>
            </div>
          )}
        </section>
      </div>

      <div className={`status-bar${isError ? " error" : ""}`}>
        <span>{status}</span>
      </div>
    </div>
  );
}

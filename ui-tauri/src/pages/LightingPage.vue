<!-- SPDX-License-Identifier: AGPL-3.0-only -->
<!-- This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md). -->

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from "vue";
import {
  NAlert,
  NButton,
  NCard,
  NCheckbox,
  NColorPicker,
  NInputNumber,
  NSelect,
  NSlider,
  NSwitch,
  NTabPane,
  NTabs,
  NTag,
  useMessage,
} from "naive-ui";
import { RotateCcw } from "lucide-vue-next";
import { api } from "../api/tauri";
import {
  hexToRgb,
  KEYBOARD_101_ROWS,
  KEYBOARD_DIRECTIONS,
  KEYBOARD_MODES,
  LIGHTBAR_DIRECTIONS,
  LIGHTBAR_MODES,
  LOGO_FIXED_COLORS,
  LOGO_MODES,
  MATRIX_PRESETS,
  RAINBOW_DEFAULT,
  rgbCss,
  rgbToHex,
  type KeyCapDef,
  type KeyboardHwInfo,
  type LightingPolicyConfig,
  type LightingSavedParams,
  type LightingTabId,
  type Rgb,
} from "../types/hardware";

const props = defineProps<{
  initialTab: LightingTabId;
}>();

const emit = defineEmits<{
  "update:tab": [tab: LightingTabId];
}>();

const message = useMessage();
const activeTab = ref<LightingTabId>(props.initialTab);

watch(
  () => props.initialTab,
  (v) => {
    activeTab.value = v;
  }
);

watch(activeTab, (v) => {
  emit("update:tab", v);
});

// ======================== KEYBOARD & LIGHTBAR STATE ========================

function emptyMatrix(fill: Rgb = [0, 0, 0]): Rgb[] {
  return Array.from({ length: 126 }, () => [...fill] as Rgb);
}

interface HidZoneState {
  available: boolean;
  loaded: boolean;
  mode: string;
  brightnessPct: number;
  speedUi: number;
  direction: number;
  reactive: boolean;
  color: Rgb;
  palette: Rgb[];
  brush: Rgb;
  matrix: Rgb[];
  calibrate: boolean;
  status: string;
  isError: boolean;
}

function makeZone(isKbd: boolean): HidZoneState {
  return {
    available: true,
    loaded: false,
    mode: isKbd ? "solid" : "static",
    brightnessPct: 60,
    speedUi: 6,
    direction: 1,
    reactive: false,
    color: [0, 200, 255],
    palette: RAINBOW_DEFAULT.map((c) => [...c] as Rgb),
    brush: [255, 40, 0],
    matrix: emptyMatrix([0, 0, 0]),
    calibrate: false,
    status: "正在读取设备状态…",
    isError: false,
  };
}

const kbd = ref<HidZoneState>(makeZone(true));
const lb = ref<HidZoneState>(makeZone(false));
const painting = ref<false | "paint" | "erase">(false);
const lastKbdHw = ref<KeyboardHwInfo | null>(null);
const hidTimers: Record<"keyboard" | "lightbar", number | null> = {
  keyboard: null,
  lightbar: null,
};
let alive = true;
let kbdApplying = false;

const kbdSpec = computed(() => KEYBOARD_MODES.find((m) => m.id === kbd.value.mode) ?? KEYBOARD_MODES[0]);
const lbSpec = computed(() => LIGHTBAR_MODES.find((m) => m.id === lb.value.mode) ?? LIGHTBAR_MODES[0]);
const kbdIsOn = computed(() => !kbdSpec.value.off && kbd.value.brightnessPct > 0);
const lbIsOn = computed(() => !lbSpec.value.off && lb.value.brightnessPct > 0);

const kbdDirOptions = KEYBOARD_DIRECTIONS.map((d) => ({ label: d.label, value: d.value }));
const lbDirOptions = LIGHTBAR_DIRECTIONS.map((d) => ({ label: d.label, value: d.value }));
const triggerOptions = [
  { label: "自动播放", value: "0" },
  { label: "随按键触发", value: "1" },
];

async function loadHidZone(device: "keyboard" | "lightbar") {
  const isKbd = device === "keyboard";
  const zone = isKbd ? kbd : lb;
  const modes = isKbd ? KEYBOARD_MODES : LIGHTBAR_MODES;
  try {
    const res = await api.getLightingState(device);
    zone.value.available = res.available;
    const saved = res.saved || {};
    if (saved.mode && modes.some((m) => m.id === saved.mode)) {
      zone.value.mode = saved.mode;
    }
    if (typeof saved.brightness === "number") {
      zone.value.brightnessPct = Math.round(Math.min(100, (saved.brightness / res.bmax) * 100));
    }
    if (typeof saved.speed === "number") {
      zone.value.speedUi = 11 - Math.min(10, Math.max(1, saved.speed));
    }
    if (typeof saved.direction === "number") {
      zone.value.direction = saved.direction;
    }
    if (typeof saved.reactive === "boolean") {
      zone.value.reactive = saved.reactive;
    }
    if (Array.isArray(saved.color) && saved.color.length === 3) {
      zone.value.color = saved.color as Rgb;
    }
    if (Array.isArray(saved.colors) && saved.colors.length > 0) {
      zone.value.palette = RAINBOW_DEFAULT.map((c, idx) =>
        saved.colors && saved.colors[idx] ? ([...saved.colors[idx]] as Rgb) : ([...c] as Rgb)
      );
    }
    if (isKbd) {
      if (Array.isArray(saved.matrix) && saved.matrix.length === 126) {
        zone.value.matrix = saved.matrix.map((c) => [...c] as Rgb);
      } else {
        try {
          const preset = saved.preset || "gaming";
          const accent = (saved.color as Rgb) || [0, 200, 255];
          zone.value.matrix = await api.getMatrixPreset(preset, accent);
        } catch {
          // keep default empty
        }
      }
      zone.value.calibrate = Boolean(saved.calibrate);
      if (res.hardware) {
        lastKbdHw.value = res.hardware;
        if (res.hardware.mode_name === "off") {
          zone.value.mode = "off";
          zone.value.brightnessPct = 0;
        } else {
          zone.value.mode = res.hardware.mode_name;
          zone.value.brightnessPct = Math.round(
            Math.min(100, (res.hardware.brightness / res.bmax) * 100)
          );
          if (res.hardware.speed >= 1 && res.hardware.speed <= 10) {
            zone.value.speedUi = 11 - res.hardware.speed;
          }
        }
      }
    }
    zone.value.status = res.available
      ? isKbd
        ? "已读取当前键盘背光硬件状态"
        : "已载入海岸灯带配置"
      : "未检测到 USB HID 设备节点，请确认 udev 规则已加载";
    zone.value.isError = !res.available;
    zone.value.loaded = true;
  } catch (err) {
    zone.value.status = `读取失败：${String(err)}`;
    zone.value.isError = true;
    zone.value.loaded = true;
  }
}

function scheduleHidApply(device: "keyboard" | "lightbar") {
  const isKbd = device === "keyboard";
  const zone = isKbd ? kbd.value : lb.value;
  if (!zone.loaded || !zone.available) return;
  const bmax = isKbd ? 50 : 100;
  const modes = isKbd ? KEYBOARD_MODES : LIGHTBAR_MODES;

  if (hidTimers[device] !== null) {
    window.clearTimeout(hidTimers[device]!);
  }
  hidTimers[device] = window.setTimeout(async () => {
    hidTimers[device] = null;
    const z = isKbd ? kbd.value : lb.value;
    const payload: LightingSavedParams = {
      mode: z.mode,
      color: z.color,
      colors: z.palette,
      brightness: Math.round((z.brightnessPct * bmax) / 100),
      speed: 11 - z.speedUi,
      direction: z.direction,
      reactive: z.reactive,
    };
    if (isKbd && z.mode === "matrix") {
      payload.matrix = z.matrix;
      if (z.calibrate) payload.calibrate = true;
    }
    try {
      if (isKbd) kbdApplying = true;
      const res = await api.applyLighting(device, payload);
      if (!alive) return;
      if (isKbd && res.hardware) lastKbdHw.value = res.hardware;
      const mInfo = modes.find((m) => m.id === z.mode);
      const suffix = mInfo?.trigger && z.reactive ? " · 随按键触发" : "";
      z.status = `已应用：${mInfo?.title ?? z.mode}${suffix}`;
      z.isError = false;
    } catch (err) {
      if (!alive) return;
      z.status = `设置未成功：${String(err)}`;
      z.isError = true;
    } finally {
      if (isKbd) kbdApplying = false;
    }
  }, 90);
}

function chooseHidMode(device: "keyboard" | "lightbar", nextMode: string) {
  const z = device === "keyboard" ? kbd.value : lb.value;
  if (nextMode !== "off" && z.brightnessPct === 0) {
    z.brightnessPct = 60;
  }
  z.mode = nextMode;
  scheduleHidApply(device);
}

function paintKey(cap: KeyCapDef, erase: boolean, toggleOnSame: boolean) {
  if (kbd.value.mode !== "matrix") return;
  const primary = cap.indices[0];
  const current = kbd.value.matrix[primary] ?? [0, 0, 0];
  const brush = kbd.value.brush;
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
  const next = kbd.value.matrix.slice();
  for (const idx of cap.indices) {
    if (idx >= 0 && idx < next.length) next[idx] = [...target] as Rgb;
  }
  kbd.value.matrix = next;
  scheduleHidApply("keyboard");
}

async function applyMatrixPreset(presetId: string) {
  try {
    const cells = await api.getMatrixPreset(presetId, kbd.value.brush);
    kbd.value.matrix = cells;
    kbd.value.mode = "matrix";
    if (kbd.value.brightnessPct === 0) kbd.value.brightnessPct = 60;
    scheduleHidApply("keyboard");
  } catch (err) {
    kbd.value.status = `应用预设失败：${String(err)}`;
    kbd.value.isError = true;
  }
}

function previewColorForKey(cap: KeyCapDef, rowIdx: number, colIdx: number): Rgb {
  if (!kbdIsOn.value) return [32, 40, 54];
  if (kbd.value.mode === "matrix") return kbd.value.matrix[cap.indices[0]] ?? [0, 0, 0];
  if (kbdSpec.value.single) return kbd.value.color;
  return kbd.value.palette[(rowIdx + Math.floor(colIdx / 2)) % kbd.value.palette.length];
}

// ======================== REAR-LID LOGO STATE ========================

const LOGO_STEPS = [0, 25, 50, 75, 100] as const;
const logoMode = ref("solid");
const logoBright = ref(25);
const logoColor = ref<Rgb>([0, 200, 255]);
const logoHwSummary = ref("正在读取后盖 LOGO 状态…");
const logoStatus = ref("修改参数后点击“应用 LOGO 灯光”写入 EC 寄存器");
const logoError = ref(false);
const logoBusy = ref(false);
const logoInit = ref(false);

async function loadLogoStatus(syncControls: boolean) {
  try {
    const res = await api.getLogoStatus();
    const hw = res.hardware;
    const caption = LOGO_MODES.find((m) => m.id === hw.mode)?.label ?? hw.mode;
    logoHwSummary.value = hw.logo_enabled
      ? `当前硬件：${caption} · ${hw.brightness}%`
      : "当前硬件：LOGO 灯已关闭";
    if (syncControls) {
      const src = hw.logo_enabled ? hw : res.prefs ?? hw;
      logoMode.value = src.mode;
      logoBright.value = src.brightness;
      logoColor.value = src.color;
      logoInit.value = true;
    }
    logoError.value = false;
  } catch (err) {
    logoStatus.value = `读取 LOGO 状态失败：${String(err)}`;
    logoError.value = true;
  }
}

async function applyLogo() {
  logoBusy.value = true;
  logoError.value = false;
  logoStatus.value = "正在发送 LOGO 灯光指令并核对回读…";
  try {
    const effectiveColor =
      logoMode.value === "breathing" &&
      !LOGO_FIXED_COLORS.some(
        (c) =>
          c.rgb[0] === logoColor.value[0] &&
          c.rgb[1] === logoColor.value[1] &&
          c.rgb[2] === logoColor.value[2]
      )
        ? LOGO_FIXED_COLORS[0].rgb
        : logoColor.value;
    const hw = await api.applyLogo(logoMode.value, logoBright.value, effectiveColor);
    const caption = LOGO_MODES.find((m) => m.id === hw.mode)?.label ?? hw.mode;
    logoHwSummary.value = hw.logo_enabled
      ? `当前硬件：${caption} · ${hw.brightness}%`
      : "当前硬件：LOGO 灯已关闭";
    logoStatus.value = "设置已发送，EC 寄存器回读一致";
    message.success("后盖 LOGO 灯光已生效");
  } catch (err) {
    logoStatus.value = `LOGO 控制失败：${String(err)}`;
    logoError.value = true;
    message.error(String(err));
  } finally {
    logoBusy.value = false;
  }
}

async function turnLogoOff() {
  logoBusy.value = true;
  logoError.value = false;
  try {
    await api.setLogoEnabled(false);
    logoBright.value = 0;
    logoHwSummary.value = "当前硬件：LOGO 灯已关闭";
    logoStatus.value = "已关闭后盖 LOGO（保留先前配色）";
    message.info("后盖 LOGO 已关闭");
  } catch (err) {
    logoStatus.value = `关闭失败：${String(err)}`;
    logoError.value = true;
  } finally {
    logoBusy.value = false;
  }
}

// ======================== AUTO LIGHTING POLICY STATE ========================

const policy = ref<LightingPolicyConfig>({
  version: 1,
  enabled: false,
  power_enabled: false,
  idle_minutes: 0,
  brightness: {
    keyboard: { ac: 100, dc: 100 },
    lightbar: { ac: 100, dc: 100 },
    logo: { ac: 100, dc: 100 },
  },
});
const policyStatus = ref("正在读取自动灯光策略…");
const policyError = ref(false);
const policySaving = ref(false);
const logoStepOptions = [0, 25, 50, 75, 100].map((v) => ({ label: `${v}%`, value: v }));
const POLICY_ZONES: {
  key: keyof LightingPolicyConfig["brightness"];
  label: string;
  stepped?: boolean;
}[] = [
  { key: "keyboard", label: "键盘背光" },
  { key: "lightbar", label: "海岸灯带" },
  { key: "logo", label: "后盖 LOGO", stepped: true },
];

async function loadPolicy() {
  try {
    const loaded = await api.getLightingPolicy();
    policy.value = loaded;
    policyStatus.value = loaded.enabled
      ? "已载入自动灯光策略，由后台 iteglow-sessiond 守护实时执行。"
      : "自动策略当前关闭；修改后点击“保存策略”生效。";
    policyError.value = false;
  } catch (err) {
    policyStatus.value = `读取策略失败：${String(err)}`;
    policyError.value = true;
  }
}

async function savePolicy() {
  policySaving.value = true;
  policyError.value = false;
  try {
    const saved = await api.saveLightingPolicy(policy.value);
    policy.value = saved;
    policyStatus.value = saved.enabled
      ? "已保存；自动灯光策略已启用，后台守护将在 5 秒内生效。"
      : "已保存；自动灯光策略已关闭。";
    message.success("自动灯光策略已保存");
  } catch (err) {
    policyStatus.value = `保存失败：${String(err)}`;
    policyError.value = true;
    message.error(String(err));
  } finally {
    policySaving.value = false;
  }
}

// ======================== LIFECYCLE ========================

let hotkeyTimer: number | null = null;
let logoTimer: number | null = null;
const onMouseUp = () => {
  painting.value = false;
};

onMounted(() => {
  alive = true;
  window.addEventListener("mouseup", onMouseUp);
  window.addEventListener("blur", onMouseUp);
  loadHidZone("keyboard");
  loadHidZone("lightbar");
  loadLogoStatus(true);
  loadPolicy();

  hotkeyTimer = window.setInterval(async () => {
    if (
      activeTab.value !== "keyboard" ||
      !kbd.value.available ||
      !kbd.value.loaded ||
      kbdApplying ||
      hidTimers.keyboard !== null
    ) {
      return;
    }
    try {
      const hw = await api.pollKeyboardHw();
      if (!alive || kbdApplying || hidTimers.keyboard !== null) return;
      const prev = lastKbdHw.value;
      if (!prev) {
        lastKbdHw.value = hw;
        return;
      }
      if (
        hw.power !== prev.power ||
        hw.brightness !== prev.brightness ||
        hw.mode !== prev.mode ||
        hw.direction !== prev.direction
      ) {
        lastKbdHw.value = hw;
        if (hw.power === 1 || hw.brightness === 0) {
          kbd.value.brightnessPct = 0;
          kbd.value.status = "已同步硬件快捷键：键盘背光关闭";
        } else {
          const pct = Math.round(Math.min(100, (hw.brightness / 50) * 100));
          kbd.value.brightnessPct = pct;
          if (hw.mode_name !== "off" && kbd.value.mode !== hw.mode_name) {
            kbd.value.mode = hw.mode_name;
          }
          kbd.value.status = `已同步硬件快捷键：${pct}%`;
        }
        kbd.value.isError = false;
      }
    } catch {
      // ignore transient poll failure
    }
  }, 950);

  logoTimer = window.setInterval(() => {
    if (!logoBusy.value && activeTab.value === "logo") {
      loadLogoStatus(!logoInit.value);
    }
  }, 4000);
});

onUnmounted(() => {
  alive = false;
  window.removeEventListener("mouseup", onMouseUp);
  window.removeEventListener("blur", onMouseUp);
  if (hotkeyTimer !== null) window.clearInterval(hotkeyTimer);
  if (logoTimer !== null) window.clearInterval(logoTimer);
  if (hidTimers.keyboard !== null) window.clearTimeout(hidTimers.keyboard);
  if (hidTimers.lightbar !== null) window.clearTimeout(hidTimers.lightbar);
});
</script>

<template>
  <div class="page-stack">
    <n-tabs v-model:value="activeTab" type="segment" animated>
      <!-- ==================== TAB 1: KEYBOARD ==================== -->
      <n-tab-pane name="keyboard" tab="键盘背光">
        <div class="page-stack" style="margin-top: 12px">
          <section class="ccu-hero" style="--hero-accent: var(--accent-cyan)">
            <div class="ccu-field-row" style="margin-bottom: 14px">
              <div>
                <h2 class="ccu-section-title">101 键单键 RGB 物理视图</h2>
                <p class="ccu-section-sub">
                  {{
                    kbd.mode === "matrix"
                      ? "左键点击或拖动上色，右键擦除熄灭；实时下发至 ITE 8291 控制器"
                      : "布局与配色实时示意，Fn+F6 / Fn+F7 硬件亮度快捷键双向同步"
                  }}
                </p>
              </div>
              <n-tag round :bordered="false" :type="kbd.available ? 'success' : 'error'">
                {{ kbd.available ? "USB HID 已连接" : "未检测到键盘 HID" }}
              </n-tag>
            </div>

            <!-- Matrix Brush Toolbar -->
            <div v-if="kbd.mode === 'matrix'" class="ccu-well matrix-toolbar">
              <div style="display: inline-flex; align-items: center; gap: 8px">
                <span class="ccu-field-label">画笔颜色</span>
                <n-color-picker
                  size="small"
                  style="width: 96px"
                  :show-alpha="false"
                  :modes="['hex']"
                  :value="rgbToHex(kbd.brush)"
                  @update:value="(hex) => (kbd.brush = hexToRgb(hex))"
                />
              </div>
              <n-button
                size="small"
                secondary
                @click="
                  () => {
                    kbd.matrix = emptyMatrix(kbd.brush);
                    scheduleHidApply('keyboard');
                  }
                "
              >
                填满全键
              </n-button>
              <n-button
                size="small"
                secondary
                @click="
                  () => {
                    kbd.matrix = emptyMatrix([0, 0, 0]);
                    scheduleHidApply('keyboard');
                  }
                "
              >
                全部熄灭
              </n-button>
              <span class="toolbar-sep" />
              <n-button
                v-for="p in MATRIX_PRESETS"
                :key="p.id"
                size="small"
                secondary
                type="info"
                @click="applyMatrixPreset(p.id)"
              >
                {{ p.label }}
              </n-button>
              <n-checkbox
                v-model:checked="kbd.calibrate"
                style="margin-left: auto"
                @update:checked="() => scheduleHidApply('keyboard')"
              >
                OEM 白平衡校准
              </n-checkbox>
            </div>

            <!-- Physical 101-key Deck -->
            <div
              class="keyboard-chassis"
              style="margin-top: 10px"
              @contextmenu="(e) => kbd.mode === 'matrix' && e.preventDefault()"
            >
              <div v-for="(row, rIdx) in KEYBOARD_101_ROWS" :key="rIdx" class="kbd-row">
                <button
                  v-for="(cap, cIdx) in row"
                  :key="`${rIdx}-${cIdx}-${cap.name}`"
                  type="button"
                  class="keycap"
                  :style="
                    (() => {
                      const c = previewColorForKey(cap, rIdx, cIdx);
                      const lit = kbdIsOn && (c[0] > 0 || c[1] > 0 || c[2] > 0);
                      const dim = 0.35 + 0.65 * (kbd.brightnessPct / 100);
                      return {
                        flex: `${cap.width} 1 0%`,
                        borderColor: lit ? rgbCss(c, 0.72 * dim) : undefined,
                        boxShadow: lit ? `inset 0 -2px 9px ${rgbCss(c, 0.28 * dim)}` : undefined,
                      };
                    })()
                  "
                  :title="kbd.mode === 'matrix' ? `${cap.name}（左键上色 · 右键熄灭）` : cap.name"
                  @mousedown="
                    (e) => {
                      if (kbd.mode !== 'matrix' || (e.button !== 0 && e.button !== 2)) return;
                      e.preventDefault();
                      const erase = e.button === 2;
                      painting = erase ? 'erase' : 'paint';
                      paintKey(cap, erase, true);
                    }
                  "
                  @mouseenter="
                    () => {
                      if (kbd.mode === 'matrix' && painting) {
                        paintKey(cap, painting === 'erase', false);
                      }
                    }
                  "
                  @contextmenu.prevent
                >
                  <span>{{ cap.name }}</span>
                  <span
                    class="keycap-led"
                    :style="
                      (() => {
                        const c = previewColorForKey(cap, rIdx, cIdx);
                        const lit = kbdIsOn && (c[0] > 0 || c[1] > 0 || c[2] > 0);
                        const dim = 0.35 + 0.65 * (kbd.brightnessPct / 100);
                        return {
                          background: lit ? rgbCss(c, dim) : '#1E2738',
                          boxShadow: lit ? `0 0 7px ${rgbCss(c, 0.7 * dim)}` : 'none',
                        };
                      })()
                    "
                  />
                </button>
              </div>
            </div>
          </section>

          <div class="grid-lighting">
            <n-card :bordered="true">
              <div style="margin-bottom: 12px">
                <h3 class="ccu-section-title">灯效模式</h3>
                <p class="ccu-section-sub">{{ kbdSpec.desc }}</p>
              </div>
              <div class="effect-tile-grid">
                <button
                  v-for="m in KEYBOARD_MODES"
                  :key="m.id"
                  type="button"
                  class="effect-tile"
                  :class="{ selected: kbd.mode === m.id }"
                  :title="m.desc"
                  @click="chooseHidMode('keyboard', m.id)"
                >
                  {{ m.title }}
                </button>
              </div>
            </n-card>

            <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:16px;">
              <h3 class="ccu-section-title">灯光参数</h3>

              <div>
                <div class="ccu-field-row" style="margin-bottom: 6px">
                  <span class="ccu-field-label">亮度</span>
                  <span class="mono" style="color: var(--accent-cyan); font-weight: 700">
                    {{ kbd.brightnessPct }}%
                  </span>
                </div>
                <n-slider
                  v-model:value="kbd.brightnessPct"
                  :min="0"
                  :max="100"
                  :disabled="Boolean(kbdSpec.off)"
                  @update:value="() => scheduleHidApply('keyboard')"
                />
              </div>

              <div v-if="kbdSpec.speed">
                <div class="ccu-field-row" style="margin-bottom: 6px">
                  <span class="ccu-field-label">动态速度</span>
                  <span class="mono" style="color: var(--accent-cyan); font-weight: 700">
                    {{ kbd.speedUi }} / 10
                  </span>
                </div>
                <n-slider
                  v-model:value="kbd.speedUi"
                  :min="1"
                  :max="10"
                  @update:value="() => scheduleHidApply('keyboard')"
                />
              </div>

              <div v-if="kbdSpec.direction" class="ccu-field-row">
                <span class="ccu-field-label">流动方向</span>
                <n-select
                  v-model:value="kbd.direction"
                  style="width: 180px"
                  :options="kbdDirOptions"
                  @update:value="() => scheduleHidApply('keyboard')"
                />
              </div>

              <div v-if="kbdSpec.trigger">
                <div class="ccu-field-row">
                  <span class="ccu-field-label">触发方式</span>
                  <n-select
                    :value="kbd.reactive ? '1' : '0'"
                    style="width: 180px"
                    :options="triggerOptions"
                    @update:value="
                      (v: string) => {
                        kbd.reactive = v === '1';
                        scheduleHidApply('keyboard');
                      }
                    "
                  />
                </div>
                <div v-if="kbd.reactive" class="ccu-field-hint">
                  按键触发时，空闲熄灭属于正常现象。
                </div>
              </div>

              <div v-if="kbdSpec.single" class="ccu-field-row">
                <span class="ccu-field-label">常亮颜色</span>
                <n-color-picker
                  style="width: 150px"
                  :show-alpha="false"
                  :modes="['hex']"
                  :value="rgbToHex(kbd.color)"
                  @update:value="
                    (hex) => {
                      kbd.color = hexToRgb(hex);
                      scheduleHidApply('keyboard');
                    }
                  "
                />
              </div>

              <div v-if="kbdSpec.palette">
                <div class="ccu-field-row" style="margin-bottom: 8px">
                  <span class="ccu-field-label">七色调色板</span>
                  <n-button
                    size="tiny"
                    secondary
                    @click="
                      () => {
                        kbd.palette = RAINBOW_DEFAULT.map((c) => [...c] as Rgb);
                        scheduleHidApply('keyboard');
                      }
                    "
                  >
                    <template #icon><RotateCcw :size="12" /></template>
                    重置默认
                  </n-button>
                </div>
                <div class="palette-7">
                  <n-color-picker
                    v-for="(c, idx) in kbd.palette"
                    :key="idx"
                    size="small"
                    :show-alpha="false"
                    :modes="['hex']"
                    :value="rgbToHex(c)"
                    @update:value="
                      (hex) => {
                        const next = kbd.palette.slice();
                        next[idx] = hexToRgb(hex);
                        kbd.palette = next;
                        scheduleHidApply('keyboard');
                      }
                    "
                  />
                </div>
              </div>
            </n-card>
          </div>

          <n-alert :type="kbd.isError ? 'error' : 'info'" :show-icon="true">
            {{ kbd.status }}
          </n-alert>
        </div>
      </n-tab-pane>

      <!-- ==================== TAB 2: LIGHTBAR ==================== -->
      <n-tab-pane name="lightbar" tab="海岸灯带">
        <div class="page-stack" style="margin-top: 12px">
          <section class="ccu-hero" style="--hero-accent: var(--accent-cyan)">
            <div class="ccu-field-row" style="margin-bottom: 14px">
              <div>
                <h2 class="ccu-section-title">机身前缘海岸灯带</h2>
                <p class="ccu-section-sub">前缘与侧翼氛围灯实时效果预览</p>
              </div>
              <n-tag round :bordered="false" :type="lb.available ? 'success' : 'error'">
                {{ lb.available ? "USB HID 已连接" : "未检测到灯带 HID" }}
              </n-tag>
            </div>

            <div class="lightbar-stage">
              <div class="lightbar-chassis">
                <div class="lightbar-notch" />
                <div
                  class="lightbar-strip"
                  :style="{
                    background: !lbIsOn
                      ? '#1f293b'
                      : lbSpec.single
                        ? rgbCss(lb.color, 0.35 + 0.65 * (lb.brightnessPct / 100))
                        : `linear-gradient(90deg, ${lb.palette.map((p) => rgbCss(p)).join(', ')})`,
                    boxShadow: !lbIsOn
                      ? 'none'
                      : `0 0 24px ${rgbCss(
                          lbSpec.single ? lb.color : lb.palette[0],
                          0.65 * (lb.brightnessPct / 100)
                        )}`,
                  }"
                />
              </div>
            </div>
          </section>

          <div class="grid-lighting">
            <n-card :bordered="true">
              <div style="margin-bottom: 12px">
                <h3 class="ccu-section-title">灯带效果</h3>
                <p class="ccu-section-sub">{{ lbSpec.desc }}</p>
              </div>
              <div class="effect-tile-grid">
                <button
                  v-for="m in LIGHTBAR_MODES"
                  :key="m.id"
                  type="button"
                  class="effect-tile"
                  :class="{ selected: lb.mode === m.id }"
                  @click="chooseHidMode('lightbar', m.id)"
                >
                  {{ m.title }}
                </button>
              </div>
            </n-card>

            <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:16px;">
              <h3 class="ccu-section-title">灯带参数</h3>

              <div>
                <div class="ccu-field-row" style="margin-bottom: 6px">
                  <span class="ccu-field-label">亮度</span>
                  <span class="mono" style="color: var(--accent-cyan); font-weight: 700">
                    {{ lb.brightnessPct }}%
                  </span>
                </div>
                <n-slider
                  v-model:value="lb.brightnessPct"
                  :min="0"
                  :max="100"
                  :disabled="Boolean(lbSpec.off)"
                  @update:value="() => scheduleHidApply('lightbar')"
                />
              </div>

              <div v-if="lbSpec.speed">
                <div class="ccu-field-row" style="margin-bottom: 6px">
                  <span class="ccu-field-label">速度</span>
                  <span class="mono" style="color: var(--accent-cyan); font-weight: 700">
                    {{ lb.speedUi }} / 10
                  </span>
                </div>
                <n-slider
                  v-model:value="lb.speedUi"
                  :min="1"
                  :max="10"
                  @update:value="() => scheduleHidApply('lightbar')"
                />
              </div>

              <div v-if="lbSpec.direction" class="ccu-field-row">
                <span class="ccu-field-label">流动方向</span>
                <n-select
                  v-model:value="lb.direction"
                  style="width: 180px"
                  :options="lbDirOptions"
                  @update:value="() => scheduleHidApply('lightbar')"
                />
              </div>

              <div v-if="lbSpec.single" class="ccu-field-row">
                <span class="ccu-field-label">灯带颜色</span>
                <n-color-picker
                  style="width: 150px"
                  :show-alpha="false"
                  :modes="['hex']"
                  :value="rgbToHex(lb.color)"
                  @update:value="
                    (hex) => {
                      lb.color = hexToRgb(hex);
                      scheduleHidApply('lightbar');
                    }
                  "
                />
              </div>

              <div v-if="lbSpec.palette">
                <div class="ccu-field-row" style="margin-bottom: 8px">
                  <span class="ccu-field-label">七色调色板</span>
                  <n-button
                    size="tiny"
                    secondary
                    @click="
                      () => {
                        lb.palette = RAINBOW_DEFAULT.map((c) => [...c] as Rgb);
                        scheduleHidApply('lightbar');
                      }
                    "
                  >
                    <template #icon><RotateCcw :size="12" /></template>
                    重置默认
                  </n-button>
                </div>
                <div class="palette-7">
                  <n-color-picker
                    v-for="(c, idx) in lb.palette"
                    :key="idx"
                    size="small"
                    :show-alpha="false"
                    :modes="['hex']"
                    :value="rgbToHex(c)"
                    @update:value="
                      (hex) => {
                        const next = lb.palette.slice();
                        next[idx] = hexToRgb(hex);
                        lb.palette = next;
                        scheduleHidApply('lightbar');
                      }
                    "
                  />
                </div>
              </div>
            </n-card>
          </div>

          <n-alert :type="lb.isError ? 'error' : 'info'" :show-icon="true">
            {{ lb.status }}
          </n-alert>
        </div>
      </n-tab-pane>

      <!-- ==================== TAB 3: REAR-LID LOGO ==================== -->
      <n-tab-pane name="logo" tab="后盖 LOGO">
        <div class="page-stack" style="margin-top: 12px">
          <section class="ccu-hero" style="--hero-accent: var(--accent-violet)">
            <div class="ccu-field-row" style="margin-bottom: 14px">
              <div>
                <h2 class="ccu-section-title">屏幕后盖独立发光徽标</h2>
                <p class="ccu-section-sub">{{ logoHwSummary }}</p>
              </div>
              <n-tag round :bordered="false" :type="logoBright > 0 ? 'success' : 'default'" class="mono">
                {{ logoBright > 0 ? `${logoBright}%` : "已关闭" }}
              </n-tag>
            </div>

            <div class="logo-stage">
              <div class="logo-lid-box">
                <div
                  v-if="logoBright > 0"
                  class="logo-aura"
                  :style="{
                    background: rgbCss(
                      logoMode === 'rainbow' ? [0, 200, 255] : logoColor,
                      0.28 * (logoBright / 100)
                    ),
                  }"
                />
                <svg width="88" height="58" viewBox="0 0 78 52" fill="none" style="position: relative">
                  <path
                    d="M10 42L25 10L39 30L53 10L68 42"
                    :stroke="
                      logoBright > 0
                        ? rgbCss(
                            logoMode === 'rainbow' ? [0, 200, 255] : logoColor,
                            0.4 + 0.6 * (logoBright / 100)
                          )
                        : '#334155'
                    "
                    stroke-width="5.2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </svg>
              </div>
            </div>
          </section>

          <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:14px;">
            <div class="ccu-well ccu-field-row">
              <div>
                <div class="ccu-field-label">发光模式</div>
                <div class="ccu-field-hint">呼吸支持七种预设颜色，常亮支持任意自选 RGB</div>
              </div>
              <div style="display: flex; gap: 8px">
                <n-button
                  v-for="m in LOGO_MODES"
                  :key="m.id"
                  size="small"
                  :type="logoMode === m.id ? 'primary' : 'default'"
                  :secondary="logoMode !== m.id"
                  @click="
                    () => {
                      logoMode = m.id;
                      if (
                        m.id === 'breathing' &&
                        !LOGO_FIXED_COLORS.some(
                          (c) =>
                            c.rgb[0] === logoColor[0] &&
                            c.rgb[1] === logoColor[1] &&
                            c.rgb[2] === logoColor[2]
                        )
                      ) {
                        logoColor = [...LOGO_FIXED_COLORS[0].rgb];
                      }
                    }
                  "
                >
                  {{ m.label }}
                </n-button>
              </div>
            </div>

            <div class="ccu-well ccu-field-row">
              <div>
                <div class="ccu-field-label">硬件步进亮度</div>
                <div class="ccu-field-hint">EC 寄存器支持 5 档固定步进</div>
              </div>
              <div style="display: flex; gap: 6px">
                <n-button
                  v-for="b in LOGO_STEPS"
                  :key="b"
                  size="small"
                  class="mono"
                  :type="logoBright === b ? 'primary' : 'default'"
                  :secondary="logoBright !== b"
                  @click="logoBright = b"
                >
                  {{ b === 0 ? "关闭" : `${b}%` }}
                </n-button>
              </div>
            </div>

            <div v-if="logoMode !== 'rainbow'" class="ccu-well" style="display: flex; flex-direction: column; gap: 12px">
              <div class="ccu-field-row">
                <span class="ccu-field-label">预设七色</span>
                <div v-if="logoMode === 'solid'" style="display: inline-flex; align-items: center; gap: 8px">
                  <span class="ccu-field-hint" style="margin: 0">自选 RGB</span>
                  <n-color-picker
                    size="small"
                    style="width: 128px"
                    :show-alpha="false"
                    :modes="['hex']"
                    :value="rgbToHex(logoColor)"
                    @update:value="(hex) => (logoColor = hexToRgb(hex))"
                  />
                </div>
              </div>
              <div style="display: flex; flex-wrap: wrap; gap: 8px">
                <n-button
                  v-for="item in LOGO_FIXED_COLORS"
                  :key="item.name"
                  size="small"
                  :type="
                    item.rgb[0] === logoColor[0] &&
                    item.rgb[1] === logoColor[1] &&
                    item.rgb[2] === logoColor[2]
                      ? 'primary'
                      : 'default'
                  "
                  :secondary="
                    !(
                      item.rgb[0] === logoColor[0] &&
                      item.rgb[1] === logoColor[1] &&
                      item.rgb[2] === logoColor[2]
                    )
                  "
                  @click="logoColor = [...item.rgb]"
                >
                  <span
                    class="status-dot"
                    :style="{
                      background: rgbCss(item.rgb),
                      marginRight: '6px',
                      border: '1px solid var(--border-strong)',
                    }"
                  />
                  {{ item.name }}
                </n-button>
              </div>
            </div>

            <div style="display: flex; gap: 10px">
              <n-button type="primary" :loading="logoBusy" :disabled="!logoInit" @click="applyLogo">
                应用 LOGO 灯光
              </n-button>
              <n-button type="error" secondary :disabled="logoBusy || !logoInit" @click="turnLogoOff">
                关闭 LOGO
              </n-button>
            </div>
          </n-card>

          <n-alert :type="logoError ? 'error' : 'info'" :show-icon="true">
            {{ logoStatus }}
          </n-alert>
        </div>
      </n-tab-pane>

      <!-- ==================== TAB 4: AUTO SLEEP & POWER POLICY ==================== -->
      <n-tab-pane name="policy" tab="灯效睡眠与供电策略">
        <div class="page-stack" style="margin-top: 12px">
          <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:14px;">
            <div class="ccu-well ccu-field-row">
              <div>
                <div class="ccu-field-label">启用自动灯光与睡眠策略</div>
                <div class="ccu-field-hint">
                  按插电 / 电池状态独立控制三区亮度，键鼠空闲超时后自动熄灯并在操作恢复后还原
                </div>
              </div>
              <n-switch v-model:value="policy.enabled" />
            </div>

            <div class="ccu-well ccu-field-row" :style="{ opacity: policy.enabled ? 1 : 0.45 }">
              <div>
                <div class="ccu-field-label">按供电状态分别控制三区亮度</div>
                <div class="ccu-field-hint">仅作用于当前开启的灯区，手动关闭的灯区不会被自动点亮</div>
              </div>
              <n-switch v-model:value="policy.power_enabled" :disabled="!policy.enabled" />
            </div>

            <div
              class="ccu-well"
              style="display: flex; flex-direction: column; gap: 10px"
              :style="{ opacity: policy.enabled && policy.power_enabled ? 1 : 0.45 }"
            >
              <div class="policy-grid policy-head">
                <span>灯光区域</span>
                <span>插电亮度 (AC)</span>
                <span>电池亮度 (DC)</span>
              </div>
              <div v-for="z in POLICY_ZONES" :key="z.key" class="policy-grid">
                <span class="ccu-field-label">{{ z.label }}</span>
                <template v-for="src in (['ac', 'dc'] as const)" :key="src">
                  <n-select
                    v-if="z.stepped"
                    size="small"
                    style="width: 132px"
                    :disabled="!policy.enabled || !policy.power_enabled"
                    :options="logoStepOptions"
                    :value="policy.brightness[z.key][src]"
                    @update:value="(v: number) => (policy.brightness[z.key][src] = v)"
                  />
                  <n-input-number
                    v-else
                    size="small"
                    style="width: 132px"
                    :min="0"
                    :max="100"
                    :disabled="!policy.enabled || !policy.power_enabled"
                    :value="policy.brightness[z.key][src]"
                    @update:value="(v) => (policy.brightness[z.key][src] = v ?? 0)"
                  >
                    <template #suffix>%</template>
                  </n-input-number>
                </template>
              </div>
            </div>

            <div class="ccu-well ccu-field-row" :style="{ opacity: policy.enabled ? 1 : 0.45 }">
              <div>
                <div class="ccu-field-label">灯效睡眠时间（空闲自动熄灯）</div>
                <div class="ccu-field-hint">设为 0 表示不启用空闲熄灯，最大支持 1440 分钟</div>
              </div>
              <n-input-number
                size="small"
                style="width: 148px"
                :min="0"
                :max="1440"
                :disabled="!policy.enabled"
                :value="policy.idle_minutes"
                @update:value="(v) => (policy.idle_minutes = v ?? 0)"
              >
                <template #suffix>分钟</template>
              </n-input-number>
            </div>

            <div class="ccu-field-hint">
              自动策略产生的临时亮度不会覆盖手动灯效记录；关闭策略或退出会话时不残留临时状态。
            </div>

            <div>
              <n-button type="primary" :loading="policySaving" @click="savePolicy">
                保存自动灯光策略
              </n-button>
            </div>
          </n-card>

          <n-alert :type="policyError ? 'error' : 'info'" :show-icon="true">
            {{ policyStatus }}
          </n-alert>
        </div>
      </n-tab-pane>
    </n-tabs>
  </div>
</template>

<style scoped>
.matrix-toolbar {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px;
}

.toolbar-sep {
  width: 1px;
  height: 18px;
  background: rgba(148, 163, 184, 0.22);
}

.palette-7 {
  display: grid;
  grid-template-columns: repeat(7, minmax(0, 1fr));
  gap: 6px;
}

.lightbar-stage {
  display: grid;
  place-items: center;
  padding: 26px 20px;
  border-radius: 11px;
  background: radial-gradient(ellipse 70% 80% at 50% 60%, rgba(18, 28, 48, 0.9), #080b12 75%);
  border: 1px solid rgba(148, 163, 184, 0.14);
}

.lightbar-chassis {
  width: 100%;
  max-width: 680px;
  height: 82px;
  border-radius: 14px;
  background: linear-gradient(180deg, #171f31 0%, #0d121d 100%);
  border: 1px solid rgba(148, 163, 184, 0.22);
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-items: center;
  padding: 10px 22px 12px;
}

.lightbar-notch {
  width: 86px;
  height: 5px;
  border-radius: 999px;
  background: rgba(148, 163, 184, 0.2);
}

.lightbar-strip {
  width: 100%;
  height: 8px;
  border-radius: 999px;
  transition: background 180ms ease, box-shadow 180ms ease;
}

.logo-stage {
  display: grid;
  place-items: center;
  padding: 24px 16px;
  border-radius: 11px;
  background: radial-gradient(circle at 50% 50%, #141c2e 0%, #080b12 72%);
  border: 1px solid rgba(148, 163, 184, 0.14);
}

.logo-lid-box {
  width: 360px;
  max-width: 100%;
  height: 156px;
  border-radius: 14px;
  border: 1px solid rgba(148, 163, 184, 0.22);
  background: linear-gradient(155deg, #171f32 0%, #0d121d 100%);
  display: grid;
  place-items: center;
  position: relative;
  overflow: hidden;
}

.logo-aura {
  position: absolute;
  width: 132px;
  height: 132px;
  border-radius: 50%;
  filter: blur(26px);
}

.policy-grid {
  display: grid;
  grid-template-columns: 1fr 150px 150px;
  gap: 12px;
  align-items: center;
}

.policy-head {
  font-size: 11.5px;
  color: var(--ink-muted);
  padding-bottom: 6px;
  border-bottom: 1px solid var(--border-subtle);
}
</style>

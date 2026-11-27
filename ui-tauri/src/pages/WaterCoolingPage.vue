<!-- SPDX-License-Identifier: AGPL-3.0-only -->
<!-- This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md). -->

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, shallowRef } from "vue";
import {
  NAlert,
  NButton,
  NCard,
  NColorPicker,
  NInput,
  NInputNumber,
  NSelect,
  NSlider,
  NSwitch,
  NTag,
  useMessage,
} from "naive-ui";
import { api } from "../api/tauri";
import {
  hexToRgb,
  rgbCss,
  rgbToHex,
  type Rgb,
  type WaterCoolingConfig,
  type WaterCoolingSnapshot,
} from "../types/hardware";

const message = useMessage();

const STRATEGY_OPTIONS = [
  { value: "preset", label: "预设模式" },
  { value: "manual", label: "手动固定" },
  { value: "smart_curve", label: "智能温控曲线" },
  { value: "ec_follow", label: "跟随主机 EC" },
];

const PRESET_CARDS = [
  { id: "stop", title: "停机", spec: "0V · 泵 0% · 扇 0%", desc: "静置待机" },
  { id: "silent", title: "静音", spec: "7V · 泵 60% · 扇 40%", desc: "低噪办公" },
  { id: "balanced", title: "均衡", spec: "8V · 泵 60% · 扇 50%", desc: "日常通用" },
  { id: "performance", title: "性能", spec: "11V · 泵 60% · 扇 60%", desc: "高负载散热" },
  { id: "turbo", title: "狂暴", spec: "11V · 泵 90% · 扇 90%", desc: "游戏激冷" },
  { id: "max", title: "全速", spec: "12V · 泵 100% · 扇 100%", desc: "极限烤机" },
] as const;

const PRESET_OPTIONS = PRESET_CARDS.map((p) => ({
  value: p.id,
  label: `${p.title} (${p.spec})`,
}));

const VOLT_OPTIONS = [
  { value: 0, label: "0V (停泵)" },
  { value: 7, label: "7V" },
  { value: 8, label: "8V" },
  { value: 11, label: "11V" },
  { value: 12, label: "12V" },
];

const VOLT_SHORT_OPTIONS = [0, 7, 8, 11, 12].map((v) => ({ value: v, label: `${v}V` }));

const LED_MODE_OPTIONS = [
  { value: "off", label: "关闭灯光" },
  { value: "static", label: "静态单色" },
  { value: "breathing", label: "机身呼吸" },
  { value: "colorful", label: "机身幻彩" },
  { value: "breathing_colorful", label: "机身呼吸幻彩" },
  { value: "mode4", label: "全区静态" },
];

const FAN_POLICY_OPTIONS = [
  { value: "sync", label: "双区同步 (风扇 = 机身)" },
  { value: "body_only", label: "仅机身灯亮 (风扇关)" },
  { value: "freeze", label: "锁定风扇当前灯态" },
  { value: "split", label: "双区独立双色" },
];

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
    color: [0, 200, 255],
    fan_policy: "sync",
    fan_color: [0, 200, 255],
  },
  flow_protection: true,
  sleep_on_disconnect: true,
};

const cfg = ref<WaterCoolingConfig>(structuredClone(DEFAULT_WC));
const snap = shallowRef<WaterCoolingSnapshot | null>(null);
const dirty = ref(false);
const busy = ref(false);
const msg = ref("正在读取水冷守护状态…");
const isError = ref(false);

function markDirty() {
  dirty.value = true;
}

async function pollStatus(forceConfigSync: boolean): Promise<boolean> {
  try {
    const res = await api.waterCoolingRequest<WaterCoolingSnapshot>({ op: "status" });
    snap.value = res;
    if (res.config && (forceConfigSync || !dirty.value)) {
      cfg.value = structuredClone(res.config);
    }
    isError.value = false;
    return true;
  } catch (err) {
    msg.value = `读取水冷后台服务失败：${String(err)}`;
    isError.value = true;
    return false;
  }
}

async function sendOp(label: string, req: Record<string, unknown>, clearDirty = false) {
  busy.value = true;
  msg.value = label;
  isError.value = false;
  try {
    const res = await api.waterCoolingRequest<Record<string, unknown>>(req);
    if (req.op === "scan") {
      const devs = (res.devices as Array<{ name?: string; address?: string; rssi?: number }>) || [];
      if (devs.length === 0) {
        msg.value = "未发现周边 MECHREVO 蓝牙水冷设备";
        message.info("未扫描到周边蓝牙水冷设备");
      } else {
        if (!cfg.value.mac && devs[0].address) {
          cfg.value.mac = devs[0].address;
          dirty.value = true;
        }
        msg.value = `发现设备：${devs
          .map((d) => `${d.name ?? "LCT"} (${d.address ?? "--"}, RSSI=${d.rssi ?? "--"})`)
          .join("；")}`;
        message.success(`扫描到 ${devs.length} 台蓝牙水冷设备`);
      }
    } else if ("saved_offline" in res) {
      if (clearDirty) dirty.value = false;
      msg.value = "水冷守护未运行，配置已保存至本地文件";
      message.warning("守护离线，配置已保存至本地");
    } else {
      const nextSnap = res as unknown as WaterCoolingSnapshot;
      snap.value = nextSnap;
      if (nextSnap.config && clearDirty) {
        cfg.value = structuredClone(nextSnap.config);
        dirty.value = false;
      }
      msg.value = clearDirty ? "配置已保存并应用至水冷后台服务" : "操作已完成，状态已同步";
      message.success(clearDirty ? "水冷配置已保存并应用" : "操作已完成");
    }
  } catch (err) {
    msg.value = `操作失败：${String(err)}`;
    isError.value = true;
    message.error(String(err));
  } finally {
    busy.value = false;
  }
}

// Visualizer computed properties
const bodyEff = computed<Rgb | null>(() => {
  const m = cfg.value.led.mode;
  if (m === "off") return null;
  if (m === "colorful" || m === "breathing_colorful") return [0, 200, 255];
  return cfg.value.led.color;
});

const fanEff = computed<Rgb | null>(() => {
  const led = cfg.value.led;
  if (led.mode === "off" || led.fan_policy === "body_only") return null;
  if (led.fan_policy === "sync") return bodyEff.value;
  if (led.fan_policy === "split") return led.fan_color;
  return [120, 148, 188];
});

const fanDutyLive = computed(() =>
  snap.value?.connected ? snap.value.applied?.fan_duty ?? 0 : 0
);
const fanSpinDur = computed(() =>
  fanDutyLive.value > 0
    ? `${Math.max(0.55, 3.6 - (fanDutyLive.value / 100) * 2.9).toFixed(2)}s`
    : "0s"
);
const pumpFlowing = computed(
  () => Boolean(snap.value?.connected && !snap.value?.flow_tripped && (snap.value?.applied?.pump_volt ?? 0) > 0)
);
const statusDotColor = computed(() =>
  snap.value?.flow_tripped ? "#F43F5E" : snap.value?.connected ? "#22C55E" : "#00C8FF"
);

const statusTagType = computed(() =>
  snap.value?.sleeping
    ? "warning"
    : snap.value?.flow_tripped
      ? "error"
      : snap.value?.connected
        ? "success"
        : "default"
);
const statusTagText = computed(() =>
  snap.value?.sleeping
    ? "系统睡眠暂停"
    : snap.value?.flow_tripped
      ? "断流锁定"
      : snap.value?.connected
        ? "蓝牙已连接"
        : "未连接"
);

const flowShort = computed(() => {
  const m = snap.value?.meter_state;
  if (m === 2) return "正常循环";
  if (typeof m === "number") return `无水流 (${m})`;
  return "待机";
});
const applied = computed(
  () => snap.value?.applied ?? { pump_volt: 0, pump_duty: 0, fan_duty: 0 }
);
const tempText = computed(() =>
  typeof snap.value?.temp_c === "number" ? `${snap.value.temp_c.toFixed(1)}°C` : "--°C"
);
const ecText = computed(() => {
  const ec = snap.value?.ec;
  if (ec?.ec_available) {
    return `EC 遥测 (${ec.ec_stale ? "过期" : "实时"})：${ec.lc_supported ? "已支持" : "未确认"} · 泵 ${
      ec.ec_pump_enabled ? `${ec.ec_pump_volt}V` : "关"
    } · 扇 ${ec.ec_fan_enabled ? `${ec.ec_fan_duty}%` : "关"} · WMI C1 seq=${ec.wmi_c1_seq ?? 0}`;
  }
  return `EC 遥测：等待守护数据 · WMI ${ec?.wmi_available ? "就绪" : "无信号"}`;
});
const faultText = computed(() => snap.value?.flow_trip_reason || snap.value?.last_error || "");

// Smart curve SVG helpers
const sortedPoints = computed(() =>
  [...cfg.value.curve.points].sort((a, b) => a.temp_c - b.temp_c)
);
const mapCurveX = (t: number) => 36 + ((Math.max(25, Math.min(100, t)) - 25) / 75) * (440 - 52);
const mapCurveY = (p: number) => 116 - (Math.max(0, Math.min(100, p)) / 100) * 102;
const pumpPoly = computed(() =>
  sortedPoints.value.map((p) => `${mapCurveX(p.temp_c)},${mapCurveY(p.pump_duty)}`).join(" ")
);
const fanPoly = computed(() =>
  sortedPoints.value.map((p) => `${mapCurveX(p.temp_c)},${mapCurveY(p.fan_duty)}`).join(" ")
);

let pollTimer: number | null = null;
onMounted(() => {
  api
    .getWaterCoolingLocalConfig()
    .then((local) => {
      if (!dirty.value) cfg.value = structuredClone(local);
    })
    .catch(() => {});
  pollStatus(true).then((ok) => {
    if (ok) msg.value = "已连接冰河水冷 Rust 守护进程";
  });
  pollTimer = window.setInterval(() => {
    if (!busy.value) pollStatus(false);
  }, 3000);
});
onUnmounted(() => {
  if (pollTimer !== null) window.clearInterval(pollTimer);
});
</script>

<template>
  <div class="page-stack">
    <!-- Hero Telemetry & LCT22002 Visualizer -->
    <section class="ccu-hero" style="--hero-accent: #22d3ee">
      <div class="ccu-field-row" style="margin-bottom: 14px">
        <div>
          <h2 class="ccu-section-title">
            {{ snap?.device_name || "冰河分体式外置水冷 (LCT22002)" }}
          </h2>
          <p class="ccu-section-sub mono">
            {{ snap?.device_mac || "--" }} · 固件 {{ snap?.fw_version || "--" }} ·
            {{ snap?.reason || "idle" }}
          </p>
        </div>
        <n-tag round :bordered="false" :type="statusTagType">{{ statusTagText }}</n-tag>
      </div>

      <div class="cooler-hero-flex">
        <div class="cooler-metrics-col">
          <div class="kpi-grid-4">
            <div class="kpi-tile">
              <span class="kpi-label">水流回路</span>
              <span class="kpi-value">{{ flowShort }}</span>
            </div>
            <div class="kpi-tile">
              <span class="kpi-label">水泵输出</span>
              <span class="kpi-value mono">
                {{ applied.pump_volt }}V · {{ applied.pump_duty }}%
              </span>
            </div>
            <div class="kpi-tile">
              <span class="kpi-label">冷排风扇</span>
              <span class="kpi-value mono">{{ applied.fan_duty }}%</span>
            </div>
            <div class="kpi-tile">
              <span class="kpi-label">CPU 封装</span>
              <span class="kpi-value mono">{{ tempText }}</span>
            </div>
          </div>

          <div class="ccu-well mono" style="font-size: 11.5px; color: var(--ink-secondary)">
            {{ ecText }}
          </div>

          <n-alert v-if="snap?.flow_tripped || faultText" type="error" :show-icon="true">
            安全告警：{{ faultText }}
          </n-alert>

          <div style="display: flex; flex-wrap: wrap; gap: 8px">
            <n-button
              size="small"
              :type="snap?.connected ? 'default' : 'primary'"
              :secondary="Boolean(snap?.connected)"
              :disabled="busy"
              @click="sendOp('正在连接蓝牙水冷…', { op: 'connect', mac: cfg.mac.trim() })"
            >
              {{ snap?.connected ? "重新连接" : "连接水冷" }}
            </n-button>
            <n-button
              size="small"
              secondary
              :disabled="busy"
              @click="sendOp('正在扫描周边蓝牙水冷 (4s)…', { op: 'scan', timeout_s: 4.0 })"
            >
              扫描设备
            </n-button>
            <n-button
              size="small"
              secondary
              :disabled="busy"
              @click="sendOp('正在安全停泵并断开…', { op: 'disconnect' })"
            >
              安全断开
            </n-button>
            <n-button
              size="small"
              secondary
              :disabled="busy"
              @click="sendOp('正在刷新水冷状态…', { op: 'status' })"
            >
              刷新
            </n-button>
            <n-button
              size="small"
              secondary
              type="warning"
              :disabled="busy"
              @click="sendOp('正在重置水流保护锁定…', { op: 'reset_fault' })"
            >
              重置故障
            </n-button>
            <n-button
              size="small"
              secondary
              type="error"
              :disabled="busy"
              @click="sendOp('正在执行紧急停泵…', { op: 'emergency_stop' })"
            >
              紧急停泵
            </n-button>
          </div>
        </div>

        <!-- Sculpted LCT22002 Cooler Rig Visualizer -->
        <div class="cooler-rig-box">
          <svg width="240" height="174" viewBox="0 0 240 174" fill="none">
            <!-- Coolant quick-disconnect tubing -->
            <path
              d="M 24 148 C 24 166, 62 166, 62 148"
              stroke="#1E293B"
              stroke-width="6"
              stroke-linecap="round"
            />
            <path
              v-if="pumpFlowing"
              d="M 24 148 C 24 166, 62 166, 62 148"
              stroke="#00C8FF"
              stroke-width="2.4"
              stroke-linecap="round"
              class="coolant-stream"
            />
            <!-- Main radiator chassis -->
            <rect
              x="12"
              y="8"
              width="216"
              height="142"
              rx="15"
              fill="#101726"
              stroke="#27354E"
              stroke-width="1.4"
            />
            <circle cx="27" cy="23" r="3.6" :fill="statusDotColor" />
            <rect
              v-for="i in [0, 1, 2, 3, 4, 5]"
              :key="i"
              x="22"
              :y="38 + i * 16"
              width="28"
              height="6"
              rx="3"
              fill="#1A2438"
            />
            <!-- ARGB Fan Rotor -->
            <g transform="translate(132, 79)">
              <circle
                v-if="fanEff"
                cx="0"
                cy="0"
                r="50"
                :stroke="rgbCss(fanEff, 0.24)"
                stroke-width="9"
              />
              <circle
                cx="0"
                cy="0"
                r="44"
                fill="#090D16"
                :stroke="fanEff ? rgbCss(fanEff) : '#2A364D'"
                :stroke-width="fanEff ? 3.5 : 2"
              />
              <g
                :class="fanDutyLive > 0 ? 'fan-rotor' : undefined"
                :style="{ '--fan-dur': fanSpinDur }"
              >
                <path
                  v-for="k in [0, 1, 2, 3, 4, 5, 6]"
                  :key="k"
                  :d="
                    (() => {
                      const a = (k * 2 * Math.PI) / 7;
                      return `M ${Math.cos(a) * 9} ${Math.sin(a) * 9} Q ${Math.cos(a + 0.22) * 26} ${
                        Math.sin(a + 0.22) * 26
                      } ${Math.cos(a + 0.42) * 38} ${Math.sin(a + 0.42) * 38}`;
                    })()
                  "
                  stroke="#384761"
                  stroke-width="3.2"
                  stroke-linecap="round"
                />
              </g>
              <circle cx="0" cy="0" r="9.5" fill="#1E293E" stroke="#3B4D6B" stroke-width="1.3" />
            </g>
            <!-- Chassis side RGB lightbar -->
            <rect
              v-if="bodyEff"
              x="200"
              y="22"
              width="14"
              height="114"
              rx="7"
              :fill="rgbCss(bodyEff, 0.26)"
            />
            <rect
              x="203"
              y="26"
              width="8"
              height="106"
              rx="4"
              :fill="bodyEff ? rgbCss(bodyEff) : '#253147'"
            />
          </svg>
          <span style="font-size: 11px; color: #94a3b8">
            LCT22002 双区灯效 · 风轮 · 水流实时预览
          </span>
        </div>
      </div>
    </section>

    <!-- Lower 2-Column Workspace (100% Naive UI controls — zero native GTK select!) -->
    <div class="grid-2">
      <!-- Left: Thermal Strategy -->
      <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:14px;">
        <div>
          <h3 class="ccu-section-title">水泵与冷排散热策略</h3>
          <p class="ccu-section-sub">支持预设档位、手动电压占空比、智能温控曲线或主机 EC 跟随</p>
        </div>

        <div class="ccu-field-row">
          <span class="ccu-field-label">控制策略</span>
          <n-select
            v-model:value="cfg.strategy"
            style="width: 230px"
            :options="STRATEGY_OPTIONS"
            @update:value="markDirty"
          />
        </div>

        <div v-if="cfg.strategy === 'preset'" class="ccu-well" style="display: flex; flex-direction: column; gap: 12px">
          <div class="ccu-field-row">
            <span class="ccu-field-label">固定预设档位</span>
            <n-select
              v-model:value="cfg.preset"
              style="width: 278px"
              :options="PRESET_OPTIONS"
              @update:value="markDirty"
            />
          </div>
          <div class="wc-preset-grid">
            <button
              v-for="item in PRESET_CARDS"
              :key="item.id"
              type="button"
              class="wc-preset-tile"
              :class="{ selected: cfg.preset === item.id }"
              @click="
                () => {
                  cfg.preset = item.id;
                  markDirty();
                }
              "
            >
              <div class="wc-preset-head">
                <span>{{ item.title }}</span>
                <span class="wc-preset-desc">{{ item.desc }}</span>
              </div>
              <div class="wc-preset-spec mono">{{ item.spec }}</div>
            </button>
          </div>
        </div>

        <div
          v-if="cfg.strategy === 'manual'"
          class="ccu-well"
          style="display: flex; flex-direction: column; gap: 14px"
        >
          <div class="ccu-field-row">
            <span class="ccu-field-label">水泵供电电压</span>
            <n-select
              v-model:value="cfg.manual.pump_volt"
              style="width: 150px"
              :options="VOLT_OPTIONS"
              @update:value="markDirty"
            />
          </div>
          <div>
            <div class="ccu-field-row" style="margin-bottom: 5px">
              <span class="ccu-field-label">水泵占空比</span>
              <span class="mono" style="color: var(--accent-cyan); font-weight: 700">
                {{ cfg.manual.pump_duty }}%
              </span>
            </div>
            <n-slider
              v-model:value="cfg.manual.pump_duty"
              :min="0"
              :max="100"
              @update:value="markDirty"
            />
          </div>
          <div>
            <div class="ccu-field-row" style="margin-bottom: 5px">
              <span class="ccu-field-label">冷排风扇占空比</span>
              <span class="mono" style="color: var(--accent-cyan); font-weight: 700">
                {{ cfg.manual.fan_duty }}%
              </span>
            </div>
            <n-slider
              v-model:value="cfg.manual.fan_duty"
              :min="0"
              :max="100"
              @update:value="markDirty"
            />
          </div>
        </div>

        <div
          v-if="cfg.strategy === 'smart_curve'"
          class="ccu-well"
          style="display: flex; flex-direction: column; gap: 12px"
        >
          <svg viewBox="0 0 440 138" class="wc-curve-svg">
            <g v-for="pct in [0, 50, 100]" :key="pct">
              <line
                x1="36"
                :y1="mapCurveY(pct)"
                x2="424"
                :y2="mapCurveY(pct)"
                stroke="var(--chart-grid)"
                stroke-width="1"
              />
              <text x="4" :y="mapCurveY(pct) + 3" fill="var(--chart-axis)" font-size="9.5" class="mono">
                {{ pct }}%
              </text>
            </g>
            <text
              v-for="t in [30, 50, 70, 90]"
              :key="t"
              :x="mapCurveX(t) - 10"
              y="132"
              fill="var(--chart-axis)"
              font-size="9.5"
              class="mono"
            >
              {{ t }}°C
            </text>
            <line
              v-if="typeof snap?.temp_c === 'number'"
              :x1="mapCurveX(snap.temp_c)"
              y1="14"
              :x2="mapCurveX(snap.temp_c)"
              y2="116"
              stroke="#F59E0B"
              stroke-width="1.3"
              stroke-dasharray="3 2"
            />
            <polyline :points="pumpPoly" fill="none" stroke="var(--accent-violet)" stroke-width="2.3" />
            <polyline :points="fanPoly" fill="none" stroke="var(--accent-cyan)" stroke-width="2.3" />
            <g v-for="(p, i) in sortedPoints" :key="i">
              <circle :cx="mapCurveX(p.temp_c)" :cy="mapCurveY(p.pump_duty)" r="3.4" fill="var(--accent-violet)" />
              <circle :cx="mapCurveX(p.temp_c)" :cy="mapCurveY(p.fan_duty)" r="3.4" fill="var(--accent-cyan)" />
            </g>
          </svg>

          <div style="display: flex; gap: 14px; font-size: 11px; color: var(--ink-muted)">
            <span><span class="status-dot" style="background: #8b5cf6; margin-right: 5px" />水泵占空比</span>
            <span><span class="status-dot" style="background: #00c8ff; margin-right: 5px" />冷排风扇</span>
            <span><span class="status-dot" style="background: #fbbf24; margin-right: 5px" />实时 CPU 温度</span>
          </div>

          <div class="ccu-field-row">
            <span class="ccu-field-label">降档滞回温度 (°C)</span>
            <n-input-number
              size="small"
              style="width: 112px"
              :min="1"
              :max="15"
              :value="cfg.curve.hysteresis_c"
              @update:value="
                (v) => {
                  cfg.curve.hysteresis_c = v ?? 3;
                  markDirty();
                }
              "
            />
          </div>

          <div class="wc-point-head mono">
            <span>段</span>
            <span>温度 °C</span>
            <span>电压</span>
            <span>水泵 %</span>
            <span>风扇 %</span>
          </div>
          <div v-for="(pt, idx) in cfg.curve.points" :key="idx" class="wc-point-row">
            <span class="mono" style="color: var(--ink-muted)">T{{ idx + 1 }}</span>
            <n-input-number
              size="tiny"
              :show-button="false"
              :min="25"
              :max="100"
              :value="pt.temp_c"
              @update:value="
                (v) => {
                  pt.temp_c = v ?? 25;
                  markDirty();
                }
              "
            />
            <n-select
              size="tiny"
              :options="VOLT_SHORT_OPTIONS"
              :value="pt.pump_volt"
              @update:value="
                (v: number) => {
                  pt.pump_volt = v;
                  markDirty();
                }
              "
            />
            <n-input-number
              size="tiny"
              :show-button="false"
              :min="0"
              :max="100"
              :value="pt.pump_duty"
              @update:value="
                (v) => {
                  pt.pump_duty = v ?? 0;
                  markDirty();
                }
              "
            />
            <n-input-number
              size="tiny"
              :show-button="false"
              :min="0"
              :max="100"
              :value="pt.fan_duty"
              @update:value="
                (v) => {
                  pt.fan_duty = v ?? 0;
                  markDirty();
                }
              "
            />
          </div>
        </div>

        <div v-if="cfg.strategy === 'ec_follow'" class="ccu-well ccu-field-hint">
          跟随主机 EC 只读遥测 (0x7E6/0x7E7 & WMI 0xC1)；若遥测过期或不可用，自动回退至静音档 (7V / 60% / 40%)。
        </div>
      </n-card>

      <!-- Right: Dual-Zone LED & Protection -->
      <div style="display: flex; flex-direction: column; gap: 16px">
        <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:14px;">
          <div>
            <h3 class="ccu-section-title">双区灯光（冷排风扇 + 机身灯带）</h3>
            <p class="ccu-section-sub">蓝牙指示灯由固件独占；风扇区通过协议锁存与同步实现分控</p>
          </div>

          <div class="ccu-field-row">
            <span class="ccu-field-label">灯光效果模式</span>
            <n-select
              v-model:value="cfg.led.mode"
              style="width: 220px"
              :options="LED_MODE_OPTIONS"
              @update:value="markDirty"
            />
          </div>

          <div class="ccu-field-row">
            <span class="ccu-field-label">风扇区联动策略</span>
            <n-select
              v-model:value="cfg.led.fan_policy"
              style="width: 220px"
              :options="FAN_POLICY_OPTIONS"
              @update:value="markDirty"
            />
          </div>

          <div class="ccu-field-row">
            <span class="ccu-field-label">机身 / 主灯颜色</span>
            <n-color-picker
              style="width: 150px"
              :show-alpha="false"
              :modes="['hex']"
              :value="rgbToHex(cfg.led.color)"
              @update:value="
                (hex) => {
                  cfg.led.color = hexToRgb(hex);
                  markDirty();
                }
              "
            />
          </div>

          <div v-if="cfg.led.fan_policy === 'split'" class="ccu-field-row">
            <span class="ccu-field-label">风扇独立颜色 (split)</span>
            <n-color-picker
              style="width: 150px"
              :show-alpha="false"
              :modes="['hex']"
              :value="rgbToHex(cfg.led.fan_color)"
              @update:value="
                (hex) => {
                  cfg.led.fan_color = hexToRgb(hex);
                  markDirty();
                }
              "
            />
          </div>
        </n-card>

        <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:12px;">
          <h3 class="ccu-section-title">安全保护与蓝牙设备绑定</h3>

          <div class="ccu-field-row">
            <span class="ccu-field-label">绑定蓝牙 MAC（留空自动发现）</span>
            <n-input
              v-model:value="cfg.mac"
              size="small"
              style="width: 200px"
              placeholder="例如 52:0C:E7:02:86:AB"
              @update:value="markDirty"
            />
          </div>

          <div class="ccu-field-row">
            <span class="ccu-field-label">开机自动重连水冷设备</span>
            <n-switch v-model:value="cfg.auto_connect" @update:value="markDirty" />
          </div>

          <div class="ccu-field-row">
            <span class="ccu-field-label">断流安全保护（无水流时立即停泵防干烧）</span>
            <n-switch v-model:value="cfg.flow_protection" @update:value="markDirty" />
          </div>

          <div class="ccu-field-row">
            <span class="ccu-field-label">断开连接 / 系统休眠前自动停泵</span>
            <n-switch v-model:value="cfg.sleep_on_disconnect" @update:value="markDirty" />
          </div>

          <div style="display: flex; align-items: center; gap: 10px; margin-top: 4px">
            <n-button
              type="primary"
              :loading="busy"
              @click="
                sendOp(
                  '正在保存并应用水冷配置…',
                  { op: 'set_config', config: cfg },
                  true
                )
              "
            >
              保存并应用全部水冷设置
            </n-button>
            <n-tag v-if="dirty" size="small" type="warning" :bordered="false">有未保存修改</n-tag>
          </div>
        </n-card>
      </div>
    </div>

    <n-alert :type="isError ? 'error' : 'info'" :show-icon="true">
      {{ msg }}
    </n-alert>
  </div>
</template>

<style scoped>
.cooler-hero-flex {
  display: flex;
  gap: 18px;
  align-items: stretch;
  flex-wrap: wrap;
}

.cooler-metrics-col {
  flex: 1;
  min-width: 340px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.cooler-rig-box {
  width: 264px;
  flex-shrink: 0;
  background: linear-gradient(180deg, #0c111d 0%, #080b12 100%);
  border: 1px solid rgba(15, 23, 42, 0.24);
  border-radius: 12px;
  padding: 12px 10px 10px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 6px;
}

.wc-curve-svg {
  width: 100%;
  height: 142px;
  background: var(--bg-chart);
  border-radius: 9px;
  border: 1px solid var(--border-subtle);
}

.wc-point-head,
.wc-point-row {
  display: grid;
  grid-template-columns: 30px 1fr 84px 1fr 1fr;
  gap: 6px;
  align-items: center;
}

.wc-point-head {
  font-size: 11px;
  color: var(--ink-muted);
}

.wc-preset-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 8px;
}

.wc-preset-tile {
  text-align: left;
  padding: 10px 12px;
  border-radius: 9px;
  border: 1px solid var(--border-subtle);
  background: var(--bg-tile);
  color: var(--ink-secondary);
  cursor: pointer;
  display: flex;
  flex-direction: column;
  gap: 4px;
  transition: border-color 140ms ease, background 140ms ease, box-shadow 140ms ease;
}

.wc-preset-tile:hover {
  border-color: color-mix(in srgb, var(--accent-cyan) 48%, transparent);
}

.wc-preset-tile.selected {
  border-color: var(--accent-cyan);
  background: var(--bg-tile-selected);
  box-shadow: 0 0 12px color-mix(in srgb, var(--accent-cyan) 14%, transparent);
}

.wc-preset-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 13px;
  font-weight: 650;
  color: var(--ink-strong);
}

.wc-preset-desc {
  font-size: 11px;
  font-weight: 400;
  color: var(--ink-muted);
}

.wc-preset-spec {
  font-size: 11px;
  color: var(--ink-spec-accent);
}
</style>

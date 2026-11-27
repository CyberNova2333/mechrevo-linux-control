<!-- SPDX-License-Identifier: AGPL-3.0-only -->
<!-- This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md). -->

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, shallowRef } from "vue";
import { NButton, NCard, NProgress, NTag, useMessage } from "naive-ui";
import {
  BatteryCharging,
  Cpu,
  Droplets,
  Flame,
  Gauge,
  HardDrive,
  Leaf,
  MemoryStick,
  PowerOff,
  Sliders,
  Sparkles,
  Zap,
} from "lucide-vue-next";
import { api } from "../api/tauri";
import type {
  LightingTabId,
  LiveTelemetry,
  PageId,
  PerformanceStatus,
  SystemInfo,
  WaterCoolingSnapshot,
} from "../types/hardware";

const props = defineProps<{
  telemetry: LiveTelemetry | null;
  sysInfo: SystemInfo | null;
  perfStatus: PerformanceStatus | null;
}>();

const emit = defineEmits<{
  "update:perfStatus": [status: PerformanceStatus];
  navigate: [page: PageId, subTab?: LightingTabId];
  allLightsOff: [];
}>();

const message = useMessage();
const modeBusy = ref(false);
const wcSnap = shallowRef<WaterCoolingSnapshot | null>(null);

const MODE_CARDS = [
  {
    id: "office",
    title: "办公模式",
    desc: "安静舒适，优先低噪与长续航",
    led: "绿色指示灯",
    color: "#22C55E",
    icon: Leaf,
  },
  {
    id: "balanced",
    title: "均衡模式",
    desc: "日常通用，兼顾性能与温控噪音",
    led: "蓝色指示灯",
    color: "#00C8FF",
    icon: Gauge,
  },
  {
    id: "turbo",
    title: "狂暴模式",
    desc: "性能全开，支持静音/高能双子档",
    led: "紫色指示灯",
    color: "#A855F7",
    icon: Flame,
  },
  {
    id: "custom",
    title: "自定义模式",
    desc: "手动定制功耗墙与 11 段风扇曲线",
    led: "白色指示灯",
    color: "#E2E8F0",
    icon: Sliders,
  },
] as const;

const activeModeColor = computed(() => {
  const m = props.perfStatus?.mode;
  return MODE_CARDS.find((c) => c.id === m)?.color ?? "#00C8FF";
});

const turboInfo = computed(() => props.perfStatus?.turbo ?? {});
const turboActive = computed(() => turboInfo.value.active === true);
const turboAvail = computed(() => turboInfo.value.available === true);
const turboUncertain = computed(() => turboInfo.value.policy_uncertain === true);
const turboRecovering = computed(
  () => turboInfo.value.recovery_needed === true && !turboActive.value
);
const turboKnown = computed(
  () =>
    typeof turboInfo.value.active === "boolean" &&
    typeof turboInfo.value.recovery_needed === "boolean" &&
    typeof turboInfo.value.policy_uncertain === "boolean" &&
    typeof turboInfo.value.standard_turbo === "boolean" &&
    typeof turboInfo.value.round_adapter === "boolean" &&
    typeof turboInfo.value.combined_supported === "boolean" &&
    turboInfo.value.read_rc === 0
);
const turboEligible = computed(
  () =>
    turboAvail.value &&
    turboKnown.value &&
    turboInfo.value.combined_supported === true &&
    props.perfStatus?.mode === "turbo" &&
    turboInfo.value.standard_turbo === true &&
    turboInfo.value.round_adapter === true &&
    !props.perfStatus.tuning_active &&
    !props.perfStatus.tuning_recovery_pending &&
    !turboRecovering.value &&
    !turboUncertain.value
);
const activeTurboVariant = computed(() => {
  if (
    turboActive.value &&
    (turboInfo.value.variant === "quiet" || turboInfo.value.variant === "extreme")
  ) {
    return turboInfo.value.variant;
  }
  if (turboKnown.value && !turboActive.value && !turboRecovering.value && !turboUncertain.value) {
    return "off";
  }
  return null;
});

// Live SVG Arc Gauge helpers (220 deg sweep)
const ARC_R = 44;
const ARC_CIRC = 2 * Math.PI * ARC_R;
const ARC_SWEEP = ARC_CIRC * (225 / 360);

function arcDashOffset(pct: number): number {
  const clamped = Math.max(0, Math.min(100, pct));
  return ARC_SWEEP * (1 - clamped / 100);
}

const cpuTempVal = computed(() => props.telemetry?.cpu.temp_c ?? 0);
const cpuUsageVal = computed(() => props.telemetry?.cpu.usage_pct ?? 0);
const cpuFreqGhz = computed(() => {
  const mhz = props.telemetry?.cpu.freq_mhz;
  return mhz != null ? `${(mhz / 1000).toFixed(2)} GHz` : "-- GHz";
});

const gpuTempVal = computed(() => props.telemetry?.gpu.temp_c ?? 0);
const gpuUsageVal = computed(() => props.telemetry?.gpu.usage_pct ?? 0);
const gpuVramText = computed(() => {
  const u = props.telemetry?.gpu.vram_used_mb;
  const t = props.telemetry?.gpu.vram_total_mb;
  if (u == null || t == null) return "-- / -- GB";
  return `${(u / 1024).toFixed(1)} / ${(t / 1024).toFixed(1)} GB`;
});

// 5-axis system radar polygon (official ControlCenterX HomePage RadarDatas visual)
const radarMetrics = computed(() => {
  const cpuLoad = Math.max(8, Math.min(100, props.telemetry?.cpu.usage_pct ?? 12));
  const cpuHeat = Math.max(8, Math.min(100, ((props.telemetry?.cpu.temp_c ?? 40) / 100) * 100));
  const gpuLoad = Math.max(8, Math.min(100, props.telemetry?.gpu.usage_pct ?? 8));
  const memLoad = Math.max(8, Math.min(100, props.telemetry?.memory.usage_pct ?? 30));
  const diskLoad = Math.max(8, Math.min(100, props.telemetry?.storage.usage_pct ?? 25));
  return [
    { label: "CPU 负载", value: cpuLoad },
    { label: "CPU 温控", value: cpuHeat },
    { label: "GPU 负载", value: gpuLoad },
    { label: "内存占用", value: memLoad },
    { label: "存储占用", value: diskLoad },
  ];
});

function radarPt(idx: number, radiusRatio: number): [number, number] {
  const cx = 114;
  const cy = 98;
  const maxR = 58;
  const angle = -Math.PI / 2 + (idx * 2 * Math.PI) / 5;
  return [cx + Math.cos(angle) * maxR * radiusRatio, cy + Math.sin(angle) * maxR * radiusRatio];
}

function radarRingPoints(ratio: number): string {
  return [0, 1, 2, 3, 4]
    .map((i) => {
      const [x, y] = radarPt(i, ratio);
      return `${x.toFixed(1)},${y.toFixed(1)}`;
    })
    .join(" ");
}

const radarDataPoints = computed(() =>
  radarMetrics.value
    .map((m, i) => {
      const [x, y] = radarPt(i, m.value / 100);
      return `${x.toFixed(1)},${y.toFixed(1)}`;
    })
    .join(" ")
);

async function switchMode(modeId: string) {
  if (modeBusy.value) return;
  modeBusy.value = true;
  try {
    const next = await api.setPerformanceMode(modeId);
    emit("update:perfStatus", next);
    const title = MODE_CARDS.find((c) => c.id === modeId)?.title ?? modeId;
    message.success(`已切换至 ${title}`);
  } catch (err) {
    message.error(`切换档位失败：${String(err)}`);
  } finally {
    modeBusy.value = false;
  }
}

async function switchTurbo(variant: "quiet" | "extreme" | "off") {
  if (modeBusy.value) return;
  modeBusy.value = true;
  try {
    const next = await api.setTurboVariant(variant);
    emit("update:perfStatus", next);
    const label =
      variant === "quiet" ? "静音狂暴" : variant === "extreme" ? "高能狂暴" : "固件默认控制";
    message.success(`狂暴子模式已切换：${label}`);
  } catch (err) {
    message.error(`狂暴子模式切换失败：${String(err)}`);
  } finally {
    modeBusy.value = false;
  }
}

let wcTimer: number | null = null;
async function pollCooler() {
  try {
    wcSnap.value = await api.waterCoolingRequest<WaterCoolingSnapshot>({ op: "status" });
  } catch {
    // daemon offline is non-fatal on home
  }
}

onMounted(() => {
  pollCooler();
  wcTimer = window.setInterval(pollCooler, 4000);
});

onUnmounted(() => {
  if (wcTimer !== null) window.clearInterval(wcTimer);
});

const powerLabel = computed(() => {
  const p = props.telemetry?.power;
  if (!p) return "正在检测供电…";
  const pct = p.battery_pct != null ? ` · 电池 ${p.battery_pct}%` : "";
  return p.ac_online ? `已连接电源适配器${pct}` : `电池供电${pct}`;
});

const coolerFlowText = computed(() => {
  if (!wcSnap.value?.connected) return "未连接";
  if (wcSnap.value.flow_tripped) return "断流保护锁定";
  if (wcSnap.value.meter_state === 2) return "水流循环正常";
  return "已连接待机";
});
</script>

<template>
  <div class="page-stack">
    <!-- Hero Hardware Identity Strip -->
    <section class="ccu-hero" :style="{ '--hero-accent': activeModeColor }">
      <div class="hero-top-row">
        <div class="hero-machine">
          <div class="hero-chip-badge">
            <Zap :size="18" :color="activeModeColor" />
          </div>
          <div>
            <div class="hero-model-row">
              <h1 class="hero-model-name">
                MECHREVO {{ sysInfo?.board_name || "YAOSHI Series" }}
              </h1>
              <n-tag size="small" :bordered="false" round type="info" class="mono">
                BIOS {{ sysInfo?.bios_version || "--" }}
              </n-tag>
              <n-tag
                v-if="perfStatus?.fan_safety_protect"
                size="small"
                type="error"
                round
              >
                EC 高温安全保护激活
              </n-tag>
            </div>
            <div class="hero-spec-line mono">
              <span>{{ telemetry?.cpu.model || "Intel Core Processor" }}</span>
              <span class="spec-sep">·</span>
              <span>{{ telemetry?.gpu.name || "NVIDIA GeForce GPU" }}</span>
            </div>
          </div>
        </div>

        <div class="hero-power-pill">
          <BatteryCharging :size="15" :color="telemetry?.power.ac_online ? '#22C55E' : '#F59E0B'" />
          <span>{{ powerLabel }}</span>
        </div>
      </div>
    </section>

    <!-- Main 2-Column Command Stage: Left Mode Deck + Right Official Telemetry Radar & Gauges -->
    <div class="grid-home-main">
      <!-- Left: Official 4-Mode Selector + Turbo Submode Picker -->
      <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:14px;">
        <div class="ccu-field-row">
          <div>
            <h2 class="ccu-section-title">操作模式</h2>
            <p class="ccu-section-sub">与机身物理模式键 (0xB0) 实时双向同步</p>
          </div>
          <n-button size="small" secondary type="primary" @click="emit('navigate', 'performance')">
            高级功耗与风扇曲线 →
          </n-button>
        </div>

        <div class="mode-deck-2">
          <button
            v-for="card in MODE_CARDS"
            :key="card.id"
            type="button"
            class="mode-card"
            :class="{ active: perfStatus?.mode === card.id }"
            :style="{ '--mode-color': card.color }"
            :disabled="
              modeBusy ||
              !perfStatus?.switchable ||
              perfStatus?.mode === card.id ||
              (card.id === 'custom' && perfStatus?.fan_safety_protect)
            "
            @click="switchMode(card.id)"
          >
            <div class="mode-card-top">
              <span class="mode-card-title">{{ card.title }}</span>
              <component :is="card.icon" :size="17" :color="card.color" />
            </div>
            <div class="mode-card-desc">{{ card.desc }}</div>
            <div class="mode-card-led">
              <span
                class="status-dot"
                :style="{
                  background: card.color,
                  boxShadow: perfStatus?.mode === card.id ? `0 0 9px ${card.color}` : 'none',
                }"
              />
              <span>{{ card.led }}</span>
            </div>
          </button>
        </div>

        <!-- Inline Turbo Submode Picker (mirrors ControlCenterX TurboSubModePicker) -->
        <div class="ccu-well turbo-strip">
          <div class="ccu-field-row" style="flex-wrap: wrap; gap: 10px">
            <div>
              <div class="ccu-field-label">狂暴子模式联合调度</div>
              <div class="ccu-field-hint">
                {{
                  turboActive
                    ? `运行中：${activeTurboVariant === "quiet" ? "静音狂暴" : "高能狂暴"}`
                    : perfStatus?.mode === "turbo"
                      ? "当前为固件原生狂暴，可选择静音狂暴或高能狂暴"
                      : "切换至“狂暴模式”并连接原装圆口电源后可用"
                }}
              </div>
            </div>
            <div style="display: flex; gap: 6px">
              <n-button
                size="small"
                :type="activeTurboVariant === 'quiet' ? 'primary' : 'default'"
                :secondary="activeTurboVariant !== 'quiet'"
                :disabled="modeBusy || (!turboEligible && activeTurboVariant !== 'quiet')"
                @click="activeTurboVariant !== 'quiet' && switchTurbo('quiet')"
              >
                静音狂暴
              </n-button>
              <n-button
                size="small"
                :type="activeTurboVariant === 'extreme' ? 'primary' : 'default'"
                :secondary="activeTurboVariant !== 'extreme'"
                :disabled="modeBusy || (!turboEligible && activeTurboVariant !== 'extreme')"
                @click="activeTurboVariant !== 'extreme' && switchTurbo('extreme')"
              >
                高能狂暴
              </n-button>
              <n-button
                size="small"
                secondary
                :disabled="modeBusy || !(turboAvail && (turboActive || turboRecovering) && !turboUncertain)"
                @click="switchTurbo('off')"
              >
                固件控制
              </n-button>
            </div>
          </div>
        </div>
      </n-card>

      <!-- Right: Official Live System Radar & Dual CPU/GPU Arc Gauges -->
      <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:14px;">
        <div class="ccu-field-row">
          <div>
            <h2 class="ccu-section-title">硬件状态监控</h2>
            <p class="ccu-section-sub">实时采样传感器与负载雷达（每 2 秒刷新）</p>
          </div>
          <n-tag size="small" :bordered="false" type="success" round>实时在线</n-tag>
        </div>

        <div class="monitor-stage">
          <!-- 5-Axis Radar -->
          <div class="radar-box">
            <svg viewBox="0 0 228 198" class="radar-svg">
              <polygon
                v-for="r in [0.33, 0.66, 1]"
                :key="r"
                :points="radarRingPoints(r)"
                fill="none"
                stroke="rgba(148,163,184,0.16)"
                stroke-width="1"
              />
              <line
                v-for="i in [0, 1, 2, 3, 4]"
                :key="i"
                x1="114"
                y1="98"
                :x2="radarPt(i, 1)[0]"
                :y2="radarPt(i, 1)[1]"
                stroke="rgba(148,163,184,0.14)"
                stroke-width="1"
              />
              <polygon
                :points="radarDataPoints"
                fill="rgba(0, 200, 255, 0.22)"
                stroke="#00C8FF"
                stroke-width="2"
              />
              <circle
                v-for="(m, i) in radarMetrics"
                :key="m.label"
                :cx="radarPt(i, m.value / 100)[0]"
                :cy="radarPt(i, m.value / 100)[1]"
                r="3.2"
                fill="#5EE6FF"
              />
              <text
                v-for="(m, i) in radarMetrics"
                :key="`lbl-${m.label}`"
                :x="radarPt(i, 1.32)[0]"
                :y="radarPt(i, 1.28)[1] + 4"
                text-anchor="middle"
                fill="#94A3B8"
                font-size="10.5"
              >
                {{ m.label }}
              </text>
            </svg>
          </div>

          <!-- Dual Arc Gauges (CPU + GPU) -->
          <div class="gauges-col">
            <!-- CPU Gauge -->
            <div class="arc-card">
              <svg viewBox="0 0 108 108" class="arc-svg">
                <g transform="rotate(157.5 54 54)">
                  <circle
                    cx="54"
                    cy="54"
                    :r="ARC_R"
                    fill="none"
                    stroke="rgba(148,163,184,0.15)"
                    stroke-width="7.5"
                    stroke-linecap="round"
                    :stroke-dasharray="`${ARC_SWEEP} ${ARC_CIRC}`"
                  />
                  <circle
                    cx="54"
                    cy="54"
                    :r="ARC_R"
                    fill="none"
                    :stroke="cpuTempVal >= 85 ? '#F43F5E' : '#00C8FF'"
                    stroke-width="7.5"
                    stroke-linecap="round"
                    :stroke-dasharray="`${ARC_SWEEP} ${ARC_CIRC}`"
                    :stroke-dashoffset="arcDashOffset(cpuTempVal)"
                    style="transition: stroke-dashoffset 450ms ease"
                  />
                </g>
                <text x="54" y="51" text-anchor="middle" fill="#F8FAFC" font-size="19" font-weight="700" class="mono">
                  {{ telemetry?.cpu.temp_c != null ? `${Math.round(cpuTempVal)}°` : "--" }}
                </text>
                <text x="54" y="68" text-anchor="middle" fill="#7E8FA8" font-size="10">
                  CPU 封装
                </text>
              </svg>
              <div class="arc-meta">
                <div class="arc-title">处理器</div>
                <div class="arc-kv mono">负载 {{ cpuUsageVal.toFixed(0) }}%</div>
                <div class="arc-kv mono">{{ cpuFreqGhz }}</div>
              </div>
            </div>

            <!-- GPU Gauge -->
            <div class="arc-card">
              <svg viewBox="0 0 108 108" class="arc-svg">
                <g transform="rotate(157.5 54 54)">
                  <circle
                    cx="54"
                    cy="54"
                    :r="ARC_R"
                    fill="none"
                    stroke="rgba(148,163,184,0.15)"
                    stroke-width="7.5"
                    stroke-linecap="round"
                    :stroke-dasharray="`${ARC_SWEEP} ${ARC_CIRC}`"
                  />
                  <circle
                    cx="54"
                    cy="54"
                    :r="ARC_R"
                    fill="none"
                    :stroke="gpuTempVal >= 82 ? '#F43F5E' : '#8B5CF6'"
                    stroke-width="7.5"
                    stroke-linecap="round"
                    :stroke-dasharray="`${ARC_SWEEP} ${ARC_CIRC}`"
                    :stroke-dashoffset="arcDashOffset(gpuTempVal)"
                    style="transition: stroke-dashoffset 450ms ease"
                  />
                </g>
                <text x="54" y="51" text-anchor="middle" fill="#F8FAFC" font-size="19" font-weight="700" class="mono">
                  {{ telemetry?.gpu.temp_c != null ? `${Math.round(gpuTempVal)}°` : "--" }}
                </text>
                <text x="54" y="68" text-anchor="middle" fill="#7E8FA8" font-size="10">
                  GPU 核心
                </text>
              </svg>
              <div class="arc-meta">
                <div class="arc-title">独立显卡</div>
                <div class="arc-kv mono">
                  负载 {{ telemetry?.gpu.usage_pct != null ? `${Math.round(gpuUsageVal)}%` : "--" }}
                </div>
                <div class="arc-kv mono">
                  {{ telemetry?.gpu.power_w != null ? `${telemetry.gpu.power_w.toFixed(1)} W` : "休眠" }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </n-card>
    </div>

    <!-- Bottom 4 Telemetry & Quick Access Cards -->
    <div class="kpi-grid-4">
      <n-card size="small" :bordered="true" class="detail-card">
        <div class="detail-head">
          <Cpu :size="16" color="#00C8FF" />
          <span>中央处理器</span>
          <span class="mono detail-accent">{{ cpuUsageVal.toFixed(0) }}%</span>
        </div>
        <n-progress
          type="line"
          :percentage="Math.round(cpuUsageVal)"
          :show-indicator="false"
          :height="5"
          color="#00C8FF"
          rail-color="rgba(148,163,184,0.16)"
        />
        <div class="detail-rows mono">
          <div>
            <span>实时主频</span>
            <strong>{{ cpuFreqGhz }}</strong>
          </div>
          <div>
            <span>最大睿频</span>
            <strong>
              {{
                telemetry?.cpu.max_freq_mhz
                  ? `${(telemetry.cpu.max_freq_mhz / 1000).toFixed(1)} GHz`
                  : "--"
              }}
            </strong>
          </div>
          <div>
            <span>逻辑线程</span>
            <strong>{{ telemetry?.cpu.cores ?? "--" }} 线程</strong>
          </div>
        </div>
      </n-card>

      <n-card size="small" :bordered="true" class="detail-card">
        <div class="detail-head">
          <Flame :size="16" color="#A855F7" />
          <span>独立显卡</span>
          <span class="mono detail-accent" style="color: #c084fc">
            {{ telemetry?.gpu.power_w != null ? `${telemetry.gpu.power_w.toFixed(1)}W` : "--" }}
          </span>
        </div>
        <n-progress
          type="line"
          :percentage="Math.round(gpuUsageVal)"
          :show-indicator="false"
          :height="5"
          color="#A855F7"
          rail-color="rgba(148,163,184,0.16)"
        />
        <div class="detail-rows mono">
          <div>
            <span>核心频率</span>
            <strong>
              {{ telemetry?.gpu.clock_mhz != null ? `${telemetry.gpu.clock_mhz} MHz` : "--" }}
            </strong>
          </div>
          <div>
            <span>显存占用</span>
            <strong>{{ gpuVramText }}</strong>
          </div>
          <div>
            <span>当前状态</span>
            <strong>{{ telemetry?.gpu.available ? "已唤醒" : "休眠中" }}</strong>
          </div>
        </div>
      </n-card>

      <n-card size="small" :bordered="true" class="detail-card">
        <div class="detail-head">
          <MemoryStick :size="16" color="#38BDF8" />
          <span>内存与存储</span>
          <span class="mono detail-accent">
            {{ telemetry ? `${Math.round(telemetry.memory.usage_pct)}%` : "--" }}
          </span>
        </div>
        <n-progress
          type="line"
          :percentage="Math.round(telemetry?.memory.usage_pct ?? 0)"
          :show-indicator="false"
          :height="5"
          color="#38BDF8"
          rail-color="rgba(148,163,184,0.16)"
        />
        <div class="detail-rows mono">
          <div>
            <span>内存已用</span>
            <strong>
              {{
                telemetry
                  ? `${telemetry.memory.used_gb.toFixed(1)} / ${telemetry.memory.total_gb.toFixed(1)} GB`
                  : "--"
              }}
            </strong>
          </div>
          <div>
            <span>系统磁盘</span>
            <strong>
              {{
                telemetry
                  ? `${telemetry.storage.used_gb.toFixed(0)} / ${telemetry.storage.total_gb.toFixed(0)} GB`
                  : "--"
              }}
            </strong>
          </div>
          <div>
            <span style="display: inline-flex; align-items: center; gap: 4px">
              <HardDrive :size="11" /> NVMe 温度
            </span>
            <strong>
              {{
                telemetry?.storage.nvme_temp_c != null
                  ? `${telemetry.storage.nvme_temp_c.toFixed(0)}°C`
                  : "--"
              }}
            </strong>
          </div>
        </div>
      </n-card>

      <n-card size="small" :bordered="true" class="detail-card">
        <div class="detail-head">
          <Droplets :size="16" color="#22D3EE" />
          <span>冰河水冷与快捷</span>
          <n-tag
            size="tiny"
            :bordered="false"
            round
            :type="wcSnap?.flow_tripped ? 'error' : wcSnap?.connected ? 'success' : 'default'"
          >
            {{ coolerFlowText }}
          </n-tag>
        </div>
        <div class="detail-rows mono" style="margin-top: 2px">
          <div>
            <span>水泵输出</span>
            <strong>
              {{
                wcSnap?.connected && wcSnap.applied
                  ? `${wcSnap.applied.pump_volt}V · ${wcSnap.applied.pump_duty}%`
                  : "待机"
              }}
            </strong>
          </div>
          <div>
            <span>冷排风扇</span>
            <strong>
              {{
                wcSnap?.connected && wcSnap.applied ? `${wcSnap.applied.fan_duty}%` : "待机"
              }}
            </strong>
          </div>
        </div>
        <div class="quick-links">
          <n-button size="tiny" secondary type="primary" @click="emit('navigate', 'watercooling')">
            <template #icon><Droplets :size="12" /></template>
            水冷
          </n-button>
          <n-button size="tiny" secondary @click="emit('navigate', 'lighting', 'keyboard')">
            <template #icon><Sparkles :size="12" /></template>
            灯光
          </n-button>
          <n-button size="tiny" secondary type="error" @click="emit('allLightsOff')">
            <template #icon><PowerOff :size="12" /></template>
            熄灯
          </n-button>
        </div>
      </n-card>
    </div>
  </div>
</template>

<style scoped>
.hero-top-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  flex-wrap: wrap;
}

.hero-machine {
  display: flex;
  align-items: center;
  gap: 14px;
}

.hero-chip-badge {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  background: rgba(14, 21, 36, 0.9);
  border: 1px solid rgba(148, 163, 184, 0.2);
  display: grid;
  place-items: center;
  flex-shrink: 0;
}

.hero-model-row {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
}

.hero-model-name {
  margin: 0;
  font-size: 19px;
  font-weight: 800;
  letter-spacing: 0.03em;
  color: #f8fafc;
}

.hero-spec-line {
  margin-top: 4px;
  font-size: 12px;
  color: var(--ink-muted);
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.spec-sep {
  opacity: 0.45;
}

.hero-power-pill {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 7px 14px;
  border-radius: 999px;
  background: rgba(10, 14, 24, 0.85);
  border: 1px solid rgba(148, 163, 184, 0.18);
  font-size: 12.5px;
  color: #dce4f2;
}

.turbo-strip {
  margin-top: 2px;
}

.monitor-stage {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  align-items: center;
}

.radar-box {
  background: rgba(9, 13, 22, 0.72);
  border: 1px solid rgba(148, 163, 184, 0.12);
  border-radius: 11px;
  padding: 6px;
  display: grid;
  place-items: center;
}

.radar-svg {
  width: 100%;
  max-width: 210px;
  height: auto;
}

.gauges-col {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.arc-card {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px 12px;
  border-radius: 11px;
  background: rgba(9, 13, 22, 0.72);
  border: 1px solid rgba(148, 163, 184, 0.12);
}

.arc-svg {
  width: 82px;
  height: 82px;
  flex-shrink: 0;
}

.arc-meta {
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.arc-title {
  font-size: 13px;
  font-weight: 700;
  color: #f1f5fb;
}

.arc-kv {
  font-size: 11.5px;
  color: var(--ink-muted);
}

.detail-card :deep(.n-card__content) {
  display: flex;
  flex-direction: column;
  gap: 9px;
}

.detail-head {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  font-weight: 650;
  color: #e2e8f0;
}

.detail-accent {
  margin-left: auto;
  font-size: 13px;
  font-weight: 700;
  color: #00c8ff;
}

.detail-rows {
  display: flex;
  flex-direction: column;
  gap: 5px;
  font-size: 11.5px;
  color: var(--ink-muted);
}

.detail-rows > div {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 8px;
}

.detail-rows strong {
  color: #dce4f2;
  font-weight: 600;
}

.quick-links {
  display: flex;
  gap: 6px;
  margin-top: 4px;
}
</style>

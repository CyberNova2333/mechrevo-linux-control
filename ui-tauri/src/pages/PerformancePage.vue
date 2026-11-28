<!-- SPDX-License-Identifier: AGPL-3.0-only -->
<!-- This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md). -->

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, shallowRef } from "vue";
import {
  NAlert,
  NButton,
  NCard,
  NInputNumber,
  NPopconfirm,
  NSlider,
  NSwitch,
  NTabPane,
  NTabs,
  NTag,
  useMessage,
} from "naive-ui";
import { Flame, Gauge, Leaf, RotateCcw, Sliders } from "lucide-vue-next";
import { api } from "../api/tauri";
import {
  CPU_FAN_TEMPS,
  GPU_FAN_TEMPS,
  MIN_FAN_DUTY,
  type GraphicsProfile,
  type PerformanceStatus,
  type TuningConfig,
} from "../types/hardware";

const emit = defineEmits<{
  statusUpdated: [status: PerformanceStatus];
}>();

const message = useMessage();

const MODE_CARDS = [
  { id: "office", title: "办公模式", desc: "静音低耗", led: "绿色指示灯", color: "var(--led-office)", icon: Leaf },
  { id: "balanced", title: "均衡模式", desc: "性能与温控均衡", led: "蓝色指示灯", color: "var(--led-balanced)", icon: Gauge },
  { id: "turbo", title: "狂暴模式", desc: "释放整机高功耗", led: "紫色指示灯", color: "var(--led-turbo)", icon: Flame },
  { id: "custom", title: "自定义模式", desc: "功耗与风扇曲线", led: "白色指示灯", color: "var(--led-custom)", icon: Sliders },
] as const;

const perf = shallowRef<PerformanceStatus | null>(null);
const busy = ref(false);
const initialized = ref(false);
const statusMsg = ref("正在读取固件档位与调校状态…");
const isError = ref(false);

const cpuPower = ref(35);
const gpuEnabled = ref(false);
const gpuPower = ref(115);
const gpuDbEnabled = ref(false);
const gpuDb = ref(5);
const cpuFan = ref<number[]>([...MIN_FAN_DUTY]);
const gpuFan = ref<number[]>([...MIN_FAN_DUTY]);
const curveTab = ref<"cpu" | "gpu">("cpu");

function hydrateTuning(t: TuningConfig) {
  cpuPower.value = t.pl1;
  cpuFan.value = [...t.cpu_fan];
  gpuFan.value = [...t.gpu_fan];
  if (typeof t.gpu_ctgp === "number") {
    gpuEnabled.value = true;
    gpuPower.value = t.gpu_ctgp;
    if (typeof t.gpu_db === "number" && t.gpu_db >= 5) {
      gpuDbEnabled.value = true;
      gpuDb.value = t.gpu_db;
    } else {
      gpuDbEnabled.value = false;
    }
  } else {
    gpuEnabled.value = false;
    gpuDbEnabled.value = false;
  }
}

let alive = true;
let refreshSeq = 0;

async function refresh(syncPrefs: boolean) {
  const seq = ++refreshSeq;
  try {
    const res = await api.getPerformanceStatus();
    if (!alive || seq !== refreshSeq || busy.value) return;
    perf.value = res.status;
    emit("statusUpdated", res.status);
    if (syncPrefs) {
      const src = res.status.tuning ?? res.prefs;
      if (src) hydrateTuning(src);
      initialized.value = true;
    }
    statusMsg.value = res.status.reason || "已连接固件档位控制";
    isError.value = false;
  } catch (err) {
    if (!alive || seq !== refreshSeq) return;
    statusMsg.value = `读取固件状态失败：${String(err)}`;
    isError.value = true;
  }
}

async function runOp(label: string, fn: () => Promise<PerformanceStatus>) {
  refreshSeq += 1;
  busy.value = true;
  statusMsg.value = label;
  isError.value = false;
  try {
    const next = await fn();
    if (!alive) return;
    perf.value = next;
    emit("statusUpdated", next);
    statusMsg.value = next.reason || "设置已生效，硬件回读一致";
    message.success("固件状态已更新");
  } catch (err) {
    if (!alive) return;
    statusMsg.value = String(err);
    isError.value = true;
    message.error(String(err));
  } finally {
    if (alive) busy.value = false;
  }
}

async function handleApplyTuning() {
  if (
    cpuFan.value.some((v, i) => i > 0 && v < cpuFan.value[i - 1]) ||
    gpuFan.value.some((v, i) => i > 0 && v < gpuFan.value[i - 1])
  ) {
    statusMsg.value = "请确保风扇占空比随温度单调不减后再应用。";
    isError.value = true;
    message.warning("风扇曲线须随温度单调不减");
    return;
  }
  const cfg: TuningConfig = {
    pl1: cpuPower.value,
    pl2: cpuPower.value,
    cpu_fan: cpuFan.value,
    gpu_fan: gpuFan.value,
  };
  if (gpuEnabled.value) {
    cfg.gpu_ctgp = gpuPower.value;
    cfg.gpu_db = gpuDbEnabled.value ? gpuDb.value : 0;
  }
  await runOp("正在应用自定义功耗与双风扇曲线…", () => api.applyPerformanceTuning(cfg));
}

function updateFanNode(which: "cpu" | "gpu", idx: number, val: number | null) {
  const arr = which === "cpu" ? cpuFan.value.slice() : gpuFan.value.slice();
  const clamped = Math.max(MIN_FAN_DUTY[idx], Math.min(100, Math.round(val ?? MIN_FAN_DUTY[idx])));
  arr[idx] = clamped;
  // Auto-keep lower nodes <= clamped (never below factory floor) and upper nodes >= clamped
  for (let j = idx - 1; j >= 0; j--) {
    if (arr[j] > clamped) arr[j] = Math.max(MIN_FAN_DUTY[j], clamped);
  }
  for (let j = 1; j < arr.length; j++) {
    if (arr[j] < arr[j - 1]) arr[j] = arr[j - 1];
  }
  arr[10] = 100;
  if (which === "cpu") cpuFan.value = arr;
  else gpuFan.value = arr;
}

function resetFanFloor() {
  if (curveTab.value === "cpu") cpuFan.value = [...MIN_FAN_DUTY];
  else gpuFan.value = [...MIN_FAN_DUTY];
}

const safety = computed(() => perf.value?.fan_safety_protect === true);
const gpuSupported = computed(
  () =>
    perf.value?.gpu_tuning_supported === true &&
    perf.value.gpu_tuning_range?.[0] === 65 &&
    perf.value.gpu_tuning_range?.[1] === 115 &&
    perf.value.gpu_db_range?.[0] === 5 &&
    perf.value.gpu_db_range?.[1] === 25
);

const turbo = computed(() => perf.value?.turbo ?? {});
const turboActive = computed(() => turbo.value.active === true);
const turboAvail = computed(() => turbo.value.available === true);
const turboRecovering = computed(() => turbo.value.recovery_needed === true && !turboActive.value);
const turboUncertain = computed(() => turbo.value.policy_uncertain === true);
const turboKnown = computed(
  () =>
    typeof turbo.value.active === "boolean" &&
    typeof turbo.value.recovery_needed === "boolean" &&
    typeof turbo.value.policy_uncertain === "boolean" &&
    typeof turbo.value.standard_turbo === "boolean" &&
    typeof turbo.value.round_adapter === "boolean" &&
    typeof turbo.value.combined_supported === "boolean" &&
    turbo.value.read_rc === 0
);
const turboEligible = computed(
  () =>
    turboAvail.value &&
    turboKnown.value &&
    turbo.value.combined_supported === true &&
    perf.value?.mode === "turbo" &&
    turbo.value.standard_turbo === true &&
    turbo.value.round_adapter === true &&
    !perf.value.tuning_active &&
    !perf.value.tuning_recovery_pending &&
    !turboRecovering.value &&
    !turboUncertain.value
);
const activeTurboVariant = computed(() => {
  if (turboActive.value && (turbo.value.variant === "quiet" || turbo.value.variant === "extreme")) {
    return turbo.value.variant;
  }
  if (turboKnown.value && !turboActive.value && !turboRecovering.value && !turboUncertain.value) {
    return "off";
  }
  return null;
});

const turboCaption = computed(() => {
  if (turboUncertain.value) return "固件状态无法确认，已停止软件切档；请正常重启电脑";
  if (!turboAvail.value && (turboActive.value || turboRecovering.value))
    return "控制服务离线，正在等待内核归还控制";
  if (!turboAvail.value) return "调度服务未就绪，当前保持固件控制";
  if (turboRecovering.value) return "恢复尚未完成，请点击“交还固件控制”重试";
  if (!turboKnown.value) return "调度状态尚未确认";
  if (!turbo.value.combined_supported) return "固件未报告支持联合功耗调度";
  if (turboActive.value) {
    return `当前：${activeTurboVariant.value === "quiet" ? "静音狂暴" : "高能狂暴"} · 自动分配 CPU / GPU 功耗`;
  }
  if (perf.value?.mode !== "turbo") return "请先切换到上方的狂暴模式";
  if (!turbo.value.round_adapter) return "请连接原装圆口电源";
  return "当前：由固件控制，尚未启用子模式调度";
});

// Fan Curve SVG math
const activeTemps = computed(() => (curveTab.value === "cpu" ? CPU_FAN_TEMPS : GPU_FAN_TEMPS));
const activeFanVals = computed(() => (curveTab.value === "cpu" ? cpuFan.value : gpuFan.value));
const activeStroke = computed(() =>
  curveTab.value === "cpu" ? "var(--accent-cyan)" : "var(--accent-violet)"
);

const SVG_W = 680;
const SVG_H = 178;
const PAD_L = 40;
const PAD_R = SVG_W - 18;
const PAD_T = 16;
const PAD_B = SVG_H - 26;

const mapX = (t: number) => PAD_L + ((t - 40) / 50) * (PAD_R - PAD_L);
const mapY = (v: number) => PAD_B - ((v - 30) / 70) * (PAD_B - PAD_T);

const floorPts = computed(() =>
  activeTemps.value.map((t, i) => `${mapX(t)},${mapY(MIN_FAN_DUTY[i])}`).join(" ")
);
const floorArea = computed(
  () =>
    `${PAD_L},${PAD_B} ${floorPts.value} ${mapX(activeTemps.value[activeTemps.value.length - 1])},${PAD_B}`
);
const curvePts = computed(() =>
  activeTemps.value.map((t, i) => `${mapX(t)},${mapY(activeFanVals.value[i])}`).join(" ")
);

const graphicsModes = [
  { id: "intel", label: "仅核显" },
  { id: "on-demand", label: "混合 · 按需独显" },
  { id: "nvidia", label: "独显优先" },
] as const;
const graphicsMode = ref<GraphicsProfile | null>(null);
const graphicsBusy = ref(false);
const graphicsHint = ref("正在读取 PRIME 配置…");

async function readGraphics() {
  try {
    const mode = await api.getGraphicsProfile();
    if (!alive) return;
    graphicsMode.value = mode;
    graphicsHint.value = "更改配置后需自行重启。";
  } catch (err) {
    if (!alive) return;
    graphicsMode.value = null;
    graphicsHint.value = String(err);
  }
}

async function switchGraphics(mode: GraphicsProfile) {
  if (!alive || graphicsBusy.value || !graphicsMode.value || graphicsMode.value === mode) return;
  graphicsBusy.value = true;
  graphicsHint.value = "正在等待管理员授权并更新 PRIME 配置…";
  try {
    const saved = await api.setGraphicsProfile(mode);
    if (!alive) return;
    graphicsMode.value = saved;
    graphicsHint.value = "配置已写入，请保存工作后自行重启。";
  } catch (err) {
    if (!alive) return;
    await readGraphics();
    if (alive) graphicsHint.value = String(err);
  } finally {
    if (alive) graphicsBusy.value = false;
  }
}

let pollTimer: number | null = null;
onMounted(() => {
  alive = true;
  refresh(true);
  readGraphics();
  pollTimer = window.setInterval(() => {
    if (!busy.value) refresh(!initialized.value);
  }, 3500);
});
onUnmounted(() => {
  alive = false;
  if (pollTimer !== null) window.clearInterval(pollTimer);
});
</script>

<template>
  <div class="page-stack">
    <!-- Firmware Mode Deck -->
    <section
      class="ccu-hero"
      :style="{
        '--hero-accent':
          MODE_CARDS.find((c) => c.id === perf?.mode)?.color ?? '#00C8FF',
      }"
    >
      <div class="ccu-field-row" style="margin-bottom: 14px">
        <div>
          <h2 class="ccu-section-title">固件运行档位</h2>
          <p class="ccu-section-sub">与机身物理模式键保持双向同步，每 3.5 秒自动校验 EC 寄存器</p>
        </div>
        <n-tag
          round
          :bordered="false"
          :type="safety ? 'error' : perf?.switchable ? 'success' : 'warning'"
        >
          {{
            safety
              ? "EC 高温安全保护激活"
              : perf?.tuning_active
                ? "自定义调校运行中"
                : perf?.mode
                  ? `EC ${perf.ec_mode ?? ""}`
                  : "读取中"
          }}
        </n-tag>
      </div>

      <div class="mode-deck-4">
        <button
          v-for="card in MODE_CARDS"
          :key="card.id"
          type="button"
          class="mode-card"
          :class="{ active: perf?.mode === card.id }"
          :style="{ '--mode-color': card.color }"
          :disabled="
            busy ||
            !perf?.switchable ||
            perf?.mode === card.id ||
            (card.id === 'custom' && safety)
          "
          @click="runOp(`正在切换至${card.title}…`, () => api.setPerformanceMode(card.id))"
        >
          <div class="mode-card-top">
            <span class="mode-card-title">{{ card.title }}</span>
            <component :is="card.icon" :size="16" :color="card.color" />
          </div>
          <div class="mode-card-desc">{{ card.desc }}</div>
          <div class="mode-card-led">
            <span
              class="status-dot"
              :style="{
                background: card.color,
                boxShadow: perf?.mode === card.id ? `0 0 9px ${card.color}` : 'none',
              }"
            />
            <span>{{ card.led }}</span>
          </div>
        </button>
      </div>

      <div class="grid-2" style="margin-top: 14px">
        <div class="ccu-well">
          <div class="kpi-label">EC 高温风扇安全保护 (0x7A5)</div>
          <div
            style="margin-top: 5px; font-size: 13px; font-weight: 600"
            :style="{ color: safety ? '#E11D48' : 'var(--ink-primary)' }"
          >
            {{
              perf
                ? safety
                  ? "保护激活：已交由固件优先降温，暂停自定义调校"
                  : "正常（硬件保护待命）"
                : "读取中…"
            }}
          </div>
        </div>
        <div class="ccu-well">
          <div class="kpi-label">SmartBalance (0x7AC)</div>
          <div style="margin-top: 5px; font-size: 13px; font-weight: 600; color: var(--ink-primary)">
            {{
              perf
                ? perf.smart_balance_supported
                  ? perf.smart_balance_enabled
                    ? "支持 · 已开启"
                    : "支持 · 未开启"
                  : "本机固件不支持 (bit0=0)"
                : "读取中…"
            }}
          </div>
        </div>
      </div>
    </section>

    <n-card>
      <div class="ccu-field-row" style="flex-wrap: wrap; gap: 12px">
        <div>
          <h3 class="ccu-section-title">显卡模式（NVIDIA PRIME）</h3>
          <p class="ccu-section-sub" role="status">{{ graphicsHint }}</p>
        </div>
        <div style="display: flex; flex-wrap: wrap; gap: 8px">
          <n-popconfirm v-for="mode in graphicsModes" :key="mode.id"
            positive-text="确认更改" negative-text="取消"
            @positive-click="switchGraphics(mode.id)">
            <template #trigger>
              <n-button size="small" :type="graphicsMode === mode.id ? 'primary' : 'default'"
                :disabled="!graphicsMode || graphicsBusy || graphicsMode === mode.id"
                :aria-pressed="graphicsMode === mode.id">{{ mode.label }}</n-button>
            </template>
            将系统配置改为「{{ mode.label }}」？重启后可能影响 GNOME 登录和外接显示器输出。
          </n-popconfirm>
        </div>
      </div>
    </n-card>

    <!-- Turbo Submode Scheduling -->
    <n-card :bordered="true">
      <div class="ccu-field-row" style="flex-wrap: wrap; gap: 12px">
        <div>
          <h3 class="ccu-section-title">狂暴子模式调度（Rust mechrevo-turbod）</h3>
          <p class="ccu-section-sub">{{ turboCaption }}</p>
        </div>
        <div style="display: flex; gap: 8px; flex-wrap: wrap">
          <n-button
            size="small"
            :type="activeTurboVariant === 'quiet' && turboAvail ? 'primary' : 'default'"
            :secondary="!(activeTurboVariant === 'quiet' && turboAvail)"
            :disabled="busy || (!turboEligible && activeTurboVariant !== 'quiet')"
            @click="activeTurboVariant !== 'quiet' && runOp('正在切换至静音狂暴…', () => api.setTurboVariant('quiet'))"
          >
            静音狂暴
          </n-button>
          <n-button
            size="small"
            :type="activeTurboVariant === 'extreme' && turboAvail ? 'primary' : 'default'"
            :secondary="!(activeTurboVariant === 'extreme' && turboAvail)"
            :disabled="busy || (!turboEligible && activeTurboVariant !== 'extreme')"
            @click="activeTurboVariant !== 'extreme' && runOp('正在切换至高能狂暴…', () => api.setTurboVariant('extreme'))"
          >
            高能狂暴
          </n-button>
          <n-button
            size="small"
            :type="activeTurboVariant === 'off' && turboAvail ? 'info' : 'default'"
            secondary
            :disabled="busy || !(turboAvail && (turboActive || turboRecovering) && !turboUncertain)"
            @click="runOp('正在交还固件控制…', () => api.setTurboVariant('off'))"
          >
            交还固件控制
          </n-button>
        </div>
      </div>
      <div class="ccu-field-hint" style="margin-top: 10px">
        仅在原装圆口电源与普通狂暴档下可用；切档、拔电或休眠前自动交还固件控制，唤醒后不自动重获。
      </div>
    </n-card>

    <!-- Custom Power Limits & 11-Node Dual Fan Curve -->
    <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:16px;">
      <div class="ccu-field-row" style="flex-wrap: wrap; gap: 10px">
        <div>
          <h3 class="ccu-section-title">自定义功耗墙与 11 段双风扇曲线</h3>
          <p class="ccu-section-sub">
            应用后切入白色自定义档；切换其他档位、按下物理模式键或睡眠前自动完整回退
          </p>
        </div>
        <n-tag v-if="perf?.tuning" type="info" :bordered="false" round class="mono">
          CPU {{ perf.tuning.pl1 }}W
          {{ typeof perf.tuning.gpu_ctgp === "number" ? ` · GPU ${perf.tuning.gpu_ctgp}W` : "" }}
          {{ perf.tuning.gpu_db ? ` + ${perf.tuning.gpu_db}W DB` : "" }}
        </n-tag>
      </div>

      <!-- Power Sliders -->
      <div class="ccu-well" style="display: flex; flex-direction: column; gap: 16px">
        <div class="power-row">
          <div>
            <div class="ccu-field-label">CPU 功耗上限（PL1 = PL2 同步）</div>
            <div class="ccu-field-hint">实机验证安全包络 25–75 W</div>
          </div>
          <div class="power-slider-wrap">
            <n-slider v-model:value="cpuPower" :min="25" :max="75" :step="1" style="flex: 1" />
            <span class="mono power-val">{{ cpuPower }} W</span>
          </div>
        </div>

        <div class="power-row">
          <div style="display: flex; align-items: center; gap: 10px">
            <n-switch
              v-model:value="gpuEnabled"
              :disabled="!gpuSupported && !gpuEnabled"
              size="small"
            />
            <div>
              <div class="ccu-field-label">GPU 基础功耗上限 (cTGP)</div>
              <div class="ccu-field-hint">
                {{
                  gpuSupported
                    ? "实机验证 65–115 W；关闭时由固件原生管理"
                    : perf?.gpu_tuning_reason || "等待确认 GPU 调校支持"
                }}
              </div>
            </div>
          </div>
          <div class="power-slider-wrap" :style="{ opacity: gpuEnabled && gpuSupported ? 1 : 0.42 }">
            <n-slider
              v-model:value="gpuPower"
              :min="65"
              :max="115"
              :step="1"
              :disabled="!gpuEnabled || !gpuSupported"
              style="flex: 1"
            />
            <span class="mono power-val">{{ gpuPower }} W</span>
          </div>
        </div>

        <div class="power-row" :style="{ opacity: gpuEnabled && gpuSupported ? 1 : 0.42 }">
          <div style="display: flex; align-items: center; gap: 10px">
            <n-switch
              v-model:value="gpuDbEnabled"
              :disabled="!gpuEnabled || !gpuSupported"
              size="small"
            />
            <div>
              <div class="ccu-field-label">NVIDIA Dynamic Boost 额外功耗</div>
              <div class="ccu-field-hint">支持 5–25 W 动态功耗迁移</div>
            </div>
          </div>
          <div class="power-slider-wrap">
            <n-slider
              v-model:value="gpuDb"
              :min="5"
              :max="25"
              :step="1"
              :disabled="!gpuEnabled || !gpuSupported || !gpuDbEnabled"
              style="flex: 1"
            />
            <span class="mono power-val">{{ gpuDbEnabled ? `${gpuDb} W` : "关闭" }}</span>
          </div>
        </div>
      </div>

      <!-- 11-Node Interactive Fan Curve -->
      <div class="ccu-well" style="display: flex; flex-direction: column; gap: 12px">
        <div class="ccu-field-row" style="flex-wrap: wrap; gap: 10px">
          <n-tabs
            v-model:value="curveTab"
            type="segment"
            size="small"
            style="width: 240px"
          >
            <n-tab-pane name="cpu" tab="CPU 风扇曲线" />
            <n-tab-pane name="gpu" tab="GPU 风扇曲线" />
          </n-tabs>
          <n-button size="tiny" secondary @click="resetFanFloor">
            <template #icon><RotateCcw :size="12" /></template>
            重置为出厂基线
          </n-button>
        </div>

        <svg :viewBox="`0 0 ${SVG_W} ${SVG_H}`" class="fan-svg">
          <g v-for="pct in [40, 70, 100]" :key="pct">
            <line
              :x1="PAD_L"
              :y1="mapY(pct)"
              :x2="PAD_R"
              :y2="mapY(pct)"
              stroke="var(--chart-grid)"
              stroke-width="1"
            />
            <text :x="6" :y="mapY(pct) + 4" fill="var(--chart-axis)" font-size="10.5" class="mono">
              {{ pct }}%
            </text>
          </g>
          <text
            v-for="t in [40, 50, 60, 70, 80, 90]"
            :key="t"
            :x="mapX(t) - 12"
            :y="SVG_H - 7"
            fill="var(--chart-axis)"
            font-size="10.5"
            class="mono"
          >
            {{ t }}°C
          </text>
          <polygon :points="floorArea" fill="var(--chart-floor-fill)" />
          <polyline
            :points="floorPts"
            fill="none"
            stroke="var(--chart-floor-stroke)"
            stroke-width="1.2"
            stroke-dasharray="4 3"
          />
          <polyline
            :points="curvePts"
            fill="none"
            :stroke="activeStroke"
            stroke-opacity="0.22"
            stroke-width="7"
          />
          <polyline
            :points="curvePts"
            fill="none"
            :stroke="activeStroke"
            stroke-width="2.5"
          />
          <circle
            v-for="(t, i) in activeTemps"
            :key="i"
            :cx="mapX(t)"
            :cy="mapY(activeFanVals[i])"
            r="4"
            :fill="activeStroke"
            stroke="var(--bg-chart)"
            stroke-width="1.5"
          />
        </svg>

        <div class="fan-node-grid">
          <div v-for="(temp, i) in activeTemps" :key="i" class="fan-node-cell">
            <span class="mono fan-node-temp">
              {{ i === 0 ? `<${activeTemps[1]}` : temp }}°C
            </span>
            <n-input-number
              size="tiny"
              :show-button="false"
              :min="MIN_FAN_DUTY[i]"
              :max="100"
              :step="5"
              :disabled="i === 10"
              :value="activeFanVals[i]"
              @update:value="(v) => updateFanNode(curveTab, i, v)"
            />
          </div>
        </div>
        <div class="ccu-field-hint">
          虚线阴影区为出厂散热安全基线（禁止下探）；最高温节点锁定 100%，调整低节点会自动保持单调不减。
        </div>
      </div>

      <div style="display: flex; gap: 10px; flex-wrap: wrap">
        <n-button
          type="primary"
          :loading="busy"
          :disabled="!initialized || !perf?.switchable || safety || (gpuEnabled && !gpuSupported)"
          @click="handleApplyTuning"
        >
          应用功耗与风扇曲线
        </n-button>
        <n-button
          secondary
          :disabled="busy || !(perf?.tuning_active || perf?.tuning_recovery_pending)"
          @click="runOp('正在撤销自定义调校并还原固件…', () => api.resetPerformanceTuning())"
        >
          撤销调校 / 返回原档位
        </n-button>
      </div>
    </n-card>

    <n-alert :type="isError ? 'error' : 'info'" :show-icon="true">
      {{ statusMsg }}
    </n-alert>
  </div>
</template>

<style scoped>
.power-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 18px;
  flex-wrap: wrap;
}

.power-slider-wrap {
  display: flex;
  align-items: center;
  gap: 14px;
  width: 310px;
  max-width: 100%;
}

.power-val {
  min-width: 58px;
  text-align: right;
  font-weight: 700;
  color: var(--accent-cyan);
  font-size: 13.5px;
}

.fan-svg {
  width: 100%;
  height: 178px;
  background: var(--bg-chart);
  border-radius: 10px;
  border: 1px solid var(--border-subtle);
}

.fan-node-grid {
  display: grid;
  grid-template-columns: repeat(11, minmax(0, 1fr));
  gap: 6px;
}

@media (max-width: 1020px) {
  .fan-node-grid {
    grid-template-columns: repeat(6, minmax(0, 1fr));
  }
}

.fan-node-cell {
  background: var(--bg-node-cell);
  border: 1px solid var(--border-subtle);
  border-radius: 8px;
  padding: 6px 5px;
  display: flex;
  flex-direction: column;
  gap: 4px;
  align-items: center;
}

.fan-node-temp {
  font-size: 10.5px;
  color: var(--ink-muted);
}
</style>

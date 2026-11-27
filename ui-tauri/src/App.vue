<!-- SPDX-License-Identifier: AGPL-3.0-only -->
<!-- This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md). -->

<script setup lang="ts">
import { computed, h, onMounted, onUnmounted, ref, shallowRef } from "vue";
import {
  darkTheme,
  dateZhCN,
  NButton,
  NConfigProvider,
  NLayout,
  NLayoutContent,
  NLayoutHeader,
  NLayoutSider,
  NMenu,
  NMessageProvider,
  NTag,
  zhCN,
  type MenuOption,
} from "naive-ui";
import {
  Cpu,
  Droplets,
  Gauge,
  Home,
  Layers,
  PowerOff,
  Sparkles,
} from "lucide-vue-next";
import { api } from "./api/tauri";
import { mechrevoThemeOverrides } from "./theme/naiveTheme";
import type {
  LightingTabId,
  LiveTelemetry,
  PageId,
  PerformanceStatus,
  SystemInfo,
} from "./types/hardware";
import HomePage from "./pages/HomePage.vue";
import PerformancePage from "./pages/PerformancePage.vue";
import LightingPage from "./pages/LightingPage.vue";
import WaterCoolingPage from "./pages/WaterCoolingPage.vue";
import ProfilesPage from "./pages/ProfilesPage.vue";

interface NavMeta {
  id: PageId;
  title: string;
  subtitle: string;
}

const NAV_META: Record<PageId, NavMeta> = {
  home: {
    id: "home",
    title: "控制台主页",
    subtitle: "整机实时硬件监测、性能模式切换与外设总览",
  },
  performance: {
    id: "performance",
    title: "性能与散热调校",
    subtitle: "固件档位、狂暴子模式、自定义功耗墙与 11 段双风扇曲线",
  },
  lighting: {
    id: "lighting",
    title: "RGB 灯光控制",
    subtitle: "101 键背光矩阵、海岸氛围灯带、后盖 LOGO 与睡眠策略",
  },
  watercooling: {
    id: "watercooling",
    title: "冰河分体水冷",
    subtitle: "外置蓝牙水冷泵扇调速、温控曲线、水流安全保护与双区灯效",
  },
  profiles: {
    id: "profiles",
    title: "情景预设与联动",
    subtitle: "性能与三区灯效一键情景打包及前台进程自动切换",
  },
};

const MODE_BADGE: Record<string, { label: string; color: string }> = {
  office: { label: "办公模式", color: "#22C55E" },
  balanced: { label: "均衡模式", color: "#00C8FF" },
  turbo: { label: "狂暴模式", color: "#A855F7" },
  custom: { label: "自定义模式", color: "#E2E8F0" },
};

const activePage = ref<PageId>("home");
const lightingTab = ref<LightingTabId>("keyboard");
const sysInfo = shallowRef<SystemInfo | null>(null);
const telemetry = shallowRef<LiveTelemetry | null>(null);
const perfHeader = shallowRef<PerformanceStatus | null>(null);
const allOffBusy = ref(false);
const noticeText = ref("");
const refreshNonce = ref(0);

const menuOptions: MenuOption[] = [
  {
    label: "主页",
    key: "home",
    icon: () => h(Home, { size: 17, strokeWidth: 2 }),
  },
  {
    label: "性能调校",
    key: "performance",
    icon: () => h(Gauge, { size: 17, strokeWidth: 2 }),
  },
  {
    label: "灯光控制",
    key: "lighting",
    icon: () => h(Sparkles, { size: 17, strokeWidth: 2 }),
  },
  {
    label: "冰河水冷",
    key: "watercooling",
    icon: () => h(Droplets, { size: 17, strokeWidth: 2 }),
  },
  {
    label: "情景预设",
    key: "profiles",
    icon: () => h(Layers, { size: 17, strokeWidth: 2 }),
  },
];

const currentMeta = computed(() => NAV_META[activePage.value]);

const daemonCount = computed(() =>
  sysInfo.value ? Object.values(sysInfo.value.daemons).filter(Boolean).length : 0
);

const activeModeBadge = computed(() => {
  const m = perfHeader.value?.mode;
  if (!m) return null;
  return MODE_BADGE[m] ?? { label: m, color: "#94A3B8" };
});

function syncFromHash() {
  const raw = window.location.hash.replace(/^#/, "").trim();
  if (!raw) return;
  const [pagePart, subPart] = raw.split(":");
  if (
    pagePart === "home" ||
    pagePart === "performance" ||
    pagePart === "lighting" ||
    pagePart === "watercooling" ||
    pagePart === "profiles"
  ) {
    activePage.value = pagePart;
    if (
      pagePart === "lighting" &&
      (subPart === "keyboard" || subPart === "lightbar" || subPart === "logo" || subPart === "policy")
    ) {
      lightingTab.value = subPart;
    }
  }
}

function navigateTo(page: PageId, subTab?: LightingTabId) {
  activePage.value = page;
  if (page === "lighting" && subTab) {
    lightingTab.value = subTab;
  }
}

let sysTimer: number | null = null;
let telemetryTimer: number | null = null;
let perfTimer: number | null = null;
let noticeTimer: number | null = null;

function showNotice(text: string) {
  noticeText.value = text;
  if (noticeTimer !== null) window.clearTimeout(noticeTimer);
  noticeTimer = window.setTimeout(() => {
    noticeText.value = "";
  }, 5000);
}

async function pollSystem() {
  try {
    sysInfo.value = await api.getSystemInfo();
  } catch {
    // ignore
  }
}

async function pollTelemetry() {
  try {
    telemetry.value = await api.getLiveTelemetry();
  } catch {
    // ignore
  }
}

async function pollPerfSummary() {
  try {
    const res = await api.getPerformanceStatus();
    perfHeader.value = res.status;
  } catch {
    // ignore
  }
}

async function handleAllLightsOff() {
  if (allOffBusy.value) return;
  allOffBusy.value = true;
  try {
    const res = await api.allLightsOff();
    refreshNonce.value += 1;
    if (res.errors.length > 0) {
      showNotice(`已关闭 ${res.turned_off.join(" / ") || "部分灯区"}；异常：${res.errors.join("；")}`);
    } else {
      showNotice("已关闭全部三区灯光，原有配色已保留");
    }
  } catch (err) {
    showNotice(`关闭灯光失败：${String(err)}`);
  } finally {
    allOffBusy.value = false;
  }
}

function onGlobalKeydown(e: KeyboardEvent) {
  if (!e.ctrlKey || e.altKey || e.metaKey) return;
  const map: Record<string, PageId> = {
    "1": "home",
    "2": "performance",
    "3": "lighting",
    "4": "watercooling",
    "5": "profiles",
  };
  const target = map[e.key];
  if (target) {
    e.preventDefault();
    navigateTo(target);
  }
}

onMounted(() => {
  syncFromHash();
  window.addEventListener("hashchange", syncFromHash);
  window.addEventListener("keydown", onGlobalKeydown);
  pollSystem();
  pollTelemetry();
  pollPerfSummary();
  sysTimer = window.setInterval(pollSystem, 8000);
  telemetryTimer = window.setInterval(pollTelemetry, 2000);
  perfTimer = window.setInterval(pollPerfSummary, 5000);
});

onUnmounted(() => {
  window.removeEventListener("hashchange", syncFromHash);
  window.removeEventListener("keydown", onGlobalKeydown);
  if (sysTimer !== null) window.clearInterval(sysTimer);
  if (telemetryTimer !== null) window.clearInterval(telemetryTimer);
  if (perfTimer !== null) window.clearInterval(perfTimer);
  if (noticeTimer !== null) window.clearTimeout(noticeTimer);
});
</script>

<template>
  <n-config-provider
    :theme="darkTheme"
    :theme-overrides="mechrevoThemeOverrides"
    :locale="zhCN"
    :date-locale="dateZhCN"
  >
    <n-message-provider>
      <n-layout has-sider class="shell-root">
        <n-layout-sider :width="224" bordered content-style="padding: 0; height: 100%;">
          <div class="sider-inner">
            <div class="brand-header">
              <div class="brand-badge" aria-hidden="true">
                <svg width="22" height="16" viewBox="0 0 24 18" fill="none">
                  <path
                    d="M2 15.5L7.4 2.8L12 11.4L16.6 2.8L22 15.5"
                    stroke="#00C8FF"
                    stroke-width="2.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </svg>
              </div>
              <div>
                <div class="brand-name">MECHREVO</div>
                <div class="brand-sub">机械革命控制中心</div>
              </div>
            </div>

            <n-menu
              :value="activePage"
              :options="menuOptions"
              :indent="16"
              @update:value="(v: string) => navigateTo(v as PageId)"
            />

            <div class="sider-spacer" />

            <div class="sider-hardware-card">
              <div class="sider-hw-model">{{ sysInfo?.board_name || "YAOSHI Series" }}</div>
              <div class="sider-hw-meta mono">
                <span>BIOS {{ sysInfo?.bios_version || "--" }}</span>
                <span>Linux {{ sysInfo?.kernel || "--" }}</span>
              </div>
              <div
                class="sider-daemon-row"
                title="Rust 后台守护进程健康状态（水冷 / 会话灯光 / 狂暴调度 / EC 恢复）"
              >
                <span
                  class="status-dot"
                  :class="daemonCount === 4 ? 'ok' : daemonCount > 0 ? 'warn' : 'muted'"
                />
                <span>
                  Rust 守护 <strong class="mono">{{ daemonCount }}/4</strong> 在线
                </span>
              </div>
            </div>
          </div>
        </n-layout-sider>

        <n-layout>
          <n-layout-header class="app-topbar">
            <div class="topbar-left">
              <span class="topbar-title">{{ currentMeta.title }}</span>
              <span class="topbar-sub">{{ currentMeta.subtitle }}</span>
            </div>

            <div class="topbar-right">
              <n-tag v-if="noticeText" size="small" type="info" round :bordered="false">
                {{ noticeText }}
              </n-tag>

              <div v-if="telemetry" class="mini-stat-chip mono" title="实时 CPU / GPU 温度">
                <Cpu :size="13" color="#00C8FF" />
                <span>
                  CPU {{ telemetry.cpu.temp_c != null ? `${Math.round(telemetry.cpu.temp_c)}°C` : "--" }}
                </span>
                <span style="opacity: 0.35">|</span>
                <span>
                  GPU {{ telemetry.gpu.temp_c != null ? `${Math.round(telemetry.gpu.temp_c)}°C` : "--" }}
                </span>
              </div>

              <div v-if="activeModeBadge" class="mini-stat-chip" title="当前固件性能档位">
                <span
                  class="status-dot"
                  :style="{
                    background: activeModeBadge.color,
                    boxShadow: `0 0 8px ${activeModeBadge.color}`,
                  }"
                />
                <span>{{ activeModeBadge.label }}</span>
              </div>

              <n-button
                size="small"
                type="error"
                secondary
                :loading="allOffBusy"
                title="关闭键盘、海岸灯带与后盖 LOGO，保留配色设置"
                @click="handleAllLightsOff"
              >
                <template #icon>
                  <PowerOff :size="14" />
                </template>
                一键熄灯
              </n-button>
            </div>
          </n-layout-header>

          <n-layout-content class="workspace-body">
            <HomePage
              v-if="activePage === 'home'"
              :telemetry="telemetry"
              :sys-info="sysInfo"
              :perf-status="perfHeader"
              @update:perf-status="(s) => (perfHeader = s)"
              @navigate="navigateTo"
              @all-lights-off="handleAllLightsOff"
            />
            <PerformancePage
              v-else-if="activePage === 'performance'"
              @status-updated="(s) => (perfHeader = s)"
            />
            <LightingPage
              v-else-if="activePage === 'lighting'"
              :key="`lighting-${refreshNonce}`"
              :initial-tab="lightingTab"
              @update:tab="(t) => (lightingTab = t)"
            />
            <WaterCoolingPage v-else-if="activePage === 'watercooling'" />
            <ProfilesPage
              v-else-if="activePage === 'profiles'"
              @hardware-changed="
                () => {
                  refreshNonce += 1;
                  pollPerfSummary();
                }
              "
            />
          </n-layout-content>
        </n-layout>
      </n-layout>
    </n-message-provider>
  </n-config-provider>
</template>

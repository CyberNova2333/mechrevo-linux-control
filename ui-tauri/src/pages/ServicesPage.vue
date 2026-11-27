<!-- SPDX-License-Identifier: AGPL-3.0-only -->
<!-- This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md). -->

<script setup lang="ts">
import { computed, nextTick, onMounted, onUnmounted, ref, shallowRef } from "vue";
import {
  NAlert,
  NButton,
  NCard,
  NDrawer,
  NDrawerContent,
  NPopconfirm,
  NSelect,
  NSwitch,
  NTag,
  useMessage,
} from "naive-ui";
import {
  Copy,
  FileText,
  Play,
  RefreshCw,
  RotateCw,
  ServerCog,
  ShieldCheck,
  Square,
} from "lucide-vue-next";
import { api } from "../api/tauri";
import type { ManagedServiceItem } from "../types/hardware";

const emit = defineEmits<{
  servicesChanged: [];
}>();

const message = useMessage();
const services = shallowRef<ManagedServiceItem[]>([]);
const loading = ref(false);
const busyUnit = ref<string | null>(null);
const statusText = ref("正在读取 systemd 后台服务状态…");
const isError = ref(false);

// Log drawer state
const logDrawerOpen = ref(false);
const logTarget = shallowRef<ManagedServiceItem | null>(null);
const logLines = ref<string[]>([]);
const logLineCount = ref<number>(80);
const logLoading = ref(false);
const logWarning = ref<string | null>(null);
const logConsoleRef = ref<HTMLElement | null>(null);

const LOG_LINE_OPTIONS = [
  { label: "最近 40 行", value: 40 },
  { label: "最近 80 行", value: 80 },
  { label: "最近 150 行", value: 150 },
];

const daemons = computed(() => services.value.filter((s) => s.kind === "daemon"));
const hooks = computed(() => services.value.filter((s) => s.kind === "hook"));

const runningDaemons = computed(() => daemons.value.filter((s) => s.running).length);
const enabledHooks = computed(() => hooks.value.filter((s) => s.enabled).length);
const totalEnabled = computed(() => services.value.filter((s) => s.enabled).length);
const totalMemoryText = computed(() => {
  const bytes = daemons.value.reduce((acc, s) => acc + (s.memory_bytes ?? 0), 0);
  return formatBytes(bytes);
});

function formatBytes(bytes: number | null): string {
  if (bytes == null || bytes <= 0) return "--";
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(0)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

function formatSince(ts: string): string {
  if (!ts) return "--";
  // Typical systemd format: "Fri 2026-11-27 17:16:07 CST" -> "11-27 17:16:07"
  const m = ts.match(/\d{4}-(\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})/);
  return m ? m[1] : ts;
}

function stateBadge(item: ManagedServiceItem): {
  type: "success" | "error" | "warning" | "default" | "info";
  label: string;
} {
  if (!item.installed) return { type: "warning", label: "未安装" };
  if (item.kind === "hook") {
    if (item.active_state === "failed") return { type: "error", label: "上次执行失败" };
    return item.enabled
      ? { type: "success", label: "已挂载启用" }
      : { type: "default", label: "已禁用" };
  }
  if (item.running) return { type: "success", label: "运行中" };
  if (item.active_state === "failed") return { type: "error", label: "异常退出" };
  if (item.active_state === "activating") return { type: "info", label: "启动中" };
  return { type: "default", label: "已停止" };
}

let alive = true;
let logReqId = 0;

async function refresh(silent = false) {
  if (!silent) loading.value = true;
  try {
    const next = await api.listServices();
    if (!alive) return;
    services.value = next;
    if (!silent) {
      statusText.value = `已同步 ${services.value.length} 项受管 systemd 单元状态`;
    }
    isError.value = false;
  } catch (err) {
    if (!alive) return;
    statusText.value = `读取后台服务状态失败：${String(err)}`;
    isError.value = true;
  } finally {
    if (alive) loading.value = false;
  }
}

const ACTION_LABEL: Record<"start" | "stop" | "restart" | "enable" | "disable", string> = {
  start: "启动",
  stop: "停止",
  restart: "重启",
  enable: "启用开机自启",
  disable: "关闭开机自启",
};

async function runAction(
  item: ManagedServiceItem,
  action: "start" | "stop" | "restart" | "enable" | "disable"
) {
  if (busyUnit.value) return;
  busyUnit.value = item.unit;
  const actionName = ACTION_LABEL[action];
  statusText.value =
    item.scope === "system"
      ? `正在请求管理员授权并${actionName} ${item.title}…`
      : `正在${actionName} ${item.title}…`;
  isError.value = false;
  try {
    const res = await api.controlService(item.unit, action);
    if (!alive) return;
    services.value = services.value.map((s) => (s.unit === item.unit ? res.service : s));
    if (action === "enable" || action === "disable") {
      await refresh(true);
    }
    statusText.value = `已${actionName}：${item.title} (${item.unit})`;
    message.success(`已${actionName} ${item.title}`);
    emit("servicesChanged");
  } catch (err) {
    const msg = String(err);
    statusText.value = `${actionName} ${item.title} 失败：${msg}`;
    isError.value = true;
    message.error(msg);
  } finally {
    busyUnit.value = null;
  }
}

async function restartAllUserDaemons() {
  const targets = daemons.value.filter((d) => d.scope === "user" && d.installed);
  if (targets.length === 0 || busyUnit.value) return;
  busyUnit.value = "__user_batch__";
  isError.value = false;
  statusText.value = "正在重启全部用户级 Rust 守护进程…";
  try {
    for (const item of targets) {
      await api.controlService(item.unit, "restart");
    }
    await refresh(true);
    statusText.value = "已重启全部用户级守护进程（水冷 + 会话灯光）";
    message.success("已重启全部用户级守护进程");
    emit("servicesChanged");
  } catch (err) {
    statusText.value = `批量重启失败：${String(err)}`;
    isError.value = true;
    message.error(String(err));
  } finally {
    busyUnit.value = null;
  }
}

async function openLogs(item: ManagedServiceItem) {
  logTarget.value = item;
  logLines.value = [];
  logWarning.value = null;
  logDrawerOpen.value = true;
  await fetchLogs();
}

async function fetchLogs() {
  const target = logTarget.value;
  if (!target) return;
  const reqId = ++logReqId;
  logLoading.value = true;
  logWarning.value = null;
  try {
    const res = await api.getServiceLogs(target.unit, logLineCount.value);
    if (!alive || reqId !== logReqId) return;
    logLines.value = res.lines;
    logWarning.value = res.warning;
    await nextTick();
    if (logConsoleRef.value) {
      logConsoleRef.value.scrollTop = logConsoleRef.value.scrollHeight;
    }
  } catch (err) {
    if (!alive || reqId !== logReqId) return;
    logLines.value = [];
    logWarning.value = `读取日志失败：${String(err)}`;
  } finally {
    if (alive && reqId === logReqId) logLoading.value = false;
  }
}

async function copyLogs() {
  if (logLines.value.length === 0) return;
  try {
    await navigator.clipboard.writeText(logLines.value.join("\n"));
    message.success("已复制日志到剪贴板");
  } catch {
    message.warning("复制失败，请手动选中文本");
  }
}

let pollTimer: number | null = null;
onMounted(() => {
  alive = true;
  refresh(false);
  pollTimer = window.setInterval(() => {
    if (!busyUnit.value) refresh(true);
  }, 5000);
});
onUnmounted(() => {
  alive = false;
  if (pollTimer !== null) window.clearInterval(pollTimer);
});
</script>

<template>
  <div class="page-stack">
    <!-- Hero Overview Banner -->
    <section
      class="ccu-hero"
      :style="{
        '--hero-accent':
          runningDaemons === daemons.length && daemons.length > 0
            ? 'var(--led-office)'
            : 'var(--accent-cyan)',
      }"
    >
      <div class="ccu-field-row" style="margin-bottom: 14px; flex-wrap: wrap; gap: 12px">
        <div>
          <h2 class="ccu-section-title">
            <ServerCog :size="18" color="var(--accent-cyan)" />
            <span>后台服务与守护进程管理</span>
          </h2>
          <p class="ccu-section-sub">
            集中监测与启停用户级 / 系统级 Rust 守护进程、开机自启开关、系统安全钩子与 journald 实时日志
          </p>
        </div>
        <div style="display: flex; gap: 8px; flex-wrap: wrap">
          <n-button
            size="small"
            secondary
            type="primary"
            :loading="busyUnit === '__user_batch__'"
            :disabled="busyUnit !== null"
            @click="restartAllUserDaemons"
          >
            <template #icon><RotateCw :size="14" /></template>
            重启全部用户守护
          </n-button>
          <n-button size="small" secondary :loading="loading" @click="refresh(false)">
            <template #icon><RefreshCw :size="14" /></template>
            刷新状态
          </n-button>
        </div>
      </div>

      <div class="kpi-grid-4">
        <div class="kpi-tile">
          <span class="kpi-label">核心常驻守护</span>
          <span class="kpi-value mono">
            {{ runningDaemons }} / {{ daemons.length || 4 }} 运行中
          </span>
        </div>
        <div class="kpi-tile">
          <span class="kpi-label">安全与休眠钩子</span>
          <span class="kpi-value mono">
            {{ enabledHooks }} / {{ hooks.length || 4 }} 已启用
          </span>
        </div>
        <div class="kpi-tile">
          <span class="kpi-label">常驻总内存占用</span>
          <span class="kpi-value mono">{{ totalMemoryText }}</span>
        </div>
        <div class="kpi-tile">
          <span class="kpi-label">受管单元自启</span>
          <span class="kpi-value mono">
            {{ totalEnabled }} / {{ services.length || 8 }} 已启用
          </span>
        </div>
      </div>
    </section>

    <!-- Core Resident Daemons (2x2 Grid) -->
    <div style="display: flex; flex-direction: column; gap: 12px">
      <div class="ccu-field-row">
        <div>
          <h3 class="ccu-section-title">核心常驻 Rust 守护进程</h3>
          <p class="ccu-section-sub">
            用户级服务免密即时控制；系统级服务通过系统 Polkit 管理员授权弹窗安全执行
          </p>
        </div>
      </div>

      <div class="grid-2">
        <n-card
          v-for="item in daemons"
          :key="item.unit"
          :bordered="true"
          content-style="display:flex; flex-direction:column; gap:12px;"
        >
          <div class="svc-head">
            <div style="min-width: 0">
              <div class="svc-title-row">
                <span
                  class="status-dot"
                  :class="
                    item.running
                      ? 'ok'
                      : item.active_state === 'failed'
                        ? 'danger'
                        : 'muted'
                  "
                />
                <span class="svc-title">{{ item.title }}</span>
              </div>
              <div class="svc-unit mono">{{ item.unit }} · {{ item.binary }}</div>
            </div>
            <div style="display: flex; gap: 6px; flex-shrink: 0; align-items: center">
              <n-tag
                size="tiny"
                :bordered="false"
                round
                :type="item.scope === 'user' ? 'info' : 'warning'"
              >
                {{ item.scope === "user" ? "用户级" : "系统级" }}
              </n-tag>
              <n-tag size="tiny" :bordered="false" round :type="stateBadge(item).type">
                {{ stateBadge(item).label }}
              </n-tag>
            </div>
          </div>

          <div class="svc-desc">{{ item.summary }}</div>

          <div class="ccu-well svc-meta-grid mono">
            <div>
              <span>PID</span>
              <strong>{{ item.main_pid > 0 ? item.main_pid : "--" }}</strong>
            </div>
            <div>
              <span>内存</span>
              <strong>{{ formatBytes(item.memory_bytes) }}</strong>
            </div>
            <div>
              <span>重启</span>
              <strong>{{ item.restarts }} 次</strong>
            </div>
            <div>
              <span>运行自</span>
              <strong>{{ formatSince(item.active_since) }}</strong>
            </div>
          </div>

          <div class="svc-footer">
            <div style="display: inline-flex; align-items: center; gap: 8px">
              <n-switch
                size="small"
                :value="item.enabled"
                :loading="busyUnit === item.unit"
                :disabled="!item.installed || busyUnit !== null"
                @update:value="(v: boolean) => runAction(item, v ? 'enable' : 'disable')"
              />
              <span class="svc-auto-label">开机自启</span>
            </div>

            <div style="display: flex; gap: 6px; flex-wrap: wrap">
              <n-button
                size="tiny"
                secondary
                type="success"
                :disabled="!item.installed || item.running || busyUnit !== null"
                @click="runAction(item, 'start')"
              >
                <template #icon><Play :size="12" /></template>
                启动
              </n-button>

              <n-button
                size="tiny"
                secondary
                type="info"
                :disabled="!item.installed || busyUnit !== null"
                :loading="busyUnit === item.unit"
                @click="runAction(item, 'restart')"
              >
                <template #icon><RotateCw :size="12" /></template>
                重启
              </n-button>

              <n-popconfirm
                v-if="item.scope === 'system'"
                positive-text="确认停止"
                negative-text="取消"
                @positive-click="runAction(item, 'stop')"
              >
                <template #trigger>
                  <n-button
                    size="tiny"
                    secondary
                    type="error"
                    :disabled="!item.installed || !item.running || busyUnit !== null"
                  >
                    <template #icon><Square :size="11" /></template>
                    停止
                  </n-button>
                </template>
                停止系统守护 {{ item.title }} 会暂停对应硬件保护或调度，确定继续？
              </n-popconfirm>
              <n-button
                v-else
                size="tiny"
                secondary
                type="error"
                :disabled="!item.installed || !item.running || busyUnit !== null"
                @click="runAction(item, 'stop')"
              >
                <template #icon><Square :size="11" /></template>
                停止
              </n-button>

              <n-button size="tiny" secondary @click="openLogs(item)">
                <template #icon><FileText :size="12" /></template>
                日志
              </n-button>
            </div>
          </div>
        </n-card>
      </div>
    </div>

    <!-- System Lifecycle & Safety Hooks -->
    <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:12px;">
      <div>
        <h3 class="ccu-section-title">
          <ShieldCheck :size="17" color="var(--led-office)" />
          <span>系统启动与休眠安全钩子（Oneshot / Sleep Target）</span>
        </h3>
        <p class="ccu-section-sub">
          此类单元仅在开机校验或系统挂起/休眠瞬间由 systemd 自动触发，桌面正常运行时处于待命态（inactive）属于预期行为，只需保持启用即可。
        </p>
      </div>

      <div class="hook-list">
        <div v-for="hook in hooks" :key="hook.unit" class="ccu-well hook-row">
          <div style="min-width: 0; flex: 1">
            <div style="display: flex; align-items: center; gap: 8px; flex-wrap: wrap">
              <span class="svc-title" style="font-size: 13.5px">{{ hook.title }}</span>
              <span class="svc-unit mono">{{ hook.unit }}</span>
              <n-tag size="tiny" :bordered="false" round :type="stateBadge(hook).type">
                {{ stateBadge(hook).label }}
              </n-tag>
            </div>
            <div class="ccu-field-hint" style="margin-top: 3px">{{ hook.summary }}</div>
          </div>

          <div style="display: flex; align-items: center; gap: 10px; flex-shrink: 0">
            <n-popconfirm
              v-if="hook.enabled"
              positive-text="确认禁用"
              negative-text="取消"
              @positive-click="runAction(hook, 'disable')"
            >
              <template #trigger>
                <n-button
                  size="tiny"
                  secondary
                  type="warning"
                  :disabled="!hook.installed || busyUnit !== null"
                  :loading="busyUnit === hook.unit"
                >
                  禁用钩子
                </n-button>
              </template>
              禁用 {{ hook.title }} 可能影响休眠/唤醒硬件安全回退，确定继续？
            </n-popconfirm>
            <n-button
              v-else
              size="tiny"
              secondary
              type="success"
              :disabled="!hook.installed || busyUnit !== null"
              :loading="busyUnit === hook.unit"
              @click="runAction(hook, 'enable')"
            >
              启用钩子
            </n-button>

            <n-button size="tiny" secondary @click="openLogs(hook)">
              <template #icon><FileText :size="12" /></template>
              日志
            </n-button>
          </div>
        </div>
      </div>
    </n-card>

    <n-alert :type="isError ? 'error' : 'info'" :show-icon="true">
      {{ statusText }}
    </n-alert>

    <!-- Live journalctl Log Drawer -->
    <n-drawer v-model:show="logDrawerOpen" :width="700" placement="right">
      <n-drawer-content closable>
        <template #header>
          <div style="display: flex; flex-direction: column; gap: 2px">
            <span>{{ logTarget?.title || "服务日志" }}</span>
            <span class="mono" style="font-size: 11px; color: var(--ink-muted)">
              {{ logTarget?.unit }}
              {{ logTarget?.fragment_path ? ` · ${logTarget.fragment_path}` : "" }}
            </span>
          </div>
        </template>

        <div style="display: flex; flex-direction: column; gap: 12px; height: 100%">
          <div class="ccu-field-row" style="flex-wrap: wrap; gap: 8px">
            <n-select
              v-model:value="logLineCount"
              size="small"
              style="width: 140px"
              :options="LOG_LINE_OPTIONS"
              @update:value="fetchLogs"
            />
            <div style="display: flex; gap: 8px">
              <n-button size="small" secondary :loading="logLoading" @click="fetchLogs">
                <template #icon><RefreshCw :size="13" /></template>
                刷新日志
              </n-button>
              <n-button
                size="small"
                secondary
                :disabled="logLines.length === 0"
                @click="copyLogs"
              >
                <template #icon><Copy :size="13" /></template>
                复制
              </n-button>
            </div>
          </div>

          <n-alert v-if="logWarning" type="warning" :show-icon="true">
            {{ logWarning }}
          </n-alert>

          <div ref="logConsoleRef" class="log-console mono">
            <div v-if="logLoading && logLines.length === 0" class="log-empty">
              正在读取 journald 日志…
            </div>
            <div v-else-if="logLines.length === 0" class="log-empty">
              暂无该服务的 journald 日志记录。
            </div>
            <pre v-else class="log-pre">{{ logLines.join("\n") }}</pre>
          </div>
        </div>
      </n-drawer-content>
    </n-drawer>
  </div>
</template>

<style scoped>
.svc-head {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 10px;
}

.svc-title-row {
  display: flex;
  align-items: center;
  gap: 8px;
}

.svc-title {
  font-size: 14.5px;
  font-weight: 700;
  color: var(--ink-strong);
}

.svc-unit {
  margin-top: 3px;
  font-size: 11px;
  color: var(--ink-muted);
}

.svc-desc {
  font-size: 12px;
  color: var(--ink-secondary);
  line-height: 1.5;
}

.svc-meta-grid {
  display: grid;
  grid-template-columns: 0.7fr 0.85fr 0.65fr 1.35fr;
  gap: 8px;
  font-size: 11.5px;
}

.svc-meta-grid > div {
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.svc-meta-grid span {
  font-size: 10.5px;
  color: var(--ink-muted);
}

.svc-meta-grid strong {
  color: var(--ink-primary);
  font-weight: 600;
}

.svc-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
  flex-wrap: wrap;
  padding-top: 4px;
}

.svc-auto-label {
  font-size: 12px;
  color: var(--ink-secondary);
}

.hook-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.hook-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
  flex-wrap: wrap;
}

.log-console {
  flex: 1;
  min-height: 360px;
  overflow: auto;
  border-radius: 10px;
  background: var(--bg-chart);
  border: 1px solid var(--border-subtle);
  padding: 12px 14px;
  user-select: text;
}

.log-pre {
  margin: 0;
  font-family: inherit;
  font-size: 11.5px;
  line-height: 1.55;
  color: var(--ink-secondary);
  white-space: pre-wrap;
  word-break: break-all;
}

.log-empty {
  font-size: 12px;
  color: var(--ink-muted);
  text-align: center;
  padding: 36px 12px;
}
</style>

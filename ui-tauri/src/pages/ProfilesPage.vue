<!-- SPDX-License-Identifier: AGPL-3.0-only -->
<!-- This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md). -->

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import {
  NAlert,
  NButton,
  NCard,
  NCheckbox,
  NInput,
  NInputNumber,
  NSelect,
  NSwitch,
  NTag,
  useMessage,
} from "naive-ui";
import { Plus, Trash2 } from "lucide-vue-next";
import { api } from "../api/tauri";
import type { ProfileBundle, ProfilesConfig } from "../types/hardware";

const emit = defineEmits<{
  hardwareChanged: [];
}>();

const message = useMessage();

const cfg = ref<ProfilesConfig>({
  version: 1,
  auto_switch_enabled: false,
  restore_previous: true,
  cooldown_seconds: 5,
  profiles: {},
  rules: [],
});

const selectedName = ref<string | null>(null);
const capName = ref("");
const capLabel = ref("");
const capPerf = ref(true);
const capLight = ref(true);

const ruleName = ref("");
const ruleMatch = ref("");
const ruleTarget = ref<string | null>(null);

const busy = ref(false);
const status = ref("正在读取情景预设配置…");
const isError = ref(false);

const profileNames = computed(() => Object.keys(cfg.value.profiles));
const profileOptions = computed(() =>
  profileNames.value.map((n) => ({ label: n, value: n }))
);

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

async function persist(next: ProfilesConfig, msgText: string) {
  busy.value = true;
  isError.value = false;
  try {
    const saved = await api.saveProfilesConfig(next);
    cfg.value = saved;
    status.value = msgText;
    message.success(msgText);
  } catch (err) {
    status.value = `保存失败：${String(err)}`;
    isError.value = true;
    message.error(String(err));
  } finally {
    busy.value = false;
  }
}

async function handleImportTemplates() {
  busy.value = true;
  isError.value = false;
  try {
    const templates = await api.getBuiltinTemplates();
    const added: string[] = [];
    const nextProfiles = { ...cfg.value.profiles };
    for (const [k, v] of Object.entries(templates)) {
      if (!nextProfiles[k]) {
        nextProfiles[k] = v;
        added.push(k);
      }
    }
    if (added.length === 0) {
      status.value = "内置模板已全部存在。";
      message.info("内置模板已全部存在");
      return;
    }
    const saved = await api.saveProfilesConfig({ ...cfg.value, profiles: nextProfiles });
    cfg.value = saved;
    selectedName.value = added[0];
    if (!ruleTarget.value) ruleTarget.value = added[0];
    status.value = `已导入内置模板：${added.join("、")}`;
    message.success(`已导入 ${added.length} 个内置模板`);
  } catch (err) {
    status.value = `导入失败：${String(err)}`;
    isError.value = true;
  } finally {
    busy.value = false;
  }
}

async function handleApplySelected() {
  if (!selectedName.value || !cfg.value.profiles[selectedName.value]) return;
  busy.value = true;
  isError.value = false;
  status.value = `正在应用预设 ${selectedName.value}…`;
  try {
    const res = await api.applyProfileBundle(cfg.value.profiles[selectedName.value]);
    const warn = res.warnings.length ? `（注意：${res.warnings.join("；")}）` : "";
    status.value = `已应用预设 ${selectedName.value}${warn}`;
    message.success(`已应用预设 ${selectedName.value}`);
    emit("hardwareChanged");
  } catch (err) {
    status.value = `应用预设失败：${String(err)}`;
    isError.value = true;
    message.error(String(err));
  } finally {
    busy.value = false;
  }
}

async function handleDeleteSelected() {
  if (!selectedName.value) return;
  const deleting = selectedName.value;
  const nextProfiles = { ...cfg.value.profiles };
  delete nextProfiles[deleting];
  const nextRules = cfg.value.rules.filter((r) => r.profile !== deleting);
  const nextNames = Object.keys(nextProfiles);
  selectedName.value = nextNames[0] ?? null;
  if (ruleTarget.value === deleting) ruleTarget.value = nextNames[0] ?? null;
  await persist(
    { ...cfg.value, profiles: nextProfiles, rules: nextRules },
    `已删除预设 ${deleting}`
  );
}

async function handleCapture() {
  const cleanName = capName.value.trim();
  if (!cleanName || cleanName.includes("/") || cleanName.includes("\\")) {
    status.value = "请输入有效的预设名称（不能包含路径分隔符）";
    isError.value = true;
    return;
  }
  busy.value = true;
  isError.value = false;
  status.value = `正在捕获当前硬件状态并保存为 ${cleanName}…`;
  try {
    const bundle = await api.captureCurrentProfile(
      capLabel.value.trim() || null,
      capLight.value,
      capPerf.value
    );
    const saved = await api.saveProfilesConfig({
      ...cfg.value,
      profiles: { ...cfg.value.profiles, [cleanName]: bundle },
    });
    cfg.value = saved;
    selectedName.value = cleanName;
    if (!ruleTarget.value) ruleTarget.value = cleanName;
    capName.value = "";
    capLabel.value = "";
    status.value = `已从当前硬件状态保存预设 ${cleanName}`;
    message.success(`已保存预设 ${cleanName}`);
  } catch (err) {
    status.value = `捕获失败：${String(err)}`;
    isError.value = true;
    message.error(String(err));
  } finally {
    busy.value = false;
  }
}

function handleAddRule() {
  const name = ruleName.value.trim();
  const tokens = ruleMatch.value
    .split(",")
    .map((s) => s.trim().toLowerCase())
    .filter(Boolean);
  const target = ruleTarget.value || profileNames.value[0] || "";
  if (!name || tokens.length === 0 || !target || !cfg.value.profiles[target]) {
    status.value = "请填写规则名称、至少一个进程关键词，并选择目标预设";
    isError.value = true;
    return;
  }
  const filtered = cfg.value.rules.filter((r) => r.name !== name);
  cfg.value.rules = [...filtered, { name, enabled: true, match: tokens, profile: target }];
  ruleName.value = "";
  ruleMatch.value = "";
  status.value = `已添加规则 ${name}（点击“保存预设与联动规则”写入配置）`;
  isError.value = false;
}

onMounted(async () => {
  try {
    const loaded = await api.getProfilesConfig();
    cfg.value = loaded;
    const names = Object.keys(loaded.profiles);
    selectedName.value = names[0] ?? null;
    ruleTarget.value = names[0] ?? null;
    status.value = loaded.auto_switch_enabled
      ? "应用自动联动已启用。"
      : "自动联动默认关闭；预设仅在点击应用或匹配已启用规则时生效。";
    isError.value = false;
  } catch (err) {
    status.value = `读取预设失败：${String(err)}`;
    isError.value = true;
  }
});
</script>

<template>
  <div class="page-stack">
    <div class="grid-2">
      <!-- Left: Scenario Bundles -->
      <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:14px;">
        <div>
          <h2 class="ccu-section-title">情景预设组合（性能 + 灯光）</h2>
          <p class="ccu-section-sub">将固件性能档位与三区 RGB 灯效打包为一键情景</p>
        </div>

        <div class="ccu-well" style="display: flex; flex-direction: column; gap: 10px">
          <div class="ccu-field-row">
            <span class="ccu-field-label">当前预设</span>
            <n-select
              v-model:value="selectedName"
              style="width: 220px"
              placeholder="（暂无预设）"
              :disabled="profileNames.length === 0"
              :options="profileOptions"
            />
          </div>
          <div class="ccu-field-hint">
            {{ summarizeProfile(selectedName ? cfg.profiles[selectedName] : undefined) }}
          </div>
          <div style="display: flex; flex-wrap: wrap; gap: 8px; margin-top: 2px">
            <n-button
              size="small"
              type="primary"
              :disabled="busy || !selectedName"
              @click="handleApplySelected"
            >
              立即应用预设
            </n-button>
            <n-button size="small" secondary :disabled="busy" @click="handleImportTemplates">
              导入内置模板
            </n-button>
            <n-button
              size="small"
              secondary
              type="error"
              :disabled="busy || !selectedName"
              @click="handleDeleteSelected"
            >
              删除预设
            </n-button>
          </div>
        </div>

        <div class="ccu-well" style="display: flex; flex-direction: column; gap: 10px">
          <div class="ccu-field-label">从当前硬件状态捕获并保存</div>
          <div class="grid-2" style="gap: 8px">
            <n-input v-model:value="capName" size="small" placeholder="预设名称（如 coding / fps）" />
            <n-input v-model:value="capLabel" size="small" placeholder="备注说明（可选）" />
          </div>
          <div style="display: flex; align-items: center; gap: 14px; flex-wrap: wrap">
            <n-checkbox v-model:checked="capPerf">包含性能档位</n-checkbox>
            <n-checkbox v-model:checked="capLight">包含三区灯光</n-checkbox>
            <n-button
              size="small"
              secondary
              type="primary"
              style="margin-left: auto"
              :disabled="busy"
              @click="handleCapture"
            >
              捕获保存
            </n-button>
          </div>
        </div>
      </n-card>

      <!-- Right: App Auto-Switch Rules -->
      <n-card :bordered="true" content-style="display:flex; flex-direction:column; gap:14px;">
        <div class="ccu-field-row">
          <div>
            <h2 class="ccu-section-title">应用进程自动联动</h2>
            <p class="ccu-section-sub">按下机身模式键 (0xB0) 立即让步，EC 高温保护始终优先</p>
          </div>
          <n-switch v-model:value="cfg.auto_switch_enabled" />
        </div>

        <div
          class="ccu-well ccu-field-row"
          style="flex-wrap: wrap; gap: 10px"
          :style="{ opacity: cfg.auto_switch_enabled ? 1 : 0.48 }"
        >
          <n-checkbox v-model:checked="cfg.restore_previous" :disabled="!cfg.auto_switch_enabled">
            应用退出后恢复先前状态
          </n-checkbox>
          <div style="display: inline-flex; align-items: center; gap: 8px">
            <span class="ccu-field-hint" style="margin: 0">退出防抖</span>
            <n-input-number
              size="tiny"
              style="width: 96px"
              :min="0"
              :max="120"
              :disabled="!cfg.auto_switch_enabled"
              :value="cfg.cooldown_seconds"
              @update:value="(v) => (cfg.cooldown_seconds = v ?? 5)"
            >
              <template #suffix>秒</template>
            </n-input-number>
          </div>
        </div>

        <div style="display: flex; flex-direction: column; gap: 8px">
          <div v-if="cfg.rules.length === 0" class="ccu-well ccu-field-hint">
            尚无应用联动规则，可在下方按进程关键词添加。
          </div>
          <div
            v-for="(rule, idx) in cfg.rules"
            :key="rule.name"
            class="ccu-well ccu-field-row"
          >
            <div style="display: flex; align-items: center; gap: 10px; min-width: 0">
              <n-checkbox v-model:checked="rule.enabled" />
              <div>
                <div style="font-weight: 650; font-size: 13px; color: var(--ink-strong)">{{ rule.name }}</div>
                <div class="ccu-field-hint mono">
                  {{ rule.match.join(", ") }} →
                  <n-tag size="tiny" :bordered="false" type="info">{{ rule.profile }}</n-tag>
                </div>
              </div>
            </div>
            <n-button
              size="tiny"
              secondary
              type="error"
              @click="
                () => {
                  cfg.rules.splice(idx, 1);
                  status = `已移除规则 ${rule.name}（点击保存生效）`;
                }
              "
            >
              <template #icon><Trash2 :size="12" /></template>
              移除
            </n-button>
          </div>
        </div>

        <div class="ccu-well" style="display: flex; flex-direction: column; gap: 8px">
          <div style="display: grid; grid-template-columns: 1fr 1.4fr; gap: 8px">
            <n-input v-model:value="ruleName" size="small" placeholder="规则名称" />
            <n-input v-model:value="ruleMatch" size="small" placeholder="匹配进程关键词（逗号分隔）" />
          </div>
          <div class="ccu-field-row">
            <n-select
              v-model:value="ruleTarget"
              size="small"
              style="width: 210px"
              placeholder="选择目标预设"
              :disabled="profileNames.length === 0"
              :options="profileOptions"
            />
            <n-button
              size="small"
              secondary
              :disabled="profileNames.length === 0"
              @click="handleAddRule"
            >
              <template #icon><Plus :size="13" /></template>
              添加规则
            </n-button>
          </div>
        </div>

        <div>
          <n-button
            type="primary"
            :loading="busy"
            @click="
              persist(
                cfg,
                cfg.auto_switch_enabled
                  ? '已保存；应用自动联动已启用。'
                  : '已保存；应用自动联动保持关闭。'
              )
            "
          >
            保存预设与联动规则
          </n-button>
        </div>
      </n-card>
    </div>

    <n-alert :type="isError ? 'error' : 'info'" :show-icon="true">
      {{ status }}
    </n-alert>
  </div>
</template>

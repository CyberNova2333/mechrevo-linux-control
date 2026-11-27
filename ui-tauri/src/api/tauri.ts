// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

import { invoke } from "@tauri-apps/api/core";
import type {
  KeyboardHwInfo,
  LightingPolicyConfig,
  LightingSavedParams,
  LightingStateResponse,
  PerformanceStatus,
  ProfileBundle,
  ProfilesConfig,
  Rgb,
  SystemInfo,
  TuningConfig,
  WaterCoolingConfig,
  WaterCoolingSnapshot,
} from "../types/hardware";

export const api = {
  getSystemInfo: () => invoke<SystemInfo>("get_system_info"),
  allLightsOff: () => invoke<{ turned_off: string[]; errors: string[] }>("all_lights_off"),

  getLightingState: (device: "keyboard" | "lightbar") =>
    invoke<LightingStateResponse>("get_lighting_state", { device }),
  applyLighting: (device: "keyboard" | "lightbar", params: LightingSavedParams) =>
    invoke<{ ok: boolean; hardware: KeyboardHwInfo | null }>("apply_lighting", { device, params }),
  pollKeyboardHw: () => invoke<KeyboardHwInfo>("poll_keyboard_hw"),
  getMatrixPreset: (preset: string, accent: Rgb) =>
    invoke<Rgb[]>("get_matrix_preset", { preset, accent }),

  getLogoStatus: () =>
    invoke<{
      hardware: { logo_enabled: boolean; mode: string; brightness: number; color: Rgb };
      prefs: { mode: string; brightness: number; color: Rgb } | null;
    }>("get_logo_status"),
  applyLogo: (mode: string, brightness: number, color: Rgb) =>
    invoke<{ logo_enabled: boolean; mode: string; brightness: number; color: Rgb }>("apply_logo", {
      mode,
      brightness,
      color,
    }),
  setLogoEnabled: (enabled: boolean) =>
    invoke<{ logo_enabled: boolean; mode: string; brightness: number; color: Rgb }>("set_logo_enabled", {
      enabled,
    }),

  getLightingPolicy: () => invoke<LightingPolicyConfig>("get_lighting_policy"),
  saveLightingPolicy: (config: LightingPolicyConfig) =>
    invoke<LightingPolicyConfig>("save_lighting_policy", { config }),

  getProfilesConfig: () => invoke<ProfilesConfig>("get_profiles_config"),
  saveProfilesConfig: (config: ProfilesConfig) =>
    invoke<ProfilesConfig>("save_profiles_config", { config }),
  getBuiltinTemplates: () => invoke<Record<string, ProfileBundle>>("get_builtin_templates"),
  captureCurrentProfile: (label: string | null, includeLighting: boolean, includePerformance: boolean) =>
    invoke<ProfileBundle>("capture_current_profile", {
      label,
      includeLighting,
      includePerformance,
    }),
  applyProfileBundle: (profile: ProfileBundle) =>
    invoke<{ lighting: string[]; performance: string | null; warnings: string[] }>("apply_profile_bundle", {
      profile,
    }),

  getPerformanceStatus: () =>
    invoke<{ status: PerformanceStatus; prefs: TuningConfig | null }>("get_performance_status"),
  setPerformanceMode: (mode: string) => invoke<PerformanceStatus>("set_performance_mode", { mode }),
  setTurboVariant: (variant: string) => invoke<PerformanceStatus>("set_turbo_variant", { variant }),
  applyPerformanceTuning: (config: TuningConfig) =>
    invoke<PerformanceStatus>("apply_performance_tuning", { config }),
  resetPerformanceTuning: () => invoke<PerformanceStatus>("reset_performance_tuning"),

  getWaterCoolingLocalConfig: () => invoke<WaterCoolingConfig>("get_watercooling_local_config"),
  waterCoolingRequest: <T = WaterCoolingSnapshot>(req: Record<string, unknown>) =>
    invoke<T>("watercooling_request", { req }),
};

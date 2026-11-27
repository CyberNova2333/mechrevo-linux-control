// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).

export type Rgb = [number, number, number];

export type ThemePreference = "system" | "light" | "dark";
export type ResolvedTheme = "light" | "dark";

export type PageId =
  | "home"
  | "performance"
  | "lighting"
  | "watercooling"
  | "profiles"
  | "services";

export interface ManagedServiceItem {
  unit: string;
  title: string;
  summary: string;
  binary: string;
  scope: "user" | "system";
  kind: "daemon" | "hook";
  installed: boolean;
  load_state: string;
  active_state: string;
  sub_state: string;
  enabled_state: string;
  enabled: boolean;
  running: boolean;
  main_pid: number;
  memory_bytes: number | null;
  restarts: number;
  active_since: string;
  unit_type: string;
  fragment_path: string;
}

export interface ServiceLogsResponse {
  unit: string;
  title: string;
  lines: string[];
  warning: string | null;
}

export type LightingTabId = "keyboard" | "lightbar" | "logo" | "policy";

export interface LiveTelemetry {
  cpu: {
    model: string;
    usage_pct: number;
    temp_c: number | null;
    freq_mhz: number | null;
    max_freq_mhz: number;
    cores: number;
  };
  gpu: {
    available: boolean;
    name: string;
    temp_c: number | null;
    usage_pct: number | null;
    power_w: number | null;
    clock_mhz: number | null;
    vram_used_mb: number | null;
    vram_total_mb: number | null;
  };
  memory: {
    used_gb: number;
    total_gb: number;
    usage_pct: number;
  };
  storage: {
    used_gb: number;
    total_gb: number;
    usage_pct: number;
    nvme_temp_c: number | null;
  };
  power: {
    ac_online: boolean;
    battery_pct: number | null;
    battery_status: string;
  };
  ec_cooler: {
    lc_supported?: boolean;
    ec_pump_enabled?: boolean;
    ec_pump_volt?: number;
    ec_fan_enabled?: boolean;
    ec_fan_duty?: number;
  } | null;
}

export interface SystemInfo {
  sys_vendor: string;
  board_name: string;
  bios_version: string;
  kernel: string;
  hid_devices: string[];
  daemons: {
    watercooling: boolean;
    session_restore: boolean;
    turbo: boolean;
    profile_guard: boolean;
  };
  version: string;
}

export interface KeyboardHwInfo {
  power: number;
  mode: number;
  mode_name: string;
  speed: number;
  brightness: number;
  direction: number;
  ui_direction: number;
}

export interface LightingSavedParams {
  mode?: string;
  color?: Rgb;
  colors?: Rgb[];
  brightness?: number;
  speed?: number;
  direction?: number;
  reactive?: boolean;
  matrix?: Rgb[];
  preset?: string;
  calibrate?: boolean;
}

export interface LightingStateResponse {
  device: "keyboard" | "lightbar";
  available: boolean;
  bmax: number;
  saved: LightingSavedParams;
  hardware: KeyboardHwInfo | null;
}

export interface ModeMeta {
  id: string;
  title: string;
  desc: string;
  off?: boolean;
  single?: boolean;
  matrix?: boolean;
  palette?: boolean;
  speed?: boolean;
  direction?: boolean;
  trigger?: boolean;
}

export const RAINBOW_DEFAULT: Rgb[] = [
  [255, 0, 0],
  [255, 165, 0],
  [255, 255, 0],
  [0, 255, 0],
  [0, 0, 255],
  [0, 255, 255],
  [139, 0, 255],
];

export const KEYBOARD_MODES: ModeMeta[] = [
  { id: "off", title: "关闭", desc: "关闭键盘背光", off: true },
  { id: "solid", title: "常亮", desc: "整个键盘保持同一种颜色", single: true },
  { id: "matrix", title: "单键自定义", desc: "在上方键盘直接左键上色、右键熄灭，或套用预设", matrix: true },
  { id: "breathing", title: "呼吸", desc: "灯光逐渐亮起与淡出", palette: true, speed: true },
  { id: "wave", title: "彩色波浪", desc: "色彩沿指定方向流动", palette: true, speed: true, direction: true },
  { id: "sine", title: "正弦波", desc: "色彩如正弦曲线般柔和起伏", palette: true, speed: true, direction: true },
  { id: "interlace", title: "交错波", desc: "多色光带双向交错扫过", palette: true, speed: true },
  { id: "diagonal", title: "斜向波", desc: "色彩沿对角线方向流动", palette: true, speed: true, direction: true },
  { id: "reactive", title: "随机闪烁", desc: "星点随机闪烁，也可随按键触发", palette: true, speed: true, trigger: true },
  { id: "rainbow", title: "彩虹渐变", desc: "自动循环彩虹色彩", speed: true },
  { id: "ripple", title: "涟漪", desc: "灯光向周围扩散", palette: true, speed: true, trigger: true },
  { id: "marquee", title: "跑马灯", desc: "色彩依次流转", palette: true, speed: true },
  { id: "raindrop", title: "雨滴", desc: "如雨滴般散落的灯光", palette: true, speed: true },
  { id: "aurora", title: "极光", desc: "光带柔和扩散，也可随按键触发", palette: true, speed: true, trigger: true },
  { id: "spark", title: "烟花", desc: "光点绽放，也可随按键触发", palette: true, speed: true, trigger: true },
];

export const LIGHTBAR_MODES: ModeMeta[] = [
  { id: "off", title: "关闭", desc: "关闭机身灯带", off: true },
  { id: "static", title: "常亮", desc: "灯带保持同一种颜色", single: true },
  { id: "breathing", title: "呼吸", desc: "灯光逐渐亮起与淡出", palette: true, speed: true },
  { id: "wave", title: "彩色波浪", desc: "固定彩虹沿指定方向流动", speed: true, direction: true },
  { id: "marquee", title: "跑马灯", desc: "色彩依次流转", palette: true, speed: true, direction: true },
  { id: "neon", title: "霓虹", desc: "多种颜色交替变化", palette: true, speed: true, direction: true },
  { id: "runner", title: "追光", desc: "光点沿灯带移动", palette: true, speed: true, direction: true },
];

export const KEYBOARD_DIRECTIONS = [
  { value: 1, label: "从左向右" },
  { value: 2, label: "从右向左" },
  { value: 3, label: "从下向上" },
  { value: 4, label: "从上向下" },
];

export const LIGHTBAR_DIRECTIONS = [
  { value: 1, label: "正向" },
  { value: 2, label: "反向" },
];

export const MATRIX_PRESETS = [
  { id: "gaming", label: "游戏分区" },
  { id: "wasd_fps", label: "FPS WASD" },
  { id: "zones", label: "功能分区" },
  { id: "rainbow_grid", label: "彩虹矩阵" },
  { id: "gradient", label: "双色渐变" },
];

export interface KeyCapDef {
  name: string;
  width: number;
  indices: number[];
}

function cap(name: string, row: number, col: number, width = 1, extra: number[] = []): KeyCapDef {
  return { name, width, indices: [row * 21 + col, ...extra] };
}

export const KEYBOARD_101_ROWS: KeyCapDef[][] = [
  [
    cap("Esc", 0, 0),
    ...Array.from({ length: 12 }, (_, i) => cap(`F${i + 1}`, 0, i + 1)),
    cap("PrtSc", 0, 13),
    cap("Pause", 0, 14),
    cap("Del", 0, 15),
    cap("Home", 0, 16),
    cap("PgUp", 0, 17),
    cap("PgDn", 0, 18),
    cap("End", 0, 19),
  ],
  [
    ...["`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "="].map((l, c) => cap(l, 1, c)),
    cap("Bksp", 1, 14, 2.0),
    cap("NumLk", 1, 15),
    cap("/", 1, 16),
    cap("*", 1, 17),
    cap("-", 1, 18),
  ],
  [
    cap("Tab", 2, 0, 1.5),
    ...["Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]", "\\"].map((l, i) => cap(l, 2, 2 + i)),
    cap("Num7", 2, 15),
    cap("Num8", 2, 16),
    cap("Num9", 2, 17),
    cap("+", 2, 18),
  ],
  [
    cap("Caps", 3, 0, 1.8),
    ...["A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'"].map((l, i) => cap(l, 3, 2 + i)),
    cap("Enter", 3, 14, 2.0),
    cap("Num4", 3, 15),
    cap("Num5", 3, 16),
    cap("Num6", 3, 17),
  ],
  [
    cap("LShift", 4, 0, 2.3, [85]),
    ...["Z", "X", "C", "V", "B", "N", "M", ",", ".", "/"].map((l, i) => cap(l, 4, 3 + i)),
    cap("RShift", 4, 14, 1.7),
    cap("↑", 4, 15),
    cap("Num1", 4, 16),
    cap("Num2", 4, 17),
    cap("Num3", 4, 18),
  ],
  [
    cap("LCtrl", 5, 0, 1.5),
    cap("Fn", 5, 2),
    cap("Win", 5, 3),
    cap("LAlt", 5, 4),
    cap("Space", 5, 7, 4.4, [110, 111, 113, 114]),
    cap("RAlt", 5, 10),
    cap("RCtrl", 5, 12),
    cap("←", 5, 13),
    cap("↓", 5, 14),
    cap("→", 5, 15),
    cap("Num0", 5, 16, 1.5),
    cap("Num.", 5, 17),
    cap("NumEnt", 5, 18),
  ],
];

export const LOGO_MODES = [
  { id: "solid", label: "常亮" },
  { id: "breathing", label: "呼吸" },
  { id: "rainbow", label: "七彩渐变" },
] as const;

export const LOGO_FIXED_COLORS: { name: string; rgb: Rgb }[] = [
  { name: "白色", rgb: [255, 255, 255] },
  { name: "红色", rgb: [255, 0, 0] },
  { name: "绿色", rgb: [0, 255, 0] },
  { name: "蓝色", rgb: [0, 0, 255] },
  { name: "黄色", rgb: [255, 255, 0] },
  { name: "紫色", rgb: [136, 0, 255] },
  { name: "青色", rgb: [0, 255, 255] },
];

export const MIN_FAN_DUTY = [40, 40, 40, 40, 50, 55, 60, 70, 80, 90, 100];
export const CPU_FAN_TEMPS = [44, 48, 52, 56, 60, 64, 68, 72, 76, 80, 85];
export const GPU_FAN_TEMPS = [42, 46, 50, 54, 58, 62, 66, 70, 74, 78, 81];

export interface LightingPolicyConfig {
  version: 1;
  enabled: boolean;
  power_enabled: boolean;
  idle_minutes: number;
  brightness: {
    keyboard: { ac: number; dc: number };
    lightbar: { ac: number; dc: number };
    logo: { ac: number; dc: number };
  };
}

export interface ProfileBundle {
  label?: string;
  performance?: {
    mode: "office" | "balanced" | "turbo" | "custom";
    turbo?: "quiet" | "extreme";
    tuning?: TuningConfig;
  };
  lighting?: {
    keyboard?: LightingSavedParams;
    lightbar?: LightingSavedParams;
    logo?: { mode: string; brightness: number; color: Rgb };
  };
}

export interface ProfileRule {
  name: string;
  enabled: boolean;
  match: string[];
  profile: string;
}

export interface ProfilesConfig {
  version: 1;
  auto_switch_enabled: boolean;
  restore_previous: boolean;
  cooldown_seconds: number;
  profiles: Record<string, ProfileBundle>;
  rules: ProfileRule[];
}

export interface TuningConfig {
  pl1: number;
  pl2: number;
  cpu_fan: number[];
  gpu_fan: number[];
  gpu_ctgp?: number;
  gpu_db?: number;
}

export interface PerformanceStatus {
  mode: string;
  switchable: boolean;
  reason: string;
  ec_mode?: string;
  tuning_active: boolean;
  tuning_recovery_pending: boolean;
  tuning: TuningConfig | null;
  gpu_tuning_supported: boolean;
  gpu_tuning_range?: [number, number];
  gpu_db_range?: [number, number];
  gpu_tuning_reason?: string;
  fan_safety_protect: boolean;
  smart_balance_supported: boolean;
  smart_balance_enabled: boolean;
  wmi_bound?: boolean;
  turbo?: {
    available?: boolean;
    active?: boolean;
    variant?: string;
    recovery_needed?: boolean;
    policy_uncertain?: boolean;
    standard_turbo?: boolean;
    round_adapter?: boolean;
    combined_supported?: boolean;
    read_rc?: number;
  };
}

export interface WaterCoolingCurvePoint {
  temp_c: number;
  pump_volt: number;
  pump_duty: number;
  fan_duty: number;
}

export interface WaterCoolingConfig {
  version: 1;
  auto_connect: boolean;
  mac: string;
  strategy: "preset" | "manual" | "smart_curve" | "ec_follow";
  preset: "stop" | "silent" | "balanced" | "performance" | "turbo" | "max";
  manual: {
    pump_volt: number;
    pump_duty: number;
    fan_duty: number;
  };
  curve: {
    hysteresis_c: number;
    points: WaterCoolingCurvePoint[];
  };
  led: {
    mode: "off" | "static" | "breathing" | "colorful" | "breathing_colorful" | "mode4";
    color: Rgb;
    fan_policy: "sync" | "body_only" | "freeze" | "split";
    fan_color: Rgb;
  };
  flow_protection: boolean;
  sleep_on_disconnect: boolean;
}

export interface WaterCoolingSnapshot {
  connected: boolean;
  sleeping: boolean;
  flow_tripped: boolean;
  flow_trip_reason?: string;
  last_error?: string;
  device_name?: string;
  device_mac?: string;
  fw_version?: string;
  reason?: string;
  meter_state?: number | null;
  temp_c?: number | null;
  applied?: {
    pump_volt: number;
    pump_duty: number;
    fan_duty: number;
  };
  ec?: {
    ec_available?: boolean;
    ec_stale?: boolean;
    lc_supported?: boolean;
    ec_pump_enabled?: boolean;
    ec_pump_volt?: number;
    ec_fan_enabled?: boolean;
    ec_fan_duty?: number;
    wmi_available?: boolean;
    wmi_c1_seq?: number;
  };
  config?: WaterCoolingConfig;
}

export function rgbToHex([r, g, b]: Rgb): string {
  const h = (n: number) => Math.max(0, Math.min(255, Math.round(n))).toString(16).padStart(2, "0");
  return `#${h(r)}${h(g)}${h(b)}`;
}

export function hexToRgb(hex: string): Rgb {
  const clean = hex.replace("#", "");
  if (clean.length !== 6) return [0, 255, 255];
  return [
    parseInt(clean.slice(0, 2), 16) || 0,
    parseInt(clean.slice(2, 4), 16) || 0,
    parseInt(clean.slice(4, 6), 16) || 0,
  ];
}

export function rgbCss([r, g, b]: Rgb, alpha = 1): string {
  return alpha >= 1 ? `rgb(${r}, ${g}, ${b})` : `rgba(${r}, ${g}, ${b}, ${alpha})`;
}

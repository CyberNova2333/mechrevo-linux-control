// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Native ITE 8291/8233 USB HID protocol, 101-key matrix presets, and hidraw I/O.

use crate::zone_lock::{read_user_json, state_dir, write_user_json, ZoneGuard, MAX_STATE_BYTES};
use serde::Serialize;
use serde_json::{json, Map, Value};
use std::fs::{self, File, OpenOptions};
use std::io::Write;
use std::os::unix::io::AsRawFd;
use std::time::Duration;

pub const KBD_BRIGHTNESS_MAX: u8 = 0x32; // 50
pub const LB_BRIGHTNESS_MAX: u8 = 0x64; // 100
pub const KBD_MATRIX_ROWS: usize = 6;
pub const KBD_MATRIX_COLS: usize = 21;
pub const KBD_MATRIX_CELLS: usize = KBD_MATRIX_ROWS * KBD_MATRIX_COLS;
pub const KBD_WIRE_SLOTS: usize = 128;
pub const SPEED_MIN: u8 = 1;
pub const SPEED_MAX: u8 = 10;
pub const LB_SELECTOR: u8 = 0x22;
pub const KBD_POWER_ON: u8 = 0x02;
pub const QUERY_EFFECT: u8 = 0x88;

pub type Rgb = [u8; 3];

pub const RAINBOW: [Rgb; 7] = [
    [255, 0, 0],
    [255, 165, 0],
    [255, 255, 0],
    [0, 255, 0],
    [0, 0, 255],
    [0, 255, 255],
    [139, 0, 255],
];

pub const LOGO_COLORS: [Rgb; 7] = [
    [255, 255, 255],
    [255, 0, 0],
    [0, 255, 0],
    [0, 0, 255],
    [255, 255, 0],
    [136, 0, 255],
    [0, 255, 255],
];

pub const PRESET_NAMES: &[&str] = &["gaming", "wasd_fps", "zones", "rainbow_grid", "gradient"];

#[derive(Debug, Clone)]
pub enum Report {
    Feature([u8; 8]),
    Output(Vec<u8>),
}

#[derive(Debug, Clone, Serialize)]
pub struct KeyboardHwInfo {
    pub power: u8,
    pub mode: u8,
    pub mode_name: String,
    pub speed: u8,
    pub brightness: u8,
    pub direction: u8,
    pub ui_direction: u8,
}

#[derive(Debug, Clone, Copy)]
struct ModeSpec {
    name: &'static str,
    code: u8,
    off: bool,
    single: bool,
    matrix: bool,
    palette: bool,
    direction: bool,
    trigger: bool,
    fixed_direction: Option<u8>,
    direction_map: Option<[u8; 4]>,
}

const LIGHTBAR_MODES: &[ModeSpec] = &[
    ModeSpec { name: "off", code: 0x00, off: true, single: false, matrix: false, palette: false, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "static", code: 0x01, off: false, single: true, matrix: false, palette: false, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "breathing", code: 0x02, off: false, single: false, matrix: false, palette: true, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "wave", code: 0x03, off: false, single: false, matrix: false, palette: false, direction: true, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "marquee", code: 0x04, off: false, single: false, matrix: false, palette: true, direction: true, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "neon", code: 0x05, off: false, single: false, matrix: false, palette: true, direction: true, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "runner", code: 0x06, off: false, single: false, matrix: false, palette: true, direction: true, trigger: false, fixed_direction: None, direction_map: None },
];

const KEYBOARD_MODES: &[ModeSpec] = &[
    ModeSpec { name: "off", code: 0x00, off: true, single: false, matrix: false, palette: false, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "solid", code: 0x33, off: false, single: true, matrix: false, palette: false, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "matrix", code: 0x33, off: false, single: false, matrix: true, palette: false, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "breathing", code: 0x02, off: false, single: false, matrix: false, palette: true, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "wave", code: 0x03, off: false, single: false, matrix: false, palette: true, direction: true, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "sine", code: 0x03, off: false, single: false, matrix: false, palette: true, direction: true, trigger: false, fixed_direction: None, direction_map: Some([16, 17, 16, 17]) },
    ModeSpec { name: "interlace", code: 0x03, off: false, single: false, matrix: false, palette: true, direction: false, trigger: false, fixed_direction: Some(19), direction_map: None },
    ModeSpec { name: "diagonal", code: 0x03, off: false, single: false, matrix: false, palette: true, direction: true, trigger: false, fixed_direction: None, direction_map: Some([33, 32, 33, 32]) },
    ModeSpec { name: "reactive", code: 0x04, off: false, single: false, matrix: false, palette: true, direction: false, trigger: true, fixed_direction: None, direction_map: None },
    ModeSpec { name: "rainbow", code: 0x05, off: false, single: false, matrix: false, palette: false, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "ripple", code: 0x06, off: false, single: false, matrix: false, palette: true, direction: false, trigger: true, fixed_direction: None, direction_map: None },
    ModeSpec { name: "marquee", code: 0x09, off: false, single: false, matrix: false, palette: true, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "raindrop", code: 0x0A, off: false, single: false, matrix: false, palette: true, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "aurora", code: 0x0E, off: false, single: false, matrix: false, palette: true, direction: false, trigger: true, fixed_direction: None, direction_map: None },
    ModeSpec { name: "spark", code: 0x11, off: false, single: false, matrix: false, palette: true, direction: false, trigger: true, fixed_direction: None, direction_map: None },
];

fn find_spec(device: &str, mode: &str) -> Option<&'static ModeSpec> {
    let list = match device {
        "keyboard" => KEYBOARD_MODES,
        "lightbar" => LIGHTBAR_MODES,
        _ => return None,
    };
    list.iter().find(|s| s.name == mode)
}

fn calibrate(rgb: Rgb) -> Rgb {
    match rgb {
        [255, 255, 255] => [125, 255, 185],
        [255, 165, 0] => [255, 125, 0],
        [255, 255, 0] => [210, 255, 0],
        [0, 0, 255] => [0, 20, 255],
        [139, 0, 255] => [110, 0, 255],
        other => other,
    }
}

fn palette_reports(colors: &[Rgb]) -> Vec<Report> {
    colors
        .iter()
        .take(7)
        .enumerate()
        .map(|(i, c)| Report::Feature([0x14, 0x00, (i + 1) as u8, c[0], c[1], c[2], 0x00, 0x00]))
        .collect()
}

fn pack_usermode(slots: &[Rgb], brightness: u8) -> Vec<Report> {
    let bright = brightness.min(KBD_BRIGHTNESS_MAX);
    let mut reps = Vec::with_capacity(10);
    reps.push(Report::Feature([0x08, KBD_POWER_ON, 0x33, 0x00, bright, 0x00, 0x00, 0x00]));
    reps.push(Report::Feature([0x12, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00]));
    for pkt_idx in 0..8 {
        let mut pkt = vec![0u8; 65];
        let base = pkt_idx * 16;
        for j in 0..16 {
            let [r, g, b] = slots.get(base + j).copied().unwrap_or([0, 0, 0]);
            let off = 1 + j * 4;
            pkt[off + 1] = r;
            pkt[off + 2] = g;
            pkt[off + 3] = b;
        }
        reps.push(Report::Output(pkt));
    }
    reps
}

#[derive(Clone, Copy)]
enum KeyGroup {
    Alpha,
    Wasd,
    Mod,
    Fn,
    Nav,
    Num,
    Arrows,
}

fn lerp(c1: Rgb, c2: Rgb, t: f64) -> Rgb {
    let t = t.clamp(0.0, 1.0);
    let f = |a: u8, b: u8| ((a as f64) + ((b as f64) - (a as f64)) * t).round_ties_even().clamp(0.0, 255.0) as u8;
    [f(c1[0], c2[0]), f(c1[1], c2[1]), f(c1[2], c2[2])]
}

fn hue_rgb(pos: f64) -> Rgb {
    let m = pos.rem_euclid(1.0) * 6.0;
    let i = (m.floor() as usize) % 6;
    let f = m - m.floor();
    let up = (255.0 * f).round_ties_even().clamp(0.0, 255.0) as u8;
    let down = (255.0 * (1.0 - f)).round_ties_even().clamp(0.0, 255.0) as u8;
    match i {
        0 => [255, up, 0],
        1 => [down, 255, 0],
        2 => [0, 255, up],
        3 => [0, down, 255],
        4 => [up, 0, 255],
        _ => [255, 0, down],
    }
}

fn keycap_groups() -> Vec<(KeyGroup, Vec<usize>)> {
    let mut caps = Vec::with_capacity(101);
    let mut push = |row: usize, col: usize, extra: &[usize], group: KeyGroup| {
        let mut indices = Vec::with_capacity(1 + extra.len());
        indices.push(row * KBD_MATRIX_COLS + col);
        indices.extend_from_slice(extra);
        caps.push((group, indices));
    };
    push(0, 0, &[], KeyGroup::Mod);
    for c in 1..=12 { push(0, c, &[], KeyGroup::Fn); }
    for c in 13..=19 { push(0, c, &[], KeyGroup::Nav); }
    for c in 0..13 { push(1, c, &[], KeyGroup::Alpha); }
    push(1, 14, &[], KeyGroup::Mod);
    for c in 15..=18 { push(1, c, &[], KeyGroup::Num); }
    push(2, 0, &[], KeyGroup::Mod);
    for i in 0..13 { push(2, 2 + i, &[], if i == 1 { KeyGroup::Wasd } else { KeyGroup::Alpha }); }
    for c in 15..=18 { push(2, c, &[], KeyGroup::Num); }
    push(3, 0, &[], KeyGroup::Mod);
    for i in 0..11 { push(3, 2 + i, &[], if i <= 2 { KeyGroup::Wasd } else { KeyGroup::Alpha }); }
    push(3, 14, &[], KeyGroup::Mod);
    for c in 15..=17 { push(3, c, &[], KeyGroup::Num); }
    push(4, 0, &[85], KeyGroup::Mod);
    for i in 0..10 { push(4, 3 + i, &[], KeyGroup::Alpha); }
    push(4, 14, &[], KeyGroup::Mod);
    push(4, 15, &[], KeyGroup::Arrows);
    for c in 16..=18 { push(4, c, &[], KeyGroup::Num); }
    push(5, 0, &[], KeyGroup::Mod);
    push(5, 2, &[], KeyGroup::Mod);
    push(5, 3, &[], KeyGroup::Mod);
    push(5, 4, &[], KeyGroup::Mod);
    push(5, 7, &[110, 111, 113, 114], KeyGroup::Mod);
    push(5, 10, &[], KeyGroup::Mod);
    push(5, 12, &[], KeyGroup::Mod);
    push(5, 13, &[], KeyGroup::Arrows);
    push(5, 14, &[], KeyGroup::Arrows);
    push(5, 15, &[], KeyGroup::Arrows);
    push(5, 16, &[], KeyGroup::Num);
    push(5, 17, &[], KeyGroup::Num);
    push(5, 18, &[], KeyGroup::Num);
    caps
}

pub fn preset_cells(name: &str, accent: Rgb) -> Result<Vec<Rgb>, String> {
    if !PRESET_NAMES.contains(&name) {
        return Err(format!("未知矩阵预设: {name}"));
    }
    let secondary: Rgb = [255, 0, 128];
    let mut cells = vec![[0u8, 0, 0]; KBD_MATRIX_CELLS];
    if name == "rainbow_grid" {
        for r in 0..KBD_MATRIX_ROWS {
            for c in 0..KBD_MATRIX_COLS {
                cells[r * KBD_MATRIX_COLS + c] = hue_rgb((c as f64) / 19.0 + (r as f64) * 0.04);
            }
        }
        return Ok(cells);
    }
    if name == "gradient" {
        for r in 0..KBD_MATRIX_ROWS {
            for c in 0..KBD_MATRIX_COLS {
                cells[r * KBD_MATRIX_COLS + c] = lerp(accent, secondary, (c as f64) / 18.0);
            }
        }
        return Ok(cells);
    }
    if name == "wasd_fps" {
        for &idx in &[0usize, 63, 77, 84, 85, 112] {
            cells[idx] = [20, 45, 70];
        }
    }
    for (group, indices) in keycap_groups() {
        let col = match name {
            "wasd_fps" => match group {
                KeyGroup::Wasd => [255, 40, 0],
                KeyGroup::Arrows => accent,
                _ => [0, 0, 0],
            },
            "gaming" => match group {
                KeyGroup::Wasd => [255, 32, 0],
                KeyGroup::Arrows => [255, 140, 0],
                KeyGroup::Fn => [110, 0, 255],
                KeyGroup::Nav => [0, 120, 255],
                KeyGroup::Mod => [0, 90, 255],
                KeyGroup::Num => [0, 220, 160],
                KeyGroup::Alpha => accent,
            },
            _ => match group {
                KeyGroup::Wasd | KeyGroup::Alpha => accent,
                KeyGroup::Fn => [255, 165, 0],
                KeyGroup::Nav => [255, 220, 0],
                KeyGroup::Mod => [110, 80, 255],
                KeyGroup::Arrows => [255, 60, 120],
                KeyGroup::Num => [0, 220, 140],
            },
        };
        for idx in indices {
            if idx < KBD_MATRIX_CELLS {
                cells[idx] = col;
            }
        }
    }
    Ok(cells)
}

pub fn parse_rgb(v: &Value) -> Result<Rgb, String> {
    let Some(arr) = v.as_array() else {
        return Err("颜色需要三个 RGB 数值".to_string());
    };
    if arr.len() != 3 {
        return Err("颜色需要三个 RGB 数值".to_string());
    }
    let mut out = [0u8; 3];
    for (i, item) in arr.iter().enumerate() {
        let Some(n) = item.as_i64() else {
            return Err("颜色通道需为 0..255 整数".to_string());
        };
        if !(0..=255).contains(&n) {
            return Err("颜色通道需为 0..255 整数".to_string());
        }
        out[i] = n as u8;
    }
    Ok(out)
}

pub fn build_reports(device: &str, params: &Value) -> Result<Vec<Report>, String> {
    let Some(obj) = params.as_object() else {
        return Err("灯光参数必须是对象".to_string());
    };
    let Some(mode) = obj.get("mode").and_then(|v| v.as_str()) else {
        return Err("缺少灯效模式".to_string());
    };
    let Some(spec) = find_spec(device, mode) else {
        return Err(format!("未知 {device} 灯效模式: {mode}"));
    };
    let bmax = if device == "keyboard" {
        KBD_BRIGHTNESS_MAX
    } else {
        LB_BRIGHTNESS_MAX
    };
    let brightness = match obj.get("brightness") {
        None | Some(Value::Null) => bmax,
        Some(v) => {
            let n = v.as_i64().ok_or("亮度需为整数")?;
            if !(0..=bmax as i64).contains(&n) {
                return Err(format!("亮度范围 0..{bmax}"));
            }
            n as u8
        }
    };
    let color = match obj.get("color") {
        None => [0, 255, 255],
        Some(v) => parse_rgb(v)?,
    };
    let colors: Vec<Rgb> = match obj.get("colors") {
        None | Some(Value::Null) => RAINBOW.to_vec(),
        Some(v) => {
            let arr = v.as_array().ok_or("调色板需为数组")?;
            if arr.is_empty() || arr.len() > 7 {
                return Err("调色板需包含 1..7 种颜色".to_string());
            }
            arr.iter().map(parse_rgb).collect::<Result<Vec<_>, _>>()?
        }
    };
    let speed = match obj.get("speed") {
        None => 5u8,
        Some(v) => {
            let n = v.as_i64().ok_or("速度需为整数")?;
            if !(SPEED_MIN as i64..=SPEED_MAX as i64).contains(&n) {
                return Err("速度需在 1..10 之间".to_string());
            }
            n as u8
        }
    };
    let max_dir = if device == "keyboard" { 4 } else { 2 };
    let direction = match obj.get("direction") {
        None => 1u8,
        Some(v) => {
            let n = v.as_i64().ok_or("方向需为整数")?;
            if !(1..=max_dir).contains(&n) {
                return Err(format!("方向需在 1..{max_dir} 之间"));
            }
            n as u8
        }
    };
    let reactive = obj.get("reactive").and_then(|v| v.as_bool()).unwrap_or(false);
    let calibrate_on = obj.get("calibrate").and_then(|v| v.as_bool()).unwrap_or(false);

    if spec.off || brightness == 0 {
        return if device == "keyboard" {
            Ok(vec![Report::Feature([0x09, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])])
        } else {
            Ok(vec![
                Report::Feature([0x14, 0x00, 0x01, 0, 0, 0, 0, 0]),
                Report::Feature([0x08, LB_SELECTOR, 0x01, 0x01, 0, 0x01, 0, 0]),
            ])
        };
    }

    if device == "lightbar" {
        let mut reps = vec![Report::Feature([0x1A, 0x00, 0x01, 0x04, 0x00, 0x00, 0x00, 0x01])];
        if spec.single {
            reps.push(Report::Feature([0x14, 0x00, 0x01, color[0], color[1], color[2], 0, 0]));
            reps.push(Report::Feature([0x08, LB_SELECTOR, 0x01, 0x01, brightness, 0x01, 0, 0]));
        } else {
            reps.extend(palette_reports(&colors));
            reps.push(Report::Feature([0x08, LB_SELECTOR, spec.code, speed, brightness, 0x08, direction, 0]));
        }
        return Ok(reps);
    }

    if spec.matrix {
        let cells: Vec<Rgb> = match obj.get("matrix") {
            Some(v) if !v.is_null() => {
                let arr = v.as_array().ok_or("按键矩阵需为数组")?;
                if arr.len() != KBD_MATRIX_CELLS {
                    return Err(format!("按键矩阵需包含 {KBD_MATRIX_CELLS} 项"));
                }
                arr.iter().map(parse_rgb).collect::<Result<Vec<_>, _>>()?
            }
            _ => {
                let preset = obj.get("preset").and_then(|v| v.as_str()).unwrap_or("gaming");
                preset_cells(preset, color)?
            }
        };
        let mut slots = vec![[0u8, 0, 0]; KBD_WIRE_SLOTS];
        for col in 0..KBD_MATRIX_COLS {
            for hw_row in 0..KBD_MATRIX_ROWS {
                let top_row = KBD_MATRIX_ROWS - 1 - hw_row;
                let mut rgb = cells[top_row * KBD_MATRIX_COLS + col];
                if calibrate_on {
                    rgb = calibrate(rgb);
                }
                slots[col * KBD_MATRIX_ROWS + hw_row] = rgb;
            }
        }
        return Ok(pack_usermode(&slots, brightness));
    }

    if spec.single {
        return Ok(pack_usermode(&vec![color; KBD_WIRE_SLOTS], brightness));
    }

    let behavior = if spec.trigger {
        u8::from(reactive)
    } else if let Some(fixed) = spec.fixed_direction {
        fixed
    } else if spec.direction {
        if let Some(dmap) = spec.direction_map {
            dmap[(direction as usize).saturating_sub(1).min(3)]
        } else {
            direction
        }
    } else {
        0
    };
    let mut reps = if spec.palette {
        palette_reports(&colors)
    } else {
        Vec::new()
    };
    reps.push(Report::Feature([0x08, KBD_POWER_ON, spec.code, speed, brightness, 0x08, behavior, 0]));
    Ok(reps)
}

// ----------------------------------------------------------------- hidraw I/O

const fn ioc(dir: u32, ty: u8, nr: u32, size: u32) -> u32 {
    (dir << 30) | (size << 16) | ((ty as u32) << 8) | nr
}
const fn hidiocsfeature(len: u32) -> u32 {
    ioc(3, b'H', 0x06, len)
}
const fn hidiocgfeature(len: u32) -> u32 {
    ioc(3, b'H', 0x07, len)
}

pub fn hidraw_for_label(label: &str) -> Option<String> {
    let led_name = match label {
        "keyboard" => "rgb:kbd_backlight",
        "lightbar" => "rgb:lightbar",
        _ => return None,
    };
    let base = format!("/sys/class/leds/{led_name}/device/hidraw");
    if let Ok(mut entries) = fs::read_dir(&base) {
        if let Some(Ok(entry)) = entries.next() {
            if let Some(name) = entry.file_name().to_str() {
                return Some(format!("/dev/{name}"));
            }
        }
    }
    let board = fs::read_to_string("/sys/class/dmi/id/board_name").ok()?;
    if board.trim() != "YAOSHI Series-XxAF55xx" {
        return None;
    }
    let expected = match label {
        "keyboard" => "0003:0000048D:0000600B",
        "lightbar" => "0003:0000048D:00007001",
        _ => return None,
    };
    let mut nodes: Vec<_> = fs::read_dir("/sys/class/hidraw").ok()?.flatten().collect();
    nodes.sort_by_key(|e| e.file_name());
    for entry in nodes {
        let Ok(text) = fs::read_to_string(entry.path().join("device").join("uevent")) else {
            continue;
        };
        for line in text.lines() {
            if let Some((k, v)) = line.split_once('=') {
                if k == "HID_ID" && v == expected {
                    if let Some(name) = entry.file_name().to_str() {
                        return Some(format!("/dev/{name}"));
                    }
                }
            }
        }
    }
    None
}

pub fn available_hid_devices() -> Vec<String> {
    ["keyboard", "lightbar"]
        .into_iter()
        .filter(|d| hidraw_for_label(d).is_some())
        .map(String::from)
        .collect()
}

fn open_hidraw(label: &str) -> Result<File, String> {
    let path = hidraw_for_label(label)
        .ok_or_else(|| format!("未检测到 {label} 的 HID 设备节点，请确认 udev 规则已生效"))?;
    OpenOptions::new()
        .read(true)
        .write(true)
        .open(&path)
        .map_err(|e| format!("无法打开 {path}: {e}"))
}

fn send_reports(file: &mut File, reports: &[Report]) -> Result<(), String> {
    let fd = file.as_raw_fd();
    for rep in reports {
        match rep {
            Report::Feature(data) => {
                let mut buf = *data;
                let req = hidiocsfeature(buf.len() as u32);
                if unsafe { libc::ioctl(fd, req as libc::Ioctl, buf.as_mut_ptr()) } < 0 {
                    return Err(format!("HID Feature 写入失败: {}", std::io::Error::last_os_error()));
                }
            }
            Report::Output(data) => {
                file.write_all(data)
                    .map_err(|e| format!("HID Output 写入失败: {e}"))?;
            }
        }
    }
    Ok(())
}

pub fn decode_keyboard_mode(power: u8, code: u8, brightness: u8, hw_dir: u8, saved_mode: Option<&str>) -> (&'static str, u8) {
    if power == 1 || brightness == 0 {
        return ("off", 1);
    }
    if code == 0x03 {
        if hw_dir == 16 || hw_dir == 17 {
            return ("sine", if hw_dir == 16 { 1 } else { 2 });
        }
        if hw_dir == 19 {
            return ("interlace", 1);
        }
        if hw_dir == 32 || hw_dir == 33 {
            return ("diagonal", if hw_dir == 33 { 1 } else { 2 });
        }
        return ("wave", if (1..=4).contains(&hw_dir) { hw_dir } else { 1 });
    }
    if code == 0x33 {
        return (if saved_mode == Some("matrix") { "matrix" } else { "solid" }, 1);
    }
    let fallback = saved_mode
        .and_then(|m| KEYBOARD_MODES.iter().find(|s| s.name == m).map(|s| s.name))
        .unwrap_or("rainbow");
    let name = KEYBOARD_MODES
        .iter()
        .find(|s| s.code == code)
        .map(|s| s.name)
        .unwrap_or(fallback);
    (name, if (1..=4).contains(&hw_dir) { hw_dir } else { 1 })
}

pub fn query_keyboard_hw(saved_mode: Option<&str>) -> Result<KeyboardHwInfo, String> {
    let _guard = ZoneGuard::acquire("keyboard")?;
    query_keyboard_hw_locked(saved_mode)
}

fn query_keyboard_hw_locked(saved_mode: Option<&str>) -> Result<KeyboardHwInfo, String> {
    let mut file = open_hidraw("keyboard")?;
    send_reports(
        &mut file,
        &[Report::Feature([QUERY_EFFECT, 0, 0, 0, 0, 0, 0, 0])],
    )?;
    let mut buf = [0u8; 9];
    let req = hidiocgfeature(buf.len() as u32);
    if unsafe { libc::ioctl(file.as_raw_fd(), req as libc::Ioctl, buf.as_mut_ptr()) } < 0 {
        return Err(format!("读取键盘硬件状态失败: {}", std::io::Error::last_os_error()));
    }
    let power = buf[2];
    let mode = buf[3];
    let speed = buf[4];
    let brightness = buf[5];
    let direction = buf[7];
    let (mode_name, ui_direction) = decode_keyboard_mode(power, mode, brightness, direction, saved_mode);
    Ok(KeyboardHwInfo {
        power,
        mode,
        mode_name: mode_name.to_string(),
        speed,
        brightness,
        direction,
        ui_direction,
    })
}

pub fn apply_and_save_hid(device: &str, params: &Value, save_manual: bool) -> Result<Option<KeyboardHwInfo>, String> {
    let reports = build_reports(device, params)?;
    let _guard = ZoneGuard::acquire(device)?;
    let mut file = open_hidraw(device)?;
    send_reports(&mut file, &reports)?;

    let hw = if device == "keyboard" {
        std::thread::sleep(Duration::from_millis(15));
        let sm = params.get("mode").and_then(|v| v.as_str());
        query_keyboard_hw_locked(sm).ok()
    } else {
        None
    };

    if save_manual {
        let _sguard = ZoneGuard::acquire("manual-state")?;
        let path = state_dir().join("state.json");
        let mut root = match read_user_json(&path, MAX_STATE_BYTES).ok().flatten() {
            Some(Value::Object(m)) => m,
            _ => Map::new(),
        };
        root.insert(device.to_string(), params.clone());
        write_user_json(&path, &Value::Object(root))?;
    }
    Ok(hw)
}

pub fn load_saved_hid(device: &str) -> Value {
    let path = state_dir().join("state.json");
    match read_user_json(&path, MAX_STATE_BYTES).ok().flatten() {
        Some(Value::Object(map)) => map.get(device).cloned().unwrap_or(json!({})),
        _ => json!({}),
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn keyboard_off_and_solid_reports() {
        let off = build_reports("keyboard", &json!({"mode": "off"})).unwrap();
        assert_eq!(off.len(), 1);
        match &off[0] {
            Report::Feature(b) => assert_eq!(*b, [0x09, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]),
            _ => panic!("expected feature report"),
        }

        let solid = build_reports(
            "keyboard",
            &json!({"mode": "solid", "color": [0, 255, 255], "brightness": 50}),
        )
        .unwrap();
        assert_eq!(solid.len(), 10);
        match &solid[0] {
            Report::Feature(b) => assert_eq!(*b, [0x08, 0x02, 0x33, 0x00, 50, 0x00, 0x00, 0x00]),
            _ => panic!("expected usermode header"),
        }
        match &solid[2] {
            Report::Output(pkt) => {
                assert_eq!(pkt.len(), 65);
                assert_eq!(&pkt[1..5], &[0, 0, 255, 255]);
            }
            _ => panic!("expected output packet"),
        }
    }

    #[test]
    fn keyboard_matrix_presets_and_calibration() {
        for &p in PRESET_NAMES {
            let cells = preset_cells(p, [0, 255, 255]).unwrap();
            assert_eq!(cells.len(), KBD_MATRIX_CELLS);
        }
        let reps = build_reports(
            "keyboard",
            &json!({"mode": "matrix", "preset": "wasd_fps", "color": [255, 255, 255], "brightness": 40, "calibrate": true}),
        )
        .unwrap();
        assert_eq!(reps.len(), 10);
    }

    #[test]
    fn lightbar_static_and_off_reports() {
        let reps = build_reports(
            "lightbar",
            &json!({"mode": "static", "color": [10, 20, 30], "brightness": 80}),
        )
        .unwrap();
        assert_eq!(reps.len(), 3);
        match &reps[0] {
            Report::Feature(b) => assert_eq!(*b, [0x1A, 0x00, 0x01, 0x04, 0x00, 0x00, 0x00, 0x01]),
            _ => panic!("expected lightbar power-on"),
        }
        let off = build_reports("lightbar", &json!({"mode": "off"})).unwrap();
        assert_eq!(off.len(), 2);
    }

    #[test]
    fn decode_keyboard_submodes_matches_protocol() {
        assert_eq!(decode_keyboard_mode(1, 0x05, 50, 1, None), ("off", 1));
        assert_eq!(decode_keyboard_mode(2, 0x03, 50, 16, None), ("sine", 1));
        assert_eq!(decode_keyboard_mode(2, 0x03, 50, 17, None), ("sine", 2));
        assert_eq!(decode_keyboard_mode(2, 0x03, 50, 19, None), ("interlace", 1));
        assert_eq!(decode_keyboard_mode(2, 0x03, 50, 33, None), ("diagonal", 1));
        assert_eq!(decode_keyboard_mode(2, 0x03, 50, 32, None), ("diagonal", 2));
        assert_eq!(decode_keyboard_mode(2, 0x33, 50, 0, Some("matrix")), ("matrix", 1));
        assert_eq!(decode_keyboard_mode(2, 0x33, 50, 0, Some("solid")), ("solid", 1));
    }
}

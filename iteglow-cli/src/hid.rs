// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Native ITE 8291/8233 USB HID protocol, 101-key matrix presets & parser, and hidraw I/O.

use crate::zone_lock::{read_json_file, state_dir, write_json_file, ZoneGuard, MAX_STATE_BYTES};
use serde_json::{Map, Value};
use std::collections::HashMap;
use std::fs::{self, File, OpenOptions};
use std::io::Write;
use std::os::unix::io::AsRawFd;
use std::path::Path;

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
pub const QUERY_FW_VERSION: u8 = 0x80;
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

pub const PRESET_NAMES: &[&str] = &["gaming", "wasd_fps", "zones", "rainbow_grid", "gradient"];

pub const KNOWN_DEVICES: &[(&str, &str, &str)] = &[
    ("rgb:kbd_backlight", "048d:600b", "keyboard"),
    ("rgb:lightbar", "048d:7001", "lightbar"),
];

#[derive(Debug, Clone)]
pub enum Report {
    Feature([u8; 8]),
    Output(Vec<u8>),
}

#[derive(Debug, Clone, Copy)]
pub struct KeyboardHwInfo {
    pub power: u8,
    pub mode: u8,
    #[allow(dead_code)]
    pub speed: u8,
    pub brightness: u8,
    pub direction: u8,
}

#[derive(Debug, Clone, Copy)]
pub struct ModeSpec {
    pub name: &'static str,
    pub code: u8,
    pub off: bool,
    pub single: bool,
    pub matrix: bool,
    pub palette: bool,
    pub direction: bool,
    pub trigger: bool,
    pub fixed_direction: Option<u8>,
    pub direction_map: Option<[u8; 4]>,
}

pub const LIGHTBAR_MODES: &[ModeSpec] = &[
    ModeSpec { name: "off", code: 0x00, off: true, single: false, matrix: false, palette: false, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "static", code: 0x01, off: false, single: true, matrix: false, palette: false, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "breathing", code: 0x02, off: false, single: false, matrix: false, palette: true, direction: false, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "wave", code: 0x03, off: false, single: false, matrix: false, palette: false, direction: true, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "marquee", code: 0x04, off: false, single: false, matrix: false, palette: true, direction: true, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "neon", code: 0x05, off: false, single: false, matrix: false, palette: true, direction: true, trigger: false, fixed_direction: None, direction_map: None },
    ModeSpec { name: "runner", code: 0x06, off: false, single: false, matrix: false, palette: true, direction: true, trigger: false, fixed_direction: None, direction_map: None },
];

pub const KEYBOARD_MODES: &[ModeSpec] = &[
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

pub fn find_spec(device: &str, mode: &str) -> Option<&'static ModeSpec> {
    let list = match device {
        "keyboard" => KEYBOARD_MODES,
        "lightbar" => LIGHTBAR_MODES,
        _ => return None,
    };
    list.iter().find(|s| s.name == mode)
}

pub fn parse_color_str(s: &str) -> Result<Rgb, String> {
    let t = s.trim().trim_start_matches('#');
    if t.len() == 6 && t.bytes().all(|b| b.is_ascii_hexdigit()) {
        let r = u8::from_str_radix(&t[0..2], 16).map_err(|_| format!("bad colour {s}"))?;
        let g = u8::from_str_radix(&t[2..4], 16).map_err(|_| format!("bad colour {s}"))?;
        let b = u8::from_str_radix(&t[4..6], 16).map_err(|_| format!("bad colour {s}"))?;
        return Ok([r, g, b]);
    }
    let parts: Vec<&str> = t.split(',').collect();
    if parts.len() == 3 {
        let mut out = [0u8; 3];
        for (i, p) in parts.iter().enumerate() {
            out[i] = p
                .trim()
                .parse::<u8>()
                .map_err(|_| format!("bad colour {s}; use #RRGGBB or r,g,b"))?;
        }
        return Ok(out);
    }
    Err(format!("bad colour {s}; use #RRGGBB or r,g,b"))
}

pub fn parse_rgb_value(v: &Value) -> Result<Rgb, String> {
    if let Some(s) = v.as_str() {
        return parse_color_str(s);
    }
    let Some(arr) = v.as_array() else {
        return Err("expected RGB array or color string".to_string());
    };
    if arr.len() != 3 {
        return Err("expected 3 RGB channels".to_string());
    }
    let mut out = [0u8; 3];
    for (i, item) in arr.iter().enumerate() {
        let n = item
            .as_i64()
            .filter(|n| (0..=255).contains(n))
            .ok_or("RGB channel must be 0..255")?;
        out[i] = n as u8;
    }
    Ok(out)
}

pub fn resolve_direction(device: &str, value: &str) -> Result<u8, String> {
    let named = match device {
        "lightbar" => match value {
            "forward" => Some(1),
            "reverse" => Some(2),
            _ => None,
        },
        "keyboard" => match value {
            "ltr" => Some(1),
            "rtl" => Some(2),
            "btt" => Some(3),
            "ttb" => Some(4),
            _ => None,
        },
        _ => None,
    };
    if let Some(n) = named {
        return Ok(n);
    }
    value
        .parse::<u8>()
        .map_err(|_| format!("bad direction {value}"))
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

fn keycap_defs() -> Vec<(&'static str, KeyGroup, Vec<usize>)> {
    let mut caps = Vec::with_capacity(101);
    let mut push = |name: &'static str, row: usize, col: usize, extra: &[usize], group: KeyGroup| {
        let mut indices = Vec::with_capacity(1 + extra.len());
        indices.push(row * KBD_MATRIX_COLS + col);
        indices.extend_from_slice(extra);
        caps.push((name, group, indices));
    };
    push("Esc", 0, 0, &[], KeyGroup::Mod);
    for (c, lbl) in ["F1","F2","F3","F4","F5","F6","F7","F8","F9","F10","F11","F12"].into_iter().enumerate() {
        push(lbl, 0, c + 1, &[], KeyGroup::Fn);
    }
    for (i, lbl) in ["PrtSc","Pause","Del","Home","PgUp","PgDn","End"].into_iter().enumerate() {
        push(lbl, 0, 13 + i, &[], KeyGroup::Nav);
    }
    for (c, lbl) in ["`","1","2","3","4","5","6","7","8","9","0","-","="].into_iter().enumerate() {
        push(lbl, 1, c, &[], KeyGroup::Alpha);
    }
    push("Bksp", 1, 14, &[], KeyGroup::Mod);
    for (i, lbl) in ["NumLk","/","*","-"].into_iter().enumerate() {
        push(lbl, 1, 15 + i, &[], KeyGroup::Num);
    }
    push("Tab", 2, 0, &[], KeyGroup::Mod);
    for (i, lbl) in ["Q","W","E","R","T","Y","U","I","O","P","[","]","\\"].into_iter().enumerate() {
        push(lbl, 2, 2 + i, &[], if lbl == "W" { KeyGroup::Wasd } else { KeyGroup::Alpha });
    }
    for (i, lbl) in ["Num7","Num8","Num9","+"].into_iter().enumerate() {
        push(lbl, 2, 15 + i, &[], KeyGroup::Num);
    }
    push("Caps", 3, 0, &[], KeyGroup::Mod);
    for (i, lbl) in ["A","S","D","F","G","H","J","K","L",";","'"].into_iter().enumerate() {
        let g = if matches!(lbl, "A" | "S" | "D") { KeyGroup::Wasd } else { KeyGroup::Alpha };
        push(lbl, 3, 2 + i, &[], g);
    }
    push("Enter", 3, 14, &[], KeyGroup::Mod);
    for (i, lbl) in ["Num4","Num5","Num6"].into_iter().enumerate() {
        push(lbl, 3, 15 + i, &[], KeyGroup::Num);
    }
    push("LShift", 4, 0, &[85], KeyGroup::Mod);
    for (i, lbl) in ["Z","X","C","V","B","N","M",",",".","/"].into_iter().enumerate() {
        push(lbl, 4, 3 + i, &[], KeyGroup::Alpha);
    }
    push("RShift", 4, 14, &[], KeyGroup::Mod);
    push("↑", 4, 15, &[], KeyGroup::Arrows);
    for (i, lbl) in ["Num1","Num2","Num3"].into_iter().enumerate() {
        push(lbl, 4, 16 + i, &[], KeyGroup::Num);
    }
    push("LCtrl", 5, 0, &[], KeyGroup::Mod);
    push("Fn", 5, 2, &[], KeyGroup::Mod);
    push("Win", 5, 3, &[], KeyGroup::Mod);
    push("LAlt", 5, 4, &[], KeyGroup::Mod);
    push("Space", 5, 7, &[110, 111, 113, 114], KeyGroup::Mod);
    push("RAlt", 5, 10, &[], KeyGroup::Mod);
    push("RCtrl", 5, 12, &[], KeyGroup::Mod);
    push("←", 5, 13, &[], KeyGroup::Arrows);
    push("↓", 5, 14, &[], KeyGroup::Arrows);
    push("→", 5, 15, &[], KeyGroup::Arrows);
    push("Num0", 5, 16, &[], KeyGroup::Num);
    push("Num.", 5, 17, &[], KeyGroup::Num);
    push("NumEnt", 5, 18, &[], KeyGroup::Num);
    caps
}

pub fn parse_matrix_spec(spec: &str) -> Result<Vec<Rgb>, String> {
    let text = spec.trim().strip_prefix('@').unwrap_or(spec.trim());
    let raw = if Path::new(text).is_file() {
        fs::read_to_string(text).map_err(|e| format!("failed to read {text}: {e}"))?
    } else {
        text.to_string()
    };
    let data: Value = serde_json::from_str(&raw)
        .map_err(|e| format!("invalid matrix JSON or file {spec}: {e}"))?;
    if let Some(obj) = data.as_object() {
        let mut cells = vec![[0u8, 0, 0]; KBD_MATRIX_CELLS];
        let mut by_name: HashMap<String, Vec<usize>> = HashMap::new();
        for (name, _, indices) in keycap_defs() {
            by_name.insert(name.to_lowercase(), indices);
        }
        for (k, v) in obj {
            let key_low = k.trim().to_lowercase();
            let Some(indices) = by_name.get(&key_low) else {
                return Err(format!("unknown key name {k:?} in matrix map"));
            };
            let col = parse_rgb_value(v)?;
            for &idx in indices {
                if idx < KBD_MATRIX_CELLS {
                    cells[idx] = col;
                }
            }
        }
        return Ok(cells);
    }
    if let Some(arr) = data.as_array() {
        if arr.len() == KBD_MATRIX_ROWS
            && arr
                .iter()
                .all(|r| r.as_array().map(|a| a.len() == KBD_MATRIX_COLS).unwrap_or(false))
        {
            let mut out = Vec::with_capacity(KBD_MATRIX_CELLS);
            for row in arr {
                for cell in row.as_array().unwrap() {
                    out.push(parse_rgb_value(cell)?);
                }
            }
            return Ok(out);
        }
        if arr.len() == KBD_MATRIX_CELLS {
            return arr.iter().map(parse_rgb_value).collect();
        }
    }
    Err("matrix must be 126 cells, 6x21 rows, or {key: color} JSON".to_string())
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

pub fn preset_cells(name: &str, accent: Rgb) -> Result<Vec<Rgb>, String> {
    if !PRESET_NAMES.contains(&name) {
        return Err(format!("unknown matrix preset {name:?}"));
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
    for (_, group, indices) in keycap_defs() {
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

pub fn build_reports(device: &str, params: &Value) -> Result<Vec<Report>, String> {
    let Some(obj) = params.as_object() else {
        return Err("lighting parameters must be an object".to_string());
    };
    let Some(mode) = obj.get("mode").and_then(|v| v.as_str()) else {
        return Err("missing lighting mode".to_string());
    };
    let Some(spec) = find_spec(device, mode) else {
        return Err(format!("unknown {device} mode: {mode}"));
    };
    let bmax = if device == "keyboard" {
        KBD_BRIGHTNESS_MAX
    } else {
        LB_BRIGHTNESS_MAX
    };
    let brightness = match obj.get("brightness") {
        None | Some(Value::Null) => bmax,
        Some(v) => {
            let n = v.as_i64().ok_or("brightness must be integer")?;
            if !(0..=bmax as i64).contains(&n) {
                return Err(format!("brightness out of range 0..{bmax}"));
            }
            n as u8
        }
    };
    let color = match obj.get("color") {
        None | Some(Value::Null) => [0, 255, 255],
        Some(v) => parse_rgb_value(v)?,
    };
    let colors: Vec<Rgb> = match obj.get("colors") {
        None | Some(Value::Null) => RAINBOW.to_vec(),
        Some(v) => {
            let arr = v.as_array().ok_or("colors must be array")?;
            if arr.is_empty() || arr.len() > 7 {
                return Err("colors must contain 1..7 RGB entries".to_string());
            }
            arr.iter().map(parse_rgb_value).collect::<Result<Vec<_>, _>>()?
        }
    };
    let speed = match obj.get("speed") {
        None | Some(Value::Null) => 5u8,
        Some(v) => {
            let n = v.as_i64().ok_or("speed must be integer")?;
            if !(SPEED_MIN as i64..=SPEED_MAX as i64).contains(&n) {
                return Err("speed must be 1..10".to_string());
            }
            n as u8
        }
    };
    let max_dir = if device == "keyboard" { 4 } else { 2 };
    let direction = match obj.get("direction") {
        None | Some(Value::Null) => 1u8,
        Some(v) => {
            let n = v.as_i64().ok_or("direction must be integer")?;
            if !(1..=max_dir).contains(&n) {
                return Err(format!("direction must be 1..{max_dir}"));
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
                let arr = v.as_array().ok_or("matrix must be array")?;
                if arr.len() != KBD_MATRIX_CELLS {
                    return Err(format!("matrix must have {KBD_MATRIX_CELLS} entries"));
                }
                arr.iter().map(parse_rgb_value).collect::<Result<Vec<_>, _>>()?
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

pub fn available_devices() -> Vec<&'static str> {
    ["keyboard", "lightbar"]
        .into_iter()
        .filter(|d| hidraw_for_label(d).is_some())
        .collect()
}

pub fn open_hidraw(label: &str) -> Result<File, String> {
    let path = hidraw_for_label(label)
        .ok_or_else(|| format!("no hidraw device found for {label}"))?;
    OpenOptions::new()
        .read(true)
        .write(true)
        .open(&path)
        .map_err(|e| format!("cannot open {path}: {e}"))
}

pub fn send_reports(file: &mut File, reports: &[Report]) -> Result<(), String> {
    let fd = file.as_raw_fd();
    for rep in reports {
        match rep {
            Report::Feature(data) => {
                let mut buf = *data;
                let req = hidiocsfeature(buf.len() as u32);
                if unsafe { libc::ioctl(fd, req as libc::Ioctl, buf.as_mut_ptr()) } < 0 {
                    return Err(format!("HIDIOCSFEATURE failed: {}", std::io::Error::last_os_error()));
                }
            }
            Report::Output(data) => {
                file.write_all(data)
                    .map_err(|e| format!("hidraw output write failed: {e}"))?;
            }
        }
    }
    Ok(())
}

pub fn query_raw(file: &mut File, cmd: u8) -> Result<[u8; 8], String> {
    send_reports(file, &[Report::Feature([cmd, 0, 0, 0, 0, 0, 0, 0])])?;
    let mut buf = [0u8; 9];
    let req = hidiocgfeature(buf.len() as u32);
    if unsafe { libc::ioctl(file.as_raw_fd(), req as libc::Ioctl, buf.as_mut_ptr()) } < 0 {
        return Err(format!("HIDIOCGFEATURE failed: {}", std::io::Error::last_os_error()));
    }
    let mut out = [0u8; 8];
    out.copy_from_slice(&buf[1..9]);
    Ok(out)
}

pub fn parse_fw_version(reply: &[u8; 8]) -> (u8, u8) {
    (reply[1], reply[2])
}

pub fn parse_keyboard_state(reply: &[u8; 8]) -> KeyboardHwInfo {
    KeyboardHwInfo {
        power: reply[1],
        mode: reply[2],
        speed: reply[3],
        brightness: reply[4],
        direction: reply[6],
    }
}

pub fn decode_keyboard_mode(hw: KeyboardHwInfo, saved_mode: Option<&str>) -> (&'static str, u8) {
    if hw.power == 1 || hw.brightness == 0 {
        return ("off", 1);
    }
    if hw.mode == 0x03 {
        if hw.direction == 16 || hw.direction == 17 {
            return ("sine", if hw.direction == 16 { 1 } else { 2 });
        }
        if hw.direction == 19 {
            return ("interlace", 1);
        }
        if hw.direction == 32 || hw.direction == 33 {
            return ("diagonal", if hw.direction == 33 { 1 } else { 2 });
        }
        return ("wave", if (1..=4).contains(&hw.direction) { hw.direction } else { 1 });
    }
    if hw.mode == 0x33 {
        return (if saved_mode == Some("matrix") { "matrix" } else { "solid" }, 1);
    }
    let fallback = saved_mode
        .and_then(|m| KEYBOARD_MODES.iter().find(|s| s.name == m).map(|s| s.name))
        .unwrap_or("rainbow");
    let name = KEYBOARD_MODES
        .iter()
        .find(|s| s.code == hw.mode)
        .map(|s| s.name)
        .unwrap_or(fallback);
    (name, if (1..=4).contains(&hw.direction) { hw.direction } else { 1 })
}

pub fn apply_and_save_hid(device: &str, params: &Value, save_manual: bool) -> Result<(), String> {
    let reports = build_reports(device, params)?;
    let _guard = ZoneGuard::acquire(device)?;
    let mut file = open_hidraw(device)?;
    send_reports(&mut file, &reports)?;
    if save_manual {
        let _sguard = ZoneGuard::acquire("manual-state")?;
        let path = state_dir().join("state.json");
        let mut root = match read_json_file(&path, MAX_STATE_BYTES, true).ok().flatten() {
            Some(Value::Object(m)) => m,
            _ => Map::new(),
        };
        root.insert(device.to_string(), params.clone());
        write_json_file(&path, &Value::Object(root), 0o600)?;
    }
    Ok(())
}

// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Pure ITE 8291/8233 HID lighting protocol, 101-key per-key matrix presets,
//! saved state validation, and Linux hidraw + Logo helper execution backends.
//!
//! Byte-for-byte compatible with `iteglow.{protocol,effects,keymap,devices,lighting_session}`.

use crate::zone_lock::{read_saved_json, ZoneGuard, MAX_STATE_BYTES};
use serde_json::{json, Map, Value};
use std::fs::{self, File, OpenOptions};
use std::io::Write;
use std::os::unix::io::AsRawFd;
use std::path::Path;
use std::process::Stdio;
use std::time::Duration;
use tokio::process::Command;

pub const HELPER_PATH: &str = "/usr/local/libexec/mechrevo-profile-helper";
pub const PKEXEC_PATH: &str = "/usr/bin/pkexec";

pub const KBD_BRIGHTNESS_MAX: u8 = 0x32; // 50
pub const KBD_MATRIX_ROWS: usize = 6;
pub const KBD_MATRIX_COLS: usize = 21;
pub const KBD_MATRIX_CELLS: usize = KBD_MATRIX_ROWS * KBD_MATRIX_COLS; // 126
pub const KBD_WIRE_SLOTS: usize = 128;
pub const LB_BRIGHTNESS_MAX: u8 = 0x64; // 100
pub const SPEED_MIN: u8 = 0x01;
pub const SPEED_MAX: u8 = 0x0A;

pub const LB_SELECTOR: u8 = 0x22;
pub const KBD_POWER_ON: u8 = 0x02;
pub const QUERY_EFFECT: u8 = 0x88;

pub type Rgb = (u8, u8, u8);

pub const RAINBOW: [Rgb; 7] = [
    (255, 0, 0),
    (255, 165, 0),
    (255, 255, 0),
    (0, 255, 0),
    (0, 0, 255),
    (0, 255, 255),
    (139, 0, 255),
];

pub const LOGO_COLORS: [Rgb; 7] = [
    (255, 255, 255),
    (255, 0, 0),
    (0, 255, 0),
    (0, 0, 255),
    (255, 255, 0),
    (136, 0, 255),
    (0, 255, 255),
];

pub const PRESET_NAMES: &[&str] = &["gaming", "wasd_fps", "zones", "rainbow_grid", "gradient"];

#[derive(Debug, Clone, PartialEq, Eq)]
pub enum Report {
    Feature([u8; 8]),
    Output(Vec<u8>),
    #[allow(dead_code)]
    Sleep(u8),
}

#[derive(Debug, Clone, PartialEq, Eq)]
pub struct RestoreAction {
    pub device: String,
    pub reports: Vec<Report>,
    pub logo_json: Option<String>,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub struct KeyboardHwState {
    pub power: u8,
    pub mode: u8,
    pub speed: u8,
    pub brightness: u8,
    pub direction: u8,
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
    direction_map: Option<[u8; 4]>, // maps direction 1..=4 -> byte
}

const LIGHTBAR_MODES: &[ModeSpec] = &[
    ModeSpec {
        name: "off",
        code: 0x00,
        off: true,
        single: false,
        matrix: false,
        palette: false,
        direction: false,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "static",
        code: 0x01,
        off: false,
        single: true,
        matrix: false,
        palette: false,
        direction: false,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "breathing",
        code: 0x02,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: false,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "wave",
        code: 0x03,
        off: false,
        single: false,
        matrix: false,
        palette: false,
        direction: true,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "marquee",
        code: 0x04,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: true,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "neon",
        code: 0x05,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: true,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "runner",
        code: 0x06,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: true,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
];

const KEYBOARD_MODES: &[ModeSpec] = &[
    ModeSpec {
        name: "off",
        code: 0x00,
        off: true,
        single: false,
        matrix: false,
        palette: false,
        direction: false,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "solid",
        code: 0x33,
        off: false,
        single: true,
        matrix: false,
        palette: false,
        direction: false,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "matrix",
        code: 0x33,
        off: false,
        single: false,
        matrix: true,
        palette: false,
        direction: false,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "breathing",
        code: 0x02,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: false,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "wave",
        code: 0x03,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: true,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "sine",
        code: 0x03,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: true,
        trigger: false,
        fixed_direction: None,
        direction_map: Some([16, 17, 16, 17]),
    },
    ModeSpec {
        name: "interlace",
        code: 0x03,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: false,
        trigger: false,
        fixed_direction: Some(19),
        direction_map: None,
    },
    ModeSpec {
        name: "diagonal",
        code: 0x03,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: true,
        trigger: false,
        fixed_direction: None,
        direction_map: Some([33, 32, 33, 32]),
    },
    ModeSpec {
        name: "reactive",
        code: 0x04,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: false,
        trigger: true,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "rainbow",
        code: 0x05,
        off: false,
        single: false,
        matrix: false,
        palette: false,
        direction: false,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "ripple",
        code: 0x06,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: false,
        trigger: true,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "marquee",
        code: 0x09,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: false,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "raindrop",
        code: 0x0A,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: false,
        trigger: false,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "aurora",
        code: 0x0E,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: false,
        trigger: true,
        fixed_direction: None,
        direction_map: None,
    },
    ModeSpec {
        name: "spark",
        code: 0x11,
        off: false,
        single: false,
        matrix: false,
        palette: true,
        direction: false,
        trigger: true,
        fixed_direction: None,
        direction_map: None,
    },
];

fn find_mode_spec(device: &str, mode: &str) -> Option<&'static ModeSpec> {
    let table = match device {
        "keyboard" => KEYBOARD_MODES,
        "lightbar" => LIGHTBAR_MODES,
        _ => return None,
    };
    table.iter().find(|spec| spec.name == mode)
}

pub fn is_known_mode(device: &str, mode: &str) -> bool {
    find_mode_spec(device, mode).is_some()
}

pub fn query_report(command: u8) -> Report {
    Report::Feature([command, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])
}

pub fn parse_keyboard_state(reply: &[u8]) -> Option<KeyboardHwState> {
    if reply.len() < 7 {
        return None;
    }
    Some(KeyboardHwState {
        power: reply[1],
        mode: reply[2],
        speed: reply[3],
        brightness: reply[4],
        direction: reply[6],
    })
}

pub fn decode_keyboard_state(
    actual: KeyboardHwState,
    saved_mode: Option<&str>,
) -> (&'static str, u8) {
    if actual.power == 1 || actual.brightness == 0 {
        return ("off", 1);
    }
    let code = actual.mode;
    let hw_dir = actual.direction;
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
        return (
            if saved_mode == Some("matrix") {
                "matrix"
            } else {
                "solid"
            },
            1,
        );
    }
    let fallback: &'static str = saved_mode
        .and_then(|m| KEYBOARD_MODES.iter().find(|spec| spec.name == m).map(|s| s.name))
        .unwrap_or("rainbow");
    let name = KEYBOARD_MODES
        .iter()
        .find(|spec| spec.code == code)
        .map(|s| s.name)
        .unwrap_or(fallback);
    (name, if (1..=4).contains(&hw_dir) { hw_dir } else { 1 })
}

pub fn palette(colors: &[Rgb]) -> Vec<Report> {
    colors
        .iter()
        .take(7)
        .enumerate()
        .map(|(i, &(r, g, b))| Report::Feature([0x14, 0x00, (i + 1) as u8, r, g, b, 0x00, 0x00]))
        .collect()
}

pub fn lightbar_static(r: u8, g: u8, b: u8, brightness: u8) -> Vec<Report> {
    let bright = brightness.min(LB_BRIGHTNESS_MAX);
    vec![
        Report::Feature([0x14, 0x00, 0x01, r, g, b, 0x00, 0x00]),
        Report::Feature([0x08, LB_SELECTOR, 0x01, 0x01, bright, 0x01, 0x00, 0x00]),
    ]
}

pub fn lightbar_effect(
    effect_id: u8,
    colors: &[Rgb],
    brightness: u8,
    speed: u8,
    direction: u8,
) -> Vec<Report> {
    let bright = brightness.min(LB_BRIGHTNESS_MAX);
    let spd = speed.clamp(SPEED_MIN, SPEED_MAX);
    let mut reps = palette(colors);
    reps.push(Report::Feature([
        0x08,
        LB_SELECTOR,
        effect_id,
        spd,
        bright,
        0x08,
        direction,
        0x00,
    ]));
    reps
}

pub fn lightbar_off() -> Vec<Report> {
    lightbar_static(0, 0, 0, 0)
}

pub fn lightbar_power_on() -> Report {
    Report::Feature([0x1A, 0x00, 0x01, 0x04, 0x00, 0x00, 0x00, 0x01])
}

pub fn keyboard_brightness(brightness: u8) -> Vec<Report> {
    let bright = brightness.min(KBD_BRIGHTNESS_MAX);
    vec![Report::Feature([0x09, 0x02, bright, 0x00, 0x00, 0x00, 0x00, 0x00])]
}

pub fn keyboard_effect(
    mode: u8,
    colors: &[Rgb],
    brightness: u8,
    speed: u8,
    direction: u8,
    use_palette: bool,
) -> Vec<Report> {
    let bright = brightness.min(KBD_BRIGHTNESS_MAX);
    let spd = speed.clamp(SPEED_MIN, SPEED_MAX);
    let mut reps = if use_palette {
        palette(colors)
    } else {
        Vec::new()
    };
    reps.push(Report::Feature([
        0x08,
        KBD_POWER_ON,
        mode,
        spd,
        bright,
        0x08,
        direction,
        0x00,
    ]));
    reps
}

pub fn calibrate_keyboard3(rgb: Rgb) -> Rgb {
    match rgb {
        (255, 255, 255) => (125, 255, 185),
        (255, 165, 0) => (255, 125, 0),
        (255, 255, 0) => (210, 255, 0),
        (0, 0, 255) => (0, 20, 255),
        (139, 0, 255) => (110, 0, 255),
        other => other,
    }
}

fn pack_usermode(slots: &[Rgb], brightness: u8) -> Vec<Report> {
    let bright = brightness.min(KBD_BRIGHTNESS_MAX);
    let mut reps = Vec::with_capacity(10);
    reps.push(Report::Feature([
        0x08,
        KBD_POWER_ON,
        0x33,
        0x00,
        bright,
        0x00,
        0x00,
        0x00,
    ]));
    reps.push(Report::Feature([0x12, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00]));
    for pkt_idx in 0..8 {
        let mut pkt = vec![0u8; 65];
        let base_slot = pkt_idx * 16;
        for j in 0..16 {
            let slot_idx = base_slot + j;
            let (r, g, b) = slots.get(slot_idx).copied().unwrap_or((0, 0, 0));
            let off = 1 + j * 4;
            pkt[off + 1] = r;
            pkt[off + 2] = g;
            pkt[off + 3] = b;
        }
        reps.push(Report::Output(pkt));
    }
    reps
}

pub fn keyboard_solid(r: u8, g: u8, b: u8, brightness: u8) -> Vec<Report> {
    let slots = vec![(r, g, b); KBD_WIRE_SLOTS];
    pack_usermode(&slots, brightness)
}

pub fn keyboard_matrix(cells: &[Rgb], brightness: u8, calibrate: bool) -> Result<Vec<Report>, String> {
    if cells.len() != KBD_MATRIX_CELLS {
        return Err(format!(
            "expected {KBD_MATRIX_CELLS} matrix cells, got {}",
            cells.len()
        ));
    }
    let mut slots = vec![(0u8, 0u8, 0u8); KBD_WIRE_SLOTS];
    for col in 0..KBD_MATRIX_COLS {
        for hw_row in 0..KBD_MATRIX_ROWS {
            let top_row = KBD_MATRIX_ROWS - 1 - hw_row;
            let mut rgb = cells[top_row * KBD_MATRIX_COLS + col];
            if calibrate {
                rgb = calibrate_keyboard3(rgb);
            }
            slots[col * KBD_MATRIX_ROWS + hw_row] = rgb;
        }
    }
    Ok(pack_usermode(&slots, brightness))
}

pub fn keyboard_off() -> Vec<Report> {
    keyboard_brightness(0)
}

// ----------------------------------------------------------------- Keymap presets

#[derive(Debug, Clone, Copy)]
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
    let ch = |a: u8, b: u8| -> u8 {
        ((a as f64) + ((b as f64) - (a as f64)) * t)
            .round_ties_even()
            .clamp(0.0, 255.0) as u8
    };
    (ch(c1.0, c2.0), ch(c1.1, c2.1), ch(c1.2, c2.2))
}

fn hue_rgb(pos: f64) -> Rgb {
    let m = pos.rem_euclid(1.0) * 6.0;
    let i = (m.floor() as usize) % 6;
    let f = m - m.floor();
    let up = (255.0 * f).round_ties_even().clamp(0.0, 255.0) as u8;
    let down = (255.0 * (1.0 - f)).round_ties_even().clamp(0.0, 255.0) as u8;
    match i {
        0 => (255, up, 0),
        1 => (down, 255, 0),
        2 => (0, 255, up),
        3 => (0, down, 255),
        4 => (up, 0, 255),
        _ => (255, 0, down),
    }
}

fn all_keycap_groups() -> Vec<(KeyGroup, Vec<usize>)> {
    let mut caps = Vec::with_capacity(101);
    let mut push = |row: usize, col: usize, extra: &[usize], group: KeyGroup| {
        let mut indices = Vec::with_capacity(1 + extra.len());
        indices.push(row * KBD_MATRIX_COLS + col);
        indices.extend_from_slice(extra);
        caps.push((group, indices));
    };

    // Row 0
    push(0, 0, &[], KeyGroup::Mod);
    for c in 1..=12 {
        push(0, c, &[], KeyGroup::Fn);
    }
    for c in 13..=19 {
        push(0, c, &[], KeyGroup::Nav);
    }
    // Row 1
    for c in 0..13 {
        push(1, c, &[], KeyGroup::Alpha);
    }
    push(1, 14, &[], KeyGroup::Mod);
    for c in 15..=18 {
        push(1, c, &[], KeyGroup::Num);
    }
    // Row 2
    push(2, 0, &[], KeyGroup::Mod);
    for i in 0..13 {
        let g = if i == 1 { KeyGroup::Wasd } else { KeyGroup::Alpha };
        push(2, 2 + i, &[], g);
    }
    for c in 15..=18 {
        push(2, c, &[], KeyGroup::Num);
    }
    // Row 3
    push(3, 0, &[], KeyGroup::Mod);
    for i in 0..11 {
        let g = if i <= 2 { KeyGroup::Wasd } else { KeyGroup::Alpha };
        push(3, 2 + i, &[], g);
    }
    push(3, 14, &[], KeyGroup::Mod);
    for c in 15..=17 {
        push(3, c, &[], KeyGroup::Num);
    }
    // Row 4
    push(4, 0, &[85], KeyGroup::Mod);
    for i in 0..10 {
        push(4, 3 + i, &[], KeyGroup::Alpha);
    }
    push(4, 14, &[], KeyGroup::Mod);
    push(4, 15, &[], KeyGroup::Arrows);
    for c in 16..=18 {
        push(4, c, &[], KeyGroup::Num);
    }
    // Row 5
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
        return Err(format!("unknown matrix preset {name:?}"));
    }
    let secondary: Rgb = (255, 0, 128);
    let mut cells = vec![(0u8, 0u8, 0u8); KBD_MATRIX_CELLS];
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

    let color_for = |group: KeyGroup| -> Rgb {
        match name {
            "wasd_fps" => match group {
                KeyGroup::Wasd => (255, 40, 0),
                KeyGroup::Arrows => accent,
                _ => (0, 0, 0),
            },
            "gaming" => match group {
                KeyGroup::Wasd => (255, 32, 0),
                KeyGroup::Arrows => (255, 140, 0),
                KeyGroup::Fn => (110, 0, 255),
                KeyGroup::Nav => (0, 120, 255),
                KeyGroup::Mod => (0, 90, 255),
                KeyGroup::Num => (0, 220, 160),
                KeyGroup::Alpha => accent,
            },
            _ => match group {
                KeyGroup::Wasd | KeyGroup::Alpha => accent,
                KeyGroup::Fn => (255, 165, 0),
                KeyGroup::Nav => (255, 220, 0),
                KeyGroup::Mod => (110, 80, 255),
                KeyGroup::Arrows => (255, 60, 120),
                KeyGroup::Num => (0, 220, 140),
            },
        }
    };

    for (group, indices) in all_keycap_groups() {
        let col = color_for(group);
        for idx in indices {
            if idx < KBD_MATRIX_CELLS {
                cells[idx] = col;
            }
        }
    }

    if name == "wasd_fps" {
        for &idx in &[0usize, 63, 77, 84, 85, 112] {
            cells[idx] = (20, 45, 70);
        }
    }
    Ok(cells)
}

// ----------------------------------------------------------------- Validation

pub fn parse_strict_int(value: &Value, low: i64, high: i64, field: &str) -> Result<i64, String> {
    let Some(n) = value.as_i64() else {
        return Err(format!("{field}: expected integer {low}..{high}"));
    };
    if !(low..=high).contains(&n) {
        return Err(format!("{field}: expected integer {low}..{high}"));
    }
    Ok(n)
}

pub fn parse_rgb(value: &Value) -> Result<Rgb, String> {
    let Some(arr) = value.as_array() else {
        return Err("color: expected three RGB channels".to_string());
    };
    if arr.len() != 3 {
        return Err("color: expected three RGB channels".to_string());
    }
    let r = parse_strict_int(&arr[0], 0, 255, "color")? as u8;
    let g = parse_strict_int(&arr[1], 0, 255, "color")? as u8;
    let b = parse_strict_int(&arr[2], 0, 255, "color")? as u8;
    Ok((r, g, b))
}

pub fn validate_hid(device: &str, saved: &Value) -> Result<RestoreAction, String> {
    if device != "keyboard" && device != "lightbar" {
        return Err("unknown device or invalid saved parameters".to_string());
    }
    let Some(obj) = saved.as_object() else {
        return Err("unknown device or invalid saved parameters".to_string());
    };
    const ALLOWED: &[&str] = &[
        "mode",
        "color",
        "colors",
        "brightness",
        "speed",
        "direction",
        "reactive",
        "matrix",
        "preset",
        "calibrate",
    ];
    for k in obj.keys() {
        if !ALLOWED.contains(&k.as_str()) {
            return Err("unknown saved lighting fields".to_string());
        }
    }
    let Some(mode) = obj.get("mode").and_then(|v| v.as_str()) else {
        return Err("unknown saved lighting mode".to_string());
    };
    let Some(spec) = find_mode_spec(device, mode) else {
        return Err("unknown saved lighting mode".to_string());
    };

    let maximum = if device == "keyboard" {
        KBD_BRIGHTNESS_MAX
    } else {
        LB_BRIGHTNESS_MAX
    };
    let brightness: u8 = match obj.get("brightness") {
        None | Some(Value::Null) => maximum,
        Some(v) => parse_strict_int(v, 0, maximum as i64, "brightness")? as u8,
    };

    let color: Rgb = match obj.get("color") {
        None => (0, 255, 255),
        Some(v) => parse_rgb(v)?,
    };

    let palette_opt: Option<Vec<Rgb>> = match obj.get("colors") {
        None | Some(Value::Null) => None,
        Some(v) => {
            let Some(arr) = v.as_array() else {
                return Err("colors: expected one to seven RGB colors".to_string());
            };
            if arr.is_empty() || arr.len() > 7 {
                return Err("colors: expected one to seven RGB colors".to_string());
            }
            let mut out = Vec::with_capacity(arr.len());
            for item in arr {
                out.push(parse_rgb(item)?);
            }
            Some(out)
        }
    };

    let speed: u8 = match obj.get("speed") {
        None => 5,
        Some(v) => parse_strict_int(v, 1, 10, "speed")? as u8,
    };

    let max_dir = if device == "keyboard" { 4 } else { 2 };
    let direction: u8 = match obj.get("direction") {
        None => 1,
        Some(v) => parse_strict_int(v, 1, max_dir, "direction")? as u8,
    };

    let reactive: bool = match obj.get("reactive") {
        None => false,
        Some(Value::Bool(b)) => *b,
        Some(_) => return Err("reactive: expected a boolean".to_string()),
    };

    let matrix_opt: Option<Vec<Rgb>> = match obj.get("matrix") {
        None | Some(Value::Null) => None,
        Some(v) => {
            if device != "keyboard" {
                return Err("matrix: only supported for keyboard".to_string());
            }
            let Some(arr) = v.as_array() else {
                return Err(format!("matrix: expected {KBD_MATRIX_CELLS} RGB entries"));
            };
            if arr.len() != KBD_MATRIX_CELLS {
                return Err(format!("matrix: expected {KBD_MATRIX_CELLS} RGB entries"));
            }
            let mut out = Vec::with_capacity(arr.len());
            for item in arr {
                out.push(parse_rgb(item)?);
            }
            Some(out)
        }
    };

    let preset_opt: Option<&str> = match obj.get("preset") {
        None | Some(Value::Null) => None,
        Some(v) => {
            let Some(s) = v.as_str() else {
                return Err("preset: unknown keyboard matrix preset".to_string());
            };
            if device != "keyboard" || !PRESET_NAMES.contains(&s) {
                return Err("preset: unknown keyboard matrix preset".to_string());
            }
            Some(s)
        }
    };

    let calibrate: bool = match obj.get("calibrate") {
        None => false,
        Some(Value::Bool(b)) => *b,
        Some(_) => return Err("calibrate: expected a boolean".to_string()),
    };

    let reports = if spec.off || brightness == 0 {
        if device == "keyboard" {
            keyboard_off()
        } else {
            lightbar_off()
        }
    } else if device == "lightbar" {
        let pal: &[Rgb] = palette_opt.as_deref().unwrap_or(&RAINBOW);
        let mut reps = vec![lightbar_power_on()];
        if spec.single {
            reps.extend(lightbar_static(color.0, color.1, color.2, brightness));
        } else {
            reps.extend(lightbar_effect(spec.code, pal, brightness, speed, direction));
        }
        reps
    } else if spec.matrix {
        let cells = match matrix_opt {
            Some(m) => m,
            None => preset_cells(preset_opt.unwrap_or("gaming"), color)?,
        };
        keyboard_matrix(&cells, brightness, calibrate)?
    } else if spec.single {
        keyboard_solid(color.0, color.1, color.2, brightness)
    } else {
        let pal: &[Rgb] = palette_opt.as_deref().unwrap_or(&RAINBOW);
        let behavior = if spec.trigger {
            u8::from(reactive)
        } else if let Some(fixed) = spec.fixed_direction {
            fixed
        } else if spec.direction {
            if let Some(dmap) = spec.direction_map {
                let idx = (direction as usize).saturating_sub(1);
                *dmap.get(idx).unwrap_or(&dmap[0])
            } else {
                direction
            }
        } else {
            0
        };
        keyboard_effect(spec.code, pal, brightness, speed, behavior, spec.palette)
    };

    Ok(RestoreAction {
        device: device.to_string(),
        reports,
        logo_json: None,
    })
}

pub fn validate_logo(saved: &Value) -> Result<RestoreAction, String> {
    let Some(obj) = saved.as_object() else {
        return Err("invalid last-applied Logo record".to_string());
    };
    if obj.len() != 3
        || !obj.contains_key("mode")
        || !obj.contains_key("brightness")
        || !obj.contains_key("color")
    {
        return Err("invalid last-applied Logo record".to_string());
    }
    let Some(mode) = obj.get("mode").and_then(|v| v.as_str()) else {
        return Err("unknown Logo mode".to_string());
    };
    if !matches!(mode, "solid" | "breathing" | "rainbow") {
        return Err("unknown Logo mode".to_string());
    }
    let brightness = parse_strict_int(&obj["brightness"], 0, 100, "Logo brightness")? as u8;
    if !matches!(brightness, 0 | 25 | 50 | 75 | 100) {
        return Err("unsupported Logo brightness".to_string());
    }
    let color = parse_rgb(&obj["color"])?;
    if mode == "breathing" && !LOGO_COLORS.contains(&color) {
        return Err("unsupported Logo breathing color".to_string());
    }
    // Preserve exact field layout (`mode`, `brightness`, `color`)
    let mut map = Map::new();
    map.insert("mode".to_string(), json!(mode));
    map.insert("brightness".to_string(), json!(brightness));
    map.insert("color".to_string(), json!([color.0, color.1, color.2]));
    let logo_json = serde_json::to_string(&Value::Object(map))
        .map_err(|e| format!("serialize logo JSON failed: {e}"))?;
    Ok(RestoreAction {
        device: "logo".to_string(),
        reports: Vec::new(),
        logo_json: Some(logo_json),
    })
}

pub fn load_actions(directory: &Path, uid: u32) -> (Vec<RestoreAction>, Vec<String>) {
    let mut actions = Vec::new();
    let mut errors = Vec::new();
    match read_saved_json(&directory.join("state.json"), uid, MAX_STATE_BYTES) {
        Ok(Some(saved)) => {
            if let Some(obj) = saved.as_object() {
                let unknown = obj.keys().any(|k| k != "keyboard" && k != "lightbar");
                if unknown {
                    errors.push("state.json: state.json has unknown devices".to_string());
                } else {
                    for dev in ["keyboard", "lightbar"] {
                        if let Some(val) = obj.get(dev) {
                            match validate_hid(dev, val) {
                                Ok(act) => actions.push(act),
                                Err(e) => errors.push(format!("{dev}: {e}")),
                            }
                        }
                    }
                }
            } else {
                errors.push("state.json: state.json has unknown devices".to_string());
            }
        }
        Ok(None) => {}
        Err(e) => errors.push(format!("state.json: {e}")),
    }

    match read_saved_json(&directory.join("logo.json"), uid, MAX_STATE_BYTES) {
        Ok(Some(saved)) => match validate_logo(&saved) {
            Ok(act) => actions.push(act),
            Err(e) => errors.push(format!("logo.json: {e}")),
        },
        Ok(None) => {}
        Err(e) => errors.push(format!("logo.json: {e}")),
    }

    (actions, errors)
}

// ----------------------------------------------------------------- Linux hidraw + Logo I/O

const fn ioc(dir: u32, ty: u8, nr: u32, size: u32) -> u32 {
    (dir << 30) | (size << 16) | ((ty as u32) << 8) | nr
}

const fn hidiocsfeature(len: u32) -> u32 {
    ioc(3, b'H', 0x06, len)
}

const fn hidiocgfeature(len: u32) -> u32 {
    ioc(3, b'H', 0x07, len)
}

pub fn led_name_for_label(label: &str) -> Option<&'static str> {
    match label {
        "keyboard" => Some("rgb:kbd_backlight"),
        "lightbar" => Some("rgb:lightbar"),
        _ => None,
    }
}

pub fn hidraw_for_led(led_name: &str) -> Option<String> {
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
    let expected = match led_name {
        "rgb:kbd_backlight" => "0003:0000048D:0000600B",
        "rgb:lightbar" => "0003:0000048D:00007001",
        _ => return None,
    };
    let mut nodes: Vec<_> = fs::read_dir("/sys/class/hidraw")
        .ok()?
        .flatten()
        .collect();
    nodes.sort_by_key(|e| e.file_name());
    for entry in nodes {
        let uevent_path = entry.path().join("device").join("uevent");
        let Ok(text) = fs::read_to_string(&uevent_path) else {
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

pub fn hidraw_available(label: &str) -> bool {
    led_name_for_label(label)
        .and_then(hidraw_for_led)
        .is_some()
}

fn open_hidraw(label: &str) -> Result<File, String> {
    let led_name = led_name_for_label(label).ok_or_else(|| format!("unknown device: {label}"))?;
    let path = hidraw_for_led(led_name).ok_or_else(|| {
        format!("No hidraw device for /sys/class/leds/{led_name}")
    })?;
    OpenOptions::new()
        .read(true)
        .write(true)
        .open(&path)
        .map_err(|e| format!("open {path} failed: {e}"))
}

fn send_reports_to_file(file: &mut File, reports: &[Report]) -> Result<(), String> {
    let fd = file.as_raw_fd();
    for rep in reports {
        match rep {
            Report::Feature(data) => {
                let req = hidiocsfeature(data.len() as u32);
                let mut buf = *data;
                let rc = unsafe { libc::ioctl(fd, req as libc::Ioctl, buf.as_mut_ptr()) };
                if rc < 0 {
                    return Err(format!(
                        "HIDIOCSFEATURE failed: {}",
                        std::io::Error::last_os_error()
                    ));
                }
            }
            Report::Output(data) => {
                file.write_all(data)
                    .map_err(|e| format!("hidraw output write failed: {e}"))?;
            }
            Report::Sleep(ms) => {
                std::thread::sleep(Duration::from_millis(*ms as u64));
            }
        }
    }
    Ok(())
}

pub fn query_keyboard_hw_locked() -> Result<KeyboardHwState, String> {
    let mut file = open_hidraw("keyboard")?;
    send_reports_to_file(&mut file, &[query_report(QUERY_EFFECT)])?;
    let mut buf = [0u8; 9]; // report id 0 + 8-byte reply
    let req = hidiocgfeature(buf.len() as u32);
    let rc = unsafe { libc::ioctl(file.as_raw_fd(), req as libc::Ioctl, buf.as_mut_ptr()) };
    if rc < 8 {
        return Err(format!(
            "HIDIOCGFEATURE failed or short reply (rc={rc}): {}",
            std::io::Error::last_os_error()
        ));
    }
    if buf[1] != QUERY_EFFECT {
        return Err(format!("unexpected HID query echo byte: 0x{:02x}", buf[1]));
    }
    parse_keyboard_state(&buf[1..]).ok_or_else(|| "short keyboard query reply".to_string())
}

pub async fn run_profile_helper(args: &[&str], timeout_secs: u64) -> Result<Value, String> {
    let mut cmd = Command::new(PKEXEC_PATH);
    cmd.arg("--disable-internal-agent")
        .arg(HELPER_PATH)
        .args(args)
        .stdin(Stdio::null())
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .kill_on_drop(true);
    let output = tokio::time::timeout(Duration::from_secs(timeout_secs), cmd.output())
        .await
        .map_err(|_| format!("helper timed out after {timeout_secs}s"))?
        .map_err(|e| format!("spawn pkexec helper failed: {e}"))?;

    let stdout = String::from_utf8_lossy(&output.stdout).trim().to_string();
    let stderr = String::from_utf8_lossy(&output.stderr).trim().to_string();
    if !output.status.success() {
        let mut msg = stderr;
        if msg.is_empty() && !stdout.is_empty() {
            if let Ok(v) = serde_json::from_str::<Value>(&stdout) {
                if let Some(err) = v.get("error").and_then(|e| e.as_str()) {
                    msg = err.to_string();
                }
            }
            if msg.is_empty() {
                msg = stdout;
            }
        }
        return Err(if msg.is_empty() {
            "helper execution failed".to_string()
        } else {
            msg
        });
    }
    let parsed: Value = serde_json::from_str(&stdout)
        .map_err(|e| format!("helper returned invalid JSON: {e}"))?;
    if !parsed.is_object() {
        return Err("helper response is not a JSON object".to_string());
    }
    if let Some(err) = parsed.get("error").and_then(|e| e.as_str()) {
        return Err(err.to_string());
    }
    if parsed.get("error").is_some() {
        return Err("helper rejected operation".to_string());
    }
    Ok(parsed)
}

pub async fn apply_action_locked(action: &RestoreAction) -> Result<(), String> {
    if action.device == "logo" {
        let raw = action
            .logo_json
            .as_deref()
            .ok_or_else(|| "missing logo_json".to_string())?;
        let expected_val: Value =
            serde_json::from_str(raw).map_err(|e| format!("invalid logo JSON: {e}"))?;
        let validated = validate_logo(&expected_val)?;
        let logo_arg = validated.logo_json.as_deref().unwrap_or(raw);
        let response = run_profile_helper(&["logo", logo_arg], 5).await?;
        let Some(exp_obj) = expected_val.as_object() else {
            return Err("invalid logo JSON".to_string());
        };
        for (k, v) in exp_obj {
            if response.get(k) != Some(v) {
                return Err("Logo helper readback differs from saved state".to_string());
            }
        }
        Ok(())
    } else if action.device == "keyboard" || action.device == "lightbar" {
        let mut file = open_hidraw(&action.device)?;
        send_reports_to_file(&mut file, &action.reports)
    } else {
        Err("unknown restore target".to_string())
    }
}

pub async fn apply_action(action: &RestoreAction) -> Result<(), String> {
    let _guard = ZoneGuard::acquire(&action.device)?;
    apply_action_locked(action).await
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_off_and_zero_brightness_never_send_lightbar_power_on() {
        let pwr = lightbar_power_on();
        for dev in ["keyboard", "lightbar"] {
            for saved in [json!({"mode": "off"}), json!({"mode": "wave", "brightness": 0})] {
                let act = validate_hid(dev, &saved).unwrap();
                assert!(!act.reports.contains(&pwr));
                if dev == "keyboard" {
                    assert_eq!(act.reports, keyboard_brightness(0));
                } else {
                    assert_eq!(act.reports, lightbar_off());
                }
            }
        }
    }

    #[test]
    fn test_lightbar_on_prepends_power_on() {
        let act = validate_hid(
            "lightbar",
            &json!({"mode": "static", "brightness": 31, "color": [1, 2, 3]}),
        )
        .unwrap();
        assert_eq!(act.reports[0], lightbar_power_on());
        match act.reports.last().unwrap() {
            Report::Feature(b) => assert_eq!(b[4], 31),
            other => panic!("unexpected report: {other:?}"),
        }
    }

    #[test]
    fn test_reactive_and_submodes_match_python() {
        let auto = validate_hid("keyboard", &json!({"mode": "spark", "direction": 4})).unwrap();
        let reactive = validate_hid(
            "keyboard",
            &json!({"mode": "spark", "direction": 4, "reactive": true}),
        )
        .unwrap();
        assert_eq!(
            match auto.reports.last().unwrap() {
                Report::Feature(b) => b[6],
                _ => 255,
            },
            0
        );
        assert_eq!(
            match reactive.reports.last().unwrap() {
                Report::Feature(b) => b[6],
                _ => 255,
            },
            1
        );

        let sine2 = validate_hid("keyboard", &json!({"mode": "sine", "direction": 2})).unwrap();
        assert_eq!(
            match sine2.reports.last().unwrap() {
                Report::Feature(b) => (b[2], b[6]),
                _ => (0, 0),
            },
            (0x03, 17)
        );

        let diag1 = validate_hid("keyboard", &json!({"mode": "diagonal", "direction": 1})).unwrap();
        assert_eq!(
            match diag1.reports.last().unwrap() {
                Report::Feature(b) => (b[2], b[6]),
                _ => (0, 0),
            },
            (0x03, 33)
        );
    }

    #[test]
    fn test_invalid_fields_rejected_without_clamping() {
        let bad = [
            json!({"mode": "rainbow", "brightness": 51}),
            json!({"mode": "rainbow", "brightness": true}),
            json!({"mode": "rainbow", "speed": 11}),
            json!({"mode": "rainbow", "color": [-1, 0, 0]}),
            json!({"mode": "rainbow", "colors": [[1,2,3],[1,2,3],[1,2,3],[1,2,3],[1,2,3],[1,2,3],[1,2,3],[1,2,3]]}),
            json!({"mode": "rainbow", "reactive": 1}),
            json!({"mode": "rainbow", "arbitrary_path": "/dev/mem"}),
            json!({"mode": "unknown"}),
        ];
        for saved in &bad {
            assert!(validate_hid("keyboard", saved).is_err(), "should fail: {saved}");
        }
        assert!(validate_hid("lightbar", &json!({"mode": "wave", "direction": 4})).is_err());
        assert!(validate_logo(&json!({"mode": "solid", "brightness": 30, "color": [0, 0, 0]})).is_err());
        assert!(validate_logo(&json!({"mode": "breathing", "brightness": 25, "color": [1, 2, 3]})).is_err());
    }

    #[test]
    fn test_matrix_preset_packing_and_calibration() {
        let act = validate_hid(
            "keyboard",
            &json!({"mode": "matrix", "preset": "wasd_fps", "brightness": 50, "calibrate": true}),
        )
        .unwrap();
        assert_eq!(act.reports.len(), 10); // 2 feature + 8 output
        assert_eq!(
            act.reports[0],
            Report::Feature([0x08, 0x02, 0x33, 0x00, 50, 0x00, 0x00, 0x00])
        );
        assert_eq!(
            act.reports[1],
            Report::Feature([0x12, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00])
        );
        for rep in &act.reports[2..] {
            match rep {
                Report::Output(pkt) => {
                    assert_eq!(pkt.len(), 65);
                    assert_eq!(pkt[0], 0);
                }
                _ => panic!("expected Output report"),
            }
        }
        assert_eq!(calibrate_keyboard3((255, 255, 255)), (125, 255, 185));
    }

    #[test]
    fn test_decode_keyboard_state_variants() {
        assert_eq!(
            decode_keyboard_state(
                KeyboardHwState { power: 1, mode: 0x05, speed: 5, brightness: 50, direction: 1 },
                None
            ),
            ("off", 1)
        );
        assert_eq!(
            decode_keyboard_state(
                KeyboardHwState { power: 2, mode: 0x03, speed: 5, brightness: 40, direction: 17 },
                None
            ),
            ("sine", 2)
        );
        assert_eq!(
            decode_keyboard_state(
                KeyboardHwState { power: 2, mode: 0x33, speed: 5, brightness: 35, direction: 1 },
                Some("matrix")
            ),
            ("matrix", 1)
        );
        assert_eq!(
            decode_keyboard_state(
                KeyboardHwState { power: 2, mode: 0x33, speed: 5, brightness: 35, direction: 1 },
                None
            ),
            ("solid", 1)
        );
    }
}

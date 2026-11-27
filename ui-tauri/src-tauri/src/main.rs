// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

mod commands;
mod hid;
mod zone_lock;

fn main() {
    tauri::Builder::default()
        .invoke_handler(tauri::generate_handler![
            commands::get_system_info,
            commands::all_lights_off,
            commands::get_lighting_state,
            commands::apply_lighting,
            commands::poll_keyboard_hw,
            commands::get_matrix_preset,
            commands::get_logo_status,
            commands::apply_logo,
            commands::set_logo_enabled,
            commands::get_lighting_policy,
            commands::save_lighting_policy,
            commands::get_profiles_config,
            commands::save_profiles_config,
            commands::get_builtin_templates,
            commands::capture_current_profile,
            commands::apply_profile_bundle,
            commands::get_performance_status,
            commands::set_performance_mode,
            commands::set_turbo_variant,
            commands::apply_performance_tuning,
            commands::reset_performance_tuning,
            commands::get_watercooling_local_config,
            commands::watercooling_request,
        ])
        .run(tauri::generate_context!())
        .expect("error while running MECHREVO Linux Control");
}

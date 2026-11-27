// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Shared library modules for MECHREVO Linux Control root system daemons.

pub mod acpi_check;
pub mod common;
pub mod ec_guard;
pub mod lifecycle;
pub mod nvidia_patch;
pub mod oem_scheduler;
pub mod profile_helper;
pub mod sha256;
pub mod turbo;

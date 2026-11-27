// SPDX-License-Identifier: AGPL-3.0-only
// This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
//! Pure model of the OEM 1.5-second CPU+GPU combined/game workload scheduler,
//! strictly matching `iteglow/src/iteglow/oem_scheduler.py`.

use serde::Serialize;
use std::collections::HashMap;

pub const ACK_ON: u8 = 0xBB;
pub const ACK_OFF: u8 = 0xAA;

#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub enum Operation {
    CombinedOn,
    CombinedOff,
    GameOn,
    GameOff,
}

impl Operation {
    pub const fn as_str(self) -> &'static str {
        match self {
            Self::CombinedOn => "combined_on",
            Self::CombinedOff => "combined_off",
            Self::GameOn => "game_on",
            Self::GameOff => "game_off",
        }
    }

    pub const fn kernel_cmd(self) -> &'static str {
        match self {
            Self::CombinedOn => "combined 1",
            Self::CombinedOff => "combined 0",
            Self::GameOn => "game 1",
            Self::GameOff => "game 0",
        }
    }

    pub const fn expected_ack(self) -> u8 {
        match self {
            Self::CombinedOn | Self::GameOn => ACK_ON,
            Self::CombinedOff | Self::GameOff => ACK_OFF,
        }
    }
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub struct LoadSample {
    pub cpu_percent: u8,
    pub gpu_percent: u8,
}

impl LoadSample {
    pub fn new(cpu_percent: u8, gpu_percent: u8) -> Result<Self, String> {
        if cpu_percent > 100 || gpu_percent > 100 {
            return Err("LoadSample percentages must be in 0..=100".to_string());
        }
        Ok(Self {
            cpu_percent,
            gpu_percent,
        })
    }
}

#[derive(Debug, Clone, Copy, PartialEq, Eq, Serialize)]
pub struct SchedulerState {
    pub cpu_count: i8,
    pub combined_gpu_count: i8,
    pub game_gpu_count: i8,
    pub combined_reply: u8,
    pub single_gpu_reply: u8,
    pub combined_condition: bool,
    pub game_on: bool,
    pub game_launch_condition: i8,
}

impl Default for SchedulerState {
    fn default() -> Self {
        Self {
            cpu_count: 0,
            combined_gpu_count: 0,
            game_gpu_count: 0,
            combined_reply: 0,
            single_gpu_reply: 0,
            combined_condition: false,
            game_on: false,
            game_launch_condition: -1,
        }
    }
}

#[derive(Debug, Clone, PartialEq, Eq)]
pub struct Transition {
    pub state: SchedulerState,
    pub operations: Vec<Operation>,
    pub sampled: bool,
}

/// Evaluate one 1.5s workload callback, preserving OEM counters, ordering, and ACK gates.
pub fn advance(
    state: SchedulerState,
    sample: Option<LoadSample>,
    replies: &HashMap<Operation, u8>,
    round_adapter: bool,
    game_supported: bool,
) -> Result<Transition, String> {
    if !round_adapter {
        return Ok(Transition {
            state,
            operations: Vec::new(),
            sampled: false,
        });
    }
    let Some(sample) = sample else {
        return Err("a LoadSample is required with the round adapter present".to_string());
    };

    let cpu = if sample.cpu_percent > 95 {
        (state.cpu_count + 1).min(3)
    } else if sample.cpu_percent < 10 {
        (state.cpu_count - 1).max(-1)
    } else {
        0
    };
    let combined_gpu = (state.combined_gpu_count
        + if sample.gpu_percent > 90 { 1 } else { -1 })
    .clamp(-1, 3);
    let mut game_gpu = state.game_gpu_count;
    if game_supported {
        game_gpu = (game_gpu + if sample.gpu_percent > 50 { 1 } else { -1 }).clamp(-1, 3);
    }

    let mut current = SchedulerState {
        cpu_count: cpu,
        combined_gpu_count: combined_gpu,
        game_gpu_count: game_gpu,
        combined_condition: false,
        ..state
    };
    let mut requested = Vec::new();

    if cpu >= 3 && combined_gpu >= 3 {
        current.combined_condition = true;
        if current.game_on {
            requested.push(Operation::GameOff);
            current.game_on = false;
        }
        if current.combined_reply != ACK_ON && current.single_gpu_reply != ACK_ON {
            let Some(&ack) = replies.get(&Operation::CombinedOn) else {
                return Err("explicit firmware reply required for combined_on".to_string());
            };
            requested.push(Operation::CombinedOn);
            current.combined_reply = ack;
        }
    } else if (cpu <= -1 || combined_gpu <= -1) && current.combined_reply != ACK_OFF {
        let Some(&ack) = replies.get(&Operation::CombinedOff) else {
            return Err("explicit firmware reply required for combined_off".to_string());
        };
        requested.push(Operation::CombinedOff);
        current.combined_reply = ack;
    }

    if game_supported {
        if game_gpu >= 3 {
            if current.combined_reply != ACK_ON && !current.game_on {
                requested.push(Operation::GameOn);
                current.game_on = true;
            }
        } else if game_gpu <= -1 && (current.game_on || current.game_launch_condition == -1) {
            requested.push(Operation::GameOff);
            current.game_on = false;
            current.game_launch_condition = 0;
        }
    }

    Ok(Transition {
        state: current,
        operations: requested,
        sampled: true,
    })
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_combined_on_after_three_high_samples_and_game_off_ordering() {
        let mut st = SchedulerState {
            game_on: true,
            game_gpu_count: 3,
            game_launch_condition: 0,
            ..Default::default()
        };
        let mut replies = HashMap::new();
        replies.insert(Operation::CombinedOn, ACK_ON);
        replies.insert(Operation::CombinedOff, ACK_OFF);

        let high = LoadSample::new(98, 95).unwrap();
        for _ in 0..2 {
            let tr = advance(st, Some(high), &replies, true, true).unwrap();
            assert!(tr.operations.is_empty());
            st = tr.state;
        }
        let tr = advance(st, Some(high), &replies, true, true).unwrap();
        assert_eq!(tr.operations, vec![Operation::GameOff, Operation::CombinedOn]);
        assert!(tr.state.combined_condition);
        assert!(!tr.state.game_on);
        assert_eq!(tr.state.combined_reply, ACK_ON);
    }

    #[test]
    fn test_failed_combined_ack_falls_back_to_game_on() {
        let st = SchedulerState {
            cpu_count: 2,
            combined_gpu_count: 2,
            game_gpu_count: 2,
            ..Default::default()
        };
        let mut replies = HashMap::new();
        replies.insert(Operation::CombinedOn, 0); // failed ACK
        replies.insert(Operation::CombinedOff, 0);
        let tr = advance(st, Some(LoadSample::new(99, 95).unwrap()), &replies, true, true).unwrap();
        assert_eq!(tr.operations, vec![Operation::CombinedOn, Operation::GameOn]);
        assert!(tr.state.game_on);
    }

    #[test]
    fn test_unplugging_round_adapter_freezes_state() {
        let st = SchedulerState {
            cpu_count: 2,
            combined_reply: ACK_ON,
            ..Default::default()
        };
        let tr = advance(st, None, &HashMap::new(), false, true).unwrap();
        assert!(!tr.sampled);
        assert!(tr.operations.is_empty());
        assert_eq!(tr.state, st);
    }
}

# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
"""Offline model of the OEM 1.5-second workload scheduler.

This module only calculates state and records requested operations. It cannot
read or write hardware. The caller supplies samples and *explicit* firmware
reply bytes; replies used for simulation must not be presented as observations.

Evidence: research/firmware-link-20261115/decoded-source/sysmanager-restored.cs,
MySystemManager.cpugpuDTRunTimer_Elapsed (lines 1878-2010). Temperatures, water
cooling, ACPI notifications and GPU power budgets belong to other layers and
are intentionally not inferred here. A request is not proof of hardware effect.
"""

from dataclasses import dataclass, replace
from enum import Enum
from typing import Callable, Mapping, Optional, Tuple


INTERVAL_MS = 1500
ACK_ON = 0xBB
ACK_OFF = 0xAA


class Operation(str, Enum):
    COMBINED_ON = "combined_on"
    COMBINED_OFF = "combined_off"
    GAME_ON = "game_on"
    GAME_OFF = "game_off"


def _integer_in_range(name: str, value: int, lower: int, upper: int) -> None:
    if type(value) is not int or not lower <= value <= upper:
        raise ValueError(f"{name} must be an integer in {lower}..{upper}")


@dataclass(frozen=True)
class LoadSample:
    """OEM-style integer percentages, not a per-core maximum or process CPU%."""

    cpu_percent: int
    gpu_percent: int

    def __post_init__(self) -> None:
        _integer_in_range("cpu_percent", self.cpu_percent, 0, 100)
        _integer_in_range("gpu_percent", self.gpu_percent, 0, 100)


@dataclass(frozen=True)
class SchedulerState:
    cpu_count: int = 0
    combined_gpu_count: int = 0
    game_gpu_count: int = 0
    combined_reply: int = 0
    single_gpu_reply: int = 0
    combined_condition: bool = False
    game_on: bool = False
    game_launch_condition: int = -1

    def __post_init__(self) -> None:
        for name in ("cpu_count", "combined_gpu_count", "game_gpu_count"):
            _integer_in_range(name, getattr(self, name), -1, 3)
        for name in ("combined_reply", "single_gpu_reply"):
            _integer_in_range(name, getattr(self, name), 0, 255)
        for name in ("combined_condition", "game_on"):
            if type(getattr(self, name)) is not bool:
                raise ValueError(f"{name} must be a bool")
        if type(self.game_launch_condition) is not int or self.game_launch_condition not in (-1, 0):
            raise ValueError("game_launch_condition must be -1 or 0")


@dataclass(frozen=True)
class Transition:
    state: SchedulerState
    operations: Tuple[Operation, ...]
    sampled: bool


def round_adapter_present(ec_07cc: int, ec_0490: int) -> bool:
    """EcCtrl.GetRoundSocketStatusFromEC; this is not generic AC-online.

    Both values are supplied by the caller. This function performs no EC read.
    0x07cc bit 0 takes precedence. Only a zero low nibble permits the fallback
    to 0x0490 bit 0, so a USB-C power indication must not be treated as round AC.
    """
    _integer_in_range("ec_07cc", ec_07cc, 0, 255)
    _integer_in_range("ec_0490", ec_0490, 0, 255)
    return bool(ec_07cc & 1) or (ec_07cc & 0xF == 0 and bool(ec_0490 & 1))


def advance(
    state: SchedulerState,
    sample: Optional[LoadSample],
    replies: Mapping[Operation, int],
    *,
    round_adapter: bool,
    game_supported: bool,
) -> Transition:
    """Evaluate one callback, preserving OEM counters, ordering and ack gates.

    ``replies`` must contain a byte for each requested COMBINED operation.
    Use 0 explicitly to model an unknown/unsuccessful reply, or 0xBB/0xAA to
    model successful ON/OFF. GAME replies are ignored by the OEM and by this
    model. A missing combined reply raises before caller state is committed.

    Unplugging the round adapter freezes the OEM state; it does not issue OFF.
    The caller must serialize callbacks (the OEM uses Interlocked.Exchange).
    Firmware/thermal fail-safes must not be inferred from this host state.
    """
    if type(round_adapter) is not bool or type(game_supported) is not bool:
        raise ValueError("round_adapter and game_supported must be bools")
    if not round_adapter:
        return Transition(state, (), False)
    if not isinstance(sample, LoadSample):
        raise ValueError("a LoadSample is required with the round adapter present")

    cpu = state.cpu_count
    if sample.cpu_percent > 95:
        cpu = min(3, cpu + 1)
    elif sample.cpu_percent < 10:
        cpu = max(-1, cpu - 1)
    else:
        cpu = 0
    combined_gpu = max(-1, min(3, state.combined_gpu_count + (1 if sample.gpu_percent > 90 else -1)))
    game_gpu = state.game_gpu_count
    if game_supported:
        game_gpu = max(-1, min(3, game_gpu + (1 if sample.gpu_percent > 50 else -1)))

    current = replace(state, cpu_count=cpu, combined_gpu_count=combined_gpu,
                      game_gpu_count=game_gpu, combined_condition=False)
    requested = []

    def combined_reply(operation: Operation) -> int:
        if operation not in replies:
            raise ValueError(f"explicit firmware reply required for {operation.value}")
        value = replies[operation]
        _integer_in_range("firmware reply", value, 0, 255)
        requested.append(operation)
        return value

    if cpu >= 3 and combined_gpu >= 3:
        current = replace(current, combined_condition=True)
        if current.game_on:
            requested.append(Operation.GAME_OFF)
            current = replace(current, game_on=False)
        if current.combined_reply != ACK_ON and current.single_gpu_reply != ACK_ON:
            current = replace(current, combined_reply=combined_reply(Operation.COMBINED_ON))
    elif (cpu <= -1 or combined_gpu <= -1) and current.combined_reply != ACK_OFF:
        current = replace(current, combined_reply=combined_reply(Operation.COMBINED_OFF))

    if game_supported:
        if game_gpu >= 3:
            if current.combined_reply != ACK_ON and not current.game_on:
                requested.append(Operation.GAME_ON)
                current = replace(current, game_on=True)
        elif game_gpu <= -1 and (current.game_on or current.game_launch_condition == -1):
            requested.append(Operation.GAME_OFF)
            current = replace(current, game_on=False, game_launch_condition=0)
    return Transition(current, tuple(requested), True)


class WorkloadScheduler:
    """Small injectable polling adapter, with no timers or hardware backend.

    ``adapter_reader`` and ``sample_reader`` are read-only callbacks owned by
    the caller. ``replies`` are simulation/observation data, never an actuator.
    Missing replies or sample errors leave the previous state unchanged.
    """

    def __init__(self, *, game_supported: bool, state: Optional[SchedulerState] = None):
        if type(game_supported) is not bool:
            raise ValueError("game_supported must be a bool")
        self.game_supported = game_supported
        self.state = state if state is not None else SchedulerState()

    def poll(
        self,
        adapter_reader: Callable[[], bool],
        sample_reader: Callable[[], LoadSample],
        replies: Mapping[Operation, int],
    ) -> Transition:
        adapter = adapter_reader()
        if type(adapter) is not bool:
            raise ValueError("adapter_reader must return a bool")
        sample = sample_reader() if adapter else None
        result = advance(self.state, sample, replies, round_adapter=adapter,
                         game_supported=self.game_supported)
        self.state = result.state
        return result

    def restore_parameters(self) -> None:
        """Model RestoreCPUGPUCombineParams *after* its registry gate is met.

        That OEM function does not reset game_launch_condition or issue WMI.
        The registry check/write itself is deliberately outside this model.
        """
        self.state = SchedulerState(game_launch_condition=self.state.game_launch_condition)

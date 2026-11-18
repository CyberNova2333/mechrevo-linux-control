# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
"""Session-only policy runner; temporary outputs never update manual state files.

All sampling, session checks and writes are injectable. Importing this module
does not access D-Bus, configuration files or lighting hardware.
"""
from __future__ import annotations

from dataclasses import asdict, dataclass
from contextlib import nullcontext
import json
import math
from pathlib import Path

from .lighting_policy import (PolicyMemory, acknowledge, action_matches,
                              default_config, plan, validate_config)
from .lighting_session import _read_saved, apply_action, validate_hid, validate_logo
from .zone_lock import ZoneTransaction

CONFIG_PATH = Path.home() / '.config/iteglow/lighting-policy.json'


@dataclass(frozen=True)
class ManualState:
    records: dict
    errors: tuple = ()


def read_config(path, uid):
    saved = _read_saved(Path(path), uid)
    return default_config() if saved is None else validate_config(saved)


def read_manual(directory, uid):
    """Only successful manual application records; no preference or pin data."""
    records, errors = {}, []
    directory = Path(directory)
    try:
        saved = _read_saved(directory / 'state.json', uid)
        if saved is not None:
            if not isinstance(saved, dict) or set(saved) - {'keyboard', 'lightbar'}:
                raise ValueError('state.json contains unknown devices')
            records.update(saved)
    except (OSError, ValueError, TypeError) as error:
        errors.append(f'state.json: {error}')
    try:
        saved = _read_saved(directory / 'logo.json', uid)
        if saved is not None:
            records['logo'] = saved
    except (OSError, ValueError, TypeError) as error:
        errors.append(f'logo.json: {error}')
    return ManualState(records, tuple(errors))


class SessionInputs:
    """Known D-Bus contracts, independent of Gio so malformed replies are testable."""
    def __init__(self, system_call, user_call, variant):
        self.system_call, self.user_call, self.variant = system_call, user_call, variant

    @staticmethod
    def single(reply):
        if not isinstance(reply, (tuple, list)) or len(reply) != 1:
            raise ValueError('D-Bus reply must contain exactly one value')
        return reply[0]

    def power(self):
        value = self.single(self.system_call(
            'org.freedesktop.UPower', '/org/freedesktop/UPower',
            'org.freedesktop.DBus.Properties', 'Get',
            self.variant('(ss)', ('org.freedesktop.UPower', 'OnBattery'))))
        if type(value) is not bool:
            raise ValueError('UPower OnBattery is not boolean')
        return 'dc' if value else 'ac'

    def idle(self):
        value = self.single(self.user_call(
            'org.gnome.Mutter.IdleMonitor', '/org/gnome/Mutter/IdleMonitor/Core',
            'org.gnome.Mutter.IdleMonitor', 'GetIdletime'))
        if type(value) is not int or not 0 <= value < 2**64:
            raise ValueError('Mutter GetIdletime is not uint64 milliseconds')
        return value / 1000


class PolicyRunner:
    def __init__(self, *, active, config, manual, power, idle, writer=None,
                 apply=False, emit=None, transaction=None):
        self.active, self.config, self.manual = active, config, manual
        self.power, self.idle, self.writer = power, idle, writer or apply_action
        self.apply, self.emit = apply, emit
        self.transaction = transaction or ZoneTransaction
        self.memory = PolicyMemory()
        self.restoring = False
        self._last_report = None
        self._config_snapshot = None

    def begin_restore(self):
        self.restoring = True

    def finish_restore(self):
        # A manual restore changed actual hardware outside the policy's writes.
        self.memory = PolicyMemory()
        self.restoring = False

    def deactivate(self):
        self.memory = PolicyMemory()

    def _active(self):
        try:
            return self.active() is True
        except Exception:
            self.deactivate()
            raise

    def _finish(self, result):
        result['errors'] = list(dict.fromkeys(result['errors']))
        signature = json.dumps({'status': result['status'], 'errors': result['errors'],
                                'memory': asdict(self.memory), 'config': self._config_snapshot,
                                'preview': result['planned'] if not self.apply else None},
                               sort_keys=True, ensure_ascii=False)
        changed = signature != self._last_report
        self._last_report = signature
        if self.emit is not None and (changed or result['applied']):
            self.emit({'event': 'lighting_policy', **result})
        return result

    def _drop_zone(self, device):
        self.memory = PolicyMemory(tuple(zone for zone in self.memory.zones if zone.device != device))

    def tick(self):
        result = {'status': 'disabled', 'planned': [], 'applied': [], 'skipped': [], 'errors': []}
        try:
            if not self._active():
                self.deactivate()
                result['status'] = 'inactive'
                return self._finish(result)
            if self.restoring:
                result['status'] = 'manual_restore_pending'
                return self._finish(result)
            config = validate_config(self.config())
            self._config_snapshot = config
            # Missing/default-disabled policy with no ownership does not sample
            # power or idle and does not read/modify any manual device state.
            if not config['enabled'] and not self.memory.zones:
                return self._finish(result)
            saved = self.manual()
            result['errors'].extend(saved.errors)
            power = idle = None
            if config['enabled'] and config['power_enabled']:
                try:
                    power = self.power()
                    if power not in ('ac', 'dc', None):
                        raise ValueError('invalid power sample')
                except Exception as error:
                    power = None
                    result['errors'].append(f'power: {error}')
            if config['enabled'] and config['idle_minutes']:
                try:
                    idle = self.idle()
                    if idle is not None and (type(idle) not in (int, float)
                                             or not math.isfinite(idle) or idle < 0):
                        raise ValueError('invalid idle sample')
                except Exception as error:
                    idle = None
                    result['errors'].append(f'idle: {error}')
            proposal = plan(config, power=power, idle_seconds=idle,
                            saved=saved.records, memory=self.memory)
            self.memory = proposal.memory
            result['status'] = proposal.status if self.apply else 'preview_' + proposal.status
            result['errors'].extend(proposal.errors)
            for action in proposal.actions:
                if not self._active():
                    self.deactivate()
                    result['status'] = 'inactive'
                    break
                try:
                    # Acquire before the fresh manual read: a GUI/helper can
                    # finish and save a new manual state while we wait here.
                    with self.transaction(action.device) if self.apply else nullcontext():
                        latest = self.manual()
                        result['errors'].extend(latest.errors)
                        if not action_matches(action, latest.records):
                            self._drop_zone(action.device)
                            result['skipped'].append(action.device + ': manual state changed')
                            continue
                        target = (validate_logo(action.params) if action.device == 'logo'
                                  else validate_hid(action.device, action.params))
                        result['planned'].append({'device': action.device, 'reason': action.reason,
                                                  'brightness': action.params.get('brightness')})
                        if not self.apply:
                            continue
                        if not self._active():
                            self.deactivate()
                            result['status'] = 'inactive'
                            break
                        if self.writer(target) is False:
                            raise RuntimeError('lighting writer reported failure')
                        if not self._active():
                            self.deactivate()
                            result['status'] = 'inactive'
                            break
                        self.memory = acknowledge(self.memory, action)
                        result['applied'].append(action.device)
                except Exception as error:
                    result['errors'].append(f'{action.device}: {error}')
        except Exception as error:
            # Unknown config/session/manual input cannot authorize writes.
            result['status'] = 'error'
            result['errors'].append(str(error))
        return self._finish(result)

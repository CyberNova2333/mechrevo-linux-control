"""Session runner and actual watcher callbacks with fake buses/devices only."""
from copy import deepcopy
import contextlib
import io
import json
import os
from pathlib import Path
import sys
import tempfile
import types
import unittest
from unittest.mock import Mock, patch

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'iteglow/src'))
from iteglow import lighting_policy_session as session
from iteglow import lighting_session as watcher
from iteglow.lighting_policy import default_config


class RunnerTest(unittest.TestCase):
    def setUp(self):
        self.config = default_config()
        self.config.update(enabled=True, power_enabled=True, idle_minutes=1)
        self.config['brightness'] = {'keyboard': {'ac': 80, 'dc': 20},
                                      'lightbar': {'ac': 80, 'dc': 20},
                                      'logo': {'ac': 75, 'dc': 25}}
        self.saved = {'keyboard': {'mode': 'spark', 'brightness': 50, 'reactive': True},
                      'lightbar': {'mode': 'wave', 'brightness': 100},
                      'logo': {'mode': 'breathing', 'brightness': 100, 'color': [255, 0, 0]}}
        self.active = True
        self.power = Mock(return_value='ac')
        self.idle = Mock(return_value=0)
        self.writer, self.emit = Mock(return_value=None), Mock()
        self.manual = Mock(side_effect=lambda: session.ManualState(deepcopy(self.saved)))
        self.runner = session.PolicyRunner(active=lambda: self.active,
                                          config=lambda: deepcopy(self.config), manual=self.manual,
                                          power=self.power, idle=self.idle, writer=self.writer,
                                          apply=True, emit=self.emit)

    def test_success_acknowledges_each_zone_and_stable_polls_do_not_rewrite_or_log(self):
        before = deepcopy(self.saved)
        self.assertEqual(self.runner.tick()['applied'], ['keyboard', 'lightbar', 'logo'])
        self.assertEqual(len(self.runner.memory.zones), 3)
        self.runner.tick(); self.runner.tick()
        self.assertEqual(self.writer.call_count, 3)
        self.assertEqual(self.emit.call_count, 1)
        self.assertEqual(self.saved, before)

    def test_failed_zone_only_retries_and_repeated_error_log_is_suppressed(self):
        self.writer.side_effect = lambda target: (_ for _ in ()).throw(OSError('offline')) if target.device == 'keyboard' else None
        self.runner.tick(); self.runner.tick()
        self.assertEqual([zone.device for zone in self.runner.memory.zones], ['lightbar', 'logo'])
        self.assertEqual(self.writer.call_count, 4)
        self.assertEqual(self.emit.call_count, 1)
        self.writer.side_effect = None
        self.assertEqual(self.runner.tick()['applied'], ['keyboard'])

    def test_false_writer_result_is_not_acknowledged(self):
        self.writer.return_value = False
        self.assertEqual(self.runner.tick()['applied'], [])
        self.assertFalse(self.runner.memory.zones)

    def test_default_disabled_never_reads_manual_or_sensors(self):
        self.config = default_config()
        self.assertEqual(self.runner.tick()['status'], 'disabled')
        self.manual.assert_not_called(); self.power.assert_not_called(); self.idle.assert_not_called()
        self.writer.assert_not_called()

    def test_dry_run_does_not_write_or_ack_and_deduplicates_preview(self):
        self.runner.apply = False
        self.assertEqual(len(self.runner.tick()['planned']), 3)
        self.runner.tick()
        self.writer.assert_not_called()
        self.assertFalse(self.runner.memory.zones)
        self.assertEqual(self.emit.call_count, 1)
        self.power.return_value = 'dc'
        self.runner.tick(); self.runner.tick()
        self.assertEqual(self.emit.call_count, 2)
        self.writer.assert_not_called()

    def test_manual_state_reloaded_before_each_zone_changed_zone_is_skipped(self):
        snapshots = [deepcopy(self.saved)]
        changed = deepcopy(self.saved)
        changed['keyboard'] = {'mode': 'off'}
        snapshots.extend([deepcopy(changed)] * 3)
        self.manual.side_effect = [session.ManualState(value) for value in snapshots]
        result = self.runner.tick()
        self.assertEqual(result['applied'], ['lightbar', 'logo'])
        self.assertEqual(result['skipped'], ['keyboard: manual state changed'])
        self.assertEqual([zone.device for zone in self.runner.memory.zones], ['lightbar', 'logo'])

    def test_losing_session_during_write_drops_ownership_and_stops_remaining_zones(self):
        def write(_):
            self.active = False
        self.writer.side_effect = write
        self.assertEqual(self.runner.tick()['status'], 'inactive')
        self.assertEqual(self.writer.call_count, 1)
        self.assertFalse(self.runner.memory.zones)

    def test_inactive_and_unknown_session_drop_old_ownership_without_writes(self):
        self.runner.tick()
        self.writer.reset_mock(); self.manual.reset_mock()
        self.active = False
        self.assertEqual(self.runner.tick()['status'], 'inactive')
        self.assertFalse(self.runner.memory.zones)
        self.writer.assert_not_called(); self.manual.assert_not_called()
        self.active = True; self.runner.tick(); self.writer.reset_mock()
        self.runner.active = Mock(side_effect=OSError('logind gone'))
        self.assertEqual(self.runner.tick()['status'], 'error')
        self.assertFalse(self.runner.memory.zones)
        self.writer.assert_not_called()

    def test_restore_window_skips_and_finish_resets_before_reapplying_policy(self):
        self.runner.tick()
        self.writer.reset_mock(); self.manual.reset_mock()
        self.runner.begin_restore()
        self.assertEqual(self.runner.tick()['status'], 'manual_restore_pending')
        self.writer.assert_not_called(); self.manual.assert_not_called()
        self.runner.finish_restore()
        self.assertFalse(self.runner.memory.zones)
        self.assertEqual(len(self.runner.tick()['applied']), 3)

    def test_unknown_inputs_preserve_idle_off_then_known_activity_restores(self):
        self.idle.return_value = 61
        self.runner.tick(); self.writer.reset_mock()
        self.idle.side_effect = OSError('idle missing')
        self.power.side_effect = OSError('power missing')
        result = self.runner.tick()
        self.writer.assert_not_called()
        self.assertEqual(len(result['errors']), 2)
        self.assertTrue(all(zone.idle for zone in self.runner.memory.zones))
        self.idle.side_effect = None; self.idle.return_value = 0
        self.power.side_effect = None; self.power.return_value = 'dc'
        self.assertEqual(len(self.runner.tick()['applied']), 3)

    def test_invalid_idle_becomes_unknown_and_does_not_force_wakeup(self):
        self.idle.return_value = 61
        self.runner.tick(); self.writer.reset_mock()
        self.idle.return_value = float('nan')
        self.assertIn('idle: invalid idle sample', self.runner.tick()['errors'])
        self.writer.assert_not_called()

    def test_disable_releases_only_owned_zones_without_reading_sensors(self):
        self.runner.tick(); self.writer.reset_mock(); self.power.reset_mock(); self.idle.reset_mock()
        self.saved['keyboard'] = {'mode': 'off'}
        self.config['enabled'] = False
        self.assertEqual(self.runner.tick()['applied'], ['lightbar', 'logo'])
        self.assertFalse(self.runner.memory.zones)
        self.power.assert_not_called(); self.idle.assert_not_called()


class InputsAndFilesTest(unittest.TestCase):
    def test_dbus_contracts_and_uint64_millisecond_conversion(self):
        system, user = Mock(return_value=(True,)), Mock(return_value=(120500,))
        sampler = session.SessionInputs(system, user, lambda signature, value: (signature, value))
        self.assertEqual(sampler.power(), 'dc')
        self.assertEqual(sampler.idle(), 120.5)
        self.assertEqual(system.call_args.args[:4], ('org.freedesktop.UPower', '/org/freedesktop/UPower',
                                                    'org.freedesktop.DBus.Properties', 'Get'))
        self.assertEqual(user.call_args.args, ('org.gnome.Mutter.IdleMonitor', '/org/gnome/Mutter/IdleMonitor/Core',
                                               'org.gnome.Mutter.IdleMonitor', 'GetIdletime'))
        system.return_value = (False,)
        self.assertEqual(sampler.power(), 'ac')
        for value in ((0,), ('false',), (), (False, True)):
            system.return_value = value
            with self.assertRaises(ValueError): sampler.power()
        for value in ((True,), (-1,), (2**64,), ('1000',)):
            user.return_value = value
            with self.assertRaises(ValueError): sampler.idle()

    def test_missing_config_disabled_and_manual_files_never_rewritten(self):
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary)
            self.assertEqual(session.read_config(root / 'policy.json', os.getuid()), default_config())
            values = {'state.json': {'keyboard': {'mode': 'off'}},
                      'logo.json': {'mode': 'solid', 'brightness': 0, 'color': [1, 2, 3]},
                      'logo-prefs.json': {'mode': 'rainbow', 'brightness': 100, 'color': [255, 0, 0]}}
            for name, value in values.items(): (root / name).write_text(json.dumps(value))
            before = {p.name: (p.read_bytes(), p.stat().st_mtime_ns) for p in root.iterdir()}
            saved = session.read_manual(root, os.getuid())
            self.assertEqual(saved.records['logo'], values['logo.json'])
            self.assertEqual(saved.errors, ())
            self.assertEqual(before, {p.name: (p.read_bytes(), p.stat().st_mtime_ns) for p in root.iterdir()})
            (root / 'state.json').write_text('{invalid')
            saved = session.read_manual(root, os.getuid())
            self.assertEqual(set(saved.records), {'logo'})
            self.assertEqual(len(saved.errors), 1)


class WatcherCallbacksTest(unittest.TestCase):
    def test_actual_watch_callbacks_serialize_manual_restore_sleep_and_reactivation(self):
        """Run main() with a fake GLib loop; no real buses/timers/signals/devices."""
        with tempfile.TemporaryDirectory() as temporary:
            root = Path(temporary); (root / 'iteglow').mkdir()
            (root / 'iteglow/state.json').write_text(json.dumps({'keyboard': {'mode': 'wave', 'brightness': 50}}))
            config = default_config(); config.update(enabled=True, power_enabled=True)
            config['brightness']['keyboard']['dc'] = 20
            policy_path = root / 'policy.json'; policy_path.write_text(json.dumps(config))
            active = [True]; timers = {}; serial = [0]; callback = [None]
            class Reply:
                def __init__(self, values): self.values = values
                def unpack(self): return self.values
            class Bus:
                def call_sync(self, dest, path, interface, method, params, *ignored):
                    if method == 'ListSessions': return Reply(([('s1', os.getuid(), 'user', 'seat0', '/session')],))
                    if method == 'GetAll':
                        return Reply(({'User': (os.getuid(), '/user'), 'Seat': ('seat0', '/seat'),
                                       'Active': active[0], 'Remote': False, 'State': 'active',
                                       'Class': 'user', 'Type': 'wayland'},))
                    if method == 'GetUnit': return Reply(('/unit',))
                    if dest == 'org.freedesktop.systemd1' and method == 'Get': return Reply(('active',))
                    if dest == 'org.freedesktop.UPower': return Reply((True,))
                    if method == 'GetIdletime': return Reply((0,))
                    raise AssertionError((dest, method))
                def signal_subscribe(self, *args): callback[0] = args[-2]; return 1
                def signal_unsubscribe(self, _): pass
            def add(seconds, function):
                serial[0] += 1; timers[serial[0]] = (seconds, function); return serial[0]
            def run_timer(identifier):
                if timers[identifier][1]() is False: timers.pop(identifier, None)
            writer = Mock(return_value=None)
            def restored(): run_timer(next(key for key, (delay, _) in timers.items() if delay == 2))
            def manager(sleeping): callback[0](None, None, None, None, 'PrepareForSleep', Reply((sleeping,)), None)
            def run_loop():
                poll = next(key for key, (delay, _) in timers.items() if delay == 5)
                run_timer(poll); self.assertEqual(writer.call_count, 0)
                restored(); self.assertEqual(writer.call_count, 2)  # Manual then policy.
                run_timer(poll); self.assertEqual(writer.call_count, 2)
                manager(True); run_timer(poll); self.assertEqual(writer.call_count, 2)
                manager(False); run_timer(poll); self.assertEqual(writer.call_count, 2)
                restored(); self.assertEqual(writer.call_count, 4)
                active[0] = False; run_timer(poll); self.assertEqual(writer.call_count, 4)
                active[0] = True; run_timer(poll); self.assertEqual(writer.call_count, 4)
                restored(); self.assertEqual(writer.call_count, 6)
            glib = types.SimpleNamespace(
                Variant=lambda signature, values: Reply(values), Error=type('BusError', (Exception,), {}),
                MainLoop=lambda: types.SimpleNamespace(run=run_loop, quit=lambda: None),
                timeout_add_seconds=add, source_remove=lambda key: timers.pop(key, None),
                unix_signal_add=lambda *_: None, PRIORITY_DEFAULT=0)
            bus = Bus()
            gio = types.SimpleNamespace(bus_get_sync=lambda *_: bus, BusType=types.SimpleNamespace(SYSTEM=0, SESSION=1),
                                        DBusCallFlags=types.SimpleNamespace(NONE=0), DBusSignalFlags=types.SimpleNamespace(NONE=0))
            repository = types.ModuleType('gi.repository'); repository.Gio = gio; repository.GLib = glib
            gi = types.ModuleType('gi'); gi.repository = repository
            with (patch.dict(sys.modules, {'gi': gi, 'gi.repository': repository}),
                  patch.dict(os.environ, {'XDG_RUNTIME_DIR': temporary, 'XDG_STATE_HOME': temporary}),
                  patch.object(session, 'CONFIG_PATH', policy_path),
                  patch.object(watcher, 'apply_action', writer), contextlib.redirect_stdout(io.StringIO())):
                self.assertEqual(watcher.main(['--apply']), 0)
                writer.reset_mock()
                with (patch.object(session, 'PolicyRunner', side_effect=AssertionError('once cannot start policy')),
                      patch.object(session, 'read_config', side_effect=AssertionError('once cannot read policy'))):
                    self.assertEqual(watcher.main(['--once']), 0)
                writer.assert_not_called()
            self.assertEqual(json.loads((root / 'iteglow/state.json').read_text()),
                             {'keyboard': {'mode': 'wave', 'brightness': 50}})


if __name__ == '__main__': unittest.main()

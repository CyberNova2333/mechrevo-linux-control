"""Unit tests for Track 1 WMI mode-key handling, fan safety yield, SmartBalance telemetry, and UI."""
import json
import os
from pathlib import Path
import signal
import sys
import tempfile
import threading
import unittest
from unittest.mock import Mock, patch

source = Path(__file__).resolve().parents[1] / 'iteglow/src'
if str(source) not in sys.path:
    sys.path.insert(0, str(source))
if str(Path(__file__).resolve().parent) not in sys.path:
    sys.path.insert(0, str(Path(__file__).resolve().parent))

import gi
gi.require_version('Gtk', '4.0')
from gi.repository import Gtk
from iteglow import performance
from test_tuning import helper, TuningEC

Gtk.init()


class DummyLockContext:
    def __init__(self, ec):
        self.ec = ec

    def __call__(self, fn, *, writable=True, background=False):
        return fn(self.ec)


class WmiAndSafetyHelperTest(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.old_state = helper.STATE
        self.old_wmi = helper.WMI_STATE
        helper.STATE = Path(self.temp.name) / 'state.json'
        helper.WMI_STATE = Path(self.temp.name) / 'wmi-state.json'
        self.config = {
            'pl1': 45,
            'pl2': 45,
            'cpu_fan': helper.MIN_DUTY.copy(),
            'gpu_fan': helper.MIN_DUTY.copy(),
        }

    def tearDown(self):
        helper.STATE = self.old_state
        helper.WMI_STATE = self.old_wmi
        self.temp.cleanup()

    def test_status_reports_fan_safety_smart_balance_and_wmi_fields(self):
        ec = TuningEC()
        self.assertFalse(helper.status(ec)['fan_safety_protect'])
        self.assertFalse(helper.status(ec)['smart_balance_supported'])
        self.assertFalse(helper.status(ec)['smart_balance_enabled'])

        ec.values[0x7A5] = 0x08
        ec.values[0x7AC] = 0x03
        st = helper.status_with_turbo(ec, {'available': True, 'active': False, 'wmi_bound': True, 'wmi_seq': 7, 'wmi_last_code': 176})
        self.assertTrue(st['fan_safety_protect'])
        self.assertTrue(st['smart_balance_supported'])
        self.assertTrue(st['smart_balance_enabled'])
        self.assertTrue(st['wmi_bound'])
        self.assertEqual(st['wmi_seq'], 7)
        self.assertEqual(st['wmi_last_code'], 176)

    def test_safety_protect_blocks_custom_tuning_and_custom_mode_but_allows_office(self):
        ec = TuningEC()
        ec.values[0x7A5] = 0x08
        with self.assertRaisesRegex(RuntimeError, '高温风扇安全保护'):
            helper.apply_tuning(ec, self.config, pause=lambda _: None)
        with self.assertRaisesRegex(RuntimeError, '高温风扇安全保护'):
            helper.set_mode(ec, 'custom', pause=lambda _: None)
        self.assertEqual(helper.set_mode(ec, 'office', pause=lambda _: None)['mode'], 'office')

    def test_reconcile_safety_protect_yields_custom_without_fighting_fan_ram(self):
        ec = TuningEC()
        ec.values.update({0x7D2: 25, 0x7CC: 129, 0x741: 0x80, 0x7AB: 0xE1, 0x7C5: 3})
        before = ec.values.copy()
        with patch.object(helper, 'check_gpu_capability'), patch.object(helper, 'check_gpu_runtime'):
            helper.apply_tuning(ec, {**self.config, 'gpu_ctgp': 95, 'gpu_db': 10}, pause=lambda _: None)
        ec.writes.clear()

        # EC activates safety protect and alters fan RAM + clears custom fan bit/APPE early.
        ec.values[0x7A5] = 0x08
        ec.values[0xF10] = 200
        ec.values[0x7C6] &= ~0x04
        ec.values[0x741] &= ~0x01

        helper.reconcile_ec(ec)
        self.assertFalse(helper.STATE.exists())
        written_addrs = {a for a, _ in ec.writes}
        self.assertTrue(written_addrs.isdisjoint(range(0xF00, 0xF60)))
        self.assertEqual(ec.values[0x743] & 7, before[0x743] & 7)
        self.assertEqual(ec.values[0x744], before[0x744])
        self.assertEqual(ec.values[0x783], before[0x783])
        self.assertEqual(ec.values[0x784], before[0x784])
        self.assertEqual(ec.values[0x7C5] & 0x80, 0)
        self.assertEqual(ec.values[0x7C6] & 0x04, 0)
        self.assertEqual(ec.values[helper.MODE], before[helper.MODE])

        # Also test WMI 0xBB force_safety even if 0x7a5 bit 3 already cleared before reconcile runs.
        ec2 = TuningEC()
        before2 = ec2.values.copy()
        helper.apply_tuning(ec2, self.config, pause=lambda _: None)
        ec2.writes.clear()
        ec2.values[0xF22] = 199
        helper.reconcile_ec(ec2, force_safety=True)
        self.assertFalse(helper.STATE.exists())
        self.assertTrue({a for a, _ in ec2.writes}.isdisjoint(range(0xF00, 0xF60)))
        self.assertEqual(ec2.values[0x783], before2[0x783])
        self.assertEqual(ec2.values[helper.MODE], before2[helper.MODE])

    def test_wmi_mode_key_releases_turbo_and_cycles_mode_and_exits_custom(self):
        ec = TuningEC()
        order = []

        def fake_release(cmd, optional=False):
            order.append(('release', cmd))
            return {'active': False, 'recovery_needed': False, 'stop_reason': 'mode_key_wmi', 'sequence': 12}

        def fake_lock(fn, *, writable=True, background=False):
            order.append(('lock', writable, background))
            return fn(ec)

        bridge = {'abi': 1, 'active': False, 'stop_reason': 'mode_key_wmi', 'sequence': 12}
        with (patch.object(helper, 'turbo_request', side_effect=fake_release),
              patch.object(helper, 'run_with_lock', side_effect=fake_lock),
              patch.object(helper.time, 'sleep')):
            seq = helper.handle_mode_key_wmi(bridge, None)
            self.assertEqual(seq, 12)
            self.assertEqual([x[0] for x in order], ['release', 'lock'])
            self.assertEqual(helper.mode_name(ec.values), 'balanced')

            # Replaying the same stale sequence under APPE=0 must not double-switch.
            order.clear()
            seq2 = helper.handle_mode_key_wmi(bridge, seq)
            self.assertEqual(seq2, 12)
            self.assertEqual(order, [('lock', True, False)])
            self.assertEqual(helper.mode_name(ec.values), 'balanced')

            # Custom tuning + WMI 0xB0 restores custom tuning and switches to office.
            ec.values[helper.MODE] = 0x30
            helper.apply_tuning(ec, self.config, pause=lambda _: None)
            self.assertTrue(helper.STATE.exists())
            helper.handle_mode_key_wmi({'abi': 1, 'active': False, 'stop_reason': 'user', 'sequence': 13}, seq2)
            self.assertFalse(helper.STATE.exists())
            self.assertEqual(helper.mode_name(ec.values), 'office')

    def test_write_wmi_state_enforces_world_readable_under_strict_umask(self):
        payload = helper.sanitize_wmi_state({
            'wmi_bound': True,
            'wmi_seq': 4,
            'wmi_last_code': 193,
            'wmi_events': [{'seq': 4, 'code': 193, 'ms': 1000}],
            'fan_safety_protect': False,
            'smart_balance_supported': False,
            'smart_balance_enabled': False,
        })
        old_umask = os.umask(0o077)
        try:
            helper.write_wmi_state(payload)
        finally:
            os.umask(old_umask)
        mode = helper.WMI_STATE.stat().st_mode & 0o777
        self.assertEqual(mode, 0o644)
        written = json.loads(helper.WMI_STATE.read_text())
        self.assertEqual(written['wmi_events'], [{'seq': 4, 'code': 193, 'ms': 1000}])
        self.assertFalse(written['smart_balance_supported'])

    def test_main_logo_json_and_toggle_do_not_raise_unbound_local(self):
        class LogoEC(TuningEC):
            def __init__(self):
                super().__init__()
                self.values.update({0x7AB: 0x01, 0x767: 0, 0x7C5: 0, 0x769: 0, 0x76A: 0, 0x76B: 0, 0x78C: 0})

            def write_logo(self, address, value):
                mask = helper.LOGO_MASKS.get(address, 0xFF)
                self.values[address] = (self.values.get(address, 0) & ~mask) | value

        ec = LogoEC()
        with (patch.object(helper, 'check_machine'),
              patch.object(helper, 'run_with_lock', side_effect=lambda fn, **_: fn(ec)),
              patch.object(helper.time, 'sleep')):
            res_json = helper.main(['logo', json.dumps({'mode': 'solid', 'brightness': 50, 'color': [255, 255, 255]})])
            self.assertEqual(res_json['brightness'], 50)
            res_off = helper.main(['logo', 'off'])
            self.assertEqual(res_off['brightness'], 0)
            res_on = helper.main(['logo', 'on'])
            self.assertEqual(res_on['brightness'], 25)

    def test_guard_stops_cleanly_on_sigterm_and_skips_open_during_sleep(self):
        fake_dev = Path(self.temp.name) / 'dev-turbo'
        fake_dev.write_text('')
        fake_sleep = Path(self.temp.name) / 'sleep.json'
        fake_sleep.write_text('{}')

        open_mock = Mock()
        timer = threading.Timer(0.05, lambda: os.kill(os.getpid(), signal.SIGTERM))
        timer.start()
        try:
            with patch.object(helper, 'TURBO_DEVICE', fake_dev), \
                 patch.object(helper, 'SLEEP_MARKER', fake_sleep), \
                 patch.object(helper, 'open_wmi_watch', open_mock):
                helper.guard()
        finally:
            timer.cancel()
        open_mock.assert_not_called()

        fake_sleep.unlink()
        r_fd, w_fd = os.pipe()
        os.write(w_fd, b'x')
        prev_term = signal.getsignal(signal.SIGTERM)

        def fake_read(_fd):
            fake_sleep.write_text('{}')
            return {'wmi_events': [], 'wmi_seq': 0}

        timer2 = threading.Timer(0.05, lambda: os.kill(os.getpid(), signal.SIGTERM))
        try:
            with patch.object(helper, 'TURBO_DEVICE', fake_dev), \
                 patch.object(helper, 'SLEEP_MARKER', fake_sleep), \
                 patch.object(helper, 'open_wmi_watch', return_value=(r_fd, {'wmi_events': [], 'wmi_seq': 0})) as open_watch, \
                 patch.object(helper, 'read_turbo_bridge', side_effect=fake_read), \
                 patch.object(helper, 'write_wmi_state'):
                timer2.start()
                helper.guard()
            open_watch.assert_called_once()
            self.assertIs(signal.getsignal(signal.SIGTERM), prev_term)
            with self.assertRaises(OSError):
                os.fstat(r_fd)
        finally:
            timer2.cancel()
            try:
                os.close(w_fd)
            except OSError:
                pass
            try:
                os.close(r_fd)
            except OSError:
                pass


class PerformancePanelSafetyGuiTest(unittest.TestCase):
    @patch.object(performance.GLib, 'timeout_add_seconds', return_value=123)
    @patch.object(performance.Gio.Subprocess, 'new')
    @patch.object(performance.state, 'load', return_value={})
    def test_panel_displays_fan_safety_and_smart_balance_status(self, _load, _spawn, _timer):
        panel = performance.PerformancePanel()
        payload = {
            'mode': 'turbo',
            'switchable': True,
            'reason': '',
            'tuning_active': False,
            'tuning_recovery_pending': False,
            'tuning': None,
            'gpu_tuning_supported': True,
            'gpu_tuning_range': [65, 115],
            'gpu_db_range': [5, 25],
            'fan_safety_protect': False,
            'smart_balance_supported': False,
            'smart_balance_enabled': False,
            'turbo': {
                'available': True, 'active': False, 'variant': None,
                'recovery_needed': False, 'policy_uncertain': False,
                'standard_turbo': True, 'round_adapter': True,
                'combined_supported': True, 'read_rc': 0,
            },
        }
        process = Mock()
        process.communicate_utf8_finish.return_value = (True, json.dumps(payload), '')
        process.get_successful.return_value = True
        panel._finished(process, None, ('status', 123))
        self.assertIn('正常', panel.safety_label.get_text())
        self.assertIn('本机固件不支持 (0x7ac bit0=0)', panel.smart_balance_label.get_text())
        self.assertTrue(panel.tuning.apply_button.get_sensitive())
        custom_btn = dict(panel.buttons)['custom']
        self.assertTrue(custom_btn.get_sensitive())

        payload['fan_safety_protect'] = True
        process.communicate_utf8_finish.return_value = (True, json.dumps(payload), '')
        panel._finished(process, None, ('status', 123))
        self.assertIn('EC 高温安全保护激活', panel.safety_label.get_text())
        self.assertFalse(panel.tuning.apply_button.get_sensitive())
        self.assertFalse(custom_btn.get_sensitive())


if __name__ == '__main__':
    unittest.main()

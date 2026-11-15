"""Real Gtk widgets, with all device/helper I/O replaced; run under Xvfb."""
import unittest
from unittest.mock import Mock, patch

from iteglow import gui, performance
from gi.repository import Gtk

Gtk.init()


def status(**changes):
    value = {'mode': 'turbo', 'switchable': True, 'reason': '',
             'tuning_active': False, 'tuning_recovery_pending': False,
             'turbo': {'available': True, 'active': False, 'variant': None,
                       'recovery_needed': False, 'policy_uncertain': False,
                       'standard_turbo': True, 'round_adapter': True,
                       'combined_supported': True, 'read_rc': 0}}
    value['turbo'].update(changes)
    return value


class TurboGuiTest(unittest.TestCase):
    def setUp(self):
        self.request = Mock()
        self.editor = performance.TurboEditor(self.request)

    def enabled(self):
        return {key for key, button in self.editor.buttons.items() if button.get_sensitive()}

    def test_construction_and_status_only_read_and_explicit_click_dispatches(self):
        self.assertEqual(self.enabled(), set())
        self.editor.update_status(status())
        self.assertEqual(self.enabled(), {'quiet', 'extreme'})
        self.request.assert_not_called()
        self.editor.buttons['quiet'].emit('clicked')
        self.request.assert_called_once_with(['turbo', 'quiet'])

    def test_active_lease_recovery_flag_is_normal_not_pending_failure(self):
        self.editor.update_status(status(active=True, variant='quiet', recovery_needed=True))
        self.assertEqual(self.enabled(), {'extreme', 'off'})
        self.assertIn('静音狂暴', self.editor.current.get_text())
        self.assertTrue(self.editor.buttons['quiet'].has_css_class('selected'))

    def test_inactive_pending_recovery_only_allows_retry_release(self):
        self.editor.update_status(status(recovery_needed=True))
        self.assertEqual(self.enabled(), {'off'})
        self.assertIn('恢复尚未完成', self.editor.current.get_text())
        self.assertFalse(self.editor.buttons['off'].has_css_class('selected'))

    def test_unknown_active_variant_never_claims_extreme_and_still_allows_release(self):
        self.editor.update_status(status(active=True, variant='future-policy', recovery_needed=True))
        self.assertEqual(self.enabled(), {'off'})
        self.assertNotIn('高能狂暴', self.editor.current.get_text())
        self.assertIn('尚未确认', self.editor.current.get_text())
        self.assertFalse(any(b.has_css_class('selected') for b in self.editor.buttons.values()))

    def test_unknown_or_unsupported_caps_do_not_enable_new_policy(self):
        for field, value in [('read_rc', -5), ('read_rc', False), ('combined_supported', False),
                             ('round_adapter', 1), ('standard_turbo', 'true'),
                             ('active', 'true'), ('policy_uncertain', None)]:
            with self.subTest(field=field, value=value):
                self.editor.update_status(status(**{field: value}))
                self.assertEqual(self.enabled(), set())
        for field in ('read_rc', 'combined_supported', 'active', 'recovery_needed'):
            value = status()
            del value['turbo'][field]
            self.editor.update_status(value)
            self.assertEqual(self.enabled(), set())
        self.editor.update_status(status(active=True, variant='quiet', combined_supported=False))
        self.assertEqual(self.enabled(), {'off'})
        self.assertIn('未报告支持', self.editor.current.get_text())

    def test_dc_custom_tuning_and_uncertainty_gate_new_policy(self):
        for field, value in [('mode', 'custom'), ('tuning_active', True),
                             ('tuning_recovery_pending', True), ('tuning_active', None)]:
            data = status()
            data[field] = value
            self.editor.update_status(data)
            self.assertEqual(self.enabled(), set())
        for changes in ({'round_adapter': False}, {'standard_turbo': False},
                        {'available': False}, {'policy_uncertain': True},
                        {'active': True, 'variant': 'quiet', 'policy_uncertain': True}):
            self.editor.update_status(status(**changes))
            self.assertEqual(self.enabled(), set())

    @patch.object(performance.GLib, 'timeout_add_seconds', return_value=123)
    @patch.object(performance.Gio.Subprocess, 'new')
    @patch.object(performance.state, 'load', return_value={})
    def test_panel_startup_only_requests_status_and_busy_disables_every_writer(self, load, spawn, timer):
        panel = performance.PerformancePanel()
        self.assertEqual(spawn.call_args[0][0][-1], 'status')
        self.assertEqual(spawn.call_count, 1)
        self.assertTrue(panel.busy)
        self.assertTrue(all(not b.get_sensitive() for _, b in panel.buttons))
        self.assertTrue(all(not b.get_sensitive() for b in panel.turbo.buttons.values()))
        self.assertFalse(panel.tuning.apply_button.get_sensitive())
        self.assertFalse(panel.tuning.reset_button.get_sensitive())
        panel.request(['turbo', 'extreme'])
        self.assertEqual(spawn.call_count, 1)

    @patch.object(gui.devices, 'available_devices', return_value=[])
    @patch.object(gui, 'PerformancePanel', side_effect=lambda: Gtk.Box())
    @patch.object(gui, 'LogoPanel', side_effect=lambda: Gtk.Box())
    @patch('iteglow.lighting_policy_gui.load_config')
    def test_policy_page_reachable_from_actual_main_navigation(self, config, logo, perf, devices):
        from iteglow.lighting_policy import default_config
        config.return_value = default_config()
        app = gui.App()
        window = Gtk.Window()
        try:
            # Exercise the actual page/nav construction without registering a
            # desktop app or activating portals/keyrings in this mock test.
            with patch.object(gui.Gtk, 'ApplicationWindow', return_value=window):
                app.do_activate()
            self.assertEqual(app.stack.get_visible_child_name(), 'keyboard')
            app.nav['lighting-policy'].emit('clicked')
            self.assertEqual(app.stack.get_visible_child_name(), 'lighting-policy')
            self.assertTrue(app.nav['lighting-policy'].has_css_class('selected'))
            scroll = app.stack.get_child_by_name('lighting-policy')
            self.assertIsNotNone(scroll.get_child())
        finally:
            window.destroy()

    def test_closing_and_reopening_main_window_discards_old_devices_and_timers(self):
        from iteglow.lighting_policy import default_config
        class Device(Gtk.Box):
            def __init__(self, name):
                super().__init__()
                self.pending = 9001
                self.applied = self.closed = 0
            def apply(self): self.applied += 1
            def close(self): self.closed += 1
        class Logo(Gtk.Box):
            def __init__(self):
                super().__init__()
                self.closed = 0
            def close(self): self.closed += 1
        windows = []
        def window_factory(**kwargs):
            value = Gtk.Window(title=kwargs['title'])
            windows.append(value)
            return value
        app = gui.App()
        with patch.object(gui.devices, 'available_devices', return_value=[('', '', 'keyboard')]), \
                patch.object(gui, 'DevicePanel', Device), patch.object(gui, 'LogoPanel', Logo), \
                patch.object(gui, 'PerformancePanel', side_effect=lambda: Gtk.Box()), \
                patch.object(gui.Gtk, 'ApplicationWindow', side_effect=window_factory), \
                patch('iteglow.lighting_policy_gui.load_config', return_value=default_config()), \
                patch.object(gui.GLib, 'source_remove'):
            try:
                app.do_activate()
                old_device, old_logo = app.panels[0], app.logo
                windows[0].emit('close-request')
                self.assertEqual((old_device.applied, old_device.closed, old_logo.closed), (1, 1, 1))
                self.assertEqual(app.panels, [])
                self.assertIsNone(app.logo)
                app.do_activate()
                self.assertEqual(len(app.panels), 1)
                self.assertIsNot(app.panels[0], old_device)
                self.assertIsNot(app.logo, old_logo)
                windows[1].emit('close-request')
                self.assertEqual((old_device.closed, old_logo.closed), (1, 1))
            finally:
                for window in windows: window.destroy()


if __name__ == '__main__':
    unittest.main()

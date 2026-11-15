"""Async Logo lifetime tests: real user locks, mocked helper and GTK timers."""
import json
import os
from pathlib import Path
import tempfile
import threading
import unittest
from unittest.mock import Mock, patch

from iteglow import gui, logo
from iteglow.zone_lock import ZoneTransaction
from gi.repository import Gtk

Gtk.init()


class LogoTransactionTest(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.patches = []
        def replace(*args, **kwargs):
            value = patch(*args, **kwargs)
            self.patches.append(value)
            return value.start()
        self.env = patch.dict(os.environ, {'XDG_RUNTIME_DIR': self.temp.name})
        self.env.start()
        self.app = replace('iteglow.logo.Gio.Application.get_default').return_value
        self.spawn = replace('iteglow.logo.Gio.Subprocess.new')
        self.seconds = replace('iteglow.logo.GLib.timeout_add_seconds', return_value=1001)
        replace('iteglow.logo.GLib.timeout_add', return_value=1002)
        replace('iteglow.logo.GLib.source_remove')
        replace('iteglow.logo.GLib.MainContext.default').return_value.find_source_by_id.return_value = False
        self.save = replace('iteglow.logo.state.write')
        self.panel = logo.LogoPanel()

    def tearDown(self):
        self.panel.close()
        self.panel.release_zone(); self.panel.release_hold()
        for value in reversed(self.patches):
            value.stop()
        self.env.stop(); self.temp.cleanup()

    def can_acquire_elsewhere(self):
        result = []
        def probe():
            transaction = ZoneTransaction('logo')
            result.append(transaction.acquire(blocking=False))
            transaction.release()
        thread = threading.Thread(target=probe)
        thread.start(); thread.join(1)
        self.assertFalse(thread.is_alive())
        return result[0]

    def finish(self, *, successful=True, brightness=0):
        process = self.spawn.return_value
        data = {'mode': 'solid', 'brightness': brightness, 'color': [255, 0, 0],
                'logo_enabled': brightness != 0}
        process.get_successful.return_value = successful
        process.communicate_utf8_finish.return_value = (True, json.dumps(data), 'helper failure')
        callback = process.communicate_utf8_async.call_args.args
        callback[2](process, None, callback[3])

    def test_holds_zone_through_async_write_and_manual_readback_save(self):
        self.panel.set_enabled(False)
        self.assertFalse(self.can_acquire_elsewhere())
        checked = []
        self.save.side_effect = lambda *_: checked.append(self.can_acquire_elsewhere())
        self.finish()
        self.assertEqual(checked, [False])
        self.assertEqual(self.save.call_args.args[1]['brightness'], 0)
        self.assertTrue(self.can_acquire_elsewhere())
        self.app.hold.assert_called_once(); self.app.release.assert_called_once()

    def test_closed_window_waits_for_inflight_helper_then_saves_and_releases(self):
        self.panel.set_enabled(False)
        self.panel.close()
        self.assertFalse(self.can_acquire_elsewhere())
        self.app.release.assert_not_called()
        self.finish()
        self.save.assert_called_once()
        self.assertTrue(self.can_acquire_elsewhere())
        self.app.release.assert_called_once()

    def test_helper_error_or_timeout_does_not_save_and_releases(self):
        self.panel.set_enabled(False)
        timeout_callback = self.seconds.call_args.args[1]
        self.assertFalse(timeout_callback())
        self.spawn.return_value.force_exit.assert_called_once()
        self.assertFalse(self.can_acquire_elsewhere())
        self.finish(successful=False)
        self.save.assert_not_called()
        self.assertTrue(self.can_acquire_elsewhere())
        self.app.release.assert_called_once()

    def test_save_failure_still_releases_and_reports_applied_but_unsaved(self):
        self.panel.set_enabled(False)
        self.save.side_effect = OSError('read only')
        self.finish()
        self.assertIn('保存恢复设置失败', self.panel.message.get_text())
        self.assertTrue(self.can_acquire_elsewhere())

    def test_nonblocking_wait_coalesces_newest_manual_request(self):
        held, release = threading.Event(), threading.Event()
        def owner():
            with ZoneTransaction('logo'):
                held.set(); release.wait(3)
        thread = threading.Thread(target=owner)
        thread.start(); self.assertTrue(held.wait(1))
        try:
            self.panel.apply()
            self.spawn.assert_not_called()
            self.panel.set_enabled(False)
            self.assertTrue(self.panel.retry_zone())
            self.spawn.assert_not_called()
        finally:
            release.set(); thread.join(1)
        self.assertFalse(self.panel.retry_zone())
        self.assertEqual(self.spawn.call_args.args[0][-2:], ['logo', 'off'])
        self.finish()
        self.assertTrue(self.can_acquire_elsewhere())

    def test_close_while_waiting_cancels_without_calling_helper(self):
        held, release = threading.Event(), threading.Event()
        def owner():
            with ZoneTransaction('logo'):
                held.set(); release.wait(3)
        thread = threading.Thread(target=owner)
        thread.start(); self.assertTrue(held.wait(1))
        try:
            self.panel.set_enabled(False)
            self.panel.close()
            self.spawn.assert_not_called()
            self.app.release.assert_called_once()
            self.assertIsNone(self.panel.zone)
        finally:
            release.set(); thread.join(1)

    def test_reopened_logo_panel_cannot_reenter_old_inflight_transaction(self):
        self.panel.set_enabled(False)
        self.panel.close()
        other = logo.LogoPanel()
        try:
            other.set_enabled(False)
            self.assertEqual(self.spawn.call_count, 1)
            self.assertIsNotNone(other.zone_retry)
            self.finish()  # Closed old panel still saves before releasing.
            self.assertFalse(other.retry_zone())
            self.assertEqual(self.spawn.call_count, 2)
            self.finish()
        finally:
            other.close(); other.release_zone(); other.release_hold()
        self.assertTrue(self.can_acquire_elsewhere())


if __name__ == '__main__':
    unittest.main()

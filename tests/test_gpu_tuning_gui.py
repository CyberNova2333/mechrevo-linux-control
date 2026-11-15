"""Custom GPU controls use only explicitly reported, qualified capabilities."""
import json
import sys
from pathlib import Path
import unittest
from unittest.mock import Mock, patch

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'iteglow/src'))
from iteglow import performance
from gi.repository import Gtk
Gtk.init()


class GpuTuningGuiTest(unittest.TestCase):
    def setUp(self):
        self.request = Mock()
        with patch.object(performance.state, 'load', return_value={}):
            self.editor = performance.TuningEditor(self.request)

    def status(self, **changes):
        result = {'switchable': True, 'tuning_active': False,
                  'tuning_recovery_pending': False, 'gpu_tuning_supported': True,
                  'gpu_tuning_range': [65, 115], 'gpu_db_range': [5,25]}
        result.update(changes)
        self.editor.update_status(result)

    def test_construction_and_status_do_not_apply_and_gpu_is_opt_in(self):
        self.assertFalse(self.editor.gpu_enabled.get_active())
        self.status()
        self.request.assert_not_called()
        self.assertTrue(self.editor.gpu_enabled.get_sensitive())
        self.assertFalse(self.editor.gpu_power.get_sensitive())
        with patch.object(performance.state, 'write'):
            self.editor.apply(None)
        config = json.loads(self.request.call_args.args[0][1])
        self.assertNotIn('gpu_ctgp', config)

    def test_explicit_gpu_apply_includes_target(self):
        self.status(); self.editor.gpu_enabled.set_active(True)
        self.editor.gpu_power.set_value(100)
        with patch.object(performance.state, 'write'):
            self.editor.apply(None)
        self.assertEqual(json.loads(self.request.call_args.args[0][1])['gpu_ctgp'], 100)
        self.assertEqual(json.loads(self.request.call_args.args[0][1])['gpu_db'], 0)

    def test_dynamic_boost_requires_gpu_opt_in_and_only_emits_selected_increment(self):
        self.status()
        self.assertFalse(self.editor.gpu_db_enabled.get_sensitive())
        self.editor.gpu_enabled.set_active(True); self.editor.gpu_db_enabled.set_active(True)
        self.editor.gpu_db.set_value(15)
        with patch.object(performance.state, 'write'):
            self.editor.apply(None)
        self.assertEqual(json.loads(self.request.call_args.args[0][1])['gpu_db'], 15)
        self.editor.gpu_enabled.set_active(False)
        self.assertFalse(self.editor.gpu_db.get_sensitive())

    def test_lost_capability_blocks_saved_gpu_request_but_can_be_unchecked(self):
        self.status(); self.editor.gpu_enabled.set_active(True)
        self.status(gpu_tuning_supported=False)
        self.assertFalse(self.editor.apply_button.get_sensitive())
        self.assertTrue(self.editor.gpu_enabled.get_sensitive())
        self.editor.apply(None); self.request.assert_not_called()
        self.editor.gpu_enabled.set_active(False)
        self.assertTrue(self.editor.apply_button.get_sensitive())
        self.assertFalse(self.editor.gpu_enabled.get_sensitive())

    def test_unknown_or_changed_range_never_enables_unqualified_gpu_control(self):
        for value in (None, [0, 255], [100, 115], [True, 115]):
            self.status(gpu_tuning_range=value)
            self.assertFalse(self.editor.gpu_enabled.get_sensitive())
        self.status(gpu_tuning_supported=1)
        self.assertFalse(self.editor.gpu_enabled.get_sensitive())

    def test_readback_identifies_applied_gpu_and_busy_blocks_apply(self):
        config = {'pl1': 35, 'pl2': 35, 'cpu_fan': performance.MIN_DUTY,
                  'gpu_fan': performance.MIN_DUTY, 'gpu_ctgp': 110}
        self.status(tuning_active=True, tuning=config)
        self.assertTrue(self.editor.gpu_enabled.get_active())
        self.assertEqual(self.editor.gpu_power.get_value_as_int(), 110)
        self.assertIn('GPU 110 W', self.editor.validation.get_text())
        self.editor.set_available(False, False)
        self.assertFalse(self.editor.apply_button.get_sensitive())
        self.assertFalse(self.editor.reset_button.get_sensitive())


if __name__ == '__main__':
    unittest.main()

import importlib.machinery
import importlib.util
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

loader=importlib.machinery.SourceFileLoader('powerd_sleep',str(Path(__file__).resolve().parents[1]/'system/mechrevo-powerd-sleep'))
spec=importlib.util.spec_from_loader(loader.name,loader)
sleep=importlib.util.module_from_spec(spec);loader.exec_module(sleep)


class PowerdSleepTest(unittest.TestCase):
    def setUp(self):
        self.folder=tempfile.TemporaryDirectory()
        self.addCleanup(self.folder.cleanup)
        marker=patch.object(sleep,'MARKER',Path(self.folder.name)/'active')
        marker.start();self.addCleanup(marker.stop)

    def test_inactive_service_is_not_started_on_resume(self):
        with patch.object(sleep,'run',return_value=3) as run:
            sleep.main('pre');sleep.main('post')
        self.assertEqual(run.call_count,1)
        self.assertFalse(sleep.MARKER.exists())

    def test_healthy_resume_restores_previously_running_service(self):
        with patch.object(sleep,'run',return_value=0) as run:
            sleep.main('pre')
            self.assertEqual(sleep.MARKER.stat().st_mode&0o777,0o600)
            sleep.main('post')
        self.assertEqual(run.call_args_list[-1].args[0],['/usr/bin/systemctl','--no-block','start',sleep.UNIT])
        self.assertFalse(sleep.MARKER.exists())

    def test_corrupt_acpi_blocks_restart(self):
        sleep.MARKER.write_text('active\n')
        with patch.object(sleep,'run',return_value=1) as run:sleep.main('post')
        self.assertEqual(run.call_count,1)
        self.assertIn('--capture',run.call_args.args[0])
        self.assertFalse(sleep.MARKER.exists())

    def test_failed_stop_preserves_recovery_intent(self):
        with patch.object(sleep,'run',side_effect=[0,1]):
            with self.assertRaises(RuntimeError):sleep.main('pre')
        self.assertTrue(sleep.MARKER.exists())

    def test_restart_queue_failure_is_reported(self):
        sleep.MARKER.write_text('active\n')
        with patch.object(sleep,'run',side_effect=[0,1]):
            with self.assertRaises(RuntimeError):sleep.main('post')
        self.assertTrue(sleep.MARKER.exists())

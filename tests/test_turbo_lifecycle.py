"""PM contracts use injected processes and state: no hardware/systemd access."""
from copy import deepcopy
from contextlib import contextmanager
import importlib.util
from pathlib import Path
import sys
from types import SimpleNamespace
import unittest
from unittest.mock import patch

SOURCE = Path(__file__).resolve().parents[1] / "system"
sys.path.insert(0, str(SOURCE))
spec = importlib.util.spec_from_file_location("turbo_lifecycle_test_target", SOURCE / "mechrevo_turbo_lifecycle.py")
lifecycle = importlib.util.module_from_spec(spec)
spec.loader.exec_module(lifecycle)
from mechrevo_turbo_backend import KERNEL, DMI


class MemoryMarker:
    def __init__(self):
        self.record = None
        self.history = []

    def save(self, record):
        self.record = deepcopy(record)
        self.history.append(deepcopy(record))

    def load(self):
        return deepcopy(self.record)

    def clear(self):
        self.record = None


class LifecycleTest(unittest.TestCase):
    def setUp(self):
        self.events = []
        self.marker = MemoryMarker()
        self.active_state, self.loaded = "active", True
        self.boot = "test-boot"
        self.machine = {"kernel_release": KERNEL, "dmi": dict(DMI)}
        self.digest = "1" * 64
        self.acpi_ok, self.evidence_ok, self.loaded_version_ok = True, True, True
        self.fail_suspend, self.fail_stop, self.fail_unload, self.fail_queue = False, False, False, False

        def machine():
            self.events.append("identity")
            return deepcopy(self.machine)

        def healthy():
            self.events.append("healthy")
            if not self.acpi_ok:
                raise RuntimeError("unhealthy ACPI")

        def qualification(*, require_loaded):
            self.events.append("qualified_loaded" if require_loaded else "qualified_disk")
            if not self.evidence_ok or (require_loaded and not self.loaded_version_ok):
                raise RuntimeError("unqualified module")
            return {"driver_sha256": self.digest}

        def run(args, **kwargs):
            self.assertLessEqual(kwargs["timeout"], 20)
            if args[:3] == ["/usr/bin/systemctl", "show", "--property=ActiveState"]:
                return SimpleNamespace(returncode=0, stdout=self.active_state, stderr="")
            if args == ["/usr/local/libexec/mechrevo-turbo-helper", "suspend"]:
                self.events.append("suspend")
                return SimpleNamespace(returncode=int(self.fail_suspend), stdout="", stderr="release failed")
            if args == ["/usr/bin/systemctl", "stop", lifecycle.UNIT]:
                self.events.append("stop")
                self.assertIsNotNone(self.marker.record, "intent must be saved before stopping")
                if not self.fail_stop:
                    self.active_state = "inactive"
                return SimpleNamespace(returncode=int(self.fail_stop), stdout="", stderr="stop failed")
            if args == ["/usr/sbin/modprobe", "-r", lifecycle.MODULE]:
                self.events.append("unload")
                if not self.fail_unload:
                    self.loaded = False
                return SimpleNamespace(returncode=int(self.fail_unload), stdout="", stderr="recovery pinned")
            if args == ["/usr/sbin/modprobe", lifecycle.MODULE]:
                self.events.append("load")
                self.loaded = True
                return SimpleNamespace(returncode=0, stdout="", stderr="")
            if args == ["/usr/bin/systemctl", "--no-block", "start", lifecycle.UNIT]:
                self.events.append("queue_start")
                return SimpleNamespace(returncode=int(self.fail_queue), stdout="", stderr="queue failed")
            self.fail(f"unexpected process or policy action: {args}")

        self.manager = lifecycle.Lifecycle(run=run, marker=self.marker, machine=machine,
                                           healthy=healthy, qualification=qualification,
                                           loaded=lambda: self.loaded, boot_id=lambda: self.boot)

    def test_pre_stops_and_unloads_before_sleep_then_post_only_queues_service(self):
        self.manager.pre()
        self.assertEqual(self.events[-3:], ["suspend", "stop", "unload"])
        self.assertFalse(self.loaded)
        self.assertTrue(self.marker.record["was_active"])
        self.assertEqual(self.marker.record["phase"], "quiesced")
        self.events.clear()
        self.manager.post()
        self.assertEqual(self.events, ["identity", "healthy", "qualified_disk", "queue_start"])
        self.assertFalse(self.loaded, "post never loads a module before systemd's powerd ordering")
        self.assertEqual(self.marker.record["phase"], "resume_queued")

    def test_prepare_gates_fresh_load_and_clears_resume_marker_only_after_validation(self):
        self.manager.pre()
        self.manager.post()
        self.events.clear()
        result = self.manager.prepare()
        self.assertEqual(self.events, ["identity", "qualified_disk", "healthy", "load", "qualified_loaded"])
        self.assertFalse(result["policy_selected"])
        self.assertIsNone(self.marker.record)

    def test_unhealthy_resume_never_loads_or_starts_and_retains_failure(self):
        self.manager.pre()
        self.events.clear()
        self.acpi_ok = False
        with self.assertRaisesRegex(RuntimeError, "unhealthy"):
            self.manager.post()
        self.assertEqual(self.events, ["identity", "healthy"])
        self.assertFalse(self.loaded)
        self.assertEqual(self.marker.record["phase"], "resume_failed")

    def test_unhealthy_boot_never_loads_module(self):
        self.loaded = False
        self.acpi_ok = False
        with self.assertRaisesRegex(RuntimeError, "unhealthy"):
            self.manager.prepare()
        self.assertNotIn("load", self.events)

    def test_wrong_identity_or_disk_evidence_prevents_boot_module_load(self):
        self.loaded = False
        self.machine["kernel_release"] = "6.9.0"
        with self.assertRaises(RuntimeError):
            self.manager.prepare()
        self.assertEqual(self.events, ["identity"])
        self.machine["kernel_release"] = KERNEL
        self.evidence_ok = False
        with self.assertRaises(RuntimeError):
            self.manager.prepare()
        self.assertNotIn("load", self.events)

    def test_failed_loaded_validation_removes_new_module(self):
        self.loaded = False
        self.loaded_version_ok = False
        with self.assertRaisesRegex(RuntimeError, "unqualified"):
            self.manager.prepare()
        self.assertEqual(self.events[-3:], ["load", "qualified_loaded", "unload"])
        self.assertFalse(self.loaded)

    def test_pre_also_unloads_module_when_daemon_was_inactive(self):
        self.active_state = "inactive"
        self.manager.pre()
        self.assertFalse(self.loaded)
        self.assertNotIn("suspend", self.events)
        self.events.clear()
        result = self.manager.post()
        self.assertEqual(result["reason"], "previously_inactive")
        self.assertEqual(self.events, [])
        self.assertIsNone(self.marker.record)

    def test_helper_release_failure_still_uses_fd_close_and_verified_unload(self):
        self.fail_suspend = True
        result = self.manager.pre()
        self.assertTrue(result["quiesced"])
        self.assertTrue(result["warnings"])
        self.assertFalse(self.loaded)

    def test_cannot_stop_daemon_fails_sleep_and_preserves_original_intent(self):
        self.fail_stop = True
        with self.assertRaisesRegex(RuntimeError, "尚未停止"):
            self.manager.pre()
        self.assertNotIn("unload", self.events)
        self.assertEqual(self.marker.record["phase"], "pre_failed")
        self.assertTrue(self.marker.record["was_active"])

    def test_failed_unload_blocks_sleep_and_retry_preserves_running_intent(self):
        self.fail_unload = True
        with self.assertRaisesRegex(RuntimeError, "recovery pinned"):
            self.manager.pre()
        self.assertEqual(self.active_state, "inactive")
        self.assertEqual(self.marker.record["phase"], "pre_failed")
        self.fail_unload = False
        self.manager.pre()
        self.assertTrue(self.marker.record["was_active"])
        self.assertEqual(self.marker.record["phase"], "quiesced")

    def test_prepare_cannot_race_quiesced_sleep(self):
        self.manager.pre()
        self.events.clear()
        with self.assertRaisesRegex(RuntimeError, "睡眠清理"):
            self.manager.prepare()
        self.assertEqual(self.events, [])

    def test_sleep_cleanup_proceeds_when_evidence_missing_but_resume_is_blocked(self):
        self.evidence_ok = False
        self.manager.pre()
        self.assertFalse(self.loaded)
        self.assertIsNone(self.marker.record["driver_sha256"])
        self.evidence_ok = True
        with self.assertRaisesRegex(RuntimeError, "哈希不一致"):
            self.manager.post()
        self.assertNotIn("queue_start", self.events)

    def test_resume_identity_boot_and_driver_drift_do_not_restart(self):
        for field in ("kernel", "boot", "driver"):
            with self.subTest(field=field):
                self.setUp()
                self.manager.pre()
                self.events.clear()
                if field == "kernel":
                    self.machine["kernel_release"] = "6.9.0"
                elif field == "boot":
                    self.boot = "different-boot"
                else:
                    self.digest = "2" * 64
                with self.assertRaises(RuntimeError):
                    self.manager.post()
                self.assertNotIn("queue_start", self.events)
                self.assertNotIn("load", self.events)

    def test_repeated_pre_preserves_intent_but_new_pre_after_skipped_start_recaptures(self):
        self.manager.pre()
        self.manager.pre()
        self.assertTrue(self.marker.record["was_active"])
        self.manager.post()
        self.manager.pre()  # main ExecCondition may skip if powerd stayed stopped
        self.assertFalse(self.marker.record["was_active"])
        self.assertEqual(self.marker.record["phase"], "quiesced")

    def test_resume_queue_failure_preserves_recoverable_intent(self):
        self.manager.pre()
        self.fail_queue = True
        with self.assertRaisesRegex(RuntimeError, "queue failed"):
            self.manager.post()
        self.assertEqual(self.marker.record["phase"], "resume_failed")
        self.assertTrue(self.marker.record["was_active"])

    def test_sleep_units_require_cleanup_and_order_powerd_resume_first(self):
        sleep = (SOURCE / "mechrevo-turbo-sleep.service").read_text()
        main = (SOURCE / "mechrevo-turbo.service").read_text()
        self.assertIn("RequiredBy=sleep.target", sleep)
        self.assertIn("Before=sleep.target mechrevo-profile-sleep.service mechrevo-powerd-sleep.service", sleep)
        self.assertIn("ExecStartPre=-/usr/bin/systemctl stop mechrevo-profile-guard.service", sleep)
        self.assertIn("ExecStopPost=-/usr/bin/systemctl --no-block start mechrevo-profile-guard.service", sleep)
        self.assertNotIn("After=mechrevo-turbo.service", sleep)
        self.assertNotIn("ConditionPathExists", sleep)
        self.assertIn("ExecCondition=/usr/bin/systemctl is-active --quiet nvidia-powerd.service", main)
        self.assertIn("ExecStartPre=/usr/local/libexec/mechrevo-turbo-lifecycle prepare", main)
        self.assertIn("nvidia-powerd.service", next(line for line in main.splitlines() if line.startswith("After=")))
        self.assertIn("Also=mechrevo-turbo-sleep.service", main)

    def test_prepare_waits_for_post_handoff_without_retrying_the_operation(self):
        entries = []
        @contextmanager
        def lock(_path):
            entries.append("attempt")
            if len(entries) < 3:
                raise BlockingIOError("post still queuing")
            yield
        with patch.object(lifecycle.os, "geteuid", return_value=0), patch.object(lifecycle, "profile_lock", lock), \
                patch.object(lifecycle.time, "sleep"), patch.object(lifecycle, "Lifecycle") as manager:
            manager.return_value.prepare.return_value = {"prepared": True}
            self.assertEqual(lifecycle.main("prepare"), {"prepared": True})
            self.assertEqual(len(entries), 3)
            manager.return_value.prepare.assert_called_once_with()

    def test_prepare_operation_busy_error_is_not_retried_after_side_effects(self):
        @contextmanager
        def lock(_path):
            yield
        with patch.object(lifecycle.os, "geteuid", return_value=0), patch.object(lifecycle, "profile_lock", lock), \
                patch.object(lifecycle, "Lifecycle") as manager:
            manager.return_value.prepare.side_effect = BlockingIOError("operation failed")
            with self.assertRaises(BlockingIOError):
                lifecycle.main("prepare")
            manager.return_value.prepare.assert_called_once_with()


if __name__ == "__main__":
    unittest.main()

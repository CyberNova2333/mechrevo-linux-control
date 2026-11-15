"""Offline lease-supervisor contracts: no real device, ACPI or service access."""
from contextlib import contextmanager
from copy import deepcopy
from dataclasses import replace
import importlib.util
from pathlib import Path
import sys
import tempfile
import socket
from types import SimpleNamespace
import unittest
from unittest.mock import patch

SOURCE = Path(__file__).resolve().parents[1] / "system/mechrevo_turbo_backend.py"
spec = importlib.util.spec_from_file_location("turbo_backend_test_target", SOURCE)
backend = importlib.util.module_from_spec(spec)
spec.loader.exec_module(backend)


class MemoryJournal:
    def __init__(self):
        self.records = []
        self.current = None
        self.fail = False

    def save(self, record):
        if self.fail:
            raise OSError("journal failed")
        self.current = deepcopy(record)
        self.records.append(self.current)

    def clear(self):
        self.current = None


class Bridge:
    def __init__(self):
        self.value = dict(abi=1, active=False, standard_turbo=True, round_adapter=True,
                          return_present=False, recovery_needed=False, combined_supported=True,
                          game_supported=True, sequence=0, last_rc=0, cstm=0, gatm=0,
                          variant="extreme", last_ack=None, stop_reason="none", regs={"728": 0, "741": 128})
        self.commands = []
        self.fail = set()
        self.replies = {}
        self.recovery_fails = False

    def status(self):
        return deepcopy(self.value)

    def command(self, command):
        self.commands.append(command)
        if command in self.fail:
            raise OSError(command + " failed")
        self.value["sequence"] += 1
        if command.startswith("acquire "):
            self.value.update(active=True, recovery_needed=True, variant=command.split()[1])
        elif command.startswith("variant "):
            self.value["variant"] = command.split()[1]
        elif command == "release":
            self.value.update(active=False, recovery_needed=self.recovery_fails,
                              cstm=0, gatm=0, stop_reason="release")
        elif command.startswith(("combined ", "game ")):
            on = int(command[-1])
            key = "cstm" if command.startswith("combined") else "gatm"
            self.value[key] = on
            self.value.update(return_present=True, last_ack=self.replies.get(command, 0xBB if on else 0xAA))
        return self.status()


class LeaseTest(unittest.TestCase):
    def setUp(self):
        self.bridge, self.journal = Bridge(), MemoryJournal()
        self.locked, self.lock_busy, self.tuning = False, False, False
        self.machine = {"kernel_release": backend.KERNEL, "dmi": dict(backend.DMI)}
        self.health_calls = 0
        self.healthy = True
        self.powerd_active = True
        self.powerd_calls = 0
        def runtime_ready():
            self.health_calls += 1
            if not self.healthy:
                raise RuntimeError("unhealthy ACPI")
        def powerd_ready():
            self.powerd_calls += 1
            if not self.powerd_active:
                raise RuntimeError("powerd inactive")

        @contextmanager
        def lock():
            if self.lock_busy:
                raise BlockingIOError("other owner")
            self.assertFalse(self.locked)
            self.locked = True
            try:
                yield
            finally:
                self.locked = False

        original = self.bridge.command
        def command(value):
            self.assertTrue(self.locked, "every kernel mutation must hold profile lock")
            return original(value)
        self.bridge.command = command
        self.controller = backend.Controller(self.bridge, self.journal, lock,
                                              machine=lambda: self.machine,
                                              tuning_present=lambda: self.tuning,
                                              runtime_ready=runtime_ready,
                                              powerd_ready=powerd_ready)

    def activate(self):
        self.controller.select("quiet")
        self.bridge.commands.clear()

    def test_defaults_inactive_and_does_not_acquire_on_tick(self):
        self.controller.tick(backend.LoadSample(100, 100))
        self.assertEqual(self.bridge.commands, [])
        self.assertIsNone(self.journal.current)

    def test_acquire_journal_precedes_kernel_write(self):
        original = self.bridge.command
        def command(value):
            self.assertEqual(self.journal.current["phase"], "acquiring")
            return original(value)
        self.bridge.command = command
        self.controller.select("quiet")
        self.assertEqual(self.journal.current["phase"], "active")
        self.assertFalse(self.journal.current["acquired_before"]["active"])

    def test_journal_failure_prevents_any_acquire(self):
        self.journal.fail = True
        with self.assertRaises(OSError):
            self.controller.select("quiet")
        self.assertEqual(self.bridge.commands, [])

    def test_changed_kernel_and_dmi_refuse_before_writes(self):
        for machine in ({"kernel_release": "6.9.0", "dmi": dict(backend.DMI)},
                        {"kernel_release": backend.KERNEL, "dmi": {}}):
            self.machine = machine
            with self.assertRaises(RuntimeError):
                self.controller.select("quiet")
        self.assertEqual(self.bridge.commands, [])

    def test_unhealthy_start_or_reselection_is_refused_and_existing_lease_released(self):
        self.healthy = False
        with self.assertRaisesRegex(RuntimeError, "unhealthy"):
            self.controller.select("quiet")
        self.assertEqual(self.bridge.commands, [])
        self.healthy = True
        self.activate()
        self.healthy = False
        with self.assertRaisesRegex(RuntimeError, "unhealthy"):
            self.controller.select("extreme")
        self.assertEqual(self.bridge.commands, ["release"])
        self.assertFalse(self.bridge.value["active"])

    def test_runtime_health_checked_on_each_select_but_not_each_sample(self):
        self.activate()
        self.assertEqual(self.health_calls, 1)
        self.controller.tick(backend.LoadSample(0, 0))
        self.controller.tick()
        self.assertEqual(self.health_calls, 1)
        self.controller.select("extreme")
        self.assertEqual(self.health_calls, 2)

    def test_non_turbo_ac_loss_or_tuning_prevents_acquire(self):
        for key in ("standard_turbo", "round_adapter"):
            self.bridge.value[key] = False
            with self.assertRaises(RuntimeError):
                self.controller.select("quiet")
            self.bridge.value[key] = True
        self.tuning = True
        with self.assertRaises(RuntimeError):
            self.controller.select("quiet")
        self.assertEqual(self.bridge.commands, [])

    def test_mode_key_ac_unplug_and_custom_tuning_release(self):
        for reason in ("standard_turbo", "round_adapter", "tuning"):
            with self.subTest(reason=reason):
                self.activate()
                if reason == "tuning":
                    self.tuning = True
                else:
                    self.bridge.value[reason] = False
                self.controller.tick(backend.LoadSample(100, 100))
                self.assertEqual(self.bridge.commands, ["release"])
                self.assertFalse(self.bridge.value["active"])
                self.assertIsNone(self.journal.current)
                self.tuning = False
                self.bridge.value.update(standard_turbo=True, round_adapter=True)

    def test_kernel_watchdog_release_is_not_automatically_reacquired(self):
        self.activate()
        self.bridge.value.update(active=False, stop_reason="watchdog")
        self.controller.tick(backend.LoadSample(100, 100))
        self.controller.tick(backend.LoadSample(100, 100))
        self.assertEqual(self.bridge.commands, ["release"])

    def test_sleep_restores_and_resume_requires_explicit_selection(self):
        self.activate()
        self.controller.suspend()
        with self.assertRaises(RuntimeError):
            self.controller.select("extreme")
        self.controller.resume()
        self.controller.tick(backend.LoadSample(100, 100))
        self.assertEqual(self.bridge.commands, ["release"])

    def test_busy_profile_lock_does_not_write_or_renew(self):
        self.activate()
        self.lock_busy = True
        with self.assertRaises(BlockingIOError):
            self.controller.tick(backend.LoadSample(100, 100))
        self.assertEqual(self.bridge.commands, [])

    def test_game_off_precedes_combined_on_with_real_ack(self):
        self.activate()
        self.controller.state = backend.SchedulerState(cpu_count=2, combined_gpu_count=2,
                                                       game_gpu_count=3, game_on=True)
        self.controller.tick(backend.LoadSample(100, 100))
        self.assertEqual(self.bridge.commands, ["heartbeat", "game 0", "combined 1"])
        self.assertEqual(self.controller.state.combined_reply, 0xBB)
        self.assertFalse(self.controller.state.game_on)

    def test_oem_game_capability_false_never_calls_game_even_at_full_gpu_load(self):
        self.bridge.value["game_supported"] = False
        self.activate()
        for _ in range(8):
            self.controller.tick(backend.LoadSample(0, 100))
        self.assertFalse(any(command.startswith("game ") for command in self.bridge.commands))
        self.assertEqual(self.controller.state.game_gpu_count, 0)
        self.assertEqual(self.controller.state.combined_reply, 0xAA)

    def test_support_change_during_lease_restores_instead_of_leaving_game_running(self):
        self.activate()
        self.controller.state = backend.SchedulerState(game_on=True)
        self.bridge.value["game_supported"] = False
        self.controller.tick(backend.LoadSample(0, 100))
        self.assertEqual(self.bridge.commands, ["release"])

    def test_missing_combined_support_refuses_acquire(self):
        self.bridge.value["combined_supported"] = False
        with self.assertRaisesRegex(RuntimeError, "联合调度支持"):
            self.controller.select("quiet")
        self.assertEqual(self.bridge.commands, [])

    def test_returned_full_wmi_integer_is_reduced_to_oem_low_byte(self):
        self.activate()
        self.bridge.replies["combined 0"] = 0x1234AA
        self.controller.tick(backend.LoadSample(0, 0))
        self.assertEqual(self.controller.state.combined_reply, 0xAA)

    def test_failed_combined_ack_replans_game_fallback_without_repeating_prefix(self):
        self.activate()
        self.bridge.replies["combined 1"] = 0
        self.controller.state = backend.SchedulerState(cpu_count=2, combined_gpu_count=2,
                                                       game_gpu_count=3, game_on=True)
        self.controller.tick(backend.LoadSample(100, 100))
        self.assertEqual(self.bridge.commands, ["heartbeat", "game 0", "combined 1", "game 1"])
        self.assertEqual(self.controller.state.combined_reply, 0)

    def test_missing_return_never_becomes_invented_success(self):
        self.activate()
        original = self.bridge.command
        def no_return(command):
            result = original(command)
            result["return_present"] = False
            return result
        self.bridge.command = no_return
        with self.assertRaisesRegex(RuntimeError, "ACK缺失"):
            self.controller.tick(backend.LoadSample(0, 0))
        self.assertEqual(self.bridge.commands[-1], "release")
        self.assertEqual(self.controller.state.combined_reply, 0)

    def test_command_failure_releases_without_committing_counters(self):
        self.activate()
        self.bridge.fail.add("combined 0")
        with self.assertRaises(OSError):
            self.controller.tick(backend.LoadSample(0, 0))
        self.assertEqual(self.bridge.commands, ["heartbeat", "combined 0", "release"])
        self.assertEqual(self.controller.state, backend.SchedulerState())

    def test_restore_failure_keeps_recovery_record(self):
        self.activate()
        self.bridge.fail.add("release")
        with self.assertRaises(OSError):
            self.controller.release()
        self.assertEqual(self.journal.current["phase"], "releasing")
        self.assertTrue(self.bridge.value["active"])

    def test_inactive_but_recovery_pending_does_not_clear_journal(self):
        self.activate()
        self.bridge.recovery_fails = True
        with self.assertRaisesRegex(RuntimeError, "恢复未完成"):
            self.controller.release()
        self.assertEqual(self.journal.current["phase"], "releasing")
        self.assertFalse(self.bridge.value["active"])

    def test_variant_change_keeps_original_acquisition_snapshot(self):
        self.activate()
        self.assertTrue(self.bridge.value["recovery_needed"], "active kernel lease owns a recovery snapshot")
        original = deepcopy(self.journal.current["acquired_before"])
        self.controller.select("extreme")
        self.assertEqual(self.bridge.commands, ["variant extreme"])
        self.assertEqual(self.journal.current["acquired_before"], original)

    def test_no_sample_only_renews_and_preserves_counters(self):
        self.activate()
        self.controller.state = backend.SchedulerState(cpu_count=2)
        self.controller.tick()
        self.assertEqual(self.controller.state.cpu_count, 2)
        self.assertEqual(self.bridge.commands, ["heartbeat"])

    def test_powerd_stop_releases_before_heartbeat_and_never_reacquires(self):
        self.activate()
        self.powerd_active = False
        with self.assertRaisesRegex(RuntimeError, "powerd inactive"):
            self.controller.tick(backend.LoadSample(100, 100))
        self.assertEqual(self.bridge.commands, ["release"])
        self.powerd_active = True
        self.controller.tick(backend.LoadSample(100, 100))
        self.assertEqual(self.bridge.commands, ["release"])
        self.assertEqual(self.powerd_calls, 1)

    def test_inactive_ticks_do_not_probe_powerd_or_hash_acpi(self):
        self.controller.tick()
        self.controller.tick()
        self.assertEqual((self.powerd_calls, self.health_calls), (0, 0))

    def test_control_protocol_rejects_unknown_fields_and_commands(self):
        for request in ({"command": "raw", "value": 1}, {"command": "status", "variant": "quiet"},
                        {"command": "select", "variant": "custom"}, {"command": "write"}):
            with self.assertRaises(ValueError):
                backend.dispatch(self.controller, request)
        self.assertEqual(self.bridge.commands, [])


class ProtocolTest(unittest.TestCase):
    def test_preload_qualification_keeps_hash_and_evidence_gates_without_loaded_handle(self):
        module = b"test-qualified-module"
        manifest = {"version": 1, "identity": {"kernel_release": backend.KERNEL, "dmi": dict(backend.DMI)},
                    "driver_sha256": backend.hashlib.sha256(module).hexdigest(),
                    "combined_budget_verified": True, "lease_recovery_verified": True, "oem_ack_verified": True}
        def trusted(path, **_kwargs):
            if path == backend.QUALIFICATION:
                return backend.json.dumps(manifest).encode()
            return module
        def run(argv, **_kwargs):
            return SimpleNamespace(stdout="/test/qualified.ko" if "-n" in argv else "qualified-srcversion")
        with patch.object(backend, "trusted_file", trusted), patch.object(backend.subprocess, "run", run), \
                patch.object(Path, "read_text", side_effect=AssertionError("must not read absent module")):
            self.assertEqual(backend.check_qualification(require_loaded=False), manifest)
            manifest["oem_ack_verified"] = False
            with self.assertRaises(RuntimeError):
                backend.check_qualification(require_loaded=False)
            manifest["oem_ack_verified"] = True
            manifest["driver_sha256"] = "0" * 64
            with self.assertRaisesRegex(RuntimeError, "驱动文件"):
                backend.check_qualification(require_loaded=False)

    def test_postload_qualification_requires_loaded_srcversion_match(self):
        module = b"test-qualified-module"
        manifest = {"version": 1, "identity": {"kernel_release": backend.KERNEL, "dmi": dict(backend.DMI)},
                    "driver_sha256": backend.hashlib.sha256(module).hexdigest(),
                    "combined_budget_verified": True, "lease_recovery_verified": True, "oem_ack_verified": True}
        def trusted(path, **_kwargs):
            return backend.json.dumps(manifest).encode() if path == backend.QUALIFICATION else module
        def run(argv, **_kwargs):
            return SimpleNamespace(stdout="/test/qualified.ko" if "-n" in argv else "qualified-srcversion")
        with patch.object(backend, "trusted_file", trusted), patch.object(backend.subprocess, "run", run), \
                patch.object(Path, "read_text", return_value="unexpected-srcversion"):
            with self.assertRaisesRegex(RuntimeError, "已加载驱动"):
                backend.check_qualification()

    def test_runtime_gate_requires_healthy_json_and_active_powerd(self):
        def result(code=0, text='{"healthy":true}'):
            return SimpleNamespace(returncode=code, stdout=text)
        for replies in ([result(text='{"healthy":false}')], [result(code=1)],
                        [result(), result(code=3)]):
            calls = []
            def run(argv, **kwargs):
                calls.append(argv)
                return replies[len(calls) - 1]
            with self.assertRaises(RuntimeError):
                backend.check_runtime(run)
            self.assertEqual(calls[0][1:], ["--summary"])
        replies = iter([result(), result()])
        self.assertTrue(backend.check_runtime(lambda *a, **kw: next(replies))["healthy"])

    def test_status_schema_rejects_old_bridge_and_wrong_types(self):
        valid = Bridge().status()
        self.assertEqual(backend.validate_status(valid), valid)
        for key, value in (("abi", 0), ("active", 1), ("sequence", -1), ("cstm", 2),
                           ("last_ack", True), ("last_ack", 2**64), ("variant", "custom"),
                           ("read_rc", -5), ("return_present", True)):
            with self.subTest(key=key, value=value), self.assertRaises(RuntimeError):
                backend.validate_status(dict(valid, **{key: value}))

    def test_device_partial_write_and_stale_sequence_are_rejected(self):
        device = backend.Device.__new__(backend.Device)
        device.fd = 123
        valid = Bridge().status()
        with patch.object(device, "status", return_value=valid), patch.object(backend.os, "write", return_value=10):
            with self.assertRaises(RuntimeError):
                device.command("heartbeat")
        with self.assertRaises(ValueError):
            device.command("ec 728 0")

    def test_journal_atomic_roundtrip_and_private_permissions(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "journal.json"
            journal = backend.Journal(path)
            journal.save({"version": 1, "phase": "active"})
            self.assertEqual(path.stat().st_mode & 0o777, 0o600)
            journal.save({"version": 1, "phase": "released"})
            self.assertIn("released", path.read_text())
            self.assertEqual(len(list(Path(directory).iterdir())), 1)
            journal.clear()
            self.assertFalse(path.exists())

    def test_only_dead_trusted_socket_is_removed(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "control.sock"
            original_lstat = Path.lstat
            def pretend_root(target):
                value = original_lstat(target)
                return SimpleNamespace(st_mode=value.st_mode, st_uid=0,
                                       st_dev=value.st_dev, st_ino=value.st_ino)
            listener = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
            listener.bind(str(path))
            path.chmod(0o600)
            listener.listen(1)
            with patch.object(Path, "lstat", pretend_root):
                with self.assertRaisesRegex(RuntimeError, "活跃服务"):
                    backend.prepare_socket(path)
            listener.close()
            self.assertTrue(path.exists())
            with patch.object(Path, "lstat", pretend_root):
                backend.prepare_socket(path)
            self.assertFalse(path.exists())
            path.write_text("must survive")
            with patch.object(Path, "lstat", pretend_root):
                with self.assertRaisesRegex(RuntimeError, "非可信socket"):
                    backend.prepare_socket(path)
            self.assertEqual(path.read_text(), "must survive")


if __name__ == "__main__":
    unittest.main()

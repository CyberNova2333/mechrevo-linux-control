"""Lighting lifecycle tests with saved files and fake writers only."""
import json
import os
from pathlib import Path
import sys
import tempfile
import unittest
from unittest.mock import Mock, patch

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "iteglow/src"))
from iteglow import protocol
from iteglow.lighting_session import (
    HELPER, Lifecycle, apply_action, eligible_session, load_actions, restore_once, validate_hid, validate_logo,
)


class LightingSessionTest(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.directory = Path(self.temporary.name)
        self.uid = os.getuid()

    def tearDown(self):
        self.temporary.cleanup()

    def save(self, name, data):
        (self.directory / name).write_text(json.dumps(data))

    def test_missing_state_does_not_create_defaults(self):
        writer = Mock()
        result = restore_once(self.directory, self.uid, lambda: True, apply=True, writer=writer)
        self.assertEqual(result["planned"], [])
        self.assertEqual(result["errors"], [])
        writer.assert_not_called()

    def test_default_is_dry_run_even_with_injected_writer(self):
        self.save("state.json", {"keyboard": {"mode": "off"}})
        writer = Mock()
        result = restore_once(self.directory, self.uid, lambda: True, writer=writer)
        self.assertEqual(result["planned"], ["keyboard"])
        self.assertEqual(result["restored"], [])
        writer.assert_not_called()

    def test_off_and_zero_brightness_never_send_power_on(self):
        power_on = protocol.lightbar_power_on()[0]
        for device in ("keyboard", "lightbar"):
            for saved in ({"mode": "off"}, {"mode": "wave", "brightness": 0}):
                with self.subTest(device=device, saved=saved):
                    action = validate_hid(device, saved)
                    self.assertNotIn(power_on, action.reports)
                    if device == "keyboard":
                        self.assertEqual(action.reports, tuple(protocol.keyboard_brightness(0)))
                    else:
                        self.assertEqual(action.reports, tuple(protocol.lightbar_off()))

    def test_lightbar_on_has_enable_then_requested_effect(self):
        action = validate_hid("lightbar", {"mode": "static", "brightness": 31,
                                          "color": [1, 2, 3]})
        self.assertEqual(action.reports[0], protocol.lightbar_power_on()[0])
        self.assertEqual(action.reports[-1][1][4], 31)

    def test_logo_off_is_exactly_saved_and_preferences_are_ignored(self):
        self.save("logo-prefs.json", {"mode": "rainbow", "brightness": 100, "color": [255, 0, 0]})
        self.assertEqual(load_actions(self.directory, self.uid), ((), ()))
        saved = {"mode": "solid", "brightness": 0, "color": [1, 2, 3]}
        self.save("logo.json", saved)
        actions, errors = load_actions(self.directory, self.uid)
        self.assertEqual(errors, ())
        self.assertEqual(json.loads(actions[0].logo_json), saved)

    @patch("iteglow.lighting_session.subprocess.run")
    def test_logo_backend_uses_fixed_helper_and_requires_matching_readback(self, run):
        saved = {"mode": "solid", "brightness": 0, "color": [1, 2, 3]}
        action = validate_logo(saved)
        run.return_value = Mock(returncode=0, stdout=json.dumps(saved), stderr="")
        apply_action(action)
        command = run.call_args.args[0]
        self.assertEqual(command[:4], ["/usr/bin/pkexec", "--disable-internal-agent", HELPER, "logo"])
        self.assertEqual(json.loads(command[4]), saved)
        self.assertEqual(run.call_args.kwargs["timeout"], 5)
        run.return_value.stdout = json.dumps({**saved, "brightness": 25})
        with self.assertRaisesRegex(RuntimeError, "readback"):
            apply_action(action)

    @patch("iteglow.lighting_session.subprocess.run")
    def test_logo_backend_refuses_auth_or_helper_failure(self, run):
        action = validate_logo({"mode": "solid", "brightness": 0, "color": [0, 0, 0]})
        run.return_value = Mock(returncode=127, stdout="", stderr="not authorized")
        with self.assertRaisesRegex(RuntimeError, "not authorized"):
            apply_action(action)
        run.return_value = Mock(returncode=0, stdout='{"error":"unsupported firmware"}', stderr="")
        with self.assertRaisesRegex(RuntimeError, "rejected"):
            apply_action(action)

    def test_reactive_flag_remains_independent_of_direction(self):
        auto = validate_hid("keyboard", {"mode": "spark", "direction": 4})
        reactive = validate_hid("keyboard", {"mode": "spark", "direction": 4, "reactive": True})
        self.assertEqual(auto.reports[-1][1][6], 0)
        self.assertEqual(reactive.reports[-1][1][6], 1)

    def test_invalid_fields_rejected_without_clamping(self):
        bad = ({"mode": "rainbow", "brightness": 51},
               {"mode": "rainbow", "brightness": True},
               {"mode": "rainbow", "speed": 11},
               {"mode": "rainbow", "color": [-1, 0, 0]},
               {"mode": "rainbow", "colors": [[1, 2, 3]] * 8},
               {"mode": "rainbow", "reactive": 1},
               {"mode": "rainbow", "arbitrary_path": "/dev/mem"},
               {"mode": "unknown"})
        for saved in bad:
            with self.subTest(saved=saved), self.assertRaises(ValueError):
                validate_hid("keyboard", saved)
        with self.assertRaises(ValueError):
            validate_hid("lightbar", {"mode": "wave", "direction": 4})
        with self.assertRaises(ValueError):
            validate_logo({"mode": "solid", "brightness": 30, "color": [0, 0, 0]})
        with self.assertRaises(ValueError):
            validate_logo({"mode": "breathing", "brightness": 25, "color": [1, 2, 3]})

    def test_bad_device_state_does_not_block_valid_other_device(self):
        self.save("state.json", {"keyboard": {"mode": "bad"}, "lightbar": {"mode": "off"}})
        actions, errors = load_actions(self.directory, self.uid)
        self.assertEqual([a.device for a in actions], ["lightbar"])
        self.assertEqual(len(errors), 1)

    def test_malformed_oversized_symlink_and_world_writable_state(self):
        path = self.directory / "state.json"
        path.write_text("{broken")
        self.assertTrue(load_actions(self.directory, self.uid)[1])
        path.write_text(" " * 65537)
        self.assertTrue(load_actions(self.directory, self.uid)[1])
        path.unlink()
        original = self.directory / "elsewhere.json"
        original.write_text('{"keyboard":{"mode":"off"}}')
        path.symlink_to(original)
        self.assertTrue(load_actions(self.directory, self.uid)[1])
        path.unlink()
        self.save("state.json", {"keyboard": {"mode": "off"}})
        path.chmod(0o666)
        self.assertTrue(load_actions(self.directory, self.uid)[1])

    def test_other_owner_and_unknown_devices_are_rejected(self):
        self.save("state.json", {"keyboard": {"mode": "off"}})
        self.assertTrue(load_actions(self.directory, self.uid + 1)[1])
        self.save("state.json", {"performance": {"mode": "turbo"}})
        self.assertEqual(load_actions(self.directory, self.uid)[0], ())

    def test_inactive_or_departing_session_never_writes_remaining_devices(self):
        self.save("state.json", {"keyboard": {"mode": "off"}, "lightbar": {"mode": "off"}})
        writer = Mock()
        restore_once(self.directory, self.uid, lambda: False, apply=True, writer=writer)
        writer.assert_not_called()
        # The first completed writer represents a user switching sessions;
        # this stays meaningful when more checks are added before a write.
        result = restore_once(self.directory, self.uid, lambda: writer.call_count == 0,
                              apply=True, writer=writer)
        self.assertEqual(result["restored"], ["keyboard"])
        self.assertEqual(writer.call_count, 1)

    def test_writer_failure_is_reported_and_saved_state_not_rewritten(self):
        saved = {"keyboard": {"mode": "off"}, "lightbar": {"mode": "off"}}
        self.save("state.json", saved)
        writer = Mock(side_effect=[OSError("temporarily unavailable"), None])
        result = restore_once(self.directory, self.uid, lambda: True, apply=True, writer=writer)
        self.assertEqual(result["restored"], ["lightbar"])
        self.assertEqual(len(result["errors"]), 1)
        self.assertEqual(json.loads((self.directory / "state.json").read_text()), saved)

    def test_one_startup_and_one_restore_for_each_complete_sleep_cycle(self):
        active, schedule, cancel = Mock(return_value=True), Mock(), Mock()
        lifecycle = Lifecycle(active, schedule, cancel)
        lifecycle.startup(); lifecycle.startup()
        self.assertEqual(schedule.call_count, 1)
        lifecycle.prepare_for_sleep(False)
        self.assertEqual(schedule.call_count, 1)
        lifecycle.prepare_for_sleep(True)
        self.assertFalse(lifecycle.can_restore())
        lifecycle.prepare_for_sleep(False); lifecycle.prepare_for_sleep(False)
        self.assertEqual(schedule.call_count, 2)
        self.assertTrue(lifecycle.can_restore())
        lifecycle.stop()
        lifecycle.prepare_for_sleep(True); lifecycle.prepare_for_sleep(False)
        self.assertFalse(lifecycle.can_restore())
        self.assertEqual(schedule.call_count, 2)

    def test_resume_in_another_active_session_does_not_schedule(self):
        active, schedule = Mock(return_value=True), Mock()
        lifecycle = Lifecycle(active, schedule, Mock())
        lifecycle.startup(); lifecycle.prepare_for_sleep(True)
        active.return_value = False
        lifecycle.prepare_for_sleep(False)
        self.assertEqual(schedule.call_count, 1)
        self.assertFalse(lifecycle.can_restore())

    def test_only_local_active_owned_graphical_user_session_is_eligible(self):
        valid = {"User": (self.uid, "/user"), "Seat": ("seat0", "/seat"),
                 "Active": True, "Remote": False, "State": "active", "Class": "user", "Type": "wayland"}
        self.assertTrue(eligible_session(valid, self.uid))
        for key, value in (("User", (self.uid + 1, "/user")), ("Seat", ("", "/")),
                           ("Active", False), ("Remote", True), ("State", "closing"),
                           ("Class", "greeter"), ("Type", "tty")):
            with self.subTest(key=key):
                self.assertFalse(eligible_session({**valid, key: value}, self.uid))


if __name__ == "__main__":
    unittest.main()

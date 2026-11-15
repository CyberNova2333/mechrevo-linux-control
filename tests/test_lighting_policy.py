"""AC/DC and idle planning tests: no GUI, device, clock or bus access."""
from copy import deepcopy
import json
from pathlib import Path
import sys
import unittest

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "iteglow/src"))
from iteglow.lighting_policy import (PolicyMemory, acknowledge, action_matches,
                                    brightness_value, default_config, plan, validate_config)


class PolicyTest(unittest.TestCase):
    def setUp(self):
        self.config = default_config()
        self.config.update(enabled=True, power_enabled=True, idle_minutes=2)
        self.config["brightness"] = {"keyboard": {"ac": 63, "dc": 21},
                                     "lightbar": {"ac": 70, "dc": 13},
                                     "logo": {"ac": 75, "dc": 25}}
        self.saved = {"keyboard": {"mode": "spark", "brightness": 50, "direction": 4, "reactive": True},
                      "lightbar": {"mode": "wave", "brightness": 100, "colors": [[3, 4, 5]]},
                      "logo": {"mode": "breathing", "brightness": 100, "color": [255, 0, 0]}}

    def evaluate(self, **kwargs):
        args = dict(power="ac", idle_seconds=0, saved=self.saved)
        args.update(kwargs)
        return plan(self.config, **args)

    def applied(self, result):
        memory = result.memory
        for action in result.actions:
            memory = acknowledge(memory, action)
        return memory

    def test_default_is_disabled_and_does_not_invent_missing_effects(self):
        self.assertFalse(default_config()["enabled"])
        self.assertEqual(plan(default_config(), power="ac", idle_seconds=999, saved=self.saved).actions, ())
        self.assertEqual(self.evaluate(saved={}).actions, ())

    def test_ac_dc_three_zones_preserve_effect_details_and_source(self):
        original = deepcopy(self.saved)
        ac = self.evaluate()
        self.assertEqual([action.params["brightness"] for action in ac.actions], [32, 70, 75])
        self.assertTrue(ac.actions[0].params["reactive"])
        self.assertEqual(ac.actions[1].params["colors"], [[3, 4, 5]])
        memory = self.applied(ac)
        self.assertEqual(self.evaluate(memory=memory).actions, ())
        dc = self.evaluate(power="dc", memory=memory)
        self.assertEqual([action.params["brightness"] for action in dc.actions], [11, 13, 25])
        self.assertEqual(self.saved, original)

    def test_idle_boundary_and_activity_restore_effect_not_new_default(self):
        memory = self.applied(self.evaluate())
        self.assertEqual(self.evaluate(idle_seconds=119.999, memory=memory).actions, ())
        off = self.evaluate(idle_seconds=120, memory=memory)
        self.assertEqual([a.params["brightness"] for a in off.actions], [0, 0, 0])
        self.assertTrue(all(a.reason == "idle" for a in off.actions))
        resumed = self.evaluate(idle_seconds=0, memory=self.applied(off))
        self.assertEqual([a.params["brightness"] for a in resumed.actions], [32, 70, 75])
        self.assertEqual(resumed.actions[0].params["mode"], "spark")

    def test_power_change_while_idle_only_updates_future_restore(self):
        off = self.evaluate(idle_seconds=121, memory=self.applied(self.evaluate()))
        dc = self.evaluate(power="dc", idle_seconds=130, memory=self.applied(off))
        self.assertEqual(dc.actions, ())
        resumed = self.evaluate(power=None, idle_seconds=0, memory=dc.memory)
        self.assertEqual([a.params["brightness"] for a in resumed.actions], [11, 13, 25])

    def test_unknown_inputs_do_not_guess_power_or_restore_while_still_idle(self):
        self.assertEqual(self.evaluate(power=None, idle_seconds=None).actions, ())
        off = self.evaluate(idle_seconds=121, memory=self.applied(self.evaluate()))
        self.assertEqual(self.evaluate(power=None, idle_seconds=None, memory=self.applied(off)).actions, ())

    def test_partial_success_retries_failed_zones_only(self):
        first = self.evaluate()
        memory = acknowledge(first.memory, first.actions[0])
        retry = self.evaluate(memory=memory)
        self.assertEqual([a.device for a in retry.actions], ["lightbar", "logo"])

    def test_disabled_restores_only_owned_zones_then_forgets(self):
        first = self.evaluate()
        memory = acknowledge(first.memory, first.actions[0])
        self.config["enabled"] = False
        release = self.evaluate(memory=memory)
        self.assertEqual([a.device for a in release.actions], ["keyboard"])
        self.assertEqual(release.actions[0].params, self.saved["keyboard"])
        self.assertTrue(release.actions[0].release)
        self.assertEqual(self.applied(release), PolicyMemory())

    def test_manual_off_zero_and_black_logo_are_never_automatically_lit(self):
        for idle in (0, 121):
            saved = {"keyboard": {"mode": "off"}, "lightbar": {"mode": "wave", "brightness": 0},
                     "logo": {"mode": "solid", "brightness": 100, "color": [4, 0, 1]}}
            self.assertEqual(self.evaluate(idle_seconds=idle, saved=saved).actions, ())

    def test_manual_changes_break_ownership_without_old_state_restore(self):
        memory = self.applied(self.evaluate(idle_seconds=121))
        self.saved["keyboard"] = {"mode": "off"}
        self.config["enabled"] = False
        release = self.evaluate(memory=memory)
        self.assertEqual([a.device for a in release.actions], ["lightbar", "logo"])
        self.assertNotIn("keyboard", [zone.device for zone in release.memory.zones])

    def test_queued_action_is_invalidated_by_new_manual_state(self):
        action = self.evaluate().actions[0]
        self.assertTrue(action_matches(action, self.saved))
        self.saved["keyboard"]["mode"] = "off"
        self.assertFalse(action_matches(action, self.saved))
        self.assertFalse(action_matches(action, {}))

    def test_missing_or_bad_saved_zone_does_not_block_valid_others(self):
        memory = self.applied(self.evaluate())
        del self.saved["keyboard"]
        self.saved["logo"]["color"] = [1, 2, 3]  # invalid breathing palette
        result = self.evaluate(power="dc", memory=memory)
        self.assertEqual([a.device for a in result.actions], ["lightbar"])
        self.assertEqual(len(result.errors), 1)
        self.assertEqual([z.device for z in result.memory.zones], ["lightbar"])

    def test_idle_only_restore_returns_original_brightness(self):
        self.config["power_enabled"] = False
        self.assertEqual(self.evaluate().actions, ())
        off = self.evaluate(idle_seconds=120)
        restored = self.evaluate(memory=self.applied(off))
        self.assertTrue(all(action.release for action in restored.actions))
        self.assertEqual([a.params["brightness"] for a in restored.actions], [50, 100, 100])

    def test_disabling_idle_restores_lights_even_with_missing_idle_sensor(self):
        off = self.evaluate(idle_seconds=120)
        self.config["idle_minutes"] = 0
        self.assertEqual([a.params["brightness"] for a in self.evaluate(idle_seconds=None, memory=self.applied(off)).actions], [32, 70, 75])

    def test_all_percentages_are_bounded_and_logo_rejects_in_between(self):
        self.assertEqual([brightness_value("keyboard", p) for p in (0, 1, 50, 99, 100)], [0, 1, 25, 50, 50])
        for p in range(101):
            self.assertTrue(0 <= brightness_value("keyboard", p) <= 50)
            self.assertEqual(brightness_value("lightbar", p), p)
            if p % 25:
                with self.assertRaises(ValueError): brightness_value("logo", p)

    def test_rejects_malformed_config_and_sensor_types(self):
        mutations = [("enabled", 1), ("idle_minutes", True), ("idle_minutes", -1),
                     ("version", True), ("version", 2), ("extra", "ignored")]
        for key, value in mutations:
            config = deepcopy(self.config); config[key] = value
            with self.subTest(key=key, value=value), self.assertRaises(ValueError): validate_config(config)
        for device, value in (("logo", 30), ("keyboard", 101), ("lightbar", False)):
            config = deepcopy(self.config); config["brightness"][device]["ac"] = value
            with self.assertRaises(ValueError): validate_config(config)
        for idle in (True, -1, float("nan"), float("inf"), "5"):
            with self.assertRaises(ValueError): self.evaluate(idle_seconds=idle)
        with self.assertRaises(ValueError): self.evaluate(power=True)

    def test_input_config_and_action_params_are_not_mutated(self):
        snapshot = json.dumps(self.config, sort_keys=True)
        result = self.evaluate()
        params = result.actions[0].params; params["mode"] = "off"
        self.assertEqual(result.actions[0].params["mode"], "spark")
        self.assertEqual(json.dumps(self.config, sort_keys=True), snapshot)


if __name__ == "__main__":
    unittest.main()

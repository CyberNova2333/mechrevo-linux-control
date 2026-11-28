"""Track 3 tests: profile bundles, app auto-switching, Mode Key yield, CLI, and GUI."""
import contextlib
import io
import json
import os
from pathlib import Path
import sys
import tempfile
import unittest
from unittest.mock import Mock, patch

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "iteglow/src"))
from iteglow import cli, keymap, lighting_policy, profiles  # noqa: E402


class _NoopTx:
    def __init__(self, _zone):
        pass

    def __enter__(self):
        return self

    def __exit__(self, *_):
        return False


class ProfilesSchemaAndSecurityTest(unittest.TestCase):
    def test_defaults_templates_and_tuning_validation(self):
        cfg = profiles.default_config()
        self.assertFalse(cfg["auto_switch_enabled"])
        self.assertEqual(cfg, profiles.validate_config(cfg))

        templates = profiles.builtin_templates()
        self.assertIn("gaming-fps", templates)
        cfg["profiles"] = templates
        cfg["rules"] = [
            {"name": "Games", "enabled": True, "match": ["Cyberpunk2077.exe", "blender"], "profile": "gaming-fps"}
        ]
        validated = profiles.validate_config(cfg)
        self.assertEqual(validated["rules"][0]["match"], ["cyberpunk2077.exe", "blender"])

        valid_tuning = {
            "pl1": 55,
            "pl2": 55,
            "cpu_fan": [40, 45, 50, 55, 60, 65, 70, 75, 85, 92, 100],
            "gpu_fan": [40, 45, 50, 55, 60, 65, 70, 75, 85, 92, 100],
            "gpu_ctgp": 95,
            "gpu_db": 15,
        }
        p = profiles.validate_profile({"performance": {"mode": "custom", "tuning": valid_tuning}})
        self.assertEqual(p["performance"]["tuning"]["pl1"], 55)

        bad_tuning = dict(valid_tuning, pl2=60)
        with self.assertRaises(ValueError):
            profiles.validate_profile({"performance": {"mode": "custom", "tuning": bad_tuning}})
        bad_rule = {"name": "Broken", "enabled": True, "match": ["foo"], "profile": "nonexistent"}
        with self.assertRaises(ValueError):
            profiles.validate_rule(bad_rule, templates.keys())

    def test_load_config_rejects_symlinks_and_world_writable(self):
        with tempfile.TemporaryDirectory() as td:
            d = Path(td)
            real = d / "real.json"
            real.write_text(json.dumps(profiles.default_config()))
            link = d / "profiles.json"
            link.symlink_to(real)
            with self.assertRaises(OSError):
                profiles.load_config(link, os.getuid())

            world = d / "world.json"
            world.write_text(json.dumps(profiles.default_config()))
            os.chmod(world, 0o666)
            with self.assertRaises(ValueError):
                profiles.load_config(world, os.getuid())

    def test_lighting_policy_accepts_matrix_keyboard_state(self):
        pcfg = lighting_policy.default_config()
        pcfg.update(enabled=True, power_enabled=True, idle_minutes=5)
        pcfg["brightness"]["keyboard"]["dc"] = 40
        saved = {
            "keyboard": {
                "mode": "matrix",
                "preset": "wasd_fps",
                "matrix": [list(c) for c in keymap.preset_cells("wasd_fps")],
                "brightness": 50,
                "calibrate": True,
            }
        }
        plan_res = lighting_policy.plan(pcfg, power="dc", idle_seconds=0, saved=saved)
        self.assertEqual(plan_res.errors, ())
        self.assertEqual(len(plan_res.actions), 1)
        self.assertEqual(plan_res.actions[0].params["brightness"], 20)


class RuleEvaluatorAndSafetyTest(unittest.TestCase):
    def setUp(self):
        self.config = profiles.default_config()
        self.config["profiles"] = profiles.builtin_templates()
        self.config["auto_switch_enabled"] = True
        self.config["cooldown_seconds"] = 5
        self.config["rules"] = [
            {"name": "GameRule", "enabled": True, "match": ["cyberpunk2077", "blender"], "profile": "gaming-fps"}
        ]

    def test_proc_scan_extracts_linux_and_wine_tokens_ignoring_self(self):
        with tempfile.TemporaryDirectory() as td:
            proc = Path(td)
            p1 = proc / "1234"
            p1.mkdir()
            (p1 / "comm").write_text("MainThread\n")
            (p1 / "cmdline").write_bytes(b"Z:\\Games\\CP2077\\Cyberpunk2077.exe\x00-skipStartScreen\x00")
            p2 = proc / "1235"
            p2.mkdir()
            (p2 / "comm").write_text("iteglow\n")
            (p2 / "cmdline").write_bytes(b"/usr/bin/python3\x00-m\x00iteglow.gui\x00")

            procs = profiles.scan_processes(proc_root=proc, uid=os.getuid())
            self.assertEqual(len(procs), 1)
            self.assertIn("cyberpunk2077.exe", procs[0])
            self.assertIn("cyberpunk2077", procs[0])
            matched = profiles.match_rule(self.config["rules"], procs)
            self.assertEqual(matched["name"], "GameRule")

    def test_evaluator_cooldown_mode_key_yield_and_fan_safety(self):
        ev = profiles.RuleEvaluator()
        baseline = {"performance": {"mode": "balanced"}, "lighting": {"keyboard": {"mode": "rainbow", "brightness": 50}}}

        # 1. Baseline initialized with existing 0xB0 seq = 3; no trigger when no app running
        d0 = ev.step(self.config, processes=[], b0_seq=3, now=10.0, capture_baseline=lambda: baseline)
        self.assertEqual(d0.action, "noop")

        # 2. App starts -> switches to gaming-fps
        d1 = ev.step(self.config, processes=[("cyberpunk2077",)], b0_seq=3, now=12.0, capture_baseline=lambda: baseline)
        self.assertEqual((d1.action, d1.rule_name, d1.profile_name), ("apply", "GameRule", "gaming-fps"))

        # 3. App briefly disappears during launcher handoff (< 5s cooldown) -> hold
        d2 = ev.step(self.config, processes=[], b0_seq=3, now=15.0)
        self.assertEqual(d2.action, "hold")

        # 4. Physical Mode Key (0xB0) pressed while rule active -> immediate yield!
        d3 = ev.step(self.config, processes=[("cyberpunk2077",)], b0_seq=4, now=16.0)
        self.assertEqual(d3.action, "yield_mode_key")

        # 5. While app remains open after yield -> stay yielded, do NOT override Mode Key
        d4 = ev.step(self.config, processes=[("cyberpunk2077",)], b0_seq=4, now=30.0)
        self.assertEqual(d4.action, "yielded")

        # 6. App exits and cooldown expires -> yield resets so next launch works again
        self.assertEqual(ev.step(self.config, processes=[], b0_seq=4, now=31.0).action, "yielded")
        self.assertEqual(ev.step(self.config, processes=[], b0_seq=4, now=37.0).action, "noop")

        # 7. Fan safety protect blocks performance auto-switch
        d5 = ev.step(
            self.config,
            processes=[("blender",)],
            b0_seq=4,
            fan_safety_protect=True,
            now=40.0,
            capture_baseline=lambda: baseline,
        )
        self.assertEqual(d5.action, "fan_safety_paused")

        # 8. Normal activation -> exit after cooldown -> restore baseline
        self.assertEqual(
            ev.step(self.config, processes=[("blender",)], b0_seq=4, now=41.0, capture_baseline=lambda: baseline).action,
            "apply",
        )
        self.assertEqual(ev.step(self.config, processes=[], b0_seq=4, now=42.0).action, "hold")
        rel = ev.step(self.config, processes=[], b0_seq=4, now=48.0)
        self.assertEqual(rel.action, "release")
        self.assertEqual(rel.restore_profile, baseline)


class ProfileRunnerAndApplyTest(unittest.TestCase):
    def test_disabled_runner_performs_zero_proc_or_helper_calls(self):
        cfg = profiles.default_config()
        scanner = Mock(side_effect=AssertionError("must not scan when disabled"))
        runner = profiles.ProfileRuleRunner(
            active=lambda: True,
            config=lambda: cfg,
            directory=Path("/tmp"),
            uid=os.getuid(),
            apply=True,
            process_scanner=scanner,
            wmi_reader=Mock(side_effect=AssertionError("must not read WMI when disabled")),
            helper_runner=Mock(side_effect=AssertionError("must not run helper when disabled")),
        )
        res = runner.tick()
        self.assertEqual(res["action"], "disabled")
        scanner.assert_not_called()

    def test_retries_only_failed_profile_parts_and_restores_only_owned_zones(self):
        with tempfile.TemporaryDirectory() as tmp:
            cfg = profiles.default_config()
            cfg["profiles"] = profiles.builtin_templates()
            cfg["auto_switch_enabled"] = True
            cfg["rules"] = [{"name": "Game", "enabled": True, "match": ["game"], "profile": "gaming-fps"}]
            runner = profiles.ProfileRuleRunner(active=lambda: True, config=lambda: cfg, directory=Path(tmp),
                                                uid=os.getuid(), apply=True, process_scanner=lambda: [("game",)],
                                                wmi_reader=dict)
            target = cfg["profiles"]["gaming-fps"]["lighting"]["keyboard"]
            baseline = {"lighting": {"keyboard": target, "logo": {"mode": "solid", "brightness": 50, "color": [0, 0, 0]}},
                        "performance": {"mode": "balanced"}}
            calls = []
            fail_performance = [True]

            def apply_one(bundle, **kwargs):
                calls.append(bundle)
                if "performance" in bundle and fail_performance[0]:
                    fail_performance[0] = False
                    raise RuntimeError("temporary helper failure")
                return {"lighting": list(bundle.get("lighting", {})), "warnings": []}

            with patch.object(runner, "_capture_baseline", return_value=baseline), patch.object(profiles, "apply_profile", side_effect=apply_one):
                self.assertTrue(runner.tick()["errors"])
                self.assertTrue(runner.owns_lighting)
                calls.clear()
                self.assertFalse(runner.tick()["errors"])
                self.assertEqual(calls, [{"performance": cfg["profiles"]["gaming-fps"]["performance"]}])
                calls.clear()
                self.assertEqual(runner.tick()["action"], "noop")
                self.assertFalse(calls)
                cfg["auto_switch_enabled"] = False
                fail_performance[0] = True
                self.assertTrue(runner.tick()["errors"])
                self.assertEqual(calls, [{"lighting": {"keyboard": target}}, {"performance": {"mode": "balanced"}}])
                calls.clear()
                self.assertFalse(runner.tick()["errors"])
                self.assertEqual(calls, [{"performance": {"mode": "balanced"}}])
                self.assertEqual(runner.tick()["action"], "disabled")

    def test_runner_auto_apply_does_not_overwrite_manual_state_and_respects_b0_yield(self):
        with tempfile.TemporaryDirectory() as td:
            state_dir = Path(td)
            manual_kbd = {"mode": "rainbow", "brightness": 50, "speed": 1, "direction": 1, "reactive": False}
            (state_dir / "state.json").write_text(json.dumps({"keyboard": manual_kbd}))

            cfg = profiles.default_config()
            cfg["profiles"] = profiles.builtin_templates()
            cfg["auto_switch_enabled"] = True
            cfg["cooldown_seconds"] = 2
            cfg["rules"] = [{"name": "FPS", "enabled": True, "match": ["game"], "profile": "gaming-fps"}]

            helper_calls = []

            def fake_helper(args):
                helper_calls.append(list(args))
                if args == ["status"]:
                    return {
                        "mode": "balanced",
                        "switchable": True,
                        "fan_safety_protect": False,
                        "tuning_active": False,
                        "tuning_recovery_pending": False,
                        "turbo": {"available": True, "active": False, "round_adapter": True, "combined_supported": True},
                    }
                return {"ok": True}

            hid_actions = []
            procs = [[("game",)]]
            wmi = [{"wmi_seq": 1, "wmi_last_code": 176, "wmi_events": [{"seq": 1, "code": 176}], "fan_safety_protect": False}]
            now = [100.0]

            runner = profiles.ProfileRuleRunner(
                active=lambda: True,
                config=lambda: cfg,
                directory=state_dir,
                uid=os.getuid(),
                apply=True,
                process_scanner=lambda: procs[0],
                wmi_reader=lambda: wmi[0],
                helper_runner=fake_helper,
                hid_writer=lambda act: hid_actions.append(act.device),
                transaction=_NoopTx,
                clock=lambda: now[0],
            )
            r1 = runner.tick()
            self.assertEqual(r1["action"], "apply")
            self.assertTrue(r1["active_lighting"])
            self.assertIn(["set", "turbo"], helper_calls)
            self.assertIn(["turbo", "extreme"], helper_calls)
            self.assertEqual(hid_actions, ["keyboard"])
            # state.json must still preserve user's manual rainbow baseline
            self.assertEqual(json.loads((state_dir / "state.json").read_text())["keyboard"], manual_kbd)

            # Physical Mode Key 0xB0 arrives -> yield WITHOUT writing performance helper
            helper_calls.clear()
            hid_actions.clear()
            wmi[0] = {"wmi_seq": 2, "wmi_last_code": 176, "wmi_events": [{"seq": 2, "code": 176}], "fan_safety_protect": False}
            now[0] = 102.0
            r2 = runner.tick()
            self.assertEqual(r2["action"], "yield_mode_key")
            self.assertTrue(r2["released_lighting"])
            self.assertEqual(helper_calls, [])
            self.assertEqual(hid_actions, ["keyboard"])


class ProfileCliAndGuiTest(unittest.TestCase):
    def test_cli_profile_lifecycle(self):
        with tempfile.TemporaryDirectory() as td:
            d = Path(td)
            state_dir = d / "state/iteglow"
            state_dir.mkdir(parents=True)
            (state_dir / "state.json").write_text(json.dumps({"keyboard": {"mode": "rainbow", "brightness": 40}}))
            cfg_home = d / "config"

            with patch.dict(os.environ, {"XDG_CONFIG_HOME": str(cfg_home), "XDG_STATE_HOME": str(d / "state")}), \
                 patch.object(cli.os, "geteuid", return_value=1000), \
                 patch("iteglow.profiles.default_helper_runner", return_value={"mode": "turbo", "switchable": True, "turbo": {"active": False}, "tuning_active": False}):
                buf = io.StringIO()
                with contextlib.redirect_stdout(buf):
                    self.assertEqual(cli.main(["profile", "init-templates"]), 0)
                    self.assertEqual(cli.main(["profile", "save", "my-setup", "--label", "Work"]), 0)
                    self.assertEqual(cli.main(["profile", "rule-add", "BlenderRule", "my-setup", "--match", "blender"]), 0)
                    self.assertEqual(cli.main(["profile", "auto", "on", "--cooldown", "10"]), 0)
                    self.assertEqual(cli.main(["profile", "list"]), 0)
                out = buf.getvalue()
                self.assertIn("my-setup", out)
                self.assertIn("BlenderRule", out)

    @unittest.skipUnless(os.environ.get("DISPLAY"), "requires an isolated Xvfb display")
    def test_profiles_panel_construction_and_save(self):
        from gi.repository import Gtk
        from iteglow import profiles_gui

        Gtk.init()
        with tempfile.TemporaryDirectory() as td:
            p = Path(td) / "profiles.json"
            called = []
            panel = profiles_gui.ProfilesPanel(p, on_saved=lambda c: called.append(c))
            try:
                self.assertFalse(p.exists())
                self.assertFalse(panel.auto_enabled.get_active())
                panel.import_templates()
                self.assertTrue(p.exists())
                self.assertEqual(p.stat().st_mode & 0o777, 0o600)
                self.assertGreaterEqual(len(panel._profiles), 4)
                panel.rule_name_entry.set_text("IDE")
                panel.rule_match_entry.set_text("code, pycharm")
                panel.add_rule_clicked()
                panel.auto_enabled.set_active(True)
                panel.save()
                saved = json.loads(p.read_text())
                self.assertTrue(saved["auto_switch_enabled"])
                self.assertEqual(saved["rules"][0]["match"], ["code", "pycharm"])
                self.assertGreaterEqual(len(called), 1)
            finally:
                panel.close()

            corrupt = Path(td) / "corrupt.json"
            corrupt.write_text("{bad")
            bad_panel = profiles_gui.ProfilesPanel(corrupt)
            try:
                self.assertIn("尚未改动文件", bad_panel.message.get_text())
                self.assertEqual(corrupt.read_text(), "{bad")
            finally:
                bad_panel.close()


if __name__ == "__main__":
    unittest.main()

import importlib.machinery
import importlib.util
from pathlib import Path
import shutil
import subprocess
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[1]
PATCHER_PATH = ROOT / "system/mechrevo-nvidia-hibernate-patch"
DKMS_CONF_PATH = ROOT / "system/nvidia-dkms.conf"

loader = importlib.machinery.SourceFileLoader("nvidia_hibernate_patch", str(PATCHER_PATH))
spec = importlib.util.spec_from_loader(loader.name, loader)
patcher = importlib.util.module_from_spec(spec)
loader.exec_module(patcher)


def _write_fake_tree(base: Path) -> None:
    (base / "common/inc").mkdir(parents=True)
    (base / "nvidia").mkdir(parents=True)
    (base / "common/inc/nv-proto.h").write_text(
        "/* header */\n"
        "#if defined(CONFIG_PM)\n"
        "NV_STATUS     nv_set_system_power_state (nv_power_state_t, nv_pm_action_depth_t);\n"
        "#endif\n"
    )
    (base / "nvidia/nv.c").write_text(
        "// shifted line padding\n"
        "#if defined(CONFIG_PM)\n"
        "NV_STATUS nv_set_system_power_state(nv_power_state_t power_state, nv_pm_action_depth_t depth)\n"
        "{\n"
        "    NV_STATUS status = NV_OK;\n"
        "    nv_system_power_state = power_state;\n\n"
        "done:\n"
        "    up(&nv_system_power_state_lock);\n\n"
        "    return status;\n"
        "}\n"
        "#endif\n"
    )
    (base / "nvidia/nv-pci.c").write_text(
        "// upstream pci file\n"
        "static void\n"
        "nv_pci_shutdown(struct pci_dev *pci_dev)\n"
        "{\n"
        "    nv_linux_state_t *nvl = pci_get_drvdata(pci_dev);\n"
        "    if (!nvl || (nvl->pci_dev != pci_dev)) return;\n\n"
        "    if (nvl->is_forced_shutdown)\n"
        "    {\n"
        "        nvl->is_forced_shutdown = NV_FALSE;\n"
        "        return;\n"
        "    }\n\n"
        "#if defined(CONFIG_PM_DEVFREQ)\n"
        "    nv_pci_tegra_unregister_devfreq(pci_dev);\n"
        "#endif\n\n"
        "    nv_pci_remove_helper(pci_dev, false);\n\n"
        "    if (pci_get_drvdata(pci_dev) != NULL)\n"
        "    {\n"
        "        nvl->nv_state.is_shutdown = NV_TRUE;\n"
        "    }\n"
        "    pci_clear_master(pci_dev);\n"
        "}\n"
    )


class NvidiaHibernatePatchTest(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.addCleanup(self.tmp.cleanup)
        self.root = Path(self.tmp.name)

    def test_apply_is_idempotent_and_preserves_normal_shutdown(self):
        _write_fake_tree(self.root)
        self.assertEqual(patcher.inspect_tree(self.root), "needs_patch")
        self.assertEqual(patcher.apply_patch(self.root), "applied")
        self.assertEqual(patcher.inspect_tree(self.root), "already_patched")
        self.assertEqual(patcher.apply_patch(self.root), "already_patched")

        pci_text = (self.root / "nvidia/nv-pci.c").read_text()
        self.assertIn("!nv_is_in_hibernate() && !(READ_ONCE(nvl->nv_state.flags) & NV_FLAG_SUSPENDED)", pci_text)
        self.assertIn("pci_clear_master(pci_dev);", pci_text)
        self.assertIn("NvBool        nv_is_in_hibernate        (void);", (self.root / "common/inc/nv-proto.h").read_text())
        self.assertIn("return READ_ONCE(nv_system_power_state) == NV_POWER_STATE_IN_HIBERNATE;", (self.root / "nvidia/nv.c").read_text())

    def test_skips_when_upstream_already_handles_hibernate_shutdown(self):
        _write_fake_tree(self.root)
        pci_path = self.root / "nvidia/nv-pci.c"
        pci_path.write_text(
            "static void nv_pci_shutdown(struct pci_dev *pci_dev)\n"
            "{\n"
            "    if (!(nvl->nv_state.flags & NV_FLAG_SUSPENDED))\n"
            "        nv_pci_remove_helper(pci_dev, false);\n"
            "    pci_clear_master(pci_dev);\n"
            "}\n"
        )
        self.assertEqual(patcher.apply_patch(self.root), "upstream_already_guarded")

    def test_rolls_back_all_files_on_anchor_mismatch(self):
        _write_fake_tree(self.root)
        proto_before = (self.root / "common/inc/nv-proto.h").read_text()
        nv_c_before = (self.root / "nvidia/nv.c").read_text()
        pci_path = self.root / "nvidia/nv-pci.c"
        pci_before = (
            "static void nv_pci_shutdown(struct pci_dev *pci_dev)\n"
            "{\n"
            "    nv_pci_remove_helper(pci_dev, false);\n"
            "}\n"
        )
        pci_path.write_text(pci_before)

        with self.assertRaises(RuntimeError):
            patcher.apply_patch(self.root)

        self.assertEqual((self.root / "common/inc/nv-proto.h").read_text(), proto_before)
        self.assertEqual((self.root / "nvidia/nv.c").read_text(), nv_c_before)
        self.assertEqual(pci_path.read_text(), pci_before)

    def test_matches_real_installed_nvidia_dkms_source_read_only(self):
        real_src = Path("/usr/src/nvidia-595.99.02")
        if not real_src.is_dir():
            self.skipTest("nvidia-595.99.02 DKMS source not present")
        for rel in ("common/inc/nv-proto.h", "nvidia/nv.c", "nvidia/nv-pci.c"):
            dst = self.root / rel
            dst.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(real_src / rel, dst)
        self.assertEqual(patcher.inspect_tree(self.root), "needs_patch")
        self.assertEqual(patcher.apply_patch(self.root), "applied")
        self.assertEqual(patcher.inspect_tree(self.root), "already_patched")

    def test_dkms_override_injects_patcher_without_breaking_safe_source(self):
        script = f"""
set -e
dkms_conf_variables="MAKE MAKE_MATCH"
safe_source() {{
    local to_source_file="$1"; shift
    declare -a -r export_envs=("$@")
    local tmpfile=$(mktemp)
    ( exec >"$tmpfile"
    . "$to_source_file" >/dev/null
    for _export_env in "${{export_envs[@]}}"; do
        for _i in $(eval echo \\${{!$_export_env[@]}}); do
            eval echo '$_export_env[$_i]=\\"${{'$_export_env'[$_i]}}\\"'
        done
    done
    )
    . "$tmpfile"
    rm -f "$tmpfile"
}}
MAKE[0]="unset ARCH; [ ! -h /usr/bin/cc ] && export CC=/usr/bin/gcc; env NV_VERBOSE=1 'make' -j16 modules"
safe_source "{DKMS_CONF_PATH}" $dkms_conf_variables
printf "%s" "${{MAKE[0]}}"
"""
        proc = subprocess.run(["bash", "-c", script], check=True, text=True, capture_output=True)
        expected_tail = "; /usr/local/libexec/mechrevo-nvidia-hibernate-patch --apply && env NV_VERBOSE=1 'make' -j16 modules"
        self.assertTrue(proc.stdout.endswith(expected_tail), proc.stdout)

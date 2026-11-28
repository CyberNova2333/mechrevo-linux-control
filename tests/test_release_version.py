# SPDX-License-Identifier: AGPL-3.0-only
import json
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import tomllib
import unittest


ROOT = Path(__file__).resolve().parents[1]
SCRIPT = ROOT / ".github/scripts/prepare-release.py"


class ReleaseVersionTests(unittest.TestCase):
    def test_version_is_stamped_only_on_request(self):
        with tempfile.TemporaryDirectory() as tmp:
            checkout = Path(tmp)
            files = [*ROOT.glob("*/Cargo.toml"), *ROOT.glob("*/Cargo.lock"),
                     ROOT / "ui-tauri/src-tauri/Cargo.toml", ROOT / "ui-tauri/src-tauri/Cargo.lock",
                     ROOT / "ui-tauri/package.json", ROOT / "ui-tauri/src-tauri/tauri.conf.json"]
            for src in files:
                dest = checkout / src.relative_to(ROOT)
                dest.parent.mkdir(exist_ok=True, parents=True)
                shutil.copyfile(src, dest)

            def prepare(*args):
                return subprocess.run([sys.executable, str(SCRIPT), *args], cwd=checkout,
                                      capture_output=True, text=True)

            saved = {f: (checkout / f.relative_to(ROOT)).read_bytes() for f in files}
            version = "1.0.1-rc.1+ci-2"
            for tag in ["v" + version, version]:
                result = prepare("--print-only", tag)
                self.assertEqual(result.returncode, 0, result.stderr)
                self.assertEqual(result.stdout.strip(), version)
            for tag in ["1.0", "1.0.0-01", "1.0.0\ntag=another"]:
                self.assertNotEqual(prepare(tag).returncode, 0)
            self.assertEqual(saved, {f: (checkout / f.relative_to(ROOT)).read_bytes() for f in files})

            result = prepare("v" + version)
            self.assertEqual(result.returncode, 0, result.stderr)
            self.assertEqual(prepare("--check").stdout.strip(), version)
            for src in files:
                data = (checkout / src.relative_to(ROOT)).read_text()
                if src.name == "Cargo.lock":
                    packages = tomllib.loads(data)["package"]
                    self.assertEqual([p["version"] for p in packages if "source" not in p], [version])
                    upstream = [p for p in tomllib.loads(src.read_text())["package"] if "source" in p]
                    self.assertEqual([p for p in packages if "source" in p], upstream)
                elif src.suffix == ".json":
                    self.assertEqual(json.loads(data)["version"], version)
                else:
                    self.assertEqual(tomllib.loads(data)["package"]["version"], version)
            (checkout / "ui-tauri/package.json").write_text('{"version":"0.0.1"}')
            mismatch = prepare("--check")
            self.assertNotEqual(mismatch.returncode, 0)
            self.assertIn("ui-tauri/package.json", mismatch.stderr)

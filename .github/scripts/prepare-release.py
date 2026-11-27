#!/usr/bin/env python3
# SPDX-License-Identifier: AGPL-3.0-only
"""Apply a release version to the current checkout and print the resolved version."""

import json
import re
import sys
import tomllib
from pathlib import Path

CRATES = [
    Path("watercooling-daemon"),
    Path("session-daemon"),
    Path("system-daemon"),
    Path("iteglow-cli"),
    Path("ui-tauri/src-tauri"),
]
NUMBER = r"(?:0|[1-9][0-9]*)"
PRE_ID = rf"(?:{NUMBER}|[0-9]*[A-Za-z-][0-9A-Za-z-]*)"
SEMVER = rf"{NUMBER}\.{NUMBER}\.{NUMBER}(?:-{PRE_ID}(?:\.{PRE_ID})*)?(?:\+[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*)?"


def stamp_package(path: Path, name: str, version: str) -> None:
    # In Cargo.lock only the local package (no source field) belongs to this release.
    blocks = re.split(r"(?m)(?=^\[)", path.read_text())
    for index, block in enumerate(blocks):
        if not block.startswith(("[package]\n", "[[package]]\n")):
            continue
        package = tomllib.loads(block)["package"]
        if isinstance(package, list):
            package = package[0]
        if package["name"] == name and "source" not in package:
            updated, count = re.subn(r'(?m)^version\s*=\s*"[^"]+"', f'version = "{version}"', block)
            if count != 1:
                raise SystemExit(f"{path}: expected one version for {name}")
            blocks[index] = updated
            path.write_text("".join(blocks))
            return
    raise SystemExit(f"{path}: local package {name} not found")


def main(tag: str) -> None:
    packages = [tomllib.loads((crate / "Cargo.toml").read_text())["package"] for crate in CRATES]
    version = tag.removeprefix("v") if tag else packages[0]["version"]
    if not re.fullmatch(SEMVER, version):
        raise SystemExit("请填写有效的语义版本，例如 1.0.0、v1.0.0 或 v1.0.0-rc.1")

    for crate, package in zip(CRATES, packages):
        for filename in ("Cargo.toml", "Cargo.lock"):
            stamp_package(crate / filename, package["name"], version)
    for path in (Path("ui-tauri/package.json"), Path("ui-tauri/src-tauri/tauri.conf.json")):
        data = json.loads(path.read_text())
        data["version"] = version
        path.write_text(json.dumps(data, indent=2, ensure_ascii=False) + "\n")
    print(version)


if __name__ == "__main__":
    main(sys.argv[1])

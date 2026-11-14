"""Persist the last-applied LED state so it can be restored later.

Every successful ``iteglow set`` / GUI apply records the parameters in the
user's state file (``$XDG_STATE_HOME/iteglow/state.json``). ``sudo iteglow
pin`` copies the invoking user's file to the system path, which ``iteglow
restore --system`` -- run by ``iteglow-restore.service`` on boot and resume --
re-applies. Needed because the controllers lose their state on suspend and
dual-boot, and only the keyboard supports (partial) state readback.
"""
from __future__ import annotations

import json
import os
import pwd
from typing import Optional

SYSTEM_STATE_PATH = "/var/lib/iteglow/state.json"


def user_state_path(user: Optional[str] = None) -> str:
    if user:
        home = pwd.getpwnam(user).pw_dir
        return os.path.join(home, ".local", "state", "iteglow", "state.json")
    base = os.environ.get("XDG_STATE_HOME") or os.path.expanduser("~/.local/state")
    return os.path.join(base, "iteglow", "state.json")


def load(path: str) -> dict:
    try:
        with open(path) as f:
            return json.load(f)
    except (OSError, ValueError):
        return {}


def write(path: str, data: dict) -> None:
    os.makedirs(os.path.dirname(path), exist_ok=True)
    tmp = path + ".tmp"
    with open(tmp, "w") as f:
        json.dump(data, f, indent=2)
        f.write("\n")
    os.replace(tmp, path)


def save(label: str, params: dict) -> None:
    """Record ``params`` as the last state of ``label``. Best-effort: state
    saving must never break the actual LED operation. Root (e.g. ``sudo
    iteglow set``) also updates the system state used on boot/resume."""
    targets = [user_state_path()]
    if os.geteuid() == 0:
        targets.append(SYSTEM_STATE_PATH)
    for path in targets:
        try:
            data = load(path)
            data[label] = params
            write(path, data)
        except OSError:
            pass

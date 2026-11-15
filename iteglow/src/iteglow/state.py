"""Persist the last-applied LED state so it can be restored later.

Every successful ``iteglow set`` / GUI apply records the parameters in the
user's state file (``$XDG_STATE_HOME/iteglow/state.json``). The user-session
restorer reads this file on login and resume when configured with --apply.
``sudo iteglow pin`` only exports a copy for a later manual ``iteglow restore
--system``; it does not enable a service or automatic restoration. The
controllers may lose state on suspend and dual-boot, and only the keyboard
supports partial state readback. Logo applied state is stored separately.
"""
from __future__ import annotations

import json
import os
import pwd
from typing import Optional
from .zone_lock import ZoneTransaction

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


def save(label: str, params: dict) -> bool:
    """Record ``params`` as the last state of ``label``. Best-effort: state
    saving must never break the actual LED operation. User CLI lighting writes
    reject root so their zone locks share the active desktop user's identity.
    The legacy root/library path also attempts the optional system export.
    Return False when persistence fails; the light operation has already
    succeeded and callers should report that distinction without retrying it.
    """
    targets = [user_state_path()]
    if os.geteuid() == 0:
        targets.append(SYSTEM_STATE_PATH)
    saved = True
    for path in targets:
        try:
            with ZoneTransaction('manual-state'):
                data = load(path)
                if not isinstance(data, dict):
                    raise ValueError("saved lighting state is not an object")
                data[label] = params
                write(path, data)
        except (OSError, ValueError, TypeError, RuntimeError):
            saved = False
    return saved

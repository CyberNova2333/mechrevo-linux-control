# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
"""Qualified module lifetime; no firmware reads/writes or automatic policy selection.

Sleep removes the module, including its cached ACPI handles. A fresh health gate
must run before loading those handles again. Dependencies are injectable so the
entire sleep/boot protocol can be tested without systemd or hardware access.
"""
from contextlib import ExitStack
import json
import os
from pathlib import Path
import subprocess
import time

from mechrevo_turbo_backend import (
    Journal, check_acpi_healthy, check_identity, check_qualification,
    identity, profile_lock, trusted_file,
)

MODULE = "mechrevo_turbo_lease"
UNIT = "mechrevo-turbo.service"
MARKER = Path("/run/mechrevo-turbo-sleep.json")
LIFECYCLE_LOCK = Path("/run/mechrevo-turbo-lifecycle.lock")
PHASES = {"preparing", "quiesced", "pre_failed", "resume_queued", "resume_failed"}


class Marker(Journal):
    def __init__(self, path=MARKER):
        super().__init__(path)

    def load(self):
        try:
            value = json.loads(trusted_file(self.path, private=True))
        except FileNotFoundError:
            return None
        if (not isinstance(value, dict) or value.get("version") != 1
                or value.get("phase") not in PHASES
                or type(value.get("was_active")) is not bool
                or not isinstance(value.get("boot_id"), str)
                or not isinstance(value.get("identity"), dict)):
            raise RuntimeError("睡眠恢复记录无效；拒绝自动恢复")
        digest = value.get("driver_sha256")
        if digest is not None and (not isinstance(digest, str) or len(digest) != 64
                                   or any(c not in "0123456789abcdef" for c in digest)):
            raise RuntimeError("睡眠恢复驱动哈希无效")
        return value


class Lifecycle:
    def __init__(self, *, run=subprocess.run, marker=None, machine=identity,
                 healthy=check_acpi_healthy, qualification=check_qualification,
                 loaded=lambda: Path("/sys/module", MODULE).exists(),
                 boot_id=lambda: Path("/proc/sys/kernel/random/boot_id").read_text().strip()):
        self.run, self.marker = run, marker if marker is not None else Marker()
        self.machine, self.healthy, self.qualification = machine, healthy, qualification
        self.loaded, self.boot_id = loaded, boot_id

    def command(self, args, *, timeout=20, check=True):
        result = self.run(args, capture_output=True, text=True, timeout=timeout)
        if check and result.returncode:
            raise RuntimeError(f"命令失败: {' '.join(args)}: {result.stderr.strip()}")
        return result

    def state(self):
        value = self.command(["/usr/bin/systemctl", "show", "--property=ActiveState", "--value", UNIT]).stdout.strip()
        if value not in {"active", "reloading", "activating", "deactivating", "inactive", "failed"}:
            raise RuntimeError("无法确定性能调度服务状态")
        return value

    def prepare(self):
        """ExecStartPre: no modprobe until identity, evidence and fresh AML agree."""
        saved = self.marker.load()
        if saved is not None and saved["phase"] != "resume_queued":
            raise RuntimeError("睡眠清理或恢复尚未完成，禁止加载性能驱动")
        current = self.machine()
        check_identity(current)
        qualified = self.qualification(require_loaded=False)
        if saved is not None:
            self.validate_resume(saved, current, qualified)
        self.healthy()
        was_loaded = self.loaded()
        self.command(["/usr/sbin/modprobe", MODULE], timeout=10)
        try:
            self.qualification(require_loaded=True)
        except Exception:
            # Do not remove somebody else's pre-existing module. A newly loaded
            # module with failed loaded-version validation is immediately removed.
            if not was_loaded:
                self.command(["/usr/sbin/modprobe", "-r", MODULE], timeout=10)
            raise
        if saved is not None:
            self.marker.clear()
        return {"prepared": True, "policy_selected": False}

    def pre(self):
        """Stop all users and unload before sleep; inability to unload is fatal."""
        saved = self.marker.load()
        boot = self.boot_id()
        # Retrying a failed pre must preserve the original running-service intent.
        # A queued/failed previous resume starts a fresh sleep attempt instead.
        if saved is None or saved["boot_id"] != boot or saved["phase"] in {"resume_queued", "resume_failed"}:
            saved = {"version": 1, "phase": "preparing", "boot_id": boot,
                     "identity": self.machine(),
                     "was_active": self.state() in {"active", "reloading", "activating"},
                     "was_module_loaded": self.loaded(), "driver_sha256": None}
            try:
                saved["driver_sha256"] = self.qualification(require_loaded=False)["driver_sha256"]
            except Exception as error:
                # Even unqualified or changed code must be shut down before sleep.
                # The missing saved hash prevents any automatic restart afterward.
                saved["qualification_error"] = str(error)
        saved["phase"] = "preparing"
        self.marker.save(saved)
        errors = []
        try:
            # First request the graceful, journaled release. Failure is not proof
            # of unsafe shutdown: FD close + successful unload is authoritative.
            if self.state() in {"active", "reloading"}:
                try:
                    self.command(["/usr/local/libexec/mechrevo-turbo-helper", "suspend"], timeout=6)
                except Exception as error:
                    errors.append(str(error))
            try:
                self.command(["/usr/bin/systemctl", "stop", UNIT])
            except Exception as error:
                errors.append(str(error))
            if self.state() not in {"inactive", "failed"}:
                raise RuntimeError("性能调度服务尚未停止；禁止睡眠")
            if self.loaded():
                self.command(["/usr/sbin/modprobe", "-r", MODULE], timeout=10)
            if self.loaded():
                raise RuntimeError("性能驱动尚未卸载；禁止睡眠")
            saved.update(phase="quiesced", cleanup_warnings=errors)
            self.marker.save(saved)
            return {"quiesced": True, "was_active": saved["was_active"], "warnings": errors}
        except Exception as error:
            saved.update(phase="pre_failed", error=str(error), cleanup_warnings=errors)
            self.marker.save(saved)
            raise

    def validate_resume(self, saved, current, qualified):
        if saved["boot_id"] != self.boot_id() or saved["identity"] != current:
            raise RuntimeError("睡眠前后启动身份或机型已改变，禁止自动恢复")
        if saved["driver_sha256"] is None or saved["driver_sha256"] != qualified["driver_sha256"]:
            raise RuntimeError("睡眠前后驱动验收哈希不一致，禁止自动恢复")

    def post(self):
        """Queue only an originally running service; never recreate a policy."""
        saved = self.marker.load()
        if saved is None:
            return {"resumed": False, "reason": "no_saved_service"}
        try:
            if saved["phase"] not in {"quiesced", "resume_failed"}:
                raise RuntimeError("驱动未完成睡眠前清理，禁止自动恢复")
            if self.loaded() or self.state() not in {"inactive", "failed"}:
                raise RuntimeError("睡眠后发现未清理的驱动或服务，禁止自动恢复")
            if not saved["was_active"]:
                self.marker.clear()
                return {"resumed": False, "reason": "previously_inactive"}
            current = self.machine()
            check_identity(current)
            # This check reads raw ACPI tables, never a cached AML handle.
            self.healthy()
            qualified = self.qualification(require_loaded=False)
            self.validate_resume(saved, current, qualified)
            saved.update(phase="resume_queued")
            self.marker.save(saved)
            # powerd-sleep post is ordered before this post; its queued start job
            # completes before our main unit's ExecCondition/ExecStartPre. A stopped
            # powerd is never started here and makes that condition skip the unit.
            self.command(["/usr/bin/systemctl", "--no-block", "start", UNIT])
            return {"resumed": "queued", "policy_selected": False}
        except Exception as error:
            saved.update(phase="resume_failed", error=str(error))
            self.marker.save(saved)
            raise


def main(phase):
    if os.geteuid() != 0:
        raise RuntimeError("性能驱动生命周期操作需要root")
    with ExitStack() as guards:
        deadline = time.monotonic() + 3
        while True:
            try:
                guards.enter_context(profile_lock(LIFECYCLE_LOCK))
                break
            except BlockingIOError:
                # post queues asynchronously while still holding this lock. Its
                # ExecStartPre may arrive before post has returned; allow that
                # short handoff, without retrying the lifecycle operation itself.
                if phase != "prepare" or time.monotonic() >= deadline:
                    raise
                time.sleep(0.05)
        return getattr(Lifecycle(), phase)()

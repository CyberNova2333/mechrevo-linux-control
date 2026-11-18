# SPDX-License-Identifier: AGPL-3.0-only
# This file is part of MECHREVO Linux Control and is licensed under GNU AGPLv3 with Additional Terms (see LICENSE and LICENSE-ADDITIONAL_TERMS-zh-CN.md; ref: LICENSE-ADDITIONAL_TERMS-en.md).
"""Cooperating user clients serialize complete per-zone lighting transactions.

Lock order is zone -> manual-state (or the Logo helper's profile lock). Nothing
holding manual-state/profile may acquire a zone. Locks are never unlinked: a
second inode would let later clients bypass a transaction still in flight.
"""
import fcntl
import os
from pathlib import Path
import stat
import threading
import time

_entries = {}
_entries_lock = threading.Lock()


class _Entry:
    def __init__(self):
        self.local = threading.RLock()
        self.depth = 0
        self.fd = None


class ZoneTransaction:
    def __init__(self, device, runtime_dir=None, timeout=5, *, reentrant=True):
        if device not in ('keyboard', 'lightbar', 'logo', 'manual-state'):
            raise ValueError('unknown lighting lock')
        self.device = device
        self.runtime = Path(runtime_dir or os.environ.get('XDG_RUNTIME_DIR') or f'/run/user/{os.getuid()}')
        self.timeout = timeout
        self.reentrant = reentrant
        self.depth = 0
        key = (str(self.runtime.absolute()), device)
        with _entries_lock:
            self.entry = _entries.setdefault(key, _Entry())

    def _open(self):
        directory = os.open(self.runtime, os.O_RDONLY | os.O_DIRECTORY | os.O_CLOEXEC | os.O_NOFOLLOW)
        try:
            info = os.fstat(directory)
            if info.st_uid != os.getuid() or info.st_mode & 0o022:
                raise RuntimeError('unsafe lighting runtime directory')
            fd = os.open(f'iteglow-{self.device}.lock',
                         os.O_CREAT | os.O_RDWR | os.O_CLOEXEC | os.O_NOFOLLOW,
                         0o600, dir_fd=directory)
            info = os.fstat(fd)
            if not stat.S_ISREG(info.st_mode) or info.st_uid != os.getuid() or info.st_mode & 0o077:
                os.close(fd)
                raise RuntimeError('unsafe lighting transaction lock')
            return fd
        finally:
            os.close(directory)

    def acquire(self, blocking=True):
        until = time.monotonic() + self.timeout
        if not self.entry.local.acquire(blocking=False):
            if not blocking:
                return False
            if not self.entry.local.acquire(timeout=self.timeout):
                raise TimeoutError('lighting transaction is busy')
        try:
            # Async GTK transactions can overlap on one thread after a window
            # closes/reopens. They must not inherit synchronous RLock nesting.
            if self.entry.depth and not self.reentrant:
                if not blocking:
                    self.entry.local.release()
                    return False
                raise RuntimeError('non-reentrant lighting transaction is already active')
            if not self.entry.depth:
                fd = self._open()
                try:
                    while True:
                        try:
                            fcntl.flock(fd, fcntl.LOCK_EX | fcntl.LOCK_NB)
                            break
                        except BlockingIOError:
                            if not blocking:
                                os.close(fd)
                                self.entry.local.release()
                                return False
                            if time.monotonic() >= until:
                                raise TimeoutError('lighting transaction is busy')
                            time.sleep(.02)
                except BaseException:
                    os.close(fd)
                    raise
                self.entry.fd = fd
            self.entry.depth += 1
            self.depth += 1
            return True
        except BaseException:
            self.entry.local.release()
            raise

    def release(self):
        if not self.depth:
            return
        self.depth -= 1
        self.entry.depth -= 1
        try:
            if not self.entry.depth:
                fd, self.entry.fd = self.entry.fd, None
                try:
                    fcntl.flock(fd, fcntl.LOCK_UN)
                finally:
                    os.close(fd)
        finally:
            self.entry.local.release()

    def __enter__(self):
        self.acquire()
        return self

    def __exit__(self, *_):
        self.release()

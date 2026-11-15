"""Candidate turbo lease supervisor. All firmware writes belong to the kernel lease.

This module has no /dev/mem or generic ACPI write path. Its injected Controller
is offline-testable; Device accepts only the qualified, versioned misc bridge.
"""
from contextlib import contextmanager
from dataclasses import asdict
import fcntl
import errno
import hashlib
import importlib.util
import json
import os
from pathlib import Path
import platform
import signal
import socket
import stat
import struct
import subprocess
import sys
import tempfile
import time


HERE = Path(__file__).resolve().parent
MODEL = HERE / "oem_scheduler.py"
if not MODEL.exists():
    MODEL = HERE.parent / "iteglow/src/iteglow/oem_scheduler.py"
_spec = importlib.util.spec_from_file_location("mechrevo_oem_scheduler", MODEL)
_model = importlib.util.module_from_spec(_spec)
sys.modules[_spec.name] = _model
_spec.loader.exec_module(_model)
LoadSample, SchedulerState = _model.LoadSample, _model.SchedulerState
Operation, advance = _model.Operation, _model.advance

KERNEL = "6.8.0-149-generic"
DMI = {"sys_vendor": "MECHREVO", "board_name": "YAOSHI Series-XxAF55xx",
       "bios_version": "N.1.33MRO91"}
DEVICE = Path("/dev/mechrevo-turbo")
LOCK = Path("/run/mechrevo-profile.lock")
DAEMON_LOCK = Path("/run/mechrevo-turbo-daemon.lock")
SOCKET = Path("/run/mechrevo-turbo.sock")
JOURNAL = Path("/run/mechrevo-turbo.json")
QUALIFICATION = Path("/etc/mechrevo-turbo-validation.json")
TUNING = Path("/run/mechrevo-tuning.json")
COMMANDS = {Operation.COMBINED_ON: "combined 1", Operation.COMBINED_OFF: "combined 0",
            Operation.GAME_ON: "game 1", Operation.GAME_OFF: "game 0"}
EXPECTED_ACK = {Operation.COMBINED_ON: 0xBB, Operation.COMBINED_OFF: 0xAA,
                Operation.GAME_ON: 0xBB, Operation.GAME_OFF: 0xAA}


def identity():
    return {"kernel_release": platform.release(),
            "dmi": {key: Path("/sys/class/dmi/id", key).read_text().strip() for key in DMI}}


def check_identity(value):
    if value != {"kernel_release": KERNEL, "dmi": DMI}:
        raise RuntimeError("机型、BIOS或内核已变化；此候选仅允许已验证的6.8.0-149分支")


def check_acpi_healthy(run=subprocess.run):
    report = run(["/usr/local/libexec/mechrevo-acpi-check", "--summary"],
                 capture_output=True, text=True, timeout=2)
    data = json.loads(report.stdout)
    if report.returncode != 0 or not isinstance(data, dict) or data.get("healthy") is not True:
        raise RuntimeError("ACPI完整性检查未通过，禁止启用狂暴调度")
    return data


def check_powerd_active(run=subprocess.run):
    powerd = run(["/usr/bin/systemctl", "is-active", "--quiet", "nvidia-powerd.service"],
                 capture_output=True, text=True, timeout=1)
    if powerd.returncode != 0:
        raise RuntimeError("nvidia-powerd未运行，禁止启用狂暴调度")


def check_runtime(run=subprocess.run):
    data = check_acpi_healthy(run)
    check_powerd_active(run)
    return data


def trusted_file(path, *, private=False, limit=65536):
    fd = os.open(path, os.O_RDONLY | os.O_NOFOLLOW | os.O_CLOEXEC)
    try:
        info = os.fstat(fd)
        forbidden = 0o077 if private else 0o022
        if not stat.S_ISREG(info.st_mode) or info.st_uid != 0 or info.st_mode & forbidden:
            raise RuntimeError(f"文件必须由root持有且不可由其他用户修改: {path}")
        parts, size = [], 0
        while True:
            block = os.read(fd, min(65536, limit + 1 - size))
            if not block:
                return b"".join(parts)
            parts.append(block)
            size += len(block)
            if size > limit:
                raise RuntimeError("文件超过允许长度")
    finally:
        os.close(fd)


def check_qualification(path=QUALIFICATION, *, require_loaded=True):
    """Deployment gate is evidence bound; never manufactures validation flags."""
    data = json.loads(trusted_file(path, private=True))
    expected = {"version", "identity", "driver_sha256", "combined_budget_verified",
                "lease_recovery_verified", "oem_ack_verified"}
    if set(data) != expected or data["version"] != 1:
        raise RuntimeError("性能调度验证清单格式不匹配")
    check_identity(data["identity"])
    if any(data[key] is not True for key in
           ("combined_budget_verified", "lease_recovery_verified", "oem_ack_verified")):
        raise RuntimeError("GPU联动、lease恢复和OEM ACK必须分别通过实机验收")
    digest = data["driver_sha256"]
    if not isinstance(digest, str) or len(digest) != 64 or any(c not in "0123456789abcdef" for c in digest):
        raise RuntimeError("驱动验证哈希无效")
    module = subprocess.run(["/usr/sbin/modinfo", "-n", "mechrevo_turbo_lease"],
                            text=True, capture_output=True, timeout=2, check=True).stdout.strip()
    if hashlib.sha256(trusted_file(module, limit=16 * 1024 * 1024)).hexdigest() != digest:
        raise RuntimeError("驱动文件与验收清单不一致")
    version = subprocess.run(["/usr/sbin/modinfo", "-F", "srcversion", module],
                             text=True, capture_output=True, timeout=2, check=True).stdout.strip()
    if not version or (require_loaded and Path("/sys/module/mechrevo_turbo_lease/srcversion").read_text().strip() != version):
        raise RuntimeError("已加载驱动与验收文件版本不一致")
    for code in (Path(__file__).resolve(), MODEL.resolve()):
        trusted_file(code)
    return data


@contextmanager
def profile_lock(path=LOCK):
    fd = os.open(path, os.O_CREAT | os.O_RDWR | os.O_CLOEXEC | os.O_NOFOLLOW, 0o600)
    try:
        info = os.fstat(fd)
        if not stat.S_ISREG(info.st_mode) or info.st_uid != 0 or info.st_mode & 0o022:
            raise RuntimeError("性能控制锁文件权限异常")
        fcntl.flock(fd, fcntl.LOCK_EX | fcntl.LOCK_NB)
        yield
    finally:
        os.close(fd)


class Journal:
    def __init__(self, path=JOURNAL):
        self.path = Path(path)

    def save(self, record):
        fd, temporary = tempfile.mkstemp(prefix=".mechrevo-turbo-", dir=self.path.parent)
        try:
            with os.fdopen(fd, "w") as stream:
                json.dump(record, stream, ensure_ascii=False)
                stream.flush()
                os.fsync(stream.fileno())
            os.replace(temporary, self.path)
            directory = os.open(self.path.parent, os.O_DIRECTORY | os.O_RDONLY)
            try:
                os.fsync(directory)
            finally:
                os.close(directory)
        finally:
            if os.path.exists(temporary):
                os.unlink(temporary)

    def clear(self):
        self.path.unlink(missing_ok=True)


def validate_status(data):
    if not isinstance(data, dict) or data.get("abi") != 1:
        raise RuntimeError("内核lease协议版本不匹配；旧experimental桥不可使用")
    if data.get("read_rc", 0) != 0:
        raise RuntimeError("内核固件状态读取失败")
    for key in ("active", "standard_turbo", "round_adapter", "return_present", "recovery_needed",
                "combined_supported", "game_supported"):
        if type(data.get(key)) is not bool:
            raise RuntimeError(f"桥状态缺少有效字段: {key}")
    for key in ("sequence", "last_rc", "cstm", "gatm"):
        if type(data.get(key)) is not int:
            raise RuntimeError(f"桥状态缺少有效整数: {key}")
    if data["sequence"] < 0 or data["cstm"] not in (0, 1) or data["gatm"] not in (0, 1):
        raise RuntimeError("桥返回未验证的工作负载状态")
    if data.get("variant") not in ("quiet", "extreme"):
        raise RuntimeError("桥返回未验证的狂暴子模式")
    if data.get("last_ack") is not None and (type(data["last_ack"]) is not int or not 0 <= data["last_ack"] < 2**64):
        raise RuntimeError("桥返回无效ACK")
    if data["return_present"] and data.get("last_ack") is None:
        raise RuntimeError("桥声明WMI有返回但未提供整数")
    return data


class Device:
    def __init__(self, path=DEVICE):
        self.fd = os.open(path, os.O_RDWR | os.O_CLOEXEC | os.O_NOFOLLOW)
        info = os.fstat(self.fd)
        if not stat.S_ISCHR(info.st_mode) or info.st_uid != 0 or info.st_mode & 0o077:
            self.close()
            raise RuntimeError("性能设备必须为root专用字符设备")
        try:
            self.status()
        except BaseException:
            self.close()
            raise

    def status(self):
        return validate_status(json.loads(os.pread(self.fd, 8192, 0)))

    def command(self, command):
        allowed = {"acquire quiet", "acquire extreme", "variant quiet", "variant extreme",
                   "heartbeat", "release", *COMMANDS.values()}
        if command not in allowed:
            raise ValueError("未支持的内核命令")
        before = self.status()["sequence"]
        payload = (command + "\n").encode("ascii")
        count = os.write(self.fd, payload)
        after = self.status()
        if count != len(payload) or after["sequence"] <= before or after["last_rc"] != 0:
            raise RuntimeError("内核事务未完成或回读失败")
        return after

    def close(self):
        if getattr(self, "fd", -1) >= 0:
            os.close(self.fd)
            self.fd = -1


class Controller:
    """Serial supervisor. Kernel owns snapshots, mask restoration and emergency exit."""
    def __init__(self, device, journal, lock, *, machine=identity, tuning_present=TUNING.exists,
                 runtime_ready=check_runtime, powerd_ready=check_powerd_active):
        self.device, self.journal, self.lock = device, journal, lock
        self.machine, self.tuning_present = machine, tuning_present
        self.runtime_ready = runtime_ready
        self.powerd_ready = powerd_ready
        self.state = SchedulerState()
        self.suspended = False
        self.record = None

    def status(self):
        return {"bridge": self.device.status(), "scheduler": asdict(self.state),
                "suspended": self.suspended, "managed": self.record is not None}

    def _save(self, phase, **extra):
        record = dict(self.record or {"version": 1, "identity": self.machine(),
                                     "started_at": time.time()})
        record.update(phase=phase, updated_at=time.time(), **extra)
        self.journal.save(record)
        self.record = record

    def _release(self, reason):
        # Always issue release, including uncertain acquire/command failures.
        self._save("releasing", reason=reason)
        status = self.device.command("release")
        if status["active"] or status["recovery_needed"]:
            raise RuntimeError("内核lease仍活跃或恢复未完成，保留恢复日志")
        self._save("released", reason=reason, final=status)
        print(json.dumps({"event": "turbo_released", "reason": reason,
                          "stop_reason": status.get("stop_reason")}, ensure_ascii=False), flush=True)
        self.journal.clear()
        self.record = None
        self.state = SchedulerState()
        return status

    def release(self, reason="requested"):
        with self.lock():
            return self._release(reason)

    def select(self, variant):
        if variant not in ("quiet", "extreme"):
            raise ValueError("子模式仅支持quiet/extreme")
        with self.lock():
            check_identity(self.machine())
            try:
                self.runtime_ready()  # Recheck after every wake, even in an old daemon.
            except Exception:
                if self.device.status()["active"]:
                    self._release("runtime_unhealthy")
                raise
            if self.suspended or self.tuning_present():
                raise RuntimeError("休眠准备或自定义调校期间禁止接管狂暴档")
            before = self.device.status()
            if not before["standard_turbo"] or not before["round_adapter"]:
                raise RuntimeError("仅支持原装圆口电源下的普通狂暴档")
            if not before["combined_supported"] or (not before["active"] and before["recovery_needed"]):
                raise RuntimeError("固件未报告联合调度支持，或先前恢复尚未完成")
            if before["active"] and self.record is None:
                raise RuntimeError("未持有此活动lease，拒绝接管")
            self._save("changing" if before["active"] else "acquiring", before=before, variant=variant,
                       acquired_before=(self.record or {}).get("acquired_before", before))
            try:
                result = self.device.command(("variant " if before["active"] else "acquire ") + variant)
                if not result["active"] or result["variant"] != variant:
                    raise RuntimeError("子模式回读不一致")
                self._save("active", final=result)
                return result
            except BaseException:
                self._release("select_failed")
                raise

    def tick(self, sample=None):
        with self.lock():
            status = self.device.status()
            if not status["active"]:
                if self.record is not None:
                    # Kernel PM/mode-key/watchdog already restored; clear only after confirmation.
                    self._release(status.get("stop_reason", "kernel_released"))
                return self.status()
            try:
                check_identity(self.machine())
                if (self.suspended or self.tuning_present() or not status["standard_turbo"]
                        or not status["round_adapter"]):
                    self._release("eligibility_changed")
                    return self.status()
                if self.record is None:
                    raise RuntimeError("活动lease缺少本进程事务记录")
                original = self.record["acquired_before"]
                if any(status[key] != original[key] for key in ("combined_supported", "game_supported")):
                    self._release("capability_changed")
                    return self.status()
                # Only a bounded service liveness check here; ACPI hashes remain
                # startup/select gates. No renewal after powerd disappears.
                self.powerd_ready()
                self.device.command("heartbeat")
                if sample is None:
                    return self.status()
                replies = {Operation.COMBINED_ON: 0, Operation.COMBINED_OFF: 0}
                executed = []
                # Re-evaluate after each actual ACK. This preserves GAME_OFF before
                # COMBINED_ON and the OEM's failed-ACK GAME_ON fallback ordering.
                while True:
                    transition = advance(self.state, sample, replies, round_adapter=True,
                                         game_supported=status["game_supported"])
                    if tuple(executed) != transition.operations[:len(executed)]:
                        raise RuntimeError("实际ACK改变了已执行操作前缀")
                    if len(executed) == len(transition.operations):
                        break
                    operation = transition.operations[len(executed)]
                    self._save("applying", operation=operation.value,
                               scheduler_before=asdict(self.state))
                    response = self.device.command(COMMANDS[operation])
                    if not response["active"] or not response["return_present"]:
                        raise RuntimeError("实际WMI ACK缺失或lease已归还")
                    if type(response["last_ack"]) is not int:
                        raise RuntimeError("实际WMI ACK不是整数")
                    ack = response["last_ack"] & 0xFF  # OEM GetSetULong takes the low byte.
                    # The bridge verifies readback; the model still gets the actual
                    # byte, never a fabricated BB/AA based on desired state.
                    if operation in replies:
                        replies[operation] = ack
                    elif ack != EXPECTED_ACK[operation]:
                        raise RuntimeError("游戏负载通知ACK不匹配")
                    executed.append(operation)
                self.state = transition.state
                self._save("active", scheduler=asdict(self.state),
                           operations=[op.value for op in executed], final=self.device.status())
                return self.status()
            except BaseException:
                self._release("poll_failed")
                raise

    def suspend(self):
        self.suspended = True
        return self.release("sleep")

    def resume(self):
        # Preserve firmware's post-resume choice; explicit select is required.
        self.suspended = False
        return self.status()


class Samples:
    def __init__(self):
        self.previous = None

    def read(self):
        values = list(map(int, Path("/proc/stat").read_text().splitlines()[0].split()[1:9]))
        now = (sum(values), values[3] + values[4])
        previous, self.previous = self.previous, now
        if previous is None:
            return None
        total, idle = now[0] - previous[0], now[1] - previous[1]
        if total <= 0 or not 0 <= idle <= total:
            raise RuntimeError("CPU采样计数不连续")
        gpu = subprocess.run(["/usr/bin/nvidia-smi", "-i", "0", "--query-gpu=utilization.gpu",
                              "--format=csv,noheader,nounits"], capture_output=True,
                             text=True, timeout=0.8, check=True).stdout.strip()
        if not gpu.isdecimal():
            raise RuntimeError("NVIDIA利用率采样无效")
        return LoadSample(round(100 * (total - idle) / total), int(gpu))


def dispatch(controller, request):
    if not isinstance(request, dict) or set(request) not in ({"command"}, {"command", "variant"}):
        raise ValueError("请求格式无效")
    name = request["command"]
    if name == "select" and set(request) == {"command", "variant"}:
        return controller.select(request["variant"])
    if set(request) != {"command"}:
        raise ValueError("请求包含多余字段")
    if name == "status":
        return controller.status()
    if name == "release":
        return controller.release()
    if name == "suspend":
        return controller.suspend()
    if name == "resume":
        return controller.resume()
    raise ValueError("命令不支持")


def prepare_socket(path=SOCKET):
    """Caller holds DAEMON_LOCK. Remove only a dead, root-owned socket inode."""
    try:
        before = path.lstat()
    except FileNotFoundError:
        return
    if not stat.S_ISSOCK(before.st_mode) or before.st_uid != 0 or before.st_mode & 0o077:
        raise RuntimeError("拒绝移除非可信socket路径")
    with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as probe:
        probe.settimeout(0.2)
        try:
            probe.connect(str(path))
        except OSError as error:
            if error.errno != errno.ECONNREFUSED:
                raise RuntimeError("不能确认遗留socket已停止") from error
        else:
            raise RuntimeError("控制socket仍有活跃服务，拒绝替换")
    after = path.lstat()
    if (before.st_dev, before.st_ino) != (after.st_dev, after.st_ino):
        raise RuntimeError("socket路径在检查时发生变化")
    path.unlink()


def run_daemon():
    if os.geteuid() != 0:
        raise RuntimeError("守护进程需要root与已验收的专用内核lease")
    check_identity(identity())
    check_runtime()
    check_qualification()
    with profile_lock(DAEMON_LOCK):
        prepare_socket()
        _run_daemon_locked()


def _run_daemon_locked():
    device = Device()
    server = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
    controller = Controller(device, Journal(), profile_lock)
    stop = [False]
    for number in (signal.SIGTERM, signal.SIGINT):
        signal.signal(number, lambda *_: stop.__setitem__(0, True))
    bound = False
    try:
        # Separate daemon flock spans this entire function, including cleanup.
        server.bind(str(SOCKET))
        bound = True
        os.chmod(SOCKET, 0o600)
        server.listen(4)
        server.settimeout(0.1)
        if JOURNAL.exists():
            trusted_file(JOURNAL, private=True)
            controller.release("startup_reconcile")
        sampler = Samples()
        next_heartbeat, next_sample = 0.0, 0.0
        was_active = False
        while not stop[0]:
            now = time.monotonic()
            if now >= min(next_heartbeat, next_sample):
                try:
                    sample = None
                    active = device.status()["active"]
                    if active != was_active:
                        sampler.previous = None
                        was_active = active
                    if now >= next_sample:
                        if active:
                            sample = sampler.read()
                        next_sample = now + 1.5
                    controller.tick(sample)
                except BlockingIOError:
                    pass  # Other profile transaction wins; kernel lease remains bounded.
                except Exception as error:
                    print(json.dumps({"event": "turbo_error", "error": str(error)}, ensure_ascii=False), flush=True)
                    try:
                        controller.release("sampling_or_poll_error")
                    except Exception:
                        raise  # FD close is the kernel's final recovery path.
                if now >= next_heartbeat:
                    next_heartbeat = now + 1.0
            try:
                client, _ = server.accept()
            except socket.timeout:
                continue
            with client:
                client.settimeout(0.3)
                try:
                    _, uid, _ = struct.unpack("3i", client.getsockopt(socket.SOL_SOCKET, socket.SO_PEERCRED, 12))
                    if uid != 0:
                        raise PermissionError("仅接受root控制客户端")
                    payload = client.recv(2049)
                    if len(payload) > 2048 or b"\n" not in payload:
                        raise ValueError("请求过长或不完整")
                    response = {"ok": True, "result": dispatch(controller, json.loads(payload))}
                except Exception as error:
                    response = {"ok": False, "error": str(error)}
                client.sendall(json.dumps(response, ensure_ascii=False).encode() + b"\n")
    finally:
        try:
            controller.release("daemon_stop")
        except Exception as error:
            print(json.dumps({"event": "release_pending", "error": str(error)}, ensure_ascii=False), flush=True)
        device.close()
        server.close()
        if bound:
            SOCKET.unlink(missing_ok=True)


def client_request(request):
    info = SOCKET.lstat()
    if not stat.S_ISSOCK(info.st_mode) or info.st_uid != 0 or info.st_mode & 0o077:
        raise RuntimeError("守护进程控制socket权限异常")
    with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as client:
        client.settimeout(4)
        client.connect(str(SOCKET))
        client.sendall(json.dumps(request).encode() + b"\n")
        payload = bytearray()
        while b"\n" not in payload:
            block = client.recv(8192)
            if not block or len(payload) + len(block) > 32768:
                raise RuntimeError("守护进程响应不完整或过长")
            payload.extend(block)
        return json.loads(payload)


def doctor():
    current = identity()
    return {"candidate": True, "auth_required": "root for live operations",
            "identity": current, "identity_matches": current == {"kernel_release": KERNEL, "dmi": DMI},
            "bridge_exists": DEVICE.exists(), "qualification_exists": QUALIFICATION.exists(),
            "daemon_socket_exists": SOCKET.exists(), "live_read_performed": False}

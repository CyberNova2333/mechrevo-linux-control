#!/usr/bin/python3 -I
"""Unexecuted APPE-only contrast: off 115 -> 100, on 100 -> 115; DB off.

Default invocation prints its plan. --execute is for parent-reviewed manual use.
Only APPE bit0 is added to the reviewed mode/GPU register masks. No CPU, fan,
Logo, PMSF, WMI, service, UEFI or kernel writes are implemented.
The independent fork guard retains the profile flock and restores after parent
death or a four-second heartbeat gap. It cannot recover a kernel/power failure.
"""
import argparse
import fcntl
import hashlib
import importlib.machinery
import json
import os
from pathlib import Path
import select
import signal
import stat
import subprocess
import time
import types
import xml.etree.ElementTree as ET

BASE = 0xFE410000
HELPER = Path('/usr/local/libexec/mechrevo-profile-helper')
MASKS = {0x751: 255, 0x726: 128, 0x727: 64, 0x741: 1,
         0x743: 7, 0x744: 255, 0x745: 255, 0x746: 255}
MODE_REGS = (0x751, 0x726, 0x727)
GPU_REGS = (0x743, 0x744, 0x745, 0x746)
GROUPS = {'mode': MODE_REGS, 'gpu': GPU_REGS, 'appe': (0x741,)}
CPU_REGS = (0x783, 0x784, 0x785, 0x786, 0x787)
UEFI_VARIABLE = Path('/sys/firmware/efi/efivars/OemMagicVariable-9f33f85c-13ca-4fd1-9c4a-96217722c593')
PHASES = ((0, 115), (0, 100), (1, 100), (1, 115))
GUARD_SECONDS = 60
OBSERVE = (0x490, 0x7cc, 0x7d2, 0x7e8, 0x788, 0x7c3, 0x7d5, 0x7f7,
           0x706, 0x728, 0x741, 0x783, 0x784, 0x785, 0x786, 0x787,
           0x7c5, 0x7c6)
PLAN = {'candidate_not_executed': True, 'phases': [{'appe': a, 'ctgp_w': w} for a, w in PHASES],
        'ctva_bytes': [50, 35, 35, 50], 'samples_per_phase': 8, 'sample_sleep_s': 1,
        'absolute_guard_seconds': GUARD_SECONDS,
        'db_enabled': False, 'appe_changed': 'bit0 only', 'calls_pmsf': False,
        'uefi_gate': {'payload_bytes': 180, 'APVersionCheck_offset': '0x43', 'expected': 25},
        'basis': 'GFID=0x19 static base/range/DB=65/50/25; real UEFI APVersionCheck=25, EC 7e8 is NOT version',
        'writes': {hex(k): hex(v) for k, v in MASKS.items()}}
GPU_PROGRAM = """
import torch,time,signal
running=True
def stop(*_):
 global running
 running=False
signal.signal(signal.SIGTERM,stop)
torch.set_num_threads(2)
a=torch.randn((6144,6144),device='cuda',dtype=torch.float16);b=a.clone();c=torch.empty_like(a)
event=torch.cuda.Event(blocking=True)
print('READY',flush=True);end=time.monotonic()+70
while running and time.monotonic()<end:
 for _ in range(4):torch.mm(a,b,out=c)
 event.record();event.synchronize()
"""


def round_adapter(read):
    flags = read(0x7cc)
    return bool(flags & 1 or not flags & 15 and read(0x490) & 1)


def parse_ap_version(raw):
    # efivarfs prepends four attribute bytes; the OEM payload is exactly 180.
    if len(raw) != 184 or raw[4 + 0x43] != 25:
        raise RuntimeError('requires real UEFI OemMagicVariable payload180 APVersionCheck=25')
    return {'payload_bytes': 180, 'APVersionCheck': raw[4 + 0x43],
            'payload_sha256': hashlib.sha256(raw[4:]).hexdigest()}


def read_ap_version():
    fd = os.open(UEFI_VARIABLE, os.O_RDONLY | os.O_CLOEXEC | os.O_NOFOLLOW)
    try:
        info = os.fstat(fd)
        if info.st_uid or info.st_mode & 0o022 or not stat.S_ISREG(info.st_mode):
            raise RuntimeError('UEFI variable ownership mismatch')
        return parse_ap_version(os.read(fd, 185))
    finally:
        os.close(fd)


def cpu_fan_clean(read):
    return (all(read(r) == 0 for r in CPU_REGS)
            and not read(0x7c5) & 0x80 and not read(0x7c6) & 4)


class OwnershipLost(RuntimeError):
    pass


class Ownership:
    """Last readback-confirmed values, plus at most one in-flight write."""
    def __init__(self, confirmed, pending=None):
        self.confirmed = dict(confirmed)
        self.pending = pending
        self.relinquished = set()
        self.external_appe = False

    @classmethod
    def initial(cls, before):
        return cls({r: before[r] & MASKS[r] for r in MASKS})

    def accepts(self, reg):
        values = {self.confirmed[reg]}
        if self.pending is not None and self.pending[0] == reg:
            values.add(self.pending[1])
        return values

    def payload(self):
        return {'confirmed': {str(r): v for r, v in self.confirmed.items()},
                'pending': None if self.pending is None else {'reg': self.pending[0], 'value': self.pending[1]}}

    @classmethod
    def parse(cls, payload):
        if not isinstance(payload, dict) or set(payload) != {'confirmed', 'pending'}:
            raise ValueError('invalid ownership protocol')
        values = payload['confirmed']
        if not isinstance(values, dict) or set(values) != {str(r) for r in MASKS}:
            raise ValueError('invalid confirmed register set')
        confirmed = {int(r): v for r, v in values.items()}
        if any(type(v) is not int or v < 0 or v & ~MASKS[r] for r, v in confirmed.items()):
            raise ValueError('invalid confirmed bits')
        pending = payload['pending']
        if pending is not None:
            if not isinstance(pending, dict) or set(pending) != {'reg', 'value'}:
                raise ValueError('invalid pending write')
            r, v = pending['reg'], pending['value']
            if type(r) is not int or r not in MASKS or type(v) is not int or v < 0 or v & ~MASKS[r]:
                raise ValueError('invalid pending bits')
            pending = (r, v)
        return cls(confirmed, pending)


def write_mask(ec, reg, bits, pause=time.sleep, expected=None):
    mask = MASKS[reg]
    if type(bits) is not int or bits < 0 or bits & ~mask:
        raise ValueError('out-of-range masked write')
    current = ec.read(reg)
    if expected is not None and current & mask not in expected:
        raise OwnershipLost(f'external change immediately before write: {reg:03x}')
    ec.write_byte(reg, (current & ~mask) | bits)
    pause(.01)
    if ec.read(reg) & mask != bits:
        raise RuntimeError(f'write readback mismatch: {reg:03x}')


def restore(ec, before, ownership, pause=time.sleep):
    """Restore GPU -> APPE -> mode, with fresh ownership at each individual write."""
    errors = []
    def relinquish(name):
        ownership.relinquished.add(name)
        if name in ('appe', 'gpu'):
            # An external GPU owner needs its current APPE/custom context.
            ownership.relinquished.update(GROUPS)

    def check_appe_and_other_controls():
        if (ec.read(0x741) & 1 not in ownership.accepts(0x741)
                or not cpu_fan_clean(ec.read)):
            ownership.external_appe = True
            ownership.relinquished.update(GROUPS)
            raise OwnershipLost('external APPE/CPU/fan owner')

    def check_group(name):
        check_appe_and_other_controls()
        # Before releasing APPE or restoring mode, make sure another GPU owner
        # has not appeared after our last GPU rollback write.
        if name != 'gpu':
            gpu = {r: ec.read(r) & MASKS[r] for r in GPU_REGS}
            if any(v not in ownership.accepts(r) for r, v in gpu.items()):
                relinquish('gpu')
                raise OwnershipLost('external gpu owner')
        values = {r: ec.read(r) & MASKS[r] for r in GROUPS[name]}
        if any(v not in ownership.accepts(r) for r, v in values.items()):
            relinquish(name)
            raise OwnershipLost('external ' + name + ' owner')
        check_appe_and_other_controls()  # Cover handoff during the multi-register snapshot.
        return values

    # Parent has stopped. Resolve its final pending write, or our previous
    # rollback write that failed after acceptance; never accept an older value.
    if ownership.pending is not None:
        reg, _ = ownership.pending
        group = 'mode' if reg in MODE_REGS else 'appe' if reg == 0x741 else 'gpu'
        try:
            check_appe_and_other_controls()
            observed = ec.read(reg) & MASKS[reg]
            if observed not in ownership.accepts(reg):
                relinquish(group)
            else:
                ownership.confirmed[reg] = observed
            ownership.pending = None
        except OwnershipLost:
            ownership.pending = None
        except Exception as exc:
            errors.append(str(exc))
    # Disable our custom GPU enable bits before restoring old target bytes.
    # Admission requires all four original GPU registers to be zero.
    order = (0x743, 0x745, 0x746, 0x744, 0x741, 0x726, 0x727, 0x751)
    for reg in order:
        if errors:
            break  # Keep one pending write unambiguous for the next retry.
        group = 'mode' if reg in MODE_REGS else 'appe' if reg == 0x741 else 'gpu'
        if group in ownership.relinquished:
            continue
        try:
            current = check_group(group)[reg]
            old = before[reg] & MASKS[reg]
            if current == old:
                ownership.confirmed[reg] = old
                continue
            expected = {ownership.confirmed[reg]}
            ownership.pending = (reg, old)
            write_mask(ec, reg, old, pause, expected=expected)
            ownership.confirmed[reg] = old
            ownership.pending = None
        except OwnershipLost:
            relinquish(group)
            ownership.pending = None
        except Exception as exc:
            errors.append(f'{reg:03x}: {exc}')
    if not errors:
        for group in GROUPS:
            if group in ownership.relinquished:
                continue
            try:
                check_group(group)
            except OwnershipLost:
                pass
            except Exception as exc:
                errors.append(str(exc))
    skipped = [hex(r) for g in sorted(ownership.relinquished) for r in GROUPS[g]]
    return {'complete': not errors and not skipped and ownership.pending is None,
            'errors': errors, 'skipped': skipped,
            'mode_key_preserved': 'mode' in ownership.relinquished,
            'external_appe_preserved': ownership.external_appe}


class RawEC:
    def __init__(self, helper):
        self.helper = helper
        self.fd = os.open('/dev/mem', os.O_RDWR | os.O_CLOEXEC)

    def read(self, reg):
        if reg not in set(self.helper.REGISTERS) | set(OBSERVE) | set(MASKS):
            raise ValueError('read outside fixed scope')
        data = os.pread(self.fd, 1, BASE + reg)
        if len(data) != 1:
            raise OSError('short EC read')
        return data[0]

    def write_byte(self, reg, byte):
        if reg not in MASKS or type(byte) is not int or not 0 <= byte <= 255:
            raise ValueError('write outside fixed scope')
        if os.pwrite(self.fd, bytes([byte]), BASE + reg) != 1:
            raise OSError('short EC write')


class Transaction:
    def __init__(self, ec, before, pipe):
        self.ec, self.before, self.pipe = ec, before, pipe
        self.ownership = Ownership.initial(before)

    def heartbeat(self):
        payload = json.dumps(self.ownership.payload()).encode() + b'\n'
        if os.write(self.pipe, payload) != len(payload):
            raise OSError('guard heartbeat incomplete')

    def put(self, reg, bits):
        self.guard_state()
        if type(bits) is not int or bits < 0 or bits & ~MASKS[reg]:
            raise ValueError('invalid write intent')
        self.ownership.pending = (reg, bits)
        self.heartbeat()
        write_mask(self.ec, reg, bits, expected={self.ownership.confirmed[reg]})
        self.ownership.confirmed[reg] = bits
        self.ownership.pending = None
        self.heartbeat()  # Discard the previous value as soon as readback succeeds.

    def read(self, reg):
        return self.ec.read(reg)

    def write_mode(self, byte):
        self.put(0x751, byte)

    def write_flag(self, reg, enabled):
        if reg not in (0x726, 0x727) or type(enabled) is not bool:
            raise ValueError('unsupported helper flag')
        self.put(reg, MASKS[reg] if enabled else 0)

    def guard_state(self):
        if self.ownership.pending is not None:
            raise RuntimeError('unresolved write; guardian must restore before further writes')
        if not round_adapter(self.read):
            raise RuntimeError('round adapter lost')
        for reg, target in self.ownership.confirmed.items():
            if self.read(reg) & MASKS[reg] != target:
                raise RuntimeError(f'external profile change: {reg:03x}')
        if not cpu_fan_clean(self.read):
            raise RuntimeError('CPU/fan control changed externally')


def stop_load(pid):
    try:
        os.killpg(pid, signal.SIGTERM)
    except ProcessLookupError:
        pass


def guard_child(ec, before, pipe, gpu_pid, parent_pid, output, deadline=None):
    # Keep inherited flock/device until restoration is finished.
    signal.signal(signal.SIGINT, signal.SIG_IGN)
    signal.signal(signal.SIGTERM, signal.SIG_IGN)
    ownership = Ownership.initial(before)
    buffer = b''
    until = time.monotonic() + 4
    absolute = time.monotonic() + GUARD_SECONDS if deadline is None else deadline
    ended_by_eof = False
    monitor_error = None
    try:
        while time.monotonic() < min(until, absolute):
            ready, _, _ = select.select([pipe], [], [], max(0, min(until, absolute) - time.monotonic()))
            if not ready:
                break
            block = os.read(pipe, 4096)
            if not block:
                ended_by_eof = True
                break
            buffer += block
            while b'\n' in buffer:
                line, buffer = buffer.split(b'\n', 1)
                ownership = Ownership.parse(json.loads(line))
                until = time.monotonic() + 4
    except BaseException as exc:
        monitor_error = str(exc)
    if not ended_by_eof:
        # Prevent a stalled parent from later resuming a write after rollback.
        try:
            os.kill(parent_pid, signal.SIGKILL)
        except ProcessLookupError:
            pass
        time.sleep(.05)
    stop_load(gpu_pid)
    report = restore(ec, before, ownership)
    if report['errors']:
        time.sleep(.2)
        report = restore(ec, before, ownership)
    report.update(parent_eof=ended_by_eof, monitor_error=monitor_error)
    output.write_text(json.dumps(report, indent=2))
    return 0 if report['complete'] else 2


def telemetry(ec):
    xml = subprocess.check_output(['/usr/bin/nvidia-smi', '-i', '0', '-q', '-x'], text=True, timeout=2)
    root = ET.fromstring(xml)
    def field(*names):
        return next((root.findtext('.//' + n) for n in names if root.findtext('.//' + n) is not None), None)
    gpu_temp = field('gpu_temp')
    temperatures = [int(p.read_text()) / 1000 for hw in Path('/sys/class/hwmon').glob('hwmon*')
                    if (hw / 'name').exists() and (hw / 'name').read_text().strip() == 'coretemp'
                    for p in hw.glob('temp*_input')]
    if not temperatures or not gpu_temp:
        raise RuntimeError('temperature telemetry unavailable')
    if max(temperatures) >= 95 or float(gpu_temp.split()[0]) >= 78:
        raise RuntimeError('temperature guard')
    return {'at': time.time(), 'cpu_temp_c': max(temperatures), 'gpu_temp': gpu_temp,
            'power_draw': field('instant_power_draw', 'power_draw', 'average_power_draw'),
            'enforced_limit': field('enforced_power_limit'), 'requested_limit': field('power_limit'),
            'gpu_util': field('gpu_util'),
            'ec': {f'{r:03x}': ec.read(r) for r in (*MASKS, *OBSERVE)}}


def execute():
    if os.geteuid() != 0 or os.uname().release != '6.8.0-149-generic':
        raise RuntimeError('requires reviewed root execution on exact existing kernel')
    st = HELPER.stat()
    if st.st_uid != 0 or st.st_mode & 0o022 or not stat.S_ISREG(st.st_mode):
        raise RuntimeError('helper ownership mismatch')
    helper = types.ModuleType('reviewed_profile_helper')
    importlib.machinery.SourceFileLoader(helper.__name__, str(HELPER)).exec_module(helper)
    helper.check_machine()
    firmware_version = read_ap_version()  # Real UEFI payload, never EC 0x7e8.
    # No competing experimental module, daemon or pending tuning journal.
    if any(p.exists() for p in (Path('/sys/kernel/mechrevo_turbo'), Path('/dev/mechrevo-turbo'),
                               Path('/run/mechrevo-turbo.sock'), Path('/run/mechrevo-turbo.json'),
                               Path('/run/mechrevo-tuning.json'))):
        raise RuntimeError('another controller or pending recovery exists')
    health = subprocess.run(['/usr/local/libexec/mechrevo-acpi-check', '--summary'],
                            capture_output=True, text=True, timeout=3)
    if health.returncode or json.loads(health.stdout).get('healthy') is not True:
        raise RuntimeError('ACPI baseline unhealthy')
    subprocess.run(['/usr/bin/systemctl', 'is-active', '--quiet', 'nvidia-powerd.service'], check=True, timeout=2)
    lock = os.open('/run/mechrevo-profile.lock', os.O_CREAT | os.O_RDWR | os.O_CLOEXEC | os.O_NOFOLLOW, 0o600)
    info = os.fstat(lock)
    if not stat.S_ISREG(info.st_mode) or info.st_uid or info.st_mode & 0o022:
        os.close(lock)
        raise RuntimeError('profile lock ownership mismatch')
    fcntl.flock(lock, fcntl.LOCK_EX | fcntl.LOCK_NB)
    ec = RawEC(helper)
    before = {r: ec.read(r) for r in set(MASKS) | set(OBSERVE) | set(helper.REGISTERS)}
    if (not helper.standard_state(before) or helper.mode_name(before) != 'turbo'
            or before[0x7d2] & 31 != 25 or not cpu_fan_clean(ec.read)
            or any(before[r] for r in (0x743, 0x744, 0x745, 0x746)) or not round_adapter(ec.read)):
        raise RuntimeError('trial requires clean ordinary turbo, GFID=0x19, zero CPU/GPU request bytes and fan enables off')
    directory = Path('/run/mechrevo-custom-gpu-appe-trial-' + str(os.getpid()))
    directory.mkdir(mode=0o700)
    (directory / 'before.json').write_text(json.dumps({hex(k): v for k, v in before.items()}, indent=2))
    (directory / 'firmware-version.json').write_text(json.dumps(firmware_version, indent=2))
    log = (directory / 'gpu-load.log').open('w')
    deadline = time.monotonic() + GUARD_SECONDS  # Includes GPU readiness, not just hardware writes.
    gpu = subprocess.Popen(['/usr/sbin/runuser', '-u', 'cybernova', '--',
                            '/home/cybernova/AI/workbench/.venv/bin/python', '-I', '-c', GPU_PROGRAM],
                           stdout=log, stderr=log, start_new_session=True)
    child = None
    write_pipe = None
    rows = []
    try:
        for _ in range(30):
            if 'READY' in (directory / 'gpu-load.log').read_text():
                break
            if gpu.poll() is not None:
                raise RuntimeError('GPU load exited before readiness')
            time.sleep(.5)
        else:
            raise RuntimeError('GPU load readiness timeout')
        telemetry(ec)  # Require valid temperature samples before any write.
        read_pipe, write_pipe = os.pipe()
        parent_pid = os.getpid()
        child = os.fork()
        if child == 0:
            os.close(write_pipe)
            os._exit(guard_child(ec, before, read_pipe, gpu.pid, parent_pid, directory / 'restoration.json', deadline))
        os.close(read_pipe)
        tx = Transaction(ec, before, write_pipe)
        def interrupted(*_):
            raise KeyboardInterrupt
        signal.signal(signal.SIGINT, interrupted)
        signal.signal(signal.SIGTERM, interrupted)
        helper.set_mode(tx, 'custom')  # Existing, qualified custom-mode entry.
        tx.put(0x744, 50)
        tx.put(0x745, 0)
        tx.put(0x746, 0)
        tx.put(0x743, tx.read(0x743) & 5)  # DB off; then function control on.
        tx.put(0x743, (tx.read(0x743) & 7) | 1)
        tx.put(0x743, (tx.read(0x743) & 7) | 4)
        for appe, target in PHASES:
            # At the contrast boundary target remains100: the only changed bit
            # is APPE. Final phase changes only target100->115, APPE remains1.
            if tx.read(0x744) != target - 65:
                tx.put(0x744, target - 65)
            if tx.read(0x741) & 1 != appe:
                tx.put(0x741, appe)
            for _ in range(8):
                tx.heartbeat()
                tx.guard_state()
                if gpu.poll() is not None:
                    raise RuntimeError('GPU workload ended early')
                row = telemetry(ec)
                row['target_w'] = target
                row['appe'] = appe
                rows.append(row)
                print(json.dumps(row), flush=True)
                time.sleep(1)
    finally:
        stop_load(gpu.pid)
        if write_pipe is not None:
            os.close(write_pipe)  # Guardian exclusively restores after EOF.
        if child:
            _, result = os.waitpid(child, 0)
            print(json.dumps({'restoration': str(directory / 'restoration.json'), 'guard_status': result}), flush=True)
        try:
            gpu.wait(timeout=5)
        except subprocess.TimeoutExpired:
            os.killpg(gpu.pid, signal.SIGKILL)
            gpu.wait()
        (directory / 'samples.json').write_text(json.dumps(rows, indent=2))
        log.close()
        os.close(ec.fd)
        os.close(lock)
    report = json.loads((directory / 'restoration.json').read_text())
    if report.get('complete') is not True:
        raise RuntimeError('restoration not complete; inspect report before another test')
    print(json.dumps({'output': str(directory), 'complete': True}), flush=True)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--execute', action='store_true')
    arguments = parser.parse_args()
    if arguments.execute:
        execute()
    else:
        print(json.dumps(PLAN, indent=2, ensure_ascii=False))

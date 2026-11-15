"""Real cross-process locks; fake HID/helper I/O only."""
import json
import multiprocessing
import os
from pathlib import Path
import tempfile
import threading
import time
import unittest
from unittest.mock import Mock, patch

from iteglow import cli, devices, state
from iteglow.lighting_policy import default_config
from iteglow.lighting_policy_session import ManualState, PolicyRunner
from iteglow.zone_lock import ZoneTransaction


def manual_worker(runtime, saved, device, ready, finish, value):
    os.environ['XDG_RUNTIME_DIR'] = runtime
    with ZoneTransaction(device):
        ready.set()
        if not finish.wait(4):
            raise RuntimeError('test handoff timed out')
        Path(saved).write_text(json.dumps({device: value}))


def hid_worker(runtime, log, name, started, continue_send):
    os.environ['XDG_RUNTIME_DIR'] = runtime
    fake = object.__new__(devices.HidLed)
    fake.led_name = devices.KEYBOARD[0]
    fake.fd = 123
    count = 0
    def ioctl(fd, command, data):
        nonlocal count
        count += 1
        with open(log, 'a') as output:
            output.write(f'{name}{count}\n')
        if name == 'send' and count == 1:
            started.set()
            if not continue_send.wait(4):
                raise RuntimeError('test send timed out')
        return 0
    with patch.object(devices.fcntl, 'ioctl', side_effect=ioctl):
        if name == 'send':
            fake.send([('feature', bytes([1, 2])), ('feature', bytes([3, 4]))])
        else:
            started.set()
            fake.query(0x88)


def save_worker(runtime, saved, label, start):
    os.environ['XDG_RUNTIME_DIR'] = runtime
    original = state.load
    def delayed_load(path):
        data = original(path)
        time.sleep(.1)  # Without a shared state lock, both read the old object.
        return data
    start.wait(4)
    with patch.object(state, 'user_state_path', return_value=saved), patch.object(state, 'load', side_effect=delayed_load):
        with ZoneTransaction(label):
            if not state.save(label, {'mode': 'off'}):
                raise RuntimeError('test state save failed')


class TransactionTest(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.env = patch.dict(os.environ, {'XDG_RUNTIME_DIR': str(self.root)})
        self.env.start()
        self.ctx = multiprocessing.get_context('spawn')
        self.children = []

    def tearDown(self):
        for child in self.children:
            child.join(1)
            if child.is_alive():
                child.terminate(); child.join(2)
        self.env.stop(); self.temp.cleanup()

    def spawn(self, target, *args):
        child = self.ctx.Process(target=target, args=args)
        child.start(); self.children.append(child)
        return child

    def test_complete_multireport_send_and_query_cannot_interleave(self):
        first, second, finish = self.ctx.Event(), self.ctx.Event(), self.ctx.Event()
        log = self.root / 'traffic'
        sender = self.spawn(hid_worker, str(self.root), str(log), 'send', first, finish)
        self.assertTrue(first.wait(3))
        query = self.spawn(hid_worker, str(self.root), str(log), 'query', second, finish)
        self.assertTrue(second.wait(3))
        time.sleep(.15)
        self.assertEqual(log.read_text().splitlines(), ['send1'])
        finish.set()
        sender.join(3); query.join(3)
        self.assertEqual((sender.exitcode, query.exitcode), (0, 0))
        self.assertEqual(log.read_text().splitlines(), ['send1', 'send2', 'query1', 'query2'])

    def test_policy_waiter_rechecks_manual_after_another_process_finishes(self):
        for device in ('keyboard', 'lightbar', 'logo'):
            with self.subTest(device=device):
                saved = self.root / 'manual.json'
                on = {'mode': 'solid', 'brightness': 50, 'color': [255, 0, 0]}
                if device == 'lightbar':
                    on['mode'] = 'static'
                off = dict(on, brightness=0) if device == 'logo' else {'mode': 'off'}
                saved.write_text(json.dumps({device: on}))
                held, finish = self.ctx.Event(), self.ctx.Event()
                child = self.spawn(manual_worker, str(self.root), str(saved), device, held, finish, off)
                self.assertTrue(held.wait(3))
                planned = threading.Event()
                def manual():
                    value = ManualState(json.loads(saved.read_text()))
                    planned.set()
                    return value
                config = default_config()
                config.update(enabled=True, power_enabled=True)
                config['brightness'][device]['ac'] = 25
                writer = Mock()
                runner = PolicyRunner(active=lambda: True, config=lambda: config,
                                      manual=manual, power=lambda: 'ac', idle=lambda: None,
                                      writer=writer, apply=True)
                results = []
                thread = threading.Thread(target=lambda: results.append(runner.tick()))
                thread.start()
                self.assertTrue(planned.wait(2))
                self.assertTrue(thread.is_alive())
                finish.set(); thread.join(3); child.join(3)
                self.assertFalse(thread.is_alive())
                self.assertEqual(child.exitcode, 0)
                writer.assert_not_called()
                self.assertEqual(results[0]['skipped'], [device + ': manual state changed'])
                self.assertFalse(runner.memory.zones)

    def test_two_different_zone_manual_saves_keep_both_records(self):
        saved = self.root / 'state.json'
        start = self.ctx.Event()
        children = [self.spawn(save_worker, str(self.root), str(saved), label, start)
                    for label in ('keyboard', 'lightbar')]
        start.set()
        for child in children:
            child.join(3)
            self.assertEqual(child.exitcode, 0)
        self.assertEqual(json.loads(saved.read_text()), {'keyboard': {'mode': 'off'}, 'lightbar': {'mode': 'off'}})

    def test_nested_transactions_and_error_release(self):
        first, second = ZoneTransaction('keyboard'), ZoneTransaction('keyboard')
        with self.assertRaisesRegex(ValueError, 'test'):
            with first, second:
                self.assertEqual(first.entry.depth, 2)
                raise ValueError('test')
        self.assertEqual(first.entry.depth, 0)
        self.assertIsNone(first.entry.fd)
        with first:
            self.assertEqual(first.entry.depth, 1)

    def test_async_transactions_cannot_reenter_on_same_gtk_thread(self):
        first = ZoneTransaction('logo', reentrant=False)
        second = ZoneTransaction('logo', reentrant=False)
        self.assertTrue(first.acquire(blocking=False))
        try:
            self.assertFalse(second.acquire(blocking=False))
            self.assertFalse(first.acquire(blocking=False))
            self.assertEqual(first.entry.depth, 1)
        finally:
            first.release()
        self.assertTrue(second.acquire(blocking=False))
        second.release()

    def test_lock_symlink_or_public_permissions_fail_closed(self):
        path = self.root / 'iteglow-keyboard.lock'
        target = self.root / 'unrelated'
        target.write_text('untouched')
        path.symlink_to(target)
        with self.assertRaises(OSError):
            with ZoneTransaction('keyboard'):
                self.fail('unsafe lock accepted')
        self.assertEqual(target.read_text(), 'untouched')
        path.unlink(); path.write_text(''); path.chmod(0o666)
        with self.assertRaises(RuntimeError):
            with ZoneTransaction('keyboard'):
                self.fail('public lock accepted')

    @patch.object(cli.os, 'geteuid', return_value=0)
    @patch.object(cli, '_open')
    @patch.object(cli.devices, 'available_devices')
    def test_root_cli_writers_fail_before_device_or_state_access(self, available, opened, uid):
        import contextlib
        import io
        with patch.object(state, 'load') as load:
            for command in (cli.cmd_set, cli.cmd_off, cli.cmd_restore):
                output = io.StringIO()
                with contextlib.redirect_stderr(output):
                    self.assertEqual(command(None), 1)
                self.assertIn('without sudo', output.getvalue())
            opened.assert_not_called(); available.assert_not_called(); load.assert_not_called()


if __name__ == '__main__':
    unittest.main()

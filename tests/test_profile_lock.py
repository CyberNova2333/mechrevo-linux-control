"""Bounded foreground lock wait; no real EC or runtime service operations."""
from contextlib import ExitStack
from types import SimpleNamespace
import unittest
from unittest.mock import patch
from test_profile_helper import helper


class ProfileLockTest(unittest.TestCase):
    def setup_main(self, stack, flock):
        stack.enter_context(patch.object(helper,'check_machine'))
        stack.enter_context(patch.object(helper,'turbo_request',return_value={'active':False,'recovery_needed':False}))
        stack.enter_context(patch.object(helper.subprocess,'run',return_value=SimpleNamespace(returncode=0)))
        stack.enter_context(patch.object(helper,'STATE',SimpleNamespace(exists=lambda:True)))
        stack.enter_context(patch.object(helper.os,'open',return_value=123))
        stack.enter_context(patch.object(helper.os,'fstat',return_value=SimpleNamespace(st_mode=0o100600,st_uid=0)))
        closed=stack.enter_context(patch.object(helper.os,'close'))
        stack.enter_context(patch.object(helper.fcntl,'flock',side_effect=flock))
        ec=stack.enter_context(patch.object(helper,'EC'))
        restored=stack.enter_context(patch.object(helper,'restore_tuning'))
        stack.enter_context(patch.object(helper,'status_with_turbo',return_value={'done':True}))
        return ec,restored,closed

    def test_short_foreground_contention_waits_then_opens_ec_and_resets(self):
        with ExitStack()as stack:
            ec,restored,closed=self.setup_main(stack,[BlockingIOError(),BlockingIOError(),None])
            stack.enter_context(patch.object(helper.time,'monotonic',side_effect=[0,0.001,0.021]))
            sleep=stack.enter_context(patch.object(helper.time,'sleep'))
            self.assertEqual(helper.main(['reset']),{'done':True})
            self.assertEqual([x.args[0]for x in sleep.call_args_list],[0.02,0.02])
            ec.assert_called_once_with(writable=True);restored.assert_called_once()
            closed.assert_called_once_with(123)

    def test_foreground_timeout_does_not_open_ec_or_restore(self):
        config={'pl1':35,'pl2':35,'cpu_fan':helper.MIN_DUTY,'gpu_fan':helper.MIN_DUTY}
        for command in (['status'],['reset'],['set','office'],['logo-status'],['logo','off'],['apply',helper.json.dumps(config)]):
            with self.subTest(command=command),ExitStack()as stack:
                ec,restored,closed=self.setup_main(stack,BlockingIOError())
                stack.enter_context(patch.object(helper.time,'monotonic',side_effect=[0,1.49,1.51]))
                sleep=stack.enter_context(patch.object(helper.time,'sleep'))
                with self.assertRaisesRegex(BlockingIOError,'性能控制忙.*1.5秒'):helper.main(command)
                self.assertLessEqual(sleep.call_args.args[0],0.02)
                ec.assert_not_called();restored.assert_not_called();closed.assert_called_once_with(123)

    def test_background_reconcile_immediately_yields_without_waiting_or_ec(self):
        with ExitStack()as stack:
            ec,restored,closed=self.setup_main(stack,BlockingIOError())
            sleep=stack.enter_context(patch.object(helper.time,'sleep'))
            with self.assertRaises(BlockingIOError):helper.main(['reconcile'])
            sleep.assert_not_called();ec.assert_not_called();restored.assert_not_called()
            closed.assert_called_once_with(123)

    def test_non_contention_flock_error_is_not_retried(self):
        with patch.object(helper.fcntl,'flock',side_effect=OSError('I/O failed')),patch.object(helper.time,'sleep')as sleep:
            with self.assertRaisesRegex(OSError,'I/O failed'):helper.acquire_profile_lock(123)
            sleep.assert_not_called()


if __name__=='__main__':unittest.main()

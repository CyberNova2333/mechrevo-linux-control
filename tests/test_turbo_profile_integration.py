import json
from pathlib import Path
from types import SimpleNamespace
import unittest
from unittest.mock import patch
from test_profile_helper import helper, FakeEC


class TurboIntegrationTest(unittest.TestCase):
    def test_optional_release_error_preserves_raw_active_and_blocks_foreground_ec(self):
        raw={'abi':1,'read_rc':0,'active':True,'recovery_needed':False}
        with (patch.object(helper,'TURBO_HELPER',SimpleNamespace(exists=lambda:True)),
              patch.object(helper,'TURBO_JOURNAL',SimpleNamespace(exists=lambda:False)),
              patch.object(helper,'TURBO_DEVICE',SimpleNamespace(exists=lambda:True)),
              patch.object(helper.subprocess,'run',return_value=SimpleNamespace(returncode=1,stdout='{"ok":false,"error":"busy"}')),
              patch.object(helper.os,'open',return_value=123)as opened,
              patch.object(helper.os,'fstat',return_value=SimpleNamespace(st_mode=0o020600,st_uid=0)),
              patch.object(helper.os,'pread',return_value=json.dumps(raw).encode()),
              patch.object(helper.os,'close'),patch.object(helper,'check_machine'),patch.object(helper,'EC')as ec):
            self.assertTrue(helper.turbo_request(['release'],optional=True)['active'])
            with self.assertRaisesRegex(RuntimeError,'尚未释放'):helper.main(['reset'])
            ec.assert_not_called()
            self.assertTrue(all(call.args[0]==helper.TURBO_DEVICE for call in opened.call_args_list))

    def test_mode_and_tuning_release_turbo_before_profile_lock(self):
        for args in [['set','office'],['reset']]:
            events=[]
            def release(*_args,**_kwargs):events.append('release');return {'active':False,'available':True}
            def open_lock(*_args):events.append('lock');raise OSError('test stops before opening hardware')
            with patch.object(helper,'check_machine'),patch.object(helper,'turbo_request',side_effect=release),patch.object(helper.os,'open',side_effect=open_lock):
                with self.assertRaises(OSError):helper.main(args)
            self.assertEqual(events,['release','lock'])

    def test_failed_release_blocks_mode_write(self):
        for outcome in [RuntimeError('release failed'),{'active':True},{'active':False,'recovery_needed':True}]:
            with patch.object(helper,'check_machine'),patch.object(helper,'turbo_request',side_effect=outcome if isinstance(outcome,Exception) else None,return_value=outcome),patch.object(helper.os,'open') as opened:
                with self.assertRaises(RuntimeError):helper.main(['set','office'])
                opened.assert_not_called()

    def test_missing_daemon_with_recovery_journal_is_not_ignored(self):
        with patch.object(helper,'TURBO_HELPER') as binary,patch.object(helper,'TURBO_JOURNAL') as journal,patch.object(helper,'TURBO_DEVICE',SimpleNamespace(exists=lambda:False)):
            binary.exists.return_value=True;journal.exists.return_value=True
            with patch.object(helper.subprocess,'run',return_value=SimpleNamespace(returncode=1,stdout=json.dumps({'ok':False,'error':'stale socket'}))):
                with self.assertRaises(RuntimeError):helper.turbo_request(['release'],optional=True)
            journal.exists.return_value=False
            binary.exists.return_value=False
            self.assertEqual(helper.turbo_request(['status'],optional=True),{'available':False,'active':False})

    def test_only_managed_standard_turbo_exposes_release_then_switch(self):
        ec=FakeEC();ec.values[0x741]=1
        with patch.object(helper,'read_state',return_value=None):
            result=helper.status_with_turbo(ec,{'available':True,'active':True,'standard_turbo':True})
            self.assertTrue(result['switchable'])
            ec.values[helper.MODE]=0xA0
            self.assertFalse(helper.status_with_turbo(ec,{'available':True,'active':True,'standard_turbo':True})['switchable'])


if __name__=='__main__':unittest.main()

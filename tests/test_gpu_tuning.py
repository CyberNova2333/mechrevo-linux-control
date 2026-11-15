"""Offline optional GPU transaction and durable ownership contracts."""
from copy import deepcopy
from pathlib import Path
from types import SimpleNamespace
import json
import tempfile
import unittest
from unittest.mock import patch
from test_tuning import helper, TuningEC


class MemoryPath:
    def __init__(self, owner):self.owner=owner
    def unlink(self):self.owner.record=None
    def exists(self):return self.owner.record is not None


class GPUTransactionTest(unittest.TestCase):
    def setUp(self):
        self.record=None;self.records=[];self.fail_save=None;self.after_save=None
        self.config={'pl1':35,'pl2':35,'cpu_fan':helper.MIN_DUTY.copy(),
                     'gpu_fan':helper.MIN_DUTY.copy(),'gpu_ctgp':100}
        self.ec=TuningEC();self.ec.values.update({0x7D2:25,0x7CC:129,0x741:128,0x7AB:0xE1,0x7C5:3})
        self.before=dict(self.ec.values)
        def save(value):
            if self.fail_save and self.fail_save(value):
                self.fail_save=None
                raise OSError('injected durable save failure')
            self.record=deepcopy(value);self.records.append(deepcopy(value))
            if self.after_save:self.after_save(value)
        self.patches=[patch.object(helper,'read_state',lambda:deepcopy(self.record)),
                      patch.object(helper,'save_state',save),patch.object(helper,'STATE',MemoryPath(self)),
                      patch.object(helper,'check_gpu_capability',lambda ec:None),
                      patch.object(helper,'check_gpu_runtime',lambda:None)]
        for item in self.patches:item.start()
        self.addCleanup(lambda:[item.stop()for item in reversed(self.patches)])

    def apply(self,config=None):return helper.apply_tuning(self.ec,config or self.config,pause=lambda _:None)
    def restore(self):return helper.restore_tuning(self.ec)

    def test_optional_gpu_orders_appe_target_then_enable_and_restores_logo_bits(self):
        self.assertTrue(self.apply()['tuning_active'])
        self.assertEqual(self.ec.values[0x743]&7,5)
        self.assertEqual(self.ec.values[0x744],35)
        self.assertEqual((self.ec.values[0x745],self.ec.values[0x746]),(0,0))
        addresses=[r for r,_ in self.ec.writes]
        self.assertLess(addresses.index(0x741),addresses.index(0x744))
        self.assertLess(addresses.index(0x744),addresses.index(0x743))
        self.assertEqual(self.ec.values[0x7C5]&7,3)
        self.ec.writes.clear();self.restore()
        addresses=[r for r,_ in self.ec.writes]
        self.assertEqual(addresses[0],0x743)
        self.assertLess(addresses.index(0x744),addresses.index(0x741))
        self.assertLess(addresses.index(0x741),addresses.index(0x726))
        self.assertEqual(self.ec.values,self.before)
        self.assertIsNone(self.record)

    def test_absent_gpu_field_never_owns_or_writes_gpu(self):
        config={k:v for k,v in self.config.items()if k!='gpu_ctgp'}
        self.apply(config)
        self.assertTrue(set(helper.GPU_OWN_MASKS).isdisjoint(self.record['before']))
        self.restore()
        self.assertFalse(any(a in helper.GPU_OWN_MASKS for a,_ in self.ec.writes))

    def test_cpu_only_rejects_dormant_external_gpu_before_enabling_shared_appe(self):
        config={k:v for k,v in self.config.items()if k!='gpu_ctgp'}
        self.ec.values[0x744]=35
        with self.assertRaises(RuntimeError):self.apply(config)
        self.assertIsNone(self.record);self.assertEqual(self.ec.writes,[])

    def test_cpu_only_does_not_disable_shared_appe_after_foreign_gpu_appears(self):
        config={k:v for k,v in self.config.items()if k!='gpu_ctgp'}
        self.apply(config);self.ec.values.update({0x743:5,0x744:35});self.ec.writes.clear()
        with self.assertRaises(RuntimeError):self.restore()
        self.assertEqual(self.ec.values[0x741]&1,1)
        self.assertEqual(self.ec.writes,[])
        self.assertTrue(set(helper.GPU_OWN_MASKS).isdisjoint(self.record['before']))

    def test_invalid_gpu_range_or_type_never_undoes_existing_configuration(self):
        self.apply();state=deepcopy(self.record);self.ec.writes.clear()
        for value in (64,116,True,100.0,None):
            with self.assertRaises(ValueError):self.apply({**self.config,'gpu_ctgp':value})
        self.assertEqual(self.record,state);self.assertEqual(self.ec.writes,[])

    def test_dynamic_boost_fixed_tpp_and_write_order(self):
        self.apply({**self.config,'gpu_ctgp':65,'gpu_db':25})
        # Independent OEM/live-trial mapping:745=TPP sentinel,746=DB wattage.
        self.assertEqual([self.ec.values[a]for a in (0x743,0x744,0x745,0x746)],[7,0,255,25])
        addresses=[a for a,_ in self.ec.writes]
        self.assertLess(addresses.index(0x745),addresses.index(0x746))
        self.assertLess(addresses.index(0x746),addresses.index(0x743))
        self.restore();self.assertEqual(self.ec.values,self.before)

    def test_boost_rejects_unsupported_values_and_requires_explicit_ctgp(self):
        for value in (-1,1,4,26,True,5.0,None):
            with self.assertRaises(ValueError):self.apply({**self.config,'gpu_db':value})
        config={k:v for k,v in self.config.items()if k!='gpu_ctgp'}
        with self.assertRaises(ValueError):self.apply({**config,'gpu_db':0})
        with self.assertRaises(ValueError):self.apply({**self.config,'gpu_tpp':255})
        self.assertEqual(self.ec.writes,[])

    def test_missing_boost_defaults_off_and_explicit_zero_matches(self):
        self.apply();first={a:self.ec.values[a]for a in helper.GPU_OWN_MASKS};self.restore()
        self.apply({**self.config,'gpu_db':0})
        self.assertEqual(first,{a:self.ec.values[a]for a in helper.GPU_OWN_MASKS})

    def test_failed_firmware_gate_preserves_existing_state(self):
        self.apply();state=deepcopy(self.record);self.ec.writes.clear()
        with patch.object(helper,'check_gpu_capability',side_effect=RuntimeError('unsupported UEFI')):
            with self.assertRaisesRegex(RuntimeError,'UEFI'):self.apply()
            result=helper.status(self.ec)
        self.assertTrue(result['tuning_active']);self.assertFalse(result['gpu_tuning_supported'])
        self.assertIn('UEFI',result['gpu_tuning_reason'])
        self.assertEqual(self.record,state);self.assertEqual(self.ec.writes,[])

    def test_failed_powerd_or_acpi_gate_does_not_undo_existing_tuning(self):
        self.apply();state=deepcopy(self.record);self.ec.writes.clear()
        with patch.object(helper,'check_gpu_runtime',side_effect=RuntimeError('unhealthy runtime')):
            with self.assertRaisesRegex(RuntimeError,'runtime'):self.apply()
        self.assertEqual(self.record,state);self.assertEqual(self.ec.writes,[])

    def test_foreign_gpu_prevents_initial_acquire(self):
        self.ec.values[0x744]=20
        with self.assertRaises(RuntimeError):self.apply()
        self.assertIsNone(self.record);self.assertEqual(self.ec.writes,[])

    def test_old_gpu_baseline_is_not_accepted_as_current_ownership(self):
        self.apply();self.ec.values[0x744]=0;self.ec.writes.clear()
        with self.assertRaises(RuntimeError):self.restore()
        self.assertEqual(self.ec.writes,[])
        self.assertIn('gpu',self.record['relinquished'])

    def test_foreign_mode_kept_while_owned_gpu_cpu_fan_are_released(self):
        self.apply();self.ec.values.update({helper.MODE:0xA0,0x726:0,0x727:128});self.ec.writes.clear()
        self.restore()
        self.assertFalse(any(a in (helper.MODE,0x726,0x727)for a,_ in self.ec.writes))
        self.assertEqual(self.ec.values,{**self.before,helper.MODE:0xA0})

    def test_foreign_gpu_mid_restore_stops_before_appe_or_mode(self):
        self.apply();writer=self.ec.write_tuning;self.ec.writes.clear()
        def write(a,v):
            writer(a,v)
            if a==0x743:self.ec.values[0x744]=50
        self.ec.write_tuning=write
        with self.assertRaises(RuntimeError):self.restore()
        self.assertEqual(self.ec.writes,[(0x743,0)])
        self.assertEqual(self.ec.values[0x741]&1,1)

    def test_foreign_appe_or_cpu_or_fan_blocks_all_restore_writes(self):
        for address,value in ((0x741,128),(0x783,60),(0x7C6,0),(0xF03,1)):
            with self.subTest(address=address):
                self.record=None;self.ec=TuningEC();self.ec.values.update(self.before)
                self.apply();self.ec.values[address]=value;self.ec.writes.clear()
                with self.assertRaises(RuntimeError):self.restore()
                self.assertEqual(self.ec.writes,[])

    def test_mode_handoff_during_ram_restore_is_not_overwritten(self):
        self.apply();writer=self.ec.write_tuning;self.ec.writes.clear()
        def write(a,v):
            writer(a,v)
            if a==0xF03:self.ec.values.update({helper.MODE:0x20,0x726:0,0x727:128})
        self.ec.write_tuning=write;self.restore()
        self.assertEqual(self.ec.values,{**self.before,helper.MODE:0x20})

    def test_pending_save_failure_never_performs_unjournaled_gpu_write(self):
        self.fail_save=lambda s:s['pending']=={'address':0x744,'value':35}
        with self.assertRaises(OSError):self.apply()
        self.assertFalse(any(a==0x744 and v==35 for a,v in self.ec.writes))
        self.assertEqual(self.ec.values,self.before);self.assertIsNone(self.record)

    def test_unowned_logo_bits_changed_during_pending_save_are_preserved(self):
        def after(s):
            if s['pending']=={'address':0x7C5,'value':128}:
                self.ec.values[0x7C5]=(self.ec.values[0x7C5]&~7)|6
        self.after_save=after;self.apply()
        self.assertEqual(self.ec.values[0x7C5]&7,6)
        self.restore();self.assertEqual(self.ec.values[0x7C5]&7,6)

    def test_external_same_pending_target_is_not_mistaken_for_our_write(self):
        def after(s):
            if s['pending']=={'address':0x744,'value':35}:
                self.ec.values[0x744]=35;self.after_save=None
        self.after_save=after
        with self.assertRaisesRegex(RuntimeError,'恢复'):self.apply()
        self.assertFalse(any(a==0x744 for a,_ in self.ec.writes))
        self.assertIn('gpu',self.record['relinquished'])

    def test_mode_takeover_during_apply_pending_save_is_preserved(self):
        def after(s):
            if s['pending']=={'address':0x744,'value':35}:
                self.ec.values.update({helper.MODE:0xA0,0x726:0,0x727:128});self.after_save=None
        self.after_save=after
        with self.assertRaises(RuntimeError):self.apply()
        self.assertEqual(self.ec.values,{**self.before,helper.MODE:0xA0})
        self.assertIsNone(self.record)

    def test_appe_takeover_during_pending_save_blocks_gpu_write(self):
        def after(s):
            if s['pending']=={'address':0x744,'value':35}:
                self.ec.values[0x741]&=~1;self.after_save=None
        self.after_save=after
        with self.assertRaises(RuntimeError):self.apply()
        self.assertFalse(any(a==0x744 for a,_ in self.ec.writes))
        self.assertIn('appe',self.record['relinquished'])

    def test_mode_takeover_during_restore_pending_save_skips_mode_write(self):
        self.apply();self.ec.writes.clear()
        def after(s):
            if s['phase']=='restoring'and s['pending']=={'address':0x726,'value':0}:
                self.ec.values.update({helper.MODE:0x20,0x726:0,0x727:128});self.after_save=None
        self.after_save=after;self.restore()
        self.assertFalse(any(a in(helper.MODE,0x726,0x727)for a,_ in self.ec.writes))
        self.assertEqual(self.ec.values,{**self.before,helper.MODE:0x20})

    def test_confirmed_save_failure_recovers_persisted_pending_write(self):
        self.fail_save=lambda s:s['pending']is None and s['confirmed'][0x744]==35
        with self.assertRaises(OSError):self.apply()
        self.assertIn((0x744,35),self.ec.writes)
        self.assertEqual(self.ec.values,self.before);self.assertIsNone(self.record)

    def test_journal_error_after_atomic_replacement_is_also_recoverable(self):
        for stage in ('pending','confirmed'):
            with self.subTest(stage=stage):
                self.record=None;self.ec=TuningEC();self.ec.values.update(self.before)
                def after(s):
                    hit=(s['pending']=={'address':0x744,'value':35} if stage=='pending'else
                         s['pending']is None and s['confirmed'][0x744]==35)
                    if hit:
                        self.after_save=None
                        raise OSError('directory fsync failed after replacement')
                self.after_save=after
                with self.assertRaises(OSError):self.apply()
                self.assertEqual(self.ec.values,self.before);self.assertIsNone(self.record)

    def test_restore_confirmed_save_failure_keeps_pending_for_retry(self):
        self.apply()
        self.fail_save=lambda s:s['phase']=='restoring'and s['pending']is None and s['confirmed'][0x743]==0
        with self.assertRaises(OSError):self.restore()
        self.assertEqual(self.record['pending'],{'address':0x743,'value':0})
        self.restore();self.assertEqual(self.ec.values,self.before);self.assertIsNone(self.record)

    def test_failed_final_active_commit_restores_all_confirmed_writes(self):
        self.fail_save=lambda s:s['phase']=='active'
        with self.assertRaises(OSError):self.apply()
        self.assertEqual(self.ec.values,self.before);self.assertIsNone(self.record)

    def test_gpu_readback_error_resolves_durable_pending(self):
        read=self.ec.read;once=[True]
        def fault(a):
            if a==0x744 and self.ec.values[a]==35 and once[0]:
                once[0]=False;raise OSError('readback failed')
            return read(a)
        self.ec.read=fault
        with self.assertRaises(OSError):self.apply()
        self.assertEqual(self.ec.values,self.before);self.assertIsNone(self.record)

    def test_restore_snapshot_read_failure_does_not_write_and_remains_retryable(self):
        self.apply();self.ec.writes.clear();read=self.ec.read;once=[True]
        def fault(a):
            if a==0xF03 and once[0]:once[0]=False;raise OSError('snapshot failed')
            return read(a)
        self.ec.read=fault
        with self.assertRaises(OSError):self.restore()
        self.assertEqual(self.ec.writes,[])
        self.restore();self.assertEqual(self.ec.values,self.before)

    def test_every_apply_write_failure_before_or_after_acceptance_rolls_back(self):
        self.config['gpu_db']=25
        self.apply();count=len(self.ec.writes);self.restore()
        for failure in range(1,count+1):
            for after in (False,True):
                with self.subTest(write=failure,after=after):
                    self.ec=TuningEC();self.ec.values.update(self.before);self.record=None
                    writer=self.ec.write_tuning;calls=[0]
                    def write(a,v):
                        calls[0]+=1;fail=calls[0]==failure
                        if fail and not after:raise OSError('before acceptance')
                        writer(a,v)
                        if fail:raise OSError('after acceptance')
                    self.ec.write_tuning=write
                    with self.assertRaises(OSError):self.apply()
                    self.assertEqual(self.ec.values,self.before)
                    self.assertIsNone(self.record)

    def test_every_restore_write_failure_is_retryable(self):
        self.config['gpu_db']=25
        self.apply();self.ec.writes.clear();self.restore();count=len(self.ec.writes)
        for failure in range(1,count+1):
            for after in (False,True):
                with self.subTest(write=failure,after=after):
                    self.ec=TuningEC();self.ec.values.update(self.before);self.record=None;self.apply()
                    writer=self.ec.write_tuning;calls=[0]
                    def write(a,v):
                        calls[0]+=1;fail=calls[0]==failure
                        if fail and not after:raise OSError('before acceptance')
                        writer(a,v)
                        if fail:raise OSError('after acceptance')
                    self.ec.write_tuning=write
                    with self.assertRaises(OSError):self.restore()
                    self.ec.write_tuning=writer;self.restore()
                    self.assertEqual(self.ec.values,self.before);self.assertIsNone(self.record)

    def legacy(self,phase):
        config={k:v for k,v in self.config.items()if k!='gpu_ctgp'}
        self.apply(config)
        self.record={k:deepcopy(self.record[k])for k in ('phase','before','target','config')}
        self.record.update(version=1,phase=phase)

    def test_v1_active_status_is_read_only_and_restore_never_writes_gpu(self):
        self.legacy('active');old=deepcopy(self.record);self.ec.writes.clear()
        self.assertTrue(helper.status(self.ec)['tuning_active'])
        self.assertEqual(self.record,old)
        self.restore();self.assertEqual(self.ec.values,self.before)
        self.assertFalse(any(a in helper.GPU_OWN_MASKS for a,_ in self.ec.writes))

    def test_v1_active_does_not_reclaim_external_old_baseline(self):
        self.legacy('active');self.ec.values[0x783]=0;self.ec.writes.clear()
        with self.assertRaisesRegex(RuntimeError,'旧恢复'):self.restore()
        self.assertEqual(self.record['version'],1);self.assertEqual(self.ec.writes,[])

    def test_v1_preparing_migrates_once_with_explicit_ambiguity(self):
        self.legacy('preparing');self.ec.values[0x783]=0;self.ec.writes.clear()
        self.restore()
        self.assertTrue(any(s['version']==2 and s['legacy_ambiguous']for s in self.records))
        self.assertEqual(self.ec.values,self.before)
        self.assertFalse(any(a in helper.GPU_OWN_MASKS for a,_ in self.ec.writes))


class GPUFormatTest(unittest.TestCase):
    def test_explicit_gpu_runtime_gate_checks_powerd_then_real_healthy_json(self):
        good=SimpleNamespace(returncode=0,stdout='{"healthy":true}')
        bad=SimpleNamespace(returncode=3,stdout='{"healthy":false}')
        for replies in ([bad],[good,bad],[good,SimpleNamespace(returncode=0,stdout='{"healthy":false}')]):
            with patch.object(helper.subprocess,'run',side_effect=replies)as run:
                with self.assertRaises(RuntimeError):helper.check_gpu_runtime()
                self.assertEqual(run.call_args_list[0].args[0],['/usr/bin/systemctl','is-active','--quiet','nvidia-powerd.service'])
        with patch.object(helper.subprocess,'run',side_effect=[good,good]):helper.check_gpu_runtime()

    def test_failed_profile_cleanup_is_required_before_sleep(self):
        unit=(Path(__file__).resolve().parents[1]/'system/mechrevo-profile-sleep.service').read_text()
        self.assertIn('Before=sleep.target',unit)
        self.assertIn('RequiredBy=sleep.target',unit)
        self.assertNotIn('WantedBy=sleep.target',unit)

    def test_uefi_payload_exact_version_and_length(self):
        raw=bytearray(184);raw[:4]=(7).to_bytes(4,'little');raw[0x47]=25
        helper.parse_gpu_uefi(raw)
        for invalid in (raw[:-1],raw+b'\0',bytes(184)):
            with self.assertRaises(RuntimeError):helper.parse_gpu_uefi(invalid)

    def test_v1_on_disk_compatible_without_gpu_addresses_and_invalid_gpu_v1_rejected(self):
        config={'pl1':35,'pl2':35,'cpu_fan':helper.MIN_DUTY.copy(),'gpu_fan':helper.MIN_DUTY.copy()}
        values={a:0 for a in helper.state_masks({'config':config})}
        data={'version':1,'phase':'active','config':config,'before':values,'target':values}
        with tempfile.TemporaryDirectory(dir='/dev/shm')as directory,patch.object(helper,'STATE',Path(directory)/'state'):
            helper.save_state(data)
            self.assertEqual(helper.read_state(),data)
            data['config']['gpu_ctgp']=100;helper.save_state(data)
            with self.assertRaises(RuntimeError):helper.read_state()
            self.assertTrue(helper.STATE.exists())

    def test_gpu_identity_gate_uses_actual_gfid_kernel_and_uefi(self):
        ec=TuningEC();ec.values[0x7D2]=25;raw=bytearray(184);raw[0x47]=25
        with patch.object(helper,'check_machine'),patch.object(helper.os,'uname',return_value=SimpleNamespace(release='6.8.0-149-generic')), \
                patch.object(helper.os,'open',return_value=123),patch.object(helper.os,'close'), \
                patch.object(helper.os,'fstat',return_value=SimpleNamespace(st_mode=0o100600,st_uid=0)), \
                patch.object(helper.os,'read',return_value=bytes(raw)) as read:
            helper.check_gpu_capability(ec)
            read.assert_called_once_with(123,185)
            ec.values[0x7D2]=24
            with self.assertRaisesRegex(RuntimeError,'GFID'):helper.check_gpu_capability(ec)
            ec.values[0x7D2]=25
            with patch.object(helper.os,'uname',return_value=SimpleNamespace(release='6.9.0')):
                with self.assertRaises(RuntimeError):helper.check_gpu_capability(ec)


if __name__=='__main__':unittest.main()

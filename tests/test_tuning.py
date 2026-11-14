import tempfile
from pathlib import Path
import unittest
from test_profile_helper import helper, FakeEC


class TuningEC(FakeEC):
    def __init__(self):
        super().__init__()
        self.values.update(dict.fromkeys(range(0xF00,0xF60),0))
    def write_tuning(self,a,v):
        self.writes.append((a,v)); self.values[a]=v


class TuningTest(unittest.TestCase):
    def setUp(self):
        self.temp=tempfile.TemporaryDirectory()
        self.old=helper.STATE; helper.STATE=Path(self.temp.name)/'state.json'
        self.config={'pl1':35,'pl2':35,'cpu_fan':helper.MIN_DUTY.copy(),'gpu_fan':helper.MIN_DUTY.copy()}
    def tearDown(self):
        helper.STATE=self.old;self.temp.cleanup()
    def test_enforces_power_and_cooling_envelope(self):
        for field,value in [('pl1',24),('pl2',76),('pl1',True),('pl1',44),('cpu_fan',[0]*11),('gpu_fan',[100]*10),('gpu_fan',[90,80]+[100]*9)]:
            config={**self.config,field:value};ec=TuningEC()
            with self.assertRaises(ValueError):helper.apply_tuning(ec,config,pause=lambda _:None)
            self.assertFalse(ec.writes);self.assertFalse(helper.STATE.exists())
    def test_success_restores_every_bit_and_ram_byte(self):
        ec=TuningEC();ec.values[0x7AB]=0xE1;ec.values[0x7C6]=0x80;ec.values[0x741]=0x80
        before=ec.values.copy()
        result=helper.apply_tuning(ec,self.config,pause=lambda _:None)
        self.assertTrue(result['tuning_active']);self.assertTrue(result['switchable'])
        self.assertEqual(ec.values[0x7C6],0x84)
        self.assertEqual(ec.values[0x7AB],0xE1)
        helper.restore_tuning(ec);self.assertEqual(ec.values,before);self.assertFalse(helper.STATE.exists())
    def test_failure_after_each_write_rolls_back(self):
        # Simulate a device accepting a byte then reporting an IO error, at every stage.
        for fail_at in range(1,102):
            with self.subTest(fail_at=fail_at):
                ec=TuningEC();before=ec.values.copy();writer=ec.write_tuning;count=[0]
                def inject(a,v):
                    writer(a,v);count[0]+=1
                    if count[0]==fail_at:raise OSError('injected')
                ec.write_tuning=inject
                with self.assertRaises(OSError):helper.apply_tuning(ec,self.config,pause=lambda _:None)
                self.assertEqual(ec.values,before);self.assertFalse(helper.STATE.exists())
    def test_mode_key_wins_but_removes_owned_overrides(self):
        ec=TuningEC();before=ec.values.copy();helper.apply_tuning(ec,self.config,pause=lambda _:None)
        ec.values[0x751]=0xA0;ec.values[0x726]&=~128;ec.values[0x727]&=~64
        helper.restore_tuning(ec)
        self.assertEqual(ec.values,{**before,0x751:0xA0})
    def test_foreign_tuning_is_not_overwritten(self):
        ec=TuningEC();helper.apply_tuning(ec,self.config,pause=lambda _:None)
        ec.values[0x783]=60;ec.writes.clear()
        with self.assertRaises(RuntimeError):helper.restore_tuning(ec)
        self.assertFalse(ec.writes);self.assertTrue(helper.STATE.exists())
    def test_recovery_is_idempotent_after_interrupted_restore(self):
        ec=TuningEC();before=ec.values.copy();helper.apply_tuning(ec,self.config,pause=lambda _:None)
        writer=ec.write_tuning;count=[0]
        def inject(a,v):
            writer(a,v);count[0]+=1
            if count[0]==50:raise OSError('interrupted restore')
        ec.write_tuning=inject
        with self.assertRaises(OSError):helper.restore_tuning(ec)
        ec.write_tuning=writer;helper.restore_tuning(ec)
        self.assertEqual(ec.values,before)
    def test_pending_journal_recovers_after_helper_crash(self):
        ec=TuningEC();before=ec.values.copy();helper.apply_tuning(ec,self.config,pause=lambda _:None)
        state=helper.read_state();state['phase']='preparing';helper.save_state(state)
        helper.restore_tuning(ec);self.assertEqual(ec.values,before)

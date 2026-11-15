import sys
from pathlib import Path
import unittest
from unittest.mock import patch
source=Path(__file__).resolve().parents[1]/'iteglow/src'
if not source.exists(): source=Path(__file__).resolve().parents[2]/'iteglow-0.3.2/src'
sys.path.insert(0,str(source))
from iteglow import effects, gui, protocol
from gi.repository import Gtk
Gtk.init()

class FakeDevice:
    def __init__(self,*_):self.sent=[];self.closed=False
    def query(self,_):return bytes([0x88,2,2,5,30,8,0,0])
    def send(self,data):self.sent.append(data)
    def close(self):self.closed=True

class LightingTest(unittest.TestCase):
    def test_trigger_byte_independent_of_saved_direction(self):
        for name in ['reactive','ripple','aurora','spark']:
            for direction in [1,2,3,4]:
                self.assertEqual(effects.build('keyboard',name,direction=direction)[-1][1][6],0)
                self.assertEqual(effects.build('keyboard',name,direction=direction,reactive=True)[-1][1][6],1)
        for direction in [1,2,3,4]:
            self.assertEqual(effects.build('keyboard','wave',direction=direction)[-1][1][6],direction)
    def test_old_saved_params_migrate_to_auto(self):
        from iteglow.cli import _build_from_params
        report=_build_from_params('keyboard',{'mode':'spark','direction':4,'brightness':30})[-1][1]
        self.assertEqual(report[6],0)
    @patch.object(gui.devices,'HidLed',FakeDevice)
    @patch.object(gui.state,'load',return_value={})
    @patch.object(gui.state,'save')
    def test_ui_construction_does_not_apply_and_controls_save(self,save,_load):
        panel=gui.DevicePanel('keyboard')
        try:
            self.assertEqual(panel.dev.sent,[])
            panel.mode='aurora';panel.direction.set_selected(3)
            panel.trigger.set_selected(0);panel.bright.set_value(80);panel.speed.set_value(9)
            panel.apply()
            packet=panel.dev.sent[-1][-1][1]
            self.assertEqual((packet[2],packet[3],packet[4],packet[6]),(14,2,40,0))
            self.assertEqual(save.call_args[0][1]['reactive'],False)
            panel.trigger.set_selected(1);panel.apply()
            self.assertEqual(panel.dev.sent[-1][-1][1][6],1)
            panel.bright.set_value(0);panel.choose('spark')
            self.assertGreater(panel.bright.get_value(),0)
            self.assertEqual(panel.effect_buttons['spark'].get_label(),'烟花')
        finally:panel.close()
    @patch.object(gui.devices,'HidLed',FakeDevice)
    @patch.object(gui.state,'load',return_value={})
    def test_lightbar_constructs_without_writes(self,_load):
        panel=gui.DevicePanel('lightbar')
        try:
            self.assertEqual(panel.dev.sent,[])
            self.assertEqual(panel.effect_buttons['runner'].get_label(),'追光')
        finally:panel.close()

class LogoGuiTest(unittest.TestCase):
    @patch('iteglow.logo.Gio.Subprocess.new')
    def test_construction_and_editing_never_write(self,process):
        from iteglow.logo import LogoPanel
        panel=LogoPanel()
        try:
            panel.mode.set_selected(1);panel.palette.set_selected(7)
            self.assertEqual(panel.config()['color'],[255,255,255])
            panel.mode.set_selected(0);panel.palette.set_selected(7)
            for spin,value in zip(panel.rgb,[17,43,89]):spin.set_value(value)
            panel.brightness.set_selected(3)
            self.assertEqual(panel.config(),{'mode':'solid','brightness':75,'color':[17,43,89]})
            process.assert_not_called()
        finally:panel.close()

if __name__=='__main__':unittest.main()

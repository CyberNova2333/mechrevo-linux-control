import importlib.machinery,importlib.util,struct,unittest
from pathlib import Path
import tempfile
from unittest.mock import patch
loader=importlib.machinery.SourceFileLoader('acpi_check',str(Path(__file__).resolve().parents[1]/'system/mechrevo-acpi-check'))
spec=importlib.util.spec_from_loader(loader.name,loader);check=importlib.util.module_from_spec(spec);loader.exec_module(check)
class AcpiTest(unittest.TestCase):
    def table(self,signature=b'SSDT',length=44):
        b=bytearray(length);b[:4]=signature;struct.pack_into('<I',b,4,length);b[8]=2;b[9]=(-sum(b))%256;return bytes(b)
    def test_valid_table(self):self.assertTrue(check.inspect('SSDT26',self.table())['valid'])
    def test_different_valid_table_is_rejected(self):
        r=check.inspect('SSDT26',self.table(b'TPM2',76));self.assertEqual(r['errors'],['signature-mismatch'])
    def test_truncated_or_shifted_table_is_rejected(self):
        for b in (b'',b'NPCF',self.table()[:-1],self.table()[12:]):self.assertFalse(check.inspect('SSDT26',b)['valid'])
    def test_body_corruption_is_rejected(self):
        b=bytearray(self.table());b[-1]=1;self.assertIn('checksum-mismatch',check.inspect('SSDT26',b)['errors'])
    def test_capture_retention_and_latest_report(self):
        with tempfile.TemporaryDirectory() as root:
            tables=Path(root)/'tables';tables.mkdir()
            dest=Path(root)/'captures';dest.mkdir()
            (tables/'DSDT').write_bytes(self.table(b'DSDT'))
            b=bytearray(self.table(length=52));b[36:44]=b'NVDA0820';b[9]=0;b[9]=(-sum(b))%256
            (tables/'SSDT26').write_bytes(b)
            unrelated=dest/'unrelated';unrelated.mkdir()
            with patch.object(check,'TABLES',tables),patch.object(check,'DEST',dest):
                for _ in range(10):self.assertTrue(check.collect(True)['healthy'])
            self.assertEqual(len(list(dest.glob('*-*/report.json'))),8)
            self.assertTrue(unrelated.is_dir())
            self.assertTrue((dest/'latest.json').exists())

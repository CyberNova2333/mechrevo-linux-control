// SPDX-License-Identifier: GPL-2.0
/* XxAF55xx / N.1.33MRO91 only. Firmware submode, no clock/voltage tuning.
 * OEM 5.56.60.46: 0x728 bit 0 followed by EC0.PMSF().
 * Loading/unloading this bridge never changes hardware state.
 */
#include <linux/module.h>
#include <linux/acpi.h>
#include <linux/dmi.h>
#include <linux/kobject.h>
#include <linux/mutex.h>
#include <linux/delay.h>

static DEFINE_MUTEX(control_lock);
static struct kobject *control_kobj;
static acpi_handle reader, writer, policy;
static int read_ec(unsigned int reg, u8 *value)
{
 union acpi_object arg = { .type = ACPI_TYPE_INTEGER };
 struct acpi_object_list args = { .count = 1, .pointer = &arg };
 unsigned long long result;
 arg.integer.value = reg;
 if (ACPI_FAILURE(acpi_evaluate_integer(reader, NULL, &args, &result)) || result > 255)
  return -EIO;
 *value = result;
 return 0;
}
static int write_ec(unsigned int reg, u8 value)
{
 union acpi_object a[2] = {{ .type = ACPI_TYPE_INTEGER }, { .type = ACPI_TYPE_INTEGER }};
 struct acpi_object_list args = { .count = 2, .pointer = a };
 a[0].integer.value = reg; a[1].integer.value = value;
 return ACPI_FAILURE(acpi_evaluate_object(writer, NULL, &args, NULL)) ? -EIO : 0;
}
static int refresh_policy(void)
{
 return ACPI_FAILURE(acpi_evaluate_object(policy, NULL, NULL, NULL)) ? -EIO : 0;
}
static const unsigned int regs[] = {0x706,0x726,0x727,0x728,0x741,0x751,0x783,0x784,0x785,0x786,0x787,0x7ab,0x7c5,0x7c7,0x7e8};
static ssize_t status_show(struct kobject *k, struct kobj_attribute *a, char *buf)
{
 int i, ret = 0; ssize_t n = 0; u8 v;
 mutex_lock(&control_lock);
 for (i = 0; i < ARRAY_SIZE(regs); i++) {
  ret = read_ec(regs[i], &v); if (ret) break;
  n += sysfs_emit_at(buf, n, "%03x=%02x\n", regs[i], v);
 }
 mutex_unlock(&control_lock);
 return ret ? ret : n;
}
static int standard_turbo(void)
{
 u8 v; int ret, i;
 const unsigned int r[] = {0x706,0x726,0x727,0x751,0x783,0x784,0x785,0x7c5};
 const u8 mask[] = {0xff,0x80,0x40,0xff,0xff,0xff,0xff,0x80};
 const u8 expected[] = {0x40,0,0,0x30,0,0,0,0};
 for(i = 0; i < ARRAY_SIZE(r); i++) {
  ret = read_ec(r[i], &v); if(ret) return ret;
  if ((v & mask[i]) != expected[i]) return -EBUSY;
 }
 return 0;
}
static ssize_t variant_store(struct kobject *k, struct kobj_attribute *a, const char *buf, size_t count)
{
 bool quiet; u8 before, target, after; int ret, rollback;
 if (sysfs_streq(buf,"quiet")) quiet = true;
 else if (sysfs_streq(buf,"extreme")) quiet = false;
 else return -EINVAL;
 mutex_lock(&control_lock);
 ret = standard_turbo(); if (ret) goto out;
 ret = read_ec(0x728, &before); if (ret) goto out;
 target = (before & ~1) | quiet;
 ret = write_ec(0x728, target); if (ret) goto out;
 ret = refresh_policy();
 msleep(150);
 if (!ret) ret = read_ec(0x728, &after);
 if (!ret && after != target) ret = -EIO;
 if (!ret) ret = standard_turbo();
 if (ret) {
  /* Restore only our bit, preserving unrelated changes. */
  if (!read_ec(0x728, &after) && (after & 1) == (target & 1)) {
   rollback = write_ec(0x728, (after & ~1) | (before & 1));
   if (!rollback) rollback = refresh_policy();
   if (rollback) pr_err("mechrevo_turbo: rollback failed: %d\n", rollback);
  }
 }
 out: mutex_unlock(&control_lock);
 return ret ? ret : count;
}
static struct kobj_attribute status_attr = __ATTR_RO(status);
static struct kobj_attribute variant_attr = __ATTR(variant,0200,NULL,variant_store);
static struct attribute *attrs[] = { &status_attr.attr, &variant_attr.attr, NULL };
static const struct attribute_group group = { .attrs = attrs };
static int __init bridge_init(void)
{
 int ret;
 if (!dmi_match(DMI_SYS_VENDOR,"MECHREVO") || !dmi_match(DMI_BOARD_NAME,"YAOSHI Series-XxAF55xx") || !dmi_match(DMI_BIOS_VERSION,"N.1.33MRO91")) return -ENODEV;
 if (ACPI_FAILURE(acpi_get_handle(NULL,"\\_SB.INOU.ECRR",&reader)) || ACPI_FAILURE(acpi_get_handle(NULL,"\\_SB.INOU.ECRW",&writer)) || ACPI_FAILURE(acpi_get_handle(NULL,"\\_SB.PC00.LPCB.EC0.PMSF",&policy))) return -ENODEV;
 control_kobj = kobject_create_and_add("mechrevo_turbo",kernel_kobj); if (!control_kobj) return -ENOMEM;
 ret = sysfs_create_group(control_kobj,&group); if(ret) kobject_put(control_kobj);
 return ret;
}
static void __exit bridge_exit(void) { kobject_put(control_kobj); }
module_init(bridge_init); module_exit(bridge_exit);
MODULE_LICENSE("GPL");
MODULE_DESCRIPTION("Exact-model Mechrevo firmware turbo-submode bridge");

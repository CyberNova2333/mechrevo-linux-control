// SPDX-License-Identifier: GPL-2.0
/* One-shot exact-model read of SMRW(0xbb, 0/4/8). No writes or live interface. */
#include <linux/acpi.h>
#include <linux/dmi.h>
#include <linux/module.h>
#include <linux/slab.h>
#include <linux/utsname.h>
#include <generated/utsrelease.h>

static char table[37];
module_param_string(table, table, sizeof(table), 0444);
static unsigned int gfid, adapter;
module_param(gfid, uint, 0444);
module_param(adapter, uint, 0444);

static acpi_status resource_check(struct acpi_resource *resource, void *context)
{
	bool *found = context;
	if (resource->type == ACPI_RESOURCE_TYPE_FIXED_MEMORY32 &&
	    resource->data.fixed_memory32.address == 0xfe410000 &&
	    resource->data.fixed_memory32.address_length == 0x1000)
		*found = true;
	return AE_OK;
}

static int __init table_init(void)
{
	acpi_handle inou, method;
	unsigned long long value;
	bool found = false;
	u8 bytes[12];
	int offset, i, length = 0;
	if (strcmp(UTS_RELEASE, "6.8.0-149-generic") ||
	    strcmp(init_uts_ns.name.release, "6.8.0-149-generic") ||
	    !dmi_match(DMI_SYS_VENDOR, "MECHREVO") ||
	    !dmi_match(DMI_BOARD_NAME, "YAOSHI Series-XxAF55xx") ||
	    !dmi_match(DMI_BIOS_VERSION, "N.1.33MRO91"))
		return -ENODEV;
	if (ACPI_FAILURE(acpi_get_handle(NULL, "\\_SB.INOU", &inou)) ||
	    ACPI_FAILURE(acpi_walk_resources(inou, METHOD_NAME__CRS, resource_check, &found)) ||
	    !found || ACPI_FAILURE(acpi_get_handle(inou, "SMRW", &method)))
		return -ENODEV;
	if (ACPI_FAILURE(acpi_evaluate_integer(NULL, "\\_SB.PC00.LPCB.EC0.GFID", NULL, &value)) || value != 0x19)
		return -ENODEV;
	gfid = value;
	if (ACPI_FAILURE(acpi_evaluate_integer(NULL, "\\_SB.PC00.LPCB.EC0.ADPT", NULL, &value)) || value > 15)
		return -EPROTO;
	adapter = value;
	for (offset = 0; offset <= 8; offset += 4) {
		u8 input[13] = { 0xbb };
		union acpi_object arg = { .type = ACPI_TYPE_BUFFER };
		struct acpi_object_list args = { .count = 1, .pointer = &arg };
		struct acpi_buffer output = { ACPI_ALLOCATE_BUFFER, NULL };
		union acpi_object *object;
		acpi_status status;
		input[1] = offset;
		arg.buffer.length = sizeof(input);
		arg.buffer.pointer = input;
		status = acpi_evaluate_object(method, NULL, &args, &output);
		object = output.pointer;
		if (ACPI_FAILURE(status) || !object || object->type != ACPI_TYPE_INTEGER ||
		    object->integer.value > 0xffffffffULL) {
			kfree(output.pointer);
			return -EPROTO;
		}
		for (i = 0; i < 4; i++)
			bytes[offset + i] = object->integer.value >> (8 * i);
		kfree(output.pointer);
	}
	for (i = 0; i < 12; i++)
		length += scnprintf(table + length, sizeof(table) - length,
				    i == 11 ? "%02x" : "%02x ", bytes[i]);
	pr_info("mechrevo_gpu_table_probe: GFID=%u ADPT=%u SMRW=%s; read only\n", gfid, adapter, table);
	return 0;
}
static void __exit table_exit(void) { }
module_init(table_init);
module_exit(table_exit);
MODULE_LICENSE("GPL");
MODULE_DESCRIPTION("Fixed read-only SMRW GPU table probe for qualified XxAF55xx");

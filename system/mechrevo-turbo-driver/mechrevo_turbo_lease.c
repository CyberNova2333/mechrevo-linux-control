// SPDX-License-Identifier: GPL-2.0
/* Exact-machine candidate, ABI 1. Building does not install or load this module.
 * No arbitrary EC/ACPI access is exposed. Firmware acknowledgements are returned
 * by the OEM WMI method, never synthesized from a successful ACPI evaluation.
 */
#include <linux/acpi.h>
#include <linux/capability.h>
#include <linux/dmi.h>
#include <linux/fs.h>
#include <linux/jiffies.h>
#include <linux/miscdevice.h>
#include <linux/module.h>
#include <linux/mutex.h>
#include <linux/pm.h>
#include <linux/slab.h>
#include <linux/suspend.h>
#include <linux/uaccess.h>
#include <linux/user_namespace.h>
#include <linux/utsname.h>
#include <linux/version.h>
#include <linux/workqueue.h>
#include <generated/utsrelease.h>

#define OEM_GUID "ABBC0F6F-8EA1-11D1-00A0-C90629100000"
#define EXPECTED_RELEASE "6.8.0-149-generic"
#define LEASE_MS 8000
#define GUARD_MS 250
#define RECOVERY_MS 1000
#define MAX_RECOVERY_ATTEMPTS 8
#define STATUS_SIZE 4096
#define REG_APPE 0x741
#define REG_TB43 0x728
#define REG_APTC 0x786
#define REG_MODE 0x751

static DEFINE_MUTEX(control_lock);
static acpi_handle ec_reader, ec_writer, policy, inou;
static bool shutting_down, pm_blocked;
static struct file *lease_owner;
static unsigned long lease_deadline;
static u64 sequence;
static int last_rc, restore_rc;
static const char *stop_reason = "idle";
static bool active, recovery_needed;
static bool quiet;
static bool recovery_pin, restoring, policy_uncertain;
static unsigned int recovery_attempts;
static unsigned long recovery_next;

struct receipt {
	acpi_status status;
	u32 type;
	bool present;
	u64 value;
};

static struct receipt command_reply;
static struct receipt ec_read_reply, ec_write_reply, pmsf_reply;
static struct receipt ctaf_reply, gamm_reply;
static struct receipt combined_support_reply, game_support_reply;
static bool combined_supported, game_supported;

static const u16 registers[] = {
	0x490, 0x706, 0x726, 0x727, 0x728, 0x72b, 0x741, 0x743, 0x751,
	0x783, 0x784, 0x785, 0x786, 0x787, 0x7ab, 0x7c5, 0x7c6,
	0x7c7, 0x7cc, 0x7e8, 0x7f7,
};

struct snapshot {
	u8 value[ARRAY_SIZE(registers)];
	u64 cstm, gatm;
};

struct owned_byte {
	u16 reg;
	u8 mask, before, target;
	bool attempted, restore_attempted;
};

/* APTC/APTN is a firmware-produced full byte. We restore it only while still
 * in the same ordinary turbo profile, never after a mode/custom takeover. */
static struct owned_byte owned[] = {
	{ .reg = REG_APPE, .mask = 1 },
	{ .reg = REG_TB43, .mask = 1 },
	{ .reg = REG_APTC, .mask = 0xff },
};
static u64 before_cstm, before_gatm, target_cstm, target_gatm;
static bool cstm_attempted, gatm_attempted;
static bool cstm_restoring, gatm_restoring;
static u32 restore_skipped;

struct client {
	bool text;
	size_t length;
	char status[STATUS_SIZE];
};

static bool privileged(void)
{
	return uid_eq(current_euid(), GLOBAL_ROOT_UID) &&
		ns_capable(&init_user_ns, CAP_SYS_RAWIO);
}

static int evaluate(acpi_handle handle, struct acpi_object_list *args,
		    struct receipt *reply)
{
	struct acpi_buffer output = { ACPI_ALLOCATE_BUFFER, NULL };
	union acpi_object *object;

	memset(reply, 0, sizeof(*reply));
	reply->status = acpi_evaluate_object(handle, NULL, args, &output);
	object = output.pointer;
	if (ACPI_SUCCESS(reply->status) && object) {
		reply->present = true;
		reply->type = object->type;
		if (object->type == ACPI_TYPE_INTEGER)
			reply->value = object->integer.value;
	}
	kfree(output.pointer);
	return ACPI_FAILURE(reply->status) ? -EIO : 0;
}

static int read_name(const char *path, u64 *value)
{
	unsigned long long result;
	acpi_status status = acpi_evaluate_integer(NULL, (char *)path, NULL, &result);

	if (ACPI_FAILURE(status) || result > 1)
		return -EIO;
	*value = result;
	return 0;
}

static int read_ec(u16 address, u8 *value)
{
	union acpi_object arg = { .type = ACPI_TYPE_INTEGER };
	struct acpi_object_list args = { .count = 1, .pointer = &arg };
	int ret;

	arg.integer.value = address;
	ret = evaluate(ec_reader, &args, &ec_read_reply);
	if (ret)
		return ret;
	if (!ec_read_reply.present || ec_read_reply.type != ACPI_TYPE_INTEGER ||
	    ec_read_reply.value > 255)
		return -EPROTO;
	*value = ec_read_reply.value;
	return 0;
}

static int write_ec(u16 address, u8 value)
{
	union acpi_object arg[2] = {
		{ .type = ACPI_TYPE_INTEGER }, { .type = ACPI_TYPE_INTEGER }
	};
	struct acpi_object_list args = { .count = 2, .pointer = arg };

	arg[0].integer.value = address;
	arg[1].integer.value = value;
	return evaluate(ec_writer, &args, &ec_write_reply);
}

static int read_snapshot(struct snapshot *s)
{
	int i, ret;

	for (i = 0; i < ARRAY_SIZE(registers); i++) {
		ret = read_ec(registers[i], &s->value[i]);
		if (ret)
			return ret;
	}
	ret = read_name("\\_SB.INOU.CSTM", &s->cstm);
	if (!ret)
		ret = read_name("\\_SB.INOU.GATM", &s->gatm);
	return ret;
}

static u8 value(const struct snapshot *s, u16 reg)
{
	int i;

	for (i = 0; i < ARRAY_SIZE(registers); i++)
		if (registers[i] == reg)
			return s->value[i];
	return 0;
}

static bool custom_state(const struct snapshot *s)
{
	return (value(s, 0x726) & 0x80) || (value(s, 0x727) & 0x40) ||
		(value(s, 0x7c5) & 0x80) || (value(s, 0x7c6) & 4) ||
		(value(s, 0x743) & 7) || value(s, 0x783) || value(s, 0x784) ||
		value(s, 0x785) || value(s, 0x787);
}

static bool standard_turbo(const struct snapshot *s)
{
	return value(s, 0x706) == 0x40 && !custom_state(s) &&
		(value(s, REG_MODE) & 0xdf) == 0x10;
}

static bool round_adapter(const struct snapshot *s)
{
	u8 socket = value(s, 0x7cc);

	/* OEM MyEcCtrl.GetRoundSocketStatusFromEC, not the generic AC-present bit. */
	if (socket & 1)
		return true;
	return !(socket & 0x0f) && !!(value(s, 0x490) & 1);
}

static int remember_ap_tc(void)
{
	struct snapshot now;
	u8 observed;
	int ret;

	if (restoring && !owned[2].attempted)
		return 0; /* Never reclaim a byte relinquished before rollback. */
	/* Sample our firmware-generated byte immediately after the call, before the
	 * longer eligibility snapshot. One transient read failure gets one retry. */
	ret = read_ec(REG_APTC, &observed);
	if (ret)
		ret = read_ec(REG_APTC, &observed);
	if (ret) {
		policy_uncertain = true;
		return ret;
	}
	owned[2].target = observed;
	owned[2].attempted = true;
	ret = read_snapshot(&now);
	if (ret)
		return ret;
	if (!standard_turbo(&now))
		return -ESTALE;
	return 0;
}

static int refresh_policy(bool capture)
{
	int ret = evaluate(policy, NULL, &pmsf_reply);
	int read_ret = capture ? remember_ap_tc() : 0;

	return ret ? ret : read_ret;
}

static int wmi_call(u8 code, struct receipt *reply)
{
	u64 payload = 0x50000000000ULL | code;
	__le64 input = cpu_to_le64(payload);
	struct acpi_buffer in = { sizeof(input), &input };
	struct acpi_buffer out = { ACPI_ALLOCATE_BUFFER, NULL };
	union acpi_object *object;

	memset(reply, 0, sizeof(*reply));
	reply->status = wmi_evaluate_method(OEM_GUID, 0, 4, &in, &out);
	object = out.pointer;
	if (ACPI_SUCCESS(reply->status) && object) {
		reply->present = true;
		reply->type = object->type;
		if (object->type == ACPI_TYPE_INTEGER)
			reply->value = object->integer.value;
	}
	kfree(out.pointer);
	if (ACPI_FAILURE(reply->status))
		return -EIO;
	if (!reply->present || reply->type != ACPI_TYPE_INTEGER)
		return -EPROTO;
	return 0;
}

static int probe_support(void)
{
	int ret;

	combined_supported = game_supported = false;
	ret = wmi_call(0x03, &combined_support_reply);
	if (ret)
		return ret;
	ret = wmi_call(0x13, &game_support_reply);
	if (ret)
		return ret;
	if ((combined_support_reply.value != 0x55 && combined_support_reply.value != 0xaa) ||
	    (game_support_reply.value != 0x55 && game_support_reply.value != 0xaa))
		return -EPROTO;
	combined_supported = combined_support_reply.value == 0x55;
	game_supported = game_support_reply.value == 0x55;
	return 0;
}

static int wmi_workload(bool combined, bool enabled, bool primary, bool capture)
{
	/* _WDG BC, instance 0, method ID 4: WMBC -> OEMG -> CTAF/GAMM.
	 * SAC1 = 0x500, SA00 = 01/02 (combined) or 14/15 (game).
	 */
	struct receipt *reply = combined ? &ctaf_reply : &gamm_reply;
	u64 observed;
	int ret, read_ret;

	ret = wmi_call(combined ? (enabled ? 2 : 1) : (enabled ? 0x14 : 0x15), reply);
	if (primary)
		command_reply = *reply;
	if (!ret && reply->value != (enabled ? 0xbb : 0xaa))
		ret = -EPROTO;
	read_ret = read_name(combined ? "\\_SB.INOU.CSTM" : "\\_SB.INOU.GATM", &observed);
	if (!ret && (read_ret || observed != enabled))
		ret = read_ret ? read_ret : -EIO;
	/* These methods call PMSF themselves, including on a duplicate request. */
	read_ret = capture ? remember_ap_tc() : 0;
	return ret ? ret : read_ret;
}

static int set_owned(unsigned int index, u8 bits)
{
	struct owned_byte *b = &owned[index];
	u8 now, after;
	int ret;

	ret = read_ec(b->reg, &now);
	if (ret)
		return ret;
	if (b->attempted && (now & b->mask) != b->target)
		return -ESTALE;
	b->target = bits & b->mask;
	b->attempted = true; /* Include a write accepted before an ACPI error. */
	ret = write_ec(b->reg, (now & ~b->mask) | b->target);
	if (!ret)
		ret = read_ec(b->reg, &after);
	if (!ret && (after & b->mask) != b->target)
		ret = -EIO;
	return ret;
}

static int check_active(const struct snapshot *s)
{
	int i;

	if (!round_adapter(s))
		return -ENOLINK;
	if (!standard_turbo(s))
		return -EBUSY;
	for (i = 0; i < ARRAY_SIZE(owned); i++)
		if (owned[i].attempted && (value(s, owned[i].reg) & owned[i].mask) != owned[i].target)
			return -ESTALE;
	if (s->cstm != target_cstm || s->gatm != target_gatm)
		return -ESTALE;
	return 0;
}

static int restore_byte(unsigned int index, bool permit)
{
	struct owned_byte *b = &owned[index];
	u8 now, after;
	int ret;

	if (!b->attempted)
		return 0;
	if (!permit) {
		restore_skipped |= BIT(index);
		b->attempted = false;
		return 0;
	}
	ret = read_ec(b->reg, &now);
	if (ret)
		return ret;
	if (b->restore_attempted && (now & b->mask) == b->before) {
		b->target = b->before;
		b->attempted = false;
		return 0;
	}
	if ((now & b->mask) != b->target) {
		restore_skipped |= BIT(index);
		b->attempted = false;
		return 0;
	}
	b->restore_attempted = true;
	ret = write_ec(b->reg, (now & ~b->mask) | b->before);
	if (!ret)
		ret = read_ec(b->reg, &after);
	if (!ret && (after & b->mask) != b->before)
		ret = -EIO;
	if (!ret) {
		b->target = b->before;
		b->attempted = false;
	}
	return ret;
}

static int restore_name(bool combined, bool capture)
{
	bool *attempted = combined ? &cstm_attempted : &gatm_attempted;
	bool *in_restore = combined ? &cstm_restoring : &gatm_restoring;
	u64 *expected = combined ? &target_cstm : &target_gatm;
	u64 target = combined ? target_cstm : target_gatm;
	u64 before = combined ? before_cstm : before_gatm;
	u64 now;
	int ret;

	if (!*attempted)
		return 0;
	ret = read_name(combined ? "\\_SB.INOU.CSTM" : "\\_SB.INOU.GATM", &now);
	if (ret)
		return ret;
	if (*in_restore && now == before) {
		*expected = before;
		*attempted = false;
		return 0;
	}
	if (now != target) {
		restore_skipped |= combined ? BIT(3) : BIT(4);
		*attempted = false;
		return 0;
	}
	*in_restore = true;
	ret = wmi_workload(combined, before, false, capture);
	if (!ret) {
		*expected = before;
		*attempted = false;
	}
	return ret;
}

static void finish_recovery(void)
{
	int i;

	recovery_needed = cstm_attempted || gatm_attempted;
	for (i = 0; i < ARRAY_SIZE(owned); i++)
		recovery_needed |= owned[i].attempted;
	if (!recovery_needed && recovery_pin) {
		recovery_pin = false;
		module_put(THIS_MODULE);
	}
}

static void relinquish_policy(void)
{
	int i;

	for (i = 0; i < ARRAY_SIZE(owned); i++) {
		if (owned[i].attempted)
			restore_skipped |= BIT(i);
		owned[i].attempted = false;
	}
	if (cstm_attempted)
		restore_skipped |= BIT(3);
	if (gatm_attempted)
		restore_skipped |= BIT(4);
	cstm_attempted = gatm_attempted = false;
	finish_recovery();
}

static bool foreign_bits(const struct snapshot *s, bool failed_command)
{
	int i;
	u8 observed;

	for (i = 0; i < ARRAY_SIZE(owned); i++) {
		if (!owned[i].attempted)
			continue;
		/* A normal mode-key transition owns the new APTC; it is skipped later. */
		if (i == 2 && !standard_turbo(s))
			continue;
		observed = value(s, owned[i].reg) & owned[i].mask;
		if (observed != owned[i].target &&
		    (!(failed_command || owned[i].restore_attempted) || observed != owned[i].before))
			return true;
	}
	if (s->cstm != target_cstm && (!(failed_command || cstm_restoring) || s->cstm != before_cstm))
		return true;
	if (s->gatm != target_gatm && (!(failed_command || gatm_restoring) || s->gatm != before_gatm))
		return true;
	return false;
}

static int release_lease(const char *reason)
{
	struct snapshot s;
	bool custom = true, same_turbo = false;
	bool failed_command = !strcmp(reason, "acquire_failed") || !strcmp(reason, "command_failed");
	int ret, first = 0;

	active = false;
	stop_reason = reason;
	if (!recovery_needed)
		return 0;
	recovery_attempts++;
	recovery_next = jiffies + msecs_to_jiffies(RECOVERY_MS);
	ret = read_snapshot(&s);
	if (!ret) {
		custom = custom_state(&s);
		same_turbo = standard_turbo(&s);
	} else {
		/* A read fault is not evidence of external ownership. Retry later. */
		restore_rc = ret;
		return ret;
	}
	/* Freeze ownership before any method with side effects. Custom tuning shares
	 * APPE/APTC, so we relinquish the entire policy, not merely its mode byte.
	 * Otherwise a recovery PMSF could overwrite an external controller's state. */
	if (custom || value(&s, 0x706) != 0x40 || foreign_bits(&s, failed_command)) {
		/* An unreadable result from our own method is not proof of a foreign
		 * owner. Keep the module pinned instead of silently abandoning it. */
		if (policy_uncertain && !custom) {
			restore_rc = -EUCLEAN;
			return -EUCLEAN;
		}
		stop_reason = custom ? "custom_takeover" : "foreign_takeover";
		relinquish_policy();
		restore_rc = 0;
		return 0;
	}
	if (policy_uncertain) {
		restore_rc = -EUCLEAN;
		return -EUCLEAN;
	}
	if (!same_turbo && owned[2].attempted) {
		restore_skipped |= BIT(2);
		owned[2].attempted = false;
	}
	restoring = true;
	/* Restore TB43 first, while APPE still permits firmware policy refresh.
	 * Never write the mode byte, custom flags, power limits or fan tables. */
	ret = restore_byte(1, !custom);
	if (ret && !first)
		first = ret;
	ret = refresh_policy(same_turbo);
	if (ret && !first)
		first = ret;
	ret = restore_name(true, same_turbo);
	if (ret && !first)
		first = ret;
	ret = restore_name(false, same_turbo);
	if (ret && !first)
		first = ret;
	ret = restore_byte(0, !custom);
	if (ret && !first)
		first = ret;
	ret = refresh_policy(same_turbo);
	if (ret && !first)
		first = ret;
	ret = restore_byte(2, same_turbo);
	if (ret && !first)
		first = ret;
	restoring = false;
	finish_recovery();
	restore_rc = first;
	return first;
}

static int acquire_lease(struct file *file, bool want_quiet)
{
	struct snapshot s;
	int i, ret;

	if (active || recovery_needed || (lease_owner && lease_owner != file))
		return -EBUSY;
	ret = probe_support();
	if (ret)
		return ret;
	if (!combined_supported)
		return -EOPNOTSUPP;
	ret = read_snapshot(&s);
	if (ret)
		return ret;
	if (!round_adapter(&s))
		return -ENOLINK;
	if (!standard_turbo(&s) || (value(&s, REG_APPE) & 1) ||
	    value(&s, REG_APTC) || s.cstm || s.gatm)
		return -EBUSY;
	for (i = 0; i < ARRAY_SIZE(owned); i++) {
		owned[i].before = value(&s, owned[i].reg) & owned[i].mask;
		owned[i].target = owned[i].before;
		owned[i].attempted = false;
		owned[i].restore_attempted = false;
	}
	before_cstm = target_cstm = s.cstm;
	before_gatm = target_gatm = s.gatm;
	cstm_attempted = gatm_attempted = false;
	cstm_restoring = gatm_restoring = false;
	restore_skipped = 0;
	restore_rc = 0;
	lease_owner = file;
	recovery_needed = true;
	__module_get(THIS_MODULE);
	recovery_pin = true;
	recovery_attempts = 0;
	policy_uncertain = false;
	ret = set_owned(0, 1);
	if (!ret)
		ret = set_owned(1, want_quiet);
	if (!ret)
		ret = refresh_policy(true);
	if (!ret)
		ret = read_snapshot(&s);
	if (!ret)
		ret = check_active(&s);
	if (ret) {
		release_lease("acquire_failed");
		return ret;
	}
	quiet = want_quiet;
	active = true;
	stop_reason = "none";
	lease_deadline = jiffies + msecs_to_jiffies(LEASE_MS);
	return 0;
}

enum command {
	ACQUIRE_QUIET, ACQUIRE_EXTREME, VARIANT_QUIET, VARIANT_EXTREME,
	COMBINED_OFF, COMBINED_ON, GAME_OFF, GAME_ON, HEARTBEAT, RELEASE,
	STATUS_JSON, STATUS_TEXT,
};

static const char * const commands[] = {
	"acquire quiet", "acquire extreme", "variant quiet", "variant extreme",
	"combined 0", "combined 1", "game 0", "game 1", "heartbeat", "release",
	"status json", "status text",
};

static int run_command(struct file *file, enum command cmd)
{
	struct snapshot s;
	bool combined, enabled;
	int ret;

	if (cmd == RELEASE) {
		if (lease_owner && lease_owner != file)
			return -EPERM;
		recovery_attempts = 0; /* Explicit owner/root recovery starts a new retry window. */
		ret = release_lease("released");
		if (!recovery_needed)
			lease_owner = NULL;
		return ret;
	}
	if (shutting_down || pm_blocked)
		return -ESHUTDOWN;
	if (cmd == ACQUIRE_QUIET || cmd == ACQUIRE_EXTREME)
		return acquire_lease(file, cmd == ACQUIRE_QUIET);
	if (!active || lease_owner != file)
		return -EPERM;
	if ((cmd == GAME_ON || cmd == GAME_OFF) && !game_supported)
		return -EOPNOTSUPP;
	ret = read_snapshot(&s);
	if (!ret)
		ret = check_active(&s);
	if (ret) {
		release_lease(ret == -ENOLINK ? "adapter_lost" : "external_change");
		return ret;
	}
	if (cmd == HEARTBEAT) {
		lease_deadline = jiffies + msecs_to_jiffies(LEASE_MS);
		return 0;
	}
	if (cmd == VARIANT_QUIET || cmd == VARIANT_EXTREME) {
		enabled = cmd == VARIANT_QUIET;
		ret = set_owned(1, enabled);
		if (!ret)
			ret = refresh_policy(true);
		if (!ret)
			quiet = enabled;
	} else {
		combined = cmd == COMBINED_OFF || cmd == COMBINED_ON;
		enabled = cmd == COMBINED_ON || cmd == GAME_ON;
		if (combined) {
			target_cstm = enabled;
			cstm_attempted = true;
		} else {
			target_gatm = enabled;
			gatm_attempted = true;
		}
		ret = wmi_workload(combined, enabled, true, true);
	}
	if (!ret)
		ret = read_snapshot(&s);
	if (!ret)
		ret = check_active(&s);
	if (ret)
		release_lease("command_failed");
	return ret;
}

static ssize_t bridge_write(struct file *file, const char __user *buffer,
			    size_t count, loff_t *position)
{
	struct client *client = file->private_data;
	char command[40];
	size_t length = count;
	int i, ret = -EINVAL;

	if (!privileged() || !(file->f_mode & FMODE_WRITE))
		return -EPERM;
	if (!count || count >= sizeof(command))
		return -EINVAL;
	if (copy_from_user(command, buffer, count))
		return -EFAULT;
	if (memchr(command, '\0', count))
		return -EINVAL;
	if (command[length - 1] == '\n')
		length--;
	command[length] = '\0';
	for (i = 0; i < ARRAY_SIZE(commands); i++)
		if (!strcmp(command, commands[i]))
			break;
	if (i == ARRAY_SIZE(commands))
		return -EINVAL;
	mutex_lock(&control_lock);
	sequence++;
	memset(&command_reply, 0, sizeof(command_reply));
	if (i == STATUS_JSON || i == STATUS_TEXT) {
		client->text = i == STATUS_TEXT;
		ret = 0;
	} else {
		ret = run_command(file, i);
	}
	last_rc = ret;
	client->length = 0;
	*position = 0;
	mutex_unlock(&control_lock);
	return ret ? ret : count;
}

static size_t format_status(char *buf, bool text)
{
	struct snapshot s;
	int ret, i, n = 0;
	long remaining = active ? max_t(long, 0, (long)(lease_deadline - jiffies)) : 0;
	char ack[32];

	ret = read_snapshot(&s);
	if (command_reply.present && command_reply.type == ACPI_TYPE_INTEGER)
		scnprintf(ack, sizeof(ack), "%llu", command_reply.value);
	else
		strscpy(ack, "null", sizeof(ack));
	if (text) {
		n += scnprintf(buf + n, STATUS_SIZE - n,
			"abi=1\nactive=%u\nvariant=%s\nsequence=%llu\nlast_ack=%s\n"
			"return_present=%u\nlast_rc=%d\nacpi_status=%u\nstop_reason=%s\n"
			"lease_remaining_ms=%u\nrecovery_needed=%u\nrestore_rc=%d\nrestore_skipped=%u\nread_rc=%d\nrecovery_attempts=%u\nrecovery_pin=%u\n"
			"combined_supported=%u\ngame_supported=%u\ncombined_support_ack=%llu\ngame_support_ack=%llu\npolicy_uncertain=%u\n",
			active, quiet ? "quiet" : "extreme", sequence, ack,
			command_reply.present, last_rc, command_reply.status, stop_reason,
			jiffies_to_msecs(remaining), recovery_needed, restore_rc, restore_skipped, ret,
			recovery_attempts, recovery_pin, combined_supported, game_supported,
			combined_support_reply.value, game_support_reply.value, policy_uncertain);
		if (!ret) {
			n += scnprintf(buf + n, STATUS_SIZE - n,
				"cstm=%llu\ngatm=%llu\nstandard_turbo=%u\nround_adapter=%u\n",
				s.cstm, s.gatm, standard_turbo(&s), round_adapter(&s));
			for (i = 0; i < ARRAY_SIZE(registers); i++)
				n += scnprintf(buf + n, STATUS_SIZE - n, "%03x=%02x\n", registers[i], s.value[i]);
		}
		return n;
	}
	n += scnprintf(buf + n, STATUS_SIZE - n,
		"{\"abi\":1,\"active\":%s,\"variant\":\"%s\",\"sequence\":%llu,"
		"\"last_ack\":%s,\"return_present\":%s,\"return_type\":%u,\"last_rc\":%d,"
		"\"acpi_status\":%u,\"stop_reason\":\"%s\",\"lease_remaining_ms\":%u,"
		"\"recovery_needed\":%s,\"restore_rc\":%d,\"restore_skipped\":%u,\"read_rc\":%d,"
		"\"recovery_attempts\":%u,\"recovery_pin\":%s,"
		"\"combined_supported\":%s,\"game_supported\":%s,\"combined_support_ack\":%llu,\"game_support_ack\":%llu,\"policy_uncertain\":%s,",
		active ? "true" : "false", quiet ? "quiet" : "extreme", sequence,
		ack, command_reply.present ? "true" : "false", command_reply.type, last_rc,
		command_reply.status, stop_reason, jiffies_to_msecs(remaining),
		recovery_needed ? "true" : "false", restore_rc, restore_skipped, ret,
		recovery_attempts, recovery_pin ? "true" : "false",
		combined_supported ? "true" : "false", game_supported ? "true" : "false",
		combined_support_reply.value, game_support_reply.value, policy_uncertain ? "true" : "false");
	if (ret) {
		n += scnprintf(buf + n, STATUS_SIZE - n,
			"\"cstm\":null,\"gatm\":null,\"standard_turbo\":false,\"round_adapter\":false,\"regs\":{}}\n");
		return n;
	}
	n += scnprintf(buf + n, STATUS_SIZE - n,
		"\"cstm\":%llu,\"gatm\":%llu,\"standard_turbo\":%s,\"round_adapter\":%s,\"regs\":{",
		s.cstm, s.gatm, standard_turbo(&s) ? "true" : "false", round_adapter(&s) ? "true" : "false");
	for (i = 0; i < ARRAY_SIZE(registers); i++)
		n += scnprintf(buf + n, STATUS_SIZE - n, "%s\"%03x\":%u", i ? "," : "", registers[i], s.value[i]);
	n += scnprintf(buf + n, STATUS_SIZE - n, "},\"calls\":{");
	{
		const struct receipt *r[] = { &ec_read_reply, &ec_write_reply, &pmsf_reply, &ctaf_reply, &gamm_reply };
		const char * const names[] = { "ec_read", "ec_write", "pmsf", "ctaf", "gamm" };
		for (i = 0; i < ARRAY_SIZE(r); i++)
			n += scnprintf(buf + n, STATUS_SIZE - n,
				"%s\"%s\":{\"acpi_status\":%u,\"return_present\":%s,\"return_type\":%u,\"return_value\":%llu}",
				i ? "," : "", names[i], r[i]->status, r[i]->present ? "true" : "false", r[i]->type, r[i]->value);
	}
	n += scnprintf(buf + n, STATUS_SIZE - n, "}}\n");
	return n;
}

static ssize_t bridge_read(struct file *file, char __user *buffer, size_t count,
			   loff_t *position)
{
	struct client *client = file->private_data;
	ssize_t ret;

	if (!privileged())
		return -EPERM;
	mutex_lock(&control_lock);
	if (!*position || !client->length)
		client->length = format_status(client->status, client->text);
	ret = simple_read_from_buffer(buffer, count, position, client->status, client->length);
	mutex_unlock(&control_lock);
	return ret;
}

static int bridge_open(struct inode *inode, struct file *file)
{
	if (!privileged())
		return -EPERM;
	file->private_data = kzalloc(sizeof(struct client), GFP_KERNEL);
	return file->private_data ? 0 : -ENOMEM;
}

static int bridge_close(struct inode *inode, struct file *file)
{
	mutex_lock(&control_lock);
	if (lease_owner == file) {
		last_rc = release_lease("owner_closed");
		lease_owner = NULL;
	}
	mutex_unlock(&control_lock);
	kfree(file->private_data);
	return 0;
}

static const struct file_operations bridge_fops = {
	.owner = THIS_MODULE, .open = bridge_open, .release = bridge_close,
	.read = bridge_read, .write = bridge_write, .llseek = default_llseek,
};

static struct miscdevice bridge = {
	.minor = MISC_DYNAMIC_MINOR, .name = "mechrevo-turbo",
	.fops = &bridge_fops, .mode = 0600,
};

static void watchdog(struct work_struct *work);
static DECLARE_DELAYED_WORK(lease_work, watchdog);

static void watchdog(struct work_struct *work)
{
	struct snapshot s;
	int ret = 0;
	const char *reason = NULL;

	mutex_lock(&control_lock);
	if (active) {
		if (time_after_eq(jiffies, lease_deadline))
			reason = "lease_expired";
		else {
			ret = read_snapshot(&s);
			if (!ret)
				ret = check_active(&s);
			if (ret)
				reason = ret == -ENOLINK ? "adapter_lost" : ret == -EBUSY ? "profile_changed" : "external_change";
		}
		if (reason) {
			last_rc = release_lease(reason);
			sequence++;
		}
	} else if (recovery_needed && recovery_attempts < MAX_RECOVERY_ATTEMPTS &&
		   time_after_eq(jiffies, recovery_next)) {
		last_rc = release_lease(stop_reason);
		sequence++;
	}
	if (!shutting_down)
		schedule_delayed_work(&lease_work, msecs_to_jiffies(GUARD_MS));
	mutex_unlock(&control_lock);
}

static int pm_event(struct notifier_block *nb, unsigned long action, void *unused)
{
	int ret = 0;

	mutex_lock(&control_lock);
	switch (action) {
	case PM_SUSPEND_PREPARE:
	case PM_HIBERNATION_PREPARE:
	case PM_RESTORE_PREPARE:
		pm_blocked = true;
		ret = release_lease("pm_prepare");
		last_rc = ret;
		sequence++;
		if (ret)
			pm_blocked = false;
		break;
	case PM_POST_SUSPEND:
	case PM_POST_HIBERNATION:
	case PM_POST_RESTORE:
		pm_blocked = false;
		break;
	}
	mutex_unlock(&control_lock);
	return ret ? notifier_from_errno(ret) : NOTIFY_OK;
}

static struct notifier_block pm_notifier = { .notifier_call = pm_event };

static acpi_status resource_check(struct acpi_resource *resource, void *context)
{
	bool *found = context;

	if (resource->type == ACPI_RESOURCE_TYPE_FIXED_MEMORY32 &&
	    resource->data.fixed_memory32.address == 0xfe410000 &&
	    resource->data.fixed_memory32.address_length == 0x1000)
		*found = true;
	return AE_OK;
}

static int __init bridge_init(void)
{
	bool resource_found = false;
	int ret;

	if (LINUX_VERSION_MAJOR != 6 || LINUX_VERSION_PATCHLEVEL != 8 ||
	    strcmp(UTS_RELEASE, EXPECTED_RELEASE) ||
	    strcmp(init_uts_ns.name.release, EXPECTED_RELEASE))
		return -ENODEV;
	if (!dmi_match(DMI_SYS_VENDOR, "MECHREVO") ||
	    !dmi_match(DMI_BOARD_NAME, "YAOSHI Series-XxAF55xx") ||
	    !dmi_match(DMI_BIOS_VERSION, "N.1.33MRO91"))
		return -ENODEV;
	if (ACPI_FAILURE(acpi_get_handle(NULL, "\\_SB.INOU", &inou)) ||
	    ACPI_FAILURE(acpi_walk_resources(inou, METHOD_NAME__CRS, resource_check, &resource_found)) ||
	    !resource_found || !wmi_has_guid(OEM_GUID))
		return -ENODEV;
	if (ACPI_FAILURE(acpi_get_handle(inou, "ECRR", &ec_reader)) ||
	    ACPI_FAILURE(acpi_get_handle(inou, "ECRW", &ec_writer)) ||
	    ACPI_FAILURE(acpi_get_handle(NULL, "\\_SB.PC00.LPCB.EC0.PMSF", &policy)))
		return -ENODEV;
	ret = probe_support();
	if (ret)
		return ret;
	ret = register_pm_notifier(&pm_notifier);
	if (ret)
		return ret;
	ret = misc_register(&bridge);
	if (ret) {
		unregister_pm_notifier(&pm_notifier);
		return ret;
	}
	schedule_delayed_work(&lease_work, msecs_to_jiffies(GUARD_MS));
	pr_info("mechrevo_turbo_lease: candidate ABI 1 ready; no lease acquired\n");
	return 0;
}

static void __exit bridge_exit(void)
{
	mutex_lock(&control_lock);
	shutting_down = true;
	mutex_unlock(&control_lock);
	misc_deregister(&bridge);
	unregister_pm_notifier(&pm_notifier);
	cancel_delayed_work_sync(&lease_work);
	mutex_lock(&control_lock);
	if (release_lease("module_exit"))
		pr_err("mechrevo_turbo_lease: firmware restoration failed during unload\n");
	mutex_unlock(&control_lock);
}

module_init(bridge_init);
module_exit(bridge_exit);
MODULE_LICENSE("GPL");
MODULE_DESCRIPTION("Exact XxAF55xx firmware turbo policy lease and genuine WMI ACK bridge");
MODULE_VERSION("0.1-candidate");

#!/usr/bin/python3
"""Compile the candidate's actual control functions against fake firmware.

No module is loaded, no ACPI methods are evaluated, and no device is opened.
This checks the C ownership/lease logic rather than a second Python model.
"""
from pathlib import Path
import re
import subprocess
import tempfile

ROOT = Path(__file__).resolve().parent
source = (ROOT / 'mechrevo_turbo_lease.c').read_text()


def function(name):
    match = re.search(r'(?ms)^static [^;{}]*?\b' + name + r'\([^;{}]*?\)\n\{', source)
    if not match:
        raise RuntimeError('Function missing: ' + name)
    start = match.start()
    end = match.end()
    depth = 1
    while depth:
        if source[end] == '{':
            depth += 1
        elif source[end] == '}':
            depth -= 1
        end += 1
    return source[start:end]


names = ['read_snapshot', 'value', 'custom_state', 'standard_turbo', 'round_adapter',
         'remember_ap_tc', 'refresh_policy', 'probe_support', 'wmi_workload',
         'set_owned', 'check_active', 'restore_byte', 'restore_name', 'finish_recovery',
         'relinquish_policy', 'foreign_bits', 'release_lease', 'acquire_lease',
         'run_command', 'record_wmi_event', 'handle_wmi_mode_switch', 'watchdog', 'pm_event']
functions = [function(name) for name in names]
declarations = source[source.index('static bool shutting_down'):source.index('struct client {')]
commands = re.search(r'(?s)enum command \{.*?\};', source).group()
defines = '\n'.join(line for line in source.splitlines() if line.startswith('#define '))

shim = r'''
#include <assert.h>
#include <stdbool.h>
#include <stdint.h>
#include <stdio.h>
#include <string.h>
#include <errno.h>
typedef uint8_t u8;
typedef uint16_t u16;
typedef uint32_t u32;
typedef uint64_t u64;
typedef unsigned int acpi_status;
struct file { int id; };
struct work_struct { int unused; };
struct notifier_block { int unused; };
struct acpi_object_list { int unused; };
#define ARRAY_SIZE(a) (sizeof(a)/sizeof((a)[0]))
#define BIT(n) (1U<<(n))
#define ACPI_TYPE_INTEGER 1
#define THIS_MODULE 0
#define PM_SUSPEND_PREPARE 1
#define PM_HIBERNATION_PREPARE 2
#define PM_RESTORE_PREPARE 3
#define PM_POST_SUSPEND 4
#define PM_POST_HIBERNATION 5
#define PM_POST_RESTORE 6
#define NOTIFY_OK 0
#define mutex_lock(x) ((void)(x))
#define mutex_unlock(x) ((void)(x))
#define DEFINE_SPINLOCK(x) int x
#define DECLARE_WAIT_QUEUE_HEAD(x) int x
#define spin_lock_irqsave(l, f) do { (void)(l); (f) = 0; } while (0)
#define spin_unlock_irqrestore(l, f) do { (void)(l); (void)(f); } while (0)
#define READ_ONCE(x) (x)
#define msecs_to_jiffies(x) (x)
#define time_after_eq(a,b) ((long)((a)-(b)) >= 0)
#define notifier_from_errno(x) (x)
static int control_lock, wmi_lock, wmi_waitq, lease_work, policy;
static unsigned long jiffies;
static int pins, schedules, wmi_wakes;
static void wake_up_interruptible(int *q) { (void)q; wmi_wakes++; }
static void __module_get(int unused) { (void)unused; pins++; }
static void module_put(int unused) { (void)unused; assert(pins>0); pins--; }
static void schedule_delayed_work(int *work, unsigned delay) { (void)work;(void)delay;schedules++; }
'''

firmware = r'''
static u8 bank[4096], initial[4096];
static u64 fw_cstm, fw_gatm;
static int writes, aptc_writes, pmsfs, wmis, io_count, fail_at, persistent_fail;
static bool fail_next_tb43;
static bool fail_after, support_game=true, support_combined=true;
static bool io_fails(void) { io_count++;return persistent_fail || (fail_at && io_count==fail_at); }
static int read_ec(u16 reg,u8 *out) {
 if(io_fails())return -EIO;
 assert(reg<4096);*out=bank[reg];return 0;
}
static int read_name(const char *path,u64 *out) {
 if(io_fails())return -EIO;
 if(strstr(path,"CSTM"))*out=fw_cstm;
 else { assert(strstr(path,"GATM"));*out=fw_gatm; }
 return 0;
}
static int write_ec(u16 reg,u8 v) {
 bool failure=io_fails();
 if (reg == REG_TB43 && fail_next_tb43) {
  fail_next_tb43 = false;
  persistent_fail = 1; /* Also fail the first recovery snapshot. */
  return -EIO;
 }
 assert(reg==REG_APPE||reg==REG_TB43||reg==REG_APTC);
 if(failure&&!fail_after)return -EIO;
 bank[reg]=v;writes++;if(reg==REG_APTC)aptc_writes++;
 return failure?-EIO:0;
}
static void fake_policy(void) {
 pmsfs++;
 if(bank[REG_APPE]&1) {
  if(fw_cstm||fw_gatm)bank[REG_APTC]=0x80|((bank[REG_TB43]&1)?0x12:0x0a);
  else bank[REG_APTC]&=0x7f;
 }
}
static int evaluate(int handle,struct acpi_object_list *args,struct receipt *reply) {
 bool failure=io_fails();(void)handle;(void)args;
 memset(reply,0,sizeof(*reply));reply->status=failure?1:0;
 if(failure&&!fail_after)return -EIO;
 fake_policy();return failure?-EIO:0;
}
static int wmi_call(u8 code,struct receipt *reply) {
 bool failure=io_fails();
 memset(reply,0,sizeof(*reply));reply->status=failure?1:0;
 if(failure&&!fail_after)return -EIO;
 reply->present=true;reply->type=ACPI_TYPE_INTEGER;
 if(code==3){reply->value=support_combined?0x55:0xaa;return failure?-EIO:0;}
 if(code==0x13){reply->value=support_game?0x55:0xaa;return failure?-EIO:0;}
 wmis++;
 switch(code){case 1:fw_cstm=0;break;case 2:fw_cstm=1;break;
 case 0x14:fw_gatm=1;break;case 0x15:fw_gatm=0;break;default:assert(false);}
 reply->value=(code==2||code==0x14)?0xbb:0xaa;
 fake_policy();return failure?-EIO:0;
}
'''

tests = r'''
static struct file owner={1}, stranger={2};
static void reset(void) {
 assert(!pins);
 memset(bank,0,sizeof(bank));
 bank[0x706]=0x40;bank[0x727]=0x80;bank[REG_MODE]=0x30;
 bank[REG_APPE]=0x80;bank[REG_TB43]=0xa0;bank[0x7c5]=0x47;
 bank[0x7cc]=0x81;bank[0x490]=7;
 memcpy(initial,bank,sizeof(bank));fw_cstm=fw_gatm=0;
 active=recovery_needed=recovery_pin=restoring=shutting_down=pm_blocked=false;
 lease_owner=NULL;cstm_attempted=gatm_attempted=cstm_restoring=gatm_restoring=false;
 memset(owned,0,sizeof(owned));
 owned[0].reg=REG_APPE;owned[0].mask=1;
 owned[1].reg=REG_TB43;owned[1].mask=1;
 owned[2].reg=REG_APTC;owned[2].mask=255;
 writes=aptc_writes=pmsfs=wmis=io_count=fail_at=persistent_fail=0;
 fail_after=false;support_game=support_combined=true;
 jiffies=0;sequence=0;recovery_attempts=restore_skipped=0;
 wmi_bound=false;wmi_seq=wmi_non_int=wmi_mode_seq=wmi_handled_mode_seq=0;
 wmi_has_last=false;wmi_last_code=0;wmi_wakes=0;memset(wmi_ring,0,sizeof(wmi_ring));
}
static void acquire(void) {
 assert(run_command(&owner,ACQUIRE_QUIET)==0);
 assert(active&&pins==1&&(bank[REG_APPE]&1)&&(bank[REG_TB43]&1));
}
static void released(void) {
 assert(!active&&!recovery_needed&&!pins);
 assert(!memcmp(initial,bank,sizeof(bank)));
 assert(fw_cstm==0&&fw_gatm==0);
}
int main(void) {
 struct snapshot s;int n,phase,ret,prior,foreign_writes,foreign_pmsfs,release_skips=0,acquire_skips=0;
 int acquire_steps,release_steps,game_steps;
 reset();acquire();
 assert(run_command(&stranger,HEARTBEAT)==-EPERM);
 assert(run_command(&stranger,RELEASE)==-EPERM);
 assert(run_command(&owner,GAME_ON)==0&&command_reply.value==0xbb);
 assert(run_command(&owner,COMBINED_ON)==0&&command_reply.value==0xbb);
 assert(run_command(&owner,VARIANT_EXTREME)==0);
 assert(run_command(&owner,GAME_OFF)==0&&command_reply.value==0xaa);
 assert(run_command(&owner,RELEASE)==0);released();
 puts("PASS normal lease, real receipt routing, foreign FD refusal, mask restoration");
 for(n=0;n<256;n++) {
  reset();bank[0x7cc]=n;bank[0x490]=1;assert(!read_snapshot(&s));
  assert(round_adapter(&s)==((n&1)||!(n&15)));
  bank[0x490]=0;assert(!read_snapshot(&s));assert(round_adapter(&s)==!!(n&1));
 }
 puts("PASS all 256 adapter states and USB-C exclusion");
 reset();support_game=false;acquire();prior=wmis;
 assert(run_command(&owner,GAME_ON)==-EOPNOTSUPP&&wmis==prior);
 assert(run_command(&owner,COMBINED_ON)==0);
 assert(run_command(&owner,RELEASE)==0);released();
 reset();support_combined=false;assert(run_command(&owner,ACQUIRE_QUIET)==-EOPNOTSUPP&&!writes&&!pins);
 puts("PASS actual support-code gates");
 reset();acquire();assert(!run_command(&owner,GAME_ON));
 bank[REG_MODE]=0xa0;bank[REG_APTC]=77;initial[REG_MODE]=0xa0;
 watchdog(NULL);assert(!active&&!pins&&bank[REG_MODE]==0xa0&&!aptc_writes);
 assert(restore_skipped&BIT(2));initial[REG_APTC]=bank[REG_APTC];released();
 puts("PASS mode-key wins; no APTC restoration into new profile");
 for(n=0;n<3;n++) {
  reset();acquire();assert(!run_command(&owner,GAME_ON));
  if(n==0)bank[0x743]|=4;
  if(n==1)bank[REG_APTC]=77;
  if(n==2)fw_cstm=1;
  foreign_writes=writes;foreign_pmsfs=pmsfs;
  watchdog(NULL);
  assert(!active&&!pins&&restore_skipped&&writes==foreign_writes&&pmsfs==foreign_pmsfs);
 }
 puts("PASS custom DB bit and foreign policy ownership: zero recovery writes/method calls");
 reset();acquire();jiffies=LEASE_MS;watchdog(NULL);released();
 reset();acquire();bank[0x7cc]=2;initial[0x7cc]=2;watchdog(NULL);released();
 reset();acquire();assert(pm_event(NULL,PM_SUSPEND_PREPARE,NULL)==0);released();
 assert(run_command(&owner,ACQUIRE_QUIET)==-ESHUTDOWN);
 assert(pm_event(NULL,PM_POST_SUSPEND,NULL)==0&&!active);
 puts("PASS timeout, AC removal, PM prepare and no automatic resume acquisition");
 reset();acquire();persistent_fail=1;
 assert(run_command(&owner,RELEASE)==-EIO&&recovery_needed&&pins==1);
 for(n=0;n<20;n++){jiffies+=RECOVERY_MS;watchdog(NULL);}
 assert(recovery_attempts==MAX_RECOVERY_ATTEMPTS&&recovery_needed&&pins==1);
 persistent_fail=0;assert(run_command(&owner,RELEASE)==0);released();
 puts("PASS bounded failed recovery remains pinned until explicit successful recovery");
 reset();acquire();fail_at=io_count+1;
 assert(run_command(&owner,RELEASE)==-EIO&&recovery_needed&&pins==1);
 jiffies+=RECOVERY_MS;watchdog(NULL);released();
 puts("PASS transient read fault recovered by kernel watchdog");
 for (phase=0; phase<2; phase++) {
  reset();acquire();
  fail_next_tb43=true;
  assert(run_command(&owner,VARIANT_EXTREME)==-EIO && recovery_needed && pins==1);
  persistent_fail=0;
  if (phase) assert(pm_event(NULL,PM_SUSPEND_PREPARE,NULL)==0);
  else assert(run_command(&owner,RELEASE)==0);
  released();
 }
 puts("PASS failed variant plus failed recovery snapshot, retried by owner and PM");
 reset();acquire();assert(!run_command(&owner,COMBINED_ON));
 /* The first I/O after the eligibility snapshot is the WMI call. */
 fail_at=io_count+ARRAY_SIZE(registers)+3;
 assert(run_command(&owner,COMBINED_OFF)==-EIO);
 released();
 puts("PASS failed workload OFF restores the previously active workload");
 reset();prior=io_count;acquire();acquire_steps=io_count-prior;
 prior=io_count;assert(!run_command(&owner,GAME_ON));game_steps=io_count-prior;
 assert(!run_command(&owner,COMBINED_ON));prior=io_count;
 assert(!run_command(&owner,RELEASE));release_steps=io_count-prior;released();
 for(phase=0;phase<2;phase++)for(n=1;n<=release_steps;n++) {
  reset();acquire();assert(!run_command(&owner,GAME_ON));assert(!run_command(&owner,COMBINED_ON));
  fail_at=io_count+n;fail_after=phase;ret=run_command(&owner,RELEASE);fail_at=0;
  if(recovery_needed){jiffies+=RECOVERY_MS;watchdog(NULL);}
  if(recovery_needed)assert(!run_command(&owner,RELEASE));
  assert(!pins&&!active&&!recovery_needed);
  if(restore_skipped)release_skips++;
  released();
  (void)ret;
 }
 printf("PASS %d release I/O positions, each before/after acceptance; all original state restored\n",release_steps);
 printf("NOTE release fault positions with already-original mask skipped: %d/%d\n",release_skips,release_steps*2);
 /* Fail every direct write before/after acceptance while acquiring. All
  * failures leave either a restored policy or an explicitly pinned recovery. */
 for(phase=0;phase<2;phase++)for(n=1;n<=acquire_steps;n++) {
  reset();fail_at=n;fail_after=phase;ret=run_command(&owner,ACQUIRE_QUIET);
  fail_at=0;
  if(active||recovery_needed)assert(!run_command(&owner,RELEASE));
  assert(!pins&&!active&&!recovery_needed);
  if(restore_skipped)acquire_skips++;
  released();
  (void)ret;
 }
 printf("PASS %d acquire I/O positions, each before/after acceptance; all original state restored\n",acquire_steps);
 printf("NOTE acquire fault positions with already-original mask skipped: %d/%d\n",acquire_skips,acquire_steps*2);
 for(phase=0;phase<2;phase++)for(n=1;n<=game_steps;n++) {
  reset();acquire();fail_at=io_count+n;fail_after=phase;
  ret=run_command(&owner,GAME_ON);fail_at=0;
  if(active||recovery_needed)assert(!run_command(&owner,RELEASE));
  if(memcmp(initial,bank,sizeof(bank)))fprintf(stderr,"game fault %d rc=%d skipped=%u reason=%s\n",n,ret,restore_skipped,stop_reason);
  released();
 }
 printf("PASS %d active WMI I/O positions, each before/after acceptance; all original state restored\n",game_steps);
 reset();acquire();assert(!run_command(&owner,COMBINED_ON));
 for(n=0;n<20;n++)assert(record_wmi_event(0xB4,(u64)(n+1)*10)==false);
 assert(wmi_seq==20&&wmi_wakes==20&&wmi_last_code==0xB4&&wmi_has_last);
 assert(wmi_ring[0].seq==17&&wmi_ring[3].seq==20);
 assert(handle_wmi_mode_switch()==false&&active);
 assert(record_wmi_event(WMI_SCANCODE_MODE_SWITCH,250)==true);
 assert(handle_wmi_mode_switch()==true&&!active&&!strcmp(stop_reason,"mode_key_wmi"));
 released();
 assert(handle_wmi_mode_switch()==false);
 puts("PASS WMI ring wraparound and 0xB0 immediate lease release");
 return 0;
}
'''

prototypes = '\n'.join(item.split('\n{', 1)[0] + ';' for item in functions)
translation = '\n'.join([shim, defines, declarations, commands, prototypes, firmware,
                         '\n\n'.join(functions), tests])
with tempfile.TemporaryDirectory(prefix='mechrevo-lease-test-') as directory:
    path = Path(directory)
    (path / 'core.c').write_text(translation)
    subprocess.run(['gcc', '-std=gnu11', '-O1', '-g', '-Wall', '-Wextra',
                    '-Wno-unused-parameter', '-Wno-sign-compare', '-Wno-unused-variable',
                    '-fsanitize=address,undefined', '-fno-omit-frame-pointer',
                    str(path / 'core.c'), '-o', str(path / 'core')], check=True)
    subprocess.run([str(path / 'core')], check=True)

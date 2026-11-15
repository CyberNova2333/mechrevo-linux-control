// Static analysis rendering; not original buildable source.
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using \u000b\u0009\u0013\u000f\u0017\u0005;

namespace GCUService.MySystem.CPUInfo;

public class CpuUsageMonitor
{
	private struct SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION
	{
		public long IdleTime;

		public long KernelTime;

		public long UserTime;

		public long DpcTime;

		public long InterruptTime;

		public int InterruptCount;
	}

	private const int SystemProcessorPerformanceInformation = 8;

	private long _prevIdle;

	private long _prevKernel;

	private long _prevUser;

	private bool _hasPrevSample;

	[DllImport("ntdll.dll")]
	private static extern int NtQuerySystemInformation(int SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength, out int ReturnLength);

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetCPUUsage()
	{
		if (!TryGetAggregatedTimes(out var idle, out var kernel, out var user))
		{
			return 0;
		}
		int result = 0;
		if (_hasPrevSample)
		{
			long num = idle - _prevIdle;
			long num2 = kernel - _prevKernel;
			long num3 = user - _prevUser;
			long num4 = num2 + num3;
			long num5 = num4 - num;
			if (num4 > 0)
			{
				double num6 = (double)num5 / (double)num4 * 100.0;
				if (num6 < 0.0)
				{
					num6 = 0.0;
				}
				if (num6 > 100.0)
				{
					num6 = 100.0;
				}
				result = (int)Math.Round(num6);
			}
		}
		_prevIdle = idle;
		_prevKernel = kernel;
		_prevUser = user;
		_hasPrevSample = true;
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool TryGetAggregatedTimes(out long idle, out long kernel, out long user)
	{
		idle = (kernel = (user = 0L));
		int num = ResolvedDelegates.System_Environment_get_ProcessorCount__440();
		int num2 = ResolvedDelegates.System_Runtime_InteropServices_Marshal_SizeOf__114(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION).TypeHandle));
		int num3 = num2 * num;
		IntPtr intPtr = ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(num3);
		try
		{
			if (NtQuerySystemInformation(8, intPtr, num3, out var _) != 0)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION sYSTEM_PROCESSOR_PERFORMANCE_INFORMATION = Marshal.PtrToStructure<SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION>(IntPtr.Add(intPtr, i * num2));
				idle += sYSTEM_PROCESSOR_PERFORMANCE_INFORMATION.IdleTime;
				kernel += sYSTEM_PROCESSOR_PERFORMANCE_INFORMATION.KernelTime;
				user += sYSTEM_PROCESSOR_PERFORMANCE_INFORMATION.UserTime;
			}
			return true;
		}
		finally
		{
			ResolvedDelegates.System_Runtime_InteropServices_Marshal_FreeHGlobal__90(intPtr);
		}
	}

	public CpuUsageMonitor()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
	}
}

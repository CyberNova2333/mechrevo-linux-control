// Static analysis rendering; not original buildable source.
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using GCUService.MySystem.CPUInfo;
using Utility;

namespace MyControlCenter;

internal class CPUInfo
{
	private PerformanceCounter perfUptimeCount;

	private PerformanceCounter perfTempZoneSB;

	private PerformanceCounter freqCounter;

	private static PerformanceCounter cpuCounter;

	private static PerformanceCounter cpuCounterforCPUGPUCombine;

	private PerformanceCounter Disk;

	private static PerformanceCounterCategory Network;

	private string[] instanceName;

	public PerformanceCounter[] CPUCounters;

	public string maxFrequency;

	private ManagementObjectSearcher searcher;

	private CpuUsageMonitor _cpuMonitor = new CpuUsageMonitor();

	private const string _categoryName = "Thermal Zone Information";

	private const string _processIdCounter = "Temperature";

	public float[] CoreUsage
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get;
		[MethodImpl(MethodImplOptions.NoInlining)]
		set;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public CPUInfo()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected Obj, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected Obj, but got Unknown
		ResolvedDelegates.System_Object_ctor__0(this);
		try
		{
			int cpuMaxFrequency = GetCpuMaxFrequency();
			maxFrequency = ResolvedDelegates.System_Int32_ToString__55(ref cpuMaxFrequency);
			try
			{
				searcher = new ManagementObjectSearcher("SELECT ProcessorFrequency, PercentProcessorPerformance FROM Win32_PerfFormattedData_Counters_ProcessorInformation WHERE Name = '_Total'");
			}
			catch (Exception ex)
			{
				LogCtrl.TraceMessageProgramData("Win32_PerfFormattedData_Counters_ProcessorInformation failed: " + ex, ".ctor", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\CPUInfo.cs", 63);
			}
			try
			{
				perfTempZoneSB = new PerformanceCounter("Thermal Zone Information", "Temperature", ResolvedDelegates.System_Object_ToString__1(GetTInstance()));
				if (perfTempZoneSB != null)
				{
					ResolvedDelegates.System_Diagnostics_PerformanceCounter_NextValue__564(perfTempZoneSB);
				}
			}
			catch (Exception ex2)
			{
				LogCtrl.TraceMessageProgramData("Temperature failed: " + ex2, ".ctor", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\CPUInfo.cs", 76);
			}
		}
		catch (Exception ex3)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("System monitor", ResolvedDelegates.System_Object_ToString__1(ex3)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Update()
	{
		CoreUsage = CPUCounters.Select([MethodImpl(MethodImplOptions.NoInlining)] (PerformanceCounter o) => ResolvedDelegates.System_Diagnostics_PerformanceCounter_NextValue__564(o)).ToArray();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe static double Calculate(CounterSample oldSample, CounterSample newSample)
	{
		//IL_0012: Expected Obj, but got Ref
		//IL_0024: Expected Obj, but got Ref
		//IL_0039: Expected Obj, but got Ref
		//IL_004b: Expected Obj, but got Ref
		double num = ResolvedDelegates.System_Diagnostics_CounterSample_get_RawValue__643((object)(&newSample)) - ResolvedDelegates.System_Diagnostics_CounterSample_get_RawValue__643((object)(&oldSample));
		double num2 = ResolvedDelegates.System_Diagnostics_CounterSample_get_TimeStamp100nSec__644((object)(&newSample)) - ResolvedDelegates.System_Diagnostics_CounterSample_get_TimeStamp100nSec__644((object)(&oldSample));
		if (num2 != 0.0)
		{
			return 100.0 * (1.0 - num / num2);
		}
		return 0.0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static string GetTInstance()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected Obj, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected Obj, but got Unknown
		string result = "";
		if ((int)ResolvedDelegates.System_Diagnostics_PerformanceCounterCategory_get_CategoryType__645((object)new PerformanceCounterCategory("Thermal Zone Information")) == 0)
		{
			PerformanceCounter[] array = ResolvedDelegates.System_Diagnostics_PerformanceCounterCategory_GetCounters__646((object)new PerformanceCounterCategory("Temperature"));
			int num = 0;
			if (num < array.Length)
			{
				return array[num].InstanceName;
			}
		}
		else
		{
			PerformanceCounterCategory val = new PerformanceCounterCategory("Thermal Zone Information");
			string[] array2 = ResolvedDelegates.System_Diagnostics_PerformanceCounterCategory_GetInstanceNames__563(val);
			foreach (string text in array2)
			{
				PerformanceCounter[] array = ResolvedDelegates.System_Diagnostics_PerformanceCounterCategory_GetCounters__587(val, text);
				int num2 = 0;
				if (num2 < array.Length)
				{
					return array[num2].InstanceName;
				}
			}
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetCPUTemperature()
	{
		double num = 0.0;
		if (perfTempZoneSB != null)
		{
			num = ResolvedDelegates.System_Diagnostics_PerformanceCounter_NextValue__564(perfTempZoneSB);
			num -= 273.2;
		}
		return (int)num;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetActualCpuFrequencyMHz()
	{
		try
		{
			ManagementObject val = ((IEnumerable)ResolvedDelegates.System_Management_ManagementObjectSearcher_Get__199(searcher)).Cast<ManagementObject>().FirstOrDefault();
			if (val != null)
			{
				object obj = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205(val, "ProcessorFrequency");
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205(val, "PercentProcessorPerformance");
				double num = ((obj != null) ? Convert.ToDouble(obj) : 0.0);
				double num2 = ((obj2 != null) ? Convert.ToDouble(obj2) : 0.0);
				if (num > 0.0)
				{
					return (int)(num * (num2 / 100.0));
				}
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessageProgramData("GetActualCpuFrequencyGHz error: " + ex, "GetActualCpuFrequencyMHz", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\CPUInfo.cs", 277);
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetCpuMaxFrequency()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		try
		{
			ManagementObjectSearcher val = new ManagementObjectSearcher("select MaxClockSpeed from Win32_Processor");
			try
			{
				ManagementObject val2 = ((IEnumerable)ResolvedDelegates.System_Management_ManagementObjectSearcher_Get__199(val)).Cast<ManagementObject>().FirstOrDefault();
				if (val2 != null)
				{
					return ResolvedDelegates.System_Convert_ToInt32__47(ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205(val2, "MaxClockSpeed"));
				}
			}
			finally
			{
				if (val != null)
				{
					ResolvedDelegates.System_IDisposable_Dispose__3(val);
				}
			}
		}
		catch (Exception ex)
		{
			LogCtrl.Write("GetCpuMaxFrequency failed: " + ex);
		}
		return 0;
	}

	public int GetCPUUsage()
	{
		return _cpuMonitor.GetCPUUsage();
	}

	public int GetCPUUsageforCPUGPUCombine()
	{
		return _cpuMonitor.GetCPUUsage();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetDiskUsage()
	{
		return ResolvedDelegates.System_Convert_ToInt32__138(ResolvedDelegates.System_Diagnostics_PerformanceCounter_NextValue__564(Disk));
	}
}

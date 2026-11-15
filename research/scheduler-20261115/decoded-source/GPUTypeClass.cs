// Static analysis rendering; not original buildable source.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using GCUService.MySystem.GPUInfo;
using MyECIO;
using Utility;

namespace MyControlCenter;

internal class GPUTypeClass
{
	public delegate void GPUInfoCBK(IntPtr files, int count);

	public static string[] GpuInfo2 = new string[10]
	{
		"0",
		"0",
		"0",
		"0",
		"0",
		"0",
		"0",
		"0",
		"0",
		"0"
	};

	private bool IsNvGpu = MyEcCtrl.Instance.GetNvGpu() == 1;

	private static List<PerformanceCounter> _gpuCounters = null;

	public uint GpuTempature
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get;
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal set;
	}

	public uint GpuUsage
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get;
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal set;
	}

	public uint GpuCoreFreq
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get;
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal set;
	}

	public uint GpuMemFreq
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get;
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal set;
	}

	public uint GpuPState
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get;
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal set;
	}

	public ulong GpuPhysicalRamSize
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get;
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal set;
	}

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool GetNVGpuCount(ref uint count);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool GetNVTemperature(uint index, ref uint temperature);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool GetNVGpuUsage(uint index, ref uint percentage);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool GetNVCoreFreq(uint index, ref uint clock);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool GetNVMemFreq(uint index, ref uint clock);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool GetNVPState(uint index, ref uint pstatue);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool GetNVPhysicalRam(uint index, ref ulong RamSize, ref uint RamType);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool GetNVGpuInfo2(GPUInfoCBK callback);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool GetNvGpuThermalLimitInfo(uint index, ref uint LimitDefault, ref uint LimitMax, ref uint LimitMin, ref uint CurrentValue);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool SetNVPState(uint index, ref uint GPUPstate);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool SetNVCoreFreqOffset(uint index, ref int CoreclockOffset);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool SetNVMemFreqOffset(uint index, ref int MemclockOffset);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool SetNVGpuThermalPoliciesStatus(uint index, ref int thermalLimit);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool GetNVGpuWhisperMode2p0Support();

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool SetNVGpuWhisperModeStatus(bool bEnable);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool SetNVGpuWhisperModeSetting(ref int Setting);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern bool SetNVGpuWhisperModeMinFps(ref int MinFps);

	[DllImport("GPUInfoDLL.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern void GetNVHandle();

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void SetGPUTempature()
	{
		uint count = 0u;
		uint temperature = 0u;
		try
		{
			if (IsNvGpu)
			{
				GetNVGpuCount(ref count);
				for (uint num = 0u; num < count; num++)
				{
					GetNVTemperature(num, ref temperature);
				}
			}
			else
			{
				temperature = (uint)GpuMonitor.GetGpuTemperature();
			}
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Gpu Usage read error: ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
		GpuTempature = temperature;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void SetGpuUsage()
	{
		uint count = 0u;
		uint percentage = 0u;
		try
		{
			if (IsNvGpu)
			{
				GetNVGpuCount(ref count);
				for (uint num = 0u; num < count; num++)
				{
					GetNVGpuUsage(num, ref percentage);
				}
			}
			else
			{
				percentage = (uint)GpuMonitor.GetGpuUsage();
			}
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Gpu Usage read error: ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
		GpuUsage = percentage;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe static float GetGpuUsageFromPerfCounterOptimized()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected Obj, but got Unknown
		float num = 0f;
		List<PerformanceCounter> list = new List<PerformanceCounter>();
		try
		{
			string[] array = ResolvedDelegates.System_Diagnostics_PerformanceCounterCategory_GetInstanceNames__563((object)new PerformanceCounterCategory("GPU Engine"));
			Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!ResolvedDelegates.System_String_Contains__108(ref *(string*)text, "engtype_3D"))
				{
					continue;
				}
				int num2 = ExtractProcessId(text);
				if (num2 > 0)
				{
					if (!dictionary.ContainsKey(num2))
					{
						dictionary[num2] = new List<string>();
					}
					dictionary[num2].Add(text);
				}
			}
			foreach (KeyValuePair<int, List<string>> item in dictionary)
			{
				if (item.Value.Count > 0)
				{
					try
					{
						string text2 = item.Value[0];
						PerformanceCounter val = new PerformanceCounter("GPU Engine", "Utilization Percentage", text2, true);
						ResolvedDelegates.System_Diagnostics_PerformanceCounter_NextValue__564(val);
						list.Add(val);
					}
					catch
					{
					}
				}
			}
			if (list.Count == 0)
			{
				return 0f;
			}
			Thread.Sleep(100);
			foreach (PerformanceCounter item2 in list)
			{
				try
				{
					float num3 = ResolvedDelegates.System_Diagnostics_PerformanceCounter_NextValue__564(item2);
					_ = 0.1f;
					if (num3 > 0f)
					{
						num += num3;
					}
				}
				catch
				{
				}
			}
			foreach (PerformanceCounter item3 in list)
			{
				((Component)(object)item3).Dispose();
			}
			return Math.Min(num, 100f);
		}
		catch (Exception)
		{
			foreach (PerformanceCounter item4 in list)
			{
				try
				{
					ResolvedDelegates.System_ComponentModel_Component_Dispose__198(item4);
				}
				catch
				{
				}
			}
			return 0f;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe static int ExtractProcessId(string instance)
	{
		try
		{
			int num = ResolvedDelegates.System_String_IndexOf__549(ref *(string*)instance, "pid_");
			if (num >= 0)
			{
				num += 4;
				int num2 = ResolvedDelegates.System_String_IndexOf__566(ref *(string*)instance, '_', num);
				int result = default;
				if (num2 > num && ResolvedDelegates.System_Int32_TryParse__132(ResolvedDelegates.System_String_Substring__109(ref *(string*)instance, num, num2 - num), ref result))
				{
					return result;
				}
			}
		}
		catch
		{
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static uint GetGpuUsage_PerfCounter()
	{
		float num = 0f;
		try
		{
			if (_gpuCounters == null || _gpuCounters.Count == 0)
			{
				InitGpuCounters();
			}
			foreach (PerformanceCounter gpuCounter in _gpuCounters)
			{
				num += ResolvedDelegates.System_Diagnostics_PerformanceCounter_NextValue__564(gpuCounter);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Gpu Usage read error: ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
		return (uint)num;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe static void InitGpuCounters()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected Obj, but got Unknown
		try
		{
			_gpuCounters = (from n in ResolvedDelegates.System_Diagnostics_PerformanceCounterCategory_GetInstanceNames__563((object)new PerformanceCounterCategory("GPU Engine"))
				where ResolvedDelegates.System_String_Contains__108(ref *(string*)n, "engtype_3D")
				select n).Select((Func<string, PerformanceCounter>)([MethodImpl(MethodImplOptions.NoInlining)] (string n) =>
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Expected Obj, but got Unknown
				return new PerformanceCounter("GPU Engine", "Utilization Percentage", n);
			})).ToList();
			foreach (PerformanceCounter gpuCounter in _gpuCounters)
			{
				gpuCounter.NextValue();
			}
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("InitGpuCounters error: ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void SetGpuCoreFreq()
	{
		uint count = 0u;
		uint clock = 0u;
		GetNVGpuCount(ref count);
		for (uint num = 0u; num < count; num++)
		{
			GetNVCoreFreq(num, ref clock);
		}
		GpuCoreFreq = clock;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void SetGpuMemFreq()
	{
		uint count = 0u;
		uint clock = 0u;
		GetNVGpuCount(ref count);
		for (uint num = 0u; num < count; num++)
		{
			GetNVMemFreq(num, ref clock);
		}
		GpuMemFreq = clock;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void SetGpuPState()
	{
		uint gpuPState = 0u;
		GpuPState = gpuPState;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void GetGpuThermalLimitInfoValue(ref uint LimitDefault, ref uint LimitMax, ref uint LimitMin, ref uint CurrentValue)
	{
		uint count = 0u;
		GetNVGpuCount(ref count);
		for (uint num = 0u; num < count; num++)
		{
			GetNvGpuThermalLimitInfo(num, ref LimitDefault, ref LimitMax, ref LimitMin, ref CurrentValue);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void SetGpuPhysicalRamSize()
	{
		uint count = 0u;
		ulong RamSize = 0uL;
		uint RamType = 0u;
		GetNVGpuCount(ref count);
		for (uint num = 0u; num < count; num++)
		{
			GetNVPhysicalRam(num, ref RamSize, ref RamType);
		}
		GpuPhysicalRamSize = RamSize;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual bool SetGpuCoreFreqOffset(int value)
	{
		uint count = 0u;
		try
		{
			GetNVGpuCount(ref count);
			for (uint num = 0u; num < count; num++)
			{
				if (!SetNVCoreFreqOffset(num, ref value))
				{
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("SetGpuCoreFreqOffset exception, ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual bool SetGpuMemFreqOffset(int value)
	{
		uint count = 0u;
		try
		{
			GetNVGpuCount(ref count);
			for (uint num = 0u; num < count; num++)
			{
				if (!SetNVMemFreqOffset(num, ref value))
				{
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("SetGpuMemFreqOffset exception, ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual bool SetGpuTargetTemperature(int value)
	{
		uint count = 0u;
		try
		{
			GetNVGpuCount(ref count);
			for (uint num = 0u; num < count; num++)
			{
				if (!SetNVGpuThermalPoliciesStatus(num, ref value))
				{
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("SetGpuTargetTemperature exception, ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual bool GetGpuWhisperMode2p0Support()
	{
		try
		{
			if (!GetNVGpuWhisperMode2p0Support())
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetGpuWhisperMode2p0Support exception, ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual bool SetGpuWhisperMode(bool status)
	{
		try
		{
			if (!SetNVGpuWhisperModeStatus(status))
			{
				ResolvedDelegates.System_Console_WriteLine__8("fail");
				return false;
			}
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("exception, ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual bool SetGpuWhisperModeSetting(int value)
	{
		try
		{
			if (!SetNVGpuWhisperModeSetting(ref value))
			{
				Console.WriteLine("fail");
				return false;
			}
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("SetGpuWhisperModeSetting, value:", ResolvedDelegates.System_Int32_ToString__55(ref value)), "SetGpuWhisperModeSetting", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\GPUInfo\\GPUTypeClass.cs", 535);
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("exception, ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual bool SetGpuWhisperModeMinFps(int value)
	{
		try
		{
			if (!SetNVGpuWhisperModeMinFps(ref value))
			{
				LogCtrl.TraceMessageProgramData("LogCtrl.TraceMessageProgramData(\"@@@Whisper WhisperModeMinFps_QUIETER return false", "SetGpuWhisperModeMinFps", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\GPUInfo\\GPUTypeClass.cs", 564);
				ResolvedDelegates.System_Console_WriteLine__8("fail");
				LogCtrl.TraceMessage("fail", "SetGpuWhisperModeMinFps", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\GPUInfo\\GPUTypeClass.cs", 567);
				return false;
			}
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("SetGpuWhisperModeMinFps, value:", ResolvedDelegates.System_Int32_ToString__55(ref value)), "SetGpuWhisperModeMinFps", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\GPUInfo\\GPUTypeClass.cs", 559);
			LogCtrl.TraceMessageProgramData("LogCtrl.TraceMessageProgramData(\"@@@Whisper WhisperModeMinFps_QUIETER SetNVGpuWhisperModeMinFps return true", "SetGpuWhisperModeMinFps", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\GPUInfo\\GPUTypeClass.cs", 560);
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("exception, ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("exception, ", ResolvedDelegates.System_Exception_get_Message__58(ex)), "SetGpuWhisperModeMinFps", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\GPUInfo\\GPUTypeClass.cs", 574);
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void GetNvGpuHandle()
	{
		try
		{
			GetNVHandle();
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("exception, ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetGpuTemperature()
	{
		SetGPUTempature();
		return GpuTempature;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetGpuUsage()
	{
		SetGpuUsage();
		return GpuUsage;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetGpuCoreFreq()
	{
		SetGpuCoreFreq();
		return GpuCoreFreq;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetGpuMemFreq()
	{
		SetGpuMemFreq();
		return GpuMemFreq;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetGpuPState()
	{
		SetGpuPState();
		return GpuPState;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public ulong GetGpuPhysicalRamSize()
	{
		SetGpuPhysicalRamSize();
		return GpuPhysicalRamSize;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void GetGpuThermalLimitInfo(ref uint LimitDefault, ref uint LimitMax, ref uint LimitMin, ref uint CurrentValue)
	{
		GetGpuThermalLimitInfoValue(ref LimitDefault, ref LimitMax, ref LimitMin, ref CurrentValue);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuCoreFreqOffsetValue(int value)
	{
		return SetGpuCoreFreqOffset(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuMemFreqOffsetValue(int value)
	{
		return SetGpuMemFreqOffset(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuTargetTemperatureValue(int value)
	{
		return SetGpuTargetTemperature(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsGpuWhisperMode2p0Support()
	{
		return GetGpuWhisperMode2p0Support();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuWhisperModeStatus(bool status)
	{
		return SetGpuWhisperMode(status);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuWhisperModeSettingValue(int value)
	{
		return SetGpuWhisperModeSetting(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuWhisperModeMinFpsValue(int value)
	{
		return SetGpuWhisperModeMinFps(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void GetGpuHandle()
	{
		GetNvGpuHandle();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public GPUTypeClass()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
	}
}

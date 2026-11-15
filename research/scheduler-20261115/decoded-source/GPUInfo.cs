// Static analysis rendering; not original buildable source.
using System.Runtime.CompilerServices;
using \u000b\u0009\u0013\u000f\u0017\u0005;

namespace MyControlCenter;

internal class GPUInfo
{
	private GPUTypeClass GPUType = new GPUTypeClass();

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe GPUInfo()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
		string graphicInfo = new HardwareInfoCollect().getGraphicInfo();
		if (ResolvedDelegates.System_String_Contains__108(ref *(string*)graphicInfo, "NVIDIA"))
		{
			GPUType = new NVIDIA();
		}
		else if (graphicInfo.Contains("ATI"))
		{
			GPUType = new ATI();
		}
		else
		{
			GPUType = new GPUTypeClass();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetGpuTemperature()
	{
		return GPUType.GetGpuTemperature();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetGpuUsage()
	{
		return GPUType.GetGpuUsage();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetGpuCoreFreq()
	{
		return GPUType.GetGpuCoreFreq();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetGpuMemFreq()
	{
		return GPUType.GetGpuMemFreq();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetGpuPState()
	{
		return GPUType.GetGpuPState();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public ulong GetGpuPhysicalRamSize()
	{
		return GPUType.GetGpuPhysicalRamSize();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void GetGpuThermalLimitInfo(ref uint LimitDefault, ref uint LimitMax, ref uint LimitMin, ref uint CurrentValue)
	{
		GPUType.GetGpuThermalLimitInfo(ref LimitDefault, ref LimitMax, ref LimitMin, ref CurrentValue);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuCoreFreqOffsetValue(int value)
	{
		return GPUType.SetGpuCoreFreqOffsetValue(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuMemFreqOffsetValue(int value)
	{
		return GPUType.SetGpuMemFreqOffsetValue(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuTargetTemperatureValue(int value)
	{
		return GPUType.SetGpuTargetTemperatureValue(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsGpuWhisperMode2p0Support()
	{
		return GPUType.IsGpuWhisperMode2p0Support();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuWhisperModeStatus(bool status)
	{
		return GPUType.SetGpuWhisperModeStatus(status);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuWhisperModeSettingValue(int value)
	{
		return GPUType.SetGpuWhisperModeSettingValue(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuWhisperModeMinFpsValue(int value)
	{
		return GPUType.SetGpuWhisperModeMinFpsValue(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void GetGpuHandle()
	{
		GPUType.GetGpuHandle();
	}
}

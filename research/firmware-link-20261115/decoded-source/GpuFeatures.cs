// Reconstructed analysis view; strings decoded statically; unresolved delegates remain.
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Define;
using MyControlCenter;
using MyECIO;
using Utility;

namespace GCUService.MyFan.Overclocking;

internal class GpuFeatures
{
	[CompilerGenerated]
	private sealed class <>c__DisplayClass23_0
	{
		public GpuFeatures <>4__this;

		public uint G;

		public <>c__DisplayClass23_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		internal void <SetNVPStateThread>b__0()
		{
			<>4__this.SetNVPState(G);
		}

		internal void <SetNVPStateThread>b__1()
		{
			<>4__this.SetNVPState(G);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <SetNVPStateThread>d__23 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public GpuFeatures <>4__this;

		public uint G;

		private <>c__DisplayClass23_0 <>8__1;

		private TaskAwaiter <>u__1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			int num = <>1__state;
			GpuFeatures gpuFeatures = <>4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = <>u__1;
					<>u__1 = default;
					num = (<>1__state = -1);
					goto IL_00d3;
				}
				<>8__1 = new <>c__DisplayClass23_0();
				<>8__1.<>4__this = <>4__this;
				<>8__1.G = G;
				if (ResolvedDelegates.System_Threading_SemaphoreSlim_get_CurrentCount__509(gpuFeatures.FansemporeSlim) != 0)
				{
					awaiter = gpuFeatures.FansemporeSlim.WaitAsync().GetAwaiter();
					if (!ResolvedDelegates.System_Runtime_CompilerServices_TaskAwaiter_get_IsCompleted__24(ref awaiter))
					{
						num = (<>1__state = 0);
						<>u__1 = awaiter;
						<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_00d3;
				}
				gpuFeatures.LastCommand = <>8__1.G;
				goto end_IL_000e;
				IL_00d3:
				awaiter.GetResult();
				try
				{
					ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(gpuFeatures.stopwatch);
					ResolvedDelegates.System_Diagnostics_Stopwatch_Start__153(gpuFeatures.stopwatch);
					ResolvedDelegates.System_Threading_Thread_Start__178(new Thread(() =>
					{
						<>8__1.<>4__this.SetNVPState(<>8__1.G);
					}));
					ResolvedDelegates.System_Diagnostics_Stopwatch_Stop__164(gpuFeatures.stopwatch);
					ResolvedDelegates.System_Diagnostics_Stopwatch_get_ElapsedMilliseconds__155(gpuFeatures.stopwatch);
				}
				catch
				{
				}
				finally
				{
					if (num < 0)
					{
						ResolvedDelegates.System_Threading_SemaphoreSlim_Release__182(gpuFeatures.FansemporeSlim);
						if (gpuFeatures.LastCommand != 0)
						{
							ResolvedDelegates.System_Threading_Thread_Start__178(new Thread(() =>
							{
								<>8__1.<>4__this.SetNVPState(<>8__1.G);
							}));
							gpuFeatures.LastCommand = 0u;
						}
					}
				}
				end_IL_000e:;
			}
			catch (Exception ex)
			{
				<>1__state = -2;
				<>8__1 = null;
				ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_SetException__194(ref <>t__builder, ex);
				return;
			}
			<>1__state = -2;
			<>8__1 = null;
			ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_SetResult__195(ref <>t__builder);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_SetStateMachine__196(ref <>t__builder, stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	private GPUInfo cGpuInfo = new GPUInfo();

	private GpuThermalLimitInfo GpuThermalLimitInfo = new GpuThermalLimitInfo();

	private const int GpuCoreClockOffsetMaximum = 250;

	private const int GpuCoreClockOffsetMinimum = 0;

	private const int GpuMemoryClockOffsetMaximum = 2000;

	private const int GpuMemoryClockOffsetMinimum = -2000;

	private bool m_GpuWhisperModeSupport;

	private bool apihandler;

	private object _PStateHandler = new object();

	private bool _PStateWorker = true;

	private Stopwatch sw = new Stopwatch();

	private Stopwatch pressSw = new Stopwatch();

	private int _SateDelay = 5000;

	private bool PressLock = true;

	private uint PressG;

	private uint LastG;

	private SemaphoreSlim FansemporeSlim = new SemaphoreSlim(1, 1);

	private Stopwatch stopwatch = new Stopwatch();

	private uint LastCommand;

	[MethodImpl(MethodImplOptions.NoInlining)]
	public GpuFeatures()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Init()
	{
		ResolvedDelegates.System_Threading_Thread_Sleep__6(5000);
		cGpuInfo.GetGpuHandle();
		apihandler = true;
		LoadGpuThermalLimitInfo(ref GpuThermalLimitInfo.DefaultTemperature, ref GpuThermalLimitInfo.MaximumTemperature, ref GpuThermalLimitInfo.MinimumTemperature);
		m_GpuWhisperModeSupport = cGpuInfo.IsGpuWhisperMode2p0Support();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool SetNVPState(uint G)
	{
		PressG = G;
		if (PressLock)
		{
			PressLock = false;
			ResolvedDelegates.System_Diagnostics_Stopwatch_Restart__163(pressSw);
			if (LastG != G)
			{
				SetPStateFunction(G);
			}
			Task.Run([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				ResolvedDelegates.System_Threading_Thread_Sleep__6(_SateDelay);
			}).ContinueWith([MethodImpl(MethodImplOptions.NoInlining)] (Task task) =>
			{
				ResolvedDelegates.System_Diagnostics_Stopwatch_Stop__164(pressSw);
				PressLock = true;
				if (LastG != PressG)
				{
					SetNVPState(PressG);
				}
			});
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<SetNVPStateThread>d__23))]
	private void SetNVPStateThread(uint G)
	{
		<SetNVPStateThread>d__23 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine.G = G;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetPStateFunction(uint G)
	{
		uint num = 0u;
		uint count = 0u;
		uint GPUPstate = 0u;
		LastG = G;
		switch (G)
		{
		case 1u:
		case 4u:
			GPUPstate = 0u;
			break;
		case 2u:
			GPUPstate = 3u;
			break;
		case 3u:
			GPUPstate = 5u;
			break;
		}
		try
		{
			GPUTypeClass.GetNVGpuCount(ref count);
			for (num = 0u; num < count; num++)
			{
				if (!Monitor.TryEnter(_PStateHandler, 100))
				{
					continue;
				}
				try
				{
					if (_PStateWorker)
					{
						ResolvedDelegates.System_Diagnostics_Stopwatch_Restart__163(sw);
						if (GPUTypeClass.SetNVPState(num, ref GPUPstate))
						{
							LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__120("Set GPU ", ResolvedDelegates.System_UInt32_ToString__78(ref num), ", P state: ", ResolvedDelegates.System_UInt32_ToString__78(ref GPUPstate)), "SetPStateFunction", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 211);
						}
						else
						{
							LogCtrl.TraceMessage("Set GPU " + ResolvedDelegates.System_UInt32_ToString__78(ref num) + ", P state: " + ResolvedDelegates.System_UInt32_ToString__78(ref GPUPstate) + " fail", "SetPStateFunction", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 215);
						}
						sw.Stop();
					}
				}
				finally
				{
					if (ResolvedDelegates.System_Diagnostics_Stopwatch_get_ElapsedMilliseconds__155(sw) > 10000)
					{
						_PStateWorker = false;
					}
					Monitor.Exit(_PStateHandler);
				}
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__79(new string[6]
			{
				"Set GPU ",
				ResolvedDelegates.System_UInt32_ToString__78(ref num),
				", P state: ",
				ResolvedDelegates.System_UInt32_ToString__78(ref GPUPstate),
				", exception: ",
				ResolvedDelegates.System_Exception_get_Message__58(ex)
			}), "SetPStateFunction", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 240);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGpuCoreFreqOffsetValue(int value)
	{
		try
		{
			if (apihandler)
			{
				if (value >= 0 && value <= 250)
				{
					LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Value: ", ResolvedDelegates.System_Int32_ToString__55(ref value)), "SetGpuCoreFreqOffsetValue", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 258);
					cGpuInfo.SetGpuCoreFreqOffsetValue(value);
				}
				else
				{
					LogCtrl.TraceMessage("Set the value failed, and error is out of range.", "SetGpuCoreFreqOffsetValue", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 263);
				}
			}
		}
		catch (Exception)
		{
			LogCtrl.TraceMessage("GpuInfo  SetGpuCoreFreqOffset failed, and error is out of range.", "SetGpuCoreFreqOffsetValue", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 268);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGpuMemFreqOffsetValue(int value)
	{
		try
		{
			if (apihandler)
			{
				if (value >= -2000 && value <= 2000)
				{
					LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Value: ", ResolvedDelegates.System_Int32_ToString__55(ref value)), "SetGpuMemFreqOffsetValue", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 287);
					cGpuInfo.SetGpuMemFreqOffsetValue(value);
				}
				else
				{
					LogCtrl.TraceMessage("Set the value failed, and error is out of range.", "SetGpuMemFreqOffsetValue", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 292);
				}
			}
		}
		catch (Exception)
		{
			LogCtrl.TraceMessage("GpuInfo  SetGpuMemFreqOffset failed, and error is out of range.", "SetGpuMemFreqOffsetValue", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 297);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public GpuThermalLimitInfo GetGpuThermalLimitInfo()
	{
		return GpuThermalLimitInfo;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGpuTargetTemperature(int value)
	{
		if (value >= GpuThermalLimitInfo.MinimumTemperature && value <= GpuThermalLimitInfo.MaximumTemperature)
		{
			cGpuInfo.SetGpuTargetTemperatureValue(value);
		}
		else
		{
			LogCtrl.TraceMessage("Set the value failed, and error is out of range.", "SetGpuTargetTemperature", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 342);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void LoadGpuThermalLimitInfo(ref int DefaultTemperature, ref int MaximumTemperature, ref int MinimumTemperature)
	{
		try
		{
			uint LimitDefault = 0u;
			uint LimitMax = 0u;
			uint LimitMin = 0u;
			uint CurrentValue = 0u;
			cGpuInfo.GetGpuThermalLimitInfo(ref LimitDefault, ref LimitMax, ref LimitMin, ref CurrentValue);
			DefaultTemperature = ResolvedDelegates.System_Convert_ToInt32__508(LimitDefault);
			MaximumTemperature = ResolvedDelegates.System_Convert_ToInt32__508(LimitMax);
			MinimumTemperature = ResolvedDelegates.System_Convert_ToInt32__508(LimitMin);
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception:", ResolvedDelegates.System_Object_ToString__1(ex)), "LoadGpuThermalLimitInfo", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 382);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGpuConfigurableTgpFunCtrlEnable(int status)
	{
		byte Data = 0;
		byte b = 251;
		byte b2 = 0;
		byte b3 = 0;
		b2 = (byte)((status == 1) ? 4 : 0);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1859, ref Data);
		b3 = (byte)((Data & b) + b2);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1859, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGpuConfigurableTGPTarget(int value, int _SmartApcTableDefaultTGP)
	{
		int num = 0;
		if (value >= _SmartApcTableDefaultTGP)
		{
			num = value - _SmartApcTableDefaultTGP;
			if (num >= 0)
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1860, ResolvedDelegates.System_Convert_ToByte__75(num));
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGpuDynamicBoostFunCtrlEnable(int status)
	{
		byte Data = 0;
		byte b = 254;
		byte b2 = 0;
		byte b3 = 0;
		b2 = 0;
		b2 = ((status == 1) ? ((byte)1) : ((byte)0));
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1859, ref Data);
		b3 = (byte)((Data & b) + b2);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1859, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGpuDynamicBoostEnable(int status)
	{
		byte Data = 0;
		byte b = 253;
		byte b2 = 0;
		byte b3 = 0;
		b2 = (byte)((status == 1) ? 2 : 0);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1859, ref Data);
		b3 = (byte)((Data & b) + b2);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1859, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGpuDynamicBoostTotalProcessingPowerTarget(int value)
	{
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1861, ResolvedDelegates.System_Convert_ToByte__75(value));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGpuDynamicBoostMaxinumTGP(int value)
	{
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1862, ResolvedDelegates.System_Convert_ToByte__75(value));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte ConvertToByte(BitArray bits)
	{
		ResolvedDelegates.System_Collections_BitArray_get_Count__447(bits);
		_ = 8;
		byte[] array = new byte[1];
		ResolvedDelegates.System_Collections_BitArray_CopyTo__448(bits, array, 0);
		return array[0];
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetSuperPerformance(bool enable)
	{
		byte Data = 0;
		MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1832, ref Data);
		BitArray bitArray = new BitArray(new byte[1] { Data });
		ResolvedDelegates.System_Collections_BitArray_set_Item__446(bitArray, 1, enable);
		byte data = ConvertToByte(bitArray);
		MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1832, data);
		object obj = ResolvedDelegates.System_String_Concat__120;
		string text = "SetSuperPerformance========ADDR_AP_OEM_BYTEB ";
		ushort num = 1832;
		LogCtrl.TraceMessage(obj(text, ResolvedDelegates.System_UInt16_ToString__169(ref num), "bit1 \uff1a", ResolvedDelegates.System_Byte_ToString__119(ref Data)), "SetSuperPerformance", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\Overclocking\\GpuFeatures.cs", 514);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsGpuWhisperMode2p0Support()
	{
		return m_GpuWhisperModeSupport;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuWhisperModeStatus(bool status)
	{
		if (!apihandler)
		{
			return true;
		}
		return cGpuInfo.SetGpuWhisperModeStatus(status);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuWhisperModeSettingValue(int value)
	{
		if (!apihandler)
		{
			return true;
		}
		return cGpuInfo.SetGpuWhisperModeSettingValue(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool SetGpuWhisperModeMinFpsValue(int value)
	{
		if (!apihandler)
		{
			return true;
		}
		return cGpuInfo.SetGpuWhisperModeMinFpsValue(value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGpuWhisperModeMainSwitch(int status)
	{
		byte Data = 0;
		byte b = 159;
		byte b2 = 0;
		byte b3 = 0;
		if (status == 1)
		{
			b2 = 96;
		}
		else
		{
			SetWhisperModeStatusDisable();
			b2 = 64;
		}
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, ref Data);
		b3 = (byte)((Data & b) + b2);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetWhisperModeStatusDisable()
	{
		byte Data = 0;
		byte b = 252;
		byte b2 = 0;
		byte b3 = 0;
		b2 = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1990, ref Data);
		b3 = (byte)((Data & b) + b2);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1990, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetWisperModeStatus()
	{
		byte Data = 0;
		byte b = 3;
		byte b2 = 0;
		int result = -1;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1990, ref Data);
		switch ((byte)(Data & b))
		{
		case 0:
			result = -1;
			break;
		case 1:
			result = 2;
			break;
		case 2:
			result = 1;
			break;
		case 3:
			result = 0;
			break;
		}
		return result;
	}
}

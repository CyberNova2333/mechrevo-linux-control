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
using Microsoft.Win32;
using MyControlCenter;
using Utility;

namespace MyECIO;

public class MyEcCtrl
{
	[CompilerGenerated]
	private sealed class <>c__DisplayClass46_0
	{
		[StructLayout(LayoutKind.Auto)]
		private struct <<Set_APExistToEC>b__0>d : IAsyncStateMachine
		{
			public int <>1__state;

			public AsyncTaskMethodBuilder <>t__builder;

			public <>c__DisplayClass46_0 <>4__this;

			private TaskAwaiter <>u__1;

			[MethodImpl(MethodImplOptions.NoInlining)]
			private void MoveNext()
			{
				int num = <>1__state;
				<>c__DisplayClass46_0 obj = <>4__this;
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						awaiter = ResolvedDelegates.System_Threading_Tasks_Task_GetAwaiter__23(ResolvedDelegates.System_Threading_Tasks_Task_Delay__22(100));
						if (!ResolvedDelegates.System_Runtime_CompilerServices_TaskAwaiter_get_IsCompleted__24(ref awaiter))
						{
							num = (<>1__state = 0);
							<>u__1 = awaiter;
							<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = <>u__1;
						<>u__1 = default;
						num = (<>1__state = -1);
					}
					awaiter.GetResult();
					obj.<>4__this.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(obj.<>4__this)), 1857, ref obj.data);
					LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("Read APExist ECSpec.ADDR_AP_OEM_BYTE:", ResolvedDelegates.System_Byte_ToString__119(ref obj.data)), "Set_APExistToEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\MyEcCtrl.cs", 599);
				}
				catch (Exception ex)
				{
					<>1__state = -2;
					ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_SetException__26(ref <>t__builder, ex);
					return;
				}
				<>1__state = -2;
				ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_SetResult__27(ref <>t__builder);
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
				ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_SetStateMachine__28(ref <>t__builder, stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		public MyEcCtrl <>4__this;

		public byte data;

		public <>c__DisplayClass46_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		[AsyncStateMachine(typeof(<<Set_APExistToEC>b__0>d))]
		internal Task <Set_APExistToEC>b__0()
		{
			<<Set_APExistToEC>b__0>d stateMachine = default;
			stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_Create__20();
			stateMachine.<>4__this = this;
			stateMachine.<>1__state = -1;
			stateMachine.<>t__builder.Start(ref stateMachine);
			return ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_get_Task__21(ref stateMachine.<>t__builder);
		}
	}

	private static readonly MyEcCtrl EcCtrl = new MyEcCtrl();

	private AcpiCtrl AcpiModel;

	private int _cachedNvGpu = -1;

	public static MyEcCtrl Instance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return EcCtrl;
		}
	}

	public bool IsUMA
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (_cachedNvGpu == -1)
			{
				_cachedNvGpu = GetNvGpu();
			}
			return _cachedNvGpu == 0;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public MyEcCtrl()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
		AcpiModel = AcpiCtrl.Instance;
	}

	public void LoadDrv()
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void SetHWOC(string AcpiName, int SetValue, ref byte outputdata)
	{
		AcpiModel?.CustomRW(AcpiName, SetValue, ref outputdata);
	}

	public void UnloadDrv()
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Read(string ReadName, ushort Addr, ref byte Data)
	{
		AcpiModel?.Read(ReadName, Addr, ref Data);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Write(string ReadName, ushort Addr, byte Data)
	{
		AcpiModel?.Write(ReadName, Addr, Data);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsChinaMode()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1894, ref Data);
		LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("&^^|Read:", ResolvedDelegates.System_Byte_ToString__119(ref Data)), "IsChinaMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\MyEcCtrl.cs", 104);
		return ResolvedDelegates.System_Convert_ToInt32__220(ResolvedDelegates.System_Collections_BitArray_get_Item__219(new BitArray(new byte[1] { Data }), 4)) == 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsChinaModeOn()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1894, ref Data);
		LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("&^^|Read:", ResolvedDelegates.System_Byte_ToString__119(ref Data)), "IsChinaModeOn", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\MyEcCtrl.cs", 114);
		BitArray bitArray = new BitArray(new byte[1] { Data });
		if (ResolvedDelegates.System_Convert_ToInt32__220(ResolvedDelegates.System_Collections_BitArray_get_Item__219(bitArray, 4)) == 1)
		{
			return true;
		}
		if (Convert.ToInt32(bitArray[5]) == 1)
		{
			return true;
		}
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1922, ref Data);
		return ResolvedDelegates.System_Convert_ToInt32__220(ResolvedDelegates.System_Collections_BitArray_get_Item__219(new BitArray(new byte[1] { Data }), 6)) == 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void ReadPciDword(byte Bus, byte Dev, byte Func, uint Offset, ref uint pData)
	{
		AcpiModel?.ReadPciDword(Bus, Dev, Func, Offset, ref pData);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetProjectIdFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1856, ref Data);
		return (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xFF);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetSystemIdFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1110, ref Data);
		return (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0x80);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetnModuleId_GPUFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2002, ref Data);
		return (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0x1F);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetnModuleIdFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2003, ref Data);
		return (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 1);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetnModule2IdFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2003, ref Data);
		return (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xF0);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetROMIdFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1905, ref Data);
		return (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 1);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetROMId2FromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1906, ref Data);
		return (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xFF);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetAdapterWattFromEC()
	{
		int result = 150;
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1183, ref Data);
		switch ((byte)(Data & 0x78))
		{
		case 0:
			result = 330;
			break;
		case 8:
			result = 230;
			break;
		case 16:
			result = 180;
			break;
		case 24:
			result = 150;
			break;
		case 32:
			result = 120;
			break;
		case 40:
			result = 90;
			break;
		case 48:
			result = 65;
			break;
		case 56:
			result = 40;
			break;
		case 64:
			result = 280;
			break;
		case 72:
			result = 210;
			break;
		case 80:
			result = 250;
			break;
		case 88:
			result = 420;
			break;
		case 96:
			result = 240;
			break;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetTypeCsatusFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1996, ref Data);
		return (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0x36);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetRoundSocketStatusFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1996, ref Data);
		if ((Data & 1) == 0)
		{
			bool flag = (Data & 0xF) == 0;
			Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1168, ref Data);
			if (!flag)
			{
				return 0;
			}
			return Data & 1;
		}
		return Data & 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetMicStructType()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3406, ref Data);
		return (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xC0);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetPowerFromTypeC()
	{
		byte Data = 0;
		byte Data2 = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1997, ref Data);
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1998, ref Data2);
		if ((int)ResolvedDelegates.System_Convert_ToUInt32__161(Data) < (int)ResolvedDelegates.System_Convert_ToUInt32__161(Data2))
		{
			return (int)ResolvedDelegates.System_Convert_ToUInt32__161(Data2);
		}
		return (int)Convert.ToUInt32(Data);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int isSupportRGBLBFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1934, ref Data);
		if ((byte)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0x10) == 16)
		{
			return 1;
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetDBType()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2002, ref Data);
		if (Data == 0 || Data == byte.MaxValue)
		{
			return 0;
		}
		Data &= 0x7F;
		if (Data == 22 || Data == 23 || Data == 24)
		{
			LogCtrl.TraceMessageProgramData("GetDBType: BD2, " + ResolvedDelegates.System_Byte_ToString__119(ref Data), "GetDBType", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\MyEcCtrl.cs", 347);
			return 1;
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetNvGpu()
	{
		int num = 0;
		if ((int)RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\Support", "APVersionCheck", 0) >= 25)
		{
			num = ((NvramVariable.GetFwVarsGPU() > 0) ? 1 : 0);
		}
		else
		{
			byte Data = 0;
			Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2002, ref Data);
			if ((Data & 0x1F) > 0)
			{
				num = 1;
			}
			else
			{
				byte Data2 = 0;
				Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1834, ref Data2);
				num = (((Data2 & 2) > 0) ? 1 : 0);
			}
			if (num == 1)
			{
				BIOS_PROJECT_ID biosProjctID = EcCtrl.GetBiosProjctID();
				if (biosProjctID == BIOS_PROJECT_ID.IDI || biosProjctID == BIOS_PROJECT_ID.IDL || biosProjctID == BIOS_PROJECT_ID.IDG)
				{
					num = 0;
				}
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetSmartBalanceSupportFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1964, ref Data);
		return Data & 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool GetTouchPadLedStatusFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1958, ref Data);
		return UtilityExtensions.GetBitFromByte(Data, 3) == 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsSupportLiquidCooling()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1956, ref Data);
		return UtilityExtensions.GetBitFromByte(Data, 7) == 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsSupportLC_HWOC_ADL()
	{
		Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1801, 5);
		ResolvedDelegates.System_Threading_Thread_Sleep__6(50);
		Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1800, 1);
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1800, ref Data);
		int num = 0;
		bool flag = true;
		while (flag)
		{
			Thread.Sleep(20);
			if (UtilityExtensions.GetBitFromByte(Data, 1) == 1)
			{
				flag = false;
			}
			num++;
			if (num >= 10)
			{
				flag = false;
				return false;
			}
		}
		byte Data2 = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1816, ref Data2);
		if (ResolvedDelegates.System_Convert_ToInt32__179(Data2) != 255 && ResolvedDelegates.System_Convert_ToInt32__179(Data2) > 0)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsSupportLiquidCoolingHWOC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3400, ref Data);
		if (ResolvedDelegates.System_Convert_ToInt32__179(Data) != 255 && ResolvedDelegates.System_Convert_ToInt32__179(Data) > 0)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsSupportAntiOC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1994, ref Data);
		return UtilityExtensions.GetBitFromByte(Data, 3) == 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsSupportRamFan1p5()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1934, ref Data);
		return UtilityExtensions.GetBitFromByte(Data, 6) == 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsSupportLCFanTable()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1992, ref Data);
		return UtilityExtensions.GetBitFromByte(Data, 5) == 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetTurboModeSupport()
	{
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1183, ref Data);
		return (UtilityExtensions.GetBitFromByte(Data, 1) == 1) ? 1 : 0;
	}

	public bool GetTypeCAdaptorPrioritySupport()
	{
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool GetOTASupport()
	{
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3397, ref Data);
		return UtilityExtensions.GetBitFromByte(Data, 6) == 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool GetThermalProtectStatusFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1992, ref Data);
		return UtilityExtensions.GetBitFromByte(Data, 4) == 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetGPUCoreFreqFromEC()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3403, ref Data);
		return (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xFF);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetGPUMemFreqFromEC()
	{
		int num = 0;
		int num2 = 0;
		byte Data = 0;
		byte Data2 = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3405, ref Data);
		num = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0x7F);
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3404, ref Data2);
		num2 = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data2) & 0xFF);
		return 256 * num + num2;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetCustomModetoEC(bool CMode)
	{
		if (CMode)
		{
			Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1798, 65);
		}
		else
		{
			Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1798, 64);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Set_APExistToEC(bool bExist)
	{
		<>c__DisplayClass46_0 CS$<>8__locals5 = new <>c__DisplayClass46_0();
		CS$<>8__locals5.<>4__this = this;
		CS$<>8__locals5.data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1857, ref CS$<>8__locals5.data);
		byte b = (byte)ResolvedDelegates.System_Convert_ToUInt64__129(CS$<>8__locals5.data);
		Write(Data: (!bExist) ? ((byte)(b & 0xFE)) : ((byte)(b | 1)), ReadName: ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), Addr: 1857);
		LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("[Application_Startup] Set_APExistToEC APExist :", ResolvedDelegates.System_Boolean_ToString__147(ref bExist)), "Set_APExistToEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\MyEcCtrl.cs", 593);
		ResolvedDelegates.System_Threading_Tasks_Task_Run__11([MethodImpl(MethodImplOptions.NoInlining)] [AsyncStateMachine(typeof(<>c__DisplayClass46_0.<<Set_APExistToEC>b__0>d))] () =>
		{
			<>c__DisplayClass46_0.<<Set_APExistToEC>b__0>d stateMachine = default;
			stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_Create__20();
			stateMachine.<>4__this = CS$<>8__locals5;
			stateMachine.<>1__state = -1;
			stateMachine.<>t__builder.Start(ref stateMachine);
			return ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_get_Task__21(ref stateMachine.<>t__builder);
		});
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsSupport6BitID()
	{
		byte Data = 0;
		Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1994, ref Data);
		return UtilityExtensions.GetBitFromByte(Data, 0) == 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public BIOS_PROJECT_ID GetBiosProjctID()
	{
		BIOS_PROJECT_ID bIOS_PROJECT_ID = BIOS_PROJECT_ID.NA;
		byte Data = 0;
		byte b = 0;
		AcpiModel.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1869, ref Data);
		if (Data > 0)
		{
			b = Data;
		}
		else
		{
			byte b2 = 15;
			if (IsSupport6BitID())
			{
				b2 = 63;
			}
			AcpiModel.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1868, ref Data);
			b = (byte)(Data & b2);
		}
		return b switch
		{
			0 => BIOS_PROJECT_ID.IDR, 
			1 => BIOS_PROJECT_ID.IDX, 
			2 => BIOS_PROJECT_ID.IDV, 
			3 => BIOS_PROJECT_ID.IDO, 
			4 => BIOS_PROJECT_ID.IDP, 
			5 => BIOS_PROJECT_ID.IDS, 
			6 => BIOS_PROJECT_ID.IDY, 
			7 => BIOS_PROJECT_ID.IDW, 
			8 => BIOS_PROJECT_ID.IDM, 
			9 => BIOS_PROJECT_ID.IDE, 
			10 => BIOS_PROJECT_ID.IDN, 
			11 => BIOS_PROJECT_ID.IDZ, 
			12 => BIOS_PROJECT_ID.IDXQ, 
			13 => BIOS_PROJECT_ID.IDQ, 
			14 => BIOS_PROJECT_ID.IDO3, 
			15 => BIOS_PROJECT_ID.ID1, 
			16 => BIOS_PROJECT_ID.ID2, 
			17 => BIOS_PROJECT_ID.ID3, 
			18 => BIOS_PROJECT_ID.IDG, 
			19 => BIOS_PROJECT_ID.IDJ, 
			20 => BIOS_PROJECT_ID.IDL, 
			21 => BIOS_PROJECT_ID.IDA, 
			22 => BIOS_PROJECT_ID.IDB, 
			23 => BIOS_PROJECT_ID.IDU, 
			24 => BIOS_PROJECT_ID.IDC, 
			25 => BIOS_PROJECT_ID.IDF, 
			26 => BIOS_PROJECT_ID.IDK, 
			27 => BIOS_PROJECT_ID.ID5, 
			28 => BIOS_PROJECT_ID.ID55, 
			29 => BIOS_PROJECT_ID.ID6, 
			30 => BIOS_PROJECT_ID.IDI, 
			31 => BIOS_PROJECT_ID.ID65, 
			32 => BIOS_PROJECT_ID.ID1New, 
			33 => BIOS_PROJECT_ID.ID9, 
			34 => BIOS_PROJECT_ID.IDH, 
			35 => BIOS_PROJECT_ID.IDR2, 
			36 => BIOS_PROJECT_ID.ID7, 
			37 => BIOS_PROJECT_ID.IDD, 
			38 => BIOS_PROJECT_ID.IDM5N, 
			39 => BIOS_PROJECT_ID.IDX6J, 
			40 => BIOS_PROJECT_ID.IDX5H, 
			64 => BIOS_PROJECT_ID.IDT6M, 
			65 => BIOS_PROJECT_ID.IDT6Z, 
			66 => BIOS_PROJECT_ID.IDS6J, 
			67 => BIOS_PROJECT_ID.IDU4T, 
			68 => BIOS_PROJECT_ID.IDU4D, 
			69 => BIOS_PROJECT_ID.IDBX1, 
			70 => BIOS_PROJECT_ID.IDS61, 
			71 => BIOS_PROJECT_ID.IDU4V, 
			72 => BIOS_PROJECT_ID.IDU6V, 
			73 => BIOS_PROJECT_ID.IDU5G, 
			_ => BIOS_PROJECT_ID.NA, 
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public GN20_GPU_SKU GetSku()
	{
		GN20_GPU_SKU result = GN20_GPU_SKU.NA;
		byte Data = 0;
		byte b = 240;
		byte b2 = 0;
		AcpiModel.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2003, ref Data);
		switch ((byte)(Data & b))
		{
		case 48:
			result = GN20_GPU_SKU.E3;
			break;
		case 64:
			result = GN20_GPU_SKU.E4;
			break;
		case 80:
			result = GN20_GPU_SKU.E5;
			break;
		case 96:
			result = GN20_GPU_SKU.MaxQ;
			break;
		case 112:
			result = GN20_GPU_SKU.E7;
			break;
		case 128:
			result = GN20_GPU_SKU.P0;
			break;
		case 144:
			result = GN20_GPU_SKU.P1;
			break;
		case 160:
			result = GN20_GPU_SKU.E6;
			break;
		case 176:
			result = GN20_GPU_SKU.E8;
			break;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsHeroProject()
	{
		GN20_GPU_SKU sku = EcCtrl.GetSku();
		BIOS_PROJECT_ID biosProjctID = EcCtrl.GetBiosProjctID();
		int projectIdFromEC = EcCtrl.GetProjectIdFromEC();
		if (sku == GN20_GPU_SKU.MaxQ && biosProjctID == BIOS_PROJECT_ID.IDP)
		{
			return true;
		}
		if (sku == GN20_GPU_SKU.MaxQ && projectIdFromEC == 22)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsAMDPlatform()
	{
		byte bus = 0;
		byte dev = 0;
		byte func = 0;
		uint offset = 0u;
		uint pData = 0u;
		EcCtrl.ReadPciDword(bus, dev, func, offset, ref pData);
		if ((pData & 0xFFFF) == 4130)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetProject2ExID(int m_ProjectID)
	{
		if (m_ProjectID == 23 || m_ProjectID == 24)
		{
			int systemIdFromEC = GetSystemIdFromEC();
			int num = GetnModuleId_GPUFromEC();
			int num2 = GetnModuleIdFromEC();
			int num3 = GetnModule2IdFromEC();
			int rOMIdFromEC = GetROMIdFromEC();
			int rOMId2FromEC = GetROMId2FromEC();
			int adapterWattFromEC = GetAdapterWattFromEC();
			switch (m_ProjectID)
			{
			case 23:
				if (systemIdFromEC == 0 && rOMIdFromEC == 0)
				{
					switch (num2)
					{
					case 0:
						m_ProjectID = 5889;
						break;
					case 1:
						m_ProjectID = 5890;
						if (rOMId2FromEC == 3)
						{
							m_ProjectID = 5895;
						}
						break;
					}
				}
				else if (systemIdFromEC == 128 && rOMIdFromEC == 0)
				{
					m_ProjectID = 5891;
				}
				else if (systemIdFromEC == 0 && rOMIdFromEC == 1)
				{
					if (num2 == 0)
					{
						m_ProjectID = 5893;
					}
				}
				else if (systemIdFromEC == 128 && rOMIdFromEC == 1)
				{
					m_ProjectID = ((num3 != 48 && num3 != 112 && num3 != 160) ? 5892 : 5894);
				}
				break;
			case 24:
				if (systemIdFromEC == 0 && rOMIdFromEC == 0)
				{
					switch (num2)
					{
					case 0:
						m_ProjectID = 6145;
						break;
					case 1:
						m_ProjectID = 6146;
						break;
					}
				}
				else if (systemIdFromEC == 128 && rOMIdFromEC == 0)
				{
					m_ProjectID = rOMId2FromEC switch
					{
						1 => 6147, 
						4 => 6154, 
						_ => 6148, 
					};
				}
				else if (systemIdFromEC == 0 && rOMIdFromEC == 1)
				{
					m_ProjectID = 6149;
				}
				else if (systemIdFromEC == 128 && rOMIdFromEC == 1)
				{
					switch (num)
					{
					case 8:
						m_ProjectID = 6150;
						break;
					case 16:
					case 17:
					case 21:
						m_ProjectID = ((adapterWattFromEC != 150) ? 6151 : 6155);
						break;
					case 12:
						m_ProjectID = ((adapterWattFromEC != 150) ? 6152 : 6156);
						break;
					case 18:
						m_ProjectID = ((adapterWattFromEC != 150) ? 6153 : 6157);
						break;
					}
				}
				break;
			}
		}
		return m_ProjectID;
	}
}

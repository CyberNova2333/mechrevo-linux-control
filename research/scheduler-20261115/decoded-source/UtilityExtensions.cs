// Static analysis rendering; not original buildable source.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using MyControlCenter;

namespace Utility;

internal class UtilityExtensions
{
	[CompilerGenerated]
	private sealed class <>c__DisplayClass0_0
	{
		public uint currentMode;

		public <>c__DisplayClass0_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		internal bool <ModeSwitchClockWise>b__0(uint n)
		{
			return ResolvedDelegates.System_UInt32_Equals__270(ref n, currentMode);
		}

		internal bool <ModeSwitchClockWise>b__1(uint n)
		{
			return ResolvedDelegates.System_UInt32_Equals__270(ref n, currentMode);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static uint ModeSwitchClockWise(uint currentMode, bool clockwise)
	{
		<>c__DisplayClass0_0 CS$<>8__locals4 = new <>c__DisplayClass0_0();
		CS$<>8__locals4.currentMode = currentMode;
		List<uint> list = new List<uint> { 0u, 1u, 2u };
		uint num = CS$<>8__locals4.currentMode;
		if (clockwise)
		{
			int num2 = list.FindIndex((uint n) => ResolvedDelegates.System_UInt32_Equals__270(ref n, CS$<>8__locals4.currentMode));
			num2++;
			num2 = ((num2 <= 2) ? num2 : 0);
			return list[num2];
		}
		int num3 = list.FindIndex((uint n) => ResolvedDelegates.System_UInt32_Equals__270(ref n, CS$<>8__locals4.currentMode));
		num3--;
		num3 = ((num3 < 0) ? 2 : num3);
		return list[num3];
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static int GetBitFromByte(byte data, int num)
	{
		return (data >> num) & 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static byte[] Combine(byte[] first, byte[] second)
	{
		byte[] array = new byte[first.Length + second.Length];
		ResolvedDelegates.System_Buffer_BlockCopy__268(first, 0, array, 0, first.Length);
		ResolvedDelegates.System_Buffer_BlockCopy__268(second, 0, array, first.Length, second.Length);
		return array;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static T BytesToStructure<T>(byte[] bytes)
	{
		int num = ResolvedDelegates.System_Runtime_InteropServices_Marshal_SizeOf__114(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(T).TypeHandle));
		if (bytes.Length < num)
		{
			throw new Exception("Invalid parameter");
		}
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		try
		{
			ResolvedDelegates.System_Runtime_InteropServices_Marshal_Copy__214(bytes, 0, intPtr, num);
			return (T)ResolvedDelegates.System_Runtime_InteropServices_Marshal_PtrToStructure__239(intPtr, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(T).TypeHandle));
		}
		finally
		{
			ResolvedDelegates.System_Runtime_InteropServices_Marshal_FreeHGlobal__90(intPtr);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe static bool IsNvGpu()
	{
		try
		{
			DateTime dateTime;
			if (ResolvedDelegates.System_String_Contains__108(ref *(string*)new HardwareInfoCollect().getGraphicInfo(), "NVIDIA"))
			{
				dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "IsNvGpu()----------------------true"));
				return true;
			}
			dateTime = DateTime.Now;
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "IsNvGpu()----------------------false"));
			return false;
		}
		catch (Exception ex)
		{
			DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__107(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "IsNvGpu()----------------------:", ResolvedDelegates.System_Exception_get_Message__58(ex)));
			return false;
		}
	}

	public UtilityExtensions()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
	}
}

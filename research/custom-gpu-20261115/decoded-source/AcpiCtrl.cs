// Static analysis rendering; not original buildable source.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Microsoft.Win32;
using Utility;

namespace MyECIO;

public class AcpiCtrl
{
	private static readonly AcpiCtrl AcpiModel = new AcpiCtrl();

	public const uint GENERIC_READ = 2147483648u;

	public const uint GENERIC_WRITE = 1073741824u;

	public const uint FILE_SHARE_WRITE = 2u;

	public const uint FILE_SHARE_READ = 1u;

	public const uint FILE_FLAG_OVERLAPPED = 1073741824u;

	public const uint OPEN_EXISTING = 3u;

	public const uint OPEN_ALWAYS = 4u;

	public const uint METHOD_BUFFERED = 0u;

	public const uint METHOD_IN_DIRECT = 1u;

	public const uint METHOD_OUT_DIRECT = 2u;

	public const uint METHOD_NEITHER = 3u;

	public const uint FILE_READ_ACCESS = 1u;

	public const uint FILE_WRITE_ACCESS = 2u;

	public const uint GPD_TYPE = 40000u;

	public const byte SMRW_CMD_READ = 187;

	public const byte SMRW_CMD_WRITE = 170;

	public const uint IOCTL_GPD_READ_PORT_UCHAR = 2621465600u;

	public const uint IOCTL_GPD_READ_PORT_USHORT = 2621465604u;

	public const uint IOCTL_GPD_READ_PORT_ULONG = 2621465608u;

	public const uint IOCTL_GPD_WRITE_PORT_UCHAR = 2621482048u;

	public const uint IOCTL_GPD_WRITE_PORT_USHORT = 2621482052u;

	public const uint IOCTL_GPD_WRITE_PORT_ULONG = 2621482056u;

	public const uint IOCTL_GPD_ACPI_CMREAD = 2621482112u;

	public const uint IOCTL_GPD_ACPI_CMWRITE = 2621482116u;

	public const uint IOCTL_GPD_ACPI_ECREAD = 2621482120u;

	public const uint IOCTL_GPD_ACPI_ECWRITE = 2621482124u;

	public const uint IOCTL_GPD_ACPI_MMREADB = 2621482128u;

	public const uint IOCTL_GPD_ACPI_MMREADD = 2621482132u;

	public const uint IOCTL_GPD_ACPI_MMWRITEB = 2621482136u;

	public const uint IOCTL_GPD_ACPI_MMWRITED = 2621482140u;

	public const uint IOCTL_GPD_ACPI_PEREAD = 2621482144u;

	public const uint IOCTL_GPD_ACPI_PEWRITE = 2621482148u;

	public const uint IOCTL_GPD_ACPI_IOREAD = 2621482176u;

	public const uint IOCTL_GPD_ACPI_IOWRITE = 2621482180u;

	public const uint IOCTL_GPD_ACPI_INIOREAD = 2621482184u;

	public const uint IOCTL_GPD_ACPI_INIOWRITE = 2621482188u;

	public const uint IOCTL_GPD_ACPI_TMPREAD1 = 2621482192u;

	public const uint IOCTL_GPD_ACPI_TMPREAD2 = 2621482196u;

	public const uint IOCTL_GPD_ACPI_TMPREAD3 = 2621482200u;

	public const uint IOCTL_GPD_ACPI_TMPWRITE1 = 2621482204u;

	public const uint IOCTL_GPD_ACPI_TMPWRITE2 = 2621482208u;

	public const uint IOCTL_GPD_ACPI_TMPWRITE3 = 2621482212u;

	public const uint IOCTL_GPD_ACPI_SMAPCTABLE = 2621482240u;

	public const uint IOCTL_GPD_ACPI_CUSTOMCTL = 2621482244u;

	private Stopwatch sw = new Stopwatch();

	private int EC_TimeOut = 2000;

	private static object m_lock_ec = new object();

	private int DelayTime = 10;

	private int errorCount;

	private string m_sRegPath = "\\OEM\\GamingCenter2";

	public static AcpiCtrl Instance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return AcpiModel;
		}
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPStr)] string strName, uint nAccess, uint nShareMode, IntPtr lpSecurity, uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool CloseHandle(IntPtr hObject);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool DeviceIoControl(IntPtr handle, uint controlCode, IntPtr inBuffer, int inBufferSize, ref int outBuffer, int outBufferSize, out int bytesReturned, IntPtr overlapped);

	[DllImport("Kernel32.dll", SetLastError = true)]
	public static extern bool DeviceIoControl(IntPtr handle, uint controlCode, IntPtr inBuffer, int inBufferSize, byte[] OutBuffer, int outBufferSize, out int bytesReturned, IntPtr overlapped);

	[DllImport("ACPIDriverDll.dll", CallingConvention = CallingConvention.StdCall)]
	public static extern IntPtr SMAPCTable(IntPtr arg0);

	[MethodImpl(MethodImplOptions.NoInlining)]
	public AcpiCtrl()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "ServiceReady", 0, RegistryValueKind.DWord);
		while (!CheckAcpiDriverDeviceExists() && errorCount < 30)
		{
			Thread.Sleep(1000);
			errorCount++;
		}
		if (errorCount < 30)
		{
			LogCtrl.TraceMessage("ACPIDriverInit Ready !", ".ctor", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\AcpiCtrl.cs", 96);
			try
			{
				ResolvedDelegates.System_Diagnostics_EventLog_WriteEntry__212("GCUService", "ACPIDriver Init Ready !");
				return;
			}
			catch
			{
				return;
			}
		}
		LogCtrl.TraceMessage("ACPIDriverInit Fail !", ".ctor", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\AcpiCtrl.cs", 107);
		try
		{
			ResolvedDelegates.System_Diagnostics_EventLog_WriteEntry__212("GCUService", "ACPIDriver Init Fail !");
		}
		catch
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool CheckAcpiDriverDeviceExists()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected Obj, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected Obj, but got Unknown
		bool result = false;
		try
		{
			ManagementObjectEnumerator val = ResolvedDelegates.System_Management_ManagementObjectCollection_GetEnumerator__203(ResolvedDelegates.System_Management_ManagementObjectSearcher_Get__199((object)new ManagementObjectSearcher("SELECT * FROM Win32_PnPSignedDriver")));
			try
			{
				while (val.MoveNext())
				{
					ManagementObject val2 = (ManagementObject)val.Current;
					try
					{
						object obj = ResolvedDelegates.System_Management_ManagementBaseObject_GetPropertyValue__213(val2, "DeviceName");
						if (obj != null && (ResolvedDelegates.System_String_IsNullOrEmpty__172(ResolvedDelegates.System_Object_ToString__1(obj)) ? string.Empty : ResolvedDelegates.System_Object_ToString__1(obj)) == "ACPIDriver")
						{
							result = true;
							return result;
						}
					}
					catch (Exception)
					{
					}
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
		catch (Exception ex2)
		{
			LogCtrl.TraceMessageProgramData(ex2.ToString(), "CheckAcpiDriverDeviceExists", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\AcpiCtrl.cs", 154);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Read(uint ReadName, ushort Addr, ref byte Data)
	{
		ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(sw);
		ResolvedDelegates.System_Diagnostics_Stopwatch_Start__153(sw);
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(m_lock_ec, EC_TimeOut))
		{
			try
			{
				ReadACPI(2621482112u, Addr, ref Data);
				ResolvedDelegates.System_Threading_Thread_Sleep__6(DelayTime);
			}
			catch (Exception)
			{
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(m_lock_ec);
			}
		}
		sw.Stop();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Read(string ReadName, ushort Addr, ref byte Data)
	{
		ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(sw);
		ResolvedDelegates.System_Diagnostics_Stopwatch_Start__153(sw);
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(m_lock_ec, EC_TimeOut))
		{
			try
			{
				ReadACPI(2621482120u, Addr, ref Data);
				ResolvedDelegates.System_Threading_Thread_Sleep__6(DelayTime);
			}
			catch (Exception)
			{
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(m_lock_ec);
			}
		}
		sw.Stop();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Write(string ReadName, ushort Addr, byte Data)
	{
		ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(sw);
		ResolvedDelegates.System_Diagnostics_Stopwatch_Start__153(sw);
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(m_lock_ec, EC_TimeOut))
		{
			try
			{
				WriteACPI(2621482124u, Addr, Data);
				ResolvedDelegates.System_Threading_Thread_Sleep__6(DelayTime);
			}
			catch (Exception)
			{
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(m_lock_ec);
			}
		}
		sw.Stop();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void ReadPciDword(byte Bus, byte Dev, byte Func, uint Offset, ref uint pData)
	{
		ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(sw);
		ResolvedDelegates.System_Diagnostics_Stopwatch_Start__153(sw);
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(m_lock_ec, EC_TimeOut))
		{
			try
			{
				int data = 0;
				int addr = 0;
				ReadACPI(2621482144u, addr, ref data);
				pData = ResolvedDelegates.System_Convert_ToUInt32__143(data);
			}
			catch (Exception)
			{
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(m_lock_ec);
			}
		}
		sw.Stop();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void ReadSmartApcTableByKey(byte Offset, ref byte[] pData)
	{
		ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(sw);
		ResolvedDelegates.System_Diagnostics_Stopwatch_Start__153(sw);
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(m_lock_ec, EC_TimeOut))
		{
			try
			{
				IntPtr intPtr = CreateFile("\\\\.\\ACPIDriver", 3221225472u, 3u, IntPtr.Zero, 3u, 0u, IntPtr.Zero);
				if (ResolvedDelegates.System_IntPtr_op_Inequality__5(intPtr, IntPtr.Zero))
				{
					object obj = ResolvedDelegates.System_Runtime_InteropServices_Marshal_Copy__214;
					byte[] array = new byte[2] { 187, Offset };
					int num = 2;
					IntPtr intPtr2 = ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(num);
					obj(array, 0, intPtr2, 2);
					byte[] array2 = new byte[4] { 255, 255, 255, 255 };
					int outBufferSize = 4;
					if (!DeviceIoControl(intPtr, 2621482240u, intPtr2, num, array2, outBufferSize, out var _, IntPtr.Zero))
					{
						LogCtrl.TraceMessage("DeviceIoControl fail.", "ReadSmartApcTableByKey", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\AcpiCtrl.cs", 299);
					}
					pData = array2;
					ResolvedDelegates.System_Runtime_InteropServices_Marshal_FreeHGlobal__90(intPtr2);
					CloseHandle(intPtr);
				}
				Thread.Sleep(DelayTime);
			}
			catch (Exception ex)
			{
				LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception: ", ResolvedDelegates.System_Object_ToString__1(ex)), "ReadSmartApcTableByKey", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\AcpiCtrl.cs", 316);
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(m_lock_ec);
			}
		}
		string text = "";
		byte[] array3 = pData;
		for (int i = 0; i < array3.Length; i++)
		{
			byte b = array3[i];
			text = ResolvedDelegates.System_String_Concat__107(text, ResolvedDelegates.System_Byte_ToString__119(ref b), ",");
		}
		ResolvedDelegates.System_Diagnostics_Stopwatch_Stop__164(sw);
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__120("ReadSmartApcTable ", ResolvedDelegates.System_Byte_ToString__119(ref Offset), " ", text));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void WriteSmartApcTableByKey(byte[] Data)
	{
		try
		{
			IntPtr intPtr = CreateFile("\\\\.\\ACPIDriver", 3221225472u, 3u, IntPtr.Zero, 3u, 0u, IntPtr.Zero);
			if (ResolvedDelegates.System_IntPtr_op_Inequality__5(intPtr, IntPtr.Zero))
			{
				byte[] array = UtilityExtensions.Combine(new byte[1] { 170 }, Data);
				int num = array.Length;
				IntPtr intPtr2 = ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(num);
				ResolvedDelegates.System_Runtime_InteropServices_Marshal_Copy__214(array, 0, intPtr2, array.Length);
				byte[] outBuffer = new byte[4] { 255, 255, 255, 255 };
				int outBufferSize = 4;
				if (!DeviceIoControl(intPtr, 2621482240u, intPtr2, num, outBuffer, outBufferSize, out var _, IntPtr.Zero))
				{
					LogCtrl.TraceMessage("DeviceIoControl fail.", "WriteSmartApcTableByKey", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\AcpiCtrl.cs", 358);
				}
				Marshal.FreeHGlobal(intPtr2);
				CloseHandle(intPtr);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception: ", ResolvedDelegates.System_Object_ToString__1(ex)), "WriteSmartApcTableByKey", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\AcpiCtrl.cs", 367);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ReadACPI(uint ioctrl, int addr, ref int data)
	{
		try
		{
			IntPtr intPtr = CreateFile("\\\\.\\ACPIDriver", 3221225472u, 3u, IntPtr.Zero, 3u, 0u, IntPtr.Zero);
			if (ResolvedDelegates.System_IntPtr_op_Inequality__5(intPtr, IntPtr.Zero))
			{
				object obj = ResolvedDelegates.System_Runtime_InteropServices_Marshal_Copy__216;
				int[] array = new int[1] { addr };
				int num = 4;
				IntPtr intPtr2 = ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(num);
				obj(array, 0, intPtr2, 1);
				int outBuffer = 0;
				int outBufferSize = 4;
				DeviceIoControl(intPtr, ioctrl, intPtr2, num, ref outBuffer, outBufferSize, out var _, IntPtr.Zero);
				data = ResolvedDelegates.System_Convert_ToInt32__151(outBuffer & 0xFFFF);
				ResolvedDelegates.System_Runtime_InteropServices_Marshal_FreeHGlobal__90(intPtr2);
				CloseHandle(intPtr);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("[ReadACPI] Exception: ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ReadACPI(uint ioctrl, int addr, ref byte data)
	{
		try
		{
			IntPtr intPtr = CreateFile("\\\\.\\ACPIDriver", 3221225472u, 3u, IntPtr.Zero, 3u, 0u, IntPtr.Zero);
			if (ResolvedDelegates.System_IntPtr_op_Inequality__5(intPtr, IntPtr.Zero))
			{
				object obj = ResolvedDelegates.System_Runtime_InteropServices_Marshal_Copy__216;
				int[] array = new int[1] { addr };
				int num = 4;
				IntPtr intPtr2 = ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(num);
				obj(array, 0, intPtr2, 1);
				int outBuffer = 0;
				int outBufferSize = 4;
				DeviceIoControl(intPtr, ioctrl, intPtr2, num, ref outBuffer, outBufferSize, out var _, IntPtr.Zero);
				data = ResolvedDelegates.System_Convert_ToByte__75(outBuffer & 0xFF);
				ResolvedDelegates.System_Runtime_InteropServices_Marshal_FreeHGlobal__90(intPtr2);
				CloseHandle(intPtr);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("[ReadACPI] Exception: ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void WriteACPI(uint ioctrl, int addr, int data)
	{
		IntPtr intPtr = CreateFile("\\\\.\\ACPIDriver", 3221225472u, 3u, IntPtr.Zero, 3u, 0u, IntPtr.Zero);
		if (ResolvedDelegates.System_IntPtr_op_Inequality__5(intPtr, IntPtr.Zero))
		{
			object obj = ResolvedDelegates.System_Runtime_InteropServices_Marshal_Copy__216;
			int[] array = new int[2] { addr, data };
			int num = 8;
			IntPtr intPtr2 = ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(num);
			obj(array, 0, intPtr2, 2);
			int outBuffer = 0;
			int outBufferSize = 4;
			DeviceIoControl(intPtr, ioctrl, intPtr2, num, ref outBuffer, outBufferSize, out var _, IntPtr.Zero);
			ResolvedDelegates.System_Runtime_InteropServices_Marshal_FreeHGlobal__90(intPtr2);
			CloseHandle(intPtr);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void ReadSmartApcTableByDll(byte Offset, ref byte[] pData)
	{
		ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(sw);
		ResolvedDelegates.System_Diagnostics_Stopwatch_Start__153(sw);
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(m_lock_ec, EC_TimeOut))
		{
			try
			{
				object obj = ResolvedDelegates.System_Runtime_InteropServices_Marshal_Copy__214;
				byte[] array = new byte[2] { 187, Offset };
				IntPtr intPtr = ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(2);
				obj(array, 0, intPtr, 2);
				byte[] array2 = new byte[4] { 255, 255, 255, 255 };
				ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(4);
				ResolvedDelegates.System_Runtime_InteropServices_Marshal_Copy__89(SMAPCTable(intPtr), array2, 0, 4);
				pData = array2;
			}
			catch (Exception ex)
			{
				LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception: ", ResolvedDelegates.System_Object_ToString__1(ex)), "ReadSmartApcTableByDll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\AcpiCtrl.cs", 489);
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(m_lock_ec);
			}
		}
		sw.Stop();
		string text = "";
		byte[] array3 = pData;
		for (int i = 0; i < array3.Length; i++)
		{
			byte b = array3[i];
			text = ResolvedDelegates.System_String_Concat__107(text, ResolvedDelegates.System_Byte_ToString__119(ref b), ",");
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void WriteSmartApcTableByDll(byte[] Data)
	{
		try
		{
			byte[] array = UtilityExtensions.Combine(new byte[1] { 170 }, Data);
			IntPtr intPtr = ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(array.Length);
			ResolvedDelegates.System_Runtime_InteropServices_Marshal_Copy__214(array, 0, intPtr, array.Length);
			_ = new byte[4] { 255, 255, 255, 255 };
			ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(4);
			SMAPCTable(intPtr);
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception: ", ResolvedDelegates.System_Object_ToString__1(ex)), "WriteSmartApcTableByDll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyEC\\AcpiCtrl.cs", 531);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void CustomRW(string methodName, int input, ref byte data)
	{
		try
		{
			List<byte> list = new List<byte>();
			char[] array = methodName.ToArray();
			foreach (char c in array)
			{
				list.Add(ResolvedDelegates.System_Convert_ToByte__217(c));
			}
			int num = ResolvedDelegates.System_BitConverter_ToInt32__218(list.ToArray(), 0);
			IntPtr intPtr = CreateFile("\\\\.\\ACPIDriver", 3221225472u, 3u, IntPtr.Zero, 3u, 0u, IntPtr.Zero);
			if (ResolvedDelegates.System_IntPtr_op_Inequality__5(intPtr, IntPtr.Zero))
			{
				object obj = ResolvedDelegates.System_Runtime_InteropServices_Marshal_Copy__216;
				int[] array2 = new int[2] { num, input };
				int num2 = 8;
				IntPtr intPtr2 = ResolvedDelegates.System_Runtime_InteropServices_Marshal_AllocHGlobal__88(num2);
				obj(array2, 0, intPtr2, 2);
				int outBuffer = 0;
				int outBufferSize = 4;
				DeviceIoControl(intPtr, 2621482244u, intPtr2, num2, ref outBuffer, outBufferSize, out var _, IntPtr.Zero);
				data = ResolvedDelegates.System_Convert_ToByte__75(outBuffer & 0xFF);
				ResolvedDelegates.System_Runtime_InteropServices_Marshal_FreeHGlobal__90(intPtr2);
				CloseHandle(intPtr);
			}
		}
		catch
		{
		}
	}
}

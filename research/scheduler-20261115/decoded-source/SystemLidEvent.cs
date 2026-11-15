// Static analysis rendering; not original buildable source.
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Microsoft.Win32;
using Utility;

namespace MyControlCenter;

internal class SystemLidEvent
{
	internal struct POWERBROADCAST_SETTING
	{
		public Guid PowerSetting;

		public uint DataLength;

		public byte Data;
	}

	private static readonly SystemLidEvent model = new SystemLidEvent();

	public EventArgs LidStatusChange;

	private IntPtr _ScreenStateNotify;

	public static bool LaptopLidStatus = true;

	private Guid GUID_LIDSWITCH_STATE_CHANGE = new Guid(3124629325u, 47127, 16532, 162, 209, 213, 99, 121, 230, 160, 243);

	private const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;

	private const int WM_POWERBROADCAST = 536;

	private const int PBT_POWERSETTINGCHANGE = 32787;

	private bool? _previousLidState;

	private IntPtr hwnd;

	private readonly MessagePumpThread thread = new MessagePumpThread();

	private HwndSource _HwndSource;

	private bool disposed;

	public Dispatcher dispatcher
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get;
		[MethodImpl(MethodImplOptions.NoInlining)]
		set;
	}

	public static SystemLidEvent CreateInstance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return model;
		}
	}

	[DllImport("User32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
	private static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, int Flags);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr GetModuleHandle(string lpModuleName);

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Init()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected Obj, but got Unknown
		try
		{
			thread.Start(MonitorLidEvent);
			SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
		}
		catch
		{
			Dispose();
			throw;
		}
	}

	private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if ((int)e.Reason != 7)
		{
			_ = e.Reason;
			_ = 8;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void MonitorLidEvent()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected Obj, but got Unknown
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected Obj, but got Unknown
		try
		{
			IntPtr intPtr = new WindowInteropHelper(new Window()).EnsureHandle();
			_ScreenStateNotify = NativeMethods.RegisterPowerSettingNotification(intPtr, ref GUID_LIDSWITCH_STATE_CHANGE, 0);
			_HwndSource = HwndSource.FromHwnd(intPtr);
			_HwndSource.AddHook((HwndSourceHook)WndProc);
			HwndSource hwndSource = _HwndSource;
			try
			{
				thread.EnterMessageLoop();
			}
			finally
			{
				if (hwndSource != null)
				{
					ResolvedDelegates.System_IDisposable_Dispose__3(hwndSource);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public SystemLidEvent()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void RegisterForPowerNotifications()
	{
		RegisterPowerSettingNotification(hwnd, ref GUID_LIDSWITCH_STATE_CHANGE, 0);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (msg == 536)
		{
			OnPowerBroadcast(wParam, lParam);
		}
		return IntPtr.Zero;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void OnPowerBroadcast(IntPtr wParam, IntPtr lParam)
	{
		if (ResolvedDelegates.System_IntPtr_op_Explicit__589(wParam) == 32787)
		{
			POWERBROADCAST_SETTING pOWERBROADCAST_SETTING = (POWERBROADCAST_SETTING)ResolvedDelegates.System_Runtime_InteropServices_Marshal_PtrToStructure__239(lParam, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(POWERBROADCAST_SETTING).TypeHandle));
			if (ResolvedDelegates.System_Guid_op_Equality__357(pOWERBROADCAST_SETTING.PowerSetting, GUID_LIDSWITCH_STATE_CHANGE))
			{
				bool isLidOpen = pOWERBROADCAST_SETTING.Data != 0;
				LidStatusChanged(isLidOpen);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void LidStatusChanged(bool isLidOpen)
	{
		if (isLidOpen)
		{
			LaptopLidStatus = isLidOpen;
			object obj = ResolvedDelegates.System_String_Concat__29;
			string text = "{0}: Lid opened!";
			DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
			LogCtrl.Write(obj(text, ResolvedDelegates.System_DateTime_ToString__171(ref dateTime)));
		}
		else
		{
			LaptopLidStatus = isLidOpen;
			object obj2 = ResolvedDelegates.System_String_Concat__29;
			string text2 = "{0}: Lid closed!";
			DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
			LogCtrl.Write(obj2(text2, ResolvedDelegates.System_DateTime_ToString__171(ref dateTime)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Dispose()
	{
		if (!disposed)
		{
			disposed = true;
			LogCtrl.Write("ModernStandby dispose ");
			thread.Dispose();
		}
	}
}

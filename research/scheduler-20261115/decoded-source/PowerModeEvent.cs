// Static analysis rendering; not original buildable source.
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Microsoft.Win32;
using MyECIO;
using Utility;

namespace MyControlCenter;

internal class PowerModeEvent
{
	public static int g_ACLineStatus;

	private PowerStatus BatteryStatus = ResolvedDelegates.System_Windows_Forms_SystemInformation_get_PowerStatus__340();

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	private MyFanCtrl m_MyFan = MyFanCtrl.Instance;

	private static int m_TypeCstatus;

	public Dispatcher dispatcher
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get;
		[MethodImpl(MethodImplOptions.NoInlining)]
		set;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public PowerModeEvent()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected Obj, but got Unknown
		ResolvedDelegates.System_Object_ctor__0(this);
		LogCtrl.Write("[InitalizeStartPerparation] \u958b\u555f PowerModeChanged Events");
		SystemEvents.PowerModeChanged += Changed;
		RefreshACLineStatus();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Changed(object sender, PowerModeChangedEventArgs e)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
		if ((int)e.Mode == 2)
		{
			RefreshACLineStatus();
			LogCtrl.Write("PowerModeEvent | Changed | StatusChange MyFan");
			m_MyFan.PowerStatusChange(g_ACLineStatus);
			if (ResolvedDelegates.System_String_op_Equality__18(CustomizeInfo.m_sLightbarType, "2"))
			{
				LogCtrl.Write("PowerModeEvent | Changed | StatusChange MyRgbLightbar");
				MyRgbLightbarCtrl.Instance.PowerStatusChange(g_ACLineStatus);
			}
			if (CustomizeInfo.m_sKeyboardType == "2")
			{
				LogCtrl.Write("[Init] SingleColorKeyboard");
				MySettingManager.CreateInstance.InitSingleColorKeyboardACDCchange();
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe void RefreshACLineStatus()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		m_TypeCstatus = EcCtrl.GetTypeCsatusFromEC();
		PowerLineStatus val = ResolvedDelegates.System_Windows_Forms_PowerStatus_get_PowerLineStatus__341(BatteryStatus);
		if (ResolvedDelegates.System_String_Equals__445(ref *(string*)((object)val/*cast due to constrained. prefix*/).ToString(), "Offline"))
		{
			g_ACLineStatus = 0;
			return;
		}
		val = BatteryStatus.PowerLineStatus;
		if (ResolvedDelegates.System_String_Equals__445(ref *(string*)((object)val/*cast due to constrained. prefix*/).ToString(), "Online"))
		{
			g_ACLineStatus = 1;
		}
	}
}

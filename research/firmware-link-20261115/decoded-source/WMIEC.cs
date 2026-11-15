// Reconstructed analysis view; strings decoded statically; unresolved delegates remain.
using System;
using System.Management;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using GCService5;
using GCUService.MySetting.Muter;
using MyECIO;
using Utility;

namespace MyControlCenter;

internal class WMIEC
{
	public delegate void ScanCode_EventHander(int scancode);

	private static readonly WMIEC wmiec = new WMIEC();

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	private static MyFanCtrl m_MyFan = MyFanCtrl.Instance;

	private static ScanCode_EventHander[] listener = new ScanCode_EventHander[16];

	private static ManagementEventWatcher watcher = null;

	private static readonly object _lock = new object();

	private object freqLock = new object();

	public const int WMI_TimeOut = 5000;

	private static object m_lock_wmi;

	private static bool ScreenFlipStatus = false;

	private static ManagementObject _classInstance;

	private ThreadLocal<bool> isLockAcquired = new ThreadLocal<bool>(() => false);

	private static int count = 0;

	public const int SMRW_CMD_OFFSET = 0;

	public const int SMRW_VALUE_OFFSET = 8;

	public const byte SMRW_CMD_MIN = 0;

	public const byte SMRW_CMD_READ = 187;

	public const byte SMRW_CMD_WRITE = 170;

	public const byte SMRW_CMD_MAX = byte.MaxValue;

	public const int GETSETULONG2_ADDR_OFFSET = 0;

	public const int GETSETULONG2_VALUE_OFFSET = 32;

	public const int GETSETULONG2_OFFSET_OFFSET = 40;

	public const int GETSETULONG2_CMD_OFFSET = 56;

	public const ulong GETSETULONG2_CMD_READ_SMART_APC_TABLE = 12uL;

	public const ulong GETSETULONG2_CMD_WRITE_SMART_APC_TABLE = 13uL;

	public static WMIEC Instance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return wmiec;
		}
	}

	public static ManagementObject CreateInstance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected Obj, but got Unknown
			if (_classInstance == null)
			{
				_classInstance = new ManagementObject("root\\WMI", "AcpiTest_MULong.InstanceName='ACPI\\PNP0C14\\1_1'", (ObjectGetOptions)null);
			}
			return _classInstance;
		}
	}

	public static object InstanceObj
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (m_lock_wmi == null)
			{
				m_lock_wmi = new object();
			}
			return m_lock_wmi;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private WMIEC()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe void WMIHandleEvent(int scancode)
	{
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			int num = ResolvedDelegates.System_Diagnostics_Process_get_Id__87(ResolvedDelegates.System_Diagnostics_Process_GetCurrentProcess__243());
			int num2 = ResolvedDelegates.System_Threading_Thread_get_ManagedThreadId__118(ResolvedDelegates.System_Threading_Thread_get_CurrentThread__117());
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Format__176("[WMI Listener  PID={0} | Thread {1} Recive scancode {2}", num, num2, scancode), "WMIHandleEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 71);
			switch (scancode)
			{
			case 64:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} OSD_WINKEY_LOCK", scancode));
				App.m_MySetting.m_Manager.UpdateWinKeyStatus(1);
				App.m_MySetting.m_Manager.SetWinKeyREG(1);
				App.m_MySetting.m_Manager.UpdateStatusToClient(1);
				App.m_Osd.SetOsdWay(scancode);
				break;
			case 65:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} OSD_WINKEY_LOCK", scancode));
				App.m_MySetting.m_Manager.UpdateWinKeyStatus(0);
				App.m_MySetting.m_Manager.SetWinKeyREG(0);
				App.m_MySetting.m_Manager.UpdateStatusToClient(1);
				App.m_Osd.SetOsdWay(scancode);
				break;
			case 1:
			case 3:
			case 4:
			case 5:
				App.m_Osd?.SetOsdWay(scancode);
				break;
			case 2:
				App.m_Osd.SetOsdWay(scancode);
				App.m_MySetting.m_Manager.InitNumPad();
				App.m_MySetting.m_Manager.UpdateStatusToClient(1);
				break;
			case 59:
			case 60:
			case 61:
			case 62:
			case 63:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} OSD_KB_LED_LEVEL", scancode));
				App.m_Osd.ShowBLOSD(scancode);
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} BacklightLevelChange", scancode));
				if (ResolvedDelegates.System_String_op_Equality__18(CustomizeInfo.m_sKeyboardType, "2"))
				{
					PowerLineStatus val = ResolvedDelegates.System_Windows_Forms_PowerStatus_get_PowerLineStatus__341(ResolvedDelegates.System_Windows_Forms_SystemInformation_get_PowerStatus__340());
					if (ResolvedDelegates.System_String_Equals__445(ref *(string*)((object)val/*cast due to constrained. prefix*/).ToString(), "Online"))
					{
						App.m_MySetting.m_Manager.AC_BacklightLevelChanged();
					}
					else
					{
						App.m_MySetting.m_Manager.DC_BacklightLevelChanged();
					}
					App.m_MySetting.m_Manager.UpdateStatusToClient(1);
				}
				break;
			case 169:
				App.m_Osd.SetPanelStatus();
				break;
			case 164:
			{
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} OSD_AIRPLANEMODE", scancode));
				bool switchOnOff = AirplaneModeCtrl.SetAirplaneModeNoDriver();
				App.m_Osd.SetOsdWay(scancode, switchOnOff);
				break;
			}
			case 165:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} WinKey_Update", scancode));
				break;
			case 57:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X}", scancode));
				if (ResolvedDelegates.System_String_op_Inequality__40(CustomizeInfo.m_sLightbarType, "1"))
				{
					return;
				}
				LogCtrl.Write("EC ScanCode BREATH LED ON");
				MySettingParams.sLightBar_Status = "LIGHTBAR_STATUS_ON";
				App.m_MySetting.m_Manager.UpdateStatusToClient(1);
				break;
			case 58:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X}", scancode));
				if (ResolvedDelegates.System_String_op_Inequality__40(CustomizeInfo.m_sLightbarType, "1"))
				{
					return;
				}
				LogCtrl.Write("EC ScanCode BREATH LED OFF");
				MySettingParams.sLightBar_Status = "LIGHTBAR_STATUS_OFF";
				App.m_MySetting.m_Manager.UpdateStatusToClient(1);
				break;
			case 176:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} OSD_FanModeSwitch", scancode));
				m_MyFan.ModeSwitchChanged();
				break;
			case 167:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} OSD_FANBOOST_UPDATE", scancode));
				m_MyFan.FanBoostUpdate();
				break;
			case 170:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_MyFanOT", scancode));
				break;
			case 173:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_ReleaseUSM", scancode));
				break;
			case 172:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_MyBat_HPOff", scancode));
				m_MyFan.FanBoostOffFromEC();
				break;
			case 187:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_HighTemp", scancode));
				m_MyFan.SafetyProtectionUpdate();
				break;
			case 175:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_HaierLB_Sw", scancode));
				if (ResolvedDelegates.System_String_op_Equality__18(CustomizeInfo.m_sLightbarType, "0") || ResolvedDelegates.System_String_op_Equality__18(CustomizeInfo.m_sLightbarType, "1") || ResolvedDelegates.System_String_op_Equality__18(CustomizeInfo.m_sLightbarType, "3"))
				{
					return;
				}
				if (CustomizeInfo.m_sLightbarType == "2")
				{
					uint powerStatus = MyRgbLightbarCtrl.Instance.GetPowerStatus();
					string text = ((powerStatus == 1) ? "POWER_OFF" : "POWER_ON");
					LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__120("curr_rgblb_powerstatus:", ResolvedDelegates.System_UInt32_ToString__78(ref powerStatus), " -> ", text), "WMIHandleEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 248);
					App.m_MQTTService.Publish("MyRgbLightbar/Control", new
					{
						Action = text
					}, retain: false);
				}
				else
				{
					LogCtrl.TraceMessage("Unhandled light bar type", "WMIHandleEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 255);
				}
				break;
			case 180:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} BacklightPowerChange", scancode));
				if (ResolvedDelegates.System_String_op_Equality__18(CustomizeInfo.m_sKeyboardType, "2"))
				{
					App.m_MySetting.m_Manager.BacklightPowerChanged();
					App.m_MySetting.m_Manager.UpdateStatusToClient(1);
				}
				break;
			case 183:
			{
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_MicMute_Sw", scancode));
				bool switchOnOff2 = MicMuteControl_new.Instance.Trigger();
				App.m_Osd.SetOsdWay(scancode, switchOnOff2);
				break;
			}
			case 184:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} OSD_FnChange", scancode));
				App.m_MySetting.m_Manager.UpdateFnKeyStatus();
				App.m_MySetting.m_Manager.UpdateStatusToClient(1);
				App.m_Osd.SetOsdWay(scancode);
				break;
			case 186:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} CallAp", scancode));
				LaunchAppByProtocol();
				break;
			case 188:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_WhisperUpdate", scancode));
				m_MyFan.WhisperUpdate();
				break;
			case 195:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_LidClose", scancode));
				MyTouchPadCtrl.Instance.m_Manager.LidCloseDisable();
				break;
			case 196:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_LidOpen", scancode));
				MyTouchPadCtrl.Instance.m_Manager.LidOpenEnable();
				break;
			case 209:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_DISPLAY_In", scancode));
				LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_DISPLAY_In", scancode), "WMIHandleEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 353);
				if (!GPUDeviceItem.isAutoEnableDetermine)
				{
					GPUDeviceItem.Instance.SystemEvents_DisplaySettingsChanged(GPUDeviceItem.Instance, null);
				}
				if (CustomizeInfo.m_supportIGPUauto == "0")
				{
					return;
				}
				if (CustomizeInfo.m_supportIGPUauto == "1" || ResolvedDelegates.System_String_op_Equality__18(CustomizeInfo.m_supportIGPUauto, "2"))
				{
					LogCtrl.Write("---------------" + LogCtrl.GetTime() + "---------------");
					LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_DISPLAY_In", scancode));
					GPUDeviceItem.Instance.SwitchIGPUafterDetectDisplayScanCode(CustomizeInfo.m_supportIGPUauto);
				}
				break;
			case 207:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_NotifyEShutter", scancode));
				App.m_MySetting.m_Manager.UpdateECCamStatus();
				App.m_MySetting.m_Manager.UpdateStatusToClient(1);
				if (!DeviceSwitchItem.IsInitFlagChanged())
				{
					App.m_Osd?.SetOsdWay(scancode);
				}
				LogCtrl.Write(string.Format("UpdateECCamStatus finish!", Array.Empty<object>()));
				break;
			case 208:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_NotifyFlipScreen", scancode));
				if (!ScreenFlipStatus)
				{
					ChangeDisplaySettingCtrl.ChangeDisplaySetting(3);
					ScreenFlipStatus = true;
				}
				else
				{
					ChangeDisplaySettingCtrl.ChangeDisplaySetting(1);
					ScreenFlipStatus = false;
				}
				break;
			case 171:
			case 199:
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_AC_Chg", scancode));
				if (ResolvedDelegates.System_String_op_Equality__18(CustomizeInfo.m_supportIGPUauto, "2"))
				{
					LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
					LogCtrl.Write(ResolvedDelegates.System_String_Format__7("AP receive 0x{0:X} TimAP_DISPLAY_In", scancode));
					GPUDeviceItem.Instance.SwitchIGPUafterDetectDisplayScanCode(CustomizeInfo.m_supportIGPUauto);
				}
				if (EcCtrl.GetNvGpu() == 1)
				{
					LogCtrl.TraceMessage("Gaming change freq scancode before lock", "WMIHandleEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 428);
					if (ResolvedDelegates.System_Threading_Monitor_TryEnter__712(freqLock))
					{
						try
						{
							GPUDeviceItem.Instance.FrequencyOnPowerChanged();
							LogCtrl.TraceMessage("Gaming change freq scancode", "WMIHandleEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 434);
						}
						finally
						{
							ResolvedDelegates.System_Threading_Monitor_Exit__71(freqLock);
						}
					}
				}
				else
				{
					LogCtrl.TraceMessage("uma change freq scancode before lock", "WMIHandleEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 447);
					if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(freqLock, 5000))
					{
						try
						{
							GPUDeviceItem.Instance.FrequencyOnPowerChanged_UMA();
							LogCtrl.TraceMessage("uma change freq scancode before lock", "WMIHandleEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 453);
						}
						finally
						{
							ResolvedDelegates.System_Threading_Monitor_Exit__71(freqLock);
						}
					}
				}
				m_MyFan.m_Manager.SwitchModePowerChange();
				break;
			default:
				Console.WriteLine("{0}", scancode);
				break;
			case 179:
				break;
			}
			if (listener != null)
			{
				ScanCode_EventHander[] array = listener;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Invoke(scancode);
				}
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ex.ToString(), "WMIHandleEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 487);
		}
	}

	protected void OnStart(string[] args)
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void LaunchAppByPs()
	{
		string text = "CCU.WinUI";
		try
		{
			string text2 = ResolvedDelegates.System_String_Format__7("$p=(Get-AppxPackage '*{0}*').PackageFamilyName; if($p){{ explorer.exe \"shell:AppsFolder\\$($p)!App\" }}", text);
			string exePath = ResolvedDelegates.System_IO_Path_Combine__284(ResolvedDelegates.System_Environment_GetFolderPath__462(Environment.SpecialFolder.System), "WindowsPowerShell\\v1.0\\powershell.exe");
			string arguments = ResolvedDelegates.System_String_Concat__107("-NoProfile -ExecutionPolicy Bypass -Command \"", text2, "\"");
			if (SmartElevator.LaunchProcessAsAdmin(exePath, arguments))
			{
				LogCtrl.TraceMessageProgramData("\u6307\u4ee4\u5df2\u6210\u529f\u901a\u8fc7 Token \u6ce8\u5165\u5230\u7528\u6237 Session\u3002", "LaunchAppByPs", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 515);
			}
			else
			{
				LogCtrl.TraceMessageProgramData("Token \u6ce8\u5165\u5931\u8d25\uff0c\u8bf7\u68c0\u67e5\u6d3b\u8dc3 Session \u662f\u5426\u5b58\u5728\u3002", "LaunchAppByPs", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 519);
			}
		}
		catch (Exception)
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void LaunchAppByProtocol()
	{
		try
		{
			string text = "ccuwinui://";
			string exePath = ResolvedDelegates.System_IO_Path_Combine__284(ResolvedDelegates.System_Environment_GetEnvironmentVariable__612("windir"), "explorer.exe");
			string arguments = text;
			if (!SmartElevator.LaunchProcessAsAdmin(exePath, arguments))
			{
				LogCtrl.TraceMessageProgramData("\u5e94\u7528\u542f\u52a8\u5931\u8d25\uff1a\u65e0\u6cd5\u901a\u8fc7 Token \u6ce8\u5165\u534f\u8bae\u3002", "LaunchAppByProtocol", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 543);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("Launcher Error: ", ResolvedDelegates.System_Exception_get_Message__58(ex)), "LaunchAppByProtocol", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 548);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void StartWMIReceiveEvent(EventArrivedEventHandler WMIHandleEvent)
	{
		//IL_00f1: Expected Obj, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected Obj, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected Obj, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected Obj, but got Unknown
		try
		{
			object obj = _lock;
			bool flag = false;
			try
			{
				ResolvedDelegates.System_Threading_Monitor_Enter__70(obj, ref flag);
				if (watcher == null)
				{
					WqlEventQuery val = new WqlEventQuery("SELECT * FROM AcpiTest_EventULong");
					watcher = new ManagementEventWatcher(new ManagementScope("\\\\.\\Root\\WMI"), (EventQuery)(object)val);
					ResolvedDelegates.System_Management_ManagementEventWatcher_Start__57(watcher);
					LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Format__7("WMI event count = {0}", ResolvedDelegates.System_Threading_Interlocked_Increment__616(ref count)), "StartWMIReceiveEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 567);
				}
				watcher.EventArrived -= WMIHandleEvent;
				ResolvedDelegates.System_Management_ManagementEventWatcher_add_EventArrived__56(watcher, WMIHandleEvent);
			}
			finally
			{
				if (flag)
				{
					ResolvedDelegates.System_Threading_Monitor_Exit__71(obj);
				}
			}
		}
		catch (ManagementException ex)
		{
			ManagementException ex2 = ex;
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("An error occurred while trying to receive an event: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void EndWMIRecieveEvent()
	{
		object obj = _lock;
		bool flag = false;
		try
		{
			ResolvedDelegates.System_Threading_Monitor_Enter__70(obj, ref flag);
			if (watcher != null)
			{
				ResolvedDelegates.System_Management_ManagementEventWatcher_Stop__197(watcher);
				ResolvedDelegates.System_ComponentModel_Component_Dispose__198(watcher);
				watcher = null;
			}
			if (listener != null)
			{
				listener = null;
			}
		}
		finally
		{
			if (flag)
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(obj);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool WMIReadECRAM(ulong Addr, ref object data)
	{
		//IL_00a5: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				Addr = 1099511627776L + Addr;
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", Addr);
				ManagementBaseObject val = obj(createInstance, "GetSetULong", methodParameters, null);
				data = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205(val, "Return");
				return true;
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__120("WMIReadECRAM: GetSetULong failed, Addr: 0x", ResolvedDelegates.System_UInt64_ToString__713(ref Addr, "X"), ", error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return false;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut++++++ Monitor.Exit+++++++++WMIWriteECRAM"));
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void WMIWriteECRAM(ulong Addr, ulong Value)
	{
		//IL_0084: Expected Obj, but got Unknown
		if (!ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			return;
		}
		try
		{
			object obj = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
			ManagementObject createInstance = CreateInstance;
			ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
			Value <<= 16;
			Addr = Value + Addr;
			ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", Addr);
			obj(createInstance, "GetSetULong", methodParameters, null);
		}
		catch (ManagementException ex)
		{
			ManagementException ex2 = ex;
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__120("WMIWriteECRAM: GetSetULong failed, Addr: ", ResolvedDelegates.System_UInt64_ToString__586(ref Addr), ", error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
		}
		finally
		{
			ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut++++++ Monitor.Exit+++++++++WMIWriteECRAM"));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool WMICheckNVHADstatus()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected Obj, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ManagementObjectCollection val = ResolvedDelegates.System_Management_ManagementObjectSearcher_Get__199((object)new ManagementObjectSearcher("select * from Win32_SoundDevice where name = 'NVIDIA High Definition Audio'"));
			if (ResolvedDelegates.System_Management_ManagementObjectCollection_get_Count__359(val) == 0)
			{
				return true;
			}
			ManagementObjectEnumerator enumerator = val.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PropertyDataEnumerator enumerator2 = ((ManagementBaseObject)(ManagementObject)enumerator.Current).Properties.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							PropertyData current = enumerator2.Current;
							if (ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_Management_PropertyData_get_Name__714(current), "StatusInfo") && ResolvedDelegates.System_Convert_ToInt16__715(ResolvedDelegates.System_Management_PropertyData_get_Value__53(current)) == 3)
							{
								return true;
							}
						}
					}
					finally
					{
						if (enumerator2 is IDisposable disposable)
						{
							ResolvedDelegates.System_IDisposable_Dispose__3(disposable);
						}
					}
				}
			}
			finally
			{
				if (enumerator != null)
				{
					ResolvedDelegates.System_IDisposable_Dispose__3(enumerator);
				}
			}
			return false;
		}
		catch
		{
			LogCtrl.Write("WMICheckNVHADstatus: failed ");
			ResolvedDelegates.System_Console_WriteLine__8("WMICheckNVHADstatus: failed ");
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIWriteECIGPUonlyON()
	{
		//IL_01c4: Expected Obj, but got Unknown
		try
		{
			DateTime dateTime;
			if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
			{
				dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " Enter++++++++++++++++++++WMIWriteECIGPUonlyON"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("Thread {0} entering monitor.", ResolvedDelegates.System_Threading_Thread_get_ManagedThreadId__118(ResolvedDelegates.System_Threading_Thread_get_CurrentThread__117())));
				isLockAcquired.Value = true;
				if (!WMICheckNVHADstatus())
				{
					LogCtrl.Write("NVHADstatus not ready!");
					return 170;
				}
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 3298534883329L);
				ManagementBaseObject val = createInstance.InvokeMethod("GetSetULong", methodParameters, (InvokeMethodOptions)null);
				dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " ++++++++++++++++++++WMIWriteECIGPUonlyON"));
				return (byte)(Convert.ToUInt64(val["Return"]) & 0xFF);
			}
			dateTime = DateTime.Now;
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " TimeOut++++++++++++++++++++WMIWriteECIGPUonlyON"));
		}
		catch (ManagementException ex)
		{
			ManagementException ex2 = ex;
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIWriteECIGPUonlyON: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
		}
		finally
		{
			if (isLockAcquired.Value)
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " Monitor.Exit+++++++++WMIWriteECIGPUonlyON"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("Thread {0} exiting monitor.", ResolvedDelegates.System_Threading_Thread_get_ManagedThreadId__118(ResolvedDelegates.System_Threading_Thread_get_CurrentThread__117())));
				isLockAcquired.Value = false;
			}
		}
		return 170;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIWriteECIGPUonlyOFF()
	{
		//IL_01c6: Expected Obj, but got Unknown
		try
		{
			if (UtilityExtensions.IsNvGpu())
			{
				return 1;
			}
			DateTime dateTime;
			if (Monitor.TryEnter(InstanceObj, 5000))
			{
				dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " Enter----------------------WMIWriteECIGPUonlyOFF"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("Thread {0} entering monitor.", ResolvedDelegates.System_Threading_Thread_get_ManagedThreadId__118(ResolvedDelegates.System_Threading_Thread_get_CurrentThread__117())));
				isLockAcquired.Value = true;
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 3298534883328L);
				ManagementBaseObject val = obj3(createInstance, "GetSetULong", methodParameters, null);
				dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " ----------------------WMIWriteECIGPUonlyOFF"));
				return (byte)(obj(obj2(val, "Return")) & 0xFF);
			}
			dateTime = DateTime.Now;
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " TimeOut----------------------WMIWriteECIGPUonlyOFF"));
		}
		catch (ManagementException ex)
		{
			ManagementException ex2 = ex;
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIWriteECIGPUonlyOFF: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
		}
		finally
		{
			if (isLockAcquired.Value)
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " Monitor.Exit-----------WMIWriteECIGPUonlyOFF"));
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("Thread {0} exiting monitor.", ResolvedDelegates.System_Threading_Thread_get_ManagedThreadId__118(ResolvedDelegates.System_Threading_Thread_get_CurrentThread__117())));
				isLockAcquired.Value = false;
			}
		}
		return 170;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIGetECDGPUStatusForIGPUonly()
	{
		//IL_0156: Expected Obj, but got Unknown
		DateTime dateTime;
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " Enter@@@@@@@@@@@@@@@@@@@WMIGetECDGPUStatusForIGPUonly"));
				object obj = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 3298534883330L);
				ManagementBaseObject val = obj(createInstance, "GetSetULong", methodParameters, null);
				dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime) + " @@@@@@@@@@@@@@@@@@@@@@@@@@WMIGetECDGPUStatusForIGPUonly: " + ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205(val, "Return"));
				return (byte)(ResolvedDelegates.System_Convert_ToUInt64__594(ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205(val, "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIGetECDGPUonlyStatus: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		dateTime = DateTime.Now;
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " TimeOut@@@@@@@@@@@@@@@@@@@@@@@@@@WMIGetECDGPUStatusForIGPUonly"));
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIGetECIGPUonlySupport()
	{
		//IL_00b3: Expected Obj, but got Unknown
		DateTime dateTime;
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 3298534883331L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIGetECIGPUonlySupport: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "Monitor.Exit################WMIGetECIGPUonlySupport"));
			}
		}
		dateTime = DateTime.Now;
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), " TimeOut################WMIGetECIGPUonlySupport"));
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIGetECTjMax()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 4398046511104L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIGetECIGPUonlySupport: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMIGetECTjMax"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIIsTurboSubModeSupport()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138888L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIIsTurboSubModeSupport: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIIsSupport_WA_Hiberboot()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 6597069766657L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIIsSupport_WA_Hiberboot: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIIsSupport_WA_DeviceKey()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 6597069766658L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIIsSupport_WA_DeviceKey: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIIsSupport_LCD_OD()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138902L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIIsSupport_LCD_OD: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIIsSupport_Mic_NoiseReduction()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138905L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIIsSupport_Mic_NoiseReduction: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIIDR2Check()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 6597069766661L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIIDR2Check: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIIsSupport_IGPU_Mode()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 6597069766659L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIIsSupport_IGPU_Mode: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMILCD_OD_OFF()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138903L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMILCD_OD_OFF: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMILCD_OD_ON()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138904L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMILCD_OD_ON: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMICPUGPUCombineRunON"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIWriteSilentSubModeForBiosRAM()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138889L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIWriteTurboModeChildForBiosRAM: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMIWriteSilentSubModeForBiosRAM"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIWriteExtremeSubModeForBiosRAM()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138890L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIWriteTurboModeChildForBiosRAM: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMIWriteExtremeSubModeForBiosRAM"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMICPUGPUCombineRunOFF()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138881L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMICPUGPUCombineRunOFF: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMICPUGPUCombineRunON()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138882L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMICPUGPUCombineRunON: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMICPUGPUCombineRunON"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIAntiOneOFF()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138896L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIAntiOneOFF: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIAntiOneON()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138897L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIAntiOneON: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMICPUGPUCombineRunON"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMICPUGPUCombineRunSupport()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138883L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMICPUGPUCombineRunSupport: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMISSDTemperatureSupport()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138891L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMICPUGPUCombineRunSupport: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMISSDTemperatureSupport"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMITurboOCAdjustSupport()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138913L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMITurboOCAdjustSupport: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMITurboOCAdjustDefault()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138914L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMITurboOCAdjustDefault: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMITurboOCAdjust43DBValue()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138915L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMITurboOCAdjustDefault: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMITurboOCAdjust46DBValue()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138916L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMITurboOCAdjustDefault: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMINewUMASupport()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138919L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMINewUMASupport: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIGPUCombineRunOFF()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138884L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIGPUCombineRunOFF: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMIGPUCombineRunOFF"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIGPUCombineRunON()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138885L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIGPUCombineRunON: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMIGPUCombineRunON"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIGPUCombineRunSupport()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138886L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIGPUCombineRunSupport: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMIGPUCombineRunSupport"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIWriteLocalDimmingON()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138894L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIWriteLocalDimmingON: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "WMIWriteLocalDimmingON"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIWriteLocalDimmingOFF()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138893L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIWriteLocalDimmingOFF: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "WMIWriteLocalDimmingOFF"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMICPUGPUCombineRunSupportGame()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138899L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMICPUGPUCombineRunSupportGame: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMIBatteryLogoSupport()
	{
		//IL_00ab: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToByte__512;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 3298534883332L);
				return obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return"));
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIBatteryLogoSupport: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMICPUGPUCombineRunONGame()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138900L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMICPUGPUCombineRunONGame: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
				DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMICPUGPUCombineRunONGame"));
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte WMICPUGPUCombineRunOFFGame()
	{
		//IL_00b3: Expected Obj, but got Unknown
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			try
			{
				object obj = ResolvedDelegates.System_Convert_ToUInt64__594;
				object obj2 = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205;
				object obj3 = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
				ManagementObject createInstance = CreateInstance;
				ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong");
				ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", 5497558138901L);
				return (byte)(obj(obj2(obj3(createInstance, "GetSetULong", methodParameters, null), "Return")) & 0xFF);
			}
			catch (ManagementException ex)
			{
				ManagementException ex2 = ex;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMICPUGPUCombineRunOFFGame: GetSetULong failed, Addr: , error message: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("GetSetULong failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
				return 0;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			}
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void WMIWriteBiosRAM(ulong Value)
	{
		//IL_008e: Expected Obj, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected Obj, but got Unknown
		if (!ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			return;
		}
		try
		{
			object obj = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
			ManagementObject val = new ManagementObject("root\\WMI", "AcpiODM_Demo.InstanceName='ACPI\\PNP0C14\\2_0'", (ObjectGetOptions)null);
			ManagementBaseObject methodParameters = val.GetMethodParameters("GetUlongEx7");
			ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", Value);
			obj((object)val, "GetUlongEx7", methodParameters, null);
		}
		catch (ManagementException ex)
		{
			ManagementException ex2 = ex;
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("WMIWriteBiosRAM : Failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("WMIWriteBiosRAM : Failed", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
		}
		finally
		{
			ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------WMIWriteBiosRAM"));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public ulong Combine(byte b7, byte b6, byte b5, byte b4, byte b3, byte b2, byte b1, byte b0)
	{
		return ResolvedDelegates.System_Convert_ToUInt64__439(ResolvedDelegates.System_BitConverter_ToInt64__711(new byte[8] { b0, b1, b2, b3, b4, b5, b6, b7 }, 0));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Smrw(ulong Cmd, ulong Value, ref object RetData)
	{
		//IL_00e2: Expected Obj, but got Unknown
		if (!ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			return;
		}
		try
		{
			object obj = ResolvedDelegates.System_Management_ManagementObject_InvokeMethod__348;
			ManagementObject createInstance = CreateInstance;
			ManagementBaseObject methodParameters = createInstance.GetMethodParameters("SMRW");
			ulong num = Cmd + (Value << 8);
			ResolvedDelegates.System_Management_ManagementBaseObject_set_Item__347(methodParameters, "Data", num);
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("0x", ResolvedDelegates.System_UInt64_ToString__713(ref num, "X")));
			ManagementBaseObject val = obj(createInstance, "SMRW", methodParameters, null);
			RetData = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205(val, "Return");
		}
		catch (ManagementException ex)
		{
			ManagementException ex2 = ex;
			Console.WriteLine("Smrw: failed, error        : " + (object)ex2);
			LogCtrl.TraceMessage("Smrw: failed, error        : " + (object)ex2, "Smrw", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\WMIEC.cs", 2614);
		}
		finally
		{
			ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------Smrw"));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void GetSetULong2(ulong Cmd, ref object RetData)
	{
		GetSetULong2(Cmd, 0uL, 0uL, 0uL, ref RetData);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void GetSetULong2(ulong Cmd, ulong Addr, ulong Offset, ulong Value, ref object RetData)
	{
		//IL_00f7: Expected Obj, but got Unknown
		if (!ResolvedDelegates.System_Threading_Monitor_TryEnter__145(InstanceObj, 5000))
		{
			return;
		}
		try
		{
			ManagementObject createInstance = CreateInstance;
			ManagementBaseObject methodParameters = createInstance.GetMethodParameters("GetSetULong2");
			ulong num;
			if (Cmd == 13)
			{
				Value &= 0xFFFFFFFFFFFFFFL;
				num = (Cmd << 56) + Value;
			}
			else
			{
				num = (Cmd << 56) + (Offset << 40) + (Value << 32) + Addr;
			}
			methodParameters["Data"] = num;
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("0x", ResolvedDelegates.System_UInt64_ToString__713(ref num, "X")));
			ManagementBaseObject val = createInstance.InvokeMethod("GetSetULong2", methodParameters, (InvokeMethodOptions)null);
			RetData = ResolvedDelegates.System_Management_ManagementBaseObject_get_Item__205(val, "Return");
		}
		catch (ManagementException ex)
		{
			Console.WriteLine("GETSETULONG2: failed, error        : " + (object)ex);
		}
		finally
		{
			ResolvedDelegates.System_Threading_Monitor_Exit__71(InstanceObj);
			DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_DateTime_ToString__171(ref dateTime), "TimeOut----------- Monitor.Exit-----------GetSetULong2"));
		}
	}
}

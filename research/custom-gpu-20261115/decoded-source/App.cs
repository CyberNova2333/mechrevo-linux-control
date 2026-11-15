// Static analysis view; no original code executed.
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Define;
using GCService5;
using GCUService;
using GCUService.MyFan.BTCooling;
using GCUService.MyFan.Overclocking;
using GCUService.MyRgbKeyboard;
using GCUService.MySetting;
using GCUService.MySystem;
using GCUService.ScanCode;
using GCUService.Utility;
using GCUService.WCFService;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using MyControlCenter.MyFan.FanTable;
using MyECIO;
using MyRGBKeyboard;
using Utility;
using Workaround;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MyControlCenter;

public class App : Application
{
	private enum ModernTargetState
	{
		None,
		On,
		Off
	}

	[CompilerGenerated]
	private static class <>o__45
	{
		public static CallSite<Func<CallSite, object, object, object>> <>p__0;

		public static CallSite<Func<CallSite, object, bool>> <>p__1;

		public static CallSite<Func<CallSite, object, object>> <>p__2;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__3;

		public static CallSite<Func<CallSite, object, uint>> <>p__4;

		public static CallSite<Func<CallSite, object, object>> <>p__5;

		public static CallSite<Func<CallSite, object, object>> <>p__6;

		public static CallSite<Func<CallSite, object, string>> <>p__7;

		public static CallSite<Func<CallSite, object, object>> <>p__8;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__9;

		public static CallSite<Func<CallSite, object, uint>> <>p__10;
	}

	[CompilerGenerated]
	private static class <>o__78
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string>> <>p__2;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <ModernOn>d__71 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public App <>4__this;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			int num = <>1__state;
			App app = <>4__this;
			try
			{
				if (ResolvedDelegates.System_Threading_Interlocked_CompareExchange__717(ref app._modernExecuting, 1, 0) == 0)
				{
					try
					{
						app.m_PowerModeEvent?.RefreshACLineStatus();
						LogCtrl.TraceMessage("Set_APExistToEC", "ModernOn", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1144);
						app.EcCtrl.Set_APExistToEC(bExist: true);
						LogCtrl.TraceMessage("Resume MyFan", "ModernOn", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1147);
						app.m_MyFan.Resume();
						LogCtrl.TraceMessage("Resume MySetting", "ModernOn", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1151);
						m_MySetting.Resume(s4resume: false);
						LogCtrl.TraceMessage("Resume BatteryProection", "ModernOn", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1155);
						BatteryProtection2.Instance.Resume();
						ReflashTray.RefreshTrayArea();
						WKD_AMD.Intel_WiFi6_YB_Recovery();
						m_MySystem.StartCPUGPUConbineONOFFTimers();
					}
					catch (Exception)
					{
					}
					finally
					{
						if (num < 0)
						{
							ResolvedDelegates.System_Threading_Interlocked_Exchange__651(ref app._modernExecuting, 0);
						}
					}
				}
			}
			catch (Exception ex2)
			{
				<>1__state = -2;
				ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_SetException__194(ref <>t__builder, ex2);
				return;
			}
			<>1__state = -2;
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <ModernStandbyChangedEventHandler>d__57 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public object status;

		public App <>4__this;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			App app = <>4__this;
			try
			{
				LogCtrl.TraceMessage("ModernStandby " + status, "ModernStandbyChangedEventHandler", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 911);
				LogCtrl.TraceMessageProgramData("ModernStandby " + status, "ModernStandbyChangedEventHandler", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 912);
				if (status?.ToString() == "Off")
				{
					app.RequestModernOff();
				}
				else if (status?.ToString() == "On")
				{
					app.RequestModernOn();
				}
			}
			catch (Exception ex)
			{
				<>1__state = -2;
				ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_SetException__194(ref <>t__builder, ex);
				return;
			}
			<>1__state = -2;
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <ServiceRecieve>d__78 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public byte[] data;

		private TaskAwaiter<object> <>u__1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			int num = <>1__state;
			try
			{
				TaskAwaiter<object> awaiter;
				if (num != 0)
				{
					awaiter = Json.ToObjectAsync<object>(ResolvedDelegates.System_Text_Encoding_GetString__351(ResolvedDelegates.System_Text_Encoding_get_UTF8__252(), data)).GetAwaiter();
					if (!awaiter.IsCompleted)
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
				object result = awaiter.GetResult();
				if (<>o__78.<>p__2 == null)
				{
					<>o__78.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle)));
				}
				Func<CallSite, object, string> target = <>o__78.<>p__2.Target;
				CallSite<Func<CallSite, object, string>> arg = <>o__78.<>p__2;
				if (<>o__78.<>p__0 == null)
				{
					<>o__78.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				object obj = <>o__78.<>p__0.Target(<>o__78.<>p__0, result, "Action");
				object arg2;
				if (obj == null)
				{
					arg2 = null;
				}
				else
				{
					if (<>o__78.<>p__1 == null)
					{
						<>o__78.<>p__1 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg2 = <>o__78.<>p__1.Target(<>o__78.<>p__1, obj);
				}
				string text = target(arg, arg2);
				_ = string.Empty;
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("App | Receive | msg = ", text));
				if (ResolvedDelegates.System_String_op_Equality__18(text, "RESTORE"))
				{
					MyFanCtrl.Instance.Restore();
					MyRgbLightbarCtrl.Instance.Restore();
					RGBKeyboard.Instance.Restore();
					m_MySetting.Restore();
				}
			}
			catch (Exception ex)
			{
				<>1__state = -2;
				ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_SetException__194(ref <>t__builder, ex);
				return;
			}
			<>1__state = -2;
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <SuperviseService>d__85 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public App <>4__this;

		public string appName;

		public CancellationToken ct;

		private bool <hasBeenCounted>5__2;

		private TaskAwaiter<string> <>u__1;

		private TaskAwaiter <>u__2;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe void MoveNext()
		{
			int num = <>1__state;
			App app = <>4__this;
			try
			{
				if (num == 0)
				{
					goto IL_0027;
				}
				if (num != 1)
				{
					<hasBeenCounted>5__2 = false;
					goto IL_0295;
				}
				TaskAwaiter awaiter = <>u__2;
				<>u__2 = default;
				num = (<>1__state = -1);
				goto IL_028e;
				IL_028e:
				awaiter.GetResult();
				goto IL_0295;
				IL_018c:
				object obj;
				int num2;
				if (num2 == 1)
				{
					Exception ex = (Exception)obj;
					LogCtrl.Write(ResolvedDelegates.System_String_Concat__79(new string[5]
					{
						"[Supervisor] ",
						appName,
						" crashed: ",
						ResolvedDelegates.System_Exception_get_Message__58(ex),
						", restarting in 30s..."
					}));
					awaiter = ResolvedDelegates.System_Threading_Tasks_Task_GetAwaiter__23(ResolvedDelegates.System_Threading_Tasks_Task_Delay__437(ResolvedDelegates.System_TimeSpan_FromSeconds__430(30.0), ct));
					if (!ResolvedDelegates.System_Runtime_CompilerServices_TaskAwaiter_get_IsCompleted__24(ref awaiter))
					{
						num = (<>1__state = 1);
						<>u__2 = awaiter;
						<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_028e;
				}
				goto IL_0295;
				IL_0027:
				try
				{
					TaskAwaiter<string> awaiter2;
					if (num == 0)
					{
						awaiter2 = <>u__1;
						<>u__1 = default;
						num = (<>1__state = -1);
						goto IL_009a;
					}
					if (app.StartUpTask(appName, null) is Task<string> task)
					{
						awaiter2 = task.GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (<>1__state = 0);
							<>u__1 = awaiter2;
							<>t__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						goto IL_009a;
					}
					goto end_IL_0027;
					IL_009a:
					string result = awaiter2.GetResult();
					if (!<hasBeenCounted>5__2 && ResolvedDelegates.System_String_Contains__108(ref *(string*)result, "Enable "))
					{
						<hasBeenCounted>5__2 = true;
						int num3 = ResolvedDelegates.System_Threading_Interlocked_Increment__616(ref ServiceLoadingCount);
						LogCtrl.Write(ResolvedDelegates.System_String_Format__176("[Supervisor] {0} ready. ({1}/{2})", appName, num3, app.ApplicationNames.Count));
						if (num3 == app.ApplicationNames.Count)
						{
							RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(app.m_sRegPath, "\\ItemSupport"), "ServiceLoadingReady", 1, RegistryValueKind.DWord);
							LogCtrl.Write("@@@@ All Services Ready!!");
						}
					}
					end_IL_0027:;
				}
				catch (Exception ex2)
				{
					obj = ex2;
					num2 = 1;
					goto IL_018c;
				}
				goto end_IL_000e;
				IL_0295:
				if (!ct.IsCancellationRequested)
				{
					num2 = 0;
					goto IL_0027;
				}
				end_IL_000e:;
			}
			catch (Exception ex3)
			{
				<>1__state = -2;
				ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_SetException__26(ref <>t__builder, ex3);
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

	public static TrayCtrl m_TrayCtrl;

	public static MySystemCtrl m_MySystem;

	private MyFanTableCtrl m_MyFanTable;

	private MyFanCtrl m_MyFan;

	private MyRgbLightbarCtrl m_RGBLB;

	public static MySettingCtrl m_MySetting;

	public static OSDManager m_Osd;

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	private static WMIEC m_Wmi = WMIEC.Instance;

	private PowerModeEvent m_PowerModeEvent;

	public static MqttClientCtrl m_MQTTService = MqttClientCtrl.Instance;

	public static bool bDebug = false;

	private RGBKeyboard rgbkeyboard;

	private HIDRGBLightbar hidrgblightbar;

	private HIDLightbar_logo hidrgblightbarlogo;

	private HIDLightbar_hinge hidrgblightbarhinge;

	private HIDLightbar_sync hidrgblightbarsync;

	private Dictionary<string, Task> TasksMoniter = new Dictionary<string, Task>();

	private Dictionary<string, Thread> ThreadList = new Dictionary<string, Thread>();

	private List<string> ApplicationNames = new List<string>
	{
		"MyRGBKeyboard",
		"MySystem",
		"MyFan",
		"MySetting",
		"MyRgbLightbar"
	};

	private List<string> gInstalledSoftware = new List<string>();

	private WCFServiceHost host;

	public static int OsdOnly = 0;

	public static int m_nCustomizeTarget = 1;

	public static int m_nBridgeType = 0;

	private string m_sRegPath = "\\OEM\\GamingCenter2";

	public static LiquidCooling liquid;

	public static bool LCSupport;

	private GpuFeatures cGpuFeatures = new GpuFeatures();

	private HardwareInfoCollect hardwareinfo = new HardwareInfoCollect();

	public static int ServiceLoadingCount = 0;

	private OemAcpiWmiListener _listener;

	private System.Timers.Timer diskinfoTimer;

	private UIntPtr LpId;

	private uint SSD_One_Temperature;

	private uint SSD_Two_Temperature;

	private bool IsWrite = true;

	private uint preTemperature;

	private uint SSD_frist;

	private object launchlock = new object();

	private static readonly double[] thresholds = new double[2] { 59.0, 65.0 };

	private ModernStandbyEvent m_modernStandby;

	private SystemLidEvent systemLidEvent;

	private bool modernON;

	private volatile ModernTargetState _targetState;

	private int _executing;

	private int _pendingRetry;

	private Thread hidLightbarThread;

	private Thread rgbkeyboardThread;

	private int _modernExecuting;

	private readonly CancellationTokenSource _cts = new CancellationTokenSource();

	private Stopwatch sw = new Stopwatch();

	private static int m_FirstTime = 0;

	private bool _contentLoaded;

	public static void log(string strLog)
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Application_Startup(object sender, StartupEventArgs e)
	{
		RenderOptions.ProcessRenderMode = (RenderMode)1;
		LogCtrl.EnableLog();
		LogCtrl.SetProgramDataLogPath();
		LogCtrl.ClearProgramDataLog();
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
		LogCtrl.Write("[Application_Startup] Start");
		LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"), "Application_Startup", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 128);
		LogCtrl.TraceMessageProgramData("[Application_Startup] Start", "Application_Startup", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 129);
		try
		{
			OsdOnly = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2", "OsdOnly", 0);
		}
		catch
		{
		}
		m_nCustomizeTarget = RegistryCtrl.GetCustomizeTarget();
		m_nBridgeType = RegistryCtrl.GetBridgeType();
		m_PowerModeEvent = new PowerModeEvent();
		m_MySystem = new MySystemCtrl();
		m_MyFanTable = MyFanTableCtrl.Instance;
		m_MyFan = MyFanCtrl.Instance;
		m_RGBLB = MyRgbLightbarCtrl.Instance;
		m_MySetting = new MySettingCtrl();
		m_Osd = new OSDManager();
		rgbkeyboard = RGBKeyboard.Instance;
		hidrgblightbar = HIDRGBLightbar.Instance;
		hidrgblightbarlogo = HIDLightbar_logo.Instance;
		hidrgblightbarhinge = HIDLightbar_hinge.Instance;
		hidrgblightbarsync = HIDLightbar_sync.Instance;
		ApplicationNames = new List<string>
		{
			"MyRGBKeyboard",
			"MySystem",
			"MyFan",
			"MySetting",
			"MyRgbLightbar"
		};
		FirstTimeDefaultTool();
		if (e.Args.Count() == 0)
		{
			if (m_MQTTService == null)
			{
				LogCtrl.TraceMessage("m_MQTTService==null, then create MqttClientCtrl.Instance.", "Application_Startup", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 184);
				m_MQTTService = MqttClientCtrl.Instance;
			}
			if (m_MQTTService != null)
			{
				m_MQTTService.ClientMessage += M_MQTTService_ClientMessage;
				m_MQTTService.ClientConnection += M_MQTTService_ClientConnection;
				m_MQTTService.Run();
				LogCtrl.TraceMessage("m_MQTTService!=null, running.", "Application_Startup", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 192);
			}
			else
			{
				LogCtrl.TraceMessage("m_MQTTService==null, do nothing.", "Application_Startup", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 196);
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (WMIEC.Instance.WMICPUGPUCombineRunSupport() == 85)
			{
				flag2 = true;
			}
			if (WMIEC.Instance.WMICPUGPUCombineRunSupportGame() == 85)
			{
				flag3 = true;
			}
			if (flag2 | flag3 | flag)
			{
				LogCtrl.Write("InitCPUGPUConbineONOFFTimers Support");
				log("WMICPUGPUCombineRunSupport Support");
				LogCtrl.TraceMessageProgramData("cpugpuDTRunTimer_Elapsed Support", "Application_Startup", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 287);
				m_MySystem.InitCPUGPUConbineONOFFTimers(flag2, flag3, flag);
			}
			else
			{
				LogCtrl.Write("InitCPUGPUConbineONOFFTimers Not Support");
				log("InitCPUGPUConbineONOFFTimers Not Support");
			}
			LCSupport = EcCtrl.IsSupportLiquidCooling();
			LpId = ProcessorUtility.SetAffinity();
			if (NvramVariable.IsHighSpeedSSD() == 1)
			{
				InitDiskinfoTimer();
			}
			BIOS_PROJECT_ID biosProjctID = EcCtrl.GetBiosProjctID();
			if (biosProjctID == BIOS_PROJECT_ID.IDI && IsSTPORKK())
			{
				GPUDeviceItem.Instance.ReadEdidFromRegistry();
			}
			if (biosProjctID == BIOS_PROJECT_ID.IDH && IsGPT())
			{
				GPUDeviceItem.Instance.ReadEdidFromRegistry("EF25QBA64.A");
			}
		}
		else
		{
			m_MQTTService.ClientMessage -= M_MQTTService_ClientMessage;
			m_MQTTService.ClientConnection -= M_MQTTService_ClientConnection;
			Arguments.Uninstall = Enumerable.Contains(e.Args, "-u");
			if (Arguments.Uninstall)
			{
				LogCtrl.TraceMessage("Uninstall", "Application_Startup", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 212);
				KillProcess("GamingCenterU");
				KillProcess("ControlCenterU");
				KillProcess("CreatorCenter");
				KillProcess("CreatorTray");
				KillProcess("ControlCenterTray");
				LogCtrl.Write("[Application_Startup] Set_APExistToEC 0");
				EcCtrl.Set_APExistToEC(bExist: false);
				NvramVariable.Get();
				rgbkeyboard.init();
				rgbkeyboard.Uninstall();
				m_MyFan?.Uninstall();
				m_MySetting.Uninstall();
				BatteryProtection2.Instance.Uninstall();
			}
			if (host != null)
			{
				host.CloseConnect();
			}
			Application.Current.Shutdown();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void InitDiskinfoTimer()
	{
		LogCtrl.Write("InitDiskinfoTimer-SSD Detect");
		diskinfoTimer = new System.Timers.Timer();
		ResolvedDelegates.System_Timers_Timer_set_Interval__441(diskinfoTimer, 1000.0);
		ResolvedDelegates.System_Timers_Timer_add_Elapsed__442(diskinfoTimer, DiskinfoTimer_Elapsed);
		ResolvedDelegates.System_Timers_Timer_Start__443(diskinfoTimer);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void StopDiskinfoTimer()
	{
		try
		{
			diskinfoTimer?.Stop();
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("System monitor error ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe void DiskinfoTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		try
		{
			hardwareinfo.SetCurrentThreadAffinityMask(hardwareinfo.GetCurrentThreadMask(), LpId);
			object diskInfo = m_MySystem.GetDiskInfo();
			if (<>o__45.<>p__1 == null)
			{
				<>o__45.<>p__1 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target = <>o__45.<>p__1.Target;
			CallSite<Func<CallSite, object, bool>> arg = <>o__45.<>p__1;
			if (<>o__45.<>p__0 == null)
			{
				<>o__45.<>p__0 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (!target(arg, <>o__45.<>p__0.Target(<>o__45.<>p__0, diskInfo, null)))
			{
				return;
			}
			object obj = launchlock;
			bool flag = false;
			try
			{
				ResolvedDelegates.System_Threading_Monitor_Enter__70(obj, ref flag);
				try
				{
					if (<>o__45.<>p__4 == null)
					{
						<>o__45.<>p__4 = CallSite<Func<CallSite, object, uint>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle)));
					}
					Func<CallSite, object, uint> target2 = <>o__45.<>p__4.Target;
					CallSite<Func<CallSite, object, uint>> arg2 = <>o__45.<>p__4;
					if (<>o__45.<>p__3 == null)
					{
						<>o__45.<>p__3 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle), new CSharpArgumentInfo[2]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
						}));
					}
					Func<CallSite, Type, object, object> target3 = <>o__45.<>p__3.Target;
					CallSite<Func<CallSite, Type, object, object>> arg3 = <>o__45.<>p__3;
					Type arg4 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
					if (<>o__45.<>p__2 == null)
					{
						<>o__45.<>p__2 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "SSD_One_Temperature", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					SSD_One_Temperature = target2(arg2, target3(arg3, arg4, <>o__45.<>p__2.Target(<>o__45.<>p__2, diskInfo)));
					if (<>o__45.<>p__7 == null)
					{
						<>o__45.<>p__7 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle)));
					}
					Func<CallSite, object, string> target4 = <>o__45.<>p__7.Target;
					CallSite<Func<CallSite, object, string>> arg5 = <>o__45.<>p__7;
					if (<>o__45.<>p__6 == null)
					{
						<>o__45.<>p__6 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					Func<CallSite, object, object> target5 = <>o__45.<>p__6.Target;
					CallSite<Func<CallSite, object, object>> arg6 = <>o__45.<>p__6;
					if (<>o__45.<>p__5 == null)
					{
						<>o__45.<>p__5 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "SSD_Two_Temperature", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					string text = target4(arg5, target5(arg6, <>o__45.<>p__5.Target(<>o__45.<>p__5, diskInfo)));
					if (text != null && ResolvedDelegates.System_String_get_Length__298(ref *(string*)text) > 0 && ResolvedDelegates.System_String_op_Inequality__40(text, "null"))
					{
						if (<>o__45.<>p__10 == null)
						{
							<>o__45.<>p__10 = CallSite<Func<CallSite, object, uint>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle)));
						}
						Func<CallSite, object, uint> target6 = <>o__45.<>p__10.Target;
						CallSite<Func<CallSite, object, uint>> arg7 = <>o__45.<>p__10;
						if (<>o__45.<>p__9 == null)
						{
							<>o__45.<>p__9 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						Func<CallSite, Type, object, object> target7 = <>o__45.<>p__9.Target;
						CallSite<Func<CallSite, Type, object, object>> arg8 = <>o__45.<>p__9;
						Type arg9 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
						if (<>o__45.<>p__8 == null)
						{
							<>o__45.<>p__8 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "SSD_Two_Temperature", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(App).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						SSD_Two_Temperature = target6(arg7, target7(arg8, arg9, <>o__45.<>p__8.Target(<>o__45.<>p__8, diskInfo)));
					}
				}
				catch
				{
					LogCtrl.Write("DiskinfoTimer_Elapsed catch");
				}
				uint num = ((SSD_One_Temperature > SSD_Two_Temperature) ? SSD_One_Temperature : SSD_Two_Temperature);
				IsWrite = CheckAndUpdateTemperature(num);
				if (IsWrite)
				{
					byte data = ResolvedDelegates.System_Convert_ToByte__716(num);
					EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2001, data);
					LogCtrl.Write(ResolvedDelegates.System_String_Concat__120("Write EC currentTemperature:", ResolvedDelegates.System_UInt32_ToString__78(ref num), "----------", LogCtrl.GetTime()));
				}
				IsWrite = false;
				preTemperature = num;
				SSD_frist = 1u;
			}
			finally
			{
				if (flag)
				{
					ResolvedDelegates.System_Threading_Monitor_Exit__71(obj);
				}
			}
		}
		catch
		{
			LogCtrl.Write("DiskinfoTimer_Elapsed flow catch");
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool CheckAndUpdateTemperature(uint currentTemperature)
	{
		int temperatureRange = GetTemperatureRange(preTemperature);
		int temperatureRange2 = GetTemperatureRange(currentTemperature);
		if ((temperatureRange == temperatureRange2 || temperatureRange2 == 2) && SSD_frist == 1)
		{
			return false;
		}
		if (SSD_frist == 0 && temperatureRange2 != 3)
		{
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static int GetTemperatureRange(double temperature)
	{
		if (!(temperature <= thresholds[0]))
		{
			if (!(temperature < thresholds[1]))
			{
				return 3;
			}
			return 2;
		}
		return 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int GetLpId(float[] CpuUsages)
	{
		float num = ResolvedDelegates.System_Linq_Enumerable_Min__148(CpuUsages);
		int num2 = 0;
		foreach (float num3 in CpuUsages)
		{
			if (num == num3)
			{
				return num2;
			}
			num2++;
		}
		return num2;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void M_MQTTService_ClientConnection(string connectMessage)
	{
		if (ResolvedDelegates.System_String_op_Equality__18(connectMessage, "Connection"))
		{
			StartMainWork();
			return;
		}
		try
		{
			((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)(() =>
			{
				Application.Current.Shutdown();
			}), Array.Empty<object>());
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessageProgramData(ex.Message, "M_MQTTService_ClientConnection", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 502);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void M_MQTTService_ClientMessage(string topic, MqttMsgPublishEventArgs e)
	{
		Recieve(e.Topic, e.Message);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe static void Recieve(string topic, byte[] Message)
	{
		try
		{
			if (topic == null)
			{
				return;
			}
			switch (ResolvedDelegates.System_String_get_Length__298(ref *(string*)topic))
			{
			case 15:
				switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)topic, 0))
				{
				case 'D':
					if (topic == "Display/Control")
					{
						MyDisplay_Manager.Instance.Receive(Message);
					}
					break;
				case 'S':
					if (topic == "Setting/Control" && m_MySetting != null)
					{
						m_MySetting.Recieve(Message);
					}
					break;
				case 'O':
					if (topic == "OTA/CCU6Control")
					{
						MR_OTA.LaunchCCU6OTA();
					}
					break;
				}
				break;
			case 22:
				switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)topic, 0))
				{
				case 'P':
					if (topic == "ProcessControl/Control")
					{
						ProcessControl.Instance.Receive(Message);
					}
					break;
				case 'H':
					if (topic == "HidLightbar_Hinge/Ctrl" && HIDLightbar_hinge.Instance != null)
					{
						HIDLightbar_hinge.Instance.OnMqttMessage(topic, ResolvedDelegates.System_Text_Encoding_GetString__351(ResolvedDelegates.System_Text_Encoding_get_UTF8__252(), Message));
					}
					break;
				}
				break;
			case 16:
				switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)topic, 0))
				{
				case 'O':
					if (topic == "Openvino/Control")
					{
						OpenvinoManager.Receive(Message);
					}
					break;
				case 'H':
					if (topic == "HidLightbar/Ctrl" && HIDRGBLightbar.Instance != null)
					{
						HIDRGBLightbar.Instance.OnMqttMessage(topic, ResolvedDelegates.System_Text_Encoding_GetString__351(ResolvedDelegates.System_Text_Encoding_get_UTF8__252(), Message));
					}
					break;
				}
				break;
			case 17:
				switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)topic, 0))
				{
				default:
					return;
				case 'C':
					if (topic == "Customize/Control")
					{
						CustomizeCtrl.Receive(Message);
					}
					return;
				case 'L':
					break;
				}
				if (!(topic == "Languages/Control"))
				{
					break;
				}
				goto IL_0603;
			case 21:
				switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)topic, 12))
				{
				case 'r':
					if (topic == "MyRgbLightbar/Control")
					{
						if (bDebug)
						{
							CustomizeInfo.m_sLightbarType = "2";
						}
						if (CustomizeInfo.m_sLightbarType == "2")
						{
							MyRgbLightbarCtrl.Instance.Recieve(Message);
						}
					}
					break;
				case 'L':
					if (topic == "HidLightbar_Logo/Ctrl" && HIDLightbar_logo.Instance != null)
					{
						HIDLightbar_logo.Instance.OnMqttMessage(topic, ResolvedDelegates.System_Text_Encoding_GetString__351(ResolvedDelegates.System_Text_Encoding_get_UTF8__252(), Message));
					}
					break;
				case 'S':
					if (topic == "HidLightbar_Sync/Ctrl" && HIDLightbar_sync.Instance != null)
					{
						HIDLightbar_sync.Instance.OnMqttMessage(topic, ResolvedDelegates.System_Text_Encoding_GetString__351(ResolvedDelegates.System_Text_Encoding_get_UTF8__252(), Message));
					}
					break;
				}
				break;
			case 13:
				switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)topic, 0))
				{
				case 'K':
					if (topic == "Keyboard/Ctrl" && RGBKeyboard.Instance != null)
					{
						RGBKeyboard.Instance.OnMqttMessage(topic, ResolvedDelegates.System_Text_Encoding_GetString__351(ResolvedDelegates.System_Text_Encoding_get_UTF8__252(), Message));
					}
					break;
				case 'S':
					if (topic == "Service/Close")
					{
						LogCtrl.Write("[M_MQTTService_ClientMessage] Service_Close");
						((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)(() =>
						{
							Application.Current.Shutdown();
						}), Array.Empty<object>());
					}
					break;
				case 'O':
					if (topic == "OTA/MRControl")
					{
						MR_OTA.LaunchOTA();
					}
					break;
				}
				break;
			case 25:
				if (ResolvedDelegates.System_String_op_Equality__18(topic, "BatteryProtection/Control"))
				{
					BatteryProtection2.Instance.Receive(Message);
				}
				break;
			case 14:
				if (ResolvedDelegates.System_String_op_Equality__18(topic, "System/Control"))
				{
					m_MySystem?.Receive(Message);
				}
				break;
			case 11:
				if (ResolvedDelegates.System_String_op_Equality__18(topic, "Fan/Control"))
				{
					MyFanCtrl.Instance.Receive(Message);
				}
				break;
			case 20:
				if (!ResolvedDelegates.System_String_op_Equality__18(topic, "Languages/AutoDetect"))
				{
					break;
				}
				goto IL_0603;
			case 12:
				if (ResolvedDelegates.System_String_op_Equality__18(topic, "Service/Ctrl"))
				{
					ServiceRecieve(Message);
				}
				break;
			case 24:
				if (ResolvedDelegates.System_String_op_Equality__18(topic, "Doudou/DoudouGameManager"))
				{
				}
				break;
			case 18:
			case 19:
			case 23:
				break;
				IL_0603:
				m_Osd?.Recieve(Message);
				break;
			}
		}
		catch (Exception)
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Application_Exit(object sender, ExitEventArgs e)
	{
		LogCtrl.TraceMessageProgramData("[Application_Exit]", "Application_Exit", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 631);
		NvramVariable.CommitFwVars();
		ServiceLoadingCount = 0;
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "ServiceLoadingReady", 0, RegistryValueKind.DWord);
		LogCtrl.TraceMessageProgramData("[ServiceLoadingReady 0]", "Application_Exit", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 635);
		LogCtrl.Write("[Application_Exit] \u505c\u6b62 WMIReceiveEvent");
		_listener?.Dispose();
		BIOS_PROJECT_ID biosProjctID = EcCtrl.GetBiosProjctID();
		if (biosProjctID == BIOS_PROJECT_ID.IDI && IsSTPORKK())
		{
			GPUDeviceItem.Instance.ReadEdidFromRegistry();
		}
		if (biosProjctID == BIOS_PROJECT_ID.IDH && IsGPT())
		{
			GPUDeviceItem.Instance.ReadEdidFromRegistry("EF25QBA64.A");
		}
		LogCtrl.Write("[Application_Exit] Disable BatteryProtection2");
		m_modernStandby?.Dispose();
		SQLiteDBHelper.Instance.Dispose();
		if (LCSupport)
		{
			Dispatcher dispatcher = ((DispatcherObject)this).Dispatcher;
			if (dispatcher != null)
			{
				dispatcher.Invoke((Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
				{
					LogCtrl.Write("[Application_Exit] Disable DisconnectLC");
					liquid?.DisconnectLC();
				}), (DispatcherPriority)9);
			}
		}
		RequestModernOff();
		Task.Run([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			NvramVariable.UpdateBufToFwVars();
			LogCtrl.Write("[Application_Exit] Disable MySystem");
			m_MySystem?.Dispose();
			LogCtrl.Write("[Application_Exit] Disable BatteryProtection2");
			BatteryProtection2.Instance.Disable();
		});
		int num = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "ServiceReady", 0, RegistryValueKind.DWord);
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "ServiceReady", 0, RegistryValueKind.DWord);
		KeyboardCloseTimer.CreateInstance.Dispose();
		m_MQTTService.Disconnect();
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"));
		LogCtrl.Write("[Application_Exit]");
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Service is Ready : ", ResolvedDelegates.System_Int32_ToString__55(ref num)));
		ReflashTray.RefreshTrayArea();
		if (Arguments.Uninstall)
		{
			LogCtrl.Write("[Application_Exit] Uninstall");
		}
		else if (num != 0)
		{
			LogCtrl.Write("[Application_Exit] Disable MyFan");
			m_MyFan?.DisableByService();
			LogCtrl.Write("[Application_Exit] Disable MyRgbKeyboard");
			if (rgbkeyboard != null)
			{
				rgbkeyboard?.DisableByService();
			}
			if (CustomizeInfo.m_sLightbarType == "2")
			{
				LogCtrl.Write("[Application_Exit] Disable MyRgbLightbar");
				m_RGBLB?.DisableByService();
			}
			LogCtrl.Write("[Application_Exit] Disable MySetting");
			m_MySetting?.DisableByService();
			LogCtrl.Write("[Application_Exit] Disable OSDManager");
			m_Osd?.DisableByService();
			LogCtrl.Write("[Application_Exit] Set_APExistToEC");
			EcCtrl.Set_APExistToEC(bExist: false);
			LogCtrl.Write("[Application_Exit] Unload EC");
			EcCtrl.UnloadDrv();
			StopDiskinfoTimer();
			systemLidEvent?.Dispose();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void InitalizeStartPerparation()
	{
		LogCtrl.Write("[InitalizeStartPerparation]");
		object obj = ResolvedDelegates.System_String_Concat__29;
		string text = "[InitalizeStartPerparation] InitFanTable ";
		DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
		LogCtrl.Write(obj(text, ResolvedDelegates.System_DateTime_ToString__171(ref dateTime)));
		if (m_MyFanTable != null)
		{
			m_MyFanTable.FanTable_Init();
		}
		LogCtrl.Write("[InitalizeStartPerparation] Set_APExistToEC");
		EcCtrl.Set_APExistToEC(bExist: true);
		CustomizeCtrl.Init();
		WKD_AMD.Intel_WiFi6_YB_Recovery();
		WKD_CriticalBatterySetting.HardCodeLevel();
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("[InitalizeStartPerparation] KeyBoardType: ", ResolvedDelegates.System_Object_ToString__1(CustomizeInfo.m_sKeyboardType)));
		if (bDebug)
		{
			CustomizeInfo.m_sLightbarType = "2";
		}
		LogCtrl.Write("[InitalizeStartPerparation] LightbarType: " + ResolvedDelegates.System_Object_ToString__1(CustomizeInfo.m_sLightbarType));
		LogCtrl.Write("[InitalizeStartPerparation] Get ACLineStatus");
		m_PowerModeEvent?.RefreshACLineStatus();
		LogCtrl.Write("[InitalizeStartPerparation] \u958b\u555f WMIReceiveEvent for \u76e3\u807d EC scancode");
		_listener = new OemAcpiWmiListener("AcpiTest_EventULong");
		_listener.Start();
		ScanCodeBus.OnScanCode += m_Wmi.WMIHandleEvent;
		systemLidEvent = SystemLidEvent.CreateInstance;
		systemLidEvent.dispatcher = ((DispatcherObject)this).Dispatcher;
		systemLidEvent.Init();
		m_modernStandby = ModernStandbyEvent.CreateInstance;
		m_modernStandby.dispatcher = ((DispatcherObject)this).Dispatcher;
		m_modernStandby.InitOemPowerManager();
		m_modernStandby.ModernStandbyChanged += ModernStandbyChangedEventHandler;
		LogCtrl.Write("[InitalizeStartPerparation] Realtek Registry clear on fisrt time");
		try
		{
			RealtekWDK instance = RealtekWDK.Instance;
			if (!instance.AuditMode())
			{
				if (instance.GetClearValue() == 1)
				{
					instance.ClearRealtekReg();
					instance.SetclearValue(0);
				}
			}
			else
			{
				instance.ClearRealtekReg();
				instance.SetclearValue(1);
			}
		}
		catch
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<ModernStandbyChangedEventHandler>d__57))]
	private void ModernStandbyChangedEventHandler(object status, EventArgs e)
	{
		<ModernStandbyChangedEventHandler>d__57 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine.status = status;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void RequestModernOn()
	{
		_targetState = ModernTargetState.On;
		TryExecute();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void RequestModernOff()
	{
		_targetState = ModernTargetState.Off;
		TryExecute();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void TryExecute()
	{
		if (ResolvedDelegates.System_Threading_Interlocked_CompareExchange__717(ref _executing, 1, 0) != 0)
		{
			ResolvedDelegates.System_Threading_Interlocked_Exchange__651(ref _pendingRetry, 1);
			LogCtrl.TraceMessageProgramData("_pendingRetry 1", "TryExecute", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1009);
			return;
		}
		Task.Run([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			ModernTargetState targetState = _targetState;
			try
			{
				switch (targetState)
				{
				case ModernTargetState.On:
					ExecuteModernOn();
					break;
				case ModernTargetState.Off:
					ExecuteModernOff();
					break;
				}
			}
			finally
			{
				ResolvedDelegates.System_Threading_Interlocked_Exchange__651(ref _executing, 0);
				if (ResolvedDelegates.System_Threading_Interlocked_Exchange__651(ref _pendingRetry, 0) == 1 || _targetState != targetState)
				{
					TryExecute();
				}
			}
		});
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ExecuteModernOn()
	{
		LogCtrl.TraceMessage("ExecuteModernOn", "ExecuteModernOn", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1042);
		try
		{
			ServiceLoadingCount = 0;
			RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "ServiceLoadingReady", 0, RegistryValueKind.DWord);
			modernON = true;
			if (LCSupport)
			{
				Dispatcher dispatcher = ((DispatcherObject)this).Dispatcher;
				if (dispatcher != null)
				{
					dispatcher.Invoke((Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
					{
						liquid?.ConnectLC();
					}), (DispatcherPriority)9);
				}
			}
			ModernStandbyManager.Instance.ModernStandbyOn();
			ResolvedDelegates.System_Threading_Interlocked_Increment__616(ref ServiceLoadingCount);
			ModernOn();
			ResolvedDelegates.System_Threading_Interlocked_Increment__616(ref ServiceLoadingCount);
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ex.ToString());
		}
		modernON = false;
		if (ServiceLoadingCount == 2)
		{
			RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "ServiceLoadingReady", 1, RegistryValueKind.DWord);
		}
		if (ServiceLoadingCount > 1000)
		{
			ServiceLoadingCount = 3;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ExecuteModernOff()
	{
		LogCtrl.TraceMessage("ExecuteModernOff", "ExecuteModernOff", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1089);
		modernON = false;
		ModernOff();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ModernOff()
	{
		if (ResolvedDelegates.System_Threading_Interlocked_CompareExchange__717(ref _modernExecuting, 1, 0) != 0)
		{
			return;
		}
		try
		{
			if (!EcCtrl.GetProjectIdFromEC().IsProjectId_Commercial())
			{
				ModernStandbyManager.Instance.ModernStandbyOff();
				m_MySystem.StopCPUGPUConbineONOFFTimers();
				ServiceLoadingCount = 0;
				RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "ServiceLoadingReady", 0, RegistryValueKind.DWord);
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			ResolvedDelegates.System_Threading_Interlocked_Exchange__651(ref _modernExecuting, 0);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<ModernOn>d__71))]
	private void ModernOn()
	{
		<ModernOn>d__71 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StartMainWork()
	{
		ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(sw);
		ResolvedDelegates.System_Diagnostics_Stopwatch_Restart__163(sw);
		InitalizeStartPerparation();
		ResolvedDelegates.System_Diagnostics_Stopwatch_Stop__164(sw);
		object obj = ResolvedDelegates.System_String_Concat__107;
		string text = "Perparation: ";
		long num = ResolvedDelegates.System_Diagnostics_Stopwatch_get_ElapsedMilliseconds__155(sw);
		LogCtrl.TraceMessage(obj(text, ResolvedDelegates.System_Int64_ToString__458(ref num), " ms"), "StartMainWork", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1184);
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "PerformanceReady", 0, RegistryValueKind.DWord);
		foreach (string applicationName in ApplicationNames)
		{
			TasksMoniter.Add(applicationName, null);
		}
		ServiceLoadingCount = 0;
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "ServiceLoadingReady", 0, RegistryValueKind.DWord);
		foreach (string applicationName2 in ApplicationNames)
		{
			SuperviseService(applicationName2, ResolvedDelegates.System_Threading_CancellationTokenSource_get_Token__167(_cts));
		}
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "ServiceReady", 1, RegistryValueKind.DWord);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private Task StartUpTask(string ApplicationName, Task taskX)
	{
		if (taskX == null || !ResolvedDelegates.System_Threading_Tasks_Task_get_IsCompleted__718(taskX))
		{
			if (!(ApplicationName == "MyRGBKeyboard"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(ApplicationName, "MySystem"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(ApplicationName, "MyFan"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(ApplicationName, "MySetting"))
						{
							if (ResolvedDelegates.System_String_op_Equality__18(ApplicationName, "MyRgbLightbar"))
							{
								return Task.Run([MethodImpl(MethodImplOptions.NoInlining)] () =>
								{
									if (ResolvedDelegates.System_String_op_Equality__18(CustomizeInfo.m_sLightbarType, "2"))
									{
										LogCtrl.Write("[StartUpTask] Enable MyRgbLightbar");
										LogCtrl.TraceMessageProgramData("[StartUpTask] Enable MyRgbLightbar", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1457);
										m_RGBLB.EnableByService();
									}
									LogCtrl.Write("[FINISH] MYLIGHTBAR ");
									LogCtrl.TraceMessageProgramData("[FINISH] MyLIGHTBAR ", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1461);
									return "Enable MyRgbLight";
								});
							}
							return null;
						}
						return Task.Run([MethodImpl(MethodImplOptions.NoInlining)] () =>
						{
							RealtekWDK.Instance.StartCalibration();
							KeyboardCloseTimer.CreateInstance.SetDispatcher(((DispatcherObject)this).Dispatcher);
							LogCtrl.Write("[StartUpTask] Enable MySetting");
							LogCtrl.TraceMessageProgramData("[StartUpTask] Enable MySetting", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1402);
							m_MySetting.EnableByService();
							LogCtrl.Write("[FINISH] MySETTING ");
							LogCtrl.TraceMessageProgramData("[FINISH] MySETTING ", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1447);
							return "Enable MySetting";
						});
					}
					return Task.Run([MethodImpl(MethodImplOptions.NoInlining)] () =>
					{
						LogCtrl.Write("[StartUpTask] Enable MyFan");
						LogCtrl.TraceMessageProgramData("[StartUpTask] Enable MyFan", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1356);
						m_MyFan.EnableByService(((DispatcherObject)this).Dispatcher);
						LCSupport = EcCtrl.IsSupportLiquidCooling();
						if (LCSupport)
						{
							try
							{
								Dispatcher dispatcher = ((DispatcherObject)this).Dispatcher;
								if (dispatcher != null)
								{
									dispatcher.Invoke((Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
									{
										LogCtrl.Write("[StartUpTask] new LiquidCooling");
										liquid = new LiquidCooling();
									}), (DispatcherPriority)9);
								}
							}
							catch (Exception ex)
							{
								LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("[StartUpTask] new LiquidCooling Exception \uff1a", ResolvedDelegates.System_Exception_get_Message__58(ex)));
							}
						}
						LogCtrl.Write("[finish] myfan ");
						LogCtrl.TraceMessageProgramData("[finish] myfan ", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1378);
						return "Enable MyFan";
					});
				}
				return Task.Run([MethodImpl(MethodImplOptions.NoInlining)] () =>
				{
					m_MQTTService.ClientMessage += M_MQTTService_ClientMessage1;
					_ = ProcessControl.Instance;
					LogCtrl.Write("[StartUpTask] Enable MySystem");
					LogCtrl.TraceMessageProgramData("[StartUpTask] Enable MySystem", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1334);
					m_MySystem.DisableByService();
					LogCtrl.Write("[StartUpTask] Enable BatteryProtection2");
					LogCtrl.TraceMessageProgramData("[StartUpTask] Enable BatteryProtection2", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1343);
					BatteryProtection2.Instance.EnableByService();
					LogCtrl.Write("[FINISH] MySystem ");
					LogCtrl.TraceMessageProgramData("[FINISH] MySystem ", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1347);
					return "Enable MySystem";
				});
			}
			return Task.Run([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				LogCtrl.Write("[StartUpTask] Enable RGBLightKeyboard ");
				LogCtrl.TraceMessageProgramData("[StartUpTask] Enable RGBLightKeyboard ", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1300);
				rgbkeyboard.init();
				rgbkeyboard.EnableByService();
				hidrgblightbar.init(detectpin: true);
				hidrgblightbar.EnableByService();
				hidrgblightbarlogo.init();
				hidrgblightbarlogo.EnableByService();
				hidrgblightbarhinge.init();
				hidrgblightbarhinge.EnableByService();
				hidrgblightbarsync.init();
				hidrgblightbarsync.EnableByService();
				ModernStandbyManager.Instance.Init();
				CheckAurora.Instance.SetDispatcher(((DispatcherObject)this).Dispatcher);
				LogCtrl.Write("[FINISH] Enable RGBLightKeyboard ");
				LogCtrl.TraceMessageProgramData("[FINISH Enable RGBLightKeyboard ", "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1323);
				return "Enable MyRGBKeyboard";
			});
		}
		if (taskX.Exception != null)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29(ApplicationName, " Task is restart"));
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("[StartUpTask] task exception ", ResolvedDelegates.System_Object_ToString__1(ResolvedDelegates.System_Threading_Tasks_Task_get_Exception__719(taskX))));
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("[StartUpTask] task exception ", ResolvedDelegates.System_Object_ToString__1(ResolvedDelegates.System_Threading_Tasks_Task_get_Exception__719(taskX))), "StartUpTask", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1495);
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__107("[StartUpTask] ", ApplicationName, " Task is restart "));
			ResolvedDelegates.System_Threading_Tasks_Task_Dispose__720(taskX);
			taskX = null;
		}
		else
		{
			Console.WriteLine("***** {0} still running *****", ApplicationName);
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void M_MQTTService_ClientMessage1(string topic, MqttMsgPublishEventArgs e)
	{
		if (ResolvedDelegates.System_String_op_Equality__18(topic, "Power/Shutdown"))
		{
			LogCtrl.Write("Power Shutdown");
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void KillProcess(string processName)
	{
		try
		{
			Process[] array = ResolvedDelegates.System_Diagnostics_Process_GetProcessesByName__85(processName);
			for (int i = 0; i < array.Count(); i++)
			{
				array[i].Kill();
				LogCtrl.Write(ResolvedDelegates.System_String_Format__7("KillProcess {0}", processName));
			}
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Format__84("KillProcess {0} Failed {1}", processName, ResolvedDelegates.System_Object_ToString__1(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<ServiceRecieve>d__78))]
	public static void ServiceRecieve(byte[] data)
	{
		<ServiceRecieve>d__78 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.data = data;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void FirstTimeDefaultTool()
	{
		int projectIdFromEC = EcCtrl.GetProjectIdFromEC();
		CustomizeInfo.m_ProjectID = ResolvedDelegates.System_Int32_ToString__55(ref projectIdFromEC);
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("m_ProjectID: ", CustomizeInfo.m_ProjectID));
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2", "ProjectID", CustomizeInfo.m_ProjectID, RegistryValueKind.DWord);
		try
		{
			m_FirstTime = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2", "FirstTime", 1);
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("m_FirstTime: ", ResolvedDelegates.System_Int32_ToString__55(ref m_FirstTime)));
			if (m_FirstTime == 1)
			{
				m_FirstTime = 0;
				RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2", "FirstTime", m_FirstTime, RegistryValueKind.DWord);
				int customizeTarget = RegistryCtrl.GetCustomizeTarget();
				string text = ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_IO_FileSystemInfo_get_FullName__275(ResolvedDelegates.System_IO_DirectoryInfo_get_Parent__721(ResolvedDelegates.System_IO_DirectoryInfo_get_Parent__721(ResolvedDelegates.System_IO_FileInfo_get_Directory__372(new FileInfo(ResolvedDelegates.System_Reflection_Assembly_get_Location__97(ResolvedDelegates.System_Reflection_Assembly_GetExecutingAssembly__96())))))), "\\DefaultTool");
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("DefaultToolDir : ", text));
				CustomizeCtrl.ImportRGBKeyboardRegistryIfFirstLaunch();
				object obj = ResolvedDelegates.System_Diagnostics_Process_Start__103;
				Process process = new Process();
				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				ResolvedDelegates.System_Diagnostics_ProcessStartInfo_set_WindowStyle__91(processStartInfo, ProcessWindowStyle.Hidden);
				ResolvedDelegates.System_Diagnostics_ProcessStartInfo_set_FileName__92(processStartInfo, ResolvedDelegates.System_String_Concat__29(text, "\\DefaultTool.exe"));
				process.StartInfo = processStartInfo;
				ResolvedDelegates.System_Diagnostics_ProcessStartInfo_set_Arguments__94(processStartInfo, ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_Int32_ToString__55(ref customizeTarget), " -i"));
				ResolvedDelegates.System_Diagnostics_ProcessStartInfo_set_RedirectStandardOutput__95(processStartInfo, true);
				ResolvedDelegates.System_Diagnostics_ProcessStartInfo_set_WorkingDirectory__99(processStartInfo, text);
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				obj(process);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage("First time DefaultTool exception: " + ex, "FirstTimeDefaultTool", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1769);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe void SetUpLoopback()
	{
		string text = ResolvedDelegates.System_String_Split__221(ref *(string*)ResolvedDelegates.System_Runtime_InteropServices_RuntimeInformation_get_OSDescription__722(), new char[1] { ' ' })[2];
		string text2 = (string)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2", "WinVer", null, RegistryValueKind.String);
		if (text2 == null)
		{
			RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2", "WinVer", text, RegistryValueKind.String);
		}
		else
		{
			if (text2 == null || ResolvedDelegates.System_String_Equals__445(ref *(string*)text, text2))
			{
				return;
			}
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\Repository\\Packages", writable: false);
			try
			{
				string[] subKeyNames = registryKey.GetSubKeyNames();
				foreach (string name in subKeyNames)
				{
					string text3 = registryKey.OpenSubKey(name).GetValue("PackageID") as string;
					if (!ResolvedDelegates.System_String_IsNullOrEmpty__172(text3))
					{
						gInstalledSoftware.Add(text3);
					}
				}
			}
			finally
			{
				if (registryKey != null)
				{
					ResolvedDelegates.System_IDisposable_Dispose__3(registryKey);
				}
			}
			string text4 = "";
			foreach (string item in gInstalledSoftware)
			{
				if (ResolvedDelegates.System_String_Contains__108(ref *(string*)ResolvedDelegates.System_String_ToLower__481(ref *(string*)item), "controlcenter3"))
				{
					text4 = ResolvedDelegates.System_String_Concat__29("ControlCenter3", ResolvedDelegates.System_String_Substring__352(ref *(string*)item, ResolvedDelegates.System_String_LastIndexOf__723(ref *(string*)item, '_')));
				}
			}
			PowerShell val = PowerShell.Create();
			try
			{
				string text5 = ResolvedDelegates.System_String_Concat__29("CheckNetIsolation.exe LoopbackExempt -a -n=", text4);
				ResolvedDelegates.System_Management_Automation_PowerShell_AddScript__724(val, text5);
				ResolvedDelegates.System_Management_Automation_PowerShell_Invoke__15(val);
			}
			finally
			{
				if (val != null)
				{
					ResolvedDelegates.System_IDisposable_Dispose__3(val);
				}
			}
			RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2", "WinVer", text, RegistryValueKind.String);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void LaunchAllPlugins()
	{
		try
		{
			string text = "\\OEM\\Plugin\\";
			string text2 = (string)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, text, "Path", "");
			string[] array = RegistryCtrl.RegistrySoftwareKeyGet(RegistryHive.LocalMachine, text);
			foreach (string text3 in array)
			{
				string text4 = ResolvedDelegates.System_String_Concat__29((string)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__107(text, "\\", text3), "AppName", ""), ".exe");
				if (ResolvedDelegates.System_IO_File_Exists__72(ResolvedDelegates.System_String_Concat__79(new string[5]
				{
					text2,
					"\\",
					ResolvedDelegates.System_Object_ToString__1(text3),
					"\\",
					text4
				})))
				{
					Process[] array2 = ResolvedDelegates.System_Diagnostics_Process_GetProcessesByName__85(ResolvedDelegates.System_Object_ToString__1(text3));
					for (int j = 0; j < array2.Count(); j++)
					{
						array2[j].Kill();
					}
					try
					{
						ResolvedDelegates.System_Diagnostics_Process_Start__307(new ProcessStartInfo
						{
							WindowStyle = ProcessWindowStyle.Hidden,
							FileName = text4,
							WorkingDirectory = ResolvedDelegates.System_String_Concat__107(text2, "\\", ResolvedDelegates.System_Object_ToString__1(text3))
						});
					}
					catch (Exception ex)
					{
						ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("LaunchAllPlugins | Process Start Exception: ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
					}
				}
				else
				{
					Console.WriteLine("LaunchAllPlugins | " + text4 + " is not found.");
				}
			}
		}
		catch (Exception ex2)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("LaunchAllPlugins | RegistrySoftwareKeyGet Exception: ", ResolvedDelegates.System_Exception_get_Message__58(ex2)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe bool IsSTPORKK()
	{
		string text = ResolvedDelegates.System_Environment_GetEnvironmentVariable__612("PROCESSOR_IDENTIFIER");
		LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("processorId : ", text), "IsSTPORKK", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1869);
		bool flag = false;
		bool flag2 = false;
		if (ResolvedDelegates.System_String_Contains__108(ref *(string*)ResolvedDelegates.System_String_ToUpper__331(ref *(string*)text), "FAMILY"))
		{
			if (ResolvedDelegates.System_String_Contains__108(ref *(string*)text, "26") && ResolvedDelegates.System_String_Contains__108(ref *(string*)ResolvedDelegates.System_String_ToLowerInvariant__452(ref *(string*)text), "model 96"))
			{
				flag = true;
				LogCtrl.Write("[Set_GamePerformancePowerProfileSettings] cputype: KRK");
			}
			else if (text.Contains("26") && (ResolvedDelegates.System_String_Contains__108(ref *(string*)ResolvedDelegates.System_String_ToLowerInvariant__452(ref *(string*)text), "model 32") || ResolvedDelegates.System_String_Contains__108(ref *(string*)ResolvedDelegates.System_String_ToLowerInvariant__452(ref *(string*)text), "model 36")))
			{
				flag2 = true;
				LogCtrl.Write("[Set_GamePerformancePowerProfileSettings] cputype: STX");
			}
		}
		return flag | flag2;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool IsGPT()
	{
		string text = ResolvedDelegates.System_Environment_GetEnvironmentVariable__612("PROCESSOR_IDENTIFIER") ?? "";
		LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("processorId : ", text), "IsGPT", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\App.xaml.cs", 1891);
		int num;
		if (ResolvedDelegates.System_Text_RegularExpressions_Regex_IsMatch__725(text, "\\bFamily\\s+26\\b", RegexOptions.IgnoreCase))
		{
			num = (ResolvedDelegates.System_Text_RegularExpressions_Regex_IsMatch__725(text, "\\bModel\\s+(36|96)\\b", RegexOptions.IgnoreCase) ? 1 : 0);
			if (num != 0)
			{
				LogCtrl.Write("[Set_GamePerformancePowerProfileSettings] cputype: GPT");
			}
		}
		else
		{
			num = 0;
		}
		return (byte)num != 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<SuperviseService>d__85))]
	private Task SuperviseService(string appName, CancellationToken ct)
	{
		<SuperviseService>d__85 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_Create__20();
		stateMachine.<>4__this = this;
		stateMachine.appName = appName;
		stateMachine.ct = ct;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
		return ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_get_Task__21(ref stateMachine.<>t__builder);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Cancel()
	{
		ResolvedDelegates.System_Threading_CancellationTokenSource_Cancel__425(_cts);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected Obj, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected Obj, but got Unknown
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			((Application)this).Startup += Application_Startup;
			((Application)this).Exit += Application_Exit;
			Uri uri = new Uri("/GCUService;component/app.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[STAThread]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public static void Main()
	{
		if (1 == 0)
		{
			_ = -1;
		}
		if (1 == 0)
		{
			_ = 5;
		}
		if (1 == 0)
		{
			_ = 7;
		}
		MethodBase? currentMethod = MethodBase.GetCurrentMethod();
		if (1 == 0)
		{
			_ = -1;
		}
		if (1 == 0)
		{
			_ = 1;
		}
		if (1 == 0)
		{
			_ = 8;
		}
		if (false)
		{
			_ = 3;
		}
		\u0004\u0004\u0007\u0006\u0009\u0002\u0002\u0005.\u0010\u0005\u0005\u0010\u000f\u0014 obj = \u0004\u0004\u0007\u0006\u0009\u0002\u0002\u0005.\u000e\u0006\u0005\u0005\u0007\u0001\u0003\u0003(null, currentMethod, 7636256u, 19, 0u, 0u, 0u, 8010312u, 5);
		object[] array = new object[0];
		if (false)
		{
			_ = 0;
		}
		if (1 == 0)
		{
			_ = 7;
		}
		object[] array2 = array;
		obj.\u0003\u0006\u0001\u0003\u000b\u0006\u000b\u000a(null, array);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public App()
	{
	}
}

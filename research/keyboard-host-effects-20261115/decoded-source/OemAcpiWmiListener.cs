// Static analysis view; decoded strings and delegate names; not executable OEM code.
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Utility;

namespace GCUService.ScanCode;

public class OemAcpiWmiListener : IDisposable
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <<StartHealthMonitor>b__17_0>d : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public OemAcpiWmiListener <>4__this;

		private TaskAwaiter <>u__1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			int num = <>1__state;
			OemAcpiWmiListener oemAcpiWmiListener = <>4__this;
			try
			{
				if (num == 0)
				{
					goto IL_0015;
				}
				goto IL_01b0;
				IL_0015:
				try
				{
					TaskAwaiter awaiter;
					if (num != 0)
					{
						awaiter = ResolvedDelegates.System_Threading_Tasks_Task_GetAwaiter__23(ResolvedDelegates.System_Threading_Tasks_Task_Delay__437(ResolvedDelegates.System_TimeSpan_FromSeconds__430(3.0), ResolvedDelegates.System_Threading_CancellationTokenSource_get_Token__167(oemAcpiWmiListener._cts)));
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
					if (!oemAcpiWmiListener.IsWmiServiceRunning())
					{
						LogCtrl.Write("WMI service not running. Waiting...");
					}
					else
					{
						TimeSpan timeSpan = DateTime.Now - oemAcpiWmiListener._lastEventTime;
						if (ResolvedDelegates.System_TimeSpan_get_TotalMinutes__438(ref timeSpan) > 5.0)
						{
							LogCtrl.Write("No event for 5 min. Doing lightweight probe...");
							oemAcpiWmiListener._lastEventTime = ResolvedDelegates.System_DateTime_get_Now__170();
							if (!oemAcpiWmiListener.ProbeWmi())
							{
								LogCtrl.Write("Probe failed. Restarting watcher...");
								oemAcpiWmiListener.Restart();
							}
						}
						if (!oemAcpiWmiListener._probeAlive)
						{
							LogCtrl.Write("WMI event pipeline dead. Restarting watcher...");
							oemAcpiWmiListener.Restart();
						}
						oemAcpiWmiListener._probeAlive = false;
					}
				}
				catch (Exception ex)
				{
					LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Health monitor error: ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
				}
				goto IL_01b0;
				IL_01b0:
				CancellationToken token = oemAcpiWmiListener._cts.Token;
				if (!ResolvedDelegates.System_Threading_CancellationToken_get_IsCancellationRequested__180(ref token))
				{
					goto IL_0015;
				}
			}
			catch (Exception ex2)
			{
				<>1__state = -2;
				ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_SetException__26(ref <>t__builder, ex2);
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <Restart>d__26 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public OemAcpiWmiListener <>4__this;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			int num = <>1__state;
			OemAcpiWmiListener oemAcpiWmiListener = <>4__this;
			try
			{
				object obj = oemAcpiWmiListener._lock;
				bool flag = false;
				try
				{
					ResolvedDelegates.System_Threading_Monitor_Enter__70(obj, ref flag);
					oemAcpiWmiListener.StopInternal();
					oemAcpiWmiListener._queue = new BlockingCollection<int>(128);
					oemAcpiWmiListener._cts = new CancellationTokenSource();
					oemAcpiWmiListener.StartWatcher();
					oemAcpiWmiListener.StartWorker();
					oemAcpiWmiListener.StartProbeWatcher();
				}
				finally
				{
					if (num < 0 && flag)
					{
						ResolvedDelegates.System_Threading_Monitor_Exit__71(obj);
					}
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

	private readonly string _ns = "root\\wmi";

	private readonly string _query;

	private ManagementEventWatcher _watcher;

	private BlockingCollection<int> _queue;

	private CancellationTokenSource _cts;

	private Task _healthTask;

	private DateTime _lastEventTime = ResolvedDelegates.System_DateTime_get_Now__170();

	private readonly object _lock = new object();

	private Task _worker;

	private int _generation;

	private volatile bool _probeAlive;

	private ManagementEventWatcher _probeWatcher;

	private readonly object _probeLock = new object();

	[MethodImpl(MethodImplOptions.NoInlining)]
	public OemAcpiWmiListener(string eventClass)
	{
		ResolvedDelegates.System_Object__ctor__0(this);
		_query = ResolvedDelegates.System_String_Concat__29("SELECT * FROM ", eventClass);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Start()
	{
		object obj = _lock;
		bool flag = false;
		try
		{
			ResolvedDelegates.System_Threading_Monitor_Enter__70(obj, ref flag);
			StopInternal();
			_queue = new BlockingCollection<int>(128);
			_cts = new CancellationTokenSource();
			StartWatcher();
			StartWorker();
			StartHealthMonitor();
			StartProbeWatcher();
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
	private void StartWorker()
	{
		_worker = ResolvedDelegates.System_Threading_Tasks_Task_Run__10([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			IEnumerator<int> enumerator = _queue.GetConsumingEnumerable(ResolvedDelegates.System_Threading_CancellationTokenSource_get_Token__167(_cts)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					int current = enumerator.Current;
					try
					{
						ScanCodeBus.Publish(current);
					}
					catch (Exception ex)
					{
						LogCtrl.Write("[WMI] Worker exception: " + ex);
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
		});
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Stop()
	{
		object obj = _lock;
		bool flag = false;
		try
		{
			ResolvedDelegates.System_Threading_Monitor_Enter__70(obj, ref flag);
			try
			{
				if (_watcher != null)
				{
					try
					{
						ResolvedDelegates.System_Management_ManagementEventWatcher_Stop__197(_watcher);
					}
					catch
					{
					}
					try
					{
						ResolvedDelegates.System_ComponentModel_Component_Dispose__198(_watcher);
					}
					catch
					{
					}
					_watcher = null;
				}
			}
			catch
			{
			}
			try
			{
				_queue?.CompleteAdding();
			}
			catch
			{
			}
			try
			{
				_cts?.Cancel();
			}
			catch
			{
			}
		}
		finally
		{
			if (flag)
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(obj);
			}
		}
		try
		{
			_worker?.Wait(1000);
		}
		catch
		{
		}
		LogCtrl.Write("WMI stopped safely.");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StartWatcher()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected Obj, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected Obj, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected Obj, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected Obj, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected Obj, but got Unknown
		object obj = _lock;
		bool flag = false;
		try
		{
			ResolvedDelegates.System_Threading_Monitor_Enter__70(obj, ref flag);
			try
			{
				ManagementScope val = new ManagementScope(_ns);
				ResolvedDelegates.System_Management_ConnectionOptions_set_EnablePrivileges__427(ResolvedDelegates.System_Management_ManagementScope_get_Options__426(val), true);
				ResolvedDelegates.System_Management_ManagementScope_Connect__319(val);
				_watcher = new ManagementEventWatcher(val, new EventQuery(_query));
				ResolvedDelegates.System_Management_ManagementEventWatcher_add_EventArrived__56(_watcher, OnEvent);
				ResolvedDelegates.System_Management_ManagementEventWatcher_add_Stopped__428(_watcher, OnStopped);
				ResolvedDelegates.System_Management_ManagementEventWatcher_Start__57(_watcher);
				LogCtrl.Write("WMI Watcher started.");
			}
			catch (Exception ex)
			{
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("StartWatcher failed: ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
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
	private void OnEvent(object sender, EventArrivedEventArgs e)
	{
		_lastEventTime = ResolvedDelegates.System_DateTime_get_Now__170();
		try
		{
			int item = ParseScanCode(e);
			if (!_queue.TryAdd(item))
			{
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("[WMI] Queue full, drop scancode ", ResolvedDelegates.System_Int32_ToString__55(ref item)));
			}
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("OnEvent error: ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void OnStopped(object sender, StoppedEventArgs e)
	{
		LogCtrl.Write("Watcher stopped. Restarting...");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StartHealthMonitor()
	{
		_healthTask = ResolvedDelegates.System_Threading_Tasks_Task_Run__11([MethodImpl(MethodImplOptions.NoInlining)] [AsyncStateMachine(typeof(<<StartHealthMonitor>b__17_0>d))] () =>
		{
			<<StartHealthMonitor>b__17_0>d stateMachine = default;
			stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_Create__20();
			stateMachine.<>4__this = this;
			stateMachine.<>1__state = -1;
			stateMachine.<>t__builder.Start(ref stateMachine);
			return ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_get_Task__21(ref stateMachine.<>t__builder);
		});
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StartProbeWatcher()
	{
		ResolvedDelegates.System_Threading_Tasks_Task_Run__10([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected Obj, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected Obj, but got Unknown
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected Obj, but got Unknown
			StopProbeWatcher();
			object probeLock = _probeLock;
			bool flag = false;
			try
			{
				ResolvedDelegates.System_Threading_Monitor_Enter__70(probeLock, ref flag);
				try
				{
					_probeWatcher = new ManagementEventWatcher((EventQuery)new WqlEventQuery("__InstanceModificationEvent", ResolvedDelegates.System_TimeSpan_FromSeconds__430(1.0), "TargetInstance ISA 'Win32_LocalTime'"));
					ResolvedDelegates.System_Management_ManagementEventWatcher_add_EventArrived__56(_probeWatcher, ProbeArrived);
					ResolvedDelegates.System_Management_ManagementEventWatcher_Start__57(_probeWatcher);
					LogCtrl.Write("ProbeWatcher started.");
				}
				catch (Exception ex)
				{
					LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("ProbeWatcher failed: ", ResolvedDelegates.System_Exception_get_Message__58(ex)));
				}
			}
			finally
			{
				if (flag)
				{
					ResolvedDelegates.System_Threading_Monitor_Exit__71(probeLock);
				}
			}
		});
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StopProbeWatcher()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected Obj, but got Unknown
		object probeLock = _probeLock;
		bool flag = false;
		try
		{
			ResolvedDelegates.System_Threading_Monitor_Enter__70(probeLock, ref flag);
			try
			{
				if (_probeWatcher != null)
				{
					ResolvedDelegates.System_Management_ManagementEventWatcher_remove_EventArrived__429(_probeWatcher, ProbeArrived);
					try
					{
						ResolvedDelegates.System_Management_ManagementEventWatcher_Stop__197(_probeWatcher);
					}
					catch
					{
					}
					try
					{
						ResolvedDelegates.System_ComponentModel_Component_Dispose__198(_probeWatcher);
					}
					catch
					{
					}
					_probeWatcher = null;
				}
			}
			catch
			{
			}
		}
		finally
		{
			if (flag)
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(probeLock);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ProbeArrived(object sender, EventArrivedEventArgs e)
	{
		_probeAlive = true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool ProbeWmi()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected Obj, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected Obj, but got Unknown
		//IL_008a: Expected Obj, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected Obj, but got Unknown
		//IL_00dd: Expected Obj, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ManagementScope val = new ManagementScope("\\\\.\\root\\wmi");
			((ManagementOptions)val.Options).Timeout = ResolvedDelegates.System_TimeSpan_FromSeconds__430(3.0);
			val.Connect();
			ObjectQuery val2 = new ObjectQuery("SELECT * FROM AcpiTest_MULong");
			EnumerationOptions val3 = new EnumerationOptions
			{
				ReturnImmediately = true,
				Rewindable = false,
				Timeout = ResolvedDelegates.System_TimeSpan_FromSeconds__430(3.0),
				DirectRead = true
			};
			ManagementObjectSearcher val4 = new ManagementObjectSearcher(val, val2, val3);
			try
			{
				ManagementObjectCollection val5 = ResolvedDelegates.System_Management_ManagementObjectSearcher_Get__199(val4);
				try
				{
					((IEnumerable)val5).Cast<ManagementObject>().FirstOrDefault();
					return true;
				}
				finally
				{
					if (val5 != null)
					{
						ResolvedDelegates.System_IDisposable_Dispose__3(val5);
					}
				}
			}
			finally
			{
				if (val4 != null)
				{
					ResolvedDelegates.System_IDisposable_Dispose__3(val4);
				}
			}
		}
		catch (ManagementException ex)
		{
			ManagementException ex2 = ex;
			ResolvedDelegates.System_String_Format__84("ManagementException: {0} / 0x{1:X}", ResolvedDelegates.System_Management_ManagementException_get_ErrorCode__435(ex2), ResolvedDelegates.System_Management_ManagementException_get_ErrorCode__435(ex2));
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Probe ManagementException: ", ((object)ResolvedDelegates.System_Management_ManagementException_get_ErrorCode__435(ex2)/*cast due to constrained. prefix*/).ToString()));
			ResolvedDelegates.System_Management_ManagementException_get_ErrorCode__435(ex2);
			_ = -2147217358;
			return false;
		}
		catch (Exception ex3)
		{
			ResolvedDelegates.System_Exception_get_Message__58(ex3);
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Probe error: ", ResolvedDelegates.System_Exception_get_Message__58(ex3)));
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool IsWmiServiceRunning()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		try
		{
			ServiceController val = new ServiceController("winmgmt");
			try
			{
				return (int)ResolvedDelegates.System_ServiceProcess_ServiceController_get_Status__436(val) == 4;
			}
			finally
			{
				if (val != null)
				{
					ResolvedDelegates.System_IDisposable_Dispose__3(val);
				}
			}
		}
		catch
		{
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<Restart>d__26))]
	private void Restart()
	{
		<Restart>d__26 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StopInternal()
	{
		try
		{
			ManagementEventWatcher watcher = _watcher;
			if (watcher != null)
			{
				watcher.Stop();
			}
			((Component)(object)_watcher)?.Dispose();
			_watcher = null;
		}
		catch
		{
		}
		_cts?.Cancel();
		_queue?.CompleteAdding();
		_queue = new BlockingCollection<int>(128);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Dispose()
	{
		Stop();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int ParseScanCode(EventArrivedEventArgs e)
	{
		return ResolvedDelegates.System_Convert_ToInt32__54(ResolvedDelegates.System_Object_ToString__1(ResolvedDelegates.System_Management_PropertyData_get_Value__53(ResolvedDelegates.System_Management_PropertyDataCollection_get_Item__52(ResolvedDelegates.System_Management_ManagementBaseObject_get_SystemProperties__51(ResolvedDelegates.System_Management_EventArrivedEventArgs_get_NewEvent__50(e)), "ULong"))));
	}
}

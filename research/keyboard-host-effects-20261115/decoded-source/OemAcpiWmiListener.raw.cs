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
						awaiter = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[23](\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[437](\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[430](3.0), \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[167](oemAcpiWmiListener._cts)));
						if (!\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[24](ref awaiter))
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
						LogCtrl.Write(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\"+&N\ufffa\u000f\ufff9￼\0\u0005\nF\uffff\ufffd\ufff7B\uffef\ufff3\ufff9\ufff8\ufff4\ufff6￼49\t\ufff6￭￩￫￭\ufff3+*)"));
					}
					else
					{
						TimeSpan timeSpan = DateTime.Now - oemAcpiWmiListener._lastEventTime;
						if (\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[438](ref timeSpan) > 5.0)
						{
							LogCtrl.Write(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017(".\u000eJ\u0006\ufff2\u0004\b\ufff1D\ufffd\u0005\uffef@,>\u0002\u0005\ufffd<9\u0014\ufffa\uffff\ufff7\uffef3\ufff6\ufffa￫\ufff7ￚ\uffd8￩\ufff4￥\ufff1ￔ'ￖￓ\uffe7￡\uffdf#\"!"));
							oemAcpiWmiListener._lastEventTime = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[170]();
							if (!oemAcpiWmiListener.ProbeWmi())
							{
								LogCtrl.Write(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("!\ufffe\b\f\u0010L\r\t\0\u0004\n\n?D\u0011\u0005￮\ufff4\ufffe￬\ufff1\ufff3\ufff5\ufffb9￩\ufff6￪\ufff2￬\ufff6￠+*)"));
								oemAcpiWmiListener.Restart();
							}
						}
						if (!oemAcpiWmiListener._probeAlive)
						{
							LogCtrl.Write(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u001703I\u0005\ufff1\u0003\a\ufff0C\ufff2\n\ufff0￼\u0002\u0006\ufffe\ufff8:\ufff5\ufff5\ufff8\ufff274\u0001\uffef￠ￜ\ufff0ￜ\uffd9\ufff5￭￥)ￓ￨ￒ￤￬￠\uffd0#\"!"));
							oemAcpiWmiListener.Restart();
						}
						oemAcpiWmiListener._probeAlive = false;
					}
				}
				catch (Exception ex)
				{
					LogCtrl.Write(\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[29](\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("￼ #\u0015,\u0017^\u0012\u000f\r\u0013%\v%V\u0012\"!\u0005\u001fVO"), \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[58](ex)));
				}
				goto IL_01b0;
				IL_01b0:
				CancellationToken token = oemAcpiWmiListener._cts.Token;
				if (!\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[180](ref token))
				{
					goto IL_0015;
				}
			}
			catch (Exception ex2)
			{
				<>1__state = -2;
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[26](ref <>t__builder, ex2);
				return;
			}
			<>1__state = -2;
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[27](ref <>t__builder);
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
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[28](ref <>t__builder, stateMachine);
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
					\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[70](obj, ref flag);
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
						\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[71](obj);
					}
				}
			}
			catch (Exception ex)
			{
				<>1__state = -2;
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[194](ref <>t__builder, ex);
				return;
			}
			<>1__state = -2;
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[195](ref <>t__builder);
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
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[196](ref <>t__builder, stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	private readonly string _ns = \u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u001843\u0017\ufffe\u0018-(");

	private readonly string _query;

	private ManagementEventWatcher _watcher;

	private BlockingCollection<int> _queue;

	private CancellationTokenSource _cts;

	private Task _healthTask;

	private DateTime _lastEventTime = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[170]();

	private readonly object _lock = new object();

	private Task _worker;

	private int _generation;

	private volatile bool _probeAlive;

	private ManagementEventWatcher _probeWatcher;

	private readonly object _probeLock = new object();

	[MethodImpl(MethodImplOptions.NoInlining)]
	public OemAcpiWmiListener(string eventClass)
	{
		\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[0](this);
		_query = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[29](\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\ufffb\b\u000e\u0006\a\ufff3fkd\ufffd\ufff0\u0004\u0005_"), eventClass);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Start()
	{
		object obj = _lock;
		bool flag = false;
		try
		{
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[70](obj, ref flag);
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
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[71](obj);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StartWorker()
	{
		_worker = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[10]([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			IEnumerator<int> enumerator = _queue.GetConsumingEnumerable(\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[167](_cts)).GetEnumerator();
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
						LogCtrl.Write(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u0011\f\u0001￼\u000fQ\a\u001e \u0018\u0011\u001dJ\u000e \n\v\u0015\u0018\f\u0011\u000fZ?") + ex);
					}
				}
			}
			finally
			{
				if (enumerator != null)
				{
					\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[3](enumerator);
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
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[70](obj, ref flag);
			try
			{
				if (_watcher != null)
				{
					try
					{
						\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[197](_watcher);
					}
					catch
					{
					}
					try
					{
						\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[198](_watcher);
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
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[71](obj);
			}
		}
		try
		{
			_worker?.Wait(1000);
		}
		catch
		{
		}
		LogCtrl.Write(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u0012\ufffb\ufff6^*0\u0014*)\u001b\u001bV\"\u0013\u0015\u0015\r\u0017I"));
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
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[70](obj, ref flag);
			try
			{
				ManagementScope val = new ManagementScope(_ns);
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[427](\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[426](val), true);
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[319](val);
				_watcher = new ManagementEventWatcher(val, new EventQuery(_query));
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[56](_watcher, OnEvent);
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[428](_watcher, OnStopped);
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[57](_watcher);
				LogCtrl.Write(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u0011\ufff6\ufff9_\r\u001e(\u001e\u0012\u0016*W)!\u0015%\u001e\u000e\fE"));
			}
			catch (Exception ex)
			{
				LogCtrl.Write(\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[29](\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u0014. 2+\v\u001c(\u001c\u0012\u0014*W\u0014\u0014\v\a\r\rZO"), \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[58](ex)));
			}
		}
		finally
		{
			if (flag)
			{
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[71](obj);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void OnEvent(object sender, EventArrivedEventArgs e)
	{
		_lastEventTime = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[170]();
		try
		{
			int item = ParseScanCode(e);
			if (!_queue.TryAdd(item))
			{
				LogCtrl.Write(\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[29](\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u0019$)$\u0017I\u0019￼\v\ufffa\tC\b\ufff6￼\ufffb:=\0￭\ufff9￩8￪\ufff9\ufff6\ufff2\ufff6\ufff1\ufff5\ufff5/"), \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref item)));
			}
		}
		catch (Exception ex)
		{
			LogCtrl.Write(\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[29](\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u000e.\u0006\u0014$*\u0013f \u0012\u0011#\u000fV_"), \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[58](ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void OnStopped(object sender, StoppedEventArgs e)
	{
		LogCtrl.Write(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\a\u001c&\u0018 \u0014$U#\u001f\u0015! \f\nOL\ufff9\a\u0018\u0014\b\u0014\u0011\r\u0005\ufffdCBA"));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StartHealthMonitor()
	{
		_healthTask = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[11]([MethodImpl(MethodImplOptions.NoInlining)] [AsyncStateMachine(typeof(<<StartHealthMonitor>b__17_0>d))] () =>
		{
			<<StartHealthMonitor>b__17_0>d stateMachine = default;
			stateMachine.<>t__builder = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[20]();
			stateMachine.<>4__this = this;
			stateMachine.<>1__state = -1;
			stateMachine.<>t__builder.Start(ref stateMachine);
			return \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[21](ref stateMachine.<>t__builder);
		});
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StartProbeWatcher()
	{
		\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[10]([MethodImpl(MethodImplOptions.NoInlining)] () =>
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
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[70](probeLock, ref flag);
				try
				{
					_probeWatcher = new ManagementEventWatcher((EventQuery)new WqlEventQuery(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u0012\u0011\ufffe \"(\u0012\u001c\u000e\u0013\ufffa\u0017\u0011\u0013\r\u0011\u0006\a\u001b\r\u000e\u000e￦\u0014\u0004\n\u0013"), \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[430](1.0), \u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u001e\u0012\u0002\u000e\v\ufff9%\u0001\ufffd\ufff5\t\ufffd\t\u0002D\u001c\u0015\"@>\r\ufff6\ufff2.,\0\f￮\ufff9\ufff6￨\uffff￫￦￭.")));
					\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[56](_probeWatcher, ProbeArrived);
					\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[57](_probeWatcher);
					LogCtrl.Write(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u00134\u0016\"\u001a\v\u001c(\u001c\u0012\u0014*W'!\u0013%\u001e\f\fE"));
				}
				catch (Exception ex)
				{
					LogCtrl.Write(\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[29](\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u00134\u0016\"\u001a\v\u001c(\u001c\u0012\u0014*W\u0014\u0014\v\a\r\rZO"), \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[58](ex)));
				}
			}
			finally
			{
				if (flag)
				{
					\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[71](probeLock);
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
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[70](probeLock, ref flag);
			try
			{
				if (_probeWatcher != null)
				{
					\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[429](_probeWatcher, ProbeArrived);
					try
					{
						\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[197](_probeWatcher);
					}
					catch
					{
					}
					try
					{
						\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[198](_probeWatcher);
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
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[71](probeLock);
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
			ManagementScope val = new ManagementScope(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\ufffe\ufffdn\ufffb\u0018,+\u000f\ufff6\u0010%("));
			((ManagementOptions)val.Options).Timeout = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[430](3.0);
			val.Connect();
			ObjectQuery val2 = new ObjectQuery(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\f\ufff5\ufffd\ufff3\ufff8\u0002U^S\ufff0\u0003\ufff5\ufff2N￬\r\u001b\u0011\ufff5\u0003\u0018\u0012\ufffa\uffe7￮￦\u0006\u0006￼"));
			EnumerationOptions val3 = new EnumerationOptions
			{
				ReturnImmediately = true,
				Rewindable = false,
				Timeout = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[430](3.0),
				DirectRead = true
			};
			ManagementObjectSearcher val4 = new ManagementObjectSearcher(val, val2, val3);
			try
			{
				ManagementObjectCollection val5 = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[199](val4);
				try
				{
					((IEnumerable)val5).Cast<ManagementObject>().FirstOrDefault();
					return true;
				}
				finally
				{
					if (val5 != null)
					{
						\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[3](val5);
					}
				}
			}
			finally
			{
				if (val4 != null)
				{
					\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[3](val4);
				}
			}
		}
		catch (ManagementException ex)
		{
			ManagementException ex2 = ex;
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[84](\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("-\u0010\b\u000e\u000f\u0010\a\u000e\u0002\ufffb+￭\u0003\b\ufff2\ufff5\ufff9\ufffa\ufff8#<￢*￦826%ￜￚ#\u0017\ufff8ￜ"), \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[435](ex2), \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[435](ex2));
			LogCtrl.Write(\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[29](\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\t& \u0014\u0018T\ufffe\u0011\u001b\u000f\u0010\u0011\u0018\u000f\u0015\u001e￬ \u0004\t\u0015\u0018\n\v\vV?"), ((object)\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[435](ex2)/*cast due to constrained. prefix*/).ToString()));
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[435](ex2);
			_ = -2147217358;
			return false;
		}
		catch (Exception ex3)
		{
			\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[58](ex3);
			LogCtrl.Write(\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[29](\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\ufffb\u001c.*\"f \u0016\u0015'\u0013Z_"), \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[58](ex3)));
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
			ServiceController val = new ServiceController(\u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\u001e+%%*#\u001b"));
			try
			{
				return (int)\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[436](val) == 4;
			}
			finally
			{
				if (val != null)
				{
					\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[3](val);
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
		stateMachine.<>t__builder = \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[190]();
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
		return \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[54](\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[1](\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[53](\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[52](\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[51](\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[50](e)), \u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("\ufffe\u0006&&,")))));
	}
}

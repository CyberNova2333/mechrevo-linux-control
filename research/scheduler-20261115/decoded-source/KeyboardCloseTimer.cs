// Static analysis rendering; not original buildable source.
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using SharpDX.RawInput;

namespace MyControlCenter;

internal class KeyboardCloseTimer
{
	private Stopwatch sw = new Stopwatch();

	private System.Timers.Timer _TimersTimer = new System.Timers.Timer();

	private int _Seconds = 1;

	private int _AccumulationTime;

	private bool _StartMoniter = true;

	[CompilerGenerated]
	private EventHandler m_ControlBrigtnessEvent;

	private Dispatcher _dispatcher;

	private static readonly KeyboardCloseTimer keyboard = new KeyboardCloseTimer();

	private InputEvents _inputevent;

	private int _InvocationListCount;

	public static KeyboardCloseTimer CreateInstance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return keyboard;
		}
	}

	public event EventHandler ControlBrigtnessEvent
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		[CompilerGenerated]
		add
		{
			EventHandler eventHandler = this.m_ControlBrigtnessEvent;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)ResolvedDelegates.System_Delegate_Combine__130(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_ControlBrigtnessEvent, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		[CompilerGenerated]
		remove
		{
			EventHandler eventHandler = this.m_ControlBrigtnessEvent;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)ResolvedDelegates.System_Delegate_Remove__131(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_ControlBrigtnessEvent, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private KeyboardCloseTimer()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
		_TimersTimer = new System.Timers.Timer();
		ResolvedDelegates.System_Timers_Timer_set_Interval__441(_TimersTimer, double.MaxValue);
		ResolvedDelegates.System_Timers_Timer_set_AutoReset__464(_TimersTimer, true);
		ResolvedDelegates.System_Timers_Timer_add_Elapsed__442(_TimersTimer, _TimersTimer_Elapsed);
		this.m_ControlBrigtnessEvent = null;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetDispatcher(Dispatcher dispatcher)
	{
		_dispatcher = dispatcher;
		Dispatcher dispatcher2 = _dispatcher;
		if (dispatcher2 != null)
		{
			dispatcher2.Invoke((Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				_inputevent = new InputEvents();
			}), (DispatcherPriority)9);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void HookEvent()
	{
		Dispatcher dispatcher = _dispatcher;
		if (dispatcher != null)
		{
			dispatcher.Invoke((Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				_inputevent.setEnableDeviceRawInput(enable: true);
				_inputevent.KeyDown -= Input_KeyDown;
				_inputevent.MouseEvent -= Input_MouseEvent;
				_inputevent.KeyDown += Input_KeyDown;
				_inputevent.MouseEvent += Input_MouseEvent;
			}), (DispatcherPriority)9);
		}
	}

	private void Input_MouseEvent(object sender, MouseInputEventArgs e)
	{
		Keyboard_MouseEventTrigger((EventArgs)(object)e);
	}

	private void Input_KeyDown(object sender, KeyboardInputEventArgs e)
	{
		Keyboard_MouseEventTrigger((EventArgs)(object)e);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void UnHookEvent()
	{
		Dispatcher dispatcher = _dispatcher;
		if (dispatcher != null)
		{
			dispatcher.Invoke((Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				_inputevent.setEnableDeviceRawInput(enable: false);
				_inputevent.KeyDown -= Input_KeyDown;
				_inputevent.MouseEvent -= Input_MouseEvent;
			}), (DispatcherPriority)9);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Keyboard_MouseEventTrigger(EventArgs e)
	{
		EnableCloseTimer();
		if (this.m_ControlBrigtnessEvent != null && !_StartMoniter)
		{
			ResolvedDelegates.System_EventHandler_Invoke__49(this.m_ControlBrigtnessEvent, false, null);
			_StartMoniter = true;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool GetStartMoniter()
	{
		return _StartMoniter;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void _TimersTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		_AccumulationTime += ResolvedDelegates.System_Convert_ToInt32__494(ResolvedDelegates.System_Timers_Timer_get_Interval__590(_TimersTimer) / 1000.0);
		DisableCloseTimer();
		if (this.m_ControlBrigtnessEvent != null && _StartMoniter)
		{
			ResolvedDelegates.System_EventHandler_Invoke__49(this.m_ControlBrigtnessEvent, true, null);
			_StartMoniter = false;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetKeyboardCloseTime(int seconds)
	{
		int num = int.MaxValue;
		ResolvedDelegates.System_Int32_TryParse__132(ResolvedDelegates.System_Int32_ToString__55(ref seconds), ref num);
		if (num != int.MaxValue)
		{
			_Seconds = seconds;
		}
		if (seconds != 0)
		{
			EnableCloseTimer();
		}
		else
		{
			DisableCloseTimer();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void DisableCloseTimer()
	{
		_AccumulationTime = 0;
		ResolvedDelegates.System_Timers_Timer_Stop__444(_TimersTimer);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void EnableCloseTimer()
	{
		try
		{
			_AccumulationTime = 0;
			if (_Seconds != 0)
			{
				ResolvedDelegates.System_Timers_Timer_set_Interval__441(_TimersTimer, _Seconds * 1000);
			}
			else
			{
				_TimersTimer.Interval = 0.0;
			}
			_TimersTimer.Start();
		}
		catch (Exception)
		{
			_AccumulationTime = 0;
			ResolvedDelegates.System_Timers_Timer_set_Interval__441(_TimersTimer, double.MaxValue);
			ResolvedDelegates.System_Timers_Timer_Start__443(_TimersTimer);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RestartCloseTimer()
	{
		if (!_StartMoniter && _Seconds > 0)
		{
			_StartMoniter = true;
			EnableCloseTimer();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void Dispose()
	{
		_inputevent?.Dispose();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int ControlBrigtnessEventCheck()
	{
		if (this.m_ControlBrigtnessEvent != null)
		{
			return _InvocationListCount = ResolvedDelegates.System_Delegate_GetInvocationList__591(this.m_ControlBrigtnessEvent).Length;
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void ControlBrigtnessEventInit()
	{
		this.m_ControlBrigtnessEvent = null;
	}
}

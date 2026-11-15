// Reconstructed analysis view; strings decoded statically; unresolved delegates remain.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Define;
using DynamicDesk;
using GCUService.WindowsW;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using MyECIO;
using Utility;

namespace MyControlCenter;

public class MySystemManager
{
	[CompilerGenerated]
	private sealed class <>c__DisplayClass56_0
	{
		public MySystemManager <>4__this;

		public string backgroundPlayType;

		public <>c__DisplayClass56_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetBackgroundPlayType>b__0()
		{
			<>4__this.windowsw.SetBackgroundPlayType(backgroundPlayType);
		}
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass73_0
	{
		public MySystemManager <>4__this;

		public bool ONFF;

		public <>c__DisplayClass73_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetMonitorSwitch>b__0()
		{
			if (<>4__this.windowsw != null)
			{
				<>4__this.windowsw.ShowMonitor(ONFF);
			}
		}
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass74_0
	{
		public MySystemManager <>4__this;

		public bool ONFF;

		public <>c__DisplayClass74_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetBackground>b__0()
		{
			if (<>4__this.windowsw != null)
			{
				<>4__this.windowsw.ShowBackground(ONFF);
			}
		}
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass75_0
	{
		public MySystemManager <>4__this;

		public string backgroundFileName;

		public <>c__DisplayClass75_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetBackgroundFile>b__0()
		{
			if (<>4__this.windowsw != null)
			{
				<>4__this.windowsw.ChangeAndPlayVideo(backgroundFileName);
			}
		}
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass76_0
	{
		public MySystemManager <>4__this;

		public string location;

		public <>c__DisplayClass76_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetLocation>b__0()
		{
			if (<>4__this.windowsw != null)
			{
				<>4__this.windowsw.ShowMonitorLocation(location);
			}
		}
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass77_0
	{
		public MySystemManager <>4__this;

		public string monitorColor;

		public <>c__DisplayClass77_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetColor>b__0()
		{
			if (<>4__this.windowsw != null)
			{
				<>4__this.windowsw.SetColor(monitorColor, monitorColor);
			}
		}
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass78_0
	{
		public MySystemManager <>4__this;

		public bool ONOFF;

		public <>c__DisplayClass78_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetHotKey>b__0()
		{
			if (<>4__this.windowsw != null)
			{
				<>4__this.windowsw.SetHotkeyEnable(ONOFF);
			}
		}
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass79_0
	{
		public MySystemManager <>4__this;

		public string percent;

		public <>c__DisplayClass79_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetTransparent>b__0()
		{
			if (<>4__this.windowsw != null)
			{
				<>4__this.windowsw.SetTransparent(percent);
			}
		}
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass80_0
	{
		public MySystemManager <>4__this;

		public string percent;

		public <>c__DisplayClass80_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetSize>b__0()
		{
			if (<>4__this.windowsw != null)
			{
				<>4__this.windowsw.SetSize(percent);
			}
		}
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass81_0
	{
		public MySystemManager <>4__this;

		public List<string> SetVideoList;

		public <>c__DisplayClass81_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetVideoList>b__0()
		{
			if (<>4__this.windowsw != null)
			{
				<>4__this.windowsw.SetVideoList(SetVideoList);
			}
		}
	}

	[CompilerGenerated]
	private static class <>o__55
	{
		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__0;
	}

	[CompilerGenerated]
	private static class <>o__70
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string>> <>p__2;

		public static CallSite<Func<CallSite, object, string, object>> <>p__3;

		public static CallSite<Func<CallSite, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, string>> <>p__5;

		public static CallSite<Func<CallSite, object, string, object>> <>p__6;

		public static CallSite<Func<CallSite, object, object>> <>p__7;

		public static CallSite<Func<CallSite, object, string>> <>p__8;

		public static CallSite<Func<CallSite, object, string, object>> <>p__9;

		public static CallSite<Func<CallSite, object, object>> <>p__10;

		public static CallSite<Func<CallSite, object, string>> <>p__11;

		public static CallSite<Func<CallSite, object, string, object>> <>p__12;

		public static CallSite<Func<CallSite, object, object>> <>p__13;

		public static CallSite<Func<CallSite, object, string>> <>p__14;

		public static CallSite<Func<CallSite, object, string, object>> <>p__15;

		public static CallSite<Func<CallSite, object, object>> <>p__16;

		public static CallSite<Func<CallSite, object, string>> <>p__17;

		public static CallSite<Func<CallSite, object, string, object>> <>p__18;

		public static CallSite<Func<CallSite, object, object>> <>p__19;

		public static CallSite<Func<CallSite, object, string>> <>p__20;

		public static CallSite<Func<CallSite, object, string, object>> <>p__21;

		public static CallSite<Func<CallSite, object, object>> <>p__22;

		public static CallSite<Func<CallSite, object, string>> <>p__23;

		public static CallSite<Func<CallSite, object, string, object>> <>p__24;

		public static CallSite<Func<CallSite, object, object>> <>p__25;

		public static CallSite<Func<CallSite, object, string>> <>p__26;

		public static CallSite<Func<CallSite, object, string, object>> <>p__27;

		public static CallSite<Func<CallSite, object, object>> <>p__28;

		public static CallSite<Func<CallSite, object, string>> <>p__29;

		public static CallSite<Func<CallSite, object, string, object>> <>p__30;

		public static CallSite<Func<CallSite, object, object>> <>p__31;

		public static CallSite<Func<CallSite, object, string, object>> <>p__32;

		public static CallSite<Func<CallSite, object, object, object>> <>p__33;

		public static CallSite<Func<CallSite, object, bool>> <>p__34;

		public static CallSite<Func<CallSite, object, object, object>> <>p__35;

		public static CallSite<Func<CallSite, object, bool>> <>p__36;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__37;

		public static CallSite<Action<CallSite, List<string>, object>> <>p__38;

		public static CallSite<Func<CallSite, object, IEnumerable>> <>p__39;
	}

	[CompilerGenerated]
	private static class <>o__82
	{
		public static CallSite<Func<CallSite, object, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, uint>> <>p__2;

		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__3;

		public static CallSite<Func<CallSite, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, string>> <>p__5;
	}

	[CompilerGenerated]
	private static class <>o__83
	{
		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__0;
	}

	[CompilerGenerated]
	private static class <>o__85
	{
		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__0;
	}

	[CompilerGenerated]
	private static class <>o__86
	{
		public static CallSite<Func<CallSite, object, object, object>> <>p__0;

		public static CallSite<Func<CallSite, object, bool>> <>p__1;

		public static CallSite<Func<CallSite, object, object>> <>p__2;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__3;

		public static CallSite<Func<CallSite, object, uint>> <>p__4;

		public static CallSite<Func<CallSite, object, object>> <>p__5;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__6;

		public static CallSite<Func<CallSite, object, uint>> <>p__7;

		public static CallSite<Func<CallSite, object, object>> <>p__8;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__9;

		public static CallSite<Func<CallSite, object, int>> <>p__10;

		public static CallSite<Func<CallSite, object, object>> <>p__11;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__12;

		public static CallSite<Func<CallSite, object, int>> <>p__13;

		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__14;

		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__15;
	}

	[CompilerGenerated]
	private static class <>o__87
	{
		public static CallSite<Func<CallSite, object, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, uint>> <>p__2;

		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__3;
	}

	[CompilerGenerated]
	private static class <>o__89
	{
		public static CallSite<Func<CallSite, object, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, uint>> <>p__2;
	}

	[CompilerGenerated]
	private static class <>o__92
	{
		public static CallSite<Func<CallSite, object, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, uint>> <>p__2;

		public static CallSite<Func<CallSite, object, object>> <>p__3;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, uint>> <>p__5;

		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__6;

		public static CallSite<Func<CallSite, object, object>> <>p__7;

		public static CallSite<Func<CallSite, object, string>> <>p__8;

		public static CallSite<Func<CallSite, object, object>> <>p__9;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__10;

		public static CallSite<Func<CallSite, object, uint>> <>p__11;

		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__12;

		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__13;

		public static CallSite<Func<CallSite, object, object, object>> <>p__14;

		public static CallSite<Func<CallSite, object, bool>> <>p__15;

		public static CallSite<Func<CallSite, object, object>> <>p__16;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__17;

		public static CallSite<Func<CallSite, object, uint>> <>p__18;

		public static CallSite<Func<CallSite, object, object>> <>p__19;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__20;

		public static CallSite<Func<CallSite, object, uint>> <>p__21;

		public static CallSite<Func<CallSite, object, object>> <>p__22;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__23;

		public static CallSite<Func<CallSite, object, int>> <>p__24;

		public static CallSite<Func<CallSite, object, object>> <>p__25;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__26;

		public static CallSite<Func<CallSite, object, int>> <>p__27;

		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__28;
	}

	[CompilerGenerated]
	private static class <>o__93
	{
		public static CallSite<Func<CallSite, object, object, object>> <>p__0;

		public static CallSite<Func<CallSite, object, bool>> <>p__1;

		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__2;
	}

	[CompilerGenerated]
	private static class <>o__97
	{
		public static CallSite<Action<CallSite, MySystemView, string, object>> <>p__0;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <InitDiskBatteryInfo>d__87 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public MySystemManager <>4__this;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			MySystemManager mySystemManager = <>4__this;
			try
			{
				object obj = mySystemManager.InitBatteryInfo();
				WindowsW_Viewmodel ww_viewmodel = mySystemManager.ww_viewmodel;
				if (<>o__87.<>p__2 == null)
				{
					<>o__87.<>p__2 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, uint> target = <>o__87.<>p__2.Target;
				CallSite<Func<CallSite, object, uint>> arg = <>o__87.<>p__2;
				if (<>o__87.<>p__1 == null)
				{
					<>o__87.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target2 = <>o__87.<>p__1.Target;
				CallSite<Func<CallSite, Type, object, object>> arg2 = <>o__87.<>p__1;
				Type arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__87.<>p__0 == null)
				{
					<>o__87.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "BatteryLifePercent", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				ww_viewmodel.BatteryLifePercent = target(arg, target2(arg2, arg3, <>o__87.<>p__0.Target(<>o__87.<>p__0, obj)));
				if (<>o__87.<>p__3 == null)
				{
					<>o__87.<>p__3 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				<>o__87.<>p__3.Target(<>o__87.<>p__3, mySystemManager.m_View, "System/BatteryInfo", obj);
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
	private struct <Receive>d__70 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public byte[] message;

		public MySystemManager <>4__this;

		private TaskAwaiter<object> <>u__1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe void MoveNext()
		{
			int num = <>1__state;
			MySystemManager mySystemManager = <>4__this;
			try
			{
				TaskAwaiter<object> awaiter;
				if (num != 0)
				{
					awaiter = Json.ToObjectAsync<object>(ResolvedDelegates.System_Text_Encoding_GetString__351(ResolvedDelegates.System_Text_Encoding_get_UTF8__252(), message)).GetAwaiter();
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
				if (<>o__70.<>p__2 == null)
				{
					<>o__70.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, string> target = <>o__70.<>p__2.Target;
				CallSite<Func<CallSite, object, string>> arg = <>o__70.<>p__2;
				if (<>o__70.<>p__0 == null)
				{
					<>o__70.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				object obj = <>o__70.<>p__0.Target(<>o__70.<>p__0, result, "Action");
				object arg2;
				if (obj == null)
				{
					arg2 = null;
				}
				else
				{
					if (<>o__70.<>p__1 == null)
					{
						<>o__70.<>p__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg2 = <>o__70.<>p__1.Target(<>o__70.<>p__1, obj);
				}
				string text = target(arg, arg2);
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("[Recieve] action = ", text));
				if (<>o__70.<>p__5 == null)
				{
					<>o__70.<>p__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, string> target2 = <>o__70.<>p__5.Target;
				CallSite<Func<CallSite, object, string>> arg3 = <>o__70.<>p__5;
				if (<>o__70.<>p__3 == null)
				{
					<>o__70.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				obj = <>o__70.<>p__3.Target(<>o__70.<>p__3, result, "Location");
				object arg4;
				if (obj == null)
				{
					arg4 = null;
				}
				else
				{
					if (<>o__70.<>p__4 == null)
					{
						<>o__70.<>p__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg4 = <>o__70.<>p__4.Target(<>o__70.<>p__4, obj);
				}
				string text2 = target2(arg3, arg4);
				if (<>o__70.<>p__8 == null)
				{
					<>o__70.<>p__8 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, string> target3 = <>o__70.<>p__8.Target;
				CallSite<Func<CallSite, object, string>> arg5 = <>o__70.<>p__8;
				if (<>o__70.<>p__6 == null)
				{
					<>o__70.<>p__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				obj = <>o__70.<>p__6.Target(<>o__70.<>p__6, result, "BackgroundShow");
				object arg6;
				if (obj == null)
				{
					arg6 = null;
				}
				else
				{
					if (<>o__70.<>p__7 == null)
					{
						<>o__70.<>p__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg6 = <>o__70.<>p__7.Target(<>o__70.<>p__7, obj);
				}
				string text3 = target3(arg5, arg6);
				if (<>o__70.<>p__11 == null)
				{
					<>o__70.<>p__11 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, string> target4 = <>o__70.<>p__11.Target;
				CallSite<Func<CallSite, object, string>> arg7 = <>o__70.<>p__11;
				if (<>o__70.<>p__9 == null)
				{
					<>o__70.<>p__9 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				obj = <>o__70.<>p__9.Target(<>o__70.<>p__9, result, "BackgroundPlayType");
				object arg8;
				if (obj == null)
				{
					arg8 = null;
				}
				else
				{
					if (<>o__70.<>p__10 == null)
					{
						<>o__70.<>p__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg8 = <>o__70.<>p__10.Target(<>o__70.<>p__10, obj);
				}
				string text4 = target4(arg7, arg8);
				if (<>o__70.<>p__14 == null)
				{
					<>o__70.<>p__14 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, string> target5 = <>o__70.<>p__14.Target;
				CallSite<Func<CallSite, object, string>> arg9 = <>o__70.<>p__14;
				if (<>o__70.<>p__12 == null)
				{
					<>o__70.<>p__12 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				obj = <>o__70.<>p__12.Target(<>o__70.<>p__12, result, "BackgroundFileName");
				object arg10;
				if (obj == null)
				{
					arg10 = null;
				}
				else
				{
					if (<>o__70.<>p__13 == null)
					{
						<>o__70.<>p__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg10 = <>o__70.<>p__13.Target(<>o__70.<>p__13, obj);
				}
				string text5 = target5(arg9, arg10);
				if (<>o__70.<>p__17 == null)
				{
					<>o__70.<>p__17 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, string> target6 = <>o__70.<>p__17.Target;
				CallSite<Func<CallSite, object, string>> arg11 = <>o__70.<>p__17;
				if (<>o__70.<>p__15 == null)
				{
					<>o__70.<>p__15 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				obj = <>o__70.<>p__15.Target(<>o__70.<>p__15, result, "MonitorShow");
				object arg12;
				if (obj == null)
				{
					arg12 = null;
				}
				else
				{
					if (<>o__70.<>p__16 == null)
					{
						<>o__70.<>p__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg12 = <>o__70.<>p__16.Target(<>o__70.<>p__16, obj);
				}
				string text6 = target6(arg11, arg12);
				if (<>o__70.<>p__20 == null)
				{
					<>o__70.<>p__20 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, string> target7 = <>o__70.<>p__20.Target;
				CallSite<Func<CallSite, object, string>> arg13 = <>o__70.<>p__20;
				if (<>o__70.<>p__18 == null)
				{
					<>o__70.<>p__18 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				obj = <>o__70.<>p__18.Target(<>o__70.<>p__18, result, "MonitorHotkey");
				object arg14;
				if (obj == null)
				{
					arg14 = null;
				}
				else
				{
					if (<>o__70.<>p__19 == null)
					{
						<>o__70.<>p__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg14 = <>o__70.<>p__19.Target(<>o__70.<>p__19, obj);
				}
				string text7 = target7(arg13, arg14);
				if (<>o__70.<>p__23 == null)
				{
					<>o__70.<>p__23 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, string> target8 = <>o__70.<>p__23.Target;
				CallSite<Func<CallSite, object, string>> arg15 = <>o__70.<>p__23;
				if (<>o__70.<>p__21 == null)
				{
					<>o__70.<>p__21 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				obj = <>o__70.<>p__21.Target(<>o__70.<>p__21, result, "MonitorColor");
				object arg16;
				if (obj == null)
				{
					arg16 = null;
				}
				else
				{
					if (<>o__70.<>p__22 == null)
					{
						<>o__70.<>p__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg16 = <>o__70.<>p__22.Target(<>o__70.<>p__22, obj);
				}
				string text8 = target8(arg15, arg16);
				if (<>o__70.<>p__26 == null)
				{
					<>o__70.<>p__26 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, string> target9 = <>o__70.<>p__26.Target;
				CallSite<Func<CallSite, object, string>> arg17 = <>o__70.<>p__26;
				if (<>o__70.<>p__24 == null)
				{
					<>o__70.<>p__24 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				obj = <>o__70.<>p__24.Target(<>o__70.<>p__24, result, "MonitorTransparent");
				object arg18;
				if (obj == null)
				{
					arg18 = null;
				}
				else
				{
					if (<>o__70.<>p__25 == null)
					{
						<>o__70.<>p__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg18 = <>o__70.<>p__25.Target(<>o__70.<>p__25, obj);
				}
				string text9 = target9(arg17, arg18);
				if (<>o__70.<>p__29 == null)
				{
					<>o__70.<>p__29 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, string> target10 = <>o__70.<>p__29.Target;
				CallSite<Func<CallSite, object, string>> arg19 = <>o__70.<>p__29;
				if (<>o__70.<>p__27 == null)
				{
					<>o__70.<>p__27 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				obj = <>o__70.<>p__27.Target(<>o__70.<>p__27, result, "MonitorSize");
				object arg20;
				if (obj == null)
				{
					arg20 = null;
				}
				else
				{
					if (<>o__70.<>p__28 == null)
					{
						<>o__70.<>p__28 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg20 = <>o__70.<>p__28.Target(<>o__70.<>p__28, obj);
				}
				string text10 = target10(arg19, arg20);
				if (<>o__70.<>p__30 == null)
				{
					<>o__70.<>p__30 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				obj = <>o__70.<>p__30.Target(<>o__70.<>p__30, result, "VideoList");
				object obj2;
				if (obj == null)
				{
					obj2 = null;
				}
				else
				{
					if (<>o__70.<>p__31 == null)
					{
						<>o__70.<>p__31 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					obj2 = <>o__70.<>p__31.Target(<>o__70.<>p__31, obj);
				}
				object arg21 = obj2;
				if (<>o__70.<>p__34 == null)
				{
					<>o__70.<>p__34 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				Func<CallSite, object, bool> target11 = <>o__70.<>p__34.Target;
				CallSite<Func<CallSite, object, bool>> arg22 = <>o__70.<>p__34;
				if (<>o__70.<>p__33 == null)
				{
					<>o__70.<>p__33 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				Func<CallSite, object, object, object> target12 = <>o__70.<>p__33.Target;
				CallSite<Func<CallSite, object, object, object>> arg23 = <>o__70.<>p__33;
				if (<>o__70.<>p__32 == null)
				{
					<>o__70.<>p__32 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				if (target11(arg22, target12(arg23, <>o__70.<>p__32.Target(<>o__70.<>p__32, result, "Action"), null)))
				{
					text = ResolvedDelegates.System_String_ToLowerInvariant__452(ref *(string*)text);
					if (ResolvedDelegates.System_String_Contains__108(ref *(string*)text, "getgraphicinfo"))
					{
						mySystemManager.GetGraphicInfo();
					}
					else if (text.Contains("gethardwareinfo"))
					{
						mySystemManager.InitSystemInfo();
					}
					else if (text.Contains("system_on"))
					{
						mySystemManager.Enable();
					}
					else if (text.Contains("system_off"))
					{
						mySystemManager.Disable();
					}
					if (text.Contains("getstatus"))
					{
						try
						{
							mySystemManager.SetStatusToClient();
						}
						catch (Exception)
						{
						}
					}
				}
				if (text2 != null)
				{
					mySystemManager.SetLocation(text2);
				}
				if (text3 != null)
				{
					bool background = ResolvedDelegates.System_String_Contains__108(ref *(string*)ResolvedDelegates.System_String_ToLowerInvariant__452(ref *(string*)text3), "on");
					mySystemManager.SetBackground(background);
				}
				if (text4 != null)
				{
					mySystemManager.SetBackgroundPlayType(text4);
				}
				if (text6 != null)
				{
					bool monitorSwitch = ResolvedDelegates.System_String_Contains__108(ref *(string*)ResolvedDelegates.System_String_ToLowerInvariant__452(ref *(string*)text6), "on");
					mySystemManager.SetMonitorSwitch(monitorSwitch);
				}
				if (text7 != null)
				{
					bool hotKey = ResolvedDelegates.System_String_Contains__108(ref *(string*)ResolvedDelegates.System_String_ToLowerInvariant__452(ref *(string*)text7), "on");
					mySystemManager.SetHotKey(hotKey);
				}
				if (text5 != null)
				{
					mySystemManager.SetBackgroundFile(text5);
				}
				if (text8 != null)
				{
					mySystemManager.SetColor(text8);
				}
				if (text9 != null)
				{
					mySystemManager.SetTransparent(text9);
				}
				if (text10 != null)
				{
					mySystemManager.SetSize(text10);
				}
				if (<>o__70.<>p__36 == null)
				{
					<>o__70.<>p__36 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				Func<CallSite, object, bool> target13 = <>o__70.<>p__36.Target;
				CallSite<Func<CallSite, object, bool>> arg24 = <>o__70.<>p__36;
				if (<>o__70.<>p__35 == null)
				{
					<>o__70.<>p__35 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				if (target13(arg24, <>o__70.<>p__35.Target(<>o__70.<>p__35, arg21, null)))
				{
					List<string> list = new List<string>();
					if (<>o__70.<>p__39 == null)
					{
						<>o__70.<>p__39 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(IEnumerable).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
					}
					IEnumerator enumerator = <>o__70.<>p__39.Target(<>o__70.<>p__39, arg21).GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object current = enumerator.Current;
							if (<>o__70.<>p__38 == null)
							{
								<>o__70.<>p__38 = CallSite<Action<CallSite, List<string>, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Action<CallSite, List<string>, object> target14 = <>o__70.<>p__38.Target;
							CallSite<Action<CallSite, List<string>, object>> arg25 = <>o__70.<>p__38;
							if (<>o__70.<>p__37 == null)
							{
								<>o__70.<>p__37 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							target14(arg25, list, <>o__70.<>p__37.Target(<>o__70.<>p__37, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle), current));
						}
					}
					finally
					{
						if (num < 0 && enumerator is IDisposable disposable)
						{
							ResolvedDelegates.System_IDisposable_Dispose__3(disposable);
						}
					}
					mySystemManager.SetVideoList(list);
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

	private MySystemView m_View = new MySystemView();

	private string beforeCpuValue = "0";

	private string beforeMemoryValue = "0";

	private string beforeGpuValue = "0";

	private string beforeBatteryValue = "0";

	private bool UPnPSwtich;

	private bool SqliteDbSwitch;

	private System.Timers.Timer cpugpuCombineRunTimer;

	private System.Timers.Timer cpuinfoTimer;

	private System.Timers.Timer diskinfoTimer;

	private System.Timers.Timer CpuTempDbTimer;

	private System.Timers.Timer BatteryTimer;

	private System.Timers.Timer FanInfoTimer;

	private CPUInfo cCpuInfo = new CPUInfo();

	private GPUInfo cGpuInfo = new GPUInfo();

	private MemoryInfo cMemoryInfo = new MemoryInfo();

	private NetworkInfo cNetworkInfo = new NetworkInfo();

	private BatteryInfo cBatteryInfo = new BatteryInfo();

	private DiskInfo cDiskInfo = new DiskInfo();

	private FanFuctionErrorInfo _FanFunctionErrorInfo = new FanFuctionErrorInfo();

	private FanInfo _FanInfo = new FanInfo();

	private HwFuelGaugeInfo _HwFuelGaugeInfo = new HwFuelGaugeInfo();

	private bool EnsureStart;

	private WindowW windowsw;

	private WindowsW_Viewmodel ww_viewmodel = WindowsW_Viewmodel.Instance;

	private object gpuInfoLock = new object();

	private int gpuCommandCount;

	private int TempGpuPState;

	private int TempTempature;

	private object diskInfoLock = new object();

	private int disErrorCount;

	private int diskCommandCount;

	private bool IsNvGpu;

	private bool CPUGPUConbineONOFFTimers;

	private static int CPUGPUConbineONOFFTimersCount = 0;

	private byte CPUGPUConbineONOFFReturnValue;

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	private BIOS_PROJECT_ID BiosProjectID;

	private byte GPUConbineONOFFReturnValue;

	private bool m_isGPUConbineSupport;

	public bool m_CPUGPUCombine_ON;

	private bool m_CPUGPUCombineGame_ON;

	private int m_CPUGPUCombineGame_LanuchCondition = -1;

	private bool m_isWMICPUGPUCombineRunSupport;

	private bool m_isWMICPUGPUCombineRunSupportGame;

	private bool m_isFurMarkFan1;

	private bool m_FurMarkFan1_ON;

	private int CPUOverLoadCount;

	private int GPUOverLoadCount;

	private int GPUOverLoadCount_cpugpu;

	private int GPUOverLoadCount_game;

	private static readonly MySystemManager system = new MySystemManager();

	private Stopwatch sw = new Stopwatch();

	private object windowswLock = new object();

	private static string _GraphicInfo = new HardwareInfoCollect().getGraphicInfo();

	public static MySystemManager CreateInstance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return system;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public MySystemManager()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void HotkeyEventHandler(object sender, EventArgs e)
	{
		object arg = new { };
		if (<>o__55.<>p__0 == null)
		{
			<>o__55.<>p__0 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		<>o__55.<>p__0.Target(<>o__55.<>p__0, m_View, "Monitor/Status", arg);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetBackgroundPlayType(string backgroundPlayType)
	{
		<>c__DisplayClass56_0 CS$<>8__locals4 = new <>c__DisplayClass56_0();
		CS$<>8__locals4.<>4__this = this;
		CS$<>8__locals4.backgroundPlayType = backgroundPlayType;
		Dispatcher dispatcher = ((DispatcherObject)windowsw).Dispatcher;
		if (dispatcher != null)
		{
			dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				CS$<>8__locals4.<>4__this.windowsw.SetBackgroundPlayType(CS$<>8__locals4.backgroundPlayType);
			}), (DispatcherPriority)4, Array.Empty<object>());
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetInitVideoList(string monitorVideoList)
	{
		if (monitorVideoList != null && ResolvedDelegates.System_String_op_Inequality__40(monitorVideoList, ""))
		{
			List<string> videoList = Json.ToObject<List<string>>(monitorVideoList);
			SetVideoList(videoList);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Standby_ModernStandbyChanged(object sender, EventArgs e)
	{
		ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_Object_ToString__1(sender), "On");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void InitTimers()
	{
		IsNvGpu = UtilityExtensions.IsNvGpu();
		BiosProjectID = EcCtrl.GetBiosProjctID();
		cpuinfoTimer = new System.Timers.Timer();
		ResolvedDelegates.System_Timers_Timer_set_Interval__441(cpuinfoTimer, 2000.0);
		ResolvedDelegates.System_Timers_Timer_add_Elapsed__442(cpuinfoTimer, CpuinfoTimer_Elapsed);
		diskinfoTimer = new System.Timers.Timer();
		ResolvedDelegates.System_Timers_Timer_set_Interval__441(diskinfoTimer, 10000.0);
		ResolvedDelegates.System_Timers_Timer_add_Elapsed__442(diskinfoTimer, DiskinfoTimer_Elapsed);
		CpuTempDbTimer = new System.Timers.Timer();
		ResolvedDelegates.System_Timers_Timer_set_Interval__441(CpuTempDbTimer, 60000.0);
		ResolvedDelegates.System_Timers_Timer_add_Elapsed__442(CpuTempDbTimer, CpuTempDbTimer_Elapsed);
		BatteryTimer = new System.Timers.Timer();
		ResolvedDelegates.System_Timers_Timer_set_Interval__441(BatteryTimer, 60000.0);
		ResolvedDelegates.System_Timers_Timer_add_Elapsed__442(BatteryTimer, BatteryTimer_Elapsed);
		FanInfoTimer = new System.Timers.Timer();
		ResolvedDelegates.System_Timers_Timer_set_Interval__441(FanInfoTimer, 2000.0);
		ResolvedDelegates.System_Timers_Timer_add_Elapsed__442(FanInfoTimer, FanInfoTimer_Elapsed);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StartTimers()
	{
		ResolvedDelegates.System_Timers_Timer_Start__443(cpuinfoTimer);
		diskinfoTimer?.Start();
		CpuTempDbTimer.Start();
		ResolvedDelegates.System_Timers_Timer_Start__443(FanInfoTimer);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void StopTimers()
	{
		try
		{
			ResolvedDelegates.System_Timers_Timer_Stop__444(cpuinfoTimer);
			diskinfoTimer?.Stop();
			CpuTempDbTimer.Stop();
			ResolvedDelegates.System_Timers_Timer_Stop__444(FanInfoTimer);
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("System monitor error ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void InitCPUGPUConbineONOFFTimers(bool IsWMICPUGPUCombineRunSupport, bool IsWMICPUGPUCombineRunSupportGame, bool isGPUConbineSupport)
	{
		m_isWMICPUGPUCombineRunSupport = IsWMICPUGPUCombineRunSupport;
		m_isWMICPUGPUCombineRunSupportGame = IsWMICPUGPUCombineRunSupportGame;
		m_isGPUConbineSupport = isGPUConbineSupport;
		cpugpuCombineRunTimer = new System.Timers.Timer();
		ResolvedDelegates.System_Timers_Timer_set_Interval__441(cpugpuCombineRunTimer, 1500.0);
		ResolvedDelegates.System_Timers_Timer_add_Elapsed__442(cpugpuCombineRunTimer, cpugpuDTRunTimer_Elapsed);
		ResolvedDelegates.System_Timers_Timer_Start__443(cpugpuCombineRunTimer);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void StartCPUGPUConbineONOFFTimers()
	{
		try
		{
			cpugpuCombineRunTimer?.Start();
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("System monitor error ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void StopCPUGPUConbineONOFFTimers()
	{
		try
		{
			cpugpuCombineRunTimer?.Stop();
		}
		catch (Exception ex)
		{
			ResolvedDelegates.System_Console_WriteLine__8(ResolvedDelegates.System_String_Concat__29("System monitor error ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Enable()
	{
		InitFanFunctionErrorInfo();
		ResolvedDelegates.System_Threading_Tasks_Task_Run__10([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			InitDiskBatteryInfo();
		});
		InitSystemInfo();
		StartTimers();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void Disable()
	{
		if (!EnsureStart)
		{
			StopTimers();
		}
	}

	public virtual void Dispose()
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<Receive>d__70))]
	public virtual void Receive(byte[] message)
	{
		<Receive>d__70 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine.message = message;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	private void SetStatusToClient()
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool GetMonitorSupport()
	{
		return ResolvedDelegates.System_Convert_ToBoolean__42(RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\", "MonitorSupport", 0, RegistryValueKind.DWord));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetMonitorSwitch(bool ONFF)
	{
		<>c__DisplayClass73_0 CS$<>8__locals6 = new <>c__DisplayClass73_0();
		CS$<>8__locals6.<>4__this = this;
		CS$<>8__locals6.ONFF = ONFF;
		Dispatcher dispatcher = ((DispatcherObject)windowsw).Dispatcher;
		if (dispatcher != null)
		{
			dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				if (CS$<>8__locals6.<>4__this.windowsw != null)
				{
					CS$<>8__locals6.<>4__this.windowsw.ShowMonitor(CS$<>8__locals6.ONFF);
				}
			}), (DispatcherPriority)4, Array.Empty<object>());
		}
		if (CS$<>8__locals6.ONFF)
		{
			EnsureStart = true;
			Enable();
		}
		else
		{
			EnsureStart = false;
			Disable();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetBackground(bool ONFF)
	{
		<>c__DisplayClass74_0 CS$<>8__locals5 = new <>c__DisplayClass74_0();
		CS$<>8__locals5.<>4__this = this;
		CS$<>8__locals5.ONFF = ONFF;
		Dispatcher dispatcher = ((DispatcherObject)windowsw).Dispatcher;
		if (dispatcher == null)
		{
			return;
		}
		dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			if (CS$<>8__locals5.<>4__this.windowsw != null)
			{
				CS$<>8__locals5.<>4__this.windowsw.ShowBackground(CS$<>8__locals5.ONFF);
			}
		}), (DispatcherPriority)4, Array.Empty<object>());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetBackgroundFile(string backgroundFileName)
	{
		<>c__DisplayClass75_0 CS$<>8__locals5 = new <>c__DisplayClass75_0();
		CS$<>8__locals5.<>4__this = this;
		CS$<>8__locals5.backgroundFileName = backgroundFileName;
		Dispatcher dispatcher = ((DispatcherObject)windowsw).Dispatcher;
		if (dispatcher == null)
		{
			return;
		}
		dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			if (CS$<>8__locals5.<>4__this.windowsw != null)
			{
				CS$<>8__locals5.<>4__this.windowsw.ChangeAndPlayVideo(CS$<>8__locals5.backgroundFileName);
			}
		}), (DispatcherPriority)4, Array.Empty<object>());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetLocation(string location)
	{
		<>c__DisplayClass76_0 CS$<>8__locals5 = new <>c__DisplayClass76_0();
		CS$<>8__locals5.<>4__this = this;
		CS$<>8__locals5.location = location;
		Dispatcher dispatcher = ((DispatcherObject)windowsw).Dispatcher;
		if (dispatcher == null)
		{
			return;
		}
		dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			if (CS$<>8__locals5.<>4__this.windowsw != null)
			{
				CS$<>8__locals5.<>4__this.windowsw.ShowMonitorLocation(CS$<>8__locals5.location);
			}
		}), (DispatcherPriority)4, Array.Empty<object>());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetColor(string monitorColor)
	{
		<>c__DisplayClass77_0 CS$<>8__locals6 = new <>c__DisplayClass77_0();
		CS$<>8__locals6.<>4__this = this;
		CS$<>8__locals6.monitorColor = monitorColor;
		Dispatcher dispatcher = ((DispatcherObject)windowsw).Dispatcher;
		if (dispatcher == null)
		{
			return;
		}
		dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			if (CS$<>8__locals6.<>4__this.windowsw != null)
			{
				CS$<>8__locals6.<>4__this.windowsw.SetColor(CS$<>8__locals6.monitorColor, CS$<>8__locals6.monitorColor);
			}
		}), (DispatcherPriority)4, Array.Empty<object>());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetHotKey(bool ONOFF)
	{
		<>c__DisplayClass78_0 CS$<>8__locals5 = new <>c__DisplayClass78_0();
		CS$<>8__locals5.<>4__this = this;
		CS$<>8__locals5.ONOFF = ONOFF;
		Dispatcher dispatcher = ((DispatcherObject)windowsw).Dispatcher;
		if (dispatcher == null)
		{
			return;
		}
		dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			if (CS$<>8__locals5.<>4__this.windowsw != null)
			{
				CS$<>8__locals5.<>4__this.windowsw.SetHotkeyEnable(CS$<>8__locals5.ONOFF);
			}
		}), (DispatcherPriority)4, Array.Empty<object>());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetTransparent(string percent)
	{
		<>c__DisplayClass79_0 CS$<>8__locals5 = new <>c__DisplayClass79_0();
		CS$<>8__locals5.<>4__this = this;
		CS$<>8__locals5.percent = percent;
		Dispatcher dispatcher = ((DispatcherObject)windowsw).Dispatcher;
		if (dispatcher == null)
		{
			return;
		}
		dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			if (CS$<>8__locals5.<>4__this.windowsw != null)
			{
				CS$<>8__locals5.<>4__this.windowsw.SetTransparent(CS$<>8__locals5.percent);
			}
		}), (DispatcherPriority)4, Array.Empty<object>());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetSize(string percent)
	{
		<>c__DisplayClass80_0 CS$<>8__locals5 = new <>c__DisplayClass80_0();
		CS$<>8__locals5.<>4__this = this;
		CS$<>8__locals5.percent = percent;
		Dispatcher dispatcher = ((DispatcherObject)windowsw).Dispatcher;
		if (dispatcher == null)
		{
			return;
		}
		dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			if (CS$<>8__locals5.<>4__this.windowsw != null)
			{
				CS$<>8__locals5.<>4__this.windowsw.SetSize(CS$<>8__locals5.percent);
			}
		}), (DispatcherPriority)4, Array.Empty<object>());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetVideoList(List<string> SetVideoList)
	{
		<>c__DisplayClass81_0 CS$<>8__locals5 = new <>c__DisplayClass81_0();
		CS$<>8__locals5.<>4__this = this;
		CS$<>8__locals5.SetVideoList = SetVideoList;
		Dispatcher dispatcher = ((DispatcherObject)windowsw).Dispatcher;
		if (dispatcher == null)
		{
			return;
		}
		dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			if (CS$<>8__locals5.<>4__this.windowsw != null)
			{
				CS$<>8__locals5.<>4__this.windowsw.SetVideoList(CS$<>8__locals5.SetVideoList);
			}
		}), (DispatcherPriority)4, Array.Empty<object>());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void BatteryTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		object batteryInfo = GetBatteryInfo();
		WindowsW_Viewmodel windowsW_Viewmodel = ww_viewmodel;
		if (<>o__82.<>p__2 == null)
		{
			<>o__82.<>p__2 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, uint> target = <>o__82.<>p__2.Target;
		CallSite<Func<CallSite, object, uint>> arg = <>o__82.<>p__2;
		if (<>o__82.<>p__1 == null)
		{
			<>o__82.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target2 = <>o__82.<>p__1.Target;
		CallSite<Func<CallSite, Type, object, object>> arg2 = <>o__82.<>p__1;
		Type arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__82.<>p__0 == null)
		{
			<>o__82.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "BatteryLifePercent", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		windowsW_Viewmodel.BatteryLifePercent = target(arg, target2(arg2, arg3, <>o__82.<>p__0.Target(<>o__82.<>p__0, batteryInfo)));
		if (<>o__82.<>p__3 == null)
		{
			<>o__82.<>p__3 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		<>o__82.<>p__3.Target(<>o__82.<>p__3, m_View, "System/BatteryInfo", batteryInfo);
		if (<>o__82.<>p__5 == null)
		{
			<>o__82.<>p__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, string> target3 = <>o__82.<>p__5.Target;
		CallSite<Func<CallSite, object, string>> arg4 = <>o__82.<>p__5;
		if (<>o__82.<>p__4 == null)
		{
			<>o__82.<>p__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "BatteryLifePercent", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		beforeBatteryValue = target3(arg4, <>o__82.<>p__4.Target(<>o__82.<>p__4, batteryInfo));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void FanInfoTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		object funInfo = GetFunInfo();
		if (<>o__83.<>p__0 == null)
		{
			<>o__83.<>p__0 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		<>o__83.<>p__0.Target(<>o__83.<>p__0, m_View, "System/FanInfo", funInfo);
	}

	private void HwFuelGaugeTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void InitFanFunctionErrorInfo()
	{
		object funError = GetFunError();
		if (<>o__85.<>p__0 == null)
		{
			<>o__85.<>p__0 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		<>o__85.<>p__0.Target(<>o__85.<>p__0, m_View, "System/FanErrorInfo", funError);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void InitSystemInfo()
	{
		try
		{
			HardwareInfoCollect hardwareInfoCollect = new HardwareInfoCollect();
			string processorInfo = hardwareInfoCollect.getProcessorInfo();
			string graphicInfo = hardwareInfoCollect.getGraphicInfo();
			string memoryUsableInfo = ResolvedDelegates.System_String_Concat__120(hardwareInfoCollect.getMemoryInfo(), " GB(", hardwareInfoCollect.getMemoryUsableInfo(), " GB usable)");
			string bIOSInfo = hardwareInfoCollect.getBIOSInfo();
			string eCInfo = hardwareInfoCollect.getECInfo();
			string systemProductInfo = hardwareInfoCollect.getSystemProductInfo();
			object arg = new
			{
				ProcessorInfo = processorInfo,
				GraphicInfo = graphicInfo,
				MemoryUsableInfo = memoryUsableInfo,
				BIOSInfo = bIOSInfo,
				ECInfo = eCInfo,
				SystemProductInfo = systemProductInfo
			};
			object gpuInfo = GetGpuInfo();
			ww_viewmodel.GpuUsage = 0u;
			ww_viewmodel.GpuTemperature = 0u;
			if (<>o__86.<>p__1 == null)
			{
				<>o__86.<>p__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target = <>o__86.<>p__1.Target;
			CallSite<Func<CallSite, object, bool>> arg2 = <>o__86.<>p__1;
			if (<>o__86.<>p__0 == null)
			{
				<>o__86.<>p__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (target(arg2, <>o__86.<>p__0.Target(<>o__86.<>p__0, gpuInfo, null)))
			{
				WindowsW_Viewmodel windowsW_Viewmodel = ww_viewmodel;
				if (<>o__86.<>p__4 == null)
				{
					<>o__86.<>p__4 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, uint> target2 = <>o__86.<>p__4.Target;
				CallSite<Func<CallSite, object, uint>> arg3 = <>o__86.<>p__4;
				if (<>o__86.<>p__3 == null)
				{
					<>o__86.<>p__3 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target3 = <>o__86.<>p__3.Target;
				CallSite<Func<CallSite, Type, object, object>> arg4 = <>o__86.<>p__3;
				Type arg5 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				object obj = gpuInfo;
				object arg6;
				if (obj == null)
				{
					arg6 = null;
				}
				else
				{
					if (<>o__86.<>p__2 == null)
					{
						<>o__86.<>p__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "GpuUsage", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					arg6 = <>o__86.<>p__2.Target(<>o__86.<>p__2, obj);
				}
				windowsW_Viewmodel.GpuUsage = target2(arg3, target3(arg4, arg5, arg6));
				WindowsW_Viewmodel windowsW_Viewmodel2 = ww_viewmodel;
				if (<>o__86.<>p__7 == null)
				{
					<>o__86.<>p__7 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, uint> target4 = <>o__86.<>p__7.Target;
				CallSite<Func<CallSite, object, uint>> arg7 = <>o__86.<>p__7;
				if (<>o__86.<>p__6 == null)
				{
					<>o__86.<>p__6 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target5 = <>o__86.<>p__6.Target;
				CallSite<Func<CallSite, Type, object, object>> arg8 = <>o__86.<>p__6;
				Type arg9 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__86.<>p__5 == null)
				{
					<>o__86.<>p__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "GpuTemperature", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				windowsW_Viewmodel2.GpuTemperature = target4(arg7, target5(arg8, arg9, <>o__86.<>p__5.Target(<>o__86.<>p__5, gpuInfo)));
				if (<>o__86.<>p__10 == null)
				{
					<>o__86.<>p__10 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, int> target6 = <>o__86.<>p__10.Target;
				CallSite<Func<CallSite, object, int>> arg10 = <>o__86.<>p__10;
				if (<>o__86.<>p__9 == null)
				{
					<>o__86.<>p__9 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target7 = <>o__86.<>p__9.Target;
				CallSite<Func<CallSite, Type, object, object>> arg11 = <>o__86.<>p__9;
				Type arg12 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__86.<>p__8 == null)
				{
					<>o__86.<>p__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "GpuTemperature", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				target6(arg10, target7(arg11, arg12, <>o__86.<>p__8.Target(<>o__86.<>p__8, gpuInfo)));
				if (<>o__86.<>p__13 == null)
				{
					<>o__86.<>p__13 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
				}
				Func<CallSite, object, int> target8 = <>o__86.<>p__13.Target;
				CallSite<Func<CallSite, object, int>> arg13 = <>o__86.<>p__13;
				if (<>o__86.<>p__12 == null)
				{
					<>o__86.<>p__12 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target9 = <>o__86.<>p__12.Target;
				CallSite<Func<CallSite, Type, object, object>> arg14 = <>o__86.<>p__12;
				Type arg15 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__86.<>p__11 == null)
				{
					<>o__86.<>p__11 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "GpuPState", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				target8(arg13, target9(arg14, arg15, <>o__86.<>p__11.Target(<>o__86.<>p__11, gpuInfo)));
				if (<>o__86.<>p__14 == null)
				{
					<>o__86.<>p__14 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				<>o__86.<>p__14.Target(<>o__86.<>p__14, m_View, "System/GpuInfo", gpuInfo);
			}
			if (<>o__86.<>p__15 == null)
			{
				<>o__86.<>p__15 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			<>o__86.<>p__15.Target(<>o__86.<>p__15, m_View, "System/HardwareInfo", arg);
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Hardware info ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<InitDiskBatteryInfo>d__87))]
	private void InitDiskBatteryInfo()
	{
		<InitDiskBatteryInfo>d__87 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void log(string strLog)
	{
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("cpugpucombine* ", strLog));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void cpugpuDTRunTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		if (EcCtrl.GetRoundSocketStatusFromEC() == 0 || ResolvedDelegates.System_Threading_Interlocked_Exchange__651(ref CPUGPUConbineONOFFTimersCount, 1) != 0)
		{
			return;
		}
		object cpuInfoforCPUGPUCombine = GetCpuInfoforCPUGPUCombine();
		if (<>o__89.<>p__2 == null)
		{
			<>o__89.<>p__2 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, uint> target = <>o__89.<>p__2.Target;
		CallSite<Func<CallSite, object, uint>> arg = <>o__89.<>p__2;
		if (<>o__89.<>p__1 == null)
		{
			<>o__89.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target2 = <>o__89.<>p__1.Target;
		CallSite<Func<CallSite, Type, object, object>> arg2 = <>o__89.<>p__1;
		Type arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__89.<>p__0 == null)
		{
			<>o__89.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "CpuUsage", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		uint num = target(arg, target2(arg2, arg3, <>o__89.<>p__0.Target(<>o__89.<>p__0, cpuInfoforCPUGPUCombine)));
		if (num > 95 && CPUOverLoadCount < 3)
		{
			CPUOverLoadCount++;
		}
		else if (num < 10 && CPUOverLoadCount > -1)
		{
			CPUOverLoadCount--;
		}
		if (num <= 95 && num >= 10)
		{
			CPUOverLoadCount = 0;
		}
		uint num2 = 0u;
		num2 = GetGpuCombineUsageOnly();
		if (num2 > 90 && GPUOverLoadCount_cpugpu < 3)
		{
			GPUOverLoadCount_cpugpu++;
		}
		else if (num2 <= 90 && GPUOverLoadCount_cpugpu > -1)
		{
			GPUOverLoadCount_cpugpu--;
		}
		if (m_isWMICPUGPUCombineRunSupportGame)
		{
			if (num2 > 50 && GPUOverLoadCount_game < 3)
			{
				GPUOverLoadCount_game++;
			}
			else if (num2 <= 50 && GPUOverLoadCount_game > -1)
			{
				GPUOverLoadCount_game--;
			}
		}
		m_CPUGPUCombine_ON = false;
		if (CPUOverLoadCount >= 3 && GPUOverLoadCount_cpugpu >= 3)
		{
			m_CPUGPUCombine_ON = true;
			if (m_CPUGPUCombineGame_ON)
			{
				log("@@@SHUANGKAO entry on, check CPUGPUCombineGame is ON, trun off first!");
				LogCtrl.TraceMessageProgramData("@@@SHUANGKAO entry on, check CPUGPUCombineGame is ON, trun off first!", "cpugpuDTRunTimer_Elapsed", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\MySystemManager.cs", 827);
				WMIEC.Instance.WMICPUGPUCombineRunOFFGame();
				m_CPUGPUCombineGame_ON = false;
			}
			if (CPUGPUConbineONOFFReturnValue == 187)
			{
				LogCtrl.TraceMessageProgramData("cpugpuDTRunTimer_Elapsed 0xBB", "cpugpuDTRunTimer_Elapsed", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\MySystemManager.cs", 834);
			}
			else if (GPUConbineONOFFReturnValue != 187)
			{
				CPUGPUConbineONOFFReturnValue = WMIEC.Instance.WMICPUGPUCombineRunON();
				log("@@@WMICPUGPUCombineRunON");
				LogCtrl.TraceMessageProgramData("cpugpuDTRunTimer_Elapsed ON", "cpugpuDTRunTimer_Elapsed", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\MySystemManager.cs", 849);
			}
			else
			{
				LogCtrl.TraceMessageProgramData("cpugpuDTRunTimer_Elapsed Can not switch", "cpugpuDTRunTimer_Elapsed", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\MySystemManager.cs", 853);
				log("@@@DAN KAO ... Can not switch");
			}
		}
		else if ((CPUOverLoadCount <= -1 || GPUOverLoadCount_cpugpu <= -1) && CPUGPUConbineONOFFReturnValue != 170)
		{
			CPUGPUConbineONOFFReturnValue = WMIEC.Instance.WMICPUGPUCombineRunOFF();
			log(ResolvedDelegates.System_String_Concat__29("@@@WMICPUGPUCombineRunOFF: ", ResolvedDelegates.System_Byte_ToString__119(ref CPUGPUConbineONOFFReturnValue)));
			LogCtrl.TraceMessageProgramData("cpugpuDTRunTimer_Elapsed OFF", "cpugpuDTRunTimer_Elapsed", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\MySystemManager.cs", 874);
		}
		if (m_isWMICPUGPUCombineRunSupportGame)
		{
			if (GPUOverLoadCount_game >= 3)
			{
				LogCtrl.TraceMessageProgramData("@@@CPUGPUCombineGame fit condition", "cpugpuDTRunTimer_Elapsed", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\MySystemManager.cs", 883);
				log("@@@CPUGPUCombineGame, fit condition");
				if (CPUGPUConbineONOFFReturnValue != 187)
				{
					LogCtrl.TraceMessageProgramData("@@@CPUGPUCombine, SHUANGKAO off", "cpugpuDTRunTimer_Elapsed", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\MySystemManager.cs", 887);
					log("@@@CPUGPUCombine, SHUANGKAO off");
					if (!m_CPUGPUCombineGame_ON)
					{
						WMIEC.Instance.WMICPUGPUCombineRunONGame();
						m_CPUGPUCombineGame_ON = true;
						log("@@@CPUGPUCombineGame, WMICPUGPUCombineRunONGame");
						LogCtrl.TraceMessageProgramData("@@@CPUGPUCombineGame, WMICPUGPUCombineRunONGame", "cpugpuDTRunTimer_Elapsed", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\MySystemManager.cs", 894);
					}
					else
					{
						log("@@@CPUGPUCombineGame is on, do nothing");
					}
				}
				else
				{
					log("@@@CPUGPUCombineGame, Under CPUGPUConbineONOFFReturnValue = " + ResolvedDelegates.System_Byte_ToString__119(ref CPUGPUConbineONOFFReturnValue) + ", WMICPUGPUCombineRunONGame not call");
				}
			}
			else if (GPUOverLoadCount_game <= -1 && (m_CPUGPUCombineGame_ON || m_CPUGPUCombineGame_LanuchCondition == -1))
			{
				log("@@@CPUGPUCombineGame is on, trun off");
				LogCtrl.TraceMessageProgramData("@@@CPUGPUCombineGame is on, trun off", "cpugpuDTRunTimer_Elapsed", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\MySystemManager.cs", 911);
				WMIEC.Instance.WMICPUGPUCombineRunOFFGame();
				m_CPUGPUCombineGame_ON = false;
				m_CPUGPUCombineGame_LanuchCondition = 0;
			}
		}
		Interlocked.Exchange(ref CPUGPUConbineONOFFTimersCount, 0);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void RestoreCPUGPUCombineParams()
	{
		try
		{
			if (ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2", "DefaultCPUGPUCombine", 0)), "1"))
			{
				m_CPUGPUCombine_ON = false;
				m_CPUGPUCombineGame_ON = false;
				CPUOverLoadCount = 0;
				GPUOverLoadCount_cpugpu = 0;
				GPUOverLoadCount_game = 0;
				CPUGPUConbineONOFFReturnValue = 0;
				GPUConbineONOFFReturnValue = 0;
				LogCtrl.TraceMessageProgramData("CPUGPUCombineParams has restored.", "RestoreCPUGPUCombineParams", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MySystem\\MySystemManager.cs", 1020);
			}
		}
		finally
		{
			RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2", "DefaultCPUGPUCombine", 0, RegistryValueKind.DWord);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe static bool IsProcessExist(string searchString)
	{
		bool result = false;
		try
		{
			Process[] array = ResolvedDelegates.System_Diagnostics_Process_GetProcesses__478();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].ProcessName.ToUpper().Contains(ResolvedDelegates.System_String_ToUpper__331(ref *(string*)searchString)))
				{
					result = true;
					break;
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Format__7("IsProcessExist Failed {0}", ResolvedDelegates.System_Object_ToString__1(ex)));
			return result;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void CpuinfoTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		object cpuInfo = GetCpuInfo();
		WindowsW_Viewmodel windowsW_Viewmodel = ww_viewmodel;
		if (<>o__92.<>p__2 == null)
		{
			<>o__92.<>p__2 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, uint> target = <>o__92.<>p__2.Target;
		CallSite<Func<CallSite, object, uint>> arg = <>o__92.<>p__2;
		if (<>o__92.<>p__1 == null)
		{
			<>o__92.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target2 = <>o__92.<>p__1.Target;
		CallSite<Func<CallSite, Type, object, object>> arg2 = <>o__92.<>p__1;
		Type arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__92.<>p__0 == null)
		{
			<>o__92.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "CpuUsage", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		windowsW_Viewmodel.CPUUsage = target(arg, target2(arg2, arg3, <>o__92.<>p__0.Target(<>o__92.<>p__0, cpuInfo)));
		WindowsW_Viewmodel windowsW_Viewmodel2 = ww_viewmodel;
		if (<>o__92.<>p__5 == null)
		{
			<>o__92.<>p__5 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, uint> target3 = <>o__92.<>p__5.Target;
		CallSite<Func<CallSite, object, uint>> arg4 = <>o__92.<>p__5;
		if (<>o__92.<>p__4 == null)
		{
			<>o__92.<>p__4 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target4 = <>o__92.<>p__4.Target;
		CallSite<Func<CallSite, Type, object, object>> arg5 = <>o__92.<>p__4;
		Type arg6 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__92.<>p__3 == null)
		{
			<>o__92.<>p__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "CpuTemperature", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		windowsW_Viewmodel2.CpuTemperature = target3(arg4, target4(arg5, arg6, <>o__92.<>p__3.Target(<>o__92.<>p__3, cpuInfo)));
		if (<>o__92.<>p__6 == null)
		{
			<>o__92.<>p__6 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		<>o__92.<>p__6.Target(<>o__92.<>p__6, m_View, "System/CpuInfo", cpuInfo);
		if (<>o__92.<>p__8 == null)
		{
			<>o__92.<>p__8 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, string> target5 = <>o__92.<>p__8.Target;
		CallSite<Func<CallSite, object, string>> arg7 = <>o__92.<>p__8;
		if (<>o__92.<>p__7 == null)
		{
			<>o__92.<>p__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "CpuUsage", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		beforeCpuValue = target5(arg7, <>o__92.<>p__7.Target(<>o__92.<>p__7, cpuInfo));
		object memoryInfo = GetMemoryInfo();
		WindowsW_Viewmodel windowsW_Viewmodel3 = ww_viewmodel;
		if (<>o__92.<>p__11 == null)
		{
			<>o__92.<>p__11 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, uint> target6 = <>o__92.<>p__11.Target;
		CallSite<Func<CallSite, object, uint>> arg8 = <>o__92.<>p__11;
		if (<>o__92.<>p__10 == null)
		{
			<>o__92.<>p__10 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target7 = <>o__92.<>p__10.Target;
		CallSite<Func<CallSite, Type, object, object>> arg9 = <>o__92.<>p__10;
		Type arg10 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__92.<>p__9 == null)
		{
			<>o__92.<>p__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "MemoryUsage", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		windowsW_Viewmodel3.MemoryUsage = target6(arg8, target7(arg9, arg10, <>o__92.<>p__9.Target(<>o__92.<>p__9, memoryInfo)));
		if (<>o__92.<>p__12 == null)
		{
			<>o__92.<>p__12 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		<>o__92.<>p__12.Target(<>o__92.<>p__12, m_View, "System/MemoryInfo", memoryInfo);
		if (GetNetworkStatus())
		{
			object networkInfo = GetNetworkInfo();
			if (<>o__92.<>p__13 == null)
			{
				<>o__92.<>p__13 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			<>o__92.<>p__13.Target(<>o__92.<>p__13, m_View, "System/NetworkInfo", networkInfo);
		}
		if (CustomizeCtrl.GetIsNewUMASupport() && CustomizeCtrl.GetIsUMA())
		{
			return;
		}
		object gpuInfo = GetGpuInfo();
		ww_viewmodel.GpuUsage = 0u;
		ww_viewmodel.GpuTemperature = 0u;
		if (<>o__92.<>p__15 == null)
		{
			<>o__92.<>p__15 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target8 = <>o__92.<>p__15.Target;
		CallSite<Func<CallSite, object, bool>> arg11 = <>o__92.<>p__15;
		if (<>o__92.<>p__14 == null)
		{
			<>o__92.<>p__14 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		if (!target8(arg11, <>o__92.<>p__14.Target(<>o__92.<>p__14, gpuInfo, null)))
		{
			return;
		}
		WindowsW_Viewmodel windowsW_Viewmodel4 = ww_viewmodel;
		if (<>o__92.<>p__18 == null)
		{
			<>o__92.<>p__18 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, uint> target9 = <>o__92.<>p__18.Target;
		CallSite<Func<CallSite, object, uint>> arg12 = <>o__92.<>p__18;
		if (<>o__92.<>p__17 == null)
		{
			<>o__92.<>p__17 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target10 = <>o__92.<>p__17.Target;
		CallSite<Func<CallSite, Type, object, object>> arg13 = <>o__92.<>p__17;
		Type arg14 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		object obj = gpuInfo;
		object arg15;
		if (obj == null)
		{
			arg15 = null;
		}
		else
		{
			if (<>o__92.<>p__16 == null)
			{
				<>o__92.<>p__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "GpuUsage", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg15 = <>o__92.<>p__16.Target(<>o__92.<>p__16, obj);
		}
		windowsW_Viewmodel4.GpuUsage = target9(arg12, target10(arg13, arg14, arg15));
		WindowsW_Viewmodel windowsW_Viewmodel5 = ww_viewmodel;
		if (<>o__92.<>p__21 == null)
		{
			<>o__92.<>p__21 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, uint> target11 = <>o__92.<>p__21.Target;
		CallSite<Func<CallSite, object, uint>> arg16 = <>o__92.<>p__21;
		if (<>o__92.<>p__20 == null)
		{
			<>o__92.<>p__20 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target12 = <>o__92.<>p__20.Target;
		CallSite<Func<CallSite, Type, object, object>> arg17 = <>o__92.<>p__20;
		Type arg18 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__92.<>p__19 == null)
		{
			<>o__92.<>p__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "GpuTemperature", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		windowsW_Viewmodel5.GpuTemperature = target11(arg16, target12(arg17, arg18, <>o__92.<>p__19.Target(<>o__92.<>p__19, gpuInfo)));
		if (<>o__92.<>p__24 == null)
		{
			<>o__92.<>p__24 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, int> target13 = <>o__92.<>p__24.Target;
		CallSite<Func<CallSite, object, int>> arg19 = <>o__92.<>p__24;
		if (<>o__92.<>p__23 == null)
		{
			<>o__92.<>p__23 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target14 = <>o__92.<>p__23.Target;
		CallSite<Func<CallSite, Type, object, object>> arg20 = <>o__92.<>p__23;
		Type arg21 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__92.<>p__22 == null)
		{
			<>o__92.<>p__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "GpuTemperature", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		target13(arg19, target14(arg20, arg21, <>o__92.<>p__22.Target(<>o__92.<>p__22, gpuInfo)));
		if (<>o__92.<>p__27 == null)
		{
			<>o__92.<>p__27 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle)));
		}
		Func<CallSite, object, int> target15 = <>o__92.<>p__27.Target;
		CallSite<Func<CallSite, object, int>> arg22 = <>o__92.<>p__27;
		if (<>o__92.<>p__26 == null)
		{
			<>o__92.<>p__26 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target16 = <>o__92.<>p__26.Target;
		CallSite<Func<CallSite, Type, object, object>> arg23 = <>o__92.<>p__26;
		Type arg24 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__92.<>p__25 == null)
		{
			<>o__92.<>p__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "GpuPState", ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		target15(arg22, target16(arg23, arg24, <>o__92.<>p__25.Target(<>o__92.<>p__25, gpuInfo)));
		if (<>o__92.<>p__28 == null)
		{
			<>o__92.<>p__28 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		<>o__92.<>p__28.Target(<>o__92.<>p__28, m_View, "System/GpuInfo", gpuInfo);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void DiskinfoTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		try
		{
			object diskInfo = GetDiskInfo();
			if (<>o__93.<>p__1 == null)
			{
				<>o__93.<>p__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target = <>o__93.<>p__1.Target;
			CallSite<Func<CallSite, object, bool>> arg = <>o__93.<>p__1;
			if (<>o__93.<>p__0 == null)
			{
				<>o__93.<>p__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (target(arg, <>o__93.<>p__0.Target(<>o__93.<>p__0, diskInfo, null)))
			{
				if (<>o__93.<>p__2 == null)
				{
					<>o__93.<>p__2 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				<>o__93.<>p__2.Target(<>o__93.<>p__2, m_View, "System/DiskInfo", diskInfo);
			}
		}
		catch (Exception)
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void CpuTempDbTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		_ = SqliteDbSwitch;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GetCalanderData(DateTime dt)
	{
		new Calendar.ChineseCalendar(dt);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GetGraphicInfo()
	{
		try
		{
			object arg = new
			{
				GraphicInfo = _GraphicInfo
			};
			if (<>o__97.<>p__0 == null)
			{
				<>o__97.<>p__0 = CallSite<Action<CallSite, MySystemView, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SendStatusToClient", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MySystemManager).TypeHandle), new CSharpArgumentInfo[3]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			<>o__97.<>p__0.Target(<>o__97.<>p__0, m_View, "System/HardwareInfo", arg);
		}
		catch (Exception ex)
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Graphic info ", ResolvedDelegates.System_Object_ToString__1(ex)));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object GetFunError()
	{
		return new
		{
			FanErrorStatus = _FanFunctionErrorInfo.GetECFanAlert()
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object GetFunInfo()
	{
		if ((int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\ItemSupport", "IsNvGpu", 0) == 1)
		{
			return new
			{
				CpuFanDuty = _FanInfo.GetEcCpuFanDuty(),
				GpuFanDuty = _FanInfo.GetEcGpuFanDuty(),
				CpuFanRpm = _FanInfo.GetEcCpuFanRpm(),
				GpuFanRpm = _FanInfo.GetEcGpuFanRpm()
			};
		}
		return new
		{
			CpuFanDuty = _FanInfo.GetEcCpuFanDuty(),
			CpuFanRpm = _FanInfo.GetEcCpuFanRpm()
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object GetHwFuelGaugeInfo()
	{
		return new
		{
			DesignCapacity = _HwFuelGaugeInfo.GetDesignCapacity(),
			DesignVoltage = _HwFuelGaugeInfo.GetECDesignVoltage(),
			Voltage = _HwFuelGaugeInfo.GetVoltage(),
			Current = _HwFuelGaugeInfo.GetECCurrent()
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object GetCpuInfo()
	{
		return new
		{
			CpuUsage = GetCpuUsage(),
			CpuTemperature = GetCpuTemperature(),
			CpuFrequency = GetCpuFrequency(),
			CpuMaxFrequency = cCpuInfo.maxFrequency
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object GetCpuInfoforCPUGPUCombine()
	{
		return new
		{
			CpuUsage = GetCpuUsageforCPUGPUCombine()
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private uint GetGpuCombineUsageOnly()
	{
		return cGpuInfo.GetGpuUsage();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object GetCpuCombineUsageOnly()
	{
		return new
		{
			CpuUsage = GetCpuUsage()
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object GetMemoryInfo()
	{
		return GetMemoryUsage();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetGpuUsageOnly()
	{
		return GetGpuUsage();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public string GetGpuPStateOnly()
	{
		return GetGpuPState();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object GetGpuInfo()
	{
		if (gpuCommandCount == 0 && ResolvedDelegates.System_Threading_Monitor_TryEnter__145(gpuInfoLock, 2000))
		{
			try
			{
				gpuCommandCount++;
				return new
				{
					GpuUsage = GetGpuUsage(),
					GpuTemperature = GetGpuTemperature(),
					GpuCoreFreq = GetGpuCoreFreq(),
					GpuMemFreq = GetGpuMemFreq(),
					GpuPState = GetGpuPState(),
					GpuMem = GetGpuPhysicalRamSize()
				};
			}
			catch
			{
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(gpuInfoLock);
				gpuCommandCount = 0;
			}
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object GetBatteryInfo()
	{
		return new
		{
			BatteryLifePercent = GetBatteryLifePercent(),
			BatteryLifeRemaining = GetBatteryLifeRemaining(),
			BatteryAbnormal = GetBatteryAbnormal(),
			BatteryCapacity = cBatteryInfo.GetBatteryCapacity(),
			BatteryCycleCount = cBatteryInfo.GetECBatteryCycleCount()
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object InitBatteryInfo()
	{
		return new
		{
			BatteryLifePercent = GetBatteryLifePercent(),
			BatteryLifeRemaining = GetBatteryLifeRemaining(),
			BatteryAbnormal = cBatteryInfo.Init(),
			BatteryCapacity = cBatteryInfo.GetBatteryCapacity(),
			BatteryCycleCount = cBatteryInfo.GetECBatteryCycleCount()
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool GetNetworkStatus()
	{
		return cNetworkInfo.CheckNetworkInfo();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private object GetNetworkInfo()
	{
		double size = 0.0;
		string unit = "Kbps";
		double size2 = 0.0;
		string unit2 = "Kbps";
		cNetworkInfo.CheckNetworkInfo();
		double downloadSpeed = cNetworkInfo.GetDownloadSpeed();
		double uploadSpeed = cNetworkInfo.GetUploadSpeed();
		cNetworkInfo.FormatSpeed(downloadSpeed, ref size, ref unit, 1);
		cNetworkInfo.FormatSpeed(uploadSpeed, ref size2, ref unit2, 1);
		double num = ResolvedDelegates.System_Math_Round__495(size, 1);
		string networkDownload = ResolvedDelegates.System_String_Concat__107(ResolvedDelegates.System_Double_ToString__585(ref num), " ", unit);
		num = ResolvedDelegates.System_Math_Round__495(size2, 1);
		return new
		{
			NetworkDownload = networkDownload,
			NetworkUpload = ResolvedDelegates.System_String_Concat__107(ResolvedDelegates.System_Double_ToString__585(ref num), " ", unit2)
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public object GetDiskInfo()
	{
		if (!ResolvedDelegates.System_Threading_Monitor_TryEnter__145(diskInfoLock, 1000))
		{
			ResolvedDelegates.System_Threading_Interlocked_Increment__616(ref disErrorCount);
			return null;
		}
		try
		{
			cDiskInfo.UpdateDiskInfo();
			var result = new
			{
				SSD_One_Model = GetSSD_One_Model(),
				SSD_One_Temperature = GetSSD_One_Temperature(),
				SSD_One_PowerOnHours = GetSSD_One_PowerOnHours(),
				SSD_One_PowerOnCount = GetSSD_One_PowerOnCount(),
				SSD_One_Interface = GetSSD_One_Interface(),
				SSD_Two_Model = GetSSD_Two_Model(),
				SSD_Two_Temperature = GetSSD_Two_Temperature(),
				SSD_Two_PowerOnHours = GetSSD_Two_PowerOnHours(),
				SSD_Two_PowerOnCount = GetSSD_Two_PowerOnCount(),
				SSD_Two_Interface = GetSSD_Two_Interface(),
				HDDModel = GetHDDModel(),
				HDDTemperature = GetHDDTemperature(),
				HDDPowerOnHours = GetHDDPowerOnHours(),
				HDDPowerOnCount = GetHDDPowerOnCount(),
				HDDInterface = GetHDDInterface()
			};
			cDiskInfo.ResetArray();
			return result;
		}
		finally
		{
			ResolvedDelegates.System_Threading_Monitor_Exit__71(diskInfoLock);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetCpuTemperature()
	{
		int cPUTemperature = cCpuInfo.GetCPUTemperature();
		return ResolvedDelegates.System_Int32_ToString__55(ref cPUTemperature);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetCpuFrequency()
	{
		int actualCpuFrequencyMHz = cCpuInfo.GetActualCpuFrequencyMHz();
		return ResolvedDelegates.System_Int32_ToString__55(ref actualCpuFrequencyMHz);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetCpuUsage()
	{
		int cPUUsage = cCpuInfo.GetCPUUsage();
		return ResolvedDelegates.System_Int32_ToString__55(ref cPUUsage);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetCpuUsageforCPUGPUCombine()
	{
		int cPUUsageforCPUGPUCombine = cCpuInfo.GetCPUUsageforCPUGPUCombine();
		return ResolvedDelegates.System_Int32_ToString__55(ref cPUUsageforCPUGPUCombine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetGpuTemperature()
	{
		uint gpuTemperature = cGpuInfo.GetGpuTemperature();
		return ResolvedDelegates.System_UInt32_ToString__78(ref gpuTemperature);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetGpuUsage()
	{
		uint gpuUsage = cGpuInfo.GetGpuUsage();
		return ResolvedDelegates.System_UInt32_ToString__78(ref gpuUsage);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetGpuCoreFreq()
	{
		uint gpuCoreFreq = cGpuInfo.GetGpuCoreFreq();
		return ResolvedDelegates.System_UInt32_ToString__78(ref gpuCoreFreq);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetGpuMemFreq()
	{
		uint gpuMemFreq = cGpuInfo.GetGpuMemFreq();
		return ResolvedDelegates.System_UInt32_ToString__78(ref gpuMemFreq);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetGpuPState()
	{
		uint gpuPState = cGpuInfo.GetGpuPState();
		return ResolvedDelegates.System_UInt32_ToString__78(ref gpuPState);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetGpuPhysicalRamSize()
	{
		ulong gpuPhysicalRamSize = cGpuInfo.GetGpuPhysicalRamSize();
		return ResolvedDelegates.System_UInt64_ToString__586(ref gpuPhysicalRamSize);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private dynamic GetMemoryUsage()
	{
		return cMemoryInfo.GetMemoryUsage();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetNetworkDownload()
	{
		double downloadSpeed = cNetworkInfo.GetDownloadSpeed();
		return ResolvedDelegates.System_Double_ToString__585(ref downloadSpeed);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetNetworkUpload()
	{
		double uploadSpeed = cNetworkInfo.GetUploadSpeed();
		return ResolvedDelegates.System_Double_ToString__585(ref uploadSpeed);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetBatteryLifePercent()
	{
		return cBatteryInfo.GetBatteryLifePercent();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetBatteryLifeRemaining()
	{
		return cBatteryInfo.GetBatteryLifeRemaining();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetBatteryAbnormal()
	{
		return cBatteryInfo.GetBatteryAbnormal();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_One_Model()
	{
		return cDiskInfo.GetSSD_One_Model();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_One_Temperature()
	{
		return cDiskInfo.GetSSD_One_Temperature();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_One_PowerOnHours()
	{
		return cDiskInfo.GetSSD_One_PowerOnHours();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_One_PowerOnCount()
	{
		return cDiskInfo.GetSSD_One_PowerOnCount();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_One_Interface()
	{
		return cDiskInfo.GetSSD_One_Interface();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_One_Usage()
	{
		return cDiskInfo.GetSSD_One_Usage();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_Two_Model()
	{
		return cDiskInfo.GetSSD_Two_Model();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_Two_Temperature()
	{
		return cDiskInfo.GetSSD_Two_Temperature();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_Two_PowerOnHours()
	{
		return cDiskInfo.GetSSD_Two_PowerOnHours();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_Two_PowerOnCount()
	{
		return cDiskInfo.GetSSD_Two_PowerOnCount();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_Two_Interface()
	{
		return cDiskInfo.GetSSD_Two_Interface();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetSSD_Two_Usage()
	{
		return cDiskInfo.GetSSD_Two_Usage();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetHDDModel()
	{
		return cDiskInfo.GetHDDModel();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetHDDTemperature()
	{
		return cDiskInfo.GetHDDTemperature();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetHDDPowerOnHours()
	{
		return cDiskInfo.GetHDDPowerOnHours();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetHDDPowerOnCount()
	{
		return cDiskInfo.GetHDDPowerOnCount();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetHDDInterface()
	{
		return cDiskInfo.GetHDDInterface();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetHDDUsage()
	{
		return cDiskInfo.GetHDDUsage();
	}
}

// Static analysis rendering; not original buildable source.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using GCUService.MyRgbKeyboard;
using LightingModel;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using MyECIO;
using Utility;

namespace MyControlCenter.MyRgbKeyboard;

public class HIDKeyboard : IDisposable
{
	[CompilerGenerated]
	private sealed class <>c__DisplayClass112_0
	{
		public HIDKeyboard <>4__this;

		public bool NightPower;

		public <>c__DisplayClass112_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <NightPowerSwitch>b__0()
		{
			if (!ResolvedDelegates.System_Threading_Monitor_TryEnter__145(<>4__this._nightModeLock, 200))
			{
				return;
			}
			try
			{
				if (NightPower)
				{
					RGB_S lastNightModeColor = <>4__this.BackgroundColorConverter(<>4__this._EffectData.save_layout_backgroundcolor);
					<>4__this._KeyboardControl?.HID_Set_Effect_Type_08H(4, 0, lastNightModeColor.R, lastNightModeColor.G, lastNightModeColor.B, 0, 0);
					<>4__this._LastNightModeColor = lastNightModeColor;
				}
				else
				{
					<>4__this._KeyboardControl?.HID_Set_Effect_Type_08H(5, 0, 0, 0, 0, 0, 0);
					<>4__this._LastNightModeColor = new RGB_S
					{
						R = 0,
						G = 0,
						B = 0
					};
				}
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(<>4__this._nightModeLock);
			}
		}
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass130_0
	{
		public HIDKeyboard <>4__this;

		public byte SAVED;

		public <>c__DisplayClass130_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <RunEffct>b__0()
		{
			<>4__this._KeyboardControl?.Set_Lighting_Effect(2, <>4__this._EffectData.save_effect, 0, <>4__this._EffectData.save_speed, <>4__this._EffectData.save_direction, SAVED, <>4__this._EffectData.save_layout_color, 0, <>4__this._EffectData.save_layout_alphbet);
		}

		internal void <RunEffct>b__1(Task task)
		{
			<>4__this.OpenAnimation();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <RunEffct>b__2()
		{
			<>4__this._KeyboardControl?.Set_Lighting_Effect(4, <>4__this._EffectData.save_effect, 0, <>4__this._EffectData.save_speed, <>4__this._EffectData.save_direction, SAVED, <>4__this._EffectData.save_layout_color, 0, <>4__this._EffectData.save_layout_alphbet);
		}

		internal void <RunEffct>b__3(Task task)
		{
			<>4__this.OpenAnimation();
		}
	}

	[CompilerGenerated]
	private static class <>o__105
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string>> <>p__2;

		public static CallSite<Func<CallSite, object, string, object>> <>p__3;

		public static CallSite<Func<CallSite, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, string>> <>p__5;

		public static CallSite<Func<CallSite, object, string, object>> <>p__6;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__7;

		public static CallSite<Func<CallSite, object, uint>> <>p__8;

		public static CallSite<Func<CallSite, object, string, object>> <>p__9;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__10;

		public static CallSite<Func<CallSite, object, uint>> <>p__11;

		public static CallSite<Func<CallSite, object, string, object>> <>p__12;

		public static CallSite<Func<CallSite, object, object, object>> <>p__13;

		public static CallSite<Func<CallSite, object, bool>> <>p__14;

		public static CallSite<Func<CallSite, object, string, object>> <>p__15;

		public static CallSite<Func<CallSite, object, object>> <>p__16;

		public static CallSite<Func<CallSite, object, string>> <>p__17;

		public static CallSite<Func<CallSite, object, string, object>> <>p__18;

		public static CallSite<Func<CallSite, object, object, object>> <>p__19;

		public static CallSite<Func<CallSite, object, bool>> <>p__20;

		public static CallSite<Func<CallSite, object, string, object>> <>p__21;

		public static CallSite<Func<CallSite, object, object>> <>p__22;

		public static CallSite<Func<CallSite, object, string>> <>p__23;

		public static CallSite<Func<CallSite, object, string, object>> <>p__24;

		public static CallSite<Func<CallSite, HIDKeyboard, object, object>> <>p__25;

		public static CallSite<Func<CallSite, object, RGBKB_Color>> <>p__26;

		public static CallSite<Func<CallSite, object, string, object>> <>p__27;

		public static CallSite<Func<CallSite, object, object, object>> <>p__28;

		public static CallSite<Func<CallSite, object, bool>> <>p__29;

		public static CallSite<Func<CallSite, object, string, object>> <>p__30;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__31;

		public static CallSite<Func<CallSite, object, int>> <>p__32;

		public static CallSite<Func<CallSite, object, string, object>> <>p__33;

		public static CallSite<Func<CallSite, object, object, object>> <>p__34;

		public static CallSite<Func<CallSite, bool, object, object>> <>p__35;

		public static CallSite<Func<CallSite, object, bool>> <>p__36;

		public static CallSite<Func<CallSite, object, string, object>> <>p__37;

		public static CallSite<Func<CallSite, object, object>> <>p__38;

		public static CallSite<Func<CallSite, object, string>> <>p__39;
	}

	[CompilerGenerated]
	private static class <>o__106
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string>> <>p__2;

		public static CallSite<Func<CallSite, object, string, object>> <>p__3;

		public static CallSite<Func<CallSite, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, string>> <>p__5;

		public static CallSite<Func<CallSite, object, string, object>> <>p__6;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__7;

		public static CallSite<Func<CallSite, object, uint>> <>p__8;

		public static CallSite<Func<CallSite, object, string, object>> <>p__9;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__10;

		public static CallSite<Func<CallSite, object, uint>> <>p__11;

		public static CallSite<Func<CallSite, object, string, object>> <>p__12;

		public static CallSite<Func<CallSite, object, object>> <>p__13;

		public static CallSite<Func<CallSite, object, string>> <>p__14;

		public static CallSite<Func<CallSite, object, string, object>> <>p__15;

		public static CallSite<Func<CallSite, HIDKeyboard, object, object>> <>p__16;

		public static CallSite<Func<CallSite, object, RGBKB_Color>> <>p__17;

		public static CallSite<Func<CallSite, object, string, object>> <>p__18;

		public static CallSite<Func<CallSite, object, object>> <>p__19;

		public static CallSite<Func<CallSite, object, string>> <>p__20;

		public static CallSite<Func<CallSite, object, string, object>> <>p__21;

		public static CallSite<Func<CallSite, object, object, object>> <>p__22;

		public static CallSite<Func<CallSite, object, bool>> <>p__23;

		public static CallSite<Func<CallSite, object, string, object>> <>p__24;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__25;

		public static CallSite<Func<CallSite, object, int>> <>p__26;

		public static CallSite<Func<CallSite, object, string, object>> <>p__27;

		public static CallSite<Func<CallSite, object, object, object>> <>p__28;

		public static CallSite<Func<CallSite, bool, object, object>> <>p__29;

		public static CallSite<Func<CallSite, object, bool>> <>p__30;

		public static CallSite<Func<CallSite, object, string, object>> <>p__31;

		public static CallSite<Func<CallSite, object, object>> <>p__32;

		public static CallSite<Func<CallSite, object, string>> <>p__33;
	}

	[CompilerGenerated]
	private static class <>o__73
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, bool>> <>p__2;

		public static CallSite<Func<CallSite, object, string, object>> <>p__3;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, uint>> <>p__5;

		public static CallSite<Func<CallSite, object, string, object>> <>p__6;

		public static CallSite<Func<CallSite, object, int, object>> <>p__7;

		public static CallSite<Func<CallSite, object, string, object>> <>p__8;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__9;

		public static CallSite<Func<CallSite, object, uint>> <>p__10;

		public static CallSite<Func<CallSite, object, string, object>> <>p__11;

		public static CallSite<Func<CallSite, object, int, object>> <>p__12;

		public static CallSite<Func<CallSite, object, string, object>> <>p__13;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__14;

		public static CallSite<Func<CallSite, object, byte>> <>p__15;

		public static CallSite<Func<CallSite, object, string, object>> <>p__16;

		public static CallSite<Func<CallSite, object, int, object>> <>p__17;

		public static CallSite<Func<CallSite, object, string, object>> <>p__18;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__19;

		public static CallSite<Func<CallSite, object, byte>> <>p__20;

		public static CallSite<Func<CallSite, object, string, object>> <>p__21;

		public static CallSite<Func<CallSite, object, int, object>> <>p__22;

		public static CallSite<Func<CallSite, object, string, object>> <>p__23;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__24;

		public static CallSite<Func<CallSite, object, byte>> <>p__25;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <InitalizeKeyboardEffect>d__92 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public HIDKeyboard <>4__this;

		public bool runeffect;

		public bool unplug;

		private TaskAwaiter <>u__1;

		private uint <aclevel>5__2;

		private uint <dclevel>5__3;

		private RGBKB_PowerStatus <powerstatus>5__4;

		private string <project>5__5;

		private TaskAwaiter<uint> <>u__2;

		private TaskAwaiter<RGBKB_PowerStatus> <>u__3;

		private TaskAwaiter<SAVE_LIGHTING_EFFECT_DATA> <>u__4;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			int num = <>1__state;
			HIDKeyboard hIDKeyboard = <>4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 3u)
					{
						goto IL_009c;
					}
					awaiter = ResolvedDelegates.System_Threading_Tasks_Task_GetAwaiter__23(ResolvedDelegates.System_Threading_SemaphoreSlim_WaitAsync__181(hIDKeyboard._semaphore));
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
				goto IL_009c;
				IL_009c:
				try
				{
					TaskAwaiter<uint> awaiter4;
					TaskAwaiter<RGBKB_PowerStatus> awaiter3;
					TaskAwaiter<SAVE_LIGHTING_EFFECT_DATA> awaiter2;
					uint result;
					RGBKB_PowerStatus result2;
					int projectID;
					CancellationTokenSource cancellationTokenSource;
					switch (num)
					{
					default:
					{
						bool flag = false;
						awaiter4 = SettingsStorageExtensions.ReadAsync<uint>("", hIDKeyboard._SaveACLight).GetAwaiter();
						if (!awaiter4.IsCompleted)
						{
							num = (<>1__state = 1);
							<>u__2 = awaiter4;
							<>t__builder.AwaitUnsafeOnCompleted(ref awaiter4, ref this);
							return;
						}
						goto IL_011c;
					}
					case 1:
						awaiter4 = <>u__2;
						<>u__2 = default;
						num = (<>1__state = -1);
						goto IL_011c;
					case 2:
						awaiter4 = <>u__2;
						<>u__2 = default;
						num = (<>1__state = -1);
						goto IL_0193;
					case 3:
						awaiter3 = <>u__3;
						<>u__3 = default;
						num = (<>1__state = -1);
						goto IL_020a;
					case 4:
						{
							awaiter2 = <>u__4;
							<>u__4 = default;
							num = (<>1__state = -1);
							break;
						}
						IL_0193:
						result = awaiter4.GetResult();
						<dclevel>5__3 = result;
						awaiter3 = SettingsStorageExtensions.ReadAsync<RGBKB_PowerStatus>("", hIDKeyboard._SavePowerSwich).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (<>1__state = 3);
							<>u__3 = awaiter3;
							<>t__builder.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
							return;
						}
						goto IL_020a;
						IL_011c:
						result = awaiter4.GetResult();
						<aclevel>5__2 = result;
						awaiter4 = SettingsStorageExtensions.ReadAsync<uint>("", hIDKeyboard._SaveDCLight).GetAwaiter();
						if (!awaiter4.IsCompleted)
						{
							num = (<>1__state = 2);
							<>u__2 = awaiter4;
							<>t__builder.AwaitUnsafeOnCompleted(ref awaiter4, ref this);
							return;
						}
						goto IL_0193;
						IL_020a:
						result2 = awaiter3.GetResult();
						<powerstatus>5__4 = result2;
						hIDKeyboard.mbLogo = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\Support", "MBALogo", 0);
						LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Init HID Device powerstatus : ", <powerstatus>5__4.ToString()));
						projectID = hIDKeyboard._KeyboardControl.GetProjectID();
						<project>5__5 = ResolvedDelegates.System_Int32_ToString__55(ref projectID);
						LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Init HID Device projectid : ", <project>5__5));
						cancellationTokenSource = new CancellationTokenSource();
						awaiter2 = SettingsStorageExtensions.ReadAsync<SAVE_LIGHTING_EFFECT_DATA>(hIDKeyboard._SaveEffectType, ResolvedDelegates.System_String_Concat__29(<project>5__5, "_LastEffect"), ResolvedDelegates.System_Threading_CancellationTokenSource_get_Token__167(cancellationTokenSource)).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (<>1__state = 4);
							<>u__4 = awaiter2;
							<>t__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
						break;
					}
					SAVE_LIGHTING_EFFECT_DATA result3 = awaiter2.GetResult();
					LogCtrl.Write(ResolvedDelegates.System_String_Concat__79(new string[7]
					{
						"******** current effect ",
						ResolvedDelegates.System_Byte_ToString__119(ref result3.save_effect),
						"********  _SaveEffectType ",
						hIDKeyboard._SaveEffectType,
						"********  project ",
						<project>5__5,
						"******** SaveLastEffect LastEffect"
					}));
					if (result3.save_effect == 0)
					{
						List<Color> list = new List<Color>
						{
							Color.FromArgb(byte.MaxValue, byte.MaxValue, (byte)0, (byte)0),
							Color.FromArgb(byte.MaxValue, byte.MaxValue, (byte)165, (byte)0),
							Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)0),
							Color.FromArgb(byte.MaxValue, (byte)0, byte.MaxValue, (byte)0),
							Color.FromArgb(byte.MaxValue, (byte)0, (byte)0, byte.MaxValue),
							Color.FromArgb(byte.MaxValue, (byte)0, byte.MaxValue, byte.MaxValue),
							Color.FromArgb(byte.MaxValue, (byte)139, (byte)0, byte.MaxValue)
						};
						RGBKB_Color save_layout_color = new RGBKB_Color(bCircular: false, 7u);
						int num2 = 0;
						List<Color>.Enumerator enumerator = list.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								Color current = enumerator.Current;
								save_layout_color.ColorBuffer[num2].ID = (uint)num2;
								save_layout_color.ColorBuffer[num2].R = current.R;
								save_layout_color.ColorBuffer[num2].G = current.G;
								save_layout_color.ColorBuffer[num2].B = current.B;
								num2++;
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
							}
						}
						hIDKeyboard._EffectData.save_mode = RGBKB_Mode.Lighting;
						hIDKeyboard._EffectData.save_effect = hIDKeyboard.Translate_ITE_EffectIndex(RGBKB_Effect.Rainbow);
						hIDKeyboard._EffectData.save_light = hIDKeyboard.Translate_ITE_LightValue(3u);
						hIDKeyboard._EffectData.save_speed = hIDKeyboard.Translate_ITE_SpeedValue(2u);
						hIDKeyboard._EffectData.save_direction = hIDKeyboard.Translate_ITE_DirectionValue(RGBKB_Direction.LeftRight);
						hIDKeyboard._EffectData.save_layout_color = save_layout_color;
						hIDKeyboard.SaveEffectData();
					}
					else
					{
						hIDKeyboard._EffectData = result3;
					}
					if (runeffect)
					{
						if (!MyEcCtrl.Instance.IsChinaMode())
						{
							bool flag = MyEcCtrl.Instance.IsChinaModeOn();
							LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("China Mode Stauts : ", ResolvedDelegates.System_Boolean_ToString__147(ref flag)));
							if (<powerstatus>5__4 == RGBKB_PowerStatus.On && !flag)
							{
								hIDKeyboard.SetPowerStatus(RGBKB_PowerStatus.Off, 0, hIDKeyboard._SavePowerSwich);
							}
							else
							{
								hIDKeyboard._PowerStatus = <powerstatus>5__4;
								hIDKeyboard.SetPowerStatus(hIDKeyboard._PowerStatus, 0, hIDKeyboard._SavePowerSwich);
							}
						}
						else
						{
							LogCtrl.Write("China Mode Stauts : no support");
							hIDKeyboard._PowerStatus = <powerstatus>5__4;
							hIDKeyboard.SetPowerStatus(hIDKeyboard._PowerStatus, 0, hIDKeyboard._SavePowerSwich);
						}
					}
					else
					{
						hIDKeyboard._PowerStatus = <powerstatus>5__4;
					}
					LogCtrl.Write("After China Mode PoserStatus : " + <powerstatus>5__4);
					if (runeffect)
					{
						hIDKeyboard.SetACDCLight(unplug, <aclevel>5__2, <dclevel>5__3);
						if (hIDKeyboard._EffectData.save_effect != 0)
						{
							hIDKeyboard.RunEffct(0, 2);
						}
					}
					<project>5__5 = null;
				}
				catch (Exception ex)
				{
					LogCtrl.TraceMessage("exception: " + ex, "InitalizeKeyboardEffect", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 548);
				}
				finally
				{
					if (num < 0)
					{
						ResolvedDelegates.System_Threading_SemaphoreSlim_Release__182(hIDKeyboard._semaphore);
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
	private struct <InitalizeWelcomeKeyboardEffect>d__98 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public HIDKeyboard <>4__this;

		public bool IsAppExists;

		public RGBKB_PowerStatus power;

		private TaskAwaiter <>u__1;

		private string <project>5__2;

		private TaskAwaiter<SAVE_LIGHTING_EFFECT_DATA> <>u__2;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			int num = <>1__state;
			HIDKeyboard hIDKeyboard = <>4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_009a;
					}
					awaiter = ResolvedDelegates.System_Threading_Tasks_Task_GetAwaiter__23(ResolvedDelegates.System_Threading_SemaphoreSlim_WaitAsync__181(hIDKeyboard._semaphore));
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
				goto IL_009a;
				IL_009a:
				try
				{
					TaskAwaiter<SAVE_LIGHTING_EFFECT_DATA> awaiter2;
					if (num != 1)
					{
						byte projectID = hIDKeyboard._KeyboardControl.GetProjectID();
						<project>5__2 = ResolvedDelegates.System_Byte_ToString__119(ref projectID);
						CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
						LogCtrl.Write(ResolvedDelegates.System_String_Format__7("Thread {0} entering InitalizeWelcomeKeyboardEffect.", ResolvedDelegates.System_Threading_Thread_get_ManagedThreadId__118(ResolvedDelegates.System_Threading_Thread_get_CurrentThread__117())));
						awaiter2 = SettingsStorageExtensions.ReadAsync<SAVE_LIGHTING_EFFECT_DATA>(hIDKeyboard._SaveEffectType, ResolvedDelegates.System_String_Concat__29(<project>5__2, "_LastWelcomeEffect"), ResolvedDelegates.System_Threading_CancellationTokenSource_get_Token__167(cancellationTokenSource)).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (<>1__state = 1);
							<>u__2 = awaiter2;
							<>t__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = <>u__2;
						<>u__2 = default;
						num = (<>1__state = -1);
					}
					SAVE_LIGHTING_EFFECT_DATA result = awaiter2.GetResult();
					LogCtrl.Write(ResolvedDelegates.System_String_Format__7("Thread {0} entering InitalizeWelcomeKeyboardEffect.", ResolvedDelegates.System_Threading_Thread_get_ManagedThreadId__118(ResolvedDelegates.System_Threading_Thread_get_CurrentThread__117())));
					LogCtrl.Write(ResolvedDelegates.System_String_Concat__79(new string[7]
					{
						"******** current effect ",
						ResolvedDelegates.System_Byte_ToString__119(ref result.save_effect),
						"********  _SaveEffectType ",
						hIDKeyboard._SaveEffectType,
						"********  project ",
						<project>5__2,
						"******** _SaveLastWelcomeEffect LastWelcomeEffect"
					}));
					if (result.save_effect != 0)
					{
						LogCtrl.Write(ResolvedDelegates.System_String_Format__84("HIDKeyBoard InitalizeWelcomeKeyboardEffect | effect.save_effect={0} light ={1}", result.save_effect, result.save_light));
						hIDKeyboard._WelcomeEffectData = result;
						LogCtrl.Write("Set_Welcome_TimeOut_Effect_Enable");
						if (IsAppExists)
						{
							if (power.Equals(RGBKB_PowerStatus.On))
							{
								hIDKeyboard._KeyboardControl?.Set_Welcome_TimeOut_Effect_Enable(Enable: true, hIDKeyboard._WelcomeEffectData.save_effect, 4, 0);
							}
							else
							{
								hIDKeyboard._KeyboardControl?.Set_Welcome_TimeOut_Effect_Enable(Enable: false, hIDKeyboard._WelcomeEffectData.save_effect, 0, 0);
							}
						}
						else if (power.Equals(RGBKB_PowerStatus.On))
						{
							hIDKeyboard._KeyboardControl?.Set_Welcome_TimeOut_Effect_Enable(Enable: true, hIDKeyboard._WelcomeEffectData.save_effect, 4, 0);
						}
						else
						{
							hIDKeyboard._KeyboardControl?.Set_Welcome_TimeOut_Effect_Enable(Enable: false, hIDKeyboard._WelcomeEffectData.save_effect, 0, 0);
						}
						byte control = hIDKeyboard.GetControl(hIDKeyboard._SaveEffectType);
						if (result.save_effect != 0)
						{
							hIDKeyboard._KeyboardControl?.Set_Lighting_Effect(control, hIDKeyboard._WelcomeEffectData.save_effect, hIDKeyboard._WelcomeEffectData.save_light, hIDKeyboard._WelcomeEffectData.save_speed, hIDKeyboard._WelcomeEffectData.save_direction, 1, hIDKeyboard._WelcomeEffectData.save_layout_color);
						}
					}
					hIDKeyboard._InitIsDone = true;
					<project>5__2 = null;
				}
				catch (Exception ex)
				{
					LogCtrl.TraceMessage("exception: " + ex, "InitalizeWelcomeKeyboardEffect", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 748);
				}
				finally
				{
					if (num < 0)
					{
						ResolvedDelegates.System_Threading_SemaphoreSlim_Release__182(hIDKeyboard._semaphore);
					}
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

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	internal const byte EFFECT_STATIC = 1;

	internal const byte EFFECT_BREATHING = 2;

	internal const byte EFFECT_REACTIVE = 4;

	internal const byte EFFECT_WAVE = 3;

	internal const byte EFFECT_RAINBOW = 5;

	internal const byte EFFECT_RIPPLE = 6;

	internal const byte EFFECT_NOMO = 8;

	internal const byte EFFECT_MARQUEE = 9;

	internal const byte EFFECT_RAINDROP = 10;

	internal const byte EFFECT_STACK = 12;

	internal const byte EFFECT_IMPACT = 13;

	internal const byte EFFECT_AURORA = 14;

	internal const byte EFFECT_NEON = 15;

	internal const byte EFFECT_SPARK = 17;

	internal const byte EFFECT_FLASH = 18;

	internal const byte EFFECT_MIX = 19;

	internal const byte EFFECT_GAMING = 21;

	internal const byte EFFECT_RIPPLEO = 22;

	internal const byte EFFECT_ALPHABET = 23;

	internal const byte EFFECT_THINKING = 33;

	internal const byte EFFECT_MUSIC = 34;

	internal const byte EFFECT_USERMODE = 51;

	internal const byte EFFECT_UNKNOWN = byte.MaxValue;

	internal const byte EFFECT_STARTSPARK = 24;

	internal const byte EFFECT_STARTCOLLISION = 25;

	internal const byte EFFECT_MANUAL = 64;

	internal const byte EFFECT_BATTERYPERCENT = 35;

	internal const byte EFFECT_COLORFULWAVE = 52;

	internal const byte EFFECT_DAWN = 7;

	internal const byte EFFECT_SINE = 163;

	internal const byte EFFECT_INTERLACE = 164;

	internal const byte EFFECT_DIAGONAL = 165;

	internal object _powerLock = new object();

	internal object _brightnessLock = new object();

	private string _FWVersion;

	internal RGBKB_Mode _Mode;

	internal string _SaveDCLight = "DCLight";

	internal string _SaveACLight = "ACLight";

	internal string _SaveEffectType = RGBKB_Type.Normal.ToString();

	internal string _SavePowerSwich = "PowerSwitch";

	internal const string _SaveLastEffect = "LastEffect";

	internal const string _SaveLastWelcomeEffect = "LastWelcomeEffect";

	internal LM_ITE_RGB _KeyboardControl;

	internal LM_EC_RGB _EcKeyboardControl;

	internal RGBKB_PowerStatus _PowerStatus;

	internal bool _NightModSwitch;

	internal RGB_S _LastNightModeColor = new RGB_S(0u, 0, 0, 0);

	internal byte _LastEffectRun;

	internal SAVE_LIGHTING_EFFECT_DATA _EffectData;

	internal SAVE_LIGHTING_EFFECT_DATA _WelcomeEffectData;

	internal RGBKB_PowerMode CurrentPowerMode;

	internal uint _DC_LightLevel;

	internal uint _AC_LightLevel = 4u;

	[CompilerGenerated]
	private RGBKB_Event_Handler m_m_Layout_Event_handler;

	private bool _firstApStart = true;

	private bool _AllPowerAnimation;

	internal bool _CloseAnimation = true;

	private bool _OpenAnimation;

	internal const ushort EC_RGBKBBKL_LEVEL_UPDATE = 240;

	internal const ushort EC_RGBKBBKL_LEVEL_DOWN = 177;

	internal const ushort EC_RGBKBBKL_LEVEL_UP = 178;

	internal const ushort EC_RGBKBBKL_CIRCLE_UP = 185;

	internal const ushort EC_ChinaMode_Change = 182;

	internal const ushort EC_SINGLEKBL_LEVEL_CHG = 180;

	public int mbLogo;

	public int customizeTarget = 1;

	private KeyboardCloseTimer keyboardcloseTimer = KeyboardCloseTimer.CreateInstance;

	private uint beforeBrightness;

	private byte controlByte = 4;

	private List<RGBKB_Type> HidLightbarBightnessList = new List<RGBKB_Type>
	{
		RGBKB_Type.MEZone_Lighbar3,
		RGBKB_Type.MEZone_Lighbar4
	};

	internal List<byte> _APEffectList = new List<byte> { 21, 12, 13, 15, 23 };

	internal List<byte> _UserModeEffectList = new List<byte> { 12, 13, 15, 23, 34 };

	internal bool _InitIsDone;

	private object lockEffect = new object();

	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

	private object _nightModeLock = new object();

	internal bool _BeforeNightIsZero;

	private bool disposed;

	public event RGBKB_Event_Handler m_Layout_Event_handler
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		[CompilerGenerated]
		add
		{
			RGBKB_Event_Handler rGBKB_Event_Handler = this.m_m_Layout_Event_handler;
			RGBKB_Event_Handler rGBKB_Event_Handler2;
			do
			{
				rGBKB_Event_Handler2 = rGBKB_Event_Handler;
				RGBKB_Event_Handler value2 = (RGBKB_Event_Handler)ResolvedDelegates.System_Delegate_Combine__130(rGBKB_Event_Handler2, value);
				rGBKB_Event_Handler = Interlocked.CompareExchange(ref this.m_m_Layout_Event_handler, value2, rGBKB_Event_Handler2);
			}
			while ((object)rGBKB_Event_Handler != rGBKB_Event_Handler2);
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		[CompilerGenerated]
		remove
		{
			RGBKB_Event_Handler rGBKB_Event_Handler = this.m_m_Layout_Event_handler;
			RGBKB_Event_Handler rGBKB_Event_Handler2;
			do
			{
				rGBKB_Event_Handler2 = rGBKB_Event_Handler;
				RGBKB_Event_Handler value2 = (RGBKB_Event_Handler)ResolvedDelegates.System_Delegate_Remove__131(rGBKB_Event_Handler2, value);
				rGBKB_Event_Handler = Interlocked.CompareExchange(ref this.m_m_Layout_Event_handler, value2, rGBKB_Event_Handler2);
			}
			while ((object)rGBKB_Event_Handler != rGBKB_Event_Handler2);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public HIDKeyboard(LM_Manager lm_Manager, string fWVersion)
	{
		ResolvedDelegates.System_Object_ctor__0(this);
		_FWVersion = fWVersion;
		if (lm_Manager.m_KB_Solution == RGBKB_Solution.ITE)
		{
			_KeyboardControl = (LM_ITE_RGB)lm_Manager.GetITE_RGB();
		}
		else if (lm_Manager.m_KB_Solution == RGBKB_Solution.EC)
		{
			_EcKeyboardControl = (LM_EC_RGB)lm_Manager.GetEC_RGB();
		}
		SettingsStorageExtensions.CheckingDir();
		keyboardcloseTimer.ControlBrigtnessEvent -= KeyboardcloseTimer_ControlBrigtnessEvent;
		keyboardcloseTimer.ControlBrigtnessEvent += KeyboardcloseTimer_ControlBrigtnessEvent;
		customizeTarget = RegistryCtrl.GetCustomizeTarget();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal RGBKB_Color ConvertJsonRGB2RGBColor(dynamic color)
	{
		if (<>o__73.<>p__2 == null)
		{
			<>o__73.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(bool).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, bool> target = <>o__73.<>p__2.Target;
		CallSite<Func<CallSite, object, bool>> arg = <>o__73.<>p__2;
		if (<>o__73.<>p__1 == null)
		{
			<>o__73.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToBoolean", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target2 = <>o__73.<>p__1.Target;
		CallSite<Func<CallSite, Type, object, object>> arg2 = <>o__73.<>p__1;
		Type arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__73.<>p__0 == null)
		{
			<>o__73.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		bool bCircular = target(arg, target2(arg2, arg3, <>o__73.<>p__0.Target(<>o__73.<>p__0, (object)color, "isCircular")));
		if (<>o__73.<>p__5 == null)
		{
			<>o__73.<>p__5 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, uint> target3 = <>o__73.<>p__5.Target;
		CallSite<Func<CallSite, object, uint>> arg4 = <>o__73.<>p__5;
		if (<>o__73.<>p__4 == null)
		{
			<>o__73.<>p__4 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target4 = <>o__73.<>p__4.Target;
		CallSite<Func<CallSite, Type, object, object>> arg5 = <>o__73.<>p__4;
		Type arg6 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__73.<>p__3 == null)
		{
			<>o__73.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		uint num = target3(arg4, target4(arg5, arg6, <>o__73.<>p__3.Target(<>o__73.<>p__3, (object)color, "ColorBlocks")));
		RGBKB_Color result = new RGBKB_Color(bCircular, num);
		for (int i = 0; i < num; i++)
		{
			ref RGB_S reference = ref result.ColorBuffer[i];
			if (<>o__73.<>p__10 == null)
			{
				<>o__73.<>p__10 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
			}
			Func<CallSite, object, uint> target5 = <>o__73.<>p__10.Target;
			CallSite<Func<CallSite, object, uint>> arg7 = <>o__73.<>p__10;
			if (<>o__73.<>p__9 == null)
			{
				<>o__73.<>p__9 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target6 = <>o__73.<>p__9.Target;
			CallSite<Func<CallSite, Type, object, object>> arg8 = <>o__73.<>p__9;
			Type arg9 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__73.<>p__8 == null)
			{
				<>o__73.<>p__8 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, string, object> target7 = <>o__73.<>p__8.Target;
			CallSite<Func<CallSite, object, string, object>> arg10 = <>o__73.<>p__8;
			if (<>o__73.<>p__7 == null)
			{
				<>o__73.<>p__7 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			Func<CallSite, object, int, object> target8 = <>o__73.<>p__7.Target;
			CallSite<Func<CallSite, object, int, object>> arg11 = <>o__73.<>p__7;
			if (<>o__73.<>p__6 == null)
			{
				<>o__73.<>p__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference.ID = target5(arg7, target6(arg8, arg9, target7(arg10, target8(arg11, <>o__73.<>p__6.Target(<>o__73.<>p__6, (object)color, "ColorBuffer"), i), "ID")));
			ref RGB_S reference2 = ref result.ColorBuffer[i];
			if (<>o__73.<>p__15 == null)
			{
				<>o__73.<>p__15 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
			}
			Func<CallSite, object, byte> target9 = <>o__73.<>p__15.Target;
			CallSite<Func<CallSite, object, byte>> arg12 = <>o__73.<>p__15;
			if (<>o__73.<>p__14 == null)
			{
				<>o__73.<>p__14 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target10 = <>o__73.<>p__14.Target;
			CallSite<Func<CallSite, Type, object, object>> arg13 = <>o__73.<>p__14;
			Type arg14 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__73.<>p__13 == null)
			{
				<>o__73.<>p__13 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, string, object> target11 = <>o__73.<>p__13.Target;
			CallSite<Func<CallSite, object, string, object>> arg15 = <>o__73.<>p__13;
			if (<>o__73.<>p__12 == null)
			{
				<>o__73.<>p__12 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			Func<CallSite, object, int, object> target12 = <>o__73.<>p__12.Target;
			CallSite<Func<CallSite, object, int, object>> arg16 = <>o__73.<>p__12;
			if (<>o__73.<>p__11 == null)
			{
				<>o__73.<>p__11 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference2.R = target9(arg12, target10(arg13, arg14, target11(arg15, target12(arg16, <>o__73.<>p__11.Target(<>o__73.<>p__11, (object)color, "ColorBuffer"), i), "R")));
			ref RGB_S reference3 = ref result.ColorBuffer[i];
			if (<>o__73.<>p__20 == null)
			{
				<>o__73.<>p__20 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
			}
			Func<CallSite, object, byte> target13 = <>o__73.<>p__20.Target;
			CallSite<Func<CallSite, object, byte>> arg17 = <>o__73.<>p__20;
			if (<>o__73.<>p__19 == null)
			{
				<>o__73.<>p__19 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target14 = <>o__73.<>p__19.Target;
			CallSite<Func<CallSite, Type, object, object>> arg18 = <>o__73.<>p__19;
			Type arg19 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__73.<>p__18 == null)
			{
				<>o__73.<>p__18 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, string, object> target15 = <>o__73.<>p__18.Target;
			CallSite<Func<CallSite, object, string, object>> arg20 = <>o__73.<>p__18;
			if (<>o__73.<>p__17 == null)
			{
				<>o__73.<>p__17 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			Func<CallSite, object, int, object> target16 = <>o__73.<>p__17.Target;
			CallSite<Func<CallSite, object, int, object>> arg21 = <>o__73.<>p__17;
			if (<>o__73.<>p__16 == null)
			{
				<>o__73.<>p__16 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference3.G = target13(arg17, target14(arg18, arg19, target15(arg20, target16(arg21, <>o__73.<>p__16.Target(<>o__73.<>p__16, (object)color, "ColorBuffer"), i), "G")));
			ref RGB_S reference4 = ref result.ColorBuffer[i];
			if (<>o__73.<>p__25 == null)
			{
				<>o__73.<>p__25 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
			}
			Func<CallSite, object, byte> target17 = <>o__73.<>p__25.Target;
			CallSite<Func<CallSite, object, byte>> arg22 = <>o__73.<>p__25;
			if (<>o__73.<>p__24 == null)
			{
				<>o__73.<>p__24 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target18 = <>o__73.<>p__24.Target;
			CallSite<Func<CallSite, Type, object, object>> arg23 = <>o__73.<>p__24;
			Type arg24 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__73.<>p__23 == null)
			{
				<>o__73.<>p__23 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, string, object> target19 = <>o__73.<>p__23.Target;
			CallSite<Func<CallSite, object, string, object>> arg25 = <>o__73.<>p__23;
			if (<>o__73.<>p__22 == null)
			{
				<>o__73.<>p__22 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			Func<CallSite, object, int, object> target20 = <>o__73.<>p__22.Target;
			CallSite<Func<CallSite, object, int, object>> arg26 = <>o__73.<>p__22;
			if (<>o__73.<>p__21 == null)
			{
				<>o__73.<>p__21 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference4.B = target17(arg22, target18(arg23, arg24, target19(arg25, target20(arg26, <>o__73.<>p__21.Target(<>o__73.<>p__21, (object)color, "ColorBuffer"), i), "B")));
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual byte Translate_ITE_EffectIndex(RGBKB_Effect layoutEffect)
	{
		return layoutEffect switch
		{
			RGBKB_Effect.Twinkling => (byte)10, 
			RGBKB_Effect.ColorMarquee => (byte)9, 
			RGBKB_Effect.Single => (byte)1, 
			RGBKB_Effect.Breathing => (byte)2, 
			RGBKB_Effect.Wave => (byte)3, 
			RGBKB_Effect.Reactive => (byte)4, 
			RGBKB_Effect.Rainbow => (byte)5, 
			RGBKB_Effect.Ripple => (byte)6, 
			RGBKB_Effect.RippleO => (byte)22, 
			RGBKB_Effect.Marquee => (byte)9, 
			RGBKB_Effect.Raindrop => (byte)10, 
			RGBKB_Effect.Spark => (byte)17, 
			RGBKB_Effect.Aurora => (byte)14, 
			RGBKB_Effect.UserMode => (byte)51, 
			RGBKB_Effect.Music => (byte)34, 
			RGBKB_Effect.Flash => (byte)18, 
			RGBKB_Effect.Mix => (byte)19, 
			RGBKB_Effect.Neon => (byte)15, 
			RGBKB_Effect.Stack => (byte)12, 
			RGBKB_Effect.Impact => (byte)13, 
			RGBKB_Effect.Gaming => (byte)21, 
			RGBKB_Effect.Alphabet => (byte)23, 
			RGBKB_Effect.StarHitting => (byte)25, 
			RGBKB_Effect.StarSpark => (byte)24, 
			RGBKB_Effect.Thinking => (byte)33, 
			RGBKB_Effect.Manual => (byte)64, 
			RGBKB_Effect.BatteryPercent => (byte)35, 
			RGBKB_Effect.ColorfulWave => (byte)52, 
			RGBKB_Effect.Dawn => (byte)7, 
			RGBKB_Effect.Sine => (byte)163, 
			RGBKB_Effect.Interlace => (byte)164, 
			RGBKB_Effect.Diagonal => (byte)165, 
			_ => byte.MaxValue, 
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual byte Translate_ITE_LightValue(uint layoutLight)
	{
		if (HidLightbarBightnessList.Contains((_KeyboardControl?.ILM_RGBKB_GetRGBKeyboardType()).Value))
		{
			return layoutLight switch
			{
				0u => (byte)0, 
				1u => (byte)25, 
				2u => (byte)50, 
				3u => (byte)75, 
				4u => (byte)100, 
				_ => (byte)0, 
			};
		}
		return layoutLight switch
		{
			0u => (byte)0, 
			1u => (byte)8, 
			2u => (byte)22, 
			3u => (byte)36, 
			4u => (byte)50, 
			_ => (byte)0, 
		};
	}

	internal virtual byte Translate_ITE_SpeedValue(uint layoutSpeed)
	{
		return layoutSpeed switch
		{
			0u => (byte)10, 
			1u => (byte)7, 
			2u => (byte)5, 
			3u => (byte)3, 
			4u => (byte)1, 
			_ => (byte)1, 
		};
	}

	internal byte Translate_ITE_DirectionValue(RGBKB_Direction layoutDirection)
	{
		return layoutDirection switch
		{
			RGBKB_Direction.None => (byte)0, 
			RGBKB_Direction.LeftRight => (byte)1, 
			RGBKB_Direction.RightLeft => (byte)2, 
			RGBKB_Direction.DownUp => (byte)3, 
			RGBKB_Direction.UpDown => (byte)4, 
			RGBKB_Direction.OnKeyPressed => (byte)1, 
			RGBKB_Direction.Sync => (byte)3, 
			_ => (byte)1, 
		};
	}

	internal virtual uint Translate_Layout_LightValue(byte ite_light)
	{
		return ite_light switch
		{
			0 => 0u, 
			8 => 1u, 
			22 => 2u, 
			36 => 3u, 
			50 => 4u, 
			_ => 0u, 
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal RGB_S BackgroundColorConverter(int colorindex)
	{
		RGB_S result = new RGB_S
		{
			ID = 0u,
			R = 0,
			G = 0,
			B = 0
		};
		switch (colorindex)
		{
		case 1:
			result.R = 3;
			break;
		case 2:
			result.R = 9;
			result.G = 1;
			break;
		case 3:
			result.R = 9;
			result.G = 3;
			break;
		case 4:
			result.G = 1;
			break;
		case 5:
			result.G = 1;
			result.B = 1;
			break;
		case 6:
			result.B = 1;
			break;
		case 7:
			result.R = 3;
			result.B = 2;
			break;
		case 8:
			result.R = 1;
			result.G = 1;
			result.B = 1;
			break;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal byte GetEffectType(byte save_effect)
	{
		byte b = 0;
		byte b2 = 1;
		byte b3 = 3;
		byte result = 4;
		if (_APEffectList.Contains(save_effect))
		{
			return result;
		}
		return save_effect switch
		{
			51 => b2, 
			34 => b3, 
			_ => b, 
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool GetInitSuccessful()
	{
		return _InitIsDone;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetACDCLightString(string PowerSwitchString, string ACLightString, string DCLightString, RGBKB_Type forceEffectSaveName = RGBKB_Type.Normal)
	{
		_SavePowerSwich = PowerSwitchString;
		_SaveACLight = ACLightString;
		_SaveDCLight = DCLightString;
		if (forceEffectSaveName == RGBKB_Type.Normal)
		{
			_SaveEffectType = _KeyboardControl?.ILM_RGBKB_GetRGBKeyboardType().ToString();
		}
		else
		{
			_SaveEffectType = forceEffectSaveName.ToString();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool GetLogoSupport()
	{
		return _KeyboardControl.GetLogoSupport();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool GetHingeSupport()
	{
		return _KeyboardControl.GetHingeSupport();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool GetBaseSupport()
	{
		return _KeyboardControl.GetBaseSupport();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool GetNewLogoSupport()
	{
		return _KeyboardControl.GetNewLogoSupport();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal bool GetMBLogoSupport()
	{
		return _KeyboardControl.GetMBLogoSupport();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<InitalizeKeyboardEffect>d__92))]
	public virtual void InitalizeKeyboardEffect(bool unplug = false, bool runeffect = true)
	{
		<InitalizeKeyboardEffect>d__92 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine.unplug = unplug;
		stateMachine.runeffect = runeffect;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void SetACDCLight(bool unplug, uint aclevel, uint dclevel)
	{
		_DC_LightLevel = dclevel;
		_AC_LightLevel = aclevel;
		if (unplug)
		{
			CurrentPowerMode = RGBKB_PowerMode.DC;
			SetBrightnessStrategy(Translate_ITE_LightValue(dclevel));
			_EffectData.save_light = Translate_ITE_LightValue(dclevel);
			SaveEffectData();
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("In DC Mode Backlight Now bcaklight : ", ResolvedDelegates.System_Byte_ToString__119(ref _EffectData.save_light)), "SetACDCLight", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 567);
		}
		else
		{
			CurrentPowerMode = RGBKB_PowerMode.AC;
			SetBrightnessStrategy(Translate_ITE_LightValue(aclevel));
			_EffectData.save_light = Translate_ITE_LightValue(aclevel);
			SaveEffectData();
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("In AC Mode Backlight Now bcaklight : ", ResolvedDelegates.System_Byte_ToString__119(ref _EffectData.save_light)), "SetACDCLight", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 577);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void _88Htest()
	{
		byte Control = 0;
		byte Effect = 0;
		byte Speed = 0;
		byte ColorIndex = 0;
		byte Direction = 0;
		byte Light = 0;
		_KeyboardControl?.HID_Get_Effect_Type_88H(ref Control, ref Effect, ref Speed, ref Light, ref ColorIndex, ref Direction);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void AsyncWelcome()
	{
		try
		{
			List<string[]> keyboardWelcomeEffect = NvramVariable.GetKeyboardWelcomeEffect();
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("data[0]: ", ResolvedDelegates.System_String_Join__295(",", keyboardWelcomeEffect[0])), "AsyncWelcome", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 597);
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("data[1]: ", ResolvedDelegates.System_String_Join__295(",", keyboardWelcomeEffect[1])), "AsyncWelcome", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 598);
			SAVE_LIGHTING_EFFECT_DATA welcomeEffectData = default;
			RGBKB_PowerStatus rGBKB_PowerStatus = RGBKB_PowerStatus.On;
			if (ResolvedDelegates.System_Convert_ToByte__343(keyboardWelcomeEffect[0][0]) != byte.MaxValue)
			{
				welcomeEffectData.save_effect = ResolvedDelegates.System_Convert_ToByte__716(ResolvedDelegates.System_Convert_ToUInt32__500(keyboardWelcomeEffect[0][2], 16));
				welcomeEffectData.save_speed = ResolvedDelegates.System_Convert_ToByte__716(ResolvedDelegates.System_Convert_ToUInt32__500(keyboardWelcomeEffect[0][3], 16));
				welcomeEffectData.save_light = ResolvedDelegates.System_Convert_ToByte__716(ResolvedDelegates.System_Convert_ToUInt32__500(keyboardWelcomeEffect[0][4], 16));
				welcomeEffectData.save_direction = ResolvedDelegates.System_Convert_ToByte__716(ResolvedDelegates.System_Convert_ToUInt32__500(keyboardWelcomeEffect[0][5], 16));
				rGBKB_PowerStatus = (ResolvedDelegates.System_Convert_ToBoolean__747(ResolvedDelegates.System_Convert_ToUInt32__500(keyboardWelcomeEffect[1][2], 16)) ? RGBKB_PowerStatus.On : RGBKB_PowerStatus.Off);
				_WelcomeEffectData = welcomeEffectData;
				SetPowerStatus(rGBKB_PowerStatus, 0, "");
			}
		}
		catch (Exception)
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<InitalizeWelcomeKeyboardEffect>d__98))]
	public virtual Task InitalizeWelcomeKeyboardEffect(RGBKB_PowerStatus power, bool IsAppExists, bool unplug = false)
	{
		<InitalizeWelcomeKeyboardEffect>d__98 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_Create__20();
		stateMachine.<>4__this = this;
		stateMachine.power = power;
		stateMachine.IsAppExists = IsAppExists;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
		return ResolvedDelegates.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_get_Task__21(ref stateMachine.<>t__builder);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte GetControl(string SaveEffectType)
	{
		byte b = 2;
		if (!ResolvedDelegates.System_String_op_Equality__18(SaveEffectType, "MEZone_Lighbar4"))
		{
			if (ResolvedDelegates.System_String_op_Equality__18(SaveEffectType, "MEZone_Lighbar4_hinge"))
			{
				return 33;
			}
			return 2;
		}
		return 34;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void SaveEffectData()
	{
		if (_EffectData.save_mode != RGBKB_Mode.Welcome && _EffectData.save_effect != 0)
		{
			LM_ITE_RGB keyboardControl = _KeyboardControl;
			object obj;
			if (keyboardControl == null)
			{
				obj = null;
			}
			else
			{
				byte projectID = keyboardControl.GetProjectID();
				obj = ResolvedDelegates.System_Byte_ToString__119(ref projectID);
			}
			string text = (string)obj;
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("@SaveAsync light : ", ResolvedDelegates.System_Byte_ToString__119(ref _EffectData.save_light)), "SaveEffectData", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 784);
			SettingsStorageExtensions.SaveAsync(_SaveEffectType, ResolvedDelegates.System_String_Concat__29(text, "_LastEffect"), _EffectData);
			if (CurrentPowerMode == RGBKB_PowerMode.AC)
			{
				SettingsStorageExtensions.SaveAsync("", _SaveACLight, _AC_LightLevel);
			}
			else if (CurrentPowerMode == RGBKB_PowerMode.DC)
			{
				SettingsStorageExtensions.SaveAsync("", _SaveDCLight, _DC_LightLevel);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void SaveWelcomeEffectData()
	{
		LM_ITE_RGB keyboardControl = _KeyboardControl;
		object obj;
		if (keyboardControl == null)
		{
			obj = null;
		}
		else
		{
			byte projectID = keyboardControl.GetProjectID();
			obj = ResolvedDelegates.System_Byte_ToString__119(ref projectID);
		}
		string text = (string)obj;
		SettingsStorageExtensions.SaveAsync(_KeyboardControl?.ILM_RGBKB_GetRGBKeyboardType().ToString(), text + "_LastWelcomeEffect", _WelcomeEffectData);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void SavePowerStatus()
	{
		SettingsStorageExtensions.SaveAsync("", _SavePowerSwich, _PowerStatus);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetMode(RGBKB_Mode mode)
	{
		_Mode = mode;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public RGBKB_Mode GetMode()
	{
		return _Mode;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual bool SetEffect(dynamic data)
	{
		if (<>o__105.<>p__2 == null)
		{
			<>o__105.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, string> target = <>o__105.<>p__2.Target;
		CallSite<Func<CallSite, object, string>> arg = <>o__105.<>p__2;
		if (<>o__105.<>p__0 == null)
		{
			<>o__105.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		object obj = <>o__105.<>p__0.Target(<>o__105.<>p__0, (object)data, "mode");
		object arg2;
		if (obj == null)
		{
			arg2 = null;
		}
		else
		{
			if (<>o__105.<>p__1 == null)
			{
				<>o__105.<>p__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg2 = <>o__105.<>p__1.Target(<>o__105.<>p__1, obj);
		}
		string text = target(arg, arg2);
		RGBKB_Mode save_mode = (RGBKB_Mode)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Mode).TypeHandle), text);
		if (<>o__105.<>p__5 == null)
		{
			<>o__105.<>p__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, string> target2 = <>o__105.<>p__5.Target;
		CallSite<Func<CallSite, object, string>> arg3 = <>o__105.<>p__5;
		if (<>o__105.<>p__3 == null)
		{
			<>o__105.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		obj = <>o__105.<>p__3.Target(<>o__105.<>p__3, (object)data, "effect");
		object arg4;
		if (obj == null)
		{
			arg4 = null;
		}
		else
		{
			if (<>o__105.<>p__4 == null)
			{
				<>o__105.<>p__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg4 = <>o__105.<>p__4.Target(<>o__105.<>p__4, obj);
		}
		string text2 = target2(arg3, arg4);
		RGBKB_Effect layoutEffect = (RGBKB_Effect)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Effect).TypeHandle), text2);
		if (<>o__105.<>p__8 == null)
		{
			<>o__105.<>p__8 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, uint> target3 = <>o__105.<>p__8.Target;
		CallSite<Func<CallSite, object, uint>> arg5 = <>o__105.<>p__8;
		if (<>o__105.<>p__7 == null)
		{
			<>o__105.<>p__7 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target4 = <>o__105.<>p__7.Target;
		CallSite<Func<CallSite, Type, object, object>> arg6 = <>o__105.<>p__7;
		Type arg7 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__105.<>p__6 == null)
		{
			<>o__105.<>p__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		uint layoutLight = target3(arg5, target4(arg6, arg7, <>o__105.<>p__6.Target(<>o__105.<>p__6, (object)data, "light")));
		if (<>o__105.<>p__11 == null)
		{
			<>o__105.<>p__11 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, uint> target5 = <>o__105.<>p__11.Target;
		CallSite<Func<CallSite, object, uint>> arg8 = <>o__105.<>p__11;
		if (<>o__105.<>p__10 == null)
		{
			<>o__105.<>p__10 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target6 = <>o__105.<>p__10.Target;
		CallSite<Func<CallSite, Type, object, object>> arg9 = <>o__105.<>p__10;
		Type arg10 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__105.<>p__9 == null)
		{
			<>o__105.<>p__9 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		uint layoutSpeed = target5(arg8, target6(arg9, arg10, <>o__105.<>p__9.Target(<>o__105.<>p__9, (object)data, "speed")));
		RGBKB_Direction layoutDirection = RGBKB_Direction.None;
		RGBKB_NV_SAVE rGBKB_NV_SAVE = RGBKB_NV_SAVE.NOT_SAVE;
		if (<>o__105.<>p__14 == null)
		{
			<>o__105.<>p__14 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target7 = <>o__105.<>p__14.Target;
		CallSite<Func<CallSite, object, bool>> arg11 = <>o__105.<>p__14;
		if (<>o__105.<>p__13 == null)
		{
			<>o__105.<>p__13 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		Func<CallSite, object, object, object> target8 = <>o__105.<>p__13.Target;
		CallSite<Func<CallSite, object, object, object>> arg12 = <>o__105.<>p__13;
		if (<>o__105.<>p__12 == null)
		{
			<>o__105.<>p__12 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		if (target7(arg11, target8(arg12, <>o__105.<>p__12.Target(<>o__105.<>p__12, (object)data, "direction"), null)))
		{
			if (<>o__105.<>p__17 == null)
			{
				<>o__105.<>p__17 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
			}
			Func<CallSite, object, string> target9 = <>o__105.<>p__17.Target;
			CallSite<Func<CallSite, object, string>> arg13 = <>o__105.<>p__17;
			if (<>o__105.<>p__15 == null)
			{
				<>o__105.<>p__15 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			obj = <>o__105.<>p__15.Target(<>o__105.<>p__15, (object)data, "direction");
			object arg14;
			if (obj == null)
			{
				arg14 = null;
			}
			else
			{
				if (<>o__105.<>p__16 == null)
				{
					<>o__105.<>p__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				arg14 = <>o__105.<>p__16.Target(<>o__105.<>p__16, obj);
			}
			string text3 = target9(arg13, arg14);
			layoutDirection = (RGBKB_Direction)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Direction).TypeHandle), text3);
		}
		if (<>o__105.<>p__20 == null)
		{
			<>o__105.<>p__20 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target10 = <>o__105.<>p__20.Target;
		CallSite<Func<CallSite, object, bool>> arg15 = <>o__105.<>p__20;
		if (<>o__105.<>p__19 == null)
		{
			<>o__105.<>p__19 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		Func<CallSite, object, object, object> target11 = <>o__105.<>p__19.Target;
		CallSite<Func<CallSite, object, object, object>> arg16 = <>o__105.<>p__19;
		if (<>o__105.<>p__18 == null)
		{
			<>o__105.<>p__18 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		if (target10(arg15, target11(arg16, <>o__105.<>p__18.Target(<>o__105.<>p__18, (object)data, "nv_save"), null)))
		{
			if (<>o__105.<>p__23 == null)
			{
				<>o__105.<>p__23 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
			}
			Func<CallSite, object, string> target12 = <>o__105.<>p__23.Target;
			CallSite<Func<CallSite, object, string>> arg17 = <>o__105.<>p__23;
			if (<>o__105.<>p__21 == null)
			{
				<>o__105.<>p__21 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			obj = <>o__105.<>p__21.Target(<>o__105.<>p__21, (object)data, "nv_save");
			object arg18;
			if (obj == null)
			{
				arg18 = null;
			}
			else
			{
				if (<>o__105.<>p__22 == null)
				{
					<>o__105.<>p__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				arg18 = <>o__105.<>p__22.Target(<>o__105.<>p__22, obj);
			}
			string text4 = target12(arg17, arg18);
			rGBKB_NV_SAVE = (RGBKB_NV_SAVE)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_NV_SAVE).TypeHandle), text4);
		}
		if (<>o__105.<>p__26 == null)
		{
			<>o__105.<>p__26 = CallSite<Func<CallSite, object, RGBKB_Color>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Color).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, RGBKB_Color> target13 = <>o__105.<>p__26.Target;
		CallSite<Func<CallSite, object, RGBKB_Color>> arg19 = <>o__105.<>p__26;
		if (<>o__105.<>p__25 == null)
		{
			<>o__105.<>p__25 = CallSite<Func<CallSite, HIDKeyboard, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ConvertJsonRGB2RGBColor", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, HIDKeyboard, object, object> target14 = <>o__105.<>p__25.Target;
		CallSite<Func<CallSite, HIDKeyboard, object, object>> arg20 = <>o__105.<>p__25;
		if (<>o__105.<>p__24 == null)
		{
			<>o__105.<>p__24 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		RGBKB_Color save_layout_color = target13(arg19, target14(arg20, this, <>o__105.<>p__24.Target(<>o__105.<>p__24, (object)data, "color")));
		int save_layout_backgroundcolor = 0;
		string save_layout_alphbet = "";
		if (<>o__105.<>p__29 == null)
		{
			<>o__105.<>p__29 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target15 = <>o__105.<>p__29.Target;
		CallSite<Func<CallSite, object, bool>> arg21 = <>o__105.<>p__29;
		if (<>o__105.<>p__28 == null)
		{
			<>o__105.<>p__28 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		Func<CallSite, object, object, object> target16 = <>o__105.<>p__28.Target;
		CallSite<Func<CallSite, object, object, object>> arg22 = <>o__105.<>p__28;
		if (<>o__105.<>p__27 == null)
		{
			<>o__105.<>p__27 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		if (target15(arg21, target16(arg22, <>o__105.<>p__27.Target(<>o__105.<>p__27, (object)data, "backgroundcolor"), null)))
		{
			if (<>o__105.<>p__32 == null)
			{
				<>o__105.<>p__32 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
			}
			Func<CallSite, object, int> target17 = <>o__105.<>p__32.Target;
			CallSite<Func<CallSite, object, int>> arg23 = <>o__105.<>p__32;
			if (<>o__105.<>p__31 == null)
			{
				<>o__105.<>p__31 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target18 = <>o__105.<>p__31.Target;
			CallSite<Func<CallSite, Type, object, object>> arg24 = <>o__105.<>p__31;
			Type arg25 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__105.<>p__30 == null)
			{
				<>o__105.<>p__30 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			save_layout_backgroundcolor = target17(arg23, target18(arg24, arg25, <>o__105.<>p__30.Target(<>o__105.<>p__30, (object)data, "backgroundcolor")));
		}
		if (<>o__105.<>p__36 == null)
		{
			<>o__105.<>p__36 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target19 = <>o__105.<>p__36.Target;
		CallSite<Func<CallSite, object, bool>> arg26 = <>o__105.<>p__36;
		bool flag = layoutEffect.Equals(RGBKB_Effect.Alphabet);
		object arg30;
		if (flag)
		{
			if (<>o__105.<>p__35 == null)
			{
				<>o__105.<>p__35 = CallSite<Func<CallSite, bool, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, bool, object, object> target20 = <>o__105.<>p__35.Target;
			CallSite<Func<CallSite, bool, object, object>> arg27 = <>o__105.<>p__35;
			bool arg28 = flag;
			if (<>o__105.<>p__34 == null)
			{
				<>o__105.<>p__34 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target21 = <>o__105.<>p__34.Target;
			CallSite<Func<CallSite, object, object, object>> arg29 = <>o__105.<>p__34;
			if (<>o__105.<>p__33 == null)
			{
				<>o__105.<>p__33 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			arg30 = target20(arg27, arg28, target21(arg29, <>o__105.<>p__33.Target(<>o__105.<>p__33, (object)data, "alphabet"), null));
		}
		else
		{
			arg30 = flag;
		}
		if (target19(arg26, arg30))
		{
			if (<>o__105.<>p__39 == null)
			{
				<>o__105.<>p__39 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
			}
			Func<CallSite, object, string> target22 = <>o__105.<>p__39.Target;
			CallSite<Func<CallSite, object, string>> arg31 = <>o__105.<>p__39;
			if (<>o__105.<>p__37 == null)
			{
				<>o__105.<>p__37 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			obj = <>o__105.<>p__37.Target(<>o__105.<>p__37, (object)data, "alphabet");
			object arg32;
			if (obj == null)
			{
				arg32 = null;
			}
			else
			{
				if (<>o__105.<>p__38 == null)
				{
					<>o__105.<>p__38 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				arg32 = <>o__105.<>p__38.Target(<>o__105.<>p__38, obj);
			}
			save_layout_alphbet = target22(arg31, arg32);
		}
		SAVE_LIGHTING_EFFECT_DATA sAVE_LIGHTING_EFFECT_DATA = new SAVE_LIGHTING_EFFECT_DATA
		{
			save_mode = save_mode,
			save_effect = Translate_ITE_EffectIndex(layoutEffect),
			save_light = Translate_ITE_LightValue(layoutLight),
			save_speed = Translate_ITE_SpeedValue(layoutSpeed),
			save_direction = Translate_ITE_DirectionValue(layoutDirection),
			save_layout_color = save_layout_color,
			bSaved = ResolvedDelegates.System_Convert_ToBoolean__748((byte)rGBKB_NV_SAVE),
			save_layout_alphbet = save_layout_alphbet,
			save_layout_backgroundcolor = save_layout_backgroundcolor
		};
		if (_EffectData.Equals(sAVE_LIGHTING_EFFECT_DATA))
		{
			return false;
		}
		_EffectData = sAVE_LIGHTING_EFFECT_DATA;
		if (CurrentPowerMode == RGBKB_PowerMode.DC)
		{
			_DC_LightLevel = Translate_Layout_LightValue(_EffectData.save_light);
		}
		else
		{
			_AC_LightLevel = Translate_Layout_LightValue(_EffectData.save_light);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual bool SetWelcomeEffect(dynamic data)
	{
		if (<>o__106.<>p__2 == null)
		{
			<>o__106.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, string> target = <>o__106.<>p__2.Target;
		CallSite<Func<CallSite, object, string>> arg = <>o__106.<>p__2;
		if (<>o__106.<>p__0 == null)
		{
			<>o__106.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		object obj = <>o__106.<>p__0.Target(<>o__106.<>p__0, (object)data, "mode");
		object arg2;
		if (obj == null)
		{
			arg2 = null;
		}
		else
		{
			if (<>o__106.<>p__1 == null)
			{
				<>o__106.<>p__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg2 = <>o__106.<>p__1.Target(<>o__106.<>p__1, obj);
		}
		string text = target(arg, arg2);
		RGBKB_Mode save_mode = (RGBKB_Mode)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Mode).TypeHandle), text);
		if (<>o__106.<>p__5 == null)
		{
			<>o__106.<>p__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, string> target2 = <>o__106.<>p__5.Target;
		CallSite<Func<CallSite, object, string>> arg3 = <>o__106.<>p__5;
		if (<>o__106.<>p__3 == null)
		{
			<>o__106.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		obj = <>o__106.<>p__3.Target(<>o__106.<>p__3, (object)data, "effect");
		object arg4;
		if (obj == null)
		{
			arg4 = null;
		}
		else
		{
			if (<>o__106.<>p__4 == null)
			{
				<>o__106.<>p__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg4 = <>o__106.<>p__4.Target(<>o__106.<>p__4, obj);
		}
		string text2 = target2(arg3, arg4);
		RGBKB_Effect layoutEffect = (RGBKB_Effect)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Effect).TypeHandle), text2);
		if (<>o__106.<>p__8 == null)
		{
			<>o__106.<>p__8 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, uint> target3 = <>o__106.<>p__8.Target;
		CallSite<Func<CallSite, object, uint>> arg5 = <>o__106.<>p__8;
		if (<>o__106.<>p__7 == null)
		{
			<>o__106.<>p__7 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target4 = <>o__106.<>p__7.Target;
		CallSite<Func<CallSite, Type, object, object>> arg6 = <>o__106.<>p__7;
		Type arg7 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__106.<>p__6 == null)
		{
			<>o__106.<>p__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		uint layoutLight = target3(arg5, target4(arg6, arg7, <>o__106.<>p__6.Target(<>o__106.<>p__6, (object)data, "light")));
		if (<>o__106.<>p__11 == null)
		{
			<>o__106.<>p__11 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, uint> target5 = <>o__106.<>p__11.Target;
		CallSite<Func<CallSite, object, uint>> arg8 = <>o__106.<>p__11;
		if (<>o__106.<>p__10 == null)
		{
			<>o__106.<>p__10 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target6 = <>o__106.<>p__10.Target;
		CallSite<Func<CallSite, Type, object, object>> arg9 = <>o__106.<>p__10;
		Type arg10 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__106.<>p__9 == null)
		{
			<>o__106.<>p__9 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		uint layoutSpeed = target5(arg8, target6(arg9, arg10, <>o__106.<>p__9.Target(<>o__106.<>p__9, (object)data, "speed")));
		if (<>o__106.<>p__14 == null)
		{
			<>o__106.<>p__14 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, string> target7 = <>o__106.<>p__14.Target;
		CallSite<Func<CallSite, object, string>> arg11 = <>o__106.<>p__14;
		if (<>o__106.<>p__12 == null)
		{
			<>o__106.<>p__12 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		obj = <>o__106.<>p__12.Target(<>o__106.<>p__12, (object)data, "direction");
		object arg12;
		if (obj == null)
		{
			arg12 = null;
		}
		else
		{
			if (<>o__106.<>p__13 == null)
			{
				<>o__106.<>p__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg12 = <>o__106.<>p__13.Target(<>o__106.<>p__13, obj);
		}
		string text3 = target7(arg11, arg12);
		RGBKB_Direction layoutDirection = (RGBKB_Direction)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Direction).TypeHandle), text3);
		if (<>o__106.<>p__17 == null)
		{
			<>o__106.<>p__17 = CallSite<Func<CallSite, object, RGBKB_Color>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Color).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, RGBKB_Color> target8 = <>o__106.<>p__17.Target;
		CallSite<Func<CallSite, object, RGBKB_Color>> arg13 = <>o__106.<>p__17;
		if (<>o__106.<>p__16 == null)
		{
			<>o__106.<>p__16 = CallSite<Func<CallSite, HIDKeyboard, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ConvertJsonRGB2RGBColor", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, HIDKeyboard, object, object> target9 = <>o__106.<>p__16.Target;
		CallSite<Func<CallSite, HIDKeyboard, object, object>> arg14 = <>o__106.<>p__16;
		if (<>o__106.<>p__15 == null)
		{
			<>o__106.<>p__15 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		RGBKB_Color save_layout_color = target8(arg13, target9(arg14, this, <>o__106.<>p__15.Target(<>o__106.<>p__15, (object)data, "color")));
		if (<>o__106.<>p__20 == null)
		{
			<>o__106.<>p__20 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
		}
		Func<CallSite, object, string> target10 = <>o__106.<>p__20.Target;
		CallSite<Func<CallSite, object, string>> arg15 = <>o__106.<>p__20;
		if (<>o__106.<>p__18 == null)
		{
			<>o__106.<>p__18 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		obj = <>o__106.<>p__18.Target(<>o__106.<>p__18, (object)data, "nv_save");
		object arg16;
		if (obj == null)
		{
			arg16 = null;
		}
		else
		{
			if (<>o__106.<>p__19 == null)
			{
				<>o__106.<>p__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg16 = <>o__106.<>p__19.Target(<>o__106.<>p__19, obj);
		}
		string text4 = target10(arg15, arg16);
		_ = (RGBKB_NV_SAVE)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_NV_SAVE).TypeHandle), text4);
		int save_layout_backgroundcolor = 0;
		string save_layout_alphbet = "";
		if (<>o__106.<>p__23 == null)
		{
			<>o__106.<>p__23 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target11 = <>o__106.<>p__23.Target;
		CallSite<Func<CallSite, object, bool>> arg17 = <>o__106.<>p__23;
		if (<>o__106.<>p__22 == null)
		{
			<>o__106.<>p__22 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		Func<CallSite, object, object, object> target12 = <>o__106.<>p__22.Target;
		CallSite<Func<CallSite, object, object, object>> arg18 = <>o__106.<>p__22;
		if (<>o__106.<>p__21 == null)
		{
			<>o__106.<>p__21 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		if (target11(arg17, target12(arg18, <>o__106.<>p__21.Target(<>o__106.<>p__21, (object)data, "backgroundcolor"), null)))
		{
			if (<>o__106.<>p__26 == null)
			{
				<>o__106.<>p__26 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
			}
			Func<CallSite, object, int> target13 = <>o__106.<>p__26.Target;
			CallSite<Func<CallSite, object, int>> arg19 = <>o__106.<>p__26;
			if (<>o__106.<>p__25 == null)
			{
				<>o__106.<>p__25 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target14 = <>o__106.<>p__25.Target;
			CallSite<Func<CallSite, Type, object, object>> arg20 = <>o__106.<>p__25;
			Type arg21 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__106.<>p__24 == null)
			{
				<>o__106.<>p__24 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			save_layout_backgroundcolor = target13(arg19, target14(arg20, arg21, <>o__106.<>p__24.Target(<>o__106.<>p__24, (object)data, "backgroundcolor")));
		}
		if (<>o__106.<>p__30 == null)
		{
			<>o__106.<>p__30 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target15 = <>o__106.<>p__30.Target;
		CallSite<Func<CallSite, object, bool>> arg22 = <>o__106.<>p__30;
		bool flag = layoutEffect.Equals(RGBKB_Effect.Alphabet);
		object arg26;
		if (flag)
		{
			if (<>o__106.<>p__29 == null)
			{
				<>o__106.<>p__29 = CallSite<Func<CallSite, bool, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, bool, object, object> target16 = <>o__106.<>p__29.Target;
			CallSite<Func<CallSite, bool, object, object>> arg23 = <>o__106.<>p__29;
			bool arg24 = flag;
			if (<>o__106.<>p__28 == null)
			{
				<>o__106.<>p__28 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target17 = <>o__106.<>p__28.Target;
			CallSite<Func<CallSite, object, object, object>> arg25 = <>o__106.<>p__28;
			if (<>o__106.<>p__27 == null)
			{
				<>o__106.<>p__27 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			arg26 = target16(arg23, arg24, target17(arg25, <>o__106.<>p__27.Target(<>o__106.<>p__27, (object)data, "alphabet"), null));
		}
		else
		{
			arg26 = flag;
		}
		if (target15(arg22, arg26))
		{
			if (<>o__106.<>p__33 == null)
			{
				<>o__106.<>p__33 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle)));
			}
			Func<CallSite, object, string> target18 = <>o__106.<>p__33.Target;
			CallSite<Func<CallSite, object, string>> arg27 = <>o__106.<>p__33;
			if (<>o__106.<>p__31 == null)
			{
				<>o__106.<>p__31 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			obj = <>o__106.<>p__31.Target(<>o__106.<>p__31, (object)data, "alphabet");
			object arg28;
			if (obj == null)
			{
				arg28 = null;
			}
			else
			{
				if (<>o__106.<>p__32 == null)
				{
					<>o__106.<>p__32 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(HIDKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				arg28 = <>o__106.<>p__32.Target(<>o__106.<>p__32, obj);
			}
			save_layout_alphbet = target18(arg27, arg28);
		}
		SAVE_LIGHTING_EFFECT_DATA sAVE_LIGHTING_EFFECT_DATA = new SAVE_LIGHTING_EFFECT_DATA
		{
			save_mode = save_mode,
			save_effect = Translate_ITE_EffectIndex(layoutEffect),
			save_light = Translate_ITE_LightValue(layoutLight),
			save_speed = Translate_ITE_SpeedValue(layoutSpeed),
			save_direction = Translate_ITE_DirectionValue(layoutDirection),
			save_layout_color = save_layout_color,
			save_layout_alphbet = save_layout_alphbet,
			save_layout_backgroundcolor = save_layout_backgroundcolor
		};
		if (_WelcomeEffectData.Equals(sAVE_LIGHTING_EFFECT_DATA))
		{
			return false;
		}
		_EffectData.save_light = sAVE_LIGHTING_EFFECT_DATA.save_light;
		_WelcomeEffectData = sAVE_LIGHTING_EFFECT_DATA;
		if (CurrentPowerMode == RGBKB_PowerMode.DC)
		{
			_DC_LightLevel = Translate_Layout_LightValue(_EffectData.save_light);
		}
		else
		{
			_AC_LightLevel = Translate_Layout_LightValue(_EffectData.save_light);
		}
		SaveWelcomeEffectData();
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void SetPowerStatus(RGBKB_PowerStatus powerstatus, byte logo_hinge_status = 0, string powerSwitchName = "")
	{
		_PowerStatus = powerstatus;
		if (!_PowerStatus.Equals(RGBKB_PowerStatus.On))
		{
			_KeyboardControl?.ILM_RGBKB_SetPower(powerstatus, logo_hinge_status, powerSwitchName);
			_KeyboardControl?.Set_Welcome_TimeOut_Effect_Enable(Enable: false, _WelcomeEffectData.save_effect, 0, logo_hinge_status);
			SavePowerStatus();
		}
		else
		{
			if (!Monitor.TryEnter(_powerLock, 1500))
			{
				return;
			}
			try
			{
				_KeyboardControl?.ILM_RGBKB_SetPower(powerstatus, 0, "");
				_KeyboardControl?.Set_Welcome_TimeOut_Effect_Enable(Enable: true, _WelcomeEffectData.save_effect, 4, logo_hinge_status);
				if (_EffectData.save_effect != 0)
				{
					RunEffct(0, 2);
				}
				_ = _WelcomeEffectData.save_effect;
				SavePowerStatus();
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(_powerLock);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public RGBKB_PowerStatus GetPowerStatus()
	{
		if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(_powerLock, 1500))
		{
			try
			{
				return _PowerStatus;
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(_powerLock);
			}
		}
		return RGBKB_PowerStatus.Off;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public string GetWelcomeStatus()
	{
		string text = null;
		switch (_WelcomeEffectData.save_effect)
		{
		case 1:
			text = RGBKB_Effect.Single.ToString();
			break;
		case 2:
			text = RGBKB_Effect.Breathing.ToString();
			break;
		case 4:
			text = RGBKB_Effect.Reactive.ToString();
			break;
		case 3:
			text = RGBKB_Effect.Wave.ToString();
			break;
		case 5:
			text = RGBKB_Effect.Rainbow.ToString();
			break;
		case 6:
			text = RGBKB_Effect.Ripple.ToString();
			break;
		case 9:
			text = RGBKB_Effect.Marquee.ToString();
			break;
		case 10:
			text = RGBKB_Effect.Raindrop.ToString();
			break;
		case 14:
			text = RGBKB_Effect.Aurora.ToString();
			break;
		case 21:
			text = RGBKB_Effect.Gaming.ToString();
			break;
		case 19:
			text = RGBKB_Effect.Mix.ToString();
			break;
		case 18:
			text = RGBKB_Effect.Flash.ToString();
			break;
		case 13:
			text = RGBKB_Effect.Impact.ToString();
			break;
		case 33:
			text = RGBKB_Effect.Thinking.ToString();
			break;
		case 164:
			text = RGBKB_Effect.Interlace.ToString();
			break;
		case 165:
			text = RGBKB_Effect.Diagonal.ToString();
			break;
		}
		LogCtrl.Write("KeyboardType: " + _KeyboardControl?.ILM_RGBKB_GetRGBKeyboardType().ToString() + ", Welcome Effect : " + text);
		return text;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public string GetWelcomeDirectionStatus()
	{
		string text = null;
		if (_WelcomeEffectData.save_direction == 0)
		{
			text = RGBKB_Direction.LeftRight.ToString();
		}
		else if (_WelcomeEffectData.save_direction == 2)
		{
			text = RGBKB_Direction.DownUp.ToString();
		}
		else if (_WelcomeEffectData.save_direction == 3)
		{
			text = RGBKB_Direction.RightLeft.ToString();
		}
		else if (_WelcomeEffectData.save_direction == 4)
		{
			text = RGBKB_Direction.UpDown.ToString();
		}
		LogCtrl.Write("Welcome direction : " + text);
		return text;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void NightPowerSwitch(bool NightPower)
	{
		object obj = ResolvedDelegates.System_Threading_Tasks_Task_Run__10;
		<>c__DisplayClass112_0 obj2 = new <>c__DisplayClass112_0();
		obj2.<>4__this = this;
		obj2.NightPower = NightPower;
		obj([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			if (ResolvedDelegates.System_Threading_Monitor_TryEnter__145(obj2.<>4__this._nightModeLock, 200))
			{
				try
				{
					if (obj2.NightPower)
					{
						RGB_S lastNightModeColor = obj2.<>4__this.BackgroundColorConverter(obj2.<>4__this._EffectData.save_layout_backgroundcolor);
						obj2.<>4__this._KeyboardControl?.HID_Set_Effect_Type_08H(4, 0, lastNightModeColor.R, lastNightModeColor.G, lastNightModeColor.B, 0, 0);
						obj2.<>4__this._LastNightModeColor = lastNightModeColor;
					}
					else
					{
						obj2.<>4__this._KeyboardControl?.HID_Set_Effect_Type_08H(5, 0, 0, 0, 0, 0, 0);
						obj2.<>4__this._LastNightModeColor = new RGB_S
						{
							R = 0,
							G = 0,
							B = 0
						};
					}
				}
				finally
				{
					ResolvedDelegates.System_Threading_Monitor_Exit__71(obj2.<>4__this._nightModeLock);
				}
			}
		});
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	protected virtual void ScanCodeUpdatedBrightness(RGBKB_Event_Data event_data)
	{
		if (this.m_m_Layout_Event_handler != null)
		{
			this.m_m_Layout_Event_handler(event_data);
		}
		LM_EC_RGB ecKeyboardControl = _EcKeyboardControl;
		if (ecKeyboardControl == null || ecKeyboardControl.ILM_RGBKB_GetRGBKeyboardType() != RGBKB_Type.SingleZone)
		{
			byte Data = 0;
			MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1894, ref Data);
			Data |= 0x10;
			MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1894, Data);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetChinaMode(int scancode)
	{
		if (scancode == 182 && !MyEcCtrl.Instance.IsChinaMode())
		{
			if (MyEcCtrl.Instance.IsChinaModeOn())
			{
				SetPowerStatus(RGBKB_PowerStatus.On, 0, "");
			}
			else
			{
				SetPowerStatus(RGBKB_PowerStatus.Off, 0, "");
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void SetBrinessByScanCode(int scancode)
	{
		switch (scancode)
		{
		case 177:
		case 178:
		{
			LogCtrl.Write("HID_Default B1 B2 : scan code: " + ResolvedDelegates.System_Int32_ToString__55(ref scancode));
			if (_PowerStatus.Equals(RGBKB_PowerStatus.Off))
			{
				LogCtrl.Write("Scan code open power");
				_PowerStatus = RGBKB_PowerStatus.On;
				SavePowerStatus();
				RunEffct(0, 2);
			}
			uint num2 = Translate_Layout_LightValue(_EffectData.save_light);
			bool flag = true;
			switch (scancode)
			{
			case 177:
				num2 = ((num2 != 0) ? (num2 - 1) : 0u);
				break;
			case 178:
				if (num2 + 1 > 4)
				{
					num2 = 4u;
					flag = false;
				}
				else
				{
					num2++;
				}
				break;
			}
			_EffectData.save_light = Translate_ITE_LightValue(num2);
			if (flag)
			{
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("2ND in scancode support 09h : ", ResolvedDelegates.System_Byte_ToString__119(ref _EffectData.save_light)));
				SetBrightnessStrategy(_EffectData.save_light);
			}
			RGBKB_Event_Data event_data2 = new RGBKB_Event_Data
			{
				event_id = RGBKB_EventID.Brightness_update,
				envet_data_len = 1u,
				event_data = new byte[1]
			};
			event_data2.event_data[0] = (byte)Translate_Layout_LightValue(_EffectData.save_light);
			AsyncBrignessToEC(_EffectData.save_light);
			ScanCodeUpdatedBrightness(event_data2);
			break;
		}
		case 240:
			try
			{
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("HID_Default FO : scan code: ", ResolvedDelegates.System_Int32_ToString__55(ref scancode)));
				if (_PowerStatus.Equals(RGBKB_PowerStatus.Off))
				{
					_PowerStatus = RGBKB_PowerStatus.On;
					SavePowerStatus();
					RunEffct(0, 2);
				}
				byte fWLight = GetFWLight();
				if (_firstApStart)
				{
					_EffectData.save_light = fWLight;
					SetBrightnessStrategy(fWLight);
					RunEffct(0, 2);
					_firstApStart = false;
				}
				else
				{
					_EffectData.save_light = fWLight;
					SetBrightnessStrategy(fWLight);
				}
				RGBKB_Event_Data event_data3 = new RGBKB_Event_Data
				{
					event_id = RGBKB_EventID.Brightness_update,
					envet_data_len = 1u,
					event_data = new byte[1]
				};
				event_data3.event_data[0] = (byte)Translate_Layout_LightValue(fWLight);
				ScanCodeUpdatedBrightness(event_data3);
				ResolvedDelegates.System_Threading_Thread_Sleep__6(50);
				break;
			}
			catch (Exception)
			{
				break;
			}
		case 185:
		{
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("HID_Default Circle B9 : scan code: ", ResolvedDelegates.System_Int32_ToString__55(ref scancode)));
			if (_PowerStatus.Equals(RGBKB_PowerStatus.Off))
			{
				LogCtrl.Write("Scan code open power");
				_PowerStatus = RGBKB_PowerStatus.On;
				SavePowerStatus();
				RunEffct(0, 2);
			}
			uint num = Translate_Layout_LightValue(_EffectData.save_light);
			num = (num + 1) % 5;
			_EffectData.save_light = Translate_ITE_LightValue(num);
			SetBrightnessStrategy(_EffectData.save_light);
			RGBKB_Event_Data event_data = new RGBKB_Event_Data
			{
				event_id = RGBKB_EventID.Brightness_update,
				envet_data_len = 1u,
				event_data = new byte[1]
			};
			event_data.event_data[0] = (byte)Translate_Layout_LightValue(_EffectData.save_light);
			AsyncBrignessToEC(_EffectData.save_light);
			ScanCodeUpdatedBrightness(event_data);
			break;
		}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private byte GetFWLight()
	{
		byte light = 0;
		uint num = 0u;
		while (true)
		{
			_KeyboardControl?.Get_ITE_Light_Value(ref light);
			if (light == _EffectData.save_light || (light != 1 && light != _EffectData.save_light) || num >= 5)
			{
				break;
			}
			ResolvedDelegates.System_Threading_Thread_Sleep__6(100);
			num++;
		}
		return light;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void UnPlugged()
	{
		byte brightness = Translate_ITE_LightValue(_DC_LightLevel);
		CurrentPowerMode = RGBKB_PowerMode.DC;
		PluggedSetBrightness(brightness);
		LM_ITE_RGB keyboardControl = _KeyboardControl;
		if (keyboardControl != null && keyboardControl.ILM_RGBKB_GetRGBKeyboardType() == RGBKB_Type.FourZone && _EffectData.save_effect == 34)
		{
			_KeyboardControl?.StopMusicTransfer();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void Plugged()
	{
		byte brightness = Translate_ITE_LightValue(_AC_LightLevel);
		CurrentPowerMode = RGBKB_PowerMode.AC;
		PluggedSetBrightness(brightness);
		LM_ITE_RGB keyboardControl = _KeyboardControl;
		if (keyboardControl != null && keyboardControl.ILM_RGBKB_GetRGBKeyboardType() == RGBKB_Type.FourZone && _EffectData.save_effect == 34)
		{
			RunEffct(0, 2);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void PluggedSetBrightness(uint brightness)
	{
		try
		{
			SetBrightnessStrategy((byte)brightness);
			_EffectData.save_light = (byte)brightness;
			RGBKB_Event_Data event_data = new RGBKB_Event_Data
			{
				event_id = RGBKB_EventID.Brightness_update,
				envet_data_len = 2u,
				event_data = new byte[2]
			};
			event_data.event_data[0] = (byte)Translate_Layout_LightValue(_EffectData.save_light);
			event_data.event_data[1] = 2;
			ScanCodeUpdatedBrightness(event_data);
		}
		catch (Exception)
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void KeyboardcloseTimer_ControlBrigtnessEvent(object sender, EventArgs e)
	{
		if (CheckAurora.Instance.GetAuroraStauts())
		{
			return;
		}
		if (Convert.ToBoolean(sender))
		{
			beforeBrightness = GetBrightness();
			LM_ITE_RGB keyboardControl = _KeyboardControl;
			if (keyboardControl != null && keyboardControl.ILM_RGBKB_GetRGBKeyboardType() == RGBKB_Type.FourZone && _EffectData.save_effect == 34)
			{
				_KeyboardControl?.StopMusicTransfer();
			}
			CloseAnimation();
			return;
		}
		bool unplug = CurrentPowerMode == RGBKB_PowerMode.DC;
		InitalizeKeyboardEffect(unplug);
		if (_EffectData.save_effect != 51)
		{
			RGBKB_Type? rGBKB_Type = _KeyboardControl?.ILM_RGBKB_GetRGBKeyboardType();
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(rGBKB_Type.ToString(), " CloseTimer ee"));
		}
		else
		{
			RGBKB_Type? rGBKB_Type = _KeyboardControl?.ILM_RGBKB_GetRGBKeyboardType();
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29(rGBKB_Type.ToString(), " CloseTimer usermode ee"));
		}
		LM_ITE_RGB keyboardControl2 = _KeyboardControl;
		if (keyboardControl2 != null && keyboardControl2.ILM_RGBKB_GetRGBKeyboardType() == RGBKB_Type.FourZone && _EffectData.save_effect == 34)
		{
			_KeyboardControl?.StartMusicTransfer();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void SetBrightness(uint level)
	{
		try
		{
			_EffectData.save_light = Translate_ITE_LightValue(level);
			SetBrightnessStrategy(_EffectData.save_light);
			AsyncBrignessToEC(_EffectData.save_light);
		}
		catch (Exception)
		{
		}
	}

	private void AsyncBrignessToEC(byte brightness)
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal byte ConvertToByte(BitArray bits)
	{
		ResolvedDelegates.System_Collections_BitArray_get_Count__447(bits);
		_ = 8;
		byte[] array = new byte[1];
		ResolvedDelegates.System_Collections_BitArray_CopyTo__448(bits, array, 0);
		return array[0];
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual uint GetBrightness()
	{
		return Translate_Layout_LightValue(_EffectData.save_light);
	}

	public virtual void SetBrightness_ACDC(uint ACBrightness, uint DCBrighteness, bool saveACDC = true)
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal virtual void SetBrightnessStrategy(byte brightness, bool saveACDC = true)
	{
		if (saveACDC)
		{
			if (CurrentPowerMode == RGBKB_PowerMode.DC)
			{
				_DC_LightLevel = Translate_Layout_LightValue(brightness);
			}
			else
			{
				_AC_LightLevel = Translate_Layout_LightValue(brightness);
			}
			SaveEffectData();
		}
		if (_EffectData.save_mode.Equals(RGBKB_Mode.Night))
		{
			if (brightness < 8)
			{
				_BeforeNightIsZero = true;
				_KeyboardControl?.ILM_RGBKB_SetLightingPower(RGBKB_PowerStatus.Off, 0);
				return;
			}
			if (_BeforeNightIsZero)
			{
				_KeyboardControl.UserModeLight = brightness;
				BackgroundColorConverter(_EffectData.save_layout_backgroundcolor);
				NightPowerSwitch(NightPower: true);
				_KeyboardControl.ChangeEffect = false;
				_KeyboardControl.Set_Lighting_Effect(controlByte, _EffectData.save_effect, brightness, _EffectData.save_speed, _EffectData.save_direction, 0, _EffectData.save_layout_color, _EffectData.save_layout_backgroundcolor, _EffectData.save_layout_alphbet);
			}
			else if (_UserModeEffectList.Contains(_EffectData.save_effect))
			{
				_KeyboardControl.UserModeLight = brightness;
			}
			else if (_EffectData.save_effect.Equals(21) || ResolvedDelegates.System_Byte_Equals__749(ref _EffectData.save_effect, 51))
			{
				_KeyboardControl.UserModeLight = brightness;
				_KeyboardControl?.Set_Lighting_Effect(controlByte, _EffectData.save_effect, brightness, _EffectData.save_speed, _EffectData.save_direction, 0, _EffectData.save_layout_color, _EffectData.save_layout_backgroundcolor, _EffectData.save_layout_alphbet);
			}
			else
			{
				_KeyboardControl?.HID_Set_Brightness_Level_09H_ControlCode(brightness, controlByte, _EffectData, _PowerStatus);
			}
			_BeforeNightIsZero = false;
		}
		else
		{
			_KeyboardControl?.HID_Set_Brightness_Level_09H_ControlCode(brightness, 2, _EffectData, _PowerStatus);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void RunWelcomeEffect()
	{
		if (_PowerStatus.Equals(RGBKB_PowerStatus.On) && _Mode == RGBKB_Mode.Welcome)
		{
			_KeyboardControl?.Set_Welcome_Effect(3, _WelcomeEffectData.save_effect, _WelcomeEffectData.save_light, _WelcomeEffectData.save_speed, _WelcomeEffectData.save_direction, Translate_ITE_LightValue(_DC_LightLevel), _WelcomeEffectData.save_layout_color);
			if (_EffectData.save_effect != 0)
			{
				RunEffct(1, 2);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void NightModeReflash(RGBKB_Mode mode)
	{
		if (mode.Equals(RGBKB_Mode.Night))
		{
			NightPowerSwitch(NightPower: true);
			_NightModSwitch = true;
			return;
		}
		if (_NightModSwitch)
		{
			NightPowerSwitch(NightPower: false);
		}
		_NightModSwitch = false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void RunEffct(byte SAVED = 0, byte control = 2)
	{
		<>c__DisplayClass130_0 CS$<>8__locals20 = new <>c__DisplayClass130_0();
		CS$<>8__locals20.<>4__this = this;
		CS$<>8__locals20.SAVED = SAVED;
		if (CheckAurora.Instance.GetAuroraStauts())
		{
			return;
		}
		try
		{
			if (!_PowerStatus.Equals(RGBKB_PowerStatus.On))
			{
				return;
			}
			LogCtrl.TraceMessage("KeyboardType: " + _KeyboardControl?.ILM_RGBKB_GetRGBKeyboardType().ToString() + ", Run Effect in " + ResolvedDelegates.System_Byte_ToString__119(ref _EffectData.save_effect), "RunEffct", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1665);
			if (_EffectData.save_mode == RGBKB_Mode.Lighting)
			{
				_KeyboardControl.m_effect_type = GetEffectType(_EffectData.save_effect);
				_KeyboardControl.ChangeEffect = _LastEffectRun != _EffectData.save_effect;
				NightModeReflash(_EffectData.save_mode);
				if (_AllPowerAnimation)
				{
					ResolvedDelegates.System_Threading_Tasks_Task_ContinueWith__507(ResolvedDelegates.System_Threading_Tasks_Task_Run__10([MethodImpl(MethodImplOptions.NoInlining)] () =>
					{
						CS$<>8__locals20.<>4__this._KeyboardControl?.Set_Lighting_Effect(2, CS$<>8__locals20.<>4__this._EffectData.save_effect, 0, CS$<>8__locals20.<>4__this._EffectData.save_speed, CS$<>8__locals20.<>4__this._EffectData.save_direction, CS$<>8__locals20.SAVED, CS$<>8__locals20.<>4__this._EffectData.save_layout_color, 0, CS$<>8__locals20.<>4__this._EffectData.save_layout_alphbet);
					}), (Task task) =>
					{
						CS$<>8__locals20.<>4__this.OpenAnimation();
					});
				}
				else
				{
					_KeyboardControl?.Set_Lighting_Effect(control, _EffectData.save_effect, _EffectData.save_light, _EffectData.save_speed, _EffectData.save_direction, CS$<>8__locals20.SAVED, _EffectData.save_layout_color, 0, _EffectData.save_layout_alphbet);
				}
				SaveEffectData();
				_LastEffectRun = _EffectData.save_effect;
			}
			else if (_EffectData.save_mode == RGBKB_Mode.Night)
			{
				_KeyboardControl.m_effect_type = GetEffectType(_EffectData.save_effect);
				_KeyboardControl.ChangeEffect = _LastEffectRun != _EffectData.save_effect;
				NightModeReflash(_EffectData.save_mode);
				if (_EffectData.save_light == 0)
				{
					_KeyboardControl.UserModeLight = 0;
					_BeforeNightIsZero = true;
				}
				else
				{
					_KeyboardControl.UserModeLight = _EffectData.save_light;
					_BeforeNightIsZero = false;
				}
				if (_AllPowerAnimation)
				{
					ResolvedDelegates.System_Threading_Tasks_Task_ContinueWith__507(ResolvedDelegates.System_Threading_Tasks_Task_Run__10([MethodImpl(MethodImplOptions.NoInlining)] () =>
					{
						CS$<>8__locals20.<>4__this._KeyboardControl?.Set_Lighting_Effect(4, CS$<>8__locals20.<>4__this._EffectData.save_effect, 0, CS$<>8__locals20.<>4__this._EffectData.save_speed, CS$<>8__locals20.<>4__this._EffectData.save_direction, CS$<>8__locals20.SAVED, CS$<>8__locals20.<>4__this._EffectData.save_layout_color, 0, CS$<>8__locals20.<>4__this._EffectData.save_layout_alphbet);
					}), (Task task) =>
					{
						CS$<>8__locals20.<>4__this.OpenAnimation();
					});
				}
				else
				{
					_KeyboardControl?.Set_Lighting_Effect(4, _EffectData.save_effect, _EffectData.save_light, _EffectData.save_speed, _EffectData.save_direction, CS$<>8__locals20.SAVED, _EffectData.save_layout_color, _EffectData.save_layout_backgroundcolor, _EffectData.save_layout_alphbet);
					if (_EffectData.save_effect == 21)
					{
						NightModeReflash(_EffectData.save_mode);
					}
				}
				if (_EffectData.save_light == 0)
				{
					_KeyboardControl?.ILM_RGBKB_SetLightingPower(RGBKB_PowerStatus.Off, 0);
				}
				SaveEffectData();
				_LastEffectRun = _EffectData.save_effect;
			}
			else
			{
				LogCtrl.TraceMessage("Run Effect But Power Off ", "RunEffct", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1786);
			}
		}
		catch (Exception)
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RunEffctEC()
	{
		try
		{
			if (mbLogo == 0)
			{
				return;
			}
			bool flag = false;
			if (_PowerStatus.Equals(RGBKB_PowerStatus.On))
			{
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("RunEffctEC(), Run Effect in ", ResolvedDelegates.System_Byte_ToString__119(ref _EffectData.save_effect)));
				byte Data = 0;
				byte b = 64;
				switch (ResolvedDelegates.System_Convert_ToUInt32__161(_EffectData.save_light))
				{
				case 0u:
					b = 0;
					break;
				case 25u:
					b = 32;
					break;
				case 50u:
					b = 64;
					break;
				case 75u:
					b = 96;
					break;
				case 100u:
					b = 128;
					break;
				}
				if (_EffectData.save_effect == 1)
				{
					byte Data2 = 0;
					MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1895, ref Data2);
					LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("0x767 Single Read: ", ResolvedDelegates.System_Byte_ToString__119(ref Data2)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1847);
					Data2 &= 0x7F;
					MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1895, Data2);
					LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("0x767 Single Write: ", ResolvedDelegates.System_Byte_ToString__119(ref Data2)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1850);
					Data2 = 0;
					MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, ref Data2);
					Data2 &= 0xF8;
					MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, Data2);
					byte b2 = DivideValue(_EffectData.save_layout_color.ColorBuffer[0].R, 5.1);
					byte b3 = DivideValue(_EffectData.save_layout_color.ColorBuffer[0].G, 5.1);
					byte b4 = DivideValue(_EffectData.save_layout_color.ColorBuffer[0].B, 5.1);
					if (b2 == 0 && b3 == 0 && b4 == 0)
					{
						flag = true;
					}
					else
					{
						MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1897, b2);
						MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1898, b3);
						MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1899, b4);
						LogCtrl.Write(ResolvedDelegates.System_String_Concat__79(new string[6]
						{
							"RunEffctEC() RGB: ",
							ResolvedDelegates.System_Byte_ToString__119(ref b2),
							",",
							ResolvedDelegates.System_Byte_ToString__119(ref b3),
							",",
							ResolvedDelegates.System_Byte_ToString__119(ref b4)
						}));
					}
				}
				else if (_EffectData.save_effect == 2)
				{
					byte Data3 = 0;
					MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1895, ref Data3);
					LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("0x767 Breathing Read: ", ResolvedDelegates.System_Byte_ToString__119(ref Data3)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1880);
					Data3 &= 0x7F;
					MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1895, Data3);
					LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("0x767 Breathing Write: ", ResolvedDelegates.System_Byte_ToString__119(ref Data3)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1883);
					int num = 0;
					byte b5 = 0;
					int r = _EffectData.save_layout_color.ColorBuffer[0].R;
					int g = _EffectData.save_layout_color.ColorBuffer[0].G;
					num = _EffectData.save_layout_color.ColorBuffer[0].B;
					if (r == 255 && g == 0 && num == 0)
					{
						b5 = 2;
					}
					else if (r == 0 && g == 255 && num == 0)
					{
						b5 = 3;
					}
					else if (r == 0 && g == 0 && num == 255)
					{
						b5 = 4;
					}
					else if (r == 255 && g == 255 && num == 0)
					{
						b5 = 5;
					}
					else if (r == 136 && g == 0 && num == 255)
					{
						b5 = 6;
					}
					else if (r == 0 && g == 255 && num == 255)
					{
						b5 = 7;
					}
					else if (r == 255 && g == 255 && num == 255)
					{
						b5 = 1;
					}
					Data3 = 0;
					MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, ref Data3);
					Data3 &= 0xF8;
					Data3 |= b5;
					MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, Data3);
				}
				else if (_EffectData.save_effect == 19)
				{
					byte Data4 = 0;
					MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, ref Data4);
					Data4 &= 0xF8;
					MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, Data4);
					Data4 = 0;
					MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1895, ref Data4);
					LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("0x767 Colorful Read: ", ResolvedDelegates.System_Byte_ToString__119(ref Data4)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1922);
					Data4 &= 0x7F;
					Data4 |= 0x80;
					MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1895, Data4);
					LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("0x767 Colorful Write: ", ResolvedDelegates.System_Byte_ToString__119(ref Data4)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1926);
				}
				MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1963, ref Data);
				Data &= 0x1F;
				if (flag)
				{
					b = 0;
				}
				Data |= b;
				MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1963, Data);
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("RunEffctEC() Brightness: ", ResolvedDelegates.System_Byte_ToString__119(ref Data)));
				byte Data5 = 0;
				MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1932, ref Data5);
				Data5 |= 0x10;
				MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1932, Data5);
			}
			else
			{
				byte b6 = 0;
				byte Data6 = 0;
				MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1963, ref Data6);
				Data6 &= 0x1F;
				Data6 |= b6;
				MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1963, Data6);
				byte Data7 = 0;
				MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1932, ref Data7);
				Data7 |= 0x10;
				MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1932, Data7);
			}
		}
		catch (Exception)
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void CloseEffectEC()
	{
		try
		{
			byte b = 0;
			byte Data = 0;
			MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1963, ref Data);
			Data &= 0x1F;
			Data |= b;
			MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1963, Data);
			byte Data2 = 0;
			MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1932, ref Data2);
			Data2 |= 0x10;
			MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1932, Data2);
		}
		catch (Exception)
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static byte DivideValue(byte value, double divisor)
	{
		return (byte)ResolvedDelegates.System_Math_Min__755((int)ResolvedDelegates.System_Math_Floor__188((double)(int)value / divisor), 50);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void CloseAnimation()
	{
		if (_CloseAnimation && _PowerStatus == RGBKB_PowerStatus.On)
		{
			try
			{
				SetBrightnessStrategy(0, saveACDC: false);
			}
			catch
			{
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void OpenAnimation()
	{
		if (!_OpenAnimation)
		{
			return;
		}
		_OpenAnimation = false;
		if (_EffectData.save_effect == 2 || _EffectData.save_effect == 10)
		{
			SetBrightnessStrategy(_EffectData.save_light);
		}
		else if (_EffectData.save_effect == 3 || _EffectData.save_effect == 14)
		{
			Thread.Sleep(100);
			for (int i = 0; i <= _EffectData.save_light; i += 8)
			{
				SetBrightnessStrategy((byte)i);
				ResolvedDelegates.System_Threading_Thread_Sleep__6(ResolvedDelegates.System_Convert_ToInt32__494((double)(50 - i) * 2.5));
			}
		}
		else
		{
			Thread.Sleep(50);
			for (int j = 0; j <= _EffectData.save_light; j += 8)
			{
				SetBrightnessStrategy((byte)j);
				ResolvedDelegates.System_Threading_Thread_Sleep__6((50 - j) * 2);
			}
		}
		_OpenAnimation = true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				this.m_m_Layout_Event_handler = null;
			}
			disposed = true;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Dispose()
	{
		Dispose(disposing: true);
		ResolvedDelegates.System_GC_SuppressFinalize__756(this);
	}

	public virtual void Uninstall()
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public SAVE_LIGHTING_EFFECT_DATA GetAllKeyboardStatus()
	{
		return _EffectData;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual bool GetSingleZonePowerSupport()
	{
		byte Data = 0;
		MyEcCtrl.Instance.Read("HIDKeyboard", 1934, ref Data);
		return ResolvedDelegates.System_Collections_BitArray_get_Item__219(new BitArray(new byte[1] { Data }), 5);
	}

	public virtual int GetMonochromeIndex()
	{
		return 0;
	}

	public virtual int GetManualIndex1()
	{
		return 0;
	}

	public virtual int GetManualIndex2()
	{
		return 0;
	}

	public virtual int GetManualIndex3()
	{
		return 0;
	}

	public virtual int GetManualIndex4()
	{
		return 0;
	}

	public virtual int GetManualIndex5()
	{
		return 0;
	}

	public virtual int GetManualIndex6()
	{
		return 0;
	}

	public virtual int GetManualInterval()
	{
		return 0;
	}

	public virtual int GetBreathingIndex()
	{
		return 0;
	}

	public virtual string GetDefaultData()
	{
		return "";
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual RGBKB_Color GetLayoutColor()
	{
		return _EffectData.save_layout_color;
	}
}

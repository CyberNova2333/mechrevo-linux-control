// Static analysis rendering; not original buildable source.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using GCUService.MyRgbKeyboard;
using GCUService.ScanCode;
using LightingModel;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using MyControlCenter;
using MyControlCenter.MyRgbKeyboard;
using MyECIO;
using Utility;

namespace MyRGBKeyboard;

public class RGBKeyboard
{
	[CompilerGenerated]
	private sealed class <>c__DisplayClass25_0
	{
		public RGBKeyboard <>4__this;

		public RGBKB_Event_Data event_data;

		public int brightness;

		public <>c__DisplayClass25_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <Event_LM>b__0()
		{
			_ = new
			{
				MqttID = "KeyboardUserId",
				function = "UpdateBrightness",
				level = brightness
			};
			if (<>4__this.m_hidkeyboad != null)
			{
				_RgbKeyboardStatus.solution = m_LM.m_KB_Solution.ToString();
				_RgbKeyboardStatus.type = m_LM.m_KB_Type.ToString();
				_RgbKeyboardStatus.powerStatus = <>4__this.m_hidkeyboad?.GetPowerStatus().ToString();
				RGBKeyboardStauts rgbKeyboardStatus = _RgbKeyboardStatus;
				HIDKeyboard hidkeyboad = <>4__this.m_hidkeyboad;
				object brightNess;
				if (hidkeyboad == null)
				{
					brightNess = null;
				}
				else
				{
					uint num = hidkeyboad.GetBrightness();
					brightNess = ResolvedDelegates.System_UInt32_ToString__78(ref num);
				}
				rgbKeyboardStatus.brightNess = (string)brightNess;
				_RgbKeyboardStatus.ACBrightness = <>4__this.m_hidkeyboad?._AC_LightLevel.ToString();
				_RgbKeyboardStatus.DCBrightness = <>4__this.m_hidkeyboad?._DC_LightLevel.ToString();
				_RgbKeyboardStatus.welcomeStatus = <>4__this.m_hidkeyboad?.GetWelcomeStatus();
				_RgbKeyboardStatus.welcomeDirectionStatus = <>4__this.m_hidkeyboad?.GetWelcomeDirectionStatus();
				_RgbKeyboardStatus.auroraStatus = <>4__this.aurora.GetAuroraStauts();
				_RgbKeyboardStatus.currentPowerMode = <>4__this.m_hidkeyboad?.CurrentPowerMode.ToString();
				m_MQTTClient.Publish("Keyboard/Status", _RgbKeyboardStatus);
			}
			if (event_data.envet_data_len == 1)
			{
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("RGBKeyboard|Event_LM : UpdateStatusToOSD level: ", ResolvedDelegates.System_Int32_ToString__55(ref brightness)));
				<>4__this.UpdateStatusToOSD(brightness);
			}
			else
			{
				LogCtrl.Write("Only update UI");
			}
		}
	}

	[CompilerGenerated]
	private static class <>o__24
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

	[CompilerGenerated]
	private static class <>o__33
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string>> <>p__2;

		public static CallSite<Func<CallSite, HIDKeyboard, object, object>> <>p__3;

		public static CallSite<Func<CallSite, bool, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, bool>> <>p__5;

		public static CallSite<Func<CallSite, HIDKeyboard, object, object>> <>p__6;

		public static CallSite<Func<CallSite, bool, object, object>> <>p__7;

		public static CallSite<Func<CallSite, object, bool>> <>p__8;
	}

	[CompilerGenerated]
	private static class <>o__34
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string, object>> <>p__2;

		public static CallSite<Func<CallSite, object, object, object>> <>p__3;

		public static CallSite<Func<CallSite, object, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, bool>> <>p__5;

		public static CallSite<Func<CallSite, object, bool>> <>p__6;

		public static CallSite<Func<CallSite, object, string, object>> <>p__7;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__8;

		public static CallSite<Func<CallSite, object, uint>> <>p__9;

		public static CallSite<Func<CallSite, object, string, object>> <>p__10;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__11;

		public static CallSite<Func<CallSite, object, uint>> <>p__12;
	}

	[CompilerGenerated]
	private static class <>o__35
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string>> <>p__2;

		public static CallSite<Func<CallSite, HIDKeyboard, object, object>> <>p__3;

		public static CallSite<Func<CallSite, object, bool>> <>p__4;

		public static CallSite<Func<CallSite, object, string, object>> <>p__5;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__6;

		public static CallSite<Func<CallSite, object, uint>> <>p__7;
	}

	[CompilerGenerated]
	private static class <>o__36
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string>> <>p__2;

		public static CallSite<Func<CallSite, object, string, object>> <>p__3;

		public static CallSite<Func<CallSite, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, string>> <>p__5;

		public static CallSite<Func<CallSite, object, string, object>> <>p__6;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__7;

		public static CallSite<Func<CallSite, object, RGBKB_Color>> <>p__8;
	}

	[CompilerGenerated]
	private static class <>o__40
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string>> <>p__2;

		public static CallSite<Func<CallSite, object, string, object>> <>p__3;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, string>> <>p__5;

		public static CallSite<Func<CallSite, object, string, object>> <>p__6;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__7;

		public static CallSite<Func<CallSite, object, bool>> <>p__8;

		public static CallSite<Func<CallSite, object, string, object>> <>p__9;

		public static CallSite<Func<CallSite, object, object>> <>p__10;

		public static CallSite<Func<CallSite, object, string>> <>p__11;

		public static CallSite<Action<CallSite, RGBKeyboard, object>> <>p__12;

		public static CallSite<Action<CallSite, RGBKeyboard, object>> <>p__13;

		public static CallSite<Action<CallSite, RGBKeyboard, object>> <>p__14;

		public static CallSite<Action<CallSite, RGBKeyboard, object>> <>p__15;

		public static CallSite<Action<CallSite, RGBKeyboard, object>> <>p__16;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <OnMqttMessage>d__40 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public string topic;

		public string msg;

		public RGBKeyboard <>4__this;

		private TaskAwaiter<object> <>u__1;

		private RGBKeyboardStauts <>7__wrap1;

		private TaskAwaiter<string> <>u__2;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe void MoveNext()
		{
			int num = <>1__state;
			RGBKeyboard CS$<>8__locals87 = <>4__this;
			try
			{
				_ = 4;
				try
				{
					TaskAwaiter<object> awaiter2;
					TaskAwaiter<string> awaiter;
					string text;
					object result;
					Func<CallSite, object, string> target;
					CallSite<Func<CallSite, object, string>> arg;
					Func<CallSite, Type, object, object> target2;
					CallSite<Func<CallSite, Type, object, object>> arg2;
					Type arg3;
					Func<CallSite, object, string> target3;
					CallSite<Func<CallSite, object, string>> arg4;
					Func<CallSite, Type, object, object> target4;
					CallSite<Func<CallSite, Type, object, object>> arg5;
					Type arg6;
					string text2;
					char c;
					string result2;
					switch (num)
					{
					default:
						if (ResolvedDelegates.System_String_op_Equality__18(topic, "Keyboard/Ctrl"))
						{
							awaiter2 = Json.ToObjectAsync<object>(msg).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (<>1__state = 0);
								<>u__1 = awaiter2;
								<>t__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
								return;
							}
							goto IL_00ae;
						}
						break;
					case 0:
						awaiter2 = <>u__1;
						<>u__1 = default;
						num = (<>1__state = -1);
						goto IL_00ae;
					case 1:
					case 2:
						try
						{
							if (num != 1)
							{
								if (num != 2)
								{
									if (m_LM == null)
									{
										break;
									}
									if (m_LM.m_KB_Type != RGBKB_Type.SingleZone)
									{
										_RgbKeyboardStatus.solution = m_LM.m_KB_Solution.ToString();
										_RgbKeyboardStatus.type = m_LM.m_KB_Type.ToString();
										RGBKeyboardStauts rgbKeyboardStatus = _RgbKeyboardStatus;
										HIDKeyboard hidkeyboad = CS$<>8__locals87.m_hidkeyboad;
										object brightNess;
										if (hidkeyboad == null)
										{
											brightNess = null;
										}
										else
										{
											uint brightness = hidkeyboad.GetBrightness();
											brightNess = ResolvedDelegates.System_UInt32_ToString__78(ref brightness);
										}
										rgbKeyboardStatus.brightNess = (string)brightNess;
										_RgbKeyboardStatus.ACBrightness = CS$<>8__locals87.m_hidkeyboad?._AC_LightLevel.ToString();
										_RgbKeyboardStatus.DCBrightness = CS$<>8__locals87.m_hidkeyboad?._DC_LightLevel.ToString();
										_RgbKeyboardStatus.powerStatus = CS$<>8__locals87.m_hidkeyboad?.GetPowerStatus().ToString();
										_RgbKeyboardStatus.welcomeStatus = CS$<>8__locals87.m_hidkeyboad?.GetWelcomeStatus();
										_RgbKeyboardStatus.welcomeDirectionStatus = CS$<>8__locals87.m_hidkeyboad?.GetWelcomeDirectionStatus();
										_RgbKeyboardStatus.auroraStatus = CS$<>8__locals87.aurora.GetAuroraStauts();
										_RgbKeyboardStatus.auroraExist = CS$<>8__locals87.aurora.GetAuroraExists();
										_RgbKeyboardStatus.currentPowerMode = CS$<>8__locals87.m_hidkeyboad?.CurrentPowerMode.ToString();
										RGBKeyboardStauts rgbKeyboardStatus2 = _RgbKeyboardStatus;
										HIDKeyboard hidkeyboad2 = CS$<>8__locals87.m_hidkeyboad;
										object effect;
										if (hidkeyboad2 == null)
										{
											effect = null;
										}
										else
										{
											SAVE_LIGHTING_EFFECT_DATA allKeyboardStatus = hidkeyboad2.GetAllKeyboardStatus();
											effect = ResolvedDelegates.System_Byte_ToString__119(ref allKeyboardStatus.save_effect);
										}
										rgbKeyboardStatus2.effect = (string)effect;
										_RgbKeyboardStatus.powerstatus = ((CS$<>8__locals87.m_hidkeyboad?.GetPowerStatus().ToString() == "On") ? 1 : 0);
										RGBKeyboardStauts rgbKeyboardStatus3 = _RgbKeyboardStatus;
										HIDKeyboard hidkeyboad3 = CS$<>8__locals87.m_hidkeyboad;
										object light;
										if (hidkeyboad3 == null)
										{
											light = null;
										}
										else
										{
											uint brightness = hidkeyboad3.GetBrightness();
											light = ResolvedDelegates.System_UInt32_ToString__78(ref brightness);
										}
										rgbKeyboardStatus3.light = (string)light;
										RGBKeyboardStauts rgbKeyboardStatus4 = _RgbKeyboardStatus;
										HIDKeyboard hidkeyboad4 = CS$<>8__locals87.m_hidkeyboad;
										object speed;
										if (hidkeyboad4 == null)
										{
											speed = null;
										}
										else
										{
											SAVE_LIGHTING_EFFECT_DATA allKeyboardStatus = hidkeyboad4.GetAllKeyboardStatus();
											speed = ResolvedDelegates.System_Byte_ToString__119(ref allKeyboardStatus.save_speed);
										}
										rgbKeyboardStatus4.speed = (string)speed;
										RGBKeyboardStauts rgbKeyboardStatus5 = _RgbKeyboardStatus;
										HIDKeyboard hidkeyboad5 = CS$<>8__locals87.m_hidkeyboad;
										object direction;
										if (hidkeyboad5 == null)
										{
											direction = null;
										}
										else
										{
											SAVE_LIGHTING_EFFECT_DATA allKeyboardStatus = hidkeyboad5.GetAllKeyboardStatus();
											direction = ResolvedDelegates.System_Byte_ToString__119(ref allKeyboardStatus.save_direction);
										}
										rgbKeyboardStatus5.direction = (string)direction;
										<>7__wrap1 = _RgbKeyboardStatus;
										awaiter = Json.StringifyAsync(CS$<>8__locals87.m_hidkeyboad?.GetAllKeyboardStatus().save_layout_color).GetAwaiter();
										if (!awaiter.IsCompleted)
										{
											num = (<>1__state = 1);
											<>u__2 = awaiter;
											<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
											return;
										}
										goto IL_066f;
									}
									if (m_LM.m_KB_Type == RGBKB_Type.SingleZone && CS$<>8__locals87.m_hidkeyboad != null)
									{
										RGBKeyboardStauts rgbKeyboardStatus6 = _RgbKeyboardStatus;
										HIDKeyboard hidkeyboad6 = CS$<>8__locals87.m_hidkeyboad;
										object effect2;
										if (hidkeyboad6 == null)
										{
											effect2 = null;
										}
										else
										{
											SAVE_LIGHTING_EFFECT_DATA allKeyboardStatus = hidkeyboad6.GetAllKeyboardStatus();
											effect2 = ResolvedDelegates.System_Byte_ToString__119(ref allKeyboardStatus.save_effect);
										}
										rgbKeyboardStatus6.effect = (string)effect2;
										RGBKeyboardStauts rgbKeyboardStatus7 = _RgbKeyboardStatus;
										HIDKeyboard hidkeyboad7 = CS$<>8__locals87.m_hidkeyboad;
										object speed2;
										if (hidkeyboad7 == null)
										{
											speed2 = null;
										}
										else
										{
											SAVE_LIGHTING_EFFECT_DATA allKeyboardStatus = hidkeyboad7.GetAllKeyboardStatus();
											speed2 = ResolvedDelegates.System_Byte_ToString__119(ref allKeyboardStatus.save_speed);
										}
										rgbKeyboardStatus7.speed = (string)speed2;
										_RgbKeyboardStatus.powerStatus = CS$<>8__locals87.m_hidkeyboad?.GetPowerStatus().ToString();
										_RgbKeyboardStatus.solution = m_LM.m_KB_Solution.ToString();
										_RgbKeyboardStatus.type = m_LM.m_KB_Type.ToString();
										_RgbKeyboardStatus.SingleZonePowerSupport = Convert.ToBoolean(CS$<>8__locals87.m_hidkeyboad?.GetSingleZonePowerSupport());
										RGBKeyboardStauts rgbKeyboardStatus8 = _RgbKeyboardStatus;
										HIDKeyboard hidkeyboad8 = CS$<>8__locals87.m_hidkeyboad;
										object brightNess2;
										if (hidkeyboad8 == null)
										{
											brightNess2 = null;
										}
										else
										{
											uint brightness = hidkeyboad8.GetBrightness();
											brightNess2 = ResolvedDelegates.System_UInt32_ToString__78(ref brightness);
										}
										rgbKeyboardStatus8.brightNess = (string)brightNess2;
										RGBKeyboardStauts rgbKeyboardStatus9 = _RgbKeyboardStatus;
										int monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetMonochromeIndex();
										rgbKeyboardStatus9.MonochromeIndex = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
										RGBKeyboardStauts rgbKeyboardStatus10 = _RgbKeyboardStatus;
										monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex1();
										rgbKeyboardStatus10.ManualIndex1 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
										RGBKeyboardStauts rgbKeyboardStatus11 = _RgbKeyboardStatus;
										monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex2();
										rgbKeyboardStatus11.ManualIndex2 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
										RGBKeyboardStauts rgbKeyboardStatus12 = _RgbKeyboardStatus;
										monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex3();
										rgbKeyboardStatus12.ManualIndex3 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
										RGBKeyboardStauts rgbKeyboardStatus13 = _RgbKeyboardStatus;
										monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex4();
										rgbKeyboardStatus13.ManualIndex4 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
										RGBKeyboardStauts rgbKeyboardStatus14 = _RgbKeyboardStatus;
										monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex5();
										rgbKeyboardStatus14.ManualIndex5 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
										RGBKeyboardStauts rgbKeyboardStatus15 = _RgbKeyboardStatus;
										monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex6();
										rgbKeyboardStatus15.ManualIndex6 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
										RGBKeyboardStauts rgbKeyboardStatus16 = _RgbKeyboardStatus;
										monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualInterval();
										rgbKeyboardStatus16.ManualInterval = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
										RGBKeyboardStauts rgbKeyboardStatus17 = _RgbKeyboardStatus;
										monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetBreathingIndex();
										rgbKeyboardStatus17.BreathingIndex = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
										_RgbKeyboardStatus.DefaultData = CS$<>8__locals87.m_hidkeyboad.GetDefaultData();
										_RgbKeyboardStatus.color = CS$<>8__locals87.m_hidkeyboad.GetLayoutColor();
										_RgbKeyboardStatus.currentPowerMode = CS$<>8__locals87.m_hidkeyboad?.CurrentPowerMode.ToString();
									}
									goto IL_0b09;
								}
								awaiter = <>u__2;
								<>u__2 = default;
								num = (<>1__state = -1);
								goto IL_07d3;
							}
							awaiter = <>u__2;
							<>u__2 = default;
							num = (<>1__state = -1);
							goto IL_066f;
							IL_066f:
							result2 = awaiter.GetResult();
							<>7__wrap1.rkgcolor = result2;
							<>7__wrap1 = null;
							if (_RgbKeyboardStatus.rkgcolor == null)
							{
								RGBKB_Color rGBKB_Color = new RGBKB_Color
								{
									ColorBlocks = 7u,
									isCircular = true
								};
								List<RGB_S> list = new List<RGB_S>();
								int num2 = 0;
								List<Color>.Enumerator enumerator = CS$<>8__locals87.DefalutColorList.GetEnumerator();
								try
								{
									while (enumerator.MoveNext())
									{
										Color current = enumerator.Current;
										list.Add(new RGB_S
										{
											ID = (uint)num2,
											R = ResolvedDelegates.System_Drawing_Color_get_R__61(ref current),
											G = ResolvedDelegates.System_Drawing_Color_get_G__62(ref current),
											B = ResolvedDelegates.System_Drawing_Color_get_B__63(ref current)
										});
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
								rGBKB_Color.ColorBuffer = list.ToArray();
								<>7__wrap1 = _RgbKeyboardStatus;
								awaiter = Json.StringifyAsync(rGBKB_Color).GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (<>1__state = 2);
									<>u__2 = awaiter;
									<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
									return;
								}
								goto IL_07d3;
							}
							goto IL_0b09;
							IL_0b09:
							m_MQTTClient.Publish("Keyboard/Status", _RgbKeyboardStatus);
							goto end_IL_034c;
							IL_07d3:
							result2 = awaiter.GetResult();
							<>7__wrap1.rkgcolor = result2;
							<>7__wrap1 = null;
							goto IL_0b09;
							end_IL_034c:;
						}
						catch
						{
						}
						break;
					case 3:
						awaiter = <>u__2;
						<>u__2 = default;
						num = (<>1__state = -1);
						goto IL_1481;
					case 4:
						{
							awaiter = <>u__2;
							<>u__2 = default;
							num = (<>1__state = -1);
							goto IL_15e5;
						}
						IL_0be7:
						switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)text, 0))
						{
						case 'S':
							if (!(text == "SetLightingLevel"))
							{
								break;
							}
							if (CS$<>8__locals87.m_hidkeyboad is _2p1ndKeyboard_KC || CS$<>8__locals87.m_hidkeyboad is _2p1ndKeyboard_QC)
							{
								if (<>o__40.<>p__14 == null)
								{
									<>o__40.<>p__14 = CallSite<Action<CallSite, RGBKeyboard, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "RGBKeyboard_SetEffectLight_ACDC", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								<>o__40.<>p__14.Target(<>o__40.<>p__14, CS$<>8__locals87, result);
							}
							else
							{
								if (<>o__40.<>p__15 == null)
								{
									<>o__40.<>p__15 = CallSite<Action<CallSite, RGBKeyboard, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "RGBKeyboard_SetEffectLight", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								<>o__40.<>p__15.Target(<>o__40.<>p__15, CS$<>8__locals87, result);
							}
							break;
						case 'G':
							if (text == "GetLightingLevel")
							{
							}
							break;
						}
						break;
						IL_1923:
						if (CS$<>8__locals87.m_hidkeyboad != null)
						{
							m_MQTTClient.Publish("Keyboard/Status", _RgbKeyboardStatus);
						}
						break;
						IL_00ae:
						result = awaiter2.GetResult();
						if (<>o__40.<>p__2 == null)
						{
							<>o__40.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
						}
						target = <>o__40.<>p__2.Target;
						arg = <>o__40.<>p__2;
						if (<>o__40.<>p__1 == null)
						{
							<>o__40.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						target2 = <>o__40.<>p__1.Target;
						arg2 = <>o__40.<>p__1;
						arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
						if (<>o__40.<>p__0 == null)
						{
							<>o__40.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						text = target(arg, target2(arg2, arg3, <>o__40.<>p__0.Target(<>o__40.<>p__0, result, "function")));
						if (<>o__40.<>p__5 == null)
						{
							<>o__40.<>p__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
						}
						target3 = <>o__40.<>p__5.Target;
						arg4 = <>o__40.<>p__5;
						if (<>o__40.<>p__4 == null)
						{
							<>o__40.<>p__4 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						target4 = <>o__40.<>p__4.Target;
						arg5 = <>o__40.<>p__4;
						arg6 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
						if (<>o__40.<>p__3 == null)
						{
							<>o__40.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						text2 = target3(arg4, target4(arg5, arg6, <>o__40.<>p__3.Target(<>o__40.<>p__3, result, "Action")));
						if (text2 == null || !ResolvedDelegates.System_String_op_Equality__18(text2, "GETSTATUS"))
						{
							if (text == null || text == null)
							{
								break;
							}
							switch (ResolvedDelegates.System_String_get_Length__298(ref *(string*)text))
							{
							case 9:
								goto IL_0b97;
							case 8:
								goto IL_0bbc;
							case 16:
								goto IL_0be7;
							case 4:
								if (ResolvedDelegates.System_String_op_Equality__18(text, "Init"))
								{
									CS$<>8__locals87.RGBKeyboard_Init();
								}
								break;
							case 6:
								if (ResolvedDelegates.System_String_op_Equality__18(text, "DeInit"))
								{
									RGBKeyboard_DeInit();
								}
								break;
							case 14:
								if (ResolvedDelegates.System_String_op_Equality__18(text, "SetPowerSaving"))
								{
								}
								break;
							case 12:
								if (ResolvedDelegates.System_String_op_Equality__18(text, "SetEffectALL"))
								{
									if (<>o__40.<>p__12 == null)
									{
										<>o__40.<>p__12 = CallSite<Action<CallSite, RGBKeyboard, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "RGBKeyboard_SetEffectALL", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									<>o__40.<>p__12.Target(<>o__40.<>p__12, CS$<>8__locals87, result);
								}
								break;
							case 18:
								if (ResolvedDelegates.System_String_op_Equality__18(text, "GetFirmwareVersion"))
								{
								}
								break;
							case 17:
								if (ResolvedDelegates.System_String_op_Equality__18(text, "SaveLightingLevel"))
								{
									if (<>o__40.<>p__16 == null)
									{
										<>o__40.<>p__16 = CallSite<Action<CallSite, RGBKeyboard, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "RGBKeyboard_SaveLightingLevel", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									<>o__40.<>p__16.Target(<>o__40.<>p__16, CS$<>8__locals87, result);
								}
								break;
							case 5:
							case 7:
							case 10:
							case 11:
							case 13:
							case 15:
								break;
							}
							break;
						}
						goto case 1;
						IL_0b97:
						switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)text, 0))
						{
						case 'S':
						{
							if (!(text == "SetAurora"))
							{
								break;
							}
							if (<>o__40.<>p__8 == null)
							{
								<>o__40.<>p__8 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(bool).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
							}
							Func<CallSite, object, bool> target5 = <>o__40.<>p__8.Target;
							CallSite<Func<CallSite, object, bool>> arg7 = <>o__40.<>p__8;
							if (<>o__40.<>p__7 == null)
							{
								<>o__40.<>p__7 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToBoolean", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, Type, object, object> target6 = <>o__40.<>p__7.Target;
							CallSite<Func<CallSite, Type, object, object>> arg8 = <>o__40.<>p__7;
							Type arg9 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
							if (<>o__40.<>p__6 == null)
							{
								<>o__40.<>p__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							bool flag = target5(arg7, target6(arg8, arg9, <>o__40.<>p__6.Target(<>o__40.<>p__6, result, "AuroraSwitch")));
							if (!ResolvedDelegates.System_Threading_Monitor_TryEnter__145(CS$<>8__locals87.AuroraSwitchLock, 1000))
							{
								break;
							}
							try
							{
								if (!flag)
								{
									if (CS$<>8__locals87.m_hidkeyboad != null)
									{
										CS$<>8__locals87.aurora.SetAuroraSwtich(flag);
										ResolvedDelegates.System_Threading_Thread_Sleep__6(250);
										CS$<>8__locals87.m_hidkeyboad.RunEffct(0, 2);
									}
								}
								else
								{
									CS$<>8__locals87.m_hidkeyboad?._KeyboardControl?.StopMusicTransfer();
									CS$<>8__locals87.m_hidkeyboad?._KeyboardControl?.Set_ITE_Effect_Type_ApMode_Stop();
									Thread.Sleep(250);
									CS$<>8__locals87.aurora.SetAuroraSwtich(flag);
								}
							}
							catch
							{
							}
							finally
							{
								if (num < 0)
								{
									ResolvedDelegates.System_Threading_Monitor_Exit__71(CS$<>8__locals87.AuroraSwitchLock);
								}
							}
							break;
						}
						case 'G':
							if (text == "GetEffect")
							{
							}
							break;
						}
						break;
						IL_0bbc:
						c = ResolvedDelegates.System_String_get_Chars__334(ref *(string*)text, 3);
						if (c != 'C')
						{
							if (c != 'P' || !(text == "SetPower"))
							{
								break;
							}
							if (<>o__40.<>p__11 == null)
							{
								<>o__40.<>p__11 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
							}
							Func<CallSite, object, string> target7 = <>o__40.<>p__11.Target;
							CallSite<Func<CallSite, object, string>> arg10 = <>o__40.<>p__11;
							if (<>o__40.<>p__9 == null)
							{
								<>o__40.<>p__9 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							object obj3 = <>o__40.<>p__9.Target(<>o__40.<>p__9, result, "powerstatus");
							object arg11;
							if (obj3 == null)
							{
								arg11 = null;
							}
							else
							{
								if (<>o__40.<>p__10 == null)
								{
									<>o__40.<>p__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								arg11 = <>o__40.<>p__10.Target(<>o__40.<>p__10, obj3);
							}
							string text3 = target7(arg10, arg11);
							RGBKB_PowerStatus rGBKB_PowerStatus = (RGBKB_PowerStatus)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_PowerStatus).TypeHandle), text3);
							if (!MyEcCtrl.Instance.IsChinaModeOn())
							{
								byte Data = 0;
								MyEcCtrl.Instance.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(CS$<>8__locals87)), 1922, ref Data);
								Data |= 0x40;
								MyEcCtrl.Instance.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(CS$<>8__locals87)), 1922, Data);
							}
							if (rGBKB_PowerStatus != RGBKB_PowerStatus.On)
							{
								Task task = ResolvedDelegates.System_Threading_Tasks_Task_Run__10(() =>
								{
									CS$<>8__locals87.m_hidkeyboad?.CloseAnimation();
								});
								ResolvedDelegates.System_Threading_Tasks_Task_WaitAll__345(new Task[1] { task });
								CS$<>8__locals87.m_hidkeyboad?.SetPowerStatus(rGBKB_PowerStatus, 0, "");
							}
							else
							{
								CS$<>8__locals87.m_hidkeyboad?.SetPowerStatus(rGBKB_PowerStatus, 0, "");
							}
							if (m_LM.m_KB_Type != RGBKB_Type.SingleZone)
							{
								_RgbKeyboardStatus.solution = m_LM.m_KB_Solution.ToString();
								_RgbKeyboardStatus.type = m_LM.m_KB_Type.ToString();
								RGBKeyboardStauts rgbKeyboardStatus18 = _RgbKeyboardStatus;
								HIDKeyboard hidkeyboad9 = CS$<>8__locals87.m_hidkeyboad;
								object brightNess3;
								if (hidkeyboad9 == null)
								{
									brightNess3 = null;
								}
								else
								{
									uint brightness = hidkeyboad9.GetBrightness();
									brightNess3 = ResolvedDelegates.System_UInt32_ToString__78(ref brightness);
								}
								rgbKeyboardStatus18.brightNess = (string)brightNess3;
								_RgbKeyboardStatus.powerStatus = CS$<>8__locals87.m_hidkeyboad?.GetPowerStatus().ToString();
								_RgbKeyboardStatus.welcomeStatus = CS$<>8__locals87.m_hidkeyboad?.GetWelcomeStatus();
								_RgbKeyboardStatus.welcomeDirectionStatus = CS$<>8__locals87.m_hidkeyboad?.GetWelcomeDirectionStatus();
								_RgbKeyboardStatus.auroraStatus = CS$<>8__locals87.aurora.GetAuroraStauts();
								RGBKeyboardStauts rgbKeyboardStatus19 = _RgbKeyboardStatus;
								HIDKeyboard hidkeyboad10 = CS$<>8__locals87.m_hidkeyboad;
								object effect3;
								if (hidkeyboad10 == null)
								{
									effect3 = null;
								}
								else
								{
									SAVE_LIGHTING_EFFECT_DATA allKeyboardStatus = hidkeyboad10.GetAllKeyboardStatus();
									effect3 = ResolvedDelegates.System_Byte_ToString__119(ref allKeyboardStatus.save_effect);
								}
								rgbKeyboardStatus19.effect = (string)effect3;
								_RgbKeyboardStatus.powerstatus = ((CS$<>8__locals87.m_hidkeyboad?.GetPowerStatus().ToString() == "On") ? 1 : 0);
								RGBKeyboardStauts rgbKeyboardStatus20 = _RgbKeyboardStatus;
								HIDKeyboard hidkeyboad11 = CS$<>8__locals87.m_hidkeyboad;
								object light2;
								if (hidkeyboad11 == null)
								{
									light2 = null;
								}
								else
								{
									uint brightness = hidkeyboad11.GetBrightness();
									light2 = ResolvedDelegates.System_UInt32_ToString__78(ref brightness);
								}
								rgbKeyboardStatus20.light = (string)light2;
								RGBKeyboardStauts rgbKeyboardStatus21 = _RgbKeyboardStatus;
								HIDKeyboard hidkeyboad12 = CS$<>8__locals87.m_hidkeyboad;
								object speed3;
								if (hidkeyboad12 == null)
								{
									speed3 = null;
								}
								else
								{
									SAVE_LIGHTING_EFFECT_DATA allKeyboardStatus = hidkeyboad12.GetAllKeyboardStatus();
									speed3 = ResolvedDelegates.System_Byte_ToString__119(ref allKeyboardStatus.save_speed);
								}
								rgbKeyboardStatus21.speed = (string)speed3;
								RGBKeyboardStauts rgbKeyboardStatus22 = _RgbKeyboardStatus;
								HIDKeyboard hidkeyboad13 = CS$<>8__locals87.m_hidkeyboad;
								object direction2;
								if (hidkeyboad13 == null)
								{
									direction2 = null;
								}
								else
								{
									SAVE_LIGHTING_EFFECT_DATA allKeyboardStatus = hidkeyboad13.GetAllKeyboardStatus();
									direction2 = ResolvedDelegates.System_Byte_ToString__119(ref allKeyboardStatus.save_direction);
								}
								rgbKeyboardStatus22.direction = (string)direction2;
								<>7__wrap1 = _RgbKeyboardStatus;
								awaiter = Json.StringifyAsync(CS$<>8__locals87.m_hidkeyboad?.GetAllKeyboardStatus().save_layout_color).GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (<>1__state = 3);
									<>u__2 = awaiter;
									<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
									return;
								}
								goto IL_1481;
							}
							if (m_LM.m_KB_Type == RGBKB_Type.SingleZone && CS$<>8__locals87.m_hidkeyboad != null)
							{
								RGBKeyboardStauts rgbKeyboardStatus23 = _RgbKeyboardStatus;
								HIDKeyboard hidkeyboad14 = CS$<>8__locals87.m_hidkeyboad;
								object effect4;
								if (hidkeyboad14 == null)
								{
									effect4 = null;
								}
								else
								{
									SAVE_LIGHTING_EFFECT_DATA allKeyboardStatus = hidkeyboad14.GetAllKeyboardStatus();
									effect4 = ResolvedDelegates.System_Byte_ToString__119(ref allKeyboardStatus.save_effect);
								}
								rgbKeyboardStatus23.effect = (string)effect4;
								_RgbKeyboardStatus.color = CS$<>8__locals87.m_hidkeyboad.GetLayoutColor();
								RGBKeyboardStauts rgbKeyboardStatus24 = _RgbKeyboardStatus;
								HIDKeyboard hidkeyboad15 = CS$<>8__locals87.m_hidkeyboad;
								object speed4;
								if (hidkeyboad15 == null)
								{
									speed4 = null;
								}
								else
								{
									SAVE_LIGHTING_EFFECT_DATA allKeyboardStatus = hidkeyboad15.GetAllKeyboardStatus();
									speed4 = ResolvedDelegates.System_Byte_ToString__119(ref allKeyboardStatus.save_speed);
								}
								rgbKeyboardStatus24.speed = (string)speed4;
								_RgbKeyboardStatus.powerStatus = CS$<>8__locals87.m_hidkeyboad?.GetPowerStatus().ToString();
								_RgbKeyboardStatus.solution = m_LM.m_KB_Solution.ToString();
								_RgbKeyboardStatus.type = m_LM.m_KB_Type.ToString();
								_RgbKeyboardStatus.SingleZonePowerSupport = CS$<>8__locals87.m_hidkeyboad.GetSingleZonePowerSupport();
								RGBKeyboardStauts rgbKeyboardStatus25 = _RgbKeyboardStatus;
								HIDKeyboard hidkeyboad16 = CS$<>8__locals87.m_hidkeyboad;
								object brightNess4;
								if (hidkeyboad16 == null)
								{
									brightNess4 = null;
								}
								else
								{
									uint brightness = hidkeyboad16.GetBrightness();
									brightNess4 = ResolvedDelegates.System_UInt32_ToString__78(ref brightness);
								}
								rgbKeyboardStatus25.brightNess = (string)brightNess4;
								RGBKeyboardStauts rgbKeyboardStatus26 = _RgbKeyboardStatus;
								int monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetMonochromeIndex();
								rgbKeyboardStatus26.MonochromeIndex = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
								RGBKeyboardStauts rgbKeyboardStatus27 = _RgbKeyboardStatus;
								monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex1();
								rgbKeyboardStatus27.ManualIndex1 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
								RGBKeyboardStauts rgbKeyboardStatus28 = _RgbKeyboardStatus;
								monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex2();
								rgbKeyboardStatus28.ManualIndex2 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
								RGBKeyboardStauts rgbKeyboardStatus29 = _RgbKeyboardStatus;
								monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex3();
								rgbKeyboardStatus29.ManualIndex3 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
								RGBKeyboardStauts rgbKeyboardStatus30 = _RgbKeyboardStatus;
								monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex4();
								rgbKeyboardStatus30.ManualIndex4 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
								RGBKeyboardStauts rgbKeyboardStatus31 = _RgbKeyboardStatus;
								monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex5();
								rgbKeyboardStatus31.ManualIndex5 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
								RGBKeyboardStauts rgbKeyboardStatus32 = _RgbKeyboardStatus;
								monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualIndex6();
								rgbKeyboardStatus32.ManualIndex6 = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
								RGBKeyboardStauts rgbKeyboardStatus33 = _RgbKeyboardStatus;
								monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetManualInterval();
								rgbKeyboardStatus33.ManualInterval = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
								RGBKeyboardStauts rgbKeyboardStatus34 = _RgbKeyboardStatus;
								monochromeIndex = CS$<>8__locals87.m_hidkeyboad.GetBreathingIndex();
								rgbKeyboardStatus34.BreathingIndex = ResolvedDelegates.System_Int32_ToString__55(ref monochromeIndex);
								_RgbKeyboardStatus.DefaultData = CS$<>8__locals87.m_hidkeyboad.GetDefaultData();
								_RgbKeyboardStatus.currentPowerMode = CS$<>8__locals87.m_hidkeyboad?.CurrentPowerMode.ToString();
							}
							goto IL_1923;
						}
						if (text == "SetColor")
						{
							if (<>o__40.<>p__13 == null)
							{
								<>o__40.<>p__13 = CallSite<Action<CallSite, RGBKeyboard, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "RGBKeyboard_SetColor", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							<>o__40.<>p__13.Target(<>o__40.<>p__13, CS$<>8__locals87, result);
						}
						break;
						IL_15e5:
						result2 = awaiter.GetResult();
						<>7__wrap1.rkgcolor = result2;
						<>7__wrap1 = null;
						_RgbKeyboardStatus.currentPowerMode = CS$<>8__locals87.m_hidkeyboad?.CurrentPowerMode.ToString();
						goto IL_1923;
						IL_1481:
						result2 = awaiter.GetResult();
						<>7__wrap1.rkgcolor = result2;
						<>7__wrap1 = null;
						if (_RgbKeyboardStatus.rkgcolor == null)
						{
							RGBKB_Color rGBKB_Color2 = new RGBKB_Color
							{
								ColorBlocks = 7u,
								isCircular = true
							};
							List<RGB_S> list2 = new List<RGB_S>();
							int num3 = 0;
							List<Color>.Enumerator enumerator = CS$<>8__locals87.DefalutColorList.GetEnumerator();
							try
							{
								while (enumerator.MoveNext())
								{
									Color current2 = enumerator.Current;
									list2.Add(new RGB_S
									{
										ID = (uint)num3,
										R = ResolvedDelegates.System_Drawing_Color_get_R__61(ref current2),
										G = ResolvedDelegates.System_Drawing_Color_get_G__62(ref current2),
										B = ResolvedDelegates.System_Drawing_Color_get_B__63(ref current2)
									});
									num3++;
								}
							}
							finally
							{
								if (num < 0)
								{
									((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
								}
							}
							rGBKB_Color2.ColorBuffer = list2.ToArray();
							<>7__wrap1 = _RgbKeyboardStatus;
							awaiter = Json.StringifyAsync(rGBKB_Color2).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (<>1__state = 4);
								<>u__2 = awaiter;
								<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
							goto IL_15e5;
						}
						goto IL_1923;
					}
				}
				catch (Exception ex)
				{
					LogCtrl.TraceMessage("exception: " + ex, "OnMqttMessage", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\RGBKeyboard.cs", 1123);
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

	public static LM_Manager m_LM;

	public static MqttClientCtrl m_MQTTClient = null;

	public const string KeyboardTOPIC = "Keyboard/Ctrl";

	public const string Keyboard_Status = "Keyboard/Status";

	private PowerStatus ps = ResolvedDelegates.System_Windows_Forms_SystemInformation_get_PowerStatus__340();

	private HIDKeyboard m_hidkeyboad;

	private object _powerLock = new object();

	private static readonly RGBKeyboard clientService = new RGBKeyboard();

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	private static RGBKeyboardStauts _RgbKeyboardStatus;

	[CompilerGenerated]
	private EventHandler m_HIDLightbarInitedEvent;

	private CheckAurora aurora = CheckAurora.Instance;

	private static string BiosPprojectID;

	private static int m_CustomizeTarget = 1;

	internal const ushort EC_ChinaMode_Change = 182;

	internal List<Color> DefalutColorList = new List<Color>
	{
		ResolvedDelegates.System_Drawing_Color_FromArgb__133(255, 255, 0, 0),
		ResolvedDelegates.System_Drawing_Color_FromArgb__133(255, 255, 165, 0),
		ResolvedDelegates.System_Drawing_Color_FromArgb__133(255, 255, 255, 0),
		ResolvedDelegates.System_Drawing_Color_FromArgb__133(255, 0, 255, 0),
		ResolvedDelegates.System_Drawing_Color_FromArgb__133(255, 0, 22, 255),
		ResolvedDelegates.System_Drawing_Color_FromArgb__133(255, 0, 0, 255),
		ResolvedDelegates.System_Drawing_Color_FromArgb__133(255, 139, 0, 255)
	};

	private object AuroraSwitchLock = new object();

	private object suspendLock = new object();

	public static RGBKeyboard Instance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return clientService;
		}
	}

	public event EventHandler HIDLightbarInitedEvent
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		[CompilerGenerated]
		add
		{
			EventHandler eventHandler = this.m_HIDLightbarInitedEvent;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)ResolvedDelegates.System_Delegate_Combine__130(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_HIDLightbarInitedEvent, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		[CompilerGenerated]
		remove
		{
			EventHandler eventHandler = this.m_HIDLightbarInitedEvent;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)ResolvedDelegates.System_Delegate_Remove__131(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_HIDLightbarInitedEvent, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public RGBKeyboard(string DefaultTool, bool IsAppExists)
	{
		ResolvedDelegates.System_Object_ctor__0(this);
		RGBKeyboard_Init(DefaultTool, IsAppExists);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public HIDKeyboard GetHIDKeyboard()
	{
		return m_hidkeyboad;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Dispose()
	{
		m_LM?.LM_DeInit();
		RGBKeyboard_DeInit();
		m_hidkeyboad = null;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static int GetProjectID()
	{
		byte Data = 0;
		MyEcCtrl.Instance.Read("RGBKeyboard", 1856, ref Data);
		return (byte)ResolvedDelegates.System_Convert_ToUInt64__129(Data);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private RGBKeyboard()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
		_RgbKeyboardStatus = new RGBKeyboardStauts();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		if ((int)e.Mode == 2)
		{
			if ((int)ResolvedDelegates.System_Windows_Forms_PowerStatus_get_PowerLineStatus__341(ps) == 0)
			{
				m_hidkeyboad?.UnPlugged();
			}
			if ((int)ps.PowerLineStatus == 1)
			{
				m_hidkeyboad?.Plugged();
			}
			KeyboardCloseTimer.CreateInstance.RestartCloseTimer();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static RGBKB_Color ConvertJsonRGB2RGBColor(dynamic color)
	{
		if (<>o__24.<>p__2 == null)
		{
			<>o__24.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(bool).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
		}
		Func<CallSite, object, bool> target = <>o__24.<>p__2.Target;
		CallSite<Func<CallSite, object, bool>> arg = <>o__24.<>p__2;
		if (<>o__24.<>p__1 == null)
		{
			<>o__24.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToBoolean", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target2 = <>o__24.<>p__1.Target;
		CallSite<Func<CallSite, Type, object, object>> arg2 = <>o__24.<>p__1;
		Type arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__24.<>p__0 == null)
		{
			<>o__24.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		bool bCircular = target(arg, target2(arg2, arg3, <>o__24.<>p__0.Target(<>o__24.<>p__0, (object)color, "isCircular")));
		if (<>o__24.<>p__5 == null)
		{
			<>o__24.<>p__5 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
		}
		Func<CallSite, object, uint> target3 = <>o__24.<>p__5.Target;
		CallSite<Func<CallSite, object, uint>> arg4 = <>o__24.<>p__5;
		if (<>o__24.<>p__4 == null)
		{
			<>o__24.<>p__4 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target4 = <>o__24.<>p__4.Target;
		CallSite<Func<CallSite, Type, object, object>> arg5 = <>o__24.<>p__4;
		Type arg6 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__24.<>p__3 == null)
		{
			<>o__24.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		uint num = target3(arg4, target4(arg5, arg6, <>o__24.<>p__3.Target(<>o__24.<>p__3, (object)color, "ColorBlocks")));
		RGBKB_Color result = new RGBKB_Color(bCircular, num);
		for (int i = 0; i < num; i++)
		{
			ref RGB_S reference = ref result.ColorBuffer[i];
			if (<>o__24.<>p__10 == null)
			{
				<>o__24.<>p__10 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
			}
			Func<CallSite, object, uint> target5 = <>o__24.<>p__10.Target;
			CallSite<Func<CallSite, object, uint>> arg7 = <>o__24.<>p__10;
			if (<>o__24.<>p__9 == null)
			{
				<>o__24.<>p__9 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target6 = <>o__24.<>p__9.Target;
			CallSite<Func<CallSite, Type, object, object>> arg8 = <>o__24.<>p__9;
			Type arg9 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__24.<>p__8 == null)
			{
				<>o__24.<>p__8 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, string, object> target7 = <>o__24.<>p__8.Target;
			CallSite<Func<CallSite, object, string, object>> arg10 = <>o__24.<>p__8;
			if (<>o__24.<>p__7 == null)
			{
				<>o__24.<>p__7 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			Func<CallSite, object, int, object> target8 = <>o__24.<>p__7.Target;
			CallSite<Func<CallSite, object, int, object>> arg11 = <>o__24.<>p__7;
			if (<>o__24.<>p__6 == null)
			{
				<>o__24.<>p__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference.ID = target5(arg7, target6(arg8, arg9, target7(arg10, target8(arg11, <>o__24.<>p__6.Target(<>o__24.<>p__6, (object)color, "ColorBuffer"), i), "ID")));
			ref RGB_S reference2 = ref result.ColorBuffer[i];
			if (<>o__24.<>p__15 == null)
			{
				<>o__24.<>p__15 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
			}
			Func<CallSite, object, byte> target9 = <>o__24.<>p__15.Target;
			CallSite<Func<CallSite, object, byte>> arg12 = <>o__24.<>p__15;
			if (<>o__24.<>p__14 == null)
			{
				<>o__24.<>p__14 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target10 = <>o__24.<>p__14.Target;
			CallSite<Func<CallSite, Type, object, object>> arg13 = <>o__24.<>p__14;
			Type arg14 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__24.<>p__13 == null)
			{
				<>o__24.<>p__13 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, string, object> target11 = <>o__24.<>p__13.Target;
			CallSite<Func<CallSite, object, string, object>> arg15 = <>o__24.<>p__13;
			if (<>o__24.<>p__12 == null)
			{
				<>o__24.<>p__12 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			Func<CallSite, object, int, object> target12 = <>o__24.<>p__12.Target;
			CallSite<Func<CallSite, object, int, object>> arg16 = <>o__24.<>p__12;
			if (<>o__24.<>p__11 == null)
			{
				<>o__24.<>p__11 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference2.R = target9(arg12, target10(arg13, arg14, target11(arg15, target12(arg16, <>o__24.<>p__11.Target(<>o__24.<>p__11, (object)color, "ColorBuffer"), i), "R")));
			ref RGB_S reference3 = ref result.ColorBuffer[i];
			if (<>o__24.<>p__20 == null)
			{
				<>o__24.<>p__20 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
			}
			Func<CallSite, object, byte> target13 = <>o__24.<>p__20.Target;
			CallSite<Func<CallSite, object, byte>> arg17 = <>o__24.<>p__20;
			if (<>o__24.<>p__19 == null)
			{
				<>o__24.<>p__19 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target14 = <>o__24.<>p__19.Target;
			CallSite<Func<CallSite, Type, object, object>> arg18 = <>o__24.<>p__19;
			Type arg19 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__24.<>p__18 == null)
			{
				<>o__24.<>p__18 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, string, object> target15 = <>o__24.<>p__18.Target;
			CallSite<Func<CallSite, object, string, object>> arg20 = <>o__24.<>p__18;
			if (<>o__24.<>p__17 == null)
			{
				<>o__24.<>p__17 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			Func<CallSite, object, int, object> target16 = <>o__24.<>p__17.Target;
			CallSite<Func<CallSite, object, int, object>> arg21 = <>o__24.<>p__17;
			if (<>o__24.<>p__16 == null)
			{
				<>o__24.<>p__16 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference3.G = target13(arg17, target14(arg18, arg19, target15(arg20, target16(arg21, <>o__24.<>p__16.Target(<>o__24.<>p__16, (object)color, "ColorBuffer"), i), "G")));
			ref RGB_S reference4 = ref result.ColorBuffer[i];
			if (<>o__24.<>p__25 == null)
			{
				<>o__24.<>p__25 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
			}
			Func<CallSite, object, byte> target17 = <>o__24.<>p__25.Target;
			CallSite<Func<CallSite, object, byte>> arg22 = <>o__24.<>p__25;
			if (<>o__24.<>p__24 == null)
			{
				<>o__24.<>p__24 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target18 = <>o__24.<>p__24.Target;
			CallSite<Func<CallSite, Type, object, object>> arg23 = <>o__24.<>p__24;
			Type arg24 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__24.<>p__23 == null)
			{
				<>o__24.<>p__23 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, string, object> target19 = <>o__24.<>p__23.Target;
			CallSite<Func<CallSite, object, string, object>> arg25 = <>o__24.<>p__23;
			if (<>o__24.<>p__22 == null)
			{
				<>o__24.<>p__22 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			Func<CallSite, object, int, object> target20 = <>o__24.<>p__22.Target;
			CallSite<Func<CallSite, object, int, object>> arg26 = <>o__24.<>p__22;
			if (<>o__24.<>p__21 == null)
			{
				<>o__24.<>p__21 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference4.B = target17(arg22, target18(arg23, arg24, target19(arg25, target20(arg26, <>o__24.<>p__21.Target(<>o__24.<>p__21, (object)color, "ColorBuffer"), i), "B")));
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Event_LM(RGBKB_Event_Data event_data)
	{
		<>c__DisplayClass25_0 CS$<>8__locals19 = new <>c__DisplayClass25_0();
		CS$<>8__locals19.<>4__this = this;
		CS$<>8__locals19.event_data = event_data;
		if (CS$<>8__locals19.event_data.event_id != RGBKB_EventID.Brightness_update)
		{
			return;
		}
		CS$<>8__locals19.brightness = CS$<>8__locals19.event_data.event_data[0];
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			_ = new
			{
				MqttID = "KeyboardUserId",
				function = "UpdateBrightness",
				level = CS$<>8__locals19.brightness
			};
			if (CS$<>8__locals19.<>4__this.m_hidkeyboad != null)
			{
				_RgbKeyboardStatus.solution = m_LM.m_KB_Solution.ToString();
				_RgbKeyboardStatus.type = m_LM.m_KB_Type.ToString();
				_RgbKeyboardStatus.powerStatus = CS$<>8__locals19.<>4__this.m_hidkeyboad?.GetPowerStatus().ToString();
				RGBKeyboardStauts rgbKeyboardStatus = _RgbKeyboardStatus;
				HIDKeyboard hidkeyboad = CS$<>8__locals19.<>4__this.m_hidkeyboad;
				object brightNess;
				if (hidkeyboad == null)
				{
					brightNess = null;
				}
				else
				{
					uint brightness = hidkeyboad.GetBrightness();
					brightNess = ResolvedDelegates.System_UInt32_ToString__78(ref brightness);
				}
				rgbKeyboardStatus.brightNess = (string)brightNess;
				_RgbKeyboardStatus.ACBrightness = CS$<>8__locals19.<>4__this.m_hidkeyboad?._AC_LightLevel.ToString();
				_RgbKeyboardStatus.DCBrightness = CS$<>8__locals19.<>4__this.m_hidkeyboad?._DC_LightLevel.ToString();
				_RgbKeyboardStatus.welcomeStatus = CS$<>8__locals19.<>4__this.m_hidkeyboad?.GetWelcomeStatus();
				_RgbKeyboardStatus.welcomeDirectionStatus = CS$<>8__locals19.<>4__this.m_hidkeyboad?.GetWelcomeDirectionStatus();
				_RgbKeyboardStatus.auroraStatus = CS$<>8__locals19.<>4__this.aurora.GetAuroraStauts();
				_RgbKeyboardStatus.currentPowerMode = CS$<>8__locals19.<>4__this.m_hidkeyboad?.CurrentPowerMode.ToString();
				m_MQTTClient.Publish("Keyboard/Status", _RgbKeyboardStatus);
			}
			if (CS$<>8__locals19.event_data.envet_data_len == 1)
			{
				LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("RGBKeyboard|Event_LM : UpdateStatusToOSD level: ", ResolvedDelegates.System_Int32_ToString__55(ref CS$<>8__locals19.brightness)));
				CS$<>8__locals19.<>4__this.UpdateStatusToOSD(CS$<>8__locals19.brightness);
			}
			else
			{
				LogCtrl.Write("Only update UI");
			}
		}), Array.Empty<object>());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void UpdateStatusToOSD(int BL_Level)
	{
		string function = "KeyBoardBrghtiness";
		string text = "";
		string img_name;
		switch (BL_Level)
		{
		case 0:
			img_name = "OSD_KB_Light_4-01.png";
			text = "0";
			break;
		case 1:
			img_name = "OSD_KB_Light_4-02.png";
			text = "25";
			break;
		case 2:
			img_name = "OSD_KB_Light_4-03.png";
			text = "50";
			break;
		case 3:
			img_name = "OSD_KB_Light_4-04.png";
			text = "75";
			break;
		case 4:
			img_name = "OSD_KB_Light_4-05.png";
			text = "100";
			break;
		default:
			img_name = "OSD_KB_Light_4-01.png";
			text = "0";
			break;
		}
		if (m_hidkeyboad is _2p1ndKeyboard_KC)
		{
			var data = new
			{
				Function = function,
				Level = text
			};
			m_MQTTClient.Publish("OSD/Status", data, retain: false);
		}
		else
		{
			App.m_Osd.ShowBLOSD(img_name);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RGBKeyboard_Init(string DefaulTool, bool IsAppExists)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		LogCtrl.Write("RGBKeyboard | RGBKeyboard_Init");
		if (m_LM != null)
		{
			return;
		}
		m_LM = new LM_Manager();
		if (m_LM.LM_Init(DefaulTool))
		{
			m_hidkeyboad = HIDKeyboardFactory.CreateHIDDevice(m_LM);
			if ((int)ResolvedDelegates.System_Windows_Forms_PowerStatus_get_PowerLineStatus__341(ps) == 0)
			{
				m_hidkeyboad?.InitalizeWelcomeKeyboardEffect(RGBKB_PowerStatus.On, IsAppExists, unplug: true);
				Thread.Sleep(2000);
				m_hidkeyboad?.InitalizeKeyboardEffect(unplug: true);
			}
			else
			{
				m_hidkeyboad?.InitalizeWelcomeKeyboardEffect(RGBKB_PowerStatus.On, IsAppExists);
				Thread.Sleep(2000);
				m_hidkeyboad?.InitalizeKeyboardEffect();
			}
		}
	}

	private void AuroraStatusChanged(object auroraEnable, EventArgs e)
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RGBKeyboard_Init()
	{
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Expected Obj, but got Unknown
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Expected Obj, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected Obj, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected Obj, but got Unknown
		LogCtrl.Write("RGBKeyboard | RGBKeyboard_Init");
		CheckAurora checkAurora = aurora;
		checkAurora.AuroraStatusChanged = (ErrorEventHandler)Delegate.Combine(checkAurora.AuroraStatusChanged, new ErrorEventHandler(AuroraStatusChanged));
		if (m_LM == null)
		{
			m_LM = new LM_Manager();
			if (m_LM.LM_Init())
			{
				m_hidkeyboad = HIDKeyboardFactory.CreateHIDDevice(m_LM);
				if (ResolvedDelegates.System_String_op_Equality__18(m_LM.m_KB_Type.ToString(), RGBKB_Type.FourZone.ToString()) || ResolvedDelegates.System_String_op_Equality__18(m_LM.m_KB_Type.ToString(), RGBKB_Type.FourZoneSingleColor.ToString()))
				{
					CheckAurora.Instance.SetAuroraSwtich(enable: false);
				}
				if ((int)ps.PowerLineStatus == 0)
				{
					m_hidkeyboad?.InitalizeKeyboardEffect(unplug: true);
				}
				else
				{
					m_hidkeyboad?.InitalizeKeyboardEffect();
				}
				if (m_hidkeyboad != null)
				{
					m_hidkeyboad.m_Layout_Event_handler -= Event_LM;
					m_hidkeyboad.m_Layout_Event_handler += Event_LM;
					SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
					SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
					ScanCodeBus.OnScanCode -= ScanCode_Hnadler;
					ScanCodeBus.OnScanCode += ScanCode_Hnadler;
				}
			}
			else
			{
				CheckAurora.Instance.SetAuroraSwtich(enable: false);
			}
		}
		else
		{
			_RgbKeyboardStatus.solution = m_LM.m_KB_Solution.ToString();
			_RgbKeyboardStatus.type = m_LM.m_KB_Type.ToString();
			m_hidkeyboad = HIDKeyboardFactory.CreateHIDDevice(m_LM);
			if ((int)ResolvedDelegates.System_Windows_Forms_PowerStatus_get_PowerLineStatus__341(ps) == 0)
			{
				m_hidkeyboad?.InitalizeKeyboardEffect(unplug: true);
			}
			else
			{
				m_hidkeyboad?.InitalizeKeyboardEffect();
			}
			if (m_hidkeyboad != null)
			{
				m_hidkeyboad.m_Layout_Event_handler -= Event_LM;
				m_hidkeyboad.m_Layout_Event_handler += Event_LM;
				SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
				SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
				ScanCodeBus.OnScanCode -= ScanCode_Hnadler;
				ScanCodeBus.OnScanCode += ScanCode_Hnadler;
			}
		}
		BiosPprojectID = Convert.ToString(EcCtrl.GetBiosProjctID());
		m_CustomizeTarget = RegistryCtrl.GetCustomizeTarget();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void RGBKeyboard_DeInit()
	{
		if (m_LM != null)
		{
			m_LM.LM_DeInit();
			m_LM = null;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ScanCode_Hnadler(int scancode)
	{
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Keybaord Get Scancode : ", ResolvedDelegates.System_Int32_ToString__55(ref scancode)));
		if (!CheckAurora.Instance.GetAuroraStauts() && m_hidkeyboad != null)
		{
			m_hidkeyboad?.SetBrinessByScanCode(scancode);
			m_hidkeyboad?.SetChinaMode(scancode);
			if (scancode == 182 && !MyEcCtrl.Instance.IsChinaMode() && MyEcCtrl.Instance.IsChinaModeOn())
			{
				RGBKB_Mode mode = m_hidkeyboad?.GetMode() ?? RGBKB_Mode.Lighting;
				m_hidkeyboad?.SetMode(RGBKB_Mode.Welcome);
				m_hidkeyboad?.RunWelcomeEffect();
				m_hidkeyboad?.SetMode(mode);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RGBKeyboard_SetEffectALL(dynamic data)
	{
		if (m_LM == null)
		{
			return;
		}
		if (<>o__33.<>p__2 == null)
		{
			<>o__33.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
		}
		Func<CallSite, object, string> target = <>o__33.<>p__2.Target;
		CallSite<Func<CallSite, object, string>> arg = <>o__33.<>p__2;
		if (<>o__33.<>p__0 == null)
		{
			<>o__33.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		object obj = <>o__33.<>p__0.Target(<>o__33.<>p__0, (object)data, "mode");
		object arg2;
		if (obj == null)
		{
			arg2 = null;
		}
		else
		{
			if (<>o__33.<>p__1 == null)
			{
				<>o__33.<>p__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg2 = <>o__33.<>p__1.Target(<>o__33.<>p__1, obj);
		}
		string text = target(arg, arg2);
		RGBKB_Mode mode = (RGBKB_Mode)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Mode).TypeHandle), text);
		bool flag;
		if (mode.Equals(RGBKB_Mode.Welcome))
		{
			if (<>o__33.<>p__5 == null)
			{
				<>o__33.<>p__5 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target2 = <>o__33.<>p__5.Target;
			CallSite<Func<CallSite, object, bool>> arg3 = <>o__33.<>p__5;
			flag = m_hidkeyboad != null;
			object arg7;
			if (flag)
			{
				if (<>o__33.<>p__4 == null)
				{
					<>o__33.<>p__4 = CallSite<Func<CallSite, bool, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, bool, object, object> target3 = <>o__33.<>p__4.Target;
				CallSite<Func<CallSite, bool, object, object>> arg4 = <>o__33.<>p__4;
				bool arg5 = flag;
				HIDKeyboard hidkeyboad = m_hidkeyboad;
				object arg6;
				if (hidkeyboad == null)
				{
					arg6 = null;
				}
				else
				{
					if (<>o__33.<>p__3 == null)
					{
						<>o__33.<>p__3 = CallSite<Func<CallSite, HIDKeyboard, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "SetWelcomeEffect", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
						}));
					}
					arg6 = <>o__33.<>p__3.Target(<>o__33.<>p__3, hidkeyboad, (object)data);
				}
				arg7 = target3(arg4, arg5, arg6);
			}
			else
			{
				arg7 = flag;
			}
			if (target2(arg3, arg7))
			{
				m_hidkeyboad.SetMode(mode);
				m_hidkeyboad?.RunWelcomeEffect();
			}
			return;
		}
		if (<>o__33.<>p__8 == null)
		{
			<>o__33.<>p__8 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target4 = <>o__33.<>p__8.Target;
		CallSite<Func<CallSite, object, bool>> arg8 = <>o__33.<>p__8;
		flag = m_hidkeyboad != null;
		object arg12;
		if (flag)
		{
			if (<>o__33.<>p__7 == null)
			{
				<>o__33.<>p__7 = CallSite<Func<CallSite, bool, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, bool, object, object> target5 = <>o__33.<>p__7.Target;
			CallSite<Func<CallSite, bool, object, object>> arg9 = <>o__33.<>p__7;
			bool arg10 = flag;
			HIDKeyboard hidkeyboad = m_hidkeyboad;
			object arg11;
			if (hidkeyboad == null)
			{
				arg11 = null;
			}
			else
			{
				if (<>o__33.<>p__6 == null)
				{
					<>o__33.<>p__6 = CallSite<Func<CallSite, HIDKeyboard, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "SetEffect", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				arg11 = <>o__33.<>p__6.Target(<>o__33.<>p__6, hidkeyboad, (object)data);
			}
			arg12 = target5(arg9, arg10, arg11);
		}
		else
		{
			arg12 = flag;
		}
		if (target4(arg8, arg12))
		{
			m_hidkeyboad.SetMode(mode);
			m_hidkeyboad?.RunEffct(0, 2);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RGBKeyboard_SetEffectLight_ACDC(dynamic data)
	{
		if (<>o__34.<>p__6 == null)
		{
			<>o__34.<>p__6 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target = <>o__34.<>p__6.Target;
		CallSite<Func<CallSite, object, bool>> arg = <>o__34.<>p__6;
		if (<>o__34.<>p__1 == null)
		{
			<>o__34.<>p__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		Func<CallSite, object, object, object> target2 = <>o__34.<>p__1.Target;
		CallSite<Func<CallSite, object, object, object>> arg2 = <>o__34.<>p__1;
		if (<>o__34.<>p__0 == null)
		{
			<>o__34.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		object obj = target2(arg2, <>o__34.<>p__0.Target(<>o__34.<>p__0, (object)data, "ACBrightness"), null);
		if (<>o__34.<>p__5 == null)
		{
			<>o__34.<>p__5 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		object arg5;
		if (!<>o__34.<>p__5.Target(<>o__34.<>p__5, obj))
		{
			if (<>o__34.<>p__4 == null)
			{
				<>o__34.<>p__4 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, object, object, object> target3 = <>o__34.<>p__4.Target;
			CallSite<Func<CallSite, object, object, object>> arg3 = <>o__34.<>p__4;
			if (<>o__34.<>p__3 == null)
			{
				<>o__34.<>p__3 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target4 = <>o__34.<>p__3.Target;
			CallSite<Func<CallSite, object, object, object>> arg4 = <>o__34.<>p__3;
			if (<>o__34.<>p__2 == null)
			{
				<>o__34.<>p__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			arg5 = target3(arg3, obj, target4(arg4, <>o__34.<>p__2.Target(<>o__34.<>p__2, (object)data, "DCBrightness"), null));
		}
		else
		{
			arg5 = obj;
		}
		if (target(arg, arg5))
		{
			if (<>o__34.<>p__9 == null)
			{
				<>o__34.<>p__9 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
			}
			Func<CallSite, object, uint> target5 = <>o__34.<>p__9.Target;
			CallSite<Func<CallSite, object, uint>> arg6 = <>o__34.<>p__9;
			if (<>o__34.<>p__8 == null)
			{
				<>o__34.<>p__8 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target6 = <>o__34.<>p__8.Target;
			CallSite<Func<CallSite, Type, object, object>> arg7 = <>o__34.<>p__8;
			Type arg8 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__34.<>p__7 == null)
			{
				<>o__34.<>p__7 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			uint aCBrightness = target5(arg6, target6(arg7, arg8, <>o__34.<>p__7.Target(<>o__34.<>p__7, (object)data, "ACBrightness")));
			if (<>o__34.<>p__12 == null)
			{
				<>o__34.<>p__12 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
			}
			Func<CallSite, object, uint> target7 = <>o__34.<>p__12.Target;
			CallSite<Func<CallSite, object, uint>> arg9 = <>o__34.<>p__12;
			if (<>o__34.<>p__11 == null)
			{
				<>o__34.<>p__11 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target8 = <>o__34.<>p__11.Target;
			CallSite<Func<CallSite, Type, object, object>> arg10 = <>o__34.<>p__11;
			Type arg11 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__34.<>p__10 == null)
			{
				<>o__34.<>p__10 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			uint dCBrighteness = target7(arg9, target8(arg10, arg11, <>o__34.<>p__10.Target(<>o__34.<>p__10, (object)data, "DCBrightness")));
			m_hidkeyboad?.SetBrightness_ACDC(aCBrightness, dCBrighteness);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RGBKeyboard_SetEffectLight(dynamic data)
	{
		if (<>o__35.<>p__2 == null)
		{
			<>o__35.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
		}
		Func<CallSite, object, string> target = <>o__35.<>p__2.Target;
		CallSite<Func<CallSite, object, string>> arg = <>o__35.<>p__2;
		if (<>o__35.<>p__0 == null)
		{
			<>o__35.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		object obj = <>o__35.<>p__0.Target(<>o__35.<>p__0, (object)data, "mode");
		object arg2;
		if (obj == null)
		{
			arg2 = null;
		}
		else
		{
			if (<>o__35.<>p__1 == null)
			{
				<>o__35.<>p__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg2 = <>o__35.<>p__1.Target(<>o__35.<>p__1, obj);
		}
		string text = target(arg, arg2);
		RGBKB_Mode mode = (RGBKB_Mode)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Mode).TypeHandle), text);
		if (mode.Equals(RGBKB_Mode.Welcome))
		{
			if (<>o__35.<>p__4 == null)
			{
				<>o__35.<>p__4 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target2 = <>o__35.<>p__4.Target;
			CallSite<Func<CallSite, object, bool>> arg3 = <>o__35.<>p__4;
			HIDKeyboard hidkeyboad = m_hidkeyboad;
			object arg4;
			if (hidkeyboad == null)
			{
				arg4 = null;
			}
			else
			{
				if (<>o__35.<>p__3 == null)
				{
					<>o__35.<>p__3 = CallSite<Func<CallSite, HIDKeyboard, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "SetWelcomeEffect", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				arg4 = <>o__35.<>p__3.Target(<>o__35.<>p__3, hidkeyboad, (object)data);
			}
			if (target2(arg3, arg4))
			{
				m_hidkeyboad.SetMode(mode);
				m_hidkeyboad?.RunWelcomeEffect();
			}
		}
		else if (m_LM != null)
		{
			if (<>o__35.<>p__7 == null)
			{
				<>o__35.<>p__7 = CallSite<Func<CallSite, object, uint>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(uint).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
			}
			Func<CallSite, object, uint> target3 = <>o__35.<>p__7.Target;
			CallSite<Func<CallSite, object, uint>> arg5 = <>o__35.<>p__7;
			if (<>o__35.<>p__6 == null)
			{
				<>o__35.<>p__6 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target4 = <>o__35.<>p__6.Target;
			CallSite<Func<CallSite, Type, object, object>> arg6 = <>o__35.<>p__6;
			Type arg7 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__35.<>p__5 == null)
			{
				<>o__35.<>p__5 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			uint brightness = target3(arg5, target4(arg6, arg7, <>o__35.<>p__5.Target(<>o__35.<>p__5, (object)data, "light")));
			m_hidkeyboad?.SetMode(mode);
			m_hidkeyboad?.SetBrightness(brightness);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void RGBKeyboard_SetColor(dynamic data)
	{
		if (m_LM == null)
		{
			return;
		}
		if (<>o__36.<>p__2 == null)
		{
			<>o__36.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
		}
		Func<CallSite, object, string> target = <>o__36.<>p__2.Target;
		CallSite<Func<CallSite, object, string>> arg = <>o__36.<>p__2;
		if (<>o__36.<>p__0 == null)
		{
			<>o__36.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		object obj = <>o__36.<>p__0.Target(<>o__36.<>p__0, (object)data, "mode");
		object arg2;
		if (obj == null)
		{
			arg2 = null;
		}
		else
		{
			if (<>o__36.<>p__1 == null)
			{
				<>o__36.<>p__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg2 = <>o__36.<>p__1.Target(<>o__36.<>p__1, obj);
		}
		string text = target(arg, arg2);
		RGBKB_Mode layout_mode = (RGBKB_Mode)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Mode).TypeHandle), text);
		if (<>o__36.<>p__5 == null)
		{
			<>o__36.<>p__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
		}
		Func<CallSite, object, string> target2 = <>o__36.<>p__5.Target;
		CallSite<Func<CallSite, object, string>> arg3 = <>o__36.<>p__5;
		if (<>o__36.<>p__3 == null)
		{
			<>o__36.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		obj = <>o__36.<>p__3.Target(<>o__36.<>p__3, (object)data, "effect");
		object arg4;
		if (obj == null)
		{
			arg4 = null;
		}
		else
		{
			if (<>o__36.<>p__4 == null)
			{
				<>o__36.<>p__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			arg4 = <>o__36.<>p__4.Target(<>o__36.<>p__4, obj);
		}
		string text2 = target2(arg3, arg4);
		RGBKB_Effect layout_effect = (RGBKB_Effect)ResolvedDelegates.System_Enum_Parse__342(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Effect).TypeHandle), text2);
		if (<>o__36.<>p__8 == null)
		{
			<>o__36.<>p__8 = CallSite<Func<CallSite, object, RGBKB_Color>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKB_Color).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle)));
		}
		Func<CallSite, object, RGBKB_Color> target3 = <>o__36.<>p__8.Target;
		CallSite<Func<CallSite, object, RGBKB_Color>> arg5 = <>o__36.<>p__8;
		if (<>o__36.<>p__7 == null)
		{
			<>o__36.<>p__7 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ConvertJsonRGB2RGBColor", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target4 = <>o__36.<>p__7.Target;
		CallSite<Func<CallSite, Type, object, object>> arg6 = <>o__36.<>p__7;
		Type arg7 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle);
		if (<>o__36.<>p__6 == null)
		{
			<>o__36.<>p__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(RGBKeyboard).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		RGBKB_Color layout_color = target3(arg5, target4(arg6, arg7, <>o__36.<>p__6.Target(<>o__36.<>p__6, (object)data, "color")));
		if (m_LM.m_KB_Type == RGBKB_Type.FourZoneSingleColor)
		{
			LogCtrl.TraceMessage("FourZoneSingleColor keyboard, can not set color", "RGBKeyboard_SetColor", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\RGBKeyboard.cs", 789);
		}
		else
		{
			m_LM.LM_SetColor(layout_mode, layout_effect, layout_color);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void RGBKeyboard_SaveLightingLevel(dynamic data)
	{
		if (m_LM != null)
		{
			m_LM.LM_SaveLightingLevel(0u);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<OnMqttMessage>d__40))]
	public void OnMqttMessage(string topic, string msg)
	{
		<OnMqttMessage>d__40 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine.topic = topic;
		stateMachine.msg = msg;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	public static void OnMqttDisconnection()
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool init()
	{
		m_MQTTClient = MqttClientCtrl.Instance;
		RGBKeyboard_Init();
		return m_MQTTClient != null;
	}

	public void EnableByService()
	{
	}

	public void DisableByService()
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Resume()
	{
		if (!CheckAurora.Instance.GetAuroraStauts() && m_hidkeyboad != null)
		{
			LogCtrl.Write("RGBKeybaord From S4 Resume");
			if (m_hidkeyboad._KeyboardControl != null)
			{
				m_hidkeyboad?._KeyboardControl?.Set_ITE_Effect_Type_ApMode_Stop();
			}
			m_hidkeyboad?.RunEffct(0, 2);
			LogCtrl.Write("KeybaordTest");
			HIDKeyboard hidkeyboad = m_hidkeyboad;
			if (hidkeyboad != null && hidkeyboad.GetPowerStatus() == RGBKB_PowerStatus.Off)
			{
				m_hidkeyboad?.SetPowerStatus(RGBKB_PowerStatus.Off, 0, "");
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Suspend()
	{
		if (!ResolvedDelegates.System_Threading_Monitor_TryEnter__145(suspendLock, 3000))
		{
			return;
		}
		try
		{
			object obj = ResolvedDelegates.System_Threading_Thread_Join__324;
			Thread thread = new Thread(() =>
			{
				m_hidkeyboad?.CloseAnimation();
			});
			thread.Start();
			obj(thread);
		}
		catch (Exception)
		{
		}
		finally
		{
			ResolvedDelegates.System_Threading_Monitor_Exit__71(suspendLock);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Restore()
	{
		string folderPath = ResolvedDelegates.System_IO_FileInfo_get_DirectoryName__98(new FileInfo(ResolvedDelegates.System_Reflection_Assembly_get_Location__97(ResolvedDelegates.System_Reflection_Assembly_GetExecutingAssembly__96())));
		string text = null;
		folderPath = LogCtrl.GetParentDirectoryPath(folderPath, 2);
		text = ResolvedDelegates.System_String_Concat__107("/s \"", folderPath, "\\RGBKeyboard.reg\"");
		if (m_CustomizeTarget == 23 && ResolvedDelegates.System_String_op_Equality__18(BiosPprojectID, "IDZ"))
		{
			text = ResolvedDelegates.System_String_Concat__107("/s \"", folderPath, "\\IDZ\\RGBKeyboard.reg\"");
		}
		Process.Start("regedit.exe", text).WaitForExit();
		ResolvedDelegates.System_Threading_Tasks_Task_Start__166(new Task<int>([MethodImpl(MethodImplOptions.NoInlining)] () =>
		{
			int num = 0;
			RGBKeyboard_Init();
			do
			{
				GetHIDKeyboard()?.GetInitSuccessful();
				Thread.Sleep(300);
				num++;
			}
			while (num != 20);
			return num;
		}));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Uninstall()
	{
		m_hidkeyboad?.Uninstall();
	}
}

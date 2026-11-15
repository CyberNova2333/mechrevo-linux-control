// Reconstructed static analysis view, not original source: HIDKeyboard.cs:3045-3231
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
				LogCtrl.Write(Delegates.System_String_Concat__29("RunEffctEC(), Run Effect in ", Delegates.System_Byte_ToString__119(ref _EffectData.save_effect)));
				byte Data = 0;
				byte b = 64;
				switch (Delegates.System_Convert_ToUInt32__161(_EffectData.save_light))
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
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, ref Data2);
					LogCtrl.TraceMessage(Delegates.System_String_Concat__29("0x767 Single Read: ", Delegates.System_Byte_ToString__119(ref Data2)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1847);
					Data2 &= 0x7F;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, Data2);
					LogCtrl.TraceMessage(Delegates.System_String_Concat__29("0x767 Single Write: ", Delegates.System_Byte_ToString__119(ref Data2)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1850);
					Data2 = 0;
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, ref Data2);
					Data2 &= 0xF8;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, Data2);
					byte b2 = DivideValue(_EffectData.save_layout_color.ColorBuffer[0].R, 5.1);
					byte b3 = DivideValue(_EffectData.save_layout_color.ColorBuffer[0].G, 5.1);
					byte b4 = DivideValue(_EffectData.save_layout_color.ColorBuffer[0].B, 5.1);
					if (b2 == 0 && b3 == 0 && b4 == 0)
					{
						flag = true;
					}
					else
					{
						MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1897, b2);
						MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1898, b3);
						MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1899, b4);
						LogCtrl.Write(Delegates.System_String_Concat__79(new string[6]
						{
							"RunEffctEC() RGB: ",
							Delegates.System_Byte_ToString__119(ref b2),
							",",
							Delegates.System_Byte_ToString__119(ref b3),
							",",
							Delegates.System_Byte_ToString__119(ref b4)
						}));
					}
				}
				else if (_EffectData.save_effect == 2)
				{
					byte Data3 = 0;
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, ref Data3);
					LogCtrl.TraceMessage(Delegates.System_String_Concat__29("0x767 Breathing Read: ", Delegates.System_Byte_ToString__119(ref Data3)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1880);
					Data3 &= 0x7F;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, Data3);
					LogCtrl.TraceMessage(Delegates.System_String_Concat__29("0x767 Breathing Write: ", Delegates.System_Byte_ToString__119(ref Data3)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1883);
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
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, ref Data3);
					Data3 &= 0xF8;
					Data3 |= b5;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, Data3);
				}
				else if (_EffectData.save_effect == 19)
				{
					byte Data4 = 0;
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, ref Data4);
					Data4 &= 0xF8;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, Data4);
					Data4 = 0;
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, ref Data4);
					LogCtrl.TraceMessage(Delegates.System_String_Concat__29("0x767 Colorful Read: ", Delegates.System_Byte_ToString__119(ref Data4)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1922);
					Data4 &= 0x7F;
					Data4 |= 0x80;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, Data4);
					LogCtrl.TraceMessage(Delegates.System_String_Concat__29("0x767 Colorful Write: ", Delegates.System_Byte_ToString__119(ref Data4)), "RunEffctEC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyRgbKeyboard\\HIDKeyboard\\HIDKeyboard.cs", 1926);
				}
				MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, ref Data);
				Data &= 0x1F;
				if (flag)
				{
					b = 0;
				}
				Data |= b;
				MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, Data);
				LogCtrl.Write(Delegates.System_String_Concat__29("RunEffctEC() Brightness: ", Delegates.System_Byte_ToString__119(ref Data)));
				byte Data5 = 0;
				MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, ref Data5);
				Data5 |= 0x10;
				MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, Data5);
			}
			else
			{
				byte b6 = 0;
				byte Data6 = 0;
				MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, ref Data6);
				Data6 &= 0x1F;
				Data6 |= b6;
				MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, Data6);
				byte Data7 = 0;
				MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, ref Data7);
				Data7 |= 0x10;
				MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, Data7);
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
			MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, ref Data);
			Data &= 0x1F;
			Data |= b;
			MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, Data);
			byte Data2 = 0;
			MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, ref Data2);
			Data2 |= 0x10;
			MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, Data2);
		}
		catch (Exception)
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static byte DivideValue(byte value, double divisor)
	{
		return (byte)Delegates.System_Math_Min__755((int)Delegates.System_Math_Floor__188((double)(int)value / divisor), 50);
	}

// Reconstructed static analysis view, not original source: LM_ITE_RGB.cs:5124-5135
				RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\RGBKeyboard", "Forcenewlogosupport", newlogosupport, RegistryValueKind.DWord);
				byte Data = 0;
				EcCtrl.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, ref Data);
				Data &= 1;
				if (Data == 1)
				{
					MBlogoSupport = true;
				}
				RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\RGBKeyboard", "MBlogoSupport", MBlogoSupport, RegistryValueKind.DWord);
			}
			catch
			{

// Reconstructed static analysis view, not original source: NvramVariable.cs:1109-1115
		m_ALog_EC_Control = isSupportECALogo();
		if (m_SingleZoneDetection == 1 || m_supportSingleColorKeyboard == 1)
		{
			m_ALog_EC_Control = 0;
		}
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\Support", "MBALogo", m_ALog_EC_Control, RegistryValueKind.DWord);
		LogCtrl.Write("NvramVariable  [Get] End");

// Reconstructed static analysis view, not original source: NvramVariable.cs:1253-1264
	private static int isSupportECALogo()
	{
		byte Data = 0;
		EcCtrl.Read(m_ClassName, 1963, ref Data);
		if ((byte)(Delegates.System_Convert_ToUInt64__129(Data) & 1) == 1)
		{
			LogCtrl.Write("NvramVariable  [Get] isSupportECALogo | true");
			return 1;
		}
		LogCtrl.Write("NvramVariable  [Get] isSupportECALogo | false");
		return 0;
	}

// Reconstructed static analysis view, not original source: HIDLightbar_logo.cs:890-980
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void CreateInstance_ControlBrigtnessEvent(object sender, EventArgs e)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Invalid comparison between Unknown and I4
		if (mbLogo <= 0)
		{
			return;
		}
		if (Delegates.System_Convert_ToBoolean__42(sender))
		{
			if (m_hidLighBar != null)
			{
				m_hidLighBar.CloseEffectEC();
			}
			else
			{
				SetBrightnessALogo(0u);
			}
		}
		else if (m_hidLighBar != null)
		{
			m_hidLighBar.RunEffctEC();
		}
		else
		{
			string text = "0";
			text = (((int)Delegates.System_Windows_Forms_PowerStatus_get_PowerLineStatus__341(ps) != 1) ? ((string)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\RGBKeyboard", "Lightbar_logo_DCLight", null, RegistryValueKind.String)) : ((string)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\RGBKeyboard", "Lightbar_logo_ACLight", null, RegistryValueKind.String)));
			uint brightnessALogo = (uint)Convert.ToInt32(text);
			SetBrightnessALogo(brightnessALogo);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetBrightnessALogo(uint light)
	{
		if (_PowerStatus == RGBKB_PowerStatus.On)
		{
			byte b = 64;
			byte Data = 0;
			switch (light)
			{
			case 0u:
				b = 0;
				break;
			case 1u:
				b = 32;
				break;
			case 2u:
				b = 64;
				break;
			case 3u:
				b = 96;
				break;
			case 4u:
				b = 128;
				break;
			}
			MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, ref Data);
			Data &= 0x1F;
			Data |= b;
			MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, Data);
			LogCtrl.Write(Delegates.System_String_Concat__29("RunEffctEC() Brightness: ", Delegates.System_Byte_ToString__119(ref Data)));
			byte Data2 = 0;
			MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, ref Data2);
			Data2 |= 0x10;
			MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, Data2);
		}
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
			if ((int)Delegates.System_Windows_Forms_PowerStatus_get_PowerLineStatus__341(ps) == 0)
			{
				m_hidLighBar?.UnPlugged();
			}
			if ((int)ps.PowerLineStatus == 1)
			{
				m_hidLighBar?.Plugged();
			}
			m_hidLighBar?.RunEffctEC();
			KeyboardCloseTimer.CreateInstance.RestartCloseTimer();
		}

// Reconstructed static analysis view, not original source: HIDLightbar_logo.cs:1910-2045
				case 1u:
					b = 32;
					break;
				case 2u:
					b = 64;
					break;
				case 3u:
					b = 96;
					break;
				case 4u:
					b = 128;
					break;
				}
				switch (rGBKB_Effect)
				{
				case RGBKB_Effect.Single:
				{
					byte Data4 = 0;
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, ref Data4);
					Data4 &= 0x7F;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, Data4);
					Data4 = 0;
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, ref Data4);
					Data4 &= 0xF8;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, Data4);
					byte b6 = HIDKeyboard.DivideValue(b2, 5.1);
					byte b7 = HIDKeyboard.DivideValue(b3, 5.1);
					byte b8 = HIDKeyboard.DivideValue(b4, 5.1);
					if (b6 != 0 || b7 != 0 || b8 != 0)
					{
						MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1897, b6);
						MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1898, b7);
						MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1899, b8);
						LogCtrl.Write(Delegates.System_String_Concat__79(new string[6]
						{
							"RunEffctEC() RGB: ",
							Delegates.System_Byte_ToString__119(ref b6),
							",",
							Delegates.System_Byte_ToString__119(ref b7),
							",",
							Delegates.System_Byte_ToString__119(ref b8)
						}));
					}
					break;
				}
				case RGBKB_Effect.Breathing:
				{
					byte Data3 = 0;
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, ref Data3);
					Data3 &= 0x7F;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, Data3);
					byte b5 = 0;
					if (b2 == byte.MaxValue && b3 == 0 && b4 == 0)
					{
						b5 = 2;
					}
					else if (b2 == 0 && b3 == byte.MaxValue && b4 == 0)
					{
						b5 = 3;
					}
					else if (b2 == 0 && b3 == 0 && b4 == byte.MaxValue)
					{
						b5 = 4;
					}
					else if (b2 == byte.MaxValue && b3 == byte.MaxValue && b4 == 0)
					{
						b5 = 5;
					}
					else if (b2 == 136 && b3 == 0 && b4 == byte.MaxValue)
					{
						b5 = 6;
					}
					else if (b2 == 0 && b3 == byte.MaxValue && b4 == byte.MaxValue)
					{
						b5 = 7;
					}
					else if (b2 == byte.MaxValue && b3 == byte.MaxValue && b4 == byte.MaxValue)
					{
						b5 = 1;
					}
					Data3 = 0;
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, ref Data3);
					Data3 &= 0xF8;
					Data3 |= b5;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, Data3);
					break;
				}
				case RGBKB_Effect.Mix:
				{
					byte Data2 = 0;
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, ref Data2);
					Data2 &= 0xF8;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1989, Data2);
					Data2 = 0;
					MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, ref Data2);
					Data2 &= 0x7F;
					Data2 |= 0x80;
					MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1895, Data2);
					break;
				}
				}
				MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, ref Data);
				Data &= 0x1F;
				Data |= b;
				MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, Data);
				LogCtrl.Write(Delegates.System_String_Concat__29("RunEffctEC() Brightness: ", Delegates.System_Byte_ToString__119(ref Data)));
				byte Data5 = 0;
				MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, ref Data5);
				Data5 |= 0x10;
				MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, Data5);
			}
			else
			{
				b = 0;
				string text3 = "0";
				text3 = (((int)Delegates.System_Windows_Forms_PowerStatus_get_PowerLineStatus__341(ps) != 1) ? ((string)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\RGBKeyboard", "Lightbar_logo_DCLight", null, RegistryValueKind.String)) : ((string)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\RGBKeyboard", "Lightbar_logo_ACLight", null, RegistryValueKind.String)));
				num = (uint)Convert.ToInt32(text3);
				byte Data6 = 0;
				MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, ref Data6);
				Data6 &= 0x1F;
				Data6 |= b;
				MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1963, Data6);
				byte Data7 = 0;
				MyEcCtrl.Instance.Read(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, ref Data7);
				Data7 |= 0x10;
				MyEcCtrl.Instance.Write(Delegates.System_Reflection_MemberInfo_get_Name__128(Delegates.System_Object_GetType__121(this)), 1932, Data7);
			}
			_HidLightbarStatus.brightNess = Delegates.System_UInt32_ToString__78(ref num);
			_HidLightbarStatus.powerStatus = _PowerStatus.ToString();
			_HidLightbarStatus.currentPowerMode = (((int)Delegates.System_Windows_Forms_PowerStatus_get_PowerLineStatus__341(ps) == 0) ? RGBKB_PowerMode.DC.ToString() : RGBKB_PowerMode.AC.ToString());
			m_MQTTClient.Publish("HidLightbar_Logo/Status", _HidLightbarStatus);
		}
		catch (Exception)
		{
		}
	}

// Reconstructed static analysis view, not original source: HIDLightbar_logo.cs:2070-2120
	public void Resume()
	{
		if (!CheckAurora.Instance.GetAuroraStauts() && m_hidLighBar != null)
		{
			LogCtrl.Write("HidLightbar From S4 Resume");
			m_hidLighBar?._KeyboardControl.Set_ITE_Effect_Type_ApMode_Stop();
			m_hidLighBar?.RunEffct(0, 2);
			HIDKeyboard hidLighBar = m_hidLighBar;
			if (hidLighBar != null && hidLighBar.GetPowerStatus() == RGBKB_PowerStatus.Off)
			{
				m_hidLighBar?.SetPowerStatus(RGBKB_PowerStatus.Off, 0, "");
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Suspend()
	{
		if (!Delegates.System_Threading_Monitor_TryEnter__145(suspendLock, 5000))
		{
			return;
		}
		try
		{
			object obj = Delegates.System_Threading_Thread_Join__324;
			Thread thread = new Thread([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				m_hidLighBar?.CloseAnimation();
			});
			thread.Start();
			obj(thread);
		}
		catch (Exception)
		{
		}
		finally
		{
			Delegates.System_Threading_Monitor_Exit__71(suspendLock);
		}
	}

	public void Uninstall()
	{
		SettingsStorageExtensions.DeleteRGBKeyboardRegistry();
	}
}

// Reconstructed static analysis view, not original source: KeyboardCloseTimer.cs:144-190
	private void Keyboard_MouseEventTrigger(EventArgs e)
	{
		EnableCloseTimer();
		if (this.m_ControlBrigtnessEvent != null && !_StartMoniter)
		{
			Delegates.System_EventHandler_Invoke__49(this.m_ControlBrigtnessEvent, false, null);
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
		_AccumulationTime += Delegates.System_Convert_ToInt32__494(Delegates.System_Timers_Timer_get_Interval__590(_TimersTimer) / 1000.0);
		DisableCloseTimer();
		if (this.m_ControlBrigtnessEvent != null && _StartMoniter)
		{
			Delegates.System_EventHandler_Invoke__49(this.m_ControlBrigtnessEvent, true, null);
			_StartMoniter = false;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetKeyboardCloseTime(int seconds)
	{
		int num = int.MaxValue;
		Delegates.System_Int32_TryParse__132(Delegates.System_Int32_ToString__55(ref seconds), ref num);
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


// Reconstructed static analysis view, not original source: HIDLightbar4_logo.cs:150-157
	[MethodImpl(MethodImplOptions.NoInlining)]
	public HIDLightbar4_logo(LM_Manager lm_Manger, string fwVersion)
		: base(lm_Manger, fwVersion)
	{
		SetACDCLightString("Lightbar_logo_PowerSwitch", "Lightbar_logo_ACLight", "Lightbar_logo_DCLight", RGBKB_Type.MEZone_Lighbar4_logo);
		_KeyboardControl.ColShift = 0;
		_KeyboardControl.ColRightShift = 1;
	}

// Reconstructed static analysis view, not original source: HIDLightbar4_logo.cs:274-317
	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void RunEffct(byte saved = 0, byte control = 2)
	{
		_EffectData.save_layout_color.isCircular = false;
		if (_EffectData.save_effect == 1)
		{
			_EffectData.save_layout_color.isCircular = false;
			base.RunEffct(saved, controlByte);
		}
		else if (_EffectData.save_effect == 2)
		{
			_EffectData.save_layout_color.isCircular = true;
			base.RunEffct(saved, controlByte);
		}
		else if (_EffectData.save_effect == 4 || _EffectData.save_effect == 13)
		{
			_EffectData.save_effect = 4;
			base.RunEffct(saved, controlByte);
		}
		else if (_EffectData.save_effect == 5 || _EffectData.save_effect == 33)
		{
			_EffectData.save_effect = 5;
			base.RunEffct(saved, controlByte);
		}
		else if (_EffectData.save_effect == 6 || _EffectData.save_effect == 10)
		{
			_EffectData.save_layout_color.isCircular = false;
			_EffectData.save_effect = 6;
			base.RunEffct(saved, controlByte);
		}
		else
		{
			base.RunEffct(saved, controlByte);
		}
		RunEffctEC();
		beforeEffect = _EffectData;
		LogCtrl.Write("Run Effect HIDLightbar4_logo");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<InitalizeWelcomeKeyboardEffect>d__11))]
	public override Task InitalizeWelcomeKeyboardEffect(RGBKB_PowerStatus power, bool IsAppExists, bool unplug = false)
	{
		<InitalizeWelcomeKeyboardEffect>d__11 stateMachine = default;

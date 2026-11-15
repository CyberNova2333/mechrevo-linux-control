// Reconstructed analysis view; strings decoded statically; unresolved delegates remain.
using System;
using System.Runtime.CompilerServices;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Microsoft.CSharp.RuntimeBinder;
using MyECIO;
using Utility;

namespace MyControlCenter.MyFan.FanTable;

public class MyFanTableCtrl
{
	[CompilerGenerated]
	private sealed class <>c__DisplayClass15_0
	{
		public MyFanTableCtrl <>4__this;

		public string _name;

		public <>c__DisplayClass15_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void <SetFanTable>b__0()
		{
			<>4__this.SetFanControlByRamFan1p5(status: false);
			<>4__this.m_Manager.SetFanTable(_name);
			<>4__this.SetFanControlByRamFan1p5(status: true);
		}
	}

	[CompilerGenerated]
	private static class <>o__16
	{
		public static CallSite<Action<CallSite, FanTable_Manager1p5, object, bool>> <>p__0;
	}

	[CompilerGenerated]
	private static class <>o__17
	{
		public static CallSite<Action<CallSite, FanTable_Manager1p5, object>> <>p__0;
	}

	[CompilerGenerated]
	private static class <>o__19
	{
		public static CallSite<Action<CallSite, FanTable_Manager1p5, object>> <>p__0;
	}

	private FanTable_Manager1p5 m_Manager;

	private static readonly MyFanTableCtrl control = new MyFanTableCtrl();

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	private BackgroundQueue backgroundQueue = new BackgroundQueue();

	private bool _IsLCConnect;

	private string _ModeFanTable = "DefaultFanTable_Gaming";

	public static MyFanTableCtrl Instance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return control;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public MyFanTableCtrl()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
		int customizeTarget = RegistryCtrl.GetCustomizeTarget();
		if (EcCtrl.IsSupportRamFan1p5())
		{
			if (!EcCtrl.GetProject2ExID(EcCtrl.GetProjectIdFromEC()).IsProjectId_Commercial())
			{
				m_Manager = new FanTable_Manager1p5();
			}
			else if (customizeTarget == 11)
			{
				m_Manager = null;
			}
			else
			{
				m_Manager = new FanTable_Manager1p5_CML();
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool GetFanTableInit()
	{
		if (m_Manager != null)
		{
			return m_Manager.GetFanTableInit();
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void FanTable_Init()
	{
		if (m_Manager != null)
		{
			m_Manager?.FanTable_Init();
		}
	}

	public void DisableByService()
	{
		SetFanControlByRamFan1p5(status: false);
	}

	public void Resume()
	{
	}

	public void Uninstall()
	{
		SetFanControlByRamFan1p5(status: false);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void GetFanTable(string _name)
	{
		m_Manager.GetFanTable(_name);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetFanTable(string _name)
	{
		<>c__DisplayClass15_0 CS$<>8__locals12 = new <>c__DisplayClass15_0();
		CS$<>8__locals12.<>4__this = this;
		CS$<>8__locals12._name = _name;
		if (ResolvedDelegates.System_String_op_Inequality__40(CS$<>8__locals12._name, "IsLCConnect") && ResolvedDelegates.System_String_op_Inequality__40(CS$<>8__locals12._name, "IsLCDisconnect"))
		{
			_ModeFanTable = CS$<>8__locals12._name;
		}
		if (CS$<>8__locals12._name == "IsLCConnect")
		{
			_IsLCConnect = true;
		}
		else if (CS$<>8__locals12._name == "IsLCDisconnect")
		{
			_IsLCConnect = false;
			CS$<>8__locals12._name = _ModeFanTable;
		}
		if (!_IsLCConnect)
		{
			backgroundQueue.QueueTask([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				CS$<>8__locals12.<>4__this.SetFanControlByRamFan1p5(status: false);
				CS$<>8__locals12.<>4__this.m_Manager.SetFanTable(CS$<>8__locals12._name);
				CS$<>8__locals12.<>4__this.SetFanControlByRamFan1p5(status: true);
			});
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetFanTableSetting(dynamic data, bool saveEC = true)
	{
		if (<>o__16.<>p__0 == null)
		{
			<>o__16.<>p__0 = CallSite<Action<CallSite, FanTable_Manager1p5, object, bool>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetFanTableSetting", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanTableCtrl).TypeHandle), new CSharpArgumentInfo[3]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			}));
		}
		<>o__16.<>p__0.Target(<>o__16.<>p__0, m_Manager, (object)data, saveEC);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetFanControlRespective(dynamic data)
	{
		if (<>o__17.<>p__0 == null)
		{
			<>o__17.<>p__0 = CallSite<Action<CallSite, FanTable_Manager1p5, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetFanControlRespective", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanTableCtrl).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		<>o__17.<>p__0.Target(<>o__17.<>p__0, m_Manager, (object)data);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RestoreDefaultFanTableAll()
	{
		m_Manager.RestoreDefaultFanTableAll();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RestoreDefaultFanTable(dynamic data)
	{
		if (<>o__19.<>p__0 == null)
		{
			<>o__19.<>p__0 = CallSite<Action<CallSite, FanTable_Manager1p5, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "RestoreDefaultFanTable", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanTableCtrl).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		<>o__19.<>p__0.Target(<>o__19.<>p__0, m_Manager, (object)data);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void GetPLDefaultValue(string name, ref string PL1, ref string PL2, ref string PL1_dc, ref string PL2_dc)
	{
		m_Manager.GetPLDefaultValue(name, ref PL1, ref PL2, ref PL1_dc, ref PL2_dc);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void GetGpuFeatureDefaultValue(string name, ref string CTGP, ref string DB, ref string WM)
	{
		m_Manager.GetGpuFeatureDefaultValue(name, ref CTGP, ref DB, ref WM);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void GetFeatureDefaultValue_BALANCED_PROTECT(string name, ref string CTGP2, ref string DB2, ref string PL1_S2, ref string PL2_S2)
	{
		m_Manager.GetFeatureDefaultValue_BALANCED_PROTECT(name, ref CTGP2, ref DB2, ref PL1_S2, ref PL2_S2);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetFanControlByRamFan1p5(bool status)
	{
		byte Data = 0;
		byte b = 251;
		byte b2 = 0;
		byte b3 = 0;
		if (status)
		{
			byte Data2 = 0;
			byte Data3 = 0;
			EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3840, ref Data2);
			EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3841, ref Data3);
			b2 = (byte)((Data2 != 0 || Data3 != 0) ? 4 : 0);
		}
		else
		{
			b2 = 0;
		}
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1990, ref Data);
		b3 = (byte)((Data & b) + b2);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1990, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetAPCustomerModeLightOn()
	{
		byte Data = 0;
		byte b = 64;
		byte b2 = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1831, ref Data);
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Read SetAPCustomerModeLightOn 0x727: ", ResolvedDelegates.System_Byte_ToString__119(ref Data)));
		b2 = (byte)(Data | b);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1831, b2);
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Write SetAPCustomerModeLightOn 0x727: ", ResolvedDelegates.System_Byte_ToString__119(ref b2)));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetAPCustomerModeLightOff()
	{
		byte Data = 0;
		byte b = 191;
		byte b2 = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1831, ref Data);
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Read SetAPCustomerModeLightOff 0x727: ", ResolvedDelegates.System_Byte_ToString__119(ref Data)));
		b2 = (byte)(Data & b);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1831, b2);
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Write SetAPCustomerModeLightOff 0x727: ", ResolvedDelegates.System_Byte_ToString__119(ref b2)));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private byte ReadAPCustomerModeLight()
	{
		byte Data = 0;
		byte b = 64;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1831, ref Data);
		return (byte)(Data & b);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void CustomerModeLightOn()
	{
		SetAPCustomerModeLightOn();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void CustomerModeLightOff()
	{
		SetAPCustomerModeLightOff();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public byte ReadCustomerModeLightStatus()
	{
		return ReadAPCustomerModeLight();
	}
}

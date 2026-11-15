// Reconstructed analysis view; strings decoded statically; unresolved delegates remain.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Microsoft.CSharp.RuntimeBinder;
using MyECIO;
using Newtonsoft.Json;
using Utility;

namespace MyControlCenter.MyFan.FanTable;

public class FanTable_Manager1p5
{
	[CompilerGenerated]
	private static class <>o__35
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

		public static CallSite<Func<CallSite, object, string>> <>p__9;

		public static CallSite<Func<CallSite, object, string, object>> <>p__10;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__11;

		public static CallSite<Func<CallSite, object, bool>> <>p__12;
	}

	[CompilerGenerated]
	private static class <>o__38
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

		public static CallSite<Func<CallSite, object, string>> <>p__9;

		public static CallSite<Func<CallSite, object, string, object>> <>p__10;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__11;

		public static CallSite<Func<CallSite, object, string>> <>p__12;

		public static CallSite<Action<CallSite, FanTable_Manager1p5, FanTable1p5, string, object, bool>> <>p__13;
	}

	[CompilerGenerated]
	private static class <>o__40
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, bool>> <>p__2;

		public static CallSite<Func<CallSite, object, string, object>> <>p__3;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, string>> <>p__5;
	}

	[CompilerGenerated]
	private static class <>o__48
	{
		public static CallSite<Func<CallSite, object, object, object>> <>p__0;

		public static CallSite<Func<CallSite, object, bool>> <>p__1;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__2;

		public static CallSite<Func<CallSite, object, string>> <>p__3;
	}

	[CompilerGenerated]
	private static class <>o__54
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, byte>> <>p__2;

		public static CallSite<Func<CallSite, object, string, object>> <>p__3;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, byte>> <>p__5;

		public static CallSite<Func<CallSite, object, string, object>> <>p__6;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__7;

		public static CallSite<Func<CallSite, object, byte>> <>p__8;

		public static CallSite<Func<CallSite, object, string, object>> <>p__9;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__10;

		public static CallSite<Func<CallSite, object, byte>> <>p__11;

		public static CallSite<Func<CallSite, object, string, object>> <>p__12;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__13;

		public static CallSite<Func<CallSite, object, byte>> <>p__14;

		public static CallSite<Func<CallSite, object, string, object>> <>p__15;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__16;

		public static CallSite<Func<CallSite, object, byte>> <>p__17;

		public static CallSite<Func<CallSite, object, string, object>> <>p__18;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__19;

		public static CallSite<Func<CallSite, object, byte>> <>p__20;

		public static CallSite<Func<CallSite, object, string, object>> <>p__21;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__22;

		public static CallSite<Func<CallSite, object, byte>> <>p__23;

		public static CallSite<Func<CallSite, object, string, object>> <>p__24;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__25;

		public static CallSite<Func<CallSite, object, byte>> <>p__26;

		public static CallSite<Func<CallSite, object, string, object>> <>p__27;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__28;

		public static CallSite<Func<CallSite, object, byte>> <>p__29;

		public static CallSite<Func<CallSite, object, string, object>> <>p__30;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__31;

		public static CallSite<Func<CallSite, object, byte>> <>p__32;

		public static CallSite<Func<CallSite, object, string, object>> <>p__33;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__34;

		public static CallSite<Func<CallSite, object, byte>> <>p__35;

		public static CallSite<Func<CallSite, object, string, object>> <>p__36;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__37;

		public static CallSite<Func<CallSite, object, byte>> <>p__38;

		public static CallSite<Func<CallSite, object, string, object>> <>p__39;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__40;

		public static CallSite<Func<CallSite, object, byte>> <>p__41;

		public static CallSite<Func<CallSite, object, string, object>> <>p__42;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__43;

		public static CallSite<Func<CallSite, object, byte>> <>p__44;

		public static CallSite<Func<CallSite, object, string, object>> <>p__45;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__46;

		public static CallSite<Func<CallSite, object, byte>> <>p__47;

		public static CallSite<Func<CallSite, object, string, object>> <>p__48;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__49;

		public static CallSite<Func<CallSite, object, byte>> <>p__50;

		public static CallSite<Func<CallSite, object, string, object>> <>p__51;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__52;

		public static CallSite<Func<CallSite, object, byte>> <>p__53;

		public static CallSite<Func<CallSite, object, string, object>> <>p__54;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__55;

		public static CallSite<Func<CallSite, object, byte>> <>p__56;

		public static CallSite<Func<CallSite, object, string, object>> <>p__57;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__58;

		public static CallSite<Func<CallSite, object, byte>> <>p__59;

		public static CallSite<Func<CallSite, object, string, object>> <>p__60;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__61;

		public static CallSite<Func<CallSite, object, byte>> <>p__62;

		public static CallSite<Func<CallSite, object, string, object>> <>p__63;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__64;

		public static CallSite<Func<CallSite, object, byte>> <>p__65;

		public static CallSite<Func<CallSite, object, string, object>> <>p__66;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__67;

		public static CallSite<Func<CallSite, object, byte>> <>p__68;

		public static CallSite<Func<CallSite, object, string, object>> <>p__69;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__70;

		public static CallSite<Func<CallSite, object, byte>> <>p__71;

		public static CallSite<Func<CallSite, object, string, object>> <>p__72;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__73;

		public static CallSite<Func<CallSite, object, byte>> <>p__74;

		public static CallSite<Func<CallSite, object, string, object>> <>p__75;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__76;

		public static CallSite<Func<CallSite, object, byte>> <>p__77;

		public static CallSite<Func<CallSite, object, string, object>> <>p__78;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__79;

		public static CallSite<Func<CallSite, object, byte>> <>p__80;

		public static CallSite<Func<CallSite, object, string, object>> <>p__81;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__82;

		public static CallSite<Func<CallSite, object, byte>> <>p__83;

		public static CallSite<Func<CallSite, object, string, object>> <>p__84;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__85;

		public static CallSite<Func<CallSite, object, byte>> <>p__86;

		public static CallSite<Func<CallSite, object, string, object>> <>p__87;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__88;

		public static CallSite<Func<CallSite, object, byte>> <>p__89;

		public static CallSite<Func<CallSite, object, string, object>> <>p__90;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__91;

		public static CallSite<Func<CallSite, object, byte>> <>p__92;

		public static CallSite<Func<CallSite, object, string, object>> <>p__93;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__94;

		public static CallSite<Func<CallSite, object, byte>> <>p__95;
	}

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	private string m_path = ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_IO_FileInfo_get_DirectoryName__98(new FileInfo(ResolvedDelegates.System_Reflection_Assembly_get_Location__97(ResolvedDelegates.System_Reflection_Assembly_GetExecutingAssembly__96()))), "\\UserFanTables");

	private FanTable1p5 DefaultFanTable_Gaming = new FanTable1p5(16u, "DefaultFanTable_Gaming");

	private FanTable1p5 DefaultFanTable_Office = new FanTable1p5(16u, "DefaultFanTable_Office");

	private FanTable1p5 DefaultFanTable_Turbo = new FanTable1p5(16u, "DefaultFanTable_Turbo");

	private FanTable1p5 M1T1;

	private FanTable1p5 M1T2;

	private FanTable1p5 M1T3;

	private FanTable1p5 M1T4;

	private FanTable1p5 M1T5;

	private FanTable1p5 M2T1;

	private FanTable1p5 M2T2;

	private FanTable1p5 M2T3;

	private FanTable1p5 M2T4;

	private FanTable1p5 M2T5;

	private FanTable1p5 M3T1;

	private FanTable1p5 M3T2;

	private FanTable1p5 M3T3;

	private FanTable1p5 M3T4;

	private FanTable1p5 M3T5;

	private FanTable1p5 M4T1;

	private FanTable1p5 M4T2;

	private FanTable1p5 M4T3;

	private FanTable1p5 M4T4;

	private FanTable1p5 M4T5;

	private bool bInit;

	private bool m_IsNvGpu;

	private List<string> FileNames = new List<string>
	{
		"M1T1",
		"M1T2",
		"M1T3",
		"M1T4",
		"M1T5",
		"M2T1",
		"M2T2",
		"M2T3",
		"M2T4",
		"M2T5",
		"M3T1",
		"M3T2",
		"M3T3",
		"M3T4",
		"M3T5",
		"M4T1",
		"M4T2",
		"M4T3",
		"M4T4",
		"M4T5"
	};

	[MethodImpl(MethodImplOptions.NoInlining)]
	public FanTable_Manager1p5()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
		m_IsNvGpu = UtilityExtensions.IsNvGpu();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual bool GetFanTableInit()
	{
		return bInit;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void FanTable_Init()
	{
		RefreshFanTable(ref M1T1, "M1T1");
		RefreshFanTable(ref M1T2, "M1T2");
		RefreshFanTable(ref M1T3, "M1T3");
		RefreshFanTable(ref M1T4, "M1T4");
		RefreshFanTable(ref M1T5, "M1T5");
		RefreshFanTable(ref M2T1, "M2T1");
		RefreshFanTable(ref M2T2, "M2T2");
		RefreshFanTable(ref M2T3, "M2T3");
		RefreshFanTable(ref M2T4, "M2T4");
		RefreshFanTable(ref M2T5, "M2T5");
		RefreshFanTable(ref M3T1, "M3T1");
		RefreshFanTable(ref M3T2, "M3T2");
		RefreshFanTable(ref M3T3, "M3T3");
		RefreshFanTable(ref M3T4, "M3T4");
		RefreshFanTable(ref M3T5, "M3T5");
		RefreshFanTable(ref M4T1, "M4T1");
		RefreshFanTable(ref M4T2, "M4T2");
		RefreshFanTable(ref M4T3, "M4T3");
		RefreshFanTable(ref M4T4, "M4T4");
		RefreshFanTable(ref M4T5, "M4T5");
		if (RegistryCtrl.IsNewEcVersion(new HardwareInfoCollect().getECInfo()))
		{
			LogCtrl.TraceMessage("EC is the different version, then load EC default fan table.", "FanTable_Init", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\FanTable\\FanTable_Manager1p5.cs", 99);
			FanTable_Refresh();
			return;
		}
		bool flag = true;
		foreach (string fileName in FileNames)
		{
			if (!CheckFile(fileName))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			LogCtrl.TraceMessage("EC is the same version, then load local fan table.", "FanTable_Init", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\FanTable\\FanTable_Manager1p5.cs", 118);
			DefaultFanTable_Office = LoadFanTableFromJson("DefaultFanTable_Office");
			DefaultFanTable_Gaming = LoadFanTableFromJson("DefaultFanTable_Gaming");
			DefaultFanTable_Turbo = LoadFanTableFromJson("DefaultFanTable_Turbo");
			if (IsInvalidDefaultFanTable())
			{
				LogCtrl.TraceMessage("The local data is invalid, then load EC default fan table.", "FanTable_Init", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\FanTable\\FanTable_Manager1p5.cs", 125);
				DefaultFanTable_Office = new FanTable1p5(16u, "DefaultFanTable_Office");
				DefaultFanTable_Gaming = new FanTable1p5(16u, "DefaultFanTable_Gaming");
				DefaultFanTable_Turbo = new FanTable1p5(16u, "DefaultFanTable_Turbo");
				FanTable_Refresh();
				return;
			}
			if (IsInvalidDefaultFanTableAllZero())
			{
				LogCtrl.TraceMessage("The local data is all zero , then load EC default fan table.", "FanTable_Init", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\FanTable\\FanTable_Manager1p5.cs", 133);
				FanTable_Refresh();
				return;
			}
			M1T1 = LoadFanTableFromJson(M1T1.Name);
			M1T2 = LoadFanTableFromJson(M1T2.Name);
			M1T3 = LoadFanTableFromJson(M1T3.Name);
			M1T4 = LoadFanTableFromJson(M1T4.Name);
			M1T5 = LoadFanTableFromJson(M1T5.Name);
			M2T1 = LoadFanTableFromJson(M2T1.Name);
			M2T2 = LoadFanTableFromJson(M2T2.Name);
			M2T3 = LoadFanTableFromJson(M2T3.Name);
			M2T4 = LoadFanTableFromJson(M2T4.Name);
			M2T5 = LoadFanTableFromJson(M2T5.Name);
			M3T1 = LoadFanTableFromJson(M3T1.Name);
			M3T2 = LoadFanTableFromJson(M3T2.Name);
			M3T3 = LoadFanTableFromJson(M3T3.Name);
			M3T4 = LoadFanTableFromJson(M3T4.Name);
			M3T5 = LoadFanTableFromJson(M3T5.Name);
			M4T1 = LoadFanTableFromJson(M4T1.Name);
			M4T2 = LoadFanTableFromJson(M4T2.Name);
			M4T3 = LoadFanTableFromJson(M4T3.Name);
			M4T4 = LoadFanTableFromJson(M4T4.Name);
			M4T5 = LoadFanTableFromJson(M4T5.Name);
		}
		else
		{
			LogCtrl.TraceMessage("EC is the same version but local fan table is not found, then load EC default fan table.", "FanTable_Init", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\FanTable\\FanTable_Manager1p5.cs", 166);
			FanTable_Refresh();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool CheckFanTable()
	{
		throw new NotImplementedException();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool IsInvalidDefaultFanTable()
	{
		try
		{
			bool result = false;
			for (int i = 0; i <= 14; i++)
			{
				if (DefaultFanTable_Office.CPU[i].Duty > DefaultFanTable_Office.CPU[i + 1].Duty)
				{
					return true;
				}
				if (DefaultFanTable_Gaming.CPU[i].Duty > DefaultFanTable_Gaming.CPU[i + 1].Duty)
				{
					return true;
				}
				if (DefaultFanTable_Turbo.CPU[i].Duty > DefaultFanTable_Turbo.CPU[i + 1].Duty)
				{
					return true;
				}
				if (m_IsNvGpu)
				{
					if (DefaultFanTable_Office.GPU[i].Duty > DefaultFanTable_Office.GPU[i + 1].Duty)
					{
						return true;
					}
					if (DefaultFanTable_Gaming.GPU[i].Duty > DefaultFanTable_Gaming.GPU[i + 1].Duty)
					{
						return true;
					}
					if (DefaultFanTable_Turbo.GPU[i].Duty > DefaultFanTable_Turbo.GPU[i + 1].Duty)
					{
						return true;
					}
				}
			}
			return result;
		}
		catch
		{
			return true;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool IsInvalidDefaultFanTableAllZero()
	{
		bool result = false;
		for (int i = 0; i <= 14; i++)
		{
			if (DefaultFanTable_Office.CPU.All((FanTable1p5Buffer n) => n.Duty == 0))
			{
				return true;
			}
			if (DefaultFanTable_Gaming.CPU.All((FanTable1p5Buffer n) => n.Duty == 0))
			{
				return true;
			}
			if (DefaultFanTable_Turbo.CPU.All((FanTable1p5Buffer n) => n.Duty == 0))
			{
				return true;
			}
			if (m_IsNvGpu)
			{
				if (DefaultFanTable_Office.GPU.All((FanTable1p5Buffer n) => n.Duty == 0))
				{
					return true;
				}
				if (DefaultFanTable_Gaming.GPU.All((FanTable1p5Buffer n) => n.Duty == 0))
				{
					return true;
				}
				if (DefaultFanTable_Turbo.GPU.All((FanTable1p5Buffer n) => n.Duty == 0))
				{
					return true;
				}
			}
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void FanTable_Refresh()
	{
		bInit = RefreshDefaultFanTableAll();
		M1T1 = CopyFanTable(DefaultFanTable_Gaming, "M1T1");
		M1T2 = CopyFanTable(DefaultFanTable_Gaming, "M1T2");
		M1T3 = CopyFanTable(DefaultFanTable_Gaming, "M1T3");
		M1T4 = CopyFanTable(DefaultFanTable_Gaming, "M1T4");
		M1T5 = CopyFanTable(DefaultFanTable_Gaming, "M1T5");
		WriteFanTableToJson(M1T1, activated: false);
		WriteFanTableToJson(M1T2, activated: false);
		WriteFanTableToJson(M1T3, activated: false);
		WriteFanTableToJson(M1T4, activated: false);
		WriteFanTableToJson(M1T5, activated: false);
		M2T1 = CopyFanTable(DefaultFanTable_Office, "M2T1");
		M2T2 = CopyFanTable(DefaultFanTable_Office, "M2T2");
		M2T3 = CopyFanTable(DefaultFanTable_Office, "M2T3");
		M2T4 = CopyFanTable(DefaultFanTable_Office, "M2T4");
		M2T5 = CopyFanTable(DefaultFanTable_Office, "M2T5");
		WriteFanTableToJson(M2T1, activated: false);
		WriteFanTableToJson(M2T2, activated: false);
		WriteFanTableToJson(M2T3, activated: false);
		WriteFanTableToJson(M2T4, activated: false);
		WriteFanTableToJson(M2T5, activated: false);
		M3T1 = CopyFanTable(DefaultFanTable_Turbo, "M3T1");
		M3T2 = CopyFanTable(DefaultFanTable_Turbo, "M3T2");
		M3T3 = CopyFanTable(DefaultFanTable_Turbo, "M3T3");
		M3T4 = CopyFanTable(DefaultFanTable_Turbo, "M3T4");
		M3T5 = CopyFanTable(DefaultFanTable_Turbo, "M3T5");
		WriteFanTableToJson(M3T1, activated: false);
		WriteFanTableToJson(M3T2, activated: false);
		WriteFanTableToJson(M3T3, activated: false);
		WriteFanTableToJson(M3T4, activated: false);
		WriteFanTableToJson(M3T5, activated: false);
		M4T1 = CopyFanTable(DefaultFanTable_Gaming, "M4T1");
		M4T2 = CopyFanTable(DefaultFanTable_Gaming, "M4T2");
		M4T3 = CopyFanTable(DefaultFanTable_Gaming, "M4T3");
		M4T4 = CopyFanTable(DefaultFanTable_Gaming, "M4T4");
		M4T5 = CopyFanTable(DefaultFanTable_Gaming, "M4T5");
		WriteFanTableToJson(M4T1, activated: false);
		WriteFanTableToJson(M4T2, activated: false);
		WriteFanTableToJson(M4T3, activated: false);
		WriteFanTableToJson(M4T4, activated: false);
		WriteFanTableToJson(M4T5, activated: false);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void SetFanControlRespective(dynamic data)
	{
		try
		{
			if (<>o__35.<>p__6 == null)
			{
				<>o__35.<>p__6 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target = <>o__35.<>p__6.Target;
			CallSite<Func<CallSite, object, bool>> arg = <>o__35.<>p__6;
			if (<>o__35.<>p__1 == null)
			{
				<>o__35.<>p__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target2 = <>o__35.<>p__1.Target;
			CallSite<Func<CallSite, object, object, object>> arg2 = <>o__35.<>p__1;
			if (<>o__35.<>p__0 == null)
			{
				<>o__35.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			object obj = target2(arg2, <>o__35.<>p__0.Target(<>o__35.<>p__0, (object)data, "Name"), null);
			if (<>o__35.<>p__5 == null)
			{
				<>o__35.<>p__5 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object arg5;
			if (!<>o__35.<>p__5.Target(<>o__35.<>p__5, obj))
			{
				if (<>o__35.<>p__4 == null)
				{
					<>o__35.<>p__4 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, object, object, object> target3 = <>o__35.<>p__4.Target;
				CallSite<Func<CallSite, object, object, object>> arg3 = <>o__35.<>p__4;
				if (<>o__35.<>p__3 == null)
				{
					<>o__35.<>p__3 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				Func<CallSite, object, object, object> target4 = <>o__35.<>p__3.Target;
				CallSite<Func<CallSite, object, object, object>> arg4 = <>o__35.<>p__3;
				if (<>o__35.<>p__2 == null)
				{
					<>o__35.<>p__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				arg5 = target3(arg3, obj, target4(arg4, <>o__35.<>p__2.Target(<>o__35.<>p__2, (object)data, "FanControlRespective"), null));
			}
			else
			{
				arg5 = obj;
			}
			if (target(arg, arg5))
			{
				if (<>o__35.<>p__9 == null)
				{
					<>o__35.<>p__9 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
				}
				Func<CallSite, object, string> target5 = <>o__35.<>p__9.Target;
				CallSite<Func<CallSite, object, string>> arg6 = <>o__35.<>p__9;
				if (<>o__35.<>p__8 == null)
				{
					<>o__35.<>p__8 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target6 = <>o__35.<>p__8.Target;
				CallSite<Func<CallSite, Type, object, object>> arg7 = <>o__35.<>p__8;
				Type arg8 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__35.<>p__7 == null)
				{
					<>o__35.<>p__7 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				string name = target5(arg6, target6(arg7, arg8, <>o__35.<>p__7.Target(<>o__35.<>p__7, (object)data, "Name")));
				if (<>o__35.<>p__12 == null)
				{
					<>o__35.<>p__12 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(bool).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
				}
				Func<CallSite, object, bool> target7 = <>o__35.<>p__12.Target;
				CallSite<Func<CallSite, object, bool>> arg9 = <>o__35.<>p__12;
				if (<>o__35.<>p__11 == null)
				{
					<>o__35.<>p__11 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToBoolean", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target8 = <>o__35.<>p__11.Target;
				CallSite<Func<CallSite, Type, object, object>> arg10 = <>o__35.<>p__11;
				Type arg11 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__35.<>p__10 == null)
				{
					<>o__35.<>p__10 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				bool flag = target7(arg9, target8(arg10, arg11, <>o__35.<>p__10.Target(<>o__35.<>p__10, (object)data, "FanControlRespective")));
				SetFanTableActivated(name, activated: true);
				RefreshFanControlRespective(name, flag);
				SetEcFanControlRespective(flag);
				WriteFanTableToJson(TableTransform(name), activated: true);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception: ", ResolvedDelegates.System_Exception_get_Message__58(ex)), "SetFanControlRespective", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\FanTable\\FanTable_Manager1p5.cs", 436);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void GetFanTable(string name)
	{
		FanTable1p5 fanTable1p = default;
		fanTable1p = TableTransform(name);
		App.m_MQTTService.Publish("Fan/Table", fanTable1p, retain: false);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void SetFanTable(string name)
	{
		SetEcFanControlRespective(TableTransform(name).FanControlRespective);
		SetEcFanTable(TableTransform(name));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void SetFanTableSetting(dynamic data, bool saveEC)
	{
		try
		{
			if (<>o__38.<>p__6 == null)
			{
				<>o__38.<>p__6 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target = <>o__38.<>p__6.Target;
			CallSite<Func<CallSite, object, bool>> arg = <>o__38.<>p__6;
			if (<>o__38.<>p__1 == null)
			{
				<>o__38.<>p__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target2 = <>o__38.<>p__1.Target;
			CallSite<Func<CallSite, object, object, object>> arg2 = <>o__38.<>p__1;
			if (<>o__38.<>p__0 == null)
			{
				<>o__38.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			object obj = target2(arg2, <>o__38.<>p__0.Target(<>o__38.<>p__0, (object)data, "Name"), null);
			if (<>o__38.<>p__5 == null)
			{
				<>o__38.<>p__5 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object arg5;
			if (!<>o__38.<>p__5.Target(<>o__38.<>p__5, obj))
			{
				if (<>o__38.<>p__4 == null)
				{
					<>o__38.<>p__4 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, object, object, object> target3 = <>o__38.<>p__4.Target;
				CallSite<Func<CallSite, object, object, object>> arg3 = <>o__38.<>p__4;
				if (<>o__38.<>p__3 == null)
				{
					<>o__38.<>p__3 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				Func<CallSite, object, object, object> target4 = <>o__38.<>p__3.Target;
				CallSite<Func<CallSite, object, object, object>> arg4 = <>o__38.<>p__3;
				if (<>o__38.<>p__2 == null)
				{
					<>o__38.<>p__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				arg5 = target3(arg3, obj, target4(arg4, <>o__38.<>p__2.Target(<>o__38.<>p__2, (object)data, "Type"), null));
			}
			else
			{
				arg5 = obj;
			}
			if (target(arg, arg5))
			{
				if (<>o__38.<>p__9 == null)
				{
					<>o__38.<>p__9 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
				}
				Func<CallSite, object, string> target5 = <>o__38.<>p__9.Target;
				CallSite<Func<CallSite, object, string>> arg6 = <>o__38.<>p__9;
				if (<>o__38.<>p__8 == null)
				{
					<>o__38.<>p__8 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target6 = <>o__38.<>p__8.Target;
				CallSite<Func<CallSite, Type, object, object>> arg7 = <>o__38.<>p__8;
				Type arg8 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__38.<>p__7 == null)
				{
					<>o__38.<>p__7 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				string name = target5(arg6, target6(arg7, arg8, <>o__38.<>p__7.Target(<>o__38.<>p__7, (object)data, "Name")));
				if (<>o__38.<>p__12 == null)
				{
					<>o__38.<>p__12 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
				}
				Func<CallSite, object, string> target7 = <>o__38.<>p__12.Target;
				CallSite<Func<CallSite, object, string>> arg9 = <>o__38.<>p__12;
				if (<>o__38.<>p__11 == null)
				{
					<>o__38.<>p__11 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target8 = <>o__38.<>p__11.Target;
				CallSite<Func<CallSite, Type, object, object>> arg10 = <>o__38.<>p__11;
				Type arg11 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__38.<>p__10 == null)
				{
					<>o__38.<>p__10 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				string arg12 = target7(arg9, target8(arg10, arg11, <>o__38.<>p__10.Target(<>o__38.<>p__10, (object)data, "Type")));
				SetFanTableActivated(name, activated: true);
				if (<>o__38.<>p__13 == null)
				{
					<>o__38.<>p__13 = CallSite<Action<CallSite, FanTable_Manager1p5, FanTable1p5, string, object, bool>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "SetFanTableSetting", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[5]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
					}));
				}
				<>o__38.<>p__13.Target(<>o__38.<>p__13, this, TableTransform(name), arg12, (object)data, saveEC);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception: ", ResolvedDelegates.System_Exception_get_Message__58(ex)), "SetFanTableSetting", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\FanTable\\FanTable_Manager1p5.cs", 481);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void RestoreDefaultFanTableAll()
	{
		M1T1 = CopyFanTable(DefaultFanTable_Gaming, "M1T1");
		M1T2 = CopyFanTable(DefaultFanTable_Gaming, "M1T2");
		M1T3 = CopyFanTable(DefaultFanTable_Gaming, "M1T3");
		M1T4 = CopyFanTable(DefaultFanTable_Gaming, "M1T4");
		M1T5 = CopyFanTable(DefaultFanTable_Gaming, "M1T5");
		WriteFanTableToJson(M1T1, activated: false);
		WriteFanTableToJson(M1T2, activated: false);
		WriteFanTableToJson(M1T3, activated: false);
		WriteFanTableToJson(M1T4, activated: false);
		WriteFanTableToJson(M1T5, activated: false);
		M2T1 = CopyFanTable(DefaultFanTable_Office, "M2T1");
		M2T2 = CopyFanTable(DefaultFanTable_Office, "M2T2");
		M2T3 = CopyFanTable(DefaultFanTable_Office, "M2T3");
		M2T4 = CopyFanTable(DefaultFanTable_Office, "M2T4");
		M2T5 = CopyFanTable(DefaultFanTable_Office, "M2T5");
		WriteFanTableToJson(M2T1, activated: false);
		WriteFanTableToJson(M2T2, activated: false);
		WriteFanTableToJson(M2T3, activated: false);
		WriteFanTableToJson(M2T4, activated: false);
		WriteFanTableToJson(M2T5, activated: false);
		M3T1 = CopyFanTable(DefaultFanTable_Turbo, "M3T1");
		M3T2 = CopyFanTable(DefaultFanTable_Turbo, "M3T2");
		M3T3 = CopyFanTable(DefaultFanTable_Turbo, "M3T3");
		M3T4 = CopyFanTable(DefaultFanTable_Turbo, "M3T4");
		M3T5 = CopyFanTable(DefaultFanTable_Turbo, "M3T5");
		WriteFanTableToJson(M3T1, activated: false);
		WriteFanTableToJson(M3T2, activated: false);
		WriteFanTableToJson(M3T3, activated: false);
		WriteFanTableToJson(M3T4, activated: false);
		WriteFanTableToJson(M3T5, activated: false);
		M4T1 = CopyFanTable(DefaultFanTable_Gaming, "M4T1");
		M4T2 = CopyFanTable(DefaultFanTable_Gaming, "M4T2");
		M4T3 = CopyFanTable(DefaultFanTable_Gaming, "M4T3");
		M4T4 = CopyFanTable(DefaultFanTable_Gaming, "M4T4");
		M4T5 = CopyFanTable(DefaultFanTable_Gaming, "M4T5");
		WriteFanTableToJson(M4T1, activated: false);
		WriteFanTableToJson(M4T2, activated: false);
		WriteFanTableToJson(M4T3, activated: false);
		WriteFanTableToJson(M4T4, activated: false);
		WriteFanTableToJson(M4T5, activated: false);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe virtual void RestoreDefaultFanTable(dynamic data)
	{
		if (<>o__40.<>p__2 == null)
		{
			<>o__40.<>p__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target = <>o__40.<>p__2.Target;
		CallSite<Func<CallSite, object, bool>> arg = <>o__40.<>p__2;
		if (<>o__40.<>p__1 == null)
		{
			<>o__40.<>p__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		Func<CallSite, object, object, object> target2 = <>o__40.<>p__1.Target;
		CallSite<Func<CallSite, object, object, object>> arg2 = <>o__40.<>p__1;
		if (<>o__40.<>p__0 == null)
		{
			<>o__40.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		if (!target(arg, target2(arg2, <>o__40.<>p__0.Target(<>o__40.<>p__0, (object)data, "Name"), null)))
		{
			return;
		}
		if (<>o__40.<>p__5 == null)
		{
			<>o__40.<>p__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
		}
		Func<CallSite, object, string> target3 = <>o__40.<>p__5.Target;
		CallSite<Func<CallSite, object, string>> arg3 = <>o__40.<>p__5;
		if (<>o__40.<>p__4 == null)
		{
			<>o__40.<>p__4 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
			}));
		}
		Func<CallSite, Type, object, object> target4 = <>o__40.<>p__4.Target;
		CallSite<Func<CallSite, Type, object, object>> arg4 = <>o__40.<>p__4;
		Type arg5 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
		if (<>o__40.<>p__3 == null)
		{
			<>o__40.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		string text = target3(arg3, target4(arg4, arg5, <>o__40.<>p__3.Target(<>o__40.<>p__3, (object)data, "Name")));
		FanTable1p5 fanTable1p = default;
		if (text != null)
		{
			int num = ResolvedDelegates.System_String_get_Length__298(ref *(string*)text);
			if (num == 4)
			{
				switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)text, 3))
				{
				case '1':
					if (!ResolvedDelegates.System_String_op_Equality__18(text, "M1T1"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(text, "M2T1"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(text, "M3T1"))
							{
								if (ResolvedDelegates.System_String_op_Equality__18(text, "M4T1"))
								{
									M4T1 = CopyFanTable(DefaultFanTable_Turbo, text);
									fanTable1p = M4T1;
								}
							}
							else
							{
								M3T1 = CopyFanTable(DefaultFanTable_Turbo, text);
								fanTable1p = M3T1;
							}
						}
						else
						{
							M2T1 = CopyFanTable(DefaultFanTable_Office, text);
							fanTable1p = M2T1;
						}
					}
					else
					{
						M1T1 = CopyFanTable(DefaultFanTable_Gaming, text);
						fanTable1p = M1T1;
					}
					break;
				case '2':
					if (!ResolvedDelegates.System_String_op_Equality__18(text, "M1T2"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(text, "M2T2"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(text, "M3T2"))
							{
								if (ResolvedDelegates.System_String_op_Equality__18(text, "M4T2"))
								{
									M4T2 = CopyFanTable(DefaultFanTable_Turbo, text);
									fanTable1p = M4T2;
								}
							}
							else
							{
								M3T2 = CopyFanTable(DefaultFanTable_Turbo, text);
								fanTable1p = M3T2;
							}
						}
						else
						{
							M2T2 = CopyFanTable(DefaultFanTable_Office, text);
							fanTable1p = M2T2;
						}
					}
					else
					{
						M1T2 = CopyFanTable(DefaultFanTable_Gaming, text);
						fanTable1p = M1T2;
					}
					break;
				case '3':
					if (!ResolvedDelegates.System_String_op_Equality__18(text, "M1T3"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(text, "M2T3"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(text, "M3T3"))
							{
								if (ResolvedDelegates.System_String_op_Equality__18(text, "M4T3"))
								{
									M4T3 = CopyFanTable(DefaultFanTable_Turbo, text);
									fanTable1p = M4T3;
								}
							}
							else
							{
								M3T3 = CopyFanTable(DefaultFanTable_Turbo, text);
								fanTable1p = M3T3;
							}
						}
						else
						{
							M2T3 = CopyFanTable(DefaultFanTable_Office, text);
							fanTable1p = M2T3;
						}
					}
					else
					{
						M1T3 = CopyFanTable(DefaultFanTable_Gaming, text);
						fanTable1p = M1T3;
					}
					break;
				case '4':
					if (!ResolvedDelegates.System_String_op_Equality__18(text, "M1T4"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(text, "M2T4"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(text, "M3T4"))
							{
								if (ResolvedDelegates.System_String_op_Equality__18(text, "M4T4"))
								{
									M4T4 = CopyFanTable(DefaultFanTable_Turbo, text);
									fanTable1p = M4T4;
								}
							}
							else
							{
								M3T4 = CopyFanTable(DefaultFanTable_Turbo, text);
								fanTable1p = M3T4;
							}
						}
						else
						{
							M2T4 = CopyFanTable(DefaultFanTable_Office, text);
							fanTable1p = M2T4;
						}
					}
					else
					{
						M1T4 = CopyFanTable(DefaultFanTable_Gaming, text);
						fanTable1p = M1T4;
					}
					break;
				case '5':
					if (!ResolvedDelegates.System_String_op_Equality__18(text, "M1T5"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(text, "M2T5"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(text, "M3T5"))
							{
								if (ResolvedDelegates.System_String_op_Equality__18(text, "M4T5"))
								{
									M4T5 = CopyFanTable(DefaultFanTable_Turbo, text);
									fanTable1p = M4T5;
								}
							}
							else
							{
								M3T5 = CopyFanTable(DefaultFanTable_Turbo, text);
								fanTable1p = M3T5;
							}
						}
						else
						{
							M2T5 = CopyFanTable(DefaultFanTable_Office, text);
							fanTable1p = M2T5;
						}
					}
					else
					{
						M1T5 = CopyFanTable(DefaultFanTable_Gaming, text);
						fanTable1p = M1T5;
					}
					break;
				}
			}
		}
		if (fanTable1p.CPU != null && fanTable1p.GPU != null)
		{
			WriteFanTableToJson(fanTable1p, activated: false);
			SetEcFanControlRespective(fanTable1p.FanControlRespective);
			SetEcFanTable(fanTable1p);
			App.m_MQTTService.Publish("Fan/Table", fanTable1p, retain: false);
		}
	}

	public virtual void GetPLDefaultValue(string name, ref string PL1, ref string PL2, ref string PL1_dc, ref string PL2_dc)
	{
		PL1 = "NA";
		PL2 = "NA";
		PL1_dc = "NA";
		PL2_dc = "NA";
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void GetGpuFeatureDefaultValue(string name, ref string CTGP, ref string DB, ref string WM)
	{
		CTGP = "NA";
		DB = "NA";
		WM = "NA";
	}

	public virtual void GetFeatureDefaultValue_BALANCED_PROTECT(string name, ref string CTGP2, ref string DB2, ref string PL1_S2, ref string PL2_S2)
	{
		CTGP2 = "NA";
		DB2 = "NA";
		PL1_S2 = "NA";
		PL2_S2 = "NA";
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void RefreshFanTable(ref FanTable1p5 fantable, string name)
	{
		fantable.Name = name;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void LoadOfficeFanTables()
	{
		M2T1 = GenerateFanDuty(DefaultFanTable_Office, 0.8, "M2T1");
		M2T2 = GenerateFanDuty(DefaultFanTable_Office, 0.9, "M2T2");
		M2T3 = CopyFanTable(DefaultFanTable_Office, "M2T3");
		M2T4 = GenerateFanDuty(DefaultFanTable_Office, 1.1, "M2T4");
		M2T5 = GenerateFanDuty(DefaultFanTable_Office, 1.2, "M2T5");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool CheckFile(string _fileName)
	{
		bool result = false;
		try
		{
			if (ResolvedDelegates.System_IO_File_Exists__72(ResolvedDelegates.System_String_Concat__120(m_path, "\\", _fileName, ".json")))
			{
				result = true;
			}
		}
		catch
		{
			result = false;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void WriteFanTableToJson(FanTable1p5 fantable, bool activated)
	{
		try
		{
			string path = ResolvedDelegates.System_String_Concat__120(m_path, "\\", fantable.Name, ".json");
			if (!ResolvedDelegates.System_IO_Directory_Exists__283(m_path))
			{
				ResolvedDelegates.System_IO_Directory_CreateDirectory__232(m_path);
			}
			fantable.Activated = activated;
			string contents = JsonConvert.SerializeObject((object)fantable, (Formatting)1);
			File.WriteAllText(path, contents);
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception: ", ResolvedDelegates.System_Exception_get_Message__58(ex)), "WriteFanTableToJson", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\FanTable\\FanTable_Manager1p5.cs", 719);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private FanTable1p5 LoadFanTableFromJson(string fileName)
	{
		FanTable1p5 result = default;
		object obj = LocalFileLoader.LoadJsonFromPath_NonAsync<object>(m_path, fileName, ResolvedDelegates.System_Reflection_Assembly_GetExecutingAssembly__96());
		if (<>o__48.<>p__1 == null)
		{
			<>o__48.<>p__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target = <>o__48.<>p__1.Target;
		CallSite<Func<CallSite, object, bool>> arg = <>o__48.<>p__1;
		if (<>o__48.<>p__0 == null)
		{
			<>o__48.<>p__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		if (target(arg, <>o__48.<>p__0.Target(<>o__48.<>p__0, obj, null)))
		{
			if (<>o__48.<>p__3 == null)
			{
				<>o__48.<>p__3 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, string> target2 = <>o__48.<>p__3.Target;
			CallSite<Func<CallSite, object, string>> arg2 = <>o__48.<>p__3;
			if (<>o__48.<>p__2 == null)
			{
				<>o__48.<>p__2 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Stringify", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			return JsonConvert.DeserializeObject<FanTable1p5>(target2(arg2, <>o__48.<>p__2.Target(<>o__48.<>p__2, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Json).TypeHandle), obj)));
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GetEcFanTable(ref FanTable1p5 fantable)
	{
		byte Data = 0;
		for (int i = 0; i <= 15; i++)
		{
			if (i == 0)
			{
				fantable.CPU[i].UpT = 0;
			}
			else
			{
				EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3840 + i - 1), ref Data);
				fantable.CPU[i].UpT = Data;
			}
			if (i < 15)
			{
				EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3856 + i + 1), ref Data);
				fantable.CPU[i].DownT = Data;
			}
			else
			{
				EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3856 + i), ref Data);
				fantable.CPU[i].DownT = Data;
			}
			EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3872 + i), ref Data);
			fantable.CPU[i].Duty = (byte)(Data / 2);
		}
		if (!m_IsNvGpu)
		{
			return;
		}
		for (int j = 0; j <= 15; j++)
		{
			if (j == 0)
			{
				fantable.GPU[j].UpT = 0;
			}
			else
			{
				EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3888 + j - 1), ref Data);
				fantable.GPU[j].UpT = Data;
			}
			if (j < 15)
			{
				EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3904 + j + 1), ref Data);
				fantable.GPU[j].DownT = Data;
			}
			else
			{
				EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3904 + j), ref Data);
				fantable.GPU[j].DownT = Data;
			}
			EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3920 + j), ref Data);
			fantable.GPU[j].Duty = (byte)(Data / 2);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetEcFanTable(FanTable1p5 fantable)
	{
		if (fantable.CPU != null)
		{
			for (int i = 0; i <= 15; i++)
			{
				if (i < 15)
				{
					EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3840 + i), fantable.CPU[i + 1].UpT);
				}
				else
				{
					EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3840 + i), byte.MaxValue);
				}
				if (i < 15)
				{
					EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3856 + i + 1), fantable.CPU[i].DownT);
				}
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3872 + i), (byte)(fantable.CPU[i].Duty * 2));
			}
		}
		if (fantable.GPU == null)
		{
			return;
		}
		for (int j = 0; j <= 15; j++)
		{
			if (j < 15)
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3888 + j), fantable.GPU[j + 1].UpT);
			}
			else
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3888 + j), byte.MaxValue);
			}
			if (j < 15)
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3904 + j + 1), fantable.GPU[j].DownT);
			}
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3920 + j), (byte)(fantable.GPU[j].Duty * 2));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetEcFanTable_Cpu(FanTable1p5 fantable)
	{
		if (fantable.CPU == null)
		{
			return;
		}
		for (int i = 0; i <= 15; i++)
		{
			if (i < 15)
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3840 + i), fantable.CPU[i + 1].UpT);
			}
			else
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3840 + i), byte.MaxValue);
			}
			if (i < 15)
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3856 + i + 1), fantable.CPU[i].DownT);
			}
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3872 + i), (byte)(fantable.CPU[i].Duty * 2));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetEcFanTable_Gpu(FanTable1p5 fantable)
	{
		if (fantable.GPU == null)
		{
			return;
		}
		for (int i = 0; i <= 15; i++)
		{
			if (i < 15)
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3888 + i), fantable.GPU[i + 1].UpT);
			}
			else
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3888 + i), byte.MaxValue);
			}
			if (i < 15)
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3904 + i + 1), fantable.GPU[i].DownT);
			}
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3920 + i), (byte)(fantable.GPU[i].Duty * 2));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetFanTableSetting(FanTable1p5 fanTable, string type, int id, string key, int value)
	{
		if (id < 0 || id > 15 || fanTable.CPU == null || fanTable.GPU == null)
		{
			return;
		}
		if (ResolvedDelegates.System_String_op_Equality__18(type, "CPU"))
		{
			if (!ResolvedDelegates.System_String_op_Equality__18(key, "Duty"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(key, "UpT"))
				{
					if (ResolvedDelegates.System_String_op_Equality__18(key, "DownT"))
					{
						fanTable.CPU[id].DownT = (byte)value;
					}
				}
				else
				{
					fanTable.CPU[id].UpT = (byte)value;
				}
			}
			else
			{
				fanTable.CPU[id].Duty = (byte)value;
			}
		}
		else if (type == "GPU")
		{
			if (!ResolvedDelegates.System_String_op_Equality__18(key, "Duty"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(key, "UpT"))
				{
					if (ResolvedDelegates.System_String_op_Equality__18(key, "DownT"))
					{
						fanTable.GPU[id].DownT = (byte)value;
					}
				}
				else
				{
					fanTable.GPU[id].UpT = (byte)value;
				}
			}
			else
			{
				fanTable.GPU[id].Duty = (byte)value;
			}
		}
		WriteFanTableToJson(fanTable, activated: true);
		SetEcFanTable(fanTable);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetFanTableSetting(FanTable1p5 fanTable, string type, dynamic data, bool saveEC)
	{
		if (ResolvedDelegates.System_String_op_Equality__18(type, "CPU"))
		{
			ref FanTable1p5Buffer reference = ref fanTable.CPU[0];
			if (<>o__54.<>p__2 == null)
			{
				<>o__54.<>p__2 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target = <>o__54.<>p__2.Target;
			CallSite<Func<CallSite, object, byte>> arg = <>o__54.<>p__2;
			if (<>o__54.<>p__1 == null)
			{
				<>o__54.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target2 = <>o__54.<>p__1.Target;
			CallSite<Func<CallSite, Type, object, object>> arg2 = <>o__54.<>p__1;
			Type arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__0 == null)
			{
				<>o__54.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference.Duty = target(arg, target2(arg2, arg3, <>o__54.<>p__0.Target(<>o__54.<>p__0, (object)data, "T0")));
			ref FanTable1p5Buffer reference2 = ref fanTable.CPU[1];
			if (<>o__54.<>p__5 == null)
			{
				<>o__54.<>p__5 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target3 = <>o__54.<>p__5.Target;
			CallSite<Func<CallSite, object, byte>> arg4 = <>o__54.<>p__5;
			if (<>o__54.<>p__4 == null)
			{
				<>o__54.<>p__4 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target4 = <>o__54.<>p__4.Target;
			CallSite<Func<CallSite, Type, object, object>> arg5 = <>o__54.<>p__4;
			Type arg6 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__3 == null)
			{
				<>o__54.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference2.Duty = target3(arg4, target4(arg5, arg6, <>o__54.<>p__3.Target(<>o__54.<>p__3, (object)data, "T1")));
			ref FanTable1p5Buffer reference3 = ref fanTable.CPU[2];
			if (<>o__54.<>p__8 == null)
			{
				<>o__54.<>p__8 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target5 = <>o__54.<>p__8.Target;
			CallSite<Func<CallSite, object, byte>> arg7 = <>o__54.<>p__8;
			if (<>o__54.<>p__7 == null)
			{
				<>o__54.<>p__7 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target6 = <>o__54.<>p__7.Target;
			CallSite<Func<CallSite, Type, object, object>> arg8 = <>o__54.<>p__7;
			Type arg9 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__6 == null)
			{
				<>o__54.<>p__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference3.Duty = target5(arg7, target6(arg8, arg9, <>o__54.<>p__6.Target(<>o__54.<>p__6, (object)data, "T2")));
			ref FanTable1p5Buffer reference4 = ref fanTable.CPU[3];
			if (<>o__54.<>p__11 == null)
			{
				<>o__54.<>p__11 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target7 = <>o__54.<>p__11.Target;
			CallSite<Func<CallSite, object, byte>> arg10 = <>o__54.<>p__11;
			if (<>o__54.<>p__10 == null)
			{
				<>o__54.<>p__10 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target8 = <>o__54.<>p__10.Target;
			CallSite<Func<CallSite, Type, object, object>> arg11 = <>o__54.<>p__10;
			Type arg12 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__9 == null)
			{
				<>o__54.<>p__9 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference4.Duty = target7(arg10, target8(arg11, arg12, <>o__54.<>p__9.Target(<>o__54.<>p__9, (object)data, "T3")));
			ref FanTable1p5Buffer reference5 = ref fanTable.CPU[4];
			if (<>o__54.<>p__14 == null)
			{
				<>o__54.<>p__14 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target9 = <>o__54.<>p__14.Target;
			CallSite<Func<CallSite, object, byte>> arg13 = <>o__54.<>p__14;
			if (<>o__54.<>p__13 == null)
			{
				<>o__54.<>p__13 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target10 = <>o__54.<>p__13.Target;
			CallSite<Func<CallSite, Type, object, object>> arg14 = <>o__54.<>p__13;
			Type arg15 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__12 == null)
			{
				<>o__54.<>p__12 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference5.Duty = target9(arg13, target10(arg14, arg15, <>o__54.<>p__12.Target(<>o__54.<>p__12, (object)data, "T4")));
			ref FanTable1p5Buffer reference6 = ref fanTable.CPU[5];
			if (<>o__54.<>p__17 == null)
			{
				<>o__54.<>p__17 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target11 = <>o__54.<>p__17.Target;
			CallSite<Func<CallSite, object, byte>> arg16 = <>o__54.<>p__17;
			if (<>o__54.<>p__16 == null)
			{
				<>o__54.<>p__16 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target12 = <>o__54.<>p__16.Target;
			CallSite<Func<CallSite, Type, object, object>> arg17 = <>o__54.<>p__16;
			Type arg18 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__15 == null)
			{
				<>o__54.<>p__15 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference6.Duty = target11(arg16, target12(arg17, arg18, <>o__54.<>p__15.Target(<>o__54.<>p__15, (object)data, "T5")));
			ref FanTable1p5Buffer reference7 = ref fanTable.CPU[6];
			if (<>o__54.<>p__20 == null)
			{
				<>o__54.<>p__20 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target13 = <>o__54.<>p__20.Target;
			CallSite<Func<CallSite, object, byte>> arg19 = <>o__54.<>p__20;
			if (<>o__54.<>p__19 == null)
			{
				<>o__54.<>p__19 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target14 = <>o__54.<>p__19.Target;
			CallSite<Func<CallSite, Type, object, object>> arg20 = <>o__54.<>p__19;
			Type arg21 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__18 == null)
			{
				<>o__54.<>p__18 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference7.Duty = target13(arg19, target14(arg20, arg21, <>o__54.<>p__18.Target(<>o__54.<>p__18, (object)data, "T6")));
			ref FanTable1p5Buffer reference8 = ref fanTable.CPU[7];
			if (<>o__54.<>p__23 == null)
			{
				<>o__54.<>p__23 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target15 = <>o__54.<>p__23.Target;
			CallSite<Func<CallSite, object, byte>> arg22 = <>o__54.<>p__23;
			if (<>o__54.<>p__22 == null)
			{
				<>o__54.<>p__22 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target16 = <>o__54.<>p__22.Target;
			CallSite<Func<CallSite, Type, object, object>> arg23 = <>o__54.<>p__22;
			Type arg24 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__21 == null)
			{
				<>o__54.<>p__21 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference8.Duty = target15(arg22, target16(arg23, arg24, <>o__54.<>p__21.Target(<>o__54.<>p__21, (object)data, "T7")));
			ref FanTable1p5Buffer reference9 = ref fanTable.CPU[8];
			if (<>o__54.<>p__26 == null)
			{
				<>o__54.<>p__26 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target17 = <>o__54.<>p__26.Target;
			CallSite<Func<CallSite, object, byte>> arg25 = <>o__54.<>p__26;
			if (<>o__54.<>p__25 == null)
			{
				<>o__54.<>p__25 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target18 = <>o__54.<>p__25.Target;
			CallSite<Func<CallSite, Type, object, object>> arg26 = <>o__54.<>p__25;
			Type arg27 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__24 == null)
			{
				<>o__54.<>p__24 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference9.Duty = target17(arg25, target18(arg26, arg27, <>o__54.<>p__24.Target(<>o__54.<>p__24, (object)data, "T8")));
			ref FanTable1p5Buffer reference10 = ref fanTable.CPU[9];
			if (<>o__54.<>p__29 == null)
			{
				<>o__54.<>p__29 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target19 = <>o__54.<>p__29.Target;
			CallSite<Func<CallSite, object, byte>> arg28 = <>o__54.<>p__29;
			if (<>o__54.<>p__28 == null)
			{
				<>o__54.<>p__28 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target20 = <>o__54.<>p__28.Target;
			CallSite<Func<CallSite, Type, object, object>> arg29 = <>o__54.<>p__28;
			Type arg30 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__27 == null)
			{
				<>o__54.<>p__27 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference10.Duty = target19(arg28, target20(arg29, arg30, <>o__54.<>p__27.Target(<>o__54.<>p__27, (object)data, "T9")));
			ref FanTable1p5Buffer reference11 = ref fanTable.CPU[10];
			if (<>o__54.<>p__32 == null)
			{
				<>o__54.<>p__32 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target21 = <>o__54.<>p__32.Target;
			CallSite<Func<CallSite, object, byte>> arg31 = <>o__54.<>p__32;
			if (<>o__54.<>p__31 == null)
			{
				<>o__54.<>p__31 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target22 = <>o__54.<>p__31.Target;
			CallSite<Func<CallSite, Type, object, object>> arg32 = <>o__54.<>p__31;
			Type arg33 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__30 == null)
			{
				<>o__54.<>p__30 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference11.Duty = target21(arg31, target22(arg32, arg33, <>o__54.<>p__30.Target(<>o__54.<>p__30, (object)data, "T10")));
			ref FanTable1p5Buffer reference12 = ref fanTable.CPU[11];
			if (<>o__54.<>p__35 == null)
			{
				<>o__54.<>p__35 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target23 = <>o__54.<>p__35.Target;
			CallSite<Func<CallSite, object, byte>> arg34 = <>o__54.<>p__35;
			if (<>o__54.<>p__34 == null)
			{
				<>o__54.<>p__34 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target24 = <>o__54.<>p__34.Target;
			CallSite<Func<CallSite, Type, object, object>> arg35 = <>o__54.<>p__34;
			Type arg36 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__33 == null)
			{
				<>o__54.<>p__33 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference12.Duty = target23(arg34, target24(arg35, arg36, <>o__54.<>p__33.Target(<>o__54.<>p__33, (object)data, "T11")));
			ref FanTable1p5Buffer reference13 = ref fanTable.CPU[12];
			if (<>o__54.<>p__38 == null)
			{
				<>o__54.<>p__38 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target25 = <>o__54.<>p__38.Target;
			CallSite<Func<CallSite, object, byte>> arg37 = <>o__54.<>p__38;
			if (<>o__54.<>p__37 == null)
			{
				<>o__54.<>p__37 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target26 = <>o__54.<>p__37.Target;
			CallSite<Func<CallSite, Type, object, object>> arg38 = <>o__54.<>p__37;
			Type arg39 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__36 == null)
			{
				<>o__54.<>p__36 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference13.Duty = target25(arg37, target26(arg38, arg39, <>o__54.<>p__36.Target(<>o__54.<>p__36, (object)data, "T12")));
			ref FanTable1p5Buffer reference14 = ref fanTable.CPU[13];
			if (<>o__54.<>p__41 == null)
			{
				<>o__54.<>p__41 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target27 = <>o__54.<>p__41.Target;
			CallSite<Func<CallSite, object, byte>> arg40 = <>o__54.<>p__41;
			if (<>o__54.<>p__40 == null)
			{
				<>o__54.<>p__40 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target28 = <>o__54.<>p__40.Target;
			CallSite<Func<CallSite, Type, object, object>> arg41 = <>o__54.<>p__40;
			Type arg42 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__39 == null)
			{
				<>o__54.<>p__39 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference14.Duty = target27(arg40, target28(arg41, arg42, <>o__54.<>p__39.Target(<>o__54.<>p__39, (object)data, "T13")));
			ref FanTable1p5Buffer reference15 = ref fanTable.CPU[14];
			if (<>o__54.<>p__44 == null)
			{
				<>o__54.<>p__44 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target29 = <>o__54.<>p__44.Target;
			CallSite<Func<CallSite, object, byte>> arg43 = <>o__54.<>p__44;
			if (<>o__54.<>p__43 == null)
			{
				<>o__54.<>p__43 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target30 = <>o__54.<>p__43.Target;
			CallSite<Func<CallSite, Type, object, object>> arg44 = <>o__54.<>p__43;
			Type arg45 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__42 == null)
			{
				<>o__54.<>p__42 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference15.Duty = target29(arg43, target30(arg44, arg45, <>o__54.<>p__42.Target(<>o__54.<>p__42, (object)data, "T14")));
			ref FanTable1p5Buffer reference16 = ref fanTable.CPU[15];
			if (<>o__54.<>p__47 == null)
			{
				<>o__54.<>p__47 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target31 = <>o__54.<>p__47.Target;
			CallSite<Func<CallSite, object, byte>> arg46 = <>o__54.<>p__47;
			if (<>o__54.<>p__46 == null)
			{
				<>o__54.<>p__46 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target32 = <>o__54.<>p__46.Target;
			CallSite<Func<CallSite, Type, object, object>> arg47 = <>o__54.<>p__46;
			Type arg48 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__45 == null)
			{
				<>o__54.<>p__45 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference16.Duty = target31(arg46, target32(arg47, arg48, <>o__54.<>p__45.Target(<>o__54.<>p__45, (object)data, "T15")));
			WriteFanTableToJson(fanTable, activated: true);
			if (saveEC)
			{
				SetEcFanTable_Cpu(fanTable);
			}
		}
		else if (type == "GPU")
		{
			ref FanTable1p5Buffer reference17 = ref fanTable.GPU[0];
			if (<>o__54.<>p__50 == null)
			{
				<>o__54.<>p__50 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target33 = <>o__54.<>p__50.Target;
			CallSite<Func<CallSite, object, byte>> arg49 = <>o__54.<>p__50;
			if (<>o__54.<>p__49 == null)
			{
				<>o__54.<>p__49 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target34 = <>o__54.<>p__49.Target;
			CallSite<Func<CallSite, Type, object, object>> arg50 = <>o__54.<>p__49;
			Type arg51 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__48 == null)
			{
				<>o__54.<>p__48 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference17.Duty = target33(arg49, target34(arg50, arg51, <>o__54.<>p__48.Target(<>o__54.<>p__48, (object)data, "T0")));
			ref FanTable1p5Buffer reference18 = ref fanTable.GPU[1];
			if (<>o__54.<>p__53 == null)
			{
				<>o__54.<>p__53 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target35 = <>o__54.<>p__53.Target;
			CallSite<Func<CallSite, object, byte>> arg52 = <>o__54.<>p__53;
			if (<>o__54.<>p__52 == null)
			{
				<>o__54.<>p__52 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target36 = <>o__54.<>p__52.Target;
			CallSite<Func<CallSite, Type, object, object>> arg53 = <>o__54.<>p__52;
			Type arg54 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__51 == null)
			{
				<>o__54.<>p__51 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference18.Duty = target35(arg52, target36(arg53, arg54, <>o__54.<>p__51.Target(<>o__54.<>p__51, (object)data, "T1")));
			ref FanTable1p5Buffer reference19 = ref fanTable.GPU[2];
			if (<>o__54.<>p__56 == null)
			{
				<>o__54.<>p__56 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target37 = <>o__54.<>p__56.Target;
			CallSite<Func<CallSite, object, byte>> arg55 = <>o__54.<>p__56;
			if (<>o__54.<>p__55 == null)
			{
				<>o__54.<>p__55 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target38 = <>o__54.<>p__55.Target;
			CallSite<Func<CallSite, Type, object, object>> arg56 = <>o__54.<>p__55;
			Type arg57 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__54 == null)
			{
				<>o__54.<>p__54 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference19.Duty = target37(arg55, target38(arg56, arg57, <>o__54.<>p__54.Target(<>o__54.<>p__54, (object)data, "T2")));
			ref FanTable1p5Buffer reference20 = ref fanTable.GPU[3];
			if (<>o__54.<>p__59 == null)
			{
				<>o__54.<>p__59 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target39 = <>o__54.<>p__59.Target;
			CallSite<Func<CallSite, object, byte>> arg58 = <>o__54.<>p__59;
			if (<>o__54.<>p__58 == null)
			{
				<>o__54.<>p__58 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target40 = <>o__54.<>p__58.Target;
			CallSite<Func<CallSite, Type, object, object>> arg59 = <>o__54.<>p__58;
			Type arg60 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__57 == null)
			{
				<>o__54.<>p__57 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference20.Duty = target39(arg58, target40(arg59, arg60, <>o__54.<>p__57.Target(<>o__54.<>p__57, (object)data, "T3")));
			ref FanTable1p5Buffer reference21 = ref fanTable.GPU[4];
			if (<>o__54.<>p__62 == null)
			{
				<>o__54.<>p__62 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target41 = <>o__54.<>p__62.Target;
			CallSite<Func<CallSite, object, byte>> arg61 = <>o__54.<>p__62;
			if (<>o__54.<>p__61 == null)
			{
				<>o__54.<>p__61 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target42 = <>o__54.<>p__61.Target;
			CallSite<Func<CallSite, Type, object, object>> arg62 = <>o__54.<>p__61;
			Type arg63 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__60 == null)
			{
				<>o__54.<>p__60 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference21.Duty = target41(arg61, target42(arg62, arg63, <>o__54.<>p__60.Target(<>o__54.<>p__60, (object)data, "T4")));
			ref FanTable1p5Buffer reference22 = ref fanTable.GPU[5];
			if (<>o__54.<>p__65 == null)
			{
				<>o__54.<>p__65 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target43 = <>o__54.<>p__65.Target;
			CallSite<Func<CallSite, object, byte>> arg64 = <>o__54.<>p__65;
			if (<>o__54.<>p__64 == null)
			{
				<>o__54.<>p__64 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target44 = <>o__54.<>p__64.Target;
			CallSite<Func<CallSite, Type, object, object>> arg65 = <>o__54.<>p__64;
			Type arg66 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__63 == null)
			{
				<>o__54.<>p__63 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference22.Duty = target43(arg64, target44(arg65, arg66, <>o__54.<>p__63.Target(<>o__54.<>p__63, (object)data, "T5")));
			ref FanTable1p5Buffer reference23 = ref fanTable.GPU[6];
			if (<>o__54.<>p__68 == null)
			{
				<>o__54.<>p__68 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target45 = <>o__54.<>p__68.Target;
			CallSite<Func<CallSite, object, byte>> arg67 = <>o__54.<>p__68;
			if (<>o__54.<>p__67 == null)
			{
				<>o__54.<>p__67 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target46 = <>o__54.<>p__67.Target;
			CallSite<Func<CallSite, Type, object, object>> arg68 = <>o__54.<>p__67;
			Type arg69 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__66 == null)
			{
				<>o__54.<>p__66 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference23.Duty = target45(arg67, target46(arg68, arg69, <>o__54.<>p__66.Target(<>o__54.<>p__66, (object)data, "T6")));
			ref FanTable1p5Buffer reference24 = ref fanTable.GPU[7];
			if (<>o__54.<>p__71 == null)
			{
				<>o__54.<>p__71 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target47 = <>o__54.<>p__71.Target;
			CallSite<Func<CallSite, object, byte>> arg70 = <>o__54.<>p__71;
			if (<>o__54.<>p__70 == null)
			{
				<>o__54.<>p__70 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target48 = <>o__54.<>p__70.Target;
			CallSite<Func<CallSite, Type, object, object>> arg71 = <>o__54.<>p__70;
			Type arg72 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__69 == null)
			{
				<>o__54.<>p__69 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference24.Duty = target47(arg70, target48(arg71, arg72, <>o__54.<>p__69.Target(<>o__54.<>p__69, (object)data, "T7")));
			ref FanTable1p5Buffer reference25 = ref fanTable.GPU[8];
			if (<>o__54.<>p__74 == null)
			{
				<>o__54.<>p__74 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target49 = <>o__54.<>p__74.Target;
			CallSite<Func<CallSite, object, byte>> arg73 = <>o__54.<>p__74;
			if (<>o__54.<>p__73 == null)
			{
				<>o__54.<>p__73 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target50 = <>o__54.<>p__73.Target;
			CallSite<Func<CallSite, Type, object, object>> arg74 = <>o__54.<>p__73;
			Type arg75 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__72 == null)
			{
				<>o__54.<>p__72 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference25.Duty = target49(arg73, target50(arg74, arg75, <>o__54.<>p__72.Target(<>o__54.<>p__72, (object)data, "T8")));
			ref FanTable1p5Buffer reference26 = ref fanTable.GPU[9];
			if (<>o__54.<>p__77 == null)
			{
				<>o__54.<>p__77 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target51 = <>o__54.<>p__77.Target;
			CallSite<Func<CallSite, object, byte>> arg76 = <>o__54.<>p__77;
			if (<>o__54.<>p__76 == null)
			{
				<>o__54.<>p__76 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target52 = <>o__54.<>p__76.Target;
			CallSite<Func<CallSite, Type, object, object>> arg77 = <>o__54.<>p__76;
			Type arg78 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__75 == null)
			{
				<>o__54.<>p__75 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference26.Duty = target51(arg76, target52(arg77, arg78, <>o__54.<>p__75.Target(<>o__54.<>p__75, (object)data, "T9")));
			ref FanTable1p5Buffer reference27 = ref fanTable.GPU[10];
			if (<>o__54.<>p__80 == null)
			{
				<>o__54.<>p__80 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target53 = <>o__54.<>p__80.Target;
			CallSite<Func<CallSite, object, byte>> arg79 = <>o__54.<>p__80;
			if (<>o__54.<>p__79 == null)
			{
				<>o__54.<>p__79 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target54 = <>o__54.<>p__79.Target;
			CallSite<Func<CallSite, Type, object, object>> arg80 = <>o__54.<>p__79;
			Type arg81 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__78 == null)
			{
				<>o__54.<>p__78 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference27.Duty = target53(arg79, target54(arg80, arg81, <>o__54.<>p__78.Target(<>o__54.<>p__78, (object)data, "T10")));
			ref FanTable1p5Buffer reference28 = ref fanTable.GPU[11];
			if (<>o__54.<>p__83 == null)
			{
				<>o__54.<>p__83 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target55 = <>o__54.<>p__83.Target;
			CallSite<Func<CallSite, object, byte>> arg82 = <>o__54.<>p__83;
			if (<>o__54.<>p__82 == null)
			{
				<>o__54.<>p__82 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target56 = <>o__54.<>p__82.Target;
			CallSite<Func<CallSite, Type, object, object>> arg83 = <>o__54.<>p__82;
			Type arg84 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__81 == null)
			{
				<>o__54.<>p__81 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference28.Duty = target55(arg82, target56(arg83, arg84, <>o__54.<>p__81.Target(<>o__54.<>p__81, (object)data, "T11")));
			ref FanTable1p5Buffer reference29 = ref fanTable.GPU[12];
			if (<>o__54.<>p__86 == null)
			{
				<>o__54.<>p__86 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target57 = <>o__54.<>p__86.Target;
			CallSite<Func<CallSite, object, byte>> arg85 = <>o__54.<>p__86;
			if (<>o__54.<>p__85 == null)
			{
				<>o__54.<>p__85 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target58 = <>o__54.<>p__85.Target;
			CallSite<Func<CallSite, Type, object, object>> arg86 = <>o__54.<>p__85;
			Type arg87 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__84 == null)
			{
				<>o__54.<>p__84 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference29.Duty = target57(arg85, target58(arg86, arg87, <>o__54.<>p__84.Target(<>o__54.<>p__84, (object)data, "T12")));
			ref FanTable1p5Buffer reference30 = ref fanTable.GPU[13];
			if (<>o__54.<>p__89 == null)
			{
				<>o__54.<>p__89 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target59 = <>o__54.<>p__89.Target;
			CallSite<Func<CallSite, object, byte>> arg88 = <>o__54.<>p__89;
			if (<>o__54.<>p__88 == null)
			{
				<>o__54.<>p__88 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target60 = <>o__54.<>p__88.Target;
			CallSite<Func<CallSite, Type, object, object>> arg89 = <>o__54.<>p__88;
			Type arg90 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__87 == null)
			{
				<>o__54.<>p__87 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference30.Duty = target59(arg88, target60(arg89, arg90, <>o__54.<>p__87.Target(<>o__54.<>p__87, (object)data, "T13")));
			ref FanTable1p5Buffer reference31 = ref fanTable.GPU[14];
			if (<>o__54.<>p__92 == null)
			{
				<>o__54.<>p__92 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target61 = <>o__54.<>p__92.Target;
			CallSite<Func<CallSite, object, byte>> arg91 = <>o__54.<>p__92;
			if (<>o__54.<>p__91 == null)
			{
				<>o__54.<>p__91 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target62 = <>o__54.<>p__91.Target;
			CallSite<Func<CallSite, Type, object, object>> arg92 = <>o__54.<>p__91;
			Type arg93 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__90 == null)
			{
				<>o__54.<>p__90 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference31.Duty = target61(arg91, target62(arg92, arg93, <>o__54.<>p__90.Target(<>o__54.<>p__90, (object)data, "T14")));
			ref FanTable1p5Buffer reference32 = ref fanTable.GPU[15];
			if (<>o__54.<>p__95 == null)
			{
				<>o__54.<>p__95 = CallSite<Func<CallSite, object, byte>>.Create(Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(byte).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle)));
			}
			Func<CallSite, object, byte> target63 = <>o__54.<>p__95.Target;
			CallSite<Func<CallSite, object, byte>> arg94 = <>o__54.<>p__95;
			if (<>o__54.<>p__94 == null)
			{
				<>o__54.<>p__94 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToByte", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target64 = <>o__54.<>p__94.Target;
			CallSite<Func<CallSite, Type, object, object>> arg95 = <>o__54.<>p__94;
			Type arg96 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__54.<>p__93 == null)
			{
				<>o__54.<>p__93 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(FanTable_Manager1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			reference32.Duty = target63(arg94, target64(arg95, arg96, <>o__54.<>p__93.Target(<>o__54.<>p__93, (object)data, "T15")));
			WriteFanTableToJson(fanTable, activated: true);
			if (saveEC)
			{
				SetEcFanTable_Gpu(fanTable);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private FanTable1p5 GenerateFanDuty(FanTable1p5 defaultFantable, double ratio, string name)
	{
		byte b = 0;
		FanTable1p5 result = new FanTable1p5(16u, name);
		result.CpuTemp_DefaultMaxLevel = defaultFantable.CpuTemp_DefaultMaxLevel;
		result.GpuTemp_DefaultMaxLevel = defaultFantable.GpuTemp_DefaultMaxLevel;
		for (int i = 0; i <= 15; i++)
		{
			result.CPU[i].UpT = defaultFantable.CPU[i].UpT;
			result.GPU[i].UpT = defaultFantable.GPU[i].UpT;
			result.CPU[i].DownT = defaultFantable.CPU[i].DownT;
			result.GPU[i].DownT = defaultFantable.GPU[i].DownT;
			b = defaultFantable.CPU[i].Duty;
			result.CPU[i].Duty = ResolvedDelegates.System_Convert_ToByte__135((double)(int)b * ratio);
			b = defaultFantable.GPU[i].Duty;
			result.GPU[i].Duty = ResolvedDelegates.System_Convert_ToByte__135((double)(int)b * ratio);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private FanTable1p5 CopyFanTable(FanTable1p5 defaultFantable, string name)
	{
		FanTable1p5 result = new FanTable1p5(16u, name);
		result.CpuTemp_DefaultMaxLevel = defaultFantable.CpuTemp_DefaultMaxLevel;
		result.GpuTemp_DefaultMaxLevel = defaultFantable.GpuTemp_DefaultMaxLevel;
		for (int i = 0; i <= 15; i++)
		{
			result.CPU[i].UpT = defaultFantable.CPU[i].UpT;
			result.GPU[i].UpT = defaultFantable.GPU[i].UpT;
			result.CPU[i].DownT = defaultFantable.CPU[i].DownT;
			result.GPU[i].DownT = defaultFantable.GPU[i].DownT;
			result.CPU[i].Duty = defaultFantable.CPU[i].Duty;
			result.GPU[i].Duty = defaultFantable.GPU[i].Duty;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public virtual void ClearFanTableAll()
	{
		byte data = 0;
		for (int i = 0; i <= 15; i++)
		{
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3840 + i), data);
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3856 + i), data);
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3872 + i), data);
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3888 + i), data);
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3904 + i), data);
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), (ushort)(3920 + i), data);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool RefreshDefaultFanTableAll()
	{
		bool result = false;
		EcCtrl.Set_APExistToEC(bExist: false);
		bool flag = RefreshDefaultFanTable(ref DefaultFanTable_Gaming, 2);
		DefaultFanTable_Gaming.CpuTemp_DefaultMaxLevel = GetCpuTemMaxLevel(ref DefaultFanTable_Gaming);
		DefaultFanTable_Gaming.GpuTemp_DefaultMaxLevel = GetGpuTemMaxLevel(ref DefaultFanTable_Gaming);
		bool flag2 = RefreshDefaultFanTable(ref DefaultFanTable_Office, 3);
		DefaultFanTable_Office.CpuTemp_DefaultMaxLevel = GetCpuTemMaxLevel(ref DefaultFanTable_Office);
		DefaultFanTable_Office.GpuTemp_DefaultMaxLevel = GetGpuTemMaxLevel(ref DefaultFanTable_Office);
		bool flag3 = RefreshDefaultFanTable(ref DefaultFanTable_Turbo, 1);
		DefaultFanTable_Turbo.CpuTemp_DefaultMaxLevel = GetCpuTemMaxLevel(ref DefaultFanTable_Turbo);
		DefaultFanTable_Turbo.GpuTemp_DefaultMaxLevel = GetGpuTemMaxLevel(ref DefaultFanTable_Turbo);
		if (flag & flag2 & flag3)
		{
			WriteFanTableToJson(DefaultFanTable_Gaming, activated: false);
			WriteFanTableToJson(DefaultFanTable_Office, activated: false);
			WriteFanTableToJson(DefaultFanTable_Turbo, activated: false);
			result = true;
		}
		EcCtrl.Set_APExistToEC(bExist: true);
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool IsReadyToRead()
	{
		byte Data = 253;
		byte Data2 = 201;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3933, ref Data);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3934, ref Data2);
		if (Data != 253 && Data2 != 201)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool RefreshDefaultFanTable(ref FanTable1p5 fantable, byte mode)
	{
		bool flag = false;
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3935, mode);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3933, 253);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 3934, 201);
		int num = 0;
		do
		{
			Thread.Sleep(500);
			if (IsReadyToRead())
			{
				GetEcFanTable(ref fantable);
				LogCtrl.TraceMessage("GetFanTable success.", "RefreshDefaultFanTable", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\FanTable\\FanTable_Manager1p5.cs", 1114);
				return true;
			}
			num++;
		}
		while (num != 10);
		LogCtrl.TraceMessage("GetFanTable fail.", "RefreshDefaultFanTable", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\FanTable\\FanTable_Manager1p5.cs", 1122);
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe FanTable1p5 TableTransform(string name)
	{
		if (name != null)
		{
			switch (ResolvedDelegates.System_String_get_Length__298(ref *(string*)name))
			{
			case 4:
				switch (name[3])
				{
				case '1':
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T1"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T1"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T1"))
							{
								if (!ResolvedDelegates.System_String_op_Equality__18(name, "M4T1"))
								{
									break;
								}
								return M4T1;
							}
							return M3T1;
						}
						return M2T1;
					}
					return M1T1;
				case '2':
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T2"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T2"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T2"))
							{
								if (!ResolvedDelegates.System_String_op_Equality__18(name, "M4T2"))
								{
									break;
								}
								return M4T2;
							}
							return M3T2;
						}
						return M2T2;
					}
					return M1T2;
				case '3':
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T3"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T3"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T3"))
							{
								if (!ResolvedDelegates.System_String_op_Equality__18(name, "M4T3"))
								{
									break;
								}
								return M4T3;
							}
							return M3T3;
						}
						return M2T3;
					}
					return M1T3;
				case '4':
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T4"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T4"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T4"))
							{
								if (!ResolvedDelegates.System_String_op_Equality__18(name, "M4T4"))
								{
									break;
								}
								return M4T4;
							}
							return M3T4;
						}
						return M2T4;
					}
					return M1T4;
				case '5':
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T5"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T5"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T5"))
							{
								if (!ResolvedDelegates.System_String_op_Equality__18(name, "M4T5"))
								{
									break;
								}
								return M4T5;
							}
							return M3T5;
						}
						return M2T5;
					}
					return M1T5;
				}
				break;
			case 22:
				switch (name[16])
				{
				case 'G':
					if (!(name == "DefaultFanTable_Gaming"))
					{
						break;
					}
					return DefaultFanTable_Gaming;
				case 'O':
					if (!(name == "DefaultFanTable_Office"))
					{
						break;
					}
					return DefaultFanTable_Office;
				}
				break;
			case 21:
				if (!(name == "DefaultFanTable_Turbo"))
				{
					break;
				}
				return DefaultFanTable_Turbo;
			}
		}
		return default;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe void SetFanTableActivated(string name, bool activated)
	{
		if (name == null)
		{
			return;
		}
		int num = ResolvedDelegates.System_String_get_Length__298(ref *(string*)name);
		if (num != 4)
		{
			return;
		}
		switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)name, 3))
		{
		case '1':
			if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T1"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T1"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T1"))
					{
						if (ResolvedDelegates.System_String_op_Equality__18(name, "M4T1"))
						{
							M4T1.Activated = activated;
						}
					}
					else
					{
						M3T1.Activated = activated;
					}
				}
				else
				{
					M2T1.Activated = activated;
				}
			}
			else
			{
				M1T1.Activated = activated;
			}
			break;
		case '2':
			if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T2"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T2"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T2"))
					{
						if (ResolvedDelegates.System_String_op_Equality__18(name, "M4T2"))
						{
							M4T2.Activated = activated;
						}
					}
					else
					{
						M3T2.Activated = activated;
					}
				}
				else
				{
					M2T2.Activated = activated;
				}
			}
			else
			{
				M1T2.Activated = activated;
			}
			break;
		case '3':
			if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T3"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T3"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T3"))
					{
						if (ResolvedDelegates.System_String_op_Equality__18(name, "M4T3"))
						{
							M4T3.Activated = activated;
						}
					}
					else
					{
						M3T3.Activated = activated;
					}
				}
				else
				{
					M2T3.Activated = activated;
				}
			}
			else
			{
				M1T3.Activated = activated;
			}
			break;
		case '4':
			if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T4"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T4"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T4"))
					{
						if (ResolvedDelegates.System_String_op_Equality__18(name, "M4T4"))
						{
							M4T4.Activated = activated;
						}
					}
					else
					{
						M3T4.Activated = activated;
					}
				}
				else
				{
					M2T4.Activated = activated;
				}
			}
			else
			{
				M1T4.Activated = activated;
			}
			break;
		case '5':
			if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T5"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T5"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T5"))
					{
						if (ResolvedDelegates.System_String_op_Equality__18(name, "M4T5"))
						{
							M4T5.Activated = activated;
						}
					}
					else
					{
						M3T5.Activated = activated;
					}
				}
				else
				{
					M2T5.Activated = activated;
				}
			}
			else
			{
				M1T5.Activated = activated;
			}
			break;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe void RefreshFanControlRespective(string name, bool status)
	{
		if (name == null)
		{
			return;
		}
		int num = ResolvedDelegates.System_String_get_Length__298(ref *(string*)name);
		if (num != 4)
		{
			return;
		}
		switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)name, 3))
		{
		case '1':
			if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T1"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T1"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T1"))
					{
						if (ResolvedDelegates.System_String_op_Equality__18(name, "M4T1"))
						{
							M4T1.FanControlRespective = status;
						}
					}
					else
					{
						M3T1.FanControlRespective = status;
					}
				}
				else
				{
					M2T1.FanControlRespective = status;
				}
			}
			else
			{
				M1T1.FanControlRespective = status;
			}
			break;
		case '2':
			if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T2"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T2"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T2"))
					{
						if (ResolvedDelegates.System_String_op_Equality__18(name, "M4T2"))
						{
							M4T2.FanControlRespective = status;
						}
					}
					else
					{
						M3T2.FanControlRespective = status;
					}
				}
				else
				{
					M2T2.FanControlRespective = status;
				}
			}
			else
			{
				M1T2.FanControlRespective = status;
			}
			break;
		case '3':
			if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T3"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T3"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T3"))
					{
						if (ResolvedDelegates.System_String_op_Equality__18(name, "M4T3"))
						{
							M4T3.FanControlRespective = status;
						}
					}
					else
					{
						M3T3.FanControlRespective = status;
					}
				}
				else
				{
					M2T3.FanControlRespective = status;
				}
			}
			else
			{
				M1T3.FanControlRespective = status;
			}
			break;
		case '4':
			if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T4"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T4"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T4"))
					{
						if (ResolvedDelegates.System_String_op_Equality__18(name, "M4T4"))
						{
							M4T4.FanControlRespective = status;
						}
					}
					else
					{
						M3T4.FanControlRespective = status;
					}
				}
				else
				{
					M2T4.FanControlRespective = status;
				}
			}
			else
			{
				M1T4.FanControlRespective = status;
			}
			break;
		case '5':
			if (!ResolvedDelegates.System_String_op_Equality__18(name, "M1T5"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(name, "M2T5"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(name, "M3T5"))
					{
						if (ResolvedDelegates.System_String_op_Equality__18(name, "M4T5"))
						{
							M4T5.FanControlRespective = status;
						}
					}
					else
					{
						M3T5.FanControlRespective = status;
					}
				}
				else
				{
					M2T5.FanControlRespective = status;
				}
			}
			else
			{
				M1T5.FanControlRespective = status;
			}
			break;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int GetCpuTemMaxLevel(ref FanTable1p5 fantable)
	{
		int num = 10;
		for (int i = 1; i <= 15; i++)
		{
			if (fantable.CPU[i].Duty > fantable.CPU[i - 1].Duty)
			{
				num = i;
			}
		}
		return num + 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int GetGpuTemMaxLevel(ref FanTable1p5 fantable)
	{
		int num = 10;
		for (int i = 1; i <= 15; i++)
		{
			if (fantable.GPU[i].Duty > fantable.GPU[i - 1].Duty)
			{
				num = i;
			}
		}
		return num + 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetEcFanControlRespective(bool bEnable)
	{
		byte Data = 0;
		byte b = 127;
		byte b2 = 0;
		byte b3 = 0;
		b2 = (byte)(bEnable ? 128 : 0);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, ref Data);
		b3 = (byte)((Data & b) + b2);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, b3);
	}
}

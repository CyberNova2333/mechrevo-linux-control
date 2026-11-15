// Static analysis rendering; not original buildable source.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Define;
using MyControlCenter;
using MyECIO;

namespace Utility;

internal class SmartApcTableCtrl
{
	private AcpiCtrl AcpiModel = AcpiCtrl.Instance;

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	private static readonly SmartApcTableCtrl TableCtrl = new SmartApcTableCtrl();

	private int SupportSmartApcTable = 1;

	private List<GN20_GPU_SKU> SupportGpuSkuList = new List<GN20_GPU_SKU>
	{
		GN20_GPU_SKU.E3,
		GN20_GPU_SKU.E4,
		GN20_GPU_SKU.E5,
		GN20_GPU_SKU.MaxQ,
		GN20_GPU_SKU.E7,
		GN20_GPU_SKU.P0,
		GN20_GPU_SKU.P1,
		GN20_GPU_SKU.E6,
		GN20_GPU_SKU.E8
	};

	private List<BIOS_PROJECT_ID> SupportProjectIDs = new List<BIOS_PROJECT_ID>
	{
		BIOS_PROJECT_ID.IDR,
		BIOS_PROJECT_ID.IDX,
		BIOS_PROJECT_ID.IDV,
		BIOS_PROJECT_ID.IDO,
		BIOS_PROJECT_ID.IDP,
		BIOS_PROJECT_ID.IDS,
		BIOS_PROJECT_ID.IDY,
		BIOS_PROJECT_ID.IDW,
		BIOS_PROJECT_ID.IDM,
		BIOS_PROJECT_ID.IDE,
		BIOS_PROJECT_ID.IDN,
		BIOS_PROJECT_ID.IDZ,
		BIOS_PROJECT_ID.IDXQ,
		BIOS_PROJECT_ID.IDQ,
		BIOS_PROJECT_ID.IDO3
	};

	private object _SmartApcTableLock = new object();

	public static SmartApcTableCtrl Instance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return TableCtrl;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private SmartApcTableCtrl()
	{
		ResolvedDelegates.System_Object_ctor__0(this);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsSupportSmartApcTable()
	{
		if (SupportSmartApcTable == 1)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public SMAPCTABLE_STRUCT GetSMAPCTable()
	{
		LogCtrl.TraceMessage("Start", "GetSMAPCTable", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\Utility\\SmartApcTableCtrl.cs", 96);
		SMAPCTABLE_STRUCT result = default;
		if (SupportSmartApcTable == 0)
		{
			return result;
		}
		try
		{
			ResolvedDelegates.System_Diagnostics_EventLog_WriteEntry__212("GCUService", "Load SMAPCTABLE Start");
			result = GetTableByDriver();
			ResolvedDelegates.System_Diagnostics_EventLog_WriteEntry__212("GCUService", "Load SMAPCTABLE End");
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception: ", ResolvedDelegates.System_Object_ToString__1(ex)), "GetSMAPCTable", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\Utility\\SmartApcTableCtrl.cs", 114);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private SMAPCTABLE_STRUCT GetTableByDriver()
	{
		byte[] array = new byte[12];
		List<byte> list = new List<byte>();
		for (int i = 0; i < 12; i++)
		{
			if (!Monitor.TryEnter(_SmartApcTableLock, 1000))
			{
				continue;
			}
			try
			{
				byte[] pData = new byte[4];
				AcpiModel.ReadSmartApcTableByKey(ResolvedDelegates.System_Convert_ToByte__75(i), ref pData);
				list.Add(pData[0]);
			}
			catch (Exception)
			{
			}
			finally
			{
				ResolvedDelegates.System_Threading_Monitor_Exit__71(_SmartApcTableLock);
			}
		}
		array = list.ToArray();
		string text = string.Empty;
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			text = ResolvedDelegates.System_String_Concat__29(text, ResolvedDelegates.System_String_Format__7("{0:x2}", b));
		}
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("OutValue: ", text), "GetTableByDriver", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\Utility\\SmartApcTableCtrl.cs", 179);
		ResolvedDelegates.System_Diagnostics_EventLog_WriteEntry__212("GCUService", ResolvedDelegates.System_String_Concat__29("Buffer Finish ! OutValue: ", text));
		SMAPCTABLE_STRUCT result = UtilityExtensions.BytesToStructure<SMAPCTABLE_STRUCT>(array);
		ResolvedDelegates.System_Diagnostics_EventLog_WriteEntry__212("GCUService", "Table Finish !");
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private SMAPCTABLE_STRUCT GetTableByWMI()
	{
		return default;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private SMAPCTABLE_STRUCT GetTableByDLL()
	{
		byte[] pData = new byte[4];
		byte[] pData2 = new byte[4];
		byte[] pData3 = new byte[4];
		ResolvedDelegates.System_Threading_Thread_Sleep__6(100);
		LogCtrl.TraceMessage("ReadSmartApcTableByDll Start", "GetTableByDLL", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\Utility\\SmartApcTableCtrl.cs", 236);
		ResolvedDelegates.System_Diagnostics_EventLog_WriteEntry__212("GCUService", "ReadSmartApcTableByDll Start");
		AcpiModel.ReadSmartApcTableByDll(0, ref pData);
		ResolvedDelegates.System_Threading_Thread_Sleep__6(100);
		AcpiModel.ReadSmartApcTableByDll(4, ref pData2);
		ResolvedDelegates.System_Threading_Thread_Sleep__6(100);
		AcpiModel.ReadSmartApcTableByDll(8, ref pData3);
		LogCtrl.TraceMessage("ReadSmartApcTableByDll End", "GetTableByDLL", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\Utility\\SmartApcTableCtrl.cs", 243);
		ResolvedDelegates.System_Diagnostics_EventLog_WriteEntry__212("GCUService", "ReadSmartApcTableByDll End");
		byte[] array = new byte[12];
		array = UtilityExtensions.Combine(pData, pData2);
		array = UtilityExtensions.Combine(array, pData3);
		string text = string.Empty;
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			text = ResolvedDelegates.System_String_Concat__29(text, ResolvedDelegates.System_String_Format__7("{0:x2}", b));
		}
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("OutValue: ", text), "GetTableByDLL", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\Utility\\SmartApcTableCtrl.cs", 256);
		return UtilityExtensions.BytesToStructure<SMAPCTABLE_STRUCT>(array);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool WriteSMAPCTable(string varname, byte value)
	{
		bool result = false;
		if (SupportSmartApcTable == 0)
		{
			return false;
		}
		SMAPCTABLE_STRUCT sMAPCTable = GetSMAPCTable();
		if (!ResolvedDelegates.System_String_op_Equality__18(varname, "PL1"))
		{
			if (!ResolvedDelegates.System_String_op_Equality__18(varname, "PL2"))
			{
				if (!ResolvedDelegates.System_String_op_Equality__18(varname, "PL4"))
				{
					if (!ResolvedDelegates.System_String_op_Equality__18(varname, "ConfigurableTGP"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(varname, "DynamicBoost"))
						{
							if (ResolvedDelegates.System_String_op_Equality__18(varname, "DefaultTGP"))
							{
								sMAPCTable.DefaultTGP = value;
							}
						}
						else
						{
							sMAPCTable.DynamicBoost = value;
						}
					}
					else
					{
						sMAPCTable.ConfigurableTGP = value;
					}
				}
				else
				{
					sMAPCTable.PL4 = value;
				}
			}
			else
			{
				sMAPCTable.PL2 = value;
			}
		}
		else
		{
			sMAPCTable.PL1 = value;
		}
		try
		{
			byte[] data = new byte[ResolvedDelegates.System_Runtime_InteropServices_Marshal_SizeOf__114(ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(SMAPCTABLE_STRUCT).TypeHandle))];
			AcpiModel.WriteSmartApcTableByDll(data);
			result = true;
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception: ", ResolvedDelegates.System_Object_ToString__1(ex)), "WriteSMAPCTable", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\Utility\\SmartApcTableCtrl.cs", 318);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public int GetTGP_IncreaseRangeMaximum(int ConfigurableTGP)
	{
		int result = 0;
		int tGP_NormalMinimum = GetTGP_NormalMinimum();
		if (ConfigurableTGP > 0 && tGP_NormalMinimum != 0)
		{
			result = ConfigurableTGP - tGP_NormalMinimum;
		}
		else
		{
			LogCtrl.TraceMessage("Integrity check fail, then TGPIncreaseRangeMaximum: " + ResolvedDelegates.System_Int32_ToString__55(ref result), "GetTGP_IncreaseRangeMaximum", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\Utility\\SmartApcTableCtrl.cs", 336);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int GetTGP_NormalMinimum()
	{
		GN20_GPU_SKU sku = EcCtrl.GetSku();
		BIOS_PROJECT_ID biosProjctID = EcCtrl.GetBiosProjctID();
		int result = 0;
		int project2ExID = EcCtrl.GetProject2ExID(EcCtrl.GetProjectIdFromEC());
		if (CustomizeCtrl.GetIsAMDPlatform())
		{
			switch (sku)
			{
			case GN20_GPU_SKU.E3:
				switch (biosProjctID)
				{
				case BIOS_PROJECT_ID.IDS:
				case BIOS_PROJECT_ID.IDM:
					result = 115;
					break;
				case BIOS_PROJECT_ID.IDR:
				case BIOS_PROJECT_ID.IDO:
				case BIOS_PROJECT_ID.IDP:
				case BIOS_PROJECT_ID.IDY:
				case BIOS_PROJECT_ID.IDW:
				case BIOS_PROJECT_ID.IDE:
					result = 80;
					break;
				}
				if (project2ExID == 5894)
				{
					result = 80;
				}
				break;
			case GN20_GPU_SKU.E5:
				switch (biosProjctID)
				{
				case BIOS_PROJECT_ID.IDS:
				case BIOS_PROJECT_ID.IDM:
					result = 115;
					break;
				case BIOS_PROJECT_ID.IDP:
				case BIOS_PROJECT_ID.IDY:
				case BIOS_PROJECT_ID.IDW:
					result = 115;
					break;
				}
				break;
			case GN20_GPU_SKU.E6:
				if ((uint)(biosProjctID - 5) <= 4u || biosProjctID == BIOS_PROJECT_ID.IDN)
				{
					result = 115;
				}
				if (project2ExID == 5894)
				{
					result = 115;
				}
				break;
			case GN20_GPU_SKU.E7:
				switch (biosProjctID)
				{
				case BIOS_PROJECT_ID.IDS:
				case BIOS_PROJECT_ID.IDM:
					result = 115;
					break;
				case BIOS_PROJECT_ID.IDP:
				case BIOS_PROJECT_ID.IDY:
				case BIOS_PROJECT_ID.IDW:
					result = 115;
					break;
				}
				if (project2ExID == 5894)
				{
					result = 115;
				}
				break;
			case GN20_GPU_SKU.E8:
				if ((uint)(biosProjctID - 5) <= 4u || biosProjctID == BIOS_PROJECT_ID.IDN)
				{
					result = 115;
				}
				break;
			case GN20_GPU_SKU.MaxQ:
				if (biosProjctID == BIOS_PROJECT_ID.IDP)
				{
					result = 80;
				}
				break;
			}
		}
		else
		{
			switch (sku)
			{
			case GN20_GPU_SKU.E3:
				switch (biosProjctID)
				{
				case BIOS_PROJECT_ID.IDM:
					result = 115;
					break;
				case BIOS_PROJECT_ID.IDP:
				case BIOS_PROJECT_ID.IDY:
					result = 80;
					break;
				case BIOS_PROJECT_ID.IDR:
				case BIOS_PROJECT_ID.IDO:
				case BIOS_PROJECT_ID.IDE:
					result = 80;
					break;
				}
				break;
			case GN20_GPU_SKU.E5:
				switch (biosProjctID)
				{
				case BIOS_PROJECT_ID.IDP:
				case BIOS_PROJECT_ID.IDY:
				case BIOS_PROJECT_ID.IDM:
					result = 115;
					break;
				}
				break;
			}
		}
		LogCtrl.TraceMessage("GN20_GPU_SKU: " + sku.ToString() + ", BIOS_PROJECT_ID: " + biosProjctID.ToString() + ", then the minimum: " + ResolvedDelegates.System_Int32_ToString__55(ref result) + "W", "GetTGP_NormalMinimum", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\Utility\\SmartApcTableCtrl.cs", 486);
		return result;
	}
}

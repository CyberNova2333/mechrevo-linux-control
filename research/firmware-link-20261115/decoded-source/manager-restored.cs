// Reconstructed analysis view; strings decoded statically; unresolved delegates remain.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using Define;
using GCUService.MyFan.BTCooling;
using GCUService.MyFan.Overclocking;
using GCUService.ScanCode;
using GCUService.Utility;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using MyControlCenter.MyFan.FanTable;
using MyECIO;
using Newtonsoft.Json;
using Utility;

namespace MyControlCenter.MyFan;

internal class MyFanManager_RamFan1p5 : MyFanManager
{
	[CompilerGenerated]
	private sealed class <>c__DisplayClass201_0
	{
		public MyFanManager_RamFan1p5 <>4__this;

		public string _tableName;

		public <>c__DisplayClass201_0()
		{
			ResolvedDelegates.System_Object_ctor__0(this);
		}

		internal void <SetFanTableThread>b__0()
		{
			<>4__this.FanTable.SetFanTable(_tableName);
		}

		internal void <SetFanTableThread>b__1()
		{
			<>4__this.FanTable.SetFanTable(_tableName);
		}
	}

	[CompilerGenerated]
	private static class <>o__101
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string>> <>p__2;

		public static CallSite<Func<CallSite, object, string, object>> <>p__3;

		public static CallSite<Func<CallSite, object, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, string, object>> <>p__5;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__6;

		public static CallSite<Func<CallSite, object, int, object>> <>p__7;

		public static CallSite<Func<CallSite, object, object, object>> <>p__8;

		public static CallSite<Func<CallSite, object, bool>> <>p__9;

		public static CallSite<Func<CallSite, object, bool>> <>p__10;

		public static CallSite<Func<CallSite, object, string, object>> <>p__11;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__12;

		public static CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>> <>p__13;

		public static CallSite<Func<CallSite, object, string>> <>p__14;

		public static CallSite<Func<CallSite, object, string, object>> <>p__15;

		public static CallSite<Func<CallSite, object, object, object>> <>p__16;

		public static CallSite<Func<CallSite, object, string, object>> <>p__17;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__18;

		public static CallSite<Func<CallSite, object, int, object>> <>p__19;

		public static CallSite<Func<CallSite, object, object, object>> <>p__20;

		public static CallSite<Func<CallSite, object, bool>> <>p__21;

		public static CallSite<Func<CallSite, object, bool>> <>p__22;

		public static CallSite<Func<CallSite, object, string, object>> <>p__23;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__24;

		public static CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>> <>p__25;

		public static CallSite<Func<CallSite, object, string>> <>p__26;

		public static CallSite<Func<CallSite, object, string, object>> <>p__27;

		public static CallSite<Func<CallSite, object, object, object>> <>p__28;

		public static CallSite<Func<CallSite, object, string, object>> <>p__29;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__30;

		public static CallSite<Func<CallSite, object, int, object>> <>p__31;

		public static CallSite<Func<CallSite, object, object, object>> <>p__32;

		public static CallSite<Func<CallSite, object, bool>> <>p__33;

		public static CallSite<Func<CallSite, object, bool>> <>p__34;

		public static CallSite<Func<CallSite, object, string, object>> <>p__35;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__36;

		public static CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>> <>p__37;

		public static CallSite<Func<CallSite, object, string>> <>p__38;

		public static CallSite<Func<CallSite, object, string, object>> <>p__39;

		public static CallSite<Func<CallSite, object, object, object>> <>p__40;

		public static CallSite<Func<CallSite, object, string, object>> <>p__41;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__42;

		public static CallSite<Func<CallSite, object, int, object>> <>p__43;

		public static CallSite<Func<CallSite, object, object, object>> <>p__44;

		public static CallSite<Func<CallSite, object, bool>> <>p__45;

		public static CallSite<Func<CallSite, object, bool>> <>p__46;

		public static CallSite<Func<CallSite, object, string, object>> <>p__47;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__48;

		public static CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>> <>p__49;

		public static CallSite<Func<CallSite, object, string>> <>p__50;

		public static CallSite<Func<CallSite, object, string, object>> <>p__51;

		public static CallSite<Func<CallSite, object, object, object>> <>p__52;

		public static CallSite<Func<CallSite, object, bool>> <>p__53;

		public static CallSite<Func<CallSite, object, string, object>> <>p__54;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__55;

		public static CallSite<Func<CallSite, object, int>> <>p__56;

		public static CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>> <>p__57;

		public static CallSite<Action<CallSite, MyFanTableCtrl, object, bool>> <>p__58;

		public static CallSite<Action<CallSite, MyFanTableCtrl, object, bool>> <>p__59;

		public static CallSite<Action<CallSite, MyFanTableCtrl, object>> <>p__60;

		public static CallSite<Action<CallSite, MyFanTableCtrl, object>> <>p__61;

		public static CallSite<Func<CallSite, object, string, object>> <>p__62;

		public static CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>> <>p__63;

		public static CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>> <>p__64;

		public static CallSite<Func<CallSite, object, string, object>> <>p__65;

		public static CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>> <>p__66;

		public static CallSite<Func<CallSite, object, string, object>> <>p__67;

		public static CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>> <>p__68;

		public static CallSite<Func<CallSite, object, string, object>> <>p__69;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__70;

		public static CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>> <>p__71;
	}

	[CompilerGenerated]
	private static class <>o__123
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, bool, object>> <>p__2;

		public static CallSite<Func<CallSite, object, bool>> <>p__3;

		public static CallSite<Func<CallSite, object, bool>> <>p__4;

		public static CallSite<Func<CallSite, object, string, object>> <>p__5;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__6;

		public static CallSite<Func<CallSite, object, int>> <>p__7;
	}

	[CompilerGenerated]
	private static class <>o__124
	{
		public static CallSite<Func<CallSite, object, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, int>> <>p__2;
	}

	[CompilerGenerated]
	private static class <>o__125
	{
		public static CallSite<Func<CallSite, object, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, int>> <>p__2;
	}

	[CompilerGenerated]
	private static class <>o__131
	{
		public static CallSite<Func<CallSite, object, string, object>> <>p__0;

		public static CallSite<Func<CallSite, object, object, object>> <>p__1;

		public static CallSite<Func<CallSite, object, bool, object>> <>p__2;

		public static CallSite<Func<CallSite, object, bool>> <>p__3;

		public static CallSite<Func<CallSite, object, bool>> <>p__4;

		public static CallSite<Func<CallSite, object, string, object>> <>p__5;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__6;

		public static CallSite<Func<CallSite, object, int>> <>p__7;

		public static CallSite<Func<CallSite, object, string, object>> <>p__8;

		public static CallSite<Func<CallSite, object, object, object>> <>p__9;

		public static CallSite<Func<CallSite, object, bool>> <>p__10;

		public static CallSite<Func<CallSite, object, string, object>> <>p__11;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__12;

		public static CallSite<Func<CallSite, object, int>> <>p__13;

		public static CallSite<Func<CallSite, object, string, object>> <>p__14;

		public static CallSite<Func<CallSite, object, object, object>> <>p__15;

		public static CallSite<Func<CallSite, object, bool>> <>p__16;

		public static CallSite<Func<CallSite, object, string, object>> <>p__17;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__18;

		public static CallSite<Func<CallSite, object, int>> <>p__19;

		public static CallSite<Func<CallSite, object, string, object>> <>p__20;

		public static CallSite<Func<CallSite, object, object, object>> <>p__21;

		public static CallSite<Func<CallSite, object, bool>> <>p__22;

		public static CallSite<Func<CallSite, object, string, object>> <>p__23;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__24;

		public static CallSite<Func<CallSite, object, int>> <>p__25;

		public static CallSite<Func<CallSite, object, string, object>> <>p__26;

		public static CallSite<Func<CallSite, object, object, object>> <>p__27;

		public static CallSite<Func<CallSite, object, bool>> <>p__28;

		public static CallSite<Func<CallSite, object, string, object>> <>p__29;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__30;

		public static CallSite<Func<CallSite, object, int>> <>p__31;

		public static CallSite<Func<CallSite, object, string, object>> <>p__32;

		public static CallSite<Func<CallSite, object, object, object>> <>p__33;

		public static CallSite<Func<CallSite, object, bool>> <>p__34;

		public static CallSite<Func<CallSite, object, string, object>> <>p__35;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__36;

		public static CallSite<Func<CallSite, object, int>> <>p__37;

		public static CallSite<Func<CallSite, object, string, object>> <>p__38;

		public static CallSite<Func<CallSite, object, object, object>> <>p__39;

		public static CallSite<Func<CallSite, object, bool>> <>p__40;

		public static CallSite<Func<CallSite, object, string, object>> <>p__41;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__42;

		public static CallSite<Func<CallSite, object, int>> <>p__43;

		public static CallSite<Func<CallSite, object, string, object>> <>p__44;

		public static CallSite<Func<CallSite, object, object, object>> <>p__45;

		public static CallSite<Func<CallSite, object, bool>> <>p__46;

		public static CallSite<Func<CallSite, object, string, object>> <>p__47;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__48;

		public static CallSite<Func<CallSite, object, int>> <>p__49;

		public static CallSite<Func<CallSite, object, string, object>> <>p__50;

		public static CallSite<Func<CallSite, object, object, object>> <>p__51;

		public static CallSite<Func<CallSite, object, bool>> <>p__52;

		public static CallSite<Func<CallSite, object, string, object>> <>p__53;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__54;

		public static CallSite<Func<CallSite, object, int>> <>p__55;

		public static CallSite<Func<CallSite, object, string, object>> <>p__56;

		public static CallSite<Func<CallSite, object, object, object>> <>p__57;

		public static CallSite<Func<CallSite, object, bool>> <>p__58;

		public static CallSite<Func<CallSite, object, string, object>> <>p__59;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__60;

		public static CallSite<Func<CallSite, object, int>> <>p__61;

		public static CallSite<Func<CallSite, object, string, object>> <>p__62;

		public static CallSite<Func<CallSite, object, object, object>> <>p__63;

		public static CallSite<Func<CallSite, object, bool>> <>p__64;

		public static CallSite<Func<CallSite, object, string, object>> <>p__65;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__66;

		public static CallSite<Func<CallSite, object, int>> <>p__67;

		public static CallSite<Func<CallSite, object, string, object>> <>p__68;

		public static CallSite<Func<CallSite, object, object, object>> <>p__69;

		public static CallSite<Func<CallSite, object, bool>> <>p__70;

		public static CallSite<Func<CallSite, object, string, object>> <>p__71;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__72;

		public static CallSite<Func<CallSite, object, int>> <>p__73;

		public static CallSite<Func<CallSite, object, string, object>> <>p__74;

		public static CallSite<Func<CallSite, object, object, object>> <>p__75;

		public static CallSite<Func<CallSite, object, bool>> <>p__76;

		public static CallSite<Func<CallSite, object, string, object>> <>p__77;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__78;

		public static CallSite<Func<CallSite, object, int>> <>p__79;

		public static CallSite<Func<CallSite, object, string, object>> <>p__80;

		public static CallSite<Func<CallSite, object, object, object>> <>p__81;

		public static CallSite<Func<CallSite, object, bool>> <>p__82;

		public static CallSite<Func<CallSite, object, string, object>> <>p__83;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__84;

		public static CallSite<Func<CallSite, object, int>> <>p__85;

		public static CallSite<Func<CallSite, object, string, object>> <>p__86;

		public static CallSite<Func<CallSite, object, object, object>> <>p__87;

		public static CallSite<Func<CallSite, object, bool>> <>p__88;

		public static CallSite<Func<CallSite, object, string, object>> <>p__89;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__90;

		public static CallSite<Func<CallSite, object, int>> <>p__91;

		public static CallSite<Func<CallSite, object, string, object>> <>p__92;

		public static CallSite<Func<CallSite, object, object, object>> <>p__93;

		public static CallSite<Func<CallSite, object, bool>> <>p__94;

		public static CallSite<Func<CallSite, object, string, object>> <>p__95;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__96;

		public static CallSite<Func<CallSite, object, int>> <>p__97;

		public static CallSite<Func<CallSite, object, string, object>> <>p__98;

		public static CallSite<Func<CallSite, object, object, object>> <>p__99;

		public static CallSite<Func<CallSite, object, bool>> <>p__100;

		public static CallSite<Func<CallSite, object, string, object>> <>p__101;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__102;

		public static CallSite<Func<CallSite, object, int>> <>p__103;

		public static CallSite<Func<CallSite, object, string, object>> <>p__104;

		public static CallSite<Func<CallSite, object, object, object>> <>p__105;

		public static CallSite<Func<CallSite, object, bool>> <>p__106;

		public static CallSite<Func<CallSite, object, string, object>> <>p__107;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__108;

		public static CallSite<Func<CallSite, object, int>> <>p__109;

		public static CallSite<Func<CallSite, object, string, object>> <>p__110;

		public static CallSite<Func<CallSite, object, object, object>> <>p__111;

		public static CallSite<Func<CallSite, object, bool>> <>p__112;

		public static CallSite<Func<CallSite, object, string, object>> <>p__113;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__114;

		public static CallSite<Func<CallSite, object, int>> <>p__115;

		public static CallSite<Func<CallSite, object, string, object>> <>p__116;

		public static CallSite<Func<CallSite, object, object, object>> <>p__117;

		public static CallSite<Func<CallSite, object, bool>> <>p__118;

		public static CallSite<Func<CallSite, object, string, object>> <>p__119;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__120;

		public static CallSite<Func<CallSite, object, int>> <>p__121;

		public static CallSite<Func<CallSite, object, string, object>> <>p__122;

		public static CallSite<Func<CallSite, object, object, object>> <>p__123;

		public static CallSite<Func<CallSite, object, bool>> <>p__124;

		public static CallSite<Func<CallSite, object, string, object>> <>p__125;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__126;

		public static CallSite<Func<CallSite, object, int>> <>p__127;

		public static CallSite<Func<CallSite, object, string, object>> <>p__128;

		public static CallSite<Func<CallSite, object, object, object>> <>p__129;

		public static CallSite<Func<CallSite, object, bool>> <>p__130;

		public static CallSite<Func<CallSite, object, string, object>> <>p__131;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__132;

		public static CallSite<Func<CallSite, object, int>> <>p__133;

		public static CallSite<Func<CallSite, object, string, object>> <>p__134;

		public static CallSite<Func<CallSite, object, object, object>> <>p__135;

		public static CallSite<Func<CallSite, object, bool>> <>p__136;

		public static CallSite<Func<CallSite, object, string, object>> <>p__137;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__138;

		public static CallSite<Func<CallSite, object, int>> <>p__139;

		public static CallSite<Func<CallSite, object, string, object>> <>p__140;

		public static CallSite<Func<CallSite, object, object, object>> <>p__141;

		public static CallSite<Func<CallSite, object, bool>> <>p__142;

		public static CallSite<Func<CallSite, object, string, object>> <>p__143;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__144;

		public static CallSite<Func<CallSite, object, int>> <>p__145;

		public static CallSite<Func<CallSite, object, string, object>> <>p__146;

		public static CallSite<Func<CallSite, object, object, object>> <>p__147;

		public static CallSite<Func<CallSite, object, bool>> <>p__148;

		public static CallSite<Func<CallSite, object, string, object>> <>p__149;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__150;

		public static CallSite<Func<CallSite, object, int>> <>p__151;

		public static CallSite<Func<CallSite, object, string, object>> <>p__152;

		public static CallSite<Func<CallSite, object, object, object>> <>p__153;

		public static CallSite<Func<CallSite, object, bool>> <>p__154;

		public static CallSite<Func<CallSite, object, string, object>> <>p__155;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__156;

		public static CallSite<Func<CallSite, object, int>> <>p__157;

		public static CallSite<Func<CallSite, object, string, object>> <>p__158;

		public static CallSite<Func<CallSite, object, object, object>> <>p__159;

		public static CallSite<Func<CallSite, object, bool>> <>p__160;

		public static CallSite<Func<CallSite, object, string, object>> <>p__161;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__162;

		public static CallSite<Func<CallSite, object, int>> <>p__163;

		public static CallSite<Func<CallSite, object, string, object>> <>p__164;

		public static CallSite<Func<CallSite, object, object, object>> <>p__165;

		public static CallSite<Func<CallSite, object, bool>> <>p__166;

		public static CallSite<Func<CallSite, object, string, object>> <>p__167;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__168;

		public static CallSite<Func<CallSite, object, int>> <>p__169;

		public static CallSite<Func<CallSite, object, string, object>> <>p__170;

		public static CallSite<Func<CallSite, object, object, object>> <>p__171;

		public static CallSite<Func<CallSite, object, bool>> <>p__172;

		public static CallSite<Func<CallSite, object, string, object>> <>p__173;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__174;

		public static CallSite<Func<CallSite, object, int>> <>p__175;

		public static CallSite<Func<CallSite, object, string, object>> <>p__176;

		public static CallSite<Func<CallSite, object, object, object>> <>p__177;

		public static CallSite<Func<CallSite, object, bool>> <>p__178;

		public static CallSite<Func<CallSite, object, string, object>> <>p__179;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__180;

		public static CallSite<Func<CallSite, object, int>> <>p__181;

		public static CallSite<Func<CallSite, object, string, object>> <>p__182;

		public static CallSite<Func<CallSite, object, object, object>> <>p__183;

		public static CallSite<Func<CallSite, object, bool>> <>p__184;

		public static CallSite<Func<CallSite, object, string, object>> <>p__185;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__186;

		public static CallSite<Func<CallSite, object, int>> <>p__187;

		public static CallSite<Func<CallSite, object, string, object>> <>p__188;

		public static CallSite<Func<CallSite, object, object, object>> <>p__189;

		public static CallSite<Func<CallSite, object, bool>> <>p__190;

		public static CallSite<Func<CallSite, object, string, object>> <>p__191;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__192;

		public static CallSite<Func<CallSite, object, int>> <>p__193;

		public static CallSite<Func<CallSite, object, string, object>> <>p__194;

		public static CallSite<Func<CallSite, object, object, object>> <>p__195;

		public static CallSite<Func<CallSite, object, bool>> <>p__196;

		public static CallSite<Func<CallSite, object, string, object>> <>p__197;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__198;

		public static CallSite<Func<CallSite, object, int>> <>p__199;

		public static CallSite<Func<CallSite, object, string, object>> <>p__200;

		public static CallSite<Func<CallSite, object, object, object>> <>p__201;

		public static CallSite<Func<CallSite, object, bool>> <>p__202;

		public static CallSite<Func<CallSite, object, string, object>> <>p__203;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__204;

		public static CallSite<Func<CallSite, object, int>> <>p__205;

		public static CallSite<Func<CallSite, object, string, object>> <>p__206;

		public static CallSite<Func<CallSite, object, object, object>> <>p__207;

		public static CallSite<Func<CallSite, object, bool>> <>p__208;

		public static CallSite<Func<CallSite, object, string, object>> <>p__209;

		public static CallSite<Func<CallSite, object, object, object>> <>p__210;

		public static CallSite<Func<CallSite, object, bool>> <>p__211;
	}

	[CompilerGenerated]
	private static class <>o__145
	{
		public static CallSite<Func<CallSite, object, object, object>> <>p__0;

		public static CallSite<Func<CallSite, object, bool>> <>p__1;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__2;

		public static CallSite<Func<CallSite, object, string>> <>p__3;
	}

	[CompilerGenerated]
	private static class <>o__146
	{
		public static CallSite<Func<CallSite, object, object, object>> <>p__0;

		public static CallSite<Func<CallSite, object, bool>> <>p__1;

		public static CallSite<Func<CallSite, Type, object, object>> <>p__2;

		public static CallSite<Func<CallSite, object, string>> <>p__3;
	}

	[CompilerGenerated]
	private static class <>o__147
	{
		public static CallSite<Func<CallSite, object, string, uint, object>> <>p__0;

		public static CallSite<Func<CallSite, Type, object, Formatting, object>> <>p__1;

		public static CallSite<Func<CallSite, object, string>> <>p__2;
	}

	[CompilerGenerated]
	private static class <>o__149
	{
		public static CallSite<Func<CallSite, object, object>> <>p__0;

		public static CallSite<Func<CallSite, object, char, object>> <>p__1;

		public static CallSite<Func<CallSite, object, int, object>> <>p__2;

		public static CallSite<Func<CallSite, object, string>> <>p__3;

		public static CallSite<Func<CallSite, object, object>> <>p__4;

		public static CallSite<Func<CallSite, object, char, object>> <>p__5;

		public static CallSite<Func<CallSite, object, int, object>> <>p__6;

		public static CallSite<Func<CallSite, object, string>> <>p__7;

		public static CallSite<Func<CallSite, object, string, string, object>> <>p__8;

		public static CallSite<Func<CallSite, Type, object, Formatting, object>> <>p__9;

		public static CallSite<Func<CallSite, object, string>> <>p__10;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <Receive>d__101 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public byte[] data;

		public MyFanManager_RamFan1p5 <>4__this;

		private TaskAwaiter<object> <>u__1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe void MoveNext()
		{
			int num = <>1__state;
			MyFanManager_RamFan1p5 myFanManager_RamFan1p = <>4__this;
			try
			{
				TaskAwaiter<object> awaiter;
				if (num != 0)
				{
					awaiter = Json.ToObjectAsync<object>(ResolvedDelegates.System_Text_Encoding_GetString__351(ResolvedDelegates.System_Text_Encoding_get_UTF8__252(), data)).GetAwaiter();
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
				if (<>o__101.<>p__2 == null)
				{
					<>o__101.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, string> target = <>o__101.<>p__2.Target;
				CallSite<Func<CallSite, object, string>> arg = <>o__101.<>p__2;
				if (<>o__101.<>p__1 == null)
				{
					<>o__101.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target2 = <>o__101.<>p__1.Target;
				CallSite<Func<CallSite, Type, object, object>> arg2 = <>o__101.<>p__1;
				Type arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__101.<>p__0 == null)
				{
					<>o__101.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				string text = target(arg, target2(arg2, arg3, <>o__101.<>p__0.Target(<>o__101.<>p__0, result, "Action")));
				LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__107("---------------", LogCtrl.GetTime(), "---------------"), "Receive", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 668);
				LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Action = ", text), "Receive", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 669);
				if (text != null)
				{
					switch (ResolvedDelegates.System_String_get_Length__298(ref *(string*)text))
					{
					case 21:
						switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)text, 10))
						{
						case 'G':
						{
							if (!(text == "OPERATING_GAMING_MODE"))
							{
								break;
							}
							if (<>o__101.<>p__10 == null)
							{
								<>o__101.<>p__10 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							Func<CallSite, object, bool> target28 = <>o__101.<>p__10.Target;
							CallSite<Func<CallSite, object, bool>> arg41 = <>o__101.<>p__10;
							if (<>o__101.<>p__4 == null)
							{
								<>o__101.<>p__4 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							Func<CallSite, object, object, object> target29 = <>o__101.<>p__4.Target;
							CallSite<Func<CallSite, object, object, object>> arg42 = <>o__101.<>p__4;
							if (<>o__101.<>p__3 == null)
							{
								<>o__101.<>p__3 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							object obj = target29(arg42, <>o__101.<>p__3.Target(<>o__101.<>p__3, result, "ProfileIndex"), null);
							if (<>o__101.<>p__9 == null)
							{
								<>o__101.<>p__9 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							object arg48;
							if (!<>o__101.<>p__9.Target(<>o__101.<>p__9, obj))
							{
								if (<>o__101.<>p__8 == null)
								{
									<>o__101.<>p__8 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, object, object, object> target30 = <>o__101.<>p__8.Target;
								CallSite<Func<CallSite, object, object, object>> arg43 = <>o__101.<>p__8;
								object arg44 = obj;
								if (<>o__101.<>p__7 == null)
								{
									<>o__101.<>p__7 = CallSite<Func<CallSite, object, int, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.GreaterThan, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								Func<CallSite, object, int, object> target31 = <>o__101.<>p__7.Target;
								CallSite<Func<CallSite, object, int, object>> arg45 = <>o__101.<>p__7;
								if (<>o__101.<>p__6 == null)
								{
									<>o__101.<>p__6 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, Type, object, object> target32 = <>o__101.<>p__6.Target;
								CallSite<Func<CallSite, Type, object, object>> arg46 = <>o__101.<>p__6;
								Type arg47 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
								if (<>o__101.<>p__5 == null)
								{
									<>o__101.<>p__5 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								arg48 = target30(arg43, arg44, target31(arg45, target32(arg46, arg47, <>o__101.<>p__5.Target(<>o__101.<>p__5, result, "ProfileIndex")), -1));
							}
							else
							{
								arg48 = obj;
							}
							if (target28(arg41, arg48))
							{
								if (<>o__101.<>p__14 == null)
								{
									<>o__101.<>p__14 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
								}
								Func<CallSite, object, string> target33 = <>o__101.<>p__14.Target;
								CallSite<Func<CallSite, object, string>> arg49 = <>o__101.<>p__14;
								if (<>o__101.<>p__13 == null)
								{
									<>o__101.<>p__13 = CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "TransferToName", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[3]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, MyFanManager_RamFan1p5, string, object, object> target34 = <>o__101.<>p__13.Target;
								CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>> arg50 = <>o__101.<>p__13;
								string arg51 = "OPERATING_GAMING_MODE";
								if (<>o__101.<>p__12 == null)
								{
									<>o__101.<>p__12 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, Type, object, object> target35 = <>o__101.<>p__12.Target;
								CallSite<Func<CallSite, Type, object, object>> arg52 = <>o__101.<>p__12;
								Type arg53 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
								if (<>o__101.<>p__11 == null)
								{
									<>o__101.<>p__11 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								string text4 = target33(arg49, target34(arg50, myFanManager_RamFan1p, arg51, target35(arg52, arg53, <>o__101.<>p__11.Target(<>o__101.<>p__11, result, "ProfileIndex"))));
								myFanManager_RamFan1p.SetOperatingModeProfileIndexThread(text4);
								if (CustomizeCtrl.GetCustomId() == 43 || CustomizeCtrl.GetCustomId() == 39)
								{
									if (text4 == "M1P2" && myFanManager_RamFan1p.projectID == 5894)
									{
										myFanManager_RamFan1p.UserSet_FanBoost(1u);
									}
									else
									{
										myFanManager_RamFan1p.UserSet_FanBoost(0u);
									}
								}
								myFanManager_RamFan1p._OPperatingMode = 1;
							}
							else
							{
								string modeProfileIndex5 = myFanManager_RamFan1p.TransferToName("OPERATING_GAMING_MODE", myFanManager_RamFan1p.m_Option.GamingProfileIndex);
								myFanManager_RamFan1p.SetOperatingModeProfileIndexThread(modeProfileIndex5);
								myFanManager_RamFan1p.UpdateStatusToClient(1);
							}
							string text5 = "Mode changed to: ";
							uint operatingMode = myFanManager_RamFan1p.m_Option.OperatingMode;
							LogCtrl.Write(text5 + ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode));
							break;
						}
						case 'O':
						{
							if (!(text == "OPERATING_OFFICE_MODE"))
							{
								break;
							}
							if (<>o__101.<>p__22 == null)
							{
								<>o__101.<>p__22 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							Func<CallSite, object, bool> target36 = <>o__101.<>p__22.Target;
							CallSite<Func<CallSite, object, bool>> arg54 = <>o__101.<>p__22;
							if (<>o__101.<>p__16 == null)
							{
								<>o__101.<>p__16 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							Func<CallSite, object, object, object> target37 = <>o__101.<>p__16.Target;
							CallSite<Func<CallSite, object, object, object>> arg55 = <>o__101.<>p__16;
							if (<>o__101.<>p__15 == null)
							{
								<>o__101.<>p__15 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							object obj = target37(arg55, <>o__101.<>p__15.Target(<>o__101.<>p__15, result, "ProfileIndex"), null);
							if (<>o__101.<>p__21 == null)
							{
								<>o__101.<>p__21 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							object arg61;
							if (!<>o__101.<>p__21.Target(<>o__101.<>p__21, obj))
							{
								if (<>o__101.<>p__20 == null)
								{
									<>o__101.<>p__20 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, object, object, object> target38 = <>o__101.<>p__20.Target;
								CallSite<Func<CallSite, object, object, object>> arg56 = <>o__101.<>p__20;
								object arg57 = obj;
								if (<>o__101.<>p__19 == null)
								{
									<>o__101.<>p__19 = CallSite<Func<CallSite, object, int, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.GreaterThan, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								Func<CallSite, object, int, object> target39 = <>o__101.<>p__19.Target;
								CallSite<Func<CallSite, object, int, object>> arg58 = <>o__101.<>p__19;
								if (<>o__101.<>p__18 == null)
								{
									<>o__101.<>p__18 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, Type, object, object> target40 = <>o__101.<>p__18.Target;
								CallSite<Func<CallSite, Type, object, object>> arg59 = <>o__101.<>p__18;
								Type arg60 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
								if (<>o__101.<>p__17 == null)
								{
									<>o__101.<>p__17 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								arg61 = target38(arg56, arg57, target39(arg58, target40(arg59, arg60, <>o__101.<>p__17.Target(<>o__101.<>p__17, result, "ProfileIndex")), -1));
							}
							else
							{
								arg61 = obj;
							}
							if (target36(arg54, arg61))
							{
								if (<>o__101.<>p__26 == null)
								{
									<>o__101.<>p__26 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
								}
								Func<CallSite, object, string> target41 = <>o__101.<>p__26.Target;
								CallSite<Func<CallSite, object, string>> arg62 = <>o__101.<>p__26;
								if (<>o__101.<>p__25 == null)
								{
									<>o__101.<>p__25 = CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "TransferToName", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[3]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, MyFanManager_RamFan1p5, string, object, object> target42 = <>o__101.<>p__25.Target;
								CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>> arg63 = <>o__101.<>p__25;
								string arg64 = "OPERATING_OFFICE_MODE";
								if (<>o__101.<>p__24 == null)
								{
									<>o__101.<>p__24 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, Type, object, object> target43 = <>o__101.<>p__24.Target;
								CallSite<Func<CallSite, Type, object, object>> arg65 = <>o__101.<>p__24;
								Type arg66 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
								if (<>o__101.<>p__23 == null)
								{
									<>o__101.<>p__23 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								string modeProfileIndex6 = target41(arg62, target42(arg63, myFanManager_RamFan1p, arg64, target43(arg65, arg66, <>o__101.<>p__23.Target(<>o__101.<>p__23, result, "ProfileIndex"))));
								myFanManager_RamFan1p.SetOperatingModeProfileIndexThread(modeProfileIndex6);
								myFanManager_RamFan1p._OPperatingMode = 0;
							}
							else
							{
								string modeProfileIndex7 = myFanManager_RamFan1p.TransferToName("OPERATING_OFFICE_MODE", myFanManager_RamFan1p.m_Option.OfficeProfileIndex);
								myFanManager_RamFan1p.SetOperatingModeProfileIndexThread(modeProfileIndex7);
								myFanManager_RamFan1p.UpdateStatusToClient(1);
							}
							string text6 = "Mode changed to: ";
							uint operatingMode = myFanManager_RamFan1p.m_Option.OperatingMode;
							LogCtrl.Write(text6 + ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode));
							break;
						}
						case 'C':
						{
							if (!(text == "OPERATING_CUSTOM_MODE"))
							{
								break;
							}
							if (<>o__101.<>p__46 == null)
							{
								<>o__101.<>p__46 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							Func<CallSite, object, bool> target20 = <>o__101.<>p__46.Target;
							CallSite<Func<CallSite, object, bool>> arg28 = <>o__101.<>p__46;
							if (<>o__101.<>p__40 == null)
							{
								<>o__101.<>p__40 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							Func<CallSite, object, object, object> target21 = <>o__101.<>p__40.Target;
							CallSite<Func<CallSite, object, object, object>> arg29 = <>o__101.<>p__40;
							if (<>o__101.<>p__39 == null)
							{
								<>o__101.<>p__39 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							object obj = target21(arg29, <>o__101.<>p__39.Target(<>o__101.<>p__39, result, "ProfileIndex"), null);
							if (<>o__101.<>p__45 == null)
							{
								<>o__101.<>p__45 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							object arg35;
							if (!<>o__101.<>p__45.Target(<>o__101.<>p__45, obj))
							{
								if (<>o__101.<>p__44 == null)
								{
									<>o__101.<>p__44 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, object, object, object> target22 = <>o__101.<>p__44.Target;
								CallSite<Func<CallSite, object, object, object>> arg30 = <>o__101.<>p__44;
								object arg31 = obj;
								if (<>o__101.<>p__43 == null)
								{
									<>o__101.<>p__43 = CallSite<Func<CallSite, object, int, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.GreaterThan, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								Func<CallSite, object, int, object> target23 = <>o__101.<>p__43.Target;
								CallSite<Func<CallSite, object, int, object>> arg32 = <>o__101.<>p__43;
								if (<>o__101.<>p__42 == null)
								{
									<>o__101.<>p__42 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, Type, object, object> target24 = <>o__101.<>p__42.Target;
								CallSite<Func<CallSite, Type, object, object>> arg33 = <>o__101.<>p__42;
								Type arg34 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
								if (<>o__101.<>p__41 == null)
								{
									<>o__101.<>p__41 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								arg35 = target22(arg30, arg31, target23(arg32, target24(arg33, arg34, <>o__101.<>p__41.Target(<>o__101.<>p__41, result, "ProfileIndex")), -1));
							}
							else
							{
								arg35 = obj;
							}
							if (target20(arg28, arg35))
							{
								if (<>o__101.<>p__50 == null)
								{
									<>o__101.<>p__50 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
								}
								Func<CallSite, object, string> target25 = <>o__101.<>p__50.Target;
								CallSite<Func<CallSite, object, string>> arg36 = <>o__101.<>p__50;
								if (<>o__101.<>p__49 == null)
								{
									<>o__101.<>p__49 = CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "TransferToName", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[3]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, MyFanManager_RamFan1p5, string, object, object> target26 = <>o__101.<>p__49.Target;
								CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>> arg37 = <>o__101.<>p__49;
								string arg38 = "OPERATING_CUSTOM_MODE";
								if (<>o__101.<>p__48 == null)
								{
									<>o__101.<>p__48 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, Type, object, object> target27 = <>o__101.<>p__48.Target;
								CallSite<Func<CallSite, Type, object, object>> arg39 = <>o__101.<>p__48;
								Type arg40 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
								if (<>o__101.<>p__47 == null)
								{
									<>o__101.<>p__47 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								string modeProfileIndex3 = target25(arg36, target26(arg37, myFanManager_RamFan1p, arg38, target27(arg39, arg40, <>o__101.<>p__47.Target(<>o__101.<>p__47, result, "ProfileIndex"))));
								myFanManager_RamFan1p.LoadOverClockingValue(myFanManager_RamFan1p.currentProfile.OverClockingEnabled);
								myFanManager_RamFan1p.SetOperatingModeProfileIndexThread(modeProfileIndex3);
								object obj2 = ResolvedDelegates.System_String_Concat__29;
								string text3 = "Mode changed to: ";
								uint operatingMode = myFanManager_RamFan1p.m_Option.OperatingMode;
								LogCtrl.Write(obj2(text3, ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode)));
							}
							else
							{
								string modeProfileIndex4 = myFanManager_RamFan1p.TransferToName("OPERATING_CUSTOM_MODE", myFanManager_RamFan1p.m_Option.CustomProfileIndex);
								myFanManager_RamFan1p.SetOperatingModeProfileIndexThread(modeProfileIndex4);
								myFanManager_RamFan1p.UpdateStatusToClient(1);
							}
							break;
						}
						}
						break;
					case 25:
						switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)text, 0))
						{
						case 'O':
						{
							if (!(text == "OPERATING_GAME_WHITE_LIST"))
							{
								break;
							}
							if (<>o__101.<>p__53 == null)
							{
								<>o__101.<>p__53 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							Func<CallSite, object, bool> target16 = <>o__101.<>p__53.Target;
							CallSite<Func<CallSite, object, bool>> arg23 = <>o__101.<>p__53;
							if (<>o__101.<>p__52 == null)
							{
								<>o__101.<>p__52 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							Func<CallSite, object, object, object> target17 = <>o__101.<>p__52.Target;
							CallSite<Func<CallSite, object, object, object>> arg24 = <>o__101.<>p__52;
							if (<>o__101.<>p__51 == null)
							{
								<>o__101.<>p__51 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							if (!target16(arg23, target17(arg24, <>o__101.<>p__51.Target(<>o__101.<>p__51, result, "GameWhitelistSwitch"), null)))
							{
								break;
							}
							try
							{
								if (<>o__101.<>p__56 == null)
								{
									<>o__101.<>p__56 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
								}
								Func<CallSite, object, int> target18 = <>o__101.<>p__56.Target;
								CallSite<Func<CallSite, object, int>> arg25 = <>o__101.<>p__56;
								if (<>o__101.<>p__55 == null)
								{
									<>o__101.<>p__55 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, Type, object, object> target19 = <>o__101.<>p__55.Target;
								CallSite<Func<CallSite, Type, object, object>> arg26 = <>o__101.<>p__55;
								Type arg27 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
								if (<>o__101.<>p__54 == null)
								{
									<>o__101.<>p__54 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								myFanManager_RamFan1p.gameWhitelistSwitch = target18(arg25, target19(arg26, arg27, <>o__101.<>p__54.Target(<>o__101.<>p__54, result, "GameWhitelistSwitch")));
							}
							catch (Exception)
							{
							}
							RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\MySetting", "GameWhitelistSwitch", myFanManager_RamFan1p.gameWhitelistSwitch, RegistryValueKind.DWord);
							LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("GameWhitelistSwitch wrtire: ", ResolvedDelegates.System_Int32_ToString__55(ref myFanManager_RamFan1p.gameWhitelistSwitch)), "Receive", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 832);
							myFanManager_RamFan1p.UpdateStatusToClient(1);
							break;
						}
						case 'S':
							if (text == "SET_OPERATING_MODE_DETAIL")
							{
								if (<>o__101.<>p__57 == null)
								{
									<>o__101.<>p__57 = CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "UserSet_Mode_Detail", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								<>o__101.<>p__57.Target(<>o__101.<>p__57, myFanManager_RamFan1p, result);
							}
							break;
						}
						break;
					case 29:
						switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)text, 0))
						{
						case 'R':
							if (text == "RESTORE_OPERATING_MODE_DETAIL")
							{
								myFanManager_RamFan1p.UserRestore_Mode_Details();
								myFanManager_RamFan1p.UpdateStatusToClient(1);
							}
							break;
						case 'S':
							if (text == "SET_CUSTOM_PROFILE_OSD_STRING")
							{
								if (<>o__101.<>p__63 == null)
								{
									<>o__101.<>p__63 = CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "WriteCustomOSDstringToJson", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Action<CallSite, MyFanManager_RamFan1p5, object> target15 = <>o__101.<>p__63.Target;
								CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>> arg22 = <>o__101.<>p__63;
								if (<>o__101.<>p__62 == null)
								{
									<>o__101.<>p__62 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								target15(arg22, myFanManager_RamFan1p, <>o__101.<>p__62.Target(<>o__101.<>p__62, result, "ProfileName"));
							}
							break;
						}
						break;
					case 27:
						switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)text, 0))
						{
						case 'G':
							if (text == "GET_FAN_SPEED_CURVE_SETTING")
							{
								myFanManager_RamFan1p.FanTable.GetFanTable(myFanManager_RamFan1p.currentProfile.FAN.TableName);
							}
							break;
						case 'S':
							if (!(text == "SET_FAN_SPEED_CURVE_SETTING"))
							{
								if (ResolvedDelegates.System_String_op_Equality__18(text, "SET_CPU_CORE_OFFSET_EXTREME"))
								{
									if (<>o__101.<>p__66 == null)
									{
										<>o__101.<>p__66 = CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "UserSet_Cpu_Core_Offset_Extreme", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									Action<CallSite, MyFanManager_RamFan1p5, object> target14 = <>o__101.<>p__66.Target;
									CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>> arg21 = <>o__101.<>p__66;
									if (<>o__101.<>p__65 == null)
									{
										<>o__101.<>p__65 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
										}));
									}
									target14(arg21, myFanManager_RamFan1p, <>o__101.<>p__65.Target(<>o__101.<>p__65, result, "EXTREME"));
								}
							}
							else if (myFanManager_RamFan1p.m_FanSafetyProtect == 0)
							{
								if (<>o__101.<>p__58 == null)
								{
									<>o__101.<>p__58 = CallSite<Action<CallSite, MyFanTableCtrl, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetFanTableSetting", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[3]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								<>o__101.<>p__58.Target(<>o__101.<>p__58, myFanManager_RamFan1p.FanTable, result, arg4: true);
							}
							else
							{
								if (<>o__101.<>p__59 == null)
								{
									<>o__101.<>p__59 = CallSite<Action<CallSite, MyFanTableCtrl, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetFanTableSetting", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[3]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								<>o__101.<>p__59.Target(<>o__101.<>p__59, myFanManager_RamFan1p.FanTable, result, arg4: false);
							}
							break;
						}
						break;
					case 26:
						switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)text, 20))
						{
						case 'E':
							if (text == "SET_FAN_CONTROL_RESPECTIVE" && myFanManager_RamFan1p.m_FanSafetyProtect == 0)
							{
								if (<>o__101.<>p__60 == null)
								{
									<>o__101.<>p__60 = CallSite<Action<CallSite, MyFanTableCtrl, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetFanControlRespective", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								<>o__101.<>p__60.Target(<>o__101.<>p__60, myFanManager_RamFan1p.FanTable, result);
							}
							break;
						case 'D':
							if (text == "SET_CPU_CORE_OFFSET_DETAIL")
							{
								if (<>o__101.<>p__64 == null)
								{
									<>o__101.<>p__64 = CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "UserSet_Cpu_Core_Offset_Detail", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								<>o__101.<>p__64.Target(<>o__101.<>p__64, myFanManager_RamFan1p, result);
							}
							break;
						case 'S':
							if (text == "SET_CPU_CORE_OFFSET_SILENT")
							{
								if (<>o__101.<>p__68 == null)
								{
									<>o__101.<>p__68 = CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "UserSet_Cpu_Core_Offset_Silent", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Action<CallSite, MyFanManager_RamFan1p5, object> target13 = <>o__101.<>p__68.Target;
								CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>> arg20 = <>o__101.<>p__68;
								if (<>o__101.<>p__67 == null)
								{
									<>o__101.<>p__67 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								target13(arg20, myFanManager_RamFan1p, <>o__101.<>p__67.Target(<>o__101.<>p__67, result, "SILENT"));
							}
							break;
						}
						break;
					case 9:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "GETSTATUS") && myFanManager_RamFan1p.m_bInit)
						{
							myFanManager_RamFan1p.UpdateStatusToClient(1);
						}
						break;
					case 7:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "DEFAULT"))
						{
							myFanManager_RamFan1p.UserSet_Default();
							myFanManager_RamFan1p.UpdateStatusToClient(1);
						}
						break;
					case 12:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "FAN_BOOST_ON"))
						{
							myFanManager_RamFan1p.UserSet_FanBoost(1u);
						}
						break;
					case 13:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "FAN_BOOST_OFF"))
						{
							myFanManager_RamFan1p.UserSet_FanBoost(0u);
						}
						break;
					case 15:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "COOLING_MODE_ON"))
						{
							myFanManager_RamFan1p.UserSet_CoolingMode(1);
						}
						break;
					case 16:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "COOLING_MODE_OFF"))
						{
							myFanManager_RamFan1p.UserSet_CoolingMode(0);
						}
						break;
					case 20:
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(text, "OPERATING_TURBO_MODE"))
						{
							break;
						}
						if (<>o__101.<>p__34 == null)
						{
							<>o__101.<>p__34 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						Func<CallSite, object, bool> target5 = <>o__101.<>p__34.Target;
						CallSite<Func<CallSite, object, bool>> arg7 = <>o__101.<>p__34;
						if (<>o__101.<>p__28 == null)
						{
							<>o__101.<>p__28 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						Func<CallSite, object, object, object> target6 = <>o__101.<>p__28.Target;
						CallSite<Func<CallSite, object, object, object>> arg8 = <>o__101.<>p__28;
						if (<>o__101.<>p__27 == null)
						{
							<>o__101.<>p__27 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						object obj = target6(arg8, <>o__101.<>p__27.Target(<>o__101.<>p__27, result, "ProfileIndex"), null);
						if (<>o__101.<>p__33 == null)
						{
							<>o__101.<>p__33 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						object arg14;
						if (!<>o__101.<>p__33.Target(<>o__101.<>p__33, obj))
						{
							if (<>o__101.<>p__32 == null)
							{
								<>o__101.<>p__32 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, object, object, object> target7 = <>o__101.<>p__32.Target;
							CallSite<Func<CallSite, object, object, object>> arg9 = <>o__101.<>p__32;
							object arg10 = obj;
							if (<>o__101.<>p__31 == null)
							{
								<>o__101.<>p__31 = CallSite<Func<CallSite, object, int, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.GreaterThan, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							Func<CallSite, object, int, object> target8 = <>o__101.<>p__31.Target;
							CallSite<Func<CallSite, object, int, object>> arg11 = <>o__101.<>p__31;
							if (<>o__101.<>p__30 == null)
							{
								<>o__101.<>p__30 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, Type, object, object> target9 = <>o__101.<>p__30.Target;
							CallSite<Func<CallSite, Type, object, object>> arg12 = <>o__101.<>p__30;
							Type arg13 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
							if (<>o__101.<>p__29 == null)
							{
								<>o__101.<>p__29 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							arg14 = target7(arg9, arg10, target8(arg11, target9(arg12, arg13, <>o__101.<>p__29.Target(<>o__101.<>p__29, result, "ProfileIndex")), -1));
						}
						else
						{
							arg14 = obj;
						}
						if (target5(arg7, arg14))
						{
							if (<>o__101.<>p__38 == null)
							{
								<>o__101.<>p__38 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
							}
							Func<CallSite, object, string> target10 = <>o__101.<>p__38.Target;
							CallSite<Func<CallSite, object, string>> arg15 = <>o__101.<>p__38;
							if (<>o__101.<>p__37 == null)
							{
								<>o__101.<>p__37 = CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "TransferToName", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[3]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, MyFanManager_RamFan1p5, string, object, object> target11 = <>o__101.<>p__37.Target;
							CallSite<Func<CallSite, MyFanManager_RamFan1p5, string, object, object>> arg16 = <>o__101.<>p__37;
							string arg17 = "OPERATING_TURBO_MODE";
							if (<>o__101.<>p__36 == null)
							{
								<>o__101.<>p__36 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToUInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, Type, object, object> target12 = <>o__101.<>p__36.Target;
							CallSite<Func<CallSite, Type, object, object>> arg18 = <>o__101.<>p__36;
							Type arg19 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
							if (<>o__101.<>p__35 == null)
							{
								<>o__101.<>p__35 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							string modeProfileIndex = target10(arg15, target11(arg16, myFanManager_RamFan1p, arg17, target12(arg18, arg19, <>o__101.<>p__35.Target(<>o__101.<>p__35, result, "ProfileIndex"))));
							myFanManager_RamFan1p.SetOperatingModeProfileIndexThread(modeProfileIndex);
							myFanManager_RamFan1p._OPperatingMode = 2;
						}
						else
						{
							string modeProfileIndex2 = myFanManager_RamFan1p.TransferToName("OPERATING_TURBO_MODE", myFanManager_RamFan1p.m_Option.TurboProfileIndex);
							myFanManager_RamFan1p.SetOperatingModeProfileIndexThread(modeProfileIndex2);
							myFanManager_RamFan1p.UpdateStatusToClient(1);
						}
						string text2 = "Mode changed to: ";
						uint operatingMode = myFanManager_RamFan1p.m_Option.OperatingMode;
						LogCtrl.Write(text2 + ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode));
						break;
					}
					case 35:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "RESTORE_FAN_SPEED_CURVE_SETTING_ALL"))
						{
							myFanManager_RamFan1p.FanTable.RestoreDefaultFanTableAll();
						}
						break;
					case 31:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "RESTORE_FAN_SPEED_CURVE_SETTING"))
						{
							if (<>o__101.<>p__61 == null)
							{
								<>o__101.<>p__61 = CallSite<Action<CallSite, MyFanTableCtrl, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "RestoreDefaultFanTable", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							<>o__101.<>p__61.Target(<>o__101.<>p__61, myFanManager_RamFan1p.FanTable, result);
						}
						break;
					case 22:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "SET_DEFAULT_NOTIFY_OFF"))
						{
							myFanManager_RamFan1p.UserSet_DefaultNotifyStatus(0u);
						}
						break;
					case 33:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "SET_FAN_SAFETY_PROTECT_NOTIFY_OFF"))
						{
							myFanManager_RamFan1p.UserSet_FanSafetyProtectNotifyStatus(0u);
						}
						break;
					case 14:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "TestModeSwitch"))
						{
							myFanManager_RamFan1p.ModeSwitchChanged();
						}
						break;
					case 17:
						if (ResolvedDelegates.System_String_op_Equality__18(text, "SET_BALANCE_SMART"))
						{
							if (<>o__101.<>p__71 == null)
							{
								<>o__101.<>p__71 = CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "UserSetSmartBalance", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Action<CallSite, MyFanManager_RamFan1p5, object> target3 = <>o__101.<>p__71.Target;
							CallSite<Action<CallSite, MyFanManager_RamFan1p5, object>> arg4 = <>o__101.<>p__71;
							if (<>o__101.<>p__70 == null)
							{
								<>o__101.<>p__70 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, Type, object, object> target4 = <>o__101.<>p__70.Target;
							CallSite<Func<CallSite, Type, object, object>> arg5 = <>o__101.<>p__70;
							Type arg6 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
							if (<>o__101.<>p__69 == null)
							{
								<>o__101.<>p__69 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							target3(arg4, myFanManager_RamFan1p, target4(arg5, arg6, <>o__101.<>p__69.Target(<>o__101.<>p__69, result, "SmartBalanceSwitch")));
						}
						break;
					case 8:
					case 10:
					case 11:
					case 18:
					case 19:
					case 23:
					case 24:
					case 28:
					case 30:
					case 32:
					case 34:
						break;
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
	private struct <SetFanTableThread>d__201 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public MyFanManager_RamFan1p5 <>4__this;

		public string _tableName;

		private <>c__DisplayClass201_0 <>8__1;

		private TaskAwaiter <>u__1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			int num = <>1__state;
			MyFanManager_RamFan1p5 myFanManager_RamFan1p = <>4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = <>u__1;
					<>u__1 = default;
					num = (<>1__state = -1);
					goto IL_010e;
				}
				<>8__1 = new <>c__DisplayClass201_0();
				<>8__1.<>4__this = <>4__this;
				<>8__1._tableName = _tableName;
				if (<>8__1._tableName != null)
				{
					LogCtrl.TraceMessage(<>8__1._tableName, "SetFanTableThread", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5112);
					if (ResolvedDelegates.System_Threading_SemaphoreSlim_get_CurrentCount__509(myFanManager_RamFan1p.FansemporeSlim2) != 0)
					{
						awaiter = myFanManager_RamFan1p.FansemporeSlim2.WaitAsync().GetAwaiter();
						if (!ResolvedDelegates.System_Runtime_CompilerServices_TaskAwaiter_get_IsCompleted__24(ref awaiter))
						{
							num = (<>1__state = 0);
							<>u__1 = awaiter;
							<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_010e;
					}
					myFanManager_RamFan1p.LastCommand2 = <>8__1._tableName;
				}
				goto end_IL_000e;
				IL_010e:
				awaiter.GetResult();
				try
				{
					ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(myFanManager_RamFan1p.stopwatch2);
					ResolvedDelegates.System_Diagnostics_Stopwatch_Start__153(myFanManager_RamFan1p.stopwatch2);
					ResolvedDelegates.System_Threading_Thread_Start__178(new Thread(() =>
					{
						<>8__1.<>4__this.FanTable.SetFanTable(<>8__1._tableName);
					}));
					ResolvedDelegates.System_Diagnostics_Stopwatch_Stop__164(myFanManager_RamFan1p.stopwatch2);
					ResolvedDelegates.System_Diagnostics_Stopwatch_get_ElapsedMilliseconds__155(myFanManager_RamFan1p.stopwatch2);
				}
				catch
				{
				}
				finally
				{
					if (num < 0)
					{
						ResolvedDelegates.System_Threading_SemaphoreSlim_Release__182(myFanManager_RamFan1p.FansemporeSlim2);
						if (ResolvedDelegates.System_String_op_Inequality__40(myFanManager_RamFan1p.LastCommand2, "M1T1"))
						{
							ResolvedDelegates.System_Threading_Thread_Start__178(new Thread(() =>
							{
								<>8__1.<>4__this.FanTable.SetFanTable(<>8__1._tableName);
							}));
							myFanManager_RamFan1p.LastCommand2 = "M1T1";
						}
					}
				}
				end_IL_000e:;
			}
			catch (Exception ex)
			{
				<>1__state = -2;
				<>8__1 = null;
				ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_SetException__194(ref <>t__builder, ex);
				return;
			}
			<>1__state = -2;
			<>8__1 = null;
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
	private struct <SetModeSwitchChangeThread>d__217 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public int _mode;

		public MyFanManager_RamFan1p5 <>4__this;

		public bool fromscancode;

		private TaskAwaiter <>u__1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			int num = <>1__state;
			MyFanManager_RamFan1p5 myFanManager_RamFan1p = <>4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = <>u__1;
					<>u__1 = default;
					num = (<>1__state = -1);
					goto IL_00b0;
				}
				if (_mode != -1)
				{
					if (myFanManager_RamFan1p.FansemporeSlim4.CurrentCount != 0)
					{
						awaiter = myFanManager_RamFan1p.FansemporeSlim4.WaitAsync().GetAwaiter();
						if (!ResolvedDelegates.System_Runtime_CompilerServices_TaskAwaiter_get_IsCompleted__24(ref awaiter))
						{
							num = (<>1__state = 0);
							<>u__1 = awaiter;
							<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_00b0;
					}
					myFanManager_RamFan1p.LastCommand4 = _mode;
					myFanManager_RamFan1p.LastCommandScancode = fromscancode;
				}
				goto end_IL_000e;
				IL_00b0:
				awaiter.GetResult();
				try
				{
					ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(myFanManager_RamFan1p.stopwatch4);
					ResolvedDelegates.System_Diagnostics_Stopwatch_Start__153(myFanManager_RamFan1p.stopwatch4);
					myFanManager_RamFan1p.SetModeSwitchChange(_mode, fromscancode);
					ResolvedDelegates.System_Diagnostics_Stopwatch_Stop__164(myFanManager_RamFan1p.stopwatch4);
					ResolvedDelegates.System_Diagnostics_Stopwatch_get_ElapsedMilliseconds__155(myFanManager_RamFan1p.stopwatch4);
				}
				catch
				{
				}
				finally
				{
					if (num < 0)
					{
						ResolvedDelegates.System_Threading_SemaphoreSlim_Release__182(myFanManager_RamFan1p.FansemporeSlim4);
						if (myFanManager_RamFan1p.LastCommand4 != -1)
						{
							myFanManager_RamFan1p.SetModeSwitchChange(myFanManager_RamFan1p.LastCommand4, myFanManager_RamFan1p.LastCommandScancode);
							myFanManager_RamFan1p.LastCommand4 = -1;
							myFanManager_RamFan1p.LastCommandScancode = false;
						}
					}
				}
				end_IL_000e:;
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
	private struct <SetOperatingModeProfileIndexThread>d__211 : IAsyncStateMachine
	{
		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public string _modeProfileIndex;

		public MyFanManager_RamFan1p5 <>4__this;

		private TaskAwaiter <>u__1;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void MoveNext()
		{
			int num = <>1__state;
			MyFanManager_RamFan1p5 myFanManager_RamFan1p = <>4__this;
			try
			{
				TaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = <>u__1;
					<>u__1 = default;
					num = (<>1__state = -1);
					goto IL_00f7;
				}
				if (_modeProfileIndex != null)
				{
					LogCtrl.TraceMessage(_modeProfileIndex, "SetOperatingModeProfileIndexThread", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5349);
					if (ResolvedDelegates.System_Threading_SemaphoreSlim_get_CurrentCount__509(myFanManager_RamFan1p.FansemporeSlim3) != 0)
					{
						awaiter = myFanManager_RamFan1p.FansemporeSlim3.WaitAsync().GetAwaiter();
						if (!ResolvedDelegates.System_Runtime_CompilerServices_TaskAwaiter_get_IsCompleted__24(ref awaiter))
						{
							num = (<>1__state = 0);
							<>u__1 = awaiter;
							<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_00f7;
					}
					myFanManager_RamFan1p.LastCommand3 = _modeProfileIndex;
					LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("SetOperatingModeProfileIndexThread LastCommand ", myFanManager_RamFan1p.LastCommand3));
				}
				goto end_IL_000e;
				IL_00f7:
				awaiter.GetResult();
				try
				{
					ResolvedDelegates.System_Diagnostics_Stopwatch_Reset__156(myFanManager_RamFan1p.stopwatch3);
					ResolvedDelegates.System_Diagnostics_Stopwatch_Start__153(myFanManager_RamFan1p.stopwatch3);
					myFanManager_RamFan1p.SetOperatingModeProfileIndex(_modeProfileIndex);
					ResolvedDelegates.System_Diagnostics_Stopwatch_Stop__164(myFanManager_RamFan1p.stopwatch3);
					long num2 = ResolvedDelegates.System_Diagnostics_Stopwatch_get_ElapsedMilliseconds__155(myFanManager_RamFan1p.stopwatch3);
					myFanManager_RamFan1p.reallyCommand = _modeProfileIndex;
					LogCtrl.Write(ResolvedDelegates.System_String_Concat__120("ModeProfileIndex : ", _modeProfileIndex, " SetOperatingModeProfileIndexThread time: ", ResolvedDelegates.System_Int64_ToString__458(ref num2)));
				}
				catch
				{
				}
				finally
				{
					if (num < 0)
					{
						ResolvedDelegates.System_Threading_SemaphoreSlim_Release__182(myFanManager_RamFan1p.FansemporeSlim3);
						if (myFanManager_RamFan1p.LastCommand3 != null)
						{
							string lastCommand = myFanManager_RamFan1p.LastCommand3;
							myFanManager_RamFan1p.LastCommand3 = null;
							LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("SetOperatingModeProfileIndexThread Last command setting ! ", lastCommand));
							myFanManager_RamFan1p.SetOperatingModeProfileIndexThread(lastCommand, IsLastCommand: true);
						}
					}
				}
				end_IL_000e:;
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

	private static readonly MyFanManager_RamFan1p5 myfanMan1p5;

	private BIOS_PROJECT_ID BiosProjectID;

	private MyEcCtrl EcCtrl = MyEcCtrl.Instance;

	private string m_RegistryPath = "\\OEM\\GamingCenter2\\SMAPCTable";

	private string m_path = ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_IO_FileInfo_get_DirectoryName__98(new FileInfo(ResolvedDelegates.System_Reflection_Assembly_get_Location__97(ResolvedDelegates.System_Reflection_Assembly_GetExecutingAssembly__96()))), "\\UserPofiles");

	private Dispatcher _dispatcher;

	private bool m_bInit;

	private GpuFeatures cGpuFeatures = new GpuFeatures();

	private MyFanTableCtrl FanTable = MyFanTableCtrl.Instance;

	private bool m_OcSupport = true;

	private bool m_IsNvGpu;

	private bool m_IsAMDPlatform;

	private bool isFirstLaunch;

	private uint m_DefaultMode;

	private int m_FanSafetyProtect;

	private int m_AmdCpuOverClockSupport;

	private bool m_FanBoostBtnSupport = true;

	private bool m_GpuWhisperModeSupport;

	private int WhisperModeMinFpsMaximum = 60;

	private const int WhisperModeMinFpsMinimum = 30;

	private MainOption m_Option = new MainOption();

	private PLDefaultValue GamingModePLDefaultValue = new PLDefaultValue();

	private PLDefaultValue OfficeModePLDefaultValue = new PLDefaultValue();

	private PLDefaultValue TurboModePLDefaultValue = new PLDefaultValue();

	private GpuThermalLimitInfo GpuThermalLimitInfo = new GpuThermalLimitInfo();

	private NvramVariableInfo NvramVariableInfo = new NvramVariableInfo();

	private SMAPCTABLE_STRUCT _SmartApcTable;

	private int CpuPL1Minimum = 10;

	private int CpuPL2Minimum = 10;

	private int CpuPL4Minimum = 10;

	private bool m_GpuConfigurableTGPSupport;

	private int m_GpuConfigurableTGPMaximum;

	private int GpuCoreClockOffsetMaximum = 250;

	private int GpuCoreClockOffsetMaximumHWOC = 250;

	private const int GpuCoreClockOffsetMinimum = 0;

	private int GpuMemoryClockOffsetMaximum = 1000;

	private int GpuMemoryClockOffsetMaximumHWOC = 1800;

	private int GpuMemoryClockOffsetMinimum = -1000;

	private int GpuMemoryClockOffsetMinimumHWOC = -1800;

	private const int GpuDynamicBoostMinimum = 5;

	private int GpuDynamicBoostDefaultValue = 5;

	private bool m_GpuDynamicBoostSupport;

	private int CpuOffsetCoreVoltageMaximum;

	private int CpuOffsetCoreVoltageMinimum;

	private static bool m_IsTurboSubModeSupport;

	private static int m_IsTurboOCAdjustSupport;

	private static int m_IsTurboOCDefualtON;

	private static uint trubo43;

	private static uint trubo46;

	private byte _tjMax;

	private ModeProfile currentProfile = new ModeProfile();

	private ModeProfile Mode1_Profile1 = new ModeProfile();

	private ModeProfile Mode1_Profile2 = new ModeProfile();

	private ModeProfile Mode1_Profile3 = new ModeProfile();

	private ModeProfile Mode1_Profile4 = new ModeProfile();

	private ModeProfile Mode1_Profile5 = new ModeProfile();

	private ModeProfile Mode2_Profile1 = new ModeProfile();

	private ModeProfile Mode2_Profile2 = new ModeProfile();

	private ModeProfile Mode2_Profile3 = new ModeProfile();

	private ModeProfile Mode2_Profile4 = new ModeProfile();

	private ModeProfile Mode2_Profile5 = new ModeProfile();

	private ModeProfile Mode3_Profile1 = new ModeProfile();

	private ModeProfile Mode3_Profile2 = new ModeProfile();

	private ModeProfile Mode3_Profile3 = new ModeProfile();

	private ModeProfile Mode3_Profile4 = new ModeProfile();

	private ModeProfile Mode3_Profile5 = new ModeProfile();

	private ModeProfile Mode4_Profile1 = new ModeProfile();

	private ModeProfile Mode4_Profile2 = new ModeProfile();

	private ModeProfile Mode4_Profile3 = new ModeProfile();

	private ModeProfile Mode4_Profile4 = new ModeProfile();

	private ModeProfile Mode4_Profile5 = new ModeProfile();

	private GPUInfo cGpuInfo = new GPUInfo();

	private int m_nAppType = 1;

	private string m_sRegPath = "\\OEM\\GamingCenter2";

	private int projectID;

	private int _skuGN21;

	private bool HWOCSupportADL;

	private bool HWOCSupport;

	private bool _HWOCSupport;

	private bool _Enable;

	private bool roundFlag;

	private bool isAc;

	private bool SmartBalanceSupport;

	private AcpiCtrl AcpiModel = AcpiCtrl.Instance;

	private int _OPperatingMode;

	private string m_className = ResolvedDelegates.System_String_Concat__107("[", ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Reflection_MemberInfo_get_DeclaringType__280(MethodBase.GetCurrentMethod())), "] ");

	private int gameWhitelistSwitch;

	private bool HWOCMode;

	private SemaphoreSlim FansemporeSlim2 = new SemaphoreSlim(1, 1);

	private Stopwatch stopwatch2 = new Stopwatch();

	private string LastCommand2 = "M1T1";

	private SemaphoreSlim FansemporeSlim3 = new SemaphoreSlim(1, 1);

	private Stopwatch stopwatch3 = new Stopwatch();

	private string LastCommand3;

	private string reallyCommand;

	private SemaphoreSlim FansemporeSlim4 = new SemaphoreSlim(1, 1);

	private Stopwatch stopwatch4 = new Stopwatch();

	private int LastCommand4 = -1;

	private bool LastCommandScancode;

	public static MyFanManager_RamFan1p5 Instance
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return myfanMan1p5;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public MyFanManager_RamFan1p5()
	{
		m_nAppType = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, m_sRegPath, "AppType", 1);
		cGpuFeatures.Init();
		projectID = EcCtrl.GetProject2ExID(EcCtrl.GetProjectIdFromEC());
		_skuGN21 = EcCtrl.GetnModuleId_GPUFromEC();
		HWOCSupportADL = EcCtrl.IsSupportLC_HWOC_ADL();
		HWOCSupport = EcCtrl.IsSupportLiquidCoolingHWOC();
		if (HWOCSupportADL || HWOCSupport)
		{
			_HWOCSupport = true;
		}
		else
		{
			_HWOCSupport = false;
		}
		_tjMax = WMIEC.Instance.WMIGetECTjMax();
		SmartBalanceSupport = EcCtrl.GetSmartBalanceSupportFromEC() == 1;
		m_IsTurboSubModeSupport = WMIEC.Instance.WMIIsTurboSubModeSupport() == 85;
		if (IsUMA())
		{
			m_IsTurboSubModeSupport = false;
		}
		int num = (int)RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\Support", "APVersionCheck", 0);
		if (num == 0)
		{
			num = NvramVariable.GetFwVars().APVersionCheck;
		}
		if (num >= 25)
		{
			m_IsTurboOCAdjustSupport = ((WMIEC.Instance.WMITurboOCAdjustSupport() == 85) ? 1 : 0);
		}
		else
		{
			m_IsTurboOCAdjustSupport = 0;
		}
		int num2 = ((WMIEC.Instance.WMIIsSupport_IGPU_Mode() == 85) ? 1 : 0);
		m_IsTurboOCAdjustSupport |= num2;
		m_IsTurboOCDefualtON = ((WMIEC.Instance.WMITurboOCAdjustDefault() > 0) ? 1 : 0);
		if (m_IsTurboOCAdjustSupport == 1)
		{
			if (m_IsTurboSubModeSupport)
			{
				trubo43 = WMIEC.Instance.WMITurboOCAdjust43DBValue();
			}
			trubo46 = WMIEC.Instance.WMITurboOCAdjust46DBValue();
		}
		BiosProjectID = EcCtrl.GetBiosProjctID();
		isAc = EcCtrl.GetRoundSocketStatusFromEC() == 1 || num < 24;
		LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("this is AC", ResolvedDelegates.System_Boolean_ToString__147(ref isAc)), ".ctor", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 210);
		try
		{
			gameWhitelistSwitch = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\MySetting"), "GameWhitelistSwitch", 0);
		}
		catch (Exception ex)
		{
			gameWhitelistSwitch = 0;
			RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\MySetting", "GameWhitelistSwitch", gameWhitelistSwitch, RegistryValueKind.DWord);
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("GameWhitelistSwitch :", ResolvedDelegates.System_Exception_get_Message__58(ex)), ".ctor", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 219);
		}
		IsSupportCPUPL4DoubleFlag();
	}

	public void AdjustTGPOnModeChange()
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool IsUMA()
	{
		return EcCtrl.GetNvGpu() == 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public bool isTypeCCharging()
	{
		if ((EcCtrl.GetTypeCsatusFromEC() & 6) > 0)
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetHighPerformanceModeForUMA()
	{
		if (EcCtrl.IsUMA && isTypeCCharging())
		{
			if (CustomizeCtrl.GetIsUMATurbo() && m_Option.OperatingMode == 1)
			{
				MySettingManager.CreateInstance.SetHighPerformancePowerModeScancode(2);
			}
			else
			{
				MySettingManager.CreateInstance.SetHighPerformancePowerModeScancode((int)m_Option.OperatingMode);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void SwitchModePowerChange(bool isInit = false)
	{
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Auto Switch Mode... currentProfile :", currentProfile.Name), "SwitchModePowerChange", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 286);
		if (EcCtrl.GetRoundSocketStatusFromEC() == 0 && EcCtrl.GetNvGpu() == 1)
		{
			cGpuFeatures.SetGpuCoreFreqOffsetValue(0);
		}
		UpdateSmartBalanceSupport();
		if (NvramVariableInfo.APVersionCheck < 24)
		{
			return;
		}
		try
		{
			uint fanMode = GetFanMode();
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("Get EC perf mode: ", ResolvedDelegates.System_UInt32_ToString__78(ref fanMode)), "SwitchModePowerChange", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 304);
			if (EcCtrl.GetRoundSocketStatusFromEC() == 1)
			{
				isAc = true;
			}
			else
			{
				isAc = false;
			}
			uint num;
			if (m_Option.OperatingMode != fanMode)
			{
				switch (fanMode)
				{
				case 0u:
					m_Option.OperatingMode = 0u;
					WriteMainOptionToJson("OperatingMode", 0u);
					App.m_Osd.ShowOSDByName(TransToGlobalType(0u), "");
					break;
				case 1u:
					m_Option.OperatingMode = 1u;
					WriteMainOptionToJson("OperatingMode", 1u);
					App.m_Osd.ShowOSDByName(TransToGlobalType(1u), "");
					if (SmartBalanceSupport)
					{
						string text = "0";
						try
						{
							text = ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\MySetting"), "SmartBalanceSwitch", 0));
						}
						catch
						{
						}
						if (text == "1")
						{
							UserSetSmartBalance(1);
						}
						else
						{
							UserSetSmartBalance(0);
						}
					}
					else
					{
						ClearSmartBalanceEnable();
					}
					break;
				case 2u:
				{
					m_Option.OperatingMode = 2u;
					string text2 = ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "SilentPerformanceModeSwitch", 0));
					int num2 = ((m_IsTurboSubModeSupport && m_IsTurboOCAdjustSupport > 0 && ResolvedDelegates.System_String_op_Equality__18(text2, "0")) ? m_Option.TurboSilentGPUOCOffset : m_Option.TurboGPUOCOffset);
					if (num2 > 0)
					{
						cGpuFeatures.SetGpuCoreFreqOffsetValue(num2);
					}
					WriteMainOptionToJson("OperatingMode", 2u);
					if (m_IsTurboSubModeSupport)
					{
						if (ResolvedDelegates.System_String_op_Equality__18(text2, "1"))
						{
							UserSet_Cpu_Core_Offset_Extreme(1);
						}
						else if (text2 == "0")
						{
							UserSet_Cpu_Core_Offset_Silent(1);
						}
					}
					App.m_Osd.ShowOSDByName(TransToGlobalType(2u), "");
					break;
				}
				case 3u:
					m_Option.OperatingMode = 3u;
					WriteMainOptionToJson("OperatingMode", 3u);
					if (CustomizeCtrl.GetCustomId() == 9)
					{
						App.m_Osd.ShowOSDByName(TransToGlobalType(3u), ResolvedDelegates.System_UInt32_ToString__78(ref m_Option.CustomProfileIndex));
					}
					else
					{
						OSDManager osd = App.m_Osd;
						uint mode = TransToGlobalType(3u);
						num = m_Option.CustomProfileIndex + 1;
						osd.ShowOSDByName(mode, ResolvedDelegates.System_UInt32_ToString__78(ref num));
					}
					LoadOverClockingValue(currentProfile.OverClockingEnabled);
					break;
				}
			}
			else
			{
				switch (fanMode)
				{
				case 2u:
				{
					string text4 = ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "SilentPerformanceModeSwitch", 0));
					if (m_IsTurboSubModeSupport)
					{
						if (ResolvedDelegates.System_String_op_Equality__18(text4, "1"))
						{
							UserSet_Cpu_Core_Offset_Extreme(1);
						}
						else if (text4 == "0")
						{
							UserSet_Cpu_Core_Offset_Silent(1);
						}
					}
					break;
				}
				case 1u:
					if (SmartBalanceSupport)
					{
						string text3 = "0";
						try
						{
							text3 = ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\MySetting"), "SmartBalanceSwitch", 0));
						}
						catch
						{
						}
						if (text3 == "1")
						{
							UserSetSmartBalance(1);
						}
						else
						{
							UserSetSmartBalance(0);
						}
					}
					else
					{
						ClearSmartBalanceEnable();
					}
					break;
				}
			}
			AdjustTGPOnModeChange();
			if (isInit)
			{
				Init();
			}
			else
			{
				Init(GpuTesting: false, isManual: false);
			}
			UpdateStatusToClient(1);
			object obj3 = ResolvedDelegates.System_String_Concat__29;
			string text5 = "performance mode is: ";
			num = m_Option.OperatingMode;
			LogCtrl.TraceMessageProgramData(obj3(text5, ResolvedDelegates.System_UInt32_ToString__78(ref num)), "SwitchModePowerChange", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 434);
		}
		catch
		{
		}
		SetHighPerformanceModeForUMA();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetDispatcher(Dispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}

	public override int GetTurboModeSupportFlag()
	{
		return g_TurboModeSupport;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void EnableByService(Dispatcher dispatcher = null)
	{
		SetDispatcher(dispatcher);
		GetFanMode();
		g_TurboModeSupport = EcCtrl.GetTurboModeSupport();
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("g_TurboModeSupport: ", ResolvedDelegates.System_Int32_ToString__55(ref g_TurboModeSupport)), "EnableByService", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 493);
		if (CustomizeCtrl.GetCustomId() == 42)
		{
			g_TurboModeSupport = 0;
		}
		m_DefaultMode = GetDefaultMode();
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("m_DefaultMode: ", ResolvedDelegates.System_UInt32_ToString__78(ref m_DefaultMode)), "EnableByService", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 498);
		g_QkeyDefine = GetQkeyDefine();
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("g_QkeyDefine: ", ResolvedDelegates.System_UInt32_ToString__78(ref g_QkeyDefine)), "EnableByService", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 501);
		m_FanBoostBtnSupport = CustomizeCtrl.GetFanBoostBtnSupport();
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("m_FanBoostBtnSupport: ", ResolvedDelegates.System_Boolean_ToString__147(ref m_FanBoostBtnSupport)), "EnableByService", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 504);
		m_IsNvGpu = UtilityExtensions.IsNvGpu();
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("m_IsNvGpu: ", ResolvedDelegates.System_Boolean_ToString__147(ref m_IsNvGpu)), "EnableByService", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 510);
		m_IsAMDPlatform = CustomizeCtrl.GetIsAMDPlatform();
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("m_IsAMDPlatform: ", ResolvedDelegates.System_Boolean_ToString__147(ref m_IsAMDPlatform)), "EnableByService", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 513);
		int num = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, m_sRegPath, "firstlaunchdefault", -1);
		if (num < 0)
		{
			RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, m_sRegPath, "firstlaunchdefault", 1, RegistryValueKind.DWord);
			num = 1;
		}
		isFirstLaunch = num == 1;
		LoadProfileAll();
		syncBiosSettings();
		if (NvramVariableInfo.APVersionCheck < 24)
		{
			Init();
		}
		else
		{
			SwitchModePowerChange(isInit: true);
		}
		UserSet_FanBoost(m_Option.FanBoostEnable);
		m_bInit = true;
		if (isFirstLaunch)
		{
			RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, m_sRegPath, "firstlaunchdefault", 0, RegistryValueKind.DWord);
			isFirstLaunch = false;
		}
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "PerformanceReady", 1, RegistryValueKind.DWord);
		ScanCodeBus.OnScanCode += WMIHandleEvent;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void Disable()
	{
		FanTable.DisableByService();
		m_DefaultMode = GetDefaultMode();
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("m_DefaultMode: ", ResolvedDelegates.System_UInt32_ToString__78(ref m_DefaultMode)), "Disable", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 569);
		if (NvramVariableInfo.APVersionCheck < 24)
		{
			SetFanMode(m_DefaultMode);
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2024, 0);
		}
		if (!m_OcSupport)
		{
			return;
		}
		SetCpuTccOffset(0, bApExist: false);
		SetFanSwitchSpeedEnabled(0);
		if (m_IsNvGpu)
		{
			SetGpuDynamicBoostSwitch(0);
			if (m_DefaultMode == 0)
			{
				SetGpuWhisperModeSwitchOnOFF(1);
			}
			else
			{
				SetGpuWhisperModeSwitchOnOFF(0);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void Uninstall()
	{
		FanTable.Uninstall();
		SetDefaultMode();
		if (m_OcSupport)
		{
			SetCpuTccOffset(0, bApExist: false);
			SetFanSwitchSpeedEnabled(0);
			if (m_IsNvGpu)
			{
				SetGpuDynamicBoostSwitch(0);
				if (m_DefaultMode == 0)
				{
					SetGpuWhisperModeSwitchOnOFF(1);
				}
				else
				{
					SetGpuWhisperModeSwitchOnOFF(0);
				}
			}
		}
		if (NvramVariableInfo.APVersionCheck < 24)
		{
			NvramVariable.SetFwVarsForUninstall("PowerMode", ResolvedDelegates.System_Convert_ToByte__716(m_DefaultMode));
		}
		NvramVariable.SetFwVarsForUninstall("ApUseFlag", 0);
		NvramVariable.SetFwVarsForUninstall("MemoryOverClockSwitch", 0);
		NvramVariable.SetFwVarsForUninstall("ApExistFlag", 0);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__107("Sync up BIOS Variable, ApUseFlag: 0, PowerMode: ", ResolvedDelegates.System_UInt32_ToString__78(ref m_DefaultMode), ", MemoryOverClockSwitch: 0, ApExistFlag: 0"), "Uninstall", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 647);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetDefaultMode()
	{
		m_DefaultMode = GetDefaultMode();
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("m_DefaultMode: ", ResolvedDelegates.System_UInt32_ToString__78(ref m_DefaultMode)), "SetDefaultMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 657);
		SetFanMode(m_DefaultMode);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2024, 0);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<Receive>d__101))]
	public override void Receive(byte[] data)
	{
		<Receive>d__101 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine.data = data;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void UpdateStatusToClient(int autosave = 1, int cpuusage = 0, int cputemp = 0, bool updateOperationMode = true)
	{
		if (NvramVariableInfo.APVersionCheck < 24 || EcCtrl.GetNvGpu() == 0)
		{
			isAc = true;
		}
		var data = new
		{
			IsAC = isAc,
			OverClockingSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.OverClockingEnabled),
			OperatingMode = (updateOperationMode ? Convert.ToString(m_Option.OperatingMode) : null),
			GamingProfileIndex = Convert.ToString(m_Option.GamingProfileIndex),
			OfficeProfileIndex = ResolvedDelegates.System_Convert_ToString__763(m_Option.OfficeProfileIndex),
			TurboProfileIndex = ResolvedDelegates.System_Convert_ToString__763(m_Option.TurboProfileIndex),
			CustomProfileIndex = ResolvedDelegates.System_Convert_ToString__763(m_Option.CustomProfileIndex),
			FanBoostEnable = ResolvedDelegates.System_Convert_ToString__763(m_Option.FanBoostEnable),
			ProfileName = currentProfile.Name,
			FAN_FanSwitchSpeedEnabled = ResolvedDelegates.System_Convert_ToString__453(currentProfile.FAN.FanSwitchSpeedEnabled),
			FAN_FanSwitchSpeed = ResolvedDelegates.System_Convert_ToString__453(currentProfile.FAN.FanSwitchSpeed),
			FAN_TableName = currentProfile.FAN.TableName,
			FAN_SafetyProtectNotify = ResolvedDelegates.System_Convert_ToString__763(m_Option.FanSafetyProtectNotify),
			FAN_SafetyProtect = ResolvedDelegates.System_Convert_ToString__453(m_FanSafetyProtect),
			CPU_PL1 = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.PL1),
			CPU_PL1Maximum = ResolvedDelegates.System_Convert_ToString__764(_SmartApcTable.PL1),
			CPU_PL1Minimum = ResolvedDelegates.System_Convert_ToString__453(CpuPL1Minimum),
			CPU_PL2 = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.PL2),
			CPU_PL2Maximum = ResolvedDelegates.System_Convert_ToString__764(_SmartApcTable.PL2),
			CPU_PL2Minimum = ResolvedDelegates.System_Convert_ToString__453(CpuPL2Minimum),
			CPU_PL4 = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.PL4),
			CPU_PL4Maximum = ResolvedDelegates.System_Convert_ToString__764(_SmartApcTable.PL4),
			CPU_PL4Minimum = ResolvedDelegates.System_Convert_ToString__453(CpuPL4Minimum),
			CPU_OffsetCoreVoltage = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.OffsetCoreVoltage),
			CPU_OffsetCoreVoltageOC = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.OffsetCoreVoltageOC),
			CPU_OffsetCoreVoltageMaximum = ResolvedDelegates.System_Convert_ToString__453(CpuOffsetCoreVoltageMaximum),
			CPU_OffsetCoreVoltageMinimum = ResolvedDelegates.System_Convert_ToString__453(CpuOffsetCoreVoltageMinimum),
			CPU_OffsetCoreVoltageSupport = ResolvedDelegates.System_Convert_ToString__764(NvramVariableInfo.ICpuCoreVoltageOffsetSupport),
			CPU_TccOffsetSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.TccOffsetSwitch),
			CPU_TccOffsetMaximum = ResolvedDelegates.System_Convert_ToString__764(_SmartApcTable.TccOffset),
			CPU_TccOffset = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.TccOffset),
			GPU_CoreClockOffset = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.CoreClockOffset),
			GPU_CoreClockOffsetOC = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.CoreClockOffsetOC),
			GPU_CoreClockOffsetMaximum = ResolvedDelegates.System_Convert_ToString__453(GpuCoreClockOffsetMaximum),
			GPU_CoreClockOffsetMaximumHWOC = ResolvedDelegates.System_Convert_ToString__453(GpuCoreClockOffsetMaximumHWOC),
			GPU_CoreClockOffsetMinimum = ResolvedDelegates.System_Convert_ToString__453(0),
			GPU_MemoryClockOffset = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.MemoryClockOffset),
			GPU_MemoryClockOffsetOC = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.MemoryClockOffsetOC),
			GPU_MemoryClockOffsetMaximum = ResolvedDelegates.System_Convert_ToString__453(GpuMemoryClockOffsetMaximum),
			GPU_MemoryClockOffsetMaximumHWOC = ResolvedDelegates.System_Convert_ToString__453(GpuMemoryClockOffsetMaximumHWOC),
			GPU_MemoryClockOffsetMinimum = ResolvedDelegates.System_Convert_ToString__453(GpuMemoryClockOffsetMinimum),
			GPU_MemoryClockOffsetMinimumHWOC = ResolvedDelegates.System_Convert_ToString__453(GpuMemoryClockOffsetMinimumHWOC),
			GPU_TargetTemperature = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.TargetTemperature),
			GPU_TargetTemperatureMaximum = ResolvedDelegates.System_Convert_ToString__453(GpuThermalLimitInfo.MaximumTemperature),
			GPU_TargetTemperatureMinimum = ResolvedDelegates.System_Convert_ToString__453(GpuThermalLimitInfo.MinimumTemperature),
			GPU_ConfigurableTGPSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.ConfigurableTGPSwitch),
			GPU_ConfigurableTGPTarget = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.ConfigurableTGPTarget),
			GPU_ConfigurableTGPMaximum = ResolvedDelegates.System_Convert_ToString__453(m_GpuConfigurableTGPMaximum),
			GPU_ConfigurableTGPMinimum = ResolvedDelegates.System_Convert_ToString__764(_SmartApcTable.DefaultTGP),
			GPU_DynamicBoostSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.DynamicBoostSwitch),
			CustomTGPinGCUforGN21_Value = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.CustomTGPinGCUforGN21_Value),
			CustomTGPinGCUforGN21_Enable = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.CustomTGPinGCUforGN21_Enable),
			CPU_PerformanceAndOverClockMenuSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.PerformanceAndOverClockMenuSwitch),
			GPU_DynamicBoost = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.DynamicBoost),
			GPU_DynamicBoostMaximum = ResolvedDelegates.System_Convert_ToString__764(_SmartApcTable.DynamicBoost),
			GPU_DynamicBoostMinimum = ResolvedDelegates.System_Convert_ToString__453(5),
			GPU_DynamicBoostTotalProcessingPowerTarget = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.DynamicBoostTotalProcessingPowerTarget),
			GPU_WhisperModeSupport = m_GpuWhisperModeSupport,
			GPU_WhisperModeSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeSwitch),
			GPU_WhisperModeSetting = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeSetting),
			GPU_WhisperModeMinFps_QUIETER = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeMinFps_QUIETER),
			GPU_WhisperModeMinFps_QUIET = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeMinFps_QUIET),
			GPU_WhisperModeMinFps_BALANCED = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeMinFps_BALANCED),
			GPU_WhisperModeMinFpsMaximum = ResolvedDelegates.System_Convert_ToString__453(WhisperModeMinFpsMaximum),
			GPU_WhisperModeMinFpsMinimum = ResolvedDelegates.System_Convert_ToString__453(30),
			CPU_AmdSPL = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdSPL),
			CPU_AmdSPPT = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdSPPT),
			CPU_AmdFPPT = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdFPPT),
			CPU_AmdTccTarget = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdTccTarget),
			CPU_AmdOverClockSupport = ResolvedDelegates.System_Convert_ToString__764(NvramVariableInfo.ACpuOverClockSupport),
			CPU_AmdCoreFreq = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdCoreFreq),
			CPU_AmdCoreFreqOC = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdCoreFreqOC),
			CPU_AmdCoreFreqMaximum = ResolvedDelegates.System_Convert_ToString__763(NvramVariableInfo.ACpuFreqValueMaximum),
			CPU_AmdCoreVoltage = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdCoreVoltage),
			CPU_AmdCoreVoltageOC = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdCoreVoltageOC),
			CPU_AmdCoreVoltageMaximum = ResolvedDelegates.System_Convert_ToString__763(NvramVariableInfo.ACpuVoltageValueMaximum),
			MEM_MemoryOverClockSupport = ResolvedDelegates.System_Convert_ToString__764(NvramVariableInfo.MemoryOverClockSupport),
			MEM_MemoryOverClockSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.MEM.MemoryOverClockSwitch),
			BSOD_DefaultNotify = ResolvedDelegates.System_Convert_ToString__763(m_Option.DefaultNotify),
			BSOD_TimestampRestored = ResolvedDelegates.System_DateTime_ToString__274(ref m_Option.BsodTimestampRestored, "yyyy-MM-dd H:mm:ss"),
			OcSupport = m_OcSupport,
			IsAMDPlatform = m_IsAMDPlatform,
			IsNvGpu = m_IsNvGpu,
			TurboModeOption = ResolvedDelegates.System_Convert_ToString__453(g_TurboModeSupport),
			FanBoostBtnSupport = m_FanBoostBtnSupport,
			PowerMode = ResolvedDelegates.System_Convert_ToString__453(PowerModeEvent.g_ACLineStatus),
			CPU_PL4_Double_Flag = ResolvedDelegates.System_Convert_ToString__453(g_SupportCPUDoubleFlag),
			TjMax = ResolvedDelegates.System_Convert_ToString__764(_tjMax),
			Extreme = ResolvedDelegates.System_Convert_ToString__453(currentProfile.Extreme),
			Silent = ResolvedDelegates.System_Convert_ToString__453(currentProfile.Silent),
			SmartBalance = ResolvedDelegates.System_Convert_ToString__453(currentProfile.SmartBalance),
			TurboGPUOCEnabled = ((m_IsTurboSubModeSupport && m_IsTurboOCAdjustSupport > 0 && ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "SilentPerformanceModeSwitch", 0)), "0")) ? (m_Option.TurboSilentGPUOCOffset > 0) : (m_Option.TurboGPUOCOffset > 0)),
			GameWhitelistSwitch = gameWhitelistSwitch
		};
		App.m_MQTTService.Publish("Fan/Status", data, retain: false);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UpdateWhisperModeStatusToClient()
	{
		var data = new
		{
			GPU_WhisperModeSupport = m_GpuWhisperModeSupport,
			GPU_WhisperModeSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeSwitch),
			GPU_WhisperModeSetting = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeSetting),
			GPU_WhisperModeMinFps_QUIETER = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeMinFps_QUIETER),
			GPU_WhisperModeMinFps_QUIET = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeMinFps_QUIET),
			GPU_WhisperModeMinFps_BALANCED = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeMinFps_BALANCED),
			GPU_WhisperModeMinFpsMaximum = ResolvedDelegates.System_Convert_ToString__453(WhisperModeMinFpsMaximum),
			GPU_WhisperModeMinFpsMinimum = ResolvedDelegates.System_Convert_ToString__453(30)
		};
		App.m_MQTTService.Publish("WhisperMode/Status", data, retain: false);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void UpdateStatusToTray()
	{
		if (NvramVariableInfo.APVersionCheck < 24 || EcCtrl.GetNvGpu() == 0)
		{
			isAc = true;
		}
		var anon = new
		{
			IsAC = isAc,
			OperatingMode = ResolvedDelegates.System_Convert_ToString__763(m_Option.OperatingMode),
			FanBoostEnable = ResolvedDelegates.System_Convert_ToString__763(m_Option.FanBoostEnable)
		};
		App.m_MQTTService.Publish("Tray/Status", anon, retain: false);
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("#######################Topic.Tray_Status : ", ResolvedDelegates.System_Object_ToString__1(anon)));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void Resume()
	{
		LoadProfileAll();
		SwitchModePowerChange();
		UserSet_FanBoost(m_Option.FanBoostEnable);
	}

	public override void OnModernStandby()
	{
	}

	public override void PowerStatusChange(int mode)
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void Restore()
	{
		LoadDefault();
		Init(GpuTesting: true, isManual: false);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void ModeSwitchChanged()
	{
		object obj = ResolvedDelegates.System_String_Concat__107;
		string text = "Mode: ";
		uint operatingMode = m_Option.OperatingMode;
		LogCtrl.TraceMessage(obj(text, ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode), " OSD changed to the next mode "), "ModeSwitchChanged", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1164);
		if (EcCtrl.GetNvGpu() == 1)
		{
			switch (m_Option.OperatingMode)
			{
			case 1u:
				if (NvramVariableInfo.APVersionCheck >= 24 && EcCtrl.GetRoundSocketStatusFromEC() != 1)
				{
					SetModeSwitchChangeThread(0);
					_OPperatingMode = 0;
				}
				else
				{
					SetModeSwitchChangeThread(2);
					_OPperatingMode = 2;
				}
				break;
			case 0u:
				SetModeSwitchChangeThread(1);
				_OPperatingMode = 1;
				break;
			case 2u:
				if (CustomizeCtrl.GetCustomId() == 39 || CustomizeCtrl.GetCustomId() == 43)
				{
					SetModeSwitchChangeThread(0);
				}
				else
				{
					SetModeSwitchChangeThread(3);
				}
				break;
			case 3u:
				SetModeSwitchChangeThread(0);
				_OPperatingMode = 0;
				break;
			}
		}
		else
		{
			switch (m_Option.OperatingMode)
			{
			case 1u:
				if (GetTurboModeSupportFlag() == 0)
				{
					SetModeSwitchChangeThread(0);
				}
				else
				{
					SetModeSwitchChangeThread(2);
				}
				break;
			case 0u:
				SetModeSwitchChangeThread(1);
				break;
			case 2u:
				SetModeSwitchChangeThread(0);
				break;
			}
		}
		string text2 = "OSD Current Mode: ";
		operatingMode = m_Option.OperatingMode;
		LogCtrl.TraceMessage(text2 + ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode), "ModeSwitchChanged", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1221);
		if (CustomizeCtrl.GetIsUMATurbo() && m_Option.OperatingMode == 1)
		{
			MySettingManager.CreateInstance.SetHighPerformancePowerModeScancode(2);
		}
		else
		{
			MySettingManager.CreateInstance.SetHighPerformancePowerModeScancode((int)m_Option.OperatingMode);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void FanBoostUpdate()
	{
		App.m_Osd.SetOsdWay(167);
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, ref Data);
		if (((byte)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xC0) & 0x40) == 64)
		{
			m_Option.FanBoostEnable = 1u;
		}
		else
		{
			m_Option.FanBoostEnable = 0u;
		}
		LogCtrl.TraceMessage("performance mode is: " + ResolvedDelegates.System_Byte_ToString__119(ref Data), "FanBoostUpdate", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1246);
		SetFanMode(m_Option.OperatingMode);
		WriteMainOptionToJson("FanBoostEnable", m_Option.FanBoostEnable);
		UpdateStatusToClient(1);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void FanBoostOffFromEC()
	{
		m_Option.FanBoostEnable = 0u;
		SetFanMode(m_Option.OperatingMode);
		WriteMainOptionToJson("FanBoostEnable", m_Option.FanBoostEnable);
		UpdateStatusToClient(1);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void SafetyProtectionUpdate()
	{
		m_FanSafetyProtect = GetFanSafetyProtectStatus();
		if (m_FanSafetyProtect == 1)
		{
			m_Option.FanSafetyProtectNotify = 1u;
			if (m_Option.OperatingMode == 1)
			{
				FanTable.SetFanTable("DefaultFanTable_Gaming");
			}
			else if (m_Option.OperatingMode == 0)
			{
				FanTable.SetFanTable("DefaultFanTable_Office");
			}
			else if (m_Option.OperatingMode == 2)
			{
				FanTable.SetFanTable("DefaultFanTable_Turbo");
			}
			else if (m_Option.OperatingMode == 3)
			{
				FanTable.SetFanTable("DefaultFanTable_Gaming");
			}
		}
		else
		{
			m_Option.FanSafetyProtectNotify = 0u;
			FanTable.SetFanTable(currentProfile.FAN.TableName);
		}
		WriteMainOptionToJson("FanSafetyProtectNotify", m_Option.FanSafetyProtectNotify);
		UpdateStatusToClient(1);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void WhisperUpdate()
	{
		if (m_GpuWhisperModeSupport)
		{
			UpdateWhisperModeStatusToClient();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UpdateWhisperStatus()
	{
		int wisperModeStatus = cGpuFeatures.GetWisperModeStatus();
		if (wisperModeStatus == -1)
		{
			currentProfile.GPU.WhisperModeSwitch = 0;
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__120("GetWisperModeStatus: ", ResolvedDelegates.System_Int32_ToString__55(ref wisperModeStatus), ", WhisperModeSwitch: ", ResolvedDelegates.System_Int32_ToString__55(ref currentProfile.GPU.WhisperModeSwitch)), "UpdateWhisperStatus", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1315);
			WriteMainOptionToJson("WhisperModeSwitch", ResolvedDelegates.System_Convert_ToUInt32__143(currentProfile.GPU.WhisperModeSwitch));
			return;
		}
		currentProfile.GPU.WhisperModeSwitch = 1;
		currentProfile.GPU.WhisperModeSetting = wisperModeStatus;
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__79(new string[6]
		{
			"GetWisperModeStatus: ",
			ResolvedDelegates.System_Int32_ToString__55(ref wisperModeStatus),
			", WhisperModeSwitch: ",
			ResolvedDelegates.System_Int32_ToString__55(ref currentProfile.GPU.WhisperModeSwitch),
			", WhisperModeSetting: ",
			ResolvedDelegates.System_Int32_ToString__55(ref currentProfile.GPU.WhisperModeSetting)
		}), "UpdateWhisperStatus", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1324);
		WriteMainOptionToJson("WhisperModeSwitch", ResolvedDelegates.System_Convert_ToUInt32__143(currentProfile.GPU.WhisperModeSwitch));
		WriteMainOptionToJson("WhisperModeSetting", ResolvedDelegates.System_Convert_ToUInt32__143(currentProfile.GPU.WhisperModeSetting));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void UserSet_Mode1()
	{
		if (m_Option.OperatingMode != 0 && m_Option.OperatingMode != 1 && m_Option.OperatingMode != 2)
		{
			cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(0);
			cGpuFeatures.SetGpuDynamicBoostEnable(0);
			ResolvedDelegates.System_Threading_Thread_Sleep__6(50);
			cGpuFeatures.SetGpuDynamicBoostFunCtrlEnable(0);
		}
		cGpuFeatures.SetGpuConfigurableTGPTarget(95, _SmartApcTable.DefaultTGP);
		if (m_nAppType == 3)
		{
			m_Option.OperatingMode = 2u;
			RefreshCurrentProfile(m_Option.OperatingMode, save: false);
			SetUserProfile(2u);
			WriteMainOptionToJson("OperatingMode", 2u);
			NvramVariable.SetFwVarsForModeSwitchChange("PowerMode", ResolvedDelegates.System_Convert_ToByte__716(2u));
		}
		else
		{
			isAc = NvramVariableInfo.APVersionCheck < 24 || EcCtrl.GetRoundSocketStatusFromEC() == 1;
			m_Option.OperatingMode = 1u;
			RefreshCurrentProfile(m_Option.OperatingMode, save: false);
			SetUserProfile(1u);
			if (NvramVariableInfo.APVersionCheck >= 24 && EcCtrl.GetRoundSocketStatusFromEC() != 1)
			{
				AdjustTGPOnModeChange();
			}
			WriteMainOptionToJson("OperatingMode", 1u);
			NvramVariable.SetFwVarsForModeSwitchChange("PowerMode", ResolvedDelegates.System_Convert_ToByte__716(1u));
		}
		cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(0);
		object obj = ResolvedDelegates.System_String_Concat__29;
		string text = "performance mode is  : ";
		uint operatingMode = m_Option.OperatingMode;
		LogCtrl.TraceMessageProgramData(obj(text, ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode)), "UserSet_Mode1", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1391);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void UserSet_Mode2()
	{
		if (m_Option.OperatingMode != 0 && m_Option.OperatingMode != 1 && m_Option.OperatingMode != 2)
		{
			cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(0);
			cGpuFeatures.SetGpuDynamicBoostEnable(0);
			ResolvedDelegates.System_Threading_Thread_Sleep__6(50);
			cGpuFeatures.SetGpuDynamicBoostFunCtrlEnable(0);
		}
		cGpuFeatures.SetGpuConfigurableTGPTarget(95, _SmartApcTable.DefaultTGP);
		isAc = NvramVariableInfo.APVersionCheck < 24 || EcCtrl.GetRoundSocketStatusFromEC() == 1;
		m_Option.OperatingMode = 0u;
		RefreshCurrentProfile(m_Option.OperatingMode, save: false);
		SetUserProfile(0u);
		WriteMainOptionToJson("OperatingMode", 0u);
		NvramVariable.SetFwVarsForModeSwitchChange("PowerMode", ResolvedDelegates.System_Convert_ToByte__716(0u));
		cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(0);
		object obj = ResolvedDelegates.System_String_Concat__29;
		string text = "performance mode is: ";
		uint operatingMode = m_Option.OperatingMode;
		LogCtrl.TraceMessageProgramData(obj(text, ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode)), "UserSet_Mode2", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1434);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void UserSet_Mode3()
	{
		if (m_Option.OperatingMode != 0 && m_Option.OperatingMode != 1 && m_Option.OperatingMode != 2)
		{
			cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(0);
			cGpuFeatures.SetGpuDynamicBoostEnable(0);
			ResolvedDelegates.System_Threading_Thread_Sleep__6(50);
			cGpuFeatures.SetGpuDynamicBoostFunCtrlEnable(0);
		}
		cGpuFeatures.SetGpuConfigurableTGPTarget(115, _SmartApcTable.DefaultTGP);
		isAc = NvramVariableInfo.APVersionCheck < 24 || EcCtrl.GetRoundSocketStatusFromEC() == 1;
		m_Option.OperatingMode = 2u;
		RefreshCurrentProfile(m_Option.OperatingMode, save: false);
		currentProfile.GPU.CoreClockOffset = 0;
		SetUserProfile(2u);
		WriteMainOptionToJson("OperatingMode", 2u);
		NvramVariable.SetFwVarsForModeSwitchChange("PowerMode", ResolvedDelegates.System_Convert_ToByte__716(2u));
		object obj = ResolvedDelegates.System_String_Concat__29;
		string text = "performance mode is: ";
		uint operatingMode = m_Option.OperatingMode;
		LogCtrl.TraceMessageProgramData(obj(text, ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode)), "UserSet_Mode3", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1472);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void UserSet_Mode4(bool fromscancode = false)
	{
		cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(1);
		cGpuFeatures.SetGpuDynamicBoostFunCtrlEnable(1);
		cGpuFeatures.SetGpuDynamicBoostEnable(1);
		isAc = NvramVariableInfo.APVersionCheck < 24 || EcCtrl.GetRoundSocketStatusFromEC() == 1;
		m_Option.OperatingMode = 3u;
		if (fromscancode)
		{
			m_Option.CustomProfileIndex = (m_Option.CustomProfileIndex + 1) % 5;
			RefreshCurrentProfile(m_Option.OperatingMode, save: false);
		}
		else
		{
			RefreshCurrentProfile(m_Option.OperatingMode, save: false);
		}
		SetUserProfile(3u);
		WriteMainOptionToJson("OperatingMode", 3u);
		NvramVariable.SetFwVarsForModeSwitchChange("PowerMode", ResolvedDelegates.System_Convert_ToByte__716(3u));
		object obj = ResolvedDelegates.System_String_Concat__29;
		string text = "performance mode is: ";
		uint operatingMode = m_Option.OperatingMode;
		LogCtrl.TraceMessageProgramData(obj(text, ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode)), "UserSet_Mode4", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1511);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void SetPairedProfileIndex(uint index)
	{
		if (index == 0)
		{
			RefreshCurrentProfile(m_Option.OperatingMode, save: false);
			SetUserProfile(m_Option.OperatingMode);
			UpdateStatusToClient(1);
			return;
		}
		m_Option.GamingProfileIndex = index;
		WriteMainOptionToJson("GamingProfileIndex", m_Option.GamingProfileIndex);
		RefreshCurrentProfile(1u, save: false);
		SetUserProfile(1u);
		var data = new
		{
			OverClockingSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.OverClockingEnabled),
			OperatingMode = ResolvedDelegates.System_Convert_ToString__763(1u),
			GamingProfileIndex = ResolvedDelegates.System_Convert_ToString__763(m_Option.GamingProfileIndex),
			OfficeProfileIndex = ResolvedDelegates.System_Convert_ToString__763(m_Option.OfficeProfileIndex),
			TurboProfileIndex = ResolvedDelegates.System_Convert_ToString__763(m_Option.TurboProfileIndex),
			CustomProfileIndex = ResolvedDelegates.System_Convert_ToString__763(m_Option.CustomProfileIndex),
			FanBoostEnable = ResolvedDelegates.System_Convert_ToString__763(m_Option.FanBoostEnable),
			ProfileName = currentProfile.Name,
			FAN_FanSwitchSpeed = ResolvedDelegates.System_Convert_ToString__453(currentProfile.FAN.FanSwitchSpeed),
			FAN_TableName = currentProfile.FAN.TableName,
			CPU_PL1 = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.PL1),
			CPU_PL2 = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.PL2),
			CPU_PL4 = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.PL4),
			CPU_OffsetCoreVoltage = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.OffsetCoreVoltage),
			CPU_OffsetCoreVoltageOC = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.OffsetCoreVoltageOC),
			CPU_TccOffsetSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.TccOffsetSwitch),
			CPU_TccOffset = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.TccOffset),
			CPU_PerformanceAndOverClockMenuSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.PerformanceAndOverClockMenuSwitch),
			GPU_CoreClockOffset = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.CoreClockOffset),
			GPU_CoreClockOffsetOC = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.CoreClockOffsetOC),
			GPU_MemoryClockOffset = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.MemoryClockOffset),
			GPU_MemoryClockOffsetOC = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.MemoryClockOffsetOC),
			GPU_TargetTemperature = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.TargetTemperature),
			GPU_ConfigurableTGPSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.ConfigurableTGPSwitch),
			GPU_ConfigurableTGPTarget = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.ConfigurableTGPTarget),
			GPU_DynamicBoostSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.DynamicBoostSwitch),
			CustomTGPinGCUforGN21_Value = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.CustomTGPinGCUforGN21_Value),
			CustomTGPinGCUforGN21_Enable = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.CustomTGPinGCUforGN21_Enable),
			GPU_DynamicBoost = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.DynamicBoost),
			GPU_WhisperModeSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.GPU.WhisperModeSwitch),
			CPU_AmdSPL = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdSPL),
			CPU_AmdSPPT = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdSPPT),
			CPU_AmdFPPT = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdFPPT),
			CPU_AmdTccTarget = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdTccTarget),
			CPU_AmdCoreFreq = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdCoreFreq),
			CPU_AmdCoreFreqOC = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdCoreFreqOC),
			CPU_AmdCoreVoltage = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdCoreVoltage),
			CPU_AmdCoreVoltageOC = ResolvedDelegates.System_Convert_ToString__453(currentProfile.CPU.AmdCoreVoltageOC),
			MEM_MemoryOverClockSwitch = ResolvedDelegates.System_Convert_ToString__453(currentProfile.MEM.MemoryOverClockSwitch),
			CPU_PL4_Double_Flag = ResolvedDelegates.System_Convert_ToString__453(g_SupportCPUDoubleFlag),
			TjMax = ResolvedDelegates.System_Convert_ToString__764(_tjMax),
			SmartBalance = currentProfile.SmartBalance
		};
		App.m_MQTTService.Publish("Fan/Status", data, retain: false);
	}

	private void UserSet_Default()
	{
		Restore();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UserSet_FanBoost(uint status)
	{
		m_Option.FanBoostEnable = status;
		SetFanMode(m_Option.OperatingMode);
		WriteMainOptionToJson("FanBoostEnable", status);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UserSet_CoolingMode(int enable)
	{
		if (enable == 1)
		{
			byte pL1Value = 120;
			byte pL2Value = 120;
			byte pL4Value = 200;
			byte gPUdstate = 1;
			SetPL1Value(pL1Value);
			SetPL2Value(pL2Value);
			SetPL4Value(pL4Value);
			SetGPUdstate(gPUdstate);
			cGpuFeatures.SetGpuConfigurableTGPTarget(200, 0);
		}
		else if (PowerModeEvent.g_ACLineStatus == 1)
		{
			SetPL1Value(currentProfile.CPU.PL1);
			SetPL2Value(currentProfile.CPU.PL2);
			SetPL4Value(currentProfile.CPU.PL4);
			SetGPUdstate(0u);
			cGpuFeatures.SetGpuConfigurableTGPTarget(currentProfile.GPU.ConfigurableTGPTarget, _SmartApcTable.DefaultTGP);
		}
		else
		{
			SetPL1Value(0);
			SetPL2Value(0);
			SetPL4Value(0);
			SetGPUdstate(0u);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UserSet_Cpu_Core_Offset_Detail(dynamic data)
	{
		try
		{
			if (<>o__123.<>p__4 == null)
			{
				<>o__123.<>p__4 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target = <>o__123.<>p__4.Target;
			CallSite<Func<CallSite, object, bool>> arg = <>o__123.<>p__4;
			if (<>o__123.<>p__1 == null)
			{
				<>o__123.<>p__1 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target2 = <>o__123.<>p__1.Target;
			CallSite<Func<CallSite, object, object, object>> arg2 = <>o__123.<>p__1;
			if (<>o__123.<>p__0 == null)
			{
				<>o__123.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			object obj = target2(arg2, <>o__123.<>p__0.Target(<>o__123.<>p__0, (object)data, "OverClockingSwitch"), null);
			if (<>o__123.<>p__3 == null)
			{
				<>o__123.<>p__3 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object arg3;
			if (!<>o__123.<>p__3.Target(<>o__123.<>p__3, obj))
			{
				if (<>o__123.<>p__2 == null)
				{
					<>o__123.<>p__2 = CallSite<Func<CallSite, object, bool, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
					}));
				}
				arg3 = <>o__123.<>p__2.Target(<>o__123.<>p__2, obj, m_Option.OperatingMode == 3);
			}
			else
			{
				arg3 = obj;
			}
			if (target(arg, arg3))
			{
				ModeProfile modeProfile = currentProfile;
				if (<>o__123.<>p__7 == null)
				{
					<>o__123.<>p__7 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, int> target3 = <>o__123.<>p__7.Target;
				CallSite<Func<CallSite, object, int>> arg4 = <>o__123.<>p__7;
				if (<>o__123.<>p__6 == null)
				{
					<>o__123.<>p__6 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target4 = <>o__123.<>p__6.Target;
				CallSite<Func<CallSite, Type, object, object>> arg5 = <>o__123.<>p__6;
				Type arg6 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__123.<>p__5 == null)
				{
					<>o__123.<>p__5 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				modeProfile.OverClockingEnabled = target3(arg4, target4(arg5, arg6, <>o__123.<>p__5.Target(<>o__123.<>p__5, (object)data, "OverClockingSwitch")));
				LoadOverClockingValueSimp(currentProfile.OverClockingEnabled);
				UpdateStatusToClient(1);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception", ResolvedDelegates.System_Exception_get_Message__58(ex)), "UserSet_Cpu_Core_Offset_Detail", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1667);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UserSet_Cpu_Core_Offset_Extreme(dynamic Extreme)
	{
		try
		{
			byte Data = 0;
			EcCtrl.Read(m_className, 1832, ref Data);
			Data = SetbitValue(Data, 1, flag: false);
			EcCtrl.Write(m_className, 1832, Data);
			object obj = ResolvedDelegates.System_String_Concat__120;
			string text = "UserSet_Cpu_Core_Offset_Extreme========ADDR_AP_OEM_BYTEB ";
			ushort num = 1832;
			LogCtrl.TraceMessage(obj(text, ResolvedDelegates.System_UInt16_ToString__169(ref num), "bit0 \uff1a", ResolvedDelegates.System_Byte_ToString__119(ref Data)), "UserSet_Cpu_Core_Offset_Extreme", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1680);
			ModeProfile modeProfile = currentProfile;
			if (<>o__124.<>p__2 == null)
			{
				<>o__124.<>p__2 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
			}
			Func<CallSite, object, int> target = <>o__124.<>p__2.Target;
			CallSite<Func<CallSite, object, int>> arg = <>o__124.<>p__2;
			if (<>o__124.<>p__1 == null)
			{
				<>o__124.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target2 = <>o__124.<>p__1.Target;
			CallSite<Func<CallSite, Type, object, object>> arg2 = <>o__124.<>p__1;
			Type arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__124.<>p__0 == null)
			{
				<>o__124.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			modeProfile.Extreme = target(arg, target2(arg2, arg3, <>o__124.<>p__0.Target(<>o__124.<>p__0, (object)Extreme)));
			currentProfile.Silent = 0;
			WMIEC.Instance.WMIWriteExtremeSubModeForBiosRAM();
			UpdateStatusToClient(1, 0, 0, updateOperationMode: false);
			WriteProfileToJson(currentProfile);
			RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "SilentPerformanceModeSwitch", 1, RegistryValueKind.DWord);
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception", ResolvedDelegates.System_Exception_get_Message__58(ex)), "UserSet_Cpu_Core_Offset_Extreme", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1692);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UserSet_Cpu_Core_Offset_Silent(dynamic Silent)
	{
		try
		{
			currentProfile.Extreme = 0;
			ModeProfile modeProfile = currentProfile;
			if (<>o__125.<>p__2 == null)
			{
				<>o__125.<>p__2 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
			}
			Func<CallSite, object, int> target = <>o__125.<>p__2.Target;
			CallSite<Func<CallSite, object, int>> arg = <>o__125.<>p__2;
			if (<>o__125.<>p__1 == null)
			{
				<>o__125.<>p__1 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			Func<CallSite, Type, object, object> target2 = <>o__125.<>p__1.Target;
			CallSite<Func<CallSite, Type, object, object>> arg2 = <>o__125.<>p__1;
			Type arg3 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
			if (<>o__125.<>p__0 == null)
			{
				<>o__125.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			modeProfile.Silent = target(arg, target2(arg2, arg3, <>o__125.<>p__0.Target(<>o__125.<>p__0, (object)Silent)));
			byte Data = 0;
			EcCtrl.Read(m_className, 1832, ref Data);
			Data = SetbitValue(Data, 1, flag: true);
			EcCtrl.Write(m_className, 1832, Data);
			object obj = ResolvedDelegates.System_String_Concat__120;
			string text = "UserSet_Cpu_Core_Offset_Silent========ADDR_AP_OEM_BYTEB ";
			ushort num = 1832;
			LogCtrl.TraceMessage(obj(text, ResolvedDelegates.System_UInt16_ToString__169(ref num), "bit0 \uff1a", ResolvedDelegates.System_Byte_ToString__119(ref Data)), "UserSet_Cpu_Core_Offset_Silent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1706);
			WMIEC.Instance.WMIWriteSilentSubModeForBiosRAM();
			UpdateStatusToClient(1, 0, 0, updateOperationMode: false);
			WriteProfileToJson(currentProfile);
			RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "SilentPerformanceModeSwitch", 0, RegistryValueKind.DWord);
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception", ResolvedDelegates.System_Exception_get_Message__58(ex)), "UserSet_Cpu_Core_Offset_Silent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1716);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static byte SetbitValue(byte data, int index, bool flag)
	{
		int num;
		switch (index)
		{
		default:
			throw new ArgumentOutOfRangeException();
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
			num = 2 << index - 2;
			break;
		case 1:
			num = index;
			break;
		}
		int num2 = num;
		if (!flag)
		{
			return (byte)(data & ~num2);
		}
		return (byte)(data | num2);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UserSetSmartBalance(int enable)
	{
		currentProfile.SmartBalance = enable;
		EnableOrDisableSmartBalance(enable);
		UpdateStatusToClient(1);
		WriteProfileToJson(currentProfile);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void EnableOrDisableSmartBalance(int enable)
	{
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\MySetting"), "SmartBalanceSwitch", enable, RegistryValueKind.DWord);
		if (enable == 1)
		{
			byte Data = 0;
			EcCtrl.Read(m_className, 1964, ref Data);
			Data |= 2;
			EcCtrl.Write(m_className, 1964, Data);
		}
		else
		{
			byte Data2 = 0;
			EcCtrl.Read(m_className, 1964, ref Data2);
			Data2 &= 0xFD;
			EcCtrl.Write(m_className, 1964, Data2);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ClearSmartBalanceEnable()
	{
		byte Data = 0;
		EcCtrl.Read(m_className, 1964, ref Data);
		Data &= 0xFD;
		EcCtrl.Write(m_className, 1964, Data);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UpdateSmartBalanceSupport()
	{
		int smartBalanceSupportFromEC = EcCtrl.GetSmartBalanceSupportFromEC();
		SmartBalanceSupport = smartBalanceSupportFromEC == 1;
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "IsSmartBalanceSupport", smartBalanceSupportFromEC, RegistryValueKind.DWord);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UserSet_Mode_Detail(dynamic data)
	{
		try
		{
			uint fanMode = GetFanMode();
			if (<>o__131.<>p__4 == null)
			{
				<>o__131.<>p__4 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target = <>o__131.<>p__4.Target;
			CallSite<Func<CallSite, object, bool>> arg = <>o__131.<>p__4;
			if (<>o__131.<>p__1 == null)
			{
				<>o__131.<>p__1 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target2 = <>o__131.<>p__1.Target;
			CallSite<Func<CallSite, object, object, object>> arg2 = <>o__131.<>p__1;
			if (<>o__131.<>p__0 == null)
			{
				<>o__131.<>p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			object obj = target2(arg2, <>o__131.<>p__0.Target(<>o__131.<>p__0, (object)data, "OverClockingSwitch"), null);
			if (<>o__131.<>p__3 == null)
			{
				<>o__131.<>p__3 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			object arg3;
			if (!<>o__131.<>p__3.Target(<>o__131.<>p__3, obj))
			{
				if (<>o__131.<>p__2 == null)
				{
					<>o__131.<>p__2 = CallSite<Func<CallSite, object, bool, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
					}));
				}
				arg3 = <>o__131.<>p__2.Target(<>o__131.<>p__2, obj, m_Option.OperatingMode == 3);
			}
			else
			{
				arg3 = obj;
			}
			if (target(arg, arg3))
			{
				ModeProfile modeProfile = currentProfile;
				if (<>o__131.<>p__7 == null)
				{
					<>o__131.<>p__7 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, int> target3 = <>o__131.<>p__7.Target;
				CallSite<Func<CallSite, object, int>> arg4 = <>o__131.<>p__7;
				if (<>o__131.<>p__6 == null)
				{
					<>o__131.<>p__6 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target4 = <>o__131.<>p__6.Target;
				CallSite<Func<CallSite, Type, object, object>> arg5 = <>o__131.<>p__6;
				Type arg6 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__131.<>p__5 == null)
				{
					<>o__131.<>p__5 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				modeProfile.OverClockingEnabled = target3(arg4, target4(arg5, arg6, <>o__131.<>p__5.Target(<>o__131.<>p__5, (object)data, "OverClockingSwitch")));
				LoadOverClockingValue(currentProfile.OverClockingEnabled);
				UpdateStatusToClient(1);
			}
			if (<>o__131.<>p__10 == null)
			{
				<>o__131.<>p__10 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target5 = <>o__131.<>p__10.Target;
			CallSite<Func<CallSite, object, bool>> arg7 = <>o__131.<>p__10;
			if (<>o__131.<>p__9 == null)
			{
				<>o__131.<>p__9 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target6 = <>o__131.<>p__9.Target;
			CallSite<Func<CallSite, object, object, object>> arg8 = <>o__131.<>p__9;
			if (<>o__131.<>p__8 == null)
			{
				<>o__131.<>p__8 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (target5(arg7, target6(arg8, <>o__131.<>p__8.Target(<>o__131.<>p__8, (object)data, "PL1"), null)))
			{
				CPU cPU = currentProfile.CPU;
				if (<>o__131.<>p__13 == null)
				{
					<>o__131.<>p__13 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, int> target7 = <>o__131.<>p__13.Target;
				CallSite<Func<CallSite, object, int>> arg9 = <>o__131.<>p__13;
				if (<>o__131.<>p__12 == null)
				{
					<>o__131.<>p__12 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target8 = <>o__131.<>p__12.Target;
				CallSite<Func<CallSite, Type, object, object>> arg10 = <>o__131.<>p__12;
				Type arg11 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__131.<>p__11 == null)
				{
					<>o__131.<>p__11 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				cPU.PL1 = target7(arg9, target8(arg10, arg11, <>o__131.<>p__11.Target(<>o__131.<>p__11, (object)data, "PL1")));
				ReCheckCpuPLMinimumValueI();
			}
			if (<>o__131.<>p__16 == null)
			{
				<>o__131.<>p__16 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target9 = <>o__131.<>p__16.Target;
			CallSite<Func<CallSite, object, bool>> arg12 = <>o__131.<>p__16;
			if (<>o__131.<>p__15 == null)
			{
				<>o__131.<>p__15 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target10 = <>o__131.<>p__15.Target;
			CallSite<Func<CallSite, object, object, object>> arg13 = <>o__131.<>p__15;
			if (<>o__131.<>p__14 == null)
			{
				<>o__131.<>p__14 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (target9(arg12, target10(arg13, <>o__131.<>p__14.Target(<>o__131.<>p__14, (object)data, "PL2"), null)))
			{
				CPU cPU2 = currentProfile.CPU;
				if (<>o__131.<>p__19 == null)
				{
					<>o__131.<>p__19 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, int> target11 = <>o__131.<>p__19.Target;
				CallSite<Func<CallSite, object, int>> arg14 = <>o__131.<>p__19;
				if (<>o__131.<>p__18 == null)
				{
					<>o__131.<>p__18 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target12 = <>o__131.<>p__18.Target;
				CallSite<Func<CallSite, Type, object, object>> arg15 = <>o__131.<>p__18;
				Type arg16 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__131.<>p__17 == null)
				{
					<>o__131.<>p__17 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				cPU2.PL2 = target11(arg14, target12(arg15, arg16, <>o__131.<>p__17.Target(<>o__131.<>p__17, (object)data, "PL2")));
				ReCheckCpuPLMinimumValueI();
			}
			if (<>o__131.<>p__22 == null)
			{
				<>o__131.<>p__22 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target13 = <>o__131.<>p__22.Target;
			CallSite<Func<CallSite, object, bool>> arg17 = <>o__131.<>p__22;
			if (<>o__131.<>p__21 == null)
			{
				<>o__131.<>p__21 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target14 = <>o__131.<>p__21.Target;
			CallSite<Func<CallSite, object, object, object>> arg18 = <>o__131.<>p__21;
			if (<>o__131.<>p__20 == null)
			{
				<>o__131.<>p__20 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (target13(arg17, target14(arg18, <>o__131.<>p__20.Target(<>o__131.<>p__20, (object)data, "PL4"), null)))
			{
				CPU cPU3 = currentProfile.CPU;
				if (<>o__131.<>p__25 == null)
				{
					<>o__131.<>p__25 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, int> target15 = <>o__131.<>p__25.Target;
				CallSite<Func<CallSite, object, int>> arg19 = <>o__131.<>p__25;
				if (<>o__131.<>p__24 == null)
				{
					<>o__131.<>p__24 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target16 = <>o__131.<>p__24.Target;
				CallSite<Func<CallSite, Type, object, object>> arg20 = <>o__131.<>p__24;
				Type arg21 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__131.<>p__23 == null)
				{
					<>o__131.<>p__23 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				cPU3.PL4 = target15(arg19, target16(arg20, arg21, <>o__131.<>p__23.Target(<>o__131.<>p__23, (object)data, "PL4")));
				ReCheckCpuPLMinimumValueI();
			}
			if (<>o__131.<>p__28 == null)
			{
				<>o__131.<>p__28 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target17 = <>o__131.<>p__28.Target;
			CallSite<Func<CallSite, object, bool>> arg22 = <>o__131.<>p__28;
			if (<>o__131.<>p__27 == null)
			{
				<>o__131.<>p__27 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target18 = <>o__131.<>p__27.Target;
			CallSite<Func<CallSite, object, object, object>> arg23 = <>o__131.<>p__27;
			if (<>o__131.<>p__26 == null)
			{
				<>o__131.<>p__26 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (target17(arg22, target18(arg23, <>o__131.<>p__26.Target(<>o__131.<>p__26, (object)data, "CpuAmdSPL"), null)))
			{
				CPU cPU4 = currentProfile.CPU;
				if (<>o__131.<>p__31 == null)
				{
					<>o__131.<>p__31 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, int> target19 = <>o__131.<>p__31.Target;
				CallSite<Func<CallSite, object, int>> arg24 = <>o__131.<>p__31;
				if (<>o__131.<>p__30 == null)
				{
					<>o__131.<>p__30 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target20 = <>o__131.<>p__30.Target;
				CallSite<Func<CallSite, Type, object, object>> arg25 = <>o__131.<>p__30;
				Type arg26 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__131.<>p__29 == null)
				{
					<>o__131.<>p__29 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				cPU4.AmdSPL = target19(arg24, target20(arg25, arg26, <>o__131.<>p__29.Target(<>o__131.<>p__29, (object)data, "CpuAmdSPL")));
				ReCheckCpuPLMinimumValueA();
				if (PowerModeEvent.g_ACLineStatus == 1 && currentProfile.CPU.AmdSPL >= 75)
				{
					SetCpuVrmCurrentLimit(65, 120);
				}
			}
			if (<>o__131.<>p__34 == null)
			{
				<>o__131.<>p__34 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target21 = <>o__131.<>p__34.Target;
			CallSite<Func<CallSite, object, bool>> arg27 = <>o__131.<>p__34;
			if (<>o__131.<>p__33 == null)
			{
				<>o__131.<>p__33 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target22 = <>o__131.<>p__33.Target;
			CallSite<Func<CallSite, object, object, object>> arg28 = <>o__131.<>p__33;
			if (<>o__131.<>p__32 == null)
			{
				<>o__131.<>p__32 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (target21(arg27, target22(arg28, <>o__131.<>p__32.Target(<>o__131.<>p__32, (object)data, "CpuAmdSPPT"), null)))
			{
				CPU cPU5 = currentProfile.CPU;
				if (<>o__131.<>p__37 == null)
				{
					<>o__131.<>p__37 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, int> target23 = <>o__131.<>p__37.Target;
				CallSite<Func<CallSite, object, int>> arg29 = <>o__131.<>p__37;
				if (<>o__131.<>p__36 == null)
				{
					<>o__131.<>p__36 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target24 = <>o__131.<>p__36.Target;
				CallSite<Func<CallSite, Type, object, object>> arg30 = <>o__131.<>p__36;
				Type arg31 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__131.<>p__35 == null)
				{
					<>o__131.<>p__35 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				cPU5.AmdSPPT = target23(arg29, target24(arg30, arg31, <>o__131.<>p__35.Target(<>o__131.<>p__35, (object)data, "CpuAmdSPPT")));
				ReCheckCpuPLMinimumValueA();
				if (PowerModeEvent.g_ACLineStatus == 1 && currentProfile.CPU.AmdSPPT >= 75)
				{
					SetCpuVrmCurrentLimit(65, 120);
				}
			}
			if (<>o__131.<>p__40 == null)
			{
				<>o__131.<>p__40 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target25 = <>o__131.<>p__40.Target;
			CallSite<Func<CallSite, object, bool>> arg32 = <>o__131.<>p__40;
			if (<>o__131.<>p__39 == null)
			{
				<>o__131.<>p__39 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target26 = <>o__131.<>p__39.Target;
			CallSite<Func<CallSite, object, object, object>> arg33 = <>o__131.<>p__39;
			if (<>o__131.<>p__38 == null)
			{
				<>o__131.<>p__38 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (target25(arg32, target26(arg33, <>o__131.<>p__38.Target(<>o__131.<>p__38, (object)data, "CpuAmdFPPT"), null)))
			{
				CPU cPU6 = currentProfile.CPU;
				if (<>o__131.<>p__43 == null)
				{
					<>o__131.<>p__43 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, int> target27 = <>o__131.<>p__43.Target;
				CallSite<Func<CallSite, object, int>> arg34 = <>o__131.<>p__43;
				if (<>o__131.<>p__42 == null)
				{
					<>o__131.<>p__42 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target28 = <>o__131.<>p__42.Target;
				CallSite<Func<CallSite, Type, object, object>> arg35 = <>o__131.<>p__42;
				Type arg36 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__131.<>p__41 == null)
				{
					<>o__131.<>p__41 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				cPU6.AmdFPPT = target27(arg34, target28(arg35, arg36, <>o__131.<>p__41.Target(<>o__131.<>p__41, (object)data, "CpuAmdFPPT")));
				ReCheckCpuPLMinimumValueA();
			}
			if (<>o__131.<>p__46 == null)
			{
				<>o__131.<>p__46 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			Func<CallSite, object, bool> target29 = <>o__131.<>p__46.Target;
			CallSite<Func<CallSite, object, bool>> arg37 = <>o__131.<>p__46;
			if (<>o__131.<>p__45 == null)
			{
				<>o__131.<>p__45 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, object, object> target30 = <>o__131.<>p__45.Target;
			CallSite<Func<CallSite, object, object, object>> arg38 = <>o__131.<>p__45;
			if (<>o__131.<>p__44 == null)
			{
				<>o__131.<>p__44 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			if (target29(arg37, target30(arg38, <>o__131.<>p__44.Target(<>o__131.<>p__44, (object)data, "FanSwitchSpeed"), null)))
			{
				FanSettings fAN = currentProfile.FAN;
				if (<>o__131.<>p__49 == null)
				{
					<>o__131.<>p__49 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, int> target31 = <>o__131.<>p__49.Target;
				CallSite<Func<CallSite, object, int>> arg39 = <>o__131.<>p__49;
				if (<>o__131.<>p__48 == null)
				{
					<>o__131.<>p__48 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					}));
				}
				Func<CallSite, Type, object, object> target32 = <>o__131.<>p__48.Target;
				CallSite<Func<CallSite, Type, object, object>> arg40 = <>o__131.<>p__48;
				Type arg41 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
				if (<>o__131.<>p__47 == null)
				{
					<>o__131.<>p__47 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				fAN.FanSwitchSpeed = target31(arg39, target32(arg40, arg41, <>o__131.<>p__47.Target(<>o__131.<>p__47, (object)data, "FanSwitchSpeed")));
				SetFanSwitchSpeed(currentProfile.FAN.FanSwitchSpeed, bApExist: true);
			}
			else
			{
				if (<>o__131.<>p__52 == null)
				{
					<>o__131.<>p__52 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				Func<CallSite, object, bool> target33 = <>o__131.<>p__52.Target;
				CallSite<Func<CallSite, object, bool>> arg42 = <>o__131.<>p__52;
				if (<>o__131.<>p__51 == null)
				{
					<>o__131.<>p__51 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				Func<CallSite, object, object, object> target34 = <>o__131.<>p__51.Target;
				CallSite<Func<CallSite, object, object, object>> arg43 = <>o__131.<>p__51;
				if (<>o__131.<>p__50 == null)
				{
					<>o__131.<>p__50 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				if (target33(arg42, target34(arg43, <>o__131.<>p__50.Target(<>o__131.<>p__50, (object)data, "FanSwitchSpeedEnabled"), null)))
				{
					FanSettings fAN2 = currentProfile.FAN;
					if (<>o__131.<>p__55 == null)
					{
						<>o__131.<>p__55 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
					}
					Func<CallSite, object, int> target35 = <>o__131.<>p__55.Target;
					CallSite<Func<CallSite, object, int>> arg44 = <>o__131.<>p__55;
					if (<>o__131.<>p__54 == null)
					{
						<>o__131.<>p__54 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
						}));
					}
					Func<CallSite, Type, object, object> target36 = <>o__131.<>p__54.Target;
					CallSite<Func<CallSite, Type, object, object>> arg45 = <>o__131.<>p__54;
					Type arg46 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
					if (<>o__131.<>p__53 == null)
					{
						<>o__131.<>p__53 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
						}));
					}
					fAN2.FanSwitchSpeedEnabled = target35(arg44, target36(arg45, arg46, <>o__131.<>p__53.Target(<>o__131.<>p__53, (object)data, "FanSwitchSpeedEnabled")));
					SetFanSwitchSpeedEnabled(currentProfile.FAN.FanSwitchSpeedEnabled);
				}
				else
				{
					if (<>o__131.<>p__58 == null)
					{
						<>o__131.<>p__58 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					Func<CallSite, object, bool> target37 = <>o__131.<>p__58.Target;
					CallSite<Func<CallSite, object, bool>> arg47 = <>o__131.<>p__58;
					if (<>o__131.<>p__57 == null)
					{
						<>o__131.<>p__57 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
						}));
					}
					Func<CallSite, object, object, object> target38 = <>o__131.<>p__57.Target;
					CallSite<Func<CallSite, object, object, object>> arg48 = <>o__131.<>p__57;
					if (<>o__131.<>p__56 == null)
					{
						<>o__131.<>p__56 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
						}));
					}
					if (target37(arg47, target38(arg48, <>o__131.<>p__56.Target(<>o__131.<>p__56, (object)data, "CpuOffsetCoreVoltage"), null)))
					{
						CPU cPU7 = currentProfile.CPU;
						if (<>o__131.<>p__61 == null)
						{
							<>o__131.<>p__61 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
						}
						Func<CallSite, object, int> target39 = <>o__131.<>p__61.Target;
						CallSite<Func<CallSite, object, int>> arg49 = <>o__131.<>p__61;
						if (<>o__131.<>p__60 == null)
						{
							<>o__131.<>p__60 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						Func<CallSite, Type, object, object> target40 = <>o__131.<>p__60.Target;
						CallSite<Func<CallSite, Type, object, object>> arg50 = <>o__131.<>p__60;
						Type arg51 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
						if (<>o__131.<>p__59 == null)
						{
							<>o__131.<>p__59 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						cPU7.OffsetCoreVoltage = target39(arg49, target40(arg50, arg51, <>o__131.<>p__59.Target(<>o__131.<>p__59, (object)data, "CpuOffsetCoreVoltage")));
						SetCpuCoreVoltageOffset(currentProfile.CPU.OffsetCoreVoltage);
					}
					else
					{
						if (<>o__131.<>p__64 == null)
						{
							<>o__131.<>p__64 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
						}
						Func<CallSite, object, bool> target41 = <>o__131.<>p__64.Target;
						CallSite<Func<CallSite, object, bool>> arg52 = <>o__131.<>p__64;
						if (<>o__131.<>p__63 == null)
						{
							<>o__131.<>p__63 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						Func<CallSite, object, object, object> target42 = <>o__131.<>p__63.Target;
						CallSite<Func<CallSite, object, object, object>> arg53 = <>o__131.<>p__63;
						if (<>o__131.<>p__62 == null)
						{
							<>o__131.<>p__62 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
							}));
						}
						if (target41(arg52, target42(arg53, <>o__131.<>p__62.Target(<>o__131.<>p__62, (object)data, "CpuOffsetCoreVoltageOC"), null)))
						{
							CPU cPU8 = currentProfile.CPU;
							if (<>o__131.<>p__67 == null)
							{
								<>o__131.<>p__67 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
							}
							Func<CallSite, object, int> target43 = <>o__131.<>p__67.Target;
							CallSite<Func<CallSite, object, int>> arg54 = <>o__131.<>p__67;
							if (<>o__131.<>p__66 == null)
							{
								<>o__131.<>p__66 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, Type, object, object> target44 = <>o__131.<>p__66.Target;
							CallSite<Func<CallSite, Type, object, object>> arg55 = <>o__131.<>p__66;
							Type arg56 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
							if (<>o__131.<>p__65 == null)
							{
								<>o__131.<>p__65 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							cPU8.OffsetCoreVoltageOC = target43(arg54, target44(arg55, arg56, <>o__131.<>p__65.Target(<>o__131.<>p__65, (object)data, "CpuOffsetCoreVoltageOC")));
							SetCpuCoreVoltageOffset(currentProfile.CPU.OffsetCoreVoltageOC);
						}
						else
						{
							if (<>o__131.<>p__70 == null)
							{
								<>o__131.<>p__70 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
							}
							Func<CallSite, object, bool> target45 = <>o__131.<>p__70.Target;
							CallSite<Func<CallSite, object, bool>> arg57 = <>o__131.<>p__70;
							if (<>o__131.<>p__69 == null)
							{
								<>o__131.<>p__69 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							Func<CallSite, object, object, object> target46 = <>o__131.<>p__69.Target;
							CallSite<Func<CallSite, object, object, object>> arg58 = <>o__131.<>p__69;
							if (<>o__131.<>p__68 == null)
							{
								<>o__131.<>p__68 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							if (target45(arg57, target46(arg58, <>o__131.<>p__68.Target(<>o__131.<>p__68, (object)data, "CpuTccOffsetSwitch"), null)))
							{
								CPU cPU9 = currentProfile.CPU;
								if (<>o__131.<>p__73 == null)
								{
									<>o__131.<>p__73 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
								}
								Func<CallSite, object, int> target47 = <>o__131.<>p__73.Target;
								CallSite<Func<CallSite, object, int>> arg59 = <>o__131.<>p__73;
								if (<>o__131.<>p__72 == null)
								{
									<>o__131.<>p__72 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
									}));
								}
								Func<CallSite, Type, object, object> target48 = <>o__131.<>p__72.Target;
								CallSite<Func<CallSite, Type, object, object>> arg60 = <>o__131.<>p__72;
								Type arg61 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
								if (<>o__131.<>p__71 == null)
								{
									<>o__131.<>p__71 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								cPU9.TccOffsetSwitch = target47(arg59, target48(arg60, arg61, <>o__131.<>p__71.Target(<>o__131.<>p__71, (object)data, "CpuTccOffsetSwitch")));
								if (currentProfile.CPU.TccOffsetSwitch == 1 && fanMode == 3)
								{
									if (m_IsAMDPlatform)
									{
										SetCpuTccOffset(currentProfile.CPU.AmdTccTarget, bApExist: true);
									}
									else
									{
										SetCpuTccOffset(currentProfile.CPU.TccOffset, bApExist: true);
									}
								}
								else if (currentProfile.CPU.TccOffsetSwitch == 0 && fanMode == 3)
								{
									SetCpuTccOffset(0, bApExist: false);
								}
							}
							else
							{
								if (<>o__131.<>p__76 == null)
								{
									<>o__131.<>p__76 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
								}
								Func<CallSite, object, bool> target49 = <>o__131.<>p__76.Target;
								CallSite<Func<CallSite, object, bool>> arg62 = <>o__131.<>p__76;
								if (<>o__131.<>p__75 == null)
								{
									<>o__131.<>p__75 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								Func<CallSite, object, object, object> target50 = <>o__131.<>p__75.Target;
								CallSite<Func<CallSite, object, object, object>> arg63 = <>o__131.<>p__75;
								if (<>o__131.<>p__74 == null)
								{
									<>o__131.<>p__74 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
									{
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
									}));
								}
								if (target49(arg62, target50(arg63, <>o__131.<>p__74.Target(<>o__131.<>p__74, (object)data, "CpuTccOffset"), null)))
								{
									CPU cPU10 = currentProfile.CPU;
									if (<>o__131.<>p__79 == null)
									{
										<>o__131.<>p__79 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
									}
									Func<CallSite, object, int> target51 = <>o__131.<>p__79.Target;
									CallSite<Func<CallSite, object, int>> arg64 = <>o__131.<>p__79;
									if (<>o__131.<>p__78 == null)
									{
										<>o__131.<>p__78 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									Func<CallSite, Type, object, object> target52 = <>o__131.<>p__78.Target;
									CallSite<Func<CallSite, Type, object, object>> arg65 = <>o__131.<>p__78;
									Type arg66 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
									if (<>o__131.<>p__77 == null)
									{
										<>o__131.<>p__77 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
										}));
									}
									cPU10.TccOffset = target51(arg64, target52(arg65, arg66, <>o__131.<>p__77.Target(<>o__131.<>p__77, (object)data, "CpuTccOffset")));
									if (fanMode == 3)
									{
										SetCpuTccOffset(currentProfile.CPU.TccOffset, bApExist: true);
									}
								}
								else
								{
									if (<>o__131.<>p__82 == null)
									{
										<>o__131.<>p__82 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
									}
									Func<CallSite, object, bool> target53 = <>o__131.<>p__82.Target;
									CallSite<Func<CallSite, object, bool>> arg67 = <>o__131.<>p__82;
									if (<>o__131.<>p__81 == null)
									{
										<>o__131.<>p__81 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
										}));
									}
									Func<CallSite, object, object, object> target54 = <>o__131.<>p__81.Target;
									CallSite<Func<CallSite, object, object, object>> arg68 = <>o__131.<>p__81;
									if (<>o__131.<>p__80 == null)
									{
										<>o__131.<>p__80 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
										}));
									}
									if (target53(arg67, target54(arg68, <>o__131.<>p__80.Target(<>o__131.<>p__80, (object)data, "GpuCoreClockOffset"), null)))
									{
										GPU gPU = currentProfile.GPU;
										if (<>o__131.<>p__85 == null)
										{
											<>o__131.<>p__85 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
										}
										Func<CallSite, object, int> target55 = <>o__131.<>p__85.Target;
										CallSite<Func<CallSite, object, int>> arg69 = <>o__131.<>p__85;
										if (<>o__131.<>p__84 == null)
										{
											<>o__131.<>p__84 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
											{
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
											}));
										}
										Func<CallSite, Type, object, object> target56 = <>o__131.<>p__84.Target;
										CallSite<Func<CallSite, Type, object, object>> arg70 = <>o__131.<>p__84;
										Type arg71 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
										if (<>o__131.<>p__83 == null)
										{
											<>o__131.<>p__83 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
											{
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
											}));
										}
										gPU.CoreClockOffset = target55(arg69, target56(arg70, arg71, <>o__131.<>p__83.Target(<>o__131.<>p__83, (object)data, "GpuCoreClockOffset")));
										cGpuFeatures.SetGpuCoreFreqOffsetValue(currentProfile.GPU.CoreClockOffset);
										if (m_Option.OperatingMode == 2 && NvramVariableInfo.APVersionCheck >= 24 && !IsUMA())
										{
											m_Option.TurboGPUOCOffset = currentProfile.GPU.CoreClockOffset;
										}
									}
									else
									{
										if (<>o__131.<>p__88 == null)
										{
											<>o__131.<>p__88 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
										}
										Func<CallSite, object, bool> target57 = <>o__131.<>p__88.Target;
										CallSite<Func<CallSite, object, bool>> arg72 = <>o__131.<>p__88;
										if (<>o__131.<>p__87 == null)
										{
											<>o__131.<>p__87 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
											{
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
											}));
										}
										Func<CallSite, object, object, object> target58 = <>o__131.<>p__87.Target;
										CallSite<Func<CallSite, object, object, object>> arg73 = <>o__131.<>p__87;
										if (<>o__131.<>p__86 == null)
										{
											<>o__131.<>p__86 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
											{
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
											}));
										}
										if (target57(arg72, target58(arg73, <>o__131.<>p__86.Target(<>o__131.<>p__86, (object)data, "GpuCoreClockOffsetOC"), null)))
										{
											if (<>o__131.<>p__91 == null)
											{
												<>o__131.<>p__91 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
											}
											Func<CallSite, object, int> target59 = <>o__131.<>p__91.Target;
											CallSite<Func<CallSite, object, int>> arg74 = <>o__131.<>p__91;
											if (<>o__131.<>p__90 == null)
											{
												<>o__131.<>p__90 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
												{
													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
												}));
											}
											Func<CallSite, Type, object, object> target60 = <>o__131.<>p__90.Target;
											CallSite<Func<CallSite, Type, object, object>> arg75 = <>o__131.<>p__90;
											Type arg76 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
											if (<>o__131.<>p__89 == null)
											{
												<>o__131.<>p__89 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
												{
													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
												}));
											}
											int num = target59(arg74, target60(arg75, arg76, <>o__131.<>p__89.Target(<>o__131.<>p__89, (object)data, "GpuCoreClockOffsetOC")));
											currentProfile.GPU.CoreClockOffsetOC = num;
											cGpuFeatures.SetGpuCoreFreqOffsetValue(currentProfile.GPU.CoreClockOffsetOC);
											if (m_Option.OperatingMode == 2 && !IsUMA())
											{
												if (m_IsTurboSubModeSupport && m_IsTurboOCAdjustSupport > 0 && ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "SilentPerformanceModeSwitch", 0)), "0"))
												{
													m_Option.TurboSilentGPUOCOffset = num;
													WriteMainOptionToJson("TurboSilentGPUOCOffset", (uint)num);
													LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("TurboSilentGPUOCOffset wrtire: ", ResolvedDelegates.System_Int32_ToString__55(ref m_Option.TurboGPUOCOffset)), "UserSet_Mode_Detail", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1931);
												}
												else
												{
													m_Option.TurboGPUOCOffset = num;
													WriteMainOptionToJson("TurboGPUOCOffset", (uint)num);
													LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("TurboGPUOCOffset wrtire: ", ResolvedDelegates.System_Int32_ToString__55(ref m_Option.TurboGPUOCOffset)), "UserSet_Mode_Detail", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1937);
												}
												RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2\\MySetting", "TURBO_GPU_OC", m_Option.TurboGPUOCOffset, RegistryValueKind.DWord);
												LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("TURBO_GPU_OC wrtire: ", ResolvedDelegates.System_Int32_ToString__55(ref m_Option.TurboGPUOCOffset)), "UserSet_Mode_Detail", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 1941);
											}
										}
										else
										{
											if (<>o__131.<>p__94 == null)
											{
												<>o__131.<>p__94 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
											}
											Func<CallSite, object, bool> target61 = <>o__131.<>p__94.Target;
											CallSite<Func<CallSite, object, bool>> arg77 = <>o__131.<>p__94;
											if (<>o__131.<>p__93 == null)
											{
												<>o__131.<>p__93 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
												{
													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
												}));
											}
											Func<CallSite, object, object, object> target62 = <>o__131.<>p__93.Target;
											CallSite<Func<CallSite, object, object, object>> arg78 = <>o__131.<>p__93;
											if (<>o__131.<>p__92 == null)
											{
												<>o__131.<>p__92 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
												{
													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
												}));
											}
											if (target61(arg77, target62(arg78, <>o__131.<>p__92.Target(<>o__131.<>p__92, (object)data, "GpuMemoryClockOffset"), null)))
											{
												GPU gPU2 = currentProfile.GPU;
												if (<>o__131.<>p__97 == null)
												{
													<>o__131.<>p__97 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
												}
												Func<CallSite, object, int> target63 = <>o__131.<>p__97.Target;
												CallSite<Func<CallSite, object, int>> arg79 = <>o__131.<>p__97;
												if (<>o__131.<>p__96 == null)
												{
													<>o__131.<>p__96 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
													{
														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
													}));
												}
												Func<CallSite, Type, object, object> target64 = <>o__131.<>p__96.Target;
												CallSite<Func<CallSite, Type, object, object>> arg80 = <>o__131.<>p__96;
												Type arg81 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
												if (<>o__131.<>p__95 == null)
												{
													<>o__131.<>p__95 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
													{
														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
													}));
												}
												gPU2.MemoryClockOffset = target63(arg79, target64(arg80, arg81, <>o__131.<>p__95.Target(<>o__131.<>p__95, (object)data, "GpuMemoryClockOffset")));
												cGpuFeatures.SetGpuMemFreqOffsetValue(currentProfile.GPU.MemoryClockOffset);
											}
											else
											{
												if (<>o__131.<>p__100 == null)
												{
													<>o__131.<>p__100 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
												}
												Func<CallSite, object, bool> target65 = <>o__131.<>p__100.Target;
												CallSite<Func<CallSite, object, bool>> arg82 = <>o__131.<>p__100;
												if (<>o__131.<>p__99 == null)
												{
													<>o__131.<>p__99 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
													{
														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
													}));
												}
												Func<CallSite, object, object, object> target66 = <>o__131.<>p__99.Target;
												CallSite<Func<CallSite, object, object, object>> arg83 = <>o__131.<>p__99;
												if (<>o__131.<>p__98 == null)
												{
													<>o__131.<>p__98 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
													{
														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
													}));
												}
												if (target65(arg82, target66(arg83, <>o__131.<>p__98.Target(<>o__131.<>p__98, (object)data, "GpuMemoryClockOffsetOC"), null)))
												{
													GPU gPU3 = currentProfile.GPU;
													if (<>o__131.<>p__103 == null)
													{
														<>o__131.<>p__103 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
													}
													Func<CallSite, object, int> target67 = <>o__131.<>p__103.Target;
													CallSite<Func<CallSite, object, int>> arg84 = <>o__131.<>p__103;
													if (<>o__131.<>p__102 == null)
													{
														<>o__131.<>p__102 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
														{
															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
														}));
													}
													Func<CallSite, Type, object, object> target68 = <>o__131.<>p__102.Target;
													CallSite<Func<CallSite, Type, object, object>> arg85 = <>o__131.<>p__102;
													Type arg86 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
													if (<>o__131.<>p__101 == null)
													{
														<>o__131.<>p__101 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
														{
															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
														}));
													}
													gPU3.MemoryClockOffsetOC = target67(arg84, target68(arg85, arg86, <>o__131.<>p__101.Target(<>o__131.<>p__101, (object)data, "GpuMemoryClockOffsetOC")));
													cGpuFeatures.SetGpuMemFreqOffsetValue(currentProfile.GPU.MemoryClockOffsetOC);
												}
												else
												{
													if (<>o__131.<>p__106 == null)
													{
														<>o__131.<>p__106 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
													}
													Func<CallSite, object, bool> target69 = <>o__131.<>p__106.Target;
													CallSite<Func<CallSite, object, bool>> arg87 = <>o__131.<>p__106;
													if (<>o__131.<>p__105 == null)
													{
														<>o__131.<>p__105 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
														{
															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
														}));
													}
													Func<CallSite, object, object, object> target70 = <>o__131.<>p__105.Target;
													CallSite<Func<CallSite, object, object, object>> arg88 = <>o__131.<>p__105;
													if (<>o__131.<>p__104 == null)
													{
														<>o__131.<>p__104 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
														{
															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
														}));
													}
													if (target69(arg87, target70(arg88, <>o__131.<>p__104.Target(<>o__131.<>p__104, (object)data, "GpuTargetTemperature"), null)))
													{
														GPU gPU4 = currentProfile.GPU;
														if (<>o__131.<>p__109 == null)
														{
															<>o__131.<>p__109 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
														}
														Func<CallSite, object, int> target71 = <>o__131.<>p__109.Target;
														CallSite<Func<CallSite, object, int>> arg89 = <>o__131.<>p__109;
														if (<>o__131.<>p__108 == null)
														{
															<>o__131.<>p__108 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
															{
																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
															}));
														}
														Func<CallSite, Type, object, object> target72 = <>o__131.<>p__108.Target;
														CallSite<Func<CallSite, Type, object, object>> arg90 = <>o__131.<>p__108;
														Type arg91 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
														if (<>o__131.<>p__107 == null)
														{
															<>o__131.<>p__107 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
															{
																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
															}));
														}
														gPU4.TargetTemperature = target71(arg89, target72(arg90, arg91, <>o__131.<>p__107.Target(<>o__131.<>p__107, (object)data, "GpuTargetTemperature")));
														cGpuFeatures.SetGpuTargetTemperature(currentProfile.GPU.TargetTemperature);
													}
													else
													{
														if (<>o__131.<>p__112 == null)
														{
															<>o__131.<>p__112 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
														}
														Func<CallSite, object, bool> target73 = <>o__131.<>p__112.Target;
														CallSite<Func<CallSite, object, bool>> arg92 = <>o__131.<>p__112;
														if (<>o__131.<>p__111 == null)
														{
															<>o__131.<>p__111 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
															{
																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
															}));
														}
														Func<CallSite, object, object, object> target74 = <>o__131.<>p__111.Target;
														CallSite<Func<CallSite, object, object, object>> arg93 = <>o__131.<>p__111;
														if (<>o__131.<>p__110 == null)
														{
															<>o__131.<>p__110 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
															{
																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
															}));
														}
														if (target73(arg92, target74(arg93, <>o__131.<>p__110.Target(<>o__131.<>p__110, (object)data, "GpuConfigurableTGPSwitch"), null)))
														{
															GPU gPU5 = currentProfile.GPU;
															if (<>o__131.<>p__115 == null)
															{
																<>o__131.<>p__115 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
															}
															Func<CallSite, object, int> target75 = <>o__131.<>p__115.Target;
															CallSite<Func<CallSite, object, int>> arg94 = <>o__131.<>p__115;
															if (<>o__131.<>p__114 == null)
															{
																<>o__131.<>p__114 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																{
																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																}));
															}
															Func<CallSite, Type, object, object> target76 = <>o__131.<>p__114.Target;
															CallSite<Func<CallSite, Type, object, object>> arg95 = <>o__131.<>p__114;
															Type arg96 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
															if (<>o__131.<>p__113 == null)
															{
																<>o__131.<>p__113 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																{
																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																}));
															}
															gPU5.ConfigurableTGPSwitch = target75(arg94, target76(arg95, arg96, <>o__131.<>p__113.Target(<>o__131.<>p__113, (object)data, "GpuConfigurableTGPSwitch")));
															cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(currentProfile.GPU.ConfigurableTGPSwitch);
														}
														else
														{
															if (<>o__131.<>p__118 == null)
															{
																<>o__131.<>p__118 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
															}
															Func<CallSite, object, bool> target77 = <>o__131.<>p__118.Target;
															CallSite<Func<CallSite, object, bool>> arg97 = <>o__131.<>p__118;
															if (<>o__131.<>p__117 == null)
															{
																<>o__131.<>p__117 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																{
																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																}));
															}
															Func<CallSite, object, object, object> target78 = <>o__131.<>p__117.Target;
															CallSite<Func<CallSite, object, object, object>> arg98 = <>o__131.<>p__117;
															if (<>o__131.<>p__116 == null)
															{
																<>o__131.<>p__116 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																{
																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																}));
															}
															if (target77(arg97, target78(arg98, <>o__131.<>p__116.Target(<>o__131.<>p__116, (object)data, "GpuConfigurableTGPTarget"), null)))
															{
																GPU gPU6 = currentProfile.GPU;
																if (<>o__131.<>p__121 == null)
																{
																	<>o__131.<>p__121 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																}
																Func<CallSite, object, int> target79 = <>o__131.<>p__121.Target;
																CallSite<Func<CallSite, object, int>> arg99 = <>o__131.<>p__121;
																if (<>o__131.<>p__120 == null)
																{
																	<>o__131.<>p__120 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																	{
																		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																	}));
																}
																Func<CallSite, Type, object, object> target80 = <>o__131.<>p__120.Target;
																CallSite<Func<CallSite, Type, object, object>> arg100 = <>o__131.<>p__120;
																Type arg101 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																if (<>o__131.<>p__119 == null)
																{
																	<>o__131.<>p__119 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																	{
																		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																	}));
																}
																gPU6.ConfigurableTGPTarget = target79(arg99, target80(arg100, arg101, <>o__131.<>p__119.Target(<>o__131.<>p__119, (object)data, "GpuConfigurableTGPTarget")));
																cGpuFeatures.SetGpuConfigurableTGPTarget(currentProfile.GPU.ConfigurableTGPTarget, _SmartApcTable.DefaultTGP);
															}
															else
															{
																if (<>o__131.<>p__124 == null)
																{
																	<>o__131.<>p__124 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																}
																Func<CallSite, object, bool> target81 = <>o__131.<>p__124.Target;
																CallSite<Func<CallSite, object, bool>> arg102 = <>o__131.<>p__124;
																if (<>o__131.<>p__123 == null)
																{
																	<>o__131.<>p__123 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																	{
																		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																	}));
																}
																Func<CallSite, object, object, object> target82 = <>o__131.<>p__123.Target;
																CallSite<Func<CallSite, object, object, object>> arg103 = <>o__131.<>p__123;
																if (<>o__131.<>p__122 == null)
																{
																	<>o__131.<>p__122 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																	{
																		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																	}));
																}
																if (target81(arg102, target82(arg103, <>o__131.<>p__122.Target(<>o__131.<>p__122, (object)data, "GpuDynamicBoostSwitch"), null)))
																{
																	GPU gPU7 = currentProfile.GPU;
																	if (<>o__131.<>p__127 == null)
																	{
																		<>o__131.<>p__127 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																	}
																	Func<CallSite, object, int> target83 = <>o__131.<>p__127.Target;
																	CallSite<Func<CallSite, object, int>> arg104 = <>o__131.<>p__127;
																	if (<>o__131.<>p__126 == null)
																	{
																		<>o__131.<>p__126 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																		{
																			CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																			CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																		}));
																	}
																	Func<CallSite, Type, object, object> target84 = <>o__131.<>p__126.Target;
																	CallSite<Func<CallSite, Type, object, object>> arg105 = <>o__131.<>p__126;
																	Type arg106 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																	if (<>o__131.<>p__125 == null)
																	{
																		<>o__131.<>p__125 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																		{
																			CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																			CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																		}));
																	}
																	gPU7.DynamicBoostSwitch = target83(arg104, target84(arg105, arg106, <>o__131.<>p__125.Target(<>o__131.<>p__125, (object)data, "GpuDynamicBoostSwitch")));
																	SetGpuDynamicBoostSwitch(currentProfile.GPU.DynamicBoostSwitch);
																}
																else
																{
																	if (<>o__131.<>p__130 == null)
																	{
																		<>o__131.<>p__130 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																	}
																	Func<CallSite, object, bool> target85 = <>o__131.<>p__130.Target;
																	CallSite<Func<CallSite, object, bool>> arg107 = <>o__131.<>p__130;
																	if (<>o__131.<>p__129 == null)
																	{
																		<>o__131.<>p__129 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																		{
																			CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																			CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																		}));
																	}
																	Func<CallSite, object, object, object> target86 = <>o__131.<>p__129.Target;
																	CallSite<Func<CallSite, object, object, object>> arg108 = <>o__131.<>p__129;
																	if (<>o__131.<>p__128 == null)
																	{
																		<>o__131.<>p__128 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																		{
																			CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																			CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																		}));
																	}
																	if (target85(arg107, target86(arg108, <>o__131.<>p__128.Target(<>o__131.<>p__128, (object)data, "GpuDynamicBoost"), null)))
																	{
																		GPU gPU8 = currentProfile.GPU;
																		if (<>o__131.<>p__133 == null)
																		{
																			<>o__131.<>p__133 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																		}
																		Func<CallSite, object, int> target87 = <>o__131.<>p__133.Target;
																		CallSite<Func<CallSite, object, int>> arg109 = <>o__131.<>p__133;
																		if (<>o__131.<>p__132 == null)
																		{
																			<>o__131.<>p__132 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																			{
																				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																			}));
																		}
																		Func<CallSite, Type, object, object> target88 = <>o__131.<>p__132.Target;
																		CallSite<Func<CallSite, Type, object, object>> arg110 = <>o__131.<>p__132;
																		Type arg111 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																		if (<>o__131.<>p__131 == null)
																		{
																			<>o__131.<>p__131 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																			{
																				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																			}));
																		}
																		gPU8.DynamicBoost = target87(arg109, target88(arg110, arg111, <>o__131.<>p__131.Target(<>o__131.<>p__131, (object)data, "GpuDynamicBoost")));
																		if (currentProfile.OverClockingEnabled == 0)
																		{
																			cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(currentProfile.GPU.DynamicBoost);
																		}
																		else if (currentProfile.OverClockingEnabled == 1)
																		{
																			cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(_SmartApcTable.DynamicBoost);
																		}
																	}
																	else
																	{
																		if (<>o__131.<>p__136 == null)
																		{
																			<>o__131.<>p__136 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																		}
																		Func<CallSite, object, bool> target89 = <>o__131.<>p__136.Target;
																		CallSite<Func<CallSite, object, bool>> arg112 = <>o__131.<>p__136;
																		if (<>o__131.<>p__135 == null)
																		{
																			<>o__131.<>p__135 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																			{
																				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																			}));
																		}
																		Func<CallSite, object, object, object> target90 = <>o__131.<>p__135.Target;
																		CallSite<Func<CallSite, object, object, object>> arg113 = <>o__131.<>p__135;
																		if (<>o__131.<>p__134 == null)
																		{
																			<>o__131.<>p__134 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																			{
																				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																			}));
																		}
																		if (target89(arg112, target90(arg113, <>o__131.<>p__134.Target(<>o__131.<>p__134, (object)data, "GpuDynamicBoostTotalProcessingPowerTarget"), null)))
																		{
																			GPU gPU9 = currentProfile.GPU;
																			if (<>o__131.<>p__139 == null)
																			{
																				<>o__131.<>p__139 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																			}
																			Func<CallSite, object, int> target91 = <>o__131.<>p__139.Target;
																			CallSite<Func<CallSite, object, int>> arg114 = <>o__131.<>p__139;
																			if (<>o__131.<>p__138 == null)
																			{
																				<>o__131.<>p__138 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																				{
																					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																				}));
																			}
																			Func<CallSite, Type, object, object> target92 = <>o__131.<>p__138.Target;
																			CallSite<Func<CallSite, Type, object, object>> arg115 = <>o__131.<>p__138;
																			Type arg116 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																			if (<>o__131.<>p__137 == null)
																			{
																				<>o__131.<>p__137 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																				{
																					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																				}));
																			}
																			gPU9.DynamicBoostTotalProcessingPowerTarget = target91(arg114, target92(arg115, arg116, <>o__131.<>p__137.Target(<>o__131.<>p__137, (object)data, "GpuDynamicBoostTotalProcessingPowerTarget")));
																			cGpuFeatures.SetGpuDynamicBoostTotalProcessingPowerTarget(currentProfile.GPU.DynamicBoostTotalProcessingPowerTarget);
																		}
																		else
																		{
																			if (<>o__131.<>p__142 == null)
																			{
																				<>o__131.<>p__142 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																			}
																			Func<CallSite, object, bool> target93 = <>o__131.<>p__142.Target;
																			CallSite<Func<CallSite, object, bool>> arg117 = <>o__131.<>p__142;
																			if (<>o__131.<>p__141 == null)
																			{
																				<>o__131.<>p__141 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																				{
																					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																				}));
																			}
																			Func<CallSite, object, object, object> target94 = <>o__131.<>p__141.Target;
																			CallSite<Func<CallSite, object, object, object>> arg118 = <>o__131.<>p__141;
																			if (<>o__131.<>p__140 == null)
																			{
																				<>o__131.<>p__140 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																				{
																					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																				}));
																			}
																			if (target93(arg117, target94(arg118, <>o__131.<>p__140.Target(<>o__131.<>p__140, (object)data, "WhisperModeSwitch"), null)))
																			{
																				if (m_GpuWhisperModeSupport)
																				{
																					GPU gPU10 = currentProfile.GPU;
																					if (<>o__131.<>p__145 == null)
																					{
																						<>o__131.<>p__145 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																					}
																					Func<CallSite, object, int> target95 = <>o__131.<>p__145.Target;
																					CallSite<Func<CallSite, object, int>> arg119 = <>o__131.<>p__145;
																					if (<>o__131.<>p__144 == null)
																					{
																						<>o__131.<>p__144 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																						{
																							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																						}));
																					}
																					Func<CallSite, Type, object, object> target96 = <>o__131.<>p__144.Target;
																					CallSite<Func<CallSite, Type, object, object>> arg120 = <>o__131.<>p__144;
																					Type arg121 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																					if (<>o__131.<>p__143 == null)
																					{
																						<>o__131.<>p__143 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																						{
																							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																						}));
																					}
																					gPU10.WhisperModeSwitch = target95(arg119, target96(arg120, arg121, <>o__131.<>p__143.Target(<>o__131.<>p__143, (object)data, "WhisperModeSwitch")));
																					WriteMainOptionToJson("WhisperModeSwitch", ResolvedDelegates.System_Convert_ToUInt32__143(currentProfile.GPU.WhisperModeSwitch));
																					SetGpuWhisperModeSwitchOnOFF(currentProfile.GPU.WhisperModeSwitch);
																				}
																			}
																			else
																			{
																				if (<>o__131.<>p__148 == null)
																				{
																					<>o__131.<>p__148 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																				}
																				Func<CallSite, object, bool> target97 = <>o__131.<>p__148.Target;
																				CallSite<Func<CallSite, object, bool>> arg122 = <>o__131.<>p__148;
																				if (<>o__131.<>p__147 == null)
																				{
																					<>o__131.<>p__147 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																					{
																						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																					}));
																				}
																				Func<CallSite, object, object, object> target98 = <>o__131.<>p__147.Target;
																				CallSite<Func<CallSite, object, object, object>> arg123 = <>o__131.<>p__147;
																				if (<>o__131.<>p__146 == null)
																				{
																					<>o__131.<>p__146 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																					{
																						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																					}));
																				}
																				if (target97(arg122, target98(arg123, <>o__131.<>p__146.Target(<>o__131.<>p__146, (object)data, "GpuWhisperModeSetting"), null)))
																				{
																					if (m_GpuWhisperModeSupport)
																					{
																						GPU gPU11 = currentProfile.GPU;
																						if (<>o__131.<>p__151 == null)
																						{
																							<>o__131.<>p__151 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																						}
																						Func<CallSite, object, int> target99 = <>o__131.<>p__151.Target;
																						CallSite<Func<CallSite, object, int>> arg124 = <>o__131.<>p__151;
																						if (<>o__131.<>p__150 == null)
																						{
																							<>o__131.<>p__150 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																							{
																								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																							}));
																						}
																						Func<CallSite, Type, object, object> target100 = <>o__131.<>p__150.Target;
																						CallSite<Func<CallSite, Type, object, object>> arg125 = <>o__131.<>p__150;
																						Type arg126 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																						if (<>o__131.<>p__149 == null)
																						{
																							<>o__131.<>p__149 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																							{
																								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																							}));
																						}
																						gPU11.WhisperModeSetting = target99(arg124, target100(arg125, arg126, <>o__131.<>p__149.Target(<>o__131.<>p__149, (object)data, "GpuWhisperModeSetting")));
																						WriteMainOptionToJson("WhisperModeSetting", ResolvedDelegates.System_Convert_ToUInt32__143(currentProfile.GPU.WhisperModeSetting));
																						cGpuFeatures.SetGpuWhisperModeSettingValue(currentProfile.GPU.WhisperModeSetting);
																						switch (currentProfile.GPU.WhisperModeSetting)
																						{
																						case 0:
																							cGpuFeatures.SetGpuWhisperModeMinFpsValue(currentProfile.GPU.WhisperModeMinFps_QUIETER);
																							break;
																						case 1:
																							cGpuFeatures.SetGpuWhisperModeMinFpsValue(currentProfile.GPU.WhisperModeMinFps_QUIET);
																							break;
																						case 2:
																							cGpuFeatures.SetGpuWhisperModeMinFpsValue(currentProfile.GPU.WhisperModeMinFps_BALANCED);
																							break;
																						}
																						UpdateStatusToClient(1);
																					}
																				}
																				else
																				{
																					if (<>o__131.<>p__154 == null)
																					{
																						<>o__131.<>p__154 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																					}
																					Func<CallSite, object, bool> target101 = <>o__131.<>p__154.Target;
																					CallSite<Func<CallSite, object, bool>> arg127 = <>o__131.<>p__154;
																					if (<>o__131.<>p__153 == null)
																					{
																						<>o__131.<>p__153 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																						{
																							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																						}));
																					}
																					Func<CallSite, object, object, object> target102 = <>o__131.<>p__153.Target;
																					CallSite<Func<CallSite, object, object, object>> arg128 = <>o__131.<>p__153;
																					if (<>o__131.<>p__152 == null)
																					{
																						<>o__131.<>p__152 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																						{
																							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																						}));
																					}
																					if (target101(arg127, target102(arg128, <>o__131.<>p__152.Target(<>o__131.<>p__152, (object)data, "GpuWhisperModeMinFps"), null)))
																					{
																						if (m_GpuWhisperModeSupport)
																						{
																							int num2 = 0;
																							if (<>o__131.<>p__157 == null)
																							{
																								<>o__131.<>p__157 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																							}
																							Func<CallSite, object, int> target103 = <>o__131.<>p__157.Target;
																							CallSite<Func<CallSite, object, int>> arg129 = <>o__131.<>p__157;
																							if (<>o__131.<>p__156 == null)
																							{
																								<>o__131.<>p__156 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																								{
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																								}));
																							}
																							Func<CallSite, Type, object, object> target104 = <>o__131.<>p__156.Target;
																							CallSite<Func<CallSite, Type, object, object>> arg130 = <>o__131.<>p__156;
																							Type arg131 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																							if (<>o__131.<>p__155 == null)
																							{
																								<>o__131.<>p__155 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																								{
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																								}));
																							}
																							num2 = target103(arg129, target104(arg130, arg131, <>o__131.<>p__155.Target(<>o__131.<>p__155, (object)data, "GpuWhisperModeMinFps")));
																							switch (currentProfile.GPU.WhisperModeSetting)
																							{
																							case 0:
																								currentProfile.GPU.WhisperModeMinFps_QUIETER = num2;
																								WriteMainOptionToJson("WhisperModeMinFps_QUIETER", ResolvedDelegates.System_Convert_ToUInt32__143(num2));
																								break;
																							case 1:
																								currentProfile.GPU.WhisperModeMinFps_QUIET = num2;
																								WriteMainOptionToJson("WhisperModeMinFps_QUIET", ResolvedDelegates.System_Convert_ToUInt32__143(num2));
																								break;
																							case 2:
																								currentProfile.GPU.WhisperModeMinFps_BALANCED = num2;
																								WriteMainOptionToJson("WhisperModeMinFps_BALANCED", ResolvedDelegates.System_Convert_ToUInt32__143(num2));
																								break;
																							}
																							cGpuFeatures.SetGpuWhisperModeMinFpsValue(num2);
																						}
																					}
																					else
																					{
																						if (<>o__131.<>p__160 == null)
																						{
																							<>o__131.<>p__160 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																						}
																						Func<CallSite, object, bool> target105 = <>o__131.<>p__160.Target;
																						CallSite<Func<CallSite, object, bool>> arg132 = <>o__131.<>p__160;
																						if (<>o__131.<>p__159 == null)
																						{
																							<>o__131.<>p__159 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																							{
																								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																							}));
																						}
																						Func<CallSite, object, object, object> target106 = <>o__131.<>p__159.Target;
																						CallSite<Func<CallSite, object, object, object>> arg133 = <>o__131.<>p__159;
																						if (<>o__131.<>p__158 == null)
																						{
																							<>o__131.<>p__158 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																							{
																								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																							}));
																						}
																						if (target105(arg132, target106(arg133, <>o__131.<>p__158.Target(<>o__131.<>p__158, (object)data, "CpuAmdTccTarget"), null)))
																						{
																							CPU cPU11 = currentProfile.CPU;
																							if (<>o__131.<>p__163 == null)
																							{
																								<>o__131.<>p__163 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																							}
																							Func<CallSite, object, int> target107 = <>o__131.<>p__163.Target;
																							CallSite<Func<CallSite, object, int>> arg134 = <>o__131.<>p__163;
																							if (<>o__131.<>p__162 == null)
																							{
																								<>o__131.<>p__162 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																								{
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																								}));
																							}
																							Func<CallSite, Type, object, object> target108 = <>o__131.<>p__162.Target;
																							CallSite<Func<CallSite, Type, object, object>> arg135 = <>o__131.<>p__162;
																							Type arg136 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																							if (<>o__131.<>p__161 == null)
																							{
																								<>o__131.<>p__161 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																								{
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																								}));
																							}
																							cPU11.AmdTccTarget = target107(arg134, target108(arg135, arg136, <>o__131.<>p__161.Target(<>o__131.<>p__161, (object)data, "CpuAmdTccTarget")));
																							if (fanMode == 3)
																							{
																								SetCpuTccOffset(currentProfile.CPU.AmdTccTarget, bApExist: true);
																							}
																						}
																						else
																						{
																							if (<>o__131.<>p__166 == null)
																							{
																								<>o__131.<>p__166 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																							}
																							Func<CallSite, object, bool> target109 = <>o__131.<>p__166.Target;
																							CallSite<Func<CallSite, object, bool>> arg137 = <>o__131.<>p__166;
																							if (<>o__131.<>p__165 == null)
																							{
																								<>o__131.<>p__165 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																								{
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																								}));
																							}
																							Func<CallSite, object, object, object> target110 = <>o__131.<>p__165.Target;
																							CallSite<Func<CallSite, object, object, object>> arg138 = <>o__131.<>p__165;
																							if (<>o__131.<>p__164 == null)
																							{
																								<>o__131.<>p__164 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																								{
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																								}));
																							}
																							if (target109(arg137, target110(arg138, <>o__131.<>p__164.Target(<>o__131.<>p__164, (object)data, "CpuAmdCoreFreq"), null)))
																							{
																								CPU cPU12 = currentProfile.CPU;
																								if (<>o__131.<>p__169 == null)
																								{
																									<>o__131.<>p__169 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																								}
																								Func<CallSite, object, int> target111 = <>o__131.<>p__169.Target;
																								CallSite<Func<CallSite, object, int>> arg139 = <>o__131.<>p__169;
																								if (<>o__131.<>p__168 == null)
																								{
																									<>o__131.<>p__168 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																									{
																										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																									}));
																								}
																								Func<CallSite, Type, object, object> target112 = <>o__131.<>p__168.Target;
																								CallSite<Func<CallSite, Type, object, object>> arg140 = <>o__131.<>p__168;
																								Type arg141 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																								if (<>o__131.<>p__167 == null)
																								{
																									<>o__131.<>p__167 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																									{
																										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																									}));
																								}
																								cPU12.AmdCoreFreq = target111(arg139, target112(arg140, arg141, <>o__131.<>p__167.Target(<>o__131.<>p__167, (object)data, "CpuAmdCoreFreq")));
																								SetCpuAmdCoreFreq(currentProfile.CPU.AmdCoreFreq);
																							}
																							else
																							{
																								if (<>o__131.<>p__172 == null)
																								{
																									<>o__131.<>p__172 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																								}
																								Func<CallSite, object, bool> target113 = <>o__131.<>p__172.Target;
																								CallSite<Func<CallSite, object, bool>> arg142 = <>o__131.<>p__172;
																								if (<>o__131.<>p__171 == null)
																								{
																									<>o__131.<>p__171 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																									{
																										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																									}));
																								}
																								Func<CallSite, object, object, object> target114 = <>o__131.<>p__171.Target;
																								CallSite<Func<CallSite, object, object, object>> arg143 = <>o__131.<>p__171;
																								if (<>o__131.<>p__170 == null)
																								{
																									<>o__131.<>p__170 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																									{
																										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																										CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																									}));
																								}
																								if (target113(arg142, target114(arg143, <>o__131.<>p__170.Target(<>o__131.<>p__170, (object)data, "CpuAmdCoreFreqOC"), null)))
																								{
																									CPU cPU13 = currentProfile.CPU;
																									if (<>o__131.<>p__175 == null)
																									{
																										<>o__131.<>p__175 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																									}
																									Func<CallSite, object, int> target115 = <>o__131.<>p__175.Target;
																									CallSite<Func<CallSite, object, int>> arg144 = <>o__131.<>p__175;
																									if (<>o__131.<>p__174 == null)
																									{
																										<>o__131.<>p__174 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																										{
																											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																										}));
																									}
																									Func<CallSite, Type, object, object> target116 = <>o__131.<>p__174.Target;
																									CallSite<Func<CallSite, Type, object, object>> arg145 = <>o__131.<>p__174;
																									Type arg146 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																									if (<>o__131.<>p__173 == null)
																									{
																										<>o__131.<>p__173 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																										{
																											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																										}));
																									}
																									cPU13.AmdCoreFreqOC = target115(arg144, target116(arg145, arg146, <>o__131.<>p__173.Target(<>o__131.<>p__173, (object)data, "CpuAmdCoreFreqOC")));
																									SetCpuAmdCoreFreq(currentProfile.CPU.AmdCoreFreqOC);
																								}
																								else
																								{
																									if (<>o__131.<>p__178 == null)
																									{
																										<>o__131.<>p__178 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																									}
																									Func<CallSite, object, bool> target117 = <>o__131.<>p__178.Target;
																									CallSite<Func<CallSite, object, bool>> arg147 = <>o__131.<>p__178;
																									if (<>o__131.<>p__177 == null)
																									{
																										<>o__131.<>p__177 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																										{
																											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																										}));
																									}
																									Func<CallSite, object, object, object> target118 = <>o__131.<>p__177.Target;
																									CallSite<Func<CallSite, object, object, object>> arg148 = <>o__131.<>p__177;
																									if (<>o__131.<>p__176 == null)
																									{
																										<>o__131.<>p__176 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																										{
																											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																										}));
																									}
																									if (target117(arg147, target118(arg148, <>o__131.<>p__176.Target(<>o__131.<>p__176, (object)data, "CpuAmdCoreVoltage"), null)))
																									{
																										CPU cPU14 = currentProfile.CPU;
																										if (<>o__131.<>p__181 == null)
																										{
																											<>o__131.<>p__181 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																										}
																										Func<CallSite, object, int> target119 = <>o__131.<>p__181.Target;
																										CallSite<Func<CallSite, object, int>> arg149 = <>o__131.<>p__181;
																										if (<>o__131.<>p__180 == null)
																										{
																											<>o__131.<>p__180 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																											{
																												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																											}));
																										}
																										Func<CallSite, Type, object, object> target120 = <>o__131.<>p__180.Target;
																										CallSite<Func<CallSite, Type, object, object>> arg150 = <>o__131.<>p__180;
																										Type arg151 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																										if (<>o__131.<>p__179 == null)
																										{
																											<>o__131.<>p__179 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																											{
																												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																											}));
																										}
																										cPU14.AmdCoreVoltage = target119(arg149, target120(arg150, arg151, <>o__131.<>p__179.Target(<>o__131.<>p__179, (object)data, "CpuAmdCoreVoltage")));
																										SetCpuAmdCoreVoltage(currentProfile.CPU.AmdCoreVoltage);
																									}
																									else
																									{
																										if (<>o__131.<>p__184 == null)
																										{
																											<>o__131.<>p__184 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																										}
																										Func<CallSite, object, bool> target121 = <>o__131.<>p__184.Target;
																										CallSite<Func<CallSite, object, bool>> arg152 = <>o__131.<>p__184;
																										if (<>o__131.<>p__183 == null)
																										{
																											<>o__131.<>p__183 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																											{
																												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																											}));
																										}
																										Func<CallSite, object, object, object> target122 = <>o__131.<>p__183.Target;
																										CallSite<Func<CallSite, object, object, object>> arg153 = <>o__131.<>p__183;
																										if (<>o__131.<>p__182 == null)
																										{
																											<>o__131.<>p__182 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																											{
																												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																												CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																											}));
																										}
																										if (target121(arg152, target122(arg153, <>o__131.<>p__182.Target(<>o__131.<>p__182, (object)data, "CpuAmdCoreVoltageOC"), null)))
																										{
																											CPU cPU15 = currentProfile.CPU;
																											if (<>o__131.<>p__187 == null)
																											{
																												<>o__131.<>p__187 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																											}
																											Func<CallSite, object, int> target123 = <>o__131.<>p__187.Target;
																											CallSite<Func<CallSite, object, int>> arg154 = <>o__131.<>p__187;
																											if (<>o__131.<>p__186 == null)
																											{
																												<>o__131.<>p__186 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																												{
																													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																												}));
																											}
																											Func<CallSite, Type, object, object> target124 = <>o__131.<>p__186.Target;
																											CallSite<Func<CallSite, Type, object, object>> arg155 = <>o__131.<>p__186;
																											Type arg156 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																											if (<>o__131.<>p__185 == null)
																											{
																												<>o__131.<>p__185 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																												{
																													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																												}));
																											}
																											cPU15.AmdCoreVoltageOC = target123(arg154, target124(arg155, arg156, <>o__131.<>p__185.Target(<>o__131.<>p__185, (object)data, "CpuAmdCoreVoltageOC")));
																											SetCpuAmdCoreVoltage(currentProfile.CPU.AmdCoreVoltageOC);
																										}
																										else
																										{
																											if (<>o__131.<>p__190 == null)
																											{
																												<>o__131.<>p__190 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																											}
																											Func<CallSite, object, bool> target125 = <>o__131.<>p__190.Target;
																											CallSite<Func<CallSite, object, bool>> arg157 = <>o__131.<>p__190;
																											if (<>o__131.<>p__189 == null)
																											{
																												<>o__131.<>p__189 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																												{
																													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																												}));
																											}
																											Func<CallSite, object, object, object> target126 = <>o__131.<>p__189.Target;
																											CallSite<Func<CallSite, object, object, object>> arg158 = <>o__131.<>p__189;
																											if (<>o__131.<>p__188 == null)
																											{
																												<>o__131.<>p__188 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																												{
																													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																													CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																												}));
																											}
																											if (target125(arg157, target126(arg158, <>o__131.<>p__188.Target(<>o__131.<>p__188, (object)data, "MemoryOverClockSwitch"), null)))
																											{
																												MemorySettings mEM = currentProfile.MEM;
																												if (<>o__131.<>p__193 == null)
																												{
																													<>o__131.<>p__193 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																												}
																												Func<CallSite, object, int> target127 = <>o__131.<>p__193.Target;
																												CallSite<Func<CallSite, object, int>> arg159 = <>o__131.<>p__193;
																												if (<>o__131.<>p__192 == null)
																												{
																													<>o__131.<>p__192 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																													{
																														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																													}));
																												}
																												Func<CallSite, Type, object, object> target128 = <>o__131.<>p__192.Target;
																												CallSite<Func<CallSite, Type, object, object>> arg160 = <>o__131.<>p__192;
																												Type arg161 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																												if (<>o__131.<>p__191 == null)
																												{
																													<>o__131.<>p__191 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																													{
																														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																													}));
																												}
																												mEM.MemoryOverClockSwitch = target127(arg159, target128(arg160, arg161, <>o__131.<>p__191.Target(<>o__131.<>p__191, (object)data, "MemoryOverClockSwitch")));
																												SetMemoryOverClockSwitch(currentProfile.MEM.MemoryOverClockSwitch);
																											}
																											else
																											{
																												if (<>o__131.<>p__196 == null)
																												{
																													<>o__131.<>p__196 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																												}
																												Func<CallSite, object, bool> target129 = <>o__131.<>p__196.Target;
																												CallSite<Func<CallSite, object, bool>> arg162 = <>o__131.<>p__196;
																												if (<>o__131.<>p__195 == null)
																												{
																													<>o__131.<>p__195 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																													{
																														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																													}));
																												}
																												Func<CallSite, object, object, object> target130 = <>o__131.<>p__195.Target;
																												CallSite<Func<CallSite, object, object, object>> arg163 = <>o__131.<>p__195;
																												if (<>o__131.<>p__194 == null)
																												{
																													<>o__131.<>p__194 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																													{
																														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																														CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																													}));
																												}
																												if (target129(arg162, target130(arg163, <>o__131.<>p__194.Target(<>o__131.<>p__194, (object)data, "CustomTGPinGCUforGN21_Value"), null)))
																												{
																													GPU gPU12 = currentProfile.GPU;
																													if (<>o__131.<>p__199 == null)
																													{
																														<>o__131.<>p__199 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																													}
																													Func<CallSite, object, int> target131 = <>o__131.<>p__199.Target;
																													CallSite<Func<CallSite, object, int>> arg164 = <>o__131.<>p__199;
																													if (<>o__131.<>p__198 == null)
																													{
																														<>o__131.<>p__198 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																														{
																															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																														}));
																													}
																													Func<CallSite, Type, object, object> target132 = <>o__131.<>p__198.Target;
																													CallSite<Func<CallSite, Type, object, object>> arg165 = <>o__131.<>p__198;
																													Type arg166 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																													if (<>o__131.<>p__197 == null)
																													{
																														<>o__131.<>p__197 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																														{
																															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																														}));
																													}
																													gPU12.CustomTGPinGCUforGN21_Value = target131(arg164, target132(arg165, arg166, <>o__131.<>p__197.Target(<>o__131.<>p__197, (object)data, "CustomTGPinGCUforGN21_Value")));
																													string command = ResolvedDelegates.System_String_Concat__29("nvidia-smi -pl ", ResolvedDelegates.System_Int32_ToString__55(ref currentProfile.GPU.CustomTGPinGCUforGN21_Value));
																													ExecuteCommand(command);
																												}
																												else
																												{
																													if (<>o__131.<>p__202 == null)
																													{
																														<>o__131.<>p__202 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																													}
																													Func<CallSite, object, bool> target133 = <>o__131.<>p__202.Target;
																													CallSite<Func<CallSite, object, bool>> arg167 = <>o__131.<>p__202;
																													if (<>o__131.<>p__201 == null)
																													{
																														<>o__131.<>p__201 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																														{
																															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																														}));
																													}
																													Func<CallSite, object, object, object> target134 = <>o__131.<>p__201.Target;
																													CallSite<Func<CallSite, object, object, object>> arg168 = <>o__131.<>p__201;
																													if (<>o__131.<>p__200 == null)
																													{
																														<>o__131.<>p__200 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																														{
																															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																															CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																														}));
																													}
																													if (target133(arg167, target134(arg168, <>o__131.<>p__200.Target(<>o__131.<>p__200, (object)data, "CustomTGPinGCUforGN21_Enable"), null)))
																													{
																														GPU gPU13 = currentProfile.GPU;
																														if (<>o__131.<>p__205 == null)
																														{
																															<>o__131.<>p__205 = CallSite<Func<CallSite, object, int>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(int).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
																														}
																														Func<CallSite, object, int> target135 = <>o__131.<>p__205.Target;
																														CallSite<Func<CallSite, object, int>> arg169 = <>o__131.<>p__205;
																														if (<>o__131.<>p__204 == null)
																														{
																															<>o__131.<>p__204 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToInt32", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																															{
																																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
																																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
																															}));
																														}
																														Func<CallSite, Type, object, object> target136 = <>o__131.<>p__204.Target;
																														CallSite<Func<CallSite, Type, object, object>> arg170 = <>o__131.<>p__204;
																														Type arg171 = ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Convert).TypeHandle);
																														if (<>o__131.<>p__203 == null)
																														{
																															<>o__131.<>p__203 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																															{
																																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																															}));
																														}
																														gPU13.CustomTGPinGCUforGN21_Enable = target135(arg169, target136(arg170, arg171, <>o__131.<>p__203.Target(<>o__131.<>p__203, (object)data, "CustomTGPinGCUforGN21_Enable")));
																													}
																													else
																													{
																														if (<>o__131.<>p__208 == null)
																														{
																															<>o__131.<>p__208 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																														}
																														Func<CallSite, object, bool> target137 = <>o__131.<>p__208.Target;
																														CallSite<Func<CallSite, object, bool>> arg172 = <>o__131.<>p__208;
																														if (<>o__131.<>p__207 == null)
																														{
																															<>o__131.<>p__207 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																															{
																																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																															}));
																														}
																														Func<CallSite, object, object, object> target138 = <>o__131.<>p__207.Target;
																														CallSite<Func<CallSite, object, object, object>> arg173 = <>o__131.<>p__207;
																														if (<>o__131.<>p__206 == null)
																														{
																															<>o__131.<>p__206 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																															{
																																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																																CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																															}));
																														}
																														if (target137(arg172, target138(arg173, <>o__131.<>p__206.Target(<>o__131.<>p__206, (object)data, "CPUPerformanceAndOverClockMenuSwitch_ON"), null)))
																														{
																															NvramVariable.SetFwVars("CPUPerformanceAndOverClockMenuSwitch", 1);
																															NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch = 1;
																															currentProfile.CPU.PerformanceAndOverClockMenuSwitch = 1;
																															currentProfile.MEM.MemoryOverClockSwitch = 1;
																															SetMemoryOverClockSwitch(1);
																															UpdateStatusToClient(1);
																														}
																														else
																														{
																															if (<>o__131.<>p__211 == null)
																															{
																																<>o__131.<>p__211 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
																															}
																															Func<CallSite, object, bool> target139 = <>o__131.<>p__211.Target;
																															CallSite<Func<CallSite, object, bool>> arg174 = <>o__131.<>p__211;
																															if (<>o__131.<>p__210 == null)
																															{
																																<>o__131.<>p__210 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																																{
																																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
																																}));
																															}
																															Func<CallSite, object, object, object> target140 = <>o__131.<>p__210.Target;
																															CallSite<Func<CallSite, object, object, object>> arg175 = <>o__131.<>p__210;
																															if (<>o__131.<>p__209 == null)
																															{
																																<>o__131.<>p__209 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
																																{
																																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
																																	CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
																																}));
																															}
																															if (target139(arg174, target140(arg175, <>o__131.<>p__209.Target(<>o__131.<>p__209, (object)data, "CPUPerformanceAndOverClockMenuSwitch_OFF"), null)))
																															{
																																NvramVariable.SetFwVars("CPUPerformanceAndOverClockMenuSwitch", 0);
																																NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch = 0;
																																currentProfile.CPU.PerformanceAndOverClockMenuSwitch = 0;
																																currentProfile.MEM.MemoryOverClockSwitch = 0;
																																SetMemoryOverClockSwitch(0);
																																UpdateStatusToClient(1);
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			RefreshCurrentProfile(m_Option.OperatingMode);
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception", ResolvedDelegates.System_Exception_get_Message__58(ex)), "UserSet_Mode_Detail", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2146);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UserRestore_Mode_Details()
	{
		currentProfile.OverClockingEnabled = 0;
		LoadOverClockingValue(currentProfile.OverClockingEnabled);
		RestoreCurrentProfile(m_Option.OperatingMode);
		if (m_Option.OperatingMode == 0 && m_GpuWhisperModeSupport)
		{
			currentProfile.GPU.WhisperModeSwitch = 0;
			if (CustomizeCtrl.GetCustomId() == 43 || CustomizeCtrl.GetCustomId() == 39 || CustomizeCtrl.GetCustomId() == 22)
			{
				currentProfile.GPU.WhisperModeSetting = 2;
			}
			else
			{
				currentProfile.GPU.WhisperModeSetting = 0;
			}
			currentProfile.GPU.WhisperModeMinFps_QUIETER = 30;
			currentProfile.GPU.WhisperModeMinFps_QUIET = 40;
			currentProfile.GPU.WhisperModeMinFps_BALANCED = 60;
			WriteMainOptionToJson("WhisperModeSwitch", ResolvedDelegates.System_Convert_ToUInt32__143(currentProfile.GPU.WhisperModeSwitch));
			WriteMainOptionToJson("WhisperModeSetting", ResolvedDelegates.System_Convert_ToUInt32__143(currentProfile.GPU.WhisperModeSetting));
			WriteMainOptionToJson("WhisperModeMinFps_QUIETER", ResolvedDelegates.System_Convert_ToUInt32__143(currentProfile.GPU.WhisperModeMinFps_QUIETER));
			WriteMainOptionToJson("WhisperModeMinFps_QUIET", ResolvedDelegates.System_Convert_ToUInt32__143(currentProfile.GPU.WhisperModeMinFps_QUIET));
			WriteMainOptionToJson("WhisperModeMinFps_BALANCED", ResolvedDelegates.System_Convert_ToUInt32__143(currentProfile.GPU.WhisperModeMinFps_BALANCED));
		}
		if (NvramVariableInfo.APVersionCheck >= 24)
		{
			currentProfile.GPU.CoreClockOffset = 0;
			currentProfile.GPU.CoreClockOffsetOC = 0;
			currentProfile.GPU.MemoryClockOffset = 0;
			currentProfile.GPU.MemoryClockOffsetOC = 0;
		}
		if (NvramVariableInfo.CPUPerformanceAndOverClockMenuSupport == 1)
		{
			byte b = 0;
			NvramVariable.SetFwVars("CPUPerformanceAndOverClockMenuSwitch", b);
			NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch = b;
			currentProfile.MEM.MemoryOverClockSwitch = b;
			SetMemoryOverClockSwitch(b);
		}
		SetUserProfile(m_Option.OperatingMode);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UserSet_DefaultNotifyStatus(uint status)
	{
		if (status == 1)
		{
			m_Option.DefaultNotify = 1u;
		}
		else
		{
			m_Option.DefaultNotify = 0u;
		}
		WriteMainOptionToJson("DefaultNotify", m_Option.DefaultNotify);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void UserSet_FanSafetyProtectNotifyStatus(uint status)
	{
		if (status == 1)
		{
			m_Option.FanSafetyProtectNotify = 1u;
		}
		else
		{
			m_Option.FanSafetyProtectNotify = 0u;
		}
		WriteMainOptionToJson("FanSafetyProtectNotify", m_Option.FanSafetyProtectNotify);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ResetTruboOC(int IsTurboOCAdjustSupport, int IsTurboOCDefualtON, bool IsTurboSubModeSupport, int EC_DB2_Value, uint trubo43, uint trubo46)
	{
		if (IsTurboOCAdjustSupport == 1)
		{
			if (IsTurboOCDefualtON == 1)
			{
				if (IsTurboSubModeSupport)
				{
					WriteMainOptionToJson("TurboSilentGPUOCOffset", trubo43);
					LogCtrl.TraceMessageProgramData("TurboSilentGPUOCOffset: trubo43", "ResetTruboOC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2281);
				}
				WriteMainOptionToJson("TurboGPUOCOffset", trubo46);
				LogCtrl.TraceMessageProgramData("TurboGPUOCOffset: trubo46", "ResetTruboOC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2284);
			}
			else
			{
				if (IsTurboSubModeSupport)
				{
					WriteMainOptionToJson("TurboSilentGPUOCOffset", 0u);
					LogCtrl.TraceMessageProgramData("TurboSilentGPUOCOffset: 0", "ResetTruboOC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2292);
				}
				WriteMainOptionToJson("TurboGPUOCOffset", 0u);
				LogCtrl.TraceMessageProgramData("TurboGPUOCOffset: 0", "ResetTruboOC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2295);
			}
		}
		else if (EC_DB2_Value == 1)
		{
			WriteMainOptionToJson("TurboGPUOCOffset", 105u);
			LogCtrl.TraceMessageProgramData("TurboGPUOCOffset: 105", "ResetTruboOC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2304);
		}
		else
		{
			WriteMainOptionToJson("TurboGPUOCOffset", 0u);
			LogCtrl.TraceMessageProgramData("TurboGPUOCOffset: 0", "ResetTruboOC", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2309);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void LoadProfileAll()
	{
		GetGamingPLDefaultValue(ref GamingModePLDefaultValue.PL1, ref GamingModePLDefaultValue.PL2, ref GamingModePLDefaultValue.PL4, ref GamingModePLDefaultValue.DState);
		GetOfficePLDefaultValue(ref OfficeModePLDefaultValue.PL1, ref OfficeModePLDefaultValue.PL2, ref OfficeModePLDefaultValue.PL4, ref OfficeModePLDefaultValue.DState);
		GetTurboPLDefaultValue(ref TurboModePLDefaultValue.PL1, ref TurboModePLDefaultValue.PL2, ref TurboModePLDefaultValue.PL4, ref TurboModePLDefaultValue.DState);
		GetTccDefaultValue(ref GamingModePLDefaultValue.TCC, ref OfficeModePLDefaultValue.TCC, ref TurboModePLDefaultValue.TCC);
		RefreshCpuPLMinimumValue();
		if (m_IsNvGpu)
		{
			GpuThermalLimitInfo = cGpuFeatures.GetGpuThermalLimitInfo();
			m_GpuWhisperModeSupport = cGpuFeatures.IsGpuWhisperMode2p0Support();
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("NV API WhisperModeSupport: ", ResolvedDelegates.System_Boolean_ToString__147(ref m_GpuWhisperModeSupport)), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2337);
			GpuDynamicBoostDefaultValue = 5;
		}
		LoadNvramVariableInfo();
		LogCtrl.TraceMessage("Get BIOS SMAPCTable once when installing AP for first time.", "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2355);
		_SmartApcTable = SmartApcTableCtrl.Instance.GetSMAPCTable();
		WriteSMAPCTableRegistryAll(_SmartApcTable);
		if (NvramVariableInfo.ApUseFlag != 1)
		{
			LogCtrl.TraceMessage("NvramVariable ApUseFlag != 1, then get BIOS SMAPCTable.", "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2362);
			_SmartApcTable = SmartApcTableCtrl.Instance.GetSMAPCTable();
			NvramVariable.SetFwVars("ApUseFlag", 1);
			WriteSMAPCTableRegistryAll(_SmartApcTable);
		}
		else
		{
			LogCtrl.TraceMessage("NvramVariable ApUseFlag == 1, then get SMAPCTable from local registry.", "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2369);
			try
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				num = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, m_RegistryPath, "PL1", 0);
				num2 = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, m_RegistryPath, "PL2", 0);
				num3 = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, m_RegistryPath, "PL4", 0);
				num4 = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, m_RegistryPath, "TccOffset", 0);
				num5 = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, m_RegistryPath, "ConfigurableTGP", 0);
				num6 = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, m_RegistryPath, "DynamicBoost", 0);
				num7 = (int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, m_RegistryPath, "DefaultTGP", 0);
				_SmartApcTable.PL1 = ResolvedDelegates.System_Convert_ToByte__75(num);
				_SmartApcTable.PL2 = ResolvedDelegates.System_Convert_ToByte__75(num2);
				_SmartApcTable.PL4 = ResolvedDelegates.System_Convert_ToByte__75(num3);
				_SmartApcTable.TccOffset = ResolvedDelegates.System_Convert_ToByte__75(num4);
				_SmartApcTable.ConfigurableTGP = ResolvedDelegates.System_Convert_ToByte__75(num5);
				_SmartApcTable.DynamicBoost = ResolvedDelegates.System_Convert_ToByte__75(num6);
				_SmartApcTable.DefaultTGP = ResolvedDelegates.System_Convert_ToByte__75(num7);
				if (CheckIvalidSmartAPCTable(_SmartApcTable))
				{
					LogCtrl.TraceMessage("NvramVariable ApUseFlag == 1, but smartapctabl error load default", "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2398);
					_SmartApcTable = SmartApcTableCtrl.Instance.GetSMAPCTable();
					WriteSMAPCTableRegistryAll(_SmartApcTable);
				}
			}
			catch
			{
				LogCtrl.TraceMessage("Load SMAPCTable Registry Fail, then get BIOS SMAPCTable.", "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2405);
				_SmartApcTable = SmartApcTableCtrl.Instance.GetSMAPCTable();
				WriteSMAPCTableRegistryAll(_SmartApcTable);
			}
		}
		try
		{
			if ((int)RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, "\\OEM\\GamingCenter2", "DebugMode", 0) == 1)
			{
				NvramVariableInfo.ICpuCoreVoltageOffsetRangeType = 1;
				NvramVariableInfo.CoreVoltageOffsetMaximum = 100;
				NvramVariableInfo.ACpuOverClockSupport = 0;
				NvramVariableInfo.ACpuFreqValueMaximum = 255u;
				NvramVariableInfo.ACpuFreqValueMinimum = 0u;
				NvramVariableInfo.ACpuVoltageValueMaximum = 2000u;
				NvramVariableInfo.ACpuVoltageValueMinimum = 0u;
				NvramVariableInfo.MemoryOverClockSupport = 1;
				_SmartApcTable.PL1 = 120;
				_SmartApcTable.PL2 = 120;
				_SmartApcTable.PL4 = 165;
				if (m_IsAMDPlatform)
				{
					_SmartApcTable.TccOffset = 95;
				}
				else
				{
					_SmartApcTable.TccOffset = 5;
				}
				_SmartApcTable.ConfigurableTGP = 20;
				_SmartApcTable.DynamicBoost = 15;
				_SmartApcTable.DefaultTGP = 115;
				WriteSMAPCTableRegistryAll(_SmartApcTable);
				m_GpuWhisperModeSupport = true;
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Load Features Registry Exception", ResolvedDelegates.System_Exception_get_Message__58(ex)), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2451);
		}
		LogCtrl.TraceMessage("NvramVariableInfo, CoreVoltageOffsetMaximum: " + ResolvedDelegates.System_Int32_ToString__55(ref NvramVariableInfo.CoreVoltageOffsetMaximum) + ", ICpuCoreVoltageOffsetSupport: " + ResolvedDelegates.System_Byte_ToString__119(ref NvramVariableInfo.ICpuCoreVoltageOffsetSupport) + ", ACpuOverClockSupport: " + ResolvedDelegates.System_Byte_ToString__119(ref NvramVariableInfo.ACpuOverClockSupport) + ", ACpuFreqValueMaximum: " + ResolvedDelegates.System_UInt32_ToString__78(ref NvramVariableInfo.ACpuFreqValueMaximum) + ", ACpuFreqValueMinimum: " + ResolvedDelegates.System_UInt32_ToString__78(ref NvramVariableInfo.ACpuFreqValueMinimum) + ", ACpuVoltageValueMaximum: " + ResolvedDelegates.System_UInt32_ToString__78(ref NvramVariableInfo.ACpuVoltageValueMaximum) + ", ACpuVoltageValueMinimum: " + ResolvedDelegates.System_UInt32_ToString__78(ref NvramVariableInfo.ACpuVoltageValueMinimum) + ", OverClockRecoveryFlag: " + ResolvedDelegates.System_Byte_ToString__119(ref NvramVariableInfo.OverClockRecoveryFlag), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2454);
		if (NvramVariableInfo.ICpuCoreVoltageOffsetRangeType == 0)
		{
			CpuOffsetCoreVoltageMinimum = 0;
			CpuOffsetCoreVoltageMaximum = NvramVariableInfo.CoreVoltageOffsetMaximum;
		}
		else if (NvramVariableInfo.ICpuCoreVoltageOffsetRangeType == 1)
		{
			CpuOffsetCoreVoltageMinimum = -NvramVariableInfo.CoreVoltageOffsetMaximum;
			CpuOffsetCoreVoltageMaximum = 0;
		}
		else if (NvramVariableInfo.ICpuCoreVoltageOffsetRangeType == 2)
		{
			CpuOffsetCoreVoltageMinimum = -NvramVariableInfo.CoreVoltageOffsetMaximum;
			CpuOffsetCoreVoltageMaximum = NvramVariableInfo.CoreVoltageOffsetMaximum;
		}
		LogCtrl.TraceMessage("ICpuCoreVoltageOffset, RangeType: " + ResolvedDelegates.System_Byte_ToString__119(ref NvramVariableInfo.ICpuCoreVoltageOffsetRangeType) + ", Minimum: " + ResolvedDelegates.System_Int32_ToString__55(ref CpuOffsetCoreVoltageMinimum) + ", Maximum: " + ResolvedDelegates.System_Int32_ToString__55(ref CpuOffsetCoreVoltageMaximum), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2483);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__79(new string[14]
		{
			"SmartApcTable, PL1: ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.PL1),
			", PL2: ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.PL2),
			", PL4: ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.PL4),
			", TccOffset: ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.TccOffset),
			", ConfigurableTGP(range): ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.ConfigurableTGP),
			", DynamicBoost: ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.DynamicBoost),
			", DefaultTGP(base): ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.DefaultTGP)
		}), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2487);
		CheckInvalidSmartApcTable();
		m_GpuConfigurableTGPMaximum = _SmartApcTable.DefaultTGP + _SmartApcTable.ConfigurableTGP;
		m_GpuConfigurableTGPSupport = _SmartApcTable.ConfigurableTGP > 0;
		m_GpuDynamicBoostSupport = _SmartApcTable.DynamicBoost > 0;
		if (ResolvedDelegates.System_IO_Directory_Exists__283(m_path))
		{
			try
			{
				if (ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "TurboModeSupport", "0")), "1"))
				{
					int dBType = EcCtrl.GetDBType();
					string text = ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "IsTurboOCAdjustSupport", "-1"));
					string text2 = ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "IsTurboOCDefualtON", "-1"));
					string text3 = ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "DB2", "-1"));
					if (ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\MySetting"), "TURBO_GPU_OC", "null")), "null"))
					{
						ResetTruboOC(m_IsTurboOCAdjustSupport, m_IsTurboOCDefualtON, m_IsTurboSubModeSupport, dBType, trubo43, trubo46);
					}
					else if (text3 != ResolvedDelegates.System_Int32_ToString__55(ref dBType) || ResolvedDelegates.System_String_op_Inequality__40(text, ResolvedDelegates.System_Int32_ToString__55(ref m_IsTurboOCAdjustSupport)) || ResolvedDelegates.System_String_op_Inequality__40(text2, ResolvedDelegates.System_Int32_ToString__55(ref m_IsTurboOCDefualtON)))
					{
						LogCtrl.TraceMessageProgramData("DB2 exchanged!!!", "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2529);
						LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("****REG_EC_DB2_Value", text3), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2530);
						LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("****EC_DB2_Value.ToString()", ResolvedDelegates.System_Int32_ToString__55(ref dBType)), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2531);
						LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("****REG_IsTurboOCAdjustSupport", text), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2532);
						LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("**** m_IsTurboOCAdjustSupport.ToString()", ResolvedDelegates.System_Int32_ToString__55(ref m_IsTurboOCAdjustSupport)), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2533);
						LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("**** REG_IsTurboOCDefualtON", text2), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2534);
						LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("**** m_IsTurboOCDefualtON.ToString()", ResolvedDelegates.System_Int32_ToString__55(ref m_IsTurboOCDefualtON)), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2535);
						ResetTruboOC(m_IsTurboOCAdjustSupport, m_IsTurboOCDefualtON, m_IsTurboSubModeSupport, dBType, trubo43, trubo46);
						try
						{
							RegistryCtrl.RegistrySoftwareKeyDelete(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\MySetting"), "TURBO_GPU_OC");
						}
						catch
						{
						}
					}
					RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "IsTurboOCAdjustSupport", m_IsTurboOCAdjustSupport, RegistryValueKind.DWord);
					RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "IsTurboOCDefualtON", m_IsTurboOCDefualtON, RegistryValueKind.DWord);
					RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "TurboOCAdjust43", trubo43, RegistryValueKind.DWord);
					RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "TurboOCAdjust46", trubo46, RegistryValueKind.DWord);
					RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "DB2", dBType, RegistryValueKind.DWord);
				}
			}
			catch (Exception ex2)
			{
				LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("TurboGPUOCOffset: ", ResolvedDelegates.System_Object_ToString__1(ex2)), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2554);
			}
			m_Option = LoadMainOptionFromJson("MainOption");
			Mode1_Profile1 = LoadProfileFromJson("Mode1_Profile1");
			Mode1_Profile2 = LoadProfileFromJson("Mode1_Profile2");
			Mode2_Profile1 = LoadProfileFromJson("Mode2_Profile1");
			Mode2_Profile2 = LoadProfileFromJson("Mode2_Profile2");
			Mode3_Profile1 = LoadProfileFromJson("Mode3_Profile1");
			Mode3_Profile2 = LoadProfileFromJson("Mode3_Profile2");
			Mode4_Profile1 = LoadProfileFromJson("Mode4_Profile1");
			Mode4_Profile2 = LoadProfileFromJson("Mode4_Profile2");
			Mode4_Profile3 = LoadProfileFromJson("Mode4_Profile3");
			Mode4_Profile4 = LoadProfileFromJson("Mode4_Profile4");
			Mode4_Profile5 = LoadProfileFromJson("Mode4_Profile5");
			RefreshDefaultProfiles();
		}
		else
		{
			LoadDefault();
		}
		if (g_TurboModeSupport == 0 && m_Option.OperatingMode == 2)
		{
			m_Option.OperatingMode = m_DefaultMode;
		}
		string text4 = "performance mode is: ";
		uint operatingMode = m_Option.OperatingMode;
		LogCtrl.TraceMessageProgramData(text4 + ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode), "LoadProfileAll", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2592);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void WriteSMAPCTableRegistryAll(SMAPCTABLE_STRUCT _table)
	{
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, m_RegistryPath, "PL1", _table.PL1, RegistryValueKind.DWord);
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, m_RegistryPath, "PL2", _table.PL2, RegistryValueKind.DWord);
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, m_RegistryPath, "PL4", _table.PL4, RegistryValueKind.DWord);
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, m_RegistryPath, "TccOffset", _table.TccOffset, RegistryValueKind.DWord);
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, m_RegistryPath, "ConfigurableTGP", _table.ConfigurableTGP, RegistryValueKind.DWord);
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, m_RegistryPath, "DynamicBoost", _table.DynamicBoost, RegistryValueKind.DWord);
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, m_RegistryPath, "DefaultTGP", _table.DefaultTGP, RegistryValueKind.DWord);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool CheckIvalidSmartAPCTable(SMAPCTABLE_STRUCT _SmartApcTable)
	{
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__79(new string[14]
		{
			"Check SmartApcTable, PL1: ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.PL1),
			", PL2: ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.PL2),
			", PL4: ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.PL4),
			", TccOffset: ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.TccOffset),
			", ConfigurableTGP(range): ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.ConfigurableTGP),
			", DynamicBoost: ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.DynamicBoost),
			", DefaultTGP(base): ",
			ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.DefaultTGP)
		}), "CheckIvalidSmartAPCTable", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2608);
		if (_SmartApcTable.PL4 == 0)
		{
			return true;
		}
		if (_SmartApcTable.PL1 != byte.MaxValue)
		{
			return false;
		}
		if (_SmartApcTable.PL2 != byte.MaxValue)
		{
			return false;
		}
		if (_SmartApcTable.PL4 != byte.MaxValue)
		{
			return false;
		}
		if (_SmartApcTable.TccOffset != byte.MaxValue)
		{
			return false;
		}
		if (_SmartApcTable.ConfigurableTGP != byte.MaxValue)
		{
			return false;
		}
		if (_SmartApcTable.DynamicBoost != byte.MaxValue)
		{
			return false;
		}
		if (_SmartApcTable.DefaultTGP != byte.MaxValue)
		{
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void CheckInvalidSmartApcTable()
	{
		if (_SmartApcTable.PL1 == byte.MaxValue)
		{
			_SmartApcTable.PL1 = 0;
		}
		if (_SmartApcTable.PL2 == byte.MaxValue)
		{
			_SmartApcTable.PL2 = 0;
		}
		if (_SmartApcTable.PL4 == byte.MaxValue)
		{
			_SmartApcTable.PL4 = 0;
		}
		if (_SmartApcTable.TccOffset == byte.MaxValue)
		{
			_SmartApcTable.TccOffset = 0;
		}
		if (_SmartApcTable.ConfigurableTGP == byte.MaxValue)
		{
			_SmartApcTable.ConfigurableTGP = 0;
		}
		if (_SmartApcTable.DynamicBoost == byte.MaxValue)
		{
			_SmartApcTable.DynamicBoost = 0;
		}
		if (_SmartApcTable.DefaultTGP == byte.MaxValue)
		{
			_SmartApcTable.DefaultTGP = 0;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void RefreshDefaultProfiles()
	{
		if (!Mode1_Profile1.Activated)
		{
			RefreshMode(ref Mode1_Profile1, "Mode1_Profile1", "1", GamingModePLDefaultValue, "M1T1");
			CreateModeProfiles(Mode1_Profile1);
		}
		if (!Mode1_Profile2.Activated)
		{
			RefreshMode(ref Mode1_Profile2, "Mode1_Profile2", "2", GamingModePLDefaultValue, "M1T2");
			CreateModeProfiles(Mode1_Profile2);
		}
		if (!Mode2_Profile1.Activated)
		{
			RefreshMode(ref Mode2_Profile1, "Mode2_Profile1", "1", OfficeModePLDefaultValue, "M2T1");
			CreateModeProfiles(Mode2_Profile1);
		}
		if (!Mode2_Profile2.Activated)
		{
			RefreshMode(ref Mode2_Profile2, "Mode2_Profile2", "2", OfficeModePLDefaultValue, "M2T2");
			CreateModeProfiles(Mode2_Profile2);
		}
		if (!Mode3_Profile1.Activated)
		{
			RefreshMode(ref Mode3_Profile1, "Mode3_Profile1", "1", TurboModePLDefaultValue, "M3T1");
			CreateModeProfiles(Mode3_Profile1);
		}
		if (!Mode3_Profile2.Activated)
		{
			RefreshMode(ref Mode3_Profile2, "Mode3_Profile2", "2", TurboModePLDefaultValue, "M3T2");
			CreateModeProfiles(Mode3_Profile2);
		}
		PLDefaultValue gamingModePLDefaultValue = GamingModePLDefaultValue;
		if (!Mode4_Profile1.Activated)
		{
			RefreshMode(ref Mode4_Profile1, "Mode4_Profile1", "1", gamingModePLDefaultValue, "M4T1");
			CreateModeProfiles(Mode4_Profile1);
		}
		if (!Mode4_Profile2.Activated)
		{
			RefreshMode(ref Mode4_Profile2, "Mode4_Profile2", "2", gamingModePLDefaultValue, "M4T2");
			CreateModeProfiles(Mode4_Profile2);
		}
		if (!Mode4_Profile3.Activated)
		{
			RefreshMode(ref Mode4_Profile3, "Mode4_Profile3", "3", gamingModePLDefaultValue, "M4T3");
			CreateModeProfiles(Mode4_Profile3);
		}
		if (!Mode4_Profile4.Activated)
		{
			RefreshMode(ref Mode4_Profile4, "Mode4_Profile4", "4", gamingModePLDefaultValue, "M4T4");
			CreateModeProfiles(Mode4_Profile4);
		}
		if (!Mode4_Profile5.Activated)
		{
			RefreshMode(ref Mode4_Profile5, "Mode4_Profile5", "5", gamingModePLDefaultValue, "M4T5");
			CreateModeProfiles(Mode4_Profile5);
		}
		switch (m_Option.CustomProfileIndex)
		{
		case 0u:
			if (Mode4_Profile1.CPU.PerformanceAndOverClockMenuSwitch != NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch)
			{
				Mode4_Profile1.CPU.PerformanceAndOverClockMenuSwitch = NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch;
				CreateModeProfiles(Mode4_Profile1);
			}
			break;
		case 1u:
			if (Mode4_Profile2.CPU.PerformanceAndOverClockMenuSwitch != NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch)
			{
				Mode4_Profile2.CPU.PerformanceAndOverClockMenuSwitch = NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch;
				CreateModeProfiles(Mode4_Profile2);
			}
			break;
		case 2u:
			if (Mode4_Profile3.CPU.PerformanceAndOverClockMenuSwitch != NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch)
			{
				Mode4_Profile3.CPU.PerformanceAndOverClockMenuSwitch = NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch;
				CreateModeProfiles(Mode4_Profile3);
			}
			break;
		case 3u:
			if (Mode4_Profile4.CPU.PerformanceAndOverClockMenuSwitch != NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch)
			{
				Mode4_Profile4.CPU.PerformanceAndOverClockMenuSwitch = NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch;
				CreateModeProfiles(Mode4_Profile4);
			}
			break;
		case 4u:
			if (Mode4_Profile5.CPU.PerformanceAndOverClockMenuSwitch != NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch)
			{
				Mode4_Profile5.CPU.PerformanceAndOverClockMenuSwitch = NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch;
				CreateModeProfiles(Mode4_Profile5);
			}
			break;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void LoadDefault()
	{
		currentProfile.OverClockingEnabled = 0;
		currentProfile.Extreme = 0;
		currentProfile.Silent = 1;
		currentProfile.SmartBalance = 0;
		m_Option.OperatingMode = m_DefaultMode;
		m_Option.GamingProfileIndex = 0u;
		m_Option.OfficeProfileIndex = 0u;
		m_Option.TurboProfileIndex = 0u;
		m_Option.CustomProfileIndex = 0u;
		m_Option.FanBoostEnable = 0u;
		m_Option.FanSafetyProtectNotify = 0u;
		m_Option.DefaultNotify = 0u;
		m_Option.BsodTimestampRestored = ResolvedDelegates.System_DateTime_get_Now__170();
		currentProfile.GPU.WhisperModeSwitch = 0;
		currentProfile.GPU.WhisperModeSetting = 0;
		currentProfile.GPU.WhisperModeMinFps_QUIETER = 30;
		currentProfile.GPU.WhisperModeMinFps_QUIET = 40;
		currentProfile.GPU.WhisperModeMinFps_BALANCED = 60;
		CreateMainOption(m_Option);
		RefreshMode(ref Mode1_Profile1, "Mode1_Profile1", "1", GamingModePLDefaultValue, "M1T1");
		RefreshMode(ref Mode1_Profile2, "Mode1_Profile2", "2", GamingModePLDefaultValue, "M1T2");
		RefreshMode(ref Mode2_Profile1, "Mode2_Profile1", "1", OfficeModePLDefaultValue, "M2T1");
		RefreshMode(ref Mode2_Profile2, "Mode2_Profile2", "2", OfficeModePLDefaultValue, "M2T2");
		RefreshMode(ref Mode3_Profile1, "Mode3_Profile1", "1", TurboModePLDefaultValue, "M3T1");
		RefreshMode(ref Mode3_Profile2, "Mode3_Profile2", "2", TurboModePLDefaultValue, "M3T2");
		PLDefaultValue gamingModePLDefaultValue = GamingModePLDefaultValue;
		RefreshMode(ref Mode4_Profile1, "Mode4_Profile1", "1", gamingModePLDefaultValue, "M4T1");
		RefreshMode(ref Mode4_Profile2, "Mode4_Profile2", "2", gamingModePLDefaultValue, "M4T2");
		RefreshMode(ref Mode4_Profile3, "Mode4_Profile3", "3", gamingModePLDefaultValue, "M4T3");
		RefreshMode(ref Mode4_Profile4, "Mode4_Profile4", "4", gamingModePLDefaultValue, "M4T4");
		RefreshMode(ref Mode4_Profile5, "Mode4_Profile5", "5", gamingModePLDefaultValue, "M4T5");
		CreateModeProfiles(Mode1_Profile1);
		CreateModeProfiles(Mode1_Profile2);
		CreateModeProfiles(Mode2_Profile1);
		CreateModeProfiles(Mode2_Profile2);
		CreateModeProfiles(Mode3_Profile1);
		CreateModeProfiles(Mode3_Profile2);
		CreateModeProfiles(Mode4_Profile1);
		CreateModeProfiles(Mode4_Profile2);
		CreateModeProfiles(Mode4_Profile3);
		CreateModeProfiles(Mode4_Profile4);
		CreateModeProfiles(Mode4_Profile5);
		object obj = ResolvedDelegates.System_String_Concat__29;
		string text = "performance mode is: ";
		uint operatingMode = m_Option.OperatingMode;
		LogCtrl.TraceMessageProgramData(obj(text, ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode)), "LoadDefault", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 2911);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe void RefreshMode(ref ModeProfile profile, string name, string customizeName, PLDefaultValue defaultValue, string fanTableName)
	{
		profile.Activated = false;
		profile.Name = name;
		profile.CustomizeName = customizeName;
		profile.OverClockingEnabled = 0;
		profile.FAN.FanSwitchSpeedEnabled = 0;
		profile.FAN.FanSwitchSpeed = 300;
		profile.FAN.TableName = fanTableName;
		profile.CPU.PL1 = defaultValue.PL1;
		profile.CPU.PL2 = defaultValue.PL2;
		profile.CPU.PL4 = defaultValue.PL4;
		profile.CPU.OffsetCoreVoltage = 0;
		profile.CPU.OffsetCoreVoltageOC = 0;
		profile.CPU.TccOffsetSwitch = 0;
		profile.CPU.TccOffset = defaultValue.TCC;
		profile.GPU.CoreClockOffset = 0;
		profile.GPU.CoreClockOffsetOC = 0;
		profile.GPU.MemoryClockOffset = 0;
		profile.GPU.MemoryClockOffsetOC = 0;
		if (CustomizeCtrl.GetCustomId() == 9 && projectID == 5894)
		{
			profile.GPU.TargetTemperature = 75;
		}
		else
		{
			profile.GPU.TargetTemperature = GpuThermalLimitInfo.DefaultTemperature;
		}
		profile.GPU.ConfigurableTGPSwitch = 0;
		profile.GPU.ConfigurableTGPTarget = _SmartApcTable.DefaultTGP;
		profile.GPU.DynamicBoostSwitch = 0;
		profile.GPU.DynamicBoost = _SmartApcTable.DynamicBoost;
		if (ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_String_Substring__109(ref *(string*)profile.Name, 0, 5), "Mode1") && CustomizeCtrl.GetCustomId() != 42)
		{
			if (_skuGN21 == 16 || _skuGN21 == 21)
			{
				profile.GPU.ConfigurableTGPSwitch = 0;
				profile.GPU.ConfigurableTGPTarget = _SmartApcTable.DefaultTGP;
			}
			else
			{
				profile.GPU.ConfigurableTGPSwitch = 1;
				profile.GPU.ConfigurableTGPTarget = m_GpuConfigurableTGPMaximum;
			}
		}
		else if (profile.Name.Substring(0, 5) == "Mode3")
		{
			if (CustomizeCtrl.GetCustomId() == 42)
			{
				profile.GPU.DynamicBoostSwitch = 1;
				profile.GPU.DynamicBoost = _SmartApcTable.DynamicBoost;
			}
			else
			{
				if (_skuGN21 == 2 || _skuGN21 == 8 || _skuGN21 == 12)
				{
					profile.GPU.CoreClockOffset = 50;
					profile.GPU.CoreClockOffsetOC = 50;
				}
				else
				{
					profile.GPU.CoreClockOffset = 0;
					profile.GPU.CoreClockOffsetOC = 0;
					profile.GPU.MemoryClockOffset = 0;
					profile.GPU.MemoryClockOffsetOC = 0;
				}
				profile.GPU.ConfigurableTGPSwitch = 1;
				profile.GPU.ConfigurableTGPTarget = m_GpuConfigurableTGPMaximum + _SmartApcTable.DynamicBoost;
				profile.GPU.DynamicBoostSwitch = 0;
			}
		}
		else if (profile.Name.Substring(0, 5) == "Mode4")
		{
			profile.GPU.ConfigurableTGPSwitch = 1;
			profile.GPU.ConfigurableTGPTarget = _SmartApcTable.DefaultTGP + _SmartApcTable.ConfigurableTGP;
			if (_SmartApcTable.DynamicBoost == 0)
			{
				profile.GPU.DynamicBoostSwitch = 0;
			}
			else
			{
				profile.GPU.DynamicBoostSwitch = 1;
			}
			profile.GPU.DynamicBoost = _SmartApcTable.DynamicBoost;
		}
		if (CustomizeCtrl.GetCustomId() == 33)
		{
			if (ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_String_Substring__109(ref *(string*)profile.Name, 0, 5), "Mode1"))
			{
				profile.GPU.ConfigurableTGPSwitch = 1;
				profile.GPU.ConfigurableTGPTarget = _SmartApcTable.DefaultTGP + _SmartApcTable.ConfigurableTGP;
				profile.GPU.DynamicBoostSwitch = 1;
				profile.GPU.DynamicBoost = _SmartApcTable.DynamicBoost;
			}
			else if (profile.Name.Substring(0, 5) == "Mode3")
			{
				profile.CPU.PL1 = _SmartApcTable.PL1;
				profile.CPU.PL2 = _SmartApcTable.PL2;
				profile.CPU.PL4 = _SmartApcTable.PL4;
				profile.GPU.ConfigurableTGPSwitch = 1;
				profile.GPU.ConfigurableTGPTarget = _SmartApcTable.DefaultTGP + _SmartApcTable.ConfigurableTGP;
				profile.GPU.DynamicBoostSwitch = 1;
				profile.GPU.DynamicBoost = _SmartApcTable.DynamicBoost;
			}
		}
		if (CustomizeCtrl.GetCustomId() == 26)
		{
			if (ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_String_Substring__109(ref *(string*)profile.Name, 0, 5), "Mode1"))
			{
				if (_skuGN21 == 16 || _skuGN21 == 21 || _skuGN21 == 17 || _skuGN21 == 18)
				{
					profile.GPU.ConfigurableTGPSwitch = 0;
					profile.GPU.ConfigurableTGPTarget = _SmartApcTable.DefaultTGP;
				}
				else
				{
					profile.GPU.ConfigurableTGPSwitch = 1;
					profile.GPU.ConfigurableTGPTarget = m_GpuConfigurableTGPMaximum;
				}
			}
			else if (profile.Name.Substring(0, 5) == "Mode3")
			{
				if (_skuGN21 == 2 || _skuGN21 == 8 || _skuGN21 == 12)
				{
					profile.GPU.CoreClockOffset = 50;
					profile.GPU.CoreClockOffsetOC = 50;
				}
				else
				{
					profile.GPU.CoreClockOffset = 100;
					profile.GPU.CoreClockOffsetOC = 100;
					profile.GPU.MemoryClockOffset = 500;
					profile.GPU.MemoryClockOffsetOC = 500;
				}
				if (_skuGN21 == 16 || _skuGN21 == 21 || _skuGN21 == 17 || _skuGN21 == 18)
				{
					profile.GPU.ConfigurableTGPSwitch = 0;
					profile.GPU.ConfigurableTGPTarget = _SmartApcTable.DefaultTGP;
					profile.GPU.DynamicBoostSwitch = 1;
					profile.GPU.DynamicBoost = _SmartApcTable.DynamicBoost;
				}
				else
				{
					profile.GPU.ConfigurableTGPSwitch = 1;
					profile.GPU.ConfigurableTGPTarget = m_GpuConfigurableTGPMaximum + _SmartApcTable.DynamicBoost;
					profile.GPU.DynamicBoostSwitch = 0;
				}
			}
			else if (profile.Name.Substring(0, 5) == "Mode4")
			{
				if (_skuGN21 == 16 || _skuGN21 == 21 || _skuGN21 == 17 || _skuGN21 == 18)
				{
					profile.GPU.ConfigurableTGPSwitch = 0;
					profile.GPU.ConfigurableTGPTarget = _SmartApcTable.DefaultTGP;
					if (_SmartApcTable.DynamicBoost == 0)
					{
						profile.GPU.DynamicBoostSwitch = 0;
					}
					else
					{
						profile.GPU.DynamicBoostSwitch = 1;
					}
					profile.GPU.DynamicBoost = _SmartApcTable.DynamicBoost;
				}
				else
				{
					profile.GPU.ConfigurableTGPSwitch = 1;
					profile.GPU.ConfigurableTGPTarget = _SmartApcTable.DefaultTGP + _SmartApcTable.ConfigurableTGP;
					if (_SmartApcTable.DynamicBoost == 0)
					{
						profile.GPU.DynamicBoostSwitch = 0;
					}
					else
					{
						profile.GPU.DynamicBoostSwitch = 1;
					}
					profile.GPU.DynamicBoost = _SmartApcTable.DynamicBoost;
				}
			}
		}
		if (NvramVariableInfo.APVersionCheck >= 24)
		{
			profile.GPU.CoreClockOffset = 0;
			profile.GPU.CoreClockOffsetOC = 0;
			profile.GPU.MemoryClockOffset = 0;
			profile.GPU.MemoryClockOffsetOC = 0;
			profile.GPU.DynamicBoostSwitch = 0;
			profile.GPU.DynamicBoost = 5;
		}
		profile.GPU.DynamicBoostTotalProcessingPowerTarget = 255;
		profile.GPU.WhisperModeSwitch = 0;
		if (CustomizeCtrl.GetCustomId() == 43 || CustomizeCtrl.GetCustomId() == 39 || CustomizeCtrl.GetCustomId() == 22)
		{
			profile.GPU.WhisperModeSetting = 2;
		}
		else
		{
			profile.GPU.WhisperModeSetting = 0;
		}
		profile.GPU.WhisperModeMinFps_QUIETER = 30;
		profile.GPU.WhisperModeMinFps_QUIET = 40;
		profile.GPU.WhisperModeMinFps_BALANCED = 60;
		profile.CPU.AmdSPL = defaultValue.PL1;
		profile.CPU.AmdSPPT = defaultValue.PL2;
		profile.CPU.AmdFPPT = defaultValue.PL4;
		profile.CPU.AmdTccTarget = defaultValue.TCC;
		profile.CPU.AmdCoreFreq = 0;
		profile.CPU.AmdCoreFreqOC = 0;
		profile.CPU.AmdCoreVoltage = 0;
		profile.CPU.AmdCoreVoltageOC = 0;
		profile.MEM.MemoryOverClockSwitch = 0;
		profile.CPU.PerformanceAndOverClockMenuSwitch = 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void CreateMainOption(MainOption option)
	{
		try
		{
			string path = ResolvedDelegates.System_String_Concat__29(m_path, "\\MainOption.json");
			if (!ResolvedDelegates.System_IO_Directory_Exists__283(m_path))
			{
				ResolvedDelegates.System_IO_Directory_CreateDirectory__232(m_path);
			}
			string contents = JsonConvert.SerializeObject((object)option, (Formatting)1);
			File.WriteAllText(path, contents);
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception", ResolvedDelegates.System_Exception_get_Message__58(ex)), "CreateMainOption", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3156);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void CreateModeProfiles(ModeProfile profile)
	{
		try
		{
			string path = ResolvedDelegates.System_String_Concat__120(m_path, "\\", profile.Name, ".json");
			if (!ResolvedDelegates.System_IO_Directory_Exists__283(m_path))
			{
				ResolvedDelegates.System_IO_Directory_CreateDirectory__232(m_path);
			}
			string contents = JsonConvert.SerializeObject((object)profile, (Formatting)1);
			File.WriteAllText(path, contents);
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception", ResolvedDelegates.System_Exception_get_Message__58(ex)), "CreateModeProfiles", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3174);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private MainOption LoadMainOptionFromJson(string fileName)
	{
		MainOption mainOption = new MainOption();
		object obj = LocalFileLoader.LoadJsonFromPath_NonAsync<object>(m_path, fileName, ResolvedDelegates.System_Reflection_Assembly_GetExecutingAssembly__96());
		if (<>o__145.<>p__1 == null)
		{
			<>o__145.<>p__1 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target = <>o__145.<>p__1.Target;
		CallSite<Func<CallSite, object, bool>> arg = <>o__145.<>p__1;
		if (<>o__145.<>p__0 == null)
		{
			<>o__145.<>p__0 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		if (target(arg, <>o__145.<>p__0.Target(<>o__145.<>p__0, obj, null)))
		{
			if (<>o__145.<>p__3 == null)
			{
				<>o__145.<>p__3 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
			}
			Func<CallSite, object, string> target2 = <>o__145.<>p__3.Target;
			CallSite<Func<CallSite, object, string>> arg2 = <>o__145.<>p__3;
			if (<>o__145.<>p__2 == null)
			{
				<>o__145.<>p__2 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "Stringify", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			mainOption = JsonConvert.DeserializeObject<MainOption>(target2(arg2, <>o__145.<>p__2.Target(<>o__145.<>p__2, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Json).TypeHandle), obj)));
		}
		else
		{
			CreateMainOption(mainOption);
		}
		return mainOption;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private ModeProfile LoadProfileFromJson(string fileName)
	{
		ModeProfile result = new ModeProfile();
		object obj = LocalFileLoader.LoadJsonFromPath_NonAsync<object>(m_path, fileName, ResolvedDelegates.System_Reflection_Assembly_GetExecutingAssembly__96());
		if (<>o__146.<>p__1 == null)
		{
			<>o__146.<>p__1 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, bool> target = <>o__146.<>p__1.Target;
		CallSite<Func<CallSite, object, bool>> arg = <>o__146.<>p__1;
		if (<>o__146.<>p__0 == null)
		{
			<>o__146.<>p__0 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
			}));
		}
		if (target(arg, <>o__146.<>p__0.Target(<>o__146.<>p__0, obj, null)))
		{
			if (<>o__146.<>p__3 == null)
			{
				<>o__146.<>p__3 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
			}
			Func<CallSite, object, string> target2 = <>o__146.<>p__3.Target;
			CallSite<Func<CallSite, object, string>> arg2 = <>o__146.<>p__3;
			if (<>o__146.<>p__2 == null)
			{
				<>o__146.<>p__2 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "Stringify", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				}));
			}
			result = JsonConvert.DeserializeObject<ModeProfile>(target2(arg2, <>o__146.<>p__2.Target(<>o__146.<>p__2, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(Json).TypeHandle), obj)));
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void WriteMainOptionToJson(string key, uint value)
	{
		try
		{
			string text = ResolvedDelegates.System_String_Concat__29(m_path, "\\MainOption.json");
			if (ResolvedDelegates.System_IO_File_Exists__72(text))
			{
				object obj = JsonConvert.DeserializeObject(ResolvedDelegates.System_IO_File_ReadAllText__234(text));
				if (<>o__147.<>p__0 == null)
				{
					<>o__147.<>p__0 = CallSite<Func<CallSite, object, string, uint, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.SetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[3]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
					}));
				}
				<>o__147.<>p__0.Target(<>o__147.<>p__0, obj, key, value);
				if (<>o__147.<>p__2 == null)
				{
					<>o__147.<>p__2 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, string> target = <>o__147.<>p__2.Target;
				CallSite<Func<CallSite, object, string>> arg = <>o__147.<>p__2;
				if (<>o__147.<>p__1 == null)
				{
					<>o__147.<>p__1 = CallSite<Func<CallSite, Type, object, Formatting, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "SerializeObject", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[3]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				string text2 = target(arg, <>o__147.<>p__1.Target(<>o__147.<>p__1, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(JsonConvert).TypeHandle), obj, (Formatting)1));
				ResolvedDelegates.System_IO_File_WriteAllText__236(text, text2);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception", ResolvedDelegates.System_Exception_get_Message__58(ex)), "WriteMainOptionToJson", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3230);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void WriteProfileToJson(ModeProfile profile)
	{
		try
		{
			if (m_Option.OperatingMode != 3)
			{
				profile.OverClockingEnabled = 0;
			}
			string path = m_path + "\\" + profile.Name + ".json";
			profile.Activated = true;
			string contents = JsonConvert.SerializeObject((object)profile, (Formatting)1);
			File.WriteAllText(path, contents);
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception", ResolvedDelegates.System_Exception_get_Message__58(ex)), "WriteProfileToJson", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3247);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void WriteCustomOSDstringToJson(dynamic newProfileName)
	{
		try
		{
			string text = (string)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\OEM\\GamingCenter2")?.GetValue("Language");
			string text2 = ResolvedDelegates.System_String_Concat__120(ResolvedDelegates.System_String_Concat__29(LogCtrl.GetParentDirectoryPath(ResolvedDelegates.System_Windows_Forms_Application_get_StartupPath__41(), 2), "\\logo"), "\\Language\\", text, ".json");
			if (<>o__149.<>p__3 == null)
			{
				<>o__149.<>p__3 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
			}
			Func<CallSite, object, string> target = <>o__149.<>p__3.Target;
			CallSite<Func<CallSite, object, string>> arg = <>o__149.<>p__3;
			if (<>o__149.<>p__2 == null)
			{
				<>o__149.<>p__2 = CallSite<Func<CallSite, object, int, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, int, object> target2 = <>o__149.<>p__2.Target;
			CallSite<Func<CallSite, object, int, object>> arg2 = <>o__149.<>p__2;
			if (<>o__149.<>p__1 == null)
			{
				<>o__149.<>p__1 = CallSite<Func<CallSite, object, char, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "Split", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, char, object> target3 = <>o__149.<>p__1.Target;
			CallSite<Func<CallSite, object, char, object>> arg3 = <>o__149.<>p__1;
			if (<>o__149.<>p__0 == null)
			{
				<>o__149.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			string text3 = target(arg, target2(arg2, target3(arg3, <>o__149.<>p__0.Target(<>o__149.<>p__0, (object)newProfileName), '-'), 0));
			if (<>o__149.<>p__7 == null)
			{
				<>o__149.<>p__7 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
			}
			Func<CallSite, object, string> target4 = <>o__149.<>p__7.Target;
			CallSite<Func<CallSite, object, string>> arg4 = <>o__149.<>p__7;
			if (<>o__149.<>p__6 == null)
			{
				<>o__149.<>p__6 = CallSite<Func<CallSite, object, int, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, int, object> target5 = <>o__149.<>p__6.Target;
			CallSite<Func<CallSite, object, int, object>> arg5 = <>o__149.<>p__6;
			if (<>o__149.<>p__5 == null)
			{
				<>o__149.<>p__5 = CallSite<Func<CallSite, object, char, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "Split", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[2]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Func<CallSite, object, char, object> target6 = <>o__149.<>p__5.Target;
			CallSite<Func<CallSite, object, char, object>> arg6 = <>o__149.<>p__5;
			if (<>o__149.<>p__4 == null)
			{
				<>o__149.<>p__4 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "ToString", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			string text4 = target4(arg4, target5(arg5, target6(arg6, <>o__149.<>p__4.Target(<>o__149.<>p__4, (object)newProfileName), '-'), 1));
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 1; i <= 5; i++)
			{
				if (text3 == ResolvedDelegates.System_Object_ToString__1(ResolvedDelegates.System_String_Concat__29("Profile", ResolvedDelegates.System_Int32_ToString__55(ref i))))
				{
					dictionary.Add(ResolvedDelegates.System_String_Concat__29("Profile", ResolvedDelegates.System_Int32_ToString__55(ref i)), text4);
				}
				else
				{
					dictionary.Add(ResolvedDelegates.System_String_Concat__29("Profile", ResolvedDelegates.System_Int32_ToString__55(ref i)), "");
				}
			}
			if (ResolvedDelegates.System_IO_File_Exists__72(text2))
			{
				object obj = JsonConvert.DeserializeObject(ResolvedDelegates.System_IO_File_ReadAllText__234(text2));
				if (<>o__149.<>p__8 == null)
				{
					<>o__149.<>p__8 = CallSite<Func<CallSite, object, string, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.SetIndex(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[3]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
					}));
				}
				<>o__149.<>p__8.Target(<>o__149.<>p__8, obj, text3, text4);
				if (<>o__149.<>p__10 == null)
				{
					<>o__149.<>p__10 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(string).TypeHandle), ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle)));
				}
				Func<CallSite, object, string> target7 = <>o__149.<>p__10.Target;
				CallSite<Func<CallSite, object, string>> arg7 = <>o__149.<>p__10;
				if (<>o__149.<>p__9 == null)
				{
					<>o__149.<>p__9 = CallSite<Func<CallSite, Type, object, Formatting, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "SerializeObject", null, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(MyFanManager_RamFan1p5).TypeHandle), new CSharpArgumentInfo[3]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				string text5 = target7(arg7, <>o__149.<>p__9.Target(<>o__149.<>p__9, ResolvedDelegates.System_Type_GetTypeFromHandle__32(typeof(JsonConvert).TypeHandle), obj, (Formatting)1));
				ResolvedDelegates.System_IO_File_WriteAllText__236(text2, text5);
			}
			else
			{
				string text6 = JsonConvert.SerializeObject((object)dictionary, (Formatting)1);
				ResolvedDelegates.System_IO_File_WriteAllText__236(text2, text6);
			}
		}
		catch (Exception ex)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Exception", ResolvedDelegates.System_Exception_get_Message__58(ex)), "WriteCustomOSDstringToJson", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3292);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void syncBiosSettings()
	{
		NvramVariable.SetFwVars("ApExistFlag", 1);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("ApVersionCheck: ", ResolvedDelegates.System_Byte_ToString__119(ref NvramVariableInfo.APVersionCheck)), "syncBiosSettings", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3299);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Format__7("Current Mode: {0}", m_Option.OperatingMode), "syncBiosSettings", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3300);
		if (m_Option.OperatingMode == 3)
		{
			return;
		}
		if (NvramVariableInfo.APVersionCheck >= 24)
		{
			uint num = GetFanMode();
			if (isFirstLaunch)
			{
				num = m_DefaultMode;
			}
			isAc = EcCtrl.GetRoundSocketStatusFromEC() == 1;
			if (m_Option.OperatingMode != num)
			{
				LogCtrl.TraceMessage("Change local Power Mode.", "syncBiosSettings", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3315);
				m_Option.OperatingMode = num;
				WriteMainOptionToJson("OperatingMode", m_Option.OperatingMode);
			}
		}
		else if (NvramVariableInfo.APVersionCheck > 0)
		{
			uint num2 = ResolvedDelegates.System_Convert_ToUInt32__161(NvramVariable.GetFwVars().PowerMode);
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("PowerMode = 0x", ResolvedDelegates.System_UInt32_ToString__539(ref num2, "X2")), "syncBiosSettings", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3326);
			if (num2 != 255)
			{
				if (m_Option.OperatingMode != num2)
				{
					LogCtrl.TraceMessage("Change local Power Mode.", "syncBiosSettings", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3331);
					m_Option.OperatingMode = num2;
					WriteMainOptionToJson("OperatingMode", m_Option.OperatingMode);
				}
			}
			else
			{
				LogCtrl.TraceMessage("NvramVariable Power Mode = 0XFF, then change to default Power Mode.", "syncBiosSettings", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3338);
				m_Option.OperatingMode = m_DefaultMode;
				WriteMainOptionToJson("OperatingMode", m_Option.OperatingMode);
			}
		}
		string text = "performance mode is: ";
		uint operatingMode = m_Option.OperatingMode;
		LogCtrl.TraceMessageProgramData(text + ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode), "syncBiosSettings", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3344);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Init(bool GpuTesting = true, bool isManual = true)
	{
		if (m_OcSupport)
		{
			if (GpuTesting)
			{
				DetectRecoveryFlagFromBiosVariable();
			}
			if (m_GpuWhisperModeSupport)
			{
				UpdateWhisperStatus();
			}
		}
		string text = "OperatingMode = ";
		uint operatingMode = m_Option.OperatingMode;
		LogCtrl.TraceMessage(text + ResolvedDelegates.System_UInt32_ToString__78(ref operatingMode) + ", " + OperatingModeString(m_Option.OperatingMode), "Init", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3379);
		RefreshCurrentProfile(m_Option.OperatingMode, save: false);
		SetUserProfile(m_Option.OperatingMode, isManual);
		if (m_Option.OperatingMode != 0 && m_Option.OperatingMode != 1 && m_Option.OperatingMode != 2)
		{
			cGpuFeatures.SetGpuDynamicBoostFunCtrlEnable(1);
			cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(1);
		}
		else
		{
			cGpuFeatures.SetGpuDynamicBoostFunCtrlEnable(0);
			cGpuFeatures.SetGpuDynamicBoostEnable(0);
			cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(0);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void RefreshCurrentProfile(uint mode, bool save = true)
	{
		switch (mode)
		{
		case 1u:
			switch (m_Option.GamingProfileIndex)
			{
			case 0u:
				currentProfile = Mode1_Profile1;
				break;
			case 1u:
				currentProfile = Mode1_Profile2;
				break;
			}
			break;
		case 0u:
			switch (m_Option.OfficeProfileIndex)
			{
			case 0u:
				currentProfile = Mode2_Profile1;
				break;
			case 1u:
				currentProfile = Mode2_Profile2;
				break;
			}
			break;
		case 2u:
			switch (m_Option.TurboProfileIndex)
			{
			case 0u:
				currentProfile = Mode3_Profile1;
				break;
			case 1u:
				currentProfile = Mode3_Profile2;
				break;
			}
			break;
		case 3u:
			if (CustomizeCtrl.GetCustomId() == 9 && m_Option.CustomProfileIndex > 2)
			{
				m_Option.CustomProfileIndex = 0u;
			}
			switch (m_Option.CustomProfileIndex)
			{
			case 0u:
				currentProfile = Mode4_Profile1;
				break;
			case 1u:
				currentProfile = Mode4_Profile2;
				break;
			case 2u:
				currentProfile = Mode4_Profile3;
				break;
			case 3u:
				currentProfile = Mode4_Profile4;
				break;
			case 4u:
				currentProfile = Mode4_Profile5;
				break;
			}
			break;
		}
		if (save)
		{
			WriteProfileToJson(currentProfile);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void CheckInvalidParameter()
	{
		if (currentProfile.CPU.PL1 > currentProfile.CPU.PL2)
		{
			currentProfile.CPU.PL2 = currentProfile.CPU.PL1;
		}
		if (currentProfile.CPU.AmdSPL > currentProfile.CPU.AmdSPPT)
		{
			currentProfile.CPU.AmdSPPT = currentProfile.CPU.AmdSPL;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void RestoreCurrentProfile(uint mode)
	{
		switch (mode)
		{
		case 1u:
			switch (m_Option.GamingProfileIndex)
			{
			case 0u:
				RefreshMode(ref Mode1_Profile1, "Mode1_Profile1", "1", GamingModePLDefaultValue, "M1T1");
				CreateModeProfiles(Mode1_Profile1);
				currentProfile = Mode1_Profile1;
				break;
			case 1u:
				RefreshMode(ref Mode1_Profile2, "Mode1_Profile2", "2", GamingModePLDefaultValue, "M1T2");
				CreateModeProfiles(Mode1_Profile2);
				currentProfile = Mode1_Profile2;
				break;
			}
			break;
		case 0u:
			switch (m_Option.OfficeProfileIndex)
			{
			case 0u:
				RefreshMode(ref Mode2_Profile1, "Mode2_Profile1", "1", OfficeModePLDefaultValue, "M2T1");
				CreateModeProfiles(Mode2_Profile1);
				currentProfile = Mode2_Profile1;
				break;
			case 1u:
				RefreshMode(ref Mode2_Profile2, "Mode2_Profile2", "2", OfficeModePLDefaultValue, "M2T2");
				CreateModeProfiles(Mode2_Profile2);
				currentProfile = Mode2_Profile2;
				break;
			}
			break;
		case 2u:
			switch (m_Option.TurboProfileIndex)
			{
			case 0u:
				RefreshMode(ref Mode3_Profile1, "Mode3_Profile1", "1", TurboModePLDefaultValue, "M3T1");
				CreateModeProfiles(Mode3_Profile1);
				currentProfile = Mode3_Profile1;
				break;
			case 1u:
				RefreshMode(ref Mode3_Profile2, "Mode3_Profile2", "2", TurboModePLDefaultValue, "M3T2");
				CreateModeProfiles(Mode3_Profile2);
				currentProfile = Mode3_Profile2;
				break;
			}
			break;
		case 3u:
		{
			PLDefaultValue gamingModePLDefaultValue = GamingModePLDefaultValue;
			switch (m_Option.CustomProfileIndex)
			{
			case 0u:
				RefreshMode(ref Mode4_Profile1, "Mode4_Profile1", "1", gamingModePLDefaultValue, "M4T1");
				CreateModeProfiles(Mode4_Profile1);
				currentProfile = Mode4_Profile1;
				break;
			case 1u:
				RefreshMode(ref Mode4_Profile2, "Mode4_Profile2", "2", gamingModePLDefaultValue, "M4T2");
				CreateModeProfiles(Mode4_Profile2);
				currentProfile = Mode4_Profile2;
				break;
			case 2u:
				RefreshMode(ref Mode4_Profile3, "Mode4_Profile3", "3", gamingModePLDefaultValue, "M4T3");
				CreateModeProfiles(Mode4_Profile3);
				currentProfile = Mode4_Profile3;
				break;
			case 3u:
				RefreshMode(ref Mode4_Profile4, "Mode4_Profile4", "4", gamingModePLDefaultValue, "M4T4");
				CreateModeProfiles(Mode4_Profile4);
				currentProfile = Mode4_Profile4;
				break;
			case 4u:
				RefreshMode(ref Mode4_Profile5, "Mode4_Profile5", "5", gamingModePLDefaultValue, "M4T5");
				CreateModeProfiles(Mode4_Profile5);
				currentProfile = Mode4_Profile5;
				break;
			}
			break;
		}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void UserSet_HwOCMode(int CoreFreqOffset, int MemFreqOffset)
	{
		if (CoreFreqOffset > 0 && MemFreqOffset > 0)
		{
			HWOCMode = true;
			uint fanMode = 2u;
			SetFanMode(fanMode);
			int value = m_GpuConfigurableTGPMaximum + _SmartApcTable.DynamicBoost;
			LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("Set Ctgp : ", ResolvedDelegates.System_Int32_ToString__55(ref value)));
			cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(1);
			cGpuFeatures.SetGpuConfigurableTGPTarget(value, _SmartApcTable.DefaultTGP);
			cGpuFeatures.SetGpuDynamicBoostEnable(0);
			cGpuFeatures.SetGpuCoreFreqOffsetValue(currentProfile.GPU.CoreClockOffsetOC);
			cGpuFeatures.SetGpuMemFreqOffsetValue(currentProfile.GPU.MemoryClockOffsetOC);
		}
		else
		{
			HWOCMode = false;
			uint fanMode2 = 3u;
			SetFanMode(fanMode2);
			cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(currentProfile.GPU.ConfigurableTGPSwitch);
			cGpuFeatures.SetGpuConfigurableTGPTarget(currentProfile.GPU.ConfigurableTGPTarget, _SmartApcTable.DefaultTGP);
			cGpuFeatures.SetGpuDynamicBoostEnable(currentProfile.GPU.DynamicBoostSwitch);
			cGpuFeatures.SetGpuCoreFreqOffsetValue(currentProfile.GPU.CoreClockOffset);
			cGpuFeatures.SetGpuMemFreqOffsetValue(currentProfile.GPU.MemoryClockOffset);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void HWOCSetCoreMem(int CoreFreqOffset, int MemFreqOffset)
	{
		if (!m_IsNvGpu)
		{
			return;
		}
		if (CoreFreqOffset == 0 && MemFreqOffset == 0)
		{
			if (m_Option.OperatingMode == 2 && NvramVariableInfo.APVersionCheck >= 24 && !IsUMA())
			{
				if (m_IsTurboSubModeSupport && m_IsTurboOCAdjustSupport > 0 && ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "SilentPerformanceModeSwitch", 0)), "0"))
				{
					cGpuFeatures.SetGpuCoreFreqOffsetValue(m_Option.TurboSilentGPUOCOffset);
					LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("HWOCSetCoreMem TurboSilentGPUOCOffset wrtireI: ", ResolvedDelegates.System_Int32_ToString__55(ref m_Option.TurboSilentGPUOCOffset)), "HWOCSetCoreMem", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3736);
				}
				else
				{
					cGpuFeatures.SetGpuCoreFreqOffsetValue(m_Option.TurboGPUOCOffset);
					LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("HWOCSetCoreMem TurboGPUOCOffset wrtireI: ", ResolvedDelegates.System_Int32_ToString__55(ref m_Option.TurboGPUOCOffset)), "HWOCSetCoreMem", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3741);
				}
			}
			else
			{
				cGpuFeatures.SetGpuCoreFreqOffsetValue(currentProfile.GPU.CoreClockOffset);
			}
			cGpuFeatures.SetGpuMemFreqOffsetValue(currentProfile.GPU.MemoryClockOffset);
			return;
		}
		if (m_Option.OperatingMode == 2 && !IsUMA())
		{
			if (m_IsTurboSubModeSupport && m_IsTurboOCAdjustSupport > 0 && ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "SilentPerformanceModeSwitch", 0)), "0"))
			{
				cGpuFeatures.SetGpuCoreFreqOffsetValue(m_Option.TurboSilentGPUOCOffset);
				LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("HWOCSetCoreMem TurboSilentGPUOCOffset wrtireII: ", ResolvedDelegates.System_Int32_ToString__55(ref m_Option.TurboSilentGPUOCOffset)), "HWOCSetCoreMem", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3760);
			}
			else
			{
				cGpuFeatures.SetGpuCoreFreqOffsetValue(m_Option.TurboGPUOCOffset);
				LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("HWOCSetCoreMem TurboGPUOCOffset wrtireII: ", ResolvedDelegates.System_Int32_ToString__55(ref m_Option.TurboGPUOCOffset)), "HWOCSetCoreMem", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3765);
			}
		}
		else
		{
			cGpuFeatures.SetGpuCoreFreqOffsetValue(CoreFreqOffset);
		}
		cGpuFeatures.SetGpuMemFreqOffsetValue(MemFreqOffset);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void RefreshCurrentProfile()
	{
		LoadProfileAll();
		RefreshCurrentProfile(m_Option.OperatingMode, save: false);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe void SetUserProfile(uint mode, bool isManual = true, bool supportGPU = true)
	{
		LogCtrl.Write(ResolvedDelegates.System_String_Concat__29("#######################I'm set mode : ", ResolvedDelegates.System_UInt32_ToString__78(ref mode)));
		RegistryCtrl.RegistrySoftwareKeyWrite(RegistryHive.LocalMachine, m_sRegPath, "FanMode", mode, RegistryValueKind.DWord);
		if (isManual)
		{
			SetFanMode(mode);
		}
		UpdateStatusToTray();
		m_FanSafetyProtect = GetFanSafetyProtectStatus();
		if (m_FanSafetyProtect == 1)
		{
			m_Option.FanSafetyProtectNotify = 1u;
			switch (mode)
			{
			case 1u:
				FanTable.SetFanTable("DefaultFanTable_Gaming");
				break;
			case 0u:
				FanTable.SetFanTable("DefaultFanTable_Office");
				break;
			case 2u:
				FanTable.SetFanTable("DefaultFanTable_Turbo");
				break;
			case 3u:
				FanTable.SetFanTable("DefaultFanTable_Gaming");
				break;
			}
		}
		else
		{
			m_Option.FanSafetyProtectNotify = 0u;
			FanTable.SetFanTable(currentProfile.FAN.TableName);
		}
		WriteMainOptionToJson("FanSafetyProtectNotify", m_Option.FanSafetyProtectNotify);
		if (m_OcSupport)
		{
			if (m_IsAMDPlatform)
			{
				if (mode == 3)
				{
					SetPL1Value(currentProfile.CPU.AmdSPL);
					SetPL2Value(currentProfile.CPU.AmdSPPT);
					SetPL4Value(currentProfile.CPU.AmdFPPT);
					if (currentProfile.CPU.AmdSPL >= 75 || currentProfile.CPU.AmdSPPT >= 75)
					{
						SetCpuVrmCurrentLimit(65, 120);
					}
				}
				else
				{
					SetPL1Value(0);
					SetPL2Value(0);
					SetPL4Value(0);
				}
				if (currentProfile.CPU.TccOffsetSwitch == 1 && mode == 3)
				{
					SetCpuTccOffset(currentProfile.CPU.AmdTccTarget, bApExist: true);
				}
				else if (currentProfile.CPU.TccOffsetSwitch == 0 && mode == 3)
				{
					SetCpuTccOffset(0, bApExist: false);
				}
				if (NvramVariableInfo.ACpuOverClockSupport == 1)
				{
					SetCpuAmdCoreFreq(currentProfile.CPU.AmdCoreFreq);
					SetCpuAmdCoreVoltage(currentProfile.CPU.AmdCoreVoltage);
				}
			}
			else
			{
				if (mode == 3)
				{
					SetPL1Value(currentProfile.CPU.PL1);
					SetPL2Value(currentProfile.CPU.PL2);
					SetPL4Value(currentProfile.CPU.PL4);
				}
				else
				{
					SetPL1Value(0);
					SetPL2Value(0);
					SetPL4Value(0);
				}
				if (supportGPU)
				{
					SetCpuCoreVoltageOffset(currentProfile.CPU.OffsetCoreVoltage);
				}
				if (currentProfile.CPU.TccOffsetSwitch == 1 && mode == 3)
				{
					SetCpuTccOffset(currentProfile.CPU.TccOffset, bApExist: true);
				}
				else if (currentProfile.CPU.TccOffsetSwitch == 0 && mode == 3)
				{
					SetCpuTccOffset(0, bApExist: false);
				}
			}
			if (m_IsNvGpu)
			{
				if (supportGPU)
				{
					LogCtrl.TraceMessage(ResolvedDelegates.System_String_Format__7("currentProfile.OverClockingEnabled = {0}", currentProfile.OverClockingEnabled), "SetUserProfile", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3943);
					if (currentProfile.OverClockingEnabled == 1)
					{
						LoadOverClockingValue(currentProfile.OverClockingEnabled);
					}
					else
					{
						if (m_Option.OperatingMode == 2 && NvramVariableInfo.APVersionCheck >= 24 && !IsUMA())
						{
							if (m_IsTurboSubModeSupport && m_IsTurboOCAdjustSupport > 0 && ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_Object_ToString__1(RegistryCtrl.RegistrySoftwareKeyRead_NoThrow(RegistryHive.LocalMachine, ResolvedDelegates.System_String_Concat__29(m_sRegPath, "\\ItemSupport"), "SilentPerformanceModeSwitch", 0)), "0"))
							{
								cGpuFeatures.SetGpuCoreFreqOffsetValue(m_Option.TurboSilentGPUOCOffset);
								LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("HWOCSetCoreMem TurboSilentGPUOCOffset wrtireIII: ", ResolvedDelegates.System_Int32_ToString__55(ref m_Option.TurboSilentGPUOCOffset)), "SetUserProfile", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3955);
							}
							else
							{
								cGpuFeatures.SetGpuCoreFreqOffsetValue(m_Option.TurboGPUOCOffset);
								LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("HWOCSetCoreMem TurboGPUOCOffset wrtireIII: ", ResolvedDelegates.System_Int32_ToString__55(ref m_Option.TurboGPUOCOffset)), "SetUserProfile", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 3960);
							}
						}
						else
						{
							cGpuFeatures.SetGpuCoreFreqOffsetValue(currentProfile.GPU.CoreClockOffset);
						}
						cGpuFeatures.SetGpuMemFreqOffsetValue(currentProfile.GPU.MemoryClockOffset);
						cGpuFeatures.SetGpuTargetTemperature(currentProfile.GPU.TargetTemperature);
					}
					if (currentProfile.Name.Substring(0, 5) == "Mode3")
					{
						LiquidHWOC.Instance.RamFan1P5SetMode(2);
					}
				}
				if (PowerModeEvent.g_ACLineStatus == 1)
				{
					if (m_GpuConfigurableTGPSupport)
					{
						if (currentProfile.GPU.ConfigurableTGPTarget > m_GpuConfigurableTGPMaximum)
						{
							cGpuFeatures.SetGpuConfigurableTGPTarget(m_GpuConfigurableTGPMaximum, _SmartApcTable.DefaultTGP);
						}
						else
						{
							cGpuFeatures.SetGpuConfigurableTGPTarget(currentProfile.GPU.ConfigurableTGPTarget, _SmartApcTable.DefaultTGP);
						}
					}
					if (m_GpuDynamicBoostSupport)
					{
						if (m_Option.OperatingMode != 0 && m_Option.OperatingMode != 1 && m_Option.OperatingMode != 2)
						{
							if (ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_String_Substring__109(ref *(string*)currentProfile.Name, 0, 5), "Mode3"))
							{
								EnableGpuDynamicBoost(1);
							}
							else
							{
								SetGpuDynamicBoostSwitch(currentProfile.GPU.DynamicBoostSwitch);
							}
							cGpuFeatures.SetGpuDynamicBoostFunCtrlEnable(1);
						}
					}
					else
					{
						SetGpuDynamicBoostSwitch(0);
						cGpuFeatures.SetGpuDynamicBoostFunCtrlEnable(0);
					}
					if (supportGPU)
					{
						if (mode == 0 && CustomizeCtrl.GetCustomId() == 42)
						{
							SetGpuWhisperModeSwitchOnOFF(currentProfile.GPU.WhisperModeSwitch);
						}
						else
						{
							SetGpuWhisperModeSwitchOnOFF(0);
						}
					}
					if (currentProfile.OverClockingEnabled == 1)
					{
						cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(_SmartApcTable.DynamicBoost);
					}
				}
				else if (supportGPU)
				{
					SetGpuWhisperModeSwitchOnOFF(0);
				}
			}
			SetFanSwitchSpeedEnabled(currentProfile.FAN.FanSwitchSpeedEnabled);
			if (NvramVariableInfo.MemoryOverClockSupport == 1 && supportGPU)
			{
				if (NvramVariableInfo.APVersionCheck >= 23)
				{
					SetMemoryOverClockSwitch(currentProfile.CPU.PerformanceAndOverClockMenuSwitch);
				}
				else
				{
					SetMemoryOverClockSwitch(currentProfile.MEM.MemoryOverClockSwitch);
				}
			}
		}
		if (mode == 1)
		{
			UpdateSmartBalanceSupport();
			if (SmartBalanceSupport)
			{
				UserSetSmartBalance(currentProfile.SmartBalance);
			}
		}
		else
		{
			ClearSmartBalanceEnable();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetGpuDynamicBoostSwitch(int status)
	{
		if (currentProfile.OverClockingEnabled == 1)
		{
			cGpuFeatures.SetGpuDynamicBoostTotalProcessingPowerTarget(currentProfile.GPU.DynamicBoostTotalProcessingPowerTarget);
			cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(currentProfile.GPU.DynamicBoost);
			cGpuFeatures.SetGpuDynamicBoostEnable(1);
		}
		else if (status == 1)
		{
			cGpuFeatures.SetGpuDynamicBoostTotalProcessingPowerTarget(currentProfile.GPU.DynamicBoostTotalProcessingPowerTarget);
			cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(currentProfile.GPU.DynamicBoost);
			cGpuFeatures.SetGpuDynamicBoostEnable(status);
		}
		else
		{
			cGpuFeatures.SetGpuDynamicBoostTotalProcessingPowerTarget(0);
			cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(0);
			cGpuFeatures.SetGpuDynamicBoostEnable(status);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void EnableGpuDynamicBoost(int status)
	{
		if (status == 1)
		{
			cGpuFeatures.SetGpuDynamicBoostTotalProcessingPowerTarget(currentProfile.GPU.DynamicBoostTotalProcessingPowerTarget);
			cGpuFeatures.SetGpuDynamicBoostEnable(status);
		}
		else
		{
			cGpuFeatures.SetGpuDynamicBoostTotalProcessingPowerTarget(0);
			cGpuFeatures.SetGpuDynamicBoostEnable(status);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetGpuWhisperModeSwitchOnOFF(int status)
	{
		if (!m_GpuWhisperModeSupport)
		{
			return;
		}
		if (status == 1)
		{
			cGpuFeatures.SetGpuWhisperModeStatus(status: true);
			cGpuFeatures.SetGpuWhisperModeSettingValue(currentProfile.GPU.WhisperModeSetting);
			switch (currentProfile.GPU.WhisperModeSetting)
			{
			case 0:
				cGpuFeatures.SetGpuWhisperModeMinFpsValue(currentProfile.GPU.WhisperModeMinFps_QUIETER);
				break;
			case 1:
				cGpuFeatures.SetGpuWhisperModeMinFpsValue(currentProfile.GPU.WhisperModeMinFps_QUIET);
				break;
			case 2:
				cGpuFeatures.SetGpuWhisperModeMinFpsValue(currentProfile.GPU.WhisperModeMinFps_BALANCED);
				break;
			}
			LogCtrl.TraceMessage("SetGpuWhisperModeStatus: " + ResolvedDelegates.System_Int32_ToString__55(ref status) + ", SetGpuWhisperModeSettingValue: " + ResolvedDelegates.System_Int32_ToString__55(ref currentProfile.GPU.WhisperModeSetting), "SetGpuWhisperModeSwitchOnOFF", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4254);
		}
		else
		{
			cGpuFeatures.SetGpuWhisperModeStatus(status: false);
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("SetGpuWhisperModeStatus: ", ResolvedDelegates.System_Int32_ToString__55(ref status)), "SetGpuWhisperModeSwitchOnOFF", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4261);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetFanSwitchSpeedEnabled(int status)
	{
		if (status == 1)
		{
			SetFanSwitchSpeed(currentProfile.FAN.FanSwitchSpeed, bApExist: true);
		}
		else
		{
			SetFanSwitchSpeed(0, bApExist: false);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetFanMode(uint mode)
	{
		switch (mode)
		{
		case 1u:
			if (m_Option.FanBoostEnable == 1)
			{
				BitArray bitArray3 = new BitArray(new byte[1]);
				ResolvedDelegates.System_Collections_BitArray_set_Item__446(bitArray3, 6, true);
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, ConvertToByte(bitArray3));
				object obj4 = ResolvedDelegates.System_String_Concat__29;
				string text4 = "performance mode write: ";
				byte b2 = ConvertToByte(bitArray3);
				LogCtrl.TraceMessageProgramData(obj4(text4, ResolvedDelegates.System_Byte_ToString__119(ref b2)), "SetFanMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4305);
			}
			else
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, 0);
				LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("performance mode write: ", ECSpec.MyFanCTLByteFlag.Normal_Mode.ToString()), "SetFanMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4310);
			}
			FanTable.CustomerModeLightOff();
			break;
		case 0u:
			if (m_Option.FanBoostEnable == 1)
			{
				BitArray bitArray2 = new BitArray(new byte[1] { 160 });
				ResolvedDelegates.System_Collections_BitArray_set_Item__446(bitArray2, 6, true);
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, ConvertToByte(bitArray2));
				object obj3 = ResolvedDelegates.System_String_Concat__29;
				string text3 = "performance mode write: ";
				byte b2 = ConvertToByte(bitArray2);
				LogCtrl.TraceMessageProgramData(obj3(text3, ResolvedDelegates.System_Byte_ToString__119(ref b2)), "SetFanMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4324);
			}
			else
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, 160);
				LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("performance mode write: ", ECSpec.MyFanCTLByteFlag.User_Fan_HiMode.ToString()), "SetFanMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4329);
			}
			FanTable.CustomerModeLightOff();
			break;
		case 2u:
			if (m_Option.FanBoostEnable == 1)
			{
				BitArray bitArray4 = new BitArray(new byte[1] { 16 });
				ResolvedDelegates.System_Collections_BitArray_set_Item__446(bitArray4, 6, true);
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, ConvertToByte(bitArray4));
				object obj5 = ResolvedDelegates.System_String_Concat__29;
				string text5 = "performance mode write: ";
				byte b2 = ConvertToByte(bitArray4);
				LogCtrl.TraceMessageProgramData(obj5(text5, ResolvedDelegates.System_Byte_ToString__119(ref b2)), "SetFanMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4345);
			}
			else
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, 16);
				LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("performance mode write: ", ECSpec.MyFanCTLByteFlag.Turbo_Mode.ToString()), "SetFanMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4350);
			}
			FanTable.CustomerModeLightOff();
			break;
		case 3u:
		{
			byte b = 0;
			FanTable.CustomerModeLightOn();
			if (m_Option.FanBoostEnable == 1)
			{
				BitArray bitArray = new BitArray(new byte[1] { b });
				ResolvedDelegates.System_Collections_BitArray_set_Item__446(bitArray, 6, true);
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, ConvertToByte(bitArray));
				object obj = ResolvedDelegates.System_String_Concat__29;
				string text = "performance mode write: ";
				byte b2 = ConvertToByte(bitArray);
				LogCtrl.TraceMessageProgramData(obj(text, ResolvedDelegates.System_Byte_ToString__119(ref b2)), "SetFanMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4373);
			}
			else
			{
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, b);
				object obj2 = ResolvedDelegates.System_String_Concat__29;
				string text2 = "performance mode write: ";
				ushort num = 1873;
				LogCtrl.TraceMessageProgramData(obj2(text2, ResolvedDelegates.System_UInt16_ToString__169(ref num)), "SetFanMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4378);
			}
			break;
		}
		}
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1830, ref Data);
		Data = ((mode != 3) ? ((byte)(Data & 0x7F)) : ((byte)(Data | 0x80)));
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1830, Data);
		if (isFirstLaunch)
		{
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2024, 0);
		}
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
	private void SetFanBoost(uint status)
	{
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, ref Data);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("performance mode is: ", ResolvedDelegates.System_Byte_ToString__119(ref Data)), "SetFanBoost", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4411);
		byte b = (byte)ResolvedDelegates.System_Convert_ToUInt64__129(Data);
		b = ((status != 1) ? ((byte)(b & 0xBF)) : ((byte)(b | 0x40)));
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, b);
		LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("performance mode write: ", ResolvedDelegates.System_Byte_ToString__119(ref b)), "SetFanBoost", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4421);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public uint GetFanMode()
	{
		uint result = 1u;
		byte Data = byte.MaxValue;
		byte Data2 = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1831, ref Data2);
		if ((int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data2) & 0x40) == 64)
		{
			result = 3u;
		}
		else
		{
			EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1873, ref Data);
			byte data = (byte)ResolvedDelegates.System_Convert_ToUInt64__129(Data);
			int bitFromByte = UtilityExtensions.GetBitFromByte(data, 4);
			int bitFromByte2 = UtilityExtensions.GetBitFromByte(data, 7);
			if (bitFromByte == 0 && bitFromByte2 == 0)
			{
				result = 1u;
			}
			else if (bitFromByte == 0 && bitFromByte2 == 1)
			{
				result = 0u;
			}
			else if (bitFromByte == 1 && bitFromByte2 == 0)
			{
				result = 2u;
			}
			LogCtrl.TraceMessage(string.Format("EC 0751 bit4[{0}], bit7[{1}]", bitFromByte, bitFromByte2), "GetFanMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4449);
		}
		if (isFirstLaunch)
		{
			result = m_DefaultMode;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetCpuVrmCurrentLimit(int limit, int maximum)
	{
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1875, (byte)limit);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1876, (byte)maximum);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetPL1Value(int value)
	{
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1923, (byte)value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetPL2Value(int value)
	{
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1924, (byte)value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetPL4Value(int value)
	{
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1925, (byte)value);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void IsSupportCPUPL4DoubleFlag()
	{
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1831, ref Data);
		if ((byte)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0x80) == 128)
		{
			g_SupportCPUDoubleFlag = 1;
		}
		else
		{
			g_SupportCPUDoubleFlag = 0;
		}
		LogCtrl.Write("MyFanManager_RamFan1p5[IsSupportCPUPL4DoubleFlag] g_SupportCPUDoubleFlag = " + ResolvedDelegates.System_Int32_ToString__55(ref g_SupportCPUDoubleFlag));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetCpuTccOffset(int value, bool bApExist)
	{
		byte b = 0;
		if (bApExist)
		{
			b = (byte)(value | 0x80);
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1926, b);
		}
		else
		{
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1926, 0);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetFanSwitchSpeed(int value, bool bApExist)
	{
		byte b = 0;
		if (bApExist)
		{
			int num = value / 100;
			if (num > 0)
			{
				b = (byte)(num | 0x80);
				EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1927, b);
			}
		}
		else
		{
			EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1927, 0);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetGPUdstate(uint dstate)
	{
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1931, ref Data);
		ulong num = ResolvedDelegates.System_Convert_ToUInt64__129(Data);
		num &= 0xF8;
		num |= dstate;
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1931, (byte)num);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Set D-State to: ", ResolvedDelegates.System_UInt32_ToString__78(ref dstate)), "SetGPUdstate", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4581);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetGPUdstateByGpuMode(uint G)
	{
		uint num = 1u;
		switch (G)
		{
		case 1u:
		case 2u:
		case 4u:
			num = 1u;
			break;
		case 3u:
			num = 2u;
			break;
		}
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1931, ref Data);
		ulong num2 = ResolvedDelegates.System_Convert_ToUInt64__129(Data);
		num2 &= 0xF8;
		num2 |= num;
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1931, (byte)num2);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("Set D-State to: ", ResolvedDelegates.System_UInt32_ToString__78(ref num)), "SetGPUdstateByGpuMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4612);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private uint GetQkeyDefine()
	{
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1922, ref Data);
		if ((byte)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 2) == 2)
		{
			return 1u;
		}
		return 0u;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int GetOfficeModeFanTableType()
	{
		int num = 0;
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1922, ref Data);
		if ((byte)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 4) == 4)
		{
			return 2;
		}
		return 1;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private uint GetDefaultMode()
	{
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1922, ref Data);
		_ = (byte)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0x10);
		return 1u;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetPowerLedStatus(uint mode)
	{
		byte Data = 0;
		byte b = 252;
		byte b2 = 0;
		byte b3 = 0;
		switch (mode)
		{
		case 2u:
			b2 = 0;
			break;
		case 1u:
			b2 = 1;
			break;
		case 3u:
			b2 = 2;
			break;
		case 4u:
			b2 = 1;
			break;
		}
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1957, ref Data);
		b3 = (byte)((Data & b) + b2);
		object obj = ResolvedDelegates.System_String_Concat__79;
		string[] array = new string[6]
		{
			"Mode: ",
			ResolvedDelegates.System_UInt32_ToString__78(ref mode),
			", Set ECRAM 0x",
			null,
			null,
			null
		};
		ushort num = 1957;
		array[3] = ResolvedDelegates.System_UInt16_ToString__540(ref num, "X");
		array[4] = " = 0x";
		array[5] = ResolvedDelegates.System_Byte_ToString__173(ref b3, "X2");
		LogCtrl.TraceMessage(obj(array), "SetPowerLedStatus", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4693);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1957, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetFanQuietModeEnable(bool bEnable)
	{
		byte Data = 0;
		byte b = 251;
		byte b2 = 0;
		byte b3 = 0;
		b2 = (byte)(bEnable ? 4 : 0);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1957, ref Data);
		b3 = (byte)((Data & b) + b2);
		object obj = ResolvedDelegates.System_String_Concat__79;
		string[] array = new string[6]
		{
			"bEnable ",
			ResolvedDelegates.System_Boolean_ToString__147(ref bEnable),
			", Set ECRAM 0x",
			null,
			null,
			null
		};
		ushort num = 1957;
		array[3] = ResolvedDelegates.System_UInt16_ToString__540(ref num, "X");
		array[4] = " = 0x";
		array[5] = ResolvedDelegates.System_Byte_ToString__173(ref b3, "X2");
		LogCtrl.TraceMessage(obj(array), "SetFanQuietModeEnable", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4714);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1957, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int GetFanSafetyProtectStatus()
	{
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1957, ref Data);
		if (UtilityExtensions.GetBitFromByte(Data, 3) == 1)
		{
			return m_FanSafetyProtect = 1;
		}
		return m_FanSafetyProtect = 0;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetOverBoostMode(bool bEnable)
	{
		byte Data = 0;
		byte b = 239;
		byte b2 = 0;
		byte b3 = 0;
		b2 = (byte)(bEnable ? 16 : 0);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1957, ref Data);
		b3 = (byte)((Data & b) + b2);
		object obj = ResolvedDelegates.System_String_Concat__79;
		string[] array = new string[6]
		{
			"bEnable ",
			ResolvedDelegates.System_Boolean_ToString__147(ref bEnable),
			", Set ECRAM 0x",
			null,
			null,
			null
		};
		ushort num = 1957;
		array[3] = ResolvedDelegates.System_UInt16_ToString__540(ref num, "X");
		array[4] = " = 0x";
		array[5] = ResolvedDelegates.System_Byte_ToString__173(ref b3, "X2");
		LogCtrl.TraceMessage(obj(array), "SetOverBoostMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4751);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1957, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetPowerStatus(bool bHigh)
	{
		byte Data = 0;
		byte b = 127;
		byte b2 = 0;
		byte b3 = 0;
		b2 = (byte)(bHigh ? 128 : 0);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1957, ref Data);
		b3 = (byte)((Data & b) + b2);
		object obj = ResolvedDelegates.System_String_Concat__79;
		string[] array = new string[6]
		{
			"bHigh ",
			ResolvedDelegates.System_Boolean_ToString__147(ref bHigh),
			", Set ECRAM 0x",
			null,
			null,
			null
		};
		ushort num = 1957;
		array[3] = ResolvedDelegates.System_UInt16_ToString__540(ref num, "X");
		array[4] = " = 0x";
		array[5] = ResolvedDelegates.System_Byte_ToString__173(ref b3, "X2");
		LogCtrl.TraceMessage(obj(array), "SetPowerStatus", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4772);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1957, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetOverBoostByDynamicTemp(bool bEnable)
	{
		byte Data = 0;
		byte b = 253;
		byte b2 = 0;
		byte b3 = 0;
		b2 = (byte)((!bEnable) ? 2 : 0);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1958, ref Data);
		b3 = (byte)((Data & b) + b2);
		object obj = ResolvedDelegates.System_String_Concat__79;
		string[] array = new string[6]
		{
			"bEnable ",
			ResolvedDelegates.System_Boolean_ToString__147(ref bEnable),
			", Set ECRAM 0x",
			null,
			null,
			null
		};
		ushort num = 1958;
		array[3] = ResolvedDelegates.System_UInt16_ToString__540(ref num, "X");
		array[4] = " = 0x";
		array[5] = ResolvedDelegates.System_Byte_ToString__173(ref b3, "X2");
		LogCtrl.TraceMessage(obj(array), "SetOverBoostByDynamicTemp", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4795);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1958, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GetGamingPLDefaultValue(ref int PL1, ref int PL2, ref int PL4, ref int DState)
	{
		byte Data = 0;
		byte Data2 = 0;
		byte Data3 = 0;
		byte Data4 = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1840, ref Data);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1841, ref Data2);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1842, ref Data3);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1843, ref Data4);
		PL1 = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xFF);
		PL2 = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data2) & 0xFF);
		PL4 = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data3) & 0xFF);
		DState = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data4) & 0xFF);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__79(new string[8]
		{
			"PL1:",
			ResolvedDelegates.System_Int32_ToString__55(ref PL1),
			", PL2:",
			ResolvedDelegates.System_Int32_ToString__55(ref PL2),
			", PL4:",
			ResolvedDelegates.System_Int32_ToString__55(ref PL4),
			", DState:",
			ResolvedDelegates.System_Int32_ToString__55(ref DState)
		}), "GetGamingPLDefaultValue", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4815);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GetOfficePLDefaultValue(ref int PL1, ref int PL2, ref int PL4, ref int DState)
	{
		byte Data = 0;
		byte Data2 = 0;
		byte Data3 = 0;
		byte Data4 = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1844, ref Data);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1845, ref Data2);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1846, ref Data3);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1847, ref Data4);
		PL1 = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xFF);
		PL2 = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data2) & 0xFF);
		PL4 = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data3) & 0xFF);
		DState = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data4) & 0xFF);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__79(new string[8]
		{
			"PL1:",
			ResolvedDelegates.System_Int32_ToString__55(ref PL1),
			", PL2:",
			ResolvedDelegates.System_Int32_ToString__55(ref PL2),
			", PL4:",
			ResolvedDelegates.System_Int32_ToString__55(ref PL4),
			", DState:",
			ResolvedDelegates.System_Int32_ToString__55(ref DState)
		}), "GetOfficePLDefaultValue", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4832);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GetTurboPLDefaultValue(ref int PL1, ref int PL2, ref int PL4, ref int DState)
	{
		byte Data = 0;
		byte Data2 = 0;
		byte Data3 = 0;
		byte Data4 = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1959, ref Data);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1960, ref Data2);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1961, ref Data3);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1962, ref Data4);
		PL1 = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xFF);
		PL2 = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data2) & 0xFF);
		PL4 = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data3) & 0xFF);
		DState = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data4) & 0xFF);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__79(new string[8]
		{
			"PL1:",
			ResolvedDelegates.System_Int32_ToString__55(ref PL1),
			", PL2:",
			ResolvedDelegates.System_Int32_ToString__55(ref PL2),
			", PL4:",
			ResolvedDelegates.System_Int32_ToString__55(ref PL4),
			", DState:",
			ResolvedDelegates.System_Int32_ToString__55(ref DState)
		}), "GetTurboPLDefaultValue", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4849);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private int GetTauDefaultValue()
	{
		int num = 0;
		byte Data = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1848, ref Data);
		num = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xFF);
		if (num == 0)
		{
			num = 56;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GetTccDefaultValue(ref int gamingTcc, ref int officeTcc, ref int turboTcc)
	{
		byte Data = 0;
		byte Data2 = 0;
		byte Data3 = 0;
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2008, ref Data);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2009, ref Data2);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 2010, ref Data3);
		gamingTcc = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data) & 0xFF);
		officeTcc = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data2) & 0xFF);
		turboTcc = (int)(ResolvedDelegates.System_Convert_ToUInt64__129(Data3) & 0xFF);
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__79(new string[6]
		{
			"GamingTcc:",
			ResolvedDelegates.System_Int32_ToString__55(ref gamingTcc),
			", OfficeTcc:",
			ResolvedDelegates.System_Int32_ToString__55(ref officeTcc),
			", TurboTcc:",
			ResolvedDelegates.System_Int32_ToString__55(ref turboTcc)
		}), "GetTccDefaultValue", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4877);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SkipOfficeModeSafetyProtect(int status)
	{
		byte Data = 0;
		byte b = 239;
		byte b2 = 0;
		byte b3 = 0;
		b2 = (byte)((status == 1) ? 16 : 0);
		EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, ref Data);
		b3 = (byte)((Data & b) + b2);
		EcCtrl.Write(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1989, b3);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetCpuCoreVoltageOffset(int value)
	{
		if (value == 0)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__120("ICpuCoreVoltageOffsetValue: ", ResolvedDelegates.System_Int32_ToString__55(ref value), ", ICpuCoreVoltageOffsetNegativeValue: ", ResolvedDelegates.System_Int32_ToString__55(ref value)), "SetCpuCoreVoltageOffset", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4930);
			NvramVariable.SetFwVarsForModeSwitchChange("ICpuCoreVoltageOffsetValue", ResolvedDelegates.System_Convert_ToUInt16__691(value));
			NvramVariable.SetFwVarsForModeSwitchChange("ICpuCoreVoltageOffsetNegativeValue", ResolvedDelegates.System_Convert_ToUInt16__691(value));
		}
		else if (value > 0 && value <= CpuOffsetCoreVoltageMaximum)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__107("ICpuCoreVoltageOffsetValue: ", ResolvedDelegates.System_Int32_ToString__55(ref value), ", ICpuCoreVoltageOffsetNegativeValue: 0"), "SetCpuCoreVoltageOffset", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4942);
			NvramVariable.SetFwVarsForModeSwitchChange("ICpuCoreVoltageOffsetValue", ResolvedDelegates.System_Convert_ToUInt16__691(value));
			NvramVariable.SetFwVarsForModeSwitchChange("ICpuCoreVoltageOffsetNegativeValue", ResolvedDelegates.System_Convert_ToUInt16__691(0));
		}
		else if (value >= CpuOffsetCoreVoltageMinimum && value < 0)
		{
			object obj = ResolvedDelegates.System_String_Concat__29;
			string text = "ICpuCoreVoltageOffsetValue: 0, ICpuCoreVoltageOffsetNegativeValue: ";
			ushort num = ResolvedDelegates.System_Convert_ToUInt16__691(ResolvedDelegates.System_Math_Abs__746(value));
			LogCtrl.TraceMessage(obj(text, ResolvedDelegates.System_UInt16_ToString__169(ref num)), "SetCpuCoreVoltageOffset", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4952);
			NvramVariable.SetFwVarsForModeSwitchChange("ICpuCoreVoltageOffsetValue", ResolvedDelegates.System_Convert_ToUInt16__691(0));
			NvramVariable.SetFwVarsForModeSwitchChange("ICpuCoreVoltageOffsetNegativeValue", ResolvedDelegates.System_Convert_ToUInt16__691(ResolvedDelegates.System_Math_Abs__746(value)));
		}
		else
		{
			LogCtrl.TraceMessage("Set the value failed, and error is out of range.", "SetCpuCoreVoltageOffset", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4963);
			value = 0;
			NvramVariable.SetFwVarsForModeSwitchChange("ICpuCoreVoltageOffsetValue", ResolvedDelegates.System_Convert_ToUInt16__691(value));
			NvramVariable.SetFwVarsForModeSwitchChange("ICpuCoreVoltageOffsetNegativeValue", ResolvedDelegates.System_Convert_ToUInt16__691(value));
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetCpuAmdCoreFreq(int value)
	{
		if (value >= NvramVariableInfo.ACpuFreqValueMinimum && value <= NvramVariableInfo.ACpuFreqValueMaximum)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("ACpuFreqValue: ", ResolvedDelegates.System_Int32_ToString__55(ref value)), "SetCpuAmdCoreFreq", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4980);
			NvramVariable.SetFwVarsForModeSwitchChange("ACpuFreqValue", ResolvedDelegates.System_Convert_ToUInt32__143(value));
		}
		else
		{
			LogCtrl.TraceMessage("Set the value failed, and error is out of range.", "SetCpuAmdCoreFreq", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4988);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetCpuAmdCoreVoltage(int value)
	{
		if (value >= NvramVariableInfo.ACpuVoltageValueMinimum && value <= NvramVariableInfo.ACpuVoltageValueMaximum)
		{
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("ACpuVoltageValue: ", ResolvedDelegates.System_Int32_ToString__55(ref value)), "SetCpuAmdCoreVoltage", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 4995);
			NvramVariable.SetFwVarsForModeSwitchChange("ACpuVoltageValue", ResolvedDelegates.System_Convert_ToUInt32__143(value));
		}
		else
		{
			LogCtrl.TraceMessage("Set the value failed, and error is out of range.", "SetCpuAmdCoreVoltage", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5003);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void LoadNvramVariableInfo()
	{
		NVRAM_STRUCT fwVars = NvramVariable.GetFwVars();
		NvramVariableInfo.ICpuCoreVoltageOffsetRangeType = fwVars.ICpuCoreVoltageOffsetRangeType;
		NvramVariableInfo.CoreVoltageOffsetMaximum = fwVars.ICpuCoreVoltageOffsetMaximum;
		NvramVariableInfo.ICpuCoreVoltageOffsetSupport = fwVars.ICpuCoreVoltageOffsetSupport;
		NvramVariableInfo.ACpuOverClockSupport = fwVars.ACpuOverClockSupport;
		if (NvramVariableInfo.ACpuOverClockSupport == 1)
		{
			NvramVariableInfo.ACpuFreqValueMaximum = fwVars.ACpuFreqValueMaximum;
			NvramVariableInfo.ACpuFreqValueMinimum = fwVars.ACpuFreqValueMinimum;
			NvramVariableInfo.ACpuVoltageValueMaximum = fwVars.ACpuVoltageValueMaximum;
			NvramVariableInfo.ACpuVoltageValueMinimum = fwVars.ACpuVoltageValueMinimum;
		}
		NvramVariableInfo.OverClockRecoveryFlag = fwVars.OverClockRecoveryFlag;
		NvramVariableInfo.MemoryOverClockSupport = fwVars.MemoryOverClockSupport;
		NvramVariableInfo.ApUseFlag = fwVars.ApUseFlag;
		NvramVariableInfo.CPUPerformanceAndOverClockMenuSupport = fwVars.CPUPerformanceAndOverClockMenuSupport;
		NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch = fwVars.CPUPerformanceAndOverClockMenuSwitch;
		NvramVariableInfo.APVersionCheck = fwVars.APVersionCheck;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetMemoryOverClockSwitch(int value)
	{
		LogCtrl.TraceMessage(ResolvedDelegates.System_String_Concat__29("MemoryOverClockSwitch: ", ResolvedDelegates.System_Int32_ToString__55(ref value)), "SetMemoryOverClockSwitch", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5079);
		NvramVariable.SetFwVarsForModeSwitchChange("MemoryOverClockSwitch", ResolvedDelegates.System_Convert_ToByte__75(value));
	}

	private string OperatingModeString(uint mode)
	{
		return mode switch
		{
			0u => "Office mode", 
			1u => "Gaming mode", 
			2u => "Turbo mode", 
			_ => "", 
		};
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<SetFanTableThread>d__201))]
	private void SetFanTableThread(string _tableName)
	{
		<SetFanTableThread>d__201 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine._tableName = _tableName;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private uint TransToGlobalType(uint mode)
	{
		uint result = 1u;
		switch (mode)
		{
		case 0u:
			result = 1u;
			break;
		case 1u:
			result = 0u;
			break;
		case 2u:
			result = 2u;
			break;
		case 3u:
			result = 3u;
			break;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void DetectBsodFromEventLog()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected Obj, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected Obj, but got Unknown
		string text = "*[System/EventID=41]";
		DateTime dateTime = ResolvedDelegates.System_DateTime_get_Now__170();
		bool flag = false;
		EventLogReader val = new EventLogReader(new EventLogQuery("System", (PathType)1, text));
		for (EventRecord val2 = ResolvedDelegates.System_Diagnostics_Eventing_Reader_EventLogReader_ReadEvent__765(val); val2 != null; val2 = ResolvedDelegates.System_Diagnostics_Eventing_Reader_EventLogReader_ReadEvent__765(val))
		{
			dateTime = val2.TimeCreated.Value;
			flag = true;
		}
		if (flag && ResolvedDelegates.System_DateTime_op_GreaterThan__599(dateTime, m_Option.BsodTimestampRestored))
		{
			LogCtrl.TraceMessage("Detect the last BSOD TimeCreated > Timestamp, then restore OC settings.", "DetectBsodFromEventLog", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5188);
			m_Option.DefaultNotify = 1u;
			LoadDefault();
			FanTable.RestoreDefaultFanTableAll();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void DetectErrorFanTable()
	{
		LoadDefault();
		FanTable.RestoreDefaultFanTableAll();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void DetectRecoveryFlagFromBiosVariable()
	{
		if (NvramVariableInfo.OverClockRecoveryFlag == 1)
		{
			LogCtrl.TraceMessage("Detect BiosVariable OverClockRecoveryFlag = 1, then restore OC settings.", "DetectRecoveryFlagFromBiosVariable", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5209);
			m_Option.DefaultNotify = 1u;
			LoadDefault();
			FanTable.RestoreDefaultFanTableAll();
			NvramVariableInfo.OverClockRecoveryFlag = 0;
			NvramVariable.SetFwVars("OverClockRecoveryFlag", 0);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe void SetOperatingModeProfileIndex(string _modeProfileIndex)
	{
		if (CustomizeCtrl.GetCustomId() == 9 && ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M4P4"))
		{
			_modeProfileIndex = "M4P1";
		}
		if (_modeProfileIndex != null)
		{
			int num = ResolvedDelegates.System_String_get_Length__298(ref *(string*)_modeProfileIndex);
			if (num == 4)
			{
				switch (ResolvedDelegates.System_String_get_Chars__334(ref *(string*)_modeProfileIndex, 3))
				{
				case '1':
					if (!ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M1P1"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M2P1"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M3P1"))
							{
								if (ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M4P1"))
								{
									m_Option.CustomProfileIndex = 0u;
									WriteMainOptionToJson("CustomProfileIndex", m_Option.CustomProfileIndex);
									UserSet_Mode4();
								}
							}
							else
							{
								m_Option.TurboProfileIndex = 0u;
								WriteMainOptionToJson("TurboProfileIndex", m_Option.TurboProfileIndex);
								UserSet_Mode3();
							}
						}
						else
						{
							m_Option.OfficeProfileIndex = 0u;
							WriteMainOptionToJson("OfficeProfileIndex", m_Option.OfficeProfileIndex);
							UserSet_Mode2();
						}
					}
					else
					{
						m_Option.GamingProfileIndex = 0u;
						WriteMainOptionToJson("GamingProfileIndex", m_Option.GamingProfileIndex);
						UserSet_Mode1();
					}
					break;
				case '2':
					if (!ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M1P2"))
					{
						if (!ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M2P2"))
						{
							if (!ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M3P2"))
							{
								if (ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M4P2"))
								{
									m_Option.CustomProfileIndex = 1u;
									WriteMainOptionToJson("CustomProfileIndex", m_Option.CustomProfileIndex);
									UserSet_Mode4();
								}
							}
							else
							{
								m_Option.TurboProfileIndex = 1u;
								WriteMainOptionToJson("TurboProfileIndex", m_Option.TurboProfileIndex);
								UserSet_Mode3();
							}
						}
						else
						{
							m_Option.OfficeProfileIndex = 1u;
							WriteMainOptionToJson("OfficeProfileIndex", m_Option.OfficeProfileIndex);
							UserSet_Mode2();
						}
					}
					else
					{
						m_Option.GamingProfileIndex = 1u;
						WriteMainOptionToJson("GamingProfileIndex", m_Option.GamingProfileIndex);
						UserSet_Mode1();
					}
					break;
				case '3':
					if (ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M4P3"))
					{
						m_Option.CustomProfileIndex = 2u;
						WriteMainOptionToJson("CustomProfileIndex", m_Option.CustomProfileIndex);
						UserSet_Mode4();
					}
					break;
				case '4':
					if (ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M4P4"))
					{
						m_Option.CustomProfileIndex = 3u;
						WriteMainOptionToJson("CustomProfileIndex", m_Option.CustomProfileIndex);
						UserSet_Mode4();
					}
					break;
				case '5':
					if (ResolvedDelegates.System_String_op_Equality__18(_modeProfileIndex, "M4P5"))
					{
						m_Option.CustomProfileIndex = 4u;
						WriteMainOptionToJson("CustomProfileIndex", m_Option.CustomProfileIndex);
						UserSet_Mode4();
					}
					break;
				}
			}
		}
		UpdateStatusToClient(1, 0, 0, updateOperationMode: false);
		MySettingManager.CreateInstance.SetHighPerformancePowerModeScancode(-1);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<SetOperatingModeProfileIndexThread>d__211))]
	private void SetOperatingModeProfileIndexThread(string _modeProfileIndex, bool IsLastCommand = false)
	{
		<SetOperatingModeProfileIndexThread>d__211 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine._modeProfileIndex = _modeProfileIndex;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string TransferToName(string mode, uint index)
	{
		string result = "M1P1";
		if (ResolvedDelegates.System_String_op_Equality__18(mode, "OPERATING_GAMING_MODE"))
		{
			switch (index)
			{
			case 0u:
				result = "M1P1";
				break;
			case 1u:
				result = "M1P2";
				break;
			}
		}
		else if (mode == "OPERATING_OFFICE_MODE")
		{
			switch (index)
			{
			case 0u:
				result = "M2P1";
				break;
			case 1u:
				result = "M2P2";
				break;
			}
		}
		else if (mode == "OPERATING_TURBO_MODE")
		{
			switch (index)
			{
			case 0u:
				result = "M3P1";
				break;
			case 1u:
				result = "M3P2";
				break;
			}
		}
		else if (mode == "OPERATING_CUSTOM_MODE")
		{
			switch (index)
			{
			case 0u:
				result = "M4P1";
				break;
			case 1u:
				result = "M4P2";
				break;
			case 2u:
				result = "M4P3";
				break;
			case 3u:
				result = "M4P4";
				break;
			case 4u:
				result = "M4P5";
				break;
			}
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[AsyncStateMachine(typeof(<SetModeSwitchChangeThread>d__217))]
	private void SetModeSwitchChangeThread(int _mode, bool fromscancode = false)
	{
		<SetModeSwitchChangeThread>d__217 stateMachine = default;
		stateMachine.<>t__builder = ResolvedDelegates.System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Create__190();
		stateMachine.<>4__this = this;
		stateMachine._mode = _mode;
		stateMachine.fromscancode = fromscancode;
		stateMachine.<>1__state = -1;
		stateMachine.<>t__builder.Start(ref stateMachine);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetModeSwitchChange(int mode, bool fromscancode = false)
	{
		switch (mode)
		{
		case 1:
			ResolvedDelegates.System_Threading_Tasks_Task_Run__10([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				if (CustomizeCtrl.GetIsUMATurbo())
				{
					App.m_Osd.ShowOSDByName(TransToGlobalType(2u), "");
				}
				else
				{
					App.m_Osd.ShowOSDByName(TransToGlobalType(1u), "");
				}
			});
			LogCtrl.TraceMessage("Switch to Gaming", "SetModeSwitchChange", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5579);
			UserSet_Mode1();
			break;
		case 0:
			ResolvedDelegates.System_Threading_Tasks_Task_Run__10([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				App.m_Osd.ShowOSDByName(TransToGlobalType(0u), "");
			});
			LogCtrl.TraceMessage("Switch to Office", "SetModeSwitchChange", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5589);
			UserSet_Mode2();
			break;
		case 2:
			ResolvedDelegates.System_Threading_Tasks_Task_Run__10([MethodImpl(MethodImplOptions.NoInlining)] () =>
			{
				App.m_Osd.ShowOSDByName(TransToGlobalType(2u), "");
			});
			LogCtrl.TraceMessage("Switch to Turbo", "SetModeSwitchChange", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5599);
			UserSet_Mode3();
			break;
		case 3:
			LogCtrl.TraceMessage("Switch to Custom", "SetModeSwitchChange", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5606);
			UserSet_Mode4(fromscancode);
			if (CustomizeCtrl.GetCustomId() == 9)
			{
				ResolvedDelegates.System_Threading_Tasks_Task_Run__10([MethodImpl(MethodImplOptions.NoInlining)] () =>
				{
					App.m_Osd.ShowOSDByName(TransToGlobalType(3u), ResolvedDelegates.System_UInt32_ToString__78(ref m_Option.CustomProfileIndex));
				});
			}
			else
			{
				Task.Run([MethodImpl(MethodImplOptions.NoInlining)] () =>
				{
					OSDManager osd = App.m_Osd;
					uint mode2 = TransToGlobalType(3u);
					uint num = m_Option.CustomProfileIndex + 1;
					osd.ShowOSDByName(mode2, ResolvedDelegates.System_UInt32_ToString__78(ref num));
				});
			}
			LoadOverClockingValue(currentProfile.OverClockingEnabled);
			break;
		}
		UpdateStatusToClient(1);
		FanTable.GetFanTable(currentProfile.FAN.TableName);
		LiquidHWOC.Instance.RamFan1P5SetMode(mode);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void RefreshCpuPLMinimumValue()
	{
		GN20_GPU_SKU sku = EcCtrl.GetSku();
		BIOS_PROJECT_ID biosProjctID = EcCtrl.GetBiosProjctID();
		if (sku == GN20_GPU_SKU.E5 && biosProjctID == BIOS_PROJECT_ID.IDR)
		{
			CpuPL1Minimum = 30;
			CpuPL2Minimum = 30;
			CpuPL4Minimum = 30;
		}
		else
		{
			CpuPL1Minimum = 10;
			CpuPL2Minimum = 10;
			CpuPL4Minimum = 10;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ReCheckCpuPLMinimumValueI()
	{
		if (g_SupportCPUDoubleFlag == 1)
		{
			if (currentProfile.CPU.PL2 < currentProfile.CPU.PL1)
			{
				currentProfile.CPU.PL2 = currentProfile.CPU.PL1;
			}
			if (currentProfile.CPU.PL4 < currentProfile.CPU.PL1 / 2)
			{
				currentProfile.CPU.PL4 = currentProfile.CPU.PL1 / 2;
			}
			if (currentProfile.CPU.PL4 < currentProfile.CPU.PL2 / 2)
			{
				currentProfile.CPU.PL4 = currentProfile.CPU.PL2 / 2;
			}
		}
		else
		{
			if (currentProfile.CPU.PL2 < currentProfile.CPU.PL1)
			{
				currentProfile.CPU.PL2 = currentProfile.CPU.PL1;
			}
			if (currentProfile.CPU.PL4 < currentProfile.CPU.PL1)
			{
				currentProfile.CPU.PL4 = currentProfile.CPU.PL1;
			}
			if (currentProfile.CPU.PL4 < currentProfile.CPU.PL2)
			{
				currentProfile.CPU.PL4 = currentProfile.CPU.PL2;
			}
		}
		SetPL1Value(currentProfile.CPU.PL1);
		SetPL2Value(currentProfile.CPU.PL2);
		SetPL4Value(currentProfile.CPU.PL4);
		UpdateStatusToClient(1);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ReCheckCpuPLMinimumValueA()
	{
		if (g_SupportCPUDoubleFlag == 1)
		{
			if (currentProfile.CPU.AmdSPPT < currentProfile.CPU.AmdSPL)
			{
				currentProfile.CPU.AmdSPPT = currentProfile.CPU.AmdSPL;
			}
			if (currentProfile.CPU.AmdFPPT < currentProfile.CPU.AmdSPL / 2)
			{
				currentProfile.CPU.AmdFPPT = currentProfile.CPU.AmdSPL / 2;
			}
			if (currentProfile.CPU.AmdFPPT < currentProfile.CPU.AmdSPPT / 2)
			{
				currentProfile.CPU.AmdFPPT = currentProfile.CPU.AmdSPPT / 2;
			}
		}
		else
		{
			if (currentProfile.CPU.AmdSPPT < currentProfile.CPU.AmdSPL)
			{
				currentProfile.CPU.AmdSPPT = currentProfile.CPU.AmdSPL;
			}
			if (currentProfile.CPU.AmdFPPT < currentProfile.CPU.AmdSPL)
			{
				currentProfile.CPU.AmdFPPT = currentProfile.CPU.AmdSPL;
			}
			if (currentProfile.CPU.AmdFPPT < currentProfile.CPU.AmdSPPT)
			{
				currentProfile.CPU.AmdFPPT = currentProfile.CPU.AmdSPPT;
			}
		}
		SetPL1Value(currentProfile.CPU.AmdSPL);
		SetPL2Value(currentProfile.CPU.AmdSPPT);
		SetPL4Value(currentProfile.CPU.AmdFPPT);
		UpdateStatusToClient(1);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void LoadOverClockingValueSimp(int OCEnable)
	{
		if (OCEnable == 1)
		{
			currentProfile.GPU.CoreClockOffsetOC = 35;
			currentProfile.GPU.MemoryClockOffsetOC = -125;
			cGpuFeatures.SetGpuCoreFreqOffsetValue(currentProfile.GPU.CoreClockOffsetOC);
			cGpuFeatures.SetGpuMemFreqOffsetValue(currentProfile.GPU.MemoryClockOffsetOC);
		}
		else
		{
			cGpuFeatures.SetGpuCoreFreqOffsetValue(0);
			cGpuFeatures.SetGpuMemFreqOffsetValue(0);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void LoadOverClockingValue(int OCEnable)
	{
		uint fanMode = 3u;
		SetFanMode(fanMode);
		FanTable.SetFanTable(currentProfile.FAN.TableName);
		if (OCEnable == 1)
		{
			cGpuFeatures.SetGpuCoreFreqOffsetValue(currentProfile.GPU.CoreClockOffsetOC);
			cGpuFeatures.SetGpuMemFreqOffsetValue(currentProfile.GPU.MemoryClockOffsetOC);
		}
		else
		{
			cGpuFeatures.SetGpuCoreFreqOffsetValue(0);
			cGpuFeatures.SetGpuMemFreqOffsetValue(0);
		}
		if (CustomizeCtrl.GetCustomId() == 26 && (_skuGN21 == 16 || _skuGN21 == 21 || _skuGN21 == 17 || _skuGN21 == 18))
		{
			cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(1);
			cGpuFeatures.SetGpuConfigurableTGPTarget(m_GpuConfigurableTGPMaximum, _SmartApcTable.DefaultTGP);
			cGpuFeatures.SetGpuDynamicBoostEnable(1);
			cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(_SmartApcTable.DynamicBoost);
		}
		else if (OCEnable == 1)
		{
			cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(1);
			cGpuFeatures.SetGpuConfigurableTGPTarget(m_GpuConfigurableTGPMaximum, _SmartApcTable.DefaultTGP);
			cGpuFeatures.SetGpuDynamicBoostEnable(1);
			cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(_SmartApcTable.DynamicBoost);
			cGpuFeatures.SetSuperPerformance(enable: true);
		}
		else
		{
			cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(currentProfile.GPU.ConfigurableTGPSwitch);
			cGpuFeatures.SetGpuConfigurableTGPTarget(currentProfile.GPU.ConfigurableTGPTarget, _SmartApcTable.DefaultTGP);
			cGpuFeatures.SetGpuDynamicBoostEnable(currentProfile.GPU.DynamicBoostSwitch);
			cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(currentProfile.GPU.DynamicBoost);
			cGpuFeatures.SetSuperPerformance(enable: false);
		}
		if (EcCtrl.IsAMDPlatform())
		{
			SetCpuAmdCoreFreq(currentProfile.CPU.AmdCoreFreq);
			SetCpuAmdCoreVoltage(currentProfile.CPU.AmdCoreVoltage);
			SetCpuAmdCoreVoltage(currentProfile.CPU.AmdCoreVoltage);
		}
		else
		{
			SetCpuCoreVoltageOffset(currentProfile.CPU.OffsetCoreVoltage);
		}
		if (NvramVariableInfo.MemoryOverClockSupport == 1)
		{
			if (NvramVariableInfo.APVersionCheck >= 23)
			{
				SetMemoryOverClockSwitch(NvramVariableInfo.CPUPerformanceAndOverClockMenuSwitch);
			}
			else
			{
				SetMemoryOverClockSwitch(currentProfile.MEM.MemoryOverClockSwitch);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void SetGameMode(int onoff, int ctgp, int db)
	{
		cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(onoff);
		cGpuFeatures.SetGpuConfigurableTGPTarget(ctgp + _SmartApcTable.DefaultTGP, _SmartApcTable.DefaultTGP);
		cGpuFeatures.SetGpuDynamicBoostTotalProcessingPowerTarget((onoff == 1) ? currentProfile.GPU.DynamicBoostTotalProcessingPowerTarget : 0);
		cGpuFeatures.SetGpuDynamicBoostEnable(onoff);
		cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(db);
		cGpuFeatures.SetSuperPerformance(enable: false);
		LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__120("SetGpuConfigurableTGPTarget ctgp \uff1a ", ResolvedDelegates.System_Int32_ToString__55(ref ctgp), " | SetGpuDynamicBoostMaxinumTGP : ", ResolvedDelegates.System_Int32_ToString__55(ref db)), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5885);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void SetDynamicBoostforTurboMode()
	{
		cGpuFeatures.SetGpuDynamicBoostEnable(1);
		cGpuFeatures.SetGpuDynamicBoostMaxinumTGP(_SmartApcTable.DynamicBoost);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void TriggerCTGPforTurboMode()
	{
		cGpuFeatures.SetGpuConfigurableTgpFunCtrlEnable(1);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe void WMIHandleEvent(int scancode)
	{
		try
		{
			int num = ResolvedDelegates.System_Diagnostics_Process_get_Id__87(ResolvedDelegates.System_Diagnostics_Process_GetCurrentProcess__243());
			int num2 = ResolvedDelegates.System_Threading_Thread_get_ManagedThreadId__118(ResolvedDelegates.System_Threading_Thread_get_CurrentThread__117());
			LogCtrl.TraceMessage(ResolvedDelegates.System_String_Format__176("[WMI Listener  PID={0} | Thread {1} Recive scancode {2}", num, num2, scancode), "WMIHandleEvent", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 5909);
			if (scancode == 200 && ResolvedDelegates.System_String_op_Equality__18(ResolvedDelegates.System_String_Substring__109(ref *(string*)currentProfile.Name, 0, 5), "Mode2"))
			{
				_Enable = true;
				currentProfile.GPU.WhisperModeSwitch = ResolvedDelegates.System_Convert_ToInt32__220(_Enable);
				for (int i = 0; i < 10; i++)
				{
					SetGpuWhisperModeSwitchOnOFF(currentProfile.GPU.WhisperModeSwitch);
					ResolvedDelegates.System_Threading_Thread_Sleep__6(20);
					byte Data = 0;
					EcCtrl.Read(ResolvedDelegates.System_Reflection_MemberInfo_get_Name__128(ResolvedDelegates.System_Object_GetType__121(this)), 1990, ref Data);
					if ((Data & 3) == 3)
					{
						break;
					}
				}
				RefreshCurrentProfile(m_Option.OperatingMode);
			}
			if (scancode == 201)
			{
				_Enable = !_Enable;
				if (_Enable)
				{
					SetModeSwitchChangeThread(3);
				}
				else
				{
					SetModeSwitchChangeThread(_OPperatingMode);
				}
			}
		}
		catch
		{
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ExecuteCommand(string command)
	{
		object obj = ResolvedDelegates.System_Diagnostics_Process_Close__312;
		Process process = ResolvedDelegates.System_Diagnostics_Process_Start__307(new ProcessStartInfo("cmd.exe", ResolvedDelegates.System_String_Concat__29("/c ", command))
		{
			CreateNoWindow = true,
			UseShellExecute = false,
			RedirectStandardError = true,
			RedirectStandardOutput = true
		});
		process.WaitForExit();
		process.StandardOutput.ReadToEnd();
		process.StandardError.ReadToEnd();
		_ = process.ExitCode;
		obj(process);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void SetGameMode(GameProfile profile)
	{
		if (profile != null)
		{
			RefreshCurrentProfile();
			int num = ResolvedDelegates.System_Convert_ToInt32__54(profile.TjMax);
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("Clamp _SmartApcTable.TccOffset : ", ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.TccOffset)), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6014);
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("Clamp profile.TjMax : ", profile.TjMax), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6015);
			SetCpuTccOffset(num, bApExist: true);
			m_IsAMDPlatform = CustomizeCtrl.GetIsAMDPlatform();
			if (m_IsAMDPlatform)
			{
				SetCpuTccOffset(num, bApExist: true);
			}
			else
			{
				int num2 = 100;
				num2 = ((_tjMax == 0) ? (100 - num) : (_tjMax - num));
				SetCpuTccOffset(num2, bApExist: true);
			}
			LogCtrl.TraceMessageProgramData("SetCpuTccOffset _SmartApcTable.TccOffset \uff1a " + ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.TccOffset), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6040);
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("SetCpuTccOffset TjMax \uff1a ", ResolvedDelegates.System_Int32_ToString__55(ref num)), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6041);
			int ctgp = ResolvedDelegates.System_Convert_ToInt32__54(profile.ConfigurableTgp);
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("Clamp ConfigurableTgp : ", ResolvedDelegates.System_Int32_ToString__55(ref ctgp)), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6045);
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("Clamp _SmartApcTable.DefaultTGP : ", ResolvedDelegates.System_Byte_ToString__119(ref _SmartApcTable.DefaultTGP)), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6046);
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("Clamp m_GpuConfigurableTGPMaximum : ", ResolvedDelegates.System_Int32_ToString__55(ref m_GpuConfigurableTGPMaximum)), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6047);
			int value = ResolvedDelegates.System_Convert_ToInt32__54(profile.DynamicBoost);
			value = Clamp(value, 5, _SmartApcTable.DynamicBoost);
			SetGameMode(1, ctgp, value);
			RefreshCpuPLMinimumValue();
			int value2 = ResolvedDelegates.System_Convert_ToInt32__54(profile.Pl4);
			value2 = Clamp(value2, CpuPL4Minimum, _SmartApcTable.PL4);
			int value3 = ResolvedDelegates.System_Convert_ToInt32__54(profile.Pl2);
			value3 = Clamp(value3, CpuPL2Minimum, _SmartApcTable.PL2);
			int value4 = ResolvedDelegates.System_Convert_ToInt32__54(profile.Pl1);
			value4 = Clamp(value4, CpuPL1Minimum, _SmartApcTable.PL1);
			SetPL4Value(value2);
			SetPL2Value(value3);
			SetPL1Value(value4);
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__79(new string[6]
			{
				"SetPL1Value PL1 \uff1a ",
				ResolvedDelegates.System_Int32_ToString__55(ref value4),
				" | SetPL1Value PL2 \uff1a ",
				ResolvedDelegates.System_Int32_ToString__55(ref value3),
				"SetPL1Value PL4 \uff1a ",
				ResolvedDelegates.System_Int32_ToString__55(ref value2)
			}), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6071);
			int value5 = ResolvedDelegates.System_Convert_ToInt32__54(profile.GpuClock);
			value5 = Clamp(value5, 0, GpuCoreClockOffsetMaximum);
			cGpuFeatures.SetGpuCoreFreqOffsetValue(value5);
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("SetGpuCoreFreqOffsetValue GpuClock \uff1a ", ResolvedDelegates.System_Int32_ToString__55(ref value5)), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6077);
			int value6 = ResolvedDelegates.System_Convert_ToInt32__54(profile.MemClock);
			value6 = Clamp(value6, GpuMemoryClockOffsetMinimum, GpuMemoryClockOffsetMaximum);
			cGpuFeatures.SetGpuMemFreqOffsetValue(value6);
			LogCtrl.TraceMessageProgramData(ResolvedDelegates.System_String_Concat__29("SetGpuMemFreqOffsetValue MemClock \uff1a ", ResolvedDelegates.System_Int32_ToString__55(ref value6)), "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6083);
			LogCtrl.TraceMessageProgramData("SetGameMode Success!", "SetGameMode", "E:\\WorkSpace\\CCU_GX\\MyControlCenter\\MyControlCenter\\MyFan\\MyFanManager_RamFan1p5.cs", 6085);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public T Clamp<T>(T value, T min, T max) where T : IComparable<T>
	{
		if (value.CompareTo(min) < 0)
		{
			return min;
		}
		if (value.CompareTo(max) > 0)
		{
			return max;
		}
		return value;
	}
}

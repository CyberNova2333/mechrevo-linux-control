// Static analysis view; decoded strings and delegate names; not executable OEM code.
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using \u000b\u0009\u0013\u000f\u0017\u0005;
using LightingModel;

namespace GCUService.MyRgbKeyboard;

internal class KeytoLightMapping
{
	private RGBKB_Type KeyboardType;

	private Dictionary<Keys, string> keyMapping = new Dictionary<Keys, string>();

	[DllImport("user32", SetLastError = true)]
	private static extern int GetKeyNameText(int lParam, StringBuilder lpString, int cchSize);

	[MethodImpl(MethodImplOptions.NoInlining)]
	public KeytoLightMapping()
	{
		ResolvedDelegates.System_Object__ctor__0(this);
		List<Keys> keys = EnumHelper.ToList<Keys>();
		Create101KeyMapping(keys);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public string ReturnLightNumber(Keys key)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!Enumerable.Contains(keyMapping.Keys, key))
		{
			return null;
		}
		return keyMapping[key];
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Create101KeyMapping(List<Keys> keys)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		keyMapping.Add((Keys)27, "0");
		num = 1;
		IEnumerator<Keys> enumerator = keys.Skip(107).Take(12).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Keys current = enumerator.Current;
				keyMapping.Add(current, ResolvedDelegates.System_Int32_ToString__55(ref num));
				num++;
			}
		}
		finally
		{
			if (enumerator != null)
			{
				ResolvedDelegates.System_IDisposable_Dispose__3(enumerator);
			}
		}
		keyMapping.Add((Keys)44, "13");
		keyMapping.Add((Keys)19, "14");
		keyMapping.Add((Keys)46, "15");
		keyMapping.Add((Keys)35, "16");
		num = 21;
		keyMapping.Add((Keys)192, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		enumerator = keys.Skip(52).Take(9).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Keys current2 = enumerator.Current;
				keyMapping.Add(current2, ResolvedDelegates.System_Int32_ToString__55(ref num));
				num++;
			}
		}
		finally
		{
			if (enumerator != null)
			{
				ResolvedDelegates.System_IDisposable_Dispose__3(enumerator);
			}
		}
		keyMapping.Add((Keys)48, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)189, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)187, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)8, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num = 42;
		keyMapping.Add((Keys)9, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)81, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)87, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)69, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)82, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)84, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)89, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)85, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)73, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)79, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)80, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)219, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)221, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)220, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)103, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)104, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)105, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num = 63;
		keyMapping.Add((Keys)20, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		num++;
		keyMapping.Add((Keys)65, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)83, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)68, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)70, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)71, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)72, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)74, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)75, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)76, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)186, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)222, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)13, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num = 84;
		keyMapping.Add((Keys)160, ResolvedDelegates.System_String_Concat__29(ResolvedDelegates.System_Int32_ToString__55(ref num), ",85"));
		num++;
		num++;
		keyMapping.Add((Keys)90, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)88, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)67, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)86, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)66, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)78, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)77, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)188, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)190, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)191, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)161, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)38, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num = 105;
		keyMapping.Add((Keys)162, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)0, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)91, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)164, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		num++;
		num++;
		num++;
		keyMapping.Add((Keys)32, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		num++;
		num++;
		keyMapping.Add((Keys)165, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)93, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)163, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)37, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)40, ResolvedDelegates.System_Int32_ToString__55(ref num));
		num++;
		keyMapping.Add((Keys)39, ResolvedDelegates.System_Int32_ToString__55(ref num));
		foreach (Keys key in keys)
		{
			_ = key;
			num++;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static string GetKeyName(byte key)
	{
		StringBuilder stringBuilder = new StringBuilder(20);
		while (GetKeyNameText(key << 16, stringBuilder, ResolvedDelegates.System_Text_StringBuilder_get_Capacity__407(stringBuilder)) == ResolvedDelegates.System_Text_StringBuilder_get_Capacity__407(stringBuilder) - 1)
		{
			stringBuilder.Capacity *= 2;
		}
		return ResolvedDelegates.System_Object_ToString__1(stringBuilder);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static string[] GetKeyNames()
	{
		string[] array = new string[255];
		for (byte b = 0; b < 254; b++)
		{
			array[b] = GetKeyName(b);
		}
		return array;
	}
}

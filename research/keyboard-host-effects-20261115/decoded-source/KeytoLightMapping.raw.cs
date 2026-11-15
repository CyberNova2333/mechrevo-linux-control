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
		\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[0](this);
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
		keyMapping.Add((Keys)27, \u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("_"));
		num = 1;
		IEnumerator<Keys> enumerator = keys.Skip(107).Take(12).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Keys current = enumerator.Current;
				keyMapping.Add(current, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
				num++;
			}
		}
		finally
		{
			if (enumerator != null)
			{
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[3](enumerator);
			}
		}
		keyMapping.Add((Keys)44, \u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("a^"));
		keyMapping.Add((Keys)19, \u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("ac"));
		keyMapping.Add((Keys)46, \u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("ad"));
		keyMapping.Add((Keys)35, \u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("aa"));
		num = 21;
		keyMapping.Add((Keys)192, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		enumerator = keys.Skip(52).Take(9).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Keys current2 = enumerator.Current;
				keyMapping.Add(current2, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
				num++;
			}
		}
		finally
		{
			if (enumerator != null)
			{
				\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[3](enumerator);
			}
		}
		keyMapping.Add((Keys)48, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)189, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)187, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)8, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num = 42;
		keyMapping.Add((Keys)9, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)81, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)87, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)69, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)82, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)84, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)89, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)85, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)73, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)79, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)80, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)219, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)221, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)220, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)103, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)104, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)105, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num = 63;
		keyMapping.Add((Keys)20, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		num++;
		keyMapping.Add((Keys)65, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)83, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)68, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)70, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)71, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)72, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)74, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)75, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)76, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)186, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)222, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)13, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num = 84;
		keyMapping.Add((Keys)160, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[29](\u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num), \u0009\u000e\u0013\u0006\u0006\u000c.\u0009\u0008\u0018\u000e\u000c\u0017("mXb")));
		num++;
		num++;
		keyMapping.Add((Keys)90, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)88, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)67, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)86, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)66, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)78, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)77, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)188, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)190, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)191, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)161, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)38, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num = 105;
		keyMapping.Add((Keys)162, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)0, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)91, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)164, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		num++;
		num++;
		num++;
		keyMapping.Add((Keys)32, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		num++;
		num++;
		keyMapping.Add((Keys)165, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)93, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)163, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)37, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)40, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
		num++;
		keyMapping.Add((Keys)39, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[55](ref num));
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
		while (GetKeyNameText(key << 16, stringBuilder, \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[407](stringBuilder)) == \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[407](stringBuilder) - 1)
		{
			stringBuilder.Capacity *= 2;
		}
		return \u0001\u0006\u0005\u0004\u0005\u0005\u0003\u0005\u0001\u0003\u0001\u0004.\u0008\u0009\u0010\u000c\u0003\u000f\u0011\u0002[1](stringBuilder);
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

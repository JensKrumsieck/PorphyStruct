using System;

namespace PorphyStruct.Oxy.Override
{
	/// <summary>
	/// some utils for oxyplot stuff
	/// </summary>
	public static class OxyUtils
    {
		public static string _axisFormatter(double d)
		{
			//numbers are usually between 0 and 20 for x and -1 and 1 for y.
			//so resolution for y is x.yy and for x is x
			if(d < 1 && d > -1)
			{
				if (d < 1E-3 && d > -1e-3 && d != 0)
					return d.ToString(System.Globalization.CultureInfo.InvariantCulture);
				return d.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
			}
			return d.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
    }
}

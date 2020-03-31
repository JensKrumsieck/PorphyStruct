namespace PorphyStruct.OxyPlotOverride
{
    /// <summary>
    /// some utils for oxyplot stuff
    /// </summary>
    public static class OxyUtils
    {
        public static string yFormatter(double d)
        {
            if (d < 1E-3 && d > -1e-3 && d != 0)
                return xFormatter(d);
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:N2}", d);
        }

        public static string xFormatter(double d) => d.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}

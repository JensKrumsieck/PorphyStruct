using OxyPlot;

namespace PorphyStruct.Core.OxyPlot.Custom
{
    public static class CustomPalettes
    {
        /// <summary>
        /// Default Palette defined by Black, Red, Blue, Green
        /// </summary>
        /// <param name="numberOfColors"></param>
        /// <returns></returns>
        public static OxyPalette Default(int numberOfColors)
        {
            return OxyPalette.Interpolate(numberOfColors, OxyColors.Black, OxyColors.Red, OxyColors.Blue, OxyColors.Green);
        }
    }
}

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
        public static OxyPalette Default(int numberOfColors) => OxyPalette.Interpolate(numberOfColors,
            OxyColors.Black,
            OxyColors.Red,
            OxyColors.Blue,
            OxyColors.Green,
            OxyColors.Orange,
            OxyColors.Purple,
            OxyColors.DarkRed
            );

        /// <summary>
        /// Palette derived from Bulma CSS Framework (http://bulma.io)
        /// </summary>
        /// <param name="numberOfColors"></param>
        /// <returns></returns>
        public static OxyPalette Bulma(int numberOfColors) => OxyPalette.Interpolate(numberOfColors,
            OxyColors.Black,
            OxyColor.Parse("#ff470f"), //orange
            OxyColor.Parse("#3273dc"), //blue
            OxyColor.Parse("#48c774"), //green
            OxyColor.Parse("#ffdd57"), //yellow
            OxyColor.Parse("#b86bff"), //purple
            OxyColor.Parse("#f14668") //red
            );

        /// <summary>
        /// Palette derived from Twitter Bootstrap (http://getbootstrap)
        /// </summary>
        /// <param name="numberOfColors"></param>
        /// <returns></returns>
        public static OxyPalette Modern(int numberOfColors) => OxyPalette.Interpolate(numberOfColors,
            OxyColors.Black,
            OxyColor.Parse("#007bff"), //blue
            OxyColor.Parse("#dc3545"), //red
            OxyColor.Parse("#28a745"), //green
            OxyColor.Parse("#6610f2"), //indigo
            OxyColor.Parse("#e83e8c"), //pink
            OxyColor.Parse("#fd7e14") //orange
            );

        /// <summary>
        /// Modern Grayscale Palette
        /// </summary>
        /// <param name="numberOfColors"></param>
        /// <returns></returns>
        public static OxyPalette ModernGrayScale(int numberOfColors) => OxyPalette.Interpolate(numberOfColors,
            OxyColors.Black,
            OxyColor.Parse("#212529"),
            OxyColor.Parse("#343a40"),
            OxyColor.Parse("#495057"),
            OxyColor.Parse("#6c757d"),
            OxyColor.Parse("#adb5bd")
            );

        /// <summary>
        /// Default Grayscale Palette
        /// </summary>
        /// <param name="numberOfColors"></param>
        /// <returns></returns>
        public static OxyPalette GrayScale(int numberOfColors) => OxyPalette.Interpolate(numberOfColors,
            OxyColors.Black,
            OxyColor.Parse("#111111"),
            OxyColor.Parse("#222222"),
            OxyColor.Parse("#333333"),
            OxyColor.Parse("#444444"),
            OxyColor.Parse("#555555"),
            OxyColor.Parse("#666666"),
            OxyColor.Parse("#777777"),
            OxyColor.Parse("#888888")
            );

        /// <summary>
        /// Palette consiting of darker colors
        /// </summary>
        /// <param name="numberOfColors"></param>
        /// <returns></returns>
        public static OxyPalette DarkColors(int numberOfColors) => OxyPalette.Interpolate(numberOfColors,
            OxyColor.Parse("#17202A"), //darkblue
            OxyColor.Parse("#78281F"), //darkred
            OxyColor.Parse("#0B5345"), //darkgreen
            OxyColor.Parse("#4A235A"), //deeppurple
            OxyColor.Parse("#424949"), //gray
            OxyColor.Parse("#78281F"), //another dark red
            OxyColor.Parse("#6E2C00") //dark orange
            );

        /// <summary>
        /// Colorful Scheme
        /// </summary>
        /// <param name="numberOfColors"></param>
        /// <returns></returns>
        public static OxyPalette RoseMary(int numberOfColors) => OxyPalette.Interpolate(numberOfColors,
            OxyColors.Black,
            OxyColor.Parse("#ec466f"),
            OxyColor.Parse("#4b4e76"),
            OxyColor.Parse("#f67d49"),
            OxyColor.Parse("#aea82d"),
            OxyColor.Parse("#f67d49"),
            OxyColor.Parse("#63dbcb")
            );
    }
}

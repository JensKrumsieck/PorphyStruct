using System;
using System.Drawing;

namespace PorphyStruct.Extension
{
    public static class ColorUtil
    {
        /// <summary>
        /// Converts Color to Hex
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ToHexString(this Color c) => "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

        public static string HexStringFromGuid(this Guid g) => g.ColorFromGuid().ToHexString();

        public static Color ColorFromGuid(this Guid g) => Color.FromArgb(g.GetHashCode());
    }
}

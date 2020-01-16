using PorphyStruct.Chemistry;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PorphyStruct.Windows
{
    /// <summary>
    /// static extensions for windows, basically for Helix Toolkit 3D
    /// </summary>
    public static class Win32Util
    {
        /// <summary>
        /// Helix Toolkit origin shortcut
        /// </summary>
        public static Point3D Origin => new Point3D(0, 0, 0);


        /// <summary>
        /// get Brush for Helix
        /// </summary>
        public static Brush Brush(this Element element) => (Brush)new BrushConverter().ConvertFromString(element.Color);

        /// <summary>
        /// Brush Wrapper for Atom
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Brush Brush(this Atom a) => Brush(a.Element);


        /// <summary>
        /// Converts Atom to simple Point3D
        /// </summary>
        /// <returns></returns>
        public static Point3D ToPoint3D(this Atom a) => new Point3D(a.X, a.Y, a.Z);
    }
}

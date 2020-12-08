using PorphyStruct.Chemistry;
using System.Collections.Generic;
using System.Windows;
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


        /// <summary>
        /// Finds Visual Children of DependencyObjecz
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T dependencyObject) yield return dependencyObject;
                foreach (T childOfChild in FindVisualChildren<T>(child)) yield return childOfChild;
            }
        }
    }
}

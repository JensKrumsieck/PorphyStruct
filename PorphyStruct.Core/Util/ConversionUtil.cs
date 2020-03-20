using MathNet.Spatial.Euclidean;
using PorphyStruct.Chemistry;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Util
{
    /// <summary>
    /// Provids converter methods for some objects
    /// </summary>
    public static class ConversionUtil
    {
        /// <summary>
        /// Converts the xyz into Point3D
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Point3D> ToPoint3D(this IEnumerable<Atom> Atoms) => from Atom atom in Atoms select atom.ToPoint3D();

        /// <summary>
        /// Converts Atom to Point3d
        /// </summary>
        /// <param name="atom"></param>
        /// <returns></returns>
        public static Point3D ToPoint3D(this Atom atom) => new Point3D(atom.X, atom.Y, atom.Z);

        /// <summary>
        /// Get DataPoints from double to calc deriv/integral
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datapoints"></param>
        /// <returns></returns>
        public static List<AtomDataPoint> ToAtomDataPoints(this double[] data, IList<AtomDataPoint> datapoints)
        {
            var list = new List<AtomDataPoint>();
            for (int i = 0; i < data.Length; i++) list.Add(new AtomDataPoint(datapoints[i].X, data[i], datapoints[i].atom));
            return list;
        }

        /// <summary>
        /// get ydata from cycle
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        public static double[] ToDoubleArray(this IList<AtomDataPoint> datapoints) => datapoints.Select(s => s.Y).ToArray();
    }
}

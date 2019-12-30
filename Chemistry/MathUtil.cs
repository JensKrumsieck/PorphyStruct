using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry
{
    public static class MathUtil
    {
        /// <summary>
        /// Converts the xyz into Point3D because some methods need math net spatial...
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Point3D> ToPoint3D(this IEnumerable<Atom> Atoms)
        {
            foreach (Atom atom in Atoms)
            {
                yield return new Point3D(atom.X, atom.Y, atom.Z);
            }
        }

        /// <summary>
        /// gets highest Y Value (or lowest)
        /// </summary>
        /// <param name="data">AtomDataPoints</param>
        /// <returns>highest/lowest Y Value</returns>
        public static double GetNormalizationFactor(List<AtomDataPoint> data)
        {
            //find min & max
            double min = 0;
            double max = 0;
            double fac;
            foreach (AtomDataPoint dp in data)
            {
                //exclude metal from normalization
                if (!dp.atom.IsMetal)
                {
                    if (dp.Y < min)
                        min = dp.Y;
                    if (dp.Y > max)
                        max = dp.Y;
                }
            }

            if (Math.Abs(max) > Math.Abs(min))
                fac = Math.Abs(max);
            else
                fac = Math.Abs(min);
            return fac;
        }

        /// <summary>
        /// normalizes data with factor detection
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<AtomDataPoint> Normalize(List<AtomDataPoint> data)
        {
            double fac = GetNormalizationFactor(data);
            return Factor(data, fac);
        }

        /// <summary>
        /// invert
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<AtomDataPoint> Invert(List<AtomDataPoint> data)
        {
            return Factor(data, -1);
        }

        /// <summary>
        /// multiply by given factor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fac"></param>
        /// <returns></returns>
        public static List<AtomDataPoint> Factor(List<AtomDataPoint> data, double fac)
        {
            List<AtomDataPoint> normData = new List<AtomDataPoint>();
            foreach (AtomDataPoint dp in data)
            {
                normData.Add(new AtomDataPoint(dp.X, dp.Y / fac, dp.atom));
            }
            return normData;
        }


        /// <summary>
        /// Vector Crossproduct for MathNet Numerics
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vector<double> CrossProduct(Vector<double> left, Vector<double> right)
        {
            Vector<double> result = DenseVector.Create(3, 0);
            result[0] = left[1] * right[2] - left[2] * right[1];
            result[1] = -left[0] * right[2] + left[2] * right[0];
            result[2] = left[0] * right[1] - left[1] * right[0];
            return result;
        }


    }
}

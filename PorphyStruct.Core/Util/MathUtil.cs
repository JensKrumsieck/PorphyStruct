using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using PorphyStruct.Chemistry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Util
{
    public static class MathUtil
    {
        /// <summary>
        /// gets highest Y Value (or lowest)
        /// </summary>
        /// <param name="data">AtomDataPoints</param>
        /// <returns>highest/lowest Y Value</returns>
        public static double GetNormalizationFactor(this IEnumerable<AtomDataPoint> data)
        {
            //find min & max
            double min = 0;
            double max = 0;
            double fac;
            foreach (var dp in data.Where(dp => !dp.atom.IsMetal))
            {
                if (dp.Y < min)
                    min = dp.Y;
                if (dp.Y > max)
                    max = dp.Y;
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
        public static IEnumerable<AtomDataPoint> Normalize(this IEnumerable<AtomDataPoint> data)
        {
            double fac = GetNormalizationFactor(data);
            return Factor(data, fac).ToList();
        }

        /// <summary>
        /// invert
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<AtomDataPoint> Invert(this IEnumerable<AtomDataPoint> data) => Factor(data, -1).ToList();

        /// <summary>
        /// multiply by given factor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fac"></param>
        /// <returns></returns>
        public static IEnumerable<AtomDataPoint> Factor(this IEnumerable<AtomDataPoint> data, double fac) => data.Select(dp => new AtomDataPoint(dp.X, dp.Y / fac, dp.atom));

        /// <summary>
        /// Gets the difference between to lists of ADP
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        public static IEnumerable<AtomDataPoint> Difference(this IEnumerable<AtomDataPoint> data1, IEnumerable<AtomDataPoint> data2)
        {
            //sort lists
            data1 = data1.OrderBy(s => s.X).ToList();
            data2 = data2.OrderBy(s => s.X).ToList();
            for (int i = 0; i < data2.Count(); i++) yield return new AtomDataPoint(data1.ElementAt(i).X, data1.ElementAt(i).Y - data2.ElementAt(i).Y, data1.ElementAt(i).atom);
        }

        /// <summary>
        /// Mean Displacement
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double MeanDisplacement(this IEnumerable<AtomDataPoint> data) => Math.Sqrt(data.Sum(s => Math.Pow(s.Y, 2)));

        // <summary>
        /// Calculate the Derivative of given DataPoints
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<double> Derive(this List<AtomDataPoint> data)
        {
            for (int i = 0; i < data.Count; i++)
                yield return i == 0 ? 0 : (data[i].Y - data[i - 1].Y) / (data[i].X - data[i - 1].X);
        }

        /// <summary>
		/// Calculate the integral of given DataPoints
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static IEnumerable<double> Integrate(this double[] data)
        {
            double previous = 0;
            for (int i = 0; i < data.Length; i++)
                yield return previous += data[i];
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
            result[0] = (left[1] * right[2]) - (left[2] * right[1]);
            result[1] = (left[2] * right[0]) - (left[0] * right[2]);
            result[2] = (left[0] * right[1]) - (left[1] * right[0]);
            return result;
        }

        /// <summary>
        /// calculates the absolute sum
        /// </summary>
        /// <param name="d"></param>
        public static double AbsSum(this IEnumerable<double> d) => d.Sum(s => Math.Abs(s));

        /// <summary>
        /// Normalizes IEnumerable<double>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<double> Normalize(this IEnumerable<double> input)
        {
            foreach (double d in input) yield return d / input.AbsSum();
        }

        /// <summary>
        /// Calculates Dihedral
        /// </summary>
        /// <param name="atoms"></param>
        /// <returns></returns>
        public static double Dihedral(IList<Atom> atoms)
        {
            if (atoms.Count() != 4) return 0;
            //build normalized vectors
            Vector<double> b1 = -(atoms[0].Vector - atoms[1].Vector);
            Vector<double> b2 = (atoms[1].Vector - atoms[2].Vector);
            Vector<double> b3 = (atoms[3].Vector - atoms[2].Vector);

            //calculate crossproducts
            Vector<double> c1 = CrossProduct(b1, b2).Normalize(2);
            Vector<double> c2 = CrossProduct(b2, b3).Normalize(2);
            Vector<double> c3 = CrossProduct(c1, b2.Normalize(2));

            return 180.0 / Math.PI * Math.Atan2(c3.DotProduct(c2), c1.DotProduct(c2));
        }

        /// <summary>
        /// Returns the Angle by Identifiers
        /// </summary>
        /// <param name="atoms"></param>
        /// <returns></returns>
        public static double Angle(IList<Atom> atoms)
        {
            if (atoms.Count() != 3) return 0;
            Vector<double> b1 = (atoms[0].Vector - atoms[1].Vector).Normalize(2);
            Vector<double> b2 = (atoms[2].Vector - atoms[1].Vector).Normalize(2);

            return (180 / Math.PI) * Math.Acos(b1.DotProduct(b2));
        }

        /// <summary>
        /// Calculates an angle between two planes
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Angle(Plane p1, Plane p2)
        {
            double d = Math.Abs((p1.A * p2.A) + (p1.B * p2.B) + (p1.C * p2.C));
            double e1 = Math.Pow(p1.A, 2) + Math.Pow(p1.B, 2) + Math.Pow(p1.C, 2);
            double e2 = Math.Pow(p2.A, 2) + Math.Pow(p2.B, 2) + Math.Pow(p2.C, 2);

            return 180 / Math.PI * Math.Acos(d / (Math.Sqrt(e1) * Math.Sqrt(e2)));
        }

        /// <summary>
        /// List Wrapper for Atom.Distance with IList to fit the delegate
        /// </summary>
        /// <param name="atoms"></param>
        /// <returns></returns>
        public static double Distance(IList<Atom> atoms)
        {
            if (atoms.Count() != 2) return 0;
            return Atom.Distance(atoms[0], atoms[1]);
        }
    }
}

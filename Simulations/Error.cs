using PorphyStruct.Chemistry;
using System;
using System.Collections.Generic;

namespace PorphyStruct.Simulations
{
    public static class Error
    {

        /// <summary>
        /// returns the error of two doubles
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        public static double FromDouble(double x1, double x2)
        {
            return Math.Pow(x1 - x2, 2);
        }

        /// <summary>
        /// returns the error of two arrays
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static double FromArray(double[] arr1, double[] arr2)
        {
            double error = 0;
            if (arr1.Length == arr2.Length)
            {
                for (int i = 0; i < arr2.Length; i++)
                {
                    error += FromDouble(arr1[i], arr2[i]);
                }
            }
            else return double.PositiveInfinity;
            return Math.Sqrt(error) / arr2.Length;
        }

        /// <summary>
        /// returns the error of two AtomDataPoints
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        public static double FromDataPoint(AtomDataPoint adp1, AtomDataPoint adp2)
        {
            return FromDouble(adp1.Y, adp2.Y);
        }

        /// <summary>
        /// Returns the error of to Datapoint lists
        /// </summary>
        /// <param name="al1"></param>
        /// <param name="al2"></param>
        /// <returns></returns>
        public static double FromDataPointList(List<AtomDataPoint> al1, List<AtomDataPoint> al2)
        {
            double error = 0;
            if (al1.Count == al2.Count)
            {
                for (int i = 0; i < al2.Count; i++)
                {
                    error += FromDataPoint(al1[i], al2[i]);
                }
            }
            else return double.PositiveInfinity;
            return Math.Sqrt(error) / al1.Count;
        }

        /// <summary>
        /// Returns the error of two macrocycles
        /// </summary>
        /// <param name="cy1"></param>
        /// <param name="cy2"></param>
        /// <returns></returns>
        public static double FromCycle(Macrocycle cy1, Macrocycle cy2)
        {
            return FromDataPointList(cy1.dataPoints, cy2.dataPoints);
        }

    }
}

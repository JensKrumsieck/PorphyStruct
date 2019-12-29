using System;
using System.Collections.Generic;
using System.Linq;

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
        public static double FromArray(IEnumerable<double> arr1, IEnumerable<double> arr2)
        {
            //if unequal lenght->infinity
            if (arr1.Count() != arr2.Count())
                return double.PositiveInfinity;
            return Math.Sqrt(arr1.Zip(arr2, (a, b) => FromDouble(a, b)).Sum()) / arr1.Count();
        }
    }
}

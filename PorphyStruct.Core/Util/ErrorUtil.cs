using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Util
{
    public static class ErrorUtil
    {

        /// <summary>
        /// returns the error of two doubles
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        public static double ErrorFromDouble(double x1, double x2)
        {
            return Math.Pow(x1 - x2, 2);
        }

        /// <summary>
        /// returns the error of two IEnumerable<double>s
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static double ErrorFromArray(IEnumerable<double> arr1, IEnumerable<double> arr2)
        {
            //if unequal lenght->infinity
            if (arr1.Count() != arr2.Count())
                return double.PositiveInfinity;
            return Math.Sqrt(arr1.Zip(arr2, (a, b) => ErrorFromDouble(a, b)).Sum()) / arr1.Count();
        }
    }
}

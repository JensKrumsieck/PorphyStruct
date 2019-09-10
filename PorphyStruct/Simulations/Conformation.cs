using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Chemistry;
using System;
using System.Collections.Generic;

namespace PorphyStruct.Simulations
{
    static class Conformation
    {

        /// <summary>
        /// Caclulate the resulting conformation from given parameters
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Result Calculate(Macrocycle cycle, double[] param)
        {
            Matrix<double> D = Displacements.DisplacementMatrix(cycle.type, param.Length);
            Vector<double> C = D * DenseVector.OfArray(param);
            //normalize conformation vector
            C = Normalize(C);

            double[] data = YFromCycle(cycle);

            return new Result(C.ToArray(), param, new double[] {
                Error.FromArray(data, C.ToArray()),
                Error.FromArray(Derive(cycle.dataPoints), Derive(GetAtomDataPoints(C.ToArray(), cycle.dataPoints))),
                Error.FromArray(Integrate(data), Integrate(C.ToArray()))
                });
        }

        /// <summary>
        /// get ydata from cycle
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        private static double[] YFromCycle(Macrocycle cycle)
        {
            double[] y = new double[cycle.dataPoints.Count];
            for (int i = 0; i < cycle.dataPoints.Count; i++)
            {
                y[i] = cycle.dataPoints[i].Y;
            }
            return y;
        }

        /// <summary>
		/// Normalizes a Vector
		/// </summary>
		/// <param name="v">The Vector</param>
		/// <returns>Normalized Vector</returns>
		private static Vector<double> Normalize(Vector<double> v)
        {
            //normalize conformation
            //find min & max
            double min = 0;
            double max = 0;
            foreach (double d in v)
            {
                if (d < min)
                    min = d;
                if (d > max)
                    max = d;
            }
            double fac;
            if (Math.Abs(max) > Math.Abs(min))
                fac = Math.Abs(max);
            else
                fac = Math.Abs(min);
            return v / fac;
        }

        // <summary>
        /// Calculate the Derivative of given DataPoints
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static double[] Derive(List<AtomDataPoint> data)
        {
            double[] derivative = new double[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                if (i != 0)
                {
                    derivative[i] = (data[i].Y - data[i - 1].Y) / (data[i].X - data[i - 1].X);
                }
                else
                    derivative[i] = 0;
            }
            return derivative;
        }

        /// <summary>
		/// Calculate the integral of given DataPoints
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static double[] Integrate(double[] data)
        {
            double[] integral = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                if (i != 0)
                {
                    integral[i] = integral[i - 1] + data[i];
                }
                else
                    integral[i] = data[i];
            }
            return integral;
        }

        /// <summary>
        /// Get DataPoints from double to calc deriv/integral
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datapoints"></param>
        /// <returns></returns>
        private static List<AtomDataPoint> GetAtomDataPoints(double[] data, List<AtomDataPoint> datapoints)
        {
            List<AtomDataPoint> list = new List<AtomDataPoint>();
            for (int i = 0; i < data.Length; i++)
            {
                list.Add(new AtomDataPoint(datapoints[i].X, data[i], datapoints[i].atom));
            }
            return list;
        }
    }
}

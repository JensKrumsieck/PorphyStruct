using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Chemistry;
using PorphyStruct.Util;
using System;

namespace PorphyStruct.Simulations
{
    public class Result
    {
        public double[] Conformation, Coefficients;
        public double[] Error;

        /// <summary>
        /// Construct Result
        /// </summary>
        /// <param name="Conformation"></param>
        /// <param name="Coefficients"></param>
        /// <param name="Error"></param>
        public Result(double[] Conformation, double[] Coefficients, double[] Error)
        {
            this.Conformation = Conformation;
            this.Coefficients = Coefficients;
            this.Error = Error;
        }

        internal double MeanDisplacement()
        {
            double sum = 0;
            foreach (double d in Conformation)
            {
                sum += Math.Pow(d, 2);
            }
            return Math.Sqrt(sum);
        }

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
            var c = C.Normalize(double.PositiveInfinity).ToArray();

            double[] data = cycle.dataPoints.ToDoubleArray();

            return new Result(c, param, new double[] {
                Simulations.Error.FromArray(data, c),
                Simulations.Error.FromArray(cycle.dataPoints.Derive(), c.ToAtomDataPoints(cycle.dataPoints).Derive()),
                Simulations.Error.FromArray(data.Integrate(), c.Integrate())
                });
        }
    }
}

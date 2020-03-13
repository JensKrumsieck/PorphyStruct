using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Chemistry;
using PorphyStruct.Util;

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
                ErrorUtil.ErrorFromArray(data, c),
                ErrorUtil.ErrorFromArray(cycle.dataPoints.Derive(), c.ToAtomDataPoints(cycle.dataPoints).Derive()),
                ErrorUtil.ErrorFromArray(data.Integrate(), c.Integrate())
                });
        }
    }
}

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Chemistry;
using PorphyStruct.Util;

namespace PorphyStruct.Simulations
{
    public static class Conformation
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
            var c = C.Normalize(double.PositiveInfinity).ToArray();

            double[] data = cycle.dataPoints.ToDoubleArray();

            return new Result(c, param, new double[] {
                Error.FromArray(data, c),
                Error.FromArray(cycle.dataPoints.Derive(), c.ToAtomDataPoints(cycle.dataPoints).Derive()),
                Error.FromArray(data.Integrate(), c.Integrate())
                });
        }
    }
}

using ChemSharp.Mathematics;
using ChemSharp.Molecules.DataProviders;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Core.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PorphyStruct.Core.Analysis.Properties
{
    public class Simulation
    {
        private readonly List<string> _modes = new List<string> { "Doming", "Saddling", "Ruffling", "WavingX", "WavingY", "Propellering" };

        public Matrix<double> ReferenceMatrix { get; private set; }

        /// <summary>
        /// Creates a Simulation Object for given Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="minimal">for Porphyrins only - will reduce basis by Waving 2 to fit Shelnutt et. al.</param>
        public Simulation(MacrocycleType type)
        {
            var typePrefix = $"PorphyStruct.Core.Reference.{type.ToString()}.";
            ReferenceMatrix = DisplacementMatrix(_modes.Select(s => typePrefix + s + ".xyz"), type);
        }

        /// <summary>
        /// Calculates Simulation for given Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] Simulate(double[] data) => ReferenceMatrix.QR().Solve(DenseVector.OfArray(data)).ToArray();

        /// <summary>
        /// Build Displacement Matrix from Array of Paths (in Resources)
        /// </summary>
        /// <param name="res"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Matrix<double> DisplacementMatrix(IEnumerable<string> res, MacrocycleType type)
        {
            var mat = new List<double[]>();
            foreach (var s in res)
            {
                var stream = ResourceUtil.LoadResource(s);
                var xyz = new XYZDataProvider(stream);
                var cycle = new Macrocycle(xyz) { MacrocycleType = type };
                Task.Run(cycle.Detect).Wait(500);
                var part = cycle.DetectedParts[0];
                var data = part.DataPoints.OrderBy(d => d.X).Select(d => d.Y).ToArray();
                mat.Add(data.Normalize());
            }
            return Matrix.Build.DenseOfColumnArrays(mat);
        }
    }
}

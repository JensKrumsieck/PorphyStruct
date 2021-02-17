using ChemSharp.Molecules.DataProviders;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChemSharp.Mathematics;

namespace PorphyStruct.Analysis
{
    public class Simulation
    {
        private readonly List<(string name, string path)> PorphyrinPaths = new List<(string name, string path)>()
        {
            ("Doming", "PorphyStruct.Reference.Porphyrin.Doming.xyz"),
            ("Saddling", "PorphyStruct.Reference.Porphyrin.Saddling.xyz"),
            ("Ruffling", "PorphyStruct.Reference.Porphyrin.Ruffling.xyz"),
            ("WavingX", "PorphyStruct.Reference.Porphyrin.WavingX.xyz"),
            ("WavingY", "PorphyStruct.Reference.Porphyrin.WavingY.xyz"),
            ("Waving2X", "PorphyStruct.Reference.Porphyrin.Waving2X.xyz"),
            ("Waving2Y", "PorphyStruct.Reference.Porphyrin.Waving2Y.xyz"),
            ("Propellering", "PorphyStruct.Reference.Porphyrin.Propellering.xyz")
        };


        public Matrix<double> ReferenceMatrix { get; private set; }

        /// <summary>
        /// Creates a Simulation Object for given Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="minimal">for Porphyrins only - will reduce basis by Waving 2 to fit Shelnutt et. al.</param>
        public Simulation(MacrocycleType type, bool minimal = false)
        {
            if (type != MacrocycleType.Porphyrin) return;
            BuildPorphyrinMatrix(minimal);
        }

        public double[] Simulate(double[] data) => ReferenceMatrix.Svd().Solve(DenseVector.OfArray(data)).ToArray();

        private Matrix<double> DisplacementMatrix(IEnumerable<string> res)
        {
            var mat = new List<double[]>();
            foreach (var s in res)
            {
                var stream = ResourceUtil.LoadResource(s);
                var xyz = new XYZDataProvider(stream);
                var cycle = new Macrocycle(xyz);
                Task.Run(cycle.Detect).Wait(500);
                var part = cycle.DetectedParts[0];
                var data = part.DataPoints.OrderBy(d => d.X).Select(d => d.Y).ToArray();
                mat.Add(data.Normalize());
            }
            return Matrix.Build.DenseOfColumnArrays(mat);
        }

        private void BuildPorphyrinMatrix(bool minimal = false)
        {
            ReferenceMatrix = DisplacementMatrix(PorphyrinPaths.Select(s => s.path));
            if (minimal)
                ReferenceMatrix =
                    DisplacementMatrix(PorphyrinPaths.Where(s => !s.name.Contains("2")).Select(s => s.path));
        }
    }
}

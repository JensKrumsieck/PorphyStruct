using System;
using ChemSharp.Molecules.DataProviders;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Extension;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace PorphyStruct.Analysis
{
    public class Simulation
    {
        private const string PorphyrinDomingPath = "PorphyStruct.Reference.Porphyrin.Doming.xyz";
        private const string PorphyrinSaddlingPath = "PorphyStruct.Reference.Porphyrin.Saddling.xyz";
        private const string PorphyrinRufflingPath = "PorphyStruct.Reference.Porphyrin.Ruffling.xyz";
        private const string PorphyrinWavingXPath = "PorphyStruct.Reference.Porphyrin.WavingX.xyz";
        private const string PorphyrinWavingYPath = "PorphyStruct.Reference.Porphyrin.WavingY.xyz";
        private const string PorphyrinWaving2XPath = "PorphyStruct.Reference.Porphyrin.Waving2X.xyz";
        private const string PorphyrinWaving2YPath = "PorphyStruct.Reference.Porphyrin.Waving2Y.xyz";
        private const string PorphyrinPropelleringPath = "PorphyStruct.Reference.Porphyrin.Propellering.xyz";

        public Matrix<double> ReferenceMatrix { get; }

        public Simulation(MacrocycleType type)
        {
            if (type != MacrocycleType.Porphyrin) return;
            ReferenceMatrix = DisplacementMatrix(new[]
            {
                PorphyrinDomingPath, PorphyrinSaddlingPath, PorphyrinRufflingPath,
                PorphyrinWavingXPath, PorphyrinWavingYPath, 
                PorphyrinPropelleringPath
            });
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
                var data = part.DataPoints.Select(d => d.Y).ToArray();
                mat.Add(data);
            }
            return Matrix.Build.DenseOfColumnArrays(mat);
        }

        public static double[] Normalize(double[] data)
        {
            var min = Math.Abs(data.Min());
            var max = Math.Abs(data.Max());
            return data.Select(s => s / (min < max ? max : min)).ToArray();
        }
    }
}

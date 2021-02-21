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
        private readonly MacrocycleType _type;

        private readonly List<string> _modes = new List<string>
            {"Doming", "Saddling", "Ruffling", "WavingX", "WavingY", "Propellering"};

        /// <summary>
        /// Matrix containing reference values
        /// </summary>
        public Matrix<double> ReferenceMatrix { get; }

        /// <summary>
        /// Result of Simulation, may be empty
        /// </summary>
        public List<(string mode, double value)> SimulationResult { get; } = new List<(string, double)>();

        /// <summary>
        /// Creates a Simulation Object for given Type
        /// </summary>
        /// <param name="type"></param>
        public Simulation(MacrocycleType type)
        {
            _type = type;
            var typePrefix = $"PorphyStruct.Core.Reference.{_type}.";
            ReferenceMatrix = DisplacementMatrix(_modes.Select(s => typePrefix + s + ".xyz"));
        }

        /// <summary>
        /// Calculates Simulation for given Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] Simulate(double[] data)
        {
            SimulationResult.Clear();
            var result = ReferenceMatrix.QR().Solve(DenseVector.OfArray(data)).ToArray();
            for (var i = 0; i < _modes.Count; i++) SimulationResult.Add((_modes[i], result[i]));
            return result;
        }

        /// <summary>
        /// Returns simulated Conformation Y Values
        /// </summary>
        public List<double> ConformationY => (ReferenceMatrix * DenseVector.OfEnumerable(SimulationResult.Select(s => s.value))).ToList();

        public double OutOfPlaneParameter => ConformationY.Length();

        /// <summary>
        /// Build Displacement Matrix from Array of Paths (in Resources)
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private Matrix<double> DisplacementMatrix(IEnumerable<string> res)
        {
            var mat = new List<double[]>();
            foreach (var s in res)
            {
                var stream = ResourceUtil.LoadResource(s);
                var xyz = new XYZDataProvider(stream);
                var cycle = new Macrocycle(xyz) { MacrocycleType = _type };
                Task.Run(cycle.Detect).Wait(1500);
                var part = cycle.DetectedParts[0];
                var data = part.DataPoints.OrderBy(d => d.X).Select(d => d.Y).ToArray();
                mat.Add(data.Normalize());
            }
            return Matrix.Build.DenseOfColumnArrays(mat);
        }

        public override string ToString()
        {
            var result = $"Simulation ({_type})\n";
            foreach (var (mode, value) in SimulationResult)
                result += $"{mode}: {value:N4} Å\n";
            if (ConformationY.Any()) result += $"Doop: {OutOfPlaneParameter:N4}";
            return result;
        }
    }
}

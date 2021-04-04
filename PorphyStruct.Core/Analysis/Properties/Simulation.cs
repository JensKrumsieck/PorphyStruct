using System;
using ChemSharp.Mathematics;
using ChemSharp.Molecules.DataProviders;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Core.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PorphyStruct.Core.Analysis.Properties
{
    public class Simulation
    {
        [JsonIgnore]
        internal readonly MacrocycleType Type;

        private static readonly List<string> Modes = new List<string>
            {"Doming", "Saddling", "Ruffling", "WavingX", "WavingY", "Propellering"};

        private static readonly List<string> ExtendedModes = Modes.Select(s => s + "2").ToList();
        [JsonIgnore]
        internal readonly List<string> UsedModes;

        /// <summary>
        /// Matrix containing reference values
        /// </summary>
        [JsonIgnore]
        public Matrix<double> ReferenceMatrix { get; private set; }

        /// <summary>
        /// Result of Simulation, may be empty
        /// </summary>
        public List<KeyValueProperty> SimulationResult { get; } = new List<KeyValueProperty>();

        /// <summary>
        /// Creates a Simulation Object for given Type
        /// </summary>
        /// <param name="type"></param>
        public Simulation(MacrocycleType type)
        {
            UsedModes = Settings.Instance.UseExtendedBasis ? Modes.Concat(ExtendedModes).ToList() : Modes;
            if (type == MacrocycleType.Porphyrin || type == MacrocycleType.Norcorrole) UsedModes.Remove("WavingY2");//Por: WavingX2 = -WavingY2 -> linear dependent! //Nor: Only one Waving2 Mode could be found
            Type = type;

        }

        public static async Task<Simulation> CreateAsync(MacrocycleType type)
        {
            var sim = new Simulation(type);
            var typePrefix = $"PorphyStruct.Core.Reference.{sim.Type}.";
            sim.ReferenceMatrix = await DisplacementMatrix(sim.UsedModes.Select(s => typePrefix + s + ".xyz"), type);
            return sim;
        }

        /// <summary>
        /// Calculates Simulation for given Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] Simulate(double[] data)
        {
            SimulationResult.Clear();
            var matrix = ReferenceMatrix;
            if (Type == MacrocycleType.Porphyrin)
            {
                matrix = matrix.RemoveRow(matrix.RowCount - 1);
                data = data.RemoveLast().ToArray();
            }

            var result = matrix.QR().Solve(DenseVector.OfArray(data)).ToArray();

            for (var i = 0; i < UsedModes.Count; i++) SimulationResult.Add(new KeyValueProperty { Key = UsedModes[i], Value = result[i], Unit = "Å" });
            OutOfPlaneParameter.Value = Type == MacrocycleType.Porphyrin ? ConformationY.RemoveLast().Length(): ConformationY.Length();
            return result;
        }

        /// <summary>
        /// Returns simulated Conformation Y Values
        /// </summary>
        public List<double> ConformationY => (ReferenceMatrix * DenseVector.OfEnumerable(SimulationResult.Select(s => s.Value))).ToList();

        public KeyValueProperty OutOfPlaneParameter { get; set; } = new KeyValueProperty { Key = "Doop (sim.)", Unit = "Å" };

        /// <summary>
        /// Build Displacement Matrix from Array of Paths (in Resources)
        /// </summary>
        /// <param name="res"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<Matrix<double>> DisplacementMatrix(IEnumerable<string> res, MacrocycleType type)
        {
            var mat = new List<double[]>();
            foreach (var s in res)
            {
                var stream = ResourceUtil.LoadResource(s);
                var xyz = new XYZDataProvider(stream);
                var cycle = new Macrocycle(xyz) { MacrocycleType = type };
                await Task.Run(cycle.Detect);
                var part = cycle.DetectedParts[0];
                var data = part.DataPoints.OrderBy(d => d.X).Select(d => d.Y).ToArray();
                mat.Add(data.Normalize());
            }
            return Matrix.Build.DenseOfColumnArrays(mat);
        }

        public override string ToString()
        {
            var result = $"Simulation ({Type})\n";
            result = SimulationResult.Aggregate(result, (current, prop) => current + prop + "\n");
            if (ConformationY.Any()) result += OutOfPlaneParameter.ToString();
            return result;
        }
    }
}

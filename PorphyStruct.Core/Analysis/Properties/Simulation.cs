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
        private readonly MacrocycleType _type;

        private static readonly List<string> Modes = new List<string>
            {"Doming", "Saddling", "Ruffling", "WavingX", "WavingY", "Propellering"};

        private static readonly List<string> ExtendedModes = Modes.Select(s => s + "2").ToList();

        private readonly List<string> _usedModes;

        /// <summary>
        /// Matrix containing reference values
        /// </summary>
        [JsonIgnore]
        public Matrix<double> ReferenceMatrix { get; }

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
            _usedModes = Settings.Instance.UseExtendedBasis ? Modes.Concat(ExtendedModes).ToList() : Modes;
            if (type == MacrocycleType.Porphyrin || type == MacrocycleType.Norcorrole) _usedModes.Remove("WavingY2");//Por: WavingX2 = -WavingY2 -> linear dependent! //Nor: Only one Waving2 Mode could be found
            _type = type;
            var typePrefix = $"PorphyStruct.Core.Reference.{_type}.";
            ReferenceMatrix = DisplacementMatrix(_usedModes.Select(s => typePrefix + s + ".xyz"), type);
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

            for (var i = 0; i < _usedModes.Count; i++) SimulationResult.Add(new KeyValueProperty { Key = _usedModes[i], Value = result[i], Unit = "Å" });
            OutOfPlaneParameter.Value = ConformationY.Length();
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
        /// <returns></returns>
        public static Matrix<double> DisplacementMatrix(IEnumerable<string> res, MacrocycleType type)
        {
            var mat = new List<double[]>();
            foreach (var s in res)
            {
                var stream = ResourceUtil.LoadResource(s);
                var xyz = new XYZDataProvider(stream);
                var cycle = new Macrocycle(xyz) { MacrocycleType = type };
                Task.Run(cycle.Detect).Wait(15000);
                var part = cycle.DetectedParts[0];
                var data = part.DataPoints.OrderBy(d => d.X).Select(d => d.Y).ToArray();
                mat.Add(data.Normalize());
            }
            return Matrix.Build.DenseOfColumnArrays(mat);
        }

        public override string ToString()
        {
            var result = $"Simulation ({_type})\n";
            result = SimulationResult.Aggregate(result, (current, prop) => current + prop + "\n");
            if (ConformationY.Any()) result += OutOfPlaneParameter.ToString();
            return result;
        }
    }
}

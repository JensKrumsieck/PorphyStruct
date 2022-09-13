using System.Text.Json.Serialization;
using ChemSharp.Mathematics;
using ChemSharp.Molecules;
using ChemSharp.Molecules.DataProviders;
using ChemSharp.Molecules.Properties;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Core.Extension;

namespace PorphyStruct.Core.Analysis.Properties;

public class Simulation
{
    [JsonIgnore]
    internal readonly MacrocycleType Type;

    private static readonly List<string> Modes = new() { "Doming", "Saddling", "Ruffling", "WavingX", "WavingY", "Propellering" };

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
    public List<KeyValueProperty> SimulationResult { get; } = new();

    /// <summary>
    /// Creates a Simulation Object for given Type
    /// </summary>
    /// <param name="type"></param>
    public Simulation(MacrocycleType type)
    {
        UsedModes = Settings.Instance.UseExtendedBasis ? Modes.Concat(ExtendedModes).ToList() : Modes;
        if (type == MacrocycleType.Porphyrin || type == MacrocycleType.Norcorrole) UsedModes.Remove("WavingY2");//Por: WavingX2 = -WavingY2 -> linear dependent! //Nor: Only one Waving2 Mode could be found
        Type = type;
        var typePrefix = $"PorphyStruct.Core.Reference.{Type}.";
        ReferenceMatrix = DisplacementMatrix(UsedModes.Select(s => typePrefix + s + ".mol2"), type);
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
        OutOfPlaneParameter.Value = Type == MacrocycleType.Porphyrin ? ConformationY.RemoveLast().Length() : ConformationY.Length();
        return result;
    }

    /// <summary>
    /// Returns simulated Conformation Y Values
    /// </summary>
    public List<double> ConformationY => (ReferenceMatrix * DenseVector.OfEnumerable(SimulationResult.Select(s => s.Value))).ToList();

    public KeyValueProperty OutOfPlaneParameter { get; set; } = new() { Key = "Doop (sim.)", Unit = "Å" };

    /// <summary>
    /// Gets Parameters as Percentage
    /// </summary>
    public List<KeyValueProperty> SimulationResultPercentage
    {
        get => SimulationResult.Select(s =>
                                new KeyValueProperty()
                                {
                                    Value = Math.Abs(s.Value) / SimulationResult.Sum(v => Math.Abs(v.Value)) * 100,
                                    Key = s.Key,
                                    Unit = " %"
                                }).ToList();
    }

    /// <summary>
    /// Build Displacement Matrix from Array of Paths (in Resources)
    /// </summary>
    /// <param name="res"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Matrix<double> DisplacementMatrix(IEnumerable<string> res, MacrocycleType type)
    {
        var mat = new List<double[]>();
        foreach (var s in res)
        {
            var stream = ResourceUtil.LoadResource(s);
            var cycle = new Macrocycle(stream!, s.Split('.').Last());
            var atoms = cycle.Atoms.Where(a => a.IsNonCoordinative()).ToList();
            var bonds = cycle.Bonds.Where(b => atoms.Contains(b.Atom1) && atoms.Contains(b.Atom2));
            //await Task.Run(cycle.Detect);
            cycle.DetectedParts.Add(MacrocycleAnalysis.Create(new Molecule(atoms, bonds), type));
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

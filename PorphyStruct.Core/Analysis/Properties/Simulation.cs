﻿using System.Text.Json.Serialization;
using ChemSharp.Mathematics;
using ChemSharp.Molecules;
using ChemSharp.Molecules;
using ChemSharp.Molecules.Properties;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Core.Extension;

namespace PorphyStruct.Core.Analysis.Properties;

public class Simulation
{
    [JsonIgnore] internal readonly MacrocycleType Type;

    private static readonly List<string> Modes = new()
        {"Doming", "Saddling", "Ruffling", "WavingX", "WavingY", "Propellering"};

    private static readonly List<string> ExtendedModes = Modes.Select(s => s + "2").ToList();
    [JsonIgnore] internal readonly List<string> UsedModes;

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
        if (type == MacrocycleType.Porphyrin || type == MacrocycleType.Norcorrole)
            UsedModes.Remove("WavingY2"); //Por: WavingX2 = -WavingY2 -> linear dependent! //Nor: Only one Waving2 Mode could be found
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

        for (var i = 0; i < UsedModes.Count; i++)
            SimulationResult.Add(new KeyValueProperty {Key = UsedModes[i], Value = result[i], Unit = "Å"});
        OutOfPlaneParameter.Value = Type == MacrocycleType.Porphyrin
            ? ConformationY.RemoveLast().Length()
            : ConformationY.Length();
        return result;
    }

    /// <summary>
    /// Returns simulated Conformation Y Values
    /// </summary>
    public List<double> ConformationY =>
        (ReferenceMatrix * DenseVector.OfEnumerable(SimulationResult.Select(s => s.Value))).ToList();

    public KeyValueProperty OutOfPlaneParameter { get; set; } = new() {Key = "Doop (sim.)", Unit = "Å"};

    /// <summary>
    /// Gets Parameters as Percentage
    /// </summary>
    public List<KeyValueProperty> SimulationResultPercentage
    {
        get => SimulationResult.Select(s =>
                                           new KeyValueProperty()
                                           {
                                               Value = s.Value != 0
                                                   ? Math.Abs(s.Value) / SimulationResult.Sum(v => Math.Abs(v.Value)) *
                                                     100
                                                   : 0,
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
            var atoms = cycle.Atoms.Where(a => a.CanBeRingMember()).ToList();
            var bonds = cycle.Bonds.Where(b => atoms.Contains(b.Atom1) && atoms.Contains(b.Atom2));
            //await Task.Run(cycle.Detect);
            var mapping = new Dictionary<string, Atom>();
            for (var i = 0; i < atoms.Count; i++) 
                mapping.Add(atoms[i].Title, atoms[i]);
            cycle.DetectedParts.Add(MacrocycleAnalysis.Create(new Molecule(atoms, bonds), mapping, type));
            var part = cycle.DetectedParts[0];
            var data = EnforceDirection(part, s);
            mat.Add(data.Normalize());
        }

        return Matrix.Build.DenseOfColumnArrays(mat);
    }

    private static double[] EnforceDirection(MacrocycleAnalysis analysis, string mode)
    {
        var data = analysis.DataPoints.OrderBy(d => d.X).Select(d => d.Y).ToArray();

        if (
            //fix waving and doming modes to be N1 positive
            ((mode.Contains("Doming") || mode.Contains("Waving")) &&
             analysis.DataPoints.First(s => s.Atom.Title == "N1").Y < 0)
            ||
            //fix ruffling modes to be C5 positive
            (mode.Contains("Ruffling") && 
             analysis.DataPoints.First(s => s.Atom.Title == "C5").Y < 0)
            ||
            //fix pro modes to be C2 positive
            (mode.Contains("Propellering") && 
             analysis.DataPoints.First(s => s.Atom.Title == "C2").Y < 0)
            ||
            //fix saddling so than N facing downwards, therefore c2 is higher than n1
            (mode.Contains("Saddling") &&
             analysis.DataPoints.First(s => s.Atom.Title == "C2").Y < analysis.DataPoints.First(s => s.Atom.Title=="N1").Y)
        )
            return data.Select(i => -i).ToArray();
        return data;
    }

    public override string ToString()
    {
        var result = $"Simulation ({Type})\n";
        result = SimulationResult.Aggregate(result, (current, prop) => current + prop + "\n");
        if (ConformationY.Any()) result += OutOfPlaneParameter.ToString();
        return result;
    }
}
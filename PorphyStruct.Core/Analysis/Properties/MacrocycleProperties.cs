﻿using System.Text.Json;
using System.Text.Json.Serialization;
using ChemSharp.Mathematics;
using ChemSharp.Molecules.Mathematics;
using ChemSharp.Molecules.Properties;
using PorphyStruct.Core.Extension;

namespace PorphyStruct.Core.Analysis.Properties;

public sealed class MacrocycleProperties
{
    [JsonIgnore] public readonly MacrocycleAnalysis Analysis;

    [JsonIgnore] public double Delta => Math.Abs(OutOfPlaneParameter.Value - Simulation.OutOfPlaneParameter.Value);

    [JsonIgnore] public double Error => Delta / OutOfPlaneParameter.Value;

    public string AnalysisType => Analysis.Type.ToString();
    public KeyValueProperty OutOfPlaneParameter { get; } = new() {Key = "Doop (exp.)", Unit = "Å"};
    public Simulation? Simulation { get; private set; }
    public List<Dihedral> Dihedrals { get; } = new();
    public List<Angle> Angles { get; } = new();
    public List<Distance> Distances { get; } = new();
    public List<PlaneDistance> PlaneDistances { get; } = new();
    public N4Cavity Cavity { get; private set; }
    public KeyValueProperty InterplanarAngle { get; } = new() {Unit = "°"};


    public MacrocycleProperties(MacrocycleAnalysis analysis)
    {
        Analysis = analysis;
        Rebuild();
    }

    /// <summary>
    /// Dihedrals for Corrole, Porphyrin and Norcorrole
    /// </summary>
    [JsonIgnore] private static readonly IList<string[]> PorphyrinoidDihedrals = new List<string[]>
    {
        new[] {"C3", "C4", "C6", "C7"}, //chi1
        new[] {"C2", "C1", "C19", "C18"}, //chi2
        new[] {"C13", "C14", "C16", "C17"}, //chi3 
        new[] {"C8", "C9", "C11", "C12"}, //chi4
        new[] {"C4", "N1", "N3", "C11"}, //psi1
        new[] {"C9", "N2", "N4", "C16"}, //psi2
        new[] {"N1", "C4", "C6", "N2"}, //phi1
        new[] {"N1", "C1", "C19", "N4"}, //phi2
        new[] {"N3", "C14", "C16", "N4"}, //phi3
        new[] {"N2", "C9", "C11", "N3"} //phi4
    };

    [JsonIgnore] private static readonly IList<string[]> CorrphyceneDihedrals = new List<string[]>
    {
        new[] {"C3", "C4", "C6", "C7"}, //chi1
        new[] {"C2", "C1", "C20", "C19"}, //chi2
        new[] {"C14", "C15", "C17", "C18"}, //chi3
        new[] {"C8", "C9", "C12", "C13"}, //chi4
        new[] {"C4", "N1", "N3", "C12"}, //psi1
        new[] {"C9", "N2", "N4", "C17"}, //psi2
        new[] {"N1", "C4", "C6", "N2"}, //phi1
        new[] {"N1", "C1", "C20", "N4"}, //phi2
        new[] {"N3", "C14", "C17", "N4"}, //phi3
        new[] {"N2", "C9", "C11", "N3"} //phi4
    };

    [JsonIgnore] private static readonly IList<string[]> PorphyceneDihedrals = new List<string[]>
    {
        new[] {"C3", "C4", "C7", "C8"}, //chi1
        new[] {"C2", "C1", "C20", "C19"}, //chi2
        new[] {"C13", "C14", "C17", "C18"}, //chi3
        new[] {"C9", "C10", "C11", "C12"}, //chi4
        new[] {"C4", "N1", "N3", "C11"}, //psi1
        new[] {"C10", "N2", "N4", "C17"}, //psi2
        new[] {"N1", "C4", "C7", "N2"}, //phi1
        new[] {"N1", "C1", "C20", "N4"}, //phi2
        new[] {"N3", "C14", "C17", "N4"}, //phi3
        new[] {"N2", "C10", "C11", "N3"} //phi4
    };

    /// <summary>
    /// Fills all Lists
    /// </summary>
    public void Rebuild()
    {
        Simulation ??= new Simulation(Analysis.GetAnalysisType());
        if (Analysis.DataPoints.Any())
            Simulation?.Simulate(Analysis.DataPoints.OrderBy(s => s.X).Select(s => s.Y).ToArray());
        OutOfPlaneParameter.Value = Analysis.GetAnalysisType() == MacrocycleType.Porphyrin
            ? Analysis.DataPoints.OrderBy(s => s.X).RemoveLast().DisplacementValue()
            : Analysis.DataPoints.OrderBy(s => s.X).DisplacementValue();

        RebuildDihedrals();
        RebuildAngles();
        RebuildDistances();
        var n1 = Analysis.FindAtomByTitle("N1");
        var n2 = Analysis.FindAtomByTitle("N2");
        var n3 = Analysis.FindAtomByTitle("N3");
        var n4 = Analysis.FindAtomByTitle("N4");
        if (n1 == null | n2 == null | n3 == null | n4 == null) return;
        Cavity = new N4Cavity(n1!, n2!, n3!, n4!);
    }

    /// <summary>
    /// Rebuilds Dihedrals
    /// </summary>
    private void RebuildDihedrals()
    {
        Dihedrals.Clear();

        Dihedrals.Add(new Dihedral(Analysis.FindAtomByTitle("N1"), Analysis.FindAtomByTitle("N2"),
                                   Analysis.FindAtomByTitle("N3"), Analysis.FindAtomByTitle("N4")));
        RebuildTypeSpecificDihedrals();
    }

    /// <summary>
    /// Rebuilds Type Specific Dihedrals
    /// </summary>
    private void RebuildTypeSpecificDihedrals()
    {
        //add dihedrals from string list
        switch (Analysis)
        {
            case PorphyrinAnalysis _:
            case CorroleAnalysis _:
                BuildDihedrals(PorphyrinoidDihedrals);
                break;
            case CorrphyceneAnalysis _:
                BuildDihedrals(CorrphyceneDihedrals);
                break;
            case PorphyceneAnalysis _:
                BuildDihedrals(PorphyceneDihedrals);
                break;
        }
    }

    /// <summary>
    /// Business logic for <see cref="RebuildTypeSpecificDihedrals"/>
    /// </summary>
    /// <param name="list"></param>
    private void BuildDihedrals(IEnumerable<string[]> list) =>
        Dihedrals.AddRange(list.Select(s => new Dihedral(Analysis.FindAtomByTitle(s[0]),
                                                         Analysis.FindAtomByTitle(s[1]), Analysis.FindAtomByTitle(s[2]),
                                                         Analysis.FindAtomByTitle(s[3]))));

    /// <summary>
    /// Rebuilds Angles
    /// </summary>
    private void RebuildAngles()
    {
        Angles.Clear();
        RebuildMetalAngles();
        foreach (var m in Analysis.Meso)
        {
            var n = Analysis.Neighbors(m).ToArray();
            if (n.Length == 2) Angles.Add(new Angle(n[0], m, n[1]));
        }
    }

    private void RebuildMetalAngles()
    {
        InterplanarAngle.Value = 0;
        if (Analysis.Metal == null) return;
        Angles.Add(new Angle(Analysis.FindAtomByTitle("N1"), Analysis.Metal, Analysis.FindAtomByTitle("N4")));
        Angles.Add(new Angle(Analysis.FindAtomByTitle("N2"), Analysis.Metal, Analysis.FindAtomByTitle("N3")));
        InterplanarAngle.Key = $"[N1-{Analysis.Metal?.Title}-N4]x[N2-{Analysis.Metal?.Title}-N3]";
        var angle = Angles[0].PlaneAngle(Angles[1]);
        InterplanarAngle.Value = double.IsNaN(angle) ? 0 : angle;
    }

    /// <summary>
    /// Rebuilds Distances
    /// </summary>
    private void RebuildDistances()
    {
        Distances.Clear();
        PlaneDistances.Clear();

        Distances.Add(new Distance(Analysis.FindAtomByTitle("N1"), Analysis.FindAtomByTitle("N3")));
        Distances.Add(new Distance(Analysis.FindAtomByTitle("N2"), Analysis.FindAtomByTitle("N4")));
        if (Analysis.Metal == null) return;
        Distances.AddRange(Analysis.Atoms.Where(s => Analysis.Metal.BondToByCovalentRadii(s))
                                   .Select(s => new Distance(Analysis.Metal, s)));
        PlaneDistances.Add(new PlaneDistance(Analysis.Metal, Analysis.MeanPlane, "Mean Plane"));
        PlaneDistances.Add(new PlaneDistance(Analysis.Metal,
                                             MathV.MeanPlane(Analysis.N4Cavity.Select(s => s.Location).ToList()),
                                             "N4 Plane"));
    }

    /// <summary>
    /// Exports Properties in Machine readable format
    /// </summary>
    /// <returns></returns>
    public string ExportJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions {WriteIndented = true});

    /// <summary>
    /// Exports Properties in Human readable format
    /// </summary>
    /// <returns></returns>
    public string ExportString()
    {
        var result = "";
        result += $"## Macrocycle Type: {AnalysisType}\n";
        result += ExportBlock("Simulation",
                              Simulation.SimulationResult.Append(OutOfPlaneParameter)
                                        .Append(Simulation.OutOfPlaneParameter));
        result += ExportBlock("Simulation in %",
                              Simulation.SimulationResultPercentage);
        result += ExportBlock("Cavity", new[] {Cavity});
        result += Analysis.Metal != null
            ? ExportBlock("Distances", Distances.Cast<KeyValueProperty>().Concat(PlaneDistances))
            : ExportBlock("Distances", Distances);
        result += Analysis.Metal != null
            ? ExportBlock("Angles", Angles.Append(InterplanarAngle))
            : ExportBlock("Angles", Angles);
        result += ExportBlock("Dihedrals", Dihedrals);
        return result;
    }


    private static string ExportBlock(string title, IEnumerable<KeyValueProperty> content)
    {
        var result = $"### {title}\n";
        result = content.Aggregate(result, (current, prop) => current + ("* " + prop + "\n"));
        result += "\n";
        return result;
    }
}
﻿#nullable enable
using System.Numerics;
using ChemSharp.Mathematics;
using ChemSharp.Molecules;
using ChemSharp.Molecules.Extensions;
using Nodo.Pathfinding;
using PorphyStruct.Core.Analysis.Properties;
using PorphyStruct.Core.Extension;
using PorphyStruct.Core.Plot;

namespace PorphyStruct.Core.Analysis;

public abstract class MacrocycleAnalysis
{
    internal Molecule Molecule { get; }

    /// <summary>
    /// Selected Atoms for Analysis
    /// </summary>
    public List<Atom> Atoms => Molecule.Atoms;

    /// <summary>
    /// Selected Bonds for Analysis
    /// </summary>
    public IEnumerable<Bond> Bonds => Molecule.Bonds;

    private readonly Guid _guid;

    public Atom? Metal { get; set; }

    /// <summary>
    /// Color Representation for Analysis to use as indicator
    /// </summary>
    public string AnalysisColor => _guid.HexStringFromGuid();

    protected MacrocycleAnalysis(Molecule mol)
    {
        Molecule = mol;
        _guid = Guid.NewGuid();
        Alpha = Atoms.Where(IsAlpha).ToList();
        Beta = Atoms.Where(s => Neighbors(s).Count == 2 && Neighbors(s).Count(IsAlpha) == 1).ToList();
        N4Cavity = Atoms.Where(s => Neighbors(s).Where(IsAlpha).Count(n => Backtracking.BackTrack(n, s, Neighbors, 5).Count == 5) == 2)
            .ToList();
        Meso = Atoms.Except(Alpha).Except(Beta).Except(N4Cavity).ToList();
    }

    /// <summary>
    /// RingAtoms by Identifier
    /// </summary>
    protected abstract List<string> RingAtoms { get; }

    /// <summary>
    /// Alpha atoms of Macrocycle by Identifiers
    /// </summary>
    protected abstract string[] AlphaAtoms { get; }

    /// <summary>
    /// Multipliers for C-Atom positioning
    /// </summary>
    protected abstract Dictionary<string, double> Multiplier { get; }

    /// <summary>
    /// A Set of Macrocyclic Properties
    /// </summary>
    public MacrocycleProperties? Properties { get; set; }

    private IEnumerable<AtomDataPoint>? _dataPoints;
    /// <summary>
    /// Cached Datapoints
    /// </summary>
    public IEnumerable<AtomDataPoint> DataPoints => _dataPoints ??= CalculateDataPoints();

    /// <summary>
    /// Returns Mean Square Plane of Analysis Fragment
    /// </summary>
    public Plane MeanPlane => Atoms.Select(s => s.Location).ToList().MeanPlane();

    /// <summary>
    /// Returns Neighbors in context of analysis
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public List<Atom> Neighbors(Atom a) => Molecule.Neighbors(a);

    /// <summary>
    /// Generates DataPoints
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<AtomDataPoint> CalculateDataPoints()
    {
        var atoms = Atoms.OrderBy(s => RingAtoms.IndexOf(s.Title)).ToList();
        var dist = 0d;
        var fix = 1d;

        foreach (var a in atoms)
        {
            var coordX = 1d;
            if (a.Title.Contains('C')) coordX = fix + dist * Multiplier[a.Title];
            if (a.Title.Contains('N')) coordX = fix + dist / 2d;
            //starts with C1 which is alpha per definition, so refresh distance every alpha atom.
            if (IsAlpha(a) && NextAlpha(a) != null) dist = a.DistanceTo(NextAlpha(a));
            //alpha atoms are fix-points
            if (IsAlpha(a)) fix = coordX;
            yield return new AtomDataPoint(coordX, MeanPlane.Distance(a.Location), a);
        }
    }

    /// <summary>
    /// checks whether it's an alpha atom or not
    /// </summary>
    /// <param name="a"></param>
    /// <returns>boolean</returns>
    private bool IsAlpha(Atom a) => Neighbors(a).Count == 3;

    /// <summary>
    /// gets next alpha position for distance measuring
    /// </summary>
    /// <param name="a"></param>
    /// <returns>Atom</returns>
    private Atom? NextAlpha(Atom a)
    {
        var i = Array.IndexOf(AlphaAtoms, a.Title) + 1;
        return AlphaAtoms.Length > i ? FindAtomByTitle(AlphaAtoms[i]) : null;
    }

    /// <summary>
    /// An overrideable Method to get C1 Atom. 
    /// In a Porphyrin it does not care which alpha atom is C1, so return any of them...
    /// override this method in any other class
    /// </summary>
    /// <returns></returns>
    protected virtual Atom C1 => Atoms.First(a => Neighbors(a).Count == 3);

    /// <summary>
    /// Lists N4 Cavity
    /// </summary>
    public readonly List<Atom> N4Cavity;

    /// <summary>
    /// Lists all Beta Atoms
    /// </summary>
    public readonly List<Atom> Beta;

    /// <summary>
    /// Lists all alpha Atoms
    /// </summary>
    public readonly List<Atom> Alpha;

    /// <summary>
    /// Lists all meso Atoms
    /// </summary>
    public readonly List<Atom> Meso;

    /// <summary>
    /// calculate distance between two atoms (as identifiers are needed this must be in Macrocycle-Class!!)
    /// </summary>
    /// <param name="id1">Identifier 1</param>
    /// <param name="id2">Identifier 2</param>
    /// <returns>The Vector Distance</returns>
    protected double CalculateDistance(string id1, string id2) => Vector3.Distance(FindAtomByTitle(id1)!.Location, FindAtomByTitle(id2)!.Location);

    /// <summary>
    /// Gets DataPoints for Bonds
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<(AtomDataPoint a1, AtomDataPoint a2)> BondDataPoints() =>
        from bond in Bonds.Where(s => !s.Atoms.Select(a => a.Title).ScrambledEquals(new[] { RingAtoms.First(), RingAtoms.Last() })) //Do not draw bond between first and last element
        let start = DataPoints.FirstOrDefault(s => s.Atom.Equals(bond.Atom1))
        let end = DataPoints.FirstOrDefault(s => s.Atom.Equals(bond.Atom2))
        select (start, end);

    /// <summary>
    /// Creates Analysis Type
    /// </summary>
    /// <param name="mol"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static MacrocycleAnalysis Create(Molecule mol, MacrocycleType type)
    {
        MacrocycleAnalysis analysis = type switch
        {
            MacrocycleType.Porphyrin => new PorphyrinAnalysis(mol),
            MacrocycleType.Corrole => new CorroleAnalysis(mol),
            MacrocycleType.Norcorrole => new NorcorroleAnalysis(mol),
            MacrocycleType.Porphycene => new PorphyceneAnalysis(mol),
            MacrocycleType.Corrphycene => new CorrphyceneAnalysis(mol),
            _ => throw new InvalidOperationException()
        };
        return analysis;
    }
    
    /// <summary>
    /// Finds atom by Title
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    public Atom? FindAtomByTitle(string title) => Atoms.FirstOrDefault(s => s.Title == title) ?? null;
}

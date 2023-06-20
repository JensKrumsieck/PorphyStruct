#nullable enable
using System.Numerics;
using ChemSharp.Extensions;
using ChemSharp.Mathematics;
using ChemSharp.Molecules;
using Nodo.Pathfinding;
using PorphyStruct.Core.Analysis.Properties;
using PorphyStruct.Core.Extension;
using PorphyStruct.Core.Plot;

namespace PorphyStruct.Core.Analysis;

public abstract class MacrocycleAnalysis
{
    public abstract MacrocycleType Type { get; }
    
    internal Molecule Molecule { get; }
    
    internal Dictionary<string, Atom> _mapping;


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

    protected MacrocycleAnalysis(Molecule mol, Dictionary<string, Atom> mapping)
    {
        Molecule = mol;
        _mapping = mapping;
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

    public int GetMappingIndex(Atom a)
    {
        var key = _mapping.FirstOrDefault(m => m.Value == a).Key;
        if (key == null) return -1;
        return RingAtoms.IndexOf(key);
    }
    /// <summary>
    /// Generates DataPoints
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<AtomDataPoint> CalculateDataPoints()
    {
        Molecule.RebuildCache();
        var atoms = _mapping.OrderBy(s => RingAtoms.IndexOf(s.Key)).ToList();
        var dist = 0d;
        var fix = 1d;

        foreach (var a in atoms)
        {
            var coordX = 1d;
            if (a.Key.Contains('C')) coordX = fix + dist * Multiplier[a.Key];
            if (a.Key.Contains('N')) coordX = fix + dist / 2d;
            //starts with C1 which is alpha per definition, so refresh distance every alpha atom.
            if (IsAlpha(a.Value) && NextAlpha(a.Key) != null) dist = a.Value.DistanceTo(NextAlpha(a.Key)!);
            //alpha atoms are fix-points
            if (IsAlpha(a.Value)) fix = coordX;
            yield return new AtomDataPoint(coordX, MeanPlane.Distance(a.Value.Location), a.Value, tag: a.Key);
        }
    }

    public void InvertYDataPoints()
    {
        _dataPoints = _dataPoints!.Invert();
        Properties!.Rebuild();
    }

    //flips the x axis positions by remapping
    public void InvertXDataPoints()
    {
        Dictionary<string, Atom> newMapping = new();
        var orderedMap = _mapping.OrderBy(s => RingAtoms.IndexOf(s.Key)).ToList();
        for (var i = 0; i < orderedMap.Count; i++)
        {
            if(Type == MacrocycleType.Porphyrin && orderedMap[i].Key != "C20")
                newMapping[orderedMap[i].Key] = orderedMap[_mapping.Count - (i+2)].Value;
            else newMapping[orderedMap[i].Key] = orderedMap[_mapping.Count - (i+1)].Value;
        }
        if(Type == MacrocycleType.Porphyrin) 
            newMapping["C20"] = _mapping["C20"];
        _mapping = newMapping;
        Properties!.Rebuild();
    }

    /// <summary>
    /// Rotates Datapoints for Porphyrin: 90 deg, for Norcorrole and Porphycene 180 deg
    /// </summary>
    public void RotateDataPoints(int delta, bool rebuildProps = false)
    {
        Dictionary<string, Atom> newMapping = new();
        for (var i = 0; i < _mapping.Count; i++)
        {
            if (!_mapping.ElementAt(i).Key.StartsWith("C")) continue;
            var no = int.Parse(_mapping.ElementAt(i).Key[1..]);
            var newNo = (int) (no + delta - 20 * Math.Floor((no + delta) / 20d));
            if (newNo == 0) newNo = 20;
            newMapping["C" + newNo] = _mapping.ElementAt(i).Value;
        }

        for (var i = 0; i < _mapping.Count; i++)
        {
            if (!_mapping.ElementAt(i).Key.StartsWith("N")) continue;
            var c = _mapping.ElementAt(i).Value;
            var neighbors = Neighbors(c);
            foreach (var a in neighbors)
            {
                var mapped = newMapping.FirstOrDefault(k => k.Value == a).Key;
                if (mapped == AlphaAtoms[0]) newMapping["N1"] = c;
                if (mapped == AlphaAtoms[2]) newMapping["N2"] = c;
                if (mapped == AlphaAtoms[4]) newMapping["N3"] = c;
                if (mapped == AlphaAtoms[6]) newMapping["N4"] = c;
            }
        }

        _mapping = newMapping;
        
        if(rebuildProps)
            Properties!.Rebuild();
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
    /// <param name="key"></param>
    /// <returns>Atom</returns>
    private Atom? NextAlpha(string key)
    {
        var i = Array.IndexOf(AlphaAtoms, key) + 1;
        return AlphaAtoms.Length > i ? FindAtomByTitle(AlphaAtoms[i]) : null;
    }

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
        from bond in Bonds.Where(s => !s.Atoms.ScrambledEquals(new [] {_mapping[RingAtoms.First()], _mapping[RingAtoms.Last()]})) //Do not draw bond between first and last element
        let start = DataPoints.FirstOrDefault(s => s.Atom.Equals(bond.Atom1))
        let end = DataPoints.FirstOrDefault(s => s.Atom.Equals(bond.Atom2))
        select (start, end);

    /// <summary>
    /// Creates Analysis Type
    /// </summary>
    /// <param name="mol"></param>
    /// <param name="mapping"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static MacrocycleAnalysis Create(Molecule mol, Dictionary<string, Atom> mapping, MacrocycleType type)
    {
        MacrocycleAnalysis analysis = type switch
        {
            MacrocycleType.Porphyrin => new PorphyrinAnalysis(mol, mapping),
            MacrocycleType.Corrole => new CorroleAnalysis(mol, mapping),
            MacrocycleType.Norcorrole => new NorcorroleAnalysis(mol, mapping),
            MacrocycleType.Porphycene => new PorphyceneAnalysis(mol, mapping),
            MacrocycleType.Corrphycene => new CorrphyceneAnalysis(mol, mapping),
            _ => throw new InvalidOperationException()
        };
        return analysis;
    }
    
    /// <summary>
    /// Finds atom by Title
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    public Atom? FindAtomByTitle(string title) => _mapping.TryAndGet(title);
}

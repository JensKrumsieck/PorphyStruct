#nullable enable
using System.Diagnostics;
using ChemSharp.Mathematics;
using ChemSharp.Molecules;
using ChemSharp.Molecules.Properties;
using PorphyStruct.Core.Plot;

namespace PorphyStruct.Core.Analysis;

public class PorphyrinAnalysis : MacrocycleAnalysis
{
    public PorphyrinAnalysis(Molecule mol) : base(mol)
    {
        //check if is Isoporphyrin and reorder:
        if (!Isoporphyrin) return;
        RemapAtoms();
    }

    private void RemapAtoms()
    {
        var isoMeso = _isoCarbon!; //is not null as Isoporphyrin is true!
        var isoNo = int.Parse(isoMeso.Title[1..]);
        if (isoNo == 10) return;
        var delta = isoNo switch
        {
            5 => 5,
            15 => -5,
            20 => -10,
            _ => 0 //wtf?
        };
        Molecule.CacheNeighbors = false;
        foreach (var c in Atoms.Where(a => a.Title.StartsWith("C")).OrderBy(a => a.Title))
        {
            var no = int.Parse(c.Title[1..]);
            var newNo = (int) (no + delta - 20 * Math.Floor((no + delta) / 20d));
            if (newNo == 0) newNo = 20;
            c.Title = $"C{newNo}";
        }

        foreach (var n in Atoms.Where(a => a.Title.StartsWith("N")))
        {
            var neighbors = Molecule.Neighbors(n);
            if (neighbors.Any(c => c.Title == "C1")) n.Title = "N1";
            if (neighbors.Any(c => c.Title == "C6")) n.Title = "N2";
            if (neighbors.Any(c => c.Title == "C11")) n.Title = "N3";
            if (neighbors.Any(c => c.Title == "C16")) n.Title = "N4";
        }

        Molecule.RebuildCache();
        Molecule.CacheNeighbors = true;
    }
#pragma warning disable IDE1006
    //ReSharper disable InconsistentNaming
    private static readonly string[] _AlphaAtoms = { "C1", "C4", "C6", "C9", "C11", "C14", "C16", "C19", "C1" };
    internal static readonly List<string> _RingAtoms = new() { "C1", "C2", "N1", "C3", "C4", "C5", "C6", "C7", "N2", "C8", "C9", "C10", "C11", "C12", "N3", "C13", "C14", "C15", "C16", "C17", "N4", "C18", "C19", "C20" };
    internal static Dictionary<string, double> _Multiplier => new()
    {
        { "C1", 0d },
        { "C2", 1 / 3d },
        { "C3", 2 / 3d },
        { "C4", 1d },
        { "C5", 1 / 2d },
        { "C6", 1d },
        { "C7", 1 / 3d },
        { "C8", 2 / 3d },
        { "C9", 1d },
        { "C10", 1 / 2d },
        { "C11", 1d },
        { "C12", 1 / 3d },
        { "C13", 2 / 3d },
        { "C14", 1d },
        { "C15", 1 / 2d },
        { "C16", 1d },
        { "C17", 1 / 3d },
        { "C18", 2 / 3d },
        { "C19", 1d },
        { "C20", 1 / 2d }
    };
    // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006

    /// <summary>
    /// Overrides Macrocycle.CalculateDataPoints
    /// Add PorphyrinDataPoint and shift all Points because of C20 being first
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<AtomDataPoint> CalculateDataPoints() => base.CalculateDataPoints().Select(s => s = new AtomDataPoint(s.X + (CalculateDistance("C1", "C19") / 2), s.Y, s.Atom)).Concat(CalculatePorphyrinDataPoints());

    /// <summary>
    /// Calculates PorphyrinDataPoints
    /// </summary>
    /// <returns></returns>
    private IEnumerable<AtomDataPoint> CalculatePorphyrinDataPoints()
    {
        //add c20
        var c20 = Atoms.FirstOrDefault(s => s.Title == "C20");
        if (c20 != null) yield return new AtomDataPoint(1, MathV.Distance(MeanPlane, c20.Location), c20);
    }

    ///<inheritdoc/>
    public override IEnumerable<(AtomDataPoint a1, AtomDataPoint a2)> BondDataPoints() => base.BondDataPoints().Where(s => s.a1.Atom.Title != "C20" && s.a2.Atom.Title != "C20").Concat(PorphyrinBonds());

    /// <summary>
    /// Calculates Bond DataPoints for Porphyrins
    /// </summary>
    /// <returns></returns>
    private IEnumerable<(AtomDataPoint a1, AtomDataPoint a2)> PorphyrinBonds()
    {
        yield return (DataPoints.OrderBy(s => s.X).First(), DataPoints.First(s => s.Atom.Title == "C1"));
        yield return (DataPoints.OrderBy(s => s.X).Last(), DataPoints.First(s => s.Atom.Title == "C19"));
    }

    /// <summary>
    /// Indicates whether this could be an isoporphyrin
    /// </summary>
    public bool Isoporphyrin
    {
        get
        {
            var mesoAngles = (from m in Meso let n = Neighbors(m).ToArray() select new Angle(n[0], m, n[1])).ToList();
            var median = mesoAngles.OrderBy(s => s.Value).ToArray()[mesoAngles.Count / 2];
            var isoAngle = mesoAngles.FirstOrDefault(s => Math.Abs(s.Value - median.Value) > 5d);
            _isoCarbon = isoAngle?.Atom2;
            return isoAngle != null;
        }
    }
    private Atom? _isoCarbon;

    protected override List<string> RingAtoms => _RingAtoms;
    protected override string[] AlphaAtoms => _AlphaAtoms;
    protected override Dictionary<string, double> Multiplier => _Multiplier;
}

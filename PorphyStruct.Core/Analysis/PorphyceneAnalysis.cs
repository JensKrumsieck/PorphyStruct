using ChemSharp.Molecules;

namespace PorphyStruct.Core.Analysis;

public class PorphyceneAnalysis : MacrocycleAnalysis
{
    public PorphyceneAnalysis(Molecule mol, Dictionary<string, Atom> mapping) : base(mol, mapping) { }

#pragma warning disable IDE1006
    //ReSharper disable InconsistentNaming
    private static readonly string[] _AlphaAtoms = { "C1", "C4", "C7", "C10", "C11", "C14", "C17", "C20" };
    private static readonly List<string> _RingAtoms = PorphyrinAnalysis._RingAtoms;

    private static Dictionary<string, double> _Multiplier => new()
    {
        { "C1", 0d },
        { "C2", 1 / 3d },
        { "C3", 2 / 3d },
        { "C4", 1d },
        { "C5", 1 / 3d },
        { "C6", 2 / 3d },
        { "C7", 1d },
        { "C8", 1 / 3d },
        { "C9", 2 / 3d },
        { "C10", 1d },
        { "C11", 1d },
        { "C12", 1 / 3d },
        { "C13", 2 / 3d },
        { "C14", 1d },
        { "C15", 1 / 3d },
        { "C16", 2 / 3d },
        { "C17", 1d },
        { "C18", 1 / 3d },
        { "C19", 2 / 3d },
        { "C20", 1d }
    };
    // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006

    protected virtual Atom C1 => Alpha.FirstOrDefault(atom => Neighbors(atom).Any(l => Alpha.Contains(l)))!;

    public override MacrocycleType Type => MacrocycleType.Porphycene;
    protected override List<string> RingAtoms => _RingAtoms;
    protected override string[] AlphaAtoms => _AlphaAtoms;
    protected override Dictionary<string, double> Multiplier => _Multiplier;

}

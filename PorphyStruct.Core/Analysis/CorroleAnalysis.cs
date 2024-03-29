﻿using ChemSharp.Molecules;

namespace PorphyStruct.Core.Analysis;

public class CorroleAnalysis : MacrocycleAnalysis
{
    public CorroleAnalysis(Molecule mol, Dictionary<string, Atom> mapping) : base(mol, mapping) { }

#pragma warning disable IDE1006
    //ReSharper disable InconsistentNaming
    private static readonly string[] _AlphaAtoms = { "C1", "C4", "C6", "C9", "C11", "C14", "C16", "C19" };
    internal static readonly List<string> _RingAtoms = PorphyrinAnalysis._RingAtoms.Except(new List<string> { "C20" }).ToList();
    private static Dictionary<string, double> _Multiplier => PorphyrinAnalysis._Multiplier;
    // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006

    protected virtual Atom C1 => Alpha.FirstOrDefault(atom => Neighbors(atom).Any(l => Alpha.Contains(l)))!;

    public override MacrocycleType Type => MacrocycleType.Corrole;
    protected override List<string> RingAtoms => _RingAtoms;
    protected override string[] AlphaAtoms => _AlphaAtoms;
    protected override Dictionary<string, double> Multiplier => _Multiplier;
}

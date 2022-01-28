using ChemSharp.Molecules;

namespace PorphyStruct.Core.Analysis;

public class NorcorroleAnalysis : CorroleAnalysis
{
    public NorcorroleAnalysis(List<Atom> atoms, IEnumerable<Bond> bonds) : base(atoms, bonds) { }

    //ReSharper disable InconsistentNaming
    private new static readonly List<string> _RingAtoms = CorroleAnalysis._RingAtoms.Except(new List<string> { "C10" }).ToList();
    // ReSharper restore InconsistentNaming

    protected override List<string> RingAtoms => _RingAtoms;
}

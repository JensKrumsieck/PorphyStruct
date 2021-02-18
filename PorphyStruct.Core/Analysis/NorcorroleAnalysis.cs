using System.Collections.Generic;
using System.Linq;
using ChemSharp.Molecules;

namespace PorphyStruct.Core.Analysis
{
    public class NorcorroleAnalysis : CorroleAnalysis
    {
        public NorcorroleAnalysis(List<Atom> atoms, IEnumerable<Bond> bonds) : base(atoms, bonds) { }

        //ReSharper disable InconsistentNaming
        public new static List<string> _RingAtoms = CorroleAnalysis._RingAtoms.Except(new List<string> { "C10" }).ToList();
        // ReSharper restore InconsistentNaming
        public override List<string> RingAtoms => _RingAtoms;
    }
}

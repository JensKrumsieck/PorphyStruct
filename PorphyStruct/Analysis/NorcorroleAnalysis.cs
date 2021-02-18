using ChemSharp.Molecules;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Analysis
{
    public class NorcorroleAnalysis : CorroleAnalysis
    {
        public NorcorroleAnalysis(List<Atom> atoms, IEnumerable<Bond> bonds) : base(atoms, bonds) { }

        //ReSharper disable InconsistentNaming
        public static List<string> _RingAtoms = CorroleAnalysis._RingAtoms.Except(new List<string> { "C10" }).ToList();
        // ReSharper restore InconsistentNaming
        public override List<string> RingAtoms => _RingAtoms;
    }
}

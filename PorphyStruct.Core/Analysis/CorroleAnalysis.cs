using ChemSharp.Molecules;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Core.Analysis
{
    public class CorroleAnalysis : MacrocycleAnalysis
    {
        public CorroleAnalysis(List<Atom> atoms, IEnumerable<Bond> bonds) : base(atoms, bonds) { }

        //ReSharper disable InconsistentNaming
        internal static string[] _AlphaAtoms = { "C1", "C4", "C6", "C9", "C11", "C14", "C16", "C19" };
        internal static List<string> _RingAtoms = PorphyrinAnalysis._RingAtoms.Except(new List<string> { "C20" }).ToList();
        internal static Dictionary<string, double> _Multiplier => PorphyrinAnalysis._Multiplier;
        // ReSharper restore InconsistentNaming

        public override Atom C1 => Alpha.FirstOrDefault(atom => Neighbors(atom).Count(l => Alpha.Contains(l)) != 0);

        public override List<string> RingAtoms => _RingAtoms;
        public override string[] AlphaAtoms => _AlphaAtoms;
        public override Dictionary<string, double> Multiplier => _Multiplier;
    }
}

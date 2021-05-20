using ChemSharp.Molecules;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Core.Analysis
{
    public class CorroleAnalysis : MacrocycleAnalysis
    {
        public CorroleAnalysis(List<Atom> atoms, IEnumerable<Bond> bonds) : base(atoms, bonds) { }

#pragma warning disable IDE1006
        //ReSharper disable InconsistentNaming
        private static readonly string[] _AlphaAtoms = { "C1", "C4", "C6", "C9", "C11", "C14", "C16", "C19" };
        internal static readonly List<string> _RingAtoms = PorphyrinAnalysis._RingAtoms.Except(new List<string> { "C20" }).ToList();
        private static Dictionary<string, double> _Multiplier => PorphyrinAnalysis._Multiplier;
        // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006

        protected override Atom C1 => Alpha.FirstOrDefault(atom => Neighbors(atom).Count(l => Alpha.Contains(l)) != 0);

        protected override List<string> RingAtoms => _RingAtoms;
        protected override string[] AlphaAtoms => _AlphaAtoms;
        protected override Dictionary<string, double> Multiplier => _Multiplier;
    }
}

using ChemSharp.Molecules;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Core.Analysis
{
    public class CorrphyceneAnalysis : MacrocycleAnalysis
    {
        public CorrphyceneAnalysis(List<Atom> atoms, IEnumerable<Bond> bonds) : base(atoms, bonds) { }

        //ReSharper disable InconsistentNaming
        internal static string[] _AlphaAtoms = {"C1", "C4", "C6", "C9", "C12", "C15", "C17", "C20"};
        internal static List<string> _RingAtoms = PorphyrinAnalysis._RingAtoms;

        internal static Dictionary<string, double> _Multiplier => new Dictionary<string, double>
        {
            {"C1", 0d},
            {"C2", 1 / 3d},
            {"C3", 2 / 3d},
            {"C4", 1d},
            {"C5", 1 / 2d},
            {"C6", 1d},
            {"C7", 1 / 3d},
            {"C8", 2 / 3d},
            {"C9", 1d},
            {"C10", 1 / 3d},
            {"C11", 2 / 3d},
            {"C12", 1d},
            {"C13", 1 / 3d},
            {"C14", 2 / 3d},
            {"C15", 1d},
            {"C16", 1 / 2d},
            {"C17", 1d},
            {"C18", 1 / 3d},
            {"C19", 2 / 3d},
            {"C20", 1d}
        };
        // ReSharper restore InconsistentNaming

        public override Atom C1 => Alpha.FirstOrDefault(atom => Neighbors(atom).Count(l => Alpha.Contains(l)) != 0);

        public override List<string> RingAtoms => _RingAtoms;
        public override string[] AlphaAtoms => _AlphaAtoms;
        public override Dictionary<string, double> Multiplier => _Multiplier;
    }
}

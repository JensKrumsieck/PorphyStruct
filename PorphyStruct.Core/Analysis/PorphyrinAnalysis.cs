using ChemSharp.Mathematics;
using ChemSharp.Molecules;
using PorphyStruct.Core.Plot;
using System.Collections.Generic;
using System.Linq;
using PorphyStruct.Core.Analysis.Properties;

namespace PorphyStruct.Core.Analysis
{
    public class PorphyrinAnalysis : MacrocycleAnalysis
    {
        public PorphyrinAnalysis(List<Atom> atoms, IEnumerable<Bond> bonds) : base(atoms, bonds) { }

        //ReSharper disable InconsistentNaming
        internal static string[] _AlphaAtoms = { "C1", "C4", "C6", "C9", "C11", "C14", "C16", "C19", "C1" };
        internal static List<string> _RingAtoms = new List<string> { "C1", "C2", "N1", "C3", "C4", "C5", "C6", "C7", "N2", "C8", "C9", "C10", "C11", "C12", "N3", "C13", "C14", "C15", "C16", "C17", "N4", "C18", "C19", "C20" };

        internal static Dictionary<string, double> _Multiplier => new Dictionary<string, double>
        {
            { "C1", 0d },
            { "C2", 1 / 3d },
            { "C3", 2 / 3d },
            { "C4", 1d },
            { "C5",1 / 2d },
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

        internal static IList<string[]> PorphyrinDihedrals = new List<string[]>{
            new[] { "C3", "C4", "C6", "C7" }, //chi1
            new[] { "C2", "C1", "C19", "C18" }, //chi2
            new[] { "C13", "C14", "C16", "C17" }, //chi3 
            new[] { "C8", "C9", "C11", "C12" }, //chi4
            new[] { "C4", "N1", "N3", "C11" }, //psi1
            new[] { "C9", "N2", "N4", "C16" }, //psi2
            new[] { "N1", "C4", "C6", "N2" }, //phi1
            new[] { "N1", "C1", "C19", "N4" }, //phi2
            new[] { "N3", "C14", "C16", "N4" }, //phi3
            new[] { "N2", "C9", "C11", "N3" } //phi4
        };
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Overrides Macrocycle.CalculateDataPoints
        /// Add PorphyrinDataPoint and shift all Points because of C20 being first
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<AtomDataPoint> CalculateDataPoints() => base.CalculateDataPoints().Select(s => s = new AtomDataPoint(s.X + (CalculateDistance("C1", "C19") / 2), s.Y, s.Atom)).Concat(CalculatePorphyrinDataPoints());

        // <summary>
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
        public IEnumerable<(AtomDataPoint a1, AtomDataPoint a2)> PorphyrinBonds()
        {
            yield return (DataPoints.OrderBy(s => s.X).First(), DataPoints.First(s => s.Atom.Title == "C1"));
            yield return (DataPoints.OrderBy(s => s.X).Last(), DataPoints.First(s => s.Atom.Title == "C19"));
        }

        public override List<string> RingAtoms => _RingAtoms;
        public override string[] AlphaAtoms => _AlphaAtoms;
        public override Dictionary<string, double> Multiplier => _Multiplier;
        public override IList<Dihedral> Dihedrals => base.Dihedrals
            .Concat(PorphyrinDihedrals.Select(s => new Dihedral(s[0], s[1], s[2], s[3], Atoms))).ToList();
    }
}

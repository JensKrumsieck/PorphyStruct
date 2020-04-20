using OxyPlot.Annotations;
using PorphyStruct.Chemistry.Data;
using PorphyStruct.Chemistry.Properties;
using PorphyStruct.Core.Util;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class Porphyrin : Macrocycle
    {
        public Porphyrin(AsyncObservableCollection<Atom> Atoms) : base(Atoms) => PropertyProviders.Add(new PorphyrinDihedrals(ByIdentifier));

        //assign type (legacy)
        public override Type type => Type.Porphyrin;

        /// <summary>
        /// Porphyrins Bonds by Identifiers
        /// </summary>
        public static List<(string Atom1, string Atom2)> _Bonds => new List<(string Atom1, string Atom2)>()
        {
            ("C1", "C2"),
            ("C1", "N1"),
            ("C2", "C3"),
            ("C3", "C4"),
            ("N1", "C4"),
            ("C5", "C4"),
            ("C5", "C6"),
            ("C6", "N2"),
            ("C6", "C7"),
            ("C8", "C7"),
            ("C8", "C9"),
            ("C9", "C10"),
            ("C9", "N2"),
            ("C11", "C10"),
            ("C11", "N3"),
            ("C11", "C12"),
            ("C12", "C13"),
            ("C13", "C14"),
            ("N3", "C14"),
            ("C14", "C15"),
            ("C15", "C16"),
            ("C16", "C17"),
            ("C17", "C18"),
            ("C18", "C19"),
            ("N4", "C19"),
            ("N4", "C16"),
            ("C19", "C20"),
            ("C20", "C1")
         };
        public override List<(string Atom1, string Atom2)> Bonds => _Bonds;

        /// <summary>
        /// Porphyrins Ring Atoms by Identifier
        /// </summary>
        public static List<string> _RingAtoms = new List<string>() { "C1", "C2", "N1", "C3", "C4", "C5", "C6", "C7", "N2", "C8", "C9", "C10", "C11", "C12", "N3", "C13", "C14", "C15", "C16", "C17", "N4", "C18", "C19", "C20" };
        public override List<string> RingAtoms => _RingAtoms;

        /// <summary>
        /// Porphyrins Ring Atoms by Identifier
        /// </summary>
        public static string[] _AlphaAtoms = new string[] { "C1", "C4", "C6", "C9", "C11", "C14", "C16", "C19", "C1" };
        public override string[] AlphaAtoms => _AlphaAtoms;

        /// <summary>
        /// Multipliers for C-Atom positioning
        /// </summary>
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
        public override Dictionary<string, double> Multiplier => _Multiplier;

        /// <summary>
        /// Overrides Macrocycle.DrawBonds
        /// because of special C1-C20 Bond.
        /// </summary>
        /// <returns>Annotation aka Bonds</returns>
        public override IEnumerable<ArrowAnnotation> DrawBonds(IAtomDataPointProvider data) => base.DrawBonds(data).Where(s => !((string)s.Tag).Contains("C20")).Concat(DrawPorphyrinBonds(data));

        /// <summary>
        /// Draw Porphyrin specific Bonds
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private IEnumerable<ArrowAnnotation> DrawPorphyrinBonds(IAtomDataPointProvider data)
        {
            yield return DrawBond(data.DataPoints.OrderBy(s => s.X).First(), data.DataPoints.Where(s => s.atom.Identifier == "C1" && s.atom.IsMacrocycle).First(), data);
            yield return DrawBond(data.DataPoints.OrderBy(s => s.X).Last(), data.DataPoints.Where(s => s.atom.Identifier == "C19" && s.atom.IsMacrocycle).First(), data);
        }

        /// <summary>
        /// Overrides Macrocycle.CalculateDataPoints
        /// Add PorphyrinDataPoint and shift all Points because of C20 being first
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<AtomDataPoint> CalculateDataPoints() => base.CalculateDataPoints().Select(s => s = new AtomDataPoint(s.X + (CalculateDistance("C1", "C19") / 2), s.Y, s.atom)).Concat(CalculatePorphyrinDataPoints());

        /// <summary>
        /// Calculates PorphyrinDataPoints
        /// </summary>
        /// <returns></returns>
        private IEnumerable<AtomDataPoint> CalculatePorphyrinDataPoints()
        {
            //add c20
            yield return new AtomDataPoint(1, ByIdentifier("C20", true).DistanceToPlane(GetMeanPlane()), ByIdentifier("C20", true));
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        public override object Clone() => new Porphyrin(Atoms);
    }
}

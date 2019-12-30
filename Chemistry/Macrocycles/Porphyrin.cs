using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class Porphyrin : Macrocycle
    {
        public Porphyrin(List<Atom> Atoms) : base(Atoms) { }

        /// <summary>
        /// Porphyrins Bonds by Identifiers
        /// </summary>
        public static List<Tuple<string, string>> _Bonds => new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C1", "C2"),
            new Tuple<string, string>("C1", "N1"),
            new Tuple<string, string>("C2", "C3"),
            new Tuple<string, string>("C3", "C4"),
            new Tuple<string, string>("N1", "C4"),
            new Tuple<string, string>("C5", "C4"),
            new Tuple<string, string>("C5", "C6"),
            new Tuple<string, string>("C6", "N2"),
            new Tuple<string, string>("C6", "C7"),
            new Tuple<string, string>("C8", "C7"),
            new Tuple<string, string>("C8", "C9"),
            new Tuple<string, string>("C9", "C10"),
            new Tuple<string, string>("C9", "N2"),
            new Tuple<string, string>("C11", "C10"),
            new Tuple<string, string>("C11", "N3"),
            new Tuple<string, string>("C11", "C12"),
            new Tuple<string, string>("C12", "C13"),
            new Tuple<string, string>("C13", "C14"),
            new Tuple<string, string>("N3", "C14"),
            new Tuple<string, string>("C14", "C15"),
            new Tuple<string, string>("C15", "C16"),
            new Tuple<string, string>("C16", "C17"),
            new Tuple<string, string>("C17", "C18"),
            new Tuple<string, string>("C18", "C19"),
            new Tuple<string, string>("N4", "C19"),
            new Tuple<string, string>("N4", "C16"),
            new Tuple<string, string>("C19", "C20"),
            new Tuple<string, string>("C20", "C1")
         };
        public override List<Tuple<string, string>> Bonds => _Bonds;

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
        internal static Dictionary<string, double> _Multiplier
        {
            get
            {
                return new Dictionary<string, double>
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
            }
        }
        public override Dictionary<string, double> Multiplier => _Multiplier;
    }
}

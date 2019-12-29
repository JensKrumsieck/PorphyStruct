using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class Porphycene : Macrocycle
    {
        public Porphycene(List<Atom> Atoms) : base(Atoms) { }

        /// <summary>
        /// Corroles Bonds by Identifiers
        /// </summary>
        public static List<Tuple<string, string>> _Bonds => Porphyrin._Bonds.Except(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C6", "N2"),
            new Tuple<string, string>("C9", "N2"),
            new Tuple<string, string>("N4", "C16"),
            new Tuple<string, string>("N4", "C19"),
            new Tuple<string, string>("C20", "C1")
        }).Concat(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C7", "N2"),
            new Tuple<string, string>("C10", "N2"),
            new Tuple<string, string>("C17", "N4"),
            new Tuple<string, string>("C20", "N4"),
        }).ToList();
        public override List<Tuple<string, string>> Bonds => _Bonds;

        /// <summary>
        /// Porphyrins Ring Atoms by Identifier
        /// </summary>
        public static List<string> _RingAtoms = Porphyrin._RingAtoms;
        public override List<string> RingAtoms => _RingAtoms;

        /// <summary>
        /// Porphyrins Ring Atoms by Identifier
        /// </summary>
        public static string[] _AlphaAtoms = new string[] { "C1", "C4", "C7", "C10", "C11", "C14", "C17", "C20" };
        public override string[] AlphaAtoms => _AlphaAtoms;
    }
}

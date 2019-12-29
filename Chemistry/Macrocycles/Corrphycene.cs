using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class Corrphycene : Macrocycle
    {
        public Corrphycene(List<Atom> Atoms) : base(Atoms) { }

        /// <summary>
        /// Corroles Bonds by Identifiers
        /// </summary>
        public static List<Tuple<string, string>> _Bonds => Porphyrin._Bonds.Except(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C11", "N3"),
            new Tuple<string, string>("N3", "C14"),
            new Tuple<string, string>("N4", "C16"),
            new Tuple<string, string>("N4", "C19"),
            new Tuple<string, string>("C20", "C1")
        }).Concat(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C12", "N3"),
            new Tuple<string, string>("C15", "N3"),
            new Tuple<string, string>("C17", "N4"),
            new Tuple<string, string>("C20", "N4")
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
        public static string[] _AlphaAtoms = new string[] { "C1", "C4", "C6", "C9", "C12", "C15", "C17", "C20" };
        public override string[] AlphaAtoms => _AlphaAtoms;
    }
}

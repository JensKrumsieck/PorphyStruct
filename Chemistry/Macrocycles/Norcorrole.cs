using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class Norcorrole : Macrocycle
    {
        public Norcorrole(List<Atom> Atoms) : base(Atoms) { }

        //assign type (legacy)
        public override Type type => Type.Norcorrole;

        /// <summary>
        /// Corroles Bonds by Identifiers
        /// </summary>
        public static List<Tuple<string, string>> _Bonds => Corrole._Bonds.Except(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C9", "C10"),
            new Tuple<string, string>("C11", "C10")
        }).Concat(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C9", "C11")
        }).ToList();
        public override List<Tuple<string, string>> Bonds => _Bonds;

        /// <summary>
        /// Porphyrins Ring Atoms by Identifier
        /// </summary>
        public static List<string> _RingAtoms = Corrole._RingAtoms.Except(new List<string>() { "C10" }).ToList();
        public override List<string> RingAtoms => _RingAtoms;

        /// <summary>
        /// Porphyrins Ring Atoms by Identifier
        /// </summary>
        public static string[] _AlphaAtoms = Corrole._AlphaAtoms;
        public override string[] AlphaAtoms => _AlphaAtoms;

        /// <summary>
        /// X-Coordinate Multiplier
        /// </summary>
        public override Dictionary<string, double> Multiplier => Porphyrin._Multiplier;
        
        /// <summary>
        /// Norcorroles Dihedrals
        /// </summary>
        public override List<string[]> Dihedrals => Porphyrin._Dihedrals;

        /// <summary>
        /// Sets C1 to Corrole-Type C1
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        public override Atom C1(IEnumerable<Atom> cycle) => Corrole._C1(cycle);

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        public override object Clone() => new Norcorrole(Atoms);
    }
}

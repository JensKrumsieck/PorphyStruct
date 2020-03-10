using PorphyStruct.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class Corrole : Macrocycle
    {
        public Corrole(AsyncObservableCollection<Atom> Atoms) : base(Atoms) {
            PropertyProviders.Add(new PorphyrinDihedrals(ByIdentifier));
        }

        //assign type (legacy)
        public override Type type => Type.Corrole;
        /// <summary>
        /// Corroles Bonds by Identifiers
        /// </summary>
        public static List<Tuple<string, string>> _Bonds => Porphyrin._Bonds.Except(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C20", "C1"),
            new Tuple<string, string>("C19", "C20")
        }).ToList();
        public override List<Tuple<string, string>> Bonds => _Bonds;

        /// <summary>
        /// Porphyrins Ring Atoms by Identifier
        /// </summary>
        public static List<string> _RingAtoms = Porphyrin._RingAtoms.Except(new List<string>() { "C20" }).ToList();
        public override List<string> RingAtoms => _RingAtoms;

        /// <summary>
        /// Porphyrins Ring Atoms by Identifier
        /// </summary>
        public static string[] _AlphaAtoms = new string[] { "C1", "C4", "C6", "C9", "C11", "C14", "C16", "C19" };
        public override string[] AlphaAtoms => _AlphaAtoms;

        /// <summary>
        /// X-Coordinate Multiplier
        /// </summary>
        public override Dictionary<string, double> Multiplier => Porphyrin._Multiplier;

        /// <summary>
        /// Returns C1 for Corrole (and Norcorrole)
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        public static Atom _C1(IEnumerable<Atom> cycle)
        {
            Atom C1 = null;
            //loop alpha atoms
            foreach (var atom in Vertex3Atoms(cycle))
            {
                if (Neighbors(atom, cycle).Where(l => Vertex3Atoms(cycle).Contains(l)).Count() != 0) C1 = atom;
            }
            return C1;
        }

        public override Atom C1(IEnumerable<Atom> cycle) => _C1(cycle);

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        public override object Clone() => new Corrole(Atoms);
    }
}

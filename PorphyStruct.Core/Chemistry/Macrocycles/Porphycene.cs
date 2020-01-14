using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class Porphycene : Macrocycle
    {
        public Porphycene(List<Atom> Atoms) : base(Atoms) { }

        //assign type (legacy)
        public override Type type => Type.Porphycene;

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

        /// <summary>
        /// Multiplier of X-Coordinate
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
                    { "C5", 1 / 3d},
                    { "C6", 2 / 3d},
                    { "C7", 1d},
                    { "C8", 1 / 3d },
                    { "C9", 2 / 3d },
                    { "C10", 1d  },
                    { "C11", 1d },
                    { "C12",  1 / 3d },
                    { "C13",2 / 3d },
                    { "C14",  1d },
                    { "C15",1 / 3d },
                    { "C16", 2 / 3d },
                    { "C17", 1d  },
                    { "C18", 1 / 3d  },
                    { "C19",  2 / 3d },
                    { "C20",  1d  }
                };
            }
        }
        public override Dictionary<string, double> Multiplier => _Multiplier;

        /// <summary>
        /// Porphycene Dihedrals
        /// </summary>
        internal static List<string[]> _Dihedrals => new List<string[]>(){
            new string[] { "C2", "C1", "C20", "C19" },
            new string[] { "C3", "C4", "C7", "C8" },
            new string[] { "C9", "C10", "C11", "C12" },
            new string[] { "C13", "C14", "C17", "C18" },
            new string[] { "C10", "N2", "N4", "C17" },
            new string[] { "C4", "N1", "N3", "C11" }
        };
        public override List<string[]> Dihedrals => _Dihedrals;

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
        public override object Clone() => new Porphycene(Atoms);
    }
}

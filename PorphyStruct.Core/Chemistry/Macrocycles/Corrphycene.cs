using PorphyStruct.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class Corrphycene : Macrocycle
    {
        public Corrphycene(AsyncObservableCollection<Atom> Atoms) : base(Atoms)
        {
            PropertyProviders.Add(new CorrphyceneDihedrals(ByIdentifier));
        }

        //assign type (legacy)
        public override Type type => Type.Corrphycene;


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
                    { "C5", 1 / 2d },
                    { "C6",1d },
                    { "C7", 1 / 3d },
                    { "C8", 2 / 3d },
                    { "C9", 1d },
                    { "C10", 1 / 3d},
                    { "C11",2 / 3d  },
                    { "C12", 1d  },
                    { "C13",  1 / 3d  },
                    { "C14",  2 / 3d  },
                    { "C15",  1d  },
                    { "C16",  1 / 2d  },
                    { "C17", 1d },
                    { "C18", 1 / 3d },
                    { "C19",  2 / 3d },
                    { "C20",  1d  }
                };
            }
        }
        public override Dictionary<string, double> Multiplier => _Multiplier;

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
        public override object Clone() => new Corrphycene(Atoms);
    }
}

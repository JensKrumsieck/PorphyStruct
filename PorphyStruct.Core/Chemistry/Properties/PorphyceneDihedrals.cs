using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Properties
{
    public class PorphyceneDihedrals : AbstractPropertyProvider
    {
        public PorphyceneDihedrals(Func<string, Atom> function) : base(function) { }

        public override IList<string[]> Selectors =>
            new List<string[]>()
            {
                new string[] { "C2", "C1", "C20", "C19" }, //chi2
                new string[] { "C3", "C4", "C7", "C8" }, //chi1
                new string[] { "C9", "C10", "C11", "C12" }, //chi4
                new string[] { "C13", "C14", "C17", "C18" }, //chi3
                new string[] { "C10", "N2", "N4", "C17" }, //psi2
                new string[] { "C4", "N1", "N3", "C11" }, //psi1
                new string[] { "N1", "C1", "C20", "N4"}, //phi2
                new string[] { "N1", "C4", "C7" , "N2"}, //phi1
                new string[] { "N2", "C10", "C11", "N3"}, //phi4
                new string[] { "N3", "C14", "C17", "N4"} //phi3
            };

        public override PropertyType Type => PropertyType.Dihedral;
    }
}

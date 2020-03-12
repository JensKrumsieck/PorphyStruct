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
                new string[] { "C2", "C1", "C20", "C19" },
                new string[] { "C3", "C4", "C7", "C8" },
                new string[] { "C9", "C10", "C11", "C12" },
                new string[] { "C13", "C14", "C17", "C18" },
                new string[] { "C10", "N2", "N4", "C17" },
                new string[] { "C4", "N1", "N3", "C11" }
            };

        public override PropertyType Type => PropertyType.Dihedral;
    }
}

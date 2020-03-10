using PorphyStruct.Core.Util;
using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class CorrphyceneDihedrals : AbstractPropertyProvider
    {
        public CorrphyceneDihedrals(Func<string, Atom> function) : base(function) { }

        public override IList<string[]> Selectors =>
            new List<string[]>()
            {
                new string[] { "C2", "C1", "C20", "C19" },
                new string[] { "C3", "C4", "C6", "C7" },
                new string[] { "C8", "C9", "C12", "C13" },
                new string[] { "C14", "C15", "C17", "C18" },
                new string[] { "C9", "N2", "N4", "C17" },
                new string[] { "C4", "N1", "N3", "C12" }
            };

        public override PropertyType Type => PropertyType.Dihedral;
    }
}

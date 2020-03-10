using PorphyStruct.Core.Util;
using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class PorphyrinDihedrals : AbstractPropertyProvider
    {
        public PorphyrinDihedrals(Func<string, Atom> function) : base(function) { }

        public override IList<string[]> Selectors =>
            new List<string[]>(){
                new string[] { "C2", "C1", "C19", "C18" },
                new string[] { "C3", "C4", "C6", "C7" },
                new string[] { "C8", "C9", "C11", "C12" },
                new string[] { "C13", "C14", "C16", "C17" },
                new string[] { "C9", "N2", "N4", "C16" },
                new string[] { "C4", "N1", "N3", "C11" }
            };

        public override PropertyType Type => PropertyType.Dihedral;
    }
}

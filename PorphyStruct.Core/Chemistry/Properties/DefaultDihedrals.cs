using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Properties
{
    public class DefaultDihedrals : AbstractPropertyProvider
    {
        public DefaultDihedrals(Func<string, Atom> function) : base(function) { }

        public override IList<string[]> Selectors =>
            new List<string[]>(){
                new string[] { "N1", "N2", "N3", "N4"}
            };

        public override PropertyType Type => PropertyType.Dihedral;
    }
}

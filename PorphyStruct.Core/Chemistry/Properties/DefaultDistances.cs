using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Properties
{
    public class DefaultDistances : AbstractPropertyProvider
    {
        public DefaultDistances(Func<string, Atom> function) : base(function) { }


        public override IList<string[]> Selectors =>
            new List<string[]>()
            {
                new [] {"N1","N3"},
                new [] {"N2","N4"},
            };

        public override PropertyType Type => PropertyType.Distance;
    }
}

using PorphyStruct.Chemistry;
using PorphyStruct.Core.Util;
using System.Collections.Generic;

namespace PorphyStruct.Test
{
    internal static class MacrocycleSetup
    {
        internal static Macrocycle CreateWithDummyAtom(Macrocycle.Type type)
        {
            Atom dummy = new Atom("Co42", 0, 0, 0);
            return MacrocycleFactory.Build(new AsyncObservableCollection<Atom>(new List<Atom>() { dummy }), type);
        }
    }
}

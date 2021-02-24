using ChemSharp.Molecules;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Core
{
    public static class Constants
    {
        public static readonly List<string> DeadEnds = Element.DesiredSaturation.Where(s => s.Value == 1).Select(s => s.Key).ToList();
    }
}

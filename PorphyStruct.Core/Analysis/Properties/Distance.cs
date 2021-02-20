using ChemSharp.Mathematics;
using ChemSharp.Molecules;

namespace PorphyStruct.Core.Analysis.Properties
{
    public class Distance
    {
        public double Value => MathV.Distance(Atom1.Location, Atom2.Location);
        public Atom Atom1 { get; set; }
        public Atom Atom2 { get; set; }

        public Distance(Atom a1, Atom a2)
        {
            Atom1 = a1;
            Atom2 = a2;
        }

        public override string ToString() => ToString("{0:N}");

        public string ToString(string format) =>
            $"{Atom1.Title} - {Atom2.Title} : {string.Format(format, Value)} Å";
    }
}

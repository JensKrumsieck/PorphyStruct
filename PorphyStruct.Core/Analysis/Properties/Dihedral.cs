using ChemSharp.Mathematics;
using ChemSharp.Molecules;

namespace PorphyStruct.Core.Analysis.Properties
{
    public class Dihedral
    {
        public double Value => MathV.Dihedral(Atom1.Location, Atom2.Location, Atom3.Location, Atom4.Location);
        public Atom Atom1 { get; set; }
        public Atom Atom2 { get; set; }
        public Atom Atom3 { get; set; }
        public Atom Atom4 { get; set; }

        public Dihedral(Atom a1, Atom a2, Atom a3, Atom a4) {
           
            Atom1 = a1;
            Atom2 = a2;
            Atom3 = a3;
            Atom4 = a4;
        }

        public override string ToString() => ToString("{0:N}");

        public string ToString(string format) =>
            $"{Atom1.Title} - {Atom2.Title} - {Atom3.Title} - {Atom4.Title}: {string.Format(format, Value)} °";
    }
}

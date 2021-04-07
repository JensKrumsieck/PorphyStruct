using ChemSharp.Molecules;
using ChemSharp.Molecules.Properties;
using PorphyStruct.Core.Extension;

namespace PorphyStruct.Core.Analysis.Properties
{
    public class N4Cavity : KeyValueProperty
    {
        public override double Value => MathUtil.Heron(_n1, _n2, _n3) + MathUtil.Heron(_n1, _n4, _n3);
        public override string Key => "Cavity size";
        public override string Unit => "Å²";

        private readonly Atom _n1;
        private readonly Atom _n2;
        private readonly Atom _n3;
        private readonly Atom _n4;
        public N4Cavity(Atom n1, Atom n2, Atom n3, Atom n4)
        {
            _n1 = n1;
            _n2 = n2;
            _n3 = n3;
            _n4 = n4;
        }
    }
}

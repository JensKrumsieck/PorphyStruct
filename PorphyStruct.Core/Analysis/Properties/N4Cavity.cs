using ChemSharp.Molecules;
using System;

namespace PorphyStruct.Core.Analysis.Properties
{
    public class N4Cavity : KeyValueProperty
    {
        public override double Value => HeronArea(_n1, _n2, _n3) + HeronArea(_n1, _n4, _n3);
        public override string Key => "Cavity size";
        public override string Unit => "Å²";

        private Atom _n1;
        private Atom _n2;
        private Atom _n3;
        private Atom _n4;
        public N4Cavity(Atom n1, Atom n2, Atom n3, Atom n4)
        {
            _n1 = n1;
            _n2 = n2;
            _n3 = n3;
            _n4 = n4;
        }

        /// <summary>
        /// Calculates area of triangle using heron's formula
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="a3"></param>
        /// <returns></returns>
        public double HeronArea(Atom a1, Atom a2, Atom a3)
        {
            var a = a1.DistanceTo(a2);
            var b = a2.DistanceTo(a3);
            var c = a3.DistanceTo(a1);
            var sqrt = (a + b + c) * (-a + b + c) * (a - b + c) * (a + b - c);
            return 1f / 4f * MathF.Sqrt(sqrt);
        }
    }
}

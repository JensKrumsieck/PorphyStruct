using ChemSharp.Mathematics;
using ChemSharp.Molecules;
using System.Numerics;

namespace PorphyStruct.Core.Analysis.Properties
{
    public class Angle
    {
        public double Value => MathV.Angle(Atom1.Location, Atom2.Location, Atom3.Location);
        public Atom Atom1 { get; set; }
        public Atom Atom2 { get; set; }
        public Atom Atom3 { get; set; }

        public Angle(Atom a1, Atom a2, Atom a3)
        {
            Atom1 = a1;
            Atom2 = a2;
            Atom3 = a3; 
        }

        public override string ToString() => ToString("{0:N}");

        public string ToString(string format) =>
            $"{Atom1.Title} - {Atom2.Title} - {Atom3.Title} : {string.Format(format, Value)} °";

        /// <summary>
        /// Handles each set of atoms as plane and returns Angle
        /// </summary>
        /// <param name="angle2"></param>
        /// <returns></returns>
        public double PlaneAngle(Angle angle2)
        {
            var thisPlane = Plane.CreateFromVertices(Atom1.Location, Atom2.Location, Atom3.Location);
            var otherPlane = Plane.CreateFromVertices(angle2.Atom1.Location, angle2.Atom2.Location, angle2.Atom3.Location);
            return MathV.Angle(thisPlane, otherPlane);
        }
    }
}

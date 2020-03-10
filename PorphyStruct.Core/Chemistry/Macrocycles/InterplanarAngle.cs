using MathNet.Spatial.Euclidean;
using PorphyStruct.Core.Util;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class InterplanarAngle : AbstractPropertyProvider
    {
        /// <summary>
        /// Add Metal to Constructor
        /// </summary>
        /// <param name="function"></param>
        /// <param name="Metal"></param>
        public InterplanarAngle(Func<string, Atom> function, Atom Metal) : base(function)
        {
            this.Metal = Metal;
        }

        public InterplanarAngle(Func<string, Atom> function) : base(function) { }

        public Atom Metal { get; set; }

        public override IList<string[]> Selectors =>
            new List<string[]>()
            {
                new [] {"N1", Metal.Identifier, "N4"},
                new [] {"N2", Metal.Identifier, "N3"},
            };

        public override PropertyType Type => PropertyType.Angle;

        /// <summary>
        /// Override default PropertyCalculation
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Property> CalculateProperties()
        {
            IList<Plane> planes = new List<Plane>();
            foreach (var s in Selectors)
                planes.Add(Plane.FromPoints(AtomFunction(s[0]).ToPoint3D(), AtomFunction(s[1]).ToPoint3D(), AtomFunction(s[2]).ToPoint3D()));

            //Interplanar angle is angle between N1-M-N4 and N2-M-N3 Planes!
            yield return new Property($"[{string.Join("-", Selectors[0])}]x[{string.Join("-", Selectors[1])}]", MathUtil.Angle(planes[0], planes[1]).ToString("G4") + "°");
        }
    }
}

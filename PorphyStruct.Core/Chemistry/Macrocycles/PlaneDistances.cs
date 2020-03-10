using MathNet.Spatial.Euclidean;
using PorphyStruct.Core.Util;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class PlaneDistances : AbstractPropertyProvider
    {
        /// <summary>
        /// Add Metal to Constructor
        /// </summary>
        /// <param name="function"></param>
        /// <param name="Metal"></param>
        public PlaneDistances(Atom Metal, IEnumerable<Plane> Planes, IEnumerable<string> PlaneIdentifiers) : base()
        {
            this.Metal = Metal;
            this.Planes = Planes;
            this.PlaneIdentifiers = PlaneIdentifiers;
        }
        public Atom Metal { get; set; }
        public IEnumerable<Plane> Planes { get; set; }
        public IEnumerable<string> PlaneIdentifiers { get; set; }

        public override PropertyType Type => PropertyType.Distance;

        /// <summary>
        /// Override default PropertyCalculation
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Property> CalculateProperties()
        {
            for (int i = 0; i < Planes.Count(); i++)
                yield return new Property($"{Metal.Identifier}-{PlaneIdentifiers.ElementAt(i)}", Metal.DistanceToPlane(Planes.ElementAt(i)).ToString("G3") + " Å");
        }

        /** DANGER ZONE**/

        /// <summary>
        /// Do not use this Constructor here as it's not resulting in any content 
        /// Special Case!
        /// </summary>
        /// <param name="function"></param>
        public PlaneDistances(Func<string, Atom> function) : base(function) { }
        public override IList<string[]> Selectors => throw new NotImplementedException();
    }
}

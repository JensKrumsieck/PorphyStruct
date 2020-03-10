using PorphyStruct.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class PlaneDistances : AbstractPropertyProvider
    {
        /// <summary>
        /// Add Metal to Constructor
        /// </summary>
        /// <param name="function"></param>
        /// <param name="Metal"></param>
        public PlaneDistances(IEnumerable<Atom> Atoms) : base()
        {
            this.Atoms = Atoms;
        }
        public IEnumerable<Atom> Atoms { get; set; }

        public override PropertyType Type => PropertyType.Distance;

        /// <summary>
        /// Override default PropertyCalculation
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Property> CalculateProperties()
        {
            var Metal = Atoms.Where(s => s.IsMetal).FirstOrDefault(); 
            yield return new Property($"{Metal.Identifier} - Mean Plane", Metal.DistanceToPlane(Molecule.GetMeanPlane(Atoms.Where(s => s.IsMacrocycle))).ToString("G3") + " Å");
            yield return new Property($"{Metal.Identifier} - N4 Plane", Metal.DistanceToPlane(Molecule.GetMeanPlane(Atoms.Where(s => s.IsMacrocycle && s.Identifier.Contains("N")))).ToString("G3") + " Å");
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

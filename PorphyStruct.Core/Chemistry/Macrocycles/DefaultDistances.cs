using PorphyStruct.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class DefaultDistances : AbstractPropertyProvider
    {
        /// <summary>
        /// Add Metal to Constructor
        /// </summary>
        /// <param name="function"></param>
        /// <param name="Metal"></param>
        public DefaultDistances(Atom Metal, IList<Atom> Atoms) : base()
        {
            this.Metal = Metal;
            this.Atoms = Atoms;
        }
        public Atom Metal { get; set; }
        public IList<Atom> Atoms { get; set; }

        public override PropertyType Type => PropertyType.Distance;

        /// <summary>
        /// Override default PropertyCalculation
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Property> CalculateProperties() => 
            from s in Atoms
                   select new Property($"{Metal.Identifier}-{s.Identifier}", Atom.Distance(Metal, s).ToString("G3") + " Å");

        /** DANGER ZONE**/

        /// <summary>
        /// Do not use this Constructor here as it's not resulting in any content 
        /// Special Case!
        /// </summary>
        /// <param name="function"></param>
        public DefaultDistances(Func<string, Atom> function) : base(function) { }
        public override IList<string[]> Selectors => throw new NotImplementedException();
    }
}

using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Properties
{
    public enum PropertyType { Distance, Angle, Dihedral, Parameter, Simulation }
    public interface IPropertyProvider
    {
        /// <summary>
        /// The PropertyType
        /// </summary>
        public PropertyType Type { get; }

        /// <summary>
        /// A List of Identifiers 
        /// (Bond needs 2, Angle 3, Dihedral 4 Identifiers)
        /// </summary>
        public IList<string[]> Selectors { get; }

        /// <summary>
        /// Converts Atoms to 
        /// </summary>
        public Func<string, Atom> AtomFunction { get; set; }

        /// <summary>
        /// Returns the actual properties
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Property> CalculateProperties();
    }
}

using PorphyStruct.Chemistry;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Core.Util
{
    /// <summary>
    /// Abstract Implementation of IPropertyProvider
    /// </summary>
    public abstract class AbstractPropertyProvider : IPropertyProvider
    {
        //base constructor
        public AbstractPropertyProvider() {}

        public AbstractPropertyProvider(Func<string, Atom> function) : this()
        {
            AtomFunction = function;
        }

        public abstract PropertyType Type { get; }

        public abstract IList<string[]> Selectors { get; }

        public Func<string, Atom> AtomFunction { get; set; }

        public virtual IEnumerable<Property> CalculateProperties()
        {
            Func<IList<Atom>, double> func = Type switch
            {
                PropertyType.Angle => MathUtil.Angle,
                PropertyType.Dihedral => MathUtil.Dihedral,
                PropertyType.Distance => MathUtil.Distance,
                _ => null
            };

            string unit = Type switch
            {
                PropertyType.Distance => " Å",
                _ => "°"
            };
            return from s in Selectors
                   select new Property(string.Join("-", s), func(s.Select(a => AtomFunction(a)).ToList()).ToString("G4") + unit);
        }
    }
}

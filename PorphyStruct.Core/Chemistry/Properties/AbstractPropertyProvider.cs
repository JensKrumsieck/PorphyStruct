using PorphyStruct.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry.Properties
{
    /// <summary>
    /// Abstract Implementation of IPropertyProvider
    /// </summary>
    public abstract class AbstractPropertyProvider : IPropertyProvider
    {
        //base constructor
        public AbstractPropertyProvider() { }

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
            return from selector in Selectors
                   where selector.Select(identifier => AtomFunction(identifier)).Count(item => item == null) == 0 //Validate AtomFunction
                   select new Property(string.Join("-", selector), func(selector.Select(identifier => AtomFunction(identifier)).ToList()).ToString("G4") + unit);
        }
    }
}

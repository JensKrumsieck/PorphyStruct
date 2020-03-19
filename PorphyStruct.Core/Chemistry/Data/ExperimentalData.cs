using PorphyStruct.Chemistry.Properties;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Data
{
    public class ExperimentalData : AbstractAtomDataPointProvider, IPropertyProvider
    {
        public override DataType DataType => DataType.Experimental;

        public ExperimentalData(IEnumerable<AtomDataPoint> dataPoints) => DataPoints = dataPoints;

        //No need to keep track of states as Experimental is regenrated every time
        public override void Normalize() => DataPoints = base.DataPoints.Normalize();

        public override void Invert() => DataPoints = DataPoints.Invert();

        public override int Priority => 0;


        //Properties provided by ExperimentalData (D OOP)
        public PropertyType Type => PropertyType.Parameter;

        public IEnumerable<Property> CalculateProperties()
        {
            yield return new Property("OOP-Distortion (exp.)", DataPoints.MeanDisplacement().ToString("G6") + " Å");
        }
        /*
        UNUSED
        */
        public IList<string[]> Selectors => null;

        public Func<string, Atom> AtomFunction { get; set; }
    }
}

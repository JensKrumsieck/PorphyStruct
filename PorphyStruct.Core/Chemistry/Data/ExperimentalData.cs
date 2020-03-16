using PorphyStruct.Util;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Data
{
    public class ExperimentalData : AbstractAtomDataPointProvider
    {
        public override DataType DataType => DataType.Experimental;

        public ExperimentalData(IEnumerable<AtomDataPoint> dataPoints) => DataPoints = dataPoints;

        //No need to keep track of states as Experimental is regenrated every time
        public override void Normalize() => DataPoints = DataPoints.Normalize();

        public override void Invert() => DataPoints = DataPoints.Invert();

        public override int Priority => 0;
    }
}

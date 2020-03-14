using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Data
{
    public class ExperimentalData : IAtomDataPointProvider
    {
        public DataType DataType => DataType.Experimental;
        public IEnumerable<AtomDataPoint> DataPoints { get; set; }

        public ExperimentalData(IEnumerable<AtomDataPoint> dataPoints) => DataPoints = dataPoints;
    }
}

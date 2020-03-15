using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Data
{
    public class CompareData : AbstractAtomDataPointProvider
    {
        public override DataType DataType => DataType.Comparison;

        public string FileName { get; set; }

        public CompareData(IEnumerable<AtomDataPoint> dataPoints, string filename)
        {
            DataPoints = dataPoints;
            FileName = filename;
        }
    }
}

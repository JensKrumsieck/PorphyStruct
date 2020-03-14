using PorphyStruct.Util;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Data
{
    public class DifferenceData : IAtomDataPointProvider
    {
        public DataType DataType => DataType.Difference;

        public IEnumerable<AtomDataPoint> DataPoints { get; set; }

        public DifferenceData(ExperimentalData exp, SimulationData sim) => DataPoints = exp.DataPoints.Difference(sim.DataPoints);
    }
}

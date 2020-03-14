using PorphyStruct.Util;

namespace PorphyStruct.Chemistry.Data
{
    public class DifferenceData : AbstractAtomDataPointProvider
    {
        public override DataType DataType => DataType.Difference;
        public DifferenceData(ExperimentalData exp, SimulationData sim) => DataPoints = exp.DataPoints.Difference(sim.DataPoints);
    }
}

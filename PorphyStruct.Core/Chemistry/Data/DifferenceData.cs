using PorphyStruct.Util;

namespace PorphyStruct.Chemistry.Data
{
    public class DifferenceData : AbstractAtomDataPointProvider
    {
        public override DataType DataType => DataType.Difference;
        public override int Priority => 2;
        public DifferenceData(ExperimentalData exp, SimulationData sim) => DataPoints = exp.DataPoints.Difference(sim.DataPoints);
    }
}

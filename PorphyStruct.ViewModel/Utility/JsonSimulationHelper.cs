using ChemSharp.Molecules.Properties;

namespace PorphyStruct.ViewModel.Utility
{
    public struct JsonSimulationHelper
    {
        public List<KeyValueProperty> SimulationResult { get; set; }
        public List<double> ConformationY { get; set; }
        public KeyValueProperty OutOfPlaneParameter { get; set; }
    }
}
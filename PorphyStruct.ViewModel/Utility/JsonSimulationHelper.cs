using ChemSharp.Molecules.Properties;
using System.Collections.Generic;

namespace PorphyStruct.ViewModel.Utility
{
    public struct JsonSimulationHelper
    {
        public List<KeyValueProperty> SimulationResult { get; set; }
        public List<double> ConformationY { get; set; }
        public KeyValueProperty OutOfPlaneParameter { get; set; }
    }
}
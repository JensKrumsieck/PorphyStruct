using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Data
{
    /// <summary>
    /// Interface for all types that want to inject data into Macrocycles
    /// </summary>
    public interface IAtomDataPointProvider
    {
        /// <summary>
        /// Indicates from which type the data is
        /// </summary>
        public DataType DataType { get; }

        /// <summary>
        /// Data provides by the provider
        /// </summary>
        public IEnumerable<AtomDataPoint> DataPoints { get; set; }
    }

    public enum DataType { Experimental, Simulation, Comparison, Difference }
}

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

        /// <summary>
        /// Handles Normalization
        /// </summary>
        public void Normalize();

        /// <summary>
        /// Handles Inversion
        /// </summary>
        public void Invert();

        public bool Normalized { get; set; }
        public bool Inverted { get; set; }
        public double Factor { get; set; }

        /// <summary>
        /// Priority to order for coloring
        /// </summary>
        public int Priority { get; }

    }

    public enum DataType { Experimental, Simulation, Comparison, Difference }
}

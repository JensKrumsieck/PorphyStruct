using OxyPlot;
using OxyPlot.Series;
using PorphyStruct.Chemistry;

namespace PorphyStruct
{
	/// <summary>
	/// Wrapper for OxyPlot.Series.ScatterPoint containing Atom Object
	/// </summary>
    public class AtomDataPoint : ScatterPoint
    {
        public Atom atom { get; set; }

		/// <summary>
		/// Constructs a new AtomDataPoint
		/// </summary>
		/// <param name="x">X Coordinate in Angstrom</param>
		/// <param name="y">Y Coordinate in Angstrom</param>
		/// <param name="atom">The Atom where the information comes from</param>
		/// <param name="size"></param>
		/// <param name="value">Used internally for coloring</param>
		/// <param name="tag"></param>
		public AtomDataPoint(double x, double y, Atom atom, double size = double.NaN, double value = double.NaN, object tag = null)
            : base(x, y, size, value, tag)
        {
            this.atom = atom;
			this.Value = x;
        }

		/// <summary>
		/// Convert to Parent Class (ScatterPoint)
		/// </summary>
		/// <returns>ScatterPoint</returns>
        public ScatterPoint GetScatterPoint()
        {
            return new ScatterPoint(X, Y);
        }

		/// <summary>
		/// Convert to DataPoint
		/// </summary>
		/// <returns>DataPoint</returns>
        public DataPoint GetDataPoint()
        {
            return new DataPoint(X, Y);
        }
    }
}

using ChemSharp.Molecules;
using OxyPlot;
using OxyPlot.Series;

namespace PorphyStruct
{
    public class AtomDataPoint : ScatterPoint
    {
        public Atom Atom { get; set; }

        public AtomDataPoint(double x, double y, Atom atom, double size = double.NaN, double value = double.NaN, object tag = null)
            : base(x, y, size, value, tag)
        {
            Atom = atom;
            Value = atom.AtomicNumber;
        }

        public DataPoint ToDataPoint() => new DataPoint(X, Y);

        public static string TrackerFormatString = "{0} \r\n{1}: {2} \r\n{3}: {4} \r\n{Atom}";
    }
}

using ChemSharp.Molecules;
using OxyPlot;
using OxyPlot.Series;

namespace PorphyStruct.Core.Plot;

public sealed class AtomDataPoint : ScatterPoint
{
    public Atom Atom { get; private set; }

    public AtomDataPoint(double x, double y, Atom atom, double size = double.NaN, double value = double.NaN, object? tag = null)
        : base(x, y, size, value, tag)
    {
        Atom = atom;
        Value = atom.AtomicNumber;
        if (Settings.Instance.UseAtomRadiusMarkerSize)
            Size = Settings.Instance.MarkerSize * Atom.CovalentRadius / 77d;
        if (Settings.Instance.NotMarkedPoints.Split(",").Contains(atom.Title) ||
            Settings.Instance.NotMarkedPoints.Split(",").Contains(atom.Symbol))
            Size = 0;
        Tag = tag;
    }

    public DataPoint ToDataPoint() => new(X, Y);

    public const string TrackerFormatString = "{0} \r\n{1}: {2} \r\n{3}: {4} \r\n{Atom} \r\nMapped to {Tag}";
}

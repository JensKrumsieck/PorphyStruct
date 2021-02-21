using OxyPlot;
using OxyPlot.Series;

namespace PorphyStruct.Core.Plot
{
    public class DefaultScatterSeries : ScatterSeries
    {
        public bool Inverted;

        public DefaultScatterSeries()
        {
            TrackerFormatString = AtomDataPoint.TrackerFormatString;
            ColorAxisKey = "colors";
            MarkerType = Settings.Instance.MarkerType;
            MarkerStrokeThickness = Settings.Instance.BorderThickness;
            MarkerStroke = OxyColor.Parse(Settings.Instance.MarkerBorderColor);
            MarkerSize = Settings.Instance.MarkerSize;
            Mapping = InverseMapping;
        }

        private ScatterPoint InverseMapping(object arg)
        {
            var adp = (AtomDataPoint)arg;
            var y = Inverted ? -adp.Y : adp.Y;
            return new AtomDataPoint(adp.X, y, adp.Atom);
        }
    }
}

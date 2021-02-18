using OxyPlot;
using OxyPlot.Series;

namespace PorphyStruct.Core.Plot
{
    public class DefaultScatterSeries : ScatterSeries
    {
        public DefaultScatterSeries()
        {
            TrackerFormatString = AtomDataPoint.TrackerFormatString;
            ColorAxisKey = "colors";
            MarkerType = Settings.Instance.MarkerType;
            MarkerStrokeThickness = Settings.Instance.BorderThickness;
            MarkerStroke = OxyColor.Parse(Settings.Instance.MarkerBorderColor);
            MarkerSize = Settings.Instance.MarkerSize;
        }
    }
}

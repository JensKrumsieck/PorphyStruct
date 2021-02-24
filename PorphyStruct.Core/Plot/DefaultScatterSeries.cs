using OxyPlot;
using OxyPlot.Series;
using System;
using System.Linq;

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
            if (Settings.Instance.SingleColor) MarkerFill = OxyColor.Parse(Settings.Instance.MarkerColor);
        }

        private ScatterPoint InverseMapping(object arg)
        {
            var adp = (AtomDataPoint)arg;
            var y = Inverted ? -adp.Y : adp.Y;
            return new AtomDataPoint(adp.X, y, adp.Atom);
        }

        public BondAnnotation CreateBondAnnotation(AtomDataPoint a1, AtomDataPoint a2)
        {
            var src = ItemsSource.Cast<AtomDataPoint>().ToList();
            var annotation = new BondAnnotation(
                    src.FirstOrDefault(s => s.Atom.Title == a1.Atom.Title && Math.Abs(s.X - a1.X) < 1),
                    src.FirstOrDefault(s => s.Atom.Title == a2.Atom.Title && Math.Abs(s.X - a2.X) < .51),
                    this)
                { Opacity = 128 };

            if (!Title.Contains("Simulation"))
                return annotation;

            annotation.Opacity = 255;
            annotation.Color = OxyColor.Parse(Settings.Instance.SimulationBondColor);
            return annotation;
        }
    }
}

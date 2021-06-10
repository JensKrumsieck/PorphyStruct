using OxyPlot;
using OxyPlot.Axes;

namespace PorphyStruct.Core.Plot
{
    public class BasePlotModel : PlotModel
    {
        public LinearAxis XAxis { get; }

        public BasePlotModel()
        {
            TitleFontWeight = 200;
            DefaultFontSize = Settings.Instance.FontSize;
            DefaultFont = Settings.Instance.Font;
            PlotAreaBorderThickness = new OxyThickness(Settings.Instance.BorderThickness);
            Padding = new OxyThickness(Settings.Instance.Padding, Settings.Instance.Padding, Settings.Instance.Padding, Settings.Instance.Padding);

            XAxis = new LinearAxis
            {
                Title = "X",
                Unit = "Å",
                Position = AxisPosition.Bottom,
                Key = "X",
                AxislineThickness = Settings.Instance.AxisThickness,
                TitleFormatString = Settings.Instance.AxisFormat
            };
            Axes.Add(XAxis);

            if (!PlotAreaBorderThickness.Equals(new OxyThickness(0))) return;
            XAxis.AxislineStyle = LineStyle.Solid;
        }
    }
}

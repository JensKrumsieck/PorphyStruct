using OxyPlot;
using OxyPlot.Axes;

namespace PorphyStruct.Core.Plot;

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
            FontWeight = Settings.Instance.FontWeight,
            FontSize = Settings.Instance.FontSize,
            Font = Settings.Instance.Font,
            AxislineThickness = Settings.Instance.AxisThickness,
            MinorGridlineThickness = Settings.Instance.AxisThickness / 2,
            MajorGridlineThickness = Settings.Instance.AxisThickness,
            MajorTickSize = Settings.Instance.AxisThickness * 3.5,
            MinorTickSize = Settings.Instance.AxisThickness * 2,
            TitleFormatString = Settings.Instance.AxisFormat
        };
        Axes.Add(XAxis);

        if (!PlotAreaBorderThickness.Equals(new OxyThickness(0))) return;
        XAxis.AxislineStyle = LineStyle.Solid;
    }
}

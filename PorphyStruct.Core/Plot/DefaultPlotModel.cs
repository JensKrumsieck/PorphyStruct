using OxyPlot;
using OxyPlot.Axes;

namespace PorphyStruct.Core.Plot;

public sealed class DefaultPlotModel : BasePlotModel
{
    public LinearAxis YAxis { get; }

    //Expose left padding
    public double PaddingLeft
    {
        get => Padding.Left;
        set => Padding = new OxyThickness(value, Padding.Top, Padding.Right, Padding.Bottom);
    }

    public DefaultPlotModel()
    {
        Padding = new OxyThickness(Settings.Instance.Padding + Settings.Instance.LabelPadding, Settings.Instance.Padding, Settings.Instance.Padding, Settings.Instance.Padding);

        XAxis.IsAxisVisible = Settings.Instance.ShowXAxis;

        YAxis = new LinearAxis
        {
            Title = "Δ_{msp}",
            Unit = "Å",
            Position = AxisPosition.Left,
            Key = "Y",
            IsAxisVisible = true,
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

        Axes.Add(YAxis);
        Axes.Add(ColorAxis);

        if (!PlotAreaBorderThickness.Equals(new OxyThickness(0))) return;
        YAxis.AxislineStyle = LineStyle.Solid;
        XAxis.AxislineStyle = LineStyle.Solid;
    }

    /// <summary>
    /// Adds zero line
    /// </summary>
    public void AddZero()
    {
        //add zero
        if (!Settings.Instance.ShowZero) return;
        //show zero
        var zero = new OxyPlot.Annotations.LineAnnotation()
        {
            Color = OxyColor.FromAColor(40, OxyColors.Gray),
            StrokeThickness = Settings.Instance.BondThickness,
            Intercept = 0,
            Slope = 0
        };
        Annotations.Add(zero);
    }

    /// <summary>
    /// Inits Model for Analysis
    /// </summary>
    public void Init()
    {
        Series.Clear();
        Annotations.Clear();
        AddZero();
    }

    /// <summary>
    /// Builds RangeColorAxis
    /// </summary>
    /// <returns></returns>
    public static AtomRangeColorAxis ColorAxis => new()
    {
        Key = "colors",
        Position = AxisPosition.Bottom,
        IsAxisVisible = false
    };

    private OxyColor _bondColor = OxyColor.Parse(Settings.Instance.BondColor);
    public OxyColor BondColor
    {
        get => _bondColor;
        set
        {
            _bondColor = value;
            SetBondProperty(nameof(BondAnnotation.Color), value);
        }
    }

    private double _bondThickness = Settings.Instance.BondThickness;

    public double BondThickness
    {
        get => _bondThickness;
        set
        {
            _bondThickness = value;
            SetBondProperty(nameof(BondAnnotation.StrokeThickness), value);
        }
    }

    private void SetBondProperty<T>(string prop, T value)
    {
        foreach (var annotation in Annotations)
        {
            if (annotation is not BondAnnotation b) continue;
            var pInfo = typeof(BondAnnotation).GetProperty(prop);
            pInfo?.SetValue(b, value);
        }
    }
}

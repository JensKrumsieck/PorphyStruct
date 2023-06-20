using OxyPlot;
using OxyPlot.Series;

namespace PorphyStruct.Core.Plot;

public sealed class DefaultScatterSeries : ScatterSeries
{
    public byte Opacity { get; set; } = 255;

    public DefaultScatterSeries()
    {
        TrackerFormatString = AtomDataPoint.TrackerFormatString;
        ColorAxisKey = "colors";
        MarkerType = Settings.Instance.MarkerType;
        MarkerStrokeThickness = Settings.Instance.BorderThickness;
        MarkerStroke = OxyColor.Parse(Settings.Instance.MarkerBorderColor);
        MarkerSize = Settings.Instance.MarkerSize;
        if (Settings.Instance.SingleColor) MarkerFill = OxyColor.Parse(Settings.Instance.MarkerColor);
        SelectionMode = SelectionMode.Single;
    }

    public BondAnnotation CreateBondAnnotation(AtomDataPoint a1, AtomDataPoint a2)
    {
        var src = ItemsSource.Cast<AtomDataPoint>().ToList();
        var annotation = new BondAnnotation(
                src.FirstOrDefault(s => s.Atom.Title == a1.Atom.Title && Math.Abs(s.X - a1.X) < .1)!,
                src.FirstOrDefault(s => s.Atom.Title == a2.Atom.Title && Math.Abs(s.X - a2.X) < .1)!,
                this)
        { Opacity = 255 };

        if (!Title.Contains("Simulation"))
            return annotation;

        annotation.Opacity = (byte)Math.Round(255u * Settings.Instance.SimulationOpacity);
        annotation.Color = OxyColor.Parse(Settings.Instance.SimulationBondColor);
        return annotation;
    }
    
    /// <inheritdoc/>
    /// see https://github.com/oxyplot/oxyplot/blob/develop/Source/OxyPlot/Series/ScatterSeries%7BT%7D.cs#L322-L460
    public override void Render(IRenderContext rc)
    {
        var actualPoints = ActualPointsList;
        if (actualPoints == null || actualPoints.Count == 0) return;

        var clippingRect = GetClippingRect();
        var n = actualPoints.Count;
        var allPoints = new List<ScreenPoint>(n);
        var allMarkerSizes = new List<double>(n);
        var selectedPoints = new List<ScreenPoint>();
        var selectedMarkerSizes = new List<double>(n);
        var groupPoints = new Dictionary<int, IList<ScreenPoint>>();
        var groupSizes = new Dictionary<int, IList<double>>();
        
        var isSelected = IsSelected();
        
        // Transform all points to screen coordinates
        for (var i = 0; i < n; i++)
        {
            var dp = new DataPoint(actualPoints[i].X, actualPoints[i].Y);
            // Skip invalid points
            if (!IsValidPoint(dp)) continue;

            var size = double.NaN;
            var value = double.NaN;

            var scatterPoint = actualPoints[i];
            if (scatterPoint != null)
            {
                size = scatterPoint.Size;
                value = scatterPoint.Value;
            }

            if (double.IsNaN(size)) size = MarkerSize;

            // Transform from data to screen coordinates
            var screenPoint = this.Transform(dp.X, dp.Y);

            if (isSelected && IsItemSelected(i))
            {
                selectedPoints.Add(screenPoint);
                selectedMarkerSizes.Add(size);
                continue;
            }
            
            if (ColorAxis != null)
            {
                if (double.IsNaN(value)) continue;

                var group = ColorAxis.GetPaletteIndex(value);
                if (!groupPoints.ContainsKey(group))
                {
                    groupPoints.Add(group, new List<ScreenPoint>());
                    groupSizes.Add(group, new List<double>());
                }

                groupPoints[group].Add(screenPoint);
                groupSizes[group].Add(size);
            }
            else
            {
                allPoints.Add(screenPoint);
                allMarkerSizes.Add(size);
            }
        }

        // Offset of the bins
        var binOffset = this.Transform(MinX, MaxY);

        if (ColorAxis != null)
        {
            // Draw the grouped (by color defined in ColorAxis) markers
            var markerIsStrokedOnly = MarkerType == MarkerType.Plus || MarkerType == MarkerType.Star || MarkerType == MarkerType.Cross;
            foreach (var (key, value) in groupPoints)
            {
                var color = ColorAxis.GetColor(key);
                rc.DrawMarkers(
                    value,
                    MarkerType,
                    MarkerOutline,
                    groupSizes[key],
                    OxyColor.FromAColor(Opacity, MarkerFill.GetActualColor(color)),
                    markerIsStrokedOnly ? color : MarkerStroke,
                    MarkerStrokeThickness,
                    EdgeRenderingMode,
                    BinSize,
                    binOffset);
            }
        }

        rc.DrawMarkers(
            allPoints,
            MarkerType,
            MarkerOutline,
            allMarkerSizes,
            OxyColor.FromAColor(Opacity, ActualMarkerFillColor),
            MarkerStroke,
            MarkerStrokeThickness,
            EdgeRenderingMode,
            BinSize,
            binOffset);

        // Draw the selected markers
        rc.DrawMarkers(
            selectedPoints,
            MarkerType,
            MarkerOutline,
            selectedMarkerSizes,
            PlotModel.SelectionColor,
            PlotModel.SelectionColor,
            MarkerStrokeThickness,
            EdgeRenderingMode,
            BinSize,
            binOffset);

        
        if (LabelFormatString != null) RenderPointLabels(rc, clippingRect);
    }
}

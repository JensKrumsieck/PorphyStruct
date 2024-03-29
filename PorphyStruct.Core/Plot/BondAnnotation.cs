﻿using OxyPlot;
using OxyPlot.Annotations;

namespace PorphyStruct.Core.Plot;

/// <summary>
/// BondAnnotation inspired by ArrowAnnotation
/// https://github.com/oxyplot/oxyplot/blob/develop/Source/OxyPlot/Annotations/ArrowAnnotation.cs
/// </summary>
public sealed class BondAnnotation : TransposableAnnotation
{
    private ScreenPoint _screenEndPoint;
    private ScreenPoint _screenStartPoint;

    public AtomDataPoint StartPoint { get; set; }
    public AtomDataPoint EndPoint { get; set; }

    public OxyColor Color { get; set; }
    public byte Opacity { get; set; } = 255;

    public OxyColor ActualColor => OxyColor.FromAColor(Opacity, Color);

    public double StrokeThickness { get; set; }
    public LineStyle LineStyle { get; set; }
    public LineJoin LineJoin { get; set; }

    public DefaultScatterSeries AssociatedSeries { get; set; }

    public BondAnnotation(AtomDataPoint a1, AtomDataPoint a2, DefaultScatterSeries associatedSeries)
    {
        StartPoint = a1;
        EndPoint = a2;
        Color = OxyColor.Parse(Settings.Instance.BondColor);
        StrokeThickness = Settings.Instance.BondThickness;
        Layer = AnnotationLayer.BelowSeries;
        Tag = $"{a1.Atom.Title} - {a2.Atom.Title} ({a1}, {a2})";
        AssociatedSeries = associatedSeries;
    }

    public override void Render(IRenderContext rc)
    {
        base.Render(rc);
        var sY = StartPoint.Y;
        var eY = EndPoint.Y;
        var actualStart = new DataPoint(StartPoint.X, sY);
        var actualEnd = new DataPoint(EndPoint.X, eY);
        _screenEndPoint = Transform(actualEnd);
        _screenStartPoint = Transform(actualStart);
        var d = _screenEndPoint - _screenStartPoint;
        d.Normalize();
        var n = new ScreenVector(d.Y, -d.X);

        const double minimumSegmentLength = 4;

        var dashArray = LineStyle.GetDashArray();

        if (!(StrokeThickness > 0) || LineStyle == LineStyle.None) return;
        rc.DrawReducedLine(
            new[] { _screenStartPoint, _screenEndPoint },
            minimumSegmentLength * minimumSegmentLength,
            GetSelectableColor(ActualColor),
            StrokeThickness,
            EdgeRenderingMode,
            dashArray,
            LineJoin);
    }
}

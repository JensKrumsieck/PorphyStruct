using OxyPlot;
using OxyPlot.Series;

namespace PorphyStruct.Core.Plot;

public sealed class MarginBarSeries : BarSeries
{
    private readonly double _margin;

    public MarginBarSeries(double margin)
    {
        _margin = margin;
    }

    public override void Render(IRenderContext rc)
    {
        ActualBarRectangles = new List<OxyRect>();
        if (!ValidItems.Any()) return;

        var actualMargin = GetActualMargin();
        var actualBarWidth = GetActualBarWidth();
        foreach (var item in ValidItems)
        {
            var categoryIndex = item.CategoryIndex;
            var value = item.Value;
            var x = actualMargin / 2 * Math.Sign(value);
            var topValue = x + value;
            var categoryValue = categoryIndex - 0.5 + Manager.GetCurrentBarOffset(categoryIndex);

            var rect = new OxyRect(this.Transform(x, categoryValue), this.Transform(topValue, categoryValue + actualBarWidth));
            ActualBarRectangles.Add(rect);

            RenderItem(rc, topValue, categoryValue, actualBarWidth, item, rect);
            RenderLabel(rc, item, x, topValue, categoryValue, categoryValue + actualBarWidth);
            Manager.IncreaseCurrentBarOffset(categoryIndex, actualBarWidth);
        }
        RenderSpacing(rc, actualMargin / 2, ValidItems);
    }

    private void RenderSpacing(IRenderContext rc, double x, ICollection<BarItem> items)
    {
        var mult = items.Count == 12 ? 2d : 1d;
        var tl = this.Transform(-x, items.Count / mult - .5);
        var br = this.Transform(x, -.5);
        var size = new OxySize(Math.Abs(tl.X - br.X) - 20, Math.Abs(br.Y - tl.Y) - 10);
        rc.DrawRectangle(new OxyRect(new ScreenPoint(tl.X + 10, tl.Y + 5), size), OxyColors.Transparent, OxyColors.Black, 1, EdgeRenderingMode.Adaptive);
    }

    private double GetActualMargin()
    {
        var zero = this.Transform(0, 0);
        //margin x in Screen point x
        var marginStart = zero.X - _margin / 2;
        var marginEnd = zero.X + _margin / 2;

        var start = this.InverseTransform(marginStart, 0);
        var end = this.InverseTransform(marginEnd, 0);

        return Math.Abs(start.X - end.X);
    }
}

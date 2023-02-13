using OxyPlot;
using OxyPlot.Series;

namespace PorphyStruct.Core.Plot;

public sealed class MarginBarSeries : BarSeries
{
    private readonly double _margin;

    public MarginBarSeries(double margin) => _margin = margin;

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
            var xStartValue = actualMargin / 2 * Math.Sign(value); //left or right from margin rectangle
            var topValue = xStartValue + value;
            var categoryValue = categoryIndex - 0.5 + Manager.GetCurrentBarOffset(categoryIndex);

            var rect = new OxyRect(
                                   this.Transform(xStartValue, categoryValue),
                                   this.Transform(topValue, categoryValue + actualBarWidth)
                                  );
            ActualBarRectangles.Add(rect);

            RenderItem(rc, topValue, categoryValue, actualBarWidth, item, rect);
            var offset = (Math.Sign(value) * categoryValue) + (Math.Abs(value) < 0.5 ? Math.Sign(value) : 0);
            RenderLabel(rc, item, xStartValue - offset, topValue, categoryValue, categoryValue + actualBarWidth);
            Manager.IncreaseCurrentBarOffset(categoryIndex, actualBarWidth);
        }

        RenderSpacing(rc, actualMargin / 2, ValidItems);
    }

    private void RenderSpacing(IRenderContext rc, double spacing, IList<BarItem> items)
    {
        var transformXTop = this.Transform(-spacing * (items.Count > 6 ? 1 : .75), 0);
        var transformXBottom = this.Transform(spacing, 0);
        var rectangle = new OxyRect(
                                    new ScreenPoint(transformXTop.X, ActualBarRectangles[0].Bottom),
                                    new ScreenPoint(transformXBottom.X, ActualBarRectangles[^1].Top)
                                   );
        rc.DrawRectangle(rectangle, OxyColors.Transparent, OxyColors.Black, 1, EdgeRenderingMode.Adaptive);
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
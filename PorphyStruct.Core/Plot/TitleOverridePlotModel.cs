using OxyPlot;

namespace PorphyStruct.Core.Plot
{
    public class TitleOverridePlotModel : BasePlotModel
    {
        protected override void RenderOverride(IRenderContext rc, OxyRect rect)
        {
            //save titles
            var title = Title;
            var subTitle = Subtitle;

            //unset titles
            Title = " ";
            Subtitle = " ";
            //should not render titles so we can do ourselves
            base.RenderOverride(rc, rect);

            //restore titles and render
            Title = title;
            Subtitle = subTitle;
            RenderTitle(rc);
        }

        public double SubTitleMargin = 5;

        /// <summary>
        /// Override private render method
        /// <inheritdoc cref="PlotModel.RenderTitle"/>
        /// </summary>
        /// <param name="rc"></param>
        private void RenderTitle(IRenderContext rc)
        {
            OxySize? maxSize = null;

            if (ClipTitle) maxSize = new OxySize(TitleArea.Width * TitleClippingLength, double.MaxValue);

            var titleSize = rc.MeasureText(Title, ActualTitleFont, TitleFontSize, TitleFontWeight);

            var x = (TitleArea.Left + TitleArea.Right) * 0.5;
            var y = TitleArea.Top;

            if (!string.IsNullOrEmpty(Title))
            {
                rc.SetToolTip(TitleToolTip);

                rc.DrawMathText(
                    new ScreenPoint(x, y),
                    Title,
                    TitleColor.GetActualColor(TextColor),
                    ActualTitleFont,
                    TitleFontSize,
                    TitleFontWeight,
                    0,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Top,
                    maxSize);
                y += titleSize.Height + SubTitleMargin;

                rc.SetToolTip(null);
            }

            if (!string.IsNullOrEmpty(Subtitle))
            {
                rc.DrawMathText(
                    new ScreenPoint(x, y),
                    Subtitle,
                    SubtitleColor.GetActualColor(TextColor),
                    ActualSubtitleFont,
                    SubtitleFontSize,
                    SubtitleFontWeight,
                    0,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Top,
                    maxSize);
            }
        }
    }
}

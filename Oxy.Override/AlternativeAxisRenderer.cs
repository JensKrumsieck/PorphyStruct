using OxyPlot;
using OxyPlot.Axes;
using System;

namespace PorphyStruct.Oxy.Override
{
    public class AlternativeAxisRenderer : HorizontalAndVerticalAxisRenderer
    {
        public AlternativeAxisRenderer(IRenderContext rc, PlotModel plot)
            : base(rc, plot)
        {
        }

        /// <summary>
        /// Gets the axis title position, rotation and alignment.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="titlePosition">The title position.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="halign">The horizontal alignment.</param>
        /// <param name="valign">The vertical alignment.</param>
        /// <see cref="HorizontalAndVerticalAxisRenderer.GetAxisTitlePositionAndAlignment(Axis, double, ref double, ref HorizontalAlignment, ref VerticalAlignment)"/>
        /// <returns>The <see cref="ScreenPoint" />.</returns>
        protected override ScreenPoint GetAxisTitlePositionAndAlignment(
            Axis axis,
            double titlePosition,
            ref double angle,
            ref HorizontalAlignment halign,
            ref VerticalAlignment valign)
        {
            if (axis.Position == AxisPosition.Left && PorphyStruct.Properties.Settings.Default.rotateTitle)
                titlePosition += 10;
            double middle = axis.IsHorizontal()
                                ? Lerp(axis.ScreenMin.X, axis.ScreenMax.X, axis.TitlePosition)
                                : Lerp(axis.ScreenMax.Y, axis.ScreenMin.Y, axis.TitlePosition);

            if (axis.PositionAtZeroCrossing)
            {
                middle = Lerp(axis.Transform(axis.ActualMaximum), axis.Transform(axis.ActualMinimum), axis.TitlePosition);
            }

            switch (axis.Position)
            {
                case AxisPosition.Left:
                    if (PorphyStruct.Properties.Settings.Default.rotateTitle)
                    {
                        angle = 0;
                    }
                    return new ScreenPoint(titlePosition, middle);
                case AxisPosition.Right:
                    valign = VerticalAlignment.Bottom;
                    return new ScreenPoint(titlePosition, middle);
                case AxisPosition.Top:
                    halign = HorizontalAlignment.Center;
                    valign = VerticalAlignment.Top;
                    angle = 0;
                    return new ScreenPoint(middle, titlePosition);
                case AxisPosition.Bottom:
                    halign = HorizontalAlignment.Center;
                    valign = VerticalAlignment.Bottom;
                    angle = 0;
                    return new ScreenPoint(middle, titlePosition);
                default:
                    throw new ArgumentOutOfRangeException("axis");
            }
        }
    }
}

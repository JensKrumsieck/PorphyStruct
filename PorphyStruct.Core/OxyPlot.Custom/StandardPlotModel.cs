using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.OxyPlotOverride
{
    /// <summary>
    /// Contains the default settings for plots
    /// </summary>
    public class StandardPlotModel : PlotModel
    {
        public OxyPlot.Axes.LinearAxis xAxis;
        public LinearAxis yAxis;

        public StandardPlotModel()
            : base()
        {
            Padding = new OxyThickness(Core.Properties.Settings.Default.plotPadding);
            IsLegendVisible = false;
            LegendPosition = LegendPosition.RightTop;
            DefaultFontSize = Core.Properties.Settings.Default.defaultFontSize;
            LegendFontSize = Core.Properties.Settings.Default.defaultFontSize;
            DefaultFont = Core.Properties.Settings.Default.defaultFont;
            PlotAreaBorderThickness = new OxyThickness(Core.Properties.Settings.Default.lineThickness);
            TitleFontWeight = 200;

            var x = new OxyPlot.Axes.LinearAxis
            {
                Title = "X",
                Unit = "Å",
                Position = AxisPosition.Bottom,
                Key = "X",
                IsAxisVisible = Core.Properties.Settings.Default.xAxis,
                MajorGridlineThickness = Core.Properties.Settings.Default.lineThickness,
                AbsoluteMinimum = Core.Properties.Settings.Default.minX,
                AbsoluteMaximum = Core.Properties.Settings.Default.maxX,
                TitleFormatString = Core.Properties.Settings.Default.titleFormat,
                LabelFormatter = OxyUtils.xFormatter
            };

            //use my override for title rotation :) works unexpectedly good :D
            var y = new LinearAxis
            {
                Title = "Δ_{msp}",
                Unit = "Å",
                Position = AxisPosition.Left,
                Key = "Y",
                IsAxisVisible = true,
                MajorGridlineThickness = Core.Properties.Settings.Default.lineThickness,
                TitleFormatString = Core.Properties.Settings.Default.titleFormat,
                LabelFormatter = OxyUtils.yFormatter
            };

            //handle settings
            if (Core.Properties.Settings.Default.rotateTitle) y.AxisTitleDistance = 20;

            if (!Core.Properties.Settings.Default.showBox)
            {
                PlotAreaBorderThickness = new OxyThickness(0);
                y.AxislineStyle = LineStyle.Solid;
                y.AxislineThickness = Core.Properties.Settings.Default.lineThickness;
                x.AxislineStyle = LineStyle.Solid;
                x.AxislineThickness = Core.Properties.Settings.Default.lineThickness;
            }

            Axes.Add(x);
            Axes.Add(y);

            //make it accessible
            xAxis = x;
            yAxis = y;
            AddZero();
        }

        /// <summary>
        /// Handles scaling
        /// </summary>
        /// <param name="a"></param>
        /// <param name="isY"></param>
        /// <param name="normalize"></param>
        public void Scale(Axis a, bool isY = false, bool normalize = false)
        {
            string val = isY ? "Y" : "X";

            double min, max;
            min = double.PositiveInfinity;
            max = double.NegativeInfinity;
            if ((!isY && Core.Properties.Settings.Default.autoscaleX) || (isY && Core.Properties.Settings.Default.autoscaleY))
            {
                //find min & max automatically
                foreach (ScatterSeries s in Series)
                {
                    double series_min = ((IEnumerable<AtomDataPoint>)s.ItemsSource).Min(dp => Convert.ToDouble(dp.GetType().GetProperty(val).GetValue(dp, null)));
                    double series_max = ((IEnumerable<AtomDataPoint>)s.ItemsSource).Max(dp => Convert.ToDouble(dp.GetType().GetProperty(val).GetValue(dp, null)));
                    min = Math.Min(series_min, min);
                    max = Math.Max(series_max, max);
                }
                min -= (isY ? 0.05 : 1);
                max += (isY ? 0.05 : 1);
            }
            else
            {
                //set min & max manually
                min = (isY ? Core.Properties.Settings.Default.minY : Core.Properties.Settings.Default.minX);
                max = (isY ? Core.Properties.Settings.Default.maxY : Core.Properties.Settings.Default.maxX);
            }

            //normalize?
            min = isY && normalize ? -1.1 : min;
            max = isY && normalize ? 1.1 : max;

            a.AbsoluteMinimum = min;
            a.AbsoluteMaximum = max;
            a.Zoom(min, max);

            InvalidatePlot(true);
        }

        /// <summary>
        /// Adds zero line
        /// </summary>
        public void AddZero()
        {
            //add zero
            if (Core.Properties.Settings.Default.zero)
            {
                //show zero
                var zero = new OxyPlot.Annotations.LineAnnotation()
                {
                    Color = OxyColor.FromAColor(40, OxyColors.Gray),
                    StrokeThickness = Core.Properties.Settings.Default.lineThickness,
                    Intercept = 0,
                    Slope = 0
                };
                Annotations.Add(zero);
            }
        }
    }
}

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
            IsLegendVisible = false;
            LegendPosition = LegendPosition.RightTop;
            DefaultFontSize = PorphyStruct.Core.Settings.Default.defaultFontSize;
            LegendFontSize = PorphyStruct.Core.Settings.Default.defaultFontSize;
            DefaultFont = PorphyStruct.Core.Settings.Default.defaultFont;
            PlotAreaBorderThickness = new OxyThickness(PorphyStruct.Core.Settings.Default.lineThickness);

            OxyPlot.Axes.LinearAxis x = new OxyPlot.Axes.LinearAxis
            {
                Title = "X",
                Unit = "Å",
                Position = AxisPosition.Bottom,
                Key = "X",
                IsAxisVisible = PorphyStruct.Core.Settings.Default.xAxis,
                MajorGridlineThickness = PorphyStruct.Core.Settings.Default.lineThickness,
                AbsoluteMinimum = PorphyStruct.Core.Settings.Default.minX,
                AbsoluteMaximum = PorphyStruct.Core.Settings.Default.maxX,
                TitleFormatString = PorphyStruct.Core.Settings.Default.titleFormat,
                LabelFormatter = OxyUtils._axisFormatter
            };

            //use my override for title rotation :) works unexpectedly good :D
            LinearAxis y = new LinearAxis
            {
                Title = "Δ_{msp}",
                Unit = "Å",
                Position = AxisPosition.Left,
                Key = "Y",
                IsAxisVisible = true,
                MajorGridlineThickness = PorphyStruct.Core.Settings.Default.lineThickness,
                TitleFormatString = PorphyStruct.Core.Settings.Default.titleFormat,
                LabelFormatter = OxyUtils._axisFormatter
            };

            //handle settings
            if (PorphyStruct.Core.Settings.Default.rotateTitle)
            {
                y.AxisTitleDistance = 15;
            }

            if (!PorphyStruct.Core.Settings.Default.showBox)
            {
                PlotAreaBorderThickness = new OxyThickness(0);
                y.AxislineStyle = LineStyle.Solid;
                y.AxislineThickness = PorphyStruct.Core.Settings.Default.lineThickness;
                x.AxislineStyle = LineStyle.Solid;
                x.AxislineThickness = PorphyStruct.Core.Settings.Default.lineThickness;
            }

            this.Axes.Add(x);
            this.Axes.Add(y);

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
            if ((!isY && PorphyStruct.Core.Settings.Default.autoscaleX) || (isY && PorphyStruct.Core.Settings.Default.autoscaleY))
            {
                //find min & max automatically
                foreach (ScatterSeries s in this.Series)
                {
                    double series_min = ((List<AtomDataPoint>)s.ItemsSource).Min(dp => Convert.ToDouble(dp.GetType().GetProperty(val).GetValue(dp, null)));
                    double series_max = ((List<AtomDataPoint>)s.ItemsSource).Max(dp => Convert.ToDouble(dp.GetType().GetProperty(val).GetValue(dp, null)));
                    min = Math.Min(series_min, min);
                    max = Math.Max(series_max, max);
                }
                min -= (isY ? 0.05 : 1);
                max += (isY ? 0.05 : 1);
            }
            else
            {
                //set min & max manually
                min = (isY ? PorphyStruct.Core.Settings.Default.minY : PorphyStruct.Core.Settings.Default.minX);
                max = (isY ? PorphyStruct.Core.Settings.Default.maxY : PorphyStruct.Core.Settings.Default.maxX);
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
            if (PorphyStruct.Core.Settings.Default.zero)
            {
                //show zero
                OxyPlot.Annotations.LineAnnotation zero = new OxyPlot.Annotations.LineAnnotation()
                {
                    Color = OxyColor.FromAColor(40, OxyColors.Gray),
                    StrokeThickness = PorphyStruct.Core.Settings.Default.lineThickness,
                    Intercept = 0,
                    Slope = 0
                };
                Annotations.Add(zero);
            }
        }
    }
}

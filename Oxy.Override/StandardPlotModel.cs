using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Oxy.Override
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
            DefaultFontSize = Properties.Settings.Default.defaultFontSize;
            LegendFontSize = Properties.Settings.Default.defaultFontSize;
            DefaultFont = Properties.Settings.Default.defaultFont;
            PlotAreaBorderThickness = new OxyThickness(Properties.Settings.Default.lineThickness);

            OxyPlot.Axes.LinearAxis x = new OxyPlot.Axes.LinearAxis
            {
                Title = "X",
                Unit = "Å",
                Position = AxisPosition.Bottom,
                Key = "X",
                IsAxisVisible = Properties.Settings.Default.xAxis,
                MajorGridlineThickness = Properties.Settings.Default.lineThickness,
                AbsoluteMinimum = Properties.Settings.Default.minX,
                AbsoluteMaximum = Properties.Settings.Default.maxX,
                TitleFormatString = Properties.Settings.Default.titleFormat,
                LabelFormatter = Oxy.Override.OxyUtils._axisFormatter
            };

            //use my override for title rotation :) works unexpectedly good :D
            LinearAxis y = new LinearAxis
            {
                Title = "Δ_{msp}",
                Unit = "Å",
                Position = AxisPosition.Left,
                Key = "Y",
                IsAxisVisible = true,
                MajorGridlineThickness = Properties.Settings.Default.lineThickness,
                TitleFormatString = Properties.Settings.Default.titleFormat,
                LabelFormatter = Oxy.Override.OxyUtils._axisFormatter
            };

            //handle settings
            if (PorphyStruct.Properties.Settings.Default.rotateTitle)
            {
                y.AxisTitleDistance = 15;
            }

            if (!Properties.Settings.Default.showBox)
            {
                PlotAreaBorderThickness = new OxyThickness(0);
                y.AxislineStyle = LineStyle.Solid;
                y.AxislineThickness = Properties.Settings.Default.lineThickness;
                x.AxislineStyle = LineStyle.Solid;
                x.AxislineThickness = Properties.Settings.Default.lineThickness;
            }

            this.Axes.Add(x);
            this.Axes.Add(y);

            //make it accessible
            xAxis = x;
            yAxis = y;

        }


        public void Scale(Axis a, bool isY = false, bool normalize = false)
        {
            string val = isY ? "Y" : "X";

            double min, max;
            if (!normalize)
            {
                min = double.PositiveInfinity;
                max = double.NegativeInfinity;
                if ((!isY && Properties.Settings.Default.autoscaleX) || (isY && Properties.Settings.Default.autoscaleY))
                {
                    //find min & max automatically
                    //scale x
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
                    min = (isY ? Properties.Settings.Default.minY : Properties.Settings.Default.minX);
                    max = (isY ? Properties.Settings.Default.maxY : Properties.Settings.Default.maxX);
                }
            }
            else
            {
                min = -1.1;
                max = 1.1;
            }

            a.AbsoluteMinimum = min;
            a.AbsoluteMaximum = max;
            a.Zoom(min, max);

            InvalidatePlot(true);
        }
    }
}

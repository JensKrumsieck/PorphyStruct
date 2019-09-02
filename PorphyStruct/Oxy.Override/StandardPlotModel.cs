using OxyPlot;
using OxyPlot.Axes;
using System.Collections.Generic;

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

        /// <summary>
        /// Scale X Axis
        /// </summary>
        /// <param name="data"></param>
        public void ScaleX(List<AtomDataPoint> data)
        {
            //scale X
            double min = 0;
            double max = 0;
            if (Properties.Settings.Default.autoscaleX)
            {
                //find min & max automatically
                foreach (AtomDataPoint dp in data)
                {
                    if (dp.X < min)
                        min = dp.X;
                    if (dp.X > max)
                        max = dp.X;
                }
                min = min - 1;
                max = max + 1;
            }
            else
            {
                //set min & max manually
                min = Properties.Settings.Default.minX;
                max = Properties.Settings.Default.maxX;
            }
            xAxis.AbsoluteMinimum = min;
            xAxis.AbsoluteMaximum = max;
            xAxis.Zoom(min, max);
        }

        /// <summary>
        /// Scale Y Axis
        /// </summary>
        /// <param name="data"></param>
        public void ScaleY(List<AtomDataPoint> data)
        {
            //scale y
            double min = 0;
            double max = 0;
            if (Properties.Settings.Default.autoscaleY)
            {
                //find min & max automatically
                foreach (AtomDataPoint dp in data)
                {
                    if (dp.Y < min)
                        min = dp.Y;
                    if (dp.Y > max)
                        max = dp.Y;
                }
                min = min - 0.05;
                max = max + 0.05;
            }
            else
            {
                //set min & max manually
                min = Properties.Settings.Default.minY;
                max = Properties.Settings.Default.maxY;
            }

            yAxis.Zoom(min, max);
            yAxis.AbsoluteMinimum = min;
            yAxis.AbsoluteMaximum = max;
        }
    }
}

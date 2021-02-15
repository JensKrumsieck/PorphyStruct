using System.Collections.Generic;
using System.Drawing;
using OxyPlot;
using OxyPlot.Axes;

namespace PorphyStruct.Plot
{
    public class DefaultPlotModel : PlotModel
    {
        public LinearAxis XAxis { get; }
        public LinearAxis YAxis { get; }

        public DefaultPlotModel()
        {
            TitleFontWeight = 200;
            DefaultFontSize = Settings.Instance.FontSize;
            DefaultFont = Settings.Instance.Font;
            PlotAreaBorderThickness = new OxyThickness(Settings.Instance.BorderThickness);

            XAxis = new LinearAxis
            {
                Title = "X",
                Unit = "Å",
                Position = AxisPosition.Bottom,
                Key = "X",
                AxislineThickness = Settings.Instance.AxisThickness,
                TitleFormatString = Settings.Instance.AxisFormat,
                IsAxisVisible = Settings.Instance.ShowXAxis
            };

            YAxis = new LinearAxis
            {
                Title = "Δ_{msp}",
                Unit = "Å",
                Position = AxisPosition.Left,
                Key = "Y",
                IsAxisVisible = true,
                AxislineThickness = Settings.Instance.AxisThickness,
                TitleFormatString = Settings.Instance.AxisFormat,
            };

            Axes.Add(XAxis);
            Axes.Add(YAxis);
            Axes.Add(ColorAxis);

            
            if (!PlotAreaBorderThickness.Equals(new OxyThickness(0))) return;
            YAxis.AxislineStyle = LineStyle.Solid;
            XAxis.AxislineStyle = LineStyle.Solid;
        }

        /// <summary>
        /// Adds zero line
        /// </summary>
        public void AddZero()
        {
            //add zero
            if (!Settings.Instance.ShowZero) return;
            //show zero
            var zero = new OxyPlot.Annotations.LineAnnotation()
            {
                Color = OxyColor.FromAColor(40, OxyColors.Gray),
                StrokeThickness = Settings.Instance.SeriesThickness,
                Intercept = 0,
                Slope = 0
            };
            Annotations.Add(zero);
        }

        /// <summary>
        /// Inits Model for Analysis
        /// </summary>
        public void Init()
        {
            Series.Clear();
            Annotations.Clear(); 
            AddZero();
        }

        /// <summary>
        /// Builds RangeColorAxis
        /// </summary>
        /// <returns></returns>
        public static AtomRangeColorAxis ColorAxis => new AtomRangeColorAxis
        {
            Key = "colors",
            Position = AxisPosition.Bottom,
            IsAxisVisible = false
        };
    }
}

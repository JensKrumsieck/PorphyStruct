using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PorphyStruct.Chemistry.Data;
using PorphyStruct.Core.OxyPlot.Custom;
using PorphyStruct.OxyPlotOverride;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PorphyStruct.Chemistry
{
    public static class MacrocyclePainter
    {
        /// <summary>
        /// Paints the Macrocycle object
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="cycle"></param>
        /// <param name="mode"></param>
        public static void Paint(this Macrocycle cycle, PlotModel pm, IAtomDataPointProvider data)
        {
            foreach (AtomDataPoint dp in data.DataPoints) AssignValue(dp, data.DataType);
            //read marker type
            MarkerType mType = MarkerType.Circle;
            if (data.DataType == DataType.Experimental) mType = Core.Properties.Settings.Default.markerType;
            if (data.DataType == DataType.Simulation || data.DataType == DataType.Difference) mType = Core.Properties.Settings.Default.simMarkerType;
            if (data.DataType == DataType.Comparison) mType = Core.Properties.Settings.Default.comMarkerType;

            //build series
            var series = new ScatterSeries()
            {
                MarkerType = mType,
                ItemsSource = data.DataPoints,
                ColorAxisKey = Core.Properties.Settings.Default.singleColor ? null : "colors",
                Title = $"{(data.DataType == DataType.Comparison ? $"{((CompareData)data).FileName} " : "")}{data.DataType}",
                MarkerSize = Core.Properties.Settings.Default.markerSize
            };
            //add series
            if (!Core.Properties.Settings.Default.singleColor) pm.Axes.Add(ColorAxis(data.DataPoints));
            else series.MarkerFill = SingleColor(cycle.DataProviders.IndexOf(data), cycle.DataProviders.Count);
            pm.Series.Add(series);

            //draw bonds			
            foreach (OxyPlot.Annotations.ArrowAnnotation a in cycle.DrawBonds(data)) pm.Annotations.Add(a);
        }

        /// <summary>
        /// Assigns Value to an AtomDataPoint
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        private static void AssignValue(AtomDataPoint dp, DataType mode) => dp.Value = dp.atom.Type == "C" ? 1000d * (int)mode : dp.X + (1000 * ((int)mode));

        /// <summary>
        /// Builds RangeColorAxis
        /// </summary>
        /// <returns></returns>
        public static AtomRangeColorAxis ColorAxis(IEnumerable<AtomDataPoint> dataPoints) => new AtomRangeColorAxis(dataPoints)
        {
            Key = "colors",
            Position = AxisPosition.Bottom,
            IsAxisVisible = false
        };

        /// <summary>
        /// Get Palette Colors
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static OxyColor SingleColor(int index, int count)
        {
            int min = 7;
            if (Core.Properties.Settings.Default.ColorPalette.Contains("GrayScale")) min = 1;
            OxyPalette palette = CurrentPalette(count > min ? count : min);
            return palette.Colors[index >= 0 ? index : 0];
        }

        /// <summary>
        /// returns palette based on settings and reflection
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static OxyPalette CurrentPalette(int count)
        {

            string title = Core.Properties.Settings.Default.ColorPalette;
            IEnumerable<MethodInfo> palette = from MethodInfo method in typeof(CustomPalettes).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                              where method.Name == title
                                              select method;
            return palette.FirstOrDefault().Invoke(null, new object[] { count }) as OxyPalette;

        }

    }
}

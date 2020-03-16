using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PorphyStruct.Chemistry.Data;
using PorphyStruct.OxyPlotOverride;
using System.Collections.Generic;

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
            foreach (var dp in data.DataPoints) AssignValue(dp, data.DataType);
            //read marker type
            MarkerType mType = MarkerType.Circle;
            if (data.DataType == DataType.Experimental) mType = Core.Properties.Settings.Default.markerType;
            if (data.DataType == DataType.Simulation || data.DataType == DataType.Difference) mType = Core.Properties.Settings.Default.simMarkerType;
            if (data.DataType == DataType.Comparison) mType = Core.Properties.Settings.Default.comMarkerType;

            //build series
            ScatterSeries series = new ScatterSeries()
            {
                MarkerType = mType,
                ItemsSource = data.DataPoints,
                ColorAxisKey = Core.Properties.Settings.Default.singleColor ? null : "colors",
                Title = data.DataType.ToString()
            };
            //add series
            if (!Core.Properties.Settings.Default.singleColor) pm.Axes.Add(ColorAxis(data.DataPoints));
            else series.MarkerFill = Atom.modesSingleColor[(int)data.DataType];
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

    }
}

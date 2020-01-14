using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PorphyStruct.OxyPlotOverride;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry
{
    public static class MacrocyclePainter
    {
        /// <summary>
        /// Paintmode Enum
        /// </summary>
        public enum PaintMode { Exp, Sim, Diff, Com1, Com2 };

        /// <summary>
        /// Paints the Macrocycle object
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="cycle"></param>
        /// <param name="mode"></param>
        public static void Paint(this Macrocycle cycle, PlotModel pm, PaintMode mode)
        {
            foreach (var dp in cycle.dataPoints) AssignValue(dp, mode);
            //read marker type
            MarkerType mType = MarkerType.Circle;
            if (mode == PaintMode.Exp) mType = PorphyStruct.Core.Settings.Default.markerType;
            if (mode == PaintMode.Sim || mode == PaintMode.Diff) mType = PorphyStruct.Core.Settings.Default.simMarkerType;
            if (mode == PaintMode.Com1) mType = PorphyStruct.Core.Settings.Default.com1MarkerType;
            if (mode == PaintMode.Com2) mType = PorphyStruct.Core.Settings.Default.com2MarkerType;

            //build series
            ScatterSeries series = new ScatterSeries()
            {
                MarkerType = mType,
                ItemsSource = cycle.dataPoints,
                ColorAxisKey = PorphyStruct.Core.Settings.Default.singleColor ? null : "colors",
                Title = mode.ToString()
            };
            if (PorphyStruct.Core.Settings.Default.singleColor)
                series.MarkerFill = Atom.modesSingleColor[(int)mode];
            //add series

            if(!PorphyStruct.Core.Settings.Default.singleColor)  pm.Axes.Add(ColorAxis(cycle.dataPoints));
            else series.MarkerFill = Atom.modesSingleColor[(int) mode];

            pm.Series.Add(series);

            //draw bonds			
            foreach (OxyPlot.Annotations.ArrowAnnotation a in cycle.DrawBonds((int)mode)) pm.Annotations.Add(a);
        }

        /// <summary>
        /// Assigns Value to an AtomDataPoint
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        private static void AssignValue(AtomDataPoint dp, PaintMode mode) => dp.Value = dp.atom.Type == "C" ? 1000d * (int)mode : dp.X + (1000 * ((int)mode));

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

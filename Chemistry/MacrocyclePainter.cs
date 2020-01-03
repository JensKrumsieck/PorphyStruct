using OxyPlot;
using OxyPlot.Series;

namespace PorphyStruct.Chemistry
{
    public static class MacrocyclePainter
    {
        /// <summary>
        /// Paintmode Enum
        /// </summary>
        public enum PaintMode { Exp, Sim, Diff, Com1, Com2 };

        public static void Paint(PlotModel pm, Macrocycle cycle, PaintMode mode)
        {
            foreach (var dp in cycle.dataPoints) AssignValue(dp, mode);
            //read marker type
            MarkerType mType = MarkerType.Circle;
            if (mode == PaintMode.Exp) mType = Properties.Settings.Default.markerType;
            if (mode == PaintMode.Sim || mode == PaintMode.Diff) mType = Properties.Settings.Default.simMarkerType;
            if (mode == PaintMode.Com1) mType = Properties.Settings.Default.com1MarkerType;
            if (mode == PaintMode.Com2) mType = Properties.Settings.Default.com2MarkerType;

            //build series
            ScatterSeries series = new ScatterSeries()
            {
                MarkerType = mType,
                ItemsSource = cycle.dataPoints,
                ColorAxisKey = Properties.Settings.Default.singleColor ? null : "colors",
                Title = mode.ToString()
            };
            if (Properties.Settings.Default.singleColor)
                series.MarkerFill = Atom.modesSingleColor[(int)mode];
            //add series
            pm.Series.Add(series);

            //draw bonds			
            foreach (OxyPlot.Annotations.ArrowAnnotation a in cycle.DrawBonds((int)mode)) pm.Annotations.Add(a);
        }

        /// <summary>
        /// Assigns Value to an AtomDataPoint
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        private static void AssignValue(AtomDataPoint dp, PaintMode mode) => dp.Value = dp.atom.Type == "C" ? 1000d * (int)mode : dp.Value + (1000 * ((int)mode+1));
    }
}

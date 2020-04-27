using OxyPlot;
using OxyPlot.Axes;
using PorphyStruct.Chemistry;
using System.Collections.Generic;

namespace PorphyStruct.OxyPlotOverride
{
    public class AtomRangeColorAxis : RangeColorAxis
    {
        private const double min = 0.25;
        private const double max = 0.25;

        public AtomRangeColorAxis(IEnumerable<AtomDataPoint> datapoints) : base()
        {
            foreach (AtomDataPoint dp in datapoints) AddAtom(dp);
        }

        /// <summary>
        /// Add Atom to DataPointList
        /// </summary>
        /// <param name="dp"></param>
        public void AddAtom(AtomDataPoint dp)
        {
            for (int i = 0; i <= 4; i++) AddAtomRange(dp, dp.atom.Type == "C" ? 0 : dp.X, i);
        }

        /// <summary>
        /// Add Range for specfic Atom and mode
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        /// <param name="mode"></param>
        private void AddAtomRange(AtomDataPoint dp, double value, int mode)
        {
            byte alpha = Atom.GetAlpha(mode);
            AddRange(
                (value + (1000 * mode)) - min,
                (value + (1000 * mode)) + max,
                OxyColor.FromAColor(alpha, dp.atom.OxyColor));
        }
    }
}

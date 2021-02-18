using ChemSharp.Molecules.DataProviders;
using OxyPlot;
using OxyPlot.Axes;
using Element = ChemSharp.Molecules.Element;

namespace PorphyStruct.Core.Plot
{
    public class AtomRangeColorAxis : RangeColorAxis
    {
        private const double min = 0.25;
        private const double max = 0.25;

        public AtomRangeColorAxis()
        {
            foreach (var e in ElementDataProvider.ElementData) AddAtomRange(e);
        }

        /// <summary>
        /// Add Range for specfic Atom and mode
        /// </summary>
        /// <param name="e"></param>
        private void AddAtomRange(Element e)
        {
            AddRange(
                e.AtomicNumber - min,
                e.AtomicNumber + max,
                OxyColor.Parse(e.Color));
        }
    }
}

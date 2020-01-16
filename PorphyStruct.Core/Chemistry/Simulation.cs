using OxyPlot;
using PorphyStruct.Simulations;
using PorphyStruct.Util;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry
{
    public class Simulation
    {
        public bool isNormalized = true;
        public bool isInverted = false;
        public Dictionary<string, double> par = new Dictionary<string, double>();
        public double[] errors;
        public List<SimParam> simParam;
        public Macrocycle cycle;

        /// <summary>
        /// Constructor assigns cycle object
        /// </summary>
        /// <param name="cycle"></param>
        public Simulation(Macrocycle cycle) => this.cycle = cycle;

        /// <summary>
        /// Paints the Simulation to a PlotModel
        /// </summary>
        /// <param name="pm">PlotModel</param>
        public void Paint(PlotModel pm, string title = "Sim")
        {
            //set mode
            int mode = 0;
            if (title == "Sim")
                mode = 1;
            if (title == "Diff")
                mode = 2;
            if (title == "Com1")
                mode = 3;
            if (title == "Com2")
                mode = 4;

            cycle.Paint(pm, (MacrocyclePainter.PaintMode)mode);
        }

        /// <summary>
        /// Normalizes Simulation
        /// </summary>
        public void Normalize(bool normalize, double factor = 0d)
        {
            if (!isNormalized && normalize)
            {
                cycle.dataPoints = cycle.dataPoints.Normalize();
                isNormalized = true;
            }
            else if (!normalize && isNormalized)
            {
                cycle.dataPoints = cycle.dataPoints.Factor(1 / factor).ToList();
                isNormalized = false;
            }
        }

        /// <summary>
        /// inverts simulation
        /// </summary>
        public void Invert(bool invert)
        {
            if (!isInverted && invert)
            {
                cycle.dataPoints = cycle.dataPoints.Invert();
                isInverted = true;
            }
            else if (isInverted && !invert)
            {
                cycle.dataPoints = cycle.dataPoints.Invert();
                isInverted = false;
            }
        }
    }
}

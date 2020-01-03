using OxyPlot;
using PorphyStruct.Simulations;
using System.Collections.Generic;

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
        public void Paint(PlotModel pm, string title = "Sim.")
        {
            //set mode
            int mode = 0;
            if (title == "Sim.")
                mode = 1;
            if (title == "Diff.")
                mode = 2;
            if (title == "Com.1")
                mode = 3;
            if (title == "Com.2")
                mode = 4;

            MacrocyclePainter.Paint(pm, cycle, (MacrocyclePainter.PaintMode)mode);
        }
    }
}

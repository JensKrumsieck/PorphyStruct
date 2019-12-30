using OxyPlot;
using OxyPlot.Series;
using PorphyStruct.Simulations;
using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry
{
    public class Simulation : Macrocycle
    {
        public bool isNormalized = true;
        public bool isInverted = false;
        public Dictionary<string, double> par = new Dictionary<string, double>();
        public double[] errors;
        public List<SimParam> simParam;
        public Simulation(List<Atom> atoms) : base(atoms) { }

        /// <summary>
        /// Get Properties from Cycle Class
        /// </summary>
        public override string[] AlphaAtoms => RingProperty("_AlphaAtoms") as string[];
        public override List<Tuple<string, string>> Bonds => RingProperty("_Bonds") as List<Tuple<string, string>>;
        public override List<string> RingAtoms => RingProperty("_RingAtoms") as List<string>;
        public override List<string[]> Dihedrals => RingProperty("_Dihedrals") as List<string[]>;
        public override Dictionary<string, double> Multiplier => RingProperty("_Multiplier") as Dictionary<string, double>;

        /// <summary>
        /// Gets static Ring Properties
        /// </summary>
        /// <param name="Property"></param>
        /// <returns></returns>
        internal object RingProperty(string Property) => System.Type.GetType($"PorphyStruct.Chemistry.Macrocycles.{type.ToString()}").GetProperty(Property).GetValue(this);

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

            //change dp values for a little workaround with colors.
            foreach (AtomDataPoint dp in dataPoints)
            {
                if (dp.atom.Type == "C")
                    dp.Value = (1000 * mode);
                else if (dp.Value < (1000 * mode))
                    dp.Value += (1000 * mode);
            }
            MarkerType mtype = new MarkerType();
            if (mode == 1)
                mtype = Properties.Settings.Default.simMarkerType;
            if (mode == 2)
                mtype = Properties.Settings.Default.simMarkerType;
            if (mode == 3)
                mtype = Properties.Settings.Default.com1MarkerType;
            if (mode == 4)
                mtype = Properties.Settings.Default.com2MarkerType;

            ScatterSeries sim = new ScatterSeries()
            {
                MarkerType = mtype,
                ItemsSource = dataPoints,
                ColorAxisKey = Properties.Settings.Default.singleColor ? null : "colors",
                Title = title
            };
            if (Properties.Settings.Default.singleColor)
                sim.MarkerFill = Atom.modesSingleColor[mode];
            pm.Series.Add(sim);


            //draw sim bonds			
            foreach (OxyPlot.Annotations.ArrowAnnotation a in this.DrawBonds(mode))
            {
                pm.Annotations.Add(a);
            }
        }
    }
}

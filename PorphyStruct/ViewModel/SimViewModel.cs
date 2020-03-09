using OxyPlot;
using PorphyStruct.Chemistry;
using PorphyStruct.OxyPlotOverride;
using PorphyStruct.Simulations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PorphyStruct.ViewModel
{
    public class SimViewModel : AbstractViewModel
    {
        public SimViewModel(Macrocycle Cycle)
        {
            this.Model = new StandardPlotModel();
            this.Cycle = Cycle;

            //drop metal data
            if (Cycle.HasMetal()) this.Cycle.dataPoints = this.Cycle.dataPoints.Where(s => !s.atom.IsMetal).ToList();
            Cycle.Paint(Model, MacrocyclePainter.PaintMode.Exp);
            Parameters = SimParam.ListParameters(Cycle.type);
        }

        /// <summary>
        /// PlotModel
        /// </summary>
        public StandardPlotModel Model { get => Get<StandardPlotModel>(); set => Set(value); }
        
        /// <summary>
        /// Macrocycle
        /// </summary>
        public Macrocycle Cycle { get => Get<Macrocycle>(); set => Set(value); }

        /// <summary>
        /// Simulation Parameters
        /// </summary>
        public List<SimParam> Parameters { get => Get<List<SimParam>>(); set => Set(value); }
    }
}

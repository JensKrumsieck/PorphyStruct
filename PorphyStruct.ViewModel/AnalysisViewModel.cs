using MathNet.Numerics.LinearAlgebra.Double;
using OxyPlot.Annotations;
using PorphyStruct.Analysis;
using PorphyStruct.Extension;
using PorphyStruct.Plot;
using System.Linq;
using TinyMVVM;
using ChemSharp.Mathematics;

namespace PorphyStruct.ViewModel
{
    public class AnalysisViewModel : ListItemViewModel<MacrocycleViewModel, AnalysisViewModel>
    {
        public MacrocycleAnalysis Analysis { get; set; }

        public DefaultPlotModel Model { get; set; }

        public override string Title => Parent.Title;

        public AnalysisViewModel(MacrocycleViewModel parent) : base(parent)
        {
            Model = new DefaultPlotModel();
        }

        public void Analyze()
        {
            Model.Init();
            var points = Analysis.CalculateDataPoints();
            foreach (var (a1, a2) in Analysis.BondDataPoints()) 
                Model.Annotations.Add(new BondAnnotation(a1,a2));
            Model.Series.Add(new DefaultScatterSeries { ItemsSource = points });
            Model.InvalidatePlot(true);
        }
        public void Simulate()
        {
            var sim = new Simulation(Parent.Macrocycle.MacrocycleType);
            var data = Analysis.DataPoints.OrderBy(d => d.X).Select(s => s.Y).ToArray();
            var res = sim.Simulate(data);
            var conf = sim.ReferenceMatrix * DenseVector.OfArray(res);
            var doop = conf.AsArray().Length();
            var expDoop = Analysis.DataPoints.DisplacementValue();
            //TODO:Save data somehow
        }
    }
}

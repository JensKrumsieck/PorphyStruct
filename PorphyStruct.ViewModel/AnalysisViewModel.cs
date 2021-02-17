using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;
using OxyPlot.Annotations;
using OxyPlot.Series;
using PorphyStruct.Analysis;
using PorphyStruct.Plot;
using TinyMVVM;

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
            {
                Model.Annotations.Add(new ArrowAnnotation { StartPoint = a1.ToDataPoint(), EndPoint = a2.ToDataPoint(), HeadLength = 0 });
            }
            Model.Series.Add(new ScatterSeries { ItemsSource = points, TrackerFormatString = AtomDataPoint.TrackerFormatString, ColorAxisKey = "colors", MarkerType = Settings.Instance.MarkerType });
            Model.InvalidatePlot(true);
        }
        public void Simulate()
        {
            var sim = new Simulation(Parent.Macrocycle.MacrocycleType);
            var data = Analysis.DataPoints.Select(s => s.Y).ToArray();
            var res = sim.Simulate(data);
        }
    }
}

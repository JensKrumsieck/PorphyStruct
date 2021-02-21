using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Plot;
using System.Collections.ObjectModel;
using System.Linq;
using TinyMVVM;
using TinyMVVM.Utility;

namespace PorphyStruct.ViewModel
{
    public class AnalysisViewModel : ListItemViewModel<MacrocycleViewModel, AnalysisViewModel>
    {
        public MacrocycleAnalysis Analysis { get; }

        public DefaultPlotModel Model { get; }

        public DefaultScatterSeries ExperimentalSeries { get; } = new DefaultScatterSeries();

        public DefaultScatterSeries SimulationSeries { get; } = new DefaultScatterSeries();

        public ObservableCollection<BondAnnotation> ExperimentalBonds { get; } = new ObservableCollection<BondAnnotation>();

        public ObservableCollection<BondAnnotation> SimulationBonds { get; } = new ObservableCollection<BondAnnotation>();

        private bool _simulationVisible;
        /// <summary>
        /// Gets or sets visibility for simulation series
        /// </summary>
        public bool SimulationVisible
        {
            get => _simulationVisible;
            set => Set(ref _simulationVisible, value, SimulationChanged);
        }

        public override string Title => Parent.Title;

        public AnalysisViewModel(MacrocycleViewModel parent, MacrocycleAnalysis analysis) : base(parent)
        {
            //Init Object
            Analysis = analysis;
            Model = new DefaultPlotModel();
            Model.Init();
            Model.Series.Add(ExperimentalSeries);
            Model.Series.Add(SimulationSeries);

            ExperimentalSeries.Title = $"Experimental Data of {Parent.Title}";
            SimulationSeries.Title = $"Simulation of {Parent.Title}";

            //Add Subscriptions
            Subscribe(ExperimentalBonds, Model.Annotations, b => b, a => a, () => Model.InvalidatePlot(true));
            Subscribe(SimulationBonds, Model.Annotations, b => b, a => a, () => Model.InvalidatePlot(true));

            //Add Experimental Data
            ExperimentalSeries.ItemsSource = Analysis.DataPoints;
            foreach (var (a1, a2) in Analysis.BondDataPoints())
                ExperimentalBonds.Add(new BondAnnotation(a1, a2));
        }

        public void SimulationChanged()
        {
            SimulationBonds.ClearAndNotify();
            SimulationSeries.ItemsSource = null;
            Model.InvalidatePlot(true);

            if (!SimulationVisible) return;
            var simY = Analysis.Properties.Simulation.ConformationY;
            var cache = Analysis.DataPoints.OrderBy(s => s.X).ToList();
            var src = cache.Select((t, i) => new AtomDataPoint(t.X, simY[i], t.Atom)).ToList();
            SimulationSeries.ItemsSource = src;

            // ReSharper disable  CompareOfFloatsByEqualityOperator
            foreach (var (a1, a2) in Analysis.BondDataPoints())
                SimulationBonds.Add(new BondAnnotation(src.FirstOrDefault(s => s.X == a1.X), src.FirstOrDefault(s => s.X == a2.X)) { Opacity = 128 });
            // ReSharper restore  CompareOfFloatsByEqualityOperator
        }
    }
}

using System.Collections.Generic;
using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Plot;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
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

        private bool _inverted;
        /// <summary>
        /// Indicates whether the series are inverted
        /// </summary>
        public bool Inverted
        {
            get => _inverted;
            set => Set(ref _inverted, value, () =>
            {
                ExperimentalSeries.Inverted = value;
                SimulationSeries.Inverted = value;
                Model.InvalidatePlot(true);
            });
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
            Subscribe(ExperimentalBonds, Model.Annotations, b => b, a => a, () => Model.InvalidatePlot(false));
            Subscribe(SimulationBonds, Model.Annotations, b => b, a => a, () => Model.InvalidatePlot(false));

            //Add Experimental Data
            ExperimentalSeries.ItemsSource = Analysis.DataPoints;
            foreach (var (a1, a2) in Analysis.BondDataPoints())
                ExperimentalBonds.Add(new BondAnnotation(a1, a2, ExperimentalSeries));
        }

        public void SimulationChanged()
        {
            SimulationBonds.ClearAndNotify();
            SimulationSeries.ItemsSource = null;
            Model.InvalidatePlot(false);

            if (!SimulationVisible) return;
            var simY = Analysis.Properties.Simulation.ConformationY;
            var src = BuildSimulationData(simY);
            SimulationSeries.ItemsSource = src;

            // ReSharper disable  CompareOfFloatsByEqualityOperator
            foreach (var (a1, a2) in Analysis.BondDataPoints())
                SimulationBonds.Add(new BondAnnotation(src.FirstOrDefault(s => s.X == a1.X), src.FirstOrDefault(s => s.X == a2.X), SimulationSeries) { Opacity = 128 });
            // ReSharper restore  CompareOfFloatsByEqualityOperator
            Model.InvalidatePlot(true);
        }

        /// <summary>
        /// Calculates Conformation
        /// </summary>
        /// <returns></returns>
        private List<AtomDataPoint> BuildSimulationData(IList<double> yAxis)
        {
            var cache = Analysis.DataPoints.OrderBy(s => s.X).ToList();
            return cache.Select((t, i) => new AtomDataPoint(t.X, yAxis[i], t.Atom)).ToList();
        }
    }
}

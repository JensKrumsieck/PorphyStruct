﻿using System.Collections.ObjectModel;
using OxyPlot;
using PorphyStruct.Core;
using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Plot;
using PorphyStruct.ViewModel.IO;
using TinyMVVM;
using TinyMVVM.Utility;

namespace PorphyStruct.ViewModel;

public class AnalysisViewModel : ListItemViewModel<MacrocycleViewModel, AnalysisViewModel>
{
    public MacrocycleAnalysis Analysis { get; }

    public DefaultPlotModel Model { get; }
    
    public PlotController Controller { get; set; }

    public DefaultScatterSeries ExperimentalSeries { get; } = new();

    public DefaultScatterSeries SimulationSeries { get; } = new();

    public ObservableCollection<BondAnnotation> ExperimentalBonds { get; } = new();

    public ObservableCollection<BondAnnotation> SimulationBonds { get; } = new();

    public ObservableCollection<CompareData> CompareData { get; } = new();

    public List<DefaultScatterSeries> ComparisonSeries { get; } = new();

    public ObservableCollection<BondAnnotation> ComparisonBonds { get; } = new();

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
            Analysis.InvertYDataPoints();
            DataChanged(true);
        });
    }
    
    private bool _invertedX;
    /// <summary>
    /// Indicates whether the series are x flipped
    /// </summary>
    public bool InvertedX
    {
        get => _invertedX;
        set => Set(ref _invertedX, value, () =>
        {
            Analysis.InvertXDataPoints();
            DataChanged(true);
        });
    }

    public void Rotate()
    {
        switch (Analysis.Type)
        {
            case MacrocycleType.Porphyrin:
                Analysis.RotateDataPoints(5, true);
                break;
            case MacrocycleType.Norcorrole:
            case MacrocycleType.Porphycene:
                Analysis.RotateDataPoints(10, true);
                break;
            case MacrocycleType.Corrole:
            case MacrocycleType.Corrphycene:
            default:
                break;
        }
        DataChanged();
    }

    private void Invalidate()
    {
        HandleZoomY();
        Model.InvalidatePlot(true);
    }

    public override string Title => Parent.Title;

    public AnalysisViewModel(MacrocycleViewModel parent, MacrocycleAnalysis analysis) : base(parent)
    {
        //Init Object
        Analysis = analysis;
        Model = new DefaultPlotModel();
        Controller = new PlotController();
        Controller.BindMouseEnter(PlotCommands.HoverSnapTrack);
        var handleSelect = new DelegatePlotCommand<OxyMouseDownEventArgs>((v, c, e) =>
        {
            var args = new HitTestArguments(e.Position, 10);
            var result = ExperimentalSeries.HitTest(args);
            if (result?.Item is not AtomDataPoint point) return;
            Parent.SelectedAtom = point.Atom;
        });
        Controller.Bind(new OxyMouseDownGesture(OxyMouseButton.Left), handleSelect);
        Model.Init();
        Model.Series.Add(ExperimentalSeries);
        Model.Series.Add(SimulationSeries);

        ExperimentalSeries.Title = $"Experimental Data of {Parent.Title}";
        SimulationSeries.Title = $"Simulation of {Parent.Title}";
        SimulationSeries.Opacity = (byte)Math.Round(255u * Settings.Instance.SimulationOpacity);

        //Add Subscriptions
        Subscribe(ExperimentalBonds, Model.Annotations, b => b, a => a, () => Model.InvalidatePlot(false));
        Subscribe(SimulationBonds, Model.Annotations, b => b, a => a, () => Model.InvalidatePlot(false));
        Subscribe(ComparisonBonds, Model.Annotations, b => b, a => a, () => Model.InvalidatePlot(false));

        //Add Experimental Data
        ExperimentalSeries.ItemsSource = Analysis.DataPoints;
        foreach (var (a1, a2) in Analysis.BondDataPoints())
            ExperimentalBonds.Add(new BondAnnotation(a1, a2, ExperimentalSeries));
        //zoom
        Model.XAxis.BindableActualMinimum = .5;
        Model.XAxis.BindableActualMaximum = Analysis.DataPoints.Max(s => s.X) + .5;
        Invalidate();
    }

    private void HandleZoomY()
    {
        var yMax = Analysis.DataPoints.Max(s => s.Y) + .05;
        var yMin = Analysis.DataPoints.Min(s => s.Y) - .05;
        if (yMax < .5) yMax = .5;
        if (yMin > -.5) yMin = -.5;
        Model.YAxis.BindableActualMinimum = yMin;
        Model.YAxis.BindableActualMaximum = yMax;
    }

    public void SimulationChanged()
    {
        SimulationBonds.ClearAndNotify();
        SimulationSeries.ItemsSource = null;
        Model.InvalidatePlot(false);

        if (!SimulationVisible) return;
        var simY = Analysis.Properties?.Simulation?.ConformationY;
        if (simY == null) return;
        var src = BuildSimulationData(simY);
        SimulationSeries.ItemsSource = src;
        if (Settings.Instance.SingleColor) SimulationSeries.MarkerFill = OxyColor.Parse(Settings.Instance.SimulationMarkerColor);

        foreach (var (a1, a2) in Analysis.BondDataPoints())
            SimulationBonds.Add(SimulationSeries.CreateBondAnnotation(a1, a2));
        
        Invalidate();
    }

    public void DataChanged(bool resetData = false)
    {
        if (resetData) 
            ExperimentalSeries.ItemsSource = Analysis.DataPoints;
        ExperimentalBonds.ClearAndNotify();
        foreach (var (a1, a2) in Analysis.BondDataPoints())
            ExperimentalBonds.Add(new BondAnnotation(a1, a2, ExperimentalSeries));
        OnPropertyChanged(nameof(Analysis));
        if(SimulationVisible) SimulationChanged();
        Invalidate();
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

    public void PrepareCompare()
    {
        if (CompareData.Count == 0)
        {
            foreach (var comp in ComparisonSeries) Model.Series.Remove(comp);
            ComparisonBonds.ClearAndNotify();
        }
        foreach (var data in CompareData)
        {
            var series = new DefaultScatterSeries { ItemsSource = data.DataPoints, Title = $"Comparison {data.Title}" };
            if (Settings.Instance.SingleColor && Settings.Instance.ComparisonColorPalette.Any())
                series.MarkerFill =
                    OxyColor.Parse(Settings.Instance.ComparisonColorPalette[
                        CompareData.IndexOf(data) % Settings.Instance.ComparisonColorPalette.Count]);
            ComparisonSeries.Add(series);
            Model.Series.Add(series);
            foreach (var (a1, a2) in Analysis.BondDataPoints())
            {
                var bond = series.CreateBondAnnotation(a1, a2);
                if (Settings.Instance.ComparisonColorPalette.Any()) bond.Color = OxyColor.Parse(Settings.Instance.ComparisonColorPalette[
                      CompareData.IndexOf(data) % Settings.Instance.ComparisonColorPalette.Count]);
                ComparisonBonds.Add(bond);
            }
        }
        
        Invalidate();
    }
}

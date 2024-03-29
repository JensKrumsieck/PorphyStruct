﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using ChemSharp.Molecules;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Core;
using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Analysis.Properties;
using PorphyStruct.Core.Extension;
using PorphyStruct.Core.Plot;
using TinyMVVM;
using TinyMVVM.Utility;

namespace PorphyStruct.ViewModel;

public class CalculatorViewModel : BaseViewModel
{
    public DefaultPlotModel Model { get; }

    public DefaultScatterSeries Series { get; } = new DefaultScatterSeries();

    public ObservableCollection<BondAnnotation> Bonds { get; } = new ObservableCollection<BondAnnotation>();

    public ObservableCollection<Mode> ModeVector { get; } = new ObservableCollection<Mode>();

    private MacrocycleType _cycleType = MacrocycleType.Porphyrin;

    public MacrocycleType CycleType
    {
        get => _cycleType;
        set
        {
            _cycleType = value;
            BuildModeVector();
            Task.Run(Recalculate);
        }
    }

    public CalculatorViewModel()
    {
        Model = new DefaultPlotModel();
        Model.Init();
        Model.Series.Add(Series);
        Series.Title = "Calculation";

        //Add Subscriptions
        Subscribe(Bonds, Model.Annotations, b => b, a => a, () => Model.InvalidatePlot(false));

        //Build Vector
        BuildModeVector();
        Recalculate();
    }


    private async void MOnPropertyChanged(object? sender, PropertyChangedEventArgs e) =>
        await Task.Run(Recalculate);

    public void Recalculate()
    {
        Bonds.ClearAndNotify();
        Series.ItemsSource = null;
        var typePrefix = $"PorphyStruct.Core.Reference.{CycleType}.";
        var matrix = Simulation.DisplacementMatrix(ModeVector.Select(s => typePrefix + s.Key + ".mol2"), CycleType);
        var result = (matrix * DenseVector.OfEnumerable(ModeVector.Select(s => s.Value))).ToList();

        var dom = LoadSample();

        Series.ItemsSource = dom.DataPoints.OrderBy(d => d.X)
            .Select((s, i) => new AtomDataPoint(s.X, result[i], s.Atom));
        foreach (var (a1, a2) in dom.BondDataPoints())
            Bonds.Add(Series.CreateBondAnnotation(a1, a2));
        var data = ((IEnumerable<AtomDataPoint>)Series.ItemsSource).ToList();
        var doop = data.DisplacementValue();
        var mean = data.Sum(s => Math.Abs(s.Y)) / result.Count;
        Model.Title = "D_{oop} = " + doop.ToString("N3") + " Å — Mean Disp. = " + mean.ToString("N3") + " Å";
        Model.InvalidatePlot(true);
    }

    public MacrocycleAnalysis LoadSample()
    {
        var stream = ResourceUtil.LoadResource($"PorphyStruct.Core.Reference.{CycleType}.Doming.mol2");
        var cycle = new Macrocycle(stream!, "mol2");
        
        var mapping = new Dictionary<string, Atom>();
        for (var i = 0; i < cycle.Atoms.Count; i++) 
            mapping.Add(cycle.Atoms[i].Title, cycle.Atoms[i]);
        var analysis =
            MacrocycleAnalysis.Create(new Molecule(cycle.Atoms.Where(a => a.CanBeRingMember() && a.Symbol != "H")),
                mapping, CycleType);
        return analysis;
    }

    private void BuildModeVector()
    {
        //Remove Events
        foreach (var m in ModeVector ?? Enumerable.Empty<Mode>()) m.PropertyChanged -= MOnPropertyChanged;

        if (ModeVector == null) return;
        ModeVector.ClearAndNotify();
        ModeVector.Add(new Mode("Doming", 1d));
        ModeVector.Add(new Mode("Saddling", 0));
        ModeVector.Add(new Mode("Ruffling", 0));
        ModeVector.Add(new Mode("WavingX", 0));
        ModeVector.Add(new Mode("WavingY", 0));
        ModeVector.Add(new Mode("Propellering", 0));

        foreach (var m in ModeVector.ToList().Where(m => m.Key != "WavingY" || (CycleType != MacrocycleType.Porphyrin && CycleType != MacrocycleType.Norcorrole)))
        {
            ModeVector.Add(new Mode(m.Key + "2", 0));
        }

        //Add Events
        foreach (var m in ModeVector) m.PropertyChanged += MOnPropertyChanged;
    }
}

public class Mode : BindableBase
{
    private string _key;
    private double _value;

    public string Key
    {
        get => _key;
        set => Set(ref _key, value);
    }

    public double Value
    {
        get => _value;
        set => Set(ref _value, value);
    }

    public Mode(string key, double value)
    {
        Key = key;
        Value = value;
    }
}


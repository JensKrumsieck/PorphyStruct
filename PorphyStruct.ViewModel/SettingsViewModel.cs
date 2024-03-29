﻿using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using OxyPlot;
using PorphyStruct.Core;
using TinyMVVM;
using TinyMVVM.Command;

namespace PorphyStruct.ViewModel;

public class SettingsViewModel : BaseViewModel
{
    #region Font
    public string Font { get => Get<string>(); set => Set(value); }
    public int FontSize { get => Get<int>(); set => Set(value); }
    public int FontWeight { get => Get<int>(); set => Set(value); }
    #endregion

    #region PlotArea
    public double BorderThickness { get => Get<double>(); set => Set(value); }
    public double Padding { get => Get<double>(); set => Set(value); }
    #endregion

    #region Axis
    public string AxisFormat { get => Get<string>(); set => Set(value); }
    public double AxisThickness { get => Get<double>(); set => Set(value); }
    public double LabelAngle { get => Get<double>(); set => Set(value); }
    public double LabelPadding { get => Get<double>(); set => Set(value); }
    public double LabelPosition { get => Get<double>(); set => Set(value); }
    public bool ShowXAxis { get => Get<bool>(); set => Set(value); }
    public bool ShowZero { get => Get<bool>(); set => Set(value); }
    #endregion

    #region Series
    public double BondThickness { get => Get<double>(); set => Set(value); }
    public string BondColor { get => Get<string>(); set => Set(value); }
    public MarkerType MarkerType { get => Get<MarkerType>(); set => Set(value); }
    public string MarkerColor { get => Get<string>(); set => Set(value); }
    public int MarkerSize { get => Get<int>(); set => Set(value); }
    public bool UseAtomRadiusMarkerSize { get => Get<bool>(); set => Set(value); }
    public int MarkerBorderThickness { get => Get<int>(); set => Set(value); }
    public string MarkerBorderColor { get => Get<string>(); set => Set(value); }
    public string NotMarkedPoints { get => Get<string>(); set => Set(value); }
    public bool SingleColor { get => Get<bool>(); set => Set(value); }
    public double NonValidOpacity { get => Get<double>(); set => Set(value); }
    #endregion

    #region Export
    public float ExportWidth { get => Get<float>(); set => Set(value); }
    public float ExportHeight { get => Get<float>(); set => Set(value); }
    public float ExportDPI { get => Get<float>(); set => Set(value); }
    public string DefaultExportPath { get => Get<string>(); set => Set(value); }
    public string DefaultImportPath { get => Get<string>(); set => Set(value); }
    #endregion

    #region Simulation/Comparions
    public string SimulationBondColor { get => Get<string>(); set => Set(value); }
    public string SimulationMarkerColor { get => Get<string>(); set => Set(value); }
    public MarkerType SimulationMarkerType { get => Get<MarkerType>(); set => Set(value); }
    public MarkerType ComparisonMarkerType { get => Get<MarkerType>(); set => Set(value); }
    public List<string> ComparisonColorPalette { get => Get<List<string>>(); set => Set(value); }
    #endregion

    #region Data
    public bool HandlePhosphorusMetal { get => Get<bool>(); set => Set(value); }
    public bool HandleBoronMetal { get => Get<bool>(); set => Set(value); }
    public bool HandleSiliconMetal { get => Get<bool>(); set => Set(value); }
    public double SimulationOpacity { get => Get<double>(); set => Set(value); }
    public bool UseExtendedBasis { get => Get<bool>(); set => Set(value); }
    #endregion

    public bool AutoUpdate { get => Get<bool>(); set => Set(value); }
    public ObservableCollection<OxyColor> ComparisonColors { get; }

    public RelayCommand<OxyColor> DeleteColorCommand { get; }

    public SettingsViewModel()
    {
        ComparisonColors = new ObservableCollection<OxyColor>(ComparisonColorPalette.Select(OxyColor.Parse));
        DeleteColorCommand = new RelayCommand<OxyColor>(DeleteColor);
        Subscribe(ComparisonColors, ComparisonColorPalette, c => c.ToByteString(), c => c.ToByteString());
    }

    public void Set(object value, [CallerMemberName] string? propertyName = null)
    {
        var pInfo = typeof(Settings).GetProperty(propertyName!);
        pInfo?.SetValue(Settings.Instance, value);
        OnPropertyChanged(propertyName);
    }

    public static T Get<T>([CallerMemberName] string? propertyName = null)
    {
        var pInfo = typeof(Settings).GetProperty(propertyName!);
        return (T)pInfo!.GetValue(Settings.Instance)!;
    }

    private void DeleteColor(OxyColor col) => ComparisonColors.Remove(col);

}

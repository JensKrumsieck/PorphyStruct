using System.Runtime.CompilerServices;
using OxyPlot;
using PorphyStruct.Core;
using TinyMVVM;

namespace PorphyStruct.ViewModel
{
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
        public bool ShowXAxis { get => Get<bool>();  set => Set(value); }
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
        #endregion

        #region Export
        public float ExportWidth { get => Settings.Instance.ExportWidth; set => Settings.Instance.ExportWidth = value; }
        public float ExportHeight { get => Settings.Instance.ExportHeight; set => Settings.Instance.ExportHeight = value; }
        public float ExportDPI { get => Settings.Instance.ExportDPI; set => Settings.Instance.ExportDPI = value; }
        public string DefaultExportPath { get => Settings.Instance.DefaultExportPath; set => Settings.Instance.DefaultExportPath = value; }
        public string DefaultImportPath { get => Settings.Instance.DefaultImportPath; set => Settings.Instance.DefaultImportPath = value; }
        #endregion

        #region Simulation/Comparions
        public string SimulationBondColor { get => Get<string>(); set => Set(value); }
        public string SimulationMarkerColor { get => Get<string>(); set => Set(value);}
        public MarkerType SimulationMarkerType { get => Get<MarkerType>(); set => Set(value); }
        #endregion

        public void Set(object value, [CallerMemberName] string propertyName = null)
        {
            var pInfo = typeof(Settings).GetProperty(propertyName!);
            pInfo?.SetValue(Settings.Instance, value);
            OnPropertyChanged(propertyName);
        }

        public T Get<T>([CallerMemberName] string propertyName = null)
        {
            var pInfo = typeof(Settings).GetProperty(propertyName!);
            return (T)pInfo?.GetValue(Settings.Instance);
        }
    }
}

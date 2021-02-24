using OxyPlot;
using PorphyStruct.Core;
using TinyMVVM;

namespace PorphyStruct.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Font
        public string FontFamily { get => Settings.Instance.Font; set => Settings.Instance.Font = value; }
        public int FontSize { get => Settings.Instance.FontSize; set => Settings.Instance.FontSize = value; }
        public int FontWeight { get => Settings.Instance.FontWeight; set => Settings.Instance.FontWeight = value; }
        #endregion

        #region PlotArea
        public double BorderThickness { get => Settings.Instance.BorderThickness; set => Settings.Instance.BorderThickness = value; }
        public double Padding { get => Settings.Instance.Padding; set => Settings.Instance.Padding = value; }
        #endregion

        #region Axis
        public string AxisFormat { get => Settings.Instance.AxisFormat; set => Settings.Instance.AxisFormat = value; }
        public double AxisThickness { get => Settings.Instance.AxisThickness; set => Settings.Instance.AxisThickness = value; }
        public double LabelAngle { get => Settings.Instance.LabelAngle; set => Settings.Instance.LabelAngle = value; }
        public double LabelPadding { get => Settings.Instance.LabelPadding; set => Settings.Instance.LabelPadding = value; }
        public double LabelPosition { get => Settings.Instance.LabelPosition; set => Settings.Instance.LabelPosition = value; }
        public bool ShowXAxis { get => Settings.Instance.ShowXAxis; set => Settings.Instance.ShowXAxis = value; }
        public bool ShowZero { get => Settings.Instance.ShowZero; set => Settings.Instance.ShowZero = value; }
        #endregion

        #region Series
        public double BondThickness { get => Settings.Instance.BondThickness; set => Settings.Instance.BondThickness = value; }
        public string BondColor { get => Settings.Instance.BondColor; set => Settings.Instance.BondColor = value; }
        public MarkerType MarkerType { get => Settings.Instance.MarkerType; set => Settings.Instance.MarkerType = value; }
        public int MarkerSize { get => Settings.Instance.MarkerSize; set => Settings.Instance.MarkerSize = value; }
        public bool UseAtomRadiusMarkerSize { get => Settings.Instance.UseAtomRadiusMarkerSize; set => Settings.Instance.UseAtomRadiusMarkerSize = value; }
        public int MarkerBorderThickness { get => Settings.Instance.MarkerBorderThickness; set => Settings.Instance.MarkerBorderThickness = value; }
        public string MarkerBorderColor { get => Settings.Instance.MarkerBorderColor; set => Settings.Instance.MarkerBorderColor = value; }
        public string NotMarkedPoints { get => Settings.Instance.NotMarkedPoints; set => Settings.Instance.NotMarkedPoints = value; }
        #endregion

        #region Export
        public float ExportWidth { get => Settings.Instance.ExportWidth; set => Settings.Instance.ExportWidth = value; }
        public float ExportHeight { get => Settings.Instance.ExportHeight; set => Settings.Instance.ExportHeight = value; }
        public float ExportDPI { get => Settings.Instance.ExportDPI; set => Settings.Instance.ExportDPI = value; }
        public string DefaultExportPath { get => Settings.Instance.DefaultExportPath; set => Settings.Instance.DefaultExportPath = value; }
        public string DefaultImportPath { get => Settings.Instance.DefaultImportPath; set => Settings.Instance.DefaultImportPath = value; }
        #endregion
    }
}

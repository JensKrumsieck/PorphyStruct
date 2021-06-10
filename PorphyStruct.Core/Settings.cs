using OxyPlot;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TinyMVVM.Utility;

namespace PorphyStruct.Core
{
    public sealed class Settings : Singleton<Settings>
    {
        #region Font

        public string Font { get; set; } = "Arial";
        public int FontWeight { get; set; } = 500;
        public int FontSize { get; set; } = 18;
        #endregion

        #region PlotArea

        public double BorderThickness { get; set; }
        public double Padding { get; set; } = 8;
        #endregion

        #region Axis

        public string AxisFormat { get; set; } = "{0} / {1}";
        public double LabelAngle { get; set; } = -90d;
        public double LabelPadding { get; set; }
        public double LabelPosition { get; set; } = 0.5d;
        public double AxisThickness { get; set; } = 2d;
        public bool ShowXAxis { get; set; }
        public bool ShowZero { get; set; } = true;
        #endregion

        #region Series

        public double BondThickness { get; set; } = 2d;
        public string MarkerColor { get; set; } = "#FF000000";
        public string BondColor { get; set; } = "#FF000000";
        public MarkerType MarkerType { get; set; } = MarkerType.Circle;
        public int MarkerSize { get; set; } = 6;
        public bool UseAtomRadiusMarkerSize { get; set; }
        public int MarkerBorderThickness { get; set; }
        public string MarkerBorderColor { get; set; } = "#FF000000";
        public string NotMarkedPoints { get; set; } = "";
        public bool SingleColor { get; set; }

        public bool UseExtendedBasis { get; set; }
        #endregion

        #region Export

        public float ExportWidth { get; set; } = 3000f;
        public float ExportHeight { get; set; } = 1500f;
        public float ExportDPI { get; set; } = 300f;
        public string DefaultExportPath { get; set; } = "";
        public string DefaultImportPath { get; set; } = "";
        #endregion

        #region Simulation/Comparions
        public string SimulationBondColor { get; set; } = "#FF000000";
        public string SimulationMarkerColor { get; set; } = "#FFFF0000";
        public MarkerType SimulationMarkerType { get; set; } = MarkerType.Circle;
        public MarkerType ComparisonMarkerType { get; set; } = MarkerType.Circle;

        public List<string> ComparisonColorPalette { get; set; } = new List<string>
        {
            OxyColors.Red.ToByteString(),
            OxyColors.Blue.ToByteString(),
            OxyColors.Green.ToByteString(),
            OxyColors.Purple.ToByteString()
        };
        #endregion

        #region Data

        public bool HandlePhosphorusMetal { get; set; } = true;
        public bool HandleBoronMetal { get; set; } = true;
        public bool HandleSiliconMetal { get; set; } = false;
        public double SimulationOpacity { get; set; } = .5d;
        #endregion

        /// <summary>
        /// use for json only!
        /// For accessing settings use <see cref="Singleton.Instance"/>
        /// </summary>
        [JsonConstructor]
        public Settings() { }

        /// <summary>
        /// Load Settings
        /// </summary>
        public void Load()
        {
            var settings = Instance;
            if (!Directory.Exists(Constants.SettingsFolder)) Directory.CreateDirectory(Constants.SettingsFolder);
            if (File.Exists(Constants.SettingsLocation))
            {
                var file = File.ReadAllText(Constants.SettingsLocation);
                settings = JsonSerializer.Deserialize<Settings>(file);
            }

            var properties = typeof(Settings).GetProperties();
            foreach (var p in properties.Where(s => s.PropertyType != typeof(Settings))) p.SetValue(this, p.GetValue(settings));

            //save here to update potential missing settings into file
            Instance.Save();
        }

        /// <summary>
        /// Save Settings
        /// </summary>
        public void Save()
        {
            var content = JsonSerializer.Serialize(Instance, new JsonSerializerOptions { WriteIndented = true });
            //File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}/settings.json", content);
            File.WriteAllText(Constants.SettingsLocation, content);
        }
    }
}

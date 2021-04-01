using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;
using PorphyStruct.Core.Analysis.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot.Annotations;

namespace PorphyStruct.Core.Plot
{
    public static class PlotExtension
    {
        /// <summary>
        /// Colors for Bar Series. Use default proposal by mb
        /// </summary>
        private static readonly Dictionary<string, string> Colors = new Dictionary<string, string>
        {
            {"Doming", "#953735"},
            {"Saddling", "#17375E"},
            {"Ruffling", "#77933C"},
            {"WavingX", "#E46C0A"},
            {"WavingY", "#403152"},
            {"Propellering", "#4A452A"}
        };


        private static readonly List<string> Categories = new List<string>
        {
            "Propellering",
            "WavingY",
            "WavingX",
            "Ruffling",
            "Saddling",
            "Doming"
        };

        static PlotExtension()
        {
            if (Settings.Instance.ComparisonColorPalette.Count < 6) return;
            Colors["Doming"] = Settings.Instance.ComparisonColorPalette[0];
            Colors["Saddling"] = Settings.Instance.ComparisonColorPalette[1];
            Colors["Ruffling"] = Settings.Instance.ComparisonColorPalette[2];
            Colors["WavingX"] = Settings.Instance.ComparisonColorPalette[3];
            Colors["WavingY"] = Settings.Instance.ComparisonColorPalette[4];
            Colors["Propellering"] = Settings.Instance.ComparisonColorPalette[5];
        }

        public static MarginBarSeries PrepareBarSeries(this Simulation sim)
        { 
            var series = new MarginBarSeries(150)
            {
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString ="{0:N3} Å",
            };
            foreach (var prop in sim.SimulationResult.ToArray().Reverse())
            {
                var key = prop.Key.Replace("2", "");
                var color = OxyColor.Parse(Colors[key]);
                if (prop.Key.Contains("2")) color = OxyColor.FromAColor(200, color);
                series.Items.Add(new BarItem(prop.Value, Categories.IndexOf(key)) { Color = color });
            }
            return series;
        }

        public static BasePlotModel PrepareSummaryPlot(this Simulation sim)
        {
            var model = new BasePlotModel();
            var yAxis = new CategoryAxis
            {
                Position = AxisPosition.Left,
                ItemsSource = Categories,
                AxislineThickness = Settings.Instance.AxisThickness,
                GapWidth = .2,
                IsAxisVisible = false
            };
            model.Axes.Add(yAxis);
            model.Series.Add(sim.PrepareBarSeries());

            for(var i = 0; i < Categories.Count; i++)
            {
                var a = new TextAnnotation
                {
                    Font = Settings.Instance.Font,
                    FontSize = Settings.Instance.FontSize,
                    FontWeight = Settings.Instance.FontWeight,
                    TextColor = OxyColors.Black,
                    TextPosition = new DataPoint(0, i),
                    Text = Categories[i],
                    StrokeThickness = 0,
                    Padding = new OxyThickness(0)
                };
                model.Annotations.Add(a);
            }

            model.PlotAreaBorderThickness = new OxyThickness(0);
            yAxis.TickStyle = TickStyle.None;
            model.XAxis.IsAxisVisible = false;
            model.XAxis.Zoom(Math.Min(-.5, sim.SimulationResult.Min(s => s.Value) * 1.5), Math.Max(.5, sim.SimulationResult.Max(s => s.Value) * 1.5));
            return model;
        }

        public static void ExportSummaryPlot(this Simulation sim, string filename) => PngExporter.Export(
            sim.PrepareSummaryPlot(), filename, (int)Settings.Instance.ExportWidth, (int)Settings.Instance.ExportHeight,
            (int)Settings.Instance.ExportDPI);
    }
}

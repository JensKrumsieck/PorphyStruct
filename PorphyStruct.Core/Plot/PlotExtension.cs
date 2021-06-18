using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;
using PorphyStruct.Core.Analysis.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PorphyStruct.Core.Plot
{
    public static class PlotExtension
    {
        /// <summary>
        /// Colors for Bar Series. Use default proposal by mb
        /// </summary>
        private static readonly Dictionary<string, string> Colors = new()
        {
            { "Doming", "#953735" },
            { "Saddling", "#17375E" },
            { "Ruffling", "#77933C" },
            { "WavingX", "#E46C0A" },
            { "WavingY", "#403152" },
            { "Propellering", "#4A452A" }
        };


        private static readonly List<string> Categories = new()
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
            var series = new MarginBarSeries(Settings.Instance.FontSize * 5.5)
            {
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0:N3} Å",
            };
            foreach (var prop in sim.SimulationResult.ToArray().Reverse())
            {
                var key = prop.Key.Replace("2", "");
                var color = OxyColor.Parse(Colors[key]);
                var value = Settings.Instance.UseExtendedBasis ? prop.Value : Math.Abs(prop.Value);
                if (prop.Key.Contains("2")) color = OxyColor.FromAColor(200, color);
                series.Items.Add(new BarItem(value, Categories.IndexOf(key)) { Color = color });
            }
            return series;
        }

        public static TitleOverridePlotModel PrepareSummaryPlot(this MacrocycleProperties props)
        {
            var delta = props.OutOfPlaneParameter.Value - props.Simulation.OutOfPlaneParameter.Value;
            var model = new TitleOverridePlotModel()
            {
                Title = "D_{oop} = " + props.OutOfPlaneParameter.Value.ToString("N3") + " Å",
                Subtitle = "δ_{ oop } = " + delta.ToString("N3") + " Å  — " + (delta / props.OutOfPlaneParameter.Value).ToString("P1"),
                SubtitleFontWeight = Settings.Instance.FontWeight - 100,
                SubtitleFont = Settings.Instance.Font,
                SubtitleFontSize = Settings.Instance.FontSize - 2,
                TitleFont = Settings.Instance.Font,
                TitleFontWeight = Settings.Instance.FontWeight,
                TitleFontSize = Settings.Instance.FontSize + 2,
                TitlePadding = 4,
                SubtitleColor = OxyColor.Parse("#293133")
            };
            var yAxis = new CategoryAxis
            {
                Position = AxisPosition.Left,
                ItemsSource = Categories,
                AxislineThickness = Settings.Instance.AxisThickness,
                GapWidth = .2,
                IsAxisVisible = false
            };
            model.Axes.Add(yAxis);
            var series = props.Simulation.PrepareBarSeries();
            model.Series.Add(series);

            model.PlotAreaBorderThickness = new OxyThickness(0);
            yAxis.TickStyle = TickStyle.None;
            model.XAxis.IsAxisVisible = false;
            var min = props.Simulation.SimulationResult.Min(s => s.Value);
            var max = props.Simulation.SimulationResult.Max(s => s.Value);
            model.XAxis.Zoom(
                Settings.Instance.UseExtendedBasis ? Math.Min(-1, min * 2) : -.1,
                Math.Max(1, max * 2));

            PrepareYAxisAnnotations(ref model);
            return model;
        }

        private static void PrepareYAxisAnnotations(ref TitleOverridePlotModel model)
        {
            for (var i = 0; i < Categories.Count; i++)
            {
                model.Annotations.Add(new TextAnnotation
                {
                    Font = Settings.Instance.Font,
                    FontSize = Settings.Instance.FontSize,
                    FontWeight = (Settings.Instance.FontWeight + 200) % 900,
                    TextColor = OxyColors.Black,
                    TextPosition = new DataPoint(0, i),
                    Text = Categories[i].Substring(0, 3).ToLower() +
                           (Categories[i].Contains("Waving") ? " " + Categories[i].Last() : ""),
                    StrokeThickness = 0,
                    Padding = new OxyThickness(0),
                    TextVerticalAlignment = VerticalAlignment.Middle
                });
            }
        }

        public static void ExportSummaryPlot(this MacrocycleProperties props, string filename, string extension)
        {
            switch (extension)
            {
                case "png":
                    PngExporter.Export(
                        props.PrepareSummaryPlot(), filename + ".png", (int)Settings.Instance.ExportWidth, (int)Settings.Instance.ExportHeight,
                        (int)Settings.Instance.ExportDPI);
                    break;
                case "svg":
                    new SvgExporter
                    {
                        Height = Settings.Instance.ExportHeight,
                        Width = Settings.Instance.ExportWidth,
                        Dpi = Settings.Instance.ExportDPI
                    }.Export(props.PrepareSummaryPlot(), File.Create(filename + ".svg"));
                    break;
            }
        }
    }
}

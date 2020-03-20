using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using PorphyStruct.Chemistry.Properties;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace PorphyStruct.Chemistry
{
    public static class MacrocycleExporter
    {
        /// <summary>
        /// Exports Graph into file with given exporter
        /// neccessary becaus for quality reasons we need wpf exporters... :/
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="filename"></param>
        /// <param name="exporter"></param>
        /// <param name="extension"></param>
        public static void ExportGraph(this PlotModel pm, string filename, IExporter exporter, string extension = "png")
        {
            using var file = File.Create(filename + "Analysis." + extension);
            //for svg double sizes ;)
            if (extension == "svg")
            {
                pm.DefaultFontSize *= 2;
                pm.Padding = new OxyThickness(pm.Padding.Left * 2.5);
                foreach (var axis in pm.Axes) axis.AxislineThickness *= 2;
                foreach (ScatterSeries series in pm.Series) series.MarkerSize *= 2;
                foreach (ArrowAnnotation annotation in pm.Annotations.Where(s => s.GetType() == typeof(ArrowAnnotation))) annotation.StrokeThickness *= 2;
                if (Core.Properties.Settings.Default.showBox) pm.PlotAreaBorderThickness = new OxyThickness(Core.Properties.Settings.Default.lineThickness * 2);
            }

            exporter.Export(pm, file);

            if (extension == "svg")
            {
                pm.DefaultFontSize /= 2;
                foreach (var axis in pm.Axes) axis.AxislineThickness /= 2;
                foreach (ScatterSeries series in pm.Series) series.MarkerSize /= 2;
                foreach (ArrowAnnotation annotation in pm.Annotations.Where(s => s.GetType() == typeof(ArrowAnnotation))) annotation.StrokeThickness /= 2;
                if (Core.Properties.Settings.Default.showBox) pm.PlotAreaBorderThickness = new OxyThickness(Core.Properties.Settings.Default.lineThickness);
                pm.Padding = new OxyThickness(pm.Padding.Left / 2.5);
            }
        }

        /// <summary>
        /// Saves ASCII File of a given Plotmodel
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="filename"></param>
        /// <param name="extension"></param>
        public static void SaveASCII(this PlotModel pm, string filename, string extension = "dat")
        {
            var export = new List<OxyPlot.Series.ScatterSeries>();
            foreach (OxyPlot.Series.ScatterSeries s in pm.Series) export.Add(s);

            //make title string
            string title = "A;X;";
            foreach (OxyPlot.Series.Series s in export) title += s.Title + ";";

            //write data
            using StreamWriter sw = new StreamWriter(filename + "Data." + extension);
            sw.WriteLine(title);

            //write data
            for (int i = 0; i < export[0].ItemsSource.OfType<AtomDataPoint>().Count(); i++)
            {
                string line = export[0].ItemsSource.OfType<AtomDataPoint>().ElementAt(i).atom.Identifier + ";";
                line += export[0].ItemsSource.OfType<AtomDataPoint>().ElementAt(i).X + ";";
                for (int j = 0; j < export.Count; j++) line += export[j].ItemsSource.OfType<AtomDataPoint>().ElementAt(i).Y + ";";
                sw.WriteLine(line);
            }
        }

        /// <summary>
        /// Saves Properties of given Macrocycle
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="filename"></param>
        /// <param name="extension"></param>
        public static void SaveProperties(this Macrocycle cycle, string filename, string extension = "json")
        {
            var properties = cycle.Properties.ToLookup(x => x.Key, y => y.Value).ToDictionary(group => group.Key, group => group.SelectMany(value => value));
            switch (extension)
            {
                case "json":
                default:
                    {
                        var options = new JsonSerializerOptions() { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
                        var json = JsonSerializer.Serialize(properties, typeof(Dictionary<string, IEnumerable<Property>>), options);
                        using StreamWriter sw = new StreamWriter(filename + "Properties." + extension);
                        sw.Write(json);
                    }
                    break;
                case "txt":
                    {
                        using StreamWriter sw = new StreamWriter(filename + "Properties." + extension);
                        foreach (var g in properties)
                        {
                            sw.WriteLine(g.Key + ":");
                            foreach (var p in g.Value) sw.WriteLine("\t" + p.Name + ": " + p.Value);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Exports Molecule as IXYZ File
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="filename"></param>
        /// <param name="CycleOnly"></param>
        public static void SaveIXYZ(this Macrocycle cycle, string filename, bool CycleOnly = false)
        {
            filename += (CycleOnly ? "Macrocycle" : "Molecule") + ".ixyz";
            var export = cycle.Atoms.AsEnumerable();
            if (CycleOnly) export = cycle.Atoms.Where(s => s.IsMacrocycle);

            using StreamWriter sw = new StreamWriter(filename);
            sw.WriteLine(export.Count());
            sw.WriteLine(filename);
            foreach (Atom a in export) sw.WriteLine(a.ExportText);
        }
    }
}

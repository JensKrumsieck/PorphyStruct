using OxyPlot;
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
        /// <param name="cycle"></param>
        /// <param name="filename"></param>
        /// <param name="exporter"></param>
        /// <param name="extension"></param>
        public static void ExportGraph(this PlotModel pm, string filename, IExporter exporter, string extension = "png")
        {
            using var file = File.Create(filename + "Analysis." + extension);
            exporter.Export(pm, file);
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
        /// <param name="extensionm"></param>
        public static void SaveProperties(this Macrocycle cycle, string filename, string extension = "json")
        {
            var properties = cycle.Properties.ToLookup(x => x.Key, y => y.Value).ToDictionary(group => group.Key, group => group.SelectMany(value => value));
            switch (extension)
            {
                case "json":
                default:
                    {
                        var options = new JsonSerializerOptions()
                        {
                            WriteIndented = true,
                            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                        };
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

        /// <summary>
        /// Exports simulation to xml
        /// </summary>
        /// <param name="sim"></param>
        /// <param name="cycle"></param>
        /// <param name="filename"></param>
        //public static void SaveResult(this Simulation sim, Macrocycle exp, string filename)
        //{
        //    List<XElement> simRes = sim.par.Select(par => new XElement("parameter", new XAttribute("name", par.Key), par.Value)).ToList();
        //    simRes.AddRange(sim.par.Select(par => new XElement("absolute", new XAttribute("name", par.Key), par.Value / 100 * sim.cycle.MeanDisplacement())).ToList());

        //    //List<XElement> metrix = exp.Metrics().Select(met => new XElement(met.Key.Split('_')[0], new XAttribute("atoms", met.Key.Split('_')[1]), met.Value)).ToList();

        //    simRes.Add(new XElement("doop", sim.cycle.MeanDisplacement()));
        //    simRes.Add(new XElement("errors",
        //        new XElement("data", sim.errors[0]),
        //        new XElement("derivative", sim.errors[1]),
        //        new XElement("integral", sim.errors[2])
        //        ));

        //    XElement Molecule = new XElement("molecule",
        //        new XAttribute("name", exp.Title),
        //        new XElement("type", exp.GetType().Name),
        //        new XElement("simulation", simRes)
        //        //new XElement("metrics", metrix)
        //        );
        //    Molecule.Save(File.Create(filename + "Result.xml"));
        //}
    }
}

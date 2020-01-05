using OxyPlot;
using OxyPlot.Reporting;
using OxyPlot.Wpf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace PorphyStruct.Chemistry
{
    public static class MacrocycleExporter
    {
        /// <summary>
        /// Saves the Graph of a given PlotModel
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="filename"></param>
        /// <param name="extension"></param>
        public static void SaveGraph(PlotModel pm, Macrocycle cycle, string filename, string extension = "png")
        {
            pm = MacrocycleExporter.ExportModel(pm, cycle.Bonds.Count);

            //exports png
            if (extension == "png")
                PngExporter.Export(pm, filename + "Analysis.png", Properties.Settings.Default.pngWidth, Properties.Settings.Default.pngHeight, OxyColors.Transparent, Properties.Settings.Default.pngRes);

            //exports svg
            if (extension == "svg")
                new OxyPlot.Wpf.SvgExporter()
                {
                    Width = Properties.Settings.Default.pngWidth,
                    Height = Properties.Settings.Default.pngHeight
                }.ExportToFile(pm, filename + "Analysis.svg");
        }

        /// <summary>
        /// Saves ASCII File of a given Plotmodel
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="filename"></param>
        /// <param name="extension"></param>
        public static void SaveASCII(PlotModel pm, string filename, string extension = ".dat")
        {
            var export = new List<OxyPlot.Series.ScatterSeries>();
            foreach (OxyPlot.Series.ScatterSeries s in pm.Series) export.Add(s);

            //make title string
            string title = "A;X;";
            foreach (OxyPlot.Series.Series s in export) title += s.Title + ";";

            //write data
            using (StreamWriter sw = new StreamWriter(filename + "Data." + extension))
            {
                sw.WriteLine(title);

                //write data
                for (int i = 0; i < export[0].ItemsSource.OfType<AtomDataPoint>().Count(); i++)
                {
                    string line = export[0].ItemsSource.OfType<AtomDataPoint>().ElementAt(i).atom.Identifier + ";";
                    line += export[0].ItemsSource.OfType<AtomDataPoint>().ElementAt(i).X + ";";
                    for (int j = 0; j < export.Count; j++)
                    {
                        line += export[j].ItemsSource.OfType<AtomDataPoint>().ElementAt(i).Y + ";";
                    }
                    sw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Exports Molecule as IXYZ File
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="filename"></param>
        /// <param name="CycleOnly"></param>
        public static void SaveIXYZ(Macrocycle cycle, string filename, bool CycleOnly = false)
        {
            filename += (CycleOnly ? "Macrocycle" : "Molecule") + ".ixyz";
            var export = cycle.Atoms.AsEnumerable();
            if (CycleOnly) export = cycle.Atoms.Where(s => s.IsMacrocycle);

            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(export.Count());

                sw.WriteLine(filename);

                foreach (Atom a in export) sw.WriteLine(a.ExportText);
            }
        }


        /// <summary>
        /// removes invisible series and returns model to export
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        public static PlotModel ExportModel(PlotModel pm, int bonds)
        {
            //remove bonds
            List<OxyPlot.Annotations.Annotation> annotations = new List<OxyPlot.Annotations.Annotation>();
            foreach (OxyPlot.Series.ScatterSeries s in pm.Series)
            {
                if (!s.IsVisible)
                {
                    int index = pm.Series.IndexOf(s);
                    for (int i = index * bonds; i < (index * bonds) + bonds; i++)
                    {
                        annotations.Add(pm.Annotations[i]);
                    }
                }
            }
            foreach (OxyPlot.Annotations.Annotation a in annotations)
            {
                pm.Annotations.Remove(a);
            }

            return pm;
        }


        ///<summary>
        /// gets Reportstyle
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static ReportStyle ReportStyle => new ReportStyle(Properties.Settings.Default.defaultFont, Properties.Settings.Default.defaultFont, Properties.Settings.Default.defaultFont)
        {
            FigureTextFormatString = "Figure {0}: {1}.",
            TableCaptionFormatString = "Table {0}: {1}."
        };
    }
}

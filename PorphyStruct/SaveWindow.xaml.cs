using MaterialDesignThemes.Wpf;
using OxyPlot;
using OxyPlot.OpenXml;
using OxyPlot.Pdf;
using OxyPlot.Reporting;
using OxyPlot.Wpf;
using PorphyStruct.Chemistry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using winforms = System.Windows.Forms;
using System.Windows;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        public PlotModel model;
        public Macrocycle cycle;
        public Simulation sim;
        public string filename = "";

        public SaveWindow(PlotModel model, Macrocycle cycle, Simulation sim = null)
        {
            InitializeComponent();
            this.model = model;
            this.cycle = cycle;
            this.sim = sim;
            DataContext = this;
            NameTB.Text = cycle.Title;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<FileType> types = new List<FileType>();
            foreach (object o in TypeList.SelectedItems)
            {
                types.Add((FileType)o);
            }
            foreach (FileType t in types)
            {
                switch (t.Title)
                {
                    case "Graph":
                        SaveGraph(t.Extension);
                        break;
                    case "ASCII":
                        SaveASCII(t.Extension);
                        break;
                    case "Macrocycle":
                        SaveMolecule(t.Extension, true);
                        break;
                    case "Molecule":
                        SaveMolecule(t.Extension);
                        break;
                    case "Report":
                        SaveReport(t.Extension);
                        break;
                }
            }
            this.Close();
        }

        /// <summary>
        /// Handles Saving Graphs
        /// </summary>
        /// <param name="Extension"></param>
        private void SaveGraph(string Extension)
        {
            //exports png
            if (Extension == "png")
                PngExporter.Export(model, this.filename + "Analysis.png", Properties.Settings.Default.pngWidth, Properties.Settings.Default.pngHeight, OxyColors.Transparent, Properties.Settings.Default.pngRes);

            //exports svg
            if (Extension == "svg")
            {
                var svg = new OxyPlot.Wpf.SvgExporter()
                {
                    Width = Properties.Settings.Default.pngWidth,
                    Height = Properties.Settings.Default.pngHeight
                };
                svg.ExportToFile(model, this.filename + "Analysis.svg");
            }
        }

        /// <summary>
        /// Saves ascii file
        /// </summary>
        /// <param name="Extension"></param>
        private void SaveASCII(string Extension)
        {
            using (StreamWriter sw = new StreamWriter(this.filename + "Data." + Extension))
            {
                sw.WriteLine("X;Y");
                List<AtomDataPoint> data = new List<AtomDataPoint>();
                OxyPlot.Series.ScatterSeries series = (OxyPlot.Series.ScatterSeries)model.Series.Where(t => t.Title == "Exp.").FirstOrDefault();
                data.AddRange((List<AtomDataPoint>)series.ItemsSource);
                for (int i = 0; i < data.Count; i++)
                {
                    sw.WriteLine(data[i].X.ToString() + ";" + data[i].Y.ToString());
                }
            }
        }

        /// <summary>
        /// saves Molecule/Macrocycle
        /// </summary>
        /// <param name="Extension"></param>
        /// <param name="CycleOnly"></param>
        private void SaveMolecule(string Extension, bool CycleOnly = false)
        {
            string filename = this.filename + (CycleOnly ? "Macrocycle" : "Molecule");
            if (Extension == "ixyz")
            {
                filename += ".ixyz";

                using (StreamWriter sw = new StreamWriter(filename))
                {
                    if (!CycleOnly) sw.WriteLine(cycle.Atoms.Count);
                    else
                        sw.WriteLine(cycle.Atoms.Where(s => s.isMacrocycle).ToList().Count);

                    sw.WriteLine(filename);

                    foreach (Atom a in cycle.Atoms)
                    {
                        if (CycleOnly && a.isMacrocycle)
                        {
                            sw.WriteLine(a.Identifier + "/" + a.Type + "\t" + a.X.ToString("N8", System.Globalization.CultureInfo.InvariantCulture) + "\t" + a.Y.ToString("N8", System.Globalization.CultureInfo.InvariantCulture) + "\t" + a.Z.ToString("N8", System.Globalization.CultureInfo.InvariantCulture));
                        }
                        else if (!CycleOnly)
                        {
                            sw.WriteLine(a.Identifier + "/" + a.Type + "\t" + a.X.ToString("N8", System.Globalization.CultureInfo.InvariantCulture) + "\t" + a.Y.ToString("N8", System.Globalization.CultureInfo.InvariantCulture) + "\t" + a.Z.ToString("N8", System.Globalization.CultureInfo.InvariantCulture));
                        }
                    }
                }
            }
        }

        private void SaveReport(string Extension)
        {
            string filename = this.filename + "Report." + Extension;
            Report r = SaveReport();
            if (Extension == "docx")
            {
                using (var w = new WordDocumentReportWriter(filename))
                {
                    w.WriteReport(r, GetReportStyle());
                    w.Save();
                }
            }
            if (Extension == "pdf")
            {
                using (var w = new PdfReportWriter(filename))
                {
                    w.WriteReport(r, GetReportStyle());
                }

            }
        }

        ///<summary>
        /// gets Reportstyle
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private ReportStyle GetReportStyle()
        {
            string lang = Properties.Settings.Default.exportLang;
            ReportStyle reportStyle = new ReportStyle(Properties.Settings.Default.defaultFont, Properties.Settings.Default.defaultFont, Properties.Settings.Default.defaultFont)
            {
                FigureTextFormatString = (lang == "de" ? "Abbildung " : "Figure ") + "{0}. {1}",
                TableCaptionFormatString = (lang == "de" ? "Tabelle " : "Table ") + "{0}. {1}"
            };
            return reportStyle;
        }

        /// <summary>
        /// Gets Report
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private Report SaveReport()
        {
            string lang = Properties.Settings.Default.exportLang;
            Report report = new Report()
            {
                Title = (lang == "de" ? "Analyse " : "Analysis "),
            };
            ReportSection main = new ReportSection();
            report.AddHeader(1, (lang == "de" ? "Konformationsanalyse " : "Conformational analysis ") + "TEXT" + " - " + cycle.Title);
            report.Add(main);
            string type = "";
            if (cycle.type == Macrocycle.Type.Corrole)
                type = (lang == "de" ? "Corrol" : "Corrole");
            if (cycle.type == Macrocycle.Type.Porphyrin)
                type = (lang == "de" ? "Porphyrin" : "Porhyrin");
            if (cycle.type == Macrocycle.Type.Norcorrole)
                type = (lang == "de" ? "Norcorrol" : "Norcorrole");
            if (cycle.type == Macrocycle.Type.Corrphycene)
                type = (lang == "de" ? "Corrphycen" : "Corrphycene");
            if (cycle.type == Macrocycle.Type.Norcorrole)
                type = (lang == "de" ? "Porphycen" : "Porphycene");

            main.AddHeader(2, (lang == "de" ? "Makrozyklische Konformation" : "Macrocyclic Conformation"));
            main.AddParagraph((lang == "de" ? "In nachfolgender Abbildung ist die Konformationsanalyse von "
                                            + "TEXT" +
                                            " als Auslenkungsdiagramm dargestellt. Durch die Ringatome des " + type + "s wurde eine mittlere"
                                            + "Ebene gelegt. Der Abstand der Ringatome zur Ebene wird gegen berechnete Kreiskoordinaten aufgetragen."
                                        : "The following figure shows the conformational analysis of "
                                            + "TEXT" +
                                            " displayed as a displacement diagram. A middle plane was placed through the ring atoms of  " + type.ToLower() + "." +
                                            " The distance of the ring atoms to the plane is plotted against calculated circle coordinates."));
            //export as image first
            PngExporter.Export(model, this.filename + "Analysis.png", Properties.Settings.Default.pngWidth, Properties.Settings.Default.pngHeight, OxyColors.Transparent, Properties.Settings.Default.pngRes);
            main.AddImage(this.filename + "Analysis.png", (lang == "de" ? "Auslenkungsdiagramm der Konformationsanalyse von " + "TEXT" : "Displacement diagram of " + "TEXT"));
            if (this.sim != null)
            {
                ReportSection simu = new ReportSection();
                report.Add(simu);
                simu.AddHeader(2, "Simulation");
                simu.AddParagraph((lang == "de" ? "Mittels der Methode kleinster Quadrate wurde die Konforamtion des " + type + "s auf die Schwingungsnormalmoden " +
                    "von Metallo" + type.ToLower() + "en zurückgeführt. Die simulierte Zusammensetzung ist nachfolgender Tabelle zu entnehmen."
                    : "Using the least squares method, the conformation of the" + type.ToLower() + "was traced back to the vibration normal modes of " +
                    "metallo" + type.ToLower() + "s. The simulated composition is shown in the following table."));
                simu.AddPropertyTable((lang == "de" ? "Simulationsparameter mit einem mittleren Auslenkungsparameter von " : "Simulationparameters with a mean displacement parameter of") + sim.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture), sim.par);
                simu.AddParagraph((lang == "de"
                        ? "Die Summen der Auslenkungsfehlerquadrate ergibt für die Daten, deren Ableitung und Integral die folgenden Werte: "
                        : "The sums of squared displacement errors for data, derivative and integral are: ")
                        + sim.errors[0] + ", " + sim.errors[1] + ", " + sim.errors[2] + ".");
            }
            return report;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Properties.Settings.Default.savePath != "")
                initialDir = Properties.Settings.Default.savePath;
            winforms.FolderBrowserDialog fbd = new winforms.FolderBrowserDialog
            {
                SelectedPath = initialDir
            };
            if (fbd.ShowDialog() == winforms.DialogResult.OK)
            {
                PathTB.Text = fbd.SelectedPath;
            }
        }
    }

    public struct FileType
    {
        public string Title { get; set; }
        public PackIcon Icon { get; set; }
        public PackIcon secondary { get; set; }
        public string Extension { get; set; }

        public override string ToString()
        {
            return Title + "." + Extension;
        }
    }
}

using MaterialDesignThemes.Wpf;
using OxyPlot;
using OxyPlot.OpenXml;
using OxyPlot.Pdf;
using OxyPlot.Reporting;
using OxyPlot.Wpf;
using PorphyStruct.Chemistry;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using winforms = System.Windows.Forms;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        public PlotModel model { get; set; }
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

        /// <summary>
        /// Handles file saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //validate form
            if (PathTB.Text == "" || !Directory.Exists(PathTB.Text))
            {
                MessageBox.Show("The specified directory does not exist!", "I/O Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (TypeList.SelectedItems.Count == 0)
            {

                MessageBox.Show("No Datatype has been selected!", "Datatype Empty", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this.filename = PathTB.Text + "/" + NameTB.Text + "_";
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
            Close();
        }

        /// <summary>
        /// Handles Saving Graphs
        /// </summary>
        /// <param name="Extension"></param>
        private void SaveGraph(string Extension)
        {
            //reanalyze
            Application.Current.Windows.OfType<MainWindow>().First().Analyze();

            //check if data is present
            if (model.Series.Where(s => s.IsVisible = true).Count() == 0)
            {
                MessageBox.Show("No Data present!", "Data Empty", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //get number of bonds.
            int bonds = 0;
            switch (cycle.type)
            {
                case Macrocycle.Type.Corrole:
                    bonds = Macrocycle.CorroleBonds.Count;
                    break;
                case Macrocycle.Type.Corrphycene:
                    bonds = Macrocycle.CorrphyceneBonds.Count;
                    break;
                case Macrocycle.Type.Norcorrole:
                    bonds = Macrocycle.NorcorroleBonds.Count;
                    break;
                case Macrocycle.Type.Porphycene:
                    bonds = Macrocycle.PorphyceneBonds.Count;
                    break;
                case Macrocycle.Type.Porphyrin:
                    bonds = Macrocycle.PorphyrinBonds.Count;
                    break;
            }

            //remove bonds
            List<OxyPlot.Annotations.Annotation> annotations = new List<OxyPlot.Annotations.Annotation>();
            foreach (OxyPlot.Series.ScatterSeries s in model.Series)
            {
                if (!s.IsVisible)
                {
                    int index = model.Series.IndexOf(s);
                    for (int i = index * bonds; i < (index * bonds) + bonds; i++)
                    {
                        annotations.Add(model.Annotations[i]);
                    }
                }
            }
            foreach (OxyPlot.Annotations.Annotation a in annotations)
            {
                model.Annotations.Remove(a);
            }


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
            List<OxyPlot.Series.ScatterSeries> export = new List<OxyPlot.Series.ScatterSeries>();
            foreach(OxyPlot.Series.ScatterSeries s in model.Series)
            {
                export.Add(s);
            }

            if (export.Count == 0)
            {
                MessageBox.Show("No ASCII Data present!", "Data Empty", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //make title string
            string title = "X;";
            foreach (OxyPlot.Series.Series s in export) title += s.Title + ";";

            //write data
            using (StreamWriter sw = new StreamWriter(this.filename + "Data." + Extension))
            {
                sw.WriteLine(title);

                //write data
                for (int i = 0; i < export[0].ItemsSource.OfType<AtomDataPoint>().Count(); i++)
                {
                    string line = export[0].ItemsSource.OfType<AtomDataPoint>().ElementAt(i).X + ";";
                    for(int j = 0; j < export.Count; j++)
                    {
                        line += export[j].ItemsSource.OfType<AtomDataPoint>().ElementAt(i).Y + ";";
                    }
                    sw.WriteLine(line);
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
            //no validation since a molecule is ALWAYS present!.

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

        /// <summary>
        /// Saves Report
        /// </summary>
        /// <param name="Extension"></param>
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
            report.AddHeader(1, "Conformational analysis " + NameTB.Text + cycle.Title);
            report.Add(main);
            string type = cycle.type.ToString();

            main.AddHeader(2, "Macrocyclic Conformation");
            //save graph. overwrittes if already done.
            this.SaveGraph(".png");
            

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

        /// <summary>
        /// handles search button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        public bool HasData(string data)
        {
            if ((data == "Graph" || data == "ASCII" || data == "Report") && (model == null || model.Series.Count == 0)) return false;
            if (data == "SimResult" && sim == null) return false;
            return true;
        }
    }
    public struct FileType
    {
        public string Title { get; set; }
        public PackIcon Icon { get; set; }
        public PackIcon secondary { get; set; }
        public string Extension { get; set; }

        public bool IsEnabled
        {
            get
            {
                SaveWindow sw = Application.Current.Windows.OfType<SaveWindow>().First();
                return sw.HasData(Title);
            }
        }

        public override string ToString()
        {
            return Title + "." + Extension;
        }
    }
}

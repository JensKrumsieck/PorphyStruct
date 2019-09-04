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
            //reanalyze
            if (Application.Current.Windows.OfType<MainWindow>().First().normalize) Application.Current.Windows.OfType<MainWindow>().First().NormalizeButton_Click(null, null);
            Application.Current.Windows.OfType<MainWindow>().First().Analyze();

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

            //check if graph is present when report is present, otherwise add! (rip = report is present, gip = graph ...)
            bool rip = false;
            bool gip = false;
            foreach (object o in TypeList.SelectedItems)
            {
                if (((FileType)o).Title == "Report")
                {
                    types.Add(new FileType() { Title = "Graph", Extension = "png" });
                    if(sim != null) types.Add(new FileType() { Title = "SimResult", Extension = "png" });
                }
            }

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
                    case "SimResult":
                        SaveSimResult(t.Extension);
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
            foreach (OxyPlot.Series.ScatterSeries s in model.Series)
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
                    for (int j = 0; j < export.Count; j++)
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
            ReportStyle reportStyle = new ReportStyle(Properties.Settings.Default.defaultFont, Properties.Settings.Default.defaultFont, Properties.Settings.Default.defaultFont)
            {
                FigureTextFormatString = "Figure {0}: {1}.",
                TableCaptionFormatString = "Table {0}: {1}."
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
                Title = "Analysis"
            };
            ReportSection main = new ReportSection();
            report.AddHeader(1, "Conformational analysis " + NameTB.Text + cycle.Title);
            report.Add(main);
            string type = cycle.type.ToString();

            //basic information
            main.AddHeader(2, "Macrocyclic Conformation");
            main.AddParagraph("The following figure shows the conformational analysis of " + (cycle.Title != "" ? cycle.Title : "the input macrocycle ")
                    + "displayed as displacement diagram. A middle plane was places through the " + type.ToLower() + "'s ring atoms. "
                    + "The distance of the ring atoms to the mean plane is plotted against calculated circle coordinates.");


            main.AddImage(this.filename + "Analysis.png", "Displacement Diagram of the " + type.ToLower());

            //get exp series
            OxyPlot.Series.ScatterSeries exp = (OxyPlot.Series.ScatterSeries)model.Series.FirstOrDefault(s => s.Title == "Exp.");
            main.AddItemsTable("Experimental Data",
                exp.ItemsSource.OfType<AtomDataPoint>(),
                new List<ItemsTableField> { new ItemsTableField("X", "X") { Width = 1000 }, new ItemsTableField("Y", "Y") { Width = 1000 }
                });

            if (this.sim != null)
            {
                ReportSection simu = new ReportSection();
                report.Add(simu);
                simu.AddHeader(2, "Simulation Details");
                simu.AddParagraph("A simulation has been done using the least squares methode. The conformation of the "
                    + type.ToLower() + " was traced back to the vibration normal modes of metallo" + type.ToLower() + "s. "
                    + "The standard vibration modes were obtained by DFT analysis of a metallo" + type.ToLower() + " at "
                    + "B3LYP/Def2-SVP level of theory. The simulated conformation was calculated with an error of " + sim.errors[0]
                    + " as root of the sum of the squared errors per atom. For the derivates the error is " + sim.errors[1]
                    + " and for the integrals the following error was measured " + sim.errors[2]);

                string composition = "";
                string absComposition = "";
                foreach (KeyValuePair<string, double> i in sim.par)
                {
                    composition += i.Value.ToString("N2", System.Globalization.CultureInfo.InvariantCulture) + "% of " + i.Key + ",";
                    absComposition += (i.Value / 100 * sim.MeanDisplacement()).ToString("N4", System.Globalization.CultureInfo.InvariantCulture) + " of " + i.Key + ",";
                }
                composition.Remove(composition.LastIndexOf(','));
                simu.AddParagraph("The final composition of normal modes is " + composition + " as also listed in the table below. "
                    + "The mean displacement parameter is " + sim.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture)
                    + " The absolute composition therefore is " + absComposition + ".");

                simu.AddImage(this.filename + "SimResult.png", "Visualization of Simulationparameters");
                simu.AddPropertyTable("Simulationparameters with a mean displacement parameter of " + sim.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture), sim.par);
            }
            return report;
        }

        /// <summary>
        /// Saves the sim result as plot
        /// </summary>
        /// <param name="extension"></param>
        private void SaveSimResult(string Extension)
        {
            if (sim == null)
            {
                MessageBox.Show("No Simulation present!", "Data Empty", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //copied from 
            PlotModel pm = new PlotModel()
            {
                IsLegendVisible = false,
                LegendPosition = LegendPosition.RightTop,
                DefaultFontSize = Properties.Settings.Default.defaultFontSize,
                LegendFontSize = Properties.Settings.Default.defaultFontSize,
                DefaultFont = Properties.Settings.Default.defaultFont,
                PlotAreaBorderThickness = new OxyThickness(Properties.Settings.Default.lineThickness)
            };

            OxyPlot.Axes.LinearAxis x = new OxyPlot.Axes.LinearAxis
            {
                Title = "",
                Unit = "%",
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Key = "X",
                IsAxisVisible = true,
                MajorGridlineThickness = Properties.Settings.Default.lineThickness,
                TitleFormatString = "{1}"
            };

            OxyPlot.Axes.CategoryAxis y = new OxyPlot.Axes.CategoryAxis
            {
                IsAxisVisible = false,
                Position = OxyPlot.Axes.AxisPosition.Left
            };

            if (!Properties.Settings.Default.showBox)
            {
                pm.PlotAreaBorderThickness = new OxyThickness(0);

                x.AxislineStyle = LineStyle.Solid;
                x.AxislineThickness = Properties.Settings.Default.lineThickness;
            }

            pm.Axes.Add(x);
            pm.Axes.Add(y);

            //add data;
            OxyPlot.Series.IntervalBarSeries s = new OxyPlot.Series.IntervalBarSeries();
            int a = 0;
            foreach (KeyValuePair<string, double> i in sim.par.Reverse())
            {
                OxyPlot.Series.IntervalBarItem item = new OxyPlot.Series.IntervalBarItem
                {
                    Start = (i.Value < 0? i.Value : 0),
                    End = (i.Value < 0 ? 0 : i.Value),
                    Title = i.Key,
                    CategoryIndex = a,
                    Color = OxyColor.Parse(Properties.Settings.Default.color2)
                    
                };
                s.Items.Add(item);
                a++;
            }
            pm.Series.Add(s);

            pm.Annotations.Add(new OxyPlot.Annotations.LineAnnotation()
            {
                Color = OxyColors.Black,
                StrokeThickness = Properties.Settings.Default.lineThickness,
                Type = OxyPlot.Annotations.LineAnnotationType.Vertical,
                X = 0.0
            });

            pm.InvalidatePlot(true);

            //save image
            if (Extension == "png")
                PngExporter.Export(pm, this.filename + "SimResult.png", Properties.Settings.Default.pngWidth, Properties.Settings.Default.pngHeight, OxyColors.Transparent, Properties.Settings.Default.pngRes);

            //exports svg
            if (Extension == "svg")
            {
                var svg = new OxyPlot.Wpf.SvgExporter()
                {
                    Width = Properties.Settings.Default.pngWidth,
                    Height = Properties.Settings.Default.pngHeight
                };
                svg.ExportToFile(pm, this.filename + "SimResult.svg");
            }

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

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
using System.Xml.Linq;
using winforms = System.Windows.Forms;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        public PlotModel Model { get; set; }
        public Macrocycle Cycle;
        public Simulation Sim;
        public string Filename = "";

        /// <summary>
        /// returns the default save directory
        /// </summary>
        public string DefaultPath
        {
            get
            {
                if (Properties.Settings.Default.useImportExportPath) return Properties.Settings.Default.importPath;
                else return Properties.Settings.Default.savePath;
            }
        }

        public SaveWindow(Macrocycle cycle, Simulation Sim = null)
        {
            InitializeComponent();
            this.Cycle = cycle;
            this.Sim = Sim;
            DataContext = this;
            NameTB.Text = cycle.Title;

            this.Model = Application.Current.Windows.OfType<MainWindow>().First().displaceView.Model;
            //reanalyze
            if (Application.Current.Windows.OfType<MainWindow>().First().normalize && !(Model == null || Model.Series.Count == 0))
            {
                Application.Current.Windows.OfType<MainWindow>().First().NormalizeButton_Click(null, null);
                Application.Current.Windows.OfType<MainWindow>().First().Analyze();
                //as its beeing recreated
                this.Model = Application.Current.Windows.OfType<MainWindow>().First().displaceView.Model;
            }

        }

        /// <summary>
        /// Handles file saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //validate form
            if (String.IsNullOrEmpty(PathTB.Text) || !Directory.Exists(PathTB.Text))
            {
                MessageBox.Show("The specified directory does not exist!", "I/O Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (TypeList.SelectedItems.Count == 0)
            {

                MessageBox.Show("No Datatype has been selected!", "Datatype Empty", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this.Filename = PathTB.Text + "/" + NameTB.Text + "_";
            List<FileType> types = new List<FileType>();

            //check if graph is present when report is present, otherwise add! 
            foreach (object o in TypeList.SelectedItems)
            {
                if (((FileType)o).Title == "Report")
                {
                    types.Add(new FileType() { Title = "Graph", Extension = "png" });
                    if (Sim != null) types.Add(new FileType() { Title = "SimResult", Extension = "png" });
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
                    case "Result":
                        SaveResult(t.Extension);
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
            //Application.Current.Windows.OfType<MainWindow>().First().Analyze();

            //check if data is present
            if (Model.Series.Where(s => s.IsVisible == true).Count() == 0)
            {
                MessageBox.Show("No Data present!", "Data Empty", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //get number of bonds.
            int bonds = 0;
            switch (Cycle.type)
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
            foreach (OxyPlot.Series.ScatterSeries s in Model.Series)
            {
                if (!s.IsVisible)
                {
                    int index = Model.Series.IndexOf(s);
                    for (int i = index * bonds; i < (index * bonds) + bonds; i++)
                    {
                        annotations.Add(Model.Annotations[i]);
                    }
                }
            }
            foreach (OxyPlot.Annotations.Annotation a in annotations)
            {
                Model.Annotations.Remove(a);
            }


            //exports png
            if (Extension == "png")
                PngExporter.Export(Model, this.Filename + "Analysis.png", Properties.Settings.Default.pngWidth, Properties.Settings.Default.pngHeight, OxyColors.Transparent, Properties.Settings.Default.pngRes);

            //exports svg
            if (Extension == "svg")
            {
                var svg = new OxyPlot.Wpf.SvgExporter()
                {
                    Width = Properties.Settings.Default.pngWidth,
                    Height = Properties.Settings.Default.pngHeight
                };
                svg.ExportToFile(Model, this.Filename + "Analysis.svg");
            }
        }

        /// <summary>
        /// Saves ascii file
        /// </summary>
        /// <param name="Extension"></param>
        private void SaveASCII(string Extension)
        {
            List<OxyPlot.Series.ScatterSeries> export = new List<OxyPlot.Series.ScatterSeries>();
            foreach (OxyPlot.Series.ScatterSeries s in Model.Series)
            {
                export.Add(s);
            }

            if (export.Count == 0)
            {
                MessageBox.Show("No ASCII Data present!", "Data Empty", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //make title string
            string title = "A;X;";
            foreach (OxyPlot.Series.Series s in export) title += s.Title + ";";

            //write data
            using (StreamWriter sw = new StreamWriter(this.Filename + "Data." + Extension))
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
        /// saves Molecule/Macrocycle
        /// </summary>
        /// <param name="Extension"></param>
        /// <param name="CycleOnly"></param>
        private void SaveMolecule(string Extension, bool CycleOnly = false)
        {
            //no validation since a molecule is ALWAYS present!.

            string Filename = this.Filename + (CycleOnly ? "Macrocycle" : "Molecule");
            if (Extension == "ixyz")
            {
                Filename += ".ixyz";

                using (StreamWriter sw = new StreamWriter(Filename))
                {
                    if (!CycleOnly) sw.WriteLine(Cycle.Atoms.Count);
                    else
                        sw.WriteLine(Cycle.Atoms.Where(s => s.IsMacrocycle).ToList().Count);

                    sw.WriteLine(Filename);

                    foreach (Atom a in Cycle.Atoms)
                    {
                        if (CycleOnly && a.IsMacrocycle)
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
            string Filename = this.Filename + "Report." + Extension;
            Report r = SaveReport();
            if (Extension == "docx")
            {
                using (var w = new WordDocumentReportWriter(Filename))
                {
                    w.WriteReport(r, GetReportStyle());
                    w.Save();
                }
            }
            if (Extension == "pdf")
            {
                using (var w = new PdfReportWriter(Filename))
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
            report.AddHeader(1, "Conformational analysis " + NameTB.Text);
            report.Add(main);
            string type = Cycle.type.ToString();

            //basic information
            main.AddHeader(2, "Macrocyclic Conformation");
            main.AddParagraph("The following figure shows the conformational analysis of " + (!String.IsNullOrEmpty(Cycle.Title) ? Cycle.Title : "the input macrocycle ")
                    + "displayed as displacement diagram. A middle plane was places through the " + type.ToLower() + "'s ring atoms. "
                    + "The distance of the ring atoms to the mean plane is plotted against calculated circle coordinates.");


            main.AddImage(this.Filename + "Analysis.png", "Displacement Diagram of the " + type.ToLower());

            //get exp series
            OxyPlot.Series.ScatterSeries exp = (OxyPlot.Series.ScatterSeries)Model.Series.FirstOrDefault(s => s.Title == "Exp.");
            main.AddItemsTable("Experimental Data",
                exp.ItemsSource.OfType<AtomDataPoint>(),
                new List<ItemsTableField> { new ItemsTableField("X", "X") { Width = 1000 }, new ItemsTableField("Y", "Y") { Width = 1000 }
                });

            if (this.Sim != null)
            {
                ReportSection Simu = new ReportSection();
                report.Add(Simu);
                Simu.AddHeader(2, "Simulation Details");
                Simu.AddParagraph("A Simulation has been done using the least squares methode. The conformation of the "
                    + type.ToLower() + " was traced back to the vibration normal modes of metallo" + type.ToLower() + "s. "
                    + "The standard vibration modes were obtained by DFT analysis of a metallo" + type.ToLower() + " at "
                    + "B3LYP/Def2-SVP level of theory. The Simulated conformation was calculated with an error of " + Sim.errors[0]
                    + " as root of the sum of the squared errors per atom. For the derivates the error is " + Sim.errors[1]
                    + " and for the integrals the following error was measured " + Sim.errors[2]);

                string composition = "";
                string absComposition = "";
                foreach (KeyValuePair<string, double> i in Sim.par)
                {
                    composition += i.Value.ToString("N2", System.Globalization.CultureInfo.InvariantCulture) + "% of " + i.Key + ",";
                    absComposition += (i.Value / 100 * Sim.MeanDisplacement()).ToString("N4", System.Globalization.CultureInfo.InvariantCulture) + " of " + i.Key + ",";
                }
                composition.Remove(composition.LastIndexOf(','));
                Simu.AddParagraph("The final composition of normal modes is " + composition + " as also listed in the table below. "
                    + "The mean displacement parameter is " + Sim.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture)
                    + " The absolute composition therefore is " + absComposition + ".");

                Simu.AddImage(this.Filename + "SimResult.png", "Visualization of Simulationparameters");
                Simu.AddPropertyTable("Simulationparameters with a mean displacement parameter of " + Sim.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture), Sim.par);
            }
            return report;
        }

        /// <summary>
        /// Saves the Sim result as plot
        /// </summary>
        /// <param name="extension"></param>
        private void SaveSimResult(string Extension)
        {
            if (Sim == null)
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
            foreach (KeyValuePair<string, double> i in Sim.par.Reverse())
            {
                OxyPlot.Series.IntervalBarItem item = new OxyPlot.Series.IntervalBarItem
                {
                    Start = (i.Value < 0 ? i.Value : 0),
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
                PngExporter.Export(pm, this.Filename + "SimResult.png", Properties.Settings.Default.pngWidth, Properties.Settings.Default.pngHeight, OxyColors.Transparent, Properties.Settings.Default.pngRes);

            //exports svg
            if (Extension == "svg")
            {
                var svg = new OxyPlot.Wpf.SvgExporter()
                {
                    Width = Properties.Settings.Default.pngWidth,
                    Height = Properties.Settings.Default.pngHeight
                };
                svg.ExportToFile(pm, this.Filename + "SimResult.svg");
            }

        }

        /// <summary>
        /// Save XML Result
        /// </summary>
        /// <param name="Extension"></param>
        private void SaveResult(string Extension)
        {
            List<XElement> simRes = Sim.par.Select(par => new XElement("parameter", new XAttribute("name", par.Key), par.Value)).ToList();
            List<XElement> metrix = Cycle.Metrics().Select(met => new XElement("metric", new XAttribute("name", met.Key), met.Value)).ToList();

            simRes.Add(new XElement("doop", Sim.MeanDisplacement()));
            simRes.Add(new XElement("errors",
                new XElement("data", Sim.errors[0]),
                new XElement("derivative", Sim.errors[1]),
                new XElement("integral", Sim.errors[2])
                ));

            XElement Molecule = new XElement("molecule",
                new XAttribute("name", NameTB.Text),
                new XElement("type", Cycle.type.ToString()),
                new XElement("simulation", simRes),
                new XElement("metrics", metrix)
                );
            Molecule.Save(File.Create(Filename + "Result." + Extension));
        }

        /// <summary>
        /// handles search button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!String.IsNullOrEmpty(Properties.Settings.Default.savePath) && !Properties.Settings.Default.useImportExportPath)
                initialDir = Properties.Settings.Default.savePath;
            else if (!String.IsNullOrEmpty(Properties.Settings.Default.importPath))
                initialDir = Properties.Settings.Default.importPath;
            using (winforms.FolderBrowserDialog fbd = new winforms.FolderBrowserDialog
            {
                SelectedPath = initialDir
            })
            {
                if (fbd.ShowDialog() == winforms.DialogResult.OK)
                {
                    PathTB.Text = fbd.SelectedPath;
                }
            }
        }

        public bool HasData(string data)
        {
            if ((data == "Graph" || data == "ASCII" || data == "Report") && (Model == null || Model.Series.Count == 0)) return false;
            if ((data == "SimResult" || data == "Result") && Sim == null) return false;
            return true;
        }
    }
    public class FileType
    {
        public string Title { get; set; }
        public PackIcon Icon { get; set; }
        public PackIcon Secondary { get; set; }
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

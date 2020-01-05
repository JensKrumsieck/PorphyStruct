using MaterialDesignThemes.Wpf;
using OxyPlot;
using OxyPlot.OpenXml;
using OxyPlot.Pdf;
using OxyPlot.Reporting;
using OxyPlot.Wpf;
using PorphyStruct.Chemistry;
using PorphyStruct.Files;
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
                this.Model = Application.Current.Windows.OfType<MainWindow>().First().displaceView.Model;
            }

        }

        /// <summary>
        /// Validates Path and Types
        /// </summary>
        public bool Validate
        {
            get
            {
                //validate form
                if (String.IsNullOrEmpty(PathTB.Text) || !Directory.Exists(PathTB.Text)) { MessageBox.Show("The specified directory does not exist!", "I/O Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
                if (TypeList.SelectedItems.Count == 0) { MessageBox.Show("No Datatype has been selected!", "Datatype Empty", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
                return true;
            }
        }

        /// <summary>
        /// Handles file saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate) return; 
            this.Filename = PathTB.Text + "/" + NameTB.Text + "_";
            List<ExportFileType> types = new List<ExportFileType>();

            //check if graph is present when report is present, otherwise add! 
            foreach (object o in TypeList.SelectedItems)
            {
                types.Add((ExportFileType)o);
                if (((ExportFileType)o).Title == "Report")
                {
                    types.Add(new ExportFileType() { Title = "Graph", Extension = "png" });
                    if (Sim != null) types.Add(new ExportFileType() { Title = "SimResult", Extension = "png" });
                }
            }

            foreach (ExportFileType t in types)
            {
                switch (t.Title)
                {
                    case "Graph":
                        Model.SaveGraph(Cycle, Filename, t.Extension);
                        break;
                    case "ASCII":
                        Model.SaveASCII(Filename);
                        break;
                    case "SimResult":
                        SaveSimResult(t.Extension);
                        break;
                    case "Macrocycle":
                        Cycle.SaveIXYZ(Filename, true);
                        break;
                    case "Molecule":
                        MacrocycleExporter.SaveIXYZ(Cycle, Filename);
                        break;
                    case "Report":
                        SaveReport(t.Extension);
                        break;
                    case "Result":
                        Sim.SaveResult(Cycle, Filename);    
                        break;
                }
            }
            Close();
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
                    w.WriteReport(r, MacrocycleExporter.ReportStyle);
                    w.Save();
                }
            }
            if (Extension == "pdf")
            {
                using (var w = new PdfReportWriter(Filename))
                {
                    w.WriteReport(r, MacrocycleExporter.ReportStyle);
                }

            }
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
                    absComposition += (i.Value / 100 * Sim.cycle.MeanDisplacement()).ToString("N4", System.Globalization.CultureInfo.InvariantCulture) + " of " + i.Key + ",";
                }
                composition.Remove(composition.LastIndexOf(','));
                Simu.AddParagraph("The final composition of normal modes is " + composition + " as also listed in the table below. "
                    + "The mean displacement parameter is " + Sim.cycle.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture)
                    + " The absolute composition therefore is " + absComposition + ".");

                Simu.AddImage(this.Filename + "SimResult.png", "Visualization of Simulationparameters");
                Simu.AddPropertyTable("Simulationparameters with a mean displacement parameter of " + Sim.cycle.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture), Sim.par);
            }
            return report;
        }

        /// <summary>
        /// Saves the Sim result as plot
        /// leave this here until gui is capable of showing this in the first place
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

        /// <summary>
        /// checks the fields for data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool HasData(string data)
        {
            if ((data == "Graph" || data == "ASCII" || data == "Report") && (Model == null || Model.Series.Count == 0)) return false;
            if ((data == "SimResult" || data == "Result") && Sim == null) return false;
            return true;
        }
    }
}

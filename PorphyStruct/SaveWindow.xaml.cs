using OxyPlot;
using OxyPlot.Reporting;
using OxyPlot.Wpf;
using PorphyStruct.Chemistry;
using PorphyStruct.Windows;
using System;
using System.Collections.Generic;
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
                if (PorphyStruct.Core.Properties.Settings.Default.useImportExportPath) return PorphyStruct.Core.Properties.Settings.Default.importPath;
                else return PorphyStruct.Core.Properties.Settings.Default.savePath;
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
        public bool Validate()
        {
            //validate form
            if (String.IsNullOrEmpty(PathTB.Text) || !Directory.Exists(PathTB.Text)) { MessageBox.Show("The specified directory does not exist!", "I/O Error", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            if (TypeList.SelectedItems.Count == 0) { MessageBox.Show("No Datatype has been selected!", "Datatype Empty", MessageBoxButton.OK, MessageBoxImage.Error); return false; }
            return true;
        }

        /// <summary>
        /// Handles file saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;
            Filename = PathTB.Text + "/" + NameTB.Text + "_";
            foreach (ExportFileType t in TypeList.SelectedItems)
            {
                switch (t.Title)
                {
                    case "Graph": //Use WPF Exporters here as the quality is better (at this time) than the OxyPlot.Core.Drawing implementation... So Exporting is still Windows dependent.
                        if (t.Extension == "png") Model.ExportGraph(Cycle, Filename, new PngExporter() { Height = Core.Properties.Settings.Default.pngHeight, Width = Core.Properties.Settings.Default.pngWidth, Resolution = Core.Properties.Settings.Default.pngRes, Background = OxyColors.Transparent }, t.Extension);
                        if (t.Extension == "svg") Model.ExportGraph(Cycle, Filename, new OxyPlot.Wpf.SvgExporter() { Height = Core.Properties.Settings.Default.pngHeight, Width = Core.Properties.Settings.Default.pngWidth }, t.Extension);
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
                        Cycle.SaveIXYZ(Filename);
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
            /** string Filename = this.Filename + "Report." + Extension;
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

             }**/
        }


        /// <summary>
        /// Gets Report
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private Report SaveReport()
        {
            string type = Application.Current.Windows.OfType<MainWindow>().First().type.ToString().ToLower();
            Report report = new Report() { Title = "Analysis" };
            ReportSection main = new ReportSection();
            report.AddHeader(1, "Conformational analysis " + NameTB.Text);
            report.Add(main);

            //basic information
            main.AddHeader(2, "Macrocyclic Conformation");
            main.AddParagraph(string.Format(Properties.Resources.ExplanationParagraph, Cycle.Title, type));
            main.AddImage(Filename + "Analysis.png", $"Displacement Diagram of the {type}");

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
                Simu.AddParagraph(string.Format(Properties.Resources.SimDetailsParagraph, type, Sim.errors[0], Sim.errors[1], Sim.errors[2]));

                string composition = "";
                string absComposition = "";
                foreach (KeyValuePair<string, double> i in Sim.par)
                {
                    composition += i.Value.ToString("N2", System.Globalization.CultureInfo.InvariantCulture) + "% of " + i.Key + ",";
                    absComposition += (i.Value / 100 * Sim.cycle.MeanDisplacement()).ToString("N4", System.Globalization.CultureInfo.InvariantCulture) + " of " + i.Key + ",";
                }
                composition.Remove(composition.LastIndexOf(','));
                Simu.AddParagraph(string.Format(Properties.Resources.CompositionParagraph, composition, Sim.cycle.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture), absComposition));

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
                DefaultFontSize = PorphyStruct.Core.Properties.Settings.Default.defaultFontSize,
                LegendFontSize = PorphyStruct.Core.Properties.Settings.Default.defaultFontSize,
                DefaultFont = PorphyStruct.Core.Properties.Settings.Default.defaultFont,
                PlotAreaBorderThickness = new OxyThickness(PorphyStruct.Core.Properties.Settings.Default.lineThickness)
            };

            OxyPlot.Axes.LinearAxis x = new OxyPlot.Axes.LinearAxis
            {
                Title = "",
                Unit = "%",
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Key = "X",
                IsAxisVisible = true,
                MajorGridlineThickness = PorphyStruct.Core.Properties.Settings.Default.lineThickness,
                TitleFormatString = "{1}"
            };

            OxyPlot.Axes.CategoryAxis y = new OxyPlot.Axes.CategoryAxis
            {
                IsAxisVisible = false,
                Position = OxyPlot.Axes.AxisPosition.Left
            };

            if (!PorphyStruct.Core.Properties.Settings.Default.showBox)
            {
                pm.PlotAreaBorderThickness = new OxyThickness(0);

                x.AxislineStyle = LineStyle.Solid;
                x.AxislineThickness = PorphyStruct.Core.Properties.Settings.Default.lineThickness;
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
                    Color = OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color2)
                };
                s.Items.Add(item);
                a++;
            }
            pm.Series.Add(s);

            pm.Annotations.Add(new OxyPlot.Annotations.LineAnnotation()
            {
                Color = OxyColors.Black,
                StrokeThickness = PorphyStruct.Core.Properties.Settings.Default.lineThickness,
                Type = OxyPlot.Annotations.LineAnnotationType.Vertical,
                X = 0.0
            });

            pm.InvalidatePlot(true);

            //save image
            if (Extension == "png")
                PngExporter.Export(pm, this.Filename + "SimResult.png", PorphyStruct.Core.Properties.Settings.Default.pngWidth, PorphyStruct.Core.Properties.Settings.Default.pngHeight, OxyColors.Transparent, PorphyStruct.Core.Properties.Settings.Default.pngRes);

            //exports svg
            if (Extension == "svg")
            {
                var svg = new OxyPlot.Wpf.SvgExporter()
                {
                    Width = PorphyStruct.Core.Properties.Settings.Default.pngWidth,
                    Height = PorphyStruct.Core.Properties.Settings.Default.pngHeight
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
            if (!String.IsNullOrEmpty(PorphyStruct.Core.Properties.Settings.Default.savePath) && !PorphyStruct.Core.Properties.Settings.Default.useImportExportPath)
                initialDir = PorphyStruct.Core.Properties.Settings.Default.savePath;
            else if (!String.IsNullOrEmpty(PorphyStruct.Core.Properties.Settings.Default.importPath))
                initialDir = PorphyStruct.Core.Properties.Settings.Default.importPath;
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

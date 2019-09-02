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
using System.Windows;
using winforms = System.Windows.Forms;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        public enum Format { png, svg, dat, pdf, docx, MiXYZ, iXYZ };
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
        }

        /// <summary>
        /// Handle Search Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_Click(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Handle Save Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.filename = PathTB.Text + @"\" + Title_TB.Text + cycle.Title + "_PorphyStruct_";
            HandleSave((Format)SaveFormat_CB.SelectedIndex);
            if (CloseCB.IsChecked == true) this.Close();
        }

        /// <summary>
        /// Handle Save -> Format Switch
        /// </summary>
        /// <param name="f"></param>
        private void HandleSave(Format f)
        {
            switch (f)
            {
                case Format.png:
                    HandleSavePNG();
                    break;
                case Format.dat:
                    HandleSaveDat();
                    break;
                case Format.iXYZ:
                    HandleSaveXYZ(false);
                    break;
                case Format.MiXYZ:
                    HandleSaveXYZ(true);
                    break;
                case Format.svg:
                    HandleSaveSVG();
                    break;
                case Format.pdf:
                    HandleSavePDF();
                    break;
                case Format.docx:
                    HandleSaveDocx();
                    break;
            }
        }

        /// <summary>
        /// Export PNG
        /// </summary>
        private void HandleSavePNG()
        {
            PngExporter.Export(model, this.filename + "Analysis.png", Properties.Settings.Default.pngWidth, Properties.Settings.Default.pngHeight, OxyColors.Transparent, Properties.Settings.Default.pngRes);
        }

        /// <summary>
        /// Export SVG
        /// </summary>
        private void HandleSaveSVG()
        {
            var svg = new OxyPlot.Wpf.SvgExporter()
            {
                Width = Properties.Settings.Default.pngWidth,
                Height = Properties.Settings.Default.pngHeight
            };
            svg.ExportToFile(model, this.filename + "Analysis.svg");

        }

        /// <summary>
        /// Export PDF
        /// </summary>
        private void HandleSavePDF()
        {
            string filename = this.filename + "Analysis.pdf";
            Report r = SaveReport();
            using (var w = new PdfReportWriter(filename))
            {
                w.WriteReport(r, GetReportStyle());
            }
        }

        /// <summary>
        /// Export Docx
        /// </summary>
        private void HandleSaveDocx()
        {
            string filename = this.filename + "Analysis.docx";
            Report r = SaveReport();
            using (var w = new WordDocumentReportWriter(filename))
            {
                w.WriteReport(r, GetReportStyle());
                w.Save();
            }
        }

        /// <summary>
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
            report.AddHeader(1, (lang == "de" ? "Konformationsanalyse " : "Conformational analysis ") + Title_TB.Text + " - " + cycle.Title);
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
                                            + Title_TB.Text +
                                            " als Auslenkungsdiagramm dargestellt. Durch die Ringatome des " + type + "s wurde eine mittlere"
                                            + "Ebene gelegt. Der Abstand der Ringatome zur Ebene wird gegen berechnete Kreiskoordinaten aufgetragen."
                                        : "The following figure shows the conformational analysis of "
                                            + Title_TB.Text +
                                            " displayed as a displacement diagram. A middle plane was placed through the ring atoms of  " + type.ToLower() + "." +
                                            " The distance of the ring atoms to the plane is plotted against calculated circle coordinates."));
            //export as image first
            PngExporter.Export(model, this.filename + "Analysis.png", Properties.Settings.Default.pngWidth, Properties.Settings.Default.pngHeight, OxyColors.Transparent, Properties.Settings.Default.pngRes);
            main.AddImage(this.filename + "Analysis.png", (lang == "de" ? "Auslenkungsdiagramm der Konformationsanalyse von " + Title_TB.Text : "Displacement diagram of " + Title_TB.Text));
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
            //// get 3d image //
            ////get mainwindow instance
            //MainWindow mw = Application.Current.Windows.OfType<MainWindow>().First();
            ////center molecule
            //mw.Center();
            //BitmapSource bitmap = Helix.Override.Viewport3DHelper.RenderBitmap(mw.MolViewer.Viewport, Properties.Settings.Default.pngWidth, Properties.Settings.Default.pngWidth, Brushes.Transparent);

            //using (var s = new FileStream(this.filename + "Viewport.png", FileMode.Create))
            //{
            //	BitmapEncoder encode = new PngBitmapEncoder();
            //	encode.Frames.Add(BitmapFrame.Create(bitmap));
            //	encode.Save(s);
            //}
            //ReportSection r3D = new ReportSection();
            //report.Add(r3D);
            //r3D.AddHeader(2, "Bond Pathway");
            //r3D.AddParagraph((lang == "de" ? "In nachfolgender Abbildung ist die 3D Repräsentation des Moleküls mit Hervorhebung der für die Analyse verwendeten Atome und Bindungen zu sehen."
            //		: "The following figure shows the 3D representation of the molecule with emphasis on the atoms and bonds used for the analysis."));
            //r3D.AddImage(this.filename + "Viewport.png", (lang == "de" ? "Für die Analyse herangezogene Bindungen." : "Bonds used in present analysis."));

            return report;
        }

        /// <summary>
        /// Export DAT
        /// </summary>
        private void HandleSaveDat()
        {
            using (StreamWriter sw = new StreamWriter(this.filename + "Analysis.dat"))
            {
                sw.WriteLine("X;Y");
                //combine series
                List<AtomDataPoint> data = new List<AtomDataPoint>();
                OxyPlot.Series.ScatterSeries series = (OxyPlot.Series.ScatterSeries)model.Series[0];
                data.AddRange((List<AtomDataPoint>)series.ItemsSource);

                for (int i = 0; i < data.Count; i++)
                {
                    sw.WriteLine(data[i].X.ToString() + ";" + data[i].Y.ToString());
                }
            }
        }

        /// <summary>
        /// Export XYZ
        /// </summary>
        /// <param name="cycleOnly">Only Macrocyclic Atoms</param>
        private void HandleSaveXYZ(bool cycleOnly = false)
        {
            using (StreamWriter sw = new StreamWriter(this.filename + "_Analysis.ixyz"))
            {
                if (!cycleOnly) sw.WriteLine(cycle.Atoms.Count);
                else
                    sw.WriteLine(cycle.Atoms.Where(s => s.isMacrocycle).ToList().Count);

                sw.WriteLine(Title_TB.Text + cycle.Title + "_PorphyStruct_Analysis");

                foreach (Atom a in cycle.Atoms)
                {
                    if (cycleOnly && a.isMacrocycle)
                    {
                        sw.WriteLine(a.Identifier + "/" + a.Type + "\t" + a.X.ToString("N8", System.Globalization.CultureInfo.InvariantCulture) + "\t" + a.Y.ToString("N8", System.Globalization.CultureInfo.InvariantCulture) + "\t" + a.Z.ToString("N8", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (!cycleOnly)
                    {
                        sw.WriteLine(a.Identifier + "/" + a.Type + "\t" + a.X.ToString("N8", System.Globalization.CultureInfo.InvariantCulture) + "\t" + a.Y.ToString("N8", System.Globalization.CultureInfo.InvariantCulture) + "\t" + a.Z.ToString("N8", System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
            }
        }
    }
}

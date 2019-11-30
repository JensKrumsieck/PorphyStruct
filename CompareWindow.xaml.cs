using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using PorphyStruct.Chemistry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für CompareWindow.xaml
    /// </summary>
    public partial class CompareWindow : Window
    {
        private bool loaded = false;
        public CompareWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles Button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            //get clicked button number
            int.TryParse((sender as Button).Tag.ToString(), out int comp);

            //use save dir as default because there should be the results
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!String.IsNullOrEmpty(Properties.Settings.Default.savePath) && !Properties.Settings.Default.useImportExportPath)
                initialDir = Properties.Settings.Default.savePath;
            else if (!String.IsNullOrEmpty(Properties.Settings.Default.importPath))
                initialDir = Properties.Settings.Default.importPath;
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = "ASCII Files (DAT) (*.dat)|*.dat",
                RestoreDirectory = true
            };
            var DialogResult = ofd.ShowDialog();

            if (DialogResult.HasValue && DialogResult.Value)
            {
                (FindName($"comparison{comp}Path") as TextBox).Text = ofd.FileName;
            }
        }

        /// <summary>
        /// Gets Data from dat file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Simulation GetData(string path)
        {
            MainWindow mw = Application.Current.Windows.OfType<MainWindow>().First();
            Macrocycle cycle = mw.GetCycle();
            List<AtomDataPoint> mol = new List<AtomDataPoint>();
            string file = File.ReadAllText(path);
            string[] lines = file.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None);

            double[] dataX = new double[lines.Length - 1]; // first line is bs
            double[] dataY = new double[lines.Length - 1]; // first line is bs
            string[] dataA = new string[lines.Length - 1]; // first line is bs
            int index = 0;
            foreach (string line in lines)
            {
                if (!String.IsNullOrEmpty(line) && !line.StartsWith("A;"))
                {
                    dataA[index] = line.Split(';')[0]; //atom identifier
                    dataX[index] = Convert.ToDouble(line.Split(';')[1]);
                    dataY[index] = Convert.ToDouble(line.Split(';')[2]);
                    index++;
                }
            }
            //remove 0;
            dataX = dataX.Where(s => s != 0).ToArray();
            //protect against y = 0
            if (dataY.Where(s => s != 0).ToArray().Length == dataX.Length)
                dataY = dataY.Where(s => s != 0).ToArray();
            else dataY = dataY.Where(s => s != dataY.Last()).ToArray(); //remove last because it's may a newline at the end

            Array.Sort(dataX.ToArray(), dataY);
            Array.Sort(dataX, dataA);
            List<Atom> atoms = new List<Atom>();
            for (int i = 0; i < dataX.Length; i++)
            {
                //add datapoint with dummy atom only having identifier
                Atom A = new Atom(dataA[i], 0, 0, 0)
                {
                    IsMacrocycle = true
                };
                atoms.Add(A);
                mol.Add(new AtomDataPoint(dataX[i], dataY[i], A));
            }
            Simulation tmpCycle = new Simulation(atoms)
            {
                dataPoints = mol,
                type = cycle.type
            };
            return tmpCycle;
        }

        /// <summary>
        /// Handle Text Change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComparisonPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (loaded)
            {
                TextBox tb = (TextBox)sender;
                if (tb.Name.Contains("1"))
                    comp1Plot.Model.Axes.Add(GetData(tb.Text).BuildColorAxis());
                else
                    comp2Plot.Model.Axes.Add(GetData(tb.Text).BuildColorAxis());

                GetData(tb.Text).Paint(tb.Name.Contains("1") ? comp1Plot.Model : comp2Plot.Model, tb.Name.Contains("1") ? "Com.1" : "Com.2");

                comp1Plot.InvalidatePlot();
                comp2Plot.InvalidatePlot();
            }
        }

        /// <summary>
        /// Ok Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Cancel Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// handle loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comp1Plot.Model = Pm;
            comp2Plot.Model = Pm;

            loaded = true;
            if (!String.IsNullOrEmpty(comparison1Path.Text))
                ComparisonPath_TextChanged(comparison1Path, null);
            if (!String.IsNullOrEmpty(comparison2Path.Text))
                ComparisonPath_TextChanged(comparison2Path, null);
        }

        /// <summary>
        /// y axis
        /// </summary>
        /// <returns></returns>
        private PorphyStruct.Oxy.Override.LinearAxis Y_
        {
           get{ 
               PorphyStruct.Oxy.Override.LinearAxis y = new PorphyStruct.Oxy.Override.LinearAxis
               {
                   Title = "Δ_{msp}",
                   Unit = "Å",
                   Position = AxisPosition.Left,
                   Key = "Y",
                   IsAxisVisible = true,
                   MajorGridlineThickness = Properties.Settings.Default.lineThickness,
                   TitleFormatString = Properties.Settings.Default.titleFormat,
                   LabelFormatter = Oxy.Override.OxyUtils._axisFormatter
               };
                return y;
             }
        }

        /// <summary>
        /// x axis
        /// </summary>
        /// <returns></returns>
        private LinearAxis X_
        {
            get
            {
                LinearAxis x = new LinearAxis
                {
                    Title = "X",
                    Unit = "Å",
                    Position = AxisPosition.Bottom,
                    Key = "X",
                    IsAxisVisible = Properties.Settings.Default.xAxis,
                    MajorGridlineThickness = Properties.Settings.Default.lineThickness,
                    AbsoluteMinimum = Properties.Settings.Default.minX,
                    AbsoluteMaximum = Properties.Settings.Default.maxX,
                    TitleFormatString = Properties.Settings.Default.titleFormat,
                    LabelFormatter = Oxy.Override.OxyUtils._axisFormatter
                };
                return x;
            }
        }

        /// <summary>
        /// gets PlotModel
        /// </summary>
        private PlotModel Pm
        {
            get
            {
                PlotModel pm = new PlotModel()
                {
                    IsLegendVisible = false,
                    DefaultFontSize = Properties.Settings.Default.defaultFontSize,
                    LegendFontSize = Properties.Settings.Default.defaultFontSize,
                    DefaultFont = Properties.Settings.Default.defaultFont,
                    PlotAreaBorderThickness = new OxyThickness(Properties.Settings.Default.lineThickness)
                };
                pm.Axes.Add(X_);
                pm.Axes.Add(Y_);
                return pm;
            }
        }
    }
}

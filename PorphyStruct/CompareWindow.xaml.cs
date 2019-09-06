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
        /// Handle Open1 Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open1Btn_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!String.IsNullOrEmpty(Properties.Settings.Default.savePath))
                initialDir = Properties.Settings.Default.savePath;
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = "ASCII Files (DAT) (*.dat)|*.dat",
                RestoreDirectory = true
            };
            var DialogResult = ofd.ShowDialog();

            if (DialogResult.HasValue && DialogResult.Value)
            {
                comparison1Path.Text = ofd.FileName;
            }
        }

        /// <summary>
        /// Handle Open2 Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open2Btn_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!String.IsNullOrEmpty(Properties.Settings.Default.savePath))
                initialDir = Properties.Settings.Default.savePath;
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = "ASCII Files (DAT) (*.dat)|*.dat",
                RestoreDirectory = true
            };
            var DialogResult = ofd.ShowDialog();

            if (DialogResult.HasValue && DialogResult.Value)
            {
                comparison2Path.Text = ofd.FileName;
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
            int index = 0;
            foreach (string line in lines)
            {
                if (!String.IsNullOrEmpty(line) && !line.StartsWith("X;"))
                {
                    dataX[index] = Convert.ToDouble(line.Split(';')[0]);
                    dataY[index] = Convert.ToDouble(line.Split(';')[1]);
                    index++;
                }
            }
            //remove 0;
            dataX = dataX.Where(s => s != 0).ToArray();
            //protect against y = 0
            if (dataY.Where(s => s != 0).ToArray().Length == dataX.Length)
                dataY = dataY.Where(s => s != 0).ToArray();
            else dataY = dataY.Where(s => s != dataY.Last()).ToArray(); //remove last because it's may a newline at the end

            Array.Sort(dataX, dataY);
            for(int i = 0; i < dataX.Length; i++)
            {
                mol.Add(new AtomDataPoint(dataX[i], dataY[i], cycle.dataPoints.OrderBy(s => s.X).ToList()[i].atom));
            }
            Simulation tmpCycle = new Simulation(cycle.Atoms)
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
            PlotModel pm1 = new PlotModel()
            {
                IsLegendVisible = false,
                DefaultFontSize = Properties.Settings.Default.defaultFontSize,
                LegendFontSize = Properties.Settings.Default.defaultFontSize,
                DefaultFont = Properties.Settings.Default.defaultFont,
                PlotAreaBorderThickness = new OxyThickness(Properties.Settings.Default.lineThickness)
            };
            PlotModel pm2 = new PlotModel()
            {
                IsLegendVisible = false,
                DefaultFontSize = Properties.Settings.Default.defaultFontSize,
                LegendFontSize = Properties.Settings.Default.defaultFontSize,
                DefaultFont = Properties.Settings.Default.defaultFont,
                PlotAreaBorderThickness = new OxyThickness(Properties.Settings.Default.lineThickness)
            };

            comp1Plot.Model = pm1;
            comp2Plot.Model = pm2;

            pm1.Axes.Add(X_());
            pm1.Axes.Add(Y_());
            pm2.Axes.Add(X_());
            pm2.Axes.Add(Y_());

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
        private PorphyStruct.Oxy.Override.LinearAxis Y_()
        {
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

        /// <summary>
        /// x axis
        /// </summary>
        /// <returns></returns>
        private LinearAxis X_()
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
}

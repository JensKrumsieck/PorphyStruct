using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using PorphyStruct.Chemistry;
using PorphyStruct.Simulations;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für SimWindow.xaml
    /// </summary>
    public partial class SimWindow : Window
    {
        /// <summary>
        /// Form Parameters
        /// </summary>
        private readonly SynchronizationContext synchronizationContext;
        private readonly OxyPlot.Wpf.PlotView parentView;
        private DateTime previousTime = DateTime.Now;

        /// <summary>
        /// Boolean Parameters
        /// </summary>
        private bool firstOnly, targetData, targetInt, targetDeriv, running; //setup booleans for target and running

        /// <summary>
        /// Simulation Parameters
        /// </summary>
        public Macrocycle cycle;
        public Macrocycle.Type MType = Application.Current.Windows.OfType<MainWindow>().First().type;
        public List<SimParam> param; //list of all simulation parameters
        private Simplex simplex = null; //simplex matrix

        //error array
        private double[] currentErr = new double[] { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity };

        //Simulation Mode Setter
        private enum SimulationMode { MonteCarlo, Simplex };
        private SimulationMode type = SimulationMode.MonteCarlo;

        /// <summary>
        /// Constructs the Window
        /// </summary>
        /// <param name="cycle">Macrocycle to simulate</param>
        /// <param name="pv">Plotview to Plot after Simulation</param>
        public SimWindow(Macrocycle cycle, OxyPlot.Wpf.PlotView pv)
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            this.cycle = cycle;

            //drop metal data
            if (cycle.HasMetal) this.cycle.dataPoints = this.cycle.dataPoints.Where(s => !s.atom.IsMetal).ToList();

            this.parentView = pv;

            simGrid.ItemsSource = this.param = SimParam.ListParameters(MType);
            PlotExp();
        }

        //if call comes with sim, use this param
        public SimWindow(Macrocycle cycle, OxyPlot.Wpf.PlotView pv, Simulation sim) : this(cycle, pv)
        {
            param = sim.simParam;
            simGrid.ItemsSource = this.param;
        }


        /// <summary>
        /// Plots the experimental data
        /// basically a copy from MainWindow
        /// <seealso cref="MainWindow.Analyze"/>
        /// </summary>
        private void PlotExp()
        {
            //plot that shit
            Oxy.Override.StandardPlotModel pm = new Oxy.Override.StandardPlotModel();
            MacrocyclePainter.Paint(pm, cycle, MacrocyclePainter.PaintMode.Exp);

            simView.Model = pm;
            pm.Scale(pm.xAxis);
            pm.InvalidatePlot(true);
        }


        /// <summary>
        /// Do the Simulation
        /// </summary>
        private void Simulate()
        {
            //update param
            this.param = (List<SimParam>)simGrid.ItemsSource;
            double[] coeff = param.Select(p => p.Start).ToArray();
            List<int> indices = new List<int>();
            this.simplex = new Simplex(Conformation.Calculate, coeff, cycle);
            //set start values
            for (int i = 0; i < param.Count; i++) if (!param[i].Optimize) indices.Add(i);

            while (running)
            {
                //get result
                Result result;
                if (firstOnly) result = Conformation.Calculate(cycle, coeff);
                else
                {
                    if (type == SimulationMode.MonteCarlo)
                    {
                        MonteCarlo mc = new MonteCarlo(Conformation.Calculate, coeff, cycle) { Indices = indices };
                        result = mc.Next();
                    }
                    else //simplex
                    {
                        simplex.Parameters = coeff;
                        simplex.Indices = indices;
                        result = simplex.Next();
                    }
                }

                for (int i = 0; i < result.Coefficients.Length; i++) param[i].Current = result.Coefficients[i];

                //plot current
                if ((DateTime.Now - previousTime).Milliseconds >= 500) SetNewCurrent(result);

                //get new best values if every single error is smaller or the overall sum is smaller
                if (IsNewBest(result)) SetNewBest(result);
                //if cb is set return...
                if (firstOnly) EndSimulation();
            }

        }

        /// <summary>
        /// Syncs the diagram for current and best
        /// </summary>
        internal void SynchronizeDiagram(List<AtomDataPoint> data, string target = "Current")
        {
            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                if (simView.Model.Series.Where(s => s.Title == target).Count() != 0) simView.Model.Series.Remove(simView.Model.Series.Where(s => s.Title == target).FirstOrDefault());
                simView.Model.Series.Add(new ScatterSeries() { ItemsSource = (List<AtomDataPoint>)o, Title = target, MarkerFill = target == "Current" ? OxyColors.PaleVioletRed : OxyColors.LawnGreen, MarkerType = Properties.Settings.Default.simMarkerType });
                simView.InvalidatePlot();
                simGrid.Items.Refresh();
            }), data);
        }

        /// <summary>
        /// Synchronize Error Textbox
        /// </summary>
        /// <param name="result"></param>
        internal void SynchronizeErrors(Result result)
        {
            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                double[] error = (double[])o;
                ErrTB.Text = string.Join(";", error.Select(s => s.ToString("N6", System.Globalization.CultureInfo.InvariantCulture)));
                simGrid.Items.Refresh();
            }), result.Error);
        }

        /// <summary>
        /// Synchronizes Mean Displacement Textbox
        /// </summary>
        /// <param name="data"></param>
        internal void SynchronizeMeanDisp(List<AtomDataPoint> data)
        {
            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                //update meadDisplacement BUT! Denormalize before!
                var tmp = MacrocycleFactory.Build(cycle.Atoms, MType);
                tmp.dataPoints = ((List<AtomDataPoint>)o).Factor(Application.Current.Windows.OfType<MainWindow>().First().normFac).ToList();
                meanDisPar.Content = tmp.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture);

            }), data);
        }

        /// <summary>
        /// Updates current
        /// </summary>
        /// <param name="result"></param>
        private void SetNewCurrent(Result result)
        {
            var currentConf = cycle.dataPoints.OrderBy(s => s.X).Select(s => new AtomDataPoint(s.X, result.Conformation[cycle.dataPoints.IndexOf(s)], s.atom)).ToList();
            SynchronizeDiagram(currentConf);
            previousTime = DateTime.Now;
        }

        /// <summary>
        /// Sets new BestValues
        /// </summary>
        /// <param name="result"></param>
        private void SetNewBest(Result result)
        {
            currentErr = result.Error;
            //new bestvalues
            for (int i = 0; i < result.Coefficients.ToArray().Count(); i++) param[i].Best = result.Coefficients.ToArray()[i];
            var bestConf = cycle.dataPoints.OrderBy(s => s.X).Select(s => new AtomDataPoint(s.X, result.Conformation[cycle.dataPoints.IndexOf(s)], s.atom)).ToList();
            SynchronizeDiagram(bestConf, "Best");
            SynchronizeMeanDisp(bestConf);
            SynchronizeErrors(result);
        }

        /// <summary>
        /// Checks Errors and returns a boolean whether the result is the new best result
        /// </summary>
        /// <param name="result">Result Object (for getting current errors)</param>
        /// <param name="ls">LeastSquares Object (for getting current minima)</param>
        /// <returns></returns>
        public bool IsNewBest(Result result)
        {
            //set error vars
            double error = result.Error[0];
            double derErr = result.Error[1];
            double intErr = result.Error[2];

            //check targets
            List<Tuple<double, double>> errorTargets = new List<Tuple<double, double>>();
            if (targetData) errorTargets.Add(new Tuple<double, double>(error, currentErr[0]));
            if (targetDeriv) errorTargets.Add(new Tuple<double, double>(derErr, currentErr[1]));
            if (targetInt) errorTargets.Add(new Tuple<double, double>(intErr, currentErr[2]));

            double targetSum = 0, lsSum = 0;
            bool[] isBest = new bool[errorTargets.Count];
            for (int i = 0; i < errorTargets.Count; i++)
            {
                targetSum += errorTargets[i].Item1;
                lsSum += errorTargets[i].Item2;
                if (errorTargets[i].Item1 < errorTargets[i].Item2) isBest[i] = true;
                else isBest[i] = false;
            }

            if (isBest.All(x => x)) return true;
            //now check the sum! if sum of targets is smaller -> return true
            //if not, do nothing
            if (targetSum < lsSum) return true;
            //return false as default
            return false;
        }

        /// <summary>
        /// Handle Start Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            //enable/disable gui elements while simulation
            running = stopBtn.IsEnabled = true;
            simGrid.IsEnabled = startBtn.IsEnabled = firstOnlyCB.IsEnabled = keepBestCB.IsEnabled = finishSimBtn.IsEnabled = fitDataCB.IsEnabled = fitIntCB.IsEnabled = fitDerivCB.IsEnabled = false;
            await Task.Run(() => this.Simulate());
        }

        /// <summary>
        /// Handle Simulation has ended
        /// </summary>
        private void EndSimulation()
        {
            //after running reenable/disable gui elements
            running = stopBtn.IsEnabled = false;
            startBtn.IsEnabled = firstOnlyCB.IsEnabled = keepBestCB.IsEnabled = simGrid.IsEnabled = finishSimBtn.IsEnabled = fitDataCB.IsEnabled = fitIntCB.IsEnabled = fitDerivCB.IsEnabled = true;
            // set best to start
            if (keepBestCB.IsChecked == true)
            {
                for (int i = 0; i < this.param.Count; i++) this.param[i].Start = this.param[i].Best;
                simGrid.Items.Refresh();
            }
            //clear simplex
            simplex = null;
        }

        /// <summary>
        /// Handle Stop Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopBtn_Click(object sender, RoutedEventArgs e) => EndSimulation();


        /// <summary>
        /// Handle Check&Uncheck of First Only Checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstOnlyCB_Checked(object sender, RoutedEventArgs e) => firstOnly = (firstOnlyCB.IsChecked == true ? true : false);

        /// <summary>
        /// Handle Selection Changed of Mode ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModeCB_SelectionChanged(object sender, SelectionChangedEventArgs e) => type = (SimulationMode)modeCB.SelectedIndex;

        /// <summary>
        /// Handle Finalize Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishSimBtn_Click(object sender, RoutedEventArgs e)
        {
            if (simView.Model.Series.Count >= 2)
            {
                //save exp series
                ScatterSeries exp = (ScatterSeries)parentView.Model.Series[0];

                ScatterSeries sim = (ScatterSeries)simView.Model.Series.FirstOrDefault(s => s.Title == "Best");
                Simulation simObj = new Simulation((Macrocycle)cycle.Clone()) { simParam = param };
                simObj.cycle.dataPoints = (List<AtomDataPoint>)sim.ItemsSource;
                //export param
                foreach (SimParam p in param) simObj.par.Add(p.Title, Math.Round(p.Best * 100, 2));
                //export errors
                simObj.errors = ErrTB.Text.Split(';').Select(s => Convert.ToDouble(s, System.Globalization.CultureInfo.InvariantCulture)).ToArray();

                //set true if exp has been inverted
                if (Application.Current.Windows.OfType<MainWindow>().First().invert) simObj.isInverted = true;
                Application.Current.Windows.OfType<MainWindow>().First().DelSimButton.IsEnabled = true;
                Application.Current.Windows.OfType<MainWindow>().First().DiffSimButton.IsEnabled = true;
                Application.Current.Windows.OfType<MainWindow>().First().simulation = simObj;
                Application.Current.Windows.OfType<MainWindow>().First().Analyze();
            }
        }

        /// <summary>
        /// Handle fit target checkboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FitDataCB_Checked(object sender, RoutedEventArgs e) => targetData = (fitDataCB.IsChecked == true ? true : false);
        private void FitDerivCB_Checked(object sender, RoutedEventArgs e) => targetDeriv = (fitDerivCB.IsChecked == true ? true : false);
        private void FitIntCB_Checked(object sender, RoutedEventArgs e) => targetInt = (fitIntCB.IsChecked == true ? true : false);

        /// <summary>
        /// Handle Window Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => running = false;

        /// <summary>
        /// Reloads Sim File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadSimBtn_Click(object sender, RoutedEventArgs e)
        {
            //use save dir as default because there should be the results
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!String.IsNullOrEmpty(Properties.Settings.Default.savePath) && !Properties.Settings.Default.useImportExportPath)
                initialDir = Properties.Settings.Default.savePath;
            else if (!String.IsNullOrEmpty(Properties.Settings.Default.importPath))
                initialDir = Properties.Settings.Default.importPath;
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = "Simulation File (*.xml)|*.xml",
                RestoreDirectory = true
            };
            bool? DialogResult = ofd.ShowDialog();

            if (DialogResult.HasValue && DialogResult.Value)
            {
                string file = File.ReadAllText(ofd.FileName);
                XmlDocument xmld = new XmlDocument();
                xmld.Load(new StringReader(file));
                var simul = xmld.SelectSingleNode("descendant::simulation");
                param = new List<SimParam>();
                foreach (XmlNode node in simul.SelectNodes("descendant::parameter"))
                {
                    param.Add(new SimParam(node.Attributes.GetNamedItem("name").InnerText, double.Parse(node.InnerText, System.Globalization.CultureInfo.InvariantCulture) / 100));
                }
                simGrid.ItemsSource = this.param;
            }
        }
    }
}

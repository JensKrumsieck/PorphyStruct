﻿using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PorphyStruct.Chemistry;
using PorphyStruct.Simulations;
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
        public bool running = false; //simulation is running
        private bool firstOnly, targetData, targetInt, targetDeriv; //setup booleans for target

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
            param = new List<SimParam>
            {
                new SimParam("Doming", 0),
                new SimParam("Ruffling", 0),
                new SimParam("Saddling", 0)
            };
            if (MType == Macrocycle.Type.Corrole || MType == Macrocycle.Type.Corrphycene || MType == Macrocycle.Type.Porphycene)
            {
                param.Add(new SimParam("Waving 2 (X)", 0));
                param.Add(new SimParam("Waving 2 (Y)", 0));
            }
            else if (MType == Macrocycle.Type.Porphyrin)
            {
                param.Add(new SimParam("Waving 1 (X)", 0));
                param.Add(new SimParam("Waving 1 (Y)", 0));
                param.Add(new SimParam("Waving 2 (X)", 0));
                param.Add(new SimParam("Waving 2 (Y)", 0));
            }
            else if (MType == Macrocycle.Type.Norcorrole)
            {
                param.Add(new SimParam("Waving 2 (Dipy)", 0));
                param.Add(new SimParam("Waving 2 (Bipy)", 0));
            }
            param.Add(new SimParam("Propellering", 0));

            simGrid.ItemsSource = this.param;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Ausstehend>")]
        private void Simulate()
        {
            this.param = (List<SimParam>)simGrid.ItemsSource;
            double[] coeff = new double[param.Count];
            List<int> indices = new List<int>();
            this.simplex = new Simplex(Conformation.Calculate, coeff, cycle);
            //set start values
            for (int i = 0; i < param.Count; i++)
            {
                coeff[i] = param[i].start;
                if (!param[i].optimize) indices.Add(i);
            }
            while (running)
            {
                //get result
                Result result = new Result(Conformation: Array.Empty<double>(), Coefficients: Array.Empty<double>(), Error: Array.Empty<double>());
                if (firstOnly) result = Conformation.Calculate(cycle, coeff);
                else
                {
                    if (type == SimulationMode.MonteCarlo)
                    {
                        MonteCarlo mc = new MonteCarlo(Conformation.Calculate, coeff, cycle)
                        {
                            Indices = indices
                        };
                        result = mc.Next();
                    }
                    else //simplex
                    {
                        simplex.Parameters = coeff;
                        simplex.Indices = indices;
                        result = simplex.Next();
                    }

                }

                //write current
                for (int i = 0; i < result.Coefficients.Length; i++)
                {
                    this.param[i].current = result.Coefficients[i];
                }

                //plot current
                if ((DateTime.Now - previousTime).Milliseconds >= 500)
                {
                    Macrocycle currentConf = MacrocycleFactory.Build(cycle.Atoms, MType);
                    currentConf.dataPoints = cycle.dataPoints.OrderBy(s => s.X).ToList();

                    for (int i = 0; i < currentConf.dataPoints.Count; i++)
                    {
                        //write new y data
                        currentConf.dataPoints[i] = new AtomDataPoint(currentConf.dataPoints[i].X, result.Conformation[i], currentConf.dataPoints[i].atom);
                    }
                    synchronizationContext.Post(new SendOrPostCallback(o =>
                    {
                        try
                        { simView.Model.Series.Remove(simView.Model.Series.FirstOrDefault(s => s.Title == "Current")); }
                        catch { }
                        simView.Model.Series.Add(new ScatterSeries() { ItemsSource = (List<AtomDataPoint>)o, Title = "Current", MarkerFill = OxyColors.PaleVioletRed, MarkerType = Properties.Settings.Default.simMarkerType });
                        simView.InvalidatePlot();
                        simGrid.Items.Refresh();
                    }), currentConf.dataPoints);

                    previousTime = DateTime.Now;
                }

                //get new best values if every single error is smaller or the overall sum is smaller
                if (IsNewBest(result))
                {
                    currentErr[0] = result.Error[0];
                    currentErr[1] = result.Error[1];
                    currentErr[2] = result.Error[2];

                    //new bestvalues

                    for (int i = 0; i < result.Coefficients.ToArray().Count(); i++)
                    {
                        this.param[i].best = result.Coefficients.ToArray()[i];

                    }
                    double[] err = result.Error;
                    synchronizationContext.Post(new SendOrPostCallback(o =>
                    {
                        double[] error = (double[])o;
                        ErrTB.Text = error[0].ToString("N6", System.Globalization.CultureInfo.InvariantCulture) + ";" + error[1].ToString("N6", System.Globalization.CultureInfo.InvariantCulture) + ";" + error[2].ToString("N6", System.Globalization.CultureInfo.InvariantCulture);
                        simGrid.Items.Refresh();
                    }), err);

                    Macrocycle bestConf = MacrocycleFactory.Build(cycle.Atoms, MType);
                    bestConf.dataPoints = cycle.dataPoints.OrderBy(s => s.X).ToList();

                    for (int i = 0; i < bestConf.dataPoints.Count; i++)
                    {
                        //write new y data
                        bestConf.dataPoints[i] = new AtomDataPoint(bestConf.dataPoints[i].X, result.Conformation[i], bestConf.dataPoints[i].atom);
                    }
                    synchronizationContext.Post(new SendOrPostCallback(o =>
                    {
                        try
                        { simView.Model.Series.Remove(simView.Model.Series.FirstOrDefault(s => s.Title == "Best")); }
                        catch { }
                        simView.Model.Series.Add(new ScatterSeries() { ItemsSource = (List<AtomDataPoint>)o, Title = "Best", MarkerFill = OxyColors.LawnGreen, MarkerType = Properties.Settings.Default.simMarkerType });
                        simView.InvalidatePlot();

                        //update meadDisplacement BUT! Denormalize before!
                        double fac = Application.Current.Windows.OfType<MainWindow>().First().normFac;
                        List<AtomDataPoint> tmp = new List<AtomDataPoint>();
                        foreach (AtomDataPoint dp in bestConf.dataPoints)
                        {
                            tmp.Add(new AtomDataPoint(dp.X, dp.Y * fac, dp.atom));
                        }
                        Macrocycle tmpC = MacrocycleFactory.Build(cycle.Atoms, MType);
                        tmpC.dataPoints = tmp;
                        meanDisPar.Content = tmpC.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture);

                    }), bestConf.dataPoints);

                }
                //if cb is set return...
                if (firstOnly)
                {
                    synchronizationContext.Post(new SendOrPostCallback(o =>
                    {
                        running = false;
                        startBtn.IsEnabled = true;
                        stopBtn.IsEnabled = false;
                        firstOnlyCB.IsEnabled = true;
                        keepBestCB.IsEnabled = true;
                        finishSimBtn.IsEnabled = true;
                        simGrid.IsEnabled = true;
                        fitDataCB.IsEnabled = true;
                        fitIntCB.IsEnabled = true;
                        fitDerivCB.IsEnabled = true;
                    }), null);
                }
            }

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
            if (targetData)
                errorTargets.Add(new Tuple<double, double>(error, currentErr[0]));
            if (targetDeriv)
                errorTargets.Add(new Tuple<double, double>(derErr, currentErr[1]));
            if (targetInt)
                errorTargets.Add(new Tuple<double, double>(intErr, currentErr[2]));



            double targetSum = 0;
            double lsSum = 0;
            bool[] isBest = new bool[errorTargets.Count];
            for (int i = 0; i < errorTargets.Count; i++)
            {
                targetSum += errorTargets[i].Item1;
                lsSum += errorTargets[i].Item2;
                if (errorTargets[i].Item1 < errorTargets[i].Item2)
                    isBest[i] = true;
                else
                    isBest[i] = false;
            }

            if (isBest.All(x => x))
                return true;
            //now check the sum! if sum of targets is smaller -> return true
            //if not, do nothing
            if (targetSum < lsSum)
                return true;

            //return false as default
            return false;

        }
        #region UI Interaction
        /// <summary>
        /// Handle Start Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            this.running = true;
            simGrid.IsEnabled = false;
            startBtn.IsEnabled = false;
            stopBtn.IsEnabled = true;
            firstOnlyCB.IsEnabled = false;
            keepBestCB.IsEnabled = false;
            finishSimBtn.IsEnabled = false;
            fitDataCB.IsEnabled = false;
            fitIntCB.IsEnabled = false;
            fitDerivCB.IsEnabled = false;

            await Task.Run(() => this.Simulate());
        }

        /// <summary>
        /// Handle Stop Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            this.running = false;

            //set best to start
            if (keepBestCB.IsChecked == true)
            {
                for (int i = 0; i < this.param.Count; i++)
                {
                    this.param[i].start = this.param[i].best;
                }
                simGrid.Items.Refresh();
            }
            startBtn.IsEnabled = true;
            stopBtn.IsEnabled = false;
            firstOnlyCB.IsEnabled = true;
            keepBestCB.IsEnabled = true;
            simGrid.IsEnabled = true;
            finishSimBtn.IsEnabled = true;
            fitDataCB.IsEnabled = true;
            fitIntCB.IsEnabled = true;
            fitDerivCB.IsEnabled = true;
            simplex = null;
        }

        /// <summary>
        /// Handle Check&Uncheck of First Only Checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstOnlyCB_Checked(object sender, RoutedEventArgs e)
        {
            firstOnly = (firstOnlyCB.IsChecked == true ? true : false);
        }

        /// <summary>
        /// Handle Selection Changed of Mode ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            type = (SimulationMode)modeCB.SelectedIndex;
        }

        /// <summary>
        /// Handle Finalize Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishSimBtn_Click(object sender, RoutedEventArgs e)
        {
            string[] dontMark = Properties.Settings.Default.dontMark.Split(',');
            parentView.InvalidatePlot();
            //has simulation!
            if (simView.Model.Series.Count >= 2)
            {
                parentView.Model.Annotations.Clear();
                //save exp series
                ScatterSeries exp = (ScatterSeries)parentView.Model.Series[0];

                //clear all series and readd series
                parentView.Model.Series.Clear();
                parentView.Model.Series.Add(exp);

                ScatterSeries sim = (ScatterSeries)simView.Model.Series.FirstOrDefault(s => s.Title == "Best");
                Simulation simObj = new Simulation((Macrocycle)cycle.Clone())
                {
                    simParam = param
                };
                simObj.cycle.dataPoints = (List<AtomDataPoint>)sim.ItemsSource;

                //dont mark (sim)
                for (int i = 0; i < simObj.cycle.dataPoints.Count; i++)
                {
                    if (dontMark.Contains(simObj.cycle.dataPoints[i].atom.Type) || dontMark.Contains(simObj.cycle.dataPoints[i].atom.Identifier))
                        simObj.cycle.dataPoints[i].Size = 0;
                }
                //export param
                foreach (SimParam p in param)
                {
                    simObj.par.Add(p.title, Math.Round(p.best * 100, 2));
                }
                //export errors
                simObj.errors = new double[]
                {
                    Convert.ToDouble(ErrTB.Text.Split(';')[0], System.Globalization.CultureInfo.InvariantCulture),
                    Convert.ToDouble(ErrTB.Text.Split(';')[1], System.Globalization.CultureInfo.InvariantCulture),
                    Convert.ToDouble(ErrTB.Text.Split(';')[2], System.Globalization.CultureInfo.InvariantCulture),
                };

                //set true if exp has been inverted
                if (Application.Current.Windows.OfType<MainWindow>().First().invert)
                    simObj.isInverted = true;
                Application.Current.Windows.OfType<MainWindow>().First().DelSimButton.IsEnabled = true;
                Application.Current.Windows.OfType<MainWindow>().First().DiffSimButton.IsEnabled = true;

                MacrocyclePainter.Paint(parentView.Model, simObj.cycle, MacrocyclePainter.PaintMode.Sim);
                Application.Current.Windows.OfType<MainWindow>().First().simulation = simObj;
                Application.Current.Windows.OfType<MainWindow>().First().Analyze();
            }
        }

        /// <summary>
        /// Handle fit target checkboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FitDataCB_Checked(object sender, RoutedEventArgs e)
        {
            targetData = (fitDataCB.IsChecked == true ? true : false);
        }
        private void FitDerivCB_Checked(object sender, RoutedEventArgs e)
        {
            targetDeriv = (fitDerivCB.IsChecked == true ? true : false);
        }
        private void FitIntCB_Checked(object sender, RoutedEventArgs e)
        {
            targetInt = (fitIntCB.IsChecked == true ? true : false);
        }

        /// <summary>
        /// Handle Window Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //stop sim thread on closing
            running = false;
        }
        #endregion

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

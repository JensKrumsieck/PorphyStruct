using OxyPlot;
using OxyPlot.Series;
using PorphyStruct.Chemistry;
using PorphyStruct.Simulations;
using PorphyStruct.Util;
using PorphyStruct.ViewModel;
using PorphyStruct.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
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

        private MainViewModel MainVM;

        public SimViewModel viewModel;

        private DateTime LastUpdate = DateTime.Now;

        /// <summary>
        /// Constructs the Window
        /// </summary>
        /// <param name="cycle">Macrocycle to simulate</param>
        /// <param name="pv">Plotview to Plot after Simulation</param>
        public SimWindow()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;

            //Copy Cycle & Model
            MainVM = Application.Current.Windows.OfType<MainWindow>().First().viewModel;
            viewModel = new SimViewModel(MainVM.Cycle);
            //Set Context
            DataContext = viewModel;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;

        }

        /// <summary>
        /// Keep Track of GUI Elements reacting to changed properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(viewModel.IsRunning):
                    if (!viewModel.IsRunning)
                    {
                        // set best to start
                        if (viewModel.KeepBest)
                        {
                            for (int i = 0; i < viewModel.Parameters.Count; i++) viewModel.Parameters[i].Start = viewModel.Parameters[i].Best;
                        }
                        synchronizationContext.Send(new SendOrPostCallback(o =>
                        {
                            //after running reenable/disable gui elements
                            simGrid.Items.Refresh();
                            stopBtn.IsEnabled = false;
                            startBtn.IsEnabled = firstOnlyCB.IsEnabled = keepBestCB.IsEnabled = simGrid.IsEnabled = finishSimBtn.IsEnabled = fitDataCB.IsEnabled = fitIntCB.IsEnabled = fitDerivCB.IsEnabled = true;
                        }), 0);
                    }
                    else
                    {
                        synchronizationContext.Send(new SendOrPostCallback(o =>
                        {
                            //enable/disable gui elements while simulation
                            stopBtn.IsEnabled = true;
                            simGrid.IsEnabled = startBtn.IsEnabled = firstOnlyCB.IsEnabled = keepBestCB.IsEnabled = finishSimBtn.IsEnabled = fitDataCB.IsEnabled = fitIntCB.IsEnabled = fitDerivCB.IsEnabled = false;
                        }), 0);
                    }
                    break;
                case nameof(viewModel.Best):
                    SyncGUI(viewModel.Best, "Best");
                    break;
                case nameof(viewModel.Current):
                    SyncGUI(viewModel.Current);

                    break;
            }
        }

        //if call comes with sim, use this param
        public SimWindow(Simulation sim) : this()
        {
            viewModel.Parameters = sim.simParam;
        }

        /// <summary>
        /// Synchronize GUI
        /// </summary>
        /// <param name="result"></param>
        /// <param name="target"></param>
        private void SyncGUI(Result result, string target = "Current")
        {
            if ((DateTime.Now - LastUpdate).Milliseconds >= 100 || target == "Best")
            {
                SynchronizeDiagram(viewModel.ConformationToData(result), target);
                LastUpdate = DateTime.Now;
            }
            if (target == "Best")
            {
                SynchronizeErrors(result);
                SynchronizeMeanDisplacement(viewModel.ConformationToData(result));
            }
        }

        /// <summary>
        /// Syncs the diagram for current and best
        /// </summary>
        internal void SynchronizeDiagram(List<AtomDataPoint> data, string target = "Current")
        {
            synchronizationContext.Send(new SendOrPostCallback(o =>
            {
                simGrid.Items.Refresh();
                if (simView.Model.Series.Where(s => s.Title == target).Count() != 0) simView.Model.Series.Remove(simView.Model.Series.Where(s => s.Title == target).FirstOrDefault());
                simView.Model.Series.Add(new ScatterSeries() { ItemsSource = (List<AtomDataPoint>)o, Title = target, MarkerFill = target == "Current" ? OxyColors.PaleVioletRed : OxyColors.LawnGreen, MarkerType = PorphyStruct.Core.Properties.Settings.Default.simMarkerType });
                simView.InvalidatePlot();
            }), data);
        }

        /// <summary>
        /// Synchronize Error Textbox
        /// </summary>
        /// <param name="result"></param>
        internal void SynchronizeErrors(Result result)
        {
            synchronizationContext.Send(new SendOrPostCallback(o =>
            {
                double[] error = (double[])o;
                ErrTB.Text = string.Join(";", error.Select(s => s.ToString("N6", System.Globalization.CultureInfo.InvariantCulture)));
            }), result.Error);
        }

        /// <summary>
        /// Synchronizes Mean Displacement Textbox
        /// </summary>
        /// <param name="data"></param>
        internal void SynchronizeMeanDisplacement(List<AtomDataPoint> data)
        {
            synchronizationContext.Send(new SendOrPostCallback(o =>
            {
                //update meadDisplacement BUT! Denormalize before!
                var tmp = MacrocycleFactory.Build(viewModel.Cycle.Atoms, viewModel.Cycle.type);
                tmp.dataPoints = ((List<AtomDataPoint>)o).Factor(MainVM.normFac).ToList();
                meanDisPar.Content = tmp.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture);
            }), data);
        }

        /// <summary>
        /// Handle Start Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartBtn_Click(object sender, RoutedEventArgs e) => viewModel.StartSimulation();

        /// <summary>
        /// Handle Stop Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopBtn_Click(object sender, RoutedEventArgs e) => viewModel.StopSimulation();

        /// <summary>
        /// Handle Finalize Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishSimBtn_Click(object sender, RoutedEventArgs e)
        {
            if (simView.Model.Series.Count >= 2)
            {
                ScatterSeries sim = (ScatterSeries)simView.Model.Series.FirstOrDefault(s => s.Title == "Best");
                Simulation simObj = new Simulation((Macrocycle)viewModel.Cycle.Clone()) { simParam = viewModel.Parameters };
                simObj.cycle.dataPoints = (List<AtomDataPoint>)sim.ItemsSource;
                //export param
                foreach (SimParam p in viewModel.Parameters) simObj.par.Add(p.Title, p.Best * 100);
                //export errors
                simObj.errors = ErrTB.Text.Split(';').Select(s => Convert.ToDouble(s, System.Globalization.CultureInfo.InvariantCulture)).ToArray();

                //set true if exp has been inverted
                if (MainVM.Invert) simObj.isInverted = true;
                Application.Current.Windows.OfType<MainWindow>().First().DelSimButton.IsEnabled = true;
                Application.Current.Windows.OfType<MainWindow>().First().DiffSimButton.IsEnabled = true;
                MainVM.simulation = simObj;
                MainVM.Normalize = false;
                Application.Current.Windows.OfType<MainWindow>().First().Analyze();
            }
        }

        /// <summary>
        /// Handle Window Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => viewModel.StopSimulation();

        /// <summary>
        /// Reloads Sim File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadSimBtn_Click(object sender, RoutedEventArgs e)
        {
            //use save dir as default because there should be the results
            var ofd = FileUtil.DefaultOpenFileDialog("Simulation File (*.xml)|*.xml", true);
            bool? DialogResult = ofd.ShowDialog();

            if (DialogResult.HasValue && DialogResult.Value)
            {
                string file = File.ReadAllText(ofd.FileName);
                XmlDocument xmld = new XmlDocument();
                xmld.Load(new StringReader(file));
                var simul = xmld.SelectSingleNode("descendant::simulation");
                viewModel.Parameters = new List<SimParam>();
                foreach (XmlNode node in simul.SelectNodes("descendant::parameter"))
                {
                    viewModel.Parameters.Add(new SimParam(node.Attributes.GetNamedItem("name").InnerText, double.Parse(node.InnerText, System.Globalization.CultureInfo.InvariantCulture) / 100));
                }
                simGrid.ItemsSource = viewModel.Parameters;
            }
        }
    }
}

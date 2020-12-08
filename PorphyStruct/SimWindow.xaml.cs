﻿using OxyPlot;
using OxyPlot.Series;
using PorphyStruct.Chemistry.Data;
using PorphyStruct.Chemistry.Properties;
using PorphyStruct.Simulations;
using PorphyStruct.Util;
using PorphyStruct.ViewModel;
using PorphyStruct.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Windows;

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
        private readonly SynchronizationContext? synchronizationContext;

        private readonly MainViewModel MainVM;

        public SimViewModel viewModel;

        private DateTime LastUpdate = DateTime.Now;

        /// <summary>
        /// Constructs the Window
        /// </summary>
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
                        if (viewModel.KeepBest)
                            for (int i = 0; i < viewModel.Parameters.Count; i++) viewModel.Parameters[i].Start = viewModel.Parameters[i].Best;
                        synchronizationContext?.Send(new SendOrPostCallback(o =>
                        {
                            simGrid.Items.Refresh();
                            stopBtn.IsEnabled = false;
                            startBtn.IsEnabled = firstOnlyCB.IsEnabled = keepBestCB.IsEnabled = simGrid.IsEnabled = finishSimBtn.IsEnabled = fitDataCB.IsEnabled = fitIntCB.IsEnabled = fitDerivCB.IsEnabled = true;
                        }), 0);
                    }
                    else
                    {
                        synchronizationContext?.Send(new SendOrPostCallback(o =>
                        {
                            stopBtn.IsEnabled = true;
                            simGrid.IsEnabled = startBtn.IsEnabled = firstOnlyCB.IsEnabled = keepBestCB.IsEnabled = finishSimBtn.IsEnabled = fitDataCB.IsEnabled = fitIntCB.IsEnabled = fitDerivCB.IsEnabled = false;
                        }), 0);
                    }
                    break;
                case nameof(viewModel.Best): SyncGUI(viewModel.Best, "Best"); break;
                case nameof(viewModel.Current): SyncGUI(viewModel.Current); break;
            }
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
            if (target == "Best") SynchronizeErrors(result);
        }

        /// <summary>
        /// Syncs the diagram for current and best
        /// </summary>
        internal void SynchronizeDiagram(List<AtomDataPoint> data, string target = "Current")
        {
            //also update current if best is updated ;)
            if (target == "Best") SynchronizeDiagram(data, "Current");
            synchronizationContext?.Send(new SendOrPostCallback(o =>
            {
                simGrid.Items.Refresh();
                if (simView.Model.Series.Count(s => s.Title == target) != 0) simView.Model.Series.Remove(simView.Model.Series.FirstOrDefault(s => s.Title == target));
                simView.Model.Series.Add(new ScatterSeries() { ItemsSource = (List<AtomDataPoint>)o!, Title = target, MarkerFill = target == "Current" ? OxyColors.PaleVioletRed : OxyColors.LawnGreen, MarkerType = PorphyStruct.Core.Properties.Settings.Default.simMarkerType });
                simView.InvalidatePlot();
            }), data);
        }

        /// <summary>
        /// Synchronize Error Textbox
        /// </summary>
        /// <param name="result"></param>
        internal void SynchronizeErrors(Result result) => synchronizationContext?.Send(new SendOrPostCallback(o =>
                                                        {
                                                            double[] error = (double[])o!;
                                                            ErrTB.Text = string.Join(";", error.Select(s => s.ToString("N6", System.Globalization.CultureInfo.InvariantCulture)));
                                                        }), result.Error);

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
                //only one Simulation is allowed!
                viewModel.Cycle.DataProviders.RemoveAll(s => s.DataType == DataType.Simulation);
                viewModel.Cycle.PropertyProviders.RemoveAll(s => s.Type == PropertyType.Simulation);

                var sim = (ScatterSeries)simView.Model.Series.FirstOrDefault(s => s.Title == "Best");
                var simObj = new SimulationData((IEnumerable<AtomDataPoint>)sim.ItemsSource)
                {
                    SimulationParameters = viewModel.Parameters,
                    Inverted = MainVM.Invert
                };

                //Le sim is property and data provider
                viewModel.Cycle.PropertyProviders.Add(simObj);
                viewModel.Cycle.DataProviders.Add(simObj);

                //Denormalize Sim
                simObj.DataPoints = simObj.DataPoints.Factor(1 / viewModel.Cycle.CalculateDataPoints().GetNormalizationFactor()).ToList();

                Application.Current.Windows.OfType<MainWindow>().First().DelSimButton.IsEnabled = true;
                Application.Current.Windows.OfType<MainWindow>().First().DiffSimButton.IsEnabled = true;
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
            Microsoft.Win32.OpenFileDialog ofd = FileUtil.DefaultOpenFileDialog("Properties file (*.json)|*.json", true);
            bool? dialogResult = ofd.ShowDialog();

            if (!dialogResult.HasValue || !dialogResult.Value) return;

            string file = File.ReadAllText(ofd.FileName);
            if (!(JsonSerializer.Deserialize(file, typeof(Dictionary<string, IEnumerable<Property>>)) is Dictionary<string, IEnumerable<Property>> deserialized)) return;
            if (!deserialized.ContainsKey("Simulation")) return;
            viewModel.Parameters.Clear();
            foreach (Property item in deserialized["Simulation"])
            {
                if (!item.Name.Contains("percentage")) continue;
                viewModel.Parameters.Add(
                    new SimParam(
                        item.Name.Split(' ')[0],
                        Convert.ToDouble(item.Value.Split(' ')[0]) / 100,
                        Convert.ToDouble(item.Value.Split(' ')[0]) / 100
                    )
                );
            }

            simGrid.Items.Refresh();
        }
    }
}

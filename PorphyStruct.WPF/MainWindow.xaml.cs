using System;
using HelixToolkit.Wpf;
using OxyPlot;
using OxyPlot.Series;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.Windows;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PorphyStruct.Plot;
using ThemeCommons.Controls;

namespace PorphyStruct.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DefaultWindow
    {
        public MacrocycleViewModel ViewModel { get; }

        public MainWindow()
        {
            Settings.Instance.Load($"{AppDomain.CurrentDomain.BaseDirectory}/settings.json");
            DataContext = ViewModel = new MacrocycleViewModel();
            InitializeComponent();
            PlotView.Model = ViewModel.Model;
            PlotView.Background = Brushes.White;
        }

        /// <summary>
        /// Handles Atom selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Viewport3D_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var hits = Viewport3D.Viewport.FindHits(e.GetPosition(Viewport3D));
            foreach (var hit in hits.OrderBy(s => s.Distance))
            {
                if (hit.Visual.GetType() != typeof(AtomVisual3D)) continue;
                var av3d = hit.Visual as AtomVisual3D;
                ViewModel.SelectedItem = av3d?.Atom;
                CoordinateGrid.ScrollIntoView(ViewModel.SelectedItem!);
            }
        }

        private async void Detect_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(ViewModel.Macrocycle.Detect);
            ViewModel.Validate();
        }

        private void Analyze_OnClick(object sender, RoutedEventArgs e)
        {
            var points = ViewModel.Macrocycle.DetectedParts[0].CalculateDataPoints();
            var model = new DefaultPlotModel();
            model.Series.Add(new ScatterSeries { ItemsSource = points, TrackerFormatString = AtomDataPoint.TrackerFormatString, ColorAxisKey = "colors", MarkerType = Settings.Instance.MarkerType});
            PlotView.Model = model;
        }
    }
}

using ChemSharp.Mathematics;
using HelixToolkit.Wpf;
using PorphyStruct.Analysis;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.Windows;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using ThemeCommons.Controls;

namespace PorphyStruct.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DefaultWindow
    {
        public MacrocycleViewModel ViewModel { get; private set; }

        public MainWindow()
        {
            Settings.Instance.Load($"{AppDomain.CurrentDomain.BaseDirectory}/settings.json");
            InitializeComponent();
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
                ViewModel.SelectedAtom = av3d?.Atom;
                CoordinateGrid.ScrollIntoView(ViewModel.SelectedAtom!);
            }
        }

        /// <summary>
        /// Drag and Drop Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void OnFileDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            OpenFile((files ?? throw new InvalidOperationException()).FirstOrDefault());
        }

        private void OpenFile(string path)
        {
            DataContext = ViewModel = new MacrocycleViewModel(path);
            ViewModel.SelectedIndexChanged += ViewModelOnSelectedIndexChanged;
            TypePopup.Visibility = Visibility.Visible;
            Viewport3D.CameraController.CameraTarget =
                MathV.Centroid(ViewModel.Macrocycle.Atoms.Select(s => s.Location)).ToPoint3D();
        }

        private void ViewModelOnSelectedIndexChanged(object? sender, EventArgs e) => Viewport3D.CameraController.CameraTarget =
                MathV.Centroid(ViewModel.SelectedItem.Analysis.Atoms.Select(s => s.Location)).ToPoint3D();
        private async void Analyze_OnClick(object sender, RoutedEventArgs e) => await ViewModel.Detect();
        private void Simulate_OnClick(object sender, RoutedEventArgs e) => Task.Run(ViewModel.SelectedItem.Simulate);

        private void TypeSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;
            var type = (MacrocycleType)TypeList.SelectedIndex;
            ViewModel.Macrocycle.MacrocycleType = type;
            TypePopup.Visibility = Visibility.Hidden;
        }

        private void URL_OnClicked(object sender, RequestNavigateEventArgs e) =>
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });

        private void Info_OnClick(object sender, RoutedEventArgs e) => InfoPopup.Visibility =
            InfoPopup.Visibility == Visibility.Collapsed
                ? InfoPopup.Visibility = Visibility.Visible
                : InfoPopup.Visibility = Visibility.Collapsed;
    }
}

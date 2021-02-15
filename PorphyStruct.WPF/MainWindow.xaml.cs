using HelixToolkit.Wpf;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
            DataContext = ViewModel = new MacrocycleViewModel((files ?? throw new InvalidOperationException()).FirstOrDefault());
        }


        private async void Detect_OnClick(object sender, RoutedEventArgs e) => await ViewModel.Detect();

        private void Analyze_OnClick(object sender, RoutedEventArgs e) => ViewModel.SelectedItem.Analyze();
    }
}

using ChemSharp.Mathematics;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using PorphyStruct.Core;
using PorphyStruct.Core.Analysis;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.IO;
using PorphyStruct.ViewModel.Windows.Visual;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using ThemeCommons.Controls;
using MacrocycleViewModel = PorphyStruct.ViewModel.Windows.MacrocycleViewModel;

namespace PorphyStruct.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DefaultWindow
    {
        public MacrocycleViewModel ViewModel { get; private set; }

        /// <summary>
        /// Should not be null!
        /// </summary>
        public string Version => Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
            .InformationalVersion!;

        public MainWindow()
        {
            Settings.Instance.Load();
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

        /// <summary>
        /// Handles open File Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_OnClick(object sender, RoutedEventArgs e)
        {
            var path = string.IsNullOrEmpty(Settings.Instance.DefaultImportPath)
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                : Settings.Instance.DefaultImportPath;
            var ofd = new OpenFileDialog
            {
                InitialDirectory = path,
                Filter = Constants.OpenFileFilter,
                Multiselect = false
            };
            if (ofd.ShowDialog(this) != true) return;
            OpenFile(ofd.FileName);
        }

        /// <summary>
        /// Handles actual file opening
        /// </summary>
        /// <param name="path"></param>
        internal void OpenFile(string path)
        {
            DataContext = ViewModel = new MacrocycleViewModel(path);
            ViewModel.SelectedIndexChanged += ViewModelOnSelectedIndexChanged;
            TypePopup.Visibility = Visibility.Visible;
            Viewport3D.CameraController.CameraTarget =
                MathV.Centroid(ViewModel.Macrocycle.Atoms.Select(s => s.Location).ToList()).ToPoint3D();
            Activate();
        }

        private void ViewModelOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewModel?.SelectedItem != null)
                Viewport3D.CameraController.CameraTarget =
                    MathV.Centroid(ViewModel.SelectedItem.Analysis.Atoms.Select(s => s.Location)).ToPoint3D();
        }

        private async Task PrepareAnalysis()
        {
            //Block UI interaction during this
            MainGrid.IsEnabled = false;
            TitleGrid.IsEnabled = false;
            await ViewModel.Analyze();
            MainGrid.IsEnabled = true;
            TitleGrid.IsEnabled = true;
            AnalyzePopup.IsOpen = false;
        }

        private async void Analyze_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Instance.UseExtendedBasis = false;
            await PrepareAnalysis();
        }

        private async void AnalyzeExt_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Instance.UseExtendedBasis = true;
            await PrepareAnalysis();
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            var sw = new SaveWindow(ViewModel.SelectedItem);
            sw.ShowDialog();
        }

        private void TypeSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;
            var type = (MacrocycleType)TypeList.SelectedIndex;
            ViewModel.Macrocycle.MacrocycleType = type;
            TogglePopup(TypePopup);
        }

        private void URL_OnClicked(object sender, RequestNavigateEventArgs e) =>
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });

        private void Info_OnClick(object sender, RoutedEventArgs e) => TogglePopup(InfoPopup);

        private void Append_OnClick(object sender, RoutedEventArgs e)
        {
            var path = string.IsNullOrEmpty(Settings.Instance.DefaultImportPath)
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                : Settings.Instance.DefaultImportPath;
            var ofd = new OpenFileDialog
            {
                InitialDirectory = path,
                Filter = Constants.CompareFileFilter,
                Multiselect = false
            };
            if (ofd.ShowDialog(this) != true) return;
            var data = new CompareData(ofd.FileName);
            ViewModel.SelectedItem.CompareData.Add(data);
            AppendedData.SelectedItems.Add(data);
        }

        private void CompareAdd_OnClick(object sender, RoutedEventArgs e)
        {
            var appendedData = AppendedData.SelectedItems.Cast<CompareData>().ToList();
            var remove = ViewModel.SelectedItem.CompareData.Except(appendedData);
            foreach (var item in remove) ViewModel.SelectedItem.CompareData.Remove(item);
            var localData = LocalData.SelectedItems.Cast<AnalysisViewModel>();
            foreach (var item in localData)
                ViewModel.SelectedItem.CompareData.Add(new CompareData(item.Title, item.Analysis.DataPoints.ToList()));
            ViewModel.SelectedItem.PrepareCompare();
            TogglePopup(ComparePopUp);
        }

        private void Compare_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem.CompareData.Clear();
            ViewModel.SelectedItem.PrepareCompare();
            TogglePopup(ComparePopUp);
        }

        private static void TogglePopup(UIElement sender) => sender.Visibility =
            sender.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

        private void Settings_OnClick(object sender, RoutedEventArgs e) => new SettingsWindow().ShowDialog();

        private void Stats_OnClick(object sender, RoutedEventArgs e) => new StatisticsWindow().Show();

        private void Batch_OnClick(object sender, RoutedEventArgs e) => new BatchWindow().Show();

        private void Isolation_OnClick(object sender, RoutedEventArgs e) => new IsolationWindow(this).ShowDialog();

        /// <summary>
        /// Shows Analyze Popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalyzeBtn_Click(object sender, RoutedEventArgs e) => AnalyzePopup.IsOpen = true;
    }
}

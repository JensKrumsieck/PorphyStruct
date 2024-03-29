﻿using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using ChemSharp.Mathematics;
using ChemSharp.Molecules.HelixToolkit;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using PorphyStruct.Core;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.IO;
using ThemeCommons.Controls;
using MacrocycleViewModel = PorphyStruct.ViewModel.Windows.MacrocycleViewModel;

namespace PorphyStruct.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : DefaultWindow
{
    public MacrocycleViewModel ViewModel { get; private set; }

    /// <summary>
    /// Should not be null!
    /// </summary>
    public static string Version => Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
        .InformationalVersion!;

    public MainWindow()
    {
        Settings.Instance.Load();
        Atom3D.SelectionColor = Brushes.Fuchsia;
        InitializeComponent();
        CheckUpdate();
    }

    /// <summary>
    /// Checks for new Versions on Startup
    /// </summary>
    private async void CheckUpdate(bool force = false)
    {
        if (!Settings.Instance.AutoUpdate && !force) return;
        (Updater u, bool current) = await Updater.CreateAsync();
        if (!current) UpdateMsg.Visibility = Visibility.Visible;
        else if (force) MessageBox.Show($"You already have the latest Version of PorphyStruct ({u.Latest})");
        if (!current && force)
        {
            Process.Start(new ProcessStartInfo("http://github.com/jenskrumsieck/porphystruct/releases/latest") { UseShellExecute = true });
            UpdateMsg.Visibility = Visibility.Collapsed;
        }
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
            if (hit.Visual.GetType() != typeof(Atom3D)) continue;
            var av3d = hit.Visual as Atom3D;
            if (av3d != null) ViewModel.SelectedAtom = av3d.Atom;
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
        OpenFile((files ?? throw new InvalidOperationException()).First());
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
        Viewport3D.CameraController.CameraTarget =
            MathV.Centroid(ViewModel.Macrocycle.Atoms.Select(s => s.Location).ToList()).ToPoint3D();
        Activate();
    }

    private void ViewModelOnSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (ViewModel?.SelectedItem != null)
            Viewport3D.CameraController.CameraTarget =
                MathV.Centroid(ViewModel.SelectedItem.Analysis.Atoms.Select(s => s.Location)).ToPoint3D();
        MainTabMenu.Focus();
    }

    private async Task PrepareAnalysis()
    {
        //Block UI interaction during this
        MainGrid.IsEnabled = false;
        TitleGrid.IsEnabled = false;
        AnalyzePopup.IsOpen = false;
        await ViewModel.Analyze();
        MainGrid.IsEnabled = true;
        TitleGrid.IsEnabled = true;
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
    
    private void Rotate_OnClick(object sender, RoutedEventArgs e) => ViewModel.SelectedItem.Rotate();

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        var sw = new SaveWindow(ViewModel.SelectedItem);
        sw.ShowDialog();
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
    private void Update_Click(object sender, RoutedEventArgs e) => CheckUpdate(true);
}

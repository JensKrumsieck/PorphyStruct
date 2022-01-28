using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using OxyPlot.SkiaSharp;
using PorphyStruct.Core;
using PorphyStruct.ViewModel;
using ThemeCommons.Controls;
using Path = System.IO.Path;
using SvgExporter = PorphyStruct.Core.Plot.SvgExporter;

namespace PorphyStruct.WPF;

/// <summary>
/// Interaktionslogik für StatisticsWindow.xaml
/// </summary>
public partial class StatisticsWindow : DefaultWindow
{
    public StatisticsWindow()
    {
        InitializeComponent();
    }

    private void Search_OnClick(object sender, RoutedEventArgs e)
    {
        var ofd = new OpenFileDialog()
        {
            FileName = "Select Folder.",
            InitialDirectory = PathTextBox.Text,
            ValidateNames = false,
            CheckFileExists = false,
            CheckPathExists = false
        };
        if (ofd.ShowDialog(this) != true) return;
        PathTextBox!.Text = Path.GetDirectoryName(ofd.FileName) ?? "";
        PathTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
    }

    private void DataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyType != typeof(double)) return;
        if (e.Column is DataGridTextColumn column) column.Binding.StringFormat = "{0:N3}";
    }

    private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = (StatisticsViewModel)DataContext;
        foreach (var item in e.AddedItems)
            vm.SelectedYColumns.Add(item.ToString());
        foreach (var item in e.RemovedItems)
            vm.SelectedYColumns.Remove(item.ToString());
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        var vm = (StatisticsViewModel)DataContext;
        var sfd = new SaveFileDialog
        {
            Filter = "Scalable Vector Graphics (*.svg)|*.svg|Portable Network Graphics (*.png)|*.png",
            InitialDirectory = vm.WorkingDir
        };
        if (sfd.ShowDialog(this) != true) return;
        var filename = sfd.FileName;
        switch (Path.GetExtension(filename))
        {
            case ".svg":
                new SvgExporter
                {
                    Width = Settings.Instance.ExportWidth,
                    Height = Settings.Instance.ExportHeight,
                    Dpi = Settings.Instance.ExportDPI
                }.Export(vm.PlotModel, File.Create(filename));
                break;
            case ".png":
                PngExporter.Export(vm.PlotModel, File.Create(filename), (int)Settings.Instance.ExportWidth, (int)Settings.Instance.ExportHeight, Settings.Instance.ExportDPI);
                break;
            default: throw new InvalidOperationException();
        }
    }
}

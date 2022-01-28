using System.IO;
using System.Windows;
using Microsoft.Win32;
using OxyPlot.SkiaSharp;
using PorphyStruct.Core;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.IO;
using ThemeCommons.Controls;
using SvgExporter = PorphyStruct.Core.Plot.SvgExporter;

namespace PorphyStruct.CalculatorWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : DefaultWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        var vm = (CalculatorViewModel)DataContext;
        var sfd = new SaveFileDialog
        {
            Filter = "Scalable Vector Graphics (*.svg)|*.svg|Portable Network Graphics (*.png)|*.png|CSV Sheet (*.csv)|*.csv",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
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
                }.Export(vm.Model, File.Create(filename));
                break;
            case ".png":
                PngExporter.Export(vm.Model, File.Create(filename), (int)Settings.Instance.ExportWidth, (int)Settings.Instance.ExportHeight, Settings.Instance.ExportDPI);
                break;
            case ".csv":
                vm.ExportData(filename, "csv");
                break;
            default: throw new InvalidOperationException();
        }

    }
}

using System.Globalization;
using ChemSharp.Molecules;
using ChemSharp.Molecules.Export;
using OxyPlot;
using OxyPlot.SkiaSharp;
using PorphyStruct.Core;
using PorphyStruct.Core.Plot;
using SvgExporter = PorphyStruct.Core.Plot.SvgExporter;

namespace PorphyStruct.ViewModel.IO;

public static class SaveHandler
{
    public static void Export(this AnalysisViewModel viewModel, ExportFileType type, string path)
    {
        switch (type.Title)
        {
            case "Analysis":
                viewModel.ExportSimulation(path, type.Extension);
                break;
            case "Graph":
                viewModel.ExportPlot(path, type.Extension);
                break;
            case "Macrocycle":
                viewModel.ExportCycle(path);
                break;
            case "Molecule":
                viewModel.ExportMolecule(path);
                break;
            case "XYData":
                viewModel.ExportData(path, type.Extension);
                break;
        }
    }

    public static void ExportSimulation(this AnalysisViewModel viewModel, string path, string extension)
    {
        switch (extension)
        {
            case "json":
                File.WriteAllText(path + "_analysis.json", viewModel.Analysis.Properties?.ExportJson());
                break;
            case "md":
                File.WriteAllText(path + "_analysis.md", viewModel.Analysis.Properties?.ExportString());
                break;
            case "png":
            case "svg":
                viewModel.Analysis.Properties?.ExportSummaryPlot(path + "_analysis", extension);
                break;
        }
    }

    public static void ExportData(this AnalysisViewModel viewModel, string path, string extension) =>
        ExportData(viewModel.Analysis.DataPoints, path, extension);
    public static void ExportData(this CalculatorViewModel viewModel, string path, string extension) =>
        ExportData((IEnumerable<AtomDataPoint>)viewModel.Series.ItemsSource, path, extension);

    private static void ExportData(IEnumerable<AtomDataPoint> dataPoints, string path, string extension)
    {
        var separator = extension == "csv" ? "," : ";";
        var result = $"X{separator}Y{separator}Atom{separator}\n";
        result = dataPoints.Aggregate(result, (current, data) => current + $"{data.X.ToString(CultureInfo.InvariantCulture)}{separator}{data.Y.ToString(CultureInfo.InvariantCulture)}{separator}{data.Atom.Title}{separator}\n");
        File.WriteAllText(path + "_data." + extension, result);
    }

    public static void ExportPlot(this AnalysisViewModel viewModel, string path, string extension)
    {
        using var stream = File.Create(path + "_graph." + extension);
        IExporter exporter = extension switch
        {
            "svg" => new SvgExporter
            {
                Height = Settings.Instance.ExportHeight,
                Width = Settings.Instance.ExportWidth,
                Dpi = Settings.Instance.ExportDPI
            },
            "png" => new PngExporter
            {
                Height = (int)Settings.Instance.ExportHeight,
                Width = (int)Settings.Instance.ExportWidth,
                Dpi = Settings.Instance.ExportDPI
            },
            _ => null
        };
        exporter?.Export(viewModel.Model, stream);
    }

    public static void ExportCycle(this AnalysisViewModel viewModel, string path) => Mol2Exporter.Export(new Molecule(viewModel.Analysis.Atoms), path + "_cycle.mol2");

    public static void ExportMolecule(this AnalysisViewModel viewModel, string path) => Mol2Exporter.Export(viewModel.Parent.Macrocycle, path + "_molecule.mol2");
}

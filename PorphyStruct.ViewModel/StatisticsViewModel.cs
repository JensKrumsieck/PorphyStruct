using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ChemSharp.Molecules;
using ChemSharp.Molecules.Properties;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using PorphyStruct.Core;
using PorphyStruct.Core.Extension;
using PorphyStruct.Core.Plot;
using PorphyStruct.ViewModel.Utility;
using TinyMVVM;
using Element = ChemSharp.Molecules.Element;

namespace PorphyStruct.ViewModel;

public class StatisticsViewModel : BaseViewModel
{
    private string _workingDir;
    public string WorkingDir
    {
        get => _workingDir;
        set => Set(ref _workingDir, value, PorphyMerge);
    }

    private bool _isRecursive = true;
    public bool IsRecursive
    {
        get => _isRecursive;
        set => Set(ref _isRecursive, value, PorphyMerge);
    }

    private bool _modesOnly = true;
    /// <summary>
    /// Show only columns that represent the simulation modes
    /// </summary>
    public bool ModesOnly
    {
        get => _modesOnly;
        set => Set(ref _modesOnly, value, PorphyMerge);
    }

    private bool _absoluteValues = true;
    /// <summary>
    /// Uses Absolute Values in Plot
    /// </summary>
    public bool AbsoluteValues
    {
        get => _absoluteValues;
        set => Set(ref _absoluteValues, value, UpdatePlot);
    }

    private bool _showLabels = true;
    /// <summary>
    /// Show Labels at Bars in BarSeries
    /// </summary>
    public bool ShowLabels
    {
        get => _showLabels;
        set => Set(ref _showLabels, value, UpdatePlot);
    }

    private string[] _files;
    public string[] Files
    {
        get => _files;
        set => Set(ref _files, value);
    }

    private string _selectedXColumn = "";
    public string SelectedXColumn
    {
        get => _selectedXColumn;
        set => Set(ref _selectedXColumn, value, UpdatePlot);
    }
    public ObservableCollection<string> SelectedYColumns = new();

    private readonly DataTable _table = new();
    public DataView Table => _table.DefaultView;

    public BasePlotModel PlotModel { get; } = new BasePlotModel();

    public readonly List<JsonPropertyHelper> Data = new();

    public StatisticsViewModel()
    {
        SelectedYColumns.CollectionChanged += SelectedYColumnsOnCollectionChanged;
    }

    private void SelectedYColumnsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        UpdatePlot();

    /// <summary>
    /// Merges all (regardless of Type) *_analysis.json files
    /// fka. as PorphyMerge (https://github.com/JensKrumsieck/PorphyMerge)
    /// </summary>
    private void PorphyMerge()
    {
        Data.Clear();
        if (!Directory.Exists(WorkingDir) || string.IsNullOrEmpty(WorkingDir)) return;
        Files = Directory.GetFiles(WorkingDir, "*_analysis.json",
            IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        if (!Files.Any()) return;
        foreach (var file in Files)
        {
            var props = JsonSerializer.Deserialize<JsonPropertyHelper>(File.ReadAllText(file));
            props.Title = Path.GetFileNameWithoutExtension(file).Replace("_analysis", "");
            Data.Add(props);
        }
        UpdateTable();
        ToCsv();
        UpdatePlot();
    }

    /// <summary>
    /// Merges all Properties into single list
    /// </summary>
    /// <param name="helper"></param>
    /// <returns></returns>
    private static IEnumerable<KeyValueProperty> Merge(JsonPropertyHelper helper) => helper.Distances
        .Concat(helper.PlaneDistances ?? new List<KeyValueProperty>())
        .Concat(helper.Angles)
        .Append(helper.InterplanarAngle)
        .Concat(helper.Dihedrals).ToList();

    /// <summary>
    /// Creates DataTable
    /// </summary>
    /// <returns></returns>
    private void UpdateTable()
    {
        _table.Clear();
        _table.Columns.Clear();
        _table.Columns.Add("Title", typeof(string));
        foreach (var d in Data)
        {
            var row = _table.NewRow();
            row["Title"] = d.Title;
            foreach (var m in d.Simulation.SimulationResult
                .Append(d.Simulation.OutOfPlaneParameter).Append(d.OutOfPlaneParameter))
            {
                if (!_table.Columns.Contains(m.Key)) _table.Columns.Add(m.Key, typeof(double));
                row.SetField(_table.Columns[m.Key]!, m.Value);
            }
            if (!ModesOnly)
                foreach (var m in Merge(d).Where(k => k.Key != null && !string.IsNullOrEmpty(k.Key)))
                {
                    var key = GenericMetal(m.Key);
                    key = key.Replace("[", "").Replace("]", "");
                    if (!_table.Columns.Contains(key)) _table.Columns.Add(key, typeof(double));
                    row.SetField(_table.Columns[key]!, m.Value);
                }
            _table.Rows.Add(row);
        }
        OnPropertyChanged(nameof(Table));
    }

    /// <summary>
    /// Creates and saves CSV File from Table
    /// </summary>
    private void ToCsv()
    {
        const char separator = ';';
        var sb = new StringBuilder();
        var columnNames = _table.Columns.Cast<DataColumn>().
            Select(column => column.ColumnName);
        sb.AppendLine(string.Join(separator, columnNames));

        foreach (DataRow row in _table.Rows)
            sb.AppendLine(string.Join(separator, row.ItemArray.Select(field => field?.ToString())));

        File.WriteAllText(WorkingDir + "/PorphyStruct_MergedData.csv", sb.ToString());
    }

    /// <summary>
    /// Updates Plot based on Selection
    /// </summary>
    private void UpdatePlot()
    {
        PlotModel.Series.Clear();
        PlotModel.Axes.Clear();
        PlotModel.Axes.Add(PlotModel.XAxis);

        if (string.IsNullOrEmpty(SelectedXColumn) || !SelectedYColumns.Any()) return;
        if (SelectedYColumns.Count > 1) PlotModel.Legends.Add(new Legend());

        if (SelectedXColumn == "Title") RequestBarSeries();
        else RequestScatterSeries();

        PlotModel.InvalidatePlot(true);
    }

    /// <summary>
    /// Requests Bar Series because Title is X
    /// </summary>
    private void RequestBarSeries()
    {
        var categoryData = (from DataRow row in Table.Table!.Rows select (string)row[SelectedXColumn]).ToList();

        foreach (var col in SelectedYColumns)
        {
            if (col == "Title") continue;
            var series = new BarSeries
            {
                Title = col,
                LabelPlacement = LabelPlacement.Base,
                LabelFormatString = ShowLabels ? "{0:N3}" : ""
            };
            foreach (DataRow row in _table.Rows)
                series.Items.Add(new BarItem(AbsoluteValues ? Math.Abs((double)row[col]) : (double)row[col]));
            PlotModel.Series.Add(series);
        }

        var yAxis = new CategoryAxis
        {
            Position = AxisPosition.Left,
            ItemsSource = categoryData,
            Title = SelectedXColumn,
            AxislineThickness = Settings.Instance.AxisThickness
        };
        PlotModel.Axes.Add(yAxis);
        if (!PlotModel.PlotAreaBorderThickness.Equals(new OxyThickness(0))) return;
        yAxis.AxislineStyle = LineStyle.Solid;

    }

    /// <summary>
    /// Requests Scatter Series because non Title Field is X
    /// </summary>
    private void RequestScatterSeries()
    {
        foreach (var col in SelectedYColumns)
        {
            if (col == "Title") continue;
            var series = new ScatterSeries() { Title = col };
            foreach (DataRow row in _table.Rows)
                series.Points.Add(new ScatterPoint(AbsoluteValues ? Math.Abs((double)row[SelectedXColumn]) : (double)row[SelectedXColumn], AbsoluteValues ? Math.Abs((double)row[col]) : (double)row[col]));
            PlotModel.Series.Add(series);
        }

        PlotModel.XAxis.Title = SelectedXColumn;
        if (!PlotModel.PlotAreaBorderThickness.Equals(new OxyThickness(0))) return;
        PlotModel.DefaultYAxis.AxislineStyle = LineStyle.Solid;
        PlotModel.DefaultYAxis.AxislineThickness = Settings.Instance.AxisThickness;
    }

    private static string GenericMetal(string input)
    {
        _ = new Element("H"); //ensure class data is loaded
        var metals = ElementDataProvider.ElementData.Where(s => s.CanBeCenterAtom())
            .Select(s => s.Symbol).ToList();
        if (!metals.Any(input.Contains)) return input;
        input = metals.Aggregate(input, (current, metal) => current.Replace(metal, "M"));
        return !Regex.IsMatch(input, "M\\d+") ? input : Regex.Replace(input, "M\\d+", "M");
    }
}

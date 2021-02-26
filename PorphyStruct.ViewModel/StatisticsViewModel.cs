using ChemSharp.Molecules;
using ChemSharp.Molecules.DataProviders;
using PorphyStruct.Core.Analysis.Properties;
using PorphyStruct.ViewModel.Utility;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using TinyMVVM;

namespace PorphyStruct.ViewModel
{
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

        private string[] _files;

        public string[] Files
        {
            get => _files;
            set => Set(ref _files, value);
        }

        private readonly DataTable _table = new DataTable();
        public DataView Table => _table.DefaultView;

        public readonly List<JsonPropertyHelper> Data = new List<JsonPropertyHelper>();

        /// <summary>
        /// Merges all (regardless of Type) *_analysis.json files
        /// fka. as PorphyMerge (https://github.com/JensKrumsieck/PorphyMerge)
        /// </summary>
        private void PorphyMerge()
        {
            if (!Directory.Exists(WorkingDir)) return;
            Files = Directory.GetFiles(WorkingDir, "*_analysis.json",
                IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var file in Files)
            {
                var props = JsonSerializer.Deserialize<JsonPropertyHelper>(File.ReadAllText(file));
                props.Title = Path.GetFileNameWithoutExtension(file).Replace("_analysis", "");
                Data.Add(props);
            }
            UpdateTable();
            ToCsv();
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

        private static string GenericMetal(string input)
        {
            _ = new Element("H"); //ensure class data is loaded
            var metals = ElementDataProvider.ElementData.Where(s => s.IsMetal)
                .Select(s => s.Symbol).ToList();
            if (!metals.Any(input.Contains)) return input;
            input = metals.Aggregate(input, (current, metal) => current.Replace(metal, "M"));
            return !Regex.IsMatch(input, "M\\d+") ? input : Regex.Replace(input, "M\\d+", "M");
        }
    }
}

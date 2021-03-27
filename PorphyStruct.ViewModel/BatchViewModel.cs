using PorphyStruct.Core;
using PorphyStruct.Core.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TinyMVVM;

namespace PorphyStruct.ViewModel
{
    public class BatchViewModel : BaseViewModel
    {
        private string _workingDir;
        public string WorkingDir
        {
            get => _workingDir;
            set => Set(ref _workingDir, value, LoadFiles);
        }

        private bool _isRecursive = true;
        public bool IsRecursive
        {
            get => _isRecursive;
            set => Set(ref _isRecursive, value, LoadFiles);
        }

        private MacrocycleType _type;

        public MacrocycleType Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        private string[] _files;
        public string[] Files
        {
            get => _files;
            set => Set(ref _files, value);
        }

        private int _currentIndex = -1;

        public int CurrentIndex
        {
            get => _currentIndex;
            set => Set(ref _currentIndex, value, () => OnPropertyChanged(nameof(CurrentItem)));
        }

        public string CurrentItem => Files?.ElementAtOrDefault(CurrentIndex - 1) ?? "";

        private int _failed;
        public int Failed
        {
            get => _failed;
            set => Set(ref _failed, value);
        }

        private string _failedItems = "";
        private void LoadFiles()
        {
            var supported = new[] { "cif", "xyz", "mol2" };
            if (!Directory.Exists(WorkingDir) || string.IsNullOrEmpty(WorkingDir)) return;
            var filter = new List<string>();
            if (File.Exists(WorkingDir + "/.psignore")) filter.AddRange(File.ReadAllLines(WorkingDir + "/.psignore"));
            Files = supported.Select(ext => "*." + ext).SelectMany(f =>
            Directory.EnumerateFiles(WorkingDir, f,
                IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                .Where(s => !filter.Any(s.Contains)).ToArray();
        }

        public async Task Process()
        {
            Failed = 0;
            for (CurrentIndex = 1; CurrentIndex < Files.Length + 1; CurrentIndex++)
            {
                var current = Files[CurrentIndex - 1];
                var cycle = new Macrocycle(current) { MacrocycleType = Type };
                await Task.Run(cycle.Detect).ContinueWith(async (ts) =>
                {
                    if (!cycle.DetectedParts.Any())
                    {
                        Failed++;
                        _failedItems += Files[CurrentIndex - 1] + "\n";
                    }
                    foreach (var analysis in cycle.DetectedParts)
                    {
                        var folder = Path.GetDirectoryName(Files[CurrentIndex - 1]);
                        var file = Path.GetFileNameWithoutExtension(Files[CurrentIndex - 1]);
                        var filename = cycle.DetectedParts.Count == 1
                            ? folder + "/" + file
                            : folder + "/" + file + "_" + analysis.AnalysisColor;
                        await File.WriteAllTextAsync(filename + "_analysis.md", analysis.Properties.ExportString());
                        await File.WriteAllTextAsync(filename + "_analysis.json", analysis.Properties.ExportJson());
                    }
                });
            }
            await File.WriteAllTextAsync(WorkingDir + "/FailedItems" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt", _failedItems);
        }
    }
}


using System.Text.RegularExpressions;
using ChemSharp.Extensions;
using ChemSharp.Molecules;
using PorphyStruct.Core.Plot;

namespace PorphyStruct.ViewModel.IO
{
    public readonly struct CompareData
    {
        public string Filename { get; }
        public List<AtomDataPoint> DataPoints { get; }

        public string Title => Path.GetFileNameWithoutExtension(Filename);

        public CompareData(string path)
        {
            Filename = path;
            DataPoints = new List<AtomDataPoint>();
            LoadData();
        }

        public CompareData(string path, List<AtomDataPoint> dataPoints)
        {
            Filename = path;
            DataPoints = dataPoints;
        }

        public void LoadData()
        {
            var separator = Path.GetExtension(Filename) == ".csv" ? "," : ";";
            var content = File.ReadAllText(Filename!);
            var lines = content.Split("\n").Skip(1); //Skip headers
            foreach (var line in lines.Where(s => !string.IsNullOrEmpty(s)))
            {
                var raw = line.Split(separator);
                var x = raw[0].ToDouble();
                var y = raw[1].ToDouble();
                var atom = raw[2];
                DataPoints.Add(new AtomDataPoint(x, y, new Atom(Regex.Match(atom, @"([A-Z][a-z]*)").Value) { Title = atom }));
            }
        }

    }
}

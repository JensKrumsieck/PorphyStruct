using PorphyStruct.Chemistry;
using PorphyStruct.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PorphyStruct.ViewModel
{
    public class SaveViewModel : AbstractViewModel
    {
        public Macrocycle Cycle { get; set; }

        public SaveViewModel(Macrocycle cycle)
        {
            Cycle = cycle;
            FileName = cycle.Title;
            Path = DefaultPath;
        }
        /// <summary>
        /// returns the default save directory
        /// </summary>
        public string DefaultPath => Core.Properties.Settings.Default.useImportExportPath ? Core.Properties.Settings.Default.importPath : Core.Properties.Settings.Default.savePath;

        //Export Filename
        public string FileName { get => Get<string>(); set => Set(value); }
        //Export Path
        public string Path { get => Get<string>(); set => Set(value); }

        /// <summary>
        /// Handles Export of FileType
        /// </summary>
        /// <param name="type"></param>
        public void Export(ExportFileType type)
        {
            string path = $"{Path}/{FileName}_";
            OxyPlotOverride.StandardPlotModel model = Application.Current.Windows.OfType<MainWindow>().First().viewModel.Model; //not beautiful but works
            foreach (string extension in type.Extension)
            {
                switch (type.Title)
                {
                    case "Properties": Cycle.SaveProperties(path, extension); break;
                    case "Macrocycle": Cycle.SaveIXYZ(path, true); break;
                    case "Molecule": Cycle.SaveIXYZ(path); break;
                    case "Graph":
                        {
                            if (extension == "png") model.ExportGraph(path, FileUtil.PngExporter);
                            if (extension == "svg") model.ExportGraph(path, FileUtil.SvgExporter, extension);
                            break;
                        }
                    case "ASCII": model.SaveASCII(path); break;
                }
            }
        }

        /// <summary>
        /// Builds List of available ExportFileTypes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExportFileType> CheckFileTypes()
        {
            if (Cycle.DataProviders.Count != 0)
            {
                yield return new ExportFileType("Graph", "ChartScatterPlotHexbin", new[] { "png" });
                yield return new ExportFileType("Graph", "ChartScatterPlot", new[] { "svg" });
                yield return new ExportFileType("ASCII", "TableLarge", new[] { "dat" });
            }
            //Save xml, json and txt every time!
            yield return new ExportFileType("Properties", "AtomVariant", new[] { "json", "txt" });
            //Molecule based
            yield return new ExportFileType("Molecule", "Molecule", new[] { "ixyz" });
            yield return new ExportFileType("Macrocycle", "Molecule", new[] { "ixyz" });

        }
        /// <summary>
        /// Gets List of available ExportFileTypes
        /// </summary>
        public IEnumerable<ExportFileType> AvailableFileTypes => CheckFileTypes();
    }
}

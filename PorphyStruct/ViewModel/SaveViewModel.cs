using OxyPlot;
using OxyPlot.Wpf;
using PorphyStruct.Chemistry;
using PorphyStruct.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        public string DefaultPath
        {
            get
            {
                if (Core.Properties.Settings.Default.useImportExportPath) return Core.Properties.Settings.Default.importPath;
                else return Core.Properties.Settings.Default.savePath;
            }
        }

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
            var model = Application.Current.Windows.OfType<MainWindow>().First().viewModel.Model; //not beautiful but works
            foreach (var extension in type.Extension)
            {
                switch (type.Title)
                {
                    case "Properties": Cycle.SaveProperties(path, extension); break;
                    case "Macrocycle": Cycle.SaveIXYZ(path, true); break;
                    case "Molecule": Cycle.SaveIXYZ(path); break;
                    case "Graph":
                        {
                            if(extension == "png")
                                model.ExportGraph(path,
                                  new PngExporter()
                                  {
                                      Height = Core.Properties.Settings.Default.pngHeight,
                                      Width = Core.Properties.Settings.Default.pngWidth,
                                      Resolution = Core.Properties.Settings.Default.pngRes,
                                      Background = Core.Properties.Settings.Default.backgroundColor ? OxyColors.White : OxyColors.Transparent
                                  });
                            if (extension == "svg")
                                model.ExportGraph(path,
                                    new OxyPlot.Wpf.SvgExporter()
                                    {
                                        Height = Core.Properties.Settings.Default.pngHeight,
                                        Width = Core.Properties.Settings.Default.pngWidth,
                                        TextMeasurer = new CanvasRenderContext(new Canvas()),
                                        IsDocument = true
                                    }, extension);
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
                yield return new ExportFileType() { Title = "Graph", Extension = new[] { "png" }, Icon = "ChartScatterPlotHexbin" };
                yield return new ExportFileType() { Title = "Graph", Extension = new[] { "svg" }, Icon = "ChartScatterPlot" };
                yield return new ExportFileType() { Title = "ASCII", Extension = new[] { "dat" }, Icon = "TableLarge" };
            }
            //Save xml, json and txt every time!
            yield return new ExportFileType() { Title = "Properties", Extension = new[] { "json", "txt" }, Icon = "AtomVariant" };
            //Molecule based
            yield return new ExportFileType() { Title = "Molecule", Extension = new[] { "ixyz" }, Icon = "Molecule" };
            yield return new ExportFileType() { Title = "Macrocycle", Extension = new[] { "ixyz" }, Icon = "Molecule" };

        }
        /// <summary>
        /// Gets List of available ExportFileTypes
        /// </summary>
        public IEnumerable<ExportFileType> AvailableFileTypes => CheckFileTypes();
    }
}

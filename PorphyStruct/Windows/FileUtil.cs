using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Windows.Controls;

namespace PorphyStruct.Windows
{
    public static class FileUtil
    {
        /// <summary>
        /// Returns the default openfiledialog
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="export">indicates whether you're im- or exporting files</param>
        /// <returns></returns>
        public static OpenFileDialog DefaultOpenFileDialog(string filter, bool export = false)
        {
            //setup inital dir
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //if export -> use savepath as default, else use import path
            if (!string.IsNullOrEmpty(PorphyStruct.Core.Properties.Settings.Default.savePath) && !PorphyStruct.Core.Properties.Settings.Default.useImportExportPath && export)
                initialDir = PorphyStruct.Core.Properties.Settings.Default.savePath;
            else if (!string.IsNullOrEmpty(PorphyStruct.Core.Properties.Settings.Default.importPath))
                initialDir = PorphyStruct.Core.Properties.Settings.Default.importPath;
            return new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = filter,
                RestoreDirectory = true
            };
        }

        /// <summary>
        /// Returns default png exporter
        /// </summary>
        public static PngExporter PngExporter => new PngExporter()
        {
            Height = Core.Properties.Settings.Default.pngHeight,
            Width = Core.Properties.Settings.Default.pngWidth,
            Resolution = Core.Properties.Settings.Default.pngRes,
            Background = Core.Properties.Settings.Default.backgroundColor ? OxyColors.White : OxyColors.Transparent
        };

        /// <summary>
        /// Returns default svg exporter
        /// </summary>
        public static OxyPlot.Wpf.SvgExporter SvgExporter => new OxyPlot.Wpf.SvgExporter()
        {
            Height = Core.Properties.Settings.Default.pngHeight,
            Width = Core.Properties.Settings.Default.pngWidth,
            TextMeasurer = new CanvasRenderContext(new Canvas()),
            IsDocument = true
        };
    }
}

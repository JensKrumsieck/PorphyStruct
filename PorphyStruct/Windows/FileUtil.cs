using Microsoft.Win32;
using System;

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
            if (!String.IsNullOrEmpty(PorphyStruct.Core.Properties.Settings.Default.savePath) && !PorphyStruct.Core.Properties.Settings.Default.useImportExportPath && export)
                initialDir = PorphyStruct.Core.Properties.Settings.Default.savePath;
            else if (!String.IsNullOrEmpty(PorphyStruct.Core.Properties.Settings.Default.importPath))
                initialDir = PorphyStruct.Core.Properties.Settings.Default.importPath;
            return new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = filter,
                RestoreDirectory = true
            };
        }
    }
}

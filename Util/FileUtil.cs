using Microsoft.Win32;
using System;

namespace PorphyStruct.Util
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
            if (!String.IsNullOrEmpty(Properties.Settings.Default.savePath) && !Properties.Settings.Default.useImportExportPath && export)
                initialDir = Properties.Settings.Default.savePath;
            else if (!String.IsNullOrEmpty(Properties.Settings.Default.importPath))
                initialDir = Properties.Settings.Default.importPath;
            return  new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = filter,
                RestoreDirectory = true
            };
        }
    }
}

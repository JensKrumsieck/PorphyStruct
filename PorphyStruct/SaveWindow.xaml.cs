using PorphyStruct.ViewModel;
using PorphyStruct.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using winforms = System.Windows.Forms;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        public MainViewModel MainVM { get; set; }

        public SaveViewModel viewModel { get; set; }

        public SaveWindow()
        {
            InitializeComponent();
            MainVM = Application.Current.Windows.OfType<MainWindow>().First().viewModel;
            viewModel = new SaveViewModel(MainVM.Cycle);
            DataContext = viewModel;
        }

        public bool Validate
        {
            get
            {
                if (string.IsNullOrEmpty(viewModel.Path) || !Directory.Exists(viewModel.Path)) return false;
                if (TypeList.SelectedItems.Count == 0) return false;
                return true;
            }
        }

        /// <summary>
        /// Handles file saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate) MessageBox.Show("Validation failed. Check whether data is present, a path is given or a export routine is selected.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);

            foreach (ExportFileType t in TypeList.SelectedItems) viewModel.Export(t);

            //open folder
            var info = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = viewModel.Path
            };
            Process.Start(info);

            //close form
            Close();
        }

        /// <summary>
        /// handles search button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!string.IsNullOrEmpty(PorphyStruct.Core.Properties.Settings.Default.savePath) && !PorphyStruct.Core.Properties.Settings.Default.useImportExportPath)
                initialDir = PorphyStruct.Core.Properties.Settings.Default.savePath;
            else if (!string.IsNullOrEmpty(PorphyStruct.Core.Properties.Settings.Default.importPath))
                initialDir = PorphyStruct.Core.Properties.Settings.Default.importPath;
            using (var fbd = new winforms.FolderBrowserDialog
            {
                SelectedPath = initialDir
            })
            {
                if (fbd.ShowDialog() == winforms.DialogResult.OK) viewModel.Path = fbd.SelectedPath;
            }
        }
    }
}

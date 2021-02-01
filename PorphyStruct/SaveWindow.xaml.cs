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
        public MainViewModel MainVm { get; set; }

        public SaveViewModel ViewModel { get; set; }

        public SaveWindow()
        {
            InitializeComponent();
            MainVm = Application.Current.Windows.OfType<MainWindow>().First().viewModel;
            ViewModel = new SaveViewModel(MainVm.Cycle);
            DataContext = ViewModel;
        }

        public bool Validate =>
            !string.IsNullOrEmpty(ViewModel.Path) && Directory.Exists(ViewModel.Path) &&
            TypeList.SelectedItems != null &&
            TypeList.SelectedItems.Count != 0;

        /// <summary>
        /// Handles file saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate)
                MessageBox.Show("Validation failed. Check whether data is present, a path is given or a export routine is selected.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);

            foreach (ExportFileType t in TypeList.SelectedItems) ViewModel.Export(t);

            //open folder
            var info = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = ViewModel.Path
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
            if (!string.IsNullOrEmpty(Core.Properties.Settings.Default.savePath) && !Core.Properties.Settings.Default.useImportExportPath)
                initialDir = Core.Properties.Settings.Default.savePath;
            else if (!string.IsNullOrEmpty(Core.Properties.Settings.Default.importPath))
                initialDir = Core.Properties.Settings.Default.importPath;
            using var fbd = new winforms.FolderBrowserDialog
            {
                SelectedPath = initialDir
            };
            if (fbd.ShowDialog() == winforms.DialogResult.OK) ViewModel.Path = fbd.SelectedPath;
        }
    }
}

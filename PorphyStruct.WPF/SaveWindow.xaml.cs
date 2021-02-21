using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Win32;
using PorphyStruct.Core;
using ThemeCommons.Controls;

namespace PorphyStruct.WPF
{
    /// <summary>
    /// Interaktionslogik für SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : DefaultWindow, INotifyPropertyChanged
    {
        public AnalysisViewModel ViewModel;

        public List<ExportFileType> AvailableFileTypes { get; } = new List<ExportFileType>
        {
            new ExportFileType("Graph", "ChartScatterPlot", "png"),
            new ExportFileType("Graph", "ChartScatterPlotHexBin", "svg"),
            new ExportFileType("Analysis", "AtomVariant", "md"),
            new ExportFileType("Analysis", "CodeJson", "json"),
            new ExportFileType("XYData", "MicrosoftExcel", "csv"),
            new ExportFileType("XYData", "TableLarge", "dat"),
            new ExportFileType("Molecule", "Molecule", "mol2"),
            new ExportFileType("Macrocycle", "Molecule", "mol2")
        };

        public SaveWindow(AnalysisViewModel viewModel)
        {
            InitializeComponent();
            DataContext = ViewModel = viewModel;
            Filename = InitialDir;
        }

        private string _filename;
        public string Filename
        {
            get => _filename;
            set
            {
                _filename = value; 
                OnPropertyChanged();
            }
        }

        private void SaveWindow_OnLoaded(object sender, RoutedEventArgs e) => TypeList.SelectAll();

        private void Cancel_OnClick(object sender, RoutedEventArgs e) => Close();

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ExportFileType t in TypeList.SelectedItems) ViewModel.Export(t, Filename);
            Close();
        }

        private void Search_OnClick(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                FileName = InitialDir
            };
            if (sfd.ShowDialog(this) != true) return;
            Filename = sfd.FileName;
        }

        private string InitialDir => string.IsNullOrEmpty(Settings.Instance.DefaultExportPath) ? Path.ChangeExtension(ViewModel.Parent.Filename, null) : Settings.Instance.DefaultExportPath + ViewModel.Title;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

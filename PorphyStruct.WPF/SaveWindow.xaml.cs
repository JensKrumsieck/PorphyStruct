using Microsoft.Win32;
using PorphyStruct.Core;
using PorphyStruct.ViewModel;
using PorphyStruct.ViewModel.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ThemeCommons.Controls;

namespace PorphyStruct.WPF
{
    public partial class SaveWindow : DefaultWindow, INotifyPropertyChanged
    {
        public AnalysisViewModel ViewModel;

        public static List<ExportFileType> AvailableFileTypes { get; } = new List<ExportFileType>
        {
            new ExportFileType("Graph", "ChartScatterPlot", "png"), //0
            new ExportFileType("Graph", "ChartScatterPlotHexBin", "svg"), //1

            new ExportFileType("Analysis", "AtomVariant", "md"), //2
            new ExportFileType("Analysis", "CodeJson", "json"), //3
            new ExportFileType("Analysis", "ChartBox", "png"), //4
            new ExportFileType("Analysis", "ChartBox", "svg"), //5

            new ExportFileType("XYData", "MicrosoftExcel", "csv"), //6
            new ExportFileType("XYData", "TableLarge", "dat"), //7

            new ExportFileType("Molecule", "Molecule", "mol2"), //8
            new ExportFileType("Macrocycle", "Molecule", "mol2"), //9

            new ExportFileType("Viewport", "ChartScatterPlot", "png") //10
        };

        public SaveWindow(AnalysisViewModel viewModel)
        {
            InitializeComponent();
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
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

        private void SaveWindow_OnLoaded(object sender, RoutedEventArgs e) =>
            SelectItems(new[] {0, 1, 2, 3, 4, 5, 6}); //select recommended indices

        private void Cancel_OnClick(object sender, RoutedEventArgs e) => Close();

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ExportFileType t in TypeList.SelectedItems)
            {
                if (t.Title == "Viewport")
                    ViewModel.ExportViewport(Filename);
                else ViewModel.Export(t, Filename);
            }
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
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void SelectItems(int[] ids)
        {
            foreach (var id in ids)
                TypeList.SelectedItems.Add(TypeList.ItemsSource.Cast<ExportFileType>().ElementAt(id));
        }
    }
}

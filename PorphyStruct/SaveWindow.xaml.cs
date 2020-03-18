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
        /// Saves the Sim result as plot
        /// leave this here until gui is capable of showing this in the first place
        /// </summary>
        /// <param name="extension"></param>
        private void SaveSimResult(string Extension)
        {
            //if (Sim == null)
            //{
            //    MessageBox.Show("No Simulation present!", "Data Empty", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            ////copied from 
            //PlotModel pm = new PlotModel()
            //{
            //    IsLegendVisible = false,
            //    LegendPosition = LegendPosition.RightTop,
            //    DefaultFontSize = PorphyStruct.Core.Properties.Settings.Default.defaultFontSize,
            //    LegendFontSize = PorphyStruct.Core.Properties.Settings.Default.defaultFontSize,
            //    DefaultFont = PorphyStruct.Core.Properties.Settings.Default.defaultFont,
            //    PlotAreaBorderThickness = new OxyThickness(PorphyStruct.Core.Properties.Settings.Default.lineThickness)
            //};

            //OxyPlot.Axes.LinearAxis x = new OxyPlot.Axes.LinearAxis
            //{
            //    Title = "",
            //    Unit = "%",
            //    Position = OxyPlot.Axes.AxisPosition.Bottom,
            //    Key = "X",
            //    IsAxisVisible = true,
            //    MajorGridlineThickness = PorphyStruct.Core.Properties.Settings.Default.lineThickness,
            //    TitleFormatString = "{1}"
            //};

            //OxyPlot.Axes.CategoryAxis y = new OxyPlot.Axes.CategoryAxis
            //{
            //    IsAxisVisible = false,
            //    Position = OxyPlot.Axes.AxisPosition.Left
            //};

            //if (!PorphyStruct.Core.Properties.Settings.Default.showBox)
            //{
            //    pm.PlotAreaBorderThickness = new OxyThickness(0);

            //    x.AxislineStyle = LineStyle.Solid;
            //    x.AxislineThickness = PorphyStruct.Core.Properties.Settings.Default.lineThickness;
            //}

            //pm.Axes.Add(x);
            //pm.Axes.Add(y);

            ////add data;
            //OxyPlot.Series.IntervalBarSeries s = new OxyPlot.Series.IntervalBarSeries();
            //int a = 0;
            //foreach (KeyValuePair<string, double> i in Sim.par.Reverse())
            //{
            //    OxyPlot.Series.IntervalBarItem item = new OxyPlot.Series.IntervalBarItem
            //    {
            //        Start = (i.Value < 0 ? i.Value : 0),
            //        End = (i.Value < 0 ? 0 : i.Value),
            //        Title = i.Key,
            //        CategoryIndex = a,
            //        Color = OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color2)
            //    };
            //    s.Items.Add(item);
            //    a++;
            //}
            //pm.Series.Add(s);

            //pm.Annotations.Add(new OxyPlot.Annotations.LineAnnotation()
            //{
            //    Color = OxyColors.Black,
            //    StrokeThickness = PorphyStruct.Core.Properties.Settings.Default.lineThickness,
            //    Type = OxyPlot.Annotations.LineAnnotationType.Vertical,
            //    X = 0.0
            //});

            //pm.InvalidatePlot(true);

            ////save image
            //if (Extension == "png")
            //    PngExporter.Export(pm, Filename + "SimResult.png", PorphyStruct.Core.Properties.Settings.Default.pngWidth, PorphyStruct.Core.Properties.Settings.Default.pngHeight, OxyColors.Transparent, PorphyStruct.Core.Properties.Settings.Default.pngRes);

            ////exports svg
            //if (Extension == "svg")
            //{
            //    var svg = new OxyPlot.Wpf.SvgExporter()
            //    {
            //        Width = PorphyStruct.Core.Properties.Settings.Default.pngWidth,
            //        Height = PorphyStruct.Core.Properties.Settings.Default.pngHeight
            //    };
            //    svg.ExportToFile(pm, Filename + "SimResult.svg");
            //}

        }

        /// <summary>
        /// handles search button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!String.IsNullOrEmpty(PorphyStruct.Core.Properties.Settings.Default.savePath) && !PorphyStruct.Core.Properties.Settings.Default.useImportExportPath)
                initialDir = PorphyStruct.Core.Properties.Settings.Default.savePath;
            else if (!String.IsNullOrEmpty(PorphyStruct.Core.Properties.Settings.Default.importPath))
                initialDir = PorphyStruct.Core.Properties.Settings.Default.importPath;
            using (winforms.FolderBrowserDialog fbd = new winforms.FolderBrowserDialog
            {
                SelectedPath = initialDir
            })
            {
                if (fbd.ShowDialog() == winforms.DialogResult.OK) viewModel.Path = fbd.SelectedPath;
            }
        }
    }
}

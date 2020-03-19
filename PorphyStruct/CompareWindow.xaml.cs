using PorphyStruct.Chemistry.Data;
using PorphyStruct.Util;
using PorphyStruct.ViewModel;
using PorphyStruct.Windows;
using System.Linq;
using System.Windows;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für CompareWindow.xaml
    /// </summary>
    public partial class CompareWindow : Window
    {
        //ViewModel of MainWindow
        private readonly MainViewModel MainVM;

        public CompareViewModel viewModel;

        public CompareWindow()
        {
            InitializeComponent();
            //get MainViewModel to get info about cycle
            MainVM = Application.Current.Windows.OfType<MainWindow>().First().viewModel;
            //build model
            viewModel = new CompareViewModel(MainVM.Cycle);
            DataContext = viewModel;
            CompList.ItemsSource = viewModel.Cycle.DataProviders;
            CompList.Items.Filter = ComparisonFilter;
        }

        /// <summary>
        /// Applies a filter to itemview
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool ComparisonFilter(object item)
        {
            IAtomDataPointProvider provider = item as IAtomDataPointProvider;
            if (provider.DataType == DataType.Comparison) return true;
            return false;
        }

        /// <summary>
        /// Handles Button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var ofd = FileUtil.DefaultOpenFileDialog("ASCII Files(DAT)(*.dat) | *.dat", true);
            var DialogResult = ofd.ShowDialog();

            if (DialogResult.HasValue && DialogResult.Value)
            {
                comparisonPath.Text = ofd.FileName;
                viewModel.LoadData(ofd.FileName);

                CompPlot.Model = viewModel.Model;
            }

        }

        /// <summary>
        /// Add Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.Current != null) viewModel.Cycle.DataProviders.Add(viewModel.Current);
        }

        /// <summary>
        /// Remove Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAll_Click(object sender, RoutedEventArgs e) => viewModel.Cycle.DataProviders.RemoveAll(s => s.DataType == DataType.Comparison);
    }
}

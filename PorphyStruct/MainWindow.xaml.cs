using HelixToolkit.Wpf;
using MathNet.Spatial.Euclidean;
using PorphyStruct.Chemistry;
using PorphyStruct.ViewModel;
using PorphyStruct.Windows;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel viewModel;

        public MainWindow()
        {
            //CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
        }

        /// <summary>
        /// Analyze current Macrocycle and print result to PlotView
        /// </summary>
        public void Analyze()
        {
            //set readonly to get away from the add/edit error
            coordGrid.IsReadOnly = !coordGrid.IsReadOnly;
            viewModel.Analyze();
            displaceView.Model = viewModel.Model;
            //enable simulations
            SimButton.IsEnabled = true;
            coordGrid.IsReadOnly = !coordGrid.IsReadOnly;
        }
        /// <summary>
        /// Update Properties
        /// </summary>
        public void UpdateStack()
        {
            var flattened = viewModel.CycleProperties.SelectMany(x => x.Value?.Select(y => new { x.Key, y.Name, y.Value })).ToList();
            Cycle_Properties.ItemsSource = flattened;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Cycle_Properties.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Key");
            view.GroupDescriptions.Add(groupDescription);
        }

        /// <summary>
        /// Centers Molecule into origin and updates camera to bestview
        /// </summary>
        public void Center()
        {
            //Center Molecule
            viewModel.Cycle.Center(s => s.IsMacrocycle);
            //update camera
            Plane pl = viewModel.Cycle.GetMeanPlane();
            var normal = new System.Windows.Media.Media3D.Point3D(pl.A, pl.B, pl.C);
            while (normal.DistanceTo(Win32Util.Origin) < 25) normal = normal.Multiply(1.1);
            MolViewer.CameraController.CameraPosition = normal;
            MolViewer.CameraController.CameraTarget = Win32Util.Origin;
        }

        /// <summary>
        /// Handle Open File Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            //open new wizard window
            Wizard wi = new Wizard();
            wi.ShowDialog();
            if (wi.DialogResult.HasValue && wi.DialogResult.Value)
            {
                displaceView.Background = Brushes.White;

                //drive the reset train
                foreach (var child in this.FindVisualChildren<Button>())
                    child.IsEnabled = !child.Name.Contains("Sim");

                viewModel = new MainViewModel(wi.FileName, (Macrocycle.Type)wi.Type);
                DataContext = viewModel;
                viewModel.PropertyChanged += ViewModel_PropertyChanged;

                Title = $"Structural Analysis of Porphyrinoids (PorphyStruct) - {viewModel.Type} Mode";

                //clear plotview
                displaceView.Model = null;
                displaceView.InvalidatePlot();

                //set properties initially
                viewModel.UpdateProperties();
            }
        }

        /// <summary>
        /// Check for changed properties that are not tracked by bindings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(viewModel.CycleProperties):
                    UpdateStack();
                    break;
            }
        }

        /// <summary>
        /// Handle Analyze Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Analyze_Click(object sender, RoutedEventArgs e) => Analyze();

        /// <summary>
        /// Handle Center Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CenterMolButton_Click(object sender, RoutedEventArgs e) => Center();


        /// <summary>
        /// Handle Normalize Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NormalizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!NormalizeButton.IsEnabled) return;
            viewModel.Normalize = !viewModel.Normalize;
            Analyze();
        }

        /// <summary>
        /// Handle Save Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e) { if (SaveButton.IsEnabled) new SaveWindow(viewModel.Cycle, viewModel.simulation).ShowDialog(); }

        /// <summary>
        /// Handle Invert Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (!InvertButton.IsEnabled) return;
            viewModel.Invert = !viewModel.Invert;
            Analyze();
        }

        /// <summary>
        /// Handle Simulate Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimButton_Click(object sender, RoutedEventArgs e)
        {

            //drop metal data
            if (viewModel.Cycle.HasMetal(true))
                viewModel.Cycle.Atoms.Where(s => s.IsMacrocycle && s.IsMetal).ToList().ForEach(s => s.IsMacrocycle = false);
            //normalize if not done yet!
            if (!viewModel.Normalize)
                NormalizeButton_Click(sender, e);

            new SimWindow().Show();
        }

        /// <summary>
        /// Handle Settings Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButton_Click(object sender, RoutedEventArgs e) => new Settings().ShowDialog();

        /// <summary>
        /// Handle Delete Simulation Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelSimButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.simulation = null;
            DelSimButton.IsEnabled = false;
            viewModel.HasDifference = false;
            DiffSimButton.IsEnabled = false;
            Analyze();
        }
        /// <summary>
        /// Handle Diff Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffSimButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.HasDifference = !viewModel.HasDifference;
            Analyze();
        }

        /// <summary>
        /// Handle Info Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoButton_Click(object sender, RoutedEventArgs e) => new Info().Show();


        /// <summary>
        /// handle compare button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompButton_Click(object sender, RoutedEventArgs e)
        {
            Analyze();
            CompareWindow cw = new CompareWindow();
            cw.comparison1Path.Text = viewModel.comp1Path;
            cw.comparison2Path.Text = viewModel.comp2Path;
            cw.ShowDialog();
            if (cw.DialogResult.HasValue && cw.DialogResult.Value)
            {
                viewModel.comp1Path = cw.comparison1Path.Text;
                viewModel.comp2Path = cw.comparison2Path.Text;
            }
            else
            {
                viewModel.comp1Path = "";
                viewModel.comp2Path = "";
            }
            Analyze();
        }

        /// <summary>
        /// Detect Macrocycle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Detect_Click(object sender, RoutedEventArgs e)
        {
            //Block UI Interaction during Detect
            IsEnabled = false;
            await Task.Run(viewModel.Cycle.Detect);
            IsEnabled = true;
        }
    }
}
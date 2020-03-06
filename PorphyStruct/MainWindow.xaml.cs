using HelixToolkit.Wpf;
using MaterialDesignThemes.Wpf;
using MathNet.Spatial.Euclidean;
using OxyPlot.Series;
using PorphyStruct.Chemistry;
using PorphyStruct.Util;
using PorphyStruct.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Macrocycle cycle;
        private Macrocycle old;
        public double normFac = 0;
        private int oldIndex = -1;
        private bool _hasDifference, _invert, _normalize;

        /// <summary>
        /// Handles Normalisation
        /// </summary>
        public bool Normalize
        {
            get => _normalize;
            set {
                _normalize = value;
                NotifyPropertyChanged(nameof(Normalize));
            }
        }

        /// <summary>
        /// Handles Invert
        /// </summary>
        public bool Invert
        {
            get => _invert;
            set
            {
                _invert = value;
                NotifyPropertyChanged(nameof(Invert));
            }
        }

        /// <summary>
        /// Handles Difference
        /// </summary>
        public bool HasDifference
        {
            get => _hasDifference;
            set
            {
                _hasDifference = value;
            }
        }

        public bool HasComparison => !string.IsNullOrEmpty(comp1Path) || !string.IsNullOrEmpty(comp2Path);

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Simulation simulation = null;
        public Macrocycle.Type type = Macrocycle.Type.Corrole;

        public string comp1Path, comp2Path, path;
        public string FileName {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        public static DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Analyze current Macrocycle and print result to PlotView
        /// </summary>
        public void Analyze() {
            //set up default plot model
            OxyPlotOverride.StandardPlotModel pm = new OxyPlotOverride.StandardPlotModel();

            //do analysis
            cycle.GetDataPoints();

            //normalisation
            if (Normalize)
            {
                normFac = MathUtil.GetNormalizationFactor(cycle.dataPoints);
                cycle.dataPoints = cycle.dataPoints.Normalize();
            }
            //invert
            if (Invert) cycle.dataPoints = cycle.dataPoints.Invert();

            //handle sim
            if (simulation != null)
            {
                simulation.Normalize(Normalize, normFac);
                simulation.Invert(Invert);
                simulation.Paint(pm);
            }
            //paint difference
            if (HasDifference) cycle.GetDifference(simulation).Paint(pm, "Diff");
            //paint comparison
            if (!String.IsNullOrEmpty(comp1Path)) CompareWindow.GetData(comp1Path).Paint(pm, "Com1");
            if (!String.IsNullOrEmpty(comp2Path)) CompareWindow.GetData(comp2Path).Paint(pm, "Com2");
            //paint exp
            cycle.Paint(pm, MacrocyclePainter.PaintMode.Exp);

            //handle dont mark
            foreach (ScatterSeries s in pm.Series) ((List<AtomDataPoint>)s.ItemsSource).Where(dp => Core.Properties.Settings.Default.dontMark.Split(',').Contains(dp.atom.Identifier) || Core.Properties.Settings.Default.dontMark.Split(',').Contains(dp.atom.Type)).ToList().ForEach(dp => dp.Size = 0);

            displaceView.Model = pm;
            pm.Scale(pm.yAxis, true, Normalize);
            pm.Scale(pm.xAxis);
            //update simstack
            UpdateStack();
        }
        /// <summary>
        /// Update Sim WrapPanel
        /// </summary>
        public void UpdateStack()
        {
            simStack.Children.Clear();
            if (simulation != null)
                foreach (string key in simulation.par.Keys) simStack.Children.Add(new Chip
                {
                    Content = key + ": " + simulation.par[key].ToString(System.Globalization.CultureInfo.InvariantCulture) + "%",
                    Margin = new Thickness(0, 0, 4, 4),
                    FontSize = 8
                });

            if (coordGrid.ItemsSource != null)
            {
                //update plane coordinates
                Plane pl = cycle.GetMeanPlane();
                UnitVecTB.Text = $"({pl.A.ToString("G3")}, {pl.B.ToString("G3")}, {pl.C.ToString("G3")})";
                DistTB.Text = pl.D.ToString("G3");
            }
        }


        /// <summary>
        /// Update 3D Model
        /// </summary>
        /// <param name="markSelection"></param>
        /// <param name="force"></param>
        private void UpdateMolView(bool markSelection = false, bool force = false)
        {
            if ((old != null && !cycle.Atoms.SequenceEqual(old.Atoms)) || (markSelection && oldIndex != coordGrid.SelectedIndex) || force)
            {
                old = cycle;
                oldIndex = coordGrid.SelectedIndex;
                MolViewer.Children.Clear();
                MolViewer.Children.Add(new DefaultLights());
                Atom selected = markSelection ? (Atom)coordGrid.SelectedItem : null;

                //create 3d model
                System.Windows.Media.Media3D.ModelVisual3D model = new System.Windows.Media.Media3D.ModelVisual3D() { Content = cycle.Paint3D(selected) };
                MolViewer.Children.Add(model);
            }
        }


        /// <summary>
        /// Centers Molecule into origin and updates camera to bestview
        /// </summary>
        public void Center()
        {
            //Center Molecule
            cycle.Center(s => s.IsMacrocycle);

            //update list
            coordGrid.ItemsSource = cycle.Atoms.OrderByDescending(s => s.IsMacrocycle).ToList();
            coordGrid.Items.Refresh();
            //update 3d image
            this.UpdateMolView(false, true);
            //update camera
            Plane pl = cycle.GetMeanPlane();
            var normal = new System.Windows.Media.Media3D.Point3D(pl.A, pl.B, pl.C);
            while (normal.DistanceTo(Win32Util.Origin) < 25) normal = normal.Multiply(1.1);
            MolViewer.CameraController.CameraPosition = normal;
            MolViewer.CameraController.CameraTarget = Win32Util.Origin;
        }

        /// <summary>
        /// Handle Updated Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordGrid_Updated(object sender, EventArgs e) => this.UpdateMolView(true);


        /// <summary>
        /// Handle Window Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //subscribe to event
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            if (dpd != null) dpd.AddValueChanged(coordGrid, CoordGrid_Updated);
        }

        /// <summary>
        /// Handle Refresh Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, RoutedEventArgs e) { if (InvertButton.IsEnabled) UpdateMolView(false, true); }


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

                path = wi.FileName;
                type = (Macrocycle.Type)wi.Type;
                this.Title = "Structural Analysis of Porphyrinoids (PorphyStruct) - " + type.ToString() + " Mode";

                //reset gui elements
                AnalButton.IsEnabled = RefMolButton.IsEnabled = CenterMolButton.IsEnabled = DetectMolButton.IsEnabled = NormalizeButton.IsEnabled = InvertButton.IsEnabled = SaveButton.IsEnabled = SimButton.IsEnabled = CompButton.IsEnabled = true;
                Normalize = Invert = HasDifference = DelSimButton.IsEnabled = DiffSimButton.IsEnabled = false;

                //clear plotview
                displaceView.Model = null;
                displaceView.InvalidatePlot();

                //clear sim
                this.simulation = null;
                //update simstack
                UpdateStack();

                //reset values
                normFac = 0;
                oldIndex = -1;
                comp1Path = comp2Path = "";

                cycle = MacrocycleFactory.Load(path, type);
                FileName = cycle.Title;
                //add to grid
                coordGrid.ItemsSource = cycle.Atoms.OrderByDescending(s => s.IsMacrocycle).ToList();
            }
        }

        /// <summary>
        /// Handle Analyze Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            if (!AnalButton.IsEnabled) return;
            //get the current data from source
            cycle.Atoms = ((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.IsMacrocycle).ToList();
            //TODO: validate cycle here
            //call analyze void
            Analyze();
        }

        /// <summary>
        /// Handle Center Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CenterMolButton_Click(object sender, RoutedEventArgs e) => this.Center();


        /// <summary>
        /// Handle Normalize Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NormalizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!NormalizeButton.IsEnabled) return;
            Normalize = !Normalize;
            this.Analyze();
        }

        /// <summary>
        /// Handle Save Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e) { if (SaveButton.IsEnabled) new SaveWindow(cycle, simulation).ShowDialog(); }

        /// <summary>
        /// Handle Invert Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (!InvertButton.IsEnabled) return;
            Invert = !Invert;
            this.Analyze();
        }

        /// <summary>
        /// Handle Simulate Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimButton_Click(object sender, RoutedEventArgs e)
        {
            //normalize if not done yet!
            if (!Normalize)
                NormalizeButton_Click(sender, e);
            if (simulation != null)
            {
                new SimWindow(cycle, displaceView, simulation).Show();
            }
            else new SimWindow(cycle, displaceView).Show();
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
            this.simulation = null;
            DelSimButton.IsEnabled = false;
            HasDifference = false;
            DiffSimButton.IsEnabled = false;
            this.Analyze();
        }

        /// <summary>
        /// Handle Diff Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffSimButton_Click(object sender, RoutedEventArgs e)
        {
            HasDifference = !HasDifference;
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
            this.Analyze();
            CompareWindow cw = new CompareWindow();
            cw.comparison1Path.Text = comp1Path;
            cw.comparison2Path.Text = comp2Path;
            cw.ShowDialog();
            if (cw.DialogResult.HasValue && cw.DialogResult.Value)
            {
                comp1Path = cw.comparison1Path.Text;
                comp2Path = cw.comparison2Path.Text;
                NotifyPropertyChanged(nameof(HasComparison));
            }
            else
            {
                comp1Path = "";
                comp2Path = "";
                NotifyPropertyChanged(nameof(HasComparison));
            }
            this.Analyze();
        }

        /// <summary>
        /// Detect Macrocycle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Detect_Click(object sender, RoutedEventArgs e)
        {
            cycle.Detect();
            //update 3d image
            this.UpdateMolView();
            coordGrid.Items.Refresh();
        }
    }
}

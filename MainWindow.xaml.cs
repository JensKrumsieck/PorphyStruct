using HelixToolkit.Wpf;
using MaterialDesignThemes.Wpf;
using MathNet.Spatial.Euclidean;
using OxyPlot;
using OxyPlot.Series;
using PorphyStruct.Chemistry;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
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
        public Macrocycle cycle;
        private Macrocycle old;
        public string path = "";
        public double normFac = 0;
        private int oldIndex = -1;

        public bool normalize = false;
        private bool hasDifference = false;
        public bool invert = false;

        public Simulation simulation = null;
        public Macrocycle.Type type = Macrocycle.Type.Corrole;


        string comp1Path = "";
        string comp2Path = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Analyze current Macrocycle and print result to PlotView
        /// </summary>
        public void Analyze()
        {
            //set up default plot model
            Oxy.Override.StandardPlotModel pm = new Oxy.Override.StandardPlotModel();
            Oxy.Override.LinearAxis y = pm.yAxis;
            cycle.GetDataPoints();

            //normalisation
            if (normalize)
            {
                this.normFac = MathUtil.GetNormalizationFactor(cycle.dataPoints);
                cycle.dataPoints = cycle.dataPoints.Normalize();
                if(simulation != null && !simulation.isNormalized)
                {
                    simulation.cycle.dataPoints = simulation.cycle.dataPoints.Normalize();
                    simulation.isNormalized = true;
                }
            }
            else
            {
                if (simulation != null && simulation.isNormalized)
                {
                    simulation.cycle.dataPoints = simulation.cycle.dataPoints.Factor(1 / this.normFac).ToList();
                    simulation.isNormalized = false;
                }
            }

            //invert
            if (invert)
            {
                cycle.dataPoints = cycle.dataPoints.Invert();
                if (simulation != null && !simulation.isInverted)
                {
                    simulation.cycle.dataPoints = simulation.cycle.dataPoints.Invert();
                    simulation.isInverted = true;
                }
            }
            else if (simulation != null && simulation.isInverted)
            {
                simulation.cycle.dataPoints = simulation.cycle.dataPoints.Invert();
                simulation.isInverted = false;
            }

            if (simulation != null) simulation.Paint(pm);
            if (hasDifference) GetDifference(cycle, simulation).Paint(pm, "Diff");

            MacrocyclePainter.Paint(pm, cycle, MacrocyclePainter.PaintMode.Exp);

            if (!String.IsNullOrEmpty(comp1Path)) CompareWindow.GetData(comp1Path).Paint(pm, "Com1");           
            if (!String.IsNullOrEmpty(comp2Path)) CompareWindow.GetData(comp2Path).Paint(pm, "Com2");

            //handle dont mark
            foreach (ScatterSeries s in pm.Series)
            {
                string[] dontMark = Properties.Settings.Default.dontMark.Split(',');
                List<AtomDataPoint> data = (List<AtomDataPoint>)s.ItemsSource;
                data.Where(dp => dontMark.Contains(dp.atom.Identifier) || dontMark.Contains(dp.atom.Type)).ToList().ForEach(dp => dp.Size = 0);
            } 

            displaceView.Model = pm;
            pm.InvalidatePlot(true);
                       
            //scale if neccessary
            if (!normalize)
            {
                pm.Scale(pm.yAxis, true);
            }
            else
            {
                double min = -1.1;
                double max = 1.1;
                y.Zoom(min, max);
                y.AbsoluteMinimum = min;
                y.AbsoluteMaximum = max;
            }
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
            {
                foreach (string key in simulation.par.Keys)
                {
                    Chip c = new Chip
                    {
                        Content = key + ": " + simulation.par[key].ToString(System.Globalization.CultureInfo.InvariantCulture) + "%",
                        Margin = new Thickness(0, 0, 4, 4),
                        FontSize = 8
                    };
                    simStack.Children.Add(c);
                }
            }
            if (coordGrid.ItemsSource != null)
            {
                //update plane coordinates
                Plane pl = cycle.GetMeanPlane();
                UnitVecTB.Text = "(" + pl.A.ToString("G3") + "," + pl.B.ToString("G3") + "," + pl.C.ToString("G3") + ")";
                DistTB.Text = pl.D.ToString("G3");
            }
        }     


        /// <summary>
        /// Update 3D Model
        /// </summary>
        /// <param name="markSelection"></param>
        /// <param name="force"></param>
        private void UpdateMolView(bool markSelection = false, bool force = false, bool detect = false)
        {
            if (detect)
                cycle.Detect();

            if ((old != null && !cycle.Atoms.SequenceEqual(old.Atoms)) || (markSelection && oldIndex != coordGrid.SelectedIndex) || force)
            {
                old = cycle;
                oldIndex = coordGrid.SelectedIndex;
                MolViewer.Children.Clear();
                MolViewer.Children.Add(new DefaultLights());
                Atom selected = markSelection ? (Atom)coordGrid.SelectedItem : null;

                //create 3d model
                System.Windows.Media.Media3D.ModelVisual3D model = new System.Windows.Media.Media3D.ModelVisual3D();
                model.Content = MacrocyclePainter.Paint3D(cycle, selected);
                MolViewer.Children.Add(model);
            }
        }

        /// <summary>
        /// returns difference between exp and sim
        /// </summary>
        /// <returns></returns>
        public Simulation GetDifference(Macrocycle cycle, Simulation sim)
        {
            Macrocycle difference = (Macrocycle)cycle.Clone();
            difference.dataPoints = cycle.dataPoints.Where(s => !s.atom.IsMetal).Difference(sim.cycle.dataPoints).ToList();
            return new Simulation(difference);
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
            while (normal.DistanceTo(MathUtil.Origin) < 25) normal = normal.Multiply(1.1);
            MolViewer.CameraController.CameraPosition = normal;
            MolViewer.CameraController.CameraTarget = MathUtil.Origin;
        }

        #region UI Interaction
        /// <summary>
        /// Handle Updated Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordGrid_Updated(object sender, EventArgs e) => this.UpdateMolView();


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
        private void Refresh_Click(object sender, RoutedEventArgs e) => this.UpdateMolView(false, true);

        /// <summary>
        /// Handle Selection Changed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) => this.UpdateMolView(true);

        /// <summary>
        /// Handle Target Updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordGrid_TargetUpdated(object sender, DataTransferEventArgs e) => this.UpdateMolView(true);

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
                normalize = invert = hasDifference = DelSimButton.IsEnabled = DiffSimButton.IsEnabled = false;
                NormalizeButton.Foreground = InvertButton.Foreground = DiffSimButton.Foreground = CompButton.Foreground = Brushes.Black;

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

                //reset comparison
                comp1Path = comp2Path = "";

                cycle = MacrocycleFactory.Load(path, type);
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
            //get the current data from source
            cycle.Atoms = ((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.IsMacrocycle).ToList();
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
            //set false
            if (this.normalize)
            {
                this.normalize = false;
                this.NormalizeButton.Foreground = Brushes.Black;
            }
            else
            {
                this.normalize = true;
                this.NormalizeButton.Foreground = Brushes.CornflowerBlue;
            }
            //reanalyze
            this.Analyze();
        }

        /// <summary>
        /// Handle Save Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //open save dialog
            SaveWindow svW = new SaveWindow(cycle, this.simulation);
            svW.ShowDialog();
        }


        /// <summary>
        /// Handle Invert Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvertButton_Click(object sender, RoutedEventArgs e)
        {
            //set false
            if (this.invert)
            {
                this.invert = false;
                this.InvertButton.Foreground = Brushes.Black;
            }
            else
            {
                this.invert = true;
                this.InvertButton.Foreground = Brushes.Magenta;
            }
            //reanalyze
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
            if (!normalize)
                NormalizeButton_Click(sender, e);
            SimWindow sw;
            if (simulation != null)
            {
                sw = new SimWindow(cycle, displaceView, simulation);
            }
            else sw = new SimWindow(cycle, displaceView);

            //open sim window
            sw.Show();
        }

        /// <summary>
        /// Handle Settings Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings st = new Settings();
            st.ShowDialog();
        }

        /// <summary>
        /// Handle Delete Simulation Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelSimButton_Click(object sender, RoutedEventArgs e)
        {
            this.simulation = null;
            DelSimButton.IsEnabled = false;
            hasDifference = false;
            DiffSimButton.IsEnabled = false;
            this.DiffSimButton.Foreground = Brushes.Black;
            this.Analyze();
        }

        /// <summary>
        /// Handle Diff Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffSimButton_Click(object sender, RoutedEventArgs e)
        {
            if (!hasDifference)
            {
                hasDifference = true;
                this.Analyze();
                this.DiffSimButton.Foreground = Brushes.IndianRed;
            }
            else
            {
                hasDifference = false;
                this.Analyze();
                this.DiffSimButton.Foreground = Brushes.Black;
            }
        }

        /// <summary>
        /// Handle Info Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            Info iw = new Info();
            iw.Show();
        }

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
                CompButton.Foreground = Brushes.CadetBlue;
            }
            else
            {
                comp1Path = "";
                comp2Path = "";
                CompButton.Foreground = Brushes.Black;
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
            this.UpdateMolView(false, false, true);
            coordGrid.Items.Refresh();
        }

        #endregion
    }
}

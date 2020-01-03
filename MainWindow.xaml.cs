using HelixToolkit.Wpf;
using MaterialDesignThemes.Wpf;
using MathNet.Spatial.Euclidean;
using OxyPlot;
using OxyPlot.Series;
using PorphyStruct.Chemistry;
using PorphyStruct.Files;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private Macrocycle old;
        public string path = "";
        public double normFac = 0;
        private int oldIndex = -1;

        public bool normalize = false;
        private bool hasDifference = false;
        public bool invert = false;

        public Simulation simulation = null;
        public Macrocycle.Type type = Macrocycle.Type.Corrole;
        public SnackbarMessageQueue MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

        string comp1Path = "";
        string comp2Path = "";

        public MainWindow()
        {
            InitializeComponent();
            Snack.MessageQueue = MessageQueue;
        }

        /// <summary>
        /// Analyze current Macrocycle and print result to PlotView
        /// </summary>
        public void Analyze()
        {
            string[] dontMark = Properties.Settings.Default.dontMark.Split(',');
            //plot that shit
            Oxy.Override.StandardPlotModel pm = new Oxy.Override.StandardPlotModel();
            Oxy.Override.LinearAxis y = pm.yAxis;

            //generate cycle
            Macrocycle cycle = MacrocycleFactory.Build(((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.IsMacrocycle).ToList(), type);

            List<AtomDataPoint> data = cycle.GetDataPoints();

            //normalisation
            if (normalize)
            {
                this.normFac = MathUtil.GetNormalizationFactor(data);
                //save normalized data
                data = MathUtil.Normalize(data);
                cycle.dataPoints = data;
            }

            //invert
            if (invert)
            {
                data = MathUtil.Invert(data);
                cycle.dataPoints = data;
            }

            //dont mark (data)
            for (int i = 0; i < cycle.dataPoints.Count; i++)
            {
                if (dontMark.Contains(cycle.dataPoints[i].atom.Type) || dontMark.Contains(cycle.dataPoints[i].atom.Identifier))
                    cycle.dataPoints[i].Size = 0;
            }

            //has simulation
            if (simulation != null)
            {
                //if normalize and simulation is'nt->normalize
                if (normalize && !simulation.isNormalized)
                {
                    simulation.cycle.dataPoints = MathUtil.Normalize(simulation.cycle.dataPoints);
                    simulation.isNormalized = true;
                }
                //if not normalzing but sim is normalized, denorm!
                else if (!normalize && simulation.isNormalized)
                {
                    simulation.cycle.dataPoints = MathUtil.Factor(simulation.cycle.dataPoints, 1 / this.normFac);
                    simulation.isNormalized = false;
                }

                //if inverting but sim is not inverted yet, invert!
                if (invert && !simulation.isInverted)
                {
                    simulation.cycle.dataPoints = MathUtil.Invert(simulation.cycle.dataPoints);
                    simulation.isInverted = true;
                }
                //if not inverting but simulation is Inverted-> invert!
                else if (!invert && simulation.isInverted)
                {
                    simulation.cycle.dataPoints = MathUtil.Invert(simulation.cycle.dataPoints);
                    simulation.isInverted = false;
                }

                //dont mark (sim)
                for (int i = 0; i < simulation.cycle.dataPoints.Count; i++)
                {
                    if (dontMark.Contains(simulation.cycle.dataPoints[i].atom.Type) || dontMark.Contains(simulation.cycle.dataPoints[i].atom.Identifier))
                        simulation.cycle.dataPoints[i].Size = 0;
                }
                //paint simulation
                simulation.Paint(pm);

                //if simulation has Difference
                if (hasDifference)
                {
                    Simulation tmpDiff = new Simulation((Macrocycle)cycle.Clone());
                    tmpDiff.cycle.dataPoints = GetDifference(data, simulation.cycle.dataPoints);
                    //dont mark (diff)
                    for (int i = 0; i < tmpDiff.cycle.dataPoints.Count; i++)
                    {
                        if (dontMark.Contains(tmpDiff.cycle.dataPoints[i].atom.Type) || dontMark.Contains(tmpDiff.cycle.dataPoints[i].atom.Identifier))
                            tmpDiff.cycle.dataPoints[i].Size = 0;
                    }
                    tmpDiff.Paint(pm, "Diff.");
                }
            }


            if (!Properties.Settings.Default.singleColor)
            {
                //build atom coloring axis
                OxyPlot.Axes.RangeColorAxis xR = cycle.BuildColorAxis();
                pm.Axes.Add(xR);
            }

            ScatterSeries series = new ScatterSeries
            {
                MarkerType = Properties.Settings.Default.markerType,
                ItemsSource = data,
                ColorAxisKey = Properties.Settings.Default.singleColor ? null : "colors",
                Title = "Exp."
            };
            if (Properties.Settings.Default.singleColor)
                series.MarkerFill = Atom.modesSingleColor[0];

            pm.Series.Add(series);

            displaceView.Model = pm;
            pm.InvalidatePlot(true);

            foreach (OxyPlot.Annotations.ArrowAnnotation a in cycle.DrawBonds())
            {
                pm.Annotations.Add(a);
            }

            if (Properties.Settings.Default.zero)
            {
                //show zero
                OxyPlot.Annotations.LineAnnotation zero = new OxyPlot.Annotations.LineAnnotation()
                {
                    Color = OxyColor.FromAColor(75, OxyColors.Gray),
                    StrokeThickness = Properties.Settings.Default.lineThickness,
                    Intercept = 0,
                    Slope = 0
                };
                pm.Annotations.Add(zero);
            }

            //comparison
            if (!String.IsNullOrEmpty(comp1Path))
            {
                Simulation com = CompareWindow.GetData(comp1Path);
                // dont mark(comp1)
                for (int i = 0; i < com.cycle.dataPoints.Count; i++)
                {
                    if (dontMark.Contains(com.cycle.dataPoints[i].atom.Type) || dontMark.Contains(com.cycle.dataPoints[i].atom.Identifier))
                        com.cycle.dataPoints[i].Size = 0;
                }
                com.Paint(pm, "Com.1");
            }
            if (!String.IsNullOrEmpty(comp2Path))
            {
                Simulation com = CompareWindow.GetData(comp2Path);
                // dont mark(comp1)
                for (int i = 0; i < com.cycle.dataPoints.Count; i++)
                {
                    if (dontMark.Contains(com.cycle.dataPoints[i].atom.Type) || dontMark.Contains(com.cycle.dataPoints[i].atom.Identifier))
                        com.cycle.dataPoints[i].Size = 0;
                }
                com.Paint(pm, "Com.2");
            }


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

            //update plane coordinates
            Plane pl = cycle.GetMeanPlane();
            UnitVecTB.Text = "(" + pl.A.ToString("G3") + "," + pl.B.ToString("G3") + "," + pl.C.ToString("G3") + ")";
            DistTB.Text = pl.D.ToString("G3");
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
        }     

        /// <summary>
        /// Handle CIF Files
        /// </summary>
        private void OpenCif()
        {
            //open file and set itemssource
            CifFile file = new CifFile(path);
            Crystal mol = file.GetMolecule();
            mol.SetIsMacrocycle(this.type);
            coordGrid.ItemsSource = mol.Atoms.OrderByDescending(s => s.IsMacrocycle).ToList();
            //show message
            MessageQueue.Enqueue("CIF-File opened!");
        }

        /// <summary>
        /// Handle CIF Files
        /// </summary>
        private void OpenMol2()
        {
            //open file and set itemssource
            Mol2File file = new Mol2File(path);
            Molecule mol = file.GetMolecule();
            mol.SetIsMacrocycle(this.type);
            coordGrid.ItemsSource = mol.Atoms.OrderByDescending(s => s.IsMacrocycle).ToList();

            //show message
            MessageQueue.Enqueue("Mol2-File opened!");
        }


        /// <summary>
        /// Open XYZ File
        /// </summary>
        /// <param name="isIXYZ"></param>
        private void OpenXYZ(bool isIXYZ = false)
        {
            //open file and set itemssource
            XYZFile file = new XYZFile(path);
            Molecule mol;
            if (isIXYZ) mol = file.GetMolecule(true);
            else mol = file.GetMolecule();
            //here the guess is used, be careful!!
            mol.SetIsMacrocycle(this.type);
            coordGrid.ItemsSource = mol.Atoms.OrderByDescending(s => s.IsMacrocycle).ToList();

            //show message
            MessageQueue.Enqueue("XYZ-File opened!");
        }


        /// <summary>
        /// Update 3D Model
        /// </summary>
        /// <param name="markSelection"></param>
        /// <param name="force"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Don't need to catch sth.")]
        private void UpdateMolView(bool markSelection = false, bool force = false, bool detect = false)
        {
            Macrocycle cycle = MacrocycleFactory.Build(((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.IsMacrocycle).ToList(), type);
            if (detect)
                cycle.Detect();

            if ((old != null && !CompareAtoms(cycle.Atoms, old.Atoms)) || (markSelection && oldIndex != coordGrid.SelectedIndex) || force)
            {
                old = cycle;
                oldIndex = coordGrid.SelectedIndex;
                MolViewer.Children.Clear();

                MolViewer.Children.Add(new DefaultLights());

                //create 3d model
                System.Windows.Media.Media3D.ModelVisual3D model = new System.Windows.Media.Media3D.ModelVisual3D();
                System.Windows.Media.Media3D.Model3DGroup group = new System.Windows.Media.Media3D.Model3DGroup();

                foreach (Atom a in cycle.Atoms)
                {
                    MeshBuilder b = new MeshBuilder(true, true);

                    //atom vars
                    string identifier = a.Type;
                    var pos = new System.Windows.Media.Media3D.Point3D(a.X, a.Y, a.Z);
                    var radius = a.AtomRadius / 2;
                    Brush brush = a.Brush;
                    if (markSelection)
                    {
                        if (a == (Atom)coordGrid.SelectedItem)
                        {
                            brush = Brushes.LightGoldenrodYellow;
                        }
                    }
                    var fill = MaterialHelper.CreateMaterial(brush);
                    b.AddSphere(pos, radius);
                    group.Children.Add(new System.Windows.Media.Media3D.GeometryModel3D(b.ToMesh(), fill));
                }
                foreach (Tuple<string, string> t in cycle.Bonds)
                {
                    MeshBuilder b = new MeshBuilder(true, true);
                    try
                    {
                        Atom a1 = cycle.ByIdentifier(t.Item1, true);
                        Atom a2 = cycle.ByIdentifier(t.Item2, true);
                        var p1 = new System.Windows.Media.Media3D.Point3D(a1.X, a1.Y, a1.Z);
                        var p2 = new System.Windows.Media.Media3D.Point3D(a2.X, a2.Y, a2.Z);
                        b.AddCylinder(p1, p2, 0.2, 10);
                        //add only to selection if both are macrocycle marked
                        if (a1.IsMacrocycle && a2.IsMacrocycle)
                            group.Children.Add(new System.Windows.Media.Media3D.GeometryModel3D(b.ToMesh(), Materials.Blue));
                    }
                    catch {/**does nothing**/ }
                }
                //add metal bonds
                if (cycle.HasMetal)
                {
                    List<Atom> Nitrogen = cycle.Atoms.Where(s => Regex.IsMatch(s.Identifier, "N[1-4]")).ToList();
                    Atom m = cycle.GetMetal();
                    foreach (Atom n in Nitrogen)
                    {
                        MeshBuilder b = new MeshBuilder(true, true);
                        try
                        {
                            var p1 = new System.Windows.Media.Media3D.Point3D(n.X, n.Y, n.Z);
                            var p2 = new System.Windows.Media.Media3D.Point3D(m.X, m.Y, m.Z);
                            b.AddCylinder(p1, p2, 0.2, 10);
                            //add only to selection if both are macrocycle marked
                            if (n.IsMacrocycle && m.IsMacrocycle)
                                group.Children.Add(new System.Windows.Media.Media3D.GeometryModel3D(b.ToMesh(), Materials.Blue));
                        }
                        catch {/**does nothing**/ }
                    }
                }

                for (int i = 0; i < cycle.Atoms.Count; i++)
                {
                    for (int j = 0; j < cycle.Atoms.Count; j++)
                    {
                        Atom a1 = cycle.Atoms[i];
                        Atom a2 = cycle.Atoms[j];
                        if (Atom.Distance(a1, a2) < (a1.Element.Radius + a2.Element.Radius) + 0.25)
                        {
                            MeshBuilder b = new MeshBuilder(true, true);
                            //draw bond
                            var p1 = new System.Windows.Media.Media3D.Point3D(a1.X, a1.Y, a1.Z);
                            var p2 = new System.Windows.Media.Media3D.Point3D(a2.X, a2.Y, a2.Z);
                            b.AddCylinder(p1, p2, 0.075, 10);
                            group.Children.Add(new System.Windows.Media.Media3D.GeometryModel3D(b.ToMesh(), Materials.Gray));
                        }
                    }
                }
                model.Content = group;
                MolViewer.Children.Add(model);
            }
        }

        /// <summary>
        /// Compare Two Atom Lists
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        private bool CompareAtoms(List<Atom> l1, List<Atom> l2)
        {
            if (l1.SequenceEqual(l2)) return true;
            else return false;
        }

        /// <summary>
        /// returns the plot data of exp.
        /// </summary>
        /// <returns></returns>
        public List<AtomDataPoint> GetData()
        {
            List<AtomDataPoint> data = new List<AtomDataPoint>();
            ScatterSeries series = (ScatterSeries)displaceView.Model.Series.FirstOrDefault(s => s.Title == "Exp.");
            data.AddRange((List<AtomDataPoint>)series.ItemsSource);
            return data;
        }

        /// <summary>
        /// returns current macrocycle
        /// </summary>
        /// <returns></returns>
        public Macrocycle GetCycle()
        {
            List<AtomDataPoint> data = new List<AtomDataPoint>();
            try
            {
                data = GetData();
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Failed to MainWindow.GetData() because no Data is present");
            }
            var cycle = MacrocycleFactory.Build(((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.IsMacrocycle).ToList(), type);
            cycle.Title = Path.GetFileNameWithoutExtension(this.path);
            cycle.dataPoints = data;

            return cycle;
        }

        /// <summary>
        /// returns difference between exp and sim
        /// </summary>
        /// <returns></returns>
        public List<AtomDataPoint> GetDifference(List<AtomDataPoint> data, List<AtomDataPoint> simData)
        {
            List<AtomDataPoint> diff = new List<AtomDataPoint>();

            //strip metal
            data = data.Where(s => !s.atom.IsMetal).ToList();
            //Shows the difference between sim and exp.
            if (this.simulation != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    AtomDataPoint dp = new AtomDataPoint(data[i].X, (data[i].Y - simData[i].Y), data[i].atom);
                    diff.Add(dp);
                }
            }
            return diff;
        }


        /// <summary>
        /// Centers Molecule into origin and updates camera to bestview
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Ausstehend>")]
        public void Center()
        {
            Macrocycle cycle = MacrocycleFactory.Build(((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.IsMacrocycle).ToList(), type);
            Vector3D centroid = cycle.GetCentroid();

            foreach (Atom a in cycle.Atoms)
            {
                Vector3D coord = new Vector3D(a.X, a.Y, a.Z);
                Vector3D newCoord = coord - centroid;

                a.X = newCoord.X;
                a.Y = newCoord.Y;
                a.Z = newCoord.Z;
            }

            coordGrid.ItemsSource = cycle.Atoms.OrderByDescending(s => s.IsMacrocycle).ToList();
            coordGrid.Items.Refresh();
            this.UpdateMolView(false, true);
            try
            {
                //update camera
                Plane pl = cycle.GetMeanPlane();
                var normal = new System.Windows.Media.Media3D.Point3D(pl.A, pl.B, pl.C);
                while (normal.DistanceTo(new System.Windows.Media.Media3D.Point3D(0, 0, 0)) < 25)
                {
                    normal = normal.Multiply(1.1);
                }
                MolViewer.CameraController.CameraPosition = normal;
                MolViewer.CameraController.CameraTarget = new System.Windows.Media.Media3D.Point3D(0, 0, 0);
            }
            catch { }

        }

        #region UI Interaction
        /// <summary>
        /// Handle Updated Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordGrid_Updated(object sender, EventArgs e)
        {
            this.UpdateMolView();
        }


        /// <summary>
        /// Handle Window Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //subscribe to event
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            if (dpd != null)
            {
                dpd.AddValueChanged(coordGrid, CoordGrid_Updated);
            }
        }

        /// <summary>
        /// Handle Refresh Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateMolView(false, true);
        }

        /// <summary>
        /// Handle Selection Changed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //update on cell edit if loading is finished
            this.UpdateMolView(true);
        }

        /// <summary>
        /// Handle Target Updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            //update on cell edit if loading is finished
            this.UpdateMolView(true);
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

                path = wi.FileName;
                type = (Macrocycle.Type)wi.Type;
                this.Title = "Structural Analysis of Porphyrinoids (PorphyStruct) - " + type.ToString() + " Mode";
                AnalButton.IsEnabled = true;
                RefMolButton.IsEnabled = true;
                CenterMolButton.IsEnabled = true;
                DetectMolButton.IsEnabled = true;
                NormalizeButton.IsEnabled = true;
                InvertButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
                SimButton.IsEnabled = true;
                CompButton.IsEnabled = true;

                //clear plotview
                if (displaceView.Model != null)
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
                comp1Path = "";
                comp2Path = "";

                //reset buttons
                normalize = invert = hasDifference = false;
                NormalizeButton.Foreground = InvertButton.Foreground = DiffSimButton.Foreground = CompButton.Foreground = Brushes.Black;

                //disable delete&diff sim button
                DelSimButton.IsEnabled = false;
                DiffSimButton.IsEnabled = false;

                //handle file open
                if (System.IO.Path.GetExtension(path) == ".cif")
                    OpenCif();
                else if (System.IO.Path.GetExtension(path) == ".mol" || System.IO.Path.GetExtension(path) == ".mol2")
                    OpenMol2();
                else if (System.IO.Path.GetExtension(path) == ".ixyz")
                    OpenXYZ(true);
                else
                    OpenXYZ();
            }
        }

        /// <summary>
        /// Handle Analyze Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            //call analyze void
            Analyze();

            //show message
            MessageQueue.Enqueue("Analysis complete!");
        }

        /// <summary>
        /// Handle Center Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CenterMolButton_Click(object sender, RoutedEventArgs e)
        {
            this.Center();

            //show message
            MessageQueue.Enqueue("Molecule has been centerd into origin by subtraction of its centroid!");
        }


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
            //show message
            MessageQueue.Enqueue("Analysis complete!");
        }

        /// <summary>
        /// Handle Save Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //open save dialog
            SaveWindow svW = new SaveWindow(GetCycle(), this.simulation);
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
            //show message
            MessageQueue.Enqueue("Analysis complete!");
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
                sw = new SimWindow(GetCycle(), displaceView, simulation);
            }
            else sw = new SimWindow(GetCycle(), displaceView);

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
            //show message
            MessageQueue.Enqueue("Detection Algorithmus finished!");
        }

        #endregion

        /// <summary>
        /// Implement IDisposable
        /// </summary>
        /// <param name="disposing"></param>
        /// <see cref="IDisposable.Dispose"/>
        protected virtual void Dispose(bool disposing)
        {
            //dispose managed resources
            if (disposing) MessageQueue.Dispose();
        }

        /// <summary>
        /// Wrapper for Dispose(true)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}

using HelixToolkit.Wpf;
using MathNet.Spatial.Euclidean;
using OxyPlot.Series;
using PorphyStruct.Chemistry;
using PorphyStruct.Core.Util;
using PorphyStruct.Util;
using PorphyStruct.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PorphyStruct
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Macrocycle _cycle;
        public Macrocycle Cycle
        {
            get => _cycle;
            set => SetAndNotify(ref _cycle, value);
        }

        public double normFac = 0;
        private bool _hasDifference, _invert, _normalize;
        /// <summary>
        /// Handles Normalisation
        /// </summary>
        public bool Normalize
        {
            get => _normalize;
            set => SetAndNotify(ref _normalize, value);
        }

        /// <summary>
        /// Handles Invert
        /// </summary>
        public bool Invert
        {
            get => _invert;
            set => SetAndNotify(ref _invert, value);
        }

        /// <summary>
        /// Handles Difference
        /// </summary>
        public bool HasDifference
        {
            get => _hasDifference;
            set => SetAndNotify(ref _hasDifference, value);
        }

        /// <summary>
        /// Indicates whether a comparison is present
        /// </summary>
        public bool HasComparison => !string.IsNullOrEmpty(comp1Path) || !string.IsNullOrEmpty(comp2Path);

        /// <summary>
        /// The Molecule as 3D Representation
        /// </summary>
        public AsyncObservableCollection<ModelVisual3D> Molecule3D = new AsyncObservableCollection<ModelVisual3D>();

        Dictionary<string, List<Property>> _cycleProperties = new Dictionary<string, List<Property>>();
        /// <summary>
        /// A Dictionary with all shown CycleProperties
        /// </summary>
        public Dictionary<string, List<Property>> CycleProperties { get => _cycleProperties; set => SetAndNotify(ref _cycleProperties, value); }

        /// <summary>
        /// Set and Notify Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void SetAndNotify<T>(ref T field, T value, [CallerMemberName] String propertyName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                NotifyPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Adds PropertyChanged Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies on Property Change
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Simulation simulation = null;
        public Macrocycle.Type type = Macrocycle.Type.Corrole;

        public string comp1Path, comp2Path, path;
        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        public static DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(MainWindow));

        public MainWindow()
        {
            //CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
            DataContext = this; 
        }

        /// <summary>
        /// Analyze current Macrocycle and print result to PlotView
        /// </summary>
        public void Analyze()
        {
            //set up default plot model
            OxyPlotOverride.StandardPlotModel pm = new OxyPlotOverride.StandardPlotModel();

            //do analysis
            Cycle.GetDataPoints();

            //normalisation
            if (Normalize)
            {
                normFac = MathUtil.GetNormalizationFactor(Cycle.dataPoints);
                Cycle.dataPoints = Cycle.dataPoints.Normalize();
            }
            //invert
            if (Invert) Cycle.dataPoints = Cycle.dataPoints.Invert();

            //handle sim
            if (simulation != null)
            {
                simulation.Normalize(Normalize, normFac);
                simulation.Invert(Invert);
                simulation.Paint(pm);
            }
            //paint difference
            if (HasDifference) Cycle.GetDifference(simulation).Paint(pm, "Diff");
            //paint comparison
            if (!String.IsNullOrEmpty(comp1Path)) CompareWindow.GetData(comp1Path).Paint(pm, "Com1");
            if (!String.IsNullOrEmpty(comp2Path)) CompareWindow.GetData(comp2Path).Paint(pm, "Com2");
            //paint exp
            Cycle.Paint(pm, MacrocyclePainter.PaintMode.Exp);

            //handle dont mark
            foreach (ScatterSeries s in pm.Series) ((List<AtomDataPoint>)s.ItemsSource).Where(dp => Core.Properties.Settings.Default.dontMark.Split(',').Contains(dp.atom.Identifier) || Core.Properties.Settings.Default.dontMark.Split(',').Contains(dp.atom.Type)).ToList().ForEach(dp => dp.Size = 0);

            displaceView.Model = pm;
            pm.Scale(pm.yAxis, true, Normalize);
            pm.Scale(pm.xAxis);
            UpdateStack();
        }
        /// <summary>
        /// Update Sim WrapPanel
        /// </summary>
        public void UpdateStack()
        {
            //Hardcode that until a better solution is found
            if (Cycle != null)
            {
                var msp = Cycle.GetMeanPlane();
                CycleProperties["General"] = new List<Property>()
                {
                    new Property("D_oop", Cycle.MeanDisplacement().ToString("G5")),
                    new Property("D_oop(sim)", simulation != null ? simulation.cycle.MeanDisplacement().ToString("G5") : double.NaN.ToString())
                };

                CycleProperties["Dihedrals"] = Cycle.Dihedrals.Select(s => new Property(string.Join("-", s), Cycle.Dihedral(s).ToString("G3") + "°")).ToList();
                if (Cycle.HasMetal(false))
                    CycleProperties["Distances"] = Cycle.Atoms.Where(s => s.BondTo(Cycle.GetMetal()))
                        .Select(s => new Property($"{s.Identifier}-{Cycle.GetMetal().Identifier}", Atom.Distance(s, Cycle.GetMetal()).ToString("G3") + " Å"))
                        .Append(new Property($"{Cycle.GetMetal().Identifier} - Mean Plane", Cycle.GetMetal().DistanceToPlane(msp).ToString("G3") + " Å")).ToList();
                if (simulation != null)
                    CycleProperties["Simulation"] = simulation.par.Select(s => new Property(s.Key, 
                        $"{s.Value.ToString("G4")} % / {(s.Value / 100 * simulation.cycle.MeanDisplacement()).ToString("G6")} Å")).ToList();
                CycleProperties["Mean Plane"] = new List<Property>()
                {
                    new Property("Unit Vector", $"({msp.A.ToString("G3")}, {msp.B.ToString("G3")}, {msp.C.ToString("G3")})"),
                    new Property("Distance", msp.D.ToString("G3"))
                };
            }

            //bind to control
            var flattened = CycleProperties.SelectMany(x => x.Value?.Select(y => new { x.Key, y.Name, y.Value })).ToList();
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
            Cycle.Center(s => s.IsMacrocycle);
            //update camera
            Plane pl = Cycle.GetMeanPlane();
            var normal = new System.Windows.Media.Media3D.Point3D(pl.A, pl.B, pl.C);
            while (normal.DistanceTo(Win32Util.Origin) < 25) normal = normal.Multiply(1.1);
            MolViewer.CameraController.CameraPosition = normal;
            MolViewer.CameraController.CameraTarget = Win32Util.Origin;
        }

        /// <summary>
        /// Returns Atom's Model in Molecule Model
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private ModelVisual3D ModelByAtom(Atom a) => Molecule3D.Where(s => (s as AtomModelVisual3D)?.Atom == a).FirstOrDefault();

        /// <summary>
        /// Handles Selection Change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //read selected index of coordgrid
            Atom selected = (Atom)e.AddedItems[0] ?? null;

            //remove atom
            Molecule3D.Remove(ModelByAtom(selected));
            //add atom
            Molecule3D.Add(selected.Atom3D(true));

            //handle previous
            Atom previous = e.RemovedItems.Count != 0 ? (Atom)e.RemovedItems[0] : null;
            if (previous != null)
            {
                Molecule3D.Remove(ModelByAtom(previous));
                Molecule3D.Add(previous.Atom3D(false));
            }
        }

        /// <summary>
        /// Handles Collection Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (Atom item in e.OldItems)
                {
                    var removedItem = ModelByAtom(item);
                    if (removedItem != null) Molecule3D.Remove(removedItem);
                    item.PropertyChanged -= Atom_PropertyChanged;
                }
            if (e.NewItems != null)
                foreach (Atom item in e.NewItems)
                {
                    Molecule3D.Add(item.Atom3D());
                    item.PropertyChanged += Atom_PropertyChanged;
                }
        }

        /// <summary>
        /// Fires when a Property of an Atom has changed. So Repaint the Atom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Atom_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //read selected index of coordgrid
            Atom selected = (Atom)coordGrid.SelectedItem ?? null;
            Atom atom = (Atom)sender;

            //remove atom and bonds
            Molecule3D.Remove(ModelByAtom(atom));
            var models = Molecule3D.Where(s => (s as BondModelVisual3D)?.Atoms.Contains(atom) ?? false).ToList();
            foreach (var m in models) Molecule3D.Remove(m);

            //add atom and bonds
            Molecule3D.Add(atom.Atom3D(atom == selected));
            foreach (var a2 in Cycle.Neighbors(atom)) Molecule3D.Add(a2.Bond3D(atom, Cycle));
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
                Title = $"Structural Analysis of Porphyrinoids (PorphyStruct) - {type.ToString()} Mode";

                //reset gui elements
                AnalButton.IsEnabled = CenterMolButton.IsEnabled = DetectMolButton.IsEnabled = NormalizeButton.IsEnabled = InvertButton.IsEnabled = SaveButton.IsEnabled = SimButton.IsEnabled = CompButton.IsEnabled = true;
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
                comp1Path = comp2Path = "";

                Cycle = MacrocycleFactory.Load(path, type);
                Cycle.Atoms.CollectionChanged += OnCollectionChanged;

                foreach (var atom in Cycle.Atoms) atom.PropertyChanged += Atom_PropertyChanged;

                FileName = Cycle.Title;

                //bind 
                coordGrid.ItemsSource = Cycle.Atoms;
                Molecule3D = new AsyncObservableCollection<ModelVisual3D>(Cycle.Paint3D());
                MolViewer.ItemsSource = Molecule3D;
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
            //TODO: validate cycle here
            //call analyze void
            Analyze();
        }

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
            Normalize = !Normalize;
            this.Analyze();
        }

        /// <summary>
        /// Handle Save Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e) { if (SaveButton.IsEnabled) new SaveWindow(Cycle, simulation).ShowDialog(); }

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
            if (simulation != null) new SimWindow(Cycle, displaceView, simulation).Show();
            else new SimWindow(Cycle, displaceView).Show();
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
        private async void Detect_Click(object sender, RoutedEventArgs e) => await Cycle.Detect();
    }
}

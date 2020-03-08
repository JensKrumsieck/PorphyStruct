using OxyPlot.Series;
using PorphyStruct.Chemistry;
using PorphyStruct.Core.Util;
using PorphyStruct.OxyPlotOverride;
using PorphyStruct.Util;
using PorphyStruct.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace PorphyStruct.ViewModel
{
    public class MainViewModel : AbstractViewModel
    {

        public MainViewModel(string path, Macrocycle.Type type)
        {
            Path = path;
            Type = type;
            //Load cycle
            Cycle = MacrocycleFactory.Load(Path, Type);
            //Bind Events
            Cycle.Atoms.CollectionChanged += OnCollectionChanged;
            foreach (var atom in Cycle.Atoms) atom.PropertyChanged += Atom_PropertyChanged;
            //fill molecule 3D
            Molecule3D = new AsyncObservableCollection<ModelVisual3D>();
            Cycle.Paint3D().ToList().ForEach(s => Molecule3D.Add(s));

            //init properties
            CycleProperties = new Dictionary<string, List<Property>>();
        }

        public Simulation simulation = null;
        public Macrocycle.Type Type = Macrocycle.Type.Corrole;
        public string comp1Path, comp2Path, Path;
        public double normFac = 0;

        public Macrocycle Cycle { get => Get<Macrocycle>(); set => Set(value); }
        /// <summary>
        /// Handles Normalisation
        /// </summary>
        public bool Normalize { get => Get<bool>(); set => Set(value); }

        /// <summary>
        /// Handles Invert
        /// </summary>
        public bool Invert { get => Get<bool>(); set => Set(value); }

        /// <summary>
        /// Handles Difference
        /// </summary>
        public bool HasDifference { get => Get<bool>(); set => Set(value); }

        /// <summary>
        /// Contains the default plotmodel
        /// </summary>
        public StandardPlotModel Model { get => Get<StandardPlotModel>(); private set => Set(value); }

        public Atom SelectedItem { get => Get<Atom>(); set => Set(value); }

        /// <summary>
        /// Indicates whether a comparison is present
        /// </summary>
        public bool HasComparison => !string.IsNullOrEmpty(comp1Path) || !string.IsNullOrEmpty(comp2Path);

        /// <summary>
        /// The Molecule as 3D Representation
        /// </summary>
        public AsyncObservableCollection<ModelVisual3D> Molecule3D { get => Get<AsyncObservableCollection<ModelVisual3D>>(); set => Set(value); }

        /// <summary>
        /// A Dictionary with all shown CycleProperties
        /// </summary>
        public Dictionary<string, List<Property>> CycleProperties { get => Get<Dictionary<string, List<Property>>>(); set => Set(value); }


        /// <summary>
        /// Fires when a Property of an Atom has changed. So Repaint the Atom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Atom_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //read selected index of coordgrid
            Atom selected = SelectedItem ?? null;
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
        /// check if selected item changes
        /// </summary>
        /// <param name="name"></param>
        protected override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            base.OnPropertyChanged(name);
            if(name == "SelectedItem")
            {
                //remove selected
                Molecule3D.Remove(ModelByAtom(SelectedItem));
                //add atom
                Molecule3D.Add(SelectedItem.Atom3D(true));
            }
        }

        /// <summary>
        /// Fires before selection changed
        /// </summary>
        /// <param name="name"></param>
        protected override void OnPropertyChanging([CallerMemberName] string name = null)
        {
            base.OnPropertyChanging(name);

            if (name == "SelectedItem"  && SelectedItem != null)
            {
                //remove selected
                Molecule3D.Remove(ModelByAtom(SelectedItem));
                //add atom
                Molecule3D.Add(SelectedItem.Atom3D());
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
        /// Returns Atom's Model in Molecule Model
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private ModelVisual3D ModelByAtom(Atom a) => Molecule3D.Where(s => (s as AtomModelVisual3D)?.Atom == a).FirstOrDefault();

        public void Analyze()
        {
            Model = new StandardPlotModel();
            //calculate Data
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
                simulation.Paint(Model);
            }
            //paint difference
            if (HasDifference) Cycle.GetDifference(simulation).Paint(Model, "Diff");
            //paint comparison
            if (!string.IsNullOrEmpty(comp1Path)) CompareWindow.GetData(comp1Path).Paint(Model, "Com1");
            if (!string.IsNullOrEmpty(comp2Path)) CompareWindow.GetData(comp2Path).Paint(Model, "Com2");
            //paint exp
            Cycle.Paint(Model, MacrocyclePainter.PaintMode.Exp);

            //handle dont mark
            foreach (ScatterSeries s in Model.Series) ((List<AtomDataPoint>)s.ItemsSource).Where(dp => Core.Properties.Settings.Default.dontMark.Split(',').Contains(dp.atom.Identifier) || Core.Properties.Settings.Default.dontMark.Split(',').Contains(dp.atom.Type)).ToList().ForEach(dp => dp.Size = 0);

            Model.Scale(Model.yAxis, true, Normalize);
            Model.Scale(Model.xAxis);
        }

        public void UpdateProperties()
        {
            //hardcoded until i find better solution....
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
        }
    }
}

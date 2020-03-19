using OxyPlot.Series;
using PorphyStruct.Chemistry;
using PorphyStruct.Chemistry.Data;
using PorphyStruct.Chemistry.Properties;
using PorphyStruct.OxyPlotOverride;
using PorphyStruct.Util;
using PorphyStruct.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            //init properties
            CycleProperties = new Dictionary<string, IEnumerable<Property>>();

            //Load cycle
            Cycle = MacrocycleFactory.Load(Path, Type);
            //Bind Events
            Cycle.PropertyChanged += Cycle_PropertyChanged;
            Cycle.Atoms.CollectionChanged += OnCollectionChanged;
            Cycle.DataProviders.CollectionChanged += OnDataChanged;
            foreach (var atom in Cycle.Atoms) atom.PropertyChanged += Atom_PropertyChanged;
            //fill molecule 3D
            Molecule3D = new ObservableCollection<ModelVisual3D>();
            Cycle.Paint3D().ToList().ForEach(s => Molecule3D.Add(s));
            FileName = Cycle.Title;
        }

        /// <summary>
        /// DataProviders Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataChanged(object sender, NotifyCollectionChangedEventArgs e) => HasComparison = Cycle.DataProviders.Where(s => s.DataType == DataType.Comparison).Count() > 0;

        /// <summary>
        /// Is Valid? Redraw!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cycle_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Cycle.IsValid):
                    //atoms all changed!
                    foreach (var a in Cycle.Atoms.Where(s => s.IsMacrocycle))
                        Atom_PropertyChanged(a, e);
                    break;
            }
        }

        public Macrocycle.Type Type = Macrocycle.Type.Corrole;
        public string Path;
        public double normFac = 0;

        /// <summary>
        /// The Macrocycle
        /// </summary>
        public Macrocycle Cycle { get => Get<Macrocycle>(); set => Set(value); }

        //Cycles FileName / Title
        public string FileName { get => Get<string>(); set => Set(value); }

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
        public bool HasComparison { get => Get<bool>(); set => Set(value); }

        /// <summary>
        /// The Molecule as 3D Representation
        /// </summary>
        public ObservableCollection<ModelVisual3D> Molecule3D { get => Get<ObservableCollection<ModelVisual3D>>(); set => Set(value); }

        /// <summary>
        /// A Dictionary with all shown CycleProperties
        /// </summary>
        public Dictionary<string, IEnumerable<Property>> CycleProperties { get => Get<Dictionary<string, IEnumerable<Property>>>(); set => Set(value); }


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
            //Invoke to not run into object ownership issues
            App.Current.Dispatcher.Invoke(() =>
            {
                Molecule3D.Remove(ModelByAtom(atom));
                var models = Molecule3D.Where(s => (s as BondModelVisual3D)?.Atoms.Contains(atom) ?? false).ToList();
                foreach (var m in models) Molecule3D.Remove(m);

                //add atom and bonds
                Molecule3D.Add(atom.Atom3D(atom == selected));
                foreach (var a2 in Cycle.Neighbors(atom)) Molecule3D.Add(a2.Bond3D(atom, Cycle));
            });
        }

        /// <summary>
        /// check if selected item changes
        /// </summary>
        /// <param name="name"></param>
        protected override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            base.OnPropertyChanged(name);
            switch (name)
            {
                case nameof(SelectedItem):
                    if (SelectedItem != null)
                    {
                        //remove selected
                        Molecule3D.Remove(ModelByAtom(SelectedItem));
                        //add atom
                        Molecule3D.Add(SelectedItem.Atom3D(true));
                    }
                    break;
                case nameof(Model):
                case nameof(Cycle):
                    UpdateProperties();
                    break;
            }
        }

        /// <summary>
        /// Fires before selection changed
        /// </summary>
        /// <param name="name"></param>
        protected override void OnPropertyChanging([CallerMemberName] string name = null)
        {
            base.OnPropertyChanging(name);

            if (name == "SelectedItem" && SelectedItem != null)
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

            Cycle.Normalize(Normalize);
            Cycle.Invert(Invert);

            //paint difference
            //not affected by Normalize and Invert since always freshly calc'd
            Cycle.DataProviders.RemoveAll(s => s.DataType == DataType.Difference);
            if (HasDifference)
            {
                Cycle.DataProviders.Add(
                    new DifferenceData(
                        Cycle.DataProviders.Where(s => s.DataType == DataType.Experimental).FirstOrDefault() as ExperimentalData,
                        Cycle.DataProviders.Where(s => s.DataType == DataType.Simulation).FirstOrDefault() as SimulationData)
                    );
            }
            Cycle.Paint(Model);

            //handle dont mark
            foreach (ScatterSeries s in Model.Series) ((IEnumerable<AtomDataPoint>)s.ItemsSource).Where(dp => Core.Properties.Settings.Default.dontMark.Split(',').Contains(dp.atom.Identifier) || Core.Properties.Settings.Default.dontMark.Split(',').Contains(dp.atom.Type)).ToList().ForEach(dp => dp.Size = 0);

            Model.Scale(Model.yAxis, true, Normalize);
            Model.Scale(Model.xAxis);
            //set properties
            UpdateProperties();
        }

        public void UpdateProperties()
        {
            if (Cycle != null)
            {
                CycleProperties = Cycle.Properties.ToLookup(x => x.Key, y => y.Value).ToDictionary(group => group.Key, group => group.SelectMany(value => value));
                var msp = Cycle.GetMeanPlane();
                CycleProperties["Mean Plane"] = new List<Property>()
                {
                    new Property("Unit Vector", $"({msp.A.ToString("G3")}, {msp.B.ToString("G3")}, {msp.C.ToString("G3")})"),
                    new Property("Distance", msp.D.ToString("G3"))
                };
            }
            OnPropertyChanged(nameof(CycleProperties));
        }
    }
}

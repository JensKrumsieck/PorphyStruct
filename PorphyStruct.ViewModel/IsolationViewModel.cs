using ChemSharp.Molecules;
using ChemSharp.Molecules.Export;
using PorphyStruct.Core;
using PorphyStruct.ViewModel.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Media;
using TinyMVVM;
using TinyMVVM.Command;

namespace PorphyStruct.ViewModel
{
    public class IsolationViewModel : BaseViewModel
    {
        public Macrocycle DataObject { get; set; }
        /// <summary>
        /// 3D Representation of Atoms
        /// </summary>
        public ObservableCollection<AtomVisual3D> Atoms3D { get; }
        /// <summary>
        /// 3D Representation of Bonds
        /// </summary>
        public ObservableCollection<BondVisual3D> Bonds3D { get; }

        private readonly string _path;

        public ObservableCollection<Atom> Isolation { get; } = new ObservableCollection<Atom>();

        public IsolationViewModel(MacrocycleViewModel mvm) : this(mvm.Macrocycle)
        {
            _path = mvm.Filename;
        }

        public RelayCommand<Atom> DeleteCommand { get; }

        public IsolationViewModel(Macrocycle cycle)
        {
            if (string.IsNullOrEmpty(_path)) _path = cycle.Title;

            DataObject = cycle;
            Atoms3D = new ObservableCollection<AtomVisual3D>(DataObject.Atoms.Select(s => new AtomVisual3D(s)));
            Bonds3D = new ObservableCollection<BondVisual3D>(DataObject.Bonds.Select(s => new BondVisual3D(s)));
            Isolation.CollectionChanged += IsolationOnCollectionChanged;
            DeleteCommand = new RelayCommand<Atom>(a => Isolation.Remove(a));
        }

        private void IsolationOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => Validate();

        private void Validate()
        {
            foreach (var bond in Bonds3D)
            {
                if (Isolation.Count(atom => bond.Bond.Atoms.Contains(atom)) != 2) continue;
                bond.Color = Colors.Green;
                bond.IsValid = true;
            }
        }

        public string Save()
        {
            var bonds = DataObject.Bonds.Where(bond => Isolation.Count(atom => bond.Atoms.Contains(atom)) == 2);
            var mol = new Molecule(Isolation.ToList(), bonds);
            var folder = Path.GetDirectoryName(_path);
            var file = Path.GetFileNameWithoutExtension(_path);
            var createdFile = folder + "/" + file + "_isolation.mol2";
            Mol2Exporter.Export(mol, createdFile);
            return createdFile;
        }
    }
}

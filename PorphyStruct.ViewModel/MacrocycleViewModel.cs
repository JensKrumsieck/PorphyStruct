using ChemSharp.Molecules;
using PorphyStruct.ViewModel.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using TinyMVVM;

namespace PorphyStruct.ViewModel
{
    public class MacrocycleViewModel : BaseViewModel
    {
        /// <summary>
        /// The Path to open from
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The opened Macrocycle
        /// </summary>
        public Macrocycle Macrocycle { get; set; }

        private Atom _selectedItem;
        /// <summary>
        /// Gets or Sets the selected Atom
        /// </summary>
        public Atom SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value, () =>
            {
                foreach (var atom in Atoms3D)
                    atom.IsSelected = atom.Atom.Equals(_selectedItem);
            });
        }
        /// <summary>
        /// 3D Representation of Atoms
        /// </summary>
        public ObservableCollection<AtomVisual3D> Atoms3D { get; }
        /// <summary>
        /// 3D Representation of Bonds
        /// </summary>
        public ObservableCollection<BondVisual3D> Bonds3D { get; }

        public MacrocycleViewModel()
        {
            Path = @"D:\Desktop\PorphyStruct_CIFS_PAPER\ps16bw_compl.cif";
            Macrocycle = new Macrocycle(Path);
            Atoms3D = new ObservableCollection<AtomVisual3D>(Macrocycle.Atoms.Select(s => new AtomVisual3D(s) { IsSelected = s.Equals(SelectedItem) }));
            Bonds3D = new ObservableCollection<BondVisual3D>(Macrocycle.Bonds.Select(s => new BondVisual3D(s)));
        }
    }
}

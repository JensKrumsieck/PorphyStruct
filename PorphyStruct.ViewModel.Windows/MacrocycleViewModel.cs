using ChemSharp.Molecules;
using PorphyStruct.ViewModel.Windows.Visual;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace PorphyStruct.ViewModel.Windows
{
    public class MacrocycleViewModel : ViewModel.MacrocycleViewModel
    {
        private Atom _selectedAtom;
        /// <summary>
        /// Gets or Sets the selected Atom
        /// </summary>
        public Atom SelectedAtom
        {
            get => _selectedAtom;
            set => Set(ref _selectedAtom, value, () =>
            {
                foreach (var atom in Atoms3D)
                    atom.IsSelected = atom.Atom.Equals(_selectedAtom);
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

        public MacrocycleViewModel(string path) : base(path)
        {
            Atoms3D = new ObservableCollection<AtomVisual3D>(Macrocycle.Atoms.Select(s => new AtomVisual3D(s) { IsSelected = s.Equals(SelectedAtom) }));
            Bonds3D = new ObservableCollection<BondVisual3D>(Macrocycle.Bonds.Select(s => new BondVisual3D(s)));
        }

        protected override void Validate()
        {
            base.Validate();
            foreach (var a in Atoms3D)
                a.IsValid = false;

            foreach (var part in Macrocycle.DetectedParts)
            {
                foreach (var a3d in part.Atoms.Select(a => Atoms3D.FirstOrDefault(s => s.Atom.Equals(a))).Where(a3d => a3d != null))
                    a3d.IsValid = true;

                var validBonds = Bonds3D.Where(bond => part.Atoms.Count(atom => bond.Bond.Atoms.Contains(atom)) == 2);
                foreach (var bond in validBonds)
                {
                    bond.Color = (Color)ColorConverter.ConvertFromString(part.AnalysisColor)!;
                    bond.IsValid = true;
                }
            }
        }
    }
}

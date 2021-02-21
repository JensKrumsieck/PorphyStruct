using ChemSharp.Molecules;
using OxyPlot;
using PorphyStruct.ViewModel.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using PorphyStruct.Core;
using TinyMVVM;

namespace PorphyStruct.ViewModel
{
    public class MacrocycleViewModel : ListingViewModel<AnalysisViewModel>
    {
        /// <summary>
        /// The Path to open from
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The opened Macrocycle
        /// </summary>
        public Macrocycle Macrocycle { get; set; }

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

        public PlotModel Model { get; set; }

        /// <summary>
        /// 3D Representation of Atoms
        /// </summary>
        public ObservableCollection<AtomVisual3D> Atoms3D { get; }
        /// <summary>
        /// 3D Representation of Bonds
        /// </summary>
        public ObservableCollection<BondVisual3D> Bonds3D { get; }

        public MacrocycleViewModel(string path)
        {
            Filename = path;
            Macrocycle = new Macrocycle(Filename);
            Atoms3D = new ObservableCollection<AtomVisual3D>(Macrocycle.Atoms.Select(s => new AtomVisual3D(s) { IsSelected = s.Equals(SelectedAtom) }));
            Bonds3D = new ObservableCollection<BondVisual3D>(Macrocycle.Bonds.Select(s => new BondVisual3D(s)));
        }

        public override string Title => Path.GetFileNameWithoutExtension(Filename);

        /// <summary>
        /// Runs Detection Algorithm
        /// </summary>
        /// <returns></returns>
        public async Task Analyze()
        {
            Items.Clear();
            await Task.Run(Macrocycle.Detect);
            Validate();
            foreach (var part in Macrocycle.DetectedParts)
            {
                var analysis = new AnalysisViewModel(this) { Analysis = part };
                await Task.Run(analysis.Analyze);
                Items.Add(analysis);
                SelectedIndex = Items.IndexOf(analysis);
            }
        }

        private void Validate()
        {
            foreach (var part in Macrocycle.DetectedParts)
            {
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

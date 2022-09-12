using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Windows.Media;
using ChemSharp;
using ChemSharp.Molecules;
using ChemSharp.Molecules.HelixToolkit;
using PorphyStruct.Core.Extension;

namespace PorphyStruct.ViewModel.Windows;

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
    public ObservableCollection<Atom3D> Atoms3D { get; }
    /// <summary>
    /// 3D Representation of Bonds
    /// </summary>
    public ObservableCollection<Bond3D> Bonds3D { get; }

    public MacrocycleViewModel(string path) : base(path)
    {
        //with that number of Atoms it probably is a HUGE protein, so leave out amino acids for performance
        if (Macrocycle.Atoms.Count > 15000)
        {
            var subset = Macrocycle.Atoms
                                   .Where(s => !Constants.AminoAcids.ContainsKey(s.Residue)).ToList();
            Atoms3D = new ObservableCollection<Atom3D>(subset.Select(s => new Atom3D(s)
            {
                IsSelected = s.Equals(SelectedAtom)
            }));
            Bonds3D = new ObservableCollection<Bond3D>(Macrocycle.Bonds
                                                                 .Where(b => subset.Contains(b.Atom1) && subset.Contains(b.Atom2))
                                                                 .Select(s => new Bond3D(s)));
        }
        else
        {
            Atoms3D = new ObservableCollection<Atom3D>(Macrocycle.Atoms.Select(s => new Atom3D(s)
            {
                IsSelected = s.Equals(SelectedAtom)
            }));
            Bonds3D = new ObservableCollection<Bond3D>(Macrocycle.Bonds.Select(s => new Bond3D(s)));
        }
    }

    protected override void Validate()
    {
        base.Validate();
        foreach (var a in Atoms3D)
            a.IsValid = false;

        foreach (var part in Macrocycle.DetectedParts)
        {
            foreach (var a3d in part.Atoms.Select(a => Atoms3D.FirstOrDefault(s => s.Atom.Equals(a))).Where(a3d => a3d != null))
                a3d!.IsValid = true;

            var validBonds = Bonds3D.Where(bond => part.Atoms.Count(atom => bond.Bond.Atoms.Contains(atom)) == 2);
            foreach (var bond in validBonds)
            {
                bond.Color = (Color)ColorConverter.ConvertFromString(part.AnalysisColor)!;
                bond.IsValid = true;
            }
        }
    }
}

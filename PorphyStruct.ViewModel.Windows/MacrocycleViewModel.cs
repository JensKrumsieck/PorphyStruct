using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ChemSharp;
using ChemSharp.Molecules;
using ChemSharp.Molecules.HelixToolkit;
using HelixToolkit.Wpf;

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
    public ObservableCollection<Atom3D> Atoms3D { get; private set; }

    /// <summary>
    /// 3D Representation of Bonds
    /// </summary>
    public ObservableCollection<Bond3D> Bonds3D { get; private set; }

    public ObservableCollection<TubeVisual3D> Tubes { get; private set; }

    public MacrocycleViewModel(string path) : base(path) => Init();

    private void Init()
    {
        var subset = Macrocycle.Atoms.Where(a => Constants.AminoAcids.ContainsKey(a.Residue)).ToHashSet();
        var residual = Macrocycle.Atoms.Where(a => a.Residue != "HOH").Except(subset).ToHashSet();
        var selectedBonds3D = Macrocycle.Bonds.Where(b => residual.Contains(b.Atom1) && residual.Contains(b.Atom2))
            .Select(b => new Bond3D(b));
        
        Atoms3D = new ObservableCollection<Atom3D>(residual.Select(a => new Atom3D(a)));
        Bonds3D = new ObservableCollection<Bond3D>(selectedBonds3D);
        Tubes = new ObservableCollection<TubeVisual3D>();
        
        var groups = subset.GroupBy(a => a.ChainId);
        foreach (var g in groups) BuildSpline(g);
    }

    private void BuildSpline(IEnumerable<Atom> g)
    {
        var points = g.GroupBy(a => a.ResidueId).Select(s => s.First().Location.ToPoint3D()).ToList();
        var spline = CanonicalSplineHelper.CreateSpline(points);
        var tube = new TubeVisual3D
        {
            Path = new Point3DCollection(spline),
            IsPathClosed = false,
            Diameter = .75,
            ThetaDiv = 12,
            Material = MaterialHelper.CreateMaterial(Colors.Green, .5d),
            BackMaterial = null
        };
        Tubes.Add(tube);
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

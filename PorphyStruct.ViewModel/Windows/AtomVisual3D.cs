using ChemSharp.Molecules;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PorphyStruct.ViewModel.Windows
{
    public class AtomVisual3D : ModelVisual3D
    {
        public Atom Atom { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            var brush = (Brush)new BrushConverter().ConvertFromString(Atom.Color);
            ((GeometryModel3D)Content).Material = MaterialHelper.CreateMaterial(IsSelected ? Brushes.LightGoldenrodYellow : brush, 0, 0);
        }

        public AtomVisual3D(Atom atom)
        {
            var builder = new MeshBuilder();
            builder.AddSphere(atom.Location.ToPoint3D(), (double)(atom.CovalentRadius ?? 100) / 200);
            var brush = (Brush)new BrushConverter().ConvertFromString(atom.Color);
            Atom = atom;
            Content = new GeometryModel3D(builder.ToMesh(), MaterialHelper.CreateMaterial(brush, 0, 0));
        }

        public override string ToString() => Atom + (IsSelected ? " SelectedAtom" : "");
    }
}

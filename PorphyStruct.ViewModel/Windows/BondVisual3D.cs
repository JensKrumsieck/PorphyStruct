using ChemSharp.Molecules;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PorphyStruct.ViewModel.Windows
{
    public class BondVisual3D : ModelVisual3D
    {
        public Bond Bond { get; set; }

        public BondVisual3D(Bond bond)
        {
            var builder = new MeshBuilder();
            builder.AddCylinder(bond.Atom1.Location.ToPoint3D(), bond.Atom2.Location.ToPoint3D(), 0.075, 10);
            var brush = Brushes.DarkGray.Clone();
            Content = new GeometryModel3D(builder.ToMesh(), MaterialHelper.CreateMaterial(brush, 0, 0));
            Bond = bond;
        }
    }
}

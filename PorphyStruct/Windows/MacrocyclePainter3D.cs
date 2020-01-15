using HelixToolkit.Wpf;
using PorphyStruct.Chemistry;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PorphyStruct.Windows
{
    /// <summary>
    /// Painting for Windows specific Stuff
    /// </summary>
    public static class MacrocyclePainter3D
    {
        /// <summary>
        /// Paint the Molecule in 3D
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static Model3DGroup Paint3D(this Macrocycle cycle, Atom selected = null)
        {
            Model3DGroup group = new Model3DGroup();
            //loop atoms
            foreach (var atom in cycle.Atoms)
            {
                var builder = new MeshBuilder(true, true);
                builder.AddSphere(atom.ToPoint3D(), atom.AtomRadius / 2);
                Brush brush = atom.Brush();
                if (selected != null && atom == (selected)) brush = Brushes.LightGoldenrodYellow;
                group.Children.Add(new GeometryModel3D(builder.ToMesh(), MaterialHelper.CreateMaterial(brush)));
            }
            //loop bonds
            foreach (var a1 in cycle.Atoms)
            {
                foreach (var a2 in cycle.Atoms)
                {
                    var builder = new MeshBuilder(true, true);
                    if (a1.BondTo(a2))
                        builder.AddCylinder(a1.ToPoint3D(), a2.ToPoint3D(), cycle.IsValidBond(a1, a2) ? 0.2 : 0.075, 10);//add only to selection if both are macrocycle marked
                    group.Children.Add(new GeometryModel3D(builder.ToMesh(), cycle.IsValidBond(a1, a2) ? Materials.Blue : Materials.Gray));
                }
            }
            return group;
        }
    }
}

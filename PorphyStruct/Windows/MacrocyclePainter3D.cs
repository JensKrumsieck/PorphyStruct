using HelixToolkit.Wpf;
using PorphyStruct.Chemistry;
using System.Collections.Generic;
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
        public static IEnumerable<ModelVisual3D> Paint3D(this Macrocycle cycle)
        {
            yield return new DefaultLights();
            //loop atoms
            foreach (var atom in cycle.Atoms)
                    yield return Atom3D(atom);
            //loop bonds
            foreach (var a1 in cycle.Atoms)
            {
                foreach (var a2 in cycle.Atoms)
                {
                    if (a1.BondTo(a2))
                    {
                        yield return a1.Bond3D(a2, cycle);
                    }
                }
            }
        }

        /// <summary>
        /// Builds 3D Model from Atom
        /// </summary>
        /// <param name="atom"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static AtomModelVisual3D Atom3D(this Atom atom, bool selected = false)
        {
            var builder = new MeshBuilder(true, true);
            builder.AddSphere(atom.ToPoint3D(), atom.AtomRadius / 2);
            Brush brush = atom.Brush();
            if (selected) brush = Brushes.LightGoldenrodYellow;
            var vis = new AtomModelVisual3D();
            vis.Content = new GeometryModel3D(builder.ToMesh(), MaterialHelper.CreateMaterial(brush));
            vis.Atom = atom;
            return vis;
        }

        /// <summary>
        /// Draws 3d Models for Bonds
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="cycle"></param>
        /// <returns></returns>
        public static BondModelVisual3D Bond3D(this Atom a1, Atom a2, Macrocycle cycle)
        {
            var valid = cycle.IsValid;
            var builder = new MeshBuilder(true, true);
            builder.AddCylinder(a1.ToPoint3D(), a2.ToPoint3D(), cycle.IsValidBond(a1, a2) ? 0.2 : 0.075, 10);//add only to selection if both are macrocycle marked
            var vis = new BondModelVisual3D();
            vis.Atoms = new[] { a1, a2 };
            var material = Materials.Gray;
            if (cycle.IsValidBond(a1, a2))
                material = (valid ? Materials.Green : Materials.Red);
            vis.Content = new GeometryModel3D(builder.ToMesh(), material);
            return vis;
        }
    }
}

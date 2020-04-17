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
            foreach (Atom atom in cycle.Atoms) yield return Atom3D(atom);
            //loop bonds
            foreach (Atom a1 in cycle.Atoms)
            {
                foreach (Atom a2 in cycle.Atoms)
                {
                    if (a1.BondTo(a2)) yield return a1.Bond3D(a2, cycle);
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
            var builder = new MeshBuilder();
            builder.AddSphere(atom.ToPoint3D(), atom.AtomRadius / 2);
            Brush brush = atom.Brush();
            if (Core.Properties.Settings.Default.PaintNonMacrocyclicAtoms && !atom.IsMacrocycle) brush.Opacity = 1 - (Core.Properties.Settings.Default.StrengthNonMacrocyclicAtoms / 100d);
            if (selected) brush = Brushes.LightGoldenrodYellow;
            return new AtomModelVisual3D
            {
                Content = new GeometryModel3D(builder.ToMesh(), MaterialHelper.CreateMaterial(brush, 0, 0)) { BackMaterial = MaterialHelper.CreateMaterial(brush, 0, 0) },
                Atom = atom
            };
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
            bool valid = cycle.IsValid;
            var builder = new MeshBuilder();
            builder.AddCylinder(a1.ToPoint3D(), a2.ToPoint3D(), cycle.IsValidBond(a1, a2) ? 0.2 : 0.075, 10);//add only to selection if both are macrocycle marked
            Brush brush = Brushes.Gray.Clone();
            if (cycle.IsValidBond(a1, a2)) brush = (valid ? Brushes.Green.Clone() : Brushes.Red.Clone());
            else if (Core.Properties.Settings.Default.PaintNonMacrocyclicAtoms) brush.Opacity = 1 - (Core.Properties.Settings.Default.StrengthNonMacrocyclicAtoms / 100d);
            return new BondModelVisual3D
            {
                Atoms = new[] { a1, a2 },
                Content = new GeometryModel3D(builder.ToMesh(), MaterialHelper.CreateMaterial(brush, 0, 0)) { BackMaterial = MaterialHelper.CreateMaterial(brush, 0, 0) }
            };
        }
    }
}

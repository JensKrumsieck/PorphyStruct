using PorphyStruct.Chemistry;
using System.Windows.Media.Media3D;

namespace PorphyStruct.Windows
{
    /// <summary>
    /// Extends ModelVisual3D so that an Atom can be appended.
    /// </summary>
    public class AtomModelVisual3D : ModelVisual3D
    {
        /// <summary>
        /// The Atom
        /// </summary>
        public Atom Atom { get; set; }
    }

    public class BondModelVisual3D : ModelVisual3D
    {
        public Atom[] Atoms { get; set; }
    }
}

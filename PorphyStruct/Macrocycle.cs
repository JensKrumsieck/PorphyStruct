using ChemSharp.Molecules;

namespace PorphyStruct
{
    public class Macrocycle : Molecule
    {
        public Macrocycle(string path) : base(MoleculeFactory.CreateProvider(path))
        { }
    }
}

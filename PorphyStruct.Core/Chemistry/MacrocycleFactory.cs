using PorphyStruct.Chemistry.Macrocycles;
using PorphyStruct.Core.Util;
using PorphyStruct.Files;
using System.IO;

namespace PorphyStruct.Chemistry
{
    public static class MacrocycleFactory
    {
        /// <summary>
        /// Builds a Macrocyle by given Type
        /// </summary>
        /// <param name="Atoms"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Macrocycle Build(AsyncObservableCollection<Atom> Atoms, Macrocycle.Type type)
        {
            return type switch
            {
                Macrocycle.Type.Corrole => new Corrole(Atoms),
                Macrocycle.Type.Norcorrole => new Norcorrole(Atoms),
                Macrocycle.Type.Porphyrin => new Porphyrin(Atoms),
                Macrocycle.Type.Corrphycene => new Corrphycene(Atoms),
                Macrocycle.Type.Porphycene => new Porphycene(Atoms),
                _ => null,
            };
        }

        /// <summary>
        /// Loads Macrocycle Object by path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Macrocycle Load(string path, Macrocycle.Type type)
        {
            //get molecule
            var molecule = Load(path);

            Macrocycle cycle = Build(molecule.Atoms, type);
            cycle.Title = Path.GetFileNameWithoutExtension(path);
            return cycle;
        }

        /// <summary>
        /// Loads Molecule
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Molecule Load(string path)
        {
            TextFile file;
            if (Path.GetExtension(path) == ".cif")
                file = new CifFile(path);
            else if (Path.GetExtension(path) == ".mol" || Path.GetExtension(path) == ".mol2")
                file = new Mol2File(path);
            else if (Path.GetExtension(path) == ".ixyz")
                file = new XYZFile(path, true);
            else
                file = new XYZFile(path);
            //get molecule
            return file.GetMolecule();
        }
    }
}

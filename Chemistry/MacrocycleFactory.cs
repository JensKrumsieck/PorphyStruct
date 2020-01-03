using PorphyStruct.Chemistry.Macrocycles;
using PorphyStruct.Files;
using System.Collections.Generic;
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
        public static Macrocycle Build(List<Atom> Atoms, Macrocycle.Type type)
        {
            switch (type)
            {
                case Macrocycle.Type.Corrole: return new Corrole(Atoms);
                case Macrocycle.Type.Norcorrole: return new Norcorrole(Atoms);
                case Macrocycle.Type.Porphyrin: return new Porphyrin(Atoms);
                case Macrocycle.Type.Corrphycene: return new Corrphycene(Atoms);
                case Macrocycle.Type.Porphycene: return new Porphycene(Atoms);
                default: return null;
            }
        }

        /// <summary>
        /// Loads Macrocycle Object by path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Macrocycle Load(string path, Macrocycle.Type type)
        {
            //handle file open
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
            var molecule = file.GetMolecule();

            Macrocycle cycle = Build(molecule.Atoms, type);
            cycle.SetIsMacrocycle(type);
            return cycle;
        }
    }
}

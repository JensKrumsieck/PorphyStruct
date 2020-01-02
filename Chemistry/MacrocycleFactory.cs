using PorphyStruct.Chemistry.Macrocycles;
using System.Collections.Generic;

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
    }
}

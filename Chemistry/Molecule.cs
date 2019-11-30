using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PorphyStruct.Chemistry
{
    public class Molecule
    {
        public string Title = "";
        //public List<Atom> Atoms = new List<Atom>();
        public List<Atom> Atoms = new List<Atom>();

        /// <summary>
        /// Construct Molecule
        /// </summary>
        public Molecule() { }

        /// <summary>
        /// Construct Molecule with title
        /// <seealso cref="Molecule"/>
        /// </summary>
        /// <param name="title"></param>
        public Molecule(string title)
        {
            this.Title = title;
        }

        /// <summary>
        /// Construct Molecule with AtomList
        /// <seealso cref="Molecule"/>
        /// </summary>
        /// <param name="title"></param>
        public Molecule(List<Atom> Atoms)
            : this()
        {
            this.Atoms = Atoms;
        }

        /// <summary>
        /// Set Is Macrocycle for Atoms
        /// </summary>
        /// <param name="type"></param>
        public void SetIsMacrocycle(Macrocycle.Type type)
        {
            bool idiot = false;
            if (Atoms.Where(s => s.Identifier.StartsWith("N0")).Count() != 0) idiot = true; //haha

            foreach (Atom atom in Atoms)
            {
                //set everything to false first
                atom.IsMacrocycle = false;

                //increment atom id for those who start counting at 0
                if (idiot && (atom.Type == "C" || atom.Type == "N")) atom.Identifier = atom.Type + atom.ID + 1;

                //adjust core to N1->N4
                if (atom.Type == "N")
                {   
                    if (atom.ID >= 21 && atom.ID <= 24)
                    atom.Identifier = "N" + (atom.ID - 20) + atom.Suffix;
                }

                //remove suffix if suffix is A (primary). Maybe suffix selection in future
                if (atom.Suffix == "A" || atom.Suffix == "a") atom.Identifier = atom.Type + atom.ID;

                //c between 0 and 20 ; n between 1 and 4
                if (((atom.Type == "C" && atom.ID <= 20) || (atom.Type == "N" && atom.ID <= 4)) && string.IsNullOrEmpty(atom.Suffix)) atom.IsMacrocycle = true;
                //corroles and norcorroles missing 1 atom
                if ((type == Macrocycle.Type.Corrole || type == Macrocycle.Type.Norcorrole) && atom.ID >= 20) atom.IsMacrocycle = false;
            }
        }
    }
}

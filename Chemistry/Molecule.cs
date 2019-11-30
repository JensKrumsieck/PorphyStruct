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
        public Molecule()
        {
            //does nothing right now..
        }

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
            if (this.Atoms.Where(s => s.Identifier.StartsWith("N0")).Count() != 0)
            {
                idiot = true; //haha
            }

            foreach (Atom atom in this.Atoms)
            {
                //set everything to true first and then turn off if not meeting preferences
                atom.IsMacrocycle = true;


                //automatic detection sets all non C,N Atoms to false! 
                if (atom.Type != "C" && atom.Type != "N")
                {
                    //this detection fails for any heteroatoms in the core!
                    atom.IsMacrocycle = false;
                }                

                //remove secondary structure (prime or A/B)
                if (atom.Identifier.Contains("'") || atom.Identifier.Contains("b") || atom.Identifier.Contains("B"))
                {
                    atom.IsMacrocycle = false;
                }
                else if (atom.Identifier.EndsWith("A"))
                {
                    atom.Identifier = Regex.Match(atom.Identifier, @"([A-Z][a-z]*)(\d*\,{0,1}\d*)").Value;
                }
                
                //workaround for those who start counting at 0
                if (idiot)
                {
                    if (atom.Identifier.Contains("C"))
                    {
                        atom.Identifier = "C" + (atom.ID + 1);
                    }
                    if (atom.Identifier.Contains("N"))
                    {
                        atom.Identifier = "N" + (atom.ID + 1);
                    }
                }

                //most cif files either number by IUPAC or ascending for C and 1,2,3,4 for N, so remove all other atoms
                if (atom.Type == "C" && atom.IsMacrocycle)
                {
                    //carbon atoms from 1-19 for corroles & norcorroles and 20 for porphyrins, corrphycenes & porphycenes... 
                    if ((type == Macrocycle.Type.Corrole || type == Macrocycle.Type.Norcorrole) && atom.ID > 19)
                        atom.IsMacrocycle = false;
                    //greater than 20 -> no porphyrinoid
                    else if (atom.ID > 20)
                        atom.IsMacrocycle = false;
                }
                //as cavity is always N4 (or detection fails by design!) not type specific matching is needed.
                if (atom.Type == "N" && atom.IsMacrocycle)
                {
                    if (atom.ID > 24) atom.IsMacrocycle = false; //definitivly no macrocycle (or some kind of azarocorrole maybe?)
                    else
                    {
                        //id <= 24
                        if (atom.ID >= 21 && atom.ID <= 24)
                            atom.Identifier = "N" + (atom.ID - 20);
                       
                        if (atom.ID > 4) atom.IsMacrocycle = false; //set everything to false if greater than N4!
                    }
                }
            }
        }
    }
}

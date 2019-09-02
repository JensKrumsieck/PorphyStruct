﻿using System.Collections.Generic;
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
                idiot = true;
            }

            foreach (Atom atom in this.Atoms)
            {
                //set everythin to true first and then turn off if not meeting preferences
                atom.isMacrocycle = true;


                //automatic detection sets all non C,N Atoms to false! Note that! -> Contains(H) -> H20C and ...
                if (!atom.Identifier.Contains("C") && !atom.Identifier.Contains("N") || atom.Identifier.Contains("H"))
                {
                    //this detection fails for any heteroatoms in the core!
                    atom.isMacrocycle = false;

                }
                //fails for chlorine, nickel, niobium, ... so sort them out if string contains lowercase
                else if (Regex.Matches(atom.Identifier, @"[a-z]", RegexOptions.Multiline).Count > 0)
                {
                    atom.isMacrocycle = false;
                }

                //remove secondary structure (prime or A/B)
                if (atom.Identifier.Contains("'") || atom.Identifier.Contains("b") || atom.Identifier.Contains("B"))
                {
                    atom.isMacrocycle = false;
                }
                else if (atom.Identifier.EndsWith("A"))
                {
                    atom.Identifier = Regex.Match(atom.Identifier, @"([A-Z][a-z]*)(\d*\,{0,1}\d*)").Value;
                }

                //get atom ID
                int id = 0;
                int.TryParse(Regex.Match(atom.Identifier, @"\d+").Value, out id);

                //workaround for idiots that start atom numbering with 0...
                if (idiot)
                {
                    if (atom.Identifier.Contains("C"))
                    {
                        atom.Identifier = "C" + (id + 1);
                    }
                    if (atom.Identifier.Contains("N"))
                    {
                        atom.Identifier = "N" + (id + 1);
                    }
                }

                //most cif files either number by IUPAC or ascending for C and 1,2,3,4 for N, so remove all other atoms
                if (atom.Identifier.Contains("C") && atom.isMacrocycle)
                {
                    //carbon atoms from 1-19 for corroles & norcorroles and 20 for porphyrins, corrphycenes & porphycenes... 
                    if (type == Macrocycle.Type.Corrole || type == Macrocycle.Type.Norcorrole)
                    {
                        //greater than 19 == no corrole
                        if (id > 19) atom.isMacrocycle = false;
                    }
                    else
                    {
                        //greater than 20 == no porphyrin
                        if (id > 20) atom.isMacrocycle = false;
                    }
                }
                //as cavity is always N4 (or detection fails by design!) not type specific matching is needed.
                if (atom.Identifier.Contains("N") && atom.isMacrocycle)
                {
                    if (id > 24) atom.isMacrocycle = false; //definitivly no macrocycle (or some kind of azarocorrole maybe?)
                    else
                    {
                        //id <= 24
                        if (id >= 21 && id <= 24)
                        {
                            //alternative (IUPAC) numbering
                            id = id - 20;
                            //new identifier so everything is fine
                            atom.Identifier = "N" + id;
                        }
                        if (id > 4) atom.isMacrocycle = false; //set everything to false if greater than N4!
                    }
                }
            }
        }
    }
}

using PorphyStruct.Chemistry;
using System;
using System.Globalization;
using System.Linq;

namespace PorphyStruct.Files
{
    class XYZFile : TextFile
    {
        /// <summary>
        /// is ixyz format used?
        /// </summary>
        public bool IXYZ = false;
        public XYZFile(string path) : base(path) { }

        public XYZFile(string path, bool isIXYZ) : base(path) => IXYZ = isIXYZ;

        /// <summary>
        /// Gets Molecule from XYZ File
        /// </summary>
        /// <param name="isIXYZ">Is PorphyStruct iXYZ File</param>
        /// <returns></returns>
        public override Molecule GetMolecule()
        {
            //PLEASE DO NOT USE XYZ Files with non cartesian coordinates
            string title = System.IO.Path.GetFileNameWithoutExtension(Path);
            Molecule molecule = new Molecule(title);

            for (int i = 0; i <= Lines.Count() - 1; i++)
            {
                if (i > 1 && !String.IsNullOrEmpty(Lines[i]))
                {
                    string[] xyzLine = Lines[i].Split(new[] { " ", "\t" }, StringSplitOptions.None).Where(j => !string.IsNullOrEmpty(j)).ToArray();
                    Atom a = new Atom((IXYZ ? xyzLine[0].Split('/')[0] : xyzLine[0]),
                        Convert.ToDouble(xyzLine[1], CultureInfo.InvariantCulture),
                        Convert.ToDouble(xyzLine[2], CultureInfo.InvariantCulture),
                        Convert.ToDouble(xyzLine[3], CultureInfo.InvariantCulture));
                    if (IXYZ)
                        a.Element = Element.Create(xyzLine[0].Split('/')[1]);
                    molecule.Atoms.Add(a);
                }
            }
            //if is iXYZ the identifier is set correctly (hopefully!)
            if (!IXYZ) molecule = GuessIdentifiers(molecule);

            return molecule;
        }

        /// <summary>
        /// Guessing Identifiers by just enumerating C&N Atoms
        /// Will fail often, but we have a detect method in macrocycle class
        /// for later use
        /// </summary>
        /// <param name="molecule"></param>
        /// <returns></returns>
        private Molecule GuessIdentifiers(Molecule molecule)
        {
            //making a guess!
            int C = 1;
            int N = 1;

            foreach (Atom a in molecule.Atoms)
            {
                if (a.Identifier == "C")
                {
                    a.Identifier = "C" + C;
                    C++;
                }
                if (a.Identifier == "N")
                {
                    a.Identifier = "N" + N;
                    N++;
                }
            }
            return molecule;
        }
    }
}

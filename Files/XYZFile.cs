using PorphyStruct.Chemistry;
using System;
using System.Globalization;
using System.Linq;

namespace PorphyStruct.Files
{
    class XYZFile : TextFile
    {

        public XYZFile(string path) : base(path) { }

        /// <summary>
        /// Gets Molecule from XYZ File
        /// </summary>
        /// <param name="isIXYZ">Is PorphyStruct iXYZ File</param>
        /// <returns></returns>
        public Molecule GetMolecule(bool isIXYZ = false)
        {
            //atom count is first line, second line is title
            //PLEASE DO NOT USE XYZ Files with non cartesian coordinates
            string title = System.IO.Path.GetFileNameWithoutExtension(this.Path);
            Molecule molecule = new Molecule(title);

            //works fine...but where to get Atom Numbering??
            for (int i = 0; i <= Lines.Count() - 1; i++)
            {
                if (i > 1 && Lines[i] != "")
                {
                    string[] xyzLine = Lines[i].Split(new[] { " ", "\t" }, StringSplitOptions.None);
                    xyzLine = xyzLine.Where(j => !string.IsNullOrEmpty(j)).ToArray();

                    string identifier = "";
                    identifier = xyzLine[0];
                    if (isIXYZ)
                        identifier = xyzLine[0].Split('/')[0];
                    double x = Convert.ToDouble(xyzLine[1], CultureInfo.InvariantCulture);
                    double y = Convert.ToDouble(xyzLine[2], CultureInfo.InvariantCulture);
                    double z = Convert.ToDouble(xyzLine[3], CultureInfo.InvariantCulture);
                    Atom a = new Atom(identifier, x, y, z);
                    if (isIXYZ)
                    {
                        try
                        { a.Element = Element.Create(xyzLine[0].Split('/')[1]); }
                        catch { }
                    }
                    molecule.Atoms.Add(a);
                }
            }

            //if is iXYZ the identifier is set correctly (hopefully!)
            if (!isIXYZ)
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
            }
            return molecule;
        }
    }
}

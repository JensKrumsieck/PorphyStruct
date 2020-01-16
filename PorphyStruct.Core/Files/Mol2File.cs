using PorphyStruct.Chemistry;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PorphyStruct.Files
{
    public class Mol2File : TextFile
    {

        public Mol2File(string path) : base(path) { }

        /// <summary>
        /// Gets Molecule from Mol2 File
        /// </summary>
        /// <returns></returns>
        public override Molecule GetMolecule()
        {
            // read mol2 - File and get parameters & coordinate
            string title = System.IO.Path.GetFileNameWithoutExtension(this.Path);
            Molecule molecule = new Molecule(title);
            //get loop with coordinates
            string[] tripos = Content.Split(new[] { "@<TRIPOS>" }, StringSplitOptions.None);
            string atomTripos = tripos.FirstOrDefault(s => s.StartsWith("ATOM"));
            string[] atoms = atomTripos.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None);

            //guessing
            int C = 1;
            int N = 1;
            foreach (string atom in atoms)
            {
                if (atom != "ATOM" && atom != "") //don't use "ATOM" starting line
                {
                    //there are two possibilities: 1	C	[coords] vs 1    C1    [coords]
                    string[] columns = atom.Split(new[] { " ", "\t" }, StringSplitOptions.None);
                    columns = columns.Where(j => !string.IsNullOrEmpty(j)).ToArray();
                    string identifier = columns[1];
                    double x = Convert.ToDouble(columns[2], CultureInfo.InvariantCulture);
                    double y = Convert.ToDouble(columns[3], CultureInfo.InvariantCulture);
                    double z = Convert.ToDouble(columns[4], CultureInfo.InvariantCulture);

                    int id = 0;
                    int.TryParse(Regex.Match(identifier, @"\d+").Value, out id);
                    //no id, so set one.
                    if (id == 0)
                    {
                        if (identifier == "C")
                        {
                            identifier += C.ToString();
                            C++;
                        }
                        if (identifier == "N")
                        {
                            identifier += N.ToString();
                            N++;
                        }
                    }
                    molecule.Atoms.Add(new Atom(identifier, x, y, z));
                }
            }
            return molecule;
        }
    }
}

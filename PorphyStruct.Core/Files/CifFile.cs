using PorphyStruct.Chemistry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Files
{
    public class CifFile : TextFile
    {

        /// <summary>
        /// Constructs a CIF File Object from given File
        /// </summary>
        /// <param name="path">Filepath</param>
        public CifFile(string path)
            : base(path) { }

        /// <summary>
        /// Builds a Molecule/Crystal Object out of raw data
        /// </summary>
        /// <returns>Crystal</returns>
        public override Molecule GetMolecule()
        {
            //get cell parameters
            double[] cellLenghts = GetCellParameters("cell_length").ToArray();
            double[] cellAngles = GetCellParameters("cell_angle").ToArray();

            //get loop with coordinates
            string[] loops = Content.Split(new[] { "loop_" }, StringSplitOptions.None);

            //this loop contains molecule!
            string moleculeLoop = Array.Find(loops, s => s.Contains("_atom_site_label"));

            List<string[]> data = new List<string[]>();
            int headers = moleculeLoop.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None).Count(s => s.Trim().StartsWith("_"));
            //loop through lines
            foreach (string line in moleculeLoop.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None))
            {
                if (!line.TrimStart().StartsWith("_")) data.Add(line.Split(' '));
            }

            //build returning object
            Crystal crystal = new Crystal(System.IO.Path.GetFileNameWithoutExtension(this.Path),
                cellLenghts, cellAngles, data, headers);

            return crystal.ToMolecule();
        }

        /// <summary>
        /// returns cell parameters
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<double> GetCellParameters(string type)
        {
            foreach (string l in Lines)
            {
                //everythin starts \w underscore
                if (l.StartsWith("_" + type))
                {
                    string value = l.Split(' ').Last().Split('(')[0];
                    yield return Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        }


    }
}

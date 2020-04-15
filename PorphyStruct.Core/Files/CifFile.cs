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

            var data = new List<string[]>();
            var headers = moleculeLoop.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None).Where(s => s.Trim().StartsWith("_")).ToArray();
            int disorderGroupIndex = Array.IndexOf(headers, "_atom_site_disorder_group");
            //loop through lines
            foreach (string line in moleculeLoop.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None).Where(s => !s.StartsWith("_")))
            {
                var raw_data = line.Split(' ');
                if (Core.Properties.Settings.Default.IgnoreDisorder
                    && disorderGroupIndex >= 0
                    && disorderGroupIndex < raw_data.Count()
                    && raw_data[disorderGroupIndex] == "2") continue;  //checks if disorderGroupIndex is equal to 2 which is not the "main" molecule

                data.Add(raw_data);
            }

            //build returning object
            var crystal = new Crystal(System.IO.Path.GetFileNameWithoutExtension(Path),
                cellLenghts, cellAngles, data, headers.Count());

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

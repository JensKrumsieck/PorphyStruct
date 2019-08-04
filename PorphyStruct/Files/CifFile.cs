using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Chemistry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Files
{
	class CifFile
    {
        public string Path = "";

		/// <summary>
		/// Constructs a CIF File Object from given File
		/// </summary>
		/// <param name="path">Filepath</param>
        public CifFile(string path)
        {
            this.Path = path;
        }

		/// <summary>
		/// Builds a Molecule/Crystal Object out of raw data
		/// </summary>
		/// <returns>Crystal</returns>
        public Crystal GetMolecule()
        {
            //read cif-File and get parameters & coordinates
            string text = System.IO.File.ReadAllText(this.Path);

            //get cell parameters
            double[] cellLenghts = new double[3];
            double[] cellAngles = new double[3];
            string[] lines = text.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None);
            int i = 0;
            int j = 0;
            foreach (string l in lines)
            {
                if (l.StartsWith("_cell_length"))
                {
                    string value = l.Split(' ').Last().Split('(')[0];
                    cellLenghts[i] = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
                    i++;
                }
                if (l.StartsWith("_cell_angle"))
                {
                    string value = l.Split(' ').Last().Split('(')[0];
                    cellAngles[j] = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
                    j++;
                }
            }

            //build returning object
            Crystal molecule = new Crystal(cellLenghts[0], cellLenghts[1], cellLenghts[2], cellAngles[0], cellAngles[1], cellAngles[2]);


            //get loop with coordinates
            string[] loops = text.Split(new[] { "loop_" }, StringSplitOptions.None);

            //this loop contains molecule!
            string moleculeLoop = Array.Find(loops, s => s.Contains("_atom_site_label"));

            List<string[]> data = new List<string[]>();
            List<string> headers = new List<string>();
            //loop through lines
            foreach (string line in moleculeLoop.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None))
            {
                if (line.TrimStart().StartsWith("_")) { headers.Add(line); }
                //get actual data
                else
                {
                    data.Add(line.Split(' '));
                }
            }

            //loop throug properties
            foreach (string[] props in data)
            {
                if (props.Length == headers.Count)
                {
                    //get xyz in fractional coordinates
                    //1 is label, 2 is x, 3 is y, 4 is z
                    double xfrac = Convert.ToDouble(props[2].Split('(')[0], System.Globalization.CultureInfo.InvariantCulture);
                    double yfrac = Convert.ToDouble(props[3].Split('(')[0], System.Globalization.CultureInfo.InvariantCulture);
                    double zfrac = Convert.ToDouble(props[4].Split('(')[0], System.Globalization.CultureInfo.InvariantCulture);

                    //build matrix for cartesian transformation //need Math.NET ... @see https://en.wikipedia.org/wiki/Fractional_coordinates#Conversion_to_Cartesian_coordinates
                    Matrix<double> A = DenseMatrix.OfArray(new double[,] {
                        { molecule.a, cellLenghts[1] * Math.Cos(molecule.gamma * Math.PI / 180), molecule.c * Math.Cos(molecule.beta * Math.PI / 180) },
                        { 0, cellLenghts[1] * Math.Sin(molecule.gamma * Math.PI / 180), molecule.c*(Math.Cos(molecule.alpha * Math.PI / 180)-Math.Cos(molecule.beta * Math.PI / 180)*Math.Cos(molecule.gamma * Math.PI / 180))/Math.Sin(molecule.gamma * Math.PI / 180) },
                        {0, 0, molecule.c*(Math.Sqrt(1-Math.Pow(Math.Cos(molecule.alpha * Math.PI / 180),2)-Math.Pow(Math.Cos(molecule.beta * Math.PI / 180),2)-Math.Pow(Math.Cos(molecule.gamma * Math.PI / 180),2)+2*Math.Cos(molecule.alpha * Math.PI / 180)*Math.Cos(molecule.beta * Math.PI / 180)*Math.Cos(molecule.gamma * Math.PI / 180)))/Math.Sin(molecule.gamma * Math.PI / 180)}
                    });
                    //coordinate vector
                    Vector<double> frac = DenseVector.OfArray(new double[] { xfrac, yfrac, zfrac });

                    //cartesian coordinates of current atom
                    Vector<double> coord = A * frac;

                    //add new atom to molecule with coordinates above
                    molecule.Atoms.Add(new Atom(props[0], coord[0], coord[1], coord[2]));
                }
            }

            return molecule;
        }

        
    }
}

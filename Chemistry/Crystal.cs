using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry
{
    /// <summary>
    /// wrapper for molecule with crystal parameters
    /// </summary>
    public class Crystal : Molecule
    {
        public double[] cellLenghts;
        public double[] cellAngles;
        /// <summary>
        /// Builds crystal object
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cellLenghts"></param>
        /// <param name="cellAngles"></param>
        public Crystal(string title, double[] cellLenghts, double[] cellAngles, List<string[]> raw_data, int count)
            : base(title)
        {
            this.cellAngles = cellAngles;
            this.cellLenghts = cellLenghts;

            //add atoms
            CartesianCoordinates(raw_data, count);
        }

        //Short names for cellangles/lenghts stay valid
        public double a => cellLenghts[0];
        public double b => cellLenghts[1];
        public double c => cellLenghts[2];

        public double alpha => cellAngles[0];
        public double beta => cellAngles[1];
        public double gamma => cellAngles[2];

        /// <summary>
        /// builds the cartesian coordinates from fraction coordinates
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        public void CartesianCoordinates(IEnumerable<string[]> data, int count)
        {
            //loop through properties
            foreach (string[] props in data)
            {
                if (props.Length == count)
                {
                    //get xyz in fractional coordinates
                    //1 is label, 2 is x, 3 is y, 4 is z
                    double xfrac = Convert.ToDouble(props[2].Split('(')[0], System.Globalization.CultureInfo.InvariantCulture);
                    double yfrac = Convert.ToDouble(props[3].Split('(')[0], System.Globalization.CultureInfo.InvariantCulture);
                    double zfrac = Convert.ToDouble(props[4].Split('(')[0], System.Globalization.CultureInfo.InvariantCulture);

                    //build matrix for cartesian transformation //need Math.NET ... @see https://en.wikipedia.org/wiki/Fractional_coordinates#Conversion_to_Cartesian_coordinates
                    Matrix<double> A = DenseMatrix.OfArray(new double[,] {
                        { a, b * Math.Cos(gamma * Math.PI / 180), c * Math.Cos(beta * Math.PI / 180) },
                        { 0, cellLenghts[1] * Math.Sin(gamma * Math.PI / 180), c*(Math.Cos(alpha * Math.PI / 180)-Math.Cos(beta * Math.PI / 180)*Math.Cos(gamma * Math.PI / 180))/Math.Sin(gamma * Math.PI / 180) },
                        {0, 0, c*(Math.Sqrt(1-Math.Pow(Math.Cos(alpha * Math.PI / 180),2)-Math.Pow(Math.Cos(beta * Math.PI / 180),2)-Math.Pow(Math.Cos(gamma * Math.PI / 180),2)+2*Math.Cos(alpha * Math.PI / 180)*Math.Cos(beta * Math.PI / 180)*Math.Cos(gamma * Math.PI / 180)))/Math.Sin(gamma * Math.PI / 180)}
                    });
                    //coordinate vector
                    Vector<double> frac = DenseVector.OfArray(new double[] { xfrac, yfrac, zfrac });

                    //cartesian coordinates of current atom
                    Vector<double> coord = A * frac;

                    //add new atom to molecule with coordinates above
                    Atoms.Add(new Atom(props[0], coord[0], coord[1], coord[2]));
                }
            }
        }

        /// <summary>
        /// Converts To Molecule
        /// </summary>
        /// <returns></returns>
        public Molecule ToMolecule() => new Molecule(Atoms);
    }
}

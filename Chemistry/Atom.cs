﻿using MathNet.Spatial.Euclidean;
using OxyPlot;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace PorphyStruct.Chemistry
{
    public class Atom
    {

        public string Identifier { get; set; }
        public bool IsMacrocycle { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Element Element { get; set; }

        public string Type { get => Element.Symbol; }

        /// <summary>
        /// Constructs a new Atom object
        /// </summary>
        /// <param name="identifier">The Crystal Identifier e.g. C19</param>
        /// <param name="x">X Coordinate in Angstrom</param>
        /// <param name="y">Y Coordinate in Angstrom</param>
        /// <param name="z">Z Coordinate in Angstrom</param>
        public Atom(string identifier, double x, double y, double z)
        {
            this.Identifier = identifier;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Element = SetElement();
        }

        /// <summary>
        /// XYZ Coordinates as array
        /// </summary>
        /// <returns>double[]</returns>
        public double[] XYZ()
        {
            return new double[] { X, Y, Z };
        }

        /// <summary>
        /// Get the Atoms distance to a given Plane
        /// </summary>
        /// <param name="plane">Plane Object (MathNet.Spatial.Euclidean)</param>
        /// <returns>double</returns>
        public double DistanceToPlane(Plane plane)
        {
            return plane.SignedDistanceTo(new Point3D(X, Y, Z));
        }

        /// <summary>
        /// Calculates Distance between to atoms
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static double Distance(Atom a1, Atom a2)
        {
            return MathNet.Numerics.Distance.Euclidean(a1.XYZ(), a2.XYZ());
        }

        /// <summary>
        /// get element by Identifier (C19 -> C)
        /// </summary>
        /// <returns>string</returns>
        public Element SetElement()
        {
            return Element.Create(Regex.Match(Identifier, @"([A-Z][a-z]*)").Value);
        }

        /// <summary>
        /// Indicates if this is a metal atom.
        /// </summary>
        /// <returns></returns>
        public bool IsMetal
        {
            get => Metals.Contains(Type) || Identifier == "M";
        }

        /// <summary>
        /// some common metals in coord. chem.
        /// may be completed soon
        /// </summary>
        public static List<string> Metals = new List<string>()
        {
            "Li",
            "Mg",
            "Sc", "Ti", "V", "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn",
            "Y", "Zr", "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd",
            "Hf", "Ta", "W", "Re", "Os", "Ir", "Pt", "Au", "Hg",
            "La", "U",
            "Ga", "Sn", "Sb"
        };

        /// <summary>
        /// get Color from  OxyAtomColor-Dictionary
        /// </summary>
        /// <returns>OxyPlot.OxyColor</returns>
        public OxyColor OxyColor
        {
            get => this.Element.OxyColor;
        }


        /// <summary>
        /// Radii of Atoms for 3D Visualisation
        /// </summary>
        public double AtomRadius { get => Element.Radius; }

        /// <summary>
        /// Colors of Atoms for 3D Visualisation
        /// </summary>
        public Brush Brush
        {
            get => this.Element.Brush;
        }

        //colors for bonds
        public static OxyColor[] modesMultiColor = new OxyColor[]
        {
                OxyColor.Parse(Properties.Settings.Default.color1),
                OxyColor.FromAColor(75, OxyColor.Parse(Properties.Settings.Default.color1)),
                OxyColor.FromAColor(50, OxyColor.Parse(Properties.Settings.Default.color1)),
                OxyColor.FromAColor(75, OxyColor.Parse(Properties.Settings.Default.color1)),
                OxyColor.FromAColor(75, OxyColor.Parse(Properties.Settings.Default.color1)),
        };
        public static OxyColor[] modesSingleColor = new OxyColor[]
        {
                OxyColor.Parse(Properties.Settings.Default.color1),
                OxyColor.Parse(Properties.Settings.Default.color2),
                OxyColor.Parse(Properties.Settings.Default.color3),
                OxyColor.Parse(Properties.Settings.Default.color4),
                OxyColor.Parse(Properties.Settings.Default.color5),
        };
    }
}

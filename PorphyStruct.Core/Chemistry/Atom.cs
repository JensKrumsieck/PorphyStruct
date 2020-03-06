using MathNet.Spatial.Euclidean;
using OxyPlot;
using PorphyStruct.Core.Util;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PorphyStruct.Chemistry
{
    public class Atom : Bindable, ICloneable
    {

        public string Identifier { get => Get<string>(); set => Set(value); }
        public bool IsMacrocycle { get => Get<bool>(); set => Set(value); }
        public double X { get => Get<double>(); set => Set(value); }
        public double Y { get => Get<double>(); set => Set(value); }
        public double Z { get => Get<double>(); set => Set(value); }
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
        /// Indicates a bond
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public bool BondTo(Atom test)
        {
            if (Atom.Distance(this, test) < (Element.Radius + test.Element.Radius) + 0.25)
                return true;
            return false;
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
            "Ga", "Sn", "Sb","Tl", "Pb"
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

        //colors for bonds
        public static OxyColor[] modesMultiColor = new OxyColor[]
        {
                OxyColor.Parse(Core.Properties.Settings.Default.color1),
                OxyColor.FromAColor(75, OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color1)),
                OxyColor.FromAColor(50, OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color1)),
                OxyColor.FromAColor(75, OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color1)),
                OxyColor.FromAColor(75, OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color1)),
        };
        public static OxyColor[] modesSingleColor = new OxyColor[]
        {
                OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color1),
                OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color2),
                OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color3),
                OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color4),
                OxyColor.Parse(PorphyStruct.Core.Properties.Settings.Default.color5),
        };

        /// <summary>
        /// clones an atom
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Atom(Identifier, X, Y, Z);

        }

        /// <summary>
        /// creates export Text for Atom
        /// </summary>
        public string ExportText
        {
            get => Identifier + "/" + Type + "\t" + X.ToString("N8", System.Globalization.CultureInfo.InvariantCulture) + "\t" + Y.ToString("N8", System.Globalization.CultureInfo.InvariantCulture) + "\t" + Z.ToString("N8", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// returns number of identifier
        /// </summary>
        public int ID
        {
            get
            {
                int.TryParse(Regex.Match(Identifier, @"\d+").Value, out int id);
                return id;
            }
        }

        /// <summary>
        /// return Identifiers Suffix
        /// </summary>
        public string Suffix
        {
            get => Regex.Match(Identifier, @"\D$").Value;
        }

        /// <summary>
        /// Prints Identifier
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Identifier;

    }

}

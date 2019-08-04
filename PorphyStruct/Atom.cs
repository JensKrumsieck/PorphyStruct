using MathNet.Spatial.Euclidean;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;
using OxyPlot;

namespace PorphyStruct
{
	public class Atom
    {

        public string Identifier { get; set; }
        public bool isMacrocycle { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; } 
		public string Type { get; set; }

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
			this.Type = getType();
        }

		/// <summary>
		/// XYZ Coordinates as array
		/// </summary>
		/// <returns>double[]</returns>
		public double[] getXYZ()
        {
            return new double[] { X, Y, Z };
        }

		/// <summary>
		/// Get the Atoms distance to a given Plane
		/// </summary>
		/// <param name="plane">Plane Object (MathNet.Spatial.Euclidean)</param>
		/// <returns>double</returns>
        public double getDistanceToPlane(Plane plane)
        {
            return plane.SignedDistanceTo(new Point3D(X, Y, Z));
        }

		/// <summary>
		/// get Atom Type by Identifier (C19 -> C)
		/// </summary>
		/// <returns>string</returns>
		public string getType()
		{
			return Regex.Match(Identifier, @"([A-Z][a-z]*)").Value;
		}

		/// <summary>
		/// get Color from  OxyAtomColor-Dictionary
		/// </summary>
		/// <returns>OxyPlot.OxyColor</returns>
		public OxyColor getOxyColor()
		{
			return (OxyAtomColor.ContainsKey(Type) ? OxyAtomColor[Type] : OxyAtomColor["C"]);
		}

		/// <summary>
		/// Colors for Atoms in Displacement View
		/// </summary>
		public static Dictionary<string, OxyColor> OxyAtomColor = new Dictionary<string, OxyColor>() {
				{ "C", OxyColors.Black },
				{ "N", OxyColors.Blue },
				{ "S", OxyColors.LightGoldenrodYellow },
				{ "O", OxyColors.Red }
		};
	
		/// <summary>
		/// Radii of Atoms for 3D Visualisation
		/// </summary>
		public static Dictionary<string, double> AtomRadius = new Dictionary<string, double>()
		{
			{ "Al", 1.43 },
			{ "Be", 1.12 },
			{ "B", .88 },
			{ "Br", 1.14 },
			{ "Cs", 2.62 },
			{ "Ca", 1.97 },
			{ "Cl", .99 },
			{ "F", .64 },
			{ "Ga", 1.22 },
			{ "Ge", 1.22 },
			{ "I", 1.33 },
			{ "K", 2.02 },
			{ "C", 0.77 },
			{ "Cu", 1.28 },
			{ "Li", 1.52 },
			{ "Mg", 1.6 },
			{ "Mn", 1.24 },
			{ "Na", 1.86 },
			{ "P", 1.1 },
			{ "Rb", 2.44 },
			{ "O", 0.66 },
			{ "S", 1.04 },
			{ "Se", 1.17 },
			{ "Si", 1.17 },
			{ "N", 0.7 },
			{ "Ag", 1.44 },
			{ "H", 0.38 },
			{ "Cr", 1.28 },
			{ "Co", 1.25 },
			{ "Fe", 1.24 },
			{ "Ni", 1.25 },
			{ "Sc", 1.62 },
			{ "Ti", 1.46 },
			{ "V", 1.34 },
			{ "Zn", 1.33 },
			{ "Cd", 1.48 }
		};

		/// <summary>
		/// Colors of Atoms for 3D Visualisation
		/// </summary>
		public static Dictionary<string, Brush> AtomColor = new Dictionary<string, Brush>()
		{
			{ "H", Brushes.WhiteSmoke },
			{ "C", Brushes.Black},
			{ "N", Brushes.Blue },
			{ "B", Brushes.LightCoral },
			{ "O", Brushes.Red },
			{ "F", Brushes.YellowGreen },
			{ "Li", Brushes.LightSalmon },
			{ "Mg", Brushes.LightSeaGreen },
			{ "Si", Brushes.Beige },
			{ "P", Brushes.Orange },
			{ "S", Brushes.OrangeRed },
			{ "Cl", Brushes.LightGreen },
			{ "Mn", Brushes.MediumPurple },
			{ "Cr", Brushes.BlueViolet },
			{ "Fe", Brushes.DarkOrange },
			{ "Co", Brushes.CornflowerBlue },
			{ "Ni", Brushes.Green },
			{ "Cu", Brushes.RosyBrown },
			{ "Zn", Brushes.DarkSlateBlue },
			{ "Se", Brushes.Orange },
			{ "Br", Brushes.SandyBrown },
			{ "I", Brushes.DarkViolet },
			{ "Mo", Brushes.MediumTurquoise },
			{ "Cd", Brushes.Gold }
		};

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

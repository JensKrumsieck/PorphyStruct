using OxyPlot;
using System;
using System.Linq;
using System.Windows.Media;

namespace PorphyStruct.Chemistry
{
    public class Element
    {
        /// <summary>
        /// element's atomic number
        /// </summary>
        public int AtomicNumber { get; set; }
        /// <summary>
        /// element's symbol
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// element's name in en
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// element's weight
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// element's color as hex string
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// element's covalent radius
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="AtomicNumber"></param>
        /// <param name="Symbol"></param>
        /// <param name="Name"></param>
        /// <param name="Weight"></param>
        /// <param name="Color"></param>
        /// <param name="Radius"></param>
        public Element(int AtomicNumber, string Symbol, string Name, double Weight, string Color, double Radius)
        {
            this.AtomicNumber = AtomicNumber;
            this.Symbol = Symbol;
            this.Name = Name;
            this.Weight = Weight;
            this.Color = Color;
            this.Radius = Radius;
        }

        /// <summary>
        /// creates Element from File
        /// </summary>
        /// <returns></returns>
        public static Element Create(string Symbol)
        {
            string[] element = null;
            string[] lines = Properties.Resources.Elements.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.Split(';').Contains(Symbol))
                {
                    element = line.Split(';');
                }
            }
            return new Element(Convert.ToInt32(element[0]), element[1], element[2], Convert.ToDouble(element[3]), "#" + element[4], Convert.ToDouble(element[5]));

        }

        /// <summary>
        /// get Color for OxyPlot
        /// </summary>
        public OxyColor OxyColor { get => OxyColor.Parse(this.Color); }

        /// <summary>
        /// get Brush for Helix
        /// </summary>
        public Brush Brush { get => (Brush)new BrushConverter().ConvertFromString(this.Color); }
    }
}

using System.Collections.Generic;
using System.Linq;
using ChemSharp.Mathematics;
using ChemSharp.Molecules;

namespace PorphyStruct.Core.Analysis.Properties
{
    public class Angle
    {
        public double Value { get; set; }
        public Atom Atom1 { get; set; }
        public Atom Atom2 { get; set; }
        public Atom Atom3 { get; set; }

        public Angle(string a1, string a2, string a3, IList<Atom> atoms) : this(new List<Atom>
        {
            atoms.FirstOrDefault(s => s.Title == a1),
            atoms.FirstOrDefault(s => s.Title == a2),
            atoms.FirstOrDefault(s => s.Title == a3)
        })
        { }

        public Angle(IList<Atom> atoms)
        {
            if (atoms.Count != 3 || atoms.Any(s => s == null)) return;
            Atom1 = atoms[0];
            Atom2 = atoms[1];
            Atom3 = atoms[2];
            var vectors = atoms.Select(s => s.Location).ToList();
            Value = MathV.Angle(vectors[0], vectors[1], vectors[2]);
        }

        public override string ToString() => ToString("{0:N}");

        public string ToString(string format) =>
            $"{Atom1.Title} - {Atom2.Title} - {Atom3.Title} : {string.Format(format, Value)}";
    }
}

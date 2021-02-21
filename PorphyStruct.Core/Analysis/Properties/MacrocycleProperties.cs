using ChemSharp.Molecules.Mathematics;
using PorphyStruct.Core.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PorphyStruct.Core.Analysis.Properties
{
    public class MacrocycleProperties
    {
        [JsonIgnore]
        public MacrocycleAnalysis Analysis;

        public List<Dihedral> Dihedrals { get; } = new List<Dihedral>();
        public List<Angle> Angles { get; } = new List<Angle>();
        public List<Distance> Distances { get; } = new List<Distance>();

        public Simulation Simulation { get; set; }

        public KeyValueProperty InterplanarAngle { get; set; } = new KeyValueProperty { Unit = "°" };

        public KeyValueProperty OutOfPlaneParameter { get; set; } = new KeyValueProperty { Key = "Doop (exp.)", Unit = "Å" };

        public MacrocycleProperties(MacrocycleAnalysis analysis)
        {
            Analysis = analysis;
        }

        /// <summary>
        /// Dihedrals for Corrole, Porphyrin and Norcorrole
        /// </summary>
        internal static IList<string[]> PorphyrinoidDihedrals = new List<string[]>{
            new[] { "C3", "C4", "C6", "C7" }, //chi1
            new[] { "C2", "C1", "C19", "C18" }, //chi2
            new[] { "C13", "C14", "C16", "C17" }, //chi3 
            new[] { "C8", "C9", "C11", "C12" }, //chi4
            new[] { "C4", "N1", "N3", "C11" }, //psi1
            new[] { "C9", "N2", "N4", "C16" }, //psi2
            new[] { "N1", "C4", "C6", "N2" }, //phi1
            new[] { "N1", "C1", "C19", "N4" }, //phi2
            new[] { "N3", "C14", "C16", "N4" }, //phi3
            new[] { "N2", "C9", "C11", "N3" } //phi4
        };

        /// <summary>
        /// Fills all Lists
        /// </summary>
        public void Rebuild()
        {
            Simulation ??= new Simulation(Analysis.GetAnalysisType());
            Angles.Clear();
            Distances.Clear();

            if (Analysis.DataPoints.Any()) Simulation?.Simulate(Analysis.DataPoints.OrderBy(s => s.X).Select(s => s.Y).ToArray());
            OutOfPlaneParameter.Value = Analysis.DataPoints.OrderBy(s => s.X).DisplacementValue();
            RebuildDihedrals();

            if (Analysis.Metal == null) return;
            Angles.Add(new Angle(Analysis.FindAtomByTitle("N1"), Analysis.Metal, Analysis.FindAtomByTitle("N4")));
            Angles.Add(new Angle(Analysis.FindAtomByTitle("N2"), Analysis.Metal, Analysis.FindAtomByTitle("N3")));
            InterplanarAngle.Key = $"[N1-{Analysis.Metal.Title}-N4]x[N2-{Analysis.Metal.Title}-N3]";
            InterplanarAngle.Value = Angles[0].PlaneAngle(Angles[1]);
            Distances.AddRange(Analysis.Atoms.Where(s => Analysis.Metal.BondToByCovalentRadii(s))
                .Select(s => new Distance(Analysis.Metal, s)));
        }

        /// <summary>
        /// Rebuilds Dihedrals
        /// </summary>
        private void RebuildDihedrals()
        {
            Dihedrals.Clear();

            Dihedrals.Add(new Dihedral(Analysis.FindAtomByTitle("N1"), Analysis.FindAtomByTitle("N2"), Analysis.FindAtomByTitle("N3"), Analysis.FindAtomByTitle("N4")));
            Distances.Add(new Distance(Analysis.FindAtomByTitle("N1"), Analysis.FindAtomByTitle("N3")));
            Distances.Add(new Distance(Analysis.FindAtomByTitle("N2"), Analysis.FindAtomByTitle("N4")));

            //add dihedrals from string list
            if (Analysis is PorphyrinAnalysis || Analysis is CorroleAnalysis)
            {
                Dihedrals.AddRange(PorphyrinoidDihedrals.Select(s => new Dihedral(Analysis.FindAtomByTitle(s[0]),
                    Analysis.FindAtomByTitle(s[1]), Analysis.FindAtomByTitle(s[2]), Analysis.FindAtomByTitle(s[3]))));
            }
        }

        /// <summary>
        /// Exports Properties in Machine readable format
        /// </summary>
        /// <returns></returns>
        public string ExportJson() => JsonSerializer.Serialize(this);

        /// <summary>
        /// Exports Properties in Human readable format
        /// </summary>
        /// <returns></returns>
        public string ExportString()
        {
            var result = "";
            result += ExportBlock("Simulation", Simulation.SimulationResult.Append(OutOfPlaneParameter).Append(Simulation.OutOfPlaneParameter));
            result += ExportBlock("Distances", Distances);
            result += Analysis.Metal != null ? ExportBlock("Angles", Angles.Append(InterplanarAngle)) : ExportBlock("Angles", Angles);
            result += ExportBlock("Dihedrals", Dihedrals);
            return result;
        }


        private static string ExportBlock(string title, IEnumerable<KeyValueProperty> content)
        {
            var result = $"{title}\n";
            result = content.Aggregate(result, (current, prop) => current + (prop + "\n"));
            result += "\n";
            return result;
        }
    }
}

#nullable enable
using ChemSharp.Extensions;
using ChemSharp.Mathematics;
using ChemSharp.Molecules;
using ChemSharp.Molecules.Extensions;
using PorphyStruct.Core.Analysis.Properties;
using PorphyStruct.Core.Extension;
using PorphyStruct.Core.Plot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace PorphyStruct.Core.Analysis
{
    public abstract class MacrocycleAnalysis
    {
        /// <summary>
        /// Selected Atoms for Analysis
        /// </summary>
        public List<Atom> Atoms { get; set; }

        /// <summary>
        /// Selected Bonds for Analysis
        /// </summary>
        public IEnumerable<Bond> Bonds { get; set; }

        private readonly Guid _guid;

        public Atom? Metal { get; set; }

        /// <summary>
        /// Color Representation for Analysis to use as indicator
        /// </summary>
        public string AnalysisColor => _guid.HexStringFromGuid();

        protected MacrocycleAnalysis(List<Atom> atoms, IEnumerable<Bond> bonds)
        {
            Atoms = atoms;
            Bonds = bonds;
            _guid = Guid.NewGuid();
            Task.Run(() =>
                {
                    NameAtoms();
                    Properties = new MacrocycleProperties(this);
                }
            ).Wait(15000);
        }

        /// <summary>
        /// RingAtoms by Identifier
        /// </summary>
        public abstract List<string> RingAtoms { get; }

        /// <summary>
        /// Alpha atoms of Macrocycle by Identifiers
        /// </summary>
        public abstract string[] AlphaAtoms { get; }

        /// <summary>
        /// Multipliers for C-Atom positioning
        /// </summary>
        public abstract Dictionary<string, double> Multiplier { get; }

        private MacrocycleProperties _properties = null!;
        /// <summary>
        /// A Set of Macrocyclic Properties
        /// </summary>
        public MacrocycleProperties Properties
        {
            get
            {
                _properties?.Rebuild();
                return _properties!;
            }
            set => _properties = value;
        }

        private IEnumerable<AtomDataPoint>? _dataPoints;

        /// <summary>
        /// Cached Datapoints
        /// </summary>
        public IEnumerable<AtomDataPoint> DataPoints
        {
            get => _dataPoints ??= CalculateDataPoints();
            set => _dataPoints = value;
        }

        /// <summary>
        /// Returns Mean Square Plane of Analysis Fragment
        /// </summary>
        public Plane MeanPlane => MathV.MeanPlane(Atoms.Select(s => s.Location).ToList());

        /// <summary>
        /// Returns Neighbors in context of analysis
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public IEnumerable<Atom> Neighbors(Atom a) => AtomUtil.Neighbors(a, Bonds);

        /// <summary>
        /// Generates DataPoints
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<AtomDataPoint> CalculateDataPoints()
        {
            Atoms = Atoms.OrderBy(s => RingAtoms.IndexOf(s.Title)).ToList();
            var dist = 0d;
            var fix = 1d;

            foreach (var a in Atoms)
            {
                var coordX = 1d;
                if (a.Title.Contains("C")) coordX = fix + dist * Multiplier[a.Title];
                if (a.Title.Contains("N")) coordX = fix + dist / 2d;
                //starts with C1 which is alpha per definition, so refresh distance every alpha atom.
                if (IsAlpha(a) && NextAlpha(a) != null) dist = a.DistanceTo(NextAlpha(a));
                //alpha atoms are fixpoints
                if (IsAlpha(a)) fix = coordX;
                yield return new AtomDataPoint(coordX, MathV.Distance(MeanPlane, a.Location), a);
            }
        }

        /// <summary>
        /// checks whether it's an alpha atom or not
        /// </summary>
        /// <param name="a"></param>
        /// <returns>boolean</returns>
        private bool IsAlpha(Atom a) => DFSUtil.VertexDegree(a, Neighbors) == 3;

        /// <summary>
        /// gets next alpha position for distance measuring
        /// </summary>
        /// <param name="a"></param>
        /// <returns>Atom</returns>
        private Atom? NextAlpha(Atom a)
        {
            var i = Array.IndexOf(AlphaAtoms, a.Title) + 1;
            return AlphaAtoms.Length > i ? FindAtomByTitle(AlphaAtoms[i]) : null;
        }

        /// <summary>
        /// An overrideable Method to get C1 Atom. 
        /// In a Porphyrin it does not care which alpha atom is C1, so return any of them...
        /// override this methode in any other class
        /// </summary>
        /// <returns></returns>
        public virtual Atom C1 => Atoms.First(s => DFSUtil.VertexDegree(s, Neighbors) == 3);

        /// <summary>
        /// Lists N4 Cavity
        /// </summary>
        public List<Atom> N4Cavity =>
            Atoms.Where(s => DFSUtil.VertexDegree(s, a => Neighbors(a).Where(IsAlpha).Where(n => DFSUtil.BackTrack(n, a, Neighbors, 5).Count == 5)) == 2).ToList();

        /// <summary>
        /// Lists all Beta Atoms
        /// </summary>
        public List<Atom> Beta => Atoms.Where(s =>
            DFSUtil.VertexDegree(s, Neighbors) == 2 && Neighbors(s).Count(IsAlpha) == 1).ToList();

        /// <summary>
        /// Lists all alpha Atoms
        /// </summary>
        public List<Atom> Alpha => Atoms.Where(IsAlpha).ToList();

        /// <summary>
        /// Lists all meso Atoms
        /// </summary>
        public List<Atom> Meso => Atoms.Except(Alpha).Except(Beta).Except(N4Cavity).ToList();

        /// <summary>
        /// calculate distance between two atoms (as identifiers are needed this must be in Macrocycle-Class!!)
        /// </summary>
        /// <param name="id1">Identifier 1</param>
        /// <param name="id2">Identifier 2</param>
        /// <returns>The Vectordistance</returns>
        public double CalculateDistance(string id1, string id2) => MathV.Distance(FindAtomByTitle(id1)!.Location, FindAtomByTitle(id2)!.Location);

        /// <summary>
        /// Gets DataPoints for Bonds
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<(AtomDataPoint a1, AtomDataPoint a2)> BondDataPoints() =>
            from bond in Bonds.Where(s => !s.Atoms.Select(a => a.Title).ScrambledEquals(new[] { RingAtoms.First(), RingAtoms.Last() })) //Do not draw bond between first and last element
            let start = DataPoints.FirstOrDefault(s => s.Atom.Equals(bond.Atom1))
            let end = DataPoints.FirstOrDefault(s => s.Atom.Equals(bond.Atom2))
            select (start, end);

        /// <summary>
        /// Creates Analysis Type
        /// </summary>
        /// <param name="atoms"></param>
        /// <param name="bonds"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MacrocycleAnalysis Create(List<Atom> atoms, IEnumerable<Bond> bonds, MacrocycleType type) =>
            type switch
            {
                MacrocycleType.Porphyrin => new PorphyrinAnalysis(atoms, bonds),
                MacrocycleType.Corrole => new CorroleAnalysis(atoms, bonds),
                MacrocycleType.Norcorrole => new NorcorroleAnalysis(atoms, bonds),
                MacrocycleType.Porphycene => new PorphyceneAnalysis(atoms, bonds),
                MacrocycleType.Corrphycene => new CorrphyceneAnalysis(atoms, bonds),
                _ => throw new InvalidOperationException()
            };

        /// <summary>
        /// Assign the correct Identifiers for a cycle CN-corpus
        /// </summary>
        public void NameAtoms()
        {
            //cycle valid?
            if (Atoms.Count != RingAtoms.Count) return;
            //track visited atoms
            var visited = new HashSet<Atom>();
            //set Identifier for C1 Atom
            C1.Title = "C1";
            //get N4 Cavity, necessary for performance
            var cavity = N4Cavity;
            //force c2 to be first step
            var current = Neighbors(C1).First(s => !cavity.Contains(s) && Beta.Contains(s));
            current.Title = "C2";
            //add C1&C2 to visited
            visited.UnionWith(new HashSet<Atom>() { current, C1 });
            //get carbon atoms
            var carbons = RingAtoms.Where(s => s.Contains("C")).OrderBy(s => int.Parse(s.Replace("C", ""))).ToList();
            var i = 2;
            //loop through atoms and name them
            while (visited.Count != carbons.Count)
            {
                foreach (var neighbor in Neighbors(current).Where(s => !visited.Contains(s) && !cavity.Contains(s)))
                {
                    //add to visited and assign Identifier
                    neighbor.Title = carbons[i];
                    visited.Add(current = neighbor);
                    i++;
                }
            }

            //set up identifiers for nitrogens
            for (var j = 1; j <= 4; j++)
            {
                var alpha = FindAtomByTitle(AlphaAtoms[2 * j - 1]);
                var nitrogen = cavity.First(s => Neighbors(s).Contains(alpha));
                nitrogen.Title = "N" + j;
            }
        }

        /// <summary>
        /// Finds atom by Title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public Atom? FindAtomByTitle(string title) => Atoms.FirstOrDefault(s => s.Title == title) ?? null;
    }
}

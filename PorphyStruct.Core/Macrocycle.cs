using ChemSharp.Extensions;
using ChemSharp.Molecules;
using ChemSharp.Molecules.DataProviders;
using ChemSharp.Molecules.Mathematics;
using PorphyStruct.Core.Analysis;
using PorphyStruct.Core.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PorphyStruct.Core
{
    public class Macrocycle : Molecule
    {
        /// <summary>
        /// Correlates type to size of ring fragment
        /// </summary>
        public static Dictionary<MacrocycleType, int> RingSize = new Dictionary<MacrocycleType, int>
        {
            {MacrocycleType.Porphyrin, 24},
            {MacrocycleType.Corrole, 23},
            {MacrocycleType.Norcorrole, 22},
            {MacrocycleType.Porphycene, 24},
            {MacrocycleType.Corrphycene, 24}
        };

        public Macrocycle(string path) : base(MoleculeFactory.CreateProvider(path)) { }

        public Macrocycle(IAtomDataProvider provider) : base(provider) { }

        /// <summary>
        /// Gets or Sets the Macrocycle Type
        /// </summary>
        public MacrocycleType MacrocycleType { get; set; } = MacrocycleType.Porphyrin;

        /// <summary>
        /// Contains detected fragment data
        /// </summary>
        public IList<MacrocycleAnalysis> DetectedParts = new List<MacrocycleAnalysis>();

        /// <summary>
        /// Runs a Graph-Theory based Detection algorithm to find Macrocycles in Molecule
        /// </summary>
        /// <returns></returns>
        public async Task Detect()
        {
            DetectedParts.Clear();
            var parts = await GetParts();
            var distinct = parts.Distinct(new EnumerableEqualityComparer<Atom>());
            foreach (var part in distinct)
            {
                var p = part.ToHashSet();
                var data = FindCorpus(ref p);
                var bonds = Bonds.Where(b => data.Count(a => b.Atoms.Contains(a)) == 2);
                var unique = true;
                foreach (var current in DetectedParts)
                    if (current.Atoms.ScrambledEquals(data)) unique = false;

                if (data.Count != RingSize[MacrocycleType] || !unique) continue;
                var analysis = MacrocycleAnalysis.Create(data.ToList(), bonds, MacrocycleType);
                var metal = Neighbors(analysis.N4Cavity[0]).FirstOrDefault(s => !s.IsNonCoordinative());
                if (metal != null) analysis.Metal = metal;
                DetectedParts.Add(analysis);
            }
        }

        /// <summary>
        /// Returns connected figures
        /// </summary>
        /// <returns></returns>
        private async Task<IList<IEnumerable<Atom>>> GetParts()
        {
            var parts = new List<IEnumerable<Atom>>();
            await foreach (var fig in DFSUtil.ConnectedFigures(
                Atoms.Where(s => Neighbors(s).Count() >= 2), NonMetalNonDeadEndNeighbors))
            {
                var connectedAtoms = fig.Distinct().ToArray();
                connectedAtoms = connectedAtoms.Where(s => s.IsNonCoordinative() && s.Symbol != "H").ToArray();
                if (connectedAtoms.Length >= RingSize[MacrocycleType]) parts.Add(connectedAtoms);
            }
            return parts;
        }

        /// <summary>
        /// Finds Macrocycles in part
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private HashSet<Atom> FindCorpus(ref HashSet<Atom> part)
        {
            HashSet<Atom> corpus;
            foreach (var atom in part.Where(s => s.IsNonCoordinative()))
            {
                corpus = RingPath(atom, RingSize[MacrocycleType] - 8);
                foreach (var a in corpus.SelectMany(NonMetalNonDeadEndNeighbors))
                {
                    var outer = RingPath(a, RingSize[MacrocycleType] - 4);
                    outer.UnionWith(corpus);
                    if (outer.Count == RingSize[MacrocycleType]) return outer;
                }
            }
            corpus = FindCorpusFallBack(ref part);
            return corpus;
        }

        /// <summary>
        /// Fallback Method for Detection e.g. when bonds are not set correctly
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private HashSet<Atom> FindCorpusFallBack(ref HashSet<Atom> part)
        {
            var corpus = new HashSet<Atom>();
            foreach (var atom in part.Where(s => s?.IsNonCoordinative() ?? false))
            {
                var p = part;
                IEnumerable<Atom> Func(Atom s) => p?.Where(a => a.BondToByCovalentRadii(s) && a.IsNonCoordinative());
                corpus = FuncRingPath(atom, RingSize[MacrocycleType] - 8, Func);
                foreach (var n in corpus.SelectMany(Func))
                {
                    var outer = FuncRingPath(n, RingSize[MacrocycleType] - 4, Func);
                    outer.UnionWith(corpus);
                    if (outer.Count == RingSize[MacrocycleType]) return outer;
                }
            }
            return corpus;
        }

        private HashSet<Atom> RingPath(Atom atom, int size) => FuncRingPath(atom, size, NonMetalNonDeadEndNeighbors);
        private static HashSet<Atom> FuncRingPath(Atom atom, int size, Func<Atom, IEnumerable<Atom>> func)
        {
            var goal = func(atom).FirstOrDefault();
            return goal == null ? new HashSet<Atom>() : DFSUtil.BackTrack(atom, goal, func, size);
        }
        private IEnumerable<Atom> NonMetalNonDeadEndNeighbors(Atom atom) => NonMetalNeighbors(atom)?.Where(a => !Constants.DeadEnds.Contains(a.Symbol) && a.IsNonCoordinative());
    }
}

using ChemSharp.Extensions;
using ChemSharp.Molecules;
using ChemSharp.Molecules.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PorphyStruct.Analysis;

namespace PorphyStruct
{
    public class Macrocycle : Molecule
    {
        public Macrocycle(string path) : base(MoleculeFactory.CreateProvider(path))
        { }

        public IList<MacrocycleAnalysis> DetectedParts = new List<MacrocycleAnalysis>();

        /// <summary>
        /// Runs a Graph-Theory based Detection algorithm to find Macrocycles in Molecule
        /// </summary>
        /// <returns></returns>
        public async Task Detect()
        {
            DetectedParts.Clear();
            var parts = await GetParts();
            foreach (var part in parts.Distinct())
            {
                var p = part.ToHashSet();
                var data = FindCorpus(ref p);
                var bonds = Bonds.Where(b => data.Count(a => b.Atoms.Contains(a)) == 2);
                if (data.Count == 24) DetectedParts.Add(MacrocycleAnalysis.Create(data.ToList(), bonds, MacrocycleType.Porphyrin));
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
                var connectedAtoms = fig as Atom[] ?? fig.ToArray();
                if (connectedAtoms.Length >= 24) parts.Add(connectedAtoms); //TODO: Types!
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
            var corpus = new HashSet<Atom>();
            foreach (var atom in part.Where(s => !s.IsMetal))
            {
                corpus = RingPath(atom, 24 - 8); //TODO: Types
                foreach (var a in corpus.SelectMany(NonMetalNonDeadEndNeighbors))
                {
                    var outer = RingPath(a, 24 - 4);
                    outer.UnionWith(corpus);
                    if (outer.Count == 24) return outer;
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
        private static HashSet<Atom> FindCorpusFallBack(ref HashSet<Atom> part)
        {
            var corpus = new HashSet<Atom>();
            foreach (var atom in part.Where(s => !s?.IsMetal ?? false))
            {
                var p = part;
                IEnumerable<Atom> Func(Atom s) => p?.Where(a => a.BondToByCovalentRadii(s) && !a.IsMetal);
                corpus = FuncRingPath(atom, 24 - 8, Func);
                foreach (var n in corpus.SelectMany((Func<Atom, IEnumerable<Atom>>)Func))
                {
                    var outer = FuncRingPath(n, 24 - 4, Func);
                    outer.UnionWith(corpus);
                    if (outer.Count == 24) return outer;
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
        private IEnumerable<Atom> NonMetalNonDeadEndNeighbors(Atom atom) => NonMetalNeighbors(atom)?.Where(a => !Constants.DeadEnds.Contains(a.Symbol));
    }
}

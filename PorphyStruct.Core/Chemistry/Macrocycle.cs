using MathNet.Spatial.Euclidean;
using OxyPlot;
using OxyPlot.Annotations;
using PorphyStruct.Chemistry.Data;
using PorphyStruct.Chemistry.Properties;
using PorphyStruct.Core.Util;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PorphyStruct.Chemistry
{
    public abstract class Macrocycle : Molecule, ICloneable
    {
        public Macrocycle(AsyncObservableCollection<Atom> Atoms) : base(Atoms)
        {
            if (!IsValid) SetIsMacrocycle(type);

            if (HasMetal(false))
            {
                PropertyProviders.Add(new MetalAngles(ByIdentifier, Metal));
                PropertyProviders.Add(new MetalDistances(Metal, Neighbors(Metal).ToList()));
                PropertyProviders.Add(new PlaneDistances(Atoms));
                PropertyProviders.Add(new InterplanarAngle(ByIdentifier, Metal));
            }
            PropertyProviders.Add(new DefaultDihedrals(ByIdentifier));
            PropertyProviders.Add(new DefaultDistances(ByIdentifier));
        }

        /// <summary>
        /// PropertyProviders
        /// </summary>
        public List<IPropertyProvider> PropertyProviders { get; set; } = new List<IPropertyProvider>();

        /// <summary>
        /// The Datapoint providers
        /// </summary>
        public ObservableCollection<IAtomDataPointProvider> DataProviders { get; set; } = new ObservableCollection<IAtomDataPointProvider>();

        /// <summary>
        /// Bonds of Macrocycle by Identifiers
        /// </summary>
        public abstract List<Tuple<string, string>> Bonds { get; }
        /// <summary>
        /// Ringatoms of Macrocycle by Identifiers
        /// </summary>
        public abstract List<string> RingAtoms { get; }
        /// <summary>
        /// Alphaatoms of Macrocycle by Identifiers
        /// </summary>
        public abstract string[] AlphaAtoms { get; }

        public List<AtomDataPoint> DataPoints => DataProviders.Where(s => s.DataType == DataType.Experimental).FirstOrDefault()?.DataPoints.ToList();

        /// <summary>
        /// Multipliers for C-Atom positioning
        /// </summary>
        public abstract Dictionary<string, double> Multiplier { get; }

        public enum Type { Corrole, Porphyrin, Norcorrole, Corrphycene, Porphycene };
        public virtual Type type { get; }

        /// <summary>
        /// Returns Cyclic Properties
        /// </summary>
        public IEnumerable<KeyValuePair<string, IEnumerable<Property>>> Properties
        {
            get
            {
                foreach (IPropertyProvider propertyProvider in PropertyProviders)
                    yield return new KeyValuePair<string, IEnumerable<Property>>(propertyProvider.Type.ToString(), propertyProvider.CalculateProperties());
            }
        }

        /// <summary>
        /// Paints all Data
        /// </summary>
        /// <param name="Model"></param>
        public void Paint(PlotModel Model)
        {
            DataProviders.Sort(s => s.Priority);
            foreach (IAtomDataPointProvider dataProvider in DataProviders)
                MacrocyclePainter.Paint(this, Model, dataProvider);
        }

        /// <summary>
        /// Normalizes all Data
        /// </summary>
        public void Normalize(bool normalize = true)
        {
            foreach (IAtomDataPointProvider dataProvider in DataProviders)
            {
                if (normalize && !dataProvider.Normalized || !normalize && dataProvider.Normalized)
                    dataProvider.Normalize();
            }
        }

        /// <summary>
        /// Inverts all Data
        /// </summary>
        public void Invert(bool invert = true)
        {
            foreach (IAtomDataPointProvider dataProvider in DataProviders)
            {
                if (invert && !dataProvider.Inverted || !invert && dataProvider.Inverted)
                    dataProvider.Invert();
            }
        }

        /// <summary>
        /// Gets the centroid of this Macrocycle
        /// </summary>
        /// <returns>Centroid as Vector3D</returns>
        public Vector3D GetCentroid() => GetCentroid(Atoms.Where(s => s.IsMacrocycle && !s.IsMetal));

        /// <summary>
        /// Gets mean plane of the macrocyle
        /// </summary>
        /// <returns></returns>
        public Plane GetMeanPlane() => GetMeanPlane(Atoms.Where(s => s.IsMacrocycle && !s.IsMetal));

        /// <summary>
        /// calculate distance between two atoms (as identifiers are needed this must be in Macrocycle-Class!!)
        /// </summary>
        /// <param name="id1">Identifier 1</param>
        /// <param name="id2">Identifier 2</param>
        /// <returns>The Vectordistance</returns>
        public double CalculateDistance(string id1, string id2) => Atom.Distance(ByIdentifier(id1, true), ByIdentifier(id2, true));

        /// <summary>
        /// checks whether it's an alpha atom or not
        /// </summary>
        /// <param name="a"></param>
        /// <returns>boolean</returns>
        private bool IsAlpha(Atom a) => AlphaAtoms.Contains(a.Identifier) ? true : false;

        /// <summary>
        /// gets next alpha position for distance measuring
        /// </summary>
        /// <param name="a"></param>
        /// <returns>Atom</returns>
        private Atom GetNextAlpha(Atom a)
        {
            int i = Array.IndexOf(AlphaAtoms, a.Identifier) + 1;
            if (AlphaAtoms.Length > i) return ByIdentifier(AlphaAtoms[i], true);
            else return null;
        }

        /// <summary>
        /// Validates Configuration
        /// </summary>
        private bool Validate
        {
            get
            {
                if (Atoms.Where(a => a.IsMacrocycle).Count() != RingAtoms.Count) return false; //count mismatch
                foreach (string id in RingAtoms)
                {
                    if (Atoms.Where(a => a.IsMacrocycle && a.Identifier == id).Count() != 1)
                        return false; //identifier missing
                }
                foreach (Tuple<string, string> b in Bonds)
                {
                    if (!IsValidBond(ByIdentifier(b.Item1, true), ByIdentifier(b.Item2, true)) || !ByIdentifier(b.Item1, true).BondTo(ByIdentifier(b.Item2, true)))
                        return false; //Bond is missing
                }
                return true;
            }
        }
        /// <summary>
        /// public validation boolean
        /// </summary>
        public bool IsValid
        {
            get
            {
                Set(Validate);
                return Get<bool>();
            }
        }

        /// <summary>
        /// Method for Datapoint calculation //rewritten
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<AtomDataPoint> CalculateDataPoints()
        {
            //reorder Atoms
            Atoms = new AsyncObservableCollection<Atom>(Atoms.OrderBy(s => RingAtoms.IndexOf(s.Identifier)));

            //current alpha-alpha distance
            double distance = 0;
            //current fixpoint
            double fixPoint = 1;

            //loop through every non Metal Atom of Macrocycle and return datapoint
            foreach (Atom a in Atoms.Where(s => s.IsMacrocycle && !s.IsMetal))
            {
                double xCoord = 1;
                if (a.Identifier.Contains("C")) xCoord = fixPoint + (distance * Multiplier[a.Identifier]);
                if (a.Identifier.Contains("N")) xCoord = fixPoint + distance / 2;

                //starts with C1 which is alpha per definition, so refresh distance every alpha atom.
                if (IsAlpha(a) && GetNextAlpha(a) != null) distance = Atom.Distance(a, GetNextAlpha(a));

                //alpha atoms are fixpoints
                if (IsAlpha(a)) fixPoint = xCoord;

                yield return new AtomDataPoint(xCoord, a.DistanceToPlane(GetMeanPlane()), a);
            }
        }
        /// <summary>
        /// Return the cycle's datapoints
        /// </summary>
        /// <returns>AtomDataPoints</returns>
        public void GetDataPoints()
        {
            //delete provider
            DataProviders.RemoveAll(s => s.DataType == DataType.Experimental);
            PropertyProviders.RemoveAll(s => s.GetType() == typeof(ExperimentalData));

            var data = CalculateDataPoints().ToList();

            if (HasMetal()) data.Add(new AtomDataPoint(
                    (data.Max(s => s.X) + data.Min(s => s.X)) / 2,
                    Metal.DistanceToPlane(GetMeanPlane()),
                    Metal));

            var provider = new ExperimentalData(data);
            DataProviders.Add(provider);
            PropertyProviders.Add(provider);
        }


        /// <summary>
        /// Indicates if a Metal Atom is present
        /// </summary>
        public bool HasMetal(bool isMacrocycle = true) => Atoms.Where(s => (isMacrocycle ? s.IsMacrocycle == isMacrocycle : true) && s.IsMetal).Count() > 0;


        /// <summary>
        /// Draws a line between two points
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="type"></param>
        /// <returns>ArrowAnnotation aka Bond</returns>
        public ArrowAnnotation DrawBond(AtomDataPoint a1, AtomDataPoint a2, IAtomDataPointProvider data) => new ArrowAnnotation
        {
            StartPoint = a1.GetDataPoint(),
            EndPoint = a2.GetDataPoint(),
            HeadWidth = 0,
            HeadLength = 0,
            Color = Core.Properties.Settings.Default.singleColor ? MacrocyclePainter.SingleColor(DataProviders.IndexOf(data), DataProviders.Count) : Atom.modesMultiColor[(int)data.DataType],
            Layer = AnnotationLayer.BelowSeries,
            StrokeThickness = Core.Properties.Settings.Default.lineThickness,
            Tag = a1.atom.Identifier + "," + a2.atom.Identifier
        };

        /// <summary>
        /// Generates all Bonds as Annotations
        /// </summary
        /// <returns>Annotation aka Bonds</returns>
        public virtual IEnumerable<ArrowAnnotation> DrawBonds(IAtomDataPointProvider data)
        {
            data.DataPoints = data.DataPoints.OrderBy(s => s.X).ToList();

            foreach (Tuple<string, string> t in Bonds)
                yield return DrawBond(
                    data.DataPoints.Where(s => s.atom.Identifier == t.Item1 && s.atom.IsMacrocycle).First(),
                    data.DataPoints.Where(s => s.atom.Identifier == t.Item2 && s.atom.IsMacrocycle).First(),
                    data);

            //add metal atoms
            if (HasMetal())
                foreach (AtomDataPoint n in data.DataPoints.Where(s => s.atom.Type == "N").ToList())
                {
                    ArrowAnnotation b = DrawBond(data.DataPoints.Where(s => s.atom.IsMetal).FirstOrDefault(), n, data);
                    b.LineStyle = LineStyle.Dash;
                    b.Color = OxyColor.FromAColor(75, b.Color);
                    b.Tag = "Metal";
                    yield return b;
                }
        }

        /// <summary>
        /// Get Atom by Identifier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="forceMacroCycle"></param>
        /// <returns>Atom</returns>
        public Atom ByIdentifier(string id, bool forceMacroCycle) => Atoms.Where(s => s.Identifier == id && (forceMacroCycle ? s.IsMacrocycle == forceMacroCycle : true)).FirstOrDefault();

        /// <summary>
        /// Get Atom by Identifier
        /// without IsMacrocycle needed to be true
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Atom ByIdentifier(string id) => ByIdentifier(id, false);

        /// <summary>
        /// Gets the absolute mean displacement over all atoms
        /// </summary>
        /// <returns></returns>
        public double? MeanDisplacement() => DataPoints?.MeanDisplacement();

        /// <summary>
        /// Gets the 4 atoms of cavity by centroid method
        /// </summary>
        /// <param name="mol"></param>
        /// <returns></returns>
        internal List<Atom> N4Cavity(IEnumerable<Atom> mol)
        {
            //calculate centroid
            Vector3D centroid = GetCentroid(mol);
            //inject centroid into atoms as Lanthanium atom (because of big radius)
            //basically need a sphere where to catch atoms in to get N4 Cavity
            Atoms.Add(new Atom("La Centroid", centroid.X, centroid.Y, centroid.Z));
            //find neighbors which have more neighbors that are non metal nor hydrogen (NH) and belong to current figure
            IEnumerable<Atom> neighbors = NonMetalNeighbors(ByIdentifier("La Centroid"), mol).Where(s => NonMetalNeighbors(s, mol).Where(n => n.Type != "H").Count() == 2 && mol.Contains(s));

            //use the nearest 4 neighbors by mean plane and centroid distance to get cavity atoms
            var cavity = neighbors.OrderBy(n => Atom.Distance(ByIdentifier("La Centroid"), n) + Math.Abs(n.DistanceToPlane(GetMeanPlane(mol)))).Take(4).ToList();

            //remove centroid
            Atoms.Remove(ByIdentifier("La Centroid"));

            return cavity;
        }

        /// <summary>
        /// Fallback if cavity detection does not work
        /// </summary>
        /// <param name="mol"></param>
        /// <returns></returns>
        internal HashSet<Atom> UseAlternativeCavity(IEnumerable<Atom> mol)
        {
            //use alternative cavity definition and try find again
            var cavity = NonMetalNeighbors(Metal, mol).ToList();
            //iterate all combinations
            foreach (IEnumerable<Atom> comb in cavity.GetCombinations(4).OrderBy(l => l.Sum(a => Atom.Distance(a, Metal))))
            {
                HashSet<Atom> path = RingPath(comb.First(), RingAtoms.Count() - 8);
                if (path != null)
                {
                    foreach (Atom N in comb) path.UnionWith(RingPath(N, 5).AsParallel());
                    if (path.Count == RingAtoms.Count()) return path;
                }
            }
            return null;
        }

        /// <summary>
        /// Find Corpus of Macrocyle
        /// </summary>
        /// <param name="mol"></param>
        /// <returns></returns>
        internal HashSet<Atom> FindCorpus(IEnumerable<Atom> mol)
        {
            //get N4 Cavity
            List<Atom> cavity = N4Cavity(mol);

            //get inner ring path 
            var corpus = new HashSet<Atom>(); ;
            if (cavity.Count != 0) corpus = RingPath(cavity.First(), RingAtoms.Count() - 8);
            if (corpus?.Count == 0 && HasMetal(false)) corpus = UseAlternativeCavity(mol);

            //just need to find beta atoms now to complete the cycle, luckily we can use the cavity to find five membered rings and combine with corpus, and distinct.
            foreach (Atom N in cavity) corpus?.UnionWith(RingPath(N, 5));

            //try to use other method
            if (corpus?.Count != RingAtoms.Count && HasMetal(false)) return UseAlternativeCavity(mol);

            return corpus;
        }

        /// <summary>
        /// returns ring path with given size
        /// in theory every cavity atom should have a path to it's alpha neighbor surrounding the whole cycle without beta!
        /// So it is  RingAtoms.Count - 8 (8 beta positions) for the inner large ring and RingAtoms.Count - 4 for the outer large ring.
        /// </summary>
        /// <param name="mol"></param>
        /// <param name="atom">An Atom</param>
        /// <returns></returns>
        internal HashSet<Atom> RingPath(Atom atom, int size) => DFSUtil.GetAllPaths(atom, NonMetalNeighbors(atom).Where(n => n.Type != "H").First(), NonMetalNeighbors, size).FirstOrDefault();


        /// <summary>
        /// An overrideable Method to get C1 Atom. 
        /// In a Porphyrin it does not care which alpha atom is C1, so return any of them...
        /// override this methode in any other class
        /// TODO: Think of a switch for Isoporphyrins to return C10 as meso sp3 C.
        /// </summary>
        /// <returns></returns>
        public virtual Atom C1(IEnumerable<Atom> cycle) => Vertex3Atoms(cycle).First();

        /// <summary>
        /// Detect the macrocyclic structure
        /// </summary>
        /// <returns></returns>
        public async Task Detect()
        {
            //find all connected structures
            //Note: the axial ligand is connected to the macrocycle
            //it will lead to wrong mean planes and Centroid in N4Cavity() -> Cobalamins
            var figures = new List<IEnumerable<Atom>>();
            await foreach (IEnumerable<Atom> fig in DFSUtil.ConnectedFigures(Atoms.Where(s => !s.IsMetal && NonMetalNeighbors(s).Count() >= 2), NonMetalNeighbors))
                figures.Add(fig);

            var cycle = new HashSet<Atom>();
            //loop through all remaining connected figures
            foreach (IEnumerable<Atom> mol in figures.Where(s => s.Count() >= RingAtoms.Count))
            {
                //get Corpus by Pathfinding Method
                cycle = FindCorpus(mol);

                //default to first macrocycle found for now
                if (cycle != null && cycle.Count == RingAtoms.Count)
                    break;
            }
            //assign Identifiers
            if (cycle != null) NameAtoms(cycle);
        }

        /// <summary>
        /// Assign the correct Identifiers for a cycle CN-corpus
        /// </summary>
        /// <param name="cycle"></param>
        public void NameAtoms(IEnumerable<Atom> cycle)
        {
            //cycle valid?
            if (cycle.Count() == RingAtoms.Count)
            {
                //set macrocycle flag
                Atoms.Where(s => cycle.Contains(s)).ToList().ForEach(s => { s.IsMacrocycle = true; s.Identifier = s.Type + "M"; });
                Atoms.Where(s => !cycle.Contains(s)).ToList().ForEach(s => { s.IsMacrocycle = false; s.Identifier = s.Type + "X"; });
                //track visited atoms
                var visited = new HashSet<Atom>();

                //set Identifier for C1 Atom
                C1(cycle).Identifier = "C1";
                //get N4 Cavity, necessary for performance
                List<Atom> cavity = N4Cavity(cycle);
                //force c2 to be first step
                Atom current = Neighbors(C1(cycle), cycle).Where(s => !cavity.Contains(s) && !RingPath(cavity.First(), RingAtoms.Count() - 8).Contains(s)).FirstOrDefault();
                current.Identifier = "C2";
                //add C1&C2 to visited
                visited.UnionWith(new HashSet<Atom>() { current, C1(cycle) });
                //get carbon atoms
                var carbons = RingAtoms.Where(s => s.Contains("C")).OrderBy(s => int.Parse(s.Replace("C", ""))).ToList();
                int i = 2;
                //loop through atoms and name them
                while (visited.Count() != carbons.Count())
                {
                    foreach (Atom neighbor in Neighbors(current, cycle).Where(s => !visited.Contains(s) && !cavity.Contains(s)))
                    {
                        //add to visited and assign Identifier
                        neighbor.Identifier = carbons[i];
                        visited.Add(current = neighbor);
                        i++;
                    }
                }
                //set up identifiers for nitrogens
                for (int j = 1; j <= 4; j++) cavity.Where(s => Neighbors(s).Contains(ByIdentifier(AlphaAtoms[2 * j - 1]))).FirstOrDefault().Identifier = "N" + j;
            }
        }

        /// <summary>
        /// Returns true if the bond between the two atoms is a valid macrocycle bond
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public bool IsValidBond(Atom a1, Atom a2) => a1.IsMacrocycle && a2.IsMacrocycle
            && (
                Bonds.Contains(new Tuple<string, string>(a1.Identifier, a2.Identifier))
                || Bonds.Contains(new Tuple<string, string>(a2.Identifier, a1.Identifier)
                )
            || (
                (a1.IsMetal || a2.IsMetal)
                && HasMetal())
            );

        /// <summary>
        /// Returns new Instance of this molecule
        /// ICloneable.Clone()
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();
    }
}

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry
{
    public abstract class Macrocycle : Molecule, ICloneable
    {
        public Macrocycle(List<Atom> Atoms) : base(Atoms) { }

        /// <summary>
        /// Current Data Points
        /// </summary>
        public List<AtomDataPoint> dataPoints = new List<AtomDataPoint>();
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

        /// <summary>
        /// Multipliers for C-Atom positioning
        /// </summary>
        public abstract Dictionary<string, double> Multiplier { get; }

        /// <summary>
        /// Dihedrals of Macrocylce
        /// </summary>
        public abstract List<string[]> Dihedrals { get; }

        public enum Type { Corrole, Porphyrin, Norcorrole, Corrphycene, Porphycene };
        public virtual Macrocycle.Type type { get; }

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
        private bool isAlpha(Atom a) => AlphaAtoms.Contains(a.Identifier) ? true : false;

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
        /// Method for Datapoint calculation //rewritten
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<AtomDataPoint> CalculateDataPoints()
        {
            //check if every atom is present in configuration
            foreach (string id in RingAtoms)
            {
                if (Atoms.FindAll(s => s.Identifier == id && s.IsMacrocycle).Count != 1)
                {
                    System.Windows.Forms.MessageBox.Show($"Found Issues with Atom {id}! Please check your configuration.");
                    yield break;
                }
            }
            //reorder Atoms
            Atoms = Atoms.OrderBy(s => RingAtoms.IndexOf(s.Identifier)).ToList();

            //current alpha-alpha distance
            double distance = 0;
            //current fixpoint
            double fixPoint = 1;

            //loop through every non Metal Atom of Macrocycle and return datapoint
            foreach (Atom a in Atoms.Where(s => s.IsMacrocycle && !s.IsMetal))
            {
                double xCoord = 1;
                if (a.Type == "C") xCoord = fixPoint + distance * Multiplier[a.Identifier];
                if (a.Type == "N") xCoord = fixPoint + distance / 2;

                //starts with C1 which is alpha per definition, so refresh distance every alpha atom.
                if (isAlpha(a) && GetNextAlpha(a) != null) distance = Atom.Distance(a, GetNextAlpha(a));

                //alpha atoms are fixpoints
                if (isAlpha(a)) fixPoint = xCoord;

                yield return new AtomDataPoint(xCoord, a.DistanceToPlane(GetMeanPlane()), a);
            }
        }
        /// <summary>
        /// Return the cycle's datapoints
        /// </summary>
        /// <returns>AtomDataPoints</returns>
        public List<AtomDataPoint> GetDataPoints()
        {
            dataPoints.Clear();
            dataPoints.AddRange(CalculateDataPoints());

            if (HasMetal) dataPoints.Add(new AtomDataPoint(
                    (dataPoints.First().X + dataPoints.Last().X) / 2,
                    GetMetal().DistanceToPlane(GetMeanPlane()),
                    GetMetal()));

            return dataPoints;
        }


        /// <summary>
        /// Indicates if a Metal Atom is present
        /// </summary>
        public bool HasMetal => Atoms.Where(s => s.IsMacrocycle && s.IsMetal).Count() > 0;

        /// <summary>
        /// Returns Bondlenghts, etc. for the cycles
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> Metrics()
        {
            Dictionary<string, double> MetricDict = new Dictionary<string, double>();
            //N4 Lenghts
            if (HasMetal)
            {
                for (int i = 1; i <= 4; i++) MetricDict.Add($"Bond_N{i},M", CalculateDistance($"N{i}", GetMetal().Identifier));
                MetricDict.Add("Bond_M,msp", GetMetal().DistanceToPlane(GetMeanPlane()));
            }
            Dihedrals.ForEach(d => MetricDict.Add("Dihedral_" + string.Join(",", d), Dihedral(d)));

            return MetricDict;
        }

        /// <summary>
        /// calculates a dihedral
        /// </summary>
        /// <param name="Atoms"></param>
        /// <returns></returns>
        public double Dihedral(string[] Atoms)
        {
            if (Atoms.Length != 4) return 0;
            //build normalized vectors
            Vector<double> b1 = (-(DenseVector.OfArray(ByIdentifier(Atoms[0], true).XYZ()) - DenseVector.OfArray(ByIdentifier(Atoms[1], true).XYZ()))).Normalize(2);
            Vector<double> b2 = (DenseVector.OfArray(ByIdentifier(Atoms[1], true).XYZ()) - DenseVector.OfArray(ByIdentifier(Atoms[2], true).XYZ())).Normalize(2);
            Vector<double> b3 = (DenseVector.OfArray(ByIdentifier(Atoms[3], true).XYZ()) - DenseVector.OfArray(ByIdentifier(Atoms[2], true).XYZ())).Normalize(2);

            //calculate crossproducts
            var c1 = MathUtil.CrossProduct(b1, b2);
            var c2 = MathUtil.CrossProduct(b2, b3);
            var c3 = MathUtil.CrossProduct(c1, b2);

            //get x&y as dotproducts 
            var x = c1.DotProduct(c2);
            var y = c3.DotProduct(c2);

            return 180.0 / Math.PI * Math.Atan2(y, x);
        }

        /// <summary>
        /// Draws a line between two points
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="sim">is Simulation</param>
        /// <returns>ArrowAnnotation aka Bond</returns>
        public static ArrowAnnotation DrawBond(AtomDataPoint a1, AtomDataPoint a2, int mode = 0) => new ArrowAnnotation
        {
            StartPoint = a1.GetDataPoint(),
            EndPoint = a2.GetDataPoint(),
            HeadWidth = 0,
            HeadLength = 0,
            Color = Properties.Settings.Default.singleColor ? Atom.modesSingleColor[mode] : Atom.modesMultiColor[mode],
            Layer = AnnotationLayer.BelowSeries,
            StrokeThickness = Properties.Settings.Default.lineThickness,
            Tag = a1.atom.Identifier + "," + a2.atom.Identifier
        };

        /// <summary>
        /// Generates all Bonds as Annotations
        /// </summary
        /// <returns>Annotation aka Bonds</returns>
        public virtual IEnumerable<ArrowAnnotation> DrawBonds(int mode = 0)
        {
            dataPoints = dataPoints.OrderBy(s => s.X).ToList();

            foreach (Tuple<string, string> t in Bonds)
                yield return Macrocycle.DrawBond(
                    dataPoints.Where(s => s.atom.Identifier == t.Item1 && s.atom.IsMacrocycle).First(),
                    dataPoints.Where(s => s.atom.Identifier == t.Item2 && s.atom.IsMacrocycle).First(),
                    mode);

            //add metal atoms
            if (HasMetal)
                foreach (AtomDataPoint n in dataPoints.Where(s => s.atom.Type == "N").ToList())
                {
                    ArrowAnnotation b = DrawBond(dataPoints.Where(s => s.atom == GetMetal()).FirstOrDefault(), n);
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
        public Atom ByIdentifier(string id, bool forceMacroCycle = false) => Atoms.Where(s => s.Identifier == id && (forceMacroCycle ? s.IsMacrocycle == forceMacroCycle : true)).FirstOrDefault();

        /// <summary>
        /// Builds RangeColorAxis
        /// </summary>
        /// <returns></returns>
        public RangeColorAxis BuildColorAxis()
        {
            RangeColorAxis xR = new RangeColorAxis()
            {
                Key = "colors",
                Position = AxisPosition.Bottom,
                IsAxisVisible = false
            };



            xR.AddRange(0, 0.1, Element.Create("C").OxyColor);
            //sim
            xR.AddRange(1000, 1000.1, OxyColor.FromAColor(75, Element.Create("C").OxyColor));
            //diff
            xR.AddRange(2000, 2000.1, OxyColor.FromAColor(50, Element.Create("C").OxyColor));

            //comparisons
            xR.AddRange(3000, 3000.1, OxyColor.FromAColor(75, Element.Create("C").OxyColor));
            xR.AddRange(4000, 4000.1, OxyColor.FromAColor(75, Element.Create("C").OxyColor));

            foreach (AtomDataPoint dp in dataPoints)
            {
                if (dp.atom.Type != "C")
                {
                    OxyColor color = dp.atom.OxyColor;
                    double min = dp.X - 0.25;
                    double max = dp.X + 0.25;
                    xR.AddRange(min, max, color);
                    xR.AddRange(min + 1000, max + 1000, OxyColor.FromAColor(75, color));
                    xR.AddRange(min + 2000, max + 2000, OxyColor.FromAColor(50, color));
                    xR.AddRange(min + 3000, max + 3000, OxyColor.FromAColor(75, color));
                    xR.AddRange(min + 4000, max + 4000, OxyColor.FromAColor(75, color));
                }
                else if (dp.Value != 1000 && dp.Value != 2000 && dp.Value != 3000 && dp.Value != 4000)
                {
                    dp.Value = 0;
                }
            }

            return xR;
        }

        /// <summary>
        /// Gets the absolute mean displacement over all atoms
        /// </summary>
        /// <returns></returns>
        public double MeanDisplacement() => Math.Sqrt(dataPoints.Sum(s => Math.Pow(s.Y, 2)));

        /// <summary>
        /// Returns all Neighbors of an Atom
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public IEnumerable<Atom> Neighbors(Atom A) => Neighbors(A, Atoms);

        /// <summary>
        /// Returns non Metal Neighbors of an atom
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public IEnumerable<Atom> NonMetalNeighbors(Atom A) => NonMetalNeighbors(A, Atoms);


        /// <summary>
        /// Gets the 4 atoms of cavity by centroid method
        /// </summary>
        /// <param name="mol"></param>
        /// <returns></returns>
        internal List<Atom> N4Cavity(IEnumerable<Atom> mol)
        {
            //calculate centroid
            var centroid = Molecule.GetCentroid(mol);
            //inject centroid into atoms as Lanthanium atom (because of big radius)
            //basically need a sphere where to catch atoms in to get N4 Cavity
            Atoms.Add(new Atom("La Centroid", centroid.X, centroid.Y, centroid.Z));
            //find neighbors which have more neighbors that are non metal nor hydrogen (NH) and belong to current figure
            var neighbors = NonMetalNeighbors(ByIdentifier("La Centroid"), mol).Where(s => NonMetalNeighbors(s, mol).Where(n => n.Type != "H").Count() == 2 && mol.Contains(s));

            //use the nearest 4 neighbors by mean plane and centroid distance to get cavity atoms
            var cavity = neighbors.OrderBy(n => Atom.Distance(ByIdentifier("La Centroid"), n) + Math.Abs(n.DistanceToPlane(GetMeanPlane(mol)))).Take(4).ToList();

            //remove centroid
            Atoms.Remove(ByIdentifier("La Centroid"));

            return cavity;
        }

        /// <summary>
        /// Find Corpus of Macrocyle
        /// </summary>
        /// <param name="mol"></param>
        /// <returns></returns>
        internal HashSet<Atom> FindCorpus(IEnumerable<Atom> mol)
        {
            //get N4 Cavity
            var cavity = N4Cavity(mol);

            //get inner ring path
            var corpus = RingPath(cavity.First(), RingAtoms.Count() - 8);

            //just need to find beta atoms now to complete the cycle, luckily we can use the cavity to find five membered rings and combine with corpus, and distinct.
            foreach (Atom N in cavity) corpus.UnionWith(RingPath(N, 5));

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
        /// TODO: Think of a switch for Isoporphyrins to return C10 as meso C.
        /// </summary>
        /// <returns></returns>
        public virtual Atom C1(IEnumerable<Atom> cycle) => Vertex3Atoms(cycle).First();

        /// <summary>
        /// Detect the macrocyclic structure
        /// </summary>
        /// <returns></returns>
        public void Detect()
        {
            //find all connected structures
            HashSet<HashSet<Atom>> connected = new HashSet<HashSet<Atom>>();
            foreach (var atom in Atoms.Where(s => !s.IsMetal))
                connected.Add(DFSUtil.DFS(atom, NonMetalNeighbors));
            //and compare to get all distinct structures
            //from the distinct structures select all that have more atoms than type's ringatoms 
            var distinct = connected.Distinct(HashSet<Atom>.CreateSetComparer()).Where(s => s.Count() >= RingAtoms.Count);

            var cycle = new HashSet<Atom>();
            //loop through all remaining connected figures
            foreach (var mol in distinct)
            {
                //get Corpus by Pathfinding Method
                cycle = FindCorpus(mol);

                //default to first macrocycle found for now
                if (cycle.Count == RingAtoms.Count)
                    break;
            }
            //assign Identifiers
            NameAtoms(cycle);
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
                HashSet<Atom> visited = new HashSet<Atom>();

                //set Identifier for C1 Atom
                C1(cycle).Identifier = "C1";

                //force c2 to be first step
                Atom current = Neighbors(C1(cycle), cycle).Where(s => !N4Cavity(cycle).Contains(s) && !RingPath(N4Cavity(cycle).First(), RingAtoms.Count() - 8).Contains(s)).FirstOrDefault();
                current.Identifier = "C2";
                //add C1&C2 to visited
                visited.UnionWith(new HashSet<Atom>() { current, C1(cycle) });
                var carbons = RingAtoms.Where(s => s.Contains("C")).OrderBy(s => int.Parse(s.Replace("C", ""))).ToList();
                int i = 2;
                //loop through atoms and name them
                while (visited.Count() != carbons.Count())
                {
                    foreach (var neighbor in Neighbors(current, cycle).Where(s => !visited.Contains(s) && !N4Cavity(cycle).Contains(s)))
                    {
                        //add to visited and assign Identifier
                        neighbor.Identifier = carbons[i];
                        visited.Add(current = neighbor);
                        i++;
                    }
                }
                //set up identifiers for nitrogens
                for (int j = 1; j <= 4; j++) N4Cavity(cycle).Where(s => Neighbors(s).Contains(ByIdentifier(AlphaAtoms[2 * j - 1]))).FirstOrDefault().Identifier = "N" + j;
            }
        }
        /// <summary>
        /// Returns new Instance of this molecule
        /// ICloneable.Clone()
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();
    }
}

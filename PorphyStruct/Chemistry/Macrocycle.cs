using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry
{
    public class Macrocycle : Molecule
    {
        public List<AtomDataPoint> dataPoints = new List<AtomDataPoint>();
        //note! the Macrocycle NEEDS(!!!) the crystal identifiers. if data comes elsewhere you MUST type identifers by yourself
        public Macrocycle(List<Atom> Atoms) : base(Atoms)
        {
            //does nothing for now
        }

        public enum Type { Corrole, Porphyrin, Norcorrole, Corrphycene, Porphycene };
        public Macrocycle.Type type;

        //all bonds of a porphyrin
        public static List<Tuple<string, string>> PorphyrinBonds = new List<Tuple<string, string>>() {
            new Tuple<string, string>("C1", "C2"),
            new Tuple<string, string>("C1", "N1"),
            new Tuple<string, string>("C2", "C3"),
            new Tuple<string, string>("C3", "C4"),
            new Tuple<string, string>("N1", "C4"),
            new Tuple<string, string>("C5", "C4"),
            new Tuple<string, string>("C5", "C6"),
            new Tuple<string, string>("C6", "N2"),
            new Tuple<string, string>("C6", "C7"),
            new Tuple<string, string>("C8", "C7"),
            new Tuple<string, string>("C8", "C9"),
            new Tuple<string, string>("C9", "C10"),
            new Tuple<string, string>("C9", "N2"),
            new Tuple<string, string>("C11", "C10"),
            new Tuple<string, string>("C11", "N3"),
            new Tuple<string, string>("C11", "C12"),
            new Tuple<string, string>("C12", "C13"),
            new Tuple<string, string>("C13", "C14"),
            new Tuple<string, string>("N3", "C14"),
            new Tuple<string, string>("C14", "C15"),
            new Tuple<string, string>("C15", "C16"),
            new Tuple<string, string>("C16", "C17"),
            new Tuple<string, string>("C17", "C18"),
            new Tuple<string, string>("C18", "C19"),
            new Tuple<string, string>("N4", "C19"),
            new Tuple<string, string>("N4", "C16"),
            new Tuple<string,string>("C19", "C20"),
            new Tuple<string, string>("C20", "C1")
        };

        //all bonds of a corrole == Porphyrin Bonds without Bonding to C20
        public static List<Tuple<string, string>> CorroleBonds = PorphyrinBonds.Except(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C20", "C1"),
            new Tuple<string, string>("C19", "C20")
        }).ToList();

        //all bonds of a norcorrole == Corrole Bonds without Bonding to C10, but add bond from c9 to c11
        public static List<Tuple<string, string>> NorcorroleBonds = CorroleBonds.Except(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C9", "C10"),
            new Tuple<string, string>("C11", "C10")
        }).Concat(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C9", "C11")
        }).ToList();

        //all bonds of a corrphycene. == Porphyrin Bonds without C11 & C14 to N3, C16 & C19 to N4, C20 to C1,  but add C12 and C15 to N3, C17 and C20 to N4
        public static List<Tuple<string, string>> CorrphyceneBonds = PorphyrinBonds.Except(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C11", "N3"),
            new Tuple<string, string>("N3", "C14"),
            new Tuple<string, string>("N4", "C16"),
            new Tuple<string, string>("N4", "C19"),
            new Tuple<string, string>("C20", "C1")
        }).Concat(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C12", "N3"),
            new Tuple<string, string>("C15", "N3"),
            new Tuple<string, string>("C17", "N4"),
            new Tuple<string, string>("C20", "N4")
        }).ToList();

        //all bonds of a Porphycene. == Porphyrin Bonds without C6&C9 to N2 && C16 & C19 to N4 but add C7&C10 to N2 && C17&C20 to N4, remove C20toC1
        public static List<Tuple<string, string>> PorphyceneBonds = PorphyrinBonds.Except(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C6", "N2"),
            new Tuple<string, string>("C9", "N2"),
            new Tuple<string, string>("N4", "C16"),
            new Tuple<string, string>("N4", "C19"),
            new Tuple<string, string>("C20", "C1")
        }).Concat(new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("C7", "N2"),
            new Tuple<string, string>("C10", "N2"),
            new Tuple<string, string>("C17", "N4"),
            new Tuple<string, string>("C20", "N4"),
        }).ToList();

        //list of porphyrin atoms, used for porphycene and corrphycene, too
        public static List<string> PorphyrinAtoms = new List<string>() { "C1", "C2", "N1", "C3", "C4", "C5", "C6", "C7", "N2", "C8", "C9", "C10", "C11", "C12", "N3", "C13", "C14", "C15", "C16", "C17", "N4", "C18", "C19", "C20" };

        //list of corrole atoms = Porphyrin Atoms without C20
        public static List<string> CorroleAtoms = PorphyrinAtoms.Except(new List<string>() { "C20" }).ToList();

        //list of norcorrole atoms = Corrole Atoms without C10
        public static List<string> NorcorroleAtoms = CorroleAtoms.Except(new List<string>() { "C10" }).ToList();


        /// <summary>
        /// Converts Crystal to Macrocycle
        /// </summary>
        /// <param name="v"></param>
        public static explicit operator Macrocycle(Crystal v)
        {
            return new Macrocycle(v.Atoms);
        }

        /// <summary>
        /// Gets the centroid of this Macrocycle
        /// </summary>
        /// <returns>Centroid as Vector3D</returns>
        public Vector3D GetCentroid()
        {            
            //get the centroid
            return Point3D.Centroid(GetPoints()).ToVector3D();
        }

        /// <summary>
        /// Converts the xyz into Point3D because some methods need math net spatial...
        /// </summary>
        /// <returns></returns>
        private List<Point3D> GetPoints()
        {
            List<Point3D> points = new List<Point3D>();
            foreach (Atom atom in Atoms)
            {
                if (atom.IsMacrocycle) points.Add(new Point3D(atom.X, atom.Y, atom.Z));
            }
            return points;
        }


        /// <summary>
        /// Gets the mean plane of this Macrocycle
        /// </summary>
        /// <returns>The Plane Object (Math.Net)</returns>
        public Plane GetMeanPlane()
        {
            //convert coordinates into Point3D because centroid method is only available in math net spatial
            List<Point3D> points = GetPoints();
            foreach (Atom atom in Atoms)
            {
                if (atom.IsMacrocycle) points.Add(new Point3D(atom.X, atom.Y, atom.Z));
            }
            //calculate Centroid first
            //get the centroid
            Vector3D centroid = GetCentroid();

            //subtract centroid from each point... & build matrix of that
            Matrix<double> A = Matrix<double>.Build.Dense(3, points.Count);
            for (int x = 0; x < points.Count; x++)
            {
                A[0, x] = (points[x] - centroid).X;
                A[1, x] = (points[x] - centroid).Y;
                A[2, x] = (points[x] - centroid).Z;
            }

            //get svd
            var svd = A.Svd(true);

            //get plane unit vector
            double a = svd.U[0, 2];
            double b = svd.U[1, 2];
            double c = svd.U[2, 2];

            double d = -(centroid.DotProduct(new Vector3D(a, b, c)));

            return new Plane(a, b, c, d);
        }

        /// <summary>
        /// calculate distance between two atoms (as identifiers are needed this must be in Macrocycle-Class!!)
        /// </summary>
        /// <param name="id1">Identifier 1</param>
        /// <param name="id2">Identifier 2</param>
        /// <returns>The Vectordistance</returns>
        public double CalculateDistance(string id1, string id2)
        {
            return Distance.Euclidean(this.Atoms.Where(s => s.Identifier == id1 && s.IsMacrocycle).First().XYZ(), this.Atoms.Where(s => s.Identifier == id2 && s.IsMacrocycle).First().XYZ());
        }

        /// <summary>
        /// Calculates the DataPoints for a corrole type cycle.
        /// </summary>
        /// <returns>AtomDataPoints</returns>
        public List<AtomDataPoint> GetCorroleDataPoints()
        {
            //check if every atom is found:
            foreach (string id in CorroleAtoms)
            {
                if (Atoms.Where(s => s.Identifier == id).Count() == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Atom " + id + " could not be determined!");
                    return null;
                }
            }

            //order atoms
            Atoms = Atoms.OrderBy(s => CorroleAtoms.IndexOf(s.Identifier)).ToList();

            //get the 7 distances for corrole
            double d1 = CalculateDistance("C1", "C4");
            double d2 = CalculateDistance("C4", "C6");
            double d3 = CalculateDistance("C6", "C9");
            double d4 = CalculateDistance("C9", "C11");
            double d5 = CalculateDistance("C11", "C14");
            double d6 = CalculateDistance("C14", "C16");
            double d7 = CalculateDistance("C16", "C19");

            double[] fixPoints = new double[6];
            foreach (Atom a in Atoms)
            {
                if (a.IsMacrocycle)
                {
                    //defaulting to 1
                    double xCoord = 1;

                    //set x axis
                    if (a.Identifier == "C1")
                        xCoord = 1;
                    if (a.Identifier == "C2")
                        xCoord = 1 + d1 / 3;
                    if (a.Identifier == "N1")
                        xCoord = 1 + d1 / 2;
                    if (a.Identifier == "C3")
                        xCoord = 1 + (d1 / 3) * 2;
                    if (a.Identifier == "C4")
                    { xCoord = 1 + d1; fixPoints[0] = xCoord; }
                    if (a.Identifier == "C5")
                        xCoord = d2 / 2 + fixPoints[0];
                    if (a.Identifier == "C6")
                    { xCoord = d2 + fixPoints[0]; fixPoints[1] = xCoord; }
                    if (a.Identifier == "C7")
                        xCoord = d3 / 3 + fixPoints[1];
                    if (a.Identifier == "N2")
                        xCoord = d3 / 2 + fixPoints[1];
                    if (a.Identifier == "C8")
                        xCoord = (d3 / 3) * 2 + fixPoints[1];
                    if (a.Identifier == "C9")
                    { xCoord = d3 + fixPoints[1]; fixPoints[2] = xCoord; }
                    if (a.Identifier == "C10")
                        xCoord = d4 / 2 + fixPoints[2];
                    if (a.Identifier == "C11")
                    { xCoord = d4 + fixPoints[2]; fixPoints[3] = xCoord; }
                    if (a.Identifier == "C12")
                        xCoord = d5 / 3 + fixPoints[3];
                    if (a.Identifier == "N3")
                        xCoord = d5 / 2 + fixPoints[3];
                    if (a.Identifier == "C13")
                        xCoord = (d5 / 3) * 2 + fixPoints[3];
                    if (a.Identifier == "C14")
                    { xCoord = d5 + fixPoints[3]; fixPoints[4] = xCoord; };
                    if (a.Identifier == "C15")
                        xCoord = d6 / 2 + fixPoints[4];
                    if (a.Identifier == "C16")
                    { xCoord = d6 + fixPoints[4]; fixPoints[5] = xCoord; };
                    if (a.Identifier == "C17")
                        xCoord = d7 / 3 + fixPoints[5];
                    if (a.Identifier == "N4")
                        xCoord = d7 / 2 + fixPoints[5];
                    if (a.Identifier == "C18")
                        xCoord = (d7 / 3) * 2 + fixPoints[5];
                    if (a.Identifier == "C19")
                        xCoord = d7 + fixPoints[5];

                    //add data
                    dataPoints.Add(new AtomDataPoint(xCoord, a.DistanceToPlane(GetMeanPlane()), a));
                }
            }
            return dataPoints;
        }

        /// <summary>
        /// Calculates the DataPoints for a corrole type cycle.
        /// </summary>
        /// <returns>AtomDataPoints</returns>
        public List<AtomDataPoint> GetNorcorroleDataPoints()
        {
            //check if every atom is found:
            foreach (string id in NorcorroleAtoms)
            {
                if (Atoms.Where(s => s.Identifier == id).Count() == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Atom " + id + " could not be determined!");
                    return null;
                }
            }

            //order atoms
            Atoms = Atoms.OrderBy(s => NorcorroleAtoms.IndexOf(s.Identifier)).ToList();

            //get the 7 distances for corrole
            double d1 = CalculateDistance("C1", "C4");
            double d2 = CalculateDistance("C4", "C6");
            double d3 = CalculateDistance("C6", "C9");
            double d4 = CalculateDistance("C9", "C11");
            double d5 = CalculateDistance("C11", "C14");
            double d6 = CalculateDistance("C14", "C16");
            double d7 = CalculateDistance("C16", "C19");

            double[] fixPoints = new double[6];
            foreach (Atom a in Atoms)
            {
                if (a.IsMacrocycle)
                {
                    //defaulting to 1
                    double xCoord = 1;

                    //set x axis
                    if (a.Identifier == "C1")
                        xCoord = 1;
                    if (a.Identifier == "C2")
                        xCoord = 1 + d1 / 3;
                    if (a.Identifier == "N1")
                        xCoord = 1 + d1 / 2;
                    if (a.Identifier == "C3")
                        xCoord = 1 + (d1 / 3) * 2;
                    if (a.Identifier == "C4")
                    { xCoord = 1 + d1; fixPoints[0] = xCoord; }
                    if (a.Identifier == "C5")
                        xCoord = d2 / 2 + fixPoints[0];
                    if (a.Identifier == "C6")
                    { xCoord = d2 + fixPoints[0]; fixPoints[1] = xCoord; }
                    if (a.Identifier == "C7")
                        xCoord = d3 / 3 + fixPoints[1];
                    if (a.Identifier == "N2")
                        xCoord = d3 / 2 + fixPoints[1];
                    if (a.Identifier == "C8")
                        xCoord = (d3 / 3) * 2 + fixPoints[1];
                    if (a.Identifier == "C9")
                    { xCoord = d3 + fixPoints[1]; fixPoints[2] = xCoord; }
                    if (a.Identifier == "C11")
                    { xCoord = d4 + fixPoints[2]; fixPoints[3] = xCoord; }
                    if (a.Identifier == "C12")
                        xCoord = d5 / 3 + fixPoints[3];
                    if (a.Identifier == "N3")
                        xCoord = d5 / 2 + fixPoints[3];
                    if (a.Identifier == "C13")
                        xCoord = (d5 / 3) * 2 + fixPoints[3];
                    if (a.Identifier == "C14")
                    { xCoord = d5 + fixPoints[3]; fixPoints[4] = xCoord; };
                    if (a.Identifier == "C15")
                        xCoord = d6 / 2 + fixPoints[4];
                    if (a.Identifier == "C16")
                    { xCoord = d6 + fixPoints[4]; fixPoints[5] = xCoord; };
                    if (a.Identifier == "C17")
                        xCoord = d7 / 3 + fixPoints[5];
                    if (a.Identifier == "N4")
                        xCoord = d7 / 2 + fixPoints[5];
                    if (a.Identifier == "C18")
                        xCoord = (d7 / 3) * 2 + fixPoints[5];
                    if (a.Identifier == "C19")
                        xCoord = d7 + fixPoints[5];

                    //add data
                    dataPoints.Add(new AtomDataPoint(xCoord, a.DistanceToPlane(GetMeanPlane()), a));
                }
            }
            return dataPoints;
        }

        /// <summary>
        /// Calculates the DataPoints for a porphyrin type cycle.
        /// </summary>
        /// <returns>AtomDataPoints</returns>
        public List<AtomDataPoint> GetPorphyrinDataPoints()
        {
            //check if every atom is found:
            foreach (string id in PorphyrinAtoms)
            {
                if (Atoms.Where(s => s.Identifier == id).Count() == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Atom " + id + " could not be determined!");
                    return null;
                }
            }

            //order atoms
            Atoms = Atoms.OrderBy(s => PorphyrinAtoms.IndexOf(s.Identifier)).ToList();

            //get the x distances for porphyrin
            double d1 = CalculateDistance("C1", "C4");
            double d2 = CalculateDistance("C4", "C6");
            double d3 = CalculateDistance("C6", "C9");
            double d4 = CalculateDistance("C9", "C11");
            double d5 = CalculateDistance("C11", "C14");
            double d6 = CalculateDistance("C14", "C16");
            double d7 = CalculateDistance("C16", "C19");
            double d8 = CalculateDistance("C19", "C1");

            double[] fixPoints = new double[7];
            foreach (Atom a in Atoms)
            {
                if (a.IsMacrocycle)
                {
                    //defaulting to 1
                    double xCoord = 1;

                    //set x axis

                    if (a.Identifier == "C20")
                    {
                        xCoord = 1;
                        //add c20 twice
                        dataPoints.Add(new AtomDataPoint((d7 + fixPoints[6] + (d8 / 2)), a.DistanceToPlane(GetMeanPlane()), a));
                    }
                    if (a.Identifier == "C1")
                    {
                        xCoord = 1 + (d8 / 2);
                        fixPoints[0] = xCoord;
                    }
                    if (a.Identifier == "C2")
                        xCoord = d1 / 3 + fixPoints[0];
                    if (a.Identifier == "N1")
                        xCoord = d1 / 2 + fixPoints[0];
                    if (a.Identifier == "C3")
                        xCoord = (d1 / 3) * 2 + fixPoints[0];
                    if (a.Identifier == "C4")
                    {
                        xCoord = d1 + fixPoints[0];
                        fixPoints[1] = xCoord;
                    }
                    if (a.Identifier == "C5")
                        xCoord = d2 / 2 + fixPoints[1];
                    if (a.Identifier == "C6")
                    {
                        xCoord = d2 + fixPoints[1];
                        fixPoints[2] = xCoord;
                    }
                    if (a.Identifier == "C7")
                        xCoord = d3 / 3 + fixPoints[2];
                    if (a.Identifier == "N2")
                        xCoord = d3 / 2 + fixPoints[2];
                    if (a.Identifier == "C8")
                        xCoord = (d3 / 3) * 2 + fixPoints[2];
                    if (a.Identifier == "C9")
                    {
                        xCoord = d3 + fixPoints[2];
                        fixPoints[3] = xCoord;
                    }
                    if (a.Identifier == "C10")
                        xCoord = d4 / 2 + fixPoints[3];
                    if (a.Identifier == "C11")
                    {
                        xCoord = d4 + fixPoints[3];
                        fixPoints[4] = xCoord;
                    }
                    if (a.Identifier == "C12")
                        xCoord = d5 / 3 + fixPoints[4];
                    if (a.Identifier == "N3")
                        xCoord = d5 / 2 + fixPoints[4];
                    if (a.Identifier == "C13")
                        xCoord = (d5 / 3) * 2 + fixPoints[4];
                    if (a.Identifier == "C14")
                    {
                        xCoord = d5 + fixPoints[4];
                        fixPoints[5] = xCoord;
                    };
                    if (a.Identifier == "C15")
                        xCoord = d6 / 2 + fixPoints[5];
                    if (a.Identifier == "C16")
                    {
                        xCoord = d6 + fixPoints[5];
                        fixPoints[6] = xCoord;
                    };
                    if (a.Identifier == "C17")
                        xCoord = d7 / 3 + fixPoints[6];
                    if (a.Identifier == "N4")
                        xCoord = d7 / 2 + fixPoints[6];
                    if (a.Identifier == "C18")
                        xCoord = (d7 / 3) * 2 + fixPoints[6];
                    if (a.Identifier == "C19")
                        xCoord = d7 + fixPoints[6];



                    //add data
                    dataPoints.Add(new AtomDataPoint(xCoord, a.DistanceToPlane(GetMeanPlane()), a));
                }
            }
            return dataPoints;
        }



        /// <summary>
        /// Calculates the DataPoints for a corrphycene type cycle.
        /// </summary>
        /// <returns>AtomDataPoints</returns>
        public List<AtomDataPoint> GetCorrphyceneDataPoints()
        {
            //check if every atom is found: //Porphyrin Atoms used here is correct!
            foreach (string id in PorphyrinAtoms)
            {
                if (Atoms.Where(s => s.Identifier == id).Count() == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Atom " + id + " could not be determined!");
                    return null;
                }
            }

            //order atoms
            Atoms = Atoms.OrderBy(s => PorphyrinAtoms.IndexOf(s.Identifier)).ToList();

            //get the 7 distances for corrphycene
            double d1 = CalculateDistance("C1", "C4");
            double d2 = CalculateDistance("C4", "C6");
            double d3 = CalculateDistance("C6", "C9");
            double d4 = CalculateDistance("C9", "C12");
            double d5 = CalculateDistance("C12", "C15");
            double d6 = CalculateDistance("C15", "C17");
            double d7 = CalculateDistance("C17", "C20");

            double[] fixPoints = new double[7];
            foreach (Atom a in Atoms)
            {
                if (a.IsMacrocycle)
                {
                    //defaulting to 1
                    double xCoord = 1;

                    //set x axis
                    if (a.Identifier == "C1")
                    {
                        xCoord = 1;
                        fixPoints[0] = xCoord;
                    }
                    if (a.Identifier == "C2")
                        xCoord = d1 / 3 + fixPoints[0];
                    if (a.Identifier == "N1")
                        xCoord = d1 / 2 + fixPoints[0];
                    if (a.Identifier == "C3")
                        xCoord = (d1 / 3) * 2 + fixPoints[0];
                    if (a.Identifier == "C4")
                    {
                        xCoord = d1 + fixPoints[0];
                        fixPoints[1] = xCoord;
                    }
                    if (a.Identifier == "C5")
                        xCoord = d2 / 2 + fixPoints[1];
                    if (a.Identifier == "C6")
                    {
                        xCoord = d2 + fixPoints[1];
                        fixPoints[2] = xCoord;
                    }
                    if (a.Identifier == "C7")
                        xCoord = d3 / 3 + fixPoints[2];
                    if (a.Identifier == "N2")
                        xCoord = d3 / 2 + fixPoints[2];
                    if (a.Identifier == "C8")
                        xCoord = (d3 / 3) * 2 + fixPoints[2];
                    if (a.Identifier == "C9")
                    {
                        xCoord = d3 + fixPoints[2];
                        fixPoints[3] = xCoord;
                    }
                    if (a.Identifier == "C10")
                        xCoord = d4 / 3 + fixPoints[3];
                    if (a.Identifier == "C11")
                        xCoord = (d4 / 3) * 2 + fixPoints[3];
                    if (a.Identifier == "C12")
                    {
                        xCoord = d4 + fixPoints[3];
                        fixPoints[4] = xCoord;
                    }
                    if (a.Identifier == "C13")
                        xCoord = (d5 / 3) + fixPoints[4];
                    if (a.Identifier == "N3")
                        xCoord = d5 / 2 + fixPoints[4];

                    if (a.Identifier == "C14")
                        xCoord = (d5 / 3) * 2 + fixPoints[4];
                    if (a.Identifier == "C15")
                    {
                        xCoord = d5 + fixPoints[4];
                        fixPoints[5] = xCoord;
                    }
                    if (a.Identifier == "C16")
                        xCoord = d5 / 2 + fixPoints[5];
                    if (a.Identifier == "C17")
                    {
                        xCoord = d5 + fixPoints[5];
                        fixPoints[6] = xCoord;
                    }
                    if (a.Identifier == "C18")
                        xCoord = d7 / 3 + fixPoints[6];
                    if (a.Identifier == "N4")
                        xCoord = d7 / 2 + fixPoints[6];
                    if (a.Identifier == "C19")
                        xCoord = (d7 / 3) * 2 + fixPoints[6];
                    if (a.Identifier == "C20")
                        xCoord = d7 + fixPoints[6];

                    //add data
                    dataPoints.Add(new AtomDataPoint(xCoord, a.DistanceToPlane(GetMeanPlane()), a));
                }
            }
            return dataPoints;
        }


        /// <summary>
        /// Calculates the DataPoints for a porphyrin type cycle.
        /// </summary>
        /// <returns>AtomDataPoints</returns>
        public List<AtomDataPoint> GetPorphyceneDataPoints()
        {
            //check if every atom is found:
            foreach (string id in PorphyrinAtoms)
            {
                if (Atoms.Where(s => s.Identifier == id).Count() == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Atom " + id + " could not be determined!");
                    return null;
                }
            }

            //order atoms
            Atoms = Atoms.OrderBy(s => PorphyrinAtoms.IndexOf(s.Identifier)).ToList();

            //get the x distances for porphyrin
            double d1 = CalculateDistance("C1", "C4");
            double d2 = CalculateDistance("C4", "C7");
            double d3 = CalculateDistance("C7", "C10");
            double d4 = CalculateDistance("C10", "C11");
            double d5 = CalculateDistance("C11", "C14");
            double d6 = CalculateDistance("C14", "C17");
            double d7 = CalculateDistance("C17", "C20");

            double[] fixPoints = new double[7];
            foreach (Atom a in Atoms)
            {
                if (a.IsMacrocycle)
                {
                    //defaulting to 1
                    double xCoord = 1;

                    //set x axis
                    if (a.Identifier == "C1")
                    {
                        xCoord = 1;
                        fixPoints[0] = xCoord;
                    }
                    if (a.Identifier == "C2")
                        xCoord = d1 / 3 + fixPoints[0];
                    if (a.Identifier == "N1")
                        xCoord = d1 / 2 + fixPoints[0];
                    if (a.Identifier == "C3")
                        xCoord = (d1 / 3) * 2 + fixPoints[0];
                    if (a.Identifier == "C4")
                    {
                        xCoord = d1 + fixPoints[0];
                        fixPoints[1] = xCoord;
                    }
                    if (a.Identifier == "C5")
                        xCoord = d2 / 3 + fixPoints[1];
                    if (a.Identifier == "C6")
                        xCoord = (d2 / 3) * 2 + fixPoints[1];
                    if (a.Identifier == "C7")
                    {
                        xCoord = d2 + fixPoints[1];
                        fixPoints[2] = xCoord;
                    }
                    if (a.Identifier == "C8")
                        xCoord = (d3 / 3) + fixPoints[2];
                    if (a.Identifier == "N2")
                        xCoord = d3 / 2 + fixPoints[2];
                    if (a.Identifier == "C9")
                        xCoord = (d3 / 3) * 2 + fixPoints[2];
                    if (a.Identifier == "C10")
                    {
                        xCoord = d3 + fixPoints[2];
                        fixPoints[3] = xCoord;
                    }
                    if (a.Identifier == "C11")
                    {
                        xCoord = d4 + fixPoints[3];
                        fixPoints[4] = xCoord;
                    }
                    if (a.Identifier == "C12")
                        xCoord = d5 / 3 + fixPoints[4];
                    if (a.Identifier == "N3")
                        xCoord = d5 / 2 + fixPoints[4];
                    if (a.Identifier == "C13")
                        xCoord = (d5 / 3) * 2 + fixPoints[4];
                    if (a.Identifier == "C14")
                    {
                        xCoord = d5 + fixPoints[4];
                        fixPoints[5] = xCoord;
                    };
                    if (a.Identifier == "C15")
                        xCoord = d6 / 3 + fixPoints[5];
                    if (a.Identifier == "C16")
                        xCoord = (d6 / 3) * 2 + fixPoints[5];
                    if (a.Identifier == "C17")
                    {
                        xCoord = d6 + fixPoints[5];
                        fixPoints[6] = xCoord;
                    }
                    if (a.Identifier == "C18")
                        xCoord = (d7 / 3) + fixPoints[6];
                    if (a.Identifier == "N4")
                        xCoord = d7 / 2 + fixPoints[6];
                    if (a.Identifier == "C19")
                        xCoord = (d7 / 3) * 2 + fixPoints[6];
                    if (a.Identifier == "C20")
                        xCoord = d7 + fixPoints[6];



                    //add data
                    dataPoints.Add(new AtomDataPoint(xCoord, a.DistanceToPlane(GetMeanPlane()), a));
                }
            }
            return dataPoints;
        }


        /// <summary>
        /// Return the cycle's datapoints
        /// </summary>
        /// <returns>AtomDataPoints</returns>
        public List<AtomDataPoint> GetDataPoints()
        {
            if (this.type == Macrocycle.Type.Corrole)
            {
                dataPoints = GetCorroleDataPoints();
            }
            else if (this.type == Macrocycle.Type.Porphyrin)
            {
                dataPoints = GetPorphyrinDataPoints();
            }
            else if (this.type == Macrocycle.Type.Norcorrole)
            {
                dataPoints = GetNorcorroleDataPoints();
            }
            else if (this.type == Macrocycle.Type.Corrphycene)
            {
                dataPoints = GetCorrphyceneDataPoints();
            }
            else if (this.type == Macrocycle.Type.Porphycene)
            {
                dataPoints = GetPorphyceneDataPoints();
            }
            return dataPoints;
        }

        /// <summary>
        /// Draws a line between two points
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="sim">is Simulation</param>
        /// <returns>ArrowAnnotation aka Bond</returns>
        public static ArrowAnnotation DrawBond(AtomDataPoint a1, AtomDataPoint a2, int mode = 0)
        {
            OxyColor color;
            if (Properties.Settings.Default.singleColor)
            {
                color = Atom.modesSingleColor[mode];
            }
            else
            {
                color = Atom.modesMultiColor[mode];
            }

            ArrowAnnotation bond = new ArrowAnnotation
            {
                StartPoint = a1.GetDataPoint(),
                EndPoint = a2.GetDataPoint(),
                HeadWidth = 0,
                HeadLength = 0,
                Color = color,
                Layer = AnnotationLayer.BelowSeries,
                StrokeThickness = Properties.Settings.Default.lineThickness
            };
            return bond;
        }

        /// <summary>
        /// Generates all Bonds as Annotations
        /// </summary>
        /// <param name="sim">is Simulation</param>
        /// <returns>Annotation aka Bonds</returns>
        public List<ArrowAnnotation> DrawBonds(int mode = 0)
        {
            dataPoints = dataPoints.OrderBy(s => s.X).ToList();

            List<ArrowAnnotation> bonds = new List<ArrowAnnotation>();
            List<Tuple<string, string>> Bonds = PorphyrinBonds;
            if (type == Type.Corrole)
            {
                Bonds = CorroleBonds;
            }
            else if (type == Type.Norcorrole)
            {
                Bonds = NorcorroleBonds;
            }
            else if (type == Type.Corrphycene)
            {
                Bonds = CorrphyceneBonds;
            }
            else if (type == Type.Porphycene)
            {
                Bonds = PorphyceneBonds;
            }
            foreach (Tuple<string, string> t in Bonds)
            {
                try
                {
                    AtomDataPoint a1;
                    AtomDataPoint a2;
                    if (t.Item1 == "C20" && t.Item2 == "C1" && type == Type.Porphyrin)
                    {
                        a1 = dataPoints.OrderBy(s => s.X).First();
                        a2 = dataPoints.Where(s => s.atom.Identifier == t.Item2 && s.atom.IsMacrocycle).First();
                    }
                    else if (t.Item1 == "C19" && t.Item2 == "C20" && type == Type.Porphyrin)
                    {
                        a1 = dataPoints.Where(s => s.atom.Identifier == t.Item1 && s.atom.IsMacrocycle).First();
                        a2 = dataPoints.OrderBy(s => s.X).Last();
                    }
                    else
                    {
                        a1 = dataPoints.Where(s => s.atom.Identifier == t.Item1 && s.atom.IsMacrocycle).First();
                        a2 = dataPoints.Where(s => s.atom.Identifier == t.Item2 && s.atom.IsMacrocycle).First();
                    }
                    bonds.Add(Macrocycle.DrawBond(a1, a2, mode));
                }
                catch { }
            }

            return bonds;
        }

        /// <summary>
        /// Get Atom by Identifier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="forceMacroCycle"></param>
        /// <returns>Atom</returns>
        public Atom ByIdentifier(string id, bool forceMacroCycle = false)
        {
            return Atoms.Where(s => s.Identifier == id && s.IsMacrocycle == forceMacroCycle).First();
        }

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

            xR.AddRange(0, 0.1, Atom.OxyAtomColor["C"]);
            //sim
            xR.AddRange(1000, 1000.1, OxyColor.FromAColor(75, Atom.OxyAtomColor["C"]));
            //diff
            xR.AddRange(2000, 2000.1, OxyColor.FromAColor(50, Atom.OxyAtomColor["C"]));

            //comparisons
            xR.AddRange(3000, 3000.1, OxyColor.FromAColor(75, Atom.OxyAtomColor["C"]));
            xR.AddRange(4000, 4000.1, OxyColor.FromAColor(75, Atom.OxyAtomColor["C"]));

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
        public double MeanDisplacement()
        {
            double sum = 0;
            foreach (AtomDataPoint dp in dataPoints)
            {
                sum += Math.Abs(dp.Y);
            }
            return sum / dataPoints.Count;
        }
    }
}

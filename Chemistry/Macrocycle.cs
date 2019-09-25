using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            return new Macrocycle(v.Atoms) { Title = v.Title };
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
                if (atom.IsMacrocycle && !atom.IsMetal) points.Add(new Point3D(atom.X, atom.Y, atom.Z));
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
                if (atom.IsMacrocycle && !atom.IsMetal) points.Add(new Point3D(atom.X, atom.Y, atom.Z));
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
            return Atom.Distance(this.Atoms.Where(s => s.Identifier == id1 && s.IsMacrocycle).First(), this.Atoms.Where(s => s.Identifier == id2 && s.IsMacrocycle).First());
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
                if (a.IsMacrocycle && !a.IsMetal)
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
                if (a.IsMacrocycle && !a.IsMetal)
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
                if (a.IsMacrocycle && !a.IsMetal)
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
                if (a.IsMacrocycle && !a.IsMetal)
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
                if (a.IsMacrocycle && !a.IsMetal)
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

            if (HasMetal)
            {
                Atom M = GetMetal();
                dataPoints.Add(new AtomDataPoint(
                    (dataPoints.First().X + dataPoints.Last().X) / 2,
                    M.DistanceToPlane(GetMeanPlane()),
                    M));
            }

            return dataPoints;
        }


        /// <summary>
        /// Indicates if a Metal Atom is present
        /// </summary>
        public bool HasMetal
        {
            get
            {
                bool check = false;
                foreach (Atom a in Atoms)
                {
                    //only return metal if it's marked as macrocycle even if the option is set to on!
                    if (a.IsMetal && a.IsMacrocycle) check = true;
                }
                return check;
            }
        }

        /// <summary>
        /// Gets the first detected metal atom
        /// </summary>
        /// <returns></returns>
        public Atom GetMetal()
        {
            Atom m = null;
            foreach (Atom a in Atoms)
            {
                if (a.IsMetal) m = a;
                if (a.Identifier == "M") return a; //if identifier is set return!
            }
            return m;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Ausstehend>")]
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

            //add metal atoms
            if (HasMetal)
            {
                List<AtomDataPoint> Nitrogen = dataPoints.Where(s => Regex.IsMatch(s.atom.Identifier, "N[1-4]")).ToList();
                AtomDataPoint m = dataPoints.Where(s => s.atom == GetMetal()).FirstOrDefault();
                foreach (AtomDataPoint n in Nitrogen)
                {
                    try
                    {
                        ArrowAnnotation b;
                        b = DrawBond(m, n);
                        b.LineStyle = LineStyle.Dash;
                        b.Color = OxyColor.FromAColor(75, b.Color);
                        b.Tag = "Metal";
                        bonds.Add(b);
                    }
                    catch { /*don't throw if metal is omitted */}
                }
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
        public double MeanDisplacement()
        {
            double sum = 0;
            foreach (AtomDataPoint dp in dataPoints)
            {
                sum += Math.Pow(dp.Y, 2);
            }
            return Math.Sqrt(sum);
        }

        public List<Atom> Neighbors(Atom A)
        {
            List<Atom> result = new List<Atom>();
            foreach (Atom B in Atoms)
            {
                if (A.BondTo(B) && A != B)
                {
                    result.Add(B);
                }
            }
            return result;
        }

        /// <summary>
        /// Detect the macrocyclic structure
        /// </summary>
        /// <returns></returns>
        public List<Atom> Detect()
        {
            List<List<Atom>> Pyrroles = new List<List<Atom>>();
            for (int i = 0; i < Atoms.Count; i++)
            {
                if (Atoms[i].Type == "N")
                {
                    //is n atom
                    List<Atom> _n = Neighbors(Atoms[i]).Where(s => s.Type == "C").ToList();
                    //n mostly be pyrrolic, so all neighbors be pyrrolic due to beeing alpha carbons
                    List<Atom> Pyrrole = _n.Where(s => s.Type == "C").ToList();
                    Pyrrole.Add(Atoms[i]);
                    foreach (Atom __n in _n)
                    {
                        List<Atom> _nn = Neighbors(__n).Where(s => s.Type == "C").ToList();
                        foreach (Atom __nn in _nn)
                        {
                            //get new neighbors (for c2 its c3 and c19/20
                            List<Atom> _nnn = Neighbors(__nn).Where(s => !Pyrrole.Contains(s)).ToList();
                            //find the beta atoms in _nn
                            foreach (Atom __nnn in _nnn)
                            {
                                if (Neighbors(__nnn).Where(s => Pyrrole.Contains(s)).Count() != 0)
                                {
                                    //found a atom thats in the list so __nnn and __nn are beta
                                    if (Pyrrole.Distinct().Count() != 5)
                                    {
                                        Pyrrole.Add(__nnn);
                                        Pyrrole.Add(__nn);
                                    }
                                }
                            }
                        }
                    }
                    if (Pyrrole.Distinct().Count() == 5)
                        Pyrroles.Add(Pyrrole.Distinct().ToList());
                }
            }
            List<Atom> meso = new List<Atom>();
            //find atoms connection pyrroles
            for (int i = 0; i < Pyrroles.Count; i++)
            {
                for (int j = i + 1; j < Pyrroles.Count; j++)
                {
                    foreach (Atom a in Atoms)
                    {
                        //if atom has neighbors in both lists, it's meso (unless its a porphycene/corrphycene)
                        if (Pyrroles[i].Intersect(Neighbors(a)).Count() != 0
                            && Pyrroles[j].Intersect(Neighbors(a)).Count() != 0
                            && i != j
                            && !a.IsMetal)
                        {
                            meso.Add(a);
                        }
                    }
                }
            }

            //merge
            List<Atom> macrocycle = new List<Atom>();
            foreach (List<Atom> py in Pyrroles)
            {
                macrocycle.AddRange(py);
            }
            macrocycle.AddRange(meso);
            macrocycle = macrocycle.Distinct().ToList();


            foreach (Atom a in Atoms)
            {
                if (macrocycle.Contains(a))
                {
                    a.IsMacrocycle = true;
                    a.Identifier = a.Type + "M";
                }
                else
                {
                    a.IsMacrocycle = false;
                    a.Identifier = "X" + a.Type;
                }
            }

            Atom start = null;
            if (type != Type.Corrole)
            {
                start = Atoms.Where(s => s.IsMacrocycle && Neighbors(s).Where(l => l.Element.Symbol == "N").Count() != 0).FirstOrDefault();
            }
            else
            {
                List<Atom> alpha = Atoms.Where(s => s.IsMacrocycle && Neighbors(s).Where(l => l.Element.Symbol == "N").Count() != 0).ToList();
                foreach(Atom a in alpha)
                {
                    if (Neighbors(a).Where(l => alpha.Contains(l)).Count() != 0) start = a;
                }
            }

            //loop over C atoms
            int indexC = 1;
            Atom current = start;
            start.Identifier = "C" + indexC; //this is c1
            indexC++;
            bool cycling = true;
            //loop over cycle...
            while (cycling)
            {
                List<Atom> X = Neighbors(current).Where(s => s.Type != "N" && s.IsMacrocycle && s.Identifier != "C" + (indexC - 2)).ToList();
                if(indexC == 2)
                {
                    //go to beta!
                    //currently current is alpha c1, so it will have a beta as neighbor which is not bond to nitrogen
                    //but meso also is not bond to nitrogen and is neighbor
                    foreach(Atom t in X)
                    {
                        var neighT = Neighbors(t).Where(s => s.IsMacrocycle);
                        if (neighT.Where(s => s.Type == "N").Count() == 0)
                        {
                            int count = 0;
                            //either is meso or beta...
                            //beta has another beta and alpha as neighbor, meso has two alpha as neighbors.
                            //so if neighbors of neighbors of beta has a single Count(N) = 1
                            //this is start
                            foreach(Atom u in neighT)
                            {
                                if (Neighbors(u).Where(s => s.Type == "N").Count() != 0) count++;
                            }
                            if (count == 1) current = t;
                        }
                    }
                }
                else
                    current = X.FirstOrDefault();

                if (current == null) cycling = false; //cancel hard
                else if (current == start)
                {
                    cycling = false;
                    break;
                }
                else current.Identifier = "C" + indexC;
                indexC++;
            }

            //assign N
            foreach(Atom n in Atoms.Where(s => s.Type == "N" && s.IsMacrocycle))
            {
                if (Neighbors(n).Where(l => l.Identifier == "C1").Count() == 1) n.Identifier = "N1";
                if (Neighbors(n).Where(l => l.Identifier == "C6").Count() == 1) n.Identifier = "N2";
                if (Neighbors(n).Where(l => l.Identifier == "C11").Count() == 1) n.Identifier = "N3";
                if (Neighbors(n).Where(l => l.Identifier == "C16").Count() == 1) n.Identifier = "N4";
            }
            return Atoms;
        }
    }
}

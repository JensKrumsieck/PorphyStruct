using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Chemistry
{
    public class Molecule
    {
        public string Title = "";
        //public List<Atom> Atoms = new List<Atom>();
        public List<Atom> Atoms = new List<Atom>();

        /// <summary>
        /// Construct Molecule
        /// </summary>
        public Molecule() { }

        /// <summary>
        /// Construct Molecule with title
        /// <seealso cref="Molecule"/>
        /// </summary>
        /// <param name="title"></param>
        public Molecule(string title)
        {
            this.Title = title;
        }

        /// <summary>
        /// Construct Molecule with AtomList
        /// <seealso cref="Molecule"/>
        /// </summary>
        /// <param name="title"></param>
        public Molecule(List<Atom> Atoms)
            : this()
        {
            this.Atoms = Atoms;
        }

        /// <summary>
        /// Set Is Macrocycle for Atoms
        /// </summary>
        /// <param name="type"></param>
        public void SetIsMacrocycle(Macrocycle.Type type)
        {
            bool idiot = false;
            if (Atoms.Where(s => s.Identifier.StartsWith("N0")).Count() != 0) idiot = true; //haha

            foreach (Atom atom in Atoms)
            {
                //set everything to false first
                atom.IsMacrocycle = false;

                //increment atom id for those who start counting at 0
                if (idiot && (atom.Type == "C" || atom.Type == "N")) atom.Identifier = atom.Type + atom.ID + 1;

                //adjust core to N1->N4
                if (atom.Type == "N")
                {   
                    if (atom.ID >= 21 && atom.ID <= 24)
                    atom.Identifier = "N" + (atom.ID - 20) + atom.Suffix;
                }

                //remove suffix if suffix is A (primary). Maybe suffix selection in future
                if (atom.Suffix == "A" || atom.Suffix == "a") atom.Identifier = atom.Type + atom.ID;

                //c between 0 and 20 ; n between 1 and 4
                if (((atom.Type == "C" && atom.ID <= 20) || (atom.Type == "N" && atom.ID <= 4)) && string.IsNullOrEmpty(atom.Suffix)) atom.IsMacrocycle = true;
                //corroles and norcorroles missing 1 atom
                if ((type == Macrocycle.Type.Corrole || type == Macrocycle.Type.Norcorrole) && atom.ID >= 20) atom.IsMacrocycle = false;
            }
        }

        /// <summary>
        /// Gets the centroid of a list of atoms
        /// </summary>
        /// <param name="atoms"></param>
        /// <returns></returns>
        public static Vector3D GetCentroid(IEnumerable<Atom> atoms) => Point3D.Centroid(atoms.ToPoint3D()).ToVector3D();

        /// <summary>
        /// Gets the mean plane of a list of atoms
        /// </summary>
        /// <returns>The Plane Object (Math.Net)</returns>
        public Plane GetMeanPlane(IEnumerable<Atom> atoms)
        {
            //convert coordinates into Point3D because centroid method is only available in math net spatial
            List<Point3D> points = atoms.ToPoint3D().ToList();
            //calculate Centroid first
            //get the centroid
            Vector3D centroid = GetCentroid(atoms);

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
        /// Return Neighbors of atom in list
        /// </summary>
        /// <param name="A"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Atom> Neighbors(Atom A, IEnumerable<Atom> list) => list.Where(B => A.BondTo(B) && A != B);

        /// <summary>
        /// Returns non metla neighbors of atom
        /// </summary>
        /// <param name="A"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Atom> NonMetalNeighbors(Atom A, IEnumerable<Atom> list) => Neighbors(A, list).Where(s => !s.IsMetal);


        /// <summary>
        /// Returns a collection of Atoms with VertexDegree of 3
        /// NOTE: If this is used on al found cycle by Macrocyle.Detect() this will return alpha atoms
        /// </summary>
        /// <param name="mol"></param>
        /// <returns></returns>
        public static IEnumerable<Atom> Vertex3Atoms(IEnumerable<Atom> mol) => mol.Where(s => DFSUtil.VertexDegree(mol, s, Neighbors) == 3);


        /// <summary>
        /// Gets the first detected metal atom in molecule
        /// </summary>
        /// <returns></returns>
        public Atom GetMetal() => Atoms.Where(s => s.IsMetal).FirstOrDefault();        
    }
}

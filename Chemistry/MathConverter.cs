using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PorphyStruct.Chemistry
{
    public static class MathConverter
    {
        /// <summary>
        /// Converts the xyz into Point3D because some methods need math net spatial...
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Point3D> ToPoint3D(this IEnumerable<Atom> Atoms)
        {
            foreach (Atom atom in Atoms)
            {
                yield return new Point3D(atom.X, atom.Y, atom.Z);
            }
        }

    }
}

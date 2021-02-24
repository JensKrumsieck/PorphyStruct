using System.Numerics;
using System.Windows.Media.Media3D;

namespace PorphyStruct.ViewModel.Windows
{
    public static class Util3D
    {
        public static Point3D ToPoint3D(this Vector3 v) => new Point3D(v.X, v.Y, v.Z);
    }
}

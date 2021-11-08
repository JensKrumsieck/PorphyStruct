﻿using System.Numerics;
using System.Windows.Media.Media3D;

namespace PorphyStruct.ViewModel.Windows.Visual
{
    public static class Util3D
    {
        public static Point3D ToPoint3D(this Vector3 v) => new(v.X, v.Y, v.Z);
    }
}

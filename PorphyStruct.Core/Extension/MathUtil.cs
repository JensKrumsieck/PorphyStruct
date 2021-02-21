﻿using ChemSharp.Mathematics;
using PorphyStruct.Core.Plot;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Core.Extension
{
    public static class MathUtil
    {
        /// <summary>
        /// Displacement Value aka Vector euclidean norm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double DisplacementValue(this IEnumerable<AtomDataPoint> data) => data.Select(s => s.Y).Length();
    }
}
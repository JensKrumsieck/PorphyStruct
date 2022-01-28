using ChemSharp.Mathematics;
using ChemSharp.Molecules;
using PorphyStruct.Core.Plot;

namespace PorphyStruct.Core.Extension;

public static class MathUtil
{
    /// <summary>
    /// Displacement Value aka Vector euclidean norm
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static double DisplacementValue(this IEnumerable<AtomDataPoint> data) => data.Select(s => s.Y).Length();

    /// <summary>
    /// Calculates area of triangle using heron's formula
    /// </summary>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <param name="a3"></param>
    /// <returns></returns>
    public static double Heron(Atom a1, Atom a2, Atom a3)
    {
        var a = a1.DistanceTo(a2);
        var b = a2.DistanceTo(a3);
        var c = a3.DistanceTo(a1);
        var sqrt = (a + b + c) * (-a + b + c) * (a - b + c) * (a + b - c);
        return 1f / 4f * MathF.Sqrt(sqrt);
    }
}

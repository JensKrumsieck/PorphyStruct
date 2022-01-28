using System.Numerics;
using ChemSharp.Mathematics;
using ChemSharp.Molecules;
using ChemSharp.Molecules.Properties;

namespace PorphyStruct.Core.Analysis.Properties;

public class PlaneDistance : KeyValueProperty
{
    public override double Value => MathV.Distance(Plane, Atom.Location);
    public override string Key => $"{Atom.Title} - {(!string.IsNullOrEmpty(PlaneTitle) ? PlaneTitle : Plane.ToString())}";
    public override string Unit => "Å";

    public Atom Atom;
    public Plane Plane;
    public string PlaneTitle;

    public PlaneDistance(Atom a, Plane plane, string planeTitle = "")
    {
        Atom = a;
        Plane = plane;
        PlaneTitle = planeTitle;
    }
}

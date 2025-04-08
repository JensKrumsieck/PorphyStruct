using ChemSharp.Molecules;

namespace PorphyStruct.Core.Extension;

public static class ElementUtil
{
    public static bool CanBeRingMember(this Element e) => e.IsNonMetal || e.IsMetalloid || e.Symbol == "Pt";

    public static bool CanBeCenterAtom(this Element e) => e.IsMetal || e.IsMetalloid || e.Symbol == "P";
}


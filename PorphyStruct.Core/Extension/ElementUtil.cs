using ChemSharp.Molecules;

namespace PorphyStruct.Core.Extension;

public static class ElementUtil
{
    public static bool IsNonCoordinative(this Element e) =>
        e.Symbol switch
        {
            "P" when !Settings.Instance.HandlePhosphorusMetal => true,
            "P" when Settings.Instance.HandlePhosphorusMetal => false,
            "B" when !Settings.Instance.HandleBoronMetal => true,
            "B" when Settings.Instance.HandleBoronMetal => false,
            "Si" when !Settings.Instance.HandleSiliconMetal => true,
            "Si" when Settings.Instance.HandleSiliconMetal => false,
            _ => e.IsNonMetal
        };
}

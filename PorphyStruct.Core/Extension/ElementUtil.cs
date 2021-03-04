using ChemSharp.Molecules;

namespace PorphyStruct.Core.Extension
{
    public static class ElementUtil
    {
        public static bool IsNonCoordinative(this Element e) =>
            e.Symbol switch
            {
                "P" when !Settings.Instance.HandlePhosphorusMetal => true,
                "B" when !Settings.Instance.HandleBoronMetal => true,
                "Si" when !Settings.Instance.HandleSiliconMetal => true,
                _ => e.IsNonMetal
            };
    }
}

using ChemSharp.Molecules;

namespace PorphyStruct.Core.Extension
{
    public static class ElementUtil
    {
        public static bool IsNonCoordinative(this Element e)
        {
            return e.Symbol == "P" && !Settings.Instance.HandlePhosphorusMetal || e.IsNonMetal && e.Symbol != "P";
        }
    }
}

using ChemSharp.Molecules;

namespace PorphyStruct.Core;

public static class Constants
{
    public static readonly List<string> DeadEnds = Element.DesiredSaturation.Where(s => s.Value == 1).Select(s => s.Key).ToList();

    public static string SettingsFolder =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PorphyStruct");
    public static string SettingsLocation =>
        Path.Combine(SettingsFolder, "settings.json");
}

using System.Reflection;

namespace PorphyStruct.Core.Extension;

public static class ResourceUtil
{
    /// <summary>
    /// Loads ResourceStream
    /// </summary>
    /// <param name="resourceName"></param>
    /// <returns></returns>
    public static Stream? LoadResource(string resourceName) => Assembly.GetAssembly(typeof(Constants))
        ?.GetManifestResourceStream(resourceName);

    /// <summary>
    /// Reads a String from Resource
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadResourceString(string path)
    {
        StreamReader sr;
        if (path.Contains("PorphyStruct")) //resource loading
        {
            var stream = LoadResource(path);
            if (stream == null) throw new FileNotFoundException($"Could not find Resource {path}!");
            sr = new StreamReader(stream);
        }
        else sr = new StreamReader(path);
        var data = sr.ReadToEnd();
        sr.Close();
        return data;
    }
}

using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

namespace PorphyStruct.WPF;

public class Updater
{
    public string Latest = "0.0.0";


    /// <summary>
    /// Returns Instance asynchronously
    /// </summary>
    /// <returns></returns>
    public static async Task<(Updater, bool)> CreateAsync()
    {
        var updater = new Updater();
        return (updater, await updater.CheckVersion());
    }

    /// <summary>
    /// Checks if Version is current
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CheckVersion()
    {
        const string baseAddr = "https://api.github.com";
        const string url = "repos/jenskrumsieck/porphystruct/releases/latest";
        var version = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
        .InformationalVersion!;
        var client = new HttpClient
        {
            BaseAddress = new Uri(baseAddr),
            Timeout = TimeSpan.FromSeconds(1)
        };

        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PorphyStruct", version));
        try
        {
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var res = await JsonSerializer.DeserializeAsync
                    <Dictionary<string, object>>(responseStream);
                if (res == null | res?["tag_name"] == null) return true;
                Latest = res!["tag_name"].ToString()![1..];
                var latest = Version.Parse(Latest);
                var current = Version.Parse(version);
                return current >= latest;
            }
        }
        catch (Exception) { return true; }//no internet => ignore!
                                          //return true if no response could be made
        return true;
    }
}

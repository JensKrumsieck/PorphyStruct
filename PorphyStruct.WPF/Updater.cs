using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace PorphyStruct.WPF
{
    public class Updater
    {
        public string Latest = "v0.0.0";

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
                Timeout = new TimeSpan(0, 0, 1)
            };
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PorphyStruct", version));
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var res = await JsonSerializer.DeserializeAsync
                    <Dictionary<string, object>>(responseStream);
                Latest = res["tag_name"].ToString();
                return Latest == $"v{version}";
            }
            //return true if no response could be made
            return true;
        }

        public void DownloadLatest()
        {
            var client = new WebClient();
            client.DownloadFileAsync(new Uri($"https://github.com/JensKrumsieck/PorphyStruct/releases/download/{Latest}/PorphyStruct.exe"), Core.Constants.SettingsFolder + "/PorphyStruct.exe");
        }
    }
}

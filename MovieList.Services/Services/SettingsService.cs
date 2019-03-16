using System;
using System.Drawing;
using System.IO;

using Newtonsoft.Json;

namespace MovieList.Services
{
    public class SettingsService
    {
        public static readonly string SettingsFile = "settings.json";

        public Settings Settings { get; private set; } = new Settings(String.Empty, Color.Black, Color.Black);

        public void ReadSettings()
        {
            using var file = new StreamReader(SettingsFile);
            this.Settings = JsonConvert.DeserializeObject<Settings>(file.ReadToEnd());
        }

        public void WriteSettings()
        {
            using var file = new StreamWriter(SettingsFile);
            file.Write(JsonConvert.SerializeObject(this.Settings, Formatting.Indented));
        }
    }
}

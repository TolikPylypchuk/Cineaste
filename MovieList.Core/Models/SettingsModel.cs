using System.Collections.Generic;

using MovieList.Data;
using MovieList.Data.Models;

namespace MovieList.Models
{
    public class SettingsModel
    {
        public SettingsModel(Settings settings, List<Kind> kinds)
        {
            this.Settings = settings;
            this.Kinds = kinds;
        }

        public Settings Settings { get; }
        public List<Kind> Kinds { get; }
    }
}

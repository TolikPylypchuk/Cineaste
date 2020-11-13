using System.Collections.Generic;
using System.Linq;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

using Splat;

using static MovieList.Data.Constants;

namespace MovieList.Data.Services.Implementations
{
    internal class SettingsService : ServiceBase, ISettingsService
    {
        public SettingsService(string file)
            : base(file)
        { }

        public Settings GetSettings()
        {
            this.Log().Debug("Getting all settings");
            return this.WithTransaction((connection, transaction) =>
                Settings.FromDictionary(connection.GetAll<Setting>().ToDictionary(s => s.Key, s => s.Value)));
        }

        public void UpdateSettings(Settings settings) =>
            this.WithTransaction((connection, transaction) =>
            {
                this.Log().Debug("Saving settings");

                var dbSettings = connection.GetAll<Setting>(transaction);

                var settingsDictionary = this.ToDictionary(settings);

                foreach (var setting in dbSettings)
                {
                    if (settingsDictionary.ContainsKey(setting.Key) &&
                        settingsDictionary[setting.Key] != setting.Value)
                    {
                        setting.Value = settingsDictionary[setting.Key];
                        connection.Update(setting, transaction);
                    }
                }
            });

        private Dictionary<string, string> ToDictionary(Settings settings) =>
            new Dictionary<string, string>
            {
                [SettingsListNameKey] = settings.ListName,
                [SettingsListVersionKey] = settings.ListVersion.ToString(),
                [SettingsDefaultSeasonTitleKey] = settings.DefaultSeasonTitle,
                [SettingsDefaultSeasonOriginalTitleKey] = settings.DefaultSeasonOriginalTitle,
                [SettingsListCultureKey] = settings.CultureInfo.ToString()
            };
    }
}

using System.Linq;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

using Splat;

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

                var settingsDictionary = settings.ToDictionary();

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
    }
}

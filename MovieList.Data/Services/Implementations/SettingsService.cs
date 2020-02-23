using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<Settings> GetSettingsAsync()
        {
            this.Log().Debug("Getting all settings.");

            return await this.WithTransactionAsync(async (connection, transaction) =>
            {
                var settings = (await connection.GetAllAsync<Setting>()).ToDictionary(s => s.Key, s => s.Value);

                return new Settings(
                    settings[SettingsListNameKey],
                    Int32.Parse(settings[SettingsListVersionKey]),
                    settings[SettingsDefaultSeasonTitleKey],
                    settings[SettingsDefaultSeasonOriginalTitleKey],
                    settings[SettingsListCultureKey]);
            });
        }

        public async Task UpdateSettingsAsync(Settings settings)
        {
            this.Log().Debug("Saving settings.");

            await this.WithTransactionAsync(async (connection, transaction) =>
            {
                var dbSettings = await connection.GetAllAsync<Setting>(transaction);

                var settingsDictionary = this.ToDictionary(settings);

                foreach (var setting in dbSettings)
                {
                    if (settingsDictionary.ContainsKey(setting.Key) &&
                        settingsDictionary[setting.Key] != setting.Value)
                    {
                        setting.Value = settingsDictionary[setting.Key];
                        await connection.UpdateAsync(setting, transaction);
                    }
                }
            });
        }

        private Dictionary<string, string> ToDictionary(Settings settings)
            => new Dictionary<string, string>
            {
                [SettingsListNameKey] = settings.ListName,
                [SettingsListVersionKey] = settings.ListVersion.ToString(),
                [SettingsDefaultSeasonTitleKey] = settings.DefaultSeasonTitle,
                [SettingsDefaultSeasonOriginalTitleKey] = settings.DefaultSeasonOriginalTitle
            };
    }
}

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

        [LogException]
        public async Task<Settings> GetSettingsAsync()
        {
            this.Log().Debug("Getting all settings.");

            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            var settings = (await connection.GetAllAsync<Setting>()).ToDictionary(s => s.Key, s => s.Value);

            return new Settings(
                settings[SettingsListNameKey],
                Int32.Parse(settings[SettingsListVersionKey]),
                settings[SettingsDefaultSeasonTitleKey],
                settings[SettingsDefaultSeasonOriginalTitleKey]);
        }

        [LogException]
        public async Task UpdateSettingsAsync(Settings settings)
        {
            this.Log().Debug("Saving settings.");

            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            var dbSettings = await connection.GetAllAsync<Setting>(transaction);

            var settingsDictionary = this.ToDictionary(settings);

            foreach (var setting in dbSettings)
            {
                if (settingsDictionary.ContainsKey(setting.Key) && settingsDictionary[setting.Key] != setting.Value)
                {
                    setting.Value = settingsDictionary[setting.Key];
                    await connection.UpdateAsync(setting, transaction);
                }
            }

            await transaction.CommitAsync();
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

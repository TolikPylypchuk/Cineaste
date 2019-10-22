using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [LogException]
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            this.Log().Debug("Getting all settings.");

            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            var settings = await connection.GetAllAsync<Settings>();

            return settings.ToDictionary(s => s.Key, s => s.Value);
        }

        [LogException]
        public async Task UpdateSettingsAsync(IDictionary<string, string> settings)
        {
            this.Log().Debug("Saving settings.");

            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            foreach (var setting in await connection.GetAllAsync<Settings>())
            {
                if (settings.ContainsKey(setting.Key) && settings[setting.Key] != setting.Value)
                {
                    setting.Value = settings[setting.Key];
                    await connection.UpdateAsync(setting, transaction);
                }
            }

            await transaction.CommitAsync();
        }
    }
}

using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.Sqlite;

using MovieList.Data.Models;

using Resourcer;

using Splat;

using static MovieList.Data.Constants;

namespace MovieList.Data.Services.Implementations
{
    internal class DatabaseService : ServiceBase, IDatabaseService
    {
        private const string SchemaSql = "../../schema.sql";
        private const string SqliteFileHeader = "SQLite format 3";

        public DatabaseService(string file)
            : base(file)
        { }

        [SuppressMessage(
            "Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "SQL comes from a database creation script")]
        public async Task CreateDatabaseAsync(Settings settings, IEnumerable<Kind> initialKinds)
        {
            this.Log().Debug($"Creating a new database: {this.DatabasePath}.");

            if (File.Exists(this.DatabasePath))
            {
                this.Log().Warn($"{this.DatabasePath} already exists.");
                return;
            }
            
            string sql = Resource.AsString(SchemaSql);

            await this.WithTransactionAsync(async (connection ,transaction) =>
            {
                await connection.ExecuteAsync(sql, transaction);
                await this.InitSettingsAsync(connection, transaction, settings);
                await connection.InsertAsync(initialKinds, transaction);
            });
        }

        public async Task<bool> ValidateDatabaseAsync()
        {
            this.Log().Debug($"Validating the database: {this.DatabasePath}.");

            bool isSqliteFile = await this.CheckIfSqliteDatabaseAsync();

            if (!isSqliteFile)
            {
                return false;
            }

            this.Log().Debug($"Checking the database schema: {this.DatabasePath}.");

            return true;
        }

        private async Task InitSettingsAsync(SqliteConnection connection, IDbTransaction transaction, Settings settings)
        {
            this.Log().Debug($"Initializing settings for the database: {this.DatabasePath}.");

            var settingsList = new List<Setting>
            {
                new Setting
                {
                    Key = SettingsListNameKey,
                    Value = settings.ListName
                },
                new Setting
                {
                    Key = SettingsListVersionKey,
                    Value = settings.ListVersion.ToString()
                },
                new Setting
                {
                    Key = SettingsDefaultSeasonTitleKey,
                    Value = settings.DefaultSeasonTitle
                },
                new Setting
                {
                    Key = SettingsDefaultSeasonOriginalTitleKey,
                    Value = settings.DefaultSeasonOriginalTitle
                }
            };

            foreach (var setting in settingsList)
            {
                await connection.InsertAsync(setting, transaction);
            }
        }

        private async Task<bool> CheckIfSqliteDatabaseAsync()
        {
            this.Log().Debug($"Checking if the file is an SQLite database: {this.DatabasePath}.");

            if (!File.Exists(this.DatabasePath))
            {
                this.Log().Error($"{this.DatabasePath} doesn't exist.");
                return false;
            }

            const int headerSize = 16;

            await using var stream = new FileStream(
                this.DatabasePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var header = new byte[headerSize];
            await stream.ReadAsync(header, 0, headerSize);

            bool isSqliteFile = Encoding.UTF8.GetString(header).Contains(SqliteFileHeader);

            if (!isSqliteFile)
            {
                this.Log().Error($"{this.DatabasePath} is not an SQLite file.");
            }

            return isSqliteFile;
        }
    }
}

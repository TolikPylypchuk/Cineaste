using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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

        [LogException]
        [SuppressMessage(
            "Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "SQL comes from a database creation script")]
        public async Task CreateDatabaseAsync()
        {
            this.Log().Debug($"Creating a new database: {this.DatabasePath}.");

            if (File.Exists(this.DatabasePath))
            {
                this.Log().Warn($"{this.DatabasePath} already exists.");
                return;
            }
            
            string sql = Resource.AsString(SchemaSql);

            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql);

            await this.InitSettingsAsync(connection);
        }

        [LogException]
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

        private async Task InitSettingsAsync(SqliteConnection connection)
        {
            this.Log().Debug($"Initializing settings for the database: {this.DatabasePath}.");

            await using var transaction = await connection.BeginTransactionAsync();
            string fileName = Path.GetFileNameWithoutExtension(this.DatabasePath);

            var settings = new List<Settings>
            {
                new Settings
                {
                    Key = SettingsListNameKey,
                    Value = fileName
                },
                new Settings
                {
                    Key = SettingsListVersionKey,
                    Value = "1"
                },
                new Settings
                {
                    Key = SettingsColorForNotWatchedKey,
                    Value = SettingsColorForNotWatchedValue
                },
                new Settings
                {
                    Key = SettingsColorForNotReleasedKey,
                    Value = SettingsColorForNotReleasedValue
                },
                new Settings
                {
                    Key = SettingsDefaultSeasonTitleKey,
                    Value = SettingsDefaultSeasonTitleValue
                },
                new Settings
                {
                    Key = SettingsDefaultSeasonOriginalTitleKey,
                    Value = SettingsDefaultSeasonOriginalTitleValue
                }
            };

            foreach (var setting in settings)
            {
                await connection.InsertAsync(setting, transaction);
            }

            await transaction.CommitAsync();
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

            await using var stream = new FileStream(this.DatabasePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var header = new byte[headerSize];
            await stream.ReadAsync(header, 0, headerSize);

            bool isSqliteFile = System.Text.Encoding.UTF8.GetString(header).Contains(SqliteFileHeader);

            if (!isSqliteFile)
            {
                this.Log().Error($"{this.DatabasePath} is not an SQLite file.");
            }

            return isSqliteFile;
        }
    }
}

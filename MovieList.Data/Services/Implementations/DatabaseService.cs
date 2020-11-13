using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using Dapper;
using Dapper.Contrib.Extensions;

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

        public void CreateDatabase(Settings settings, IEnumerable<Kind> initialKinds, IEnumerable<Tag> initialTags)
        {
            this.Log().Debug($"Creating a new database: {this.DatabasePath}");

            if (File.Exists(this.DatabasePath))
            {
                this.Log().Warn($"{this.DatabasePath} already exists");
                return;
            }

            string sql = Resource.AsString(SchemaSql);

            this.WithTransaction((connection, transaction) =>
            {
                connection.Execute(sql, transaction);

                this.InitSettings(connection, transaction, settings);

                connection.Insert(initialKinds, transaction);
                this.InsertTags(initialTags, connection, transaction);
            });
        }

        public bool ValidateDatabase()
        {
            this.Log().Debug($"Validating the database: {this.DatabasePath}");

            bool isSqliteFile = this.CheckIfSqliteDatabase();

            if (!isSqliteFile)
            {
                return false;
            }

            this.Log().Debug($"Checking the database schema: {this.DatabasePath}");

            return true;
        }

        private void InsertTags(IEnumerable<Tag> initialTags, IDbConnection connection, IDbTransaction transaction)
        {
            foreach (var tag in initialTags)
            {
               tag.Id = (int)connection.Insert(tag, transaction);
            }

            var implications = initialTags
                .SelectMany(tag => tag.ImpliedTags
                    .Select(impliedTag => new TagImplication
                    {
                        PremiseId = tag.Id,
                        ConsequenceId = impliedTag.Id
                    }));

            connection.Insert(implications, transaction);

            foreach (var tag in initialTags)
            {
                tag.Id = default;
            }
        }

        private void InitSettings(IDbConnection connection, IDbTransaction transaction, Settings settings)
        {
            this.Log().Debug($"Initializing settings for the database: {this.DatabasePath}");

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
                    Key = SettingsListCultureKey,
                    Value = settings.CultureInfo.Name
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
                },
                new Setting
                {
                    Key = SettingsDefaultFirstSortOrderKey,
                    Value = settings.DefaultFirstSortOrder.ToString()
                },
                new Setting
                {
                    Key = SettingsDefaultSecondSortOrderKey,
                    Value = settings.DefaultSecondSortOrder.ToString()
                },
                new Setting
                {
                    Key = SettingsDefaultFirstSortDirectionKey,
                    Value = settings.DefaultFirstSortDirection.ToString()
                },
                new Setting
                {
                    Key = SettingsDefaultSecondSortDirectionKey,
                    Value = settings.DefaultSecondSortDirection.ToString()
                }
            };

            foreach (var setting in settingsList)
            {
                connection.Insert(setting, transaction);
            }
        }

        private bool CheckIfSqliteDatabase()
        {
            this.Log().Debug($"Checking if the file is an SQLite database: {this.DatabasePath}");

            if (!File.Exists(this.DatabasePath))
            {
                this.Log().Error($"{this.DatabasePath} doesn't exist");
                return false;
            }

            const int headerSize = 16;

            using var stream = new FileStream(
                this.DatabasePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var header = new byte[headerSize];
            stream.Read(header, 0, headerSize);

            bool isSqliteFile = Encoding.UTF8.GetString(header).Contains(SqliteFileHeader);

            if (!isSqliteFile)
            {
                this.Log().Error($"{this.DatabasePath} is not an SQLite file");
            }

            return isSqliteFile;
        }
    }
}

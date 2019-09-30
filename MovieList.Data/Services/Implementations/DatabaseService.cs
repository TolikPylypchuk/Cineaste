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

namespace MovieList.Data.Services.Implementations
{
    internal class DatabaseService : IDatabaseService, IEnableLogger
    {
        private const string SchemaSql = "../../schema.sql";

        private readonly string file;

        public DatabaseService(string file)
            => this.file = file;

        public async Task<IEnumerable<Kind>> GetAllKindsAsync()
        {
            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            return await connection.GetAllAsync<Kind>();
        }

        [LogException]
        [SuppressMessage(
            "Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "SQL comes from a database creation script")]
        public async Task CreateDatabaseAsync()
        {
            if (File.Exists(this.file))
            {
                this.Log().Warn($"{this.file} already exists.");
                return;
            }
            
            string sql = Resource.AsString(SchemaSql);

            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql);
        }

        private SqliteConnection GetSqliteConnection()
            => Locator.Current.GetService<SqliteConnection>(this.file);
    }
}

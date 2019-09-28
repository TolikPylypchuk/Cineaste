using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.Sqlite;

using Resourcer;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal class DatabaseService : IDatabaseService, IEnableLogger
    {
        public const string SchemaSql = "../../schema.sql";

        [LogException]
        [SuppressMessage(
            "Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "SQL comes from a database creation script")]
        public async Task CreateDatabaseAsync(string file)
        {
            if (File.Exists(file))
            {
                this.Log().Warn($"{file} already exists.");
                return;
            }
            
            string sql = Resource.AsString(SchemaSql);

            await using var connection = new SqliteConnection($"Data Source={file}");
            await connection.ExecuteAsync(sql);
        }
    }
}

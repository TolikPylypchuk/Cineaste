using Microsoft.Data.Sqlite;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal abstract class ServiceBase
    {
        protected readonly string DatabasePath;

        protected ServiceBase(string file)
            => this.DatabasePath = file;

        protected SqliteConnection GetSqliteConnection()
            => Locator.Current.GetService<SqliteConnection>(this.DatabasePath);
    }
}

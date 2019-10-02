using Microsoft.Data.Sqlite;

using MovieList.Data.Services;
using MovieList.Data.Services.Implementations;

using Splat;

namespace MovieList.Data
{
    public static class LocatorExtensions
    {
        public static void RegisterDatabaseServices(this IMutableDependencyResolver resolver, string file)
        {
            if (!resolver.HasRegistration(typeof(IDatabaseService), file))
            {
                resolver.Register(() => new DatabaseService(file), typeof(IDatabaseService), file);
                resolver.Register(() => new KindService(file), typeof(IKindService), file);
                resolver.Register(() => new SettingsService(file), typeof(ISettingsService), file);
                resolver.Register(() => new SqliteConnection($"Data Source={file}"), typeof(SqliteConnection), file);
            }
        }

        public static void UnregisterDatabaseServices(this IMutableDependencyResolver resolver, string file)
        {
            if (resolver.HasRegistration(typeof(IDatabaseService), file))
            {
                resolver.UnregisterCurrent(typeof(IDatabaseService), file);
                resolver.UnregisterCurrent(typeof(IKindService), file);
                resolver.UnregisterCurrent(typeof(ISettingsService), file);
                resolver.UnregisterCurrent(typeof(SqliteConnection), file);
            }
        }
    }
}

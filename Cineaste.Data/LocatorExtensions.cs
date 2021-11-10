namespace Cineaste.Data;

using Microsoft.Data.Sqlite;

public static class LocatorExtensions
{
    public static void RegisterDatabaseServices(this IMutableDependencyResolver resolver, string file)
    {
        if (!resolver.HasRegistration(typeof(IDatabaseService), file))
        {
            resolver.RegisterLazySingleton<IDatabaseService>(() => new DatabaseService(file), file);
            resolver.RegisterLazySingleton<IListService>(() => new ListService(file), file);

            resolver.RegisterLazySingleton<IEntityService<Movie>>(() => new MovieService(file), file);
            resolver.RegisterLazySingleton<IEntityService<Series>>(() => new SeriesService(file), file);
            resolver.RegisterLazySingleton<IEntityService<Franchise>>(() => new FranchiseService(file), file);

            resolver.RegisterLazySingleton<ISettingsEntityService<Kind>>(() => new KindService(file), file);
            resolver.RegisterLazySingleton<ISettingsEntityService<Tag>>(() => new TagService(file), file);
            resolver.RegisterLazySingleton<ISettingsService>(() => new SettingsService(file), file);

            resolver.Register<IDbConnection>(() => new SqliteConnection($"Data Source={file}"), file);
        }
    }

    public static void UnregisterDatabaseServices(this IMutableDependencyResolver resolver, string file)
    {
        if (resolver.HasRegistration(typeof(IDatabaseService), file))
        {
            resolver.UnregisterCurrent<IDatabaseService>(file);
            resolver.UnregisterCurrent<IListService>(file);

            resolver.UnregisterCurrent<IEntityService<Movie>>(file);
            resolver.UnregisterCurrent<IEntityService<Series>>(file);
            resolver.UnregisterCurrent<IEntityService<Franchise>>(file);

            resolver.UnregisterCurrent<ISettingsEntityService<Kind>>(file);
            resolver.UnregisterCurrent<ISettingsEntityService<Tag>>(file);
            resolver.UnregisterCurrent<ISettingsService>(file);

            resolver.UnregisterCurrent<IDbConnection>(file);
        }
    }
}

using MovieList.Data.Services;
using MovieList.Data.Services.Implementations;

using Splat;

namespace MovieList.Data
{
    public static class LocatorExtensions
    {
        public static void RegisterDatabaseServices(this IMutableDependencyResolver resolver)
        {
            resolver.Register(() => new DatabaseService(), typeof(IDatabaseService));
        }
    }
}

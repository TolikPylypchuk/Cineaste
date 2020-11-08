using MovieList.Core.State;

using ReactiveUI;

using Splat;

namespace MovieList.Core
{
    public static class LocatorExtensions
    {
        public static void RegisterSuspensionDriver(this IMutableDependencyResolver resolver) =>
            resolver.RegisterLazySingleton<ISuspensionDriver>(() => new AkavacheSuspensionDriver<AppState>());
    }
}

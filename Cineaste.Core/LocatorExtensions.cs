using Cineaste.Core.State;

using ReactiveUI;

using Splat;

namespace Cineaste.Core
{
    public static class LocatorExtensions
    {
        public static void RegisterSuspensionDriver(this IMutableDependencyResolver resolver) =>
            resolver.RegisterLazySingleton<ISuspensionDriver>(() => new AkavacheSuspensionDriver<AppState>());
    }
}

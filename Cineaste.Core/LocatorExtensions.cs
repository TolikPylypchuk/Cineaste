using System;

using Cineaste.Core.State;

using ReactiveUI;

using Splat;

namespace Cineaste.Core
{
    [Obsolete("No need to use this class if JsonSuspensionDriver is used")]
    public static class LocatorExtensions
    {
        public static void RegisterSuspensionDriver(this IMutableDependencyResolver resolver) =>
            resolver.RegisterLazySingleton<ISuspensionDriver>(() => new AkavacheSuspensionDriver<AppState>());
    }
}

using System.Reflection;

using Fluxor.Blazor.Web.ReduxDevTools;

namespace Cineaste.Client;

public static class FluxorExtensions
{
    public static IServiceCollection AddCineasteFluxor(this IServiceCollection services) =>
        services.AddFluxor(options =>
        {
            options.ScanAssemblies(Assembly.GetExecutingAssembly());

#if DEBUG
            options.UseReduxDevTools();
#endif
        });
}

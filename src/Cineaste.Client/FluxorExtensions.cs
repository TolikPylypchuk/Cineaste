using System.Reflection;

using Fluxor.Blazor.Web.ReduxDevTools;

namespace Cineaste.Client;

public static class FluxorExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCineasteFluxor() =>
            services.AddFluxor(options =>
            {
                options.ScanAssemblies(Assembly.GetExecutingAssembly());

#if DEBUG
                options.UseReduxDevTools();
#endif
            });
    }
}

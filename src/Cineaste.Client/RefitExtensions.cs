using Cineaste.Client.BaseUri;

namespace Cineaste.Client;

public static class RefitExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCineasteRefitClients()
        {
            services.AddCineasteRefitClient<ICultureApi>();
            services.AddCineasteRefitClient<IListApi>();
            services.AddCineasteRefitClient<IMovieApi>();
            services.AddCineasteRefitClient<ISeriesApi>();
            services.AddCineasteRefitClient<IFranchiseApi>();

            return services;
        }

        private IHttpClientBuilder AddCineasteRefitClient<T>() =>
            services.AddRefitClient(typeof(T), provider =>
                new RefitSettings()
                {
                    ContentSerializer = new SystemTextJsonContentSerializer(
                        provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value)
                })
                .ConfigureHttpClient((provider, client) =>
                    client.BaseAddress = new Uri(provider.GetRequiredService<IBaseUriProvider>().BaseUri, "/api"));
    }
}

namespace Cineaste.Client;

public static class RefitExtensions
{
    public static IHttpClientBuilder AddRefitClient<T>(this IServiceCollection services, Uri baseAddress) =>
        services.AddRefitClient(typeof(T), provider =>
            new RefitSettings()
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                    provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value)
            })
            .ConfigureHttpClient(client => client.BaseAddress = baseAddress);
}

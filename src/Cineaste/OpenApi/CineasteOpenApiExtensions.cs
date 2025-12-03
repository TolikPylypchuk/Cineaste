using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace Cineaste.OpenApi;

public static class CineasteOpenApiExtensions
{
    public static IServiceCollection AddCineasteOpenApi(this IServiceCollection services, IConfiguration config) =>
        services
            .Configure<CineasteOpenApiOptions>(config)
            .AddOpenApi("api", options =>
            {
                options.AddDocumentTransformer((document, context, token) =>
                {
                    var options = context.ApplicationServices
                        .GetRequiredService<IOptions<CineasteOpenApiOptions>>()
                        .Value;

                    document.Info.Title = options.Info.Title;
                    document.Info.Description = options.Info.Description;
                    document.Info.Version = options.Info.Version;

                    document.Servers =
                    [
                        new OpenApiServer
                        {
                            Url = options.Server.Url,
                            Description = options.Server.Description
                        }
                    ];

                    return Task.CompletedTask;
                });
            });
}

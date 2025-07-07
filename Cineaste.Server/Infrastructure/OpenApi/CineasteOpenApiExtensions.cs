using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Cineaste.Server.Infrastructure.OpenApi;

public static class CineasteOpenApiExtensions
{
    public static IServiceCollection AddCineasteOpenApi(this IServiceCollection services) =>
        services.AddOpenApi("api", options =>
        {
            options.AddDocumentTransformer((document, context, token) =>
            {
                var options = context.ApplicationServices.GetRequiredService<IOptions<CineasteOpenApiOptions>>().Value;

                document.Info.Title = options.Info.Title;
                document.Info.Description = options.Info.Description;
                document.Info.Version = options.Info.Version;

                document.Servers.Clear();
                document.Servers.Add(new OpenApiServer
                {
                    Url = options.Server.Url,
                    Description = options.Server.Description
                });

                return Task.CompletedTask;
            });
        });
}

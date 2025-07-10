namespace Cineaste.Server.Infrastructure.OpenApi;

public record CineasteOpenApiOptions
{
    public required CineasteOpenApiInfo Info { get; init; }
    public required CineasteOpenApiServer Server { get; init; }
}

public record CineasteOpenApiInfo
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Version { get; init; }
}

public record CineasteOpenApiServer
{
    public required string Url { get; init; }
    public required string Description { get; init; }
}

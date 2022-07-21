namespace Cineaste.Server.Api;

public static class FallbackRoutes
{
    public static void MapFallbackRoutes(this IEndpointRouteBuilder routes, PathString prefix)
    {
        routes.MapFallback(prefix + "/{**path}", NotFound);
        routes.MapFallbackToFile("/{**path}", "index.html");
    }

    private static void NotFound() =>
        throw new NotFoundException(Resources.Any, "The requested resource was not found");
}

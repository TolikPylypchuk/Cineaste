namespace Cineaste.Server.Api;

public static class CultureRoutes
{
    public static void MapCultureRoutes(this IEndpointRouteBuilder routes) =>
        routes.MapGet("/api/cultures", GetAllCultures);

    private static IResult GetAllCultures(CultureExtractor cultureExtractor) =>
        Results.Ok(cultureExtractor.GetAllCultures());
}

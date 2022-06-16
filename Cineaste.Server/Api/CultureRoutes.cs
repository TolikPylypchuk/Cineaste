namespace Cineaste.Server.Api;

public static class CultureRoutes
{
    public static void MapCultureRoutes(this IEndpointRouteBuilder routes) =>
        routes.MapGet("/cultures", GetAllCultures);

    private static IResult GetAllCultures(ICultureExtractor cultureExtractor) =>
        Results.Ok(cultureExtractor.GetAllCultures());
}

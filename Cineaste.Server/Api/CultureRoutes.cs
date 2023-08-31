namespace Cineaste.Server.Api;

public static class CultureRoutes
{
    public static void MapCultureRoutes(this IEndpointRouteBuilder routes) =>
        routes.MapGet("/api/cultures", GetAllCultures);

    private static IResult GetAllCultures(CultureProvider cultureProvider) =>
        Results.Ok(cultureProvider.GetAllCultures());
}

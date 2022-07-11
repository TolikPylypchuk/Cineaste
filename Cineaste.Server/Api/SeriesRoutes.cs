namespace Cineaste.Server.Api;

public static class SeriesRoutes
{
    public static void MapSeriesRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/series/{id}", GetSeries);
    }

    private static async Task<IResult> GetSeries(Guid id, ISeriesService seriesService) =>
        Results.Ok(await seriesService.GetSeries(Id.Create<Series>(id)));
}

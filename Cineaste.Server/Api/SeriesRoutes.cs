namespace Cineaste.Server.Api;

public static class SeriesRoutes
{
    public static void MapSeriesRoutes(this IEndpointRouteBuilder routes, PathString prefix)
    {
        routes.MapGet(prefix + "/series/{id}", GetSeries);
        routes.MapDelete(prefix + "/series/{id}", DeleteSeries);
    }

    private static async Task<IResult> GetSeries(Guid id, ISeriesService seriesService) =>
        Results.Ok(await seriesService.GetSeries(Id.Create<Series>(id)));

    private static async Task<IResult> DeleteSeries(Guid id, ISeriesService seriesService)
    {
        await seriesService.DeleteSeries(Id.Create<Series>(id));
        return Results.NoContent();
    }
}

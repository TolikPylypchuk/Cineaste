namespace Cineaste.Server.Api;

public static class SeriesRoutes
{
    public static void MapSeriesRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/series/{id}", GetSeries);
        routes.MapPost("/api/series", CreateSeries);
        routes.MapPut("/api/series/{id}", UpdateSeries);
        routes.MapDelete("/api/series/{id}", DeleteSeries);
    }

    private static async Task<IResult> GetSeries(Guid id, SeriesService seriesService) =>
        Results.Ok(await seriesService.GetSeries(Id.For<Series>(id)));

    private static async Task<IResult> CreateSeries(Validated<SeriesRequest> request, SeriesService seriesService)
    {
        var series = await seriesService.CreateSeries(request);
        return Results.Created($"/api/series/{series.Id}", series);
    }

    private static async Task<IResult> UpdateSeries(
        Guid id,
        Validated<SeriesRequest> request,
        SeriesService seriesService) =>
        Results.Ok(await seriesService.UpdateSeries(Id.For<Series>(id), request));

    private static async Task<IResult> DeleteSeries(Guid id, SeriesService seriesService)
    {
        await seriesService.DeleteSeries(Id.For<Series>(id));
        return Results.NoContent();
    }
}

namespace Cineaste.Client.Api;

public interface ISeriesApi
{
    [Get("/api/series/{id}")]
    public Task<IApiResponse<SeriesModel>> GetSeries(Guid id);
}

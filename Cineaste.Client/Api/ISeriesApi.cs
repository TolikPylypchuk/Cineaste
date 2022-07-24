namespace Cineaste.Client.Api;

public interface ISeriesApi
{
    [Get("/series/{id}")]
    public Task<IApiResponse<SeriesModel>> GetSeries(Guid id);

    [Delete("/series/{id}")]
    public Task<IApiResponse> DeleteSeries(Guid id);
}

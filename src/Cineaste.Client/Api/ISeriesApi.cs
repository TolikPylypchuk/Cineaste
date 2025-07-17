namespace Cineaste.Client.Api;

public interface ISeriesApi
{
    [Get("/series/{id}")]
    public Task<IApiResponse<SeriesModel>> GetSeries(Guid id);

    [Post("/series")]
    public Task<IApiResponse<SeriesModel>> AddSeries([Body] SeriesRequest request);

    [Put("/series/{id}")]
    public Task<IApiResponse<SeriesModel>> UpdateSeries(Guid id, [Body] SeriesRequest request);

    [Delete("/series/{id}")]
    public Task<IApiResponse> RemoveSeries(Guid id);
}

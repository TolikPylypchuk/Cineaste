namespace Cineaste.Client.Api;

public interface ILimitedSeriesApi
{
    [Get("/limited-series/{id}")]
    public Task<IApiResponse<LimitedSeriesModel>> GetLimitedSeries(Guid id);

    [Post("/limited-series")]
    public Task<IApiResponse<LimitedSeriesModel>> AddLimitedSeries([Body] LimitedSeriesRequest request);

    [Put("/limited-series/{id}")]
    public Task<IApiResponse<LimitedSeriesModel>> UpdateLimitedSeries(Guid id, [Body] LimitedSeriesRequest request);

    [Delete("/limited-series/{id}")]
    public Task<IApiResponse> RemoveLimitedSeries(Guid id);

    [Multipart]
    [Put("/limited-series/{id}/poster")]
    public Task<IApiResponse> SetLimitedSeriesPoster(Guid id, StreamPart file);

    [Put("/limited-series/{id}/poster")]
    public Task<IApiResponse> SetLimitedSeriesPoster(Guid id, [Body] PosterRequestBase request);

    [Delete("/limited-series/{id}/poster")]
    public Task<IApiResponse> RemoveLimitedSeriesPoster(Guid id);
}

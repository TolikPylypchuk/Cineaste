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

    [Multipart]
    [Put("/series/{id}/poster")]
    public Task<IApiResponse> SetSeriesPoster(Guid id, StreamPart file);

    [Put("/series/{id}/poster")]
    public Task<IApiResponse> SetSeriesPoster(Guid id, [Body] PosterUrlRequest request);

    [Delete("/series/{id}/poster")]
    public Task<IApiResponse> RemoveSeriesPoster(Guid id);

    [Multipart]
    [Put("/series/{seriesId}/seasons/periods/{periodId}/poster")]
    public Task<IApiResponse> SetSeasonPoster(Guid seriesId, Guid periodId, StreamPart file);

    [Put("/series/{seriesId}/seasons/periods/{periodId}/poster")]
    public Task<IApiResponse> SetSeasonPoster(Guid seriesId, Guid periodId, [Body] PosterUrlRequest request);

    [Delete("/series/{seriesId}/seasons/periods/{periodId}/poster")]
    public Task<IApiResponse> RemoveSeasonPoster(Guid seriesId, Guid periodId);

    [Multipart]
    [Put("/series/{seriesId}/special-episodes/{episodeId}/poster")]
    public Task<IApiResponse> SetSpecialEpisodePoster(Guid seriesId, Guid episodeId, StreamPart file);

    [Put("/series/{seriesId}/special-episodes/{episodeId}/poster")]
    public Task<IApiResponse> SetSpecialEpisodePoster(Guid seriesId, Guid episodeId, [Body] PosterUrlRequest request);

    [Delete("/series/{seriesId}/special-episodes/{episodeId}/poster")]
    public Task<IApiResponse> RemoveSpecialEpisodePoster(Guid seriesId, Guid episodeId);
}

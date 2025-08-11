namespace Cineaste.Client.Api;

public interface IMovieApi
{
    [Get("/movies/{id}")]
    public Task<IApiResponse<MovieModel>> GetMovie(Guid id);

    [Post("/movies")]
    public Task<IApiResponse<MovieModel>> AddMovie([Body] MovieRequest request);

    [Put("/movies/{id}")]
    public Task<IApiResponse<MovieModel>> UpdateMovie(Guid id, [Body] MovieRequest request);

    [Delete("/movies/{id}")]
    public Task<IApiResponse> RemoveMovie(Guid id);

    [Multipart]
    [Put("/movies/{id}/poster")]
    public Task<IApiResponse> SetMoviePoster(Guid id, StreamPart file);

    [Put("/movies/{id}/poster")]
    public Task<IApiResponse> SetMoviePoster(Guid id, [Body] PosterRequestBase request);

    [Delete("/movies/{id}/poster")]
    public Task<IApiResponse> RemoveMoviePoster(Guid id);
}

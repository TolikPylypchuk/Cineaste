namespace Cineaste.Client.Api;

public interface IMovieApi
{
    [Get("/api/movies/{id}")]
    public Task<IApiResponse<MovieModel>> GetMovie(Guid id);

    [Post("/api/movies")]
    public Task<IApiResponse<MovieModel>> CreateMovie([Body] MovieRequest request);

    [Put("/api/movies/{id}")]
    public Task<IApiResponse<MovieModel>> UpdateMovie(Guid id, [Body] MovieRequest request);

    [Delete("/api/movies/{id}")]
    public Task<IApiResponse> DeleteMovie(Guid id);
}

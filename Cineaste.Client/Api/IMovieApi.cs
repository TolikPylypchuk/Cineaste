namespace Cineaste.Client.Api;

public interface IMovieApi
{
    [Get("/movies/{id}")]
    public Task<IApiResponse<MovieModel>> GetMovie(Guid id);

    [Post("/movies")]
    public Task<IApiResponse<MovieModel>> CreateMovie([Body] MovieRequest request);

    [Put("/movies/{id}")]
    public Task<IApiResponse<MovieModel>> UpdateMovie(Guid id, [Body] MovieRequest request);

    [Delete("/movies/{id}")]
    public Task<IApiResponse> DeleteMovie(Guid id);
}

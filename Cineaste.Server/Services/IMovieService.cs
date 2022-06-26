namespace Cineaste.Server.Services;

public interface IMovieService
{
    Task<MovieModel> GetMovie(Id<Movie> id);
    Task<MovieModel> CreateMovie(Validated<MovieRequest> request);
    Task DeleteMovie(Id<Movie> id);
}

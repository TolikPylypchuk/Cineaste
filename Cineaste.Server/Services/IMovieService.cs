namespace Cineaste.Server.Services;

public interface IMovieService
{
    Task<MovieModel> GetMovie(Id<Movie> id);
    Task DeleteMovie(Id<Movie> id);
}

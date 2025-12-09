namespace Cineaste.Application.Services.Poster;

public interface IPosterUrlProvider
{
    [return: NotNullIfNotNull(nameof(posterHash))]
    string? GetPosterUrl(Id<Movie> movieId, PosterHash? posterHash);

    [return: NotNullIfNotNull(nameof(posterHash))]
    string? GetPosterUrl(Id<Series> seriesId, PosterHash? posterHash);

    [return: NotNullIfNotNull(nameof(posterHash))]
    string? GetPosterUrl(Id<Series> seriesId, Id<Period> periodId, PosterHash? posterHash);

    [return: NotNullIfNotNull(nameof(posterHash))]
    string? GetPosterUrl(Id<Series> seriesId, Id<SpecialEpisode> episodeId, PosterHash? posterHash);

    [return: NotNullIfNotNull(nameof(posterHash))]
    string? GetPosterUrl(Id<Franchise> franchiseId, PosterHash? posterHash);
}

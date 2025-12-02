namespace Cineaste.Application;

public static class Urls
{
    [return: NotNullIfNotNull(nameof(posterHash))]
    public static string? MoviePoster(Id<Movie> movieId, PosterHash? posterHash) =>
        posterHash is not null ? $"/api/movies/{movieId.Value}/poster/?h={posterHash?.Value}" : null;

    [return: NotNullIfNotNull(nameof(posterHash))]
    public static string? SeriesPoster(Id<Series> seriesId, PosterHash? posterHash) =>
        posterHash is not null ? $"/api/series/{seriesId.Value}/poster/?h={posterHash?.Value}" : null;

    [return: NotNullIfNotNull(nameof(posterHash))]
    public static string? SeasonPoster(Id<Series> seriesId, Id<Period> periodId, PosterHash? posterHash) =>
        posterHash is not null
            ? $"/api/series/{seriesId.Value}/seasons/periods/{periodId.Value}/poster/?h={posterHash?.Value}"
            : null;

    [return: NotNullIfNotNull(nameof(posterHash))]
    public static string? SpecialEpisodePoster(
        Id<Series> seriesId,
        Id<SpecialEpisode> episodeId,
        PosterHash? posterHash) =>
        posterHash is not null
            ? $"/api/series/{seriesId.Value}/special-episodes/{episodeId.Value}/poster/?h={posterHash?.Value}"
            : null;

    [return: NotNullIfNotNull(nameof(posterHash))]
    public static string? FranchisePoster(Id<Franchise> franchiseId, PosterHash? posterHash) =>
        posterHash is not null ? $"/api/franchises/{franchiseId.Value}/poster/?h={posterHash?.Value}" : null;
}

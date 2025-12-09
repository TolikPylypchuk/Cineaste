using System.Diagnostics.CodeAnalysis;

using Cineaste.Application.Services.Poster;

namespace Cineaste.Services.Poster;

public sealed class PosterUrlProvider : IPosterUrlProvider
{
    [return: NotNullIfNotNull(nameof(posterHash))]
    public string? GetPosterUrl(Id<Movie> movieId, PosterHash? posterHash) =>
        posterHash is not null ? $"/api/movies/{movieId}/poster?h={posterHash}" : null;

    [return: NotNullIfNotNull(nameof(posterHash))]
    public string? GetPosterUrl(Id<Series> seriesId, PosterHash? posterHash) =>
        posterHash is not null ? $"/api/series/{seriesId}/poster?h={posterHash}" : null;

    [return: NotNullIfNotNull(nameof(posterHash))]
    public string? GetPosterUrl(Id<Series> seriesId, Id<Period> periodId, PosterHash? posterHash) =>
        posterHash is not null ? $"/api/series/{seriesId}/seasons/periods/{periodId}/poster?h={posterHash}" : null;

    [return: NotNullIfNotNull(nameof(posterHash))]
    public string? GetPosterUrl(Id<Series> seriesId, Id<SpecialEpisode> episodeId, PosterHash? posterHash) =>
        posterHash is not null ? $"/api/series/{seriesId}/special-episodes/{episodeId}/poster?h={posterHash}" : null;

    [return: NotNullIfNotNull(nameof(posterHash))]
    public string? GetPosterUrl(Id<Franchise> franchiseId, PosterHash? posterHash) =>
        posterHash is not null ? $"/api/franchises/{franchiseId}/poster?h={posterHash}" : null;
}

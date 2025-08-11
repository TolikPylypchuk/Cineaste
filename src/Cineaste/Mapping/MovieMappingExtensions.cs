namespace Cineaste.Mapping;

public static class MovieMappingExtensions
{
    public static MovieModel ToMovieModel(this Movie movie) =>
        new(
            movie.Id.Value,
            movie.AllTitles.ToTitleModels(isOriginal: false),
            movie.AllTitles.ToTitleModels(isOriginal: true),
            movie.Year,
            movie.IsWatched,
            movie.IsReleased,
            movie.Kind.ToListKindModel(),
            movie.ImdbId?.Value,
            movie.RottenTomatoesId?.Value,
            movie.GetActiveColor().HexValue,
            movie.ListItem?.SequenceNumber ?? 0,
            movie.GetPosterUrl(),
            movie.FranchiseItem.ToFranchiseItemInfoModel());

    public static Movie ToMovie(this Validated<MovieRequest> request, Id<Movie> id, MovieKind kind) =>
        new(
            id,
            request.Value.ToTitles(),
            request.Value.Year,
            request.Value.IsWatched,
            request.Value.IsReleased,
            kind)
        {
            ImdbId = ImdbId.Nullable(request.Value.ImdbId),
            RottenTomatoesId = RottenTomatoesId.Nullable(request.Value.RottenTomatoesId)
        };

    public static void Update(this Movie movie, Validated<MovieRequest> request, MovieKind kind)
    {
        movie.ReplaceTitles(
            request.Value.Titles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
            isOriginal: false);

        movie.ReplaceTitles(
            request.Value.OriginalTitles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
            isOriginal: true);

        movie.Year = request.Value.Year;
        movie.IsWatched = request.Value.IsWatched;
        movie.IsReleased = request.Value.IsReleased;
        movie.Kind = kind;
        movie.ImdbId = ImdbId.Nullable(request.Value.ImdbId);
        movie.RottenTomatoesId = RottenTomatoesId.Nullable(request.Value.RottenTomatoesId);

        movie.ListItem?.SetProperties(movie);
    }

    private static string? GetPosterUrl(this Movie movie) =>
        Urls.MoviePoster(movie.Id, movie.PosterHash);
}

namespace Cineaste.Server.Mapping;

public static class MovieMappingExtensions
{
    public static MovieModel ToMovieModel(this Movie movie) =>
        new(
            movie.Id.Value,
            movie.Titles
                .Where(title => !title.IsOriginal)
                .Select(title => title.ToTitleModel())
                .OrderBy(title => title.Priority)
                .ToImmutableList(),
            movie.Titles
                .Where(title => title.IsOriginal)
                .Select(title => title.ToTitleModel())
                .OrderBy(title => title.Priority)
                .ToImmutableList(),
            movie.Year,
            movie.IsWatched,
            movie.IsReleased,
            movie.Kind.ToListKindModel(),
            movie.ImdbId,
            movie.RottenTomatoesLink,
            movie.FranchiseItem.GetDisplayNumber());

    public static Movie ToMovie(this Validated<MovieRequest> request, Id<Movie> id, MovieKind kind) =>
        new(
            id,
            request.Value.Titles
                .Select(titleRequest => titleRequest.ToTitle(isOriginal: false))
                .Concat(request.Value.OriginalTitles
                    .Select(titleRequest => titleRequest.ToTitle(isOriginal: true))),
            request.Value.Year,
            request.Value.IsWatched,
            request.Value.IsReleased,
            kind);

    public static void Update(this Movie movie, Validated<MovieRequest> request, MovieKind kind)
    {
        movie.ReplaceTitles(
            request.Value.Titles.OrderBy(title => title.Priority).Select(title => title.Name),
            isOriginal: false);

        movie.ReplaceTitles(
            request.Value.OriginalTitles.OrderBy(title => title.Priority).Select(title => title.Name),
            isOriginal: true);

        movie.Year = request.Value.Year;
        movie.IsWatched = request.Value.IsWatched;
        movie.IsReleased = request.Value.IsReleased;
        movie.Kind = kind;
        movie.ImdbId = request.Value.ImdbId;
        movie.RottenTomatoesLink = request.Value.RottenTomatoesLink;
    }
}

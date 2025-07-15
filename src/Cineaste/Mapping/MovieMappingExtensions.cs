namespace Cineaste.Mapping;

public static class MovieMappingExtensions
{
    public static MovieModel ToMovieModel(this Movie movie) =>
        new(
            movie.Id.Value,
            movie.Titles.ToTitleModels(isOriginal: false),
            movie.Titles.ToTitleModels(isOriginal: true),
            movie.Year,
            movie.IsWatched,
            movie.IsReleased,
            movie.Kind.ToListKindModel(),
            movie.ImdbId,
            movie.RottenTomatoesId,
            movie.FranchiseItem?.ParentFranchise.Id.Value,
            movie.FranchiseItem?.SequenceNumber,
            movie.FranchiseItem.GetDisplayNumber(),
            movie.FranchiseItem.IsFirst(),
            movie.FranchiseItem.IsLast());

    public static Movie ToMovie(this Validated<MovieRequest> request, Id<Movie> id, MovieKind kind) =>
        new(
            id,
            request.Value.ToTitles(),
            request.Value.Year,
            request.Value.IsWatched,
            request.Value.IsReleased,
            kind)
        {
            ImdbId = request.Value.ImdbId,
            RottenTomatoesId = request.Value.RottenTomatoesId
        };

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
        movie.RottenTomatoesId = request.Value.RottenTomatoesId;
    }
}

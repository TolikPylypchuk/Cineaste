namespace Cineaste.Server.Mappers;

public static class MovieMappingExtensions
{
    public static MovieModel ToMovieModel(this Movie movie) =>
        new(
            movie.Id.Value,
            movie.Titles.Where(title => !title.IsOriginal).Select(title => title.ToTitleModel()).ToImmutableList(),
            movie.Titles.Where(title => title.IsOriginal).Select(title => title.ToTitleModel()).ToImmutableList(),
            movie.Year,
            movie.Kind.ToSimpleKindModel(),
            movie.ImdbId,
            movie.RottenTomatoesLink);
}

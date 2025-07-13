namespace Cineaste.Mapping;

public static class TitleMappingExtensions
{
    public static TitleModel ToTitleModel(this Title title) =>
        new(title.Name, title.Priority);

    public static Title ToTitle(this TitleRequest title, bool isOriginal) =>
        new(title.Name, title.Priority, isOriginal);

    public static ImmutableList<TitleModel> ToTitleModels(this IEnumerable<Title> titles, bool isOriginal) =>
        [.. titles
            .Where(title => title.IsOriginal == isOriginal)
            .Select(title => title.ToTitleModel())
            .OrderBy(title => title.Priority)];

    public static IEnumerable<Title> ToTitles(this ITitledRequest request) =>
        Enumerable.Concat(
            request.Titles.Select(titleRequest => titleRequest.ToTitle(isOriginal: false)),
            request.OriginalTitles.Select(titleRequest => titleRequest.ToTitle(isOriginal: true)));
}

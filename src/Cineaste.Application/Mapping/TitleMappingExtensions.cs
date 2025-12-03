namespace Cineaste.Application.Mapping;

public static class TitleMappingExtensions
{
    extension(Title title)
    {
        public TitleModel ToTitleModel() =>
            new(title.Name, title.SequenceNumber);
    }

    extension(TitleRequest title)
    {
        public Title ToTitle(bool isOriginal) =>
            new(title.Name, title.SequenceNumber, isOriginal);
    }

    extension(IEnumerable<Title> titles)
    {
        public ImmutableList<TitleModel> ToTitleModels(bool isOriginal) =>
            [.. titles
            .Where(title => title.IsOriginal == isOriginal)
            .Select(title => title.ToTitleModel())
            .OrderBy(title => title.SequenceNumber)];
    }

    extension(ITitledRequest request)
    {
        public IEnumerable<Title> ToTitles() =>
            Enumerable.Concat(
                request.Titles.Select(titleRequest => titleRequest.ToTitle(isOriginal: false)),
                request.OriginalTitles.Select(titleRequest => titleRequest.ToTitle(isOriginal: true)));
    }
}

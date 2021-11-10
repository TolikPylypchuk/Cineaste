namespace Cineaste.Converters;

public sealed class ListSortDirectionConverter : EnumConverter<ListSortDirection>
{
    protected override Dictionary<ListSortDirection, string> CreateConverterDictionary() =>
        new()
        {
            [ListSortDirection.Ascending] = Messages.ListSortDirectionAscending,
            [ListSortDirection.Descending] = Messages.ListSortDirectionDescending
        };
}

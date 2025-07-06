using Cineaste.Shared.Models.Shared;

namespace Cineaste.Shared.Validation.TestData;

internal class TitleUtils
{
    public static ImmutableValueList<TitleRequest> TitleRequests(params string[] titles) =>
        TitleRequests(titles, differentTitlePriorities: false);

    public static ImmutableValueList<TitleRequest> TitleRequests(
        IEnumerable<string>? titles,
        bool differentTitlePriorities) =>
        titles is null
            ? ImmutableList.Create(new TitleRequest("Title", 1)).AsValue()
            : titles.Select((title, index) => new TitleRequest(title, differentTitlePriorities ? index + 1 : 1))
                .ToImmutableList()
                .AsValue();
}

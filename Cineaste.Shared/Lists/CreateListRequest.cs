namespace Cineaste.Shared.Lists;

public sealed record CreateListRequest
{
    public string Name { get; }
    public string Handle { get; }
    public string Culture { get; }
    public string DefaultSeasonTitle { get; }
    public string DefaultSeasonOriginalTitle { get; }

    public CreateListRequest(
        string name,
        string handle,
        string culture,
        string defaultSeasonTitle,
        string defaultSeasonOriginalTitle)
    {
        this.Name = name;
        this.Handle = handle;
        this.Culture = culture;
        this.DefaultSeasonTitle = defaultSeasonTitle;
        this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
    }
}

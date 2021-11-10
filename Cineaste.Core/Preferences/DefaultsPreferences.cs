namespace Cineaste.Core.Preferences;

[ToString]
[Equals(DoNotAddEqualityOperators = true)]
public sealed class DefaultsPreferences
{
    public DefaultsPreferences(
        string defaultSeasonTitle,
        string defaultSeasonOriginalTitle,
        List<Kind> defaultKinds,
        List<Tag> defaultTags,
        CultureInfo defaultCultureInfo,
        ListSortOrder defaultFirstSortOrder,
        ListSortOrder defaultSecondSortOrder,
        ListSortDirection defaultFirstSortDirection,
        ListSortDirection defaultSecondSortDirection)
    {
        this.DefaultSeasonTitle = defaultSeasonTitle;
        this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;

        this.DefaultKinds = defaultKinds;
        this.DefaultTags = defaultTags;

        this.DefaultCultureInfo = defaultCultureInfo;

        this.DefaultFirstSortOrder = defaultFirstSortOrder;
        this.DefaultSecondSortOrder = defaultSecondSortOrder;

        this.DefaultFirstSortDirection = defaultFirstSortDirection;
        this.DefaultSecondSortDirection = defaultSecondSortDirection;

        this.CreateTagImplicationConnections();
    }

    public string DefaultSeasonTitle { get; set; }
    public string DefaultSeasonOriginalTitle { get; set; }

    public List<Kind> DefaultKinds { get; set; }
    public List<Tag> DefaultTags { get; set; }

    public CultureInfo DefaultCultureInfo { get; set; }

    public ListSortOrder DefaultFirstSortOrder { get; set; }
    public ListSortOrder DefaultSecondSortOrder { get; set; }

    public ListSortDirection DefaultFirstSortDirection { get; set; }
    public ListSortDirection DefaultSecondSortDirection { get; set; }

    private void CreateTagImplicationConnections()
    {
        var tagsByNameCategory = this.DefaultTags
            .ToDictionary(tag => (tag.Name, tag.Category), tag => tag);

        foreach (var tag in this.DefaultTags)
        {
            tag.ImpliedTags = tag.ImpliedTags
                .Select(tag => tagsByNameCategory[(tag.Name, tag.Category)])
                .ToHashSet();
        }
    }
}

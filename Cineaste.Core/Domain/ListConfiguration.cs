namespace Cineaste.Core.Domain;

using System.Globalization;

public sealed class ListConfiguration : Entity<ListConfiguration>
{
    private CultureInfo culture;
    private string defaultSeasonTitle;
    private string defaultSeasonOriginalTitle;

    public CultureInfo Culture
    {
        get => this.culture;

        [MemberNotNull(nameof(culture))]
        set => this.culture = Require.NotNull(value);
    }

    public string DefaultSeasonTitle
    {
        get => this.defaultSeasonTitle;

        [MemberNotNull(nameof(defaultSeasonTitle))]
        set => this.defaultSeasonTitle = Require.NotNull(value);
    }

    public string DefaultSeasonOriginalTitle
    {
        get => this.defaultSeasonOriginalTitle;

        [MemberNotNull(nameof(defaultSeasonOriginalTitle))]
        set => this.defaultSeasonOriginalTitle = Require.NotNull(value);
    }

    public ListSortingConfiguration SortingConfiguration { get; private set; }

    public ListConfiguration(
        Id<ListConfiguration> id,
        CultureInfo culture,
        string defaultSeasonTitle,
        string defaultSeasonOriginalTitle,
        ListSortingConfiguration sortingConfiguration)
        : base(id)
    {
        this.Culture = culture;
        this.DefaultSeasonTitle = defaultSeasonTitle;
        this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
        this.SortingConfiguration = Require.NotNull(sortingConfiguration);
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private ListConfiguration(Id<ListConfiguration> id)
        : base(id)
    {
        this.culture = null!;
        this.defaultSeasonTitle = null!;
        this.defaultSeasonOriginalTitle = null!;
        this.SortingConfiguration = null!;
    }
}

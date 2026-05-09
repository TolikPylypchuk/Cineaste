namespace Cineaste.Core.Domain;

public sealed record TagContainer
{
    public required Tag Tag { get; init; }
}

public sealed class Tag : Entity<Tag>
{
    private readonly List<Tag> impliedTags;
    private readonly List<Tag> implyingTags;

    public string Name
    {
        get;
        set => field = Require.NotBlank(value);
    }

    public TagCategory Category { get; set; }

    public string? Description
    {
        get;
        set => field = String.IsNullOrEmpty(value) ? null : value.Trim();
    }

    public Color Color { get; set; }

    public bool IsApplicableToMovies { get; set; } = true;
    public bool IsApplicableToSeries { get; set; } = true;

    public IReadOnlyCollection<Tag> ImpliedTags =>
        this.impliedTags.AsReadOnly();

    public IReadOnlyCollection<Tag> ImplyingTags =>
        this.implyingTags.AsReadOnly();

    public Tag(
        Id<Tag> id,
        string name,
        TagCategory category,
        string? description,
        Color color,
        bool isApplicableToMovies,
        bool isApplicableToSeries)
        : base(id)
    {
        this.Name = name;
        this.Category = category;
        this.Description = description;
        this.Color = color;
        this.IsApplicableToMovies = isApplicableToMovies;
        this.IsApplicableToSeries = isApplicableToSeries;
        this.impliedTags = [];
        this.implyingTags = [];
    }

    public void AddImplication(Tag impliedTag)
    {
        this.impliedTags.Add(impliedTag);
        impliedTag.implyingTags.Add(this);
    }
}

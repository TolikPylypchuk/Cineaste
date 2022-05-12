namespace Cineaste.Core.Domain;

public sealed record TagContainer
{
    public Tag Tag { get; init; } = null!;
}

public sealed class Tag : Entity<Tag>
{
    private string name;
    private TagCategory category;
    private string? description;
    private Color color;

    private readonly List<Tag> impliedTags = new();
    private readonly List<Tag> implyingTags = new();

    public string Name
    {
        get => this.name;

        [MemberNotNull(nameof(name))]
        set => this.name = Require.NotBlank(value);
    }

    public TagCategory Category
    {
        get => this.category;

        [MemberNotNull(nameof(category))]
        set => this.category = Require.NotNull(value);
    }

    public string? Description
    {
        get => this.description;
        set => this.description = String.IsNullOrEmpty(value) ? null : value.Trim();
    }

    public Color Color
    {
        get => this.color;

        [MemberNotNull(nameof(color))]
        set => this.color = Require.NotNull(value);
    }

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
    }

    public void AddImplication(Tag impliedTag)
    {
        this.impliedTags.Add(impliedTag);
        impliedTag.implyingTags.Add(this);
    }
}

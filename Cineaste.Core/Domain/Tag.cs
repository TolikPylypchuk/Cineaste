namespace Cineaste.Core.Domain;

public sealed class Tag : Entity<Tag>
{
    private string name;
    private TagCategory category;
    private string description;
    private Color color;

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

    public string Description
    {
        get => this.description;

        [MemberNotNull(nameof(description))]
        set => this.description = Require.NotBlank(value);
    }

    public Color Color
    {
        get => this.color;

        [MemberNotNull(nameof(color))]
        set => this.color = Require.NotNull(value);
    }

    public bool IsApplicableToMovies { get; set; } = true;
    public bool IsApplicableToSeries { get; set; } = true;

    public Tag(
        Id<Tag> id,
        string name,
        TagCategory category,
        string description,
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
}

namespace Cineaste.Core.Domain;

public sealed class Tag : DomainObject
{
    public static readonly string DefaultNewTagColor = "#FFFFFFFF";

    public string Name { get; set; } = String.Empty;
    public string Category { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public string Color { get; set; } = DefaultNewTagColor;

    public bool IsApplicableToMovies { get; set; } = true;
    public bool IsApplicableToSeries { get; set; } = true;

    public override string ToString() =>
        $"Tag #{this.Id}: {this.Name} ({this.Category})";
}

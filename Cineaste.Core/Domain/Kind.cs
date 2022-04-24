namespace Cineaste.Core.Domain;

public sealed class Kind : DomainObject
{
    public static readonly string DefaultNewKindColor = "#FF000000";

    public string Name { get; set; } = String.Empty;

    public string ColorForWatchedMovie { get; set; } = DefaultNewKindColor;
    public string ColorForWatchedSeries { get; set; } = DefaultNewKindColor;

    public string ColorForNotWatchedMovie { get; set; } = DefaultNewKindColor;
    public string ColorForNotWatchedSeries { get; set; } = DefaultNewKindColor;

    public string ColorForNotReleasedMovie { get; set; } = DefaultNewKindColor;
    public string ColorForNotReleasedSeries { get; set; } = DefaultNewKindColor;

    public override string ToString() =>
        $"Kind #{this.Id}: {this.Name}";
}

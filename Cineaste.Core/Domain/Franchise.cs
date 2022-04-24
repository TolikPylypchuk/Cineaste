namespace Cineaste.Core.Domain;

public sealed class Franchise : DomainObject
{
    public bool ShowTitles { get; set; }
    public bool IsLooselyConnected { get; set; }
    public bool MergeDisplayNumbers { get; set; }

    public string? PosterUrl { get; set; }

    public FranchiseItem? ParentFranchiseItem { get; set; }

    public List<FranchiseItem> Children { get; set; } = new();

    public List<Title> Titles { get; set; } = new();

    public MovieSeriesList OwnerList { get; set; } = null!;

    public List<Title> ActualTitles =>
        this.Titles.Count != 0
            ? this.Titles
            : this.Children.OrderBy(e => e.SequenceNumber).FirstOrDefault()?.Titles ?? new();

    public Title? Title =>
        this.Titles
            .Where(title => !title.IsOriginal)
            .OrderBy(title => title.Priority)
            .FirstOrDefault();

    public Title? OriginalTitle =>
        this.Titles
            .Where(title => title.IsOriginal)
            .OrderBy(title => title.Priority)
            .FirstOrDefault();

    public override string ToString() =>
        $"Franchise #{this.Id}: {Title.ToString(this.ActualTitles)}";
}

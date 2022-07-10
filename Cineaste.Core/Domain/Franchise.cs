namespace Cineaste.Core.Domain;

public sealed class Franchise : Entity<Franchise>
{
    private readonly List<FranchiseItem> children;
    private readonly List<Title> titles;

    public bool ShowTitles { get; set; }
    public bool IsLooselyConnected { get; set; }
    public bool MergeDisplayNumbers { get; set; }

    public Poster? Poster { get; set; }

    public FranchiseItem? FranchiseItem { get; set; }

    public IReadOnlyCollection<FranchiseItem> Children =>
        this.children.AsReadOnly();

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

    public IReadOnlyCollection<Title> ActualTitles =>
        this.Titles.Count != 0
            ? this.Titles
            : this.Children.OrderBy(item => item.SequenceNumber).FirstOrDefault()?.Titles
                ?? new List<Title>().AsReadOnly();

    public Title? Title =>
        this.ActualTitles
            .Where(title => !title.IsOriginal)
            .OrderBy(title => title.Priority)
            .FirstOrDefault();

    public Title? OriginalTitle =>
        this.ActualTitles
            .Where(title => title.IsOriginal)
            .OrderBy(title => title.Priority)
            .FirstOrDefault();

    public Franchise(
        Id<Franchise> id,
        IEnumerable<Title> titles,
        IEnumerable<FranchiseItem> children,
        bool showTitles,
        bool isLooselyConnected,
        bool mergeDisplayNumbers)
        : base(id)
    {
        this.ShowTitles = showTitles;
        this.IsLooselyConnected = isLooselyConnected;
        this.MergeDisplayNumbers = mergeDisplayNumbers;

        this.children = children.ToList();
        this.titles = titles.ToList();
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private Franchise(Id<Franchise> id)
        : base(id)
    {
        this.titles = new();
        this.children = new();
    }

    public Title AddTitle(string name, bool isOriginal)
    {
        int priority = this.titles
            .Where(title => title.IsOriginal == isOriginal)
            .Max(title => title.Priority) + 1;

        var title = new Title(name, priority, isOriginal);

        this.titles.Add(title);

        return title;
    }

    public void RemoveTitle(string name, bool isOriginal) =>
        this.titles.RemoveAll(title => title.Name == name && title.IsOriginal == isOriginal);

    public FranchiseItem AddMovie(Movie movie)
    {
        var item = new FranchiseItem(Domain.Id.CreateNew<FranchiseItem>(), movie, this, this.Children.Count + 1, true);

        this.children.Add(item);
        movie.FranchiseItem = item;

        return item;
    }

    public FranchiseItem AddSeries(Series series)
    {
        var item = new FranchiseItem(Domain.Id.CreateNew<FranchiseItem>(), series, this, this.Children.Count + 1, true);

        this.children.Add(item);
        series.FranchiseItem = item;

        return item;
    }

    public FranchiseItem AddFranchise(Franchise franchise)
    {
        var item = new FranchiseItem(Domain.Id.CreateNew<FranchiseItem>(), franchise, this, this.Children.Count + 1, true);

        this.children.Add(item);
        franchise.FranchiseItem = item;

        return item;
    }

    public void RemoveMovie(Movie movie)
    {
        if (movie.FranchiseItem is not null && this.children.Contains(movie.FranchiseItem))
        {
            this.children.Remove(movie.FranchiseItem);
            movie.FranchiseItem = null;
        }
    }

    public void RemoveSeries(Series series)
    {
        if (series.FranchiseItem is not null && this.children.Contains(series.FranchiseItem))
        {
            this.children.Remove(series.FranchiseItem);
            series.FranchiseItem = null;
        }
    }

    public void RemoveSeries(Franchise franchise)
    {
        if (franchise.FranchiseItem is not null && this.children.Contains(franchise.FranchiseItem))
        {
            this.children.Remove(franchise.FranchiseItem);
            franchise.FranchiseItem = null;
        }
    }
}

namespace Cineaste.Core.Domain;

public sealed class Franchise : FranchiseItemEntity<Franchise>
{
    private readonly List<FranchiseItem> children;

    public bool ShowTitles { get; set; }
    public bool IsLooselyConnected { get; set; }
    public bool ContinueNumbering { get; set; }

    public Poster? Poster { get; set; }

    public IReadOnlyCollection<FranchiseItem> Children =>
        this.children.AsReadOnly();

    public IReadOnlyCollection<Title> ActualTitles =>
        this.Titles.Count != 0
            ? this.Titles
            : this.Children.MinBy(item => item.SequenceNumber)?.Titles
                ?? new List<Title>().AsReadOnly();

    public Title? ActualTitle =>
        this.ActualTitles
            .Where(title => !title.IsOriginal)
            .MinBy(title => title.Priority);

    public Title? ActualOriginalTitle =>
        this.ActualTitles
            .Where(title => title.IsOriginal)
            .MinBy(title => title.Priority);

    public int? StartYear =>
        this.Children.Min(item => item.Select(
            movie => movie.Year, series => series.StartYear, franchise => franchise.StartYear));

    public int? EndYear =>
        this.Children.Max(item => item.Select(
            movie => movie.Year, series => series.EndYear, franchise => franchise.EndYear));

    public Franchise(
        Id<Franchise> id,
        IEnumerable<Title> titles,
        bool showTitles,
        bool isLooselyConnected,
        bool continueNumbering)
        : base(id, titles)
    {
        this.ShowTitles = showTitles;
        this.IsLooselyConnected = isLooselyConnected;
        this.ContinueNumbering = continueNumbering;

        this.children = [];
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private Franchise(Id<Franchise> id)
        : base(id) =>
        this.children = [];

    public FranchiseItem? FindMovie(Movie movie) =>
        this.Children.FirstOrDefault(item => item.Movie == movie);

    public FranchiseItem? FindSeries(Series series) =>
        this.Children.FirstOrDefault(item => item.Series == series);

    public FranchiseItem? FindFranchise(Franchise franchise) =>
        this.Children.FirstOrDefault(item => item.Franchise == franchise);

    public FranchiseItem AddMovie(Movie movie, bool addDisplayNumber)
    {
        var item = new FranchiseItem(
            Domain.Id.Create<FranchiseItem>(),
            movie,
            this,
            this.Children.Count + 1,
            addDisplayNumber ? this.GetMaxDisplayNumber() + 1 : null);

        this.children.Add(item);
        movie.FranchiseItem = item;

        return item;
    }

    public FranchiseItem AddSeries(Series series, bool addDisplayNumber)
    {
        var item = new FranchiseItem(
            Domain.Id.Create<FranchiseItem>(),
            series,
            this,
            this.Children.Count + 1,
            addDisplayNumber ? this.GetMaxDisplayNumber() + 1 : null);

        this.children.Add(item);
        series.FranchiseItem = item;

        return item;
    }

    public FranchiseItem AddFranchise(Franchise franchise, bool addDisplayNumber)
    {
        var item = new FranchiseItem(
            Domain.Id.Create<FranchiseItem>(),
            franchise,
            this,
            this.Children.Count + 1,
            addDisplayNumber ? this.GetMaxDisplayNumber() + 1 : null);

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

            this.SetDisplayNumbersForChildren();
        }
    }

    public void RemoveSeries(Series series)
    {
        if (series.FranchiseItem is not null && this.children.Contains(series.FranchiseItem))
        {
            this.children.Remove(series.FranchiseItem);
            series.FranchiseItem = null;

            this.SetDisplayNumbersForChildren();
        }
    }

    public void RemoveFranchise(Franchise franchise)
    {
        if (franchise.FranchiseItem is not null && this.children.Contains(franchise.FranchiseItem))
        {
            this.children.Remove(franchise.FranchiseItem);
            franchise.FranchiseItem = null;

            this.SetDisplayNumbersForChildren();
        }
    }

    public void SetDisplayNumbersForChildren()
    {
        int number = 1;

        foreach (var item in this.children.OrderBy(item => item.SequenceNumber))
        {
            if (item.DisplayNumber is not null)
            {
                item.DisplayNumber = number++;
            }
        }
    }

    protected override void ValidateTitles(IReadOnlyCollection<string> names, string paramName)
    {
        if (this.ShowTitles && names.Count == 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "The list of title names is empty");
        }
    }

    private int GetMaxDisplayNumber() =>
        this.Children
            .Select(item => item.DisplayNumber)
            .Where(num => num is not null)
            .Max()
            ?? 0;
}

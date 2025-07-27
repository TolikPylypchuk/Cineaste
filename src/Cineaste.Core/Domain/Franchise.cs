namespace Cineaste.Core.Domain;

public sealed class Franchise : FranchiseItemEntity<Franchise>
{
    private MovieKind movieKind;
    private SeriesKind seriesKind;
    private FranchiseKindSource kindSource;
    private readonly List<FranchiseItem> children;

    public bool ShowTitles { get; set; }
    public bool IsLooselyConnected { get; set; }
    public bool ContinueNumbering { get; set; }

    public MovieKind MovieKind
    {
        get => this.movieKind;

        [MemberNotNull(nameof(this.movieKind))]
        set => this.movieKind = Require.NotNull(value);
    }

    public SeriesKind SeriesKind
    {
        get => seriesKind;

        [MemberNotNull(nameof(this.seriesKind))]
        set => seriesKind = Require.NotNull(value);
    }

    public FranchiseKindSource KindSource
    {
        get => kindSource;
        set => kindSource = Require.ValidEnum(value);
    }

    public Poster? Poster { get; set; }

    public IReadOnlyCollection<FranchiseItem> Children =>
        [.. this.children.OrderBy(item => item.SequenceNumber)];

    public int? StartYear =>
        this.Children.Min(item => item.Select(
            movie => movie.Year, series => series.StartYear, franchise => franchise.StartYear));

    public int? EndYear =>
        this.Children.Max(item => item.Select(
            movie => movie.Year, series => series.EndYear, franchise => franchise.EndYear));

    public Franchise(
        Id<Franchise> id,
        IEnumerable<Title> titles,
        MovieKind movieKind,
        SeriesKind seriesKind,
        FranchiseKindSource kindSource,
        bool showTitles,
        bool isLooselyConnected,
        bool continueNumbering)
        : base(id, titles)
    {
        this.MovieKind = movieKind;
        this.SeriesKind = seriesKind;
        this.KindSource = kindSource;
        this.ShowTitles = showTitles;
        this.IsLooselyConnected = isLooselyConnected;
        this.ContinueNumbering = continueNumbering;

        this.children = [];
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private Franchise(Id<Franchise> id)
        : base(id)
    {
        this.children = [];
        this.movieKind = null!;
        this.seriesKind = null!;
    }

    public FranchiseItem? FindMovie(Movie movie) =>
        this.Children.FirstOrDefault(item => item.Movie == movie);

    public FranchiseItem? FindSeries(Series series) =>
        this.Children.FirstOrDefault(item => item.Series == series);

    public FranchiseItem? FindFranchise(Franchise franchise) =>
        this.Children.FirstOrDefault(item => item.Franchise == franchise);

    public FranchiseItem AttachMovie(Movie movie, bool addDisplayNumber)
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

    public FranchiseItem AttachSeries(Series series, bool addDisplayNumber)
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

    public FranchiseItem AttachFranchise(Franchise franchise, bool addDisplayNumber)
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

    public void DetachMovie(Movie movie)
    {
        if (movie.FranchiseItem is not null && this.children.Contains(movie.FranchiseItem))
        {
            this.children.Remove(movie.FranchiseItem);
            movie.FranchiseItem = null;

            this.SetSequenceNumbersForChildren();
        }
    }

    public void DetachSeries(Series series)
    {
        if (series.FranchiseItem is not null && this.children.Contains(series.FranchiseItem))
        {
            this.children.Remove(series.FranchiseItem);
            series.FranchiseItem = null;

            this.SetSequenceNumbersForChildren();
        }
    }

    public void DetachFranchise(Franchise franchise)
    {
        if (franchise.FranchiseItem is not null && this.children.Contains(franchise.FranchiseItem))
        {
            this.children.Remove(franchise.FranchiseItem);
            franchise.FranchiseItem = null;

            this.SetSequenceNumbersForChildren();
        }
    }

    public void DetachAllChildren()
    {
        foreach (var item in this.children)
        {
            item.Do(
                movie =>
                {
                    movie.FranchiseItem = null;
                    movie.ListItem!.SetProperties(movie);
                },
                series =>
                {
                    series.FranchiseItem = null;
                    series.ListItem!.SetProperties(series);
                },
                franchise =>
                {
                    franchise.FranchiseItem = null;
                    franchise.ListItem!.SetProperties(franchise);
                });
        }

        this.children.Clear();
    }

    public void SetSequenceNumbersForChildren()
    {
        int sequenceNumber = 1;
        int displayNumber = 1;

        foreach (var item in this.children.OrderBy(item => item.SequenceNumber).ToList())
        {
            item.SequenceNumber = sequenceNumber++;

            if (item.DisplayNumber is not null)
            {
                item.DisplayNumber = displayNumber++;
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

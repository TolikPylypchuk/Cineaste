namespace Cineaste.Client.FormModels;

public sealed class MovieFormModel : TitledFormModel<MovieModel, MovieRequest>
{
    private readonly ListKindModel defaultKind;

    public int Year { get; set; }

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

    public ListKindModel Kind { get; set; }

    public string ImdbId { get; set; } = String.Empty;
    public string RottenTomatoesLink { get; set; } = String.Empty;

    public MovieFormModel(IReadOnlyCollection<ListKindModel> availableKinds)
    {
        ArgumentNullException.ThrowIfNull(availableKinds);

        if (availableKinds.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(availableKinds), $"{nameof(availableKinds)} is empty");
        }

        this.defaultKind = availableKinds.First();
        this.Kind = this.defaultKind;
    }

    public MovieRequest ToMovieRequest(Guid listId) =>
        new(
            listId,
            this.ToTitleRequests(this.Titles),
            this.ToTitleRequests(this.OriginalTitles),
            this.Year,
            this.IsWatched,
            this.IsReleased,
            this.Kind.Id,
            this.ImdbId,
            this.RottenTomatoesLink);

    protected override void CopyFromModel()
    {
        var movie = this.BackingModel;

        this.CopyTitles(movie);

        this.Year = movie?.Year ?? DateTime.Now.Year;
        this.IsWatched = movie?.IsWatched ?? false;
        this.IsReleased = movie?.IsReleased ?? true;
        this.Kind = movie?.Kind ?? this.defaultKind;
    }
}

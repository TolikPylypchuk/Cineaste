namespace Cineaste.Client.FormModels;

public sealed class MovieFormModel : TitledFormModelBase<MovieRequest, MovieModel>
{
    private readonly Guid listId;
    private readonly ListKindModel defaultKind;

    public int Year { get; set; }

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

    public ListKindModel Kind { get; set; }

    public string ImdbId { get; set; } = String.Empty;
    public string RottenTomatoesId { get; set; } = String.Empty;

    public Guid? ParentFranchiseId { get; private set; }

    public int? SequenceNumber { get; private set; }

    public bool IsFirst { get; private set; }
    public bool IsLast { get; private set; }

    public MovieFormModel(Guid listId, IReadOnlyCollection<ListKindModel> availableKinds)
    {
        ArgumentNullException.ThrowIfNull(availableKinds);

        if (availableKinds.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(availableKinds), $"{nameof(availableKinds)} is empty");
        }

        this.listId = listId;
        this.defaultKind = availableKinds.First();
        this.Kind = this.defaultKind;

        this.FinishInitialization();
    }

    public override MovieRequest CreateRequest() =>
        new(
            this.listId,
            this.ToTitleRequests(this.Titles),
            this.ToTitleRequests(this.OriginalTitles),
            this.Year,
            this.IsWatched,
            this.IsReleased,
            this.Kind.Id,
            this.ImdbId,
            this.RottenTomatoesId);

    protected override void CopyFromModel()
    {
        var movie = this.BackingModel;

        this.CopyTitles(movie);

        this.Year = movie?.Year ?? DateTime.Now.Year;
        this.IsWatched = movie?.IsWatched ?? false;
        this.IsReleased = movie?.IsReleased ?? true;
        this.Kind = movie?.Kind ?? this.defaultKind;
        this.ImdbId = movie?.ImdbId ?? String.Empty;
        this.RottenTomatoesId = movie?.RottenTomatoesId ?? String.Empty;

        this.ParentFranchiseId = movie?.ParentFranchiseId;
        this.SequenceNumber = movie?.SequenceNumber;
        this.IsFirst = movie?.IsFirstInFranchise ?? false;
        this.IsLast = movie?.IsLastInFranchise ?? false;
    }
}

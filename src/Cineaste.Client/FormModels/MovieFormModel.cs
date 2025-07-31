namespace Cineaste.Client.FormModels;

public sealed class MovieFormModel : TitledFormModelBase<MovieRequest, MovieModel>
{
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

    public MovieFormModel(ListKindModel kind, Guid? parentFranchiseId)
    {
        ArgumentNullException.ThrowIfNull(kind);

        this.defaultKind = kind;
        this.Kind = this.defaultKind;
        this.ParentFranchiseId = parentFranchiseId;

        this.FinishInitialization();
    }

    public override MovieRequest CreateRequest() =>
        new(
            this.ToTitleRequests(this.Titles),
            this.ToTitleRequests(this.OriginalTitles),
            this.Year,
            this.IsWatched,
            this.IsReleased,
            this.Kind.Id,
            this.ImdbId,
            this.RottenTomatoesId,
            this.ParentFranchiseId);

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

        if (movie?.FranchiseItem is { } franchiseItem)
        {
            this.ParentFranchiseId = franchiseItem.ParentFranchiseId;
            this.SequenceNumber = franchiseItem.SequenceNumber;
            this.IsFirst = franchiseItem.IsFirstInFranchise;
            this.IsLast = franchiseItem.IsLastInFranchise;
        }
    }
}

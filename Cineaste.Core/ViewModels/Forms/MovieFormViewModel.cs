namespace Cineaste.Core.ViewModels.Forms;

public sealed class MovieFormViewModel : TaggedFormBase<Movie, MovieFormViewModel>
{
    private readonly IEntityService<Movie> movieService;

    public MovieFormViewModel(
        Movie movie,
        ReadOnlyObservableCollection<Kind> kinds,
        ReadOnlyObservableCollection<Tag> tags,
        string fileName,
        ResourceManager? resourceManager = null,
        IScheduler? scheduler = null,
        IEntityService<Movie>? movieService = null)
        : base(movie.Entry, tags, resourceManager, scheduler)
    {
        this.Movie = movie;
        this.Kinds = kinds;

        this.movieService = movieService ?? GetDefaultService<IEntityService<Movie>>(fileName);

        this.CopyProperties();

        this.ValidationRule(vm => vm.Year, MovieMinYear, MovieMaxYear);
        this.ValidationRule(vm => vm.ImdbLink, link => link.IsUrl(), "ImdbLinkInvalid");
        this.ValidationRule(vm => vm.RottenTomatoesLink, link => link.IsUrl(), "RottenTomatoesLinkInvalid");
        this.ValidationRule(vm => vm.PosterUrl, url => url.IsUrl(), "PosterUrlInvalid");

        this.InitializeValueDependencies();
        this.CanDeleteWhenNotChanged();
        this.CanCreateFranchise();
        this.EnableChangeTracking();
    }

    public Movie Movie { get; }

    public ReadOnlyObservableCollection<Kind> Kinds { get; }

    [Reactive]
    public int Year { get; set; }

    [Reactive]
    public Kind Kind { get; set; }

    [Reactive]
    public bool IsWatched { get; set; }

    [Reactive]
    public bool IsReleased { get; set; }

    [Reactive]
    public string ImdbLink { get; set; } = String.Empty;

    [Reactive]
    public string RottenTomatoesLink { get; set; } = String.Empty;

    [Reactive]
    public string PosterUrl { get; set; } = String.Empty;

    public override bool IsNew =>
        this.Movie.Id == default;

    protected override MovieFormViewModel Self => this;

    protected override ICollection<Title> ItemTitles =>
        this.Movie.Titles;

    protected override IEnumerable<Tag> ItemTags =>
        this.Movie.Tags;

    protected override string NewItemKey => "NewMovie";

    protected override void EnableChangeTracking()
    {
        this.TrackChanges(vm => vm.Year, vm => vm.Movie.Year);
        this.TrackChanges(vm => vm.Kind, vm => vm.Movie.Kind);
        this.TrackChanges(vm => vm.IsWatched, vm => vm.Movie.IsWatched);
        this.TrackChanges(vm => vm.IsReleased, vm => vm.Movie.IsReleased);
        this.TrackChanges(vm => vm.ImdbLink, vm => vm.Movie.ImdbLink.EmptyIfNull());
        this.TrackChanges(vm => vm.RottenTomatoesLink, vm => vm.Movie.RottenTomatoesLink.EmptyIfNull());
        this.TrackChanges(vm => vm.PosterUrl, vm => vm.Movie.PosterUrl.EmptyIfNull());

        base.EnableChangeTracking();
    }

    protected override IObservable<Movie> OnSave()
        => this.SaveTitles()
            .Select(() =>
            {
                this.Movie.IsWatched = this.IsWatched;
                this.Movie.IsReleased = this.IsReleased;
                this.Movie.Year = this.Year;
                this.Movie.Kind = this.Kind;
                this.Movie.ImdbLink = this.ImdbLink.NullIfEmpty();
                this.Movie.RottenTomatoesLink = this.RottenTomatoesLink.NullIfEmpty();
                this.Movie.PosterUrl = this.PosterUrl.NullIfEmpty();

                this.Movie.Tags.Clear();

                foreach (var tag in this.TagsSource.Items)
                {
                    this.Movie.Tags.Add(tag);
                }

                return this.Movie;
            })
            .DoAsync(this.movieService.SaveInTaskPool);

    protected override IObservable<Movie?> OnDelete() =>
        Dialog.PromptToDelete(
            "DeleteMovie", () => this.movieService.DeleteInTaskPool(this.Movie).Select(() => this.Movie));

    [MemberNotNull(nameof(Kind))]
    protected override void CopyProperties()
    {
        base.CopyProperties();

        this.Year = this.Movie.Year;
        this.Kind = this.Movie.Kind;
        this.IsWatched = this.Movie.IsWatched;
        this.IsReleased = this.Movie.IsReleased;
        this.ImdbLink = this.Movie.ImdbLink.EmptyIfNull();
        this.RottenTomatoesLink = this.Movie.RottenTomatoesLink.EmptyIfNull();
        this.PosterUrl = this.Movie.PosterUrl.EmptyIfNull();
    }

    protected override void AttachTitle(Title title) =>
        title.Movie = this.Movie;

    protected override bool IsTagApplicable(Tag tag) =>
        tag.IsApplicableToMovies;

    private void InitializeValueDependencies()
    {
        this.WhenAnyValue(vm => vm.IsReleased)
            .Where(isReleased => !isReleased)
            .Subscribe(_ => this.IsWatched = false);

        this.WhenAnyValue(vm => vm.IsWatched)
            .Where(isWatched => isWatched)
            .Subscribe(_ => this.IsReleased = true);

        this.WhenAnyValue(vm => vm.Year)
            .Where(year => MovieMinYear <= year && year <= MovieMaxYear)
            .Where(year => year != this.Scheduler.Now.Year)
            .Subscribe(year => this.IsReleased = year < this.Scheduler.Now.Year);
    }
}

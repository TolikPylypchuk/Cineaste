namespace Cineaste.Core.ViewModels.Forms;

public sealed class SpecialEpisodeFormViewModel
    : SeriesComponentFormBase<SpecialEpisode, SpecialEpisodeFormViewModel>
{
    public SpecialEpisodeFormViewModel(
        SpecialEpisode episode,
        SeriesFormViewModel parent,
        IObservable<int> maxSequenceNumber,
        ResourceManager? resourceManager,
        IScheduler? scheduler = null)
        : base(parent, maxSequenceNumber, resourceManager, scheduler)
    {
        this.SpecialEpisode = episode;
        this.CopyProperties();

        this.ValidationRule(vm => vm.Channel, channel => !String.IsNullOrWhiteSpace(channel), "ChannelEmpty");
        this.ValidationRule(vm => vm.Year, SeriesMinYear, SeriesMaxYear);
        this.ValidationRule(vm => vm.RottenTomatoesLink, link => link.IsUrl(), "RottenTomatoesLinkInvalid");
        this.ValidationRule(vm => vm.PosterUrl, url => url.IsUrl(), "PosterUrlInvalid");

        this.InitializeValueDependencies();
        this.CanAlwaysDelete();
        this.EnableChangeTracking();
    }

    public SpecialEpisode SpecialEpisode { get; }

    [Reactive]
    public int Month { get; set; }

    [Reactive]
    public int Year { get; set; }

    [Reactive]
    public bool IsWatched { get; set; }

    [Reactive]
    public bool IsReleased { get; set; }

    [Reactive]
    public override string Channel { get; set; } = String.Empty;

    [Reactive]
    public string? RottenTomatoesLink { get; set; }

    [Reactive]
    public string? PosterUrl { get; set; }

    public override bool IsNew =>
        this.SpecialEpisode.Id == default;

    protected override SpecialEpisodeFormViewModel Self => this;

    protected override ICollection<Title> ItemTitles =>
        this.SpecialEpisode.Titles;

    protected override string NewItemKey => "NewSpecialEpisode";

    public override int GetNextYear() =>
        this.Year + 1;

    protected override void EnableChangeTracking()
    {
        this.TrackChanges(vm => vm.Month, vm => vm.SpecialEpisode.Month);
        this.TrackChanges(vm => vm.Year, vm => vm.SpecialEpisode.Year);
        this.TrackChanges(vm => vm.IsWatched, vm => vm.SpecialEpisode.IsWatched);
        this.TrackChanges(vm => vm.IsReleased, vm => vm.SpecialEpisode.IsReleased);
        this.TrackChanges(vm => vm.Channel, vm => vm.SpecialEpisode.Channel);
        this.TrackChanges(vm => vm.SequenceNumber, vm => vm.SpecialEpisode.SequenceNumber);
        this.TrackChanges(vm => vm.RottenTomatoesLink, vm => vm.SpecialEpisode.RottenTomatoesLink.EmptyIfNull());
        this.TrackChanges(vm => vm.PosterUrl, vm => vm.SpecialEpisode.PosterUrl.EmptyIfNull());

        base.EnableChangeTracking();
    }

    protected override IObservable<SpecialEpisode> OnSave() =>
        this.SaveTitles()
            .Select(() =>
            {
                this.SpecialEpisode.Month = this.Month;
                this.SpecialEpisode.Year = this.Year;
                this.SpecialEpisode.IsWatched = this.IsWatched;
                this.SpecialEpisode.IsReleased = this.IsReleased;
                this.SpecialEpisode.Channel = this.Channel;
                this.SpecialEpisode.SequenceNumber = this.SequenceNumber;
                this.SpecialEpisode.RottenTomatoesLink = this.RottenTomatoesLink.NullIfEmpty();
                this.SpecialEpisode.PosterUrl = this.PosterUrl.NullIfEmpty();

                return this.SpecialEpisode;
            });

    protected override IObservable<SpecialEpisode?> OnDelete() =>
        Dialog.PromptToDelete("DeleteSpecialEpisode", () => Observable.Return(this.SpecialEpisode));

    protected override void CopyProperties()
    {
        base.CopyProperties();

        this.Month = this.SpecialEpisode.Month;
        this.Year = this.SpecialEpisode.Year;
        this.IsWatched = this.SpecialEpisode.IsWatched;
        this.IsReleased = this.SpecialEpisode.IsReleased;
        this.Channel = this.SpecialEpisode.Channel;
        this.SequenceNumber = this.SpecialEpisode.SequenceNumber;
        this.RottenTomatoesLink = this.SpecialEpisode.RottenTomatoesLink.EmptyIfNull();
        this.PosterUrl = this.SpecialEpisode.PosterUrl.EmptyIfNull();
    }

    protected override void AttachTitle(Title title) =>
        title.SpecialEpisode = this.SpecialEpisode;

    private void InitializeValueDependencies()
    {
        this.WhenAnyValue(vm => vm.IsReleased)
            .Where(isReleased => !isReleased)
            .Subscribe(_ => this.IsWatched = false);

        this.WhenAnyValue(vm => vm.IsWatched)
            .Where(isWatched => isWatched)
            .Subscribe(_ => this.IsReleased = true);

        this.WhenAnyValue(vm => vm.Year)
            .Where(year => SeriesMinYear <= year && year <= SeriesMaxYear)
            .CombineLatest(this.WhenAnyValue(vm => vm.Month), (year, month) => new DateTime(year, month, 1))
            .Where(date => date.Year != this.Scheduler.Now.Year ||
                date.Year == this.Scheduler.Now.Year && date.Month != this.Scheduler.Now.Month)
            .Subscribe(date => this.IsReleased =
                date.Year < this.Scheduler.Now.Year ||
                date.Year == this.Scheduler.Now.Year && date.Month < this.Scheduler.Now.Month);
    }
}

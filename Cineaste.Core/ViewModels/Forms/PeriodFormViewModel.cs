namespace Cineaste.Core.ViewModels.Forms;

public sealed class PeriodFormViewModel : ReactiveForm<Period, PeriodFormViewModel>
{
    private const string YearProperty = "Year";

    public PeriodFormViewModel(
        Period period,
        IObservable<bool> canDelete,
        ResourceManager? resourceManager,
        IScheduler? scheduler = null)
        : base(resourceManager, scheduler)
    {
        this.Period = period;
        this.CopyProperties();

        this.ValidationRule(vm => vm.StartYear, SeriesMinYear, SeriesMaxYear, YearProperty);
        this.ValidationRule(vm => vm.EndYear, SeriesMinYear, SeriesMaxYear, YearProperty);
        this.ValidationRule(vm => vm.NumberOfEpisodes, PeriodMinNumberOfEpisodes, PeriodMaxNumberOfEpisodes);
        this.ValidationRule(vm => vm.RottenTomatoesLink, link => link.IsUrl(), "RottenTomatoesLinkInvalid");
        this.ValidationRule(vm => vm.PosterUrl, url => url.IsUrl(), "PosterUrlInvalid");

        var periodValid =
            this.WhenAnyValue(
                vm => vm.StartMonth,
                vm => vm.StartYear,
                vm => vm.EndMonth,
                vm => vm.EndYear,
                (int startMonth, int startYear, int endMonth, int endYear) =>
                    startYear < endYear || startYear == endYear && startMonth <= endMonth)
                .Throttle(TimeSpan.FromMilliseconds(250), this.Scheduler)
                .ObserveOn(RxApp.MainThreadScheduler);

        this.ValidPeriodRule = this.ValidationRule(
            periodValid, this.ResourceManager.GetString("ValidationPeriodInvalid") ?? String.Empty);

        this.CanDeleteWhen(canDelete);

        this.InitializeValueDependencies();

        this.EnableChangeTracking();
    }

    public Period Period { get; }

    [Reactive]
    public int StartMonth { get; set; }

    [Reactive]
    public int StartYear { get; set; }

    [Reactive]
    public int EndMonth { get; set; }

    [Reactive]
    public int EndYear { get; set; }

    [Reactive]
    public int NumberOfEpisodes { get; set; }

    [Reactive]
    public bool IsSingleDayRelease { get; set; }

    [Reactive]
    public string RottenTomatoesLink { get; set; } = String.Empty;

    [Reactive]
    public string PosterUrl { get; set; } = String.Empty;

    [Reactive]
    public bool ShowPosterUrl { get; set; } = true;

    public ValidationHelper ValidPeriodRule { get; }

    public override bool IsNew =>
        this.Period.Id == default;

    protected override PeriodFormViewModel Self => this;

    protected override void EnableChangeTracking()
    {
        this.TrackChanges(vm => vm.StartMonth, vm => vm.Period.StartMonth);
        this.TrackChanges(vm => vm.StartYear, vm => vm.Period.StartYear);
        this.TrackChanges(vm => vm.EndMonth, vm => vm.Period.EndMonth);
        this.TrackChanges(vm => vm.EndYear, vm => vm.Period.EndYear);
        this.TrackChanges(vm => vm.NumberOfEpisodes, vm => vm.Period.NumberOfEpisodes);
        this.TrackChanges(vm => vm.IsSingleDayRelease, vm => vm.Period.IsSingleDayRelease);
        this.TrackChanges(vm => vm.RottenTomatoesLink, vm => vm.Period.RottenTomatoesLink.EmptyIfNull());
        this.TrackChanges(vm => vm.PosterUrl, vm => vm.Period.PosterUrl.EmptyIfNull());

        base.EnableChangeTracking();
    }

    protected override IObservable<Period> OnSave()
    {
        this.Period.StartMonth = this.StartMonth;
        this.Period.StartYear = this.StartYear;
        this.Period.EndMonth = this.EndMonth;
        this.Period.EndYear = this.EndYear;
        this.Period.NumberOfEpisodes = this.NumberOfEpisodes;
        this.Period.IsSingleDayRelease = this.IsSingleDayRelease;
        this.Period.RottenTomatoesLink = this.RottenTomatoesLink.NullIfEmpty();
        this.Period.PosterUrl = this.PosterUrl.NullIfEmpty();

        return Observable.Return(this.Period);
    }

    protected override IObservable<Period?> OnDelete() =>
        Observable.Return(this.Period);

    protected override void CopyProperties()
    {
        this.StartMonth = this.Period.StartMonth;
        this.StartYear = this.Period.StartYear;
        this.EndMonth = this.Period.EndMonth;
        this.EndYear = this.Period.EndYear;
        this.NumberOfEpisodes = this.Period.NumberOfEpisodes;
        this.IsSingleDayRelease = this.Period.IsSingleDayRelease;
        this.RottenTomatoesLink = this.Period.RottenTomatoesLink.EmptyIfNull();
        this.PosterUrl = this.Period.PosterUrl.EmptyIfNull();
    }

    private void InitializeValueDependencies()
    {
        this.WhenAnyValue(
                vm => vm.StartMonth,
                vm => vm.IsSingleDayRelease,
                (month, isSingleDayRelease) => (Month: month, IsSingleDayRelease: isSingleDayRelease))
            .Where(value => value.IsSingleDayRelease)
            .Select(value => value.Month)
            .BindTo(this, vm => vm.EndMonth);

        this.WhenAnyValue(
                vm => vm.StartYear,
                vm => vm.IsSingleDayRelease,
                (year, isSingleDayRelease) => (Year: year, IsSingleDayRelease: isSingleDayRelease))
            .Where(value => value.IsSingleDayRelease)
            .Select(value => value.Year)
            .BindTo(this, vm => vm.EndYear);
    }
}

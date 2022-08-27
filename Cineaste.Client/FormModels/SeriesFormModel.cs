namespace Cineaste.Client.FormModels;

using static Cineaste.Basic.Constants;

public sealed class SeriesFormModel : TitledFormModel<SeriesModel>
{
    private readonly ListKindModel defaultKind;
    private readonly ObservableCollection<ISeriesComponentFormModel> components = new();

    private readonly string defaultSeasonTitle;
    private readonly string defaultSeasonOriginalTitle;

    public SeriesWatchStatus WatchStatus { get; set; }
    public SeriesReleaseStatus ReleaseStatus { get; set; }

    public ListKindModel Kind { get; set; }

    public ReadOnlyObservableCollection<ISeriesComponentFormModel> Components { get; }

    public string ImdbId { get; set; } = String.Empty;
    public string RottenTomatoesId { get; set; } = String.Empty;

    public SeriesFormModel(
        IReadOnlyCollection<ListKindModel> availableKinds,
        string defaultSeasonTitle,
        string defaultSeasonOriginalTitle)
    {
        ArgumentNullException.ThrowIfNull(availableKinds);
        ArgumentNullException.ThrowIfNull(defaultSeasonTitle);
        ArgumentNullException.ThrowIfNull(defaultSeasonOriginalTitle);

        if (availableKinds.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(availableKinds), $"{nameof(availableKinds)} is empty");
        }

        this.defaultKind = availableKinds.First();
        this.Kind = this.defaultKind;

        this.Components = new(this.components);

        this.defaultSeasonTitle = defaultSeasonTitle;
        this.defaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
    }

    public SeasonFormModel AddSeason()
    {
        int lastYear = this.GetLastYear();
        string seasonNumber = (this.components.OfType<SeasonFormModel>().Count() + 1).ToString();

        var season = new SeasonFormModel(
            this.defaultSeasonTitle.Replace(SeasonTitleNumberPlaceholder, seasonNumber),
            this.defaultSeasonOriginalTitle.Replace(SeasonTitleNumberPlaceholder, seasonNumber),
            new DateOnly(lastYear + 1, 1, 1),
            this.components.Count > 0 ? this.components[^1].Channel : String.Empty,
            this.GetSequenceNumberForComponent,
            this.GetLastSequenceNumber);

        this.components.Add(season);

        return season;
    }

    public SpecialEpisodeFormModel AddSpecialEpisode()
    {
        int lastYear = this.GetLastYear();

        var episode = new SpecialEpisodeFormModel(
            new DateOnly(lastYear + 1, 1, 1),
            this.components.Count > 0 ? this.components[^1].Channel : String.Empty,
            this.GetSequenceNumberForComponent,
            this.GetLastSequenceNumber);

        this.components.Add(episode);

        return episode;
    }

    public void RemoveComponent(ISeriesComponentFormModel component)
    {
        this.components.Remove(component);

        foreach (var c in this.components)
        {
            if (c.SequenceNumber > component.SequenceNumber)
            {
                c.SequenceNumber--;
            }
        }
    }

    public void MoveComponentUp(ISeriesComponentFormModel component)
    {
        if (component.SequenceNumber == 1)
        {
            return;
        }

        int index = component.SequenceNumber - 1;

        var previousComponent = this.components[index - 1];

        previousComponent.SequenceNumber++;
        component.SequenceNumber--;

        this.components[index - 1] = component;
        this.components[index] = previousComponent;
    }

    public void MoveComponentDown(ISeriesComponentFormModel component)
    {
        if (component.SequenceNumber == this.components.Count)
        {
            return;
        }

        int index = component.SequenceNumber - 1;

        var nextComponent = this.components[index + 1];

        nextComponent.SequenceNumber--;
        component.SequenceNumber++;

        this.components[index + 1] = component;
        this.components[index] = nextComponent;
    }

    public SeriesRequest ToRequest(Guid listId) =>
        new(
            listId,
            this.ToTitleRequests(this.Titles),
            this.ToTitleRequests(this.OriginalTitles),
            this.WatchStatus,
            this.ReleaseStatus,
            this.Kind.Id,
            this.Components.OfType<SeasonFormModel>().Select(season => season.ToRequest()).ToImmutableList(),
            this.Components.OfType<SpecialEpisodeFormModel>().Select(episode => episode.ToRequest()).ToImmutableList(),
            this.ImdbId,
            this.RottenTomatoesId);

    protected override void CopyFromModel()
    {
        var series = this.BackingModel;

        this.CopyTitles(series);

        this.WatchStatus = series?.WatchStatus ?? SeriesWatchStatus.NotWatched;
        this.ReleaseStatus = series?.ReleaseStatus ?? SeriesReleaseStatus.NotStarted;
        this.Kind = series?.Kind ?? this.defaultKind;
        this.ImdbId = series?.ImdbId ?? String.Empty;
        this.RottenTomatoesId = series?.RottenTomatoesId ?? String.Empty;

        this.components.Clear();

        foreach (var component in series?.Components ?? Enumerable.Empty<ISeriesComponentModel>())
        {
            switch (component)
            {
                case SeasonModel season:
                    this.AddSeason().CopyFrom(season);
                    break;
                case SpecialEpisodeModel episode:
                    this.AddSpecialEpisode().CopyFrom(episode);
                    break;
                default:
                    Match.ImpossibleType(component);
                    break;
            }
        }
    }

    private int GetSequenceNumberForComponent(ISeriesComponentFormModel component) =>
        this.components.Contains(component)
            ? this.components.IndexOf(component) + 1
            : (this.components.Max(c => c.SequenceNumber as int?) ?? 0) + 1;

    private int GetLastSequenceNumber() =>
        this.components.Count;

    private int GetLastYear()
    {
        if (this.components.Count == 0)
        {
            return DateTime.Now.Year - 1;
        }

        return this.components
            .Max(component => component switch
            {
                SeasonFormModel season => season.Periods.Max(period => period.EndYear),
                SpecialEpisodeFormModel episode => episode.Year,
                _ => Match.ImpossibleType<int>(component)
            });
    }
}

namespace Cineaste.Client.FormModels;

public sealed class SeriesFormModel : TitledFormModel<SeriesModel, SeriesRequest>
{
    private readonly ListKindModel defaultKind;
    private readonly ObservableCollection<ISeriesComponentFormModel> components = new();

    public SeriesWatchStatus WatchStatus { get; set; }
    public SeriesReleaseStatus ReleaseStatus { get; set; }

    public ListKindModel Kind { get; set; }

    public ReadOnlyObservableCollection<ISeriesComponentFormModel> Components { get; }

    public string ImdbId { get; set; } = String.Empty;
    public string RottenTomatoesId { get; set; } = String.Empty;

    public SeriesFormModel(IReadOnlyCollection<ListKindModel> availableKinds)
    {
        ArgumentNullException.ThrowIfNull(availableKinds);

        if (availableKinds.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(availableKinds), $"{nameof(availableKinds)} is empty");
        }

        this.defaultKind = availableKinds.First();
        this.Kind = this.defaultKind;

        this.Components = new(this.components);
    }

    public SeasonFormModel AddSeason()
    {
        var season = new SeasonFormModel(this.GetNextSequenceNumber);
        this.components.Add(season);

        return season;
    }

    public void RemoveSeason(SeasonFormModel season) =>
        this.components.Remove(season);

    public SpecialEpisodeFormModel AddSpecialEpisode()
    {
        var episode = new SpecialEpisodeFormModel(this.GetNextSequenceNumber);
        this.components.Add(episode);

        return episode;
    }

    public void RemoveSpecialEpisode(SpecialEpisodeFormModel episode) =>
        this.components.Remove(episode);

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

    private int GetNextSequenceNumber() =>
        this.components.Max(c => c.SequenceNumber) + 1;
}

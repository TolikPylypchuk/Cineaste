namespace Cineaste.Client.FormModels;

public sealed class SpecialEpisodeFormModel : SeriesComponentFormModel<SpecialEpisodeModel, SpecialEpisodeRequest>
{
    private readonly Func<int> getDefaultSequenceNumber;

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

    public string Channel { get; set; } = String.Empty;

    public int Month { get; set; }
    public int Year { get; set; }

    public string RottenTomatoesId { get; set; } = String.Empty;

    public override string Years =>
        this.Year.ToString();

    public SpecialEpisodeFormModel(Func<int> getDefaultSequenceNumber) =>
        this.getDefaultSequenceNumber = getDefaultSequenceNumber;

    protected override void CopyFromModel()
    {
        var episode = this.BackingModel;

        this.CopyTitles(episode);

        this.IsWatched = episode?.IsWatched ?? false;
        this.IsReleased = episode?.IsReleased ?? true;

        this.Channel = episode?.Channel ?? String.Empty;

        var today = DateTime.Now;

        this.Month = episode?.Month ?? today.Month;
        this.Year = episode?.Year ?? today.Year;

        this.RottenTomatoesId = episode?.RottenTomatoesId ?? String.Empty;
        this.SequenceNumber = episode?.SequenceNumber ?? this.getDefaultSequenceNumber();
    }
}

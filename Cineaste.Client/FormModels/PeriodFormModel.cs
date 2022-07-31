namespace Cineaste.Client.FormModels;

public sealed class PeriodFormModel : FormModel<PeriodModel, PeriodRequest>
{
    public int StartMonth { get; set; }
    public int StartYear { get; set; }

    public int EndMonth { get; set; }
    public int EndYear { get; set; }

    public int EpisodeCount { get; set; }
    public bool IsSingleDayRelease { get; set; }

    public string RottenTomatoesId { get; set; } = String.Empty;

    protected override void CopyFromModel()
    {
        var period = this.BackingModel;
        var today = DateTime.Now;

        this.StartMonth = period?.StartMonth ?? today.Month;
        this.StartYear = period?.StartYear ?? today.Year;

        this.EndMonth = period?.EndMonth ?? today.Month;
        this.EndYear = period?.EndYear ?? today.Year;

        this.EpisodeCount = period?.EpisodeCount ?? 1;
        this.IsSingleDayRelease = period?.IsSingleDayRelease ?? false;

        this.RottenTomatoesId = period?.RottenTomatoesId ?? String.Empty;
    }
}

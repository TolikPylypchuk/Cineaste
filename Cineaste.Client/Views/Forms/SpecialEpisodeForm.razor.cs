namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.SeriesForm;

public partial class SpecialEpisodeForm
{
    [Parameter]
    public SpecialEpisodeModel? Episode { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    [Parameter]
    public EventCallback Delete { get; set; }

    [Parameter]
    public SpecialEpisodeFormModel FormModel { get; set; } = null!;

    private string FormTitle { get; set; } = String.Empty;

    private bool CanChangeIsWatched { get; set; } = true;
    private bool CanChangeIsReleased { get; set; } = true;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.UpdateFormTitle();
        this.OnMonthYearChanged();
    }

    private void SetPropertyValues()
    {
        this.FormModel.CopyFrom(this.Episode);
        this.UpdateFormTitle();
        this.OnMonthYearChanged();
    }

    private void AddTitle(ObservableCollection<string> titles) =>
        titles.Add(String.Empty);

    private void UpdateFormTitle() =>
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;

    private void OnMonthYearChanged()
    {
        var today = DateTime.Now;

        if (this.FormModel.Year < today.Year ||
            (this.FormModel.Year == today.Year && this.FormModel.Month < today.Month))
        {
            this.FormModel.IsReleased = true;
            this.CanChangeIsWatched = true;
            this.CanChangeIsReleased = false;
        } else if (this.FormModel.Year > today.Year ||
            (this.FormModel.Year == today.Year && this.FormModel.Month > today.Month))
        {
            this.FormModel.IsWatched = false;
            this.FormModel.IsReleased = false;
            this.CanChangeIsWatched = false;
            this.CanChangeIsReleased = false;
        } else
        {
            this.CanChangeIsWatched = true;
            this.CanChangeIsReleased = true;
        }
    }

    private void OnIsWatchedChanged()
    {
        if (this.FormModel.IsWatched)
        {
            this.FormModel.IsReleased = true;
        }
    }

    private void OnIsReleasedChanged()
    {
        if (!this.FormModel.IsReleased)
        {
            this.FormModel.IsWatched = false;
        }
    }

    private void GoToSeries() =>
        this.Dispatcher.Dispatch(new GoToSeriesAction());

    private void Cancel() =>
        this.SetPropertyValues();
}

using Cineaste.Client.Store.Forms.SeriesForm;
using Cineaste.Client.Store.ListPage;

namespace Cineaste.Client.Components.Forms;

public partial class SpecialEpisodeForm
{
    [Parameter]
    public SpecialEpisodeModel? Episode { get; set; }

    [Parameter]
    public override SpecialEpisodeFormModel FormModel { get; set; } = null!;

    [Parameter]
    public EventCallback Delete { get; set; }

    [Parameter]
    public EventCallback GoToNextComponent { get; set; }

    [Parameter]
    public EventCallback GoToPreviousComponent { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    private bool CanChangeIsWatched { get; set; } = true;
    private bool CanChangeIsReleased { get; set; } = true;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        this.FormModel.TitlesUpdated += (sender, e) => this.UpdateFormTitle();
        this.FormModel.OriginalTitlesUpdated += (sender, e) => this.StateHasChanged();

        this.SubsribeToSuccessfulResult<SetSpecialEpisodePosterResultAction>(this.OnPosterUpdated);
        this.SubsribeToSuccessfulResult<RemoveSpecialEpisodePosterResultAction>(this.OnPosterUpdated);

        this.UpdateFormTitle();
        this.OnMonthYearChanged();
    }

    protected override object? CreateSetPosterAction(Guid _, PosterRequest request) =>
        this.FormModel.BackingModel is { Id: var episodeId } && this.State.Value.Model is { Id: var seriesId }
            ? new SetSpecialEpisodePosterAction(seriesId, episodeId, request)
            : null;

    protected override object? CreateRemovePosterAction(Guid _) =>
        this.FormModel.BackingModel is { Id: var episodeId } && this.State.Value.Model is { Id: var seriesId }
            ? new RemoveSpecialEpisodePosterAction(seriesId, episodeId)
            : null;

    protected override void UpdateFormModel()
    {
        if (this.Episode is not null && this.State.Value.Model is { SpecialEpisodes: var episodes })
        {
            this.FormModel.CopyFrom(episodes.FirstOrDefault(e => e.Id == this.Episode.Id));
        }
    }

    private void SetPropertyValues()
    {
        this.FormModel.CopyFrom(this.Episode);
        this.UpdateFormTitle();
        this.OnMonthYearChanged();
    }

    private void UpdateFormTitle()
    {
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;
        this.StateHasChanged();
    }

    private bool HasRottenTomatoesId() =>
        !String.IsNullOrEmpty(this.FormModel.RottenTomatoesId);


    private void OnMonthYearChanged(DateTime? date)
    {
        this.FormModel.Date = date;
        this.OnMonthYearChanged();
    }

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

    private void OnIsWatchedChanged(bool isWatched)
    {
        this.FormModel.IsWatched = isWatched;
        if (isWatched)
        {
            this.FormModel.IsReleased = true;
        }
    }

    private void OnIsReleasedChanged(bool isReleased)
    {
        this.FormModel.IsReleased = isReleased;
        if (!isReleased)
        {
            this.FormModel.IsWatched = false;
        }
    }

    private async Task OpenPosterDialog()
    {
        if (this.FormModel.BackingModel is not null)
        {
            await this.OpenPosterDialog(this.FormTitle);
        }
    }

    private async Task RemovePoster() =>
        await this.RemovePoster(
            "SpecialEpisodeForm.RemovePosterDialog.Title", "SpecialEpisodeForm.RemovePosterDialog.Body");

    private Task OnGoToNextComponent() =>
        this.WithValidation(r => this.GoToNextComponent.InvokeAsync());

    private Task OnGoToPreviousComponent() =>
        this.WithValidation(r => this.GoToPreviousComponent.InvokeAsync());

    private void GoToSeries() =>
        this.WithValidation(r => this.Dispatcher.Dispatch(new GoToSeriesAction()));

    private void Close() =>
        this.Dispatcher.Dispatch(new CloseItemAction());

    private void Cancel()
    {
        this.SetPropertyValues();
        this.ClearValidation();
    }
}

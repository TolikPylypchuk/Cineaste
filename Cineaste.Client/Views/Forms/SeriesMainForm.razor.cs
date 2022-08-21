namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.SeriesForm;

public partial class SeriesMainForm
{
    [Parameter]
    public Guid ListId { get; set; }

    [Parameter]
    public ListItemModel? ListItem { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    [Parameter]
    public SeriesFormModel FormModel { get; set; } = null!;

    [Parameter]
    public string FormTitle { get; set; } = String.Empty;

    [Parameter]
    public EventCallback FormTitleUpdated { get; set; }

    [Parameter]
    public EventCallback Save { get; set; }

    [Parameter]
    public EventCallback Cancel { get; set; }

    private ImmutableArray<SeriesWatchStatus> AllWatchStatuses { get; } =
        Enum.GetValues<SeriesWatchStatus>().ToImmutableArray();

    private ImmutableArray<SeriesReleaseStatus> AllReleaseStatuses { get; } =
        Enum.GetValues<SeriesReleaseStatus>().ToImmutableArray();

    private PropertyValidator<SeriesRequest, ImmutableList<TitleRequest>>? TitlesValidator { get; set; }
    private PropertyValidator<SeriesRequest, ImmutableList<TitleRequest>>? OriginalTitlesValidator { get; set; }
    private PropertyValidator<SeriesRequest, SeriesWatchStatus>? WatchStatusValidator { get; set; }
    private PropertyValidator<SeriesRequest, ImmutableList<SeasonRequest>>? SeasonsValidator { get; set; }
    private PropertyValidator<SeriesRequest, string>? ImdbIdValidator { get; set; }
    private PropertyValidator<SeriesRequest, string>? RottenTomatoesIdValidator { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.InitializeValidators();
    }

    private void InitializeValidators()
    {
        var validator = SeriesRequest.CreateValidator();

        this.TitlesValidator = PropertyValidator.Create(validator, (SeriesRequest req) => req.Titles, this);
        this.OriginalTitlesValidator = PropertyValidator.Create(
            validator, (SeriesRequest req) => req.OriginalTitles, this);

        this.WatchStatusValidator = PropertyValidator.Create(validator, (SeriesRequest req) => req.WatchStatus, this);
        this.SeasonsValidator = PropertyValidator.Create(validator, (SeriesRequest req) => req.Seasons, this);

        this.ImdbIdValidator = PropertyValidator.Create(validator, (SeriesRequest req) => req.ImdbId, this);
        this.RottenTomatoesIdValidator = PropertyValidator.Create(
            validator, (SeriesRequest req) => req.RottenTomatoesId, this);
    }

    private void FetchSeries()
    {
        if (this.ListItem is not null)
        {
            this.Dispatcher.Dispatch(new FetchSeriesAction(
                this.ListItem.Id, this.State.Value.AvailableKinds, this.State.Value.ListConfiguration));
        }
    }

    private void AddTitle(ObservableCollection<string> titles) =>
        titles.Add(String.Empty);

    private void AddSeason()
    {
        var season = this.FormModel.AddSeason();
        this.OpenSeriesComponentForm(season);
    }

    private void AddSpecialEpisode()
    {
        var episode = this.FormModel.AddSpecialEpisode();
        this.OpenSeriesComponentForm(episode);
    }

    private bool CanMoveUp(ISeriesComponentFormModel component) =>
        component.SequenceNumber != 1;

    private void MoveUp(ISeriesComponentFormModel component) =>
        this.FormModel.MoveComponentUp(component);

    private bool CanMoveDown(ISeriesComponentFormModel component) =>
        component.SequenceNumber != this.FormModel.Components.Count;

    private void MoveDown(ISeriesComponentFormModel component) =>
        this.FormModel.MoveComponentDown(component);

    private async Task UpdateFormTitle() =>
        await this.FormTitleUpdated.InvokeAsync();

    private void OpenSeriesComponentForm(ISeriesComponentFormModel component) =>
        this.Dispatcher.Dispatch(new OpenSeriesComponentFormAction(component));

    private bool HasImdbId() =>
        !String.IsNullOrEmpty(this.FormModel.ImdbId);

    private bool HasRottenTomatoesId() =>
        !String.IsNullOrEmpty(this.FormModel.RottenTomatoesId);

    private async Task Delete()
    {
        bool? delete = await this.DialogService.Confirm(
            this.Loc["SeriesForm.DeleteDialog.Body"],
            this.Loc["SeriesForm.DeleteDialog.Title"],
            new()
            {
                OkButtonText = this.Loc["Confirmation.Confirm"],
                CancelButtonText = this.Loc["Confirmation.Cancel"],
                CloseDialogOnEsc = true,
                CloseDialogOnOverlayClick = true
            });

        if (delete == true && this.ListItem is { Id: var id })
        {
            this.Dispatcher.Dispatch(new DeleteSeriesAction(id));
        }
    }
}

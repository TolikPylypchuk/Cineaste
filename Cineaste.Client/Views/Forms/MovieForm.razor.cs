namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.MovieForm;

public partial class MovieForm
{
    private ConfirmationDialog? deleteConfirmationDialog;

    [Parameter]
    public Guid ListId { get; set; }

    [Parameter]
    public ListItemModel? ListItem { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    private bool CanChangeIsWatched { get; set; } = true;
    private bool CanChangeIsReleased { get; set; } = true;

    private bool IsSaving =>
        this.State.Value.Create.IsInProgress || this.State.Value.Update.IsInProgress;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        this.FormModel = new(this.ListId, this.State.Value.AvailableKinds);

        this.FormModel.TitlesUpdated += (sender, e) => this.UpdateFormTitle();
        this.FormModel.OriginalTitlesUpdated += (sender, e) => this.StateHasChanged();

        this.SetPropertyValues();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            this.SubsribeToSuccessfulResult<FetchMovieResultAction>(this.SetPropertyValues);
            this.SubsribeToSuccessfulResult<CreateMovieResultAction>(this.OnMovieCreated);
            this.SubsribeToSuccessfulResult<UpdateMovieResultAction>(this.OnMovieUpdated);
        }
    }

    private void FetchMovie()
    {
        if (this.ListItem is not null)
        {
            this.Dispatcher.Dispatch(new FetchMovieAction(this.ListItem.Id, this.State.Value.AvailableKinds));
        }
    }

    private void SetPropertyValues()
    {
        this.FormModel.CopyFrom(this.State.Value.Model);
        this.UpdateFormTitle();
        this.OnYearChanged(this.State.Value.Model?.Year ?? DateTime.Now.Year);
    }

    private void AddTitle(ICollection<string> titles) =>
        titles.Add(String.Empty);

    private void UpdateFormTitle()
    {
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;
        this.StateHasChanged();
    }

    private void OnYearChanged(int year)
    {
        this.FormModel.Year = year;
        int currentYear = DateTime.Now.Year;

        if (this.FormModel.Year < currentYear)
        {
            this.FormModel.IsReleased = true;
            this.CanChangeIsWatched = true;
            this.CanChangeIsReleased = false;
        } else if (this.FormModel.Year > currentYear)
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

    private bool HasImdbId() =>
        !String.IsNullOrEmpty(this.FormModel.ImdbId);

    private bool HasRottenTomatoesId() =>
        !String.IsNullOrEmpty(this.FormModel.RottenTomatoesId);

    private void Save() =>
        this.WithValidation(request =>
            this.Dispatcher.Dispatch(this.ListItem is not null
                ? new UpdateMovieAction(this.ListItem.Id, request)
                : new CreateMovieAction(request)));

    private void Cancel()
    {
        this.SetPropertyValues();
        this.ClearValidation();
    }

    private async Task Delete()
    {
        if (this.deleteConfirmationDialog is null)
        {
            return;
        }

        bool? delete = await this.deleteConfirmationDialog.RequestConfirmation();

        if (delete == true && this.ListItem is { Id: var id })
        {
            this.Dispatcher.Dispatch(new DeleteMovieAction(id));
        }
    }

    private void OnMovieCreated() =>
        this.SetPropertyValues();

    private void OnMovieUpdated() =>
        this.SetPropertyValues();
}

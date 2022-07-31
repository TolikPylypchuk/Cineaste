namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.MovieForm;

public partial class MovieForm
{
    [Parameter]
    public Guid ListId { get; set; }

    [Parameter]
    public ListItemModel? ListItem { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    private MovieFormModel FormModel { get; set; } = null!;

    private bool CanChangeIsWatched { get; set; } = true;
    private bool CanChangeIsReleased { get; set; } = true;

    private PropertyValidator<MovieRequest, ImmutableList<TitleRequest>>? TitlesValidator { get; set; }
    private PropertyValidator<MovieRequest, ImmutableList<TitleRequest>>? OriginalTitlesValidator { get; set; }
    private PropertyValidator<MovieRequest, int>? YearValidator { get; set; }
    private PropertyValidator<MovieRequest, string>? ImdbIdValidator { get; set; }
    private PropertyValidator<MovieRequest, string>? RottenTomatoesLinkValidator { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.InitializeValidators();

        this.FormModel = new(this.State.Value.AvailableKinds);
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

    private void InitializeValidators()
    {
        var validator = MovieRequest.CreateValidator();

        this.TitlesValidator = PropertyValidator.Create(validator, (MovieRequest req) => req.Titles, this);
        this.OriginalTitlesValidator = PropertyValidator.Create(
            validator, (MovieRequest req) => req.OriginalTitles, this);

        this.YearValidator = PropertyValidator.Create(validator, (MovieRequest req) => req.Year, this);
        this.ImdbIdValidator = PropertyValidator.Create(validator, (MovieRequest req) => req.ImdbId, this);
        this.RottenTomatoesLinkValidator = PropertyValidator.Create(
            validator, (MovieRequest req) => req.RottenTomatoesLink, this);
    }

    private void FetchMovie()
    {
        if (this.ListItem != null)
        {
            this.Dispatcher.Dispatch(new FetchMovieAction(this.ListItem.Id, this.State.Value.AvailableKinds));
        }
    }

    private void SetPropertyValues()
    {
        this.FormModel.CopyFrom(this.State.Value.Model);
        this.UpdateFormTitle();
    }

    private void AddTitle(ObservableCollection<string> titles) =>
        titles.Add(String.Empty);

    private void UpdateFormTitle() =>
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;

    private void OnYearChanged()
    {
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

    private bool HasImdbId() =>
        !String.IsNullOrEmpty(this.FormModel.ImdbId);

    private bool HasRottenTomatoesLink() =>
        !String.IsNullOrEmpty(this.FormModel.RottenTomatoesLink);

    private void Save() =>
        this.WithValidation(() =>
        {
            if (this.ListItem is not null)
            {
                this.Dispatcher.Dispatch(new UpdateMovieAction(
                    this.ListItem.Id, this.FormModel.ToMovieRequest(this.ListId)));
            } else
            {
                this.Dispatcher.Dispatch(new CreateMovieAction(this.FormModel.ToMovieRequest(this.ListId)));
            }
        });

    private void Cancel() =>
        this.SetPropertyValues();

    private async Task Delete()
    {
        bool? delete = await this.DialogService.Confirm(
            this.Loc["MovieForm.DeleteDialog.Body"],
            this.Loc["MovieForm.DeleteDialog.Title"],
            new()
            {
                OkButtonText = this.Loc["Confirmation.Confirm"],
                CancelButtonText = this.Loc["Confirmation.Cancel"],
                CloseDialogOnEsc = true,
                CloseDialogOnOverlayClick = true
            });

        if (delete == true && this.ListItem is { Id: var id })
        {
            this.Dispatcher.Dispatch(new DeleteMovieAction(id));
        }
    }

    private void OnMovieCreated()
    {
        this.SetPropertyValues();
        this.ShowSuccessNotification("MovieForm.CreateMovie.Success", ShortNotificationDuration);
    }

    private void OnMovieUpdated()
    {
        this.SetPropertyValues();
        this.ShowSuccessNotification("MovieForm.UpdateMovie.Success", ShortNotificationDuration);
    }
}

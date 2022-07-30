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

    private ObservableCollection<string> Titles { get; set; } = new();
    private ObservableCollection<string> OriginalTitles { get; set; } = new();

    private TitlesForm<MovieRequest> TitlesForm { get; set; } = null!;
    private TitlesForm<MovieRequest> OriginalTitlesForm { get; set; } = null!;

    private bool IsWatched { get; set; }
    private bool IsReleased { get; set; }

    private bool CanChangeIsWatched { get; set; } = true;
    private bool CanChangeIsReleased { get; set; } = true;

    private int Year { get; set; }

    private ListKindModel Kind { get; set; } = null!;

    private string ImdbId { get; set; } = String.Empty;
    private string RottenTomatoesLink { get; set; } = String.Empty;

    private PropertyValidator<MovieRequest, ImmutableList<TitleRequest>>? TitlesValidator { get; set; }
    private PropertyValidator<MovieRequest, ImmutableList<TitleRequest>>? OriginalTitlesValidator { get; set; }
    private PropertyValidator<MovieRequest, int>? YearValidator { get; set; }
    private PropertyValidator<MovieRequest, string>? ImdbIdValidator { get; set; }
    private PropertyValidator<MovieRequest, string>? RottenTomatoesLinkValidator { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.InitializeValidators();
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
        var movie = this.State.Value.Model;

        this.Titles = movie?.Titles?.Select(title => title.Name)?.ToObservableCollection()
            ?? new() { String.Empty };

        this.OriginalTitles = movie?.OriginalTitles?.Select(title => title.Name)?.ToObservableCollection()
            ?? new() { String.Empty };

        this.IsWatched = movie?.IsWatched ?? false;
        this.IsReleased = movie?.IsReleased ?? true;
        this.Year = movie?.Year ?? DateTime.Now.Year;
        this.Kind = movie?.Kind ?? this.State.Value.AvailableKinds.FirstOrDefault()!;
        this.ImdbId = movie?.ImdbId ?? String.Empty;
        this.RottenTomatoesLink = movie?.RottenTomatoesLink ?? String.Empty;

        this.UpdateFormTitle();
    }

    private void AddTitle(ObservableCollection<string> titles) =>
        titles.Add(String.Empty);

    private void UpdateFormTitle() =>
        this.FormTitle = this.Titles.FirstOrDefault() ?? String.Empty;

    private void OnYearChanged()
    {
        int currentYear = DateTime.Now.Year;

        if (this.Year < currentYear)
        {
            this.IsReleased = true;
            this.CanChangeIsWatched = true;
            this.CanChangeIsReleased = false;
        } else if (this.Year > currentYear)
        {
            this.IsWatched = false;
            this.IsReleased = false;
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
        if (this.IsWatched)
        {
            this.IsReleased = true;
        }
    }

    private void OnIsReleasedChanged()
    {
        if (!this.IsReleased)
        {
            this.IsWatched = false;
        }
    }

    private void Save() =>
        this.WithValidation(() =>
        {
            if (this.ListItem is not null)
            {
                this.Dispatcher.Dispatch(new UpdateMovieAction(this.ListItem.Id, this.CreateMovieRequest()));
            } else
            {
                this.Dispatcher.Dispatch(new CreateMovieAction(this.CreateMovieRequest()));
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

    private MovieRequest CreateMovieRequest() =>
        new(
            this.ListId,
            this.TitlesForm.TitlesRequests,
            this.OriginalTitlesForm.TitlesRequests,
            this.Year,
            this.IsWatched,
            this.IsReleased,
            this.Kind.Id,
            this.ImdbId,
            this.RottenTomatoesLink);
}

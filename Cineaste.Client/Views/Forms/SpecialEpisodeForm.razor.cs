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

    [Parameter]
    public EventCallback GoToNextComponent { get; set; }

    [Parameter]
    public EventCallback GoToPreviousComponent { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    private bool CanChangeIsWatched { get; set; } = true;
    private bool CanChangeIsReleased { get; set; } = true;

    private PropertyValidator<SpecialEpisodeRequest, ImmutableList<TitleRequest>>? TitlesValidator { get; set; }
    private PropertyValidator<SpecialEpisodeRequest, ImmutableList<TitleRequest>>? OriginalTitlesValidator { get; set; }

    private PropertyValidator<SpecialEpisodeRequest, int>? YearValidator { get; set; } = null!;
    private PropertyValidator<SpecialEpisodeRequest, string>? ChannelValidator { get; set; }
    private PropertyValidator<SpecialEpisodeRequest, string>? RottenTomatoesIdValidator { get; set; } = null!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        this.InitializeValidators();
        this.UpdateFormTitle();
        this.OnMonthYearChanged();
    }

    private void InitializeValidators()
    {
        var validator = SpecialEpisodeRequest.CreateValidator();

        this.TitlesValidator = PropertyValidator.Create(validator, (SpecialEpisodeRequest req) => req.Titles, this);
        this.OriginalTitlesValidator = PropertyValidator.Create(
            validator, (SpecialEpisodeRequest req) => req.OriginalTitles, this);

        this.YearValidator = PropertyValidator.Create(validator, (SpecialEpisodeRequest req) => req.Year, this);
        this.ChannelValidator = PropertyValidator.Create(validator, (SpecialEpisodeRequest req) => req.Channel, this);
        this.RottenTomatoesIdValidator = PropertyValidator.Create(
            validator, (SpecialEpisodeRequest req) => req.RottenTomatoesId, this);
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

    private Task OnGoToNextComponent() =>
        this.WithValidation(this.GoToNextComponent.InvokeAsync);

    private Task OnGoToPreviousComponent() =>
        this.WithValidation(this.GoToPreviousComponent.InvokeAsync);

    private void GoToSeries() =>
        this.WithValidation(() => this.Dispatcher.Dispatch(new GoToSeriesAction()));

    private void Cancel()
    {
        this.SetPropertyValues();
        this.ClearValidation();
    }
}

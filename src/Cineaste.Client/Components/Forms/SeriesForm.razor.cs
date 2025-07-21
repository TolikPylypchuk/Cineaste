using Cineaste.Client.Store.Forms.SeriesForm;

namespace Cineaste.Client.Components.Forms;

public partial class SeriesForm
{
    [Parameter]
    public required Guid ListId { get; set; }

    [Parameter]
    public required ListConfigurationModel ListConfiguration { get; set; }

    [Parameter]
    public required ImmutableList<ListKindModel> AvailableKinds { get; set; }

    [Parameter]
    public ListItemModel? ListItem { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    private SeriesFormModel FormModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        this.SubsribeToSuccessfulResult<FetchSeriesResultAction>(this.SetPropertyValues);
        this.SubsribeToSuccessfulResult<AddSeriesResultAction>(this.OnSeriesCreated);
        this.SubsribeToSuccessfulResult<UpdateSeriesResultAction>(this.OnSeriesUpdated);

        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        var config = this.ListConfiguration;
        this.FormModel = new(
            this.ListId, this.AvailableKinds, config.DefaultSeasonTitle, config.DefaultSeasonOriginalTitle);

        this.FormModel.TitlesUpdated += (sender, e) => this.UpdateFormTitle();
        this.FormModel.OriginalTitlesUpdated += (sender, e) => this.StateHasChanged();

        this.SetPropertyValues();
    }

    private void SetPropertyValues()
    {
        this.FormModel.CopyFrom(this.State.Value.Model);
        this.UpdateFormTitle();
    }

    private void UpdateFormTitle()
    {
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;
        this.StateHasChanged();
    }

    private void RemoveComponent(ISeriesComponentFormModel component)
    {
        this.FormModel.RemoveComponent(component);
        this.Dispatcher.Dispatch(new GoToSeriesAction());
    }

    private void GoToNextComponent() =>
        this.Dispatcher.Dispatch(new GoToNextSeriesComponentAction(this.FormModel));

    private void GoToPreviousComponent() =>
        this.Dispatcher.Dispatch(new GoToPreviousSeriesComponentAction(this.FormModel));

    private void Save(SeriesRequest request) =>
        this.Dispatcher.Dispatch(this.ListItem is not null
            ? new UpdateSeriesAction(this.ListItem.Id, request)
            : new AddSeriesAction(request));

    private void Cancel() =>
        this.SetPropertyValues();

    private void OnSeriesCreated() =>
        this.SetPropertyValues();

    private void OnSeriesUpdated() =>
        this.SetPropertyValues();
}

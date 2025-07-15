using Cineaste.Client.Store.Forms.SeriesForm;

namespace Cineaste.Client.Components.Forms;

public partial class SeriesForm
{
    [Parameter]
    public Guid ListId { get; set; }

    [Parameter]
    public ListItemModel? ListItem { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }

    private string FormTitle { get; set; } = String.Empty;

    private SeriesFormModel FormModel { get; set; } = null!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        var config = this.State.Value.ListConfiguration;
        this.FormModel = new(
            this.ListId, this.State.Value.AvailableKinds, config.DefaultSeasonTitle, config.DefaultSeasonOriginalTitle);

        this.FormModel.TitlesUpdated += (sender, e) => this.UpdateFormTitle();
        this.FormModel.OriginalTitlesUpdated += (sender, e) => this.StateHasChanged();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            this.SubsribeToSuccessfulResult<FetchSeriesResultAction>(this.SetPropertyValues);
            this.SubsribeToSuccessfulResult<CreateSeriesResultAction>(this.OnSeriesCreated);
            this.SubsribeToSuccessfulResult<UpdateSeriesResultAction>(this.OnSeriesUpdated);
        }
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
            : new CreateSeriesAction(request));

    private void Cancel() =>
        this.SetPropertyValues();

    private void OnSeriesCreated() =>
        this.SetPropertyValues();

    private void OnSeriesUpdated() =>
        this.SetPropertyValues();
}

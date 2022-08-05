namespace Cineaste.Client.Views.Forms;

using Cineaste.Client.Store.Forms.SeriesForm;

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
            this.State.Value.AvailableKinds, config.DefaultSeasonTitle, config.DefaultSeasonOriginalTitle);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            this.SubsribeToSuccessfulResult<FetchSeriesResultAction>(this.SetPropertyValues);
        }
    }

    private void SetPropertyValues()
    {
        this.FormModel.CopyFrom(this.State.Value.Model);
        this.UpdateFormTitle();
    }

    private void UpdateFormTitle() =>
        this.FormTitle = this.FormModel.Titles.FirstOrDefault() ?? String.Empty;

    private void Save()
    { }

    private void Cancel() =>
        this.SetPropertyValues();
}

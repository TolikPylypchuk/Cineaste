namespace Cineaste.Client.FormModels;

using Cineaste.Shared;

public sealed class CreateListFormModel : FormModelBase<CreateListRequest, object>
{
    private const string DefaultDefaultSeasonTitle = "Season #";

    private string name = String.Empty;

    public string Name
    {
        get => this.name;
        set
        {
            this.name = value;
            this.Handle = ListUtils.CreateHandleFromName(value);
        }
    }

    public string Handle { get; private set; } = String.Empty;

    public SimpleCultureModel Culture { get; set; } =
        new(CultureInfo.InvariantCulture.ToString(), CultureInfo.InvariantCulture.DisplayName);

    public string DefaultSeasonTitle { get; set; } = DefaultDefaultSeasonTitle;
    public string DefaultSeasonOriginalTitle { get; set; } = DefaultDefaultSeasonTitle;

    public CreateListFormModel() =>
        this.FinishInitialization();

    public override CreateListRequest CreateRequest() =>
        new(this.Name, this.Culture.Id, this.DefaultSeasonTitle, this.DefaultSeasonOriginalTitle);

    protected override void CopyFromModel()
    { }
}

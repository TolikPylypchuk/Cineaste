namespace Cineaste.Client.ViewModels;

public sealed class CreateListPageViewModel : ReactiveObject
{
    private static readonly SimpleCultureModel DefaultCultureModel = new(
        CultureInfo.InvariantCulture.ToString(), CultureInfo.InvariantCulture.DisplayName);

    private const string DefaultDefaultSeasonTitle = "Season #";

    private readonly IPageNavigator pageNavigator;

    private readonly SourceCache<SimpleCultureModel, string> allCulturesSource = new(culture => culture.Id);
    private readonly ReadOnlyObservableCollection<SimpleCultureModel> allCultures;

    [Reactive]
    public string Name { get; set; } = String.Empty;

    public string Handle { [ObservableAsProperty] get; } = String.Empty;

    [Reactive]
    public SimpleCultureModel Culture { get; set; } = DefaultCultureModel;

    [Reactive]
    public string DefaultSeasonTitle { get; set; } = DefaultDefaultSeasonTitle;

    [Reactive]
    public string DefaultSeasonOriginalTitle { get; set; } = DefaultDefaultSeasonTitle;

    public ReadOnlyObservableCollection<SimpleCultureModel> AllCultures =>
        this.allCultures;

    public RemoteCall<List<SimpleCultureModel>> GetAllCulturesCall { get; }

    public RemoteCall<SimpleListModel> CreateListCall { get; }

    public CreateListPageViewModel(IRemoteCallFactory remoteCallFactory, IPageNavigator pageNavigator)
    {
        this.pageNavigator = pageNavigator;

        this.GetAllCulturesCall = remoteCallFactory.Create((ICultureApi api) => api.GetAllCultures());

        this.CreateListCall = remoteCallFactory.Create((IListApi api) => api.CreateList(new CreateListRequest(
            this.Name, this.Handle, this.Culture.Id, this.DefaultSeasonTitle, this.DefaultSeasonOriginalTitle)));

        this.WhenAnyValue(vm => vm.Name)
            .Select(this.CreateHandleFromName)
            .ToPropertyEx(this, vm => vm.Handle);

        this.allCulturesSource.Connect()
            .Bind(out this.allCultures)
            .Subscribe();
    }

    public async Task Initialize()
    {
        this.Name = String.Empty;
        this.Culture = DefaultCultureModel;

        await this.LoadCultures();
    }

    public async Task LoadCultures()
    {
        await this.GetAllCulturesCall.Execute();

        if (this.GetAllCulturesCall.Result is not null)
        {
            this.allCulturesSource.Edit(list =>
            {
                list.Clear();
                list.AddOrUpdate(this.GetAllCulturesCall.Result);
            });
        }
    }

    public void GoToHomePage() =>
        this.pageNavigator.GoToHomePage();

    public async Task CreateList()
    {
        await this.CreateListCall.Execute();

        if (this.CreateListCall.Result is not null)
        {
            pageNavigator.GoToListPage(this.CreateListCall.Result.Handle);
        }
    }

    private string CreateHandleFromName(string name) =>
        Uri.EscapeDataString(name.Trim()
            .Replace("&", "-and-")
            .Replace("@", "-at-")
            .Replace("/", String.Empty)
            .Replace("\\", String.Empty)
            .Replace(".", String.Empty)
            .Replace(",", String.Empty)
            .Replace("!", String.Empty)
            .Replace("?", String.Empty)
            .Replace("|", String.Empty)
            .Replace("#", String.Empty)
            .Replace("$", String.Empty)
            .Replace("^", String.Empty)
            .Replace("*", String.Empty)
            .Replace("(", String.Empty)
            .Replace(")", String.Empty)
            .Replace(" ", "-")
            .Replace("\t", "-")
            .Replace("\r\n", "-")
            .Replace("\n", "-")
            .Replace("--", "-"))
            .ToLowerInvariant();
}

namespace Cineaste.Client.ViewModels;

using System.Globalization;

public sealed class CreateListPageViewModel : ReactiveObject
{
    private static readonly SimpleCultureModel DefaultCultureModel = new(
        CultureInfo.InvariantCulture.ToString(), CultureInfo.InvariantCulture.DisplayName);

    private const string DefaultDefaultSeasonTitle = "Season #";

    private readonly IApiExecutorFactory api;
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

    [Reactive]
    public bool FailedLoadingCultures { get; set; }

    [Reactive]
    public bool FailedCreatingList { get; set; }

    public ReadOnlyObservableCollection<SimpleCultureModel> AllCultures =>
        this.allCultures;

    public CreateListPageViewModel(IApiExecutorFactory api, IPageNavigator pageNavigator)
    {
        this.api = api;
        this.pageNavigator = pageNavigator;

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
        this.FailedLoadingCultures = false;

        var response = await this.api.For<ICultureApi>().Fetch(api => api.GetAllCultures());

        if (response is ApiSuccess<List<SimpleCultureModel>> cultures)
        {
            this.allCulturesSource.Edit(list =>
            {
                list.Clear();
                list.AddOrUpdate(cultures.Content);
            });

            this.FailedLoadingCultures = false;
        } else
        {
            this.FailedLoadingCultures = true;
        }
    }

    public void GoToHomePage() =>
        this.pageNavigator.GoToHomePage();

    public async Task CreateList()
    {
        this.FailedCreatingList = false;

        var request = new CreateListRequest(
            this.Name, this.Handle, this.Culture.Id, this.DefaultSeasonTitle, this.DefaultSeasonOriginalTitle);

        var response = await this.api.For<IListApi>().Fetch(api => api.CreateList(request));

        if (response is ApiSuccess<SimpleListModel> list)
        {
            pageNavigator.GoToListPage(list.Content.Handle);
        } else
        {
            this.FailedCreatingList = true;
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

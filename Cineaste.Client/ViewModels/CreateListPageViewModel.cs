namespace Cineaste.Client.ViewModels;

using System.Globalization;

public sealed class CreateListPageViewModel : ReactiveObject
{
    private static readonly ListCultureModel DefaultCultureModel = new(
        CultureInfo.InvariantCulture.ToString(), CultureInfo.InvariantCulture.DisplayName);

    private const string DefaultDefaultSeasonTitle = "Season #";

    private readonly IListService listService;
    private readonly IPageNavigator pageNavigator;

    private readonly SourceCache<ListCultureModel, string> allCulturesSource = new(culture => culture.Id);
    private readonly ReadOnlyObservableCollection<ListCultureModel> allCultures;

    [Reactive]
    public string Name { get; set; } = String.Empty;

    public string Handle { [ObservableAsProperty] get; } = String.Empty;

    [Reactive]
    public ListCultureModel Culture { get; set; } = DefaultCultureModel;

    [Reactive]
    public string DefaultSeasonTitle { get; set; } = DefaultDefaultSeasonTitle;

    [Reactive]
    public string DefaultSeasonOriginalTitle { get; set; } = DefaultDefaultSeasonTitle;

    public ReadOnlyObservableCollection<ListCultureModel> AllCultures =>
        this.allCultures;

    public CreateListPageViewModel(IListService listService, IPageNavigator pageNavigator)
    {
        this.listService = listService;
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

        var cultures = await this.listService.GetAllCultures();

        this.allCulturesSource.Edit(list =>
        {
            list.Clear();
            list.AddOrUpdate(cultures);
        });
    }

    public void GoToHomePage() =>
        this.pageNavigator.GoToHomePage();

    public async Task CreateList()
    {
        var request = new CreateListRequest(
            this.Name, this.Handle, this.Culture.Id, this.DefaultSeasonTitle, this.DefaultSeasonOriginalTitle);

        var model = await this.listService.CreateList(request);

        if (model is not null)
        {
            pageNavigator.GoToListPage(model.Handle);
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
